using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Brand
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(BrandNameMaxLength)]
        public string Name { get; set; }

        public ICollection<AssetModel> AssetModels { get; set; } = new HashSet<AssetModel>();
    }
}
