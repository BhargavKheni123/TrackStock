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
using System.Data.SqlClient;
using System.Globalization;

namespace eTurns.DAL
{
    public partial class ReceivedOrderTransferDetailDAL : eTurnsBaseDAL
    {
        string stryQry = "EXEC [GetReceivedOrderTransferDetail] @RoomID,@CompanyID,@OrderGuid,@OrderDetailGuid,@ItemGuid,@BinID,@IsDeleted,@IsArchived,@ROTDGuid";


        public IEnumerable<ReceivedOrderTransferDetailDTO> GetAllRecordsRoomAndCompanyWise(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            IEnumerable<ReceivedOrderTransferDetailDTO> lst = GetAllRecordsRoomAndCompanyWise(RoomID, CompanyID).Where(x => x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false);
            return lst;

        }


        public void UnCloseEdit(List<ReceivedOrderTransferDetailDTO> ROTDs)
        {

            if (ROTDs == null || ROTDs.Count <= 0)
                return;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ReceivedOrderTransferDetail objROTD = null;
                ItemLocationDetail objILD = null;
                MaterialStagingPullDetail objMSPullDetail = null;
                OrderDetail objOD = null;
                OrderDetailsDTO objOrdDtlOD = null;

                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                OrderDetailsDAL objOrdDtlDAL = new OrderDetailsDAL(base.DataBaseName);
                ItemLocationQTYDAL objLocQtyDAL = null;
                double PrevReceivedCustOwnQty = 0;
                double PrevReceivedConsQty = 0;
                List<ItemLocationQTYDTO> objListQty = new List<ItemLocationQTYDTO>();

                foreach (var objNewDTO in ROTDs)
                {
                    try
                    {
                        ItemMasterDTO objItemMasterDTO = objItemDAL.GetRecordByItemGUID(Guid.Parse(objNewDTO.ItemGUID.ToString()), Convert.ToInt64(objNewDTO.Room), Convert.ToInt64(objNewDTO.CompanyID));
                        objOrdDtlOD = objOrdDtlDAL.GetRecord(objNewDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0));

                        OrderDetail od = context.OrderDetails.FirstOrDefault<OrderDetail>(x => x.GUID == (objNewDTO.OrderDetailGUID ?? Guid.Empty));
                        OrderMaster om = context.OrderMasters.FirstOrDefault(x => x.GUID == (od.OrderGUID ?? Guid.Empty));
                        //Step 1: SET ReceivedOrderTransferDetail Table record
                        objROTD = context.ReceivedOrderTransferDetails.FirstOrDefault(x => x.GUID == objNewDTO.GUID);
                        PrevReceivedCustOwnQty = objROTD.CustomerOwnedQuantity.GetValueOrDefault(0);
                        PrevReceivedConsQty = objROTD.ConsignedQuantity.GetValueOrDefault(0);
                        objROTD = SetRecieveOrderTransferDetail(objROTD, objNewDTO);
                        // Set OrderUOM QTY
                        if (objOrdDtlOD.OrderUOMValue == null || objOrdDtlOD.OrderUOMValue <= 0)
                            objOrdDtlOD.OrderUOMValue = 1;

                        if (objItemMasterDTO.SerialNumberTracking == false && objItemMasterDTO.IsAllowOrderCostuom && om.OrderType == (int)OrderType.Order)
                        {
                            if (objROTD.ConsignedQuantity != null && objROTD.ConsignedQuantity >= 0)
                                objROTD.ConsignedQuantity = objROTD.ConsignedQuantity * objOrdDtlOD.OrderUOMValue;

                            if (objROTD.CustomerOwnedQuantity != null && objROTD.CustomerOwnedQuantity >= 0)
                                objROTD.CustomerOwnedQuantity = objROTD.CustomerOwnedQuantity * objOrdDtlOD.OrderUOMValue;
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

                            objILD = objILDDAL.SetItemLocationDetail(objILD, objNewDTO, PrevReceivedCustOwnQty, PrevReceivedConsQty, (OrderType)om.OrderType, IsBinDelete);

                            // Set OrderUOM QTY
                            if (objItemMasterDTO.SerialNumberTracking == false)
                            {
                                if (objILD.ConsignedQuantity != null && objILD.ConsignedQuantity >= 0 && objItemMasterDTO.IsAllowOrderCostuom && om.OrderType == (int)OrderType.Order)
                                    objILD.ConsignedQuantity = objILD.ConsignedQuantity * objOrdDtlOD.OrderUOMValue;

                                if (objILD.CustomerOwnedQuantity != null && objILD.CustomerOwnedQuantity >= 0 && objItemMasterDTO.IsAllowOrderCostuom && om.OrderType == (int)OrderType.Order)
                                    objILD.CustomerOwnedQuantity = objILD.CustomerOwnedQuantity * objOrdDtlOD.OrderUOMValue;

                                objILD.InitialQuantity = (objILD.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objILD.ConsignedQuantity.GetValueOrDefault(0));

                                if (objILD.InitialQuantityWeb.GetValueOrDefault(0) > 0)
                                    objILD.InitialQuantityWeb = (objILD.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objILD.ConsignedQuantity.GetValueOrDefault(0));
                                else if (objILD.InitialQuantityPDA.GetValueOrDefault(0) > 0)
                                    objILD.InitialQuantityPDA = (objILD.CustomerOwnedQuantity.GetValueOrDefault(0)) + (objILD.ConsignedQuantity.GetValueOrDefault(0));
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

                        ////Step 3: Delete & Insert ItemLocationQty

                        //objLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                        //objListQty = objLocQtyDAL.GetListOfItemLocQty(objNewDTO.ItemGUID ?? Guid.Empty, objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0), objNewDTO.LastUpdatedBy.GetValueOrDefault(0));

                        //if (objListQty != null && objListQty.Count() > 0)
                        //{
                        //    objLocQtyDAL.Save(objListQty);
                        //}
                        if (objILD != null)
                        {
                            IEnumerable<ItemLocationQTY> objItemLocationQtyDelete = context.ItemLocationQTies.Where(x => x.ItemGUID == objNewDTO.ItemGUID);
                            foreach (var item in objItemLocationQtyDelete)
                            {
                                context.ItemLocationQTies.DeleteObject(item);
                            }

                            IEnumerable<ItemLocationQTY> objItemLocationQtyAdd = GetItemLocationQty(objNewDTO, context);
                            foreach (var item in objItemLocationQtyAdd)
                            {
                                context.ItemLocationQTies.AddObject(item);
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
                        //////objOD.ReceivedQuantity = context.ReceivedOrderTransferDetails.Where(x => x.OrderDetailGUID == objNewDTO.OrderDetailGUID && !(x.IsDeleted ?? false) && !(x.IsArchived ?? false)).Sum(x => ((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)));
                        objOD.LastUpdated = DateTimeUtility.DateTimeNow;
                        objOD.LastUpdatedBy = objNewDTO.LastUpdatedBy;
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

                            ////if (objOD.ReceivedQuantity != null && objOD.ReceivedQuantity >= 0)
                            ////{
                            ////    if (objItemMasterDTO.IsAllowOrderCostuom && om.OrderType == (int)OrderType.Order)
                            ////    {
                            ////        objOD.ReceivedQuantityUOM = objOD.ReceivedQuantity / objOrdDtlOD.OrderUOMValue;
                            ////        if (objOD.ReceivedQuantityUOM > 0 && objOD.ReceivedQuantityUOM < 1)
                            ////            objOD.ReceivedQuantityUOM = 0;
                            ////    }
                            ////    else
                            ////    {
                            ////        objOD.ReceivedQuantityUOM = objOD.ReceivedQuantity;
                            ////    }
                            ////}
                        }
                        objOD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        context.SaveChanges();

                        //Step 5: Update Order status
                        OrderDetailsDAL ordDetailDAL = new DAL.OrderDetailsDAL(base.DataBaseName);
                        //ordDetailDAL.UpdateOrderStatusByReceive(objOD.OrderGUID.GetValueOrDefault(Guid.Empty), objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0), objNewDTO.LastUpdatedBy.GetValueOrDefault(0), objNewDTO.IsOnlyFromUI);


                        //Step 7: Update onorder qtyt
                        string queryToUpdateOnOrderQty = "EXEC [Item_UpdateItemOnOrderQuantity]  @ItemGUID,@Room,@CompanyID,@LastUpdatedBy";
                        var params1 = new SqlParameter[] {
                                        new SqlParameter("@ItemGUID", objNewDTO.ItemGUID.GetValueOrDefault(Guid.Empty)),
                                        new SqlParameter("@Room", objNewDTO.Room.GetValueOrDefault(0)),
                                        new SqlParameter("@CompanyID", objNewDTO.CompanyID.GetValueOrDefault(0)),
                                        new SqlParameter("@LastUpdatedBy", objNewDTO.LastUpdatedBy.GetValueOrDefault(0))
                                        };

                        var rslt = context.ExecuteStoreQuery<object>(queryToUpdateOnOrderQty, params1);

                        //Step 8: Update Cost mark up cell price and update cart for edited item.
                        UpdateItemMaster(objNewDTO, context);

                        //objItemDAL = new ItemMasterDAL(base.DataBaseName);
                        //objItemDAL.UpdateCostMarkupSellPrice(objNewDTO.ItemGUID ?? Guid.Empty, objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0));

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

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReceivedOrderTransferDetailDTO> GetCachedData(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@OrderGuid", DBNull.Value),
                    new SqlParameter("@OrderDetailGuid", DBNull.Value),
                    new SqlParameter("@ItemGuid", ItemGUID),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", DBNull.Value),
                    new SqlParameter("@IsDeleted", DBNull.Value),
                    new SqlParameter("@ROTDGuid", DBNull.Value)
                };
                IEnumerable<ReceivedOrderTransferDetailDTO> obj = context.ExecuteStoreQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();
                return obj;
            }

        }

        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedOrderTransferDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            //Get Cached-Media
            IEnumerable<ReceivedOrderTransferDetailDTO> ObjCache = GetCachedData(ItemGUID, RoomID, CompanyID);
            IEnumerable<ReceivedOrderTransferDetailDTO> ObjGlobalCache = null;

            if (OrderDetailGUID == null)
            {
                ObjGlobalCache = ObjCache;
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.ItemGUID == ItemGUID));
            }
            else
            {
                ObjGlobalCache = ObjCache.Where(t => t.OrderDetailGUID == OrderDetailGUID);
                ObjCache = ObjCache.Where(t => (t.IsArchived.GetValueOrDefault(false) == false && t.IsDeleted.GetValueOrDefault(false) == false && t.OrderDetailGUID == OrderDetailGUID));
            }

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
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ReceivedOrderTransferDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ReceivedOrderTransferDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }


        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedOrderTransferDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ReceivedOrderTransferDetailDTO> result = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] objParams = new SqlParameter[] {
                    new SqlParameter("@RoomID",RoomID),
                    new SqlParameter("@CompanyID", CompanyId),
                    new SqlParameter("@OrderGuid", DBNull.Value),
                    new SqlParameter("@OrderDetailGuid", OrderDetailGUID??(object)DBNull.Value),
                    new SqlParameter("@ItemGuid", ItemGUID),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", false),
                    new SqlParameter("@IsDeleted", false),
                    new SqlParameter("@ROTDGuid", DBNull.Value)
                };

                result = context.ExecuteStoreQuery<ReceivedOrderTransferDetailDTO>(stryQry, objParams).OrderBy(ColumnName).ToList();
                return result;
            }

        }


        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedOrderTransferDetailDTO> GetAllRecordByOrderMasterGuid(Int64 RoomId, Int64 CompanyID, Guid OderGuid, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ReceivedOrderTransferDetailDTO> result = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] objParams = new SqlParameter[] {
                    new SqlParameter("@RoomID",RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@OrderGuid", OderGuid),
                    new SqlParameter("@OrderDetailGuid",DBNull.Value),
                    new SqlParameter("@ItemGuid", DBNull.Value),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", false),
                    new SqlParameter("@IsDeleted", false),
                    new SqlParameter("@ROTDGuid", DBNull.Value)
                };

                result = context.ExecuteStoreQuery<ReceivedOrderTransferDetailDTO>(stryQry, objParams).OrderBy(ColumnName).ToList();
                return result;
            }


        }


        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedOrderTransferDetailDTO> GetAllRecordByOrderDetailGuid(Int64 RoomId, Int64 CompanyID, Guid OrderDetailGuid, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ReceivedOrderTransferDetailDTO> result = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] objParams = new SqlParameter[] {
                    new SqlParameter("@RoomID",RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@OrderGuid", DBNull.Value),
                    new SqlParameter("@OrderDetailGuid",OrderDetailGuid),
                    new SqlParameter("@ItemGuid", DBNull.Value),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", false),
                    new SqlParameter("@IsDeleted", false),
                    new SqlParameter("@ROTDGuid", DBNull.Value)
                };

                result = context.ExecuteStoreQuery<ReceivedOrderTransferDetailDTO>(stryQry, objParams).OrderBy(ColumnName).ToList();
                return result;
            }


        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReceivedOrderTransferDetailDTO> GetAllRecordsRoomAndCompanyWise(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@OrderGuid", DBNull.Value),
                    new SqlParameter("@OrderDetailGuid", DBNull.Value),
                    new SqlParameter("@ItemGuid", DBNull.Value),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", DBNull.Value),
                    new SqlParameter("@IsDeleted", DBNull.Value),
                    new SqlParameter("@ROTDGuid", DBNull.Value)
                };
                IEnumerable<ReceivedOrderTransferDetailDTO> obj = context.ExecuteStoreQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).ToList();
                return obj;


            }
        }

        /// <summary>
        /// Get Particullar Record from the ReceivedOrderTransferDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ReceivedOrderTransferDetailDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@OrderGuid", DBNull.Value),
                    new SqlParameter("@OrderDetailGuid", DBNull.Value),
                    new SqlParameter("@ItemGuid", DBNull.Value),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", DBNull.Value),
                    new SqlParameter("@IsDeleted", DBNull.Value),
                    new SqlParameter("@ROTDGuid", GUID)
                };
                ReceivedOrderTransferDetailDTO obj = context.ExecuteStoreQuery<ReceivedOrderTransferDetailDTO>(stryQry, params1).FirstOrDefault();
                return obj;
            }
            /*
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ReceivedOrderTransferDetailDTO>(@"SELECT A.*, I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, E.ItemNumber,E.Consignment,E.ItemType,
                    E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber 
                    FROM ReceivedOrderTransferDetail A 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID LEFT OUTER JOIN BinMaster I on A.BinID = I.ID AND A.ItemGUID = I.ItemGUID
                    WHERE A.GUID = '" + GUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString())
                        select new ReceivedOrderTransferDetailDTO
                        {
                            ID = u.ID,

                            BinID = u.BinID,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            MeasurementID = u.MeasurementID,
                            LotNumber = u.LotNumber,
                            SerialNumber = u.SerialNumber,
                            ExpirationDate = u.ExpirationDate,
                            ReceivedDate = u.ReceivedDate,
                            Expiration = u.Expiration == null ? DateTime.Now.ToString("MM/dd/yy") : u.Expiration,
                            Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                            Cost = u.Cost,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
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
                            TransferDetailGUID = u.TransferDetailGUID,
                            KitDetailGUID = u.KitDetailGUID,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            ItemLocationDetailGUID = u.ItemLocationDetailGUID,
                            PackSlipNumber = u.PackSlipNumber,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            IsEDISent = u.IsEDISent,
                            LastEDIDate = u.LastEDIDate,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            ControlNumber = u.ControlNumber
                        }).FirstOrDefault();

            }
            */
        }
    }
}
