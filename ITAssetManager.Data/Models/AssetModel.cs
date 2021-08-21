using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class AssetModel
    {
        public int Id { get; init; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        [Required]
        [MaxLength(AssetModelNameMaxLength)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Required]
        [MaxLength(AssetModelImageUrlMaxLength)]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(AssetModelDetailsMaxLength)]
        public string Details { get; set; }

        public ICollection<Asset> Assets { get; set; } = new HashSet<Asset>();

        public ICollection<Request> Requests { get; init; } = new HashSet<Request>();
    }
}
