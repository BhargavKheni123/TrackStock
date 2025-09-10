using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ReceivedOrderTransferDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ReceivedOrderTransferDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ReceivedOrderTransferDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion


        #region [Class Methods]

        public ReceivedOrderTransferDetailDTO GetROTDByGuidPlain(Guid ROTDGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByGuidPlain] @ROTDGuid,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ROTDGuid", ROTDGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).FirstOrDefault();

            }
        }
        public ReceivedOrderTransferDetailDTO GetROTDByGuidNormal(Guid ROTDGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByGuidNormal] @ROTDGuid,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ROTDGuid", ROTDGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public ReceivedOrderTransferDetailDTO GetROTDByGuidFull(Guid ROTDGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByGuidFull] @ROTDGuid,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ROTDGuid", ROTDGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public List<ReceivedOrderTransferDetailDTO> GetROTDByRoomPlain(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByRoomPlain] @RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();

            }
        }

        public List<ReceivedOrderTransferDetailDTO> GetROTDByIDsPlain(string ROTDIDs, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByIDsPlain] @ROTDIDs,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ROTDIDs", ROTDIDs),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();

            }
        }


        public List<ReceivedOrderTransferDetailDTO> GetROTDByOrderGUIDFull(Guid OrderGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByOrderGUIDFull] @OrderGUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderGUID", OrderGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();

            }
        }

        public List<ReceivedOrderTransferDetailDTO> GetROTDByOrderDetailGUIDPlain(Guid OrderDetailGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByOrderDetailGUIDPlain] @OrderDetailGUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderDetailGUID", OrderDetailGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();

            }
        }


        public List<ReceivedOrderTransferDetailDTO> GetROTDByOrderDetailGUIDFull(Guid OrderDetailGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetROTDByOrderDetailGUIDFull] @OrderDetailGUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderDetailGUID", OrderDetailGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();

            }
        }

        public ReceivedOrderTransferDetailDTO GetROTDByILDGUIDFull(Guid ILDGUID, Guid OrderDetailGUID, long RoomID, long CompanyID)
        {
            var ROTDList = GetROTDByOrderDetailGUIDPlain(OrderDetailGUID, RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
            return ROTDList.Where(x => x.ItemLocationDetailGUID == ILDGUID).FirstOrDefault();
        }

        public Int64 Insert(ItemLocationDetailsDTO objDTO)
        {

            ReceivedOrderTransferDetailDTO objNewDTO = new ReceivedOrderTransferDetailDTO()
            {
                Action = objDTO.Action,
                BinID = objDTO.BinID,
                BinNumber = objDTO.BinNumber,
                CompanyID = objDTO.CompanyID,
                ConsignedQuantity = objDTO.ConsignedQuantity,
                Cost = objDTO.Cost,
                Created = DateTime.Now,
                CreatedBy = objDTO.CreatedBy,
                CreatedByName = objDTO.CreatedByName,
                CriticalQuantity = objDTO.CriticalQuantity,
                CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity,
                DateCodeTracking = objDTO.DateCodeTracking,
                eVMISensorID = objDTO.eVMISensorID,
                eVMISensorPort = objDTO.eVMISensorPort,
                Expiration = objDTO.Expiration,
                ExpirationDate = objDTO.ExpirationDate,
                GUID = Guid.NewGuid(),
                HistoryID = 0,
                ID = 0,
                IsArchived = false,
                IsCreditPull = objDTO.IsCreditPull,
                IsDeleted = false,
                ItemGUID = objDTO.ItemGUID,
                ItemLocationDetailGUID = objDTO.GUID,
                ItemNumber = objDTO.ItemNumber,
                ItemType = objDTO.ItemType,
                KitDetailGUID = objDTO.KitDetailGUID,
                LastUpdatedBy = objDTO.LastUpdatedBy,
                LotNumber = (!string.IsNullOrWhiteSpace(objDTO.LotNumber)) ? objDTO.LotNumber.Trim() : string.Empty,
                LotNumberTracking = objDTO.LotNumberTracking,
                MaximumQuantity = objDTO.MaximumQuantity,
                MeasurementID = objDTO.MeasurementID,
                MinimumQuantity = objDTO.MinimumQuantity,
                mode = objDTO.mode,
                OrderDetailGUID = objDTO.OrderDetailGUID,
                Received = objDTO.Received,
                ReceivedDate = objDTO.ReceivedDate,
                Room = objDTO.Room,
                RoomName = objDTO.RoomName,
                SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.SerialNumber)) ? objDTO.SerialNumber.Trim() : string.Empty,
                SerialNumberTracking = objDTO.SerialNumberTracking,
                TransferDetailGUID = objDTO.TransferDetailGUID,
                UDF1 = objDTO.UDF1,
                UDF2 = objDTO.UDF2,
                UDF3 = objDTO.UDF3,
                UDF4 = objDTO.UDF4,
                UDF5 = objDTO.UDF5,
                UDF6 = objDTO.UDF6,
                UDF7 = objDTO.UDF7,
                UDF8 = objDTO.UDF8,
                UDF9 = objDTO.UDF9,
                UDF10 = objDTO.UDF10,
                Updated = objDTO.Updated,
                UpdatedByName = objDTO.UpdatedByName,
                PackSlipNumber = objDTO.PackSlipNumber,
                AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom),
                EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom),
                ReceivedOn = objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                ReceivedOnWeb = objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
            };

            objNewDTO.Updated = DateTimeUtility.DateTimeNow;
            objNewDTO.Created = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedOrderTransferDetail obj = new ReceivedOrderTransferDetail();
                obj.ID = 0;

                obj.BinID = objNewDTO.BinID;
                obj.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objNewDTO.ConsignedQuantity;
                obj.MeasurementID = objNewDTO.MeasurementID;
                obj.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
                obj.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
                obj.ExpirationDate = objNewDTO.ExpirationDate;
                obj.Cost = objNewDTO.Cost;
                obj.eVMISensorPort = objNewDTO.eVMISensorPort;
                obj.eVMISensorID = objNewDTO.eVMISensorID;
                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.ItemGUID = objNewDTO.ItemGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.OrderDetailGUID = objNewDTO.OrderDetailGUID;
                obj.KitDetailGUID = objNewDTO.KitDetailGUID;
                obj.TransferDetailGUID = objNewDTO.TransferDetailGUID;
                obj.PackSlipNumber = objNewDTO.PackSlipNumber;

                if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                {
                    obj.Expiration = objNewDTO.Expiration.Replace("-", "/");
                }

                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }
                obj.ItemLocationDetailGUID = objNewDTO.ItemLocationDetailGUID;
                obj.ReceivedDate = objNewDTO.ReceivedDate;
                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = objNewDTO.ReceivedOn == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(objNewDTO.ReceivedOn);
                obj.ReceivedOnWeb = objNewDTO.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(objNewDTO.ReceivedOnWeb);


                context.ReceivedOrderTransferDetails.Add(obj);
                context.SaveChanges();
                objNewDTO.ID = obj.ID;
                objNewDTO.GUID = obj.GUID;

                if (objNewDTO.ID > 0 && objNewDTO.OrderDetailGUID.HasValue)
                {
                    PreReceivOrderDetailDAL preRecDAL = new PreReceivOrderDetailDAL(base.DataBaseName);
                    preRecDAL.UpdateIsReceived(objNewDTO);

                }

                return objDTO.ID;
            }
        }



        public void Edit(List<ReceivedOrderTransferDetailDTO> ROTDs, long SessionUserId, long EnterpriseId)
        {

            if (ROTDs == null || ROTDs.Count <= 0)
                return;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ReceivedOrderTransferDetail objROTD = null;
                ItemLocationDetail objILD = null;
                ItemLocationDetail objILDNew = null;
                MaterialStagingPullDetail objMSPullDetail = null;
                OrderDetail objOD = null;

                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                OrderDetailsDAL objOrdDtlDAL = new OrderDetailsDAL(base.DataBaseName);
                double PrevReceivedCustOwnQty = 0;
                double PrevReceivedConsQty = 0;
                List<ItemLocationQTYDTO> objListQty = new List<ItemLocationQTYDTO>();

                foreach (var objNewDTO in ROTDs)
                {
                    try
                    {
                        ItemMasterDTO objItemMasterDTO = objItemDAL.GetRecordByItemGUID(Guid.Parse(objNewDTO.ItemGUID.ToString()), Convert.ToInt64(objNewDTO.Room), Convert.ToInt64(objNewDTO.CompanyID));
                        OrderDetail od = context.OrderDetails.FirstOrDefault<OrderDetail>(x => x.GUID == (objNewDTO.OrderDetailGUID ?? Guid.Empty));
                        OrderMaster om = context.OrderMasters.FirstOrDefault(x => x.GUID == (od.OrderGUID));

                        //Step 1: SET ReceivedOrderTransferDetail Table record
                        objROTD = context.ReceivedOrderTransferDetails.FirstOrDefault(x => x.GUID == objNewDTO.GUID);
                        PrevReceivedCustOwnQty = objROTD.CustomerOwnedQuantity.GetValueOrDefault(0);
                        PrevReceivedConsQty = objROTD.ConsignedQuantity.GetValueOrDefault(0);
                        objROTD = SetRecieveOrderTransferDetail(objROTD, objNewDTO);
                        // Set OrderUOM QTY
                        if (objItemMasterDTO.OrderUOMValue == null || objItemMasterDTO.OrderUOMValue <= 0)
                            objItemMasterDTO.OrderUOMValue = 1;

                        if (objItemMasterDTO.SerialNumberTracking == false && objItemMasterDTO.IsAllowOrderCostuom && om.OrderType == (int)OrderType.Order)
                        {
                            if (objROTD.ConsignedQuantity != null && objROTD.ConsignedQuantity >= 0)
                                objROTD.ConsignedQuantity = objROTD.ConsignedQuantity * objItemMasterDTO.OrderUOMValue;

                            if (objROTD.CustomerOwnedQuantity != null && objROTD.CustomerOwnedQuantity >= 0)
                                objROTD.CustomerOwnedQuantity = objROTD.CustomerOwnedQuantity * objItemMasterDTO.OrderUOMValue;
                        }


                        //Step 2: SET Respected Location Detail Record. UpdateReceivedOrderTransferDetail Table record
                        objILD = context.ItemLocationDetails.FirstOrDefault(x => x.GUID == objNewDTO.ItemLocationDetailGUID);
                        //OrderDetail od = context.OrderDetails.FirstOrDefault<OrderDetail>(x => x.GUID == (objNewDTO.OrderDetailGUID ?? Guid.Empty));
                        //OrderMaster om = context.OrderMasters.FirstOrDefault(x => x.GUID == (od.OrderGUID ?? Guid.Empty));
                        BinMaster bin = null;
                        if (objILD != null)
                        {


                            ItemLocationDetailsDAL objILDDAL = new ItemLocationDetailsDAL(base.DataBaseName);

                            bin = context.BinMasters.FirstOrDefault(x => x.ID == objILD.BinID);
                            bool IsBinDelete = false;
                            if (bin != null && bin.ID > 0)
                                IsBinDelete = bin.IsDeleted;

                            if (objNewDTO.EditedFrom != "Web>>UncloseReturnOrder" && objNewDTO.EditedFrom != "Web>>EditReceipt>>ReturnOrder")
                            {

                                double prevInitialQuantityWeb = objILD.InitialQuantityWeb.GetValueOrDefault(0);
                                double prevCustOwnQty = 0;
                                double prevConsQty = 0;
                                if (objILD.IsConsignedSerialLot)
                                    prevConsQty = (objILD.InitialQuantityWeb.GetValueOrDefault(0) - objILD.ConsignedQuantity.GetValueOrDefault(0)) * (-1);
                                else
                                    prevCustOwnQty = (objILD.InitialQuantityWeb.GetValueOrDefault(0) - objILD.CustomerOwnedQuantity.GetValueOrDefault(0)) * (-1);


                                objILD = objILDDAL.SetItemLocationDetailAsPerNewROTD(objILD, objNewDTO, PrevReceivedCustOwnQty, PrevReceivedConsQty, (OrderType)om.OrderType, IsBinDelete);
                                objILDNew = objILDDAL.setOldValueInNewItemLocationDetail(objILD);

                                if (objItemMasterDTO.SerialNumberTracking == false && objItemMasterDTO.IsAllowOrderCostuom && om.OrderType == (int)OrderType.Order)
                                {
                                    if (objILDNew.ConsignedQuantity != null && objILDNew.ConsignedQuantity >= 0)
                                        objILDNew.ConsignedQuantity = objILDNew.ConsignedQuantity * objItemMasterDTO.OrderUOMValue;

                                    if (objILDNew.CustomerOwnedQuantity != null && objILDNew.CustomerOwnedQuantity >= 0)
                                        objILDNew.CustomerOwnedQuantity = objILDNew.CustomerOwnedQuantity * objItemMasterDTO.OrderUOMValue;

                                    objILDNew.InitialQuantity = (objILDNew.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objILDNew.ConsignedQuantity.GetValueOrDefault(0));

                                    if (objILDNew.InitialQuantityWeb.GetValueOrDefault(0) > 0)
                                        objILDNew.InitialQuantityWeb = (objILDNew.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objILDNew.ConsignedQuantity.GetValueOrDefault(0));
                                    else if (objILDNew.InitialQuantityPDA.GetValueOrDefault(0) > 0)
                                        objILDNew.InitialQuantityPDA = (objILDNew.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objILDNew.ConsignedQuantity.GetValueOrDefault(0));


                                }

                                context.ItemLocationDetails.Add(objILDNew);
                                objROTD.ItemLocationDetailGUID = objILDNew.GUID;
                                objILD.InitialQuantity = 0;
                                objILD.InitialQuantityWeb = 0;
                                objILD.ConsignedQuantity = prevConsQty;
                                objILD.CustomerOwnedQuantity = prevCustOwnQty;
                            }
                            else
                            {
                                objILD = objILDDAL.SetItemLocationDetail(objILD, objNewDTO, PrevReceivedCustOwnQty, PrevReceivedConsQty, (OrderType)om.OrderType, IsBinDelete);

                            }

                        }
                        else
                        {
                            objMSPullDetail = context.MaterialStagingPullDetails.FirstOrDefault(x => x.GUID == objNewDTO.ItemLocationDetailGUID);
                            if (objMSPullDetail != null)
                            {
                                MaterialStagingPullDetailDAL objMSPDDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                                bin = context.BinMasters.FirstOrDefault(x => x.ID == objMSPullDetail.BinID);
                                bool IsBinDelete = false;
                                if (bin != null && bin.ID > 0)
                                    IsBinDelete = bin.IsDeleted;

                                objMSPullDetail = objMSPDDAL.SetMSPullDetail(objMSPullDetail, objNewDTO, PrevReceivedCustOwnQty, PrevReceivedConsQty, (OrderType)om.OrderType, IsBinDelete);
                            }
                        }

                        context.SaveChanges();


                        if (objILD != null)
                        {
                            IEnumerable<ItemLocationQTY> objItemLocationQtyDelete = context.ItemLocationQTies.Where(x => x.ItemGUID == objNewDTO.ItemGUID);
                            foreach (var item in objItemLocationQtyDelete)
                            {
                                context.ItemLocationQTies.Remove(item);
                            }

                            IEnumerable<ItemLocationQTY> objItemLocationQtyAdd = GetItemLocationQty(objNewDTO, context);
                            foreach (var item in objItemLocationQtyAdd)
                            {
                                context.ItemLocationQTies.Add(item);
                            }
                        }
                        else if (objMSPullDetail != null)
                        {
                            MaterialStagingDetail msDetail = context.MaterialStagingDetails.FirstOrDefault(x => x.GUID == objMSPullDetail.MaterialStagingdtlGUID);

                            if (om.OrderType == 1)
                            {
                                if (objNewDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                {
                                    msDetail.Quantity = (msDetail.Quantity.GetValueOrDefault(0) - PrevReceivedConsQty) + objNewDTO.ConsignedQuantity.GetValueOrDefault(0);
                                }
                                else if (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                {
                                    msDetail.Quantity = (msDetail.Quantity.GetValueOrDefault(0) - PrevReceivedCustOwnQty) + objNewDTO.CustomerOwnedQuantity;
                                }
                            }
                            else if (om.OrderType == 2)
                            {
                                if (objNewDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                {
                                    msDetail.Quantity = (msDetail.Quantity.GetValueOrDefault(0) + PrevReceivedConsQty) - objNewDTO.ConsignedQuantity.GetValueOrDefault(0);
                                }
                                else if (objNewDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                {
                                    msDetail.Quantity = (msDetail.Quantity.GetValueOrDefault(0) + PrevReceivedCustOwnQty) - objNewDTO.CustomerOwnedQuantity;
                                }
                            }

                        }
                        context.SaveChanges();

                        //Step 4: Update Order detail record for Update Received quantity
                        objOD = context.OrderDetails.FirstOrDefault(x => x.GUID == objNewDTO.OrderDetailGUID);
                        objOD.ReceivedQuantity = context.ReceivedOrderTransferDetails.Where(x => x.OrderDetailGUID == objNewDTO.OrderDetailGUID && !(x.IsDeleted ?? false) && !(x.IsArchived ?? false)).Sum(x => ((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)));
                        objOD.LastUpdated = DateTimeUtility.DateTimeNow;
                        objOD.LastUpdatedBy = objNewDTO.LastUpdatedBy;

                        if (om != null && om.OrderType == (int)OrderType.Order)
                        {
                            objOD.ItemCost = objNewDTO.Cost.GetValueOrDefault(0);
                            objOD.ItemSellPrice = objOD.ItemCost.GetValueOrDefault(0) + ((objOD.ItemCost.GetValueOrDefault(0) * objOD.ItemMarkup.GetValueOrDefault(0)) / 100);
                            if (objNewDTO.IsReceivedCostChange.GetValueOrDefault(false))
                            {
                                if (objOD.ItemCostUOMValue == null
                                    || objOD.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                                {
                                    objOD.ItemCostUOMValue = 1;
                                }

                                objOD.OrderLineItemExtendedCost = double.Parse(Convert.ToString((om.OrderStatus <= 2 ? (objOD.RequestedQuantity.GetValueOrDefault(0) * (objOD.ItemCost.GetValueOrDefault(0) / objOD.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (objOD.ApprovedQuantity.GetValueOrDefault(0) * (objOD.ItemCost.GetValueOrDefault(0) / objOD.ItemCostUOMValue.GetValueOrDefault(1))))));

                                objOD.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((om.OrderStatus <= 2 ? (objOD.RequestedQuantity.GetValueOrDefault(0) * (objOD.ItemSellPrice.GetValueOrDefault(0) / objOD.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (objOD.ApprovedQuantity.GetValueOrDefault(0) * (objOD.ItemSellPrice.GetValueOrDefault(0) / objOD.ItemCostUOMValue.GetValueOrDefault(1))))));
                            }
                        }
                        if (objNewDTO.IsOnlyFromUI)
                        {
                            objOD.EditedFrom = "Web";
                            objOD.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }


                        if (objItemMasterDTO != null)
                        {
                            CostUOMMasterDTO costUOM = new CostUOMMasterDTO();
                            if (objItemMasterDTO.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }
                            else
                            {
                                costUOM.CostUOMValue = objItemMasterDTO.CostUOMValue;
                            }

                            if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }

                            if (objOD.ReceivedQuantity != null && objOD.ReceivedQuantity >= 0)
                            {
                                if (objItemMasterDTO.IsAllowOrderCostuom && om.OrderType == (int)OrderType.Order)
                                {
                                    objOD.ReceivedQuantityUOM = objOD.ReceivedQuantity / objItemMasterDTO.OrderUOMValue;
                                    if (objOD.ReceivedQuantityUOM > 0 && objOD.ReceivedQuantityUOM < 1)
                                        objOD.ReceivedQuantityUOM = 0;
                                }
                                else
                                {
                                    objOD.ReceivedQuantityUOM = objOD.ReceivedQuantity;
                                }
                            }
                        }
                        objOD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        context.SaveChanges();

                        //Step 5: Update Order status
                        OrderDetailsDAL ordDetailDAL = new DAL.OrderDetailsDAL(base.DataBaseName);
                        ordDetailDAL.UpdateOrderStatusByReceive(objOD.OrderGUID.GetValueOrDefault(Guid.Empty), objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0), objNewDTO.LastUpdatedBy.GetValueOrDefault(0), objNewDTO.IsOnlyFromUI);


                        //Step 7: Update onorder qtyt
                        string queryToUpdateOnOrderQty = "EXEC [Item_UpdateItemOnOrderQuantity]  @ItemGUID,@Room,@CompanyID,@LastUpdatedBy";
                        var params1 = new SqlParameter[] {
                                        new SqlParameter("@ItemGUID", objNewDTO.ItemGUID.GetValueOrDefault(Guid.Empty)),
                                        new SqlParameter("@Room", objNewDTO.Room.GetValueOrDefault(0)),
                                        new SqlParameter("@CompanyID", objNewDTO.CompanyID.GetValueOrDefault(0)),
                                        new SqlParameter("@LastUpdatedBy", objNewDTO.LastUpdatedBy.GetValueOrDefault(0))
                                        };

                        var rslt = context.Database.SqlQuery<object>(queryToUpdateOnOrderQty, params1).ToList();

                        //Step 8: Update Cost mark up cell price and update cart for edited item.
                        // WI-6215 changes

                        UpdateItemMaster(objNewDTO, context, SessionUserId, objNewDTO.IsReceivedCostChange.GetValueOrDefault(false), EnterpriseId);

                    }
                    finally
                    {
                        objROTD = null;
                        objILD = null;
                        objOD = null;
                        //objItemDAL = null;
                    }
                }
            }
        }

        public Int64 Insert(ReceivedOrderTransferDetailDTO objNewDTO)
        {

            objNewDTO.Updated = DateTimeUtility.DateTimeNow;
            objNewDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedOrderTransferDetail obj = new ReceivedOrderTransferDetail();
                obj.ID = 0;
                obj.BinID = objNewDTO.BinID;
                obj.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objNewDTO.ConsignedQuantity;
                obj.MeasurementID = objNewDTO.MeasurementID;
                obj.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
                obj.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
                obj.ExpirationDate = objNewDTO.ExpirationDate;
                obj.Cost = objNewDTO.Cost;
                obj.eVMISensorPort = objNewDTO.eVMISensorPort;
                obj.eVMISensorID = objNewDTO.eVMISensorID;
                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.ItemGUID = objNewDTO.ItemGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.OrderDetailGUID = objNewDTO.OrderDetailGUID;
                obj.KitDetailGUID = objNewDTO.KitDetailGUID;
                obj.TransferDetailGUID = objNewDTO.TransferDetailGUID;
                obj.PackSlipNumber = objNewDTO.PackSlipNumber;
                if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                {
                    obj.Expiration = objNewDTO.Expiration.Replace("-", "/");
                }

                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }
                obj.ItemLocationDetailGUID = objNewDTO.ItemLocationDetailGUID;
                obj.ReceivedDate = objNewDTO.ReceivedDate;

                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = objNewDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow);
                obj.ReceivedOnWeb = objNewDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow);


                context.ReceivedOrderTransferDetails.Add(obj);
                context.SaveChanges();
                objNewDTO.ID = obj.ID;
                objNewDTO.GUID = obj.GUID;

                if (objNewDTO.ID > 0)
                {
                    PreReceivOrderDetailDAL preRecDAL = new PreReceivOrderDetailDAL(base.DataBaseName);
                    preRecDAL.UpdateIsReceived(objNewDTO);

                }

                return objNewDTO.ID;
            }
        }

        public Int64 InsertMS(MaterialStagingPullDetailDTO objDTO, string RoomDateFormat)
        {
            ReceivedOrderTransferDetailDTO objNewDTO = new ReceivedOrderTransferDetailDTO()
            {
                BinID = objDTO.StagingBinId,
                CompanyID = objDTO.CompanyID,
                ConsignedQuantity = objDTO.ConsignedQuantity,
                Cost = objDTO.ItemCost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = objDTO.CreatedBy,
                CreatedByName = objDTO.CreatedByName,
                CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity,
                DateCodeTracking = objDTO.DateCodeTracking,
                Expiration = objDTO.Expiration,
                GUID = Guid.NewGuid(),
                HistoryID = 0,
                ID = 0,
                IsArchived = false,
                IsDeleted = false,
                ItemGUID = objDTO.ItemGUID,
                ItemLocationDetailGUID = objDTO.GUID,
                ItemNumber = objDTO.ItemNumber,
                LastUpdatedBy = objDTO.LastUpdatedBy,
                LotNumber = (!string.IsNullOrWhiteSpace(objDTO.LotNumber)) ? objDTO.LotNumber.Trim() : string.Empty,
                LotNumberTracking = objDTO.LotNumberTracking,
                OrderDetailGUID = objDTO.OrderDetailGUID,
                Received = objDTO.Received,
                Room = objDTO.Room,
                RoomName = objDTO.RoomName,
                SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.SerialNumber)) ? objDTO.SerialNumber.Trim() : string.Empty,
                SerialNumberTracking = objDTO.SerialNumberTracking,
                Updated = objDTO.Updated,
                UpdatedByName = objDTO.UpdatedByName,
                PackSlipNumber = objDTO.PackSlipNumber,
                UDF1 = objDTO.UDF1,
                UDF2 = objDTO.UDF2,
                UDF3 = objDTO.UDF3,
                UDF4 = objDTO.UDF4,
                UDF5 = objDTO.UDF5,
                AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom),
                EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom),
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                ReceivedDate = objDTO.Received != null ? DateTime.ParseExact(objDTO.Received, RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult) : Convert.ToDateTime(objDTO.Received),
            };

            objNewDTO.Updated = DateTimeUtility.DateTimeNow;
            objNewDTO.Created = DateTimeUtility.DateTimeNow;

            if (objNewDTO.AddedFrom == "Web Transfer Staging" && objNewDTO.EditedFrom == "Web Transfer Staging")
            {
                objNewDTO.TransferDetailGUID = objDTO.TransferDetailGUID;
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedOrderTransferDetail obj = new ReceivedOrderTransferDetail();
                obj.ID = 0;

                obj.BinID = objNewDTO.BinID;
                obj.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objNewDTO.ConsignedQuantity;
                obj.MeasurementID = objNewDTO.MeasurementID;
                obj.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
                obj.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
                obj.ExpirationDate = objNewDTO.ExpirationDate;
                obj.Cost = objNewDTO.Cost;
                obj.eVMISensorPort = objNewDTO.eVMISensorPort;
                obj.eVMISensorID = objNewDTO.eVMISensorID;
                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.ItemGUID = objNewDTO.ItemGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.OrderDetailGUID = objNewDTO.OrderDetailGUID;
                obj.KitDetailGUID = objNewDTO.KitDetailGUID;
                obj.TransferDetailGUID = objNewDTO.TransferDetailGUID;
                obj.PackSlipNumber = objNewDTO.PackSlipNumber;

                if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                {
                    obj.Expiration = objNewDTO.Expiration.Replace("-", "/");
                }

                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }
                obj.ItemLocationDetailGUID = objNewDTO.ItemLocationDetailGUID;
                obj.ReceivedDate = objNewDTO.ReceivedDate;
                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                context.ReceivedOrderTransferDetails.Add(obj);
                context.SaveChanges();
                objNewDTO.ID = obj.ID;
                objNewDTO.GUID = obj.GUID;

                if (objNewDTO.ID > 0)
                {
                    PreReceivOrderDetailDAL preRecDAL = new PreReceivOrderDetailDAL(base.DataBaseName);
                    preRecDAL.UpdateIsReceived(objNewDTO);

                }

                return objDTO.ID;
            }
        }

        public bool Edit(ItemLocationDetailsDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedOrderTransferDetailDTO objNewDTO = GetROTDByILDGUIDFull(objDTO.GUID, objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));

                if (objNewDTO == null)
                    return false;

                ReceivedOrderTransferDetail obj = new ReceivedOrderTransferDetail();
                obj.ID = objNewDTO.ID;
                obj.BinID = objNewDTO.BinID;
                obj.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objNewDTO.ConsignedQuantity;
                obj.MeasurementID = objNewDTO.MeasurementID;
                obj.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
                obj.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
                obj.ExpirationDate = objNewDTO.ExpirationDate;
                obj.Cost = objNewDTO.Cost;
                obj.eVMISensorPort = objNewDTO.eVMISensorPort;
                obj.eVMISensorID = objNewDTO.eVMISensorID;
                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = objNewDTO.GUID;
                obj.ItemGUID = objNewDTO.ItemGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.PackSlipNumber = objNewDTO.PackSlipNumber;
                obj.IsDeleted = objNewDTO.IsDeleted.HasValue ? (bool)objNewDTO.IsDeleted : false;
                obj.IsArchived = objNewDTO.IsArchived.HasValue ? (bool)objNewDTO.IsArchived : false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.OrderDetailGUID = objNewDTO.OrderDetailGUID;
                obj.ItemLocationDetailGUID = objNewDTO.ItemLocationDetailGUID;
                obj.ControlNumber = objNewDTO.ControlNumber;
                if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                {
                    obj.Expiration = objNewDTO.Expiration.Replace("-", "/");
                }

                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }

                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.ReceivedOnWeb = objNewDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow);

                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = objNewDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow);

                obj.KitDetailGUID = objNewDTO.KitDetailGUID;
                obj.TransferDetailGUID = objNewDTO.TransferDetailGUID;
                obj.ReceivedDate = objNewDTO.ReceivedDate;
                obj.IsEDISent = objNewDTO.IsEDISent;
                obj.LastEDIDate = objNewDTO.LastEDIDate;
                context.ReceivedOrderTransferDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();



                return true;
            }
        }

        public bool EditMS(MaterialStagingPullDetailDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedOrderTransferDetailDTO objNewDTO = GetROTDByILDGUIDFull(objDTO.GUID, objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                if (objNewDTO == null)
                    return false;

                ReceivedOrderTransferDetail obj = new ReceivedOrderTransferDetail();
                obj.ID = objNewDTO.ID;

                obj.BinID = objNewDTO.BinID;
                obj.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objNewDTO.ConsignedQuantity;
                obj.MeasurementID = objNewDTO.MeasurementID;
                obj.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
                obj.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
                obj.ExpirationDate = objNewDTO.ExpirationDate;
                obj.Cost = objNewDTO.Cost;
                obj.eVMISensorPort = objNewDTO.eVMISensorPort;
                obj.eVMISensorID = objNewDTO.eVMISensorID;
                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = objNewDTO.GUID;
                obj.ItemGUID = objNewDTO.ItemGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.PackSlipNumber = objNewDTO.PackSlipNumber;
                obj.IsDeleted = objNewDTO.IsDeleted.HasValue ? (bool)objNewDTO.IsDeleted : false;
                obj.IsArchived = objNewDTO.IsArchived.HasValue ? (bool)objNewDTO.IsArchived : false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.OrderDetailGUID = objNewDTO.OrderDetailGUID;

                obj.ItemLocationDetailGUID = objNewDTO.GUID;
                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = Convert.ToDateTime(objNewDTO.ReceivedOn);
                obj.ReceivedOnWeb = Convert.ToDateTime(objNewDTO.ReceivedOnWeb);
                obj.ControlNumber = objNewDTO.ControlNumber;
                if (!string.IsNullOrEmpty(objNewDTO.Expiration))
                {
                    obj.Expiration = objNewDTO.Expiration.Replace("-", "/");
                }

                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }
                obj.KitDetailGUID = objNewDTO.KitDetailGUID;
                obj.TransferDetailGUID = objNewDTO.TransferDetailGUID;
                obj.ReceivedDate = objNewDTO.ReceivedDate;
                obj.IsEDISent = objNewDTO.IsEDISent;
                obj.LastEDIDate = objNewDTO.LastEDIDate;
                context.ReceivedOrderTransferDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }


        /// <summary>
        /// Set RecieveOrderTransferDetail
        /// WI-1298
        /// </summary>
        /// <param name="rotd"></param>
        private ReceivedOrderTransferDetail SetRecieveOrderTransferDetail(ReceivedOrderTransferDetail objROTD, ReceivedOrderTransferDetailDTO objNewDTO)
        {
            objROTD.BinID = objNewDTO.BinID;
            objROTD.CustomerOwnedQuantity = objNewDTO.CustomerOwnedQuantity;
            objROTD.ConsignedQuantity = objNewDTO.ConsignedQuantity;
            objROTD.LotNumber = (!string.IsNullOrWhiteSpace(objNewDTO.LotNumber)) ? objNewDTO.LotNumber.Trim() : string.Empty;
            objROTD.SerialNumber = (!string.IsNullOrWhiteSpace(objNewDTO.SerialNumber)) ? objNewDTO.SerialNumber.Trim() : string.Empty;
            objROTD.ExpirationDate = objNewDTO.ExpirationDate;
            objROTD.Cost = objNewDTO.Cost;
            objROTD.UDF1 = objNewDTO.UDF1;
            objROTD.UDF2 = objNewDTO.UDF2;
            objROTD.UDF3 = objNewDTO.UDF3;
            objROTD.UDF4 = objNewDTO.UDF4;
            objROTD.UDF5 = objNewDTO.UDF5;

            objROTD.PackSlipNumber = objNewDTO.PackSlipNumber;

            objROTD.IsDeleted = false;
            objROTD.IsArchived = false;

            if (!string.IsNullOrEmpty(objNewDTO.Expiration))
            {
                objROTD.Expiration = objNewDTO.Expiration.Replace("-", "/");
            }

            if (!string.IsNullOrEmpty(objNewDTO.Received))
            {
                objROTD.Received = objNewDTO.Received.Replace("-", "/");
            }
            objROTD.ReceivedDate = objNewDTO.ReceivedDate;
            objROTD.ExpirationDate = objNewDTO.ExpirationDate;

            if (objNewDTO.IsOnlyFromUI)
            {
                objROTD.EditedFrom = "Web";
                objROTD.ReceivedOn = DateTimeUtility.DateTimeNow;
            }
            objROTD.Updated = DateTimeUtility.DateTimeNow;
            objROTD.LastUpdatedBy = objNewDTO.LastUpdatedBy;

            return objROTD;
        }




        public bool DeleteReturnedRecords(Guid[] guIDs, bool IsStaging, Int64 userid, Int64 RoomID, Int64 CompanyID, long SessionUserId, long EnterpriseId)
        {
            ItemLocationDetailsDAL itemLocDal = null;
            ReceivedOrderTransferDetailDTO objDTO = null;
            MaterialStagingPullDetailDAL MSPullDtlDAL = null;

            MaterialStagingPullDetailDTO MsPULLDtlDTO = null;

            List<ItemLocationDetailsDTO> itemLocDTOList = new List<ItemLocationDetailsDTO>();
            MaterialStagingDetailDAL MSdetailDAL = null;
            MaterialStagingDetailDTO MSDetailDTO = null;
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    MSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                    itemLocDal = new ItemLocationDetailsDAL(base.DataBaseName);

                    for (int i = 0; i < guIDs.Length; i++)
                    {
                        objDTO = GetROTDByGuidPlain(guIDs[i], RoomID, CompanyID);

                        if (IsStaging)
                        {
                            MsPULLDtlDTO = MSPullDtlDAL.GetRecord(objDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            MsPULLDtlDTO.CustomerOwnedQuantity = MsPULLDtlDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                            MsPULLDtlDTO.ConsignedQuantity = MsPULLDtlDTO.ConsignedQuantity.GetValueOrDefault(0) + objDTO.ConsignedQuantity.GetValueOrDefault(0);
                            MsPULLDtlDTO.Updated = DateTimeUtility.DateTimeNow;
                            MsPULLDtlDTO.LastUpdatedBy = userid;
                            MSPullDtlDAL.Edit(MsPULLDtlDTO);
                            MSPullDtlDAL.UpdateStagedQuantity(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, SessionUserId, EnterpriseId);

                            MSdetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                            //MSDetailDTO = MSdetailDAL.GetRecord(MsPULLDtlDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            MSDetailDTO = MSdetailDAL.GetMaterialStagingDetailByGUID(MsPULLDtlDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                            MSDetailDTO.Quantity = MSDetailDTO.Quantity + objDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.ConsignedQuantity.GetValueOrDefault(0);
                            MSdetailDAL.Edit(MSDetailDTO);
                        }
                        else
                        {
                            DeleteReturnedItems(objDTO, userid, SessionUserId, EnterpriseId);
                        }


                    }

                    return true;

                }
            }
            finally
            {
                itemLocDal = null;
                objDTO = null;
                MSPullDtlDAL = null;
                MsPULLDtlDTO = null;
                // itmLocDTO = null;

            }
        }

        public bool DeleteRecievedRecords(Guid[] guIDs, bool IsStaging, Int64 userid, Int64 RoomID, Int64 CompanyID, long SessionUserId)
        {
            ItemLocationDetailsDAL itemLocDal = null;
            ReceivedOrderTransferDetailDTO objDTO = null;
            MaterialStagingPullDetailDAL MSPullDtlDAL = null;
            string strQuery = "UPDATE ReceivedOrderTransferDetail SET  Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted = 1 WHERE ISNULL(IsEDISent,0) = 0 AND GUID  IN (";
            string strItemLocationIDs = "";

            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    for (int i = 0; i < guIDs.Length; i++)
                    {
                        if (i > 0)
                            strQuery += ",";

                        objDTO = GetROTDByGuidPlain(guIDs[i], RoomID, CompanyID);
                        if (!string.IsNullOrEmpty(strItemLocationIDs))
                            strItemLocationIDs += ",";

                        strItemLocationIDs += objDTO.ItemLocationDetailGUID;

                        strQuery += "'" + guIDs[i].ToString() + "'";

                    }

                    if (!string.IsNullOrEmpty(strItemLocationIDs) && strItemLocationIDs.Length > 0)
                    {
                        if (IsStaging)
                        {
                            MSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                            MSPullDtlDAL.DeleteRecordsFromOrder(strItemLocationIDs, userid, RoomID, CompanyID);
                        }
                        else
                        {
                            itemLocDal = new ItemLocationDetailsDAL(base.DataBaseName);
                            itemLocDal.DeleteRecords(strItemLocationIDs, userid, RoomID, CompanyID, "Replenish >> Delete ReceiveOrder", SessionUserId);
                        }
                    }

                    strQuery += ")";
                    context.Database.ExecuteSqlCommand(strQuery);


                    return true;

                }
            }
            finally
            {
                itemLocDal = null;
                objDTO = null;
                MSPullDtlDAL = null;
                strQuery = string.Empty;
                strItemLocationIDs = string.Empty;

            }
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID, long SessionUserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string strItemLocationIDs = "";
                bool IsStaging = false;
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        ReceivedOrderTransferDetailDTO objDTO = GetROTDByGuidPlain(Guid.Parse(item), RoomID, CompanyID);
                        BinMasterDTO binDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID);
                        //BinMasterDTO binDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, objDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                        IsStaging = binDTO.IsStagingLocation;

                        if (!string.IsNullOrEmpty(strItemLocationIDs))
                            strItemLocationIDs += ",";

                        strItemLocationIDs += objDTO.ItemLocationDetailGUID;

                        strQuery += "UPDATE ReceivedOrderTransferDetail SET  Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ISNULL(IsEDISent,0) = 0 AND GUID ='" + item.ToString() + "';";
                    }
                }
                if (context.Database.ExecuteSqlCommand(strQuery) > 0)
                {
                    if (!string.IsNullOrEmpty(strItemLocationIDs))
                    {
                        if (!IsStaging)
                        {
                            ItemLocationDetailsDAL itemLocDal = new ItemLocationDetailsDAL(base.DataBaseName);
                            itemLocDal.DeleteRecords(strItemLocationIDs, userid, RoomID, CompanyID, "Replenish >> Delete ReceiveOrderTransfer", SessionUserId);
                        }
                        else
                        {
                            MaterialStagingPullDetailDAL itemLocDal = new MaterialStagingPullDetailDAL(base.DataBaseName);
                            itemLocDal.DeleteRecordsFromOrder(strItemLocationIDs, userid, RoomID, CompanyID);
                        }
                    }
                }

                return true;
            }
        }

        public void DeleteReturnedItems(ReceivedOrderTransferDetailDTO ROTDs, Int64 UserID, long SessionUserId, long EnterpriseId)
        {

            if (ROTDs == null)
                return;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ReceivedOrderTransferDetail objROTD = null;
                ItemLocationDetail objILD = null;

                ItemMasterDAL objItemDAL = null;
                ItemLocationQTYDAL objLocQtyDAL = null;
                double ReturnedCustOwnQty = 0;
                double ReturnedConsQty = 0;
                List<ItemLocationQTYDTO> objListQty = new List<ItemLocationQTYDTO>();

                try
                {
                    //Step 1: SET ReceivedOrderTransferDetail Table record
                    objROTD = context.ReceivedOrderTransferDetails.FirstOrDefault(x => x.GUID == ROTDs.GUID);
                    ReturnedCustOwnQty = objROTD.CustomerOwnedQuantity.GetValueOrDefault(0);
                    ReturnedConsQty = objROTD.ConsignedQuantity.GetValueOrDefault(0);
                    objROTD.IsDeleted = true;
                    objROTD.EditedFrom = "Web";
                    objROTD.Updated = DateTimeUtility.DateTimeNow;
                    objROTD.LastUpdatedBy = UserID;
                    objROTD.ReceivedOn = DateTimeUtility.DateTimeNow;
                    context.SaveChanges();

                    //Step 2: SET Respected Location Detail Record. UpdateReceivedOrderTransferDetail Table record
                    objILD = context.ItemLocationDetails.FirstOrDefault(x => x.GUID == ROTDs.ItemLocationDetailGUID);
                    objILD.ConsignedQuantity = (objILD.ConsignedQuantity.GetValueOrDefault(0) + ReturnedCustOwnQty);
                    objILD.CustomerOwnedQuantity = (objILD.CustomerOwnedQuantity.GetValueOrDefault(0) + ReturnedConsQty);

                    objILD.LastUpdatedBy = UserID;
                    objILD.Updated = DateTimeUtility.DateTimeNow;
                    objILD.IsWebEdit = true;
                    objILD.EditedFrom = "Web";
                    objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objILD.InsertedFrom = "Delete Return";

                    context.SaveChanges();

                    objLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                    objListQty = objLocQtyDAL.GetListOfItemLocQty(ROTDs.ItemGUID ?? Guid.Empty, ROTDs.Room.GetValueOrDefault(0), ROTDs.CompanyID.GetValueOrDefault(0), UserID);

                    if (objListQty != null && objListQty.Count() > 0)
                    {
                        objLocQtyDAL.Save(objListQty, SessionUserId, EnterpriseId);
                    }

                    //Step 5: Update Cost mark up cell price and update cart for edited item.
                    objItemDAL = new ItemMasterDAL(base.DataBaseName);
                    objItemDAL.UpdateCostMarkupSellPrice(ROTDs.ItemGUID ?? Guid.Empty, ROTDs.Room.GetValueOrDefault(0), ROTDs.CompanyID.GetValueOrDefault(0), SessionUserId, EnterpriseId);

                }
                finally
                {
                    objROTD = null;
                    objILD = null;
                    objItemDAL = null;
                }

            }
        }

        public Int64 InsertReceive(ReceivedOrderTransferDetailDTO objDTO, long SessionUserId, bool IsReceiveCostChange, long EnterpriseId,bool IsFromRecieveService = false)
        {
            OrderDetailsDAL objOrdDtlDAL = new OrderDetailsDAL(base.DataBaseName);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 7200;//150;
                ItemLocationDetail objILD = GetItemLocationDetailObject(objDTO);
                context.ItemLocationDetails.Add(objILD);

                ReceivedOrderTransferDetail objROTD = GetReceiveOrderTransferDetailObject(objDTO);

                objROTD.ItemLocationDetailGUID = objILD.GUID;
                context.ReceivedOrderTransferDetails.Add(objROTD);

                PreReceivOrderDetail objPreRecieve = GetPreReceiveDetail(objDTO, context);
                context.SaveChanges();
                objOrdDtlDAL.UpdateOrderTrackingDetailData(objROTD.OrderDetailTrackingID, objROTD.CompanyID, objROTD.Room);
                IEnumerable<ItemLocationQTY> objItemLocationQtyDelete = context.ItemLocationQTies.Where(x => x.ItemGUID == objDTO.ItemGUID);
                foreach (var item in objItemLocationQtyDelete)
                {
                    context.ItemLocationQTies.Remove(item);
                }

                IEnumerable<ItemLocationQTY> objItemLocationQtyAdd = GetItemLocationQty(objDTO, context);
                foreach (var item in objItemLocationQtyAdd)
                {
                    //if (item.BinID == objROTD.BinID)
                    //{
                    //    item.CustomerOwnedQuantity = (item.CustomerOwnedQuantity ?? 0) + (objROTD.CustomerOwnedQuantity ?? 0);
                    //    item.ConsignedQuantity = (item.ConsignedQuantity ?? 0) + (objROTD.ConsignedQuantity ?? 0);
                    //    item.Quantity = (item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0);
                    //}

                    context.ItemLocationQTies.Add(item);
                }

                context.SaveChanges();
                objDTO.ID = objROTD.ID;
                objDTO.GUID = objROTD.GUID;


                IEnumerable<ReceivedOrderTransferDetail> lstROTDs = context.ReceivedOrderTransferDetails.Where(x => (x.OrderDetailGUID == objDTO.OrderDetailGUID) && !(x.IsDeleted ?? false) && !(x.IsArchived ?? false));
                double receivedQuantity = lstROTDs.Sum(x => (x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0));

                UpdateOrderDetails(objDTO, receivedQuantity,IsFromRecieveService);

                UpdateItemMaster(objDTO, context, SessionUserId, IsReceiveCostChange, EnterpriseId);
            }

            DashboardDAL objdashBoard = new DashboardDAL(base.DataBaseName);
            objdashBoard.UpdateTurnsByItemGUIDAfterTxn(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.ItemGUID ?? Guid.Empty, objDTO.LastUpdatedBy ?? 0);
            objdashBoard.UpdateAvgUsageByItemGUIDAfterTxn(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.ItemGUID ?? Guid.Empty, objDTO.LastUpdatedBy ?? 0, SessionUserId);

            return objDTO.ID;

        }

        private void UpdateOrderDetails(ReceivedOrderTransferDetailDTO objDTO, double RcvQty,bool IsFromRecieveService = false)
        {
            OrderDetailsDAL ordDetailDAL = new OrderDetailsDAL(base.DataBaseName);
            ItemMasterDAL ItemDAL = new ItemMasterDAL(base.DataBaseName);
            OrderDetailsDTO OrdDetailDTO = ordDetailDAL.GetOrderDetailByGuidFull(objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
            OrdDetailDTO.ReceivedQuantity = RcvQty;
            OrdDetailDTO.IsOnlyFromUI = objDTO.IsOnlyFromUI;
            //OrdDetailDTO.IsEDISent = objDTO.IsEDISent; //  commented for WI-5814: Receives are changing the OrderDetails.IsEDISent flag.
            OrdDetailDTO.LastEDIDate = objDTO.LastEDIDate;
            OrdDetailDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
            OrdDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            OrdDetailDTO.EditedFrom = objDTO.EditedFrom;
            OrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;


            // Update OrderLineItemExtendedCost and OrderLineItemExtendedPrice Based on Approved Quantity

            if (OrdDetailDTO.CostUOMValue == null || OrdDetailDTO.CostUOMValue <= 0)
            {
                OrdDetailDTO.CostUOMValue = 1;
            }

            if (OrdDetailDTO.OrderUOMValue == null || OrdDetailDTO.OrderUOMValue <= 0)
            {
                OrdDetailDTO.OrderUOMValue = 1;
            }

            if (OrdDetailDTO.ItemCostUOMValue == null
                || OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
            {
                OrdDetailDTO.ItemCostUOMValue = 1;
            }

            //OrdDetailDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDetailDTO.RequestedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.Cost.GetValueOrDefault(0) / OrdDetailDTO.CostUOMValue.GetValueOrDefault(1)))
            //                                                    : (OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.Cost.GetValueOrDefault(0) / OrdDetailDTO.CostUOMValue.GetValueOrDefault(1))))));

            //OrdDetailDTO.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDetailDTO.RequestedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.SellPrice.GetValueOrDefault(0) / OrdDetailDTO.CostUOMValue.GetValueOrDefault(1)))
            //                                                 : (OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.SellPrice.GetValueOrDefault(0) / OrdDetailDTO.CostUOMValue.GetValueOrDefault(1))))));

            #region WI-6215 and Other Relevant order cost related jira
            if (objDTO.Cost.GetValueOrDefault(0) > 0)
            {
                if (OrdDetailDTO.ItemCost.GetValueOrDefault(0)
                    != objDTO.Cost.GetValueOrDefault(0))
                {
                    OrdDetailDTO.ItemCost = objDTO.Cost.GetValueOrDefault(0);
                    if (OrdDetailDTO.ItemMarkup > 0)
                    {
                        OrdDetailDTO.ItemSellPrice = objDTO.Cost + ((objDTO.Cost * OrdDetailDTO.ItemMarkup) / 100);
                    }
                    else
                    {
                        OrdDetailDTO.ItemSellPrice = objDTO.Cost;
                    }
                }
            }
            else
            {
                OrdDetailDTO.ItemSellPrice = 0;
            }
            //if (objDTO.OrderStatus < (int)OrderStatus.Transmitted)
            //{
            //    OrdDetailDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDetailDTO.RequestedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemCost.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1)))
            //                                                     : (OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemCost.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1))))));

            //    OrdDetailDTO.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDetailDTO.RequestedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemSellPrice.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1)))
            //                                                     : (OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemSellPrice.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1))))));
            //}
            //else 
            if (objDTO.IsReceivedCostChange.GetValueOrDefault(false))
            {
                OrdDetailDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDetailDTO.RequestedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemCost.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                 : (OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemCost.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1))))));

                OrdDetailDTO.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDetailDTO.RequestedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemSellPrice.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                 : (OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (OrdDetailDTO.ItemSellPrice.GetValueOrDefault(0) / OrdDetailDTO.ItemCostUOMValue.GetValueOrDefault(1))))));
            }
            #endregion

            if (OrdDetailDTO.ReceivedQuantity != null && OrdDetailDTO.ReceivedQuantity >= 0)
            {
                ItemMasterDTO ImDTO = ItemDAL.GetItemByGuidPlain(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), OrdDetailDTO.Room.GetValueOrDefault(0), OrdDetailDTO.CompanyID.GetValueOrDefault(0));
                if (ImDTO != null && ImDTO.IsAllowOrderCostuom)
                {
                    OrdDetailDTO.ReceivedQuantityUOM = OrdDetailDTO.ReceivedQuantity / OrdDetailDTO.OrderUOMValue;
                    if (OrdDetailDTO.ReceivedQuantityUOM > 0 && OrdDetailDTO.ReceivedQuantityUOM < 1)
                        OrdDetailDTO.ReceivedQuantityUOM = 0;

                    if (ImDTO.SerialNumberTracking && (OrdDetailDTO.ReceivedQuantityUOM % 1) != 0)
                    {
                        OrdDetailDTO.ReceivedQuantityUOM = Math.Floor(OrdDetailDTO.ReceivedQuantityUOM.GetValueOrDefault(0));
                    }
                }
                else
                {
                    OrdDetailDTO.ReceivedQuantityUOM = OrdDetailDTO.ReceivedQuantity;
                }

            }
            if (!IsFromRecieveService)
            {
                OrdDetailDTO.POItemLineNumber = objDTO.POLineItemNumber;
            }
            ordDetailDAL.UpdateOrderDetail(OrdDetailDTO);
        }

        private void UpdateItemMaster(ReceivedOrderTransferDetailDTO objDTO, eTurnsEntities context, long SessionUserId, bool isReceiveCostChange, long EnterpriseId)
        {
            ItemMasterDAL objItem = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO oItemMaster = objItem.GetItemWithoutJoins(null, objDTO.ItemGUID.GetValueOrDefault(Guid.Empty));

            if (isReceiveCostChange)
            {
                //ItemMaster oItemMaster = context.ItemMasters.Where(x => x.GUID == objDTO.ItemGUID && x.CompanyID == objDTO.CompanyID && x.Room == objDTO.Room).FirstOrDefault();
                Room oRoom = context.Rooms.Where(x => x.ID == oItemMaster.Room && x.CompanyID == oItemMaster.CompanyID).FirstOrDefault();
                if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !oItemMaster.Consignment)
                    objItem.UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), false, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !oItemMaster.Consignment)
                    objItem.UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), true, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
            }
            oItemMaster = objItem.GetItemWithoutJoins(null, objDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
            if (context.ItemLocationQTies.Where(x => x.ItemGUID == objDTO.ItemGUID).Any())
            {
                oItemMaster.OnHandQuantity = context.ItemLocationQTies.Where(x => x.ItemGUID == objDTO.ItemGUID).Sum(x => (x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0));
            }
            else
            {
                oItemMaster.OnHandQuantity = 0;
            }
            oItemMaster.WhatWhereAction = "Insert Receive";
            oItemMaster.Updated = DateTimeUtility.DateTimeNow;
            oItemMaster.LastUpdatedBy = objDTO.LastUpdatedBy;
            if (isReceiveCostChange)
            {
                oItemMaster.Cost = objDTO.Cost;
            }
            objItem.Edit(oItemMaster, SessionUserId, EnterpriseId);


        }

        private IEnumerable<ItemLocationQTY> GetItemLocationQty(ReceivedOrderTransferDetailDTO objDTO, eTurnsEntities context)
        {
            //var qry = (from x in context.ItemLocationDetails
            //           join b in context.BinMasters on x.BinID equals b.ID
            //           where (x.BinID != null)
            //           && x.ItemGUID == objDTO.ItemGUID
            //           && (x.IsDeleted ?? false) == false
            //           && (x.IsArchived ?? false) == false
            //           && b.IsDeleted == false
            //           && (b.IsArchived ?? false) == false
            //           group x by new { x.ItemGUID, x.BinID } into grp
            //           select new ItemLocationQTYDTO
            //           {
            //               BinID = grp.Key.BinID ?? 0,
            //               ItemGUID = grp.Key.ItemGUID,
            //               CompanyID = objDTO.CompanyID,
            //               Room = objDTO.Room,
            //               LastUpdated = DateTimeUtility.DateTimeNow,
            //               Created = DateTimeUtility.DateTimeNow,
            //               CreatedBy = objDTO.CreatedBy,
            //               LastUpdatedBy = objDTO.LastUpdatedBy,
            //               CustomerOwnedQuantity = grp.Sum(x => x.CustomerOwnedQuantity ?? 0),
            //               ConsignedQuantity = grp.Sum(x => x.ConsignedQuantity ?? 0),
            //               Quantity = grp.Sum(x => (x.ConsignedQuantity ?? 0) + (x.CustomerOwnedQuantity ?? 0)),
            //               GUID = Guid.NewGuid(),
            //               AddedFrom = !string.IsNullOrEmpty(objDTO.AddedFrom) ? objDTO.AddedFrom : "Web",
            //               EditedFrom = !string.IsNullOrEmpty(objDTO.EditedFrom) ? objDTO.EditedFrom : "Web",
            //               ReceivedOn = DateTimeUtility.DateTimeNow,
            //               ReceivedOnWeb = DateTimeUtility.DateTimeNow,

            //           });

            //var sql = ((System.Data.Objects.ObjectQuery)qry).ToTraceString();
            //var objilq = qry.ToList();

            var objilq = context.Database.SqlQuery<ItemLocationQTYDTO>("exec uspGetItemLocationQty @ItemGUID", new SqlParameter("@ItemGUID", objDTO.ItemGUID)).ToList();


            List<ItemLocationQTY> lstQty = new List<ItemLocationQTY>();
            foreach (var item in objilq)
            {
                ItemLocationQTY qty = new ItemLocationQTY()
                {
                    AddedFrom = !string.IsNullOrEmpty(objDTO.AddedFrom) ? objDTO.AddedFrom : "Web",//item.AddedFrom,
                    BinID = item.BinID,
                    CompanyID = objDTO.CompanyID,//item.CompanyID,
                    ConsignedQuantity = item.ConsignedQuantity,
                    Created = DateTimeUtility.DateTimeNow,//item.Created,
                    CreatedBy = objDTO.CreatedBy,//item.CreatedBy,
                    EditedFrom = !string.IsNullOrEmpty(objDTO.EditedFrom) ? objDTO.EditedFrom : "Web",//item.EditedFrom,
                    CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                    GUID = item.GUID,
                    ID = item.ID,
                    ItemGUID = item.ItemGUID,
                    LastUpdated = DateTimeUtility.DateTimeNow,//item.LastUpdated,
                    LastUpdatedBy = objDTO.LastUpdatedBy,//item.LastUpdatedBy,
                    LotNumber = item.LotNumber,
                    ReceivedOn = DateTimeUtility.DateTimeNow,
                    Quantity = item.Quantity,
                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    Room = objDTO.Room//item.Room,
                };

                if (qty.GUID == null || qty.GUID == Guid.Empty)
                {
                    qty.GUID = Guid.NewGuid();
                }

                lstQty.Add(qty);
            }
            return lstQty;
        }


        private PreReceivOrderDetail GetPreReceiveDetail(ReceivedOrderTransferDetailDTO objROTD, eTurnsEntities context)
        {
            PreReceivOrderDetail objPreReceive = null;
            double qty = (objROTD.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objROTD.ConsignedQuantity.GetValueOrDefault(0));
            if (!string.IsNullOrEmpty(objROTD.LotNumber))
            {
                if (objROTD.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                && x.LotNumber.Trim() == objROTD.LotNumber.Trim() && x.ExpirationDate == objROTD.ExpirationDate
                                && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
                }
                else
                {
                    objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                && x.LotNumber.Trim() == objROTD.LotNumber.Trim() && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
                }
            }
            else if (!string.IsNullOrEmpty(objROTD.SerialNumber))
            {
                if (objROTD.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                && x.SerialNumber.Trim() == x.SerialNumber.Trim() && x.ExpirationDate == objROTD.ExpirationDate
                                && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
                }
                else
                {
                    objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                && x.SerialNumber.Trim() == x.SerialNumber.Trim() && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
                }
            }
            else if (objROTD.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
            {
                objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                               && x.ExpirationDate == objROTD.ExpirationDate
                                               && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
            }
            else
            {
                objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                              && x.ItemGUID == objROTD.ItemGUID
                                              && x.OrderDetailGUID == objROTD.OrderDetailGUID);
                //&& string.IsNullOrEmpty(x.SerialNumber) && string.IsNullOrEmpty(x.LotNumber) && !x.ExpirationDate.HasValue);
            }

            if (objPreReceive != null && objPreReceive.ID > 0)
            {
                objPreReceive.IsReceived = true;
                objPreReceive.Updated = DateTimeUtility.DateTimeNow;
            }

            return objPreReceive;


        }
        private ItemLocationDetail GetItemLocationDetailObject(ReceivedOrderTransferDetailDTO objDTO)
        {
            ItemLocationDetail objILD = new ItemLocationDetail();
            objILD.ID = 0;
            objILD.Created = DateTimeUtility.DateTimeNow;
            objILD.Updated = DateTimeUtility.DateTimeNow;
            objILD.GUID = Guid.NewGuid();
            objILD.IsDeleted = false;
            objILD.IsArchived = false;
            objILD.AddedFrom = "Unknown";
            objILD.EditedFrom = "Unknown";
            objILD.InsertedFrom = "InsertReceive";
            objILD.ReceivedOn = DateTimeUtility.DateTimeNow;
            objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

            objILD.BinID = objDTO.BinID;
            objILD.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
            objILD.ConsignedQuantity = objDTO.ConsignedQuantity;
            objILD.LotNumber = (!string.IsNullOrWhiteSpace(objDTO.LotNumber)) ? objDTO.LotNumber.Trim() : string.Empty;
            objILD.SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.SerialNumber)) ? objDTO.SerialNumber.Trim() : string.Empty;
            objILD.ExpirationDate = objDTO.ExpirationDate;
            objILD.Cost = objDTO.Cost;
            objILD.ReceivedDate = objDTO.ReceivedDate;
            objILD.ItemGUID = objDTO.ItemGUID;
            objILD.OrderDetailGUID = objDTO.OrderDetailGUID;
            objILD.UDF1 = objDTO.UDF1;
            objILD.UDF2 = objDTO.UDF2;
            objILD.UDF3 = objDTO.UDF3;
            objILD.UDF4 = objDTO.UDF4;
            objILD.UDF5 = objDTO.UDF5;
            objILD.CreatedBy = objDTO.CreatedBy;
            objILD.LastUpdatedBy = objDTO.LastUpdatedBy;
            objILD.CompanyID = objDTO.CompanyID;
            objILD.Room = objDTO.Room;

            objILD.InitialQuantity = (objDTO.CustomerOwnedQuantity ?? 0) + (objDTO.ConsignedQuantity ?? 0);
            objILD.InitialQuantityWeb = (objDTO.CustomerOwnedQuantity ?? 0) + (objDTO.ConsignedQuantity ?? 0);

            if (objDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                objILD.IsConsignedSerialLot = true;

            if (objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                objILD.Expiration = objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");

            if (objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                objILD.Received = objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");

            if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                objILD.AddedFrom = objDTO.AddedFrom;

            if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                objILD.EditedFrom = objDTO.EditedFrom;

            return objILD;
        }

        private ReceivedOrderTransferDetail GetReceiveOrderTransferDetailObject(ReceivedOrderTransferDetailDTO objDTO)
        {
            ReceivedOrderTransferDetail objROTD = new ReceivedOrderTransferDetail();

            objROTD.ID = 0;
            objROTD.Created = DateTimeUtility.DateTimeNow;
            objROTD.Updated = DateTimeUtility.DateTimeNow;
            objROTD.GUID = Guid.NewGuid();
            objROTD.IsDeleted = false;
            objROTD.IsArchived = false;
            objROTD.AddedFrom = objDTO.AddedFrom ?? "Unknown";
            objROTD.EditedFrom = objDTO.EditedFrom ?? "Unknown";

            objROTD.ReceivedOn = DateTimeUtility.DateTimeNow;
            objROTD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

            objROTD.BinID = objDTO.BinID;
            objROTD.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
            objROTD.ConsignedQuantity = objDTO.ConsignedQuantity;
            objROTD.LotNumber = (!string.IsNullOrWhiteSpace(objDTO.LotNumber)) ? objDTO.LotNumber.Trim() : string.Empty;
            objROTD.SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.SerialNumber)) ? objDTO.SerialNumber.Trim() : string.Empty;
            objROTD.ExpirationDate = objDTO.ExpirationDate;
            objROTD.Cost = objDTO.Cost;
            objROTD.ReceivedDate = objDTO.ReceivedDate;
            objROTD.ItemGUID = objDTO.ItemGUID;
            objROTD.OrderDetailGUID = objDTO.OrderDetailGUID;
            objROTD.UDF1 = objDTO.UDF1;
            objROTD.UDF2 = objDTO.UDF2;
            objROTD.UDF3 = objDTO.UDF3;
            objROTD.UDF4 = objDTO.UDF4;
            objROTD.UDF5 = objDTO.UDF5;
            objROTD.CreatedBy = objDTO.CreatedBy;
            objROTD.LastUpdatedBy = objDTO.LastUpdatedBy;
            objROTD.CompanyID = objDTO.CompanyID;
            objROTD.Room = objDTO.Room;
            objROTD.IsEDISent = objDTO.IsEDISent;
            objROTD.LastEDIDate = objDTO.LastEDIDate;
            objROTD.PackSlipNumber = objDTO.PackSlipNumber;
            objROTD.OrderDetailTrackingID = objDTO.OrderDetailTrackingID;

            if (objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                objROTD.Expiration = objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            if (objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                objROTD.Received = objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                objROTD.AddedFrom = objDTO.AddedFrom;

            if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                objROTD.EditedFrom = objDTO.EditedFrom;


            return objROTD;

        }

        public IEnumerable<ReceivedOrderTransferDetailDTO> GetPagedReceivedOrderTransferDetailRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            string stryQry = "EXEC [GetReceivedOrderTransferDetail] @RoomID,@CompanyID,@OrderGuid,@OrderDetailGuid,@ItemGuid,@BinID,@IsDeleted,@IsArchived,@ROTDGuid";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@OrderGuid", DBNull.Value),
                    new SqlParameter("@OrderDetailGuid", OrderDetailGUID),
                    new SqlParameter("@ItemGuid", ItemGUID),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@ROTDGuid", DBNull.Value)
                };

                IEnumerable<ReceivedOrderTransferDetailDTO> ObjCache = context.Database.SqlQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        #endregion





    }
}
