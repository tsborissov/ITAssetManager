using ITAssetManager.Data;
using ITAssetManager.Data.Models;
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
        public IActionResult All([FromQuery] StatusesQueryModel query)
        {
            var statusesQuery = this.data.Statuses.AsQueryable();

            if (!String.IsNullOrEmpty(query.SearchString))
            {
                statusesQuery = statusesQuery
                    .Where(s =>
                        s.Name.ToLower().Contains(query.SearchString.ToLower()));
            }

            statusesQuery = query.SortOrder switch
            {
                "name_desc" => statusesQuery.OrderByDescending(s => s.Name),
                _ => statusesQuery.OrderBy(s => s.Name)
            };

            var itemsCount = statusesQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (query.CurrentPage < 1)
            {
                query.CurrentPage = 1;
            }

            if (query.CurrentPage > lastPage)
            {
                query.CurrentPage = lastPage;
            }

            var statuses = statusesQuery
                .Skip((query.CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new StatusListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .ToList();

            return View(new StatusesQueryModel
            {
                Statuses = statuses,
                SearchString = query.SearchString,
                SortOrder = query.SortOrder,
                CurrentPage = query.CurrentPage,
                HasPreviousPage = query.CurrentPage > 1,
                HasNextPage = query.CurrentPage < lastPage
            });
        }
    }
}
