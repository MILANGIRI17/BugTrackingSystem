using BugTracker.Application.Requests;
using BugTracker.Shared.Pagination;
using System.Linq.Expressions;

namespace BugTracker.Application.Repositories
{
    public interface IRepo<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null!, string[] includingProperties = null!, CancellationToken cancellationToken = default);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null!, string[] includingProperties = null!, CancellationToken cancellationToken = default);
        Task<PagedList<T>> GetPagedAsync(IQueryObject queryObject = null!, Expression<Func<T, bool>> filter = null!, string[] includingProperties = null!, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }
}
