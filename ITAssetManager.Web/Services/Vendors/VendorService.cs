using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Common;
using ITAssetManager.Web.Services.Vendors.Models;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Vendors
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext data;
        private readonly IMapper mapper;

        public VendorService(AppDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public string Add(VendorAddFormServiceModel vendorModel)
        {
            var vendor = this.mapper.Map<Vendor>(vendorModel);

            var result = this.data.Vendors.Add(vendor);
            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public VendorQueryServiceModel All(string searchString, string sortOrder, int currentPage)
        {
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

            var pages = Pagination.GetPages(vendorsQuery, currentPage, ItemsPerPage);

            currentPage = pages.currentPage;
            var lastPage = pages.lastPage;

            var vendors = vendorsQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new VendorListingServiceModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Vat = v.Vat,
                    IsInUse = v.Assets.Any()
                })
                .ToList();

            return new VendorQueryServiceModel 
            {
                Vendors = vendors,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = pages.hasPreviousPage,
                HasNextPage = pages.hasNextPage
            };
        }

        public VendorDetailsServiceModel Details(int id, string sortOrder, string searchString, int currentPage)
            => this.data
                .Vendors
                .Where(v => v.Id == id)
                .Select(v => new VendorDetailsServiceModel
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

        public int Update(VendorEditServiceModel vendorModel)
        {
            var targetVendor = this.data
                .Vendors
                .Where(v => v.Id == vendorModel.Id)
                .FirstOrDefault();

            targetVendor.Name = vendorModel.Name;
            targetVendor.Vat = vendorModel.Vat;
            targetVendor.Telephone = vendorModel.Telephone;
            targetVendor.Email = vendorModel.Email;
            targetVendor.Address = vendorModel.Address;

            return this.data.SaveChanges();
        }

        public string Delete(int id)
        {
            var targetVendor = this.data
                .Vendors
                .Where(v => v.Id == id)
                .FirstOrDefault();

            var result = this.data.Vendors.Remove(targetVendor);
            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public bool IsExistingName(string name)
            => this.data.Vendors.Any(v => v.Name == name);

        public bool IsExistingVat(string vat)
         => this.data.Vendors.Any(v => v.Vat == vat);

        public bool IsExistingVendor(int id)
            => this.data.Vendors.Any(v => v.Id == id);

        public bool IsInUse(int id)
            => this.data
            .Vendors
            .Where(v => v.Id == id)
            .SelectMany(v => v.Assets)
            .Any();
    }
}
