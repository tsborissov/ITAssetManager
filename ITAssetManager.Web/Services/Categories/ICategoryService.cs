namespace ITAssetManager.Web.Services.Categories
{
    public interface ICategoryService
    {
        CategoryQueryServiceModel All(string searchString, string sortOrder, int currentPage);
    }
}
