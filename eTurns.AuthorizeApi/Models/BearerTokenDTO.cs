using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTurns.AuthorizeApi.Models
{
    public class BearerTokenDTO
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public TimeSpan expires_in { get; set; }


    }

    public class TokenRequestDTO
    {
        public string ClientURL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string grant_type { get; set; }
        public string client_id { get; set; }
    }
}