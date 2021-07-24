using System.Collections.Generic;

namespace ITAssetManager.Web.Services.Statuses
{
    public class StatusQueryServiceModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IEnumerable<StatusListingServiceModel> Statuses { get; set; }
    }
}
