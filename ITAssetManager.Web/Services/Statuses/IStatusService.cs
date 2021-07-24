namespace ITAssetManager.Web.Services.Statuses
{
    public interface IStatusService
    {
        StatusQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        void Add(string name);

        StatusEditServiceModel Details(int id);

        void Update(StatusEditServiceModel status);

        bool IsExistingStatus(int id);

        public bool IsExistingName(string name);
    }
}
