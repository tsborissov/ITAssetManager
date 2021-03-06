using ITAssetManager.Web.Services.Assets.Models;
using System.Collections.Generic;

namespace ITAssetManager.Web.Models.Assets
{
    public class AssetsQueryModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IEnumerable<AssetListingServiceModel> Assets { get; set; } = new HashSet<AssetListingServiceModel>();
    }
}
