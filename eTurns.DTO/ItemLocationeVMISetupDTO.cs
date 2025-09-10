using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class ItemLocationeVMISetupDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResItemLocationeVMISetup))]
        public Nullable<System.Int64> BinID { get; set; }

        //eVMISensorPort
        [Display(Name = "eVMISensorPort", ResourceType = typeof(ResItemLocationeVMISetup))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String eVMISensorPort { get; set; }

        //eVMISensorID
        [Display(Name = "eVMISensorID", ResourceType = typeof(ResItemLocationeVMISetup))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> eVMISensorID { get; set; }

        //PoundsPerPiece
        [Display(Name = "PoundsPerPiece", ResourceType = typeof(ResItemLocationeVMISetup))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PoundsPerPiece { get; set; }

        //Quantity
        [Display(Name = "Quantity", ResourceType = typeof(ResItemLocationeVMISetup))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Quantity { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationeVMISetup))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        //ConsignedQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationeVMISetup))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemLocationeVMISetup))]
        public Nullable<Guid> ItemGUID { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        public System.String EditedFrom { get; set; }


        public string BinNumber { get; set; }

        public string ItemNumber { get; set; }

        public Nullable<System.Double> WeightPerPiece { get; set; }

        public bool IsChecked { get; set; }

        public bool Consignment { get; set; }

        public Nullable<System.Double> NewQuantity { get; set; }

        public Nullable<System.Double> NewWeightPerPiece { get; set; }

        /// <summary>
        /// This property is used for eVMI Desktop application
        /// </summary>
        public string PollStatusDesc { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true);
                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }

        private string _updatedDate;
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
        private string _ReceivedOn;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOn))
                {
                    _ReceivedOn = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOn;
            }
            set { this._ReceivedOn = value; }
        }

        private string _ReceivedOnWeb;
        public string ReceivedOnDateWeb
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
            set { this._ReceivedOnWeb = value; }
        }
    }

    public class ResItemLocationeVMISetup
    {
        private static string ResourceFileName = "ResItemLocationeVMISetup";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceRead.GetResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationeVMISetup {0} already exist! Try with Another!.
        /// </summary>
        public static string Duplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationeVMISetup.
        /// </summary>
        public static string ItemLocationeVMISetupHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationeVMISetupHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationeVMISetup.
        /// </summary>
        public static string ItemLocationeVMISetup
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationeVMISetup", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BinID.
        /// </summary>
        public static string BinID
        {
            get
            {
                return ResourceRead.GetResourceValue("BinID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eVMISensorPort.
        /// </summary>
        public static string eVMISensorPort
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISensorPort", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eVMISensorID.
        /// </summary>
        public static string eVMISensorID
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISensorID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PoundsPerPiece.
        /// </summary>
        public static string PoundsPerPiece
        {
            get
            {
                return ResourceRead.GetResourceValue("PoundsPerPiece", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quantity.
        /// </summary>
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerOwnedQuantity.
        /// </summary>
        public static string CustomerOwnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ConsignedQuantity.
        /// </summary>
        public static string ConsignedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated.
        /// </summary>
        public static string Updated
        {
            get
            {
                return ResourceRead.GetResourceValue("Updated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemGUID.
        /// </summary>
        public static string ItemGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemGUID", ResourceFileName);
            }
        }


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        public static string ReceivedOnWeb
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnWeb", ResourceFileName);
            }
        }

        public static string ReceivedOn
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOn", ResourceFileName);
            }
        }

        public static string EditedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("EditedFrom", ResourceFileName);
            }
        }

        public static string AddedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("AddedFrom", ResourceFileName);
            }
        }

    }
}


