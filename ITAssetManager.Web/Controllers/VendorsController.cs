using AutoMapper;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Vendors;
using ITAssetManager.Web.Services.Vendors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class VendorsController : Controller
    {
        private readonly IVendorService vendorService;
        private readonly IMapper mapper;

        public VendorsController(IVendorService vendorService, IMapper mapper)
        {
            this.vendorService = vendorService;
            this.mapper = mapper;
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Add() => View();

        [Authorize(Roles = AdministratorRoleName)]
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

            var vendorAdded = this.vendorService.Add(vendorModel);

            TempData[SuccessMessageKey] = $"New Vendor '{vendorAdded}' created.";

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult All(VendorsQueryModel query)
        {
            var queryResult = this.vendorService.All(query.SearchString, query.SortOrder, query.CurrentPage);

            var vendors = this.mapper.Map<VendorsQueryModel>(queryResult);

            return View(vendors);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Details(int id, string sortOrder, string searchString, int currentPage)
        {
            var vendor = this.vendorService.Details(id, sortOrder, searchString, currentPage);

            if (vendor == null)
            {
                return Redirect(ErrorPageUrl);
            }

            return View(vendor);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Edit(int id, string sortOrder, string searchString, int currentPage)
        {
            var vendorDetails = this.vendorService.Details(id, sortOrder, searchString, currentPage);

            if (vendorDetails == null)
            {
                return Redirect(ErrorPageUrl);
            }

            var vendor = this.mapper.Map<VendorEditServiceModel>(vendorDetails);

            return View(vendor);
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Edit(VendorEditServiceModel vendorModel)
        {
            if (!this.ModelState.IsValid)
            {
                return View(vendorModel);
            }

            if (!this.vendorService.IsExistingVendor(vendorModel.Id))
            {
                return Redirect(ErrorPageUrl);
            }

            var result = this.vendorService.Update(vendorModel);

            if (result > 0)
            {
                TempData[SuccessMessageKey] = "Vendor successfully updated.";
            }

            var vendor = this.mapper.Map<VendorDetailsServiceModel>(vendorModel);

            return RedirectToAction(nameof(Details), vendor);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Delete(int id, string sortOrder, string searchString, int currentPage)
        {
            if (!this.vendorService.IsExistingVendor(id))
            {
                return Redirect(ErrorPageUrl);
            }

            if (this.vendorService.IsInUse(id))
            {
                return Redirect(ErrorPageUrl);
            }

            var deletedVendorName = this.vendorService.Delete(id);

            TempData[SuccessMessageKey] = $"Vendor '{deletedVendorName}' successfully deleted.";

            if (deletedVendorName == null)
            {
                TempData[ErrorMessageKey] = $"There was an error deleting Vendor with ID {id}.";
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
