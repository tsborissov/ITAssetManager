using System.Collections.Generic;

namespace ITAssetManager.Web.Models.Statuses
{
    public class StatusesQueryModel
    {
        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public IEnumerable<StatusListingViewModel> Statuses { get; init; }
    }
}
