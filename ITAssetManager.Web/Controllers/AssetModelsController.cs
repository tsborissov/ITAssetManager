using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.AssetModels;
using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Models.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ITAssetManager.Web.Controllers
{
    public class AssetModelsController : Controller
    {
        private readonly ItAssetManagerDbContext data;

        public AssetModelsController(ItAssetManagerDbContext data) 
            => this.data = data;

        [Authorize]
        public IActionResult Add() => View(new AssetModelsAddFormModel 
        {
            Brands = this.GetBrands(),
            Categories = this.GetCategories()
        });

        [Authorize]
        [HttpPost]
        public IActionResult Add(AssetModelsAddFormModel assetModelModel)
        {
            if (!this.data.Brands.Any(b => b.Id == assetModelModel.BrandId))
            {
                this.ModelState.AddModelError(nameof(assetModelModel.BrandId), "Invalid 'Brand' selected!");
            }

            if (!this.data.Categories.Any(c => c.Id == assetModelModel.CategoryId))
            {
                this.ModelState.AddModelError(nameof(assetModelModel.CategoryId), "Invalid 'Category' selected!");
            }

            if (!this.ModelState.IsValid)
            {
                assetModelModel.Categories = this.GetCategories();
                assetModelModel.Brands = this.GetBrands();

                return View(assetModelModel);
            }

            var assetModel = new AssetModel 
            {
                BrandId = assetModelModel.BrandId,
                CategoryId = assetModelModel.CategoryId,
                Name = assetModelModel.Name,
                ImageUrl = assetModelModel.ImageUrl,
                Details = assetModelModel.Details
            };

            this.data.AssetModels.Add(assetModel);
            this.data.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private IEnumerable<BrandDropdownViewModel> GetBrands() 
            => this.data
                   .Brands
                   .OrderBy(b => b.Name)
                   .Select(b => new BrandDropdownViewModel
                   {
                       Id = b.Id,
                       Name = b.Name
                   })
                   .ToList();

        private IEnumerable<CategoryDropdownViewModel> GetCategories()
            => this.data
                    .Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDropdownViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList();
    }
}
