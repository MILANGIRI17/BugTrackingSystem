using BugTracker.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BugTracker.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? _currentUser => _httpContextAccessor?.HttpContext?.User;

        public string GetUserId() => _currentUser?.FindFirst("userId")?.Value ?? string.Empty;

        public string GetUserRoles() => string.Join(",", _currentUser?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(role => role.Value) ?? new string[0]);
    }
}
