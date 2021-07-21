using System.Collections.Generic;

namespace ITAssetManager.Web.Models.Vendors
{
    public class VendorsQueryModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IEnumerable<VendorListingViewModel> Vendors { get; init; }
    }
}
