using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class AssetCategory
    {
        [Key]
        [Required]
        [MaxLength(IdDefaultLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Asset> Assets { get; init; } = new HashSet<Asset>();

        public ICollection<Request> Requests { get; init; } = new HashSet<Request>();
    }
}
