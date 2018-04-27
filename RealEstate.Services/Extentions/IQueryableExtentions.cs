using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.Extentions
{
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
