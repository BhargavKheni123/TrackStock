using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO.Resources
{
    public class ResourceLanguageDTO
    {
        public Int64 ID { get; set; }
        [Display(Name = "Language")]
        public string Language { get; set; }
        [Display(Name = "Culture")]
        public string Culture { get; set; }


    }
    public class ResourceLanguage
    {
        private static string ResourceFileName = "ResourceLanguage";

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        /// 
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string Culture
        {
            get
            {
                return ResourceRead.GetResourceValue("Culture", ResourceFileName);
            }
        }
        public static string Language
        {
            get
            {
                return ResourceRead.GetResourceValue("Language", ResourceFileName);
            }
        }
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }
        public static string ResourceLanguageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ResourceLanguage", ResourceFileName);
            }
        }
        public static string AddLanguage
        {
            get
            {
                return ResourceRead.GetResourceValue("AddLanguage", ResourceFileName);
            }
        }

        public static string LanguageAddedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("LanguageAddedSuccessfully", ResourceFileName);
            }
        }
        public static string SelectLanguage
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectLanguage", ResourceFileName);
            }
        }
    }

    public class ResourceTranslationDetailsDTO
    {
        public Int64 ID { get; set; }
        public string ResourceFileName { get; set; }
        public string Culture { get; set; }
        public int ResourceType { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }
        public bool isTranslated { get; set; }
        public bool CompletedinBase { get; set; }
        public bool CompletedinEnterprise { get; set; }
        public string TranslationError { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
