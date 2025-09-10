using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class UserSettingDTO
    {
        public Int64 Id { get; set; }

        public Int64 UserId { get; set; }

        public bool IsRemember { get; set; }

        public string RedirectURL { get; set; }

        [Display(Name = "IsRequistionReportDisplay", ResourceType = typeof(Resources.ResCommon))]
        public bool? IsRequistionReportDisplay { get; set; }

        [Display(Name = "IsNeverExpirePwd", ResourceType = typeof(Resources.ResCommon))]
        public bool IsNeverExpirePwd { get; set; }

        [Display(Name = "IsNeverExpirePwd", ResourceType = typeof(Resources.ResCommon))]
        public bool ShowDateTime { get; set; }

        [Display(Name = "IsAutoLogin", ResourceType = typeof(Resources.ResCommon))]
        public bool IsAutoLogin { get; set; }

        [Display(Name = "SearchPattern", ResourceType = typeof(ResUserMaster))]
        public Int32 SearchPattern { get; set; }

        public bool ShowABConsentRemider { get; set; }
    }
}
