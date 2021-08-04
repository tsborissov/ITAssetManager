using AutoMapper;
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
        private readonly IMapper mapper;

        public StatusesController(IStatusService statusService, IMapper mapper)
        {
            this.statusService = statusService;
            this.mapper = mapper;
        }

        public IActionResult Add() => View();

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

            this.statusService.Add(statusModel);

            return RedirectToAction(nameof(All));
        }

        public IActionResult All(StatusesQueryModel query)
        {
            var queryResult = this.statusService.All(query.SearchString, query.SortOrder, query.CurrentPage);

            var statuses = this.mapper.Map<StatusesQueryModel>(queryResult);

            return View(statuses);
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

            this.statusService.Delete(id);

            return RedirectToAction(nameof(All), new
            {
                SortOrder = sortOrder,
                SearchString = searchString,
                CurrentPage = currentPage
            });
        }
    }
}
