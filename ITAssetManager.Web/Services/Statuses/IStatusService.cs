using ITAssetManager.Web.Services.Statuses.Models;

namespace ITAssetManager.Web.Services.Statuses
{
    public interface IStatusService
    {
        StatusQueryServiceModel All(string searchString, string sortOrder, int currentPage);

        string Add(StatusAddFormServiceModel statusModel);

        StatusEditServiceModel Details(int id);

        int Update(StatusEditServiceModel status);

        string Delete(int id);

        bool IsExistingStatus(int id);

        public bool IsExistingName(string name);

        bool IsInUse(int id);
    }
}
