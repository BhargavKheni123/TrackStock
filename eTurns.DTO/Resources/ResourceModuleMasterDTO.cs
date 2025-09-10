using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO.Resources
{
    public class ResourceModuleMasterDTO
    {
        public Int64 ID { get; set; }

        [Display(Name = "ModuleName")]
        public string ModuleName { get; set; }

        [Display(Name = "ResModuleKey")]
        public string ResModuleKey { get; set; }


    }
}
