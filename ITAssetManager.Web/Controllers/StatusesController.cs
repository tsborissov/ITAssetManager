using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models;
using ITAssetManager.Web.Models.Statuses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

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

            var statusesQuery = this.data.Statuses.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                statusesQuery = statusesQuery
                    .Where(s =>
                        s.Name.ToLower().Contains(searchString.ToLower()));
            }

            statusesQuery = sortOrder switch
            {
                "name_desc" => statusesQuery.OrderByDescending(s => s.Name),
                _ => statusesQuery.OrderBy(s => s.Name)
            };

            var statuses = statusesQuery
                .Select(v => new StatusListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name
                });

            return View(PaginatedList<StatusListingViewModel>.Create(statuses, pageNumber ?? 1, ItemsPerPage));
        }
    }
}
