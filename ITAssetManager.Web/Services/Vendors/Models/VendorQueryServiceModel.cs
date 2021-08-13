using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Vendors.Models
{
    public class VendorQueryServiceModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public ICollection<VendorListingServiceModel> Vendors { get; init; }
    }
}
