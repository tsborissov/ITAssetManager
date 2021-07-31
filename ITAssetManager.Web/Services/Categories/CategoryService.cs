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

        public CategoryService(AppDbContext data)
            => this.data = data;

        public void Add(string name)
        {
            var category = new Category
            {
                Name = name
            };

            this.data.Categories.Add(category);
            this.data.SaveChanges();
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
                    Name = v.Name
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

        public void Update(CategoryEditServiceModel category)
        {
            var targetCategory = this.data.Categories.Find(category.Id);

            targetCategory.Name = category.Name;
            this.data.SaveChanges();
        }

        public bool IsExistingCategory(int id)
            => this.data.Categories.Any(c => c.Id == id);

        public bool IsExistingName(string name)
            => this.data.Categories.Any(c => c.Name == name);
    }
}
