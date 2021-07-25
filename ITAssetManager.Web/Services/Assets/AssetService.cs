using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using System.Collections.Generic;
using System.Linq;

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
