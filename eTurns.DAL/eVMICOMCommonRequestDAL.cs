using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class eVMICOMCommonRequestDAL : eTurnsBaseDAL
    {
        public eVMICOMCommonRequestDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public bool InsertEVMICOMCommonRequest(eVMICOMCommonRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", objDTO.ItemGUID),
                                                   new SqlParameter("@BinID", objDTO.BinID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                                                   new SqlParameter("@ScaleID", objDTO.ScaleID),
                                                   new SqlParameter("@ComPortRoomMappingID", objDTO.ComPortRoomMappingID),
                                                   new SqlParameter("@ModelNumber", objDTO.ModelNumber.ToDBNull()),
                                                };

                long Id = context.Database.SqlQuery<long>("EXEC [InsertEVMICOMCommonRequest] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
                objDTO.ID = Id;

                return true;
            }
        }

        public bool UpdateCOMCommonRequest(eVMICOMCommonRequestDTO objDTO)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objDTO.ID),
                                                   new SqlParameter("@IsComReqStarted", objDTO.IsComReqStarted),
                                                   new SqlParameter("@ComStartTime", objDTO.ComStartTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsComCompleted", objDTO.IsComCompleted),
                                                   new SqlParameter("@ComCompletionTime", objDTO.ComCompletionTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@ErrorDescription", objDTO.ErrorDescription ?? (object)DBNull.Value),
                                                   new SqlParameter("@UpdatedBy", objDTO.UpdatedBy),
                                                   new SqlParameter("@Version", objDTO.Version.ToDBNull()),
                                                   new SqlParameter("@SerialNumber", objDTO.SerialNumber.ToDBNull()),
                                                   new SqlParameter("@ModelNumber", objDTO.ModelNumber.ToDBNull())
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateEVMICOMCommonRequest] " + params1.ToSQLParaNameCSV(), params1);
                return true;
            }

        }

        public List<eVMICalRequestRooms> GetCOMCommonRequestRooms(eVMICOMCommonRequestType requestType)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RequestType", (int)requestType) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<eVMICalRequestRooms> list = context.Database.SqlQuery<eVMICalRequestRooms>
                    ("exec GetEVMICOMCommonRequestRooms " + params1.ToSQLParaNameCSV(), params1).ToList();

                return list;
            }
        }

        public List<eVMICommonReqDTO> GetCOMCommonRequestByRoom(eVMICOMCommonRequestType requestType,long roomID, long companyID, bool IsComReqStarted,
            bool IsComCompleted, long? ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                     new SqlParameter("@RequestType", (int)requestType),
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@IsComReqStarted", IsComReqStarted),
                    new SqlParameter("@IsComCompleted", IsComCompleted),
                    
                    new SqlParameter("@ID", ID.ToDBNull()),
                };

                var list = context.Database.SqlQuery<eVMICommonReqDTO>("exec [GetEVMICOMCommonRequestByRoom] " 
                    + params1.ToSQLParaNameCSV(), params1).ToList();

                return list;
            }
        }


        public IEnumerable<eVMICOMCommonRequestDTO> GeteVMICOMCommonRequestRecordsByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string strQuer = "";
            string Creaters = "";
            string Updators = "";
            string RequestTypes="";
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
                    string[] arrrequesttype = FieldsPara[2].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrrequesttype)
                    {
                        RequestTypes += item + ",";
                    }
                    RequestTypes = RequestTypes.Replace("@", "");
                    RequestTypes = RequestTypes.TrimEnd(',');
                }
                if (FieldsPara.Length > 3 && !string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (FieldsPara.Length > 4 && !string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[4].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[4].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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
                    new SqlParameter("@RequestTypes", RequestTypes),
                    new SqlParameter("@CreatedDateFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedDateTo", CreatedDateTo),
                    new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedDateTo", UpdatedDateTo),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@CompanyID", CompanyId) };

                List<eVMICOMCommonRequestDTO> lstcats = context.Database.SqlQuery<eVMICOMCommonRequestDTO>("exec [GeteVMICOMCommonPagedRecord] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@Creaters,@Updators,@RequestTypes,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@Room,@CompanyID", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;

            }

        }

        public IEnumerable<CommonDTO> DB_GeteVMICOMRequestListNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC  [GeteVMICOMRequestListNarrowSearchData] ";
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
