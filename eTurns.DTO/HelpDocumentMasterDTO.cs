using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class HelpDocumentMasterDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ID", ResourceType = typeof(ResHelpDocumentMaster))]
        public System.Int64 ID { get; set; }

        //AssetName
        [Display(Name = "ModuleName", ResourceType = typeof(ResHelpDocumentMaster))]
        public System.String ModuleName { get; set; }

        //Description
        [Display(Name = "ModuleDocPath", ResourceType = typeof(ResHelpDocumentMaster))]
        public System.String ModuleDocPath { get; set; }

        //Make
        [Display(Name = "ModuleDocName", ResourceType = typeof(ResHelpDocumentMaster))]
        public System.String ModuleDocName { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        [Display(Name = "ModuleVideoPath", ResourceType = typeof(ResHelpDocumentMaster))]
        public System.String ModuleVideoPath { get; set; }

        public bool? IsDoc { get; set; }

        public bool? IsVideo { get; set; }
        public int? TotalRecords { get; set; }

        [Display(Name = "ModuleVideoName", ResourceType = typeof(ResHelpDocumentMaster))]
        public System.String ModuleVideoName { get; set; }

        public static string ValidFileExtention = ".pdf,.mp4";

        public List<HelpDocumentDetailDTO> HelpDocDetail { get; set; }
        
    }

    public class ResHelpDocumentMaster
    {
        private static string ResourceFileName = "ResHelpDocumentMaster";

        public static string ModuleName
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleName", ResourceFileName);
            }
        }

        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        public static string ModuleDocPath
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleDocPath", ResourceFileName);
            }
        }

        public static string ModuleDocName
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleDocName", ResourceFileName);
            }
        }

        public static string FileNotSelect
        {
            get
            {
                return ResourceRead.GetResourceValue("FileNotSelect", ResourceFileName);
            }
        }

        public static string VideoUpload
        {
            get
            {
                return ResourceRead.GetResourceValue("VideoUpload", ResourceFileName);
            }
        }

        public static string DocShow
        {
            get
            {
                return ResourceRead.GetResourceValue("DocShow", ResourceFileName);
            }
        }

        public static string VideoShow
        {
            get
            {
                return ResourceRead.GetResourceValue("VideoShow", ResourceFileName);
            }
        }

        public static string ModuleVideoPath
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleVideoPath", ResourceFileName);
            }
        }

        public static string ModuleVideoName
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleVideoName", ResourceFileName);
            }
        }

        //

        public static string NoValidPDFFile
        {
            get
            {
                return ResourceRead.GetResourceValue("NoValidPDFFile", ResourceFileName);
            }
        }

        public static string NoValidVideoFile
        {
            get
            {
                return ResourceRead.GetResourceValue("NoValidVideoFile", ResourceFileName);
            }
        }

        public static string Upload
        {
            get
            {
                return ResourceRead.GetResourceValue("Upload", ResourceFileName);
            }
        }

        public static string DocumentDeleteConfirmMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("DocumentDeleteConfirmMessage", ResourceFileName);
            }
        }
        public static string VideoDeleteConfirmMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("VideoDeleteConfirmMessage", ResourceFileName);
            }
        }
        public static string DeleteDocumentSuccessMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("DeleteDocumentSuccessMessage", ResourceFileName);
            }
        }
        public static string DeleteVideoSuccessMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("DeleteVideoSuccessMessage", ResourceFileName);
            }
        }
        public static string DeleteDocumentErrorMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("DeleteDocumentErrorMessage", ResourceFileName);
            }
        }
        public static string DeleteVideoErrorMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("DeleteVideoErrorMessage", ResourceFileName);
            }
        }
        public static string ValidFileExtension
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidFileExtension", ResourceFileName);
            }
        }
        public static string SpecialCharacterNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("SpecialCharacterNotAllowed", ResourceFileName);
            }
        }

        public static string MaxCharacterAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxCharacterAllowed", ResourceFileName);
            }
        }
        public static string ReqName
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqName", ResourceFileName);
            }
        }
        public static string FileUploadSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("FileUploadSuccessfully", ResourceFileName);
            }
        }
        public static string VideoUploadSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("VideoUploadSuccessfully", ResourceFileName);
            }
        }
        public static string FileVideoUploadSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("FileVideoUploadSuccessfully", ResourceFileName);
            }
        }

        public static string Operations
        {
            get
            {
                return ResourceRead.GetResourceValue("Operations", ResourceFileName);
            }
        }

        public static string Reporting
        {
            get
            {
                return ResourceRead.GetResourceValue("Reporting", ResourceFileName);
            }
        }
        public static string ReportHeading
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportHeading", ResourceFileName);
            }
        }

        public static string ReportDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportDescription", ResourceFileName);
            }
        }

        

    }

    public enum HelpDocType
    {
        Module = 1,
        Report = 2,
        Mobile = 3,
    }

    public class HelpDocumentMasterList
    {
        public string ModuleItemsPDFPath { get; set; }
        public string ModuleQLPDFPath { get; set; }
        public string ModuleCountPDFPath { get; set; }
        public string MaterialStagingPDFPath { get; set; }
        public string MoveMaterialPDFPath { get; set; }

        public string PullsPDFPath { get; set; }
        public string RequisitionPDFPath { get; set; }
        public string WorkOrderPDFPath { get; set; }
        public string ProjectSpendPDFPath { get; set; }

        public string CartPDFPath { get; set; }
        public string OrdersPDFPath { get; set; }
        public string ReceivePDFPath { get; set; }
        public string TransferPDFPath { get; set; }
        public string ReturnOrderPDFPath { get; set; }
        public string QuotePDFPath { get; set; }

        public string ToolsPDFPath { get; set; }
        public string AssetPDFPath { get; set; }
        public string MaintenancePDFPath { get; set; }


        public string KitsPDFPath { get; set; }

        public string RptAuditTrail { get; set; }
        public string RptInventoryReconciliation { get; set; }
        public string RptPullSummarybyQuarter { get; set; }
        public string RptAuditTrailTransaction { get; set; }
        public string RptInventoryStockOut { get; set; }
        public string RptPullSummaryByWO { get; set; }
        public string RptItemReceivedReceivable { get; set; }
        public string RptQuoteSummary { get; set; }
        public string RptCompany { get; set; }
        public string RptReceivableItems { get; set; }
        public string RptCreditPull { get; set; }
        public string RptItemSerialNumber { get; set; }
        public string RptReceivedItems { get; set; }
        public string RptCumulativePull { get; set; }
        public string RptItemStockOutHistory { get; set; }
        public string RptReceivedItemsMoreThanApproved { get; set; }
        public string RptDiscrepancyReport { get; set; }
        public string RptItemsWithSuppliers { get; set; }
        public string RptRequisitionItemSummary { get; set; }
        public string RptEnterpriseRoom { get; set; }
        public string RptKitDetail { get; set; }
        public string RptRequisitionWithLineItems { get; set; }
        public string RptEnterprisesList { get; set; }
        public string RptKitSerial { get; set; }

        public string RptReturnItemCandidates { get; set; }
        public string RptEnterpriseUser { get; set; }
        public string RptKitSummary { get; set; }
        public string RptRoom { get; set; }
        public string RpteVMIPollHistory { get; set; }
        public string RptMaintenanceDue { get; set; }
        public string RptSupplier { get; set; }
        public string RpteVMIUsage { get; set; }
        public string RptNotPulledReport { get; set; }
        public string RptToolAuditTrail { get; set; }
        public string RpteVMIUsageManualCount { get; set; }
        public string RptOrderItemSummary { get; set; }
        public string RptToolAuditTrailTransaction { get; set; }
        public string RpteVMIUsageNoHeader { get; set; }
        public string RptOrderSummary { get; set; }

        public string RptExpiringItems { get; set; }
        public string RptOrderSummaryLineItem { get; set; }
        public string RptToolAssetOrder { get; set; }
        public string RptInStock { get; set; }
        public string RptOrdersClosed { get; set; }
        public string RptToolAssetOrdersWithLineItems { get; set; }
        public string RptInStockByActivity { get; set; }
        public string RptOrdersWithLineItems { get; set; }

        public string RptToolscheckedout { get; set; }
        public string RptInStockMargin { get; set; }
        public string RptPreciseDemandPlanning { get; set; }
        public string RptToolsCheckInoutHistory { get; set; }
        public string RptInstockwithQOH { get; set; }
        public string RptPreciseDemandPlanningByItem { get; set; }
        public string RptTransferWithLineItems { get; set; }
        public string RptInventoryCountConsigned { get; set; }
        public string RptPullCompleted { get; set; }
        public string RptUsers { get; set; }
        public string RptInventoryCountCustomerOwned { get; set; }
        public string RptPullIncomplete { get; set; }
        public string RptWorkOrderLastCost { get; set; }
        public string RptInventoryDailyHistory { get; set; }
        public string RptPullItemSummary { get; set; }
        public string RptWorkOrderWithAttachment { get; set; }
        public string RptInventoryDailyHistoryWithDateRange { get; set; }
        public string RptPullNoHeader { get; set; }
        public string RptWorkOrderWithGroupedPulls { get; set; }
        public string RptPullSummary { get; set; }
        public string RptWorkordersList { get; set; }
        public string RptPullSummaryByConsignedPO { get; set; }
        public string RptWrittenOffTools { get; set; }
        public string RptAuditTrailTransactionSummary { get; set; }
        public string RptItemSerialLotDatcode { get; set; }
        public string RptToolInstock { get; set; }
    }

}
