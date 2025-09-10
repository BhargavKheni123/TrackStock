using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class ToolAssetPullDetailsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }


        //ItemGUID
        [Display(Name = "ToolGUID", ResourceType = typeof(ResToolAssetPullDetails))]
        public Nullable<Guid> ToolGUID { get; set; }


        //ItemCost
        [Display(Name = "ToolCost", ResourceType = typeof(ResToolAssetPullDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ToolCost { get; set; }


        //CustomerOwnedQuantity
        [Display(Name = "Quantity", ResourceType = typeof(ResToolAssetPullDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Quantity { get; set; }



        //PoolQuantity
        [Display(Name = "PullQuantity", ResourceType = typeof(ResToolAssetPullDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PullQuantity { get; set; }



        //Received
        [Display(Name = "Received", ResourceType = typeof(ResToolAssetPullDetails))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Received { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResToolAssetPullDetails))]
        public Nullable<System.Int64> BinID { get; set; }

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
        public Nullable<System.Int64> RoomID { get; set; }

        //PullCredit
        [Display(Name = "PullCredit", ResourceType = typeof(ResToolAssetPullDetails))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String PullCredit { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public Nullable<Guid> ItemLocationDetailGUID { get; set; }

        public Guid GUID { get; set; }
        public Nullable<Guid> PULLGUID { get; set; }

        public string BinName { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public Int32 Type { get; set; }

        public double? ToolOnhandQty { get; set; }
        public bool IsAddedFromPDA { get; set; }
        public bool IsProcessedAfterSync { get; set; }
        //public string CreatedDate
        //{
        //    get
        //    {
        //        return FnCommon.ConvertDateByTimeZone(Created, true);
        //    }
        //}

        //public string UpdatedDate
        //{
        //    get
        //    {
        //        return FnCommon.ConvertDateByTimeZone(Updated, true);
        //    }
        //}

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

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

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

        public Nullable<System.Double> CreditQuantity { get; set; }


        public string CreditBinName { get; set; }
        public string CreditProjectName { get; set; }
        public string CreditBinID { get; set; }
        public string CreditUDF1 { get; set; }
        public string CreditUDF2 { get; set; }
        public string CreditUDF3 { get; set; }
        public string CreditUDF4 { get; set; }
        public string CreditUDF5 { get; set; }


        public Nullable<Guid> NEW_PullGUID { get; set; }
    }

    public class ResToolAssetPullDetails
    {
        private static string ResourceFileName = "ResToolAssetPullDetails";

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
        ///   Looks up a localized string similar to PullDetails {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to PullDetails.
        /// </summary>
        public static string PullDetailsHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PullDetailsHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PullDetails.
        /// </summary>
        public static string PullDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("PullDetails", ResourceFileName);
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
        ///   Looks up a localized string similar to PULLID.
        /// </summary>
        public static string PULLID
        {
            get
            {
                return ResourceRead.GetResourceValue("PULLID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", ResourceFileName);
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

        /// <summary>
        ///   Looks up a localized string similar to ProjectID.
        /// </summary>
        public static string ProjectID
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemCost.
        /// </summary>
        public static string ItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCost", ResourceFileName);
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
        ///   Looks up a localized string similar to PoolQuantity.
        /// </summary>
        public static string PoolQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PoolQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SerialNumber.
        /// </summary>
        public static string SerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LotNumber.
        /// </summary>
        public static string LotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Expiration.
        /// </summary>
        public static string Expiration
        {
            get
            {
                return ResourceRead.GetResourceValue("Expiration", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Received.
        /// </summary>
        public static string Received
        {
            get
            {
                return ResourceRead.GetResourceValue("Received", ResourceFileName);
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
        ///   Looks up a localized string similar to PullCredit.
        /// </summary>
        public static string PullCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("PullCredit", ResourceFileName);
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
    }

    #region New Changes related to Credit History Table

    public class ToolAssetPullCreditHistoryDTO
    {
        public System.Int64 ID { get; set; }
        public System.Guid GUID { get; set; }
        public System.Guid PullDetailGuid { get; set; }
        public System.Guid PULLGUID { get; set; }
        public System.Guid ToolGUID { get; set; }
        public Nullable<System.Double> CreditQuantity { get; set; }
        public System.Int64 CompanyID { get; set; }
        public System.Int64 RoomID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public System.String WhatWhereAction { get; set; }
    }

    #endregion New Changes related to Credit History Table
}


