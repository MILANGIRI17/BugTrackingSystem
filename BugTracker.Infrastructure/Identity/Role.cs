using Microsoft.AspNetCore.Identity;

namespace BugTracker.Infrastructure.Identity
{
    public class Role : IdentityRole
    {
        public Role() { }
        public Role(string value)
        {
            Name = value;
        }
    }
}
