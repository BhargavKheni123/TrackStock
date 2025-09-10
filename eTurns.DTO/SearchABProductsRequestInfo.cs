using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class SearchABProductsRequestInfo
    {
        public string ApiQueryURL { get; set; }
        public string IAMAccessKey { get; set; }
        public string IAMSecretKey { get; set; }
        public string AWSRegion { get; set; }
        public string AccessToken { get; set; }
        public string EmailAddress { get; set; }
        public string AWSServiceName { get { return "execute-api"; } }
        public string AWSHMACSignPrefix { get { return "AWS4-HMAC-SHA256"; } }
        public string AWSSigningVersion { get { return "AWS4"; } }
        public string EmptyBodySignature { get { return "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"; } }
        public string AWSRequestType { get { return "aws4_request"; } }
        public string RequstBodyJSON { get; set; }
    }
}
