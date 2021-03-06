using System.Collections.Generic;

namespace ITAssetManager.Web.Services.AssetModels.Models
{
    public class AssetModelQueryServiceModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public ICollection<AssetModelListingServiceModel> AssetModels { get; init; }
    }
}
