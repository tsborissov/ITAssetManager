using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class AssetStatus
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(StatusNameMaxLength)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Asset> Assets { get; init; } = new HashSet<Asset>();
    }
}