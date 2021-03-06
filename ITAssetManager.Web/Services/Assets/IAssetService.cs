using ITAssetManager.Web.Services.Assets.Models;
using System;
using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Assets
{
    public interface IAssetService
    {
        bool Add(AssetAddFormServiceModel vendorModel);

        AssetsQueryServiceModel All(string searchString, string sortOrder, int currentPage, string userId);

        AssetAssignServiceModel GetById(int id, string searchString, string sortOrder, int currentPage);

        bool Assign(string userId, int assetId);

        AssetCollectServiceModel GetUserAssetById(int id);

        bool Collect(string userId, int assetId, DateTime returnDate);

        AssetEditFormServiceModel EditById(int id, string searchString, string sortOrder, int currentPage);

        bool Update(AssetEditFormServiceModel asset);

        bool Delete(int id);

        IEnumerable<StatusDropdownServiceModel> GetStatuses();

        IEnumerable<AssetModelDropdownServiceModel> GetModels();

        IEnumerable<VendorDropdownServiceModel> GetVendors();

        IEnumerable<UserDropdownServiceModel> GetAllUsers();

        bool IsValidAsset(int id);

        bool IsValidUser(string id);

        bool IsValidModel(int id);

        bool IsValidStatus(int id);

        bool IsValidVendor(int id);

        bool IsExistingSerialNr(string serialNr);

        bool IsExistingInventoryNr(string inventoryNr);

        bool IsInUse(int id);
    }
}
