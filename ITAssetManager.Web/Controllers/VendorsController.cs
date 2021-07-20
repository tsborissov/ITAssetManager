using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models;
using ITAssetManager.Web.Models.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

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
        public IActionResult All(
            string sortOrder, 
            string searchString, 
            string currentFilter,
            int? pageNumber)

        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["VatSortParm"] = sortOrder == "vat" ? "vat_desc" : "vat";

            if (String.IsNullOrEmpty(searchString))
            {
                searchString = currentFilter;
            }
            else
            {
                pageNumber = 1;
            }

            ViewData["CurrentFilter"] = searchString;

            var vendorsQuery = this.data.Vendors.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                vendorsQuery = vendorsQuery
                    .Where(v => 
                        v.Name.ToLower().Contains(searchString.ToLower()) || 
                        v.Vat.ToLower().Contains(searchString.ToLower()));
            }

            vendorsQuery = sortOrder switch
            {
                "name_desc" => vendorsQuery.OrderByDescending(v => v.Name),
                "vat" => vendorsQuery.OrderBy(s => s.Vat),
                "vat_desc" => vendorsQuery.OrderByDescending(s => s.Vat),
                _ => vendorsQuery.OrderBy(s => s.Name),
            };

            var vendors = vendorsQuery
                .Select(v => new VendorListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Vat = v.Vat
                });

            return View(PaginatedList<VendorListingViewModel>.Create(vendors, pageNumber ?? 1, ListingPageSize));
        }

        [Authorize]
        public IActionResult Details(
            int id,
            string sortOrder,
            string currentFilter,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = currentFilter;
            ViewData["CurrentPage"] = pageNumber;

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
                    Address = v.Address
                })
                .FirstOrDefault();

            return View(vendor);
        }

        [Authorize]
        public IActionResult Edit(int id)
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
                    Address = v.Address
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

            if (targetVendor != null)
            {
                targetVendor.Name = vendorModel.Name;
                targetVendor.Vat = vendorModel.Vat;
                targetVendor.Telephone = vendorModel.Telephone;
                targetVendor.Email = vendorModel.Email;
                targetVendor.Address = vendorModel.Address;

                this.data.SaveChanges();
            }
            else
            {
                return RedirectToAction(nameof(All));
            }

            return View(vendorModel);
        }
    }
}
