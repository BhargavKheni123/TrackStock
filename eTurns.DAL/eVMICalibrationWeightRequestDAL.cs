using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class eVMICalibrationWeightRequestDAL : eTurnsBaseDAL
    {
        public eVMICalibrationWeightRequestDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
               

        public List<eVMICalRequestRooms> GetCalRequestRooms(eVMICalibrationWeightRequestType weightRequestType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RequestType", (int)weightRequestType) };
                
                List<eVMICalRequestRooms> list = context.Database.SqlQuery<eVMICalRequestRooms>
                    ("exec GetEVMICalRequestRooms " + params1.ToSQLParaNameCSV() , params1).ToList();
                return list;
            }
        }

        public bool InsertEVMICalibrationWeightRequest(eVMICalibrationWeightRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", objDTO.ItemGUID),
                                                   new SqlParameter("@BinID", objDTO.BinID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@CalibrationWeight", objDTO.CalibrationWeight ?? 0),  
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                                                   new SqlParameter("@ScaleID", objDTO.ScaleID),
                                                   new SqlParameter("@ComPortRoomMappingID", objDTO.ComPortRoomMappingID)
                                                };

                long Id = context.Database.SqlQuery<long>("EXEC [InsertEVMICalibrationWeightRequest] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
                objDTO.ID = Id;

                return true;
            }
        }

        public List<eVMICalReqDTO> GetCalWeightRequestByRoom(long roomID, long companyID, bool isStarted,
            bool isCompleted,int requestType, long? ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@IsCalWeightStarted", isStarted),
                    new SqlParameter("@IsCalWeightCompleted", isCompleted),
                    new SqlParameter("@RequestType", requestType),
                    new SqlParameter("@ID", ID.ToDBNull()),
                };

                var list = context.Database.SqlQuery<eVMICalReqDTO>("exec [GetEVMICalWeightRequestByRoom] " + params1.ToSQLParaNameCSV(), params1).ToList();
                return list;
            }
        }

        public bool UpdateCalWeightRequest(eVMICalibrationWeightRequestDTO objDTO)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objDTO.ID),
                                                   new SqlParameter("@IsCalWeightStarted", objDTO.IsCalWeightStarted),
                                                   new SqlParameter("@CalWeightStartTime", objDTO.CalWeightStartTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsCalWeightCompleted", objDTO.IsCalWeightCompleted),
                                                   new SqlParameter("@CalWeightCompletionTime", objDTO.CalWeightCompletionTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@CalibrationWeight", objDTO.CalibrationWeight),
                                                   new SqlParameter("@ErrorDescription", objDTO.ErrorDescription ?? (object)DBNull.Value),
                                                   new SqlParameter("@UpdatedBy", objDTO.UpdatedBy)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateEVMICalWeightRequest] " + params1.ToSQLParaNameCSV(), params1);
                return true;
            }

        }

        public IEnumerable<eVMICalibrationWeightRequestDTO> GeteVMICalibrationWeightRequestRecordsByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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

                List<eVMICalibrationWeightRequestDTO> lstcats = context.Database.SqlQuery<eVMICalibrationWeightRequestDTO>("exec [GeteVMICalibrationWeightPagedRecord] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@Room,@CompanyID", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;

            }

        }

        public IEnumerable<CommonDTO> DB_GeteVMICalibrationWeightRequestNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC  [GeteVMICalibrationWeightRequestListNarrowSearchData] ";
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
    }
}
