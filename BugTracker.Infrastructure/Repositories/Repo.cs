using BugTracker.Application.Repositories;
using BugTracker.Application.Requests;
using BugTracker.Infrastructure.Data;
using BugTracker.Infrastructure.Extensions;
using BugTracker.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BugTracker.Infrastructure.Repositories
{
    public class Repo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public IQueryable<T> Entities => _dbSet;

        public Repo(AppDbContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string[] includingProperties = null!, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includingProperties != null)
            {
                foreach (var item in includingProperties)
                {
                    query = query.Include(item);
                }
            }
            return await query
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter = null, string[] includingProperties = null!, CancellationToken cancellationToken = default)
        {
            if (filter == null) return null;
            var query = _dbSet.AsQueryable();
            if (includingProperties != null)
            {
                foreach (var item in includingProperties)
                {
                    query = query.Include(item);
                }
            }
            return await query.FirstOrDefaultAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<PagedList<T>> GetPagedAsync(IQueryObject queryObject = null, Expression<Func<T, bool>> filter = null, string[] includingProperties = null!, CancellationToken cancellationToken = default)
        {
            var result = new PagedList<T>();
            var query = _dbSet
                .AsQueryable();
            if (includingProperties != null)
            {
                foreach (var item in includingProperties)
                {
                    query = query.Include(item);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            result.TotalCount = await query.CountAsync(cancellationToken);
            if (queryObject != null)
            {
                query = query.ApplyOrdering(queryObject);
                query = query.ApplyPaging(queryObject);
            }
            result.Items = await query
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return result;
        }

       
    }
}
