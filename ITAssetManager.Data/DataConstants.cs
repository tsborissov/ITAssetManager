namespace ITAssetManager.Data
{
    public class DataConstants
    {
        public const int ItemsPerPage = 5;
        public const int ModelsPerPage = 3;

        public const string AssetTargetAssignStatus = "In Use";
        public const string AssetTargetCollectStatus = "In Stock";

        public const int AssetModelNameMinLength = 3;
        public const int AssetModelNameMaxLength = 50;
        public const int AssetModelImageUrlMinLength = 3;
        public const int AssetModelImageUrlMaxLength = 2048;
        public const int AssetModelDetailsMinLength = 3;
        public const int AssetModelDetailsMaxLength = 200;

        public const int VendorNameMinLength = 2;
        public const int VendorNameMaxLength = 50;
        public const int VendorVatMaxLength = 15;
        public const string VendorVatRegexPattern = @"^([A-Z]{2}[0-9]{4,15})$";
        public const int VendorTelephoneMinLength = 4;
        public const int VendorTelephoneMaxLength = 20;
        public const int VendorEmailMaxLength = 30;
        public const int VendorAddressMinLength = 4;
        public const int VendorAddressMaxLength = 200;

        public const int BrandNameMinLength = 1;
        public const int BrandNameMaxLength = 30;

        public const int SerialNrMinLength = 3;
        public const int SerialNrMaxLength = 30;
        public const int InventoryNrMaxLength = 8;
        public const string InventoryNrRegexPattern = @"^([A-Z]{2}[0-9]{6})$";
        public const int InvoiceNrMinLength = 3;
        public const int InvoiceNrMaxLength = 15;
        public const int PriceMinValue = 0;
        public const int PriceMaxValue = 100000;

        public const int CategoryNameMinLength = 2;
        public const int CategoryNameMaxLength = 20;

        public const int StatusNameMinLength = 1;
        public const int StatusNameMaxLength = 20;
    }
}
