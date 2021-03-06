using AutoMapper;
using AutoMapper.QueryableExtensions;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Assets.Models;
using ITAssetManager.Web.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Assets
{
    public class AssetService : IAssetService
    {
        private readonly AppDbContext data;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;

        public AssetService(AppDbContext data, IMapper mapper, IMemoryCache cache)
        {
            this.data = data;
            this.mapper = mapper;
            this.cache = cache;
        }

        public bool Add(AssetAddFormServiceModel assetModel)
        {
            var assetData = this.mapper.Map<Asset>(assetModel);

            this.data.Assets.Add(assetData);
            var result = this.data.SaveChanges();

            return result > 0;
        }

        public AssetsQueryServiceModel All(string searchString, string sortOrder, int currentPage, string userId)
        {
            var assetsQuery = this.data
                .Assets
                .AsQueryable();

            if (userId != null)
            {
                assetsQuery = assetsQuery
                       .Where(a => a.AssetUsers.Any(au => au.UserId == userId && au.ReturnDate == null));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                assetsQuery = assetsQuery
                    .Where(a =>
                        a.AssetModel.Brand.Name.ToLower().Contains(searchString.ToLower()) ||
                        a.AssetModel.Name.ToLower().Contains(searchString.ToLower()) ||
                        a.InventoryNr.ToLower().Contains(searchString.ToLower()) ||
                        a.SerialNr.ToLower().Contains(searchString.ToLower()) ||
                        a.Status.Name.ToLower().Contains(searchString.ToLower()) ||
                        a.AssetUsers.Any(au => au.User.UserName.ToLower()
                            .Contains(
                                searchString.ToLower()) &&
                                au.Asset.Status.Name == "In Use" &&
                                au.ReturnDate == null));
            }

            assetsQuery = sortOrder switch
            {
                "brand_desc" => assetsQuery.OrderByDescending(a => a.AssetModel.Brand.Name),
                "model" => assetsQuery.OrderBy(a => a.AssetModel.Name),
                "model_desc" => assetsQuery.OrderByDescending(a => a.AssetModel.Name),
                "status" => assetsQuery.OrderBy(a => a.Status.Name),
                "status_desc" => assetsQuery.OrderByDescending(a => a.Status.Name),
                _ => assetsQuery.OrderBy(a => a.AssetModel.Brand.Name),
            };

            var pages = Pagination.GetPages(assetsQuery, currentPage, ItemsPerPage);

            currentPage = pages.currentPage;
            var lastPage = pages.lastPage;

            var assets = assetsQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(a => new AssetListingServiceModel
                {
                    Id = a.Id,
                    Brand = a.AssetModel.Brand.Name,
                    Model = a.AssetModel.Name,
                    ModelId = a.AssetModel.Id,
                    SerialNr = a.SerialNr,
                    InventoryNr = a.InventoryNr,
                    Status = a.Status.Name,
                    User = a.AssetUsers
                        .Where(au => au.ReturnDate == null)
                        .Select(au => au.User.UserName)
                        .FirstOrDefault(),
                    IsInUse = a.AssetUsers.Any(au => au.ReturnDate == null)
                })
                .ToList();

            return new AssetsQueryServiceModel
            {
                Assets = assets,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = pages.hasPreviousPage,
                HasNextPage = pages.hasNextPage
            };
        }

        public AssetAssignServiceModel GetById(int id, string searchString, string sortOrder, int currentPage)
        {
            var targetAsset = this.data
                .Assets
                .Where(a => a.Id == id)
                .Select(a => new AssetAssignServiceModel
                {
                    Id = a.Id,
                    Model = a.AssetModel.Brand.Name + " " + a.AssetModel.Name,
                    SerialNr = a.SerialNr,
                    InventoryNr = a.InventoryNr,
                    SearchString = searchString,
                    SortOrder = sortOrder,
                    CurrentPage = currentPage
                })
                .FirstOrDefault();

            return targetAsset;
        }

        public bool Assign(string userId, int assetId)
        {
            var userAsset = new UserAsset
            {
                UserId = userId,
                AssetId = assetId,
                AssignDate = DateTime.UtcNow
            };

            var targetAsset = this.data
                .Assets
                .Where(a => a.Id == assetId)
                .FirstOrDefault();

            var targetStatusId = this.data
                .Statuses
                .Where(s => s.Name == AssetTargetAssignStatus)
                .Select(s => s.Id)
                .FirstOrDefault();

            targetAsset.StatusId = targetStatusId;

            this.data.UsersAssets.Add(userAsset);
            var result = this.data.SaveChanges();

            return result > 0;
        }

        public AssetCollectServiceModel GetUserAssetById(int id)
        {
            var targetUserAsset = this.data
                .UsersAssets
                .Where(ua => ua.AssetId == id && ua.ReturnDate == null)
                .ProjectTo<AssetCollectServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();

            return targetUserAsset;
        }

        public bool Collect(string userId, int assetId, DateTime returnDate)
        {
            var targetUserAsset = this.data
                .UsersAssets
                .Where(ua => ua.UserId == userId && ua.AssetId == assetId && ua.ReturnDate == null)
                .FirstOrDefault();

            var targetAsset = this.data
                .Assets
                .Where(a => a.Id == assetId)
                .FirstOrDefault();

            var targetStatusId = this.data
                .Statuses
                .Where(s => s.Name == AssetTargetCollectStatus)
                .Select(s => s.Id)
                .FirstOrDefault();

            targetAsset.StatusId = targetStatusId;
            targetUserAsset.ReturnDate = returnDate.ToUniversalTime();

            var result = this.data.SaveChanges();

            return result > 0;
        }

        public AssetEditFormServiceModel EditById(int id, string searchString, string sortOrder, int currentPage)
        {
            var targetAsset = this.data
                .Assets
                .Where(a => a.Id == id)
                .ProjectTo<AssetEditFormServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();

            targetAsset.SearchString = searchString;
            targetAsset.SortOrder = sortOrder;
            targetAsset.CurrentPage = currentPage;
            targetAsset.Models = this.GetModels();
            targetAsset.Statuses = this.GetStatuses();
            targetAsset.Vendors = this.GetVendors();

            return targetAsset;
        }

        public bool Update(AssetEditFormServiceModel asset)
        {
            var targetAsset = this.data
                .Assets
                .Where(a => a.Id == asset.Id)
                .FirstOrDefault();

            targetAsset.AssetModelId = asset.AssetModelId;
            targetAsset.SerialNr = asset.SerialNr;
            targetAsset.InventoryNr = asset.InventoryNr;
            targetAsset.StatusId = asset.StatusId;
            targetAsset.VendorId = asset.VendorId;
            targetAsset.InvoiceNr = asset.InvoiceNr;
            targetAsset.Price = asset.Price;
            targetAsset.PurchaseDate = asset.PurchaseDate;
            targetAsset.WarranyExpirationDate = asset.WarranyExpirationDate;

            var result = this.data.SaveChanges();

            return result > 0;
        }

        public bool Delete(int id)
        {
            var targetUserAssets = this.data
                .UsersAssets
                .Where(ua => ua.AssetId == id)
                .ToList();

            this.data.UsersAssets.RemoveRange(targetUserAssets);
            this.data.SaveChanges();

            var targetAsset = this.data
                .Assets
                .Where(a => a.Id == id)
                .FirstOrDefault();

            this.data.Assets.Remove(targetAsset);
            var result = this.data.SaveChanges();

            return result > 0;
        }

        public IEnumerable<UserDropdownServiceModel> GetAllUsers()
        {
            const string allUsersCacheKey = "UsersCacheKey";

            var allUses = this.cache.Get<List<UserDropdownServiceModel>>(allUsersCacheKey);

            if (allUses == null)
            {
                allUses = this.data
                           .Users
                           .OrderBy(u => u.UserName)
                           .Select(u => new UserDropdownServiceModel
                           {
                               Id = u.Id,
                               Username = u.UserName
                           })
                           .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(20));

                this.cache.Set(allUsersCacheKey, allUses, cacheOptions);
            }

            return allUses;
        }

        public IEnumerable<AssetModelDropdownServiceModel> GetModels()
        {
            const string allModelsCacheKey = "ModelsCacheKey";

            var allModels = this.cache.Get<List<AssetModelDropdownServiceModel>>(allModelsCacheKey);

            if (allModels == null)
            {
                allModels = this.data
                           .AssetModels
                           .OrderBy(a => a.Name)
                           .Select(a => new AssetModelDropdownServiceModel
                           {
                               Id = a.Id,
                               Name = a.Name
                           })
                           .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                this.cache.Set(allModelsCacheKey, allModels, cacheOptions);
            }

            return allModels;
        }

        public IEnumerable<StatusDropdownServiceModel> GetStatuses()
        {
            const string allStatusesCacheKey = "StatusesCacheKey";

            var allStatuses = this.cache.Get<List<StatusDropdownServiceModel>>(allStatusesCacheKey);

            if (allStatuses == null)
            {
                allStatuses = this.data
                           .Statuses
                           .Where(s => s.Name != AssetTargetAssignStatus)
                           .OrderBy(s => s.Name)
                           .Select(s => new StatusDropdownServiceModel
                           {
                               Id = s.Id,
                               Name = s.Name
                           })
                           .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                this.cache.Set(allStatusesCacheKey, allStatuses, cacheOptions);
            }

            return allStatuses;
        }

        public IEnumerable<VendorDropdownServiceModel> GetVendors()
        {
            const string allVendorsCacheKey = "VendorsCacheKey";

            var allVendors = this.cache.Get<List<VendorDropdownServiceModel>>(allVendorsCacheKey);

            if (allVendors == null)
            {
                allVendors = this.data
                           .Vendors
                           .OrderBy(v => v.Name)
                           .Select(v => new VendorDropdownServiceModel
                           {
                               Id = v.Id,
                               Name = v.Name
                           })

                           .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                this.cache.Set(allVendorsCacheKey, allVendors, cacheOptions);
            }

            return allVendors;
        }

        public bool IsValidAsset(int id)
            => this.data.Assets.Any(a => a.Id == id);

        public bool IsValidUser(string id)
            => this.data.Users.Any(u => u.Id == id);

        public bool IsExistingInventoryNr(string inventoryNr)
            => this.data.Assets.Any(i => i.InventoryNr == inventoryNr);

        public bool IsExistingSerialNr(string serialNr)
            => this.data.Assets.Any(s => s.SerialNr == serialNr);

        public bool IsValidModel(int id)
            => this.data.AssetModels.Any(a => a.Id == id);

        public bool IsValidStatus(int id)
            => this.data.Statuses.Any(s => s.Id == id);

        public bool IsValidVendor(int id)
            => this.data.Vendors.Any(v => v.Id == id);

        public bool IsInUse(int id)
            => this.data
                .Assets
                .Where(a => a.Id == id)
                .SelectMany(a => a.AssetUsers)
                .Any(ua => ua.ReturnDate == null);
    }
}
