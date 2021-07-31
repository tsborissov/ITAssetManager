using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Assets.Models
{
    public class AssetEditFormServiceModel
    {
        public int Id { get; set; }

        [Display(Name = "Model")]
        public int AssetModelId { get; init; }

        [BindNever]
        public IEnumerable<AssetModelDropdownServiceModel> Models { get; set; }

        [Required]
        [Display(Name = "Serial Number")]
        [StringLength(SerialNrMaxLength, MinimumLength = SerialNrMinLength)]
        public string SerialNr { get; init; }

        [Required]
        [Display(Name = "Inventory Number")]
        [RegularExpression(InventoryNrRegexPattern)]
        public string InventoryNr { get; init; }

        [Display(Name = "Status")]
        public int StatusId { get; init; }

        [BindNever]
        public IEnumerable<StatusDropdownServiceModel> Statuses { get; set; }

        [Display(Name = "Vendor")]
        public int VendorId { get; init; }

        [BindNever]
        public IEnumerable<VendorDropdownServiceModel> Vendors { get; set; }

        [Required]
        [Display(Name = "Invoice")]
        [StringLength(InvoiceNrMaxLength, MinimumLength = InvoiceNrMinLength)]
        public string InvoiceNr { get; init; }

        [Range(PriceMinValue, PriceMaxValue)]
        public double Price { get; init; }

        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; init; }

        [DataType(DataType.Date)]
        [Display(Name = "Warrany Expiration Date")]
        public DateTime WarranyExpirationDate { get; init; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}
