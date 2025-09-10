using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class RequisitionDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public RequisitionDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public RequisitionDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public List<RequisitionDetailsDTO> GetReqLinesByReqGUIDPlain(Guid RequisitionGUID, long SupplierID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@SupplierID", SupplierID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetReqLinesByReqGUIDPlain] @RequisitionGUID,@SupplierID,@RoomId,@CompanyID", params1).ToList();
            }
        }
        public List<RequisitionDetailsDTO> GetReqToolLinesByReqGUIDPlain(Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetReqToolLinesByReqGUIDPlain] @RequisitionGUID,@RoomId,@CompanyID", params1).ToList();
            }
        }

        public List<RequisitionDetailsDTO> GetReqLinesByReqGUIDToolAndCategory(Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetReqLinesByReqGUIDToolAndCategory] @RequisitionGUID,@RoomId,@CompanyID", params1).ToList();
            }
        }

        public List<RequisitionDetailsDTO> GetReqLinesByReqGUIDFull(Guid RequisitionGUID, long SupplierID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@SupplierID", SupplierID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsusingReqGuid] @CompanyID,@RoomId,@RequisitionGUID,@SupplierID", params1).AsEnumerable<RequisitionDetailsDTO>().OrderBy("ID DESC").ToList();
            }
        }

        public RequisitionDetailsDTO GetReqLinesByReqGUIDToolCategory(Guid RequisitionGUID, long ToolCategoryID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@ToolCategoryID", ToolCategoryID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetReqLinesByReqGUIDToolCategory] @RequisitionGUID,@ToolCategoryID,@RoomId,@CompanyID", params1).FirstOrDefault();
            }
        }

        public RequisitionDetailsDTO GetRequisitionDetailsByGUIDPlain(Guid ReqdtlGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqdtlGUID", ReqdtlGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsByGUIDPlain] @ReqdtlGUID", params1).FirstOrDefault();
            }
        }
        public RequisitionDetailsDTO GetRequisitionDetailsByIDPlain(long ReqdtlID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqdtlID", ReqdtlID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsByIDPlain] @ReqdtlID", params1).FirstOrDefault();
            }
        }
        public List<RequisitionDetailsDTO> GetRequisitionDetailsItemGUIDPlain(Guid ItemGUID, Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsItemGUIDPlain] @ItemGUID,@RequisitionGUID,@RoomId,@CompanyID", params1).ToList();
            }
        }
        public List<RequisitionDetailsDTO> GetReqDtlFirstByItemGUIDPlain(Guid ItemGUID, Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            return GetRequisitionDetailsItemGUIDPlain(ItemGUID, RequisitionGUID, RoomId, CompanyID).OrderByDescending(t => t.ID).ToList();
        }
        public List<RequisitionDetailsDTO> GetRequisitionDetailsItemGUIDBinIDPlain(Guid ItemGUID, long BinID, Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@BinID", BinID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsItemGUIDBinIDPlain] @ItemGUID,@BinID,@RequisitionGUID,@RoomId,@CompanyID", params1).ToList();
            }
        }
        public RequisitionDetailsDTO GetRequisitionDetailsItemGUIDBinIDSinglePlain(Guid ItemGUID, long BinID, Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            return GetRequisitionDetailsItemGUIDBinIDPlain(ItemGUID, BinID, RequisitionGUID, RoomId, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public List<ItemQuantityDetail> GetItemRequistionQtyDetail(Guid ItemGUID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemQuantityDetail>("exec [GetItemRequistionQtyDetail] @ItemGUID,@RoomId,@CompanyID", params1).ToList();
            }
        }

        public List<ItemQuantityDetail> GetItemRequistionQtyDetail(Guid ItemGUID, long RoomId, long CompanyID, long BinID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@BinID", BinID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemQuantityDetail>("exec [GetItemBinRequistionQtyDetail] @ItemGUID,@RoomId,@CompanyID,@BinID", params1).ToList();
            }
        }

        public List<RequisitionDetailsDTO> GetRequisitionDetailsToolGUIDPlain(Guid ToolGUID, Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsToolGUIDPlain] @ToolGUID,@RequisitionGUID,@RoomId,@CompanyID", params1).ToList();
            }
        }
        public RequisitionDetailsDTO GetRequisitionDetailsToolGUIDSinglePlain(Guid ToolGUID, Guid RequisitionGUID, long RoomId, long CompanyID)
        {
            return GetRequisitionDetailsToolGUIDPlain(ToolGUID, RequisitionGUID, RoomId, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }

        public List<RequisitionDetailsDTO> GetHistoryRecordByRequisitionId(Guid RequisitionGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@RequisitionGUID", RequisitionGUID), new SqlParameter("@SupplierID", DBNull.Value) };
                return context.Database.SqlQuery<RequisitionDetailsDTO>("exec [GetRequisitionDetailsusingReqGuid] @CompanyID,@RoomId,@RequisitionGUID,@SupplierID", params1).ToList();
            }
        }
        public Int64 Insert(RequisitionDetailsDTO objDTO, long SessionUserId)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionDetail obj = new RequisitionDetail();
                obj.ID = 0;
                obj.RequisitionGUID = objDTO.RequisitionGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.ItemSellPrice = objDTO.ItemSellPrice;
                obj.BinID = objDTO.BinID;

                if (objDTO.ProjectSpendGUID != Guid.Empty)
                    obj.ProjectSpendGUID = objDTO.ProjectSpendGUID;

                if (objDTO.SupplierAccountGuid != Guid.Empty)
                    obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;

                obj.QuantityRequisitioned = objDTO.QuantityRequisitioned;
                obj.QuantityPulled = objDTO.QuantityPulled;
                obj.QuantityApproved = objDTO.QuantityApproved;
                obj.RequiredDate = objDTO.RequiredDate;
                obj.Created = objDTO.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                if (objDTO.GUID != null && objDTO.GUID != Guid.Empty)
                {
                    obj.GUID = objDTO.GUID;
                }
                else
                {
                    obj.GUID = Guid.NewGuid();
                }
                obj.PullUDF1 = objDTO.PullUDF1;
                obj.PullUDF2 = objDTO.PullUDF2;
                obj.PullUDF3 = objDTO.PullUDF3;
                obj.PullUDF4 = objDTO.PullUDF4;
                obj.PullUDF5 = objDTO.PullUDF5;
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";
                obj.QuickListItemGUID = objDTO.QuickListItemGUID;
                obj.TechnicianGuid = objDTO.TechnicianGUID;
                obj.ToolCheckoutUDF1 = objDTO.ToolCheckoutUDF1;
                obj.ToolCheckoutUDF2 = objDTO.ToolCheckoutUDF2;
                obj.ToolCheckoutUDF3 = objDTO.ToolCheckoutUDF3;
                obj.ToolCheckoutUDF4 = objDTO.ToolCheckoutUDF4;
                obj.ToolCheckoutUDF5 = objDTO.ToolCheckoutUDF5;
                obj.ToolGuid = objDTO.ToolGUID;
                obj.ToolCategoryID = objDTO.ToolCategoryID;
                obj.IsToolCategoryInsert = objDTO.IsToolCategoryInsert;
                obj.PullOrderNumber = objDTO.PullOrderNumber;

                context.RequisitionDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                if (string.IsNullOrEmpty(objDTO.RequisitionStatus))
                {
                    Guid ReqGUID = objDTO.RequisitionGUID ?? Guid.Empty;
                    objDTO.RequisitionStatus = context.RequisitionMasters.FirstOrDefault(x => x.GUID == ReqGUID).RequisitionStatus;
                }

                Guid ItemGUID = objDTO.ItemGUID ?? Guid.Empty;
                if (ItemGUID != Guid.Empty)
                {
                    RequisitionDetailsDAL objReqDAL = new RequisitionDetailsDAL(base.DataBaseName);
                    objReqDAL.UpdateItemOnRequisitionQty(ItemGUID, objDTO.Room, objDTO.CompanyID, objDTO.LastUpdatedBy);
                }

                if (objDTO.RequisitionStatus == "Approved")
                {
                    if (ItemGUID != Guid.Empty)
                    {
                        new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(ItemGUID, objDTO.LastUpdatedBy, "web", "Consume >> Create Requisition", SessionUserId);

                        ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                        ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, ItemGUID);
                        if (ItemDTO != null && ItemDTO.ID > 0)
                        {
                            /* WI-5009 update QtyToMeetDemand */
                            if (ItemDTO.ItemType == 3 && ItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                            {
                                new KitDetailDAL(base.DataBaseName).UpdateQtyToMeedDemand(ItemDTO.GUID, objDTO.LastUpdatedBy, SessionUserId);
                            }
                            /* WI-5009 update QtyToMeetDemand */
                        }
                    }
                }

                return obj.ID;
            }

        }
        public bool Edit(RequisitionDetailsDTO objDTO, long SessionUserId, bool IsUpdateReqItemAndCost = true)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            double OldReqQTY = 0;
            double OldAprQTY = 0;
            double OldPulQTY = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionDetailsDTO objDTO1 = GetRequisitionDetailsByGUIDPlain(objDTO.GUID);
                RequisitionDetail obj = new RequisitionDetail();

                OldReqQTY = objDTO1.QuantityRequisitioned.GetValueOrDefault(0);
                OldAprQTY = objDTO1.QuantityApproved.GetValueOrDefault(0);
                OldPulQTY = objDTO1.QuantityPulled.GetValueOrDefault(0);

                if (objDTO.ID == 0)
                {
                    obj.ID = objDTO1.ID;
                    objDTO.ID = objDTO1.ID;
                }
                else
                {
                    obj.ID = objDTO.ID;
                }

                if (objDTO.RequisitionGUID == null)
                {
                    obj.RequisitionGUID = objDTO1.RequisitionGUID;
                    objDTO.RequisitionGUID = objDTO1.RequisitionGUID;
                }
                else
                {
                    obj.RequisitionGUID = objDTO.RequisitionGUID;
                }

                //if (objDTO.ItemGUID == null)
                if (objDTO.ItemGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                {
                    obj.ItemGUID = objDTO1.ItemGUID;
                    objDTO.ItemGUID = objDTO1.ItemGUID;
                }
                else
                {
                    obj.ItemGUID = objDTO.ItemGUID;
                }
                if ((obj.ItemGUID ?? Guid.Empty) == Guid.Empty)
                {
                    obj.ItemGUID = null;
                }
                obj.ItemCost = objDTO1.ItemCost;
                obj.ItemSellPrice = objDTO1.ItemSellPrice;

                if (objDTO.QuantityRequisitioned == null)
                {
                    objDTO.QuantityRequisitioned = objDTO1.QuantityRequisitioned;
                    obj.QuantityRequisitioned = objDTO1.QuantityRequisitioned;
                }
                else
                {
                    obj.QuantityRequisitioned = objDTO.QuantityRequisitioned;
                }

                if (objDTO.QuantityPulled == null)
                {
                    objDTO.QuantityPulled = objDTO1.QuantityPulled;
                    obj.QuantityPulled = objDTO1.QuantityPulled;
                }
                else
                {
                    obj.QuantityPulled = objDTO.QuantityPulled;
                }

                if (objDTO.QuantityApproved == null)
                {
                    objDTO.QuantityApproved = objDTO1.QuantityApproved;
                    obj.QuantityApproved = objDTO1.QuantityApproved;
                }
                else
                {
                    obj.QuantityApproved = objDTO.QuantityApproved;
                }

                if (objDTO.BinID == null)
                {
                    objDTO.BinID = objDTO1.BinID;
                    obj.BinID = objDTO1.BinID;
                }
                else
                {
                    obj.BinID = objDTO.BinID;
                }

                if (objDTO.ToolCategoryID == null)
                {
                    objDTO.ToolCategoryID = objDTO1.ToolCategoryID;
                    obj.ToolCategoryID = objDTO1.ToolCategoryID;
                }
                else
                {
                    obj.ToolCategoryID = objDTO.ToolCategoryID;
                }

                if (objDTO.IsToolCategoryInsert == null)
                {
                    objDTO.IsToolCategoryInsert = objDTO1.IsToolCategoryInsert;
                    obj.IsToolCategoryInsert = objDTO1.IsToolCategoryInsert;
                }
                else
                {
                    obj.IsToolCategoryInsert = objDTO.IsToolCategoryInsert;
                }

                //if (objDTO.ProjectSpendGUID == null)
                //{
                //    objDTO.ProjectSpendGUID = objDTO1.ProjectSpendGUID;
                //    obj.ProjectSpendGUID = objDTO1.ProjectSpendGUID;
                //}
                //else
                //{
                obj.ProjectSpendGUID = objDTO.ProjectSpendGUID;
                //}


                if (objDTO.SupplierAccountGuid == null)
                {
                    objDTO.SupplierAccountGuid = objDTO1.SupplierAccountGuid;
                    obj.SupplierAccountGuid = objDTO1.SupplierAccountGuid;
                }
                else
                {
                    obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
                }

                if (objDTO.RequiredDate != null)
                {
                    obj.RequiredDate = objDTO.RequiredDate;
                }

                objDTO.Created = objDTO1.Created;
                obj.Created = objDTO1.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO1.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO1.Room;
                obj.IsDeleted = objDTO.IsDeleted ?? false;
                obj.IsArchived = objDTO.IsArchived ?? false;
                obj.CompanyID = objDTO1.CompanyID;
                obj.PullUDF1 = objDTO.PullUDF1;
                obj.PullUDF2 = objDTO.PullUDF2;
                obj.PullUDF3 = objDTO.PullUDF3;
                obj.PullUDF4 = objDTO.PullUDF4;
                obj.PullUDF5 = objDTO.PullUDF5;

                obj.ReceivedOnWeb = objDTO1.ReceivedOnWeb;
                obj.AddedFrom = objDTO1.AddedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.QuickListItemGUID = objDTO.QuickListItemGUID;

                obj.GUID = objDTO1.GUID;

                obj.TechnicianGuid = objDTO.TechnicianGUID;
                obj.ToolCheckoutUDF1 = objDTO.ToolCheckoutUDF1;
                obj.ToolCheckoutUDF2 = objDTO.ToolCheckoutUDF2;
                obj.ToolCheckoutUDF3 = objDTO.ToolCheckoutUDF3;
                obj.ToolCheckoutUDF4 = objDTO.ToolCheckoutUDF4;
                obj.ToolCheckoutUDF5 = objDTO.ToolCheckoutUDF5;
                obj.ToolGuid = objDTO.ToolGUID;
                obj.PullOrderNumber = objDTO.PullOrderNumber;

                context.RequisitionDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if (string.IsNullOrEmpty(objDTO.RequisitionStatus))
                {
                    Guid ReqGUID = objDTO.RequisitionGUID.GetValueOrDefault(Guid.Empty);
                    objDTO.RequisitionStatus = context.RequisitionMasters.FirstOrDefault(x => x.GUID == ReqGUID).RequisitionStatus;
                }

                Guid ItemGUID = objDTO.ItemGUID ?? Guid.Empty;
                if (ItemGUID != Guid.Empty)
                {
                    RequisitionDetailsDAL objReqDAL = new RequisitionDetailsDAL(base.DataBaseName);
                    objReqDAL.UpdateItemOnRequisitionQty(ItemGUID, objDTO.Room, objDTO.CompanyID, objDTO.LastUpdatedBy);
                }

                if (objDTO.RequisitionStatus == "Approved")
                {
                    //Guid ItemGUID = objDTO.ItemGUID.GetValueOrDefault(Guid.Empty);
                    if (ItemGUID != Guid.Empty)
                    {
                        //UpdateRequisitionedQuantity("Edit", objDTO, objDTO1);
                        //string qry = GetQueryOfOnlyUpdateReqQty(ItemGUID, objDTO.Room, objDTO.CompanyID, objDTO.LastUpdatedBy);
                        //double onReqQty = context.Database.SqlQuery<double>(qry).FirstOrDefault();
                        //ItemMaster item = context.ItemMasters.FirstOrDefault(x => x.GUID == ItemGUID);
                        //item.RequisitionedQuantity = onReqQty;
                        //context.SaveChanges();
                        //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(ItemGUID, objDTO.LastUpdatedBy, "web", "Req.DetailDAL >> Edit");
                        new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(ItemGUID, objDTO.LastUpdatedBy, "web", "Consume >> Modified Requisition", SessionUserId);

                        ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                        ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, ItemGUID);
                        if (ItemDTO != null && ItemDTO.ID > 0)
                        {
                            /* WI-5009 update QtyToMeetDemand */
                            if (ItemDTO.ItemType == 3 && ItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                            {
                                new KitDetailDAL(base.DataBaseName).UpdateQtyToMeedDemand(ItemDTO.GUID, objDTO.LastUpdatedBy, SessionUserId);
                            }
                            /* WI-5009 update QtyToMeetDemand */
                        }
                    }
                }
                if (objDTO.RequisitionStatus == "Unsubmitted" || objDTO.RequisitionStatus == "Submitted")
                {
                    if (IsUpdateReqItemAndCost)
                        UpdateRequisitionTotalCost(objDTO.RequisitionGUID.Value, objDTO.Room, objDTO.CompanyID);
                }
                return true;
            }
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 Room, Int64 CompanyID, long SessionUserId,long EnterpriseId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ///Then from DB....
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE RequisitionDetails SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                    }
                }

                if (context.Database.ExecuteSqlCommand(strQuery) > 0)
                {

                }

                //First Remove from Cache....
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        RequisitionDetailsDAL objDAL = new RequisitionDetailsDAL(base.DataBaseName);
                        RequisitionDetailsDTO objDTO = objDAL.GetRequisitionDetailsByGUIDPlain(Guid.Parse(item));
                        if (objDTO.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            UpdateRequisitionedQuantity("Delete", objDTO, objDTO);

                        UpdateRequisitionTotalCost(objDTO.RequisitionGUID.Value, objDTO.Room, objDTO.CompanyID);
                        PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                        var pullIds = objPullDAL.GetPullIdsByRequisitionDetailGuid(Guid.Parse(item), Room, CompanyID);

                        if (pullIds != null && pullIds.Any())
                        {
                            objPullDAL.DeletePullsByPullIds(string.Join(",", pullIds.Select(i => i.ToString()).ToArray()), userid, CompanyID, Room, SessionUserId,EnterpriseId);
                        }

                        //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy, "web", "Req.DetailDAL >> Delete");
                        new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy, "web", "Consume >> Delete Requisition", SessionUserId);

                        ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                        ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                        if (ItemDTO != null && ItemDTO.ID > 0)
                        {
                            /* WI-5009 update QtyToMeetDemand */
                            if (ItemDTO.ItemType == 3 && ItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                            {
                                new KitDetailDAL(base.DataBaseName).UpdateQtyToMeedDemand(ItemDTO.GUID, objDTO.LastUpdatedBy, SessionUserId);
                            }
                            /* WI-5009 update QtyToMeetDemand */
                        }
                    }
                }

                return true;
            }
        }
        public bool DeleteRecordsFromMaster(string IDs, Int64 userid, Int64 Room, Int64 CompanyID, long SessionUserId,long EnterpriseId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ///Then from DB....
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE RequisitionDetails SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";

                        PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                        var pullIds = objPullDAL.GetPullIdsByRequisitionDetailGuid(Guid.Parse(item), Room, CompanyID);

                        if (pullIds != null && pullIds.Any())
                        {
                            objPullDAL.DeletePullsByPullIds(string.Join(",", pullIds.Select(i => i.ToString()).ToArray()), userid, CompanyID, Room, SessionUserId,EnterpriseId);
                        }
                    }
                }

                if (context.Database.ExecuteSqlCommand(strQuery) > 0)
                {

                }

                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        RequisitionDetailsDAL objDAL = new RequisitionDetailsDAL(base.DataBaseName);
                        RequisitionDetailsDTO objDTO = objDAL.GetRequisitionDetailsByGUIDPlain(Guid.Parse(item));
                        UpdateRequisitionedQuantity("Delete", objDTO, objDTO);
                        UpdateRequisitionTotalCost(objDTO.RequisitionGUID.Value, objDTO.Room, objDTO.CompanyID);
                    }
                }
                return true;
            }
        }
        public ItemMasterDTO UpdateItemOnRequisitionQty(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@UserId", UserID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec [UpdateItemREquisitionedQty] @RoomId,@CompanyId,@ItemGuid,@UserId", params1).FirstOrDefault();

            }
        }
        public double GetItemCurrentOnRequisitionQty(Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid) };
                return context.Database.SqlQuery<double>("exec [GetItemCurrentOnRequisitionQty] @ItemGuid", params1).FirstOrDefault();
            }
        }
        public bool EditUDFDetailOnly(RequisitionDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionDetailsDTO objDTO1 = GetRequisitionDetailsByGUIDPlain(objDTO.GUID);
                RequisitionDetail obj = new RequisitionDetail();


                if (objDTO1 != null)
                {
                    obj.ID = objDTO1.ID;
                    obj.RequisitionGUID = objDTO1.RequisitionGUID;
                    obj.ItemGUID = objDTO1.ItemGUID;
                    obj.ItemCost = objDTO1.ItemCost;
                    obj.ItemSellPrice = objDTO1.ItemSellPrice;
                    obj.QuantityRequisitioned = objDTO1.QuantityRequisitioned;
                    obj.QuantityPulled = objDTO1.QuantityPulled;
                    obj.QuantityApproved = objDTO1.QuantityApproved;
                    //obj.BinID = objDTO1.BinID;
                    obj.BinID = objDTO.BinID;
                    obj.ProjectSpendGUID = objDTO1.ProjectSpendGUID;
                    obj.RequiredDate = objDTO1.RequiredDate;
                    obj.Created = objDTO1.Created;
                    obj.LastUpdated = objDTO.LastUpdated;
                    obj.CreatedBy = objDTO1.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Room = objDTO1.Room;
                    obj.IsDeleted = (bool)objDTO1.IsDeleted;
                    obj.IsArchived = (bool)objDTO1.IsArchived;
                    obj.CompanyID = objDTO1.CompanyID;
                    obj.PullUDF1 = objDTO.PullUDF1;
                    obj.PullUDF2 = objDTO.PullUDF2;
                    obj.PullUDF3 = objDTO.PullUDF3;
                    obj.PullUDF4 = objDTO.PullUDF4;
                    obj.PullUDF5 = objDTO.PullUDF5;

                    obj.ReceivedOnWeb = objDTO1.ReceivedOnWeb;
                    obj.AddedFrom = objDTO1.AddedFrom;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.QuickListItemGUID = objDTO.QuickListItemGUID;
                    obj.SupplierAccountGuid = objDTO1.SupplierAccountGuid;
                    obj.ToolCategoryID = objDTO1.ToolCategoryID;
                    obj.IsToolCategoryInsert = objDTO1.IsToolCategoryInsert;
                    obj.PullOrderNumber = objDTO1.PullOrderNumber;
                    obj.GUID = objDTO1.GUID;
                    obj.TechnicianGuid = objDTO1.TechnicianGUID;
                    context.RequisitionDetails.Attach(obj);
                    context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                }


                RequisitionMasterDAL objRequMDAL = new RequisitionMasterDAL(base.DataBaseName);
                RequisitionMasterDTO ReqDto = objRequMDAL.GetRequisitionByGUIDPlain((objDTO.RequisitionGUID ?? Guid.Empty));
                string RequisitionStatus = "";
                if (ReqDto != null && ReqDto.ID > 0)
                    RequisitionStatus = ReqDto.RequisitionStatus;

                UpdateRequisitionedQuantity("Edit", objDTO, objDTO1);
                if (RequisitionStatus == "Unsubmitted" || RequisitionStatus == "Submitted")
                {
                    UpdateRequisitionTotalCost(objDTO.RequisitionGUID.Value, objDTO.Room, objDTO.CompanyID);
                }

                return true;
            }
        }
        public void UpdateRequisitionedQuantity(string Action, RequisitionDetailsDTO objDTO, RequisitionDetailsDTO objOLDDTO)
        {
            if (objDTO != null && objDTO.ItemGUID != null && objDTO.ItemGUID.HasValue && objDTO.ItemGUID.Value != Guid.Empty)
            {
                UpdateItemOnRequisitionQty(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room, objDTO.CompanyID, objDTO.LastUpdatedBy);
            }
            return;

            //if (objDTO.QuantityRequisitioned > 0 && objDTO.ItemGUID.HasValue)
            //{
            //    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            //    ItemMasterDTO objItemDTO = objItemDAL.GetRecord(objDTO.ItemGUID.ToString(), objDTO.Room, objDTO.CompanyID);
            //    if (Action == "Add")
            //    {
            //        if (objDTO.QuantityApproved.GetValueOrDefault(0) == 0 && objDTO.QuantityPulled.GetValueOrDefault(0) == 0)
            //        {
            //            objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) + objDTO.QuantityRequisitioned.GetValueOrDefault(0);
            //        }
            //        else if (objDTO.QuantityApproved.GetValueOrDefault(0) > 0 && objDTO.QuantityPulled.GetValueOrDefault(0) == 0)
            //        {
            //            if (objItemDTO.RequisitionedQuantity > 0)
            //            {
            //                objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - (objDTO.QuantityRequisitioned.GetValueOrDefault(0) - objDTO.QuantityApproved.GetValueOrDefault(0));
            //            }
            //            else
            //            {
            //                objItemDTO.RequisitionedQuantity = objDTO.QuantityRequisitioned.GetValueOrDefault(0) - objDTO.QuantityApproved.GetValueOrDefault(0);
            //            }
            //        }
            //        else if (objDTO.QuantityPulled.GetValueOrDefault(0) > 0)
            //        {
            //            objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - (objDTO.QuantityPulled.GetValueOrDefault(0));
            //        }
            //    }
            //    else if (Action == "Edit")
            //    {
            //        objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - ((objOLDDTO.QuantityRequisitioned.GetValueOrDefault(0) - objOLDDTO.QuantityApproved.GetValueOrDefault(0)) + objOLDDTO.QuantityPulled.GetValueOrDefault(0));
            //        objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) + ((objDTO.QuantityRequisitioned.GetValueOrDefault(0) - objDTO.QuantityApproved.GetValueOrDefault(0)) + objDTO.QuantityPulled.GetValueOrDefault(0));
            //    }
            //    else if (Action == "Delete")
            //    {
            //        if (objDTO.QuantityPulled.GetValueOrDefault(0) > 0)
            //        {
            //            objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - (objDTO.QuantityApproved.GetValueOrDefault(0) - objDTO.QuantityPulled.GetValueOrDefault(0));
            //        }
            //        else if (objDTO.QuantityApproved.GetValueOrDefault(0) > 0)
            //            objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - objDTO.QuantityApproved.GetValueOrDefault(0);
            //        else
            //            objItemDTO.RequisitionedQuantity = objItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - objDTO.QuantityRequisitioned.GetValueOrDefault(0);
            //    }
            //    objItemDTO.WhatWhereAction = "Requisition";
            //    objItemDAL.Edit(objItemDTO);
            //}
        }
        public void UpdateRequisitionTotalCost(Guid RequisitionGUID, Int64 Room, Int64 CompanyID)
        {
            RequisitionMasterDAL objDAL = new RequisitionMasterDAL(base.DataBaseName);
            RequisitionMasterDTO objDTO = objDAL.GetRequisitionByGUIDPlain(RequisitionGUID);

            if (objDTO != null)
            {
                //RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(base.DataBaseName);
                //IEnumerable<RequisitionDetailsDTO> lstDetail = objDetailDAL.GetAllRecords(RequisitionGUID, Room, CompanyID).Where(x => !x.IsArchived.GetValueOrDefault(false) && !x.IsDeleted.GetValueOrDefault(false));


                //if (lstDetail != null)
                //{
                //    objDTO.NumberofItemsrequisitioned = lstDetail.Count();
                //    objDTO.TotalCost = lstDetail.Select(x => (Decimal)(x.ItemCost.GetValueOrDefault(0)) * (Decimal)(x.QuantityRequisitioned.GetValueOrDefault(0))).Sum();
                //}
                //else
                //{
                //    objDTO.NumberofItemsrequisitioned = 0;
                //    objDTO.TotalCost = 0;
                //}
                objDAL.Edit(objDTO);
            }
        }
        public void AddQLToReq(Guid ReqGUID, Guid QlGuid, double Qty, long RoomId, long CompanyId, long EnterpriseId, long UserId, string PullUDF1, string PullUDF2, string PullUDF3, string PullUDF4, string PullUDF5, long RequisationModuleId, long SessionUserId)
        {
            List<Guid> ItemGuids = new List<Guid>();
            RequisitionDetailsDAL objApi = new RequisitionDetailsDAL(base.DataBaseName);
            RequisitionDetailsDTO objRequisitionDetails = null;
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (context.QuickListItems.Any(t => t.QuickListGUID == QlGuid && (t.IsDeleted ?? false) == false))
                {
                    //---------------------------------------------------------
                    //
                    List<QuickListItem> lstItems = context.QuickListItems.Where(t => t.QuickListGUID == QlGuid && (t.IsDeleted ?? false) == false).ToList();
                    lstItems.ForEach(t =>
                    {
                        objRequisitionDetails = objApi.GetRequisitionDetailsItemGUIDBinIDSinglePlain((Guid)t.ItemGUID, t.BinID ?? 0, ReqGUID, RoomId, CompanyId);
                        if (objRequisitionDetails == null)
                        {
                            ItemMaster objItem = context.ItemMasters.FirstOrDefault(im => im.GUID == t.ItemGUID);
                            RequisitionDetail objRequisitionDetailsDTO = new RequisitionDetail();
                            objRequisitionDetailsDTO.CompanyID = CompanyId;
                            objRequisitionDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                            objRequisitionDetailsDTO.CreatedBy = UserId;
                            objRequisitionDetailsDTO.GUID = Guid.NewGuid();
                            objRequisitionDetailsDTO.ID = 0;
                            objRequisitionDetailsDTO.IsArchived = false;
                            objRequisitionDetailsDTO.IsDeleted = false;
                            objRequisitionDetailsDTO.IsNotification = false;
                            objRequisitionDetailsDTO.ItemCost = objItemMasterDAL.GetItemCostByRoomModuleSettings(CompanyId, RoomId, RequisationModuleId, (Guid)objItem.GUID, false);
                            objRequisitionDetailsDTO.ItemSellPrice = objItemMasterDAL.GetItemSellPriceByRoomModuleSettings(CompanyId, RoomId, RequisationModuleId, (Guid)objItem.GUID, false) ?? 0;
                            objRequisitionDetailsDTO.ItemGUID = objItem.GUID;
                            objRequisitionDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objRequisitionDetailsDTO.LastUpdatedBy = UserId;
                            objRequisitionDetailsDTO.ProjectSpendGUID = null;
                            objRequisitionDetailsDTO.SupplierAccountGuid = null;
                            objRequisitionDetailsDTO.QuantityApproved = 0;
                            objRequisitionDetailsDTO.QuantityPulled = 0;
                            objRequisitionDetailsDTO.QuantityRequisitioned = t.Quantity * Qty;
                            objRequisitionDetailsDTO.RequiredDate = DateTime.Now;
                            objRequisitionDetailsDTO.RequisitionGUID = ReqGUID;
                            objRequisitionDetailsDTO.Room = RoomId;
                            objRequisitionDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objRequisitionDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objRequisitionDetailsDTO.AddedFrom = "Web";
                            objRequisitionDetailsDTO.EditedFrom = "Web";
                            objRequisitionDetailsDTO.QuickListItemGUID = t.GUID;

                            //objRequisitionDetailsDTO.PullUDF1 = PullUDF1;
                            //objRequisitionDetailsDTO.PullUDF2 = PullUDF2;
                            //objRequisitionDetailsDTO.PullUDF3 = PullUDF3;
                            //objRequisitionDetailsDTO.PullUDF4 = PullUDF4;
                            //objRequisitionDetailsDTO.PullUDF5 = PullUDF5;

                            //------------------------------SET UDF------------------------------
                            //
                            QuickListDAL objQuickListDAL = new QuickListDAL(base.DataBaseName);
                            long? QLBinId = 0;
                            Dictionary<string, string> dictUDF = objQuickListDAL.GetUDFValueOrDefault(CompanyId, RoomId, QlGuid, objItem.GUID, t.GUID, out QLBinId);

                            if (QLBinId != null && QLBinId != 0)
                                objRequisitionDetailsDTO.BinID = QLBinId;

                            PullUDF1 = (dictUDF != null && !string.IsNullOrEmpty(dictUDF["UDF1"]) && !string.IsNullOrWhiteSpace(dictUDF["UDF1"]) ? dictUDF["UDF1"] : PullUDF1);
                            PullUDF2 = (dictUDF != null && !string.IsNullOrEmpty(dictUDF["UDF2"]) && !string.IsNullOrWhiteSpace(dictUDF["UDF2"]) ? dictUDF["UDF2"] : PullUDF2);
                            PullUDF3 = (dictUDF != null && !string.IsNullOrEmpty(dictUDF["UDF3"]) && !string.IsNullOrWhiteSpace(dictUDF["UDF3"]) ? dictUDF["UDF3"] : PullUDF3);
                            PullUDF4 = (dictUDF != null && !string.IsNullOrEmpty(dictUDF["UDF4"]) && !string.IsNullOrWhiteSpace(dictUDF["UDF4"]) ? dictUDF["UDF4"] : PullUDF4);
                            PullUDF5 = (dictUDF != null && !string.IsNullOrEmpty(dictUDF["UDF5"]) && !string.IsNullOrWhiteSpace(dictUDF["UDF5"]) ? dictUDF["UDF5"] : PullUDF5);

                            bool UDF1Encrypt = false;
                            bool UDF2Encrypt = false;
                            bool UDF3Encrypt = false;
                            bool UDF4Encrypt = false;
                            bool UDF5Encrypt = false;
                            UDFDAL objUdf = new UDFDAL(base.DataBaseName);
                            int TotalRecordCount = 0;
                            var DataFromDBUDF = objUdf.GetPagedUDFsByUDFTableNamePlain(0, 5, out TotalRecordCount, "ID asc", CompanyId, "PullMaster", RoomId);

                            foreach (UDFDTO u in DataFromDBUDF)
                            {
                                if (u.UDFColumnName == "UDF1")
                                {
                                    UDF1Encrypt = u.IsEncryption ?? false;
                                }
                                if (u.UDFColumnName == "UDF2")
                                {
                                    UDF2Encrypt = u.IsEncryption ?? false;
                                }
                                if (u.UDFColumnName == "UDF3")
                                {
                                    UDF3Encrypt = u.IsEncryption ?? false;
                                }
                                if (u.UDFColumnName == "UDF4")
                                {
                                    UDF4Encrypt = u.IsEncryption ?? false;
                                }
                                if (u.UDFColumnName == "UDF5")
                                {
                                    UDF5Encrypt = u.IsEncryption ?? false;
                                }
                            }

                            CommonDAL objCommon = new CommonDAL(base.DataBaseName);
                            objRequisitionDetailsDTO.PullUDF1 = (UDF1Encrypt == true && PullUDF1 != null && (!string.IsNullOrWhiteSpace(PullUDF1))) ? objCommon.GetEncryptValue(PullUDF1) : PullUDF1;
                            objRequisitionDetailsDTO.PullUDF2 = (UDF2Encrypt == true && PullUDF2 != null && (!string.IsNullOrWhiteSpace(PullUDF2))) ? objCommon.GetEncryptValue(PullUDF2) : PullUDF2;
                            objRequisitionDetailsDTO.PullUDF3 = (UDF3Encrypt == true && PullUDF3 != null && (!string.IsNullOrWhiteSpace(PullUDF3))) ? objCommon.GetEncryptValue(PullUDF3) : PullUDF3;
                            objRequisitionDetailsDTO.PullUDF4 = (UDF4Encrypt == true && PullUDF4 != null && (!string.IsNullOrWhiteSpace(PullUDF4))) ? objCommon.GetEncryptValue(PullUDF4) : PullUDF4;
                            objRequisitionDetailsDTO.PullUDF5 = (UDF5Encrypt == true && PullUDF5 != null && (!string.IsNullOrWhiteSpace(PullUDF5))) ? objCommon.GetEncryptValue(PullUDF5) : PullUDF5;

                            //-------------------------------------------------------------------
                            //
                            if (objItem != null)
                            {
                                ItemGuids.Add(t.ItemGUID.Value);
                            }
                            context.RequisitionDetails.Add(objRequisitionDetailsDTO);

                            RequisitionMaster objReqmst = context.RequisitionMasters.FirstOrDefault(x => x.GUID == ReqGUID);
                            if (objReqmst != null && (objReqmst.RequisitionStatus == "Approved"))
                            {
                                objItem.RequisitionedQuantity = (objItem.RequisitionedQuantity ?? 0) + objRequisitionDetailsDTO.QuantityRequisitioned;
                            }
                        }
                    });
                    context.SaveChanges();

                    //---------------------------------------------------------
                    //
                    RequisitionMaster objReqMaster = context.RequisitionMasters.FirstOrDefault(t => t.GUID == ReqGUID);
                    if (objReqMaster != null)
                    {
                        if (context.RequisitionDetails.Any(t => t.RequisitionGUID == ReqGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false))
                        {
                            objReqMaster.NumberofItemsrequisitioned = context.RequisitionDetails.Count(t => t.RequisitionGUID == ReqGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false);
                        }
                        else
                        {
                            objReqMaster.NumberofItemsrequisitioned = 0;
                        }
                    }
                    context.SaveChanges();

                    //---------------------------------------------------------
                    //

                    if (ItemGuids.Count() > 0)
                    {
                        foreach (Guid itemgid in ItemGuids)
                        {
                            //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(itemgid, UserId, "web", "RequisitiondetailsDAL >> AddQLToReq");
                            new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(itemgid, UserId, "web", "Consume >> Add QuickList to Requisition", SessionUserId);
                        }

                    }

                }

            }
        }

        public List<RequisitionDetailsDTO> GetPagedRequisitionDetails(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool UserConsignmentAllowed, Guid ReqGUID)
        {
            List<RequisitionDetailsDTO> lstReqDetails = new List<RequisitionDetailsDTO>();
            TotalCount = 0;
            RequisitionDetailsDTO objRequisitionDetailsDTO = new RequisitionDetailsDTO();
            DataSet dsReqDetails = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstReqDetails;
            }

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            var spname = IsArchived ? "GetPagedReqLineItems_Archive" : "GetPagedReqLineItems";
            dsReqDetails = SqlHelper.ExecuteDataset(EturnsConnection, spname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, ReqGUID);

            if (dsReqDetails != null && dsReqDetails.Tables.Count > 0)
            {
                DataTable dtReqDetails = dsReqDetails.Tables[0];
                if (dtReqDetails.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtReqDetails.Rows[0]["TotalRecords"]);

                    foreach (DataRow dr in dtReqDetails.Rows)
                    {
                        long templong = 0;
                        Guid tempguid = Guid.Empty;
                        bool tempbool = false;
                        double tempdouble = 0;
                        double temdeci = 0;
                        double temItemSellPrice = 0;
                        objRequisitionDetailsDTO = new RequisitionDetailsDTO();
                        objRequisitionDetailsDTO.CompanyID = CompanyID;
                        objRequisitionDetailsDTO.Room = RoomID;

                        if (dtReqDetails.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objRequisitionDetailsDTO.ID = templong;
                        }
                        if (dtReqDetails.Columns.Contains("RequisitionGUID"))
                        {
                            Guid.TryParse(Convert.ToString(dr["RequisitionGUID"]), out tempguid);
                            objRequisitionDetailsDTO.RequisitionGUID = tempguid;
                        }
                        if (dtReqDetails.Columns.Contains("ItemGUID"))
                        {
                            Guid.TryParse(Convert.ToString(dr["ItemGUID"]), out tempguid);
                            objRequisitionDetailsDTO.ItemGUID = tempguid;
                        }
                        if (dtReqDetails.Columns.Contains("ItemCost"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemCost"]), out temdeci);
                            objRequisitionDetailsDTO.ItemCost = temdeci;
                        }
                        if (dtReqDetails.Columns.Contains("ItemSellPrice"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemSellPrice"]), out temItemSellPrice);
                            objRequisitionDetailsDTO.ItemSellPrice = temItemSellPrice;
                        }
                        if (dtReqDetails.Columns.Contains("QuantityRequisitioned"))
                        {
                            double.TryParse(Convert.ToString(dr["QuantityRequisitioned"]), out tempdouble);
                            objRequisitionDetailsDTO.QuantityRequisitioned = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("QuantityPulled"))
                        {
                            double.TryParse(Convert.ToString(dr["QuantityPulled"]), out tempdouble);
                            objRequisitionDetailsDTO.QuantityPulled = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("QuantityApproved"))
                        {
                            double.TryParse(Convert.ToString(dr["QuantityApproved"]), out tempdouble);
                            objRequisitionDetailsDTO.QuantityApproved = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("RequiredDate"))
                        {
                            DateTime dt = new DateTime();
                            if (!string.IsNullOrWhiteSpace(Convert.ToString(dr["RequiredDate"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(dr["RequiredDate"]), out dt))
                                {
                                    objRequisitionDetailsDTO.RequiredDate = dt;
                                }
                            }
                        }
                        if (dtReqDetails.Columns.Contains("Created"))
                        {
                            objRequisitionDetailsDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtReqDetails.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objRequisitionDetailsDTO.CreatedBy = templong;
                        }
                        if (dtReqDetails.Columns.Contains("CreatedByName"))
                        {
                            objRequisitionDetailsDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                        }

                        if (dtReqDetails.Columns.Contains("LastUpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            objRequisitionDetailsDTO.LastUpdatedBy = templong;
                        }
                        if (dtReqDetails.Columns.Contains("LastUpdated"))
                        {
                            objRequisitionDetailsDTO.LastUpdated = Convert.ToDateTime(dr["LastUpdated"]);
                        }
                        if (dtReqDetails.Columns.Contains("UpdatedByName"))
                        {
                            objRequisitionDetailsDTO.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }

                        if (dtReqDetails.Columns.Contains("GUID"))
                        {
                            Guid.TryParse(Convert.ToString(dr["GUID"]), out tempguid);
                            objRequisitionDetailsDTO.GUID = tempguid;
                        }

                        if (dtReqDetails.Columns.Contains("IsArchived"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempbool);
                            objRequisitionDetailsDTO.IsArchived = tempbool;
                        }
                        if (dtReqDetails.Columns.Contains("IsDeleted"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempbool);
                            objRequisitionDetailsDTO.IsDeleted = tempbool;
                        }

                        if (dtReqDetails.Columns.Contains("ProjectSpendGUID"))
                        {
                            Guid.TryParse(Convert.ToString(dr["ProjectSpendGUID"]), out tempguid);
                            objRequisitionDetailsDTO.ProjectSpendGUID = tempguid;
                        }

                        if (dtReqDetails.Columns.Contains("BinNumber"))
                        {
                            objRequisitionDetailsDTO.BinName = Convert.ToString(dr["BinNumber"]);
                        }
                        if (dtReqDetails.Columns.Contains("BinID"))
                        {
                            templong = 0;
                            long.TryParse(Convert.ToString(dr["BinID"]), out templong);
                            if (templong > 0)
                                objRequisitionDetailsDTO.BinID = templong;

                        }
                        if (dtReqDetails.Columns.Contains("PullUDF1"))
                        {
                            objRequisitionDetailsDTO.PullUDF1 = Convert.ToString(dr["PullUDF1"]);
                        }
                        if (dtReqDetails.Columns.Contains("PullUDF2"))
                        {
                            objRequisitionDetailsDTO.PullUDF2 = Convert.ToString(dr["PullUDF2"]);
                        }
                        if (dtReqDetails.Columns.Contains("PullUDF3"))
                        {
                            objRequisitionDetailsDTO.PullUDF3 = Convert.ToString(dr["PullUDF3"]);
                        }
                        if (dtReqDetails.Columns.Contains("PullUDF4"))
                        {
                            objRequisitionDetailsDTO.PullUDF4 = Convert.ToString(dr["PullUDF4"]);
                        }
                        if (dtReqDetails.Columns.Contains("PullUDF5"))
                        {
                            objRequisitionDetailsDTO.PullUDF5 = Convert.ToString(dr["PullUDF5"]);
                        }
                        // Item Attributes //

                        if (dtReqDetails.Columns.Contains("Description"))
                        {
                            objRequisitionDetailsDTO.Description = Convert.ToString(dr["Description"]);
                        }

                        if (dtReqDetails.Columns.Contains("LongDescription"))
                        {
                            objRequisitionDetailsDTO.LongDescription = Convert.ToString(dr["LongDescription"]);
                        }

                        if (dtReqDetails.Columns.Contains("ItemNumber"))
                        {
                            objRequisitionDetailsDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }

                        if (dtReqDetails.Columns.Contains("SerialNumberTracking"))
                        {
                            objRequisitionDetailsDTO.SerialNumberTracking = Convert.ToBoolean(dr["SerialNumberTracking"]);
                        }

                        if (dtReqDetails.Columns.Contains("SupplierName"))
                        {
                            objRequisitionDetailsDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF1"))
                        {
                            objRequisitionDetailsDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                            objRequisitionDetailsDTO.ItemUDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF2"))
                        {
                            objRequisitionDetailsDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                            objRequisitionDetailsDTO.ItemUDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF3"))
                        {
                            objRequisitionDetailsDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                            objRequisitionDetailsDTO.ItemUDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF4"))
                        {
                            objRequisitionDetailsDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                            objRequisitionDetailsDTO.ItemUDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF5"))
                        {
                            objRequisitionDetailsDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                            objRequisitionDetailsDTO.ItemUDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF6"))
                        {
                            objRequisitionDetailsDTO.UDF6 = Convert.ToString(dr["UDF6"]);
                            objRequisitionDetailsDTO.ItemUDF6 = Convert.ToString(dr["UDF6"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF7"))
                        {
                            objRequisitionDetailsDTO.UDF7 = Convert.ToString(dr["UDF7"]);
                            objRequisitionDetailsDTO.ItemUDF7 = Convert.ToString(dr["UDF7"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF8"))
                        {
                            objRequisitionDetailsDTO.UDF8 = Convert.ToString(dr["UDF8"]);
                            objRequisitionDetailsDTO.ItemUDF8 = Convert.ToString(dr["UDF8"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF9"))
                        {
                            objRequisitionDetailsDTO.UDF9 = Convert.ToString(dr["UDF9"]);
                            objRequisitionDetailsDTO.ItemUDF9 = Convert.ToString(dr["UDF9"]);
                        }
                        if (dtReqDetails.Columns.Contains("UDF10"))
                        {
                            objRequisitionDetailsDTO.UDF10 = Convert.ToString(dr["UDF10"]);
                            objRequisitionDetailsDTO.ItemUDF10 = Convert.ToString(dr["UDF10"]);
                        }
                        if (dtReqDetails.Columns.Contains("Category"))
                        {
                            objRequisitionDetailsDTO.CategoryName = Convert.ToString(dr["Category"]);
                        }
                        if (dtReqDetails.Columns.Contains("CategoryID"))
                        {
                            long.TryParse(Convert.ToString(dr["CategoryID"]), out templong);
                            objRequisitionDetailsDTO.CategoryID = templong;
                        }
                        if (dtReqDetails.Columns.Contains("UOMID"))
                        {
                            long.TryParse(Convert.ToString(dr["UOMID"]), out templong);
                            objRequisitionDetailsDTO.UOMID = templong;
                        }
                        if (dtReqDetails.Columns.Contains("Unit"))
                        {
                            objRequisitionDetailsDTO.Unit = Convert.ToString(dr["Unit"]);
                        }
                        if (dtReqDetails.Columns.Contains("PackingQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.PackingQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("DefaultReorderQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["DefaultReorderQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.DefaultReorderQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("OnOrderQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["OnOrderQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.OnOrderQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("OnTransferQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["OnTransferQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.OnTransferQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("RequisitionedQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["RequisitionedQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.RequisitionedQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("CriticalQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["CriticalQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.CriticalQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("MaximumQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["MaximumQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.MaximumQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("MinimumQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["MinimumQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.MinimumQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("OnHandQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["OnHandQuantity"]), out tempdouble);
                            objRequisitionDetailsDTO.OnHandQuantity = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("SupplierName"))
                        {
                            objRequisitionDetailsDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtReqDetails.Columns.Contains("SupplierPartNo"))
                        {
                            objRequisitionDetailsDTO.SupplierPartNo = Convert.ToString(dr["SupplierPartNo"]);
                        }
                        if (dtReqDetails.Columns.Contains("Manufacturer"))
                        {
                            objRequisitionDetailsDTO.ManufacturerName = Convert.ToString(dr["Manufacturer"]);
                        }
                        if (dtReqDetails.Columns.Contains("ManufacturerNumber"))
                        {
                            objRequisitionDetailsDTO.ManufacturerNumber = Convert.ToString(dr["ManufacturerNumber"]);
                        }
                        if (dtReqDetails.Columns.Contains("ManufacturerID"))
                        {
                            long.TryParse(Convert.ToString(dr["ManufacturerID"]), out templong);
                            objRequisitionDetailsDTO.ManufacturerID = templong;
                        }
                        if (dtReqDetails.Columns.Contains("IsItemLevelMinMaxQtyRequired"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsItemLevelMinMaxQtyRequired"]), out tempbool);
                            objRequisitionDetailsDTO.IsItemLevelMinMaxQtyRequired = tempbool;
                        }
                        if (dtReqDetails.Columns.Contains("Cost"))
                        {
                            double.TryParse(Convert.ToString(dr["Cost"]), out temdeci);
                            objRequisitionDetailsDTO.Cost = temdeci;
                        }
                        if (dtReqDetails.Columns.Contains("Markup"))
                        {
                            double.TryParse(Convert.ToString(dr["Markup"]), out temdeci);
                            objRequisitionDetailsDTO.Markup = temdeci;
                        }
                        if (dtReqDetails.Columns.Contains("SellPrice"))
                        {
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out temdeci);
                            objRequisitionDetailsDTO.SellPrice = temdeci;
                        }
                        if (dtReqDetails.Columns.Contains("Turns"))
                        {
                            double.TryParse(Convert.ToString(dr["Turns"]), out tempdouble);
                            objRequisitionDetailsDTO.Turns = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("AverageUsage"))
                        {
                            double.TryParse(Convert.ToString(dr["AverageUsage"]), out tempdouble);
                            objRequisitionDetailsDTO.AverageUsage = tempdouble;
                        }
                        if (dtReqDetails.Columns.Contains("Taxable"))
                        {
                            bool.TryParse(Convert.ToString(dr["Taxable"]), out tempbool);
                            objRequisitionDetailsDTO.Taxable = tempbool;
                        }
                        if (dtReqDetails.Columns.Contains("GLAccount"))
                        {
                            objRequisitionDetailsDTO.GLAccount = Convert.ToString(dr["GLAccount"]);
                        }
                        if (dtReqDetails.Columns.Contains("ReceivedOn"))
                        {
                            DateTime dt = new DateTime();
                            if (!string.IsNullOrWhiteSpace(Convert.ToString(dr["ReceivedOn"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(dr["ReceivedOn"]), out dt))
                                {
                                    objRequisitionDetailsDTO.ReceivedOn = dt;
                                }
                            }
                        }
                        if (dtReqDetails.Columns.Contains("ReceivedOnWeb"))
                        {
                            DateTime dt = new DateTime();
                            if (!string.IsNullOrWhiteSpace(Convert.ToString(dr["ReceivedOnWeb"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(dr["ReceivedOnWeb"]), out dt))
                                {
                                    objRequisitionDetailsDTO.ReceivedOnWeb = dt;
                                }
                            }
                        }
                        if (dtReqDetails.Columns.Contains("AddedFrom"))
                        {
                            objRequisitionDetailsDTO.AddedFrom = Convert.ToString(dr["AddedFrom"]);
                        }
                        if (dtReqDetails.Columns.Contains("EditedFrom"))
                        {
                            objRequisitionDetailsDTO.EditedFrom = Convert.ToString(dr["EditedFrom"]);
                        }

                        if (dtReqDetails.Columns.Contains("ItemType"))
                        {
                            objRequisitionDetailsDTO.ItemType = Convert.ToInt32(dr["ItemType"]);
                        }

                        if (dtReqDetails.Columns.Contains("ItemID"))
                        {
                            objRequisitionDetailsDTO.ItemID = Convert.ToInt64(dr["ItemID"]);
                        }

                        if (dtReqDetails.Columns.Contains("DefaultPullQuantity"))
                        {
                            objRequisitionDetailsDTO.DefaultPullQuantity = Convert.ToDecimal(dr["DefaultPullQuantity"]);
                        }

                        if (dtReqDetails.Columns.Contains("SerialNumberTracking"))
                        {
                            objRequisitionDetailsDTO.SerialNumberTracking = Convert.ToBoolean(dr["SerialNumberTracking"]);
                        }

                        if (dtReqDetails.Columns.Contains("LotNumberTracking"))
                        {
                            objRequisitionDetailsDTO.LotNumberTracking = Convert.ToBoolean(dr["LotNumberTracking"]);
                        }
                        if (dtReqDetails.Columns.Contains("DateCodeTracking"))
                        {
                            objRequisitionDetailsDTO.DateCodeTracking = Convert.ToBoolean(dr["DateCodeTracking"]);
                        }
                        if (dtReqDetails.Columns.Contains("IsCloseItem"))
                        {
                            objRequisitionDetailsDTO.IsCloseItem = Convert.ToBoolean(dr["IsCloseItem"]);
                        }

                        if (dtReqDetails.Columns.Contains("QuickListItemGUID"))
                        {
                            Guid.TryParse(Convert.ToString(dr["QuickListItemGUID"]), out tempguid);
                            objRequisitionDetailsDTO.QuickListItemGUID = tempguid;
                        }

                        if (dtReqDetails.Columns.Contains("ToolGuid"))
                        {
                            tempguid = Guid.Empty;
                            Guid.TryParse(Convert.ToString(dr["ToolGuid"]), out tempguid);
                            objRequisitionDetailsDTO.ToolGUID = tempguid;
                        }
                        if (dtReqDetails.Columns.Contains("TechnicianGuid"))
                        {
                            tempguid = Guid.Empty;
                            Guid.TryParse(Convert.ToString(dr["TechnicianGuid"]), out tempguid);
                            objRequisitionDetailsDTO.TechnicianGUID = tempguid;
                        }
                        if (dtReqDetails.Columns.Contains("ToolName"))
                        {
                            objRequisitionDetailsDTO.ToolName = Convert.ToString(dr["ToolName"]);
                        }
                        if (dtReqDetails.Columns.Contains("Technician"))
                        {
                            objRequisitionDetailsDTO.Technician = Convert.ToString(dr["Technician"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolCheckoutUDF1"))
                        {
                            objRequisitionDetailsDTO.ToolCheckoutUDF1 = Convert.ToString(dr["ToolCheckoutUDF1"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolCheckoutUDF2"))
                        {
                            objRequisitionDetailsDTO.ToolCheckoutUDF2 = Convert.ToString(dr["ToolCheckoutUDF2"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolCheckoutUDF3"))
                        {
                            objRequisitionDetailsDTO.ToolCheckoutUDF3 = Convert.ToString(dr["ToolCheckoutUDF3"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolCheckoutUDF4"))
                        {
                            objRequisitionDetailsDTO.ToolCheckoutUDF4 = Convert.ToString(dr["ToolCheckoutUDF4"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolCheckoutUDF5"))
                        {
                            objRequisitionDetailsDTO.ToolCheckoutUDF5 = Convert.ToString(dr["ToolCheckoutUDF5"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolSerialNumber"))
                        {
                            objRequisitionDetailsDTO.ToolSerialNumber = Convert.ToString(dr["ToolSerialNumber"]);
                        }
                        if (dtReqDetails.Columns.Contains("SupplierAccountGuid"))
                        {
                            tempguid = Guid.Empty;
                            Guid.TryParse(Convert.ToString(dr["SupplierAccountGuid"]), out tempguid);
                            if (tempguid != Guid.Empty)
                                objRequisitionDetailsDTO.SupplierAccountGuid = tempguid;
                        }
                        if (dtReqDetails.Columns.Contains("SupplierAccountNo"))
                        {
                            objRequisitionDetailsDTO.SupplierAccountNo = Convert.ToString(dr["SupplierAccountNo"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolCategoryID"))
                        {
                            objRequisitionDetailsDTO.ToolCategoryID = Convert.ToInt64(dr["ToolCategoryID"]);
                        }
                        if (dtReqDetails.Columns.Contains("ToolCategory"))
                        {
                            objRequisitionDetailsDTO.ToolCategory = Convert.ToString(dr["ToolCategory"]);
                        }
                        
                        if (dtReqDetails.Columns.Contains("ImagePath"))
                        {
                            objRequisitionDetailsDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                        }
                        if (dtReqDetails.Columns.Contains("ImageType"))
                        {
                            objRequisitionDetailsDTO.ImageType = Convert.ToString(dr["ImageType"]);
                        }
                        if (dtReqDetails.Columns.Contains("ItemImageExternalURL"))
                        {
                            objRequisitionDetailsDTO.ItemImageExternalURL = Convert.ToString(dr["ItemImageExternalURL"]);
                        }
                        if (dtReqDetails.Columns.Contains("PullOrderNumber"))
                        {
                            objRequisitionDetailsDTO.PullOrderNumber = Convert.ToString(dr["PullOrderNumber"]);
                        }
                        lstReqDetails.Add(objRequisitionDetailsDTO);
                    }
                }
            }
            return lstReqDetails;
        }
        public RequisitionMasterDTO GetRequisitionQty(long RoomID, long CompanyID, Guid ReqGUID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ReqGUID", ReqGUID) };
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequistionQTY] @RoomID,@CompanyID,@ReqGUID", params1).FirstOrDefault();
            }

        }

        public void CloseSelectedRequistionItem(Guid[] RequisitionDetailGUIDs, long UserId)
        {
            if (RequisitionDetailGUIDs != null && RequisitionDetailGUIDs.Count() > 0)
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RequisitionDetailGUIDs", string.Join(",", RequisitionDetailGUIDs)), new SqlParameter("@UserId", UserId), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.SqlQuery<ItemMasterDTO>("exec [CloseRequisitionItem] @RequisitionDetailGUIDs,@UserId,@RoomID,@CompanyID", params1).FirstOrDefault();
                }
            }
        }
        public double? GetRequisitionTotalCost(long RoomID, long CompanyID, Guid ReqGUID, int PriseSelectionOption, bool CalculateFromReqDetail)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var Result = (from RD in context.RequisitionDetails
                              join IM in context.ItemMasters on RD.ItemGUID equals IM.GUID
                              join cuom in context.CostUOMMasters on IM.CostUOMID equals cuom.ID into cuom_join
                              from CUOM in cuom_join.DefaultIfEmpty()
                              where RD.RequisitionGUID == ReqGUID
                                    && RD.CompanyID == CompanyID
                                    && RD.Room == RoomID
                                    && (RD.IsDeleted == null || RD.IsDeleted == false)
                                    && (IM.IsDeleted == null || IM.IsDeleted == false)
                              select new
                              {
                                  RequisitionGUID = RD.RequisitionGUID,
                                  Cost = (IM.Cost == null ? 0 : IM.Cost) * RD.QuantityRequisitioned,
                                  SellPrice = (IM.SellPrice == null ? 0 : IM.SellPrice) * RD.QuantityRequisitioned,
                                  ReqItemCost = (RD.ItemCost == null ? 0 : RD.ItemCost) * RD.QuantityRequisitioned,
                                  CostUOMValue = (CUOM == null || CUOM.CostUOMValue == null || CUOM.CostUOMValue == 0 ? 1 : CUOM.CostUOMValue)
                              }).GroupBy(x => x.RequisitionGUID).Select(G => new
                              {
                                  GUID = G.Key,
                                  TotalCost = G.Sum(x => (x.Cost / x.CostUOMValue)),
                                  TotalSellPrice = G.Sum(x => (x.SellPrice / x.CostUOMValue)),
                                  TotalReqItemCost = G.Sum(x => (x.ReqItemCost / x.CostUOMValue))
                              }).FirstOrDefault();

                if (Result != null)
                {
                    if (CalculateFromReqDetail == true)
                    {
                        return Result.TotalReqItemCost;
                    }
                    else
                    {
                        if (PriseSelectionOption == 1)
                        {
                            return Result.TotalSellPrice;
                        }
                        else
                        {
                            return Result.TotalCost;
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        public Dictionary<string, string> GetRequisitionUDFs(Guid RequisitionDetailGUID)
        {
            Dictionary<string, string> dictReturn = new Dictionary<string, string>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var onjUdf = (from RD in context.RequisitionDetails
                              where RD.GUID == RequisitionDetailGUID
                                    && (RD.IsDeleted == null || RD.IsDeleted == false)
                              select new
                              {
                                  UDF1 = RD.PullUDF1,
                                  UDF2 = RD.PullUDF2,
                                  UDF3 = RD.PullUDF3,
                                  UDF4 = RD.PullUDF4,
                                  UDF5 = RD.PullUDF5,
                              }).FirstOrDefault();

                if (onjUdf != null)
                {
                    dictReturn.Add("UDF1", (string.IsNullOrEmpty(onjUdf.UDF1) ? "" : onjUdf.UDF1));
                    dictReturn.Add("UDF2", (string.IsNullOrEmpty(onjUdf.UDF2) ? "" : onjUdf.UDF2));
                    dictReturn.Add("UDF3", (string.IsNullOrEmpty(onjUdf.UDF3) ? "" : onjUdf.UDF3));
                    dictReturn.Add("UDF4", (string.IsNullOrEmpty(onjUdf.UDF4) ? "" : onjUdf.UDF4));
                    dictReturn.Add("UDF5", (string.IsNullOrEmpty(onjUdf.UDF5) ? "" : onjUdf.UDF5));
                }
                else
                {
                    dictReturn.Add("UDF1", "");
                    dictReturn.Add("UDF2", "");
                    dictReturn.Add("UDF3", "");
                    dictReturn.Add("UDF4", "");
                    dictReturn.Add("UDF5", "");
                }
            }
            return dictReturn;
        }
        public double? GetRequisitionTotalSellPrice(long RoomID, long CompanyID, Guid ReqGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var Result = (from RD in context.RequisitionDetails
                              join IM in context.ItemMasters on RD.ItemGUID equals IM.GUID
                              join cuom in context.CostUOMMasters on IM.CostUOMID equals cuom.ID into cuom_join
                              from CUOM in cuom_join.DefaultIfEmpty()
                              where RD.RequisitionGUID == ReqGUID
                                    && RD.CompanyID == CompanyID
                                    && RD.Room == RoomID
                                    && (RD.IsDeleted == null || RD.IsDeleted == false)
                                    && (IM.IsDeleted == null || IM.IsDeleted == false)
                              select new
                              {
                                  RequisitionGUID = RD.RequisitionGUID,
                                  Cost = (IM.Cost == null ? 0 : IM.Cost) * RD.QuantityRequisitioned,
                                  SellPrice = (IM.SellPrice == null ? 0 : IM.SellPrice) * RD.QuantityRequisitioned,
                                  ReqItemSellPrice = (RD.ItemSellPrice) * RD.QuantityRequisitioned,
                                  CostUOMValue = (CUOM == null || CUOM.CostUOMValue == null || CUOM.CostUOMValue == 0 ? 1 : CUOM.CostUOMValue)
                              }).GroupBy(x => x.RequisitionGUID).Select(G => new
                              {
                                  GUID = G.Key,
                                  TotalCost = G.Sum(x => (x.Cost / x.CostUOMValue)),
                                  TotalSellPrice = G.Sum(x => (x.SellPrice / x.CostUOMValue)),
                                  TotalReqItemSellPrice = G.Sum(x => (x.ReqItemSellPrice / x.CostUOMValue))
                              }).FirstOrDefault();

                if (Result != null)
                {
                    return Result.TotalReqItemSellPrice;
                }
                else
                {
                    return 0;
                }
            }

        }

        public void UpdateRequisitionedSupplierAccountNumber(Guid RequisitionDetailsGuid, Guid ItemGuid, Guid SupplierAccountGuid, long RoomID, long CompanyID)
        {
            if (RequisitionDetailsGuid != null && ItemGuid != null && SupplierAccountGuid != null
                && RequisitionDetailsGuid != Guid.Empty && ItemGuid != Guid.Empty && SupplierAccountGuid != Guid.Empty)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID),
                                                       new SqlParameter("@CompanyId", CompanyID),
                                                       new SqlParameter("@RequisitionDetailsGuid", RequisitionDetailsGuid),
                                                       new SqlParameter("@ItemGuid", ItemGuid),
                                                       new SqlParameter("@SupplierAccountGuid", SupplierAccountGuid) };
                    context.Database.ExecuteSqlCommand("exec [UpdateRequisitionedSupplierAccountNumber] @RoomId,@CompanyId,@RequisitionDetailsGuid,@ItemGuid,@SupplierAccountGuid", params1);

                }
            }
        }

        #endregion

    }
}


