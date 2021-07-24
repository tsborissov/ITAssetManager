using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class VendorsController : Controller
    {
        private readonly IVendorService vendorService;
        private readonly ItAssetManagerDbContext data;

        public VendorsController(IVendorService vendorService, ItAssetManagerDbContext data)
        {
            this.vendorService = vendorService;
            this.data = data;
        }

        [Authorize]
        public IActionResult Add() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Add(VendorAddFormModel vendorModel)
        {
            if (this.data.Vendors.Any(v => v.Name == vendorModel.Name))
            {
                this.ModelState.AddModelError(nameof(vendorModel.Name), $"Vendor '{vendorModel.Name}' already exists!");
            }

            if (this.data.Vendors.Any(v => v.Vat == vendorModel.Vat))
            {
                this.ModelState.AddModelError(nameof(vendorModel.Vat), $"Vendor with VAT '{vendorModel.Vat}' already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(vendorModel);
            }

            var vendor = new Vendor 
            {
                Name = vendorModel.Name,
                Vat = vendorModel.Vat,
                Telephone = vendorModel.Telephone,
                Email = vendorModel.Email,
                Address = vendorModel.Address
            };

            this.data.Vendors.Add(vendor);
            this.data.SaveChanges();

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult All([FromQuery] VendorsQueryModel query)
        {
            var queryResult = this.vendorService.All(query.SearchString, query.SortOrder, query.CurrentPage);

            query.Vendors = queryResult.Vendors;
            query.SearchString = queryResult.SearchString;
            query.SortOrder = queryResult.SortOrder;
            query.CurrentPage = queryResult.CurrentPage;
            query.HasNextPage = queryResult.HasNextPage;
            query.HasPreviousPage = queryResult.HasPreviousPage;

            return View(query);
        }

        [Authorize]
        public IActionResult Details(int id, string sortOrder, string searchString, int currentPage)
        {
            var vendor = this.data
                .Vendors
                .Where(v => v.Id == id)
                .Select(v => new VendorDetailsViewModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Vat = v.Vat,
                    Email = v.Email,
                    Telephone = v.Telephone,
                    Address = v.Address,
                    SearchString = searchString,
                    SortOrder = sortOrder,
                    CurrentPage = currentPage
                })
                .FirstOrDefault();

            return View(vendor);
        }

        [Authorize]
        public IActionResult Edit(int id, string sortOrder, string searchString, int currentPage)
        {
            var vendor = this.data
                .Vendors
                .Where(v => v.Id == id)
                .Select(v => new VendorEditModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Vat = v.Vat,
                    Email = v.Email,
                    Telephone = v.Telephone,
                    Address = v.Address,
                    SortOrder = sortOrder,
                    SearchString = searchString,
                    CurrentPage = currentPage
                })
                .FirstOrDefault();

            return View(vendor);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(VendorEditModel vendorModel)
        {
            if (!this.ModelState.IsValid)
            {
                return View(vendorModel);
            }

            var targetVendor = this.data
                .Vendors
                .Where(v => v.Id == vendorModel.Id)
                .FirstOrDefault();

            if (targetVendor == null)
            {
                return RedirectToAction("Error", "Home");
            }

            targetVendor.Name = vendorModel.Name;
            targetVendor.Vat = vendorModel.Vat;
            targetVendor.Telephone = vendorModel.Telephone;
            targetVendor.Email = vendorModel.Email;
            targetVendor.Address = vendorModel.Address;

            this.data.SaveChanges();

            return RedirectToAction(nameof(Details), 
                new VendorDetailsViewModel 
                {
                    Id = vendorModel.Id,
                    Name = vendorModel.Name,
                    Vat = vendorModel.Vat,
                    Telephone = vendorModel.Telephone,
                    Email = vendorModel.Email,
                    Address = vendorModel.Address,
                    SearchString = vendorModel.SearchString,
                    SortOrder = vendorModel.SortOrder,
                    CurrentPage = vendorModel.CurrentPage
                });
        }
    }
}
