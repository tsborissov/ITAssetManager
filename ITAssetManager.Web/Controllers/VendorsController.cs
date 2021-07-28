using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Controllers
{
    [Authorize]
    public class VendorsController : Controller
    {
        private readonly IVendorService vendorService;

        public VendorsController(IVendorService vendorService)
        {
            this.vendorService = vendorService;
        }

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(VendorAddFormServiceModel vendorModel)
        {
            if (this.vendorService.IsExistingName(vendorModel.Name))
            {
                this.ModelState.AddModelError(nameof(vendorModel.Name), $"Vendor '{vendorModel.Name}' already exists!");
            }

            if (this.vendorService.IsExistingVat(vendorModel.Vat))
            {
                this.ModelState.AddModelError(nameof(vendorModel.Vat), $"Vendor with VAT '{vendorModel.Vat}' already exists!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(vendorModel);
            }

            this.vendorService.Add(vendorModel);

            return RedirectToAction(nameof(All));
        }

        public IActionResult All(VendorsQueryModel query)
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

        public IActionResult Details(int id, string sortOrder, string searchString, int currentPage)
        {
            var vendor = this.vendorService.Details(id, sortOrder, searchString, currentPage);

            if (vendor == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(vendor);
        }


        public IActionResult Edit(int id, string sortOrder, string searchString, int currentPage)
        {
            var vendor = this.vendorService.Details(id, sortOrder, searchString, currentPage);

            if (vendor == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(new VendorEditServiceModel 
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Vat = vendor.Vat,
                Email = vendor.Email,
                Telephone = vendor.Telephone,
                Address = vendor.Address,
                CurrentPage = vendor.CurrentPage,
                SearchString = vendor.SearchString,
                SortOrder = vendor.SortOrder
            });
        }

        [HttpPost]
        public IActionResult Edit(VendorEditServiceModel vendorModel)
        {
            if (!this.ModelState.IsValid)
            {
                return View(vendorModel);
            }

            if (!this.vendorService.IsExistingVendor(vendorModel.Id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.vendorService.Update(vendorModel);

            return RedirectToAction(nameof(Details), 
                new VendorDetailsServiceModel 
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
