using ITAssetManager.Data;
using ITAssetManager.Web.Services.AssetModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Controllers
{
    public class AssetModelsController : Controller
    {
        private readonly IAssetModelService assetModelService;

        public AssetModelsController(IAssetModelService assetModelService, ItAssetManagerDbContext data) 
            => this.assetModelService = assetModelService;

        [Authorize]
        public IActionResult Add() => View(
            new AssetModelsAddFormServiceModel
            {
                Brands = this.assetModelService.GetBrands(),
                Categories = this.assetModelService.GetCategories()
            });

        [Authorize]
        [HttpPost]
        public IActionResult Add(AssetModelsAddFormServiceModel assetModel)
        {
            if (!this.assetModelService.IsBrandValid(assetModel.BrandId))
            {
                this.ModelState.AddModelError(nameof(assetModel.BrandId), "Invalid 'Brand' selected!");
            }

            if (!this.assetModelService.IsCategoryValid(assetModel.CategoryId))
            {
                this.ModelState.AddModelError(nameof(assetModel.CategoryId), "Invalid 'Category' selected!");
            }

            if (!this.ModelState.IsValid)
            {
                assetModel.Categories = this.assetModelService.GetCategories();
                assetModel.Brands = this.assetModelService.GetBrands();

                return View(assetModel);
            }

            this.assetModelService.Add(assetModel);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult All(string searchString, string sortOrder, int currentPage)
        {
            var queryResult = this.assetModelService.All(searchString, currentPage);

            return View(queryResult);
        }

        [Authorize]
        public IActionResult Details(int id, string searchString, int currentPage)
        {
            if (!assetModelService.IsExistingModel(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var assetModel = this.assetModelService.Details(id);

            assetModel.SearchString = searchString;
            assetModel.CurrentPage = currentPage;

            return View(assetModel);
        }
    }
}
