using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.AssetModels.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.AssetModels
{
    public class AssetModelService : IAssetModelService
    {
        private readonly AppDbContext data;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache; 

        public AssetModelService(AppDbContext data, IMapper mapper, IMemoryCache cache)
        {
            this.data = data;
            this.mapper = mapper;
            this.cache = cache;
        }

        public string Add(AssetModelsAddFormServiceModel assetModel)
        {
            var assetModelData = this.mapper.Map<AssetModel>(assetModel);

            var result = this.data.AssetModels.Add(assetModelData);

            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public AssetModelQueryServiceModel All(string searchString, int currentPage)
        {
            var assetModelsQuery = this.data
                .AssetModels
                .OrderBy(m => m.Brand.Name)
                .ThenBy(m => m.Name)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                assetModelsQuery = assetModelsQuery
                    .Where(m => 
                    m.Name.ToLower().Contains(searchString.ToLower()) ||
                    m.Brand.Name.ToLower().Contains(searchString.ToLower()) ||
                    m.Category.Name.ToLower().Contains(searchString.ToLower()));
            }

            var itemsCount = assetModelsQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ModelsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            var assetModels = assetModelsQuery
                .Skip((currentPage - 1) * ModelsPerPage)
                .Take(ModelsPerPage)
                .Select(am => new AssetModelListingServiceModel
                {
                    Id = am.Id,
                    Category = am.Category.Name,
                    Brand = am.Brand.Name,
                    Name = am.Name,
                    Details = am.Details,
                    ImageUrl = am.ImageUrl,
                })
                .ToList();

            return new AssetModelQueryServiceModel 
            {
                AssetModels = assetModels,
                SearchString = searchString,
                CurrentPage = currentPage,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < lastPage
            };
        }

        public AssetModelDetailsServiceModel Details(int id)
        {
            return this.data
                .AssetModels
                .Where(am => am.Id == id)
                .Select(am =>
                    new AssetModelDetailsServiceModel
                    {
                        Id = am.Id,
                        Brand = am.Brand.Name,
                        Category = am.Category.Name,
                        Name = am.Name,
                        Details = am.Details,
                        ImageUrl = am.ImageUrl,
                        IsInUse = am.Assets.Any()
                    })
                .FirstOrDefault();
        }

        public int Update(AssetModelEditFormServiceModel assetModel)
        {
            var targetAssetModel = this.data
                .AssetModels
                .Where(m => m.Id == assetModel.Id)
                .FirstOrDefault();

            targetAssetModel.BrandId = assetModel.BrandId;
            targetAssetModel.CategoryId = assetModel.CategoryId;
            targetAssetModel.Details = assetModel.Details;
            targetAssetModel.ImageUrl = assetModel.ImageUrl;
            targetAssetModel.Name = assetModel.Name;

            return this.data.SaveChanges();
        }

        public string Delete(int id)
        {
            var targetAssetModel = this.data
                .AssetModels
                .Where(am => am.Id == id)
                .FirstOrDefault();

            var result = this.data.AssetModels.Remove(targetAssetModel);
            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public IEnumerable<BrandDropdownServiceModel> GetBrands()
        {
            const string allBrandsCacheKey = "BrandsCacheKey";

            var allBrands = this.cache.Get<List<BrandDropdownServiceModel>>(allBrandsCacheKey);

            if (allBrands == null)
            {
                allBrands = this.data
                              .Brands
                              .OrderBy(b => b.Name)
                              .Select(b => new BrandDropdownServiceModel
                              {
                                  Id = b.Id,
                                  Name = b.Name
                              })
                              .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                this.cache.Set(allBrandsCacheKey, allBrands, cacheOptions);
            }

            return allBrands;
        }

        public IEnumerable<CategoryDropdownServiceModel> GetCategories()
        {
            const string allCategoriesCacheKey = "CategoriesCacheKey";

            var allCategories = this.cache.Get<List<CategoryDropdownServiceModel>>(allCategoriesCacheKey);

            if (allCategories == null)
            {
                allCategories = this.data
                               .Categories
                               .OrderBy(c => c.Name)
                               .Select(c => new CategoryDropdownServiceModel
                               {
                                   Id = c.Id,
                                   Name = c.Name
                               })
                               .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                this.cache.Set(allCategoriesCacheKey, allCategories, cacheOptions);
            }

            return allCategories;
        }

        public bool IsBrandValid(int id)
            => this.data.Brands.Any(b => b.Id == id);

        public bool IsCategoryValid(int id)
            => this.data.Categories.Any(c => c.Id == id);

        public bool IsExistingModel(int id)
            => this.data.AssetModels.Any(m => m.Id == id);

        public bool IsInUse(int id)
            => this.data
                .AssetModels
                .Where(am => am.Id == id)
                .SelectMany(am => am.Assets)
                .Any();
    }
}
