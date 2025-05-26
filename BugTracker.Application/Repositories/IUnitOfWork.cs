namespace BugTracker.Application.Repositories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepo<T> Repo<T>() where T : class;
        Task<int> Commit(CancellationToken cancellationToken = default);
        Task Rollback();
    }
}
