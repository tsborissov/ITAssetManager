using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Categories.Models;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext data;
        private readonly IMapper mapper;

        public CategoryService(AppDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public string Add(CategoryAddFormServiceModel categoryModel)
        {
            var category = this.mapper.Map<Category>(categoryModel);

            var result = this.data.Categories.Add(category);
            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public CategoryQueryServiceModel All(string searchString, string sortOrder, int currentPage)
        {
            var categoriesQuery = this.data.Categories.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                categoriesQuery = categoriesQuery
                    .Where(c =>
                        c.Name.ToLower()
                        .Contains(searchString.ToLower()));
            }

            categoriesQuery = sortOrder switch
            {
                "name_desc" => categoriesQuery.OrderByDescending(c => c.Name),
                _ => categoriesQuery.OrderBy(c => c.Name)
            };

            var itemsCount = categoriesQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            var categories = categoriesQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new CategoryListingServiceModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    IsInUse = v.AssetModels.Any()
                })
                .ToList();

            return new CategoryQueryServiceModel
            {
                Categories = categories,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < lastPage
            };
        }

        public CategoryEditServiceModel Details(int id)
            => this.data
                .Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryEditServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .FirstOrDefault();

        public int Update(CategoryEditServiceModel category)
        {
            var targetCategory = this.data.Categories.Find(category.Id);

            targetCategory.Name = category.Name;
            
            return this.data.SaveChanges();
        }

        public string Delete(int id)
        {
            var targetCategory = this.data
                .Categories
                .Where(c => c.Id == id)
                .FirstOrDefault();

            var result = this.data.Categories.Remove(targetCategory);
            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public bool IsExistingCategory(int id)
            => this.data.Categories.Any(c => c.Id == id);

        public bool IsExistingName(string name)
            => this.data.Categories.Any(c => c.Name == name);

        public bool IsInUse(int id) 
            => this.data
               .Categories
               .Where(c => c.Id == id)
               .SelectMany(c => c.AssetModels)
               .Any();
    }
}

