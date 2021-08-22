using ITAssetManager.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Requests.Models
{
    public class RequestProcessServiceModel
    {
        public int Id { get; set; }

        public string User { get; set; }

        public string Model { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; init; }

        public string Rationale { get; init; }

        [Required]
        [StringLength(CloseCommentMaxLength, MinimumLength = CloseCommentMinLength)]
        public string CloseComment { get; set; }

        public string SearchString { get; set; }

        public int CurrentPage { get; set; }
    }
}
