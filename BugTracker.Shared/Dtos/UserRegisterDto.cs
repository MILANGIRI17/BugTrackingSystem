using System.ComponentModel.DataAnnotations;

namespace BugTracker.Shared.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public bool IsDeveloper { get; set; }
    }
}
