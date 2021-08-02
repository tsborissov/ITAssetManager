using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Brands.Models;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Brands
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext data;
        private readonly IMapper mapper;

        public BrandService(AppDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public void Add(BrandAddFormServiceModel brandModel)
        {
            var brand = this.mapper.Map<Brand>(brandModel);

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
                    Name = v.Name,
                    IsInUse = v.AssetModels.Any()
                })
                .ToList();

            var hasPreviousPage = currentPage > 1;
            var hasNextPage = currentPage < lastPage;

            return new BrandQueryServiceModel
            {
                Brands = brands,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage
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

        public void Delete(int id)
        {
            var targetBrand = this.data
                .Brands
                .Where(b => b.Id == id)
                .FirstOrDefault();

            this.data.Brands.Remove(targetBrand);
            this.data.SaveChanges();
        }

        public bool IsExistingBrand(int id)
            => this.data.Brands.Any(b => b.Id == id);

        public bool IsExistingName(string name)
            => this.data.Brands.Any(b => b.Name == name);

        public bool IsInUse(int id)
            => this.data
                .Brands
                .Where(b => b.Id == id)
                .SelectMany(b => b.AssetModels)
                .Any();
    }
}
