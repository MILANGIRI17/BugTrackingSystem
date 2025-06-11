using BugTracker.Shared.Dtos;
using BugTracker.Shared.Enum;
using BugTracker.Shared.Helper;
using BugTracker.Shared.Pagination;
using BugTracker.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace BugTracker.Web.Controllers
{
    public class BugController : Controller
    {
        private readonly ApiSettingsHelper _settings;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BugController(ApiSettingsHelper settings, HttpClient httpClient, IWebHostEnvironment webHostEnvironment)
        {
            _settings = settings;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? searchText)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                ViewData["Roles"] = GetRolesFromToken(token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var getDeveloperResponse = await _httpClient.GetAsync("bug/get-developers");
                List<UserDto> developers = new List<UserDto>();
                if (getDeveloperResponse.IsSuccessStatusCode)
                {
                    var developerResult = await getDeveloperResponse.Content.ReadFromJsonAsync<ResponseHandler<List<UserDto>>>();
                    developers = developerResult?.Data ?? new List<UserDto>();
                }

                var url = "bug/get-paged-bugs";
                if (!string.IsNullOrEmpty(searchText))
                {
                    url += $"?Search={searchText}";
                }
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseHandler<PagedList<BugDto>>>();
                    var pagedBug = result?.Data ?? new PagedList<BugDto>();
                    foreach (var bug in pagedBug.Items)
                    {
                        if (!string.IsNullOrEmpty(bug.AssignToUserId))
                        {
                            var assignedUser =
                            bug.Assignee = developers.FirstOrDefault(u => u.Id == bug.AssignToUserId)?.UserName ?? string.Empty;
                        }
                    }
                    return View(result?.Data);
                }
                return View(new PagedList<BugDto>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> CreateAsync()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var getDeveloperResponse = await _httpClient.GetAsync("bug/get-developers");
            if (getDeveloperResponse.IsSuccessStatusCode)
            {
                var developerResult = await getDeveloperResponse.Content.ReadFromJsonAsync<ResponseHandler<List<UserDto>>>();
                ViewBag.Developers = new SelectList(developerResult?.Data ?? new List<UserDto>(),"Id", "UserName");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BugCreateOrUpdateDto bugReportDto)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //Uploading attachments;
                var attachments = await FileUploader.UploadFiles(bugReportDto.FileAttachments, _webHostEnvironment.WebRootPath);
                if (attachments != null && attachments.Count > 0)
                {
                    bugReportDto.Attachments = attachments;
                }
                bugReportDto.FileAttachments = null; // Clear the file attachments after uploading
                if (bugReportDto.Id == null) bugReportDto.Id = Guid.NewGuid().ToString();
                var httpMessageResponse = await _httpClient.PostAsJsonAsync("bug/add-bug", bugReportDto);
                var resposeContent = await httpMessageResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseHandler<string>>(resposeContent);
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "Bug");
                }
                TempData["ErrorMessage"] = result?.Message ?? "Something went wrong";
                return View(bugReportDto);
            }
            catch
            {
                return View();
            }
        }


        // GET: BugController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var token = HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            ViewData["Roles"] = GetRolesFromToken(token);
            var response = await _httpClient.GetAsync($"bug/get-bug/{id}");
            var userResponse = await _httpClient.GetAsync("bug/get-developers");
            List<UserDto> users = new List<UserDto>();
            if (userResponse.IsSuccessStatusCode)
            {
                var content = await userResponse.Content.ReadFromJsonAsync<ResponseHandler<List<UserDto>>>();
                users = content?.Data ?? new List<UserDto>();
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ResponseHandler<BugDto>>();
                var updateDto = new BugCreateOrUpdateDto
                {
                    Id = result.Data.Id,
                    Title = result.Data.Title,
                    TicketNumber = result.Data.TicketNumber,
                    Description = result.Data.Description,
                    ReproductionStep = result.Data.ReproductionStep,
                    Severity = result.Data.Severity,
                    Status = result.Data.Status,
                    AssignToUserId = result.Data.AssignToUserId,
                    Assignee = users.FirstOrDefault(u => u.Id == result.Data.AssignToUserId)?.UserName,
                    Attachments = result.Data.FileNames
                                .Select(fileName => new AttachmentCreateOrUpdateDto { Name = fileName })
                                .ToList()
                };
                return View(updateDto);
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BugCreateOrUpdateDto bugReportDto)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                //Uploading attachments;
                var attachments = await FileUploader.UploadFiles(bugReportDto.FileAttachments, _webHostEnvironment.WebRootPath);
                if (bugReportDto.Status == Status.InProgress)
                {
                    bugReportDto.AssignToUserId = getUserId(token); // Set severity to Low if status is InProgress
                }
                if (attachments != null && attachments.Count > 0)
                {
                    bugReportDto.Attachments = attachments;
                }
                bugReportDto.FileAttachments = null; // Clear the file attachments after uploading
                var httpMessageResponse = await _httpClient.PutAsJsonAsync($"bug/update-bug/{id}", bugReportDto);
                var resposeContent = await httpMessageResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseHandler<string>>(resposeContent);
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "Bug");
                }
                TempData["ErrorMessage"] = result?.Message ?? "Something went wrong";
                return View(bugReportDto); 
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            var token = HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var httpMessageResponse = await _httpClient.DeleteAsync($"bug/delete-bug/{id}");
            var resposeContent = await httpMessageResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseHandler<string>>(resposeContent);
            if (result != null && result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index", "Bug");
            }

            TempData["ErrorMessage"] = result?.Message ?? "Something went wrong";
            return RedirectToAction("Index", "Bug");
        }

        private string getUserId(string? token)
        {
            if (string.IsNullOrEmpty(token))
                return string.Empty;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        }

        private string GetRolesFromToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
                return string.Empty;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Depending on how roles are stored, you may need to adjust the claim type
            var roleClaims = string.Join(", ", jwtToken.Claims
                            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                            .Select(c => c.Value));

            return roleClaims;
        }
    }
}
