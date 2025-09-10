using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class SiteSettingInfo
    {

        public string LoginWithSelection { get; set; }
        public string ChartLabelCharSize { get; set; }
        public string DelayRedCount { get; set; }
        public string EnableOptimizations { get; set; }
        public string ItemDetailsNew { get; set; }
        public string AcceptLicence { get; set; }
        public string ILQImportNew { get; set; }
        public string RoleCodeImprove { get; set; }
        public string ItemDetailsChange { get; set; }
        public string OldResultView { get; set; }
        public string OldUserMasterUDF { get; set; }
        public string ReleaseNumber { get; set; }
        public string OrderNumberDateFormat { get; set; }
        public string NoImage { get; set; }
        public string InventoryPhoto { get; set; }
        public string AssetPhoto { get; set; }
        public string ToolPhoto { get; set; }
        public string InventoryLink2 { get; set; }
        public string SupplierPhoto { get; set; }
        public string WorkOrderFilePaths { get; set; }
        public string OrderFilePaths { get; set; }
        public string AllowedCharacter { get; set; }
        public string XMLFilePath { get; set; }
        public string ProjectSpendLimitPercentage { get; set; }
        public string EnableNewCountPopup { get; set; }
        public string ApplyChangePassword { get; set; }
        public string NewBaseResourceFileName { get; set; }
        public string BOMInventoryPhoto { get; set; }
        public string BOMInventoryLink2 { get; set; }
        public string ItemBinBarcodeNewPage { get; set; }
        public string UDFMaxLength { get; set; }
        public string ExceptActionsForNoRoom { get; set; }
        public string AccessQryUserNames { get; set; }
        public string AccessQryRoleIds { get; set; }
        public string RunWithReportConnection { get; set; }
        public string ApplyNewAuthorization { get; set; }
        public string ValidateAntiForgeryTokenOnAllPosts { get; set; }
        public string MethodsToIgnoreXSRF { get; set; }
        public string SaveNewAuthorizationError { get; set; }
        public string SaveAntiForgeryError { get; set; }
        public string IsShowGlobalReprotBuilder { get; set; }
        public string CustomShortDatePatterns { get; set; }
        public string MethodsToCheckIsInsert { get; set; }
        public string IsNewNotification { get; set; }
        public string ControllerName { get; set; }
        public string ModuleName { get; set; }
        public string ModuleMasterName { get; set; }
        public string AllowEnterpriseRoomForNN { get; set; }
        public string LoadEnterpriseGridOrdering { get; set; }
        public string ValidatePhoneNumber { get; set; }
        public string ProductEnvironment { get; set; }
        public string OnlyIfRoomAvailable { get; set; }
        public string decimalPointFromConfig { get; set; }
        public string ForWardDatesEnterpriseID { get; set; }
        public string NewRangeDataFill { get; set; }
        public string LoadNarrowDataCount { get; set; }
        public string DisplayInventoryValueforReplenish { get; set; }
        public string RoomReportGrid { get; set; }
        public string RoomReportGridCompanyID { get; set; }
        public string RoomReportGridRoomID { get; set; }
        public string AllowedPullQtyEdit { get; set; }
        public string WorkOrderAllowedFileExtension { get; set; }
        public string OrderAllowedFileExtension { get; set; }

        public string ResourceSave { get; set; }
        public string ResourceRead { get; set; }
        public string EnterpriseIdsToShowReleaseNo { get; set; }
        public string eVMIRooms { get; set; }
        public string QBRooms { get; set; }
        public string QBClientID { get; set; }
        public string QBClientSecret { get; set; }
        public string QBEnvironment { get; set; }
        public string QBRedirectUrl { get; set; }
        public string QBDiscoveryUrl { get; set; }
        public string QBVersion { get; set; }
        public string NonRedirectablUrls { get; set; }
        public eVMISiteSettings eVMISiteSettings { get; set; }
        public string HelpDocumentFolderPath { get; set; }
        public string QuoteNumberDateFormat { get; set; }
        public string UsersUISettingType { get; set; }
        public string DomainProtocol { get; set; }
        public string ABOrderSyncMode { get; set; }
        public string EnforceResourcePagesRestriction { get; set; }
        public string AllowedUserIdsResources { get; set; }
        public string AllowedEntIdsResources { get; set; }
        public string StrictRediretion { get; set; }
        public string RequisitionFilePaths { get; set; }
        public string RequisitionAllowedFileExtension { get; set; }
        public string LogResourceCalls { get; set; }
        public string ReceiveFilePaths { get; set; }
        public string ReceiveAllowedFileExtension { get; set; }
        public string CommonAllowedFileExtension { get; set; }
        public string Link2AllowedFileExtension { get; set; }
        public string MailOrderFieldRoomRestriction { get; set; }
        public string IsOktaEnable { get; set; }
        public string EmptyHeaderCheck { get; set; }
        public string IsBorderStateURL { get; set; }
        public string BorderStateDesignCssPostFix { get; set; }
        public string OktaServiceUrl { get; set; }
        public string OktaReturnUrl { get; set; }
        public string EnableOktaLoginForSpecialUrls { get; set; }
        public string EnableBorderStateID { get; set; }
        public string RestrictNewUserEMailEnterpriseIds { get; set; }
        public string IgnoreLatestCode { get; set; }

    }

    public class eVMISiteSettings
    {
        public const string ImmediateVal = "Immediate";
        public const string QueuedVal = "Queued";
        public const string PollCommand_W = "W";
        public const string PollCommand_H = "H";

        #region Config JSON Properties

        string _GetVersionRequestType;

        [Display(Name = "Get Version Request Type")]
        public string GetFirmWareVersionRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_GetVersionRequestType))
                {
                    _GetVersionRequestType = ImmediateVal;
                }
                return _GetVersionRequestType;
            }
            set
            {
                _GetVersionRequestType = value;
            }
        }

        string _GetSerialNoRequestType;

        [Display(Name = "Get Serial No Request Type")]
        public string GetSerialNoRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_GetSerialNoRequestType))
                {
                    _GetSerialNoRequestType = ImmediateVal;
                }
                return _GetSerialNoRequestType;
            }
            set
            {
                _GetSerialNoRequestType = value;
            }
        }


        string _GetModelRequestType;
        [Display(Name = "Get Model Request Type")]
        public string GetModelRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_GetModelRequestType))
                {
                    _GetModelRequestType = ImmediateVal;
                }
                return _GetModelRequestType;
            }
            set
            {
                _GetModelRequestType = value;
            }
        }

        string _SetModelRequestType;
        [Display(Name = "Set Model Request Type")]
        public string SetModelRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SetModelRequestType))
                {
                    _SetModelRequestType = ImmediateVal;
                }
                return _SetModelRequestType;
            }
            set
            {
                _SetModelRequestType = value;
            }
        }

        string _GetShelfIDRequestType;
        [Display(Name = "Get Shelf ID Request Type")]
        public string GetShelfIDRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_GetShelfIDRequestType))
                {
                    _GetShelfIDRequestType = ImmediateVal;
                }
                return _GetShelfIDRequestType;
            }
            set
            {
                _GetShelfIDRequestType = value;
            }
        }

        string _SetShelfIDRequestType;
        [Display(Name = "Set Shelf ID Request Type")]
        public string SetShelfIDRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SetShelfIDRequestType))
                {
                    _SetShelfIDRequestType = ImmediateVal;
                }
                return _SetShelfIDRequestType;
            }
            set
            {
                _SetShelfIDRequestType = value;
            }
        }

        string _GetCalibrationWeightRequestType;
        [Display(Name = "Get Calibration Weight Request Type")]
        public string GetCalibrationWeightRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_GetCalibrationWeightRequestType))
                {
                    _GetCalibrationWeightRequestType = ImmediateVal;
                }
                return _GetCalibrationWeightRequestType;
            }
            set
            {
                _GetCalibrationWeightRequestType = value;
            }
        }

        string _SetCalibrationWeightRequestType;
        [Display(Name = "Set Calibration Weight Request Type")]
        public string SetCalibrationWeightRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SetCalibrationWeightRequestType))
                {
                    _SetCalibrationWeightRequestType = ImmediateVal;
                }
                return _SetCalibrationWeightRequestType;
            }
            set
            {
                _SetCalibrationWeightRequestType = value;
            }
        }

        string _CalibrateRequestType;

        [Display(Name = "Calibrate Request Type")]
        public string CalibrateRequestType
        {
            get
            {
                //if (string.IsNullOrWhiteSpace(_CalibrateRequestType))
                {
                    _CalibrateRequestType = ImmediateVal;
                }
                return _CalibrateRequestType;
            }
            set
            {
                _CalibrateRequestType = value;
            }
        }

        string _PollRequestType;

        [Display(Name = "Poll Request Type")]
        public string PollRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_PollRequestType))
                {
                    _PollRequestType = ImmediateVal;
                }
                return _PollRequestType;
            }
            set
            {
                _PollRequestType = value;
            }
        }


        string _TareRequestType;
        [Display(Name = "Tare Request Type")]
        public string TareRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_TareRequestType))
                {
                    _TareRequestType = ImmediateVal;
                }
                return _TareRequestType;
            }
            set
            {
                _TareRequestType = value;
            }
        }

        string _ItemWeightPerPieceRequestType;
        [Display(Name = "Item Weight Per Piece Request Type")]
        public string ItemWeightPerPieceRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_ItemWeightPerPieceRequestType))
                {
                    _ItemWeightPerPieceRequestType = ImmediateVal;
                }
                return _ItemWeightPerPieceRequestType;
            }
            set
            {
                _ItemWeightPerPieceRequestType = value;
            }
        }


        string _ResetRequestType;

        [Display(Name = "Reset Request Type")]
        public string ResetRequestType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_ResetRequestType))
                {
                    _ResetRequestType = ImmediateVal;
                }
                return _ResetRequestType;
            }
            set
            {
                _ResetRequestType = value;
            }
        }

        string _PollCommand;

        [Display(Name = "Poll Command")]
        public string PollCommand
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_PollCommand))
                {
                    _PollCommand = PollCommand_W;
                }
                return _PollCommand;
            }
            set
            {
                _PollCommand = value;
            }
        }

        #endregion

        #region Get Is Immediate Immediate functions

        bool IsImmediateRequest(string reuqestType)
        {
            bool b = reuqestType == ImmediateVal;
            return b;
        }


        public bool IsGetVersionRequestImmediate()
        {
            return IsImmediateRequest(this.GetFirmWareVersionRequestType);
        }


        public bool IsGetSerialNoRequestImmediate()
        {
            return IsImmediateRequest(this.GetSerialNoRequestType);
        }


        public bool IsGetModelRequestImmediate()
        {
            return IsImmediateRequest(this.GetModelRequestType);
        }

        public bool IsSetModelRequestImmediate()
        {
            return IsImmediateRequest(this.SetModelRequestType);
        }


        public bool IsGetCalibrationWeightRequestImmediate()
        {
            return IsImmediateRequest(this.GetCalibrationWeightRequestType);
        }

        public bool IsSetCalibrationWeightRequestImmediate()
        {
            return IsImmediateRequest(this.SetCalibrationWeightRequestType);
        }

        public bool IsPollRequestImmediate()
        {
            return IsImmediateRequest(this.PollRequestType);
        }

        public bool IsTareRequestImmediate()
        {
            return IsImmediateRequest(this.TareRequestType);
        }

        public bool IsItemWeightPerPieceRequestImmediate()
        {
            return IsImmediateRequest(this.ItemWeightPerPieceRequestType);
        }

        public bool IsResetRequestImmediate()
        {
            return IsImmediateRequest(this.ResetRequestType);
        }

        public bool IsGetShelfIDRequestImmediate()
        {
            return IsImmediateRequest(this.GetShelfIDRequestType);
        }

        public bool IsSetShelfIDRequestImmediate()
        {
            return IsImmediateRequest(this.SetShelfIDRequestType);
        }

        public bool IsCalibrateRequestImmediate()
        {
            return IsImmediateRequest(this.CalibrateRequestType);
        }

        public string GetPollCommandType()
        {
            return this.PollCommand;
        }
        public long ID { get; set; }
        public long RoomID { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public Guid Guid { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public long? CreatedBy { get; set; }
        public long? LastUpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        #endregion
    }

    public static class SiteSettingHelper
    {
        static SiteSettingHelper()
        {
            SiteSettingInfo objSiteSetting = new SiteSettingInfo();
            //CommonDAL objCommanDAL = new CommonDAL(DbConnectionHelper.GetETurnsMasterDBName());
            SiteSettingInfoHelper obj = new SiteSettingInfoHelper();
            //objSiteSetting = objCommanDAL.GetSiteSettingInfo();
            objSiteSetting = obj.GetSiteSettingInfo();
            if (objSiteSetting != null)
            {
                _LoginWithSelection = objSiteSetting.LoginWithSelection;
                _ChartLabelCharSize = objSiteSetting.ChartLabelCharSize;
                _DelayRedCount = objSiteSetting.DelayRedCount;
                _EnableOptimizations = objSiteSetting.EnableOptimizations;
                _ItemDetailsNew = objSiteSetting.ItemDetailsNew;
                _AcceptLicence = objSiteSetting.AcceptLicence;
                _ILQImportNew = objSiteSetting.ILQImportNew;
                _RoleCodeImprove = objSiteSetting.RoleCodeImprove;
                _ItemDetailsChange = objSiteSetting.ItemDetailsChange;
                _OldResultView = objSiteSetting.OldResultView;
                _OldUserMasterUDF = objSiteSetting.OldUserMasterUDF;
                _ReleaseNumber = objSiteSetting.ReleaseNumber;
                _OrderNumberDateFormat = objSiteSetting.OrderNumberDateFormat;
                _NoImage = objSiteSetting.NoImage;
                _InventoryPhoto = objSiteSetting.InventoryPhoto;
                _AssetPhoto = objSiteSetting.AssetPhoto;
                _ToolPhoto = objSiteSetting.ToolPhoto;
                _InventoryLink2 = objSiteSetting.InventoryLink2;
                _SupplierPhoto = objSiteSetting.SupplierPhoto;
                _WorkOrderFilePaths = objSiteSetting.WorkOrderFilePaths;
                _OrderFilePaths = objSiteSetting.OrderFilePaths;
                _AllowedCharacter = objSiteSetting.AllowedCharacter;
                _XMLFilePath = objSiteSetting.XMLFilePath;
                _ProjectSpendLimitPercentage = objSiteSetting.ProjectSpendLimitPercentage;
                _EnableNewCountPopup = objSiteSetting.EnableNewCountPopup;
                _ApplyChangePassword = objSiteSetting.ApplyChangePassword;
                _NewBaseResourceFileName = objSiteSetting.NewBaseResourceFileName;
                _BOMInventoryPhoto = objSiteSetting.BOMInventoryPhoto;
                _BOMInventoryLink2 = objSiteSetting.BOMInventoryLink2;
                _ItemBinBarcodeNewPage = objSiteSetting.ItemBinBarcodeNewPage;
                _UDFMaxLength = objSiteSetting.UDFMaxLength;
                _ExceptActionsForNoRoom = objSiteSetting.ExceptActionsForNoRoom;
                _AccessQryUserNames = objSiteSetting.AccessQryUserNames;
                _AccessQryRoleIds = objSiteSetting.AccessQryRoleIds;
                _RunWithReportConnection = objSiteSetting.RunWithReportConnection;
                _ApplyNewAuthorization = objSiteSetting.ApplyNewAuthorization;
                _ValidateAntiForgeryTokenOnAllPosts = objSiteSetting.ValidateAntiForgeryTokenOnAllPosts;
                _MethodsToIgnoreXSRF = objSiteSetting.MethodsToIgnoreXSRF;
                _SaveNewAuthorizationError = objSiteSetting.SaveNewAuthorizationError;
                _SaveAntiForgeryError = objSiteSetting.SaveAntiForgeryError;
                _IsShowGlobalReprotBuilder = objSiteSetting.IsShowGlobalReprotBuilder;
                _CustomShortDatePatterns = objSiteSetting.CustomShortDatePatterns;
                _MethodsToCheckIsInsert = objSiteSetting.MethodsToCheckIsInsert;
                _IsNewNotification = objSiteSetting.IsNewNotification;
                _ControllerName = objSiteSetting.ControllerName;
                _ModuleName = objSiteSetting.ModuleName;
                _ModuleMasterName = objSiteSetting.ModuleMasterName;
                _AllowEnterpriseRoomForNN = objSiteSetting.AllowEnterpriseRoomForNN;
                _LoadEnterpriseGridOrdering = objSiteSetting.LoadEnterpriseGridOrdering;
                _ValidatePhoneNumber = objSiteSetting.ValidatePhoneNumber;
                _ProductEnvironment = objSiteSetting.ProductEnvironment;
                _OnlyIfRoomAvailable = objSiteSetting.OnlyIfRoomAvailable;
                _decimalPointFromConfig = objSiteSetting.decimalPointFromConfig;
                _ForWardDatesEnterpriseID = objSiteSetting.ForWardDatesEnterpriseID;
                _NewRangeDataFill = objSiteSetting.NewRangeDataFill;
                _LoadNarrowDataCount = objSiteSetting.LoadNarrowDataCount;
                _DisplayInventoryValueforReplenish = objSiteSetting.DisplayInventoryValueforReplenish;
                _RoomReportGrid = objSiteSetting.RoomReportGrid;
                _RoomReportGridCompanyID = objSiteSetting.RoomReportGridCompanyID;
                _RoomReportGridRoomID = objSiteSetting.RoomReportGridRoomID;
                _AllowedPullQtyEdit = objSiteSetting.AllowedPullQtyEdit;
                _WorkOrderAllowedFileExtension = objSiteSetting.WorkOrderAllowedFileExtension;
                _OrderAllowedFileExtension = objSiteSetting.OrderAllowedFileExtension;
                _ResourceRead = objSiteSetting.ResourceRead;
                _ResourceSave = objSiteSetting.ResourceSave;
                _EnterpriseIdsToShowReleaseNo = objSiteSetting.EnterpriseIdsToShowReleaseNo;
                _eVMIRooms = objSiteSetting.eVMIRooms;
                _QBRooms = objSiteSetting.QBRooms;
                _QBClientID = objSiteSetting.QBClientID;
                _QBClientSecret = objSiteSetting.QBClientSecret;
                _QBEnvironment = objSiteSetting.QBEnvironment;
                _QBRedirectUrl = objSiteSetting.QBRedirectUrl;
                _QBDiscoveryUrl = objSiteSetting.QBDiscoveryUrl;
                _QBVersion = objSiteSetting.QBVersion;
                _NonRedirectablUrls = objSiteSetting.NonRedirectablUrls;
                _eVMISiteSettings = objSiteSetting.eVMISiteSettings;
                _HelpDocumentFolderPath = objSiteSetting.HelpDocumentFolderPath;
                _QuoteNumberDateFormat = objSiteSetting.QuoteNumberDateFormat;
                _UsersUISettingType = objSiteSetting.UsersUISettingType;
                _DomainProtocol = objSiteSetting.DomainProtocol;
                _ABOrderSyncMode = objSiteSetting.ABOrderSyncMode;
                _EnforceResourcePagesRestriction = objSiteSetting.EnforceResourcePagesRestriction;
                _AllowedUserIdsResources = objSiteSetting.AllowedUserIdsResources;
                _AllowedEntIdsResources = objSiteSetting.AllowedEntIdsResources;
                _StrictRediretion = objSiteSetting.StrictRediretion;
                _RequisitionFilePaths = objSiteSetting.RequisitionFilePaths;
                _RequisitionAllowedFileExtension = objSiteSetting.RequisitionAllowedFileExtension;
                _LogResourceCalls = objSiteSetting.LogResourceCalls;
                _ReceiveFilePaths = objSiteSetting.ReceiveFilePaths;
                _ReceiveAllowedFileExtension = objSiteSetting.ReceiveAllowedFileExtension;
                _CommonAllowedFileExtension = objSiteSetting.CommonAllowedFileExtension;
                _Link2AllowedFileExtension = objSiteSetting.Link2AllowedFileExtension;
                _MailOrderFieldRoomRestriction = objSiteSetting.MailOrderFieldRoomRestriction;
                _IsOktaEnable = objSiteSetting.IsOktaEnable;
                _EmptyHeaderCheck = objSiteSetting.EmptyHeaderCheck;
                _IsBorderStateURL = objSiteSetting.IsBorderStateURL;
                _BorderStateDesignCssPostFix = objSiteSetting.BorderStateDesignCssPostFix;
                _OktaServiceUrl = objSiteSetting.OktaServiceUrl;
                _OktaReturnUrl = objSiteSetting.OktaReturnUrl;
                _EnableOktaLoginForSpecialUrls = objSiteSetting.EnableOktaLoginForSpecialUrls;
                _EnableBorderStateID = objSiteSetting.EnableBorderStateID;
                _RestrictNewUserEMailEnterpriseIds = objSiteSetting.RestrictNewUserEMailEnterpriseIds;
                _IgnoreLatestCode = objSiteSetting.IgnoreLatestCode;
            }

        }

        private static string _LoginWithSelection;
        public static string LoginWithSelection
        {
            get { return _LoginWithSelection; }
            set { _LoginWithSelection = value; }
        }

        private static string _ChartLabelCharSize;
        public static string ChartLabelCharSize
        {
            get { return _ChartLabelCharSize; }
            set { _ChartLabelCharSize = value; }
        }

        private static string _DelayRedCount;
        public static string DelayRedCount
        {
            get { return _DelayRedCount; }
            set { _DelayRedCount = value; }
        }

        private static string _EnableOptimizations;
        public static string EnableOptimizations
        {
            get { return _EnableOptimizations; }
            set { _EnableOptimizations = value; }
        }

        private static string _ItemDetailsNew;
        public static string ItemDetailsNew
        {
            get { return _ItemDetailsNew; }
            set { _ItemDetailsNew = value; }
        }

        private static string _AcceptLicence;
        public static string AcceptLicence
        {
            get { return _AcceptLicence; }
            set { _AcceptLicence = value; }
        }

        private static string _ILQImportNew;
        public static string ILQImportNew
        {
            get { return _ILQImportNew; }
            set { _ILQImportNew = value; }
        }

        private static string _RoleCodeImprove;
        public static string RoleCodeImprove
        {
            get { return _RoleCodeImprove; }
            set { _RoleCodeImprove = value; }
        }

        private static string _ItemDetailsChange;
        public static string ItemDetailsChange
        {
            get { return _ItemDetailsChange; }
            set { _ItemDetailsChange = value; }
        }

        private static string _OldResultView;
        public static string OldResultView
        {
            get { return _OldResultView; }
            set { _OldResultView = value; }
        }

        private static string _OldUserMasterUDF;
        public static string OldUserMasterUDF
        {
            get { return _OldUserMasterUDF; }
            set { _OldUserMasterUDF = value; }
        }

        private static string _ReleaseNumber;
        public static string ReleaseNumber
        {
            get { return _ReleaseNumber; }
            set { _ReleaseNumber = value; }
        }

        private static string _OrderNumberDateFormat;
        public static string OrderNumberDateFormat
        {
            get { return _OrderNumberDateFormat; }
            set { _OrderNumberDateFormat = value; }
        }
        private static string _QuoteNumberDateFormat;
        public static string QuoteNumberDateFormat
        {
            get { return _QuoteNumberDateFormat; }
            set { _QuoteNumberDateFormat = value; }
        }
        private static string _NoImage;
        public static string NoImage
        {
            get { return _NoImage; }
            set { _NoImage = value; }
        }

        private static string _InventoryPhoto;
        public static string InventoryPhoto
        {
            get { return _InventoryPhoto; }
            set { _InventoryPhoto = value; }
        }

        private static string _AssetPhoto;
        public static string AssetPhoto
        {
            get { return _AssetPhoto; }
            set { _AssetPhoto = value; }
        }

        private static string _ToolPhoto;
        public static string ToolPhoto
        {
            get { return _ToolPhoto; }
            set { _ToolPhoto = value; }
        }

        private static string _InventoryLink2;
        public static string InventoryLink2
        {
            get { return _InventoryLink2; }
            set { _InventoryLink2 = value; }
        }
        private static string _HelpDocumentFolderPath;
        public static string HelpDocumentFolderPath
        {
            get { return _HelpDocumentFolderPath; }
            set { _HelpDocumentFolderPath = value; }
        }
        private static string _SupplierPhoto;
        public static string SupplierPhoto
        {
            get { return _SupplierPhoto; }
            set { _SupplierPhoto = value; }
        }

        private static string _WorkOrderFilePaths;
        public static string WorkOrderFilePaths
        {
            get { return _WorkOrderFilePaths; }
            set { _WorkOrderFilePaths = value; }
        }

        private static string _AllowedCharacter;
        public static string AllowedCharacter
        {
            get { return _AllowedCharacter; }
            set { _AllowedCharacter = value; }
        }

        private static string _XMLFilePath;
        public static string XMLFilePath
        {
            get { return _XMLFilePath; }
            set { _XMLFilePath = value; }
        }

        private static string _ProjectSpendLimitPercentage;
        public static string ProjectSpendLimitPercentage
        {
            get { return _ProjectSpendLimitPercentage; }
            set { _ProjectSpendLimitPercentage = value; }
        }

        private static string _EnableNewCountPopup;
        public static string EnableNewCountPopup
        {
            get { return _EnableNewCountPopup; }
            set { _EnableNewCountPopup = value; }
        }

        private static string _ApplyChangePassword;
        public static string ApplyChangePassword
        {
            get { return _ApplyChangePassword; }
            set { _ApplyChangePassword = value; }
        }

        private static string _NewBaseResourceFileName;
        public static string NewBaseResourceFileName
        {
            get { return _NewBaseResourceFileName; }
            set { _NewBaseResourceFileName = value; }
        }

        private static string _BOMInventoryPhoto;
        public static string BOMInventoryPhoto
        {
            get { return _BOMInventoryPhoto; }
            set { _BOMInventoryPhoto = value; }
        }

        private static string _BOMInventoryLink2;
        public static string BOMInventoryLink2
        {
            get { return _BOMInventoryLink2; }
            set { _BOMInventoryLink2 = value; }
        }

        private static string _ItemBinBarcodeNewPage;
        public static string ItemBinBarcodeNewPage
        {
            get { return _ItemBinBarcodeNewPage; }
            set { _ItemBinBarcodeNewPage = value; }
        }

        private static string _UDFMaxLength;
        public static string UDFMaxLength
        {
            get { return _UDFMaxLength; }
            set { _UDFMaxLength = value; }
        }

        private static string _ExceptActionsForNoRoom;
        public static string ExceptActionsForNoRoom
        {
            get { return _ExceptActionsForNoRoom; }
            set { _ExceptActionsForNoRoom = value; }
        }

        private static string _AccessQryUserNames;
        public static string AccessQryUserNames
        {
            get { return _AccessQryUserNames; }
            set { _AccessQryUserNames = value; }
        }

        private static string _AccessQryRoleIds;
        public static string AccessQryRoleIds
        {
            get { return _AccessQryRoleIds; }
            set { _AccessQryRoleIds = value; }
        }

        private static string _RunWithReportConnection;
        public static string RunWithReportConnection
        {
            get { return _RunWithReportConnection; }
            set { _RunWithReportConnection = value; }
        }

        private static string _ApplyNewAuthorization;
        public static string ApplyNewAuthorization
        {
            get { return _ApplyNewAuthorization; }
            set { _ApplyNewAuthorization = value; }
        }

        private static string _ValidateAntiForgeryTokenOnAllPosts;
        public static string ValidateAntiForgeryTokenOnAllPosts
        {
            get { return _ValidateAntiForgeryTokenOnAllPosts; }
            set { _ValidateAntiForgeryTokenOnAllPosts = value; }
        }

        private static string _MethodsToIgnoreXSRF;
        public static string MethodsToIgnoreXSRF
        {
            get { return _MethodsToIgnoreXSRF; }
            set { _MethodsToIgnoreXSRF = value; }
        }

        private static string _SaveNewAuthorizationError;
        public static string SaveNewAuthorizationError
        {
            get { return _SaveNewAuthorizationError; }
            set { _SaveNewAuthorizationError = value; }
        }

        private static string _SaveAntiForgeryError;
        public static string SaveAntiForgeryError
        {
            get { return _SaveAntiForgeryError; }
            set { _SaveAntiForgeryError = value; }
        }

        private static string _IsShowGlobalReprotBuilder;
        public static string IsShowGlobalReprotBuilder
        {
            get { return _IsShowGlobalReprotBuilder; }
            set { _IsShowGlobalReprotBuilder = value; }
        }

        private static string _CustomShortDatePatterns;
        public static string CustomShortDatePatterns
        {
            get { return _CustomShortDatePatterns; }
            set { _CustomShortDatePatterns = value; }
        }

        private static string _MethodsToCheckIsInsert;
        public static string MethodsToCheckIsInsert
        {
            get { return _MethodsToCheckIsInsert; }
            set { _MethodsToCheckIsInsert = value; }
        }

        private static string _IsNewNotification;
        public static string IsNewNotification
        {
            get { return _IsNewNotification; }
            set { _IsNewNotification = value; }
        }

        private static string _ControllerName;
        public static string ControllerName
        {
            get { return _ControllerName; }
            set { _ControllerName = value; }
        }

        private static string _ModuleName;
        public static string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value; }
        }

        private static string _ModuleMasterName;
        public static string ModuleMasterName
        {
            get { return _ModuleMasterName; }
            set { _ModuleMasterName = value; }
        }

        private static string _AllowEnterpriseRoomForNN;
        public static string AllowEnterpriseRoomForNN
        {
            get { return _AllowEnterpriseRoomForNN; }
            set { _AllowEnterpriseRoomForNN = value; }
        }

        private static string _LoadEnterpriseGridOrdering;
        public static string LoadEnterpriseGridOrdering
        {
            get { return _LoadEnterpriseGridOrdering; }
            set { _LoadEnterpriseGridOrdering = value; }
        }

        private static string _ValidatePhoneNumber;
        public static string ValidatePhoneNumber
        {
            get { return _ValidatePhoneNumber; }
            set { _ValidatePhoneNumber = value; }
        }

        private static string _ProductEnvironment;
        public static string ProductEnvironment
        {
            get { return _ProductEnvironment; }
            set { _ProductEnvironment = value; }
        }

        private static string _OnlyIfRoomAvailable;
        public static string OnlyIfRoomAvailable
        {
            get { return _OnlyIfRoomAvailable; }
            set { _OnlyIfRoomAvailable = value; }
        }

        private static string _decimalPointFromConfig;
        public static string decimalPointFromConfig
        {
            get { return _decimalPointFromConfig; }
            set { _decimalPointFromConfig = value; }
        }

        private static string _ForWardDatesEnterpriseID;
        public static string ForWardDatesEnterpriseID
        {
            get { return _ForWardDatesEnterpriseID; }
            set { _ForWardDatesEnterpriseID = value; }
        }

        private static string _NewRangeDataFill;
        public static string NewRangeDataFill
        {
            get { return _NewRangeDataFill; }
            set { _NewRangeDataFill = value; }
        }

        private static string _LoadNarrowDataCount;
        public static string LoadNarrowDataCount
        {
            get { return _LoadNarrowDataCount; }
            set { _LoadNarrowDataCount = value; }
        }

        private static string _DisplayInventoryValueforReplenish;
        public static string DisplayInventoryValueforReplenish
        {
            get { return _DisplayInventoryValueforReplenish; }
            set { _DisplayInventoryValueforReplenish = value; }
        }

        private static string _RoomReportGrid;
        public static string RoomReportGrid
        {
            get { return _RoomReportGrid; }
            set { _RoomReportGrid = value; }
        }

        private static string _RoomReportGridCompanyID;
        public static string RoomReportGridCompanyID
        {
            get { return _RoomReportGridCompanyID; }
            set { _RoomReportGridCompanyID = value; }
        }

        private static string _RoomReportGridRoomID;
        public static string RoomReportGridRoomID
        {
            get { return _RoomReportGridRoomID; }
            set { _RoomReportGridRoomID = value; }
        }

        private static string _AllowedPullQtyEdit;
        public static string AllowedPullQtyEdit
        {
            get { return _AllowedPullQtyEdit; }
            set { _AllowedPullQtyEdit = value; }
        }

        private static string _WorkOrderAllowedFileExtension;
        public static string WorkOrderAllowedFileExtension
        {
            get { return _WorkOrderAllowedFileExtension; }
            set { _WorkOrderAllowedFileExtension = value; }
        }

        private static string _ResourceSave;
        public static string ResourceSave
        {
            get { return _ResourceSave; }
            set { _ResourceSave = value; }
        }

        private static string _ResourceRead;
        public static string ResourceRead
        {
            get { return _ResourceRead; }
            set { _ResourceRead = value; }
        }

        private static string _EnterpriseIdsToShowReleaseNo;
        public static string EnterpriseIdsToShowReleaseNo
        {
            get { return _EnterpriseIdsToShowReleaseNo; }
            set { _EnterpriseIdsToShowReleaseNo = value; }
        }

        private static string _eVMIRooms;
        public static string eVMIRooms
        {
            get { return _eVMIRooms; }
            set { _eVMIRooms = value; }
        }

        private static string _QBRooms;
        public static string QBRooms
        {
            get { return _QBRooms; }
            set { _QBRooms = value; }
        }

        private static string _QBClientID;
        public static string QBClientID
        {
            get { return _QBClientID; }
            set { _QBClientID = value; }
        }

        private static string _QBClientSecret;
        public static string QBClientSecret
        {
            get { return _QBClientSecret; }
            set { _QBClientSecret = value; }
        }

        private static string _QBEnvironment;
        public static string QBEnvironment
        {
            get { return _QBEnvironment; }
            set { _QBEnvironment = value; }
        }

        private static string _QBRedirectUrl;
        public static string QBRedirectUrl
        {
            get { return _QBRedirectUrl; }
            set { _QBRedirectUrl = value; }
        }

        private static string _QBDiscoveryUrl;
        public static string QBDiscoveryUrl
        {
            get { return _QBDiscoveryUrl; }
            set { _QBDiscoveryUrl = value; }
        }

        private static string _QBVersion;
        public static string QBVersion
        {
            get { return _QBVersion; }
            set { _QBVersion = value; }
        }

        private static string _NonRedirectablUrls;
        public static string NonRedirectablUrls
        {
            get { return _NonRedirectablUrls; }
            set { _NonRedirectablUrls = value; }
        }

        static eVMISiteSettings _eVMISiteSettings;
        public static eVMISiteSettings eVMISiteSettings
        {
            get { return _eVMISiteSettings; }
            set { _eVMISiteSettings = value; }
        }

        static string _UsersUISettingType;
        public static string UsersUISettingType
        {
            get { return _UsersUISettingType; }
            set { _UsersUISettingType = value; }
        }
        static string _DomainProtocol;
        public static string DomainProtocol
        {
            get { return _DomainProtocol; }
            set { _DomainProtocol = value; }
        }
        static string _ABOrderSyncMode;
        public static string ABOrderSyncMode
        {
            get { return _ABOrderSyncMode; }
            set { _ABOrderSyncMode = value; }
        }
        static string _EnforceResourcePagesRestriction;
        public static string EnforceResourcePagesRestriction
        {
            get { return _EnforceResourcePagesRestriction; }
            set { _EnforceResourcePagesRestriction = value; }
        }
        static string _AllowedUserIdsResources;
        public static string AllowedUserIdsResources
        {
            get { return _AllowedUserIdsResources; }
            set { _AllowedUserIdsResources = value; }
        }
        static string _AllowedEntIdsResources;
        public static string AllowedEntIdsResources
        {
            get { return _AllowedEntIdsResources; }
            set { _AllowedEntIdsResources = value; }
        }

        static string _StrictRediretion;
        public static string StrictRediretion
        {
            get { return _StrictRediretion; }
            set { _StrictRediretion = value; }
        }

        private static string _OrderFilePaths;
        public static string OrderFilePaths
        {
            get { return _OrderFilePaths; }
            set { _OrderFilePaths = value; }
        }

        private static string _OrderAllowedFileExtension;
        public static string OrderAllowedFileExtension
        {
            get { return _OrderAllowedFileExtension; }
            set { _OrderAllowedFileExtension = value; }
        }

        private static string _RequisitionFilePaths;
        public static string RequisitionFilePaths
        {
            get { return _RequisitionFilePaths; }
            set { _RequisitionFilePaths = value; }
        }

        private static string _RequisitionAllowedFileExtension;
        public static string RequisitionAllowedFileExtension
        {
            get { return _RequisitionAllowedFileExtension; }
            set { _RequisitionAllowedFileExtension = value; }
        }

        private static string _LogResourceCalls;
        public static string LogResourceCalls
        {
            get => _LogResourceCalls;
            set => _LogResourceCalls = value;
        }
        private static string _ReceiveFilePaths;
        public static string ReceiveFilePaths
        {
            get { return _ReceiveFilePaths; }
            set { _ReceiveFilePaths = value; }
        }

        private static string _ReceiveAllowedFileExtension;
        public static string ReceiveAllowedFileExtension
        {
            get { return _ReceiveAllowedFileExtension; }
            set { _ReceiveAllowedFileExtension = value; }
        }
        private static string _CommonAllowedFileExtension;
        public static string CommonAllowedFileExtension
        {
            get { return _CommonAllowedFileExtension; }
            set { _CommonAllowedFileExtension = value; }
        }

        private static string _Link2AllowedFileExtension;
        public static string Link2AllowedFileExtension
        {
            get { return _Link2AllowedFileExtension; }
            set { _Link2AllowedFileExtension = value; }
        }

        private static string _MailOrderFieldRoomRestriction { get; set; }

        public static string MailOrderFieldRoomRestriction
        {
            get { return _MailOrderFieldRoomRestriction; }
            set { _MailOrderFieldRoomRestriction = value; }
        }

        private static string _IsOktaEnable { get; set; }

        public static string IsOktaEnable
        {
            get { return _IsOktaEnable; }
            set { _IsOktaEnable = value; }
        }
        private static string _EmptyHeaderCheck { get; set; }

        public static string EmptyHeaderCheck
        {
            get { return _EmptyHeaderCheck; }
            set { _EmptyHeaderCheck = value; }
        }

        private static string _IsBorderStateURL { get; set; }
        public static string IsBorderStateURL
        {
            get { return _IsBorderStateURL; }
            set { _IsBorderStateURL = value; }
        }
        private static string _BorderStateDesignCssPostFix { get; set; }
        public static string BorderStateDesignCssPostFix
        {
            get { return _BorderStateDesignCssPostFix; }
            set { _BorderStateDesignCssPostFix = value; }
        }
        private static string _OktaServiceUrl { get; set; }

        public static string OktaServiceUrl
        {
            get { return _OktaServiceUrl; }
            set { _OktaServiceUrl = value; }
        }
        private static string _OktaReturnUrl { get; set; }

        public static string OktaReturnUrl
        {
            get { return _OktaReturnUrl; }
            set { _OktaReturnUrl = value; }
        }
        private static string _EnableOktaLoginForSpecialUrls { get; set; }
        public static string EnableOktaLoginForSpecialUrls
        {
            get { return _EnableOktaLoginForSpecialUrls; }
            set { _EnableOktaLoginForSpecialUrls = value; }
        }
        private static string _EnableBorderStateID { get; set; }
        public static string EnableBorderStateID
        {
            get { return _EnableBorderStateID; }
            set { _EnableBorderStateID = value; }
        }

        private static string _RestrictNewUserEMailEnterpriseIds { get; set; }
        public static string RestrictNewUserEMailEnterpriseIds
        {
            get { return _RestrictNewUserEMailEnterpriseIds; }
            set { _RestrictNewUserEMailEnterpriseIds = value; }
        }

        private static string _IgnoreLatestCode { get; set; }
        public static string IgnoreLatestCode
        {
            get { return _IgnoreLatestCode; }
            set { _IgnoreLatestCode = value; }
        }

        

    }


    public class SiteSettingInfoHelper
    {
        public SiteSettingInfo GetSiteSettingInfo()
        {
            SiteSettingInfo objSiteSetting = new SiteSettingInfo();
            string PathFile = System.Configuration.ConfigurationManager.AppSettings["JsonSiteSettingFilePath"];
            string strJson = System.IO.File.ReadAllText(PathFile);
            objSiteSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<SiteSettingInfo>(strJson);
            return objSiteSetting;
        }

        public void SaveSiteSettingInfo(SiteSettingInfo objSiteSetting)
        {
            if (objSiteSetting != null)
            {
                string PathFile = System.Configuration.ConfigurationManager.AppSettings["JsonSiteSettingFilePath"];
                string JSONData = Newtonsoft.Json.JsonConvert.SerializeObject(objSiteSetting, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(PathFile, JSONData);


                SiteSettingHelper.LoginWithSelection = objSiteSetting.LoginWithSelection;
                SiteSettingHelper.ChartLabelCharSize = objSiteSetting.ChartLabelCharSize;
                SiteSettingHelper.DelayRedCount = objSiteSetting.DelayRedCount;
                SiteSettingHelper.EnableOptimizations = objSiteSetting.EnableOptimizations;
                SiteSettingHelper.ItemDetailsNew = objSiteSetting.ItemDetailsNew;
                SiteSettingHelper.AcceptLicence = objSiteSetting.AcceptLicence;
                SiteSettingHelper.ILQImportNew = objSiteSetting.ILQImportNew;
                SiteSettingHelper.RoleCodeImprove = objSiteSetting.RoleCodeImprove;
                SiteSettingHelper.ItemDetailsChange = objSiteSetting.ItemDetailsChange;
                SiteSettingHelper.OldResultView = objSiteSetting.OldResultView;
                SiteSettingHelper.OldUserMasterUDF = objSiteSetting.OldUserMasterUDF;
                SiteSettingHelper.ReleaseNumber = objSiteSetting.ReleaseNumber;
                SiteSettingHelper.OrderNumberDateFormat = objSiteSetting.OrderNumberDateFormat;
                SiteSettingHelper.NoImage = objSiteSetting.NoImage;
                SiteSettingHelper.InventoryPhoto = objSiteSetting.InventoryPhoto;
                SiteSettingHelper.AssetPhoto = objSiteSetting.AssetPhoto;
                SiteSettingHelper.ToolPhoto = objSiteSetting.ToolPhoto;
                SiteSettingHelper.InventoryLink2 = objSiteSetting.InventoryLink2;
                SiteSettingHelper.SupplierPhoto = objSiteSetting.SupplierPhoto;
                SiteSettingHelper.WorkOrderFilePaths = objSiteSetting.WorkOrderFilePaths;
                SiteSettingHelper.OrderFilePaths = objSiteSetting.OrderFilePaths;
                SiteSettingHelper.AllowedCharacter = objSiteSetting.AllowedCharacter;
                SiteSettingHelper.XMLFilePath = objSiteSetting.XMLFilePath;
                SiteSettingHelper.ProjectSpendLimitPercentage = objSiteSetting.ProjectSpendLimitPercentage;
                SiteSettingHelper.EnableNewCountPopup = objSiteSetting.EnableNewCountPopup;
                SiteSettingHelper.ApplyChangePassword = objSiteSetting.ApplyChangePassword;
                SiteSettingHelper.NewBaseResourceFileName = objSiteSetting.NewBaseResourceFileName;
                SiteSettingHelper.BOMInventoryPhoto = objSiteSetting.BOMInventoryPhoto;
                SiteSettingHelper.BOMInventoryLink2 = objSiteSetting.BOMInventoryLink2;
                SiteSettingHelper.ItemBinBarcodeNewPage = objSiteSetting.ItemBinBarcodeNewPage;
                SiteSettingHelper.UDFMaxLength = objSiteSetting.UDFMaxLength;
                SiteSettingHelper.ExceptActionsForNoRoom = objSiteSetting.ExceptActionsForNoRoom;
                SiteSettingHelper.AccessQryUserNames = objSiteSetting.AccessQryUserNames;
                SiteSettingHelper.AccessQryRoleIds = objSiteSetting.AccessQryRoleIds;
                SiteSettingHelper.RunWithReportConnection = objSiteSetting.RunWithReportConnection;
                SiteSettingHelper.ApplyNewAuthorization = objSiteSetting.ApplyNewAuthorization;
                SiteSettingHelper.ValidateAntiForgeryTokenOnAllPosts = objSiteSetting.ValidateAntiForgeryTokenOnAllPosts;
                SiteSettingHelper.MethodsToIgnoreXSRF = objSiteSetting.MethodsToIgnoreXSRF;
                SiteSettingHelper.SaveNewAuthorizationError = objSiteSetting.SaveNewAuthorizationError;
                SiteSettingHelper.SaveAntiForgeryError = objSiteSetting.SaveAntiForgeryError;
                SiteSettingHelper.IsShowGlobalReprotBuilder = objSiteSetting.IsShowGlobalReprotBuilder;
                SiteSettingHelper.CustomShortDatePatterns = objSiteSetting.CustomShortDatePatterns;
                SiteSettingHelper.MethodsToCheckIsInsert = objSiteSetting.MethodsToCheckIsInsert;
                SiteSettingHelper.IsNewNotification = objSiteSetting.IsNewNotification;
                SiteSettingHelper.ControllerName = objSiteSetting.ControllerName;
                SiteSettingHelper.ModuleName = objSiteSetting.ModuleName;
                SiteSettingHelper.ModuleMasterName = objSiteSetting.ModuleMasterName;
                SiteSettingHelper.AllowEnterpriseRoomForNN = objSiteSetting.AllowEnterpriseRoomForNN;
                SiteSettingHelper.LoadEnterpriseGridOrdering = objSiteSetting.LoadEnterpriseGridOrdering;
                SiteSettingHelper.ValidatePhoneNumber = objSiteSetting.ValidatePhoneNumber;
                SiteSettingHelper.ProductEnvironment = objSiteSetting.ProductEnvironment;
                SiteSettingHelper.OnlyIfRoomAvailable = objSiteSetting.OnlyIfRoomAvailable;
                SiteSettingHelper.decimalPointFromConfig = objSiteSetting.decimalPointFromConfig;
                SiteSettingHelper.ForWardDatesEnterpriseID = objSiteSetting.ForWardDatesEnterpriseID;
                SiteSettingHelper.NewRangeDataFill = objSiteSetting.NewRangeDataFill;
                SiteSettingHelper.LoadNarrowDataCount = objSiteSetting.LoadNarrowDataCount;
                SiteSettingHelper.DisplayInventoryValueforReplenish = objSiteSetting.DisplayInventoryValueforReplenish;
                SiteSettingHelper.RoomReportGrid = objSiteSetting.RoomReportGrid;
                SiteSettingHelper.RoomReportGridCompanyID = objSiteSetting.RoomReportGridCompanyID;
                SiteSettingHelper.RoomReportGridRoomID = objSiteSetting.RoomReportGridRoomID;
                SiteSettingHelper.AllowedPullQtyEdit = objSiteSetting.AllowedPullQtyEdit;
                SiteSettingHelper.WorkOrderAllowedFileExtension = objSiteSetting.WorkOrderAllowedFileExtension;
                SiteSettingHelper.OrderAllowedFileExtension = objSiteSetting.OrderAllowedFileExtension;
                SiteSettingHelper.ResourceRead = objSiteSetting.ResourceRead;
                SiteSettingHelper.ResourceSave = objSiteSetting.ResourceSave;
                SiteSettingHelper.EnterpriseIdsToShowReleaseNo = objSiteSetting.EnterpriseIdsToShowReleaseNo;
                SiteSettingHelper.eVMIRooms = objSiteSetting.eVMIRooms;
                SiteSettingHelper.QBRooms = objSiteSetting.QBRooms;
                SiteSettingHelper.QBClientID = objSiteSetting.QBClientID;
                SiteSettingHelper.QBClientSecret = objSiteSetting.QBClientSecret;
                SiteSettingHelper.QBEnvironment = objSiteSetting.QBEnvironment;
                SiteSettingHelper.QBRedirectUrl = objSiteSetting.QBRedirectUrl;
                SiteSettingHelper.QBDiscoveryUrl = objSiteSetting.QBDiscoveryUrl;
                SiteSettingHelper.QBVersion = objSiteSetting.QBVersion;
                SiteSettingHelper.NonRedirectablUrls = objSiteSetting.NonRedirectablUrls;
                SiteSettingHelper.eVMISiteSettings = objSiteSetting.eVMISiteSettings;
                SiteSettingHelper.HelpDocumentFolderPath = objSiteSetting.HelpDocumentFolderPath;
                SiteSettingHelper.QuoteNumberDateFormat = objSiteSetting.QuoteNumberDateFormat;
                SiteSettingHelper.UsersUISettingType = objSiteSetting.UsersUISettingType;
                SiteSettingHelper.DomainProtocol = objSiteSetting.DomainProtocol;
                SiteSettingHelper.ABOrderSyncMode = objSiteSetting.ABOrderSyncMode;
                SiteSettingHelper.EnforceResourcePagesRestriction = objSiteSetting.EnforceResourcePagesRestriction;
                SiteSettingHelper.AllowedUserIdsResources = objSiteSetting.AllowedUserIdsResources;
                SiteSettingHelper.AllowedEntIdsResources = objSiteSetting.AllowedEntIdsResources;
                SiteSettingHelper.StrictRediretion = objSiteSetting.StrictRediretion;
                SiteSettingHelper.RequisitionFilePaths = objSiteSetting.RequisitionFilePaths;
                SiteSettingHelper.RequisitionAllowedFileExtension = objSiteSetting.RequisitionAllowedFileExtension;
                SiteSettingHelper.LogResourceCalls = objSiteSetting.LogResourceCalls;
                SiteSettingHelper.ReceiveFilePaths = objSiteSetting.ReceiveFilePaths;
                SiteSettingHelper.ReceiveAllowedFileExtension = objSiteSetting.ReceiveAllowedFileExtension;
                SiteSettingHelper.CommonAllowedFileExtension = objSiteSetting.CommonAllowedFileExtension;
                SiteSettingHelper.Link2AllowedFileExtension = objSiteSetting.Link2AllowedFileExtension;
                SiteSettingHelper.MailOrderFieldRoomRestriction = objSiteSetting.MailOrderFieldRoomRestriction;
                SiteSettingHelper.IsOktaEnable = objSiteSetting.IsOktaEnable;
                SiteSettingHelper.EmptyHeaderCheck = objSiteSetting.EmptyHeaderCheck;
                SiteSettingHelper.IsBorderStateURL = objSiteSetting.IsBorderStateURL;
                SiteSettingHelper.BorderStateDesignCssPostFix = objSiteSetting.BorderStateDesignCssPostFix;
                SiteSettingHelper.OktaReturnUrl = objSiteSetting.OktaReturnUrl;
                SiteSettingHelper.OktaServiceUrl = objSiteSetting.OktaServiceUrl;
                SiteSettingHelper.EnableOktaLoginForSpecialUrls = objSiteSetting.EnableOktaLoginForSpecialUrls;
                SiteSettingHelper.EnableBorderStateID = objSiteSetting.EnableBorderStateID; 
                SiteSettingHelper.RestrictNewUserEMailEnterpriseIds = objSiteSetting.RestrictNewUserEMailEnterpriseIds;
                SiteSettingHelper.IgnoreLatestCode = objSiteSetting.IgnoreLatestCode;
            }
        }
    }
}
