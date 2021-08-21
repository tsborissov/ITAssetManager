using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Vendor
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(VendorNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(VendorVatMaxLength)]
        public string Vat { get; set; }

        [Required]
        [MaxLength(VendorTelephoneMaxLength)]
        public string Telephone { get; set; }
        
        [Required]
        [MaxLength(VendorEmailMaxLength)]
        public string Email { get; set; }

        [MaxLength(VendorAddressMaxLength)]
        public string Address { get; set; }

        public ICollection<Asset> Assets { get; init; } = new HashSet<Asset>();
    }
}
