using ITAssetManager.Web.Services.Statuses.Models;

namespace ITAssetManager.Web.Services.Statuses
{
    public interface IStatusService
    {
        StatusQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        void Add(StatusAddFormServiceModel statusModel);

        StatusEditServiceModel Details(int id);

        void Update(StatusEditServiceModel status);

        void Delete(int id);

        bool IsExistingStatus(int id);

        public bool IsExistingName(string name);

        bool IsInUse(int id);
    }
}
