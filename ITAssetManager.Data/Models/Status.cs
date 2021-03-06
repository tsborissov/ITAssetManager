using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Status
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(StatusNameMaxLength)]
        public string Name { get; set; }

        public ICollection<Asset> Assets { get; init; } = new HashSet<Asset>();
    }
}