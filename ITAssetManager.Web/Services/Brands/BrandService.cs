using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Brands
{
    public class BrandService : IBrandService
    {
        private readonly ItAssetManagerDbContext data;

        public BrandService(ItAssetManagerDbContext data) 
            => this.data = data;

        public void Add(string name)
        {
            var brand = new Brand
            {
                Name = name
            };

            this.data.Brands.Add(brand);
            this.data.SaveChanges();
        }

        public BrandQueryServiceModel All(string searchString, string sortOrder, int currentPage)
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
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            var brands = brandsQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
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

        public BrandEditServiceModel Details(int id)
            => this.data
                .Brands
                .Where(b => b.Id == id)
                .Select(b => new BrandEditServiceModel
                {
                    Id = b.Id,
                    Name = b.Name,
                })
                .FirstOrDefault();

        public void Update(BrandEditServiceModel brand)
        {
            var targetBrand = this.data
                .Brands
                .Where(b => b.Id == brand.Id)
                .FirstOrDefault();

            targetBrand.Name = brand.Name;
            this.data.SaveChanges();
        }

        public bool IsExistingBrand(int id)
            => this.data.Brands.Any(b => b.Id == id);

        public bool IsExistingName(string name)
            => this.data.Brands.Any(b => b.Name == name);
    }
}
