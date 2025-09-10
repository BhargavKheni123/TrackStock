using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class ABProductByASINDTO: ICloneable
    {
        public int matchingProductCount { get; set; }
        public List<Product> products { get; set; }
        public List<object> notFoundAsins { get; set; }
        public int pageSize { get; set; }
        public string RegionCurrencySymbol { get; set; }
        public int numberOfPages { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }    
    public class ABProductByASINPostReqInfo
    {
        public List<string> productIds { get; set; }
        public List<string> facets { get; set; }
        public string productRegion { get; set; }
        public string locale { get; set; }
    }


}
