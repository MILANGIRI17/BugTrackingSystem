using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using BugTracker.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BugTracker.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiSettingsHelper _settings;
        public AccountController(ApiSettingsHelper settings)
        {
            _settings = settings;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(_settings.BaseUrl + "auth/login"),
                        Content = content
                    };
                    var httpMessageResponse = await httpClient.SendAsync(request);
                    var readContent = await httpMessageResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseHandler<UserTokenDto>>(readContent);
                    if (result?.Data != null && !string.IsNullOrEmpty(result.Data.AccessToken))
                    {

                        HttpContext.Session.SetString("AccessToken", result.Data.AccessToken);
                        HttpContext.Session.SetString("TokenType", result.Data.Type);
                        return RedirectToAction("Index", "Bug");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = result?.Message;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDto userRegister)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(userRegister), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(_settings.BaseUrl + "auth/register"),
                        Content = content
                    };
                    var httpMessageResponse = await httpClient.SendAsync(request);
                    var resposeContent = await httpMessageResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseHandler<string>>(resposeContent);
                    if (result != null && result.IsSuccess)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    ViewData["ErrorMessage"] = result?.Message ?? "Something went wrong";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
