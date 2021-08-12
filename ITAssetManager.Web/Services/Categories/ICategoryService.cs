using ITAssetManager.Web.Services.Categories.Models;

namespace ITAssetManager.Web.Services.Categories
{
    public interface ICategoryService
    {
        CategoryQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        string Add(CategoryAddFormServiceModel categoryModel);

        CategoryEditServiceModel Details(int id);

        int Update(CategoryEditServiceModel category);

        string Delete(int id);

        bool IsExistingCategory(int id);

        public bool IsExistingName(string name);

        bool IsInUse(int id);
    }
}
