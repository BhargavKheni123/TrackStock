using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Web;
using System.Data.SqlClient;


namespace eTurns.DAL
{
    public partial class RequisitionDetailsDAL : eTurnsBaseDAL
    {
        public void UpdateItemOnRequisitionQty(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID, string DbConnectionString)
        {
            using (var context = new eTurnsEntities(DbConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@UserId", UserID) };
                context.ExecuteStoreQuery<ItemMasterDTO>("exec [UpdateItemREquisitionedQty] @RoomId,@CompanyId,@ItemGuid,@UserId", params1).FirstOrDefault();
            }
        }
        private string GetQueryOfOnlyUpdateReqQty(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            string qry = @"SELECT ISNULL((CASE WHEN ISNULL(A.OnRequsitionQty,0) <0 THEN 0 ELSE A.OnRequsitionQty END),0) AS OnRequisitionQuantity FROM
                              (SELECT  SUM(((Case When ISNULL(RD.QuantityApproved,0) > 0 THEN ISNULL(RD.QuantityApproved,0) ELSE ISNULL(RD.QuantityRequisitioned,0) END) - ISNULL(RD.QuantityPulled,0))) AS OnRequsitionQty
	                           FROM RequisitionMaster RM Inner join RequisitionDetails RD ON RM.GUID = RD.RequisitionGUID
		                       WHERE ISNULL(RM.IsDeleted,0) =0 AND ISNULL(RM.IsArchived,0) =0
			                   AND ISNULL(RD.IsDeleted,0) =0 AND ISNULL(RD.IsArchived,0) =0
			                   AND RM.RequisitionStatus Not in ('Closed')
			                   AND RM.Room = " + RoomID + @" AND RM.CompanyID = " + CompanyID + @"
			                   AND RD.Room = " + RoomID + @" AND RD.CompanyID = " + CompanyID + @"
			                   AND RD.ItemGUID = '" + ItemGuid.ToString() + @"'
                               ) AS A";

            return qry;

        }
        public Int64 InsertQLItems(RequisitionDetailsDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionDetail obj = new RequisitionDetail();
                obj.ID = 0;
                obj.RequisitionGUID = objDTO.RequisitionGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.ItemSellPrice = objDTO.ItemSellPrice;
                obj.BinID = objDTO.BinID;

                if (objDTO.ProjectSpendGUID != Guid.Empty)
                    obj.ProjectSpendGUID = objDTO.ProjectSpendGUID;

                obj.QuantityRequisitioned = objDTO.QuantityRequisitioned;
                obj.QuantityPulled = objDTO.QuantityPulled;
                obj.QuantityApproved = objDTO.QuantityApproved;
                obj.RequiredDate = objDTO.RequiredDate;
                obj.Created = objDTO.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();
                obj.PullUDF1 = objDTO.PullUDF1;
                obj.PullUDF2 = objDTO.PullUDF2;
                obj.PullUDF3 = objDTO.PullUDF3;
                obj.PullUDF4 = objDTO.PullUDF4;
                obj.PullUDF5 = objDTO.PullUDF5;

                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";

                context.RequisitionDetails.AddObject(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<RequisitionDetailsDTO> ObjCache = CacheHelper<IEnumerable<RequisitionDetailsDTO>>.GetCacheItem("Cached_RequisitionDetails_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<RequisitionDetailsDTO> tempC = new List<RequisitionDetailsDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<RequisitionDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<RequisitionDetailsDTO>>.AppendToCacheItem("Cached_RequisitionDetails_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                //RequisitionMasterDAL objRequMDAL = new RequisitionMasterDAL(base.DataBaseName);
                //string RequisitionStatus = objRequMDAL.GetRecord(objDTO.RequisitionGUID.Value, (Int64)objDTO.Room, (Int64)objDTO.CompanyID, (bool)objDTO.IsArchived, (bool)objDTO.IsDeleted).RequisitionStatus;
                //if (RequisitionStatus == "Unsubmitted" || RequisitionStatus == "Submittted")
                //{
                //    UpdateRequisitionedQuantity("Add", objDTO, objDTO);
                //    UpdateRequisitionTotalCost(objDTO.RequisitionGUID.Value, objDTO.Room, objDTO.CompanyID);
                //}

                return obj.ID;
            }

        }
        public IEnumerable<RequisitionDetailsDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, string DBConnectionstring)
        {
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetails] @CompanyID,@RoomId", params1).ToList();
            }

        }
        public IEnumerable<RequisitionDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }
        public IEnumerable<RequisitionDetailsDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<RequisitionDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            IEnumerable<RequisitionDetailsDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<RequisitionDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<RequisitionDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }
        public RequisitionDetailsDTO GetHistoryRecord(Int64 id, long RoomID, long CompanyID)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<RequisitionDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM RequisitionDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new RequisitionDetailsDTO
                        {
                            ID = u.ID,
                            RequisitionGUID = u.RequisitionGUID,
                            ItemGUID = u.ItemGUID,
                            ItemCost = Math.Round(u.ItemCost.GetValueOrDefault(0), objeTurnsRegionInfo.CurrencyDecimalDigits),
                            ItemSellPrice = Math.Round(u.ItemSellPrice, objeTurnsRegionInfo.CurrencyDecimalDigits),
                            QuantityRequisitioned = Math.Round(u.QuantityRequisitioned.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            QuantityPulled = Math.Round(u.QuantityPulled.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            QuantityApproved = Math.Round(u.QuantityApproved.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            RequiredDate = u.RequiredDate,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            PullUDF1 = u.PullUDF1,
                            PullUDF2 = u.PullUDF2,
                            PullUDF3 = u.PullUDF3,
                            PullUDF4 = u.PullUDF4,
                            PullUDF5 = u.PullUDF5,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                        }).SingleOrDefault();
            }
        }
        public IEnumerable<RequisitionDetailsDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetails] @CompanyID,@RoomId", params1).ToList();
            }
        }
        public IEnumerable<RequisitionDetailsDTO> GetReqLineItemsByReqGUID(Guid RequisitionGUID, Int64 RoomID, Int64 CompanyId, Int64 SupplierID = 0)
        {
            //if (SupplierID > 0)
            // return GetCachedData(RoomID, CompanyId).Where(x => x.SupplierID.GetValueOrDefault(0) == SupplierID).Where(t => t.RequisitionGUID == RequisitionGUID).OrderBy("ID DESC");
            return GetAllReqLineItemsByReqGUID(RoomID, CompanyId, RequisitionGUID, SupplierID).OrderBy("ID DESC");
            //else
            //  return GetCachedData(RoomID, CompanyId).Where(t => t.RequisitionGUID == RequisitionGUID).OrderBy("ID DESC");
        }
        public IEnumerable<RequisitionDetailsDTO> GetAllReqLineItemsByReqGUID(Int64 RoomID, Int64 CompanyID, Guid? RequisitionGUID, Int64? SupplierID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@SupplierID", SupplierID) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsusingReqGuid] @CompanyID,@RoomId,@RequisitionGUID,@SupplierID", params1).ToList();
            }
        }
        public IEnumerable<RequisitionDetailsDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid RequisitionGUID)
        {
            //Get Cached-Media
            IEnumerable<RequisitionDetailsDTO> ObjCache = GetAllReqLineItemsByReqGUID(RoomID, CompanyID, RequisitionGUID, 0);
            IEnumerable<RequisitionDetailsDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<RequisitionDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<RequisitionDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }
        public RequisitionDetailsDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@Guid", GUID) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsGuid] @CompanyID,@RoomId,@Guid", params1).FirstOrDefault();
            }
        }
        public RequisitionDetailsDTO GetRecordByID(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetRecord(GUID, RoomID, CompanyID);
        }
        public RequisitionDetailsDTO GetRecordByItemID(Guid ItemGUID, Guid RequisitionGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RequisitionGUID", RequisitionGUID) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsItemGUID] @CompanyID,@RoomId,@ItemGUID,@RequisitionGUID", params1).FirstOrDefault();
            }
        }
        public RequisitionDetailsDTO GetRecordByItemIDBinId(Guid ItemGUID, long BinId, Guid RequisitionGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@BinId", BinId) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsBinIdItemGUID] @CompanyID,@RoomId,@ItemGUID,@RequisitionGUID,@BinId", params1).FirstOrDefault();
            }
        }
        public RequisitionDetailsDTO GetNonDeletedRequisitionDetailsBinIdItemGUID(Guid ItemGUID, long BinId, Guid RequisitionGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@BinId", BinId) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetNonDeletedRequisitionDetailsBinIdItemGUID] @CompanyID,@RoomId,@ItemGUID,@RequisitionGUID,@BinId", params1).FirstOrDefault();
            }
        }
        public List<RequisitionDetailsDTO> GetRecordByItemID(Guid ItemGUID, long RoomID, long CompanyID, bool Isdelete, bool IsArchieve)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@IsDeleted", Isdelete), new SqlParameter("@IsArchived", IsArchieve) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRecordByItemID] @CompanyID,@RoomId,@ItemGUID,@IsDeleted,@IsArchived", params1).ToList();
            }
        }
        public IEnumerable<RequisitionDetailsDTO> GetRequisitionDetailDataWithItemBin(Int64 RoomID, Int64 CompanyID, Guid RequisitionGUID, Guid ItemGuid, int BinID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@ItemGUID", ItemGuid), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@BinId", BinID) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsBinIdItemGUID] @CompanyID,@RoomId,@ItemGUID,@RequisitionGUID,@BinId", params1).ToList();

                //IEnumerable<RequisitionDetailsDTO> obj = (from u in context.ExecuteStoreQuery<RequisitionDetailsDTO>(@"SELECT A.*, 
                //    B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,RM.RequisitionStatus,
                //    E.ItemNumber,E.Description,E.LongDescription,E.SerialNumberTracking, CM.Category as CategoryName,SM.SupplierName,SM.ID as SupplierID,RM.RequisitionType ,
                //    E.UDF1 AS ItemUDF1,E.UDF2 AS ItemUDF2,E.UDF3 AS ItemUDF3,E.UDF4 AS ItemUDF4,E.UDF5 AS ItemUDF5,RS.CurrencyDecimalDigits,RS.NumberDecimalDigits FROM RequisitionDetails A 
                //                                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                //                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                //                                                LEFT OUTER JOIN Room D on A.Room = D.ID  
                //                                                LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID 
                //                                                Left Outer join CategoryMaster CM on CM.ID = e.CategoryID  
                //                                                left Outer join SupplierMaster SM on SM.ID = e.SupplierID 
                //                                                left outer join RequisitionMaster RM on RM.GUID = A.RequisitionGUID  
                //                                                left outer join RegionalSetting RS on A.Room = RS.RoomID
                //                                                WHERE A.Isdeleted=0 and A.CompanyID = " + CompanyID.ToString() + " and A.Room =" + RoomID.ToString() + " and A.RequisitionGUID = '" + RequisitionGUID.ToString() + "' and A.ItemGuid= '" + ItemGuid.ToString() + "' and A.BinID=" + BinID.ToString())
                //                                          select new RequisitionDetailsDTO
                //                                          {
                //                                              CurrencyDecimalDigits = u.CurrencyDecimalDigits ?? 0,
                //                                              NumberDecimalDigits = u.NumberDecimalDigits ?? 0,
                //                                              ID = u.ID,
                //                                              RequisitionGUID = u.RequisitionGUID,
                //                                              ItemGUID = u.ItemGUID,
                //                                              ItemCost = Math.Round(u.ItemCost.GetValueOrDefault(0), u.CurrencyDecimalDigits ?? 0),
                //                                              QuantityRequisitioned = Math.Round(u.QuantityRequisitioned.GetValueOrDefault(0), u.NumberDecimalDigits ?? 0),
                //                                              QuantityPulled = Math.Round(u.QuantityPulled.GetValueOrDefault(0), u.NumberDecimalDigits ?? 0),
                //                                              QuantityApproved = Math.Round(u.QuantityApproved.GetValueOrDefault(0), u.NumberDecimalDigits ?? 0),
                //                                              RequiredDate = u.RequiredDate,
                //                                              Created = u.Created,
                //                                              LastUpdated = u.LastUpdated,
                //                                              CreatedBy = u.CreatedBy,
                //                                              LastUpdatedBy = u.LastUpdatedBy,
                //                                              Room = u.Room,
                //                                              IsDeleted = u.IsDeleted.GetValueOrDefault(false),//(u.IsDeleted.HasValue ? u.IsDeleted : false),
                //                                              IsArchived = u.IsArchived.GetValueOrDefault(false), //(u.IsArchived.HasValue ? u.IsArchived : false),
                //                                              CompanyID = u.CompanyID,
                //                                              GUID = u.GUID,
                //                                              CreatedByName = u.CreatedByName,
                //                                              UpdatedByName = u.UpdatedByName,
                //                                              RoomName = u.RoomName,
                //                                              SupplierID = u.SupplierID,
                //                                              SupplierName = u.SupplierName,
                //                                              CategoryName = u.CategoryName,
                //                                              LongDescription = u.LongDescription,
                //                                              ItemNumber = u.ItemNumber,
                //                                              BinID = u.BinID,
                //                                              ProjectSpendGUID = u.ProjectSpendGUID,
                //                                              RequisitionStatus = u.RequisitionStatus,
                //                                              SerialNumberTracking = u.SerialNumberTracking,
                //                                              ItemUDF1 = u.ItemUDF1,
                //                                              ItemUDF2 = u.ItemUDF2,
                //                                              ItemUDF3 = u.ItemUDF3,
                //                                              ItemUDF4 = u.ItemUDF4,
                //                                              ItemUDF5 = u.ItemUDF5,
                //                                              PullUDF1 = u.PullUDF1,
                //                                              PullUDF2 = u.PullUDF2,
                //                                              PullUDF3 = u.PullUDF3,
                //                                              PullUDF4 = u.PullUDF4,
                //                                              PullUDF5 = u.PullUDF5,
                //                                              Description = u.Description,
                //                                              ReceivedOn = u.ReceivedOn,
                //                                              ReceivedOnWeb = u.ReceivedOnWeb,
                //                                              AddedFrom = u.AddedFrom,
                //                                              EditedFrom = u.EditedFrom,
                //                                          }).AsParallel().ToList();
                //if (obj != null && obj.Count() > 0)
                //{
                //    return obj;
                //}
                //else
                //{
                //    return null;
                //}
            }

        }
        public IEnumerable<RequisitionDetailsDTO> GetRequisitionDetailDataWithTool(Int64 RoomID, Int64 CompanyID, Guid RequisitionGUID, Guid ToolGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@ToolGUID", ToolGuid), new SqlParameter("@RequisitionGUID", RequisitionGUID) };
                return context.ExecuteStoreQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsToolGUID] @CompanyID,@RoomId,@ToolGUID,@RequisitionGUID", params1).ToList();
            }
        }
    }
}
