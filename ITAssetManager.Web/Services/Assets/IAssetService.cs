using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Assets
{
    public interface IAssetService
    {
        int Add(AssetAddFormServiceModel vendorModel);

        AssetsQueryServiceModel All(string searchString, string sortOrder, int currentPage, string userId);

        AssetAssignServiceModel GetById(int id);

        void Assign(string userId, int assetId);

        AssetCollectServiceModel UserAssetById(int assetId);

        void Collect(string userId, int assetId);

        IEnumerable<StatusDropdownServiceModel> GetStatuses();

        IEnumerable<AssetModelDropdownServiceModel> GetModels();

        IEnumerable<VendorDropdownServiceModel> GetVendors();

        IEnumerable<UserDropdownServiceModel> GetAllUsers();

        bool IsValidModel(int id);

        bool IsValidStatus(int id);

        bool IsValidVendor(int id);

        bool IsExistingSerialNr(string serialNr);

        bool IsExistingInventoryNr(string inventoryNr);
    }
}
