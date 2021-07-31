using ITAssetManager.Web.Services.Categories.Models;
using System.Collections.Generic;

namespace ITAssetManager.Web.Models.Categories
{
    public class CategoriesQueryModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IEnumerable<CategoryListingServiceModel> Categories { get; set; }
    }
}
