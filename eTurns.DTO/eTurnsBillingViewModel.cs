using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class eTurnsBillingViewModel
    {
        [Display(Name = "EnterpriseName", ResourceType = typeof(ResEnterprise))]
        public long EnterpriseID { get; set; }

        [Display(Name = "BillingRoomType", ResourceType = typeof(ResRoomMaster))]
        public int? BillingRoomTypeId { get; set; }

        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "BillingRoomTypeName", ResourceType = typeof(ResRoomMaster))]
        public string BillingRoomTypeName { get; set; }
        
        public SelectList BillingRoomTypeList
        {
            get; set;
        }

        public SelectList EnterpriseList
        {
            get; set;
        }

        public double BaseCost { get; set; }

        public double OneTimeLicenceFee { get; set; }

        public byte Grouping { get; set; }

        public List<BillingRoomTypeModuleMasterDTO> ModuleMapping { get; set; }
    }
}
