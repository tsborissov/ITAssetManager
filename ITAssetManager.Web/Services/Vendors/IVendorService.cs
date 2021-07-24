namespace ITAssetManager.Web.Services.Vendors
{
    public interface IVendorService
    {
        VendorQueryServiceModel All(string searchString, string sortOrder, int currentPage);
    }
}
