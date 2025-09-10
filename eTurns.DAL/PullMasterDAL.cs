using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace eTurns.DAL
{
    public partial class PullMasterDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        #region [Class Constructor]

        public PullMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PullMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Get Paged Records from the PullMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<PullMasterViewDTO> GetAllPullsFull(long RoomID, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetAllPullsFull] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<Guid> GetItemGuidsByPullActionType(long RoomID, long CompanyId, string PullAction)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@PullAction", PullAction) };
                return context.Database.SqlQuery<Guid>("exec [GetItemGuidsByPullActionType] @RoomID,@CompanyID,@PullAction", params1).ToList();
            }
        }

        public PullMasterViewDTO GetPullByItemGuidPlain(long RoomID, long CompanyId, Guid ItemGuid, long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullByItemGuidPlain] @RoomID,@CompanyID,@ItemGuid,@Id", params1).FirstOrDefault();
            }
        }

        public IEnumerable<PullMasterViewDTO> GetPullByWorkOrderDetailGUIDPlain(long RoomID, long CompanyId, Guid WorkOrderDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@WorkOrderDetailGUID", WorkOrderDetailGUID) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullByWorkOrderDetailGUIDPlain] @RoomID,@CompanyID,@WorkOrderDetailGUID", params1).ToList();
            }
        }

        public IEnumerable<PullMasterViewDTO> GetPullByItemGuidAndPullCreditType(long RoomID, long CompanyId, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@ItemGuid", ItemGuid) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullByItemGuidAndPullCreditType] @RoomID,@CompanyID,@ItemGuid", params1).ToList();
            }
        }

        public PullMasterViewDTO GetPullByIdPlain(long Id, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id), new SqlParameter("@IsDeleted", IsDeleted) };
                var pull = context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullByIdPlain] @Id,@IsDeleted", params1).FirstOrDefault();

                if (pull != null && pull.ID > 0)
                {
                    pull.PullCredit = pull.PullCredit ?? "Pull";
                    pull.ActionType = pull.ActionType ?? "Pull";
                }
                return pull;
            }
        }

        public PullMasterViewDTO GetPullByGuidPlain(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                var pull = context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullByGuidPlain] @Guid", params1).FirstOrDefault();

                if (pull != null && pull.ID > 0)
                {
                    pull.PullCredit = pull.PullCredit ?? "Pull";
                    pull.ActionType = pull.ActionType ?? "Pull";
                }
                return pull;
            }
        }

        public List<long> GetPullIdsByRequisitionDetailGuid(Guid RequisitionDetaillGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RequisitionDetaillGUID", RequisitionDetaillGUID), new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<long>("exec [GetPullIdsByRequisitionDetailGuid] @RequisitionDetaillGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<PullMasterViewDTO> GetPullsByRequisitionDetailGuidNormal(Guid RequisitionDetaillGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RequisitionDetaillGUID", RequisitionDetaillGUID), new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId) };
                var pulls = context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullsByRequisitionDetailGuidNormal] @RequisitionDetaillGUID,@RoomID,@CompanyID", params1).ToList();

                if (pulls != null && pulls.Any())
                {
                    pulls.Select(c => c.ActionType ?? "Pull");
                    pulls.Select(c => c.PullCredit ?? "Pull");
                }
                return pulls;
            }
        }

        public List<long> GetPullIdsByWorkOrderDetailGuid(Guid WorkOrderDetailGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderDetailGUID", WorkOrderDetailGUID), new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<long>("exec [GetPullIdsByWorkOrderDetailGuid] @WorkOrderDetailGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<PullMasterViewDTO> GetPullsByWorkOrderDetailGUIDNormal(Guid WorkOrderDetailGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@WorkOrderDetailGUID", WorkOrderDetailGUID) };
                var pulls = context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullsByWorkOrderDetailGUIDNormal] @RoomID,@CompanyID,@WorkOrderDetailGUID", params1).ToList();

                if (pulls != null && pulls.Any())
                {
                    pulls.Select(c => c.ActionType ?? "Pull");
                    pulls.Select(c => c.PullCredit ?? "Pull");
                }
                return pulls;
            }
        }

        public List<PullMasterViewDTO> GetPullHistoryByWorkOrderDetailGUIDFull(Guid WorkOrderDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderDetailGUID", WorkOrderDetailGUID) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullHistoryByWorkOrderDetailGUIDFull] @WorkOrderDetailGUID", params1).ToList();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase PullMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(PullMasterViewDTO objDTO)
        {
            try
            {
                ItemMasterDTO ItemData = new ItemMasterDAL(base.DataBaseName).GetItemWithMasterTableJoins(null, objDTO.ItemGUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));

                objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.Created = DateTimeUtility.DateTimeNow;

                List<SqlParameter> lstSQLPara = new List<SqlParameter>();
                //lstSQLPara.Add(new SqlParameter("@ID", 0));
                lstSQLPara.Add(new SqlParameter("@ProjectSpendGUID", objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? (object)DBNull.Value : objDTO.ProjectSpendGUID));
                lstSQLPara.Add(new SqlParameter("@UOI", objDTO.UOI ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@PoolQuantity", objDTO.PoolQuantity ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@PULLCost", objDTO.PullCost ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@ConsignedQuantity", objDTO.ConsignedQuantity ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@CustomerOwnedQuantity", objDTO.CustomerOwnedQuantity ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@CreditConsignedQuantity", objDTO.CreditConsignedQuantity ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@CreditCustomerOwnedQuantity", objDTO.CreditCustomerOwnedQuantity ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@ActionType", objDTO.ActionType ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@PullCredit", objDTO.PullCredit ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@PullPrice", objDTO.PullPrice ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@BinID", objDTO.BinID ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@UDF1", objDTO.UDF1 ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@UDF2", objDTO.UDF2 ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@UDF3", objDTO.UDF3 ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@UDF4", objDTO.UDF4 ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@UDF5", objDTO.UDF5 ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@GUID", objDTO.GUID));
                lstSQLPara.Add(new SqlParameter("@ItemGUID", objDTO.ItemGUID ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@CreatedBy", objDTO.CreatedBy ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@IsDeleted", objDTO.IsDeleted ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@IsArchived", objDTO.IsArchived ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@CompanyID", objDTO.CompanyID ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@RoomID", objDTO.Room ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@IsProcessedAfterSync", true));
                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Pull";
                lstSQLPara.Add(new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@WorkOrderDetailGUID", objDTO.WorkOrderDetailGUID ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@CountLineItemGuid", objDTO.CountLineItemGuid ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@RequisitionDetailGUID", objDTO.RequisitionDetailGUID ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@PullOrderNumber", objDTO.PullOrderNumber ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@ReceivedOn", objDTO.ReceivedOn));
                lstSQLPara.Add(new SqlParameter("@ReceivedOnWeb", objDTO.ReceivedOnWeb));
                if (string.IsNullOrEmpty(objDTO.AddedFrom))
                    objDTO.AddedFrom = "Web";
                if (string.IsNullOrEmpty(objDTO.EditedFrom))
                    objDTO.EditedFrom = "Web";
                lstSQLPara.Add(new SqlParameter("@AddedFrom", objDTO.AddedFrom ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom ?? (object)DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@SupplierAccountGuid", objDTO.SupplierAccountGuid ?? (object)DBNull.Value));
                if (ItemData != null && ItemData.ID > 0)
                {
                    lstSQLPara.Add(new SqlParameter("@ItemCost", ItemData.Cost ?? (object)DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemSellPrice", ItemData.SellPrice ?? (object)DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemAverageCost", ItemData.AverageCost ?? (object)DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemMarkup", ItemData.Markup ?? (object)DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemCostUOMValue", ItemData.CostUOMValue ?? (object)DBNull.Value));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@ItemCost", DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemSellPrice", DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemAverageCost", DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemMarkup", DBNull.Value));
                    lstSQLPara.Add(new SqlParameter("@ItemCostUOMValue", DBNull.Value));
                }
                objDTO.PullType = objDTO.PullType > 0 ? objDTO.PullType : 1;
                lstSQLPara.Add(new SqlParameter("@PullType", objDTO.PullType));
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    Int64 id = context.Database.SqlQuery<Int64>("EXEC [InsertPullMaster] @ProjectSpendGUID,@UOI,@PoolQuantity,@PULLCost,@ConsignedQuantity,@CustomerOwnedQuantity,@CreditConsignedQuantity,@CreditCustomerOwnedQuantity,@ActionType,@PullCredit,@PullPrice,@BinID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@GUID,@ItemGUID,@CreatedBy,@LastUpdatedBy,@IsDeleted,@IsArchived,@CompanyID,@RoomID,@IsProcessedAfterSync,@WhatWhereAction,@WorkOrderDetailGUID,@CountLineItemGuid,@RequisitionDetailGUID,@PullOrderNumber,@ReceivedOn,@ReceivedOnWeb,@AddedFrom,@EditedFrom,@SupplierAccountGuid,@ItemCost,@ItemSellPrice,@ItemAverageCost,@ItemMarkup,@ItemCostUOMValue,@PullType", lstSQLPara.ToArray()).FirstOrDefault();

                    return id;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            //PullMaster obj = new PullMaster();
            //    obj.ID = 0;
            //    obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
            //    obj.UOI = objDTO.UOI;
            //    obj.PoolQuantity = objDTO.PoolQuantity;
            //    obj.PULLCost = objDTO.PullCost;
            //    obj.ConsignedQuantity = objDTO.ConsignedQuantity;
            //    obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
            //    obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
            //    obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
            //    obj.ActionType = objDTO.ActionType;
            //    obj.PullCredit = objDTO.PullCredit;
            //    obj.PullPrice = objDTO.PullPrice;
            //    obj.BinID = objDTO.BinID;
            //    obj.UDF1 = objDTO.UDF1;
            //    obj.UDF2 = objDTO.UDF2;
            //    obj.UDF3 = objDTO.UDF3;
            //    obj.UDF4 = objDTO.UDF4;
            //    obj.UDF5 = objDTO.UDF5;


            //obj.GUID = objDTO.GUID;//Guid.NewGuid();
            //    obj.ItemGUID = objDTO.ItemGUID;
            //    objDTO.Created = DateTimeUtility.DateTimeNow;
            //    objDTO.Updated = DateTimeUtility.DateTimeNow;
            //    obj.Created = DateTimeUtility.DateTimeNow;
            //    obj.Updated = DateTimeUtility.DateTimeNow;
            //    obj.CreatedBy = objDTO.CreatedBy;
            //    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
            //    obj.IsDeleted = (bool)objDTO.IsDeleted;
            //    obj.IsArchived = (bool)objDTO.IsArchived;
            //    obj.CompanyID = objDTO.CompanyID;
            //    obj.Room = objDTO.Room;
            //    obj.IsProcessedAfterSync = true;
            //    obj.WhatWhereAction = objDTO.WhatWhereAction;


            //obj.WorkOrderDetailGUID = objDTO.WorkOrderDetailGUID;
            //    obj.CountLineItemGuid = objDTO.CountLineItemGuid;
            //    obj.RequisitionDetailGUID = objDTO.RequisitionDetailGUID;
            //    obj.PullOrderNumber = objDTO.PullOrderNumber;
            //    obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //    obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //    obj.AddedFrom = objDTO.AddedFrom;
            //    obj.EditedFrom = objDTO.EditedFrom;



            //obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
            //    if (ItemData != null && ItemData.ID > 0)
            //    {
            //        obj.ItemCost = ItemData.Cost;
            //        obj.ItemSellPrice = ItemData.SellPrice;
            //        obj.ItemAverageCost = ItemData.AverageCost;
            //        obj.ItemMarkup = ItemData.Markup;
            //        obj.ItemCostUOMValue = ItemData.CostUOMValue;
            //    }
            //    obj.PullType = (objDTO.PullType > 0 ? objDTO.PullType : 1);
            //    context.PullMasters.Add(obj);
            //    context.SaveChanges();
            //    objDTO.ID = obj.ID;
            //    objDTO.GUID = obj.GUID;

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(PullMasterViewDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            PullMaster obj;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = context.PullMasters.FirstOrDefault(t => t.ID == objDTO.ID);

                if (obj != null)
                {
                    obj.ID = objDTO.ID;
                    obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
                    obj.UOI = objDTO.UOI;
                    obj.ActionType = objDTO.ActionType;
                    obj.PullCredit = objDTO.PullCredit;
                    obj.PULLCost = objDTO.PullCost;
                    obj.PullPrice = objDTO.PullPrice;
                    obj.PoolQuantity = objDTO.PoolQuantity;
                    obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                    obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                    obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
                    obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
                    obj.BinID = objDTO.BinID;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.ControlNumber = objDTO.ControlNumber;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Pull";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;
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
        public bool DeletePullsByPullIds(string IDs, long userid, long CompanyID, long RoomID, long SessionUserId,long EnterpriceId)
        {
            string MSG = "";

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool isAllowDelete = false;
                string PULLGUID = "";
                PullMasterViewDTO objDTO = null;
                try
                {
                    string[] strPullIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    #region "Reverse process after Successful DELETE"
                    foreach (var item in strPullIDs)
                    {
                        objDTO = new PullMasterViewDTO();
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            objDTO = GetPullByIdPlain(long.Parse(item), false);
                            if (objDTO == null)
                            {
                                continue;
                            }
                            PULLGUID = objDTO.GUID.ToString();
                            isAllowDelete = PullCreditAfterDelete(objDTO, RoomID, CompanyID, out MSG, SessionUserId,EnterpriceId);

                            //below code is to credit back the requisitioned quantity which has pulled during pull from requisition
                            if (isAllowDelete && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                RequisitionDetailsDAL objReqDtlDal = new RequisitionDetailsDAL(base.DataBaseName);
                                objReqDtlDal.UpdateItemOnRequisitionQty(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0));

                                // Below code is to credit back the approved and pulled qty in Requisition module
                                RequisitionDetailsDTO objRequDetDTO = objReqDtlDal.GetRequisitionDetailsByGUIDPlain(objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty));
                                if (objRequDetDTO != null)
                                {
                                    objRequDetDTO.QuantityPulled = objRequDetDTO.QuantityPulled.GetValueOrDefault(0) - objDTO.PoolQuantity.GetValueOrDefault(0);
                                    objReqDtlDal.Edit(objRequDetDTO, SessionUserId);
                                }
                            }

                        }
                    }
                    #endregion

                    if (isAllowDelete)
                    {

                        if (strPullIDs != null && strPullIDs.Any())
                        {
                            var pullIds = string.Join(",", strPullIDs);
                            var deleteResult = DeletePullsByPullIds(pullIds, userid);
                        }

                        foreach (var item in strPullIDs)
                        {
                            if (!string.IsNullOrEmpty(item.Trim()))
                            {
                                objDTO = GetPullByIdPlain(long.Parse(item), true);

                                if (objDTO != null && objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                                    objWOLDAL.UpdateWOItemAndTotalCost(objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));

                                }
                                if (objDTO != null)
                                {
                                    DashboardDAL objdashBoard = new DashboardDAL(base.DataBaseName);
                                    objdashBoard.UpdateTurnsByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID ?? Guid.Empty, objDTO.LastUpdatedBy.GetValueOrDefault(0));
                                    objdashBoard.UpdateAvgUsageByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID ?? Guid.Empty, objDTO.LastUpdatedBy.GetValueOrDefault(0), SessionUserId);
                                }
                            }
                        }

                        MSG = "ok";
                        return true;
                    }
                    else
                    {
                        MSG = "duplicate";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MSG = "error";
                    return false;
                }
            }
        }

        public bool DeletePullsByPullIds(string PullIds, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Ids", PullIds), new SqlParameter("@UserId", UserId) };
                context.Database.ExecuteSqlCommand("exec [DeletePullsByPullIds] @Ids,@UserId", params1);
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing
        /// <returns></returns>
        public bool UpdatePullData(PullMasterViewDTO objDTO, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, long ModuleId, out string ItemLocationMSG, bool IsProjectSpendAllowed, out bool IsPSLimitExceed, string RoomDateFormat, long SessionUserId,long EnterpriseID,string CultureCode, bool AllowNegetive = false,double? EditedSellPrice=null,bool isFromWorkOrder = false ,bool AllowEditItemSellPriceonWorkOrderPull = false)
        {
            #region "Global Variables"
            ItemLocationMSG = "";
            IsPSLimitExceed = false;
            Int64 TempOldBinID = 0;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
            PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO obj = new PullMasterViewDTO();
            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
            ItemLocationQTYDTO lstLocDTO1 = new ItemLocationQTYDTO();
            BinMasterDTO objBINDTO = new BinMasterDTO();
            List<ItemLocationDetailsDTO> ObjItemLocation = null;
            PullMasterViewDTO ReturnDto = null;
            RoomDTO objRoomDTO = new RoomDTO();
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            PullDetailsDAL objPullDtlDal = new PullDetailsDAL(base.DataBaseName);

            bool IsStagginLocation = false;
            //bool IsProjectSpendMandatoryPleaseSelect = false;

            #endregion
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

            // objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);
            string columnList = "ID,RoomName,IsProjectSpendMandatory,InventoryConsuptionMethod,MethodOfValuingInventory";
            objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseID, CompanyID);

            if (ItemDTO != null && ItemDTO.ItemType != 4)
            {
                if (objRoomDTO != null)
                {
                    if (objRoomDTO.IsProjectSpendMandatory)
                    {
                        if (objDTO.ProjectSpendGUID == null)
                        {
                            string msgProjectSpendMandatorySelectIt = ResourceRead.GetResourceValueByKeyAndFullFilePath("ProjectSpendMandatorySelectIt", pullMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResPullMaster", CultureCode);
                            ItemLocationMSG = msgProjectSpendMandatorySelectIt;
                            return false;
                        }
                    }
                }
            }

            BinMasterDAL objLocDAL = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO objLocDTO = objLocDAL.GetBinByID(objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID);

            if (objLocDTO != null && objLocDTO.ID > 0)
            {
                if (objLocDTO.IsStagingLocation)
                {
                    IsStagginLocation = true;
                }
            }

            //---------------------------------------------------------------------
            //
            double? ItemPullCost = 0;
            double? ItemPullPrice = 0;

            if (((objDTO.RequisitionDetailGUID != null && objDTO.RequisitionDetailGUID != Guid.Empty)
                        || (objDTO.WorkOrderDetailGUID != null && objDTO.WorkOrderDetailGUID != Guid.Empty)
                     ) && ModuleId != 0)
            {
                int? PriseSelectionOption = 0;
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(CompanyID, RoomID, ModuleId);
                if (objRoomModuleSettingsDTO == null || objRoomModuleSettingsDTO.PriseSelectionOption == null || (objRoomModuleSettingsDTO.PriseSelectionOption != 1 && objRoomModuleSettingsDTO.PriseSelectionOption != 2))
                    PriseSelectionOption = 1;
                else
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;

                if ((objDTO.ActionType.ToLower() == "pull" || objDTO.ActionType.ToLower() == "ms pull")
                    && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull
                    && EditedSellPrice != null)
                {
                    ItemPullCost = EditedSellPrice.GetValueOrDefault(0);
                    if (ItemDTO.Markup.GetValueOrDefault(0) > 0
                        && EditedSellPrice.GetValueOrDefault(0) > 0)
                    {
                        ItemPullPrice = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(EditedSellPrice.GetValueOrDefault(0))) / (Convert.ToDecimal(ItemDTO.Markup) + Convert.ToDecimal(100)));
                    }
                    else
                    { ItemPullPrice = EditedSellPrice.GetValueOrDefault(0); }
                }
                else
                {
                    ItemPullCost = ItemDTO.SellPrice;
                    ItemPullPrice = ItemDTO.Cost;
                }
            }
            else
            {
                if (ItemDTO != null && ItemDTO.ItemType == 4)
                {
                    ItemPullCost = ItemDTO.Cost;
                }
                else
                {
                    ItemPullCost = ItemDTO.SellPrice;
                }
            }
            if ((objDTO.ActionType.ToLower() == "pull" || objDTO.ActionType.ToLower() == "ms pull")
                    && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull
                    && EditedSellPrice != null)
            {
                if (ItemDTO.Markup.GetValueOrDefault(0) > 0 && EditedSellPrice.GetValueOrDefault(0) > 0)
                {
                    ItemPullPrice = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(EditedSellPrice.GetValueOrDefault(0))) / (Convert.ToDecimal(ItemDTO.Markup) + Convert.ToDecimal(100)));
                }
                else
                { ItemPullPrice = EditedSellPrice.GetValueOrDefault(0); }
            }
            else
            {
                ItemPullPrice = ItemDTO.Cost;
            }

            //---------------------------------------------------------------------
            //

            if (ItemDTO != null && ItemDTO.ItemType == 4)
            {
                #region "Pull Insert Update"
                objDTO.CustomerOwnedQuantity = objDTO.PoolQuantity;
                ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal, SessionUserId);
                ItemLocationDetailsDTO ObjTempItemLocation = new ItemLocationDetailsDTO();
                ObjTempItemLocation.CustomerOwnedQuantity = objDTO.TempPullQTY;
                ObjTempItemLocation.ItemGUID = objDTO.ItemGUID;
                ObjTempItemLocation.Room = objDTO.Room;
                ObjTempItemLocation.CompanyID = objDTO.CompanyID;
                ObjTempItemLocation.PullCredit = objDTO.PullCredit;

                obj = AddtoPullDetail(ObjTempItemLocation, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), ItemPullCost.GetValueOrDefault(0), obj.LastUpdatedBy, objDTO.TempPullQTY.GetValueOrDefault(0), 0, ItemPullPrice.GetValueOrDefault(0));
                obj.CustomerOwnedQuantity = obj.PoolQuantity;
                #endregion

                #region "Project Spend Quantity Update"

                if (objPullDal.Edit(obj))
                {

                    if (objDTO.ProjectSpendGUID != null)
                    {
                        UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
                    }
                }
                #endregion
            }
            else
            {
                #region "LIFO FIFO"

                Boolean IsFIFO = false;
                if (objRoomDTO != null && objRoomDTO.ID > 0)
                {
                    if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "fifo")
                        IsFIFO = true;
                    if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "lifo")
                        IsFIFO = false;
                }
                else
                {
                    IsFIFO = true;
                }

                #endregion

                #region "For Item Pull"
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //if staging location then check qty on staging module
                    if (IsStagginLocation)
                    {
                        #region "Stagging Bin Wise Quantity Check"

                        // if block is for WI-4665: allow staging pull if qty available at Staging header which is selected while creating the requisition.
                        double retval = 0;

                        if ((objDTO.RequisitionDetailGUID.HasValue && objDTO.RequisitionDetailGUID != null && objDTO.RequisitionDetailGUID != Guid.Empty))
                        {
                            var requisition = new RequisitionMasterDAL(base.DataBaseName).GetRequisitionByReqDetailGUIDPlain(objDTO.RequisitionDetailGUID.Value);

                            //IQueryable<MaterialStagingDetail> qry;
                            List<MaterialStagingDetailDTO> lstMaterialStagingDetailDTO = new List<MaterialStagingDetailDTO>();
                            if (requisition != null && requisition.MaterialStagingGUID.HasValue && requisition.MaterialStagingGUID.Value != Guid.Empty)
                            {
                                lstMaterialStagingDetailDTO = new MaterialStagingDetailDAL(base.DataBaseName)
                                                            .GetMaterialStagingDetailByMaterialStagingGUIDAndItemGUIDAndBinIDPlain(requisition.MaterialStagingGUID.Value, ItemDTO.GUID, objDTO.BinID);
                            }
                            else
                            {
                                lstMaterialStagingDetailDTO = new MaterialStagingDetailDAL(base.DataBaseName)
                                                            .GetMaterialStagingDetailByItemGUIDAndBinIDPlain(ItemDTO.GUID, objDTO.BinID);
                            }

                            if (lstMaterialStagingDetailDTO.Count()>0)
                            {
                                retval = lstMaterialStagingDetailDTO.Sum(t => t.Quantity);
                            }
                            if (retval < objDTO.TempPullQTY)
                            {
                                //bool validateQty = true;
                                //if (AllowNegetive
                                //    && !ItemDTO.SerialNumberTracking
                                //    && !ItemDTO.LotNumberTracking
                                //    && !ItemDTO.DateCodeTracking)
                                //{
                                //    validateQty = false;
                                //}
                                //if (validateQty)
                                //{
                                string msgNotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", pullMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResPullMaster", CultureCode);
                                ItemLocationMSG = string.Format(msgNotEnoughQuantityForLocation, objLocDTO.BinNumber, retval.ToString());
                                return true;
                                //}
                            }
                        }
                        else
                        {

                            List<MaterialStagingDetailDTO> lstMaterialStagingDetailDTO = new MaterialStagingDetailDAL(base.DataBaseName)
                                                            .GetMaterialStagingDetailByItemGUIDAndBinIDPlain(ItemDTO.GUID, objDTO.BinID);
                            if (lstMaterialStagingDetailDTO !=null && lstMaterialStagingDetailDTO.Count()>0)
                            {
                                retval = lstMaterialStagingDetailDTO.Sum(t => t.Quantity);
                            }
                            if (retval < objDTO.TempPullQTY)
                            {
                                //bool validateQty = true;
                                //if (AllowNegetive
                                //    && !ItemDTO.SerialNumberTracking
                                //    && !ItemDTO.LotNumberTracking
                                //    && !ItemDTO.DateCodeTracking)
                                //{
                                //    validateQty = false;
                                //}
                                //if (validateQty)
                                //{
                                string msgNotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", pullMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResPullMaster", CultureCode);
                                ItemLocationMSG = string.Format(msgNotEnoughQuantityForLocation, objLocDTO.BinNumber, retval.ToString());
                                return true;
                                //}
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region "Bin Wise Quantity Check"
                        //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).FirstOrDefault();
                        lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objDTO.BinID, Convert.ToString(objDTO.ItemGUID.Value)).FirstOrDefault();
                        lstLocDTO1 = objLocationDAL.GetItemQtyByLocation(objDTO.BinID ?? 0, objDTO.ItemGUID ?? Guid.Empty, RoomID, CompanyID, objDTO.CreatedBy ?? 0);
                        if (lstLocDTO == null && lstLocDTO1 != null && lstLocDTO1.Quantity > 0)
                        {
                            objLocQTY.Insert(lstLocDTO1, SessionUserId, EnterpriseID);
                            //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).FirstOrDefault();
                            lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objDTO.BinID, Convert.ToString(objDTO.ItemGUID.Value)).FirstOrDefault();
                        }
                        objBINDTO = null;// objBINDAL.GetRecord((Int64)objDTO.BinID, RoomID, CompanyID, false, false);

                        if (AllowNegetive)
                        {
                            if (ItemDTO.SerialNumberTracking || ItemDTO.LotNumberTracking)
                            {
                                if (BinWiseQuantityCheck(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, EnterpriseID, CultureCode))
                                {
                                    return true;
                                }
                            }

                            if ((objDTO.WhatWhereAction ?? string.Empty).ToLower().Contains("insertpull_realtime"))
                            {
                                objDTO.TempPullQTY = objDTO.PoolQuantity.GetValueOrDefault(0);
                            }
                        }
                        else
                        {
                            if (BinWiseQuantityCheck(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, EnterpriseID, CultureCode))
                            {
                                return true;
                            }
                        }
                        #endregion
                    }

                    if (!IsStagginLocation)
                    {

                        //Pick up the locations.....
                        if (ItemDTO.SerialNumberTracking)
                        {
                            if (IsCreditPullNothing == 2) // pull
                            {
                                ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).Take((int)objDTO.TempPullQTY).ToList();
                            }
                        }
                        else
                        {
                            if (IsCreditPullNothing == 1)
                            {
                                if (lstLocDTO == null)
                                {
                                    lstLocDTO = new ItemLocationQTYDTO();
                                    lstLocDTO.ID = 0;
                                    lstLocDTO.BinID = objDTO.BinID.Value;
                                    lstLocDTO.CustomerOwnedQuantity = 0;
                                    lstLocDTO.ConsignedQuantity = 0;
                                    lstLocDTO.Quantity = 0;
                                    lstLocDTO.LotNumber = "";
                                    lstLocDTO.GUID = Guid.NewGuid();
                                    lstLocDTO.ItemGUID = ItemDTO.GUID;
                                    lstLocDTO.Created = DateTime.UtcNow;
                                    lstLocDTO.LastUpdated = DateTime.UtcNow;
                                    lstLocDTO.CreatedBy = objDTO.CreatedBy;
                                    lstLocDTO.LastUpdatedBy = objDTO.CreatedBy;
                                    lstLocDTO.Room = RoomID;
                                    lstLocDTO.CompanyID = CompanyID;
                                    lstLocDTO.ReceivedOnWeb = DateTime.UtcNow;
                                    lstLocDTO.ReceivedOn = DateTime.UtcNow;
                                    if (String.IsNullOrWhiteSpace(objDTO.EditedFrom))
                                    {
                                        lstLocDTO.AddedFrom = "Web";
                                        lstLocDTO.EditedFrom = "Web";
                                    }
                                    else
                                    {
                                        lstLocDTO.AddedFrom = objDTO.EditedFrom;
                                        lstLocDTO.EditedFrom = objDTO.EditedFrom;
                                    }
                                    objLocQTY.Insert(lstLocDTO, SessionUserId, EnterpriseID);
                                    //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).FirstOrDefault();
                                    lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objDTO.BinID, Convert.ToString(objDTO.ItemGUID.Value)).FirstOrDefault();
                                }

                                //------------------------------------
                                //
                                ItemLocationDetailsDTO objItemLocationDetailsDTO = new ItemLocationDetailsDTO();

                                if (ItemDTO.Consignment == true)
                                {
                                    objItemLocationDetailsDTO.ConsignedQuantity = objDTO.PoolQuantity;
                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = 0;
                                    objDTO.CreditConsignedQuantity = objDTO.PoolQuantity;
                                    objDTO.CreditCustomerOwnedQuantity = 0;
                                }
                                else
                                {
                                    objItemLocationDetailsDTO.ConsignedQuantity = 0;
                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = objDTO.PoolQuantity;
                                    objDTO.CreditConsignedQuantity = 0;
                                    objDTO.CreditCustomerOwnedQuantity = objDTO.PoolQuantity;
                                }

                                objItemLocationDetailsDTO.BinID = objLocDTO.ID;
                                objItemLocationDetailsDTO.IsDeleted = false;
                                objItemLocationDetailsDTO.IsArchived = false;
                                objItemLocationDetailsDTO.Cost = ItemDTO.Cost;
                                objItemLocationDetailsDTO.ReceivedDate = DateTime.UtcNow;
                                objItemLocationDetailsDTO.Received = objItemLocationDetailsDTO.ReceivedDate.Value.ToString(RoomDateFormat);
                                objItemLocationDetailsDTO.ReceivedOn = DateTime.UtcNow;
                                objItemLocationDetailsDTO.ReceivedOnWeb = DateTime.UtcNow;
                                if (string.IsNullOrWhiteSpace(ItemDTO.EditedFrom))
                                {
                                    objItemLocationDetailsDTO.AddedFrom = "Web";
                                    objItemLocationDetailsDTO.EditedFrom = "Web";
                                }
                                else
                                {
                                    objItemLocationDetailsDTO.AddedFrom = ItemDTO.EditedFrom;
                                    objItemLocationDetailsDTO.EditedFrom = ItemDTO.EditedFrom;
                                }
                                objItemLocationDetailsDTO.ItemGUID = ItemDTO.GUID;
                                objItemLocationDetailsDTO.CreatedBy = objDTO.CreatedBy;
                                objItemLocationDetailsDTO.Created = DateTime.UtcNow;
                                objItemLocationDetailsDTO.Room = RoomID;
                                objItemLocationDetailsDTO.CompanyID = CompanyID;
                                objItemLocationDetailsDTO = objLocationDAL.Insert(objItemLocationDetailsDTO);
                                ObjItemLocation = new List<ItemLocationDetailsDTO>() { objItemLocationDetailsDTO };
                            }
                            else
                            {
                                ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).ToList();
                                if (!string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                                {
                                    ObjItemLocation.ForEach(t => t.EditedFrom = objDTO.EditedFrom);
                                }
                            }
                        }

                        #region "Project Wise Quantity Check"

                        if (objDTO.ProjectSpendGUID != null && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                        {
                            var tmpsupplierIds = new List<long>();
                            //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objDTO.ItemGUID).FirstOrDefault();
                            objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID, tmpsupplierIds, Convert.ToString(objDTO.ItemGUID)).FirstOrDefault();
                            objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);

                            if (ProjectWiseQuantityCheck(objPrjSpenItmDTO, objPrjMstDTO, out ItemLocationMSG, objDTO, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, EnterpriseID, CultureCode, CompanyID, RoomID))
                            {
                                return true;
                            }
                        }
                        #endregion

                        #region "Pull Insert Update"
                        if (ItemDTO.Consignment && string.IsNullOrWhiteSpace(objDTO.PullOrderNumber))
                        {
                            AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, ItemDTO.SupplierID ?? 0, ItemDTO.GUID, EnterpriseID, null, false);
                            if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
                            {
                                objDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
                            }
                            else
                            {
                                bool isAutoNumberGenerated = true;
                                if (objAutoOrderNumberGenerate != null && objAutoOrderNumberGenerate.IsBlank)
                                {
                                    NotificationDAL objNotificationDAL = new NotificationDAL(base.DataBaseName);
                                    SchedulerDTO objSchedulerDTO = objNotificationDAL.GetRoomSchedulesBySupplierID(ItemDTO.SupplierID.GetValueOrDefault(0), RoomID, CompanyID);
                                    if (objSchedulerDTO != null && objSchedulerDTO.ScheduleMode == 6)
                                    {
                                        if (objDTO.PullOrderNumber == null)
                                            objDTO.PullOrderNumber = "";
                                        isAutoNumberGenerated = false;
                                    }
                                }
                                if (isAutoNumberGenerated)
                                {
                                    DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
                                    objDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
                                }
                            }

                        }
                        else if (ItemDTO.Consignment && !string.IsNullOrWhiteSpace(objDTO.PullOrderNumber))
                        {
                            string strPONumber = new AutoSequenceDAL(base.DataBaseName).GetAndUpdatePONumber(objDTO.PullOrderNumber, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, ItemDTO.SupplierID ?? 0, ItemDTO.GUID);
                            objDTO.PullOrderNumber = strPONumber;
                        }

                        ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal, SessionUserId);

                        #endregion

                        #region "Item Location & Quantity  Wise Deduction"

                        #region "ItemLocation Deduction"
                        if (IsCreditPullNothing != 3)
                        {
                            if (ItemDTO.SerialNumberTracking)
                            {
                                #region "Serial logic"


                                double LocalSearilaCount = 0;
                                double TakenCreditCount = 0;
                                foreach (var itemoil in ObjItemLocation)
                                {
                                    double loopCurrentTakenCustomer = 0;
                                    double loopCurrentTakenConsignment = 0;

                                    /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                                    if (ItemDTO.Consignment)
                                    {
                                        #region "Consignment Credit and Pull"

                                        if (IsCreditPullNothing == 1) // credit
                                        {
                                            PullMasterViewDTO result = objPullDal.GetPullByItemGuidPlain(RoomID, CompanyID, ItemDTO.GUID, obj.ID);//.Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID).Where(t => (t.ConsignedQuantity.GetValueOrDefault(0) != t.CreditConsignedQuantity.GetValueOrDefault(0)) || (t.CustomerOwnedQuantity.GetValueOrDefault(0) != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0))).OrderByDescending(t => t.ID).Take(1).FirstOrDefault();

                                            if (result != null)
                                            {
                                                if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
                                                {
                                                    // un-satled Customer Owned                   
                                                    if (TakenCreditCount != objDTO.TempPullQTY.GetValueOrDefault(0))
                                                    {
                                                        obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + 1;
                                                        TakenCreditCount += 1;
                                                    }
                                                    itemoil.CustomerOwnedQuantity = 1;
                                                    result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + 1;
                                                }
                                                else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
                                                {
                                                    if (TakenCreditCount != objDTO.TempPullQTY.GetValueOrDefault(0))
                                                    {
                                                        obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
                                                        TakenCreditCount += 1;
                                                    }
                                                    itemoil.ConsignedQuantity = 1;
                                                    result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
                                                }
                                                result.WhatWhereAction = "Consignment";
                                                objPullDal.Edit(result);
                                            }

                                            //get last pull record to know the quantity....
                                            itemoil.ConsignedQuantity = 1;
                                        }
                                        if (IsCreditPullNothing == 2) // pull
                                        {
                                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                                            {
                                                loopCurrentTakenConsignment = 1;
                                                itemoil.ConsignedQuantity = 0;
                                            }
                                            else
                                            {
                                                loopCurrentTakenCustomer = 1;
                                                itemoil.CustomerOwnedQuantity = 0;
                                            }
                                        }
                                        #endregion
                                    }
                                    else //customerowendQuantity
                                    {
                                        #region "customerowned Credit and Pull"
                                        if (IsCreditPullNothing == 1) // credit
                                        {
                                            if (LocalSearilaCount != objDTO.TempPullQTY)
                                            {
                                                LocalSearilaCount += 1;
                                                itemoil.CustomerOwnedQuantity = 1;
                                            }
                                            else
                                            {
                                                objLocationDAL.Edit(itemoil);
                                                break;
                                            }
                                        }
                                        else if (IsCreditPullNothing == 2) // pull
                                        {
                                            loopCurrentTakenCustomer = 1;
                                            itemoil.CustomerOwnedQuantity = 0;
                                        }
                                        #endregion
                                    }

                                    #region For Rool Level Average Cost and Customerowned Item

                                    if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                        && loopCurrentTakenConsignment == 0)
                                    {
                                        itemoil.SellPrice = itemoil.Cost + (((itemoil.Cost ?? 0) * (ItemDTO.Markup ?? 0)) / 100);
                                    }
                                    #endregion

                                    double? itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, itemoil.SellPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    double? itemPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, itemoil.Cost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    if ((objDTO.ActionType.ToLower() == "pull" || objDTO.ActionType.ToLower() == "ms pull")
                                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull
                                        && EditedSellPrice != null)
                                    {
                                        itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, ItemPullCost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                        itemPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, ItemPullPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    }
                                    objLocationDAL.Edit(itemoil);
                                    itemoil.PullCredit = objDTO.PullCredit;
                                    obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), itemCost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, ItemPullPrice.GetValueOrDefault(0));
                                }
                                obj.CreditCustomerOwnedQuantity = LocalSearilaCount;
                                #endregion
                            }
                            else
                            {
                                #region "LOt and other type logic"

                                Double takenQunatity = 0;

                                foreach (var itemoil in ObjItemLocation)
                                {
                                    Double loopCurrentTakenCustomer = 0;
                                    Double loopCurrentTakenConsignment = 0;
                                    /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                                    if (takenQunatity == objDTO.TempPullQTY)
                                    {
                                        break;
                                    }

                                    if (ItemDTO.Consignment)
                                    {
                                        #region "Consignment Credit and Pull"
                                        if (IsCreditPullNothing == 1) // credit
                                        {
                                            if (ItemDTO.Consignment == true)
                                            {
                                                ReturnDto.CreditConsignedQuantity = objDTO.PoolQuantity;
                                                ReturnDto.CreditCustomerOwnedQuantity = 0;
                                            }
                                            else
                                            {
                                                ReturnDto.CreditConsignedQuantity = 0;
                                                ReturnDto.CreditCustomerOwnedQuantity = objDTO.PoolQuantity;
                                            }
                                            PullMasterViewDTO result = ReturnDto;

                                            if (result != null)
                                            {
                                                double unSatalledDifferent = 0;
                                                if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
                                                {
                                                    // un-satled Customer Owned                   
                                                    unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                                                    if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
                                                    {
                                                        obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                    }
                                                    else
                                                    {
                                                        double qtyavailable = 0;
                                                        qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                                        obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                        takenQunatity += qtyavailable;
                                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + qtyavailable;
                                                        result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                    }

                                                }
                                                else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
                                                {
                                                    // un-satled Customer Owned                   
                                                    unSatalledDifferent = result.ConsignedQuantity.GetValueOrDefault(0) - result.CreditConsignedQuantity.GetValueOrDefault(0);
                                                    if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
                                                    {
                                                        obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                    }
                                                    else
                                                    {
                                                        double qtyavailable = 0;
                                                        qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                                        obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                        takenQunatity += qtyavailable;
                                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + qtyavailable;
                                                        result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                    }
                                                }
                                                result.WhatWhereAction = "Consignment";
                                                objPullDal.Edit(result);
                                            }
                                        }
                                        if (IsCreditPullNothing == 2) // pull
                                        {
                                            //Both's sum we have available.
                                            if (itemoil.CustQConsumable.GetValueOrDefault(0) == 0 && itemoil.CustQConsumable.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
                                            {
                                                loopCurrentTakenConsignment = (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                itemoil.ConsignedQuantity = (itemoil.ConsignedQuantity ?? 0) - ((objDTO.TempPullQTY ?? 0) - takenQunatity);
                                                takenQunatity += (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                goto Save;
                                            }
                                            else if (itemoil.CustQConsumable.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
                                            {
                                                loopCurrentTakenCustomer = (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((objDTO.TempPullQTY ?? 0) - takenQunatity);
                                                takenQunatity += (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                goto Save;
                                            }
                                            else
                                            {
                                                takenQunatity += itemoil.CustQConsumable.GetValueOrDefault(0);
                                                loopCurrentTakenCustomer = itemoil.CustQConsumable.GetValueOrDefault(0);
                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - itemoil.CustQConsumable; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
                                                // needs to write logic for break down deduction from consigned or customer quantity location wise ...
                                                if (itemoil.ConQConsumable >= ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity))
                                                {
                                                    loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
                                                    itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
                                                    takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                    goto Save;
                                                }
                                                else
                                                {
                                                    loopCurrentTakenConsignment = itemoil.ConQConsumable.GetValueOrDefault(0);
                                                    takenQunatity += itemoil.ConQConsumable.GetValueOrDefault(0);
                                                    itemoil.ConsignedQuantity = itemoil.ConsignedQuantity - itemoil.ConQConsumable;
                                                }
                                                //PENDING -- loop by varialbe from mupliple locations...
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region "Customreowned Credit and Pull"

                                        if (IsCreditPullNothing == 1) // credit - customer owened - lot number
                                        {
                                            PullMasterViewDTO result = objPullDal.GetPullByItemGuidPlain(RoomID, CompanyID, ItemDTO.GUID, obj.ID);

                                            if (result != null)
                                            {
                                                double unSatalledDifferent = 0;

                                                if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
                                                {
                                                    // un-satled Customer Owned                   
                                                    unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                                                    if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
                                                    {
                                                        obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                    }
                                                    else
                                                    {
                                                        double qtyavailable = 0;
                                                        qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                                        obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                        takenQunatity += qtyavailable;
                                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + qtyavailable;
                                                        result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                    }

                                                }
                                                else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
                                                {
                                                    // un-satled Customer Owned                   
                                                    unSatalledDifferent = result.ConsignedQuantity.GetValueOrDefault(0) - result.CreditConsignedQuantity.GetValueOrDefault(0);

                                                    if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
                                                    {
                                                        obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                        result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
                                                    }
                                                    else
                                                    {
                                                        double qtyavailable = 0;
                                                        qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                                        obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                        takenQunatity += qtyavailable;
                                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + qtyavailable;
                                                        result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
                                                    }
                                                }
                                                result.WhatWhereAction = "Customreowned";
                                                objPullDal.Edit(result);
                                            }

                                        }
                                        else if (IsCreditPullNothing == 2) // pull
                                        {
                                            //Both's sum we have available.
                                            if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
                                            {
                                                loopCurrentTakenCustomer = (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                itemoil.CustomerOwnedQuantity = (itemoil.CustomerOwnedQuantity ?? 0) - ((objDTO.TempPullQTY ?? 0) - takenQunatity);
                                                takenQunatity += (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                goto Save;
                                            }
                                            else
                                            {
                                                loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                                takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
                                            }
                                        }
                                        #endregion
                                    }
                                Save:

                                    #region For Rool Level Average Cost and Customerowned Item

                                    if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                        && loopCurrentTakenConsignment == 0)
                                    {
                                        itemoil.SellPrice = itemoil.Cost + (((itemoil.Cost ?? 0) * (ItemDTO.Markup ?? 0)) / 100);
                                    }
                                    #endregion

                                    double? itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, itemoil.SellPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    double? ItemSellPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, itemoil.Cost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    if ((objDTO.ActionType.ToLower() == "pull" || objDTO.ActionType.ToLower() == "ms pull")
                                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull
                                        && EditedSellPrice != null)
                                    {
                                        itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, ItemPullCost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                        ItemSellPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, ItemPullPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    }
                                    if (IsCreditPullNothing == 1)
                                    {
                                        if (ItemDTO.Consignment == true)
                                        {
                                            itemoil.ConsignedQuantity = objDTO.PoolQuantity;
                                            itemoil.CustomerOwnedQuantity = 0;
                                            loopCurrentTakenConsignment = objDTO.PoolQuantity ?? 0;
                                        }
                                        else
                                        {
                                            itemoil.ConsignedQuantity = 0;
                                            itemoil.CustomerOwnedQuantity = objDTO.PoolQuantity;
                                            loopCurrentTakenCustomer = objDTO.PoolQuantity ?? 0;
                                        }
                                    }

                                    objLocationDAL.Edit(itemoil);
                                    itemoil.PullCredit = objDTO.PullCredit;
                                    obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID, itemCost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, ItemSellPrice);
                                }

                                if (objDTO.PoolQuantity > takenQunatity)
                                {
                                    DoILAndPDNegative(ReturnDto, takenQunatity, ItemDTO);

                                    #region WI-5979 -- while negative pull pullcost and pull price not updated
                                    //Edit pull master
                                    double PullQty = (ReturnDto.PoolQuantity ?? 0) - takenQunatity;

                                    double? PullitemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, ItemDTO.Consignment, ItemPullCost, ItemPullCost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    double? PullItemSellPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, ItemDTO.Consignment, ItemPullPrice, ItemPullPrice, ItemDTO.CostUOMID, RoomID, CompanyID);

                                    PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
                                    PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(ReturnDto.GUID);

                                    if (objPullDTO != null && objPullDTO.ID > 0)
                                    {
                                        objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (PullQty * PullitemCost.GetValueOrDefault(0));
                                        objPullDTO.PullPrice = (objPullDTO.PullPrice.GetValueOrDefault(0)) + (PullQty * PullItemSellPrice.GetValueOrDefault(0));
                                        objPullMAster.Edit(objPullDTO);

                                        obj.PullCost = objPullDTO.PullCost;
                                        obj.PullPrice = objPullDTO.PullPrice;
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region "ItemLocation Quantity Deduction"

                        if (IsCreditPullNothing == 1) // credit
                        {
                            if (!ItemDTO.Consignment)
                            {
                                obj.CreditCustomerOwnedQuantity = objDTO.TempPullQTY;
                                obj.CreditConsignedQuantity = 0;

                                ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
                                lstLocDTO.CustomerOwnedQuantity = (lstLocDTO.CustomerOwnedQuantity ?? 0) + (objDTO.TempPullQTY ?? 0);
                                lstLocDTO.Quantity = lstLocDTO.Quantity + (objDTO.TempPullQTY ?? 0);
                            }
                            else
                            {
                                /// PENDING
                                //get last pull record to know the quantity....Credit Unsetteled records
                                obj.CreditCustomerOwnedQuantity = 0;
                                obj.CreditConsignedQuantity = objDTO.TempPullQTY;
                                ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
                                if (lstLocDTO != null)
                                {
                                    lstLocDTO.ConsignedQuantity = (lstLocDTO.ConsignedQuantity ?? 0) + (objDTO.TempPullQTY ?? 0);
                                    lstLocDTO.Quantity = lstLocDTO.Quantity + (objDTO.TempPullQTY ?? 0);
                                }
                            }
                        }
                        else if (IsCreditPullNothing == 2) // pull
                        {
                            ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - objDTO.TempPullQTY;

                            if (ItemDTO.Consignment)
                            {
                                //Both's sum we have available.
                                if (lstLocDTO != null)
                                {
                                    if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                                    {
                                        obj.ConsignedQuantity = objDTO.TempPullQTY ?? 0;
                                        lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - objDTO.TempPullQTY;
                                        lstLocDTO.Quantity = lstLocDTO.Quantity - (objDTO.TempPullQTY ?? 0);
                                    }
                                    else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY ?? 0))
                                    {
                                        obj.CustomerOwnedQuantity = objDTO.TempPullQTY ?? 0;
                                        lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - (objDTO.TempPullQTY ?? 0);
                                        lstLocDTO.Quantity = lstLocDTO.Quantity - (objDTO.TempPullQTY ?? 0);
                                    }
                                    else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < (objDTO.TempPullQTY ?? 0))
                                    {
                                        double cstqty = (objDTO.TempPullQTY ?? 0) - lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                        double consqty = cstqty;
                                        lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
                                        obj.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                        obj.ConsignedQuantity = consqty;
                                        lstLocDTO.CustomerOwnedQuantity = 0;
                                        lstLocDTO.Quantity = lstLocDTO.Quantity - (obj.CustomerOwnedQuantity.GetValueOrDefault(0) + obj.ConsignedQuantity.GetValueOrDefault(0));
                                    }
                                }
                            }
                            else
                            {
                                if (lstLocDTO != null)
                                {
                                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objDTO.TempPullQTY.GetValueOrDefault(0);
                                    lstLocDTO.Quantity = lstLocDTO.Quantity - (objDTO.TempPullQTY ?? 0);
                                }
                                obj.CustomerOwnedQuantity = objDTO.TempPullQTY ?? 0;
                            }
                        }


                        #endregion

                        #region "Saving Location and QTY data"
                        // update requisition qty

                        if (objDTO.RequisitionDetailGUID != null && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            RequisitionDetailsDAL objReqDtlDal = new RequisitionDetailsDAL(base.DataBaseName);
                            RequisitionDetailsDTO objRequDetDTO = objReqDtlDal.GetRequisitionDetailsByGUIDPlain(objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty));

                            if (objRequDetDTO != null && (objRequDetDTO.RequisitionStatus == "Approved"))
                            {
                                ItemMasterDTO ItemDTO1 = new RequisitionDetailsDAL(base.DataBaseName).UpdateItemOnRequisitionQty(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0));
                                ItemDTO.RequisitionedQuantity = ItemDTO1.RequisitionedQuantity;
                            }
                        }

                        ItemDTO.WhatWhereAction = objDTO.PullCredit;
                        objItemDAL.Edit(ItemDTO, SessionUserId, EnterpriseID);
                        List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                        if (lstLocDTO == null)
                        {
                            lstLocDTO = new ItemLocationQTYDTO();
                            lstLocDTO.ID = 0;
                            lstLocDTO.BinID = objDTO.BinID.GetValueOrDefault(0);
                            lstLocDTO.Quantity = objDTO.PoolQuantity.GetValueOrDefault();
                            lstLocDTO.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                            lstLocDTO.ConsignedQuantity = objDTO.ConsignedQuantity;
                            lstLocDTO.LotNumber = objDTO.LotNumber;
                            lstLocDTO.GUID = Guid.NewGuid();
                            lstLocDTO.ItemGUID = objDTO.ItemGUID;
                            lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                            lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            lstLocDTO.CreatedBy = objDTO.CreatedBy;
                            lstLocDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                            lstLocDTO.Room = objDTO.Room;
                            lstLocDTO.CompanyID = objDTO.CompanyID;
                            lstLocDTO.AddedFrom = "Web";
                            lstLocDTO.EditedFrom = "Web";
                            lstLocDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            lstLocDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objLocQTY.Insert(lstLocDTO, SessionUserId, EnterpriseID);
                        }
                        lstUpdate.Add(lstLocDTO);
                        objLocQTY.Save(lstUpdate, SessionUserId, EnterpriseID);
                        #endregion

                        #endregion

                        #region "Project Spend Quantity Update"

                        if (objPullDal.Edit(obj))
                        {

                            if (objDTO.ProjectSpendGUID != null && objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
                            }
                        }
                        if (IsCreditPullNothing == 2 && obj != null && (obj.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 || obj.ConsignedQuantity.GetValueOrDefault(0) == 0))
                        {
                            List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                            if (lstPullDtl != null && lstPullDtl.Count > 0)
                            {
                                obj.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                obj.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                                objPullDal.Edit(obj);
                            }
                        }
                        #endregion

                        #region "Update Turns and Average Usgae"
                        UpdateTurnsAverageUsage(obj, SessionUserId);
                        #endregion

                        #region "Update Ext Cost And Avg Cost"
                        new ItemMasterDAL(base.DataBaseName).GetAndUpdateExtCostAndAvgCost(ItemDTO.GUID, RoomID, CompanyID);
                        #endregion
                    }
                    else
                    {
                        List<MaterialStagingPullDetailDTO> ObjItemLocationMS = null;
                        MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                        MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                        List<MaterialStagingDetailDTO> lstLocDTOMS = new List<MaterialStagingDetailDTO>();
                        List<MaterialStagingPullDetailDTO> lstPullDetailsDTO = objMaterialStagingPullDetailDAL.GetAvialableMsPullDetailsByItemGUIDANDBinID(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID).ToList();
                        string InventoryConsuptionMethod = string.Empty;

                        if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                        {
                            InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                        }

                        if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                            InventoryConsuptionMethod = "";

                        switch (InventoryConsuptionMethod.ToLower())
                        {
                            case "lifo":
                            case "lifooverride":
                                lstPullDetailsDTO = lstPullDetailsDTO.OrderByDescending(x => x.ReceivedDate).ToList();
                                break;
                            case "fifo":
                            case "fifooverride":
                                lstPullDetailsDTO = lstPullDetailsDTO.OrderBy(x => x.ReceivedDate).ToList();
                                break;
                            default:
                                lstPullDetailsDTO = lstPullDetailsDTO.OrderBy(x => x.ReceivedDate).ToList();
                                break;
                        }

                        //Pick up the locations.....
                        if (ItemDTO.SerialNumberTracking)
                        {
                            ObjItemLocationMS = lstPullDetailsDTO.Take((int)objDTO.TempPullQTY).ToList();
                        }
                        else
                        {
                            ObjItemLocationMS = lstPullDetailsDTO;
                        }

                        #region "Project Wise Quantity Check"

                        if (objDTO.ProjectSpendGUID != null)
                        {
                            var tmpsupplierIds = new List<long>();
                            //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objDTO.ItemGUID).FirstOrDefault();
                            objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(objDTO.ProjectSpendGUID.Value, RoomID, CompanyID, tmpsupplierIds, Convert.ToString(objDTO.ItemGUID)).FirstOrDefault();
                            objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);

                            if (ProjectWiseQuantityCheck(objPrjSpenItmDTO, objPrjMstDTO, out ItemLocationMSG, objDTO, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, EnterpriseID, CultureCode, CompanyID, RoomID))
                            {
                                return true;
                            }
                        }
                        #endregion

                        #region "Pull Insert Update"

                        if (ItemDTO.Consignment == true)
                        {
                            objDTO.ConsignedQuantity = objDTO.PoolQuantity;
                            objDTO.CustomerOwnedQuantity = 0;
                            obj.ConsignedQuantity = objDTO.PoolQuantity;
                            obj.CustomerOwnedQuantity = 0;
                        }
                        else
                        {
                            objDTO.ConsignedQuantity = 0;
                            objDTO.CustomerOwnedQuantity = objDTO.PoolQuantity;
                            obj.ConsignedQuantity = 0;
                            obj.CustomerOwnedQuantity = objDTO.PoolQuantity;
                        }

                        obj.PoolQuantity = objDTO.PoolQuantity;

                        if (objDTO.ActionType == null || objDTO.ActionType == "MS Pull")
                        {
                            obj.ActionType = "MS Pull";
                            obj.PullCredit = "MS Pull";
                            objDTO.ActionType = "MS Pull";
                            objDTO.PullCredit = "MS Pull";
                            objDTO.WhatWhereAction = "MS Pull";
                        }
                        else
                        {
                            obj.PullCredit = objDTO.ActionType;
                            objDTO.ActionType = objDTO.ActionType;
                            objDTO.PullCredit = objDTO.ActionType;
                            objDTO.WhatWhereAction = objDTO.ActionType;
                        }
                        ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal, SessionUserId);
                        #endregion

                        #region "Item Location & Quantity  Wise Deduction"

                        #region "ItemLocation Deduction"

                        if (IsCreditPullNothing != 3)
                        {
                            if (ItemDTO.SerialNumberTracking)
                            {
                                #region "Serial logic"

                                double LocalSearilaCount = 0;

                                foreach (var itemoil in ObjItemLocationMS)
                                {
                                    double loopCurrentTakenCustomer = 0;
                                    double loopCurrentTakenConsignment = 0;

                                    /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                                    if (ItemDTO.Consignment)
                                    {
                                        #region "Consignment Credit and Pull"

                                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                                        {
                                            loopCurrentTakenConsignment = 1;
                                            itemoil.ConsignedQuantity = 0;
                                        }
                                        else
                                        {
                                            loopCurrentTakenCustomer = 1;
                                            itemoil.CustomerOwnedQuantity = 0;
                                        }

                                        #endregion
                                    }
                                    else //customerowendQuantity
                                    {
                                        loopCurrentTakenCustomer = 1;
                                        itemoil.CustomerOwnedQuantity = 0;
                                    }

                                    itemoil.PoolQuantity = (itemoil.ConsignedQuantity.GetValueOrDefault(0) + itemoil.CustomerOwnedQuantity.GetValueOrDefault(0));

                                    #region For Rool Level Average Cost and Customerowned Item

                                    if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                        && loopCurrentTakenConsignment == 0)
                                    {
                                        itemoil.SellPrice = itemoil.ItemCost + (((itemoil.ItemCost ?? 0) * (ItemDTO.Markup ?? 0)) / 100);
                                    }
                                    #endregion

                                    double? itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, itemoil.SellPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    double? ItemSellPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, itemoil.ItemCost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    if ((objDTO.ActionType.ToLower() == "pull" || objDTO.ActionType.ToLower() == "ms pull")
                                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull
                                        && EditedSellPrice != null)
                                    {
                                        itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, ItemPullCost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                        ItemSellPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, ItemPullPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    }
                                    objMaterialStagingPullDetailDAL.Edit(itemoil);
                                    itemoil.PullCredit = objDTO.ActionType;
                                    obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), Convert.ToDouble(itemCost.GetValueOrDefault(0)), obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, Convert.ToDouble(ItemSellPrice.GetValueOrDefault(0)));
                                    //MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                                    MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(itemoil.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                                    objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
                                    objMaterialStagingDetailDAL.Edit(objmsddto);
                                }
                                obj.CreditCustomerOwnedQuantity = LocalSearilaCount;

                                #endregion
                            }
                            else
                            {
                                #region "LOt and other type logic"

                                Double takenQunatity = 0;

                                foreach (var itemoil in ObjItemLocationMS)
                                {
                                    Double loopCurrentTakenCustomer = 0;
                                    Double loopCurrentTakenConsignment = 0;
                                    /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                                    if (takenQunatity == objDTO.TempPullQTY)
                                    {
                                        break;
                                    }

                                    if (ItemDTO.Consignment)
                                    {
                                        #region "Consignment Credit and Pull"

                                        //Both's sum we have available.
                                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
                                        {
                                            loopCurrentTakenConsignment = (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                            itemoil.ConsignedQuantity = (itemoil.ConsignedQuantity ?? 0) - ((objDTO.TempPullQTY ?? 0) - takenQunatity);
                                            takenQunatity += (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                            goto Save;
                                        }
                                        else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
                                        {
                                            loopCurrentTakenCustomer = (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                            itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((objDTO.TempPullQTY ?? 0) - takenQunatity);
                                            takenQunatity += (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                            goto Save;
                                        }
                                        else
                                        {
                                            takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                            itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
                                            // needs to write logic for break down deduction from consigned or customer quantity location wise ...
                                            if (itemoil.ConsignedQuantity >= (objDTO.TempPullQTY ?? 0) - takenQunatity)
                                            {
                                                loopCurrentTakenConsignment = (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((objDTO.TempPullQTY ?? 0) - takenQunatity);
                                                takenQunatity += (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                                goto Save;
                                            }
                                            else
                                            {
                                                loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                                takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                                itemoil.ConsignedQuantity = 0;
                                            }
                                            //PENDING -- loop by varialbe from mupliple locations...
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region "Customreowned Credit and Pull"

                                        //Both's sum we have available.
                                        if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
                                        {
                                            loopCurrentTakenCustomer = (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                            itemoil.CustomerOwnedQuantity = (itemoil.CustomerOwnedQuantity ?? 0) - ((objDTO.TempPullQTY ?? 0) - takenQunatity);
                                            takenQunatity += (objDTO.TempPullQTY ?? 0) - takenQunatity;
                                            goto Save;
                                        }
                                        else
                                        {
                                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                            takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                            itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
                                        }

                                        #endregion
                                    }
                                Save:
                                    #region For Rool Level Average Cost and Customerowned Item

                                    if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                        && loopCurrentTakenConsignment == 0)
                                    {
                                        itemoil.SellPrice = itemoil.ItemCost + (((itemoil.ItemCost ?? 0) * (ItemDTO.Markup ?? 0)) / 100);
                                    }
                                    #endregion

                                    double? itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, itemoil.SellPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    double? itemPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, itemoil.ItemCost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    if ((objDTO.ActionType.ToLower() == "pull" || objDTO.ActionType.ToLower() == "ms pull")
                                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull
                                        && EditedSellPrice != null)
                                    {
                                        itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullCost, ItemPullCost, ItemDTO.CostUOMID, RoomID, CompanyID);
                                        itemPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemPullPrice, ItemPullPrice, ItemDTO.CostUOMID, RoomID, CompanyID);
                                    }
                                    itemoil.PoolQuantity = (itemoil.ConsignedQuantity.GetValueOrDefault(0) + itemoil.CustomerOwnedQuantity.GetValueOrDefault(0));
                                    objMaterialStagingPullDetailDAL.Edit(itemoil);
                                    itemoil.PullCredit = objDTO.ActionType;
                                    obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), Convert.ToDouble(itemCost.GetValueOrDefault(0)), obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, Convert.ToDouble(itemPrice.GetValueOrDefault(0)));

                                    //MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                                    MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(itemoil.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                                    if (objmsddto != null)
                                    {
                                        objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
                                        objMaterialStagingDetailDAL.Edit(objmsddto);
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                        ItemDTO.WhatWhereAction = objDTO.ActionType;
                        objItemDAL.Edit(ItemDTO, SessionUserId, EnterpriseID);

                        #endregion

                        //Update started quantity...
                        objMaterialStagingPullDetailDAL.UpdateStagedQuantity(ItemDTO.GUID, RoomID, CompanyID, SessionUserId, EnterpriseID);
                        //Updated PS

                        #region "Project Spend Quantity Update"

                        if (objPullDal.Edit(obj))
                        {
                            if (objDTO.ProjectSpendGUID != null && objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }

            if (!IsStagginLocation && ItemDTO != null && ItemDTO.GUID != Guid.Empty)
            {
                ItemMasterDAL objItemMaster = new ItemMasterDAL(base.DataBaseName);
                objItemMaster.EditDate(ItemDTO.GUID, "EditPulledDate");
            }

            if (objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                objWOLDAL.UpdateWOItemAndTotalCost(objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
            }
            UpdateCumulativeOnHand(ReturnDto);
            return true;
        }

        public double? CalculateAndGetPullCost(string MethodOfValuingInventory, bool IsCosigned, double? itemCost, double? itemDetailCost, long? costUOM, Int64 RoomID, Int64 CompanyID)
        {
            itemCost = itemCost ?? 0;
            double? pullCost = null;
            int costUOMValue = 1;
            CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(costUOM.GetValueOrDefault(0));
            if (oCostUOMMasterDTO != null)
            {
                costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                if (costUOMValue == 0)
                    costUOMValue = 1;
            }

            if (MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString())
            {
                if (itemCost.HasValue)
                    pullCost = itemCost.Value / costUOMValue;
            }
            else if (MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString())
            {
                if (IsCosigned)
                {
                    if (itemCost.HasValue)
                        pullCost = itemCost.Value / costUOMValue;
                }
                else
                {
                    if (itemDetailCost.HasValue)
                        pullCost = itemDetailCost.Value / costUOMValue;
                }
            }
            else
            {
                if (itemDetailCost.HasValue)
                    pullCost = itemCost.Value / costUOMValue;
            }

            return pullCost;
        }
        public double? CalculateAndGetPullPrice(string MethodOfValuingInventory, bool IsCosigned, double? itemCost, double? itemDetailCost, long? costUOM, Int64 RoomID, Int64 CompanyID)
        {
            itemCost = itemCost ?? 0;
            double? pullPrice = null;
            int costUOMValue = 1;
            CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(costUOM.GetValueOrDefault(0));
            if (oCostUOMMasterDTO != null)
            {
                costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                if (costUOMValue == 0)
                    costUOMValue = 1;
            }

            if (MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString())
            {
                if (itemCost.HasValue)
                    pullPrice = itemCost.Value / costUOMValue;
            }
            else if (MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString())
            {
                if (IsCosigned)
                {
                    if (itemCost.HasValue)
                        pullPrice = itemCost.Value / costUOMValue;
                }
                else
                {
                    if (itemDetailCost.HasValue)
                        pullPrice = itemDetailCost.Value / costUOMValue;
                }
            }
            else
            {
                if (itemDetailCost.HasValue)
                    pullPrice = itemCost.Value / costUOMValue;
            }

            return pullPrice;
        }

        public bool CheckValidPullData(PullMasterViewDTO objDTO, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, out string ItemLocationMSG, bool IsProjectSpendAllowed,
                                        out bool IsPSLimitExceed, long EnterpriseId, string CultureCode)
        {
            #region "Global Variables"

            ItemLocationMSG = "";
            IsPSLimitExceed = false;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
            PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO obj = new PullMasterViewDTO();
            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
            BinMasterDTO objBINDTO = new BinMasterDTO();
            bool IsStagginLocation = false;
            #endregion

            BinMasterDAL objLocDAL = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO objLocDTO = objLocDAL.GetBinByID(objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID);

            if (objLocDTO != null && objLocDTO.ID > 0)
            {
                if (objLocDTO.IsStagingLocation)
                {
                    IsStagginLocation = true;
                }
            }

            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

            if (ItemDTO != null && ItemDTO.ItemType == 4)
            {
                return true;
            }
            else
            {
                #region "For Item Pull"
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //if staging location then check qty on staging module
                    if (IsStagginLocation)
                    {
                        #region "Stagging Bin Wise Quantity Check"
                        double retval = 0;
                        List<MaterialStagingDetailDTO> lstMaterialStagingDetailDTO = new MaterialStagingDetailDAL(base.DataBaseName)
                                                            .GetMaterialStagingDetailByItemGUIDAndBinIDPlain(ItemDTO.GUID, objDTO.BinID);
                        if (lstMaterialStagingDetailDTO != null && lstMaterialStagingDetailDTO.Count() > 0)
                        {
                            retval = lstMaterialStagingDetailDTO.Sum(t => t.Quantity);
                        }
                        if (retval < objDTO.TempPullQTY)
                        {
                            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                            string msgNotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                            ItemLocationMSG = string.Format(msgNotEnoughQuantityForLocation, objLocDTO.BinNumber, retval.ToString());
                            return true;
                        }
                        #endregion
                    }
                    else
                    {
                        #region "Bin Wise Quantity Check"
                        //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).SingleOrDefault();
                        lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objDTO.BinID, Convert.ToString(objDTO.ItemGUID.Value)).FirstOrDefault();
                        objBINDTO = objBINDAL.GetBinByID((Int64)objDTO.BinID, RoomID, CompanyID);

                        if (BinWiseQuantityCheck(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, EnterpriseId, CultureCode))
                        {
                            return true;
                        }
                        #endregion
                    }
                    return true;
                }
                #endregion
            }
        }

        /// <summary>
        /// Sub Routine to calculate Project Spend values with Item location with costing...
        /// </summary>
        /// <param name="objPullDTO"></param>
        /// <param name="ProjectSpendGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        public void UpdateProjectSpendWithCost(ItemMasterDTO ItemDTO, PullMasterViewDTO objPullDTO, Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyID)
        {
            ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
            ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
            objProjectSpendDTO = objProjectDAL.GetRecord(ProjectSpendGUID, RoomID, CompanyID, false, false);

            if (objProjectSpendDTO != null)
            {
                objProjectSpendDTO.DollarUsedAmount = objProjectSpendDTO.DollarUsedAmount.GetValueOrDefault(0) + (decimal)objPullDTO.PullCost.GetValueOrDefault(0);
                objProjectSpendDTO.WhatWhereAction = "Pull";
                objProjectDAL.Edit(objProjectSpendDTO);
                ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                var tmpsupplierIds = new List<long>();
                //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(ProjectSpendGUID, RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == ItemDTO.GUID).FirstOrDefault(); //&& x.Created.Value <= ItemDTO.Created.Value
                objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(ProjectSpendGUID, RoomID, CompanyID, tmpsupplierIds, Convert.ToString(ItemDTO.GUID)).FirstOrDefault();

                if (objPrjSpenItmDTO != null)
                {
                    //Update Quanitty used limit
                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) + (decimal)objPullDTO.PullCost.GetValueOrDefault(0);
                    objPrjSpenItmDTO.QuantityUsed = (double)objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) + (double)objPullDTO.PoolQuantity.GetValueOrDefault(0);
                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                }
                else
                {
                    objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                    objPrjSpenItmDTO.Action = "insert";
                    objPrjSpenItmDTO.AddedFrom = "web";
                    objPrjSpenItmDTO.CompanyID = objProjectSpendDTO.CompanyID;
                    objPrjSpenItmDTO.Created = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.CreatedBy = objPullDTO.CreatedBy;
                    objPrjSpenItmDTO.Description = "Added from pull";
                    objPrjSpenItmDTO.DollarLimitAmount = null;
                    objPrjSpenItmDTO.DollarUsedAmount = (decimal)objPullDTO.PullCost.GetValueOrDefault(0);
                    objPrjSpenItmDTO.EditedFrom = "web";
                    objPrjSpenItmDTO.GUID = Guid.NewGuid();
                    objPrjSpenItmDTO.ID = 0;
                    objPrjSpenItmDTO.IsArchived = false;
                    objPrjSpenItmDTO.IsDeleted = false;
                    objPrjSpenItmDTO.ItemGUID = ItemDTO.GUID;
                    objPrjSpenItmDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.LastUpdatedBy = objPullDTO.CreatedBy;
                    objPrjSpenItmDTO.ProjectGUID = objProjectSpendDTO.GUID;
                    objPrjSpenItmDTO.QuantityLimit = null;
                    objPrjSpenItmDTO.QuantityUsed = (double)objPullDTO.PoolQuantity.GetValueOrDefault(0);
                    objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.Room = objProjectSpendDTO.Room;
                    new ProjectSpendItemsDAL(base.DataBaseName).Insert(objPrjSpenItmDTO);
                }

                objProjectSpendDTO.ProjectSpendItems = new List<ProjectSpendItemsDTO>();
                objProjectSpendDTO.ProjectSpendItems.Add(objPrjSpenItmDTO);
                /*////////CODE FOR SEND MAIL OF PROJECT SPEND LIMIT EXCEED///////////*/
                SendEmailForProjectSpendLimitExceed(objProjectSpendDTO);
                /*////////CODE FOR SEND MAIL OF PROJECT SPEND LIMIT EXCEED///////////*/

            }

        }

        public bool PullCreditAfterUndelete(PullMasterViewDTO objDTO, long RoomID, long CompanyID, out string MSG, long SessionUserId, long EnterpriseId)
        {
            MSG = string.Empty;

            //Get PullDetails Data 
            PullDetailsDAL objPullDetailDal = new PullDetailsDAL(base.DataBaseName);
            List<PullDetailsDTO> lstPullDetailsDTO = objPullDetailDal.GetPullDetailsByPullGuidPlain(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0)).OrderByDescending(e => e.ID).ToList();

            //Get ItemMaster Data
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

            foreach (var item in lstPullDetailsDTO)
            {
                if (item.MaterialStagingPullDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                {
                    ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);

                    #region "Location Details"

                    if (ItemDTO.SerialNumberTracking)
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID.Value, null, "ID ASC").Where(t => t.SerialNumber == item.SerialNumber && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).SingleOrDefault();

                        if (objItemDetail != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                        //}
                    }
                    else if (ItemDTO.LotNumberTracking)
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID.Value, null, "ID ASC").Where(t => t.LotNumber == item.LotNumber && t.BinID == item.BinID && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).SingleOrDefault();

                        if (objItemDetail != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                    }
                    else if (ItemDTO.DateCodeTracking)
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID.Value, null, "ID ASC").Where(t => t.BinID == item.BinID && t.Expiration == item.Expiration && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).Take(1).SingleOrDefault();
                        if (objItemDetail != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                    }
                    else
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID.Value, null, "ID ASC").Where(t => t.BinID == item.BinID && t.Received == item.Received && t.IsDeleted == false && t.GUID == item.ItemLocationDetailGUID).Take(1).SingleOrDefault();

                        if (objItemDetail != null)
                        {
                            if (objDTO.PullType == Convert.ToInt32(PullInsertTypeEnum.WebImportPullHistory))
                            {
                                objItemDetail.CustomerOwnedQuantity = 0;
                                objItemDetail.ConsignedQuantity = 0;
                            }
                            else
                            {
                                if (objDTO.PullCredit.ToLower() == "pull")
                                {
                                    objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                }
                                else
                                {
                                    objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                                }
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                    }

                    //item locations qty update
                    //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == item.BinID && x.ItemGUID == item.ItemGUID).SingleOrDefault();
                    ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, item.BinID, Convert.ToString(item.ItemGUID)).FirstOrDefault();

                    if (lstLocDTO != null)
                    {
                        if (objDTO.PullType == Convert.ToInt32(PullInsertTypeEnum.WebImportPullHistory))
                        {
                            lstLocDTO.CustomerOwnedQuantity = 0;
                            lstLocDTO.ConsignedQuantity = 0;
                        }
                        else
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                        }
                        lstLocDTO.Quantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                        List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                        lst.Add(lstLocDTO);
                        objLocQTY.Save(lst, SessionUserId, EnterpriseId);
                    }

                    //Project Spend update
                    if (item.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                        ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                        ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                        ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objDTO.ItemGUID && x.Created.Value <= item.Created.Value).SingleOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, tmpsupplierIds, Convert.ToString(objDTO.ItemGUID), item.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);

                        if (objPrjMstDTO != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }
                            else
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }

                            objPrjMsgDAL.Edit(objPrjMstDTO);

                            if (objPrjSpenItmDTO != null)
                            {
                                //Update Quanitty used limit
                                if (objDTO.PullCredit.ToLower() == "pull")
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) + item.PoolQuantity.GetValueOrDefault(0);
                                }
                                else
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - item.PoolQuantity.GetValueOrDefault(0);
                                }
                                objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region "Stage location"

                    MaterialStagingPullDetailDAL objLocationDALMS = new MaterialStagingPullDetailDAL(base.DataBaseName);
                    MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    bool IsUndelete = false;
                    IsUndelete = objLocationDALMS.UnDeleteRecordsByGUID(Convert.ToString(item.MaterialStagingPullDetailGUID.GetValueOrDefault(Guid.Empty)), item.LastUpdatedBy.GetValueOrDefault(0), CompanyID);
                    Guid MSDGUID = Guid.Empty;

                    if (ItemDTO.SerialNumberTracking)
                    {
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID).Where(t => t.SerialNumber == item.SerialNumber && t.GUID == item.MaterialStagingPullDetailGUID && t.IsDeleted == false).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, string.Empty, item.SerialNumber, string.Empty, string.Empty, 0, Convert.ToString(item.MaterialStagingPullDetailGUID.GetValueOrDefault(Guid.Empty)), false).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.UnDeleteRecords(objItemDetail.ID.ToString(), objItemDetail.LastUpdatedBy.GetValueOrDefault(0), CompanyID);
                            }

                        }
                    }
                    else if (ItemDTO.LotNumberTracking)
                    {
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID).Where(t => t.LotNumber == item.LotNumber && t.StagingBinId == item.BinID && t.GUID == item.MaterialStagingPullDetailGUID && t.IsDeleted == false).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID, item.LotNumber, string.Empty, string.Empty, string.Empty, item.BinID, Convert.ToString(item.MaterialStagingPullDetailGUID), false).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.UnDeleteRecords(objItemDetail.ID.ToString(), objItemDetail.LastUpdatedBy.GetValueOrDefault(0), CompanyID);
                            }

                        }
                    }
                    else if (ItemDTO.DateCodeTracking)
                    {
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID).Where(t => t.StagingBinId == item.BinID && t.Expiration == item.Expiration && t.GUID == item.MaterialStagingPullDetailGUID && t.IsDeleted == false).Take(1).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID, string.Empty, string.Empty, item.Expiration, string.Empty, item.BinID, Convert.ToString(item.MaterialStagingPullDetailGUID), false).Take(1).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.UnDeleteRecords(objItemDetail.ID.ToString(), objItemDetail.LastUpdatedBy.GetValueOrDefault(0), CompanyID);
                            }
                        }
                    }
                    else
                    {
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID).Where(t => t.StagingBinId == item.BinID && t.Received == item.Received && t.IsDeleted == false && t.GUID == item.MaterialStagingPullDetailGUID).Take(1).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID, string.Empty, string.Empty, string.Empty, item.Received, item.BinID, Convert.ToString(item.MaterialStagingPullDetailGUID), false).Take(1).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.Updated = System.DateTime.Now;
                                objLocationDALMS.UnDeleteRecords(objItemDetail.ID.ToString(), objItemDetail.LastUpdatedBy.GetValueOrDefault(0), CompanyID);
                            }
                        }
                    }

                    //item locations qty update - TO DO MaterialStagingDetail
                    if (MSDGUID != Guid.Empty)
                    {
                        //MaterialStagingDetailDTO objmsd = objMaterialStagingDetailDAL.GetRecord(MSDGUID, RoomID, CompanyID);
                        MaterialStagingDetailDTO objmsd = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(MSDGUID, RoomID, CompanyID);

                        if (objmsd != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objmsd.Quantity = objmsd.Quantity - (item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0));
                                objMaterialStagingDetailDAL.Edit(objmsd);
                            }
                            else
                            {
                                objmsd.Quantity = objmsd.Quantity + (item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0));
                                objMaterialStagingDetailDAL.UnDeleteRecords(objmsd.ID.ToString(), objmsd.LastUpdatedBy, CompanyID, RoomID);
                            }

                        }
                    }

                    //Project Spend update
                    if (item.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                        ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                        ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                        ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objDTO.ItemGUID && x.Created.Value <= item.Created.Value).SingleOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, tmpsupplierIds, Convert.ToString(objDTO.ItemGUID), item.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);

                        if (objPrjMstDTO != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }
                            else
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }

                            objPrjMsgDAL.Edit(objPrjMstDTO);

                            if (objPrjSpenItmDTO != null)
                            {
                                //Update Quanitty used limit
                                if (objDTO.PullCredit.ToLower() == "pull")
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) + item.PoolQuantity.GetValueOrDefault(0);
                                }
                                else
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - item.PoolQuantity.GetValueOrDefault(0);
                                }
                                objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                            }
                        }
                    }
                    #endregion
                    //Update started quantity...
                    objLocationDALMS.UpdateStagedQuantity(ItemDTO.GUID, RoomID, CompanyID, SessionUserId, EnterpriseId);
                }
            }

            return true;
        }

        public bool PullCreditAfterDelete(PullMasterViewDTO objDTO, long RoomID, long CompanyID, out string MSG, long SessionUserId, long EnterpriseId)
        {
            MSG = "";
            //get all pull details 
            PullDetailsDAL objPullDetailDal = new PullDetailsDAL(base.DataBaseName);
            List<PullDetailsDTO> lstPullDetailsDTO = objPullDetailDal.GetPullDetailsByPullGuidPlain(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0)).OrderByDescending(e => e.ID).ToList();
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
            BinMasterDAL objLocDAL = new BinMasterDAL(base.DataBaseName);

            foreach (var item in lstPullDetailsDTO)
            {
                if (item.MaterialStagingPullDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                {
                    #region When pull deteld for deleted bin, un delted bin and then credit back
                    objLocDAL.UnDeleteLocation(item.BinID.GetValueOrDefault(0), RoomID, CompanyID);
                    #endregion

                    ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                    #region "Location Delete"
                    //item locations details

                    if (ItemDTO.SerialNumberTracking)
                    {
                        CommonDAL objCDal = new CommonDAL(base.DataBaseName);
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID.Value, null, "ID ASC").Where(t => t.GUID == item.ItemLocationDetailGUID).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                    }
                    else if (ItemDTO.LotNumberTracking)
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID.Value, null, "ID ASC").Where(t => t.GUID == item.ItemLocationDetailGUID).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = (objItemDetail.ConsignedQuantity ?? 0) + (item.ConsignedQuantity ?? 0);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                    }
                    else if (ItemDTO.DateCodeTracking)
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID.Value, null, "ID ASC").Where(t => t.GUID == item.ItemLocationDetailGUID).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                    }
                    else
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetRecordsByItemGuid((Guid)item.ItemGUID, RoomID, CompanyID).Where(t => t.GUID == item.ItemLocationDetailGUID).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            if (objDTO.PullType == Convert.ToInt32(PullInsertTypeEnum.WebImportPullHistory))
                            {
                                objItemDetail.CustomerOwnedQuantity = 0;
                                objItemDetail.ConsignedQuantity = 0;
                            }
                            else
                            {
                                if (objDTO.PullCredit.ToLower() == "pull")
                                {
                                    objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                                }
                                else
                                {
                                    objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                }
                            }
                            objItemDetail.Updated = DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                        }
                    }

                    //item locations qty update
                    //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == item.BinID && x.ItemGUID == item.ItemGUID).SingleOrDefault();
                    ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, item.BinID, Convert.ToString(item.ItemGUID)).FirstOrDefault();

                    if (lstLocDTO != null)
                    {
                        if (objDTO.PullType == Convert.ToInt32(PullInsertTypeEnum.WebImportPullHistory))
                        {
                            lstLocDTO.CustomerOwnedQuantity = 0;
                            lstLocDTO.ConsignedQuantity = 0;
                        }
                        else
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                            }
                        }
                        lstLocDTO.Quantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                        List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                        lst.Add(lstLocDTO);
                        objLocQTY.Save(lst, SessionUserId, EnterpriseId);
                    }

                    //Project Spend update
                    if (item.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                        ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                        ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                        ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objDTO.ItemGUID && x.Created.Value <= item.Created.Value).SingleOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, tmpsupplierIds, Convert.ToString(objDTO.ItemGUID), item.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);

                        if (objPrjMstDTO != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "pull")
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }
                            else
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }

                            objPrjMsgDAL.Edit(objPrjMstDTO);

                            if (objPrjSpenItmDTO != null)
                            {
                                //Update Quanitty used limit
                                if (objDTO.PullCredit.ToLower() == "pull")
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - item.PoolQuantity.GetValueOrDefault(0);
                                }
                                else
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) + item.PoolQuantity.GetValueOrDefault(0);
                                }
                                objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region "Stage location Delete"

                    MaterialStagingPullDetailDAL objLocationDALMS = new MaterialStagingPullDetailDAL(base.DataBaseName);
                    MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    Guid MSDGUID = Guid.Empty;
                    #region When pull deteld for deleted bin, un delted bin and then credit back
                    objLocDAL.UnDeleteLocation(item.BinID.GetValueOrDefault(0), RoomID, CompanyID);
                    #endregion
                    if (ItemDTO.SerialNumberTracking)
                    {
                        CommonDAL objCDal = new CommonDAL(base.DataBaseName);
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID).Where(t => t.SerialNumber == item.SerialNumber && t.GUID == item.MaterialStagingPullDetailGUID && t.IsDeleted == false).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, string.Empty, item.SerialNumber, string.Empty, string.Empty, 0, Convert.ToString(item.MaterialStagingPullDetailGUID), false).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                            if (objDTO.PullCredit.ToLower() == "ms pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                        }
                    }
                    else if (ItemDTO.LotNumberTracking)
                    {
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID).Where(t => t.LotNumber == item.LotNumber && t.StagingBinId == item.BinID && t.GUID == item.MaterialStagingPullDetailGUID && t.IsDeleted == false).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID, item.LotNumber, string.Empty, string.Empty, string.Empty, item.BinID, Convert.ToString(item.MaterialStagingPullDetailGUID), false).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                            if (objDTO.PullCredit.ToLower() == "ms pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                        }
                    }
                    else if (ItemDTO.DateCodeTracking)
                    {
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID).Where(t => t.StagingBinId == item.BinID && t.Expiration == item.Expiration && t.GUID == item.MaterialStagingPullDetailGUID && t.IsDeleted == false).Take(1).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID, string.Empty, string.Empty, item.Expiration, string.Empty, item.BinID, Convert.ToString(item.MaterialStagingPullDetailGUID), false).Take(1).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                            if (objDTO.PullCredit.ToLower() == "ms pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                        }
                    }
                    else
                    {
                        //MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID).Where(t => t.StagingBinId == item.BinID && t.Received == item.Received && t.IsDeleted == false && t.GUID == item.MaterialStagingPullDetailGUID).Take(1).SingleOrDefault();
                        MaterialStagingPullDetailDTO objItemDetail = objLocationDALMS.GetMsPullDetailsByItemGUID(item.ItemGUID.Value, RoomID, CompanyID, string.Empty, string.Empty, string.Empty, item.Received, item.BinID, Convert.ToString(item.MaterialStagingPullDetailGUID), false).Take(1).FirstOrDefault();

                        if (objItemDetail != null)
                        {
                            MSDGUID = objItemDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty);

                            if (objDTO.PullCredit.ToLower() == "ms pull")
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                            else
                            {
                                objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                                objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                                objItemDetail.Updated = DateTime.Now;
                                objLocationDALMS.Edit(objItemDetail);
                            }
                        }
                    }

                    //item locations qty update - TO DO MaterialStagingDetail
                    if (MSDGUID != Guid.Empty)
                    {
                        //MaterialStagingDetailDTO objmsd = objMaterialStagingDetailDAL.GetRecord(MSDGUID, RoomID, CompanyID);
                        MaterialStagingDetailDTO objmsd = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(MSDGUID, RoomID, CompanyID);

                        if (objmsd != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "ms pull")
                            {
                                objmsd.Quantity = objmsd.Quantity + (item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0));
                                objMaterialStagingDetailDAL.Edit(objmsd);
                            }
                            else
                            {
                                objmsd.Quantity = objmsd.Quantity - (item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0));
                                objMaterialStagingDetailDAL.Edit(objmsd);
                            }
                        }
                    }

                    //Project Spend update
                    if (item.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
                        ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                        ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
                        ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objDTO.ItemGUID && x.Created.Value <= item.Created.Value).SingleOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItemCompareDate(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, tmpsupplierIds, Convert.ToString(objDTO.ItemGUID), item.Created.Value.ToString("yyyy-MM-dd HH:mm:ss")).SingleOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);

                        if (objPrjMstDTO != null)
                        {
                            if (objDTO.PullCredit.ToLower() == "ms pull")
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }
                            else
                            {
                                objPrjMstDTO.DollarUsedAmount = objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                            }

                            objPrjMsgDAL.Edit(objPrjMstDTO);

                            if (objPrjSpenItmDTO != null)
                            {
                                //Update Quanitty used limit
                                if (objDTO.PullCredit.ToLower() == "ms pull")
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - item.PoolQuantity.GetValueOrDefault(0);
                                }
                                else
                                {
                                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)item.PoolQuantity.GetValueOrDefault(0) * Convert.ToDecimal(item.ItemCost.GetValueOrDefault(0)));
                                    objPrjSpenItmDTO.QuantityUsed = objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) + item.PoolQuantity.GetValueOrDefault(0);
                                }
                                objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                            }
                        }
                    }
                    #endregion
                    //Update started quantity...
                    objLocationDALMS.UpdateStagedQuantity(ItemDTO.GUID, RoomID, CompanyID, SessionUserId, EnterpriseId);
                }
            }
            return true;
        }

        protected PullMasterViewDTO AddtoPullDetail(ItemLocationDetailsDTO itemlocationdetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, Double? ItemSellPrice)
        {
            PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
            PullDetailsDTO objDTO = new PullDetailsDTO();
            objDTO.PULLGUID = PullGUID;
            objDTO.ItemGUID = itemlocationdetail.ItemGUID;

            if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : ProjectSpendGUID;
            }

            if (itemlocationdetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (itemlocationdetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (itemlocationdetail.SerialNumber != null)
                objDTO.SerialNumber = itemlocationdetail.SerialNumber;

            if (itemlocationdetail.LotNumber != null)
                objDTO.LotNumber = itemlocationdetail.LotNumber;

            if (itemlocationdetail.Expiration != null)
                objDTO.Expiration = itemlocationdetail.Expiration;

            objDTO.Received = itemlocationdetail.Received;
            objDTO.BinID = itemlocationdetail.BinID;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = itemlocationdetail.Room;
            objDTO.CompanyID = itemlocationdetail.CompanyID;
            objDTO.ItemLocationDetailGUID = itemlocationdetail.GUID;
            objDTO.ItemCost = ItemCost;
            objDTO.ItemPrice = ItemSellPrice;
            objDTO.PullCredit = itemlocationdetail.PullCredit;

            objDTO.ID = objDAL.Insert(objDTO);

            //Edit pull master
            PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty));

            if (objPullDTO != null)
            {
                objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemCost.GetValueOrDefault(0));
                objPullDTO.PullPrice = (objPullDTO.PullPrice.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemSellPrice.GetValueOrDefault(0));
                objPullDTO.WhatWhereAction = itemlocationdetail.PullCredit;
                objPullMAster.Edit(objPullDTO);
            }

            return objPullDTO;
        }

        protected PullMasterViewDTO AddtoPullDetail(MaterialStagingPullDetailDTO itemlocationdetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, Double? ItemSellPrice)
        {
            PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
            PullDetailsDTO objDTO = new PullDetailsDTO();
            objDTO.PULLGUID = PullGUID;
            objDTO.ItemGUID = itemlocationdetail.ItemGUID;

            if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
            }

            if (itemlocationdetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (itemlocationdetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (itemlocationdetail.SerialNumber != null)
                objDTO.SerialNumber = itemlocationdetail.SerialNumber;

            if (itemlocationdetail.LotNumber != null)
                objDTO.LotNumber = itemlocationdetail.LotNumber;

            if (itemlocationdetail.Expiration != null)
                objDTO.Expiration = itemlocationdetail.Expiration;

            objDTO.Received = itemlocationdetail.Received;
            objDTO.BinID = itemlocationdetail.StagingBinId;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = itemlocationdetail.Room;
            objDTO.CompanyID = itemlocationdetail.CompanyID;
            objDTO.MaterialStagingPullDetailGUID = itemlocationdetail.GUID;
            objDTO.ItemPrice = ItemSellPrice;
            objDTO.ItemCost = ItemCost;
            objDTO.PullCredit = itemlocationdetail.PullCredit;
            objDAL.Insert(objDTO);

            //Edit pull master
            PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty));

            if (objPullDTO != null)
            {
                objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * (double)ItemCost.GetValueOrDefault(0));
                objPullDTO.PullPrice = (objPullDTO.PullPrice.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * (double)ItemSellPrice.GetValueOrDefault(0));
                objPullDTO.WhatWhereAction = itemlocationdetail.PullCredit;
                objPullMAster.Edit(objPullDTO);
            }

            return objPullDTO;
        }

        private bool BinWiseQuantityCheck(ItemLocationQTYDTO lstLocDTO, ItemLocationQTYDAL objLocQTY, int IsCreditPullNothing, Int64 RoomID, long CompanyID, out string ItemLocationMSG,
                                            BinMasterDTO objBINDTO, PullMasterViewDTO objDTO, BinMasterDAL objBINDAL, ItemMasterDTO ItemDTO, long EnterpriseId, string CultureCode)
        {
            #region "Bin Wise Quantity Check"
            // if selected location has not enough quantity then send message  and don't save data                
            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);


            string LocationName = string.Empty; ;
            if (lstLocDTO != null)
            {
                LocationName = lstLocDTO.BinNumber;
            }

            if (objDTO.ID == 0)
                objDTO.TempPullQTY = objDTO.PoolQuantity.GetValueOrDefault(0);

            if (objDTO.TempPullQTY == 0)
            {
                string msgEnterProperValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterProperValue", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                ItemLocationMSG = msgEnterProperValue;
                return true;
            }

            if (IsCreditPullNothing != 1) // No credit
            {
                if (lstLocDTO != null)
                {
                    double AvailableQuantity = 0;
                    if (ItemDTO.Consignment)
                    {
                        string msgNotEnoughCustAndConsQtyForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughCustAndConsQtyForLocation", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                        AvailableQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                        ItemLocationMSG = string.Format(msgNotEnoughCustAndConsQtyForLocation, LocationName, lstLocDTO.Quantity.ToString());
                    }
                    else
                    {
                        string msgNotEnoughCustomerQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughCustomerQuantityForLocation", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                        AvailableQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                        ItemLocationMSG = string.Format(msgNotEnoughCustomerQuantityForLocation, LocationName, lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0));
                    }

                    if (objDTO.TempPullQTY > AvailableQuantity)
                    {
                        //ItemLocationMSG = "Not Enough Quantity for Location ## " + LocationName + "(Avl.QTY=" + lstLocDTO.Quantity.ToString() + ")";
                        return true;
                    }
                    else
                    {
                        ItemLocationMSG = "";
                    }
                }
                else
                {
                    string msgNotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                    ItemLocationMSG = string.Format(msgNotEnoughQuantityForLocation, LocationName, "0");
                    return true;
                }
            }
            #endregion
            ItemLocationMSG = "";
            return false;
        }

        //public bool ProjectWiseQuantityCheck(ProjectMasterDAL objPrjMsgDAL, ProjectSpendItemsDTO objPrjSpenItmDTO, ProjectMasterDTO objPrjMstDTO, ProjectSpendItemsDAL objPrjSpenItmDAL, int IsCreditPullNothing, long RoomID, long CompanyID, out string ItemLocationMSG, BinMasterDTO objBINDTO, PullMasterViewDTO objDTO, BinMasterDAL objBINDAL, ItemMasterDTO ItemDTO, bool IsProjectSpendAllowed, out bool IsPSLimitExceed, List<ItemLocationDetailsDTO> ObjItemLocation)
        public bool ProjectWiseQuantityCheck(ProjectSpendItemsDTO objPrjSpenItmDTO, ProjectMasterDTO objPrjMstDTO, out string ItemLocationMSG, PullMasterViewDTO objDTO,
            ItemMasterDTO ItemDTO, bool IsProjectSpendAllowed, out bool IsPSLimitExceed, List<ItemLocationDetailsDTO> ObjItemLocation, long EnterpriseId, string CultureCode
            , long CompanyId, long RoomId)
        {
            double pickPrice = 0;

            #region "Project Wise Quantity Check"
            var projectMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResProjectMaster", CultureCode, EnterpriseId, CompanyId);


            if (IsProjectSpendAllowed == false)
            {
                if (objPrjMstDTO != null)
                {
                    if (ItemDTO.SerialNumberTracking)
                    {
                        foreach (var iLocDetail in ObjItemLocation)
                        {
                            if (ItemDTO.Consignment)
                            {
                                pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) * (iLocDetail.Cost ?? 0)) + (iLocDetail.ConsignedQuantity.GetValueOrDefault(0) * (iLocDetail.Cost ?? 0));
                            }
                            else
                            {
                                pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) * (iLocDetail.Cost ?? 0));
                            }
                        }
                    }
                    else
                    {
                        double localPickQTY = 0;
                        foreach (var iLocDetail in ObjItemLocation)
                        {
                            //localPickQTY 
                            if (ItemDTO.Consignment)
                            {
                                if (objDTO.TempPullQTY.GetValueOrDefault(0) != localPickQTY)
                                {
                                    if ((iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) >= (objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY))
                                    {
                                        pickPrice += ((objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY) * (iLocDetail.Cost ?? 0));
                                        localPickQTY += objDTO.TempPullQTY.GetValueOrDefault(0);
                                    }
                                    else if ((iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) < (objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY))
                                    {
                                        localPickQTY += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0));
                                        pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) * (iLocDetail.Cost ?? 0);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (objDTO.TempPullQTY.GetValueOrDefault(0) != localPickQTY)
                                {
                                    if (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY))
                                    {
                                        localPickQTY += objDTO.TempPullQTY.GetValueOrDefault(0);
                                        pickPrice += (objDTO.TempPullQTY.GetValueOrDefault(0) * (iLocDetail.Cost ?? 0));
                                    }
                                    else if (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) < (objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY))
                                    {
                                        localPickQTY += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - localPickQTY);
                                        pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) * (iLocDetail.Cost ?? 0));
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (objPrjMstDTO.DollarLimitAmount.GetValueOrDefault(0) > 0 && objPrjMstDTO.DollarLimitAmount.GetValueOrDefault(0) < (objPrjMstDTO.DollarUsedAmount.GetValueOrDefault(0) + (decimal)pickPrice))
                    {
                        string msgProjectSpendDollarAmountLimitExceed = ResourceRead.GetResourceValueByKeyAndFullFilePath("ProjectSpendDollarAmountLimitExceed", projectMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResProjectMaster", CultureCode);
                        ItemLocationMSG = msgProjectSpendDollarAmountLimitExceed;
                        IsPSLimitExceed = true;
                        return true;
                    }
                }

                if (objPrjSpenItmDTO != null)
                {
                    if (objPrjSpenItmDTO.QuantityLimit.GetValueOrDefault(0) > 0 && (objDTO.TempPullQTY.GetValueOrDefault(0) + objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0)) > objPrjSpenItmDTO.QuantityLimit.GetValueOrDefault(0))
                    {
                        string msgProjectSpendItemQuantityLimitExceed = ResourceRead.GetResourceValueByKeyAndFullFilePath("ProjectSpendItemQuantityLimitExceed", projectMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResProjectMaster", CultureCode);
                        ItemLocationMSG = msgProjectSpendItemQuantityLimitExceed;
                        IsPSLimitExceed = true;
                        return true;
                    }

                    if (objPrjSpenItmDTO.DollarLimitAmount.GetValueOrDefault(0) > 0 && ((decimal)pickPrice + objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0)) > objPrjSpenItmDTO.DollarLimitAmount.GetValueOrDefault(0))
                    {
                        string msgProjectSpendItemDollarLimitExceed = ResourceRead.GetResourceValueByKeyAndFullFilePath("ProjectSpendItemDollarLimitExceed", projectMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResProjectMaster", CultureCode);
                        ItemLocationMSG = msgProjectSpendItemDollarLimitExceed;
                        IsPSLimitExceed = true;
                        return true;
                    }
                }
            }
            #endregion
            ItemLocationMSG = "";
            IsPSLimitExceed = false;
            return false;
        }

        private PullMasterViewDTO PullInsertUpdate(PullMasterViewDTO objDTO, PullMasterViewDTO obj, Int64 TempOldBinID, PullMasterDAL objPullDal, long SessionUserId)
        {
            #region "Pull Insert Update"

            if (objDTO.ID > 0)
            {
                #region "Pull Update"

                objDTO.ID = 0;
                objDTO.PoolQuantity = objDTO.TempPullQTY;
                obj.ID = objDTO.ID; obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
                obj.SupplierAccountGuid = objDTO.SupplierAccountGuid.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.SupplierAccountGuid;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.BinID = objDTO.BinID; obj.Updated = objDTO.Updated; obj.LastUpdatedBy = objDTO.LastUpdatedBy;

                if (objDTO.GUID != Guid.Empty)
                {
                    obj.GUID = objDTO.GUID;
                }
                else
                {
                    obj.GUID = Guid.NewGuid();
                    objDTO.GUID = obj.GUID;
                }

                obj.ItemGUID = objDTO.ItemGUID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow; obj.CreatedBy = objDTO.CreatedBy; obj.LastUpdatedBy = objDTO.LastUpdatedBy; obj.IsDeleted = false; obj.IsArchived = false; obj.CompanyID = objDTO.CompanyID; obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.ActionType;
                obj.ActionType = objDTO.ActionType;
                obj.UDF1 = objDTO.UDF1; obj.UDF2 = objDTO.UDF2; obj.UDF3 = objDTO.UDF3; obj.UDF4 = objDTO.UDF4; obj.UDF5 = objDTO.UDF5;
                obj.RequisitionDetailGUID = objDTO.RequisitionDetailGUID;
                obj.WorkOrderDetailGUID = obj.WorkOrderDetailGUID;
                obj.CountLineItemGuid = obj.CountLineItemGuid;
                obj.SupplierID = objDTO.SupplierID;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.EditedFrom ?? "Web";
                obj.EditedFrom = objDTO.EditedFrom ?? "Web";

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom;

                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.WhatWhereAction = objDTO.PullCredit;

                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.PullType = (objDTO.PullType > 0 ? objDTO.PullType : 1);
                obj.ID = objPullDal.Insert(obj);

                #endregion
            }
            else
            {
                #region "Pull Insert"

                obj.ID = objDTO.ID; obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.BinID = objDTO.BinID; obj.Updated = objDTO.Updated; obj.LastUpdatedBy = objDTO.LastUpdatedBy;

                if (objDTO.GUID != Guid.Empty)
                {
                    obj.GUID = objDTO.GUID;
                }
                else
                {
                    obj.GUID = Guid.NewGuid();
                    objDTO.GUID = obj.GUID;
                }

                obj.ItemGUID = objDTO.ItemGUID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow; obj.CreatedBy = objDTO.CreatedBy; obj.LastUpdatedBy = objDTO.LastUpdatedBy; obj.IsDeleted = false; obj.IsArchived = false; obj.CompanyID = objDTO.CompanyID; obj.Room = objDTO.Room;
                obj.PullCredit = objDTO.PullCredit;
                obj.ActionType = objDTO.PullCredit;
                obj.UDF1 = objDTO.UDF1; obj.UDF2 = objDTO.UDF2; obj.UDF3 = objDTO.UDF3; obj.UDF4 = objDTO.UDF4; obj.UDF5 = objDTO.UDF5;
                obj.RequisitionDetailGUID = objDTO.RequisitionDetailGUID;
                obj.WorkOrderDetailGUID = objDTO.WorkOrderDetailGUID;
                obj.CountLineItemGuid = objDTO.CountLineItemGuid;
                obj.SupplierID = objDTO.SupplierID;
                obj.PullOrderNumber = objDTO.PullOrderNumber;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = "Web";
                obj.EditedFrom = "Web";

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom;

                obj.PoolQuantity = objDTO.PoolQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.WhatWhereAction = objDTO.PullCredit;
                obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
                obj.PullType = (objDTO.PullType > 0 ? objDTO.PullType : 1);
                objDTO.ID = objPullDal.Insert(obj);

                #endregion
            }

            if (objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(base.DataBaseName);
                RequisitionDetailsDTO objReqDTO = objReqDDAL.GetRequisitionDetailsByGUIDPlain(objDTO.RequisitionDetailGUID.Value);
                objReqDTO.PullUDF1 = objDTO.UDF1;
                objReqDTO.PullUDF2 = objDTO.UDF2;
                objReqDTO.PullUDF3 = objDTO.UDF3;
                objReqDTO.PullUDF4 = objDTO.UDF4;
                objReqDTO.PullUDF5 = objDTO.UDF5;
                objReqDTO.QuantityPulled = objReqDTO.QuantityPulled.GetValueOrDefault(0) + objDTO.PoolQuantity;
                objReqDTO.EditedFrom = objDTO.EditedFrom ?? "Web";
                objReqDDAL.Edit(objReqDTO, SessionUserId);
            }

            PullMasterViewDTO ReturnDTO = GetPullByGuidPlain(obj.GUID);
            return ReturnDTO;
            #endregion
        }

        public void UpdateTurnsAverageUsage(PullMasterViewDTO objDTO, long SessionUserId)
        {
            try
            {
                DashboardDAL objDashbordDal = new DashboardDAL(base.DataBaseName);
                DashboardAnalysisInfo objDashbordTurns = objDashbordDal.UpdateTurnsByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy ?? 0, null, null);
                DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy ?? 0, SessionUserId, null, null);
            }
            catch (Exception)
            {

            }
        }

        public List<PullMasterViewDTO> GetPagedPullRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool UserConsignmentAllowed, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<PullMasterViewDTO> lstPulls = new List<PullMasterViewDTO>();
            string ActionTypes = null;
            string ItemSuppliers = null;
            string ItemManufacturers = null;
            string ItemCategories = null;
            string ItemCost = null;
            string PullCreaters = null;
            string PullUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string WorkOrders = null;
            string ProjectSpends = null;
            string Requistions = null;
            bool? Billing = null;
            bool? IsEDISent = null;
            string OrderNumbers = null;
            bool? isConsignment = null;
            TotalCount = 0;

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    PullCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    PullUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    ItemManufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    ItemCost = FieldsPara[15].Split('_')[0];
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategories = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[58]))
                {
                    string[] arrReplenishTypes = FieldsPara[58].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ProjectSpends = ProjectSpends + supitem + "','";
                    }
                    ProjectSpends = ProjectSpends.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[59]))
                {
                    string[] arrReplenishTypes = FieldsPara[59].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        WorkOrders = WorkOrders + supitem + "','";
                    }
                    WorkOrders = WorkOrders.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[60]))
                {
                    string[] arrReplenishTypes = FieldsPara[60].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        Requistions = Requistions + supitem + "','";
                    }
                    Requistions = Requistions.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[50]))
                {
                    string[] arrReplenishTypes = FieldsPara[50].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ActionTypes = ActionTypes + supitem + "','";
                    }
                    ActionTypes = ActionTypes.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[61]))
                {
                    Billing = Convert.ToBoolean(FieldsPara[61]);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[62]))
                {
                    IsEDISent = Convert.ToBoolean(FieldsPara[62]);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[78]))
                {
                    string[] arrReplenishTypes = FieldsPara[78].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        OrderNumbers = OrderNumbers + supitem + "','";
                    }
                    OrderNumbers = OrderNumbers.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[105]))
                {
                    if (FieldsPara[105] == "0")
                    {
                        isConsignment = false;
                    }
                    else if (FieldsPara[105] == "1")
                    {
                        isConsignment = true;
                    }
                    else
                    {
                    }
                }
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@startrowindex",StartRowIndex)
                                                    , new SqlParameter("@maxrows",MaxRows)
                                                    , new SqlParameter("@searchterm",SearchTerm ?? (object)DBNull.Value)
                                                    , new SqlParameter("@sortcolumnname",sortColumnName)
                                                    , new SqlParameter("@actiontypes",ActionTypes ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemsuppliers",ItemSuppliers ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemmanufacturers",ItemManufacturers ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemcost",ItemCost ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemcategories",ItemCategories ?? (object)DBNull.Value)
                                                    , new SqlParameter("@pullcreaters",PullCreaters ?? (object)DBNull.Value)
                                                    , new SqlParameter("@pullupdators",PullUpdators ?? (object)DBNull.Value)
                                                    , new SqlParameter("@createddatefrom",CreatedDateFrom ?? (object)DBNull.Value)
                                                    , new SqlParameter("@createddateto",CreatedDateTo ?? (object)DBNull.Value)
                                                    , new SqlParameter("@updateddatefrom",UpdatedDateFrom ?? (object)DBNull.Value)
                                                    , new SqlParameter("@updateddateto",UpdatedDateTo ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf1",UDF1 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf2",UDF2 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf3",UDF3 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf4",UDF4 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf5",UDF5 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@isdeleted",IsDeleted)
                                                    , new SqlParameter("@isarchived",IsArchived)
                                                    , new SqlParameter("@roomid",RoomID)
                                                    , new SqlParameter("@companyid",CompanyID)
                                                    , new SqlParameter("@SupplierIds",strSupplierIds  )
                                                    , new SqlParameter("@userconsignmentallowed",UserConsignmentAllowed  )
                                                    , new SqlParameter("@WorkOrders",WorkOrders ?? (object)DBNull.Value)
                                                    , new SqlParameter("@ProjectSpends",ProjectSpends ?? (object)DBNull.Value)
                                                    , new SqlParameter("@Requistions",Requistions ?? (object)DBNull.Value)
                                                    , new SqlParameter("@Billing",Billing ?? (object)DBNull.Value)
                                                    , new SqlParameter("@IsEDISent",IsEDISent ?? (object)DBNull.Value)
                                                    , new SqlParameter("@OrderNumbers",OrderNumbers ?? (object)DBNull.Value)
                                                    , new SqlParameter("@IsConsignment",isConsignment ?? (object)DBNull.Value)};


                lstPulls = context.Database.SqlQuery<PullMasterViewDTO>("EXEC dbo.GetPagedPullMasterRecords @startrowindex,@maxrows,@searchterm,@sortcolumnname,@actiontypes,@itemsuppliers,@itemmanufacturers,@itemcost,@itemcategories,@pullcreaters,@pullupdators,@createddatefrom,@createddateto,@updateddatefrom,@updateddateto,@udf1,@udf2,@udf3,@udf4,@udf5,@isdeleted,@isarchived,@roomid,@companyid,@SupplierIds,@userconsignmentallowed,@WorkOrders,@ProjectSpends,@Requistions,@Billing,@IsEDISent,@OrderNumbers,@IsConsignment", params1).ToList();
                if (lstPulls != null && lstPulls.Count() > 0)
                {
                    TotalCount = lstPulls.First().TotalCount ?? 0;
                }
            }

            return lstPulls;
        }
        public List<PullMasterListViewDTO> GetPagedPullRecordsView(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool UserConsignmentAllowed, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<PullMasterListViewDTO> lstPulls = new List<PullMasterListViewDTO>();
            string ActionTypes = null;
            string ItemSuppliers = null;
            string ItemManufacturers = null;
            string ItemCategories = null;
            string ItemCost = null;
            string PullCreaters = null;
            string PullUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string WorkOrders = null;
            string ProjectSpends = null;
            string Requistions = null;
            bool? Billing = null;
            bool? IsEDISent = null;
            string OrderNumbers = null;
            bool? isConsignment = null;
            string SupplierAccountNumberGuid = null;
            TotalCount = 0;
            string SpName = "dbo.GetPagedPullMasterRecords";

            if (IsArchived)
            {
                SpName = "dbo.GetPagedPullMasterRecords_Archive";
            }

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    PullCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    PullUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + HttpUtility.UrlDecode(supitem) + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    ItemManufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    //ItemCost = FieldsPara[15].Split('_')[0];
                    ItemCost = FieldsPara[15];
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategories = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[58]))
                {
                    string[] arrReplenishTypes = FieldsPara[58].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ProjectSpends = ProjectSpends + supitem + "','";
                    }
                    ProjectSpends = ProjectSpends.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[59]))
                {
                    string[] arrReplenishTypes = FieldsPara[59].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        WorkOrders = WorkOrders + supitem + "','";
                    }
                    WorkOrders = WorkOrders.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[60]))
                {
                    string[] arrReplenishTypes = FieldsPara[60].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        Requistions = Requistions + supitem + "','";
                    }
                    Requistions = Requistions.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[50]))
                {
                    string[] arrReplenishTypes = FieldsPara[50].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ActionTypes = ActionTypes + supitem + "','";
                    }
                    ActionTypes = ActionTypes.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[61]))
                {
                    Billing = Convert.ToBoolean(FieldsPara[61]);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[62]))
                {
                    IsEDISent = Convert.ToBoolean(FieldsPara[62]);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[78]))
                {
                    string[] arrReplenishTypes = FieldsPara[78].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        OrderNumbers = OrderNumbers + supitem + "','";
                    }
                    OrderNumbers = OrderNumbers.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[105]))
                {
                    if (FieldsPara[105] == "0")
                    {
                        isConsignment = false;
                    }
                    else if (FieldsPara[105] == "1")
                    {
                        isConsignment = true;
                    }
                    else
                    {
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[119]))
                {
                    string[] arrSupplierAccountNumbers = FieldsPara[119].Split(',');
                    foreach (string supitem in arrSupplierAccountNumbers)
                    {
                        SupplierAccountNumberGuid = SupplierAccountNumberGuid + supitem + "','";
                    }
                    SupplierAccountNumberGuid = SupplierAccountNumberGuid.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@startrowindex",StartRowIndex)
                                                    , new SqlParameter("@maxrows",MaxRows)
                                                    , new SqlParameter("@searchterm",SearchTerm ?? (object)DBNull.Value)
                                                    , new SqlParameter("@sortcolumnname",sortColumnName)
                                                    , new SqlParameter("@actiontypes",ActionTypes ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemsuppliers",ItemSuppliers ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemmanufacturers",ItemManufacturers ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemcost",ItemCost ?? (object)DBNull.Value)
                                                    , new SqlParameter("@itemcategories",ItemCategories ?? (object)DBNull.Value)
                                                    , new SqlParameter("@pullcreaters",PullCreaters ?? (object)DBNull.Value)
                                                    , new SqlParameter("@pullupdators",PullUpdators ?? (object)DBNull.Value)
                                                    , new SqlParameter("@createddatefrom",CreatedDateFrom ?? (object)DBNull.Value)
                                                    , new SqlParameter("@createddateto",CreatedDateTo ?? (object)DBNull.Value)
                                                    , new SqlParameter("@updateddatefrom",UpdatedDateFrom ?? (object)DBNull.Value)
                                                    , new SqlParameter("@updateddateto",UpdatedDateTo ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf1",UDF1 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf2",UDF2 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf3",UDF3 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf4",UDF4 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@udf5",UDF5 ?? (object)DBNull.Value)
                                                    , new SqlParameter("@isdeleted",IsDeleted)
                                                    , new SqlParameter("@isarchived",IsArchived)
                                                    , new SqlParameter("@roomid",RoomID)
                                                    , new SqlParameter("@companyid",CompanyID)
                                                    , new SqlParameter("@SupplierIds",strSupplierIds)
                                                    , new SqlParameter("@userconsignmentallowed",UserConsignmentAllowed  )
                                                    , new SqlParameter("@WorkOrders",WorkOrders ?? (object)DBNull.Value)
                                                    , new SqlParameter("@ProjectSpends",ProjectSpends ?? (object)DBNull.Value)
                                                    , new SqlParameter("@Requistions",Requistions ?? (object)DBNull.Value)
                                                    , new SqlParameter("@Billing",Billing ?? (object)DBNull.Value)
                                                    , new SqlParameter("@IsEDISent",IsEDISent ?? (object)DBNull.Value)
                                                    , new SqlParameter("@OrderNumbers",OrderNumbers ?? (object)DBNull.Value)
                                                    , new SqlParameter("@IsConsignment",isConsignment ?? (object)DBNull.Value)
                                                    , new SqlParameter("@SupplierAccountNumberGuid",SupplierAccountNumberGuid ?? (object)DBNull.Value)};

                context.Database.CommandTimeout = 3600;
                lstPulls = context.Database.SqlQuery<PullMasterListViewDTO>("EXEC " + SpName + " @startrowindex,@maxrows,@searchterm,@sortcolumnname,@actiontypes,@itemsuppliers,@itemmanufacturers,@itemcost,@itemcategories,@pullcreaters,@pullupdators,@createddatefrom,@createddateto,@updateddatefrom,@updateddateto,@udf1,@udf2,@udf3,@udf4,@udf5,@isdeleted,@isarchived,@roomid,@companyid,@SupplierIds,@userconsignmentallowed,@WorkOrders,@ProjectSpends,@Requistions,@Billing,@IsEDISent,@OrderNumbers,@IsConsignment,@SupplierAccountNumberGuid", params1).ToList();
                if (lstPulls != null && lstPulls.Count() > 0)
                {
                    TotalCount = lstPulls.First().TotalCount ?? 0;
                }
            }

            return lstPulls;
        }

        public List<PullMasterViewDTO> ProcessPullForBilling(long SupplierID, long RoomID, long CompanyID, DateTime FromTime, DateTime ToTime, long UserID, string WhatWhereAction)
        {
            List<PullMasterViewDTO> lstPulls = new List<PullMasterViewDTO>();            
            string Guids = string.Empty;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@SupplierID", SupplierID), 
                                                    new SqlParameter("@RoomID", RoomID), 
                                                    new SqlParameter("@CompanyID", CompanyID), 
                                                    new SqlParameter("@FromTime", FromTime),
                                                    new SqlParameter("@ToTime", ToTime),
                                                    new SqlParameter("@UserID", UserID),
                                                    new SqlParameter("@WhatWhereAction", WhatWhereAction)
                                                };
                List<PullMasterDTO> lstPullMasterDTO = context.Database.SqlQuery<PullMasterDTO>("exec [ProcessPullForBilling] @SupplierID,@RoomID,@CompanyID,@FromTime,@ToTime,@UserID,@WhatWhereAction", params1).ToList();

                if (lstPullMasterDTO!=null && lstPullMasterDTO.Count()>0)
                {
                    Guids = string.Join(",", lstPullMasterDTO.Select(x => x.GUID));
                    lstPulls = GetPullsByGuidsNormal(Guids);
                }

                                }

            return lstPulls;
        }
        public List<PullMasterViewDTO> GetPullsByGuidsNormal(string Guids)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@PullGuids", Guids ?? (object)DBNull.Value) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullsByGuidsNormal] @PullGuids", params1).ToList();
            }
        }

        public bool UpdateBillingAndPullOrderNumber(PullMasterViewDTO oPullMaster)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullMaster obj = context.PullMasters.FirstOrDefault(t => t.ID == oPullMaster.ID);

                if (obj != null)
                {
                    obj.Billing = oPullMaster.Billing;
                    obj.PullOrderNumber = oPullMaster.PullOrderNumber;
                    obj.ReceivedOn = oPullMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = oPullMaster.LastUpdatedBy;
                    if (oPullMaster.UDF1 != null)
                        obj.UDF1 = oPullMaster.UDF1;
                    if (oPullMaster.UDF2 != null)
                        obj.UDF2 = oPullMaster.UDF2;
                    if (oPullMaster.UDF3 != null)
                        obj.UDF3 = oPullMaster.UDF3;
                    if (oPullMaster.UDF4 != null)
                        obj.UDF4 = oPullMaster.UDF4;
                    if (oPullMaster.UDF5 != null)
                        obj.UDF5 = oPullMaster.UDF5;
                    obj.EditedFrom = oPullMaster.EditedFrom = "Web";
                }

                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Get Paged Records from the PullMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<PullMasterViewDTO> GetAllWorkOrderRecords(long RoomID, long CompanyId, string WorkOrderGUID)
        {
            List<PullMasterViewDTO> obj = new List<PullMasterViewDTO>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetWorkOrderRecords", RoomID, CompanyId, WorkOrderGUID);
            DataTable dtCart = dsCart.Tables[0];

            if (dtCart.Rows.Count > 0)
            {
                obj = dtCart.AsEnumerable()
                  .Select(row => new PullMasterViewDTO
                  {
                      ID = row.Field<long>("ID"),
                      ProjectSpendGUID = row.Field<Guid?>("ProjectSpendGUID"),
                      ProjectName = row.Field<string>("ProjectSpendName"),
                      ProjectSpendName = row.Field<string>("ProjectSpendName"),
                      BinNumber = ((row.Field<string>("BinNumber") ?? string.Empty).Equals("[|EmptyStagingBin|]") ? string.Empty : row.Field<string>("BinNumber")),
                      DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity").GetValueOrDefault(0),
                      OnHandQuantity = row.Field<double?>("OnHandQuantity").GetValueOrDefault(0),
                      ItemGUID = row.Field<Guid?>("ItemGUID"),
                      ItemNumber = row.Field<string>("ItemNumber"),
                      CustomerOwnedQuantity = row.Field<double?>("CustomerOwnedQuantity").GetValueOrDefault(0),
                      ConsignedQuantity = row.Field<double?>("ConsignedQuantity").GetValueOrDefault(0),
                      CreditConsignedQuantity = row.Field<double?>("CreditConsignedQuantity").GetValueOrDefault(0),
                      CreditCustomerOwnedQuantity = row.Field<double?>("CreditCustomerOwnedQuantity").GetValueOrDefault(0),
                      PoolQuantity = row.Field<double?>("PoolQuantity").GetValueOrDefault(0),
                      PullOrderNumber = row.Field<string>("PullOrderNumber"),
                      PullCost = row.Field<double?>("PullCost").GetValueOrDefault(0),
                      SerialNumber = string.Empty,
                      LotNumber = string.Empty,
                      DateCode = string.Empty,
                      BinID = row.Field<long?>("BinID").GetValueOrDefault(0),
                      UDF1 = row.Field<string>("UDF1"),
                      UDF2 = row.Field<string>("UDF2"),
                      UDF3 = row.Field<string>("UDF3"),
                      UDF4 = row.Field<string>("UDF4"),
                      UDF5 = row.Field<string>("UDF5"),
                      ItemUDF1 = row.Field<string>("ITEMUDF1"),
                      ItemUDF2 = row.Field<string>("ITEMUDF2"),
                      ItemUDF3 = row.Field<string>("ITEMUDF3"),
                      ItemUDF4 = row.Field<string>("ITEMUDF4"),
                      ItemUDF5 = row.Field<string>("ITEMUDF5"),
                      ItemUDF6 = row.Field<string>("ItemUDF6"),
                      ItemUDF7 = row.Field<string>("ItemUDF7"),
                      ItemUDF8 = row.Field<string>("ItemUDF8"),
                      ItemUDF9 = row.Field<string>("ItemUDF9"),
                      ItemUDF10 = row.Field<string>("ItemUDF10"),
                      GUID = row.Field<Guid>("GUID"),
                      Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                      Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                      CreatedBy = row.Field<long?>("CreatedBy").GetValueOrDefault(0),
                      LastUpdatedBy = row.Field<long?>("LastUpdatedBy").GetValueOrDefault(0),
                      IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                      IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                      CompanyID = row.Field<long?>("CompanyID").GetValueOrDefault(0),
                      Room = row.Field<long?>("Room").GetValueOrDefault(0),
                      CreatedByName = row.Field<string>("CreatedByName"),
                      UpdatedByName = row.Field<string>("UpdatedByName"),
                      RoomName = row.Field<string>("RoomName"),
                      SupplierID = row.Field<long?>("SupplierID").GetValueOrDefault(0),
                      ManufacturerID = row.Field<long?>("ManufacturerID").GetValueOrDefault(0),
                      CategoryID = row.Field<long?>("CategoryID").GetValueOrDefault(0),
                      CategoryName = row.Field<string>("CategoryName"),
                      Markup = row.Field<double?>("Markup").GetValueOrDefault(0),
                      SellPrice = row.Field<double?>("SellPrice").GetValueOrDefault(0),
                      ItemCost = row.Field<double?>("ItemCost").GetValueOrDefault(0),// row.Field<bool>("Trend"),
                      Description = (row.Field<string>("Description")),
                      Unit = (row.Field<string>("Unit")),
                      PullCredit = row.Field<string>("PullCredit"),
                      ActionType = row.Field<string>("ActionType"),
                      ItemType = row.Field<int?>("ItemType"),
                      RequisitionDetailGUID = row.Field<Guid?>("RequisitionDetailGUID").GetValueOrDefault(Guid.Empty),
                      WorkOrderDetailGUID = row.Field<Guid?>("WorkOrderDetailGUID").GetValueOrDefault(Guid.Empty),
                      Billing = row.Field<bool?>("Billing").GetValueOrDefault(false),
                      CountLineItemGuid = row.Field<Guid?>("CountLineItemGuid").GetValueOrDefault(Guid.Empty),
                      PackingQuantity = row.Field<double?>("PackingQuantity").GetValueOrDefault(0),
                      Manufacturer = row.Field<string>("Manufacturer"),
                      ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                      SupplierName = row.Field<string>("SupplierName"),
                      SupplierPartNo = row.Field<string>("SupplierPartNo"),
                      AverageUsage = row.Field<double?>("AverageUsage").GetValueOrDefault(0),
                      CriticalQuantity = row.Field<double>("CriticalQuantity"),
                      GLAccount = row.Field<string>("GLAccount"),
                      InTransitquantity = row.Field<double?>("InTransitquantity").GetValueOrDefault(0),
                      IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool>("IsItemLevelMinMaxQtyRequired") : false), //row.Field<bool>("IsPurchase"),
                      MaximumQuantity = (row.Field<double>("MaximumQuantity")), //row.Field<bool>("IsTransfer"),
                      MinimumQuantity = row.Field<double>("MinimumQuantity"),
                      Turns = row.Field<double?>("Turns").GetValueOrDefault(0),
                      Taxable = row.Field<bool>("Taxable"),
                      OnOrderQuantity = (row.Field<double?>("OnOrderQuantity")).GetValueOrDefault(0),
                      OnTransferQuantity = (row.Field<double?>("OnTransferQuantity")).GetValueOrDefault(0),
                      Consignment = (row.Field<bool?>("Consignment").HasValue ? row.Field<bool>("Consignment") : false),
                      ItemOnhandQty = row.Field<double?>("ItemOnhandQty").GetValueOrDefault(0),
                      IsAddedFromPDA = row.Field<bool>("IsAddedFromPDA"),
                      IsProcessedAfterSync = row.Field<bool>("IsProcessedAfterSync"),
                      //for item grid display purpose - CART, PUll  
                      ItemStageQty = row.Field<double?>("ItemStageQty").GetValueOrDefault(0),
                      ItemStageLocationQty = row.Field<double?>("ItemStageLocationQty").GetValueOrDefault(0),
                      ItemLocationOnHandQty = row.Field<double?>("ItemLocationOnHandQty").GetValueOrDefault(0),
                      ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                      ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                      AddedFrom = row.Field<string>("AddedFrom"),
                      EditedFrom = row.Field<string>("EditedFrom"),
                      ExtendedCost = row.Field<double?>("ExtendedCost").GetValueOrDefault(0),
                      AverageCost = (row.Field<double?>("AverageCost")).GetValueOrDefault(0),
                      CostUOMName = (row.Field<string>("CostUOMName")),
                      ToolName = (row.Field<string>("ToolName")),
                      ToolGUID = (row.Field<Guid?>("ToolGUID")),
                      PullPrice = row.Field<double?>("PullPrice").GetValueOrDefault(0),
                      PullItemCost = row.Field<double?>("PullItemCost").GetValueOrDefault(0),
                      PullItemSellPrice = row.Field<double?>("PullItemSellPrice").GetValueOrDefault(0),
                      PullMarkup = row.Field<double?>("PullMarkup").GetValueOrDefault(0),
                      ItemCostOnPullDate = row.Field<double?>("ItemCostOnPullDate").GetValueOrDefault(0),
                      SupplierAccountGuid = row.Field<Guid?>("SupplierAccountGuid")

                  }).ToList();
            }
            return obj;
        }

        public List<DTOForAutoComplete> GetPullOrderNumberForNewPullGrid(string NameStartWith, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@NameStartWith", NameStartWith) };
                return context.Database.SqlQuery<DTOForAutoComplete>("exec [GetPullOrderNumbers] @RoomID,@CompanyID,@NameStartWith", params1).ToList();
            }
        }

        #region [For Service]

        public PullMasterViewDTO UpdateCumulativeOnHand(PullMasterViewDTO objDTO)
        {
            PullMaster ObjPull = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool UpdateBillingFlag = false;
                string[] arrpullnames = new string[] { "p_evmi", "pull", "ms pull" };
                string[] arrCreditNames = new string[] { "cr_evmi", "credit", "ms credit", "credit:evmi" };
                string[] AllPullTypes = new string[] { "p_evmi", "pull", "ms pull", "cr_evmi", "credit", "ms credit", "credit:evmi" };
                ObjPull = context.PullMasters.FirstOrDefault(t => t.GUID == objDTO.GUID);
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objDTO.ItemGUID);

                if (objItem != null && ObjPull != null)
                {
                    if (objItem.Consignment)
                    {
                        SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                        PullSchedulerDTO objpullSchedule = objSupplierMasterDAL.GetRoomScheduleForPull(objItem.SupplierID ?? 0, objItem.Room ?? 0, 7);

                        if (objpullSchedule != null && objpullSchedule.Pull_ScheduleMode == 5)
                        {
                            if (AllPullTypes.Contains((ObjPull.ActionType ?? "").ToLower()) || AllPullTypes.Contains((ObjPull.PullCredit ?? "").ToLower()))
                            {
                                if (objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                                {
                                    UpdateBillingFlag = true;
                                }
                                else
                                {
                                    //RoomDTO objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDPlain(objItem.Room.GetValueOrDefault(0));
                                    string columnList = "ID,RoomName,IsWOSignatureRequired";
                                    RoomDTO objRoomDTO = new CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objItem.Room.GetValueOrDefault(0).ToString() + "", "");

                                    if (objRoomDTO != null && objRoomDTO.IsWOSignatureRequired == true)
                                    {
                                        WorkOrderDTO objWorkOrderDTO = new WorkOrderDAL(base.DataBaseName).GetWorkOrderByGUIDPlain(objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty));
                                        if (objWorkOrderDTO != null) // && objWorkOrderDTO.CreatedFrom.GetValueOrDefault(0) == (int)WorkOrderCreatedFrom.EDIService) // 1 = Web,2 = EDI Service
                                        {
                                            if (!string.IsNullOrWhiteSpace(objWorkOrderDTO.SignatureName))
                                            {
                                                UpdateBillingFlag = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        UpdateBillingFlag = true;
                                    }
                                }
                            }
                        }
                    }

                    ObjPull.ItemOnhandQty = objItem.OnHandQuantity;
                    ObjPull.ItemStageQty = objItem.StagedQuantity;
                    ObjPull.ItemLocationOnhandQty = 0;
                    ObjPull.ItemStageLocationQty = 0;

                    if (UpdateBillingFlag)
                    {
                        ObjPull.Billing = true;
                    }

                    if (context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.BinID == objDTO.BinID).Any())
                    {
                        ObjPull.ItemLocationOnhandQty = context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.BinID == objDTO.BinID).Sum(t => (t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0));
                    }

                    if (context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.StagingBinId == objDTO.BinID).Any())
                    {
                        ObjPull.ItemStageLocationQty = context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.StagingBinId == objDTO.BinID).Sum(t => (t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0));
                    }

                    IQueryable<PullDetail> lstPulldetails = context.PullDetails.Where(t => t.PULLGUID == ObjPull.GUID && (t.IsDeleted ?? false) == false).OrderByDescending(t => t.ID);

                    if (lstPulldetails.Any())
                    {
                        double PreviousQuantity = 0;

                        if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && arrpullnames.Contains(ObjPull.ActionType.ToLower()))
                        {
                            foreach (var item in lstPulldetails)
                            {
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && (ObjPull.ActionType.ToLower() == "pull" || ObjPull.ActionType.ToLower() == "p_evmi"))
                                {
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0) + PreviousQuantity;
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0) + PreviousQuantity;
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0);
                                }

                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && ObjPull.ActionType.ToLower() == "ms pull")
                                {
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0) + PreviousQuantity;
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0) + PreviousQuantity;
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0);
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0);
                                }
                                PreviousQuantity = PreviousQuantity + item.PoolQuantity ?? 0;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && arrCreditNames.Contains(ObjPull.ActionType.ToLower()))
                        {
                            foreach (var item in lstPulldetails)
                            {
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && (ObjPull.ActionType.ToLower() == "cr_evmi" || ObjPull.ActionType.ToLower() == "credit" || ObjPull.ActionType.ToLower() == "credit:evmi"))
                                {
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0) - PreviousQuantity;
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0) - PreviousQuantity;
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0);
                                }
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && ObjPull.ActionType.ToLower() == "ms credit")
                                {
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0) - PreviousQuantity;
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0);
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0) - PreviousQuantity;
                                }
                                PreviousQuantity = PreviousQuantity + item.PoolQuantity ?? 0;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
            return objDTO;
        }

        public PullMasterViewDTO UpdateCumulativeOnHandNew(PullMasterViewDTO objDTO)
        {
            PullMaster ObjPull = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool UpdateBillingFlag = false;
                string[] arrpullnames = new string[] { "p_evmi", "pull", "ms pull" };
                string[] arrCreditNames = new string[] { "cr_evmi", "credit", "ms credit", "credit:evmi" };
                string[] AllPullTypes = new string[] { "p_evmi", "pull", "ms pull", "cr_evmi", "credit", "ms credit", "credit:evmi" };
                ObjPull = context.PullMasters.FirstOrDefault(t => t.GUID == objDTO.GUID);
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objDTO.ItemGUID);

                if (objItem != null && ObjPull != null)
                {
                    if (objDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                    {
                        SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                        PullSchedulerDTO objpullSchedule = objSupplierMasterDAL.GetRoomScheduleForPull(objItem.SupplierID ?? 0, objItem.Room ?? 0, 7);

                        if (objpullSchedule != null && objpullSchedule.Pull_ScheduleMode == 5)
                        {
                            if (AllPullTypes.Contains((ObjPull.ActionType ?? "").ToLower()) || AllPullTypes.Contains((ObjPull.PullCredit ?? "").ToLower()))
                            {
                                UpdateBillingFlag = true;
                            }
                        }
                    }

                    ObjPull.ItemOnhandQty = objItem.OnHandQuantity;
                    ObjPull.ItemStageQty = objItem.StagedQuantity;
                    ObjPull.ItemLocationOnhandQty = 0;
                    ObjPull.ItemStageLocationQty = 0;

                    if (UpdateBillingFlag)
                    {
                        ObjPull.Billing = true;
                    }

                    if (context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.BinID == objDTO.BinID).Any())
                    {
                        ObjPull.ItemLocationOnhandQty = context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.BinID == objDTO.BinID).Sum(t => (t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0));
                    }

                    if (context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.StagingBinId == objDTO.BinID).Any())
                    {
                        ObjPull.ItemStageLocationQty = context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.StagingBinId == objDTO.BinID).Sum(t => (t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0));
                    }

                    IQueryable<PullDetail> lstPulldetails = context.PullDetails.Where(t => t.PULLGUID == ObjPull.GUID && (t.IsDeleted ?? false) == false).OrderByDescending(t => t.ID);

                    if (lstPulldetails.Any())
                    {
                        double PreviousQuantity = 0;

                        if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && arrpullnames.Contains(ObjPull.ActionType.ToLower()))
                        {
                            foreach (var item in lstPulldetails)
                            {
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && (ObjPull.ActionType.ToLower() == "pull" || ObjPull.ActionType.ToLower() == "p_evmi"))
                                {
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0) + PreviousQuantity;
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0) + PreviousQuantity;
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0);
                                }
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && ObjPull.ActionType.ToLower() == "ms pull")
                                {
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0) + PreviousQuantity;
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0) + PreviousQuantity;
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0);
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0);
                                }
                                PreviousQuantity = PreviousQuantity + item.PoolQuantity ?? 0;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && arrCreditNames.Contains(ObjPull.ActionType.ToLower()))
                        {
                            foreach (var item in lstPulldetails)
                            {
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && (ObjPull.ActionType.ToLower() == "cr_evmi" || ObjPull.ActionType.ToLower() == "credit" || ObjPull.ActionType.ToLower() == "credit:evmi"))
                                {
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0) - PreviousQuantity;
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0) - PreviousQuantity;
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0);
                                }
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && ObjPull.ActionType.ToLower() == "ms credit")
                                {
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0) - PreviousQuantity;
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0);
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0) - PreviousQuantity;
                                }
                                PreviousQuantity = PreviousQuantity + item.PoolQuantity ?? 0;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
            return objDTO;
        }

        public void DoILAndPDNegative(PullMasterViewDTO ObjPull, double TakenQty, ItemMasterDTO objItem)
        {
            double pulleddtl = (ObjPull.PoolQuantity ?? 0) - TakenQty;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (ObjPull.PoolQuantity >= pulleddtl)
                {
                    ItemLocationDetail objItemLocationDetail = new ItemLocationDetail();
                    objItemLocationDetail.AddedFrom = "web";
                    if (!string.IsNullOrEmpty(ObjPull.AddedFrom))
                        objItemLocationDetail.AddedFrom = ObjPull.AddedFrom;
                    objItemLocationDetail.BinID = ObjPull.BinID;
                    objItemLocationDetail.CompanyID = ObjPull.CompanyID;
                    objItemLocationDetail.Cost = objItem.Cost;
                    objItemLocationDetail.Created = DateTime.UtcNow;
                    objItemLocationDetail.CreatedBy = ObjPull.LastUpdatedBy;
                    objItemLocationDetail.EditedFrom = "web";
                    if (!string.IsNullOrEmpty(ObjPull.EditedFrom))
                        objItemLocationDetail.EditedFrom = ObjPull.EditedFrom;
                    objItemLocationDetail.eVMISensorID = null;
                    objItemLocationDetail.eVMISensorPort = null;
                    objItemLocationDetail.Expiration = null;
                    objItemLocationDetail.ExpirationDate = null;
                    objItemLocationDetail.GUID = Guid.NewGuid();
                    objItemLocationDetail.ID = 0;
                    objItemLocationDetail.InitialQuantity = 0;
                    objItemLocationDetail.InitialQuantityPDA = 0;
                    objItemLocationDetail.InitialQuantityWeb = 0;
                    objItemLocationDetail.InsertedFrom = "web";
                    if (!string.IsNullOrEmpty(ObjPull.AddedFrom))
                        objItemLocationDetail.InsertedFrom = ObjPull.AddedFrom;
                    objItemLocationDetail.IsArchived = false;
                    objItemLocationDetail.IsDeleted = false;
                    objItemLocationDetail.IsPDAEdit = false;
                    objItemLocationDetail.IsWebEdit = true;
                    objItemLocationDetail.ItemGUID = ObjPull.ItemGUID;
                    objItemLocationDetail.KitDetailGUID = null;
                    objItemLocationDetail.LastUpdatedBy = ObjPull.LastUpdatedBy;
                    objItemLocationDetail.MeasurementID = null;
                    objItemLocationDetail.OrderDetailGUID = null;
                    objItemLocationDetail.PULLGUID = null;
                    objItemLocationDetail.Received = null;
                    objItemLocationDetail.ReceivedDate = DateTime.UtcNow.Date;
                    objItemLocationDetail.ReceivedOn = DateTime.UtcNow;
                    objItemLocationDetail.ReceivedOnWeb = DateTime.UtcNow;
                    objItemLocationDetail.RefPDASelfGUID = null;
                    objItemLocationDetail.RefWebSelfGUID = null;
                    objItemLocationDetail.Room = ObjPull.Room;
                    objItemLocationDetail.TransferDetailGUID = null;
                    objItemLocationDetail.UDF1 = null;
                    objItemLocationDetail.UDF2 = null;
                    objItemLocationDetail.UDF3 = null;
                    objItemLocationDetail.UDF4 = null;
                    objItemLocationDetail.UDF5 = null;
                    objItemLocationDetail.Updated = DateTime.UtcNow;

                    if (objItem.Consignment)
                    {
                        objItemLocationDetail.IsConsignedSerialLot = true;
                        objItemLocationDetail.ConsignedQuantity = pulleddtl * (-1);
                        objItemLocationDetail.CustomerOwnedQuantity = 0;
                        objItemLocationDetail.LotNumber = null;
                        objItemLocationDetail.SerialNumber = null;
                    }
                    else
                    {
                        objItemLocationDetail.IsConsignedSerialLot = false;
                        objItemLocationDetail.ConsignedQuantity = 0;
                        objItemLocationDetail.CustomerOwnedQuantity = pulleddtl * (-1);
                        objItemLocationDetail.LotNumber = null;
                        objItemLocationDetail.SerialNumber = null;
                    }

                    context.ItemLocationDetails.Add(objItemLocationDetail);

                    int costUOMValue = 1;
                    if (objItem != null)
                    {
                        CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItem.CostUOMID.GetValueOrDefault(0));
                        if (oCostUOMMasterDTO != null)
                        {
                            costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                            if (costUOMValue == 0)
                                costUOMValue = 1;
                        }
                    }

                    PullDetail objPullDetail = new PullDetail();
                    objPullDetail.AddedFrom = "web";
                    if (!string.IsNullOrEmpty(ObjPull.AddedFrom))
                        objPullDetail.AddedFrom = ObjPull.AddedFrom;
                    objPullDetail.BinID = ObjPull.BinID;
                    objPullDetail.CompanyID = ObjPull.CompanyID;
                    objPullDetail.Created = DateTime.UtcNow;
                    objPullDetail.CreatedBy = ObjPull.LastUpdatedBy;
                    objPullDetail.CreditConsignedQuantity = 0;
                    objPullDetail.CreditCustomerOwnedQuantity = 0;
                    objPullDetail.EditedFrom = "web";
                    if (!string.IsNullOrEmpty(ObjPull.EditedFrom))
                        objPullDetail.EditedFrom = ObjPull.EditedFrom;
                    objPullDetail.Expiration = null;
                    objPullDetail.GUID = Guid.NewGuid();
                    objPullDetail.ID = 0;
                    objPullDetail.IsAddedFromPDA = false;
                    objPullDetail.IsArchived = false;
                    objPullDetail.IsDeleted = false;
                    objPullDetail.IsProcessedAfterSync = true;
                    objPullDetail.ItemCost = objItem.SellPrice.GetValueOrDefault(0) > 0 ? (objItem.SellPrice / costUOMValue) : 0; //objItem.Cost;
                    objPullDetail.ItemPrice = objItem.Cost.GetValueOrDefault(0) > 0 ? (objItem.Cost / costUOMValue) : 0;
                    objPullDetail.ItemGUID = objItem.GUID;
                    objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;
                    objPullDetail.LastUpdatedBy = ObjPull.LastUpdatedBy;
                    objPullDetail.LotNumber = null;
                    objPullDetail.MaterialStagingPullDetailGUID = null;
                    objPullDetail.ProjectSpendGUID = ObjPull.ProjectSpendGUID;
                    objPullDetail.PullCredit = ObjPull.ActionType;
                    objPullDetail.PULLGUID = ObjPull.GUID;
                    objPullDetail.Received = null;
                    objPullDetail.ReceivedOn = DateTime.UtcNow;
                    objPullDetail.ReceivedOnWeb = DateTime.UtcNow;
                    objPullDetail.Room = ObjPull.Room;
                    objPullDetail.SerialNumber = null;
                    objPullDetail.Updated = DateTime.UtcNow;

                    if (objItem.Consignment)
                    {
                        objPullDetail.CustomerOwnedQuantity = 0;
                        objPullDetail.ConsignedQuantity = pulleddtl;
                        objPullDetail.PoolQuantity = pulleddtl;
                    }
                    else
                    {
                        objPullDetail.CustomerOwnedQuantity = pulleddtl;
                        objPullDetail.ConsignedQuantity = 0;
                        objPullDetail.PoolQuantity = pulleddtl;
                    }
                    context.PullDetails.Add(objPullDetail);
                    context.SaveChanges();
                }
            }

        }

        #endregion


        /// <summary>
        ///  Pull Labor Item
        /// </summary>
        /// <param name="ItemPullData"></param>
        private void PullLaborItem(ItemInfoToPull ItemPullData, out Guid PullGUID)
        {
            PullMasterViewDTO PullDTO = null;
            PullDetailsDTO objPullDetailDTO = null;
            PullDetailsDAL objPullDetailDAL = null;
            WorkOrderLineItemsDAL objWOLDAL = null;
            PullGUID = Guid.Empty;

            try
            {
                PullDTO = new PullMasterViewDTO();

                if (ItemPullData.ProjectSpend != null && ItemPullData.ProjectSpend.ID > 0)
                    PullDTO.ProjectSpendGUID = ItemPullData.ProjectSpend.GUID;

                if (ItemPullData.RequisitionDTO != null)
                    PullDTO.RequisitionDetailGUID = ItemPullData.RequisitionDTO.GUID;

                if (ItemPullData.WorkOrder != null)
                    PullDTO.WorkOrderDetailGUID = ItemPullData.WorkOrder.GUID;

                if (ItemPullData.PullGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    PullDTO.GUID = ItemPullData.PullGUID.GetValueOrDefault(Guid.Empty);
                }
                else
                {
                    PullDTO.GUID = Guid.NewGuid();
                }

                PullDTO.CustomerOwnedQuantity = ItemPullData.QtyToPull;
                PullDTO.PoolQuantity = ItemPullData.QtyToPull;
                PullDTO.ActionType = ItemPullData.ActionType;
                PullDTO.ItemGUID = ItemPullData.ItemDTO.GUID;
                PullDTO.CompanyID = ItemPullData.Company.ID;
                PullDTO.Room = ItemPullData.Room.ID;
                PullDTO.PullCredit = ItemPullData.ActionType;
                PullDTO.UDF1 = ItemPullData.PullUDF1; PullDTO.UDF2 = ItemPullData.PullUDF2;
                PullDTO.UDF3 = ItemPullData.PullUDF3; PullDTO.UDF4 = ItemPullData.PullUDF4;
                PullDTO.UDF5 = ItemPullData.PullUDF5;
                PullDTO.LastUpdatedBy = ItemPullData.User.ID; PullDTO.CreatedBy = ItemPullData.User.ID;
                PullDTO.PullCost = (PullDTO.PoolQuantity.GetValueOrDefault(0) * ItemPullData.ItemDTO.Cost.GetValueOrDefault(0));
                PullDTO.PullPrice = (PullDTO.PoolQuantity.GetValueOrDefault(0) * ItemPullData.ItemDTO.Cost.GetValueOrDefault(0));
                PullDTO.PullOrderNumber = ItemPullData.PullOrderNumber;

                if (ItemPullData.CallFrom.ToUpper().Contains(".EDIWEBSERVICE") || ItemPullData.CallFrom.ToUpper().Contains(".WEBAPISERVICE"))
                {
                    PullDTO.AddedFrom = "Web Service";
                    PullDTO.EditedFrom = "Web Service";
                }
                else
                {
                    PullDTO.AddedFrom = ItemPullData.CallFrom;
                    PullDTO.EditedFrom = ItemPullData.CallFrom;
                }

                Insert(PullDTO);

                objPullDetailDTO = new PullDetailsDTO();
                objPullDetailDTO.PULLGUID = PullDTO.GUID;
                objPullDetailDTO.ItemGUID = PullDTO.ItemGUID;
                objPullDetailDTO.ProjectSpendGUID = PullDTO.ProjectSpendGUID;
                objPullDetailDTO.CustomerOwnedQuantity = ItemPullData.QtyToPull;
                objPullDetailDTO.PoolQuantity = ItemPullData.QtyToPull;
                objPullDetailDTO.BinID = PullDTO.BinID;
                objPullDetailDTO.ItemCost = ItemPullData.ItemDTO.Cost;
                objPullDetailDTO.Room = PullDTO.Room;
                objPullDetailDTO.CompanyID = PullDTO.CompanyID;
                objPullDetailDTO.LastUpdatedBy = ItemPullData.User.ID;
                objPullDetailDTO.CreatedBy = ItemPullData.User.ID;

                objPullDetailDAL = new PullDetailsDAL(base.DataBaseName);

                if (ItemPullData.CallFrom.ToUpper().Contains(".EDIWEBSERVICE") || ItemPullData.CallFrom.ToUpper().Contains(".WEBAPISERVICE"))
                {
                    objPullDetailDTO.AddedFrom = "Web Service";
                    objPullDetailDTO.EditedFrom = "Web Service";
                }
                else
                {
                    objPullDetailDTO.AddedFrom = ItemPullData.CallFrom;
                    objPullDetailDTO.EditedFrom = ItemPullData.CallFrom;
                }

                objPullDetailDTO.ID = objPullDetailDAL.Insert(objPullDetailDTO);

                if (ItemPullData.ProjectSpend != null)
                {
                    UpdateProjectSpendWithCost(ItemPullData.ItemDTO, PullDTO, ItemPullData.ProjectSpend.GUID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                }

                if (ItemPullData.WorkOrder != null)
                {
                    objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                    objWOLDAL.UpdateWOItemAndTotalCost(ItemPullData.WorkOrder.GUID.ToString(), ItemPullData.Room.ID, ItemPullData.Company.ID);
                    objWOLDAL = null;
                }
                UpdateCumulativeOnHand(PullDTO);
                PullGUID = PullDTO.GUID;
            }
            finally
            {
                PullDTO = null; objPullDetailDTO = null; objPullDetailDAL = null; objWOLDAL = null;
            }
        }

        public string PullItemFromInventory(ItemInfoToPull ItemPullData, out List<long> NegativeBinIds, out Guid PullGUID, long SessionUserId,long EnterpriseID)
        {
            NegativeBinIds = new List<long>();
            string returnMassage = string.Empty;
            PullGUID = Guid.Empty;

            try
            {
                if (ItemPullData.ItemDTO != null && ItemPullData.ItemDTO.ItemType == 4)
                {
                    PullLaborItem(ItemPullData, out PullGUID);
                }
                else
                {
                    bool IsFIFO = true;
                    if (!string.IsNullOrEmpty(ItemPullData.Room.InventoryConsuptionMethod) && ItemPullData.Room.InventoryConsuptionMethod.ToLower() == "lifo")
                        IsFIFO = false;

                    if (!ItemPullData.PullBin.IsStagingLocation)
                    {
                        returnMassage = PullItemFromGeneralBin(ItemPullData, IsFIFO, out NegativeBinIds, out PullGUID, SessionUserId, EnterpriseID);
                    }
                }
                return returnMassage;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string PullItemFromGeneralBin(ItemInfoToPull ItemPullData, bool IsFIFO, out List<long> NegativeBinIds, out Guid PullGUID, long SessionUserId,long EnterpriseId)
        {
            string returnmsg = "";
            ItemLocationDetailsDAL ItemLocationDAL = null;
            List<ItemLocationDetailsDTO> ItemLocations = null;
            ProjectSpendItemsDAL ProjectSpendItemDAL = null;
            ProjectSpendItemsDTO ProjectSpendItem = null;
            List<ItemLocationDetailsDTO> ItemLocationsUpdated = null;
            PullMasterViewDTO PullViewDTO = null;
            PullDetailsDAL pullDetailDAL = null;
            ItemMasterDAL itemDAL = null;
            WorkOrderLineItemsDAL objWOLDAL = null;
            NegativeBinIds = new List<long>();
            PullGUID = Guid.Empty;
            RoomDTO objRoomDTO = new RoomDTO();
            RoomDAL objRoomDAL = null;

            try
            {
                objRoomDAL = new RoomDAL(base.DataBaseName);
                //objRoomDTO = objRoomDAL.GetRoomByIDPlain(ItemPullData.Room.ID);
                string columnList = "ID,RoomName,MethodOfValuingInventory";
                objRoomDTO = new CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + ItemPullData.Room.ID.ToString() + "", "");

                ItemLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                ItemLocations = ItemLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, ItemPullData.PullBin.ID, ItemPullData.Room.ID, ItemPullData.Company.ID, ItemPullData.ItemDTO.GUID, null).ToList();
                var roomId = ItemPullData.Room.ID;
                var companyId = ItemPullData.Company.ID;
                var cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(roomId, companyId, -1);

                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }

                if (!ItemPullData.AllowNegative)
                {
                    returnmsg = CheckQuantity(ItemPullData, ItemLocations, EnterpriseId, cultureCode);
                    if (!string.IsNullOrEmpty(returnmsg))
                        return returnmsg;
                }
                
                if (ItemPullData.ProjectSpend != null && ItemPullData.RequisitionDTO == null)
                {
                    ProjectSpendItemDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                    var tmpsupplierIds = new List<long>();
                    //ProjectSpendItem = ProjectSpendItemDAL.GetAllRecords(ItemPullData.ProjectSpend.GUID, ItemPullData.Room.ID, ItemPullData.Company.ID, tmpsupplierIds).FirstOrDefault(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemPullData.ItemDTO.GUID);
                    ProjectSpendItem = ProjectSpendItemDAL.GetProjectSpendItem(ItemPullData.ProjectSpend.GUID, ItemPullData.Room.ID, ItemPullData.Company.ID, tmpsupplierIds, Convert.ToString(ItemPullData.ItemDTO.GUID)).FirstOrDefault();
                    returnmsg = CheckProjectLimit(ItemPullData, ProjectSpendItem, ItemLocations,EnterpriseId,cultureCode);
                    if (!string.IsNullOrEmpty(returnmsg))
                        return returnmsg;
                }

                ItemLocationsUpdated = new List<ItemLocationDetailsDTO>();
                var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", cultureCode, EnterpriseId, companyId);
                if (ItemLocations != null && ItemLocations.Count() > 0)
                {
                    double QtyToBePulled = ItemPullData.QtyToPull;

                    if (ItemPullData.lstPullDetails != null && ItemPullData.lstPullDetails.Count > 0 && (ItemLocations[0].SerialNumberTracking || ItemLocations[0].LotNumberTracking || ItemLocations[0].DateCodeTracking))
                    {
                        if (ItemLocations[0].SerialNumberTracking)
                        {
                            ItemLocations = ItemPullData.lstPullDetails.SelectMany(x => ItemLocations.Where(xPull => xPull.SerialNumber == x.SerialNumber)).ToList();
                            if (ItemLocations.Count <= 0)
                            {
                                string msgItemBinDoesnotHaveQtyToPullForSerialNo = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemBinDoesnotHaveQtyToPullForSerialNo", pullMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResPullMaster", cultureCode);
                                return string.Format(msgItemBinDoesnotHaveQtyToPullForSerialNo, ItemPullData.ItemDTO.ItemNumber, ItemPullData.PullBin.BinNumber);
                            }
                        }
                        else if (ItemLocations[0].LotNumberTracking)
                        {
                            ItemLocations = ItemPullData.lstPullDetails.SelectMany(x => ItemLocations.Where(xPull => xPull.LotNumber == x.LotNumber)).ToList();
                            if (ItemLocations.Count <= 0)
                            {
                                string msgItemBinDoesnotHaveQtyToPullForLotNo = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemBinDoesnotHaveQtyToPullForLotNo", pullMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResPullMaster", cultureCode);
                                return string.Format(msgItemBinDoesnotHaveQtyToPullForLotNo, ItemPullData.ItemDTO.ItemNumber, ItemPullData.PullBin.BinNumber);
                            }
                        }
                        else if (ItemLocations[0].DateCodeTracking)
                        {
                            ItemLocations = ItemPullData.lstPullDetails.SelectMany(x => ItemLocations.Where(xPull => xPull.ExpirationDate.Value.Date == x.ExpirationDate.Value.Date)).ToList();
                            if (ItemLocations.Count <= 0)
                            {
                                string msgItemBinDoesnotHaveQtyToPullForExpiredDate = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemBinDoesnotHaveQtyToPullForExpiredDate", pullMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResPullMaster", cultureCode);
                                return string.Format(msgItemBinDoesnotHaveQtyToPullForExpiredDate, ItemPullData.ItemDTO.ItemNumber, ItemPullData.PullBin.BinNumber);
                            }
                        }

                        List<ItemInfoToPullDetails> lstReferenceDetails = new List<ItemInfoToPullDetails>();
                        lstReferenceDetails = new List<ItemInfoToPullDetails>(ItemPullData.lstPullDetails);

                        if (ItemLocations != null)
                        {
                            foreach (var item in ItemLocations)
                            {
                                ItemLocationDetailsDTO NewItemLocation = GetNewItemLocation(item);
                                if (ItemLocations[0].SerialNumberTracking)
                                {
                                    QtyToBePulled = lstReferenceDetails.Where(x => x.SerialNumber == item.SerialNumber).Sum(x => x.Quantity).GetValueOrDefault(0);

                                    lstReferenceDetails.RemoveAll(delegate (ItemInfoToPullDetails v)
                                    {
                                        return v.SerialNumber == item.SerialNumber;
                                    });
                                }
                                else if (ItemLocations[0].LotNumberTracking)
                                {
                                    QtyToBePulled = lstReferenceDetails.Where(x => x.LotNumber == item.LotNumber).Sum(x => x.Quantity).GetValueOrDefault(0);
                                    lstReferenceDetails.RemoveAll(delegate (ItemInfoToPullDetails v)
                                    {
                                        return v.LotNumber == item.LotNumber;
                                    });
                                }
                                else if (ItemLocations[0].DateCodeTracking)
                                {
                                    QtyToBePulled = lstReferenceDetails.Where(x => x.ExpirationDate.GetValueOrDefault(DateTime.MinValue).Date == item.ExpirationDate.GetValueOrDefault(DateTime.MinValue).Date).Sum(x => x.Quantity).GetValueOrDefault(0);
                                    lstReferenceDetails.RemoveAll(delegate (ItemInfoToPullDetails v)
                                    {
                                        return v.ExpirationDate.GetValueOrDefault(DateTime.MinValue).Date == item.ExpirationDate.GetValueOrDefault(DateTime.MinValue).Date;
                                    });
                                }
                                if (QtyToBePulled > 0)
                                {
                                    if (item.CustQConsumable.GetValueOrDefault(0) >= QtyToBePulled)
                                    {
                                        item.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0) - QtyToBePulled;
                                        NewItemLocation.CustomerOwnedQuantity = QtyToBePulled;
                                        QtyToBePulled = 0;
                                    }
                                    else
                                    {
                                        QtyToBePulled = QtyToBePulled - item.CustQConsumable.GetValueOrDefault(0);
                                        NewItemLocation.CustomerOwnedQuantity = item.CustQConsumable;
                                        item.CustomerOwnedQuantity = item.CustomerOwnedQuantity - item.CustQConsumable;
                                    }
                                    if (item.ConQConsumable.GetValueOrDefault(0) >= QtyToBePulled)
                                    {
                                        item.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) - QtyToBePulled;
                                        NewItemLocation.ConsignedQuantity = QtyToBePulled;
                                        QtyToBePulled = 0;
                                    }
                                    else
                                    {
                                        QtyToBePulled = QtyToBePulled - item.ConQConsumable.GetValueOrDefault(0);
                                        NewItemLocation.ConsignedQuantity = item.ConQConsumable.GetValueOrDefault(0);
                                        item.ConsignedQuantity = item.ConsignedQuantity - item.ConQConsumable;
                                    }

                                    #region For Rool Level Average Cost and Customerowned Item

                                    if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                        && NewItemLocation.ConsignedQuantity == 0)
                                    {
                                        NewItemLocation.SellPrice = NewItemLocation.Cost + (((NewItemLocation.Cost ?? 0) * (ItemPullData.ItemDTO.Markup ?? 0)) / 100);
                                    }
                                    #endregion

                                    ItemLocationsUpdated.Add(NewItemLocation);
                                }

                                if (QtyToBePulled <= 0 && ItemLocationsUpdated.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) >= ItemPullData.QtyToPull)
                                    break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in ItemLocations)
                        {
                            ItemLocationDetailsDTO NewItemLocation = GetNewItemLocation(item);
                            if (QtyToBePulled > 0)
                            {
                                if (item.CustQConsumable.GetValueOrDefault(0) >= QtyToBePulled)
                                {
                                    item.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0) - QtyToBePulled;
                                    NewItemLocation.CustomerOwnedQuantity = QtyToBePulled;
                                    QtyToBePulled = 0;
                                }
                                else
                                {
                                    QtyToBePulled = QtyToBePulled - item.CustQConsumable.GetValueOrDefault(0);
                                    NewItemLocation.CustomerOwnedQuantity = item.CustQConsumable;
                                    item.CustomerOwnedQuantity = item.CustomerOwnedQuantity - item.CustQConsumable;
                                }

                                //if (ItemPullData.ItemDTO.Consignment && QtyToBePulled > 0)

                                if (item.ConQConsumable.GetValueOrDefault(0) >= QtyToBePulled)
                                {
                                    item.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) - QtyToBePulled;
                                    NewItemLocation.ConsignedQuantity = QtyToBePulled;
                                    QtyToBePulled = 0;
                                }
                                else
                                {
                                    QtyToBePulled = QtyToBePulled - item.ConQConsumable.GetValueOrDefault(0);
                                    NewItemLocation.ConsignedQuantity = item.ConQConsumable.GetValueOrDefault(0);
                                    item.ConsignedQuantity = item.ConsignedQuantity - item.ConQConsumable;
                                }

                                #region For Rool Level Average Cost and Customerowned Item

                                if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                    && NewItemLocation.ConsignedQuantity == 0)
                                {
                                    NewItemLocation.SellPrice = NewItemLocation.Cost + (((NewItemLocation.Cost ?? 0) * (ItemPullData.ItemDTO.Markup ?? 0)) / 100);
                                }
                                #endregion

                                ItemLocationsUpdated.Add(NewItemLocation);
                            }
                            if (QtyToBePulled <= 0 || ItemLocationsUpdated.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) >= ItemPullData.QtyToPull)
                                break;
                        }
                    }
                }

                double PulledQty = ItemLocationsUpdated.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0));

                if (ItemPullData.AllowNegative && PulledQty < ItemPullData.QtyToPull)
                {
                    double remainingQty = ItemPullData.QtyToPull - PulledQty;
                    ItemLocationsUpdated.Add(GetNewItemLocationDetailToInsertNegatie(ItemPullData, remainingQty));
                }

                PullViewDTO = CreatePullViewObject(ItemPullData, ItemLocationsUpdated,EnterpriseId);
                List<PullDetailsDTO> PullDetails = CreateListOfPullDetails(ItemPullData, PullViewDTO, ItemLocationsUpdated);
                PullViewDTO.PullCost = PullDetails.Sum(x => x.ItemCost.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0));
                PullViewDTO.PullPrice = PullDetails.Sum(x => x.ItemPrice.GetValueOrDefault(0) * x.PoolQuantity.GetValueOrDefault(0));
                bool IsLocationDataSaved = SaveItemLocations(ItemPullData, ItemLocations, ref ItemLocationsUpdated, SessionUserId,EnterpriseId);

                //------------------------------------------------------------------------------------------
                //
                foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in ItemLocationsUpdated)
                {
                    if ((ItemPullData.ItemDTO.Consignment ? objItemLocationDetailsDTO.ConsignedQuantity : objItemLocationDetailsDTO.CustomerOwnedQuantity) < 0)
                    {
                        NegativeBinIds.Add((long)objItemLocationDetailsDTO.BinID);
                    }
                }

                if (IsLocationDataSaved)
                {
                    PullViewDTO.WhatWhereAction = ItemPullData.CallFrom;

                    if (ItemPullData.CallFrom.ToUpper().Contains(".EDIWEBSERVICE") || ItemPullData.CallFrom.ToUpper().Contains(".WEBAPISERVICE"))
                    {
                        PullViewDTO.AddedFrom = "Web Service";
                        PullViewDTO.EditedFrom = "Web Service";
                    }
                    else
                    {
                        PullViewDTO.AddedFrom = ItemPullData.CallFrom;
                        PullViewDTO.EditedFrom = ItemPullData.CallFrom;
                    }

                    if (!string.IsNullOrWhiteSpace(ItemPullData.EditedFrom))
                    {
                        PullViewDTO.AddedFrom = ItemPullData.EditedFrom;
                        PullViewDTO.EditedFrom = ItemPullData.EditedFrom;
                    }
                    Insert(PullViewDTO);

                    pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
                    foreach (var item in PullDetails)
                    {
                        if (ItemPullData.CallFrom.ToUpper().Contains(".EDIWEBSERVICE") || ItemPullData.CallFrom.ToUpper().Contains(".WEBAPISERVICE"))
                        {
                            item.AddedFrom = "Web Service";
                            item.EditedFrom = "Web Service";
                        }
                        else
                        {
                            item.AddedFrom = ItemPullData.CallFrom;
                            item.EditedFrom = ItemPullData.CallFrom;
                        }
                        if (!string.IsNullOrWhiteSpace(ItemPullData.EditedFrom))
                        {
                            item.AddedFrom = ItemPullData.EditedFrom;
                            item.EditedFrom = ItemPullData.EditedFrom;
                        }
                        pullDetailDAL.Insert(item);
                    }

                    if (ItemPullData.ProjectSpend != null && ItemPullData.ProjectSpend.ID > 0)
                        UpdateProjectSpendWithCost(ItemPullData.ItemDTO, PullViewDTO, ItemPullData.ProjectSpend.GUID, ItemPullData.Room.ID, ItemPullData.Company.ID);

                    UpdateTurnsAverageUsage(PullViewDTO, SessionUserId);
                    itemDAL = new ItemMasterDAL(base.DataBaseName);
                    itemDAL.GetAndUpdateExtCostAndAvgCost(ItemPullData.ItemDTO.GUID, ItemPullData.Room.ID, ItemPullData.Company.ID);

                    objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                    if (ItemPullData.WorkOrder != null && ItemPullData.WorkOrder.ID > 0)
                        objWOLDAL.UpdateWOItemAndTotalCost(ItemPullData.WorkOrder.GUID.ToString(), ItemPullData.Room.ID, ItemPullData.Company.ID);

                    UpdateCumulativeOnHand(PullViewDTO);

                    if (ItemPullData != null && ItemPullData.ItemDTO != null && ItemPullData.ItemDTO.GUID != Guid.Empty)
                    {
                        ItemMasterDAL objItemMaster = new ItemMasterDAL(base.DataBaseName);
                        objItemMaster.EditDate(ItemPullData.ItemDTO.GUID, "EditPulledDate");
                    }

                    PullGUID = PullViewDTO.GUID;
                    return returnmsg;
                }
                string msgDataNotInsertSuccessfully = ResourceRead.GetResourceValueByKeyAndFullFilePath("DataNotInsertSuccessfully", pullMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResPullMaster", cultureCode);
                return msgDataNotInsertSuccessfully;
            }
            finally
            {
                ItemLocationDAL = null;
                ItemLocations = null;
                ProjectSpendItemDAL = null;
                ProjectSpendItem = null;
                ItemLocationsUpdated = null;
                PullViewDTO = null;
                pullDetailDAL = null;
                itemDAL = null;
                objWOLDAL = null;
            }
        }

        private ItemLocationDetailsDTO GetNewItemLocation(ItemLocationDetailsDTO item)
        {
            ItemLocationDetailsDTO obj = new ItemLocationDetailsDTO()
            {
                Action = item.Action,
                BinNumber = item.BinNumber,
                BinID = item.BinID,
                AddedFrom = item.AddedFrom,
                CompanyID = item.CompanyID,
                ConsignedQuantity = item.ConsignedQuantity,
                Cost = item.Cost,
                CountCustOrConsQty = item.CountCustOrConsQty,
                CountLineItemDtlGUID = item.CountLineItemDtlGUID,
                Created = item.Created,
                CreatedBy = item.CreatedBy,
                CreatedByName = item.CreatedByName,
                CreatedDate = item.CreatedDate,
                CriticalQuantity = item.CriticalQuantity,
                CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                DateCodeTracking = item.DateCodeTracking,
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
                ItemNumber = item.ItemNumber,
                ItemType = item.ItemType,
                KitDetailGUID = item.KitDetailGUID,
                LastUpdatedBy = item.LastUpdatedBy,
                LotNumber = item.LotNumber,
                MeasurementID = item.MeasurementID,
                ProjectSpentGUID = item.ProjectSpentGUID,
                LotNumberTracking = item.LotNumberTracking,
                mode = item.mode,
                Markup = item.Markup,
                MaximumQuantity = item.MaximumQuantity,
                MinimumQuantity = item.MinimumQuantity,
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
                RoomName = item.RoomName,
                SellPrice = item.SellPrice,
                SerialNumber = item.SerialNumber,
                SerialNumberTracking = item.SerialNumberTracking,
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
                UpdatedByName = item.UpdatedByName,
                UpdatedDate = item.UpdatedDate,
                WorkOrderGUID = item.WorkOrderGUID,
                ConQConsumable = item.ConQConsumable,
                CustQConsumable = item.CustQConsumable,
                ConQRunningTotal = item.ConQRunningTotal,
                CustQRunningTotal = item.CustQRunningTotal
            };

            return obj;
        }

        private bool SaveItemLocations(ItemInfoToPull ItemPullData, List<ItemLocationDetailsDTO> ItemLocations, ref List<ItemLocationDetailsDTO> ItemLocationsUpdated, long SessionUserId
            ,long EnterpriseId)
        {
            ItemLocationDetailsDAL ItemLocDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            List<ItemLocationQTYDTO> itemLocQty = null;
            ItemLocationQTYDAL itemLocQtyDAL = null;
            ItemLocationDetailsDTO objItemLocationDetailsDTO;

            if (ItemLocationsUpdated != null && ItemLocationsUpdated.Count > 0)
            {
                foreach (var item in ItemLocationsUpdated)
                {
                    if (item.ID <= 0)
                    {
                        objItemLocationDetailsDTO = ItemLocDAL.InsertByGuid(item);
                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                        {
                            item.ID = objItemLocationDetailsDTO.ID;
                        }
                    }
                }
            }

            if (ItemLocations != null && ItemLocations.Count() > 0)
            {
                foreach (var item in ItemLocations)
                {
                    if (ItemLocationsUpdated.FindIndex(x => x.GUID == item.GUID) >= 0)
                    {
                        item.EditedFrom = ItemPullData.CallFrom;
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.Updated = DateTimeUtility.DateTimeNow;
                        item.InsertedFrom = ItemPullData.CallFrom;
                        ItemLocDAL.Edit(item);
                    }
                }
            }

            itemLocQty = new List<ItemLocationQTYDTO>();

            ItemLocationQTYDTO LocQty = new ItemLocationQTYDTO()
            {
                AddedFrom = ItemPullData.CallFrom,
                BinID = ItemPullData.PullBin.ID,
                ItemGUID = ItemPullData.ItemDTO.GUID,
                CompanyID = ItemPullData.Company.ID,
                Room = ItemPullData.Room.ID,
                EditedFrom = ItemPullData.CallFrom,
                CreatedBy = ItemPullData.User.ID,
                LastUpdatedBy = ItemPullData.User.ID,
                Created = DateTimeUtility.DateTimeNow,
                LastUpdated = DateTimeUtility.DateTimeNow,
            };
            itemLocQty.Add(LocQty);
            itemLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
            itemLocQtyDAL.Save(itemLocQty, SessionUserId,EnterpriseId);

            return true;
        }

        private PullMasterViewDTO CreatePullViewObject(ItemInfoToPull ItemPullData, List<ItemLocationDetailsDTO> ItemLocationsUpdated,long EnterpriseId)
        {
            Guid? projectSpendGUID = null;
            Int64? binID = null;
            Int64? createdBy = null;
            Int64? lastUpdatedBy = null;
            Guid? requisitionDetailGUID = null;
            Guid? workOrderDetailGUID = null;
            Guid? gPullGUID = Guid.NewGuid();

            if (ItemPullData.ProjectSpend != null)
                projectSpendGUID = ItemPullData.ProjectSpend.GUID;
            if (ItemPullData.PullBin != null)
                binID = ItemPullData.PullBin.ID;
            if (ItemPullData.User != null)
                createdBy = ItemPullData.User.ID;
            if (ItemPullData.User != null)
                lastUpdatedBy = ItemPullData.User.ID;
            if (ItemPullData.RequisitionDTO != null)
                requisitionDetailGUID = ItemPullData.RequisitionDTO.GUID;
            if (ItemPullData.WorkOrder != null)
                workOrderDetailGUID = ItemPullData.WorkOrder.GUID;

            if (ItemPullData.PullGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                gPullGUID = ItemPullData.PullGUID;
            }

            PullMasterViewDTO PullViewDTO = new PullMasterViewDTO()
            {
                ID = 0,
                GUID = gPullGUID.GetValueOrDefault(Guid.Empty),
                WhatWhereAction = ItemPullData.CallFrom,
                ItemOnhandQty = ItemPullData.ItemDTO.OnHandQuantity,
                ItemStageQty = ItemPullData.ItemDTO.StagedQuantity,
                ItemLocationOnHandQty = null,
                ItemStageLocationQty = null,
                CreditCustomerOwnedQuantity = null,
                CreditConsignedQuantity = null,
                BinID = binID,
                CreatedBy = createdBy,
                LastUpdatedBy = lastUpdatedBy,
                ProjectSpendGUID = projectSpendGUID,
                RequisitionDetailGUID = requisitionDetailGUID,
                WorkOrderDetailGUID = workOrderDetailGUID,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                AddedFrom = ItemPullData.EditedFrom ?? ItemPullData.CallFrom,
                EditedFrom = ItemPullData.EditedFrom ?? ItemPullData.CallFrom,
                UpdatedByName = ItemPullData.User.UserName,
                CreatedByName = ItemPullData.User.UserName,
                ItemNumber = ItemPullData.ItemDTO.ItemNumber,
                BinNumber = ItemPullData.PullBin.BinNumber,
                RoomName = ItemPullData.Room.RoomName,
            };

            PullViewDTO.CustomerOwnedQuantity = ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
            PullViewDTO.CustomerOwnedQuantity = PullViewDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) * -1);
            PullViewDTO.ConsignedQuantity = ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
            PullViewDTO.ConsignedQuantity = PullViewDTO.ConsignedQuantity.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) < 0).Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) * -1);
            PullViewDTO.PoolQuantity = (PullViewDTO.ConsignedQuantity.GetValueOrDefault(0)) + (PullViewDTO.CustomerOwnedQuantity.GetValueOrDefault(0));
            if (workOrderDetailGUID != null && workOrderDetailGUID != Guid.Empty
                && (ItemPullData.ActionType.ToLower() == "pull" || ItemPullData.ActionType.ToLower() == "ms pull")
                   && ItemPullData.SellPrice.GetValueOrDefault(0) > 0)
            {
                double ItemPullCost = 0;
                double ItemSellPriceCost = 0;

                ItemSellPriceCost = ItemPullData.SellPrice.GetValueOrDefault(0);
                if (ItemPullData.ItemDTO != null
                    && ItemPullData.ItemDTO.Markup.GetValueOrDefault(0) > 0
                    && ItemSellPriceCost > 0)
                {
                    ItemPullCost = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(ItemSellPriceCost)) / (Convert.ToDecimal(ItemPullData.ItemDTO.Markup) + Convert.ToDecimal(100)));
                }
                else
                { ItemPullCost = ItemSellPriceCost; }

                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) * ItemPullCost);
                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) * -1) * ItemPullCost);
                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) * ItemPullCost);
                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) * -1) * ItemPullCost);
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) * ItemSellPriceCost);
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) * -1) * ItemSellPriceCost);
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) * ItemSellPriceCost);
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) * -1) * ItemSellPriceCost);
            }
            else
            {
                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));
                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) * -1) * x.Cost.GetValueOrDefault(0));
                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));
                PullViewDTO.PullCost = PullViewDTO.PullCost.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) * -1) * x.Cost.GetValueOrDefault(0));
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) * x.SellPrice.GetValueOrDefault(0));
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.CustomerOwnedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) * -1) * x.SellPrice.GetValueOrDefault(0));
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0).Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) * x.SellPrice.GetValueOrDefault(0));
                PullViewDTO.PullPrice = PullViewDTO.PullPrice.GetValueOrDefault(0) + ItemLocationsUpdated.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) < 0).Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) * -1) * x.SellPrice.GetValueOrDefault(0));
            }
            PullViewDTO.ItemGUID = ItemPullData.ItemDTO.GUID;
            PullViewDTO.CompanyID = ItemPullData.Company.ID;
            PullViewDTO.Room = ItemPullData.Room.ID;
            PullViewDTO.ActionType = ItemPullData.ActionType;
            PullViewDTO.PullCredit = ItemPullData.ActionType;
            PullViewDTO.UDF1 = ItemPullData.PullUDF1;
            PullViewDTO.UDF2 = ItemPullData.PullUDF2;
            PullViewDTO.UDF3 = ItemPullData.PullUDF3;
            PullViewDTO.UDF4 = ItemPullData.PullUDF4;
            PullViewDTO.UDF5 = ItemPullData.PullUDF5;
            PullViewDTO.PullOrderNumber = ItemPullData.PullOrderNumber;
            PullViewDTO.SupplierAccountGuid = ItemPullData.SupplierAccountGuid;

            if (ItemPullData.ItemDTO.Consignment && string.IsNullOrWhiteSpace(ItemPullData.PullOrderNumber))
            {
                AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(ItemPullData.Room.ID, ItemPullData.Company.ID, ItemPullData.ItemDTO.SupplierID ?? 0, ItemPullData.ItemDTO.GUID, EnterpriseId, null, false);
                if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
                {
                    PullViewDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
                }
                else
                {
                    NotificationDAL objNotificationDAL = new NotificationDAL(base.DataBaseName);
                    SchedulerDTO objSchedulerDTO = objNotificationDAL.GetRoomSchedulesBySupplierID(ItemPullData.ItemDTO.SupplierID ?? 0, ItemPullData.Room.ID, ItemPullData.Company.ID);
                    bool isPoRequired = true;
                    if (objSchedulerDTO != null && objSchedulerDTO.ScheduleMode == 6)
                    {
                        if (PullViewDTO.PullOrderNumber == null)
                            PullViewDTO.PullOrderNumber = "";
                        isPoRequired = false;
                    }
                    if (isPoRequired)
                    {
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(ItemPullData.Room.ID, ItemPullData.Company.ID, 0);
                        PullViewDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
                        objRegionSettingDAL = null;
                    }
                }
            }

            return PullViewDTO;
        }

        private List<PullDetailsDTO> CreateListOfPullDetails(ItemInfoToPull ItemPullData, PullMasterViewDTO PullViewDTO, List<ItemLocationDetailsDTO> ItemLocationsUpdated)
        {
            List<PullDetailsDTO> lstPullDetails = new List<PullDetailsDTO>();
            Guid? projectSpendGUID = null;
            long? binID = null;
            long? createdBy = null;
            long? lastUpdatedBy = null;
            Guid? requisitionDetailGUID = null;
            Guid? workOrderDetailGUID = null;

            if (ItemPullData.ProjectSpend != null)
                projectSpendGUID = ItemPullData.ProjectSpend.GUID;
            if (ItemPullData.PullBin != null)
                binID = ItemPullData.PullBin.ID;
            if (ItemPullData.User != null)
                createdBy = ItemPullData.User.ID;
            if (ItemPullData.User != null)
                lastUpdatedBy = ItemPullData.User.ID;
            if (ItemPullData.RequisitionDTO != null)
                requisitionDetailGUID = ItemPullData.RequisitionDTO.GUID;
            if (ItemPullData.WorkOrder != null)
                workOrderDetailGUID = ItemPullData.WorkOrder.GUID;

            double ItemPullCost = ItemPullData.ItemDTO.Cost.GetValueOrDefault(0);
            double ItemSellPriceCost = ItemPullData.ItemDTO.SellPrice.GetValueOrDefault(0);
            if (workOrderDetailGUID != null && workOrderDetailGUID != Guid.Empty
                && (ItemPullData.ActionType.ToLower() == "pull" || ItemPullData.ActionType.ToLower() == "ms pull")
                && ItemPullData.SellPrice.GetValueOrDefault(0) > 0)
            {
                ItemSellPriceCost = ItemPullData.SellPrice.GetValueOrDefault(0);
                if (ItemPullData.ItemDTO != null
                    && ItemPullData.ItemDTO.Markup.GetValueOrDefault(0) > 0
                    && ItemSellPriceCost > 0)
                {
                    ItemPullCost = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(ItemSellPriceCost)) / (Convert.ToDecimal(ItemPullData.ItemDTO.Markup) + Convert.ToDecimal(100)));
                }
                else
                { ItemPullCost = ItemSellPriceCost; }
            }

            foreach (var item in ItemLocationsUpdated)
            {
                if (ItemPullData.lstPullDetails != null && ItemPullData.lstPullDetails.Count > 0
                    && (item.SerialNumberTracking || item.LotNumberTracking || item.DateCodeTracking))
                {
                    List<ItemInfoToPullDetails> lstFilterData = new List<ItemInfoToPullDetails>();

                    if (item.SerialNumberTracking)
                    {
                        lstFilterData = ItemPullData.lstPullDetails.Where(x => x.PullGUID == PullViewDTO.GUID && x.SerialNumber == item.SerialNumber).ToList();
                    }
                    else if (item.LotNumberTracking)
                    {
                        lstFilterData = ItemPullData.lstPullDetails.Where(x => x.PullGUID == PullViewDTO.GUID && x.LotNumber == item.LotNumber).ToList();
                    }
                    else if (item.DateCodeTracking)
                    {
                        lstFilterData = ItemPullData.lstPullDetails.Where(x => x.PullGUID == PullViewDTO.GUID && x.ExpirationDate.Value.Date == item.ExpirationDate.Value.Date).ToList();
                    }

                    foreach (ItemInfoToPullDetails objPullDetails in lstFilterData)
                    {
                        double? calcItemCost = CalculateAndGetPullCost(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemSellPriceCost, item.SellPrice, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                        double? calcItemPrice = CalculateAndGetPullPrice(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemPullCost, item.Cost, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                        if (workOrderDetailGUID != null && workOrderDetailGUID != Guid.Empty
                            && (ItemPullData.ActionType.ToLower() == "pull" || ItemPullData.ActionType.ToLower() == "ms pull")
                            && ItemPullData.SellPrice.GetValueOrDefault(0) > 0)
                        {
                            calcItemCost = CalculateAndGetPullCost(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemSellPriceCost, ItemSellPriceCost, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                            calcItemPrice = CalculateAndGetPullPrice(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemPullCost, ItemPullCost, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                        }

                        PullDetailsDTO pullDtl = new PullDetailsDTO()
                        {
                            ID = 0,
                            GUID = Guid.NewGuid(),
                            PullCredit = ItemPullData.ActionType,
                            PULLGUID = PullViewDTO.GUID,
                            ItemGUID = ItemPullData.ItemDTO.GUID,
                            BinID = ItemPullData.PullBin.ID,
                            CompanyID = ItemPullData.Company.ID,
                            Room = ItemPullData.Room.ID,
                            ItemCost = calcItemCost,
                            ItemPrice = calcItemPrice,
                            PoolQuantity = (item.CustomerOwnedQuantity.GetValueOrDefault(0)) + (item.ConsignedQuantity.GetValueOrDefault(0)),
                            ConsignedQuantity = item.ConsignedQuantity,
                            CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                            LotNumber = objPullDetails.LotNumber,
                            SerialNumber = objPullDetails.SerialNumber,
                            Received = item.Received,
                            Expiration = (!string.IsNullOrEmpty(Convert.ToString(objPullDetails.ExpirationDate)) && objPullDetails.ExpirationDate != DateTime.MinValue ? Convert.ToString(objPullDetails.ExpirationDate.Value.ToShortDateString()) : ""),
                            ItemLocationDetailGUID = item.GUID,
                            ProjectSpendGUID = projectSpendGUID,
                            WorkOrderGUID = workOrderDetailGUID,
                            ItemOnhandQty = ItemPullData.ItemDTO.OnHandQuantity,
                            ItemType = ItemPullData.ItemDTO.ItemType,
                            CreatedBy = ItemPullData.User.ID,
                            LastUpdatedBy = ItemPullData.User.ID,
                            ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                            ReceivedOn = DateTimeUtility.DateTimeNow,
                            Created = DateTimeUtility.DateTimeNow,
                            Updated = DateTimeUtility.DateTimeNow,
                            AddedFrom = ItemPullData.EditedFrom ?? ItemPullData.CallFrom,
                            EditedFrom = ItemPullData.EditedFrom ?? ItemPullData.CallFrom,
                            UpdatedByName = ItemPullData.User.UserName,
                            CreatedByName = ItemPullData.User.UserName,
                            ItemNumber = ItemPullData.ItemDTO.ItemNumber,
                            BinName = ItemPullData.PullBin.BinNumber,
                            RoomName = ItemPullData.Room.RoomName,
                            ProjectSpendName = null,
                            ReceivedOnDate = null,
                            ReceivedOnDateWeb = null,
                            UpdatedDate = null,
                            CreatedDate = null,
                            CreditBinID = null,
                            CreditBinName = null,
                            COuntLineItemGUID = null,
                            CreditConsignedQuantity = null,
                            CreditCustomerOwnedQuantity = null,
                            CreditProjectName = null,
                            CreditUDF1 = null,
                            CreditUDF2 = null,
                            CreditUDF3 = null,
                            CreditUDF4 = null,
                            CreditUDF5 = null,
                            MaterialStagingPullDetailGUID = null,
                            IsAddedFromPDA = false,
                            IsArchived = false,
                            IsDeleted = false,
                            IsProcessedAfterSync = false,
                        };

                        if (pullDtl.CustomerOwnedQuantity.GetValueOrDefault(0) < 0)
                            pullDtl.CustomerOwnedQuantity = pullDtl.CustomerOwnedQuantity.GetValueOrDefault(0) * -1;

                        if (pullDtl.ConsignedQuantity.GetValueOrDefault(0) < 0)
                            pullDtl.ConsignedQuantity = pullDtl.ConsignedQuantity.GetValueOrDefault(0) * -1;

                        pullDtl.PoolQuantity = pullDtl.CustomerOwnedQuantity.GetValueOrDefault(0) + pullDtl.ConsignedQuantity.GetValueOrDefault(0);

                        if (pullDtl.ItemCost.GetValueOrDefault(0) < 0)
                            pullDtl.ItemCost = pullDtl.ItemCost.GetValueOrDefault(0) * -1;

                        lstPullDetails.Add(pullDtl);
                    }
                }
                else
                {
                    double? calcItemCost = CalculateAndGetPullCost(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemSellPriceCost, item.SellPrice, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                    double? calcItemPrice = CalculateAndGetPullPrice(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemPullCost, item.Cost, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                    if (workOrderDetailGUID != null && workOrderDetailGUID != Guid.Empty
                            && (ItemPullData.ActionType.ToLower() == "pull" || ItemPullData.ActionType.ToLower() == "ms pull")
                            && ItemPullData.SellPrice.GetValueOrDefault(0) > 0)
                    {
                        calcItemCost = CalculateAndGetPullCost(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemSellPriceCost, ItemSellPriceCost, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                        calcItemPrice = CalculateAndGetPullPrice(ItemPullData.Room.MethodOfValuingInventory, item.ConsignedQuantity.GetValueOrDefault(0) > 0, ItemPullCost, ItemPullCost, ItemPullData.ItemDTO.CostUOMID, ItemPullData.Room.ID, ItemPullData.Company.ID);
                    }

                    PullDetailsDTO pullDtl = new PullDetailsDTO()
                    {
                        ID = 0,
                        GUID = Guid.NewGuid(),
                        PullCredit = ItemPullData.ActionType,
                        PULLGUID = PullViewDTO.GUID,
                        ItemGUID = ItemPullData.ItemDTO.GUID,
                        BinID = ItemPullData.PullBin.ID,
                        CompanyID = ItemPullData.Company.ID,
                        Room = ItemPullData.Room.ID,
                        ItemCost = calcItemCost,
                        ItemPrice = calcItemPrice,
                        PoolQuantity = (item.CustomerOwnedQuantity.GetValueOrDefault(0)) + (item.ConsignedQuantity.GetValueOrDefault(0)),
                        ConsignedQuantity = item.ConsignedQuantity,
                        CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                        LotNumber = item.LotNumber,
                        SerialNumber = item.SerialNumber,
                        Received = item.Received,
                        Expiration = item.Expiration,
                        ItemLocationDetailGUID = item.GUID,
                        ProjectSpendGUID = projectSpendGUID,
                        WorkOrderGUID = workOrderDetailGUID,
                        ItemOnhandQty = ItemPullData.ItemDTO.OnHandQuantity,
                        ItemType = ItemPullData.ItemDTO.ItemType,
                        CreatedBy = ItemPullData.User.ID,
                        LastUpdatedBy = ItemPullData.User.ID,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        Created = DateTimeUtility.DateTimeNow,
                        Updated = DateTimeUtility.DateTimeNow,
                        AddedFrom = ItemPullData.EditedFrom ?? ItemPullData.CallFrom,
                        EditedFrom = ItemPullData.EditedFrom ?? ItemPullData.CallFrom,
                        UpdatedByName = ItemPullData.User.UserName,
                        CreatedByName = ItemPullData.User.UserName,
                        ItemNumber = ItemPullData.ItemDTO.ItemNumber,
                        BinName = ItemPullData.PullBin.BinNumber,
                        RoomName = ItemPullData.Room.RoomName,
                        ProjectSpendName = null,
                        ReceivedOnDate = null,
                        ReceivedOnDateWeb = null,
                        UpdatedDate = null,
                        CreatedDate = null,
                        CreditBinID = null,
                        CreditBinName = null,
                        COuntLineItemGUID = null,
                        CreditConsignedQuantity = null,
                        CreditCustomerOwnedQuantity = null,
                        CreditProjectName = null,
                        CreditUDF1 = null,
                        CreditUDF2 = null,
                        CreditUDF3 = null,
                        CreditUDF4 = null,
                        CreditUDF5 = null,
                        MaterialStagingPullDetailGUID = null,
                        IsAddedFromPDA = false,
                        IsArchived = false,
                        IsDeleted = false,
                        IsProcessedAfterSync = false,
                    };

                    if (pullDtl.CustomerOwnedQuantity.GetValueOrDefault(0) < 0)
                        pullDtl.CustomerOwnedQuantity = pullDtl.CustomerOwnedQuantity.GetValueOrDefault(0) * -1;

                    if (pullDtl.ConsignedQuantity.GetValueOrDefault(0) < 0)
                        pullDtl.ConsignedQuantity = pullDtl.ConsignedQuantity.GetValueOrDefault(0) * -1;

                    pullDtl.PoolQuantity = pullDtl.CustomerOwnedQuantity.GetValueOrDefault(0) + pullDtl.ConsignedQuantity.GetValueOrDefault(0);

                    if (pullDtl.ItemCost.GetValueOrDefault(0) < 0)
                        pullDtl.ItemCost = pullDtl.ItemCost.GetValueOrDefault(0) * -1;

                    lstPullDetails.Add(pullDtl);
                }
            }

            return lstPullDetails;
        }

        private ItemLocationDetailsDTO GetNewItemLocationDetailToInsertNegatie(ItemInfoToPull ItemPullData, double? QtytoPull)
        {
            ItemLocationDetailsDTO itemLocDtl = new ItemLocationDetailsDTO();
            itemLocDtl.ID = 0; itemLocDtl.GUID = Guid.NewGuid(); itemLocDtl.HistoryID = 0;
            itemLocDtl.BinID = ItemPullData.PullBin.ID;
            itemLocDtl.AddedFrom = ItemPullData.CallFrom;
            itemLocDtl.BinNumber = ItemPullData.PullBin.BinNumber;
            itemLocDtl.CompanyID = ItemPullData.Company.ID;
            itemLocDtl.ConsignedQuantity = ItemPullData.ItemDTO.Consignment ? QtytoPull * -1 : null;
            itemLocDtl.CustomerOwnedQuantity = !ItemPullData.ItemDTO.Consignment ? QtytoPull * -1 : null;
            itemLocDtl.Cost = ItemPullData.ItemDTO.Cost;
            itemLocDtl.DateCodeTracking = ItemPullData.ItemDTO.DateCodeTracking;
            itemLocDtl.LotNumberTracking = ItemPullData.ItemDTO.LotNumberTracking;
            itemLocDtl.SerialNumberTracking = ItemPullData.ItemDTO.SerialNumberTracking;
            itemLocDtl.InitialQuantity = 0;
            itemLocDtl.InitialQuantityWeb = 0;
            itemLocDtl.IsArchived = false; itemLocDtl.IsConsignedSerialLot = false;
            itemLocDtl.IsDefault = false; itemLocDtl.IsDeleted = false;
            itemLocDtl.IsCreditPull = false; itemLocDtl.IsOnlyFromUI = false;
            itemLocDtl.IsWebEdit = false;
            itemLocDtl.IsItemLevelMinMax = ItemPullData.ItemDTO.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false);
            itemLocDtl.ItemGUID = ItemPullData.ItemDTO.GUID;
            itemLocDtl.ItemNumber = ItemPullData.ItemDTO.ItemNumber;
            itemLocDtl.ItemType = ItemPullData.ItemDTO.ItemType;
            itemLocDtl.Markup = ItemPullData.ItemDTO.Markup;
            itemLocDtl.ProjectSpentGUID = ItemPullData.ProjectSpend != null ? (Guid?)ItemPullData.ProjectSpend.GUID : null;
            itemLocDtl.Room = ItemPullData.Room.ID;
            itemLocDtl.RoomName = ItemPullData.Room.RoomName;
            itemLocDtl.SellPrice = ItemPullData.ItemDTO.SellPrice;
            itemLocDtl.SuggestedOrderQuantity = ItemPullData.ItemDTO.SuggestedOrderQuantity;
            itemLocDtl.LastUpdatedBy = ItemPullData.User.ID;
            itemLocDtl.EditedFrom = ItemPullData.CallFrom;
            itemLocDtl.Created = DateTimeUtility.DateTimeNow;
            itemLocDtl.CreatedBy = ItemPullData.User.ID;
            itemLocDtl.CreatedByName = ItemPullData.User.UserName;
            itemLocDtl.ReceivedDate = DateTimeUtility.DateTimeNow;
            itemLocDtl.ReceivedOn = DateTimeUtility.DateTimeNow;
            itemLocDtl.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            itemLocDtl.Updated = DateTimeUtility.DateTimeNow;
            itemLocDtl.UpdatedByName = ItemPullData.User.UserName;
            itemLocDtl.InsertedFrom = ItemPullData.CallFrom;
            itemLocDtl.WorkOrderGUID = ItemPullData.WorkOrder != null ? (Guid?)ItemPullData.WorkOrder.GUID : null;
            itemLocDtl.Action = "Insert"; itemLocDtl.RefPDASelfGUID = null; itemLocDtl.RefWebSelfGUID = null;
            itemLocDtl.TransferDetailGUID = null; itemLocDtl.CountCustOrConsQty = null;
            itemLocDtl.CountLineItemDtlGUID = null; itemLocDtl.CriticalQuantity = null;
            itemLocDtl.ExpirationDate = null; itemLocDtl.InitialQuantityPDA = null;
            itemLocDtl.eVMISensorIDdbl = null; itemLocDtl.eVMISensorPort = null;
            itemLocDtl.IsPDAEdit = null; itemLocDtl.KitDetailGUID = null;
            itemLocDtl.MaximumQuantity = null; itemLocDtl.MeasurementID = null;
            itemLocDtl.MinimumQuantity = null; itemLocDtl.MoveQuantity = 0;
            itemLocDtl.OrderDetailGUID = null;
            itemLocDtl.eVMISensorPortstr = string.Empty; itemLocDtl.LotNumber = string.Empty;
            itemLocDtl.CreatedDate = string.Empty; itemLocDtl.eVMISensorID = string.Empty;
            itemLocDtl.Expiration = string.Empty; itemLocDtl.SerialNumber = string.Empty;
            itemLocDtl.Received = string.Empty; itemLocDtl.mode = string.Empty;
            itemLocDtl.PackSlipNumber = string.Empty; itemLocDtl.ReceivedOnDate = string.Empty;
            itemLocDtl.ReceivedOnDateWeb = string.Empty; itemLocDtl.UpdatedDate = string.Empty;
            itemLocDtl.UDF1 = string.Empty; itemLocDtl.UDF10 = string.Empty;
            itemLocDtl.UDF2 = string.Empty; itemLocDtl.UDF3 = string.Empty;
            itemLocDtl.UDF4 = string.Empty; itemLocDtl.UDF5 = string.Empty;
            itemLocDtl.UDF6 = string.Empty; itemLocDtl.UDF7 = string.Empty;
            itemLocDtl.UDF8 = string.Empty; itemLocDtl.UDF9 = string.Empty;

            return itemLocDtl;
        }

        private string CheckProjectLimit(ItemInfoToPull ItemPullData, ProjectSpendItemsDTO ProjectSpendItems, IEnumerable<ItemLocationDetailsDTO> ItemLocations,long EnterpriseId, string CultureCode)
        {
            double pickPrice = 0;
            double pickQTY = 0;

            if (!ItemPullData.AllowOverrideProjectSpendLimits && ItemPullData.ProjectSpend != null && ItemLocations != null && ItemLocations.Count() > 0)
            {
                foreach (var iLocDetail in ItemLocations)
                {
                    if (ItemPullData.QtyToPull <= pickQTY)
                        break;

                    if (ItemPullData.ItemDTO.Consignment)
                    {
                        if ((iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) >= (ItemPullData.QtyToPull - pickQTY))
                        {
                            pickPrice += (ItemPullData.QtyToPull - pickQTY) * (iLocDetail.Cost.GetValueOrDefault(0));
                            pickQTY += ItemPullData.QtyToPull;
                        }
                        else if ((iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) < (ItemPullData.QtyToPull - pickQTY))
                        {
                            pickQTY += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0));
                            pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) * (iLocDetail.Cost.GetValueOrDefault(0));
                        }
                    }
                    else
                    {
                        if (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) >= (ItemPullData.QtyToPull - pickQTY))
                        {
                            pickPrice += (ItemPullData.QtyToPull - pickQTY) * (iLocDetail.Cost.GetValueOrDefault(0));
                            pickQTY += ItemPullData.QtyToPull;
                        }
                        else if (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) < (ItemPullData.QtyToPull - pickQTY))
                        {
                            pickQTY += iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0);
                            pickPrice += iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) * (iLocDetail.Cost.GetValueOrDefault(0));
                        }

                    }
                }
                var roomId = ItemPullData.Room.ID;
                var companyId = ItemPullData.Company.ID;
                var projectMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResProjectMaster", CultureCode, EnterpriseId, companyId);
                if (ItemPullData.ProjectSpend.DollarLimitAmount.GetValueOrDefault(0) < (ItemPullData.ProjectSpend.DollarUsedAmount.GetValueOrDefault(0) + (decimal)pickPrice))
                    return ResourceRead.GetResourceValueByKeyAndFullFilePath("ProjectSpendDollarAmountLimitExceed", projectMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResProjectMaster", CultureCode);

                if (ProjectSpendItems != null)
                {
                    if (ProjectSpendItems.QuantityLimit.GetValueOrDefault(0) > 0 && (ItemPullData.QtyToPull + ProjectSpendItems.QuantityUsed.GetValueOrDefault(0)) > ProjectSpendItems.QuantityLimit.GetValueOrDefault(0))
                        return ResourceRead.GetResourceValueByKeyAndFullFilePath("ProjectSpendItemQuantityLimitExceed", projectMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResProjectMaster", CultureCode);
                    if (ProjectSpendItems.DollarLimitAmount.GetValueOrDefault(0) > 0 && ((decimal)pickPrice + ProjectSpendItems.DollarUsedAmount.GetValueOrDefault(0)) > ProjectSpendItems.DollarLimitAmount.GetValueOrDefault(0))
                        return ResourceRead.GetResourceValueByKeyAndFullFilePath("ProjectSpendItemDollarLimitExceed", projectMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResProjectMaster", CultureCode);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Check quantity available or not
        /// </summary>
        /// <param name="ItemPullData"></param>
        /// <param name="ItemLocations"></param>
        /// <returns></returns>
        private string CheckQuantity(ItemInfoToPull ItemPullData, IEnumerable<ItemLocationDetailsDTO> ItemLocations, long EnterpriseId, string CultureCode)
        {
            double AvailLocQty = ItemPullData.ItemDTO.OnHandQuantity.GetValueOrDefault(0);
            var roomId = ItemPullData.Room.ID;
            var companyId = ItemPullData.Company.ID;
            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, companyId);

            if (AvailLocQty < ItemPullData.QtyToPull)
            {
                var msgItemDoesNotHaveSufficientQtyToPull = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemDoesNotHaveSufficientQtyToPull", pullMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResPullMaster", CultureCode);
                return string.Format(msgItemDoesNotHaveSufficientQtyToPull, ItemPullData.ItemDTO.ItemNumber, ItemPullData.ItemDTO.OnHandQuantity.GetValueOrDefault(0));
            }                

            if (ItemLocations == null || ItemLocations.Count() <= 0)
            {
                var msgItemBinDoesNotHaveAnyQtyToPull = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemBinDoesNotHaveAnyQtyToPull", pullMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResPullMaster", CultureCode);
                return string.Format(msgItemBinDoesNotHaveAnyQtyToPull, ItemPullData.ItemDTO.ItemNumber, ItemPullData.PullBin.BinNumber);
            }
            

            if (ItemPullData.ItemDTO.Consignment)
                AvailLocQty = ItemLocations.Sum(x => x.ConQConsumable.GetValueOrDefault(0) + x.CustQConsumable.GetValueOrDefault(0));
            else
                AvailLocQty = ItemLocations.Sum(x => x.CustQConsumable.GetValueOrDefault(0));

            if (AvailLocQty < ItemPullData.QtyToPull)
            {
                var msgItemBinDoesNotHaveSufficientQtyToPull = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemBinDoesNotHaveSufficientQtyToPull", pullMasterResourceFilePath, EnterpriseId, companyId, roomId, "ResPullMaster", CultureCode);
                return string.Format(msgItemBinDoesNotHaveSufficientQtyToPull, ItemPullData.ItemDTO.ItemNumber, ItemPullData.PullBin.BinNumber, AvailLocQty); ;
            }
            

            return string.Empty;
        }

        /// <summary>
        /// Save Pull Credit
        /// </summary>
        /// <param name="pullDetails"></param>
        /// <returns></returns>
        public string SavePullCredit(ItemInfoToCredit CreditDetails, string EnterpriseDBName, long CompanyID, long RoomID, long UserID, string CallFrom, long SessionUserId, long EnterpriseId)
        {
            UDFDAL objUDFApiController = new UDFDAL(EnterpriseDBName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
            eTurnsRegionInfo RegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, UserID);
            string RoomDateFormat = "MM/dd/yyyy";
            string cultureCode = "en-US";

            if (RegionInfo != null)
            {
                cultureCode = RegionInfo.CultureCode;
                if (!string.IsNullOrEmpty(RegionInfo.ShortDatePattern))
                {
                    RoomDateFormat = RegionInfo.ShortDatePattern;
                }
            }

            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", RoomID, CompanyID);
            string udfRequier = string.Empty;
            PullMasterDAL objPullDAL = new PullMasterDAL(EnterpriseDBName);
            // RoomDTO objRoomDTO = new RoomDAL(EnterpriseDBName).GetRoomByIDPlain(RoomID);
            string columnList = "ID,RoomName,IsIgnoreCreditRule";
            RoomDTO objRoomDTO = new CommonDAL(EnterpriseDBName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            var messageResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", cultureCode, EnterpriseId, CompanyID);
            string msgRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequired", messageResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResMessage", cultureCode);

            foreach (var i in DataFromDB)
            {
                if (i.UDFColumnName == "UDF1" && string.IsNullOrWhiteSpace(CreditDetails.UDF1))
                    udfRequier = string.Format(msgRequired, i.UDFColumnName);
                else if (i.UDFColumnName == "UDF2" && string.IsNullOrWhiteSpace(CreditDetails.UDF2))
                    udfRequier = string.Format(msgRequired, i.UDFColumnName);
                else if (i.UDFColumnName == "UDF3" && string.IsNullOrWhiteSpace(CreditDetails.UDF3))
                    udfRequier = string.Format(msgRequired, i.UDFColumnName);
                else if (i.UDFColumnName == "UDF4" && string.IsNullOrWhiteSpace(CreditDetails.UDF4))
                    udfRequier = string.Format(msgRequired, i.UDFColumnName);
                else if (i.UDFColumnName == "UDF5" && string.IsNullOrWhiteSpace(CreditDetails.UDF5))
                    udfRequier = string.Format(msgRequired, i.UDFColumnName);

                if (!string.IsNullOrEmpty(udfRequier))
                    break;

            }

            if (!string.IsNullOrEmpty(udfRequier))
                return udfRequier;

            List<ItemLocationDetailsDTO> itemLocations = null;
            ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(EnterpriseDBName);

            if (CreditDetails != null)
            {
                string lstSerialNumber = string.Empty;
                
                if (CreditDetails.ItemTracking == "SERIALTRACK")
                {
                    foreach (var SerialItem in CreditDetails.PrevPullsToCredit)
                    {
                        CommonDAL objCommonDal = new CommonDAL(EnterpriseDBName);
                        string serailErrorMessage = objCommonDal.CheckDuplicateSerialNumbers(SerialItem.Serial, 0, RoomID, CompanyID, CreditDetails.ItemGuid.GetValueOrDefault(Guid.Empty));

                        if (serailErrorMessage.ToLower().Trim() == "duplicate")
                        {
                            if (lstSerialNumber != string.Empty)
                                lstSerialNumber = lstSerialNumber + " , " + SerialItem.Serial;
                            else
                                lstSerialNumber = SerialItem.Serial;
                        }
                    }

                    if (lstSerialNumber != string.Empty)
                    {
                        var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", cultureCode, EnterpriseId, CompanyID);
                        string msgCreditTransactionDoneForSerial = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditTransactionDoneForSerial", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", cultureCode);
                        return msgCreditTransactionDoneForSerial + " " + lstSerialNumber;
                    }
                }

                /* WI-4693-Allow specified rooms to ignore credit rules */
                if (!objRoomDTO.IsIgnoreCreditRule)
                {
                    List<PullDetailsDTO> prepulls = objPullDAL.GetPrevPull(CreditDetails, RoomID, CompanyID);
                    itemLocations = new List<ItemLocationDetailsDTO>();

                    foreach (var prePullItem in prepulls)
                    {
                        if (!string.IsNullOrEmpty(CallFrom))
                        {
                            if (CallFrom.ToUpper().Contains(".EDIWEBSERVICE") || CallFrom.ToUpper().Contains(".WEBAPISERVICE"))
                            {
                                prePullItem.AddedFrom = "Web Service";
                                prePullItem.EditedFrom = "Web Service";
                            }
                            else
                            {
                                prePullItem.AddedFrom = CallFrom;
                                prePullItem.EditedFrom = CallFrom;
                            }
                            if (!string.IsNullOrWhiteSpace(CreditDetails.EditedFrom))
                            {
                                prePullItem.AddedFrom = CreditDetails.EditedFrom;
                                prePullItem.EditedFrom = CreditDetails.EditedFrom;
                            }
                        }
                        itemLocations.Add(objPullDAL.ConvertPullDetailtoItemLocationDetail(prePullItem, RoomID, CompanyID, UserID, RoomDateFormat));
                    }
                    List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                    if (ildDAL.ItemLocationDetailsSaveForCreditPullnew(itemLocations, "Credit", RoomDateFormat, out lstCreditGuids, SessionUserId,EnterpriseId, "credit", CallFrom))
                    {
                        objPullDAL.UpdatePullRecordsForCreditQuantity(prepulls, RoomID, CompanyID, UserID, lstCreditGuids, CallFrom);

                        if (CreditDetails != null && CreditDetails.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(EnterpriseDBName);
                            objWOLDAL.UpdateWOItemAndTotalCost(CreditDetails.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), RoomID, CompanyID);
                        }
                    }
                }
                else
                {
                    #region WI-4693-Allow specified rooms to ignore credit rules

                    List<PullDetailsDTO> prepulls = objPullDAL.GetPrevPull(CreditDetails, RoomID, CompanyID);
                    double TotalAvailablePulls = prepulls.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    double TotalRemainingCredit = (CreditDetails.Quantity - TotalAvailablePulls);

                    List<PullDetailsDTO> pulls = new List<PullDetailsDTO>();
                    if (TotalRemainingCredit > 0)
                    {
                        pulls = objPullDAL.GetPrevPullForCreditEntry(prepulls, CreditDetails, TotalRemainingCredit, RoomID, CompanyID, RoomDateFormat,EnterpriseId,cultureCode);
                        prepulls.AddRange(pulls);
                    }
                    bool IsValid = true;

                    if (prepulls.Where(x => x.EditedFrom == "Fail").Count() > 0)
                    {
                        IsValid = false;
                    }

                    if (IsValid)
                    {
                        itemLocations = new List<ItemLocationDetailsDTO>();

                        foreach (var prePullItem in prepulls)
                        {
                            if (!string.IsNullOrEmpty(CallFrom))
                            {
                                if (CallFrom.ToUpper().Contains(".EDIWEBSERVICE") || CallFrom.ToUpper().Contains(".WEBAPISERVICE"))
                                {
                                    prePullItem.AddedFrom = "Web Service";
                                    prePullItem.EditedFrom = "Web Service";
                                }
                                else
                                {
                                    prePullItem.AddedFrom = CallFrom;
                                    prePullItem.EditedFrom = CallFrom;
                                }

                                if (!string.IsNullOrWhiteSpace(CreditDetails.EditedFrom))
                                {
                                    prePullItem.AddedFrom = CreditDetails.EditedFrom;
                                    prePullItem.EditedFrom = CreditDetails.EditedFrom;
                                }
                            }

                            itemLocations.Add(objPullDAL.ConvertPullDetailtoItemLocationDetailForCreditRule(prePullItem, RoomID, CompanyID, UserID, RoomDateFormat));
                        }

                        List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                        if (ildDAL.ItemLocationDetailsSaveForCreditPullnew(itemLocations, "Credit", RoomDateFormat, out lstCreditGuids, SessionUserId,EnterpriseId, "credit"))
                        {
                            objPullDAL.UpdatePullRecordsForCreditQuantity(prepulls, RoomID, CompanyID, UserID, lstCreditGuids);
                            if (pulls != null && pulls.Count > 0)
                            {
                                List<PullDetailsDTO> lstPulls = new List<PullDetailsDTO>();
                                lstPulls.AddRange(pulls);
                                objPullDAL.InsertintoCreditHistory(lstPulls, RoomID, CompanyID, UserID, "Pull Credit");
                            }

                            if (CreditDetails != null && CreditDetails.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(EnterpriseDBName);
                                objWOLDAL.UpdateWOItemAndTotalCost(CreditDetails.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), RoomID, CompanyID);
                            }
                        }
                    }
                    else
                    {
                        string ErrorMessage = string.Empty;
                        for (int i = 0; i < prepulls.Count; i++)
                        {
                            ErrorMessage = ErrorMessage + " , " + prepulls[i].AddedFrom;
                        }
                        return ErrorMessage;
                    }

                    #endregion
                }

                #region "Update Ext Cost And Avg Cost"
                new ItemMasterDAL(EnterpriseDBName).GetAndUpdateExtCostAndAvgCost(CreditDetails.ItemGuid ?? Guid.Empty, RoomID, CompanyID);
                #endregion
            }

            return "";
        }

        /// <summary>
        /// GetPreviousPulls
        /// </summary>
        /// <param name="itemToCredit"></param>
        /// <returns></returns>
        public List<PullDetailToCredit> GetPreviousPulls(ItemInfoToCredit itemToCredit, long RoomID, long CompanyID, long UserID,long EnterpriseId)
        {
            PullDetailsDAL pullDetailDAL = null;
            PullMasterDAL pullMasterDAL = null;
            IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
            List<PullDetailsDTO> itmDetails = null;
            List<PullDetailToCredit> pullDetailsToCred = null;

            try
            {
                pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
                pullMasterDAL = new PullMasterDAL(base.DataBaseName);
                itmDetails = new List<PullDetailsDTO>();
                pullDetailsToCred = new List<PullDetailToCredit>();
                lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryPlain(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "pull", RoomID, CompanyID);
                double remainingQty = itemToCredit.Quantity;
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                eTurnsRegionInfo RegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, UserID);
                string cultureCode = "en-US";

                if (RegionInfo != null)
                {
                    cultureCode = RegionInfo.CultureCode;
                }

                if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                {
                    double totalprevPullQty = lstPullDetailDTO.Sum(x => x.PoolQuantity.GetValueOrDefault(0) - (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
                    itemToCredit.PrevPullQty = totalprevPullQty;
                    //RoomDTO objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDPlain(RoomID);
                    string columnList = "ID,RoomName,IsIgnoreCreditRule";
                    RoomDTO objRoomDTO = new CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                    if (!objRoomDTO.IsIgnoreCreditRule)
                    {
                        if (totalprevPullQty < itemToCredit.Quantity)
                        {
                            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", cultureCode, EnterpriseId, CompanyID);
                            string msgCreditQtyGreaterThanPreviousPullQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditQtyGreaterThanPreviousPullQty", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", cultureCode);
                            itemToCredit.IsModelShow = true;
                            itemToCredit.ErrorMessage = string.Format(msgCreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity) + " (" + totalprevPullQty.ToString() + ").";
                            remainingQty = totalprevPullQty;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                        {
                            itemToCredit.IsModelShow = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                    {
                        if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                        {
                            totalprevPullQty = lstPullDetailDTO.Sum(x => x.PoolQuantity.GetValueOrDefault(0) - (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
                            itemToCredit.PrevPullQty = totalprevPullQty;

                            if (!objRoomDTO.IsIgnoreCreditRule)
                            {
                                if (totalprevPullQty < itemToCredit.Quantity)
                                {
                                    var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", cultureCode, EnterpriseId, CompanyID);
                                    string msgCreditQtyGreaterThanPreviousPullQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditQtyGreaterThanPreviousPullQty", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", cultureCode);
                                    itemToCredit.IsModelShow = true;
                                    itemToCredit.ErrorMessage = string.Format(msgCreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity) +  " (" + totalprevPullQty.ToString() + ")."; 
                                    remainingQty = totalprevPullQty;
                                }
                            }
                            else
                            {
                                itemToCredit.IsModelShow = true;
                            }

                            if (itemToCredit.ItemTracking == "SERIALTRACK")
                            {
                                itemToCredit.IsModelShow = true;

                                foreach (var item in lstPullDetailDTO)
                                {
                                    PullDetailToCredit pldtl = new PullDetailToCredit()
                                    {
                                        Serial = item.SerialNumber,
                                        ExpireDate = item.Expiration,
                                        Lot = string.Empty,
                                        Qty = item.PoolQuantity.GetValueOrDefault(0)
                                    };

                                    pullDetailsToCred.Add(pldtl);
                                }
                            }
                            else if (itemToCredit.ItemTracking == "LOTTRACK" || itemToCredit.ItemTracking == "DATECODETRACK")
                            {
                                List<PullDetailToCredit> lst = (from x in lstPullDetailDTO
                                                                group x by new { x.LotNumber, x.ItemGUID, x.Expiration } into grp
                                                                select new PullDetailToCredit
                                                                {
                                                                    Lot = grp.Key.LotNumber,
                                                                    ExpireDate = grp.Key.Expiration,
                                                                    Qty = grp.Sum(p => p.PoolQuantity.GetValueOrDefault(0) - (p.CreditConsignedQuantity.GetValueOrDefault(0) + p.CreditCustomerOwnedQuantity.GetValueOrDefault(0))),
                                                                }).ToList();
                                if (lst != null && lst.Count > 1)
                                {
                                    foreach (var item in lst)
                                    {
                                        pullDetailsToCred.Add(item);
                                    }
                                }
                                else
                                {
                                    foreach (var item in lst)
                                    {
                                        if (remainingQty <= item.Qty)
                                            item.Qty = remainingQty;
                                        else
                                            remainingQty -= item.Qty;

                                        pullDetailsToCred.Add(item);
                                    }
                                }

                                if (pullDetailsToCred.Count > 1)
                                    itemToCredit.IsModelShow = true;
                            }
                        }
                    }
                    else
                    {
                        PullDetailToCredit pldtl = new PullDetailToCredit()
                        {
                            Serial = string.Empty,
                            ExpireDate = string.Empty,
                            Lot = string.Empty,
                            Qty = remainingQty
                        };

                        pullDetailsToCred.Add(pldtl);
                    }
                }

                return pullDetailsToCred;
            }
            finally
            {
                pullDetailDAL = null;
                lstPullDetailDTO = null;
            }
        }

        /// <summary>
        /// GetPreviousPulls
        /// </summary>
        /// <param name="itemToCredit"></param>
        /// <returns></returns>
        public List<PullDetailsDTO> GetPrevPull(ItemInfoToCredit itemToCredit, long RoomID, long CompanyID)
        {
            PullDetailsDAL pullDetailDAL = null;
            IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
            List<PullDetailsDTO> itmDetails = null;
            List<PullDetailToCredit> pullDetailsToCred = null;
            IEnumerable<PullDetailsDTO> lstPullDetails2;

            try
            {
                pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
                itmDetails = new List<PullDetailsDTO>();
                pullDetailsToCred = new List<PullDetailToCredit>();

                if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                {
                    double remainingQty = itemToCredit.PrevPullsToCredit.Sum(x => x.Qty);

                    foreach (var itmCr in itemToCredit.PrevPullsToCredit)
                    {
                        lstPullDetails2 = null;

                        //if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                        //{
                        if (itemToCredit.ItemTracking == "LOTTRACK")
                        {
                            lstPullDetails2 = pullDetailDAL.GetPullDetailsByItemGuidAndLotNo_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "pull", RoomID, CompanyID, itmCr.Lot);//lstPullDetailDTO.Where(x => itmCr.Lot == x.LotNumber);
                            remainingQty = itemToCredit.PrevPullsToCredit.Where(x => itmCr.Lot == x.Lot).Sum(x => x.Qty);
                        }
                        else if (itemToCredit.ItemTracking == "DATECODETRACK")
                        {
                            lstPullDetails2 = pullDetailDAL.GetPullDetailsByItemGuidAndExpiration_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "pull", RoomID, CompanyID, itmCr.ExpireDate);//lstPullDetailDTO.Where(x => itmCr.ExpireDate == x.Expiration);
                            remainingQty = itemToCredit.PrevPullsToCredit.Where(x => itmCr.ExpireDate == x.ExpireDate).Sum(x => x.Qty);
                        }
                        else if (itemToCredit.ItemTracking == "SERIALTRACK")
                        {
                            lstPullDetails2 = pullDetailDAL.GetPullDetailsByItemGuidAndSerialNo_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "pull", RoomID, CompanyID, itmCr.Serial);//lstPullDetailDTO.Where(x => itmCr.Serial == x.SerialNumber);
                            remainingQty = itemToCredit.PrevPullsToCredit.Where(x => itmCr.Serial == x.Serial).Sum(x => x.Qty);
                        }
                        //}

                        if (lstPullDetails2 != null)
                        {
                            foreach (PullDetailsDTO objPullDetails in lstPullDetails2)
                            {
                                if (remainingQty > 0)
                                {
                                    if (remainingQty <= (objPullDetails.PoolQuantity.GetValueOrDefault(0) - (objPullDetails.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objPullDetails.CreditConsignedQuantity.GetValueOrDefault(0))))
                                    {
                                        if (objPullDetails.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.ConsignedQuantity = remainingQty;
                                        else if (objPullDetails.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.CustomerOwnedQuantity = remainingQty;

                                        remainingQty = 0;
                                    }
                                    else
                                    {
                                        remainingQty -= (objPullDetails.PoolQuantity.GetValueOrDefault(0) - (objPullDetails.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objPullDetails.CreditConsignedQuantity.GetValueOrDefault(0)));

                                        if (objPullDetails.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.ConsignedQuantity = (objPullDetails.PoolQuantity.GetValueOrDefault(0) - objPullDetails.CreditConsignedQuantity.GetValueOrDefault(0));
                                        else if (objPullDetails.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.CustomerOwnedQuantity = (objPullDetails.PoolQuantity.GetValueOrDefault(0) - objPullDetails.CreditCustomerOwnedQuantity.GetValueOrDefault(0));
                                    }

                                    if (!string.IsNullOrEmpty(itemToCredit.Bin))
                                        objPullDetails.CreditBinName = itemToCredit.Bin;
                                    else
                                        objPullDetails.CreditBinName = objPullDetails.BinName;

                                    if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                                        objPullDetails.CreditProjectName = itemToCredit.ProjectName;
                                    else
                                        objPullDetails.CreditProjectName = objPullDetails.ProjectSpendName;

                                    objPullDetails.CreditUDF1 = itemToCredit.UDF1;
                                    objPullDetails.CreditUDF2 = itemToCredit.UDF2;
                                    objPullDetails.CreditUDF3 = itemToCredit.UDF3;
                                    objPullDetails.CreditUDF4 = itemToCredit.UDF4;
                                    objPullDetails.CreditUDF5 = itemToCredit.UDF5;
                                    objPullDetails.WorkOrderGUID = itemToCredit.WOGuid;
                                    objPullDetails.PullOrderNumber = itemToCredit.PullOrderNumber;
                                    objPullDetails.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                                    objPullDetails.NEW_PullGUID = itemToCredit.PullGUID;
                                    itmDetails.Add(objPullDetails);
                                }
                            }
                        }
                    }
                }
                else if (itemToCredit.ItemTracking != "QUICKLIST")
                {
                    double remainingQty = itemToCredit.PrevPullsToCredit.Sum(x => x.Qty);
                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "pull", RoomID, CompanyID);

                    foreach (var item in lstPullDetailDTO)
                    {
                        if (remainingQty > 0)
                        {
                            if (remainingQty <= (item.PoolQuantity.GetValueOrDefault(0) - (item.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + item.CreditConsignedQuantity.GetValueOrDefault(0))))
                            {
                                if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                    item.ConsignedQuantity = remainingQty;
                                else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                    item.CustomerOwnedQuantity = remainingQty;

                                remainingQty = 0;
                            }
                            else
                            {
                                remainingQty -= (item.PoolQuantity.GetValueOrDefault(0) - (item.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + item.CreditConsignedQuantity.GetValueOrDefault(0)));

                                if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                    item.ConsignedQuantity = (item.PoolQuantity.GetValueOrDefault(0) - item.CreditConsignedQuantity.GetValueOrDefault(0));
                                else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                    item.CustomerOwnedQuantity = (item.PoolQuantity.GetValueOrDefault(0) - item.CreditCustomerOwnedQuantity.GetValueOrDefault(0));
                            }

                            if (!string.IsNullOrEmpty(itemToCredit.Bin))
                                item.CreditBinName = itemToCredit.Bin;
                            else
                                item.CreditBinName = item.BinName;

                            if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                                item.CreditProjectName = itemToCredit.ProjectName;
                            else
                                item.CreditProjectName = item.ProjectSpendName;

                            item.CreditUDF1 = itemToCredit.UDF1;
                            item.CreditUDF2 = itemToCredit.UDF2;
                            item.CreditUDF3 = itemToCredit.UDF3;
                            item.CreditUDF4 = itemToCredit.UDF4;
                            item.CreditUDF5 = itemToCredit.UDF5;
                            item.WorkOrderGUID = itemToCredit.WOGuid;
                            item.PullOrderNumber = itemToCredit.PullOrderNumber;
                            item.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                            item.NEW_PullGUID = itemToCredit.PullGUID;
                            itmDetails.Add(item);
                        }

                        if (string.IsNullOrEmpty(itemToCredit.ItemTracking) && itmDetails.Sum(x => (x.PoolQuantity.GetValueOrDefault(0) - (x.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + x.CreditConsignedQuantity.GetValueOrDefault(0)))) >= itemToCredit.Quantity)
                            break;
                    }
                }

                return itmDetails;
            }
            finally
            {

            }
        }

        /// <summary>
        /// GetPreviousPulls
        /// </summary>
        /// <param name="itemToCredit"></param>
        /// <returns></returns>
        public List<PullDetailsDTO> GetPrevMSPull(ItemInfoToMSCredit itemToCredit, long RoomID, long CompanyID)
        {
            PullDetailsDAL pullDetailDAL = null;
            IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
            List<PullDetailsDTO> itmDetails = null;
            List<PullDetailToCredit> pullDetailsToCred = null;
            IEnumerable<PullDetailsDTO> lstPullDetails2;

            try
            {
                pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
                itmDetails = new List<PullDetailsDTO>();
                pullDetailsToCred = new List<PullDetailToCredit>();

                if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                {
                    double remainingQty = itemToCredit.PrevPullsToMSCredit.Sum(x => x.Qty);

                    foreach (var itmCr in itemToCredit.PrevPullsToMSCredit)
                    {
                        lstPullDetails2 = null;

                        //if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                        //{
                        if (itemToCredit.ItemTracking == "LOTTRACK")
                        {
                            lstPullDetails2 = pullDetailDAL.GetPullDetailsByItemGuidAndLotNo_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "ms pull", RoomID, CompanyID, itmCr.Lot);
                            remainingQty = itemToCredit.PrevPullsToMSCredit.Where(x => itmCr.Lot == x.Lot).Sum(x => x.Qty);
                        }
                        else if (itemToCredit.ItemTracking == "DATECODETRACK")
                        {
                            lstPullDetails2 = pullDetailDAL.GetPullDetailsByItemGuidAndExpiration_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "ms pull", RoomID, CompanyID, itmCr.ExpireDate);
                            remainingQty = itemToCredit.PrevPullsToMSCredit.Where(x => itmCr.ExpireDate == x.ExpireDate).Sum(x => x.Qty);
                        }
                        else if (itemToCredit.ItemTracking == "SERIALTRACK")
                        {
                            lstPullDetails2 = pullDetailDAL.GetPullDetailsByItemGuidAndSerialNo_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "ms pull", RoomID, CompanyID, itmCr.Serial);
                            remainingQty = itemToCredit.PrevPullsToMSCredit.Where(x => itmCr.Serial == x.Serial).Sum(x => x.Qty);
                        }
                        //}

                        if (lstPullDetails2 != null)
                        {
                            foreach (PullDetailsDTO objPullDetails in lstPullDetails2)
                            {
                                if (remainingQty > 0)
                                {
                                    if (remainingQty <= (objPullDetails.PoolQuantity.GetValueOrDefault(0) - (objPullDetails.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objPullDetails.CreditConsignedQuantity.GetValueOrDefault(0))))
                                    {
                                        if (objPullDetails.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.ConsignedQuantity = remainingQty;
                                        else if (objPullDetails.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.CustomerOwnedQuantity = remainingQty;

                                        remainingQty = 0;
                                    }
                                    else
                                    {
                                        remainingQty -= (objPullDetails.PoolQuantity.GetValueOrDefault(0) - (objPullDetails.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objPullDetails.CreditConsignedQuantity.GetValueOrDefault(0)));

                                        if (objPullDetails.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.ConsignedQuantity = (objPullDetails.PoolQuantity.GetValueOrDefault(0) - objPullDetails.CreditConsignedQuantity.GetValueOrDefault(0));
                                        else if (objPullDetails.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                            objPullDetails.CustomerOwnedQuantity = (objPullDetails.PoolQuantity.GetValueOrDefault(0) - objPullDetails.CreditCustomerOwnedQuantity.GetValueOrDefault(0));
                                    }

                                    if (!string.IsNullOrEmpty(itemToCredit.Bin))
                                        objPullDetails.CreditBinName = itemToCredit.Bin;
                                    else
                                        objPullDetails.CreditBinName = objPullDetails.BinName;

                                    if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                                        objPullDetails.CreditProjectName = itemToCredit.ProjectName;
                                    else
                                        objPullDetails.CreditProjectName = objPullDetails.ProjectSpendName;

                                    objPullDetails.CreditUDF1 = itemToCredit.UDF1;
                                    objPullDetails.CreditUDF2 = itemToCredit.UDF2;
                                    objPullDetails.CreditUDF3 = itemToCredit.UDF3;
                                    objPullDetails.CreditUDF4 = itemToCredit.UDF4;
                                    objPullDetails.CreditUDF5 = itemToCredit.UDF5;
                                    objPullDetails.WorkOrderGUID = itemToCredit.WOGuid;
                                    objPullDetails.PullOrderNumber = itemToCredit.PullOrderNumber;
                                    objPullDetails.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                                    itmDetails.Add(objPullDetails);
                                }
                            }
                        }
                    }
                }
                else if (itemToCredit.ItemTracking != "QUICKLIST")
                {
                    double remainingQty = itemToCredit.PrevPullsToMSCredit.Sum(x => x.Qty);
                    lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryFull(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "ms pull", RoomID, CompanyID);

                    foreach (var item in lstPullDetailDTO)
                    {
                        if (remainingQty > 0)
                        {
                            if (remainingQty <= (item.PoolQuantity.GetValueOrDefault(0) - (item.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + item.CreditConsignedQuantity.GetValueOrDefault(0))))
                            {
                                if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                    item.ConsignedQuantity = remainingQty;
                                else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                    item.CustomerOwnedQuantity = remainingQty;

                                remainingQty = 0;
                            }
                            else
                            {
                                remainingQty -= (item.PoolQuantity.GetValueOrDefault(0) - (item.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + item.CreditConsignedQuantity.GetValueOrDefault(0)));

                                if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                    item.ConsignedQuantity = (item.PoolQuantity.GetValueOrDefault(0) - item.CreditConsignedQuantity.GetValueOrDefault(0));
                                else if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                    item.CustomerOwnedQuantity = (item.PoolQuantity.GetValueOrDefault(0) - item.CreditCustomerOwnedQuantity.GetValueOrDefault(0));
                            }

                            if (!string.IsNullOrEmpty(itemToCredit.Bin))
                                item.CreditBinName = itemToCredit.Bin;
                            else
                                item.CreditBinName = item.BinName;

                            if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                                item.CreditProjectName = itemToCredit.ProjectName;
                            else
                                item.CreditProjectName = item.ProjectSpendName;

                            item.CreditUDF1 = itemToCredit.UDF1;
                            item.CreditUDF2 = itemToCredit.UDF2;
                            item.CreditUDF3 = itemToCredit.UDF3;
                            item.CreditUDF4 = itemToCredit.UDF4;
                            item.CreditUDF5 = itemToCredit.UDF5;
                            item.WorkOrderGUID = itemToCredit.WOGuid;
                            item.PullOrderNumber = itemToCredit.PullOrderNumber;
                            item.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                            itmDetails.Add(item);
                        }

                        if (string.IsNullOrEmpty(itemToCredit.ItemTracking) && itmDetails.Sum(x => (x.PoolQuantity.GetValueOrDefault(0) - (x.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + x.CreditConsignedQuantity.GetValueOrDefault(0)))) >= itemToCredit.Quantity)
                            break;
                    }
                }

                return itmDetails;
            }
            finally
            {

            }
        }

        /// <summary>
        /// GetPreviousPulls
        /// </summary>
        /// <param name="itemToCredit"></param>
        /// <returns></returns>
        public List<PullDetailToMSCredit> GetPrevLoadMSPull(ItemInfoToMSCredit itemToCredit, long RoomID, long CompanyID,long EnterpriseId, string CultureCode)
        {
            PullDetailsDAL pullDetailDAL = null;
            IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
            List<PullDetailsDTO> itmDetails = null;
            List<PullDetailToMSCredit> pullDetailsToCred = null;

            try
            {
                pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
                itmDetails = new List<PullDetailsDTO>();
                pullDetailsToCred = new List<PullDetailToMSCredit>();
                lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryPlain(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "ms pull", RoomID, CompanyID);
                double remainingQty = itemToCredit.Quantity;

                if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                {
                    double totalprevPullQty = lstPullDetailDTO.Sum(x => x.PoolQuantity.GetValueOrDefault(0) - (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
                    itemToCredit.PrevPullQty = totalprevPullQty;
                    //  RoomDTO objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDPlain(RoomID);
                    string columnList = "ID,RoomName,IsIgnoreCreditRule";
                    RoomDTO objRoomDTO = new CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");


                    if (!objRoomDTO.IsIgnoreCreditRule)
                    {
                        if (totalprevPullQty < itemToCredit.Quantity)
                        {
                            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                            string msgCreditQtyGreaterThanPreviousPullQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditQtyGreaterThanPreviousPullQty", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                            itemToCredit.IsModelShow = true;
                            itemToCredit.ErrorMessage = string.Format(msgCreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity) + " (" + totalprevPullQty.ToString() + ")."; 
                            remainingQty = totalprevPullQty;
                        }
                    }

                    if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                    {
                        if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                        {
                            totalprevPullQty = lstPullDetailDTO.Sum(x => x.PoolQuantity.GetValueOrDefault(0) - (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
                            itemToCredit.PrevPullQty = totalprevPullQty;

                            if (!objRoomDTO.IsIgnoreCreditRule)
                            {
                                if (totalprevPullQty < itemToCredit.Quantity)
                                {
                                    var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                                    string msgCreditQtyGreaterThanPreviousPullQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditQtyGreaterThanPreviousPullQty", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                                    itemToCredit.IsModelShow = true;
                                    itemToCredit.ErrorMessage = string.Format(msgCreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity) + " (" + totalprevPullQty.ToString() + ")."; 
                                    remainingQty = totalprevPullQty;
                                }
                            }

                            if (itemToCredit.ItemTracking == "SERIALTRACK")
                            {
                                itemToCredit.IsModelShow = true;

                                foreach (var item in lstPullDetailDTO)
                                {
                                    PullDetailToMSCredit pldtl = new PullDetailToMSCredit()
                                    {
                                        Serial = item.SerialNumber,
                                        ExpireDate = item.Expiration,
                                        Lot = string.Empty,
                                        Qty = item.PoolQuantity.GetValueOrDefault(0)
                                    };

                                    pullDetailsToCred.Add(pldtl);
                                }
                            }
                            else if (itemToCredit.ItemTracking == "LOTTRACK" || itemToCredit.ItemTracking == "DATECODETRACK")
                            {
                                List<PullDetailToMSCredit> lst = (from x in lstPullDetailDTO
                                                                  group x by new { x.LotNumber, x.ItemGUID, x.Expiration } into grp
                                                                  select new PullDetailToMSCredit
                                                                  {
                                                                      Lot = grp.Key.LotNumber,
                                                                      ExpireDate = grp.Key.Expiration,
                                                                      Qty = grp.Sum(p => p.PoolQuantity.GetValueOrDefault(0) - (p.CreditConsignedQuantity.GetValueOrDefault(0) + p.CreditCustomerOwnedQuantity.GetValueOrDefault(0))),
                                                                  }).ToList();
                                if (lst != null && lst.Count > 1)
                                {
                                    foreach (var item in lst)
                                    {
                                        pullDetailsToCred.Add(item);
                                    }
                                }
                                else
                                {
                                    foreach (var item in lst)
                                    {
                                        if (remainingQty <= item.Qty)
                                            item.Qty = remainingQty;
                                        else
                                            remainingQty -= item.Qty;

                                        pullDetailsToCred.Add(item);
                                    }
                                }

                                if (pullDetailsToCred.Count > 1)
                                    itemToCredit.IsModelShow = true;
                            }
                        }
                    }
                    else
                    {
                        PullDetailToMSCredit pldtl = new PullDetailToMSCredit()
                        {
                            Serial = string.Empty,
                            ExpireDate = string.Empty,
                            Lot = string.Empty,
                            Qty = remainingQty
                        };

                        pullDetailsToCred.Add(pldtl);
                    }
                }

                return pullDetailsToCred;
            }
            finally
            {
                pullDetailDAL = null;
                lstPullDetailDTO = null;
            }
        }

        /// <summary>
        /// Convert Pull Detailto ItemLocationDetail
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itm"></param>
        /// <param name="BinID"></param>
        /// <returns></returns>
        public ItemLocationDetailsDTO ConvertPullDetailtoItemLocationDetail(PullDetailsDTO item, long RoomID, long CompanyID, long UserID, string RoomDateFormat)
        {
            BinMasterDAL binDAL = null;
            ProjectMasterDAL pmDAL = null;
            DateTime? ExpirationDate = null;
            DateTime? ReceivedDate = null;
            string Received = DateTime.UtcNow.ToString("MM/dd/yyyy"), Expiration = DateTime.UtcNow.ToString("MM/dd/yyyy");
            long? BinID = 0;
            Guid? ProjectGuid = null;

            if (!string.IsNullOrEmpty(item.CreditBinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.CreditBinName, UserID, RoomID, CompanyID);
                item.BinName = item.CreditBinName;
            }
            else if (item.BinID > 0)
            {
                BinID = item.BinID;
            }

            if (!string.IsNullOrEmpty(item.BinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, UserID, RoomID, CompanyID);
            }

            if (!string.IsNullOrEmpty(item.CreditProjectName))
            {
                pmDAL = new ProjectMasterDAL(base.DataBaseName);
                ProjectMasterDTO objDTO = pmDAL.GetProjectByName(item.CreditProjectName, RoomID, CompanyID, null);

                if (objDTO != null)
                {
                    ProjectGuid = objDTO.GUID;
                    bool isInsert = false;
                    ProjectSpendItemsDAL objSPI = new ProjectSpendItemsDAL(base.DataBaseName);
                    var tmpsupplierIds = new List<long>();
                    //var projectSpendLI = objSPI.GetAllRecords(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds);
                    var projectSpendLI = objSPI.GetProjectSpendItem(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds, string.Empty);

                    if (projectSpendLI != null && projectSpendLI.Count() > 0)
                    {
                        var lineItem = projectSpendLI.Where(t => (t.ItemNumber ?? string.Empty).Trim().ToLower() == item.ItemNumber.Trim().ToLower());
                        if (lineItem != null && lineItem.Count() > 0)
                        {
                            isInsert = false;
                        }
                        else
                        {
                            isInsert = true;
                        }
                    }
                    else
                    {
                        isInsert = true;
                    }
                    if (isInsert)
                    {
                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                        projectSpendItem.QuantityLimit = null;
                        projectSpendItem.QuantityUsed = null;
                        projectSpendItem.DollarLimitAmount = null;
                        projectSpendItem.DollarUsedAmount = null;
                        projectSpendItem.ItemGUID = item.ItemGUID;
                        projectSpendItem.CreatedBy = UserID;
                        projectSpendItem.LastUpdatedBy = UserID;
                        projectSpendItem.Room = RoomID;
                        projectSpendItem.CompanyID = CompanyID;
                        projectSpendItem.ItemNumber = item.ItemNumber;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.ProjectSpendName = item.CreditProjectName;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.ProjectGUID = objDTO.GUID;
                        objSPI.Insert(projectSpendItem);
                    }
                }
                else
                {
                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(base.DataBaseName);
                    ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                    objProjectSpendDTO.ProjectSpendName = item.CreditProjectName;
                    objProjectSpendDTO.AddedFrom = "Web";
                    objProjectSpendDTO.EditedFrom = "Web";
                    objProjectSpendDTO.CompanyID = CompanyID;
                    objProjectSpendDTO.Room = RoomID;
                    objProjectSpendDTO.DollarLimitAmount = 0;
                    objProjectSpendDTO.Description = string.Empty;
                    objProjectSpendDTO.DollarUsedAmount = null;
                    objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.CreatedBy = UserID;
                    objProjectSpendDTO.LastUpdatedBy = UserID;
                    objProjectSpendDTO.UDF1 = string.Empty;
                    objProjectSpendDTO.UDF2 = string.Empty;
                    objProjectSpendDTO.UDF3 = string.Empty;
                    objProjectSpendDTO.UDF4 = string.Empty;
                    objProjectSpendDTO.UDF5 = string.Empty;
                    objProjectSpendDTO.GUID = Guid.NewGuid();

                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                    objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID.GetValueOrDefault(Guid.Empty));

                    List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                    ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                    projectSpendItem.QuantityLimit = null;
                    projectSpendItem.QuantityUsed = null;
                    projectSpendItem.DollarLimitAmount = null;
                    projectSpendItem.DollarUsedAmount = null;
                    projectSpendItem.ItemGUID = item.ItemGUID;
                    projectSpendItem.CreatedBy = UserID;
                    projectSpendItem.LastUpdatedBy = UserID;
                    projectSpendItem.Room = RoomID;
                    projectSpendItem.CompanyID = CompanyID;

                    if (objItemmasterDTO != null)
                        projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;

                    projectSpendItem.IsArchived = false;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.ProjectSpendName = item.CreditProjectName;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.IsArchived = false;
                    projectSpendItemList.Add(projectSpendItem);
                    objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;
                    objProjectSpendDTO.IsDeleted = false;
                    objProjectSpendDTO.IsArchived = false;
                    objProjectSpendDAL.Insert(objProjectSpendDTO);
                    ProjectGuid = objProjectSpendDTO.GUID;
                }
            }
            else
            {
                ProjectGuid = item.ProjectSpendGUID;
            }

            if (!string.IsNullOrEmpty(item.Expiration))
            {
                var moveMaterialDAL = new MoveMaterialDAL(base.DataBaseName);

                if (item.ItemLocationDetailGUID.HasValue)
                {
                    var expdt = new ItemLocationDetailsDAL(base.DataBaseName).GetItemLocationDetailsByLocationGuid(item.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                    if (expdt != null && expdt.ExpirationDate.HasValue)
                    {
                        ExpirationDate = expdt.ExpirationDate;
                    }
                }

                if (!(ExpirationDate.HasValue && ExpirationDate != null))
                {
                    ExpirationDate = moveMaterialDAL.GetExpireDateFromString(item.Expiration, RoomDateFormat);
                }

                //CultureInfo CurrentCult = new CultureInfo("en-US");
                //eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);

                //if (RegionInfo != null)
                //    CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                //DateTime DTExp = DateTime.MinValue;
                //DateTime.TryParseExact(item.Expiration, RoomDateFormat, CurrentCult, DateTimeStyles.None, out DTExp);

                //if (DTExp != DateTime.MinValue)
                //{
                //    ExpirationDate = DTExp;
                //}               
            }

            if (item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue)
            {
                if (!string.IsNullOrEmpty(item.Received))
                {
                    CultureInfo CurrentCult = new CultureInfo("en-US");
                    eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    if (RegionInfo != null)
                        CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                    DateTime DTRec = DateTime.MinValue;
                    DateTime.TryParseExact(item.Received, RoomDateFormat, CurrentCult, DateTimeStyles.None, out DTRec);
                    if (DTRec != DateTime.MinValue)
                    {
                        ReceivedDate = DTRec;
                    }
                }
            }
            else
            {
                ReceivedDate = item.ReceivedDate;
            }

            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItemDTO = itemDAL.GetItemWithoutJoins(null, item.ItemGUID);

            if (ReceivedDate == null || ReceivedDate == DateTime.MinValue)
            {
                ReceivedDate = DateTime.UtcNow;
                item.Received = ReceivedDate.Value.ToString("MM/dd/yyyy");
                Received = ReceivedDate.Value.ToString("MM/dd/yyyy");
            }

            if (objItemDTO.DateCodeTracking)
            {
                if (ExpirationDate == null || ExpirationDate == DateTime.MinValue)
                {
                    ExpirationDate = DateTime.UtcNow.Date;
                    item.Expiration = ExpirationDate.Value.ToString("MM/dd/yyyy");
                    Expiration = ExpirationDate.Value.ToString("MM/dd/yyyy");
                }
            }
            else
            {
                Expiration = "";
            }
            int costUOMValue = 1;

            if (objItemDTO != null)
            {
                CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemDTO.CostUOMID.GetValueOrDefault(0));

                if (oCostUOMMasterDTO != null)
                {
                    costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                    if (costUOMValue == 0)
                        costUOMValue = 1;
                }
            }

            ItemLocationDetailsDTO itml = new ItemLocationDetailsDTO()
            {
                BinID = item.BinID,
                BinNumber = item.BinName,
                ConsignedQuantity = item.ConsignedQuantity,
                CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                ItemGUID = item.ItemGUID,
                Cost = item.ItemCost * costUOMValue,
                ProjectSpentGUID = ProjectGuid == null ? item.ProjectSpendGUID : ProjectGuid,
                SerialNumber = item.SerialNumber,
                LotNumber = item.LotNumber,
                Expiration = (item.Expiration ?? string.Empty) == string.Empty ? Expiration : item.Expiration,
                Received = (item.Received ?? string.Empty) == string.Empty ? Received : item.Received,
                ReceivedDate = ReceivedDate,
                ExpirationDate = ExpirationDate,
                UDF1 = item.CreditUDF1,
                UDF2 = item.CreditUDF2,
                UDF3 = item.CreditUDF3,
                UDF4 = item.CreditUDF4,
                UDF5 = item.CreditUDF5,
                WorkOrderGUID = item.WorkOrderGUID,
                CompanyID = CompanyID,
                Room = RoomID,
                Created = DateTimeUtility.DateTimeNow,
                IsOnlyFromUI = true,
                IsPDAEdit = false,
                IsWebEdit = true,
                EditedFrom = item.AddedFrom,
                AddedFrom = item.EditedFrom,
                InsertedFrom = "Pull Credit",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                PullOrderNumber = item.PullOrderNumber,
                SellPrice = item.ItemPrice * costUOMValue,
                SerialNumberTracking = objItemDTO != null ? objItemDTO.SerialNumberTracking : false,
                LotNumberTracking = objItemDTO != null ? objItemDTO.LotNumberTracking : false,
                DateCodeTracking = objItemDTO != null ? objItemDTO.DateCodeTracking : false,
                PullGUID = item.NEW_PullGUID,
                PullGUIDForCreditHistory = item.PULLGUID,
                PullDetailGUIDForCreditHistory = item.GUID,
                SupplierAccountGuid = item.SupplierAccountGuid
            };

            return itml;
        }

        public MaterialStagingPullDetailDTO ConvertPullDetailtoMaterialStaginPullDetail(PullDetailsDTO item, long RoomID, long CompanyID, long UserID, string RoomDateFormat)
        {
            BinMasterDAL binDAL = null;
            ProjectMasterDAL pmDAL = null;
            DateTime? ExpirationDate = null;
            DateTime? ReceivedDate = null;
            Guid? ProjectGuid = null;
            string Received = DateTime.UtcNow.ToString("MM/dd/yyyy"), Expiration = DateTime.UtcNow.ToString("MM/dd/yyyy");
            long? BinID = 0;

            if (!string.IsNullOrEmpty(item.CreditBinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.CreditBinName, UserID, RoomID, CompanyID, true);
                item.BinName = item.CreditBinName;

            }
            else if (item.BinID > 0)
            {
                BinID = item.BinID;
            }

            if (!string.IsNullOrEmpty(item.BinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, UserID, RoomID, CompanyID, true);
            }

            if (!string.IsNullOrEmpty(item.CreditProjectName))
            {
                pmDAL = new ProjectMasterDAL(base.DataBaseName);
                ProjectMasterDTO objDTO = pmDAL.GetProjectByName(item.CreditProjectName, RoomID, CompanyID, null);

                if (objDTO != null)
                {
                    ProjectGuid = objDTO.GUID;
                    bool isInsert = false;
                    ProjectSpendItemsDAL objSPI = new ProjectSpendItemsDAL(base.DataBaseName);
                    var tmpsupplierIds = new List<long>();
                    //var projectSpendLI = objSPI.GetAllRecords(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds);
                    var projectSpendLI = objSPI.GetProjectSpendItem(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds, string.Empty);

                    if (projectSpendLI != null && projectSpendLI.Count() > 0)
                    {
                        var lineItem = projectSpendLI.Where(t => (t.ItemNumber ?? string.Empty).Trim().ToLower() == item.ItemNumber.Trim().ToLower());
                        if (lineItem != null && lineItem.Count() > 0)
                        {
                            isInsert = false;
                        }
                        else
                        {
                            isInsert = true;
                        }
                    }
                    else
                    {
                        isInsert = true;
                    }
                    if (isInsert)
                    {
                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                        projectSpendItem.QuantityLimit = null;
                        projectSpendItem.QuantityUsed = null;
                        projectSpendItem.DollarLimitAmount = null;
                        projectSpendItem.DollarUsedAmount = null;
                        projectSpendItem.ItemGUID = item.ItemGUID;
                        projectSpendItem.CreatedBy = UserID;
                        projectSpendItem.LastUpdatedBy = UserID;
                        projectSpendItem.Room = RoomID;
                        projectSpendItem.CompanyID = CompanyID;
                        projectSpendItem.ItemNumber = item.ItemNumber;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.ProjectSpendName = item.CreditProjectName;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.ProjectGUID = objDTO.GUID;
                        objSPI.Insert(projectSpendItem);
                    }
                }
                else
                {
                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(base.DataBaseName);
                    ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                    objProjectSpendDTO.ProjectSpendName = item.CreditProjectName;
                    objProjectSpendDTO.AddedFrom = "Web";
                    objProjectSpendDTO.EditedFrom = "Web";
                    objProjectSpendDTO.CompanyID = CompanyID;
                    objProjectSpendDTO.Room = RoomID;
                    objProjectSpendDTO.DollarLimitAmount = 0;
                    objProjectSpendDTO.Description = string.Empty;
                    objProjectSpendDTO.DollarUsedAmount = null;
                    objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.CreatedBy = UserID;
                    objProjectSpendDTO.LastUpdatedBy = UserID;
                    objProjectSpendDTO.UDF1 = string.Empty;
                    objProjectSpendDTO.UDF2 = string.Empty;
                    objProjectSpendDTO.UDF3 = string.Empty;
                    objProjectSpendDTO.UDF4 = string.Empty;
                    objProjectSpendDTO.UDF5 = string.Empty;
                    objProjectSpendDTO.GUID = Guid.NewGuid();

                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                    objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID.GetValueOrDefault(Guid.Empty));

                    List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                    ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                    projectSpendItem.QuantityLimit = null;
                    projectSpendItem.QuantityUsed = null;
                    projectSpendItem.DollarLimitAmount = null;
                    projectSpendItem.DollarUsedAmount = null;
                    projectSpendItem.ItemGUID = item.ItemGUID;
                    projectSpendItem.CreatedBy = UserID;
                    projectSpendItem.LastUpdatedBy = UserID;
                    projectSpendItem.Room = RoomID;
                    projectSpendItem.CompanyID = CompanyID;

                    if (objItemmasterDTO != null)
                        projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;

                    projectSpendItem.IsArchived = false;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.ProjectSpendName = item.CreditProjectName;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.IsArchived = false;
                    projectSpendItemList.Add(projectSpendItem);
                    objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;
                    objProjectSpendDTO.IsDeleted = false;
                    objProjectSpendDTO.IsArchived = false;
                    objProjectSpendDAL.Insert(objProjectSpendDTO);
                    ProjectGuid = objProjectSpendDTO.GUID;
                }
            }
            else
            {
                ProjectGuid = item.ProjectSpendGUID;
            }

            if (!string.IsNullOrEmpty(item.Expiration))
            {
                CultureInfo CurrentCult = new CultureInfo("en-US");
                eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);

                if (RegionInfo != null)
                    CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                DateTime DTExp = DateTime.MinValue;
                DateTime.TryParseExact(item.Expiration, RoomDateFormat, CurrentCult, System.Globalization.DateTimeStyles.None, out DTExp);

                if (DTExp != DateTime.MinValue)
                {
                    ExpirationDate = DTExp;
                }
            }

            if (!string.IsNullOrEmpty(item.Received))
            {
                CultureInfo CurrentCult = new CultureInfo("en-US");
                eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);

                if (RegionInfo != null)
                    CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                DateTime DTRec = DateTime.MinValue;
                DateTime.TryParseExact(item.Received, RoomDateFormat, CurrentCult, System.Globalization.DateTimeStyles.None, out DTRec);

                if (DTRec != DateTime.MinValue)
                {
                    ReceivedDate = DTRec;
                }
            }

            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItemDTO = itemDAL.GetItemWithoutJoins(null, item.ItemGUID);

            if (ReceivedDate == null || ReceivedDate == DateTime.MinValue)
            {
                ReceivedDate = DateTime.UtcNow.Date;
                item.Received = ReceivedDate.Value.ToString("MM/dd/yyyy");
                Received = ReceivedDate.Value.ToString("MM/dd/yyyy");
            }

            if (objItemDTO.DateCodeTracking)
            {
                if (ExpirationDate == null || ExpirationDate == DateTime.MinValue)
                {
                    ExpirationDate = DateTime.UtcNow.Date;
                    item.Expiration = ExpirationDate.Value.ToString("MM/dd/yyyy");
                    Expiration = ExpirationDate.Value.ToString("MM/dd/yyyy");
                }
            }

            int costUOMValue = 1;

            if (objItemDTO != null)
            {
                CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemDTO.CostUOMID.GetValueOrDefault(0));

                if (oCostUOMMasterDTO != null)
                {
                    costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                    if (costUOMValue == 0)
                        costUOMValue = 1;
                }
            }

            MaterialStagingPullDetailDTO itml = new MaterialStagingPullDetailDTO()
            {
                BinID = item.BinID,
                StagingBinId = BinID ?? 0,
                BinNumber = item.BinName,
                ConsignedQuantity = item.ConsignedQuantity,
                CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                ItemGUID = item.ItemGUID ?? Guid.Empty,
                Cost = item.ItemCost * costUOMValue,
                SellPrice = item.ItemPrice * costUOMValue,
                ProjectSpentGUID = ProjectGuid == null ? item.ProjectSpendGUID : ProjectGuid,
                SerialNumber = item.SerialNumber,
                LotNumber = item.LotNumber,
                Expiration = (item.Expiration ?? string.Empty) == string.Empty ? Expiration : item.Expiration,
                Received = (item.Received ?? string.Empty) == string.Empty ? Received : item.Received,
                ReceivedDate = ReceivedDate,
                ExpirationDate = ExpirationDate,
                UDF1 = item.CreditUDF1,
                UDF2 = item.CreditUDF2,
                UDF3 = item.CreditUDF3,
                UDF4 = item.CreditUDF4,
                UDF5 = item.CreditUDF5,
                WorkOrderGuid = item.WorkOrderGUID,
                CompanyID = CompanyID,
                Room = RoomID,
                Created = DateTimeUtility.DateTimeNow,
                IsOnlyFromUI = true,
                IsPDAEdit = false,
                IsWebEdit = true,
                EditedFrom = item.AddedFrom,
                AddedFrom = item.EditedFrom,
                InsertedFrom = "Pull MS Credit",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                PullOrderNumber = item.PullOrderNumber,
                MaterialStagingGUID = item.MaterialStagingGUID,
                MaterialStagingdtlGUID = item.MaterialStagingdtlGUID,
                PoolQuantity = item.PoolQuantity,
                SerialNumberTracking = objItemDTO != null ? objItemDTO.SerialNumberTracking : false,
                LotNumberTracking = objItemDTO != null ? objItemDTO.LotNumberTracking : false,
                DateCodeTracking = objItemDTO != null ? objItemDTO.DateCodeTracking : false,

                PullGUIDForCreditHistory = item.PULLGUID,
                PullDetailGUIDForCreditHistory = item.GUID,
                SupplierAccountGuid = item.SupplierAccountGuid
            };

            return itml;
        }

        /// <summary>
        /// Update Pull Records For Credit Quantity
        /// </summary>
        /// <param name="pullDetails"></param>
        public void UpdatePullRecordsForCreditQuantity(List<PullDetailsDTO> pullDetails, long RoomID, long CompanyID, long UserID, List<CreditHistory> lstCreditGuids, string WhatWhereAction = "")
        {
            PullDetailsDAL pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
            PullMasterDAL pullDAL = new PullMasterDAL(base.DataBaseName);
            List<Guid> lstPullMasterGuids = new List<Guid>();
            List<PullDetailsDTO> lstPullDetailsFromDB = new List<PullDetailsDTO>();

            foreach (var item in pullDetails.Where(x => x.PULLGUID != null && x.PULLGUID != Guid.Empty).ToList())
            {
                PullDetailsDTO pullDtl = pullDetailDAL.GetPullDetailByGuidNormal(item.GUID);
                pullDtl.CreditCustomerOwnedQuantity = pullDtl.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                pullDtl.CreditConsignedQuantity = pullDtl.CreditConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                pullDtl.ReceivedOn = DateTimeUtility.DateTimeNow;
                pullDtl.Updated = DateTimeUtility.DateTimeNow;
                pullDtl.EditedFrom = item.EditedFrom;
                pullDtl.LastUpdatedBy = UserID;
                pullDetailDAL.Edit(pullDtl);
                lstPullDetailsFromDB.Add(pullDtl);

                if (lstPullMasterGuids.IndexOf(item.PULLGUID.GetValueOrDefault(Guid.Empty)) < 0)
                {
                    lstPullMasterGuids.Add(item.PULLGUID.GetValueOrDefault(Guid.Empty));
                }

                #region Entry into Credit History Table
                if (pullDtl != null && pullDtl.GUID != Guid.Empty)
                {
                    CreditHistory objCreditGuids = new CreditHistory();
                    if (lstCreditGuids != null && lstCreditGuids.Count > 0)
                    {
                        objCreditGuids = lstCreditGuids.Where(x => x.PULLGUID == item.PULLGUID.GetValueOrDefault(Guid.Empty)
                                                               && x.PullDetailGuid == pullDtl.GUID).FirstOrDefault();
                    }

                    PullCreditHistoryDTO PullCreditHistoryDtl = new PullCreditHistoryDTO();
                    PullCreditHistoryDtl.GUID = Guid.NewGuid();
                    PullCreditHistoryDtl.PullDetailGuid = pullDtl.GUID;
                    PullCreditHistoryDtl.PULLGUID = item.PULLGUID.GetValueOrDefault(Guid.Empty);
                    PullCreditHistoryDtl.ItemGUID = item.ItemGUID.GetValueOrDefault(Guid.Empty);
                    PullCreditHistoryDtl.CreditCustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    PullCreditHistoryDtl.CreditConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    PullCreditHistoryDtl.CompanyID = item.CompanyID.GetValueOrDefault(0);
                    PullCreditHistoryDtl.Room = item.Room.GetValueOrDefault(0);
                    PullCreditHistoryDtl.Created = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.Updated = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.Created = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.Updated = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.CreatedBy = UserID;
                    PullCreditHistoryDtl.LastUpdatedBy = UserID;
                    PullCreditHistoryDtl.IsDeleted = (bool)pullDtl.IsDeleted;
                    PullCreditHistoryDtl.IsArchived = (bool)pullDtl.IsArchived;
                    PullCreditHistoryDtl.AddedFrom = item.AddedFrom;
                    PullCreditHistoryDtl.EditedFrom = item.EditedFrom;
                    PullCreditHistoryDtl.WhatWhereAction = (pullDtl.PullCredit.ToLower().Equals("pull") ? "Pull Credit" : "MS Pull Credit");

                    if (objCreditGuids != null)
                    {
                        PullCreditHistoryDtl.CreditGuid = objCreditGuids.CreditGuid;
                        PullCreditHistoryDtl.CreditDetailGuid = objCreditGuids.CreditDetailGuid;
                    }
                    else
                    {
                        PullCreditHistoryDtl.CreditGuid = null;
                        PullCreditHistoryDtl.CreditDetailGuid = null;
                    }
                    pullDetailDAL.InsertCreditHistory(PullCreditHistoryDtl);
                }
                #endregion

            }

            if (lstPullMasterGuids != null && lstPullMasterGuids.Count > 0)
            {
                foreach (Guid item in lstPullMasterGuids)
                {
                    PullMasterViewDTO pullDTO = pullDAL.GetPullByGuidPlain(item);
                    IEnumerable<PullDetailsDTO> lstpldtl = lstPullDetailsFromDB.Where(x => x.PULLGUID == item);

                    if (pullDTO != null && lstpldtl != null && lstpldtl.Count() > 0)
                    {
                        pullDTO.CreditConsignedQuantity = lstpldtl.Sum(x => x.CreditConsignedQuantity.GetValueOrDefault(0));
                        pullDTO.CreditCustomerOwnedQuantity = lstpldtl.Sum(x => x.CreditCustomerOwnedQuantity.GetValueOrDefault(0));
                        pullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        pullDTO.Updated = DateTimeUtility.DateTimeNow;
                        pullDTO.EditedFrom = pullDetails[0].EditedFrom;
                        pullDTO.LastUpdatedBy = UserID;

                        if (!string.IsNullOrEmpty(WhatWhereAction))
                            pullDTO.WhatWhereAction = WhatWhereAction;
                        else
                            pullDTO.WhatWhereAction = "Credit Pull";

                        pullDAL.Edit(pullDTO);
                    }
                }

                // Update sum of all Quantity from PullDetails into Pull Master table
                foreach (var item in pullDetails.Select(x => x.PULLGUID).Distinct().Select(x => x.GetValueOrDefault(Guid.Empty)))
                {
                    IEnumerable<PullDetailsDTO> pullSerialDtl = pullDetailDAL.GetPullDetailsByPullGuidPlain(item, RoomID, CompanyID);

                    if (pullSerialDtl.Count() > 0)
                    {
                        using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            PullMaster pullSerialDTO = context.PullMasters.FirstOrDefault(t => t.GUID == item);

                            if (pullSerialDTO != null && pullSerialDTO.ID > 0)
                            {
                                pullSerialDTO.CreditConsignedQuantity = pullSerialDtl.Sum(x => x.CreditConsignedQuantity).GetValueOrDefault(0);
                                pullSerialDTO.CreditCustomerOwnedQuantity = pullSerialDtl.Sum(x => x.CreditCustomerOwnedQuantity).GetValueOrDefault(0);
                            }
                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        public void UpdatePullRecordsForMSCreditQuantity(List<PullDetailsDTO> pullDetails, long RoomID, long CompanyID, long UserID, List<CreditHistory> lstCreditGuids, string WhatWhereAction = "")
        {
            PullDetailsDAL pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
            PullMasterDAL pullDAL = new PullMasterDAL(base.DataBaseName);
            List<Guid> lstPullMasterGuids = new List<Guid>();
            List<PullDetailsDTO> lstPullDetailsFromDB = new List<PullDetailsDTO>();

            foreach (var item in pullDetails.Where(x => x.PULLGUID != null && x.PULLGUID != Guid.Empty).ToList())
            {
                PullDetailsDTO pullDtl = pullDetailDAL.GetPullDetailByGuidNormal(item.GUID);
                pullDtl.CreditCustomerOwnedQuantity = pullDtl.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                pullDtl.CreditConsignedQuantity = pullDtl.CreditConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                pullDtl.ReceivedOn = DateTimeUtility.DateTimeNow;
                pullDtl.Updated = DateTimeUtility.DateTimeNow;
                pullDtl.EditedFrom = item.EditedFrom;
                pullDtl.LastUpdatedBy = UserID;
                pullDtl.ItemLocationDetailGUID = null;
                pullDetailDAL.Edit(pullDtl);
                lstPullDetailsFromDB.Add(pullDtl);

                if (lstPullMasterGuids.IndexOf(item.PULLGUID.GetValueOrDefault(Guid.Empty)) < 0)
                {
                    lstPullMasterGuids.Add(item.PULLGUID.GetValueOrDefault(Guid.Empty));
                }

                #region Entry into Credit History Table

                if (pullDtl != null && pullDtl.GUID != Guid.Empty)
                {
                    CreditHistory objCreditGuids = new CreditHistory();
                    if (lstCreditGuids != null && lstCreditGuids.Count > 0)
                    {
                        objCreditGuids = lstCreditGuids.Where(x => x.PULLGUID == item.PULLGUID.GetValueOrDefault(Guid.Empty)
                                                               && x.PullDetailGuid == pullDtl.GUID).FirstOrDefault();
                    }

                    PullCreditHistoryDTO PullCreditHistoryDtl = new PullCreditHistoryDTO();
                    PullCreditHistoryDtl.GUID = Guid.NewGuid();
                    PullCreditHistoryDtl.PullDetailGuid = pullDtl.GUID;
                    PullCreditHistoryDtl.PULLGUID = item.PULLGUID.GetValueOrDefault(Guid.Empty);
                    PullCreditHistoryDtl.ItemGUID = item.ItemGUID.GetValueOrDefault(Guid.Empty);
                    PullCreditHistoryDtl.CreditCustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    PullCreditHistoryDtl.CreditConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    PullCreditHistoryDtl.CompanyID = item.CompanyID.GetValueOrDefault(0);
                    PullCreditHistoryDtl.Room = item.Room.GetValueOrDefault(0);
                    PullCreditHistoryDtl.Created = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.Updated = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.Created = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.Updated = DateTimeUtility.DateTimeNow;
                    PullCreditHistoryDtl.CreatedBy = UserID;
                    PullCreditHistoryDtl.LastUpdatedBy = UserID;
                    PullCreditHistoryDtl.IsDeleted = (bool)pullDtl.IsDeleted;
                    PullCreditHistoryDtl.IsArchived = (bool)pullDtl.IsArchived;
                    PullCreditHistoryDtl.AddedFrom = item.AddedFrom;
                    PullCreditHistoryDtl.EditedFrom = item.EditedFrom;
                    PullCreditHistoryDtl.WhatWhereAction = (pullDtl.PullCredit.ToLower().Equals("pull") ? "Pull Credit" : "MS Pull Credit");

                    if (objCreditGuids != null)
                    {
                        PullCreditHistoryDtl.CreditGuid = objCreditGuids.CreditGuid;
                        PullCreditHistoryDtl.CreditDetailGuid = objCreditGuids.CreditDetailGuid;
                    }
                    else
                    {
                        PullCreditHistoryDtl.CreditGuid = null;
                        PullCreditHistoryDtl.CreditDetailGuid = null;
                    }

                    pullDetailDAL.InsertCreditHistory(PullCreditHistoryDtl);
                }

                #endregion
            }

            if (lstPullMasterGuids != null && lstPullMasterGuids.Count > 0)
            {
                foreach (Guid item in lstPullMasterGuids)
                {
                    PullMasterViewDTO pullDTO = pullDAL.GetPullByGuidPlain(item);
                    IEnumerable<PullDetailsDTO> lstpldtl = lstPullDetailsFromDB.Where(x => x.PULLGUID == item);

                    if (pullDTO != null && lstpldtl != null && lstpldtl.Count() > 0)
                    {
                        pullDTO.CreditConsignedQuantity = lstpldtl.Sum(x => x.CreditConsignedQuantity.GetValueOrDefault(0));
                        pullDTO.CreditCustomerOwnedQuantity = lstpldtl.Sum(x => x.CreditCustomerOwnedQuantity.GetValueOrDefault(0));
                        pullDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        pullDTO.Updated = DateTimeUtility.DateTimeNow;
                        pullDTO.EditedFrom = pullDetails[0].EditedFrom;
                        pullDTO.LastUpdatedBy = UserID;
                        pullDTO.WhatWhereAction = "MS Credit Pull";

                        pullDAL.Edit(pullDTO);
                    }
                }

                // Update sum of all Quantity from PullDetails into Pull Master table 
                foreach (var item in pullDetails.Select(x => x.PULLGUID).Distinct().Select(x => x.GetValueOrDefault(Guid.Empty)))
                {
                    IEnumerable<PullDetailsDTO> pullSerialDtl = pullDetailDAL.GetPullDetailsByPullGuidPlain(item, RoomID, CompanyID);

                    if (pullSerialDtl.Count() > 0)
                    {
                        using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            PullMaster pullSerialDTO = context.PullMasters.FirstOrDefault(t => t.GUID == item);

                            if (pullSerialDTO != null && pullSerialDTO.ID > 0)
                            {
                                pullSerialDTO.CreditConsignedQuantity = pullSerialDtl.Sum(x => x.CreditConsignedQuantity).GetValueOrDefault(0);
                                pullSerialDTO.CreditCustomerOwnedQuantity = pullSerialDtl.Sum(x => x.CreditCustomerOwnedQuantity).GetValueOrDefault(0);
                            }
                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        public bool UpdateUDF(string UDFFieldName, long RoomId, long CompanyId, bool IsEncrypt)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //List<PullMaster> objPullList = context.PullMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId).ToList();
                //List<RequisitionDetail> objReqDetailsList = context.RequisitionDetails.Where(t => t.Room == RoomId && t.CompanyID == CompanyId).ToList();
                List<PullMaster> objPullList = new List<PullMaster>();
                List<RequisitionDetail> objReqDetailsList = new List<RequisitionDetail>();
                CommonDAL objCommon = new CommonDAL(base.DataBaseName);

                switch (UDFFieldName.ToLower().Trim())
                {
                    case "udf1":
                        objPullList = context.PullMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.UDF1)).ToList();
                        objReqDetailsList = context.RequisitionDetails.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.PullUDF1)).ToList();
                        break;
                    case "udf2":
                        objPullList = context.PullMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.UDF2)).ToList();
                        objReqDetailsList = context.RequisitionDetails.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.PullUDF2)).ToList();
                        break;
                    case "udf3":
                        objPullList = context.PullMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.UDF3)).ToList();
                        objReqDetailsList = context.RequisitionDetails.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.PullUDF3)).ToList();
                        break;
                    case "udf4":
                        objPullList = context.PullMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.UDF4)).ToList();
                        objReqDetailsList = context.RequisitionDetails.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.PullUDF4)).ToList();
                        break;
                    case "udf5":
                        objPullList = context.PullMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.UDF5)).ToList();
                        objReqDetailsList = context.RequisitionDetails.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && !string.IsNullOrEmpty(t.PullUDF5)).ToList();
                        break;
                }

                switch (UDFFieldName.ToLower().Trim())
                {
                    case "udf1":
                        foreach (PullMaster p in objPullList)
                        {
                            if (p.UDF1 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(p.UDF1)))
                                {
                                    p.UDF1 = objCommon.GetEncryptValue(p.UDF1);
                                }
                                else
                                {
                                    p.UDF1 = objCommon.GetDecryptValue(p.UDF1);
                                }
                            }
                        }
                        foreach (RequisitionDetail rd in objReqDetailsList)
                        {
                            if (rd.PullUDF1 != null && (!string.IsNullOrWhiteSpace(rd.PullUDF1)))
                            {
                                if (IsEncrypt)
                                {
                                    rd.PullUDF1 = objCommon.GetEncryptValue(rd.PullUDF1);
                                }
                                else
                                {
                                    rd.PullUDF1 = objCommon.GetDecryptValue(rd.PullUDF1);
                                }
                            }
                        }

                        break;
                    case "udf2":
                        foreach (PullMaster p in objPullList)
                        {
                            if (p.UDF2 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(p.UDF2)))
                                {
                                    p.UDF2 = objCommon.GetEncryptValue(p.UDF2);
                                }
                                else
                                {
                                    p.UDF2 = objCommon.GetDecryptValue(p.UDF2);
                                }
                            }
                        }
                        foreach (RequisitionDetail rd in objReqDetailsList)
                        {

                            if (rd.PullUDF2 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(rd.PullUDF2)))
                                {
                                    rd.PullUDF2 = objCommon.GetEncryptValue(rd.PullUDF2);
                                }
                                else
                                {
                                    rd.PullUDF2 = objCommon.GetDecryptValue(rd.PullUDF2);
                                }
                            }
                        }
                        break;
                    case "udf3":
                        foreach (PullMaster p in objPullList)
                        {
                            if (p.UDF3 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(p.UDF3)))
                                {
                                    p.UDF3 = objCommon.GetEncryptValue(p.UDF3);
                                }
                                else
                                {
                                    p.UDF3 = objCommon.GetDecryptValue(p.UDF3);
                                }
                            }
                        }
                        foreach (RequisitionDetail rd in objReqDetailsList)
                        {
                            if (rd.PullUDF3 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(rd.PullUDF3)))
                                {
                                    rd.PullUDF3 = objCommon.GetEncryptValue(rd.PullUDF3);
                                }
                                else
                                {
                                    rd.PullUDF3 = objCommon.GetDecryptValue(rd.PullUDF3);
                                }
                            }
                        }
                        break;
                    case "udf4":
                        foreach (PullMaster p in objPullList)
                        {
                            if (p.UDF4 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(p.UDF4)))
                                {
                                    p.UDF4 = objCommon.GetEncryptValue(p.UDF4);
                                }
                                else
                                {
                                    p.UDF4 = objCommon.GetDecryptValue(p.UDF4);
                                }
                            }
                        }
                        foreach (RequisitionDetail rd in objReqDetailsList)
                        {
                            if (rd.PullUDF4 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(rd.PullUDF4)))
                                {
                                    rd.PullUDF4 = objCommon.GetEncryptValue(rd.PullUDF4);
                                }
                                else
                                {
                                    rd.PullUDF4 = objCommon.GetDecryptValue(rd.PullUDF4);
                                }
                            }
                        }
                        break;
                    case "udf5":
                        foreach (PullMaster p in objPullList)
                        {
                            if (p.UDF5 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(p.UDF5)))
                                {
                                    p.UDF5 = objCommon.GetEncryptValue(p.UDF5);
                                }
                                else
                                {
                                    p.UDF5 = objCommon.GetDecryptValue(p.UDF5);
                                }
                            }
                        }
                        foreach (RequisitionDetail rd in objReqDetailsList)
                        {
                            if (rd.PullUDF5 != null)
                            {
                                if (IsEncrypt && (!string.IsNullOrWhiteSpace(rd.PullUDF5)))
                                {
                                    rd.PullUDF5 = objCommon.GetEncryptValue(rd.PullUDF5);
                                }
                                else
                                {
                                    rd.PullUDF5 = objCommon.GetDecryptValue(rd.PullUDF5);
                                }
                            }
                        }
                        break;
                }
                context.SaveChanges();
            }
            return true;
        }

        public List<PullImport> PullImportExport(long RoomID, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<PullImport>("exec [PullExport] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<PullMasterViewDTO> GetAllPullsForServiceNormal(string CreatedDateFrom, string CreatedDateTo, long RoomId, long CompanyId, Guid WoGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? (object)DBNull.Value), new SqlParameter("@CreatedDateTo", CreatedDateTo ?? (object)DBNull.Value), new SqlParameter("@WoGUID", WoGUID) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetAllPullsForServiceNormal] @RoomID,@CompanyID,@CreatedDateFrom,@CreatedDateTo,@WoGUID", params1).ToList();
            }
        }

        public void UpdateUDFFromPullHistory(long RoomID, long CompanyID, long UserID, string PullGUID, string UDFColumnName, string UDFValue, string UDFControlType, long UDFID, out string _returnmessage, out string _returnstatus, long EnterpriseId, string CultureCode)
        {
            UDFDAL objUDFDAL = new UDFDAL(base.DataBaseName);            
            var messageResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", CultureCode, EnterpriseId, CompanyID);
            _returnmessage = ResourceRead.GetResourceValueByKeyAndFullFilePath("SaveMessage", messageResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResMessage", CultureCode);
            _returnstatus = "OK";
            Guid _pullGUID = new Guid(PullGUID);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullMaster obj = context.PullMasters.FirstOrDefault(t => t.GUID == _pullGUID);
                bool isSameAsBefore = true;

                if (obj != null)
                {
                    obj.LastUpdatedBy = UserID;
                    obj.Updated = DateTimeUtility.DateTimeNow;

                    if (UDFColumnName == "UDF1")
                    {
                        if ((obj.UDF1 ?? string.Empty).Trim().ToLower() != (UDFValue ?? string.Empty).Trim().ToLower())
                        {
                            isSameAsBefore = false;
                            obj.UDF1 = UDFValue;
                        }
                    }
                    else if (UDFColumnName == "UDF2")
                    {
                        if ((obj.UDF2 ?? string.Empty).Trim().ToLower() != (UDFValue ?? string.Empty).Trim().ToLower())
                        {
                            isSameAsBefore = false;
                            obj.UDF2 = UDFValue;
                        }
                    }
                    else if (UDFColumnName == "UDF3")
                    {
                        if ((obj.UDF3 ?? string.Empty).Trim().ToLower() != (UDFValue ?? string.Empty).Trim().ToLower())
                        {
                            isSameAsBefore = false;
                            obj.UDF3 = UDFValue;
                        }
                    }
                    else if (UDFColumnName == "UDF4")
                    {
                        if ((obj.UDF4 ?? string.Empty).Trim().ToLower() != (UDFValue ?? string.Empty).Trim().ToLower())
                        {
                            isSameAsBefore = false;
                            obj.UDF4 = UDFValue;
                        }
                    }
                    else if (UDFColumnName == "UDF5")
                    {
                        if ((obj.UDF5 ?? string.Empty).Trim().ToLower() != (UDFValue ?? string.Empty).Trim().ToLower())
                        {
                            isSameAsBefore = false;
                            obj.UDF5 = UDFValue;
                        }
                    }

                    if (isSameAsBefore == false)
                    {
                        context.SaveChanges();

                        if (UDFControlType == "Dropdown Editable")
                        {
                            UDFOptionDAL objUDFOptionDAL = new UDFOptionDAL(base.DataBaseName);
                            //bool IsExist = objUDFOptionDAL.GetUDFOptionsByUDFIDAndOption(UDFID, CompanyID, UDFValue);
                            bool IsExist = objUDFOptionDAL.IsUdfOptionExistsInUDF(UDFID, UDFValue);

                            if (IsExist == false)
                            {
                                UDFOptionsDTO objUDFOptionsDTO = new UDFOptionsDTO();
                                objUDFOptionsDTO.ID = 0;
                                objUDFOptionsDTO.UDFID = UDFID;
                                objUDFOptionsDTO.UDFOption = UDFValue;
                                objUDFOptionsDTO.Created = DateTimeUtility.DateTimeNow;
                                objUDFOptionsDTO.CreatedBy = UserID;
                                objUDFOptionsDTO.Updated = DateTimeUtility.DateTimeNow;
                                objUDFOptionsDTO.LastUpdatedBy = UserID;
                                objUDFOptionsDTO.CompanyID = CompanyID;
                                objUDFOptionsDTO.Room = RoomID;
                                objUDFOptionsDTO.GUID = Guid.NewGuid();
                                objUDFOptionDAL.Insert(objUDFOptionsDTO);
                            }
                        }
                    }
                }
                else
                {
                    var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                    _returnmessage = ResourceRead.GetResourceValueByKeyAndFullFilePath("RecordNotAvailable", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                    _returnstatus = "fail";
                }
            }
        }

        public void UpdatePullOrderNumberInPullHistory(long RoomID, long CompanyID, long UserID, string PullGUID, string PullOrderNumber, out string _returnmessage, out string _returnstatus,long EnterpriseId, string CultureCode)
        {
            UDFDAL objUDFDAL = new UDFDAL(base.DataBaseName);
            var messageResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", CultureCode, EnterpriseId, CompanyID);
            _returnmessage = ResourceRead.GetResourceValueByKeyAndFullFilePath("SaveMessage", messageResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResMessage", CultureCode);
            _returnstatus = "OK";
            Guid _pullGUID = new Guid(PullGUID);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullMaster obj = context.PullMasters.FirstOrDefault(t => t.GUID == _pullGUID);
                bool isSameAsBefore = true;

                if (obj != null)
                {
                    obj.LastUpdatedBy = UserID;
                    obj.Updated = DateTimeUtility.DateTimeNow;

                    if ((obj.PullOrderNumber ?? string.Empty).Trim().ToLower() != (PullOrderNumber ?? string.Empty).Trim().ToLower())
                    {
                        isSameAsBefore = false;
                        obj.PullOrderNumber = PullOrderNumber;
                    }

                    if (isSameAsBefore == false)
                    {
                        context.SaveChanges();

                        var ArrParams = new SqlParameter[] {
                                                                new SqlParameter("@PullOrderNumber", PullOrderNumber  ?? (object)DBNull.Value),
                                                                new SqlParameter("@RoomID", RoomID),
                                                                new SqlParameter("@CompanyID", CompanyID),
                                                                new SqlParameter("@UserID", UserID)
                                                            };

                        context.Database.ExecuteSqlCommand(@"EXEC InsertPullOrderNumberInPullHistory @PullOrderNumber, @RoomID, @CompanyID, @UserID", ArrParams);
                    }
                }
                else
                {
                    var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                    _returnmessage = ResourceRead.GetResourceValueByKeyAndFullFilePath("RecordNotAvailable", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                    _returnstatus = "fail";
                }
            }
        }

        public string GetPullGuidsByReportRange(string _range, string _rangeFieldID, string _rangeData, string RoomID, string CompanyID, bool _isSelectAllRangeDataPull = false)
        {
            if (string.IsNullOrWhiteSpace(_range) || string.IsNullOrWhiteSpace(_rangeFieldID))
                return string.Empty;

            string _dataGuids = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Range",_range)
                                    , new SqlParameter("@RangeData",_rangeData)
                                    , new SqlParameter("@RoomID",RoomID)
                                    , new SqlParameter("@CompanyID",CompanyID)
                                    , new SqlParameter("@IsSelectAllRangeDataPull",_isSelectAllRangeDataPull)};

                string qry = "exec [Schl_GetPullGuidsByReportRange] @Range, @RangeData,@RoomID, @CompanyID, @IsSelectAllRangeDataPull";
                List<Guid> lstBins = context.Database.SqlQuery<Guid>(qry, params1).ToList();

                if (lstBins != null && lstBins.Any())
                {
                    _dataGuids = string.Join(",", lstBins.Select(t => t).ToArray());
                }

                return _dataGuids;
            }
        }

        public string Schl_GetPullSummaryGuidsByReportRange(string _range, string _rangeFieldID, string _rangeData, string RoomID, string CompanyID, string ItemIsActive, bool _isSelectAllRangeDataPull = false)
        {
            if (string.IsNullOrWhiteSpace(_range) || string.IsNullOrWhiteSpace(_rangeFieldID))
                return string.Empty;

            string _dataGuids = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Range",_range)
                                    , new SqlParameter("@RangeData",_rangeData)
                                    , new SqlParameter("@RoomID",RoomID)
                                    , new SqlParameter("@CompanyID",CompanyID)
                                    , new SqlParameter("@IsSelectAllRangeDataPull",_isSelectAllRangeDataPull)
                                    , new SqlParameter("@ItemIsActive",ItemIsActive)};

                string qry = "exec [Schl_GetPullSummaryGuidsByReportRange] @Range, @RangeData,@RoomID, @CompanyID, @IsSelectAllRangeDataPull, @ItemIsActive";
                List<Guid> lstBins = context.Database.SqlQuery<Guid>(qry, params1).ToList();

                if (lstBins != null && lstBins.Any())
                {
                    _dataGuids = string.Join(",", lstBins.Select(t => t).ToArray());
                }

                return _dataGuids;
            }
        }


        #region [Send Eamil For ProjectSpend Limit Exceed]
        public void SendEmailForProjectSpendLimitExceed(ProjectMasterDTO objProjectSpendDTO)
        {
            if (objProjectSpendDTO != null)
            {
                bool isDLExceed = false;
                bool isItemQtyExceed = false;
                bool isItemDLExceed = false;
                bool isDLAboutToExceed = false;
                bool isItemQtyAboutToExceed = false;
                bool isItemDLAboutToExceed = false;
                //XElement Settinfile = null;

                if (HttpContext.Current != null && HttpContext.Current.Server != null)
                {
                    //Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                }
                else
                {
                    //Settinfile = XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                }

                string PSLimitPercentage = SiteSettingHelper.ProjectSpendLimitPercentage;
                decimal calculateAmount = 0;

                if (objProjectSpendDTO.DollarLimitAmount.GetValueOrDefault(0) > 0)
                {
                    calculateAmount = ((objProjectSpendDTO.DollarLimitAmount ?? 0) * Convert.ToDecimal(PSLimitPercentage)) / 100;

                    if (objProjectSpendDTO.DollarUsedAmount > objProjectSpendDTO.DollarLimitAmount)
                    {
                        isDLExceed = true;
                    }
                    else if (objProjectSpendDTO.DollarUsedAmount >= calculateAmount)
                    {
                        isDLAboutToExceed = true;
                    }
                }
                else
                {
                    isDLExceed = false;
                    isDLAboutToExceed = false;
                }

                if (!isDLExceed)
                {
                    foreach (ProjectSpendItemsDTO objDTO in objProjectSpendDTO.ProjectSpendItems)
                    {
                        double calculateQTY = 0;

                        if (objDTO.QuantityLimit.GetValueOrDefault(0) > 0)
                        {
                            calculateQTY = ((objDTO.QuantityLimit ?? 0) * Convert.ToDouble(PSLimitPercentage)) / 100;

                            if (objDTO.QuantityUsed > objDTO.QuantityLimit)
                            {
                                isItemQtyExceed = true;
                                break;
                            }
                            else if ((objDTO.QuantityUsed ?? 0) >= calculateQTY)
                            {
                                isItemQtyAboutToExceed = true;
                            }
                        }
                        else
                        {
                            isItemQtyExceed = false;
                            isItemQtyAboutToExceed = false;
                        }

                        decimal calculateItemDL = 0;

                        if (objDTO.DollarLimitAmount.GetValueOrDefault(0) > 0)
                        {
                            calculateItemDL = ((objDTO.DollarLimitAmount ?? 0) * Convert.ToDecimal(PSLimitPercentage)) / 100;

                            if (objDTO.DollarUsedAmount > objDTO.DollarLimitAmount)
                            {
                                isItemDLExceed = true;
                                break;
                            }
                            else if (objDTO.DollarUsedAmount >= calculateItemDL)
                            {
                                isItemDLAboutToExceed = true;
                            }
                        }
                        else
                        {
                            isItemDLExceed = false;
                            isItemDLAboutToExceed = false;
                        }
                    }
                }

                if (isDLExceed || isItemQtyExceed || isItemDLExceed)
                {
                    /*////////SEND EAMIL LIMIT EXCEED////////*/
                    SendEmailProjectSpendLimitExceed(objProjectSpendDTO, MailTemplate.ProjectSpendLimitExceed);
                    /*////////SEND EAMIL LIMIT EXCEED////////*/
                }
                else if (isDLAboutToExceed || isItemQtyAboutToExceed || isItemDLAboutToExceed)
                {
                    /*////////SEND EAMIL LIMIT ABOUT TO EXCEED////////*/
                    SendEmailProjectSpendLimitAboutExceed(objProjectSpendDTO, MailTemplate.ProjectSpendLimitAboutToExceed);
                    /*////////SEND EAMIL LIMIT ABOUT TO EXCEED////////*/
                }
            }
        }

        private void SendEmailProjectSpendLimitAboutExceed(ProjectMasterDTO objProjectSpendDTO, MailTemplate projectSpendLimitAboutToExceed)
        {
            AlertMail objAlertMail = new AlertMail();
            eMailMasterDAL objEmailDAL = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(base.DataBaseName).GetEnterpriseByDbName(base.DataBaseName);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurns.DAL.RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(objProjectSpendDTO.Room ?? 0, objProjectSpendDTO.CompanyID ?? 0, objProjectSpendDTO.CreatedBy ?? 0);
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            string CurrentCulturename = string.Empty;

            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                CurrentCulturename = ResourceHelper.CurrentCult.Name;
            }
            else if (objeTurnsRegionInfo != null && !string.IsNullOrWhiteSpace(objeTurnsRegionInfo.CultureCode))
            {
                CurrentCulturename = objeTurnsRegionInfo.CultureCode;
            }
            else
            {
                CurrentCulturename = "en-US";
            }

            try
            {
                objNotificationDAL = new NotificationDAL(base.DataBaseName);
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)projectSpendLimitAboutToExceed, objProjectSpendDTO.Room ?? 0, objProjectSpendDTO.CompanyID ?? 0, CurrentCulturename);
                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate.ForEach(t =>
                    {
                        string StrSubject = string.Empty;

                        if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                        {
                            StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                        }
                        string strToAddress = t.EmailAddress;
                        string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];

                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            objEmailDAL = new eMailMasterDAL(base.DataBaseName);
                            StringBuilder MessageBody = new StringBuilder();
                            objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();

                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && (t.EmailTemplateDetail.lstEmailTemplateDtls.Any()))
                            {
                                objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                            }

                            if (objEmailTemplateDetailDTO != null)
                            {
                                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                StrSubject = objEmailTemplateDetailDTO.MailSubject;
                            }
                            else
                            {
                                return;
                            }

                            if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                            {
                                if (objEnterpriseDTO != null)
                                {
                                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                    string DateTimeFormat = "MM/dd/yyyy";
                                    DateTime TZDateTimeNow = DateTime.UtcNow;

                                    if (objeTurnsRegionInfo != null)
                                    {
                                        DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;
                                        TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                                    }
                                }
                            }
                            MessageBody.Replace("@@ProjectSpendName@@", objProjectSpendDTO.ProjectSpendName);
                            MessageBody.Replace("@@DollarLimitAmount@@", Convert.ToString(objProjectSpendDTO.DollarLimitAmount ?? 0));
                            MessageBody.Replace("@@DollarUsedAmount@@", Convert.ToString(objProjectSpendDTO.DollarUsedAmount ?? 0));

                            string stratatTABLEatatTag = GetMailBodyProjectSpend(objProjectSpendDTO);

                            string replacePart = string.Empty;
                            if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                            {
                                replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                            }
                            else if (HttpContext.Current == null || HttpContext.Current.Request == null)
                            {
                                replacePart = ConfigurationManager.AppSettings["DomainName"];
                            }
                            else
                            {
                                string urlPart = HttpContext.Current.Request.Url.ToString();
                                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                            }

                            MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                            objeMailAttchList = new List<eMailAttachmentDTO>();
                            objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                        }
                    });
                }
            }
            finally
            {
                objEmailDAL = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objeMailAttchList = null;
            }
        }

        private void SendEmailProjectSpendLimitExceed(ProjectMasterDTO objProjectSpendDTO, MailTemplate projectSpendLimitExceed)
        {
            AlertMail objAlertMail = new AlertMail();
            eMailMasterDAL objEmailDAL = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(base.DataBaseName).GetEnterpriseByDbName(base.DataBaseName);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurns.DAL.RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(objProjectSpendDTO.Room ?? 0, objProjectSpendDTO.CompanyID ?? 0, objProjectSpendDTO.CreatedBy ?? 0);
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            string CurrentCulturename = string.Empty;

            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                CurrentCulturename = ResourceHelper.CurrentCult.Name;
            }
            else if (objeTurnsRegionInfo != null && !string.IsNullOrWhiteSpace(objeTurnsRegionInfo.CultureCode))
            {
                CurrentCulturename = objeTurnsRegionInfo.CultureCode;
            }
            else
            {
                CurrentCulturename = "en-US";
            }
            try
            {
                objNotificationDAL = new NotificationDAL(base.DataBaseName);
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)projectSpendLimitExceed, objProjectSpendDTO.Room ?? 0, objProjectSpendDTO.CompanyID ?? 0, CurrentCulturename);
                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate.ForEach(t =>
                    {
                        string StrSubject = string.Empty;

                        if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                        {
                            StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.FirstOrDefault().MailSubject;
                        }
                        string strToAddress = t.EmailAddress;
                        string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];

                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            objEmailDAL = new eMailMasterDAL(base.DataBaseName);
                            StringBuilder MessageBody = new StringBuilder();
                            objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();

                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.FirstOrDefault();
                            }
                            if (objEmailTemplateDetailDTO != null)
                            {
                                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                StrSubject = objEmailTemplateDetailDTO.MailSubject;
                            }
                            else
                            {
                                return;
                            }

                            if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                            {
                                if (objEnterpriseDTO != null)
                                {
                                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                    string DateTimeFormat = "MM/dd/yyyy";
                                    DateTime TZDateTimeNow = DateTime.UtcNow;

                                    if (objeTurnsRegionInfo != null)
                                    {
                                        DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;
                                        TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                                    }
                                }
                            }

                            MessageBody.Replace("@@ProjectSpendName@@", objProjectSpendDTO.ProjectSpendName);
                            MessageBody.Replace("@@DollarLimitAmount@@", Convert.ToString(objProjectSpendDTO.DollarLimitAmount ?? 0));
                            MessageBody.Replace("@@DollarUsedAmount@@", Convert.ToString(objProjectSpendDTO.DollarUsedAmount ?? 0));
                            string stratatTABLEatatTag = GetMailBodyProjectSpend(objProjectSpendDTO);
                            string replacePart = string.Empty;

                            if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                            {
                                replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                            }
                            else if (HttpContext.Current == null || HttpContext.Current.Request == null)
                            {
                                replacePart = ConfigurationManager.AppSettings["DomainName"];
                            }
                            else
                            {
                                string urlPart = HttpContext.Current.Request.Url.ToString();
                                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                            }

                            MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                            objeMailAttchList = new List<eMailAttachmentDTO>();
                            objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                        }
                    });
                }
            }
            finally
            {
                objEmailDAL = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objeMailAttchList = null;

            }
        }

        private string GetMailBodyProjectSpend(ProjectMasterDTO obj)
        {
            string messageBody = string.Empty;

            messageBody = @"
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResProjectSpendItems.QuantityLimit + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResProjectSpendItems.QuantityUsed + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResProjectSpendItems.DollarLimitAmount + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResProjectSpendItems.DollarUsedAmount + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>
            ";
            string trs = "";

            if (obj != null && obj.ProjectSpendItems != null && obj.ProjectSpendItems.Count > 0)
            {

                foreach (ProjectSpendItemsDTO item in obj.ProjectSpendItems)
                {
                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + Convert.ToString(item.QuantityLimit ?? 0) + @"
                        </td>
                        <td>
                            " + Convert.ToString(item.QuantityUsed ?? 0) + @"
                        </td>
                        <td>
                            " + Convert.ToString(item.DollarLimitAmount ?? 0) + @"
                        </td>
                         <td>
                            " + Convert.ToString(item.DollarUsedAmount) + @"
                        </td>
                    </tr>";

                }
            }
            else
            {
                trs += @"<tr>
                        <td colspan=""5"" style=""text-align:center"">
                           There is no item for this project spend
                        </td>
                    </tr>";
            }
            messageBody = messageBody.Replace("##TRS##", trs);

            return messageBody;
        }
        #endregion

        #region MS Credit

        public PullMasterViewDTO UpdateCumulativeOnHandNewForMS(PullMasterViewDTO objDTO)
        {
            PullMaster ObjPull = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool UpdateBillingFlag = false;
                string[] arrpullnames = new string[] { "p_evmi", "pull", "ms pull" };
                string[] arrCreditNames = new string[] { "cr_evmi", "credit", "ms credit", "credit:evmi" };
                string[] AllPullTypes = new string[] { "p_evmi", "pull", "ms pull", "cr_evmi", "credit", "ms credit", "credit:evmi" };
                ObjPull = context.PullMasters.FirstOrDefault(t => t.GUID == objDTO.GUID);
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objDTO.ItemGUID);

                if (objItem != null && ObjPull != null)
                {
                    if (objDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                    {
                        SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                        PullSchedulerDTO objpullSchedule = objSupplierMasterDAL.GetRoomScheduleForPull(objItem.SupplierID ?? 0, objItem.Room ?? 0, 7);

                        if (objpullSchedule != null && objpullSchedule.Pull_ScheduleMode == 5)
                        {
                            if (AllPullTypes.Contains((ObjPull.ActionType ?? "").ToLower()) || AllPullTypes.Contains((ObjPull.PullCredit ?? "").ToLower()))
                            {
                                UpdateBillingFlag = true;
                            }
                        }
                    }

                    ObjPull.ItemOnhandQty = objItem.OnHandQuantity;
                    ObjPull.ItemStageQty = objItem.StagedQuantity;
                    ObjPull.ItemLocationOnhandQty = 0;
                    ObjPull.ItemStageLocationQty = 0;

                    if (UpdateBillingFlag)
                    {
                        ObjPull.Billing = true;
                    }

                    if (context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.BinID == objDTO.BinID).Any())
                    {
                        ObjPull.ItemLocationOnhandQty = context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.BinID == objDTO.BinID).Sum(t => (t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0));

                    }

                    if (context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.StagingBinId == objDTO.BinID).Any())
                    {
                        ObjPull.ItemStageLocationQty = context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && t.StagingBinId == objDTO.BinID).Sum(t => (t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0));
                        ObjPull.ItemStageQty = ObjPull.ItemStageLocationQty;
                    }

                    IQueryable<PullDetail> lstPulldetails = context.PullDetails.Where(t => t.PULLGUID == ObjPull.GUID && (t.IsDeleted ?? false) == false).OrderByDescending(t => t.ID);

                    if (lstPulldetails.Any())
                    {
                        double PreviousQuantity = 0;

                        if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && arrpullnames.Contains(ObjPull.ActionType.ToLower()))
                        {
                            foreach (var item in lstPulldetails)
                            {
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && (ObjPull.ActionType.ToLower() == "pull" || ObjPull.ActionType.ToLower() == "p_evmi"))
                                {
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0) + PreviousQuantity;
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0) + PreviousQuantity;
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0);
                                }

                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && ObjPull.ActionType.ToLower() == "ms pull")
                                {
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0) + PreviousQuantity;
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0) + PreviousQuantity;
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0);
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0);
                                }
                                PreviousQuantity = PreviousQuantity + item.PoolQuantity ?? 0;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && arrCreditNames.Contains(ObjPull.ActionType.ToLower()))
                        {
                            foreach (var item in lstPulldetails)
                            {
                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && (ObjPull.ActionType.ToLower() == "cr_evmi" || ObjPull.ActionType.ToLower() == "credit" || ObjPull.ActionType.ToLower() == "credit:evmi"))
                                {
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0) - PreviousQuantity;
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0) - PreviousQuantity;
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0);
                                }

                                if (!string.IsNullOrWhiteSpace(ObjPull.ActionType) && ObjPull.ActionType.ToLower() == "ms credit")
                                {
                                    item.ItemStageQty = (ObjPull.ItemStageQty ?? 0) - PreviousQuantity;
                                    item.ItemOnhandQty = (ObjPull.ItemOnhandQty ?? 0);
                                    item.ItemLocationOnhandQty = (ObjPull.ItemLocationOnhandQty ?? 0);
                                    item.ItemStageLocationQty = (ObjPull.ItemStageLocationQty ?? 0) - PreviousQuantity;
                                }
                                PreviousQuantity = PreviousQuantity + item.PoolQuantity ?? 0;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
            return objDTO;
        }

        #endregion

        #region WI-4693-Allow specified rooms to ignore credit rules

        public ItemLocationDetailsDTO ConvertPullDetailtoItemLocationDetailForCreditRule(PullDetailsDTO item, long RoomID, long CompanyID, long UserID, string RoomDateFormat)
        {
            BinMasterDAL binDAL = null;
            ProjectMasterDAL pmDAL = null;
            DateTime? ExpirationDate = null;
            DateTime? ReceivedDate = null;
            string Received = DateTime.UtcNow.ToString("MM/dd/yyyy"), Expiration = DateTime.UtcNow.ToString("MM/dd/yyyy");
            long? BinID = 0;
            Guid? ProjectGuid = null;

            if (!string.IsNullOrEmpty(item.CreditBinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.CreditBinName, UserID, RoomID, CompanyID);
                item.BinName = item.CreditBinName;
                item.BinID = BinID;
            }
            else if (item.BinID > 0)
            {
                BinID = item.BinID;
            }

            if (!string.IsNullOrEmpty(item.BinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, UserID, RoomID, CompanyID);
                item.BinID = BinID;
            }

            if (!string.IsNullOrEmpty(item.CreditProjectName))
            {
                pmDAL = new ProjectMasterDAL(base.DataBaseName);
                ProjectMasterDTO objDTO = pmDAL.GetProjectByName(item.CreditProjectName, RoomID, CompanyID, null);

                if (objDTO != null)
                {
                    ProjectGuid = objDTO.GUID;
                    bool isInsert = false;
                    ProjectSpendItemsDAL objSPI = new ProjectSpendItemsDAL(base.DataBaseName);
                    var tmpsupplierIds = new List<long>();
                    //var projectSpendLI = objSPI.GetAllRecords(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds);
                    var projectSpendLI = objSPI.GetProjectSpendItem(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds, string.Empty);

                    if (projectSpendLI != null && projectSpendLI.Count() > 0)
                    {
                        var lineItem = projectSpendLI.Where(t => (t.ItemNumber ?? string.Empty).Trim().ToLower() == item.ItemNumber.Trim().ToLower());
                        if (lineItem != null && lineItem.Count() > 0)
                        {
                            isInsert = false;
                        }
                        else
                        {
                            isInsert = true;
                        }
                    }
                    else
                    {
                        isInsert = true;
                    }

                    if (isInsert)
                    {
                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                        projectSpendItem.QuantityLimit = null;
                        projectSpendItem.QuantityUsed = null;
                        projectSpendItem.DollarLimitAmount = null;
                        projectSpendItem.DollarUsedAmount = null;
                        projectSpendItem.ItemGUID = item.ItemGUID;
                        projectSpendItem.CreatedBy = UserID;
                        projectSpendItem.LastUpdatedBy = UserID;
                        projectSpendItem.Room = RoomID;
                        projectSpendItem.CompanyID = CompanyID;
                        projectSpendItem.ItemNumber = item.ItemNumber;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.ProjectSpendName = item.CreditProjectName;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.ProjectGUID = objDTO.GUID;
                        objSPI.Insert(projectSpendItem);
                    }
                }
                else
                {
                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(base.DataBaseName);
                    ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                    objProjectSpendDTO.ProjectSpendName = item.CreditProjectName;
                    objProjectSpendDTO.AddedFrom = "Web";
                    objProjectSpendDTO.EditedFrom = "Web";
                    objProjectSpendDTO.CompanyID = CompanyID;
                    objProjectSpendDTO.Room = RoomID;
                    objProjectSpendDTO.DollarLimitAmount = 0;
                    objProjectSpendDTO.Description = string.Empty;
                    objProjectSpendDTO.DollarUsedAmount = null;
                    objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.CreatedBy = UserID;
                    objProjectSpendDTO.LastUpdatedBy = UserID;
                    objProjectSpendDTO.UDF1 = string.Empty;
                    objProjectSpendDTO.UDF2 = string.Empty;
                    objProjectSpendDTO.UDF3 = string.Empty;
                    objProjectSpendDTO.UDF4 = string.Empty;
                    objProjectSpendDTO.UDF5 = string.Empty;
                    objProjectSpendDTO.GUID = Guid.NewGuid();

                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                    objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID.GetValueOrDefault(Guid.Empty));

                    List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                    ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                    projectSpendItem.QuantityLimit = null;
                    projectSpendItem.QuantityUsed = null;
                    projectSpendItem.DollarLimitAmount = null;
                    projectSpendItem.DollarUsedAmount = null;
                    projectSpendItem.ItemGUID = item.ItemGUID;
                    projectSpendItem.CreatedBy = UserID;
                    projectSpendItem.LastUpdatedBy = UserID;
                    projectSpendItem.Room = RoomID;
                    projectSpendItem.CompanyID = CompanyID;

                    if (objItemmasterDTO != null)
                        projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;

                    projectSpendItem.IsArchived = false;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.ProjectSpendName = item.CreditProjectName;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.IsArchived = false;
                    projectSpendItemList.Add(projectSpendItem);
                    objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;
                    objProjectSpendDTO.IsDeleted = false;
                    objProjectSpendDTO.IsArchived = false;

                    objProjectSpendDAL.Insert(objProjectSpendDTO);
                    ProjectGuid = objProjectSpendDTO.GUID;
                }
            }
            else
            {
                ProjectGuid = item.ProjectSpendGUID;
            }

            if (!string.IsNullOrEmpty(item.Expiration))
            {
                var moveMaterialDAL = new MoveMaterialDAL(base.DataBaseName);

                if (item.ItemLocationDetailGUID.HasValue)
                {
                    var expdt = new ItemLocationDetailsDAL(base.DataBaseName).GetItemLocationDetailsByLocationGuid(item.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                    if (expdt != null && expdt.ExpirationDate.HasValue)
                    {
                        ExpirationDate = expdt.ExpirationDate;
                    }
                }

                if (!(ExpirationDate.HasValue && ExpirationDate != null))
                {
                    ExpirationDate = moveMaterialDAL.GetExpireDateFromString(item.Expiration, RoomDateFormat);
                }
                //CultureInfo CurrentCult = new CultureInfo("en-US");
                //eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);

                //if (RegionInfo != null)
                //    CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                //DateTime DTExp = DateTime.MinValue;
                //DateTime.TryParseExact(item.Expiration, RoomDateFormat, CurrentCult, System.Globalization.DateTimeStyles.None, out DTExp);

                //if (DTExp != DateTime.MinValue)
                //{
                //    ExpirationDate = DTExp;
                //}
                if (ExpirationDate != null)
                    item.Expiration = Convert.ToDateTime(ExpirationDate).ToString("MM/dd/yyyy");
            }

            if (item.ReceivedDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue)
            {
                if (!string.IsNullOrEmpty(item.Received))
                {
                    CultureInfo CurrentCult = new CultureInfo("en-US");
                    eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                    if (RegionInfo != null)
                        CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                    DateTime DTRec = DateTime.MinValue;
                    DateTime.TryParseExact(item.Received, RoomDateFormat, CurrentCult, System.Globalization.DateTimeStyles.None, out DTRec);

                    if (DTRec != DateTime.MinValue)
                    {
                        ReceivedDate = DTRec;
                    }
                    item.Received = Convert.ToDateTime(item.Received).ToString("MM/dd/yyyy");
                }
            }
            else
            {
                ReceivedDate = item.ReceivedDate;
            }

            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItemDTO = itemDAL.GetItemWithoutJoins(null, item.ItemGUID);

            if (ReceivedDate == null || ReceivedDate == DateTime.MinValue)
            {
                ReceivedDate = DateTime.UtcNow;
                Received = ReceivedDate.Value.ToString("MM/dd/yyyy");
            }

            if (objItemDTO.DateCodeTracking)
            {
                if (ExpirationDate == null || ExpirationDate == DateTime.MinValue)
                {
                    ExpirationDate = DateTime.UtcNow.Date;
                    Expiration = ExpirationDate.Value.ToString("MM/dd/yyyy");
                }
            }
            else
            {
                Expiration = "";
            }

            int costUOMValue = 1;

            if (objItemDTO != null)
            {
                CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemDTO.CostUOMID.GetValueOrDefault(0));
                if (oCostUOMMasterDTO != null)
                {
                    costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                    if (costUOMValue == 0)
                        costUOMValue = 1;
                }
            }

            ItemLocationDetailsDTO itml = new ItemLocationDetailsDTO()
            {
                BinID = item.BinID,
                BinNumber = item.BinName,
                ConsignedQuantity = item.ConsignedQuantity,
                CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                ItemGUID = item.ItemGUID,
                Cost = item.ItemCost * costUOMValue,
                ProjectSpentGUID = ProjectGuid == null ? item.ProjectSpendGUID : ProjectGuid,
                SerialNumber = item.SerialNumber,
                LotNumber = item.LotNumber,
                Expiration = (item.Expiration ?? string.Empty) == string.Empty ? Expiration : item.Expiration,
                Received = (item.Received ?? string.Empty) == string.Empty ? Received : item.Received,
                ReceivedDate = ReceivedDate,
                ExpirationDate = ExpirationDate,
                UDF1 = item.CreditUDF1,
                UDF2 = item.CreditUDF2,
                UDF3 = item.CreditUDF3,
                UDF4 = item.CreditUDF4,
                UDF5 = item.CreditUDF5,
                WorkOrderGUID = item.WorkOrderGUID,
                CompanyID = CompanyID,
                Room = RoomID,
                Created = DateTimeUtility.DateTimeNow,
                IsOnlyFromUI = true,
                IsPDAEdit = false,
                IsWebEdit = true,
                EditedFrom = item.AddedFrom,
                AddedFrom = item.EditedFrom,
                InsertedFrom = "Pull Credit",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                PullOrderNumber = item.PullOrderNumber,
                SellPrice = item.ItemPrice * costUOMValue,
                SerialNumberTracking = objItemDTO != null ? objItemDTO.SerialNumberTracking : false,
                LotNumberTracking = objItemDTO != null ? objItemDTO.LotNumberTracking : false,
                DateCodeTracking = objItemDTO != null ? objItemDTO.DateCodeTracking : false,
                PullGUID = item.NEW_PullGUID,

                PullGUIDForCreditHistory = item.PULLGUID,
                PullDetailGUIDForCreditHistory = item.GUID,
                SupplierAccountGuid = item.SupplierAccountGuid
            };

            return itml;
        }

        public MaterialStagingPullDetailDTO ConvertPullDetailtoMaterialStaginPullDetailForCreditRule(PullDetailsDTO item, long RoomID, long CompanyID, long UserID, string RoomDateFormat, Guid MaterailStagingGuid)
        {
            BinMasterDAL binDAL = null;
            ProjectMasterDAL pmDAL = null;
            DateTime? ExpirationDate = null;
            DateTime? ReceivedDate = null;
            Guid? ProjectGuid = null;
            string Received = DateTime.UtcNow.ToString("MM/dd/yyyy"), Expiration = DateTime.UtcNow.ToString("MM/dd/yyyy");
            long? BinID = 0;

            if (!string.IsNullOrEmpty(item.CreditBinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.CreditBinName, UserID, RoomID, CompanyID, true);
                item.BinName = item.CreditBinName;
                item.BinID = BinID;
            }
            else if (item.BinID > 0)
            {
                BinID = item.BinID;
            }

            if (!string.IsNullOrEmpty(item.BinName))
            {
                binDAL = new BinMasterDAL(base.DataBaseName);
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, UserID, RoomID, CompanyID, true);
                item.BinID = BinID;
            }

            if (!string.IsNullOrEmpty(item.CreditProjectName))
            {
                pmDAL = new ProjectMasterDAL(base.DataBaseName);
                ProjectMasterDTO objDTO = pmDAL.GetProjectByName(item.CreditProjectName, RoomID, CompanyID, null);

                if (objDTO != null)
                {
                    ProjectGuid = objDTO.GUID;
                    bool isInsert = false;
                    ProjectSpendItemsDAL objSPI = new ProjectSpendItemsDAL(base.DataBaseName);
                    var tmpsupplierIds = new List<long>();
                    //var projectSpendLI = objSPI.GetAllRecords(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds);
                    var projectSpendLI = objSPI.GetProjectSpendItem(objDTO.GUID, RoomID, CompanyID, tmpsupplierIds, string.Empty);

                    if (projectSpendLI != null && projectSpendLI.Count() > 0)
                    {
                        var lineItem = projectSpendLI.Where(t => (t.ItemNumber ?? string.Empty).Trim().ToLower() == item.ItemNumber.Trim().ToLower());

                        if (lineItem != null && lineItem.Count() > 0)
                        {
                            isInsert = false;
                        }
                        else
                        {
                            isInsert = true;
                        }
                    }
                    else
                    {
                        isInsert = true;
                    }

                    if (isInsert)
                    {
                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                        projectSpendItem.QuantityLimit = null;
                        projectSpendItem.QuantityUsed = null;
                        projectSpendItem.DollarLimitAmount = null;
                        projectSpendItem.DollarUsedAmount = null;
                        projectSpendItem.ItemGUID = item.ItemGUID;
                        projectSpendItem.CreatedBy = UserID;
                        projectSpendItem.LastUpdatedBy = UserID;
                        projectSpendItem.Room = RoomID;
                        projectSpendItem.CompanyID = CompanyID;
                        projectSpendItem.ItemNumber = item.ItemNumber;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.ProjectSpendName = item.CreditProjectName;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.ProjectGUID = objDTO.GUID;

                        objSPI.Insert(projectSpendItem);
                    }
                }
                else
                {
                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(base.DataBaseName);
                    ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                    objProjectSpendDTO.ProjectSpendName = item.CreditProjectName;
                    objProjectSpendDTO.AddedFrom = "Web";
                    objProjectSpendDTO.EditedFrom = "Web";
                    objProjectSpendDTO.CompanyID = CompanyID;
                    objProjectSpendDTO.Room = RoomID;
                    objProjectSpendDTO.DollarLimitAmount = 0;
                    objProjectSpendDTO.Description = string.Empty;
                    objProjectSpendDTO.DollarUsedAmount = null;
                    objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                    objProjectSpendDTO.CreatedBy = UserID;
                    objProjectSpendDTO.LastUpdatedBy = UserID;
                    objProjectSpendDTO.UDF1 = string.Empty;
                    objProjectSpendDTO.UDF2 = string.Empty;
                    objProjectSpendDTO.UDF3 = string.Empty;
                    objProjectSpendDTO.UDF4 = string.Empty;
                    objProjectSpendDTO.UDF5 = string.Empty;
                    objProjectSpendDTO.GUID = Guid.NewGuid();

                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                    objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID.GetValueOrDefault(Guid.Empty));

                    List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                    ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                    projectSpendItem.QuantityLimit = null;
                    projectSpendItem.QuantityUsed = null;
                    projectSpendItem.DollarLimitAmount = null;
                    projectSpendItem.DollarUsedAmount = null;
                    projectSpendItem.ItemGUID = item.ItemGUID;
                    projectSpendItem.CreatedBy = UserID;
                    projectSpendItem.LastUpdatedBy = UserID;
                    projectSpendItem.Room = RoomID;
                    projectSpendItem.CompanyID = CompanyID;

                    if (objItemmasterDTO != null)
                        projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;

                    projectSpendItem.IsArchived = false;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.ProjectSpendName = item.CreditProjectName;
                    projectSpendItem.IsDeleted = false;
                    projectSpendItem.IsArchived = false;
                    projectSpendItemList.Add(projectSpendItem);
                    objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;
                    objProjectSpendDTO.IsDeleted = false;
                    objProjectSpendDTO.IsArchived = false;
                    objProjectSpendDAL.Insert(objProjectSpendDTO);
                    ProjectGuid = objProjectSpendDTO.GUID;
                }
            }
            else
            {
                ProjectGuid = item.ProjectSpendGUID;
            }

            if (!string.IsNullOrEmpty(item.Expiration))
            {
                CultureInfo CurrentCult = new CultureInfo("en-US");
                eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);

                if (RegionInfo != null)
                    CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                DateTime DTExp = DateTime.MinValue;
                DateTime.TryParseExact(item.Expiration, RoomDateFormat, CurrentCult, System.Globalization.DateTimeStyles.None, out DTExp);

                if (DTExp != DateTime.MinValue)
                {
                    ExpirationDate = DTExp;
                }
                if (ExpirationDate != null)
                    item.Expiration = Convert.ToDateTime(ExpirationDate).ToString("MM/dd/yyyy");
            }

            if (!string.IsNullOrEmpty(item.Received))
            {
                CultureInfo CurrentCult = new CultureInfo("en-US");
                eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);

                if (RegionInfo != null)
                    CurrentCult = new CultureInfo(RegionInfo.CultureCode ?? "en-US");

                DateTime DTRec = DateTime.MinValue;
                DateTime.TryParseExact(item.Received, RoomDateFormat, CurrentCult, System.Globalization.DateTimeStyles.None, out DTRec);

                if (DTRec != DateTime.MinValue)
                {
                    ReceivedDate = DTRec;
                }
                item.Received = Convert.ToDateTime(item.Received).ToString("MM/dd/yyyy");
            }

            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItemDTO = itemDAL.GetItemWithoutJoins(null, item.ItemGUID);

            if (ReceivedDate == null || ReceivedDate == DateTime.MinValue)
            {
                ReceivedDate = DateTime.UtcNow.Date;
                Received = ReceivedDate.Value.ToString("MM/dd/yyyy");
            }

            if (objItemDTO.DateCodeTracking)
            {
                if (ExpirationDate == null || ExpirationDate == DateTime.MinValue)
                {
                    ExpirationDate = DateTime.UtcNow.Date;
                    Expiration = ExpirationDate.Value.ToString("MM/dd/yyyy");
                }
            }

            int costUOMValue = 1;

            if (objItemDTO != null)
            {
                CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemDTO.CostUOMID.GetValueOrDefault(0));

                if (oCostUOMMasterDTO != null)
                {
                    costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                    if (costUOMValue == 0)
                        costUOMValue = 1;
                }
            }

            MaterialStagingPullDetailDTO itml = new MaterialStagingPullDetailDTO()
            {
                BinID = item.BinID,
                StagingBinId = BinID ?? 0,
                BinNumber = item.BinName,
                ConsignedQuantity = item.ConsignedQuantity,
                CustomerOwnedQuantity = item.CustomerOwnedQuantity,
                ItemGUID = item.ItemGUID ?? Guid.Empty,
                Cost = item.ItemCost * costUOMValue,
                SellPrice = item.ItemPrice * costUOMValue,
                ProjectSpentGUID = ProjectGuid == null ? item.ProjectSpendGUID : ProjectGuid,
                SerialNumber = item.SerialNumber,
                LotNumber = item.LotNumber,
                Expiration = (item.Expiration ?? string.Empty) == string.Empty ? Expiration : item.Expiration,
                Received = (item.Received ?? string.Empty) == string.Empty ? Received : item.Received,
                ReceivedDate = ReceivedDate,
                ExpirationDate = ExpirationDate,
                UDF1 = item.CreditUDF1,
                UDF2 = item.CreditUDF2,
                UDF3 = item.CreditUDF3,
                UDF4 = item.CreditUDF4,
                UDF5 = item.CreditUDF5,
                WorkOrderGuid = item.WorkOrderGUID,
                CompanyID = CompanyID,
                Room = RoomID,
                Created = DateTimeUtility.DateTimeNow,
                IsOnlyFromUI = true,
                IsPDAEdit = false,
                IsWebEdit = true,
                EditedFrom = item.AddedFrom,
                AddedFrom = item.EditedFrom,
                InsertedFrom = "Pull MS Credit",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                PullOrderNumber = item.PullOrderNumber,
                MaterialStagingGUID = (MaterailStagingGuid != null && MaterailStagingGuid != Guid.Empty ? MaterailStagingGuid : item.MaterialStagingGUID),
                MaterialStagingdtlGUID = item.MaterialStagingdtlGUID,
                PoolQuantity = item.PoolQuantity,
                SerialNumberTracking = objItemDTO != null ? objItemDTO.SerialNumberTracking : false,
                LotNumberTracking = objItemDTO != null ? objItemDTO.LotNumberTracking : false,
                DateCodeTracking = objItemDTO != null ? objItemDTO.DateCodeTracking : false,

                PullGUIDForCreditHistory = item.PULLGUID,
                PullDetailGUIDForCreditHistory = item.GUID,
                SupplierAccountGuid = item.SupplierAccountGuid
            };

            return itml;
        }

        public List<PullDetailsDTO> GetPrevPullForCreditEntry(List<PullDetailsDTO> lstPrevPulls, ItemInfoToCredit itemToCredit, double RemainingCreditQuantity, long RoomID, long CompanyID, 
                string RoomDateFormat, long EnterpriseId, string CultureCode)
        {
            List<PullDetailsDTO> lstPullDetailDTO = new List<PullDetailsDTO>();
            try
            {
                ItemMasterDTO objItemMaster = new ItemMasterDTO();
                objItemMaster = new ItemMasterDAL(base.DataBaseName).GetItemWithMasterTableJoins(null, itemToCredit.ItemGuid, RoomID, CompanyID);
                PullTransactionDAL objPull = new PullTransactionDAL(base.DataBaseName);
                #region for Serail,Lot and date code

                if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                {
                    if (objItemMaster != null)
                    {
                        if (lstPrevPulls.Count > 0)
                        {
                            if (itemToCredit.ItemTracking == "SERIALTRACK")
                                itemToCredit.PrevPullsToCredit = itemToCredit.PrevPullsToCredit.Distinct().Where(x => !lstPrevPulls.Any(e => e.SerialNumber == x.Serial)).ToList();
                            else if (itemToCredit.ItemTracking == "LOTTRACK")
                            {
                                if (objItemMaster.LotNumberTracking && objItemMaster.DateCodeTracking)
                                    itemToCredit.PrevPullsToCredit = itemToCredit.PrevPullsToCredit.Distinct().Where(x => !lstPrevPulls.Any(e => e.LotNumber == x.Lot && Convert.ToDateTime(e.Expiration).ToString("MM/dd/yyyy") == Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy"))).ToList();
                                else
                                    itemToCredit.PrevPullsToCredit = itemToCredit.PrevPullsToCredit.Distinct().Where(x => !lstPrevPulls.Any(e => e.LotNumber == x.Lot)).ToList();
                            }
                            else if (itemToCredit.ItemTracking == "DATECODETRACK")
                                itemToCredit.PrevPullsToCredit = itemToCredit.PrevPullsToCredit.Distinct().Where(x => !lstPrevPulls.Any(e => Convert.ToDateTime(e.Expiration).ToString("MM/dd/yyyy") == Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy"))).ToList();
                        }
                        var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                        string msgCreditTransactionForSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCreditTransactionForSerialNumber", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                        string msgCreditTransactionForLotAndExpDateAvailable = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditTransactionForLotAndExpDateAvailable", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                        
                        foreach (PullDetailToCredit itmCredit in itemToCredit.PrevPullsToCredit)
                        {
                            PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();

                            if (itemToCredit.ItemTracking == "SERIALTRACK")
                            {
                                bool IsSerailAvailableForCredit = objPull.ValidateSerialNumberForCredit(objItemMaster.GUID, itmCredit.Serial, CompanyID, RoomID);
                                if (!IsSerailAvailableForCredit)
                                {
                                    objPullDetailDTO.AddedFrom = msgCreditTransactionForSerialNumber + " " + itmCredit.Serial + ".";
                                    objPullDetailDTO.EditedFrom = "Fail";
                                    lstPullDetailDTO.Add(objPullDetailDTO);
                                    continue;
                                }
                            }

                            if (objItemMaster.LotNumberTracking && objItemMaster.DateCodeTracking)
                            {
                                DateTime ExpirationDate;
                                if (!string.IsNullOrWhiteSpace(itmCredit.ExpireDate))
                                {
                                    ExpirationDate = DateTime.ParseExact(itmCredit.ExpireDate, RoomDateFormat, ResourceHelper.CurrentCult);
                                    //ExpirationDate = Convert.ToDateTime(Convert.ToDateTime(itmCredit.ExpireDate).ToString("MM/dd/yyyy"));
                                    bool IsLotAvailableForCredit = objPull.ValidateLotDateCodeForCredit(objItemMaster.GUID, itmCredit.Lot, ExpirationDate, CompanyID, RoomID);

                                    if (!IsLotAvailableForCredit)
                                    {
                                        objPullDetailDTO.AddedFrom = string.Format(msgCreditTransactionForLotAndExpDateAvailable, itmCredit.Lot, ExpirationDate.ToString(Convert.ToString(RoomDateFormat)));
                                        objPullDetailDTO.EditedFrom = "Fail";
                                        lstPullDetailDTO.Add(objPullDetailDTO);
                                        continue;
                                    }
                                }
                            }

                            objPullDetailDTO.PULLGUID = Guid.Empty;
                            objPullDetailDTO.Room = RoomID;
                            objPullDetailDTO.CompanyID = CompanyID;
                            objPullDetailDTO.ItemGUID = objItemMaster.GUID;
                            objPullDetailDTO.ItemNumber = objItemMaster.ItemNumber;
                            objPullDetailDTO.ProjectSpendName = itemToCredit.ProjectName;
                            objPullDetailDTO.ItemCost = (objItemMaster.SellPrice
                                                        / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                            ? objItemMaster.CostUOMValue : 1));
                            objPullDetailDTO.ItemPrice = (objItemMaster.Cost
                                                        / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                            ? objItemMaster.CostUOMValue : 1));

                            objPullDetailDTO.PullCredit = "pull";
                            if (objItemMaster.Consignment)
                                objPullDetailDTO.ConsignedQuantity = itmCredit.Qty;
                            else
                                objPullDetailDTO.CustomerOwnedQuantity = itmCredit.Qty;

                            if (objItemMaster.SerialNumberTracking && objItemMaster.DateCodeTracking)
                            {
                                objPullDetailDTO.SerialNumber = itmCredit.Serial;
                                objPullDetailDTO.Expiration = itmCredit.ExpireDate;
                            }
                            if (objItemMaster.LotNumberTracking && objItemMaster.DateCodeTracking)
                            {
                                objPullDetailDTO.LotNumber = itmCredit.Lot;
                                objPullDetailDTO.Expiration = itmCredit.ExpireDate;
                            }
                            if (objItemMaster.SerialNumberTracking)
                            {
                                objPullDetailDTO.SerialNumber = itmCredit.Serial;
                            }
                            if (objItemMaster.LotNumberTracking)
                            {
                                objPullDetailDTO.LotNumber = itmCredit.Lot;
                            }
                            if (objItemMaster.DateCodeTracking)
                            {
                                objPullDetailDTO.Expiration = itmCredit.ExpireDate;
                            }
                            if (!string.IsNullOrEmpty(itemToCredit.Bin))
                                objPullDetailDTO.CreditBinName = itemToCredit.Bin;

                            if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                                objPullDetailDTO.CreditProjectName = itemToCredit.ProjectName;

                            objPullDetailDTO.CreditUDF1 = itemToCredit.UDF1;
                            objPullDetailDTO.CreditUDF2 = itemToCredit.UDF2;
                            objPullDetailDTO.CreditUDF3 = itemToCredit.UDF3;
                            objPullDetailDTO.CreditUDF4 = itemToCredit.UDF4;
                            objPullDetailDTO.CreditUDF5 = itemToCredit.UDF5;
                            objPullDetailDTO.WorkOrderGUID = itemToCredit.WOGuid;
                            objPullDetailDTO.PullOrderNumber = itemToCredit.PullOrderNumber;
                            objPullDetailDTO.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                            objPullDetailDTO.NEW_PullGUID = itemToCredit.PullGUID;

                            if (String.IsNullOrWhiteSpace(itemToCredit.EditedFrom))
                            {
                                objPullDetailDTO.AddedFrom = "Web";
                                objPullDetailDTO.EditedFrom = "Web";
                            }
                            else
                            {
                                objPullDetailDTO.AddedFrom = itemToCredit.EditedFrom;
                                objPullDetailDTO.EditedFrom = itemToCredit.EditedFrom;
                            }

                            lstPullDetailDTO.Add(objPullDetailDTO);
                        }
                    }
                }

                #endregion
                else if (itemToCredit.ItemTracking != "QUICKLIST")
                {
                    if (objItemMaster != null)
                    {
                        PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();

                        objPullDetailDTO.PULLGUID = Guid.Empty;
                        objPullDetailDTO.Room = RoomID;
                        objPullDetailDTO.CompanyID = CompanyID;
                        objPullDetailDTO.ItemGUID = objItemMaster.GUID;
                        objPullDetailDTO.ItemNumber = objItemMaster.ItemNumber;
                        objPullDetailDTO.ProjectSpendName = itemToCredit.ProjectName;
                        objPullDetailDTO.ItemCost = (objItemMaster.SellPrice
                                                    / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                        ? objItemMaster.CostUOMValue : 1));
                        objPullDetailDTO.ItemPrice = (objItemMaster.Cost
                                                    / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                        ? objItemMaster.CostUOMValue : 1));

                        objPullDetailDTO.PullCredit = "pull";
                        if (objItemMaster.Consignment)
                            objPullDetailDTO.ConsignedQuantity = RemainingCreditQuantity;
                        else
                            objPullDetailDTO.CustomerOwnedQuantity = RemainingCreditQuantity;

                        if (!string.IsNullOrEmpty(itemToCredit.Bin))
                            objPullDetailDTO.CreditBinName = itemToCredit.Bin;

                        if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                            objPullDetailDTO.CreditProjectName = itemToCredit.ProjectName;

                        objPullDetailDTO.CreditUDF1 = itemToCredit.UDF1;
                        objPullDetailDTO.CreditUDF2 = itemToCredit.UDF2;
                        objPullDetailDTO.CreditUDF3 = itemToCredit.UDF3;
                        objPullDetailDTO.CreditUDF4 = itemToCredit.UDF4;
                        objPullDetailDTO.CreditUDF5 = itemToCredit.UDF5;
                        objPullDetailDTO.WorkOrderGUID = itemToCredit.WOGuid;
                        objPullDetailDTO.PullOrderNumber = itemToCredit.PullOrderNumber;
                        objPullDetailDTO.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                        objPullDetailDTO.NEW_PullGUID = itemToCredit.PullGUID;
                        if (String.IsNullOrWhiteSpace(itemToCredit.EditedFrom))
                        {
                            objPullDetailDTO.AddedFrom = "Web";
                            objPullDetailDTO.EditedFrom = "Web";
                        }
                        else
                        {
                            objPullDetailDTO.AddedFrom = itemToCredit.EditedFrom;
                            objPullDetailDTO.EditedFrom = itemToCredit.EditedFrom;
                        }
                        lstPullDetailDTO.Add(objPullDetailDTO);
                    }
                }

                return lstPullDetailDTO;
            }
            finally
            {
            }
        }

        public List<PullDetailsDTO> GetPrevMSPullForCreditEntry(List<PullDetailsDTO> lstPrevPulls, ItemInfoToMSCredit itemToCredit, double RemainingCreditQuantity, Int64 RoomID, 
                                    Int64 CompanyID, string RoomDateFormat, TimeZoneInfo CurrentTimeZone,long EnterpriseId, string CultureCode)
        {
            List<PullDetailsDTO> lstPullDetailDTO = new List<PullDetailsDTO>();
            PullTransactionDAL objPull = new PullTransactionDAL(base.DataBaseName);

            try
            {
                var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                string msgCreditTransactionForSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCreditTransactionForSerialNumber", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                string msgCreditTransactionForLotAndExpDateAvailable = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditTransactionForLotAndExpDateAvailable", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                ItemMasterDTO objItemMaster = new ItemMasterDTO();
                objItemMaster = new ItemMasterDAL(base.DataBaseName).GetItemWithMasterTableJoins(null, itemToCredit.ItemGuid, RoomID, CompanyID);

                #region for Serail,Lot and date code

                if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                {
                    if (objItemMaster != null)
                    {
                        if (lstPrevPulls.Count > 0)
                        {
                            if (itemToCredit.ItemTracking == "SERIALTRACK")
                                itemToCredit.PrevPullsToMSCredit = itemToCredit.PrevPullsToMSCredit.Distinct().Where(x => !lstPrevPulls.Any(e => e.SerialNumber == x.Serial)).ToList();
                            else if (itemToCredit.ItemTracking == "LOTTRACK")
                            {
                                if (objItemMaster.LotNumberTracking && objItemMaster.DateCodeTracking)
                                    itemToCredit.PrevPullsToMSCredit = itemToCredit.PrevPullsToMSCredit.Distinct().Where(x => !lstPrevPulls.Any(e => e.LotNumber == x.Lot && Convert.ToDateTime(e.Expiration).ToString("MM/dd/yyyy") == Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy"))).ToList();
                                else
                                    itemToCredit.PrevPullsToMSCredit = itemToCredit.PrevPullsToMSCredit.Distinct().Where(x => !lstPrevPulls.Any(e => e.LotNumber == x.Lot)).ToList();
                            }
                            else if (itemToCredit.ItemTracking == "DATECODETRACK")
                                itemToCredit.PrevPullsToMSCredit = itemToCredit.PrevPullsToMSCredit.Distinct().Where(x => !lstPrevPulls.Any(e => Convert.ToDateTime(e.Expiration).ToString("MM/dd/yyyy") == Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy"))).ToList();
                        }

                        foreach (PullDetailToMSCredit itmCredit in itemToCredit.PrevPullsToMSCredit)
                        {
                            PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();

                            if (itemToCredit.ItemTracking == "SERIALTRACK")
                            {
                                bool IsSerailAvailableForCredit = objPull.ValidateSerialNumberForCredit(objItemMaster.GUID, itmCredit.Serial, CompanyID, RoomID);

                                if (!IsSerailAvailableForCredit)
                                {
                                    objPullDetailDTO.AddedFrom = msgCreditTransactionForSerialNumber + " " + itmCredit.Serial + ".";
                                    objPullDetailDTO.EditedFrom = "Fail";
                                    lstPullDetailDTO.Add(objPullDetailDTO);
                                    continue;
                                }
                            }

                            if (objItemMaster.LotNumberTracking && objItemMaster.DateCodeTracking)
                            {
                                DateTime ExpirationDate;

                                if (!string.IsNullOrWhiteSpace(itmCredit.ExpireDate))
                                {
                                    ExpirationDate = DateTime.ParseExact(itmCredit.ExpireDate, RoomDateFormat, ResourceHelper.CurrentCult);
                                    //ExpirationDate = Convert.ToDateTime(Convert.ToDateTime(itmCredit.ExpireDate).ToString("MM/dd/yyyy"));
                                    bool IsLotAvailableForCredit = objPull.ValidateLotDateCodeForCredit(objItemMaster.GUID, itmCredit.Lot, ExpirationDate, CompanyID, RoomID);

                                    if (!IsLotAvailableForCredit)
                                    {
                                        objPullDetailDTO.AddedFrom = string.Format(msgCreditTransactionForLotAndExpDateAvailable, itmCredit.Lot, ExpirationDate.ToString(Convert.ToString(RoomDateFormat)));
                                        objPullDetailDTO.EditedFrom = "Fail";
                                        lstPullDetailDTO.Add(objPullDetailDTO);
                                        continue;
                                    }
                                }
                            }

                            objPullDetailDTO.PULLGUID = Guid.Empty;
                            objPullDetailDTO.Room = RoomID;
                            objPullDetailDTO.CompanyID = CompanyID;
                            objPullDetailDTO.ItemGUID = objItemMaster.GUID;
                            objPullDetailDTO.ItemNumber = objItemMaster.ItemNumber;
                            objPullDetailDTO.ProjectSpendName = itemToCredit.ProjectName;
                            objPullDetailDTO.ItemCost = (objItemMaster.SellPrice
                                                        / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                            ? objItemMaster.CostUOMValue : 1));
                            objPullDetailDTO.ItemPrice = (objItemMaster.Cost
                                                        / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                            ? objItemMaster.CostUOMValue : 1));

                            objPullDetailDTO.PullCredit = "pull";
                            if (objItemMaster.Consignment)
                                objPullDetailDTO.ConsignedQuantity = itmCredit.Qty;
                            else
                                objPullDetailDTO.CustomerOwnedQuantity = itmCredit.Qty;

                            if (objItemMaster.SerialNumberTracking && objItemMaster.DateCodeTracking)
                            {
                                objPullDetailDTO.SerialNumber = itmCredit.Serial;
                                objPullDetailDTO.Expiration = itmCredit.ExpireDate;
                            }
                            if (objItemMaster.LotNumberTracking && objItemMaster.DateCodeTracking)
                            {
                                objPullDetailDTO.LotNumber = itmCredit.Lot;
                                objPullDetailDTO.Expiration = itmCredit.ExpireDate;
                            }
                            if (objItemMaster.SerialNumberTracking)
                            {
                                objPullDetailDTO.SerialNumber = itmCredit.Serial;
                            }
                            if (objItemMaster.LotNumberTracking)
                            {
                                objPullDetailDTO.LotNumber = itmCredit.Lot;
                            }
                            if (objItemMaster.DateCodeTracking)
                            {
                                objPullDetailDTO.Expiration = itmCredit.ExpireDate;
                            }
                            if (!string.IsNullOrEmpty(itemToCredit.Bin))
                                objPullDetailDTO.CreditBinName = itemToCredit.Bin;

                            if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                                objPullDetailDTO.CreditProjectName = itemToCredit.ProjectName;

                            objPullDetailDTO.CreditUDF1 = itemToCredit.UDF1;
                            objPullDetailDTO.CreditUDF2 = itemToCredit.UDF2;
                            objPullDetailDTO.CreditUDF3 = itemToCredit.UDF3;
                            objPullDetailDTO.CreditUDF4 = itemToCredit.UDF4;
                            objPullDetailDTO.CreditUDF5 = itemToCredit.UDF5;
                            objPullDetailDTO.WorkOrderGUID = itemToCredit.WOGuid;
                            objPullDetailDTO.PullOrderNumber = itemToCredit.PullOrderNumber;
                            objPullDetailDTO.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                            if (String.IsNullOrWhiteSpace(itemToCredit.EditedFrom))
                            {
                                objPullDetailDTO.AddedFrom = "Web";
                                objPullDetailDTO.EditedFrom = "Web";
                            }
                            else
                            {
                                objPullDetailDTO.AddedFrom = itemToCredit.EditedFrom;
                                objPullDetailDTO.EditedFrom = itemToCredit.EditedFrom;
                            }
                            lstPullDetailDTO.Add(objPullDetailDTO);
                        }
                    }
                }

                #endregion

                else if (itemToCredit.ItemTracking != "QUICKLIST")
                {
                    if (objItemMaster != null)
                    {
                        PullDetailsDTO objPullDetailDTO = new PullDetailsDTO();
                        objPullDetailDTO.PULLGUID = Guid.Empty;
                        objPullDetailDTO.Room = RoomID;
                        objPullDetailDTO.CompanyID = CompanyID;
                        objPullDetailDTO.ItemGUID = objItemMaster.GUID;
                        objPullDetailDTO.ItemNumber = objItemMaster.ItemNumber;
                        objPullDetailDTO.ProjectSpendName = itemToCredit.ProjectName;
                        objPullDetailDTO.ItemCost = (objItemMaster.SellPrice
                                                    / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                        ? objItemMaster.CostUOMValue : 1));
                        objPullDetailDTO.ItemPrice = (objItemMaster.Cost
                                                    / ((objItemMaster.CostUOMValue != null && objItemMaster.CostUOMValue > 0)
                                                        ? objItemMaster.CostUOMValue : 1));

                        objPullDetailDTO.PullCredit = "pull";
                        if (objItemMaster.Consignment)
                            objPullDetailDTO.ConsignedQuantity = RemainingCreditQuantity;
                        else
                            objPullDetailDTO.CustomerOwnedQuantity = RemainingCreditQuantity;

                        if (!string.IsNullOrEmpty(itemToCredit.Bin))
                            objPullDetailDTO.CreditBinName = itemToCredit.Bin;

                        if (!string.IsNullOrEmpty(itemToCredit.ProjectName))
                            objPullDetailDTO.CreditProjectName = itemToCredit.ProjectName;

                        objPullDetailDTO.CreditUDF1 = itemToCredit.UDF1;
                        objPullDetailDTO.CreditUDF2 = itemToCredit.UDF2;
                        objPullDetailDTO.CreditUDF3 = itemToCredit.UDF3;
                        objPullDetailDTO.CreditUDF4 = itemToCredit.UDF4;
                        objPullDetailDTO.CreditUDF5 = itemToCredit.UDF5;
                        objPullDetailDTO.WorkOrderGUID = itemToCredit.WOGuid;
                        objPullDetailDTO.PullOrderNumber = itemToCredit.PullOrderNumber;
                        objPullDetailDTO.SupplierAccountGuid = itemToCredit.SupplierAccountGuid;
                        if (String.IsNullOrWhiteSpace(itemToCredit.EditedFrom))
                        {
                            objPullDetailDTO.AddedFrom = "Web";
                            objPullDetailDTO.EditedFrom = "Web";
                        }
                        else
                        {
                            objPullDetailDTO.AddedFrom = itemToCredit.EditedFrom;
                            objPullDetailDTO.EditedFrom = itemToCredit.EditedFrom;
                        }
                        lstPullDetailDTO.Add(objPullDetailDTO);
                    }
                }

                return lstPullDetailDTO;
            }
            finally
            {
            }
        }

        public void InsertintoCreditHistory(List<PullDetailsDTO> pullDetails, long RoomID, long CompanyID, long UserID, string WhatWhereAction = "")
        {
            PullDetailsDAL pullDetailDAL = new PullDetailsDAL(base.DataBaseName);

            foreach (var item in pullDetails)
            {
                #region Entry into Credit History Table

                PullCreditHistoryDTO PullCreditHistoryDtl = new PullCreditHistoryDTO();
                PullCreditHistoryDtl.GUID = Guid.NewGuid();
                PullCreditHistoryDtl.PullDetailGuid = Guid.Empty;
                PullCreditHistoryDtl.PULLGUID = Guid.Empty;
                PullCreditHistoryDtl.ItemGUID = item.ItemGUID.GetValueOrDefault(Guid.Empty);
                PullCreditHistoryDtl.CreditCustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                PullCreditHistoryDtl.CreditConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                PullCreditHistoryDtl.CompanyID = item.CompanyID.GetValueOrDefault(0);
                PullCreditHistoryDtl.Room = item.Room.GetValueOrDefault(0);
                PullCreditHistoryDtl.Created = DateTimeUtility.DateTimeNow;
                PullCreditHistoryDtl.Updated = DateTimeUtility.DateTimeNow;
                PullCreditHistoryDtl.Created = DateTimeUtility.DateTimeNow;
                PullCreditHistoryDtl.Updated = DateTimeUtility.DateTimeNow;
                PullCreditHistoryDtl.CreatedBy = UserID;
                PullCreditHistoryDtl.LastUpdatedBy = UserID;
                PullCreditHistoryDtl.IsDeleted = false;
                PullCreditHistoryDtl.IsArchived = false;

                if (string.IsNullOrWhiteSpace(item.EditedFrom))
                {
                    PullCreditHistoryDtl.AddedFrom = "Web";
                    PullCreditHistoryDtl.EditedFrom = "Web";
                }
                else
                {
                    PullCreditHistoryDtl.AddedFrom = item.EditedFrom;
                    PullCreditHistoryDtl.EditedFrom = item.EditedFrom;
                }
                PullCreditHistoryDtl.WhatWhereAction = WhatWhereAction;
                PullCreditHistoryDtl.CreditGuid = (item.PULLGUID != null && item.PULLGUID != Guid.Empty) ? item.PULLGUID : Guid.Empty;
                PullCreditHistoryDtl.CreditDetailGuid = (item.GUID != null && item.GUID != Guid.Empty) ? item.GUID : Guid.Empty;
                pullDetailDAL.InsertCreditHistory(PullCreditHistoryDtl);

                #endregion
            }

        }

        public void InsertintoCreditHistoryForCount(long RoomID, long CompanyID, long UserID, Guid AppliedCountGuid, Guid AppliedCountDetailGuid, string WhatWhereAction = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@AppliedCountGuid", AppliedCountGuid),
                                                   new SqlParameter("@AppliedCountDetailGuid", AppliedCountDetailGuid) };


                List<PullDetailsDTO> lstCountCreditDetails = context.Database.SqlQuery<PullDetailsDTO>("EXEC [GetCountCreditDetailsForCreditHistory] @RoomID,@CompanyID,@AppliedCountGuid,@AppliedCountDetailGuid", paramA).ToList();

                if (lstCountCreditDetails != null && lstCountCreditDetails.Count > 0)
                {
                    InsertintoCreditHistory(lstCountCreditDetails, RoomID, CompanyID, UserID, WhatWhereAction);
                }
            }
        }

        #endregion  

        public List<PullMasterViewDTO> GetPullDetailHistoryByPullGUIDNormal(string PullGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@PullGUID", PullGUID) };
                return context.Database.SqlQuery<PullMasterViewDTO>("exec [GetPullDetailHistoryByPullGUIDNormal] @RoomID,@CompanyID,@PullGUID", params1).ToList();
            }
        }

        public List<NarrowSearchDTO> GetPullMasterListNarrowSearch(long RoomID, long CompanyID, bool IsArchived,
            bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed,
            int LoadDataCount, long UserID)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@UserId", UserID),
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var list = context.Database.SqlQuery<NarrowSearchDTO>("exec [GetPullListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@UserId", params1).ToList();
                return list;
            }
        }

        #region New Pull List Narrow Search

        public List<NarrowSearchDTO> GetNewPullNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount,string requestFor)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@RequestFor", requestFor),
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetNewPullNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@RequestFor", params1).ToList();
            }
        }
        public List<NarrowSearchDTO> GetNegativePullNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount,string requestFor)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@RequestFor", requestFor)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetNegativePullNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@RequestFor", params1).ToList();
            }
        }

        public List<NarrowSearchDTO> GetNewCreditNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, string ActionType, int LoadDataCount)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@ActionType", ActionType),
                                                new SqlParameter("@LoadDataCount", LoadDataCount)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<NarrowSearchDTO> lst = new List<NarrowSearchDTO>();
                lst = context.Database.SqlQuery<NarrowSearchDTO>("exec [GetNewCreditNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@ActionType,@LoadDataCount", params1).ToList();
                return lst;
            }
        }

        #endregion

        /// <summary>
        /// Get Paged Records from the PullMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        /// Added by amit t on 8-apr-20
        public IEnumerable<PullMasterViewDTO> GetAllWorkOrderRecordsWithPaging(long RoomID, long CompanyId, string WorkOrderGUID
            , int StartRowIndex, int MaxRows, string SortColumnName, string SearchTerm,bool IsArchieved, out long totalRecords)
        {
            totalRecords = 0;
            List<PullMasterViewDTO> obj = new List<PullMasterViewDTO>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            //DataSet dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetWorkOrderRecords", RoomID, CompanyId, WorkOrderGUID);
            string SPNAME = IsArchieved ? "GetWorkOrderRecordsWithPaging_Archieve" : "GetWorkOrderRecordsWithPaging";
            DataSet dsCart = SqlHelper.ExecuteDataset(EturnsConnection, SPNAME , RoomID,
                CompanyId, WorkOrderGUID, StartRowIndex, MaxRows, SortColumnName, SearchTerm ?? "");

            DataTable dtCart = dsCart.Tables[0];

            if (dtCart.Rows.Count > 0)
            {
                totalRecords = Convert.ToInt64(dtCart.Rows[0]["TotalRecords"]);

                obj = dtCart.AsEnumerable()
                  .Select(row => new PullMasterViewDTO
                  {
                      ID = row.Field<long>("ID"),
                      ProjectSpendGUID = row.Field<Guid?>("ProjectSpendGUID"),
                      ProjectName = row.Field<string>("ProjectSpendName"),
                      ProjectSpendName = row.Field<string>("ProjectSpendName"),
                      BinNumber = ((row.Field<string>("BinNumber") ?? string.Empty).Equals("[|EmptyStagingBin|]") ? string.Empty : row.Field<string>("BinNumber")),
                      DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity").GetValueOrDefault(0),
                      OnHandQuantity = row.Field<double?>("OnHandQuantity").GetValueOrDefault(0),
                      ItemGUID = row.Field<Guid?>("ItemGUID"),
                      ItemNumber = row.Field<string>("ItemNumber"),
                      CustomerOwnedQuantity = row.Field<double?>("CustomerOwnedQuantity").GetValueOrDefault(0),
                      ConsignedQuantity = row.Field<double?>("ConsignedQuantity").GetValueOrDefault(0),
                      CreditConsignedQuantity = row.Field<double?>("CreditConsignedQuantity").GetValueOrDefault(0),
                      CreditCustomerOwnedQuantity = row.Field<double?>("CreditCustomerOwnedQuantity").GetValueOrDefault(0),
                      PoolQuantity = row.Field<double?>("PoolQuantity").GetValueOrDefault(0),
                      PullOrderNumber = row.Field<string>("PullOrderNumber"),
                      PullCost = row.Field<double?>("PullCost").GetValueOrDefault(0),
                      SerialNumber = string.Empty,
                      LotNumber = string.Empty,
                      DateCode = string.Empty,
                      BinID = row.Field<long?>("BinID").GetValueOrDefault(0),
                      UDF1 = row.Field<string>("UDF1"),
                      UDF2 = row.Field<string>("UDF2"),
                      UDF3 = row.Field<string>("UDF3"),
                      UDF4 = row.Field<string>("UDF4"),
                      UDF5 = row.Field<string>("UDF5"),
                      ItemUDF1 = row.Field<string>("ITEMUDF1"),
                      ItemUDF2 = row.Field<string>("ITEMUDF2"),
                      ItemUDF3 = row.Field<string>("ITEMUDF3"),
                      ItemUDF4 = row.Field<string>("ITEMUDF4"),
                      ItemUDF5 = row.Field<string>("ITEMUDF5"),
                      ItemUDF6 = row.Field<string>("ItemUDF6"),
                      ItemUDF7 = row.Field<string>("ItemUDF7"),
                      ItemUDF8 = row.Field<string>("ItemUDF8"),
                      ItemUDF9 = row.Field<string>("ItemUDF9"),
                      ItemUDF10 = row.Field<string>("ItemUDF10"),
                      GUID = row.Field<Guid>("GUID"),
                      Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                      Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                      CreatedBy = row.Field<long?>("CreatedBy").GetValueOrDefault(0),
                      LastUpdatedBy = row.Field<long?>("LastUpdatedBy").GetValueOrDefault(0),
                      IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                      IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                      CompanyID = row.Field<long?>("CompanyID").GetValueOrDefault(0),
                      Room = row.Field<long?>("Room").GetValueOrDefault(0),
                      CreatedByName = row.Field<string>("CreatedByName"),
                      UpdatedByName = row.Field<string>("UpdatedByName"),
                      RoomName = row.Field<string>("RoomName"),
                      SupplierID = row.Field<long?>("SupplierID").GetValueOrDefault(0),
                      ManufacturerID = row.Field<long?>("ManufacturerID").GetValueOrDefault(0),
                      CategoryID = row.Field<long?>("CategoryID").GetValueOrDefault(0),
                      CategoryName = row.Field<string>("CategoryName"),
                      Markup = row.Field<double?>("Markup").GetValueOrDefault(0),
                      SellPrice = row.Field<double?>("SellPrice").GetValueOrDefault(0),
                      ItemCost = row.Field<double?>("ItemCost").GetValueOrDefault(0),// row.Field<bool>("Trend"),
                      Description = (row.Field<string>("Description")),
                      Unit = (row.Field<string>("Unit")),
                      PullCredit = row.Field<string>("PullCredit"),
                      ActionType = row.Field<string>("ActionType"),
                      ItemType = row.Field<int?>("ItemType"),
                      RequisitionDetailGUID = row.Field<Guid?>("RequisitionDetailGUID").GetValueOrDefault(Guid.Empty),
                      WorkOrderDetailGUID = row.Field<Guid?>("WorkOrderDetailGUID").GetValueOrDefault(Guid.Empty),
                      Billing = row.Field<bool?>("Billing").GetValueOrDefault(false),
                      CountLineItemGuid = row.Field<Guid?>("CountLineItemGuid").GetValueOrDefault(Guid.Empty),
                      PackingQuantity = row.Field<double?>("PackingQuantity").GetValueOrDefault(0),
                      Manufacturer = row.Field<string>("Manufacturer"),
                      ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                      SupplierName = row.Field<string>("SupplierName"),
                      SupplierPartNo = row.Field<string>("SupplierPartNo"),
                      AverageUsage = row.Field<double?>("AverageUsage").GetValueOrDefault(0),
                      CriticalQuantity = row.Field<double>("CriticalQuantity"),
                      GLAccount = row.Field<string>("GLAccount"),
                      InTransitquantity = row.Field<double?>("InTransitquantity").GetValueOrDefault(0),
                      IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool>("IsItemLevelMinMaxQtyRequired") : false), //row.Field<bool>("IsPurchase"),
                      MaximumQuantity = (row.Field<double>("MaximumQuantity")), //row.Field<bool>("IsTransfer"),
                      MinimumQuantity = row.Field<double>("MinimumQuantity"),
                      Turns = row.Field<double?>("Turns").GetValueOrDefault(0),
                      Taxable = row.Field<bool>("Taxable"),
                      OnOrderQuantity = (row.Field<double?>("OnOrderQuantity")).GetValueOrDefault(0),
                      OnTransferQuantity = (row.Field<double?>("OnTransferQuantity")).GetValueOrDefault(0),
                      Consignment = (row.Field<bool?>("Consignment").HasValue ? row.Field<bool>("Consignment") : false),
                      ItemOnhandQty = row.Field<double?>("ItemOnhandQty").GetValueOrDefault(0),
                      IsAddedFromPDA = row.Field<bool>("IsAddedFromPDA"),
                      IsProcessedAfterSync = row.Field<bool>("IsProcessedAfterSync"),
                      //for item grid display purpose - CART, PUll  
                      ItemStageQty = row.Field<double?>("ItemStageQty").GetValueOrDefault(0),
                      ItemStageLocationQty = row.Field<double?>("ItemStageLocationQty").GetValueOrDefault(0),
                      ItemLocationOnHandQty = row.Field<double?>("ItemLocationOnHandQty").GetValueOrDefault(0),
                      ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                      ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                      AddedFrom = row.Field<string>("AddedFrom"),
                      EditedFrom = row.Field<string>("EditedFrom"),
                      ExtendedCost = row.Field<double?>("ExtendedCost").GetValueOrDefault(0),
                      AverageCost = (row.Field<double?>("AverageCost")).GetValueOrDefault(0),
                      CostUOMName = (row.Field<string>("CostUOMName")),
                      ToolName = (row.Field<string>("ToolName")),
                      ToolGUID = (row.Field<Guid?>("ToolGUID")),
                      PullPrice = row.Field<double?>("PullPrice").GetValueOrDefault(0),
                      PullItemCost = row.Field<double?>("PullItemCost").GetValueOrDefault(0),
                      PullItemSellPrice = row.Field<double?>("PullItemSellPrice").GetValueOrDefault(0),
                      PullMarkup = row.Field<double?>("PullMarkup").GetValueOrDefault(0),
                      ItemCostOnPullDate = row.Field<double?>("ItemCostOnPullDate").GetValueOrDefault(0),
                      RowNum = row.Field<long>("RowNum"),
                      PullUDF1 = row.Field<string>("PullUDF1"),
                      PullUDF2 = row.Field<string>("PullUDF2"),
                      PullUDF3 = row.Field<string>("PullUDF3"),
                      PullUDF4 = row.Field<string>("PullUDF4"),
                      PullUDF5 = row.Field<string>("PullUDF5"),
                      ImagePath = row.Field<string>("ImagePath"),
                      ImageType = row.Field<string>("ImageType"),
                      ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                      ItemID = row.Field<long?>("ItemID"),
                  }).ToList();
            }
            return obj;
        }

        #region Material staging count - WI-4801

        public void InsertintoMSCreditHistoryForCount(long RoomID, long CompanyID, long UserID, Guid AppliedCountGuid, Guid AppliedCountDetailGuid, string WhatWhereAction = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@AppliedCountGuid", AppliedCountGuid),
                                                   new SqlParameter("@AppliedCountDetailGuid", AppliedCountDetailGuid) };


                List<PullDetailsDTO> lstCountCreditDetails = context.Database.SqlQuery<PullDetailsDTO>("EXEC [GetCountMSCreditDetailsForCreditHistory] @RoomID,@CompanyID,@AppliedCountGuid,@AppliedCountDetailGuid", paramA).ToList();

                if (lstCountCreditDetails != null && lstCountCreditDetails.Count > 0)
                {
                    InsertintoCreditHistory(lstCountCreditDetails, RoomID, CompanyID, UserID, WhatWhereAction);
                }
            }
        }

        #endregion

        #region WI-5566
        public List<SupplierAccountDetailsDTO> GetAllSupplierAccountNumbers(Guid ItemGuid, string NameStartWith, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@NameStartWith", NameStartWith) };
                return context.Database.SqlQuery<SupplierAccountDetailsDTO>("exec [GetAllSupplierAccountNumbers] @RoomID,@CompanyID,@ItemGuid,@NameStartWith", params1).ToList();
            }
        }
        #endregion

        #region 3055

        #region Pull

        public bool EditForPullQty(PullMasterViewDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            PullMaster obj;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = context.PullMasters.FirstOrDefault(t => t.ID == objDTO.ID);

                if (obj != null)
                {
                    obj.ID = objDTO.ID;
                    obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
                    obj.UOI = objDTO.UOI;
                    obj.ActionType = objDTO.ActionType;
                    obj.PullCredit = objDTO.PullCredit;
                    obj.PULLCost = objDTO.PullCost;
                    obj.PullPrice = objDTO.PullPrice;
                    obj.PoolQuantity = objDTO.PoolQuantity;
                    obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                    obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                    obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
                    obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
                    obj.ItemOnhandQty = objDTO.ItemOnhandQty;
                    obj.ItemStageQty = objDTO.ItemStageQty;
                    obj.ItemLocationOnhandQty = objDTO.ItemLocationOnHandQty;
                    obj.ItemStageLocationQty = objDTO.ItemStageLocationQty;
                    obj.BinID = objDTO.BinID;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.ControlNumber = objDTO.ControlNumber;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Pull";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;
                    context.SaveChanges();
                }

                return true;
            }
        }

        public bool UpdatePullQtyInPullHistory(PullMasterViewDTO objnewPullMasterData, PullMasterViewDTO objoldPullMasterData, ItemMasterDTO oItemRecord, int IsCreditPullNothing, Int64 RoomID, 
            Int64 CompanyID, bool isMorePull, long ModuleId, double itemCost, double itemPrice, out string ItemLocationMSG, bool IsProjectSpendAllowed, out bool IsPSLimitExceed, 
            string RoomDateFormat, long SessionUserId,long EnterpriseId,string CultureCode, bool AllowNegetive = false)
        {
            #region "Global Variables"
            ItemLocationMSG = "";
            IsPSLimitExceed = false;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
            ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
            PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO obj = new PullMasterViewDTO();
            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
            ItemLocationQTYDTO lstLocDTO1 = new ItemLocationQTYDTO();
            BinMasterDTO objBINDTO = new BinMasterDTO();
            List<ItemLocationDetailsDTO> ObjItemLocation = null;
            RoomDTO objRoomDTO = new RoomDTO();
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            PullDetailsDAL objPullDtlDal = new PullDetailsDAL(base.DataBaseName);
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);

            bool IsStagginLocation = false;
            //bool IsProjectSpendMandatoryPleaseSelect = false;         

            string columnList = "ID,RoomName,IsProjectSpendMandatory,InventoryConsuptionMethod,MethodOfValuingInventory";
            objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + Convert.ToString(RoomID) + "", "");

            BinMasterDAL objLocDAL = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO objLocDTO = objLocDAL.GetBinByID(objnewPullMasterData.BinID.GetValueOrDefault(0), RoomID, CompanyID);

            if (objLocDTO != null && objLocDTO.ID > 0)
            {
                if (objLocDTO.IsStagingLocation)
                {
                    IsStagginLocation = true;
                }
            }

            #endregion

            #region  For More Pull

            if (isMorePull)
            {
                if (IsStagginLocation)
                {
                    #region "Stagging Bin Wise Quantity Check"

                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        // if block is for WI-4665: allow staging pull if qty available at Staging header which is selected while creating the requisition.
                        double retval = 0;
                        List<MaterialStagingDetailDTO> lstMaterialStagingDetailDTO = new MaterialStagingDetailDAL(base.DataBaseName)
                                                            .GetMaterialStagingDetailByItemGUIDAndBinIDPlain(oItemRecord.GUID, objnewPullMasterData.BinID);
                        if (lstMaterialStagingDetailDTO != null && lstMaterialStagingDetailDTO.Count() > 0)
                        {
                            retval = lstMaterialStagingDetailDTO.Sum(t => t.Quantity);
                        }
                        if (retval < objnewPullMasterData.TempPullQTY)
                        {
                            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                            string msgNotEnoughQuantityForLocation = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantityForLocation", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                            ItemLocationMSG = string.Format(msgNotEnoughQuantityForLocation, objLocDTO.BinNumber, retval.ToString());
                            return true;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region "Bin Wise Quantity Check"
                    //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objoldPullMasterData.BinID && x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                    lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objoldPullMasterData.BinID, Convert.ToString(oItemRecord.GUID)).FirstOrDefault();
                    lstLocDTO1 = objLocationDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, RoomID, CompanyID, SessionUserId);
                    if (lstLocDTO == null && lstLocDTO1 != null && lstLocDTO1.Quantity > 0)
                    {
                        objLocQTY.Insert(lstLocDTO1, SessionUserId,EnterpriseId);
                        //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objoldPullMasterData.BinID && x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                        lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objoldPullMasterData.BinID, Convert.ToString(oItemRecord.GUID)).FirstOrDefault();
                    }

                    objBINDTO = null;
                    if (BinWiseQuantityCheck(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objnewPullMasterData, objBINDAL, oItemRecord,EnterpriseId,CultureCode))
                    {
                        return true;
                    }
                    #endregion
                }

                if (!IsStagginLocation)
                {
                    #region "LIFO FIFO"

                    Boolean IsFIFO = false;
                    if (objRoomDTO != null && objRoomDTO.ID > 0)
                    {
                        if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "fifo")
                            IsFIFO = true;
                        if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "lifo")
                            IsFIFO = false;
                    }
                    else
                    {
                        IsFIFO = true;
                    }

                    #endregion
                    ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objnewPullMasterData.BinID, RoomID, CompanyID, oItemRecord.GUID, null).ToList();
                    ObjItemLocation.ForEach(t => t.EditedFrom = "Web");


                    #region "Project Wise Quantity Check"

                    if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                    {
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objnewPullMasterData.ProjectSpendGUID.Value, RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(objnewPullMasterData.ProjectSpendGUID.Value, RoomID, CompanyID, tmpsupplierIds, Convert.ToString(oItemRecord.GUID)).FirstOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);
                        if (ProjectWiseQuantityCheck(objPrjSpenItmDTO, objPrjMstDTO, out ItemLocationMSG, objnewPullMasterData, oItemRecord, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, EnterpriseId, CultureCode,CompanyID,RoomID))
                        {
                            return true;
                        }
                    }
                    #endregion

                    #region "Pull Insert Update"                       
                    //ReturnDto = PullInsertUpdate(objnewPullMasterData, obj, TempOldBinID, objPullDal, SessionUserId);
                    #endregion

                    #region "Item Location & Quantity  Wise Deduction"

                    #region "ItemLocation Deduction"

                    #region "LOt and other type logic"

                    Double takenQunatity = 0;

                    foreach (var itemoil in ObjItemLocation)
                    {
                        Double loopCurrentTakenCustomer = 0;
                        Double loopCurrentTakenConsignment = 0;
                        /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                        if (takenQunatity == objnewPullMasterData.TempPullQTY)
                        {
                            break;
                        }

                        if (oItemRecord.Consignment)
                        {
                            #region "Consignment Credit and Pull"

                            #region Pull
                            //Both's sum we have available.
                            if (itemoil.CustQConsumable.GetValueOrDefault(0) == 0 && itemoil.CustQConsumable.GetValueOrDefault(0) >= (objnewPullMasterData.TempPullQTY - takenQunatity))
                            {
                                loopCurrentTakenConsignment = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                itemoil.ConsignedQuantity = (itemoil.ConsignedQuantity ?? 0) - ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                takenQunatity += (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                goto Save;
                            }
                            else if (itemoil.CustQConsumable.GetValueOrDefault(0) >= (objnewPullMasterData.TempPullQTY - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                takenQunatity += (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                goto Save;
                            }
                            else
                            {
                                takenQunatity += itemoil.CustQConsumable.GetValueOrDefault(0);
                                loopCurrentTakenCustomer = itemoil.CustQConsumable.GetValueOrDefault(0);
                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - itemoil.CustQConsumable;

                                if (itemoil.ConQConsumable >= ((Double)(objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                                {
                                    loopCurrentTakenConsignment = ((Double)(objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                    itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                    takenQunatity += (Double)(objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                    goto Save;
                                }
                                else
                                {
                                    loopCurrentTakenConsignment = itemoil.ConQConsumable.GetValueOrDefault(0);
                                    takenQunatity += itemoil.ConQConsumable.GetValueOrDefault(0);
                                    itemoil.ConsignedQuantity = itemoil.ConsignedQuantity - itemoil.ConQConsumable;
                                }
                                //PENDING -- loop by varialbe from mupliple locations...
                            }
                            #endregion
                            #endregion
                        }
                        else
                        {
                            #region "Customreowned Credit and Pull"

                            //Both's sum we have available.
                            if (itemoil.CustomerOwnedQuantity >= (objnewPullMasterData.TempPullQTY - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                itemoil.CustomerOwnedQuantity = (itemoil.CustomerOwnedQuantity ?? 0) - ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                takenQunatity += (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                goto Save;
                            }
                            else
                            {
                                loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
                            }
                            #endregion
                        }
                        Save:
                        objLocationDAL.Edit(itemoil);
                        itemoil.PullCredit = objnewPullMasterData.PullCredit;
                        obj = AddtoPullDetailForEdit(itemoil, objnewPullMasterData.GUID, objnewPullMasterData.ProjectSpendGUID != null ? objnewPullMasterData.ProjectSpendGUID.Value : objnewPullMasterData.ProjectSpendGUID, itemCost, objnewPullMasterData.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemPrice);
                    }

                    #endregion

                    #endregion

                    #region "ItemLocation Quantity Deduction"
                    oItemRecord.OnHandQuantity = oItemRecord.OnHandQuantity - objnewPullMasterData.TempPullQTY;

                    if (oItemRecord.Consignment)
                    {
                        //Both's sum we have available.
                        if (lstLocDTO != null)
                        {
                            if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                            {
                                obj.ConsignedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - (objnewPullMasterData.TempPullQTY ?? 0);
                                lstLocDTO.Quantity = lstLocDTO.Quantity - (objnewPullMasterData.TempPullQTY ?? 0);
                            }
                            else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objnewPullMasterData.TempPullQTY ?? 0))
                            {
                                obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - (objnewPullMasterData.TempPullQTY ?? 0);
                                lstLocDTO.Quantity = lstLocDTO.Quantity - (objnewPullMasterData.TempPullQTY ?? 0);
                            }
                            else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < (objnewPullMasterData.TempPullQTY ?? 0))
                            {
                                double cstqty = (objnewPullMasterData.TempPullQTY ?? 0) - lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                double consqty = cstqty;
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
                                obj.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                obj.ConsignedQuantity = consqty;
                                lstLocDTO.CustomerOwnedQuantity = 0;
                                lstLocDTO.Quantity = lstLocDTO.Quantity - (obj.CustomerOwnedQuantity.GetValueOrDefault(0) + obj.ConsignedQuantity.GetValueOrDefault(0));
                            }
                        }
                    }
                    else
                    {
                        if (lstLocDTO != null)
                        {
                            lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objnewPullMasterData.TempPullQTY.GetValueOrDefault(0);
                            lstLocDTO.Quantity = lstLocDTO.Quantity - (objnewPullMasterData.TempPullQTY ?? 0);
                        }
                        obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                    }
                    #endregion

                    #region "Saving Location and QTY data"                       

                    oItemRecord.WhatWhereAction = objnewPullMasterData.PullCredit;
                    objItemDAL.Edit(oItemRecord, SessionUserId,EnterpriseId);
                    List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                    if (lstLocDTO == null)
                    {
                        lstLocDTO = new ItemLocationQTYDTO();
                        lstLocDTO.ID = 0;
                        lstLocDTO.BinID = objnewPullMasterData.BinID.GetValueOrDefault(0);
                        lstLocDTO.Quantity = objnewPullMasterData.PoolQuantity.GetValueOrDefault();
                        lstLocDTO.CustomerOwnedQuantity = objnewPullMasterData.CustomerOwnedQuantity;
                        lstLocDTO.ConsignedQuantity = objnewPullMasterData.ConsignedQuantity;
                        lstLocDTO.LotNumber = objnewPullMasterData.LotNumber;
                        lstLocDTO.GUID = Guid.NewGuid();
                        lstLocDTO.ItemGUID = oItemRecord.GUID;
                        lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                        lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        lstLocDTO.CreatedBy = SessionUserId;
                        lstLocDTO.LastUpdatedBy = SessionUserId;
                        lstLocDTO.Room = RoomID;
                        lstLocDTO.CompanyID = CompanyID;
                        lstLocDTO.AddedFrom = "Web";
                        lstLocDTO.EditedFrom = "Web";
                        lstLocDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        lstLocDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objLocQTY.Insert(lstLocDTO, SessionUserId,EnterpriseId);
                    }
                    lstUpdate.Add(lstLocDTO);
                    objLocQTY.Save(lstUpdate, SessionUserId,EnterpriseId);
                    #endregion

                    #endregion

                    #region "Project Spend Quantity Update and PullMaster Update"

                    if (obj != null && (obj.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 || obj.ConsignedQuantity.GetValueOrDefault(0) == 0))
                    {
                        List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                        if (lstPullDtl != null && lstPullDtl.Count > 0)
                        {
                            double OldPullCost = obj.PullCost ?? 0;
                            double OldPullQuantity = obj.PoolQuantity ?? 0;

                            obj.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                            obj.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                            obj.PoolQuantity = (
                                                lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                    +
                                                lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                );

                            obj.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                            obj.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));

                            obj.WhatWhereAction = objnewPullMasterData.WhatWhereAction;

                            obj.ItemOnhandQty = oItemRecord.OnHandQuantity;
                            obj.ItemStageQty = oItemRecord.StagedQuantity;
                            obj.ItemLocationOnHandQty = 0;

                            ItemLocationQTYDTO objItemLocationQuantity = objLocationDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, RoomID, CompanyID, SessionUserId);
                            if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                            {
                                obj.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                            }

                            objPullDal.EditForPullQty(obj);
                            objPullDal.InsertPullEditHistory(obj.GUID, obj.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, objnewPullMasterData.WhatWhereAction);

                            double DiffPullCost = (OldPullCost - (obj.PullCost ?? 0));
                            double DiffPoolQuantity = (OldPullQuantity - (obj.PoolQuantity ?? 0));

                            if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                UpdateProjectSpendWithCostEditPull(oItemRecord, obj, DiffPullCost, DiffPoolQuantity, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region For Staging location

                    List<MaterialStagingPullDetailDTO> ObjItemLocationMS = null;
                    MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                    MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    List<MaterialStagingDetailDTO> lstLocDTOMS = new List<MaterialStagingDetailDTO>();
                    List<MaterialStagingPullDetailDTO> lstPullDetailsDTO = objMaterialStagingPullDetailDAL.GetAvialableMsPullDetailsByItemGUIDANDBinID(objnewPullMasterData.ItemGUID.GetValueOrDefault(Guid.Empty), objnewPullMasterData.BinID.GetValueOrDefault(0), RoomID, CompanyID).ToList();
                    string InventoryConsuptionMethod = string.Empty;

                    if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                    {
                        InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                    }

                    if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                        InventoryConsuptionMethod = "";

                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstPullDetailsDTO = lstPullDetailsDTO.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstPullDetailsDTO = lstPullDetailsDTO.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstPullDetailsDTO = lstPullDetailsDTO.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                    }

                    //Pick up the locations.....
                    //if (ItemDTO.SerialNumberTracking)
                    //{
                    //    ObjItemLocationMS = lstPullDetailsDTO.Take((int)objDTO.TempPullQTY).ToList();
                    //}
                    //else
                    //{
                    ObjItemLocationMS = lstPullDetailsDTO;
                    //}

                    #region "Project Wise Quantity Check"

                    if (objnewPullMasterData.ProjectSpendGUID != null)
                    {
                        var tmpsupplierIds = new List<long>();
                        //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objnewPullMasterData.ProjectSpendGUID.Value, RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == objnewPullMasterData.ItemGUID).FirstOrDefault();
                        objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(objnewPullMasterData.ProjectSpendGUID.Value, RoomID, CompanyID, tmpsupplierIds, Convert.ToString(objnewPullMasterData.ItemGUID)).FirstOrDefault();
                        objPrjMstDTO = objPrjMsgDAL.GetRecord(objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);

                        if (ProjectWiseQuantityCheck(objPrjSpenItmDTO, objPrjMstDTO, out ItemLocationMSG, objnewPullMasterData, oItemRecord, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, EnterpriseId, CultureCode,CompanyID,RoomID))
                        {
                            return true;
                        }
                    }
                    #endregion

                    #region "Pull Insert Update"

                    if (oItemRecord.Consignment == true)
                    {
                        objnewPullMasterData.ConsignedQuantity = objnewPullMasterData.PoolQuantity;
                        objnewPullMasterData.CustomerOwnedQuantity = 0;
                        obj.ConsignedQuantity = objnewPullMasterData.PoolQuantity;
                        obj.CustomerOwnedQuantity = 0;
                    }
                    else
                    {
                        objnewPullMasterData.ConsignedQuantity = 0;
                        objnewPullMasterData.CustomerOwnedQuantity = objnewPullMasterData.PoolQuantity;
                        obj.ConsignedQuantity = 0;
                        obj.CustomerOwnedQuantity = objnewPullMasterData.PoolQuantity;
                    }

                    obj.PoolQuantity = objnewPullMasterData.PoolQuantity;

                    if (objnewPullMasterData.ActionType == null || objnewPullMasterData.ActionType == "MS Pull")
                    {
                        obj.ActionType = "MS Pull";
                        obj.PullCredit = "MS Pull";
                        objnewPullMasterData.ActionType = "MS Pull";
                        objnewPullMasterData.PullCredit = "MS Pull";
                        //objnewPullMasterData.WhatWhereAction = "MS Pull";
                    }
                    else
                    {
                        obj.PullCredit = objnewPullMasterData.ActionType;
                        objnewPullMasterData.ActionType = objnewPullMasterData.ActionType;
                        objnewPullMasterData.PullCredit = objnewPullMasterData.ActionType;
                        //objnewPullMasterData.WhatWhereAction = objnewPullMasterData.ActionType;
                    }
                    //ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal, SessionUserId);
                    #endregion

                    #region "Item Location & Quantity  Wise Deduction"

                    #region "ItemLocation Deduction"

                    if (IsCreditPullNothing != 3)
                    {
                        #region "LOt and other type logic"

                        Double takenQunatity = 0;

                        foreach (var itemoil in ObjItemLocationMS)
                        {
                            Double loopCurrentTakenCustomer = 0;
                            Double loopCurrentTakenConsignment = 0;
                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                            if (takenQunatity == objnewPullMasterData.TempPullQTY)
                            {
                                break;
                            }

                            if (oItemRecord.Consignment)
                            {
                                #region "Consignment Credit and Pull"

                                //Both's sum we have available.
                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objnewPullMasterData.TempPullQTY - takenQunatity))
                                {
                                    loopCurrentTakenConsignment = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                    itemoil.ConsignedQuantity = (itemoil.ConsignedQuantity ?? 0) - ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                    takenQunatity += (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                    goto Save;
                                }
                                else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objnewPullMasterData.TempPullQTY - takenQunatity))
                                {
                                    loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                    itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                    takenQunatity += (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                    goto Save;
                                }
                                else
                                {
                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
                                                                       // needs to write logic for break down deduction from consigned or customer quantity location wise ...
                                    if (itemoil.ConsignedQuantity >= (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity)
                                    {
                                        loopCurrentTakenConsignment = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                        takenQunatity += (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                        goto Save;
                                    }
                                    else
                                    {
                                        loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                        takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                        itemoil.ConsignedQuantity = 0;
                                    }
                                    //PENDING -- loop by varialbe from mupliple locations...
                                }

                                #endregion
                            }
                            else
                            {
                                #region "Customreowned Credit and Pull"

                                //Both's sum we have available.
                                if (itemoil.CustomerOwnedQuantity >= (objnewPullMasterData.TempPullQTY - takenQunatity))
                                {
                                    loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                    itemoil.CustomerOwnedQuantity = (itemoil.CustomerOwnedQuantity ?? 0) - ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity);
                                    takenQunatity += (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;
                                    goto Save;
                                }
                                else
                                {
                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
                                }

                                #endregion
                            }
                            Save:
                            itemoil.PoolQuantity = (itemoil.ConsignedQuantity.GetValueOrDefault(0) + itemoil.CustomerOwnedQuantity.GetValueOrDefault(0));
                            itemoil.PullCredit = objnewPullMasterData.ActionType;
                            objMaterialStagingPullDetailDAL.Edit(itemoil);
                            obj = AddtoPullDetailForEdit(itemoil, objnewPullMasterData.GUID, objnewPullMasterData.ProjectSpendGUID != null ? objnewPullMasterData.ProjectSpendGUID.Value : objnewPullMasterData.ProjectSpendGUID, itemCost, objnewPullMasterData.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemPrice);

                            //MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(itemoil.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                            if (objmsddto != null)
                            {
                                objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
                                objMaterialStagingDetailDAL.Edit(objmsddto);
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #endregion

                    //Update started quantity...

                    oItemRecord.StagedQuantity = oItemRecord.StagedQuantity - objnewPullMasterData.TempPullQTY;
                    objMaterialStagingPullDetailDAL.UpdateStagedQuantity(oItemRecord.GUID, RoomID, CompanyID, SessionUserId,EnterpriseId);

                    //Updated PS

                    #region "Project Spend Quantity Update and PullMaster Update"

                    if (obj != null && (obj.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 || obj.ConsignedQuantity.GetValueOrDefault(0) == 0))
                    {
                        List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                        if (lstPullDtl != null && lstPullDtl.Count > 0)
                        {
                            double OldPullCost = obj.PullCost ?? 0;
                            double OldPullQuantity = obj.PoolQuantity ?? 0;

                            obj.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                            obj.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                            obj.PoolQuantity = (
                                                lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                    +
                                                lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                );

                            obj.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                            obj.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));

                            obj.WhatWhereAction = objnewPullMasterData.WhatWhereAction;

                            obj.ItemOnhandQty = oItemRecord.OnHandQuantity;
                            obj.ItemStageQty = oItemRecord.StagedQuantity;
                            obj.ItemStageLocationQty = 0;

                            MaterialStagingPullDetailDTO objItemLocationStageQuantity = objMaterialStagingPullDetailDAL.GetItemStagingQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, RoomID, CompanyID, SessionUserId);
                            if (objItemLocationStageQuantity != null && objItemLocationStageQuantity.StagingBinId > 0)
                            {
                                obj.ItemStageLocationQty = objItemLocationStageQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationStageQuantity.ConsignedQuantity.GetValueOrDefault(0);
                            }

                            objPullDal.EditForPullQty(obj);
                            objPullDal.InsertPullEditHistory(obj.GUID, obj.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, objnewPullMasterData.WhatWhereAction);

                            //objPullDal.Edit(obj);

                            double DiffPullCost = (OldPullCost - (obj.PullCost ?? 0));
                            double DiffPoolQuantity = (OldPullQuantity - (obj.PoolQuantity ?? 0));

                            if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                UpdateProjectSpendWithCostEditPull(oItemRecord, obj, DiffPullCost, DiffPoolQuantity, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
                            }
                        }
                    }
                    #endregion

                    #endregion
                }
            }

            #endregion

            #region For Less Pull

            else
            {
                List<PullDetailsDTO> lstPullDetails = new List<PullDetailsDTO>();

                lstPullDetails = objPullDtlDal.GetPullDetailsByPullGuid(objnewPullMasterData.GUID, RoomID, CompanyID).ToList();
                lstPullDetails.ForEach(t => t.EditedFrom = "Web");

                if (!IsStagginLocation)
                {
                    #region "Bin Wise Quantity Check"
                    //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objoldPullMasterData.BinID && x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                    lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objoldPullMasterData.BinID, Convert.ToString(oItemRecord.GUID)).FirstOrDefault();
                    lstLocDTO1 = objLocationDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, RoomID, CompanyID, SessionUserId);
                    if (lstLocDTO == null && lstLocDTO1 != null && lstLocDTO1.Quantity > 0)
                    {
                        objLocQTY.Insert(lstLocDTO1, SessionUserId,EnterpriseId);
                        //lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objoldPullMasterData.BinID && x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                        lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, objoldPullMasterData.BinID, Convert.ToString(oItemRecord.GUID)).FirstOrDefault();
                    }
                    //objBINDTO = null;
                    //if (BinWiseQuantityCheck(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objnewPullMasterData, objBINDAL, oItemRecord))
                    //{
                    //    return true;
                    //}
                    #endregion

                    #region "Project Wise Quantity Check"

                    //if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                    //{
                    //    var tmpsupplierIds = new List<long>();
                    //    objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objnewPullMasterData.ProjectSpendGUID.Value, RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                    //    objPrjMstDTO = objPrjMsgDAL.GetRecord(objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false);
                    //    if (ProjectWiseQuantityCheck(objPrjMsgDAL, objPrjSpenItmDTO, objPrjMstDTO, objPrjSpenItmDAL, 2, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objnewPullMasterData, objBINDAL, oItemRecord, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation))
                    //    {
                    //        return true;
                    //    }
                    //}
                    #endregion

                    #region "Item Location & Quantity  Wise Deduction"

                    #region "ItemLocation Deduction"

                    #region "LOt and other type logic"

                    Double takenQunatity = 0;

                    foreach (var itemPull in lstPullDetails)
                    {
                        bool IsNeedToEdit = false;
                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objLocationDAL.GetItemLocationDetailsByLocationGuid(itemPull.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                        Double loopCurrentTakenCustomer = 0;
                        Double loopCurrentTakenConsignment = 0;

                        if (oItemRecord.Consignment)
                        {
                            #region "Consignment Credit and Pull"

                            if (itemPull.ConsignedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                            {
                                loopCurrentTakenConsignment = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                objItemLocationDetailsDTO.ConsignedQuantity += ((itemPull.ConsignedQuantity ?? 0) - loopCurrentTakenConsignment);

                                itemPull.ConsignedQuantity = loopCurrentTakenConsignment;
                                takenQunatity += loopCurrentTakenConsignment;

                                IsNeedToEdit = true;
                                goto Save;
                            }
                            else
                            {
                                loopCurrentTakenConsignment = itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                itemPull.ConsignedQuantity = loopCurrentTakenConsignment;
                                takenQunatity += itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                IsNeedToEdit = false;
                            }
                            #endregion
                        }
                        else
                        {
                            #region "Customreowned Credit and Pull"

                            if (itemPull.CustomerOwnedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                objItemLocationDetailsDTO.CustomerOwnedQuantity += ((itemPull.CustomerOwnedQuantity ?? 0) - loopCurrentTakenCustomer);

                                itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                takenQunatity += loopCurrentTakenCustomer;
                                IsNeedToEdit = true;
                                goto Save;
                            }
                            else
                            {
                                loopCurrentTakenCustomer = itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                takenQunatity += itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                IsNeedToEdit = false;
                            }
                            #endregion
                        }
                        Save:
                        if (IsNeedToEdit)
                        {
                            objLocationDAL.Edit(objItemLocationDetailsDTO);
                            itemPull.PullCredit = objnewPullMasterData.PullCredit;
                            obj = UpdatetoPullDetail(itemPull, objnewPullMasterData.GUID, objnewPullMasterData.ProjectSpendGUID != null ? objnewPullMasterData.ProjectSpendGUID.Value : objnewPullMasterData.ProjectSpendGUID, itemPull.ItemCost, objnewPullMasterData.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemPull.ItemPrice);
                        }
                    }

                    #endregion

                    #endregion

                    #region "ItemLocation Quantity Deduction"

                    oItemRecord.OnHandQuantity = oItemRecord.OnHandQuantity + ((obj.ConsignedQuantity.GetValueOrDefault(0) + obj.CustomerOwnedQuantity.GetValueOrDefault(0)) - objnewPullMasterData.PoolQuantity.GetValueOrDefault(0));

                    if (oItemRecord.Consignment)
                    {
                        //Both's sum we have available.
                        if (lstLocDTO != null)
                        {
                            if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                            {
                                obj.ConsignedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + (objnewPullMasterData.TempPullQTY ?? 0);
                                lstLocDTO.Quantity = lstLocDTO.Quantity + (objnewPullMasterData.TempPullQTY ?? 0);
                            }
                            else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objnewPullMasterData.TempPullQTY ?? 0))
                            {
                                obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + (objnewPullMasterData.TempPullQTY ?? 0);
                                lstLocDTO.Quantity = lstLocDTO.Quantity + (objnewPullMasterData.TempPullQTY ?? 0);
                            }
                            else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < (objnewPullMasterData.TempPullQTY ?? 0))
                            {
                                double cstqty = (objnewPullMasterData.TempPullQTY ?? 0) + lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                double consqty = cstqty;
                                lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity + consqty;
                                obj.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                obj.ConsignedQuantity = consqty;
                                lstLocDTO.CustomerOwnedQuantity = 0;
                                lstLocDTO.Quantity = lstLocDTO.Quantity + (obj.CustomerOwnedQuantity.GetValueOrDefault(0) + obj.ConsignedQuantity.GetValueOrDefault(0));
                            }
                        }
                    }
                    else
                    {
                        if (lstLocDTO != null)
                        {
                            lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + objnewPullMasterData.TempPullQTY.GetValueOrDefault(0);
                            lstLocDTO.Quantity = lstLocDTO.Quantity + (objnewPullMasterData.TempPullQTY ?? 0);
                        }
                        obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                    }

                    #endregion

                    #region "Saving Location and QTY data"                       

                    oItemRecord.WhatWhereAction = objnewPullMasterData.PullCredit;
                    objItemDAL.Edit(oItemRecord, SessionUserId,EnterpriseId);
                    List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                    if (lstLocDTO == null || lstLocDTO.ID == 0)
                    {
                        lstLocDTO = new ItemLocationQTYDTO();
                        lstLocDTO.ID = 0;
                        lstLocDTO.BinID = objnewPullMasterData.BinID.GetValueOrDefault(0);
                        lstLocDTO.Quantity = objnewPullMasterData.PoolQuantity.GetValueOrDefault();
                        lstLocDTO.CustomerOwnedQuantity = objnewPullMasterData.CustomerOwnedQuantity;
                        lstLocDTO.ConsignedQuantity = objnewPullMasterData.ConsignedQuantity;
                        lstLocDTO.LotNumber = objnewPullMasterData.LotNumber;
                        lstLocDTO.GUID = Guid.NewGuid();
                        lstLocDTO.ItemGUID = oItemRecord.GUID;
                        lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                        lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        lstLocDTO.CreatedBy = SessionUserId;
                        lstLocDTO.LastUpdatedBy = SessionUserId;
                        lstLocDTO.Room = RoomID;
                        lstLocDTO.CompanyID = CompanyID;
                        lstLocDTO.AddedFrom = "Web";
                        lstLocDTO.EditedFrom = "Web";
                        lstLocDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        lstLocDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objLocQTY.Insert(lstLocDTO, SessionUserId,EnterpriseId);
                    }
                    lstUpdate.Add(lstLocDTO);
                    objLocQTY.Save(lstUpdate, SessionUserId, EnterpriseId);
                    #endregion

                    #endregion

                    #region "Project Spend Quantity Update"

                    //if (objPullDal.Edit(obj))
                    //{

                    //}
                    if (obj != null && (obj.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 || obj.ConsignedQuantity.GetValueOrDefault(0) == 0))
                    {
                        List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                        if (lstPullDtl != null && lstPullDtl.Count > 0)
                        {
                            double OldPullCost = obj.PullCost ?? 0;
                            double OldPullQuantity = obj.PoolQuantity ?? 0;

                            obj.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                            obj.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                            obj.PoolQuantity = (
                                                lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                    +
                                                lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                );
                            obj.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                            obj.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));
                            obj.WhatWhereAction = objnewPullMasterData.WhatWhereAction;

                            obj.ItemOnhandQty = oItemRecord.OnHandQuantity;
                            obj.ItemStageQty = oItemRecord.StagedQuantity;
                            obj.ItemLocationOnHandQty = 0;

                            ItemLocationQTYDTO objItemLocationQuantity = objLocationDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, RoomID, CompanyID, SessionUserId);
                            if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                            {
                                obj.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                            }

                            objPullDal.EditForPullQty(obj);
                            objPullDal.InsertPullEditHistory(obj.GUID, obj.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, objnewPullMasterData.WhatWhereAction);
                            //objPullDal.Edit(obj);

                            double DiffPullCost = (OldPullCost - (obj.PullCost ?? 0));
                            double DiffPoolQuantity = (OldPullQuantity - (obj.PoolQuantity ?? 0));

                            if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                UpdateProjectSpendWithCostEditPull(oItemRecord, obj, DiffPullCost, DiffPoolQuantity, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);

                    #region "Item Location & Quantity  Wise Deduction"

                    #region "ItemLocation Deduction"

                    #region "LOt and other type logic"

                    Double takenQunatity = 0;

                    foreach (var itemPull in lstPullDetails)
                    {
                        double OldpullQty = itemPull.PoolQuantity.GetValueOrDefault(0);
                        double NewpullQty = 0;

                        bool IsNeedToEdit = false;
                        MaterialStagingPullDetailDTO objMaterialStagingPullDetailDTO = objMaterialStagingPullDetailDAL.GetRecord(itemPull.MaterialStagingPullDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);

                        Double loopCurrentTakenCustomer = 0;
                        Double loopCurrentTakenConsignment = 0;

                        if (oItemRecord.Consignment)
                        {
                            #region "Consignment Credit and Pull"

                            if (itemPull.ConsignedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                            {
                                loopCurrentTakenConsignment = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                objMaterialStagingPullDetailDTO.ConsignedQuantity += ((itemPull.ConsignedQuantity ?? 0) - loopCurrentTakenConsignment);

                                itemPull.ConsignedQuantity = loopCurrentTakenConsignment;
                                takenQunatity += loopCurrentTakenConsignment;

                                IsNeedToEdit = true;
                                goto Save;
                            }
                            else
                            {
                                loopCurrentTakenConsignment = itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                itemPull.ConsignedQuantity = loopCurrentTakenConsignment;
                                takenQunatity += itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                IsNeedToEdit = false;
                            }
                            #endregion
                        }
                        else
                        {
                            #region "Customreowned Credit and Pull"

                            if (itemPull.CustomerOwnedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                            {
                                loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                objMaterialStagingPullDetailDTO.CustomerOwnedQuantity += ((itemPull.CustomerOwnedQuantity ?? 0) - loopCurrentTakenCustomer);

                                itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                takenQunatity += loopCurrentTakenCustomer;
                                IsNeedToEdit = true;
                                goto Save;
                            }
                            else
                            {
                                loopCurrentTakenCustomer = itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                takenQunatity += itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                IsNeedToEdit = false;
                            }
                            #endregion
                        }
                        Save:
                        if (IsNeedToEdit)
                        {
                            objMaterialStagingPullDetailDTO.PoolQuantity = (objMaterialStagingPullDetailDTO.ConsignedQuantity.GetValueOrDefault(0) + objMaterialStagingPullDetailDTO.CustomerOwnedQuantity.GetValueOrDefault(0));
                            objMaterialStagingPullDetailDAL.Edit(objMaterialStagingPullDetailDTO);
                            itemPull.PullCredit = objnewPullMasterData.PullCredit;
                            obj = UpdatetoMSPullDetail(itemPull, objnewPullMasterData.GUID, objnewPullMasterData.ProjectSpendGUID != null ? objnewPullMasterData.ProjectSpendGUID.Value : objnewPullMasterData.ProjectSpendGUID, itemPull.ItemCost, objnewPullMasterData.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemPull.ItemPrice);

                            NewpullQty = (itemPull.CustomerOwnedQuantity.GetValueOrDefault(0) + itemPull.ConsignedQuantity.GetValueOrDefault(0));

                            //MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(objMaterialStagingPullDetailDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(objMaterialStagingPullDetailDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                            if (objmsddto != null)
                            {
                                objmsddto.Quantity = objmsddto.Quantity + (OldpullQty - NewpullQty);
                                objMaterialStagingDetailDAL.Edit(objmsddto);
                            }
                        }
                    }

                    #endregion

                    #endregion

                    #region "ItemLocation Quantity Deduction"                    

                    if (oItemRecord.Consignment)
                    {
                        oItemRecord.StagedQuantity = oItemRecord.StagedQuantity + (objnewPullMasterData.ConsignedQuantity.GetValueOrDefault(0) - objnewPullMasterData.TempPullQTY);
                        obj.ConsignedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                    }
                    else
                    {
                        oItemRecord.StagedQuantity = oItemRecord.StagedQuantity + (objnewPullMasterData.CustomerOwnedQuantity.GetValueOrDefault(0) - objnewPullMasterData.TempPullQTY);
                        obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                    }

                    #endregion

                    //Update started quantity...
                    oItemRecord.StagedQuantity = oItemRecord.StagedQuantity + ((obj.ConsignedQuantity.GetValueOrDefault(0) + obj.CustomerOwnedQuantity.GetValueOrDefault(0)) - objnewPullMasterData.PoolQuantity.GetValueOrDefault(0));
                    objMaterialStagingPullDetailDAL.UpdateStagedQuantity(oItemRecord.GUID, RoomID, CompanyID, SessionUserId,EnterpriseId);
                    //Updated PS

                    #endregion

                    #region "Project Spend Quantity Update"

                    if (obj != null && (obj.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 || obj.ConsignedQuantity.GetValueOrDefault(0) == 0))
                    {
                        List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                        if (lstPullDtl != null && lstPullDtl.Count > 0)
                        {
                            double OldPullCost = obj.PullCost ?? 0;
                            double OldPullQuantity = obj.PoolQuantity ?? 0;

                            obj.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                            obj.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                            obj.PoolQuantity = (
                                                lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                    +
                                                lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                );
                            obj.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                            obj.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));
                            obj.WhatWhereAction = objnewPullMasterData.WhatWhereAction;

                            obj.ItemOnhandQty = oItemRecord.OnHandQuantity;
                            obj.ItemStageQty = oItemRecord.StagedQuantity;
                            obj.ItemStageLocationQty = 0;

                            MaterialStagingPullDetailDTO objItemLocationStageQuantity = objMaterialStagingPullDetailDAL.GetItemStagingQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, RoomID, CompanyID, SessionUserId);
                            if (objItemLocationStageQuantity != null && objItemLocationStageQuantity.StagingBinId > 0)
                            {
                                obj.ItemStageLocationQty = objItemLocationStageQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationStageQuantity.ConsignedQuantity.GetValueOrDefault(0);
                            }

                            objPullDal.EditForPullQty(obj);
                            objPullDal.InsertPullEditHistory(obj.GUID, obj.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, objnewPullMasterData.WhatWhereAction);
                            //objPullDal.Edit(obj);

                            double DiffPullCost = (OldPullCost - (obj.PullCost ?? 0));
                            double DiffPoolQuantity = (OldPullQuantity - (obj.PoolQuantity ?? 0));

                            if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                UpdateProjectSpendWithCostEditPull(oItemRecord, obj, DiffPullCost, DiffPoolQuantity, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
                            }
                        }
                    }

                    #endregion
                }
            }

            #endregion

            if (!IsStagginLocation)
            {

                #region "Update Turns and Average Usgae"
                UpdateTurnsAverageUsage(obj, SessionUserId);
                #endregion

                #region "Update Ext Cost And Avg Cost"
                new ItemMasterDAL(base.DataBaseName).GetAndUpdateExtCostAndAvgCost(oItemRecord.GUID, RoomID, CompanyID);
                #endregion
            }

            //if (!IsStagginLocation && oItemRecord != null && oItemRecord.GUID != Guid.Empty)
            //{
            //    ItemMasterDAL objItemMaster = new ItemMasterDAL(base.DataBaseName);
            //    objItemMaster.EditDate(oItemRecord.GUID, "EditPulledDate");
            //}


            if (objnewPullMasterData.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                objWOLDAL.UpdateWOItemAndTotalCost(objnewPullMasterData.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), RoomID, CompanyID);
            }

            UpdateCumulativeOnHand(objnewPullMasterData);

            return true;
        }

        public PullMasterViewDTO UpdatetoPullDetail(PullDetailsDTO Pulldetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, Double? ItemSellPrice)
        {
            PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
            PullDetailsDTO objDTO = new PullDetailsDTO();
            objDTO.GUID = Pulldetail.GUID;
            objDTO.ID = Pulldetail.ID;
            objDTO.PULLGUID = PullGUID;
            objDTO.ItemGUID = Pulldetail.ItemGUID;

            if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : ProjectSpendGUID;
            }

            if (Pulldetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (Pulldetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (Pulldetail.SerialNumber != null)
                objDTO.SerialNumber = Pulldetail.SerialNumber;

            if (Pulldetail.LotNumber != null)
                objDTO.LotNumber = Pulldetail.LotNumber;

            if (Pulldetail.Expiration != null)
                objDTO.Expiration = Pulldetail.Expiration;

            objDTO.Received = Pulldetail.Received;
            objDTO.BinID = Pulldetail.BinID;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = Pulldetail.Room;
            objDTO.CompanyID = Pulldetail.CompanyID;
            objDTO.ItemLocationDetailGUID = Pulldetail.ItemLocationDetailGUID;
            objDTO.ItemCost = ItemCost;
            objDTO.ItemPrice = ItemSellPrice;
            objDTO.PullCredit = Pulldetail.PullCredit;
            objDTO.AddedFrom = Pulldetail.AddedFrom;
            objDTO.EditedFrom = Pulldetail.EditedFrom;
            objDTO.ReceivedOnWeb = Pulldetail.ReceivedOnWeb;
            objDAL.Edit(objDTO);

            //Edit pull master
            PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty));

            //if (objPullDTO != null)
            //{
            //    objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemCost.GetValueOrDefault(0));
            //    objPullDTO.PullPrice = (objPullDTO.PullPrice.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemSellPrice.GetValueOrDefault(0));
            //    objPullDTO.WhatWhereAction = Pulldetail.PullCredit;
            //    objPullMAster.Edit(objPullDTO);
            //}

            return objPullDTO;
        }

        public void UpdateProjectSpendWithCostEditPull(ItemMasterDTO ItemDTO, PullMasterViewDTO objPullDTO, double DeductedPullCost, double DeductedPullQuantity, Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyID)
        {
            ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
            ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
            objProjectSpendDTO = objProjectDAL.GetRecord(ProjectSpendGUID, RoomID, CompanyID, false, false);

            if (objProjectSpendDTO != null)
            {
                objProjectSpendDTO.DollarUsedAmount = objProjectSpendDTO.DollarUsedAmount.GetValueOrDefault(0) - (decimal)DeductedPullCost;
                objProjectSpendDTO.WhatWhereAction = "Pull";
                objProjectDAL.Edit(objProjectSpendDTO);
                ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                var tmpsupplierIds = new List<long>();
                //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(ProjectSpendGUID, RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == ItemDTO.GUID).FirstOrDefault(); //&& x.Created.Value <= ItemDTO.Created.Value
                objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(ProjectSpendGUID, RoomID, CompanyID, tmpsupplierIds, Convert.ToString(ItemDTO.GUID)).FirstOrDefault();

                if (objPrjSpenItmDTO != null)
                {
                    //Update Quanitty used limit
                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) - (decimal)DeductedPullCost;
                    objPrjSpenItmDTO.QuantityUsed = (double)objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) - (double)DeductedPullQuantity;
                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                }
                else
                {
                    objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                    objPrjSpenItmDTO.Action = "insert";
                    objPrjSpenItmDTO.AddedFrom = "web";
                    objPrjSpenItmDTO.CompanyID = objProjectSpendDTO.CompanyID;
                    objPrjSpenItmDTO.Created = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.CreatedBy = objPullDTO.CreatedBy;
                    objPrjSpenItmDTO.Description = "Added from pull";
                    objPrjSpenItmDTO.DollarLimitAmount = null;
                    objPrjSpenItmDTO.DollarUsedAmount = (decimal)DeductedPullCost;
                    objPrjSpenItmDTO.EditedFrom = "web";
                    objPrjSpenItmDTO.GUID = Guid.NewGuid();
                    objPrjSpenItmDTO.ID = 0;
                    objPrjSpenItmDTO.IsArchived = false;
                    objPrjSpenItmDTO.IsDeleted = false;
                    objPrjSpenItmDTO.ItemGUID = ItemDTO.GUID;
                    objPrjSpenItmDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.LastUpdatedBy = objPullDTO.CreatedBy;
                    objPrjSpenItmDTO.ProjectGUID = objProjectSpendDTO.GUID;
                    objPrjSpenItmDTO.QuantityLimit = null;
                    objPrjSpenItmDTO.QuantityUsed = (double)DeductedPullQuantity;
                    objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.Room = objProjectSpendDTO.Room;
                    new ProjectSpendItemsDAL(base.DataBaseName).Insert(objPrjSpenItmDTO);
                }

                objProjectSpendDTO.ProjectSpendItems = new List<ProjectSpendItemsDTO>();
                objProjectSpendDTO.ProjectSpendItems.Add(objPrjSpenItmDTO);
                /*////////CODE FOR SEND MAIL OF PROJECT SPEND LIMIT EXCEED///////////*/
                SendEmailForProjectSpendLimitExceed(objProjectSpendDTO);
                /*////////CODE FOR SEND MAIL OF PROJECT SPEND LIMIT EXCEED///////////*/
            }
        }

        public PullMasterViewDTO UpdatetoMSPullDetail(PullDetailsDTO Pulldetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, Double? ItemSellPrice)
        {
            PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
            PullDetailsDTO objDTO = new PullDetailsDTO();
            objDTO.GUID = Pulldetail.GUID;
            objDTO.ID = Pulldetail.ID;
            objDTO.PULLGUID = PullGUID;
            objDTO.ItemGUID = Pulldetail.ItemGUID;

            if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : ProjectSpendGUID;
            }

            if (Pulldetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (Pulldetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (Pulldetail.SerialNumber != null)
                objDTO.SerialNumber = Pulldetail.SerialNumber;

            if (Pulldetail.LotNumber != null)
                objDTO.LotNumber = Pulldetail.LotNumber;

            if (Pulldetail.Expiration != null)
                objDTO.Expiration = Pulldetail.Expiration;

            objDTO.Received = Pulldetail.Received;
            objDTO.BinID = Pulldetail.BinID;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = Pulldetail.Room;
            objDTO.CompanyID = Pulldetail.CompanyID;
            objDTO.ItemLocationDetailGUID = null;
            objDTO.MaterialStagingPullDetailGUID = Pulldetail.MaterialStagingPullDetailGUID;
            objDTO.ItemCost = ItemCost;
            objDTO.ItemPrice = ItemSellPrice;
            objDTO.PullCredit = Pulldetail.PullCredit;
            objDTO.AddedFrom = Pulldetail.AddedFrom;
            objDTO.EditedFrom = Pulldetail.EditedFrom;
            objDTO.ReceivedOnWeb = Pulldetail.ReceivedOnWeb;
            objDAL.Edit(objDTO);

            //Edit pull master
            PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty));

            //if (objPullDTO != null)
            //{
            //    objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemCost.GetValueOrDefault(0));
            //    objPullDTO.PullPrice = (objPullDTO.PullPrice.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemSellPrice.GetValueOrDefault(0));
            //    objPullDTO.WhatWhereAction = Pulldetail.PullCredit;
            //    objPullMAster.Edit(objPullDTO);
            //}

            return objPullDTO;
        }

        protected PullMasterViewDTO AddtoPullDetailForEdit(ItemLocationDetailsDTO itemlocationdetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, Double? ItemSellPrice)
        {
            PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
            PullDetailsDTO objDTO = new PullDetailsDTO();
            objDTO.PULLGUID = PullGUID;
            objDTO.ItemGUID = itemlocationdetail.ItemGUID;

            if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : ProjectSpendGUID;
            }

            if (itemlocationdetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (itemlocationdetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (itemlocationdetail.SerialNumber != null)
                objDTO.SerialNumber = itemlocationdetail.SerialNumber;

            if (itemlocationdetail.LotNumber != null)
                objDTO.LotNumber = itemlocationdetail.LotNumber;

            if (itemlocationdetail.Expiration != null)
                objDTO.Expiration = itemlocationdetail.Expiration;

            objDTO.Received = itemlocationdetail.Received;
            objDTO.BinID = itemlocationdetail.BinID;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = itemlocationdetail.Room;
            objDTO.CompanyID = itemlocationdetail.CompanyID;
            objDTO.ItemLocationDetailGUID = itemlocationdetail.GUID;
            objDTO.ItemCost = ItemCost;
            objDTO.ItemPrice = ItemSellPrice;
            objDTO.PullCredit = itemlocationdetail.PullCredit;

            objDTO.ID = objDAL.Insert(objDTO);

            //Edit pull master
            PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty));

            //if (objPullDTO != null)
            //{
            //    objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemCost.GetValueOrDefault(0));
            //    objPullDTO.PullPrice = (objPullDTO.PullPrice.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * ItemSellPrice.GetValueOrDefault(0));
            //    objPullDTO.WhatWhereAction = itemlocationdetail.PullCredit;
            //    objPullMAster.Edit(objPullDTO);
            //}

            return objPullDTO;
        }

        protected PullMasterViewDTO AddtoPullDetailForEdit(MaterialStagingPullDetailDTO itemlocationdetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, Double? ItemSellPrice)
        {
            PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
            PullDetailsDTO objDTO = new PullDetailsDTO();
            objDTO.PULLGUID = PullGUID;
            objDTO.ItemGUID = itemlocationdetail.ItemGUID;

            if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
            }

            if (itemlocationdetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (itemlocationdetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (itemlocationdetail.SerialNumber != null)
                objDTO.SerialNumber = itemlocationdetail.SerialNumber;

            if (itemlocationdetail.LotNumber != null)
                objDTO.LotNumber = itemlocationdetail.LotNumber;

            if (itemlocationdetail.Expiration != null)
                objDTO.Expiration = itemlocationdetail.Expiration;

            objDTO.Received = itemlocationdetail.Received;
            objDTO.BinID = itemlocationdetail.StagingBinId;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = itemlocationdetail.Room;
            objDTO.CompanyID = itemlocationdetail.CompanyID;
            objDTO.MaterialStagingPullDetailGUID = itemlocationdetail.GUID;
            objDTO.ItemPrice = ItemSellPrice;
            objDTO.ItemCost = ItemCost;
            objDTO.PullCredit = itemlocationdetail.PullCredit;
            objDAL.Insert(objDTO);

            //Edit pull master
            PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty));

            //if (objPullDTO != null)
            //{
            //    objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * (double)ItemCost.GetValueOrDefault(0));
            //    objPullDTO.PullPrice = (objPullDTO.PullPrice.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * (double)ItemSellPrice.GetValueOrDefault(0));
            //    objPullDTO.WhatWhereAction = itemlocationdetail.PullCredit;
            //    objPullMAster.Edit(objPullDTO);
            //}

            return objPullDTO;
        }

        #endregion

        #region Credit

        public PullMasterViewDTO UpdatetoPullDetailForCredit(PullDetailsDTO Pulldetail, PullCreditHistoryDTO CreditDetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, Double? ItemSellPrice)
        {
            PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
            PullDetailsDTO objDTO = new PullDetailsDTO();
            objDTO.GUID = Pulldetail.GUID;
            objDTO.ID = Pulldetail.ID;
            objDTO.PULLGUID = PullGUID;
            objDTO.ItemGUID = Pulldetail.ItemGUID;

            if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : ProjectSpendGUID;
            }

            if (Pulldetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (Pulldetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (Pulldetail.SerialNumber != null)
                objDTO.SerialNumber = Pulldetail.SerialNumber;

            if (Pulldetail.LotNumber != null)
                objDTO.LotNumber = Pulldetail.LotNumber;

            if (Pulldetail.Expiration != null)
                objDTO.Expiration = Pulldetail.Expiration;

            objDTO.Received = Pulldetail.Received;
            objDTO.BinID = Pulldetail.BinID;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = Pulldetail.Room;
            objDTO.CompanyID = Pulldetail.CompanyID;
            objDTO.ItemLocationDetailGUID = Pulldetail.ItemLocationDetailGUID;
            objDTO.MaterialStagingPullDetailGUID = Pulldetail.MaterialStagingPullDetailGUID;
            objDTO.ItemCost = ItemCost;
            objDTO.ItemPrice = ItemSellPrice;
            objDTO.PullCredit = Pulldetail.PullCredit;
            objDTO.AddedFrom = Pulldetail.AddedFrom;
            objDTO.EditedFrom = Pulldetail.EditedFrom;
            objDTO.ReceivedOnWeb = Pulldetail.ReceivedOnWeb;
            objDAL.Edit(objDTO);

            //EditCreditHistory
            objDAL.UpdateCreditHistory(CreditDetail);

            //EDit PullDetails for Credit Quantity Update
            objDAL.UpdatePullDetailsForCreditQuantity(CreditDetail);

            // Edit PullMaster For Creedit Quantity
            objDAL.UpdatePullMasterForCreditQuantity(CreditDetail);

            //Edit pull master
            PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
            PullMasterViewDTO objPullDTO = objPullMAster.GetPullByGuidPlain(CreditDetail.CreditGuid.GetValueOrDefault(Guid.Empty));


            return objPullDTO;
        }

        public void UpdateProjectSpendWithCostEditCredit(ItemMasterDTO ItemDTO, PullMasterViewDTO objPullDTO, double DeductedPullCost, double DeductedPullQuantity, Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyID)
        {
            ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
            ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
            objProjectSpendDTO = objProjectDAL.GetRecord(ProjectSpendGUID, RoomID, CompanyID, false, false);

            if (objProjectSpendDTO != null)
            {
                objProjectSpendDTO.DollarUsedAmount = objProjectSpendDTO.DollarUsedAmount.GetValueOrDefault(0) + (decimal)DeductedPullCost;
                objProjectSpendDTO.WhatWhereAction = "Credit";
                objProjectDAL.Edit(objProjectSpendDTO);
                ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
                var tmpsupplierIds = new List<long>();
                //objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(ProjectSpendGUID, RoomID, CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == ItemDTO.GUID).FirstOrDefault(); //&& x.Created.Value <= ItemDTO.Created.Value
                objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(ProjectSpendGUID, RoomID, CompanyID, tmpsupplierIds, Convert.ToString(ItemDTO.GUID)).FirstOrDefault();

                if (objPrjSpenItmDTO != null)
                {
                    //Update Quanitty used limit
                    objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) + (decimal)DeductedPullCost;
                    objPrjSpenItmDTO.QuantityUsed = (double)objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) + (double)DeductedPullQuantity;
                    objPrjSpenItmDAL.Edit(objPrjSpenItmDTO);
                }
                else
                {
                    objPrjSpenItmDTO = new ProjectSpendItemsDTO();
                    objPrjSpenItmDTO.Action = "insert";
                    objPrjSpenItmDTO.AddedFrom = "web";
                    objPrjSpenItmDTO.CompanyID = objProjectSpendDTO.CompanyID;
                    objPrjSpenItmDTO.Created = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.CreatedBy = objPullDTO.CreatedBy;
                    objPrjSpenItmDTO.Description = "Added from credit";
                    objPrjSpenItmDTO.DollarLimitAmount = null;
                    objPrjSpenItmDTO.DollarUsedAmount = (decimal)DeductedPullCost;
                    objPrjSpenItmDTO.EditedFrom = "web";
                    objPrjSpenItmDTO.GUID = Guid.NewGuid();
                    objPrjSpenItmDTO.ID = 0;
                    objPrjSpenItmDTO.IsArchived = false;
                    objPrjSpenItmDTO.IsDeleted = false;
                    objPrjSpenItmDTO.ItemGUID = ItemDTO.GUID;
                    objPrjSpenItmDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.LastUpdatedBy = objPullDTO.CreatedBy;
                    objPrjSpenItmDTO.ProjectGUID = objProjectSpendDTO.GUID;
                    objPrjSpenItmDTO.QuantityLimit = null;
                    objPrjSpenItmDTO.QuantityUsed = (double)DeductedPullQuantity;
                    objPrjSpenItmDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objPrjSpenItmDTO.Room = objProjectSpendDTO.Room;
                    new ProjectSpendItemsDAL(base.DataBaseName).Insert(objPrjSpenItmDTO);
                }

                objProjectSpendDTO.ProjectSpendItems = new List<ProjectSpendItemsDTO>();
                objProjectSpendDTO.ProjectSpendItems.Add(objPrjSpenItmDTO);
                /*////////CODE FOR SEND MAIL OF PROJECT SPEND LIMIT EXCEED///////////*/
                SendEmailForProjectSpendLimitExceed(objProjectSpendDTO);
                /*////////CODE FOR SEND MAIL OF PROJECT SPEND LIMIT EXCEED///////////*/
            }
        }

        public List<PullDetailToCredit> GetPreviousPullsForCredit(ItemInfoToCredit itemToCredit, long RoomID, long CompanyID, long UserID,long EnterpriseId, string CultureCode)
        {
            PullDetailsDAL pullDetailDAL = null;
            PullMasterDAL pullMasterDAL = null;
            IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
            List<PullDetailsDTO> itmDetails = null;
            //List<PullDetailToCredit> pullDetailsToCred = null;

            try
            {
                pullDetailDAL = new PullDetailsDAL(base.DataBaseName);
                pullMasterDAL = new PullMasterDAL(base.DataBaseName);
                itmDetails = new List<PullDetailsDTO>();
                //pullDetailsToCred = new List<PullDetailToCredit>();
                lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryPlain(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty), "pull", RoomID, CompanyID);
                double remainingQty = itemToCredit.Quantity;

                if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                {
                    double totalprevPullQty = lstPullDetailDTO.Sum(x => x.PoolQuantity.GetValueOrDefault(0) - (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
                    itemToCredit.PrevPullQty = totalprevPullQty;
                    //RoomDTO objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDPlain(RoomID);
                    string columnList = "ID,RoomName,IsIgnoreCreditRule";
                    RoomDTO objRoomDTO = new CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");


                    if (!objRoomDTO.IsIgnoreCreditRule)
                    {
                        if (totalprevPullQty < itemToCredit.Quantity)
                        {
                            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                            string msgCreditQtyGreaterThanPreviousPullQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditQtyGreaterThanPreviousPullQty", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                            itemToCredit.IsModelShow = true;
                            itemToCredit.ErrorMessage = string.Format(msgCreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity) + " (" + totalprevPullQty.ToString() + ")."; 
                            remainingQty = totalprevPullQty;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                        {
                            itemToCredit.IsModelShow = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(itemToCredit.ItemTracking) && itemToCredit.ItemTracking != "QUICKLIST")
                    {
                        if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
                        {
                            totalprevPullQty = lstPullDetailDTO.Sum(x => x.PoolQuantity.GetValueOrDefault(0) - (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
                            itemToCredit.PrevPullQty = totalprevPullQty;

                            if (!objRoomDTO.IsIgnoreCreditRule)
                            {
                                if (totalprevPullQty < itemToCredit.Quantity)
                                {
                                    var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
                                    string msgCreditQtyGreaterThanPreviousPullQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("CreditQtyGreaterThanPreviousPullQty", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                                    itemToCredit.IsModelShow = true;
                                    itemToCredit.ErrorMessage = string.Format(msgCreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity) + " (" + totalprevPullQty.ToString() + ").";
                                    remainingQty = totalprevPullQty;
                                }
                            }
                            else
                            {
                                itemToCredit.IsModelShow = true;
                            }

                            if (itemToCredit.ItemTracking == "SERIALTRACK")
                            {
                                itemToCredit.IsModelShow = true;

                                //foreach (var item in lstPullDetailDTO)
                                //{
                                //    PullDetailToCredit pldtl = new PullDetailToCredit()
                                //    {
                                //        Serial = item.SerialNumber,
                                //        ExpireDate = item.Expiration,
                                //        Lot = string.Empty,
                                //        Qty = item.PoolQuantity.GetValueOrDefault(0)
                                //    };

                                //    pullDetailsToCred.Add(pldtl);
                                //}
                            }
                            else if (itemToCredit.ItemTracking == "LOTTRACK" || itemToCredit.ItemTracking == "DATECODETRACK")
                            {
                                //List<PullDetailToCredit> lst = (from x in lstPullDetailDTO
                                //                                group x by new { x.LotNumber, x.ItemGUID, x.Expiration } into grp
                                //                                select new PullDetailToCredit
                                //                                {
                                //                                    Lot = grp.Key.LotNumber,
                                //                                    ExpireDate = grp.Key.Expiration,
                                //                                    Qty = grp.Sum(p => p.PoolQuantity.GetValueOrDefault(0) - (p.CreditConsignedQuantity.GetValueOrDefault(0) + p.CreditCustomerOwnedQuantity.GetValueOrDefault(0))),
                                //                                }).ToList();
                                //if (lst != null && lst.Count > 1)
                                //{
                                //    foreach (var item in lst)
                                //    {
                                //        pullDetailsToCred.Add(item);
                                //    }
                                //}
                                //else
                                //{
                                //    foreach (var item in lst)
                                //    {
                                //        if (remainingQty <= item.Qty)
                                //            item.Qty = remainingQty;
                                //        else
                                //            remainingQty -= item.Qty;

                                //        pullDetailsToCred.Add(item);
                                //    }
                                //}

                                if (itemToCredit.PrevPullsToCredit.Count > 1)
                                    itemToCredit.IsModelShow = true;
                            }
                        }
                    }
                    else
                    {
                        //PullDetailToCredit pldtl = new PullDetailToCredit()
                        //{
                        //    Serial = string.Empty,
                        //    ExpireDate = string.Empty,
                        //    Lot = string.Empty,
                        //    Qty = remainingQty
                        //};

                        //pullDetailsToCred.Add(pldtl);
                    }
                }

                return itemToCredit.PrevPullsToCredit;
            }
            finally
            {
                pullDetailDAL = null;
                lstPullDetailDTO = null;
            }
        }

        public bool InsertPullEditHistory(Guid PullGuid, double NewQuantity, double OldQuantity, string UniqueID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@PullGuid", PullGuid),
                                                   new SqlParameter("@NewQuantity", NewQuantity),
                                                   new SqlParameter("@OldQuantity", OldQuantity),
                                                   new SqlParameter("@UniqueID", UniqueID)
                                                };
                context.Database.ExecuteSqlCommand("exec [InsertPullEditHistory] @PullGuid,@NewQuantity,@OldQuantity,@UniqueID", params1);
                return true;
            }
        }

        #endregion

        #endregion

        #region WI-6503 Create Work Order Label with Iine-Items details similar to Order Label

        public List<PullMasterViewDTO> GetWorkOrderLineItemsByWOGUID(Guid WorkOrderGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@WorkOrderGUID", WorkOrderGUID) };
                var pulls = context.Database.SqlQuery<PullMasterViewDTO>("exec [GetWorkOrderLineItemsByWOGUID] @RoomID,@CompanyID,@WorkOrderGUID", params1).ToList();

                return pulls;
            }
        }

        #endregion

    }
}


