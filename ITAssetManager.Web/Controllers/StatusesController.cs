using ITAssetManager.Web.Models.Statuses;
using ITAssetManager.Web.Services.Statuses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Controllers
{
    public class StatusesController : Controller
    {
        private readonly IStatusService statusService;

        public StatusesController(IStatusService statusService)
        {
            this.statusService = statusService;
        }

        [Authorize]
        public IActionResult Add() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Add(StatusAddFormModel statusModel)
        {
            if (this.statusService.IsExistingName(statusModel.Name))
            {
                this.ModelState.AddModelError(nameof(statusModel.Name), "Status already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(statusModel);
            }

            this.statusService.Add(statusModel.Name);

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
            if (!this.statusService.IsExistingStatus(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var targetStatus = this.statusService.Details(id);

            targetStatus.SortOrder = sortOrder;
            targetStatus.SearchString = searchString;
            targetStatus.CurrentPage = currentPage;

            return View(targetStatus);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(StatusEditServiceModel status)
        {
            if (!this.ModelState.IsValid)
            {
                return View(status);
            }

            if (!this.statusService.IsExistingStatus(status.Id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.statusService.Update(status);

            return RedirectToAction(nameof(All), new 
            {
                SortOrder = status.SortOrder,
                SearchString = status.SearchString,
                CurrentPage = status.CurrentPage
            });
        }
    }
}
