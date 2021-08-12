using AutoMapper;
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
        private readonly IMapper mapper;

        public BrandsController(IBrandService brandService, IMapper mapper)
        {
            this.brandService = brandService;
            this.mapper = mapper;
        }

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(BrandAddFormServiceModel brandModel)
        {
            if (this.brandService.IsExistingName(brandModel.Name))
            {
                this.ModelState.AddModelError(nameof(brandModel.Name), "Brand already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(brandModel);
            }

            var brandAdded = this.brandService.Add(brandModel);

            TempData[SuccessMessageKey] = $"New Brand '{brandAdded}' added.";

            if (brandAdded == null)
            {
                TempData[ErrorMessageKey] = "There was an error adding new brand.";
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult All(BrandsQueryModel query)
        {
            var queryResult = this.brandService.All(
                query.SearchString,
                query.SortOrder,
                query.CurrentPage);

            var brands = this.mapper.Map<BrandsQueryModel>(queryResult);

            return View(brands);
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

            var result = this.brandService.Update(brand);

            if (result > 0)
            {
                TempData[SuccessMessageKey] = $"Brand '{brand.Name}' successfully updated.";
            }

            return RedirectToAction(nameof(All), new
                {
                    SearchString = brand.SearchString,
                    SortOrder = brand.SortOrder,
                    CurrentPage = brand.CurrentPage
                });
        }

        public IActionResult Delete(int id, string sortOrder, string searchString, int currentPage)
        {
            if (!this.brandService.IsExistingBrand(id))
            {
                return RedirectToAction("Error", "Home");
            }

            if (this.brandService.IsInUse(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var deletedBrandName = this.brandService.Delete(id);

            TempData[SuccessMessageKey] = $"Brand '{deletedBrandName}' successfully deleted.";

            if (deletedBrandName == null)
            {
                TempData[ErrorMessageKey] = $"There was an error deleting Brand with ID {id}.";
            }

            return RedirectToAction(nameof(All), new
            {
                SortOrder = sortOrder,
                SearchString = searchString,
                CurrentPage = currentPage
            });
        }
    }
}
