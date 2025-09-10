using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using eTurns.DTO.Resources;
using eTurns.DTO;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data;
using System.Data.SqlClient;
namespace eTurns.DAL
{
    public partial class TransferInOutQtyDetailDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferInOutQtyDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<TransferInOutQtyDetailDTO> obj = (from u in context.ExecuteStoreQuery<TransferInOutQtyDetailDTO>(@"SELECT A.*, B.BinNumber,I.ItemNumber FROM TransferInOutQtyDetail A  
                                                                                                                                                    LEFT OUTER JOIN BinMaster B ON A.BinID = B.ID
											                                                                                                        LEFT OUTER JOIN ItemMaster I ON A.ItemGUID = I.GUID
                                                                                                                               WHERE A.CompanyID = " + CompanyID)
                                                              select new TransferInOutQtyDetailDTO
                                                              {
                                                                  ID = u.ID,
                                                                  TransferGUID = u.TransferGUID,
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
                                                                  TotalQuantity = u.TotalQuantity,
                                                                  TransferDetailGUID = u.TransferDetailGUID,
                                                                  TransferInOut = u.TransferInOut,
                                                                  Updated = u.Updated,
                                                                  IsTransfered = u.IsTransfered,
                                                                  ItemNumber = u.ItemNumber,
                                                                  BinNumber = u.BinNumber,
                                                                  ReceivedOn = u.ReceivedOn,
                                                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                                                  AddedFrom = u.AddedFrom,
                                                                  EditedFrom = u.EditedFrom,
                                                              }).AsParallel().ToList();
                return obj;
            }

        }

        /// <summary>
        /// Get Paged Records from the KitMoveInOutDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferInOutQtyDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid TransferDetailGUID)
        {
            //Get Cached-Media
            IEnumerable<TransferInOutQtyDetailDTO> ObjCache = GetAllRecords(TransferDetailGUID, RoomID, CompanyID);
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjCache = ObjCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
            }
            else if (IsArchived)
            {
                ObjCache = ObjCache.Where(t => (t.IsArchived == IsArchived));
            }
            else if (IsDeleted)
            {
                ObjCache = ObjCache.Where(t => (t.IsDeleted == IsDeleted));
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
        /// Get Particullar Record from the KitMoveInOutDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferInOutQtyDetailDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TransferInOutQtyDetailDTO obj = (from u in context.ExecuteStoreQuery<TransferInOutQtyDetailDTO>(@"SELECT A.* FROM TransferInOutQtyDetail A  WHERE A.GUID = '" + GUID + "' AND A.CompanyID = " + CompanyID)
                                                 select new TransferInOutQtyDetailDTO
                                                 {
                                                     ID = u.ID,
                                                     TransferGUID = u.TransferGUID,
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
                                                     TotalQuantity = u.TotalQuantity,
                                                     TransferDetailGUID = u.TransferDetailGUID,
                                                     TransferInOut = u.TransferInOut,
                                                     Updated = u.Updated,
                                                     IsTransfered = u.IsTransfered,
                                                     ReceivedOn = u.ReceivedOn,
                                                     ReceivedOnWeb = u.ReceivedOnWeb,
                                                     AddedFrom = u.AddedFrom,
                                                     EditedFrom = u.EditedFrom,
                                                 }).SingleOrDefault();
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
                        strQuery += "UPDATE TransferInOutQtyDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='We WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                return true;
            }
        }

        public int GetTransferDetailRecordCount(Guid TransferGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@TransferGUID", TransferGUID)
                                                };
                return context.ExecuteStoreQuery<int>("exec [GetTransferDetailRecordCount] @TransferGUID ", params1).FirstOrDefault();
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
        public ResponseMessage ReceiveQuantity(Guid ItemGUID, Guid DetailGUID, Int64 LocationID, Int64 RoomID, Int64 CompanyID, Int64 UserID, List<long> SupplierIds, string RoomDateFormat, long SessionUserId)
        {
            ResponseMessage response = new ResponseMessage();
            TransferDetailDAL objDetailDAL = new TransferDetailDAL(base.DataBaseName);
            TransferMasterDAL objMasterDAL = new TransferMasterDAL(base.DataBaseName);
            TransferInOutItemDetailDAL InOutItemDetaiDAL = new TransferInOutItemDetailDAL(base.DataBaseName);
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemLocationDetailsDAL objItemLocDetailDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemMasterDTO objItem = objItemDAL.GetItemWithoutJoins(null, ItemGUID);
            TransferDetailDTO objDetailDTO = objDetailDAL.GetRecord(DetailGUID, RoomID, CompanyID);

            if (objDetailDTO.FulFillQuantity <= 0)
            {
                response.IsSuccess = false;
                response.Message = "Not enough quantity to receive.";
                return response;
            }

            TransferMasterDTO objMasterDTO = objMasterDAL.GetTransferByGuidPlain(objDetailDTO.TransferGUID);
            TransferMasterDTO objReplenishMasterDTO = null;

            if (objMasterDTO.RefTransferGUID.HasValue && objMasterDTO.RefTransferGUID != Guid.Empty)
                objReplenishMasterDTO = objMasterDAL.GetTransferByGuidPlain(objMasterDTO.RefTransferGUID.Value);
            else
                objReplenishMasterDTO = objMasterDAL.GetTransferByRefTransferGuidPlain(objMasterDTO.ReplenishingRoomID, CompanyID, objMasterDTO.GUID);

            List<TransferDetailDTO> ReplinishDetailDTOList = objDetailDAL.GetTransferedRecord(objReplenishMasterDTO.GUID, objReplenishMasterDTO.RoomID, CompanyID, SupplierIds);

            var replinishRoomItemGuid = GetSameItemByIDInReplinishRoom(ItemGUID, objReplenishMasterDTO.RoomID, CompanyID);
            TransferDetailDTO ReplenishDetailDTO = ReplinishDetailDTOList.Where(x => x.ItemGUID == replinishRoomItemGuid).SingleOrDefault();
            IEnumerable<TransferInOutQtyDetailDTO> objReplenishInOutQtyDetaiDTOList = GetTrfInOutQtyDetailByTrfDetailAndItem(ReplenishDetailDTO.GUID, CompanyID, replinishRoomItemGuid);
            MaterialStagingDTO objMSDOT = null;
            if (objMasterDTO.StagingID.GetValueOrDefault(0) > 0)
                objMSDOT = new MaterialStagingDAL(base.DataBaseName).GetRecord(objMasterDTO.StagingID.GetValueOrDefault(0), RoomID, CompanyID);

            foreach (var QtyItem in objReplenishInOutQtyDetaiDTOList)
            {
                TransferInOutQtyDetailDTO objInOutDetaiDTO = null;
                List<TransferInOutItemDetailDTO> objReplenishInOutItemDetailDTOList = InOutItemDetaiDAL.GetTrfInOutItemDetailByInOutQtyDetailGuid(QtyItem.GUID).ToList();
                List<ItemLocationDetailsDTO> objItemLocDetailDTOList = new List<ItemLocationDetailsDTO>();
                List<MaterialStagingPullDetailDTO> objItemMSPullDetailDTOList = new List<MaterialStagingPullDetailDTO>();

                foreach (var item in objReplenishInOutItemDetailDTOList)
                {
                    BinMasterDTO oBinMasterDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(LocationID, RoomID, CompanyID);
                    string BinNumber = string.Empty;
                    if (oBinMasterDTO != null)
                        BinNumber = oBinMasterDTO.BinNumber;

                    ItemLocationDetailsDTO objItemLocDetailDTO = new ItemLocationDetailsDTO()
                    {
                        BinID = LocationID,
                        CompanyID = CompanyID,
                        BinNumber = BinNumber,
                        ConsignedQuantity = item.ConsignedQuantity,
                        Cost = item.Cost,
                        Created = DateTime.Now,
                        CreatedBy = UserID,
                        CreatedByName = string.Empty,
                        CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                        DateCodeTracking = objItem.DateCodeTracking,
                        eVMISensorID = string.Empty,
                        eVMISensorPort = null,
                        ExpirationDate = item.ExpirationDate,
                        Expiration = item.ExpirationDate != null ? item.ExpirationDate.Value.ToString("MM-dd-yy") : string.Empty, //?
                        GUID = Guid.NewGuid(),
                        ID = 0,
                        IsArchived = false,
                        IsCreditPull = false,
                        IsDeleted = false,
                        ItemGUID = objItem.GUID,
                        TransferDetailGUID = objDetailDTO.GUID,
                        ItemNumber = objItem.ItemNumber,
                        ItemType = objItem.ItemType,
                        KitDetailGUID = null,
                        LastUpdatedBy = UserID,
                        LotNumber = item.LotNumber,
                        LotNumberTracking = objItem.LotNumberTracking,
                        MeasurementID = null,
                        mode = "",//?
                        OrderDetailGUID = null,
                        Received = item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("MM-dd-yy") : DateTime.Now.ToString("MM-dd-yy"),
                        ReceivedDate = item.ReceivedDate != null ? item.ReceivedDate : DateTime.Now,
                        Room = RoomID,
                        RoomName = string.Empty,
                        SerialNumber = item.SerialNumber,
                        SerialNumberTracking = objItem.SerialNumberTracking,
                        UDF1 = string.Empty,//?
                        UDF2 = string.Empty,
                        UDF3 = string.Empty,
                        UDF4 = string.Empty,
                        UDF5 = string.Empty,
                        Updated = DateTime.Now,
                        UpdatedByName = string.Empty,
                        Action = "Receive"

                    };
                    objItemLocDetailDTOList.Add(objItemLocDetailDTO);

                    if (objMasterDTO.StagingID.GetValueOrDefault(0) > 0 && objMSDOT != null)
                    {
                        MaterialStagingPullDetailDTO objItemMSPullDetailDTO = new MaterialStagingPullDetailDTO()
                        {
                            BinID = LocationID,
                            CompanyID = CompanyID,
                            BinNumber = string.Empty,
                            ConsignedQuantity = item.ConsignedQuantity,
                            ItemCost = item.Cost,
                            Created = DateTime.Now,
                            CreatedBy = UserID,
                            CreatedByName = string.Empty,
                            CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                            DateCodeTracking = objItem.DateCodeTracking,
                            Expiration = item.ExpirationDate != null ? item.ExpirationDate.Value.ToString("MM-dd-yy") : string.Empty, //?
                            GUID = Guid.NewGuid(),
                            ID = 0,
                            IsArchived = false,
                            IsDeleted = false,
                            ItemGUID = objItem.GUID,
                            //TransferDetailGUID = objDetailDTO.GUID,
                            ItemNumber = objItem.ItemNumber,
                            LastUpdatedBy = UserID,
                            LotNumber = item.LotNumber,
                            LotNumberTracking = objItem.LotNumberTracking,
                            OrderDetailGUID = null,
                            Received = item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("MM-dd-yy") : DateTime.Now.ToString("MM-dd-yy"),
                            Room = RoomID,
                            RoomName = string.Empty,
                            SerialNumber = item.SerialNumber,
                            SerialNumberTracking = objItem.SerialNumberTracking,

                            Updated = DateTime.Now,
                            UpdatedByName = string.Empty,
                            MaterialStagingGUID = objMSDOT.GUID,

                        };
                        objItemMSPullDetailDTOList.Add(objItemMSPullDetailDTO);
                    }


                }

                bool isUpdated = false;
                if (objMasterDTO.StagingID.GetValueOrDefault(0) <= 0)
                {
                    isUpdated = objItemLocDetailDAL.ItemLocationDetailsSave(objItemLocDetailDTOList, RoomDateFormat, SessionUserId);
                }
                else
                {
                    ItemMasterDAL objITEMDAL = new ItemMasterDAL(base.DataBaseName);
                    //ItemMasterDTO ItemDTO = objITEMDAL.GetRecord(ItemGUID.ToString(), RoomID, CompanyID);
                    ItemMasterDTO ItemDTO = objITEMDAL.GetItemWithoutJoins(null, ItemGUID);
                    //ItemDTO.StagedQuantity = ItemDTO.StagedQuantity.GetValueOrDefault(0) + objDetailDTO.FulFillQuantity;
                    ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(base.DataBaseName);
                    obj.ItemLocationDetailsSaveForMSCredit(objItemMSPullDetailDTOList, RoomID, CompanyID, UserID, RoomDateFormat, SessionUserId);
                    ItemDTO.WhatWhereAction = "Transfer";
                    isUpdated = objITEMDAL.Edit(ItemDTO, SessionUserId);
                }

                if (isUpdated)
                {
                    TransferInOutQtyDetailDAL oTransferInOutQtyDetailDAL = new TransferInOutQtyDetailDAL(base.DataBaseName);
                    objInOutDetaiDTO = oTransferInOutQtyDetailDAL.GetTransferInOutQtyDetailByTrfAndItem(objDetailDTO.GUID, objMasterDTO.GUID, objItem.GUID);

                    objInOutDetaiDTO.BinID = LocationID;
                    objInOutDetaiDTO.IsArchived = false;
                    objInOutDetaiDTO.IsDeleted = false;
                    objInOutDetaiDTO.LastUpdatedBy = UserID;
                    objInOutDetaiDTO.Updated = DateTime.Now;
                    objInOutDetaiDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objInOutDetaiDTO.EditedFrom = "Web";
                    EditLocation(objInOutDetaiDTO);

                    objDetailDTO.ReceivedQuantity = objDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + objDetailDTO.FulFillQuantity;
                    objDetailDTO.FulFillQuantity = 0;
                    objDetailDTO.ShippedQuantity = 0;
                    objDetailDTO.IntransitQuantity = 0;
                    objDetailDTO.LastUpdatedBy = UserID;
                    objDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDetailDTO.EditedFrom = "Web";
                    objDetailDAL.Edit(objDetailDTO, SessionUserId);

                    ReplenishDetailDTO.ReceivedQuantity = ReplenishDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + ReplenishDetailDTO.FulFillQuantity;
                    ReplenishDetailDTO.FulFillQuantity = 0;
                    ReplenishDetailDTO.ShippedQuantity = 0;
                    ReplenishDetailDTO.IntransitQuantity = 0;
                    ReplenishDetailDTO.LastUpdatedBy = UserID;
                    ReplenishDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    ReplenishDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    ReplenishDetailDTO.EditedFrom = "Web";
                    objDetailDAL.Edit(ReplenishDetailDTO, SessionUserId);


                }

                QtyItem.TrfInOutItemDetail = objReplenishInOutItemDetailDTOList;
                QtyItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                QtyItem.EditedFrom = "Web";
                EditLocation(QtyItem);

            }

            #region //Comment Code
            //TransferMasterDTO objMasterDTO = objMasterDAL.GetRecord(objDetailDTO.TransferGUID, RoomID, CompanyID);
            //IEnumerable<TransferInOutQtyDetailDTO> objInOutQtyDetaiDTOList = GetRecordByTransferDetail(objDetailDTO.GUID, objDetailDTO.Room, CompanyID, false).Where(x => x.ItemGUID == ItemGUID);
            //foreach (var QtyItem in objInOutQtyDetaiDTOList)
            //{
            //    //TransferInOutQtyDetailDTO objInOutDetaiDTO = null;
            //    List<TransferInOutItemDetailDTO> UpdateInOutItemDetailDTOList = new List<TransferInOutItemDetailDTO>();

            //    List<TransferInOutItemDetailDTO> objInOutItemDetailDTOList = InOutItemDetaiDAL.GetRecordByTransferInOutQtyDetailID(QtyItem.GUID, QtyItem.Room, CompanyID, false).ToList();
            //    List<ItemLocationDetailsDTO> objItemLocDetailDTOList = new List<ItemLocationDetailsDTO>();

            //    QtyItem.BinID = LocationID;
            //    QtyItem.Updated = DateTime.Now;
            //    QtyItem.LastUpdatedBy = UserID;
            //    foreach (var item in objInOutItemDetailDTOList)
            //    {
            //        item.BinID = LocationID;
            //        item.Updated = DateTime.Now;
            //        item.LastUpdatedBy = UserID;
            //        item.ReceivedDate = DateTime.Now;
            //        UpdateInOutItemDetailDTOList.Add(item);

            //        //objInOutItemDetailDTOList.Add(obj);

            //        ItemLocationDetailsDTO objItemLocDetailDTO = new ItemLocationDetailsDTO()
            //        {
            //            BinID = LocationID,
            //            CompanyID = CompanyID,
            //            BinNumber = string.Empty,
            //            ConsignedQuantity = item.ConsignedQuantity,
            //            Cost = item.Cost,
            //            Created = DateTime.Now,
            //            CreatedBy = UserID,
            //            CreatedByName = string.Empty,
            //            CustomerOwnedQuantity = item.CustomerOwnedQuantity,
            //            DateCodeTracking = Convert.ToBoolean(objItem.DateCodeTracking),
            //            eVMISensorID = string.Empty,
            //            eVMISensorPort = null,
            //            ExpirationDate = item.ExpirationDate,
            //            //Expiration = item.ExpirationDate != null ? item.ExpirationDate.ToString("MM-dd-yy") : string.Empty,
            //            Expiration = item.ExpirationDate != null ? item.ExpirationDate.Value.ToString("MM-dd-yy") : string.Empty, //?
            //            GUID = Guid.NewGuid(),
            //            ID = 0,
            //            IsArchived = false,
            //            IsCreditPull = false,
            //            IsDeleted = false,
            //            ItemGUID = objItem.GUID,
            //            TransferDetailGUID = objDetailDTO.GUID,
            //            ItemNumber = objItem.ItemNumber,
            //            ItemType = Convert.ToInt32(objItem.ItemType),
            //            KitDetailGUID = null,
            //            LastUpdatedBy = UserID,
            //            LotNumber = item.LotNumber,
            //            LotNumberTracking = Convert.ToBoolean(objItem.LotNumberTracking),
            //            MeasurementID = null,
            //            mode = "",//?
            //            OrderDetailGUID = null,
            //            //Received = item.ReceivedDate != null ? item.ReceivedDate.ToString("MM-dd-yy") : string.Empty,
            //            Received = item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("MM-dd-yy") : DateTime.Now.ToString("MM-dd-yy"),
            //            ReceivedDate = item.ReceivedDate,
            //            Room = RoomID,
            //            RoomName = string.Empty,
            //            SerialNumber = item.SerialNumber,
            //            SerialNumberTracking = Convert.ToBoolean(objItem.SerialNumberTracking),
            //            UDF1 = string.Empty,
            //            UDF2 = string.Empty,
            //            UDF3 = string.Empty,
            //            UDF4 = string.Empty,
            //            UDF5 = string.Empty,
            //            UDF6 = string.Empty,
            //            UDF7 = string.Empty,
            //            UDF8 = string.Empty,
            //            UDF9 = string.Empty,
            //            UDF10 = string.Empty,
            //            Updated = DateTime.Now,
            //            UpdatedByName = string.Empty

            //        };
            //        objItemLocDetailDTOList.Add(objItemLocDetailDTO);

            //    }
            //    bool isUpdated = false;
            //    if (objMasterDTO.StagingID.GetValueOrDefault(0) <= 0)
            //    {
            //        isUpdated = objItemLocDetailDAL.ItemLocationDetailsSave(objItemLocDetailDTOList);
            //    }
            //    else
            //    {
            //        //ItemMasterDAL objITEMDAL = new ItemMasterDAL(base.DataBaseName);
            //        //ItemMasterDTO ItemDTO = objITEMDAL.GetRecord(ItemGUID.ToString(), RoomID, CompanyID);
            //        ////ItemDTO.StagedQuantity = ItemDTO.StagedQuantity.GetValueOrDefault(0) + objDetailDTO.FulFillQuantity;
            //        //ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(base.DataBaseName);
            //        //obj.ItemLocationDetailsSaveForMSCredit(objItemMSPullDetailDTOList, RoomID, CompanyID, UserID);
            //        //ItemDTO.WhatWhereAction = "Transfer";
            //        //isUpdated = objITEMDAL.Edit(ItemDTO);
            //        objItem.StagedQuantity = objItem.StagedQuantity.GetValueOrDefault(0) + objDetailDTO.FulFillQuantity;
            //        isUpdated = objItemDAL.Edit(objItem);
            //    }

            //    if (isUpdated)
            //    {
            //        QtyItem.TrfInOutItemDetail = UpdateInOutItemDetailDTOList;

            //        objDetailDTO.ReceivedQuantity = objDetailDTO.ReceivedQuantity + objDetailDTO.FulFillQuantity;
            //        objDetailDTO.FulFillQuantity = 0;
            //        objDetailDTO.ShippedQuantity = 0;
            //        objDetailDTO.IntransitQuantity = 0;
            //        objDetailDTO.LastUpdatedBy = UserID;
            //        objDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            //        objDetailDAL.Edit(objDetailDTO);

            //    }// end if

            //    UpdateIsTransferd(QtyItem);
            //} // end foreach objInOutQtyDetailDTOList

            #endregion

            response.IsSuccess = true;
            response.Message = "Quantity Received.";
            return response;
        }
    }
}
