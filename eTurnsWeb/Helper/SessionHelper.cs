using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;


namespace eTurnsWeb.Helper
{
    public static class SessionHelper
    {
        //To be set from login later and remove hardcore values from here
        public static string CriticalQuantityBackColor = "#FFA17F";
        public static string MinimumQuantityBackColor = "#FFFEC4";
        public static string InActiveQuantityBackColor = "#ABABAB";
        public static string ConsignmentItemPulled = "#F6B3B3";
        public static string ConsingmentItemBilled = "#C8F6C8";
        public static string ConsingmentItemEDISent = "#FFFF00";
        public static string TransferRequestedGreaterOHQ = "#FFFF00";
        public static string QuoteLegendNotOrderableBackColor = "#b1aeae";
        //To be set from login later and remove hardcore values from here

        public static UserMasterDTO LoggedinUser
        {
            get
            {
                if (HttpContext.Current.Session["LoggedinUser"] != null)
                {
                    return (UserMasterDTO)HttpContext.Current.Session["LoggedinUser"];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["LoggedinUser"] = value;
            }
        }

        public static eVMISiteSettings SensorBinRoomSettings
        {
            get
            {
                if (HttpContext.Current.Session["SensorBinRoomSettings"] != null)
                {
                    return (eVMISiteSettings)HttpContext.Current.Session["SensorBinRoomSettings"];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["SensorBinRoomSettings"] = value;
            }
        }



        public static Int64 RoleID
        {
            get
            {
                long id = -1;
                long.TryParse(Convert.ToString(HttpContext.Current.Session["RoleID"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["RoleID"] = value;
            }
        }
        public static int UserType
        {
            get
            {
                int id = -1;
                int.TryParse(Convert.ToString(HttpContext.Current.Session["UserType"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["UserType"] = value;
            }
        }

        public static bool SetSessionCompleted
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["SetSessionCompleted"]);
            }
            set
            {
                HttpContext.Current.Session["SetSessionCompleted"] = value;
            }
        }

        public static string EnterPriseDBName
        {
            get
            {
                if (HttpContext.Current.Session["EnterPriseDBName"] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session["EnterPriseDBName"]);
                }
                else
                {
                    return string.Empty;
                }



            }
            set
            {
                HttpContext.Current.Session["EnterPriseDBName"] = value;

            }
        }

        public static bool IsABEnterprise
        {
            get
            {
                bool _IsABEnterprise = false;
                bool.TryParse(Convert.ToString(HttpContext.Current.Session["IsABEnterprise"]), out _IsABEnterprise);
                return _IsABEnterprise;
            }
            set
            {
                HttpContext.Current.Session["IsABEnterprise"] = value;
            }
        }

        //public static string EnterPriseDBConnectionString
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["EnterPriseDBConnectionString"] != null)
        //        {
        //            return Convert.ToString(HttpContext.Current.Session["EnterPriseDBConnectionString"]);
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }

        //    }
        //    set
        //    {
        //        if (System.Configuration.ConfigurationManager.AppSettings["LiveDbAccess"] == "1")
        //        {
        //            string originalval = value;
        //            if (!string.IsNullOrWhiteSpace(originalval))
        //            {
        //                string[] strArr = (originalval ?? string.Empty).Split(new char[1] { ';' });
        //                strArr[0] = "Data Source=172.31.11.184";
        //                originalval = string.Join(";", strArr);
        //                HttpContext.Current.Session["EnterPriseDBConnectionString"] = originalval;
        //            }
        //            else
        //            {
        //                HttpContext.Current.Session["EnterPriseDBConnectionString"] = value;
        //            }

        //        }
        //        else
        //        {
        //            HttpContext.Current.Session["EnterPriseDBConnectionString"] = value;
        //        }

        //    }
        //}
        public static Int64 EnterPriceID
        {
            get
            {
                int id = -1;
                int.TryParse(Convert.ToString(HttpContext.Current.Session["EnterPriceID"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["EnterPriceID"] = value;
                HttpRuntime.Cache["Signal_EnterPriceID"] = value;
            }
        }
        public static string EnterPriceName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["EnterPriceName"]);
            }
            set
            {
                HttpContext.Current.Session["EnterPriceName"] = value;
            }
        }

        public static TimeZoneInfo CurrentTimeZone
        {
            get
            {
                TimeZoneInfo timezone = TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]));
                return timezone;
            }
            set
            {
                HttpContext.Current.Session["CurrentTimeZone"] = value.ToSerializedString();
            }
        }
        public static CultureInfo RoomCulture
        {
            get
            {
                CultureInfo timezone = (CultureInfo)HttpContext.Current.Session["RoomCulture"];
                return timezone;
            }
            set
            {
                HttpContext.Current.Session["RoomCulture"] = value;
                eTurns.DTO.Resources.ResourceHelper.CurrentCult = value;
                eTurns.DTO.Resources.ResourceModuleHelper.CurrentCult = value;
            }
        }

        public static DashboardParameterDTO DashboardParams
        {
            get
            {
                if (HttpContext.Current.Session["DashboardParams"] != null)
                {
                    return (DashboardParameterDTO)HttpContext.Current.Session["DashboardParams"];
                }
                return null;

            }
            set
            {
                HttpContext.Current.Session["DashboardParams"] = value;
            }
        }

        public static string DateTimeFormat
        {
            get
            {

                return Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]);
            }
            set
            {
                HttpContext.Current.Session["DateTimeFormat"] = value;
            }
        }

        public static string RoomDateFormat
        {
            get
            {

                return Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]);
            }
            set
            {
                HttpContext.Current.Session["RoomDateFormat"] = value;
                RoomDateJSFormat = GetJSFormateFromCSharp(value);
            }
        }

        public static string RoomTimeFormat
        {
            get
            {

                return Convert.ToString(HttpContext.Current.Session["RoomTimeFormat"]);
            }
            set
            {
                HttpContext.Current.Session["RoomTimeFormat"] = value;
            }
        }

        private static string GetJSFormateFromCSharp(string value)
        {
            /// Replace M : m
            /// Replace MM : mm
            /// Replace MMM : M
            /// Replace MMMM : MM
            /// Replace yyyy : yy
            /// Replace yy : y
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
            {
                if (value.Contains("MMM"))
                {
                    value = value.Replace("MMM", "M");
                }
                else if (value.Contains("MM"))
                {
                    value = value.Replace("MM", "mm");
                }
                else if (value.Contains("M"))
                {
                    value = value.Replace("M", "m");
                }
                if (value.Contains("yyyy"))
                {
                    value = value.Replace("yyyy", "yy");
                }
                else if (value.Contains("yy"))
                {
                    value = value.Replace("yy", "y");
                }
            }

            return value;
        }

        public static string RoomDateJSFormat
        {
            get
            {

                return Convert.ToString(HttpContext.Current.Session["RoomDateJSFormat"]);
            }
            set
            {
                HttpContext.Current.Session["RoomDateJSFormat"] = value;
            }
        }

        public static Int64 RoomID
        {
            get
            {
                int id = -1;
                int.TryParse(Convert.ToString(HttpContext.Current.Session["RoomID"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["RoomID"] = value;
                HttpRuntime.Cache["Signal_RoomID"] = value;
                if (value > 0)
                {
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterPriseDBName);
                    eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(value, CompanyID, UserID);
                    if (objeTurnsRegionInfo != null)
                    {
                        CultureInfo roomculture = CultureInfo.CreateSpecificCulture(objeTurnsRegionInfo.CultureCode);
                        TimeZoneInfo roomTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);

                        RoomCulture = roomculture;
                        CurrentTimeZone = roomTimeZone;
                        DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern + " " + objeTurnsRegionInfo.ShortTimePattern;
                        RoomDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                        RoomTimeFormat = objeTurnsRegionInfo.ShortTimePattern;
                        NumberDecimalDigits = Convert.ToString(objeTurnsRegionInfo.NumberDecimalDigits);
                        CurrencyDecimalDigits = Convert.ToString(objeTurnsRegionInfo.CurrencyDecimalDigits);
                        TurnUsageFormat = Convert.ToString(objeTurnsRegionInfo.TurnsAvgDecimalPoints.GetValueOrDefault(2));
                        WeightDecimalPoints = Convert.ToString(objeTurnsRegionInfo.WeightDecimalPoints.GetValueOrDefault(0));

                        eTurnsRegionInfoProp = objeTurnsRegionInfo;
                    }
                    else
                    {
                        RoomCulture = CultureInfo.CreateSpecificCulture("en-US");

                        CurrentTimeZone = TimeZoneInfo.Utc;
                        DateTimeFormat = "M/d/yyyy" + " " + "h:mm:ss tt";
                        RoomDateFormat = "M/d/yyyy";
                        RoomTimeFormat = "h:mm:ss tt";
                        NumberDecimalDigits = "0";
                        CurrencyDecimalDigits = "0";
                        TurnUsageFormat = "2";
                        WeightDecimalPoints = "0";

                        eTurnsRegionInfoProp = new eTurnsRegionInfo();
                        eTurnsRegionInfoProp.CultureCode = "en-US";

                        eTurnsRegionInfoProp.CultureDisplayName = RoomCulture.DisplayName;
                        eTurnsRegionInfoProp.CultureName = RoomCulture.DisplayName;
                        eTurnsRegionInfoProp.CurrencyDecimalDigits = 0;
                        eTurnsRegionInfoProp.CurrencyGroupSeparator = "";
                        eTurnsRegionInfoProp.LongDatePattern = "M/d/yyyy";
                        eTurnsRegionInfoProp.LongTimePattern = "h:mm:ss tt";
                        eTurnsRegionInfoProp.NumberDecimalDigits = 0;
                        eTurnsRegionInfoProp.NumberDecimalSeparator = "";
                        eTurnsRegionInfoProp.ShortDatePattern = DateTimeFormat;
                        eTurnsRegionInfoProp.ShortTimePattern = "h:mm:ss tt";
                        eTurnsRegionInfoProp.TimeZoneName = "UTC";
                        eTurnsRegionInfoProp.CurrencySymbol = "";
                        eTurnsRegionInfoProp.WeightDecimalPoints = 0;
                        eTurnsRegionInfoProp.TurnsAvgDecimalPoints = 0;
                    }
                    DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
                    DashboardParams = new DashboardDAL(EnterPriseDBName).GetDashboardParameters(value, CompanyID);


                }
                if (RoomPermissions != null && RoomPermissions.Count > 0 && RoomPermissions.Where(t => t.EnterpriseId == EnterPriceID && t.CompanyId == CompanyID && t.RoomID == RoomID).Any())
                {
                    UserWiseRoomsAccessDetailsDTO objUserWiseRoomsAccessDetailsDTO = RoomPermissions.FirstOrDefault(t => t.EnterpriseId == EnterPriceID && t.CompanyId == CompanyID && t.RoomID == RoomID);
                    if (objUserWiseRoomsAccessDetailsDTO != null)
                    {
                        if (objUserWiseRoomsAccessDetailsDTO.PermissionList.Any(t => t.ModuleID == 99))
                        {
                            UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = objUserWiseRoomsAccessDetailsDTO.PermissionList.FirstOrDefault(t => t.ModuleID == 99);
                            long usersupplier = 0;

                            if (!string.IsNullOrEmpty(objUserRoleModuleDetailsDTO.ModuleValue) && objUserRoleModuleDetailsDTO.ModuleValue.Length > 0)
                            {
                                var tmpSupplierIds = objUserRoleModuleDetailsDTO.ModuleValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (objUserRoleModuleDetailsDTO.ModuleValue.IndexOf(',') > 0 && tmpSupplierIds.Length > 0)
                                {
                                    long.TryParse(tmpSupplierIds[0], out usersupplier);
                                }
                                else
                                {
                                    long.TryParse(objUserRoleModuleDetailsDTO.ModuleValue, out usersupplier);
                                }
                            }
                            UserSupplierID = usersupplier;
                            string[] UserSupplierString = new string[] { };

                            if (objUserRoleModuleDetailsDTO != null && !string.IsNullOrEmpty(objUserRoleModuleDetailsDTO.ModuleValue) && objUserRoleModuleDetailsDTO.ModuleValue.Length > 0)
                            {
                                UserSupplierString = objUserRoleModuleDetailsDTO.ModuleValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            }

                            if (UserSupplierString != null)
                            {
                                if (UserSuppliers == null)
                                {
                                    UserSuppliers = new List<long>();
                                }
                                foreach (var item in UserSupplierString)
                                {
                                    long tempid = 0;

                                    if (long.TryParse(item, out tempid) && tempid > 0)
                                    {
                                        UserSuppliers.Add(tempid);
                                    }
                                }

                                if (UserSupplierIds == null)
                                {
                                    UserSupplierIds = new List<long>();
                                }
                                UserSupplierIds = UserSuppliers;
                                //UserSuppliers = (UserSupplierString.Select(t => long.Parse(t)).ToList());
                            }
                        }
                        else
                        {
                            UserSupplierID = 0;
                            UserSuppliers = new List<long>();
                            UserSupplierIds = new List<long>();
                        }
                    }
                    else
                    {
                        UserSupplierID = 0;
                        UserSuppliers = new List<long>();
                        UserSupplierIds = new List<long>();
                    }

                }
                else
                {
                    UserSupplierID = 0;
                    UserSuppliers = new List<long>();
                    UserSupplierIds = new List<long>();
                }

                if (value > 0)
                {
                    //  RoomDTO objRoomDTO = new RoomDAL(EnterPriseDBName).GetRoomByIDPlain(value);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,BillingRoomType,AllowToolOrdering,IseVMI,AllowABIntegration";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + value.ToString() + "", "");

                    //isEVMI = objRoomDTO.IseVMI;
                    BillingRoomType = objRoomDTO.BillingRoomType.GetValueOrDefault(0);
                    AllowToolOrdering = objRoomDTO.AllowToolOrdering;
                    isEVMI = objRoomDTO.IseVMI;
                    AllowABIntegration = objRoomDTO.AllowABIntegration;
                }
                else
                {
                    //isEVMI = false;
                    BillingRoomType = 0;
                    AllowToolOrdering = false;
                    isEVMI = false;
                    AllowABIntegration = false;
                }

                if (RoomList != null)
                {
                    RoomDTO objCurrentSelectedRoom = RoomList.FirstOrDefault(t => t.ID == RoomID && t.EnterpriseId == EnterPriceID && t.CompanyID == CompanyID);
                    if (objCurrentSelectedRoom != null)
                    {

                        if (!objCurrentSelectedRoom.IsRoomActive)
                        {
                            List<UserWiseRoomsAccessDetailsDTO> lstSessoion = RoomPermissions;
                            if (lstSessoion != null && lstSessoion.Count > 0 && lstSessoion.Where(t => t.EnterpriseId == EnterPriceID && t.CompanyId == CompanyID && t.RoomID == RoomID).Any())
                            {
                                UserWiseRoomsAccessDetailsDTO objUserWiseRoomsAccessDetailsDTO = lstSessoion.FirstOrDefault(t => t.EnterpriseId == EnterPriceID && t.CompanyId == CompanyID && t.RoomID == RoomID);
                                if (objUserWiseRoomsAccessDetailsDTO != null)
                                {
                                    List<UserRoleModuleDetailsDTO> lstlocalpermissions = objUserWiseRoomsAccessDetailsDTO.PermissionList;
                                    PermissionTemplateDAL objPermissionTemplateDAL = new PermissionTemplateDAL(EnterPriseDBName);
                                    List<UserRoleModuleDetailsDTO> lstInactiveRoomMap = objPermissionTemplateDAL.GetPermissionsByTemplateInactiveRoom();
                                    long[] arrNotToSet = new long[] { 6, 61, 81, 114 };
                                    lstlocalpermissions.ForEach(ob =>
                                    {
                                        UserRoleModuleDetailsDTO objInactiveUserRoleModuleDetailsDTO = lstInactiveRoomMap.FirstOrDefault(d => d.ModuleID == ob.ModuleID);
                                        if (objInactiveUserRoleModuleDetailsDTO == null)
                                        {
                                            objInactiveUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                                        }
                                        if (ob.IsDelete)
                                        {
                                            ob.IsDelete = objInactiveUserRoleModuleDetailsDTO.IsDelete;
                                        }
                                        if (ob.IsInsert)
                                        {
                                            ob.IsInsert = objInactiveUserRoleModuleDetailsDTO.IsInsert;
                                        }
                                        if (ob.IsUpdate)
                                        {
                                            ob.IsUpdate = objInactiveUserRoleModuleDetailsDTO.IsUpdate;
                                        }
                                        if (ob.IsChecked)
                                        {
                                            ob.IsChecked = objInactiveUserRoleModuleDetailsDTO.IsChecked;
                                        }
                                        if (ob.IsView)
                                        {
                                            ob.IsView = objInactiveUserRoleModuleDetailsDTO.IsView;
                                        }
                                        if (ob.ShowArchived)
                                        {
                                            ob.ShowArchived = objInactiveUserRoleModuleDetailsDTO.ShowArchived;
                                        }
                                        if (ob.ShowDeleted)
                                        {
                                            ob.ShowDeleted = objInactiveUserRoleModuleDetailsDTO.ShowDeleted;
                                        }
                                        if (ob.ShowUDF)
                                        {
                                            ob.ShowUDF = objInactiveUserRoleModuleDetailsDTO.ShowUDF;
                                        }

                                    });
                                    objUserWiseRoomsAccessDetailsDTO.PermissionList = lstlocalpermissions;
                                    UserWiseRoomsAccessDetailsDTO objUserWiseRoomsAccessDetailsDTOTobeRemoved = lstSessoion.FirstOrDefault(t => t.EnterpriseId == EnterPriceID && t.CompanyId == CompanyID && t.RoomID == RoomID);
                                    lstSessoion.Remove(objUserWiseRoomsAccessDetailsDTOTobeRemoved);
                                    //lstSessoion = lstSessoion.Where(t => t.EnterpriseId == EnterPriceID && t.CompanyId == CompanyID && t.RoomID != RoomID).ToList();
                                    lstSessoion.Add(objUserWiseRoomsAccessDetailsDTO);
                                    RoomPermissions = lstSessoion;
                                    //if (objUserWiseRoomsAccessDetailsDTO.PermissionList.Any(t => t.ModuleID == 99))
                                    //{
                                    //    UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = objUserWiseRoomsAccessDetailsDTO.PermissionList.FirstOrDefault(t => t.ModuleID == 99);
                                    //    long usersupplier = 0;
                                    //    long.TryParse(objUserRoleModuleDetailsDTO.ModuleValue, out usersupplier);
                                    //    UserSupplierID = usersupplier;
                                    //}
                                }

                            }
                        }


                    }
                }
            }
        }

        public static Int64 UserSupplierID
        {
            get
            {
                int id = 0;
                int.TryParse(Convert.ToString(HttpContext.Current.Session["UserSupplierID"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["UserSupplierID"] = value;
            }
        }

        public static List<long> UserSupplierIds
        {
            get
            {
                List<long> ids = new List<long>();
                ids = (List<long>)(HttpContext.Current.Session["UserSupplierIds"]);
                return ids;
            }
            set
            {
                HttpContext.Current.Session["UserSupplierIds"] = value;
            }
        }

        public static List<long> UserSuppliers
        {
            get
            {

                List<long> id = new List<long>();
                id = (List<long>)(HttpContext.Current.Session["UserSuppliers"]);
                return id;
            }
            set
            {

                //
                HttpContext.Current.Session["UserSuppliers"] = value;
            }
        }
        public static Int64 CompanyID
        {
            get
            {
                int id = -1;
                int.TryParse(Convert.ToString(HttpContext.Current.Session["CompanyID"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["CompanyID"] = value;
                HttpRuntime.Cache["Signal_CompanyID"] = value;
            }
        }

        public static Int64 BillingRoomType
        {
            get
            {
                int id = -1;
                int.TryParse(Convert.ToString(HttpContext.Current.Session["BillingRoomType"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["BillingRoomType"] = value;

            }
        }

        public static Int64 UserID
        {
            get
            {
                int id = -1;
                int.TryParse(Convert.ToString(HttpContext.Current.Session["UserID"]), out id);
                return id;
            }
            set
            {
                HttpContext.Current.Session["UserID"] = value;
            }
        }

        public static bool AllowToolOrdering
        {
            get
            {
                bool _AllowToolOrdering = false;
                bool.TryParse(Convert.ToString(HttpContext.Current.Session["AllowToolOrdering"]), out _AllowToolOrdering);
                return _AllowToolOrdering;
            }
            set
            {
                HttpContext.Current.Session["AllowToolOrdering"] = value;
            }
        }

        public static bool IsNgNLFAllowed
        {
            get
            {
                bool _IsNgNLFAllowed = false;
                bool.TryParse(Convert.ToString(HttpContext.Current.Session["IsNgNLFAllowed"]), out _IsNgNLFAllowed);
                return _IsNgNLFAllowed;
            }
            set
            {
                HttpContext.Current.Session["IsNgNLFAllowed"] = value;
            }
        }

        public static String UserName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["UserName"]);
            }
            set
            {
                HttpContext.Current.Session["UserName"] = value;
            }
        }

        public static String RoomName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["RoomName"]);
            }
            set
            {
                HttpContext.Current.Session["RoomName"] = value;
            }
        }

        public static bool? isEVMI
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["isEVMI"]);
            }
            set
            {
                HttpContext.Current.Session["isEVMI"] = value;
            }
        }

        public static String EnterpriseLogoUrl
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["EnterpriseLogoUrl"]);
            }
            set
            {
                HttpContext.Current.Session["EnterpriseLogoUrl"] = value;
            }
        }
        public static String CompanyLogoUrl
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["CompanyLogoUrl"]);
            }
            set
            {
                HttpContext.Current.Session["CompanyLogoUrl"] = value;
            }
        }
        public static String CompanyName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["CompanyName"]);
            }
            set
            {
                HttpContext.Current.Session["CompanyName"] = value;
            }
        }
        public static bool IsLicenceAccepted
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["IsLicenceAccepted"]);
            }
            set
            {
                HttpContext.Current.Session["IsLicenceAccepted"] = value;
            }
        }
        public static bool AnotherLicenceAccepted
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["AnotherLicenceAccepted"]);
            }
            set
            {
                HttpContext.Current.Session["AnotherLicenceAccepted"] = value;
            }
        }
        public static bool HasPasswordChanged
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["HasPasswordChanged"]);
            }
            set
            {
                HttpContext.Current.Session["HasPasswordChanged"] = value;
            }
        }
        public static bool ShowDateTime
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["ShowDateTime"]);
            }
            set
            {
                HttpContext.Current.Session["ShowDateTime"] = value;
            }
        }
        public static Int32 SearchPattern
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["SearchPattern"]);
            }
            set
            {
                HttpContext.Current.Session["SearchPattern"] = value;
            }
        }
        //public static List<KeyValuePair<long, string>> RoomList
        //{
        //    get
        //    {
        //        return (List<KeyValuePair<long, string>>)HttpContext.Current.Session["RoomList"];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["RoomList"] = value;
        //    }
        //}

        public static List<RoomDTO> RoomList
        {
            get
            {
                return (List<RoomDTO>)HttpContext.Current.Session["RoomList"];
            }
            set
            {
                HttpContext.Current.Session["RoomList"] = value;
            }
        }

        public static List<EnterpriseDTO> EnterPriseList
        {
            get
            {
                return (List<EnterpriseDTO>)HttpContext.Current.Session["EnterPriseList"];
            }
            set
            {
                HttpContext.Current.Session["EnterPriseList"] = value;
            }
        }

        public static List<CompanyMasterDTO> CompanyList
        {
            get
            {
                return (List<CompanyMasterDTO>)HttpContext.Current.Session["CompanyList"];
            }
            set
            {
                HttpContext.Current.Session["CompanyList"] = value;
            }
        }

        public static List<UserWiseRoomsAccessDetailsDTO> RoomPermissions
        {
            get
            {
                return (List<UserWiseRoomsAccessDetailsDTO>)HttpContext.Current.Session["RoomPermissions"];
            }
            set
            {
                HttpContext.Current.Session["RoomPermissions"] = value;
            }
        }

        public static bool HasChangedFirstPassword
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["HasChangedFirstPassword"]);
            }
            set
            {
                HttpContext.Current.Session["HasChangedFirstPassword"] = value;
            }
        }

        public static void Add(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static object Get(string key)
        {
            return HttpContext.Current.Session[key];
        }
        public static void RomoveSessionByKey(string key)
        {
            HttpContext.Current.Session[key] = null;
        }
        public static bool NewEulaAccept
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["NewEulaAccept"]);
            }
            set
            {
                HttpContext.Current.Session["NewEulaAccept"] = value;
            }
        }



        public static String CurrentLoggedinUserName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["CurrentLoggedinUserName"]);
            }
            set
            {
                HttpContext.Current.Session["CurrentLoggedinUserName"] = value;
            }
        }
        //public static string CompanyResourceFolder
        //{
        //    get
        //    {
        //        return Convert.ToString(HttpContext.Current.Session["CompanyResourceFolder"]);
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["CompanyResourceFolder"] = value;
        //    }
        //}

        public enum ModuleList
        {
            None = 0,
            ItemManufacturer = -999,
            ItemSupplier = -998,
            BarcodeMaster = -997,
            UDF = -996,
            ItemLocationQty = -995,
            ManualCount = -994,
            PullImport = -991,
            PullImportWithLotSerial = -1000,
            DashboardItemMasterList = -990,
            DashboardOrderDataTable = -989,
            DashboardReceiveDataTable = -988,
            DashboardTransferDataTable = -987,
            DashboardRequisitionDataTable = -986,
            DashboardProjectDataTable = -985,
            DashboardAssetsMaintenanceDataTable = -984,
            DashboardToolsMaintenanceDataTable = -983,
            DashboardCountDataTable = -982,
            DashboardCartDataTable = -981,
            DashboardItemOrderDataTable = -980,
            ToolCertificationImages = -978,
            ItemLocationChange = -993,
            PullImportWithSameQty = -992,
            ToolAdjustmentCount = -979,
            ExportPermission = 5,
            QuickListPermission = 10,
            CartOrder = 11,
            CartTransferPermission = 12,
            Reports = 15,
            AllowChangeConsignedItems = 22,
            OnTheFlyEntry = 29,
            AllowConsignedCreditPull = 32,
            OrderSubmit = 33,
            AllowOrderToConsignedItem = 34,
            BinMaster = 37,
            FTPMaster = 37,
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
            Kits = 74,
            WIP = 75,
            OrderApproval = 76,
            EmailConfiguration = 77,
            AllowOverrideProjectSpendLimits = 78,
            AllowedToChangeConsignedQuantityItems = 79,
            Imports = 81,
            ToolCategory = 82,
            VenderMaster = 83,
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
            Allowapplyoncounts = 98,
            SupplierFilter = 99,
            HideCostMarkUpSellPrice = 100,
            LabelPrinting = 101,
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
            AssetMaintenance = 113,
            PermissionTemplates = 114,
            Notifications = 115,
            AllowOverwriteLotOrSerial = 116,
            AllowToEnterLotOrSerialInBlankBox = 117,
            ViewOnlyLotOrSerial = 118,
            AllowAnOrderToBeUnclose = 119,
            AllowAnOrderToBeUnapprove = 120,
            //PullImport = 125,
            //DashboardItemMasterList = 126,
            //DashboardOrderDataTable = 127,
            //DashboardReceiveDataTable = 128,
            //DashboardTransferDataTable = 129,
            //DashboardRequisitionDataTable = 130,
            //DashboardProjectDataTable = 131,
            //DashboardAssetsMaintenanceDataTable = 132,
            //DashboardToolsMaintenanceDataTable = 133,
            //DashboardCountDataTable = 134,
            //DashboardCartDataTable = 135
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
            OrderUOMMaster = 140,
            SuggestedReturnpermission = 141,
            DataArchival = 142,
            SupplierAccountDetail = 143,
            HelpDocumentPermission = 144,
            AllowtoViewMinMaxDashboard = 145,
            EnterpriseQuickList = 146,
            WrittenOffCategory = 147,
            Quote = 148,
            AllowanquotetobeApproved = 149,
            AllowanquotetobeSubmitted = 150,
            AllowanquotetobeUnapprove = 151,
            AllowanquotetobeUnclose = 152,
            Allowanchangequote = 153,
            QuoteToOrder = 154,
            ToolKitBuildBreak = 155,
            ViewReport = 156,
            ScheduleReport = 157,
            CustomizeReport = 158,
            EnterpriseGridColumnSetup = 159,
            EnterpriseUDFSetup = 160,
            QuickBooksIntegration = 161,
            EnterpriseItemQuickList = 162,
            SensorBinsRFIDeTags = 163,
            AllowEditItemSellPriceonPull = 164,
            AllowEditItemSellPriceonWorkOrderPull = 165,
            AllowEditItemSellPriceonRequitionPull = 166,
            BinUDF = 167,
            EditItemMaster = 168,
            CommonBOMToItem = 169,
            AllUDFSetup = -977

        }

        public enum PermissionType
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

        public enum ParentModuleList
        {
            Authentication = 1,
            Master = 2,
            Assets = 3,
            Reports = 4,
            Inventory = 5,
            Consume = 6,
            Replenish = 7,
            Receive = 8,
            Kits = 9,
            Configuration = 10,
            CategoryCost = 11
        }
        public enum MobileUDFList
        {
            AddCountDetail = 15,

            AssetMaster = 1,
            AssetAddOdometer = 6,
            AssetListMaintenance = 3,
            AssetMaintenanceLineItem = 5,
            AssetSchedularDtl = 8,
            AssetToolSchMapping = 38,
            AssetToSchedular = 7,
            BarcodeMaster = 21,
            KitMaster = 22,
            CartItemList = 11,
            CustomerMaster = 40,
            Dashboard = 35,
            DefaultScreen = 96,
            ItemMaster = 12,
            ItemLocation = 13,
            KitDetails = 23,
            MainLogin = 36,
            Message = 34,
            MoveItem = 18,
            MaterialStaging = 16,
            MSAddStagingDetails = 17,
            OrderMaster = 19,
            PDASetting = 39,
            ProjectMaster = 24,
            ProjectSpendItems = 25,
            PullMaster = 30,
            QuickListMaster = 26,
            QuickListItems = 27,
            ReceivedOrderTransferDetail = 20,
            RequisitionMaster = 28,
            RequisitionItem = 29,
            ResetAutoSeqNumber = 63,

            ToolMaster = 4,
            TransferMaster = 32,
            TransferItem = 33,
            WorkOrder = 9,
            WorkOrderItem = 10,
            ToolCheckInOutHistory = 66,
            UserMaster = 73

        }
        public enum MobileLanguageList
        {
            en_US = 1,
            nl_NL = 2,
            fr_FR = 3,
            de_DE = 4,
            ru_RU = 5,
            en_AU = 6,
            en_GB = 7,
            en_IN = 8,
            nl_BE = 9
        }

        public static ModulePermission GetModulePermissionByModule(ModuleList ModuleID)
        {
            //if (RoleID == -1 || RoleID == -2)
            //{
            //    return true;
            //}
            eTurns.DTO.UserRoleModuleDetailsDTO lstsubPermission = null;

            if (RoomPermissions != null)
            {
                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = RoomPermissions.Find(element => element.RoomID == RoomID && element.CompanyId == CompanyID && element.EnterpriseId == EnterPriceID);

                if (lstPermission == null)
                {
                    if (EnterPriceID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = 0;
                        lstPermission.CompanyId = 0;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 41;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }
                    else if (EnterPriceID > 0 && CompanyID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = EnterPriceID;
                        lstPermission.CompanyId = 0;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 39;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }
                    else if (EnterPriceID > 0 && CompanyID > 0 && RoomID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = EnterPriceID;
                        lstPermission.CompanyId = CompanyID;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 52;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }

                }

                if (lstPermission != null)
                {
                    lstsubPermission = lstPermission.PermissionList.Find(item => item.ModuleID == (int)ModuleID);
                }

                if (lstsubPermission != null)
                {
                    if (lstsubPermission.GroupId == 3 && lstsubPermission.IsChecked == true)
                    {
                        lstsubPermission.IsChecked = true;
                        lstsubPermission.IsDelete = true;
                        lstsubPermission.IsInsert = true;
                        lstsubPermission.IsUpdate = true;
                        lstsubPermission.IsView = true;
                        lstsubPermission.ShowArchived = true;
                        lstsubPermission.ShowUDF = true;
                        lstsubPermission.ShowDeleted = true;
                        lstsubPermission.ShowChangeLog = true;

                    }
                }

            }

            ModulePermission modulePermission = new ModulePermission(lstsubPermission, ModuleID);


            return modulePermission;
        }

        public static bool GetModulePermission(ModuleList ModuleID, PermissionType PermissionType)
        {
            //if (RoleID == -1 || RoleID == -2)
            //{
            //    return true;
            //}
            if (RoomPermissions != null)
            {
                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = RoomPermissions.Find(element => element.RoomID == RoomID && element.CompanyId == CompanyID && element.EnterpriseId == EnterPriceID);
                eTurns.DTO.UserRoleModuleDetailsDTO lstsubPermission = null;
                if (lstPermission == null)
                {
                    if (EnterPriceID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = 0;
                        lstPermission.CompanyId = 0;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 41;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }
                    else if (EnterPriceID > 0 && CompanyID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = EnterPriceID;
                        lstPermission.CompanyId = 0;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 39;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }
                    else if (EnterPriceID > 0 && CompanyID > 0 && RoomID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = EnterPriceID;
                        lstPermission.CompanyId = CompanyID;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 52;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }

                }

                if (lstPermission != null)
                    lstsubPermission = lstPermission.PermissionList.Find(item => item.ModuleID == (int)ModuleID);
                if (lstsubPermission != null)
                {
                    if (lstsubPermission.GroupId == 3 && lstsubPermission.IsChecked == true)
                    {
                        lstsubPermission.IsChecked = true;
                        lstsubPermission.IsDelete = true;
                        lstsubPermission.IsInsert = true;
                        lstsubPermission.IsUpdate = true;
                        lstsubPermission.IsView = true;
                        lstsubPermission.ShowArchived = true;
                        lstsubPermission.ShowUDF = true;
                        lstsubPermission.ShowDeleted = true;
                        lstsubPermission.ShowChangeLog = true;

                    }
                }
                if (lstsubPermission != null)
                {
                    if (PermissionType == SessionHelper.PermissionType.View)
                        return lstsubPermission.IsView;
                    else if (PermissionType == SessionHelper.PermissionType.Insert)
                        return lstsubPermission.IsInsert;
                    else if (PermissionType == SessionHelper.PermissionType.Update)
                        return lstsubPermission.IsUpdate;
                    else if (PermissionType == SessionHelper.PermissionType.Delete)
                        return lstsubPermission.IsDelete;
                    else if (PermissionType == SessionHelper.PermissionType.ShowDeleted)
                        return lstsubPermission.ShowDeleted;
                    else if (PermissionType == SessionHelper.PermissionType.ShowArchived)
                        return lstsubPermission.ShowArchived;
                    else if (PermissionType == SessionHelper.PermissionType.ShowUDF)
                        return lstsubPermission.ShowUDF;
                    else if (PermissionType == SessionHelper.PermissionType.Approval)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.Submit)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.ChangeOrder)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.AllowPull)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.IsChecked)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.ShowChangeLog)
                        return lstsubPermission.ShowChangeLog;

                }
            }

            return false;
        }


        public static UserRoleModuleDetailsDTO GetModulePermissionAsPerCurrentRequest(PermissionType PermissionType, AlleTurnsActionMethodsDTO currentRequestDTO)
        {
            //if (RoleID == -1 || RoleID == -2)
            //{
            //    return true;
            //}
            if (RoomPermissions != null)
            {
                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = RoomPermissions.Find(element => element.RoomID == RoomID && element.CompanyId == CompanyID && element.EnterpriseId == EnterPriceID);
                eTurns.DTO.UserRoleModuleDetailsDTO lstsubPermission = null;
                if (lstPermission == null)
                {
                    if (EnterPriceID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = 0;
                        lstPermission.CompanyId = 0;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 41;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }
                    else if (EnterPriceID > 0 && CompanyID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = EnterPriceID;
                        lstPermission.CompanyId = 0;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 39;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }
                    else if (EnterPriceID > 0 && CompanyID > 0 && RoomID < 1)
                    {
                        lstPermission = new UserWiseRoomsAccessDetailsDTO();
                        lstPermission.EnterpriseId = EnterPriceID;
                        lstPermission.CompanyId = CompanyID;
                        lstPermission.RoomID = 0;
                        List<UserRoleModuleDetailsDTO> templst = new List<UserRoleModuleDetailsDTO>();
                        UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                        objUserRoleModuleDetailsDTO.CompanyId = 0;
                        objUserRoleModuleDetailsDTO.ModuleID = 52;
                        objUserRoleModuleDetailsDTO.IsChecked = true;
                        objUserRoleModuleDetailsDTO.IsDelete = true;
                        objUserRoleModuleDetailsDTO.IsInsert = true;
                        objUserRoleModuleDetailsDTO.IsModule = true;
                        objUserRoleModuleDetailsDTO.IsRoomActive = true;
                        objUserRoleModuleDetailsDTO.IsUpdate = true;
                        objUserRoleModuleDetailsDTO.ShowUDF = true;
                        objUserRoleModuleDetailsDTO.IsView = true;
                        objUserRoleModuleDetailsDTO.ShowChangeLog = true;
                        templst.Add(objUserRoleModuleDetailsDTO);
                        lstPermission.PermissionList = templst;

                    }

                }

                if (lstPermission != null)
                    lstsubPermission = lstPermission.PermissionList.Find(item => item.ModuleID == currentRequestDTO.PermissionModuleID);
                if (lstsubPermission != null)
                {
                    if (lstsubPermission.GroupId == 3 && lstsubPermission.IsChecked == true)
                    {
                        lstsubPermission.IsChecked = true;
                        lstsubPermission.IsDelete = true;
                        lstsubPermission.IsInsert = true;
                        lstsubPermission.IsUpdate = true;
                        lstsubPermission.IsView = true;
                        lstsubPermission.ShowArchived = true;
                        lstsubPermission.ShowUDF = true;
                        lstsubPermission.ShowDeleted = true;
                        lstsubPermission.ShowChangeLog = true;

                    }
                }
                if (lstsubPermission != null)
                {
                    /*
                    if (PermissionType == SessionHelper.PermissionType.View)
                        return lstsubPermission.IsView;
                    else if (PermissionType == SessionHelper.PermissionType.Insert)
                        return lstsubPermission.IsInsert;
                    else if (PermissionType == SessionHelper.PermissionType.Update)
                        return lstsubPermission.IsUpdate;
                    else if (PermissionType == SessionHelper.PermissionType.Delete)
                        return lstsubPermission.IsDelete;
                    else if (PermissionType == SessionHelper.PermissionType.ShowDeleted)
                        return lstsubPermission.ShowDeleted;
                    else if (PermissionType == SessionHelper.PermissionType.ShowArchived)
                        return lstsubPermission.ShowArchived;
                    else if (PermissionType == SessionHelper.PermissionType.ShowUDF)
                        return lstsubPermission.ShowUDF;
                    else if (PermissionType == SessionHelper.PermissionType.Approval)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.Submit)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.ChangeOrder)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.AllowPull)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.IsChecked)
                        return lstsubPermission.IsChecked;
                    else if (PermissionType == SessionHelper.PermissionType.ShowChangeLog)
                        return lstsubPermission.ShowChangeLog;

                    */
                    return lstsubPermission;
                }
            }

            return null;
        }


        public static bool GetAdminPermission(ModuleList ModuleID)
        {
            //if (RoleID == -1 || RoleID == -2)
            //{
            //    return true;
            //}
            if (RoomPermissions != null)
            {
                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = RoomPermissions.FirstOrDefault(element => element.RoomID == RoomID && element.CompanyId == CompanyID && element.EnterpriseId == EnterPriceID);
                eTurns.DTO.UserRoleModuleDetailsDTO lstsubPermission = null;
                if (lstPermission == null)
                {
                    lstPermission = new UserWiseRoomsAccessDetailsDTO();
                    lstPermission.PermissionList = new List<UserRoleModuleDetailsDTO>();
                }
                lstsubPermission = lstPermission.PermissionList.FirstOrDefault(item => item.ModuleID == (int)ModuleID);
                if (lstsubPermission != null)
                {
                    return lstsubPermission.IsChecked;
                }

            }

            return false;
        }


        public static bool GetAdminPermissionAsPerCurrentRequest(AlleTurnsActionMethodsDTO currentRequestDTO)
        {
            //if (RoleID == -1 || RoleID == -2)
            //{
            //    return true;
            //}
            if (RoomPermissions != null)
            {
                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = RoomPermissions.FirstOrDefault(element => element.RoomID == RoomID && element.CompanyId == CompanyID && element.EnterpriseId == EnterPriceID);
                eTurns.DTO.UserRoleModuleDetailsDTO lstsubPermission = null;
                if (lstPermission == null)
                {
                    lstPermission = new UserWiseRoomsAccessDetailsDTO();
                    lstPermission.PermissionList = new List<UserRoleModuleDetailsDTO>();
                }
                lstsubPermission = lstPermission.PermissionList.FirstOrDefault(item => item.ModuleID == currentRequestDTO.PermissionModuleID.GetValueOrDefault(0));
                if (lstsubPermission != null)
                {
                    return lstsubPermission.IsChecked;
                }

            }

            return false;
        }


        public static int GetUDFListID(string ModuleName)
        {
            if (!string.IsNullOrEmpty(ModuleName))
            {
                try
                {
                    int Result = Convert.ToInt32((MobileUDFList)Enum.Parse(typeof(MobileUDFList), ModuleName.Replace("checkouttool", "ToolCheckInOutHistory").Replace("Tools", "ToolMaster").Replace("CartItem", "CartItemList").Replace("ItemMasterCount", "InventoryCount").Replace("ReturnOrderMaster", "OrderMaster").Replace("Rooms", "Room"), true));
                    return Result;
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public static int GetLanguageListID(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                try
                {
                    int Result = Convert.ToInt32((MobileLanguageList)Enum.Parse(typeof(MobileLanguageList), language.Replace("-", "_")));
                    return Result;

                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public static string GetModuleName(int ModuleId)
        {
            string strModuleName = string.Empty;
            switch (ModuleId)
            {
                case (int)ModuleList.BinMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Bin;
                    break;
                case (int)ModuleList.CategoryMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Category;
                    break;
                case (int)ModuleList.CompanyMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Company;
                    break;
                case (int)ModuleList.CustomerMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Customers;
                    break;
                case (int)ModuleList.EnterpriseMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Enterprise;
                    break;
                case (int)ModuleList.FreightTypeMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.FreightType;
                    break;
                case (int)ModuleList.GLAccountsMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.GLAccount;
                    break;
                case (int)ModuleList.GXPRConsignedJobMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.GXPRConsignedJob;
                    break;
                case (int)ModuleList.JobTypeMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.JobType;
                    break;
                case (int)ModuleList.ProjectMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Project;
                    break;
                case (int)ModuleList.SupplierMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Supplier;
                    break;
                case (int)ModuleList.ShipViaMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.ShipVia;
                    break;
                case (int)ModuleList.TechnicianMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Technician;
                    break;
                case (int)ModuleList.ToolMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Tool;
                    break;
                case (int)ModuleList.UnitMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Unit;
                    break;
                case (int)ModuleList.RoomMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Room;
                    break;
                case (int)ModuleList.LocationMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Location;
                    break;
                case (int)ModuleList.ManufacturerMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Manufacturer;
                    break;
                case (int)ModuleList.MeasurementTermMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.MeasurementTerm;
                    break;
                case (int)ModuleList.RoleMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Role;
                    break;
                case (int)ModuleList.UserMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.User;
                    break;
                case (int)ModuleList.ResourceMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Resource;
                    break;
                case (int)ModuleList.QuickListPermission:
                    strModuleName = eTurns.DTO.Resources.ResLayout.QuickList;
                    break;
                case (int)ModuleList.ItemMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.ItemMasterList;
                    break;
                case (int)ModuleList.PullMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.PullMasterList;
                    break;
                case (int)ModuleList.Count:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Count;
                    break;
                case (int)ModuleList.Suppliercatalog:
                    strModuleName = eTurns.DTO.Resources.ResLayout.SupplierCatalog;
                    break;
                case (int)ModuleList.Materialstaging:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Materialstaging;
                    break;
                case (int)ModuleList.Requisitions:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Requisitions;
                    break;
                case (int)ModuleList.WorkOrders:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Workorders;
                    break;
                case (int)ModuleList.Cart:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Cart;
                    break;
                case (int)ModuleList.Orders:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Orders;
                    break;
                case (int)ModuleList.Receive:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Receive;
                    break;
                case (int)ModuleList.Transfer:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Transfer;
                    break;
                case (int)ModuleList.Assets:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Assets;
                    break;
                case (int)ModuleList.AssetMaintenance:
                    strModuleName = eTurns.DTO.Resources.ResLayout.AssetMaintenance;
                    break;
                case (int)ModuleList.Kits:
                    strModuleName = eTurns.DTO.Resources.ResLayout.Kits;
                    break;
                case (int)ModuleList.WIP:
                    strModuleName = eTurns.DTO.Resources.ResLayout.WIP;
                    break;
                case (int)ModuleList.EmailConfiguration:
                    strModuleName = eTurns.DTO.Resources.ResLayout.EmailConfiguration;
                    break;
                case (int)ModuleList.Imports:
                    strModuleName = eTurns.DTO.Resources.ResLayout.ImportMaster;
                    break;
                case (int)ModuleList.ToolCategory:
                    strModuleName = eTurns.DTO.Resources.ResLayout.ToolCategory;
                    break;
                case (int)ModuleList.AssetCategory:
                    strModuleName = eTurns.DTO.Resources.ResLayout.AssetCategory;
                    break;
                case (int)ModuleList.ExternalUserConfiguration:
                    strModuleName = eTurns.DTO.Resources.ResLayout.ExternalUserConfiguration;
                    break;
                case (int)ModuleList.CompanyConfig:
                    strModuleName = eTurns.DTO.Resources.ResLayout.CompanyConfig;
                    break;
                case (int)ModuleList.eVMISetup:
                    strModuleName = eTurns.DTO.Resources.ResLayout.eVMISetup;
                    break;
                case (int)ModuleList.CostUOMMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.CostUOM;
                    break;
                case (int)ModuleList.InventoryClassificationMaster:
                    strModuleName = eTurns.DTO.Resources.ResLayout.InventoryClassification;
                    break;
                case (int)ModuleList.LabelPrinting:
                    strModuleName = eTurns.DTO.Resources.ResLayout.LabelPrinting;
                    break;
                case (int)ModuleList.EnterPriseConfiguration:
                    strModuleName = eTurns.DTO.Resources.ResLayout.EnterPriseConfiguration;
                    break;
                case (int)ModuleList.CatalogReport:
                    strModuleName = eTurns.DTO.Resources.ResLayout.CatalogReport;
                    break;
                case (int)ModuleList.MoveMaterial:
                    strModuleName = ResMoveMaterial.MenuLinkMoveMaterial;
                    break;
                case (int)ModuleList.EnterpriseQuickList:
                    strModuleName = eTurns.DTO.Resources.ResLayout.EnterpriseQuickList;
                    break;
                default:
                    break;


            }
            return strModuleName;
        }

        public static ModuleList GetModuleIDfromPageName(string PageName)
        {
            PageName = PageName.ToLower();
            ModuleList objModuleName = ModuleList.None;
            if (string.IsNullOrEmpty(PageName))
                return objModuleName;

            if (PageName.Contains("bin"))
                objModuleName = ModuleList.BinMaster;
            else if (PageName.Contains("quick"))
                objModuleName = ModuleList.QuickListPermission;
            else if (PageName.Contains("toolcategory"))
                objModuleName = ModuleList.ToolCategory;
            else if (PageName.Contains("toollocation"))
                objModuleName = ModuleList.LocationMaster;
            else if (PageName.Contains("assetcategory"))
                objModuleName = ModuleList.AssetCategory;
            else if (PageName.Contains("asset"))
                objModuleName = ModuleList.Assets;
            else if (PageName.Contains("assettoolschedule"))
                objModuleName = ModuleList.AssetToolScheduler;
            else if (PageName.Contains("maintenance"))
                objModuleName = ModuleList.AssetMaintenance;
            else if (PageName.Contains("schedulemapping"))
                objModuleName = ModuleList.AssetToolSchedulerMapping;
            else if (PageName.Contains("writtenoffcategorylist"))
                objModuleName = ModuleList.WrittenOffCategory;
            else if (PageName.Contains("category"))
                objModuleName = ModuleList.CategoryMaster;
            else if (PageName.Contains("company"))
                objModuleName = ModuleList.CompanyMaster;
            else if (PageName.Contains("customer"))
                objModuleName = ModuleList.CustomerMaster;
            else if (PageName.Contains("enterprise"))
                objModuleName = ModuleList.EnterpriseMaster;
            else if (PageName.Contains("freighttype"))
                objModuleName = ModuleList.FreightTypeMaster;
            else if (PageName.Contains("glaccounts"))
                objModuleName = ModuleList.GLAccountsMaster;
            else if (PageName.Contains("gxprconsignedjobmaster"))
                objModuleName = ModuleList.GXPRConsignedJobMaster;
            else if (PageName.Contains("jobtype"))
                objModuleName = ModuleList.JobTypeMaster;
            else if (PageName.Contains("project"))
                objModuleName = ModuleList.ProjectMaster;
            else if (PageName.Contains("supplier"))
                objModuleName = ModuleList.SupplierMaster;
            else if (PageName.Contains("shipvia"))
                objModuleName = ModuleList.ShipViaMaster;
            else if (PageName.Contains("technician"))
                objModuleName = ModuleList.TechnicianMaster;
            else if (PageName.Contains("tool"))
                objModuleName = ModuleList.ToolMaster;
            else if (PageName.Contains("unit"))
                objModuleName = ModuleList.UnitMaster;
            else if (PageName.Contains("room"))
                objModuleName = ModuleList.RoomMaster;
            else if (PageName.Contains("location"))
                objModuleName = ModuleList.BinMaster;
            else if (PageName.Contains("manufacturer"))
                objModuleName = ModuleList.ManufacturerMaster;
            else if (PageName.Contains("measurementterm"))
                objModuleName = ModuleList.MeasurementTermMaster;
            else if (PageName.Contains("role"))
                objModuleName = ModuleList.RoleMaster;
            else if (PageName.Contains("user"))
                objModuleName = ModuleList.UserMaster;
            else if (PageName.Contains("resource"))
                objModuleName = ModuleList.ResourceMaster;
            else if (PageName.Contains("import"))
                objModuleName = ModuleList.Imports;
            else if (PageName.Contains("emailconfi"))
                objModuleName = ModuleList.EmailConfiguration;
            else if (PageName.Contains("itemmaster"))
                objModuleName = ModuleList.ItemMaster;
            else if (PageName.Contains("pullmaster") || PageName.Contains("newpull"))
                objModuleName = ModuleList.PullMaster;
            else if (PageName.Contains("cartitems"))
                objModuleName = ModuleList.Cart;
            else if (PageName.Contains("materialstaging"))
                objModuleName = ModuleList.Materialstaging;
            else if (PageName.Contains("requisition"))
                objModuleName = ModuleList.Requisitions;
            else if (PageName.Contains("workorder"))
                objModuleName = ModuleList.WorkOrders;
            else if (PageName.Contains("returnorder"))
                objModuleName = ModuleList.ReturnOrder;
            else if (PageName.Contains("notifications"))
                objModuleName = ModuleList.ScheduleReport;
            else if (PageName.Contains("viewreports") || PageName.Contains("reportlist"))
                objModuleName = ModuleList.ViewReport;
            else if (PageName.Contains("createtemplate"))
                objModuleName = ModuleList.CustomizeReport;
            //else if (PageName.Contains("viewreports") || PageName.Contains("createtemplate") || PageName.Contains("ReportSetting"))
            //    objModuleName = ModuleList.Reports;
            else if (PageName.Contains("quote"))
                objModuleName = ModuleList.Quote;
            else if (PageName.Contains("dashboard"))
            {
                objModuleName = ModuleList.AllowtoViewDashboard;
            }
            else if (PageName.Contains("dashboard"))
            {
                objModuleName = ModuleList.AllowtoViewDashboard;
            }
            else if (PageName.Contains("InventoryAnalysis".ToLower()))
            {
                objModuleName = ModuleList.AllowtoViewMinMaxDashboard;
            }
            else if (PageName.Contains("inventorycountlist"))
            {
                objModuleName = ModuleList.Count;
            }
            else if (PageName.Contains("transfer"))
            {
                objModuleName = ModuleList.Transfer;
            }
            else if (PageName.Contains("catalogreport"))
            {
                objModuleName = ModuleList.CatalogReport;
            }
            else if (PageName.Contains("itemcreate"))
                objModuleName = ModuleList.ItemMaster;
            else if (PageName.ToLower() == "orderlist")
            {
                objModuleName = ModuleList.Orders;
            }
            return objModuleName;

        }

        public static PermissionType GetPermissionTypefromPageName(string PageName)
        {
            PageName = PageName.ToLower();
            PermissionType objPermissionType = PermissionType.View;

            if (PageName.Contains("create") || PageName.Contains("newpull"))
                objPermissionType = PermissionType.Insert;
            else if (PageName.Contains("edit"))
                objPermissionType = PermissionType.Update;
            else if (PageName.Contains("delete"))
                objPermissionType = PermissionType.Delete;
            else if (PageName.Contains("list"))
                objPermissionType = PermissionType.View;
            else if (PageName.Contains("view"))
                objPermissionType = PermissionType.View;
            else if (PageName.Contains("showdeleted"))
                objPermissionType = PermissionType.ShowDeleted;
            else if (PageName.Contains("showarchived"))
                objPermissionType = PermissionType.ShowArchived;
            else if (PageName.Contains("showudf"))
                objPermissionType = PermissionType.ShowUDF;
            else if (PageName.Contains("emailconfi"))
                objPermissionType = PermissionType.Approval;
            else if (PageName.Contains("import"))
                objPermissionType = PermissionType.View;
            else if (PageName.Contains("cartitems"))
                objPermissionType = PermissionType.View;
            return objPermissionType;
        }

        public static PermissionType GetPermissionTypefromCurrentRequest(AlleTurnsActionMethodsDTO currentRequestDTO)
        {
            string currentAction = (currentRequestDTO.ActionMethod ?? string.Empty).ToLower();


            PermissionType objPermissionType = PermissionType.View;

            if (currentAction.Contains("create") || currentAction.Contains("newpull"))
                objPermissionType = PermissionType.Insert;
            else if (currentAction.Contains("edit"))
                objPermissionType = PermissionType.View;//Update
            else if (currentAction.Contains("delete"))
                objPermissionType = PermissionType.Delete;
            else if (currentAction.Contains("list"))
                objPermissionType = PermissionType.View;
            else if (currentAction.Contains("view"))
                objPermissionType = PermissionType.View;
            else if (currentAction.Contains("showdeleted"))
                objPermissionType = PermissionType.ShowDeleted;
            else if (currentAction.Contains("showarchived"))
                objPermissionType = PermissionType.ShowArchived;
            else if (currentAction.Contains("showudf"))
                objPermissionType = PermissionType.ShowUDF;
            else if (currentAction.Contains("emailconfi"))
                objPermissionType = PermissionType.Approval;
            else if (currentAction.Contains("import"))
                objPermissionType = PermissionType.View;
            else if (currentAction.Contains("cartitems"))
                objPermissionType = PermissionType.View;
            else
            {

                if (currentRequestDTO.IsDelete.GetValueOrDefault(false) == true)
                {
                    objPermissionType = PermissionType.Delete;
                }
                else if (currentRequestDTO.IsInsert.GetValueOrDefault(false) == true)
                {
                    objPermissionType = PermissionType.Insert;
                }
                else if (currentRequestDTO.IsUpdate.GetValueOrDefault(false) == true)
                {
                    objPermissionType = PermissionType.Update;
                }
                else if (currentRequestDTO.IsView.GetValueOrDefault(false) == true)
                {
                    objPermissionType = PermissionType.View;
                }
            }


            return objPermissionType;
        }

        //public static CompanyConfigDTO CompanyConfig
        //{
        //    get
        //    {
        //        return (CompanyConfigDTO)HttpContext.Current.Session["CompanyConfigDTO"];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["CompanyConfigDTO"] = value;
        //    }
        //}

        public static string QuantityFormat
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["QuantityFormat"]);
            }
            set
            {
                HttpContext.Current.Session["QuantityFormat"] = value;
            }
        }
        public static string TurnUsageFormat
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["TurnUsageFormat"]);
            }
            set
            {
                HttpContext.Current.Session["TurnUsageFormat"] = value;
            }
        }
        public static string WeightDecimalPoints
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["WeightDecimalPoints"]);
            }
            set
            {
                HttpContext.Current.Session["WeightDecimalPoints"] = value;
            }
        }
        public static string NumberAvgDecimalPoints
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["NumberAvgDecimalPoints"]);
            }
            set
            {
                HttpContext.Current.Session["NumberAvgDecimalPoints"] = value;
            }
        }
        public static string PriceFormat
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["PriceFormat"]);
            }
            set
            {
                HttpContext.Current.Session["PriceFormat"] = value;
            }
        }
        public static string WeightFormat
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["WeightFormat"]);
            }
            set
            {
                HttpContext.Current.Session["WeightFormat"] = value;
            }
        }

        public static string CurrencyDecimalDigits
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["CurrencyDecimalDigits"]);
            }
            set
            {
                HttpContext.Current.Session["CurrencyDecimalDigits"] = value;
            }
        }
        public static eTurnsRegionInfo eTurnsRegionInfoProp
        {
            get
            {
                return (eTurnsRegionInfo)HttpContext.Current.Session["eTurnsRegionInfoProp"];
            }
            set
            {
                HttpContext.Current.Session["eTurnsRegionInfoProp"] = value;
            }
        }
        public static string NumberDecimalDigits
        {
            get
            {

                return Convert.ToString(HttpContext.Current.Session["NumberDecimalDigits"]);
            }
            set
            {
                HttpContext.Current.Session["NumberDecimalDigits"] = value;
            }
        }

        public static string CurrencySymbol
        {
            get
            {

                return Convert.ToString(HttpContext.Current.Session["CurrencySymbol"]);
            }
            set
            {
                HttpContext.Current.Session["CurrencySymbol"] = value;
            }
        }

        public static HelpDocumentMasterDTO GetHelpDocumentMaster(string ModuleName)
        {
            eTurnsMaster.DAL.HelpDocumentDAL objHelpDocDAL = new eTurnsMaster.DAL.HelpDocumentDAL();
            HelpDocumentMasterDTO objHelpDocDTO = objHelpDocDAL.GetHelpDocumentMasterByName(ModuleName);
            return objHelpDocDTO;
        }

        public static HelpDocumentMasterDTO GetHelpDocumentMasterByDocType(string ModuleName, int? DocType)
        {
            int Typefilter = (int)HelpDocType.Module;
            if (DocType != null)
                Typefilter = (int)DocType;

            eTurnsMaster.DAL.HelpDocumentDAL objHelpDocDAL = new eTurnsMaster.DAL.HelpDocumentDAL();
            HelpDocumentMasterDTO objHelpDocDTO = objHelpDocDAL.GetHelpDocumentMasterByDocType(ModuleName, Typefilter);
            return objHelpDocDTO;

        }

        public static HelpDocumentMasterDTO GetHelpDocumentByReportID(Int64 ReportID)
        {
            eTurnsMaster.DAL.HelpDocumentDAL objHelpDocDAL = new eTurnsMaster.DAL.HelpDocumentDAL();
            HelpDocumentMasterDTO objHelpDocDTO = objHelpDocDAL.GetHelpDocumentByReportID(ReportID, SessionHelper.EnterPriseDBName);
            return objHelpDocDTO;
        }

        public static List<HelpDocumentDetailDTO> GetHelpDocumentDetail(string ModuleName, int? DocType)
        {
            if (DocType == null)
                DocType = (int)HelpDocType.Module;

            eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDocDtlDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
            List<HelpDocumentDetailDTO> objHelpDocDtlDTO = objHelpDocDtlDAL.GetHelpDocumentDetailByModuleName(ModuleName, Convert.ToInt32(DocType));
            return objHelpDocDtlDTO;
        }

        public static List<HelpDocumentDetailDTO> GetHelpDocumentDetailByReportID(Int64 ReportID)
        {
            eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDocDtlDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
            List<HelpDocumentDetailDTO> objHelpDocDtlDTO = objHelpDocDtlDAL.GetHelpDocumentDetailByReportID(ReportID, SessionHelper.EnterPriseDBName);
            return objHelpDocDtlDTO;
        }

        public static string CurrentDomainURL
        {
            get
            {
                string retURL = string.Empty, Proto = "http", UrlPort = HttpContext.Current.Request.Url.Port.ToString();
                if (string.IsNullOrWhiteSpace(Convert.ToString(HttpContext.Current.Session["CurrentDomainURL"])))
                {
                    //Request.Url.Host
                    if (HttpContext.Current.Request.Headers.AllKeys.Contains("X-Forwarded-Proto"))
                    {
                        Proto = HttpContext.Current.Request.Headers["X-Forwarded-Proto"];
                    }
                    else if (HttpContext.Current.Request.IsSecureConnection)
                    {
                        Proto = "https";
                    }
                    if (HttpContext.Current.Request.Headers.AllKeys.Contains("X-Forwarded-Port"))
                    {
                        UrlPort = HttpContext.Current.Request.Headers["X-Forwarded-Port"];
                    }
                    if (!string.IsNullOrWhiteSpace(SiteSettingHelper.DomainProtocol))
                    {
                        SiteSettingHelper.DomainProtocol = SiteSettingHelper.DomainProtocol.ToLower();
                        if (SiteSettingHelper.DomainProtocol == "http" || SiteSettingHelper.DomainProtocol == "https")
                        {
                            Proto = SiteSettingHelper.DomainProtocol;
                        }
                    }
                    if (UrlPort == "443" || UrlPort == "80" || UrlPort == "0")
                    {
                        retURL = Proto + "://" + HttpContext.Current.Request.Url.Host;
                    }
                    else
                    {
                        retURL = Proto + "://" + HttpContext.Current.Request.Url.Host + ":" + UrlPort;
                    }

                    HttpContext.Current.Session["CurrentDomainURL"] = retURL;
                }
                else
                {
                    retURL = Convert.ToString(HttpContext.Current.Session["CurrentDomainURL"]);
                }
                return retURL;
            }
        }

        public static bool IsFromSelectedDomain
        {
            get
            {
                if (HttpContext.Current.Session["IsFromSelectedDomain"] == null)
                {
                    CommonDAL objCommonDAL = new CommonDAL(EnterPriseDBName);
                    bool value = objCommonDAL.HasSpecialDomain(CurrentDomainURL, EnterPriceID);
                    HttpContext.Current.Session["IsFromSelectedDomain"] = value;
                    return value;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["IsFromSelectedDomain"]);
                }
            }
            set
            {
                HttpContext.Current.Session["IsFromSelectedDomain"] = value;
            }
        }

        public static bool AllowABIntegration
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["AllowABIntegration"]);
            }
            set
            {
                HttpContext.Current.Session["AllowABIntegration"] = value;
            }
        }

    } // class


    /// <summary>
    /// 
    /// </summary>
    /// Added by amit t on 11-Mar-20
    public class ModulePermission : IDisposable
    {

        public ModulePermission(UserRoleModuleDetailsDTO lstsubPermission, SessionHelper.ModuleList moduleID)
        {
            this.ModuleID = moduleID;

            if (lstsubPermission != null)
            {
                this.IsView = lstsubPermission.IsView;
                this.IsInsert = lstsubPermission.IsInsert;
                this.IsUpdate = lstsubPermission.IsUpdate;
                this.IsDelete = lstsubPermission.IsDelete;
                this.IsShowDeleted = lstsubPermission.ShowDeleted;
                this.IsShowArchived = lstsubPermission.ShowArchived;
                this.IsShowUDF = lstsubPermission.ShowUDF;
                this.IsApproval = lstsubPermission.IsChecked;
                this.IsSubmit = lstsubPermission.IsChecked;
                this.IsChangeOrder = lstsubPermission.IsChecked;
                this.IsAllowPull = lstsubPermission.IsChecked;
                this.IsChecked = lstsubPermission.IsChecked;
                this.IsShowChangeLog = lstsubPermission.ShowChangeLog;
            }
            else
            {
                this.IsView = false;
                this.IsInsert = false;
                this.IsUpdate = false;
                this.IsDelete = false;
                this.IsShowDeleted = false;
                this.IsShowArchived = false;
                this.IsShowUDF = false;
                this.IsApproval = false;
                this.IsSubmit = false;
                this.IsChangeOrder = false;
                this.IsAllowPull = false;
                this.IsChecked = false;
                this.IsShowChangeLog = false;
            }
        }

        public SessionHelper.ModuleList ModuleID { get; set; }


        /// <summary>
        /// PermissionType.View
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// PermissionType.Insert
        /// </summary>
        public bool IsInsert { get; set; }

        /// <summary>
        /// PermissionType.Update
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// PermissionType.Delete
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// PermissionType.ShowDeleted
        /// </summary>
        public bool IsShowDeleted { get; set; }

        /// <summary>
        /// PermissionType.ShowArchived
        /// </summary>
        public bool IsShowArchived { get; set; }

        /// <summary>
        /// PermissionType.ShowUDF
        /// </summary>
        public bool IsShowUDF { get; set; }

        /// <summary>
        /// PermissionType.Approval
        /// </summary>
        public bool IsApproval { get; set; }

        /// <summary>
        /// PermissionType.Submit
        /// </summary>
        public bool IsSubmit { get; set; }

        /// <summary>
        /// PermissionType.ChangeOrder
        /// </summary>
        public bool IsChangeOrder { get; set; }

        /// <summary>
        /// PermissionType.AllowPull
        /// </summary>
        public bool IsAllowPull { get; set; }

        /// <summary>
        /// PermissionType.IsChecked
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// PermissionType.ShowChangeLog
        /// </summary>
        public bool IsShowChangeLog { get; set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ModulePermission()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    //public static class WebConnectionHelper
    //{
    //    private static string _DataBaseName = Convert.ToString(HttpContext.Current.Session["EnterPriseDBName"]);
    //    private static string _DBServerName = Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]);
    //    private static string _DBUserName = Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]);
    //    private static string _DBPassword = Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]);
    //    private static string _DBproviderName = Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]);
    //    private static string _DBFailoverPartner = Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) ?? "172.31.12.215";
    //    private static string _IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
    //    private static string _WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
    //    private static string _WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
    //    private static string _MultiSubFailover = Convert.ToString(ConfigurationManager.AppSettings["MultiSubFailover"]) ?? "True";
    //    private static string _AppIntent = Convert.ToString(ConfigurationManager.AppSettings["AppIntent"]) ?? "ReadWrite";
    //    private static string _ReportAppIntent = Convert.ToString(ConfigurationManager.AppSettings["ReportAppIntent"]) ?? "ReadOnly";
    //    private static string _MasterDBName = Convert.ToString(ConfigurationManager.AppSettings["MasterDBName"]) ?? "eTurnsMaster";
    //    public static string DataBaseName
    //    {
    //        get
    //        {
    //            if (HttpContext.Current.Session["EnterPriseDBName"] != null)
    //            {
    //                return Convert.ToString(HttpContext.Current.Session["EnterPriseDBName"]);
    //            }
    //            else
    //            {
    //                return string.Empty;
    //            }
    //        }

    //    }
    //    public static string DBServerName
    //    {
    //        get
    //        {
    //            return _DBServerName;
    //        }
    //        set
    //        {
    //            _DBServerName = value;
    //        }
    //    }
    //    public static string DBUserName
    //    {
    //        get
    //        {
    //            return _DBUserName;
    //        }
    //        set
    //        {
    //            _DBUserName = value;
    //        }
    //    }
    //    public static string DBPassword
    //    {
    //        get
    //        {
    //            return _DBPassword;
    //        }
    //        set
    //        {
    //            _DBPassword = value;
    //        }
    //    }
    //    public static string DBproviderName
    //    {
    //        get
    //        {
    //            return _DBproviderName;
    //        }
    //        set
    //        {
    //            _DBproviderName = value;
    //        }
    //    }
    //    public static string DBFailoverPartner
    //    {
    //        get
    //        {
    //            return _DBFailoverPartner;
    //        }
    //        set
    //        {
    //            _DBFailoverPartner = value;
    //        }
    //    }

    //    public static string IsWSFC
    //    {
    //        get
    //        {
    //            return _IsWSFC;
    //        }
    //        set
    //        {
    //            _IsWSFC = value;
    //        }
    //    }
    //    public static string WSFCLSNR
    //    {
    //        get
    //        {
    //            return _WSFCLSNR;
    //        }
    //        set
    //        {
    //            _WSFCLSNR = value;
    //        }
    //    }
    //    public static string WSFCLSNRPORT
    //    {
    //        get
    //        {
    //            return _WSFCLSNRPORT;
    //        }
    //        set
    //        {
    //            _WSFCLSNRPORT = value;
    //        }
    //    }

    //    public static string MultiSubFailover
    //    {
    //        get
    //        {
    //            return _MultiSubFailover;
    //        }
    //        set
    //        {
    //            _MultiSubFailover = value;
    //        }
    //    }
    //    public static string AppIntent
    //    {
    //        get
    //        {
    //            return _AppIntent;
    //        }
    //        set
    //        {
    //            _AppIntent = value;
    //        }
    //    }
    //    public static string ReportAppIntent
    //    {
    //        get
    //        {
    //            return _ReportAppIntent;
    //        }
    //        set
    //        {
    //            _ReportAppIntent = value;
    //        }
    //    }

    //    public static string MasterDBName
    //    {
    //        get
    //        {
    //            return _MasterDBName;
    //        }
    //        set
    //        {
    //            _MasterDBName = value;
    //        }
    //    }

    //    public static string DataBaseEntityConnectionString
    //    {
    //        get
    //        {
    //            return GetConnectionString();
    //        }
    //    }
    //    private static string GetConnectionString()
    //    {
    //        SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
    //        sqlBuilder.DataSource = DBServerName;
    //        sqlBuilder.InitialCatalog = DataBaseName;
    //        sqlBuilder.UserID = DBUserName;
    //        sqlBuilder.Password = DBPassword;
    //        sqlBuilder.FailoverPartner = DBFailoverPartner;
    //        sqlBuilder.MultipleActiveResultSets = true;
    //        string providerString = sqlBuilder.ToString();
    //        sqlBuilder.PersistSecurityInfo = true;
    //        //sqlBuilder.MultipleActiveResultSets = true;
    //        sqlBuilder.ApplicationName = "EntityFramework";
    //        EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
    //        entityBuilder.Provider = "System.Data.SqlClient";
    //        if (IsWSFC == "yes")
    //        {
    //            providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", DataBaseName);
    //        }
    //        else
    //        {
    //            providerString = sqlBuilder.ToString();
    //        }
    //        entityBuilder.ProviderConnectionString = providerString;
    //        entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
    //        return entityBuilder.ToString();
    //    }

    //    public static string GetConnectionString(string strDataBaseName)
    //    {
    //        SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
    //        sqlBuilder.DataSource = DBServerName;
    //        sqlBuilder.InitialCatalog = strDataBaseName;
    //        sqlBuilder.UserID = DBUserName;
    //        sqlBuilder.Password = DBPassword;
    //        sqlBuilder.FailoverPartner = DBFailoverPartner;
    //        sqlBuilder.MultipleActiveResultSets = true;
    //        string providerString = sqlBuilder.ToString();
    //        sqlBuilder.PersistSecurityInfo = true;
    //        //sqlBuilder.MultipleActiveResultSets = true;
    //        sqlBuilder.ApplicationName = "EntityFramework";
    //        EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
    //        entityBuilder.Provider = "System.Data.SqlClient";
    //        if (IsWSFC == "yes")
    //        {
    //            providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", strDataBaseName);
    //        }
    //        else
    //        {
    //            providerString = sqlBuilder.ToString();
    //        }
    //        entityBuilder.ProviderConnectionString = providerString;
    //        entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
    //        return entityBuilder.ToString();
    //    }


    //    private static string GetWebsiteConnectionString()
    //    {
    //        SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
    //        sqlBuilder.DataSource = DBServerName;
    //        sqlBuilder.InitialCatalog = DataBaseName;
    //        sqlBuilder.UserID = DBUserName;
    //        sqlBuilder.Password = DBPassword;
    //        sqlBuilder.MultipleActiveResultSets = true;
    //        sqlBuilder.FailoverPartner = DBFailoverPartner;
    //        string providerString = string.Empty;

    //        if (IsWSFC == "yes")
    //        {
    //            providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", DataBaseName);
    //        }
    //        else
    //        {
    //            providerString = sqlBuilder.ToString();
    //        }

    //        return providerString;
    //    }

    //    private static string GetReportsConnectionString()
    //    {
    //        SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
    //        sqlBuilder.DataSource = DBServerName;
    //        sqlBuilder.InitialCatalog = DataBaseName;
    //        sqlBuilder.UserID = DBUserName;
    //        sqlBuilder.Password = DBPassword;
    //        sqlBuilder.MultipleActiveResultSets = true;
    //        sqlBuilder.FailoverPartner = DBFailoverPartner;
    //        string providerString = string.Empty;

    //        if (IsWSFC == "yes")
    //        {
    //            providerString = DbConnectionHelper.GetOledbConnection("GeneralReadOnly", DataBaseName);
    //        }
    //        else
    //        {
    //            providerString = sqlBuilder.ToString();
    //        }

    //        return providerString;
    //    }

    //    private static string GetReportsMasterConnectionString()
    //    {
    //        SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
    //        sqlBuilder.DataSource = DBServerName;
    //        sqlBuilder.InitialCatalog = MasterDBName;
    //        sqlBuilder.UserID = DBUserName;
    //        sqlBuilder.Password = DBPassword;
    //        sqlBuilder.MultipleActiveResultSets = true;
    //        sqlBuilder.FailoverPartner = DBFailoverPartner;
    //        string providerString = string.Empty;

    //        if (IsWSFC == "yes")
    //        {
    //            providerString = DbConnectionHelper.GetOledbConnection("GeneralReadOnly", MasterDBName);
    //        }
    //        else
    //        {
    //            providerString = sqlBuilder.ToString();
    //        }

    //        return providerString;
    //    }

    //    private static string GetAppMasterConnectionString()
    //    {
    //        SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
    //        sqlBuilder.DataSource = DBServerName;
    //        sqlBuilder.InitialCatalog = MasterDBName;
    //        sqlBuilder.UserID = DBUserName;
    //        sqlBuilder.Password = DBPassword;
    //        sqlBuilder.MultipleActiveResultSets = true;
    //        sqlBuilder.FailoverPartner = DBFailoverPartner;
    //        string providerString = string.Empty;

    //        if (IsWSFC == "yes")
    //        {
    //            providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", MasterDBName);
    //        }
    //        else
    //        {
    //            providerString = sqlBuilder.ToString();
    //        }

    //        return providerString;
    //    }

    //    public static string WebsiteDBConnectionString
    //    {
    //        get
    //        {
    //            return GetWebsiteConnectionString();
    //        }
    //    }
    //    public static string ReportsDBConnectionString
    //    {
    //        get
    //        {
    //            return GetReportsConnectionString();
    //        }
    //    }
    //    public static string ReportsMasterDBConnectionString
    //    {
    //        get
    //        {
    //            return GetReportsMasterConnectionString();
    //        }
    //    }
    //    public static string AppMasterDBConnectionString
    //    {
    //        get
    //        {
    //            return GetAppMasterConnectionString();
    //        }
    //    }

    //}
}
