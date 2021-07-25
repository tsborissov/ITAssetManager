using ITAssetManager.Data;
using ITAssetManager.Web.Models.Api.Statistics;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ITAssetManager.Web.Controllers.Api
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly AppDbContext data;

        public StatisticsApiController(AppDbContext data)
        {
            this.data = data;
        }

        [HttpGet]
        public StatisticsResponseModel Get()
        {
            return new StatisticsResponseModel
            {
                TotalAssets = this.data.Assets.Count(),
                TotalBrands = this.data.Brands.Count(),
                TotalCategories = this.data.Categories.Count(),
                TotalModels = this.data.AssetModels.Count(),
                TotalStatuses = this.data.Statuses.Count(),
                TotalVendors = this.data.Vendors.Count()
            };
        }
    }
}
