namespace ITAssetManager.Web.Services.Vendors.Models
{
    public class VendorListingServiceModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public string Vat { get; init; }

        public bool IsInUse { get; init; }
    }
}
