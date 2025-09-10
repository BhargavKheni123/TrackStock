using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;


namespace eTurns.DAL
{
    partial class ItemManufacturerDetailsDAL : eTurnsBaseDAL
    {
        public IEnumerable<ItemManufacturerDetailsDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            IEnumerable<ItemManufacturerDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.GetCacheItem("Cached_ItemManufacturerDetails_" + CompanyID.ToString());

            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ItemManufacturerDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ItemManufacturerDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemManufacturerDetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted != 1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                                   select new ItemManufacturerDetailsDTO
                                                                   {
                                                                       ID = u.ID,

                                                                       ManufacturerID = u.ManufacturerID,
                                                                       Created = u.Created,
                                                                       Updated = u.Updated,
                                                                       CreatedBy = u.CreatedBy,
                                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                                       IsDeleted = u.IsDeleted,
                                                                       IsArchived = u.IsArchived,
                                                                       IsDefault = u.IsDefault,
                                                                       ManufacturerName = u.ManufacturerName,
                                                                       ManufacturerNumber = u.ManufacturerNumber,
                                                                       Room = u.Room,
                                                                       CompanyID = u.CompanyID,
                                                                       CreatedByName = u.CreatedByName,
                                                                       UpdatedByName = u.UpdatedByName,
                                                                       RoomName = u.RoomName,
                                                                       ItemGUID = u.ItemGUID,
                                                                       GUID = u.GUID,
                                                                   }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.AddCacheItem("Cached_ItemManufacturerDetails_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(t => t.Room == RoomID);
        }
        public IEnumerable<ItemManufacturerDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<ItemManufacturerDetailsDTO> GetManufacturerByItemGuid(Int64 RoomID, Int64 CompanyID, Guid ItemGuid, bool IsDeleted)
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
                    IEnumerable<ItemManufacturerDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ItemManufacturerDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', 
                    C.UserName AS UpdatedByName, D.RoomName 
                    FROM ItemManufacturerDetails A 
                    LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted = " + deleteRecord.ToString() + @" AND A.IsArchived  = " + deleteRecord.ToString() + @"
                    AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room=" + RoomID.ToString() + @" and A.ItemGuid='" + ItemGuid.ToString() + "'")
                                                                   select new ItemManufacturerDetailsDTO
                                                                   {
                                                                       ID = u.ID,

                                                                       ManufacturerID = u.ManufacturerID,
                                                                       Created = u.Created,
                                                                       Updated = u.Updated,
                                                                       CreatedBy = u.CreatedBy,
                                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                                       IsDeleted = u.IsDeleted,
                                                                       IsArchived = u.IsArchived,
                                                                       IsDefault = u.IsDefault,
                                                                       ManufacturerName = u.ManufacturerName,
                                                                       ManufacturerNumber = u.ManufacturerNumber,
                                                                       Room = u.Room,
                                                                       CompanyID = u.CompanyID,
                                                                       CreatedByName = u.CreatedByName,
                                                                       UpdatedByName = u.UpdatedByName,
                                                                       RoomName = u.RoomName,
                                                                       ItemGUID = u.ItemGUID,
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
        public IEnumerable<ItemManufacturerDetailsDTO> GetItemManufacturers(Int64 RoomID, Int64 CompanyId, Guid ItemGuid, bool IsDeleted)
        {
            return GetManufacturerByItemGuid(RoomID, CompanyId, ItemGuid, IsDeleted).OrderBy("ID DESC");
        }
        /// <summary>
        /// Get Paged Records from the ItemManufacturerDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ItemManufacturerDetailsDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<ItemManufacturerDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            IEnumerable<ItemManufacturerDetailsDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<ItemManufacturerDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
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
                //IEnumerable<ItemManufacturerDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        /// <summary>
        /// Get Particullar Record from the ItemManufacturerDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ItemManufacturerDetailsDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);
        }
        public ItemManufacturerDetailsDTO GetRecordByItemManufacturerNumber(string ManufacturerNumber, Int64 RoomID, Int64 CompanyID, Guid ItemGUID)
        {
            return GetCachedData(RoomID, CompanyID).Where(i => i.IsDeleted == false && i.ItemGUID == ItemGUID && i.ManufacturerNumber == ManufacturerNumber).FirstOrDefault();
        }


        /// <summary>
        /// Get Particullar Record from the ItemManufacturerDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ItemManufacturerDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemManufacturerDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemManufacturerDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ItemManufacturerDetailsDTO
                        {
                            ID = u.ID,
                            ItemGUID = u.ItemGUID,
                            ManufacturerID = u.ManufacturerID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            IsDefault = u.IsDefault,
                            ManufacturerName = u.ManufacturerName,
                            ManufacturerNumber = u.ManufacturerNumber,
                            Room = u.Room,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                        }).SingleOrDefault();
            }
        }
        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ItemManufacturerDetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<ItemManufacturerDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.GetCacheItem("Cached_ItemManufacturerDetails_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ItemManufacturerDetailsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.AppendToCacheItem("Cached_ItemManufacturerDetails_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public List<ItemManufacturerDetailsDTO> GetitemMans(Guid itemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemManufacturerDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemManufacturerDetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted != 1 AND A.IsArchived != 1 AND A.ItemGUID = '" + itemGUID.ToString() + "'")
                        select new ItemManufacturerDetailsDTO
                        {
                            ID = u.ID,

                            ManufacturerID = u.ManufacturerID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            IsDefault = u.IsDefault,
                            ManufacturerName = u.ManufacturerName,
                            ManufacturerNumber = u.ManufacturerNumber,
                            Room = u.Room,
                            CompanyID = u.CompanyID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            ItemGUID = u.ItemGUID,
                            GUID = u.GUID,
                        }).AsParallel().ToList();

            }
        }

        public bool DeleteRecordsExcept(string IDs, Guid ItemGUID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                strQuery += "UPDATE ItemManufacturerDetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ItemGUID = '" + ItemGUID.ToString() + "' ";
                if (IDs != "")
                {
                    strQuery += "AND ID Not in( ";
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            strQuery += item.ToString() + ",";
                        }
                    }
                    strQuery = strQuery.Substring(0, strQuery.Length - 1);
                    strQuery += ");";
                }
                context.ExecuteStoreCommand(strQuery);

                CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();

                ItemManufacturerDetailsDAL objItemLocationLevelQuanityDAL = new ItemManufacturerDetailsDAL(base.DataBaseName);
                objItemLocationLevelQuanityDAL.GetCachedData(0, CompanyID);
                return true;
            }
        }

        public string CheckForDuplicateManufacturerNo(string ManufacturerNumber, Int64 ManufacturerID, string ItemNumber, Int64 RoomID, Int64 CompanyID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.ItemManufacturerDetails
                           join it in context.ItemMasters on em.ItemGUID equals it.GUID
                           where em.ManufacturerNumber == ManufacturerNumber && em.ManufacturerID != ManufacturerID && it.ItemNumber != ItemNumber
                           && em.IsArchived == false && em.IsDeleted == false && em.Room == RoomID && em.CompanyID == CompanyID
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;
        }

        public bool CheckManufacturerNoDuplicateByItemNumber(string ManufacturerNumber, long RoomID, long CompanyID, string ItemNumber, bool IsBOM = false)
        {
            bool result = false;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.ItemManufacturerDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.ManufacturerNumber == ManufacturerNumber && it.ItemNumber != ItemNumber
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
        public bool CheckManufacturerNoDuplicateByBOMItemNumber(string ManufacturerNumber, string ItemNumber, bool IsBOM = true)
        {
            bool result = false;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.ItemManufacturerDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.ManufacturerNumber == ManufacturerNumber && it.ItemNumber != ItemNumber
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
        public bool CheckBOMManufacturerDuplicate(string ManufacturerNumber, Guid? Itemguid, bool IsBOM = false)
        {
            bool result = false;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var qry = (from em in context.ItemManufacturerDetails
                               join it in context.ItemMasters on em.ItemGUID equals it.GUID
                               where em.ManufacturerNumber == ManufacturerNumber && em.ItemGUID != Itemguid
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
        public void DeleteItemManufacturerDetailsByItemGUID(Guid[] ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<ItemManufacturerDetail> itemsManufacturerDetails = context.ItemManufacturerDetails.Where(t => ItemGUID.Contains(t.ItemGUID ?? Guid.Empty));
                if (itemsManufacturerDetails.Any())
                {
                    foreach (var item in itemsManufacturerDetails)
                    {
                        item.IsDeleted = true;
                    }
                    context.SaveChanges();
                }
            }
        }

    }
}
