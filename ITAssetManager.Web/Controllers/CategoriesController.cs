using ITAssetManager.Web.Models.Categories;
using ITAssetManager.Web.Services.Categories;
using ITAssetManager.Web.Services.Categories.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    [Authorize(Roles = AdministratorRoleName)]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(CategoryAddFormServiceModel categoryModel)
        {
            if (this.categoryService.IsExistingName(categoryModel.Name))
            {
                this.ModelState.AddModelError(nameof(categoryModel.Name), "Category already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(categoryModel);
            }

            this.categoryService.Add(categoryModel);

            return RedirectToAction(nameof(All));
        }

        public IActionResult All(CategoriesQueryModel query)
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

        public IActionResult Edit(int id, string sortOrder, string searchString, int currentPage)
        {
            if (!this.categoryService.IsExistingCategory(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var targetCategory = this.categoryService.Details(id);

            targetCategory.SortOrder = sortOrder;
            targetCategory.SearchString = searchString;
            targetCategory.CurrentPage = currentPage;

            return View(targetCategory);
        }

        [HttpPost]
        public IActionResult Edit(CategoryEditServiceModel category)
        {
            if (!this.ModelState.IsValid)
            {
                return View(category);
            }

            if (!this.categoryService.IsExistingCategory(category.Id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.categoryService.Update(category);

            return RedirectToAction(nameof(All), new
            {
                SortOrder = category.SortOrder,
                SearchString = category.SearchString,
                CurrentPage = category.CurrentPage
            });
        }

        public IActionResult Delete(int id)
        {
            if (!this.categoryService.IsExistingCategory(id))
            {
                return RedirectToAction("Error", "Home");
            }

            if (this.categoryService.IsInUse(id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.categoryService.Delete(id);

            return RedirectToAction(nameof(All));
        }
    }
}
