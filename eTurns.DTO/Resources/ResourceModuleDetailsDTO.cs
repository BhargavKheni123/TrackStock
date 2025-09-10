using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO.Resources
{
    public class ResourceModuleDetailsDTO
    {
        public Int64 ID { get; set; }

        [Display(Name = "FileName")]
        public string FileName { get; set; }

        [Display(Name = "DisplayFileName")]
        public string DisplayFileName { get; set; }


        public Int64 ResourceModuleID { get; set; }

        [Display(Name = "ResourceModuleName")]
        public string ResourceModuleName { get; set; }

        [Display(Name = "DisplayPageName")]
        public string DisplayPageName { get; set; }

        [Display(Name = "PageName")]
        public string PageName { get; set; }

        [Display(Name = "ResModuleDetailsKey")]
        public string ResModuleDetailsKey { get; set; }

        [Display(Name = "ResMobModuleDetailsKey")]
        public string ResMobModuleDetailsKey { get; set; }


    }
}
