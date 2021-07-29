using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Services.Brands;
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
            => this.brandService = brandService;

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

            query.Brands = queryResult.Brands;
            query.SearchString = queryResult.SearchString;
            query.SortOrder = queryResult.SortOrder;
            query.CurrentPage = queryResult.CurrentPage;
            query.HasNextPage = queryResult.HasNextPage;
            query.HasPreviousPage = queryResult.HasPreviousPage;

            return View(query);
        }

        public IActionResult Edit(int id, string searchString, string sortOrder, int currentPage)
        {
            if (!this.brandService.IsExistingBrand(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var targetBrand = this.brandService.Details(id);

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
    }
}
