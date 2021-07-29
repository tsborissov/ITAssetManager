using ITAssetManager.Web.Services.Assets;
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

        public string UserName { get; set; }

        public IEnumerable<AssetListingServiceModel> Assets { get; set; }
    }
}
