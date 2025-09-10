using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DTO.Resources;
using System.Web;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class TransferMasterDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetCachedData(CompanyID, RoomID, null, null, null, null, null, null, null);
        }

        public IEnumerable<TransferMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return DB_GetCachedData(CompanyID, RoomID, IsDeleted, IsArchived, null, null, null, null, null);

            //IEnumerable<TransferMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            //ObjCache = ObjCache.Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived);
            //return ObjCache;
        }

        /// <summary>
        /// Get Paged Records from the Transfer Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetCachedData(CompanyID, RoomID, null, null, null, null, null, null, null);

            //IEnumerable<TransferMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);

            //return ObjCache;
        }

        public IEnumerable<TransferMasterDTO> GetPendingTransferList(Int64 RoomID, Int64 CompanyID)
        {
            IEnumerable<TransferMasterDTO> objOrders = GetAllRecords(RoomID, CompanyID, false, false);
            objOrders = (from x in objOrders
                         where (x.TransferStatus >= (int)OrderStatus.Transmitted
                                 && x.TransferStatus < (int)OrderStatus.Closed
                                 ) && x.RequireDate < DateTime.Now
                         orderby x.RequestType, x.TransferNumber
                         select x).AsEnumerable<TransferMasterDTO>();

            return objOrders;
        }

        public IEnumerable<TransferMasterDTO> GetPendingTransferList(Int64 RoomID, Int64 CompanyID, string DBConnectionstring)
        {
            IEnumerable<TransferMasterDTO> objOrders = GetAllRecords(RoomID, CompanyID, false, false, DBConnectionstring);
            objOrders = (from x in objOrders
                         where (x.TransferStatus >= (int)OrderStatus.Transmitted
                                 && x.TransferStatus < (int)OrderStatus.Closed
                                 ) && x.RequireDate < DateTime.Now
                         orderby x.RequestType, x.TransferNumber
                         select x).AsEnumerable<TransferMasterDTO>();

            return objOrders;
        }

        /// <summary>
        /// Get Paged Records from the Transfer Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return DB_GetCachedData(CompanyID, RoomID, IsDeleted, IsArchived, null, null, null, null, null);


            //IEnumerable<TransferMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID).Where(u => u.IsArchived == IsArchived && u.IsDeleted == IsDeleted);

            //return ObjCache;
        }

        public IEnumerable<TransferMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string DBConnectionstring)
        {
            return DB_GetGetPendingTransferList(CompanyID, RoomID, IsDeleted, IsArchived, null, null, null, null, null, DBConnectionstring);
        }

        public IEnumerable<TransferMasterDTO> DB_GetGetPendingTransferList(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? GuID, int? RequestType, int? TransferStatus, Guid? RefTransferGuid, string DBConnectionstring)
        {

            using (var context = new eTurnsEntities(DBConnectionstring))
            {



                string strCommand = "EXEC Trnsfr_GetTransferMasterData ";

                if (CompanyID.HasValue)
                    strCommand += CompanyID.Value.ToString();
                else
                    strCommand += "null";
                if (RoomID.HasValue)
                    strCommand += ", " + RoomID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (IsDeleted.HasValue)
                    strCommand += ", " + (IsDeleted.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (IsArchived.HasValue)
                    strCommand += ", " + (IsArchived.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (ID.HasValue)
                    strCommand += ", " + ID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (GuID.HasValue)
                    strCommand += ", '" + GuID.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                if (RequestType.HasValue)
                    strCommand += ", " + RequestType.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (TransferStatus.HasValue)
                    strCommand += ", " + TransferStatus.Value.ToString();
                else
                    strCommand += ", " + "null";


                if (RefTransferGuid.HasValue)
                    strCommand += ", '" + RefTransferGuid.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                IEnumerable<TransferMasterDTO> obj = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(strCommand)
                                                      select new TransferMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          TransferNumber = u.TransferNumber,
                                                          ReplenishingRoomID = u.ReplenishingRoomID,
                                                          StagingID = u.StagingID,
                                                          Comment = u.Comment,
                                                          RequestType = u.RequestType,
                                                          RequireDate = u.RequireDate,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          RoomID = u.RoomID,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,
                                                          GUID = u.GUID,
                                                          RequestingRoomID = u.RequestingRoomID,
                                                          RefTransferGUID = u.RefTransferGUID,
                                                          TransferStatus = u.TransferStatus,
                                                          CreatedByName = u.CreatedByName,
                                                          RoomName = u.RoomName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          StagingName = u.StagingName,
                                                          ReplenishingRoomName = u.ReplenishingRoomName,
                                                          RequestingRoomName = u.RequestingRoomName,
                                                          RefTransferNumber = u.RefTransferNumber,
                                                          RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                                          TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                                                          RejectionReason = u.RejectionReason,
                                                          TransferIsInReceive = u.TransferIsInReceive,
                                                          Action = string.Empty,
                                                          HistoryID = 0,
                                                          IsAbleToDelete = false,
                                                          IsHistory = false,
                                                          IsOnlyStatusUpdate = false,
                                                          IsRecordNotEditable = false,
                                                          TransferDetailList = null,
                                                          AppendedBarcodeString = u.AppendedBarcodeString,
                                                          NoOfItems = u.NoOfItems,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          TotalCost = u.TotalCost
                                                      }).AsParallel().ToList();

                return obj;
            }

        }

        public IEnumerable<TransferMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, int RequestType, int TransferStatus)
        {
            return DB_GetCachedData(CompanyID, RoomID, IsDeleted, IsArchived, null, null, RequestType, TransferStatus, null);


            //IEnumerable<TransferMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID).Where(u => u.IsArchived == IsArchived && u.IsDeleted == IsDeleted);

            //return ObjCache;
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferMasterDTO> DB_GetCachedData(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? GuID, int? RequestType, int? TransferStatus, Guid? RefTransferGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC Trnsfr_GetTransferMasterData ";

                if (CompanyID.HasValue)
                    strCommand += CompanyID.Value.ToString();
                else
                    strCommand += "null";
                if (RoomID.HasValue)
                    strCommand += ", " + RoomID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (IsDeleted.HasValue)
                    strCommand += ", " + (IsDeleted.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (IsArchived.HasValue)
                    strCommand += ", " + (IsArchived.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (ID.HasValue)
                    strCommand += ", " + ID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (GuID.HasValue)
                    strCommand += ", '" + GuID.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                if (RequestType.HasValue)
                    strCommand += ", " + RequestType.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (TransferStatus.HasValue)
                    strCommand += ", " + TransferStatus.Value.ToString();
                else
                    strCommand += ", " + "null";


                if (RefTransferGuid.HasValue)
                    strCommand += ", '" + RefTransferGuid.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                IEnumerable<TransferMasterDTO> obj = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(strCommand)
                                                      select new TransferMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          TransferNumber = u.TransferNumber,
                                                          ReplenishingRoomID = u.ReplenishingRoomID,
                                                          StagingID = u.StagingID,
                                                          Comment = u.Comment,
                                                          RequestType = u.RequestType,
                                                          RequireDate = u.RequireDate,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          RoomID = u.RoomID,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,
                                                          GUID = u.GUID,
                                                          RequestingRoomID = u.RequestingRoomID,
                                                          RefTransferGUID = u.RefTransferGUID,
                                                          TransferStatus = u.TransferStatus,
                                                          CreatedByName = u.CreatedByName,
                                                          RoomName = u.RoomName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          StagingName = u.StagingName,
                                                          ReplenishingRoomName = u.ReplenishingRoomName,
                                                          RequestingRoomName = u.RequestingRoomName,
                                                          RefTransferNumber = u.RefTransferNumber,
                                                          RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                                          TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                                                          RejectionReason = u.RejectionReason,
                                                          TransferIsInReceive = u.TransferIsInReceive,
                                                          Action = string.Empty,
                                                          HistoryID = 0,
                                                          IsAbleToDelete = false,
                                                          IsHistory = false,
                                                          IsOnlyStatusUpdate = false,
                                                          IsRecordNotEditable = false,
                                                          TransferDetailList = null,
                                                          AppendedBarcodeString = u.AppendedBarcodeString,
                                                          NoOfItems = u.NoOfItems,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                      }).AsParallel().ToList();

                return obj;
            }

        }

        /// <summary>
        /// Get Paged Records from the Transfer Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, string CurrentTimeZone, string MainFilter = "")
        {
            return DB_GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone, MainFilter);
        }

        /// <summary>
        /// Get Paged Records from the OrderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferMasterDTO> DB_GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, string CurrentTimeZone, string MainFilter = "")
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strBinIDs = string.Empty;
                string strQuer = "";
                string StatusQuery = "";
                string RequestTypeQry = "null";
                string UDF1 = null;
                string UDF2 = null;
                string UDF3 = null;
                string UDF4 = null;
                string UDF5 = null;
                string ItemCreaters = null;
                string ItemUpdators = null;
                string CreatedDateFrom = null;
                string CreatedDateTo = null;
                string UpdatedDateFrom = null;
                string UpdatedDateTo = null;
                string RequiredDateFromMoreThan3Week = null;
                string RequiredDateToMoreThan3Week = null;
                string TransferStatus = null;
                string RequiredDateFrom2to3Weeks = null;
                string RequiredDateTo2to3Weeks = null;
                string RequiredDateFromNextWeek = null;
                string RequiredDateToNextWeek = null;
                string RequiredDateFromThisWeek = null;
                string RequiredDateTothisWeek = null;
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[^]"))
                {
                    int idx = SearchTerm.IndexOf("[^]");

                    StatusQuery = SearchTerm.Remove(0, idx + 3);
                    if (!string.IsNullOrEmpty(StatusQuery))
                    {
                        if (StatusQuery.ToLower().Contains("receivable"))
                        {
                            StatusQuery = "4,5,6,7,8";
                            RequestTypeQry = "0";
                        }
                        else if (StatusQuery.ToLower().Contains("transferable"))
                        {
                            StatusQuery = "4,5,6,7,8";
                            RequestTypeQry = "1";
                        }
                        else if (StatusQuery.ToLower().Contains("changetransefer"))
                        {
                            StatusQuery = "4";
                        }
                        else if (StatusQuery.ToLower().Contains("requested"))
                        {
                            StatusQuery = "1";
                            RequestTypeQry = "1";
                        }
                    }
                    SearchTerm = SearchTerm.Substring(0, idx);

                }

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
                {
                    string[] stringSeparators = new string[] { "[###]" };
                    string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string[] FieldsPara = Fields[0].Split('~');
                    DateTime FromdDate = DateTime.Now;
                    DateTime ToDate = DateTime.Now;

                    if (Fields[0].Split('~')[3] != "")
                    {
                        if (Fields[0].Split('~')[3].Contains("1"))//  > 3 weeks 
                        {
                            FromdDate = DateTime.Now.AddDays(21);
                            ToDate = FromdDate.AddDays(999);
                            RequiredDateFromMoreThan3Week = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateToMoreThan3Week = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                        if (Fields[0].Split('~')[3].Contains("2"))// 2-3 weeks
                        {
                            FromdDate = DateTime.Now.AddDays(14);
                            ToDate = FromdDate.AddDays(7);
                            RequiredDateFrom2to3Weeks = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateTo2to3Weeks = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                        if (Fields[0].Split('~')[3].Contains("3"))// Next weeks
                        {
                            FromdDate = DateTime.Now.AddDays(7);
                            ToDate = FromdDate.AddDays(7);
                            RequiredDateFromNextWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateToNextWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                        if (Fields[0].Split('~')[3].Contains("4"))// This weeks
                        {
                            FromdDate = DateTime.Now;
                            ToDate = FromdDate.AddDays(7);
                            RequiredDateFromThisWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateTothisWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {
                        ItemCreaters = FieldsPara[0];//.TrimEnd(',');
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                    {
                        ItemUpdators = FieldsPara[1];//.TrimEnd(',');
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                    {
                        TransferStatus = FieldsPara[2];
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                    {
                        CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                    {
                        UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                    {
                        string[] arrReplenishTypes = FieldsPara[6].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF1 = UDF1 + supitem + ",";
                        }
                        UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF1 = HttpUtility.UrlDecode(UDF1);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                    {
                        string[] arrReplenishTypes = FieldsPara[7].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF2 = UDF2 + supitem + ",";
                        }
                        UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF2 = HttpUtility.UrlDecode(UDF2);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                    {
                        string[] arrReplenishTypes = FieldsPara[8].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF3 = UDF3 + supitem + ",";
                        }
                        UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF3 = HttpUtility.UrlDecode(UDF3);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                    {
                        string[] arrReplenishTypes = FieldsPara[9].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF4 = UDF4 + supitem + ",";
                        }
                        UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF4 = HttpUtility.UrlDecode(UDF4);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                    {
                        string[] arrReplenishTypes = FieldsPara[10].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF5 = UDF5 + supitem + ",";
                        }
                        UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF5 = HttpUtility.UrlDecode(UDF5);
                    }


                    if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                    {
                        strBinIDs = FieldsPara[11];
                    }

                    if (Fields.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(Fields[1]))
                            SearchTerm = Fields[1];
                        else
                            SearchTerm = string.Empty;
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }


                if (!string.IsNullOrEmpty(MainFilter) && MainFilter.ToLower() == "true")
                    MainFilter = "1,4";
                sortColumnName = sortColumnName.Replace("RequestTypeName", " RequestType");

                var spName = IsArchived ? "GetTransferMasterPagedData_Archive" : "Trnsfr_GetTransferMasterPagedData";
                strQuer = @"EXEC [" + spName + "] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@SearchTerm,@TransferStatusIn,@RequestType,@MainFilter,@TransferStatus,@BinIDs,@ItemCreaters,@ItemUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@RequiredDateFromMoreThan3Week,@RequiredDateToMoreThan3Week,@RequiredDateFrom2to3Weeks,@RequiredDateTo2to3Weeks,@RequiredDateFromNextWeek,@RequiredDateToNextWeek,@RequiredDateFromThisWeek,@RequiredDateTothisWeek,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5";

                List<SqlParameter> sqlParas = new List<SqlParameter>();
                sqlParas.Add(new SqlParameter("@CompnayID", CompanyID));
                sqlParas.Add(new SqlParameter("@RoomID", RoomID));
                sqlParas.Add(new SqlParameter("@IsDeleted", IsDeleted));
                sqlParas.Add(new SqlParameter("@IsArchived", IsArchived));
                sqlParas.Add(new SqlParameter("@StartRowIndex", StartRowIndex));
                sqlParas.Add(new SqlParameter("@MaxRows", MaxRows));

                if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName != "null")
                    sqlParas.Add(new SqlParameter("@sortColumnName", sortColumnName));
                else
                    sqlParas.Add(new SqlParameter("@sortColumnName", DBNull.Value));

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm != "null")
                    sqlParas.Add(new SqlParameter("@SearchTerm", SearchTerm));
                else
                    sqlParas.Add(new SqlParameter("@SearchTerm", DBNull.Value));

                if (!string.IsNullOrEmpty(StatusQuery) && StatusQuery != "null")
                    sqlParas.Add(new SqlParameter("@TransferStatusIn", StatusQuery));
                else
                    sqlParas.Add(new SqlParameter("@TransferStatusIn", DBNull.Value));

                if (!string.IsNullOrEmpty(RequestTypeQry) && RequestTypeQry != "null")
                    sqlParas.Add(new SqlParameter("@RequestType", RequestTypeQry));
                else
                    sqlParas.Add(new SqlParameter("@RequestType", DBNull.Value));

                if (!string.IsNullOrEmpty(MainFilter) && MainFilter != "null")
                    sqlParas.Add(new SqlParameter("@MainFilter", MainFilter));
                else
                    sqlParas.Add(new SqlParameter("@MainFilter", DBNull.Value));

                sqlParas.Add(new SqlParameter("@TransferStatus", TransferStatus ?? string.Empty));

                if (!string.IsNullOrEmpty(strBinIDs) && strBinIDs != "null")
                    sqlParas.Add(new SqlParameter("@BinIDs", strBinIDs));
                else
                    sqlParas.Add(new SqlParameter("@BinIDs", DBNull.Value));

                sqlParas.Add(new SqlParameter("@ItemCreaters", ItemCreaters ?? string.Empty));
                sqlParas.Add(new SqlParameter("@ItemUpdators", ItemUpdators ?? string.Empty));
                sqlParas.Add(new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? string.Empty));
                sqlParas.Add(new SqlParameter("@CreatedDateTo", CreatedDateTo ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFromMoreThan3Week", RequiredDateFromMoreThan3Week ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateToMoreThan3Week", RequiredDateToMoreThan3Week ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFrom2to3Weeks", RequiredDateFrom2to3Weeks ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateTo2to3Weeks", RequiredDateTo2to3Weeks ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFromNextWeek", RequiredDateFromNextWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateToNextWeek", RequiredDateToNextWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFromThisWeek", RequiredDateFromThisWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateTothisWeek", RequiredDateTothisWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF1", UDF1 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF2", UDF2 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF3", UDF3 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF4", UDF4 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF5", UDF5 ?? string.Empty));

                var params1 = sqlParas.ToArray();

                IEnumerable<TransferMasterDTO> obj = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(strQuer, params1)
                                                      select new TransferMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          TransferNumber = u.TransferNumber,
                                                          ReplenishingRoomID = u.ReplenishingRoomID,
                                                          StagingID = u.StagingID,
                                                          Comment = u.Comment,
                                                          RequestType = u.RequestType,
                                                          RequireDate = u.RequireDate,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          RoomID = u.RoomID,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,
                                                          GUID = u.GUID,
                                                          RequestingRoomID = u.RequestingRoomID,
                                                          RefTransferGUID = u.RefTransferGUID,
                                                          TransferStatus = u.TransferStatus,
                                                          CreatedByName = u.CreatedByName,
                                                          RoomName = u.RoomName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          StagingName = u.StagingName,
                                                          ReplenishingRoomName = u.ReplenishingRoomName,
                                                          RequestingRoomName = u.RequestingRoomName,
                                                          RefTransferNumber = u.RefTransferNumber,
                                                          RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                                          TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                                                          RejectionReason = u.RejectionReason,
                                                          TransferIsInReceive = u.TransferIsInReceive,
                                                          Action = string.Empty,
                                                          HistoryID = 0,
                                                          IsAbleToDelete = false,
                                                          IsHistory = false,
                                                          IsOnlyStatusUpdate = false,
                                                          IsRecordNotEditable = false,
                                                          TransferDetailList = null,
                                                          AppendedBarcodeString = u.AppendedBarcodeString,
                                                          TotalRecords = u.TotalRecords,
                                                          NoOfItems = u.NoOfItems,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          TotalCost = u.TotalCost
                                                      }).AsParallel().ToList();

                TotalCount = 0;
                if (obj != null && obj.Count() > 0)
                {
                    TotalCount = obj.ElementAt(0).TotalRecords;
                }
                return obj;
            }

        }

        /// <summary>
        /// Get Particullar Record from the KitMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetSingleRecord(CompanyID, RoomID, id, null, null);


            //            TransferMasterDTO transferDTO = GetCachedData(RoomID, CompanyID).SingleOrDefault(t => t.ID == id);
            //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //            {
            //                transferDTO = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(@"SELECT   A.*
            //	                                                                                            ,R.RoomName	
            //	                                                                                            ,RQR.RoomName AS RequestingRoomName
            //	                                                                                            ,RPR.RoomName AS ReplenishingRoomName
            //	                                                                                            ,UC.UserName AS CreatedByName
            //	                                                                                            ,UU.UserName AS UpdatedByName
            //	                                                                                            ,A1.TransferNumber AS RefTransferNumber
            //	                                                                                            ,B.BinNumber  AS StagingName
            //                                                                                                ,Convert(Bit,ISNULL((SELECT TOP 1 ISNULL(TD.ReceivedQuantity,0) FROM TransferDetail TD WHERE TD.TransferGUID = A.GUID AND TD.ReceivedQuantity >0 ),0))  AS TransferIsInReceive
            //                                                                                    FROM TransferMaster A LEFT OUTER JOIN Room R ON A.RoomID = R.ID
            //					                                                                                    LEFT OUTER JOIN Room RQR ON A.RequestingRoomID = RQR.ID
            //					                                                                                    LEFT OUTER JOIN Room RPR ON A.ReplenishingRoomID = RPR.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UC ON A.CreatedBy = UC.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UU ON A.LastUpdatedBy= UU.ID
            //					                                                                                    LEFT OUTER JOIN TransferMaster A1 ON A.RefTransferGUID= A1.GUID
            //					                                                                                    LEFT OUTER JOIN BinMaster B ON A.StagingID = B.ID 
            //                                                                                    WHERE A.CompanyID =" + CompanyID + " AND A.ID = " + id.ToString())
            //                               select new TransferMasterDTO
            //                               {
            //                                   ID = u.ID,
            //                                   TransferNumber = u.TransferNumber,
            //                                   ReplenishingRoomID = u.ReplenishingRoomID,
            //                                   StagingID = u.StagingID,
            //                                   Comment = u.Comment,
            //                                   RequestType = u.RequestType,
            //                                   RequireDate = u.RequireDate,
            //                                   UDF1 = u.UDF1,
            //                                   UDF2 = u.UDF2,
            //                                   UDF3 = u.UDF3,
            //                                   UDF4 = u.UDF4,
            //                                   UDF5 = u.UDF5,
            //                                   Created = u.Created,
            //                                   Updated = u.Updated,
            //                                   CreatedBy = u.CreatedBy,
            //                                   LastUpdatedBy = u.LastUpdatedBy,
            //                                   RoomID = u.RoomID,
            //                                   IsDeleted = u.IsDeleted,
            //                                   IsArchived = u.IsArchived,
            //                                   CompanyID = u.CompanyID,
            //                                   GUID = u.GUID,
            //                                   RequestingRoomID = u.RequestingRoomID,
            //                                   RefTransferGUID = u.RefTransferGUID,
            //                                   TransferStatus = u.TransferStatus,
            //                                   CreatedByName = u.CreatedByName,
            //                                   RoomName = u.RoomName,
            //                                   UpdatedByName = u.UpdatedByName,
            //                                   StagingName = u.StagingName,
            //                                   ReplenishingRoomName = u.ReplenishingRoomName,
            //                                   RequestingRoomName = u.RequestingRoomName,
            //                                   RefTransferNumber = u.RefTransferNumber,
            //                                   RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
            //                                   TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
            //                                   RejectionReason = u.RejectionReason,
            //                                   TransferIsInReceive = u.TransferIsInReceive,
            //                                   Action = string.Empty,
            //                                   HistoryID = 0,
            //                                   IsAbleToDelete = false,
            //                                   IsHistory = false,
            //                                   IsOnlyStatusUpdate = false,
            //                                   IsRecordNotEditable = false,
            //                                   TransferDetailList = null

            //                               }).SingleOrDefault();
            //            }
            //            return transferDTO;
        }

        /// <summary>
        /// Get Particullar Record from the KitMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, string DBConnectionString)
        {
            return DB_GetSingleRecord(CompanyID, RoomID, id, null, null, DBConnectionString);


            //            TransferMasterDTO transferDTO = GetCachedData(RoomID, CompanyID).SingleOrDefault(t => t.ID == id);
            //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //            {
            //                transferDTO = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(@"SELECT   A.*
            //	                                                                                            ,R.RoomName	
            //	                                                                                            ,RQR.RoomName AS RequestingRoomName
            //	                                                                                            ,RPR.RoomName AS ReplenishingRoomName
            //	                                                                                            ,UC.UserName AS CreatedByName
            //	                                                                                            ,UU.UserName AS UpdatedByName
            //	                                                                                            ,A1.TransferNumber AS RefTransferNumber
            //	                                                                                            ,B.BinNumber  AS StagingName
            //                                                                                                ,Convert(Bit,ISNULL((SELECT TOP 1 ISNULL(TD.ReceivedQuantity,0) FROM TransferDetail TD WHERE TD.TransferGUID = A.GUID AND TD.ReceivedQuantity >0 ),0))  AS TransferIsInReceive
            //                                                                                    FROM TransferMaster A LEFT OUTER JOIN Room R ON A.RoomID = R.ID
            //					                                                                                    LEFT OUTER JOIN Room RQR ON A.RequestingRoomID = RQR.ID
            //					                                                                                    LEFT OUTER JOIN Room RPR ON A.ReplenishingRoomID = RPR.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UC ON A.CreatedBy = UC.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UU ON A.LastUpdatedBy= UU.ID
            //					                                                                                    LEFT OUTER JOIN TransferMaster A1 ON A.RefTransferGUID= A1.GUID
            //					                                                                                    LEFT OUTER JOIN BinMaster B ON A.StagingID = B.ID 
            //                                                                                    WHERE A.CompanyID =" + CompanyID + " AND A.ID = " + id.ToString())
            //                               select new TransferMasterDTO
            //                               {
            //                                   ID = u.ID,
            //                                   TransferNumber = u.TransferNumber,
            //                                   ReplenishingRoomID = u.ReplenishingRoomID,
            //                                   StagingID = u.StagingID,
            //                                   Comment = u.Comment,
            //                                   RequestType = u.RequestType,
            //                                   RequireDate = u.RequireDate,
            //                                   UDF1 = u.UDF1,
            //                                   UDF2 = u.UDF2,
            //                                   UDF3 = u.UDF3,
            //                                   UDF4 = u.UDF4,
            //                                   UDF5 = u.UDF5,
            //                                   Created = u.Created,
            //                                   Updated = u.Updated,
            //                                   CreatedBy = u.CreatedBy,
            //                                   LastUpdatedBy = u.LastUpdatedBy,
            //                                   RoomID = u.RoomID,
            //                                   IsDeleted = u.IsDeleted,
            //                                   IsArchived = u.IsArchived,
            //                                   CompanyID = u.CompanyID,
            //                                   GUID = u.GUID,
            //                                   RequestingRoomID = u.RequestingRoomID,
            //                                   RefTransferGUID = u.RefTransferGUID,
            //                                   TransferStatus = u.TransferStatus,
            //                                   CreatedByName = u.CreatedByName,
            //                                   RoomName = u.RoomName,
            //                                   UpdatedByName = u.UpdatedByName,
            //                                   StagingName = u.StagingName,
            //                                   ReplenishingRoomName = u.ReplenishingRoomName,
            //                                   RequestingRoomName = u.RequestingRoomName,
            //                                   RefTransferNumber = u.RefTransferNumber,
            //                                   RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
            //                                   TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
            //                                   RejectionReason = u.RejectionReason,
            //                                   TransferIsInReceive = u.TransferIsInReceive,
            //                                   Action = string.Empty,
            //                                   HistoryID = 0,
            //                                   IsAbleToDelete = false,
            //                                   IsHistory = false,
            //                                   IsOnlyStatusUpdate = false,
            //                                   IsRecordNotEditable = false,
            //                                   TransferDetailList = null

            //                               }).SingleOrDefault();
            //            }
            //            return transferDTO;
        }

        public TransferMasterDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetSingleRecord(CompanyID, RoomID, null, GUID, null);

            //            TransferMasterDTO transferDTO = GetCachedData(RoomID, CompanyID).SingleOrDefault(t => t.GUID == GUID);
            //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //            {
            //                transferDTO = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(@"SELECT   A.*
            //	                                                                                            ,R.RoomName	
            //	                                                                                            ,RQR.RoomName AS RequestingRoomName
            //	                                                                                            ,RPR.RoomName AS ReplenishingRoomName
            //	                                                                                            ,UC.UserName AS CreatedByName
            //	                                                                                            ,UU.UserName AS UpdatedByName
            //	                                                                                            ,A1.TransferNumber AS RefTransferNumber
            //	                                                                                            ,B.BinNumber  AS StagingName
            //                                                                                                ,Convert(Bit,ISNULL((SELECT TOP 1 ISNULL(TD.ReceivedQuantity,0) FROM TransferDetail TD WHERE TD.TransferGUID = A.GUID AND TD.ReceivedQuantity >0 ),0))  AS TransferIsInReceive
            //                                                                                    FROM TransferMaster A LEFT OUTER JOIN Room R ON A.RoomID = R.ID
            //					                                                                                    LEFT OUTER JOIN Room RQR ON A.RequestingRoomID = RQR.ID
            //					                                                                                    LEFT OUTER JOIN Room RPR ON A.ReplenishingRoomID = RPR.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UC ON A.CreatedBy = UC.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UU ON A.LastUpdatedBy= UU.ID
            //					                                                                                    LEFT OUTER JOIN TransferMaster A1 ON A.RefTransferGUID= A1.GUID
            //					                                                                                    LEFT OUTER JOIN BinMaster B ON A.StagingID = B.ID 
            //                                                                                    WHERE A.CompanyID =" + CompanyID + " AND A.GUID = '" + GUID.ToString() + "'")
            //                               select new TransferMasterDTO
            //                               {
            //                                   ID = u.ID,
            //                                   TransferNumber = u.TransferNumber,
            //                                   ReplenishingRoomID = u.ReplenishingRoomID,
            //                                   StagingID = u.StagingID,
            //                                   Comment = u.Comment,
            //                                   RequestType = u.RequestType,
            //                                   RequireDate = u.RequireDate,
            //                                   UDF1 = u.UDF1,
            //                                   UDF2 = u.UDF2,
            //                                   UDF3 = u.UDF3,
            //                                   UDF4 = u.UDF4,
            //                                   UDF5 = u.UDF5,
            //                                   Created = u.Created,
            //                                   Updated = u.Updated,
            //                                   CreatedBy = u.CreatedBy,
            //                                   LastUpdatedBy = u.LastUpdatedBy,
            //                                   RoomID = u.RoomID,
            //                                   IsDeleted = u.IsDeleted,
            //                                   IsArchived = u.IsArchived,
            //                                   CompanyID = u.CompanyID,
            //                                   GUID = u.GUID,
            //                                   RequestingRoomID = u.RequestingRoomID,
            //                                   RefTransferGUID = u.RefTransferGUID,
            //                                   TransferStatus = u.TransferStatus,
            //                                   CreatedByName = u.CreatedByName,
            //                                   RoomName = u.RoomName,
            //                                   UpdatedByName = u.UpdatedByName,
            //                                   StagingName = u.StagingName,
            //                                   ReplenishingRoomName = u.ReplenishingRoomName,
            //                                   RequestingRoomName = u.RequestingRoomName,
            //                                   RefTransferNumber = u.RefTransferNumber,
            //                                   RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
            //                                   TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
            //                                   RejectionReason = u.RejectionReason,
            //                                   TransferIsInReceive = u.TransferIsInReceive,
            //                                   Action = string.Empty,
            //                                   HistoryID = 0,
            //                                   IsAbleToDelete = false,
            //                                   IsHistory = false,
            //                                   IsOnlyStatusUpdate = false,
            //                                   IsRecordNotEditable = false,
            //                                   TransferDetailList = null

            //                               }).SingleOrDefault();
            //            }
            //            return transferDTO;
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public TransferMasterDTO DB_GetSingleRecord(Int64? CompanyID, Int64? RoomID, Int64? ID, Guid? GuID, Guid? RefTransferGuid, string DBConnectionString)
        {

            using (var context = new eTurnsEntities(DBConnectionString))
            {

                if (ID == null && GuID == null && RefTransferGuid == null)
                    return null;

                string strCommand = "EXEC Trnsfr_GetTransferMasterData ";

                if (CompanyID.HasValue)
                    strCommand += CompanyID.Value.ToString();
                else
                    strCommand += "null";
                if (RoomID.HasValue)
                    strCommand += ", " + RoomID.Value.ToString();
                else
                    strCommand += ", " + "null";

                strCommand += ", " + "null";
                strCommand += ", " + "null";

                if (ID.HasValue)
                    strCommand += ", " + ID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (GuID.HasValue)
                    strCommand += ", '" + GuID.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                strCommand += ", " + "null";
                strCommand += ", " + "null";


                if (RefTransferGuid.HasValue)
                    strCommand += ", '" + RefTransferGuid.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                TransferMasterDTO obj = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(strCommand)
                                         select new TransferMasterDTO
                                         {
                                             ID = u.ID,
                                             TransferNumber = u.TransferNumber,
                                             ReplenishingRoomID = u.ReplenishingRoomID,
                                             StagingID = u.StagingID,
                                             Comment = u.Comment,
                                             RequestType = u.RequestType,
                                             RequireDate = u.RequireDate,
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5,
                                             Created = u.Created,
                                             Updated = u.Updated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             RoomID = u.RoomID,
                                             IsDeleted = u.IsDeleted,
                                             IsArchived = u.IsArchived,
                                             CompanyID = u.CompanyID,
                                             GUID = u.GUID,
                                             RequestingRoomID = u.RequestingRoomID,
                                             RefTransferGUID = u.RefTransferGUID,
                                             TransferStatus = u.TransferStatus,
                                             CreatedByName = u.CreatedByName,
                                             RoomName = u.RoomName,
                                             UpdatedByName = u.UpdatedByName,
                                             StagingName = u.StagingName,
                                             ReplenishingRoomName = u.ReplenishingRoomName,
                                             RequestingRoomName = u.RequestingRoomName,
                                             RefTransferNumber = u.RefTransferNumber,
                                             RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                             TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                                             RejectionReason = u.RejectionReason,
                                             TransferIsInReceive = u.TransferIsInReceive,
                                             Action = string.Empty,
                                             HistoryID = 0,
                                             IsAbleToDelete = false,
                                             IsHistory = false,
                                             IsOnlyStatusUpdate = false,
                                             IsRecordNotEditable = false,
                                             TransferDetailList = null,
                                             AppendedBarcodeString = u.AppendedBarcodeString,
                                             NoOfItems = u.NoOfItems,
                                             ReceivedOn = u.ReceivedOn,
                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                             AddedFrom = u.AddedFrom,
                                             EditedFrom = u.EditedFrom,
                                             TotalCost = u.TotalCost
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }

        }

        /// <summary>
        /// Get Particullar Record from the KitMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferMasterDTO GetRecordByRefTransferID(Guid id, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetSingleRecord(CompanyID, RoomID, null, null, id);
            //            TransferMasterDTO transferDTO = null;// GetCachedData(RoomID, CompanyID).SingleOrDefault(t => t.GUID == id);
            //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //            {
            //                transferDTO = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(@"SELECT   A.ID	        ,A.TransferNumber	,A.RequireDate    ,A.ReplenishingRoomID		,A.RequestingRoomID   ,A.Comment
            //                                                                                                ,A.StagingID	,A.TransferStatus   ,A.RequestType    ,A.RefTransferGUID		,A.UDF1				  ,A.UDF2
            //                                                                                                ,A.UDF3		    ,A.UDF4			    ,A.UDF5			  ,A.[GUID]				    ,A.Created		      ,A.Updated
            //                                                                                                ,A.CreatedBy    ,A.LastUpdatedBy    ,A.IsDeleted      ,A.IsArchived		        ,A.CompanyID		  ,A.RoomID
            //	                                                                                            ,R.RoomName	
            //	                                                                                            ,RQR.RoomName AS RequestingRoomName
            //	                                                                                            ,RPR.RoomName AS ReplenishingRoomName
            //	                                                                                            ,UC.UserName AS CreatedByName
            //	                                                                                            ,UU.UserName AS UpdatedByName
            //	                                                                                            ,A1.TransferNumber AS RefTransferNumber
            //	                                                                                            ,B.BinNumber  AS StagingName
            //                                                                                    FROM TransferMaster A LEFT OUTER JOIN Room R ON A.RoomID = R.ID
            //					                                                                                    LEFT OUTER JOIN Room RQR ON A.RequestingRoomID = RQR.ID
            //					                                                                                    LEFT OUTER JOIN Room RPR ON A.ReplenishingRoomID = RPR.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UC ON A.CreatedBy = UC.ID
            //					                                                                                    LEFT OUTER JOIN UserMaster UU ON A.LastUpdatedBy= UU.ID
            //					                                                                                    LEFT OUTER JOIN TransferMaster A1 ON A.RefTransferGUID= A1.GUID
            //					                                                                                    LEFT OUTER JOIN BinMaster B ON A.StagingID = B.ID 
            //                                                                                    WHERE A.CompanyID =" + CompanyID + " AND A.RefTransferGUID = '" + id.ToString() + "'")
            //                               select new TransferMasterDTO
            //                               {
            //                                   ID = u.ID,
            //                                   TransferNumber = u.TransferNumber,
            //                                   ReplenishingRoomID = u.ReplenishingRoomID,
            //                                   StagingID = u.StagingID,
            //                                   Comment = u.Comment,
            //                                   RequestType = u.RequestType,
            //                                   RequireDate = u.RequireDate,
            //                                   UDF1 = u.UDF1,
            //                                   UDF2 = u.UDF2,
            //                                   UDF3 = u.UDF3,
            //                                   UDF4 = u.UDF4,
            //                                   UDF5 = u.UDF5,
            //                                   Created = u.Created,
            //                                   Updated = u.Updated,
            //                                   CreatedBy = u.CreatedBy,
            //                                   LastUpdatedBy = u.LastUpdatedBy,
            //                                   RoomID = u.RoomID,
            //                                   IsDeleted = u.IsDeleted,
            //                                   IsArchived = u.IsArchived,
            //                                   CompanyID = u.CompanyID,
            //                                   GUID = u.GUID,
            //                                   RequestingRoomID = u.RequestingRoomID,
            //                                   RefTransferGUID = u.RefTransferGUID,
            //                                   TransferStatus = u.TransferStatus,
            //                                   CreatedByName = u.CreatedByName,
            //                                   RoomName = u.RoomName,
            //                                   UpdatedByName = u.UpdatedByName,
            //                                   StagingName = u.StagingName,
            //                                   ReplenishingRoomName = u.ReplenishingRoomName,
            //                                   RequestingRoomName = u.RequestingRoomName,
            //                                   RefTransferNumber = u.RefTransferNumber,
            //                                   RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
            //                                   TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
            //                                   RejectionReason = ""
            //                               }).FirstOrDefault();
            //            }
            //            return transferDTO;
        }

        /// <summary>
        /// Get Particullar Record from the OrderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferMasterDTO GetHistoryRecord(Int64 HistoryID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<TransferMasterDTO>(@"
                                        SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,MS.StagingName
                                        FROM TransferMaster_History A  LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.RoomID = D.ID 
                                                LEFT OUTER JOIN MaterialStaging MS ON (MS.ID= A.StagingID OR MS.Guid=A.MaterialStagingGUID) 
                                                WHERE A.HistoryID=" + HistoryID.ToString())
                        select new TransferMasterDTO
                        {
                            ID = u.ID,
                            TransferNumber = u.TransferNumber,
                            ReplenishingRoomID = u.ReplenishingRoomID,
                            StagingID = u.StagingID,
                            Comment = u.Comment,
                            RequestType = u.RequestType,
                            RequireDate = u.RequireDate,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            RoomID = u.RoomID,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            RequestingRoomID = u.RequestingRoomID,
                            RefTransferGUID = u.RefTransferGUID,
                            TransferStatus = u.TransferStatus,
                            CreatedByName = u.CreatedByName,
                            RoomName = u.RoomName,
                            UpdatedByName = u.UpdatedByName,
                            StagingName = u.StagingName,
                            ReplenishingRoomName = u.ReplenishingRoomName,
                            RequestingRoomName = u.RequestingRoomName,
                            RefTransferNumber = u.RefTransferNumber,
                            RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                            TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                            RejectionReason = "",
                            Action = u.Action,
                            HistoryID = u.HistoryID,
                            IsHistory = true,
                            IsOnlyStatusUpdate = false,
                            IsRecordNotEditable = true,
                            TransferIsInReceive = false,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,

                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase Transfer
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public TransferMasterDTO DB_InsertTransfer(TransferMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommnad = "Exec Trnsfr_InsertTransferMasterData ";

                strCommnad += "'" + objDTO.TransferNumber + "'";
                strCommnad += ",'" + objDTO.RequireDate + "'";
                strCommnad += "," + objDTO.ReplenishingRoomID;
                strCommnad += "," + objDTO.RequestingRoomID;

                if (!string.IsNullOrEmpty(objDTO.Comment))
                    strCommnad += ",'" + objDTO.Comment + "'";
                else
                    strCommnad += ",null";

                if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                    strCommnad += "," + objDTO.StagingID;
                else
                    strCommnad += ",null";

                strCommnad += "," + objDTO.TransferStatus;

                strCommnad += "," + objDTO.RequestType;

                if (!string.IsNullOrEmpty(objDTO.UDF1))
                    strCommnad += ",'" + objDTO.UDF1 + "'";
                else
                    strCommnad += ",null";

                if (!string.IsNullOrEmpty(objDTO.UDF2))
                    strCommnad += ",'" + objDTO.UDF2 + "'";
                else
                    strCommnad += ",null";

                if (!string.IsNullOrEmpty(objDTO.UDF3))
                    strCommnad += ",'" + objDTO.UDF3 + "'";
                else
                    strCommnad += ",null";

                if (!string.IsNullOrEmpty(objDTO.UDF4))
                    strCommnad += ",'" + objDTO.UDF4 + "'";
                else
                    strCommnad += ",null";

                if (!string.IsNullOrEmpty(objDTO.UDF5))
                    strCommnad += ",'" + objDTO.UDF5 + "'";
                else
                    strCommnad += ",null";

                strCommnad += "," + objDTO.CreatedBy;
                strCommnad += "," + objDTO.LastUpdatedBy;
                strCommnad += "," + objDTO.CompanyID;
                strCommnad += "," + objDTO.RoomID;
                if (!string.IsNullOrEmpty(objDTO.RejectionReason))
                    strCommnad += ",'" + objDTO.RejectionReason + "'";
                else
                    strCommnad += ",null";

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Transfer";

                strCommnad += ",'" + objDTO.WhatWhereAction + "'";

                //for ID and GUID
                strCommnad += ",null,null";

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    strCommnad += ",'" + objDTO.AddedFrom + "'";
                else
                    strCommnad += ",null";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    strCommnad += ",'" + objDTO.EditedFrom + "'";
                else
                    strCommnad += ",null";

                strCommnad += ",'" + objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "'";
                strCommnad += ",'" + objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "'";

                if (!string.IsNullOrEmpty(strCommnad))
                {
                    objDTO = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(strCommnad)
                              select new TransferMasterDTO
                              {
                                  ID = u.ID,
                                  TransferNumber = u.TransferNumber,
                                  ReplenishingRoomID = u.ReplenishingRoomID,
                                  StagingID = u.StagingID,
                                  Comment = u.Comment,
                                  RequestType = u.RequestType,
                                  RequireDate = u.RequireDate,
                                  UDF1 = u.UDF1,
                                  UDF2 = u.UDF2,
                                  UDF3 = u.UDF3,
                                  UDF4 = u.UDF4,
                                  UDF5 = u.UDF5,
                                  Created = u.Created,
                                  Updated = u.Updated,
                                  CreatedBy = u.CreatedBy,
                                  LastUpdatedBy = u.LastUpdatedBy,
                                  RoomID = u.RoomID,
                                  IsDeleted = u.IsDeleted,
                                  IsArchived = u.IsArchived,
                                  CompanyID = u.CompanyID,
                                  GUID = u.GUID,
                                  RequestingRoomID = u.RequestingRoomID,
                                  RefTransferGUID = u.RefTransferGUID,
                                  TransferStatus = u.TransferStatus,
                                  CreatedByName = u.CreatedByName,
                                  RoomName = u.RoomName,
                                  UpdatedByName = u.UpdatedByName,
                                  StagingName = u.StagingName,
                                  ReplenishingRoomName = u.ReplenishingRoomName,
                                  RequestingRoomName = u.RequestingRoomName,
                                  RefTransferNumber = u.RefTransferNumber,
                                  RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                  TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                                  RejectionReason = u.RejectionReason,
                                  TransferIsInReceive = u.TransferIsInReceive,
                                  Action = string.Empty,
                                  HistoryID = 0,
                                  IsAbleToDelete = false,
                                  IsHistory = false,
                                  IsOnlyStatusUpdate = false,
                                  IsRecordNotEditable = false,
                                  TransferDetailList = null,
                                  AppendedBarcodeString = u.AppendedBarcodeString,
                                  NoOfItems = u.NoOfItems,
                                  AddedFrom = u.AddedFrom,
                                  EditedFrom = u.EditedFrom,
                                  ReceivedOn = u.ReceivedOn,
                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                  TotalCost = u.TotalCost
                              }).AsParallel().FirstOrDefault();

                    if (objDTO.RequestType == 0)
                    {
                        new AutoSequenceDAL(base.DataBaseName).UpdateNextTransferNumber(objDTO.RoomID, objDTO.CompanyID, objDTO.TransferNumber);
                        // new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("NextTransferNo", objDTO.RoomID, objDTO.CompanyID, objDTO.TransferNumber);
                    }

                    return objDTO;
                }
            }
            return null;
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool DB_Edit(TransferMasterDTO objDTO, string DbConnectionString)
        {
            if (!string.IsNullOrWhiteSpace(DbConnectionString))
            {

                using (var context = new eTurnsEntities(DbConnectionString))
                {
                    string strCommnad = "Exec Trnsfr_UpdateTransferMasterData ";

                    strCommnad += "'" + objDTO.TransferNumber + "'";

                    strCommnad += ",'" + objDTO.RequireDate + "'";
                    strCommnad += "," + objDTO.ReplenishingRoomID;

                    if (!string.IsNullOrEmpty(objDTO.Comment))
                        strCommnad += ",'" + objDTO.Comment + "'";
                    else
                        strCommnad += ",null";

                    if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                        strCommnad += "," + objDTO.StagingID;
                    else
                        strCommnad += ",null";

                    strCommnad += "," + objDTO.TransferStatus;

                    if (objDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        strCommnad += ",'" + objDTO.RefTransferGUID + "'";
                    else
                        strCommnad += ",null";

                    if (!string.IsNullOrEmpty(objDTO.UDF1))
                        strCommnad += ",'" + objDTO.UDF1 + "'";
                    else
                        strCommnad += ",null";


                    if (!string.IsNullOrEmpty(objDTO.UDF2))
                        strCommnad += ",'" + objDTO.UDF2 + "'";
                    else
                        strCommnad += ",null";

                    if (!string.IsNullOrEmpty(objDTO.UDF3))
                        strCommnad += ",'" + objDTO.UDF3 + "'";
                    else
                        strCommnad += ",null";

                    if (!string.IsNullOrEmpty(objDTO.UDF4))
                        strCommnad += ",'" + objDTO.UDF4 + "'";
                    else
                        strCommnad += ",null";

                    if (!string.IsNullOrEmpty(objDTO.UDF5))
                        strCommnad += ",'" + objDTO.UDF5 + "'";
                    else
                        strCommnad += ",null";


                    strCommnad += "," + objDTO.LastUpdatedBy;

                    if (!string.IsNullOrEmpty(objDTO.RejectionReason))
                        strCommnad += ",'" + objDTO.RejectionReason + "'";
                    else
                        strCommnad += ",null";

                    strCommnad += ",'" + objDTO.GUID + "'";
                    strCommnad += "," + objDTO.ID + "";

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Transfer";

                    strCommnad += ",'" + objDTO.WhatWhereAction + "'";

                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        strCommnad += ",'" + objDTO.EditedFrom + "'";
                    else
                        strCommnad += ",null";
                    if (objDTO.ReceivedOn.HasValue)
                        strCommnad += ",'" + objDTO.ReceivedOn.Value + "'";
                    else
                        strCommnad += ",null";

                    if (!string.IsNullOrEmpty(strCommnad))
                    {
                        int cnt = context.ExecuteStoreCommand(strCommnad);
                        if (cnt > 0)
                        {
                            return true;
                        }
                    }


                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method is used to return intransit items to replenish room when user(requesting room user or replenish room user) performs close transfer.
        /// </summary>
        /// <param name="transferGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ReturnIntransitItemsResult ReturnInTransitItemsToReplenishRoomOnCloseTransfer(Guid transferGuid, long userId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommnad = "Exec Trnsfr_ReturnIntransitQuantityToReplenishRoomOnTransferClose ";
                strCommnad += "'" + transferGuid + "'";
                strCommnad += "," + userId + "";
                var returnIntransitItemsResult = context.ExecuteStoreQuery<ReturnIntransitItemsResult>(strCommnad).FirstOrDefault();
                return returnIntransitItemsResult;
            }
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public TransferMasterDTO DB_GetSingleRecord(Int64? CompanyID, Int64? RoomID, Int64? ID, Guid? GuID, Guid? RefTransferGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                if (ID == null && GuID == null && RefTransferGuid == null)
                    return null;

                string strCommand = "EXEC Trnsfr_GetTransferMasterData ";

                if (CompanyID.HasValue)
                    strCommand += CompanyID.Value.ToString();
                else
                    strCommand += "null";
                if (RoomID.HasValue)
                    strCommand += ", " + RoomID.Value.ToString();
                else
                    strCommand += ", " + "null";

                strCommand += ", " + "null";
                strCommand += ", " + "null";

                if (ID.HasValue)
                    strCommand += ", " + ID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (GuID.HasValue)
                    strCommand += ", '" + GuID.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                strCommand += ", " + "null";
                strCommand += ", " + "null";


                if (RefTransferGuid.HasValue)
                    strCommand += ", '" + RefTransferGuid.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                TransferMasterDTO obj = (from u in context.ExecuteStoreQuery<TransferMasterDTO>(strCommand)
                                         select new TransferMasterDTO
                                         {
                                             ID = u.ID,
                                             TransferNumber = u.TransferNumber,
                                             ReplenishingRoomID = u.ReplenishingRoomID,
                                             StagingID = u.StagingID,
                                             Comment = u.Comment,
                                             RequestType = u.RequestType,
                                             RequireDate = u.RequireDate,
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5,
                                             Created = u.Created,
                                             Updated = u.Updated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             RoomID = u.RoomID,
                                             IsDeleted = u.IsDeleted,
                                             IsArchived = u.IsArchived,
                                             CompanyID = u.CompanyID,
                                             GUID = u.GUID,
                                             RequestingRoomID = u.RequestingRoomID,
                                             RefTransferGUID = u.RefTransferGUID,
                                             TransferStatus = u.TransferStatus,
                                             CreatedByName = u.CreatedByName,
                                             RoomName = u.RoomName,
                                             UpdatedByName = u.UpdatedByName,
                                             StagingName = u.StagingName,
                                             ReplenishingRoomName = u.ReplenishingRoomName,
                                             RequestingRoomName = u.RequestingRoomName,
                                             RefTransferNumber = u.RefTransferNumber,
                                             RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                             TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                                             RejectionReason = u.RejectionReason,
                                             TransferIsInReceive = u.TransferIsInReceive,
                                             Action = string.Empty,
                                             HistoryID = 0,
                                             IsAbleToDelete = false,
                                             IsHistory = false,
                                             IsOnlyStatusUpdate = false,
                                             IsRecordNotEditable = false,
                                             TransferDetailList = null,
                                             AppendedBarcodeString = u.AppendedBarcodeString,
                                             NoOfItems = u.NoOfItems,
                                             ReceivedOn = u.ReceivedOn,
                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                             AddedFrom = u.AddedFrom,
                                             EditedFrom = u.EditedFrom,
                                             TotalCost = u.TotalCost,
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }

        }
    }
}
