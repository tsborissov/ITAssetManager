using AutoMapper;
using ITAssetManager.Web.Models.Statuses;
using ITAssetManager.Web.Services.Statuses;
using ITAssetManager.Web.Services.Statuses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class StatusesController : Controller
    {
        private readonly IStatusService statusService;
        private readonly IMapper mapper;

        public StatusesController(IStatusService statusService, IMapper mapper)
        {
            this.statusService = statusService;
            this.mapper = mapper;
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Add() => View();

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Add(StatusAddFormServiceModel statusModel)
        {
            if (this.statusService.IsExistingName(statusModel.Name))
            {
                this.ModelState.AddModelError(nameof(statusModel.Name), "Status already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(statusModel);
            }

            var statusAdded = this.statusService.Add(statusModel);

            TempData[SuccessMessageKey] = $"New Status '{statusAdded}' created.";

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult All(StatusesQueryModel query)
        {
            var queryResult = this.statusService.All(query.SearchString, query.SortOrder, query.CurrentPage);

            var statuses = this.mapper.Map<StatusesQueryModel>(queryResult);

            return View(statuses);
        }

        [Authorize(Roles = AdministratorRoleName)]
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

        [Authorize(Roles = AdministratorRoleName)]
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

            var result = this.statusService.Update(status);

            if (result > 0)
            {
                TempData[SuccessMessageKey] = "Status successfully updated.";
            }

            return RedirectToAction(nameof(All), new
            {
                SortOrder = status.SortOrder,
                SearchString = status.SearchString,
                CurrentPage = status.CurrentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Delete(int id, string sortOrder, string searchString, int currentPage)
        {
            if (!this.statusService.IsExistingStatus(id))
            {
                return RedirectToAction("Error", "Home");
            }

            if (this.statusService.IsInUse(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var deletedStatusName = this.statusService.Delete(id);

            TempData[SuccessMessageKey] = $"Status '{deletedStatusName}' successfully deleted.";

            if (deletedStatusName == null)
            {
                TempData[ErrorMessageKey] = $"There was an error deleting Status with ID {id}.";
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
