using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Assets
{
    public interface IAssetService
    {
        int Add(AssetAddFormServiceModel vendorModel);

        AssetsQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        IEnumerable<StatusDropdownServiceModel> GetStatuses();

        IEnumerable<AssetModelDropdownServiceModel> GetModels();

        IEnumerable<VendorDropdownServiceModel> GetVendors();

        bool IsValidModel(int id);

        bool IsValidStatus(int id);

        bool IsValidVendor(int id);

        bool IsExistingSerialNr(string serialNr);

        bool IsExistingInventoryNr(string inventoryNr);
    }
}
