using AutoMapper;
using ITAssetManager.Web.Models.Categories;
using ITAssetManager.Web.Services.Categories;
using ITAssetManager.Web.Services.Categories.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Add() => View();

        [Authorize(Roles = AdministratorRoleName)]
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

            var categoryAdded = this.categoryService.Add(categoryModel);

            TempData[SuccessMessageKey] = $"New Category '{categoryAdded}' created.";

            if (categoryAdded == null)
            {
                TempData[ErrorMessageKey] = "There was an error adding new category!";
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult All(CategoriesQueryModel query)
        {
            var queryResult = this.categoryService.All(
                query.SearchString,
                query.SortOrder,
                query.CurrentPage);

            var categories = this.mapper.Map<CategoriesQueryModel>(queryResult);

            return View(categories);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Edit(int id, string sortOrder, string searchString, int currentPage)
        {
            if (!this.categoryService.IsExistingCategory(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var targetCategory = this.categoryService.Details(id);

            targetCategory.SortOrder = sortOrder;
            targetCategory.SearchString = searchString;
            targetCategory.CurrentPage = currentPage;

            return View(targetCategory);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Edit(CategoryEditServiceModel category)
        {
            if (!this.ModelState.IsValid)
            {
                return View(category);
            }

            if (!this.categoryService.IsExistingCategory(category.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            var result = this.categoryService.Update(category);

            if (result > 0)
            {
                TempData[SuccessMessageKey] = "Category successfully updated.";
            }

            return RedirectToAction(nameof(All), new
            {
                SortOrder = category.SortOrder,
                SearchString = category.SearchString,
                CurrentPage = category.CurrentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Delete(int id, string sortOrder, string searchString, int currentPage)
        {
            if (!this.categoryService.IsExistingCategory(id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (this.categoryService.IsInUse(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var deletedCategoryName = this.categoryService.Delete(id);

            TempData[SuccessMessageKey] = $"Category '{deletedCategoryName}' successfully deleted.";

            if (deletedCategoryName == null)
            {
                TempData[ErrorMessageKey] = $"There was an error deleting Category with ID {id}.";
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
