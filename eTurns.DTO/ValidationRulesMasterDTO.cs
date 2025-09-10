using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class ValidationRulesMasterDTO
    {
        public long ID { get; set; }

        public int ValidationModuleID { get; set; }

        [Required(ErrorMessage = "Required")]
        public string DTOName { get; set; }

        public string ModulePage { get; set; }

        [Required(ErrorMessage = "Required")]
        public string DTOProperty { get; set; }

        //public string RuleType { get; set; }

        public bool IsRequired { get; set; }

        public string ResourceFileName { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime? Created { get; set; }

        public string CreatedDisp
        {
            get
            {
                if (Created == null)
                {
                    return "";
                }

                return Created.Value.ToString("dd-MMM-yyyy hh:mm:ss tt");
            }
        }
    }
}

