using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class ItemMasterBinDTO
    {


        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public virtual System.Int64 ID { get; set; }

        public Int32? TotalRecords { get; set; }

        //ItemNumber
        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]

        [AllowHtml]
        public System.String ItemNumber { get; set; }

        public Guid? ItemGuid { get; set; }

        [Display(Name = "ConsignedQty", ResourceType = typeof(ResItemBinMaster))]
        public virtual System.Double ConsignedQty { get; set; }
        [Display(Name = "CustomerOwnedQty", ResourceType = typeof(ResItemBinMaster))]
        public virtual System.Double CustomerOwnedQty { get; set; }

        [Display(Name = "OldLocation", ResourceType = typeof(ResItemBinMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long? OldLocation { get; set; }

        //DefaultLocation
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "OldLocationName", ResourceType = typeof(ResItemBinMaster))]
        public string OldLocationName { get; set; }

        public Guid? OldLocationGUID { get; set; }


        [Display(Name = "NewLocation", ResourceType = typeof(ResItemBinMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long? NewLocation { get; set; }

        //DefaultLocation
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "NewLocationName", ResourceType = typeof(ResItemBinMaster))]
        public string NewLocationName { get; set; }

        public Guid? NewLocationGUID { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResItemMaster))]
        public virtual Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public virtual Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String EditedFrom { get; set; }


        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        private string _createdDate;
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
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
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
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
        [Display(Name = "IsDefault", ResourceType = typeof(ResItemBinMaster))]
        public bool? IsDefault { get; set; }
        public string Message { get; set; }

    }
    public class ResItemBinMaster
    {
        private static string ResourceFileName = "ResItemBinMaster";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string ConsignedQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQty", ResourceFileName);
            }
        }
        public static string OldLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("OldLocation", ResourceFileName);
            }
        }
        public static string CustomerOwnedQty
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQty", ResourceFileName);
            }
        }
        public static string OldLocationName
        {
            get
            {
                return ResourceRead.GetResourceValue("OldLocationName", ResourceFileName);
            }
        }
        public static string NewLocationName
        {
            get
            {
                return ResourceRead.GetResourceValue("NewLocationName", ResourceFileName);
            }
        }

        public static string NewLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("NewLocation", ResourceFileName);
            }
        }
        public static string IsDefault
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDefault", ResourceFileName);
            }
        }

        public static string ItemLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocation", ResourceFileName);
            }
        }
        public static string ScaleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScaleID", ResourceFileName);
            }
        }
        public static string IssueWithItem
        {
            get
            {
                return ResourceRead.GetResourceValue("IssueWithItem", ResourceFileName);
            }
        }
        public static string BinSavedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("BinSavedSuccessfully", ResourceFileName);
            }
        }
        public static string InvalidItemGuid
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidItemGuid", ResourceFileName);
            }
        }
    }

}