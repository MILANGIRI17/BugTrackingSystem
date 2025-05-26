using Microsoft.Extensions.Options;

namespace BugTracker.Web.Helper
{
    public class ApiSettingsHelper
    {
        private readonly ApiSetting _apiSetting;

        public ApiSettingsHelper(IOptions<ApiSetting> apiOptions)
        {
            _apiSetting = apiOptions.Value;
        }

        public string BaseUrl => _apiSetting.BaseUrl ?? string.Empty;
    }
}
