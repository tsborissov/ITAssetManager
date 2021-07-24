﻿using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Categories
{
    public class CategoryQueryServiceModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IEnumerable<CategoryListingServiceModel> Categories { get; init; }
    }
}
