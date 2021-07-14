using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Models.Vendors
{
    public class VendorAddFormModel
    {
        [Required]
        [StringLength(VendorNameMaxLength, MinimumLength = VendorNameMinLength)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "VAT")]
        [StringLength(VendorVatMaxLength, MinimumLength = VendorVatMinLength)]
        public string Vat { get; set; }

        [StringLength(VendorDetailsMaxLength)]
        public string Details { get; set; }
    }
}
