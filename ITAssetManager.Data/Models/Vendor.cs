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
        [MaxLength(VatMaxLength)]
        public string Vat { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Asset> Assets { get; init; } = new HashSet<Asset>();
    }
}
