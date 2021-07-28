using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Assets
{
    public class AssetCollectServiceModel
    {
        public int AssetId { get; init; }

        public string Model { get; init; }

        public string SerialNr { get; init; }

        public string InventoryNr { get; init; }

        [Display(Name = "Date Assigned")]
        public string AssignDate { get; set; }

        public string UserId { get; set; }

        [Display(Name = "User")]
        public string UserName { get; set; }
    }
}
