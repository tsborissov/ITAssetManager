using ITAssetManager.Web.Services.AssetModels;
using ITAssetManager.Web.Services.AssetModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class AssetModelsController : Controller
    {
        private readonly IAssetModelService assetModelService;

        public AssetModelsController(IAssetModelService assetModelService)
        {
            this.assetModelService = assetModelService;
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Add() => View(
            new AssetModelsAddFormServiceModel
            {
                Brands = this.assetModelService.GetBrands(),
                Categories = this.assetModelService.GetCategories()
            });

        [Authorize(Roles = AdministratorRoleName)]
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

            var modelAdded = this.assetModelService.Add(assetModel);

            TempData[SuccessMessageKey] = $"New Model '{modelAdded}' created.";

            if (modelAdded == null)
            {
                TempData[ErrorMessageKey] = "There was an error adding new model!";
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult All(string searchString, int currentPage)
        {
            var allAssetModels = this.assetModelService.All(searchString, currentPage);

            return View(allAssetModels);
        }

        [Authorize]
        public IActionResult Details(int id, string searchString, int currentPage)
        {
            if (!assetModelService.IsExistingModel(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var assetModel = this.assetModelService.Details(id);

            assetModel.SearchString = searchString;
            assetModel.CurrentPage = currentPage;

            var referer = HttpContext.Request.Headers["Referer"].ToString().Split('/').Skip(3).ToArray();

            var refererController = referer[0];
            var refererAction = referer[1].Split('?').Take(1).ToArray()[0];

            if (refererAction == nameof(Edit))
            {
                refererController = ControllerContext.ActionDescriptor.ControllerName;
                refererAction = nameof(All);
            }

            ViewBag.RefererController = refererController;
            ViewBag.RefererAction = refererAction;

            return View(assetModel);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Edit(int id, string searchString, int currentPage)
        {
            if (!assetModelService.IsExistingModel(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var assetModel = this.assetModelService.Details(id);

            var brands = this.assetModelService.GetBrands();
            var currentBrandId = brands
                .Where(b => b.Name == assetModel.Brand)
                .Select(b => b.Id)
                .FirstOrDefault();
            
            var categories = this.assetModelService.GetCategories();
            var currentCategoryId = categories
                .Where(c => c.Name == assetModel.Category)
                .Select(c => c.Id)
                .FirstOrDefault();

            return View(
                new AssetModelEditFormServiceModel 
                { 
                    Id = assetModel.Id,
                    BrandId = currentBrandId,
                    Brands = brands,
                    CategoryId = currentCategoryId,
                    Categories= categories,
                    Name = assetModel.Name,
                    ImageUrl = assetModel.ImageUrl,
                    Details = assetModel.Details,
                    SearchString = searchString,
                    CurrentPage = currentPage
                });
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Edit(AssetModelEditFormServiceModel assetModel)
        {
            if (!this.ModelState.IsValid)
            {
                var brands = this.assetModelService.GetBrands();
                assetModel.Brands = brands;

                var categories = this.assetModelService.GetCategories();
                assetModel.Categories = categories;

                return View(assetModel);
            }

            if (!this.assetModelService.IsExistingModel(assetModel.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            var result = this.assetModelService.Update(assetModel);

            if (result > 0)
            {
                TempData[SuccessMessageKey] = "Model data successfully updated.";
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = AdministratorRoleName)]

        public IActionResult Delete(int id)
        {
            if (!this.assetModelService.IsExistingModel(id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (this.assetModelService.IsInUse(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var deletedModelName = this.assetModelService.Delete(id);

            TempData[SuccessMessageKey] = $"Model '{deletedModelName}' successfully deleted.";

            if (deletedModelName == null)
            {
                TempData[ErrorMessageKey] = $"There was an error deleting Model with ID {id}.";
            }

            return RedirectToAction(nameof(All));
        }
    }
}
