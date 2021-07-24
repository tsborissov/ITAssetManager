using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Statuses;
using ITAssetManager.Web.Services.Statuses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class StatusesController : Controller
    {
        private readonly IStatusService statusService;
        private readonly ItAssetManagerDbContext data;

        public StatusesController(IStatusService statusService, ItAssetManagerDbContext data)
        {
            this.statusService = statusService;
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
            var queryResult = this.statusService.All(query.SearchString, query.SortOrder, query.CurrentPage);

            query.Statuses = queryResult.Statuses;
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

        [Authorize]
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
