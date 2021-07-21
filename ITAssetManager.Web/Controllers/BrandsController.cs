using ITAssetManager.Data;
using ITAssetManager.Data.Models;
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
        public IActionResult All([FromQuery] BrandsQueryModel query)
        {
            var brandsQuery = this.data
                .Brands
                .AsQueryable();

            if (!String.IsNullOrEmpty(query.SearchString))
            {
                brandsQuery = brandsQuery
                    .Where(b =>
                        b.Name.ToLower()
                        .Contains(query.SearchString.ToLower()));
            }

            brandsQuery = query.SortOrder switch
            {
                "name_desc" => brandsQuery.OrderByDescending(b => b.Name),
                _ => brandsQuery.OrderBy(b => b.Name)
            };

            var itemsCount = brandsQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (query.CurrentPage < 1)
            {
                query.CurrentPage = 1;
            }

            if (query.CurrentPage > lastPage)
            {
                query.CurrentPage = lastPage;
            }

            var brands = brandsQuery
                .Skip((query.CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new BrandListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .ToList();

            return View(new BrandsQueryModel 
            {
                Brands = brands,
                SearchString = query.SearchString,
                SortOrder = query.SortOrder,
                CurrentPage = query.CurrentPage,
                HasPreviousPage = query.CurrentPage > 1,
                HasNextPage = query.CurrentPage < lastPage
            });
        }
    }
}
