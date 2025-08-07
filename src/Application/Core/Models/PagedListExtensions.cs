using System.Linq.Expressions;

namespace Application.Core.Models;

public static class PagedListExtensions
{
    public static async Task<PagedList<TDestination>> PaginatedListAsync<T, TDestination>(
        this IQueryable<T> repository, Expression<Func<T, bool>>? filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        where T : class
        where TDestination : class, IDto
    {
        IQueryable<T> query = repository;

        if (filter is not null)
            query = query.Where(filter);

        var list = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<TDestination>()
            .ToListAsync(cancellationToken);

        int count = await query.CountAsync(cancellationToken);

        return new PagedList<TDestination>(list, count, pageNumber, pageSize);
    }
}
