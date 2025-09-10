using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class ToolMoveInOutDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        //public ToolMoveInOutDetailDAL(base.DataBaseName)
        //{

        //}

        public ToolMoveInOutDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolMoveInOutDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        //public Int64 Insert(ToolMoveInOutDetailDTO objDTO)
        //{
        //    objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
        //    objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

        //    objDTO.Updated = DateTimeUtility.DateTimeNow;
        //    objDTO.Created = DateTimeUtility.DateTimeNow;

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        //AssetCategoryMaster obj = new AssetCategoryMaster();
        //        //obj.ID = 0;
        //        //obj.GUID = Guid.NewGuid();
        //        //obj.AssetCategory = objDTO.AssetCategory;
        //        //obj.CreatedBy = objDTO.CreatedBy;
        //        //obj.LastUpdatedBy = objDTO.LastUpdatedBy;
        //        //obj.Room = objDTO.Room;
        //        //obj.CompanyID = objDTO.CompanyID;
        //        //obj.Updated = objDTO.Updated;
        //        //obj.Created = objDTO.Created;
        //        //obj.IsDeleted = (bool)objDTO.IsDeleted;
        //        //obj.IsArchived = (bool)objDTO.IsArchived;
        //        //obj.UDF1 = objDTO.UDF1;
        //        //obj.UDF2 = objDTO.UDF2;
        //        //obj.UDF3 = objDTO.UDF3;
        //        //obj.UDF4 = objDTO.UDF4;
        //        //obj.UDF5 = objDTO.UDF5;
        //        //obj.AddedFrom = objDTO.AddedFrom;
        //        //obj.EditedFrom = objDTO.EditedFrom;
        //        //obj.ReceivedOn = objDTO.ReceivedOn;
        //        //obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
        //        //context.AssetCategoryMasters.Add(obj);
        //        //context.SaveChanges();
        //        //objDTO.ID = obj.ID;
        //        //return obj.ID;

        //    }
        //}

        public List<ToolMoveInOutDetailDTO> GetToolMoveInOutDetailByRoom(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ToolMoveInOutDetailDTO>("exec [GetToolMoveInOutDetailByRoomID] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public ToolMoveInOutDetailDTO GetToolMoveInOutDetailByIdOrGUID(long? ID, Guid? GUID, bool IsDeleted, bool IsArchived)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if ((ID ?? 0) > 0)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ID", ID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                    return context.Database.SqlQuery<ToolMoveInOutDetailDTO>("exec [GetToolMoveInOutDetailByID] @ID,@IsDeleted,@IsArchived", params1).FirstOrDefault();
                }
                else if ((GUID ?? Guid.Empty) != Guid.Empty)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                    return context.Database.SqlQuery<ToolMoveInOutDetailDTO>("exec [GetToolMoveInOutDetailByGUID] @GUID,@IsDeleted,@IsArchived", params1).FirstOrDefault();
                }

                return null;
            }
        }
        public ToolMoveInOutDetailDTO GetToolMoveInOutDetailByToolDetailGUID(Guid? ToolDetailGUID, bool IsDeleted, bool IsArchived)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", ToolDetailGUID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<ToolMoveInOutDetailDTO>("exec [GetToolMoveInOutDetailByToolDetailGUID] @GUID,@IsDeleted,@IsArchived", params1).FirstOrDefault();

            }
        }
        public List<ToolMoveInOutDetailDTO> GetAllToolMoveInOutDetailByToolDetailGUID(Guid? ToolDetailGUID, bool IsDeleted, bool IsArchived)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", ToolDetailGUID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<ToolMoveInOutDetailDTO>("exec [GetToolMoveInOutDetailByToolDetailGUID] @GUID,@IsDeleted,@IsArchived", params1).ToList();

            }
        }
        public List<ToolMoveInOutDetailDTO> GetToolMoveInOutDetailByToolKitGUID(Guid ToolKitGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolKitGUID", ToolKitGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ToolMoveInOutDetailDTO>("exec [GetToolMoveInOutDetailByToolKitGUID] @ToolKitGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public string QtyToMoveIn(ToolMoveInOutDetailDTO InOutDTO, Int64 RoomID, Int64 CompanyID, Int64 UserID,long EnterpriseId, string CultureCode, bool? AllowToolOrdering = false)
        {
            ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);
            ToolMasterDTO objToolItemDTO = null;

            try
            {
                if ((AllowToolOrdering ?? false) == true)
                {

                    objToolItemDTO = objToolDAL.GetToolByGUIDFull(InOutDTO.ToolItemGUID.Value);
                }
                else
                {
                    objToolItemDTO = objToolDAL.GetToolByGUIDPlain(InOutDTO.ToolItemGUID.Value);
                }
                if (objToolItemDTO != null)
                {
                    double TotalToolQty = 0;
                    double TotalCheckOutQty = 0;
                    double TotalCheckMQty = 0;
                    double TotalCheckOutForQty = InOutDTO.Quantity; // Quantity

                    TotalToolQty = objToolItemDTO.Quantity;
                    TotalCheckOutQty = objToolItemDTO.CheckedOutQTY ?? 0;
                    TotalCheckMQty = objToolItemDTO.CheckedOutMQTY ?? 0;
                    ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(base.DataBaseName);
                    ToolDetailDTO objToolDetailDTO = objToolDetailDAL.GetRecord(InOutDTO.GUID.ToString(), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0), false, false, false);
                    TotalCheckOutForQty = InOutDTO.Quantity;
                    if (TotalToolQty < (TotalCheckOutQty + TotalCheckMQty + TotalCheckOutForQty))
                    {
                        var toolResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseId, CompanyID);
                        string msgCheckoutQtyExceed = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCheckoutQtyExceed", toolResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                        return objToolItemDTO.Serial + " " + msgCheckoutQtyExceed; 
                    }
                    else
                    {
                        #region Tool Check Out
                        ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
                        ToolCheckInOutHistoryDTO objCICODTO = new ToolCheckInOutHistoryDTO();
                        objCICODTO.CompanyID = CompanyID;
                        objCICODTO.Created = DateTimeUtility.DateTimeNow;
                        objCICODTO.CreatedBy = UserID;
                        objCICODTO.IsArchived = false;
                        objCICODTO.IsDeleted = false;
                        objCICODTO.LastUpdatedBy = UserID;
                        objCICODTO.Room = RoomID;
                        objCICODTO.ToolGUID = InOutDTO.ToolItemGUID.Value;
                        objCICODTO.Updated = DateTimeUtility.DateTimeNow;
                        objCICODTO.UDF1 = string.Empty;
                        objCICODTO.UDF2 = string.Empty;
                        objCICODTO.UDF3 = string.Empty;
                        objCICODTO.UDF4 = string.Empty;
                        objCICODTO.UDF5 = string.Empty;
                        objCICODTO.AddedFrom = "Web";
                        objCICODTO.EditedFrom = "Web";
                        objCICODTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objCICODTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objCICODTO.IsOnlyFromItemUI = false;
                        objCICODTO.CheckedOutQTY = InOutDTO.Quantity;
                        objCICODTO.CheckedOutMQTY = 0;
                        objCICODTO.CheckedOutQTYCurrent = 0;
                        objCICODTO.CheckOutDate = DateTimeUtility.DateTimeNow;
                        objCICODTO.CheckOutStatus = "Check Out";
                        objCICODTO.ToolDetailGUID = InOutDTO.GUID;  // Remain to set; 
                        if (!string.IsNullOrWhiteSpace(InOutDTO.SerialNumber))
                        {
                            objCICODTO.SerialNumber = InOutDTO.SerialNumber;
                        }
                        objCICODAL.Insert(objCICODTO);
                        if ((AllowToolOrdering ?? false) == true)
                        {
                            ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                            objToolAssetQuantityDetailDTO.ToolGUID = objToolItemDTO.GUID;

                            objToolAssetQuantityDetailDTO.AssetGUID = null;

                            ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                            ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(RoomID, CompanyID, false, false, objToolItemDTO.GUID, UserID, "Web", "ToolMoveInOutDetailDAL>>Qtytomovein");

                            objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : objToolItemDTO.ToolLocationDetailsID;
                            objToolAssetQuantityDetailDTO.Quantity = 0;
                            objToolAssetQuantityDetailDTO.RoomID = RoomID;
                            objToolAssetQuantityDetailDTO.CompanyID = CompanyID;
                            objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                            objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                            objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                            objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                            objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyToMoveIn";
                            objToolAssetQuantityDetailDTO.ReceivedDate = null;
                            objToolAssetQuantityDetailDTO.InitialQuantityWeb = 0;
                            objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                            objToolAssetQuantityDetailDTO.ExpirationDate = null;
                            objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was CheckOut from Web While Kit movein.";
                            objToolAssetQuantityDetailDTO.CreatedBy = UserID;
                            objToolAssetQuantityDetailDTO.UpdatedBy = UserID;
                            objToolAssetQuantityDetailDTO.IsDeleted = false;
                            objToolAssetQuantityDetailDTO.IsArchived = false;

                            ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);
                            double Quantity = 0;
                            Quantity = InOutDTO.Quantity;
                            objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, Quantity, CheckoutGUID: objCICODTO.GUID, ReferalAction: "Move In", SerialNumber: (string.IsNullOrWhiteSpace(InOutDTO.SerialNumber)) ? string.Empty : InOutDTO.SerialNumber);
                        }
                        #endregion

                        #region ToolEdit
                        objToolItemDTO.CheckedOutQTY = objToolItemDTO.CheckedOutQTY.GetValueOrDefault(0) + (InOutDTO.Quantity);
                        objToolItemDTO.EditedFrom = "Web";
                        objToolItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolDAL.Edit(objToolItemDTO);
                        #endregion

                        #region ToolMoveInOutDetail Entry
                        ToolMoveInOutDetailDAL objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(base.DataBaseName);
                        ToolMoveInOutDetailDTO objToolMoveInDTO = new ToolMoveInOutDetailDTO();
                        objToolMoveInDTO.GUID = Guid.NewGuid();
                        objToolMoveInDTO.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                        objToolMoveInDTO.ToolItemGUID = InOutDTO.ToolItemGUID;
                        objToolMoveInDTO.MoveInOut = "IN";
                        objToolMoveInDTO.Quantity = InOutDTO.Quantity;
                        objToolMoveInDTO.ReasonFromMove = "From Kit Page";
                        objToolMoveInDTO.Created = DateTimeUtility.DateTimeNow;
                        objToolMoveInDTO.Updated = DateTimeUtility.DateTimeNow;
                        objToolMoveInDTO.CreatedBy = InOutDTO.CreatedBy;
                        objToolMoveInDTO.LastUpdatedBy = InOutDTO.LastUpdatedBy;
                        objToolMoveInDTO.CompanyID = CompanyID;
                        objToolMoveInDTO.Room = RoomID;
                        objToolMoveInDTO.IsDeleted = false;
                        objToolMoveInDTO.IsArchived = false;
                        objToolMoveInDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objToolMoveInDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolMoveInDTO.AddedFrom = "web";
                        objToolMoveInDTO.EditedFrom = "web";
                        objToolMoveInDTO.ToolDetailGUID = InOutDTO.GUID;
                        objToolMoveInDTO.ReasonFromMove = "From Kit Page";
                        objToolMoveInDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveIn";
                        Insert(objToolMoveInDTO);
                        #endregion

                        #region ToolDetail edit code

                        objToolDetailDTO.AvailableItemsInWIP = objToolDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) + InOutDTO.Quantity;

                        objToolDetailDTO.LastUpdatedBy = InOutDTO.LastUpdatedBy.GetValueOrDefault(0);
                        objToolDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolDetailDTO.EditedFrom = "Web";
                        objToolDetailDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveIn";
                        objToolDetailDAL.Edit(objToolDetailDTO);

                        #endregion

                        return "";

                    }
                }
                else
                {
                    var toolResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseId, CompanyID);
                    string msgToolDoesNotExist = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgToolDoesNotExist", toolResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                    return objToolItemDTO.Serial + msgToolDoesNotExist;
                }
            }
            finally
            {

            }
        }

        public string QtyToMoveOut(ToolMoveInOutDetailDTO InOutDTO, Int64 RoomID, Int64 CompanyID, Int64 UserID, long EnterpriseId, string CultureCode, bool? AllowToolOrdering = false)
        {
            ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);
            ToolMasterDTO objToolItemDTO = null;
            try
            {
                if ((AllowToolOrdering ?? false) == true)
                {

                    objToolItemDTO = objToolDAL.GetToolByGUIDFull(InOutDTO.ToolItemGUID.Value);
                }
                else
                {
                    objToolItemDTO = objToolDAL.GetToolByGUIDPlain(InOutDTO.ToolItemGUID.Value);
                }

                if (objToolItemDTO != null)
                {
                    // Validation check AvailableItemInWIP of Tool Detail
                    #region ToolDetail edit code
                    ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(base.DataBaseName);
                    ToolDetailDTO objToolDetailDTO = null;
                    objToolDetailDTO = objToolDetailDAL.GetRecord(InOutDTO.GUID.ToString(), InOutDTO.Room.GetValueOrDefault(0), InOutDTO.CompanyID.GetValueOrDefault(0), false, false, false);

                    if (InOutDTO.Quantity > objToolDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0))
                    {
                        var toolResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseId, CompanyID);
                        string msgNotEnoughQtyInWIP = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQtyInWIP", toolResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                        return msgNotEnoughQtyInWIP;
                    }
                        

                    objToolDetailDTO.AvailableItemsInWIP = objToolDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) - InOutDTO.Quantity;
                    objToolDetailDTO.LastUpdatedBy = InOutDTO.LastUpdatedBy.GetValueOrDefault(0);
                    objToolDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolDetailDTO.EditedFrom = "Web";
                    objToolDetailDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveOut";
                    objToolDetailDAL.Edit(objToolDetailDTO);
                    #endregion

                    ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(base.DataBaseName);
                    ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);

                    double TotalToolCheckOutQty = 0;
                    List<ToolCheckInOutHistoryDTO> lstCheckInOut = new List<ToolCheckInOutHistoryDTO>();
                    lstCheckInOut = objCICODAL.GetRecordByToolDetailGUID(InOutDTO.GUID, RoomID, CompanyID).Where(t => t.ToolDetailGUID != null).OrderBy(x => x.Created).ToList();
                    if (lstCheckInOut != null && lstCheckInOut.Count > 0)
                        TotalToolCheckOutQty = lstCheckInOut.Sum(x => x.CheckedOutQTY.GetValueOrDefault(0));

                    double TotalToolCheckInQty = 0;
                    List<ToolCheckInHistoryDTO> lstCheckIn = new List<ToolCheckInHistoryDTO>();
                    lstCheckIn = objCIDAL.GetRecordByToolDetailGUID(InOutDTO.GUID, RoomID, CompanyID).Where(t => t.ToolDetailGUID != null).OrderBy(x => x.Created).ToList();
                    if (lstCheckIn != null && lstCheckIn.Count > 0)
                        TotalToolCheckInQty = lstCheckIn.Sum(x => x.CheckedOutQTY.GetValueOrDefault(0));

                    double TotalCheckInForQty = InOutDTO.Quantity; // operation Quantity
                    List<ToolMoveInOutDetailDTO> lstToolMoveInOutDetailDTO = new ToolMoveInOutDetailDAL(base.DataBaseName).GetAllToolMoveInOutDetailByToolDetailGUID(InOutDTO.GUID, false, false).ToList();

                    double? MoveInTotal = lstToolMoveInOutDetailDTO.Where(t => t.MoveInOut.ToLower() == "in").Sum(t => t.Quantity);
                    double? MoveOutTotal = lstToolMoveInOutDetailDTO.Where(t => t.MoveInOut.ToLower() == "out").Sum(t => t.Quantity);

                    double? CIAgainstCOQty = 0; //This qty only increased if checkout done for qty of that tool. not in case of move in move out

                    if ((MoveOutTotal + TotalCheckInForQty) > MoveInTotal)//&& MoveInTotal < MoveOutTotal
                    {
                        var toolResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseId, CompanyID);
                        string msgTotalCheckInQtyExceed = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgTotalCheckInQtyExceed", toolResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                        return msgTotalCheckInQtyExceed;
                    }
                    else

                    {
                        double checkinQty = TotalCheckInForQty;
                        if ((MoveOutTotal + TotalCheckInForQty) <= MoveInTotal)
                        {
                            foreach (ToolCheckInOutHistoryDTO objToolCheckInOutHistory in lstCheckInOut.Where(o => o.CheckedOutQTY > o.CheckedOutQTYCurrent))
                            {
                                double CheckInByOut = 0; // Qty checkin Against Checkout
                                CheckInByOut = (double)lstCheckIn.Where(x => (x.CheckInCheckOutGUID == objToolCheckInOutHistory.GUID)).Sum(x => x.CheckedOutQTY);
                                if (checkinQty > 0)
                                {
                                    #region Tool CheckIn History Entry
                                    ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();
                                    objCIDTO.CheckInCheckOutGUID = objToolCheckInOutHistory.GUID;
                                    objCIDTO.CheckOutStatus = "Check In";
                                    // objCIDTO.CheckedOutQTY = TotalCheckInForQty;
                                    objCIDTO.CheckedOutQTY = objToolCheckInOutHistory.CheckedOutQTY - objToolCheckInOutHistory.CheckedOutQTYCurrent >= checkinQty ? checkinQty : objToolCheckInOutHistory.CheckedOutQTY - objToolCheckInOutHistory.CheckedOutQTYCurrent;
                                    objCIDTO.CheckedOutMQTY = 0;
                                    objCIDTO.CheckInDate = DateTimeUtility.DateTimeNow;
                                    objCIDTO.Created = DateTimeUtility.DateTimeNow;
                                    objCIDTO.CreatedBy = UserID;
                                    objCIDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objCIDTO.LastUpdatedBy = UserID;
                                    objCIDTO.Room = RoomID;
                                    objCIDTO.IsArchived = false;
                                    objCIDTO.IsDeleted = false;
                                    objCIDTO.CompanyID = CompanyID;
                                    objCIDTO.GUID = Guid.NewGuid();
                                    objCIDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objCIDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objCIDTO.AddedFrom = "Web";
                                    objCIDTO.EditedFrom = "Web";
                                    objCIDTO.IsOnlyFromItemUI = true;
                                    objCIDTO.ToolDetailGUID = InOutDTO.GUID;// remain to set
                                    objCIDTO.SerialNumber = InOutDTO.SerialNumber;
                                    objCIDAL.Insert(objCIDTO);
                                    if ((AllowToolOrdering ?? false) == true)
                                    {
                                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();

                                        objToolAssetQuantityDetailDTO.ToolGUID = objToolItemDTO.GUID;

                                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                                        objToolAssetQuantityDetailDTO.ToolBinID = objToolItemDTO.ToolLocationDetailsID;
                                        objToolAssetQuantityDetailDTO.Quantity = objCIDTO.CheckedOutQTY ?? 0;
                                        objToolAssetQuantityDetailDTO.RoomID = RoomID;
                                        objToolAssetQuantityDetailDTO.CompanyID = CompanyID;
                                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.WhatWhereAction = "ToolMoveinoutDetail>>QtyToMoveOut";
                                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolItemDTO.Quantity;
                                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout.";
                                        objToolAssetQuantityDetailDTO.CreatedBy = UserID;
                                        objToolAssetQuantityDetailDTO.UpdatedBy = UserID;
                                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                                        objToolAssetQuantityDetailDTO.IsArchived = false;

                                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);
                                        objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, CheckoutGUID: objToolCheckInOutHistory.GUID, CheckinGUID: objCIDTO.GUID, ReferalAction: "Move Out", SerialNumber: InOutDTO.SerialNumber);
                                    }
                                    #endregion

                                    #region Tool CheckInOut History Edit
                                    ToolCheckInOutHistoryDTO objPrvCICODTO = objCICODAL.GetTCIOHByGUIDPlain(objToolCheckInOutHistory.GUID, RoomID, CompanyID);
                                    objPrvCICODTO.CheckedOutQTYCurrent = objPrvCICODTO.CheckedOutQTYCurrent.GetValueOrDefault(0) + objCIDTO.CheckedOutQTY;
                                    objPrvCICODTO.Updated = DateTimeUtility.DateTimeNow;
                                    objPrvCICODTO.LastUpdatedBy = UserID;
                                    objPrvCICODTO.IsOnlyFromItemUI = true;
                                    objCICODAL.Edit(objPrvCICODTO);
                                    #endregion
                                    #region ToolMoveInOutDetail Entry
                                    double MOutQTy = objCIDTO.CheckedOutQTY ?? 0;
                                    foreach (ToolMoveInOutDetailDTO objToolMoveInDTONew in lstToolMoveInOutDetailDTO.Where(t => (t.Quantity) > (t.MoveOutQuntity ?? 0) && t.ToolItemGUID == InOutDTO.ToolItemGUID && t.ToolDetailGUID == InOutDTO.GUID).ToList())
                                    {
                                        if (MOutQTy > 0)
                                        {
                                            ToolMoveInOutDetailDAL objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(base.DataBaseName);
                                            ToolMoveInOutDetailDTO objToolMoveInDTO = new ToolMoveInOutDetailDTO();
                                            objToolMoveInDTO.GUID = Guid.NewGuid();
                                            objToolMoveInDTO.ToolDetailGUID = InOutDTO.GUID;
                                            objToolMoveInDTO.ToolItemGUID = InOutDTO.ToolItemGUID;
                                            objToolMoveInDTO.MoveInOut = "OUT";
                                            objToolMoveInDTO.Quantity = (objToolMoveInDTONew.Quantity - (objToolMoveInDTONew.MoveOutQuntity ?? 0) >= MOutQTy ? MOutQTy : (objToolMoveInDTONew.Quantity - (objToolMoveInDTONew.MoveOutQuntity ?? 0)));
                                            objToolMoveInDTO.Created = DateTimeUtility.DateTimeNow;
                                            objToolMoveInDTO.Updated = DateTimeUtility.DateTimeNow;
                                            objToolMoveInDTO.CreatedBy = InOutDTO.CreatedBy;
                                            objToolMoveInDTO.LastUpdatedBy = InOutDTO.LastUpdatedBy;
                                            objToolMoveInDTO.CompanyID = CompanyID;
                                            objToolMoveInDTO.Room = RoomID;
                                            objToolMoveInDTO.IsDeleted = false;
                                            objToolMoveInDTO.IsArchived = false;
                                            objToolMoveInDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            objToolMoveInDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objToolMoveInDTO.AddedFrom = "web";
                                            objToolMoveInDTO.EditedFrom = "web";
                                            objToolMoveInDTO.ReasonFromMove = "From Kit Page";
                                            objToolMoveInDTO.ToolDetailGUID = InOutDTO.GUID;
                                            objToolMoveInDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveOut";
                                            objToolMoveInDTO.RefMoveInOutGUID = objToolMoveInDTONew.GUID;
                                            Insert(objToolMoveInDTO);

                                            objToolMoveInDTONew.MoveOutQuntity = (objToolMoveInDTONew.MoveOutQuntity ?? 0) + objToolMoveInDTO.Quantity;
                                            objToolMoveInDTONew.Updated = DateTimeUtility.DateTimeNow;
                                            objToolMoveInDTONew.LastUpdatedBy = InOutDTO.CreatedBy;
                                            objToolMoveInDTONew.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objToolMoveInDTONew.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveOut";
                                            objToolMoveInDTONew.ReasonFromMove = "From Kit Page";
                                            Edit(objToolMoveInDTONew);

                                            MOutQTy = MOutQTy - objToolMoveInDTO.Quantity;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    #endregion
                                    checkinQty = checkinQty - objCIDTO.CheckedOutQTY ?? 0;

                                    CIAgainstCOQty = (CIAgainstCOQty ?? 0) + objCIDTO.CheckedOutQTY;
                                    //// Entry Create for # of checkouts
                                    //if (objToolItemDTO != null && objToolItemDTO.IsGroupOfItems.GetValueOrDefault(0) == 0 && objCIDTO != null && objCIDTO.ID > 0)
                                    //    MaintCreateForNoOfCheckoutAtCheckIn(objToolItemDTO, objCIDTO);

                                    //break;
                                    //objToolItemDTO.CheckedOutQTY = objToolItemDTO.CheckedOutQTY > checkinQty ? objToolItemDTO.CheckedOutQTY - checkinQty : objToolItemDTO.CheckedOutQTY - objToolItemDTO.CheckedOutQTY;
                                }
                                else
                                {
                                    break;
                                }

                            }

                        }
                        double QtyWipToGen = 0;
                        if (checkinQty > 0)
                        {
                            QtyWipToGen = checkinQty;
                            lstToolMoveInOutDetailDTO = lstToolMoveInOutDetailDTO.Where(t => t.ReasonFromMove == "Order Kit Received").ToList();
                            foreach (ToolMoveInOutDetailDTO t in lstToolMoveInOutDetailDTO.Where(t => (t.Quantity) > (t.MoveOutQuntity ?? 0) && t.ToolItemGUID == InOutDTO.ToolItemGUID && t.ToolDetailGUID == InOutDTO.GUID).ToList())
                            {
                                if (checkinQty > 0)
                                {
                                    double Qty = (t.Quantity >= checkinQty ? checkinQty : t.Quantity);
                                    ToolMoveInOutDetailDAL objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(base.DataBaseName);
                                    ToolMoveInOutDetailDTO objToolMoveInDTO = new ToolMoveInOutDetailDTO();
                                    objToolMoveInDTO.GUID = Guid.NewGuid();
                                    objToolMoveInDTO.ToolDetailGUID = InOutDTO.GUID;
                                    objToolMoveInDTO.ToolItemGUID = InOutDTO.ToolItemGUID;
                                    objToolMoveInDTO.MoveInOut = "OUT";
                                    objToolMoveInDTO.Quantity = Qty;
                                    objToolMoveInDTO.Created = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.CreatedBy = InOutDTO.CreatedBy;
                                    objToolMoveInDTO.LastUpdatedBy = InOutDTO.LastUpdatedBy;
                                    objToolMoveInDTO.CompanyID = CompanyID;
                                    objToolMoveInDTO.Room = RoomID;
                                    objToolMoveInDTO.IsDeleted = false;
                                    objToolMoveInDTO.IsArchived = false;
                                    objToolMoveInDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.AddedFrom = "web";
                                    objToolMoveInDTO.EditedFrom = "web";
                                    objToolMoveInDTO.ReasonFromMove = "From Kit Page";
                                    //objToolMoveInDTO.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                                    objToolMoveInDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveOut";
                                    objToolMoveInDTO.RefMoveInOutGUID = t.GUID;
                                    Insert(objToolMoveInDTO);

                                    t.MoveOutQuntity = (t.MoveOutQuntity ?? 0) + Qty;
                                    t.Updated = DateTimeUtility.DateTimeNow;
                                    t.LastUpdatedBy = InOutDTO.CreatedBy;
                                    t.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    t.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveOut";
                                    t.ReasonFromMove = "From Kit Page";
                                    Edit(t);
                                    if ((AllowToolOrdering ?? false) == true)
                                    {
                                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();

                                        objToolAssetQuantityDetailDTO.ToolGUID = objToolItemDTO.GUID;

                                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                                        objToolAssetQuantityDetailDTO.ToolBinID = objToolItemDTO.ToolLocationDetailsID;
                                        objToolAssetQuantityDetailDTO.Quantity = Qty;
                                        objToolAssetQuantityDetailDTO.RoomID = RoomID;
                                        objToolAssetQuantityDetailDTO.CompanyID = CompanyID;
                                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.WhatWhereAction = "ToolMoveinoutDetail>>QtyToMoveOut";
                                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolItemDTO.Quantity;
                                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout.";
                                        objToolAssetQuantityDetailDTO.CreatedBy = UserID;
                                        objToolAssetQuantityDetailDTO.UpdatedBy = UserID;
                                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                                        objToolAssetQuantityDetailDTO.IsArchived = false;

                                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);

                                        if (t.ReasonFromMove == "Order Kit Received")
                                        {
                                            objToolItemDTO.Quantity = (objToolItemDTO.Quantity) + Qty;
                                            objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out", SerialNumber: InOutDTO.SerialNumber);

                                        }
                                        else
                                        {
                                            objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out", SerialNumber: InOutDTO.SerialNumber);

                                        }
                                    }
                                    checkinQty = checkinQty - Qty;
                                }
                            }
                        }
                        checkinQty = TotalCheckInForQty - checkinQty;
                        #region Tool Edit entry
                        //objToolItemDTO.CheckedOutQTY = objToolItemDTO.CheckedOutQTY.GetValueOrDefault(0) - TotalCheckInForQty;
                        objToolItemDTO.CheckedOutQTY = objToolItemDTO.CheckedOutQTY > CIAgainstCOQty ? objToolItemDTO.CheckedOutQTY - CIAgainstCOQty : objToolItemDTO.CheckedOutQTY - objToolItemDTO.CheckedOutQTY;
                        objToolItemDTO.Quantity = objToolItemDTO.Quantity + QtyWipToGen;
                        objToolItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolItemDTO.EditedFrom = "Web";
                        objToolDAL.Edit(objToolItemDTO);
                        #endregion
                    }


                }
            }
            finally
            {

            }
            return "";
        }

        public Guid Insert(ToolMoveInOutDetailDTO InOutDTO)
        {
            InOutDTO.Updated = DateTimeUtility.DateTimeNow;
            InOutDTO.Created = DateTimeUtility.DateTimeNow;



            //InOutDTO.IsDeleted = false;
            //InOutDTO.IsArchived = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMoveInOutDetail objToolMoveInDTO = new ToolMoveInOutDetail();
                objToolMoveInDTO.GUID = Guid.NewGuid();
                objToolMoveInDTO.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                objToolMoveInDTO.ToolItemGUID = InOutDTO.ToolItemGUID;
                objToolMoveInDTO.MoveInOut = InOutDTO.MoveInOut;
                objToolMoveInDTO.Quantity = Convert.ToInt64(InOutDTO.Quantity);
                objToolMoveInDTO.Created = DateTimeUtility.DateTimeNow;
                objToolMoveInDTO.Updated = DateTimeUtility.DateTimeNow;
                objToolMoveInDTO.CreatedBy = InOutDTO.CreatedBy;
                objToolMoveInDTO.LastUpdatedBy = InOutDTO.LastUpdatedBy;
                objToolMoveInDTO.CompanyID = InOutDTO.CompanyID;
                objToolMoveInDTO.Room = InOutDTO.Room;
                objToolMoveInDTO.IsDeleted = InOutDTO.IsDeleted;
                objToolMoveInDTO.IsArchived = InOutDTO.IsArchived;
                objToolMoveInDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objToolMoveInDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objToolMoveInDTO.AddedFrom = InOutDTO.AddedFrom;
                objToolMoveInDTO.EditedFrom = InOutDTO.EditedFrom;
                objToolMoveInDTO.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                objToolMoveInDTO.ReasonFromMove = InOutDTO.ReasonFromMove;
                objToolMoveInDTO.WhatWhereAction = InOutDTO.WhatWhereAction;
                objToolMoveInDTO.RefMoveInOutGUID = InOutDTO.RefMoveInOutGUID;
                objToolMoveInDTO.MoveOutQuntity = InOutDTO.MoveOutQuntity;
                context.ToolMoveInOutDetails.Add(objToolMoveInDTO);


                context.SaveChanges();
                InOutDTO.ID = objToolMoveInDTO.ID;
                InOutDTO.GUID = objToolMoveInDTO.GUID ?? Guid.Empty;
            }
            return InOutDTO.GUID;
        }
        public Guid Edit(ToolMoveInOutDetailDTO InOutDTO)
        {
            InOutDTO.Updated = DateTimeUtility.DateTimeNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMoveInOutDetail obj = context.ToolMoveInOutDetails.Where(x => x.GUID == InOutDTO.GUID).FirstOrDefault();
                if (obj != null)
                {

                    obj.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                    obj.ToolItemGUID = InOutDTO.ToolItemGUID;
                    obj.MoveInOut = InOutDTO.MoveInOut;
                    obj.Quantity = Convert.ToInt64(InOutDTO.Quantity);

                    obj.Updated = InOutDTO.Updated;
                    obj.CreatedBy = InOutDTO.CreatedBy;
                    obj.LastUpdatedBy = InOutDTO.LastUpdatedBy;
                    obj.CompanyID = InOutDTO.CompanyID;
                    obj.Room = InOutDTO.Room;
                    obj.IsDeleted = InOutDTO.IsDeleted;
                    obj.IsArchived = InOutDTO.IsArchived;
                    obj.ReasonFromMove = InOutDTO.ReasonFromMove;
                    obj.ReceivedOn = InOutDTO.ReceivedOn;
                    obj.AddedFrom = InOutDTO.AddedFrom;
                    obj.EditedFrom = InOutDTO.EditedFrom;
                    obj.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                    obj.WhatWhereAction = InOutDTO.WhatWhereAction;
                    obj.RefMoveInOutGUID = InOutDTO.RefMoveInOutGUID;
                    obj.MoveOutQuntity = InOutDTO.MoveOutQuntity;
                    context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }

            }
            return InOutDTO.GUID;
        }
        public ResponseMessage BreakToolKit(MoveInOutQtyDetail objMoveInQty, Int64 RoomID, Int64 CompanyID, Int64 UserID, long EnterpriseId, string CultureCode)
        {
            ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);
            ResponseMessage response = new ResponseMessage();
            ToolMasterDTO ToolDTO = objToolDAL.GetToolByGUIDPlain(Guid.Parse(objMoveInQty.ItemGUID));
            List<ToolMoveInOutDetailDTO> lstKitMoveInOut = new List<ToolMoveInOutDetailDTO>();
            if (ToolDTO.Quantity < objMoveInQty.TotalQty)
            {
                var transferResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", CultureCode, EnterpriseId, CompanyID);
                string msgNotEnoughQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQuantity", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", CultureCode);
                response.IsSuccess = false;
                response.Message = msgNotEnoughQuantity;
                return response;
            }


            #region "Lot and other type logic"
            //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objMoveInQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == item.LocationID).OrderBy("CustomerOwnedQuantity DESC").ToList();
            Double takenQunatity = 0;

            ToolMoveInOutDetailDTO objKitMoveInOutDTO = new ToolMoveInOutDetailDTO()
            {

                MoveInOut = "BreakKit",
                ToolItemGUID = new Guid(objMoveInQty.ItemGUID),
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
                Quantity = objMoveInQty.TotalQty,
                WhatWhereAction = "ToolMoveInOutDetailDAL-->BreakToolKit",
            };
            //itemoil.KitDetailGUID = Guid.Parse(objMoveInQty.KitDetailGUID);
            takenQunatity += (ToolDTO.Quantity - takenQunatity);
            lstKitMoveInOut.Add(objKitMoveInOutDTO);

            ToolDTO.Quantity = ToolDTO.Quantity - objMoveInQty.TotalQty;
            ToolDTO.LastUpdatedBy = UserID;
            //ToolDTO.WhatWhereAction = "Kit-Break";
            objToolDAL.Edit(ToolDTO);

            foreach (var kitMoveinOut in lstKitMoveInOut)
            {
                Insert(kitMoveinOut);
            }

            ToolDetailDAL kitDetailDAL = new ToolDetailDAL(base.DataBaseName);
            List<ToolDetailDTO> lstKitDetailDTO = kitDetailDAL.GetAllRecordsByKitGUID(Guid.Parse(objMoveInQty.ItemGUID), RoomID, CompanyID, false, false, false).ToList();
            foreach (var item in lstKitDetailDTO)
            {

                item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) + (item.QuantityPerKit.GetValueOrDefault(0) * objMoveInQty.TotalQty);
                item.LastUpdatedBy = UserID;
                item.ReceivedOn = DateTimeUtility.DateTimeNow;
                item.EditedFrom = "Web";
                item.WhatWhereAction = "ToolMoveInOutDetailDAL-->BreakToolKit";
                kitDetailDAL.Edit(item);
            }

            var kitResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResKitMaster", CultureCode, EnterpriseId, CompanyID);
            string msgQuantityBreak = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityBreak", kitResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResKitMaster", CultureCode);

            response.IsSuccess = true;
            response.Message = msgQuantityBreak; 
            #endregion
            return response;
        }
        #endregion
    }
}
