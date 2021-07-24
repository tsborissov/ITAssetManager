using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Services.Brands
{
    public interface IBrandService
    {
        BrandQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        void Add(string name);

        BrandEditServiceModel Details(int id);

        void Update(BrandEditServiceModel brand);

        bool IsExistingBrand(int id);

        public bool IsExistingName(string name);
    }
}
