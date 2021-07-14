using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

            return RedirectToAction("Index", "Home");
        }
    }
}
