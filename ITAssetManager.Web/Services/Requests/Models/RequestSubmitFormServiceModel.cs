using ITAssetManager.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Requests.Models
{
    public class RequestSubmitFormServiceModel
    {
        [Display(Name = "Requestor Id")]
        public string RequestorId { get; init; }

        [Display(Name = "Model Id")]
        public int AssetModelId { get; init; }

        public RequestStatus Status { get; init; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; init; }

        [Required]
        [StringLength(RequestRationaleMaxLength, MinimumLength = RequestRationaleMinLength)]
        public string Rationale { get; init; }
    }
}
