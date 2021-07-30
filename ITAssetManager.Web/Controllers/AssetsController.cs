using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Web.Services.Assets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using ITAssetManager.Web.Infrastructure;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class AssetsController : Controller
    {
        private readonly IAssetService assetService;

        public AssetsController(IAssetService assetService) 
            => this.assetService = assetService;

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Add() => View(new AssetAddFormServiceModel 
        {
            Statuses = this.assetService.GetStatuses(),
            Models= this.assetService.GetModels(),
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
                this.ModelState.AddModelError(nameof(assetModel.AssetModelId), "Invalid model selected!");
            }

            if (!this.assetService.IsValidStatus(assetModel.StatusId))
            {
                this.ModelState.AddModelError(nameof(assetModel.StatusId), "Invalid status selected!");
            }

            if (!this.assetService.IsValidVendor(assetModel.VendorId))
            {
                this.ModelState.AddModelError(nameof(assetModel.VendorId), "Invalid vendor selected!");
            }

            if (this.assetService.IsExistingSerialNr(assetModel.SerialNr))
            {
                this.ModelState.AddModelError(nameof(assetModel.SerialNr), "Serial number alredy exists!");
            }

            if (this.assetService.IsExistingInventoryNr(assetModel.InventoryNr))
            {
                this.ModelState.AddModelError(nameof(assetModel.InventoryNr), "Inventory number already exists!");
            }
            
            if (!this.ModelState.IsValid)
            {
                assetModel.Models = this.assetService.GetModels();
                assetModel.Statuses = this.assetService.GetStatuses();
                assetModel.Vendors = this.assetService.GetVendors();
                
                return View(assetModel);
            }

            this.assetService.Add(assetModel);

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult All(AssetsQueryModel query)
        {
            var userId = !User.IsAdmin() ? User.Id() : null;

            var queryResult = this.assetService
                .All(query.SearchString, query.SortOrder, query.CurrentPage, userId);

            query.Assets = queryResult.Assets;
            query.SearchString = queryResult.SearchString;
            query.SortOrder = queryResult.SortOrder;
            query.CurrentPage = queryResult.CurrentPage;
            query.HasNextPage = queryResult.HasNextPage;
            query.HasPreviousPage = queryResult.HasPreviousPage;

            return View(query);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Assign(int id, string searchString, string sortOrder, int currentPage)
        {
            var targetAsset = this.assetService.AssignById(id, searchString, sortOrder, currentPage);
            targetAsset.AllUsers = this.assetService.GetAllUsers();

            return View(targetAsset);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Assign(AssetAssignServiceModel assetModel)
        {
            this.assetService.Assign(assetModel.UserId, assetModel.Id);

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
            return View(this.assetService.UserAssetById(id, searchString, sortOrder, currentPage));
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Collect(AssetCollectServiceModel assetModel)
        {
            this.assetService.Collect(assetModel.UserId, assetModel.Id);

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

            this.assetService.Update(assetModel);

            return RedirectToAction(nameof(All), new
            {
                SortOrder = assetModel.SortOrder,
                SearchString = assetModel.SearchString,
                CurrentPage = assetModel.CurrentPage
            });
        }
    }
}
