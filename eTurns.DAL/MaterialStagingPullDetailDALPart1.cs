using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class MaterialStagingPullDetailDAL : eTurnsBaseDAL
    {
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MaterialStagingPullDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", CustomerOwnedQuantity=0,ConsignedQuantity=0,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MaterialStagingPullDetailDTO> objTemp = new List<MaterialStagingPullDetailDTO>();// ObjCache.ToList();
                    //objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    //ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString(), objTemp);
                }

                return true;
            }
        }

        public bool UnDeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MaterialStagingPullDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MaterialStagingPullDetailDTO> objTemp = new List<MaterialStagingPullDetailDTO>();// ObjCache.ToList();
                    CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString(), objTemp);
                }

                return true;
            }
        }

        public bool UnDeleteRecordsByGUID(string GUIDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in GUIDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MaterialStagingPullDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MaterialStagingPullDetailDTO> objTemp = new List<MaterialStagingPullDetailDTO>();// ObjCache.ToList();
                    CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString(), objTemp);
                }

                return true;
            }
        }

        public bool DeleteRecordsFromOrder(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                Double Totalqty = 0;
                Guid MaterialStagingDetailGUID = Guid.Empty;
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        MaterialStagingPullDetailDTO objDTO = GetRecord(Guid.Parse(item), RoomID, CompanyID);
                        if (MaterialStagingDetailGUID == Guid.Empty)
                            MaterialStagingDetailGUID = objDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                        Totalqty += objDTO.ConsignedQuantity.GetValueOrDefault(0) + objDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                        strQuery += "UPDATE MaterialStagingPullDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                if (MaterialStagingDetailGUID != Guid.Empty)
                {
                    MaterialStagingDetailDAL detailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    //MaterialStagingDetailDTO objDetailDTO = detailDAL.GetRecord(MaterialStagingDetailGUID, RoomID, CompanyID);
                    MaterialStagingDetailDTO objDetailDTO = detailDAL.GetMaterialStagingDetailByGUID(MaterialStagingDetailGUID, RoomID, CompanyID);
                    if (Totalqty <= objDetailDTO.Quantity)
                        objDetailDTO.Quantity = objDetailDTO.Quantity - Totalqty;
                    else
                        objDetailDTO.Quantity = 0;

                    detailDAL.Edit(objDetailDTO);
                }
                //Get Cached-Media
                IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MaterialStagingPullDetailDTO> objTemp = new List<MaterialStagingPullDetailDTO>();// ObjCache.ToList();
                    //objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    //ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + CompanyID.ToString(), objTemp);
                }

                return true;
            }
        }

        public List<MaterialStagingPullDetailDTO> GetRecordsByBinNumberAndLotSerial(Guid ItemGuid, string BinNumber, string LotSerialNumber, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<MaterialStagingPullDetailDTO> lstItemLocationDetailsDTO = new List<MaterialStagingPullDetailDTO>();

                lstItemLocationDetailsDTO = (from u in context.ExecuteStoreQuery<MaterialStagingPullDetailDTO>(@"SELECT A.*, I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,
                                                                                                           D.RoomName, E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber 
                                                                                                           FROM MaterialStagingPullDetail A 
                                                                                                           Inner Join BinMaster BM on BM.ID = A.StagingBinId
                                                                                                           LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                                           LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                                           LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID LEFT OUTER JOIN BinMaster I on A.BinID = I.ID  AND A.ItemGUID = I.ItemGUID
                                                                                                           WHERE A.ItemGUID = '" + ItemGuid.ToString() + @"' AND BM.BinNumber = '" + BinNumber + @"' 
                                                                                                           	     AND (ISNULL(A.LotNumber,'') = '" + LotSerialNumber + @"' OR ISNULL(A.SerialNumber,'') = '" + LotSerialNumber + @"')
                                                                                                                 AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + "")
                                             select new MaterialStagingPullDetailDTO
                                             {
                                                 ID = u.ID,

                                                 BinID = u.BinID,
                                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                 ConsignedQuantity = u.ConsignedQuantity,
                                                 LotNumber = u.LotNumber,
                                                 SerialNumber = u.SerialNumber,
                                                 Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM /dd/yy") : u.Expiration,
                                                 Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                                                 ItemCost = u.ItemCost,
                                                 GUID = u.GUID,
                                                 ItemGUID = u.ItemGUID,
                                                 Created = u.Created,
                                                 Updated = u.Updated,
                                                 CreatedBy = u.CreatedBy,
                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                 IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                 IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                 CompanyID = u.CompanyID,
                                                 Room = u.Room,
                                                 CreatedByName = u.CreatedByName,
                                                 UpdatedByName = u.UpdatedByName,
                                                 RoomName = u.RoomName,
                                                 BinNumber = u.BinNumber,
                                                 ItemNumber = u.ItemNumber,
                                                 SerialNumberTracking = u.SerialNumberTracking,
                                                 LotNumberTracking = u.LotNumberTracking,
                                                 DateCodeTracking = u.DateCodeTracking,
                                                 OrderDetailGUID = u.OrderDetailGUID,
                                                 MaterialStagingdtlGUID = u.MaterialStagingdtlGUID,
                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                 ReceivedOn = u.ReceivedOn,
                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                 InitialQuantityWeb = u.InitialQuantityWeb ?? 0,
                                                 InitialQuantityPDA = u.InitialQuantityPDA ?? 0
                                             }).ToList();

                return lstItemLocationDetailsDTO;
            }
        }

        public void UpdateItemCost(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string EditFrom, long SessionUserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStagingPullDetail oItemMaster = context.MaterialStagingPullDetails.Where(x => x.GUID == ItemGUID && x.CompanyID == CompanyID && x.Room == RoomID).FirstOrDefault();
                Room oRoom = context.Rooms.Where(x => x.ID == RoomID && x.CompanyID == CompanyID).FirstOrDefault();
                if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !oItemMaster.IsConsignedSerialLot)
                {
                    string strQry = @"UPDATE MaterialStagingPullDetails SET ItemCost = " + oItemMaster.ItemCost.GetValueOrDefault(0) + @", 
                                                                    Received = '" + DateTimeUtility.DateTimeNow + @"', 
                                                                    EditedFrom = '" + EditFrom + @"'
                                      Where CompanyID= " + CompanyID + @" AND Room=" + RoomID + @" AND ItemGUID ='" + ItemGUID.ToString() + @"' 
                                      AND (ISNULL(ConsignedQuantity,0) > 0 OR ISNULL(CustomerOwnedQuantity,0) > 0)";

                    context.ExecuteStoreCommand(strQry);
                    ItemMasterDTO oItemDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, ItemGUID);
                    new ItemMasterDAL(base.DataBaseName).Edit(oItemDTO, SessionUserId);
                }
            }
        }

        public bool UpdateMaterialStagingPullDetailIntialQtyandPoolQty(Int64 RoomID, Int64 CompanyId, Int64 UserID, Guid MaterialStagingPulldetailGuid, Guid ItemGuid, double InitialQuantityWeb)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                strQuery += "UPDATE MaterialStagingPullDetail SET Updated = getutcdate() , LastUpdatedBy = '" + UserID + "', InitialQuantityWeb='" + InitialQuantityWeb + "',CustomerOwnedQuantity=0,ConsignedQuantity=0,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE Guid ='" + MaterialStagingPulldetailGuid + "' AND ItemGuid= '" + ItemGuid + "';";
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }

        
    }
}
