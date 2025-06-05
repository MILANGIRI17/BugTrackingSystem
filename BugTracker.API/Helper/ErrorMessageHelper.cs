using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugTracker.API.Helper
{
    public static class ErrorMessageHelper
    {
        public static string GetErrorMessage(object thisObject)
        {
            string returnString = string.Empty;
            List<string> errors = new List<string>();
            if (thisObject is ModelStateDictionary)
            {
                var modelState = (ModelStateDictionary)thisObject;
                errors = modelState.Where(x => x.Value.Errors.Count > 0)
                             .SelectMany(x => x.Value?.Errors.Select(err => err.ErrorMessage))
                             .ToList();
            }
            else if (thisObject is IdentityResult authResult)
            {
                errors = authResult.Errors.Select(x => x.Description).ToList();
            }

            foreach (var error in errors)
            {
                if (string.IsNullOrEmpty(returnString))
                    returnString += error;
                else
                    returnString += $"<br> {error}";
            }

            return returnString;
        }
    }
}
