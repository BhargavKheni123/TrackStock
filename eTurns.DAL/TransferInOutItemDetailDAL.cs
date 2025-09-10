using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class TransferInOutItemDetailDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public TransferInOutItemDetailDAL(base.DataBaseName)
        //{

        //}

        public TransferInOutItemDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public TransferInOutItemDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<TransferInOutItemDetailDTO> GetTrfInOutItemDetailByInOutQtyDetailGuidPlain(Guid InOutQtyDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferInOutQtyDetailGUID", InOutQtyDetailGUID)
                                                };
                return context.Database.SqlQuery<TransferInOutItemDetailDTO>("exec [GetTrfInOutItemDetailByInOutQtyDetailGuidPlain] @TransferInOutQtyDetailGUID ", params1).ToList();
            }
        }

        /// <summary>
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<TransferInOutItemDetailDTO> GetTrfInOutItemDetailByInOutQtyDetailGuid(Guid InOutQtyDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferInOutQtyDetailGUID", InOutQtyDetailGUID)                                                   
                                                };
                return context.Database.SqlQuery<TransferInOutItemDetailDTO>("exec [GetTrfInOutItemDetailByInOutQtyDetailGuid] @TransferInOutQtyDetailGUID ", params1).ToList();
            }
        }

        /// <summary>
        /// This method is used to get records of the TransferInOutItemDetail tables based on InOutQtyDetailGUID and lot/serial numbers
        /// </summary>
        /// <param name="InOutQtyDetailGUID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="serialNumbers"></param>
        /// <param name="lotNumbers"></param>
        /// <returns></returns>
        public List<TransferInOutItemDetailDTO> GetTrfInOutItemDetailByInOutQtyDetailGuidAndLotSr(Guid InOutQtyDetailGUID, long CompanyID, List<string> serialNumbers, List<string> lotNumbers)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferInOutQtyDetailGUID", InOutQtyDetailGUID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@SerialNumbers", string.Join(",", serialNumbers)),
                                                   new SqlParameter("@LotNumbers", string.Join(",", lotNumbers)),
                                                };
                return context.Database.SqlQuery<TransferInOutItemDetailDTO>("exec [GetTrfInOutItemDetailByInOutQtyDetailGuidAndLotSr] @TransferInOutQtyDetailGUID,@CompanyID,@SerialNumbers,@LotNumbers ", params1).ToList();
            }
        }


        /// <summary>
        /// Insert Record in the DataBase KitMoveInOutDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(TransferInOutItemDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TransferInOutItemDetail obj = new TransferInOutItemDetail()
                {
                    ID = 0,
                    ItemGUID = objDTO.ItemGUID,
                    BinID = objDTO.BinID,
                    CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity,
                    ConsignedQuantity = objDTO.ConsignedQuantity,
                    Created = DateTimeUtility.DateTimeNow,
                    Updated = DateTimeUtility.DateTimeNow,
                    CreatedBy = objDTO.CreatedBy,
                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    Room = objDTO.Room,
                    CompanyID = objDTO.CompanyID,
                    IsDeleted = false,
                    IsArchived = false,
                    GUID = Guid.NewGuid(),
                    TransferDetailGUID = objDTO.TransferDetailGUID,
                    Cost = objDTO.Cost,
                    ExpirationDate = objDTO.ExpirationDate,
                    LotNumber = objDTO.LotNumber,
                    ReceivedDate = objDTO.ReceivedDate,
                    SerialNumber = objDTO.SerialNumber,
                    TransferedDate = objDTO.TransferedDate,
                    TransferInOutQtyDetailGUID = objDTO.TransferInOutQtyDetailGUID,
                    IsTransfered = objDTO.IsTransfered,
                    ReceivedOn = DateTimeUtility.DateTimeNow,
                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    AddedFrom = "Web",
                    EditedFrom = "Web",
                    ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID,
                    ProcessedCustomerOwnedQuantity = objDTO.ProcessedCustomerOwnedQuantity,
                    ProcessedConsignedQuantity = objDTO.ProcessedConsignedQuantity,
                };
                context.TransferInOutItemDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                return obj.ID;
            }
        }

        public bool EditLocation(TransferInOutItemDetailDTO objDTO)
        {
            TransferDetailDAL transferDetailDAL = new TransferDetailDAL(base.DataBaseName);
            var trfDetail = transferDetailDAL.GetTransferDetailsByGuidPlain(objDTO.TransferDetailGUID ?? Guid.Empty); 

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TransferInOutItemDetail obj = context.TransferInOutItemDetails.Single(t => t.GUID == objDTO.GUID);
                
                if (trfDetail != null && (trfDetail.ApprovedQuantity ?? 0) > 0 && ((trfDetail.ApprovedQuantity ?? 0) - (trfDetail.ReceivedQuantity ?? 0)) <= 0)
                    obj.IsTransfered = true;
                else if (objDTO.IsTransfered)
                    obj.IsTransfered = true;

                obj.BinID = objDTO.BinID;
                
                if (objDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty && !obj.ItemLocationDetailGUID.HasValue)
                {
                    obj.ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID;
                }

                obj.ReceivedDate = objDTO.ReceivedDate;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;
                context.SaveChanges();

                return true;
            }
        }

        public bool EditTransInOutItemDetail(TransferInOutItemDetailDTO objDTO)
        {
            TransferDetailDAL transferDetailDAL = new TransferDetailDAL(base.DataBaseName);
            var trfDetail = transferDetailDAL.GetTransferDetailsByGuidPlain(objDTO.TransferDetailGUID ?? Guid.Empty);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TransferInOutItemDetail obj = context.TransferInOutItemDetails.Single(t => t.GUID == objDTO.GUID);

                if (trfDetail != null && (trfDetail.ApprovedQuantity ?? 0) > 0 && ((trfDetail.ApprovedQuantity ?? 0) - (trfDetail.ReceivedQuantity ?? 0)) <= 0)
                    obj.IsTransfered = true;
                else if (objDTO.IsTransfered)
                    obj.IsTransfered = true;

                obj.ProcessedConsignedQuantity = objDTO.ProcessedConsignedQuantity;
                obj.ProcessedCustomerOwnedQuantity = objDTO.ProcessedCustomerOwnedQuantity;
                obj.BinID = objDTO.BinID;

                if (objDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    obj.ItemLocationDetailGUID = objDTO.ItemLocationDetailGUID;
                }

                obj.ReceivedDate = objDTO.ReceivedDate;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;
                context.SaveChanges();

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
        public bool UpdateBinIdForTransInOutItemDetail(Guid TransferDetailGuid, Guid ItemGuid,long BinId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@TransferDetailGuid", TransferDetailGuid),
                                                    new SqlParameter("@ItemGuid", ItemGuid),
                                                    new SqlParameter("@BinId", BinId)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateBinIdForTransInOutItemDetail] @TransferDetailGuid,@ItemGuid,@BinId", params1);
            }
            return true;
        }

        public IEnumerable<TransferInOutItemDetailDTO> GetTransferInOutItemDetailsByTrfQtyDetailGuidNormal(Guid TransferInOutQtyDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TransferInOutQtyDetailGUID", TransferInOutQtyDetailGUID) };
                return context.Database.SqlQuery<TransferInOutItemDetailDTO>("EXEC [GetTransferInOutItemDetailsByTrfQtyDetailGuidNormal] @TransferInOutQtyDetailGUID", params1).ToList();
            }
        }
    }
}
