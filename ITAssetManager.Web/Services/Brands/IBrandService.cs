using ITAssetManager.Web.Services.Brands.Models;

namespace ITAssetManager.Web.Services.Brands
{
    public interface IBrandService
    {
        BrandQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        string Add(BrandAddFormServiceModel brandModel);

        BrandEditServiceModel Details(int id);

        int Update(BrandEditServiceModel brand);

        string Delete(int id);

        bool IsExistingBrand(int id);

        public bool IsExistingName(string name);

        public bool IsInUse(int id);
    }
}
