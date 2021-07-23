namespace ITAssetManager.Web.Models.Api.Brands
{
    public class AllBrandsApiRequestModel
    {
        public string SearchString { get; init; }

        public string SortOrder { get; init; }

        public int CurrentPage { get; init; }

        public int BrandsPerPage { get; init; } = 10;

        public bool HasPreviousPage { get; init; }

        public bool HasNextPage { get; init; }
    }
}
