using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Country
    {
        [Key]
        [Required]
        [MaxLength(IdDefaultLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(CountryNameMaxLength)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Town> Towns { get; init; } = new HashSet<Town>();
    }
}
