namespace ITAssetManager.Web.Services.Vendors
{
    public interface IVendorService
    {
        int Add(VendorAddFormServiceModel vendorModel);

        void Update(VendorEditServiceModel vendorModel);

        VendorQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        VendorDetailsServiceModel Details(int id, string sortOrder, string searchString, int currentPage);

        bool IsExistingName(string name);

        bool IsExistingVat(string vat);

        bool IsExistingVendor(int id);
    }
}
