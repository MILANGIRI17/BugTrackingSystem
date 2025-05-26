using BugTracker.Infrastructure.Identity;
using BugTracker.Shared.Constants;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Text;

namespace BugTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, SignInManager<User> signManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signManager = signManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ResponseHandler<string>.FailureResopnse("Invalid information provided"));

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null) return Ok(ResponseHandler<string>.FailureResopnse("User already exist with provided email address"));

                User authUser = new User().CreateUser(request);
                var authUserResponse = await _userManager.CreateAsync(authUser, request.Password);
                if (!authUserResponse.Succeeded) return Ok(ResponseHandler<string>.FailureResopnse("registration failed."));

                var addRoleResponse = await _userManager.AddToRoleAsync(authUser, request.IsDeveloper ? DefaultUserRole.Developer : DefaultUserRole.User);
                if (!addRoleResponse.Succeeded)
                {
                    await _userManager.DeleteAsync(authUser);
                    return Ok(ResponseHandler<string>.FailureResopnse("registration failed."));
                }

                return Ok(ResponseHandler<string>.SuccessResopnse("User has been registered succefully."));
            }
            catch (Exception)
            {
                return BadRequest(ResponseHandler<string>.FailureResopnse("Invalid Request"));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ResponseHandler<string>.FailureResopnse("Invalid information provided"));

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) return Ok(ResponseHandler<string>.SuccessResopnse("user not found"));

                var signInResult = await _signManager.CheckPasswordSignInAsync(user, request.Password, true);
                if (!signInResult.Succeeded) return Ok(ResponseHandler<string>.SuccessResopnse("password is not valid"));

                var claims = new List<Claim>
                {
                    new Claim("userId", user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName)
                };
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    Issuer = _configuration["JWT:Issuer"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256),
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(ResponseHandler<UserTokenDto>.SuccessResopnse(new UserTokenDto(tokenHandler.WriteToken(token), "Bearer")));
            }
            catch (Exception)
            {
                return BadRequest(ResponseHandler<UserTokenDto>.FailureResopnse("Invalid request"));
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] UserLoginDto request)
        {
            await _signManager.SignOutAsync();
            return Ok(ResponseHandler<string>.SuccessResopnse("Succesfully Logged out"));
        }
    }
}
