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
    public partial class ItemLocationPollRequestDAL : eTurnsBaseDAL
    {
        public ItemLocationPollRequestDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public bool InsertItemLocationPollRequest(ItemLocationPollRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", objDTO.ItemGUID),
                                                   new SqlParameter("@BinID", objDTO.BinID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@IsPollStarted", objDTO.IsPollStarted),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                                                   new SqlParameter("@CountGUID", objDTO.CountGUID ?? (object)DBNull.Value)
                                                };

                long Id = context.Database.SqlQuery<long>("EXEC [InsertItemLocationPollRequest] @ItemGUID,@BinID,@RoomID,@CompanyID,@RequestType,@IsPollStarted,@CreatedBy,@CountGUID", params1).FirstOrDefault();
                objDTO.ID = Id;
                return true;
            }
        }

        public bool InsertItemLocationPollAllRequest(ItemLocationPollRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RequestType", objDTO.RequestType),
                                                   new SqlParameter("@IsPollStarted", objDTO.IsPollStarted),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                                                   new SqlParameter("@CountGUID", objDTO.CountGUID ?? (object)DBNull.Value)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [InsertItemLocationPollAllRequest] @RoomID,@CompanyID,@RequestType,@IsPollStarted,@CreatedBy,@CountGUID", params1);

                return true;
            }
        }

        //public List<ItemLocationPollRequestDTO> GetItemLocationPollRequest(bool IsPollStarted, bool IsPollCompleted, bool IsPostProcessDone)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var params1 = new SqlParameter[] { new SqlParameter("@IsPollStarted", IsPollStarted),
        //                                           new SqlParameter("@IsPollCompleted", IsPollCompleted),
        //                                           new SqlParameter("@IsPostProcessDone", IsPostProcessDone) };

        //        //return context.Database.SqlQuery<ItemLocationPollRequestDTO>("exec [GetIemLocationPollRequest] @IsPollStarted,@IsPollCompleted,@IsPostProcessDone", params1).ToList();
        //        return context.Database.SqlQuery<ItemLocationPollRequestDTO>("exec [GetItemLocationPollRequest] @IsPollStarted,@IsPollCompleted,@IsPostProcessDone", params1).ToList();

        //    }
        //}

        public List<ILPollRequestDTO> GetItemLocationPollRequestByRoom(
            long roomID, long companyID,
            bool IsPollStarted, bool IsPollCompleted,
            bool IsPostProcessDone, long? ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@IsPollStarted", IsPollStarted),
                    new SqlParameter("@IsPollCompleted", IsPollCompleted),
                    new SqlParameter("@IsPostProcessDone", IsPostProcessDone),
                    new SqlParameter("@ID", ID.ToDBNull()),
                };

                //return context.Database.SqlQuery<ItemLocationPollRequestDTO>("exec [GetIemLocationPollRequest] @IsPollStarted,@IsPollCompleted,@IsPostProcessDone", params1).ToList();
                return context.Database.SqlQuery<ILPollRequestDTO>("exec [GetItemLocationPollRequestByRoom] " + params1.ToSQLParaNameCSV(), params1).ToList();
            }
        }



        public bool UpdateItemLocationPollRequestPollStarted(ItemLocationPollRequestDTO objDTO, string CurrentRoomTimeZone, long UpdatedBy
            , EVMIPollErrorTypeEnum errorType
            )
        {
            objDTO.IsPollStarted = true;
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
            objDTO.PollStartTime = CurrentTimeofTimeZone;
            objDTO.UpdatedBy = UpdatedBy;
            objDTO.ErrorType = errorType.ToString();
            return UpdateItemLocationPollRequest(objDTO);
        }

        public bool UpdateItemLocationPollRequestPollCompleted(ItemLocationPollRequestDTO objDTO, string CurrentRoomTimeZone,
            long UpdatedBy, double weightReading, string errorDescription, EVMIPollErrorTypeEnum errorType, string CommandText)
        {
            objDTO.WeightReading = weightReading;
            objDTO.IsPollCompleted = true; //isPollCompleted;
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
            objDTO.PollCompletionTime = CurrentTimeofTimeZone;
            objDTO.UpdatedBy = UpdatedBy;
            objDTO.ErrorDescription = errorDescription;
            objDTO.ErrorType = errorType.ToString();
            objDTO.CommandText = CommandText;
            return UpdateItemLocationPollRequest(objDTO);
        }


        public bool UpdateItemLocationPollRequestPostProcessDone(ItemLocationPollRequestDTO objDTO, long UpdatedBy, EVMIPollErrorTypeEnum errorType)
        {
            objDTO.UpdatedBy = UpdatedBy;
            objDTO.IsPostProcessDone = true;
            objDTO.ErrorType = errorType.ToString();
            return UpdateItemLocationPollRequest(objDTO);
        }

        public bool UpdateItemLocationPollRequest(ItemLocationPollRequestDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objDTO.ID),
                                                   new SqlParameter("@IsPollStarted", objDTO.IsPollStarted),
                                                   new SqlParameter("@PollStartTime", objDTO.PollStartTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsPollCompleted", objDTO.IsPollCompleted),
                                                   new SqlParameter("@PollCompletionTime", objDTO.PollCompletionTime ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsPostProcessDone", objDTO.IsPostProcessDone),
                                                   new SqlParameter("@WeightReading", objDTO.WeightReading),
                                                   new SqlParameter("@ErrorDescription", objDTO.ErrorDescription ?? (object)DBNull.Value),
                                                   new SqlParameter("@UpdatedBy", objDTO.UpdatedBy),
                                                   new SqlParameter("@ErrorType",objDTO.ErrorType.ToDBNull()),
                                                   new SqlParameter("@CommandText", objDTO.CommandText ?? (object)DBNull.Value)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateItemLocationPollRequest] @ID,@IsPollStarted,@PollStartTime,@IsPollCompleted,@PollCompletionTime,@IsPostProcessDone,@WeightReading,@ErrorDescription,@UpdatedBy,@ErrorType,@CommandText", params1);
                return true;
            }
        }

        public IEnumerable<ItemLocationPollRequestDTO> GetItemLocationPollRequestRecordsByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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


                //strQuer = @"EXEC [GetItemLocationPollRequestPagedRecord] " + StartRowIndex + "," + MaxRows + ",'" + SearchTerm + "','"
                //               + sortColumnName + "','" + Creaters + "','" + Updators + "','" + CreatedDateFrom + "','" + CreatedDateTo
                //               + "','" + UpdatedDateFrom + "','" + UpdatedDateTo + "','" + RoomID + "," + CompanyId;


            }
            else
            {
                //strQuer = @"EXEC [GetItemLocationPollRequestPagedRecord] " + StartRowIndex + "," + MaxRows + ",'" + SearchTerm + "','" + sortColumnName + "','','','','','',''," + RoomID + "," + CompanyId;
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

                List<ItemLocationPollRequestDTO> lstcats = context.Database.SqlQuery<ItemLocationPollRequestDTO>("exec [GetItemLocationPollRequestPagedRecord] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@Room,@CompanyID", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;

            }

        }

        public IEnumerable<CommonDTO> DB_GetItemLocationPollRequestListNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC  [GetItemLocationPollRequestListNarrowSearchData] ";
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

        public List<ItemLocationPollRequestRooms> GetItemLocationPollRequestRooms()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemLocationPollRequestRooms> list = context.Database.SqlQuery<ItemLocationPollRequestRooms>
                    ("exec GetItemLocationPollRequestRooms").ToList();
                return list;
            }
        }

        public ItemLocationPollRequestDTO GetItemLocationPollRequestForBetween(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", Convert.ToInt64(CompanyID)),
                                                   new SqlParameter("@RoomID", Convert.ToInt64(RoomID)) };

                return context.Database.SqlQuery<ItemLocationPollRequestDTO>("exec [GetItemLocationPollRequestForBetween] @CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }
        public long IsPollAllQueueCleared(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", Convert.ToInt64(CompanyID)),
                                                   new SqlParameter("@RoomID", Convert.ToInt64(RoomID)) };

                return context.Database.SqlQuery<long>("exec [IsPollAllQueueCleared] @CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }
    }
}
