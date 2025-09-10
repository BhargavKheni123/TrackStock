using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class BarcodeMasterDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }

        [Display(Name = "ModuleGUID", ResourceType = typeof(ResBarcodeMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid ModuleGUID { get; set; }
        public Guid RefGUID { get; set; }

        [Display(Name = "BarcodeString", ResourceType = typeof(ResBarcodeMaster))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
       // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string BarcodeString { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime UpdatedOn { get; set; }

        public Int64 CreatedBy { get; set; }
        public Int64 UpdatedBy { get; set; }
        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }


        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "ModuleName", ResourceType = typeof(ResBarcodeMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string ModuleName { get; set; }

        [Display(Name = "RefNumber")]
        public string RefNumber { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        [Display(Name = "BarcodeAdded", ResourceType = typeof(eTurns.DTO.ResBarcodeMaster))]
        public string BarcodeAdded { get; set; }

        [Display(Name = "BinGuid", ResourceType = typeof(eTurns.DTO.ResBarcodeMaster))]
        public Guid? BinGuid { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ItemMasterList", ResourceType = typeof(ResLayout))]
        public string items { get; set; }

        [Display(Name = "BinNumber", ResourceType = typeof(eTurns.DTO.ResBarcodeMaster))]
        public string BinNumber { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        private string _CreatedOn;
        public string CreatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_CreatedOn))
                {
                    _CreatedOn = FnCommon.ConvertDateByTimeZone(CreatedOn, true);
                }
                return _CreatedOn;
            }
            set { this._CreatedOn = value; }
        }

        private string _UpdatedOn;
        public string UpdatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_UpdatedOn))
                {
                    _UpdatedOn = FnCommon.ConvertDateByTimeZone(UpdatedOn, true);
                }
                return _UpdatedOn;
            }
            set { this._ReceivedOnWeb = value; }
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
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }

        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }
        public string Category { get; set; }

        public string Description { get; set; }
        public string SupplierName { get; set; }
        public string SupplierNumber { get; set; }
        public string manufacturername { get; set; }
        public string ManufacturerNumber { get; set; }
        public string OldBarcodeString { get; set; }
    }
    public class ResBarcodeMaster
    {
        private static string ResourceFileName = "ResBarcodeMaster";


        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

        

        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", ResourceFileName);
            }
        }
        public static string ModuleName
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleName", ResourceFileName);
            }
        }

        public static string ModuleGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleName", ResourceFileName);
            }
        }

        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }

        public static string BinGuid
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }
        

        public static string BarcodeString
        {
            get
            {
                return ResourceRead.GetResourceValue("BarcodeString", ResourceFileName);
            }
        }
        public static string BarcodeAdded
        {
            get
            {
                return ResourceRead.GetResourceValue("BarcodeAdded", ResourceFileName);
            }
        }
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }



        public static string ErrorGenerateBarcode
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorGenerateBarcode", ResourceFileName);
            }
        }
        public static string BarcodeStringExists
        {
            get
            {
                return ResourceRead.GetResourceValue("BarcodeStringExists", ResourceFileName);
            }
        }

        public static string BarcodeSaveRequiredMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("BarcodeSaveRequiredMsg", ResourceFileName);
            }
        }
        public static string BarcodeGeneratedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("BarcodeGeneratedSuccessfully", ResourceFileName);
            }
        }
        public static string MsgProperModuleNotSelected
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProperModuleNotSelected", ResourceFileName);
            }
        }

        public static string MsgNotProperData
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNotProperData", ResourceFileName);
            }
        }
        public static string MsgBinNumberNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgBinNumberNotAvailable", ResourceFileName);
            }
        }

        public static string SelectModule { get { return ResourceRead.GetResourceValue("SelectModule", ResourceFileName); } }
        public static string SelectItemList { get { return ResourceRead.GetResourceValue("SelectItemList", ResourceFileName); } }
        public static string SelectBinNumber { get { return ResourceRead.GetResourceValue("SelectBinNumber", ResourceFileName); } }
        public static string SelectAssetList { get { return ResourceRead.GetResourceValue("SelectAssetList", ResourceFileName); } }
        public static string SelectToolList { get { return ResourceRead.GetResourceValue("SelectToolList", ResourceFileName); } }
        public static string NoOfItemsPerPage { get { return ResourceRead.GetResourceValue("NoOfItemsPerPage", ResourceFileName); } }
        public static string BarcodeFont { get { return ResourceRead.GetResourceValue("BarcodeFont", ResourceFileName); } }
        public static string BarcodePattern { get { return ResourceRead.GetResourceValue("BarcodePattern", ResourceFileName); } }
        public static string BarcodeKey { get { return ResourceRead.GetResourceValue("BarcodeKey", ResourceFileName); } }
        public static string IncludeBin { get { return ResourceRead.GetResourceValue("IncludeBin", ResourceFileName); } }
        public static string IncludeQuantity { get { return ResourceRead.GetResourceValue("IncludeQuantity", ResourceFileName); } }
        public static string QuantityField { get { return ResourceRead.GetResourceValue("QuantityField", ResourceFileName); } }

    }
}
