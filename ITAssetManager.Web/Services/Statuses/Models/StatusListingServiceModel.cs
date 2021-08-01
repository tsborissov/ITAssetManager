namespace ITAssetManager.Web.Services.Statuses.Models
{
    public class StatusListingServiceModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public bool IsInUse { get; set; }
    }
}
