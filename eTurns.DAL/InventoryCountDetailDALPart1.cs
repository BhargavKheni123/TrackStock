using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;

namespace eTurns.DAL
{
    public partial class InventoryCountDetailDAL : eTurnsBaseDAL
    {
        public IEnumerable<InventoryCountDetailDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<InventoryCountDetailDTO> ObjCache = CacheHelper<IEnumerable<InventoryCountDetailDTO>>.GetCacheItem("Cached_InventoryCountDetail_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<InventoryCountDetailDTO> obj = (from u in context.ExecuteStoreQuery<InventoryCountDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM InventoryCountDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.RoomID = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.RoomID = " + RoomID.ToString())
                                                                select new InventoryCountDetailDTO
                                                                {
                                                                    ID = u.ID,
                                                                    InventoryCountGUID = u.InventoryCountGUID,
                                                                    ItemGUID = u.ItemGUID,
                                                                    BinGUID = u.BinGUID,
                                                                    CountQuantity = u.CountQuantity,
                                                                    CountLineItemDescription = u.CountLineItemDescription,
                                                                    CountItemStatus = u.CountItemStatus,
                                                                    UDF1 = u.UDF1,
                                                                    UDF2 = u.UDF2,
                                                                    UDF3 = u.UDF3,
                                                                    UDF4 = u.UDF4,
                                                                    UDF5 = u.UDF5,
                                                                    Created = u.Created,
                                                                    Updated = u.Updated,
                                                                    CreatedBy = u.CreatedBy,
                                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                                    IsDeleted = u.IsDeleted,
                                                                    IsArchived = u.IsArchived,
                                                                    GUID = u.GUID,
                                                                    CreatedByName = u.CreatedByName,
                                                                    UpdatedByName = u.UpdatedByName,
                                                                    RoomName = u.RoomName,
                                                                }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<InventoryCountDetailDTO>>.AddCacheItem("Cached_InventoryCountDetail_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }
        public InventoryCountDetailDTO GetRecordByGUID(Guid tGuid, Int64 RoomID, Int64 CompanyID)
        {
            List<InventoryCountDetailDTO> LstDtl = new List<InventoryCountDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<InventoryCountDetailDTO> obj = (from u in context.ExecuteStoreQuery<InventoryCountDetailDTO>(@"SELECT ID,IsApplied,InventoryCountGUID,ItemGUID,BinGUID FROM InventoryCountDetail where CompanyID = " + CompanyID.ToString() + @" AND RoomID = " + RoomID.ToString() + " AND GUID='" + tGuid.ToString() + "' and IsDeleted=0 and IsArchived=0 ")
                                                            select new InventoryCountDetailDTO
                                                            {
                                                                ID = u.ID,
                                                                IsApplied = u.IsApplied,
                                                                InventoryCountGUID = u.InventoryCountGUID,
                                                                ItemGUID = u.ItemGUID,
                                                                BinGUID = u.BinGUID
                                                            }).AsParallel().ToList();

                if (obj != null)
                    return obj.FirstOrDefault();
                else
                    return null;

            }
        }

        public IEnumerable<InventoryCountDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<InventoryCountDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<InventoryCountDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            IEnumerable<InventoryCountDetailDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<InventoryCountDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<InventoryCountDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
     
        public InventoryCountDetailDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);
        }
    
        public InventoryCountDetailDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<InventoryCountDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM InventoryCountDetail_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new InventoryCountDetailDTO
                        {
                            ID = u.ID,
                            InventoryCountGUID = u.InventoryCountGUID,
                            ItemGUID = u.ItemGUID,
                            BinGUID = u.BinGUID,
                            CountQuantity = u.CountQuantity,
                            CountLineItemDescription = u.CountLineItemDescription,
                            CountItemStatus = u.CountItemStatus,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            GUID = u.GUID,
                        }).SingleOrDefault();
            }
        }

        public Int64 Insert(InventoryCountDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail obj = new InventoryCountDetail();
                obj.ID = 0;
                obj.InventoryCountGUID = objDTO.InventoryCountGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.BinGUID = objDTO.BinGUID;
                obj.CountQuantity = objDTO.CountQuantity;
                obj.CountLineItemDescription = objDTO.CountLineItemDescription;
                obj.CountItemStatus = objDTO.CountItemStatus;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = Guid.NewGuid();
                obj.AddedFrom = "Web";
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                context.InventoryCountDetails.AddObject(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<InventoryCountDetailDTO> ObjCache = CacheHelper<IEnumerable<InventoryCountDetailDTO>>.GetCacheItem("Cached_InventoryCountDetail_" + objDTO.CompanyId.ToString());
                    if (ObjCache != null)
                    {
                        List<InventoryCountDetailDTO> tempC = new List<InventoryCountDetailDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<InventoryCountDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<InventoryCountDetailDTO>>.AppendToCacheItem("Cached_InventoryCountDetail_" + objDTO.CompanyId.ToString(), NewCache);
                    }
                }

                return obj.ID;
            }

        }

        public bool Edit(InventoryCountDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryCountDetail obj = new InventoryCountDetail();
                obj.ID = objDTO.ID;
                obj.InventoryCountGUID = objDTO.InventoryCountGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.BinGUID = objDTO.BinGUID;
                obj.CountQuantity = objDTO.CountQuantity;
                obj.CountLineItemDescription = objDTO.CountLineItemDescription;
                obj.CountItemStatus = objDTO.CountItemStatus;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = objDTO.GUID;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = "Web";
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                }

                context.InventoryCountDetails.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<InventoryCountDetailDTO> ObjCache = CacheHelper<IEnumerable<InventoryCountDetailDTO>>.GetCacheItem("Cached_InventoryCountDetail_" + objDTO.CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<InventoryCountDetailDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<InventoryCountDetailDTO> tempC = new List<InventoryCountDetailDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<InventoryCountDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<InventoryCountDetailDTO>>.AppendToCacheItem("Cached_InventoryCountDetail_" + objDTO.CompanyId.ToString(), NewCache);
                }


                return true;
            }
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE InventoryCountDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<InventoryCountDetailDTO> ObjCache = CacheHelper<IEnumerable<InventoryCountDetailDTO>>.GetCacheItem("Cached_InventoryCountDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<InventoryCountDetailDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<InventoryCountDetailDTO>>.AppendToCacheItem("Cached_InventoryCountDetail_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public int GetUnAppliedItemCount(long RoomId, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from icd in context.InventoryCountDetails
                        join ic in context.InventoryCounts on icd.InventoryCountGUID equals ic.GUID
                        where icd.RoomId == RoomId && icd.CompanyId == CompanyID && icd.IsApplied == false && icd.IsDeleted == false && ic.IsDeleted == false && ic.IsClosed == false
                        select icd).Count();

            }
        }
        public bool IsAppliedById(long ID, bool IsApplied)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                (from p in context.InventoryCountDetails
                 where p.ID == ID
                 select p).ToList()
                                         .ForEach(x => x.IsApplied = IsApplied);
                context.SaveChanges();
                return true;
            }
        }
        public bool CheckGuidExistsOrNot(Guid CountDetailGuid)
        {
            Int32 ret = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ret = (from u in context.InventoryCountDetails
                       where u.GUID == CountDetailGuid
                       select u
                        ).ToList().Count();
            }
            if (ret > 0)
            {
                return true;
            }
            return false;
        }
        public bool CheckCountDetailGuidExistsOrNot(Guid CountLineItemDetailGuid)
        {
            Int32 ret = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ret = (from u in context.CountLineItemDetails
                       where u.GUID == CountLineItemDetailGuid
                       select u
                        ).ToList().Count();
            }
            if (ret > 0)
            {
                return true;
            }
            return false;
        }
    }
}


