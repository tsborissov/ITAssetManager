using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Vendors.Models
{
    public class VendorAddFormServiceModel
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
        public string Telephone { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [StringLength(VendorAddressMaxLength, MinimumLength = VendorAddressMinLength)]
        public string Address { get; init; }
    }
}
