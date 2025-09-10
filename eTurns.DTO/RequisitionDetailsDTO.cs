using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class RequisitionDetailsDTO
    {
        public long ManufacturerID;
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }



        //RequisitionGUID
        [Display(Name = "RequisitionGUID", ResourceType = typeof(ResRequisitionDetails))]
        public Nullable<Guid> RequisitionGUID { get; set; }



        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResRequisitionDetails))]
        public Nullable<Guid> ItemGUID { get; set; }

        //ItemCost
        [Display(Name = "ItemCost", ResourceType = typeof(ResRequisitionDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ItemCost { get; set; }

        //ItemSellPrice
        [Display(Name = "ItemSellPrice", ResourceType = typeof(ResRequisitionDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double ItemSellPrice { get; set; }

        //QuantityRequisitioned
        [Display(Name = "QuantityRequisitioned", ResourceType = typeof(ResRequisitionDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> QuantityRequisitioned { get; set; }

        //QuantityPulled
        [Display(Name = "QuantityPulled", ResourceType = typeof(ResRequisitionDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> QuantityPulled { get; set; }

        //QuantityApproved
        [Display(Name = "QuantityApproved", ResourceType = typeof(ResRequisitionDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> QuantityApproved { get; set; }

        //RequiredDate
        [Display(Name = "RequiredDate", ResourceType = typeof(ResRequisitionDetails))]
        public Nullable<System.DateTime> RequiredDate { get; set; }

        private string _requiredDate;

        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string RequiredDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_requiredDate))
                {
                    _requiredDate = FnCommon.ConvertDateByTimeZone(RequiredDate, true, true);
                }
                return _requiredDate;
            }
            set { this._requiredDate = value; }
        }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResRequisitionDetails))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 Room { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CompanyID { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResRequisitionDetails))]
        public Nullable<System.Int64> BinID { get; set; }

        public System.String BinName { get; set; }

        //ProjectSpendID
        [Display(Name = "ProjectSpendID", ResourceType = typeof(ResRequisitionDetails))]
        public Nullable<System.Guid> ProjectSpendGUID { get; set; }

        public System.String ProjectSpendName { get; set; }


        public System.String PSName { get; set; }

        //Added

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }


        [Display(Name = "Description", ResourceType = typeof(ResItemMaster))]
        public string Description { get; set; }



        [Display(Name = "ItemNumber", ResourceType = typeof(ResProjectMaster))]
        public System.String ItemNumber { get; set; }
        [Display(Name = "SupplierID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> SupplierID { get; set; }
        //CategoryID
        [Display(Name = "CategoryID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> CategoryID { get; set; }
        public string CategoryName { get; set; }
        [Display(Name = "Supplier", ResourceType = typeof(ResSupplierMaster))]
        public string SupplierName { get; set; }
        [Display(Name = "LongDescription", ResourceType = typeof(ResItemMaster))]
        public System.String LongDescription { get; set; }
        public System.String RequisitionStatus { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }

        public string ItemUDF6 { get; set; }
        public string ItemUDF7 { get; set; }
        public string ItemUDF8 { get; set; }
        public string ItemUDF9 { get; set; }
        public string ItemUDF10 { get; set; }


        public string PullUDF1 { get; set; }
        public string PullUDF2 { get; set; }
        public string PullUDF3 { get; set; }
        public string PullUDF4 { get; set; }
        public string PullUDF5 { get; set; }

        public bool IsPurchase { get; set; }

        public bool IsTransfer { get; set; }

        public string UDF1 { get; set; }

        public string UDF2 { get; set; }

        public string UDF3 { get; set; }

        public string UDF4 { get; set; }

        public string UDF5 { get; set; }

        public string UDF6 { get; set; }

        public string UDF7 { get; set; }

        public string UDF8 { get; set; }

        public string UDF9 { get; set; }

        public string UDF10 { get; set; }


        public long UOMID { get; set; }

        public string Unit { get; set; }

        public double PackingQuantity { get; set; }

        public double DefaultReorderQuantity { get; set; }

        public double OnOrderQuantity { get; set; }

        public double OnTransferQuantity { get; set; }

        public double RequisitionedQuantity { get; set; }

        public double CriticalQuantity { get; set; }

        public double MaximumQuantity { get; set; }

        public double MinimumQuantity { get; set; }

        public double OnHandQuantity { get; set; }

        public string SupplierPartNo { get; set; }

        public string ManufacturerName { get; set; }

        public string ManufacturerNumber { get; set; }

        public bool IsItemLevelMinMaxQtyRequired { get; set; }

        public double Cost { get; set; }

        public double Markup { get; set; }

        public double SellPrice { get; set; }

        public double Turns { get; set; }

        public double AverageUsage { get; set; }

        public bool Taxable { get; set; }

        public string GLAccount { get; set; }

        private string _createdDate;
        private string _updatedDate;
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
        public int? CurrencyDecimalDigits { get; set; }
        public int? NumberDecimalDigits { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResCommon))]
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
        public string ReceivedOnWebDate
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

        public System.Int32 ItemType { get; set; }
        public System.Int64 ItemID { get; set; }
        public System.Decimal DefaultPullQuantity { get; set; }
        public System.Boolean LotNumberTracking { get; set; }
        public System.Boolean DateCodeTracking { get; set; }
        public Nullable<System.Boolean> IsCloseItem { get; set; }
        public Nullable<System.Guid> QuickListItemGUID { get; set; }

        public Nullable<System.Guid> ToolGUID { get; set; }
        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }
        public Nullable<System.Guid> TechnicianGUID { get; set; }

        public string ToolName { get; set; }
        public string ToolSerialNumber { get; set; }
        public string Technician { get; set; }

        [Display(Name = "SupplierAccountNumber", ResourceType = typeof(ResRequisitionDetails))]
        public Nullable<System.Guid> SupplierAccountGuid { get; set; }
        public string SupplierAccountNo { get; set; }

        public Nullable<System.Int64> ToolCategoryID { get; set; }
        public string ToolCategory { get; set; }
        public bool? IsToolCategoryInsert { get; set; }
         
        public string ImagePath { get; set; }
        public string ImageType { get; set; }
        public string ItemImageExternalURL { get; set; }

        public string PullOrderNumber { get; set; }
    }

    public class ResRequisitionDetails
    {
        private static string ResourceFileName = "ResRequisitionDetails";

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
        ///   Looks up a localized string similar to RequisitionDetails {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to RequisitionDetails.
        /// </summary>
        public static string RequisitionDetailsHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionDetailsHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequisitionDetails.
        /// </summary>
        public static string RequisitionDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionDetails", ResourceFileName);
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
        ///   Looks up a localized string similar to RequisitionID.
        /// </summary>
        public static string RequisitionID
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequisitionGUID.
        /// </summary>
        public static string RequisitionGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionGUID", ResourceFileName);
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
        ///   Looks up a localized string similar to ItemSellPrice.
        /// </summary>
        public static string ItemSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemSellPrice", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityRequisitioned.
        /// </summary>
        public static string QuantityRequisitioned
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityRequisitioned", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityPulled.
        /// </summary>
        public static string QuantityPulled
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityPulled", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityApproved.
        /// </summary>
        public static string QuantityApproved
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityApproved", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequiredDate.
        /// </summary>
        public static string RequiredDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredDate", ResourceFileName);
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
        ///   Looks up a localized string similar to LastUpdated.
        /// </summary>
        public static string LastUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdated", ResourceFileName);
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
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
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
        ///   Looks up a localized string similar to ProjectSpendID.
        /// </summary>
        public static string ProjectSpendID
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendID", ResourceFileName);
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

        public static string QtytoPull
        {
            get
            {
                return ResourceRead.GetResourceValue("QtytoPull", ResourceFileName);
            }
        }

        public static string YoucanapproveonlyRequisitionedQuantity
        {
            get
            {
                //You can approve only Requisitioned Quantity and i.e. : 
                return ResourceRead.GetResourceValue("YoucanapproveonlyRequisitionedQuantity", ResourceFileName);
            }
        }

        public static string YouhaveallreadyapprovetheQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("YouhaveallreadyapprovetheQuantity", ResourceFileName);
            }
        }

        public static string YoucanPullMaxtoApprovedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("YoucanPullMaxtoApprovedQuantity", ResourceFileName);
            }
        }

        public static string InvalidPullvalue
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidPullvalue", ResourceFileName);
            }
        }

        public static string QtytoPullandInventoryLocationareMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("Qty to Pull and Inventory Location are Mandatory.", ResourceFileName);
            }
        }

        public static string SupplierAccountNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountNumber", ResourceFileName);
            }
        }

        public static string LineItemRequiredDate
        {
            get
            {
                return ResourceRead.GetResourceValue("LineItemRequiredDate", ResourceFileName);
            }
        }
        public static string LineItemProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("LineItemProjectSpend", ResourceFileName);
            }
        }
        public static string LineItemSupplierAccount
        {
            get
            {
                return ResourceRead.GetResourceValue("LineItemSupplierAccount", ResourceFileName);
            }
        }
        public static string LineItemTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("LineItemTechnician", ResourceFileName);
            }
        }
        public static string MsgApprovedQtyMaximumValue
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgApprovedQtyMaximumValue", ResourceFileName);
            }
        }

        public static string MsgRequisitionQtyMinimumValue
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequisitionQtyMinimumValue", ResourceFileName);
            }
        }
        public static string ValidRequisitionedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidRequisitionedQuantity", ResourceFileName);
            }
        }
        public static string Invalidtechincian
        {
            get
            {
                return ResourceRead.GetResourceValue("Invalidtechincian", ResourceFileName);
            }
        }
        public static string validateDeleteRecords
        {
            get
            {
                return ResourceRead.GetResourceValue("validateDeleteRecords", ResourceFileName);
            }
        }
        public static string validatePullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("validatePullQuantity", ResourceFileName);
            }
        }
        public static string MsgFailRequisitionDetails 
        { 
            get 
            { 
                return ResourceRead.GetResourceValue("MsgFailRequisitionDetails", ResourceFileName); 
            } 
        }
        public static string MsgFailRequisitionToolDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgFailRequisitionToolDetails", ResourceFileName);
            }
        }
        public static string MsgFailRequisitionPullDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgFailRequisitionPullDetails", ResourceFileName);
            }
        }
    }
}


