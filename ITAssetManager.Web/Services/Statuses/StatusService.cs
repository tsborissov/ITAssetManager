using ITAssetManager.Data;
using System;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Services.Statuses
{
    public class StatusService : IStatusService
    {
        private readonly ItAssetManagerDbContext data;

        public StatusService(ItAssetManagerDbContext data)
        {
            this.data = data;
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
                    Name = v.Name
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
    }
}
