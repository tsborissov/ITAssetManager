namespace ITAssetManager.Web.Services.Statuses
{
    public interface IStatusService
    {
        StatusQueryServiceModel All(string searchString, string sortOrder, int currentPage);
    }
}
