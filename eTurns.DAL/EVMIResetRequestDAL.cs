using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class EVMIResetRequestDAL : eTurnsBaseDAL
    {

        public EVMIResetRequestDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }


        public bool InsertEVMIResetRequest(eVMIResetRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { 
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                                                   new SqlParameter("@ScaleID", objDTO.ScaleID),
                                                   new SqlParameter("@ComPortRoomMappingID", objDTO.ComPortRoomMappingID)
                                                };

                long Id = context.Database.SqlQuery<long>("EXEC [InsertEVMIResetRequest] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
                objDTO.ID = Id;

                return true;
            }
        }

        public List<eVMIResetRequestRooms> GetResetRequestRooms()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<eVMIResetRequestRooms> list = context.Database.SqlQuery<eVMIResetRequestRooms>
                    ("exec GetResetRequestRooms").ToList();
                return list;
            }
        }


        public List<eVMIResetReqDTO> GetResetRequestByRoom(object roomID, object companyID, bool isResetStarted, 
            bool isResetCompleted, long? ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@IsResetStarted", isResetStarted),
                    new SqlParameter("@IsResetCompleted", isResetCompleted),
                    new SqlParameter("@ID", ID.ToDBNull()),                              
                };

                var list = context.Database.SqlQuery<eVMIResetReqDTO>("exec [GetResetRequestByRoom] " + params1.ToSQLParaNameCSV(), params1).ToList();
                return list;
            }
        }

        public bool UpdateResetRequest(eVMIResetReqDTO objDTO)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objDTO.ID),
                                                   new SqlParameter("@IsResetStarted", objDTO.IsResetStarted),
                                                   new SqlParameter("@ResetStartTime", objDTO.ResetStartTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsResetCompleted", objDTO.IsResetCompleted),
                                                   new SqlParameter("@ResetCompletionTime", objDTO.ResetCompletionTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@ErrorDescription", objDTO.ErrorDescription ?? (object)DBNull.Value),
                                                   new SqlParameter("@UpdatedBy", objDTO.UpdatedBy)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateEVMIResetRequest] " + params1.ToSQLParaNameCSV(), params1);
                return true;
            }

        }

        public IEnumerable<eVMIResetRequestDTO> GetResetRequestRecordsByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string strQuer = "";
            string Creaters = "";
            string Updators = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('~');
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

                    string[] arrCreators = FieldsPara[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrCreators)
                    {
                        Creaters += item + ",";
                    }
                    Creaters = Creaters.Replace("@", "");
                    Creaters = Creaters.TrimEnd(',');


                }
                if (FieldsPara.Length > 1 && !string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    string[] arrCreators = FieldsPara[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrCreators)
                    {
                        Updators += item + ",";
                    }
                    Updators = Updators.Replace("@", "");
                    Updators = Updators.TrimEnd(',');
                }
                if (FieldsPara.Length > 2 && !string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (FieldsPara.Length > 3 && !string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

            }
            else
            {
                SearchTerm = "";
            }


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm),
                    new SqlParameter("@sortColumnName", sortColumnName),
                    new SqlParameter("@Creaters", Creaters),
                    new SqlParameter("@Updators", Updators),
                    new SqlParameter("@CreatedDateFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedDateTo", CreatedDateTo),
                    new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedDateTo", UpdatedDateTo),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@CompanyID", CompanyId) };

                List<eVMIResetRequestDTO> lstcats = context.Database.SqlQuery<eVMIResetRequestDTO>("exec [GeteVMIResetRequestPagedRecord] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@Room,@CompanyID", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;

            }

        }

        public IEnumerable<CommonDTO> DB_GetResetRequestListNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC  [GetResetRequestListNarrowSearchData] ";
                strCommand += CompanyID.ToString();
                strCommand += ", " + RoomID.ToString();
                IEnumerable<CommonDTO> obj = (from u in context.Database.SqlQuery<CommonDTO>(strCommand)
                                              select new CommonDTO
                                              {
                                                  ID = u.ID,
                                                  ControlID = "",
                                                  Count = u.Count,
                                                  PageName = u.PageName,
                                                  Text = u.Text
                                              }).AsParallel().ToList();
                return obj;
            }
        }


    }// class
}
