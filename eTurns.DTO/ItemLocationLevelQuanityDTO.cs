using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using eTurns.DTO.Resources;

namespace eTurns.DTO
{
    public class ItemLocationLevelQuanityDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResItemLocationLevelQuanity))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 BinID { get; set; }

        //CriticalQuantity
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResItemLocationLevelQuanity))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CriticalQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemLocationLevelQuanity))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MinimumQuantity { get; set; }

        //MaximumQuantity
        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemLocationLevelQuanity))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MaximumQuantity { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemLocationLevelQuanity))]
        public Nullable<Guid> ItemGUID { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResItemLocationLevelQuanity))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //guid
        public Guid GUID { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
        public string BinNumber { get; set; }

        public int SessionSr { get; set; }

        //MinimumQuantity
        [Display(Name = "SuggestedOrderQuantity", ResourceType = typeof(ResItemLocationLevelQuanity))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }

        //IsDefault
        [Display(Name = "IsDefault", ResourceType = typeof(ResItemSupplierDetails))]
        public Nullable<Boolean> IsDefault { get; set; }

        //MinimumQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationLevelQuanity))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationLevelQuanity))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        [Display(Name = "eVMISensorPort", ResourceType = typeof(ResCommon))]
        public string eVMISensorPort { get; set; }

        [Display(Name = "eVMISensorID", ResourceType = typeof(ResItemLocationLevelQuanity))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> eVMISensorID { get; set; }


    }

    public class ResItemLocationLevelQuanity
    {
        private static string ResourceFileName = "ResItemLocationLevelQuanity";

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string ConsignedQuantity
        {
            get
            {
                return ResourceHelper.GetResourceValue("ConsignedQuantity", ResourceFileName);
            }
        }
        public static string CustomerOwnedQuantity
        {
            get
            {
                return ResourceHelper.GetResourceValue("CustomerOwnedQuantity", ResourceFileName);
            }
        }
        


        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string eVMISensorPort
        {
            get
            {
                return ResourceHelper.GetResourceValue("eVMISensorPort", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string eVMISensorID
        {
            get
            {
                return ResourceHelper.GetResourceValue("eVMISensorID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceHelper.GetResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationLevelQuanity {0} already exist! Try with Another!.
        /// </summary>
        public static string Duplicate
        {
            get
            {
                return ResourceHelper.GetResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceHelper.GetResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceHelper.GetResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceHelper.GetResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceHelper.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationLevelQuanity.
        /// </summary>
        public static string ItemLocationLevelQuanityHeader
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemLocationLevelQuanityHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationLevelQuanity.
        /// </summary>
        public static string ItemLocationLevelQuanity
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemLocationLevelQuanity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceHelper.GetResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceHelper.GetResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BinID.
        /// </summary>
        public static string BinID
        {
            get
            {
                return ResourceHelper.GetResourceValue("BinID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CriticalQuantity.
        /// </summary>
        public static string CriticalQuantity
        {
            get
            {
                return ResourceHelper.GetResourceValue("CriticalQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string MinimumQuantity
        {
            get
            {
                return ResourceHelper.GetResourceValue("MinimumQuantity", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to SuggestedOrderQuantity.
        /// </summary>
        public static string SuggestedOrderQuantity
        {
            get
            {
                return ResourceHelper.GetResourceValue("SuggestedOrderQuantity", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to MaximumQuantity.
        /// </summary>
        public static string MaximumQuantity
        {
            get
            {
                return ResourceHelper.GetResourceValue("MaximumQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemGUID.
        /// </summary>
        public static string ItemGUID
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceHelper.GetResourceValue("CompanyID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceHelper.GetResourceValue("Room", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceHelper.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceHelper.GetResourceValue("Created", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdated.
        /// </summary>
        public static string LastUpdated
        {
            get
            {
                return ResourceHelper.GetResourceValue("LastUpdated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceHelper.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceHelper.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceHelper.GetResourceValue("IsArchived", ResourceFileName);
            }
        }


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceHelper.GetResourceValue("PageTitle", ResourceFileName);
            }
        }
    }
}


