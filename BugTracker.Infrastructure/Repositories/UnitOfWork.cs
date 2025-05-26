using BugTracker.Application.Repositories;
using BugTracker.Infrastructure.Data;
using System.Collections;

namespace BugTracker.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        private Hashtable _repositories;

        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<int> Commit(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        public IRepo<T> Repo<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repo<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepo<T>)_repositories[type];
        }

        public Task Rollback()
        {
            dbContext.ChangeTracker.Entries().ToList().ForEach(e => e.ReloadAsync());
            return Task.CompletedTask;
        }
        public async ValueTask DisposeAsync()
        {
            await dbContext.DisposeAsync();
        }
    }
}
