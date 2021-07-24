using ITAssetManager.Data;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Vendors
{
    public class VendorService : IVendorService
    {
        private readonly ItAssetManagerDbContext data;

        public VendorService(ItAssetManagerDbContext data)
        {
            this.data = data;
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
    }
}
