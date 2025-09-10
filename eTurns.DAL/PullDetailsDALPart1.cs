using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class PullDetailsDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuid(Guid ItemGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var PullDetailsDTO = (from u in context.ExecuteStoreQuery<PullDetailsDTO>(
                                               @"SELECT PM.PullCredit,A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, 
                                                    D.RoomName,E.ItemNumber,E.ItemType,G.ProjectSpendName,
                                                    F.BinNumber as BinName FROM PullDetails A 
                                                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                    LEFT OUTER  JOIN PullMaster PM on PM.GUID=A.PULLGUID
                                                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID
                                                    LEFT OUTER JOIN BinMaster F on A.BinID = F.ID 
                                                    LEFT OUTER JOIN ProjectMaster G on A.ProjectSpendGUID = G.GUID 
                                                    WHERE IsNULL(A.IsDeleted,0) =0 AND IsNULL(A.IsArchived,0) =0  AND A.ItemGuid = '" + ItemGuid.ToString() + "'")
                                      select new PullDetailsDTO
                                      {
                                          ID = u.ID,
                                          PULLGUID = u.PULLGUID,
                                          ItemGUID = u.ItemGUID,
                                          ProjectSpendGUID = u.ProjectSpendGUID,
                                          ItemCost = u.ItemCost.GetValueOrDefault(0),// Math.Round(u.ItemCost.GetValueOrDefault(0), objeTurnsRegionInfo.CurrencyDecimalDigits),
                                          CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                          ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),//Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                          PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),//Math.Round(u.PoolQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                          SerialNumber = u.SerialNumber,
                                          LotNumber = u.LotNumber,
                                          Expiration = u.Expiration,
                                          Received = u.Received,
                                          BinID = u.BinID,
                                          Created = u.Created,
                                          Updated = u.Updated,
                                          CreatedBy = u.CreatedBy,
                                          LastUpdatedBy = u.LastUpdatedBy,
                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                          CompanyID = u.CompanyID,
                                          Room = u.Room,
                                          PullCredit = u.PullCredit,
                                          CreatedByName = u.CreatedByName,
                                          UpdatedByName = u.UpdatedByName,
                                          RoomName = u.RoomName,
                                          BinName = u.BinName,
                                          ItemNumber = u.ItemNumber,
                                          ItemType = u.ItemType,
                                          ProjectSpendName = u.ProjectSpendName,
                                          ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                                          GUID = u.GUID,
                                          MaterialStagingPullDetailGUID = u.MaterialStagingPullDetailGUID,
                                          ReceivedOn = u.ReceivedOn,
                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                          AddedFrom = u.AddedFrom,
                                          EditedFrom = u.EditedFrom,
                                          CreditConsignedQuantity = u.CreditConsignedQuantity,
                                          CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity,
                                          ItemPrice = u.ItemPrice.GetValueOrDefault(0)
                                      }).AsParallel().ToList();
                return PullDetailsDTO;
            }
        }

        /// <summary>
        /// Get Particullar Record from the PullDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public PullDetailsDTO GetRecord(Int64 id, Guid PullGUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID, PullGUID).SingleOrDefault(t => t.ID == id);
        }

        /// <summary>
        /// Get Particullar Record from the PullDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public PullDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<PullDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM PullDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new PullDetailsDTO
                        {
                            ID = u.ID,
                            PULLGUID = u.PULLGUID,
                            ItemGUID = u.ItemGUID,
                            ProjectSpendGUID = u.ProjectSpendGUID,
                            ItemCost = u.ItemCost.GetValueOrDefault(0),//Math.Round(u.ItemCost.GetValueOrDefault(0), objeTurnsRegionInfo.CurrencyDecimalDigits),
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),//Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),// Math.Round(u.PoolQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            SerialNumber = u.SerialNumber,
                            LotNumber = u.LotNumber,
                            Expiration = u.Expiration,
                            Received = u.Received,
                            BinID = u.BinID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            PullCredit = u.PullCredit,
                            ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                            GUID = u.GUID,
                            MaterialStagingPullDetailGUID = u.MaterialStagingPullDetailGUID,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            CreditConsignedQuantity = u.CreditConsignedQuantity,
                            CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity,
                            ItemPrice = u.ItemPrice.GetValueOrDefault(0)
                        }).SingleOrDefault();
            }
        }

        #region [for service]

        //public Int64 InsertForService(PullDetailsDTO objDTO, string ConnectionString)
        //{
        //    objDTO.Updated = DateTimeUtility.DateTimeNow;
        //    objDTO.Created = DateTimeUtility.DateTimeNow;

        //    objDTO.IsDeleted = false;
        //    objDTO.IsArchived = false;

        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        PullDetail obj = new PullDetail();
        //        obj.ID = 0;
        //        obj.PULLGUID = objDTO.PULLGUID;
        //        obj.ItemGUID = objDTO.ItemGUID;
        //        obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
        //        obj.ItemCost = objDTO.ItemCost;
        //        obj.ItemPrice = objDTO.ItemPrice;
        //        obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
        //        obj.ConsignedQuantity = objDTO.ConsignedQuantity;
        //        obj.PoolQuantity = objDTO.PoolQuantity;
        //        obj.SerialNumber = objDTO.SerialNumber;
        //        obj.LotNumber = objDTO.LotNumber;
        //        obj.Expiration = objDTO.Expiration;
        //        obj.Received = objDTO.Received;
        //        obj.BinID = objDTO.BinID;
        //        obj.Created = DateTimeUtility.DateTimeNow;
        //        obj.Updated = DateTimeUtility.DateTimeNow;
        //        obj.CreatedBy = objDTO.CreatedBy;
        //        obj.LastUpdatedBy = objDTO.LastUpdatedBy;
        //        obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
        //        obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
        //        obj.CompanyID = objDTO.CompanyID;
        //        obj.Room = objDTO.Room;
        //        obj.PullCredit = objDTO.PullCredit;
        //        obj.GUID = Guid.NewGuid();
        //        obj.ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID;
        //        obj.MaterialStagingPullDetailGUID = objDTO.MaterialStagingPullDetailGUID;

        //        obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
        //        obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
        //        obj.AddedFrom = objDTO.AddedFrom = "Web";
        //        obj.EditedFrom = objDTO.EditedFrom = "Web";
        //        obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
        //        obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;

        //        context.PullDetails.AddObject(obj);
        //        context.SaveChanges();
        //        objDTO.ID = obj.ID;
        //        objDTO.GUID = obj.GUID;

        //        if (objDTO.ID > 0)
        //        {
        //            //Get Cached-Media
        //            IEnumerable<PullDetailsDTO> ObjCache = CacheHelper<IEnumerable<PullDetailsDTO>>.GetCacheItem("Cached_PullDetails_" + objDTO.CompanyID.ToString());
        //            if (ObjCache != null)
        //            {
        //                List<PullDetailsDTO> tempC = new List<PullDetailsDTO>();
        //                tempC.Add(objDTO);

        //                IEnumerable<PullDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<PullDetailsDTO>>.AppendToCacheItem("Cached_PullDetails_" + objDTO.CompanyID.ToString(), NewCache);
        //            }
        //        }

        //        return obj.ID;
        //    }

        //}

        #endregion

        #region MS Credit

        public IEnumerable<PullDetailsDTO> GetPullDetailsByItemGuidMSCredit(Guid ItemGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var PullDetailsDTO = (from u in context.ExecuteStoreQuery<PullDetailsDTO>(
                                               @"SELECT PM.PullCredit,A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, 
                                                    D.RoomName,E.ItemNumber,E.ItemType,G.ProjectSpendName,
                                                    F.BinNumber as BinName,MSPD.MaterialStagingGUID,
													MSPD.MaterialStagingdtlGUID
                                                    FROM PullDetails A 
                                                    LEFT OUTER JOIN MaterialStagingPullDetail MSPD on A.MaterialStagingPullDetailGUID = MSPD.GUID
                                                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                    LEFT OUTER  JOIN PullMaster PM on PM.GUID=A.PULLGUID
                                                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID
                                                    LEFT OUTER JOIN BinMaster F on A.BinID = F.ID 
                                                    LEFT OUTER JOIN ProjectMaster G on A.ProjectSpendGUID = G.GUID 
                                                    WHERE IsNULL(A.IsDeleted,0) =0 AND IsNULL(A.IsArchived,0) =0  AND A.ItemGuid = '" + ItemGuid.ToString() + "'")
                                      select new PullDetailsDTO
                                      {
                                          ID = u.ID,
                                          PULLGUID = u.PULLGUID,
                                          ItemGUID = u.ItemGUID,
                                          ProjectSpendGUID = u.ProjectSpendGUID ?? Guid.Empty,
                                          ItemCost = u.ItemCost.GetValueOrDefault(0),// Math.Round(u.ItemCost.GetValueOrDefault(0), objeTurnsRegionInfo.CurrencyDecimalDigits),
                                          CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                          ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),//Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                          PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),//Math.Round(u.PoolQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                                          SerialNumber = u.SerialNumber,
                                          LotNumber = u.LotNumber,
                                          Expiration = u.Expiration,
                                          Received = u.Received,
                                          BinID = u.BinID,
                                          Created = u.Created,
                                          Updated = u.Updated,
                                          CreatedBy = u.CreatedBy,
                                          LastUpdatedBy = u.LastUpdatedBy,
                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                          CompanyID = u.CompanyID,
                                          Room = u.Room,
                                          PullCredit = u.PullCredit,
                                          CreatedByName = u.CreatedByName,
                                          UpdatedByName = u.UpdatedByName,
                                          RoomName = u.RoomName,
                                          BinName = u.BinName,
                                          ItemNumber = u.ItemNumber,
                                          ItemType = u.ItemType,
                                          ProjectSpendName = u.ProjectSpendName,
                                          ItemLocationDetailGUID = u.ItemLocationDetailGUID ?? Guid.Empty,
                                          GUID = u.GUID,
                                          MaterialStagingPullDetailGUID = u.MaterialStagingPullDetailGUID,
                                          ReceivedOn = u.ReceivedOn,
                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                          AddedFrom = u.AddedFrom,
                                          EditedFrom = u.EditedFrom,
                                          CreditConsignedQuantity = u.CreditConsignedQuantity,
                                          CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity,
                                          ItemPrice = u.ItemPrice.GetValueOrDefault(0),
                                          MaterialStagingGUID = u.MaterialStagingGUID ?? Guid.Empty,
                                          MaterialStagingdtlGUID = u.MaterialStagingdtlGUID ?? Guid.Empty
                                      }).AsParallel().ToList();
                return PullDetailsDTO;
            }
        }

        #endregion

        public PullCreditHistoryDTO GetPullCreditHistoryData(Int64 RoomID, Int64 CompanyID, Guid PullGUID, Guid PullDetailGUID, Guid ItemGUID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<PullCreditHistoryDTO>(
                                                @"SELECT 
                                                    PM.PullCredit
                                                    , A.*                                                                                                         
                                                    FROM CreditHistory A 
                                                    WHERE A.CompanyID = " + CompanyID.ToString() +
                                                          " AND A.PULLGUID = '" + PullGUID.ToString() +
                                                          " AND A.PullDetailGUID = '" + PullDetailGUID.ToString() +
                                                          "' AND A.Room = " + RoomID.ToString() +
                                                          "' AND A.ItemGuid = '" + ItemGUID.ToString() + "'"
                                                          )
                        select new PullCreditHistoryDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            PullDetailGuid = u.PullDetailGuid,
                            PULLGUID = u.PULLGUID,
                            ItemGUID = u.ItemGUID,
                            CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity.GetValueOrDefault(0),
                            CreditConsignedQuantity = u.CreditConsignedQuantity.GetValueOrDefault(0),
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            WhatWhereAction = u.WhatWhereAction
                        }).AsParallel().FirstOrDefault();
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
                        strQuery += "UPDATE PullDetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(), EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<PullDetailsDTO> ObjCache = CacheHelper<IEnumerable<PullDetailsDTO>>.GetCacheItem("Cached_PullDetails_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<PullDetailsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<PullDetailsDTO>>.AppendToCacheItem("Cached_PullDetails_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public IEnumerable<PullDetailsDTO> GetAllRecords(Guid PullGUID, Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId, PullGUID).OrderBy("ID DESC");
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PullDetailsDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, Guid PullGUID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<PullDetailsDTO>(
                                                @"SELECT PM.PullCredit,A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, 
                                                    D.RoomName,E.ItemNumber,E.ItemType,G.ProjectSpendName,
                                                    F.BinNumber as BinName FROM PullDetails A 
                                                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                    LEFT OUTER  JOIN PullMaster PM on PM.GUID=A.PULLGUID
                                                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID
                                                    LEFT OUTER JOIN BinMaster F on A.BinID = F.ID 
                                                    LEFT OUTER JOIN ProjectMaster G on A.ProjectSpendGUID = G.GUID 
                                                    WHERE A.CompanyID = " + CompanyID.ToString() + " AND PULLGUID = '" + PullGUID.ToString() + "' AND A.Room = " + RoomID.ToString())
                        select new PullDetailsDTO
                        {
                            ID = u.ID,
                            PULLGUID = u.PULLGUID,
                            ItemGUID = u.ItemGUID,
                            ProjectSpendGUID = u.ProjectSpendGUID,
                            ItemCost = u.ItemCost.GetValueOrDefault(0),// Math.Round(u.ItemCost.GetValueOrDefault(0), objeTurnsRegionInfo.CurrencyDecimalDigits),
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),//Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),//Math.Round(u.PoolQuantity.GetValueOrDefault(0), objeTurnsRegionInfo.NumberDecimalDigits),
                            SerialNumber = u.SerialNumber,
                            LotNumber = u.LotNumber,
                            Expiration = u.Expiration,
                            Received = u.Received,
                            BinID = u.BinID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            PullCredit = u.PullCredit,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            BinName = u.BinName,
                            ItemNumber = u.ItemNumber,
                            ItemType = u.ItemType,
                            ProjectSpendName = u.ProjectSpendName,
                            ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                            GUID = u.GUID,
                            MaterialStagingPullDetailGUID = u.MaterialStagingPullDetailGUID,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            CreditConsignedQuantity = u.CreditConsignedQuantity,
                            CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity,
                            ItemPrice = u.ItemPrice.GetValueOrDefault(0),
                        }).AsParallel().ToList();
            }
        }
    }
}
