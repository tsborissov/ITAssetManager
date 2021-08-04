namespace ITAssetManager.Web.Services.AssetModels.Models
{
    public class AssetModelDetailsServiceModel
    {
        public int Id { get; init; }

        public string Category { get; init; }

        public string Brand { get; init; }

        public string Name { get; init; }

        public string ImageUrl { get; init; }

        public string Details { get; init; }

        public bool IsInUse { get; init; }

        public string SearchString { get; set; }

        public string SortOrder { get; set; }

        public int CurrentPage { get; set; }
    }
}
