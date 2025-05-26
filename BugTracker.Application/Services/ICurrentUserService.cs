namespace BugTracker.Application.Services
{
    public interface ICurrentUserService
    {
        string GetUserId();

        string GetUserRoles();
    }
}
