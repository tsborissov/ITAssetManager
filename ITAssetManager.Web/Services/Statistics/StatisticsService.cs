using ITAssetManager.Data;
using System.Linq;

namespace ITAssetManager.Web.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly AppDbContext data;

        public StatisticsService(AppDbContext data) 
            => this.data = data;

        public int TotalAssets()
            => this.data.Assets.Count();

        public int TotalBrands()
            => this.data.Brands.Count();

        public int TotalCategories()
            => this.data.Categories.Count();

        public int TotalModels()
        => this.data.AssetModels.Count();

        public int TotalStatuses()
            => this.data.Statuses.Count();

        public int TotalUsers()
            => this.data.Users.Count();

        public int TotalVendors()
            => this.data.Vendors.Count();
    }
}
