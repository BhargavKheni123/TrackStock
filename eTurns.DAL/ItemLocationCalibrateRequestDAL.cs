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
    public partial class ItemLocationCalibrateRequestDAL : eTurnsBaseDAL
    {
        public ItemLocationCalibrateRequestDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public bool InsertItemLocationCalibrateRequest(ItemLocationCalibrateRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", objDTO.ItemGUID),
                                                   new SqlParameter("@BinID", objDTO.BinID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@IsStep1Started", objDTO.IsStep1Started),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                                                   new SqlParameter("@CalibrationWeight", objDTO.CalibrationWeight ?? (object) DBNull.Value),
                                                };

                long id = context.Database.SqlQuery<long>("EXEC [InsertItemLocationCalibrateRequest] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
                objDTO.ID = id;
                return true;
            }
        }

      

        public bool InsertItemLocationCalibrateAllRequest(ItemLocationCalibrateRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [InsertItemLocationCalibrateAllRequest] @RoomID,@CompanyID,@RequestType,@CreatedBy", params1);
                return true;
            }
        }

        private List<ItemLocationCalibrateRequestRooms> GetItemLocationCalibrateRooms(bool isStep1, bool isStep2, bool isStep3)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IsStep1", isStep1),
                                                   new SqlParameter("@IsStep2", isStep2),
                                                   new SqlParameter("@IsStep3", isStep3)
                                                };

                List<ItemLocationCalibrateRequestRooms> list = context.Database.SqlQuery<ItemLocationCalibrateRequestRooms>
                    ("exec GetItemLocationCalibrateRooms " + params1.ToSQLParaNameCSV(), params1).ToList();
                return list;
            }
        }

        public List<ItemLocationCalibrateRequestRooms> GetItemLocationCalibrateStep1Rooms()
        {
            return GetItemLocationCalibrateRooms(true , false, false);
        }

        public List<ItemLocationCalibrateRequestRooms> GetItemLocationCalibrateStep2Rooms()
        {
            return GetItemLocationCalibrateRooms(false, true, false);
        }

        public List<ItemLocationCalibrateRequestRooms> GetItemLocationCalibrateStep3Rooms()
        {
            return GetItemLocationCalibrateRooms(false, false, true);
        }

        List<ILCalibrateRequestDTO> GetItemLocationCalibrateRequestByRoom(
           long roomID, long companyID,
           bool isStep1, bool isStep2, bool isStep3, int PageNo,long ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@IsStep1", isStep1),
                    new SqlParameter("@IsStep2", isStep2),
                    new SqlParameter("@IsStep3", isStep3),
                    new SqlParameter("@ID", ID),                    
                    //new SqlParameter("@PageNo", PageNo)                    
                };

                var list = context.Database.SqlQuery<ILCalibrateRequestDTO>("exec [GetItemLocationCalibrateRequestByRoom] " + params1.ToSQLParaNameCSV(), params1).ToList();
                return list;
            }
        }

        public bool UpdateItemLocationCalibrateRequest(ItemLocationCalibrateRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objDTO.ID),
                    new SqlParameter("@CalibrationWeight", objDTO.CalibrationWeight.ToDBNull()),
                    new SqlParameter("@IsStep1Started", objDTO.IsStep1Started),
                    new SqlParameter("@Step1StartTime", objDTO.Step1StartTime.ToDBNull()),
                    new SqlParameter("@IsStep1Completed", objDTO.IsStep1Completed),
                    new SqlParameter("@Step1CompletionTime", objDTO.Step1CompletionTime.ToDBNull()),
                    new SqlParameter("@IsStep2Started", objDTO.IsStep2Started),
                    new SqlParameter("@Step2StartTime", objDTO.Step2StartTime.ToDBNull()),
                    new SqlParameter("@IsStep2Completed", objDTO.IsStep2Completed),
                    new SqlParameter("@Step2CompletionTime", objDTO.Step2CompletionTime.ToDBNull()),
                    new SqlParameter("@IsStep3Started", objDTO.IsStep3Started),
                    new SqlParameter("@Step3StartTime", objDTO.Step3StartTime.ToDBNull()),
                    new SqlParameter("@IsStep3Completed", objDTO.IsStep3Completed),
                    new SqlParameter("@Step3CompletionTime", objDTO.Step3CompletionTime.ToDBNull()),
                    new SqlParameter("@ErrorDescription", objDTO.ErrorDescription.ToDBNull()),
                    new SqlParameter("@UpdatedBy", objDTO.UpdatedBy)
                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateItemLocationCalibrateRequest] " + params1.ToSQLParaNameCSV(), params1);
                return true;
            }
        }

        public List<ILCalibrateRequestDTO> GetILCalibrateStep1RequestByRoom(
           long roomID, long companyID,
           int PageNo, long ID)
        {
            return GetItemLocationCalibrateRequestByRoom(roomID,companyID,true,false,false,PageNo,ID);
        }

        public List<ILCalibrateRequestDTO> GetILCalibrateStep2RequestByRoom(
           long roomID, long companyID,
           int PageNo, long ID)
        {
            return GetItemLocationCalibrateRequestByRoom(roomID, companyID, false, true, false, PageNo,ID);
        }

        public List<ILCalibrateRequestDTO> GetILCalibrateStep3RequestByRoom(
                   long roomID, long companyID,
                   int PageNo, long ID)
        {
            return GetItemLocationCalibrateRequestByRoom(roomID, companyID, false, false, true, PageNo,ID);
        }

        public List<ItemLocationCalibrateRequestDTO> GetCalibrateRequestListByPaging(Int32 StartRowIndex, Int32 MaxRows,
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

                List<ItemLocationCalibrateRequestDTO> list = context.Database.SqlQuery<ItemLocationCalibrateRequestDTO>(
                    "exec [GetItemLocationCalibrateRequestList] @StartRowIndex,@MaxRows,@Room,@CompanyID,@SearchTerm,@sortColumnName,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo", params1).ToList();
                TotalCount = 0;

                if (list != null && list.Count > 0)
                {
                    TotalCount = list.First().TotalRecords ?? 0;

                    foreach (ItemLocationCalibrateRequestDTO dto in list)
                    {
                        dto.Step1StartTimeStr = FnCommon.ConvertDateByTimeZone(dto.Step1StartTime, true, false);
                        dto.Step1CompletionTimeStr = FnCommon.ConvertDateByTimeZone(dto.Step1CompletionTime, true, false);

                        dto.Step2StartTimeStr = FnCommon.ConvertDateByTimeZone(dto.Step2StartTime, true, false);
                        dto.Step2CompletionTimeStr = FnCommon.ConvertDateByTimeZone(dto.Step2CompletionTime, true, false);


                        dto.Step3StartTimeStr = FnCommon.ConvertDateByTimeZone(dto.Step3StartTime, true, false);
                        dto.Step3CompletionTimeStr = FnCommon.ConvertDateByTimeZone(dto.Step3CompletionTime, true, false);


                        dto.CreatedStr = FnCommon.ConvertDateByTimeZone(dto.Created, true, false);
                        dto.UpdatedStr = FnCommon.ConvertDateByTimeZone(dto.Updated, true, false);
                    }
                }

                return list;

            }
        }

        public CalibrateNarrowSearchData GetCalibrateListNarrowSearchData(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID)
                };


                List<NarrowSearchFieldDTO> objList = context.Database.SqlQuery<NarrowSearchFieldDTO>(
                    "exec [GetCalibrateListNarrowSearchData] @CompanyID,@RoomID", params1).ToList();

                //return objList;
                CalibrateNarrowSearchData objNarrowSearch = new CalibrateNarrowSearchData();
                objNarrowSearch.CreatedByList = objList.Where(t => t.NSFieldName == "CreatedByCount").ToList();
                objNarrowSearch.UpdatedByList = objList.Where(t => t.NSFieldName == "UpdatedByCount").ToList();

                return objNarrowSearch;
            }

        }


    }// class
}
