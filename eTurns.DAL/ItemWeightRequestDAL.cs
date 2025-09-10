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
    public partial class ItemWeightRequestDAL : eTurnsBaseDAL
    {
        public ItemWeightRequestDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public InsertWeightPerPieceResult InsertItemWeightPerPieceRequest(ItemWeightPerPieceRequestDTO objDTO)
        {
            //Int64 flag = 1;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", objDTO.ItemGUID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@IsWeightStarted", objDTO.IsWeightStarted),
                                                   new SqlParameter("@TotalQty", objDTO.TotalQty ?? (object)DBNull.Value),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                };

                var res = context.Database.SqlQuery<InsertWeightPerPieceResult>("EXEC [InsertItemWeightPerPieceRequest] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
                objDTO.ID = res.ID;
                return res;
            }
        }

        //public List<ItemWeightPerPieceRequestDTO> GetItemWeightPerPieceRequest(bool IsWeightStarted, bool IsWeightCompleted)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var params1 = new SqlParameter[] { new SqlParameter("@IsWeightStarted", IsWeightStarted),
        //                                           new SqlParameter("@IsWeightCompleted", IsWeightCompleted) };

        //        return context.Database.SqlQuery<ItemWeightPerPieceRequestDTO>("exec [GetIemWeightPerPieceRequest] @IsWeightStarted,@IsWeightCompleted", params1).ToList();

        //    }
        //}

        public List<IWPPieceRequestDTO> GetItemWeightPerPieceRequestByRoom(
            long roomID, long companyID,
            bool IsWeightStarted, bool IsWeightCompleted   ,
            long? ID
            )
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@IsWeightStarted", IsWeightStarted),
                    new SqlParameter("@IsWeightCompleted", IsWeightCompleted),
                    new SqlParameter("@ID", ID.ToDBNull())
                };

                var list = context.Database.SqlQuery<IWPPieceRequestDTO>("exec [GetItemWeightPerPieceRequestByRoom] " + params1.ToSQLParaNameCSV(), params1).ToList();
                return list;
            }
        }


        public bool UpdateItemWeightPerPieceRequest(ItemWeightPerPieceRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objDTO.ID),
                                                   new SqlParameter("@IsWeightStarted", objDTO.IsWeightStarted),
                                                   new SqlParameter("@WeightStartTime", objDTO.WeightStartTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsWeightCompleted", objDTO.IsWeightCompleted),
                                                   new SqlParameter("@WeightCompletionTime", objDTO.WeightCompletionTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@WeightReading", objDTO.WeightReading ?? (object)DBNull.Value),
                                                   new SqlParameter("@ErrorDescription", objDTO.ErrorDescription ?? (object)DBNull.Value),
                                                   new SqlParameter("@ItemWeightPerPiece", objDTO.ItemWeightPerPiece ?? (object)DBNull.Value),
                                                   new SqlParameter("@UpdatedBy", objDTO.UpdatedBy ?? (object)DBNull.Value)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateItemWeightPerPieceRequest] @ID,@IsWeightStarted,@WeightStartTime,@IsWeightCompleted,@WeightCompletionTime,@WeightReading,@ErrorDescription,@ItemWeightPerPiece,@UpdatedBy", params1);
                return true;
            }
        }

        public IEnumerable<ItemWeightPerPieceRequestDTO> GetItemWeightPerPieceRequestRecordsByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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

                List<ItemWeightPerPieceRequestDTO> lstcats = context.Database.SqlQuery<ItemWeightPerPieceRequestDTO>("exec [GetItemWeightPerPieceRequestPagedRecord] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@Room,@CompanyID", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;

            }

        }

        public IEnumerable<CommonDTO> DB_GetItemWeightPerPieceRequestListNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC  [GetItemWeightPerPieceRequestNarrowSearchData] ";
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

        public List<ItemWeightPerPieceRequestRooms> GetItemWeightPerPieceRequestRooms()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemWeightPerPieceRequestRooms> list = context.Database.SqlQuery<ItemWeightPerPieceRequestRooms>
                    ("exec GetItemWeightPerPieceRequestRooms").ToList();
                return list;
            }
        }
    }
}
