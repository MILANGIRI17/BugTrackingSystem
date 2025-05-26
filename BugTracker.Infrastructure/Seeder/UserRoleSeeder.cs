using BugTracker.Infrastructure.Identity;
using BugTracker.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BugTracker.Infrastructure.Seeder
{
    public static class UserRoleSeeder
    {
        public static async Task SeedRole(IServiceProvider serviceProvider)
        {
			try
			{
                using (var scope = serviceProvider.CreateScope())
                {
                    string[] roles = { DefaultUserRole.Developer, DefaultUserRole.User };
                    var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                    foreach (var role in roles)
                    {
                        if (!await _roleManager.RoleExistsAsync(role))
                            await _roleManager.CreateAsync(new Role(role));
                    }
                }
			}
			catch (Exception)
			{
                //Need to Implement
			}
        }
    }
}
