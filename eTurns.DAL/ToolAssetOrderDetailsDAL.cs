using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ToolAssetOrderDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ToolAssetOrderDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolAssetOrderDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Get Paged Records from the OrderDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ToolAssetOrderDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return DB_GetCachedData(RoomID, CompanyId, null, null, null, null, null);
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolAssetOrderDetailsDTO GetRecord(Guid Guid, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetCachedData(RoomID, CompanyID, null, null, null, Guid, null).FirstOrDefault();
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolAssetOrderDetailsDTO GetRecord(Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetCachedData(RoomID, CompanyID, null, null, ID, null, null).FirstOrDefault();
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<ToolAssetOrderDetailsDTO> GetOrderedRecord(Guid OrderGuid, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetCachedData(RoomID, CompanyID, null, null, null, null, OrderGuid).ToList();
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<ToolAssetOrderDetailsDTO> GetOrderedRecord(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return DB_GetCachedData(RoomID, CompanyID, IsDeleted, IsArchived, null, null, OrderGuid).ToList();
        }

        /// <summary>
        /// Insert Record in the DataBase OrderDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public ToolAssetOrderDetailsDTO Insert(ToolAssetOrderDetailsDTO objDTO)
        {
            return DB_Insert(objDTO);

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ToolAssetOrderDetailsDTO objDTO)
        {
            objDTO = DB_Update(objDTO);
            return true;
        }

        public bool UpdateLineItemBin(ToolAssetOrderDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetOrderDetail objOD = context.ToolAssetOrderDetails.FirstOrDefault(x => x.GUID == objDTO.GUID);
                objOD.ToolBinID = objDTO.ToolBinID;
                objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.EditedFrom = "Web-BinSave";
                context.SaveChanges();
                return true;
            }
        }

        public bool UpdateLineComment(ToolAssetOrderDetailsDTO objDTO, Int64 UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetOrderDetail objODNew = null;
                if (objDTO.ID > 0)
                {
                    objODNew = context.ToolAssetOrderDetails.FirstOrDefault(x => x.ID == objDTO.ID);
                }
                else
                {
                    objODNew = context.ToolAssetOrderDetails.FirstOrDefault(x => x.GUID == objDTO.GUID);
                }
                if (objODNew != null)
                {
                    objODNew.Comment = objDTO.Comment;
                    objODNew.UDF1 = objDTO.UDF1;
                    objODNew.UDF2 = objDTO.UDF2;
                    objODNew.UDF3 = objDTO.UDF3;
                    objODNew.UDF4 = objDTO.UDF4;
                    objODNew.UDF5 = objDTO.UDF5;
                    objODNew.LastUpdatedBy = UserID;
                    objODNew.LastUpdated = DateTimeUtility.DateTimeNow;
                    objODNew.EditedFrom = "Web";
                    context.SaveChanges();
                }
                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID)
        {
            return DB_DeleteRecords(IDs, userid, RoomId, CompanyID);
        }

        public double GetOrderdQtyOfItem(Guid ItmGUID)
        {
            //double retValReq = 0;
            double retValApprv = 0;
            double retValRec = 0;
            double retVal = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {


                List<ToolAssetOrderDetail> obj = (from x in context.ToolAssetOrderDetails
                                                  join m in context.ToolAssetOrderMasters on (x.ToolAssetOrderGUID ?? Guid.Empty) equals m.GUID
                                                  where ((x.ToolGUID ?? Guid.Empty) == ItmGUID || (x.AssetGUID ?? Guid.Empty) == ItmGUID)
                                                  && ((x.RequestedQuantity ?? 0) - (x.ReceivedQuantity ?? 0)) > 0

                                                  && m.OrderStatus >= (int)ToolAssetOrderStatus.Approved
                                                  && m.OrderStatus < (int)ToolAssetOrderStatus.Closed
                                                  select x).ToList();

                List<ToolAssetOrderDetail> objList = new List<ToolAssetOrderDetail>();
                ToolAssetOrderMasterDAL objOrdDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);

                retValApprv = obj.Where(t => (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false && (t.ApprovedQuantity ?? 0) > 0).Sum(t => (t.ApprovedQuantity ?? 0));
                retValRec = obj.Where(t => (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false && (t.ReceivedQuantity ?? 0) > 0).Sum(t => (t.ReceivedQuantity ?? 0));

                retVal = retValApprv - retValRec;
                if (retVal <= 0)
                    retVal = 0;
            }
            return retVal;

        }

        public ToolAssetOrderMasterDTO Update_And_Return_Order_NoOfItem_And_OrderCost(Guid OrderGuid)
        {
            Int64 CompanyID = 0;
            Int64 RoomID = 0;
            int NoOfItem = 0;
            double OrderPrice = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ToolAssetOrderDetail> obj = context.ToolAssetOrderDetails.Where(x => (x.ToolAssetOrderGUID ?? Guid.Empty) == OrderGuid && (x.IsDeleted ?? false) == false && (x.IsArchived ?? false) == false).ToList();
                if (obj != null && obj.Count > 0)
                {
                    RoomID = obj[0].RoomID.GetValueOrDefault(0);
                    CompanyID = obj[0].CompanyID.GetValueOrDefault(0);
                    NoOfItem = obj.Count;
                    foreach (var item in obj)
                    {
                        if (item.ToolGUID.HasValue)
                        {
                            double? Itemcost = 0;// new ToolMasterDAL(base.DataBaseName).CalculateAndGetItemCost(item.ToolGUID.Value, (double)((context.ItemMasters.Where(x => x.GUID == (item.ItemGUID ?? Guid.Empty)).FirstOrDefault()).Cost ?? 0), RoomID, CompanyID);
                            OrderPrice = OrderPrice + (Itemcost.GetValueOrDefault(0) * Convert.ToDouble(item.RequestedQuantity ?? 0));
                        }
                    }
                }
            }

            ToolAssetOrderMasterDAL objOMDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);
            ToolAssetOrderMasterDTO objOrderDTO = objOMDAL.GetRecord(OrderGuid, RoomID, CompanyID);
            objOrderDTO.NoOfLineItems = NoOfItem;
            objOrderDTO.OrderCost = OrderPrice;

            return objOrderDTO;

        }

        /// <summary>
        /// UpdateOrderStatusByReceive
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public bool UpdateOrderStatusByReceive(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID, bool IsOnlyFromUI = false)
        {
            ToolAssetOrderMasterDAL ordMasterDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);
            ToolAssetOrderMasterDTO ordMasterDTO = ordMasterDAL.GetRecord(OrderGuid, RoomID, CompanyID);
            if (ordMasterDTO.OrderStatus > (int)ToolAssetOrderStatus.Approved && ordMasterDTO.OrderStatus <= (int)ToolAssetOrderStatus.Closed)
            {
                List<ToolAssetOrderDetailsDTO> lstOrdDetailRecords = GetOrderedRecord(OrderGuid, RoomID, CompanyID);
                if (lstOrdDetailRecords != null && lstOrdDetailRecords.Count > 0)
                {
                    bool IsAllItemNotReceived = false;

                    foreach (var item in lstOrdDetailRecords)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > item.ReceivedQuantity.GetValueOrDefault(0) && !item.IsCloseTool.GetValueOrDefault(false))
                        {
                            IsAllItemNotReceived = true;
                            break;
                        }
                    }

                    if (IsAllItemNotReceived)
                    {
                        List<ReceivedToolAssetOrderTransferDetailDTO> lstAllLocation = new List<ReceivedToolAssetOrderTransferDetailDTO>();
                        foreach (var item in lstOrdDetailRecords)
                        {
                            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> lstLocation = new ReceivedToolAssetOrderTransferDetailDAL(base.DataBaseName).GetAllRecordByOrderDetailGuid(RoomID, CompanyID, item.GUID, "ID DESC");
                            lstAllLocation.AddRange(lstLocation);
                        }
                        List<DateTime> lstReceivedDates = new List<DateTime>();
                        if (lstAllLocation != null && lstAllLocation.Count > 0)
                        {
                            for (int i = 0; i < lstAllLocation.Count; i++)
                            {
                                if (lstAllLocation[i].ReceivedDate.HasValue)
                                {
                                    lstReceivedDates.Add(lstAllLocation[i].ReceivedDate.Value);
                                }
                            }
                        }
                        DateTime MaxReceiveDate = DateTime.Now;

                        if (lstReceivedDates != null && lstReceivedDates.Count > 0)
                            MaxReceiveDate = lstReceivedDates.Max();

                        bool IsRequeredDateIsPast = false;

                        foreach (var item in lstOrdDetailRecords)
                        {
                            if (item.RequiredDate.GetValueOrDefault(DateTime.MinValue) < MaxReceiveDate && !item.IsCloseTool.GetValueOrDefault(false))
                            {
                                IsRequeredDateIsPast = true;
                                break;
                            }
                        }

                        if (IsRequeredDateIsPast)
                        {
                            ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.TransmittedInCompletePastDue;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            if (IsOnlyFromUI)
                            {
                                ordMasterDTO.EditedFrom = "Web";
                                ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            ordMasterDAL.Edit(ordMasterDTO);
                        }
                        else
                        {
                            ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.TransmittedIncomplete;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            if (IsOnlyFromUI)
                            {
                                ordMasterDTO.EditedFrom = "Web";
                                ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            ordMasterDAL.Edit(ordMasterDTO);

                        }

                    }
                    else
                    {
                        if (ordMasterDTO.OrderStatus != (int)ToolAssetOrderStatus.Closed)
                        {
                            ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.Closed;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            if (IsOnlyFromUI)
                            {
                                ordMasterDTO.EditedFrom = "Web";
                                ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            ordMasterDAL.Edit(ordMasterDTO);
                        }

                    }
                }
            }
            return true;
        }

        /// <summary>
        /// UpdateOrderStatusByReceiveNew
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <param name="IsOnlyFromUI"></param>
        /// <returns></returns>
        public ToolAssetOrderStatus UpdateOrderStatusByReceiveNew(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID, bool IsOnlyFromUI = false)
        {
            ToolAssetOrderMasterDAL ordMasterDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);

            ToolAssetOrderMasterDTO ordMasterDTO = ordMasterDAL.GetRecord(OrderGuid, RoomID, CompanyID);

            List<ReceivedOrderTransferDetailDTO> lstAllReceived = new List<ReceivedOrderTransferDetailDTO>();
            if (ordMasterDTO.OrderStatus > (int)ToolAssetOrderStatus.Approved && ordMasterDTO.OrderStatus <= (int)ToolAssetOrderStatus.Closed)
            {

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ToolAssetOrderDetail> lstOrdDetails = context.ToolAssetOrderDetails.Where(x => x.ToolAssetOrderGUID == OrderGuid && !(x.IsCloseTool ?? false));
                    if (lstOrdDetails != null && lstOrdDetails.Count() > 0)
                    {
                        IEnumerable<ToolAssetOrderDetail> lstIncompleteOrdDetails = lstOrdDetails.Where(x => (x.ApprovedQuantity ?? 0) > (x.ReceivedQuantity ?? 0));

                        if (lstIncompleteOrdDetails.Count() > 0)
                        {
                            DateTime MaxReceiveDate = DateTime.Now;
                            DateTime MaxRequiredDate = DateTime.Now;

                            IEnumerable<Guid> lstOrderDetailsGuid = lstOrdDetails.Select(x => x.GUID);
                            MaxRequiredDate = lstIncompleteOrdDetails.Select(x => (x.RequiredDate ?? DateTime.MinValue)).Max();                            
                            string strOrderDetailGuids = string.Join(",", lstOrderDetailsGuid.ToArray());
                            var params1 = new SqlParameter[] { new SqlParameter("@OrderDetailGUIDs", strOrderDetailGuids) };

                            //DateTime? rcvdate = context.Database.SqlQuery<DateTime?>(@"SELECT Max(ReceivedDate) FROM ReceivedOrderTransferDetail 
                            //                                                           WHERE ReceivedDate IS NOT NULL AND ISNULL(IsDeleted,0)=0 AND ISNULL(IsArchived,0)=0 
                            //                                                           AND OrderDetailGUID IN (SELECT SplitValue FROM dbo.split('" + strOrderDetailGuids + @"',','))").FirstOrDefault();
                            string strQRY = "EXEC GetMaxReceiveDateFromOrderDetail @OrderDetailGUIDs";
                            DateTime? rcvdate = context.Database.SqlQuery<DateTime?>(strQRY, params1).FirstOrDefault();

                            if (rcvdate.HasValue)
                                MaxReceiveDate = rcvdate.GetValueOrDefault(DateTime.MinValue);

                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.EditedFrom = "";
                            if (IsOnlyFromUI)
                                ordMasterDTO.EditedFrom = "Web";

                            if (MaxRequiredDate < MaxReceiveDate && ordMasterDTO.OrderStatus != (int)ToolAssetOrderStatus.TransmittedInCompletePastDue)
                            {
                                ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.TransmittedInCompletePastDue;
                                ordMasterDAL.Edit(ordMasterDTO);
                            }
                            else if (MaxRequiredDate > MaxReceiveDate && ordMasterDTO.OrderStatus != (int)ToolAssetOrderStatus.TransmittedIncomplete)
                            {
                                ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.TransmittedIncomplete;
                                ordMasterDAL.Edit(ordMasterDTO);
                            }
                        }
                        else if (ordMasterDTO.OrderStatus != (int)ToolAssetOrderStatus.Closed)
                        {
                            ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.Closed;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.EditedFrom = "";
                            if (IsOnlyFromUI)
                                ordMasterDTO.EditedFrom = "Web";

                            ordMasterDAL.Edit(ordMasterDTO);
                        }
                    }
                }
            }
            return (ToolAssetOrderStatus)ordMasterDTO.OrderStatus;
        }

        #region DB

        /// <summary>
        /// From DB
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        private IEnumerable<ToolAssetOrderDetailsDTO> DB_GetCachedData(Int64? RoomID, Int64? CompanyID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? DetailGuid, Guid? OrderGUID)
        {
            string strCommand = " EXEC [GetToolAssetOrderDetailData] ";

            if (CompanyID.GetValueOrDefault(0) > 0)
                strCommand += CompanyID.Value;
            else
                strCommand += ",null";

            if (RoomID.GetValueOrDefault(0) > 0)
                strCommand += "," + RoomID.Value;
            else
                strCommand += "null";

            if (IsDeleted.HasValue)
            {
                strCommand += "," + (IsDeleted.Value ? 1 : 0);
            }
            else
            {
                strCommand += ",null";
            }

            if (IsArchived.HasValue)
            {
                strCommand += "," + (IsArchived.Value ? 1 : 0);
            }
            else
            {
                strCommand += ",null";
            }

            if (ID.GetValueOrDefault(0) > 0)
                strCommand += "," + ID.Value;
            else
                strCommand += ",null";

            if (DetailGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                strCommand += ",'" + DetailGuid.Value + "'";
            else
                strCommand += ",null";




            if (OrderGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                strCommand += ",'" + OrderGUID.Value + "'";
            else
                strCommand += ",null";

            return ExecuteQuery(strCommand);


        }

        /// <summary>
        /// DB_Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private ToolAssetOrderDetailsDTO DB_Insert(ToolAssetOrderDetailsDTO objDTO)
        {
            string strCommand = " EXEC [InsertToolAssetOrderDetailData] ";
            strCommand += "'" + objDTO.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty) + "'";
            strCommand += ",'" + objDTO.ToolGUID.GetValueOrDefault(Guid.Empty) + "'";

            strCommand += ",null";
            if (objDTO.ToolBinID.GetValueOrDefault(0) > 0)
                strCommand += "," + objDTO.ToolBinID + "";
            else
                strCommand += ",null";

            strCommand += "," + objDTO.RequestedQuantity.GetValueOrDefault(0) + "";
            if (objDTO.ApprovedQuantity.HasValue)
            {
                strCommand += "," + objDTO.ApprovedQuantity.GetValueOrDefault(0) + "";
            }
            else
            {
                strCommand += ",null";
            }
            strCommand += ",'" + objDTO.RequiredDate.GetValueOrDefault(DateTime.Now) + "'";
            strCommand += "," + objDTO.CreatedBy.GetValueOrDefault(0) + "";
            strCommand += "," + objDTO.Room.GetValueOrDefault(0) + "";
            strCommand += "," + objDTO.CompanyID.GetValueOrDefault(0) + "";

            //This InTransitQuantity May come from WCF Service
            if (objDTO.InTransitQuantity.GetValueOrDefault(0) > 0)
                strCommand += "," + objDTO.InTransitQuantity.GetValueOrDefault(0) + "";
            else
                strCommand += ",NULL";

            if (!string.IsNullOrEmpty(objDTO.ASNNumber))
                strCommand += ",'" + objDTO.ASNNumber + "'";
            else
                strCommand += ",NULL";

            if (objDTO.IsEDISent.GetValueOrDefault(false))
                strCommand += ",1";
            else
                strCommand += ",NULL";

            strCommand += ",'" + objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";
            strCommand += ",'" + objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";

            if (!string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                strCommand += ",'" + objDTO.AddedFrom + "' ";
            else
                strCommand += ",'Web'";

            if (!string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                strCommand += ",'" + objDTO.EditedFrom + "' ";
            else
                strCommand += ",'Web'";

            if (!string.IsNullOrEmpty(objDTO.LineNumber))
            {
                strCommand += ",@LineNumber = '" + objDTO.LineNumber + "'";
            }
            else
            {
                strCommand += ",@LineNumber = NULL";
            }
            if (!string.IsNullOrEmpty(objDTO.ControlNumber))
            {
                strCommand += ",@ControlNumber = '" + objDTO.ControlNumber + "'";
            }
            else
            {
                strCommand += ",@ControlNumber = NULL";
            }
            if (!string.IsNullOrWhiteSpace(objDTO.Comment))
                strCommand += ",@Comment='" + objDTO.Comment + "' ";
            else
                strCommand += ",@Comment=''";

            if (objDTO.UDF1 != null && !string.IsNullOrWhiteSpace(objDTO.UDF1))
                strCommand += ",@UDF1='" + objDTO.UDF1 + "' ";
            else
                strCommand += ",@UDF1=''";
            if (objDTO.UDF2 != null && !string.IsNullOrWhiteSpace(objDTO.UDF2))
                strCommand += ",@UDF2='" + objDTO.UDF2 + "' ";
            else
                strCommand += ",@UDF2=''";
            if (objDTO.UDF3 != null && !string.IsNullOrWhiteSpace(objDTO.UDF3))
                strCommand += ",@UDF3='" + objDTO.UDF3 + "' ";
            else
                strCommand += ",@UDF3=''";
            if (objDTO.UDF4 != null && !string.IsNullOrWhiteSpace(objDTO.UDF4))
                strCommand += ",@UDF4='" + objDTO.UDF4 + "' ";
            else
                strCommand += ",@UDF4=''";
            if (objDTO.UDF5 != null && !string.IsNullOrWhiteSpace(objDTO.UDF5))
                strCommand += ",@UDF5='" + objDTO.UDF5 + "' ";
            else
                strCommand += ",@UDF5=''";

            if (objDTO.GUID != Guid.Empty)
            {
                strCommand += ",@DetailGUID='" + objDTO.GUID + "' ";
            }

            //return ExecuteQuery(strCommand).FirstOrDefault();


            ToolAssetOrderDetailsDTO DetailDTO = ExecuteQuery(strCommand).FirstOrDefault();


            ToolMasterDAL itmDAL = new ToolMasterDAL(base.DataBaseName);
            ToolMasterDTO objItemDTO = itmDAL.GetToolByGUIDPlain(DetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty));
            objItemDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
            objItemDTO.EditedFrom = "Web";
            objItemDTO.Updated = DateTimeUtility.DateTimeNow;
            itmDAL.Edit(objItemDTO);


            return DetailDTO;

        }

        /// <summary>
        /// DB_Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private ToolAssetOrderDetailsDTO DB_Update(ToolAssetOrderDetailsDTO objDTO)
        {
            string strCommand = " EXEC [UpdateToolAssetOrderDetailData] ";

            strCommand += "'" + objDTO.ID + "'";
            if (objDTO.GUID != Guid.Empty)
                strCommand += ",'" + objDTO.GUID + "'";
            else
                strCommand += ",null";

            strCommand += ",'" + objDTO.ToolAssetOrderGUID + "'";
            strCommand += ",'" + objDTO.ToolGUID + "'";
            strCommand += ",null";

            if (objDTO.ToolBinID.GetValueOrDefault(0) > 0)
                strCommand += "," + objDTO.ToolBinID + "";
            else
                strCommand += ",null";

            strCommand += "," + objDTO.RequestedQuantity.GetValueOrDefault(0) + "";
            if (objDTO.ApprovedQuantity.GetValueOrDefault(0) > 0)
                strCommand += "," + objDTO.ApprovedQuantity.GetValueOrDefault(0) + "";
            else
                strCommand += ",null";
            if (objDTO.RequiredDate != null && objDTO.RequiredDate != DateTime.MinValue)
            {
                strCommand += ",'" + objDTO.RequiredDate.GetValueOrDefault(DateTime.Now) + "'";
            }
            else
            {
                strCommand += ",'" + DateTime.Now + "'";
            }

            if (objDTO.ReceivedQuantity.GetValueOrDefault(0) > 0)
                strCommand += "," + objDTO.ReceivedQuantity.GetValueOrDefault(0) + "";
            else
                strCommand += ",null";

            strCommand += "," + objDTO.LastUpdatedBy.GetValueOrDefault(0) + "";
            strCommand += "," + objDTO.Room.GetValueOrDefault(0) + "";
            strCommand += "," + objDTO.CompanyID.GetValueOrDefault(0) + "";
            strCommand += "," + (objDTO.IsEDISent.GetValueOrDefault(false) ? 1 : 0);

            if (objDTO.InTransitQuantity.GetValueOrDefault(0) > 0)
                strCommand += "," + (objDTO.InTransitQuantity.GetValueOrDefault(0));
            else
                strCommand += ",NULL";

            //if (!string.IsNullOrEmpty(objDTO.ASNNumber))
            //    strCommand += ",'" + objDTO.ASNNumber + "'";
            //else
            //    strCommand += ",NULL";

            strCommand += ",'" + objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";

            if (!string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                strCommand += ",'" + objDTO.EditedFrom + "' ";
            else
                strCommand += ",'Web'";

            if (!string.IsNullOrWhiteSpace(objDTO.Comment))
                strCommand += ",'" + objDTO.Comment + "' ";
            else
                strCommand += ",''";

            if (objDTO.LastEDIDate.HasValue && objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                strCommand += ",@LastEDIDate='" + objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) + "' ";
            else
                strCommand += ",@LastEDIDate=NULL";

            if (objDTO.IsCloseTool.HasValue)
                strCommand += ",@IsCloseTool=" + (objDTO.IsCloseTool.GetValueOrDefault(false) ? 1 : 0);
            else
                strCommand += ",@IsCloseTool=NULL";

            if (objDTO.UDF1 != null && !string.IsNullOrWhiteSpace(objDTO.UDF1))
                strCommand += ",@UDF1='" + objDTO.UDF1 + "' ";
            else
                strCommand += ",@UDF1=''";
            if (objDTO.UDF2 != null && !string.IsNullOrWhiteSpace(objDTO.UDF2))
                strCommand += ",@UDF2='" + objDTO.UDF2 + "' ";
            else
                strCommand += ",@UDF2=''";
            if (objDTO.UDF3 != null && !string.IsNullOrWhiteSpace(objDTO.UDF3))
                strCommand += ",@UDF3='" + objDTO.UDF3 + "' ";
            else
                strCommand += ",@UDF3=''";
            if (objDTO.UDF4 != null && !string.IsNullOrWhiteSpace(objDTO.UDF4))
                strCommand += ",@UDF4='" + objDTO.UDF4 + "' ";
            else
                strCommand += ",@UDF4=''";
            if (objDTO.UDF5 != null && !string.IsNullOrWhiteSpace(objDTO.UDF5))
                strCommand += ",@UDF5='" + objDTO.UDF5 + "' ";
            else
                strCommand += ",@UDF5=''";


            ToolAssetOrderDetailsDTO DetailDTO = ExecuteQuery(strCommand).FirstOrDefault();

            ToolMasterDAL itmDAL = new ToolMasterDAL(base.DataBaseName);
            ToolMasterDTO objItemDTO = itmDAL.GetToolByGUIDPlain(DetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty));
            objItemDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
            objItemDTO.EditedFrom = "Web";
            objItemDTO.Updated = DateTimeUtility.DateTimeNow;
            itmDAL.Edit(objItemDTO);

            return DetailDTO;
        }

        /// <summary>
        /// DB_Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public ToolAssetOrderDetailsDTO UpdateOrderDetail(ToolAssetOrderDetailsDTO objDTO)
        {
            string strCommand = " EXEC [UpdateToolAssetOrderDetailData] ";

            strCommand += "@DetailID='" + objDTO.ID + "'";
            if (objDTO.GUID != Guid.Empty)
                strCommand += ",@ToolAssetDetailGUID='" + objDTO.GUID + "'";
            else
                strCommand += ",@ToolAssetDetailGUID=null";

            strCommand += ",@ToolAssetOrderGUID='" + objDTO.ToolAssetOrderGUID + "'";
            strCommand += ",@ToolGUID='" + objDTO.ToolGUID + "'";
            strCommand += ",@AssetGUID=null";

            if (objDTO.ToolBinID.GetValueOrDefault(0) > 0)
                strCommand += ",@Bin=" + objDTO.ToolBinID + "";
            else
                strCommand += ",@Bin=null";

            strCommand += ",@RequestedQuantity=" + objDTO.RequestedQuantity.GetValueOrDefault(0) + "";
            if (objDTO.ApprovedQuantity.GetValueOrDefault(0) > 0)
                strCommand += ",@ApprovedQuantity=" + objDTO.ApprovedQuantity.GetValueOrDefault(0) + "";
            else
                strCommand += ",@ApprovedQuantity=null";

            strCommand += ",@RequiredDate='" + objDTO.RequiredDate.GetValueOrDefault(DateTime.Now) + "'";

            if (objDTO.ReceivedQuantity.GetValueOrDefault(0) > 0)
                strCommand += ",@ReceivedQuantity=" + objDTO.ReceivedQuantity.GetValueOrDefault(0) + "";
            else
                strCommand += ",@ReceivedQuantity=null";

            strCommand += ",@LastUpdatedBy=" + objDTO.LastUpdatedBy.GetValueOrDefault(0) + "";
            strCommand += ",@Room=" + objDTO.Room.GetValueOrDefault(0) + "";
            strCommand += ",@CompanyID=" + objDTO.CompanyID.GetValueOrDefault(0) + "";
            strCommand += ",@IsEDISENT=" + (objDTO.IsEDISent.GetValueOrDefault(false) ? 1 : 0);

            if (objDTO.InTransitQuantity.GetValueOrDefault(0) > 0)
                strCommand += ",@InTransitQuantity=" + (objDTO.InTransitQuantity.GetValueOrDefault(0));
            else
                strCommand += ",@InTransitQuantity=NULL";



            strCommand += ",@ReceivedOn='" + objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";

            if (!string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                strCommand += ",@EditedFrom='" + objDTO.EditedFrom + "' ";
            else
                strCommand += ",@EditedFrom='Web'";
            if (!string.IsNullOrWhiteSpace(objDTO.Comment))
                strCommand += ",@Comment='" + objDTO.Comment + "' ";
            else
                strCommand += ",@Comment=''";

            if (objDTO.LastEDIDate.HasValue && objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                strCommand += ",@LastEDIDate='" + objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) + "' ";
            else
                strCommand += ",@LastEDIDate=NULL";

            if (objDTO.IsCloseTool.HasValue)
                strCommand += ",@IsCloseTool=" + (objDTO.IsCloseTool.GetValueOrDefault(false) ? 1 : 0);
            else
                strCommand += ",@IsCloseItem=NULL";
            if (objDTO.UDF1 != null && !string.IsNullOrWhiteSpace(objDTO.UDF1))
                strCommand += ",@UDF1='" + objDTO.UDF1 + "' ";
            else
                strCommand += ",@UDF1=''";
            if (objDTO.UDF2 != null && !string.IsNullOrWhiteSpace(objDTO.UDF2))
                strCommand += ",@UDF2='" + objDTO.UDF2 + "' ";
            else
                strCommand += ",@UDF2=''";
            if (objDTO.UDF3 != null && !string.IsNullOrWhiteSpace(objDTO.UDF3))
                strCommand += ",@UDF3='" + objDTO.UDF3 + "' ";
            else
                strCommand += ",@UDF3=''";
            if (objDTO.UDF4 != null && !string.IsNullOrWhiteSpace(objDTO.UDF4))
                strCommand += ",@UDF4='" + objDTO.UDF4 + "' ";
            else
                strCommand += ",@UDF4=''";
            if (objDTO.UDF5 != null && !string.IsNullOrWhiteSpace(objDTO.UDF5))
                strCommand += ",@UDF5='" + objDTO.UDF5 + "' ";
            else
                strCommand += ",@UDF5=''";

            ToolAssetOrderDetailsDTO DetailDTO = ExecuteQuery(strCommand).FirstOrDefault();

            return DetailDTO;
        }

        /// <summary>
        /// DeleteRecords
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <param name="RoomId"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public bool DB_DeleteRecords(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string strCommand = "EXEC DeleteToolAssetOrderDetail ";
                    strCommand += "null";
                    strCommand += ",null";
                    strCommand += ",'" + strIDs + "'";
                    strCommand += ",null";
                    strCommand += "," + userid;
                    strCommand += "," + RoomId;
                    strCommand += "," + CompanyID;
                    strCommand += ",'" + DateTimeUtility.DateTimeNow + "'";
                    strCommand += ",'Web'";

                    int intReturn = context.Database.SqlQuery<int>(strCommand).FirstOrDefault();



                    if (intReturn > 0)
                        return true;

                }
            }
            return false;
        }

        /// <summary>
        /// Executer Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IEnumerable<ToolAssetOrderDetailsDTO> ExecuteQuery(string query)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 150;
                //IEnumerable<ToolAssetOrderDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolAssetOrderDetailsDTO>(query)
                //                                    select new ToolAssetOrderDetailsDTO
                //                                    {
                //                                        ID =Convert.ToInt64(u.ID),

                //                                        ToolName=u.ToolName,
                //                                        Serial=u.Serial,
                //                                        ToolViewGUID=u.ToolViewGUID,
                //                                        ChangeOrderDetailGUID = u.GUID,
                //                                        ChangeOrderDetailID = Convert.ToInt64(u.ID),
                //                                        IsOnlyFromUI = false,
                //                                        ApprovedQuantity = u.ApprovedQuantity,
                //                                        ASNNumber = u.ASNNumber,
                //                                        Bin = u.Bin,
                //                                        BinName = u.BinName,
                //                                        IsEDISent = u.IsEDISent,
                //                                        ToolGUID = u.ToolGUID,
                //                                        AssetGUID = u.AssetGUID,
                //                                        ToolAssetOrderGUID = u.ToolAssetOrderGUID,
                //                                        ReceivedQuantity = u.ReceivedQuantity,
                //                                        RequestedQuantity = u.RequestedQuantity.GetValueOrDefault(0),
                //                                        RequiredDate = u.RequiredDate,
                //                                        Created = u.Created,
                //                                        LastUpdated = u.LastUpdated,
                //                                        CreatedBy = u.CreatedBy,
                //                                        LastUpdatedBy = u.LastUpdatedBy,
                //                                        Room = u.Room,
                //                                        IsDeleted = u.IsDeleted,
                //                                        IsArchived = u.IsArchived,
                //                                        CompanyID = u.CompanyID,
                //                                        GUID = u.GUID,
                //                                        CreatedByName = u.CreatedByName,
                //                                        UpdatedByName = u.UpdatedByName,
                //                                        RoomName = u.RoomName,
                //                                        InTransitQuantity = u.InTransitQuantity,
                //                                        IsEDIRequired = u.IsEDIRequired,
                //                                        LastEDIDate = u.LastEDIDate,
                //                                        TotalRecords = u.TotalRecords,

                //                                        Action = string.Empty,
                //                                        HistoryID = 0,
                //                                        IsHistory = false,


                //                                        ToolCost = u.ToolCost,

                //                                        //ImagePath = u.ImagePath,
                //                                        IsBuildBreak = u.IsBuildBreak,

                //                                        //ODPackSlipNumbers = u.ODPackSlipNumbers,
                //                                        ReceivedOn = (u.ReceivedOn == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(u.ReceivedOn)),
                //                                        ReceivedOnWeb = (u.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(u.ReceivedOnWeb)),
                //                                        AddedFrom = (u.AddedFrom == null ? "Web" : Convert.ToString(u.AddedFrom)),
                //                                        EditedFrom = (u.EditedFrom == null ? "Web" : Convert.ToString(u.EditedFrom)),
                //                                        IsCloseItem = u.IsCloseItem,
                //                                        //CostUOM = u.CostUOM,
                //                                        ControlNumber = u.ControlNumber,
                //                                        LineNumber = u.LineNumber,
                //                                        //IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                //                                        Comment = u.Comment,

                //                                        UDF1 = u.UDF1,
                //                                        UDF2 = u.UDF2,
                //                                        UDF3 = u.UDF3,
                //                                        UDF4 = u.UDF4,
                //                                        UDF5 = u.UDF5,
                //                                        OrderCost = u.OrderCost
                //                                    }).AsParallel().ToList();
                IEnumerable<ToolAssetOrderDetailsDTO> obj = context.Database.SqlQuery<ToolAssetOrderDetailsDTO>(query).AsParallel().ToList();
                return obj;
            }

        }

        /// <summary>
        /// From DB
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public IEnumerable<ToolAssetOrderDetailsDTO> DB_GetChangeOrderDetailData(Int64? RoomID, Int64? CompanyID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? DetailGuid, Guid ChangeOrderGUID)
        {
            string strCommand = " EXEC [GetChangeToolAssetOrderDetailData] ";

            if (CompanyID.GetValueOrDefault(0) > 0)
                strCommand += CompanyID.Value;
            else
                strCommand += ",null";

            if (RoomID.GetValueOrDefault(0) > 0)
                strCommand += "," + RoomID.Value;
            else
                strCommand += "null";

            if (IsDeleted.HasValue)
            {
                strCommand += "," + (IsDeleted.Value ? 1 : 0);
            }
            else
            {
                strCommand += ",null";
            }

            if (IsArchived.HasValue)
            {
                strCommand += "," + (IsArchived.Value ? 1 : 0);
            }
            else
            {
                strCommand += ",null";
            }

            if (ID.GetValueOrDefault(0) > 0)
                strCommand += "," + ID.Value;
            else
                strCommand += ",null";

            if (DetailGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                strCommand += ",'" + DetailGuid.Value + "'";
            else
                strCommand += ",null";


            strCommand += ",'" + ChangeOrderGUID.ToString() + "'";
            return ExecuteQuery(strCommand);
        }

        /// <summary>
        /// Close Order Detail Item
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <param name="RoomId"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public bool CloseOrderDetailItem(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string strCommand = "EXEC CloseToolAssetOrderDetailItem ";
                    strCommand += "null";
                    strCommand += ",null";
                    strCommand += ",'" + strIDs + "'";
                    strCommand += ",null";
                    strCommand += "," + userid;
                    strCommand += "," + RoomId;
                    strCommand += "," + CompanyID;
                    strCommand += ",'" + DateTimeUtility.DateTimeNow + "'";
                    strCommand += ",'Web'";
                    strCommand += ",1";

                    int intReturn = context.Database.SqlQuery<int>(strCommand).FirstOrDefault();



                    if (intReturn > 0)
                        return true;

                }
            }
            return false;
        }

        /// <summary>
        /// Close Order Detail Item
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <param name="RoomId"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public bool UnCloseOrderDetailItem(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string strCommand = "EXEC CloseToolAssetOrderDetailItem ";
                    strCommand += "null";
                    strCommand += ",null";
                    strCommand += ",'" + strIDs + "'";
                    strCommand += ",null";
                    strCommand += "," + userid;
                    strCommand += "," + RoomId;
                    strCommand += "," + CompanyID;
                    strCommand += ",'" + DateTimeUtility.DateTimeNow + "'";
                    strCommand += ",'Web'";
                    strCommand += ",0";

                    int intReturn = context.Database.SqlQuery<int>(strCommand).FirstOrDefault();


                    if (intReturn > 0)
                        return true;

                }
            }
            return false;
        }

        #endregion
        public IEnumerable<ToolAssetOrderDetailsDTO> GetOrderDetailDataWithItemBin(Int64 RoomID, Int64 CompanyID, Guid OrderGUID, Guid ToolGUID, int BinID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //  IEnumerable<ToolAssetOrderDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolAssetOrderDetailsDTO>(@"SELECT A.*, 
                //      B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,OM.OrderStatus,
                //      E.ItemNumber,E.Description,E.LongDescription,E.SerialNumberTracking, CM.Category as CategoryName,SM.SupplierName,SM.ID as SupplierID,Om.OrderType ,
                //      E.UDF1 AS ItemUDF1,E.UDF2 AS ItemUDF2,E.UDF3 AS ItemUDF3,E.UDF4 AS ItemUDF4,E.UDF5 AS ItemUDF5,RS.CurrencyDecimalDigits,RS.NumberDecimalDigits 
                //      ,LM.[GUID] as ToolLocationGuid
                //,LM.[Location] as ToolLocationName
                //                                                  FROM ToolAssetOrderDetails A 
                //                                                  LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                //                                                  LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                //                                                  LEFT OUTER JOIN Room D on A.Room = D.ID  
                //                                                  LEFT OUTER JOIN ToolMaster E on A.ToolGUID = E.GUID 
                //                                                   LEFT OUTER JOIN ToolDetail AS TD ON TD.ToolGUID = E.Guid
                //                          LEFT OUTER JOIN ToolMaster AS TM1 ON TD.ToolItemGUID = TM1.Guid
                //                          LEFT OUTER JOIN ToolLocationDetails AS TLD ON (TLD.ToolGuid = E.GUID OR TLD.ToolGuid = TM1.GUID)
                //                          LEFT OUTER JOIN LocationMaster AS LM ON LM.Guid = TLD.ToolLocationGuid
                //                                          left outer join OrderMaster OM on oM.GUID = A.ToolAssetOrderGUID   
                //                                                  left outer join RegionalSetting RS on A.Room = RS.RoomID
                //                                                  WHERE A.Isdeleted=0 and A.CompanyID = " + CompanyID.ToString() + " and A.Room =" + RoomID.ToString() + " and A.ToolAssetOrderGUID = '" + OrderGUID.ToString() + "' and A.ItemGuid= '" + ItemGuid.ToString() + "' and A.Bin=" + BinID.ToString())
                var params1 = new SqlParameter[] { new SqlParameter("@CompnayID", CompanyID )
                        ,new SqlParameter("@RoomID", RoomID )
                        ,new SqlParameter("@OrderGUID", OrderGUID )
                        ,new SqlParameter("@ToolGUID", ToolGUID )
                        ,new SqlParameter("@BinID", BinID )

                };
                IEnumerable<ToolAssetOrderDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolAssetOrderDetailsDTO>("EXEC GetOrderDetailDataWithToolBin @CompnayID,@RoomID,@OrderGUID,@ToolGUID,@BinID", params1)
                                                             select new ToolAssetOrderDetailsDTO
                                                             {
                                                                 ID = u.ID,
                                                                 ToolAssetOrderGUID = u.ToolAssetOrderGUID,
                                                                 ToolGUID = u.ToolGUID,
                                                                 ToolBinID = u.ToolBinID,
                                                                 RequestedQuantity = u.RequestedQuantity,
                                                                 RequiredDate = u.RequiredDate,
                                                                 ReceivedQuantity = u.ReceivedQuantity,
                                                                 Created = u.Created,
                                                                 LastUpdated = u.LastUpdated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 Room = u.Room,
                                                                 IsDeleted = u.IsDeleted,
                                                                 IsArchived = u.IsArchived,
                                                                 CompanyID = u.CompanyID,
                                                                 GUID = u.GUID,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 IsEDISent = u.IsEDISent,
                                                                 Action = u.Action,
                                                                 HistoryID = u.HistoryID,
                                                                 IsHistory = true,
                                                                 //BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetRecord(u.Bin.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0), false, false).BinNumber : string.Empty,
                                                                 ToolLocationName = u.ToolLocationName,
                                                                 //BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetItemLocation( u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0), false, false,Guid.Empty, u.Bin.GetValueOrDefault(0),null,null).FirstOrDefault().BinNumber : string.Empty,
                                                                 //ItemDetail = u.ItemGUID.HasValue ? new ItemMasterDAL(base.DataBaseName).GetRecord(u.ItemGUID.Value.ToString(), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)) : null,
                                                                 ApprovedQuantity = u.ApprovedQuantity ?? 0,
                                                                 ASNNumber = u.ASNNumber,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,

                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 SerialNumberTracking = (u.SerialNumberTracking)
                                                             }).AsParallel().ToList();
                if (obj != null && obj.Count() > 0)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
            }

        }

        public IEnumerable<ToolAssetOrderDetailsDTO> GetOrderDetailDataWithItemBinByDetailGUID(Int64 RoomID, Int64 CompanyID, Guid OrderGUID, Guid ItemGuid, int BinID, Guid DetailGUID)
        {


            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@OrderGUID", OrderGUID),
                                                new SqlParameter("@ToolGuid", ItemGuid),
                                                new SqlParameter("@BinID", BinID),
                                                new SqlParameter("@DetailGUID", DetailGUID)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                IEnumerable<ToolAssetOrderDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolAssetOrderDetailsDTO>("exec [GetToolAssetOrderDetailDataWithItemBinByDetailGUID] @RoomID,@CompanyID,@OrderGUID,@ToolGuid,@BinID,@DetailGUID", params1)
                                                             select new ToolAssetOrderDetailsDTO
                                                             {
                                                                 ID = u.ID,
                                                                 ToolAssetOrderGUID = u.ToolAssetOrderGUID,
                                                                 ToolGUID = u.ToolGUID,
                                                                 AssetGUID = u.AssetGUID,
                                                                 ToolBinID = u.ToolBinID,
                                                                 RequestedQuantity = u.RequestedQuantity,
                                                                 RequiredDate = u.RequiredDate,
                                                                 ReceivedQuantity = u.ReceivedQuantity,
                                                                 Created = u.Created,
                                                                 LastUpdated = u.LastUpdated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 Room = u.Room,
                                                                 IsDeleted = u.IsDeleted,
                                                                 IsArchived = u.IsArchived,
                                                                 CompanyID = u.CompanyID,
                                                                 GUID = u.GUID,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 IsEDISent = u.IsEDISent,
                                                                 Action = u.Action,
                                                                 HistoryID = u.HistoryID,
                                                                 IsHistory = true,
                                                                 //BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetRecord(u.Bin.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0), false, false).BinNumber : string.Empty,
                                                                 ToolLocationName = u.ToolBinID.GetValueOrDefault(0) > 0 ? new LocationMasterDAL(base.DataBaseName).GetLocationByGuidPlain(u.ToolLocationGuid.GetValueOrDefault(Guid.Empty), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).Location : string.Empty,
                                                                 //BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetItemLocation( u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0), false, false,Guid.Empty, u.Bin.GetValueOrDefault(0),null,null).FirstOrDefault().BinNumber : string.Empty,
                                                                 //ItemDetail = u.ItemGUID.HasValue ? new ItemMasterDAL(base.DataBaseName).GetRecord(u.ItemGUID.Value.ToString(), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)) : null,
                                                                 ApprovedQuantity = u.ApprovedQuantity ?? 0,
                                                                 ASNNumber = u.ASNNumber,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,

                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 SerialNumberTracking = (u.SerialNumberTracking)
                                                             }).AsParallel().ToList();
                if (obj != null && obj.Count() > 0)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
            }

        }

        #region Get Order record by Date Range

        public IEnumerable<ToolAssetOrderDetailsDTO> GetOrderDetailsByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string OrderGuid)
        {
            return DB_GetOrderDetailsByDateRange(StartRowIndex, MaxRows, out TotalCount, RoomID, CompanyID, IsArchived, IsDeleted, OrderGuid);
        }

        private IEnumerable<ToolAssetOrderDetailsDTO> DB_GetOrderDetailsByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string OrderGuid)
        {
            if (MaxRows < 1)
            {
                MaxRows = 10;
            }
            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetToolAssetOrderDetailPagedData", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, null, null, null, OrderGuid);

            IEnumerable<ToolAssetOrderDetailsDTO> obj = null;

            if (ds != null && ds.Tables.Count > 0)
            {
                obj = DataTableHelper.ToList<ToolAssetOrderDetailsDTO>(ds.Tables[0]);

                TotalCount = 0;
                if (obj != null && obj.Count() > 0)
                {
                    TotalCount = obj.ElementAt(0).TotalRecords;
                }
                else
                {
                    TotalCount = 0;
                }
                return obj;
            }
            else
            {
                TotalCount = 0;
                return obj;
            }
        }

        #endregion

    }
}


