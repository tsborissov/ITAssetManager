using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models;
using ITAssetManager.Web.Models.Brands;
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

        public BrandsController(ItAssetManagerDbContext data)
        {
            this.data = data;
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
        public IActionResult All(
            string sortOrder,
            string searchString,
            string currentFilter,
            int? pageNumber)

        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (String.IsNullOrEmpty(searchString))
            {
                searchString = currentFilter;
            }
            else
            {
                pageNumber = 1;
            }

            ViewData["CurrentFilter"] = searchString;

            var brandsQuery = this.data.Brands.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                brandsQuery = brandsQuery
                    .Where(b =>
                        b.Name.ToLower()
                        .Contains(searchString.ToLower()));
            }

            brandsQuery = sortOrder switch
            {
                "name_desc" => brandsQuery.OrderByDescending(b => b.Name),
                _ => brandsQuery.OrderBy(b => b.Name)
            };

            var brands = brandsQuery
                .Select(v => new BrandListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name
                });

            return View(PaginatedList<BrandListingViewModel>.Create(brands, pageNumber ?? 1, ListingPageSize));
        }
    }
}
