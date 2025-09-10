using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class RequisitionMasterDTO
    {
        //ID
        public System.Int64 ID { get; set; }

        //RequisitionNumber
        [Display(Name = "RequisitionNumber", ResourceType = typeof(ResRequisitionMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String RequisitionNumber { get; set; }

        //Description
        [Display(Name = "Description", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Description { get; set; }

        //WorkorderName
        [Display(Name = "WorkorderName", ResourceType = typeof(ResRequisitionMaster))]
        public Nullable<System.Guid> WorkorderGUID { get; set; }

        //WorkorderName
        [Display(Name = "WorkorderName", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String WorkorderName { get; set; }

        //TechnicianName
        [Display(Name = "TechnicianID", ResourceType = typeof(ResRequisitionMaster))]
        //public Nullable<System.Guid> TechnicianGUID { get; set; }
        public Nullable<System.Int64> TechnicianID { get; set; }


        [Display(Name = "Technician", ResourceType = typeof(ResTechnician))]
        public System.String Technician { get; set; }

        //RequiredDate
        [Display(Name = "RequiredDate", ResourceType = typeof(ResRequisitionMaster))]

        public Nullable<System.DateTime> RequiredDate { get; set; }



        [Display(Name = "RequiredDateStr", ResourceType = typeof(ResOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String RequiredDateStr { get; set; }

        //NumberofItemsrequisitioned
        [Display(Name = "NumberofItemsrequisitioned", ResourceType = typeof(ResRequisitionMaster))]
        public Nullable<System.Int32> NumberofItemsrequisitioned { get; set; }

        //TotalCost
        [Display(Name = "TotalCost", ResourceType = typeof(ResRequisitionMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> TotalCost { get; set; }

        //TotalSellPrice
        [Display(Name = "TotalSellPrice", ResourceType = typeof(ResRequisitionMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double TotalSellPrice { get; set; }

        //CustomerID
        [Display(Name = "CustomerID", ResourceType = typeof(ResRequisitionMaster))]
        public Nullable<System.Int64> CustomerID { get; set; }

        //ProjectSpendID
        [Display(Name = "ProjectSpendGUID", ResourceType = typeof(ResRequisitionMaster))]
        public Nullable<System.Guid> ProjectSpendGUID { get; set; }

        //ProjectSpendName
        [Display(Name = "ProjectSpendName", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ProjectSpendName { get; set; }

        //RequisitionStatus
        [Display(Name = "RequisitionStatus", ResourceType = typeof(ResRequisitionMaster))]
        public System.String RequisitionStatus { get; set; }


        //RequisitionType
        [Display(Name = "RequisitionType", ResourceType = typeof(ResRequisitionMaster))]
        public System.String RequisitionType { get; set; }

        //BillingAccount
        [Display(Name = "BillingAccountID", ResourceType = typeof(ResRequisitionMaster))]
        public Nullable<System.Int64> BillingAccountID { get; set; }


        //BillingAccount
        [Display(Name = "BillingAccount", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String BillingAccount { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResRequisitionMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //Created
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

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }


        public bool IsHistory { get; set; }
        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }


        [Display(Name = "CustomerGUID", ResourceType = typeof(ResRequisitionMaster))]
        public Nullable<System.Guid> CustomerGUID { get; set; }

        public bool IsRecordEditable { get; set; }
        public bool IsOnlyStatusUpdate { get; set; }

        public double RequisitionedQuantity { get; set; }
        public double ApprovedQuantity { get; set; }
        public double PulledQuantity { get; set; }

        [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
        public string Customer { get; set; }

        public List<RequisitionDetailsDTO> RequisitionListItem { get; set; }
        public string AppendedBarcodeString { get; set; }

        public System.String WhatWhereAction { get; set; }
        public string ShowRequisitionPullNotification { get; set; }

        public System.String RequisitionNumberForSorting { get { return GetSortingString(RequisitionNumber); } }

        public int? InCompletePullCount { get; set; }
        public int rdoApprovelChoice { get; set; }
        public int PageSubmissionMethod { get; set; }
        public AutoOrderNumberGenerate AutoOrderNumber { get; set; }

        public int TotalRecords { get; set; }

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

        public string RequisitionApprover { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(ResOrder))]
        public Nullable<System.Int64> SupplierId { get; set; }
        [Display(Name = "SupplierAccountGuid", ResourceType = typeof(ResOrder))]
        public Nullable<Guid> SupplierAccountGuid { get; set; }
        public string SupplierName { get; set; }

        public string SupplierAccountNumberName { get; set; }

        public int? AttachingWOWithRequisition { get; set; }

        [Display(Name = "ReleaseNumber", ResourceType = typeof(ResRequisitionMaster))]
        public string ReleaseNumber { get; set; }
        [Display(Name = "StagingID", ResourceType = typeof(ResMaterialStaging))]
        [AllowHtml]
        public System.Int64? StagingID { get; set; }

        [Display(Name = "StagingName", ResourceType = typeof(ResMaterialStaging))]
        public string StagingName { get; set; }

        public Nullable<Guid> MaterialStagingGUID { get; set; }

        public bool? IsStagingEditable { get; set; }
        public string CompanyName { get; set; }

        string GetSortingString(string StringToSort)
        {
            Dictionary<string, Regex> dic = GetRegexDictionary();
            string inputString = StringToSort;
            string returnString = StringToSort;
            try
            {
                foreach (var item in dic)
                {
                    if (item.Value.IsMatch(inputString))
                    {
                        Match mat = item.Value.Match(inputString);
                        if (!string.IsNullOrEmpty(mat.Value))
                        {
                            string strkey = item.Key;

                            if (item.Key.StartsWith("NNNNN"))
                            {
                                returnString = mat.Value.PadLeft(13, '0');
                            }
                            else if (item.Key.EndsWith("-NNNNN"))
                            {
                                int idx = mat.Value.LastIndexOf("-");
                                string strDateVal = mat.Value.Substring(0, idx);
                                string strnumber = mat.Value.Remove(0, idx + 1);

                                DateTime dtval = DateTime.ParseExact(strDateVal, item.Key.Substring(0, item.Key.LastIndexOf("-")), new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + strnumber.PadLeft(5, '0');
                            }
                            else if (item.Key.EndsWith("#NNNNN"))
                            {
                                int idx = mat.Value.LastIndexOf("#");
                                string strDateVal = mat.Value.Substring(0, idx);
                                string strnumber = mat.Value.Remove(0, idx);

                                DateTime dtval = DateTime.ParseExact(strDateVal, item.Key.Substring(0, item.Key.LastIndexOf("#")), new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + strnumber.PadLeft(5, '0');
                            }
                            else if (item.Key.Contains("yyyy"))
                            {
                                DateTime dtval = DateTime.ParseExact(mat.Value, item.Key, new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + "00000";
                            }
                            else if (item.Key.Contains("yy"))
                            {
                                DateTime dtval = DateTime.ParseExact(mat.Value, item.Key, new CultureInfo("en-US"));
                                returnString = dtval.ToString("yyyyMMdd") + "00000";
                            }
                            break;
                        }
                    }
                }
                return returnString;
            }
            catch (Exception)
            {
                return returnString;
            }
        }
        Dictionary<string, Regex> GetRegexDictionary()
        {

            Dictionary<string, Regex> dic = new Dictionary<string, Regex>();

            dic.Add("NNNNN", new Regex(@"^[0-9]{1,9}$"));

            dic.Add("M-d-yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("M/d/yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("M.d.yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.](1[9][0-9][0-9]|2[0][0-9][0-9])$"));

            dic.Add("d-M-yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("d/M/yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/](1[9][0-9][0-9]|2[0][0-9][0-9])$"));
            dic.Add("d.M.yyyy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.](1[9][0-9][0-9]|2[0][0-9][0-9])$"));

            dic.Add("yyyy-M-d", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yyyy/M/d", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yyyy.M.d", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));

            dic.Add("yyyy-d-M", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yyyy/d/M", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yyyy.d.M", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])$"));

            dic.Add("M-d-yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("M/d/yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("M.d.yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));

            dic.Add("d-M-yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("d/M/yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));
            dic.Add("d.M.yyyy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.](1[9][0-9][0-9]|2[0][0-9][0-9])[-]([0-9]{1,5})$"));

            dic.Add("yyyy-M-d-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yyyy/M/d-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yyyy.M.d-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));

            dic.Add("yyyy-d-M-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yyyy/d/M-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yyyy.d.M-NNNNN", new Regex(@"^(1[9][0-9][0-9]|2[0][0-9][0-9])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));


            dic.Add("M-d-yy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{2})$"));
            dic.Add("M/d/yy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([0-9]{2})$"));
            dic.Add("M.d.yy", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([0-9]{2})$"));

            dic.Add("d-M-yy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{2})$"));
            dic.Add("d/M/yy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/]([0-9]{2})$"));
            dic.Add("d.M.yy", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.]([0-9]{2})$"));

            dic.Add("yy-M-d", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yy/M/d", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));
            dic.Add("yy.M.d", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$"));

            dic.Add("yy-d-M", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yy/d/M", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])$"));
            dic.Add("yy.d.M", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])$"));

            dic.Add("M-d-yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("M/d/yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("M.d.yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([0-9]{2})[-]([0-9]{1,5})$"));

            dic.Add("d-M-yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("d/M/yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[/]([0-9]{2})[-]([0-9]{1,5})$"));
            dic.Add("d.M.yy-NNNNN", new Regex(@"^([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[.]([0-9]{2})[-]([0-9]{1,5})$"));

            dic.Add("yy-M-d-NNNNN", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-2])[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yy/M/d-NNNNN", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-2])[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));
            dic.Add("yy.M.d-NNNNN", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-2])[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([0-9]{1,5})$"));

            dic.Add("yy-d-M-NNNNN", new Regex(@"^([0-9]{2})[-]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[-]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yy/d/M-NNNNN", new Regex(@"^([0-9]{2})[/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[/]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));
            dic.Add("yy.d.M-NNNNN", new Regex(@"^([0-9]{2})[.]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[.]([1-9]|0[1-9]|1[0-2])[-]([0-9]{1,5})$"));


            return dic;
        }

        public Nullable<System.Int64> RequesterID { get; set; }
        public Nullable<System.Int64> ApproverID { get; set; }
        public string RequisitionDataLog { get; set; }
    }
    public class RequisitionMasterNarrowSearchDTO
    {
        //ID
        public System.Int32? TotalCount { get; set; }

        public System.String NarrowSearchText { get; set; }
    }
    public class ResRequisitionMaster
    {
        private static string ResourceFileName = "ResRequisitionMaster";

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
        public static string msgGoBackAndCorrectAprQty
        {
            get
            {
                return ResourceRead.GetResourceValue("msgGoBackAndCorrectAprQty", ResourceFileName);
            }
        }

        public static string msgGoAheadWithZeroAprQty
        {
            get
            {
                return ResourceRead.GetResourceValue("msgGoAheadWithZeroAprQty", ResourceFileName);
            }
        }

        public static string msgGoAheadWithReqasAprQty
        {
            get
            {
                return ResourceRead.GetResourceValue("msgGoAheadWithReqasAprQty", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequisitionMaster {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to RequisitionMaster.
        /// </summary>
        public static string RequisitionMasterHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionMasterHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequisitionMaster.
        /// </summary>
        public static string RequisitionMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionMaster", ResourceFileName);
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
        ///   Looks up a localized string similar to RequisitionNumber.
        /// </summary>
        public static string RequisitionNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionNumber", ResourceFileName);
            }
        }

        public static string UncloseReq
        {
            get
            {
                return ResourceRead.GetResourceValue("UncloseReq", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WorkorderName.
        /// </summary>
        public static string WorkorderName
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkorderName", ResourceFileName);
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

        
        public static string RequiredDateStr
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredDate", ResourceFileName);
            }
        }

        public static string MsgRequiredXDaysRed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequiredXDaysRed", ResourceFileName);
            }
        }
        public static string MsgRequiredXDaysGreen
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequiredXDaysGreen", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to NumberofItemsrequisitioned.
        /// </summary>
        public static string NumberofItemsrequisitioned
        {
            get
            {
                return ResourceRead.GetResourceValue("NumberofItemsrequisitioned", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TotalCost.
        /// </summary>
        public static string TotalCost
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalCost", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TotalSellPrice.
        /// </summary>
        public static string TotalSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalSellPrice", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerID.
        /// </summary>
        public static string CustomerID
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerGUID.
        /// </summary>
        public static string CustomerGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerGUID", ResourceFileName);
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

        public static string ProjectSpendGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendID", ResourceFileName);
            }
        }

        public static string ProjectSpendName
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendName", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to RequisitionStatus.
        /// </summary>
        public static string RequisitionStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionStatus", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RejectReason.
        /// </summary>
        public static string RejectReason
        {
            get
            {
                return ResourceRead.GetResourceValue("RejectReason", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequisitionType.
        /// </summary>
        public static string RequisitionType
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BillingAccount.
        /// </summary>
        public static string BillingAccount
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingAccount", ResourceFileName);
            }
        }

        
        public static string BillingAccountID
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingAccount", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
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


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        public static string ModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ModelHeader", ResourceFileName);
            }
        }

        public static string Approved
        {
            get
            {
                return ResourceRead.GetResourceValue("Approved", ResourceFileName);
            }
        }
        public static string Unsubmitted
        {
            get
            {
                return ResourceRead.GetResourceValue("Unsubmitted", ResourceFileName);
            }
        }
        public static string PartialCheckOut
        {
            get
            {
                return ResourceRead.GetResourceValue("PartialCheckOut", ResourceFileName);
            }
        }
        public static string FullCheckOut
        {
            get
            {
                return ResourceRead.GetResourceValue("FullCheckOut", ResourceFileName);
            }
        }
        public static string Submitted
        {
            get
            {
                return ResourceRead.GetResourceValue("Submitted", ResourceFileName);
            }
        }

        public static string Closed
        {
            get
            {
                return ResourceRead.GetResourceValue("Closed", ResourceFileName);
            }
        }

        public static string AssetService
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetService", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Release Number.
        /// </summary>
        public static string ReleaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReleaseNumber", ResourceFileName);
            }
        }

        public static string Requisition
        {
            get
            {
                return ResourceRead.GetResourceValue("Requisition", ResourceFileName);
            }
        }

        public static string ToolService
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolService", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TechnicianID.
        /// </summary>
        public static string TechnicianID
        {
            get
            {
                return ResourceRead.GetResourceValue("TechnicianID", ResourceFileName);
            }
        }

        public static string SupplierApprove
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierApprove", ResourceFileName);
            }
        }
        public static string DuplicateToolRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateToolRequisition", ResourceFileName);
            }
        }

        public static string SelectToolCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectToolCategory", ResourceFileName);
            }
        }

        public static string ToolSerial
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolSerial", ResourceFileName);
            }
        }

        public static string MsgPossibleValuesForRequisitionStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPossibleValuesForRequisitionStatus", ResourceFileName);
            }
        }
        public static string MsgItemOrToolDetailRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemOrToolDetailRequired", ResourceFileName);
            }
        }
        public static string LineItemRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("LineItemRequired", ResourceFileName);
            }
        }
        public static string NoItemsAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("NoItemsAvailable", ResourceFileName);
            }
        }
        public static string msgApprovedRecordsDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("msgApprovedRecordsDelete", ResourceFileName);
            }
        }
        public static string SelectRecordstoUnDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectRecordstoUnDelete", ResourceFileName);
            }
        }
        public static string ReqLineItems
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqLineItems", ResourceFileName);
            }
        }
        public static string NoItemAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("NoItemAvailable", ResourceFileName);
            }
        }
        public static string MsgRequisitionClosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequisitionClosedSuccessfully", ResourceFileName);
            }
        }
        public static string MsgRequisitionNotClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequisitionNotClosed", ResourceFileName);
            }
        }
        public static string MsgRequisitionNotDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequisitionNotDeleted", ResourceFileName);
            }
        }
        public static string RequisitionQtyEqualsDefaultPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionQtyEqualsDefaultPullQty", ResourceFileName);
            }
        }
        public static string ApprovedQtyEqualsDefaultPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ApprovedQtyEqualsDefaultPullQty", ResourceFileName);
            }
        }
        public static string CannotAssignExistingWOToReq
        {
            get
            {
                return ResourceRead.GetResourceValue("CannotAssignExistingWOToReq", ResourceFileName);
            }
        }
        public static string FailToUpdateQtyOfItems
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToUpdateQtyOfItems", ResourceFileName);
            }
        }
        public static string FailToAddTools
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToAddTools", ResourceFileName);
            }
        }
        public static string ToolAddedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolAddedSuccessfully", ResourceFileName);
            }
        }
        public static string ToolCategoryAddedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCategoryAddedSuccessfully", ResourceFileName);
            }
        }
        public static string FailToUpdateUDF
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToUpdateUDF", ResourceFileName);
            }
        }
        public static string ItemUDFUpdatedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemUDFUpdatedSuccessfully", ResourceFileName);
            }
        }
        public static string StatusUpdatedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("StatusUpdatedSuccessfully", ResourceFileName);
            }
        }
        public static string ItemClosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemClosedSuccessfully", ResourceFileName);
            }
        }
        public static string NoRightsToInsertNewWOToReq
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRightsToInsertNewWOToReq", ResourceFileName);
            }
        }
        public static string NoRightsToUseExistingWOToReq
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRightsToUseExistingWOToReq", ResourceFileName);
            }
        }
        public static string NotAllowToApproveRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("NotAllowToApproveRequisition", ResourceFileName);
            }
        }
        public static string ItemOrToolDetailRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemOrToolDetailRequired", ResourceFileName);
            }
        }
        public static string KitOrLaborItemNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("KitOrLaborItemNotAllowed", ResourceFileName);
            }
        }

        public static string ToolSerialNotExist
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolSerialNotExist", ResourceFileName);
            }
        }
        public static string MoreThanEightWeeks
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreThanEightWeeks", ResourceFileName);
            }
        }

        public static string FourToEightWeeks
        {
            get
            {
                return ResourceRead.GetResourceValue("FourToEightWeeks", ResourceFileName);
            }
        }
        public static string LessThanFourWeeks
        {
            get
            {
                return ResourceRead.GetResourceValue("LessThanFourWeeks", ResourceFileName);
            }
        }

        public static string LessThanTwoWeeks
        {
            get
            {
                return ResourceRead.GetResourceValue("LessThanTwoWeeks", ResourceFileName);
            }
        }
        public static string LessThanOneWeek
        {
            get
            {
                return ResourceRead.GetResourceValue("LessThanOneWeek", ResourceFileName);
            }
        }

        public static string Today
        {
            get
            {
                return ResourceRead.GetResourceValue("Today", ResourceFileName);
            }
        }
        public static string PastDue
        {
            get
            {
                return ResourceRead.GetResourceValue("PastDue", ResourceFileName);
            }
        }
        public static string TotalPrice { get { return ResourceRead.GetResourceValue("TotalPrice", ResourceFileName); } }
        public static string YourRequisitionHasBeenApproved { get { return ResourceRead.GetResourceValue("YourRequisitionHasBeenApproved", ResourceFileName); } }
        public static string YourRequisitionHasAlreadyBeenApproved { get { return ResourceRead.GetResourceValue("YourRequisitionHasAlreadyBeenApproved", ResourceFileName); } }
        public static string YourRequisitionIsNotValidToApproval { get { return ResourceRead.GetResourceValue("YourRequisitionIsNotValidToApproval", ResourceFileName); } }
        public static string RequisitionHasAlreadyBeenRejected { get { return ResourceRead.GetResourceValue("RequisitionHasAlreadyBeenRejected", ResourceFileName); } }
        public static string RequisitionWasRejected { get { return ResourceRead.GetResourceValue("RequisitionWasRejected", ResourceFileName); } }
        public static string RequisitionIsNotValidForRejection { get { return ResourceRead.GetResourceValue("RequisitionIsNotValidForRejection", ResourceFileName); } }
        public static string MsgDuplicateRequisition { get { return ResourceRead.GetResourceValue("MsgDuplicateRequisition", ResourceFileName); } }
        public static string MsgToolAgainstCategoryRequisition { get { return ResourceRead.GetResourceValue("MsgToolAgainstCategoryRequisition", ResourceFileName); } }

        public static string MsgRequisitionType { get { return ResourceRead.GetResourceValue("MsgRequisitionType", ResourceFileName); } }
        public static string MsgRequisitionStatus { get { return ResourceRead.GetResourceValue("MsgRequisitionStatus", ResourceFileName); } }
        public static string MsgApprovedRequisitionQuantity { get { return ResourceRead.GetResourceValue("MsgApprovedRequisitionQuantity", ResourceFileName); } }
        public static string MsgToolNameRequired { get { return ResourceRead.GetResourceValue("MsgToolNameRequired", ResourceFileName); } }
        public static string MsgSerialRequired { get { return ResourceRead.GetResourceValue("MsgSerialRequired", ResourceFileName); } }
        public static string MsgPullCanApplyonApprovedRequisitions { get { return ResourceRead.GetResourceValue("MsgPullCanApplyonApprovedRequisitions", ResourceFileName); } }
        public static string ReqCorrecttItemName { get { return ResourceRead.GetResourceValue("ReqCorrecttItemName", ResourceFileName); } }

        public static string RequisitionPullIsWithError { get { return ResourceRead.GetResourceValue("RequisitionPullIsWithError", ResourceFileName); } }
        public static string ProvideCorrectItemNoOrItemGuid { get { return ResourceRead.GetResourceValue("ProvideCorrectItemNoOrItemGuid", ResourceFileName); } }
        public static string UserDoesNotHaveOnTheFlyRight { get { return ResourceRead.GetResourceValue("UserDoesNotHaveOnTheFlyRight", ResourceFileName); } }
        public static string PullQtyShouldBeGreaterThanZero { get { return ResourceRead.GetResourceValue("PullQtyShouldBeGreaterThanZero", ResourceFileName); } }
        public static string ProvideItemBinToPull { get { return ResourceRead.GetResourceValue("ProvideItemBinToPull", ResourceFileName); } }
        public static string ProvideItemNumberToPull { get { return ResourceRead.GetResourceValue("ProvideItemNumberToPull", ResourceFileName); } }
        public static string ProvideRequisitionNoToPull { get { return ResourceRead.GetResourceValue("ProvideRequisitionNoToPull", ResourceFileName); } }
        public static string ProvideUserNameToPull { get { return ResourceRead.GetResourceValue("ProvideUserNameToPull", ResourceFileName); } }
        public static string ProvideRoomNameToPull { get { return ResourceRead.GetResourceValue("ProvideRoomNameToPull", ResourceFileName); } }
        public static string ProvideCompanyNameToPull { get { return ResourceRead.GetResourceValue("ProvideCompanyNameToPull", ResourceFileName); } }
        public static string MsgRightsInsertRequisition { get { return ResourceRead.GetResourceValue("MsgRightsInsertRequisition", ResourceFileName); } }
        public static string RequisitionNumberRequired { get { return ResourceRead.GetResourceValue("RequisitionNumberRequired", ResourceFileName); } }
        public static string RequisitionApproveRights { get { return ResourceRead.GetResourceValue("RequisitionApproveRights", ResourceFileName); } }
        public static string RequisitionCloseRights { get { return ResourceRead.GetResourceValue("RequisitionCloseRights", ResourceFileName); } }
        public static string RequiredDateRequired { get { return ResourceRead.GetResourceValue("RequiredDateRequired", ResourceFileName); } }
        public static string RequisitionNumberAlreadyExists { get { return ResourceRead.GetResourceValue("RequisitionNumberAlreadyExists", ResourceFileName); } }
        public static string RequisitionNumberExistsInList { get { return ResourceRead.GetResourceValue("RequisitionNumberExistsInList", ResourceFileName); } }
        public static string RequisitionClosedStatusValidation { get { return ResourceRead.GetResourceValue("RequisitionClosedStatusValidation", ResourceFileName); } }
        public static string RequisitionApproveSupplierValidation { get { return ResourceRead.GetResourceValue("RequisitionApproveSupplierValidation", ResourceFileName); } }
        public static string RequisitionNumberNotExist { get { return ResourceRead.GetResourceValue("RequisitionNumberNotExist", ResourceFileName); } }
        public static string ItemNumberCorrectItemGuidValidation { get { return ResourceRead.GetResourceValue("ItemNumberCorrectItemGuidValidation", ResourceFileName); } }
        public static string ToolNameSerialNotExists { get { return ResourceRead.GetResourceValue("ToolNameSerialNotExists", ResourceFileName); } }
        public static string ToolCategoryNotExists { get { return ResourceRead.GetResourceValue("ToolCategoryNotExists", ResourceFileName); } }
        public static string ToolCategoryDoesNotMatch { get { return ResourceRead.GetResourceValue("ToolCategoryDoesNotMatch", ResourceFileName); } }
        public static string MsgRequisitionMultiValidation { get { return ResourceRead.GetResourceValue("MsgRequisitionMultiValidation", ResourceFileName); } }
        public static string MsgValidLineItem { get { return ResourceRead.GetResourceValue("MsgValidLineItem", ResourceFileName); } }
        public static string RequisitionStatusValidation { get { return ResourceRead.GetResourceValue("RequisitionStatusValidation", ResourceFileName); } }
        public static string MsgToolGuidToolCategoryValidation { get { return ResourceRead.GetResourceValue("MsgToolGuidToolCategoryValidation", ResourceFileName); } }
        public static string RequisitionErrorCheckItemsValidation { get { return ResourceRead.GetResourceValue("RequisitionErrorCheckItemsValidation", ResourceFileName); } }
        public static string BtnCloseRequistion { get { return ResourceRead.GetResourceValue("BtnCloseRequistion", ResourceFileName); } }
        public static string BtnCloseItem { get { return ResourceRead.GetResourceValue("BtnCloseItem", ResourceFileName); } }
        public static string Tool { get { return ResourceRead.GetResourceValue("Tool", ResourceFileName); } }
        public static string AddToolsCategoryHeader { get { return ResourceRead.GetResourceValue("AddToolsCategoryHeader", ResourceFileName); } }
        public static string AddToolsRequisitionHeader { get { return ResourceRead.GetResourceValue("AddToolsRequisitionHeader", ResourceFileName); } }
        public static string CloseSelectedLineItemValidation { get { return ResourceRead.GetResourceValue("CloseSelectedLineItemValidation", ResourceFileName); } }
        public static string RequisitionArchievedValidation { get { return ResourceRead.GetResourceValue("RequisitionArchievedValidation", ResourceFileName); } }
        public static string RequisitionFiles { get { return ResourceRead.GetResourceValue("RequisitionFiles", ResourceFileName); } }
        public static string RequisitionImageError { get { return ResourceRead.GetResourceValue("RequisitionImageError", ResourceFileName); } }
        public static string AttachmentDeleteValidation { get { return ResourceRead.GetResourceValue("AttachmentDeleteValidation", ResourceFileName); } }
        public static string CloseSelectedRequisition { get { return ResourceRead.GetResourceValue("CloseSelectedRequisition", ResourceFileName); } }
        public static string ReqOneUnClosedRequisition { get { return ResourceRead.GetResourceValue("ReqOneUnClosedRequisition", ResourceFileName); } }
        public static string SelectedRequisitionClosedSuccessfully { get { return ResourceRead.GetResourceValue("SelectedRequisitionClosedSuccessfully", ResourceFileName); } }




    }

    public class RPT_RequistionWithItemDTO
    {
        public Int64 ID { get; set; }
        public string Bin { get; set; }
        public string ProjectSpend { get; set; }
        public double? Total { get; set; }
        public string ActionType { get; set; }
        public Guid GUID { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }

        public string ReqDtlCreatedBy { get; set; }
        public string ReqDtlLastUpdatedBy { get; set; }

        public Int64? RoomID { get; set; }
        public Int64? CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public bool? IsEDIRequired { get; set; }
        public string LastEDIDate { get; set; }
        public bool? IsEDISent { get; set; }

        public string RequisitionNumber { get; set; }
        public string CountName { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public string ItemNumber { get; set; }
        public string ItemUniqueNumber { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public string ItemDescription { get; set; }
        public string LongDescription { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemBlanketPO { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public string GLAccount { get; set; }
        public string Unit { get; set; }
        public string DefaultLocationName { get; set; }
        public string InventoryClassificationName { get; set; }
        public string CostUOM { get; set; }
        public int? InventoryClassification { get; set; }
        public int? LeadTimeInDays { get; set; }
        public string ItemTypeName { get; set; }
        public double? ItemCost { get; set; }
        public double? SellPrice { get; set; }
        public double? ExtendedCost { get; set; }
        public double? AverageCost { get; set; }
        public double? PricePerTerm { get; set; }
        public double? OnHandQuantity { get; set; }
        public double? StagedQuantity { get; set; }
        public double? ItemInTransitquantity { get; set; }
        public double? RequisitionedQuantity { get; set; }
        public double? CriticalQuantity { get; set; }
        public double? MinimumQuantity { get; set; }
        public double? MaximumQuantity { get; set; }
        public double? DefaultReorderQuantity { get; set; }
        public double? DefaultPullQuantity { get; set; }
        public double? AverageUsage { get; set; }
        public double? Turns { get; set; }
        public double? Markup { get; set; }
        public string WeightPerPiece { get; set; }
        public string Consignment { get; set; }
        public string IsTransfer { get; set; }
        public string IsPurchase { get; set; }
        public string SerialNumberTracking { get; set; }
        public string LotNumberTracking { get; set; }
        public string DateCodeTracking { get; set; }
        public string IsBuildBreak { get; set; }
        public string Taxable { get; set; }
        public Int64? ItemID { get; set; }
        public string RoomInfo { get; set; }
        public string CompanyInfo { get; set; }
        public string BarcodeImage_ItemNumber { get; set; }
        public string BarcodeImage_PullBin { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public int? CostDecimalPoint { get; set; }


        public string Description { get; set; }
        public string RequisitionStatus { get; set; }
        public string RequisitionType { get; set; }
        public string WOName { get; set; }
        public string Customer { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }


        public string UpdatedBy { get; set; }
        public string PULLUDF1 { get; set; }
        public string PULLUDF2 { get; set; }
        public string PULLUDF3 { get; set; }
        public string PULLUDF4 { get; set; }
        public string PULLUDF5 { get; set; }
        public double? RequisitionCost { get; set; }
        public double? REQD_ItemCost { get; set; }
        public string RequiredDate { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public double? QuantityRequisitioned { get; set; }
        public double? ApproveQuantity { get; set; }
        public double? QuantityPulled { get; set; }
        public string IsDeleted { get; set; }
        public string IsArchived { get; set; }
        public string BinNumber { get; set; }

        public string ItemSupplier { get; set; }

        public double? OnReturnQuantity { get; set; }
        public double? OnOrderQuantity { get; set; }
        public double? OnTransferQuantity { get; set; }
        public double? SuggestedOrderQuantity { get; set; }
        public string BarcodeImage_OrderItemBin { get; set; }
        public string BarcodeImage_WorkOrder { get; set; }
        public string BarcodeImage_RequisitionNumb { get; set; }

        public int? NumberofItemsrequisitioned { get; set; }
        public Guid? RequisitionGUID { get; set; }
        public Guid? ItemGuid { get; set; }
        public Guid RequisitionDetailGUID { get; set; }
        public string ConsignedPO { get; set; }

        public string ReleaseNumber { get; set; }

        public Int64? SupplierID { get; set; }
        public Int64? CategoryID { get; set; }
        public Int64? ManufacturerID { get; set; }
        public string SupplierPartNoValue { get; set; }
        public string ManufacturerNumberValue { get; set; }
        public string UNSPSCValue { get; set; }
        public Guid? WorkorderGUID { get; set; }
        public Guid? ProjectSpendGUID { get; set; }
    }

    public class RPT_Requistions
    {
        public string RequisitionNumber { get; set; }
        public Guid GUID { get; set; }
        public string RequisitionStatus { get; set; }
        public string RequisitionType { get; set; }
        public string SupplierAccountNumber { get; set; }
        public string SupplierAccountName { get; set; }
        public string SupplierAccountAddress { get; set; }
        public string SupplierAccountCity { get; set; }
        public string SupplierAccountState { get; set; }
        public string SupplierAccountZipcode { get; set; }
        public string SupplierAccountDetailWithFullAddress { get; set; }

        public string ReleaseNumber { get; set; }
    }
}


