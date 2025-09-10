using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    [Serializable]
    public enum ApprovalFrequencyType
    {
        None = 0,
        Days = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public enum OrderLimitType
    {
        All = 1,
        PerOrder = 2
    }

    public enum PermissionTypeInfo
    {
        View = 1,
        Insert = 2,
        Update = 3,
        Delete = 4,
        ShowDeleted = 5,
        ShowArchived = 6,
        ShowUDF = 7,
        Approval = 8,
        Submit = 9,
        ChangeOrder = 10,
        AllowPull = 11,
        IsChecked = 12,
        ShowChangeLog = 13,
    }

    public enum ModuleInfo
    {
        None = 0,
        QuickListPermission = 10,
        Reports = 15,
        OnTheFlyEntry = 29,
        AllowOrderToConsignedItem = 34,
        BinMaster = 37,
        CategoryMaster = 38,
        CompanyMaster = 39,
        CustomerMaster = 40,
        EnterpriseMaster = 41,
        FreightTypeMaster = 42,
        GLAccountsMaster = 43,
        GXPRConsignedJobMaster = 44,
        JobTypeMaster = 45,
        ProjectMaster = 46,
        SupplierMaster = 47,
        ShipViaMaster = 48,
        TechnicianMaster = 49,
        ToolMaster = 50,
        UnitMaster = 51,
        RoomMaster = 52,
        LocationMaster = 53,
        ManufacturerMaster = 54,
        MeasurementTermMaster = 55,
        RoleMaster = 58,
        UserMaster = 59,
        ResourceMaster = 60,
        ItemMaster = 61,
        PullMaster = 62,
        Count = 63,
        Suppliercatalog = 64,
        Materialstaging = 65,
        Requisitions = 66,
        WorkOrders = 67,
        Cart = 68,
        Orders = 69,
        Receive = 70,
        Transfer = 71,
        Assets = 72,
        AssetMaintenance = 113,
        Kits = 74,
        WIP = 75,
        OrderApproval = 76,
        EmailConfiguration = 77,
        AllowOverrideProjectSpendLimits = 78,
        AllowedToChangeConsignedQuantityItems = 79,
        AllowChangeConsignedItems = 22,
        Imports = 81,
        ToolCategory = 82,
        VenderMaster = 83,
        OrderSubmit = 33,
        ChangeOrder = 84,
        RequisitionApproval = 85,
        ExternalUserConfiguration = 86,
        TransferSubmit = 87,
        TransferApproval = 88,
        ChangeTransfer = 89,
        ResetAutoNumbers = 90,
        Synch = 91,
        Barcode = 92,
        CompanyConfig = 93,
        eVMISetup = 94,
        CostUOMMaster = 96,
        InventoryClassificationMaster = 97,
        AllowConsignedCreditPull = 32,
        LabelPrinting = 101,
        HideCostMarkUpSellPrice = 100,
        ReturnOrder = 102,
        MoveMaterial = 103,
        PdaColumnsettings = 104,
        BOMItemMaster = 105,
        EnterPriseConfiguration = 106,
        HelpDocument = 107,
        ToolsScheduler = 108,
        AssetCategory = 109,
        PasswordResetRule = 110,
        AssetToolScheduler = 111,
        AssetToolSchedulerMapping = 112,
        PermissionTemplates = 114,
        ExportPermission = 5,
        Notifications = 115,
        Allowapplyoncounts = 98,
        FTPMaster = 37,
        AllowOverwriteLotOrSerial = 116,
        AllowToEnterLotOrSerialInBlankBox = 117,
        ViewOnlyLotOrSerial = 118,
        AllowAnOrderToBeUnclose = 119,
        AllowAnOrderToBeUnapprove = 120,
        CartOrder = 11,
        CartTransferPermission = 12,
        PreventTransmittedOrdersFromDisplayingInRedCount = 121,
        AllowCheckInCheckout = 122,
        CatalogReport = 123,
        RequisitionClosing = 124,
        OrderApprovalDollerLimit = 125,
        AllowtoViewDashboard = 126,
        ToolCheckInCheckOut = 127,
        ToolAssetOrder = 128,
        ToolAssetOrderApproval = 129,
        ToolAssetOrderSubmit = 130,
        ReceiveToolAsset = 131,
        AllowToolWrittenOff = 132,
        SuggestedReturnpermission = 141,
        AllowtoViewMinMaxDashboard = 145,
        Quote = 148,
        AllowanquotetobeApproved = 149,
        AllowanquotetobeSubmitted = 150,
        AllowanquotetobeUnapprove = 151,
        AllowanquotetobeUnclose = 152,
        Allowanchangequote = 153,
        QuoteToOrder = 154

    }

    [Serializable]
    public enum WorkOrderCreatedFrom
    {
        Web = 1,
        EDIService = 2,
        PDA = 3,
        IntegrateAPI = 4
    }

    [Serializable]
    public enum OrderCreatedFrom
    {
        Web = 1,
        EDIService = 2,
        PDA = 3,
        IntegrateAPI = 4
    }

    [Serializable]
    public class DollerApprovalLimitDTO
    {
        public DollerApprovalLimitDTO()
        {
            FrequencyValue = 0;
            DollerLimit = null;
            UsedLimit = 0;
            FrequencyType = ApprovalFrequencyType.None;
            OrderLimitType = OrderLimitType.All;
            ItemApprovedQuantity = null;
        }
        public int FrequencyValue { get; set; }
        public ApprovalFrequencyType FrequencyType { get; set; }

        public double? DollerLimit { get; set; }

        public double UsedLimit { get; set; }

        public OrderLimitType OrderLimitType { get; set; }

        public double? ItemApprovedQuantity { get; set; }

    }

    [Serializable]
    public class CommonDTO
    {
        public Int64 ID { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public string Value { get; set; }
        public int Count { get; set; }
        public string PageName { get; set; }
        public string ControlID { get; set; }
        public string ItemModelCallFromPageName { get; set; }
        public string IDsufix { get; set; }

        public string IDsufix2 { get; set; }

        public Int64 ParentID { get; set; }

        public string ToolModelCallFromPageName { get; set; }


        private string _listName;

        /// <summary>
        /// Property to identify page uniquely. If blank then return PageName
        /// PageName can be same for 2 pages , List name mst be unique
        /// </summary>
        public string ListName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_listName))
                {
                    _listName = PageName;
                }
                return _listName;
            }
            set
            {
                _listName = value;
            }
        }
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }
    }

    [Serializable]
    public class UserSyncHistoryDTO
    {
        public long ID { get; set; }
        public long SyncByUserID { get; set; }
        public string SyncStep { get; set; }
        public DateTime SyncTime { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public string BuildNo { get; set; }
        public string DeviceName { get; set; }
        public Guid SyncTransactionID { get; set; }
        public string ErrorDescription { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string RoomIDs { get; set; }
    }
    public class GetBinInputPara
    {
        /// <summary>
        /// get Staging location
        /// </summary>
        public bool IsStaging { get; set; }

        /// <summary>
        /// Staging header Name
        /// Only take those location under Staging header
        /// </summary>
        public string StagingHeaderName { get; set; }

        /// <summary>
        /// Include quanity with Lacation name like "ABCBin (10.00)"
        /// </summary>
        public bool IncludeQty { get; set; }

        /// <summary>
        /// Exclude Location with Zero Qty
        /// </summary>
        public bool ExcludeZeroQty { get; set; }

        /// <summary>
        /// Take All location from BinMaster and 
        /// by Isstaging  use
        /// </summary>
        public bool GetAllBins { get; set; }


        /// <summary>
        /// ItemGuid then take those location which bind to itemguid.
        /// </summary>
        public string ItemGuid { get; set; }

        public string NameStartWith { get; set; }

    }

    public class DTOForAutoComplete
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
        public double Quantity { get; set; }

        public string OtherInfo1 { get; set; }
        public string OtherInfo2 { get; set; }
        public string OtherInfo3 { get; set; }
        public string OtherInfo4 { get; set; }
        public string OtherInfo5 { get; set; }
        public Nullable<Guid> ItemGuid { get; set; }

        public Nullable<Guid> ToolGuid { get; set; }
    }

    public class SupplierCatalogNS : CommonDTO
    {
        public bool ShowSupplirSearch { get; set; }
    }

    [Serializable]
    public class KeyValDTO
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class IdNameDTO
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
    }

    public class KeyValCheckDTO
    {
        public string key { get; set; }
        public string value { get; set; }


        public string engValue { get; set; }

        public bool chkvalue { get; set; }
    }

    //public class LocalizationDisplayNameAttribute : DisplayNameAttribute
    //{
    //    private DisplayAttribute display;

    //    public LocalizationDisplayNameAttribute(string resourceName, string ResourceFileName)
    //    {
    //        Type t = Type.GetType(CommonDTO.RText + "." + ResourceFileName);
    //        this.display = new DisplayAttribute()
    //        {
    //            ResourceType = t,
    //            Name = resourceName
    //        };
    //    }

    //    public override string DisplayName
    //    {
    //        get
    //        {
    //            return display.GetName();
    //        }
    //    }
    //}


    //public class MyLocalizationDisplayNameAttribute : DisplayNameAttribute
    //{
    //    private DisplayAttribute display;
    //    private ResourceManager resmgr = null;
    //    private string ResourceKey;
    //    private string Resoucepath;
    //    public MyLocalizationDisplayNameAttribute(string resourceName, string ResourceFileName)
    //    {

    //        ResourceKey = resourceName;
    //        Resoucepath = CommonDTO.RText + "." + ResourceFileName;


    //    }

    //    public override string DisplayName
    //    {
    //        get
    //        {
    //            resmgr = new ResourceManager(Resoucepath, Assembly.GetExecutingAssembly());

    //            //return resmgr.GetString(ResourceKey);
    //            string strcult = System.Globalization.CultureInfo.CurrentCulture.ToString();
    //            if (strcult.Contains("en-US"))
    //                strcult = "";
    //            else
    //                strcult = "." + strcult;
    //            XmlDocument loResource = new XmlDocument();
    //            loResource.Load(@"D:\Projects\eTurns\Trunk\eTurns\eTurns.DTO\Resources\MasterResources\BinMaster" + strcult + ".Resx");
    //            XmlNode loRoot = loResource.SelectSingleNode("root/data[@name='" + ResourceKey + "']/value");
    //            return loRoot.InnerText;
    //        }
    //    }
    //}

    public class AutoSequenceNumbersDTO
    {
        public Int64 ID { get; set; }
        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }
        public string Prefix { get; set; }
        public Int64 RangeStartFrom { get; set; }
        public int Increment { get; set; }
        public Int64 LastGenereted { get; set; }

    }

    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }


    public class bomUpdateResp
    {
        public Nullable<Int64> room { get; set; }
        public string status { get; set; }

        public string message { get; set; }
    }

    public class NarrowSearchData
    {
        public List<CommonDTO> CreatedByList { get; set; }
        public List<CommonDTO> UpdatedByList { get; set; }
        public List<CommonDTO> SupplierByList { get; set; }
        public List<CommonDTO> StatusByList { get; set; }
        public List<CommonDTO> RequiredDateList { get; set; }
        public List<CommonDTO> UDF1List { get; set; }
        public List<CommonDTO> UDF2List { get; set; }
        public List<CommonDTO> UDF3List { get; set; }
        public List<CommonDTO> UDF4List { get; set; }
        public List<CommonDTO> UDF5List { get; set; }
        public List<CommonDTO> ChangeOrderList { get; set; }
        public List<CommonDTO> LabelModuleList { get; set; }
        public List<CommonDTO> LabelOnlyBaseList { get; set; }
        public List<CommonDTO> CategoryByList { get; set; }
        public List<CommonDTO> ManufactureByList { get; set; }
        public List<CommonDTO> ItemTypeByList { get; set; }
        public List<CommonDTO> VendorList { get; set; }

        public List<CommonDTO> ItemLocationList { get; set; }
        public List<CommonDTO> eVMICOMCommonRequestType { get; set; }
        public List<CommonDTO> PackslipNumberList { get; set; }
    }

    public class ItemQuantityDetail
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ItemNo { get; set; }
        public double qty { get; set; }
        public string status { get; set; }
        public string requirdate { get; set; }
        public string requestType { get; set; }
        public Guid? GUID { get; set; }
        public string ReleaseNumber { get; set; }
        public string BinNumber { get; set; }
        public string ItemQuantityType {get ; set; }
    }

    public class RedCountDTO
    {
        public string ModuleName { get; set; }
        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }
        public string Status { get; set; }
        public int RecCircleCount { get; set; }

    }

    public class eTurnsUtility
    {
        public void SendMail(string ToEmailID, string CCEmailID, string Subject, string Body, System.Collections.ArrayList _Attachments = null)
        {
            return;
            //            MailMessage mm = null;
            //            SmtpClient smtp = null;

            //            Int64 EnterpriseID = 0, RoomID = 0, CompanyID = 0, UserID = 0;
            //            string CallFrom = "Web => ";
            //            System.Data.SqlClient.SqlConnection cn = null;
            //            System.Data.SqlClient.SqlCommand cmd = null;
            //            int HasAttachment = 0;
            //            string BCCAddress = "";
            //            try
            //            {

            //                string strPath = "";
            //                string strReplacepath = "";

            //                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
            //                {
            //                    strPath = System.Web.HttpContext.Current.Request.Url.ToString();
            //                    strReplacepath = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            //                    strPath = strPath.Replace(strReplacepath, "/");

            //                    if (System.Web.HttpContext.Current.Session["EnterPriceID"] != null)
            //                        EnterpriseID = Int64.Parse(System.Web.HttpContext.Current.Session["EnterPriceID"].ToString());

            //                    if (System.Web.HttpContext.Current.Session["RoomID"] != null)
            //                        RoomID = Int64.Parse(System.Web.HttpContext.Current.Session["RoomID"].ToString());
            //                    if (System.Web.HttpContext.Current.Session["CompanyID"] != null)
            //                        CompanyID = Int64.Parse(System.Web.HttpContext.Current.Session["CompanyID"].ToString());
            //                    if (System.Web.HttpContext.Current.Session["UserID"] != null)
            //                        UserID = Int64.Parse(System.Web.HttpContext.Current.Session["UserID"].ToString());

            //                    CallFrom += " " + System.Web.HttpContext.Current.Request.Url.ToString();

            //                }
            //                else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DomainName"]))
            //                {
            //                    strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];

            //                    CallFrom = " Service ";
            //                }

            //                Body = Body.Replace("@@ETURNSLOGO@@", GeteTurnsImage(strPath, "Content/images/logo.jpg"));
            //                Body = Body.ToString().Replace("@@Year@@", DateTime.Now.Year.ToString());

            //                mm = new MailMessage();
            //                mm.Subject = Subject;
            //                mm.Body = Body;
            //                mm.IsBodyHtml = true;
            //                mm.To.Add(ToEmailID);

            //                if (!string.IsNullOrEmpty(CCEmailID))
            //                    mm.CC.Add(CCEmailID);

            //                BCCAddress = System.Configuration.ConfigurationManager.AppSettings["BCCAddress"];

            //                if (!string.IsNullOrEmpty(BCCAddress))
            //                    mm.Bcc.Add(BCCAddress);

            //                //if (_Attachments != null && _Attachments.Count > 0)
            //                //{
            //                //    HasAttachment = 1;
            //                //    for (int i = 0; i < _Attachments.Count; i++)
            //                //    {
            //                //        mm.Attachments.Add((Attachment)_Attachments[i]);
            //                //    }
            //                //}
            //                //smtp = new SmtpClient();
            //                //smtp.EnableSsl = true;
            //                //smtp.Timeout = 30000;
            //                ////System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            //                ////{ return true; };
            //                //smtp.Send(mm);
            //                using (SmtpClient obj = new SmtpClient())
            //                {
            //                    obj.EnableSsl = true;
            //                    obj.Timeout = 300000;
            //                    obj.Send(mm);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                if (System.Configuration.ConfigurationManager.AppSettings["EmailLoggin"] == "yes")
            //                {
            //                    string query = @" INSERT INTO [eTurnsMaster].[dbo].[eMailExceptionLogs]
            //                                            ([ToAddress]           ,[CCAddress]          ,[BCCAddress]           ,[Subject]
            //                                            ,[MailBody]	          ,[ErrorMessage]	     ,[FullException]        ,[EnterpriseID]
            //                                            ,[CompanyID]           ,[RoomID]		     ,[CreatedOn]	         ,[CallFrom]
            //                                            ,[UserID]	          ,[Remarks]             ,[HasAttachment]) 
            //                                    Values ('" + ToEmailID + @"','" + CCEmailID + @"','" + BCCAddress + @"','" + Subject
            //                                            + @"','" + Body.ToString().Replace("'", "''") + @"','" + ex.Message.Replace("'", "''") + @"','" + ex.ToString().Replace("'", "''") + @"'," + EnterpriseID + @","
            //                                            + CompanyID + @"," + RoomID + @",getutcdate(),'" + CallFrom + @"',"
            //                                            + UserID + @",'" + string.Empty + "'," + HasAttachment + ");";

            //                    cn = new System.Data.SqlClient.SqlConnection();
            //                    cn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString;
            //                    cmd = new System.Data.SqlClient.SqlCommand();
            //                    cmd.CommandText = query;
            //                    cmd.CommandType = System.Data.CommandType.Text;
            //                    cmd.Connection = cn;
            //                    cn.Open();
            //                    cmd.ExecuteNonQuery();
            //                    cn.Close();
            //                }
            //                throw ex;
            //            }

            //            finally
            //            {
            //                foreach (Attachment attachment in mm.Attachments)
            //                {
            //                    attachment.Dispose();
            //                }

            //                if (_Attachments != null)
            //                    _Attachments.Clear();

            //                if (mm != null)
            //                    mm.Dispose();

            //                if (smtp != null)
            //                    smtp.Dispose();

            //                mm = null;
            //                smtp = null;
            //                _Attachments = null;

            //                if (cmd != null)
            //                    cmd.Dispose();


            //                if (cn != null)
            //                {
            //                    cn.Close();
            //                    cn.Dispose();
            //                }
            //                cn = null;
            //                cmd = null;

            //            }
        }

        public string GeteTurnsImage(string path, string imagePath)
        {
            string str = string.Empty;

            str = @"<a href='" + path + @"' title=""E Turns Powered""> <img alt=""E Turns Powered"" src='" + (path + imagePath) + @"' style=""border: 0px currentColor; border-image: none;"" /></a>";
            return str;
        }

    }

    public class BaseResourceDTO
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Language { get; set; }
        public string Module { get; set; }

        public string Page { get; set; }
    }

    public class SupplierPODTO
    {
        public long SupplierID { get; set; }

        public string SupplierName { get; set; }

        public string PONumber { get; set; }
    }

    public class ItemBinChangeHistory
    {
        public long ID { get; set; }
        public long BinID { get; set; }

        public string BinNumber { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }

        public Guid? ItemGUID { get; set; }

        public string ItemNumber { get; set; }

        public DateTime Created { get; set; }

        public long CreatedBy { get; set; }
    }

    public class AlleTurnsActionMethodsDTO
    {
        public long ID { get; set; }
        public string ActionMethod { get; set; }
        public string Controller { get; set; }
        public string Module { get; set; }
        public bool? IsView { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsInsert { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public bool? ShowDeleted { get; set; }
        public bool? ShowArchived { get; set; }
        public bool? ShowUDF { get; set; }
        public bool? ShowChangeLog { get; set; }

        public long? PermissionModuleID { get; set; }

        public string Attributes { get; set; }
    }


    public class AllIntegrateAPIActionMethodsDTO
    {
        public long ID { get; set; }
        public string ActionMethod { get; set; }
        public string Controller { get; set; }
        public string Module { get; set; }
        public bool? IsView { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsInsert { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public bool? ShowDeleted { get; set; }
        public bool? ShowArchived { get; set; }
        public bool? ShowUDF { get; set; }
        public bool? ShowChangeLog { get; set; }

        public long? PermissionModuleID { get; set; }

        public string Attributes { get; set; }
    }

    public class EntCmpRmInfo
    {
        public string EnterpriseName { get; set; }
        public long? EnterpriseID { get; set; }
        public string CompanyNames { get; set; }
        public string RoomNames { get; set; }
        public string CompanyIDs { get; set; }
        public string RoomIDs { get; set; }

    }

    public class ReportSchedulerErrorDTO
    {
        public Int64 CompanyID { get; set; }
        public Int64 EnterpriseID { get; set; }
        public string Exception { get; set; }
        public Int64 ID { get; set; }
        public Int64 NotificationID { get; set; }
        public Int64 RoomID { get; set; }
        public int ScheduleFor { get; set; }

        public Int64 UserID { get; set; }
    }

    public class NarrowSearchFieldDTO
    {
        public string NSFieldName { get; set; }
        public long ID { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
    }

    public class TareNarrowSearchData
    {
      public List<NarrowSearchFieldDTO> CreatedByList { get; set; }
        public List<NarrowSearchFieldDTO> UpdatedByList { get; set; }
    }
    public enum PullInsertTypeEnum
    {
        WebNewPull = 1,
        WebWo = 2,
        WebReq = 3,
        WebAdjustCount = 4,
        WebManualCount = 5,

        WebImportPull = 6,
        WebImportPullHistory = 7,
        WebImportPullWithSerial = 8,
        WebImportAdjustCount = 9,
        WebImportManualCount = 10,

        SVCInsertPull = 11,
        SVCInsertWorkOrderWithImage = 12,
        SVCInsertCount = 13,
        SVCUpdateCount = 14,
        SVCSaveApplyCountLineItem = 15,
        SVCPullRequisitionItem = 16,
        OnBinDelete = 17
    }

    public enum TransactionConversionType
    {
        CarttoOrder = 1,
        CarttoTransfer = 2,
        CarttoReturn = 3,
        CarttoQuote = 4,
        QuotetoOrder = 5
    }
    public class UDFRequest
    {
        public string TableName { get; set; }
        public string UDFUniqueID { get; set; }
        public string UDFName { get; set; }
    }
}
