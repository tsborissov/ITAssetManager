using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Brands.Models
{
    public class BrandEditServiceModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(BrandNameMaxLength, MinimumLength = BrandNameMinLength)]
        public string Name { get; init; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}
