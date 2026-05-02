using Common.Utilities.Helpers;

namespace Common.Utilities.Extensions;
public static class QueryableExtensions
{
    public static IQueryable<TResult> CreatePage<TResult>(this IQueryable<TResult> query, int pageNumber, int pageSize)
    {
        Assert.NotNull(query, nameof(query));
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
