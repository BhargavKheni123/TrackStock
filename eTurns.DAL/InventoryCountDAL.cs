using eTurns.DTO;
using eTurns.DTO.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;

namespace eTurns.DAL
{
    public class InventoryCountDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        #region [Class Constructor]

        //public InventoryCountDAL(base.DataBaseName)
        //{

        //}

        public InventoryCountDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public InventoryCountDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public List<InventoryCountDTO> GetAllCounts(Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("exec [GetAllCountsByRoom] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public bool CheckLineItems(Int64 RoomID, Int64 CompanyID, Guid CountGUID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    List<InventoryCountDetail> CountDetail = (from ci in context.InventoryCountDetails
                                                              join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                                              from ci_cc in ci_cc_join.DefaultIfEmpty()
                                                              join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                                              from ci_cu in ci_cu_join.DefaultIfEmpty()
                                                              join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                                                              from ci_rm in ci_rm_join.DefaultIfEmpty()
                                                              where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.InventoryCountGUID == CountGUID && ci.IsDeleted.Equals(false) && ci.IsArchived.Equals(false)
                                                              select ci
                                                   ).ToList();
                    if (CountDetail != null && CountDetail.Where(cd => cd.IsApplied == false).Count() > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void SetAppliedFalse(Int64 RoomID, Int64 CompanyID, Guid CountGUID, bool IsApplied)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    InventoryCount obj = context.InventoryCounts.Where(x => x.GUID == CountGUID).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.IsApplied = IsApplied;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
        }

        public List<InventoryCountDTO> GetAllCounts(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            Expression<Func<InventoryCountDTO, bool>> Filterexp = q => true;
            if (IsArchived)
            {
                Filterexp = Filterexp.And(t => t.IsArchived == true);
            }
            if (IsDeleted)
            {
                Filterexp = Filterexp.And(t => t.IsDeleted == true);
            }
            List<InventoryCountDTO> lstCounts = new List<InventoryCountDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstCounts = (from ci in context.InventoryCounts
                             join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                             from ci_cc in ci_cc_join.DefaultIfEmpty()
                             join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                             from ci_cu in ci_cu_join.DefaultIfEmpty()
                             join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                             from ci_rm in ci_rm_join.DefaultIfEmpty()
                             where ci.CompanyId == CompanyID && ci.RoomId == RoomID
                             select new InventoryCountDTO
                             {
                                 ID = ci.ID,
                                 GUID = ci.GUID,
                                 CountName = ci.CountName,
                                 CountItemDescription = ci.CountItemDescription,
                                 CountType = ci.CountType,
                                 CountStatus = ci.CountStatus,
                                 UDF1 = ci.UDF1,
                                 UDF2 = ci.UDF2,
                                 UDF3 = ci.UDF3,
                                 UDF4 = ci.UDF4,
                                 UDF5 = ci.UDF5,
                                 Created = ci.Created,
                                 Updated = ci.Updated,
                                 CreatedBy = ci.CreatedBy,
                                 LastUpdatedBy = ci.LastUpdatedBy,
                                 IsDeleted = ci.IsDeleted,
                                 IsArchived = ci.IsArchived,
                                 Year = ci.Year,
                                 CompanyId = ci.CompanyId,
                                 RoomId = ci.RoomId,
                                 CountDate = ci.CountDate,
                                 CountCompletionDate = ci.CountCompletionDate,
                                 IsAutomatedCompletion = ci.IsAutomatedCompletion,
                                 CompleteCauseCountGUID = ci.CompleteCauseCountGUID,
                                 CreatedByName = ci_cc.UserName,
                                 UpdatedByName = ci_cu.UserName,
                                 RoomName = ci_rm.RoomName,
                                 ProjectSpendGUID = ci.ProjectSpendGUID,
                                 ReleaseNumber = ci.ReleaseNumber
                             }).AsExpandable().Where(Filterexp).ToList();
            }
            return lstCounts;

        }

        public InventoryCountDTO GetInventoryCountById(long ID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from ci in context.InventoryCounts
                        join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                        from ci_cc in ci_cc_join.DefaultIfEmpty()
                        join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                        from ci_cu in ci_cu_join.DefaultIfEmpty()
                        join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                        from ci_rm in ci_rm_join.DefaultIfEmpty()
                        where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.ID == ID
                        select new InventoryCountDTO
                        {
                            ID = ci.ID,
                            GUID = ci.GUID,
                            CountName = ci.CountName,
                            CountItemDescription = ci.CountItemDescription,
                            CountType = ci.CountType,
                            CountStatus = ci.CountStatus,
                            UDF1 = ci.UDF1,
                            UDF2 = ci.UDF2,
                            UDF3 = ci.UDF3,
                            UDF4 = ci.UDF4,
                            UDF5 = ci.UDF5,
                            Created = ci.Created,
                            Updated = ci.Updated,
                            CreatedBy = ci.CreatedBy,
                            LastUpdatedBy = ci.LastUpdatedBy,
                            IsDeleted = ci.IsDeleted,
                            IsArchived = ci.IsArchived,
                            Year = ci.Year,
                            CompanyId = ci.CompanyId,
                            RoomId = ci.RoomId,
                            CountDate = ci.CountDate,
                            CountCompletionDate = ci.CountCompletionDate,
                            IsAutomatedCompletion = ci.IsAutomatedCompletion,
                            CompleteCauseCountGUID = ci.CompleteCauseCountGUID,
                            CreatedByName = ci_cc.UserName,
                            UpdatedByName = ci_cu.UserName,
                            RoomName = ci_rm.RoomName,
                            ProjectSpendGUID = ci.ProjectSpendGUID,
                            ReleaseNumber = ci.ReleaseNumber
                        }).FirstOrDefault();
            }
        }

        public InventoryCountDTO GetInventoryCountByGUId(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from ci in context.InventoryCounts
                        join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                        from ci_cc in ci_cc_join.DefaultIfEmpty()
                        join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                        from ci_cu in ci_cu_join.DefaultIfEmpty()
                        join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                        from ci_rm in ci_rm_join.DefaultIfEmpty()
                        where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.GUID == GUID
                        select new InventoryCountDTO
                        {
                            ID = ci.ID,
                            GUID = ci.GUID,
                            CountName = ci.CountName,
                            CountItemDescription = ci.CountItemDescription,
                            CountType = ci.CountType,
                            CountStatus = ci.CountStatus,
                            UDF1 = ci.UDF1,
                            UDF2 = ci.UDF2,
                            UDF3 = ci.UDF3,
                            UDF4 = ci.UDF4,
                            UDF5 = ci.UDF5,
                            Created = ci.Created,
                            Updated = ci.Updated,
                            CreatedBy = ci.CreatedBy,
                            LastUpdatedBy = ci.LastUpdatedBy,
                            IsDeleted = ci.IsDeleted,
                            IsArchived = ci.IsArchived,
                            Year = ci.Year,
                            CompanyId = ci.CompanyId,
                            RoomId = ci.RoomId,
                            CountDate = ci.CountDate,
                            CountCompletionDate = ci.CountCompletionDate,
                            IsAutomatedCompletion = ci.IsAutomatedCompletion,
                            CompleteCauseCountGUID = ci.CompleteCauseCountGUID,
                            CreatedByName = ci_cc.UserName,
                            UpdatedByName = ci_cu.UserName,
                            RoomName = ci_rm.RoomName,
                            IsApplied = ci.IsApplied,
                            ReceivedOn = ci.ReceivedOn,
                            ReceivedOnWeb = ci.ReceivedOnWeb,
                            AddedFrom = ci.AddedFrom,
                            EditedFrom = ci.EditedFrom,
                            IsClosed = ci.IsClosed,
                            ProjectSpendGUID = ci.ProjectSpendGUID,
                            ReleaseNumber = ci.ReleaseNumber,
                            PullOrderNumber = ci.PullOrderNumber
                        }).FirstOrDefault();
            }
        }

        public InventoryCountDTO GetArchivedCountByGUIDFull(Guid Guid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", Guid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("exec [GetArchivedCountByGUIDFull] @GUID", params1).FirstOrDefault();
            }
        }

        public InventoryCountDTO SaveInventoryCount(InventoryCountDTO objInventoryCountDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool projectChanged = false;
                InventoryCount objInventoryCount = null;

                if (objInventoryCountDTO.ID > 0)
                {
                    objInventoryCount = context.InventoryCounts.FirstOrDefault(t => t.ID == objInventoryCountDTO.ID);
                    if (objInventoryCount != null)
                    {

                        objInventoryCount.CountName = objInventoryCountDTO.CountName;
                        objInventoryCount.CountItemDescription = objInventoryCountDTO.CountItemDescription;
                        objInventoryCount.CountDate = objInventoryCountDTO.CountDate;
                        objInventoryCount.UDF1 = objInventoryCountDTO.UDF1;
                        objInventoryCount.UDF2 = objInventoryCountDTO.UDF2;
                        objInventoryCount.UDF3 = objInventoryCountDTO.UDF3;
                        objInventoryCount.UDF4 = objInventoryCountDTO.UDF4;
                        objInventoryCount.UDF5 = objInventoryCountDTO.UDF5;
                        objInventoryCount.LastUpdatedBy = objInventoryCountDTO.LastUpdatedBy;
                        objInventoryCount.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCount.AddedFrom = objInventoryCountDTO.AddedFrom == null ? (objInventoryCount.AddedFrom != null ? objInventoryCount.AddedFrom : "Web") : objInventoryCountDTO.AddedFrom;
                        objInventoryCount.EditedFrom = objInventoryCountDTO.EditedFrom ?? "Web";
                        objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                        //objInventoryCount.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        if (objInventoryCountDTO != null && objInventoryCountDTO.CountType != null && (!string.IsNullOrWhiteSpace(objInventoryCountDTO.CountType)))
                            objInventoryCount.CountType = objInventoryCountDTO.CountType;
                        if (objInventoryCount.ProjectSpendGUID != objInventoryCountDTO.ProjectSpendGUID)
                        {
                            projectChanged = true;
                        }
                        objInventoryCount.ProjectSpendGUID = objInventoryCountDTO.ProjectSpendGUID;
                        objInventoryCountDTO.RoomId = objInventoryCount.RoomId;
                        objInventoryCountDTO.CompanyId = objInventoryCount.CompanyId;
                        objInventoryCountDTO.CompleteCauseCountGUID = objInventoryCount.CompleteCauseCountGUID;
                        objInventoryCountDTO.CountCompletionDate = objInventoryCount.CountCompletionDate;
                        objInventoryCountDTO.CountDate = objInventoryCount.CountDate;
                        objInventoryCountDTO.CountStatus = objInventoryCount.CountStatus;

                        objInventoryCountDTO.Created = objInventoryCount.Created;
                        objInventoryCountDTO.CreatedBy = objInventoryCount.CreatedBy;
                        objInventoryCountDTO.GUID = objInventoryCount.GUID;
                        objInventoryCountDTO.IsArchived = objInventoryCount.IsArchived;
                        objInventoryCountDTO.IsAutomatedCompletion = objInventoryCount.IsAutomatedCompletion;
                        objInventoryCountDTO.IsDeleted = objInventoryCount.IsDeleted;
                        objInventoryCountDTO.LastUpdatedBy = objInventoryCount.LastUpdatedBy;
                        objInventoryCountDTO.Updated = objInventoryCount.Updated;
                        objInventoryCountDTO.Year = objInventoryCount.Year;
                        objInventoryCountDTO.AddedFrom = objInventoryCountDTO.AddedFrom ?? "Web";
                        objInventoryCount.PullOrderNumber = objInventoryCountDTO.PullOrderNumber;
                        //objInventoryCountDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        SetInventoryCountReleaseNumber(objInventoryCountDTO);

                        objInventoryCount.ReleaseNumber = objInventoryCountDTO.ReleaseNumber;

                        if (objInventoryCountDTO.IsOnlyFromItemUI)
                        {
                            objInventoryCount.EditedFrom = "Web";
                            objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        if (projectChanged)
                        {
                            IQueryable<InventoryCountDetail> lst = context.InventoryCountDetails.Where(w => w.InventoryCountGUID == objInventoryCount.GUID && w.IsDeleted == false);
                            if (lst.Any())
                            {
                                foreach (var item in lst)
                                {
                                    item.ProjectSpendGUID = objInventoryCount.ProjectSpendGUID;
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                }
                else
                {
                    SetInventoryCountReleaseNumber(objInventoryCountDTO);

                    objInventoryCount = new InventoryCount();
                    objInventoryCount.ID = 0;
                    objInventoryCount.GUID = objInventoryCountDTO.GUID;
                    objInventoryCount.CountName = objInventoryCountDTO.CountName;
                    objInventoryCount.CountItemDescription = objInventoryCountDTO.CountItemDescription;
                    objInventoryCount.CountType = objInventoryCountDTO.CountType;
                    objInventoryCount.CountStatus = objInventoryCountDTO.CountStatus;
                    objInventoryCount.UDF1 = objInventoryCountDTO.UDF1;
                    objInventoryCount.UDF2 = objInventoryCountDTO.UDF2;
                    objInventoryCount.UDF3 = objInventoryCountDTO.UDF3;
                    objInventoryCount.UDF4 = objInventoryCountDTO.UDF4;
                    objInventoryCount.UDF5 = objInventoryCountDTO.UDF5;
                    objInventoryCount.Created = objInventoryCountDTO.Created; ;
                    objInventoryCount.Updated = objInventoryCountDTO.Updated;
                    objInventoryCount.CreatedBy = objInventoryCountDTO.CreatedBy;
                    objInventoryCount.LastUpdatedBy = objInventoryCountDTO.LastUpdatedBy;
                    objInventoryCount.IsDeleted = objInventoryCountDTO.IsDeleted;
                    objInventoryCount.IsArchived = objInventoryCountDTO.IsArchived;
                    objInventoryCount.Year = objInventoryCountDTO.Year;
                    objInventoryCount.CompanyId = objInventoryCountDTO.CompanyId;
                    objInventoryCount.RoomId = objInventoryCountDTO.RoomId;
                    objInventoryCount.CountDate = objInventoryCountDTO.CountDate;
                    objInventoryCount.CountCompletionDate = objInventoryCountDTO.CountCompletionDate;
                    objInventoryCount.IsAutomatedCompletion = objInventoryCountDTO.IsAutomatedCompletion;
                    objInventoryCount.CompleteCauseCountGUID = objInventoryCountDTO.CompleteCauseCountGUID;
                    objInventoryCount.AddedFrom = objInventoryCountDTO.AddedFrom ?? "Web";
                    objInventoryCount.EditedFrom = objInventoryCountDTO.EditedFrom ?? "Web";
                    objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objInventoryCount.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objInventoryCount.ProjectSpendGUID = objInventoryCountDTO.ProjectSpendGUID;
                    objInventoryCount.ReleaseNumber = objInventoryCountDTO.ReleaseNumber;
                    objInventoryCount.PullOrderNumber = objInventoryCountDTO.PullOrderNumber;
                    context.InventoryCounts.Add(objInventoryCount);
                    context.SaveChanges();
                    objInventoryCountDTO.ID = objInventoryCount.ID;
                    new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("nextcountnumber", objInventoryCountDTO.RoomId, objInventoryCountDTO.CompanyId, objInventoryCountDTO.CountName);
                }
            }
            return objInventoryCountDTO;
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE InventoryCount SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }

        public bool DeleteCountLineItems(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Guid inventiryCountGuid = Guid.Empty;
                long companyId = 0;
                long roomId = 0;

                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        long id = 0;
                        if (long.TryParse(item.Trim(), out id))
                        {
                            InventoryCountDetail objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == id);
                            if (objInventoryCountDetail != null)
                            {
                                objInventoryCountDetail.IsDeleted = true;
                                objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objInventoryCountDetail.EditedFrom = "Web";

                                if (companyId < 1)
                                {
                                    companyId = objInventoryCountDetail.CompanyId;
                                    roomId = objInventoryCountDetail.RoomId;
                                    inventiryCountGuid = objInventoryCountDetail.InventoryCountGUID;
                                }
                            }
                        }
                    }
                }
                context.SaveChanges();

                ApplyCountHeader(inventiryCountGuid, roomId, companyId, "Web");
            }
            return true;
        }

        public bool CloseCountItems(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        long id = 0;

                        if (long.TryParse(item.Trim(), out id))
                        {
                            InventoryCount objInventoryCount = context.InventoryCounts.FirstOrDefault(t => t.ID == id);

                            if (objInventoryCount != null)
                            {
                                objInventoryCount.IsClosed = true;
                                //objInventoryCount.IsApplied = true; // commented for WI-5443                         
                                objInventoryCount.CountStatus = Convert.ToChar(InventoryCountStatus.Close).ToString();
                                objInventoryCount.Updated = DateTimeUtility.DateTimeNow;
                                objInventoryCount.LastUpdatedBy = userid;
                                objInventoryCount.EditedFrom = "Web";
                                objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            return true;
        }
        public bool UnclosedInventoryCount(string ids, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var item in ids.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        long id = 0;

                        if (long.TryParse(item.Trim(), out id))
                        {
                            InventoryCount objInventoryCount = context.InventoryCounts.FirstOrDefault(t => t.ID == id);

                            if (objInventoryCount != null)
                            {
                                objInventoryCount.IsClosed = false;
                                //objInventoryCount.IsApplied = true; // commented for WI-5443                         
                                objInventoryCount.CountStatus = Convert.ToChar(InventoryCountStatus.Open).ToString();
                                objInventoryCount.Updated = DateTimeUtility.DateTimeNow;
                                objInventoryCount.LastUpdatedBy = userid;
                                objInventoryCount.EditedFrom = "Web";
                                objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            return true;
        }
        public List<InventoryCountDTO> GetPagedRecordsICListFromDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<InventoryCountDTO> lstInventoryCounts = new List<InventoryCountDTO>();
            TotalCount = 0;
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            DataSet dsCart = new DataSet();
            string spName = IsArchived ? "GetPagedInventoryCounts_Archive" : "GetPagedInventoryCounts";
            //string Connectionstring = ConfigurationManager.ConnectionStrings["eTurnsConnection"].ConnectionString;
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));            
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 1);
            if (objeTurnsRegionInfo == null)
            {
                objeTurnsRegionInfo = new eTurnsRegionInfo();
                objeTurnsRegionInfo.CompanyId = CompanyID;
                objeTurnsRegionInfo.CultureCode = "en-US";
                objeTurnsRegionInfo.CultureDisplayName = "en-US";
                objeTurnsRegionInfo.CultureName = "en-US";
                objeTurnsRegionInfo.CurrencyDecimalDigits = 2;
                objeTurnsRegionInfo.CurrencyGroupSeparator = null;
                objeTurnsRegionInfo.RoomId = RoomID;
                objeTurnsRegionInfo.ShortDatePattern = "M/d/yyyy";
                objeTurnsRegionInfo.ShortTimePattern = "h:mm:ss tt";
                objeTurnsRegionInfo.TimeZoneName = TimeZoneInfo.Utc.StandardName;
                objeTurnsRegionInfo.TimeZoneOffSet = 0;
            }
            if (Connectionstring == "")
            {
                return lstInventoryCounts;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string CountStatuses = null;
            string CountTypes = null;
            string CountCreaters = null;
            string CountUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;


            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, CountStatuses, CountTypes, CountCreaters, CountUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //WI-1461 related changes 
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
                    CountCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    CountUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[37]))
                {

                    string[] arrCountTypes = FieldsPara[37].Split(',');
                    foreach (string supitem in arrCountTypes)
                    {
                        CountTypes = CountTypes + supitem + "','";
                    }
                    CountTypes = CountTypes.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[38]))
                {
                    string[] arrCountStatuses = FieldsPara[38].Split(',');
                    foreach (string supitem in arrCountStatuses)
                    {
                        CountStatuses = CountStatuses + supitem + "','";
                    }
                    CountStatuses = CountStatuses.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

                }
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm.Trim(), sortColumnName, CountStatuses, CountTypes, CountCreaters, CountUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, CountStatuses, CountTypes, CountCreaters, CountUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCounts = dsCart.Tables[0];
                if (dtCounts.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCounts.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dtCounts.Rows)
                    {
                        long templong = 0;
                        //Guid? tempguid = null;
                        bool tempbool = false;
                        //double tempdouble = 0;

                        objInventoryCountDTO = new InventoryCountDTO();
                        objInventoryCountDTO.CompanyId = CompanyID;
                        objInventoryCountDTO.RoomId = RoomID;
                        if (dtCounts.Columns.Contains("Created"))
                        {
                            objInventoryCountDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtCounts.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objInventoryCountDTO.CreatedBy = templong;
                        }
                        if (dtCounts.Columns.Contains("CreatedByName"))
                        {
                            objInventoryCountDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                        }
                        if (dtCounts.Columns.Contains("GUID"))
                        {
                            objInventoryCountDTO.GUID = (Guid)dr["GUID"];
                        }
                        if (dtCounts.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objInventoryCountDTO.ID = templong;
                        }
                        if (dtCounts.Columns.Contains("CountName"))
                        {
                            objInventoryCountDTO.CountName = Convert.ToString(dr["CountName"]);
                        }
                        if (dtCounts.Columns.Contains("CountItemDescription"))
                        {
                            objInventoryCountDTO.CountItemDescription = Convert.ToString(dr["CountItemDescription"]);
                        }
                        if (dtCounts.Columns.Contains("CountType"))
                        {
                            objInventoryCountDTO.CountType = Convert.ToString(dr["CountType"]);
                            if (!string.IsNullOrEmpty(objInventoryCountDTO.CountType))
                            {
                                if (objInventoryCountDTO.CountType == "M")
                                {
                                    objInventoryCountDTO.CountType = "Manual";
                                }
                                if (objInventoryCountDTO.CountType == "C")
                                {
                                    objInventoryCountDTO.CountType = "Cycle";
                                }
                                if (objInventoryCountDTO.CountType == "A")
                                {
                                    objInventoryCountDTO.CountType = "Adjustment";
                                }
                            }
                        }
                        if (dtCounts.Columns.Contains("Year"))
                        {
                            short tempyesr = 0;
                            short.TryParse(Convert.ToString(dr["Year"]), out tempyesr);
                            objInventoryCountDTO.Year = tempyesr;
                        }
                        if (dtCounts.Columns.Contains("CountDate"))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["CountDate"])))
                            {
                                objInventoryCountDTO.CountDate = Convert.ToDateTime(dr["CountDate"]);
                            }
                            else
                            {
                                objInventoryCountDTO.CountDate = DateTime.MinValue;
                            }
                            objInventoryCountDTO.CountDateDisplay = DateTimeUtility.ConvertDateByTimeZone(objInventoryCountDTO.CountDate, objeTurnsRegionInfo.TimeZoneName, objeTurnsRegionInfo.ShortDatePattern, objeTurnsRegionInfo.CultureCode, false);

                        }
                        if (dtCounts.Columns.Contains("CountCompletionDate"))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["CountCompletionDate"])))
                            {
                                objInventoryCountDTO.CountCompletionDate = Convert.ToDateTime(dr["CountCompletionDate"]);
                            }
                        }
                        if (dtCounts.Columns.Contains("IsAutomatedCompletion"))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["IsAutomatedCompletion"])))
                            {
                                objInventoryCountDTO.IsAutomatedCompletion = Convert.ToBoolean(dr["IsAutomatedCompletion"]);
                            }
                            else
                            {
                                objInventoryCountDTO.IsAutomatedCompletion = null;
                            }
                        }
                        if (dtCounts.Columns.Contains("CompleteCauseCountGUID"))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["CompleteCauseCountGUID"])))
                            {
                                objInventoryCountDTO.CompleteCauseCountGUID = (Guid)dr["CompleteCauseCountGUID"];
                            }
                            else
                            {
                                objInventoryCountDTO.CompleteCauseCountGUID = null;
                            }
                        }
                        if (dtCounts.Columns.Contains("CountStatus"))
                        {

                            objInventoryCountDTO.CountStatus = Convert.ToString(dr["CountStatus"]);
                            if (!string.IsNullOrEmpty(objInventoryCountDTO.CountStatus))
                            {
                                if (objInventoryCountDTO.CountStatus == "O")
                                {
                                    objInventoryCountDTO.CountStatus = "Open";
                                }
                                if (objInventoryCountDTO.CountStatus == "C")
                                {
                                    objInventoryCountDTO.CountStatus = "Closed";
                                }
                            }
                        }
                        if (dtCounts.Columns.Contains("IsApplied"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsApplied"]), out tempbool);
                            objInventoryCountDTO.IsApplied = tempbool;
                        }
                        if (dtCounts.Columns.Contains("IsClosed"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsClosed"]), out tempbool);
                            objInventoryCountDTO.IsClosed = tempbool;
                        }
                        if (dtCounts.Columns.Contains("IsArchived"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempbool);
                            objInventoryCountDTO.IsArchived = tempbool;
                        }
                        if (dtCounts.Columns.Contains("IsDeleted"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempbool);
                            objInventoryCountDTO.IsDeleted = tempbool;
                        }
                        if (dtCounts.Columns.Contains("LastUpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            objInventoryCountDTO.LastUpdatedBy = templong;
                        }
                        if (dtCounts.Columns.Contains("UDF1"))
                        {
                            objInventoryCountDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCounts.Columns.Contains("UDF2"))
                        {
                            objInventoryCountDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCounts.Columns.Contains("UDF3"))
                        {
                            objInventoryCountDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCounts.Columns.Contains("UDF4"))
                        {
                            objInventoryCountDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCounts.Columns.Contains("UDF5"))
                        {
                            objInventoryCountDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtCounts.Columns.Contains("Updated"))
                        {
                            objInventoryCountDTO.Updated = Convert.ToDateTime(dr["Updated"]);
                        }
                        if (dtCounts.Columns.Contains("UpdatedByName"))
                        {
                            objInventoryCountDTO.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }
                        if (dtCounts.Columns.Contains("TotalItemsWithinCount"))
                        {
                            int tempint = 0;
                            int.TryParse(Convert.ToString(dr["TotalItemsWithinCount"]), out tempint);
                            objInventoryCountDTO.TotalItemsWithinCount = tempint;
                        }
                        if (dtCounts.Columns.Contains("AddedFrom"))
                        {
                            objInventoryCountDTO.AddedFrom = Convert.ToString(dr["AddedFrom"]);
                        }
                        if (dtCounts.Columns.Contains("EditedFrom"))
                        {
                            objInventoryCountDTO.EditedFrom = Convert.ToString(dr["EditedFrom"]);
                        }
                        if (dtCounts.Columns.Contains("ReceivedOn"))
                        {
                            objInventoryCountDTO.ReceivedOn = Convert.ToDateTime(dr["ReceivedOn"]);
                        }
                        if (dtCounts.Columns.Contains("ReceivedOnWeb"))
                        {
                            objInventoryCountDTO.ReceivedOnWeb = Convert.ToDateTime(dr["ReceivedOnWeb"]);
                        }
                        if (dtCounts.Columns.Contains("ProjectSpendName"))
                        {
                            objInventoryCountDTO.ProjectSpendName = Convert.ToString(dr["ProjectSpendName"]);
                        }
                        if (dtCounts.Columns.Contains("ReleaseNumber"))
                        {
                            objInventoryCountDTO.ReleaseNumber = Convert.ToString(dr["ReleaseNumber"]);
                        }
                        lstInventoryCounts.Add(objInventoryCountDTO);
                    }
                }
            }
            return lstInventoryCounts;
        }

        public List<InventoryCountDTO> GetPagedInventoryCountForDashboard(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat)
        {
            List<InventoryCountDTO> lstInventoryCounts = new List<InventoryCountDTO>();
            TotalCount = 0;
            DataSet dsCount = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 1);

            if (objeTurnsRegionInfo == null)
            {
                objeTurnsRegionInfo = new eTurnsRegionInfo();
                objeTurnsRegionInfo.CompanyId = CompanyID;
                objeTurnsRegionInfo.CultureCode = "en-US";
                objeTurnsRegionInfo.CultureDisplayName = "en-US";
                objeTurnsRegionInfo.CultureName = "en-US";
                objeTurnsRegionInfo.CurrencyDecimalDigits = 2;
                objeTurnsRegionInfo.CurrencyGroupSeparator = null;
                objeTurnsRegionInfo.RoomId = RoomID;
                objeTurnsRegionInfo.ShortDatePattern = "M/d/yyyy";
                objeTurnsRegionInfo.ShortTimePattern = "h:mm:ss tt";
                objeTurnsRegionInfo.TimeZoneName = TimeZoneInfo.Utc.StandardName;
                objeTurnsRegionInfo.TimeZoneOffSet = 0;
            }

            if (Connectionstring == "")
            {
                return lstInventoryCounts;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsCount = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryCountForDashboard", StartRowIndex, MaxRows, SearchTerm, sortColumnName, IsDeleted, IsArchived, RoomID, CompanyID);

            if (dsCount != null && dsCount.Tables.Count > 0)
            {
                lstInventoryCounts = DataTableHelper.ToList<InventoryCountDTO>(dsCount.Tables[0]);

                if (lstInventoryCounts != null && lstInventoryCounts.Any())
                {
                    TotalCount = lstInventoryCounts.ElementAt(0).TotalRecords ?? 0;
                    lstInventoryCounts.ForEach(z => z.CountDateDisplay = DateTimeUtility.ConvertDateByTimeZone(z.CountDate, objeTurnsRegionInfo.TimeZoneName, objeTurnsRegionInfo.ShortDatePattern, objeTurnsRegionInfo.CultureCode, false));
                }
            }

            return lstInventoryCounts;
        }

        public bool IsNameCountNameExist(long ID, string CountName, long RoomId, long CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.InventoryCounts
                           where em.CountName == CountName && em.IsArchived == false && em.IsDeleted == false && em.ID != ID && em.CompanyId == CompanyId && em.RoomId == RoomId
                           select em);
                if (qry.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public List<InventoryCountDetailDTO> GetPagedCountLineItems(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string CountGUID, List<long> SupplierIds)
        {
            List<InventoryCountDetailDTO> lstCountLineItems = new List<InventoryCountDetailDTO>();
            TotalCount = 0;
            InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            DataSet dsCountLineItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            string spName = IsArchived ? "GetPagedInventoryCountLineItems_Archive" : "GetPagedInventoryCountLineItems";

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstCountLineItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCountLineItems = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, IsDeleted, IsArchived, RoomID, CompanyID, CountGUID, strSupplierIds);
            }
            else
            {
                dsCountLineItems = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, IsDeleted, IsArchived, RoomID, CompanyID, CountGUID, strSupplierIds);
            }

            if (dsCountLineItems != null && dsCountLineItems.Tables.Count > 0)
            {
                DataTable dtCounts = dsCountLineItems.Tables[0];
                if (dtCounts.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCounts.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dtCounts.Rows)
                    {
                        long templong = 0;
                        //Guid? tempguid = null;
                        bool tempbool = false;
                        double tempdouble = 0;
                        Guid tempguid = Guid.Empty;

                        objInventoryCountDetailDTO = new InventoryCountDetailDTO();
                        objInventoryCountDetailDTO.CompanyId = CompanyID;

                        if (dtCounts.Columns.Contains("Created"))
                        {
                            objInventoryCountDetailDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtCounts.Columns.Contains("BinNumber"))
                        {
                            objInventoryCountDetailDTO.BinNumber = Convert.ToString(dr["BinNumber"]);
                        }
                        if (dtCounts.Columns.Contains("ItemNumber"))
                        {
                            objInventoryCountDetailDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCounts.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objInventoryCountDetailDTO.CreatedBy = templong;
                        }
                        if (dtCounts.Columns.Contains("CreatedByName"))
                        {
                            objInventoryCountDetailDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                        }
                        if (dtCounts.Columns.Contains("GUID"))
                        {
                            objInventoryCountDetailDTO.GUID = (Guid)dr["GUID"];
                        }
                        if (dtCounts.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objInventoryCountDetailDTO.ID = templong;
                        }
                        if (dtCounts.Columns.Contains("CountName"))
                        {
                            objInventoryCountDetailDTO.CountName = Convert.ToString(dr["CountName"]);
                        }
                        if (dtCounts.Columns.Contains("CountLineItemDescription"))
                        {
                            if (!string.IsNullOrWhiteSpace(Convert.ToString(dr["CountLineItemDescription"])))
                            {
                                objInventoryCountDetailDTO.CountLineItemDescription = Convert.ToString(dr["CountLineItemDescription"]);
                            }
                            else
                            {
                                objInventoryCountDetailDTO.CountLineItemDescription = string.Empty;
                            }
                        }
                        if (dtCounts.Columns.Contains("CountType"))
                        {
                            objInventoryCountDetailDTO.CountType = Convert.ToString(dr["CountType"]);
                            if (!string.IsNullOrEmpty(objInventoryCountDetailDTO.CountType))
                            {
                                if (objInventoryCountDetailDTO.CountType == "M")
                                {
                                    objInventoryCountDetailDTO.CountType = "Manual";
                                }
                                if (objInventoryCountDetailDTO.CountType == "C")
                                {
                                    objInventoryCountDetailDTO.CountType = "Cycle";
                                }
                            }
                        }

                        if (dtCounts.Columns.Contains("Consignment"))
                        {
                            bool ConsignmentVal = false;
                            bool.TryParse(Convert.ToString(dr["Consignment"]), out ConsignmentVal);
                            objInventoryCountDetailDTO.Consignment = ConsignmentVal;
                        }

                        if (dtCounts.Columns.Contains("CountDate"))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["CountDate"])))
                            {
                                objInventoryCountDetailDTO.CountDate = Convert.ToDateTime(dr["CountDate"]);
                            }
                            else
                            {
                                objInventoryCountDetailDTO.CountDate = DateTime.MinValue;
                            }
                        }

                        if (dtCounts.Columns.Contains("CountStatus"))
                        {

                            objInventoryCountDetailDTO.CountStatus = Convert.ToString(dr["CountStatus"]);
                            if (!string.IsNullOrEmpty(objInventoryCountDetailDTO.CountStatus))
                            {
                                if (objInventoryCountDetailDTO.CountStatus == "O")
                                {
                                    objInventoryCountDetailDTO.CountStatus = "Open";
                                }
                                if (objInventoryCountDetailDTO.CountStatus == "C")
                                {
                                    objInventoryCountDetailDTO.CountStatus = "Closed";
                                }
                            }
                        }
                        if (dtCounts.Columns.Contains("IsApplied"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsApplied"]), out tempbool);
                            objInventoryCountDetailDTO.IsApplied = tempbool;
                        }
                        if (dtCounts.Columns.Contains("IsClosed"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsClosed"]), out tempbool);
                            objInventoryCountDetailDTO.IsClosed = tempbool;
                        }
                        if (dtCounts.Columns.Contains("IsArchived"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempbool);
                            objInventoryCountDetailDTO.IsArchived = tempbool;
                        }
                        if (dtCounts.Columns.Contains("IsDeleted"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempbool);
                            objInventoryCountDetailDTO.IsDeleted = tempbool;
                        }
                        if (dtCounts.Columns.Contains("LastUpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            objInventoryCountDetailDTO.LastUpdatedBy = templong;
                        }
                        if (dtCounts.Columns.Contains("UDF1"))
                        {
                            objInventoryCountDetailDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCounts.Columns.Contains("UDF2"))
                        {
                            objInventoryCountDetailDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCounts.Columns.Contains("UDF3"))
                        {
                            objInventoryCountDetailDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCounts.Columns.Contains("UDF4"))
                        {
                            objInventoryCountDetailDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCounts.Columns.Contains("UDF5"))
                        {
                            objInventoryCountDetailDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtCounts.Columns.Contains("PullOrderNumber"))
                        {
                            objInventoryCountDetailDTO.PullOrderNumber = Convert.ToString(dr["PullOrderNumber"]);
                        }
                        if (dtCounts.Columns.Contains("Updated"))
                        {
                            objInventoryCountDetailDTO.Updated = Convert.ToDateTime(dr["Updated"]);
                        }
                        if (dtCounts.Columns.Contains("UpdatedByName"))
                        {
                            objInventoryCountDetailDTO.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }
                        if (dtCounts.Columns.Contains("CountConsignedQuantity"))
                        {
                            if (double.TryParse(Convert.ToString(dr["CountConsignedQuantity"]), out tempdouble))
                            {
                                if (tempdouble == (-0.000000001))
                                {
                                    objInventoryCountDetailDTO.CountConsignedQuantity = null;
                                }
                                else
                                {
                                    objInventoryCountDetailDTO.CountConsignedQuantity = tempdouble;
                                }

                            }
                        }
                        if (dtCounts.Columns.Contains("CountCustOwnedQuantity"))
                        {
                            if (double.TryParse(Convert.ToString(dr["CountCustOwnedQuantity"]), out tempdouble))
                            {
                                if (tempdouble == (-0.000000001))
                                {
                                    objInventoryCountDetailDTO.CountCustomerOwnedQuantity = null;
                                }
                                else
                                {
                                    objInventoryCountDetailDTO.CountCustomerOwnedQuantity = tempdouble;
                                }
                            }
                        }
                        if (dtCounts.Columns.Contains("IsStagingLocationCount"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsStagingLocationCount"]), out tempbool);
                            objInventoryCountDetailDTO.IsStagingLocationCount = tempbool;
                        }

                        if (dtCounts.Columns.Contains("ItemConsignedQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemConsignedQuantity"]), out tempdouble);
                            objInventoryCountDetailDTO.ConsignedQuantity = tempdouble;
                        }
                        if (dtCounts.Columns.Contains("ConsignedDifference"))
                        {
                            if (double.TryParse(Convert.ToString(dr["ConsignedDifference"]), out tempdouble))
                            {
                                if (tempdouble == (-0.000000001))
                                {
                                    objInventoryCountDetailDTO.ConsignedDifference = null;
                                }
                                else
                                {
                                    if (Convert.ToString(tempdouble).IndexOf('.', 0) > -1)
                                    {
                                        decimal consdiff;
                                        if (Decimal.TryParse(Convert.ToString(tempdouble), out consdiff))
                                            tempdouble = Convert.ToDouble(Math.Round(consdiff));
                                    }

                                    objInventoryCountDetailDTO.ConsignedDifference = tempdouble;
                                }
                            }
                        }
                        if (dtCounts.Columns.Contains("CusOwnedDifference"))
                        {
                            if (double.TryParse(Convert.ToString(dr["CusOwnedDifference"]), out tempdouble))
                            {
                                if (tempdouble == (-0.000000001))
                                {
                                    objInventoryCountDetailDTO.CusOwnedDifference = null;
                                }
                                else
                                {
                                    if (Convert.ToString(tempdouble).IndexOf('.', 0) > -1)
                                    {
                                        decimal custdiff;
                                        if (Decimal.TryParse(Convert.ToString(tempdouble), out custdiff))
                                            tempdouble = Convert.ToDouble(Math.Round(custdiff));
                                    }

                                    objInventoryCountDetailDTO.CusOwnedDifference = tempdouble;
                                }
                            }

                        }

                        if (dtCounts.Columns.Contains("ItemCustOwnedQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemCustOwnedQuantity"]), out tempdouble);
                            objInventoryCountDetailDTO.CustomerOwnedQuantity = tempdouble;
                        }

                        if (dtCounts.Columns.Contains("SerialNumberTracking"))
                        {
                            objInventoryCountDetailDTO.SerialNumberTracking = Convert.ToBoolean(dr["SerialNumberTracking"]);
                        }

                        if (dtCounts.Columns.Contains("LotNumberTracking"))
                        {
                            objInventoryCountDetailDTO.LotNumberTracking = Convert.ToBoolean(dr["LotNumberTracking"]);
                        }

                        if (dtCounts.Columns.Contains("InventoryClassification"))
                        {
                            string InventoryClassificationCode = Convert.ToString(dr["InventoryClassification"]);
                            switch (InventoryClassificationCode)
                            {
                                case "1":
                                    objInventoryCountDetailDTO.InventoryClassificationCode = "A";
                                    break;
                                case "2":
                                    objInventoryCountDetailDTO.InventoryClassificationCode = "B";
                                    break;
                                case "3":
                                    objInventoryCountDetailDTO.InventoryClassificationCode = "C";
                                    break;
                                case "4":
                                    objInventoryCountDetailDTO.InventoryClassificationCode = "D";
                                    break;
                                case "5":
                                    objInventoryCountDetailDTO.InventoryClassificationCode = "E";
                                    break;
                                default:
                                    objInventoryCountDetailDTO.InventoryClassificationCode = string.Empty;
                                    break;

                            }
                        }
                        objInventoryCountDetailDTO.CountConsignedQuantityEntry = objInventoryCountDetailDTO.CountConsignedQuantity;
                        objInventoryCountDetailDTO.CountCustomerOwnedQuantityEntry = objInventoryCountDetailDTO.CountCustomerOwnedQuantity;
                        objInventoryCountDetailDTO.CountLineItemDescriptionEntry = objInventoryCountDetailDTO.CountLineItemDescription;
                        if (dtCounts.Columns.Contains("AddedFrom"))
                        {
                            objInventoryCountDetailDTO.AddedFrom = Convert.ToString(dr["AddedFrom"]);
                        }
                        if (dtCounts.Columns.Contains("EditedFrom"))
                        {
                            objInventoryCountDetailDTO.EditedFrom = Convert.ToString(dr["EditedFrom"]);
                        }
                        if (dtCounts.Columns.Contains("ReceivedOn"))
                        {
                            objInventoryCountDetailDTO.ReceivedOn = Convert.ToDateTime(dr["ReceivedOn"]);
                        }
                        if (dtCounts.Columns.Contains("ReceivedOnWeb"))
                        {
                            objInventoryCountDetailDTO.ReceivedOnWeb = Convert.ToDateTime(dr["ReceivedOnWeb"]);
                        }
                        objInventoryCountDetailDTO.ItemType = Convert.ToInt32(dr["ItemType"]);
                        if (dtCounts.Columns.Contains("ITEMUDF1"))
                        {
                            objInventoryCountDetailDTO.ItemUDF1 = Convert.ToString(dr["ITEMUDF1"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF2"))
                        {
                            objInventoryCountDetailDTO.ItemUDF2 = Convert.ToString(dr["ITEMUDF2"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF3"))
                        {
                            objInventoryCountDetailDTO.ItemUDF3 = Convert.ToString(dr["ITEMUDF3"]);
                        }
                        if (dtCounts.Columns.Contains("ItemUDF4"))
                        {
                            objInventoryCountDetailDTO.ItemUDF4 = Convert.ToString(dr["ITEMUDF4"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF5"))
                        {
                            objInventoryCountDetailDTO.ItemUDF5 = Convert.ToString(dr["ITEMUDF5"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF6"))
                        {
                            objInventoryCountDetailDTO.ItemUDF6 = Convert.ToString(dr["ITEMUDF6"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF7"))
                        {
                            objInventoryCountDetailDTO.ItemUDF7 = Convert.ToString(dr["ITEMUDF7"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF8"))
                        {
                            objInventoryCountDetailDTO.ItemUDF8 = Convert.ToString(dr["ITEMUDF8"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF9"))
                        {
                            objInventoryCountDetailDTO.ItemUDF9 = Convert.ToString(dr["ITEMUDF9"]);
                        }
                        if (dtCounts.Columns.Contains("ITEMUDF10"))
                        {
                            objInventoryCountDetailDTO.ItemUDF10 = Convert.ToString(dr["ITEMUDF10"]);
                        }
                        if (dtCounts.Columns.Contains("DateCodeTracking"))
                        {
                            objInventoryCountDetailDTO.DateCodeTracking = Convert.ToBoolean(dr["DateCodeTracking"]);
                        }
                        if (dtCounts.Columns.Contains("ItemGUID"))
                        {
                            objInventoryCountDetailDTO.ItemGUID = (Guid)(dr["ItemGUID"]);
                        }
                        if (dtCounts.Columns.Contains("ItemDescription"))
                        {
                            objInventoryCountDetailDTO.ItemDescription = Convert.ToString(dr["ItemDescription"]);
                        }
                        if (dtCounts.Columns.Contains("SupplierAccountGuid"))
                        {
                            tempguid = Guid.Empty;
                            Guid.TryParse(Convert.ToString(dr["SupplierAccountGuid"]), out tempguid);
                            if (tempguid != Guid.Empty)
                                objInventoryCountDetailDTO.SupplierAccountGuid = tempguid;
                        }
                        if (dtCounts.Columns.Contains("SupplierAccountNo"))
                        {
                            objInventoryCountDetailDTO.SupplierAccountNo = Convert.ToString(dr["SupplierAccountNo"]);
                        }
                        lstCountLineItems.Add(objInventoryCountDetailDTO);
                    }
                }
            }
            return lstCountLineItems;
        }

        public List<ItemMasterDTO> GetPagedItemLocations(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long BinID, bool ShowStagingLocation, string BinId_ItemGuid, List<long> SupplierIds)
        {
            string ItemLocationTOExclude = string.Empty;
            if (!string.IsNullOrWhiteSpace(BinId_ItemGuid))
            {
                foreach (var item in BinId_ItemGuid.Split(','))
                {
                    string[] arrReplenishTypes = BinId_ItemGuid.Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemLocationTOExclude = ItemLocationTOExclude + supitem + "','";
                    }
                    ItemLocationTOExclude = ItemLocationTOExclude.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
            }

            List<ItemMasterDTO> ItemLocations = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO ObjItemMasterDTO = new ItemMasterDTO();
            DataSet dsItemLocations = new DataSet();
            string ItemSuppliers = string.Empty;
            string ItemManufactures = string.Empty;
            string ItemCategories = string.Empty;
            string ItemCosts = string.Empty;
            string ItemStckStatus = string.Empty;
            string ItemType = string.Empty;
            string ItemCreaters = string.Empty;
            string ItemUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            string UDF1 = string.Empty;
            string UDF2 = string.Empty;
            string UDF3 = string.Empty;
            string UDF4 = string.Empty;
            string UDF5 = string.Empty;
            string Spname = "GetPagedItemLocations";

            if (ShowStagingLocation)
            {
                Spname = "GetPagedItemStaggingLocations";
            }

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return ItemLocations;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);


            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsItemLocations = SqlHelper.ExecuteDataset(EturnsConnection, Spname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemSuppliers, ItemManufactures, ItemCategories, ItemCosts, ItemStckStatus, ItemType, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, string.Empty, ItemLocationTOExclude, strSupplierIds);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                string Cost = null;
                string Cost1 = null;
                SearchTerm = string.Empty;
                string BinIds = string.Empty;
                if (!string.IsNullOrWhiteSpace(FieldsPara[25]))
                {
                    BinIds = FieldsPara[25].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    ItemManufactures = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategories = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }

                dsItemLocations = SqlHelper.ExecuteDataset(EturnsConnection, Spname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemSuppliers, ItemManufactures, ItemCategories, ItemCosts, ItemStckStatus, ItemType, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, BinIds, ItemLocationTOExclude, strSupplierIds);
            }
            else
            {
                dsItemLocations = SqlHelper.ExecuteDataset(EturnsConnection, Spname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemSuppliers, ItemManufactures, ItemCategories, ItemCosts, ItemStckStatus, ItemType, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, BinID, ItemLocationTOExclude, strSupplierIds);
            }

            if (dsItemLocations != null && dsItemLocations.Tables.Count > 0)
            {
                DataTable dtCounts = dsItemLocations.Tables[0];
                if (dtCounts.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCounts.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dtCounts.Rows)
                    {
                        long templong = 0;
                        //Guid? tempguid = null;
                        bool tempbool = false;
                        double tempdouble = 0;
                        ObjItemMasterDTO = new ItemMasterDTO();
                        ObjItemMasterDTO.CompanyID = CompanyID;
                        ObjItemMasterDTO.Room = RoomID;
                        if (dtCounts.Columns.Contains("Created"))
                        {
                            ObjItemMasterDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtCounts.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            ObjItemMasterDTO.CreatedBy = templong;
                        }
                        if (dtCounts.Columns.Contains("CreatedByName"))
                        {
                            ObjItemMasterDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                        }
                        if (dtCounts.Columns.Contains("GUID"))
                        {
                            ObjItemMasterDTO.GUID = (Guid)dr["GUID"];
                        }
                        if (dtCounts.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            ObjItemMasterDTO.ID = templong;
                        }

                        if (dtCounts.Columns.Contains("ItemNumber"))
                        {
                            ObjItemMasterDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCounts.Columns.Contains("BinNumber"))
                        {
                            ObjItemMasterDTO.BinNumber = Convert.ToString(dr["BinNumber"]);
                        }
                        if (dtCounts.Columns.Contains("BinID"))
                        {
                            long.TryParse(Convert.ToString(dr["BinID"]), out templong);
                            ObjItemMasterDTO.BinID = templong;
                        }
                        if (dtCounts.Columns.Contains("StagingBinId"))
                        {
                            long.TryParse(Convert.ToString(dr["StagingBinId"]), out templong);
                            ObjItemMasterDTO.BinID = templong;
                        }
                        if (dtCounts.Columns.Contains("CustomerOwnedQuantity"))
                        {
                            if (double.TryParse(Convert.ToString(dr["CustomerOwnedQuantity"]), out tempdouble))
                            {
                                ObjItemMasterDTO.CustomerOwnedQuantity = tempdouble;
                            }
                            else
                            {
                                ObjItemMasterDTO.CustomerOwnedQuantity = 0;
                            }

                        }
                        if (dtCounts.Columns.Contains("ConsignedQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ConsignedQuantity"]), out tempdouble);
                            ObjItemMasterDTO.ConsignedQuantity = tempdouble;
                        }
                        if (dtCounts.Columns.Contains("IsArchived"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempbool);
                            ObjItemMasterDTO.IsArchived = tempbool;
                        }
                        if (dtCounts.Columns.Contains("IsDeleted"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempbool);
                            ObjItemMasterDTO.IsDeleted = tempbool;
                        }
                        if (dtCounts.Columns.Contains("LastUpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            ObjItemMasterDTO.LastUpdatedBy = templong;
                        }
                        if (dtCounts.Columns.Contains("UDF1"))
                        {
                            ObjItemMasterDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCounts.Columns.Contains("UDF2"))
                        {
                            ObjItemMasterDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCounts.Columns.Contains("UDF3"))
                        {
                            ObjItemMasterDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCounts.Columns.Contains("UDF4"))
                        {
                            ObjItemMasterDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCounts.Columns.Contains("UDF5"))
                        {
                            ObjItemMasterDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }

                        if (dtCounts.Columns.Contains("UDF1"))
                        {
                            ObjItemMasterDTO.ItemUDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCounts.Columns.Contains("UDF2"))
                        {
                            ObjItemMasterDTO.ItemUDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCounts.Columns.Contains("UDF3"))
                        {
                            ObjItemMasterDTO.ItemUDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCounts.Columns.Contains("UDF4"))
                        {
                            ObjItemMasterDTO.ItemUDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCounts.Columns.Contains("UDF5"))
                        {
                            ObjItemMasterDTO.ItemUDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtCounts.Columns.Contains("Updated"))
                        {
                            ObjItemMasterDTO.Updated = Convert.ToDateTime(dr["Updated"]);
                        }
                        if (dtCounts.Columns.Contains("UpdatedByName"))
                        {
                            ObjItemMasterDTO.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }
                        if (dtCounts.Columns.Contains("SerialNumberTracking"))
                        {
                            ObjItemMasterDTO.SerialNumberTracking = Convert.ToBoolean(dr["SerialNumberTracking"]);
                        }
                        ItemLocations.Add(ObjItemMasterDTO);
                    }
                }
            }
            return ItemLocations;
        }

        public List<ItemMasterDTO> GetPagedItemLocationsForCount(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long BinID, bool ShowStagingLocation, string BinId_ItemGuid, List<long> SupplierIds, string CountGUID, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string ItemLocationTOExclude = string.Empty;

            // For optimization wi-1455
            //if (!string.IsNullOrWhiteSpace(BinId_ItemGuid))
            //{
            //    foreach (var item in BinId_ItemGuid.Split(','))
            //    {
            //        string[] arrReplenishTypes = BinId_ItemGuid.Split(',');
            //        foreach (string supitem in arrReplenishTypes)
            //        {
            //            ItemLocationTOExclude = ItemLocationTOExclude + supitem + "','";
            //        }
            //        ItemLocationTOExclude = ItemLocationTOExclude.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
            //    }
            //}
            List<ItemMasterDTO> ItemLocations = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO ObjItemMasterDTO = new ItemMasterDTO();
            DataSet dsItemLocations = new DataSet();

            string ItemLocationsIDs = null;
            string ItemSuppliers = null;
            string ItemManufactures = null;
            string ItemCategories = null;
            string ItemStckStatus = null;
            string ItemType = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string Spname = "GetPagedItemLocationsForCount";
            string StockStatus = null;
            string ItemTrackingType = null;
            string IsActive = null;
            string InventoryClassification = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string Cost = null;
            string Cost1 = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (ShowStagingLocation)
            {
                Spname = "GetPagedItemStaggingLocations";
            }
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return ItemLocations;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);


            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsItemLocations = SqlHelper.ExecuteDataset(EturnsConnection, Spname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemSuppliers, ItemManufactures, ItemCategories, Cost, ItemStckStatus, ItemType, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, string.Empty, ItemLocationTOExclude, strSupplierIds, StockStatus, CountGUID, Cost1, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, InventoryClassification, ItemTrackingType, ItemLocationsIDs, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
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
                string BinIds = string.Empty;
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemLocationsIDs = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemManufactures = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    ItemTrackingType = FieldsPara[2].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    ItemType = FieldsPara[3].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    ItemSuppliers = FieldsPara[4].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    ItemCategories = FieldsPara[5].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    StockStatus = ItemStckStatus = FieldsPara[6].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    IsActive = FieldsPara[7].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    if (FieldsPara[8].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[8].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[8].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[8].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[8].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[8].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                    //  Cost = FieldsPara[15].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    if (FieldsPara[9].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[9].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[9].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[9].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[9].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        AvgUsageTo = (Fields[1].Split('@')[9].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    if (FieldsPara[10].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[10].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[10].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[10].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[10].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        turnsTo = (Fields[1].Split('@')[10].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    InventoryClassification = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[12]))
                {
                    ItemCreaters = FieldsPara[12].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[13]))
                {
                    ItemUpdators = FieldsPara[13].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[14]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[14].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[14].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[15].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[15].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[16]))
                {
                    string[] arrReplenishTypes = FieldsPara[16].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[17]))
                {
                    string[] arrReplenishTypes = FieldsPara[17].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[18]))
                {
                    string[] arrReplenishTypes = FieldsPara[18].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[19]))
                {
                    string[] arrReplenishTypes = FieldsPara[19].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[20]))
                {
                    string[] arrReplenishTypes = FieldsPara[20].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    string[] arrReplenishTypes = FieldsPara[21].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + "','";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    string[] arrReplenishTypes = FieldsPara[22].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + "','";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    string[] arrReplenishTypes = FieldsPara[23].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + "','";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[24]))
                {
                    string[] arrReplenishTypes = FieldsPara[24].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + "','";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[25]))
                {
                    string[] arrReplenishTypes = FieldsPara[25].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + "','";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }

                dsItemLocations = SqlHelper.ExecuteDataset(EturnsConnection, Spname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemSuppliers, ItemManufactures, ItemCategories, Cost, ItemStckStatus, ItemType, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, BinIds, ItemLocationTOExclude, strSupplierIds, StockStatus, CountGUID, Cost1, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, InventoryClassification, ItemTrackingType, ItemLocationsIDs, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else
            {
                dsItemLocations = SqlHelper.ExecuteDataset(EturnsConnection, Spname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemSuppliers, ItemManufactures, ItemCategories, Cost, ItemStckStatus, ItemType, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, BinID, ItemLocationTOExclude, strSupplierIds, StockStatus, CountGUID, Cost1, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, InventoryClassification, ItemTrackingType, ItemLocationsIDs, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }


            if (dsItemLocations != null && dsItemLocations.Tables.Count > 0)
            {
                DataTable dtCounts = dsItemLocations.Tables[0];
                if (dtCounts.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCounts.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dtCounts.Rows)
                    {
                        long templong = 0;
                        //Guid? tempguid = null;                        
                        double tempdouble = 0;
                        ObjItemMasterDTO = new ItemMasterDTO();
                        ObjItemMasterDTO.CompanyID = CompanyID;
                        ObjItemMasterDTO.Room = RoomID;
                        if (dtCounts.Columns.Contains("Created"))
                        {
                            ObjItemMasterDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtCounts.Columns.Contains("CreatedBy"))
                        {
                            long templongCreatedBy;
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templongCreatedBy);
                            ObjItemMasterDTO.CreatedBy = templongCreatedBy;
                        }
                        if (dtCounts.Columns.Contains("CreatedUser"))
                        {
                            ObjItemMasterDTO.CreatedByName = Convert.ToString(dr["CreatedUser"]);
                        }
                        if (dtCounts.Columns.Contains("ItemLastUpdatedBy"))
                        {
                            long templongItemLastUpdatedBy;
                            long.TryParse(Convert.ToString(dr["ItemLastUpdatedBy"]), out templongItemLastUpdatedBy);
                            ObjItemMasterDTO.LastUpdatedBy = templongItemLastUpdatedBy;
                        }
                        if (dtCounts.Columns.Contains("UpdatedUser"))
                        {
                            ObjItemMasterDTO.UpdatedByName = Convert.ToString(dr["UpdatedUser"]);
                        }
                        if (dtCounts.Columns.Contains("GUID"))
                        {
                            ObjItemMasterDTO.GUID = (Guid)dr["GUID"];
                        }
                        if (dtCounts.Columns.Contains("ID"))
                        {
                            long templongID;
                            long.TryParse(Convert.ToString(dr["ID"]), out templongID);
                            ObjItemMasterDTO.ID = templongID;
                        }

                        if (dtCounts.Columns.Contains("ItemNumber"))
                        {
                            ObjItemMasterDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCounts.Columns.Contains("BinNumber"))
                        {
                            ObjItemMasterDTO.BinNumber = (Convert.ToString(dr["BinNumber"]).Equals("[|EmptyStagingBin|]") ? "" : Convert.ToString(dr["BinNumber"]));
                        }
                        if (dtCounts.Columns.Contains("BinID"))
                        {
                            long templongBinID;
                            long.TryParse(Convert.ToString(dr["BinID"]), out templongBinID);
                            ObjItemMasterDTO.BinID = templongBinID;
                        }
                        if (dtCounts.Columns.Contains("StagingBinId"))
                        {
                            long templongStagingBinId;
                            long.TryParse(Convert.ToString(dr["StagingBinId"]), out templongStagingBinId);
                            ObjItemMasterDTO.BinID = templongStagingBinId;
                        }
                        if (dtCounts.Columns.Contains("CustomerOwnedQuantity"))
                        {
                            if (double.TryParse(Convert.ToString(dr["CustomerOwnedQuantity"]), out tempdouble))
                            {
                                ObjItemMasterDTO.CustomerOwnedQuantity = tempdouble;
                            }
                            else
                            {
                                ObjItemMasterDTO.CustomerOwnedQuantity = 0;
                            }

                        }
                        if (dtCounts.Columns.Contains("ConsignedQuantity"))
                        {
                            double tempdoubleConsignedQuantity;
                            double.TryParse(Convert.ToString(dr["ConsignedQuantity"]), out tempdoubleConsignedQuantity);
                            ObjItemMasterDTO.ConsignedQuantity = tempdoubleConsignedQuantity;
                        }
                        if (dtCounts.Columns.Contains("IsArchived"))
                        {
                            bool IsArchivedVal = false;
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out IsArchivedVal);
                            ObjItemMasterDTO.IsArchived = IsArchivedVal;
                        }
                        if (dtCounts.Columns.Contains("Consignment"))
                        {
                            bool ConsignmentVal = false;
                            bool.TryParse(Convert.ToString(dr["Consignment"]), out ConsignmentVal);
                            ObjItemMasterDTO.Consignment = ConsignmentVal;
                        }

                        if (dtCounts.Columns.Contains("IsDeleted"))
                        {
                            bool IsDeletedVal = false;
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out IsDeletedVal);
                            ObjItemMasterDTO.IsDeleted = IsDeletedVal;
                        }
                        if (dtCounts.Columns.Contains("LastUpdatedBy"))
                        {
                            long templongLastUpdatedBy;
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templongLastUpdatedBy);
                            ObjItemMasterDTO.LastUpdatedBy = templongLastUpdatedBy;
                        }
                        if (dtCounts.Columns.Contains("UDF1"))
                        {
                            ObjItemMasterDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCounts.Columns.Contains("UDF2"))
                        {
                            ObjItemMasterDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCounts.Columns.Contains("UDF3"))
                        {
                            ObjItemMasterDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCounts.Columns.Contains("UDF4"))
                        {
                            ObjItemMasterDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCounts.Columns.Contains("UDF5"))
                        {
                            ObjItemMasterDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }

                        if (dtCounts.Columns.Contains("itemUDF1"))
                        {
                            ObjItemMasterDTO.ItemUDF1 = Convert.ToString(dr["itemUDF1"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF2"))
                        {
                            ObjItemMasterDTO.ItemUDF2 = Convert.ToString(dr["itemUDF2"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF3"))
                        {
                            ObjItemMasterDTO.ItemUDF3 = Convert.ToString(dr["itemUDF3"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF4"))
                        {
                            ObjItemMasterDTO.ItemUDF4 = Convert.ToString(dr["itemUDF4"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF5"))
                        {
                            ObjItemMasterDTO.ItemUDF5 = Convert.ToString(dr["itemUDF5"]);
                        }
                        if (dtCounts.Columns.Contains("Updated"))
                        {
                            ObjItemMasterDTO.Updated = Convert.ToDateTime(dr["Updated"]);
                        }
                        if (dtCounts.Columns.Contains("UpdatedByName"))
                        {
                            ObjItemMasterDTO.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }
                        if (dtCounts.Columns.Contains("SerialNumberTracking"))
                        {
                            bool SerialNumberTrackingVal = false;
                            bool.TryParse(Convert.ToString(dr["SerialNumberTracking"]), out SerialNumberTrackingVal);
                            ObjItemMasterDTO.SerialNumberTracking = SerialNumberTrackingVal;
                        }
                        if (dtCounts.Columns.Contains("LotNumberTracking"))
                        {
                            bool LotNumberTrackingVal = false;
                            bool.TryParse(Convert.ToString(dr["LotNumberTracking"]), out LotNumberTrackingVal);
                            ObjItemMasterDTO.LotNumberTracking = LotNumberTrackingVal;
                        }
                        if (dtCounts.Columns.Contains("DateCodeTracking"))
                        {
                            bool DateCodeTrackingVal = false;
                            bool.TryParse(Convert.ToString(dr["DateCodeTracking"]), out DateCodeTrackingVal);
                            ObjItemMasterDTO.DateCodeTracking = DateCodeTrackingVal;
                        }
                        if (dtCounts.Columns.Contains("ItemDescription"))
                        {
                            ObjItemMasterDTO.Description = Convert.ToString(dr["ItemDescription"]);
                        }
                        if (dtCounts.Columns.Contains("ItemType"))
                        {
                            Int32 ItemTypeID;
                            Int32.TryParse(Convert.ToString(dr["ItemType"]), out ItemTypeID);
                            ObjItemMasterDTO.ItemType = ItemTypeID;
                            //ObjItemMasterDTO.ItemType = Convert.ToInt32(dr["ItemType"]);
                        }
                        if (dtCounts.Columns.Contains("ManufacturerID"))
                        {
                            long templongManufacturerID;
                            long.TryParse(Convert.ToString(dr["ManufacturerID"]), out templongManufacturerID);
                            ObjItemMasterDTO.ManufacturerID = templongManufacturerID;
                        }
                        if (dtCounts.Columns.Contains("ManufacturerNumber"))
                        {
                            ObjItemMasterDTO.ManufacturerNumber = Convert.ToString(dr["ManufacturerNumber"]);
                        }
                        if (dtCounts.Columns.Contains("ManufacturerName"))
                        {
                            ObjItemMasterDTO.ManufacturerName = Convert.ToString(dr["ManufacturerName"]);
                        }
                        if (dtCounts.Columns.Contains("SupplierID"))
                        {
                            long templongSupplierID;
                            long.TryParse(Convert.ToString(dr["SupplierID"]), out templongSupplierID);
                            ObjItemMasterDTO.SupplierID = templongSupplierID;
                        }
                        if (dtCounts.Columns.Contains("SupplierPartNo"))
                        {
                            ObjItemMasterDTO.SupplierPartNo = Convert.ToString(dr["SupplierPartNo"]);
                        }
                        if (dtCounts.Columns.Contains("SupplierName"))
                        {
                            ObjItemMasterDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCounts.Columns.Contains("CategoryID"))
                        {
                            long templongCategoryID;
                            long.TryParse(Convert.ToString(dr["CategoryID"]), out templongCategoryID);
                            ObjItemMasterDTO.CategoryID = templongCategoryID;
                        }
                        if (dtCounts.Columns.Contains("Category"))
                        {
                            ObjItemMasterDTO.CategoryName = Convert.ToString(dr["Category"]);
                        }
                        if (dtCounts.Columns.Contains("ParentBinName"))
                        {
                            ObjItemMasterDTO.ParentBinName = Convert.ToString(dr["ParentBinName"]);
                        }
                        if (dtCounts.Columns.Contains("ParentBinId"))
                        {
                            long templongParentBinId;
                            long.TryParse(Convert.ToString(dr["ParentBinId"]), out templongParentBinId);
                            ObjItemMasterDTO.ParentBinId = templongParentBinId;
                        }
                        if (dtCounts.Columns.Contains("Cost"))
                        {
                            long templongCost;
                            long.TryParse(Convert.ToString(dr["Cost"]), out templongCost);
                            ObjItemMasterDTO.Cost = templongCost;
                        }
                        if (dtCounts.Columns.Contains("AverageUsage"))
                        {
                            long templongAverageUsage = 0;
                            long.TryParse(Convert.ToString(dr["AverageUsage"]), out templongAverageUsage);
                            ObjItemMasterDTO.AverageUsage = templongAverageUsage;
                        }
                        if (dtCounts.Columns.Contains("Turns"))
                        {
                            long templongTurns;
                            long.TryParse(Convert.ToString(dr["Turns"]), out templongTurns);
                            ObjItemMasterDTO.Turns = templongTurns;
                        }
                        if (dtCounts.Columns.Contains("OnHandQuantity"))
                        {
                            long templongOnHandQuantity;
                            long.TryParse(Convert.ToString(dr["OnHandQuantity"]), out templongOnHandQuantity);
                            ObjItemMasterDTO.OnHandQuantity = templongOnHandQuantity;
                        }
                        if (dtCounts.Columns.Contains("CriticalQuantity"))
                        {
                            long templongCriticalQuantity;
                            long.TryParse(Convert.ToString(dr["CriticalQuantity"]), out templongCriticalQuantity);
                            ObjItemMasterDTO.CriticalQuantity = templongCriticalQuantity;
                        }
                        if (dtCounts.Columns.Contains("MinimumQuantity"))
                        {
                            long templongMinimumQuantity;
                            long.TryParse(Convert.ToString(dr["MinimumQuantity"]), out templongMinimumQuantity);
                            ObjItemMasterDTO.MinimumQuantity = templongMinimumQuantity;
                        }
                        if (dtCounts.Columns.Contains("MaximumQuantity"))
                        {
                            long templongMaximumQuantity;
                            long.TryParse(Convert.ToString(dr["MaximumQuantity"]), out templongMaximumQuantity);
                            ObjItemMasterDTO.MaximumQuantity = templongMaximumQuantity;
                        }
                        if (dtCounts.Columns.Contains("DefaultLocation"))
                        {
                            long.TryParse(Convert.ToString(dr["DefaultLocation"]), out templong);
                            ObjItemMasterDTO.DefaultLocation = templong;
                        }
                        if (dtCounts.Columns.Contains("InventoryClassification"))
                        {
                            Int32 tempintInventoryClassification = 0;
                            Int32.TryParse(Convert.ToString(dr["InventoryClassification"]), out tempintInventoryClassification);
                            ObjItemMasterDTO.InventoryClassification = tempintInventoryClassification;
                        }
                        if (dtCounts.Columns.Contains("AverageCost"))
                        {
                            long.TryParse(Convert.ToString(dr["AverageCost"]), out templong);
                            ObjItemMasterDTO.AverageCost = templong;
                        }
                        if (dtCounts.Columns.Contains("itemUDF6"))
                        {
                            ObjItemMasterDTO.ItemUDF6 = Convert.ToString(dr["itemUDF6"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF7"))
                        {
                            ObjItemMasterDTO.ItemUDF7 = Convert.ToString(dr["itemUDF7"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF8"))
                        {
                            ObjItemMasterDTO.ItemUDF8 = Convert.ToString(dr["itemUDF8"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF9"))
                        {
                            ObjItemMasterDTO.ItemUDF9 = Convert.ToString(dr["itemUDF9"]);
                        }
                        if (dtCounts.Columns.Contains("itemUDF10"))
                        {
                            ObjItemMasterDTO.ItemUDF10 = Convert.ToString(dr["itemUDF10"]);
                        }
                        ItemLocations.Add(ObjItemMasterDTO);
                    }
                }
            }
            return ItemLocations;
        }

        public InventoryCountDetailDTO SaveInventoryCountLineItem(InventoryCountDetailDTO objInventoryCountDetailDTO, bool SaveCountLineItemDetail = true, List<CountLineItemDetailDTO> lstCountLineItemDetail = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail objInventoryCountDetail = null;
                double ItemLocationConsignedQty = 0;
                double ItemLocationCoownedQty = 0;
                ItemMasterDTO objItem = new ItemMasterDTO();
                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                objItem = objItemDAL.GetItemWithoutJoins(null, objInventoryCountDetailDTO.ItemGUID);

                if (objInventoryCountDetailDTO.IsStagingLocationCount)
                {
                    var q = context.MaterialStagingPullDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetailDTO.ItemGUID && t.StagingBinId == objInventoryCountDetailDTO.BinID);
                    if (q.Any())
                    {
                        ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                        ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    }
                }
                else
                {
                    var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetailDTO.ItemGUID && t.BinID == objInventoryCountDetailDTO.BinID);
                    if (q.Any())
                    {
                        ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                        ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    }
                }

                if (objInventoryCountDetailDTO.ID > 0)
                {
                    objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == objInventoryCountDetailDTO.ID);
                    if (objInventoryCountDetail != null)
                    {
                        objInventoryCountDetail.CountConsignedQuantity = objInventoryCountDetailDTO.CountConsignedQuantity ?? (-0.000000001);
                        objInventoryCountDetail.CountCustOwnedQuantity = objInventoryCountDetailDTO.CountConsignedQuantity ?? (-0.000000001);
                        objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                        objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                        objInventoryCountDetail.CountLineItemDescription = objInventoryCountDetailDTO.CountLineItemDescription;
                        objInventoryCountDetail.ProjectSpendGUID = objInventoryCountDetailDTO.ProjectSpendGUID;

                        objInventoryCountDetail.SupplierAccountGuid = objInventoryCountDetailDTO.SupplierAccountGuid;

                        //----------------------------------------------------------------------------------
                        //
                        double CusOwnedDifference = 0;
                        double ConsignedDifference = 0;
                        if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0
                            && GetCustomerConsignDiff(lstCountLineItemDetail, (objItem == null ? false : objItem.Consignment), (objItem == null ? false : objItem.SerialNumberTracking), context, out CusOwnedDifference, out ConsignedDifference))
                        {
                            objInventoryCountDetail.CusOwnedDifference = CusOwnedDifference;
                            if (objItem != null && objItem.Consignment)
                            {
                                objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                            }
                            else
                            {
                                objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                            }
                        }
                        else
                        {
                            if (objItem != null && objItem.Consignment)
                                objInventoryCountDetail.CusOwnedDifference = (objInventoryCountDetailDTO.CountCustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty;
                            else
                                objInventoryCountDetail.CusOwnedDifference = 0;
                            objInventoryCountDetail.ConsignedDifference = (objInventoryCountDetailDTO.CountConsignedQuantity ?? 0) - ItemLocationConsignedQty;
                        }

                        //----------------------------------------------------------------------------------
                        //
                        if (objInventoryCountDetailDTO.IsOnlyFromItemUI)
                        {
                            objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objInventoryCountDetail.EditedFrom = objInventoryCountDetailDTO.EditedFrom ?? "web";
                        }

                        context.SaveChanges();
                        objInventoryCountDetailDTO.CountDetailGUID = objInventoryCountDetail.GUID;
                    }
                }
                else
                {
                    objInventoryCountDetail = new InventoryCountDetail();
                    objInventoryCountDetail.AppliedDate = objInventoryCountDetailDTO.AppliedDate;
                    objInventoryCountDetail.BinGUID = objInventoryCountDetailDTO.BinGUID;
                    objInventoryCountDetail.BinID = objInventoryCountDetailDTO.BinID;
                    objInventoryCountDetail.CountConsignedQuantity = objInventoryCountDetailDTO.CountConsignedQuantity ?? (-0.000000001);
                    objInventoryCountDetail.CountCustOwnedQuantity = objInventoryCountDetailDTO.CountCustomerOwnedQuantity ?? (-0.000000001);
                    objInventoryCountDetail.CountItemStatus = objInventoryCountDetailDTO.CountItemStatus;
                    objInventoryCountDetail.CountLineItemDescription = objInventoryCountDetailDTO.CountLineItemDescription;
                    objInventoryCountDetail.CountQuantity = objInventoryCountDetailDTO.CountQuantity;
                    objInventoryCountDetail.Created = objInventoryCountDetailDTO.Created;
                    objInventoryCountDetail.CreatedBy = objInventoryCountDetailDTO.CreatedBy;
                    objInventoryCountDetail.GUID = objInventoryCountDetailDTO.GUID;
                    objInventoryCountDetail.ID = objInventoryCountDetailDTO.ID;
                    objInventoryCountDetail.InventoryCountGUID = objInventoryCountDetailDTO.InventoryCountGUID;
                    objInventoryCountDetail.IsApplied = objInventoryCountDetailDTO.IsApplied;
                    objInventoryCountDetail.IsArchived = objInventoryCountDetailDTO.IsArchived;
                    objInventoryCountDetail.IsDeleted = objInventoryCountDetailDTO.IsDeleted;
                    objInventoryCountDetail.IsStagingLocationCount = objInventoryCountDetailDTO.IsStagingLocationCount;
                    objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                    objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                    objInventoryCountDetail.ItemGUID = objInventoryCountDetailDTO.ItemGUID;
                    objInventoryCountDetail.ItemQuantity = objInventoryCountDetailDTO.ItemQuantity ?? 0;
                    objInventoryCountDetail.LastUpdatedBy = objInventoryCountDetailDTO.LastUpdatedBy;
                    objInventoryCountDetail.UDF1 = objInventoryCountDetailDTO.UDF1;
                    objInventoryCountDetail.UDF2 = objInventoryCountDetailDTO.UDF2;
                    objInventoryCountDetail.UDF3 = objInventoryCountDetailDTO.UDF3;
                    objInventoryCountDetail.UDF4 = objInventoryCountDetailDTO.UDF4;
                    objInventoryCountDetail.UDF5 = objInventoryCountDetailDTO.UDF5;
                    objInventoryCountDetail.PullOrderNumber = objInventoryCountDetailDTO.PullOrderNumber;
                    objInventoryCountDetail.Updated = objInventoryCountDetailDTO.Updated;
                    objInventoryCountDetail.RoomId = objInventoryCountDetailDTO.RoomId;
                    objInventoryCountDetail.CompanyId = objInventoryCountDetailDTO.CompanyId;
                    objInventoryCountDetail.ProjectSpendGUID = objInventoryCountDetailDTO.ProjectSpendGUID;
                    objInventoryCountDetailDTO.CountDetailGUID = objInventoryCountDetailDTO.GUID;
                    objInventoryCountDetail.SupplierAccountGuid = objInventoryCountDetailDTO.SupplierAccountGuid;
                    //----------------------------------------------------------------------------------
                    //
                    double CusOwnedDifference = 0;
                    double ConsignedDifference = 0;
                    if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0
                        && GetCustomerConsignDiff(lstCountLineItemDetail, (objItem == null ? false : objItem.Consignment), (objItem == null ? false : objItem.SerialNumberTracking), context, out CusOwnedDifference, out ConsignedDifference))
                    {
                        objInventoryCountDetail.CusOwnedDifference = CusOwnedDifference;
                        if (objItem != null && objItem.Consignment)
                        {
                            objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                        }
                        else
                        {
                            objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                        }
                    }
                    else
                    {
                        objInventoryCountDetail.CusOwnedDifference = objInventoryCountDetailDTO.CountCustomerOwnedQuantity.HasValue ? ((objInventoryCountDetailDTO.CountCustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty) : (-0.000000001);
                        if (objItem != null && objItem.Consignment)
                        {

                            objInventoryCountDetail.ConsignedDifference = objInventoryCountDetailDTO.CountConsignedQuantity.HasValue ? ((objInventoryCountDetailDTO.CountConsignedQuantity ?? 0) - ItemLocationConsignedQty) : (-0.000000001);
                        }
                        else
                        {
                            objInventoryCountDetail.ConsignedDifference = (-0.000000001);
                        }
                    }

                    //----------------------------------------------------------------------------------
                    //
                    objInventoryCountDetail.AddedFrom = objInventoryCountDetailDTO.AddedFrom ?? "web";
                    objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objInventoryCountDetail.EditedFrom = objInventoryCountDetailDTO.EditedFrom ?? "web";
                    objInventoryCountDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    context.InventoryCountDetails.Add(objInventoryCountDetail);
                    context.SaveChanges();
                    objInventoryCountDetailDTO.ID = objInventoryCountDetail.ID;
                    objInventoryCountDetailDTO.CountDetailGUID = objInventoryCountDetail.GUID;
                }

                //---------------------------------------------------------------------------------------------
                //
                if (SaveCountLineItemDetail == true)
                {
                    CountLineItemDetail objCountLineItemDetail = (from CLID in context.CountLineItemDetails
                                                                  where CLID.CountDetailGUID == objInventoryCountDetail.GUID
                                                                  select CLID).FirstOrDefault();
                    if (objCountLineItemDetail != null)
                    {
                        objCountLineItemDetail.ItemGUID = objInventoryCountDetail.ItemGUID;
                        objCountLineItemDetail.BinID = objInventoryCountDetail.BinID;
                        //objCountLineItemDetail.CustomerOwnedQuantity = objInventoryCountDetail.ItemGUID;
                        objCountLineItemDetail.CountCustomerOwnedQuantity = objInventoryCountDetail.CountCustOwnedQuantity;
                        //objCountLineItemDetail.ConsignedQuantity = objInventoryCountDetail.ItemGUID;
                        objCountLineItemDetail.CountConsignedQuantity = objInventoryCountDetail.CountConsignedQuantity;
                        objCountLineItemDetail.Updated = objInventoryCountDetail.Updated;
                        objCountLineItemDetail.LastUpdatedBy = objInventoryCountDetail.LastUpdatedBy;
                        objCountLineItemDetail.IsDeleted = objInventoryCountDetail.IsDeleted;
                        objCountLineItemDetail.IsArchived = objInventoryCountDetail.IsArchived;
                        objCountLineItemDetail.CompanyID = objInventoryCountDetail.CompanyId;
                        objCountLineItemDetail.RoomID = objInventoryCountDetail.RoomId;
                        objCountLineItemDetail.CountGUID = objInventoryCountDetail.InventoryCountGUID;
                        objCountLineItemDetail.CountDetailGUID = objInventoryCountDetail.GUID;
                        objCountLineItemDetail.EditedFrom = objInventoryCountDetail.EditedFrom;
                        objCountLineItemDetail.CusOwnedDifference = objInventoryCountDetail.CusOwnedDifference;
                        objCountLineItemDetail.ConsignedDifference = objInventoryCountDetail.ConsignedDifference;
                        context.CountLineItemDetails.Add(objCountLineItemDetail);
                        context.SaveChanges();
                    }
                    else
                    {
                        objCountLineItemDetail = new CountLineItemDetail();
                        objCountLineItemDetail.GUID = Guid.NewGuid();
                        objCountLineItemDetail.ItemGUID = objInventoryCountDetail.ItemGUID;
                        objCountLineItemDetail.BinID = objInventoryCountDetail.BinID;
                        //objCountLineItemDetail.CustomerOwnedQuantity = objInventoryCountDetail.ItemGUID;
                        objCountLineItemDetail.CountCustomerOwnedQuantity = objInventoryCountDetail.CountCustOwnedQuantity;
                        //objCountLineItemDetail.ConsignedQuantity = objInventoryCountDetail.ItemGUID;
                        objCountLineItemDetail.CountConsignedQuantity = objInventoryCountDetail.CountConsignedQuantity;
                        objCountLineItemDetail.LotNumber = null;
                        objCountLineItemDetail.SerialNumber = null;
                        objCountLineItemDetail.Comment = null;
                        objCountLineItemDetail.Expiration = null;
                        objCountLineItemDetail.ExpirationDate = null;
                        objCountLineItemDetail.Received = null;
                        objCountLineItemDetail.ReceivedDate = DateTime.UtcNow;
                        objCountLineItemDetail.Cost = (from I in context.ItemMasters where I.GUID == objInventoryCountDetail.ItemGUID select I.Cost).FirstOrDefault();
                        objCountLineItemDetail.Created = objInventoryCountDetail.Created;
                        objCountLineItemDetail.Updated = objInventoryCountDetail.Updated;
                        objCountLineItemDetail.CreatedBy = objInventoryCountDetail.CreatedBy;
                        objCountLineItemDetail.LastUpdatedBy = objInventoryCountDetail.LastUpdatedBy;
                        objCountLineItemDetail.IsDeleted = objInventoryCountDetail.IsDeleted;
                        objCountLineItemDetail.IsArchived = objInventoryCountDetail.IsArchived;
                        objCountLineItemDetail.CompanyID = objInventoryCountDetail.CompanyId;
                        objCountLineItemDetail.RoomID = objInventoryCountDetail.RoomId;
                        objCountLineItemDetail.CountGUID = objInventoryCountDetail.InventoryCountGUID;
                        objCountLineItemDetail.CountDetailGUID = objInventoryCountDetail.GUID;
                        objCountLineItemDetail.ReceivedOn = objInventoryCountDetail.ReceivedOn;
                        objCountLineItemDetail.ReceivedOnWeb = objInventoryCountDetail.ReceivedOnWeb;
                        objCountLineItemDetail.AddedFrom = objInventoryCountDetail.AddedFrom;
                        objCountLineItemDetail.EditedFrom = objInventoryCountDetail.EditedFrom;

                        objCountLineItemDetail.CusOwnedDifference = objInventoryCountDetail.CusOwnedDifference;
                        objCountLineItemDetail.ConsignedDifference = objInventoryCountDetail.ConsignedDifference;

                        context.CountLineItemDetails.Add(objCountLineItemDetail);
                        context.SaveChanges();
                    }
                }

                return objInventoryCountDetailDTO;
            }
        }

        public bool GetCustomerConsignDiff(List<CountLineItemDetailDTO> lstCountLineItemDetail, bool Consignment, bool IsSerialTracking, eTurnsEntities context, out double CusOwnedDifference, out double ConsignedDifference)
        {
            CusOwnedDifference = 0;
            ConsignedDifference = 0;

            try
            {
                //--------------------------------------------------------------
                //
                var CountLineItemDetailNew = (from A in lstCountLineItemDetail
                                              group A by new { A.ItemGUID, A.BinID, A.LotNumber, A.SerialNumber, A.ExpirationDate } into G
                                              select new
                                              {
                                                  ItemGUID = G.Key.ItemGUID.Value,
                                                  BinID = G.Key.BinID.Value,
                                                  LotSerial = (((G.Key.LotNumber ?? string.Empty).Trim() != "") ? G.Key.LotNumber.Trim() : (G.Key.SerialNumber ?? string.Empty).Trim()),
                                                  ExpirationDate = (IsSerialTracking != true && G.Key.ExpirationDate != null ? (DateTime?)G.Key.ExpirationDate.Value.Date : null),
                                                  CountCustomerOwnedQuantity = (G.Sum(x => x.CountCustomerOwnedQuantity) == null || G.Sum(x => x.CountCustomerOwnedQuantity) < 0 ? 0 : G.Sum(x => x.CountCustomerOwnedQuantity)),
                                                  CountConsignedQuantity = (G.Sum(x => x.CountConsignedQuantity) == null || G.Sum(x => x.CountConsignedQuantity) < 0 ? 0 : G.Sum(x => x.CountConsignedQuantity))
                                              }
                                              ).ToList();

                //--------------------------------------------------------------
                //
                Guid ItemGuid = lstCountLineItemDetail[0].ItemGUID.Value;
                long BinID = lstCountLineItemDetail[0].BinID.Value;
                bool IsStage = lstCountLineItemDetail[0].IsStagingLocationCount.GetValueOrDefault(false);

                if (IsStage)
                {

                    var lstILD = (from IM in context.ItemMasters
                                  join BM in context.BinMasters on new { ItemGUID = IM.GUID, IsDeleted = false, IsArchived = false } equals new { ItemGUID = BM.ItemGUID.Value, IsDeleted = BM.IsDeleted, IsArchived = (BM.IsArchived ?? false) }
                                  join ILD in context.MaterialStagingPullDetails on new { ItemGUID = IM.GUID, BinID = BM.ID, IsDeleted = false, IsArchived = false } equals new { ItemGUID = ILD.ItemGUID, BinID = ILD.StagingBinId, IsDeleted = (ILD.IsDeleted ?? false), IsArchived = (ILD.IsArchived ?? false) }
                                  where IM.GUID == ItemGuid && BM.ID == BinID
                                  select new
                                  {
                                      ItemGUID = IM.GUID,
                                      BinID = BM.ID,
                                      LotSerial = (IM.LotNumberTracking ? ILD.LotNumber : (IM.SerialNumberTracking ? ILD.SerialNumber : "")),
                                      ExpirationDate = (IsSerialTracking != true && IM.DateCodeTracking && ILD.ExpirationDate != null ? (DateTime?)ILD.ExpirationDate.Value : null),
                                      //CustomerOwnedQty = (ILD.CustomerOwnedQuantity == null || ILD.CustomerOwnedQuantity < 0 ? 0 : ILD.CustomerOwnedQuantity),
                                      //ConsignedQty = (ILD.ConsignedQuantity == null || ILD.ConsignedQuantity < 0 ? 0 : ILD.ConsignedQuantity)
                                      CustomerOwnedQty = (ILD.CustomerOwnedQuantity == null ? 0 : ILD.CustomerOwnedQuantity),
                                      ConsignedQty = (ILD.ConsignedQuantity == null ? 0 : ILD.ConsignedQuantity)
                                  }
                     ).GroupBy(x => new { x.ItemGUID, x.BinID, x.LotSerial, x.ExpirationDate }).Select(y => new
                     {
                         ItemGUID = y.Key.ItemGUID,
                         BinID = y.Key.BinID,
                         LotSerial = y.Key.LotSerial,
                         ExpirationDate = y.Key.ExpirationDate,
                         CustomerOwnedQty = y.Sum(z => z.CustomerOwnedQty),
                         ConsignedQty = y.Sum(z => z.ConsignedQty)
                     }).ToList();


                    var varNewOld1 = (from A in lstILD
                                      join B1 in CountLineItemDetailNew on new { ItemGUID = A.ItemGUID, BinID = A.BinID, LotSerial = A.LotSerial, ExpirationDate = A.ExpirationDate }
                                                                                equals new { ItemGUID = B1.ItemGUID, BinID = B1.BinID, LotSerial = B1.LotSerial, ExpirationDate = B1.ExpirationDate }
                                      into B2
                                      from B in B2.DefaultIfEmpty()
                                      select new
                                      {
                                          ItemGUID = A.ItemGUID,
                                          BinID = A.BinID,
                                          CustomerOwnedQty = A.CustomerOwnedQty,
                                          ConsignedQty = A.ConsignedQty,
                                          CustomerOwnedNewQty = (B != null ? B.CountCustomerOwnedQuantity : (IsSerialTracking == true ? 0 : A.CustomerOwnedQty)),
                                          ConsignedNewQty = (B != null ? B.CountConsignedQuantity : (IsSerialTracking == true ? 0 : A.ConsignedQty)),
                                      }
                     ).ToList();

                    var varNewOld2 = (from A in CountLineItemDetailNew
                                      where !lstILD.Any(x => x.ItemGUID == A.ItemGUID
                                                        && x.BinID == A.BinID
                                                        && x.LotSerial == A.LotSerial
                                                        && x.ExpirationDate == A.ExpirationDate
                                                        )
                                      select new
                                      {
                                          ItemGUID = A.ItemGUID,
                                          BinID = A.BinID,
                                          CustomerOwnedQty = 0,
                                          ConsignedQty = 0,
                                          CustomerOwnedNewQty = A.CountCustomerOwnedQuantity,
                                          ConsignedNewQty = A.CountConsignedQuantity,
                                      }
                                      ).ToList();

                    //--------------------------------------------------------------
                    //
                    var varDiff = (from A in varNewOld1
                                   group A by new { A.ItemGUID, A.BinID } into G
                                   select new
                                   {
                                       CustomerOwnedDiffQty = G.Sum(x => x.CustomerOwnedNewQty) - G.Sum(x => x.CustomerOwnedQty),
                                       ConsignedDiffQty = G.Sum(x => x.ConsignedNewQty) - G.Sum(x => x.ConsignedQty)
                                   }
                                  ).FirstOrDefault();

                    var varDiff2 = (from A in varNewOld2
                                    group A by new { A.ItemGUID, A.BinID } into G
                                    select new
                                    {
                                        CustomerOwnedDiffQty = G.Sum(x => x.CustomerOwnedNewQty) - G.Sum(x => x.CustomerOwnedQty),
                                        ConsignedDiffQty = G.Sum(x => x.ConsignedNewQty) - G.Sum(x => x.ConsignedQty)
                                    }
                                  ).FirstOrDefault();

                    //--------------------------------------------------------------
                    //

                    if (varDiff != null)
                    {
                        CusOwnedDifference = varDiff.CustomerOwnedDiffQty.GetValueOrDefault(0);
                    }

                    if (varDiff2 != null)
                    {
                        CusOwnedDifference = CusOwnedDifference + (varDiff2.CustomerOwnedDiffQty.GetValueOrDefault(0));
                    }

                    if (Consignment)
                    {
                        if (varDiff != null)
                        {
                            ConsignedDifference = varDiff.ConsignedDiffQty.GetValueOrDefault(0);
                        }

                        if (varDiff2 != null)
                        {
                            ConsignedDifference = ConsignedDifference + (varDiff2.ConsignedDiffQty.GetValueOrDefault(0));
                        }
                    }
                    else
                    {
                        ConsignedDifference = (-0.000000001);
                    }
                }
                else
                {

                    var lstILD = (from IM in context.ItemMasters
                                  join BM in context.BinMasters on new { ItemGUID = IM.GUID, IsDeleted = false, IsArchived = false } equals new { ItemGUID = BM.ItemGUID.Value, IsDeleted = BM.IsDeleted, IsArchived = (BM.IsArchived ?? false) }
                                  join ILD in context.ItemLocationDetails on new { ItemGUID = IM.GUID, BinID = BM.ID, IsDeleted = false, IsArchived = false } equals new { ItemGUID = ILD.ItemGUID.Value, BinID = ILD.BinID.Value, IsDeleted = (ILD.IsDeleted ?? false), IsArchived = (ILD.IsArchived ?? false) }
                                  where IM.GUID == ItemGuid && BM.ID == BinID
                                  select new
                                  {
                                      ItemGUID = IM.GUID,
                                      BinID = BM.ID,
                                      LotSerial = (IM.LotNumberTracking ? ILD.LotNumber : (IM.SerialNumberTracking ? ILD.SerialNumber : "")),
                                      ExpirationDate = (IsSerialTracking != true && IM.DateCodeTracking && ILD.ExpirationDate != null ? (DateTime?)ILD.ExpirationDate.Value : null),
                                      //CustomerOwnedQty = (ILD.CustomerOwnedQuantity == null || ILD.CustomerOwnedQuantity < 0 ? 0 : ILD.CustomerOwnedQuantity),
                                      //ConsignedQty = (ILD.ConsignedQuantity == null || ILD.ConsignedQuantity < 0 ? 0 : ILD.ConsignedQuantity)
                                      CustomerOwnedQty = (ILD.CustomerOwnedQuantity == null ? 0 : ILD.CustomerOwnedQuantity),
                                      ConsignedQty = (ILD.ConsignedQuantity == null ? 0 : ILD.ConsignedQuantity)
                                  }
                     ).GroupBy(x => new { x.ItemGUID, x.BinID, x.LotSerial, x.ExpirationDate }).Select(y => new
                     {
                         ItemGUID = y.Key.ItemGUID,
                         BinID = y.Key.BinID,
                         LotSerial = y.Key.LotSerial,
                         ExpirationDate = y.Key.ExpirationDate,
                         CustomerOwnedQty = y.Sum(z => z.CustomerOwnedQty),
                         ConsignedQty = y.Sum(z => z.ConsignedQty)
                     }).ToList();


                    var varNewOld1 = (from A in lstILD
                                      join B1 in CountLineItemDetailNew on new { ItemGUID = A.ItemGUID, BinID = A.BinID, LotSerial = A.LotSerial, ExpirationDate = A.ExpirationDate }
                                                                                equals new { ItemGUID = B1.ItemGUID, BinID = B1.BinID, LotSerial = B1.LotSerial, ExpirationDate = B1.ExpirationDate }
                                      into B2
                                      from B in B2.DefaultIfEmpty()
                                      select new
                                      {
                                          ItemGUID = A.ItemGUID,
                                          BinID = A.BinID,
                                          CustomerOwnedQty = A.CustomerOwnedQty,
                                          ConsignedQty = A.ConsignedQty,
                                          CustomerOwnedNewQty = (B != null ? B.CountCustomerOwnedQuantity : (IsSerialTracking == true ? 0 : A.CustomerOwnedQty)),
                                          ConsignedNewQty = (B != null ? B.CountConsignedQuantity : (IsSerialTracking == true ? 0 : A.ConsignedQty)),
                                      }
                     ).ToList();

                    var varNewOld2 = (from A in CountLineItemDetailNew
                                      where !lstILD.Any(x => x.ItemGUID == A.ItemGUID
                                                        && x.BinID == A.BinID
                                                        && x.LotSerial == A.LotSerial
                                                        && x.ExpirationDate == A.ExpirationDate
                                                        )
                                      select new
                                      {
                                          ItemGUID = A.ItemGUID,
                                          BinID = A.BinID,
                                          CustomerOwnedQty = 0,
                                          ConsignedQty = 0,
                                          CustomerOwnedNewQty = A.CountCustomerOwnedQuantity,
                                          ConsignedNewQty = A.CountConsignedQuantity,
                                      }
                                      ).ToList();

                    //--------------------------------------------------------------
                    //
                    var varDiff = (from A in varNewOld1
                                   group A by new { A.ItemGUID, A.BinID } into G
                                   select new
                                   {
                                       CustomerOwnedDiffQty = G.Sum(x => x.CustomerOwnedNewQty) - G.Sum(x => x.CustomerOwnedQty),
                                       ConsignedDiffQty = G.Sum(x => x.ConsignedNewQty) - G.Sum(x => x.ConsignedQty)
                                   }
                                  ).FirstOrDefault();

                    var varDiff2 = (from A in varNewOld2
                                    group A by new { A.ItemGUID, A.BinID } into G
                                    select new
                                    {
                                        CustomerOwnedDiffQty = G.Sum(x => x.CustomerOwnedNewQty) - G.Sum(x => x.CustomerOwnedQty),
                                        ConsignedDiffQty = G.Sum(x => x.ConsignedNewQty) - G.Sum(x => x.ConsignedQty)
                                    }
                                  ).FirstOrDefault();

                    //--------------------------------------------------------------
                    //

                    if (varDiff != null)
                    {
                        CusOwnedDifference = varDiff.CustomerOwnedDiffQty.GetValueOrDefault(0);
                    }

                    if (varDiff2 != null)
                    {
                        CusOwnedDifference = CusOwnedDifference + (varDiff2.CustomerOwnedDiffQty.GetValueOrDefault(0));
                    }

                    if (Consignment)
                    {
                        if (varDiff != null)
                        {
                            ConsignedDifference = varDiff.ConsignedDiffQty.GetValueOrDefault(0);
                        }

                        if (varDiff2 != null)
                        {
                            ConsignedDifference = ConsignedDifference + (varDiff2.ConsignedDiffQty.GetValueOrDefault(0));
                        }
                    }
                    else
                    {
                        ConsignedDifference = (-0.000000001);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetCustomerConsignDiff_CLD(List<CountLineItemDetailDTO> lstCountLineItemDetail, bool Consignment, bool IsSerialTracking, eTurnsEntities context, out double CusOwnedDifference, out double ConsignedDifference)
        {
            CusOwnedDifference = 0;
            ConsignedDifference = 0;

            try
            {
                //--------------------------------------------------------------
                //
                var CountLineItemDetailNew = (from A in lstCountLineItemDetail
                                              group A by new { A.ItemGUID, A.BinID, A.LotNumber, A.SerialNumber, A.ExpirationDate } into G
                                              select new
                                              {
                                                  ItemGUID = G.Key.ItemGUID.Value,
                                                  BinID = G.Key.BinID.Value,
                                                  LotSerial = (((G.Key.LotNumber ?? string.Empty).Trim() != "") ? G.Key.LotNumber.Trim() : (G.Key.SerialNumber ?? string.Empty).Trim()),
                                                  ExpirationDate = (IsSerialTracking != true && G.Key.ExpirationDate != null ? (DateTime?)G.Key.ExpirationDate.Value.Date : null),
                                                  CountCustomerOwnedQuantity = (G.Sum(x => x.CountCustomerOwnedQuantity) == null || G.Sum(x => x.CountCustomerOwnedQuantity) < 0 ? 0 : G.Sum(x => x.CountCustomerOwnedQuantity)),
                                                  CountConsignedQuantity = (G.Sum(x => x.CountConsignedQuantity) == null || G.Sum(x => x.CountConsignedQuantity) < 0 ? 0 : G.Sum(x => x.CountConsignedQuantity))
                                              }
                                              ).ToList();

                //--------------------------------------------------------------
                //
                Guid ItemGuid = lstCountLineItemDetail[0].ItemGUID.Value;
                long BinID = lstCountLineItemDetail[0].BinID.Value;
                string _lotSerial = lstCountLineItemDetail[0].LotSerialNumber ?? string.Empty;
                string _Expiration = lstCountLineItemDetail[0].Expiration ?? string.Empty;
                DateTime _ExpirationDate = lstCountLineItemDetail[0].ExpirationDate.GetValueOrDefault(DateTime.MinValue);
                bool isLotSerialTrack = false;
                if (lstCountLineItemDetail[0].LotNumberTracking.GetValueOrDefault(false) == true || lstCountLineItemDetail[0].SerialNumberTracking.GetValueOrDefault(false) == true)
                {
                    isLotSerialTrack = true;
                }

                var lstILD = (from IM in context.ItemMasters
                              join BM in context.BinMasters on new { ItemGUID = IM.GUID, IsDeleted = false, IsArchived = false } equals new { ItemGUID = BM.ItemGUID.Value, IsDeleted = BM.IsDeleted, IsArchived = (BM.IsArchived ?? false) }
                              join ILD in context.ItemLocationDetails on new { ItemGUID = IM.GUID, BinID = BM.ID, IsDeleted = false, IsArchived = false } equals new { ItemGUID = ILD.ItemGUID.Value, BinID = ILD.BinID.Value, IsDeleted = (ILD.IsDeleted ?? false), IsArchived = (ILD.IsArchived ?? false) }
                              where IM.GUID == ItemGuid && BM.ID == BinID
                              select new
                              {
                                  ItemGUID = IM.GUID,
                                  BinID = BM.ID,
                                  LotSerial = (IM.LotNumberTracking ? ILD.LotNumber : (IM.SerialNumberTracking ? ILD.SerialNumber : "")),
                                  ExpirationDate = (IsSerialTracking != true && IM.DateCodeTracking && ILD.ExpirationDate != null ? (DateTime?)ILD.ExpirationDate.Value : null),
                                  //CustomerOwnedQty = (ILD.CustomerOwnedQuantity == null || ILD.CustomerOwnedQuantity < 0 ? 0 : ILD.CustomerOwnedQuantity),
                                  //ConsignedQty = (ILD.ConsignedQuantity == null || ILD.ConsignedQuantity < 0 ? 0 : ILD.ConsignedQuantity)
                                  CustomerOwnedQty = (ILD.CustomerOwnedQuantity == null ? 0 : ILD.CustomerOwnedQuantity),
                                  ConsignedQty = (ILD.ConsignedQuantity == null ? 0 : ILD.ConsignedQuantity)
                              }
                 ).GroupBy(x => new { x.ItemGUID, x.BinID, x.LotSerial, x.ExpirationDate }).Select(y => new
                 {
                     ItemGUID = y.Key.ItemGUID,
                     BinID = y.Key.BinID,
                     LotSerial = y.Key.LotSerial,
                     ExpirationDate = y.Key.ExpirationDate,
                     CustomerOwnedQty = y.Sum(z => z.CustomerOwnedQty),
                     ConsignedQty = y.Sum(z => z.ConsignedQty)
                 }).ToList();

                if (isLotSerialTrack == true)
                {
                    lstILD = lstILD.Where(x => x.LotSerial == _lotSerial).ToList();
                }

                if (!string.IsNullOrEmpty(_Expiration) && _ExpirationDate != DateTime.MinValue && IsSerialTracking != true)
                {
                    lstILD = lstILD.Where(x => x.ExpirationDate.GetValueOrDefault(DateTime.MinValue).Date == _ExpirationDate.Date).ToList();
                }

                var varNewOld1 = (from A in lstILD
                                  join B1 in CountLineItemDetailNew on new { ItemGUID = A.ItemGUID, BinID = A.BinID, LotSerial = A.LotSerial, ExpirationDate = A.ExpirationDate }
                                                                            equals new { ItemGUID = B1.ItemGUID, BinID = B1.BinID, LotSerial = B1.LotSerial, ExpirationDate = B1.ExpirationDate }
                                  into B2
                                  from B in B2.DefaultIfEmpty()
                                  select new
                                  {
                                      ItemGUID = A.ItemGUID,
                                      BinID = A.BinID,
                                      CustomerOwnedQty = A.CustomerOwnedQty,
                                      ConsignedQty = A.ConsignedQty,
                                      CustomerOwnedNewQty = (B != null ? B.CountCustomerOwnedQuantity : (IsSerialTracking == true ? 0 : A.CustomerOwnedQty)),
                                      ConsignedNewQty = (B != null ? B.CountConsignedQuantity : (IsSerialTracking == true ? 0 : A.ConsignedQty)),
                                  }
                 ).ToList();

                var varNewOld2 = (from A in CountLineItemDetailNew
                                  where !lstILD.Any(x => x.ItemGUID == A.ItemGUID
                                                    && x.BinID == A.BinID
                                                    && x.LotSerial == A.LotSerial
                                                    && x.ExpirationDate == A.ExpirationDate
                                                    )
                                  select new
                                  {
                                      ItemGUID = A.ItemGUID,
                                      BinID = A.BinID,
                                      CustomerOwnedQty = 0,
                                      ConsignedQty = 0,
                                      CustomerOwnedNewQty = A.CountCustomerOwnedQuantity,
                                      ConsignedNewQty = A.CountConsignedQuantity,
                                  }
                                  ).ToList();

                //--------------------------------------------------------------
                //
                var varDiff = (from A in varNewOld1
                               group A by new { A.ItemGUID, A.BinID } into G
                               select new
                               {
                                   CustomerOwnedDiffQty = G.Sum(x => x.CustomerOwnedNewQty) - G.Sum(x => x.CustomerOwnedQty),
                                   ConsignedDiffQty = G.Sum(x => x.ConsignedNewQty) - G.Sum(x => x.ConsignedQty)
                               }
                              ).FirstOrDefault();

                var varDiff2 = (from A in varNewOld2
                                group A by new { A.ItemGUID, A.BinID } into G
                                select new
                                {
                                    CustomerOwnedDiffQty = G.Sum(x => x.CustomerOwnedNewQty) - G.Sum(x => x.CustomerOwnedQty),
                                    ConsignedDiffQty = G.Sum(x => x.ConsignedNewQty) - G.Sum(x => x.ConsignedQty)
                                }
                              ).FirstOrDefault();

                //--------------------------------------------------------------
                //
                if (varDiff != null)
                {
                    CusOwnedDifference = (varDiff != null && varDiff.CustomerOwnedDiffQty == null ? 0 : varDiff.CustomerOwnedDiffQty.Value);
                }

                if (varDiff2 != null)
                {
                    CusOwnedDifference = CusOwnedDifference + (varDiff2.CustomerOwnedDiffQty == null ? 0 : varDiff2.CustomerOwnedDiffQty.Value);
                }

                if (Consignment)
                {
                    if (varDiff != null)
                    {
                        ConsignedDifference = (varDiff != null && varDiff.ConsignedDiffQty == null ? 0 : varDiff.ConsignedDiffQty.Value);
                    }

                    if (varDiff2 != null)
                    {
                        ConsignedDifference = ConsignedDifference + (varDiff2.ConsignedDiffQty == null ? 0 : varDiff2.ConsignedDiffQty.Value);
                    }
                }
                else
                {
                    ConsignedDifference = (-0.000000001);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<InventoryCountDetailDTO> SaveCountLineItems(List<InventoryCountDetailDTO> lstCountLineItems, long UserId)
        {
            List<InventoryCountDetailDTO> lstCountLineItemsReturn = new List<InventoryCountDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (InventoryCountDetailDTO Editeditem in lstCountLineItems)
                {
                    InventoryCountDetail objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == Editeditem.ID);

                    if (Editeditem.isLineItemAlreadyApplied.GetValueOrDefault(false))
                    {
                        if (objInventoryCountDetail != null)
                        {
                            objInventoryCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                            objInventoryCountDetail.UDF1 = HttpUtility.UrlDecode(Editeditem.UDF1);
                            objInventoryCountDetail.UDF2 = HttpUtility.UrlDecode(Editeditem.UDF2);
                            objInventoryCountDetail.UDF3 = HttpUtility.UrlDecode(Editeditem.UDF3);
                            objInventoryCountDetail.UDF4 = HttpUtility.UrlDecode(Editeditem.UDF4);
                            objInventoryCountDetail.UDF5 = HttpUtility.UrlDecode(Editeditem.UDF5);
                            objInventoryCountDetail.PullOrderNumber = Editeditem.PullOrderNumber;
                            objInventoryCountDetail.LastUpdatedBy = UserId;
                            objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                            if (Editeditem.IsOnlyFromItemUI)
                            {
                                objInventoryCountDetail.EditedFrom = "Web";
                                objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                        }
                    }
                    else
                    {
                        if (objInventoryCountDetail != null)
                        {
                            ItemMasterDTO objItem = new ItemMasterDTO();
                            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                            objItem = objItemDAL.GetItemWithoutJoins(null, objInventoryCountDetail.ItemGUID);
                            double ItemLocationConsignedQty = 0;
                            double ItemLocationCoownedQty = 0;

                            if (Editeditem.IsStagingLocationCount)
                            {
                                var q = context.MaterialStagingPullDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetail.ItemGUID && t.StagingBinId == objInventoryCountDetail.BinID);

                                if (q.Any())
                                {
                                    ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                                    ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                                }
                            }
                            else
                            {
                                var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetail.ItemGUID && t.BinID == objInventoryCountDetail.BinID);
                                if (q.Any())
                                {
                                    ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                                    ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                                }
                            }

                            objInventoryCountDetail.CountConsignedQuantity = Editeditem.CountConsignedQuantity ?? (-0.000000001);
                            objInventoryCountDetail.CountCustOwnedQuantity = Editeditem.CountCustomerOwnedQuantity ?? (-0.000000001);
                            objInventoryCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                            objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                            objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                            objInventoryCountDetail.UDF1 = HttpUtility.UrlDecode(Editeditem.UDF1);
                            objInventoryCountDetail.UDF2 = HttpUtility.UrlDecode(Editeditem.UDF2);
                            objInventoryCountDetail.UDF3 = HttpUtility.UrlDecode(Editeditem.UDF3);
                            objInventoryCountDetail.UDF4 = HttpUtility.UrlDecode(Editeditem.UDF4);
                            objInventoryCountDetail.UDF5 = HttpUtility.UrlDecode(Editeditem.UDF5);
                            objInventoryCountDetail.PullOrderNumber = Editeditem.PullOrderNumber;
                            objInventoryCountDetail.LastUpdatedBy = UserId;
                            objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                            objInventoryCountDetail.ProjectSpendGUID = Editeditem.ProjectSpendGUID;
                            objInventoryCountDetail.SupplierAccountGuid = Editeditem.SupplierAccountGuid;
                            if (Editeditem.IsOnlyFromItemUI)
                            {
                                objInventoryCountDetail.EditedFrom = "Web";
                                objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }

                            objInventoryCountDetail.CusOwnedDifference = Editeditem.CountCustomerOwnedQuantity.HasValue ? ((Editeditem.CountCustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty) : (-0.000000001);
                            if (objItem != null && objItem.Consignment)
                            {
                                objInventoryCountDetail.ConsignedDifference = Editeditem.CountConsignedQuantity.HasValue ? ((Editeditem.CountConsignedQuantity ?? 0) - ItemLocationConsignedQty) : (-0.000000001);
                            }
                            else
                            {
                                objInventoryCountDetail.ConsignedDifference = (-0.000000001);
                            }

                        }

                        if (Editeditem.LotNumberTracking != true && Editeditem.SerialNumberTracking != true && objInventoryCountDetail.GUID != null && objInventoryCountDetail.GUID != Guid.Empty)
                        {
                            UpdateCountLineItemDetailQty(objInventoryCountDetail.GUID, objInventoryCountDetail.CountCustOwnedQuantity, objInventoryCountDetail.CountConsignedQuantity, objInventoryCountDetail.CusOwnedDifference, objInventoryCountDetail.ConsignedDifference);
                        }
                    }
                    lstCountLineItemsReturn.Add(Editeditem);
                }
                context.SaveChanges();
            }
            return lstCountLineItemsReturn;
        }

        public bool ApplyCountOnItemLocations(List<InventoryCountDetailDTO> lstCountLineItems, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (InventoryCountDetailDTO Editeditem in lstCountLineItems)
                {
                    InventoryCountDetail objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == Editeditem.ID);

                    if (objInventoryCountDetail != null)
                    {
                        double ItemLocationConsignedQty = 0;
                        double ItemLocationCoownedQty = 0;
                        var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetail.ItemGUID && t.BinID == objInventoryCountDetail.BinID);

                        if (q.Any())
                        {
                            ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                            ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                        }

                        if (Editeditem.SaveAndApply)
                        {
                            objInventoryCountDetail.CountConsignedQuantity = Editeditem.CountConsignedQuantity ?? (-0.000000001);
                            objInventoryCountDetail.CountCustOwnedQuantity = Editeditem.CountCustomerOwnedQuantity ?? (-0.000000001);
                            objInventoryCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                            objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                            objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                            objInventoryCountDetail.LastUpdatedBy = UserId;
                            objInventoryCountDetail.UDF1 = HttpUtility.UrlDecode(Editeditem.UDF1);
                            objInventoryCountDetail.UDF2 = HttpUtility.UrlDecode(Editeditem.UDF2);
                            objInventoryCountDetail.UDF3 = HttpUtility.UrlDecode(Editeditem.UDF3);
                            objInventoryCountDetail.UDF4 = HttpUtility.UrlDecode(Editeditem.UDF4);
                            objInventoryCountDetail.UDF5 = HttpUtility.UrlDecode(Editeditem.UDF5);
                            objInventoryCountDetail.PullOrderNumber = Editeditem.PullOrderNumber;
                            objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                            objInventoryCountDetail.IsApplied = true;
                            if (Editeditem.IsOnlyFromItemUI)
                            {
                                objInventoryCountDetail.EditedFrom = "Web";
                                objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                        }
                        else
                        {
                            objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                            objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                            objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                            objInventoryCountDetail.IsApplied = true;
                            if (Editeditem.IsOnlyFromItemUI)
                            {
                                objInventoryCountDetail.EditedFrom = "Web";
                                objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            return true;
        }

        public InventoryCountDetailDTO PostApplyOnSignleLineItem(InventoryCountDetailDTO Editeditem, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == Editeditem.ID);
                if (objInventoryCountDetail != null)
                {
                    if (Editeditem.SaveAndApply)
                    {
                        objInventoryCountDetail.CountConsignedQuantity = Editeditem.CountConsignedQuantity ?? (-0.000000001);
                        objInventoryCountDetail.CountCustOwnedQuantity = Editeditem.CountCustomerOwnedQuantity ?? (-0.000000001);
                        objInventoryCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                        objInventoryCountDetail.LastUpdatedBy = UserId;
                        objInventoryCountDetail.UDF1 = HttpUtility.UrlDecode(Editeditem.UDF1);
                        objInventoryCountDetail.UDF2 = HttpUtility.UrlDecode(Editeditem.UDF2);
                        objInventoryCountDetail.UDF3 = HttpUtility.UrlDecode(Editeditem.UDF3);
                        objInventoryCountDetail.UDF4 = HttpUtility.UrlDecode(Editeditem.UDF4);
                        objInventoryCountDetail.UDF5 = HttpUtility.UrlDecode(Editeditem.UDF5);
                        objInventoryCountDetail.PullOrderNumber = Editeditem.PullOrderNumber;
                        objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetail.IsApplied = true;
                        objInventoryCountDetail.AppliedDate = DateTime.Now.Date;

                        if (!string.IsNullOrWhiteSpace(Editeditem.EditedFrom))
                        {
                            objInventoryCountDetail.EditedFrom = Editeditem.EditedFrom;

                        }
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objInventoryCountDetail.EditedFrom = "Web";
                            objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    else
                    {
                        objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetail.IsApplied = true;
                        objInventoryCountDetail.AppliedDate = DateTime.Now.Date;

                        if (!string.IsNullOrWhiteSpace(Editeditem.EditedFrom))
                        {
                            objInventoryCountDetail.EditedFrom = Editeditem.EditedFrom;

                        }
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objInventoryCountDetail.EditedFrom = "Web";
                            objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    context.SaveChanges();
                    //Apply Header if all line items are applied
                    ApplyCountHeader(objInventoryCountDetail.InventoryCountGUID, objInventoryCountDetail.RoomId, objInventoryCountDetail.CompanyId, objInventoryCountDetail.EditedFrom);
                }
            }
            return Editeditem;
        }

        public InventoryCountDetailDTO ApplyOnSignleLineItem(InventoryCountDetailDTO Editeditem, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == Editeditem.ID);
                if (objInventoryCountDetail != null)
                {
                    ItemMasterDTO objItem = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objInventoryCountDetail.ItemGUID);
                    double ItemLocationConsignedQty = 0;
                    double ItemLocationCoownedQty = 0;
                    var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetail.ItemGUID && t.BinID == objInventoryCountDetail.BinID);
                    if (q.Any())
                    {
                        ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                        ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    }
                    if (Editeditem.SaveAndApply)
                    {
                        objInventoryCountDetail.CountConsignedQuantity = Editeditem.CountConsignedQuantity ?? 0;
                        objInventoryCountDetail.CountCustOwnedQuantity = Editeditem.CountCustomerOwnedQuantity ?? 0;
                        objInventoryCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                        objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                        objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                        objInventoryCountDetail.LastUpdatedBy = UserId;
                        objInventoryCountDetail.UDF1 = HttpUtility.UrlDecode(Editeditem.UDF1);
                        objInventoryCountDetail.UDF2 = HttpUtility.UrlDecode(Editeditem.UDF2);
                        objInventoryCountDetail.UDF3 = HttpUtility.UrlDecode(Editeditem.UDF3);
                        objInventoryCountDetail.UDF4 = HttpUtility.UrlDecode(Editeditem.UDF4);
                        objInventoryCountDetail.UDF5 = HttpUtility.UrlDecode(Editeditem.UDF5);
                        objInventoryCountDetail.PullOrderNumber = Editeditem.PullOrderNumber;
                        objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetail.IsApplied = true;
                        objInventoryCountDetail.CusOwnedDifference = (Editeditem.CountCustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty;
                        if (objItem != null && objItem.Consignment)
                            objInventoryCountDetail.ConsignedDifference = (Editeditem.CountConsignedQuantity ?? 0) - ItemLocationConsignedQty;
                        else
                            objInventoryCountDetail.ConsignedDifference = 0;
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objInventoryCountDetail.EditedFrom = "Web";
                            objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    else
                    {
                        objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                        objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                        objInventoryCountDetail.IsApplied = true;
                        objInventoryCountDetail.CusOwnedDifference = (Editeditem.CountCustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty;
                        if (objItem != null && objItem.Consignment)
                            objInventoryCountDetail.ConsignedDifference = (Editeditem.CountConsignedQuantity ?? 0) - ItemLocationConsignedQty;
                        else
                            objInventoryCountDetail.ConsignedDifference = 0;
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objInventoryCountDetail.EditedFrom = "Web";
                            objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    context.SaveChanges();
                    Editeditem.CustomerOwnedQuantity = ItemLocationCoownedQty;
                    Editeditem.ConsignedQuantity = ItemLocationConsignedQty;
                    Editeditem.CusOwnedDifference = (Editeditem.CountCustomerOwnedQuantity ?? 0) - (Editeditem.CustomerOwnedQuantity ?? 0);
                    if (objItem != null && objItem.Consignment)
                        Editeditem.ConsignedDifference = (Editeditem.CountConsignedQuantity ?? 0) - (Editeditem.ConsignedQuantity ?? 0);
                    else
                        Editeditem.ConsignedDifference = 0;
                    Editeditem.TotalDifference = (Editeditem.CusOwnedDifference ?? 0) + (Editeditem.ConsignedDifference ?? 0);
                    Editeditem.InventoryClassification = objItem.InventoryClassification ?? 0;
                    Editeditem.ItemGUID = objInventoryCountDetail.ItemGUID;
                    Editeditem.BinID = objInventoryCountDetail.BinID;
                    Editeditem.IsStagingLocationCount = objInventoryCountDetail.IsStagingLocationCount;
                    if (Editeditem.IsOnlyFromItemUI)
                    {
                        Editeditem.EditedFrom = "Web";
                        Editeditem.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                }
            }
            return Editeditem;
        }

        public InventoryCountDetailDTO CalculateCountDifferences(long ID, double CountConsignedQty, double CountCusOwnedQty)
        {
            InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            InventoryCountDetail objInventoryCountDetail = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == ID);
            }
            return objInventoryCountDetailDTO;
        }

        public InventoryCountDetailDTO BeforeApplyAction(InventoryCountDetailDTO Editeditem, long UserId, List<CountLineItemDetailDTO> lstCountLineItemDetail = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail objInventoryCountDetail = context.InventoryCountDetails.FirstOrDefault(t => t.ID == Editeditem.ID);

                if (objInventoryCountDetail != null)
                {
                    ItemMasterDTO objItem = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objInventoryCountDetail.ItemGUID);
                    double ItemLocationConsignedQty = 0;
                    double ItemLocationCoownedQty = 0;

                    if (Editeditem.IsStagingLocationCount)
                    {
                        var q = context.MaterialStagingPullDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetail.ItemGUID && t.StagingBinId == objInventoryCountDetail.BinID);
                        if (q.Any())
                        {
                            ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                            ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                        }
                    }
                    else
                    {
                        var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objInventoryCountDetail.ItemGUID && t.BinID == objInventoryCountDetail.BinID);
                        if (q.Any())
                        {
                            ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                            ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                        }
                    }
                    if (Editeditem.SaveAndApply)
                    {
                        objInventoryCountDetail.CountConsignedQuantity = Editeditem.CountConsignedQuantity ?? (-0.000000001);
                        objInventoryCountDetail.CountCustOwnedQuantity = Editeditem.CountCustomerOwnedQuantity ?? (-0.000000001);
                        objInventoryCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                        objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                        objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                        objInventoryCountDetail.LastUpdatedBy = UserId;
                        objInventoryCountDetail.UDF1 = HttpUtility.UrlDecode(Editeditem.UDF1);
                        objInventoryCountDetail.UDF2 = HttpUtility.UrlDecode(Editeditem.UDF2);
                        objInventoryCountDetail.UDF3 = HttpUtility.UrlDecode(Editeditem.UDF3);
                        objInventoryCountDetail.UDF4 = HttpUtility.UrlDecode(Editeditem.UDF4);
                        objInventoryCountDetail.UDF5 = HttpUtility.UrlDecode(Editeditem.UDF5);
                        objInventoryCountDetail.PullOrderNumber = Editeditem.PullOrderNumber;
                        objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetail.SupplierAccountGuid = Editeditem.SupplierAccountGuid;

                        //TODO: Start Jira Issue: WI-1560 below lin commented
                        //objInventoryCountDetail.IsApplied = true;

                        //----------------------------------------------------------------------------------
                        //
                        double CusOwnedDifference = 0;
                        double ConsignedDifference = 0;
                        bool someflag = GetCustomerConsignDiff(lstCountLineItemDetail, (objItem == null ? false : objItem.Consignment), (objItem == null ? false : objItem.SerialNumberTracking), context, out CusOwnedDifference, out ConsignedDifference);
                        if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0 && someflag)
                        {
                            if (objInventoryCountDetail.CusOwnedDifference != (-0.000000001))
                            {
                                objInventoryCountDetail.CusOwnedDifference = CusOwnedDifference;
                            }
                            if (objItem != null && objItem.Consignment)
                            {
                                if (objInventoryCountDetail.ConsignedDifference != (-0.000000001))
                                {
                                    objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                                }
                            }
                            else
                            {
                                if (objInventoryCountDetail.ConsignedDifference != (-0.000000001))
                                {
                                    objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                                }
                            }
                        }
                        else
                        {
                            objInventoryCountDetail.CusOwnedDifference = Editeditem.CountCustomerOwnedQuantity.HasValue ? ((Editeditem.CountCustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty) : (-0.000000001);
                            if (objItem != null && objItem.Consignment)
                                objInventoryCountDetail.ConsignedDifference = Editeditem.CountConsignedQuantity.HasValue ? ((Editeditem.CountConsignedQuantity ?? 0) - ItemLocationConsignedQty) : (-0.000000001);
                            else
                            {
                                objInventoryCountDetail.ConsignedDifference = 0;
                            }
                        }

                        //----------------------------------------------------------------------------------
                        //
                        if (!string.IsNullOrWhiteSpace(Editeditem.EditedFrom))
                        {
                            objInventoryCountDetail.EditedFrom = Editeditem.EditedFrom;
                        }
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objInventoryCountDetail.EditedFrom = "Web";
                            objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    else
                    {
                        objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetail.ItemCustOwnedQuantity = ItemLocationCoownedQty;
                        objInventoryCountDetail.ItemConsignedQuantity = ItemLocationConsignedQty;
                        objInventoryCountDetail.SupplierAccountGuid = Editeditem.SupplierAccountGuid;
                        objInventoryCountDetail.PullOrderNumber = Editeditem.PullOrderNumber;
                        //TODO: Start Jira Issue: WI-1560 below lin commented
                        //objInventoryCountDetail.IsApplied = true;

                        //----------------------------------------------------------------------------------
                        //
                        double CusOwnedDifference = 0;
                        double ConsignedDifference = 0;
                        bool someflag = GetCustomerConsignDiff(lstCountLineItemDetail, (objItem == null ? false : objItem.Consignment), (objItem == null ? false : objItem.SerialNumberTracking), context, out CusOwnedDifference, out ConsignedDifference);
                        if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0 && someflag)
                        {
                            if (objInventoryCountDetail.CusOwnedDifference != (-0.000000001))
                            {
                                objInventoryCountDetail.CusOwnedDifference = CusOwnedDifference;
                            }

                            if (objItem != null && objItem.Consignment)
                            {
                                if (objInventoryCountDetail.ConsignedDifference != (-0.000000001))
                                {
                                    objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                                }
                            }
                            else
                            {
                                if (objInventoryCountDetail.ConsignedDifference != (-0.000000001))
                                {
                                    objInventoryCountDetail.ConsignedDifference = ConsignedDifference;
                                }
                            }
                        }
                        else
                        {
                            objInventoryCountDetail.CusOwnedDifference = Editeditem.CountCustomerOwnedQuantity.HasValue ? ((Editeditem.CountCustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty) : (-0.000000001);
                            if (objItem != null && objItem.Consignment)
                                objInventoryCountDetail.ConsignedDifference = Editeditem.CountConsignedQuantity.HasValue ? ((Editeditem.CountConsignedQuantity ?? 0) - ItemLocationConsignedQty) : (-0.000000001);
                            else
                                objInventoryCountDetail.ConsignedDifference = (-0.000000001);
                        }
                        if (!string.IsNullOrWhiteSpace(Editeditem.EditedFrom))
                        {
                            objInventoryCountDetail.EditedFrom = Editeditem.EditedFrom;
                        }
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objInventoryCountDetail.EditedFrom = "Web";
                            objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    //TODO: Start Jira Issue: WI-1560 below lin un-commented
                    context.SaveChanges();
                    Editeditem.GUID = objInventoryCountDetail.GUID;
                    Editeditem.CustomerOwnedQuantity = ItemLocationCoownedQty;
                    Editeditem.ConsignedQuantity = ItemLocationConsignedQty;
                    Editeditem.CusOwnedDifference = (objInventoryCountDetail.CusOwnedDifference == (-0.000000001)) ? (new double?()) : objInventoryCountDetail.CusOwnedDifference;
                    Editeditem.ConsignedDifference = (objInventoryCountDetail.ConsignedDifference == (-0.000000001)) ? (new double?()) : objInventoryCountDetail.ConsignedDifference;
                    Editeditem.TotalDifference = (Editeditem.CusOwnedDifference ?? 0) + (Editeditem.ConsignedDifference ?? 0);
                    Editeditem.InventoryClassification = objItem.InventoryClassification ?? 0;
                    Editeditem.ItemType = objItem.ItemType;
                    Editeditem.ItemGUID = objInventoryCountDetail.ItemGUID;
                    Editeditem.BinID = objInventoryCountDetail.BinID;
                    Editeditem.IsStagingLocationCount = objInventoryCountDetail.IsStagingLocationCount;

                    if (Editeditem.IsOnlyFromItemUI)
                    {
                        Editeditem.EditedFrom = "Web";
                        Editeditem.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                }
            }
            return Editeditem;
        }

        public InventoryCountDetailDTO GetInventoryCountdtlByGUId(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from ci in context.InventoryCountDetails
                        join ic in context.InventoryCounts on ci.InventoryCountGUID equals ic.GUID
                        join im in context.ItemMasters on ci.ItemGUID equals im.GUID
                        join bm in context.BinMasters on ci.BinID equals bm.ID
                        join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                        from ci_cc in ci_cc_join.DefaultIfEmpty()
                        join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                        from ci_cu in ci_cu_join.DefaultIfEmpty()
                        join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                        from ci_rm in ci_rm_join.DefaultIfEmpty()
                        where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.GUID == GUID
                        select new InventoryCountDetailDTO
                        {
                            AppliedDate = ci.AppliedDate,
                            BinGUID = ci.BinGUID ?? Guid.Empty,
                            BinID = ci.BinID,
                            BinNumber = bm.BinNumber,
                            CompanyId = ci.CompanyId,
                            ConsignedDifference = ci.ConsignedDifference,
                            ConsignedQuantity = ci.ItemConsignedQuantity,
                            CountConsignedQuantity = ci.CountConsignedQuantity,
                            CountConsignedQuantityEntry = ci.CountConsignedQuantity,
                            CountCustomerOwnedQuantity = ci.CountCustOwnedQuantity,
                            CountCustomerOwnedQuantityEntry = ci.CountCustOwnedQuantity,
                            CountDate = ic.CountDate,
                            CountItemStatus = ci.CountItemStatus,
                            CountLineItemDescription = ci.CountLineItemDescription,
                            CountLineItemDescriptionEntry = ci.CountLineItemDescription,
                            CountName = ic.CountName,
                            CountQuantity = (ci.CountConsignedQuantity + ci.CountCustOwnedQuantity),
                            CountStatus = ic.CountStatus,
                            CountType = ic.CountType,
                            Created = ci.Created,
                            CreatedBy = ci.CreatedBy,
                            CreatedByName = ci_cc.UserName,
                            CusOwnedDifference = ci.CusOwnedDifference,
                            CustomerOwnedQuantity = ci.ItemCustOwnedQuantity,
                            GUID = ci.GUID,
                            ID = ci.ID,
                            InventoryCountGUID = ci.InventoryCountGUID,
                            IsApplied = ci.IsApplied,
                            IsArchived = ci.IsArchived,
                            IsClosed = false,
                            IsDeleted = ci.IsDeleted,
                            IsStagingLocationCount = ci.IsStagingLocationCount,
                            ItemGUID = ci.ItemGUID,
                            ItemNumber = im.ItemNumber,
                            ItemQuantity = ci.ItemCustOwnedQuantity + ci.ItemConsignedQuantity,
                            InventoryClassification = im.InventoryClassification ?? 0,
                            LastUpdatedBy = ci.LastUpdatedBy,
                            RoomId = ci.RoomId,
                            RoomName = ci_rm.RoomName,
                            SaveAndApply = false,
                            UDF1 = ci.UDF1,
                            UDF2 = ci.UDF2,
                            UDF3 = ci.UDF3,
                            UDF4 = ci.UDF4,
                            UDF5 = ci.UDF5,
                            PullOrderNumber = ci.PullOrderNumber,
                            Updated = ci.Updated,
                            UpdatedByName = ci_cu.UserName,
                            AddedFrom = ci.AddedFrom,
                            EditedFrom = ci.EditedFrom,
                            ReceivedOn = ci.ReceivedOn,
                            ReceivedOnWeb = ci.ReceivedOnWeb,
                            LotNumberTracking = im.LotNumberTracking,
                            SerialNumberTracking = im.SerialNumberTracking,
                            DateCodeTracking = im.DateCodeTracking
                        }).FirstOrDefault();
            }
        }

        public InventoryCountDetailDTO GetInventoryCountdtlById(long ID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from ci in context.InventoryCountDetails
                        join ic in context.InventoryCounts on ci.InventoryCountGUID equals ic.GUID
                        join im in context.ItemMasters on ci.ItemGUID equals im.GUID
                        join bm in context.BinMasters on ci.BinID equals bm.ID
                        join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                        from ci_cc in ci_cc_join.DefaultIfEmpty()
                        join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                        from ci_cu in ci_cu_join.DefaultIfEmpty()
                        join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                        from ci_rm in ci_rm_join.DefaultIfEmpty()
                        where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.ID == ID
                        select new InventoryCountDetailDTO
                        {
                            AppliedDate = ci.AppliedDate,
                            BinGUID = ci.BinGUID ?? Guid.Empty,
                            BinID = ci.BinID,
                            BinNumber = bm.BinNumber,
                            CompanyId = ci.CompanyId,
                            ConsignedDifference = ci.ConsignedDifference,
                            ConsignedQuantity = ci.ItemConsignedQuantity,
                            CountConsignedQuantity = ci.CountConsignedQuantity,
                            CountConsignedQuantityEntry = ci.CountConsignedQuantity,
                            CountCustomerOwnedQuantity = ci.CountCustOwnedQuantity,
                            CountCustomerOwnedQuantityEntry = ci.CountCustOwnedQuantity,
                            CountDate = ic.CountDate,
                            CountItemStatus = ci.CountItemStatus,
                            CountLineItemDescription = ci.CountLineItemDescription,
                            CountLineItemDescriptionEntry = ci.CountLineItemDescription,
                            CountName = ic.CountName,
                            CountQuantity = (ci.CountConsignedQuantity) + (ci.CountCustOwnedQuantity),
                            CountStatus = ic.CountStatus,
                            CountType = ic.CountType,
                            Created = ci.Created,
                            CreatedBy = ci.CreatedBy,
                            CreatedByName = ci_cc.UserName,
                            CusOwnedDifference = ci.CusOwnedDifference,
                            CustomerOwnedQuantity = ci.ItemCustOwnedQuantity,
                            GUID = ci.GUID,
                            ID = ci.ID,
                            InventoryCountGUID = ci.InventoryCountGUID,
                            IsApplied = ci.IsApplied,
                            IsArchived = ci.IsArchived,
                            IsClosed = false,
                            IsDeleted = ci.IsDeleted,
                            IsStagingLocationCount = ci.IsStagingLocationCount,
                            ItemGUID = ci.ItemGUID,
                            ItemNumber = im.ItemNumber,
                            ItemQuantity = ci.ItemCustOwnedQuantity + ci.ItemConsignedQuantity,
                            InventoryClassification = im.InventoryClassification ?? 0,
                            LastUpdatedBy = ci.LastUpdatedBy,
                            RoomId = ci.RoomId,
                            RoomName = ci_rm.RoomName,
                            SaveAndApply = false,
                            UDF1 = ci.UDF1,
                            UDF2 = ci.UDF2,
                            UDF3 = ci.UDF3,
                            UDF4 = ci.UDF4,
                            UDF5 = ci.UDF5,
                            PullOrderNumber = ci.PullOrderNumber,
                            Updated = ci.Updated,
                            UpdatedByName = ci_cu.UserName,
                            AddedFrom = ci.AddedFrom,
                            EditedFrom = ci.EditedFrom,
                            ReceivedOn = ci.ReceivedOn,
                            ReceivedOnWeb = ci.ReceivedOnWeb
                        }).FirstOrDefault();
            }
        }

        public List<InventoryCountDetailDTO> GetAllLineItemsWithinCount(long CountId)
        {
            List<InventoryCountDetailDTO> lstLineitems = new List<InventoryCountDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLineitems = (from icd in context.InventoryCountDetails
                                join bm in context.BinMasters on icd.BinID equals bm.ID
                                join im in context.ItemMasters on icd.ItemGUID equals im.GUID
                                join ic in context.InventoryCounts on icd.InventoryCountGUID equals ic.GUID
                                join cb in context.UserMasters on icd.CreatedBy equals cb.ID into icd_cb_join
                                from icd_cb in icd_cb_join.DefaultIfEmpty()
                                join ub in context.UserMasters on icd.CreatedBy equals ub.ID into icd_ub_join
                                from icd_ub in icd_ub_join.DefaultIfEmpty()
                                where ic.ID == CountId && (icd.IsDeleted == false) && (icd.IsArchived == false)
                                select new InventoryCountDetailDTO
                                {
                                    AppliedDate = icd.AppliedDate,
                                    BinGUID = icd.BinGUID ?? Guid.Empty,
                                    BinID = icd.BinID,
                                    BinNumber = bm.BinNumber,
                                    CompanyId = bm.CompanyID ?? 0,
                                    ConsignedDifference = icd.ConsignedDifference,
                                    ConsignedQuantity = icd.ItemConsignedQuantity,
                                    CountConsignedQuantity = icd.CountConsignedQuantity,
                                    CountConsignedQuantityEntry = icd.CountConsignedQuantity,
                                    CountCustomerOwnedQuantity = icd.CountCustOwnedQuantity,
                                    CountCustomerOwnedQuantityEntry = icd.CountCustOwnedQuantity,
                                    CountDate = ic.CountDate,
                                    CountItemStatus = icd.CountItemStatus,
                                    CountLineItemDescription = icd.CountLineItemDescription,
                                    CountLineItemDescriptionEntry = icd.CountLineItemDescription,
                                    CountName = ic.CountName,
                                    CountQuantity = (icd.CountCustOwnedQuantity) + (icd.CountConsignedQuantity),
                                    CountStatus = ic.CountStatus,
                                    CountType = ic.CountType,
                                    Created = icd.Created,
                                    CreatedBy = icd.CreatedBy,
                                    CreatedByName = icd_cb.UserName,
                                    CusOwnedDifference = icd.CusOwnedDifference,
                                    CustomerOwnedQuantity = icd.ItemCustOwnedQuantity,
                                    GUID = icd.GUID,
                                    ID = icd.ID,
                                    InventoryClassification = icd.InventoryClassification ?? 0,
                                    InventoryClassificationCode = string.Empty,
                                    InventoryCountGUID = icd.InventoryCountGUID,
                                    IsApplied = icd.IsApplied,
                                    IsArchived = icd.IsArchived,
                                    IsClosed = false,
                                    IsDeleted = icd.IsDeleted,
                                    IsStagingLocationCount = icd.IsStagingLocationCount,
                                    ItemGUID = icd.ItemGUID,
                                    ItemNumber = im.ItemNumber,
                                    ItemQuantity = icd.ItemConsignedQuantity + icd.ItemCustOwnedQuantity,
                                    LastUpdatedBy = icd.LastUpdatedBy,
                                    RoomId = icd.RoomId,
                                    RoomName = string.Empty,
                                    SaveAndApply = false,
                                    TotalDifference = ((icd.CountCustOwnedQuantity) + (icd.CountConsignedQuantity)) - (icd.ItemConsignedQuantity + icd.ItemCustOwnedQuantity),
                                    UDF1 = icd.UDF1,
                                    UDF2 = icd.UDF2,
                                    UDF3 = icd.UDF3,
                                    UDF4 = icd.UDF4,
                                    UDF5 = icd.UDF5,
                                    PullOrderNumber = icd.PullOrderNumber,
                                    Updated = icd.Updated,
                                    UpdatedByName = icd_ub.UserName,
                                    AddedFrom = icd.AddedFrom,
                                    EditedFrom = icd.EditedFrom,
                                    ReceivedOn = icd.ReceivedOn,
                                    ReceivedOnWeb = icd.ReceivedOnWeb
                                }).ToList();
            }
            return lstLineitems;
        }

        public InventoryCountDTO GetHistoryRecord(Int64 ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", ID), new SqlParameter("@dbName", DataBaseName) };
                return (from u in context.Database.SqlQuery<InventoryCountDTO>("exec [GetInventoryCountChangeLogByHistoryID] @HistoryID,@dbName", params1)
                        select new InventoryCountDTO
                        {
                            HistoryID = u.HistoryID,
                            CompanyId = u.CompanyId,
                            CompleteCauseCountGUID = u.CompleteCauseCountGUID,
                            CountCompletionDate = u.CountCompletionDate,
                            CountDate = u.CountDate,
                            CountItemDescription = u.CountItemDescription,
                            CountName = u.CountName,
                            CountStatus = u.CountStatus,
                            CountType = u.CountType,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            CreatedByName = u.CreatedByName,
                            GUID = u.GUID,
                            ID = u.ID,
                            IsApplied = u.IsApplied,
                            IsArchived = u.IsArchived,
                            IsAutomatedCompletion = u.IsAutomatedCompletion,
                            IsClosed = u.IsClosed,
                            IsDeleted = u.IsDeleted,
                            LastUpdatedBy = u.LastUpdatedBy,
                            RoomId = u.RoomId,
                            RoomName = u.RoomName,
                            TotalItemsWithinCount = u.TotalItemsWithinCount,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Updated = u.Updated,
                            UpdatedByName = u.UpdatedByName,
                            Year = u.Year,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            ReleaseNumber = u.ReleaseNumber
                        }).FirstOrDefault();
            }
        }

        public List<InventoryCountDetailDTO> GetHistoryRecordbyCountGUID(Guid ICGuiD)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from icd in context.InventoryCountDetail_History
                        join bm in context.BinMasters on icd.BinID equals bm.ID
                        join im in context.ItemMasters on icd.ItemGUID equals im.GUID
                        join ic in context.InventoryCounts on icd.InventoryCountGUID equals ic.GUID
                        join cb in context.UserMasters on icd.CreatedBy equals cb.ID into icd_cb_join
                        from icd_cb in icd_cb_join.DefaultIfEmpty()
                        join ub in context.UserMasters on icd.CreatedBy equals ub.ID into icd_ub_join
                        from icd_ub in icd_ub_join.DefaultIfEmpty()
                        where ic.GUID == ICGuiD
                        select new InventoryCountDetailDTO
                        {
                            HistoryID = icd.HistoryID,
                            ICHistoryId = icd.ICHistoryId ?? 0,
                            AppliedDate = icd.AppliedDate,
                            BinGUID = icd.BinGUID,
                            BinID = icd.BinID,
                            BinNumber = bm.BinNumber,
                            CompanyId = bm.CompanyID ?? 0,
                            ConsignedDifference = (icd.ConsignedDifference == (-0.000000001)) ? 0 : icd.ConsignedDifference,
                            ConsignedQuantity = icd.ItemConsignedQuantity,
                            CountConsignedQuantity = (icd.CountConsignedQuantity == (-0.000000001)) ? 0 : icd.CountConsignedQuantity,
                            CountConsignedQuantityEntry = (icd.CountConsignedQuantity == (-0.000000001)) ? 0 : icd.CountConsignedQuantity,
                            CountCustomerOwnedQuantity = (icd.CountCustOwnedQuantity == (-0.000000001)) ? 0 : icd.CountCustOwnedQuantity,
                            CountCustomerOwnedQuantityEntry = (icd.CountCustOwnedQuantity == (-0.000000001)) ? 0 : icd.CountCustOwnedQuantity,
                            CountDate = ic.CountDate,
                            CountItemStatus = icd.CountItemStatus,
                            CountLineItemDescription = icd.CountLineItemDescription,
                            CountLineItemDescriptionEntry = icd.CountLineItemDescription,
                            CountName = ic.CountName,
                            CountQuantity = ((icd.CountCustOwnedQuantity == (-0.000000001)) ? 0 : icd.CountCustOwnedQuantity) + ((icd.CountConsignedQuantity == (-0.000000001)) ? 0 : icd.CountConsignedQuantity),
                            CountStatus = ic.CountStatus,
                            CountType = ic.CountType,
                            Created = icd.Created,
                            CreatedBy = icd.CreatedBy,
                            CreatedByName = icd_cb.UserName,
                            CusOwnedDifference = (icd.CusOwnedDifference == (-0.000000001)) ? 0 : icd.CusOwnedDifference,
                            CustomerOwnedQuantity = (icd.ItemCustOwnedQuantity == (-0.000000001)) ? 0 : icd.ItemCustOwnedQuantity,
                            GUID = icd.GUID,
                            ID = icd.ID,
                            InventoryClassification = icd.InventoryClassification ?? 0,
                            InventoryClassificationCode = string.Empty,
                            InventoryCountGUID = icd.InventoryCountGUID,
                            IsApplied = icd.IsApplied,
                            IsArchived = icd.IsArchived,
                            IsClosed = false,
                            IsDeleted = icd.IsDeleted,
                            IsStagingLocationCount = icd.IsStagingLocationCount,
                            ItemGUID = icd.ItemGUID,
                            ItemNumber = im.ItemNumber,
                            ItemQuantity = icd.ItemConsignedQuantity + icd.ItemCustOwnedQuantity,
                            LastUpdatedBy = icd.LastUpdatedBy,
                            RoomId = icd.RoomId,
                            RoomName = string.Empty,
                            SaveAndApply = false,
                            TotalDifference = (icd.CountCustOwnedQuantity + icd.CountConsignedQuantity) - (icd.ItemConsignedQuantity + icd.ItemCustOwnedQuantity),
                            UDF1 = icd.UDF1,
                            UDF2 = icd.UDF2,
                            UDF3 = icd.UDF3,
                            UDF4 = icd.UDF4,
                            UDF5 = icd.UDF5,
                            PullOrderNumber = icd.PullOrderNumber,
                            Updated = icd.Updated,
                            UpdatedByName = icd_ub.UserName,
                            AddedFrom = icd.AddedFrom,
                            EditedFrom = icd.EditedFrom,
                            ReceivedOn = icd.ReceivedOn,
                            ReceivedOnWeb = icd.ReceivedOnWeb
                        }).ToList();
            }
        }

        public Int32 GetInventoryItemCount()
        {
            Int32 ret = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ret = (from u in context.InventoryCounts
                       where u.IsDeleted == false && u.IsArchived == false && u.IsClosed == false
                       select u
                        ).ToList().Count();
            }
            return ret;
        }

        public List<InventoryCountDTO> GetInventoryCountForChart(int MaxItemsInGraph, string SortColumnName, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                {
                    new SqlParameter("@MaxItemsInGraph", MaxItemsInGraph),
                    new SqlParameter("@SortColumnName", SortColumnName),
                    new SqlParameter("@RoomId", RoomId),
                    new SqlParameter("@CompanyId", CompanyId)
                };

                return context.Database.SqlQuery<InventoryCountDTO>("exec [GetInventoryCountForChart] @MaxItemsInGraph,@SortColumnName,@RoomId,@CompanyId", params1).ToList();
            }
        }

        public void ApplyCountHeader(Guid ICGuid, long RoomID, long CompanyID, string EditedFrom)
        {
            InventoryCountDTO objInvCount = new InventoryCountDTO();
            objInvCount = GetInventoryCountByGUId(ICGuid, RoomID, CompanyID);

            if (objInvCount != null)
            {
                List<InventoryCountDetailDTO> LstDtl = new List<InventoryCountDetailDTO>();
                InventoryCountDetailDAL inventoryCountDetailDAL = new InventoryCountDetailDAL(base.DataBaseName);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var unAppliedInventoryDetailCount = inventoryCountDetailDAL.GetUnAppliedInventoryDetailCountByICGUIDPlain(ICGuid);
                    InventoryCount objEntCnt = context.InventoryCounts.Where(x => x.ID == objInvCount.ID).FirstOrDefault();

                    if (unAppliedInventoryDetailCount == 0)
                    {
                        if (objEntCnt != null)
                        {
                            objEntCnt.Updated = DateTimeUtility.DateTimeNow;
                            objEntCnt.IsApplied = true;

                            if (string.IsNullOrWhiteSpace(EditedFrom))
                            {
                                objEntCnt.EditedFrom = "Web";
                            }
                            else
                            {
                                objEntCnt.EditedFrom = EditedFrom;
                            }

                            objEntCnt.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    else
                    {
                        objEntCnt.Updated = DateTimeUtility.DateTimeNow;
                        objEntCnt.IsApplied = false;

                        if (string.IsNullOrWhiteSpace(EditedFrom))
                        {
                            objEntCnt.EditedFrom = "Web";
                        }
                        else
                        {
                            objEntCnt.EditedFrom = EditedFrom;
                        }
                        objEntCnt.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }

                    context.SaveChanges();

                }
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForCountPull(Guid ItemGUID, long RoomID, long CompanyID, long BinID, double CustomerOwnedPullQTY, double ConsignedPullQTY, string IsPullQtyLimits, string SerialOrLotNumber, CultureInfo RoomCulture)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>(@"EXEC GetItemLocationsWithLotSerialsForCountPull '" + ItemGUID.ToString() + "'," + BinID + "," + CompanyID + "," + RoomID + "," + CustomerOwnedPullQTY + "," + ConsignedPullQTY + "," + IsPullQtyLimits + ",'" + SerialOrLotNumber + "'")
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        Expiration = il.Expiration,
                                        SerialNumber = il.SerialNumber,
                                        LotNumber = il.LotNumber,
                                        Received = !il.ReceivedDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true), RoomCulture).ToString("MM/dd/yyyy"),
                                        ReceivedDate = il.ReceivedDate,

                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                                        //CumulativeTotalQuantity = il.CumulativeTotalQuantity
                                        ConsignedTobePulled = il.ConsignedQuantity ?? 0,
                                        CustomerOwnedTobePulled = il.CustomerOwnedQuantity ?? 0,
                                    }).ToList();
                return lstItemLocations;
            }
        }

        public InventoryCountDTO GetInventoryCountByLIGUId(Guid LIGUID, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LIGUID", LIGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("EXEC GetInventoryCountByLIGUId @LIGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<CountLineItemDetailDTO> GetLotDetailForCount(Guid ItemGUID, Int64 BinId, string RoomDateFormat, Int64 CompanyId, Int64 RoomId, bool IsStage)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@BinId", BinId), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomId", RoomId), new SqlParameter("@IsStage", IsStage) };

                List<CountLineItemDetailDTO> lstCountLineItemDetail = (from A in context.Database.SqlQuery<CountLineItemDetailDTO>("exec [csp_ItemLocationDetails_GetLotDetailForCount] @ItemGUID,@BinId,@CompanyId,@RoomId,@IsStage", params1)
                                                                       select new CountLineItemDetailDTO()
                                                                       {
                                                                           ID = 0,
                                                                           ItemGUID = A.ItemGUID,
                                                                           ItemNumber = A.ItemNumber,
                                                                           ItemDescription = A.ItemDescription,
                                                                           ItemType = A.ItemType,
                                                                           Consignment = A.Consignment,
                                                                           Comment = A.Comment,
                                                                           BinID = A.BinID,
                                                                           BinNumber = A.BinNumber,
                                                                           CustomerOwnedQuantity = A.CustomerOwnedQuantity,
                                                                           AvailableQuantity = A.AvailableQuantity,
                                                                           CountCustomerOwnedQuantity = A.CustomerOwnedQuantity,
                                                                           ConsignedQuantity = A.ConsignedQuantity,
                                                                           CountConsignedQuantity = A.ConsignedQuantity,
                                                                           SerialNumberTracking = A.SerialNumberTracking,
                                                                           LotNumberTracking = A.LotNumberTracking,
                                                                           LotNumber = ((A.LotNumberTracking == null || A.LotNumberTracking == false) ? A.LotSerialNumber : ""),
                                                                           SerialNumber = ((A.SerialNumberTracking == null || A.SerialNumberTracking == false) ? A.LotSerialNumber : ""),
                                                                           LotSerialNumber = A.LotSerialNumber,
                                                                           DateCodeTracking = A.DateCodeTracking,
                                                                           Expiration = (A.ExpirationDate == null ? "" : A.ExpirationDate.Value.ToString(RoomDateFormat)),
                                                                           Received = (A.ReceivedDate == null ? "" : A.ReceivedDate.Value.ToString("")),
                                                                           ExpirationDate = A.ExpirationDate,
                                                                           ReceivedDate = A.ReceivedDate,
                                                                           Cost = A.Cost,
                                                                           GUID = Guid.Empty,
                                                                           Created = null,
                                                                           Updated = null,
                                                                           CreatedBy = null,
                                                                           LastUpdatedBy = null,
                                                                           IsDeleted = null,
                                                                           IsArchived = null,
                                                                           CompanyID = null,
                                                                           RoomID = null,
                                                                           CountGUID = null,
                                                                           CountDetailGUID = null,
                                                                           ReceivedOn = null,
                                                                           ReceivedOnWeb = null,
                                                                           AddedFrom = null,
                                                                           EditedFrom = null

                                                                       }).ToList();
                return lstCountLineItemDetail;
            }
        }

        public List<CountLineItemDetailDTO> GetLotDetailForCountByCountDetailGUID(Guid CountDetailGUID, Guid ItemGUID, string RoomDateFormat, Int64 CompanyId, Int64 RoomId, bool AppendExpDate = true)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<CountLineItemDetailDTO> lstCountLineItemDetail = (from A in context.Database.SqlQuery<CountLineItemDetailDTO>(@"EXEC csp_GetLotDetailForCountByCountDetailGUID '" + CountDetailGUID.ToString() + "', '" + ItemGUID.ToString() + "', " + CompanyId.ToString() + ", " + RoomId.ToString() + ", " + (AppendExpDate == true ? "1" : "0"))
                                                                       select new CountLineItemDetailDTO
                                                                       {
                                                                           ID = 0,
                                                                           ItemGUID = A.ItemGUID,
                                                                           ItemNumber = A.ItemNumber,
                                                                           ItemDescription = A.ItemDescription,
                                                                           ItemType = A.ItemType,
                                                                           Consignment = A.Consignment,
                                                                           Comment = A.Comment,
                                                                           BinID = A.BinID,
                                                                           BinNumber = A.BinNumber,
                                                                           CustomerOwnedQuantity = A.CustomerOwnedQuantity,
                                                                           CountCustomerOwnedQuantity = A.CountCustomerOwnedQuantity,
                                                                           ConsignedQuantity = A.ConsignedQuantity,
                                                                           CountConsignedQuantity = A.CountConsignedQuantity,
                                                                           AvailableQuantity = A.AvailableQuantity,
                                                                           IsConsigned = A.IsConsigned,
                                                                           SerialNumberTracking = A.SerialNumberTracking,
                                                                           LotNumberTracking = A.LotNumberTracking,
                                                                           LotNumber = (!string.IsNullOrWhiteSpace(A.LotNumber)) ? A.LotNumber.Trim() : string.Empty,
                                                                           SerialNumber = (!string.IsNullOrWhiteSpace(A.SerialNumber)) ? A.SerialNumber.Trim() : string.Empty,
                                                                           LotSerialNumber = A.LotSerialNumber,
                                                                           DateCodeTracking = A.DateCodeTracking,
                                                                           Expiration = A.Expiration,
                                                                           Received = A.Received,
                                                                           ExpirationDate = A.ExpirationDate,
                                                                           ReceivedDate = A.ReceivedDate,
                                                                           Cost = A.Cost,
                                                                           GUID = A.GUID,
                                                                           Created = A.Created,
                                                                           Updated = A.Updated,
                                                                           CreatedBy = A.CreatedBy,
                                                                           LastUpdatedBy = A.LastUpdatedBy,
                                                                           IsDeleted = A.IsDeleted,
                                                                           IsArchived = A.IsArchived,
                                                                           CompanyID = A.CompanyID,
                                                                           RoomID = A.RoomID,
                                                                           CountGUID = A.CountGUID,
                                                                           CountDetailGUID = A.CountDetailGUID,
                                                                           ReceivedOn = A.ReceivedOn,
                                                                           ReceivedOnWeb = A.ReceivedOnWeb,
                                                                           AddedFrom = A.AddedFrom,
                                                                           EditedFrom = A.EditedFrom,
                                                                           IsStagingLocationCount = A.IsStagingLocationCount,
                                                                           SupplierAccountGuid = A.SupplierAccountGuid
                                                                       }).ToList();

                return lstCountLineItemDetail;

                //var ItemDetail = (from I in context.ItemMasters
                //                  where I.GUID == ItemGUID
                //                  select new
                //                  {
                //                      ItemNumber = I.ItemNumber,
                //                      Description = I.Description,
                //                      ItemType = I.ItemType,
                //                      Consignment = I.Consignment,
                //                      SerialNumberTracking = I.SerialNumberTracking,
                //                      LotNumberTracking = I.LotNumberTracking,
                //                      DateCodeTracking = I.DateCodeTracking
                //                  }).FirstOrDefault();

                //List<CountLineItemDetailDTO> lstCountLineItemDetail = (from A in context.CountLineItemDetails.AsEnumerable().Select(x =>
                //                                                        {
                //                                                            x.Expiration = (x.ExpirationDate == null ? "" : x.ExpirationDate.Value.ToString(RoomDateFormat));
                //                                                            x.Received = (x.ReceivedDate == null ? "" : x.ReceivedDate.Value.ToString(RoomDateFormat));
                //                                                            return x;
                //                                                        })
                //                                                       join B in context.BinMasters on A.BinID equals B.ID
                //                                                       where A.CountDetailGUID == CountDetailGUID
                //                                                             && (A.IsDeleted == null || A.IsDeleted == false)
                //                                                             && (A.IsArchived == null || A.IsArchived == false)
                //                                                       select new CountLineItemDetailDTO()
                //                                                       {
                //                                                           ID = 0,
                //                                                           ItemGUID = A.ItemGUID,
                //                                                           ItemNumber = ItemDetail.ItemNumber,
                //                                                           ItemDescription = ItemDetail.Description,
                //                                                           ItemType = ItemDetail.ItemType,
                //                                                           Consignment = ItemDetail.Consignment,
                //                                                           Comment = A.Comment,
                //                                                           BinID = A.BinID,
                //                                                           BinNumber = B.BinNumber,

                //                                                           CustomerOwnedQuantity = A.CustomerOwnedQuantity,
                //                                                           CountCustomerOwnedQuantity = (A.CountCustomerOwnedQuantity == (-0.000000001) ? null : A.CountCustomerOwnedQuantity),
                //                                                           ConsignedQuantity = A.ConsignedQuantity,
                //                                                           CountConsignedQuantity = (A.CountConsignedQuantity == (-0.000000001) ? null : A.CountConsignedQuantity),

                //                                                           AvailableQuantity = (A.CountConsignedQuantity != (-0.000000001) ? A.CountConsignedQuantity :
                //                                                                                    (A.CountCustomerOwnedQuantity != (-0.000000001) ? A.CountCustomerOwnedQuantity : null)
                //                                                                               ),
                //                                                           IsConsigned = (A.CountConsignedQuantity != (-0.000000001) ? true : false),

                //                                                           SerialNumberTracking = ItemDetail.SerialNumberTracking,
                //                                                           LotNumberTracking = ItemDetail.LotNumberTracking,
                //                                                           LotNumber = A.LotNumber,
                //                                                           SerialNumber = A.SerialNumber,
                //                                                           LotSerialNumber = ((A.LotNumber != null && A.LotNumber.Trim() != "") ? A.LotNumber : A.SerialNumber) +
                //                                                                             (AppendExpDate == true && ItemDetail.DateCodeTracking == true && A.ExpirationDate != null ? "-" + A.ExpirationDate.Value.ToString("MM/dd/yy") : ""),
                //                                                           DateCodeTracking = ItemDetail.DateCodeTracking,
                //                                                           Expiration = A.Expiration,
                //                                                           Received = A.Received,
                //                                                           ExpirationDate = A.ExpirationDate,
                //                                                           ReceivedDate = A.ReceivedDate,
                //                                                           Cost = A.Cost,
                //                                                           GUID = A.GUID,
                //                                                           Created = A.Created,
                //                                                           Updated = A.Updated,
                //                                                           CreatedBy = A.CreatedBy,
                //                                                           LastUpdatedBy = A.LastUpdatedBy,
                //                                                           IsDeleted = A.IsDeleted,
                //                                                           IsArchived = A.IsArchived,
                //                                                           CompanyID = A.CompanyID,
                //                                                           RoomID = A.RoomID,
                //                                                           CountGUID = A.CountGUID,
                //                                                           CountDetailGUID = A.CountDetailGUID,
                //                                                           ReceivedOn = A.ReceivedOn,
                //                                                           ReceivedOnWeb = A.ReceivedOnWeb,
                //                                                           AddedFrom = A.AddedFrom,
                //                                                           EditedFrom = A.EditedFrom
                //                                                       }).ToList();
                //return lstCountLineItemDetail;
            }
        }

        public bool GetCountDetailGUIDByItemGUIDBinID(Guid CountGUID, Guid ItemGUID, long BinID, out Guid? CountDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail objInventoryCountDetail = (from A in context.InventoryCountDetails
                                                                where A.InventoryCountGUID == CountGUID && A.ItemGUID == ItemGUID && A.BinID == BinID
                                                                      && (A.IsDeleted == false)
                                                                      && (A.IsArchived == false)
                                                                      && A.IsApplied == false
                                                                select A).FirstOrDefault();

                if (objInventoryCountDetail != null)
                {
                    CountDetailGUID = objInventoryCountDetail.GUID;
                    return true;
                }
                else
                {
                    CountDetailGUID = Guid.Empty;
                    return true;
                }
            }
        }

        public bool AddCountLineItemDetail(CountLineItemDetailDTO objCountLineItemDetail)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double CusOwnedDifference = 0;
                double ConsignedDifference = 0;
                List<CountLineItemDetailDTO> lstCountLineItemDetail = new List<CountLineItemDetailDTO>();
                lstCountLineItemDetail.Add(objCountLineItemDetail);
                bool someflag = GetCustomerConsignDiff_CLD(lstCountLineItemDetail, objCountLineItemDetail.IsConsigned.GetValueOrDefault(false), objCountLineItemDetail.SerialNumberTracking.GetValueOrDefault(false), context, out CusOwnedDifference, out ConsignedDifference);

                CountLineItemDetail objDATA = new CountLineItemDetail();
                objDATA.ItemGUID = objCountLineItemDetail.ItemGUID;
                objDATA.BinID = objCountLineItemDetail.BinID;
                objDATA.CustomerOwnedQuantity = objCountLineItemDetail.CustomerOwnedQuantity;
                objDATA.CountCustomerOwnedQuantity = objCountLineItemDetail.CountCustomerOwnedQuantity;
                objDATA.ConsignedQuantity = objCountLineItemDetail.ConsignedQuantity;
                objDATA.CountConsignedQuantity = objCountLineItemDetail.CountConsignedQuantity;
                objDATA.Comment = objCountLineItemDetail.Comment;
                objDATA.LotNumber = objCountLineItemDetail.LotNumber;
                objDATA.SerialNumber = objCountLineItemDetail.SerialNumber;
                objDATA.Expiration = objCountLineItemDetail.Expiration;
                objDATA.Received = objCountLineItemDetail.Received;
                objDATA.ExpirationDate = objCountLineItemDetail.ExpirationDate;
                objDATA.ReceivedDate = objCountLineItemDetail.ReceivedDate;
                objDATA.Cost = objCountLineItemDetail.Cost;
                objDATA.GUID = (objCountLineItemDetail.GUID != null ? objCountLineItemDetail.GUID.Value : Guid.NewGuid());
                objDATA.Created = (objCountLineItemDetail.Created != null ? objCountLineItemDetail.Created.Value : DateTime.Now);
                objDATA.Updated = (objCountLineItemDetail.Updated != null ? objCountLineItemDetail.Updated.Value : DateTime.Now);
                objDATA.CreatedBy = objCountLineItemDetail.CreatedBy;
                objDATA.LastUpdatedBy = objCountLineItemDetail.LastUpdatedBy;
                objDATA.IsDeleted = objCountLineItemDetail.IsDeleted;
                objDATA.IsArchived = objCountLineItemDetail.IsArchived;
                objDATA.CompanyID = objCountLineItemDetail.CompanyID;
                objDATA.RoomID = objCountLineItemDetail.RoomID;
                objDATA.CountGUID = objCountLineItemDetail.CountGUID;
                objDATA.CountDetailGUID = objCountLineItemDetail.CountDetailGUID;
                objDATA.ReceivedOn = (objCountLineItemDetail.ReceivedOn != null ? objCountLineItemDetail.ReceivedOn.Value : DateTime.Now);
                objDATA.ReceivedOnWeb = (objCountLineItemDetail.ReceivedOnWeb != null ? objCountLineItemDetail.ReceivedOnWeb.Value : DateTime.Now);
                objDATA.AddedFrom = objCountLineItemDetail.AddedFrom;
                objDATA.EditedFrom = objCountLineItemDetail.EditedFrom;

                objDATA.CusOwnedDifference = CusOwnedDifference;
                objDATA.ConsignedDifference = ConsignedDifference;

                context.CountLineItemDetails.Add(objDATA);
                context.SaveChanges();
            }

            return true;
        }

        public bool UpdateCountLineItemDetail(CountLineItemDetailDTO objCountLineItemDetail)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double CusOwnedDifference = 0;
                double ConsignedDifference = 0;
                List<CountLineItemDetailDTO> lstCountLineItemDetail = new List<CountLineItemDetailDTO>();
                lstCountLineItemDetail.Add(objCountLineItemDetail);
                bool someflag = GetCustomerConsignDiff_CLD(lstCountLineItemDetail, objCountLineItemDetail.IsConsigned.GetValueOrDefault(false), objCountLineItemDetail.SerialNumberTracking.GetValueOrDefault(false), context, out CusOwnedDifference, out ConsignedDifference);


                CountLineItemDetail objDATA = (from A in context.CountLineItemDetails
                                               where A.GUID == objCountLineItemDetail.GUID
                                               select A).FirstOrDefault();

                if (objDATA != null)
                {
                    objDATA.BinID = objCountLineItemDetail.BinID;
                    objDATA.CustomerOwnedQuantity = objCountLineItemDetail.CustomerOwnedQuantity;
                    objDATA.CountCustomerOwnedQuantity = objCountLineItemDetail.CountCustomerOwnedQuantity;
                    objDATA.ConsignedQuantity = objCountLineItemDetail.ConsignedQuantity;
                    objDATA.CountConsignedQuantity = objCountLineItemDetail.CountConsignedQuantity;
                    objDATA.Comment = objCountLineItemDetail.Comment;
                    objDATA.LotNumber = objCountLineItemDetail.LotNumber;
                    objDATA.SerialNumber = objCountLineItemDetail.SerialNumber;
                    objDATA.Expiration = objCountLineItemDetail.Expiration;
                    objDATA.Received = objCountLineItemDetail.Received;
                    objDATA.ExpirationDate = objCountLineItemDetail.ExpirationDate;
                    objDATA.ReceivedDate = objCountLineItemDetail.ReceivedDate;
                    objDATA.Cost = objCountLineItemDetail.Cost;
                    objDATA.GUID = (objCountLineItemDetail.GUID != null ? objCountLineItemDetail.GUID.Value : Guid.NewGuid());
                    objDATA.Updated = (objCountLineItemDetail.Updated != null ? objCountLineItemDetail.Updated.Value : DateTime.Now);
                    objDATA.LastUpdatedBy = objCountLineItemDetail.LastUpdatedBy;
                    objDATA.IsDeleted = objCountLineItemDetail.IsDeleted;
                    objDATA.IsArchived = objCountLineItemDetail.IsArchived;
                    objDATA.CompanyID = objCountLineItemDetail.CompanyID;
                    objDATA.RoomID = objCountLineItemDetail.RoomID;
                    objDATA.CountGUID = objCountLineItemDetail.CountGUID;
                    objDATA.CountDetailGUID = objCountLineItemDetail.CountDetailGUID;
                    objDATA.ReceivedOn = (objCountLineItemDetail.ReceivedOn != null ? objCountLineItemDetail.ReceivedOn.Value : DateTime.Now);
                    objDATA.ReceivedOnWeb = (objCountLineItemDetail.ReceivedOnWeb != null ? objCountLineItemDetail.ReceivedOnWeb.Value : DateTime.Now);
                    objDATA.EditedFrom = objCountLineItemDetail.EditedFrom;
                    objDATA.CusOwnedDifference = CusOwnedDifference;
                    objDATA.ConsignedDifference = ConsignedDifference;
                    context.SaveChanges();
                }
            }

            return true;
        }

        public bool DeleteCountLineItemDetail(Guid CountLineItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CountLineItemDetail objDATA = (from A in context.CountLineItemDetails
                                               where A.GUID == CountLineItemGuid
                                               select A).FirstOrDefault();

                if (objDATA != null)
                {
                    objDATA.IsDeleted = true;
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool UpdateCountInInventoryCountDetails(Guid? CountDetailGUID, double? CustomerOwnedQuantity, double? ConsignedQuantity, List<CountLineItemDetailDTO> lstCountLineItemDetail = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail objDATA = (from A in context.InventoryCountDetails
                                                where A.GUID == CountDetailGUID
                                                select A).FirstOrDefault();

                if (objDATA != null)
                {
                    //-----------------------------------------------------------------
                    //
                    double ItemLocationCoownedQty = 0;
                    double ItemLocationConsignedQty = 0;

                    if (objDATA.IsStagingLocationCount)
                    {
                        var q = context.MaterialStagingPullDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objDATA.ItemGUID && t.StagingBinId == objDATA.BinID);
                        if (q.Any())
                        {
                            ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                            ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                        }
                    }
                    else
                    {
                        var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objDATA.ItemGUID && t.BinID == objDATA.BinID);
                        if (q.Any())
                        {
                            ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity ?? 0));
                            ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                        }
                    }

                    //-----------------------------------------------------------------
                    //
                    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO objItem = objItemDAL.GetItemWithoutJoins(null, objDATA.ItemGUID);

                    //-----------------------------------------------------------------
                    //
                    objDATA.CountCustOwnedQuantity = (CustomerOwnedQuantity == null ? 0 : CustomerOwnedQuantity.Value);
                    objDATA.CountConsignedQuantity = (ConsignedQuantity == null ? 0 : ConsignedQuantity.Value);

                    //----------------------------------------------------------------------------------
                    //
                    double CusOwnedDifference = 0;
                    double ConsignedDifference = 0;
                    if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0
                        && GetCustomerConsignDiff(lstCountLineItemDetail, (objItem == null ? false : objItem.Consignment), (objItem == null ? false : objItem.SerialNumberTracking), context, out CusOwnedDifference, out ConsignedDifference))
                    {
                        objDATA.CusOwnedDifference = CusOwnedDifference;
                        if (objItem != null && objItem.Consignment)
                        {
                            objDATA.ConsignedDifference = ConsignedDifference;
                        }
                        else
                        {
                            objDATA.ConsignedDifference = ConsignedDifference;
                        }
                    }
                    else
                    {
                        objDATA.CusOwnedDifference = CustomerOwnedQuantity.HasValue ? ((CustomerOwnedQuantity ?? 0) - ItemLocationCoownedQty) : (-0.000000001);
                        if (objItem != null && objItem.Consignment)
                        {

                            objDATA.ConsignedDifference = ConsignedQuantity.HasValue ? ((ConsignedQuantity ?? 0) - ItemLocationConsignedQty) : (-0.000000001);
                        }
                        else
                        {
                            objDATA.ConsignedDifference = (-0.000000001);
                        }
                    }

                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool UpdateCountInInventoryCountDetailsByCountLineItemGuid(Guid CountLineItemGuid, long UserID, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Guid? CountDetailGUID = (from CLI in context.CountLineItemDetails
                                         where CLI.GUID == CountLineItemGuid
                                         select CLI.CountDetailGUID).FirstOrDefault();

                InventoryCountDetail objDATA = (from A in context.InventoryCountDetails
                                                where A.GUID == CountDetailGUID
                                                select A).FirstOrDefault();

                if (objDATA != null)
                {
                    //------------------------------------------------
                    //
                    double ItemLocationCoownedQty = 0;
                    double ItemLocationConsignedQty = 0;

                    if (objDATA.IsStagingLocationCount)
                    {
                        var q = context.MaterialStagingPullDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objDATA.ItemGUID && t.StagingBinId == objDATA.BinID);
                        if (q.Any())
                        {
                            ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity == null || t.ConsignedQuantity < 0 ? 0 : (double)t.ConsignedQuantity));
                            ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity == null || t.CustomerOwnedQuantity < 0 ? 0 : (double)t.CustomerOwnedQuantity));
                        }
                    }
                    else
                    {
                        var q = context.ItemLocationDetails.Where(t => (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false && t.ItemGUID == objDATA.ItemGUID && t.BinID == objDATA.BinID);
                        if (q.Any())
                        {
                            ItemLocationConsignedQty = q.Sum(t => (t.ConsignedQuantity == null || t.ConsignedQuantity < 0 ? 0 : (double)t.ConsignedQuantity));
                            ItemLocationCoownedQty = q.Sum(t => (t.CustomerOwnedQuantity == null || t.CustomerOwnedQuantity < 0 ? 0 : (double)t.CustomerOwnedQuantity));
                        }
                    }

                    //------------------------------------------------
                    //
                    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO objItem = objItemDAL.GetItemWithoutJoins(null, objDATA.ItemGUID);

                    //------------------------------------------------
                    //
                    List<CountLineItemDetailDTO> lstInventoryCountDetail = (from CLI in context.CountLineItemDetails
                                                                            where CLI.CountDetailGUID == CountDetailGUID
                                                                                  && (CLI.IsDeleted == null || CLI.IsDeleted == false)
                                                                                  && (CLI.IsArchived == null || CLI.IsArchived == false)
                                                                            select new CountLineItemDetailDTO()
                                                                            {
                                                                                ID = CLI.ID,
                                                                                ItemGUID = CLI.ItemGUID,
                                                                                BinID = CLI.BinID,
                                                                                Comment = CLI.Comment,
                                                                                CustomerOwnedQuantity = CLI.CustomerOwnedQuantity,
                                                                                CountCustomerOwnedQuantity = CLI.CountCustomerOwnedQuantity,
                                                                                ConsignedQuantity = CLI.ConsignedQuantity,
                                                                                CountConsignedQuantity = CLI.CountConsignedQuantity,
                                                                                LotNumber = CLI.LotNumber,
                                                                                SerialNumber = CLI.SerialNumber,
                                                                                LotSerialNumber = (CLI.LotNumber != null && CLI.LotNumber.Trim() != "" ? CLI.LotNumber :
                                                                                                   (CLI.SerialNumber != null && CLI.SerialNumber.Trim() != "" ? CLI.SerialNumber.Trim() : "")),
                                                                                Expiration = CLI.Expiration,
                                                                                Received = CLI.Received,
                                                                                ExpirationDate = CLI.ExpirationDate,
                                                                                ReceivedDate = CLI.ReceivedDate,
                                                                                Cost = CLI.Cost,
                                                                                GUID = CLI.GUID,
                                                                                IsDeleted = CLI.IsDeleted,
                                                                                IsArchived = CLI.IsArchived,
                                                                                CompanyID = CLI.CompanyID,
                                                                                RoomID = CLI.RoomID,
                                                                                CountGUID = CLI.CountGUID,
                                                                                CountDetailGUID = CLI.CountDetailGUID,
                                                                            }).ToList();

                    //------------------------------------------------
                    //
                    double CusOwnedDifference = 0;
                    double ConsignedDifference = 0;
                    if (lstInventoryCountDetail != null && lstInventoryCountDetail.Count > 0
                        && GetCustomerConsignDiff(lstInventoryCountDetail, (objItem == null ? false : objItem.Consignment), (objItem == null ? false : objItem.SerialNumberTracking), context, out CusOwnedDifference, out ConsignedDifference))
                    {
                        double CustomerOwnedQuantity = lstInventoryCountDetail.Sum(x => (x.CountCustomerOwnedQuantity == null || x.CountCustomerOwnedQuantity < 0 ? 0 : x.CountCustomerOwnedQuantity.Value));
                        double ConsignedQuantity = lstInventoryCountDetail.Sum(x => (x.CountConsignedQuantity == null || x.CountConsignedQuantity < 0 ? 0 : x.CountConsignedQuantity.Value));

                        objDATA.CountCustOwnedQuantity = CustomerOwnedQuantity;
                        objDATA.CountConsignedQuantity = ConsignedQuantity;

                        objDATA.CusOwnedDifference = CusOwnedDifference;
                        if (objItem != null && objItem.Consignment)
                        {
                            objDATA.ConsignedDifference = ConsignedDifference;
                        }
                        else
                        {
                            objDATA.ConsignedDifference = ConsignedDifference;
                        }
                    }
                    else
                    {
                        double CustomerOwnedQuantity = ((lstInventoryCountDetail != null && lstInventoryCountDetail.Count > 0) ?
                                                            lstInventoryCountDetail.Sum(x => (x.CountCustomerOwnedQuantity == null || x.CountCustomerOwnedQuantity < 0 ? 0 : x.CountCustomerOwnedQuantity.Value)) : 0);
                        double ConsignedQuantity = ((lstInventoryCountDetail != null && lstInventoryCountDetail.Count > 0) ?
                                                            lstInventoryCountDetail.Sum(x => (x.CountConsignedQuantity == null || x.CountConsignedQuantity < 0 ? 0 : x.CountConsignedQuantity.Value)) : 0);

                        objDATA.CountCustOwnedQuantity = CustomerOwnedQuantity;
                        objDATA.CountConsignedQuantity = ConsignedQuantity;

                        objDATA.CusOwnedDifference = objDATA.CountCustOwnedQuantity >= 0 ? (objDATA.CountCustOwnedQuantity - ItemLocationCoownedQty) : (-0.000000001);
                        if (objItem != null && objItem.Consignment)
                        {

                            objDATA.ConsignedDifference = objDATA.CountConsignedQuantity >= 0 ? (objDATA.CountConsignedQuantity - ItemLocationConsignedQty) : (-0.000000001);
                        }
                        else
                        {
                            objDATA.ConsignedDifference = (-0.000000001);
                        }
                    }

                    context.SaveChanges();
                }
                else
                {
                    DeleteCountLineItems(CountDetailGUID.ToString(), UserID, CompanyId);
                }
            }
            return true;
        }

        public List<CountLineItemDetailDTO> GetCountLineItemDetailList(Guid InventoryCountGUID, string RoomDateFormat)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<CountLineItemDetailDTO> lstCountLineItemDetail = (from CLD in context.CountLineItemDetails.AsEnumerable().Select(x =>
                                                                        {
                                                                            x.Expiration = (x.ExpirationDate != null ? x.ExpirationDate.Value.ToString(RoomDateFormat) : "");
                                                                            return x;
                                                                        })
                                                                       where CLD.CountDetailGUID == InventoryCountGUID
                                                                                 && (CLD.IsDeleted == null || CLD.IsDeleted == false)
                                                                                 && (CLD.IsArchived == null || CLD.IsArchived == false)

                                                                       select new CountLineItemDetailDTO()
                                                                       {
                                                                           ID = CLD.ID,
                                                                           ItemGUID = CLD.ItemGUID,
                                                                           BinID = CLD.BinID,
                                                                           Comment = CLD.Comment,
                                                                           CustomerOwnedQuantity = CLD.CustomerOwnedQuantity,
                                                                           CountCustomerOwnedQuantity = CLD.CountCustomerOwnedQuantity,
                                                                           ConsignedQuantity = CLD.ConsignedQuantity,
                                                                           CountConsignedQuantity = CLD.CountConsignedQuantity,
                                                                           LotNumber = CLD.LotNumber,
                                                                           SerialNumber = CLD.SerialNumber,
                                                                           LotSerialNumber = (CLD.LotNumber != null && CLD.LotNumber.Trim() != "" ? CLD.LotNumber :
                                                                                                (CLD.SerialNumber != null && CLD.SerialNumber.Trim() != "" ? CLD.SerialNumber.Trim() : "")),
                                                                           Expiration = CLD.Expiration,
                                                                           Received = CLD.Received,
                                                                           ExpirationDate = CLD.ExpirationDate,
                                                                           ReceivedDate = CLD.ReceivedDate,
                                                                           Cost = CLD.Cost,
                                                                           GUID = CLD.GUID,
                                                                           Created = CLD.Created,
                                                                           Updated = CLD.Updated,
                                                                           CreatedBy = CLD.CreatedBy,
                                                                           LastUpdatedBy = CLD.LastUpdatedBy,
                                                                           IsDeleted = CLD.IsDeleted,
                                                                           IsArchived = CLD.IsArchived,
                                                                           CompanyID = CLD.CompanyID,
                                                                           RoomID = CLD.RoomID,
                                                                           CountGUID = CLD.CountGUID,
                                                                           CountDetailGUID = CLD.CountDetailGUID,
                                                                           ReceivedOn = CLD.ReceivedOn,
                                                                           ReceivedOnWeb = CLD.ReceivedOnWeb,
                                                                           AddedFrom = CLD.AddedFrom,
                                                                           EditedFrom = CLD.EditedFrom,
                                                                           ConsignedDifference = CLD.ConsignedDifference,
                                                                           CusOwnedDifference = CLD.CusOwnedDifference
                                                                       }
                                                                      ).ToList();

                return lstCountLineItemDetail;
            }
        }

        public List<CountLineItemDetailDTO> GetCountLineItemDetailList(Guid InventoryCountGUID, string RoomDateFormat, bool LotNumberTracking, bool SerialNumberTracking, bool DateCodeTracking, bool IsApplied, string BinNumber, string Description, bool IsStage = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<CountLineItemDetailDTO> lstCountLineItemDetail = (from CLD in context.CountLineItemDetails.AsEnumerable().Select(x =>
                                                                                                                                            {
                                                                                                                                                x.Expiration = (x.ExpirationDate != null ? x.ExpirationDate.Value.ToString(RoomDateFormat) : "");
                                                                                                                                                return x;
                                                                                                                                            }
                                                                                                                                      )
                                                                       where CLD.CountDetailGUID == InventoryCountGUID
                                                                                 && (CLD.IsDeleted == null || CLD.IsDeleted == false)
                                                                                 && (CLD.IsArchived == null || CLD.IsArchived == false)

                                                                       select new CountLineItemDetailDTO()
                                                                       {
                                                                           ID = CLD.ID,
                                                                           ItemGUID = CLD.ItemGUID,
                                                                           BinID = CLD.BinID,
                                                                           BinNumber = BinNumber,
                                                                           Comment = CLD.Comment,
                                                                           CustomerOwnedQuantity = CLD.CustomerOwnedQuantity,
                                                                           CountCustomerOwnedQuantity = CLD.CountCustomerOwnedQuantity,
                                                                           ConsignedQuantity = CLD.ConsignedQuantity,
                                                                           CountConsignedQuantity = CLD.CountConsignedQuantity,
                                                                           LotNumber = CLD.LotNumber,
                                                                           SerialNumber = CLD.SerialNumber,
                                                                           //LotSerialNumber = ((CLD.LotNumber != null && CLD.LotNumber.Trim() != "") ? CLD.LotNumber : CLD.SerialNumber) +
                                                                           // (DateCodeTracking == true && CLD.ExpirationDate != null ? "-" + CLD.ExpirationDate.Value.ToString("MM/dd/yy") : ""),
                                                                           LotSerialNumber = ((CLD.LotNumber != null && CLD.LotNumber.Trim() != "") ? CLD.LotNumber : CLD.SerialNumber),
                                                                           Expiration = CLD.Expiration,
                                                                           Received = CLD.Received,
                                                                           ExpirationDate = CLD.ExpirationDate,
                                                                           ReceivedDate = CLD.ReceivedDate,
                                                                           Cost = CLD.Cost,
                                                                           GUID = CLD.GUID,
                                                                           Created = CLD.Created,
                                                                           Updated = CLD.Updated,
                                                                           CreatedBy = CLD.CreatedBy,
                                                                           LastUpdatedBy = CLD.LastUpdatedBy,
                                                                           IsDeleted = CLD.IsDeleted,
                                                                           IsArchived = CLD.IsArchived,
                                                                           CompanyID = CLD.CompanyID,
                                                                           RoomID = CLD.RoomID,
                                                                           CountGUID = CLD.CountGUID,
                                                                           CountDetailGUID = CLD.CountDetailGUID,
                                                                           ReceivedOn = CLD.ReceivedOn,
                                                                           ReceivedOnWeb = CLD.ReceivedOnWeb,
                                                                           AddedFrom = CLD.AddedFrom,
                                                                           EditedFrom = CLD.EditedFrom,
                                                                           IsApplied = IsApplied,
                                                                           SerialNumberTracking = SerialNumberTracking,
                                                                           LotNumberTracking = LotNumberTracking,
                                                                           DateCodeTracking = DateCodeTracking,
                                                                           InventoryCountDetailDescription = Description,
                                                                           CusOwnedDifference = CLD.CusOwnedDifference,
                                                                           ConsignedDifference = CLD.ConsignedDifference,
                                                                           IsStagingLocationCount = IsStage
                                                                       }
                                                                      ).ToList();

                return lstCountLineItemDetail;
            }
        }

        public bool UpdateCountLineItemDetailQty(Guid CountDetailGUID, double? CountCustomerOwnedQuantity, double? CountConsignedQuantity, double? CusOwnedDifference, double? ConsignedDifference)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    CountLineItemDetail objCountLineItemDetail = (from A in context.CountLineItemDetails
                                                                  where A.CountDetailGUID == CountDetailGUID
                                                                  select A).FirstOrDefault();
                    if (objCountLineItemDetail != null)
                    {
                        objCountLineItemDetail.CountCustomerOwnedQuantity = CountCustomerOwnedQuantity;
                        objCountLineItemDetail.CountConsignedQuantity = CountConsignedQuantity;
                        objCountLineItemDetail.ConsignedDifference = ConsignedDifference;
                        objCountLineItemDetail.CusOwnedDifference = CusOwnedDifference;
                    }
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public List<string[]> GetCountLineItemListUsingItemGuid(Guid ItemGUID, string RoomDateFormat, Int64 BinId)
        {
            List<string[]> lstReturn = new List<string[]>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //----------------------------------------------------------------------
                //
                var lstCountLineItemDetail = (from CLD in context.CountLineItemDetails
                                              join ICD in context.InventoryCountDetails on CLD.CountDetailGUID equals ICD.GUID
                                              join IC in context.InventoryCounts on CLD.CountGUID equals IC.GUID
                                              join BM in context.BinMasters on CLD.BinID equals BM.ID
                                              where CLD.ItemGUID == ItemGUID
                                              && CLD.BinID != BinId
                                                        && (CLD.IsDeleted == null || CLD.IsDeleted == false)
                                                        && (CLD.IsArchived == null || CLD.IsArchived == false)
                                                        && ICD.IsDeleted == false && ICD.IsArchived == false
                                                        && IC.IsDeleted == false && IC.IsArchived == false
                                                        && CLD.IsDeleted == false
                                                        && (BM.IsArchived == null || BM.IsArchived == false)
                                                        && (ICD.IsApplied == false)
                                              select new
                                              {
                                                  SerialNumber = CLD.SerialNumber,
                                                  BinId = (CLD.BinID),
                                                  CountBin = IC.CountName + "~" + BM.BinNumber
                                              }).ToList();

                if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count() > 0)
                {
                    foreach (var objCountLineItemDetail in lstCountLineItemDetail)
                    {
                        lstReturn.Add(new string[] { objCountLineItemDetail.SerialNumber + "###" + Convert.ToString(objCountLineItemDetail.BinId), objCountLineItemDetail.CountBin });
                    }
                }

                //----------------------------------------------------------------------
                //
                var lstLocationDetail = (from CLD in context.ItemLocationDetails
                                         where CLD.ItemGUID == ItemGUID
                                               && (CLD.IsDeleted == null || CLD.IsDeleted == false)
                                               && (CLD.IsArchived == null || CLD.IsArchived == false)
                                               && (CLD.ConsignedQuantity != 0 || CLD.CustomerOwnedQuantity != 0)
                                         && CLD.BinID != BinId
                                                   && (CLD.IsDeleted == null || CLD.IsDeleted == false)
                                                   && (CLD.IsArchived == null || CLD.IsArchived == false)
                                         select new
                                         {
                                             SerialNumber = CLD.SerialNumber,
                                             BinId = CLD.BinID
                                         }).ToList();

                if (lstLocationDetail != null && lstLocationDetail.Count() > 0)
                {
                    foreach (var objLocationDetail in lstLocationDetail)
                    {
                        lstReturn.Add(new string[] { objLocationDetail.SerialNumber + "###" + Convert.ToString(objLocationDetail.BinId), "" });
                    }
                }

                return lstReturn;
            }
        }

        public InventoryCountDTO GetCountByIDGUID(long? ID, Guid? GUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", (ID ?? (object)DBNull.Value)), new SqlParameter("@GUID", GUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("exec [GetCountByIDGUID] @ID,@GUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<InventoryCountDTO> GetCountsByCountName(string CountName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CountName", (CountName ?? (object)DBNull.Value)), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("exec [GetCountByCountName] @CountName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<InventoryCountDTO> GetCountsByNamesIDs(string CountNames, string CountIDs, string CountGUIDs, string RoomIDs, string CompanyIDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CountNames", (CountNames ?? (object)DBNull.Value)), new SqlParameter("@CountIDs", (CountIDs ?? (object)DBNull.Value)), new SqlParameter("@CountGUIDs", (CountGUIDs ?? (object)DBNull.Value)), new SqlParameter("@RoomIDs", (RoomIDs ?? (object)DBNull.Value)), new SqlParameter("@CompanyIDs", (CompanyIDs ?? (object)DBNull.Value)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("exec [GetCountsByNamesIDs] @CountNames,@CountIDs,@CountGUIDs,@RoomIDs,@CompanyIDs", params1).ToList();
            }
        }
        public List<InventoryCountDetailDTO> GetCountDetailsRecord(Guid CountGUID, Guid ItemGUID, long BinID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CountGUID", (CountGUID)), new SqlParameter("@ItemGUID", (ItemGUID)), new SqlParameter("@BinID", BinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDetailDTO>("exec [GetCountDetailsRecords] @CountGUID,@ItemGUID,@BinID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public bool CheckGuidExistsOrNot(Guid CountGuid)
        {
            Int32 ret = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ret = (from u in context.InventoryCounts
                       where u.GUID == CountGuid
                       select u
                        ).ToList().Count();
            }
            if (ret > 0)
            {
                return true;
            }
            return false;
        }
        public Int32 GetAppliedCountDetailscnt(Int64 RoomID, Int64 CompanyID, Guid CountGUID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    List<InventoryCountDetail> CountDetail = (from ci in context.InventoryCountDetails
                                                              join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                                              from ci_cc in ci_cc_join.DefaultIfEmpty()
                                                              join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                                              from ci_cu in ci_cu_join.DefaultIfEmpty()
                                                              join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                                                              from ci_rm in ci_rm_join.DefaultIfEmpty()
                                                              where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.InventoryCountGUID == CountGUID
                                                              select ci
                                                   ).ToList();
                    if (CountDetail != null && CountDetail.Where(cd => cd.IsApplied == true).Count() > 0)
                    {
                        return CountDetail.Where(cd => cd.IsApplied == true).Count();
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// This method is used to get the max release number based on CompanyID,RoomID, and RequistionNumber
        /// </summary>
        /// <param name="RequistionNumber"></param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <returns></returns>
        public double GetInventoryCountReleaseNumber(long id, string countName, long companyId, long roomId)
        {
            double releaseNumber = 1;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var releaseNumberStr = context.InventoryCounts.Where(u => !string.IsNullOrEmpty(u.ReleaseNumber) && u.RoomId == roomId && u.CompanyId == companyId
                                                   && u.CountName == countName && (id < 1 || (id >= 1 && u.ID != id)) && u.IsDeleted == false && u.IsArchived == false).ToList().OrderByDescending(u => Convert.ToInt32(u.ReleaseNumber)).FirstOrDefault();

                if (releaseNumberStr != null && !string.IsNullOrEmpty(releaseNumberStr.ReleaseNumber))
                {
                    releaseNumber = Convert.ToInt32(releaseNumberStr.ReleaseNumber) + 1;
                }
            }
            return releaseNumber;
        }

        public void SetInventoryCountReleaseNumber(InventoryCountDTO objInventoryCountDTO)
        {
            var releaseNumber = GetInventoryCountReleaseNumber(objInventoryCountDTO.ID, objInventoryCountDTO.CountName, objInventoryCountDTO.CompanyId, objInventoryCountDTO.RoomId);
            objInventoryCountDTO.ReleaseNumber = Convert.ToString(releaseNumber);
        }

        #region WI-4991

        public List<InventoryCountDetail> GetCountDetailGUIDByCountGuid(Guid CountGUID, Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<InventoryCountDetail> objInventoryCountDetail = (from A in context.InventoryCountDetails
                                                                      where A.InventoryCountGUID == CountGUID && A.ItemGUID == ItemGUID
                                                                            && (A.IsDeleted == false)
                                                                            && (A.IsArchived == false)
                                                                      // && A.IsApplied == false
                                                                      select A).ToList();
                return objInventoryCountDetail;
            }
        }

        public InventoryCountDTO SaveInventoryCountForImport(InventoryCountDTO objInventoryCountDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool projectChanged = false;
                InventoryCount objInventoryCount = null;

                if (objInventoryCountDTO.ID > 0)
                {
                    objInventoryCount = context.InventoryCounts.FirstOrDefault(t => t.ID == objInventoryCountDTO.ID);

                    if (objInventoryCount != null)
                    {
                        objInventoryCount.CountName = objInventoryCountDTO.CountName;
                        objInventoryCount.CountItemDescription = objInventoryCountDTO.CountItemDescription;
                        objInventoryCount.CountDate = objInventoryCountDTO.CountDate;
                        objInventoryCount.UDF1 = objInventoryCountDTO.UDF1;
                        objInventoryCount.UDF2 = objInventoryCountDTO.UDF2;
                        objInventoryCount.UDF3 = objInventoryCountDTO.UDF3;
                        objInventoryCount.UDF4 = objInventoryCountDTO.UDF4;
                        objInventoryCount.UDF5 = objInventoryCountDTO.UDF5;
                        objInventoryCount.LastUpdatedBy = objInventoryCountDTO.LastUpdatedBy;
                        objInventoryCount.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCount.AddedFrom = objInventoryCountDTO.AddedFrom == null ? (objInventoryCount.AddedFrom != null ? objInventoryCount.AddedFrom : "Web") : objInventoryCountDTO.AddedFrom;
                        objInventoryCount.EditedFrom = objInventoryCountDTO.EditedFrom ?? "Web";
                        objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objInventoryCount.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        if (objInventoryCountDTO != null && objInventoryCountDTO.CountType != null && (!string.IsNullOrWhiteSpace(objInventoryCountDTO.CountType)))
                            objInventoryCount.CountType = objInventoryCountDTO.CountType;

                        if (objInventoryCount.ProjectSpendGUID != objInventoryCountDTO.ProjectSpendGUID)
                        {
                            projectChanged = true;
                        }

                        objInventoryCount.ProjectSpendGUID = objInventoryCountDTO.ProjectSpendGUID;
                        objInventoryCountDTO.RoomId = objInventoryCount.RoomId;
                        objInventoryCountDTO.CompanyId = objInventoryCount.CompanyId;
                        objInventoryCountDTO.CompleteCauseCountGUID = objInventoryCount.CompleteCauseCountGUID;
                        objInventoryCountDTO.CountCompletionDate = objInventoryCount.CountCompletionDate;
                        objInventoryCountDTO.CountDate = objInventoryCount.CountDate;
                        objInventoryCountDTO.CountStatus = objInventoryCount.CountStatus;
                        objInventoryCountDTO.Created = objInventoryCount.Created;
                        objInventoryCountDTO.CreatedBy = objInventoryCount.CreatedBy;
                        objInventoryCountDTO.GUID = objInventoryCount.GUID;
                        objInventoryCountDTO.IsArchived = objInventoryCount.IsArchived;
                        objInventoryCountDTO.IsAutomatedCompletion = objInventoryCount.IsAutomatedCompletion;
                        objInventoryCountDTO.IsDeleted = objInventoryCount.IsDeleted;
                        objInventoryCountDTO.LastUpdatedBy = objInventoryCount.LastUpdatedBy;
                        objInventoryCountDTO.Updated = objInventoryCount.Updated;
                        objInventoryCountDTO.Year = objInventoryCount.Year;
                        objInventoryCountDTO.AddedFrom = objInventoryCountDTO.AddedFrom ?? "Web";
                        objInventoryCountDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        SetInventoryCountReleaseNumber(objInventoryCountDTO);

                        objInventoryCount.ReleaseNumber = objInventoryCountDTO.ReleaseNumber;

                        if (objInventoryCountDTO.IsOnlyFromItemUI)
                        {
                            objInventoryCount.EditedFrom = "Web";
                            objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        if (projectChanged)
                        {
                            IQueryable<InventoryCountDetail> lst = context.InventoryCountDetails.Where(w => w.InventoryCountGUID == objInventoryCount.GUID && w.IsDeleted == false);
                            if (lst.Any())
                            {
                                foreach (var item in lst)
                                {
                                    item.ProjectSpendGUID = objInventoryCount.ProjectSpendGUID;
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                }
                else
                {
                    SetInventoryCountReleaseNumber(objInventoryCountDTO);
                    objInventoryCount = new InventoryCount();
                    objInventoryCount.ID = 0;
                    objInventoryCount.GUID = objInventoryCountDTO.GUID;
                    objInventoryCount.CountName = objInventoryCountDTO.CountName;
                    objInventoryCount.CountItemDescription = objInventoryCountDTO.CountItemDescription;
                    objInventoryCount.CountType = objInventoryCountDTO.CountType;
                    objInventoryCount.CountStatus = objInventoryCountDTO.CountStatus;
                    objInventoryCount.UDF1 = objInventoryCountDTO.UDF1;
                    objInventoryCount.UDF2 = objInventoryCountDTO.UDF2;
                    objInventoryCount.UDF3 = objInventoryCountDTO.UDF3;
                    objInventoryCount.UDF4 = objInventoryCountDTO.UDF4;
                    objInventoryCount.UDF5 = objInventoryCountDTO.UDF5;
                    objInventoryCount.Created = objInventoryCountDTO.Created; ;
                    objInventoryCount.Updated = objInventoryCountDTO.Updated;
                    objInventoryCount.CreatedBy = objInventoryCountDTO.CreatedBy;
                    objInventoryCount.LastUpdatedBy = objInventoryCountDTO.LastUpdatedBy;
                    objInventoryCount.IsDeleted = objInventoryCountDTO.IsDeleted;
                    objInventoryCount.IsArchived = objInventoryCountDTO.IsArchived;
                    objInventoryCount.Year = objInventoryCountDTO.Year;
                    objInventoryCount.CompanyId = objInventoryCountDTO.CompanyId;
                    objInventoryCount.RoomId = objInventoryCountDTO.RoomId;
                    objInventoryCount.CountDate = objInventoryCountDTO.CountDate;
                    objInventoryCount.CountCompletionDate = objInventoryCountDTO.CountCompletionDate;
                    objInventoryCount.IsAutomatedCompletion = objInventoryCountDTO.IsAutomatedCompletion;
                    objInventoryCount.CompleteCauseCountGUID = objInventoryCountDTO.CompleteCauseCountGUID;
                    objInventoryCount.AddedFrom = objInventoryCountDTO.AddedFrom ?? "Web";
                    objInventoryCount.EditedFrom = objInventoryCountDTO.EditedFrom ?? "Web";
                    objInventoryCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objInventoryCount.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objInventoryCount.ProjectSpendGUID = objInventoryCountDTO.ProjectSpendGUID;
                    objInventoryCount.ReleaseNumber = objInventoryCountDTO.ReleaseNumber;
                    context.InventoryCounts.Add(objInventoryCount);
                    context.SaveChanges();
                    objInventoryCountDTO.ID = objInventoryCount.ID;
                    new AutoSequenceDAL(base.DataBaseName).UpdateNextCountNumberForImport(objInventoryCountDTO.RoomId, objInventoryCountDTO.CompanyId, objInventoryCountDTO.CountName);
                }
            }
            return objInventoryCountDTO;
        }


        #endregion

        public bool GetCountLineItemDetailByGUID(Guid CntLineItemDtlGUID, long CompanyId, long RoomId)
        {
            bool IsExist = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CountLineItemDetail objInventoryCountDetail = (from A in context.CountLineItemDetails
                                                               where A.GUID == CntLineItemDtlGUID
                                                               && A.CompanyID == CompanyId && A.RoomID == RoomId
                                                               select A).FirstOrDefault();

                if (objInventoryCountDetail != null)
                {
                    IsExist = true;
                }
                return IsExist;
            }
        }
        public List<InventoryCountDTO> GetInventoryCountChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("exec [GetInventoryCountChangeLog] @ID,@dbName", params1).ToList();
            }
        }

        public SaveEVMIInvCountDetailDTO SaveEVMIInventoryCountDetail(EVMIInvCountDetailDTO dto)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@ItemGUID", dto.ItemGUID),
                new SqlParameter("@BinGUID", dto.BinGUID),
                new SqlParameter("@NewQuantity", dto.NewQuantity),
                new SqlParameter("@BinID", dto.BinID),
                new SqlParameter("@RoomId", dto.RoomId),
                new SqlParameter("@CompanyId", dto.CompanyId),
                new SqlParameter("@CreatedBy", dto.CreatedBy),
                new SqlParameter("@IsConsignment", dto.IsConsignment),
                new SqlParameter("@ItemCost", dto.ItemCost),
                new SqlParameter("@ItemPrice", dto.ItemPrice),
                new SqlParameter("@CountGUID", dto.CountGUID ?? (object)DBNull.Value)

            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var ret = context.Database.SqlQuery<SaveEVMIInvCountDetailDTO>(
                    "exec [SaveEVMIInventoryCountDetail] @ItemGUID,@BinGUID,@NewQuantity,@BinID,@RoomId,@CompanyId,@CreatedBy,@IsConsignment,@ItemCost,@ItemPrice,@CountGUID", params1)
                    .FirstOrDefault();
                return ret;
            }
        }

        public ApplyCountOnLineItemRespDTO ApplyCountOnLineItemsNew(long RoomID, long CompanyID, long UserID,
            List<InventoryCountDetailDTO> lstLineItems, string RoomDateFormat, bool IsFromWeb, long EnterpriseId)
        {
            string EnterPriseDBName = base.DataBaseName;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            ItemMasterDTO objItemDTO;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(EnterPriseDBName);
            double CustomerOwnedQty = 0;
            bool IsValidInvalidQuantity = true;
            double ConsignedOwnedQty = 0;
            string message = string.Empty, status = string.Empty;
            int InvalidQuantity = 0;
            int TotalItems = 0;
            bool _IsCountClosed = false;

            // RoomDTO objRoomDTO = new RoomDAL(EnterPriseDBName).GetRoomByIDPlain(RoomID);
            string columnList = "ID,RoomName,IsIgnoreCreditRule";
            CommonDAL objCommonDAL = new CommonDAL(EnterPriseDBName);
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            string CultureCode = "en-US";
            var regionalSettingDAL = new RegionSettingDAL(EnterPriseDBName);
            var currentCulture = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            if (currentCulture != null)
            {
                CultureCode = currentCulture.CultureCode;
            }

            long SessionUserId = UserID;
            var inventoryCountResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResInventoryCount", CultureCode, EnterpriseId, CompanyID);
            var messageResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", CultureCode, EnterpriseId, CompanyID);
            string SaveMessage = ResourceRead.GetResourceValueByKeyAndFullFilePath("SaveMessage", messageResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResMessage", CultureCode);
            string SaveErrorMsg = ResourceRead.GetResourceValueByKeyAndFullFilePath("SaveErrorMsg", messageResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResMessage", CultureCode);
            string SaveError = ResourceRead.GetResourceValueByKeyAndFullFilePath("SaveError", inventoryCountResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResInventoryCount", CultureCode);

            if (lstLineItems != null && lstLineItems.Count > 0)
            {
                try
                {

                    var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                    string msgProvideValidCustOrConsQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("ProvideValidCustOrConsQty", inventoryCountResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                    string msgCantApplyCountNotEnoughPullForCredit = ResourceRead.GetResourceValueByKeyAndFullFilePath("CantApplyCountNotEnoughPullForCredit", inventoryCountResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                    string msgCantApplyCountTotalQtyNotMatchedWithDetailQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("CantApplyCountTotalQtyNotMatchedWithDetailQty", inventoryCountResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                    string msgCantApplyCountForSomeItems = ResourceRead.GetResourceValueByKeyAndFullFilePath("CantApplyCountForSomeItems", inventoryCountResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                    string SaveSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("SaveSuccess", inventoryCountResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResInventoryCount", CultureCode);
                    string msgCreditTransactionForSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCreditTransactionForSerialNumber", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                    string msgCreditTransactionForLotAndExpDateAvailable = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditTransactionForLotAndExpDateAvailable", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);


                    foreach (InventoryCountDetailDTO InventoryCount in lstLineItems)
                    {
                        if (InventoryCount.SerialNumberTracking == false && InventoryCount.LotNumberTracking == false
                            && InventoryCount.CountCustomerOwnedQuantity == null
                            && (InventoryCount.Consignment == true && InventoryCount.CountConsignedQuantity == null))
                        {
                            return new ApplyCountOnLineItemRespDTO() { Message = msgProvideValidCustOrConsQty, Status = "fail", IsCountClosed = false };
                            //return Json(new { Message = "Please provide valid value for customer owned quantity Or consigned quantity", Status = "fail", IsCountClosed = false });
                            ////if (InventoryCount.CountCustomerOwnedQuantity == null)
                            ////{
                            ////    return Json(new { Message = "Please provide valid value for customer owned quantity", Status = "fail", IsCountClosed = false });
                            ////}
                            ////else if (InventoryCount.Consignment == true && InventoryCount.CountConsignedQuantity == null)
                            ////{
                            ////    return Json(new { Message = "Please provide valid value for consigned quantity", Status = "fail", IsCountClosed = false });
                            ////}
                        }
                    }

                    ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(EnterPriseDBName);
                    foreach (InventoryCountDetailDTO InventoryCount in lstLineItems)
                    {
                        objItemDTO = objItemDAL.GetItemWithoutJoins(null, InventoryCount.ItemGUID);
                        if (InventoryCount.SerialNumberTracking == true)
                        {
                            AddMissingLotSerial(InventoryCount.CountDetailGUID, InventoryCount.ItemGUID, RoomDateFormat, UserID, CompanyID, RoomID);
                        }

                        IsValidInvalidQuantity = true;
                        objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountByLIGUId(InventoryCount.CountDetailGUID, RoomID, CompanyID);
                        List<CountLineItemDetailDTO> lstCountLineItemDetail = objInventoryCountDAL.GetLotDetailForCountByCountDetailGUID(
                            (InventoryCount.GUID != null && InventoryCount.GUID != Guid.Empty ? InventoryCount.GUID : InventoryCount.CountDetailGUID),
                                                InventoryCount.ItemGUID, RoomDateFormat, CompanyID, RoomID);

                        bool isValidConsignedCredit = false;
                        bool isValidCusOwnedCredit = false;
                        if (objInventoryCountDTO.CountType.ToLower().Equals("m"))
                        {
                            foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
                            {
                                bool IsStagginLocation = false;
                                BinMasterDTO objLocDTO = new BinMasterDAL(EnterPriseDBName).GetBinByID(objCountLineItemDetailDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID);
                                if (objLocDTO != null)
                                {
                                    IsStagginLocation = objLocDTO.IsStagingLocation;
                                }

                                //isValidConsignedCredit = false;
                                //isValidCusOwnedCredit = false;
                                bool isnotsufficientpull = false;
                                double? CusOwnedDifference = 0;
                                double? ConsignedDifference = 0;

                                double? countedCusOwned = 0;
                                double? countedConsigned = 0;
                                if (IsStagginLocation)
                                {
                                    IEnumerable<MaterialStagingPullDetailDTO> lstMSPullDetail = null;
                                    lstMSPullDetail = new MaterialStagingPullDetailDAL(EnterPriseDBName).GetCountDifferenceforValidatMSPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
                                    if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
                                    }
                                    else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    if (lstMSPullDetail != null && lstMSPullDetail.Count() > 0)
                                    {
                                        countedCusOwned = lstMSPullDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                        countedConsigned = lstMSPullDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                                    }
                                }
                                else
                                {
                                    IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
                                    lstItemLocationDetail = new ItemLocationDetailsDAL(EnterPriseDBName).GetCountDifferenceforValidatPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
                                    if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
                                    }
                                    else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                                    {
                                        countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                        countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                                    }
                                }
                                CusOwnedDifference = (objCountLineItemDetailDTO.CountCustomerOwnedQuantity - countedCusOwned);
                                ConsignedDifference = (objCountLineItemDetailDTO.CountConsignedQuantity - countedConsigned);

                                if (CusOwnedDifference > 0 || ConsignedDifference > 0)
                                {

                                    double TotalConsignedPoolQuantity = 0;
                                    double TotalCusOwnedPoolQuantity = 0;
                                    string _pullAction = "pull";
                                    IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
                                    PullDetailsDAL pullDetailDAL = new PullDetailsDAL(EnterPriseDBName);
                                    if (IsStagginLocation)
                                    {
                                        _pullAction = "ms pull";
                                    }
                                    else
                                    {
                                        _pullAction = "pull";
                                    }

                                    PullTransactionDAL objPullDetails = new PullTransactionDAL(EnterPriseDBName);

                                    if (InventoryCount.SerialNumberTracking)
                                    {
                                        lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndSerialNo_CreditHistoryForCountNormal(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), _pullAction, RoomID, CompanyID, objCountLineItemDetailDTO.SerialNumber);
                                        // if (objRoomDTO.IsIgnoreCreditRule)
                                        // {
                                        bool IsSerailAvailableForCredit = objPullDetails.ValidateSerialNumberForCreditCount(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.SerialNumber, CompanyID, RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
                                        if (IsSerailAvailableForCredit == false)
                                        {
                                            return new ApplyCountOnLineItemRespDTO() { Message = msgCreditTransactionForSerialNumber + " " + objCountLineItemDetailDTO.SerialNumber, Status = "fail", IsCountClosed = false };
                                            //return Json(new { Message = "Credit transaction is already done for selected Serial Number " + objCountLineItemDetailDTO.SerialNumber, Status = "fail", IsCountClosed = false });
                                        }
                                        // }
                                    }
                                    else if (InventoryCount.LotNumberTracking)
                                    {
                                        lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndLotNo_CreditHistoryForCountNormal(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), _pullAction, RoomID, CompanyID, objCountLineItemDetailDTO.LotNumber);
                                        //if (objRoomDTO.IsIgnoreCreditRule)
                                        //{
                                        if (InventoryCount.LotNumberTracking && InventoryCount.DateCodeTracking)
                                        {
                                            DateTime ExpirationDate;
                                            if (!string.IsNullOrWhiteSpace(Convert.ToString(objCountLineItemDetailDTO.ExpirationDate)))
                                            {
                                                ExpirationDate = Convert.ToDateTime(objCountLineItemDetailDTO.ExpirationDate.Value.ToString("MM/dd/yyyy"));
                                                bool IsLotAvailableForCredit = objPullDetails.ValidateLotDateCodeForCredit(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.LotNumber, ExpirationDate, CompanyID, RoomID);

                                                if (!IsLotAvailableForCredit)
                                                {
                                                    return new ApplyCountOnLineItemRespDTO() { Message = string.Format(msgCreditTransactionForLotAndExpDateAvailable, objCountLineItemDetailDTO.LotNumber, ExpirationDate.ToString(Convert.ToString(RoomDateFormat))), Status = "fail", IsCountClosed = false };
                                                    //return Json(new { Message = "Credit transaction is for selected Lot Number " + objCountLineItemDetailDTO.LotNumber + " and ExpirationDate " + ExpirationDate.ToString(Convert.ToString(RoomDateFormat)) + " is available.You can not use other lot", Status = "fail", IsCountClosed = false });
                                                }
                                            }
                                        }
                                        //}
                                    }

                                    if (!InventoryCount.SerialNumberTracking && !InventoryCount.LotNumberTracking)
                                    {
                                        lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryForCountNormal(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), _pullAction, RoomID, CompanyID);
                                    }

                                    if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                                    {
                                        if (ConsignedDifference > 0)
                                        {
                                            TotalConsignedPoolQuantity = lstPullDetailDTO.Where(x => (x.ConsignedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
                                            if (TotalConsignedPoolQuantity >= (ConsignedDifference))
                                            {
                                                isValidConsignedCredit = true;
                                            }
                                            else
                                            {
                                                isValidConsignedCredit = false;
                                                isnotsufficientpull = true;
                                            }
                                        }
                                        if (CusOwnedDifference > 0)
                                        {
                                            TotalCusOwnedPoolQuantity = lstPullDetailDTO.Where(x => (x.CustomerOwnedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
                                            if (TotalCusOwnedPoolQuantity >= (CusOwnedDifference))
                                            {
                                                isValidCusOwnedCredit = true;
                                            }
                                            else
                                            {
                                                isValidCusOwnedCredit = false;
                                                isnotsufficientpull = true;
                                            }
                                        }
                                        if (isnotsufficientpull)
                                        {
                                            if (!objRoomDTO.IsIgnoreCreditRule)
                                            {
                                                return new ApplyCountOnLineItemRespDTO() { Message = msgCantApplyCountNotEnoughPullForCredit, Status = "fail", IsCountClosed = false };
                                                //return Json(new { Message = "Can not apply count because enough pull is not available for credit count.", Status = "fail", IsCountClosed = false });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!objRoomDTO.IsIgnoreCreditRule)
                                        {
                                            return new ApplyCountOnLineItemRespDTO() { Message = msgCantApplyCountNotEnoughPullForCredit, Status = "fail", IsCountClosed = false };
                                            //return Json(new { Message = "Can not apply count because enough pull is not available for credit count.", Status = "fail", IsCountClosed = false });
                                        }
                                    }

                                }
                            }
                        }
                        else if (objInventoryCountDTO.CountType.ToLower().Equals("a"))
                        {
                            foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
                            {
                                bool IsStagginLocation = false;
                                BinMasterDTO objLocDTO = new BinMasterDAL(EnterPriseDBName).GetBinByID(objCountLineItemDetailDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID);
                                if (objLocDTO != null)
                                {
                                    IsStagginLocation = objLocDTO.IsStagingLocation;
                                }

                                double? CusOwnedDifference = 0;
                                double? ConsignedDifference = 0;

                                double? countedCusOwned = 0;
                                double? countedConsigned = 0;

                                if (IsStagginLocation)
                                {
                                    IEnumerable<MaterialStagingPullDetailDTO> lstMSPullDetail = null;
                                    lstMSPullDetail = new MaterialStagingPullDetailDAL(EnterPriseDBName).GetCountDifferenceforValidatMSPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
                                    if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
                                    }
                                    else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstMSPullDetail = lstMSPullDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    if (lstMSPullDetail != null && lstMSPullDetail.Count() > 0)
                                    {
                                        countedCusOwned = lstMSPullDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                        countedConsigned = lstMSPullDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                                    }
                                }
                                else
                                {
                                    IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
                                    lstItemLocationDetail = new ItemLocationDetailsDAL(EnterPriseDBName).GetCountDifferenceforValidatPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
                                    if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
                                    }
                                    else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
                                    }
                                    else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
                                    {
                                        lstItemLocationDetail = lstItemLocationDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
                                    }
                                    if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
                                    {
                                        countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
                                        countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
                                    }
                                }

                                CusOwnedDifference = (objCountLineItemDetailDTO.CountCustomerOwnedQuantity - countedCusOwned);
                                ConsignedDifference = (objCountLineItemDetailDTO.CountConsignedQuantity - countedConsigned);

                                if (CusOwnedDifference > 0 || ConsignedDifference > 0)
                                {
                                    if (InventoryCount.SerialNumberTracking)
                                    {
                                        PullTransactionDAL objPullDetails = new PullTransactionDAL(EnterPriseDBName);
                                        //bool IsSerailAvailableForCredit = objPullDetails.ValidateSerialNumberForCredit(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.SerialNumber, CompanyID, RoomID);
                                        bool IsSerailAvailableForCredit = objPullDetails.ValidateSerialNumberForCreditCount(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.SerialNumber, CompanyID, RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
                                        if (IsSerailAvailableForCredit == false)
                                        {
                                            return new ApplyCountOnLineItemRespDTO() { Message = msgCreditTransactionForSerialNumber + " " + objCountLineItemDetailDTO.SerialNumber, Status = "fail", IsCountClosed = false };
                                            //return Json(new { Message = "Credit transaction is already done for selected Serial Number " + objCountLineItemDetailDTO.SerialNumber, Status = "fail", IsCountClosed = false });
                                        }
                                    }
                                }
                            }
                        }

                        objInventoryCountDetailDTO = objInventoryCountDAL.BeforeApplyAction(InventoryCount, UserID, lstCountLineItemDetail);
                        if (lstCountLineItemDetail != null || lstCountLineItemDetail.Count > 0)
                        {
                            CustomerOwnedQty = lstCountLineItemDetail.Sum(x => (x.CountCustomerOwnedQuantity == null ? 0 : x.CountCustomerOwnedQuantity.Value));
                            ConsignedOwnedQty = lstCountLineItemDetail.Sum(x => (x.CountConsignedQuantity == null ? 0 : x.CountConsignedQuantity.Value));
                            TotalItems++;
                            if (((objInventoryCountDetailDTO.CountCustomerOwnedQuantity == null || objInventoryCountDetailDTO.CountCustomerOwnedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountCustomerOwnedQuantity.Value) != (CustomerOwnedQty == (-0.000000001) ? 0 : CustomerOwnedQty)
                                || ((objInventoryCountDetailDTO.CountConsignedQuantity == null || objInventoryCountDetailDTO.CountConsignedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountConsignedQuantity.Value) != (ConsignedOwnedQty == (-0.000000001) ? 0 : ConsignedOwnedQty))
                            {
                                if (InventoryCount.SerialNumberTracking == true || InventoryCount.LotNumberTracking == true)
                                {
                                    IsValidInvalidQuantity = false;
                                    InvalidQuantity++;
                                }
                                else
                                {
                                    CustomerOwnedQty = ((objInventoryCountDetailDTO.CountCustomerOwnedQuantity == null || objInventoryCountDetailDTO.CountCustomerOwnedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountCustomerOwnedQuantity.Value);
                                    ConsignedOwnedQty = ((objInventoryCountDetailDTO.CountConsignedQuantity == null || objInventoryCountDetailDTO.CountConsignedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountConsignedQuantity.Value);
                                    objInventoryCountDAL.UpdateCountLineItemDetailQty(InventoryCount.CountDetailGUID, objInventoryCountDetailDTO.CountCustomerOwnedQuantity, objInventoryCountDetailDTO.CountConsignedQuantity, objInventoryCountDetailDTO.CusOwnedDifference, objInventoryCountDetailDTO.ConsignedDifference);
                                }
                            }

                            if (IsValidInvalidQuantity == true)
                            {
                                List<ItemLocationDetailsDTO> lstProperRecords = new List<ItemLocationDetailsDTO>();
                                List<MaterialStagingPullDetailDTO> lstStgProperRecords = new List<MaterialStagingPullDetailDTO>();
                                CartItemDAL objCartItemDAL = new CartItemDAL(EnterPriseDBName);
                                ItemLocationDetailsDTO objItemLocationDetailsDTO;
                                MaterialStagingPullDetailDTO objMaterialStgPullDetailsDTO;
                                bool isProperRecordsAvail = false;

                                foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
                                {
                                    if (objCountLineItemDetailDTO.IsStagingLocationCount.GetValueOrDefault(false) == true)
                                    {
                                        objMaterialStgPullDetailsDTO = new MaterialStagingPullDetailDTO();
                                        objMaterialStgPullDetailsDTO.ItemGUID = objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty);
                                        objMaterialStgPullDetailsDTO.StagingBinId = objCountLineItemDetailDTO.BinID.GetValueOrDefault(0);

                                        objMaterialStgPullDetailsDTO.ConsignedQuantity = objCountLineItemDetailDTO.CountConsignedQuantity;
                                        objMaterialStgPullDetailsDTO.CustomerOwnedQuantity = objCountLineItemDetailDTO.CountCustomerOwnedQuantity;
                                        if (objMaterialStgPullDetailsDTO.SerialNumberTracking == true || objMaterialStgPullDetailsDTO.LotNumberTracking == true)
                                        {
                                            if (objMaterialStgPullDetailsDTO.ConsignedQuantity < 0)
                                                objMaterialStgPullDetailsDTO.ConsignedQuantity = 0;

                                            if (objMaterialStgPullDetailsDTO.CustomerOwnedQuantity < 0)
                                                objMaterialStgPullDetailsDTO.CustomerOwnedQuantity = 0;
                                        }

                                        objMaterialStgPullDetailsDTO.UDF1 = HttpUtility.UrlDecode(InventoryCount.UDF1);
                                        objMaterialStgPullDetailsDTO.UDF2 = HttpUtility.UrlDecode(InventoryCount.UDF2);
                                        objMaterialStgPullDetailsDTO.UDF3 = HttpUtility.UrlDecode(InventoryCount.UDF3);
                                        objMaterialStgPullDetailsDTO.UDF4 = HttpUtility.UrlDecode(InventoryCount.UDF4);
                                        objMaterialStgPullDetailsDTO.UDF5 = HttpUtility.UrlDecode(InventoryCount.UDF5);
                                        objMaterialStgPullDetailsDTO.ItemNumber = objCountLineItemDetailDTO.ItemNumber;
                                        objMaterialStgPullDetailsDTO.BinNumber = objCountLineItemDetailDTO.BinNumber;
                                        objMaterialStgPullDetailsDTO.Expiration = objCountLineItemDetailDTO.Expiration;
                                        objMaterialStgPullDetailsDTO.Received = objCountLineItemDetailDTO.Received;
                                        objMaterialStgPullDetailsDTO.LotNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.LotNumber)) ? objCountLineItemDetailDTO.LotNumber.Trim() : string.Empty;
                                        objMaterialStgPullDetailsDTO.SerialNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.SerialNumber)) ? objCountLineItemDetailDTO.SerialNumber.Trim() : string.Empty;
                                        lstStgProperRecords.Add(objMaterialStgPullDetailsDTO);
                                    }
                                    else
                                    {
                                        objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                        objItemLocationDetailsDTO.ItemGUID = objCountLineItemDetailDTO.ItemGUID;
                                        objItemLocationDetailsDTO.BinID = objCountLineItemDetailDTO.BinID;

                                        objItemLocationDetailsDTO.ConsignedQuantity = objCountLineItemDetailDTO.CountConsignedQuantity;
                                        objItemLocationDetailsDTO.CustomerOwnedQuantity = objCountLineItemDetailDTO.CountCustomerOwnedQuantity;
                                        if (objCountLineItemDetailDTO.SerialNumberTracking == true || objCountLineItemDetailDTO.LotNumberTracking == true)
                                        {
                                            if (objItemLocationDetailsDTO.ConsignedQuantity < 0)
                                                objItemLocationDetailsDTO.ConsignedQuantity = 0;

                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity < 0)
                                                objItemLocationDetailsDTO.CustomerOwnedQuantity = 0;
                                        }

                                        objItemLocationDetailsDTO.UDF1 = HttpUtility.UrlDecode(InventoryCount.UDF1);
                                        objItemLocationDetailsDTO.UDF2 = HttpUtility.UrlDecode(InventoryCount.UDF2);
                                        objItemLocationDetailsDTO.UDF3 = HttpUtility.UrlDecode(InventoryCount.UDF3);
                                        objItemLocationDetailsDTO.UDF4 = HttpUtility.UrlDecode(InventoryCount.UDF4);
                                        objItemLocationDetailsDTO.UDF5 = HttpUtility.UrlDecode(InventoryCount.UDF5);
                                        objItemLocationDetailsDTO.ItemNumber = objCountLineItemDetailDTO.ItemNumber;
                                        objItemLocationDetailsDTO.BinNumber = objCountLineItemDetailDTO.BinNumber;
                                        objItemLocationDetailsDTO.Expiration = objCountLineItemDetailDTO.Expiration;
                                        objItemLocationDetailsDTO.Received = objCountLineItemDetailDTO.Received;
                                        objItemLocationDetailsDTO.LotNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.LotNumber)) ? objCountLineItemDetailDTO.LotNumber.Trim() : string.Empty;
                                        objItemLocationDetailsDTO.SerialNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.SerialNumber)) ? objCountLineItemDetailDTO.SerialNumber.Trim() : string.Empty;
                                        lstProperRecords.Add(objItemLocationDetailsDTO);
                                    }

                                    if (objCountLineItemDetailDTO.LotNumberTracking.GetValueOrDefault(false) == true || objCountLineItemDetailDTO.SerialNumberTracking.GetValueOrDefault(false) == true || objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) == true)
                                    {
                                        UpdateCountLineItemOnApply(objCountLineItemDetailDTO, lstCountLineItemDetail, UserID, RoomDateFormat);
                                    }
                                }


                                if (lstStgProperRecords != null && lstStgProperRecords.Count > 0)
                                {
                                    DataTable dtItemLocations = GetTableFromStgList(lstStgProperRecords, RoomID, CompanyID, UserID, RoomDateFormat);
                                    objItemLocationDetailsDAL.ApplyStageCountLineitem(dtItemLocations, RoomID, CompanyID, UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);

                                    if (objInventoryCountDTO.CountType.ToLower().Equals("m"))
                                    {
                                        if (objRoomDTO.IsIgnoreCreditRule)
                                        {
                                            if (objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
                                                new PullMasterDAL(EnterPriseDBName).InsertintoMSCreditHistoryForCount(RoomID, CompanyID, UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID, "MS Pull Credit");
                                        }
                                        else
                                        {
                                            if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
                                                new ItemLocationDetailsDAL(EnterPriseDBName).InsertMSCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
                                        }
                                    }
                                    else
                                    {
                                        if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
                                            new ItemLocationDetailsDAL(EnterPriseDBName).InsertMSCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
                                    }

                                    isProperRecordsAvail = true;
                                }


                                if (lstProperRecords != null && lstProperRecords.Count > 0)
                                {
                                    DataTable dtItemLocations = GetTableFromList(lstProperRecords, RoomDateFormat);
                                    objItemLocationDetailsDAL.ApplyCountLineitem(dtItemLocations, RoomID, CompanyID, UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
                                    if (objInventoryCountDTO.CountType.ToLower().Equals("m"))
                                    {
                                        if (objRoomDTO.IsIgnoreCreditRule)
                                        {
                                            if (objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
                                                new PullMasterDAL(EnterPriseDBName).InsertintoCreditHistoryForCount(RoomID, CompanyID, UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID, "Credit");
                                        }
                                        else
                                        {
                                            if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
                                                new ItemLocationDetailsDAL(EnterPriseDBName).InsertCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
                                        }
                                    }
                                    else
                                    {
                                        if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
                                            new ItemLocationDetailsDAL(EnterPriseDBName).InsertCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
                                    }

                                    lstProperRecords.ForEach(t =>
                                    {
                                        //objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, UserID, "Web", "Inventorycontroller>> ApplyOnCountLineItem");
                                        objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, UserID, "Web", "Inventory >> Apply Count", SessionUserId);
                                    });

                                    if (objItemDTO != null && objItemDTO.ItemType == 3 && objItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                                    {
                                        new KitDetailDAL(EnterPriseDBName).UpdateQtyToMeedDemand(InventoryCount.ItemGUID, UserID, SessionUserId);
                                    }
                                    //if (objItemDTO != null)
                                    //{
                                    //    QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(EnterPriseDBName);
                                    //    EnterpriseDAL objEntDAL = new EnterpriseDAL(EnterPriseDBName);
                                    //    EnterpriseDTO objEntDTO = objEntDAL.GetEnterpriseByDbName(EnterPriseDBName);
                                    //    //objQBItemDAL.InsertQuickBookItem(objItemDTO.GUID, objEntDTO.ID, CompanyID, RoomID, "Update", objItemDTO.IsDeleted, SessionUserId, "Web", null, "Inventory >> Apply Count");
                                    //}


                                    //if (InventoryCount.ItemLotSerialType != "LOT_SERIAL")
                                    //{
                                    //    ApplyOnCountLineItem(objInventoryCountDetailDTO);
                                    //}

                                    isProperRecordsAvail = true;

                                }

                                if (isProperRecordsAvail)
                                {
                                    //PostApplyAction(objInventoryCountDetailDTO);
                                    this.PostApplyOnSignleLineItem(objInventoryCountDetailDTO, UserID);
                                }
                            }
                        }
                    }

                    if (InvalidQuantity == 0)
                    {
                        /* closed count comment for WI-3311 */
                        //if (objItemLocationDetailsDAL.CloseCountIfAllApplied(lstLineItems[0].InventoryCountGUID, UserID, "Web"))
                        //{
                        //    _IsCountClosed = true;
                        //}
                        string msg = IsFromWeb ? SaveMessage : SaveSuccess;
                        return new ApplyCountOnLineItemRespDTO() { Message = msg, Status = "ok", IsCountClosed = _IsCountClosed, CurrentObj = lstLineItems.FirstOrDefault() };
                        //return Json(new { Message = ResMessage.SaveMessage, Status = "ok", IsCountClosed = _IsCountClosed });
                    }
                    else if (TotalItems == InvalidQuantity)
                    {
                        return new ApplyCountOnLineItemRespDTO() { Message = msgCantApplyCountTotalQtyNotMatchedWithDetailQty, Status = "fail", IsCountClosed = false };
                        //return Json(new { Message = "Can not apply count because total quantities are not matching with detail quantities", Status = "fail", IsCountClosed = false });
                    }
                    else
                    {
                        return new ApplyCountOnLineItemRespDTO() { Message = msgCantApplyCountForSomeItems, Status = "fail", IsCountClosed = false };
                        //return Json(new { Message = "Can not apply count for some items because total quantities are not matching with detail quantities", Status = "fail", IsCountClosed = false });
                    }
                }
                catch (Exception ex)
                {
                    string msg = IsFromWeb ? SaveMessage : ex.Message;
                    return new ApplyCountOnLineItemRespDTO() { Message = msg, Status = "fail", IsCountClosed = false, CurrentObj = lstLineItems.FirstOrDefault() };
                    //return Json(new { Message = ResMessage.SaveErrorMsg, Status = "fail", IsCountClosed = false });
                }
            }
            else
            {
                message = IsFromWeb ? SaveErrorMsg : SaveError;
                status = "fail";
                return new ApplyCountOnLineItemRespDTO() { Message = message, Status = status, CurrentObj = lstLineItems.First(), IsCountClosed = false };
                //return Json(new { Message = message, Status = status, CurrentObj = lstLineItems.First(), IsCountClosed = false });
            }
        }

        public bool AddMissingLotSerial(Guid CountDetailGUID, Guid ItemGUID, string RoomDateFormat, long UserID, long CompanyID, long RoomID, bool IsStagingLocation = false)
        {
            string EnterPriseDBName = base.DataBaseName;
            CountLineItemDetailDTO objCountLineItemDetailDTO;
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(EnterPriseDBName);
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(EnterPriseDBName);

            List<CountLineItemDetailDTO> lstCountLineItemDetail = objInventoryCountDAL.GetLotDetailForCountByCountDetailGUID(CountDetailGUID, ItemGUID, RoomDateFormat, CompanyID, RoomID, false);

            if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0)
            {
                List<CountLineItemDetailDTO> locations = objBinMasterDAL.GetAllItemLocationsByItemId(lstCountLineItemDetail[0].ItemGUID.Value, CompanyID, RoomID, lstCountLineItemDetail[0].IsStagingLocationCount.GetValueOrDefault(false));
                List<CountLineItemDetailDTO> Sublocations = locations.Where(x => x.BinID == lstCountLineItemDetail[0].BinID && !lstCountLineItemDetail.Select(y => y.LotSerialNumber.ToUpper().Trim()).Contains(x.LotSerialNumber.ToUpper().Trim())).ToList();

                if (Sublocations != null && Sublocations.Count > 0)
                {
                    foreach (CountLineItemDetailDTO objLocation in Sublocations)
                    {
                        objCountLineItemDetailDTO = new CountLineItemDetailDTO();
                        objCountLineItemDetailDTO.ItemGUID = lstCountLineItemDetail[0].ItemGUID;
                        objCountLineItemDetailDTO.ItemNumber = lstCountLineItemDetail[0].ItemNumber;
                        objCountLineItemDetailDTO.ItemDescription = lstCountLineItemDetail[0].ItemDescription;
                        objCountLineItemDetailDTO.ItemType = lstCountLineItemDetail[0].ItemType;
                        objCountLineItemDetailDTO.Consignment = lstCountLineItemDetail[0].Consignment;
                        objCountLineItemDetailDTO.Comment = "Not specified serials in UI";
                        objCountLineItemDetailDTO.BinID = objLocation.BinID;
                        objCountLineItemDetailDTO.BinNumber = objLocation.BinNumber;
                        objCountLineItemDetailDTO.CustomerOwnedQuantity = null;
                        objCountLineItemDetailDTO.AvailableQuantity = null;
                        objCountLineItemDetailDTO.CountCustomerOwnedQuantity = 0;
                        objCountLineItemDetailDTO.ConsignedQuantity = null;
                        objCountLineItemDetailDTO.CountConsignedQuantity = 0;
                        objCountLineItemDetailDTO.SerialNumberTracking = lstCountLineItemDetail[0].SerialNumberTracking;
                        objCountLineItemDetailDTO.LotNumberTracking = lstCountLineItemDetail[0].LotNumberTracking;
                        objCountLineItemDetailDTO.DateCodeTracking = lstCountLineItemDetail[0].DateCodeTracking;
                        objCountLineItemDetailDTO.LotSerialNumber = objLocation.LotSerialNumber;
                        objCountLineItemDetailDTO.LotNumber = (lstCountLineItemDetail[0].LotNumberTracking == true ? objLocation.LotSerialNumber : "");
                        objCountLineItemDetailDTO.SerialNumber = (lstCountLineItemDetail[0].SerialNumberTracking == true ? objLocation.LotSerialNumber : "");
                        objCountLineItemDetailDTO.Expiration = (objLocation.ExpirationDate == null ? "" : objLocation.ExpirationDate.Value.ToString(RoomDateFormat));
                        objCountLineItemDetailDTO.ExpirationDate = objLocation.ExpirationDate;
                        objCountLineItemDetailDTO.ReceivedDate = DateTime.UtcNow;
                        objCountLineItemDetailDTO.Cost = lstCountLineItemDetail[0].Cost;
                        objCountLineItemDetailDTO.IsStagingLocationCount = lstCountLineItemDetail[0].IsStagingLocationCount;
                        objCountLineItemDetailDTO.CountGUID = lstCountLineItemDetail[0].CountGUID;
                        objCountLineItemDetailDTO.CountDetailGUID = lstCountLineItemDetail[0].CountDetailGUID;
                        objCountLineItemDetailDTO.IsValidObject = true;
                        lstCountLineItemDetail.Add(objCountLineItemDetailDTO);
                    }

                    SaveLotDetailPopupForCountUpdate(lstCountLineItemDetail, RoomDateFormat, UserID, CompanyID, RoomID);
                }
            }

            return true;
        }

        public bool SaveLotDetailPopupForCountUpdate(List<CountLineItemDetailDTO> lstCountLineItemDetail, string RoomDateFormat, long UserID,
            long CompanyID, long RoomID
            )
        {
            List<CountLineItemDetailDTO> lstCountLineItemDetailTmp;
            CountLineItemDetailDTO objCountLineItemDetailTmp;
            DateTime? ExpirationDate = null;
            double? CustomerOwnedQuantity = 0;
            double? ConsignedQuantity = 0;
            string EnterPriseDBName = base.DataBaseName;
            //-------------------------------------------------------------------
            //
            if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0 && lstCountLineItemDetail[0].CountDetailGUID != null)
            {
                InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(EnterPriseDBName);
                List<CountLineItemDetailDTO> lstCountLineItemDetailExisting = objInventoryCountDAL.GetCountLineItemDetailList(lstCountLineItemDetail[0].CountDetailGUID.Value, RoomDateFormat);

                if (lstCountLineItemDetail[0].SerialNumberTracking == true || lstCountLineItemDetail[0].LotNumberTracking == true)
                {
                    foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetailExisting)
                    {
                        if (lstCountLineItemDetail.Where(x => x.LotSerialNumber.ToUpper().Trim() == objCountLineItemDetail.LotSerialNumber.ToUpper().Trim()
                                                                      && (x.DateCodeTracking == false || x.ExpirationDate == null ? "" : ((DateTime)x.ExpirationDate).ToString("dd.MM.yyyy")) == (objCountLineItemDetail.DateCodeTracking == false || objCountLineItemDetail.ExpirationDate == null ? "" : ((DateTime)objCountLineItemDetail.ExpirationDate).ToString("dd.MM.yyyy"))).Count() <= 0)
                        {
                            //---------------Delete Missing LOT/Serial---------------
                            //
                            objCountLineItemDetail.IsDeleted = true;
                            objInventoryCountDAL.DeleteCountLineItemDetail(objCountLineItemDetail.GUID.Value);
                        }
                    }

                    CountLineItemDetailDTO objCountLineItemEditDetail;
                    foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetail)
                    {
                        if (lstCountLineItemDetailExisting.Where(x => x.LotSerialNumber.ToUpper().Trim() == objCountLineItemDetail.LotSerialNumber.ToUpper().Trim()
                                                                      && (x.DateCodeTracking == false || x.ExpirationDate == null ? "" : ((DateTime)x.ExpirationDate).ToString("dd.MM.yyyy")) == (objCountLineItemDetail.DateCodeTracking == false || objCountLineItemDetail.ExpirationDate == null ? "" : ((DateTime)objCountLineItemDetail.ExpirationDate).ToString("dd.MM.yyyy"))).Count() > 0)
                        {
                            //---------------Update Existing LOT/Serial---------------
                            //
                            objCountLineItemEditDetail = lstCountLineItemDetailExisting.Where(x => x.LotSerialNumber.ToUpper().Trim() == objCountLineItemDetail.LotSerialNumber.ToUpper().Trim()
                                                                      && (x.DateCodeTracking == false || x.ExpirationDate == null ? "" : ((DateTime)x.ExpirationDate).ToString("dd.MM.yyyy")) == (objCountLineItemDetail.DateCodeTracking == false || objCountLineItemDetail.ExpirationDate == null ? "" : ((DateTime)objCountLineItemDetail.ExpirationDate).ToString("dd.MM.yyyy"))).FirstOrDefault();

                            objCountLineItemEditDetail.Comment = objCountLineItemDetail.Comment;
                            objCountLineItemEditDetail.CountCustomerOwnedQuantity = objCountLineItemDetail.CountCustomerOwnedQuantity;
                            objCountLineItemEditDetail.CountConsignedQuantity = objCountLineItemDetail.CountConsignedQuantity;

                            ExpirationDate = objCountLineItemDetail.ExpirationDate;
                            objCountLineItemEditDetail.ExpirationDate = ExpirationDate;
                            objCountLineItemEditDetail.Expiration = (ExpirationDate == null ? "" : ExpirationDate.Value.ToString(RoomDateFormat));

                            objCountLineItemEditDetail.IsConsigned = lstCountLineItemDetail[0].IsConsigned;
                            objCountLineItemEditDetail.LotNumberTracking = lstCountLineItemDetail[0].LotNumberTracking;
                            objCountLineItemEditDetail.SerialNumberTracking = lstCountLineItemDetail[0].SerialNumberTracking;
                            objCountLineItemEditDetail.DateCodeTracking = lstCountLineItemDetail[0].DateCodeTracking;

                            objCountLineItemEditDetail.Updated = DateTime.UtcNow;
                            objCountLineItemEditDetail.LastUpdatedBy = UserID;
                            objCountLineItemEditDetail.EditedFrom = "Web";



                            objInventoryCountDAL.UpdateCountLineItemDetail(objCountLineItemEditDetail);
                        }
                        else
                        {
                            //---------------Insert New LOT/Serial---------------
                            //
                            objCountLineItemDetail.GUID = Guid.NewGuid();
                            objCountLineItemDetail.CompanyID = CompanyID;
                            objCountLineItemDetail.RoomID = RoomID;
                            objCountLineItemDetail.Created = DateTime.UtcNow;
                            objCountLineItemDetail.CreatedBy = UserID;
                            objCountLineItemDetail.Updated = DateTime.UtcNow;
                            objCountLineItemDetail.LastUpdatedBy = UserID;
                            objCountLineItemDetail.IsDeleted = false;
                            objCountLineItemDetail.IsArchived = false;
                            objCountLineItemDetail.AddedFrom = "Web";
                            objCountLineItemDetail.ReceivedOn = DateTime.UtcNow;
                            objCountLineItemDetail.ReceivedOnWeb = DateTime.UtcNow;
                            objInventoryCountDAL.AddCountLineItemDetail(objCountLineItemDetail);
                        }
                    }

                    ////---------------Delete Missing LOT/Serial---------------
                    ////
                    //lstCountLineItemDetailTmp = lstCountLineItemDetailExisting.Where(x => !lstCountLineItemDetail.Select(y => y.LotSerialNumber.ToUpper().Trim()).Contains(x.LotSerialNumber.ToUpper().Trim())).ToList();
                    //foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetailTmp)
                    //{
                    //    objCountLineItemDetail.IsDeleted = true;
                    //    objInventoryCountDAL.DeleteCountLineItemDetail(objCountLineItemDetail.GUID.Value);
                    //}

                    ////---------------Update Existing LOT/Serial---------------
                    ////
                    //lstCountLineItemDetailTmp = lstCountLineItemDetailExisting.Where(x => lstCountLineItemDetail.Select(y => y.LotSerialNumber.ToUpper().Trim()).Contains(x.LotSerialNumber.ToUpper().Trim())).ToList();
                    //foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetailTmp)
                    //{
                    //    objCountLineItemDetail.Comment = lstCountLineItemDetail.Where(x => x.LotSerialNumber.ToUpper().Trim() == objCountLineItemDetail.LotSerialNumber.ToUpper().Trim()).Select(x => x.Comment).FirstOrDefault();
                    //    objCountLineItemDetail.CountCustomerOwnedQuantity = lstCountLineItemDetail.Where(x => x.LotSerialNumber.ToUpper().Trim() == objCountLineItemDetail.LotSerialNumber.ToUpper().Trim()).Select(x => x.CountCustomerOwnedQuantity).FirstOrDefault();
                    //    objCountLineItemDetail.CountConsignedQuantity = lstCountLineItemDetail.Where(x => x.LotSerialNumber.ToUpper().Trim() == objCountLineItemDetail.LotSerialNumber.ToUpper().Trim()).Select(x => x.CountConsignedQuantity).FirstOrDefault();
                    //    ExpirationDate = lstCountLineItemDetail.Where(x => x.LotSerialNumber.ToUpper().Trim() == objCountLineItemDetail.LotSerialNumber.ToUpper().Trim()).Select(x => x.ExpirationDate).FirstOrDefault();
                    //    objCountLineItemDetail.ExpirationDate = ExpirationDate;
                    //    objCountLineItemDetail.Expiration = (ExpirationDate == null ? "" : ExpirationDate.Value.ToString(SessionHelper.RoomDateFormat));
                    //    objCountLineItemDetail.Updated = DateTime.UtcNow;
                    //    objCountLineItemDetail.LastUpdatedBy = SessionHelper.UserID;
                    //    objCountLineItemDetail.EditedFrom = "Web";
                    //    objInventoryCountDAL.UpdateCountLineItemDetail(objCountLineItemDetail);
                    //}

                    ////---------------Insert New LOT/Serial---------------
                    ////
                    //lstCountLineItemDetailTmp = lstCountLineItemDetail.Where(x => !lstCountLineItemDetailExisting.Select(y => y.LotSerialNumber.ToUpper().Trim()).Contains(x.LotSerialNumber.ToUpper().Trim())).ToList();
                    //foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetailTmp)
                    //{
                    //    objCountLineItemDetail.GUID = Guid.NewGuid();
                    //    objCountLineItemDetail.CompanyID = SessionHelper.CompanyID;
                    //    objCountLineItemDetail.RoomID = SessionHelper.RoomID;
                    //    objCountLineItemDetail.Created = DateTime.UtcNow;
                    //    objCountLineItemDetail.CreatedBy = SessionHelper.UserID;
                    //    objCountLineItemDetail.Updated = DateTime.UtcNow;
                    //    objCountLineItemDetail.LastUpdatedBy = SessionHelper.UserID;
                    //    objCountLineItemDetail.IsDeleted = false;
                    //    objCountLineItemDetail.IsArchived = false;
                    //    objCountLineItemDetail.AddedFrom = "Web";
                    //    objCountLineItemDetail.ReceivedOn = DateTime.UtcNow;
                    //    objCountLineItemDetail.ReceivedOnWeb = DateTime.UtcNow;
                    //    objInventoryCountDAL.AddCountLineItemDetail(objCountLineItemDetail);
                    //}
                }
                else
                {
                    bool IsNew = false;
                    foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetail)
                    {
                        IsNew = true;
                        objCountLineItemDetailTmp = null;
                        if (objCountLineItemDetail.GUID != null && objCountLineItemDetail.GUID != Guid.Empty)
                        {
                            objCountLineItemDetailTmp = lstCountLineItemDetailExisting.Where(x => x.GUID == objCountLineItemDetail.GUID).FirstOrDefault();
                            if (objCountLineItemDetailTmp != null)
                                IsNew = false;
                        }

                        if (IsNew)
                        {
                            objCountLineItemDetailTmp = new CountLineItemDetailDTO();
                            objCountLineItemDetail.GUID = Guid.NewGuid();
                            objCountLineItemDetail.ItemGUID = objCountLineItemDetail.ItemGUID;
                            objCountLineItemDetail.BinID = objCountLineItemDetail.BinID;
                            objCountLineItemDetail.CompanyID = CompanyID;
                            objCountLineItemDetail.RoomID = RoomID;
                            objCountLineItemDetail.Created = DateTime.UtcNow;
                            objCountLineItemDetail.CreatedBy = UserID;
                            objCountLineItemDetail.AddedFrom = "Web";
                            objCountLineItemDetail.ReceivedOn = DateTime.UtcNow;
                            objCountLineItemDetail.ReceivedOnWeb = DateTime.UtcNow;
                            objCountLineItemDetail.Received = (objCountLineItemDetail.ExpirationDate == null ? "" : objCountLineItemDetail.ExpirationDate.Value.ToString(RoomDateFormat));
                            objCountLineItemDetail.Cost = objCountLineItemDetail.Cost;
                            objCountLineItemDetail.CountGUID = objCountLineItemDetail.CountGUID;
                            objCountLineItemDetail.CountDetailGUID = objCountLineItemDetail.CountDetailGUID;
                        }

                        objCountLineItemDetailTmp.Comment = objCountLineItemDetail.Comment;
                        objCountLineItemDetailTmp.CountCustomerOwnedQuantity = objCountLineItemDetail.CountCustomerOwnedQuantity;
                        objCountLineItemDetailTmp.CountConsignedQuantity = objCountLineItemDetail.CountCustomerOwnedQuantity;
                        ExpirationDate = objCountLineItemDetail.ExpirationDate;
                        objCountLineItemDetailTmp.ExpirationDate = ExpirationDate;
                        objCountLineItemDetailTmp.Expiration = (ExpirationDate == null ? "" : ExpirationDate.Value.ToString(RoomDateFormat));
                        objCountLineItemDetailTmp.Updated = DateTime.UtcNow;
                        objCountLineItemDetailTmp.LastUpdatedBy = UserID;
                        objCountLineItemDetailTmp.EditedFrom = "Web";
                        objCountLineItemDetail.IsDeleted = false;
                        objCountLineItemDetail.IsArchived = false;

                        if (IsNew)
                        {
                            objInventoryCountDAL.AddCountLineItemDetail(objCountLineItemDetail);
                        }
                        else
                        {
                            objInventoryCountDAL.UpdateCountLineItemDetail(objCountLineItemDetail);
                        }
                    }

                    //---------------Delete Missing LOT/Serial---------------
                    //
                    lstCountLineItemDetailTmp = lstCountLineItemDetailExisting.Where(x => !lstCountLineItemDetail.Select(y => y.GUID).Contains(x.GUID)).ToList();
                    foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetailTmp)
                    {
                        objCountLineItemDetail.IsDeleted = true;
                        objInventoryCountDAL.DeleteCountLineItemDetail(objCountLineItemDetail.GUID.Value);
                    }
                }

                CustomerOwnedQuantity = lstCountLineItemDetail.Select(x => (x.CountCustomerOwnedQuantity < 0 ? 0 : x.CountCustomerOwnedQuantity)).Sum();
                ConsignedQuantity = lstCountLineItemDetail.Select(x => (x.CountConsignedQuantity < 0 ? 0 : x.CountConsignedQuantity)).Sum();
                objInventoryCountDAL.UpdateCountInInventoryCountDetails(lstCountLineItemDetail[0].CountDetailGUID, CustomerOwnedQuantity, ConsignedQuantity, lstCountLineItemDetail);
            }

            return true;
        }

        public void UpdateCountLineItemOnApply(CountLineItemDetailDTO objCountLineItemDetail,
            List<CountLineItemDetailDTO> lstCountLineItemDetail, long UserID, string RoomDateFormat)
        {
            string EnterPriseDBName = base.DataBaseName;
            CountLineItemDetailDTO objCountLineItemEditDetail = new CountLineItemDetailDTO();
            DateTime? ExpirationDate = null;
            objCountLineItemEditDetail = lstCountLineItemDetail.Where(x => (x.LotSerialNumber ?? string.Empty).ToUpper().Trim() == (objCountLineItemDetail.LotSerialNumber ?? string.Empty).ToUpper().Trim()
                                                                     && (x.DateCodeTracking == false || x.ExpirationDate == null ? "" : ((DateTime)x.ExpirationDate).ToString("dd.MM.yyyy")) == (objCountLineItemDetail.DateCodeTracking == false || objCountLineItemDetail.ExpirationDate == null ? "" : ((DateTime)objCountLineItemDetail.ExpirationDate).ToString("dd.MM.yyyy"))).FirstOrDefault();

            objCountLineItemEditDetail.Comment = objCountLineItemDetail.Comment;
            objCountLineItemEditDetail.CountCustomerOwnedQuantity = objCountLineItemDetail.CountCustomerOwnedQuantity;
            objCountLineItemEditDetail.CountConsignedQuantity = objCountLineItemDetail.CountConsignedQuantity;

            ExpirationDate = objCountLineItemDetail.ExpirationDate;
            objCountLineItemEditDetail.ExpirationDate = ExpirationDate;
            objCountLineItemEditDetail.Expiration = (ExpirationDate == null ? "" : ExpirationDate.Value.ToString(RoomDateFormat));

            objCountLineItemEditDetail.IsConsigned = lstCountLineItemDetail[0].IsConsigned;
            objCountLineItemEditDetail.LotNumberTracking = lstCountLineItemDetail[0].LotNumberTracking;
            objCountLineItemEditDetail.SerialNumberTracking = lstCountLineItemDetail[0].SerialNumberTracking;
            objCountLineItemEditDetail.DateCodeTracking = lstCountLineItemDetail[0].DateCodeTracking;

            objCountLineItemEditDetail.Updated = DateTime.UtcNow;
            objCountLineItemEditDetail.LastUpdatedBy = UserID;
            objCountLineItemEditDetail.EditedFrom = "Web";

            new InventoryCountDAL(EnterPriseDBName).UpdateCountLineItemDetail(objCountLineItemEditDetail);
        }


        private DataTable GetTableFromList(List<ItemLocationDetailsDTO> lstItemLocs, string RoomDateFormat)
        {
            //RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = DateTime.UtcNow; //objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            DataTable ReturnDT = new DataTable("ItemLocationParam");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]            {
                new DataColumn() { AllowDBNull=true,ColumnName="ItemGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="ItemNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Expiration",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="LotNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ConsignedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="CustomerOwnedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ProjectSpend",DataType=typeof(String)}
            };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstItemLocs != null && lstItemLocs.Count > 0)
                {
                    foreach (var item in lstItemLocs)
                    {
                        DateTime tempDT = DateTime.Now;
                        DateTime.TryParseExact(item.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out tempDT);
                        DataRow row = ReturnDT.NewRow();
                        row["ItemGUID"] = item.ItemGUID;
                        row["ItemNumber"] = item.ItemNumber;
                        row["BinID"] = (item.BinID ?? 0) > 0 ? (object)item.BinID : DBNull.Value;
                        row["BinNumber"] = item.BinNumber;
                        row["Expiration"] = tempDT != DateTime.MinValue ? (object)tempDT : DBNull.Value;
                        row["Received"] = datetimetoConsider;
                        row["LotNumber"] = item.LotNumber;
                        row["SerialNumber"] = item.SerialNumber;
                        row["ConsignedQuantity"] = item.ConsignedQuantity.HasValue ? (object)item.ConsignedQuantity.Value : DBNull.Value;
                        row["CustomerOwnedQuantity"] = item.CustomerOwnedQuantity.HasValue ? (object)item.CustomerOwnedQuantity.Value : DBNull.Value;
                        row["ReceiptCost"] = item.Cost.HasValue ? (object)item.Cost.Value : DBNull.Value;
                        row["UDF1"] = item.UDF1;
                        row["UDF2"] = item.UDF2;
                        row["UDF3"] = item.UDF3;
                        row["UDF4"] = item.UDF4;
                        row["UDF5"] = item.UDF5;
                        row["ProjectSpend"] = string.Empty;
                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }

        private DataTable GetTableFromStgList(List<MaterialStagingPullDetailDTO> lstItemLocs, long RoomID, long CompanyID, long UserID, string RoomDateFormat)
        {
            string EnterPriseDBName = base.DataBaseName;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, UserID);
            DataTable ReturnDT = new DataTable("ItemLocationParam");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]            {
                new DataColumn() { AllowDBNull=true,ColumnName="ItemGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="ItemNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Expiration",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="LotNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ConsignedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="CustomerOwnedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ProjectSpend",DataType=typeof(String)}
            };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstItemLocs != null && lstItemLocs.Count > 0)
                {
                    foreach (var item in lstItemLocs)
                    {
                        DateTime tempDT = DateTime.Now;
                        DateTime.TryParseExact(item.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out tempDT);
                        DataRow row = ReturnDT.NewRow();
                        row["ItemGUID"] = item.ItemGUID;
                        row["ItemNumber"] = item.ItemNumber;
                        row["BinID"] = (item.StagingBinId > 0) ? (object)item.StagingBinId : DBNull.Value;
                        row["BinNumber"] = item.BinNumber;
                        row["Expiration"] = tempDT != DateTime.MinValue ? (object)tempDT : DBNull.Value;
                        row["Received"] = datetimetoConsider;
                        row["LotNumber"] = item.LotNumber;
                        row["SerialNumber"] = item.SerialNumber;
                        row["ConsignedQuantity"] = item.ConsignedQuantity.HasValue ? (object)item.ConsignedQuantity.Value : DBNull.Value;
                        row["CustomerOwnedQuantity"] = item.CustomerOwnedQuantity.HasValue ? (object)item.CustomerOwnedQuantity.Value : DBNull.Value;
                        row["ReceiptCost"] = item.Cost.HasValue ? (object)item.Cost.Value : DBNull.Value;
                        row["UDF1"] = item.UDF1;
                        row["UDF2"] = item.UDF2;
                        row["UDF3"] = item.UDF3;
                        row["UDF4"] = item.UDF4;
                        row["UDF5"] = item.UDF5;
                        row["ProjectSpend"] = string.Empty;
                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }

        public void SeteVMICountHeader(Int64 CompanyID, Int64 RoomID, string CountName, string CountType, Int64 UserID, string AddedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CountName", CountName),
                                                   new SqlParameter("@CountType", CountType),
                                                   new SqlParameter("@CreatedBy", UserID),
                                                   new SqlParameter("@AddedFrom", AddedFrom) };

                context.Database.ExecuteSqlCommand("Exec [eVMICountValidate] @CompanyID,@RoomID,@CountName,@CountType,@CreatedBy,@AddedFrom", params1);
            }
        }

        public List<InventoryCountDetailDTO> GetInventoryCountDetailsbyGuids(string CDGuids, Int64 CompanyID, Int64 RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CDGuids", CDGuids)
                                                 , new SqlParameter("@CompanyID", CompanyID)
                                                 , new SqlParameter("@RoomID", RoomID)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDetailDTO>("exec [GetInventoryCountDetailsbyGuids] @CDGuids,@CompanyID,@RoomID", params1).ToList();
            }
        }

        public void SendMailForApplyCount(List<InventoryCountDetailDTO> lstInventoryCountDetailGuids, string EnterPriseDBName, Int64 EnterPriceID, string CompanyName, Int64 CompanyID,
                string RoomName, Int64 RoomID, string UserName)
        {
            if (lstInventoryCountDetailGuids != null && lstInventoryCountDetailGuids.Count > 0)
            {
                AlertMail objAlertMail = new AlertMail();
                eMailMasterDAL objEmailDAL = null;
                EmailTemplateDAL objEmailTemplateDAL = null;
                EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
                List<eMailAttachmentDTO> objeMailAttchList = null;
                NotificationDAL objNotificationDAL = null;
                EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(EnterPriseDBName).GetEnterprise(EnterPriceID);
                List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
                List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
                InventoryCountDAL inventoryCountDAL = new InventoryCountDAL(EnterPriseDBName);
                string cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(EnterPriseDBName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }

                try
                {
                    string CDGuids = string.Join(",", lstInventoryCountDetailGuids.Select(x => x.GUID).ToList());
                    List<InventoryCountDetailDTO> lstInventoryCountDetails = inventoryCountDAL.GetInventoryCountDetailsbyGuids(CDGuids, CompanyID, RoomID);
                    if (lstInventoryCountDetails != null && lstInventoryCountDetails.Count > 0)
                    {
                        objNotificationDAL = new NotificationDAL(EnterPriseDBName);
                        lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.CountApplyMail, RoomID, CompanyID, cultureCode);
                        lstNotifications.ForEach(t =>
                        {
                            if (t.SchedulerParams.ScheduleMode == 5)
                            {
                                lstNotificationsImidiate.Add(t);
                            }
                        });

                        if (lstNotificationsImidiate.Count > 0)
                        {
                            lstNotificationsImidiate.ForEach(t =>
                            {
                                string StrSubject = string.Empty;
                                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                                {
                                    StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                                }
                                string strToAddress = t.EmailAddress;

                                string QtyFormat = "N";
                                string dateFormate = "MM/dd/yyyy";

                                if (t.objeTurnsRegionInfo != null && t.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                                    QtyFormat += t.objeTurnsRegionInfo.NumberDecimalDigits;

                                if (t.objeTurnsRegionInfo != null
                                && !string.IsNullOrEmpty(t.objeTurnsRegionInfo.ShortDatePattern))
                                    dateFormate = t.objeTurnsRegionInfo.ShortDatePattern;

                                if (!string.IsNullOrEmpty(strToAddress))
                                {
                                    objEmailDAL = new eMailMasterDAL(EnterPriseDBName);
                                    StringBuilder MessageBody = new StringBuilder();
                                    objEmailTemplateDAL = new EmailTemplateDAL(EnterPriseDBName);
                                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                                    if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                                    {
                                        objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                                    }
                                    if (objEmailTemplateDetailDTO != null)
                                    {
                                        MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                        StrSubject = objEmailTemplateDetailDTO.MailSubject;
                                    }
                                    else
                                    {
                                        return;
                                    }
                                    MessageBody.Replace("@@COUNTNAME@@", lstInventoryCountDetails.FirstOrDefault().CountName);
                                    MessageBody.Replace("@@COUNTDATE@@", lstInventoryCountDetails.FirstOrDefault().CountDate.ToString(dateFormate));
                                    string stratatTABLEatatTag = GetMailBodyForCountApply(lstInventoryCountDetails, t, EnterPriceID, CompanyID, RoomID, cultureCode);

                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                    objeMailAttchList = new List<eMailAttachmentDTO>();
                                    MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                    MessageBody.Replace("@@USERNAME@@", UserName);
                                    MessageBody.Replace("@@COMPANYNAME@@", CompanyName);
                                    Dictionary<string, string> Params = new Dictionary<string, string>();
                                    Params.Add("DataGuids", lstInventoryCountDetails.FirstOrDefault().InventoryCountGUID.ToString());
                                    objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                    objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                                }
                            });
                        }
                    }
                }
                finally
                {
                    objEmailDAL = null;
                    objEmailTemplateDAL = null;
                    objEmailTemplateDetailDTO = null;
                    objeMailAttchList = null;
                }
            }
        }

        private string GetMailBodyForCountApply(List<InventoryCountDetailDTO> LstCountDtl, NotificationDTO objNotification, long EnterpriseId, long CompanyId, long RoomId, string CultureCode)
        {
            var inventoryCountResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResInventoryCountDetail", CultureCode, EnterpriseId, CompanyId);
            var itemResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", CultureCode, EnterpriseId, CompanyId);
            string binNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("BinNumber", inventoryCountResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResInventoryCountDetail", CultureCode);
            string consignedDifference = ResourceRead.GetResourceValueByKeyAndFullFilePath("ConsignedDifference", inventoryCountResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResInventoryCountDetail", CultureCode);
            string cusOwnedDifference = ResourceRead.GetResourceValueByKeyAndFullFilePath("CusOwnedDifference", inventoryCountResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResInventoryCountDetail", CultureCode);
            string isApplied = ResourceRead.GetResourceValueByKeyAndFullFilePath("IsApplied", inventoryCountResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResInventoryCountDetail", CultureCode);
            string appliedDate = ResourceRead.GetResourceValueByKeyAndFullFilePath("AppliedDate", inventoryCountResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResInventoryCountDetail", CultureCode);
            string itemNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", itemResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);

            string AlterNativeRowStyle = "Style='background:#DBD9D9;'";
            string htmlTabl = string.Empty;
            htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0"" cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + itemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + binNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + consignedDifference + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + cusOwnedDifference + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + isApplied + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + appliedDate + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";

            string trs = "";
            //List<InventoryCountDetailDTO> LstCountDtl = new List<InventoryCountDetailDTO>();
            //InventoryCountDAL objCountDal = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            //LstCountDtl = objCountDal.GetAllLineItemsWithinCount(objInventoryCount.ID);

            if (LstCountDtl != null && LstCountDtl.Count() > 0)
            {
                string QtyFormat = "N";
                string dateFormate = "MM/dd/yyyy";


                if (objNotification.objeTurnsRegionInfo != null && objNotification.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotification.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotification.objeTurnsRegionInfo != null
                    && !string.IsNullOrEmpty(objNotification.objeTurnsRegionInfo.ShortDatePattern))
                    dateFormate = objNotification.objeTurnsRegionInfo.ShortDatePattern;

                int cntrow = 1;
                foreach (var item in LstCountDtl)
                {
                    string RowStyle = string.Empty;
                    if (cntrow % 2 == 0)
                    {
                        RowStyle = AlterNativeRowStyle;
                    }
                    trs += @"<tr " + RowStyle + @" >
                               <td>
                                   " + item.ItemNumber + @"
                               </td>
                               <td>
                                   " + (string.IsNullOrEmpty(item.BinNumber) ? "&nbsp;" : item.BinNumber) + @"
                               </td>
                               <td>
                                   " + (item.ConsignedDifference.HasValue ? item.ConsignedDifference.Value.ToString(QtyFormat) : "NA") + @"
                               </td>
                               <td>
                                   " + (item.CusOwnedDifference.HasValue ? item.CusOwnedDifference.Value.ToString(QtyFormat) : "NA") + @"
                               </td>
                                <td>
                                   " + (item.IsApplied ? "True" : "False") + @"
                               </td>
                               <td>
                                    " + (item.AppliedDate.GetValueOrDefault(DateTime.MinValue).ToString(dateFormate)) + @"
                               </td>
                             </tr>";
                    cntrow += 1;
                }
            }
            else
            {
                var commonResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", CultureCode, EnterpriseId, CompanyId);
                //if Not Data Found 
                string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", commonResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResCommon", CultureCode);
                string RowStyle = string.Empty;
                trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                   </tr>";
            }

            htmlTabl = htmlTabl.Replace("##TRS##", trs);
            return htmlTabl;
        }

        public InventoryCountDTO InsertPollCount(long RoomID, long CompanyID, long UserID, string EditedFrom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@userid", UserID), new SqlParameter("@editedfrom", (EditedFrom ?? (object)DBNull.Value)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryCountDTO>("exec [InsertPollCount] @RoomID,@CompanyID,@userid,@editedfrom", params1).FirstOrDefault();
            }
        }

    }// class
}


