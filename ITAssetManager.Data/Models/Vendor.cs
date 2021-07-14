using System;
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
        [MaxLength(VendorDetailsMaxLength)]
        public string Details { get; set; }

        public ICollection<Asset> Assets { get; init; } = new HashSet<Asset>();
    }
}
