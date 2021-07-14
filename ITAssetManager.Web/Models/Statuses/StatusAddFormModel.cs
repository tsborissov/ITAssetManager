using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Models.Statuses
{
    public class StatusAddFormModel
    {
        [Required]
        [StringLength(StatusNameMaxLength, MinimumLength = StatusNameMinLength)]
        public string Name { get; set; }
    }
}
