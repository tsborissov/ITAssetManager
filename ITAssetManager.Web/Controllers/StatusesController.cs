using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Statuses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ITAssetManager.Web.Controllers
{
    public class StatusesController : Controller
    {
        private readonly ItAssetManagerDbContext data;

        public StatusesController(ItAssetManagerDbContext data)
        {
            this.data = data;
        }

        [Authorize]
        public IActionResult Add() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Add(StatusAddFormModel statusModel)
        {
            if (this.data.Statuses.Any(s => s.Name == statusModel.Name))
            {
                this.ModelState.AddModelError(nameof(statusModel.Name), "Status already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(statusModel);
            }

            var status = new Status 
            {
                Name = statusModel.Name
            };

            this.data.Statuses.Add(status);
            this.data.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
