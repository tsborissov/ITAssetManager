namespace ITAssetManager.Data
{
    public class DataConstants
    {
        public const string ErrorPageUrl = "/Home/Error";

        public const string AdministratorRoleName = "Administrator";
        public const string UserRoleName = "User";

        public const string AdministratorUsername = "admin@email.com";

        public const string SuccessMessageKey = "SuccessMessageKey";
        public const string ErrorMessageKey = "ErrorMessageKey";

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

        public const int RequestRationaleMinLength = 5;
        public const int RequestRationaleMaxLength = 200;
        public const int CloseCommentMinLength = 1;
        public const int CloseCommentMaxLength = 200;
        public const string RequestCancelCloseComment = "Cancelled by user.";

        public const string InvalidModelMessage = "Invalid model selected!";
        public const string InvalidStatusMessage = "Invalid status selected!";
        public const string InvalidVendorMessage = "Invalid vendor selected!";
        public const string InvalidSerialNumberMessage = "Serial number alredy exists!";
        public const string InvalidInventoryNumberMessage = "Inventory number already exists!";

        public const string ReturnDateBeforeAssignDateError = "'Return date' cannot be before 'Assign date'!";
        public const string FutureReturnDateError = "'Return date' cannot be in the future!";
        public const string AssetSuccessfullyCreatedMessage = "New Asset created.";
        public const string ErrorCreatingAssetMessage = "There was an error creating new asset!";
        public const string AssetSuccessfullyAssignedMessage = "Asset successfully assigned.";
        public const string ErrorAssigningAssetMessage = "There was an error assignig asset!";
        public const string AssetSuccessfullyCollectedMessage = "Asset successfully collected.";
        public const string ErrorCollectingAssetMessage = "There was an error collecting asset!";
        public const string AssetSuccessfullyUpdatedMessage = "Asset successfully updated.";
        public const string ErrorUpdatingAssetMessage = "There was an error updating asset!";
        public const string AssetSuccessfullyDeletedMessage = "Asset successfully deleted.";
        public const string ErrorDeletingAssetMessage = "There was an error deleting asset!";
    }
}
