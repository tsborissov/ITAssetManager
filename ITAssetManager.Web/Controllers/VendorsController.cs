using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ITAssetManager.Web.Controllers
{
    public class VendorsController : Controller
    {
        private readonly ItAssetManagerDbContext data;

        public VendorsController(ItAssetManagerDbContext data)
        {
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
        public IActionResult All(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["VatSortParm"] = sortOrder == "Vat" ? "vat_desc" : "Vat";

            var vendorsQuery = this.data.Vendors.AsQueryable();

            vendorsQuery = sortOrder switch
            {
                "name_desc" => vendorsQuery.OrderByDescending(v => v.Name),
                "Vat" => vendorsQuery.OrderBy(s => s.Vat),
                "vat_desc" => vendorsQuery.OrderByDescending(s => s.Vat),
                _ => vendorsQuery.OrderBy(s => s.Name),
            };

            var vendors = vendorsQuery
                .Select(v => new VendorListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Vat = v.Vat
                })
                .ToList();

            return View(vendors);
        }
    }
}
