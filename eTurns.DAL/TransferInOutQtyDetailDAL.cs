using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class TransferInOutQtyDetailDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        #region [Class Constructor]

        //public TransferInOutQtyDetailDAL(base.DataBaseName)
        //{

        //}

        public TransferInOutQtyDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public TransferInOutQtyDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// GetTransferInOutQtyDetailsByTransferfDtlGUIDNormal
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferInOutQtyDetailDTO> GetTransferInOutQtyDetailsByTransferfDtlGUIDNormal(Guid TransferDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TransferDetailGUID", TransferDetailGUID) };
                return context.Database.SqlQuery<TransferInOutQtyDetailDTO>("EXEC [GetTransferInOutQtyDetailsByTransferfDtlGUIDNormal] @TransferDetailGUID", params1).ToList();
            }
        }

        /// <summary>
        /// This Method is used to get the data for receive transfer detail grid.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferInOutQtyDetailDTO> GetTransferInOutQtyDetailsForReceiveByTransferfDtlGUIDNormal(Guid TransferDetailGUID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TransferDetailGUID", TransferDetailGUID) };
                return context.Database.SqlQuery<TransferInOutQtyDetailDTO>("EXEC [GetTransferInOutQtyDetailsForReceiveByTransferfDtlGUIDNormal] @TransferDetailGUID", params1).ToList();
            }

        }

        /// <summary>
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferInOutQtyDetailDTO GetTransferInOutQtyDetailByTrfAndItem(Guid TransferDetailGuid,Guid TransferGuid, Guid ItemGuid )
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferDetailGuid", TransferDetailGuid),
                                                   new SqlParameter("@TransferGuid", TransferGuid),
                                                   new SqlParameter("@ItemGuid", ItemGuid)
                                                };
                return context.Database.SqlQuery<TransferInOutQtyDetailDTO>("exec [GetTransferInOutQtyDetailByTrfAndItem] @TransferDetailGuid,@TransferGuid,@ItemGuid ", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<TransferInOutQtyDetailDTO> GetTrfInOutQtyDetailByTrfDetailAndItem(Guid TransferDetailGuid, long CompanyID, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferDetailGuid", TransferDetailGuid),
                                                   new SqlParameter("@ItemGuid", ItemGuid),
                                                   new SqlParameter("@CompanyID", CompanyID)                                                   
                                                };
                return context.Database.SqlQuery<TransferInOutQtyDetailDTO>("exec [GetTrfInOutQtyDetailByTrfDetailAndItem] @TransferDetailGuid,@ItemGuid,@CompanyID ", params1).ToList();
            }
        }


        /// <summary>
        /// This method is used to get the records of TransferInOutQtyDetail table based on TransferDetailGUID 
        /// And Lot/Serial Numbers (Lot/serial selection implementation in transfer)
        /// </summary>
        /// <param name="TransferDetailGuid"></param>
        /// <param name="CompanyID"></param>
        /// <param name="serialNumbers"></param>
        /// <param name="lotNumbers"></param>
        /// <returns></returns>
        public List<TransferInOutQtyDetailDTO> GetTransferInOutQtyDetailByTransferDetailAndItem(Guid TransferDetailGuid, Guid itemGUID, Int64 CompanyID,List<string> serialNumbers, List<string> lotNumbers)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferDetailGuid", TransferDetailGuid),
                                                   new SqlParameter("@ItemGuid", itemGUID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@SerialNumbers", string.Join(",", serialNumbers)),
                                                   new SqlParameter("@LotNumbers", string.Join(",", lotNumbers)),
                                                };
                return context.Database.SqlQuery<TransferInOutQtyDetailDTO>("exec [GetTransferInOutQtyDetailByTransferDetailAndItem] @TransferDetailGuid,@ItemGuid,@CompanyID,@SerialNumbers,@LotNumbers ", params1).ToList();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase KitMoveInOutDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(TransferInOutQtyDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TransferInOutQtyDetail obj = new TransferInOutQtyDetail()
                {
                    ID = 0,
                    ItemGUID = objDTO.ItemGUID,
                    BinID = objDTO.BinID,
                    CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity,
                    ConsignedQuantity = objDTO.ConsignedQuantity,
                    TotalQuantity = objDTO.TotalQuantity,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    CreatedBy = objDTO.CreatedBy,
                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    Room = objDTO.Room,
                    CompanyID = objDTO.CompanyID,
                    IsDeleted = false,
                    IsArchived = false,
                    GUID = Guid.NewGuid(),
                    TransferDetailGUID = objDTO.TransferDetailGUID,
                    TransferGUID = objDTO.TransferGUID,
                    TransferInOut = objDTO.TransferInOut,
                    IsTransfered = objDTO.IsTransfered,
                    ReceivedOn = DateTimeUtility.DateTimeNow,
                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    AddedFrom = "Web",
                    EditedFrom = "Web",
                };
                context.TransferInOutQtyDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                if (objDTO.TrfInOutItemDetail != null && objDTO.TrfInOutItemDetail.Count > 0)
                {
                    TransferInOutItemDetailDAL objDAL = new TransferInOutItemDetailDAL(base.DataBaseName);
                    foreach (var item in objDTO.TrfInOutItemDetail)
                    {
                        item.TransferInOutQtyDetailGUID = objDTO.GUID;
                        objDAL.Insert(item);
                    }
                }
                return obj.ID;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase KitMoveInOutDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public bool EditLocation(TransferInOutQtyDetailDTO objDTO)
        {
            TransferDetailDAL transferDetailDAL = new TransferDetailDAL(base.DataBaseName);
            var trfDetail = transferDetailDAL.GetTransferDetailsByGuidPlain(objDTO.TransferDetailGUID ?? Guid.Empty);
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TransferInOutQtyDetail obj = context.TransferInOutQtyDetails.Single(t => t.GUID == objDTO.GUID);

                if (trfDetail != null &&  (trfDetail.ApprovedQuantity ?? 0) > 0 && ((trfDetail.ApprovedQuantity ?? 0) - (trfDetail.ReceivedQuantity ?? 0)) <= 0)
                    obj.IsTransfered = true;
                else if (objDTO.IsTransfered)
                    obj.IsTransfered = true;
                //else if ((trfDetail.ApprovedQuantity ?? 0) > 0 &&
                //        obj.TotalQuantity <= ((trfDetail.ReceivedQuantity ?? 0) + objDTO.TotalQuantity))
                //    obj.IsTransfered = true;

                obj.BinID = objDTO.BinID;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.TransferInOutQtyDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                TransferInOutItemDetailDAL objDAL = new TransferInOutItemDetailDAL(base.DataBaseName);
                List<TransferInOutItemDetailDTO> oTransferInOutItemDetailDTOList = objDAL.GetTrfInOutItemDetailByInOutQtyDetailGuidPlain(objDTO.GUID);

                if (oTransferInOutItemDetailDTOList != null && oTransferInOutItemDetailDTOList.Count > 0)
                {
                    foreach (var item in oTransferInOutItemDetailDTOList)
                    {
                        item.BinID = objDTO.BinID;
                        item.ReceivedDate = DateTimeUtility.DateTimeNow;
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        item.IsTransfered = obj.IsTransfered;
                        objDAL.EditLocation(item);
                    }
                }
                return true;
            }

        }


        /// <summary>
        /// DecreaseQuantity
        /// </summary>
        /// <param name="objMoveInQty"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public ResponseMessage TransferQuantity(TakenQtyDetail objMoveInQty, Int64 RoomID, Int64 CompanyID, Int64 UserID, long SessionUserId, string CultureCode,long EnterPriseId)
        {
            Guid itemGUID = Guid.Empty;
            ItemMasterDTO ItemDTO = null;
            ResponseMessage response = new ResponseMessage();
            if (Guid.TryParse(objMoveInQty.ItemGUID, out itemGUID))
            {
                ItemDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, itemGUID);
            }

            if (ItemDTO != null && ItemDTO.OnHandQuantity < objMoveInQty.TotalQty)
            {
                var transferResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", CultureCode, EnterPriseId,CompanyID); 
                string notEnoughQtyMsg = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantity", transferResourceFilePath, EnterPriseId, CompanyID, RoomID, "ResTransfer", CultureCode);
                response.IsSuccess = false;
                response.Message = notEnoughQtyMsg;
                return response;
            }
            else
            {
                bool IsQtyAvailable = true;
                string msg = "";

                foreach (var item in objMoveInQty.BinWiseQty)
                {

                    ResponseMessage ResponseMsg = new CommonDAL(base.DataBaseName).CheckQuantityByLocation(item.LocationID, Guid.Parse(objMoveInQty.ItemGUID), item.Quantity, RoomID, CompanyID,EnterPriseId,UserID);
                    if (!ResponseMsg.IsSuccess)
                    {
                        msg = ResponseMsg.Message;
                        IsQtyAvailable = false;
                        break;
                    }
                }

                if (!IsQtyAvailable)
                {
                    response.IsSuccess = false;
                    response.Message = msg;
                    return response;
                }
            }

            foreach (var item in objMoveInQty.BinWiseQty)
            {
                //ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == item.LocationID && x.ItemGUID == Guid.Parse(objMoveInQty.ItemGUID)).SingleOrDefault();
                ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationQTYByItemBinPlain(RoomID, CompanyID, item.LocationID, objMoveInQty.ItemGUID);
                //BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(item.LocationID, RoomID, CompanyID);
                //BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, item.LocationID,null,null).FirstOrDefault();
                ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                double CosigneQty = 0;
                double CustomerQty = 0;
                List<TransferInOutItemDetailDTO> objTItemDetailList = new List<TransferInOutItemDetailDTO>();
                PullTransactionDAL objPullDAL = new PullTransactionDAL(base.DataBaseName);

                #region "ItemLocation Deduction"
                if (ItemDTO.SerialNumberTracking)
                {
                    #region "Serial logic"
                    List<ItemLocationLotSerialDTO> ObjItemLocLotSerial = null;
                    //ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == item.LocationID && (x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 || x.ConsignedQuantity.GetValueOrDefault(0) > 0)).Take((int)item.Quantity).ToList();
                    ObjItemLocLotSerial = objPullDAL.GetItemLocationsLotSerials(Guid.Parse(objMoveInQty.ItemGUID), RoomID, CompanyID, item.LocationID, item.Quantity, true);
                    foreach (var itemoil in ObjItemLocLotSerial)
                    {
                        ItemLocationDetailsDTO objItemLocationDTO = objLocationDAL.GetItemLocationDetailByIdNormal(itemoil.ID);

                        TransferInOutItemDetailDTO objTItemDetail = new TransferInOutItemDetailDTO()
                        {
                            BinID = objItemLocationDTO.BinID.GetValueOrDefault(0),
                            CompanyID = objItemLocationDTO.CompanyID.GetValueOrDefault(0),
                            Room = RoomID,
                            TransferInOutQtyDetailGUID = Guid.Empty,
                            TransferedDate = DateTime.Now,
                            SerialNumber = objItemLocationDTO.SerialNumber,
                            Cost = objItemLocationDTO.Cost,
                            ReceivedDate = objItemLocationDTO.ReceivedDate,
                            LotNumber = objItemLocationDTO.LotNumber,
                            Created = DateTime.Now,
                            CreatedBy = UserID,
                            ExpirationDate = objItemLocationDTO.ExpirationDate,
                            GUID = Guid.NewGuid(),
                            ID = 0,
                            IsArchived = false,
                            IsDeleted = false,
                            ItemGUID = objItemLocationDTO.ItemGUID,
                            TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID),
                        };

                        if (ItemDTO.Consignment)
                        {
                            if (objItemLocationDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                            {
                                objItemLocationDTO.ConsignedQuantity = 0;
                                objTItemDetail.ConsignedQuantity = 1;
                            }
                            else
                            {
                                objItemLocationDTO.CustomerOwnedQuantity = 0;
                                objTItemDetail.CustomerOwnedQuantity = 1;
                            }
                        }
                        else
                        {
                            objItemLocationDTO.CustomerOwnedQuantity = 0;
                            objTItemDetail.CustomerOwnedQuantity = 1;
                        }

                        objItemLocationDTO.TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID);
                        objLocationDAL.Edit(objItemLocationDTO);
                        objTItemDetailList.Add(objTItemDetail);
                    }
                    #endregion
                }
                else
                {
                    #region "Lot and other type logic"
                    //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == item.LocationID).OrderBy("CustomerOwnedQuantity DESC").ToList();
                    Double takenQunatity = 0;
                    List<ItemLocationLotSerialDTO> ObjItemLocLotSerial = null;
                    ObjItemLocLotSerial = objPullDAL.GetItemLocationsLotSerials(Guid.Parse(objMoveInQty.ItemGUID), RoomID, CompanyID, item.LocationID, item.Quantity, true);

                    foreach (var itemoil in ObjItemLocLotSerial)
                    {
                        ItemLocationDetailsDTO objItemLocationDTO = objLocationDAL.GetItemLocationDetailByIdNormal(itemoil.ID);

                        TransferInOutItemDetailDTO objTItemDetail = new TransferInOutItemDetailDTO()
                        {
                            BinID = objItemLocationDTO.BinID.GetValueOrDefault(0),
                            CompanyID = objItemLocationDTO.CompanyID.GetValueOrDefault(0),
                            Room = RoomID,
                            TransferInOutQtyDetailGUID = Guid.Empty,
                            TransferedDate = DateTime.Now,
                            SerialNumber = objItemLocationDTO.SerialNumber,
                            Cost = objItemLocationDTO.Cost,
                            ReceivedDate = objItemLocationDTO.ReceivedDate,
                            LotNumber = objItemLocationDTO.LotNumber,
                            Created = DateTime.Now,
                            CreatedBy = UserID,
                            ExpirationDate = objItemLocationDTO.ExpirationDate,
                            GUID = Guid.NewGuid(),
                            ID = 0,
                            IsArchived = false,
                            IsDeleted = false,
                            ItemGUID = objItemLocationDTO.ItemGUID,
                            TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID),
                        };

                        Double loopCurrentTakenCustomer = 0;
                        Double loopCurrentTakenConsignment = 0;
                        if (takenQunatity == item.Quantity)
                        {
                            break;
                        }
                        objItemLocationDTO.TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID);
                        if (ItemDTO.Consignment)
                        {
                            #region "Consignment Pull"
                            if (objItemLocationDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && objItemLocationDTO.ConsignedQuantity >= (item.Quantity - takenQunatity))
                            {
                                loopCurrentTakenConsignment = item.Quantity - takenQunatity;
                                objItemLocationDTO.ConsignedQuantity = (Double)objItemLocationDTO.ConsignedQuantity - (item.Quantity - takenQunatity);
                                takenQunatity = (item.Quantity - takenQunatity);

                                objTItemDetail.ConsignedQuantity = objTItemDetail.ConsignedQuantity.GetValueOrDefault(0) + takenQunatity;

                            }
                            else if (objItemLocationDTO.CustomerOwnedQuantity >= (item.Quantity - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (item.Quantity - takenQunatity);
                                objItemLocationDTO.CustomerOwnedQuantity = (Double)objItemLocationDTO.CustomerOwnedQuantity - (item.Quantity - takenQunatity);
                                takenQunatity = item.Quantity - takenQunatity;
                                objTItemDetail.CustomerOwnedQuantity = objTItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + takenQunatity;
                            }
                            else
                            {
                                takenQunatity = objItemLocationDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemLocationDTO.CustomerOwnedQuantity = objItemLocationDTO.CustomerOwnedQuantity - takenQunatity;
                                objTItemDetail.CustomerOwnedQuantity = objTItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + takenQunatity;

                                if (objItemLocationDTO.ConsignedQuantity >= (item.Quantity - takenQunatity))
                                {
                                    loopCurrentTakenConsignment = (item.Quantity - takenQunatity);
                                    objItemLocationDTO.ConsignedQuantity = objItemLocationDTO.ConsignedQuantity - (item.Quantity - takenQunatity);
                                    takenQunatity += item.Quantity - takenQunatity;

                                    objTItemDetail.ConsignedQuantity = objTItemDetail.ConsignedQuantity.GetValueOrDefault(0) + takenQunatity;
                                }
                                else
                                {
                                    loopCurrentTakenConsignment = objItemLocationDTO.ConsignedQuantity.GetValueOrDefault(0);
                                    takenQunatity += objItemLocationDTO.ConsignedQuantity.GetValueOrDefault(0);
                                    objItemLocationDTO.ConsignedQuantity = 0;

                                    objTItemDetail.ConsignedQuantity = objTItemDetail.ConsignedQuantity.GetValueOrDefault(0) + takenQunatity;
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region "Customer own Pull"
                            if (objItemLocationDTO.CustomerOwnedQuantity >= (item.Quantity - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (item.Quantity - takenQunatity);
                                objItemLocationDTO.CustomerOwnedQuantity = (Double)objItemLocationDTO.CustomerOwnedQuantity - (item.Quantity - takenQunatity);
                                takenQunatity += (item.Quantity - takenQunatity);
                                objTItemDetail.CustomerOwnedQuantity = objTItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + takenQunatity;
                            }
                            else
                            {
                                loopCurrentTakenCustomer = objItemLocationDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                takenQunatity += objItemLocationDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemLocationDTO.CustomerOwnedQuantity = objItemLocationDTO.CustomerOwnedQuantity - takenQunatity;
                                objTItemDetail.CustomerOwnedQuantity = objTItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + takenQunatity;
                            }
                            #endregion
                        }
                        objItemLocationDTO.TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID);
                        objLocationDAL.Edit(objItemLocationDTO);
                        objTItemDetailList.Add(objTItemDetail);
                    }
                    #endregion
                }
                #endregion

                #region "ItemLocation Quantity Deduction"

                if (ItemDTO.Consignment)
                {
                    //Both's sum we have available.
                    if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                    {
                        CosigneQty = item.Quantity;
                        lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - item.Quantity;
                        lstLocDTO.Quantity = lstLocDTO.Quantity - item.Quantity;
                    }
                    else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= item.Quantity)
                    {
                        CustomerQty = item.Quantity;
                        lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - item.Quantity;
                        lstLocDTO.Quantity = lstLocDTO.Quantity - item.Quantity;
                    }
                    else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < item.Quantity)
                    {
                        Double cstqty = item.Quantity - (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); // -((Double)objDTO.TempPullQTY - cstqty);
                        Double consqty = item.Quantity - cstqty;

                        lstLocDTO.CustomerOwnedQuantity = cstqty;
                        lstLocDTO.ConsignedQuantity = consqty;
                        CustomerQty = cstqty;
                        CosigneQty = consqty;
                        lstLocDTO.Quantity = lstLocDTO.Quantity - ((Double)cstqty + (Double)consqty);
                    }
                }
                else
                {
                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - item.Quantity;
                    lstLocDTO.Quantity = lstLocDTO.Quantity - item.Quantity;
                    CustomerQty = item.Quantity;
                }
                #endregion

                #region "Saving Location QTY data"

                ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                lstUpdate.Add(lstLocDTO);
                objLocQTY.Save(lstUpdate, SessionUserId,EnterPriseId);

                #endregion

                #region "Saving Kit move inout table"

                TransferDetailDTO objTrfDetailDTO = new TransferDetailDAL(base.DataBaseName).GetTransferDetailsByGuidPlain(Guid.Parse(objMoveInQty.DetailGUID));

                TransferInOutQtyDetailDTO objTQtyDetail = new TransferInOutQtyDetailDTO()
                {
                    BinID = item.LocationID,
                    TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID),
                    TransferInOut = "OUT",
                    ItemGUID = new Guid(objMoveInQty.ItemGUID),
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    CreatedBy = UserID,
                    LastUpdatedBy = UserID,
                    CompanyID = CompanyID,
                    Room = RoomID,

                    IsArchived = false,
                    IsDeleted = false,

                    ConsignedQuantity = CosigneQty,
                    CustomerOwnedQuantity = CustomerQty,
                    TotalQuantity = item.Quantity,
                    GUID = Guid.NewGuid(),
                    ID = 0,
                    TrfInOutItemDetail = objTItemDetailList,
                    TransferGUID = objTrfDetailDTO.TransferGUID
                };

                Insert(objTQtyDetail);
                #endregion
            }

            TransferDetailDAL DetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferMasterDAL MasterDAL = new TransferMasterDAL(base.DataBaseName);

            #region "Update Transfer Detail table"
            TransferDetailDTO DetailDTO = DetailDAL.GetTransferDetailsByGuidPlain(Guid.Parse(objMoveInQty.DetailGUID));
            DetailDTO.IntransitQuantity = DetailDTO.IntransitQuantity.GetValueOrDefault(0) + objMoveInQty.TotalQty;
            DetailDTO.ShippedQuantity = DetailDTO.ShippedQuantity.GetValueOrDefault(0) + objMoveInQty.TotalQty;
            DetailDTO.FulFillQuantity = DetailDTO.FulFillQuantity.GetValueOrDefault(0) + objMoveInQty.TotalQty;

            DetailDTO.LastUpdatedBy = UserID;
            DetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            DetailDTO.EditedFrom = "Web";
            DetailDAL.Edit(DetailDTO, SessionUserId,EnterPriseId);

            TransferMasterDTO MasterDTO = MasterDAL.GetTransferByGuidPlain(DetailDTO.TransferGUID);

            //TODO: Code for Replinishing Room

            TransferMasterDTO ReplenishMasterDTO = null;// MasterDAL.GetRecord(DetailDTO.TransferID, RoomID, CompanyID);
            if (MasterDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                ReplenishMasterDTO = MasterDAL.GetTransferByGuidPlain(MasterDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty));
            else
                ReplenishMasterDTO = MasterDAL.GetTransferByRefTransferGuidPlain(MasterDTO.ReplenishingRoomID, CompanyID, MasterDTO.GUID);

            if (ReplenishMasterDTO != null)
            {
                var replinishRoomItemGuid = GetSameItemByIDInReplinishRoom(Guid.Parse(objMoveInQty.ItemGUID), ReplenishMasterDTO.RoomID, CompanyID);
                var tmpsupplierIds = new List<long>();
                //List<TransferDetailDTO> ReplinishDetailDTOList = DetailDAL.GetTransferedRecord(ReplenishMasterDTO.GUID, ReplenishMasterDTO.RoomID, ReplenishMasterDTO.CompanyID, tmpsupplierIds);
                var ReplinishDetailDTOList = DetailDAL.GetTransferDetailsByTrfGuidAndItemGuidNormal(ReplenishMasterDTO.GUID, replinishRoomItemGuid);

                TransferDetailDTO ReplenishDetailDTO = ReplinishDetailDTOList.FirstOrDefault();
                //TransferDetailDTO ReplenishDetailDTO = ReplinishDetailDTOList.Where(x => x.ItemGUID == replinishRoomItemGuid).SingleOrDefault();
                ReplenishDetailDTO.IntransitQuantity = ReplenishDetailDTO.IntransitQuantity.GetValueOrDefault(0) + objMoveInQty.TotalQty;
                ReplenishDetailDTO.ShippedQuantity = ReplenishDetailDTO.ShippedQuantity.GetValueOrDefault(0) + objMoveInQty.TotalQty;
                ReplenishDetailDTO.FulFillQuantity = ReplenishDetailDTO.FulFillQuantity.GetValueOrDefault(0) + objMoveInQty.TotalQty;

                ReplenishDetailDTO.LastUpdatedBy = UserID;
                ReplenishDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                ReplenishDetailDTO.EditedFrom = "Web";
                DetailDAL.Edit(ReplenishDetailDTO, SessionUserId,EnterPriseId);

                if (ReplenishMasterDTO.TransferStatus < (int)TransferStatus.FullFillQuantity)
                {
                    ReplenishMasterDTO.TransferStatus = (int)TransferStatus.FullFillQuantity;
                    ReplenishMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    ReplenishMasterDTO.EditedFrom = "Web";
                    MasterDAL.Edit(ReplenishMasterDTO);
                }

                if (MasterDTO.TransferStatus < (int)TransferStatus.FullFillQuantity)
                {
                    MasterDTO.TransferStatus = (int)TransferStatus.FullFillQuantity;
                    MasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    MasterDTO.EditedFrom = "Web";
                    MasterDAL.Edit(MasterDTO);
                }
            }

            var transferResFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", CultureCode, EnterPriseId, CompanyID);
            string quantityTransferredMsg = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityTransferred", transferResFilePath, EnterPriseId, CompanyID, RoomID, "ResTransfer", CultureCode);

            response.IsSuccess = true;
            response.Message = quantityTransferredMsg; 
            #endregion
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objMoveInQty"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public ResponseMessage TransferApprovedQuantity(TakenQtyDetail objMoveInQty, Int64 RoomID, Int64 CompanyID, Int64 UserID, long SessionUserId, long EnterpriseId, 
            string InventoryConsuptionMethod, string CurrentCulture,string MsgQuantityTransferred,string MsgQuantityNotAvailableToTransfer,string MsgApprovedQuantityIsZero,string MsgApprovedQuantityAlreadyTransferred)
        {
            ResponseMessage response = new ResponseMessage();
            ItemLocationDetailsDAL objItemLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemLocationQTYDAL itemLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            TransferDetailDAL TransferDetailDAL = new TransferDetailDAL(base.DataBaseName);
            RoomDAL roomDAL = new RoomDAL(base.DataBaseName);
            List<ItemLocationQTYDTO> lstLocQty = new List<ItemLocationQTYDTO>();
            BinMasterDAL binDAL = new DAL.BinMasterDAL(base.DataBaseName);
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);
            string orderByString = "ReceivedDate asc,ID asc";
            
            if (string.IsNullOrEmpty(InventoryConsuptionMethod) || string.IsNullOrWhiteSpace(InventoryConsuptionMethod))
            {
                string columnList = "ID,RoomName,InventoryConsuptionMethod";
                RoomDTO roomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                if (roomDTO != null && roomDTO.InventoryConsuptionMethod != "Fifo")
                {
                    orderByString = "ReceivedDate desc,ID desc";
                }                    
            }
            else
            {
                if (InventoryConsuptionMethod != "Fifo")
                {
                    orderByString = "ReceivedDate desc,ID desc";
                }
            }
            

            //double QtyToTransfer = objMoveInQty.TotalQty - objMoveInQty.ReceivedQty;
            TransferDetailDTO objTrfDetailDTO = TransferDetailDAL.GetTransferDetailsByGuidNormal(Guid.Parse(objMoveInQty.DetailGUID));
            //TODO: Chirag on 2017-08-10 Commented and change for WI-3153
            //double QtyToTransfer = objMoveInQty.TotalQty - (objTrfDetailDTO.FulFillQuantity.GetValueOrDefault(0) + objTrfDetailDTO.ReceivedQuantity.GetValueOrDefault(0));
            //double QtyToTransfer = objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) - (objTrfDetailDTO.FulFillQuantity.GetValueOrDefault(0) + objTrfDetailDTO.ReceivedQuantity.GetValueOrDefault(0));
            double QtyToTransfer = objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) - (objTrfDetailDTO.FulFillQuantity.GetValueOrDefault(0));
            //if (objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) > 0 && objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) <= (objTrfDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + objTrfDetailDTO.FulFillQuantity.GetValueOrDefault(0)))

            var cultureCode = "en-US";
            if (string.IsNullOrEmpty(CurrentCulture) || string.IsNullOrWhiteSpace(CurrentCulture))
            {
                var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }
            }
            else
            {
                cultureCode = CurrentCulture;
            }

            var transferResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", cultureCode, EnterpriseId, CompanyID);            

            if (QtyToTransfer > objMoveInQty.TotalQty)
            {
                QtyToTransfer = objMoveInQty.TotalQty;
            }

            if (objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) > 0 && objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) <= (objTrfDetailDTO.FulFillQuantity.GetValueOrDefault(0)))
            {
                string approvedQtyAlreadyTransferredMsg = (string.IsNullOrEmpty(MsgApprovedQuantityAlreadyTransferred) || string.IsNullOrWhiteSpace(MsgApprovedQuantityAlreadyTransferred))
                    ? ResourceRead.GetResourceValueByKeyAndFullFilePath("ApprovedQuantityAlreadyTransferred", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode)
                    : MsgApprovedQuantityAlreadyTransferred;

                response.IsSuccess = false; 
                response.Message = approvedQtyAlreadyTransferredMsg;
                return response;
            }
            else if (objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) <= 0 && QtyToTransfer <= 0)
            {
                string approvedQuantityIsZeroMsg = (string.IsNullOrEmpty(MsgApprovedQuantityIsZero) || string.IsNullOrWhiteSpace(MsgApprovedQuantityIsZero))
                    ? ResourceRead.GetResourceValueByKeyAndFullFilePath("ApprovedQuantityIsZero", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode)
                    : MsgApprovedQuantityIsZero;

                response.IsSuccess = true;
                response.Message = approvedQuantityIsZeroMsg;
                return response;
            }

            //if (QtyToTransfer <= 0)
            //{
            //    response.IsSuccess = false;
            //    response.Message = "Total tranfered quantity must be less than approved quantity.";
            //    return response;
            //}
            var serialNumbers = new List<string>();
            var lotNumbers = new List<string>();
            var serialNumberTracking = false;
            var lotNumberTracking = false;

            if (objMoveInQty.ItemTransferDetails != null && objMoveInQty.ItemTransferDetails.Any())
            {
                serialNumbers = objMoveInQty.ItemTransferDetails.Where(e => Convert.ToString(e.ItemGUID) == (objMoveInQty.ItemGUID) && !string.IsNullOrEmpty(e.SerialNumber)).Select(d => d.SerialNumber).ToList();
                lotNumbers = objMoveInQty.ItemTransferDetails.Where(e => Convert.ToString(e.ItemGUID) == (objMoveInQty.ItemGUID) && !string.IsNullOrEmpty(e.LotNumber)).Select(d => d.LotNumber).ToList();
                serialNumberTracking = objMoveInQty.ItemTransferDetails.Where(e => Convert.ToString(e.ItemGUID) == (objMoveInQty.ItemGUID)).Select(d => d.SerialNumberTracking).FirstOrDefault();
                lotNumberTracking = objMoveInQty.ItemTransferDetails.Where(e => Convert.ToString(e.ItemGUID) == (objMoveInQty.ItemGUID)).Select(d => d.LotNumberTracking).FirstOrDefault();
            }

            IEnumerable<ItemLocationDetailsDTO> ItemLocations = objTrfDetailDTO.Bin.GetValueOrDefault(0) > 0
                ? objItemLocationDAL.GetItemLocationsByBinId(objTrfDetailDTO.Bin.Value,RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), orderByString, serialNumbers, lotNumbers)
                :objItemLocationDAL.GetAllRecordseVmi(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, orderByString, serialNumbers, lotNumbers);

            //if (objMoveInQty.IsSignleTransfer)
            //{
            if (!string.IsNullOrEmpty(objMoveInQty.BinWiseQty[0].LocationName))
            {
                Int64? BinID = binDAL.GetOrInsertBinIDByName(Guid.Parse(objMoveInQty.ItemGUID), objMoveInQty.BinWiseQty[0].LocationName, UserID, RoomID, CompanyID);
                ItemLocations = ItemLocations.Where(x => x.BinID == BinID.GetValueOrDefault(0));

                if (!ItemLocations.Any())
                {
                    ItemLocations = objItemLocationDAL.GetItemLocationsByBinId(BinID.Value, RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), orderByString, serialNumbers, lotNumbers);
                }
            }
            else if (objMoveInQty.ItemTransferDetails != null && objMoveInQty.ItemTransferDetails.Any())
            {
                if (!string.IsNullOrEmpty(objMoveInQty.ItemTransferDetails[0].BinNumber))
                {
                    Int64? BinID = binDAL.GetOrInsertBinIDByName(objMoveInQty.ItemTransferDetails[0].ItemGUID.Value, objMoveInQty.ItemTransferDetails[0].BinNumber, UserID, RoomID, CompanyID);
                    ItemLocations = ItemLocations.Where(x => x.BinID == BinID.GetValueOrDefault(0));

                    if (!ItemLocations.Any())
                    {
                        ItemLocations = objItemLocationDAL.GetItemLocationsByBinId(BinID.Value, RoomID, CompanyID, objMoveInQty.ItemTransferDetails[0].ItemGUID.Value, orderByString, serialNumbers, lotNumbers);
                    }
                }
            }
            //}

            double AvailableQty = ItemLocations.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0));
            string quantityNotAvailableToTransferMsg = (string.IsNullOrEmpty(MsgQuantityNotAvailableToTransfer) || string.IsNullOrWhiteSpace(MsgQuantityNotAvailableToTransfer))
                ? ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityNotAvailableToTransfer", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode)
                : MsgQuantityNotAvailableToTransfer;

            if (AvailableQty <= 0 && QtyToTransfer > 0)
            {
                response.IsSuccess = false;
                response.Message = quantityNotAvailableToTransferMsg;
                return response;
            }

            if (QtyToTransfer > AvailableQty)
                QtyToTransfer = AvailableQty;

            ItemLocations = ItemLocations.Where(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0)) >= 0 && (x.ConsignedQuantity.GetValueOrDefault(0)) >= 0);
            ItemLocations = ItemLocations.Where(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) > 0);

            List<ItemLocationDetailsDTO> ItemLocationsUpdated = null;
            List<TransferInOutItemDetailDTO> TransferOutItemDetailList = null;
            List<TransferInOutItemDetailDTO> TransferInItemDetailList = null;

            //ItemMasterDTO ItemDTO = itemDAL.GetItemWithoutJoins(null, Guid.Parse(objMoveInQty.ItemGUID));

            if (ItemLocations != null && ItemLocations.Count() > 0)
            {
                #region Deduction from ItemLocationDetails table

                ItemLocationsUpdated = new List<ItemLocationDetailsDTO>();
                TransferOutItemDetailList = new List<TransferInOutItemDetailDTO>();
                double QtyToBePulled = QtyToTransfer;

                foreach (var item in ItemLocations)
                {

                    if ((serialNumberTracking || lotNumberTracking))
                    {
                        double tmpQtyToTransfer = QtyToTransfer;
                        if (serialNumberTracking && serialNumbers.Any())
                        {
                            tmpQtyToTransfer = objMoveInQty.ItemTransferDetails.Where(e => Convert.ToString(e.ItemGUID) == (objMoveInQty.ItemGUID) && e.SerialNumber.Equals(item.SerialNumber)).Select(d => d.PullQuantity).FirstOrDefault();
                            var availableQty = ItemLocations.Where(e => e.SerialNumber.Equals(item.SerialNumber)).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0));
                            if (availableQty <= 0 && tmpQtyToTransfer > 0)
                            {
                                response.IsSuccess = false;
                                response.Message = quantityNotAvailableToTransferMsg;
                                return response;
                            }

                            if (tmpQtyToTransfer > AvailableQty)
                                tmpQtyToTransfer = availableQty;

                        }
                        if (lotNumberTracking && lotNumbers.Any())
                        {
                            tmpQtyToTransfer = objMoveInQty.ItemTransferDetails.Where(e => Convert.ToString(e.ItemGUID) == (objMoveInQty.ItemGUID) && e.LotNumber.Equals(item.LotNumber)).Select(d => d.PullQuantity).FirstOrDefault();
                            var availableQty = ItemLocations.Where(e => e.LotNumber.Equals(item.LotNumber)).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0));
                            if (availableQty <= 0 && tmpQtyToTransfer > 0)
                            {
                                response.IsSuccess = false;
                                response.Message = quantityNotAvailableToTransferMsg;
                                return response;
                            }

                            if (tmpQtyToTransfer > AvailableQty)
                                tmpQtyToTransfer = availableQty;
                        }

                        ItemLocationDetailsDTO NewItemLocation = GetNewItemLocation(item);

                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) >= tmpQtyToTransfer)
                        {
                            item.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0) - tmpQtyToTransfer;
                            NewItemLocation.CustomerOwnedQuantity = tmpQtyToTransfer;
                            QtyToBePulled -= tmpQtyToTransfer;
                            tmpQtyToTransfer = 0;
                        }
                        else
                        {
                            QtyToBePulled = QtyToBePulled - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            NewItemLocation.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            item.CustomerOwnedQuantity = 0;
                        }

                        if (tmpQtyToTransfer > 0)
                        {
                            if (item.ConsignedQuantity.GetValueOrDefault(0) >= tmpQtyToTransfer)
                            {
                                item.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) - tmpQtyToTransfer;
                                NewItemLocation.ConsignedQuantity = tmpQtyToTransfer;
                                QtyToBePulled -= tmpQtyToTransfer;
                                tmpQtyToTransfer = 0;
                            }
                            else
                            {
                                QtyToBePulled = QtyToBePulled - item.ConsignedQuantity.GetValueOrDefault(0);
                                NewItemLocation.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                item.ConsignedQuantity = 0;
                            }
                        }
                        item.TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID);
                        ItemLocationsUpdated.Add(NewItemLocation);

                        TransferInOutItemDetailDTO TransferInOutItemDetail = GetTransferOutItemDetailForOut(NewItemLocation, RoomID, UserID, Guid.Parse(objMoveInQty.DetailGUID));
                        TransferOutItemDetailList.Add(TransferInOutItemDetail);
                        objItemLocationDAL.Edit(item);

                        //if ((QtyToBePulled <= 0 || ItemLocationsUpdated.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) > QtyToBePulled))
                        if (QtyToBePulled <= 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        ItemLocationDetailsDTO NewItemLocation = GetNewItemLocation(item);

                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) >= QtyToBePulled)
                        {
                            item.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0) - QtyToBePulled;
                            NewItemLocation.CustomerOwnedQuantity = QtyToBePulled;
                            QtyToBePulled = 0;
                        }
                        else
                        {
                            QtyToBePulled = QtyToBePulled - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            NewItemLocation.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            item.CustomerOwnedQuantity = 0;
                        }

                        if (QtyToBePulled > 0)
                        {
                            if (item.ConsignedQuantity.GetValueOrDefault(0) >= QtyToBePulled)
                            {
                                item.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) - QtyToBePulled;
                                NewItemLocation.ConsignedQuantity = QtyToBePulled;
                                QtyToBePulled = 0;
                            }
                            else
                            {
                                QtyToBePulled = QtyToBePulled - item.ConsignedQuantity.GetValueOrDefault(0);
                                NewItemLocation.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                                item.ConsignedQuantity = 0;
                            }
                        }
                        item.TransferDetailGUID = Guid.Parse(objMoveInQty.DetailGUID);
                        ItemLocationsUpdated.Add(NewItemLocation);

                        TransferInOutItemDetailDTO TransferInOutItemDetail = GetTransferOutItemDetailForOut(NewItemLocation, RoomID, UserID, Guid.Parse(objMoveInQty.DetailGUID));
                        TransferOutItemDetailList.Add(TransferInOutItemDetail);
                        objItemLocationDAL.Edit(item);

                        if (QtyToBePulled <= 0 || ItemLocationsUpdated.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) >= QtyToTransfer)
                        {
                            break;
                        }
                    }

                }

                #endregion

                #region Insert into InOutQtyDetail for Out
                lstLocQty = itemLocQtyDAL.GetItemLocationQTYByItemGuidNormal(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID)).ToList();
                itemLocQtyDAL.Save(lstLocQty, SessionUserId,EnterpriseId);
                IEnumerable<Int64> BinIDs = ItemLocationsUpdated.Select(x => x.BinID.GetValueOrDefault(0)).Distinct();

                foreach (var binID in BinIDs)
                {
                    TransferInOutQtyDetailDTO transInOutQtyDetail = GetTransferInOutQtyDetailForOut(binID, objTrfDetailDTO, UserID);
                    transInOutQtyDetail.ConsignedQuantity = ItemLocationsUpdated.Where(x => x.BinID.GetValueOrDefault(0) == binID).Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                    transInOutQtyDetail.CustomerOwnedQuantity = ItemLocationsUpdated.Where(x => x.BinID.GetValueOrDefault(0) == binID).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    transInOutQtyDetail.TotalQuantity = transInOutQtyDetail.ConsignedQuantity.GetValueOrDefault(0) + transInOutQtyDetail.CustomerOwnedQuantity.GetValueOrDefault(0);
                    transInOutQtyDetail.TrfInOutItemDetail = TransferOutItemDetailList.Where(x => x.BinID.GetValueOrDefault(0) == binID).ToList();
                    Insert(transInOutQtyDetail);
                }
                #endregion

                TransferMasterDTO oMasterOut = objMasterDAL.GetTransferByGuidPlain(objTrfDetailDTO.TransferGUID);
                TransferMasterDTO oMasterIn = null;

                if (oMasterOut.RefTransferGUID.HasValue && oMasterOut.RefTransferGUID != Guid.Empty)
                    oMasterIn = objMasterDAL.GetTransferByGuidPlain(oMasterOut.RefTransferGUID.Value);
                else
                    oMasterIn = objMasterDAL.GetTransferByRefTransferGuidPlain(oMasterOut.ReplenishingRoomID, CompanyID, oMasterOut.GUID);

                #region update TransferInoutItemDetail and InOutQtyDetail for InRoom

                objTrfDetailDTO.IntransitQuantity = objTrfDetailDTO.IntransitQuantity.GetValueOrDefault(0) + QtyToTransfer;
                objTrfDetailDTO.ShippedQuantity = objTrfDetailDTO.ShippedQuantity.GetValueOrDefault(0) + QtyToTransfer;
                objTrfDetailDTO.FulFillQuantity = objTrfDetailDTO.FulFillQuantity.GetValueOrDefault(0) + QtyToTransfer;

                if (objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) <= 0)
                    objTrfDetailDTO.ApprovedQuantity = objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0) + objMoveInQty.TotalQty;

                objTrfDetailDTO.LastUpdatedBy = UserID;
                objTrfDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objTrfDetailDTO.EditedFrom = "Web";
                TransferDetailDAL.Edit(objTrfDetailDTO, SessionUserId,EnterpriseId);

                var replinishRoomItemGuid = GetSameItemByIDInReplinishRoom(new Guid(objMoveInQty.ItemGUID), oMasterIn.RoomID, CompanyID);
                var tmpsupplierIds = new List<long>();

                IEnumerable<TransferDetailDTO> lstTrfDetaiIN = TransferDetailDAL.GetTransferDetailsByTrfGuidAndItemGuidNormal(oMasterIn.GUID, replinishRoomItemGuid);

                if (lstTrfDetaiIN != null && lstTrfDetaiIN.Count() == 1)
                {
                    TransferDetailDTO oDetailIn = lstTrfDetaiIN.FirstOrDefault();// TransferDetailDAL.GetTransferedRecord(oMasterIn.GUID, oMasterIn.RoomID, CompanyID, 0).Where(x => x.ItemGUID == replinishRoomItemGuid).FirstOrDefault();
                    TransferInItemDetailList = GetTransferInItemDetailForIn(oDetailIn, TransferOutItemDetailList, UserID);

                    #region Update TransferDetail for In Room
                    oDetailIn.IntransitQuantity = oDetailIn.IntransitQuantity.GetValueOrDefault(0) + QtyToTransfer;
                    oDetailIn.ShippedQuantity = oDetailIn.ShippedQuantity.GetValueOrDefault(0) + QtyToTransfer;
                    oDetailIn.FulFillQuantity = oDetailIn.FulFillQuantity.GetValueOrDefault(0) + QtyToTransfer;

                    if (oDetailIn.ApprovedQuantity.GetValueOrDefault(0) <= 0)
                        oDetailIn.ApprovedQuantity = objTrfDetailDTO.ApprovedQuantity.GetValueOrDefault(0);

                    oDetailIn.LastUpdatedBy = UserID;
                    oDetailIn.ReceivedOn = DateTimeUtility.DateTimeNow;
                    oDetailIn.EditedFrom = "Web";

                    TransferInOutQtyDetailDTO objTQtyDetailIn = GetTransferInQtyDetailForIn(oDetailIn, TransferInItemDetailList, UserID);
                    Insert(objTQtyDetailIn);

                    TransferDetailDAL.Edit(oDetailIn, SessionUserId,EnterpriseId);
                    #endregion
                }
                else
                {
                    SaveTransferIn_SameItemWithDifferntBins(lstTrfDetaiIN, objTrfDetailDTO, TransferOutItemDetailList, QtyToTransfer, UserID, objMoveInQty.TotalQty, SessionUserId,EnterpriseId);
                }
                #endregion

                #region Update  TransferMasterDTO for In/Out Room

                if (oMasterIn.TransferStatus < (int)TransferStatus.FullFillQuantity)
                {
                    oMasterIn.TransferStatus = (int)TransferStatus.FullFillQuantity;
                    oMasterIn.ReceivedOn = DateTimeUtility.DateTimeNow;
                    oMasterIn.EditedFrom = "Web";
                    objMasterDAL.Edit(oMasterIn);
                }

                if (oMasterOut.TransferStatus < (int)TransferStatus.FullFillQuantity)
                {
                    oMasterOut.TransferStatus = (int)TransferStatus.FullFillQuantity;
                    oMasterOut.ReceivedOn = DateTimeUtility.DateTimeNow;
                    oMasterOut.EditedFrom = "Web";
                    objMasterDAL.Edit(oMasterOut);
                }

                #endregion
                string quantityTransferredMsg = (string.IsNullOrEmpty(MsgQuantityTransferred) || string.IsNullOrWhiteSpace(MsgQuantityTransferred))
                ? ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityTransferred", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode)
                : MsgQuantityTransferred;
                
                response.IsSuccess = true;
                response.Message = quantityTransferredMsg;

                #region WI-7595
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(RoomID,CompanyID, Guid.Parse(objMoveInQty.ItemGUID), UserID, SessionUserId, null, null);
                #endregion
            }

            return response;
        }

        private TransferInOutItemDetailDTO GetNewTrfInoutItemDetailDTO_For_IN(TransferDetailDTO oDetailIn, Int64 UserID, TransferInOutItemDetailDTO item, double Qty)
        {
            TransferInOutItemDetailDTO newItem = new TransferInOutItemDetailDTO()
            {
                CustomerOwnedQuantity = Qty,
                BinID = oDetailIn.Bin,
                CompanyID = oDetailIn.CompanyID,
                ItemGUID = oDetailIn.ItemGUID,
                Room = oDetailIn.Room,
                TransferDetailGUID = oDetailIn.GUID,

                CreatedBy = UserID,
                LastUpdatedBy = UserID,

                Cost = item.Cost,
                ExpirationDate = item.ExpirationDate,
                LotNumber = item.LotNumber,
                SerialNumber = item.SerialNumber,

                Created = DateTimeUtility.DateTimeNow,
                ReceivedDate = DateTimeUtility.DateTimeNow,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                TransferedDate = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,

                ItemLocationDetailGUID = null,
                TransferInOutQtyDetailGUID = null,
                ID = 0,
                GUID = Guid.Empty,

                ConsignedQuantity = 0,
                AddedFrom = "Web",
                EditedFrom = "Web",
                IsArchived = false,
                IsDeleted = false,
                IsTransfered = false,

                CreatedDate = string.Empty,
                ReceivedOnDate = string.Empty,
                ReceivedOnWebDate = string.Empty,
                UpdatedDate = string.Empty,
            };
            return newItem;
        }

        private void SaveTransferIn_SameItemWithDifferntBins(IEnumerable<TransferDetailDTO> lstTrfDetail_In, TransferDetailDTO objTrfDetail_Out,
                                            List<TransferInOutItemDetailDTO> trfInOutItemDetail_Out, double QtyToTransfer, Int64 UserID, double TotalQty, long SessionUserId,long EnterpriseId)
        {
            TransferDetailDAL transferDetailDAL = new TransferDetailDAL(base.DataBaseName);
            double qtyToReceive = QtyToTransfer;
            int cont = 1;
            int totalTDCount = lstTrfDetail_In.Count();
            foreach (var oDetailIn in lstTrfDetail_In)
            {

                if (qtyToReceive <= 0)
                {
                    break;
                }

                if (oDetailIn.ApprovedQuantity.GetValueOrDefault(0) <= 0)
                    oDetailIn.ApprovedQuantity = oDetailIn.RequestedQuantity;

                if (totalTDCount == cont)
                {
                    oDetailIn.ApprovedQuantity = trfInOutItemDetail_Out.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                }

                List<TransferInOutItemDetailDTO> trfInOutItemDetail_In = new List<TransferInOutItemDetailDTO>();
                double aleradyTransferedQty = oDetailIn.ReceivedQuantity.GetValueOrDefault(0) + oDetailIn.FulFillQuantity.GetValueOrDefault(0);
                double approvedQty = oDetailIn.ApprovedQuantity.GetValueOrDefault(0);
                double remainigQty = approvedQty - aleradyTransferedQty;

                if (remainigQty == 0)
                    continue;
                else
                    qtyToReceive = qtyToReceive - remainigQty;



                trfInOutItemDetail_Out = trfInOutItemDetail_Out.Where(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) > 0).ToList();

                foreach (var item in trfInOutItemDetail_Out)
                {
                    double receivedQty = trfInOutItemDetail_In.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0));
                    if (receivedQty == oDetailIn.ApprovedQuantity.GetValueOrDefault(0))
                        break;

                    double trsfredQty = item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                    if (trsfredQty <= remainigQty)
                    {
                        TransferInOutItemDetailDTO newItem = GetNewTrfInoutItemDetailDTO_For_IN(oDetailIn, UserID, item, trsfredQty);
                        item.CustomerOwnedQuantity = 0;
                        item.ConsignedQuantity = 0;
                        trfInOutItemDetail_In.Add(newItem);
                    }
                    else
                    {
                        TransferInOutItemDetailDTO newItem = GetNewTrfInoutItemDetailDTO_For_IN(oDetailIn, UserID, item, remainigQty);
                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                            item.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0) - remainigQty;
                        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                            item.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) - remainigQty;

                        trfInOutItemDetail_In.Add(newItem);
                    }
                }

                TransferInOutQtyDetailDTO objTQtyDetailIn = GetTransferInQtyDetailForIn(oDetailIn, trfInOutItemDetail_In, UserID);
                Insert(objTQtyDetailIn);

                #region Update TransferDetail for In Room               

                oDetailIn.IntransitQuantity = oDetailIn.IntransitQuantity.GetValueOrDefault(0) + objTQtyDetailIn.CustomerOwnedQuantity.GetValueOrDefault(0);
                oDetailIn.ShippedQuantity = oDetailIn.ShippedQuantity.GetValueOrDefault(0) + objTQtyDetailIn.CustomerOwnedQuantity.GetValueOrDefault(0);
                oDetailIn.FulFillQuantity = oDetailIn.FulFillQuantity.GetValueOrDefault(0) + objTQtyDetailIn.CustomerOwnedQuantity.GetValueOrDefault(0);

                oDetailIn.LastUpdatedBy = UserID;
                oDetailIn.ReceivedOn = DateTimeUtility.DateTimeNow;
                oDetailIn.EditedFrom = "Web";
                transferDetailDAL.Edit(oDetailIn, SessionUserId, EnterpriseId);
                #endregion
                cont += 1;
            }
        }

        private List<TransferInOutItemDetailDTO> GetTransferInItemDetailForIn(TransferDetailDTO oDetailIn, List<TransferInOutItemDetailDTO> TransferOutItemDetailList, Int64 UserID)
        {
            List<TransferInOutItemDetailDTO> TransferInItemDetailList = new List<TransferInOutItemDetailDTO>();
            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO itemDTO = null;
            foreach (var item in TransferOutItemDetailList)
            {

                TransferInOutItemDetailDTO TransferInItemDetailDTO = new TransferInOutItemDetailDTO()
                {
                    BinID = oDetailIn.Bin,
                    AddedFrom = "Web",
                    CompanyID = oDetailIn.CompanyID,

                    Cost = item.Cost,
                    Created = DateTimeUtility.DateTimeNow,
                    CreatedBy = UserID,
                    CreatedDate = string.Empty,
                    ////TODO: START WI-2705 As per issue comment on 23-Mar-2017 Receive alway customerowend
                    //ConsignedQuantity = 0,
                    //CustomerOwnedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0),
                    ////TODO: END WI-2705 As per issue comment on 23-Mar-2017 Receive alway customerowend

                    //TODO: START as per discuss on Skype with Rock on 25-Jan-2018 Receive as per Item Type
                    //ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0),
                    //CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0),
                    //TODO: END as per discuss on Skype with Rock on 25-Jan-2018 Receive as per Item Type

                    EditedFrom = "Web",
                    ExpirationDate = item.ExpirationDate,
                    GUID = Guid.NewGuid(),
                    ID = 0,
                    IsArchived = false,
                    IsDeleted = false,
                    IsTransfered = false,
                    ItemGUID = oDetailIn.ItemGUID,
                    ItemLocationDetailGUID = null,
                    LastUpdatedBy = UserID,
                    LotNumber = item.LotNumber,
                    ReceivedDate = item.ReceivedDate,
                    ReceivedOn = DateTimeUtility.DateTimeNow,
                    ReceivedOnDate = string.Empty,
                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    ReceivedOnWebDate = string.Empty,
                    Room = oDetailIn.Room,
                    SerialNumber = item.SerialNumber,
                    TransferDetailGUID = oDetailIn.GUID,
                    TransferedDate = DateTimeUtility.DateTimeNow,
                    TransferInOutQtyDetailGUID = Guid.Empty,
                    Updated = DateTimeUtility.DateTimeNow,
                    UpdatedDate = string.Empty,
                };
                itemDTO = itemDAL.GetItemByGuidPlain(oDetailIn.ItemGUID, oDetailIn.Room, oDetailIn.CompanyID);
                if (!itemDTO.Consignment)
                {
                    TransferInItemDetailDTO.ConsignedQuantity = 0;
                    TransferInItemDetailDTO.CustomerOwnedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                }
                else
                {
                    TransferInItemDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    TransferInItemDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                }

                if (itemDTO != null && itemDTO.ID > 0)
                {
                    TransferInItemDetailDTO.Cost = itemDTO.Cost;
                }

                TransferInItemDetailList.Add(TransferInItemDetailDTO);
            }

            return TransferInItemDetailList;
        }
        private TransferInOutQtyDetailDTO GetTransferInOutQtyDetailForOut(Int64 BinID, TransferDetailDTO transferDetailDTO, Int64 UserID)
        {
            TransferInOutQtyDetailDTO TranserInOutQtyDetail = new TransferInOutQtyDetailDTO()
            {
                BinID = BinID,
                TransferDetailGUID = transferDetailDTO.GUID,
                TransferInOut = "OUT",
                ItemGUID = transferDetailDTO.ItemGUID,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                CompanyID = transferDetailDTO.CompanyID,
                Room = transferDetailDTO.Room,

                IsArchived = false,
                IsDeleted = false,

                //ConsignedQuantity = ItemLocationsUpdated.Where(y.,
                //CustomerOwnedQuantity = CustomerQty,
                //TotalQuantity = Quantity,
                GUID = Guid.NewGuid(),
                ID = 0,
                //TrfInOutItemDetail = objTItemDetailList,
                TransferGUID = transferDetailDTO.TransferGUID,
                AddedFrom = "Web",
                BinNumber = string.Empty,
                CreatedDate = string.Empty,
                EditedFrom = "Web",
                IsTransfered = false,
                ItemNumber = transferDetailDTO.ItemNumber,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnDate = string.Empty,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                ReceivedOnWebDate = string.Empty,
                UpdatedDate = string.Empty,
            };

            return TranserInOutQtyDetail;
        }
        private TransferInOutQtyDetailDTO GetTransferInQtyDetailForIn(TransferDetailDTO oDetailIn, List<TransferInOutItemDetailDTO> TransferInItemDetailList, Int64 UserID)
        {
            double CosigneQty = TransferInItemDetailList.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
            double CustomerQty = TransferInItemDetailList.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO itemDTO = null;

            TransferInOutQtyDetailDTO TranserInOutQtyDetail = new TransferInOutQtyDetailDTO()
            {
                BinID = oDetailIn.Bin,
                TransferDetailGUID = oDetailIn.GUID,
                TransferInOut = "IN",
                ItemGUID = oDetailIn.ItemGUID,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                CompanyID = oDetailIn.CompanyID,
                Room = oDetailIn.Room,

                IsArchived = false,
                IsDeleted = false,
                ////TODO: START WI-2705 As per issue comment on 23-Mar-2017 Receive alway customerowend
                //ConsignedQuantity = 0,
                //CustomerOwnedQuantity = (CosigneQty + CustomerQty),
                ////TODO: END WI-2705 As per issue comment on 23-Mar-2017 Receive alway customerowend

                //TODO: START as per discuss on Skype with Rock on 25-Jan-2018 Receive as per Item Type
                // ConsignedQuantity = CosigneQty,
                // CustomerOwnedQuantity = CustomerQty,
                //TODO: END as per discuss on Skype with Rock on 25-Jan-2018 Receive as per Item Type


                TotalQuantity = (CosigneQty + CustomerQty) > oDetailIn.ApprovedQuantity.GetValueOrDefault(0) ? oDetailIn.ApprovedQuantity.GetValueOrDefault(0) : (CosigneQty + CustomerQty),
                GUID = Guid.NewGuid(),
                ID = 0,
                TrfInOutItemDetail = TransferInItemDetailList,
                TransferGUID = oDetailIn.TransferGUID,
                AddedFrom = "Web",
                BinNumber = oDetailIn.BinName,
                CreatedDate = string.Empty,
                EditedFrom = "Web",
                IsTransfered = false,
                ItemNumber = oDetailIn.ItemNumber,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnDate = string.Empty,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                ReceivedOnWebDate = string.Empty,
                UpdatedDate = string.Empty,
            };

            itemDTO = itemDAL.GetItemByGuidPlain(oDetailIn.ItemGUID, oDetailIn.Room, oDetailIn.CompanyID);
            if (!itemDTO.Consignment)
            {
                TranserInOutQtyDetail.ConsignedQuantity = 0;
                TranserInOutQtyDetail.CustomerOwnedQuantity = CosigneQty + CustomerQty;
            }
            else
            {
                TranserInOutQtyDetail.ConsignedQuantity = CosigneQty;
                TranserInOutQtyDetail.CustomerOwnedQuantity = CustomerQty;
            }

            return TranserInOutQtyDetail;
        }
        private ItemLocationDetailsDTO GetNewItemLocation(ItemLocationDetailsDTO item)
        {
            ItemLocationDetailsDTO obj = new ItemLocationDetailsDTO()
            {
                Action = item.Action,
                //BinNumber = item.BinNumber,
                BinID = item.BinID,
                AddedFrom = item.AddedFrom,
                CompanyID = item.CompanyID,
                ConsignedQuantity = item.ConsignedQuantity,
                Cost = item.Cost,
                CountCustOrConsQty = item.CountCustOrConsQty,
                CountLineItemDtlGUID = item.CountLineItemDtlGUID,
                Created = item.Created,
                CreatedBy = item.CreatedBy,
                //CreatedByName = item.CreatedByName,
                CreatedDate = item.CreatedDate,
                //CriticalQuantity = item.CriticalQuantity,
                CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                //DateCodeTracking = item.DateCodeTracking,
                EditedFrom = item.EditedFrom,
                eVMISensorID = item.eVMISensorID,
                eVMISensorIDdbl = item.eVMISensorIDdbl,
                eVMISensorPort = item.eVMISensorPort,
                eVMISensorPortstr = item.eVMISensorPortstr,
                Expiration = item.Expiration,
                ExpirationDate = item.ExpirationDate,
                GUID = item.GUID,
                HistoryID = item.HistoryID,
                ID = item.ID,
                InitialQuantity = item.InitialQuantity,
                InitialQuantityPDA = item.InitialQuantityPDA,
                InitialQuantityWeb = item.InitialQuantityWeb,
                InsertedFrom = item.InsertedFrom,
                IsArchived = item.IsArchived,
                IsConsignedSerialLot = item.IsConsignedSerialLot,
                IsCreditPull = item.IsCreditPull,
                IsDefault = item.IsDefault,
                IsDeleted = item.IsDeleted,
                IsItemLevelMinMax = item.IsItemLevelMinMax,
                IsOnlyFromUI = item.IsOnlyFromUI,
                IsPDAEdit = item.IsPDAEdit,
                IsWebEdit = item.IsWebEdit,
                ItemGUID = item.ItemGUID,
                //ItemNumber = item.ItemNumber,
                //ItemType = item.ItemType,
                KitDetailGUID = item.KitDetailGUID,
                LastUpdatedBy = item.LastUpdatedBy,
                LotNumber = item.LotNumber,
                MeasurementID = item.MeasurementID,
                ProjectSpentGUID = item.ProjectSpentGUID,
                //LotNumberTracking = item.LotNumberTracking,
                mode = item.mode,
                Markup = item.Markup,
                //MaximumQuantity = item.MaximumQuantity,
                //MinimumQuantity = item.MinimumQuantity,
                MoveQuantity = item.MoveQuantity,
                OrderDetailGUID = item.OrderDetailGUID,
                PackSlipNumber = item.PackSlipNumber,
                PullOrderNumber = item.PullOrderNumber,
                Received = item.Received,
                ReceivedDate = item.ReceivedDate,
                ReceivedOn = item.ReceivedOn,
                ReceivedOnDate = item.ReceivedOnDate,
                ReceivedOnDateWeb = item.ReceivedOnDateWeb,
                ReceivedOnWeb = item.ReceivedOnWeb,
                RefPDASelfGUID = item.RefPDASelfGUID,
                RefWebSelfGUID = item.RefWebSelfGUID,
                Room = item.Room,
                //RoomName = item.RoomName,
                SellPrice = item.SellPrice,
                SerialNumber = item.SerialNumber,
                //SerialNumberTracking = item.SerialNumberTracking,
                SuggestedOrderQuantity = item.SuggestedOrderQuantity,
                TransferDetailGUID = item.TransferDetailGUID,
                UDF1 = item.UDF1,
                UDF10 = item.UDF10,
                UDF3 = item.UDF3,
                UDF2 = item.UDF2,
                UDF4 = item.UDF4,
                UDF5 = item.UDF5,
                UDF6 = item.UDF6,
                UDF7 = item.UDF7,
                UDF8 = item.UDF8,
                UDF9 = item.UDF9,
                Updated = item.Updated,
                //UpdatedByName = item.UpdatedByName,
                UpdatedDate = item.UpdatedDate,
                WorkOrderGUID = item.WorkOrderGUID,
            };

            return obj;
        }
        private TransferInOutItemDetailDTO GetTransferOutItemDetailForOut(ItemLocationDetailsDTO NewItemLocation, Int64 RoomID, Int64 UserID, Guid TransferDetailGuid)
        {
            TransferInOutItemDetailDTO objTItemDetail = new TransferInOutItemDetailDTO()
            {
                BinID = NewItemLocation.BinID.GetValueOrDefault(0),
                CompanyID = NewItemLocation.CompanyID.GetValueOrDefault(0),
                Room = RoomID,
                TransferInOutQtyDetailGUID = Guid.Empty,
                TransferedDate = DateTimeUtility.DateTimeNow,
                SerialNumber = NewItemLocation.SerialNumber,
                Cost = NewItemLocation.Cost,
                ReceivedDate = NewItemLocation.ReceivedDate,
                LotNumber = NewItemLocation.LotNumber,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = UserID,
                ExpirationDate = NewItemLocation.ExpirationDate,
                GUID = Guid.NewGuid(),
                ID = 0,
                IsArchived = false,
                IsDeleted = false,
                ItemGUID = NewItemLocation.ItemGUID,
                TransferDetailGUID = TransferDetailGuid,
                ItemLocationDetailGUID = NewItemLocation.GUID,
                AddedFrom = "Web",
                ConsignedQuantity = NewItemLocation.ConsignedQuantity,
                CreatedDate = string.Empty,
                CustomerOwnedQuantity = NewItemLocation.CustomerOwnedQuantity,
                EditedFrom = "Web",
                IsTransfered = false,
                LastUpdatedBy = UserID,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnDate = string.Empty,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                ReceivedOnWebDate = string.Empty,
                Updated = DateTimeUtility.DateTimeNow,
                UpdatedDate = string.Empty,
                ProcessedConsignedQuantity = NewItemLocation.ConsignedQuantity,
                ProcessedCustomerOwnedQuantity = NewItemLocation.CustomerOwnedQuantity,
            };
            return objTItemDetail;
        }
        /// <summary>
        /// GetSameItemByIDInReplinishRoom
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="RoomIDFrom"></param>
        /// <param name="RoomIDTO"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public Guid GetSameItemByIDInReplinishRoom(Guid ItemGUID, Int64 RoomIDTO, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@CompnayID", CompanyID),
                                                    new SqlParameter("@ReplenishRoomID", RoomIDTO),                                                    
                                                    new SqlParameter("@ItemGUID", ItemGUID)
                                                };
                return context.Database.SqlQuery<Guid>("exec [GetItemGuidOfReplenishRoom] @CompnayID,@ReplenishRoomID,@ItemGUID ", params1).FirstOrDefault();
            }
        }

        private ItemLocationDetailsDTO GetItemLocationDetailFromInOutItemDetailForIn(TransferInOutItemDetailDTO item, BinMasterDTO binDTO, Int64 UserID, double QtyToReceive, double ProcessedQtyPerRow)
        {
            ItemLocationDetailsDTO objItemLocDetailDTO = new ItemLocationDetailsDTO()
            {
                GUID = Guid.NewGuid(),
                ID = 0,
                ItemGUID = item.ItemGUID,
                BinID = binDTO.ID,
                CompanyID = item.CompanyID,
                Room = item.Room,
                BinNumber = binDTO.BinNumber,

                Cost = item.Cost,

                TransferDetailGUID = item.TransferDetailGUID,
                ReceivedDate = item.ReceivedDate != null ? item.ReceivedDate : DateTimeUtility.DateTimeNow,


                LotNumber = item.LotNumber,
                SerialNumber = item.SerialNumber,
                ExpirationDate = item.ExpirationDate,

                Expiration = item.ExpirationDate != null ? item.ExpirationDate.Value.ToString("M/d/yyyy") : string.Empty, //?
                Received = item.ReceivedDate.GetValueOrDefault(DateTimeUtility.DateTimeNow).ToString("M/d/yyyy"),

                Action = "Transfer-Receive",
                AddedFrom = "Web",
                EditedFrom = "Web",
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                InsertedFrom = "Transfer-Receive",
                CustomerOwnedQuantity = item.ProcessedCustomerOwnedQuantity ?? 0,
                ConsignedQuantity = item.ProcessedConsignedQuantity ?? 0,
                IsArchived = false,
                IsCreditPull = false,
                IsDeleted = false,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = UserID,
                LastUpdatedBy = UserID,

            };

            /*
            if (item.CustomerOwnedQuantity.GetValueOrDefault(0) >= QtyToReceive)
                objItemLocDetailDTO.CustomerOwnedQuantity = QtyToReceive;
            else if (item.ConsignedQuantity.GetValueOrDefault(0) >= QtyToReceive)
                objItemLocDetailDTO.ConsignedQuantity = QtyToReceive;
            else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                objItemLocDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
            else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                objItemLocDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                */

            //if ((item.ProcessedCustomerOwnedQuantity.GetValueOrDefault(0) - PreviouseProcCustOwnQty) >= QtyToReceive)
            //    objItemLocDetailDTO.CustomerOwnedQuantity = QtyToReceive;
            //else if ((item.ProcessedConsignedQuantity.GetValueOrDefault(0) - PreviouseProcCongQty) >= QtyToReceive)
            //    objItemLocDetailDTO.ConsignedQuantity = QtyToReceive;
            //else if ((item.ProcessedCustomerOwnedQuantity.GetValueOrDefault(0) - PreviouseProcCustOwnQty) > 0)
            //    objItemLocDetailDTO.CustomerOwnedQuantity = (item.ProcessedCustomerOwnedQuantity.GetValueOrDefault(0) - PreviouseProcCustOwnQty);
            //else if ((item.ProcessedConsignedQuantity.GetValueOrDefault(0) - PreviouseProcCongQty) > 0)
            //    objItemLocDetailDTO.ConsignedQuantity = (item.ProcessedConsignedQuantity.GetValueOrDefault(0) - PreviouseProcCongQty);


            if ((item.ProcessedConsignedQuantity ?? 0) > 0)
            {
                objItemLocDetailDTO.ConsignedQuantity = ProcessedQtyPerRow;
                objItemLocDetailDTO.CustomerOwnedQuantity = 0;
            }
            if ((item.ProcessedCustomerOwnedQuantity ?? 0) > 0)
            {
                objItemLocDetailDTO.CustomerOwnedQuantity = ProcessedQtyPerRow;
                objItemLocDetailDTO.ConsignedQuantity = 0;
            }
            return objItemLocDetailDTO;
        }
        private MaterialStagingPullDetailDTO GEtMSPullDetailFromInOutItemDetailForIn(TransferInOutItemDetailDTO item, BinMasterDTO binDTO, Int64 UserID, MaterialStagingDTO objMSDOT, double QtyToReceive,string RoomDateFormat)
        {
            MaterialStagingPullDetailDTO objItemMSPullDetailDTO = new MaterialStagingPullDetailDTO()
            {
                BinID = binDTO.ID,
                CompanyID = item.CompanyID,
                BinNumber = binDTO.BinNumber,
                //ConsignedQuantity = item.ConsignedQuantity,
                ItemCost = item.Cost,
                Expiration = item.ExpirationDate != null ? item.ExpirationDate.Value.ToString("M/d/yyyy") : string.Empty, //?
                Received = item.ReceivedDate.GetValueOrDefault(DateTimeUtility.DateTimeNow).ToString(RoomDateFormat),
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = UserID,
                //CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                GUID = Guid.NewGuid(),
                ID = 0,
                IsArchived = false,
                IsDeleted = false,
                ItemGUID = item.ItemGUID.GetValueOrDefault(Guid.Empty),
                TransferDetailGUID = item.TransferDetailGUID,
                LastUpdatedBy = UserID,
                LotNumber = item.LotNumber,
                Room = item.Room,
                SerialNumber = item.SerialNumber,
                Updated = DateTime.Now,
                UpdatedByName = string.Empty,
                MaterialStagingGUID = objMSDOT.GUID,

            };
            if (item.CustomerOwnedQuantity.GetValueOrDefault(0) >= QtyToReceive)
                objItemMSPullDetailDTO.CustomerOwnedQuantity = QtyToReceive;
            else if (item.ConsignedQuantity.GetValueOrDefault(0) >= QtyToReceive)
                objItemMSPullDetailDTO.ConsignedQuantity = QtyToReceive;
            else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                objItemMSPullDetailDTO.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
            else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                objItemMSPullDetailDTO.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);

            return objItemMSPullDetailDTO;
        }
        /// <summary>
        /// DecreaseQuantity
        /// </summary>
        /// <param name="objMoveInQty"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>

        public ResponseMessage ReceiveTransferedQuantity(Guid ItemGUID, Guid DetailGUID, Int64 LocationID, Int64 RoomID, Int64 CompanyID, Int64 UserID, List<long> SupplierIds, double QtyToReceive, string RoomDateFormat, long SessionUserId, long EnterpriseId, string EditedFrom = "Web", List<ItemLocationLotSerialDTO> ItemLocationDetails = null)
        {
            ResponseMessage response = new ResponseMessage();
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            TransferDetailDAL objDetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferInOutItemDetailDAL InOutItemDetaiDAL = new TransferInOutItemDetailDAL(base.DataBaseName);
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemLocationDetailsDAL objItemLocDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            MaterialStagingDAL MSDAL = new MaterialStagingDAL(base.DataBaseName);
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            TransferDetailDTO objDetailDTO = objDetailDAL.GetTransferDetailsByGuidPlain(DetailGUID);
            PullTransactionDAL pullTransactionDAL = new PullTransactionDAL(base.DataBaseName);
            var cultureCode = "en-US";
            var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
            var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            if (regionInfo != null)
            {
                cultureCode = regionInfo.CultureCode;
            }
            var transferResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", cultureCode, EnterpriseId, CompanyID);
            var receiveResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", cultureCode, EnterpriseId, CompanyID);

            if (QtyToReceive <= 0)
            {
                string msgEnterQuantityReceive = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgEnterQuantityReceive", receiveResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResReceiveOrderDetails", cultureCode);
                response.IsSuccess = false;
                response.Message = msgEnterQuantityReceive;
                return response;
            }

            if (objDetailDTO.FulFillQuantity.GetValueOrDefault(0) - objDetailDTO.ReceivedQuantity.GetValueOrDefault(0) <= 0)
            {
                string msgNotEnoughQuantityToReceive = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityToReceive", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
                response.IsSuccess = false; 
                response.Message = msgNotEnoughQuantityToReceive;
                return response;
            }

            if (QtyToReceive > objDetailDTO.FulFillQuantity.GetValueOrDefault(0) - objDetailDTO.ReceivedQuantity.GetValueOrDefault(0))
            {
                string msgReceivedQtyIsNotMoreThanTransferredQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReceivedQtyIsNotMoreThanTransferredQty", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
                response.IsSuccess = false;
                response.Message = msgReceivedQtyIsNotMoreThanTransferredQty;
                return response;
            }

            TransferMasterDTO objMasterDTO = objMasterDAL.GetTransferByGuidPlain(objDetailDTO.TransferGUID);
            //ItemMasterDTO objItem = objItemDAL.GetItemWithoutJoins(null, ItemGUID);
            bool isFromLotSerialSelectionPopup = (ItemLocationDetails != null && ItemLocationDetails.Any());
            var serialNumbers = new List<string>();
            var lotNumbers = new List<string>();

            if (isFromLotSerialSelectionPopup)
            {
                serialNumbers = ItemLocationDetails.Where(e => e.ItemGUID == ItemGUID && e.SerialNumberTracking && !string.IsNullOrEmpty(e.SerialNumber)).Select(d => d.SerialNumber).ToList();
                lotNumbers = ItemLocationDetails.Where(e => e.ItemGUID == ItemGUID && e.LotNumberTracking && !string.IsNullOrEmpty(e.LotNumber)).Select(d => d.LotNumber).ToList();
            }

            MaterialStagingDTO objMSDOT = null;
            if (objMasterDTO.StagingID.GetValueOrDefault(0) > 0)
            {
                objMSDOT = MSDAL.GetMaterialStagingByIdPlain(objMasterDTO.StagingID.GetValueOrDefault(0));                
            }

            IEnumerable<TransferInOutQtyDetailDTO> lstInQtyDetail;

            if (isFromLotSerialSelectionPopup)
            {
                lstInQtyDetail = GetTransferInOutQtyDetailByTransferDetailAndItem(objDetailDTO.GUID, ItemGUID, CompanyID, serialNumbers, lotNumbers);
            }
            else
            {
                lstInQtyDetail = GetTrfInOutQtyDetailByTrfDetailAndItem(objDetailDTO.GUID, CompanyID, ItemGUID);
            }

            List<TransferInOutQtyDetailDTO> lstInQtyFiltered = new List<TransferInOutQtyDetailDTO>();
            BinMasterDTO oBinMasterDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(LocationID, RoomID, CompanyID);
            //BinMasterDTO oBinMasterDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, LocationID,null,null).FirstOrDefault();
            string BinNumber = string.Empty;
            if (oBinMasterDTO != null)
                BinNumber = oBinMasterDTO.BinNumber;

            TransferMasterDTO objReplenishMasterDTO = null;
            if (objMasterDTO.RefTransferGUID.HasValue && objMasterDTO.RefTransferGUID != Guid.Empty)
                objReplenishMasterDTO = objMasterDAL.GetTransferByGuidPlain(objMasterDTO.RefTransferGUID.Value);
            else
                objReplenishMasterDTO = objMasterDAL.GetTransferByRefTransferGuidPlain(objMasterDTO.ReplenishingRoomID, CompanyID, objMasterDTO.GUID);

            List<TransferDetailDTO> ReplinishDetailDTOList = objDetailDAL.GetTransferDetailNormal(objReplenishMasterDTO.GUID, objReplenishMasterDTO.RoomID, CompanyID, SupplierIds);
            double qtyReciveTemp = 0;
            double remainingQty = QtyToReceive;


            foreach (var InOutQtyitem in lstInQtyDetail)
            {
                if (qtyReciveTemp >= QtyToReceive)
                    break;

                var objInItemDetailDTOList = new List<TransferInOutItemDetailDTO>();

                if (isFromLotSerialSelectionPopup)
                {
                    objInItemDetailDTOList = InOutItemDetaiDAL.GetTrfInOutItemDetailByInOutQtyDetailGuidAndLotSr(InOutQtyitem.GUID, CompanyID, serialNumbers, lotNumbers).ToList();
                }
                else
                {
                    objInItemDetailDTOList = InOutItemDetaiDAL.GetTrfInOutItemDetailByInOutQtyDetailGuid(InOutQtyitem.GUID).ToList();
                }

                double TempQtyToReceive = QtyToReceive;
                double ProcessedQtyPerRow = 0;
                double TotalReceivedQty = 0;
                double AvailableQtyForCurrentRecord = 0;
                double CustAvail = 0;
                double ConsignedAvail = 0;

                // This code block is used to handle the case when user try to receive items after performing close transfer functionality 
                // and close transfer is not completed due to serial number duplication
                if (objInItemDetailDTOList == null || !objInItemDetailDTOList.Any())
                {
                    continue;
                }

                if (InOutQtyitem.TotalQuantity <= remainingQty)
                {
                    qtyReciveTemp += InOutQtyitem.TotalQuantity;
                    InOutQtyitem.IsTransfered = true;
                    remainingQty = remainingQty - qtyReciveTemp;
                }
                else
                {
                    qtyReciveTemp += remainingQty;
                    InOutQtyitem.TotalQuantity = remainingQty;
                    remainingQty = QtyToReceive - qtyReciveTemp;
                }

                foreach (var item in objInItemDetailDTOList)
                {
                    if (TempQtyToReceive > 0)
                    {
                        double tempQuantityToTransferForLotSr = 0;

                        if (isFromLotSerialSelectionPopup && (!string.IsNullOrEmpty(item.SerialNumber) || !string.IsNullOrEmpty(item.LotNumber)))
                        {
                            var itemLocation = ItemLocationDetails.Where(e => e.ItemGUID == item.ItemGUID).ToList();

                            if (itemLocation != null && itemLocation.Any())
                            {
                                if (itemLocation.FirstOrDefault().SerialNumberTracking)
                                {
                                    var serialItem = itemLocation.Where(e => e.SerialNumber == item.SerialNumber).FirstOrDefault();

                                    if (serialItem != null && serialItem.PullQuantity > 0)
                                    {
                                        var serialItemQuantity = objItemLocationDetailsDAL.GetCountForSerialItem(item.ItemGUID.Value, RoomID, CompanyID, item.SerialNumber);

                                        if (serialItemQuantity.HasValue && serialItemQuantity >= 1) // This is to prevent user from receiving duplicate serial number item.  
                                        {
                                            string msgDuplicateSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDuplicateSerialNumber", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
                                            response.IsSuccess = false;
                                            response.Message += "<br>" + string.Format(msgDuplicateSerialNumber, item.SerialNumber);
                                            InOutQtyitem.TotalQuantity -= serialItem.PullQuantity;
                                            qtyReciveTemp -= serialItem.PullQuantity;
                                            InOutQtyitem.IsTransfered = InOutQtyitem.IsTransfered ? false : InOutQtyitem.IsTransfered;
                                            remainingQty += serialItem.PullQuantity;
                                            continue;
                                        }
                                        tempQuantityToTransferForLotSr = serialItem.PullQuantity;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                if (itemLocation.FirstOrDefault().LotNumberTracking)
                                {
                                    var lotItem = itemLocation.Where(e => e.LotNumber == item.LotNumber).FirstOrDefault();

                                    if (lotItem != null && lotItem.PullQuantity > 0)
                                    {
                                        // Below given code block is to maintain unique Lot + DateExpiration combination.
                                        if (lotItem.DateCodeTracking && item.ExpirationDate.HasValue)
                                        {
                                            var isLotWithDifferentExpiredDateNotExist = pullTransactionDAL.ValidateLotDateCodeForCredit(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.LotNumber, item.ExpirationDate.Value, CompanyID, RoomID);

                                            if (!isLotWithDifferentExpiredDateNotExist)
                                            {
                                                string msgLotPlusExpirationDateNotMatched = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgLotPlusExpirationDateNotMatched", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
                                                response.IsSuccess = false;
                                                response.Message += "<br>" + string.Format(msgLotPlusExpirationDateNotMatched, item.LotNumber);
                                                InOutQtyitem.TotalQuantity -= lotItem.PullQuantity;
                                                qtyReciveTemp -= lotItem.PullQuantity;
                                                InOutQtyitem.IsTransfered = InOutQtyitem.IsTransfered ? false : InOutQtyitem.IsTransfered;
                                                remainingQty += lotItem.PullQuantity;
                                                continue;
                                            }
                                        }
                                        tempQuantityToTransferForLotSr = lotItem.PullQuantity;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }

                            CustAvail = (item.CustomerOwnedQuantity ?? 0) - (item.ProcessedCustomerOwnedQuantity ?? 0);
                            ConsignedAvail = (item.ConsignedQuantity ?? 0) - (item.ProcessedConsignedQuantity ?? 0);
                            AvailableQtyForCurrentRecord = CustAvail + ConsignedAvail;

                            if (AvailableQtyForCurrentRecord > 0)
                            {
                                if (tempQuantityToTransferForLotSr < AvailableQtyForCurrentRecord)
                                {
                                    //Update current record here
                                    ProcessedQtyPerRow = tempQuantityToTransferForLotSr;

                                    if (CustAvail > 0)
                                    {
                                        item.ProcessedCustomerOwnedQuantity = (item.ProcessedCustomerOwnedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    if (ConsignedAvail > 0)
                                    {
                                        item.ProcessedConsignedQuantity = (item.ProcessedConsignedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    TotalReceivedQty = TotalReceivedQty + tempQuantityToTransferForLotSr;
                                    TempQtyToReceive = TempQtyToReceive - tempQuantityToTransferForLotSr;
                                    // update Process qty
                                }
                                else
                                {
                                    //Update current record here
                                    TempQtyToReceive = TempQtyToReceive - AvailableQtyForCurrentRecord;
                                    ProcessedQtyPerRow = AvailableQtyForCurrentRecord;

                                    if (CustAvail > 0)
                                    {
                                        item.ProcessedCustomerOwnedQuantity = (item.ProcessedCustomerOwnedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    if (ConsignedAvail > 0)
                                    {
                                        item.ProcessedConsignedQuantity = (item.ProcessedConsignedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    TotalReceivedQty = TotalReceivedQty + AvailableQtyForCurrentRecord;
                                    // update Process qty
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            CustAvail = (item.CustomerOwnedQuantity ?? 0) - (item.ProcessedCustomerOwnedQuantity ?? 0);
                            ConsignedAvail = (item.ConsignedQuantity ?? 0) - (item.ProcessedConsignedQuantity ?? 0);
                            AvailableQtyForCurrentRecord = CustAvail + ConsignedAvail;

                            if (AvailableQtyForCurrentRecord > 0)
                            {
                                if (!string.IsNullOrEmpty(item.SerialNumber))
                                {
                                    var serialItemQuantity = objItemLocationDetailsDAL.GetCountForSerialItem(item.ItemGUID.Value, RoomID, CompanyID, item.SerialNumber);

                                    if (serialItemQuantity.HasValue && serialItemQuantity >= 1) // This code block is to prevent user from receiving duplicate serial number item.  
                                    {
                                        string msgDuplicateSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDuplicateSerialNumber", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
                                        response.IsSuccess = false;
                                        response.Message += "<br>" + string.Format(msgDuplicateSerialNumber, item.SerialNumber);
                                        InOutQtyitem.TotalQuantity -= 1;
                                        qtyReciveTemp -= 1;
                                        InOutQtyitem.IsTransfered = InOutQtyitem.IsTransfered ? false : InOutQtyitem.IsTransfered;
                                        remainingQty += 1;
                                        continue;
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.LotNumber) && item.ExpirationDate.HasValue)
                                {
                                    var isLotWithDifferentExpiredDateNotExist = pullTransactionDAL.ValidateLotDateCodeForCredit(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.LotNumber, item.ExpirationDate.Value, CompanyID, RoomID);

                                    if (!isLotWithDifferentExpiredDateNotExist)
                                    {
                                        var qtyToAdjust = (TempQtyToReceive < AvailableQtyForCurrentRecord)
                                                          ? TempQtyToReceive
                                                          : AvailableQtyForCurrentRecord;

                                        string msgLotPlusExpirationDateNotMatched = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgLotPlusExpirationDateNotMatched", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
                                        response.IsSuccess = false;
                                        response.Message += "<br>" + string.Format(msgLotPlusExpirationDateNotMatched, item.LotNumber);
                                        InOutQtyitem.TotalQuantity -= qtyToAdjust;
                                        qtyReciveTemp -= qtyToAdjust;
                                        InOutQtyitem.IsTransfered = InOutQtyitem.IsTransfered ? false : InOutQtyitem.IsTransfered;
                                        remainingQty += qtyToAdjust;
                                        continue;
                                    }
                                }

                                if (TempQtyToReceive < AvailableQtyForCurrentRecord)
                                {
                                    //Update current record here
                                    ProcessedQtyPerRow = TempQtyToReceive;

                                    if (CustAvail > 0)
                                    {
                                        item.ProcessedCustomerOwnedQuantity = (item.ProcessedCustomerOwnedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    if (ConsignedAvail > 0)
                                    {
                                        item.ProcessedConsignedQuantity = (item.ProcessedConsignedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    TotalReceivedQty = TotalReceivedQty + TempQtyToReceive;
                                    TempQtyToReceive = 0;
                                    // update Process qty
                                }
                                else
                                {
                                    //Update current record here
                                    TempQtyToReceive = TempQtyToReceive - AvailableQtyForCurrentRecord;
                                    ProcessedQtyPerRow = AvailableQtyForCurrentRecord;

                                    if (CustAvail > 0)
                                    {
                                        item.ProcessedCustomerOwnedQuantity = (item.ProcessedCustomerOwnedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    if (ConsignedAvail > 0)
                                    {
                                        item.ProcessedConsignedQuantity = (item.ProcessedConsignedQuantity ?? 0) + ProcessedQtyPerRow;
                                    }

                                    TotalReceivedQty = TotalReceivedQty + AvailableQtyForCurrentRecord;
                                    // update Process qty
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    if ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0) == (item.ProcessedConsignedQuantity ?? 0) + (item.ProcessedCustomerOwnedQuantity ?? 0))
                    {
                        item.IsTransfered = true;
                    }

                    if (objMasterDTO.StagingID.GetValueOrDefault(0) > 0)
                    {
                        MaterialStagingPullDetailDTO objItemMSPullDetailDTO = GEtMSPullDetailFromInOutItemDetailForIn(item, oBinMasterDTO, UserID, objMSDOT, QtyToReceive, RoomDateFormat);
                        List<MaterialStagingPullDetailDTO> objItemMSPullDetailDTOList = new List<MaterialStagingPullDetailDTO>();
                        objItemMSPullDetailDTOList.Add(objItemMSPullDetailDTO);
                        objItemLocDetailDAL.ItemLocationDetailsSaveForMSCredit(objItemMSPullDetailDTOList, RoomID, CompanyID, UserID, RoomDateFormat, SessionUserId,EnterpriseId, "Staging Transfer Receive");
                        //objItem.WhatWhereAction = "Transfer";
                        //objItemDAL.Edit(objItem);

                        item.ItemLocationDetailGUID = objItemMSPullDetailDTOList[0].GUID;
                    }
                    else
                    {
                        ItemLocationDetailsDTO objItemLocDetailDTO = GetItemLocationDetailFromInOutItemDetailForIn(item, oBinMasterDTO, UserID, QtyToReceive, ProcessedQtyPerRow);
                        List<ItemLocationDetailsDTO> objItemLocDetailDTOList = new List<ItemLocationDetailsDTO>();
                        objItemLocDetailDTOList.Add(objItemLocDetailDTO);
                        objItemLocDetailDAL.ItemLocationDetailsSave(objItemLocDetailDTOList, RoomDateFormat, SessionUserId,EnterpriseId);
                        item.ItemLocationDetailGUID = objItemLocDetailDTOList[0].GUID;
                    }

                    item.BinID = LocationID;
                    item.EditedFrom = EditedFrom;
                    item.Updated = DateTimeUtility.DateTimeNow;
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;

                    InOutItemDetaiDAL.EditTransInOutItemDetail(item);
                }

                InOutQtyitem.BinID = LocationID;
                InOutQtyitem.IsArchived = false;
                InOutQtyitem.IsDeleted = false;
                InOutQtyitem.LastUpdatedBy = UserID;
                InOutQtyitem.Updated = DateTime.Now;
                InOutQtyitem.ReceivedOn = DateTimeUtility.DateTimeNow;
                InOutQtyitem.EditedFrom = EditedFrom;
                EditLocation(InOutQtyitem);

                //objDetailDTO.ReceivedQuantity = objDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + objDetailDTO.FulFillQuantity;
                //TODO: Commented below line as per issue WI-3793 on 11-May-2018
                //objDetailDTO.ReceivedQuantity = objDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + InOutQtyitem.TotalQuantity;
                objDetailDTO.ReceivedQuantity = (objDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + TotalReceivedQty) > objDetailDTO.ApprovedQuantity.GetValueOrDefault(0)
                    ? objDetailDTO.ApprovedQuantity.GetValueOrDefault(0)
                    : (objDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + TotalReceivedQty);

                //objDetailDTO.FulFillQuantity = 0;
                objDetailDTO.ShippedQuantity = 0;
                objDetailDTO.IntransitQuantity = objDetailDTO.IntransitQuantity.GetValueOrDefault(0) - TotalReceivedQty;

                if (objDetailDTO.IntransitQuantity.GetValueOrDefault(0) < 0)
                    objDetailDTO.IntransitQuantity = 0;

                objDetailDTO.Bin = LocationID; // added to update Bin in requsting room(if user change receive location)
                objDetailDTO.LastUpdatedBy = UserID;
                objDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDetailDTO.EditedFrom = EditedFrom;
                objDetailDAL.Edit(objDetailDTO, SessionUserId,EnterpriseId);


                var replinishRoomItemGuid = GetSameItemByIDInReplinishRoom(ItemGUID, objReplenishMasterDTO.RoomID, CompanyID);
                TransferDetailDTO ReplenishDetailDTO = ReplinishDetailDTOList.Where(x => x.ItemGUID == replinishRoomItemGuid && x.IntransitQuantity.GetValueOrDefault(0) > 0 && (x.ReceivedQuantity.GetValueOrDefault(0) < x.FulFillQuantity.GetValueOrDefault(0) || x.ReceivedQuantity.GetValueOrDefault(0) < x.ApprovedQuantity.GetValueOrDefault(0))).FirstOrDefault();  // Modified for WI-4854: Allow adding the same item to a transfer more than once
                TransferInOutQtyDetailDTO objReplinishInOutDetaiDTO = GetTransferInOutQtyDetailByTrfAndItem(ReplenishDetailDTO.GUID, objReplenishMasterDTO.GUID, replinishRoomItemGuid);
                List<TransferInOutItemDetailDTO> objReplinishInItemDetailDTOList = InOutItemDetaiDAL.GetTrfInOutItemDetailByInOutQtyDetailGuid(objReplinishInOutDetaiDTO.GUID).ToList();

                //ReplenishDetailDTO.ReceivedQuantity = ReplenishDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + ReplenishDetailDTO.FulFillQuantity;
                //TODO: Commented below line as per issue WI-3793 on 11-May-2018
                //ReplenishDetailDTO.ReceivedQuantity = ReplenishDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + InOutQtyitem.TotalQuantity;
                ReplenishDetailDTO.ReceivedQuantity = (ReplenishDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + TotalReceivedQty) > ReplenishDetailDTO.ApprovedQuantity.GetValueOrDefault(0)
                        ? ReplenishDetailDTO.ApprovedQuantity.GetValueOrDefault(0)
                        : (ReplenishDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + TotalReceivedQty);

                ReplenishDetailDTO.ShippedQuantity = 0;
                ReplenishDetailDTO.IntransitQuantity = ReplenishDetailDTO.IntransitQuantity.GetValueOrDefault(0) - TotalReceivedQty;

                if (ReplenishDetailDTO.IntransitQuantity.GetValueOrDefault(0) < 0)
                    ReplenishDetailDTO.IntransitQuantity = 0;

                ReplenishDetailDTO.LastUpdatedBy = UserID;
                ReplenishDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                ReplenishDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                ReplenishDetailDTO.EditedFrom = EditedFrom;
                objDetailDAL.Edit(ReplenishDetailDTO, SessionUserId,EnterpriseId);

                InOutQtyitem.TrfInOutItemDetail = objReplinishInItemDetailDTOList;
                InOutQtyitem.ReceivedOn = DateTimeUtility.DateTimeNow;
                InOutQtyitem.EditedFrom = EditedFrom;
                InOutQtyitem.LastUpdatedBy = UserID;
                EditLocation(InOutQtyitem);
            }

            // Below if condition will be true in case of any of the serial number is duplicate.
            // To prevent Overwriting response.IsSuccess and response.Message we are returning response from here.
            if (!response.IsSuccess && !string.IsNullOrEmpty(response.Message))
            {
                return response;
            }

            string msgQuantityReceived = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityReceived", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
            response.IsSuccess = true;
            response.Message = msgQuantityReceived;

            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(RoomID, CompanyID, ItemGUID, UserID, SessionUserId, null, null);

            return response;
        }

        public void UpdateOnTransferQuantityByTransferDetailGUID(Guid ByTransferDetailGUID, long LoginUserId, bool IsFromCloseTransfer, long SessionUserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var objTransferDetail = (from TD in context.TransferDetails
                                         join TM in context.TransferMasters on TD.TransferGUID equals TM.GUID
                                         join IM in context.ItemMasters on TD.ItemGUID equals IM.GUID
                                         where TD.GUID == ByTransferDetailGUID
                                         select new
                                         {
                                             GUID = TD.GUID,
                                             TransferGUID = TD.TransferGUID,
                                             RequestType = TM.RequestType,
                                             RefTransferGUID = TM.RefTransferGUID,
                                             ItemGUID = TD.ItemGUID,
                                             ItemNumber = IM.ItemNumber
                                         }).FirstOrDefault();

                if (objTransferDetail != null)
                {
                    if (objTransferDetail.RequestType != 0)
                    {
                        objTransferDetail = (from TD in context.TransferDetails
                                             join TM in context.TransferMasters on TD.TransferGUID equals TM.GUID
                                             join IM in context.ItemMasters on TD.ItemGUID equals IM.GUID
                                             where TM.GUID == objTransferDetail.RefTransferGUID
                                                   && IM.ItemNumber.Trim().ToUpper() == objTransferDetail.ItemNumber.Trim().ToUpper()
                                             select new
                                             {
                                                 GUID = TD.GUID,
                                                 TransferGUID = TD.TransferGUID,
                                                 RequestType = TM.RequestType,
                                                 RefTransferGUID = TM.RefTransferGUID,
                                                 ItemGUID = TD.ItemGUID,
                                                 ItemNumber = IM.ItemNumber
                                             }).FirstOrDefault();
                    }

                    if (objTransferDetail != null)
                    {
                        double? OnTransferQuantity = (from TD in context.TransferDetails
                                                      join TM in context.TransferMasters on TD.TransferGUID equals TM.GUID
                                                      where TD.ItemGUID == objTransferDetail.ItemGUID
                                                            && (TD.IsDeleted == null || TD.IsDeleted == false)
                                                            && (TD.IsArchived == null || TD.IsArchived == false)
                                                            && (TM.IsDeleted == null || TM.IsDeleted == false)
                                                            && (TM.IsArchived == null || TM.IsArchived == false)
                                                            && TM.RequestType == 0
                                                            && (TM.TransferStatus != (int)TransferStatus.Closed && TM.TransferStatus != (int)TransferStatus.Rejected)
                                                      select (TD.RequestedQuantity) - (TD.ReceivedQuantity == null ? 0 : TD.ReceivedQuantity)).Sum();

                        ItemMaster objItemMaster = (from IM in context.ItemMasters where IM.GUID == objTransferDetail.ItemGUID select IM).FirstOrDefault();
                        objItemMaster.OnTransferQuantity = (OnTransferQuantity == null ? 0 : OnTransferQuantity.Value);
                        context.SaveChanges();

                        if (IsFromCloseTransfer)
                        {
                            double? InTransitQuantity = (from TD in context.TransferDetails
                                                         join TM in context.TransferMasters on TD.TransferGUID equals TM.GUID
                                                         where TD.ItemGUID == objTransferDetail.ItemGUID
                                                               && (TD.IsDeleted == null || TD.IsDeleted == false)
                                                               && (TD.IsArchived == null || TD.IsArchived == false)
                                                               && (TM.IsDeleted == null || TM.IsDeleted == false)
                                                               && (TM.IsArchived == null || TM.IsArchived == false)
                                                               && TM.RequestType == 0
                                                               && (TM.TransferStatus != (int)TransferStatus.Closed && TM.TransferStatus != (int)TransferStatus.Rejected)
                                                         select (TD.FulFillQuantity == null ? 0 : TD.FulFillQuantity) - (TD.ReceivedQuantity == null ? 0 : TD.ReceivedQuantity)).Sum();

                            ItemMaster item = (from IM in context.ItemMasters where IM.GUID == objTransferDetail.ItemGUID select IM).FirstOrDefault();
                            item.InTransitquantity = (InTransitQuantity == null ? 0 : InTransitQuantity);
                            context.SaveChanges();
                        }

                        CartItemDAL objCartItem = new CartItemDAL(base.DataBaseName);
                        //objCartItem.AutoCartUpdateByCode(objItemMaster.GUID, LoginUserId, "web", "CartItemDAL>TransferInOutQtyDetailDAL>UpdateOnTransferQuantityByTransferDetailGUID");
                        objCartItem.AutoCartUpdateByCode(objItemMaster.GUID, LoginUserId, "web", "Replenish >> Modified Transfer Quantity", SessionUserId);

                    }
                }
            }
        }

        public IEnumerable<TransferInOutQtyDetailLimited> GetTransferInOutQtyDetail(Guid TransferDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TransferDetailGUID", TransferDetailGUID) };
                return context.Database.SqlQuery<TransferInOutQtyDetailLimited>("EXEC [GetTransferInOutQtyDetail] @TransferDetailGUID", params1).ToList();
            }
        }
    }
}
