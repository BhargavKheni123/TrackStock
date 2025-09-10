using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;


namespace eTurns.DAL
{
    public partial class ItemSupplierDetailsDAL : eTurnsBaseDAL
    {
        public IEnumerable<ItemSupplierDetailsDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemSupplierDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.GetCacheItem("Cached_ItemSupplierDetails_" + CompanyID.ToString());
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ItemSupplierDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ItemSupplierDetailsDTO>(@"
                SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                FROM ItemSupplierDetails A 
                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                LEFT OUTER JOIN Room D on A.Room = D.ID 
                WHERE A.IsDeleted != 1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                               select new ItemSupplierDetailsDTO
                                                               {
                                                                   ID = u.ID,
                                                                   ItemGUID = u.ItemGUID,
                                                                   SupplierID = u.SupplierID,
                                                                   Created = u.Created,
                                                                   Updated = u.Updated,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   IsDefault = u.IsDefault,
                                                                   BlanketPOID = u.BlanketPOID,
                                                                   SupplierName = u.SupplierName,
                                                                   SupplierNumber = u.SupplierNumber,
                                                                   Room = u.Room,
                                                                   CompanyID = u.CompanyID,
                                                                   CreatedByName = u.CreatedByName,
                                                                   UpdatedByName = u.UpdatedByName,
                                                                   RoomName = u.RoomName,
                                                                   GUID = u.GUID,
                                                               }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.AddCacheItem("Cached_ItemSupplierDetails_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(t => t.Room == RoomID);
        }
        public IEnumerable<ItemSupplierDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<ItemSupplierDetailsDTO> GetSupplierByItemGuid(Int64 RoomID, Int64 CompanyID, Guid ItemGuid, bool IsDeleted)
        {

            try
            {
                int deleteRecord = 0;
                if (IsDeleted)
                {
                    deleteRecord = 1;
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ItemSupplierDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ItemSupplierDetailsDTO>(@"
                SELECT A.ID,A.ItemGUID,A.SupplierID,A.Created,A.Updated,A.CreatedBy
                   , A.LastUpdatedBy,A.GUID,A.IsDeleted,A.IsArchived,A.IsDefault,
                    A.SupplierNumber,A.Room,A.CompanyID,A.BlanketPOID,A.ReceivedOnWeb
                   , A.ReceivedOn,A.AddedFrom,A.EditedFrom, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,sm.SupplierName
                FROM ItemSupplierDetails A 
                Inner Join Suppliermaster sm on A.SupplierID = sm.ID
                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                LEFT OUTER JOIN Room D on A.Room = D.ID 
                WHERE A.IsDeleted = " + deleteRecord.ToString() + @" AND A.IsArchived = " + deleteRecord.ToString() + @" AND A.CompanyID = " + CompanyID.ToString() + @" and A.Room=" + RoomID.ToString() + @" and A.ItemGuid='" + ItemGuid.ToString() + @"' and Sm.IsDeleted = " + deleteRecord.ToString() + @" AND SM.IsArchived = " + deleteRecord.ToString() + @" AND Sm.CompanyID = " + CompanyID.ToString() + @" and Sm.Room=" + RoomID.ToString())
                                                               select new ItemSupplierDetailsDTO
                                                               {
                                                                   ID = u.ID,
                                                                   ItemGUID = u.ItemGUID,
                                                                   SupplierID = u.SupplierID,
                                                                   Created = u.Created,
                                                                   Updated = u.Updated,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   IsDefault = u.IsDefault,
                                                                   BlanketPOID = u.BlanketPOID,
                                                                   SupplierName = u.SupplierName,
                                                                   SupplierNumber = u.SupplierNumber,
                                                                   Room = u.Room,
                                                                   CompanyID = u.CompanyID,
                                                                   CreatedByName = u.CreatedByName,
                                                                   UpdatedByName = u.UpdatedByName,
                                                                   RoomName = u.RoomName,
                                                                   GUID = u.GUID,
                                                               }).AsParallel().ToList();
                    return obj;
                }
            }
            catch
            {
                return null;
            }

        }
        public IEnumerable<ItemSupplierDetailsDTO> GetItemSuppliers(Int64 RoomID, Int64 CompanyId, Guid ItemGuid, bool IsDeleted)
        {
            return GetSupplierByItemGuid(RoomID, CompanyId, ItemGuid, IsDeleted).OrderBy("ID DESC");
        }

        public IEnumerable<ItemSupplierDetailsDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<ItemSupplierDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            IEnumerable<ItemSupplierDetailsDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<ItemSupplierDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ItemSupplierDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public ItemSupplierDetailsDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);
        }

        public ItemSupplierDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemSupplierDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemSupplierDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ItemSupplierDetailsDTO
                        {
                            ID = u.ID,
                            ItemGUID = u.ItemGUID,
                            SupplierID = u.SupplierID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            IsDefault = u.IsDefault,
                            SupplierName = u.SupplierName,
                            SupplierNumber = u.SupplierNumber,
                            Room = u.Room,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                        }).SingleOrDefault();
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
                        strQuery += "UPDATE ItemSupplierDetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<ItemSupplierDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.GetCacheItem("Cached_ItemSupplierDetails_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ItemSupplierDetailsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.AppendToCacheItem("Cached_ItemSupplierDetails_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public bool DeleteRecordsExcept(string IDs, Guid ItemGUID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                strQuery += "UPDATE ItemSupplierDetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ItemGUID = '" + ItemGUID.ToString() + "' AND  ID Not in( ";
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

                return true;
            }

        }
        public bool UpdateItemSuppForBlanketPO(Int64 BlanketPOID, Int64 RoomID, Int64 CompanyID)
        {
            bool IsSucess = true;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string tempUpdateQuery = @"Update ItemSupplierDetails Set BlanketPOID=null,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' where BlanketPOID = '" + BlanketPOID + "' and Room = " + RoomID + " and CompanyID = " + CompanyID + "";
                    context.ExecuteStoreCommand(tempUpdateQuery);
                    List<ItemSupplierDetailsDTO> objTemp = new List<ItemSupplierDetailsDTO>();
                    CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.AppendToCacheItem("Cached_ItemSupplierDetails_" + CompanyID, objTemp);
                    return IsSucess;
                }
                catch
                {
                    IsSucess = false;
                    return IsSucess;
                }
            }
        }
        public List<ItemSupplierDetailsDTO> GetitemSupplers(Guid itemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemSupplierDetailsDTO>(@"
                SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                FROM ItemSupplierDetails A 
                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                LEFT OUTER JOIN Room D on A.Room = D.ID 
                WHERE A.IsDeleted != 1 AND A.IsArchived != 1 AND A.ItemGUID = '" + itemGUID.ToString() + "'")
                        select new ItemSupplierDetailsDTO
                        {
                            ID = u.ID,
                            ItemGUID = u.ItemGUID,
                            SupplierID = u.SupplierID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            IsDefault = u.IsDefault,
                            BlanketPOID = u.BlanketPOID,
                            SupplierName = u.SupplierName,
                            SupplierNumber = u.SupplierNumber,
                            Room = u.Room,
                            CompanyID = u.CompanyID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            GUID = u.GUID,
                        }).ToList();
            }
        }

        public bool CheckSupplierDuplicate(string SupplierNumber, long RoomID, long CompanyID, Guid? Itemguid, bool IsBOM = false)
        {
            bool result = false;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var qry = (from em in context.ItemSupplierDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.SupplierNumber == SupplierNumber && em.ItemGUID != Itemguid
                               && em.IsArchived == false && em.IsDeleted == false && em.Room == RoomID && em.CompanyID == CompanyID
                               && it.IsDeleted == false && it.Room == RoomID
                               && it.IsBOMItem == IsBOM
                               select em);
                    if (qry.Any())
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public bool CheckSupplierDuplicateByItemNumber(string SupplierNumber, long RoomID, long CompanyID, string ItemNumber, bool IsBOM = false)
        {
            bool result = false;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.ItemSupplierDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.SupplierNumber == SupplierNumber && it.ItemNumber != ItemNumber
                               && em.IsArchived == false && em.IsDeleted == false && em.Room == RoomID && em.CompanyID == CompanyID
                               && it.IsDeleted == false && it.Room == RoomID
                               && it.IsBOMItem == IsBOM
                               select em);
                    if (qry.Any())
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public bool CheckSupplierDuplicateByBOMItemNumber(string SupplierNumber, string ItemNumber, bool IsBOM = true)
        {
            bool result = false;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.ItemSupplierDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.SupplierNumber == SupplierNumber && it.ItemNumber != ItemNumber
                               && em.IsArchived == false && em.IsDeleted == false
                               && it.IsDeleted == false
                               && it.IsBOMItem == IsBOM
                               select em);
                    if (qry.Any())
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public bool CheckBOMSupplierDuplicate(string SupplierNumber, Guid? Itemguid, bool IsBOM = false)
        {
            bool result = false;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.ItemSupplierDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.SupplierNumber == SupplierNumber && em.ItemGUID != Itemguid
                               && em.IsArchived == false && em.IsDeleted == false
                               && it.IsDeleted == false
                               && it.IsBOMItem == IsBOM
                               select em);
                    if (qry.Any())
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public void DeleteItemSupplierDetailsByItemGUID(Guid[] ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<ItemSupplierDetail> itemsSupplierDetails = context.ItemSupplierDetails.Where(t => ItemGUID.Contains(t.ItemGUID ?? Guid.Empty));
                if (itemsSupplierDetails.Any())
                {
                    foreach (var item in itemsSupplierDetails)
                    {
                        item.IsDeleted = true;
                    }
                    context.SaveChanges();
                }
            }
        }
        public List<ItemSupplier> GetItemSupplierExport(long companyId, long RoomId)
        {
            List<ItemSupplier> lstSupplierDetailsmain = new List<ItemSupplier>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSupplierDetailsmain = (from p in context.ItemSupplierDetails
                                          join isup in context.SupplierBlanketPODetails on p.BlanketPOID equals isup.ID into SBP_P_Join
                                          from SB in SBP_P_Join.DefaultIfEmpty()
                                          join im in context.ItemMasters on p.ItemGUID equals im.GUID into KI_P_Join
                                          from IM in KI_P_Join.DefaultIfEmpty()
                                          where p.CompanyID == companyId && p.Room == RoomId && (p.IsArchived ?? false) == false && (p.IsDeleted ?? false) == false
                                          && IM.IsDeleted == false
                                          select new ItemSupplier
                                          {
                                              ID = p.ID,
                                              ItemNumber = IM.ItemNumber,
                                              SupplierNumber = p.SupplierNumber,
                                              SupplierName = p.SupplierName,
                                              ItemGUID = p.ItemGUID,
                                              IsDefault = p.IsDefault,
                                              BlanketPOName = SB.BlanketPO,
                                          }).ToList();
            }
            return lstSupplierDetailsmain;
        }
        public void UnDeleteItemSupplierDetailsByItemGUID(Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                eTurns.DAL.ItemMaster itemMaster = context.ItemMasters.Where(t => t.GUID == ItemGUID).FirstOrDefault();
                if (itemMaster != null)
                {

                    ItemSupplierDetail itemsSupplierDetails = context.ItemSupplierDetails.Where(t => t.SupplierID == itemMaster.SupplierID && t.ItemGUID == ItemGUID && (t.IsDefault ?? false) == true).FirstOrDefault();
                    if (itemsSupplierDetails != null)
                    {
                        itemsSupplierDetails.IsDeleted = false;
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}


