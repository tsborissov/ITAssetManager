namespace ITAssetManager.Web.Services.Statistics
{
    public interface IStatisticsService
    {
        int TotalUsers();

        int TotalAssets();

        int TotalBrands();

        int TotalCategories();

        int TotalModels();

        int TotalStatuses();

        int TotalVendors();
    }
}
