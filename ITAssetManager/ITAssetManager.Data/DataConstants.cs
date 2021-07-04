namespace ITAssetManager.Data
{
    public class DataConstants
    {
        // Common
        public const int IdDefaultLength = 40;

        // Vendor
        public const int VendorNameMinLength = 5;
        public const int VendorNameMaxLength = 50;

        public const int VatMinLength = 4;
        public const int VatMaxLength = 15;

        // Asset
        public const int AssetNameMinLength = 3;
        public const int AssetNameMaxLength = 100;

        public const int SerialNrMinLength = 3;
        public const int SerialNrMaxLength = 30;

        public const int InventoryNrMaxLength = 8;
        public const string InventoryNrRegexPattern = @"[A-Z]{2}[0-9]{6}";

        public const int InvoiceNrMinLength = 3;
        public const int InvoiceNrMaxLength = 15;

        // Asset Categories
        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 50;

        // Asset Status
        public const int StatusNameMaxLength = 20;

        // Address
        public const int CountryNameMinLength = 3;
        public const int CountryNameMaxLength = 60;

        public const int TownNameMinLength = 3;
        public const int TownNameMaxLength = 100;

        public const int AddressNameMinLength = 3;
        public const int AddressNameMaxLength = 100;
    }
}
