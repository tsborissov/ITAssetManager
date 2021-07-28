using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Assets
{
    public interface IAssetService
    {
        int Add(AssetAddFormServiceModel vendorModel);

        AssetsQueryServiceModel All(string searchString, string sortOrder, int currentPage, string userId);

        AssetAssignServiceModel AssignById(int id, string searchString, string sortOrder, int currentPage);

        void Assign(string userId, int assetId);

        AssetCollectServiceModel UserAssetById(int assetId, string searchString, string sortOrder, int currentPage);

        void Collect(string userId, int assetId);

        AssetEditFormServiceModel EditById(int id, string searchString, string sortOrder, int currentPage);

        void Update(AssetEditFormServiceModel asset);

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
