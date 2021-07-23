using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Services.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ItAssetManagerDbContext data;
        private readonly IBrandService brandService;

        public BrandsController(ItAssetManagerDbContext data, IBrandService brandService)
        {
            this.data = data;
            this.brandService = brandService;
        }

        [Authorize]
        public IActionResult Add() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Add(BrandAddFormModel brandModel)
        {
            if (this.data.Brands.Any(b => b.Name == brandModel.Name))
            {
                this.ModelState.AddModelError(nameof(brandModel.Name), "Brand already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(brandModel);
            }

            var brand = new Brand
            {
                Name = brandModel.Name
            };

            this.data.Brands.Add(brand);
            this.data.SaveChanges();

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult All([FromQuery] BrandsQueryModel query)
        {
            var queryResult = this.brandService.All(
                query.SearchString, 
                query.SortOrder, 
                query.CurrentPage,
                BrandsQueryModel.BrandsPerPage);

            query.Brands = queryResult.Brands;
            query.SearchString = queryResult.SearchString;
            query.SortOrder = queryResult.SortOrder;
            query.CurrentPage = queryResult.CurrentPage;
            query.HasNextPage = queryResult.HasNextPage;
            query.HasPreviousPage = queryResult.HasPreviousPage;

            return View(query);
        }

        [Authorize]
        public IActionResult Edit(int id, string searchString, string sortOrder, int currentPage)
        {
            var brand = this.data
                .Brands
                .Where(b => b.Id == id)
                .Select(b => new BrandEditModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    SearchString = searchString,
                    SortOrder = sortOrder,
                    CurrentPage = currentPage
                })
                .FirstOrDefault();

            if (brand == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(brand);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(BrandEditModel brand)
        {
            if (!this.ModelState.IsValid)
            {
                return View(brand);
            }

            var targetBrand = this.data.Brands.Find(brand.Id);

            if (targetBrand == null)
            {
                return RedirectToAction("Error", "Home");
            }

            targetBrand.Name = brand.Name;
            this.data.SaveChanges();

            return RedirectToAction(nameof(All), new
                {
                    SearchString = brand.SearchString,
                    SortOrder = brand.SortOrder,
                    CurrentPage = brand.CurrentPage
                });
        }
    }
}
