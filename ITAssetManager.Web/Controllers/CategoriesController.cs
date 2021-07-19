using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models;
using ITAssetManager.Web.Models.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;


namespace ITAssetManager.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ItAssetManagerDbContext data;

        public CategoriesController(ItAssetManagerDbContext data)
        {
            this.data = data;
        }

        [Authorize]
        public IActionResult Add() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Add(CategoryAddFormModel categoryModel)
        {
            if (this.data.Categories.Any(c => c.Name == categoryModel.Name))
            {
                this.ModelState.AddModelError(nameof(categoryModel.Name), "Category already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(categoryModel);
            }

            var category = new Category 
            {
                Name = categoryModel.Name
            };

            this.data.Categories.Add(category);
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

            var categoriesQuery = this.data.Categories.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                categoriesQuery = categoriesQuery
                    .Where(c =>
                        c.Name.ToLower()
                        .Contains(searchString.ToLower()));
            }

            categoriesQuery = sortOrder switch
            {
                "name_desc" => categoriesQuery.OrderByDescending(c => c.Name),
                _ => categoriesQuery.OrderBy(c => c.Name)
            };

            var categories = categoriesQuery
                .Select(c => new CategoryListingViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });

            return View(PaginatedList<CategoryListingViewModel>.Create(categories, pageNumber ?? 1, ListingPageSize));
        }
    }
}
