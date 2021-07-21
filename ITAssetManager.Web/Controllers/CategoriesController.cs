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
        public IActionResult All([FromQuery] CategoriesQueryModel query)

        {
            var categoriesQuery = this.data.Categories.AsQueryable();

            if (!String.IsNullOrEmpty(query.SearchString))
            {
                categoriesQuery = categoriesQuery
                    .Where(c =>
                        c.Name.ToLower()
                        .Contains(query.SearchString.ToLower()));
            }

            categoriesQuery = query.SortOrder switch
            {
                "name_desc" => categoriesQuery.OrderByDescending(c => c.Name),
                _ => categoriesQuery.OrderBy(c => c.Name)
            };

            var itemsCount = categoriesQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (query.CurrentPage < 1)
            {
                query.CurrentPage = 1;
            }

            if (query.CurrentPage > lastPage)
            {
                query.CurrentPage = lastPage;
            }

            var categories = categoriesQuery
                .Skip((query.CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new CategoryListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .ToList();

            return View(new CategoriesQueryModel
            {
                Categories = categories,
                SearchString = query.SearchString,
                SortOrder = query.SortOrder,
                CurrentPage = query.CurrentPage,
                HasPreviousPage = query.CurrentPage > 1,
                HasNextPage = query.CurrentPage < lastPage
            });
        }
    }
}
