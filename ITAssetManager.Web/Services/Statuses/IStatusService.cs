namespace ITAssetManager.Web.Services.Statuses
{
    public interface IStatusService
    {
        public StatusQueryServiceModel All(string searchString, string sortOrder, int currentPage);
    }
}
