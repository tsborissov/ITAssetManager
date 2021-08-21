using ITAssetManager.Web.Services.Requests.Models;

namespace ITAssetManager.Web.Services.Requests
{
    public interface IRequestService
    {
        bool Submit(RequestSubmitFormServiceModel request);

        RequestQueryServiceModel All(string searchString, int currentPage, string userId);
    }
}
