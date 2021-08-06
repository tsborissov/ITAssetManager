using AutoMapper;
using AutoMapper.QueryableExtensions;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Assets.Models;
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

        public int Add(AssetAddFormServiceModel assetModel)
        {
            var assetData = this.mapper.Map<Asset>(assetModel);

            this.data.Assets.Add(assetData);
            this.data.SaveChanges();

            return assetData.Id;
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

            var itemsCount = assetsQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

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
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < lastPage
            };
        }

        public AssetAssignServiceModel AssignById(int id, string searchString, string sortOrder, int currentPage)
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

        public void Assign(string userId, int assetId)
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
                .Select(s => s.Id )
                .FirstOrDefault();

            targetAsset.StatusId = targetStatusId;

            this.data.UsersAssets.Add(userAsset);
            this.data.SaveChanges();
        }

        public AssetCollectServiceModel UserAssetById(int id)
        {
            var targetUserAsset = this.data
                .UsersAssets
                .Where(ua => ua.AssetId == id && ua.ReturnDate == null)
                .ProjectTo<AssetCollectServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();

            return targetUserAsset;
        }

        public void Collect(string userId, int assetId, DateTime returnDate)
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

            this.data.SaveChanges();
        }

        public AssetEditFormServiceModel EditById(int id, string searchString, string sortOrder, int currentPage)
        {
            var assetData = this.data
                .Assets
                .Where(a => a.Id == id)
                .ProjectTo<AssetEditFormServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();

            assetData.SearchString = searchString;
            assetData.SortOrder = sortOrder;
            assetData.CurrentPage = currentPage;
            assetData.Models = this.GetModels();
            assetData.Statuses = this.GetStatuses();
            assetData.Vendors = this.GetVendors();

            return assetData;
        }

        public void Update(AssetEditFormServiceModel asset)
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

            this.data.SaveChanges();
        }

        public void Delete(int id)
        {
            var targetAsset = this.data
                .Assets
                .Where(a => a.Id == id)
                .FirstOrDefault();

            var targetStatusId = this.data
                .Statuses
                .Where(s => s.Name == "Disposed")
                .Select(s => s.Id)
                .FirstOrDefault();

            if (targetAsset == null)
            {
                throw new ArgumentException("Asset does not exists!");
            }

            if (targetAsset.StatusId != targetStatusId)
            {
                throw new ArgumentException("An Asset with status different than 'Disposed' cannot be deleted!");
            }

            if (this.IsInUse(id))
            {
                throw new InvalidOperationException("Delete related records first!");
            }

            var targetUserAssets = this.data
                .UsersAssets
                .Where(ua => ua.AssetId == id)
                .ToList();

            this.data.UsersAssets.RemoveRange(targetUserAssets);
            this.data.SaveChanges();

            this.data.Assets.Remove(targetAsset);
            this.data.SaveChanges();
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
