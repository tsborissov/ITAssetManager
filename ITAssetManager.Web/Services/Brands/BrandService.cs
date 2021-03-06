using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Brands.Models;
using ITAssetManager.Web.Services.Common;
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

        public string Add(BrandAddFormServiceModel brandModel)
        {
            var brand = this.mapper.Map<Brand>(brandModel);

            var result = this.data.Brands.Add(brand);
            this.data.SaveChanges();

            return result.Entity.Name;
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

            var pages = Pagination.GetPages(brandsQuery, currentPage, ItemsPerPage);

            currentPage = pages.currentPage;
            var lastPage = pages.lastPage;

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

            return new BrandQueryServiceModel
            {
                Brands = brands,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = pages.hasPreviousPage,
                HasNextPage = pages.hasNextPage
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

        public int Update(BrandEditServiceModel brand)
        {
            var targetBrand = this.data
                .Brands
                .Where(b => b.Id == brand.Id)
                .FirstOrDefault();

            targetBrand.Name = brand.Name;

            return this.data.SaveChanges();
        }

        public string Delete(int id)
        {
            var targetBrand = this.data
                .Brands
                .Where(b => b.Id == id)
                .FirstOrDefault();

            var result = this.data.Brands.Remove(targetBrand);
            this.data.SaveChanges();

            return result.Entity.Name;
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
