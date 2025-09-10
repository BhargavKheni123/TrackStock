using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class ValidationRuleModulesDTO
    {
        public int ID { get; set; }
        public string ModulePage { get; set; }
        public string BreadCrumb { get; set; }
        public string DTOName { get; set; }

        public string Module { get; set; }

        public double DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
