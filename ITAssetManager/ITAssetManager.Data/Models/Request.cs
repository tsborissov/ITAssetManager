using System;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Request
    {
        [Key]
        [Required]
        [MaxLength(IdDefaultLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        public string Details { get; set; }

        public DateTime SubmitDate { get; set; }

        public DateTime ApproveDate { get; set; }

        public DateTime HandoverDate { get; set; }

        [Required]
        public string AssetCategoryId { get; set; }

        public AssetCategory AssetCategory { get; set; }

        public bool IsApproved { get; set; }

        public bool IsDeleted { get; set; }
    }
}