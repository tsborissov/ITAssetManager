using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Assets
{
    public class AssetService : IAssetService
    {
        private readonly AppDbContext data;

        public AssetService(AppDbContext data)
            => this.data = data;

        public int Add(AssetAddFormServiceModel assetModel)
        {
            var asset = new Asset
            {
                AssetModelId = assetModel.AssetModelId,
                InventoryNr = assetModel.InventoryNr,
                InvoiceNr = assetModel.InvoiceNr,
                Price = assetModel.Price,
                PurchaseDate = assetModel.PurchaseDate,
                SerialNr = assetModel.SerialNr,
                StatusId = assetModel.StatusId,
                VendorId = assetModel.VendorId,
                WarranyExpirationDate = assetModel.WarranyExpirationDate
            };

            this.data.Assets.Add(asset);
            this.data.SaveChanges();

            return asset.Id;
        }

        public AssetsQueryServiceModel All(string searchString, string sortOrder, int currentPage, string userId)
        {
            var assetsQuery = this.data
                .Assets
                .AsQueryable();

            if (userId != null)
            {
                assetsQuery = assetsQuery
                    .Where(a => a.AssetUsers.Any(u => u.UserId == userId));
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
                    SerialNr = a.SerialNr,
                    InventoryNr = a.InventoryNr,
                    Status = a.Status.Name,
                    User = a.AssetUsers
                    .Where(au => au.ReturnDate == null)
                    .Select(au => au.User.UserName)
                    .FirstOrDefault()
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

        public AssetAssignServiceModel GetById(int id)
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
                .Where(s => s.Name == "In Use")
                .Select(s => s.Id )
                .FirstOrDefault();

            targetAsset.StatusId = targetStatusId;

            this.data.UsersAssets.Add(userAsset);
            this.data.SaveChanges();
        }

        public AssetCollectServiceModel UserAssetById(int assetId)
        {
            var targetUserAsset = this.data
                .UsersAssets
                .Where(ua => ua.AssetId == assetId && ua.ReturnDate == null)
                .Select(ua => new AssetCollectServiceModel
                {
                    AssetId = ua.AssetId,
                    Model = ua.Asset.AssetModel.Name,
                    SerialNr = ua.Asset.SerialNr,
                    InventoryNr = ua.Asset.InventoryNr,
                    AssignDate = ua.AssignDate.ToString("dd.MM.yyyy"),
                    UserId = ua.UserId,
                    UserName = ua.User.UserName
                })
                .FirstOrDefault();

            return targetUserAsset;
        }

        public void Collect(string userId, int assetId)
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
                .Where(s => s.Name == "In Stock")
                .Select(s => s.Id)
                .FirstOrDefault();

            targetAsset.StatusId = targetStatusId;

            targetUserAsset.ReturnDate = DateTime.UtcNow;
            this.data.SaveChanges();
        }

        public IEnumerable<AssetModelDropdownServiceModel> GetModels()
        => this.data
                .AssetModels
                .OrderBy(a => a.Name)
                .Select(a => new AssetModelDropdownServiceModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToList();

        public IEnumerable<StatusDropdownServiceModel> GetStatuses()
        => this.data
                .Statuses
                .OrderBy(s => s.Name)
                .Select(s => new StatusDropdownServiceModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToList();

        public IEnumerable<UserDropdownServiceModel> GetAllUsers()
            => this.data
                .Users
                .OrderBy(u => u.UserName)
                .Select(u => new UserDropdownServiceModel
                {
                    Id = u.Id,
                    Username = u.UserName
                })
                .ToList();

        public IEnumerable<VendorDropdownServiceModel> GetVendors()
         => this.data
                .Vendors
                .OrderBy(v => v.Name)
                .Select(v => new VendorDropdownServiceModel
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .ToList();

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
    }
}
