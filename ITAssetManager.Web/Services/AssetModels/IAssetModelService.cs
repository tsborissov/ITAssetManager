using System.Collections.Generic;

namespace ITAssetManager.Web.Services.AssetModels
{
    public interface IAssetModelService
    {
        void Add(AssetModelsAddFormServiceModel assetModel);

        AssetModelQueryServiceModel All(string searchString, int currentPage);

        AssetModelDetailsServiceModel Details(int id);

        IEnumerable<BrandDropdownServiceModel> GetBrands();

        IEnumerable<CategoryDropdownServiceModel> GetCategories();

        bool IsBrandValid(int id);

        bool IsCategoryValid(int id);

        bool IsExistingModel(int id);
    }
}
