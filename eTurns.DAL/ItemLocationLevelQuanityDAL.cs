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

namespace eTurns.DAL
{
    public class ItemLocationLevelQuanityDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ItemLocationLevelQuanityDAL()
        {

        }

        public ItemLocationLevelQuanityDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public ItemLocationLevelQuanityDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }

        #endregion

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ItemLocationLevelQuanityDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationLevelQuanityDTO> ObjCache; //= CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.GetCacheItem("Cached_ItemLocationLevelQuanity_" + CompanyID.ToString());
            //if (ObjCache == null)
            //{
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ItemLocationLevelQuanityDTO> obj = (from u in context.ExecuteStoreQuery<ItemLocationLevelQuanityDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, E.BinNumber, QTY.ConsignedQuantity,QTY.CustomerOwnedQuantity, V.eVMISensorPort, V.eVMISensorID   
                            FROM ItemLocationLevelQuanity A 
                            LEFT OUTER JOIN ItemLocationQTY QTY on A.ItemGUID  = QTY.ItemGUID and A.BinID = QTY.BinID  
							LEFT OUTER JOIN ItemLocationeVMISetup V on A.ItemGUID  = V.ItemGUID and A.BinID = V.BinID  
                            LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            LEFT OUTER JOIN BinMaster E on A.BinID = E.ID
                            WHERE A.CompanyID = " + CompanyID.ToString() + " AND A.IsDeleted != 1 AND A.IsArchived != 1 ")
                                                                select new ItemLocationLevelQuanityDTO
                                                                {
                                                                    ID = u.ID,
                                                                    BinID = u.BinID,
                                                                    BinNumber = u.BinNumber,
                                                                    CriticalQuantity = u.CriticalQuantity,
                                                                    MinimumQuantity = u.MinimumQuantity,
                                                                    MaximumQuantity = u.MaximumQuantity,
                                                                    ItemGUID = u.ItemGUID,
                                                                    CompanyID = u.CompanyID,
                                                                    Room = u.Room,
                                                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                    Created = u.Created,
                                                                    LastUpdated = u.LastUpdated,
                                                                    CreatedBy = u.CreatedBy,
                                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                                    CreatedByName = u.CreatedByName,
                                                                    UpdatedByName = u.UpdatedByName,
                                                                    RoomName = u.RoomName,
                                                                    GUID = u.GUID,
                                                                    SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity, 
                                                                    eVMISensorPort = u.eVMISensorPort,
                                                                    eVMISensorID  = u.eVMISensorID 
                                                                }).AsParallel().ToList();
                ObjCache = CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.AddCacheItem("Cached_ItemLocationLevelQuanity_" + CompanyID.ToString(), obj);
            }
            // }
            return ObjCache.Where(t => t.Room == RoomID);
        }

        /// <summary>
        /// Get Paged Records from the ItemLocationLevelQuanity Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ItemLocationLevelQuanityDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<ItemLocationLevelQuanityDTO> GetAllRecordsItemWise(Guid ItemGUID, Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).Where(t => t.ItemGUID == ItemGUID);
        }

        public IEnumerable<ItemLocationLevelQuanityDTO> GetAllRecordsItemBinWise(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, Int64 Id)
        {
            return GetCachedData(RoomID, CompanyId).Where(t => t.ItemGUID == ItemGUID && t.BinID == Id);
        }
        
        public ItemLocationLevelQuanityDTO GetCachedData(Guid ItemGUID, Int64 BinID, Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationLevelQuanityDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.GetCacheItem("Cached_ItemLocationLevelQuanity_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ItemLocationLevelQuanityDTO> obj = (from u in context.ExecuteStoreQuery<ItemLocationLevelQuanityDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, E.BinNumber 
                    FROM ItemLocationLevelQuanity A 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN BinMaster E on A.BinID = E.ID
                    WHERE A.CompanyID = " + CompanyID.ToString())
                                                                    select new ItemLocationLevelQuanityDTO
                                                                    {
                                                                        ID = u.ID,
                                                                        BinID = u.BinID,
                                                                        BinNumber = u.BinNumber,
                                                                        CriticalQuantity = u.CriticalQuantity,
                                                                        MinimumQuantity = u.MinimumQuantity,
                                                                        MaximumQuantity = u.MaximumQuantity,
                                                                        ItemGUID = u.ItemGUID,
                                                                        CompanyID = u.CompanyID,
                                                                        Room = u.Room,
                                                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                        Created = u.Created,
                                                                        LastUpdated = u.LastUpdated,
                                                                        CreatedBy = u.CreatedBy,
                                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                                        CreatedByName = u.CreatedByName,
                                                                        UpdatedByName = u.UpdatedByName,
                                                                        RoomName = u.RoomName,
                                                                        GUID = u.GUID,
                                                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                                    }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.AddCacheItem("Cached_ItemLocationLevelQuanity_" + CompanyID.ToString(), obj);
                }
            }
            return ObjCache.Where(t => t.Room == RoomID && t.ItemGUID == ItemGUID && t.BinID == BinID).FirstOrDefault();
        }

        /// <summary>
        /// Get Particullar Record from the ItemLocationLevelQuanity by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ItemLocationLevelQuanityDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);
        }

        /// <summary>
        /// Insert Record in the DataBase ItemLocationLevelQuanity
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ItemLocationLevelQuanityDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTime.Now;
            objDTO.LastUpdated = DateTime.Now;
            objDTO.GUID = Guid.NewGuid();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationLevelQuanity obj = new ItemLocationLevelQuanity();
                obj.ID = 0;
                obj.BinID = objDTO.BinID;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.Created = DateTime.Now;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsArchived = objDTO.IsArchived;
                obj.GUID = objDTO.GUID;
                context.AddToItemLocationLevelQuanities(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                //string strUpdateOnHand = "EXEC [dbo].[AutoCartEntryonInventoryUpDown] '" + obj.ItemGUID.ToString() + "', " + obj.CreatedBy;
                //context.ExecuteStoreCommand(strUpdateOnHand);
                //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.CreatedBy ?? 0);

                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<ItemLocationLevelQuanityDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.GetCacheItem("Cached_ItemLocationLevelQuanity_" + objDTO.CompanyID.ToString());
                    if (ObjCache != null)
                    {
                        List<ItemLocationLevelQuanityDTO> tempC = new List<ItemLocationLevelQuanityDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<ItemLocationLevelQuanityDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.AppendToCacheItem("Cached_ItemLocationLevelQuanity_" + objDTO.CompanyID.ToString(), NewCache);
                    }
                }
                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ItemLocationLevelQuanityDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTime.Now;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationLevelQuanity obj = new ItemLocationLevelQuanity();
                obj.ID = objDTO.ID;
                obj.BinID = objDTO.BinID;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.Created = objDTO.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsArchived = objDTO.IsArchived;
                obj.GUID = objDTO.GUID;
                context.ItemLocationLevelQuanities.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                //Get Cached-Media

                //string strUpdateOnHand = "EXEC [dbo].[AutoCartEntryonInventoryUpDown] '" + obj.ItemGUID.ToString() + "', " + obj.CreatedBy;
                //context.ExecuteStoreCommand(strUpdateOnHand);
                //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.CreatedBy ?? 0);

                IEnumerable<ItemLocationLevelQuanityDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.GetCacheItem("Cached_ItemLocationLevelQuanity_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ItemLocationLevelQuanityDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<ItemLocationLevelQuanityDTO> tempC = new List<ItemLocationLevelQuanityDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<ItemLocationLevelQuanityDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.AppendToCacheItem("Cached_ItemLocationLevelQuanity_" + objDTO.CompanyID.ToString(), NewCache);
                }


                return true;
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
                        strQuery += "UPDATE ItemLocationLevelQuanity SET Updated = getdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<ItemLocationLevelQuanityDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.GetCacheItem("Cached_ItemLocationLevelQuanity_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ItemLocationLevelQuanityDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.AppendToCacheItem("Cached_ItemLocationLevelQuanity_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecordsExcept(string IDs, Guid ItemGUID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IDs != "")
                {
                    string strQuery = "";
                    strQuery += "UPDATE ItemLocationLevelQuanity SET LastUpdated = getdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ItemGUID = '" + ItemGUID.ToString() + "' AND ID Not in( ";
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


                    //string strUpdateOnHand = "EXEC [dbo].[AutoCartEntryonInventoryUpDown] '" + ItemGUID.ToString() + "', " + userid;
                    //context.ExecuteStoreCommand(strUpdateOnHand);
                    //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(ItemGUID, userid);

                    //Get Cached-Media
                    CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.InvalidateCache();

                    ItemLocationLevelQuanityDAL objItemLocationLevelQuanityDAL = new ItemLocationLevelQuanityDAL();
                    objItemLocationLevelQuanityDAL.GetCachedData(0, CompanyID);
                }

                return true;
            }
        }

        public List<ItemLocationLevelQuanityDTO> GetAllRecordsItemWise(Guid? ItemGUID, long? RoomID, long? CompanyId)
        {
            return GetCachedData(Convert.ToInt64(RoomID), Convert.ToInt64(CompanyId)).Where(t => t.ItemGUID == ItemGUID).ToList();
        }
    }
}


