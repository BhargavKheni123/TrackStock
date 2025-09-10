using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurnsMaster.DAL
{
    public partial class EulaMasterDAL : eTurnsMasterBaseDAL
    {
        /// <summary>
        /// Get Paged Records from the Technician Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<EulaMasterDTO> GetAllRecords()
        {

            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                return (from x in context.ExecuteStoreQuery<EulaMasterDTO>(@"SELECT A.* from EulaMaster as A
                                                                                WHERE ISNULL(A.IsArchived,0)!=1 AND ISNULL(A.IsDeleted,0)!=1 ")
                        select new EulaMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            RoomName = x.RoomName,
                            ReceivedOn = x.ReceivedOn,
                            ReceivedOnWeb = x.ReceivedOnWeb,
                            EulaDescription = x.EulaDescription,
                            EulaName = x.EulaName,
                        }).ToList();

            }
        }

        public IEnumerable<EulaMasterDTO> GetAllRecords(bool Isdeleted, bool IsArchieved)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string Isdelete = "0";
                string IsArchieve = "0";
                if (Isdeleted)
                {
                    Isdelete = "1";
                }
                if (IsArchieved)
                {
                    IsArchieve = "1";
                }
                return (from x in context.ExecuteStoreQuery<EulaMasterDTO>(@"SELECT A.ID,A.Created as CreatedOn,A.Updated as UpdatedOn
,A.CreatedBy,A.LastUpdatedBy as UpdatedBy,A.IsDeleted,'' as UpdatedByName ,'' as CreatedByName,A.IsArchived,''as RoomName,A.EulaDescription,A.EulaName,A.EulaFileName
                                FROM			EulaMaster as A
                                    WHERE ISNULL(A.IsArchived,0) =" + IsArchieve + " AND ISNULL(A.IsDeleted,0)= " + Isdelete)
                        select new EulaMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            RoomName = x.RoomName,
                            EulaDescription = x.EulaDescription,
                            EulaName = x.EulaName,
                            EulaFileName = x.EulaFileName
                        }).ToList();

            }
        }

        public EulaMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetAllRecords(IsArchived, IsDeleted).Where(t => t.ID == id).SingleOrDefault();
        }

        public IEnumerable<EulaMasterDTO> GetAllRecords(string DBConnectionstring)
        {

            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                return (from x in context.ExecuteStoreQuery<EulaMasterDTO>(@"SELECT A.* 
                                                                                FROM EulaMaster A
                                                                                WHERE ISNULL(A.IsArchived,0)!=1 AND ISNULL(A.IsDeleted,0)!=1")
                        select new EulaMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            RoomName = x.RoomName,
                            EulaDescription = x.EulaDescription,
                            EulaName = x.EulaName,
                        }).ToList();

            }
        }

        /// <summary>
        /// Get Paged Records from the Technician Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<EulaMasterDTO> GetAllDeletedRecords()
        {

            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                return (from x in context.ExecuteStoreQuery<EulaMasterDTO>(@"SELECT A.*
                                                                                FROM EulaMaster A
                                                                                WHERE ISNULL(A.IsDeleted,0) =1 ")
                        select new EulaMasterDTO
                        {
                            ID = x.ID,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            UpdatedByName = x.UpdatedByName,
                            Action = string.Empty,
                            CreatedByName = x.CreatedByName,
                            HistoryID = 0,
                            IsArchived = x.IsArchived,
                            RoomName = x.RoomName,
                            EulaDescription = x.EulaDescription,
                            EulaName = x.EulaName,
                        }).ToList();

            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] strIds = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strIds)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE EulaMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + " and id != 1 ;";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                return true;
            }
        }

        public bool UnDeleteRecords(string IDs, Int64 userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] strIds = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strIds)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE EulaMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE ID =" + item.ToString() + " ;";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

            }
            return true;
        }

        public EulaMasterDTO GetEulaData(string EulaName)
        {
            EulaMasterDTO objEulaMasterDTO = new EulaMasterDTO();
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objEulaMasterDTO = (from x in context.EulaMasters
                                    where x.EulaName == EulaName && (x.IsDeleted ?? false) == false
                                    select new EulaMasterDTO
                                    {
                                        ID = x.ID,
                                        CreatedOn = x.Created ?? DateTime.MinValue,
                                        UpdatedOn = x.Updated ?? DateTime.MinValue,
                                        CreatedBy = x.CreatedBy ?? 0,
                                        UpdatedBy = x.LastUpdatedBy ?? 0,
                                        IsDeleted = x.IsDeleted,
                                        EulaName = x.EulaName,
                                        EulaDescription = x.EulaDescription,
                                        UpdatedByName = string.Empty,
                                        Action = string.Empty,
                                        CreatedByName = string.Empty,
                                        HistoryID = 0,
                                        IsArchived = x.IsArchived,
                                        ReceivedOn = x.ReceivedOn,
                                        ReceivedOnWeb = x.ReceivedOnWeb
                                    }).FirstOrDefault();
            }

            return objEulaMasterDTO;

        }

        public IEnumerable<EulaMasterDTO> GetEulaList(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<EulaMasterDTO> lstItems = new List<EulaMasterDTO>();
            TotalCount = 0;
            EulaMasterDTO objItemDTO = new EulaMasterDTO();

            string CreatedByName = null;
            string UpdatedByName = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;

            DataSet dsCart = new DataSet();
            string Connectionstring = string.Empty;// DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            sortColumnName = sortColumnName.Replace("CreatedDate", "CreatedOn").Replace("UpdatedDate", "UpdatedOn");
            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetEulaList", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived);
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


                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
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
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetEulaList", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetEulaList", StartRowIndex, MaxRows, SearchTerm, sortColumnName, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, IsDeleted, IsArchived);
            }
            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new EulaMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        EulaName = row.Field<string>("EulaName"),
                        EulaDescription = row.Field<string>("EulaDescription"),
                        CreatedOn = row.Field<DateTime?>("CreatedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("CreatedOn")) : Convert.ToDateTime(row.Field<DateTime?>("CreatedOn")),
                        UpdatedOn = row.Field<DateTime?>("UpdatedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("UpdatedOn")) : Convert.ToDateTime(row.Field<DateTime?>("UpdatedOn")),
                        CreatedBy = row.Field<long>("CreatedBy"),
                        UpdatedBy = row.Field<long>("UpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                    }).ToList();
                }
            }
            return lstItems;
        }
    }
}
