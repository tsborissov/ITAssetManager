using ITAssetManager.Web.Services.Requests.Models;

namespace ITAssetManager.Web.Services.Requests
{
    public interface IRequestService
    {
        bool Submit(RequestSubmitFormServiceModel request);

        RequestQueryServiceModel All(string searchString, int currentPage, string userId);

        bool Cancel(int id);

        bool Approve(int id, string reviewerId, string closeComment);

        bool Reject(int id, string reviewerId, string closeComment);

        RequestProcessServiceModel GetById(int id, string searchString, int currentPage);

        RequestDetailsServiceModel Details(int id, string searchString, int currentPage);

        bool IsExisting(int id);
    }
}
