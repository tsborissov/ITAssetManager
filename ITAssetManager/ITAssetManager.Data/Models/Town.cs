using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Town
    {
        [Key]
        [Required]
        [MaxLength(IdDefaultLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(TownNameMaxLength)]
        public string Name { get; set; }

        [Required]
        public string CountryId { get; set; }

        public Country Country { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Address> Addresses { get; init; } = new HashSet<Address>();
    }
}
