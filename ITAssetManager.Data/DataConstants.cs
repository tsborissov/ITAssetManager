namespace ITAssetManager.Data
{
    public class DataConstants
    {
        public const int VendorNameMinLength = 5;
        public const int VendorNameMaxLength = 50;

        public const int VatMinLength = 4;
        public const int VatMaxLength = 15;

        public const int AssetNameMinLength = 3;
        public const int AssetNameMaxLength = 50;

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
