using AutoMapper;
using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Services.Brands;
using ITAssetManager.Web.Services.Brands.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class BrandsController : Controller
    {
        private readonly IBrandService brandService;
        private readonly IMapper mapper;

        public BrandsController(IBrandService brandService, IMapper mapper)
        {
            this.brandService = brandService;
            this.mapper = mapper;
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Add() => View();

        [Authorize(Roles = AdministratorRoleName)]
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

            TempData[SuccessMessageKey] = $"New Brand '{brandAdded}' created.";

            if (brandAdded == null)
            {
                TempData[ErrorMessageKey] = "There was an error adding new brand!";
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult All(BrandsQueryModel query)
        {
            var queryResult = this.brandService.All(
                query.SearchString,
                query.SortOrder,
                query.CurrentPage);

            var brands = this.mapper.Map<BrandsQueryModel>(queryResult);

            return View(brands);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Edit(int id, string searchString, string sortOrder, int currentPage)
        {
            if (!this.brandService.IsExistingBrand(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var targetBrand = this.brandService.Details(id);

            targetBrand.SearchString = searchString;
            targetBrand.SortOrder = sortOrder;
            targetBrand.CurrentPage = currentPage;

            return View(targetBrand);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Edit(BrandEditServiceModel brand)
        {
            if (!this.ModelState.IsValid)
            {
                return View(brand);
            }

            if (!this.brandService.IsExistingBrand(brand.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            var result = this.brandService.Update(brand);

            if (result > 0)
            {
                TempData[SuccessMessageKey] = "Brand successfully updated.";
            }

            return RedirectToAction(nameof(All), new
                {
                    SearchString = brand.SearchString,
                    SortOrder = brand.SortOrder,
                    CurrentPage = brand.CurrentPage
                });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Delete(int id, string sortOrder, string searchString, int currentPage)
        {
            if (!this.brandService.IsExistingBrand(id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (this.brandService.IsInUse(id))
            {
                return Redirect(ErrorPageUrl);
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
