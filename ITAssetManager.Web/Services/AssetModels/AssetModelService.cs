using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.AssetModels
{
    public class AssetModelService : IAssetModelService
    {
        private readonly AppDbContext data;

        public AssetModelService(AppDbContext data)
            => this.data = data;

        public void Add(AssetModelsAddFormServiceModel assetModel)
        {
            this.data.AssetModels.Add(
                new AssetModel
                {
                    BrandId = assetModel.BrandId,
                    CategoryId = assetModel.CategoryId,
                    Name = assetModel.Name,
                    ImageUrl = assetModel.ImageUrl,
                    Details = assetModel.Details
                });

            this.data.SaveChanges();
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
                .Select(m => new AssetModelListingServiceModel
                {
                    Id = m.Id,
                    Category = m.Category.Name,
                    Brand = m.Brand.Name,
                    Name = m.Name,
                    Details = m.Details,
                    ImageUrl = m.ImageUrl
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
                .Where(m => m.Id == id)
                .Select(m => 
                    new AssetModelDetailsServiceModel
                    {
                        Id = m.Id,
                        Brand = m.Brand.Name,
                        Category = m.Category.Name,
                        Name = m.Name,
                        Details = m.Details,
                        ImageUrl = m.ImageUrl,
                    })
                .FirstOrDefault();
        }

        public void Update(AssetModelEditFormServiceModel assetModel)
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

            this.data.SaveChanges();
        }

        public IEnumerable<BrandDropdownServiceModel> GetBrands()
            => this.data
                   .Brands
                   .OrderBy(b => b.Name)
                   .Select(b => new BrandDropdownServiceModel
                   {
                       Id = b.Id,
                       Name = b.Name
                   })
                   .ToList();

        public IEnumerable<CategoryDropdownServiceModel> GetCategories()
            => this.data
                    .Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDropdownServiceModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList();

        public bool IsBrandValid(int id)
            => this.data.Brands.Any(b => b.Id == id);

        public bool IsCategoryValid(int id)
            => this.data.Categories.Any(c => c.Id == id);

        public bool IsExistingModel(int id)
            => this.data.AssetModels.Any(m => m.Id == id);
    }
}
