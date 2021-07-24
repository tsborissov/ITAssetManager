using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Vendors
{
    public class VendorDetailsServiceModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        [Display(Name = "VAT")]
        public string Vat { get; init; }

        public string Telephone { get; init; }

        public string Email { get; init; }

        public string Address { get; init; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}