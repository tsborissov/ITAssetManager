using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.AssetModels;
using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Web.Models.Statuses;
using ITAssetManager.Web.Models.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITAssetManager.Web.Controllers
{
    public class AssetsController : Controller
    {
        private readonly ItAssetManagerDbContext data;

        public AssetsController(ItAssetManagerDbContext data)
            => this.data = data;


        [Authorize]
        public IActionResult Add() => View(new AssetAddFormModel 
        {
            Statuses = this.GetStatuses(),
            Models= this.GetModels(),
            Vendors = this.GetVendors(),
            PurchaseDate = DateTime.Now.Date,
            WarranyExpirationDate = DateTime.Now.Date.AddYears(5)
        });

        [Authorize]
        [HttpPost]
        public IActionResult Add(AssetAddFormModel assetModel)
        {
            if (!this.data.AssetModels.Any(a => a.Id == assetModel.AssetModelId))
            {
                this.ModelState.AddModelError(nameof(assetModel.AssetModelId), "Invalid model selected!");
            }

            if (!this.data.Statuses.Any(s => s.Id == assetModel.StatusId))
            {
                this.ModelState.AddModelError(nameof(assetModel.StatusId), "Invalid status selected!");
            }

            if (!this.data.Vendors.Any(v => v.Id == assetModel.VendorId))
            {
                this.ModelState.AddModelError(nameof(assetModel.VendorId), "Invalid vendor selected!");
            }

            if (this.data.Assets.Any(s => s.SerialNr == assetModel.SerialNr))
            {
                this.ModelState.AddModelError(nameof(assetModel.SerialNr), "Serial number alredy exists!");
            }

            if (this.data.Assets.Any(i => i.InventoryNr == assetModel.InventoryNr))
            {
                this.ModelState.AddModelError(nameof(assetModel.InventoryNr), "Inventory number already exists!");
            }
            
            if (!this.ModelState.IsValid)
            {
                assetModel.Models = this.GetModels();
                assetModel.Statuses = this.GetStatuses();
                assetModel.Vendors = this.GetVendors();
                
                return View(assetModel);
            }

            var asset = new Asset 
            {
                AssetModelId = assetModel.AssetModelId,
                InventoryNr = assetModel.InventoryNr,
                InvoiceNr = assetModel.InvoiceNr,
                Price = assetModel.Price,
                PurchaseDate = assetModel.PurchaseDate,
                SerialNr = assetModel.SerialNr,
                StatusId = assetModel.StatusId,
                VendorId = assetModel.VendorId,
                WarranyExpirationDate = assetModel.WarranyExpirationDate
            };

            this.data.Assets.Add(asset);
            this.data.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private IEnumerable<StatusDropdownViewModel> GetStatuses()
            => this.data
                .Statuses
                .OrderBy(s => s.Name)
                .Select(s => new StatusDropdownViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToList();

        private IEnumerable<AssetModelDropdownViewModel> GetModels()
            => this.data
                .AssetModels
                .OrderBy(a => a.Name)
                .Select(a => new AssetModelDropdownViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToList();

        private IEnumerable<VendorDropdownViewModel> GetVendors()
            => this.data
                .Vendors
                .OrderBy(v => v.Name)
                .Select(v => new VendorDropdownViewModel
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .ToList();
    }
}
