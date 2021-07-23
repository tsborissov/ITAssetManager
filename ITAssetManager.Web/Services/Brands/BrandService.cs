using ITAssetManager.Data;
using System;
using System.Linq;

namespace ITAssetManager.Web.Services.Brands
{
    public class BrandService : IBrandService
    {
        private readonly ItAssetManagerDbContext data;

        public BrandService(ItAssetManagerDbContext data) 
            => this.data = data;

        public BrandQueryServiceModel All(string searchString, string sortOrder, int currentPage, int brandsPerPage)
        {
            var brandsQuery = this.data
                .Brands
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                brandsQuery = brandsQuery
                    .Where(b =>
                        b.Name.ToLower()
                        .Contains(searchString.ToLower()));
            }

            brandsQuery = sortOrder switch
            {
                "name_desc" => brandsQuery.OrderByDescending(b => b.Name),
                _ => brandsQuery.OrderBy(b => b.Name)
            };

            var itemsCount = brandsQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)brandsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            var brands = brandsQuery
                .Skip((currentPage - 1) * brandsPerPage)
                .Take(brandsPerPage)
                .Select(v => new BrandListingServiceModel
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .ToList();

            return new BrandQueryServiceModel
            {
                Brands = brands,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < lastPage
            };
        }
    }
}
