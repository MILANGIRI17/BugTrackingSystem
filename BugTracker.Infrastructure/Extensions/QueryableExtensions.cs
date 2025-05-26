using BugTracker.Application.Requests;
using BugTracker.Shared.Constants;
using System.Linq.Dynamic.Core;

namespace BugTracker.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, IQueryObject queryObj)
        {
            if (queryObj.Page <= 0)
                queryObj.Page = 1;
            if (queryObj.PageSize <= 0)
                queryObj.PageSize = Pagination.PAGE_SIZE;
            if (queryObj.PageSize > Pagination.PAGE_SIZE_MAX)
                queryObj.PageSize = Pagination.PAGE_SIZE_MAX;
            return query.Skip((queryObj.Page - 1) * queryObj.PageSize).Take(queryObj.PageSize);
        }

        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, IQueryObject queryObj)
        {
            if (string.IsNullOrWhiteSpace(queryObj.SortBy))
                return query;
            return query.OrderBy(queryObj.SortBy);
        }
    }
}
