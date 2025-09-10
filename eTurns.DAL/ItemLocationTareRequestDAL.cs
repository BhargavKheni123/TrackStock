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
    public partial class ItemLocationTareRequestDAL : eTurnsBaseDAL
    {
        public ItemLocationTareRequestDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public bool InsertItemLocationTareRequest(ItemLocationTareRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", objDTO.ItemGUID),
                                                   new SqlParameter("@BinID", objDTO.BinID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@IsTareStarted", objDTO.IsTareStarted),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy)
                                                };

                long Id = context.Database.SqlQuery<long>("EXEC [InsertItemLocationTareRequest] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
                objDTO.ID = Id;

                return true;
            }
        }

        //public List<ItemLocationTareRequestDTO> GetItemLocationTareRequest(bool IsTareStarted, bool IsTareCompleted)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var params1 = new SqlParameter[] { new SqlParameter("@IsTareStarted", IsTareStarted),
        //                                           new SqlParameter("@IsTareCompleted", IsTareCompleted) };

        //        return context.Database.SqlQuery<ItemLocationTareRequestDTO>("exec [GetItemLocationTareRequest] @IsTareStarted,@IsTareCompleted", params1).ToList();

        //    }
        //}

        public List<ILTareRequestDTO> GetItemLocationTareRequestByRoom(
            long roomID, long companyID,
            bool IsTareStarted, bool IsTareCompleted, long? ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@IsTareStarted", IsTareStarted),
                    new SqlParameter("@IsTareCompleted", IsTareCompleted),
                    new SqlParameter("@ID", ID.ToDBNull()),          
                    //new SqlParameter("@PageNo", PageNo)                    
                };

                var list = context.Database.SqlQuery<ILTareRequestDTO>("exec [GetItemLocationTareRequestByRoom] " + params1.ToSQLParaNameCSV(), params1).ToList();
                return list;
            }
        }

        public bool UpdateItemLocationTareRequest(ItemLocationTareRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objDTO.ID),
                                                   new SqlParameter("@IsTareStarted", objDTO.IsTareStarted),
                                                   new SqlParameter("@TareStartTime", objDTO.TareStartTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsTareCompleted", objDTO.IsTareCompleted),
                                                   new SqlParameter("@TareCompletionTime", objDTO.TareCompletionTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@ErrorDescription", objDTO.ErrorDescription ?? (object)DBNull.Value),
                                                   new SqlParameter("@UpdatedBy", objDTO.UpdatedBy)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateItemLocationTareRequest] " + params1.ToSQLParaNameCSV(), params1);
                return true;
            }
        }


        public List<ItemLocationTareRequestDTO> GetTareRequestListByPaging(Int32 StartRowIndex, Int32 MaxRows, 
            out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, 
            string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
        
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
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@CompanyID", CompanyId),
                    new SqlParameter("@SearchTerm", SearchTerm),
                    new SqlParameter("@sortColumnName", sortColumnName),
                    new SqlParameter("@Creaters", Creaters),
                    new SqlParameter("@Updators", Updators),
                    new SqlParameter("@CreatedDateFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedDateTo", CreatedDateTo),
                    new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedDateTo", UpdatedDateTo)
                     };

                List<ItemLocationTareRequestDTO> list = context.Database.SqlQuery<ItemLocationTareRequestDTO>(
                    "exec [GetItemLocationTareRequestList] @StartRowIndex,@MaxRows,@Room,@CompanyID,@SearchTerm,@sortColumnName,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo", params1).ToList();
                TotalCount = 0;

                if (list != null && list.Count > 0)
                {
                    TotalCount = list.First().TotalRecords ?? 0;

                    foreach(ItemLocationTareRequestDTO dto in list)
                    {
                        dto.TareStartTimeStr = FnCommon.ConvertDateByTimeZone(dto.TareStartTime, true, false);
                        dto.TareCompletionTimeStr = FnCommon.ConvertDateByTimeZone(dto.TareCompletionTime, true, false);

                        dto.CreatedStr = FnCommon.ConvertDateByTimeZone(dto.Created, true, false);
                        dto.UpdatedStr = FnCommon.ConvertDateByTimeZone(dto.Updated, true, false);                    
                    }
                }

                return list;

            }

        }


        public TareNarrowSearchData GetItemLocationTareListNarrowSearchData(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID)
                };

                
                List<NarrowSearchFieldDTO> objList = context.Database.SqlQuery<NarrowSearchFieldDTO>(
                    "exec [GetItemLocationTareListNarrowSearchData] @CompanyID,@RoomID", params1).ToList();

                //return objList;
                TareNarrowSearchData objNarrowSearch = new TareNarrowSearchData();
                objNarrowSearch.CreatedByList = objList.Where(t => t.NSFieldName == "CreatedByCount").ToList();
                objNarrowSearch.UpdatedByList = objList.Where(t => t.NSFieldName == "UpdatedByCount").ToList();

                return objNarrowSearch;
            }           

        }

        public List<ItemLocationTareRequestRooms> GetItemLocationTareRequestRooms()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemLocationTareRequestRooms> list = context.Database.SqlQuery<ItemLocationTareRequestRooms>
                    ("exec GetItemLocationTareRequestRooms").ToList();
                return list;
            }
        }

        public bool InsertItemLocationTareAllRequest(ItemLocationTareRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [InsertItemLocationTareAllRequest] @RoomID,@CompanyID,@RequestType,@CreatedBy", params1);
                return true;
            }
        }

    }
}
