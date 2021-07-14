using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Models.Categories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Models.AssetModels
{
    public class AssetModelsAddFormModel
    {
        [Display(Name = "Brand")]
        public int BrandId { get; init; }

        [BindNever]
        public IEnumerable<BrandDropdownViewModel> Brands { get; set; }

        [Required]
        [StringLength(AssetModelNameMaxLength, MinimumLength = AssetModelNameMinLength)]
        public string Name { get; init; }

        public int CategoryId { get; init; }

        [BindNever]
        public IEnumerable<CategoryDropdownViewModel> Categories { get; set; }

        [Url]
        [Required]
        [Display(Name = "Image Url")]
        public string ImageUrl { get; init; }

        [Required]
        [StringLength(AssetModelDetailsMaxLength, MinimumLength = AssetModelDetailsMinLength)]
        public string Details { get; init; }
    }
}
