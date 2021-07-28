using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Assets
{
    public class AssetAssignServiceModel
    {
        public int Id { get; init; }

        public string Model { get; init; }

        public string SerialNr { get; init; }

        public string InventoryNr { get; init; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        public IEnumerable<UserDropdownServiceModel> AllUsers { get; set; }
    }
}
