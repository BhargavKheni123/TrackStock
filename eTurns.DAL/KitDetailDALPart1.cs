using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;
using System.Web;

namespace eTurns.DAL
{
    public class KitDetailDAL : eTurnsBaseDAL
    {
        public IEnumerable<KitDetailDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool NeedItemDetail)
        {
            //Get Cached-Media
            IEnumerable<KitDetailDTO> ObjCache = null;//
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = null;// CacheHelper<IEnumerable<KitDetailDTO>>.GetCacheItem("Cached_KitDetail_" + CompanyID.ToString());
                if (ObjCache == null || ObjCache.Count() <= 0)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<KitDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER JOIN ItemMaster KM on A.KitGUID = KM.GUID  LEFT OUTER JOIN ItemMaster KD on A.ItemGUID = KD.GUID WHERE  A.IsDeleted!=1 AND A.IsArchived != 1 AND KM.IsDeleted<>1 and KD.IsDeleted<>1 AND  A.CompanyID = " + CompanyID.ToString())
                                                         select new KitDetailDTO
                                                         {
                                                             ID = u.ID,
                                                             KitGUID = u.KitGUID,
                                                             ItemGUID = u.ItemGUID,
                                                             QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                             QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                             AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                                             Created = u.Created,
                                                             LastUpdated = u.LastUpdated,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             Room = u.Room,
                                                             IsDeleted = u.IsDeleted,
                                                             IsArchived = u.IsArchived,
                                                             CompanyID = u.CompanyID,
                                                             GUID = u.GUID,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                         }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<KitDetailDTO>>.AddCacheItem("Cached_KitDetail_" + CompanyID.ToString(), obj);
                    }
                }

                ObjCache = ObjCache.Where(x => x.Room == RoomID);
                if (NeedItemDetail)
                {
                    IList<ItemMasterDTO> ObjItemCache = new ItemMasterDAL(base.DataBaseName).GetAllItemsWithJoins(RoomID, CompanyID, false, false, null).ToList();
                    ObjCache = (from u in ObjCache
                                join ica in ObjItemCache on u.ItemGUID equals ica.GUID into ica_u_join
                                from ica_u in ica_u_join.DefaultIfEmpty()
                                select new KitDetailDTO
                                {
                                    ID = u.ID,
                                    KitGUID = u.KitGUID,
                                    ItemGUID = u.ItemGUID,
                                    QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                    QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                    AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                    Created = u.Created,
                                    LastUpdated = u.LastUpdated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    GUID = u.GUID,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    ItemNumber = ica_u.ItemNumber,
                                    ItemType = ica_u.ItemType,
                                    ItemDetail = ica_u,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID),
                                }).AsParallel().ToList();
                }
            }
            else
            {
                IEnumerable<ItemMasterDTO> objItemDTO = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<KitDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE  A.CompanyID = " + CompanyID.ToString() + @" And A.Room = " + RoomID.ToString() + " AND " + sSQL)
                                                     select new KitDetailDTO
                                                     {
                                                         ID = u.ID,
                                                         KitGUID = u.KitGUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                         QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                         AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                                         Created = u.Created,
                                                         LastUpdated = u.LastUpdated,
                                                         CreatedBy = u.CreatedBy,
                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                         Room = u.Room,
                                                         IsDeleted = u.IsDeleted,
                                                         IsArchived = u.IsArchived,
                                                         CompanyID = u.CompanyID,
                                                         GUID = u.GUID,
                                                         CreatedByName = u.CreatedByName,
                                                         UpdatedByName = u.UpdatedByName,
                                                         RoomName = u.RoomName,
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemDetail = objItemDTO.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                                                         QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                     }).AsParallel().ToList();
                    return obj;
                }

            }
            return ObjCache;
        }

        public IEnumerable<KitDetailDTO> GetCachedDataNew(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool NeedItemDetail)
        {
            //Get Cached-Media
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            IEnumerable<KitDetailDTO> ObjCache = null;//
            ItemMasterDAL objobjItemDAL = new ItemMasterDAL(base.DataBaseName);
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = null;// CacheHelper<IEnumerable<KitDetailDTO>>.GetCacheItem("Cached_KitDetail_" + CompanyID.ToString());
                if (ObjCache == null || ObjCache.Count() <= 0)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<KitDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER JOIN ItemMaster KM on A.KitGUID = KM.GUID  LEFT OUTER JOIN ItemMaster KD on A.ItemGUID = KD.GUID WHERE  A.IsDeleted!=1 AND A.IsArchived != 1 AND KM.IsDeleted<>1 and KD.IsDeleted<>1 AND  A.CompanyID = " + CompanyID.ToString() + " And A.Room=" + RoomID.ToString())
                                                         select new KitDetailDTO
                                                         {
                                                             ID = u.ID,
                                                             KitGUID = u.KitGUID,
                                                             ItemGUID = u.ItemGUID,
                                                             QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                             QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                             AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                                             Created = u.Created,
                                                             LastUpdated = u.LastUpdated,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             Room = u.Room,
                                                             IsDeleted = u.IsDeleted,
                                                             IsArchived = u.IsArchived,
                                                             CompanyID = u.CompanyID,
                                                             GUID = u.GUID,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                         }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<KitDetailDTO>>.AddCacheItem("Cached_KitDetail_" + CompanyID.ToString(), obj);
                    }
                }


                if (NeedItemDetail)
                {
                    IList<ItemMasterDTO> ObjItemCache = new ItemMasterDAL(base.DataBaseName).GetAllItemsWithJoins(RoomID, CompanyID, false, false, null).ToList();
                    if (ObjItemCache != null && ObjItemCache.Count > 0)
                    {
                        ObjCache = (from u in ObjCache
                                    join ica in ObjItemCache on u.ItemGUID equals ica.GUID into ica_u_join
                                    from ica_u in ica_u_join.DefaultIfEmpty()
                                    select new KitDetailDTO
                                    {
                                        ID = u.ID,
                                        KitGUID = u.KitGUID,
                                        ItemGUID = u.ItemGUID,
                                        QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                        QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                        AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                        Created = u.Created,
                                        LastUpdated = u.LastUpdated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        Room = u.Room,
                                        IsDeleted = u.IsDeleted,
                                        IsArchived = u.IsArchived,
                                        CompanyID = u.CompanyID,
                                        GUID = u.GUID,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        ItemNumber = ica_u.ItemNumber,
                                        ItemType = ica_u.ItemType,
                                        ItemDetail = ica_u,
                                        ReceivedOn = u.ReceivedOn,
                                        ReceivedOnWeb = u.ReceivedOnWeb,
                                        AddedFrom = u.AddedFrom,
                                        EditedFrom = u.EditedFrom,
                                        QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID),
                                    }).AsParallel().ToList();
                    }
                }
            }
            else
            {
                IEnumerable<ItemMasterDTO> objItemDTO = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<KitDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE  A.CompanyID = " + CompanyID.ToString() + @" And A.Room = " + RoomID.ToString() + " AND " + sSQL)
                                                     select new KitDetailDTO
                                                     {
                                                         ID = u.ID,
                                                         KitGUID = u.KitGUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                         QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                         AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                                         Created = u.Created,
                                                         LastUpdated = u.LastUpdated,
                                                         CreatedBy = u.CreatedBy,
                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                         Room = u.Room,
                                                         IsDeleted = u.IsDeleted,
                                                         IsArchived = u.IsArchived,
                                                         CompanyID = u.CompanyID,
                                                         GUID = u.GUID,
                                                         CreatedByName = u.CreatedByName,
                                                         UpdatedByName = u.UpdatedByName,
                                                         RoomName = u.RoomName,
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemDetail = objItemDTO.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                                                         QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                     }).AsParallel().ToList();
                    return obj;
                }

            }
            return ObjCache;
        }


        public IEnumerable<KitDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string KitMasterGUID)
        {
            //Get Cached-Media
            IEnumerable<KitDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, true).Where(x => x.KitGUID == Guid.Parse(KitMasterGUID));
            IEnumerable<KitDetailDTO> ObjGlobalCache = ObjCache;


            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                TotalCount = ObjCache.Where
                  (
                      t => t.ID.ToString().Contains(SearchTerm)

                  ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public bool DeleteRecordsExcept(string IDs, Guid KitGUID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IDs != "")
                {
                    string strQuery = "";
                    strQuery += "UPDATE KitDetail SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE KitGUID = '" + KitGUID.ToString() + "' AND ID Not in( ";
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            strQuery += item.ToString() + ",";
                        }
                    }
                    strQuery = strQuery.Substring(0, strQuery.Length - 1);
                    strQuery += ");";
                    context.ExecuteStoreCommand(strQuery);

                    CacheHelper<IEnumerable<KitDetailDTO>>.InvalidateCache();

                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(base.DataBaseName);
                    objKitDetailDAL.GetCachedDataNew(0, CompanyID, false, false, true);
                }
                else
                {
                    string strQuery = "";
                    strQuery += "UPDATE KitDetail SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE KitGUID = '" + KitGUID.ToString() + "'";
                    context.ExecuteStoreCommand(strQuery);

                    CacheHelper<IEnumerable<KitDetailDTO>>.InvalidateCache();

                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(base.DataBaseName);
                    objKitDetailDAL.GetCachedDataNew(0, CompanyID, false, false, true);
                }

                return true;
            }
        }

        public IEnumerable<KitDetailDTO> GetCachedDataNewWithItemGuid(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool NeedItemDetail, Guid ItemGuid)
        {
            //Get Cached-Media
            IEnumerable<KitDetailDTO> ObjCache = null;//
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = null;// CacheHelper<IEnumerable<KitDetailDTO>>.GetCacheItem("Cached_KitDetail_" + CompanyID.ToString());
                if (ObjCache == null || ObjCache.Count() <= 0)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<KitDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER JOIN ItemMaster KM on A.KitGUID = KM.GUID  LEFT OUTER JOIN ItemMaster KD on A.ItemGUID = KD.GUID WHERE  A.IsDeleted!=1 AND A.IsArchived != 1 AND KM.IsDeleted<>1 and KD.IsDeleted<>1 AND  A.CompanyID = " + CompanyID.ToString() + " And A.Room=" + RoomID.ToString() + " AND A.ItemGuid ='" + ItemGuid.ToString() + "'")
                                                         select new KitDetailDTO
                                                         {
                                                             ID = u.ID,
                                                             KitGUID = u.KitGUID,
                                                             ItemGUID = u.ItemGUID,
                                                             QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                             QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                             AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                                             Created = u.Created,
                                                             LastUpdated = u.LastUpdated,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             Room = u.Room,
                                                             IsDeleted = u.IsDeleted,
                                                             IsArchived = u.IsArchived,
                                                             CompanyID = u.CompanyID,
                                                             GUID = u.GUID,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                         }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<KitDetailDTO>>.AddCacheItem("Cached_KitDetail_" + CompanyID.ToString(), obj);
                    }
                }


                if (NeedItemDetail)
                {
                    IList<ItemMasterDTO> ObjItemCache = new ItemMasterDAL(base.DataBaseName).GetAllItemsWithJoins(RoomID, CompanyID, false, false, null).ToList();
                    ObjCache = (from u in ObjCache
                                join ica in ObjItemCache on u.ItemGUID equals ica.GUID into ica_u_join
                                from ica_u in ica_u_join.DefaultIfEmpty()
                                select new KitDetailDTO
                                {
                                    ID = u.ID,
                                    KitGUID = u.KitGUID,
                                    ItemGUID = u.ItemGUID,
                                    QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                    QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                    AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                    Created = u.Created,
                                    LastUpdated = u.LastUpdated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    GUID = u.GUID,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    ItemNumber = ica_u.ItemNumber,
                                    ItemType = ica_u.ItemType,
                                    ItemDetail = ica_u,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID),
                                }).AsParallel().ToList();
                }
            }
            else
            {
                IEnumerable<ItemMasterDTO> objItemDTO = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<KitDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM KitDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE  A.CompanyID = " + CompanyID.ToString() + @" And A.Room = " + RoomID.ToString() + " and A.ItemGuid ='" + ItemGuid.ToString() + "'" + " AND " + sSQL)
                                                     select new KitDetailDTO
                                                     {
                                                         ID = u.ID,
                                                         KitGUID = u.KitGUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                         QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                         AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                                         Created = u.Created,
                                                         LastUpdated = u.LastUpdated,
                                                         CreatedBy = u.CreatedBy,
                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                         Room = u.Room,
                                                         IsDeleted = u.IsDeleted,
                                                         IsArchived = u.IsArchived,
                                                         CompanyID = u.CompanyID,
                                                         GUID = u.GUID,
                                                         CreatedByName = u.CreatedByName,
                                                         UpdatedByName = u.UpdatedByName,
                                                         RoomName = u.RoomName,
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemDetail = objItemDTO.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                                                         QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                     }).AsParallel().ToList();
                    return obj;
                }

            }
            return ObjCache;
        }

        public bool DeleteRecords(string GUIDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] arrGUIDs = GUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arrGUIDs)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE KitDetail SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID = '" + item + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<KitDetailDTO> ObjCache = CacheHelper<IEnumerable<KitDetailDTO>>.GetCacheItem("Cached_KitDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<KitDetailDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => arrGUIDs.Contains(i.GUID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<KitDetailDTO>>.AppendToCacheItem("Cached_KitDetail_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        // Remove rajni
        public IEnumerable<KitDetailDTO> GetKitDetail(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool NeedItemDetail, string KitGUID, string ItemGUID)
        {
            IEnumerable<KitDetailDTO> lstKitDetail = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            if (IsArchived == false && IsDeleted == false)
            {
                lstKitDetail = null;
                if (lstKitDetail == null || lstKitDetail.Count() <= 0)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                           new SqlParameter("@CompanyID", CompanyID),
                                                           new SqlParameter("@KitGUID", KitGUID),
                                                           new SqlParameter("@ItemGUID", ItemGUID) };

                        lstKitDetail = (from u in context.ExecuteStoreQuery<KitDetailDTO>("exec [GetKitDetail] @RoomID,@CompanyID,@KitGUID,@ItemGUID", params1)
                                        select new KitDetailDTO
                                        {
                                            ID = u.ID,
                                            KitGUID = u.KitGUID,
                                            ItemGUID = u.ItemGUID,
                                            QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                            QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                            AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                            Created = u.Created,
                                            LastUpdated = u.LastUpdated,
                                            CreatedBy = u.CreatedBy,
                                            LastUpdatedBy = u.LastUpdatedBy,
                                            Room = u.Room,
                                            IsDeleted = u.IsDeleted,
                                            IsArchived = u.IsArchived,
                                            CompanyID = u.CompanyID,
                                            GUID = u.GUID,
                                            CreatedByName = u.CreatedByName,
                                            UpdatedByName = u.UpdatedByName,
                                            RoomName = u.RoomName,
                                            ReceivedOn = u.ReceivedOn,
                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                            AddedFrom = u.AddedFrom,
                                            EditedFrom = u.EditedFrom,
                                            QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                        }).AsParallel().ToList();
                    }
                }

                if (NeedItemDetail)
                {
                    IList<ItemMasterDTO> ObjItemCache = new ItemMasterDAL(base.DataBaseName).GetAllItemsWithJoins(RoomID, CompanyID, false, false, null).ToList();
                    lstKitDetail = (from u in lstKitDetail
                                    join ica in ObjItemCache on u.ItemGUID equals ica.GUID into ica_u_join
                                    from ica_u in ica_u_join.DefaultIfEmpty()
                                    select new KitDetailDTO
                                    {
                                        ID = u.ID,
                                        KitGUID = u.KitGUID,
                                        ItemGUID = u.ItemGUID,
                                        QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                        QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                        AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                        Created = u.Created,
                                        LastUpdated = u.LastUpdated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        Room = u.Room,
                                        IsDeleted = u.IsDeleted,
                                        IsArchived = u.IsArchived,
                                        CompanyID = u.CompanyID,
                                        GUID = u.GUID,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        ItemNumber = ica_u.ItemNumber,
                                        ItemType = ica_u.ItemType,
                                        ItemDetail = ica_u,
                                        ReceivedOn = u.ReceivedOn,
                                        ReceivedOnWeb = u.ReceivedOnWeb,
                                        AddedFrom = u.AddedFrom,
                                        EditedFrom = u.EditedFrom,
                                        QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID),
                                    }).AsParallel().ToList();
                }
            }
            else
            {
                IEnumerable<ItemMasterDTO> objItemDTO = objItemDAL.GetAllItemsPlain(RoomID, CompanyID);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                       new SqlParameter("@KitGUID", KitGUID),
                                                       new SqlParameter("@ItemGUID", ItemGUID),
                                                       new SqlParameter("@IsDeleted", IsDeleted),
                                                       new SqlParameter("@IsArchived", IsArchived) };

                    IEnumerable<KitDetailDTO> obj = (from u in context.ExecuteStoreQuery<KitDetailDTO>("exec [GetKitDetailByKitItemGUID] @RoomID,@CompanyID,@KitGUID,@ItemGUID,@IsDeleted,@IsArchived", params1)
                                                     select new KitDetailDTO
                                                     {
                                                         ID = u.ID,
                                                         KitGUID = u.KitGUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                                                         QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                                                         AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                                                         Created = u.Created,
                                                         LastUpdated = u.LastUpdated,
                                                         CreatedBy = u.CreatedBy,
                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                         Room = u.Room,
                                                         IsDeleted = u.IsDeleted,
                                                         IsArchived = u.IsArchived,
                                                         CompanyID = u.CompanyID,
                                                         GUID = u.GUID,
                                                         CreatedByName = u.CreatedByName,
                                                         UpdatedByName = u.UpdatedByName,
                                                         RoomName = u.RoomName,
                                                         ReceivedOn = u.ReceivedOn,
                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemDetail = objItemDTO.Where(x => x.GUID == u.ItemGUID).SingleOrDefault(),
                                                         QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(u.KitGUID), u.QuantityPerKit.GetValueOrDefault(0), u.AvailableItemsInWIP.GetValueOrDefault(0), u.Room, u.CompanyID)
                                                     }).AsParallel().ToList();
                    return obj;
                }

            }
            return lstKitDetail;
        }

    }
}
