using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Assets.Models
{
    public class AssetAssignServiceModel
    {
        public int Id { get; init; }

        public string Model { get; init; }

        public string SerialNr { get; init; }

        public string InventoryNr { get; init; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<UserDropdownServiceModel> AllUsers { get; set; }
    }
}
