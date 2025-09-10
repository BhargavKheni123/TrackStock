using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class BillingRoomTypeModulesMapDTO 
    {
        public long? MapID { get; set; }
        public int? BillingRoomTypeID { get; set; }
        public long ModuleID { get; set; }
        public bool IsModuleEnabled { get; set; }
        public string ModuleName { get; set; }

        public string resourcekey { get; set; }
        public string ModuleNameFromResource
        {
            get
            {
                if (!string.IsNullOrEmpty(resourcekey))
                    return ResourceRead.GetResourceValue(resourcekey, "ResModuleName");
                else
                    return ModuleName;
            }
        }

    }
}
