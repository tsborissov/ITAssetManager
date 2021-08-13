using ITAssetManager.Web.Services.Vendors.Models;

namespace ITAssetManager.Web.Services.Vendors
{
    public interface IVendorService
    {
        string Add(VendorAddFormServiceModel vendorModel);

        int Update(VendorEditServiceModel vendorModel);

        VendorQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        VendorDetailsServiceModel Details(int id, string sortOrder, string searchString, int currentPage);

        string Delete(int id);

        bool IsExistingName(string name);

        bool IsExistingVat(string vat);

        bool IsExistingVendor(int id);

        bool IsInUse(int id);
    }
}
