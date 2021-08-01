using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Services.Brands;
using ITAssetManager.Web.Services.Brands.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    [Authorize(Roles = AdministratorRoleName)]
    public class BrandsController : Controller
    {
        private readonly IBrandService brandService;

        public BrandsController(IBrandService brandService)
        {
            this.brandService = brandService;
        }

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(BrandAddFormModel brandModel)
        {
            if (this.brandService.IsExistingName(brandModel.Name))
            {
                this.ModelState.AddModelError(nameof(brandModel.Name), "Brand already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(brandModel);
            }

            this.brandService.Add(brandModel.Name);

            return RedirectToAction(nameof(All));
        }

        public IActionResult All(BrandsQueryModel query)
        {
            var queryResult = this.brandService.All(
                query.SearchString,
                query.SortOrder,
                query.CurrentPage);

            return View(new BrandsQueryModel
            {
                Brands = queryResult.Brands,
                SearchString = queryResult.SearchString,
                SortOrder = queryResult.SortOrder,
                CurrentPage = queryResult.CurrentPage,
                HasNextPage = queryResult.HasNextPage,
                HasPreviousPage = queryResult.HasPreviousPage
            });
        }

        public IActionResult Edit(int id, string searchString, string sortOrder, int currentPage)
        {
            if (!this.brandService.IsExistingBrand(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var targetBrand = this.brandService.Details(id);

            if (targetBrand == null)
            {
                return RedirectToAction("Error", "Home");
            }

            targetBrand.SearchString = searchString;
            targetBrand.SortOrder = sortOrder;
            targetBrand.CurrentPage = currentPage;

            return View(targetBrand);
        }

        [HttpPost]
        public IActionResult Edit(BrandEditServiceModel brand)
        {
            if (!this.ModelState.IsValid)
            {
                return View(brand);
            }

            if (!this.brandService.IsExistingBrand(brand.Id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.brandService.Update(brand);

            return RedirectToAction(nameof(All),
                new
                {
                    SearchString = brand.SearchString,
                    SortOrder = brand.SortOrder,
                    CurrentPage = brand.CurrentPage
                });
        }

        public IActionResult Delete(int id)
        {
            if (!this.brandService.IsExistingBrand(id))
            {
                return RedirectToAction("Error", "Home");
            }

            if (this.brandService.IsInUse(id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.brandService.Delete(id);

            return RedirectToAction(nameof(All));
        }
    }
}
