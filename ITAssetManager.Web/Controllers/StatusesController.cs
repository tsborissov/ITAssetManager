using ITAssetManager.Web.Models.Statuses;
using ITAssetManager.Web.Services.Statuses;
using ITAssetManager.Web.Services.Statuses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    [Authorize(Roles = AdministratorRoleName)]
    public class StatusesController : Controller
    {
        private readonly IStatusService statusService;

        public StatusesController(IStatusService statusService)
        {
            this.statusService = statusService;
        }

        public IActionResult Add() => View();

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

        public IActionResult All(StatusesQueryModel query)
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

        public IActionResult Delete(int id)
        {
            if (!this.statusService.IsExistingStatus(id))
            {
                return RedirectToAction("Error", "Home");
            }

            if (this.statusService.IsInUse(id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.statusService.Delete(id);

            return RedirectToAction(nameof(All));
        }
    }
}
