using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Shared.Dtos
{
    public class UserTokenDto
    {
        public string AccessToken { get; set; }

        public string Type { get; set; }

        public UserTokenDto(string accessToken, string type)
        {
            AccessToken = accessToken;
            Type = type;
        }
    }
}
