using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extentions
{
    public static class IEnumerableExtentions
    {
        public static IEnumerable<T> Paging<T>(this IEnumerable<T> query,
            int pageNumber,
            int pageSize
            ) where T : class
        {
            return query.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);
        }
    }

    public static class IQueryableExtentions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> query,
            int pageNumber,
            int pageSize
        ) where T : class
        {
            return query.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);
        }
    }
}
