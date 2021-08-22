using System;
using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Requests.Models
{
    public class RequestListingServiceModel
    {
        public int Id { get; set; }

        public string User { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string SubmissionDate { get; set; }

        public string CompletionDate { get; set; }

        public string Status { get; set; }

        public bool IsCompleted { get; set; }

        public string CloseComment { get; set; }
    }
}
