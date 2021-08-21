namespace ITAssetManager.Web.Services.Requests.Models
{
    public class RequestListingServiceModel
    {
        public int Id { get; init; }

        public string Brand { get; init; }

        public string Model { get; init; }

        public string Status { get; init; }

        public bool IsCompleted { get; set; }
    }
}
