using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Statuses.Models
{
    public class StatusAddFormServiceModel
    {
        [Required]
        [StringLength(StatusNameMaxLength, MinimumLength = StatusNameMinLength)]
        public string Name { get; set; }
    }
}
