using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Categories
{
    public class CategoryEditServiceModel
    {
        public int Id { get; init; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength)]
        public string Name { get; init; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}
