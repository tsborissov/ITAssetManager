using ITAssetManager.Data;
using ITAssetManager.Data.Enums;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Requests.Models;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Requests
{
    public class RequestService : IRequestService
    {
        private readonly AppDbContext data;

        public RequestService(AppDbContext data)
        {
            this.data = data;
        }

        public bool Submit(RequestSubmitFormServiceModel request)
        {
            var requestData = new Request 
            {
                RequestorId = request.RequestorId,
                AssetModelId = request.AssetModelId,
                Status = request.Status,
                SubmissionDate = request.SubmissionDate.ToUniversalTime(),
                Rationale = request.Rationale
            };

            this.data.Requests.Add(requestData);
            var result = this.data.SaveChanges();

            return result > 0;
        }

        public RequestQueryServiceModel All(string searchString, int currentPage, string userId)
        {
            var requestsQuery = this.data.Requests.AsQueryable();

            if (userId != null)
            {
                requestsQuery = requestsQuery.Where(r => r.RequestorId == userId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {

            }

            var itemsCount = requestsQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            var requests = requestsQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(r => new RequestListingServiceModel
                {
                    Id = r.Id,
                    Brand = r.AssetModel.Brand.Name,
                    Model = r.AssetModel.Name,
                    Status = r.Status.ToString(),
                    IsCompleted = r.Status != RequestStatus.Submitted
                })
                .ToList();

            return new RequestQueryServiceModel
            {
                Requests = requests,
                SearchString = searchString,
                CurrentPage = currentPage,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < lastPage
            };
        }
    }
}
