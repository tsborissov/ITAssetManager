using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Vendor
    {
        [Key]
        [Required]
        [MaxLength(IdDefaultLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

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
