using System.Linq.Expressions;

namespace HairStudio.Repository.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Active<T>(this IQueryable<T> query, Expression<Func<T, bool>> isActivePredicate)
        {
            return query.Where(isActivePredicate);
        }

        public static IQueryable<T> Paged<T>(this IQueryable<T> query, int page, int rowsPerPage)
        {
            return query.Skip((page - 1) * rowsPerPage).Take(rowsPerPage);
        }
    }
}