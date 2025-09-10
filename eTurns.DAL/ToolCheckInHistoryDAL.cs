using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class ToolCheckInHistoryDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ToolCheckInHistoryDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolCheckInHistoryDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [New Methods]

        public ToolCheckInHistoryDTO GetToolCheckInByGUIDFull(Guid TCGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCGUID", TCGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInHistoryDTO>("exec [GetToolCheckInByGUIDFull] @TCGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public ToolCheckInHistoryDTO GetToolCheckInByGUIDPlain(Guid TCGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCGUID", TCGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInHistoryDTO>("exec [GetToolCheckInByGUIDPlain] @TCGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public ToolCheckInHistoryDTO GetToolCheckInByIDFull(long TCID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCID", TCID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInHistoryDTO>("exec [GetToolCheckInByIDFull] @TCID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public ToolCheckInHistoryDTO GetToolCheckInByIDPlain(long TCID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCID", TCID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInHistoryDTO>("exec [GetToolCheckInByIDPlain] @TCID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<ToolCheckInHistoryDTO> GetToolCheckInsByTCIOHGUIDFull(Guid TCIOHGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCIOHGUID", TCIOHGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInHistoryDTO>("exec [GetToolCheckInsByTCIOHGUIDFull] @TCIOHGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<ToolCheckInHistoryDTO> GetToolCheckInsByTCIOHGUIDPlain(Guid TCIOHGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCIOHGUID", TCIOHGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInHistoryDTO>("exec [GetToolCheckInsByTCIOHGUIDPlain] @TCIOHGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public IEnumerable<ToolCheckInHistoryDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid CheckInCheckOutGUID)
        {
            //Get Cached-Media
            //IEnumerable<ToolCheckInHistoryDTO> ObjCache = GetCachedData(RoomID, CompanyID).Where(x => x.CheckInCheckOutGUID == CheckInCheckOutGUID);
            IEnumerable<ToolCheckInHistoryDTO> ObjCache = GetToolCheckInsByTCIOHGUIDFull(CheckInCheckOutGUID, RoomID, CompanyID);
            IEnumerable<ToolCheckInHistoryDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));
            
            if (ObjCache != null && ObjCache.Any() && ObjCache.Count() > 0 && !string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrWhiteSpace(sortColumnName)
                && sortColumnName.Contains("CheckedOutMQTY"))
            {
                sortColumnName = ObjCache.ToList()[0].CheckedOutQTY.GetValueOrDefault(0) > 0  ? sortColumnName.Replace("CheckedOutMQTY", "CheckedOutQTY") : sortColumnName;
            }

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ToolCheckInHistoryDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ToolCheckInHistoryDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public Int64 Insert(ToolCheckInHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolCheckInHistory obj = new ToolCheckInHistory();
                obj.ID = 0;
                obj.CheckInCheckOutGUID = objDTO.CheckInCheckOutGUID;
                obj.CheckOutStatus = objDTO.CheckOutStatus;
                obj.CheckedOutQTY = objDTO.CheckedOutQTY;
                obj.CheckedOutMQTY = objDTO.CheckedOutMQTY;
                obj.CheckOutDate = objDTO.CheckOutDate;
                obj.CheckInDate = objDTO.CheckInDate;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.CheckedOutQTYCurrent = objDTO.CheckedOutQTYCurrent;
                obj.CheckedOutMQTYCurrent = objDTO.CheckedOutMQTYCurrent;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.TechnicianGuid = objDTO.TechnicianGuid;
                obj.SerialNumber = objDTO.SerialNumber ?? string.Empty;
                obj.ToolDetailGUID = objDTO.ToolDetailGUID;
                context.ToolCheckInHistories.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                return obj.ID;
            }

        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ToolCheckInHistory SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);




                return true;
            }
        }
        public List<ToolCheckInHistoryDTO> GetRecordByToolDetailGUID(Guid? ToolDetailGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolDetailGUID", ToolDetailGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ToolCheckInHistoryDTO>("exec [GetToolCheckInHistoryByToolDetailGUID] @ToolDetailGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public IEnumerable<ToolCheckInHistoryDTO> GetPagedRecordsNew(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid CheckInCheckOutGUID)
        {
            //Get Cached-Media
            IEnumerable<ToolCheckInHistoryDTO> ObjCache = GetCachedDataNew(RoomID, CompanyID).Where(x => x.CheckInCheckOutGUID == CheckInCheckOutGUID);
            IEnumerable<ToolCheckInHistoryDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ToolCheckInHistoryDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ToolCheckInHistoryDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public IEnumerable<ToolCheckInHistoryDTO> GetCachedDataNew(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            //  if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[]
                                  { new SqlParameter("@CompanyID", CompanyID )
                                    , new SqlParameter("@RoomID", RoomID ) };
                    IEnumerable<ToolCheckInHistoryDTO> obj = (from u in context.Database.SqlQuery<ToolCheckInHistoryDTO>("EXEC GetCheckInDataByRoom @CompanyID,@RoomID", params1)
                                                              select new ToolCheckInHistoryDTO
                                                              {
                                                                  ID = u.ID,
                                                                  GUID = u.GUID,
                                                                  CheckInCheckOutGUID = u.CheckInCheckOutGUID,
                                                                  CheckOutStatus = u.CheckOutStatus,
                                                                  CheckedOutQTY = u.CheckedOutQTY,
                                                                  CheckedOutMQTY = u.CheckedOutMQTY,
                                                                  CheckOutDate = u.CheckOutDate,
                                                                  CheckInDate = u.CheckInDate,
                                                                  Created = u.Created,
                                                                  CreatedBy = u.CreatedBy,
                                                                  Updated = u.Updated,
                                                                  LastUpdatedBy = u.LastUpdatedBy,
                                                                  Room = u.Room,
                                                                  IsArchived = u.IsArchived,
                                                                  IsDeleted = u.IsDeleted,
                                                                  CompanyID = u.CompanyID,
                                                                  UDF1 = u.UDF1,
                                                                  UDF2 = u.UDF2,
                                                                  UDF3 = u.UDF3,
                                                                  UDF4 = u.UDF4,
                                                                  UDF5 = u.UDF5,
                                                                  CheckedOutQTYCurrent = u.CheckedOutQTYCurrent,
                                                                  CheckedOutMQTYCurrent = u.CheckedOutMQTYCurrent,
                                                                  CreatedByName = u.CreatedByName,
                                                                  UpdatedByName = u.UpdatedByName,
                                                                  RoomName = u.RoomName,
                                                                  AddedFrom = u.AddedFrom,
                                                                  EditedFrom = u.EditedFrom,
                                                                  ReceivedOn = u.ReceivedOn,
                                                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                                                  Technician = u.Technician != null ? u.Technician : string.Empty,
                                                                  TechnicianCode = u.TechnicianCode,
                                                                  ToolDetailGUID = u.ToolDetailGUID,
                                                                  SerialNumber = u.SerialNumber ?? string.Empty
                                                              }).AsParallel().ToList();
                    return obj;

                }
            }


        }

        #endregion
    }
}


