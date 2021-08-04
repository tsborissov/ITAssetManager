namespace ITAssetManager.Web.Services.Assets.Models
{
    public class AssetListingServiceModel
    {
        public int Id { get; init; }

        public string Brand { get; init; }

        public string Model { get; init; }

        public int ModelId { get; init; }

        public string SerialNr { get; init; }

        public string InventoryNr { get; init; }

        public string Status { get; init; }

        public string User { get; init; }

        public bool IsInUse { get; init; }
    }
}
