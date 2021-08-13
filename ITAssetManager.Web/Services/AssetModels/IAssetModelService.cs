using ITAssetManager.Web.Services.AssetModels.Models;
using System.Collections.Generic;

namespace ITAssetManager.Web.Services.AssetModels
{
    public interface IAssetModelService
    {
        string Add(AssetModelsAddFormServiceModel assetModel);

        AssetModelQueryServiceModel All(string searchString, int currentPage);

        AssetModelDetailsServiceModel Details(int id);

        int Update(AssetModelEditFormServiceModel vendorModel);

        string Delete(int id);

        IEnumerable<BrandDropdownServiceModel> GetBrands();

        IEnumerable<CategoryDropdownServiceModel> GetCategories();

        bool IsBrandValid(int id);

        bool IsCategoryValid(int id);

        bool IsExistingModel(int id);

        bool IsInUse(int id);
    }
}
