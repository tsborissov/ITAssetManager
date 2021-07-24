using ITAssetManager.Web.Models.Api.Brands;
using ITAssetManager.Web.Services.Brands;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Controllers.Api
{
    [ApiController]
    [Route("api/brands")]
    public class BrandsApiController : ControllerBase
    {
        private readonly IBrandService brandService;

        public BrandsApiController(IBrandService brandService) 
            => this.brandService = brandService;

        [HttpGet]
        public BrandQueryServiceModel All([FromQuery] AllBrandsApiRequestModel query)
            => this.brandService.All(
                query.SearchString,
                query.SortOrder,
                query.CurrentPage);
    }
}
