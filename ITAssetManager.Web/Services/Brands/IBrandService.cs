using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Services.Brands
{
    public interface IBrandService
    {
        BrandQueryServiceModel All(string searchString, string sortOrder, int currentPage, int brandsPerPage);
    }
}
