using ITAssetManager.Data;
using ITAssetManager.Data.Enums;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Common;
using ITAssetManager.Web.Services.Requests.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
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
            var requestsQuery = this.data
                .Requests
                .OrderByDescending(r => r.Id)
                .AsQueryable();

            if (userId != null)
            {
                requestsQuery = requestsQuery.Where(r => r.RequestorId == userId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                if (Enum.TryParse(typeof(RequestStatus), searchString, true, out var searchStatus))
                {
                    requestsQuery = requestsQuery.Where(r => r.Status.Equals((RequestStatus)searchStatus));
                }
                else
                {
                    requestsQuery = requestsQuery
                    .Where(r =>
                        r.Requestor.UserName.ToLower().Contains(searchString.ToLower()) ||
                        r.Reviewer.UserName.ToLower().Contains(searchString.ToLower()) ||
                        r.AssetModel.Brand.Name.ToLower().Contains(searchString.ToLower()) ||
                        r.AssetModel.Name.ToLower().Contains(searchString.ToLower()));
                }
            }

            var pages = Pagination.GetPages(requestsQuery, currentPage, ItemsPerPage);

            currentPage = pages.currentPage;
            var lastPage = pages.lastPage;

            var requests = requestsQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(r => new RequestListingServiceModel
                {
                    Id = r.Id,
                    User = r.Requestor.UserName,
                    Brand = r.AssetModel.Brand.Name,
                    Model = r.AssetModel.Name,
                    SubmissionDate = r.SubmissionDate.ToLocalTime().ToString("d"),
                    CompletionDate = r.CompletionDate != null ? r.CompletionDate.Value.ToLocalTime().ToString("d") : "",
                    Status = r.Status.ToString(),
                    CloseComment = r.CloseComment,
                    Reviewer = r.Reviewer.UserName,
                    IsCompleted = r.Status != RequestStatus.Submitted
                })
                .ToList();

            return new RequestQueryServiceModel
            {
                Requests = requests,
                SearchString = searchString,
                CurrentPage = currentPage,
                HasPreviousPage = pages.hasPreviousPage,
                HasNextPage = pages.hasNextPage
            };
        }

        public bool Cancel(int id)
        {
            var targetRequest = this.data
                .Requests
                .Where(r => r.Id == id)
                .FirstOrDefault();

            targetRequest.Status = RequestStatus.Cancelled;
            targetRequest.CompletionDate = DateTime.UtcNow;
            targetRequest.CloseComment = RequestCancelCloseComment;

            var result = this.data.SaveChanges();

            return result > 0;
        }

        public bool Approve(int id, string reviewerId, string closeComment)
        {
            var targetRequest = this.data
                .Requests
                .Where(r => r.Id == id)
                .FirstOrDefault();

            targetRequest.Status = RequestStatus.Approved;
            targetRequest.CompletionDate = DateTime.UtcNow;
            targetRequest.CloseComment = closeComment;
            targetRequest.ReviewerId = reviewerId;

            var result = this.data.SaveChanges();

            return result > 0;
        }

        public bool Reject(int id, string reviewerId, string closeComment)
        {
            var targetRequest = this.data
                .Requests
                .Where(r => r.Id == id)
                .FirstOrDefault();

            targetRequest.Status = RequestStatus.Rejected;
            targetRequest.CompletionDate = DateTime.UtcNow;
            targetRequest.CloseComment = closeComment;
            targetRequest.ReviewerId = reviewerId;

            var result = this.data.SaveChanges();

            return result > 0;
        }

        public RequestDetailsServiceModel Details(int id, string searchString, int currentPage)
        {
            var targetRequest = this.data
                .Requests
                .Where(r => r.Id == id)
                .Select(r => new RequestDetailsServiceModel
                {
                    Id = r.Id,
                    User = r.Requestor.UserName,
                    Model = r.AssetModel.Brand.Name + " " + r.AssetModel.Name,
                    SubmissionDate = r.SubmissionDate.ToLocalTime().ToString(),
                    Rationale = r.Rationale,
                    CompletionDate = r.CompletionDate != null ? r.CompletionDate.Value.ToLocalTime().ToString() : "",
                    Status = r.Status.ToString(),
                    Reviewer = r.Reviewer.UserName,
                    CloseComment = r.CloseComment,
                    SearchString = searchString,
                    CurrentPage = currentPage
                })
                .FirstOrDefault();

            return targetRequest;
        }

        public RequestProcessServiceModel GetById(int id, string searchString, int currentPage)
        {
            var targetRequest = this.data
                .Requests
                .Where(r => r.Id == id)
                .Select(r => new RequestProcessServiceModel
                {
                    Id = r.Id,
                    User = r.Requestor.UserName,
                    Model = r.AssetModel.Brand.Name + " " + r.AssetModel.Name,
                    SubmissionDate = r.SubmissionDate.ToLocalTime(),
                    Rationale = r.Rationale,
                    SearchString = searchString,
                    CurrentPage = currentPage
                })
                .FirstOrDefault();

            return targetRequest;
        }

        public bool IsExisting(int id)
            => this.data.Requests.Any(r => r.Id == id);
    }
}