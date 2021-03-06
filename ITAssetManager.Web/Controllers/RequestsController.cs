using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.Assets;
using ITAssetManager.Web.Services.Requests;
using ITAssetManager.Web.Services.Requests.Models;
using ITAssetManager.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IRequestService requestService;
        private readonly IAssetService assetService;

        public RequestsController(IRequestService requestService, IAssetService assetService)
        {
            this.requestService = requestService;
            this.assetService = assetService;
        }

        [Authorize]
        public IActionResult Submit(int assetModelId, string model)
            => View(new RequestSubmitFormServiceModel
            {
                RequestorId = this.User.Id(),
                AssetModelId = assetModelId,
                Model = model,
                Status = RequestStatus.Submitted,
                SubmissionDate = DateTime.Now
            });

        [Authorize]
        [HttpPost]
        public IActionResult Submit(RequestSubmitFormServiceModel requestModel)
        {
            if (!this.assetService.IsValidModel(requestModel.AssetModelId))
            {
                return Redirect(ErrorPageUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return View(requestModel);
            }

            var isRequestSubmitted = this.requestService.Submit(requestModel);

            if (isRequestSubmitted)
            {
                TempData[SuccessMessageKey] = "Request Submitted.";
            }
            else
            {
                TempData[ErrorMessageKey] = "There was an error submitting your request!";
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult All(RequestQueryServiceModel query)
        {
            var userId = !User.IsAdmin() ? User.Id() : null;

            var requests = this.requestService
                .All(query.SearchString, query.CurrentPage, userId);

            return View(requests);
        }

        [Authorize]
        public IActionResult Cancel(int id, string searchString, int currentPage)
        {
            if (!this.requestService.IsExisting(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var isCancelled = this.requestService.Cancel(id);

            TempData[SuccessMessageKey] = "Request successfully cancelled.";

            if (!isCancelled)
            {
                TempData[ErrorMessageKey] = $"There was an error cancelling request with ID {id}.";
            }

            return RedirectToAction(nameof(All), new
            {
                SearchString = searchString,
                CurrentPage = currentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Approve(int id, string searchString, int currentPage)
        {
            if (!this.requestService.IsExisting(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var targetRequest = this.requestService.GetById(id, searchString, currentPage);

            return View(targetRequest);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Approve(RequestProcessServiceModel request)
        {
            if (!this.requestService.IsExisting(request.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return View(request);
            }

            var reviewerId = this.User.Id();

            var isApproved = this.requestService.Approve(request.Id, reviewerId, request.CloseComment);

            TempData[SuccessMessageKey] = "Request successfully approved.";

            if (!isApproved)
            {
                TempData[ErrorMessageKey] = $"There was an error approving request with ID {request.Id}.";
            }

            return RedirectToAction(nameof(All), new
            {
                SearchString = request.SearchString,
                CurrentPage = request.CurrentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Reject(int id, string searchString, int currentPage)
        {
            if (!this.requestService.IsExisting(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var targetRequest = this.requestService.GetById(id, searchString, currentPage);

            return View(targetRequest);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Reject(RequestProcessServiceModel request)
        {
            if (!this.requestService.IsExisting(request.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return View(request);
            }

            var reviewerId = this.User.Id();

            var isRejected = this.requestService.Reject(request.Id, reviewerId, request.CloseComment);

            TempData[SuccessMessageKey] = "Request was rejected.";

            if (!isRejected)
            {
                TempData[ErrorMessageKey] = $"There was an error rejecting request with ID {request.Id}.";
            }

            return RedirectToAction(nameof(All), new
            {
                SearchString = request.SearchString,
                CurrentPage = request.CurrentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Details(int id, string searchString, int currentPage)
        {
            if (!this.requestService.IsExisting(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var targetRequest = this.requestService.Details(id, searchString, currentPage);

            return View(targetRequest);
        }
    }
}
