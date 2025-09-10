using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ProductToItemQtyDTO
    {
        public ProductToItemCallFor CallFor { get; set;}
        public string ASIN { get; set;}
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [CriticleQuantityCheck("MinimumQty", ErrorMessage = "Critical quantity must be less than Minimum quantity")]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public double ?  CriticalQty{ get; set;}

        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [MinimumQuantityCheck("MaximumQty", ErrorMessage = "Minimum quantity must be less than Maximum quantity")]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public double ?  MinimumQty{ get; set;}

        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public double ?  MaximumQty{ get; set;}

        [Display(Name = "CartQuantity", ResourceType = typeof(ResABProducts))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public double ?  CartQty { get; set;}

        public bool IsExistingItem { get; set; }
    }

    public enum ProductToItemCallFor
    {
        AddToItem = 1,
        AddToCart = 2
    }
}
