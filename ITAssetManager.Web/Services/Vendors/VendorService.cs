using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Vendors
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext data;

        public VendorService(AppDbContext data)
        {
            this.data = data;
        }

        public int Add(VendorAddFormServiceModel vendorModel)
        {
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

            return vendor.Id;
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

            var itemsCount = vendorsQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            var vendors = vendorsQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new VendorListingServiceModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Vat = v.Vat
                })
                .ToList();

            return new VendorQueryServiceModel 
            {
                Vendors = vendors,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < lastPage
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

        public void Update(VendorEditServiceModel vendorModel)
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

            this.data.SaveChanges();
        }

        public bool IsExistingName(string name)
            => this.data.Vendors.Any(v => v.Name == name);

        public bool IsExistingVat(string vat)
         => this.data.Vendors.Any(v => v.Vat == vat);

        public bool IsExistingVendor(int id)
            => this.data.Vendors.Any(v => v.Id == id);
    }
}
