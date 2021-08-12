using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Statuses.Models;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Statuses
{
    public class StatusService : IStatusService
    {
        private readonly AppDbContext data;
        private readonly IMapper mapper;

        public StatusService(AppDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public string Add(StatusAddFormServiceModel statusModel)
        {
            var status = this.mapper.Map<Status>(statusModel);

            var result = this.data.Statuses.Add(status);
            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public StatusQueryServiceModel All(string searchString, string sortOrder, int currentPage)
        {
            var statusesQuery = this.data.Statuses.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                statusesQuery = statusesQuery
                    .Where(s =>
                        s.Name.ToLower().Contains(searchString.ToLower()));
            }

            statusesQuery = sortOrder switch
            {
                "name_desc" => statusesQuery.OrderByDescending(s => s.Name),
                _ => statusesQuery.OrderBy(s => s.Name)
            };

            var itemsCount = statusesQuery.Count();
            var lastPage = (int)Math.Ceiling(itemsCount / (double)ItemsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            var statuses = statusesQuery
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(v => new StatusListingServiceModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    IsInUse = v.Assets.Any()
                })
                .ToList();

            return new StatusQueryServiceModel 
            {
                Statuses = statuses,
                SearchString = searchString,
                SortOrder = sortOrder,
                CurrentPage = currentPage,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < lastPage
            };
        }

        public StatusEditServiceModel Details(int id)
            => this.data
                .Statuses
                .Where(s => s.Id == id)
                .Select(s => new StatusEditServiceModel
                {
                    Id = s.Id,
                    Name = s.Name,
                })
                .FirstOrDefault();

        public int Update(StatusEditServiceModel status)
        {
            var targetStatus = this.data
                .Statuses
                .Where(s => s.Id == status.Id)
                .FirstOrDefault();

            targetStatus.Name = status.Name;

            return this.data.SaveChanges();
        }

        public string Delete(int id)
        {
            var targetStatus = this.data
                .Statuses
                .Where(s => s.Id == id)
                .FirstOrDefault();

            var result = this.data.Statuses.Remove(targetStatus);
            this.data.SaveChanges();

            return result.Entity.Name;
        }

        public bool IsExistingName(string name)
            => this.data.Statuses.Any(s => s.Name == name);

        public bool IsExistingStatus(int id)
            => this.data.Statuses.Any(s => s.Id == id);

        public bool IsInUse(int id)
            => this.data
                .Statuses
                .Where(s => s.Id == id)
                .SelectMany(s => s.Assets)
                .Any();
    }
}
