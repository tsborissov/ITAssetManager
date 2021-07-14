namespace ITAssetManager.Data
{
    public class DataConstants
    {
        public const int AssetModelNameMinLength = 3;
        public const int AssetModelNameMaxLength = 50;
        public const int AssetModelImageUrlMinLength = 3;
        public const int AssetModelImageUrlMaxLength = 2048;
        public const int AssetModelDetailsMinLength = 3;
        public const int AssetModelDetailsMaxLength = 200;

        public const int VendorNameMinLength = 1;
        public const int VendorNameMaxLength = 50;
        public const int VendorVatMinLength = 4;
        public const int VendorVatMaxLength = 15;
        public const int VendorDetailsMaxLength = 200;

        public const int BrandNameMinLength = 1;
        public const int BrandNameMaxLength = 30;

        public const int SerialNrMinLength = 3;
        public const int SerialNrMaxLength = 30;
        public const int InventoryNrMaxLength = 8;
        public const string InventoryNrRegexPattern = @"[A-Z]{2}[0-9]{6}";
        public const int InvoiceNrMinLength = 3;
        public const int InvoiceNrMaxLength = 15;

        public const int CategoryNameMinLength = 2;
        public const int CategoryNameMaxLength = 20;

        public const int StatusNameMaxLength = 20;
    }
}
