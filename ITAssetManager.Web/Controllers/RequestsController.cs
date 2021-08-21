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
        public IActionResult Submit(int assetModelId)
            => View(new RequestSubmitFormServiceModel
            {
                RequestorId = this.User.Id(),
                AssetModelId = assetModelId,
                Status = RequestStatus.Submitted,
                SubmissionDate = DateTime.Now.Date
            });

        [Authorize]
        [HttpPost]
        public IActionResult Submit(RequestSubmitFormServiceModel requestModel)
        {
            if (!this.assetService.IsValidModel(requestModel.AssetModelId))
            {
                return RedirectToAction("Error", "Home");
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
    }
}
