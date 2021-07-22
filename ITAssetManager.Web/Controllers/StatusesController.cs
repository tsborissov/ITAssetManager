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
    [Authorize]
    public class StatusesController : Controller
    {
        private readonly ItAssetManagerDbContext data;

        public StatusesController(ItAssetManagerDbContext data)
        {
            this.data = data;
        }

        public IActionResult Add() => View();

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

        public IActionResult Edit(int id, string sortOrder, string searchString, int currentPage)
        {
            var status = this.data
                .Statuses
                .Where(s => s.Id == id)
                .Select(s => new StatusEditModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    SortOrder = sortOrder,
                    SearchString = searchString,
                    CurrentPage = currentPage
                })

                .FirstOrDefault();

            if (status == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(status);
        }

        [HttpPost]
        public IActionResult Edit(StatusEditModel status)
        {
            if (!this.ModelState.IsValid)
            {
                return View(status);
            }

            var targetStatus = this.data.Statuses.Find(status.Id);

            if (targetStatus == null)
            {
                return RedirectToAction("Error", "Home");
            }

            targetStatus.Name = status.Name;
            this.data.SaveChanges();

            return RedirectToAction(nameof(All), new 
            {
                SortOrder = status.SortOrder,
                SearchString = status.SearchString,
                CurrentPage = status.CurrentPage
            });
        }
    }
}
