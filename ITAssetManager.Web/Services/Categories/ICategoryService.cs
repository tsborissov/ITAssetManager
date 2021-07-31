using ITAssetManager.Web.Services.Categories.Models;

namespace ITAssetManager.Web.Services.Categories
{
    public interface ICategoryService
    {
        CategoryQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        void Add(string name);

        CategoryEditServiceModel Details(int id);

        void Update(CategoryEditServiceModel category);

        bool IsExistingCategory(int id);

        public bool IsExistingName(string name);
    }
}
