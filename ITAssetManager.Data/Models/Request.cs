using ITAssetManager.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Request
    {
        public int Id { get; init; }

        [Required]
        public string RequestorId { get; init; }

        public ApplicationUser Requestor { get; init; }

        public int AssetModelId { get; init; }

        public AssetModel AssetModel { get; init; }

        public RequestStatus Status { get; set; }

        public DateTime SubmissionDate { get; init; }

        public DateTime? CompletionDate { get; set; }

        public string ReviewerId { get; init; }

        public ApplicationUser Reviewer { get; set; }

        [Required]
        [MaxLength(RequestRationaleMaxLength)]
        public string Rationale { get; init; }

        [MaxLength(CloseCommentMaxLength)]
        public string CloseComment { get; set; }
    }
}
