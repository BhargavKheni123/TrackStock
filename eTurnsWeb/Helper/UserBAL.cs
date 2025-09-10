using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurnsWeb.Helper
{
    public class UserBAL
    {
        #region [Global declaration]
        public int LockooutMinutes = 3;
        public int MaxFailedAttempt = 5;

        #endregion


        public UserMasterDTO GetPasswordResetLink(string UserName)
        {
            string strPath = string.Empty;
            string strReplacepath = string.Empty;
            string PasswordResetLink = string.Empty;
            Guid? passwordLinkHist = null;
            UserMasterDTO objUser = new eTurnsMaster.DAL.EnterPriseUserMasterDAL().GetUserByNameNormal(UserName);

            if (objUser != null)
            {
                //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                //{
                //    strPath = System.Web.HttpContext.Current.Request.Url.ToString();
                //    strReplacepath = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                //    strPath = strPath.Replace(strReplacepath, "/");
                //}
                //else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DomainName"]))
                //{
                //    strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                //}
                if (!string.IsNullOrWhiteSpace(objUser.EnterPriseDomainURL))
                {
                    strPath = objUser.EnterPriseDomainURL;//+ "/"
                }
                else
                {
                    strPath = System.Configuration.ConfigurationManager.AppSettings["DomainURL"];
                }
                passwordLinkHist = Guid.NewGuid();

                string SendUserInfo = CommonUtility.EncryptText(objUser.ID + "__" + passwordLinkHist.ToString());
                PasswordResetLink = strPath + "Master/ResetPassword?fp=" + SendUserInfo;
                objUser.PasswordResetLink = PasswordResetLink;
                objUser.PasswordResetRequestID = passwordLinkHist;
            }

            return objUser;
        }

        public UserMasterDTO GetUserByName(string UserName)
        {
            UserMasterDTO objUserMasterDTO = new UserMasterDTO();
            UserMasterDTO objLocalUserMasterDTO = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                objUserMasterDTO = (from usr in context.UserMasters
                                    join usrcrt in context.UserMasters on usr.CreatedBy equals usrcrt.ID into usr_usrcrt_join
                                    from usr_usrcrt in usr_usrcrt_join.DefaultIfEmpty()
                                    join usrupd in context.UserMasters on usr.LastUpdatedBy equals usrupd.ID into usr_usrupd_join
                                    from usr_usrupd in usr_usrupd_join.DefaultIfEmpty()
                                    join em in context.EnterpriseMasters on usr.EnterpriseId equals em.ID into usr_em_join
                                    from usr_em in usr_em_join.DefaultIfEmpty()
                                    join rm in context.RoleMasters on new { rlid = usr.RoleId, ut = (usr.UserType ?? 0) < 2 } equals new { rlid = rm.ID, ut = true } into usr_rm_join
                                    from usr_rm in usr_rm_join.DefaultIfEmpty()
                                    where usr.IsDeleted == false && usr.UserName == UserName
                                    select new UserMasterDTO
                                    {
                                        CompanyID = usr.CompanyID,
                                        CompanyName = string.Empty,
                                        Created = usr.Created ?? DateTime.MinValue,
                                        CreatedBy = usr.CreatedBy,
                                        CreatedByName = usr_usrcrt.UserName,
                                        Email = usr.Email,
                                        EnterpriseDbName = usr_em.EnterpriseDBName,
                                        EnterpriseId = usr.EnterpriseId,
                                        GUID = usr.GUID,
                                        ID = usr.ID,
                                        IsArchived = usr.IsArchived,
                                        IsDeleted = usr.IsDeleted,
                                        IsLicenceAccepted = usr.IsLicenceAccepted,
                                        LastUpdatedBy = usr.LastUpdatedBy,
                                        Password = usr.Password,
                                        Phone = usr.Phone,
                                        RoleID = usr.RoleId,
                                        RoleName = string.Empty,
                                        Updated = usr.Updated,
                                        UpdatedByName = usr_usrupd.UserName,
                                        UserName = usr.UserName,
                                        UserType = usr.UserType ?? 0,
                                        UserTypeName = string.Empty,

                                    }).FirstOrDefault();
            }
            if (objUserMasterDTO != null && !string.IsNullOrWhiteSpace(objUserMasterDTO.EnterpriseDbName))
            {
                eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(objUserMasterDTO.EnterpriseDbName);
                using (var context = new eTurnsEntities(objUserMasterDAL.DataBaseEntityConnectionString))
                {
                    objLocalUserMasterDTO = (from usr in context.UserMasters
                                             join cm in context.CompanyMasters on usr.CompanyID equals cm.ID into usr_cm_join
                                             from usr_cm in usr_cm_join.DefaultIfEmpty()
                                             join rm in context.RoleMasters on new { rlid = usr.RoleId, ut = (usr.UserType ?? 0) > 1 } equals new { rlid = rm.ID, ut = true } into usr_rm_join
                                             from usr_rm in usr_rm_join.DefaultIfEmpty()
                                             where usr.ID == objUserMasterDTO.ID
                                             select new UserMasterDTO
                                             {
                                                 CompanyID = usr.CompanyID,
                                                 CompanyName = usr_cm.Name,
                                                 Created = usr.Created ?? DateTime.MinValue,
                                                 CreatedBy = usr.CreatedBy,
                                                 Email = usr.Email,
                                                 GUID = usr.GUID,
                                                 ID = usr.ID,
                                                 IsArchived = usr.IsArchived,
                                                 IsDeleted = usr.IsDeleted,
                                                 IsLicenceAccepted = usr.IsLicenceAccepted,
                                                 LastUpdatedBy = usr.LastUpdatedBy,
                                                 Password = usr.Password,
                                                 Phone = usr.Phone,
                                                 RoleID = usr.RoleId,
                                                 RoleName = string.Empty,
                                                 Updated = usr.Updated,
                                                 UserName = usr.UserName,
                                                 UserType = usr.UserType ?? 0,
                                                 UserTypeName = string.Empty
                                             }).FirstOrDefault();
                }
            }
            if (objLocalUserMasterDTO != null && objLocalUserMasterDTO.ID > 0 && objLocalUserMasterDTO.UserType > 1)
            {
                objUserMasterDTO.CompanyName = objLocalUserMasterDTO.CompanyName;
                objUserMasterDTO.RoleName = objLocalUserMasterDTO.RoleName;
            }
            return objUserMasterDTO;
        }
        public bool DeleteRecords(string IDs, Int64 userid)
        {
            if (!string.IsNullOrWhiteSpace(IDs))
            {
                string[] arrIds = new string[] { };
                arrIds = IDs.Split(',');
                string userIdsForMaster = string.Empty;
                EnterpriseMasterDAL enterPriseMasterDAL = new EnterpriseMasterDAL();
                long UserId = 0, EnterpriseId = 0;
                List<long> enterpriseIds = new List<long>();
                Dictionary<long, string> entWiseUserIds = new Dictionary<long, string>();
                
                foreach (var item in arrIds)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        string[] arrSeperated = item.Split('_');
                        if (arrSeperated.Length > 0)
                        {
                            long.TryParse(arrSeperated[0], out UserId);
                        }
                        if (arrSeperated.Length > 1)
                        {
                            long.TryParse(arrSeperated[1], out EnterpriseId);
                        }                        
                        
                        if (UserId > 0)
                        {
                            userIdsForMaster += arrSeperated[0] + ",";
                        }
                        
                        if (EnterpriseId > 0)
                        {
                            enterpriseIds.Add(EnterpriseId);
                        
                            if (UserId > 0)
                            {
                                if (entWiseUserIds.ContainsKey(EnterpriseId))
                                {
                                    string tmpUserIds = entWiseUserIds[EnterpriseId];
                                    tmpUserIds += arrSeperated[0] + ",";
                                    entWiseUserIds[EnterpriseId] = tmpUserIds;
                                }
                                else
                                {
                                    entWiseUserIds[EnterpriseId] = arrSeperated[0] + ","; 
                                }
                            }
                        }
                    }
                }

                if (enterpriseIds.Any())
                {
                    enterpriseIds = enterpriseIds.Distinct().ToList();
                }

                Dictionary<long, string> enterpriseList = enterPriseMasterDAL.GetEnterpriseListWithDBName(enterpriseIds);
                eTurnsMaster.DAL.UserMasterDAL emUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                emUserMasterDAL.DeleteUserByIds(userIdsForMaster,SessionHelper.UserID);
                
                foreach (KeyValuePair<long, string> enterprise in enterpriseList.Where(e => !string.IsNullOrEmpty(e.Value)))
                {
                    if (entWiseUserIds.ContainsKey(enterprise.Key) && !string.IsNullOrEmpty(entWiseUserIds[enterprise.Key]) 
                        && !string.IsNullOrWhiteSpace(entWiseUserIds[enterprise.Key]))
                    {
                        var userMasterDAL = new eTurns.DAL.UserMasterDAL(enterprise.Value);
                        userMasterDAL.DeleteUserByIds(entWiseUserIds[enterprise.Key], SessionHelper.UserID);
                    }
                }
            }
            
            return true;
        }
        public bool UnDeleteRecords(string IDs, Int64 userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()) && item.Trim() != userid.ToString())
                    {
                        strQuery += "UPDATE UserMaster SET Updated = '" + DateTimeUtility.DateTimeNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE ID =" + item.ToString().Trim() + ";";
                    }
                }

                if (!string.IsNullOrEmpty(strQuery))
                    context.Database.ExecuteSqlCommand(strQuery);
            }
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()) && item.Trim() != userid.ToString())
                    {
                        strQuery += "UPDATE UserMaster SET Updated = '" + DateTimeUtility.DateTimeNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE ID =" + item.ToString().Trim() + ";";
                    }
                }

                if (!string.IsNullOrEmpty(strQuery))
                    context.Database.ExecuteSqlCommand(strQuery);
            }
            return true;
        }

        public RoleWiseRoomsAccessDetailsDTO RoleWiseRoomAccessDetail(Int64 RoleID, int UserType, string EnterpriseDBName = "")
        {
            if (UserType == 1)
            {

                eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL();
                RoleMasterDTO objDTO = obj.GetRecord(RoleID);

                var objMasterList = (from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objDTO.PermissionList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                return objRoomAccessDTO;
            }
            else
            {
                eTurns.DAL.RoleMasterDAL obj;
                if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                    obj = new eTurns.DAL.RoleMasterDAL(EnterpriseDBName);
                else
                    obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

                RoleMasterDTO objDTO = obj.GetRecord(RoleID);


                var objMasterList = (from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objDTO.PermissionList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                return objRoomAccessDTO;
            }
        }

        public RoleWiseRoomsAccessDetailsDTO RoleWiseRoomAccessDetail(Int64 RoleID, int UserType, long EnterpriseID, long CompanyID, long RoomID, string EnterpriseDBName = "")
        {
            if (UserType == 1)
            {

                eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL();
                RoleMasterDTO objDTO = obj.GetRecord(RoleID, EnterpriseID, CompanyID, RoomID);
                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();

                if (objDTO != null)
                {
                    var objMasterList = (from t in objDTO.PermissionList
                                         where t.GroupId.ToString() == "1" && t.IsModule == true
                                         select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                    var objOtherModuleList = (from t in objDTO.PermissionList
                                              where t.GroupId.ToString() == "2" && t.IsModule == true
                                              select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                    var objNonModuleList = (from t in objDTO.PermissionList
                                            where t.IsModule == false && t.GroupId.ToString() != "4"
                                            select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                    var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                                   where t.GroupId.ToString() == "4" && t.IsModule == false
                                                   select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                    objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                    objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                    objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                    objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                    objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                }
                
                return objRoomAccessDTO;
            }
            else
            {
                eTurns.DAL.RoleMasterDAL obj = null;
                if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                {
                    obj = new eTurns.DAL.RoleMasterDAL(EnterpriseDBName);
                }
                else if (EnterpriseID > 0)
                {
                    EnterpriseMasterDAL enterpriseMasterDAL = new EnterpriseMasterDAL();
                    var enterpriseDTO = enterpriseMasterDAL.GetNonDeletedEnterpriseByIdPlain(EnterpriseID);
                    if (enterpriseDTO != null)
                    {
                        obj = new eTurns.DAL.RoleMasterDAL(enterpriseDTO.EnterpriseDBName);
                    }
                }

                if(obj == null)
                {
                    obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                }

                RoleMasterDTO objDTO = obj.GetRecord(RoleID, EnterpriseID, CompanyID, RoomID);
                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();

                if (objDTO != null)
                {
                    var objMasterList = (from t in objDTO.PermissionList
                                         where t.GroupId.ToString() == "1" && t.IsModule == true
                                         select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                    var objOtherModuleList = (from t in objDTO.PermissionList
                                              where t.GroupId.ToString() == "2" && t.IsModule == true
                                              select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                    var objNonModuleList = (from t in objDTO.PermissionList
                                            where t.IsModule == false && t.GroupId.ToString() != "4"
                                            select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                    var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                                   where t.GroupId.ToString() == "4" && t.IsModule == false
                                                   select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                    objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                    objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                    objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                    objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                    objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                }

                return objRoomAccessDTO;
            }
        }

        public List<UserMasterDTO> GetPagedUsersBySQlHelper(int UserType, Int64 EnterpriseId, Int64 RoomID, Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, long LoggedInUserId, long RoleID)
        {
            string RoleIds = string.Empty;
            string UserCreaters = string.Empty;
            string UserUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            string RoomIds = string.Empty;
            string CompanyIds = string.Empty;
            string EnterpriseIds = string.Empty;
            string UserTypes = string.Empty;
            List<RolePermissionInfo> outlstAllPermissions = new List<RolePermissionInfo>();
            List<UserMasterDTO> AllUser = new List<UserMasterDTO>();
            List<UserMasterDTO> PagedUsers = new List<UserMasterDTO>();
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
            string UDF1 = string.Empty;
            string UDF2 = string.Empty;
            string UDF3 = string.Empty;
            string UDF4 = string.Empty;
            string UDF5 = string.Empty;
            string UDF6 = string.Empty;
            string UDF7 = string.Empty;
            string UDF8 = string.Empty;
            string UDF9 = string.Empty;
            string UDF10 = string.Empty;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                AllUser = objUserMasterDAL.GetPagedUsers(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, IsArchived, IsDeleted, LoggedInUserId, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, CompanyIds, RoomIds, UserTypes, RoleIds, UDF1, UDF2, UDF3, UDF4, UDF5, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    UserCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UserUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    UDF1 = FieldsPara[4].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    UDF2 = FieldsPara[5].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    UDF3 = FieldsPara[6].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    UDF4 = FieldsPara[7].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    UDF5 = FieldsPara[8].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[92]))
                {
                    UDF6 = FieldsPara[92].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[93]))
                {
                    UDF7 = FieldsPara[93].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[94]))
                {
                    UDF8 = FieldsPara[94].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[95]))
                {
                    UDF9 = FieldsPara[95].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[96]))
                {
                    UDF10 = FieldsPara[96].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[39]))
                {
                    UserTypes = Convert.ToString(FieldsPara[39]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[41]))
                {
                    string[] arrReplenishTypes = FieldsPara[41].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        RoleIds = RoleIds + supitem + ",";
                    }
                    RoleIds = RoleIds.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                string filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[40]))
                {
                    string[] value = FieldsPara[40].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                RoomIds = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[42]))
                {
                    string[] value = FieldsPara[42].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                CompanyIds = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[79]))
                {
                    string[] value = FieldsPara[79].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                EnterpriseIds = filter;
                AllUser = objUserMasterDAL.GetPagedUsers(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, IsArchived, IsDeleted, LoggedInUserId, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, CompanyIds, RoomIds, UserTypes, RoleIds, UDF1, UDF2, UDF3, UDF4, UDF5, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else
            {
                AllUser = objUserMasterDAL.GetPagedUsers(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, IsArchived, IsDeleted, LoggedInUserId, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, CompanyIds, RoomIds, UserTypes, RoleIds, UDF1, UDF2, UDF3, UDF4, UDF5, UDF6, UDF7, UDF8, UDF9, UDF10);
            }

            return AllUser;
            //return AllUser;
        }

        public List<UserMasterDTO> GetPagedUsersBySQlHelperOld(int UserType, Int64 EnterpriseId, Int64 RoomID, Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, long LoggedInUserId, long RoleID)
        {
            string RoleIds = string.Empty;
            string UserCreaters = string.Empty;
            string UserUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            string RoomIds = string.Empty;
            string CompanyIds = string.Empty;
            string EnterpriseIds = string.Empty;
            string UserTypes = string.Empty;
            List<RolePermissionInfo> outlstAllPermissions = new List<RolePermissionInfo>();
            List<UserMasterDTO> AllUser = new List<UserMasterDTO>();
            List<UserMasterDTO> PagedUsers = new List<UserMasterDTO>();

            if (String.IsNullOrEmpty(SearchTerm))
            {
                AllUser = GetAllUsersBySQlHelper(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted, LoggedInUserId, RoleID, RoleIds, CompanyIds, RoomIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, out outlstAllPermissions);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    UserCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UserUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[39]))
                {
                    UserTypes = Convert.ToString(FieldsPara[39]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[41]))
                {
                    string[] arrReplenishTypes = FieldsPara[41].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        RoleIds = RoleIds + supitem + "','";
                    }
                    RoleIds = RoleIds.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                string filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[40]))
                {
                    string[] value = FieldsPara[40].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                RoomIds = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[42]))
                {
                    string[] value = FieldsPara[42].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                CompanyIds = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[79]))
                {
                    string[] value = FieldsPara[79].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                EnterpriseIds = filter;
                AllUser = GetAllUsersBySQlHelper(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted, LoggedInUserId, RoleID, RoleIds, RoomIds, CompanyIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, out outlstAllPermissions);
                AllUser = AllUser.Where(t => t.ID.ToString().Contains(SearchTerm)
                                      || (t.RoleName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                      || (t.UserName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                      || (t.Email ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                      || (t.CreatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                      || (t.UpdatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                      || (t.UserTypeName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                      ).ToList();
            }
            else
            {
                AllUser = GetAllUsersBySQlHelper(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted, LoggedInUserId, RoleID, RoleIds, RoomIds, CompanyIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, out outlstAllPermissions);
                AllUser = AllUser.Where(t => t.ID.ToString().Contains(SearchTerm)
                                       || (t.RoleName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                       || (t.UserName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                       || (t.Email ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                       || (t.CreatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                       || (t.UpdatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                       || (t.UserTypeName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                       ).ToList();
            }
            TotalCount = AllUser.Count();
            return AllUser.AsEnumerable<UserMasterDTO>().OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            //return AllUser;
        }

        public List<UserMasterDTO> GetAllUsersBySQlHelper(int UserType, long? EnterpriseId, long? CompanyId, long? RoomId, bool IsArchived, bool IsDeleted, long LoggedinUserId, long RoleID, string RoleIds, string RoomIds, string CompanyIds, string UserTypes, string UserCreaters, string UserUpdators, string CreatedDateFrom, string CreatedDateTo, string UpdatedDateFrom, string UpdatedDateTo, string EnterpriseIds, out List<RolePermissionInfo> outlstAllPermissions)
        {
            List<UserMasterDTO> lstUsers = new List<UserMasterDTO>();
            //List<UserMasterDTO> lstSuperUsers = new List<UserMasterDTO>();
            //List<UserMasterDTO> lstEnterpriseDbUsers = new List<UserMasterDTO>();
            //DataSet dsSuperUsers = new DataSet();
            DataSet dsUsers = new DataSet();
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            //SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string sqlConnectionString = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsMasterDbConnection = new SqlConnection(sqlConnectionString);

            dsUsers = eTurnsMaster.DAL.SqlHelper.ExecuteDataset(EturnsMasterDbConnection, "GetAllUsers", UserType, CompanyId, RoleID, RoleIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, EnterpriseId, LoggedinUserId);
            if (dsUsers != null && dsUsers.Tables.Count > 0)
            {
                DataTable dtSuperUsers = dsUsers.Tables[0];
                lstUsers = (from usr in dtSuperUsers.AsEnumerable()
                            select new UserMasterDTO
                            {
                                ID = usr.Field<long>("ID"),
                                CompanyID = usr.Field<long>("CompanyID"),
                                UserRoomAccess = usr.Field<string>("UserRoomAccess"),
                                Created = usr.Field<DateTime>("Created"),
                                CreatedBy = usr.Field<long?>("CreatedBy"),
                                CreatedByName = usr.Field<string>("CreatedByName"),
                                Email = usr.Field<string>("Email"),
                                GUID = usr.Field<Guid>("GUID"),
                                IsArchived = usr.Field<bool?>("IsArchived"),
                                IsDeleted = usr.Field<bool?>("IsDeleted"),
                                LastUpdatedBy = usr.Field<long?>("LastUpdatedBy"),
                                Password = usr.Field<string>("Password"),
                                Phone = usr.Field<string>("Phone"),
                                RoleID = usr.Field<long>("RoleID"),
                                RoleName = usr.Field<string>("RoleName"),
                                Updated = usr.Field<DateTime?>("Updated"),
                                UpdatedByName = usr.Field<string>("UpdatedByName"),
                                UserName = usr.Field<string>("UserName"),
                                UserType = usr.Field<int>("UserType"),
                                UserTypeName = usr.Field<string>("UserTypeName"),
                                EnterpriseId = usr.Field<long>("EnterpriseID"),
                                EnterpriseName = usr.Field<string>("EnterpriseName"),
                                FirstLicenceAccept = usr.Field<DateTime?>("FirstLicenceAccept"),
                                LastLicenceAccept = usr.Field<DateTime?>("LastLicenceAccept"),
                                Acceptcount = usr.Field<int>("Acceptcount")

                            }).ToList();
            }

            //dsUsers = eTurnsMaster.DAL.SqlHelper.ExecuteDataset(EturnsConnection, "GetAllUsers", UserType, CompanyId, RoleID, RoleIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived);
            //if (dsUsers != null && dsUsers.Tables.Count > 0)
            //{
            //    DataTable dtEnterpriseDbUsers = dsUsers.Tables[0];
            //    lstEnterpriseDbUsers = (from usr in dtEnterpriseDbUsers.AsEnumerable()
            //                            select new UserMasterDTO
            //                            {
            //                                ID = usr.Field<long>("ID"),
            //                                CompanyID = usr.Field<long>("CompanyID"),
            //                                UserRoomAccess = usr.Field<string>("UserRoomAccess"),
            //                                Created = usr.Field<DateTime>("Created"),
            //                                CreatedBy = usr.Field<long?>("CreatedBy"),
            //                                CreatedByName = usr.Field<string>("CreatedByName"),
            //                                Email = usr.Field<string>("Email"),
            //                                GUID = usr.Field<Guid>("GUID"),
            //                                IsArchived = usr.Field<bool?>("IsArchived"),
            //                                IsDeleted = usr.Field<bool?>("IsDeleted"),
            //                                LastUpdatedBy = usr.Field<long?>("LastUpdatedBy"),
            //                                Password = usr.Field<string>("Password"),
            //                                Phone = usr.Field<string>("Phone"),
            //                                RoleID = usr.Field<long>("RoleID"),
            //                                RoleName = usr.Field<string>("RoleName"),
            //                                Updated = usr.Field<DateTime?>("Updated"),
            //                                UpdatedByName = usr.Field<string>("UpdatedByName"),
            //                                UserName = usr.Field<string>("UserName"),
            //                                UserType = usr.Field<int>("UserType"),
            //                                UserTypeName = usr.Field<string>("UserTypeName")

            //                            }).ToList();

            //}
            //lstUsers = lstSuperUsers.Union(lstEnterpriseDbUsers).ToList();
            List<RolePermissionInfo> AllUserpermission = new List<RolePermissionInfo>();

            lstUsers.ForEach(t =>
            {
                if (!string.IsNullOrWhiteSpace(t.UserRoomAccess))
                {
                    List<RolePermissionInfo> lstRoomInfo = new List<RolePermissionInfo>();
                    string[] strarr = t.UserRoomAccess.Split(',');
                    foreach (string Combineditem in strarr)
                    {
                        RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                        string[] arrinner = Combineditem.Split('|');
                        objRolePermissionInfo.EnterPriseId = Convert.ToInt64(arrinner[0]);
                        objRolePermissionInfo.CompanyId = Convert.ToInt64(arrinner[1]);
                        objRolePermissionInfo.RoomId = Convert.ToInt64(arrinner[2]);
                        objRolePermissionInfo.UserId = t.ID;
                        lstRoomInfo.Add(objRolePermissionInfo);
                        AllUserpermission.Add(objRolePermissionInfo);
                    }
                    t.lstUserPermissions = lstRoomInfo;
                }
            });
            outlstAllPermissions = AllUserpermission;


            List<UserAccessDTO> oUserAccessList = new List<UserAccessDTO>();
            if (HttpContext.Current.Session["AllUserPermissions"] != null)
            {
                oUserAccessList = (List<UserAccessDTO>)HttpContext.Current.Session["AllUserPermissions"];
            }
            else
            {
                oUserAccessList = GetUserAccessForUserList(SessionHelper.UserType, SessionHelper.RoleID, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID);
                HttpContext.Current.Session["AllUserPermissions"] = oUserAccessList;
            }

            if (!string.IsNullOrWhiteSpace(CompanyIds))
            {
                List<UserAccessDTO> lstNarrowSearchList = new List<UserAccessDTO>();
                string[] strarr = CompanyIds.Split(',');
                foreach (string Combineditem in strarr)
                {
                    UserAccessDTO objRolePermissionInfo = new UserAccessDTO();
                    string[] arrinner = Combineditem.Split('_');
                    objRolePermissionInfo.EnterpriseId = Convert.ToInt64(arrinner[0]);
                    objRolePermissionInfo.CompanyId = Convert.ToInt64(arrinner[1]);
                    //objRolePermissionInfo.RoomId = Convert.ToInt64(arrinner[2]);
                    lstNarrowSearchList.Add(objRolePermissionInfo);
                }

                long[] UserIds = (from filterroom in lstNarrowSearchList
                                  join allrooms in oUserAccessList on new { filterroom.EnterpriseId, filterroom.CompanyId } equals new { allrooms.EnterpriseId, allrooms.CompanyId }
                                  select allrooms.UserId).Distinct().ToArray();
                lstUsers = (from und in lstUsers
                            where UserIds.Contains(und.ID)
                            select und).ToList();

            }

            if (!string.IsNullOrWhiteSpace(RoomIds))
            {
                List<UserAccessDTO> lstNarrowSearchList = new List<UserAccessDTO>();
                string[] strarr = RoomIds.Split(',');
                foreach (string Combineditem in strarr)
                {
                    UserAccessDTO objRolePermissionInfo = new UserAccessDTO();
                    string[] arrinner = Combineditem.Split('_');
                    objRolePermissionInfo.EnterpriseId = Convert.ToInt64(arrinner[0]);
                    objRolePermissionInfo.CompanyId = Convert.ToInt64(arrinner[1]);
                    objRolePermissionInfo.RoomId = Convert.ToInt64(arrinner[2]);
                    lstNarrowSearchList.Add(objRolePermissionInfo);
                }

                long[] UserIds = (from filterroom in lstNarrowSearchList
                                  join allrooms in oUserAccessList on new { filterroom.EnterpriseId, filterroom.CompanyId, filterroom.RoomId } equals new { allrooms.EnterpriseId, allrooms.CompanyId, allrooms.RoomId }
                                  select allrooms.UserId).Distinct().ToArray();
                lstUsers = (from und in lstUsers
                            where UserIds.Contains(und.ID)
                            select und).ToList();

            }

            if (!string.IsNullOrWhiteSpace(EnterpriseIds))
            {
                List<UserAccessDTO> lstNarrowSearchList = new List<UserAccessDTO>();
                string[] strarr = EnterpriseIds.Split(',');
                foreach (string Combineditem in strarr)
                {
                    UserAccessDTO objRolePermissionInfo = new UserAccessDTO();
                    objRolePermissionInfo.EnterpriseId = Convert.ToInt64(Combineditem);
                    lstNarrowSearchList.Add(objRolePermissionInfo);
                }

                long[] UserIds = (from filterroom in lstNarrowSearchList
                                  join allrooms in oUserAccessList on new { filterroom.EnterpriseId } equals new { allrooms.EnterpriseId }
                                  select allrooms.UserId).Distinct().ToArray();
                lstUsers = (from und in lstUsers
                            where UserIds.Contains(und.ID)
                            select und).ToList();

            }

            return lstUsers;
        }

        public List<RoleMasterDTO> GetPagedRolesBySQlHelper(int UserType, Int64 EnterpriseId, Int64 RoomID, Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, long LoggedInUserId, long RoleID)
        {
            string RoleIds = string.Empty;
            string UserCreaters = string.Empty;
            string UserUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            string RoomIds = string.Empty;
            string CompanyIds = string.Empty;
            string EnterpriseIds = string.Empty;
            string UserTypes = string.Empty;
            List<RoleMasterDTO> AllRoles = new List<RoleMasterDTO>();
            eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL();

            if (String.IsNullOrEmpty(SearchTerm))
            {
                AllRoles = objRoleMasterDAL.GetPagedRoles(EnterpriseId, CompanyID, RoomID, StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, IsArchived, IsDeleted, LoggedInUserId, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, CompanyIds, RoomIds, UserTypes, RoleIds);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    UserCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UserUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[36]))
                {
                    UserTypes = Convert.ToString(FieldsPara[36]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[41]))
                {
                    string[] arrReplenishTypes = FieldsPara[41].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        RoleIds = RoleIds + supitem + "','";
                    }
                    RoleIds = RoleIds.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                string filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[43]))
                {
                    string[] value = FieldsPara[43].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                RoomIds = filter;

                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[44]))
                {
                    string[] value = FieldsPara[44].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                CompanyIds = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[98]))
                {
                    string[] value = FieldsPara[98].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                EnterpriseIds = filter;
                AllRoles = objRoleMasterDAL.GetPagedRoles(EnterpriseId, CompanyID, RoomID, StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, IsArchived, IsDeleted, LoggedInUserId, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, CompanyIds, RoomIds, UserTypes, RoleIds);
            }
            else
            {
                AllRoles = objRoleMasterDAL.GetPagedRoles(EnterpriseId, CompanyID, RoomID, StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, IsArchived, IsDeleted, LoggedInUserId, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, EnterpriseIds, CompanyIds, RoomIds, UserTypes, RoleIds);
                AllRoles = AllRoles.Where(t => t.ID.ToString().Contains(SearchTerm)
                                        || (t.RoleName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        || (t.CreatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        || (t.UpdatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        || (t.UserTypeName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        ).ToList();
            }
            return AllRoles;
        }

        public List<RoleMasterDTO> GetPagedRolesBySQlHelperOld(int UserType, Int64 EnterpriseId, Int64 RoomID, Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, long LoggedInUserId, long RoleID)
        {
            string RoleIds = string.Empty;
            string UserCreaters = string.Empty;
            string UserUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            string RoomIds = string.Empty;
            string CompanyIds = string.Empty;
            string EnterpriseIds = string.Empty;
            string UserTypes = string.Empty;
            List<RolePermissionInfo> outlstAllPermissions = new List<RolePermissionInfo>();
            List<RoleMasterDTO> AllUser = new List<RoleMasterDTO>();
            List<RoleMasterDTO> PagedUsers = new List<RoleMasterDTO>();

            if (String.IsNullOrEmpty(SearchTerm))
            {
                AllUser = GetAllRolesBySQlHelper(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted, LoggedInUserId, RoleID, RoleIds, RoomIds, CompanyIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, string.Empty, EnterpriseIds, out outlstAllPermissions);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    UserCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UserUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    // UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[36]))
                {
                    UserTypes = Convert.ToString(FieldsPara[36]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[41]))
                {
                    string[] arrReplenishTypes = FieldsPara[41].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        RoleIds = RoleIds + supitem + "','";
                    }
                    RoleIds = RoleIds.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                string filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[43]))
                {
                    string[] value = FieldsPara[43].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                RoomIds = filter;

                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[44]))
                {
                    string[] value = FieldsPara[44].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                CompanyIds = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[98]))
                {
                    string[] value = FieldsPara[98].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                EnterpriseIds = filter;

                AllUser = GetAllRolesBySQlHelper(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted, LoggedInUserId, RoleID, RoleIds, RoomIds, CompanyIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, SearchTerm, EnterpriseIds, out outlstAllPermissions);
            }
            else
            {
                AllUser = GetAllRolesBySQlHelper(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted, LoggedInUserId, RoleID, RoleIds, RoomIds, CompanyIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, SearchTerm, EnterpriseIds, out outlstAllPermissions);
                AllUser = AllUser.Where(t => t.ID.ToString().Contains(SearchTerm)
                                        || (t.RoleName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        || (t.CreatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        || (t.UpdatedByName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        || (t.UserTypeName ?? string.Empty).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                                        ).ToList();
            }
            TotalCount = AllUser.Count();
            return AllUser.AsEnumerable<RoleMasterDTO>().OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            //return AllUser;
        }

        public List<RoleMasterDTO> GetAllRolesBySQlHelper(int UserType, long? EnterpriseId, long? CompanyId, long? RoomId, bool IsArchived, bool IsDeleted, long LoggedinUserId, long RoleID, string RoleIds, string RoomIds, string CompanyIds, string UserTypes, string UserCreaters, string UserUpdators, string CreatedDateFrom, string CreatedDateTo, string UpdatedDateFrom, string UpdatedDateTo, string SearchTerm, string EnterpriseIds, out List<RolePermissionInfo> outlstAllPermissions)
        {
            List<RoleMasterDTO> lstUsers = new List<RoleMasterDTO>();
            List<RoleMasterDTO> lstSuperUsers = new List<RoleMasterDTO>();
            List<RoleMasterDTO> lstEnterpriseDbUsers = new List<RoleMasterDTO>();
            DataSet dsSuperUsers = new DataSet();
            DataSet dsUsers = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string sqlConnectionString = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsMasterDbConnection = new SqlConnection(sqlConnectionString);
            //if (UserType == 1 && RoleID == -1)
            //{
            dsSuperUsers = eTurnsMaster.DAL.SqlHelper.ExecuteDataset(EturnsMasterDbConnection, "GetPagedRoles", UserType, CompanyId, RoleID, RoleIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, SearchTerm, LoggedinUserId);
            if (dsSuperUsers != null && dsSuperUsers.Tables.Count > 0)
            {
                DataTable dtSuperUsers = dsSuperUsers.Tables[0];
                lstSuperUsers = (from usr in dtSuperUsers.AsEnumerable()
                                 select new RoleMasterDTO
                                 {
                                     ID = usr.Field<long>("ID"),
                                     RoleRoomAccess = usr.Field<string>("UserRoomAccess"),
                                     Created = usr.Field<DateTime?>("Created"),
                                     CreatedBy = usr.Field<long?>("CreatedBy"),
                                     CreatedByName = usr.Field<string>("CreatedByName"),
                                     GUID = usr.Field<Guid>("GUID"),
                                     IsArchived = usr.Field<bool>("IsArchived"),
                                     IsDeleted = usr.Field<bool>("IsDeleted"),
                                     LastUpdatedBy = usr.Field<long?>("LastUpdatedBy"),
                                     RoleName = usr.Field<string>("RoleName"),
                                     Updated = usr.Field<DateTime?>("Updated"),
                                     UpdatedByName = usr.Field<string>("UpdatedByName"),
                                     UserType = usr.Field<int>("UserType"),
                                     UserTypeName = usr.Field<string>("UserTypeName"),
                                     CreatedDate = string.Empty,
                                     UpdatedDate = string.Empty,
                                     EnterpriseId = usr.Field<long>("EnterpriseId"),
                                     CompanyID = usr.Field<long>("CompanyId"),
                                     Room = usr.Field<long>("RoomId"),
                                 }).ToList();
            }
            //  }
            //dsUsers = eTurnsMaster.DAL.SqlHelper.ExecuteDataset(EturnsConnection, "GetAllRoles", UserType, CompanyId, RoleID, RoleIds, UserTypes, UserCreaters, UserUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, SearchTerm);
            //if (dsUsers != null && dsUsers.Tables.Count > 0)
            //{
            //    DataTable dtEnterpriseDbUsers = dsUsers.Tables[0];
            //    lstEnterpriseDbUsers = (from usr in dtEnterpriseDbUsers.AsEnumerable()
            //                            select new RoleMasterDTO
            //                            {
            //                                ID = usr.Field<long>("ID"),
            //                                CompanyID = usr.Field<long>("CompanyID"),
            //                                RoleRoomAccess = usr.Field<string>("UserRoomAccess"),
            //                                Created = usr.Field<DateTime?>("Created"),
            //                                CreatedBy = usr.Field<long?>("CreatedBy"),
            //                                CreatedByName = usr.Field<string>("CreatedByName"),
            //                                GUID = usr.Field<Guid>("GUID"),
            //                                IsArchived = usr.Field<bool>("IsArchived"),
            //                                IsDeleted = usr.Field<bool>("IsDeleted"),
            //                                LastUpdatedBy = usr.Field<long?>("LastUpdatedBy"),
            //                                RoleName = usr.Field<string>("RoleName"),
            //                                Updated = usr.Field<DateTime?>("Updated"),
            //                                UpdatedByName = usr.Field<string>("UpdatedByName"),
            //                                UserType = usr.Field<int>("UserType"),
            //                                UserTypeName = usr.Field<string>("UserTypeName"),
            //                                CreatedDate = string.Empty,
            //                                UpdatedDate = string.Empty,
            //                            }).ToList();

            //}
            // lstUsers = lstSuperUsers.Union(lstEnterpriseDbUsers).ToList();

            lstUsers = lstSuperUsers.ToList();
            List<RolePermissionInfo> AllUserpermission = new List<RolePermissionInfo>();

            lstUsers.ForEach(t =>
            {
                if (!string.IsNullOrWhiteSpace(t.RoleRoomAccess))
                {
                    List<RolePermissionInfo> lstRoomInfo = new List<RolePermissionInfo>();
                    string[] strarr = t.RoleRoomAccess.Split(',');
                    int i = 0;

                    while (i < strarr.Length)
                    {
                        RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                        string[] arrinner = strarr[i].Split('|');
                        if (arrinner.Length >= 3)
                        {
                            if (arrinner[0].Split('#').Length >= 2 && arrinner[1].Split('#').Length >= 2 && arrinner[2].Split('#').Length >= 2)
                            {
                                objRolePermissionInfo.EnterPriseId = Convert.ToInt64(arrinner[0].Split('#')[0]);
                                objRolePermissionInfo.CompanyId = Convert.ToInt64(arrinner[1].Split('#')[0]);
                                objRolePermissionInfo.RoomId = Convert.ToInt64(arrinner[2].Split('#')[0]);

                                objRolePermissionInfo.EnterPriseName = Convert.ToString(arrinner[0].Split('#')[1]);
                                objRolePermissionInfo.CompanyName = Convert.ToString(arrinner[1].Split('#')[1]);
                                objRolePermissionInfo.RoomName = Convert.ToString(arrinner[2].Split('#')[1]);
                            }
                        }

                        objRolePermissionInfo.RoleId = t.ID;
                        objRolePermissionInfo.RoleUserType = t.UserType;
                        lstRoomInfo.Add(objRolePermissionInfo);
                        AllUserpermission.Add(objRolePermissionInfo);
                        i++;
                    }
                    t.lstRolePermissions = lstRoomInfo;
                }
            });
            outlstAllPermissions = AllUserpermission;
            if (!string.IsNullOrWhiteSpace(CompanyIds))
            {
                List<RolePermissionInfo> lstNarrowSearchList = new List<RolePermissionInfo>();
                string[] strarr = CompanyIds.Split(',');
                foreach (string Combineditem in strarr)
                {
                    RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                    string[] arrinner = Combineditem.Split('_');
                    objRolePermissionInfo.EnterPriseId = Convert.ToInt64(arrinner[0]);
                    objRolePermissionInfo.CompanyId = Convert.ToInt64(arrinner[1]);
                    lstNarrowSearchList.Add(objRolePermissionInfo);
                }

                long[] UserIds = (from filterroom in lstNarrowSearchList
                                  join allrooms in AllUserpermission on new { filterroom.EnterPriseId, filterroom.CompanyId } equals new { allrooms.EnterPriseId, allrooms.CompanyId }
                                  select allrooms.RoleId).Distinct().ToArray();
                lstUsers = (from und in lstUsers
                            where UserIds.Contains(und.ID)
                            select und).ToList();

            }
            if (!string.IsNullOrWhiteSpace(EnterpriseIds))
            {
                List<RolePermissionInfo> lstNarrowSearchList = new List<RolePermissionInfo>();
                string[] strarr = EnterpriseIds.Split(',');
                foreach (string Combineditem in strarr)
                {
                    RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                    //string[] arrinner = Combineditem.Split('_');
                    objRolePermissionInfo.EnterPriseId = Convert.ToInt64(Combineditem);
                    //objRolePermissionInfo.CompanyId = Convert.ToInt64(arrinner[1]);
                    lstNarrowSearchList.Add(objRolePermissionInfo);
                }

                long[] UserIds = (from filterroom in lstNarrowSearchList
                                  join allrooms in AllUserpermission on new { filterroom.EnterPriseId } equals new { allrooms.EnterPriseId }
                                  select allrooms.RoleId).Distinct().ToArray();
                lstUsers = (from und in lstUsers
                            where UserIds.Contains(und.ID)
                            select und).ToList();

            }

            if (!string.IsNullOrWhiteSpace(RoomIds))
            {
                List<RolePermissionInfo> lstNarrowSearchList = new List<RolePermissionInfo>();
                string[] strarr = RoomIds.Split(',');
                foreach (string Combineditem in strarr)
                {
                    RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                    string[] arrinner = Combineditem.Split('_');
                    objRolePermissionInfo.EnterPriseId = Convert.ToInt64(arrinner[0]);
                    objRolePermissionInfo.CompanyId = Convert.ToInt64(arrinner[1]);
                    objRolePermissionInfo.RoomId = Convert.ToInt64(arrinner[2]);
                    lstNarrowSearchList.Add(objRolePermissionInfo);
                }

                long[] UserIds = (from filterroom in lstNarrowSearchList
                                  join allrooms in AllUserpermission on new { filterroom.EnterPriseId, filterroom.CompanyId, filterroom.RoomId } equals new { allrooms.EnterPriseId, allrooms.CompanyId, allrooms.RoomId }
                                  select allrooms.RoleId).Distinct().ToArray();
                lstUsers = (from und in lstUsers
                            where UserIds.Contains(und.ID)
                            select und).ToList();

            }
            return lstUsers;
        }


        public UserMasterDTO CheckAuthantication(string Email, string Password)
        {
            UserMasterDTO objUserMasterDTO = new UserMasterDTO();
            UserMasterDTO objLocalUserMasterDTO = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                objUserMasterDTO = (from usr in context.UserMasters
                                    join usrcrt in context.UserMasters on usr.CreatedBy equals usrcrt.ID into usr_usrcrt_join
                                    from usr_usrcrt in usr_usrcrt_join.DefaultIfEmpty()
                                    join usrupd in context.UserMasters on usr.LastUpdatedBy equals usrupd.ID into usr_usrupd_join
                                    from usr_usrupd in usr_usrupd_join.DefaultIfEmpty()
                                    join em in context.EnterpriseMasters on usr.EnterpriseId equals em.ID into usr_em_join
                                    from usr_em in usr_em_join.DefaultIfEmpty()
                                    join rm in context.RoleMasters on new { rlid = usr.RoleId, ut = (usr.UserType ?? 0) < 2 } equals new { rlid = rm.ID, ut = true } into usr_rm_join
                                    from usr_rm in usr_rm_join.DefaultIfEmpty()
                                    where usr.IsDeleted == false && usr.UserName == Email && usr.Password == Password
                                    select new UserMasterDTO
                                    {
                                        CompanyID = usr.CompanyID,
                                        CompanyName = string.Empty,
                                        Created = usr.Created ?? DateTime.MinValue,
                                        CreatedBy = usr.CreatedBy,
                                        CreatedByName = usr_usrcrt.UserName,
                                        Email = usr.Email,
                                        EnterpriseDbName = usr_em.EnterpriseDBName,
                                        EnterpriseId = usr.EnterpriseId,
                                        GUID = usr.GUID,
                                        ID = usr.ID,
                                        IsArchived = usr.IsArchived,
                                        IsDeleted = usr.IsDeleted,
                                        IsLicenceAccepted = usr.IsLicenceAccepted,
                                        LastUpdatedBy = usr.LastUpdatedBy,
                                        Password = usr.Password,
                                        Phone = usr.Phone,
                                        RoleID = usr.RoleId,
                                        RoleName = string.Empty,
                                        Updated = usr.Updated,
                                        UpdatedByName = usr_usrupd.UserName,
                                        UserName = usr.UserName,
                                        UserType = usr.UserType ?? 0,
                                        UserTypeName = string.Empty
                                    }).FirstOrDefault();
            }
            if (objUserMasterDTO != null && !string.IsNullOrWhiteSpace(objUserMasterDTO.EnterpriseDbName))
            {
                eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(objUserMasterDTO.EnterpriseDbName);
                using (var context = new eTurnsEntities(objUserMasterDAL.DataBaseEntityConnectionString))
                {
                    objLocalUserMasterDTO = (from usr in context.UserMasters
                                             join cm in context.CompanyMasters on usr.CompanyID equals cm.ID into usr_cm_join
                                             from usr_cm in usr_cm_join.DefaultIfEmpty()
                                             join rm in context.RoleMasters on new { rlid = usr.RoleId, ut = (usr.UserType ?? 0) > 1 } equals new { rlid = rm.ID, ut = true } into usr_rm_join
                                             from usr_rm in usr_rm_join.DefaultIfEmpty()
                                             where usr.ID == objUserMasterDTO.ID
                                             select new UserMasterDTO
                                             {
                                                 CompanyID = usr.CompanyID,
                                                 CompanyName = usr_cm.Name,
                                                 Created = usr.Created ?? DateTime.MinValue,
                                                 CreatedBy = usr.CreatedBy,
                                                 Email = usr.Email,
                                                 GUID = usr.GUID,
                                                 ID = usr.ID,
                                                 IsArchived = usr.IsArchived,
                                                 IsDeleted = usr.IsDeleted,
                                                 IsLicenceAccepted = usr.IsLicenceAccepted,
                                                 LastUpdatedBy = usr.LastUpdatedBy,
                                                 Password = usr.Password,
                                                 Phone = usr.Phone,
                                                 RoleID = usr.RoleId,
                                                 RoleName = string.Empty,
                                                 Updated = usr.Updated,
                                                 UserName = usr.UserName,
                                                 UserType = usr.UserType ?? 0,
                                                 UserTypeName = string.Empty
                                             }).FirstOrDefault();
                }
            }
            if (objLocalUserMasterDTO != null && objLocalUserMasterDTO.ID > 0 && objLocalUserMasterDTO.UserType > 1)
            {
                objUserMasterDTO.CompanyName = objLocalUserMasterDTO.CompanyName;
                objUserMasterDTO.RoleName = objLocalUserMasterDTO.RoleName;
            }
            return objUserMasterDTO;
        }

        public UserMasterDTO CheckAuthantication(string Email)
        {
            UserMasterDTO objUserMasterDTO = new UserMasterDTO();
            UserMasterDTO objLocalUserMasterDTO = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                objUserMasterDTO = (from usr in context.UserMasters
                                    join usrcrt in context.UserMasters on usr.CreatedBy equals usrcrt.ID into usr_usrcrt_join
                                    from usr_usrcrt in usr_usrcrt_join.DefaultIfEmpty()
                                    join usrupd in context.UserMasters on usr.LastUpdatedBy equals usrupd.ID into usr_usrupd_join
                                    from usr_usrupd in usr_usrupd_join.DefaultIfEmpty()
                                    join em in context.EnterpriseMasters on usr.EnterpriseId equals em.ID into usr_em_join
                                    from usr_em in usr_em_join.DefaultIfEmpty()
                                    join rm in context.RoleMasters on new { rlid = usr.RoleId, ut = (usr.UserType ?? 0) < 2 } equals new { rlid = rm.ID, ut = true } into usr_rm_join
                                    from usr_rm in usr_rm_join.DefaultIfEmpty()
                                    where usr.IsDeleted == false && usr.UserName == Email
                                    select new UserMasterDTO
                                    {
                                        CompanyID = usr.CompanyID,
                                        CompanyName = string.Empty,
                                        Created = usr.Created ?? DateTime.MinValue,
                                        CreatedBy = usr.CreatedBy,
                                        CreatedByName = usr_usrcrt.UserName,
                                        Email = usr.Email,
                                        EnterpriseDbName = usr_em.EnterpriseDBName,
                                        EnterpriseId = usr.EnterpriseId,
                                        GUID = usr.GUID,
                                        ID = usr.ID,
                                        IsArchived = usr.IsArchived,
                                        IsDeleted = usr.IsDeleted,
                                        IsLicenceAccepted = usr.IsLicenceAccepted,
                                        LastUpdatedBy = usr.LastUpdatedBy,
                                        Password = usr.Password,
                                        Phone = usr.Phone,
                                        RoleID = usr.RoleId,
                                        RoleName = string.Empty,
                                        Updated = usr.Updated,
                                        UpdatedByName = usr_usrupd.UserName,
                                        UserName = usr.UserName,
                                        UserType = usr.UserType ?? 0,
                                        UserTypeName = string.Empty
                                    }).FirstOrDefault();
            }
            if (objUserMasterDTO != null && !string.IsNullOrWhiteSpace(objUserMasterDTO.EnterpriseDbName))
            {
                eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(objUserMasterDTO.EnterpriseDbName);
                using (var context = new eTurnsEntities(objUserMasterDAL.DataBaseEntityConnectionString))
                {
                    objLocalUserMasterDTO = (from usr in context.UserMasters
                                             join cm in context.CompanyMasters on usr.CompanyID equals cm.ID into usr_cm_join
                                             from usr_cm in usr_cm_join.DefaultIfEmpty()
                                             join rm in context.RoleMasters on new { rlid = usr.RoleId, ut = (usr.UserType ?? 0) > 1 } equals new { rlid = rm.ID, ut = true } into usr_rm_join
                                             from usr_rm in usr_rm_join.DefaultIfEmpty()
                                             where usr.ID == objUserMasterDTO.ID
                                             select new UserMasterDTO
                                             {
                                                 CompanyID = usr.CompanyID,
                                                 CompanyName = usr_cm.Name,
                                                 Created = usr.Created ?? DateTime.MinValue,
                                                 CreatedBy = usr.CreatedBy,
                                                 Email = usr.Email,
                                                 GUID = usr.GUID,
                                                 ID = usr.ID,
                                                 IsArchived = usr.IsArchived,
                                                 IsDeleted = usr.IsDeleted,
                                                 IsLicenceAccepted = usr.IsLicenceAccepted,
                                                 LastUpdatedBy = usr.LastUpdatedBy,
                                                 Password = usr.Password,
                                                 Phone = usr.Phone,
                                                 RoleID = usr.RoleId,
                                                 RoleName = string.Empty,
                                                 Updated = usr.Updated,
                                                 UserName = usr.UserName,
                                                 UserType = usr.UserType ?? 0,
                                                 UserTypeName = string.Empty
                                             }).FirstOrDefault();
                }
            }
            if (objLocalUserMasterDTO != null && objLocalUserMasterDTO.ID > 0 && objLocalUserMasterDTO.UserType > 1)
            {
                objUserMasterDTO.CompanyName = objLocalUserMasterDTO.CompanyName;
                objUserMasterDTO.RoleName = objLocalUserMasterDTO.RoleName;
            }
            return objUserMasterDTO;
        }

        public void AddNewRoomPermissionsNew(long EnterpriseId, long CompanyID, long RoomId, long UserId, string MasterDBName)
        {
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                context.Database.ExecuteSqlCommand("EXEC csp_AddNewRoomPermissions " + EnterpriseId.ToString() + ", " + CompanyID.ToString() + ", " + RoomId.ToString() + ", " + UserId.ToString() + ", '" + MasterDBName + "'");
            }
        }

        public List<UserWiseRoomsAccessDetailsDTO> AddNewRoomPermissions(long EnterpriseId, long CompanyID, long RoomId, long UserId, long RoleId, long UserType)
        {
            List<UserWiseRoomsAccessDetailsDTO> lstPermissions = new List<UserWiseRoomsAccessDetailsDTO>();

            if (UserType == 1 && RoleId > 0)
            {
                using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    eTurnsMaster.DAL.UserRoleDetail objUserRoleDetail = new eTurnsMaster.DAL.UserRoleDetail();
                    eTurnsMaster.DAL.RoleModuleDetail objRoleModuleDetail = new eTurnsMaster.DAL.RoleModuleDetail();
                    context.ModuleMasters.ToList().ForEach(t =>
                    {
                        eTurnsMaster.DAL.UserRoleDetail oUserRoleDetail = context.UserRoleDetails.Where(x => x.UserID == UserId && x.RoleID == RoleId && x.ModuleID == t.ID && x.EnterpriseId == EnterpriseId && x.CompanyId == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (oUserRoleDetail == null)
                        {
                            objUserRoleDetail = new eTurnsMaster.DAL.UserRoleDetail();
                            objUserRoleDetail.CompanyId = CompanyID;
                            objUserRoleDetail.EnterpriseId = EnterpriseId;
                            objUserRoleDetail.GUID = Guid.NewGuid();
                            objUserRoleDetail.ID = 0;
                            objUserRoleDetail.IsChecked = true;
                            objUserRoleDetail.IsDelete = true;
                            objUserRoleDetail.IsInsert = true;
                            objUserRoleDetail.IsUpdate = true;
                            objUserRoleDetail.IsView = true;
                            objUserRoleDetail.ModuleID = t.ID;
                            objUserRoleDetail.ModuleValue = t.Value;
                            objUserRoleDetail.RoleID = RoleId;
                            objUserRoleDetail.RoomId = RoomId;
                            objUserRoleDetail.ShowArchived = true;
                            objUserRoleDetail.ShowDeleted = true;
                            objUserRoleDetail.ShowUDF = true;
                            objUserRoleDetail.UserID = UserId;
                            context.UserRoleDetails.Add(objUserRoleDetail);
                        }

                        eTurnsMaster.DAL.RoleModuleDetail objDetails = context.RoleModuleDetails.Where(x => x.RoleId == RoleId && x.ModuleID == t.ID && x.EnterpriseId == EnterpriseId && x.CompanyId == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (objDetails == null)
                        {
                            objRoleModuleDetail = new eTurnsMaster.DAL.RoleModuleDetail();
                            objRoleModuleDetail.CompanyId = CompanyID;
                            objRoleModuleDetail.EnterpriseId = EnterpriseId;
                            objRoleModuleDetail.GUID = Guid.NewGuid();
                            objRoleModuleDetail.ID = 0;
                            objRoleModuleDetail.IsChecked = true;
                            objRoleModuleDetail.IsDelete = true;
                            objRoleModuleDetail.IsInsert = true;
                            objRoleModuleDetail.IsUpdate = true;
                            objRoleModuleDetail.IsView = true;
                            objRoleModuleDetail.ModuleID = t.ID;
                            objRoleModuleDetail.ModuleValue = t.Value;
                            objRoleModuleDetail.RoleId = RoleId;
                            objRoleModuleDetail.RoomId = RoomId;
                            objRoleModuleDetail.ShowArchived = true;
                            objRoleModuleDetail.ShowDeleted = true;
                            objRoleModuleDetail.ShowUDF = true;
                            context.RoleModuleDetails.Add(objRoleModuleDetail);
                        }
                    });
                    eTurnsMaster.DAL.UserRoomAccess objUserRoomAccess = context.UserRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.UserId == UserId);
                    if (objUserRoomAccess == null)
                    {
                        objUserRoomAccess = new eTurnsMaster.DAL.UserRoomAccess();
                        objUserRoomAccess.CompanyId = CompanyID;
                        objUserRoomAccess.EnterpriseId = EnterpriseId;
                        objUserRoomAccess.ID = 0;
                        objUserRoomAccess.RoleId = RoleId;
                        objUserRoomAccess.RoomId = RoomId;
                        objUserRoomAccess.UserId = UserId;
                        context.UserRoomAccesses.Add(objUserRoomAccess);
                    }
                    else
                    {
                        objUserRoomAccess.RoomId = RoomId;
                    }
                    eTurnsMaster.DAL.RoleRoomAccess objRoleRoomAccess = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.RoleId == RoleId);
                    if (objRoleRoomAccess == null)
                    {
                        objRoleRoomAccess = new eTurnsMaster.DAL.RoleRoomAccess();
                        objRoleRoomAccess.CompanyId = CompanyID;
                        objRoleRoomAccess.EnterpriseId = EnterpriseId;
                        objRoleRoomAccess.ID = 0;
                        objRoleRoomAccess.RoleId = RoleId;
                        objRoleRoomAccess.RoomId = RoomId;
                        context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    }
                    else
                    {
                        objRoleRoomAccess.RoomId = RoomId;
                    }
                    context.SaveChanges();
                }

                if (UserId == SessionHelper.UserID)
                {
                    eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                    List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
                    string strRoomList = string.Empty;
                    lstPermissions = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
                }
            }
            else if ((UserType == 2 && RoleId > 0) || UserType == 3)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    eTurns.DAL.UserRoleDetail objUserRoleDetail = new eTurns.DAL.UserRoleDetail();
                    eTurns.DAL.RoleModuleDetail objRoleModuleDetail = new eTurns.DAL.RoleModuleDetail();
                    context.ModuleMasters.ToList().ForEach(t =>
                    {
                        eTurns.DAL.UserRoleDetail oUserRoleDetail = context.UserRoleDetails.Where(x => x.UserID == UserId && x.RoleID == RoleId && x.ModuleID == t.ID && x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (oUserRoleDetail == null)
                        {
                            objUserRoleDetail = new eTurns.DAL.UserRoleDetail();
                            objUserRoleDetail.CompanyID = CompanyID;
                            objUserRoleDetail.EnterpriseID = EnterpriseId;
                            objUserRoleDetail.GUID = Guid.NewGuid();
                            objUserRoleDetail.ID = 0;
                            objUserRoleDetail.IsChecked = true;
                            objUserRoleDetail.IsDelete = true;
                            objUserRoleDetail.IsInsert = true;
                            objUserRoleDetail.IsUpdate = true;
                            objUserRoleDetail.IsView = true;
                            objUserRoleDetail.ModuleID = t.ID;
                            objUserRoleDetail.ModuleValue = t.Value;
                            objUserRoleDetail.RoleID = RoleId;
                            objUserRoleDetail.RoomId = RoomId;
                            objUserRoleDetail.ShowArchived = true;
                            objUserRoleDetail.ShowDeleted = true;
                            objUserRoleDetail.ShowUDF = true;
                            objUserRoleDetail.UserID = UserId;
                            if (t.ID == (int)SessionHelper.ModuleList.AllowToEnterLotOrSerialInBlankBox ||
                                t.ID == (int)SessionHelper.ModuleList.ViewOnlyLotOrSerial)
                            {
                            }
                            else
                                context.UserRoleDetails.Add(objUserRoleDetail);

                        }

                        eTurns.DAL.RoleModuleDetail objDetails = context.RoleModuleDetails.Where(x => x.RoleId == RoleId && x.ModuleID == t.ID && x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyID && x.RoomId == RoomId).FirstOrDefault();
                        if (objDetails == null)
                        {
                            objRoleModuleDetail = new eTurns.DAL.RoleModuleDetail();
                            objRoleModuleDetail.CompanyID = CompanyID;
                            objRoleModuleDetail.EnterpriseID = EnterpriseId;
                            objRoleModuleDetail.GUID = Guid.NewGuid();
                            objRoleModuleDetail.ID = 0;
                            objRoleModuleDetail.IsChecked = true;
                            objRoleModuleDetail.IsDelete = true;
                            objRoleModuleDetail.IsInsert = true;
                            objRoleModuleDetail.IsUpdate = true;
                            objRoleModuleDetail.IsView = true;
                            objRoleModuleDetail.ModuleID = t.ID;
                            objRoleModuleDetail.ModuleValue = t.Value;
                            objRoleModuleDetail.RoleId = RoleId;
                            objRoleModuleDetail.RoomId = RoomId;
                            objRoleModuleDetail.ShowArchived = true;
                            objRoleModuleDetail.ShowDeleted = true;
                            objRoleModuleDetail.ShowUDF = true;
                            if (t.ID == (int)SessionHelper.ModuleList.AllowToEnterLotOrSerialInBlankBox || 
                                t.ID == (int)SessionHelper.ModuleList.ViewOnlyLotOrSerial)
                            {
                            }
                            else
                                context.RoleModuleDetails.Add(objRoleModuleDetail);

                        }
                    });
                    eTurns.DAL.UserRoomAccess objUserRoomAccess = context.UserRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.UserId == UserId);
                    if (objUserRoomAccess == null)
                    {
                        objUserRoomAccess = new eTurns.DAL.UserRoomAccess();
                        objUserRoomAccess.CompanyId = CompanyID;
                        objUserRoomAccess.EnterpriseId = EnterpriseId;
                        objUserRoomAccess.ID = 0;
                        objUserRoomAccess.RoleId = RoleId;
                        objUserRoomAccess.RoomId = RoomId;
                        objUserRoomAccess.UserId = UserId;
                        context.UserRoomAccesses.Add(objUserRoomAccess);
                    }
                    else
                    {
                        objUserRoomAccess.RoomId = RoomId;
                    }
                    eTurns.DAL.RoleRoomAccess objRoleRoomAccess = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && (t.RoomId == RoomId || t.RoomId == 0) && t.RoleId == RoleId);
                    if (objRoleRoomAccess == null)
                    {
                        objRoleRoomAccess = new eTurns.DAL.RoleRoomAccess();
                        objRoleRoomAccess.CompanyId = CompanyID;
                        objRoleRoomAccess.EnterpriseId = EnterpriseId;
                        objRoleRoomAccess.ID = 0;
                        objRoleRoomAccess.RoleId = RoleId;
                        objRoleRoomAccess.RoomId = RoomId;
                        context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    }
                    else
                    {
                        objRoleRoomAccess.RoomId = RoomId;
                    }
                    context.SaveChanges();
                }
                eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsByUserIdAndRoleId(UserId, RoleId);
                string strRoomList = string.Empty;
                lstPermissions = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
            }
            //AddRoomPermissionToEpRoleUsers(EnterpriseId, CompanyID, RoomId, UserId, RoleId, UserType);
            return lstPermissions;

        }

        public void AddNewCompanyPermissions(long EnterpriseId, long CompanyID, long UserId, long RoleId, long UserType)
        {
            List<UserWiseRoomsAccessDetailsDTO> lstPermissions = new List<UserWiseRoomsAccessDetailsDTO>();
            if (UserType == 1 && RoleId > 0)
            {
                using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    eTurnsMaster.DAL.UserRoomAccess objUserRoomAccess = context.UserRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == 0 && t.UserId == UserId);
                    if (objUserRoomAccess == null)
                    {
                        objUserRoomAccess = new eTurnsMaster.DAL.UserRoomAccess();
                        objUserRoomAccess.CompanyId = CompanyID;
                        objUserRoomAccess.EnterpriseId = EnterpriseId;
                        objUserRoomAccess.ID = 0;
                        objUserRoomAccess.RoleId = RoleId;
                        objUserRoomAccess.RoomId = 0;
                        objUserRoomAccess.UserId = UserId;
                        context.UserRoomAccesses.Add(objUserRoomAccess);
                    }
                    else
                    {
                        objUserRoomAccess.CompanyId = CompanyID;
                    }
                    eTurnsMaster.DAL.RoleRoomAccess objRoleRoomAccess = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == 0 && t.RoleId == RoleId);
                    if (objRoleRoomAccess == null)
                    {
                        objRoleRoomAccess = new eTurnsMaster.DAL.RoleRoomAccess();
                        objRoleRoomAccess.CompanyId = CompanyID;
                        objRoleRoomAccess.EnterpriseId = EnterpriseId;
                        objRoleRoomAccess.ID = 0;
                        objRoleRoomAccess.RoleId = RoleId;
                        objRoleRoomAccess.RoomId = 0;
                        context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    }
                    else
                    {
                        objRoleRoomAccess.CompanyId = CompanyID;
                    }
                    context.SaveChanges();
                }
            }
            else if (UserType == 2 && RoleId > 0)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    eTurns.DAL.UserRoomAccess objUserRoomAccess = context.UserRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == 0 && t.UserId == UserId);
                    if (objUserRoomAccess == null)
                    {
                        objUserRoomAccess = new eTurns.DAL.UserRoomAccess();
                        objUserRoomAccess.CompanyId = CompanyID;
                        objUserRoomAccess.EnterpriseId = EnterpriseId;
                        objUserRoomAccess.ID = 0;
                        objUserRoomAccess.RoleId = RoleId;
                        objUserRoomAccess.RoomId = 0;
                        objUserRoomAccess.UserId = UserId;
                        context.UserRoomAccesses.Add(objUserRoomAccess);
                    }
                    else
                    {
                        objUserRoomAccess.CompanyId = CompanyID;
                    }
                    eTurns.DAL.RoleRoomAccess objRoleRoomAccess = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == 0 && t.RoleId == RoleId);
                    if (objRoleRoomAccess == null)
                    {
                        objRoleRoomAccess = new eTurns.DAL.RoleRoomAccess();
                        objRoleRoomAccess.CompanyId = CompanyID;
                        objRoleRoomAccess.EnterpriseId = EnterpriseId;
                        objRoleRoomAccess.ID = 0;
                        objRoleRoomAccess.RoleId = RoleId;
                        objRoleRoomAccess.RoomId = 0;
                        context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    }
                    else
                    {
                        objRoleRoomAccess.CompanyId = CompanyID;
                    }

                    foreach (var item in context.RoleMasters.Where(t => (t.IsDeleted ?? false) == false && t.UserType == 2))
                    {
                        eTurns.DAL.RoleRoomAccess objRoleRoomAccess1 = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == 0 && t.RoleId == item.ID);
                        if (objRoleRoomAccess1 == null)
                        {
                            objRoleRoomAccess1 = new eTurns.DAL.RoleRoomAccess();
                            objRoleRoomAccess1.CompanyId = CompanyID;
                            objRoleRoomAccess1.EnterpriseId = EnterpriseId;
                            objRoleRoomAccess1.ID = 0;
                            objRoleRoomAccess1.RoleId = item.ID;
                            objRoleRoomAccess1.RoomId = 0;
                            context.RoleRoomAccesses.Add(objRoleRoomAccess1);
                        }
                        else
                        {
                            objRoleRoomAccess.CompanyId = CompanyID;
                        }
                    }

                    context.SaveChanges();
                }

            }

        }

        public void AddNewEPPermissions(long EnterpriseId, long UserId, long RoleId, long UserType)
        {
            List<UserWiseRoomsAccessDetailsDTO> lstPermissions = new List<UserWiseRoomsAccessDetailsDTO>();
            if (UserType == 1 && RoleId > 0)
            {
                using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                {

                    eTurnsMaster.DAL.UserRoomAccess objUserRoomAccess = new eTurnsMaster.DAL.UserRoomAccess();
                    objUserRoomAccess.CompanyId = 0;
                    objUserRoomAccess.EnterpriseId = EnterpriseId;
                    objUserRoomAccess.ID = 0;
                    objUserRoomAccess.RoleId = RoleId;
                    objUserRoomAccess.RoomId = 0;
                    objUserRoomAccess.UserId = UserId;
                    context.UserRoomAccesses.Add(objUserRoomAccess);

                    eTurnsMaster.DAL.RoleRoomAccess objRoleRoomAccess = new eTurnsMaster.DAL.RoleRoomAccess();
                    objRoleRoomAccess.CompanyId = 0;
                    objRoleRoomAccess.EnterpriseId = EnterpriseId;
                    objRoleRoomAccess.ID = 0;
                    objRoleRoomAccess.RoleId = RoleId;
                    objRoleRoomAccess.RoomId = 0;

                    context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    context.SaveChanges();
                }
            }
            else if (UserType == 2 && RoleId > 0)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    eTurns.DAL.UserRoomAccess objUserRoomAccess = new eTurns.DAL.UserRoomAccess();
                    objUserRoomAccess.CompanyId = 0;
                    objUserRoomAccess.EnterpriseId = EnterpriseId;
                    objUserRoomAccess.ID = 0;
                    objUserRoomAccess.RoleId = RoleId;
                    objUserRoomAccess.RoomId = 0;
                    objUserRoomAccess.UserId = UserId;
                    context.UserRoomAccesses.Add(objUserRoomAccess);

                    eTurns.DAL.RoleRoomAccess objRoleRoomAccess = new eTurns.DAL.RoleRoomAccess();
                    objRoleRoomAccess.CompanyId = 0;
                    objRoleRoomAccess.EnterpriseId = EnterpriseId;
                    objRoleRoomAccess.ID = 0;
                    objRoleRoomAccess.RoleId = RoleId;
                    objRoleRoomAccess.RoomId = 0;

                    context.RoleRoomAccesses.Add(objRoleRoomAccess);
                    context.SaveChanges();
                }
            }
        }

        public List<UserAccessDTO> GetUserAccess(long UserType, long RoleId, long UserId, long EnterpriseId, long CompanyID)
        {
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            List<UserAccessDTO> lstUserAccessSA = new List<UserAccessDTO>();
            List<UserAccessDTO> lstUserAccessSAWithNames = new List<UserAccessDTO>();
            List<UserAccessDTO> lstUserAccessEP = new List<UserAccessDTO>();
            if (UserType == 1)
            {
                if (RoleId < 1)
                {
                    using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccessSA = (from ua in context.UserRoomAccesses
                                           join um in context.UserMasters on ua.UserId equals um.ID
                                           join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                           join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                           where um.IsDeleted == false && um.UserType == 1 && ua.EnterpriseId == EnterpriseId
                                           select new UserAccessDTO
                                           {
                                               CompanyId = ua.CompanyId,
                                               RoleId = ua.RoleId,
                                               RoomId = ua.RoomId,
                                               UserId = ua.UserId,
                                               UserType = um.UserType ?? 0,
                                               EnterpriseId = ua.EnterpriseId,
                                               RoleName = rlm.RoleName,
                                               UserName = um.UserName
                                           }).ToList();

                        lstUserAccessSAWithNames = (from a in lstUserAccessSA
                                                    group a by new { a.EnterpriseId, a.CompanyId, a.RoomId } into groupeda
                                                    select new UserAccessDTO
                                                    {
                                                        RoomId = groupeda.Key.RoomId,
                                                        CompanyId = groupeda.Key.CompanyId,
                                                        EnterpriseId = groupeda.Key.EnterpriseId
                                                    }).ToList();

                        lstUserAccessSAWithNames = new EnterpriseMasterDAL().GetUserAccessWithNames(lstUserAccessSAWithNames);
                        lstUserAccessSA = (from a in lstUserAccessSA
                                           join b in lstUserAccessSAWithNames on new { a.EnterpriseId, a.CompanyId, a.RoomId } equals new { b.EnterpriseId, b.CompanyId, b.RoomId } into a_b_join
                                           from a_b in a_b_join.DefaultIfEmpty()
                                           select new UserAccessDTO
                                           {
                                               CompanyId = a.CompanyId,
                                               RoleId = a.RoleId,
                                               RoomId = a.RoomId,
                                               UserId = a.UserId,
                                               UserType = a.UserType,
                                               EnterpriseId = a.EnterpriseId,
                                               RoleName = a.RoleName,
                                               UserName = a.UserName,
                                               RoomName = a_b != null ? a_b.RoomName : string.Empty,
                                               CompanyName = a_b != null ? a_b.CompanyName : string.Empty,
                                           }).ToList();
                    }

                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccessEP = (from ua in context.UserRoomAccesses
                                           join um in context.UserMasters on ua.UserId equals um.ID
                                           join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                           join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                           join rm in context.Rooms on ua.RoomId equals rm.ID
                                           join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                           where um.IsDeleted == false && um.UserType > 1
                                           select new UserAccessDTO
                                           {
                                               CompanyId = ua.CompanyId,
                                               RoleId = ua.RoleId,
                                               RoomId = ua.RoomId,
                                               UserId = ua.UserId,
                                               UserType = um.UserType ?? 0,
                                               EnterpriseId = ua.EnterpriseId,
                                               RoleName = rlm.RoleName,
                                               UserName = um.UserName,
                                               RoomName = rm.RoomName,
                                               CompanyName = cm.Name
                                           }).ToList();

                    }
                    lstUserAccess = lstUserAccessSA.Union(lstUserAccessEP).ToList();
                }
                else
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccess = (from ua in context.UserRoomAccesses
                                         join um in context.UserMasters on ua.UserId equals um.ID
                                         join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                         join rm in context.Rooms on ua.RoomId equals rm.ID
                                         join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                         where um.IsDeleted == false && um.UserType > 1
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserId = ua.UserId,
                                             UserType = um.UserType ?? 0,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             UserName = um.UserName,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name
                                         }).ToList();

                    }
                }
            }
            if (UserType == 2)
            {
                if (RoleId < 0)
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccess = (from ua in context.UserRoomAccesses
                                         join um in context.UserMasters on ua.UserId equals um.ID
                                         join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                         join rm in context.Rooms on ua.RoomId equals rm.ID
                                         join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                         where um.IsDeleted == false && um.UserType > 1
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserId = ua.UserId,
                                             UserType = um.UserType ?? 0,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             UserName = um.UserName,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name
                                         }).ToList();

                    }
                }
                else
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccess = (from ua in context.UserRoomAccesses
                                         join um in context.UserMasters on ua.UserId equals um.ID
                                         join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                         join rm in context.Rooms on ua.RoomId equals rm.ID
                                         join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                         where um.IsDeleted == false && um.UserType > 1 && um.RoleId > 0
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserId = ua.UserId,
                                             UserType = um.UserType ?? 0,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             UserName = um.UserName,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name
                                         }).ToList();

                    }
                }
            }
            if (UserType == 3)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    lstUserAccess = (from ua in context.UserRoomAccesses
                                     join um in context.UserMasters on ua.UserId equals um.ID
                                     join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                     join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                     join rm in context.Rooms on ua.RoomId equals rm.ID
                                     join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                     where um.IsDeleted == false && um.UserType > 2 && um.CompanyID == CompanyID
                                     select new UserAccessDTO
                                     {
                                         CompanyId = ua.CompanyId,
                                         RoleId = ua.RoleId,
                                         RoomId = ua.RoomId,
                                         UserId = ua.UserId,
                                         UserType = um.UserType ?? 0,
                                         EnterpriseId = ua.EnterpriseId,
                                         RoleName = rlm.RoleName,
                                         UserName = um.UserName,
                                         RoomName = rm.RoomName,
                                         CompanyName = cm.Name
                                     }).ToList();

                }
            }
            return lstUserAccess;
        }

        public List<UserAccessDTO> GetUserAccessForUserList(long UserType, long RoleId, long UserId, long EnterpriseId, long CompanyID)
        {
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            //List<UserAccessDTO> lstUserAccessSA = new List<UserAccessDTO>();
            //List<UserAccessDTO> lstUserAccessSAWithNames = new List<UserAccessDTO>();
            //List<UserAccessDTO> lstUserAccessEP = new List<UserAccessDTO>();
            if (UserType == 1)
            {
                if (RoleId < 1)
                {
                    using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccess = (from ua in context.UserRoomAccesses
                                         join um in context.UserMasters on ua.UserId equals um.ID
                                         join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                         join em in context.EnterpriseMasters on ua.EnterpriseId equals em.ID
                                         join cm in context.MstCompanyMasters on ua.CompanyId equals cm.CompanyID
                                         join rm in context.MstRooms on ua.RoomId equals rm.RoomID
                                         join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                         where um.IsDeleted == false && um.UserType == 1 //&& ua.EnterpriseId == EnterpriseId
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserId = ua.UserId,
                                             UserType = um.UserType ?? 0,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             UserName = um.UserName,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name,
                                             EnterpriseName = em.Name,
                                         }).ToList();

                    }

                    List<EnterpriseDTO> oEnterpriseList = new EnterpriseMasterDAL().GetAllEnterprisesPlain();
                    foreach (EnterpriseDTO oEnterprise in oEnterpriseList)
                    {
                        if (!string.IsNullOrWhiteSpace(oEnterprise.EnterpriseDBName))
                        {
                            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(oEnterprise.EnterpriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                            {
                                var lstUsers = (from ua in context.UserRoomAccesses
                                                join um in context.UserMasters on ua.UserId equals um.ID
                                                join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                                join em in context.EnterpriseMasters on ua.EnterpriseId equals em.ID
                                                join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                                join rm in context.Rooms on ua.RoomId equals rm.ID
                                                join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                                where um.IsDeleted == false && um.UserType > 1
                                                select new UserAccessDTO
                                                {
                                                    CompanyId = ua.CompanyId,
                                                    RoleId = ua.RoleId,
                                                    RoomId = ua.RoomId,
                                                    UserId = ua.UserId,
                                                    UserType = um.UserType ?? 0,
                                                    EnterpriseId = ua.EnterpriseId,
                                                    RoleName = rlm.RoleName,
                                                    UserName = um.UserName,
                                                    RoomName = rm.RoomName,
                                                    CompanyName = cm.Name,
                                                    EnterpriseName = em.Name,
                                                }).ToList();

                                lstUserAccess = lstUserAccess.Union(lstUsers).ToList();
                            }
                        }
                    }
                }
                else
                {
                    List<long> EnterpriseIds;
                    using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        EnterpriseIds = (from ua in context.UserRoomAccesses
                                         where ua.UserId == UserId
                                         select
                                             ua.EnterpriseId
                                           ).Distinct().ToList();
                    }

                    foreach (long EntId in EnterpriseIds)
                    {
                        EnterpriseDTO objEnterprise = new EnterpriseDTO();
                        objEnterprise = new EnterpriseMasterDAL().GetEnterpriseByIdPlain(EntId);
                        if (objEnterprise != null)
                        {
                            if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                            {
                                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(objEnterprise.EnterpriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                                {
                                    var lstUsers = (from ua in context.UserRoomAccesses
                                                    join um in context.UserMasters on ua.UserId equals um.ID
                                                    join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                                    join em in context.EnterpriseMasters on ua.EnterpriseId equals em.ID
                                                    join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                                    join rm in context.Rooms on ua.RoomId equals rm.ID
                                                    join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                                    where um.IsDeleted == false && um.UserType > 1
                                                    select new UserAccessDTO
                                                    {
                                                        CompanyId = ua.CompanyId,
                                                        RoleId = ua.RoleId,
                                                        RoomId = ua.RoomId,
                                                        UserId = ua.UserId,
                                                        UserType = um.UserType ?? 0,
                                                        EnterpriseId = ua.EnterpriseId,
                                                        RoleName = rlm.RoleName,
                                                        UserName = um.UserName,
                                                        RoomName = rm.RoomName,
                                                        CompanyName = cm.Name,
                                                        EnterpriseName = em.Name,
                                                    }).ToList();

                                    lstUserAccess = lstUserAccess.Union(lstUsers).ToList();
                                }
                            }
                        }
                    }
                }
            }
            if (UserType == 2)
            {
                if (RoleId < 0)
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccess = (from ua in context.UserRoomAccesses
                                         join um in context.UserMasters on ua.UserId equals um.ID
                                         join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                         join em in context.EnterpriseMasters on ua.EnterpriseId equals em.ID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                         join rm in context.Rooms on ua.RoomId equals rm.ID
                                         join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                         where um.IsDeleted == false && um.UserType > 1
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserId = ua.UserId,
                                             UserType = um.UserType ?? 0,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             UserName = um.UserName,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name,
                                             EnterpriseName = em.Name,
                                         }).ToList();

                    }
                }
                else
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstUserAccess = (from ua in context.UserRoomAccesses
                                         join um in context.UserMasters on ua.UserId equals um.ID
                                         join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                         join em in context.EnterpriseMasters on ua.EnterpriseId equals em.ID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                         join rm in context.Rooms on ua.RoomId equals rm.ID
                                         join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                         where um.IsDeleted == false && um.UserType > 1 && um.RoleId > 0
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserId = ua.UserId,
                                             UserType = um.UserType ?? 0,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             UserName = um.UserName,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name,
                                             EnterpriseName = em.Name,
                                         }).ToList();

                    }
                }
            }
            if (UserType == 3)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    lstUserAccess = (from ua in context.UserRoomAccesses
                                     join um in context.UserMasters on ua.UserId equals um.ID
                                     join utm in context.UserTypeMasters on um.UserType equals utm.UserTypeID
                                     join em in context.EnterpriseMasters on ua.EnterpriseId equals em.ID
                                     join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                     join rm in context.Rooms on ua.RoomId equals rm.ID
                                     join rlm in context.RoleMasters on um.RoleId equals rlm.ID
                                     where um.IsDeleted == false && um.UserType > 2 && um.CompanyID == CompanyID
                                     select new UserAccessDTO
                                     {
                                         CompanyId = ua.CompanyId,
                                         RoleId = ua.RoleId,
                                         RoomId = ua.RoomId,
                                         UserId = ua.UserId,
                                         UserType = um.UserType ?? 0,
                                         EnterpriseId = ua.EnterpriseId,
                                         RoleName = rlm.RoleName,
                                         UserName = um.UserName,
                                         RoomName = rm.RoomName,
                                         CompanyName = cm.Name,
                                         EnterpriseName = em.Name,
                                     }).ToList();

                }
            }
            return lstUserAccess;
        }

        public List<UserAccessDTO> GetRoleAccess(long UserType, long RoleId, long UserId, long EnterpriseId, long CompanyID)
        {
            List<UserAccessDTO> lstRoleAccess = new List<UserAccessDTO>();
            List<UserAccessDTO> lstRoleAccessSA = new List<UserAccessDTO>();
            List<UserAccessDTO> lstRoleAccessSAWithNames = new List<UserAccessDTO>();
            List<UserAccessDTO> lstRoleAccessEP = new List<UserAccessDTO>();
            if (UserType == 1)
            {
                if (RoleId < 1)
                {
                    using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstRoleAccessSA = (from ua in context.RoleRoomAccesses
                                           join rm in context.RoleMasters on ua.RoleId equals rm.ID
                                           join utm in context.UserTypeMasters on rm.UserType equals utm.UserTypeID
                                           where rm.IsDeleted == false && rm.UserType == 1 && ua.EnterpriseId == EnterpriseId
                                           select new UserAccessDTO
                                           {
                                               CompanyId = ua.CompanyId,
                                               RoleId = ua.RoleId,
                                               RoomId = ua.RoomId,
                                               UserType = rm.UserType ?? 0,
                                               EnterpriseId = ua.EnterpriseId,
                                               RoleName = rm.RoleName
                                           }).ToList();

                        lstRoleAccessSAWithNames = (from a in lstRoleAccessSA
                                                    group a by new { a.EnterpriseId, a.CompanyId, a.RoomId } into groupeda
                                                    select new UserAccessDTO
                                                    {
                                                        RoomId = groupeda.Key.RoomId,
                                                        CompanyId = groupeda.Key.CompanyId,
                                                        EnterpriseId = groupeda.Key.EnterpriseId
                                                    }).ToList();

                        lstRoleAccessSAWithNames = new EnterpriseMasterDAL().GetUserAccessWithNames(lstRoleAccessSAWithNames);
                        lstRoleAccessSA = (from a in lstRoleAccessSA
                                           join b in lstRoleAccessSAWithNames on new { a.EnterpriseId, a.CompanyId, a.RoomId } equals new { b.EnterpriseId, b.CompanyId, b.RoomId } into a_b_join
                                           from a_b in a_b_join.DefaultIfEmpty()
                                           select new UserAccessDTO
                                           {
                                               CompanyId = a.CompanyId,
                                               RoleId = a.RoleId,
                                               RoomId = a.RoomId,
                                               UserType = a.UserType,
                                               EnterpriseId = a.EnterpriseId,
                                               RoleName = a.RoleName,
                                               RoomName = a_b != null ? a_b.RoomName : string.Empty,
                                               CompanyName = a_b != null ? a_b.CompanyName : string.Empty,
                                           }).ToList();
                    }

                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstRoleAccessEP = (from ua in context.RoleRoomAccesses
                                           join rlm in context.RoleMasters on ua.RoleId equals rlm.ID
                                           join utm in context.UserTypeMasters on rlm.UserType equals utm.UserTypeID
                                           join cm in context.CompanyMasters on ua.CompanyId equals cm.ID into ua_cm_join
                                           from ua_cm in ua_cm_join.DefaultIfEmpty()
                                           join rm in context.Rooms on ua.RoomId equals rm.ID into ua_rm_join
                                           from ua_rm in ua_rm_join.DefaultIfEmpty()
                                           where rlm.IsDeleted == false && rlm.UserType > 1
                                           select new UserAccessDTO
                                           {
                                               CompanyId = ua.CompanyId,
                                               RoleId = ua.RoleId,
                                               RoomId = ua.RoomId,
                                               UserType = rlm.UserType,
                                               EnterpriseId = ua.EnterpriseId,
                                               RoleName = rlm.RoleName,
                                               RoomName = ua_rm.RoomName,
                                               CompanyName = ua_cm.Name
                                           }).ToList();

                    }
                    lstRoleAccess = lstRoleAccessSA.Union(lstRoleAccessEP).ToList();
                    //var Q = lstRoleAccessEP.Where(t => t.CompanyId == 201180026);
                }
                else
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstRoleAccess = (from ua in context.RoleRoomAccesses
                                         join rlm in context.RoleMasters on ua.RoleId equals rlm.ID
                                         join utm in context.UserTypeMasters on rlm.UserType equals utm.UserTypeID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID into ua_cm_join
                                         from ua_cm in ua_cm_join.DefaultIfEmpty()
                                         join rm in context.Rooms on ua.RoomId equals rm.ID into ua_rm_join
                                         from ua_rm in ua_rm_join.DefaultIfEmpty()
                                         where rlm.IsDeleted == false && rlm.UserType > 1
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserType = rlm.UserType,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             RoomName = ua_rm.RoomName,
                                             CompanyName = ua_cm.Name
                                         }).ToList();

                    }
                }
            }
            if (UserType == 2)
            {
                if (RoleId < 0)
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstRoleAccess = (from ua in context.RoleRoomAccesses
                                         join rlm in context.RoleMasters on ua.RoleId equals rlm.ID
                                         join utm in context.UserTypeMasters on rlm.UserType equals utm.UserTypeID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID into ua_cm_join
                                         from ua_cm in ua_cm_join.DefaultIfEmpty()
                                         join rm in context.Rooms on ua.RoomId equals rm.ID into ua_rm_join
                                         from ua_rm in ua_rm_join.DefaultIfEmpty()
                                         where rlm.IsDeleted == false && rlm.UserType > 1
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserType = rlm.UserType,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             RoomName = ua_rm.RoomName,
                                             CompanyName = ua_cm.Name
                                         }).ToList();

                    }
                }
                else
                {
                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        lstRoleAccess = (from ua in context.RoleRoomAccesses
                                         join rlm in context.RoleMasters on ua.RoleId equals rlm.ID
                                         join utm in context.UserTypeMasters on rlm.UserType equals utm.UserTypeID
                                         join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                         join rm in context.Rooms on ua.RoomId equals rm.ID
                                         where rlm.IsDeleted == false && rlm.UserType > 1
                                         select new UserAccessDTO
                                         {
                                             CompanyId = ua.CompanyId,
                                             RoleId = ua.RoleId,
                                             RoomId = ua.RoomId,
                                             UserType = rlm.UserType,
                                             EnterpriseId = ua.EnterpriseId,
                                             RoleName = rlm.RoleName,
                                             RoomName = rm.RoomName,
                                             CompanyName = cm.Name
                                         }).ToList();


                    }
                }
            }
            if (UserType == 3)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    lstRoleAccess = (from ua in context.RoleRoomAccesses
                                     join rlm in context.RoleMasters on ua.RoleId equals rlm.ID
                                     join utm in context.UserTypeMasters on rlm.UserType equals utm.UserTypeID
                                     join cm in context.CompanyMasters on ua.CompanyId equals cm.ID
                                     join rm in context.Rooms on ua.RoomId equals rm.ID
                                     where rlm.IsDeleted == false && rlm.UserType > 2 && rlm.CompanyID == CompanyID
                                     select new UserAccessDTO
                                     {
                                         CompanyId = ua.CompanyId,
                                         RoleId = ua.RoleId,
                                         RoomId = ua.RoomId,
                                         UserType = rlm.UserType,
                                         EnterpriseId = ua.EnterpriseId,
                                         RoleName = rlm.RoleName,
                                         RoomName = rm.RoomName,
                                         CompanyName = cm.Name
                                     }).ToList();


                }
            }
            return lstRoleAccess;
        }

        public long ResetPassword(long UserId, string NewPassword)
        {
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                eTurnsMaster.DAL.UserMaster objUser = context.UserMasters.FirstOrDefault(t => t.ID == UserId);
                if (objUser != null)
                {
                    objUser.Password = NewPassword;
                    context.SaveChanges();
                    if (objUser.UserType >= 2)
                    {
                        eTurnsMaster.DAL.EnterpriseMaster objEnterprise = context.EnterpriseMasters.FirstOrDefault(t => t.ID == objUser.EnterpriseId);
                        if (objEnterprise != null)
                        {
                            string Qry = "UPDATE USERMASTER SET PASSWORD='" + NewPassword + "' WHERE ID=" + UserId;
                            SqlConnection EturnsConnection = new SqlConnection(objEnterprise.EnterpriseDBConnectionString);
                            eTurns.DAL.SqlHelper.ExecuteNonQuery(EturnsConnection, CommandType.Text, Qry);
                        }
                    }
                }
            }
            return UserId;
        }

        public bool CheckSetAcountLockout(string UserName, string Password, UserMasterDTO objDTO)
        {
            bool IsUserLocked = false;
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                if (objDTO == null)
                {
                    UserLoginFailAttempt objUserLoginFailAttempt = new UserLoginFailAttempt();
                    objUserLoginFailAttempt.AttemptTime = DateTimeUtility.DateTimeNow;
                    objUserLoginFailAttempt.ID = 0;
                    objUserLoginFailAttempt.Password = Password;
                    objUserLoginFailAttempt.UserName = UserName;
                    context.UserLoginFailAttempts.Add(objUserLoginFailAttempt);
                    context.SaveChanges();
                    IQueryable<UserLoginFailAttempt> lstAttempts = context.UserLoginFailAttempts.Where(t => t.UserName == UserName).OrderByDescending(t => t.AttemptTime);
                    if (lstAttempts.Count() >= MaxFailedAttempt)
                    {
                        eTurnsMaster.DAL.UserMaster objUser = context.UserMasters.FirstOrDefault(t => t.UserName == UserName && t.IsDeleted == false);
                        if (objUser != null && !objUser.IsLocked)
                        {
                            objUser.IsLocked = true;
                            context.SaveChanges();
                        }
                        IsUserLocked = true;
                    }

                }
                else if (objDTO != null && objDTO.ID > 0 && objDTO.IsLocked)
                {
                    IQueryable<UserLoginFailAttempt> lstAttempts = context.UserLoginFailAttempts.Where(t => t.UserName == UserName).OrderByDescending(t => t.AttemptTime);
                    if (lstAttempts.Any())
                    {
                        UserLoginFailAttempt lastAttempt = lstAttempts.First();
                        if (lastAttempt.AttemptTime.AddMinutes(LockooutMinutes) < DateTimeUtility.DateTimeNow)
                        {
                            foreach (UserLoginFailAttempt fitem in lstAttempts)
                            {
                                context.UserLoginFailAttempts.Remove(fitem);
                            }
                            eTurnsMaster.DAL.UserMaster objUser = context.UserMasters.FirstOrDefault(t => t.UserName == UserName && t.IsDeleted == false);
                            if (objUser != null)
                            {
                                objUser.IsLocked = false;
                            }
                            context.SaveChanges();
                            IsUserLocked = false;
                        }
                        else
                        {
                            IsUserLocked = true;
                        }
                    }
                }
                else if (objDTO != null && objDTO.ID > 0 && !objDTO.IsLocked)
                {
                    IQueryable<UserLoginFailAttempt> lstAttempts = context.UserLoginFailAttempts.Where(t => t.UserName == UserName).OrderByDescending(t => t.AttemptTime);
                    if (lstAttempts.Any())
                    {
                        foreach (UserLoginFailAttempt fitem in lstAttempts)
                        {
                            context.UserLoginFailAttempts.Remove(fitem);
                        }
                        context.SaveChanges();
                    }
                }
                else
                {
                    IsUserLocked = false;
                }
            }
            //using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            //{
            //    IQueryable<UserLoginFailAttempt> lstAttempts = context.UserLoginFailAttempts.Where(t => t.UserName == UserName).OrderByDescending(t => t.AttemptTime);
            //    if (lstAttempts.Any())
            //    {
            //        UserLoginFailAttempt lastAttempt = lstAttempts.First();
            //        if (lastAttempt.AttemptTime.AddHours(1) > DateTime.Now)
            //        {

            //        }
            //    }
            //    else
            //    {

            //    }
            //}
            return IsUserLocked;
        }

        public void AddRoomPermissionToEpRoleUsers(long EnterpriseId, long CompanyID, long RoomId, long UserId, long RoleId, long UserType)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            eTurns.DAL.RoleModuleDetail objRoleModuleDetail = new eTurns.DAL.RoleModuleDetail();

            using (var context = new eTurnsEntities(objBinMasterDAL.DataBaseEntityConnectionString))
            {
                foreach (var item in context.RoleMasters.Where(t => (t.IsDeleted ?? false) == false && t.UserType == 2 && t.ID > 0))
                {
                    eTurns.DAL.RoleRoomAccess objRoleRoomAccess2 = context.RoleRoomAccesses.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyID && t.RoomId == RoomId && t.RoleId == item.ID);
                    if (objRoleRoomAccess2 == null)
                    {
                        objRoleRoomAccess2 = new eTurns.DAL.RoleRoomAccess();
                        objRoleRoomAccess2.CompanyId = CompanyID;
                        objRoleRoomAccess2.EnterpriseId = EnterpriseId;
                        objRoleRoomAccess2.ID = 0;
                        objRoleRoomAccess2.RoleId = item.ID;
                        objRoleRoomAccess2.RoomId = RoomId;
                        context.RoleRoomAccesses.Add(objRoleRoomAccess2);
                    }
                    else
                    {
                        objRoleRoomAccess2.RoomId = RoomId;
                    }
                    context.ModuleMasters.ToList().ForEach(t =>
                    {
                        objRoleModuleDetail = new eTurns.DAL.RoleModuleDetail();
                        objRoleModuleDetail.CompanyID = CompanyID;
                        objRoleModuleDetail.EnterpriseID = EnterpriseId;
                        objRoleModuleDetail.GUID = Guid.NewGuid();
                        objRoleModuleDetail.ID = 0;
                        objRoleModuleDetail.IsChecked = true;
                        objRoleModuleDetail.IsDelete = true;
                        objRoleModuleDetail.IsInsert = true;
                        objRoleModuleDetail.IsUpdate = true;
                        objRoleModuleDetail.IsView = true;
                        objRoleModuleDetail.ModuleID = t.ID;
                        objRoleModuleDetail.ModuleValue = t.Value;
                        objRoleModuleDetail.RoleId = item.ID;
                        objRoleModuleDetail.RoomId = RoomId;
                        objRoleModuleDetail.ShowArchived = true;
                        objRoleModuleDetail.ShowDeleted = true;
                        objRoleModuleDetail.ShowUDF = true;
                        context.RoleModuleDetails.Add(objRoleModuleDetail);

                    });
                }

                context.SaveChanges();
            }
        }

        public void DeleteRoomReferences(long EnterpriseID, long CompanyID, string[] RoomIds, string EnterpriseDBName)
        {
            long RoomID = 0;
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                if (RoomIds != null && RoomIds.Count() > 0)
                {
                    foreach (var Roomitem in RoomIds)
                    {
                        if (long.TryParse(Roomitem, out RoomID))
                        {
                            IQueryable<eTurnsMaster.DAL.RoleRoomAccess> mstrroleRoomAccess = context.RoleRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrroleRoomAccess.Any())
                            {
                                foreach (var item in mstrroleRoomAccess)
                                {
                                    context.RoleRoomAccesses.Remove(item);
                                }
                            }

                            IQueryable<eTurnsMaster.DAL.UserRoomAccess> mstrUserRoomAccess = context.UserRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrUserRoomAccess.Any())
                            {
                                foreach (var item in mstrUserRoomAccess)
                                {
                                    context.UserRoomAccesses.Remove(item);
                                }
                            }

                            IQueryable<eTurnsMaster.DAL.RoleModuleDetail> mstrRoleModules = context.RoleModuleDetails.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrRoleModules.Any())
                            {
                                foreach (var item in mstrRoleModules)
                                {
                                    context.RoleModuleDetails.Remove(item);
                                }
                            }

                            IQueryable<eTurnsMaster.DAL.UserRoleDetail> mstrUserModules = context.UserRoleDetails.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrUserModules.Any())
                            {
                                foreach (var item in mstrUserModules)
                                {
                                    context.UserRoleDetails.Remove(item);
                                }
                            }

                        }
                    }
                    context.SaveChanges();

                }
            }

            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(EnterpriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                if (RoomIds != null && RoomIds.Count() > 0)
                {
                    foreach (var Roomitem in RoomIds)
                    {
                        if (long.TryParse(Roomitem, out RoomID))
                        {
                            IQueryable<eTurns.DAL.RoleRoomAccess> mstrroleRoomAccess = context.RoleRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrroleRoomAccess.Any())
                            {
                                foreach (var item in mstrroleRoomAccess)
                                {
                                    context.RoleRoomAccesses.Remove(item);
                                }
                            }

                            IQueryable<eTurns.DAL.UserRoomAccess> mstrUserRoomAccess = context.UserRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrUserRoomAccess.Any())
                            {
                                foreach (var item in mstrUserRoomAccess)
                                {
                                    context.UserRoomAccesses.Remove(item);
                                }
                            }

                            IQueryable<eTurns.DAL.RoleModuleDetail> mstrRoleModules = context.RoleModuleDetails.Where(t => t.EnterpriseID == EnterpriseID && t.CompanyID == CompanyID && t.RoomId == RoomID);
                            if (mstrRoleModules.Any())
                            {
                                foreach (var item in mstrRoleModules)
                                {
                                    context.RoleModuleDetails.Remove(item);
                                }
                            }

                            IQueryable<eTurns.DAL.UserRoleDetail> mstrUserModules = context.UserRoleDetails.Where(t => t.EnterpriseID == EnterpriseID && t.CompanyID == CompanyID && t.RoomId == RoomID);
                            if (mstrUserModules.Any())
                            {
                                foreach (var item in mstrUserModules)
                                {
                                    context.UserRoleDetails.Remove(item);
                                }
                            }

                        }
                    }
                    context.SaveChanges();

                }
            }

        }


        /// <summary>
        /// Created By Hetal for soft delete all reference table of Room while room has been deleted
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="UserType"></param>
        /// <returns></returns>
        /// 
        public void SoftDeleteRoomReferences(string ids, long EnterpriseID, long CompanyID, string[] RoomIds, string EnterpriseDBName)
        {
            long RoomID = 0;

            #region /// remove references from the master database.

            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                if (RoomIds != null && RoomIds.Count() > 0)
                {
                    foreach (var Roomitem in RoomIds)
                    {
                        if (long.TryParse(Roomitem, out RoomID))
                        {
                            IQueryable<eTurnsMaster.DAL.RoleRoomAccess> mstrroleRoomAccess = context.RoleRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrroleRoomAccess.Any())
                            {
                                foreach (var item in mstrroleRoomAccess)
                                {
                                    context.RoleRoomAccesses.Remove(item);
                                }
                            }

                            IQueryable<eTurnsMaster.DAL.UserRoomAccess> mstrUserRoomAccess = context.UserRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrUserRoomAccess.Any())
                            {
                                foreach (var item in mstrUserRoomAccess)
                                {
                                    context.UserRoomAccesses.Remove(item);
                                }
                            }

                            IQueryable<eTurnsMaster.DAL.RoleModuleDetail> mstrRoleModules = context.RoleModuleDetails.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrRoleModules.Any())
                            {
                                foreach (var item in mstrRoleModules)
                                {
                                    context.RoleModuleDetails.Remove(item);
                                }
                            }

                            IQueryable<eTurnsMaster.DAL.UserRoleDetail> mstrUserModules = context.UserRoleDetails.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
                            if (mstrUserModules.Any())
                            {
                                foreach (var item in mstrUserModules)
                                {
                                    context.UserRoleDetails.Remove(item);
                                }
                            }

                        }
                    }
                    context.SaveChanges();

                }
            }

            #endregion

            #region used for physical delete into reference table (RoleRoomAccesses,UserRoomAccesses,RoleModuleDetails,UserRoleDetails) for room -- Hetal

            //using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            //using (var context = new eTurnsEntities(WebConnectionHelper.GetConnectionString(EnterpriseDBName)))
            //{
            //    if (RoomIds != null && RoomIds.Count() > 0)
            //    {
            //        foreach (var Roomitem in RoomIds)
            //        {
            //            if (long.TryParse(Roomitem, out RoomID))
            //            {
            //                IQueryable<eTurns.DAL.RoleRoomAccess> mstrroleRoomAccess = context.RoleRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
            //                if (mstrroleRoomAccess.Any())
            //                {
            //                    foreach (var item in mstrroleRoomAccess)
            //                    {
            //                        context.RoleRoomAccesses.DeleteObject(item);
            //                    }
            //                }

            //                IQueryable<eTurns.DAL.UserRoomAccess> mstrUserRoomAccess = context.UserRoomAccesses.Where(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomId == RoomID);
            //                if (mstrUserRoomAccess.Any())
            //                {
            //                    foreach (var item in mstrUserRoomAccess)
            //                    {
            //                        context.UserRoomAccesses.DeleteObject(item);
            //                    }
            //                }

            //                IQueryable<eTurns.DAL.RoleModuleDetail> mstrRoleModules = context.RoleModuleDetails.Where(t => t.EnterpriseID == EnterpriseID && t.CompanyID == CompanyID && t.RoomId == RoomID);
            //                if (mstrRoleModules.Any())
            //                {
            //                    foreach (var item in mstrRoleModules)
            //                    {
            //                        context.RoleModuleDetails.DeleteObject(item);
            //                    }
            //                }

            //                IQueryable<eTurns.DAL.UserRoleDetail> mstrUserModules = context.UserRoleDetails.Where(t => t.EnterpriseID == EnterpriseID && t.CompanyID == CompanyID && t.RoomId == RoomID);
            //                if (mstrUserModules.Any())
            //                {
            //                    foreach (var item in mstrUserModules)
            //                    {
            //                        context.UserRoleDetails.DeleteObject(item);
            //                    }
            //                }

            //            }
            //        }
            //        context.SaveChanges();
            //    }
            //}
            #endregion

            #region Enter into Schedule table for soft delete all the reference table of the Room -- Hetal

            if (RoomIds != null && RoomIds.Count() > 0)
            {
                var strRoomIds = String.Join(",", RoomIds);

                RoomReferenceDeleteDAL objRoomReferenceDeleteDAL = new RoomReferenceDeleteDAL();

                RoomReferenceDeleteDTO objRoomReferenceDeleteDTO = new RoomReferenceDeleteDTO();
                objRoomReferenceDeleteDTO.RoomIds = strRoomIds;
                objRoomReferenceDeleteDTO.EnterpriseId = EnterpriseID;
                objRoomReferenceDeleteDTO.CompanyId = CompanyID;
                objRoomReferenceDeleteDTO.EnterpriseDBName = EnterpriseDBName;
                objRoomReferenceDeleteDTO.ProcessStatus = 0;

                objRoomReferenceDeleteDAL.Insert(objRoomReferenceDeleteDTO);
            }

            #endregion
        }


        public bool CheckRoleType(long RoleID, long UserType)
        {
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                return context.RoleMasters.Any(t => t.ID == RoleID && t.UserType == UserType && (t.IsDeleted ?? false) == false);
            }
        }

        public UserMasterDTO ResetPassword(string UserName, string Password)
        {
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                eTurnsMaster.DAL.UserMaster objUser = context.UserMasters.Where(u => u.UserName == UserName && u.IsDeleted == false).FirstOrDefault();
                if (objUser != null)
                {
                    objUser.Password = Password;
                    context.SaveChanges();
                    return GetUserByName(objUser.UserName);
                }
                else
                {
                    return new UserMasterDTO();
                }
            }
        }
        public UserMasterDTO ResetPasswordChild(UserMasterDTO objUserMaster)
        {
            if (!string.IsNullOrWhiteSpace(objUserMaster.EnterpriseDbName))
            {
                eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(objUserMaster.EnterpriseDbName);
                UserPasswordChangeHistoryDTO objUserPasswordChangeHistoryDTO = new UserPasswordChangeHistoryDTO();
                objUserPasswordChangeHistoryDTO.UserId = objUserMaster.ID;
                objUserPasswordChangeHistoryDTO.NewPassword = objUserMaster.Password;
                objUserMasterDAL.UpdatePassword(objUserPasswordChangeHistoryDTO);
                return objUserMaster;
                //using (var context = new eTurnsEntities(objUserMasterDAL.DataBaseEntityConnectionString))
                //{
                //    eTurns.DAL.UserMaster objUser = context.UserMasters.Where(u => u.UserName == objUserMaster.UserName && u.IsDeleted == false).FirstOrDefault();
                //    if (objUser != null)
                //    {
                //        objUser.Password = objUserMaster.Password;
                //        context.SaveChanges();
                //        return GetUserByName(objUser.UserName);
                //    }
                //    else
                //    {
                //        return new UserMasterDTO();
                //    }
                //}
            }
            else
            {
                return new UserMasterDTO();
            }
        }

        public UserMasterDTO GetUserByID(long UserId)
        {
            UserMasterDTO objUserMasterDTO = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                objUserMasterDTO = (from usr in context.UserMasters
                                    where usr.ID == UserId
                                    select new UserMasterDTO
                                    {
                                        CompanyID = usr.CompanyID,
                                        CompanyName = string.Empty,
                                        Created = usr.Created ?? DateTime.MinValue,
                                        CreatedBy = usr.CreatedBy,
                                        Email = usr.Email,
                                        EnterpriseId = usr.EnterpriseId,
                                        GUID = usr.GUID,
                                        ID = usr.ID,
                                        IsArchived = usr.IsArchived,
                                        IsDeleted = usr.IsDeleted,
                                        IsLicenceAccepted = usr.IsLicenceAccepted,
                                        LastUpdatedBy = usr.LastUpdatedBy,
                                        Password = usr.Password,
                                        Phone = usr.Phone,
                                        RoleID = usr.RoleId,
                                        RoleName = string.Empty,
                                        Updated = usr.Updated,
                                        UserName = usr.UserName,
                                        UserType = usr.UserType ?? 0,
                                        UserTypeName = string.Empty,

                                    }).FirstOrDefault();
            }
            return objUserMasterDTO;
        }

        public bool UnDeleteUseWiseRecords(long UserId, long UpdatedBy, int UserType, string EnterpriseDBName)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                var params1 = new SqlParameter[] {
                        new SqlParameter("@ID", UserId),
                        new SqlParameter("@UpdatedBy", UpdatedBy),
                        new SqlParameter("@UserType", UserType),
                        new SqlParameter("@EnterpriseDBName", EnterpriseDBName ?? (object)DBNull.Value)
                        };
                context.Database.ExecuteSqlCommand("EXEC [UndeleteUser] @ID,@UpdatedBy,@UserType,@EnterpriseDBName", params1);
                return true;
            }
        }
    }
}
