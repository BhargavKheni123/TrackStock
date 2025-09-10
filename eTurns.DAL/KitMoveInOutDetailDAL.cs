using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace eTurns.DAL
{
    public class KitMoveInOutDetailDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        #region [Class Constructor]

        public KitMoveInOutDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public KitMoveInOutDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Insert Record in the DataBase KitMoveInOutDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(KitMoveInOutDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                KitMoveInOutDetail obj = new KitMoveInOutDetail();
                obj.ID = 0;
                obj.KitDetailGUID = objDTO.KitDetailGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.BinID = objDTO.BinID;
                obj.MoveInOut = objDTO.MoveInOut;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.TotalQuantity = objDTO.TotalQuantity;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = Guid.NewGuid();
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";
                obj.ItemLocationDetailGuid = objDTO.ItemLocationDetailGUID;
                obj.LotNumber = objDTO.LotNumber;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.ExpirationDate = objDTO.ExpirationDate;
                obj.IsMovedOut = (!string.IsNullOrEmpty(obj.MoveInOut) && obj.MoveInOut.ToUpper() == "OUT") ? true : false;
                context.KitMoveInOutDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(KitMoveInOutDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                KitMoveInOutDetail obj = new KitMoveInOutDetail();
                obj.ID = objDTO.ID;
                obj.KitDetailGUID = objDTO.KitDetailGUID;

                obj.ItemGUID = objDTO.ItemGUID;
                obj.BinID = objDTO.BinID;
                obj.MoveInOut = objDTO.MoveInOut;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.TotalQuantity = objDTO.TotalQuantity;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = objDTO.GUID;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                obj.ItemLocationDetailGuid = objDTO.ItemLocationDetailGUID;
                obj.LotNumber = objDTO.LotNumber;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.ExpirationDate = objDTO.ExpirationDate;

                context.KitMoveInOutDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string GUIDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@GUIDs", GUIDs),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                context.Database.SqlQuery<int>("EXEC [DeleteKitMoveInOutDetail] @UserID,@GUIDs,@CompanyID", params1).FirstOrDefault();
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
        public ResponseMessage DecreaseQuantity(MoveInOutQtyDetail objMoveInQty, long RoomID, long CompanyID, long UserID, long SessionUserId, long EnterpriseID, string CultureCode)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ResponseMessage response = new ResponseMessage();
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, Guid.Parse(objMoveInQty.ItemGUID));
            if (ItemDTO.OnHandQuantity < objMoveInQty.TotalQty)
            {
                var transferResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", CultureCode, EnterpriseID, CompanyID);
                string msgNotEnoughQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantity", transferResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResTransfer", CultureCode);
                response.IsSuccess = false;
                response.Message = msgNotEnoughQuantity;
                return response;
            }
            else
            {
                bool IsQtyAvailable = true;
                string msg = "";

                foreach (var item in objMoveInQty.BinWiseQty)
                {

                    ResponseMessage ResponseMsg = new CommonDAL(base.DataBaseName).CheckQuantityByLocation(item.LocationID, Guid.Parse(objMoveInQty.ItemGUID), item.Quantity, RoomID, CompanyID, EnterpriseID, UserID);
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
                ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationQTY(RoomID, CompanyID, item.LocationID, objMoveInQty.ItemGUID).FirstOrDefault();
                BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(item.LocationID, RoomID, CompanyID);
                //BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, item.LocationID,null,null).FirstOrDefault();
                ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                double CosigneQty = 0;
                double CustomerQty = 0;

                #region "ItemLocation Deduction"
                if (ItemDTO.SerialNumberTracking)
                {
                    #region "Serial logic"
                    List<ItemLocationDetailsDTO> ObjItemLocation = null;
                    //ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == item.LocationID && (x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 || x.ConsignedQuantity.GetValueOrDefault(0) > 0)).Take((int)item.Quantity).ToList();
                    ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt(item.LocationID, RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").Take((int)item.Quantity).ToList();
                    //double LocalSearilaCount = 0;
                    //double TakenCreditCount = 0;
                    foreach (var itemoil in ObjItemLocation)
                    {
                        //double loopCurrentTakenCustomer = 0;
                        //double loopCurrentTakenConsignment = 0;

                        if (ItemDTO.Consignment)
                        {
                            //loopCurrentTaken = 1;

                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                            {
                                //loopCurrentTakenConsignment = 1;
                                itemoil.ConsignedQuantity = 0;
                            }
                            else
                            {
                                // loopCurrentTakenCustomer = 1;
                                itemoil.CustomerOwnedQuantity = 0;
                            }
                        }
                        else
                        {
                            // loopCurrentTakenCustomer = 1;
                            itemoil.CustomerOwnedQuantity = 0;
                        }
                        itemoil.KitDetailGUID = Guid.Parse(objMoveInQty.KitDetailGUID);
                        objLocationDAL.Edit(itemoil);
                    }
                    #endregion
                }
                else
                {
                    #region "Lot and other type logic"
                    //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == item.LocationID).OrderBy("CustomerOwnedQuantity DESC").ToList();
                    List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt(item.LocationID, RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").ToList();
                    Double takenQunatity = 0;
                    foreach (var itemoil in ObjItemLocation)
                    {
                        Double loopCurrentTakenCustomer = 0;
                        Double loopCurrentTakenConsignment = 0;
                        if (takenQunatity == item.Quantity)
                        {
                            break;
                        }
                        itemoil.KitDetailGUID = Guid.Parse(objMoveInQty.KitDetailGUID);
                        if (ItemDTO.Consignment)
                        {
                            #region "Consignment Pull"
                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                            {
                                loopCurrentTakenConsignment = item.Quantity - takenQunatity;
                                itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                takenQunatity = (item.Quantity - takenQunatity);
                            }
                            else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (item.Quantity - takenQunatity);
                                itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                takenQunatity = item.Quantity - takenQunatity;
                            }
                            else
                            {
                                takenQunatity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - takenQunatity;
                                if (itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                                {
                                    loopCurrentTakenConsignment = (item.Quantity - takenQunatity);
                                    itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                    takenQunatity += item.Quantity - takenQunatity;
                                }
                                else
                                {
                                    loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                    takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                    itemoil.ConsignedQuantity = 0;
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region "Customer own Pull"
                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (item.Quantity - takenQunatity);
                                itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                takenQunatity += (item.Quantity - takenQunatity);
                            }
                            else
                            {
                                loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;

                            }
                            #endregion
                        }
                        objLocationDAL.Edit(itemoil);
                    }
                    #endregion
                }
                #endregion

                #region "ItemLocation Quantity Deduction"

                ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - item.Quantity;
                if (ItemDTO.Consignment)
                {
                    //Both's sum we have available.
                    if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                    {
                        CosigneQty = item.Quantity;
                        lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - item.Quantity;
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
                        Double consqty = cstqty;// item.Quantity - cstqty;

                        lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
                        CustomerQty = (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); ;
                        CosigneQty = consqty;
                        lstLocDTO.CustomerOwnedQuantity = 0;
                        lstLocDTO.Quantity = lstLocDTO.Quantity - (CustomerQty + CosigneQty);
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
                ItemDTO.LastUpdatedBy = UserID;
                ItemDTO.WhatWhereAction = "Kit";
                objItemDAL.Edit(ItemDTO, SessionUserId, EnterpriseID);
                ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                lstUpdate.Add(lstLocDTO);
                objLocQTY.Save(lstUpdate, SessionUserId, EnterpriseID);

                #endregion

                #region "Saving Kit move inout table"
                KitMoveInOutDetailDTO objKitMoveInOutDTO = new KitMoveInOutDetailDTO()
                {
                    BinID = item.LocationID,

                    MoveInOut = "IN",
                    ItemGUID = new Guid(objMoveInQty.ItemGUID),
                    KitDetailGUID = new Guid(objMoveInQty.KitDetailGUID),

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
                };

                Insert(objKitMoveInOutDTO);
                #endregion
            }

            #region "Update ItemMaster and Kit Detail table"

            KitDetailDAL kitDetailDAL = new KitDetailDAL(base.DataBaseName);
            KitDetailDTO kitDetailDTO = kitDetailDAL.GetRecord(objMoveInQty.KitDetailGUID, RoomID, CompanyID, false, false, false);
            kitDetailDTO.AvailableItemsInWIP = kitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) + objMoveInQty.TotalQty;
            kitDetailDTO.LastUpdatedBy = UserID;
            kitDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            kitDetailDTO.EditedFrom = "Web";
            kitDetailDAL.Edit(kitDetailDTO, SessionUserId, EnterpriseID);

            response.IsSuccess = true;
            var kitResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResKitMaster", CultureCode, EnterpriseID, CompanyID);
            string msgQuantityMovedIn = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityMovedIn", kitResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResKitMaster", CultureCode);
            response.Message = msgQuantityMovedIn;
            #endregion
            return response;
        }

        /// <summary>
        /// DecreaseQuantity
        /// </summary>
        /// <param name="objMoveInQty"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public ResponseMessage BreakKit(MoveInOutQtyDetail objMoveInQty, long RoomID, long CompanyID, long UserID, long SessionUserId, long EnterpriseID, string CultureCode)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ResponseMessage response = new ResponseMessage();
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, Guid.Parse(objMoveInQty.ItemGUID));
            List<KitMoveInOutDetailDTO> lstKitMoveInOut = new List<KitMoveInOutDetailDTO>();
            if (ItemDTO.OnHandQuantity < objMoveInQty.TotalQty)
            {
                var transferResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", CultureCode, EnterpriseID, CompanyID);
                string msgNotEnoughQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantity", transferResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResTransfer", CultureCode);
                response.IsSuccess = false;
                response.Message = msgNotEnoughQuantity;
                return response;
            }
            else
            {
                bool IsQtyAvailable = true;
                string msg = "";

                foreach (var item in objMoveInQty.BinWiseQty)
                {
                    ResponseMessage ResponseMsg = new CommonDAL(base.DataBaseName).CheckQuantityByLocation(item.LocationID, Guid.Parse(objMoveInQty.ItemGUID), item.Quantity, RoomID, CompanyID, EnterpriseID, UserID);
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
                ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationQTY(RoomID, CompanyID, item.LocationID, objMoveInQty.ItemGUID).FirstOrDefault();
                BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(item.LocationID, RoomID, CompanyID);
                //BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, item.LocationID,null,null).FirstOrDefault();
                ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                double CosigneQty = 0;
                double CustomerQty = 0;

                #region "ItemLocation Deduction"
                if (ItemDTO.SerialNumberTracking)
                {
                    #region "Serial logic"
                    List<ItemLocationDetailsDTO> ObjItemLocation = null;
                    ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt(item.LocationID, RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").Take((int)item.Quantity).ToList();

                    foreach (var itemoil in ObjItemLocation)
                    {
                        KitMoveInOutDetailDTO objKitMoveInOutDTO = new KitMoveInOutDetailDTO()
                        {
                            BinID = item.LocationID,
                            MoveInOut = "BreakKit",
                            ItemGUID = new Guid(objMoveInQty.ItemGUID),
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            CreatedBy = UserID,
                            LastUpdatedBy = UserID,
                            CompanyID = CompanyID,
                            Room = RoomID,
                            IsArchived = false,
                            IsDeleted = false,
                            TotalQuantity = item.Quantity,
                        };

                        if (ItemDTO.Consignment)
                        {
                            //loopCurrentTaken = 1;

                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                            {
                                itemoil.ConsignedQuantity = 0;
                                objKitMoveInOutDTO.ConsignedQuantity = 1;
                            }
                            else
                            {
                                itemoil.CustomerOwnedQuantity = 0;
                                objKitMoveInOutDTO.CustomerOwnedQuantity = 1;
                            }
                        }
                        else
                        {
                            itemoil.CustomerOwnedQuantity = 0;
                            objKitMoveInOutDTO.CustomerOwnedQuantity = 1;
                        }
                        //itemoil.KitDetailGUID = Guid.Parse(objMoveInQty.KitDetailGUID);
                        objLocationDAL.Edit(itemoil);
                        objKitMoveInOutDTO.ItemLocationDetailGUID = itemoil.GUID;
                        lstKitMoveInOut.Add(objKitMoveInOutDTO);

                    }
                    #endregion
                }
                else
                {
                    #region "Lot and other type logic"
                    //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == item.LocationID).OrderBy("CustomerOwnedQuantity DESC").ToList();
                    List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt(item.LocationID, RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").ToList();
                    Double takenQunatity = 0;
                    foreach (var itemoil in ObjItemLocation)
                    {
                        if (takenQunatity >= item.Quantity)
                        {
                            break;
                        }

                        KitMoveInOutDetailDTO objKitMoveInOutDTO = new KitMoveInOutDetailDTO()
                        {
                            BinID = item.LocationID,
                            MoveInOut = "BreakKit",
                            ItemGUID = new Guid(objMoveInQty.ItemGUID),
                            Created = DateTimeUtility.DateTimeNow,
                            Updated = DateTimeUtility.DateTimeNow,
                            ReceivedOn = DateTimeUtility.DateTimeNow,
                            ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                            CreatedBy = UserID,
                            LastUpdatedBy = UserID,
                            CompanyID = CompanyID,
                            Room = RoomID,
                            IsArchived = false,
                            IsDeleted = false,
                            TotalQuantity = item.Quantity,
                        };
                        //itemoil.KitDetailGUID = Guid.Parse(objMoveInQty.KitDetailGUID);
                        if (ItemDTO.Consignment)
                        {
                            #region "Consignment Pull"
                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                            {

                                itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                objKitMoveInOutDTO.ConsignedQuantity = (item.Quantity - takenQunatity);
                                takenQunatity += (item.Quantity - takenQunatity);

                            }
                            else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                            {

                                itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                objKitMoveInOutDTO.CustomerOwnedQuantity = (item.Quantity - takenQunatity);
                                takenQunatity += item.Quantity - takenQunatity;

                            }
                            else
                            {
                                objKitMoveInOutDTO.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - takenQunatity;

                                if (itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                                {
                                    itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                    objKitMoveInOutDTO.ConsignedQuantity = (item.Quantity - takenQunatity);
                                    takenQunatity += item.Quantity - takenQunatity;

                                }
                                else
                                {
                                    takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                    objKitMoveInOutDTO.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                    itemoil.ConsignedQuantity = 0;
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region "Customer own Pull"
                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (item.Quantity - takenQunatity))
                            {
                                itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (item.Quantity - takenQunatity);
                                objKitMoveInOutDTO.CustomerOwnedQuantity = (item.Quantity - takenQunatity);
                                takenQunatity += (item.Quantity - takenQunatity);

                            }
                            else
                            {
                                takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objKitMoveInOutDTO.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
                            }
                            #endregion
                        }
                        objLocationDAL.Edit(itemoil);
                        objKitMoveInOutDTO.ItemLocationDetailGUID = itemoil.GUID;
                        lstKitMoveInOut.Add(objKitMoveInOutDTO);

                    }
                    #endregion
                }
                #endregion

                #region "ItemLocation Quantity Deduction"

                ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - item.Quantity;
                if (ItemDTO.Consignment)
                {
                    //Both's sum we have available.
                    if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                    {
                        CosigneQty = item.Quantity;
                        lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - item.Quantity;
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
                        Double consqty = cstqty;// item.Quantity - cstqty;

                        lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
                        CustomerQty = (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); ;
                        CosigneQty = consqty;
                        lstLocDTO.CustomerOwnedQuantity = 0;
                        lstLocDTO.Quantity = lstLocDTO.Quantity - (CustomerQty + CosigneQty);
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
                ItemDTO.LastUpdatedBy = UserID;
                ItemDTO.WhatWhereAction = "Kit-Break";
                objItemDAL.Edit(ItemDTO, SessionUserId, EnterpriseID);
                ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                lstUpdate.Add(lstLocDTO);
                objLocQTY.Save(lstUpdate, SessionUserId, EnterpriseID);

                #endregion

                #region "Saving Kit move inout table"
                foreach (var kitMoveinOut in lstKitMoveInOut)
                {
                    Insert(kitMoveinOut);
                }
                //KitMoveInOutDetailDTO objKitMoveInOutDTO = new KitMoveInOutDetailDTO()
                //{
                //    BinID = item.LocationID,

                //    MoveInOut = "BreakKit",
                //    ItemGUID = new Guid(objMoveInQty.ItemGUID),
                //    //KitDetailGUID = new Guid(objMoveInQty.KitDetailGUID),

                //    Created = DateTime.Now,
                //    Updated = DateTime.Now,
                //    CreatedBy = UserID,
                //    LastUpdatedBy = UserID,
                //    CompanyID = CompanyID,
                //    Room = RoomID,

                //    IsArchived = false,
                //    IsDeleted = false,

                //    ConsignedQuantity = CosigneQty,
                //    CustomerOwnedQuantity = CustomerQty,
                //    TotalQuantity = item.Quantity,
                //};

                //Insert(objKitMoveInOutDTO);
                #endregion
            }

            #region "Update ItemMaster and Kit Detail table"

            //KitDetailDAL kitDetailDAL = new KitDetailDAL(base.DataBaseName);
            //KitDetailDTO kitDetailDTO = kitDetailDAL.GetRecord(objMoveInQty.KitDetailGUID, RoomID, CompanyID, false, false, false);
            //kitDetailDTO.AvailableItemsInWIP += objMoveInQty.TotalQty;
            //kitDetailDTO.LastUpdatedBy = UserID;
            //kitDetailDAL.Edit(kitDetailDTO);

            KitDetailDAL kitDetailDAL = new KitDetailDAL(base.DataBaseName);
            List<KitDetailDTO> lstKitDetailDTO = kitDetailDAL.GetAllRecordsByKitGUID(Guid.Parse(objMoveInQty.ItemGUID), RoomID, CompanyID, false, false, false).ToList();
            foreach (var item in lstKitDetailDTO)
            {

                item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) + (item.QuantityPerKit.GetValueOrDefault(0) * objMoveInQty.TotalQty);
                item.LastUpdatedBy = UserID;
                item.ReceivedOn = DateTimeUtility.DateTimeNow;
                item.EditedFrom = "Web";
                kitDetailDAL.Edit(item, SessionUserId, EnterpriseID);
            }
            response.IsSuccess = true;
            var kitMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResKitMaster", CultureCode, EnterpriseID, CompanyID);
            string msgQuantityBreak = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityBreak", kitMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResKitMaster", CultureCode);
            response.Message = msgQuantityBreak;
            #endregion
            return response;
        }

        public KitItemToMoveDTO MoveInKitItemQuantity(KitItemToMoveDTO objItemReturnInfo, long SessionUserId, long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objItemReturnInfo == null || objItemReturnInfo.lstItemPullDetails == null)
                {
                    return objItemReturnInfo;
                }
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemReturnInfo.ItemGUID);
                objItemReturnInfo.lstItemPullDetails.ForEach(t =>
                {
                    string InventoryConsuptionMethod = string.Empty;
                    Room objRoomDTO = context.Rooms.FirstOrDefault(x => x.ID == objItemReturnInfo.RoomId);
                    if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                    {
                        InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                    }

                    if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                        InventoryConsuptionMethod = "";

                    //ItemLocationDetail objItemLocationDetail = null;
                    List<ItemLocationDetail> lstItemLocations = null;
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                          (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                              || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                          || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                          && (x.ExpirationDate == t.ExpirationDate)
                                          && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking))
                                            && (x.ExpirationDate == t.ExpirationDate)
                                            && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                           (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                               || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                           || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                           && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                    }


                    if (lstItemLocations != null)
                    {
                        foreach (var objItemLocationDetail in lstItemLocations)
                        {
                            if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) >= (t.ConsignedTobePulled + t.CustomerOwnedTobePulled))
                            {
                                KitMoveInOutDetail objKitMoveInOutDetail = new KitMoveInOutDetail();
                                objKitMoveInOutDetail.BinID = t.BinID;
                                objKitMoveInOutDetail.CompanyID = objItemReturnInfo.CompanyId;
                                objKitMoveInOutDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                objKitMoveInOutDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                objKitMoveInOutDetail.TotalQuantity = objItemReturnInfo.QtyToMoveIn;
                                objKitMoveInOutDetail.GUID = Guid.NewGuid();
                                objKitMoveInOutDetail.IsArchived = false;
                                objKitMoveInOutDetail.IsDeleted = false;
                                objKitMoveInOutDetail.MoveInOut = "IN";
                                objKitMoveInOutDetail.ItemGUID = objItemReturnInfo.ItemGUID;
                                objKitMoveInOutDetail.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                                objKitMoveInOutDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objKitMoveInOutDetail.KitDetailGUID = objItemReturnInfo.KitDetailGUID;
                                objKitMoveInOutDetail.ExpirationDate = objItemLocationDetail.ExpirationDate;

                                objKitMoveInOutDetail.Room = objItemReturnInfo.RoomId;
                                objKitMoveInOutDetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;

                                objKitMoveInOutDetail.Updated = DateTimeUtility.DateTimeNow;
                                objKitMoveInOutDetail.Created = DateTimeUtility.DateTimeNow;
                                objKitMoveInOutDetail.CreatedBy = objItemReturnInfo.CreatedBy;
                                objKitMoveInOutDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objKitMoveInOutDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objKitMoveInOutDetail.AddedFrom = "Web";
                                objKitMoveInOutDetail.EditedFrom = "Web";
                                objKitMoveInOutDetail.ItemLocationDetailGuid = objItemLocationDetail.GUID;

                                context.KitMoveInOutDetails.Add(objKitMoveInOutDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;

                                CreateItemLocationDetailsCache(objItemLocationDetail);
                                break;
                            }
                            else if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) > 0)
                            {

                                KitMoveInOutDetail objKTMIODetail = new KitMoveInOutDetail();
                                objKTMIODetail.BinID = t.BinID;
                                objKTMIODetail.CompanyID = objItemReturnInfo.CompanyId;
                                objKTMIODetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity ?? 0;
                                objKTMIODetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity ?? 0;
                                objKTMIODetail.TotalQuantity = objItemReturnInfo.QtyToMoveIn;
                                objKTMIODetail.GUID = Guid.NewGuid();
                                objKTMIODetail.IsArchived = false;
                                objKTMIODetail.IsDeleted = false;
                                objKTMIODetail.MoveInOut = "IN";
                                objKTMIODetail.ItemGUID = objItemReturnInfo.ItemGUID;
                                objKTMIODetail.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                                objKTMIODetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objKTMIODetail.KitDetailGUID = objItemReturnInfo.KitDetailGUID;
                                objKTMIODetail.ItemLocationDetailGuid = t.GUID;
                                objKTMIODetail.ExpirationDate = objItemLocationDetail.ExpirationDate;

                                t.CustomerOwnedTobePulled = (t.CustomerOwnedTobePulled) - (objItemLocationDetail.CustomerOwnedQuantity ?? 0);
                                t.ConsignedTobePulled = (t.ConsignedTobePulled) - (objItemLocationDetail.ConsignedQuantity ?? 0);

                                objKTMIODetail.Room = objItemReturnInfo.RoomId;
                                objKTMIODetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;

                                objKTMIODetail.Updated = DateTimeUtility.DateTimeNow;
                                objKTMIODetail.Created = DateTimeUtility.DateTimeNow;
                                objKTMIODetail.CreatedBy = objItemReturnInfo.CreatedBy;
                                objKTMIODetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objKTMIODetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objKTMIODetail.AddedFrom = "Web";
                                objKTMIODetail.EditedFrom = "Web";
                                objKTMIODetail.ItemLocationDetailGuid = objItemLocationDetail.GUID;
                                context.KitMoveInOutDetails.Add(objKTMIODetail);
                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;

                                CreateItemLocationDetailsCache(objItemLocationDetail);
                            }
                        }
                    }
                    context.SaveChanges();
                });



                ItemLocationQTY objItemLocationQTY = context.ItemLocationQTies.FirstOrDefault(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID);
                if (objItemLocationQTY == null)
                {
                    objItemLocationQTY = new ItemLocationQTY();
                    objItemLocationQTY.BinID = objItemReturnInfo.BinID;
                    objItemLocationQTY.CompanyID = objItemReturnInfo.CompanyId;
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Created = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.CreatedBy = objItemReturnInfo.CreatedBy;
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.GUID = Guid.NewGuid();
                    objItemLocationQTY.ItemGUID = objItemReturnInfo.ItemGUID;
                    objItemLocationQTY.LastUpdated = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                    objItemLocationQTY.Room = objItemReturnInfo.RoomId;

                    objItemLocationQTY.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.AddedFrom = "Web";
                    objItemLocationQTY.EditedFrom = "Web";

                    context.ItemLocationQTies.Add(objItemLocationQTY);
                }
                else
                {
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                }
                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                context.SaveChanges();

                //DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                //CostDTO objCostDTO = objDashboardDAL.GetAvgExtendedCost(objItem.GUID);
                //objItem.ExtendedCost = objCostDTO.ExtCost;
                //objItem.AverageCost = objCostDTO.AvgCost;

                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);

                //objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemReturnInfo.LastUpdatedBy, "web", "KitMoveInOutDetailDAL >> MoveInKitItemQty");
                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemReturnInfo.LastUpdatedBy, "web", "Kits >> KitMoveIn", SessionUserId);

                //objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(objItemReturnInfo.RoomId, objItemReturnInfo.CompanyId, objItemReturnInfo.ItemGUID, objItemReturnInfo.CreatedBy, null, null);
                //objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(objItemReturnInfo.RoomId, objItemReturnInfo.CompanyId, objItemReturnInfo.ItemGUID, objItemReturnInfo.CreatedBy, null, null);

                context.SaveChanges();
                //if (objItem.OnHandQuantity.GetValueOrDefault(0) <= 0)
                //{
                //    objItemMasterDAL.SendMailWhenItemStockOut(objItem.ID, objItem.CompanyID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0), objItem.LastUpdatedBy.GetValueOrDefault(0), objItem.OnHandQuantity.GetValueOrDefault(0), objItem.ItemNumber, objItem.GUID);
                //}
                //else
                //{
                //    objItemMasterDAL.RemoveItemStockOutMailLog(objItem.ID, objItem.CompanyID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0));
                //}

                KitDetailDAL kitDetailDAL = new KitDetailDAL(base.DataBaseName);
                KitDetailDTO kitDetailDTO = kitDetailDAL.GetRecord(objItemReturnInfo.KitDetailGUID.ToString(), objItemReturnInfo.RoomId, objItemReturnInfo.CompanyId, false, false, false);
                kitDetailDTO.AvailableItemsInWIP = kitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) + objItemReturnInfo.QtyToMoveIn;
                kitDetailDTO.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                kitDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                kitDetailDTO.EditedFrom = "Web";
                kitDetailDAL.Edit(kitDetailDTO, SessionUserId, EnterpriseId);

                return objItemReturnInfo;
            }
        }


        /// <summary>
        /// This method is used for bulk move out
        /// </summary>
        /// <param name="objItemReturnInfo"></param>
        /// <returns></returns>
        public KitItemToMoveDTO MoveOutKitItemQuantity(KitItemToMoveDTO objItemReturnInfo, long SessionUserId, long EnterpriseId)
        {
            ItemLocationDetailsDAL itemLocationDetails = new ItemLocationDetailsDAL(base.DataBaseName);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objItemReturnInfo == null || objItemReturnInfo.lstItemPullDetails == null)
                {
                    return objItemReturnInfo;
                }
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemReturnInfo.ItemGUID);

                objItemReturnInfo.lstItemPullDetails.ForEach(t =>
                {
                    DateTime? ExpirationDate = null;
                    DateTime? OldExpirationDate = null;
                    string InventoryConsuptionMethod = string.Empty;
                    Room objRoomDTO = context.Rooms.FirstOrDefault(x => x.ID == objItemReturnInfo.RoomId);

                    if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                    {
                        InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                    }

                    if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                        InventoryConsuptionMethod = "";

                    List<KitMoveInOutDetail> kitInOutDetail = new List<KitMoveInOutDetail>();

                    if ((t.ConsignedTobePulled + t.CustomerOwnedTobePulled) > 0)
                    {

                        if (!string.IsNullOrEmpty(t.Expiration))
                        {

                            CultureInfo CurrentCult = new CultureInfo("en-US");
                            eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(objRoomDTO.ID, objRoomDTO.CompanyID.GetValueOrDefault(), 0);

                            if (RegionInfo != null)
                                CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                            DateTime DTExp = DateTime.MinValue;
                            DateTime.TryParseExact(t.Expiration, RegionInfo.ShortDatePattern, CurrentCult, DateTimeStyles.None, out DTExp);

                            if (DTExp != DateTime.MinValue)
                            {
                                ExpirationDate = DTExp;
                            }
                            if (t.ExpirationDate != null && t.ExpirationDate != DateTime.MinValue)
                            {
                                OldExpirationDate = t.ExpirationDate;
                            }
                        }

                        KitMoveInOutDetail objKTMIODetail = new KitMoveInOutDetail();
                        ItemLocationDetailsDTO objDTO = new ItemLocationDetailsDTO();
                        objKTMIODetail.BinID = t.BinID;
                        objKTMIODetail.CompanyID = objItemReturnInfo.CompanyId;
                        objDTO.CompanyID = objItemReturnInfo.CompanyId;
                        objKTMIODetail.ConsignedQuantity = t.ConsignedTobePulled;
                        objKTMIODetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                        objKTMIODetail.TotalQuantity = objItemReturnInfo.QtyToMoveIn;
                        objKTMIODetail.GUID = Guid.NewGuid();
                        objKTMIODetail.IsArchived = false;
                        objKTMIODetail.IsDeleted = false;
                        objKTMIODetail.MoveInOut = "OUT";
                        objKTMIODetail.IsMovedOut = true;
                        objKTMIODetail.ItemGUID = objItemReturnInfo.ItemGUID;
                        objKTMIODetail.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                        objKTMIODetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                        objKTMIODetail.KitDetailGUID = objItemReturnInfo.KitDetailGUID;
                        objKTMIODetail.ItemLocationDetailGuid = t.GUID;
                        objKTMIODetail.Room = objItemReturnInfo.RoomId;
                        objKTMIODetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;
                        objKTMIODetail.ExpirationDate = ExpirationDate;
                        objKTMIODetail.CreatedBy = objItemReturnInfo.CreatedBy;
                        var currentDatetimeForKit = DateTimeUtility.DateTimeNow;
                        objKTMIODetail.Updated = currentDatetimeForKit;
                        objKTMIODetail.Created = currentDatetimeForKit;
                        objKTMIODetail.ReceivedOn = currentDatetimeForKit;
                        objKTMIODetail.ReceivedOnWeb = currentDatetimeForKit;
                        objKTMIODetail.AddedFrom = "Web";
                        objKTMIODetail.EditedFrom = "Web";
                        objDTO.BinID = objKTMIODetail.BinID;
                        objDTO.ConsignedQuantity = t.ConsignedTobePulled;
                        objDTO.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.IsArchived = false;
                        objDTO.IsDefault = false;
                        objDTO.ItemGUID = objItemReturnInfo.ItemGUID;
                        objDTO.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                        objDTO.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                        objDTO.KitDetailGUID = objItemReturnInfo.KitDetailGUID;
                        objDTO.ExpirationDate = ExpirationDate;
                        objDTO.Room = objItemReturnInfo.RoomId;
                        objDTO.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;
                        objDTO.CreatedBy = objItemReturnInfo.CreatedBy;

                        var currentDatetime = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = currentDatetime;
                        objDTO.Created = currentDatetime;
                        objDTO.ReceivedOn = currentDatetime;
                        objDTO.ReceivedOnWeb = currentDatetime;
                        objDTO.ReceivedDate = currentDatetime;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";

                        var itemLocation = itemLocationDetails.Insert(objDTO);

                        if (itemLocation != null && itemLocation.GUID != Guid.Empty)
                        {
                            objKTMIODetail.ItemLocationDetailGuid = itemLocation.GUID;
                            context.KitMoveInOutDetails.Add(objKTMIODetail);

                                var kitMoveInRecord = context.KitMoveInOutDetails.Where(e => e.KitDetailGUID == objKTMIODetail.KitDetailGUID
                                                    && e.ItemGUID == objKTMIODetail.ItemGUID && e.BinID == objKTMIODetail.BinID
                                                    && e.CustomerOwnedQuantity == objKTMIODetail.CustomerOwnedQuantity
                                                    && e.ConsignedQuantity == objKTMIODetail.ConsignedQuantity
                                                    && e.MoveInOut == "IN" && e.Room == objKTMIODetail.Room
                                                    && e.CompanyID == objKTMIODetail.CompanyID
                                                    && (e.IsDeleted ?? false) == false
                                                    && e.IsMovedOut == false).FirstOrDefault();

                            if (kitMoveInRecord != null && kitMoveInRecord.ID > 0)
                            {
                                kitMoveInRecord.IsMovedOut = true;
                            }

                            InsertKitMoveInOutLog(objKTMIODetail.KitDetailGUID, objKTMIODetail.GUID, t.LotNumber, objKTMIODetail.LotNumber, objKTMIODetail.SerialNumber, objKTMIODetail.SerialNumber, OldExpirationDate, objKTMIODetail.ExpirationDate, objKTMIODetail.ConsignedQuantity, objKTMIODetail.MoveInOut);
                        }
                    }
                    context.SaveChanges();
                });

                //context.SaveChanges();


                ItemLocationQTY objItemLocationQTY = context.ItemLocationQTies.FirstOrDefault(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID);

                if (objItemLocationQTY == null)
                {
                    objItemLocationQTY = new ItemLocationQTY();
                    objItemLocationQTY.BinID = objItemReturnInfo.BinID;
                    objItemLocationQTY.CompanyID = objItemReturnInfo.CompanyId;
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList().Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.CreatedBy = objItemReturnInfo.CreatedBy;
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList().Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.GUID = Guid.NewGuid();
                    objItemLocationQTY.ItemGUID = objItemReturnInfo.ItemGUID;
                    objItemLocationQTY.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                    objItemLocationQTY.Room = objItemReturnInfo.RoomId;

                    var currentDatetimeForLocation = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.Created = currentDatetimeForLocation;
                    objItemLocationQTY.LastUpdated = currentDatetimeForLocation;
                    objItemLocationQTY.ReceivedOn = currentDatetimeForLocation;
                    objItemLocationQTY.ReceivedOnWeb = currentDatetimeForLocation;
                    objItemLocationQTY.AddedFrom = "Web";
                    objItemLocationQTY.EditedFrom = "Web";

                    context.ItemLocationQTies.Add(objItemLocationQTY);
                }
                else
                {
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList().Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList().Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                }

                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList().Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                context.SaveChanges();
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                //objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemReturnInfo.LastUpdatedBy, "web", "KitMoveInOutDetailDAL >> MoveInKitItemQty");
                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemReturnInfo.LastUpdatedBy, "web", "Kits >> KitMoveIn", SessionUserId);
                context.SaveChanges();

                //if (objItem.OnHandQuantity.GetValueOrDefault(0) <= 0)
                //{
                //    objItemMasterDAL.SendMailWhenItemStockOut(objItem.ID, objItem.CompanyID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0), objItem.LastUpdatedBy.GetValueOrDefault(0), objItem.OnHandQuantity.GetValueOrDefault(0), objItem.ItemNumber, objItem.GUID);
                //}
                //else
                //{
                //    objItemMasterDAL.RemoveItemStockOutMailLog(objItem.ID, objItem.CompanyID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0));
                //}

                KitDetailDAL kitDetailDAL = new KitDetailDAL(base.DataBaseName);
                KitDetailDTO kitDetailDTO = kitDetailDAL.GetRecord(objItemReturnInfo.KitDetailGUID.ToString(), objItemReturnInfo.RoomId, objItemReturnInfo.CompanyId, false, false, false);
                kitDetailDTO.AvailableItemsInWIP = kitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) - objItemReturnInfo.QtyToMoveIn;
                kitDetailDTO.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                kitDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                kitDetailDTO.EditedFrom = "Web";
                kitDetailDAL.Edit(kitDetailDTO, SessionUserId, EnterpriseId);
                return objItemReturnInfo;
            }
        }


        private void CreateItemLocationDetailsCache(ItemLocationDetail oItemLocationDetail)
        {
            ItemLocationDetailsDTO objDTO = new ItemLocationDetailsDTO();

            objDTO.ID = oItemLocationDetail.ID;
            objDTO.BinID = oItemLocationDetail.BinID;
            objDTO.CustomerOwnedQuantity = oItemLocationDetail.CustomerOwnedQuantity;
            objDTO.ConsignedQuantity = oItemLocationDetail.ConsignedQuantity;
            objDTO.MeasurementID = oItemLocationDetail.MeasurementID;
            objDTO.LotNumber = oItemLocationDetail.LotNumber;
            objDTO.SerialNumber = oItemLocationDetail.SerialNumber;
            objDTO.Expiration = oItemLocationDetail.Expiration;
            objDTO.Received = oItemLocationDetail.Received;
            objDTO.ExpirationDate = oItemLocationDetail.ExpirationDate;
            objDTO.ReceivedDate = oItemLocationDetail.ReceivedDate;
            objDTO.Cost = oItemLocationDetail.Cost;
            objDTO.eVMISensorPort = oItemLocationDetail.eVMISensorPort;
            objDTO.eVMISensorID = oItemLocationDetail.eVMISensorID;
            objDTO.UDF1 = oItemLocationDetail.UDF1;
            objDTO.UDF2 = oItemLocationDetail.UDF2;
            objDTO.UDF3 = oItemLocationDetail.UDF3;
            objDTO.UDF4 = oItemLocationDetail.UDF4;
            objDTO.UDF5 = oItemLocationDetail.UDF5;
            objDTO.GUID = oItemLocationDetail.GUID;
            objDTO.ItemGUID = oItemLocationDetail.ItemGUID;
            objDTO.Created = oItemLocationDetail.Created;
            objDTO.Updated = oItemLocationDetail.Updated;
            objDTO.CreatedBy = oItemLocationDetail.CreatedBy;
            objDTO.LastUpdatedBy = oItemLocationDetail.LastUpdatedBy;
            objDTO.IsDeleted = oItemLocationDetail.IsDeleted;
            objDTO.IsArchived = oItemLocationDetail.IsArchived;
            objDTO.CompanyID = oItemLocationDetail.CompanyID;

            objDTO.Room = oItemLocationDetail.Room;
            objDTO.KitDetailGUID = oItemLocationDetail.KitDetailGUID;
            objDTO.TransferDetailGUID = oItemLocationDetail.TransferDetailGUID;
            objDTO.OrderDetailGUID = oItemLocationDetail.OrderDetailGUID;
            objDTO.IsConsignedSerialLot = oItemLocationDetail.IsConsignedSerialLot;
            objDTO.InitialQuantity = oItemLocationDetail.InitialQuantity;
            objDTO.IsWebEdit = oItemLocationDetail.IsWebEdit;
            objDTO.IsPDAEdit = oItemLocationDetail.IsPDAEdit;
            objDTO.RefWebSelfGUID = oItemLocationDetail.RefWebSelfGUID;
            objDTO.RefPDASelfGUID = oItemLocationDetail.RefPDASelfGUID;
            objDTO.InitialQuantityWeb = oItemLocationDetail.InitialQuantityWeb;
            objDTO.InitialQuantityPDA = oItemLocationDetail.InitialQuantityPDA;
            objDTO.InsertedFrom = oItemLocationDetail.InsertedFrom;
            objDTO.ReceivedOnWeb = oItemLocationDetail.ReceivedOnWeb;
            objDTO.ReceivedOn = oItemLocationDetail.ReceivedOn;
            objDTO.AddedFrom = oItemLocationDetail.AddedFrom;
            objDTO.EditedFrom = oItemLocationDetail.EditedFrom;

            //IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString());
            //if (ObjCache != null)
            //{
            objDTO.SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0));

            //List<ItemLocationDetailsDTO> objTemp = ObjCache.ToList();
            //if (objTemp.Any(x => x.ID == objDTO.ID))
            //{
            //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
            //}

            //ObjCache = objTemp.AsEnumerable();

            //List<ItemLocationDetailsDTO> tempC = new List<ItemLocationDetailsDTO>();
            //tempC.Add(objDTO);
            //IEnumerable<ItemLocationDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
            //CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AppendToCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString(), NewCache);
            //}
        }

        /// <summary>
        /// This method is used to get the Location with Lot/Serials for Kit move out 
        /// </summary>
        /// <param name="kitDetailGuid"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="BinID"></param>
        /// <param name="PullQuantity"></param>
        /// <param name="PullQuantityLimit"></param>
        /// <param name="SerialOrLotNumber"></param>
        /// <param name="IsStagginLocation"></param>
        /// <returns></returns>
        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForMoveOut(Guid kitDetailGuid, Guid ItemGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, string PullQuantityLimit, string SerialOrLotNumber, string IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>(@"EXEC GetItemLocationsWithLotSerialsForMoveOut '" + kitDetailGuid.ToString() + "','" + ItemGUID.ToString() + "'," + BinID + "," + PullQuantity + "," + CompanyID + "," + RoomID + "," + PullQuantityLimit + ",'" + SerialOrLotNumber + "'," + IsStagginLocation)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate,
                                        SrNo = 0
                                    }).ToList();
                int counter = 0;
                lstItemLocations.ForEach(x => x.SrNo = counter++);
                return lstItemLocations;
            }
        }

        /// <summary>
        /// This method is used to get the Location with Lot/Serials for Kit move out 
        /// </summary>
        /// <param name="kitDetailGuid"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="BinID"></param>
        /// <param name="PullQuantity"></param>
        /// <param name="PullQuantityLimit"></param>
        /// <param name="SerialOrLotNumber"></param>
        /// <param name="IsStagginLocation"></param>
        /// <returns></returns>
        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForMoveOutKitBreak(Guid kitDetailGuid, Guid ItemGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, string PullQuantityLimit, string SerialOrLotNumber, string IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>(@"EXEC GetItemLocationsWithLotSerialsForMoveOutKitBreak '" + kitDetailGuid.ToString() + "','" + ItemGUID.ToString() + "'," + BinID + "," + PullQuantity + "," + CompanyID + "," + RoomID + "," + PullQuantityLimit + ",'" + SerialOrLotNumber + "'," + IsStagginLocation)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate,
                                        SrNo = 0
                                    }).ToList();
                int counter = 0;
                lstItemLocations.ForEach(x => x.SrNo = counter++);
                return lstItemLocations;
            }
        }

        /// <summary>
        /// This method is used to get the ItemLocation details for move out when user performed move out on ordered kit.
        /// </summary>
        /// <param name="kitGuid"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForMoveOut(Guid kitGuid, Guid ItemGUID, long RoomID, long CompanyID)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> finalItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>(@"EXEC GetItemLocationsForMoveOutFromBreakKit '" + kitGuid.ToString() + "','" + ItemGUID.ToString() + "'," + CompanyID + "," + RoomID)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate,
                                        SrNo = 0
                                    }).ToList();

                if (lstItemLocations != null && lstItemLocations.Any() && lstItemLocations[0].SerialNumberTracking && lstItemLocations[0].PullQuantity > 1)
                {
                    double totalQty = lstItemLocations[0].PullQuantity;
                    bool isCustomerOwned = lstItemLocations[0].CustomerOwnedQuantity > 0;
                    bool isConsigned = lstItemLocations[0].ConsignedQuantity > 0;

                    lstItemLocations[0].PullQuantity = 1;
                    lstItemLocations[0].LotSerialQuantity = 1;
                    if (isCustomerOwned)
                    {
                        lstItemLocations[0].CustomerOwnedQuantity = 1;
                    }
                    else
                    {
                        lstItemLocations[0].ConsignedQuantity = 1;
                    }

                    for (int recordCounter = 0; recordCounter < totalQty; recordCounter++)
                    {
                        finalItemLocations.Add(lstItemLocations[0]);
                    }
                }
                else
                {
                    finalItemLocations = lstItemLocations;
                }
                int counter = 0;
                finalItemLocations.ForEach(x => x.SrNo = counter++);
                return finalItemLocations;
            }
        }

        /// <summary>
        /// This method is used to get the line items for move out
        /// </summary>
        /// <param name="kitGuid"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public List<KitMoveInOutDetailDTO> GetItemLocationsForMoveOut(Guid KitDetailGuid, Guid ItemGUID, long RoomID, long CompanyID)
        {
            var param = new SqlParameter[] {
                                              new SqlParameter("@KitDetailGuid", KitDetailGuid),
                                              new SqlParameter("@ItemGUID", ItemGUID),
                                              new SqlParameter("@RoomID", RoomID),
                                              new SqlParameter("@CompanyID", CompanyID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<KitMoveInOutDetailDTO>("exec GetItemLocationsForMoveOut @KitDetailGuid,@ItemGUID,@RoomID,@CompanyID ", param).ToList();
            }
        }

        public List<KitMoveInOutDetailDTO> GetKitMoveInOutDetailForMoveOutPopup(Guid KitDetailGuid, Guid ItemGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@KitDetailGuid",KitDetailGuid)
                                    , new SqlParameter("@ItemGUID",ItemGUID)
                                    , new SqlParameter("@Room",RoomID)
                                    , new SqlParameter("@CompanyID",CompanyID)};

                string qry = "exec [GetKitMoveInOutDetailForMoveOutPopup] @KitDetailGuid, @ItemGUID,@Room, @CompanyID";
                return context.Database.SqlQuery<KitMoveInOutDetailDTO>(qry, params1).ToList();
            }
        }

        public bool UpdateIsMovedInFlag(KitMoveInOutDetailDTO KitMoveInOutDetails)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@KitDetailGUID", KitMoveInOutDetails.KitDetailGUID.GetValueOrDefault(Guid.Empty)),
                                                    new SqlParameter("@ItemGUID", KitMoveInOutDetails.ItemGUID.GetValueOrDefault(Guid.Empty)),
                                                    new SqlParameter("@BinID", KitMoveInOutDetails.BinID ?? (object)DBNull.Value),
                                                    new SqlParameter("@TotalQuantity", KitMoveInOutDetails.TotalQuantity),
                                                    new SqlParameter("@RoomId", KitMoveInOutDetails.Room),
                                                    new SqlParameter("@CompanyID", KitMoveInOutDetails.CompanyID) };

                context.Database.ExecuteSqlCommand("exec [UpdateIsMovedInFlag] @KitDetailGUID,@ItemGUID,@BinID,@TotalQuantity,@RoomId,@CompanyID", params1);
                return true;
            }
        }

        public bool GetCorrespondingMoveInEntry(KitMoveInOutDetailDTO KitMoveInOutDetails)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@KitDetailGUID", KitMoveInOutDetails.KitDetailGUID.GetValueOrDefault(Guid.Empty)),
                                                    new SqlParameter("@ItemGUID", KitMoveInOutDetails.ItemGUID.GetValueOrDefault(Guid.Empty)),
                                                    new SqlParameter("@BinID", KitMoveInOutDetails.BinID ?? (object)DBNull.Value),
                                                    new SqlParameter("@TotalQuantity", KitMoveInOutDetails.TotalQuantity),
                                                    new SqlParameter("@RoomId", KitMoveInOutDetails.Room),
                                                    new SqlParameter("@CompanyID", KitMoveInOutDetails.CompanyID) };

                return context.Database.SqlQuery<bool>("exec [GetGetCorrespondingMoveInEntryForMoveOut] @KitDetailGUID,@ItemGUID,@BinID,@TotalQuantity,@RoomId,@CompanyID", params1).FirstOrDefault();

            }
        }

        public bool IsMoveInAvailableForKitItem(Guid kitDetailGUID, string ItemGuid, long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int count = 0;
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@kitDetailGUID", kitDetailGUID) };
                try
                {
                    count = context.Database.SqlQuery<int>("exec [CheckMoveInOperationonKit] @ItemGuid,@kitDetailGUID,@CompanyID,@RoomID", params1).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return false;
                }
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public double? GetTotalQuantityForMoveOut(Guid kitguid, Guid kitdetailguid, Guid itemguid, long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGuid", itemguid), new SqlParameter("@KitDetailGuid", kitdetailguid), new SqlParameter("@kitGUID", kitguid) };
                try
                {
                    return context.Database.SqlQuery<double?>("exec [CalculateMoveOutQuantity] @ItemGuid,@kitGUID,@KitDetailGuid,@CompanyID,@RoomID", params1).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public bool InsertKitMoveInOutLog(Guid? kitDetailGUID, Guid? KitMoveInOutDetailGUID, string OldLotNumber, string NewLotNumber, string OldSerialNumber, string NewSerialNumber, DateTime? OldExpirationDate, DateTime? NewExpirationDate, double? ConsignedQuantity, string MoveInOut)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int count = 0;
                var params1 = new SqlParameter[] { new SqlParameter("@KitDetailGUID", kitDetailGUID),
                    new SqlParameter("@KitMoveInOutDetailGUID", KitMoveInOutDetailGUID),
                    new SqlParameter("@OldLotNumber", OldLotNumber),
                    new SqlParameter("@NewLotNumber", NewLotNumber),
                    new SqlParameter("@OldSerialNumber", OldSerialNumber),
                    new SqlParameter("@NewSerialNumber", NewSerialNumber),
                    new SqlParameter("@OldExpirationDate", OldExpirationDate ?? (object)DBNull.Value),
                    new SqlParameter("@NewExpirationDate", NewExpirationDate ?? (object)DBNull.Value),
                    new SqlParameter("@ConsignedQuantity", ConsignedQuantity),
                    new SqlParameter("@MoveInOut", MoveInOut)
                };
                try
                {
                    count = context.Database.SqlQuery<int>("exec [InsertKitMoveInOutLog] @kitDetailGUID,@KitMoveInOutDetailGUID,@OldLotNumber,@NewLotNumber,@OldSerialNumber,@NewSerialNumber,@OldExpirationDate,@NewExpirationDate,@ConsignedQuantity,@MoveInOut", params1).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return false;
                }
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}