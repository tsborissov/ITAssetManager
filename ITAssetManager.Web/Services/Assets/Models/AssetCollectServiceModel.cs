using System;
using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Assets.Models
{
    public class AssetCollectServiceModel
    {
        public int Id { get; init; }

        public string Model { get; init; }

        public string SerialNr { get; init; }

        public string InventoryNr { get; init; }

        [Display(Name = "Date Assigned")]
        public DateTime AssignDate { get; set; }

        [Required]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }

        public string UserId { get; set; }

        [Display(Name = "User")]
        public string UserName { get; set; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}
