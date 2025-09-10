using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class JQueryDataTableNewParamModel
    {
        public string draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }


    }
    public class AuthenticateRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class InvalidateCacheRequest
    {
        public string CacheKey { get; set; }
    }
    public class WebApiResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
