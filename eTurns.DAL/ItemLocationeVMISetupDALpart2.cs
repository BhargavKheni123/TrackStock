using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;


namespace eTurns.DAL
{
    public partial class ItemLocationeVMISetupDAL : eTurnsBaseDAL
    {
        #region [Un used methods]

        public IEnumerable<ItemLocationeVMISetupDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationeVMISetupDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.GetCacheItem("Cached_ItemLocationeVMISetup_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ItemLocationeVMISetupDTO> obj = (from u in context.ExecuteStoreQuery<ItemLocationeVMISetupDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemLocationeVMISetup A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                                                                 select new ItemLocationeVMISetupDTO
                                                                 {
                                                                     ID = u.ID,
                                                                     BinID = u.BinID,
                                                                     eVMISensorPort = u.eVMISensorPort,
                                                                     eVMISensorID = u.eVMISensorID,
                                                                     PoundsPerPiece = u.PoundsPerPiece,
                                                                     Quantity = u.Quantity,
                                                                     CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                                     ConsignedQuantity = u.ConsignedQuantity,
                                                                     Created = u.Created,
                                                                     Updated = u.Updated,
                                                                     CreatedBy = u.CreatedBy,
                                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                                     IsDeleted = u.IsDeleted,
                                                                     IsArchived = u.IsArchived,
                                                                     CompanyID = u.CompanyID,
                                                                     Room = u.Room,
                                                                     ItemGUID = u.ItemGUID,
                                                                     CreatedByName = u.CreatedByName,
                                                                     UpdatedByName = u.UpdatedByName,
                                                                     RoomName = u.RoomName,
                                                                 }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.AddCacheItem("Cached_ItemLocationeVMISetup_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }
        public IEnumerable<ItemLocationeVMISetupDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }
        public ItemLocationeVMISetupDTO GetRecordByItemGUID(Guid? ItemGuid, Int64 BinID, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Where(t => t.ItemGUID == ItemGuid && t.BinID == BinID).SingleOrDefault();
        }
        public ItemLocationeVMISetupDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);
        }
        public ItemLocationeVMISetupDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemLocationeVMISetupDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemLocationeVMISetup_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ItemLocationeVMISetupDTO
                        {
                            ID = u.ID,
                            BinID = u.BinID,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
                            PoundsPerPiece = u.PoundsPerPiece,
                            Quantity = u.Quantity,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            ItemGUID = u.ItemGUID,
                        }).SingleOrDefault();
            }
        }
        public bool Edit(ItemLocationeVMISetupDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationeVMISetup obj = new ItemLocationeVMISetup();
                obj.ID = objDTO.ID;
                obj.BinID = objDTO.BinID;
                obj.eVMISensorPort = objDTO.eVMISensorPort;
                obj.eVMISensorID = objDTO.eVMISensorID;
                obj.PoundsPerPiece = objDTO.PoundsPerPiece;
                obj.Quantity = objDTO.Quantity;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.ItemGUID = objDTO.ItemGUID;
                context.ItemLocationeVMISetups.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<ItemLocationeVMISetupDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.GetCacheItem("Cached_ItemLocationeVMISetup_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ItemLocationeVMISetupDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<ItemLocationeVMISetupDTO> tempC = new List<ItemLocationeVMISetupDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<ItemLocationeVMISetupDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.AppendToCacheItem("Cached_ItemLocationeVMISetup_" + objDTO.CompanyID.ToString(), NewCache);
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
                        strQuery += "UPDATE ItemLocationeVMISetup SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<ItemLocationeVMISetupDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.GetCacheItem("Cached_ItemLocationeVMISetup_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ItemLocationeVMISetupDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.AppendToCacheItem("Cached_ItemLocationeVMISetup_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }
        public IEnumerable<ItemLocationeVMISetupDTO> GetItemWiseLocationList(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationeVMISetupDTO> ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from u in context.ExecuteStoreQuery<ItemLocationeVMISetupDTO>(@"
                                SELECT I.ItemNumber,A.BinID,A.eVMISensorPort,A.eVMISensorID ,A.CompanyID,A.Room,A.ItemGUID,
                                B.BinNumber,isnull(I.WeightPerPiece,0) as WeightPerPiece,
                                (select sum(ISNULL(CustomerOwnedQuantity,0)) from itemlocationdetails 
                                where BinID = A.BinID And ItemGUID = A.ItemGUID and IsDeleted = 0 and IsArchived = 0) as CustomerOwnedQuantity,
                                (select sum(ISNULL(ConsignedQuantity,0)) from itemlocationdetails 
                                where BinID = A.BinID And ItemGUID = A.ItemGUID and IsDeleted = 0 and IsArchived = 0) as ConsignedQuantity,
                                I.Consignment
                                FROM ItemLocationeVMISetup A 
                                inner join BinMaster B on A.BinID = B.ID
                                inner join ItemMaster I on A.ItemGUID = I.GUID AND I.SerialNumberTracking = 0 and I.LotNumberTracking = 0 and I.DateCodeTracking =0 and  isnull(I.isdeleted,0)=0  and isnull(I.isarchived,0)=0 
                                where isnull(B.isdeleted,0)=0 and isnull(B.isarchived,0)=0  and isnull(I.isdeleted,0)=0  and isnull(I.isarchived,0)=0 and A.CompanyID = " + CompanyID.ToString() + @" 
                                And A.Room = " + RoomID.ToString() + ""
                            )
                            select new ItemLocationeVMISetupDTO
                            {
                                IsChecked = false,
                                ID = u.ID,
                                BinID = u.BinID,
                                eVMISensorPort = u.eVMISensorPort,
                                eVMISensorID = u.eVMISensorID,
                                WeightPerPiece = u.WeightPerPiece,
                                ItemGUID = u.ItemGUID,
                                ItemNumber = u.ItemNumber,
                                CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),
                                ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),
                                CompanyID = u.CompanyID,
                                Room = u.Room,
                                BinNumber = u.BinNumber,
                                Consignment = u.Consignment,
                                Quantity = u.CustomerOwnedQuantity.GetValueOrDefault(0) + u.ConsignedQuantity.GetValueOrDefault(0)
                            }).AsParallel().ToList();
            }

            return ObjCache.Where(x => x.Room == RoomID); ;
        }
        #endregion
    }
}


