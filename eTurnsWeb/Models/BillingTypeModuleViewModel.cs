using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eTurnsWeb.Models
{
     public class BillingTypeModuleViewModel
    {
        [Display(Name = "EnterpriseName", ResourceType = typeof(ResEnterprise))]
        public long Enterprise { get; set; }

        [Display(Name = "BillingRoomType", ResourceType = typeof(ResRoomMaster))]        
        public int? BillingRoomType { get; set; }

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

        public List<BillingRoomTypeModulesMapDTO> ModuleMapping { get; set; }
    }
}