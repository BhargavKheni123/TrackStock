using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ABMarketPlaceDTO
    {
        public long ID { get; set; }
        public string ABMPName { get; set; }
        public string RegionName { get; set; }
        public string RegionCode { get; set; }
        public string CountryCode { get; set; }
        public string OAuthAuthorizationURI { get; set; }
        public string OAuthAuthAPIURI { get; set; }
        public string MarketplaceID { get; set; }
        public string ABAPIEndPoint { get; set; }
        public string AWSRegion { get; set; }
        public string RegionCurrencyCode { get; set; }
        public string RegionCurrencySymbol { get; set; }
        public string ABLocale { get; set; }
    }
}
