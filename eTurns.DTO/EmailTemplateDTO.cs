using System.Collections.Generic;
using eTurns.DTO.Resources;

namespace eTurns.DTO
{
    public class EmailTemplateDTO
    {
        public System.Int64 ID { get; set; }
        public System.String TemplateName { get; set; }
        public System.Int64 RoomId { get; set; }
        public System.Int64 CompanyId { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public System.String MailSubject { get; set; }
        public System.String MailBodyText { get; set; }
        public IEnumerable<EmailTemplateDetailDTO> lstEmailTemplateDtls { get; set; }

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
        public string IdWithValue { get; set; }

        public string ResourceKeyName { get; set; }
        private string _alertResourceName;
        public string AlertResourceName
        {
            get
            {
                if (!string.IsNullOrEmpty(ResourceKeyName))
                {
                    _alertResourceName = ResourceHelper.GetAlertNameByResource(ResourceKeyName);
                }
                else
                {
                    _alertResourceName = TemplateName;
                }
                return _alertResourceName;
            }
            set { this._alertResourceName = value; }
        }

        public bool IsSupplierRequired { get; set;}
    }
    public class EmailTemplateDetailDTO
    {
        public System.Int64 ID { get; set; }
        public System.Int64 EmailTempateId { get; set; }
        public System.String MailBodyText { get; set; }
        public System.Int64 ResourceLaguageId { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public System.Int64 CompanyID { get; set; }
        public System.Int64 RoomId { get; set; }
        public System.String MailSubject { get; set; }
        public System.Int64? NotificationID { get; set; }
        public string CultureCode { get; set; }


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
        public System.Int64? ReportId { get; set; }

    }

    public enum MailTemplate
    {
        CycleCounteMail = 1,
        CycleCountItemsMissed = 2,
        ReportTemplate = 3,
        ForgotPassword = 4,
        ItemQtyAdjust = 5,
        ItemReceipt = 6,
        ItemStockOut = 7,
        ItemUsage = 8,
        MaintanceDue = 9,
        OrderApproval = 10,
        OrderApproveReject = 11,
        PendingOrders = 12,
        PendingRequisition = 13,
        PendingTransfers = 14,
        RequisitionApproval = 15,
        RequisitionApproveReject = 16,
        //SensorNoWorking = 17,
        eVMIProblems = 17,
        SuggestedOrdersCritical = 18,
        TransferApproval = 19,
        TransferApproveReject = 20,
        OrderToSupplier = 21,
        SuggestedOrdersMinimum = 22,
        ReturnOrderApproval = 23,
        ReturnOrderApproveReject = 24,
        ReturnOrderToSupplier = 25,
        SuggestedOrders = 26,
        PendingReturnOrders = 27,
        CreateNewUser = 28,
        CountApplyMail = 29,
        ConsignedBatchPull = 30,
        sFTPFailEmail = 31,
        DiscrepancyAfterSync = 32,
        CartByRFIDAfterCall = 33,
        ProjectSpendLimitAboutToExceed = 34,
        ProjectSpendLimitExceed = 35,
        OrderUnSubmitted = 36,
        ToolAssetOrderApproval = 37,
        ToolAssetOrderApproveReject = 38,
        ToolAssetOrderUnSubmitted = 39,
        eVMIPollDone = 40,
        eVMITareDone = 41,
        eVMIItemWeightPerPieceDone = 42,
        eVMIMissingPoll = 43,
        eVMIShelfIDDone = 44,
        eVMICalibrateDone = 45,
        eVMIResetDone = 46,
        QuoteUnSubmitted = 47,
        QuoteToSupplier = 48,
        QuoteApproval = 49,
        QuoteApproveReject = 50,
        RequisitionByRFIDAfterCall = 53,
        OrderByRFIDAfterCall = 54,
    }
}
