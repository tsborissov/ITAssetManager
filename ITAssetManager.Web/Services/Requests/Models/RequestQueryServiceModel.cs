using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Requests.Models
{
    public class RequestQueryServiceModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public ICollection<RequestListingServiceModel> Requests { get; set; } = new HashSet<RequestListingServiceModel>();
    }
}
