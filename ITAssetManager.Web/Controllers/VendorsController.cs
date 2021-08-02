using AutoMapper;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Vendors;
using ITAssetManager.Web.Services.Vendors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    [Authorize(Roles = AdministratorRoleName)]
    public class VendorsController : Controller
    {
        private readonly IVendorService vendorService;
        private readonly IMapper mapper;

        public VendorsController(IVendorService vendorService, IMapper mapper)
        {
            this.vendorService = vendorService;
            this.mapper = mapper;
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

            var vendors = this.mapper.Map<VendorsQueryModel>(queryResult);

            return View(vendors);
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
            var vendorDetails = this.vendorService.Details(id, sortOrder, searchString, currentPage);

            if (vendorDetails == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var vendor = this.mapper.Map<VendorEditServiceModel>(vendorDetails);

            return View(vendor);
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

            var vendor = this.mapper.Map<VendorDetailsServiceModel>(vendorModel);

            return RedirectToAction(nameof(Details), vendor);
        }
    }
}
