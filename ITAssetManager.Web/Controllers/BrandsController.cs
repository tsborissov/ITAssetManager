using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult All()
        {
            var brands = this.data
                .Brands
                .OrderBy(b => b.Name)
                .Select(b => new BrandListingViewModel
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToList();
            
            return View(brands);
        }
    }
}
