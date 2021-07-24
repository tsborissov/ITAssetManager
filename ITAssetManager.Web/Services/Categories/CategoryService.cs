using ITAssetManager.Data;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ItAssetManagerDbContext data;

        public CategoryService(ItAssetManagerDbContext data) 
            => this.data = data;

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
    }
}
