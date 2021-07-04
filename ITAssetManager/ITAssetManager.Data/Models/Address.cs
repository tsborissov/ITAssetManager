using System;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Address
    {
        [Key]
        [Required]
        [MaxLength(IdDefaultLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(AddressNameMaxLength)]
        public string Name { get; set; }

        [Required]
        public string TownId { get; set; }

        public Town Town { get; set; }

        public bool IsDeleted { get; set; }

        // TODO : public ICollection<User> Users { get; init; } = new HashSet<User>();
    }
}
