using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class EmailUserConfigurationDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Name", ResourceType = typeof(ResEmailUserConfiguration))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"[a-zA-Z0-9.!#$%&'*+-/=?\^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+", ErrorMessage = "Please enter correct email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(ResCommon))]
        public string Email { get; set; }

        public bool IsChecked { get; set; }
        public string TemplateName { get; set; }
        public Int64? RoomId { get; set; }
        public Int64? CompanyId { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
    }

    public class EmailUserMasterDetailDTO
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
        public Int64 UserID { get; set; }

    }
    public class ResEmailUserConfiguration
    {
        private static string ResourceFileName = "ResEmailUserConfiguration";

        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }
        public static string Name
        {
            get
            {
                return ResourceRead.GetResourceValue("Name", ResourceFileName);
            }
        }
        public static string Email
        {
            get
            {
                return ResourceRead.GetResourceValue("Email", ResourceFileName);
            }
        }
        public static string EmailTemplate
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailTemplate", ResourceFileName);
            }
        }
        public static string ReqUser
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqUser", ResourceFileName);
            }
        }
    }
    public class EnterpriseUserDetailDTO
    {
        public string UserName { get; set; }
        public string Company { get; set; }
        public string Stockroom { get; set; }
        public string Phone { get; set; }
        public string email { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
