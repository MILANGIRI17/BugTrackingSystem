namespace BugTracker.Application.Logger
{
    public interface IExceptionLogger
    {
        void LogException(Exception exception);
    }
}
