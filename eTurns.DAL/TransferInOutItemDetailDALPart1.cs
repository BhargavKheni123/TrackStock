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
    public partial class TransferInOutItemDetailDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Get Paged Records from the KitMoveInOutDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferInOutItemDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<TransferInOutItemDetailDTO> ObjCache = GetAllRecords(RoomID, CompanyID);
            IEnumerable<TransferInOutItemDetailDTO> ObjGlobalCache = ObjCache;
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

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferInOutItemDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<TransferInOutItemDetailDTO> obj = (from u in context.ExecuteStoreQuery<TransferInOutItemDetailDTO>(@"SELECT A.* FROM TransferInOutItemDetail A  WHERE A.CompanyID = " + CompanyID)
                                                               select new TransferInOutItemDetailDTO
                                                               {
                                                                   ID = u.ID,

                                                                   ItemGUID = u.ItemGUID,
                                                                   BinID = u.BinID,
                                                                   Created = u.Created,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   Room = u.Room,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   CompanyID = u.CompanyID,
                                                                   GUID = u.GUID,
                                                                   ConsignedQuantity = u.ConsignedQuantity,
                                                                   CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                                   TransferDetailGUID = u.TransferDetailGUID,
                                                                   Updated = u.Updated,
                                                                   Cost = u.Cost,
                                                                   ExpirationDate = u.ExpirationDate,
                                                                   LotNumber = u.LotNumber,
                                                                   ReceivedDate = u.ReceivedDate,
                                                                   SerialNumber = u.SerialNumber,
                                                                   TransferedDate = u.TransferedDate,
                                                                   TransferInOutQtyDetailGUID = u.TransferInOutQtyDetailGUID,
                                                                   IsTransfered = u.IsTransfered,
                                                                   ReceivedOn = u.ReceivedOn,
                                                                   ReceivedOnWeb = u.ReceivedOnWeb,
                                                                   AddedFrom = u.AddedFrom,
                                                                   EditedFrom = u.EditedFrom,
                                                                   ProcessedCustomerOwnedQuantity = u.ProcessedCustomerOwnedQuantity,
                                                                   ProcessedConsignedQuantity = u.ProcessedConsignedQuantity,
                                                               }).AsParallel().ToList();
                return obj;
            }

        }

        /// <summary>
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferInOutItemDetailDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TransferInOutItemDetailDTO obj = (from u in context.ExecuteStoreQuery<TransferInOutItemDetailDTO>(@"SELECT A.* FROM TransferInOutItemDetail A  WHERE A.GUID = '" + GUID.ToString() + "' AND A.CompanyID = " + CompanyID)
                                                  select new TransferInOutItemDetailDTO
                                                  {
                                                      ID = u.ID,

                                                      ItemGUID = u.ItemGUID,
                                                      BinID = u.BinID,
                                                      Created = u.Created,
                                                      CreatedBy = u.CreatedBy,
                                                      LastUpdatedBy = u.LastUpdatedBy,
                                                      Room = u.Room,
                                                      IsDeleted = u.IsDeleted,
                                                      IsArchived = u.IsArchived,
                                                      CompanyID = u.CompanyID,
                                                      GUID = u.GUID,
                                                      ConsignedQuantity = u.ConsignedQuantity,
                                                      CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                      TransferDetailGUID = u.TransferDetailGUID,
                                                      Updated = u.Updated,
                                                      Cost = u.Cost,
                                                      ExpirationDate = u.ExpirationDate,
                                                      LotNumber = u.LotNumber,
                                                      ReceivedDate = u.ReceivedDate,
                                                      SerialNumber = u.SerialNumber,
                                                      TransferedDate = u.TransferedDate,
                                                      TransferInOutQtyDetailGUID = u.TransferInOutQtyDetailGUID,
                                                      IsTransfered = u.IsTransfered,
                                                      ReceivedOn = u.ReceivedOn,
                                                      ReceivedOnWeb = u.ReceivedOnWeb,
                                                      AddedFrom = u.AddedFrom,
                                                      EditedFrom = u.EditedFrom,
                                                      ProcessedCustomerOwnedQuantity = u.ProcessedCustomerOwnedQuantity,
                                                      ProcessedConsignedQuantity = u.ProcessedConsignedQuantity,
                                                  }).SingleOrDefault();
                return obj;
            }
        }

        /// <summary>
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public IEnumerable<TransferInOutItemDetailDTO> GetRecordByTransferDetail(Guid DetailGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<TransferInOutItemDetailDTO> obj = (from u in context.ExecuteStoreQuery<TransferInOutItemDetailDTO>(@"SELECT A.* FROM TransferInOutItemDetail A  WHERE A.TransferDetailGUID = '" + DetailGUID.ToString() + "' AND A.CompanyID = " + CompanyID)
                                                               select new TransferInOutItemDetailDTO
                                                               {
                                                                   ID = u.ID,

                                                                   ItemGUID = u.ItemGUID,
                                                                   BinID = u.BinID,
                                                                   Created = u.Created,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   Room = u.Room,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   CompanyID = u.CompanyID,
                                                                   GUID = u.GUID,
                                                                   ConsignedQuantity = u.ConsignedQuantity,
                                                                   CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                                   TransferDetailGUID = u.TransferDetailGUID,
                                                                   Updated = u.Updated,
                                                                   Cost = u.Cost,
                                                                   ExpirationDate = u.ExpirationDate,
                                                                   LotNumber = u.LotNumber,
                                                                   ReceivedDate = u.ReceivedDate,
                                                                   SerialNumber = u.SerialNumber,
                                                                   TransferedDate = u.TransferedDate,
                                                                   TransferInOutQtyDetailGUID = u.TransferInOutQtyDetailGUID,
                                                                   IsTransfered = u.IsTransfered,
                                                                   ReceivedOn = u.ReceivedOn,
                                                                   ReceivedOnWeb = u.ReceivedOnWeb,
                                                                   AddedFrom = u.AddedFrom,
                                                                   EditedFrom = u.EditedFrom,
                                                                   ProcessedCustomerOwnedQuantity = u.ProcessedCustomerOwnedQuantity,
                                                                   ProcessedConsignedQuantity = u.ProcessedConsignedQuantity,
                                                               });
                return obj;
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
                        strQuery += "UPDATE TransferInOutItemDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                return true;
            }
        }

        /// <summary>
        /// This method is used to update binId for the TransferInOutItemDetails (for receive transfer)
        /// </summary>
        /// <param name="transferDetailGuid"></param>
        /// <param name="itemGuid"></param>
        /// <param name="binId"></param>
        /// <returns></returns>
        public bool UpdateBinIdForTransInOutItemDetail(Guid transferDetailGuid, Guid itemGuid, long binId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var transgerInOutItems = context.TransferInOutItemDetails.Where(t => t.TransferDetailGUID == transferDetailGuid && t.ItemGUID == itemGuid).ToList();
                foreach (var item in transgerInOutItems)
                {
                    item.BinID = binId;
                }
                context.SaveChanges();
                return true;
            }

        }
    }
}
