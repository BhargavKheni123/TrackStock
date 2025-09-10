using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurns.DTO;

namespace eTurnsWeb.Models
{
    public class ValidationRulesMasterModel
    {
        public ValidationRulesMasterDTO ValidationRulesMaster { get; set; }
        public SelectList DTOList { get; set; }

        public SelectList ResourceFiles { get; set; }


        //public List<SelectListItem> RuleTypeList { get; set; }
        
    }
}