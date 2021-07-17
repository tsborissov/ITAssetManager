using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Models.Vendors
{
    public class VendorAddFormModel
    {
        [Required]
        [StringLength(VendorNameMaxLength, MinimumLength = VendorNameMinLength)]
        public string Name { get; init; }

        [Required]
        [Display(Name = "VAT")]
        [RegularExpression(VendorVatRegexPattern, ErrorMessage = "Invalid VAT Number!")]
        public string Vat { get; init; }

        [Required]
        [StringLength(VendorTelephoneMaxLength, MinimumLength = VendorTelephoneMinLength)]
        public string Telephone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(VendorAddressMaxLength, MinimumLength = VendorAddressMinLength)]
        public string Address { get; set; }
    }
}
