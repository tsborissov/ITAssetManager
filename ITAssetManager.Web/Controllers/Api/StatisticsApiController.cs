using ITAssetManager.Web.Models.Api.Statistics;
using ITAssetManager.Web.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly IStatisticsService statisticsService;

        public StatisticsApiController(IStatisticsService statisticsService)
        {
            this.statisticsService = statisticsService;
        }

        [HttpGet]
        public StatisticsResponseModel Get()
        {
            return new StatisticsResponseModel
            {
                TotalUsers = this.statisticsService.TotalUsers(),
                TotalAssets = this.statisticsService.TotalAssets(),
                TotalBrands = this.statisticsService.TotalBrands(),
                TotalCategories = this.statisticsService.TotalCategories(),
                TotalModels = this.statisticsService.TotalModels(),
                TotalStatuses = this.statisticsService.TotalStatuses(),
                TotalVendors = this.statisticsService.TotalVendors()
            };
        }
    }
}
