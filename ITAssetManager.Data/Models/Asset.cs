using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Asset
    {
        public int Id { get; init; }

        public int AssetModelId { get; set; }

        public AssetModel AssetModel { get; set; }

        [Required]
        [MaxLength(SerialNrMaxLength)]
        public string SerialNr { get; set; }

        [Required]
        [MaxLength(InventoryNrMaxLength)]
        public string InventoryNr { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }

        public int VendorId { get; set; }

        public Vendor Vendor { get; set; }

        [Required]
        [MaxLength(InvoiceNrMaxLength)]
        public string InvoiceNr { get; set; }

        public double Price { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime WarranyExpirationDate { get; set; }

        public DateTime? DisposalDate { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<UserAsset> AssetUsers { get; init; } = new HashSet<UserAsset>();
    }
}
