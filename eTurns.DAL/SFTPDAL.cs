using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class SFTPDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public SFTPDAL(base.DataBaseName)
        //{

        //}

        public SFTPDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public SFTPDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<FTPMasterDTO> GetAllFtpForRoom(long RoomId, long CompanyId)
        {
            List<FTPMasterDTO> lstFtpServers = new List<FTPMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstFtpServers = (from fm in context.FTPMasters
                                 where fm.RoomId == RoomId && fm.CompanyId == CompanyId && fm.IsDeleted == false
                                 select new FTPMasterDTO
                                 {
                                     CompanyId = fm.CompanyId,
                                     CreatedBy = fm.CreatedBy,
                                     Created = fm.Created,
                                     EnterpriseID = fm.EnterpriseID,
                                     GUID = fm.GUID,
                                     ID = fm.ID,
                                     IsDeleted = fm.IsDeleted,
                                     Password = fm.Password,
                                     Port = fm.Port,
                                     RoomId = fm.RoomId,
                                     ServerAddress = fm.ServerAddress,
                                     SFtpName = fm.SFtpName,
                                     UpdatedBy = fm.UpdatedBy,
                                     LastUpdated = fm.LastUpdated,
                                     UserID = fm.UserID,
                                     UserName = fm.UserName,
                                     IsImportFTP = fm.IsImportFTP,
                                     ContactEmail = fm.ContactEmail
                                 }).ToList();
            }
            return lstFtpServers;

        }

        public List<FTPMasterDTO> GetPagedFTPRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<FTPMasterDTO> lstFtps = new List<FTPMasterDTO>();
            TotalCount = 0;
            FTPMasterDTO objFTPMasterDTO = new FTPMasterDTO();
            DataSet dsFtps = new DataSet();
            //string Connectionstring = ConfigurationManager.ConnectionStrings["eTurnsConnection"].ConnectionString;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                Connectionstring = base.DataBaseConnectionString;
            }
            if (Connectionstring == "")
            {
                return lstFtps;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            string FTPCreaters = null;
            string FTPUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;


            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsFtps = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedSFtpItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, FTPCreaters, FTPUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, RoomID, CompanyId);

            }
            else if (SearchTerm.Contains("[###]"))
            {
                //&& ((Fields[1].Split('@')[24] == "") || (Fields[1].Split('@')[24].Split(',').ToList().Contains(t.ReplenishType)))
                //    && ((Fields[1].Split('@')[23] == "") || (Fields[1].Split('@')[23].Split(',').ToList().Contains(t.SupplierId.ToString())))
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                // SearchTerm = string.Empty;
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
                    FTPCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    FTPUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //   CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //   CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");

                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    // UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                dsFtps = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedSFtpItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, FTPCreaters, FTPUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, RoomID, CompanyId);
            }
            else
            {
                dsFtps = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedSFtpItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, FTPCreaters, FTPUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived, RoomID, CompanyId);
            }

            if (dsFtps != null && dsFtps.Tables.Count > 0)
            {
                DataTable dtFtps = dsFtps.Tables[0];
                if (dtFtps.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtFtps.Rows[0]["TotalRecords"]);

                    foreach (DataRow dr in dtFtps.Rows)
                    {
                        long templong = 0;
                        Guid tempguid = Guid.Empty;
                        int tempint = 0;
                        objFTPMasterDTO = new FTPMasterDTO();
                        objFTPMasterDTO.CompanyId = CompanyId;
                        objFTPMasterDTO.RoomId = RoomID;
                        if (dtFtps.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objFTPMasterDTO.ID = templong;
                        }
                        if (dtFtps.Columns.Contains("SFtpName"))
                        {
                            objFTPMasterDTO.SFtpName = Convert.ToString(dr["SFtpName"]);
                        }
                        if (dtFtps.Columns.Contains("ServerAddress"))
                        {
                            objFTPMasterDTO.ServerAddress = Convert.ToString(dr["ServerAddress"]);
                        }
                        if (dtFtps.Columns.Contains("UserName"))
                        {
                            objFTPMasterDTO.UserName = Convert.ToString(dr["UserName"]);
                        }
                        if (dtFtps.Columns.Contains("Port"))
                        {
                            int.TryParse(Convert.ToString(dr["Port"]), out tempint);
                            objFTPMasterDTO.Port = tempint;
                        }
                        if (dtFtps.Columns.Contains("UserID"))
                        {
                            long.TryParse(Convert.ToString(dr["UserID"]), out templong);
                            objFTPMasterDTO.UserID = templong;
                        }
                        if (dtFtps.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objFTPMasterDTO.CreatedBy = templong;
                        }
                        if (dtFtps.Columns.Contains("UpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["UpdatedBy"]), out templong);
                            objFTPMasterDTO.UpdatedBy = templong;
                        }
                        if (dtFtps.Columns.Contains("GUID"))
                        {
                            Guid.TryParse(Convert.ToString(dr["GUID"]), out tempguid);
                            objFTPMasterDTO.GUID = tempguid;
                        }
                        if (dtFtps.Columns.Contains("Created"))
                        {
                            objFTPMasterDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtFtps.Columns.Contains("LastUpdated"))
                        {
                            objFTPMasterDTO.LastUpdated = Convert.ToDateTime(dr["LastUpdated"]);
                        }
                        if (dtFtps.Columns.Contains("IsDeleted"))
                        {
                            objFTPMasterDTO.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
                        }
                        if (dtFtps.Columns.Contains("IsArchived"))
                        {
                            objFTPMasterDTO.IsArchived = Convert.ToBoolean(dr["IsArchived"]);
                        }
                        if (dtFtps.Columns.Contains("CreatedByName"))
                        {
                            objFTPMasterDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                        }
                        if (dtFtps.Columns.Contains("UpdatedByName"))
                        {
                            objFTPMasterDTO.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }
                        if (dtFtps.Columns.Contains("IsImportFTP"))
                        {
                            objFTPMasterDTO.IsImportFTP = Convert.ToBoolean(dr["IsImportFTP"]);
                        }
                        if (dtFtps.Columns.Contains("ContactEmail"))
                        {
                            objFTPMasterDTO.ContactEmail = Convert.ToString(dr["ContactEmail"]);
                        }
                        lstFtps.Add(objFTPMasterDTO);
                    }
                }
            }
            return lstFtps;
        }

        public FTPMasterDTO SaveFTPDetails(FTPMasterDTO objFTPMasterDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                FTPMaster objFTPMaster = null;
                if (objFTPMasterDTO.ID < 1)
                {
                    objFTPMaster = new FTPMaster();
                    objFTPMaster.CompanyId = objFTPMasterDTO.CompanyId;
                    objFTPMaster.CreatedBy = objFTPMasterDTO.CreatedBy;
                    objFTPMaster.Created = DateTimeUtility.DateTimeNow;
                    objFTPMaster.EnterpriseID = objFTPMasterDTO.EnterpriseID;
                    objFTPMaster.GUID = Guid.NewGuid();
                    objFTPMaster.ID = 0;
                    objFTPMaster.IsDeleted = false;
                    objFTPMaster.Password = objFTPMasterDTO.Password;
                    objFTPMaster.Port = objFTPMasterDTO.Port;
                    objFTPMaster.RoomId = objFTPMasterDTO.RoomId;
                    objFTPMaster.ServerAddress = objFTPMasterDTO.ServerAddress;
                    objFTPMaster.SFtpName = objFTPMasterDTO.SFtpName;
                    objFTPMaster.UpdatedBy = objFTPMasterDTO.UpdatedBy;
                    objFTPMaster.LastUpdated = DateTimeUtility.DateTimeNow;
                    objFTPMaster.UserID = objFTPMasterDTO.UserID;
                    objFTPMaster.UserName = objFTPMasterDTO.UserName;
                    objFTPMaster.IsImportFTP = objFTPMasterDTO.IsImportFTP;
                    objFTPMaster.ContactEmail = objFTPMasterDTO.ContactEmail;                    
                    context.FTPMasters.Add(objFTPMaster);
                    context.SaveChanges();
                    objFTPMasterDTO.ID = objFTPMaster.ID;
                    objFTPMasterDTO.GUID = objFTPMaster.GUID;

                }
                else
                {
                    objFTPMaster = context.FTPMasters.FirstOrDefault(t => t.ID == objFTPMasterDTO.ID);
                    if (objFTPMaster != null)
                    {
                        objFTPMaster.Password = objFTPMasterDTO.Password;
                        objFTPMaster.Port = objFTPMasterDTO.Port;
                        objFTPMaster.ServerAddress = objFTPMasterDTO.ServerAddress;
                        objFTPMaster.SFtpName = objFTPMasterDTO.SFtpName;
                        objFTPMaster.UpdatedBy = objFTPMasterDTO.UpdatedBy;
                        objFTPMaster.LastUpdated = DateTimeUtility.DateTimeNow;
                        objFTPMaster.UserName = objFTPMasterDTO.UserName;
                        objFTPMaster.IsImportFTP = objFTPMasterDTO.IsImportFTP;
                        objFTPMaster.ContactEmail = objFTPMasterDTO.ContactEmail;
                        context.SaveChanges();
                        objFTPMasterDTO.ID = objFTPMaster.ID;
                        objFTPMasterDTO.GUID = objFTPMaster.GUID;
                    }
                }
            }
            return objFTPMasterDTO;
        }

        public FTPMasterDTO GetFtpByID(long ID, long RoomId, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from fm in context.FTPMasters
                        where fm.ID == ID && fm.RoomId == RoomId && fm.CompanyId == CompanyID && fm.IsDeleted == false
                        select new FTPMasterDTO
                        {
                            CompanyId = fm.CompanyId,
                            CreatedBy = fm.CreatedBy,
                            Created = fm.Created,
                            EnterpriseID = fm.EnterpriseID,
                            GUID = fm.GUID,
                            ID = fm.ID,
                            IsDeleted = fm.IsDeleted,
                            Password = fm.Password,
                            Port = fm.Port,
                            RoomId = fm.RoomId,
                            ServerAddress = fm.ServerAddress,
                            SFtpName = fm.SFtpName,
                            UpdatedBy = fm.UpdatedBy,
                            LastUpdated = fm.LastUpdated,
                            UserID = fm.UserID,
                            UserName = fm.UserName,
                            IsImportFTP = fm.IsImportFTP,
                            ContactEmail = fm.ContactEmail
                            
                        }).FirstOrDefault();
            }
        }

        public Dictionary<string, string> DeleteFtpRecords(string Ids, long RoomId, long CompanyID)
        {
            Dictionary<string, string> Deleteresut = new Dictionary<string, string>();
            Deleteresut.Add("InUse", string.Empty);
            Deleteresut.Add("Deleted", string.Empty);

            string inUse = string.Empty;
            string Deleted = string.Empty;

            long ftpId = 0;
            string[] ids = Ids.Split(',');
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var id in ids)
                {
                    if (long.TryParse(id, out ftpId))
                    {
                        FTPMaster objFTp = context.FTPMasters.FirstOrDefault(t => t.ID == ftpId);
                        if (objFTp != null)
                        {
                            IQueryable<Notification> lstnots = context.Notifications.Where(t => t.FTPId == ftpId && t.IsDeleted == false);
                            if (lstnots.Any())
                            {
                                inUse = inUse + "," + ftpId;
                            }
                            else
                            {
                                objFTp.IsDeleted = true;
                                objFTp.LastUpdated = DateTimeUtility.DateTimeNow;
                                Deleted = Deleted + "," + ftpId;
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            Deleteresut["InUse"] = inUse;
            Deleteresut["Deleted"] = Deleted;
            return Deleteresut;
        }
        public Dictionary<string, string> UnDeleteFtpRecords(string Ids, long RoomId, long CompanyID)
        {
            Dictionary<string, string> Deleteresut = new Dictionary<string, string>();
            Deleteresut.Add("InUse", string.Empty);
            Deleteresut.Add("Deleted", string.Empty);

            string inUse = string.Empty;
            string Deleted = string.Empty;

            long ftpId = 0;
            string[] ids = Ids.Split(',');
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var id in ids)
                {
                    if (long.TryParse(id, out ftpId))
                    {
                        FTPMaster objFTp = context.FTPMasters.FirstOrDefault(t => t.ID == ftpId);
                        if (objFTp != null)
                        {
                            IQueryable<Notification> lstnots = context.Notifications.Where(t => t.FTPId == ftpId && t.IsDeleted == false);
                            if (lstnots.Any())
                            {
                                inUse = inUse + "," + ftpId;
                            }
                            else
                            {
                                objFTp.IsDeleted = false;
                                objFTp.LastUpdated = DateTimeUtility.DateTimeNow;
                                Deleted = Deleted + "," + ftpId;
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            Deleteresut["InUse"] = inUse;
            Deleteresut["Deleted"] = Deleted;
            return Deleteresut;
        }

        public string FTPDuplicateCheck(long Id, string FtpName, long RoomId, long CompanyId)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.FTPMasters
                           where em.SFtpName == FtpName && em.IsArchived == false && em.IsDeleted == false && em.ID != Id && em.CompanyId == CompanyId && em.RoomId == RoomId
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;
        }

        public string FTPServerDuplicateCheck(long Id, string FtpServer, long RoomId, long CompanyId)
        {
            {
                string msg = "";
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.FTPMasters
                               where em.ServerAddress == FtpServer && em.IsArchived == false && em.IsDeleted == false && em.ID != Id && em.CompanyId == CompanyId && em.RoomId == RoomId
                               select em);
                    if (qry.Any())
                    {
                        msg = "duplicate";
                    }
                    else
                    {
                        msg = "ok";
                    }
                }
                return msg;
            }
            #endregion


        }

        public List<FTPMasterDTO> GetFTPMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FTPMasterDTO>("exec [GetFTPMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
    }
}
