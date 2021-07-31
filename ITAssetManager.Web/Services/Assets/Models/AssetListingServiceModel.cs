namespace ITAssetManager.Web.Services.Assets.Models
{
    public class AssetListingServiceModel
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int ModelId { get; set; }

        public string SerialNr { get; set; }

        public string InventoryNr { get; set; }

        public string Status { get; set; }

        public string User { get; set; }
    }
}
