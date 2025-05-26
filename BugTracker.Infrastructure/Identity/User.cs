using BugTracker.Shared.Dtos;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Infrastructure.Identity
{
    public class User : IdentityUser
    {
        public bool IsDeveloper { get; set; }
        public User CreateUser(UserRegisterDto userCreateOrUpdateDto)
        {
            return new User
            {
                UserName = userCreateOrUpdateDto.UserName,
                Email = userCreateOrUpdateDto.Email,
                IsDeveloper = userCreateOrUpdateDto.IsDeveloper
            };
        }
    }
}
