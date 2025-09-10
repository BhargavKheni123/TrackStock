using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class QuickListMasterDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }

        [Display(Name = "Name", ResourceType = typeof(ResQuickList))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Name { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(ResQuickList))]
        public string Comment { get; set; }

        [Display(Name = "ListType", ResourceType = typeof(ResQuickList))]
        public int Type { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResQuickList))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResQuickList))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResQuickList))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResQuickList))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResQuickList))]
        public string UDF5 { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> LastUpdated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "NoOfItems", ResourceType = typeof(ResQuickList))]
        public int NoOfItems { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public bool IsHistory { get; set; }

        public List<QuickListDetailDTO> QuickListDetailList { get; set; }
        public string AppendedBarcodeString { get; set; }
        public int TotalRecords { get; set; }
        public System.String WhatWhereAction { get; set; }


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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(LastUpdated, true);
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
    }

    public enum QuickListType
    {
        General = 1,
        Asset = 2,
        Count = 3
    }

    public class ResQuickList
    {

        private static string resourceFile = "ResQuickList";

        /// <summary>
        ///   Looks up a localized string similar to Category.
        /// </summary>
        public static string Category
        {
            get
            {
                return ResourceRead.GetResourceValue("Category", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Comment.
        /// </summary>
        public static string Comment
        {
            get
            {
                return ResourceRead.GetResourceValue("Comment", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to ItemNumber.
        /// </summary>
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", resourceFile);
            }
        }
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to List Type.
        /// </summary>
        public static string ListType
        {
            get
            {
                return ResourceRead.GetResourceValue("ListType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Add Item to Quicklist.
        /// </summary>
        public static string ModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ModelHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quick List Name.
        /// </summary>
        public static string Name
        {
            get
            {
                return ResourceRead.GetResourceValue("Name", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to # of Items.
        /// </summary>
        public static string NoOfItems
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfItems", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuickList.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: QuickList.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quantity.
        /// </summary>
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quantity.
        /// </summary>
        public static string ConsignedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantity", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Supplier.
        /// </summary>
        public static string Supplier
        {
            get
            {
                return ResourceRead.GetResourceValue("Supplier", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile);
            }
        }
        public static string QuantityOrConsignedQtygreaterthanZero
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityOrConsignedQtygreaterthanZero", resourceFile);
            }
        }
        public static string NoItemFoundInQuickList
        {
            get
            {
                return ResourceRead.GetResourceValue("NoItemFoundInQuickList", resourceFile);
            }
        }
        public static string QuickListHistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickListHistoryID", resourceFile);
            }
        }
        public static string QuickListName
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickListName", resourceFile);
            }
        }
        public static string QuickListCreditQty
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickListCreditQty", resourceFile);
            }
        }
    }

    public class ResQuickListItems
    {

        private static string resourceFile = "ResQuickListItems";

        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile);
            }
        }

        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile);
            }
        }

        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile);
            }
        }

        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile);
            }
        }

        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile);
            }
        }
        public static string SerialLotDCNotAllowedForCountType
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialLotDCNotAllowedForCountType", resourceFile);
            }
        }
        public static string msgProperQLQty
        {
            get
            {
                return ResourceRead.GetResourceValue("msgProperQLQty", resourceFile);
            }
        }
    }
}
