using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Requests.Models
{
    public class RequestDetailsServiceModel
    {
        [Display(Name = "Request Id")]
        public int Id { get; init; }

        [Display(Name = "Requested By")]
        public string User { get; init; }

        public string Model { get; init; }

        [Display(Name = "Submitted On")]
        public string SubmissionDate { get; init; }

        public string Rationale { get; init; }

        [Display(Name = "Processed On")]
        public string CompletionDate { get; init; }

        public string Status { get; init; }

        [Display(Name = "Processed By")]
        public string Reviewer { get; init; }

        public string CloseComment { get; init; }

        public string SearchString { get; init; }

        public int CurrentPage { get; init; }
    }
}
