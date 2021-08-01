using ITAssetManager.Web.Services.Brands.Models;

namespace ITAssetManager.Web.Services.Brands
{
    public interface IBrandService
    {
        BrandQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        void Add(string name);

        BrandEditServiceModel Details(int id);

        void Update(BrandEditServiceModel brand);

        void Delete(int id);

        bool IsExistingBrand(int id);

        public bool IsExistingName(string name);

        public bool IsInUse(int id);
    }
}
