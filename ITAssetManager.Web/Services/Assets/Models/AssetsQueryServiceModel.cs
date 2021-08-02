using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Assets.Models
{
    public class AssetsQueryServiceModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IEnumerable<AssetListingServiceModel> Assets { get; set; } = new HashSet<AssetListingServiceModel>();
    }
}
