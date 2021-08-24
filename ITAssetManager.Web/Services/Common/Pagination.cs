using System;
using System.Linq;

namespace ITAssetManager.Web.Services.Common
{
    public static class Pagination
    {
        public static (int currentPage, int lastPage, bool hasPreviousPage, bool hasNextPage) GetPages(IQueryable<object> query, int currentPage, int itemsPerPage)
        {
            var itemsCount = query.Count();

            var lastPage = (int)Math.Ceiling(itemsCount / (double)itemsPerPage);

            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            bool hasPreviousPage = currentPage > 1;

            bool hasNextPage = currentPage < lastPage;

            return (currentPage, lastPage, hasPreviousPage, hasNextPage);
        }
    }
}
