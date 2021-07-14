using System;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Data.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(AssetNameMaxLength)]
        public string Name { get; set; }

        public int AssetCategoryId { get; set; }

        public AssetCategory AssetCategory { get; set; }

        [Required]
        [MaxLength(SerialNrMaxLength)]
        public string SerialNr { get; set; }

        [Required]
        [MaxLength(InventoryNrMaxLength)]
        public string InventoryNr { get; set; }

        [Required]
        public string AssetStatusId { get; set; }

        public AssetStatus AssetStatus { get; set; }

        [Required]
        public string VendorId { get; set; }

        public Vendor Vendor { get; set; }

        [Required]
        [MaxLength(InvoiceNrMaxLength)]
        public string InvoiceNr { get; set; }

        public double Price { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime WarranyExpirationDate { get; set; }

        public DateTime DisposalDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
