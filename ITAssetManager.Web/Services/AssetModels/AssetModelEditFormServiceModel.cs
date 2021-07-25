using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.AssetModels
{
    public class AssetModelEditFormServiceModel
    {
        public int Id { get; init; }

        [Display(Name = "Brand")]
        public int BrandId { get; init; }

        [BindNever]
        public IEnumerable<BrandDropdownServiceModel> Brands { get; set; }

        [Required]
        [StringLength(AssetModelNameMaxLength, MinimumLength = AssetModelNameMinLength)]
        public string Name { get; init; }

        [Display(Name = "Category")]
        public int CategoryId { get; init; }

        [BindNever]
        public IEnumerable<CategoryDropdownServiceModel> Categories { get; set; }

        [Url]
        [Required]
        [Display(Name = "Image Url")]
        public string ImageUrl { get; init; }

        [Required]
        [StringLength(AssetModelDetailsMaxLength, MinimumLength = AssetModelDetailsMinLength)]
        public string Details { get; init; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}
