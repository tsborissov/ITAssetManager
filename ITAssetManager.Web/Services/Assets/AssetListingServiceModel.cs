using System;
using System.ComponentModel.DataAnnotations;

namespace ITAssetManager.Web.Services.Assets
{
    public class AssetListingServiceModel
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string SerialNr { get; set; }

        public string InventoryNr { get; set; }

        public string Status { get; set; }

        public string User { get; set; }
    }
}
