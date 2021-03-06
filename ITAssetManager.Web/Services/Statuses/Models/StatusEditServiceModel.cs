using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Statuses.Models
{
    public class StatusEditServiceModel
    {
        public int Id { get; init; }

        [Required]
        [StringLength(StatusNameMaxLength, MinimumLength = StatusNameMinLength)]
        public string Name { get; init; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}
