using ITAssetManager.Data;
using ITAssetManager.Data.Models;
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
        public IActionResult All([FromQuery] VendorsQueryModel query)
        {
            var vendorsQuery = this.data.Vendors.AsQueryable();

            if (!String.IsNullOrEmpty(query.SearchString))
            {
                vendorsQuery = vendorsQuery
                    .Where(v => 
                        v.Name.ToLower().Contains(query.SearchString.ToLower()) || 
                        v.Vat.ToLower().Contains(query.SearchString.ToLower()));
            }

            vendorsQuery = query.SortOrder switch
            {
                "name_desc" => vendorsQuery.OrderByDescending(v => v.Name),
                "vat" => vendorsQuery.OrderBy(s => s.Vat),
                "vat_desc" => vendorsQuery.OrderByDescending(s => s.Vat),
                _ => vendorsQuery.OrderBy(s => s.Name),
            };

            var itemsCount = vendorsQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (query.CurrentPage < 1)
            {
                query.CurrentPage = 1;
            }

            if (query.CurrentPage > lastPage)
            {
                query.CurrentPage = lastPage;
            }

            var vendors = vendorsQuery
                .Skip((query.CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new VendorListingViewModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Vat = v.Vat
                })
                .ToList();

            return View(new VendorsQueryModel
            {
                Vendors = vendors,
                SearchString = query.SearchString,
                SortOrder = query.SortOrder,
                CurrentPage = query.CurrentPage,
                HasPreviousPage = query.CurrentPage > 1,
                HasNextPage = query.CurrentPage < lastPage
            });
        }

        [Authorize]
        public IActionResult Details(int id, string sortOrder, string searchString, int? currentPage)
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
        public IActionResult Edit(int id, string sortOrder, string searchString, int? currentPage)
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
