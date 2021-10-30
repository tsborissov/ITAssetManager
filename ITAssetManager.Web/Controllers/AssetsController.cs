using AutoMapper;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Web.Services.Assets;
using ITAssetManager.Web.Services.Assets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class AssetsController : Controller
    {
        private readonly IAssetService assetService;
        private readonly IMapper mapper;

        public AssetsController(IAssetService assetService, IMapper mapper)
        {
            this.assetService = assetService;
            this.mapper = mapper;
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Add() => View(new AssetAddFormServiceModel
        {
            Statuses = this.assetService.GetStatuses(),
            Models = this.assetService.GetModels(),
            Vendors = this.assetService.GetVendors(),
            PurchaseDate = DateTime.Now.Date,
            WarranyExpirationDate = DateTime.Now.Date.AddYears(5)
        });

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Add(AssetAddFormServiceModel assetModel)
        {
            if (!this.assetService.IsValidModel(assetModel.AssetModelId))
            {
                this.ModelState.AddModelError(nameof(assetModel.AssetModelId), InvalidModelMessage);
            }

            if (!this.assetService.IsValidStatus(assetModel.StatusId))
            {
                this.ModelState.AddModelError(nameof(assetModel.StatusId), InvalidStatusMessage);
            }

            if (!this.assetService.IsValidVendor(assetModel.VendorId))
            {
                this.ModelState.AddModelError(nameof(assetModel.VendorId), InvalidVendorMessage);
            }

            if (this.assetService.IsExistingSerialNr(assetModel.SerialNr))
            {
                this.ModelState.AddModelError(nameof(assetModel.SerialNr), InvalidSerialNumberMessage);
            }

            if (this.assetService.IsExistingInventoryNr(assetModel.InventoryNr))
            {
                this.ModelState.AddModelError(nameof(assetModel.InventoryNr), InvalidInventoryNumberMessage);
            }

            if (!this.ModelState.IsValid)
            {
                assetModel.Models = this.assetService.GetModels();
                assetModel.Statuses = this.assetService.GetStatuses();
                assetModel.Vendors = this.assetService.GetVendors();

                return View(assetModel);
            }

            var isAssetAdded = this.assetService.Add(assetModel);

            if (isAssetAdded)
            {
                TempData[SuccessMessageKey] = AssetSuccessfullyCreatedMessage;
            }
            else
            {
                TempData[ErrorMessageKey] = ErrorCreatingAssetMessage;
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult All(AssetsQueryModel query)
        {
            var userId = !User.IsAdmin() ? User.Id() : null;

            var queryResult = this.assetService
                .All(query.SearchString, query.SortOrder, query.CurrentPage, userId);

            var assets = this.mapper.Map<AssetsQueryModel>(queryResult);

            return View(assets);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Assign(int id, string searchString, string sortOrder, int currentPage)
        {
            if (!this.assetService.IsValidAsset(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var targetAsset = this.assetService.GetById(id, searchString, sortOrder, currentPage);
            targetAsset.AllUsers = this.assetService.GetAllUsers();

            return View(targetAsset);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Assign(AssetAssignServiceModel assetModel)
        {
            if (!this.assetService.IsValidAsset(assetModel.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (!this.assetService.IsValidUser(assetModel.UserId))
            {
                return Redirect(ErrorPageUrl);
            }

            var isAssetAssigned = this.assetService.Assign(assetModel.UserId, assetModel.Id);

            if (isAssetAssigned)
            {
                TempData[SuccessMessageKey] = AssetSuccessfullyAssignedMessage;
            }
            else
            {
                TempData[ErrorMessageKey] = ErrorAssigningAssetMessage;
            }

            return RedirectToAction(nameof(All), new 
            {
                SortOrder = assetModel.SortOrder,
                SearchString = assetModel.SearchString,
                CurrentPage = assetModel.CurrentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Collect(int id, string searchString, string sortOrder, int currentPage)
        {
            if (!this.assetService.IsValidAsset(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var assetQuery = this.assetService.GetUserAssetById(id);

            assetQuery.ReturnDate = DateTime.Now;
            assetQuery.SearchString = searchString;
            assetQuery.SortOrder = sortOrder;
            assetQuery.CurrentPage = currentPage;

            return View(assetQuery);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Collect(AssetCollectServiceModel assetModel)
        {
            if (!this.assetService.IsValidAsset(assetModel.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (!this.assetService.IsValidUser(assetModel.UserId))
            {
                return Redirect(ErrorPageUrl);
            }

            if (assetModel.ReturnDate < assetModel.AssignDate)
            {
                this.ModelState.AddModelError(nameof(assetModel.ReturnDate), ReturnDateBeforeAssignDateError);
            }

            if (assetModel.ReturnDate > DateTime.Now)
            {
                this.ModelState.AddModelError(nameof(assetModel.ReturnDate), FutureReturnDateError);
            }

            if (!this.ModelState.IsValid)
            {
                return View(assetModel);
            }

            var isAssetCollected = this.assetService.Collect(assetModel.UserId, assetModel.Id, assetModel.ReturnDate);

            if (isAssetCollected)
            {
                TempData[SuccessMessageKey] = AssetSuccessfullyCollectedMessage;
            }
            else
            {
                TempData[ErrorMessageKey] = ErrorCollectingAssetMessage;
            }

            return RedirectToAction(nameof(All), new
            {
                SortOrder = assetModel.SortOrder,
                SearchString = assetModel.SearchString,
                CurrentPage = assetModel.CurrentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Edit(int id, string searchString, string sortOrder, int currentPage)
        {
            if (!this.assetService.IsValidAsset(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var targetAsset = this.assetService.EditById(id, searchString, sortOrder, currentPage);

            return View(targetAsset);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Edit(AssetEditFormServiceModel assetModel)
        {
            if (!this.ModelState.IsValid)
            {
                assetModel.Models = this.assetService.GetModels();
                assetModel.Statuses = this.assetService.GetStatuses();
                assetModel.Vendors = this.assetService.GetVendors();

                return View(assetModel);
            }

            var isAssetUpdated = this.assetService.Update(assetModel);

            if (isAssetUpdated)
            {
                TempData[SuccessMessageKey] = AssetSuccessfullyUpdatedMessage;
            }
            else
            {
                TempData[ErrorMessageKey] = ErrorUpdatingAssetMessage;
            }

            return RedirectToAction(nameof(All), new
            {
                SortOrder = assetModel.SortOrder,
                SearchString = assetModel.SearchString,
                CurrentPage = assetModel.CurrentPage
            });
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Delete(int id, string searchString, string sortOrder, int currentPage)
        {
            if (!this.assetService.IsValidAsset(id) || 
                this.assetService.IsInUse(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var isDeleted = this.assetService.Delete(id);

            if (isDeleted)
            {
                TempData[SuccessMessageKey] = AssetSuccessfullyDeletedMessage;
            }
            else
            {
                TempData[ErrorMessageKey] = ErrorDeletingAssetMessage;
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
