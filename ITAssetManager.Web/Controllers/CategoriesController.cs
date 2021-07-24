using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Categories;
using ITAssetManager.Web.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;


namespace ITAssetManager.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly ItAssetManagerDbContext data;

        public CategoriesController(ICategoryService categoryService, ItAssetManagerDbContext data)
        {
            this.categoryService = categoryService;
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
            var queryResult = this.categoryService.All(
                query.SearchString,
                query.SortOrder,
                query.CurrentPage);

            query.Categories = queryResult.Categories;
            query.SearchString = queryResult.SearchString;
            query.SortOrder = queryResult.SortOrder;
            query.CurrentPage = queryResult.CurrentPage;
            query.HasPreviousPage = queryResult.HasPreviousPage;
            query.HasNextPage = queryResult.HasNextPage;

            return View(query);
        }

        [Authorize]
        public IActionResult Edit(int id, string sortOrder, string searchString, int currentPage)
        {
            var category = this.data
                .Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryEditModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    SortOrder = sortOrder,
                    SearchString = searchString,
                    CurrentPage = currentPage
                })
                .FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(category);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(CategoryEditModel category)
        {
            if (!this.ModelState.IsValid)
            {
                return View(category);
            }

            var targetCategory = this.data.Categories.Find(category.Id);

            if (targetCategory == null)
            {
                return RedirectToAction("Error", "Home");
            }

            targetCategory.Name = category.Name;
            this.data.SaveChanges();

            return RedirectToAction(nameof(All), new
            {
                SortOrder = category.SortOrder,
                SearchString = category.SearchString,
                CurrentPage = category.CurrentPage
            });
        }
    }
}
