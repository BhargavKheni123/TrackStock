using eTurns.DTO.Resources;
using eTurns.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    [Serializable]
    public class ChangePasswordDTO
    {
        [Display(Name = "CurrentPassword", ResourceType = typeof(ResUserMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(50, ErrorMessageResourceName = "PasswordLengthMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ResMessage))]
        public string CurrentPassword { get; set; }

        [PasswordRequired("ConfirmPassword", "ID", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessageResourceName = "PasswordLengthMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ResLoginForms))]
        [Display(Name = "Password", ResourceType = typeof(ResLoginForms))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$", ErrorMessageResourceName = "errPasswordRuleBreak", ErrorMessageResourceType = typeof(ResLoginForms))]
        public string FirstPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("FirstPassword")]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ResLoginForms))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string ConfirmPassword { get; set; }

        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }
        public Int64 UserId { get; set; }
        public Guid RequestToken { get; set; }
        public DateTime TokenGeneratedDate { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsExpired { get; set; }
        public string ErrorMessege { get; set; }
        public string Status { get; set; }


    }
    public class ResTermsAndCondition
    {

        private static string resourceFile = "ResTermsAndCondition";



        public static string LicenseAgrement
        {
            get
            {
                return ResourceRead.GetResourceValue("LicenseAgrement", resourceFile);
            }
        }
        public static string Pleasereview
        {
            get
            {
                return ResourceRead.GetResourceValue("Pleasereview", resourceFile);
            }
        }
        public static string Byusing
        {
            get
            {
                return ResourceRead.GetResourceValue("Byusing", resourceFile);
            }
        }
        public static string youagreeto
        {
            get
            {
                return ResourceRead.GetResourceValue("youagreeto", resourceFile);
            }
        }
        public static string newPDFMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("newPDFMessage", resourceFile);
            }
        }
        public static string TermsofService
        {
            get
            {
                return ResourceRead.GetResourceValue("TermsofService", resourceFile);
            }
        }



    }
}
