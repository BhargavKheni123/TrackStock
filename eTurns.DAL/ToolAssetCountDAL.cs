using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class ToolAssetCountDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public ToolLocationDetailsDAL(base.DataBaseName)
        //{

        //}

        public ToolAssetCountDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolAssetCountDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region WI-4988


        public bool GetCountDetailGUIDByCountGuidForTool(Guid CountGUID, Guid ToolGUID, out Guid? CountDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCountDetail objToolAssetCountDetail = (from A in context.ToolAssetCountDetails
                                                                where A.ToolAssetCountGUID == CountGUID && A.ToolGUID == ToolGUID
                                                                      && (A.IsDeleted == false)
                                                                      && (A.IsArchived == false)
                                                                // && A.IsApplied == false
                                                                select A).FirstOrDefault();

                if (objToolAssetCountDetail != null)
                {
                    CountDetailGUID = objToolAssetCountDetail.GUID;
                    return true;
                }
                else
                {
                    CountDetailGUID = Guid.Empty;
                    return true;
                }
            }
        }

        public List<ToolAssetCountLineItemDetailDTO> GetLotDetailForCountByCountDetailGUID(Guid CountDetailGUID, Guid ToolGUID, string RoomDateFormat, Int64 CompanyId, Int64 RoomId, bool AppendExpDate = true)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CountDetailGUID", CountDetailGUID), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomId", RoomId) };
                return context.Database.SqlQuery<ToolAssetCountLineItemDetailDTO>("exec [csp_GetLotDetailForToolCountByCountDetailGUID] @CountDetailGUID,@ToolGUID,@CompanyId,@RoomId", params1).ToList();
            }
        }

        public ToolAssetCountDetailDTO GetToolCountdtlByGUId(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from ci in context.ToolAssetCountDetails
                        join ic in context.ToolAssetCounts on ci.ToolAssetCountGUID equals ic.GUID
                        join im in context.ToolMasters on ci.ToolGUID equals im.GUID
                        join TLD in context.ToolLocationDetails on ci.ToolBinID equals TLD.ID
                        join LM in context.LocationMasters on TLD.LocationID equals LM.ID
                        join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                        from ci_cc in ci_cc_join.DefaultIfEmpty()
                        join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                        from ci_cu in ci_cu_join.DefaultIfEmpty()
                        join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                        from ci_rm in ci_rm_join.DefaultIfEmpty()
                        where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.GUID == GUID
                        select new ToolAssetCountDetailDTO
                        {
                            AppliedDate = ci.AppliedDate,
                            ToolBinGUID = ci.ToolBinGUID ?? Guid.Empty,
                            ToolBinID = ci.ToolBinID,
                            Location = LM.Location,
                            CompanyId = ci.CompanyId,
                            QuantityDifference = ci.QuantityDifference,
                            Quantity = ci.Quantity,
                            CountQuantity = ci.CountQuantity,
                            CountDate = ic.CountDate,
                            CountItemStatus = ci.CountItemStatus,
                            CountLineItemDescription = ci.CountLineItemDescription,
                            CountLineItemDescriptionEntry = ci.CountLineItemDescription,
                            CountName = ic.CountName,
                            CountStatus = ic.CountStatus,
                            CountType = ic.CountType,
                            Created = ci.Created,
                            CreatedBy = ci.CreatedBy,
                            CreatedByName = ci_cc.UserName,
                            GUID = ci.GUID,
                            ID = ci.ID,
                            ToolAssetCountGUID = ci.ToolAssetCountGUID,
                            IsApplied = ci.IsApplied,
                            IsArchived = ci.IsArchived,
                            IsClosed = false,
                            IsDeleted = ci.IsDeleted,
                            ToolGUID = ci.ToolGUID ?? Guid.Empty,
                            ToolName = im.ToolName,
                            ToolQuantity = ci.Quantity,
                            LastUpdatedBy = ci.LastUpdatedBy,
                            RoomId = ci.RoomId,
                            RoomName = ci_rm.RoomName,
                            SaveAndApply = false,
                            Updated = ci.Updated,
                            UpdatedByName = ci_cu.UserName,
                            AddedFrom = ci.AddedFrom,
                            EditedFrom = ci.EditedFrom,
                            ReceivedOn = ci.ReceivedOn,
                            ReceivedOnWeb = ci.ReceivedOnWeb,
                            LotNumberTracking = im.LotNumberTracking,
                            SerialNumberTracking = im.SerialNumberTracking,
                            DateCodeTracking = im.DateCodeTracking
                        }).FirstOrDefault();
            }
        }

        public ToolAssetCountDetailDTO BeforeApplyAction(ToolAssetCountDetailDTO Editeditem, long UserId, Int64 CompanyId, Int64 RoomId, List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCountDetail objToolAssetCountDetail = context.ToolAssetCountDetails.FirstOrDefault(t => t.ID == Editeditem.ID);
                if (objToolAssetCountDetail != null)
                {
                    ToolMasterDTO objTool = new ToolMasterDAL(base.DataBaseName).GetToolByGUIDPlain(objToolAssetCountDetail.ToolGUID.GetValueOrDefault(Guid.Empty));

                    double ToolLocationQty = 0;
                    var q = context.ToolAssetQuantityDetails.Where(x => x.IsDeleted == false && x.IsArchived == false && x.ToolGUID == objToolAssetCountDetail.ToolGUID && x.ToolBinID == objToolAssetCountDetail.ToolBinID);
                    if (q.Any())
                    {
                        ToolLocationQty = q.Sum(t => (t.Quantity ?? 0));
                    }
                    if (Editeditem.SaveAndApply)
                    {
                        objToolAssetCountDetail.CountQuantity = Editeditem.CountQuantity;
                        objToolAssetCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                        objToolAssetCountDetail.Quantity = ToolLocationQty;
                        objToolAssetCountDetail.LastUpdatedBy = UserId;
                        objToolAssetCountDetail.Updated = DateTimeUtility.DateTimeNow;

                        //TODO: Start Jira Issue: WI-1560 below lin commented
                        //objInventoryCountDetail.IsApplied = true;

                        //----------------------------------------------------------------------------------
                        //
                        double QuantityDifference = 0;
                        bool someflag = GetCustomerConsignDiff(lstCountLineItemDetail, objTool.SerialNumberTracking, context, out QuantityDifference);
                        if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0 && someflag)
                        {
                            if (objToolAssetCountDetail.QuantityDifference != (-0.000000001))
                            {
                                objToolAssetCountDetail.QuantityDifference = QuantityDifference;
                            }
                        }
                        else
                        {
                            objToolAssetCountDetail.QuantityDifference = Editeditem.CountQuantity > 0 ? ((Editeditem.CountQuantity) - ToolLocationQty) : (-0.000000001);
                        }

                        //----------------------------------------------------------------------------------
                        //
                        if (!string.IsNullOrWhiteSpace(Editeditem.EditedFrom))
                        {
                            objToolAssetCountDetail.EditedFrom = Editeditem.EditedFrom;
                        }
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objToolAssetCountDetail.EditedFrom = "Web";
                            objToolAssetCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    else
                    {
                        objToolAssetCountDetail.Updated = DateTimeUtility.DateTimeNow;
                        objToolAssetCountDetail.Quantity = ToolLocationQty;
                        //TODO: Start Jira Issue: WI-1560 below lin commented
                        //objInventoryCountDetail.IsApplied = true;

                        //----------------------------------------------------------------------------------
                        //
                        double QuantityDifference = 0;
                        bool someflag = GetCustomerConsignDiff(lstCountLineItemDetail, objTool.SerialNumberTracking, context, out QuantityDifference);
                        if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0 && someflag)
                        {
                            if (objToolAssetCountDetail.QuantityDifference != (-0.000000001))
                            {
                                objToolAssetCountDetail.QuantityDifference = QuantityDifference;
                            }
                        }
                        else
                        {
                            objToolAssetCountDetail.QuantityDifference = Editeditem.CountQuantity > 0 ? ((Editeditem.CountQuantity) - ToolLocationQty) : (-0.000000001);
                        }
                        if (!string.IsNullOrWhiteSpace(Editeditem.EditedFrom))
                        {
                            objToolAssetCountDetail.EditedFrom = Editeditem.EditedFrom;
                        }
                        if (Editeditem.IsOnlyFromItemUI)
                        {
                            objToolAssetCountDetail.EditedFrom = "Web";
                            objToolAssetCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    //TODO: Start Jira Issue: WI-1560 below lin un-commented
                    context.SaveChanges();
                    Editeditem.GUID = objToolAssetCountDetail.GUID;
                    Editeditem.Quantity = ToolLocationQty;
                    Editeditem.QuantityDifference = (objToolAssetCountDetail.QuantityDifference == (-0.000000001)) ? (new double?()) : objToolAssetCountDetail.QuantityDifference;

                    Editeditem.TotalDifference = (Editeditem.QuantityDifference ?? 0);
                    Editeditem.ToolType = objTool.Type;
                    Editeditem.ToolGUID = objToolAssetCountDetail.ToolGUID.Value;
                    Editeditem.ToolBinID = objToolAssetCountDetail.ToolBinID;
                    if (Editeditem.IsOnlyFromItemUI)
                    {
                        Editeditem.EditedFrom = "Web";
                        Editeditem.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                }
            }
            return Editeditem;
        }

        public bool GetCountDetailGUIDByToolGUIDBinID(Guid CountGUID, Guid ToolGUID, long ToolBinID, out Guid? CountDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCountDetail objToolAssetCountDetail = (from A in context.ToolAssetCountDetails
                                                                where A.ToolAssetCountGUID == CountGUID && A.ToolGUID == ToolGUID && A.ToolBinID == ToolBinID
                                                                      && (A.IsDeleted == false)
                                                                      && (A.IsArchived == false)
                                                                      && A.IsApplied == false
                                                                select A).FirstOrDefault();

                if (objToolAssetCountDetail != null)
                {
                    CountDetailGUID = objToolAssetCountDetail.GUID;
                    return true;
                }
                else
                {
                    CountDetailGUID = Guid.Empty;
                    return true;
                }
            }
        }

        public List<Guid> GetCountDetailGUIDByToolGUIDBinID(Guid CountGUID, Guid ToolGUID, long ToolBinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<Guid> objToolAssetCountDetail = (from A in context.ToolAssetCountDetails
                                                      where A.ToolAssetCountGUID == CountGUID && A.ToolGUID == ToolGUID
                                                    && (A.IsDeleted == false)
                                                    && (A.IsArchived == false)
                                                      select A.GUID).ToList();

                return objToolAssetCountDetail;
            }
        }

        public List<ToolAssetCountLineItemDetailDTO> GetAllToolLocationsByToolGuid(Guid ToolGuid, Int64 CompanyId, Int64 RoomID, bool IsStage = false)
        {
            List<ToolAssetCountLineItemDetailDTO> lstDTOForAutoComplete = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolGuid", ToolGuid), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomId", RoomID) };
                lstDTOForAutoComplete = (from u in context.Database.SqlQuery<ToolAssetCountLineItemDetailDTO>(@"EXEC csp_GetAllToolLocationsByToolId @ToolGuid,@CompanyId,@RoomId", params1)
                                         group u by new { u.ToolBinID, u.Location, u.SerialNumber } into grp
                                         select new ToolAssetCountLineItemDetailDTO
                                         {
                                             ID = 0,
                                             SerialNumber = grp.Key.SerialNumber,
                                             Quantity = grp.Sum(x => (x.Quantity < 0 ? 0 : x.Quantity)),
                                             Location = grp.Key.Location,
                                             ToolBinID = grp.Key.ToolBinID
                                         }).ToList();

                if (lstDTOForAutoComplete == null)
                    lstDTOForAutoComplete = new List<ToolAssetCountLineItemDetailDTO>();

            }

            return lstDTOForAutoComplete;
        }

        public List<Guid> GetCountDetailGUIDByCountGuid(Guid CountGUID, Guid ToolGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<Guid> objToolAssetCountDetail = (from A in context.ToolAssetCountDetails
                                                      where A.ToolAssetCountGUID == CountGUID && A.ToolGUID == ToolGUID
                                                    && (A.IsDeleted == false)
                                                    && (A.IsArchived == false)
                                                      select A.GUID).ToList();

                return objToolAssetCountDetail;
            }
        }

        public ToolAssetCountDetailDTO PostApplyOnSignleLineItem(ToolAssetCountDetailDTO Editeditem, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCountDetail objInventoryCountDetail = context.ToolAssetCountDetails.FirstOrDefault(t => t.ID == Editeditem.ID);
                if (objInventoryCountDetail != null)
                {
                    objInventoryCountDetail.CountQuantity = Editeditem.CountQuantity;
                    objInventoryCountDetail.CountLineItemDescription = HttpUtility.UrlDecode(Editeditem.CountLineItemDescription);
                    objInventoryCountDetail.LastUpdatedBy = UserId;
                    objInventoryCountDetail.Updated = DateTimeUtility.DateTimeNow;
                    objInventoryCountDetail.IsApplied = true;

                    objInventoryCountDetail.AppliedDate = DateTime.Now.Date;
                    if (!string.IsNullOrWhiteSpace(Editeditem.EditedFrom))
                    {
                        objInventoryCountDetail.EditedFrom = Editeditem.EditedFrom;

                    }
                    objInventoryCountDetail.EditedFrom = "Adjustment Import";
                    objInventoryCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                context.SaveChanges();
                //Apply Header if all line items are applied
                ApplyCountHeader(objInventoryCountDetail.ToolAssetCountGUID, objInventoryCountDetail.RoomId, objInventoryCountDetail.CompanyId, objInventoryCountDetail.EditedFrom);
            }
            return Editeditem;
        }

        public void ApplyCountHeader(Guid ICGuid, Int64 RoomID, Int64 CompanyID, string EditedFrom)
        {
            List<ToolAssetCount> LstDtl = new List<ToolAssetCount>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCount objEntCnt = context.ToolAssetCounts.Where(x => x.GUID == ICGuid).FirstOrDefault();
                if (objEntCnt != null)
                {
                    objEntCnt.Updated = DateTimeUtility.DateTimeNow;
                    objEntCnt.IsApplied = true;
                    if (string.IsNullOrWhiteSpace(EditedFrom))
                    {
                        objEntCnt.EditedFrom = "Adjustment Import";
                    }
                    else
                    {
                        objEntCnt.EditedFrom = EditedFrom;
                    }
                    objEntCnt.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                context.SaveChanges();
            }
        }

        public void ApplyCountLineitem(DataTable DT, long RoomID, long CompanyID, long UserID, Guid CountGUID, Guid CountLineItemGUID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection ChildDbConnection = new SqlConnection(Connectionstring);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "CountATool", RoomID, CompanyID, UserID, CountGUID, CountLineItemGUID, DT);
            }
            catch
            {

            }
        }

        public bool GetCustomerConsignDiff_CLD(List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail, bool IsSerialTracking, eTurnsEntities context, out double QuantityDifference)
        {
            QuantityDifference = 0;

            try
            {
                //--------------------------------------------------------------
                //
                var CountLineItemDetailNew = (from A in lstCountLineItemDetail
                                              group A by new { A.ToolGUID, A.ToolBinID, A.SerialNumber } into G
                                              select new
                                              {
                                                  ToolGUID = G.Key.ToolGUID.Value,
                                                  ToolBinID = G.Key.ToolBinID.Value,
                                                  LotSerial = (((G.Key.SerialNumber ?? string.Empty).Trim() != "") ? G.Key.SerialNumber.Trim() : (G.Key.SerialNumber ?? string.Empty).Trim()),
                                                  CountQuantity = (G.Sum(x => x.CountQuantity) == null || G.Sum(x => x.CountQuantity) < 0 ? 0 : G.Sum(x => x.CountQuantity))
                                              }
                                              ).ToList();

                //--------------------------------------------------------------
                //
                Guid ToolGUID = lstCountLineItemDetail[0].ToolGUID.Value;
                long ToolBinID = lstCountLineItemDetail[0].ToolBinID.Value;
                string _lotSerial = lstCountLineItemDetail[0].LotSerialNumber ?? string.Empty;

                bool isLotSerialTrack = false;
                if (lstCountLineItemDetail[0].LotNumberTracking.GetValueOrDefault(false) == true || lstCountLineItemDetail[0].SerialNumberTracking.GetValueOrDefault(false) == true)
                {
                    isLotSerialTrack = true;
                }

                var lstILD = (from TM in context.ToolMasters
                              join TQD in context.ToolAssetQuantityDetails on new { ToolGUID = TM.GUID } equals new { ToolGUID = TQD.ToolGUID.Value }
                              join TLD in context.ToolLocationDetails on new { ToolBinID = TQD.ToolBinID.Value } equals new { ToolBinID = TLD.ID }
                              where TM.GUID == ToolGUID
                                    && TQD.ToolBinID == ToolBinID
                                    && TM.IsDeleted.Value == false
                                    && TM.IsArchived.Value == false
                                    && TQD.IsDeleted == false
                                    && TQD.IsArchived == false
                                    && TLD.IsDeleted.Value == false
                                    && TLD.IsArchieved.Value == false
                              select new
                              {
                                  ToolGUID = TM.GUID,
                                  ToolBinID = TQD.ToolBinID,
                                  LotSerial = (TM.SerialNumberTracking ? TQD.SerialNumber : ""),
                                  Quantity = (TQD.Quantity == null ? 0 : TQD.Quantity)
                              }
                 ).GroupBy(x => new { x.ToolGUID, x.ToolBinID, x.LotSerial }).Select(y => new
                 {
                     ToolGUID = y.Key.ToolGUID,
                     ToolBinID = y.Key.ToolBinID,
                     LotSerial = y.Key.LotSerial,
                     Quantity = y.Sum(z => z.Quantity)
                 }).ToList();

                if (isLotSerialTrack == true)
                {
                    lstILD = lstILD.Where(x => x.LotSerial == _lotSerial).ToList();
                }

                var varNewOld1 = (from A in lstILD
                                  join B1 in CountLineItemDetailNew on new { ToolGUID = A.ToolGUID, ToolBinID = A.ToolBinID.Value, LotSerial = A.LotSerial }
                                                                            equals new { ToolGUID = B1.ToolGUID, ToolBinID = B1.ToolBinID, LotSerial = B1.LotSerial }
                                  into B2
                                  from B in B2.DefaultIfEmpty()
                                  select new
                                  {
                                      ToolGUID = A.ToolGUID,
                                      ToolBinID = A.ToolBinID,
                                      Quantity = A.Quantity,
                                      NewQty = (B != null ? B.CountQuantity : (IsSerialTracking == true ? 0 : A.Quantity))
                                  }
                 ).ToList();

                var varNewOld2 = (from A in CountLineItemDetailNew
                                  where !lstILD.Any(x => x.ToolGUID == A.ToolGUID
                                                    && x.ToolBinID == A.ToolBinID
                                                    && x.LotSerial == A.LotSerial
                                                    )
                                  select new
                                  {
                                      ToolGUID = A.ToolGUID,
                                      ToolBinID = A.ToolBinID,
                                      Quantity = 0,
                                      NewQty = A.CountQuantity
                                  }
                                  ).ToList();

                //--------------------------------------------------------------
                //
                var varDiff = (from A in varNewOld1
                               group A by new { A.ToolGUID, A.ToolBinID } into G
                               select new
                               {
                                   QuantityDiffQty = G.Sum(x => x.NewQty) - G.Sum(x => x.Quantity)
                               }
                              ).FirstOrDefault();

                var varDiff2 = (from A in varNewOld2
                                group A by new { A.ToolGUID, A.ToolBinID } into G
                                select new
                                {
                                    QuantityDiffQty = G.Sum(x => x.NewQty) - G.Sum(x => x.Quantity)
                                }
                              ).FirstOrDefault();

                //--------------------------------------------------------------
                //
                if (varDiff != null)
                {
                    QuantityDifference = (varDiff != null && varDiff.QuantityDiffQty == null ? 0 : varDiff.QuantityDiffQty.Value);
                }

                if (varDiff2 != null)
                {
                    QuantityDifference = QuantityDifference + (varDiff2.QuantityDiffQty == null ? 0 : varDiff2.QuantityDiffQty.Value);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddToolAssetCountLineItemDetail(ToolAssetCountLineItemDetailDTO objCountLineItemDetail)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double QuantityDifference = 0;
                List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail = new List<ToolAssetCountLineItemDetailDTO>();
                lstCountLineItemDetail.Add(objCountLineItemDetail);
                bool someflag = GetCustomerConsignDiff_CLD(lstCountLineItemDetail, objCountLineItemDetail.SerialNumberTracking.GetValueOrDefault(false), context, out QuantityDifference);


                ToolAssetCountLineItemDetail objDATA = new ToolAssetCountLineItemDetail();
                objDATA.ToolGUID = objCountLineItemDetail.ToolGUID;
                objDATA.ToolBinID = objCountLineItemDetail.ToolBinID;
                objDATA.Quantity = objCountLineItemDetail.Quantity;
                objDATA.CountQuantity = objCountLineItemDetail.CountQuantity;
                objDATA.Comment = objCountLineItemDetail.Comment;
                objDATA.SerialNumber = objCountLineItemDetail.SerialNumber;
                objDATA.Received = objCountLineItemDetail.Received;
                objDATA.ReceivedDate = objCountLineItemDetail.ReceivedDate;
                objDATA.Cost = objCountLineItemDetail.Cost;
                objDATA.GUID = (objCountLineItemDetail.GUID != null ? objCountLineItemDetail.GUID.Value : Guid.NewGuid());
                objDATA.Created = (objCountLineItemDetail.Created != null ? objCountLineItemDetail.Created.Value : DateTime.Now);
                objDATA.Updated = (objCountLineItemDetail.Updated != null ? objCountLineItemDetail.Updated.Value : DateTime.Now);
                objDATA.CreatedBy = objCountLineItemDetail.CreatedBy;
                objDATA.LastUpdatedBy = objCountLineItemDetail.LastUpdatedBy;
                objDATA.IsDeleted = objCountLineItemDetail.IsDeleted;
                objDATA.IsArchived = objCountLineItemDetail.IsArchived;
                objDATA.CompanyID = objCountLineItemDetail.CompanyID;
                objDATA.RoomID = objCountLineItemDetail.RoomID;
                objDATA.CountGUID = objCountLineItemDetail.CountGUID;
                objDATA.CountDetailGUID = objCountLineItemDetail.CountDetailGUID;
                objDATA.ReceivedOn = (objCountLineItemDetail.ReceivedOn != null ? objCountLineItemDetail.ReceivedOn.Value : DateTime.Now);
                objDATA.ReceivedOnWeb = (objCountLineItemDetail.ReceivedOnWeb != null ? objCountLineItemDetail.ReceivedOnWeb.Value : DateTime.Now);
                objDATA.AddedFrom = objCountLineItemDetail.AddedFrom;
                objDATA.EditedFrom = objCountLineItemDetail.EditedFrom;

                objDATA.QuantityDifference = QuantityDifference;

                context.ToolAssetCountLineItemDetails.Add(objDATA);
                context.SaveChanges();
            }

            return true;
        }

        public bool UpdateToolAssetCountLineItemDetail(ToolAssetCountLineItemDetailDTO objCountLineItemDetail)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double QuantityDifference = 0;
                List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail = new List<ToolAssetCountLineItemDetailDTO>();
                lstCountLineItemDetail.Add(objCountLineItemDetail);
                bool someflag = GetCustomerConsignDiff_CLD(lstCountLineItemDetail, objCountLineItemDetail.SerialNumberTracking.GetValueOrDefault(false), context, out QuantityDifference);


                ToolAssetCountLineItemDetail objDATA = (from A in context.ToolAssetCountLineItemDetails
                                                        where A.GUID == objCountLineItemDetail.GUID
                                                        select A).FirstOrDefault();

                if (objDATA != null)
                {
                    objDATA.ToolBinID = objCountLineItemDetail.ToolBinID;
                    objDATA.Quantity = objCountLineItemDetail.Quantity;
                    objDATA.CountQuantity = objCountLineItemDetail.CountQuantity;
                    objDATA.Comment = objCountLineItemDetail.Comment;
                    objDATA.SerialNumber = objCountLineItemDetail.SerialNumber;
                    objDATA.Received = objCountLineItemDetail.Received;
                    objDATA.ReceivedDate = objCountLineItemDetail.ReceivedDate;
                    objDATA.Cost = objCountLineItemDetail.Cost;
                    objDATA.GUID = (objCountLineItemDetail.GUID != null ? objCountLineItemDetail.GUID.Value : Guid.NewGuid());
                    objDATA.Updated = (objCountLineItemDetail.Updated != null ? objCountLineItemDetail.Updated.Value : DateTime.Now);
                    objDATA.LastUpdatedBy = objCountLineItemDetail.LastUpdatedBy;
                    objDATA.IsDeleted = objCountLineItemDetail.IsDeleted;
                    objDATA.IsArchived = objCountLineItemDetail.IsArchived;
                    objDATA.CompanyID = objCountLineItemDetail.CompanyID;
                    objDATA.RoomID = objCountLineItemDetail.RoomID;
                    objDATA.CountGUID = objCountLineItemDetail.CountGUID;
                    objDATA.CountDetailGUID = objCountLineItemDetail.CountDetailGUID;
                    objDATA.ReceivedOn = (objCountLineItemDetail.ReceivedOn != null ? objCountLineItemDetail.ReceivedOn.Value : DateTime.Now);
                    objDATA.ReceivedOnWeb = (objCountLineItemDetail.ReceivedOnWeb != null ? objCountLineItemDetail.ReceivedOnWeb.Value : DateTime.Now);
                    objDATA.EditedFrom = objCountLineItemDetail.EditedFrom;

                    objDATA.QuantityDifference = QuantityDifference;

                    context.SaveChanges();
                }
            }

            return true;
        }

        public bool UpdateCountInToolAssetCountDetails(Guid? ToolCountDetailGUID, double? Quantity, List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCountDetail objDATA = (from A in context.ToolAssetCountDetails
                                                where A.GUID == ToolCountDetailGUID
                                                select A).FirstOrDefault();

                if (objDATA != null)
                {
                    //-----------------------------------------------------------------
                    //
                    double ToolLocationQty = 0;

                    var q = context.ToolAssetQuantityDetails.Where(t => t.IsDeleted == false && t.IsArchived == false && t.ToolGUID.Value == objDATA.ToolGUID.Value && t.ToolBinID.Value == objDATA.ToolBinID);
                    if (q.Any())
                    {
                        ToolLocationQty = q.Sum(t => (t.Quantity ?? 0));
                    }

                    //-----------------------------------------------------------------
                    //
                    ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(base.DataBaseName);
                    ToolMasterDTO objTool = objToolMasterDAL.GetToolByGUIDPlain(objDATA.ToolGUID.Value);

                    ////-----------------------------------------------------------------
                    ////
                    objDATA.CountQuantity = (Quantity == null ? 0 : Quantity.Value);

                    //----------------------------------------------------------------------------------
                    //
                    double Difference = 0;
                    if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0
                        && GetCustomerConsignDiff_CLD(lstCountLineItemDetail, (objTool == null ? false : objTool.SerialNumberTracking), context, out Difference))
                    {
                        objDATA.QuantityDifference = Difference;
                    }
                    else
                    {
                        objDATA.QuantityDifference = Quantity.HasValue ? ((Quantity ?? 0) - ToolLocationQty) : (-0.000000001);
                    }

                    context.SaveChanges();
                }
            }
            return true;
        }

        public List<ToolAssetCountLineItemDetailDTO> GetToolAssetCountLineItemDetailList(Guid ToolCountDetailGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail = (from CLD in context.ToolAssetCountLineItemDetails.AsEnumerable().Select(x =>
                {
                    return x;
                })
                                                                                where CLD.CountDetailGUID == ToolCountDetailGUID
                                                                                 && (CLD.IsDeleted == null || CLD.IsDeleted == false)
                                                                                 && (CLD.IsArchived == null || CLD.IsArchived == false)

                                                                                select new ToolAssetCountLineItemDetailDTO()
                                                                                {
                                                                                    ID = CLD.ID,
                                                                                    ToolGUID = CLD.ToolGUID,
                                                                                    ToolBinID = CLD.ToolBinID,
                                                                                    Comment = CLD.Comment,
                                                                                    Quantity = CLD.Quantity,
                                                                                    CountQuantity = CLD.CountQuantity,
                                                                                    SerialNumber = CLD.SerialNumber,
                                                                                    LotSerialNumber = (CLD.SerialNumber != null && CLD.SerialNumber.Trim() != "" ? CLD.SerialNumber.Trim() : ""),
                                                                                    Received = CLD.Received,
                                                                                    ReceivedDate = CLD.ReceivedDate,
                                                                                    Cost = CLD.Cost,
                                                                                    GUID = CLD.GUID,
                                                                                    Created = CLD.Created,
                                                                                    Updated = CLD.Updated,
                                                                                    CreatedBy = CLD.CreatedBy,
                                                                                    LastUpdatedBy = CLD.LastUpdatedBy,
                                                                                    IsDeleted = CLD.IsDeleted,
                                                                                    IsArchived = CLD.IsArchived,
                                                                                    CompanyID = CLD.CompanyID,
                                                                                    RoomID = CLD.RoomID,
                                                                                    CountGUID = CLD.CountGUID,
                                                                                    CountDetailGUID = CLD.CountDetailGUID,
                                                                                    ReceivedOn = CLD.ReceivedOn,
                                                                                    ReceivedOnWeb = CLD.ReceivedOnWeb,
                                                                                    AddedFrom = CLD.AddedFrom,
                                                                                    EditedFrom = CLD.EditedFrom,
                                                                                    QuantityDifference = CLD.QuantityDifference
                                                                                }
                                                                      ).ToList();

                return lstCountLineItemDetail;
            }
        }

        public bool DeleteCountLineItemDetail(Guid CountLineItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCountLineItemDetail objDATA = (from A in context.ToolAssetCountLineItemDetails
                                                        where A.GUID == CountLineItemGuid
                                                        select A).FirstOrDefault();

                if (objDATA != null)
                {
                    objDATA.IsDeleted = true;
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool UpdateCountLineItemDetailQtyForTool(Guid CountDetailGUID, double? CountQuantity, double? QuantityDifference)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ToolAssetCountDetail objCountLineItemDetail = (from A in context.ToolAssetCountDetails
                                                                   where A.GUID == CountDetailGUID
                                                                   select A).FirstOrDefault();
                    if (objCountLineItemDetail != null)
                    {
                        objCountLineItemDetail.CountQuantity = CountQuantity ?? 0;
                        objCountLineItemDetail.QuantityDifference = QuantityDifference ?? 0;
                    }
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public ToolAssetCountDTO SaveInventoryCountForImport(ToolAssetCountDTO objToolAssetCountDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool projectChanged = false;
                ToolAssetCount objToolAssetCount = null;

                if (objToolAssetCountDTO.ID > 0)
                {
                    objToolAssetCount = context.ToolAssetCounts.FirstOrDefault(t => t.ID == objToolAssetCountDTO.ID);
                    if (objToolAssetCount != null)
                    {

                        objToolAssetCount.CountName = objToolAssetCountDTO.CountName;
                        objToolAssetCount.CountItemDescription = objToolAssetCountDTO.CountItemDescription;
                        objToolAssetCount.CountDate = objToolAssetCountDTO.CountDate;
                        objToolAssetCount.UDF1 = objToolAssetCountDTO.UDF1;
                        objToolAssetCount.UDF2 = objToolAssetCountDTO.UDF2;
                        objToolAssetCount.UDF3 = objToolAssetCountDTO.UDF3;
                        objToolAssetCount.UDF4 = objToolAssetCountDTO.UDF4;
                        objToolAssetCount.UDF5 = objToolAssetCountDTO.UDF5;
                        objToolAssetCount.LastUpdatedBy = objToolAssetCountDTO.LastUpdatedBy;
                        objToolAssetCount.Updated = DateTimeUtility.DateTimeNow;
                        objToolAssetCount.AddedFrom = objToolAssetCountDTO.AddedFrom == null ? (objToolAssetCount.AddedFrom != null ? objToolAssetCount.AddedFrom : "Web") : objToolAssetCountDTO.AddedFrom;
                        objToolAssetCount.EditedFrom = objToolAssetCountDTO.EditedFrom ?? "Web";
                        objToolAssetCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolAssetCount.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        if (objToolAssetCountDTO != null && objToolAssetCountDTO.CountType != null && (!string.IsNullOrWhiteSpace(objToolAssetCountDTO.CountType)))
                            objToolAssetCount.CountType = objToolAssetCountDTO.CountType;
                        if (objToolAssetCount.ProjectSpendGUID != objToolAssetCountDTO.ProjectSpendGUID)
                        {
                            projectChanged = true;
                        }
                        objToolAssetCount.ProjectSpendGUID = objToolAssetCountDTO.ProjectSpendGUID;
                        objToolAssetCountDTO.RoomId = objToolAssetCount.RoomId;
                        objToolAssetCountDTO.CompanyId = objToolAssetCount.CompanyId;
                        objToolAssetCountDTO.CompleteCauseCountGUID = objToolAssetCount.CompleteCauseCountGUID;
                        objToolAssetCountDTO.CountCompletionDate = objToolAssetCount.CountCompletionDate;
                        objToolAssetCountDTO.CountDate = objToolAssetCount.CountDate;
                        objToolAssetCountDTO.CountStatus = objToolAssetCount.CountStatus;

                        objToolAssetCountDTO.Created = objToolAssetCount.Created;
                        objToolAssetCountDTO.CreatedBy = objToolAssetCount.CreatedBy;
                        objToolAssetCountDTO.GUID = objToolAssetCount.GUID;
                        objToolAssetCountDTO.IsArchived = objToolAssetCount.IsArchived;
                        objToolAssetCountDTO.IsAutomatedCompletion = objToolAssetCount.IsAutomatedCompletion;
                        objToolAssetCountDTO.IsDeleted = objToolAssetCount.IsDeleted;
                        objToolAssetCountDTO.LastUpdatedBy = objToolAssetCount.LastUpdatedBy;
                        objToolAssetCountDTO.Updated = objToolAssetCount.Updated;
                        objToolAssetCountDTO.Year = objToolAssetCount.Year;
                        objToolAssetCountDTO.AddedFrom = objToolAssetCountDTO.AddedFrom ?? "Web";
                        objToolAssetCountDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        if (objToolAssetCountDTO.IsOnlyFromItemUI)
                        {
                            objToolAssetCount.EditedFrom = "Web";
                            objToolAssetCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        if (projectChanged)
                        {
                            IQueryable<InventoryCountDetail> lst = context.InventoryCountDetails.Where(w => w.InventoryCountGUID == objToolAssetCount.GUID && w.IsDeleted == false);
                            if (lst.Any())
                            {
                                foreach (var item in lst)
                                {
                                    item.ProjectSpendGUID = objToolAssetCount.ProjectSpendGUID;
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                }
                else
                {
                    objToolAssetCount = new ToolAssetCount();
                    objToolAssetCount.ID = 0;
                    objToolAssetCount.GUID = objToolAssetCountDTO.GUID;
                    objToolAssetCount.CountName = objToolAssetCountDTO.CountName;
                    objToolAssetCount.CountItemDescription = objToolAssetCountDTO.CountItemDescription;
                    objToolAssetCount.CountType = objToolAssetCountDTO.CountType;
                    objToolAssetCount.CountStatus = objToolAssetCountDTO.CountStatus;
                    objToolAssetCount.UDF1 = objToolAssetCountDTO.UDF1;
                    objToolAssetCount.UDF2 = objToolAssetCountDTO.UDF2;
                    objToolAssetCount.UDF3 = objToolAssetCountDTO.UDF3;
                    objToolAssetCount.UDF4 = objToolAssetCountDTO.UDF4;
                    objToolAssetCount.UDF5 = objToolAssetCountDTO.UDF5;
                    objToolAssetCount.Created = objToolAssetCountDTO.Created;
                    objToolAssetCount.Updated = objToolAssetCountDTO.Updated;
                    objToolAssetCount.CreatedBy = objToolAssetCountDTO.CreatedBy;
                    objToolAssetCount.LastUpdatedBy = objToolAssetCountDTO.LastUpdatedBy;
                    objToolAssetCount.IsDeleted = objToolAssetCountDTO.IsDeleted;
                    objToolAssetCount.IsArchived = objToolAssetCountDTO.IsArchived;
                    objToolAssetCount.Year = objToolAssetCountDTO.Year;
                    objToolAssetCount.CompanyId = objToolAssetCountDTO.CompanyId;
                    objToolAssetCount.RoomId = objToolAssetCountDTO.RoomId;
                    objToolAssetCount.CountDate = objToolAssetCountDTO.CountDate;
                    objToolAssetCount.CountCompletionDate = objToolAssetCountDTO.CountCompletionDate;
                    objToolAssetCount.IsAutomatedCompletion = objToolAssetCountDTO.IsAutomatedCompletion;
                    objToolAssetCount.CompleteCauseCountGUID = objToolAssetCountDTO.CompleteCauseCountGUID;
                    objToolAssetCount.AddedFrom = objToolAssetCountDTO.AddedFrom ?? "Web";
                    objToolAssetCount.EditedFrom = objToolAssetCountDTO.EditedFrom ?? "Web";
                    objToolAssetCount.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolAssetCount.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objToolAssetCount.ProjectSpendGUID = objToolAssetCountDTO.ProjectSpendGUID;
                    context.ToolAssetCounts.Add(objToolAssetCount);
                    context.SaveChanges();
                    objToolAssetCountDTO.ID = objToolAssetCount.ID;
                    new AutoSequenceDAL(base.DataBaseName).UpdateNextToolCountNumberForImport(objToolAssetCountDTO.RoomId, objToolAssetCountDTO.CompanyId, objToolAssetCountDTO.CountName);
                }
            }
            return objToolAssetCountDTO;
        }

        public ToolAssetCountDTO GetToolCountByGUId(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from ci in context.ToolAssetCounts
                        join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                        from ci_cc in ci_cc_join.DefaultIfEmpty()
                        join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                        from ci_cu in ci_cu_join.DefaultIfEmpty()
                        join rm in context.Rooms on ci.RoomId equals rm.ID into ci_rm_join
                        from ci_rm in ci_rm_join.DefaultIfEmpty()
                        where ci.CompanyId == CompanyID && ci.RoomId == RoomID && ci.GUID == GUID
                        select new ToolAssetCountDTO
                        {
                            ID = ci.ID,
                            GUID = ci.GUID,
                            CountName = ci.CountName,
                            CountItemDescription = ci.CountItemDescription,
                            CountType = ci.CountType,
                            CountStatus = ci.CountStatus,
                            UDF1 = ci.UDF1,
                            UDF2 = ci.UDF2,
                            UDF3 = ci.UDF3,
                            UDF4 = ci.UDF4,
                            UDF5 = ci.UDF5,
                            Created = ci.Created,
                            Updated = ci.Updated,
                            CreatedBy = ci.CreatedBy,
                            LastUpdatedBy = ci.LastUpdatedBy,
                            IsDeleted = ci.IsDeleted,
                            IsArchived = ci.IsArchived,
                            Year = ci.Year,
                            CompanyId = ci.CompanyId,
                            RoomId = ci.RoomId,
                            CountDate = ci.CountDate,
                            CountCompletionDate = ci.CountCompletionDate,
                            IsAutomatedCompletion = ci.IsAutomatedCompletion,
                            CompleteCauseCountGUID = ci.CompleteCauseCountGUID,
                            CreatedByName = ci_cc.UserName,
                            UpdatedByName = ci_cu.UserName,
                            RoomName = ci_rm.RoomName,
                            IsApplied = ci.IsApplied,
                            ReceivedOn = ci.ReceivedOn,
                            ReceivedOnWeb = ci.ReceivedOnWeb,
                            AddedFrom = ci.AddedFrom,
                            EditedFrom = ci.EditedFrom,
                            IsClosed = ci.IsClosed,
                            ProjectSpendGUID = ci.ProjectSpendGUID
                        }).FirstOrDefault();
            }
        }

        public List<ToolAssetCountDetailDTO> GetAllLineItemsWithinToolCount(long CountId)
        {
            List<ToolAssetCountDetailDTO> lstLineitems = new List<ToolAssetCountDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLineitems = (from icd in context.ToolAssetCountDetails
                                join bm in context.LocationMasters on icd.ToolBinID equals bm.ID
                                join im in context.ToolMasters on icd.ToolGUID equals im.GUID
                                join ic in context.ToolAssetCounts on icd.ToolAssetCountGUID equals ic.GUID
                                join cb in context.UserMasters on icd.CreatedBy equals cb.ID into icd_cb_join
                                from icd_cb in icd_cb_join.DefaultIfEmpty()
                                join ub in context.UserMasters on icd.CreatedBy equals ub.ID into icd_ub_join
                                from icd_ub in icd_ub_join.DefaultIfEmpty()
                                where ic.ID == CountId && (icd.IsDeleted == false) && (icd.IsArchived == false)
                                select new ToolAssetCountDetailDTO
                                {
                                    AppliedDate = icd.AppliedDate,
                                    ToolBinGUID = icd.ToolBinGUID ?? Guid.Empty,
                                    ToolBinID = icd.ToolBinID,
                                    Location = bm.Location,
                                    CompanyId = bm.CompanyID ?? 0,

                                    QuantityDifference = icd.QuantityDifference,
                                    Quantity = icd.Quantity,
                                    CountQuantity = icd.CountQuantity,

                                    CountDate = ic.CountDate,
                                    CountItemStatus = icd.CountItemStatus,
                                    CountLineItemDescription = icd.CountLineItemDescription,
                                    CountLineItemDescriptionEntry = icd.CountLineItemDescription,
                                    CountName = ic.CountName,
                                    CountStatus = ic.CountStatus,
                                    CountType = ic.CountType,
                                    Created = icd.Created,
                                    CreatedBy = icd.CreatedBy,
                                    CreatedByName = icd_cb.UserName,
                                    GUID = icd.GUID,
                                    ID = icd.ID,
                                    IsApplied = icd.IsApplied,
                                    IsArchived = icd.IsArchived,
                                    IsClosed = false,
                                    IsDeleted = icd.IsDeleted,
                                    ToolGUID = icd.ToolGUID ?? Guid.Empty,
                                    ToolName = im.ToolName,
                                    LastUpdatedBy = icd.LastUpdatedBy,
                                    RoomId = icd.RoomId,
                                    RoomName = string.Empty,
                                    SaveAndApply = false,
                                    UDF1 = icd.UDF1,
                                    UDF2 = icd.UDF2,
                                    UDF3 = icd.UDF3,
                                    UDF4 = icd.UDF4,
                                    UDF5 = icd.UDF5,
                                    Updated = icd.Updated,
                                    UpdatedByName = icd_ub.UserName,
                                    AddedFrom = icd.AddedFrom,
                                    EditedFrom = icd.EditedFrom,
                                    ReceivedOn = icd.ReceivedOn,
                                    ReceivedOnWeb = icd.ReceivedOnWeb
                                }).ToList();
            }
            return lstLineitems;
        }

        public ToolAssetCountDetailDTO SaveToolCountLineItem(ToolAssetCountDetailDTO objToolAssetCountDetailDTO, bool SaveCountLineItemDetail = true, List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetCountDetail objToolAssetCountDetail = null;

                double LocationQty = 0;

                ToolMasterDTO objTool = new ToolMasterDTO();
                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(base.DataBaseName);

                objTool = objToolMasterDAL.GetToolByGUIDPlain(objToolAssetCountDetailDTO.ToolGUID);

                var q = context.ToolAssetQuantityDetails.Where(t => t.IsDeleted == false && t.IsArchived == false && t.ToolGUID == objToolAssetCountDetailDTO.ToolGUID && t.ToolBinID == objToolAssetCountDetailDTO.ToolBinID);
                if (q.Any())
                {
                    LocationQty = q.Sum(t => (t.Quantity ?? 0));
                }

                if (objToolAssetCountDetailDTO.ID > 0)
                {
                    objToolAssetCountDetail = context.ToolAssetCountDetails.FirstOrDefault(t => t.ID == objToolAssetCountDetailDTO.ID);
                    if (objToolAssetCountDetail != null)
                    {
                        objToolAssetCountDetail.CountQuantity = objToolAssetCountDetailDTO.CountQuantity;
                        objToolAssetCountDetail.Quantity = LocationQty;
                        objToolAssetCountDetail.CountLineItemDescription = objToolAssetCountDetailDTO.CountLineItemDescription;

                        //----------------------------------------------------------------------------------
                        //
                        double QuantityDifference = 0;

                        if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0
                            && GetCustomerConsignDiff(lstCountLineItemDetail, (objTool == null ? false : objTool.SerialNumberTracking), context, out QuantityDifference))
                        {
                            objToolAssetCountDetail.QuantityDifference = QuantityDifference;
                        }

                        //----------------------------------------------------------------------------------
                        //
                        if (objToolAssetCountDetailDTO.IsOnlyFromItemUI)
                        {
                            objToolAssetCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objToolAssetCountDetail.EditedFrom = objToolAssetCountDetailDTO.EditedFrom ?? "web";
                        }

                        context.SaveChanges();
                        objToolAssetCountDetailDTO.CountDetailGUID = objToolAssetCountDetailDTO.GUID;
                    }
                }
                else
                {
                    objToolAssetCountDetail = new ToolAssetCountDetail();
                    objToolAssetCountDetail.AppliedDate = objToolAssetCountDetailDTO.AppliedDate;
                    objToolAssetCountDetail.ToolBinGUID = objToolAssetCountDetailDTO.ToolBinGUID;
                    objToolAssetCountDetail.ToolBinID = objToolAssetCountDetailDTO.ToolBinID;
                    objToolAssetCountDetail.CountQuantity = objToolAssetCountDetailDTO.CountQuantity;
                    objToolAssetCountDetail.CountItemStatus = objToolAssetCountDetailDTO.CountItemStatus;
                    objToolAssetCountDetail.CountLineItemDescription = objToolAssetCountDetailDTO.CountLineItemDescription;
                    objToolAssetCountDetail.CountQuantity = objToolAssetCountDetailDTO.CountQuantity;
                    objToolAssetCountDetail.Created = objToolAssetCountDetailDTO.Created;
                    objToolAssetCountDetail.CreatedBy = objToolAssetCountDetailDTO.CreatedBy;
                    objToolAssetCountDetail.GUID = objToolAssetCountDetailDTO.GUID;
                    objToolAssetCountDetail.ID = objToolAssetCountDetailDTO.ID;
                    objToolAssetCountDetail.ToolAssetCountGUID = objToolAssetCountDetailDTO.ToolAssetCountGUID;
                    objToolAssetCountDetail.IsApplied = objToolAssetCountDetailDTO.IsApplied;
                    objToolAssetCountDetail.IsArchived = objToolAssetCountDetailDTO.IsArchived;
                    objToolAssetCountDetail.IsDeleted = objToolAssetCountDetailDTO.IsDeleted;
                    objToolAssetCountDetail.ToolGUID = objToolAssetCountDetailDTO.ToolGUID; ;
                    objToolAssetCountDetail.LastUpdatedBy = objToolAssetCountDetailDTO.LastUpdatedBy;
                    objToolAssetCountDetail.UDF1 = objToolAssetCountDetailDTO.UDF1;
                    objToolAssetCountDetail.UDF2 = objToolAssetCountDetailDTO.UDF2;
                    objToolAssetCountDetail.UDF3 = objToolAssetCountDetailDTO.UDF3;
                    objToolAssetCountDetail.UDF4 = objToolAssetCountDetailDTO.UDF4;
                    objToolAssetCountDetail.UDF5 = objToolAssetCountDetailDTO.UDF5;
                    objToolAssetCountDetail.Updated = objToolAssetCountDetailDTO.Updated;
                    objToolAssetCountDetail.RoomId = objToolAssetCountDetailDTO.RoomId;
                    objToolAssetCountDetail.CompanyId = objToolAssetCountDetailDTO.CompanyId;
                    objToolAssetCountDetailDTO.CountDetailGUID = objToolAssetCountDetailDTO.GUID;

                    //----------------------------------------------------------------------------------
                    //
                    double QuantityDifference = 0;

                    if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0
                        && GetCustomerConsignDiff(lstCountLineItemDetail, (objTool == null ? false : objTool.SerialNumberTracking), context, out QuantityDifference))
                    {
                        objToolAssetCountDetail.QuantityDifference = QuantityDifference;
                    }
                    else
                    {
                        objToolAssetCountDetail.QuantityDifference = objToolAssetCountDetailDTO.CountQuantity > 0 ? ((objToolAssetCountDetailDTO.CountQuantity) - QuantityDifference) : (-0.000000001);
                    }

                    //----------------------------------------------------------------------------------
                    //
                    objToolAssetCountDetail.AddedFrom = objToolAssetCountDetailDTO.AddedFrom ?? "web";
                    objToolAssetCountDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolAssetCountDetail.EditedFrom = objToolAssetCountDetailDTO.EditedFrom ?? "web";
                    objToolAssetCountDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    context.ToolAssetCountDetails.Add(objToolAssetCountDetail);
                    context.SaveChanges();
                    objToolAssetCountDetailDTO.ID = objToolAssetCountDetail.ID;
                    objToolAssetCountDetailDTO.CountDetailGUID = objToolAssetCountDetail.GUID;
                }

                //---------------------------------------------------------------------------------------------
                //
                if (SaveCountLineItemDetail == true)
                {
                    ToolAssetCountLineItemDetail objCountLineItemDetail = (from CLID in context.ToolAssetCountLineItemDetails
                                                                           where CLID.CountDetailGUID == objToolAssetCountDetail.GUID
                                                                           select CLID).FirstOrDefault();
                    if (objCountLineItemDetail != null)
                    {
                        objCountLineItemDetail.ToolGUID = objToolAssetCountDetail.ToolGUID;
                        objCountLineItemDetail.ToolBinID = objToolAssetCountDetail.ToolBinID;
                        objCountLineItemDetail.CountQuantity = objToolAssetCountDetail.CountQuantity;
                        objCountLineItemDetail.Updated = objToolAssetCountDetail.Updated;
                        objCountLineItemDetail.LastUpdatedBy = objToolAssetCountDetail.LastUpdatedBy;
                        objCountLineItemDetail.IsDeleted = objToolAssetCountDetail.IsDeleted;
                        objCountLineItemDetail.IsArchived = objToolAssetCountDetail.IsArchived;
                        objCountLineItemDetail.CompanyID = objToolAssetCountDetail.CompanyId;
                        objCountLineItemDetail.RoomID = objToolAssetCountDetail.RoomId;
                        objCountLineItemDetail.CountGUID = objToolAssetCountDetail.ToolAssetCountGUID;
                        objCountLineItemDetail.CountDetailGUID = objToolAssetCountDetail.GUID;
                        objCountLineItemDetail.EditedFrom = objToolAssetCountDetail.EditedFrom;

                        objCountLineItemDetail.QuantityDifference = objToolAssetCountDetail.QuantityDifference;

                        context.ToolAssetCountLineItemDetails.Add(objCountLineItemDetail);
                        context.SaveChanges();
                    }
                    else
                    {
                        objCountLineItemDetail = new ToolAssetCountLineItemDetail();
                        objCountLineItemDetail.GUID = Guid.NewGuid();
                        objCountLineItemDetail.ToolGUID = objToolAssetCountDetail.ToolGUID;
                        objCountLineItemDetail.ToolBinID = objToolAssetCountDetail.ToolBinID;
                        objCountLineItemDetail.CountQuantity = objToolAssetCountDetail.CountQuantity;
                        objCountLineItemDetail.SerialNumber = null;
                        objCountLineItemDetail.Comment = null;
                        objCountLineItemDetail.Received = null;
                        objCountLineItemDetail.ReceivedDate = DateTime.UtcNow;
                        objCountLineItemDetail.Cost = (from I in context.ToolMasters where I.GUID == objToolAssetCountDetail.ToolGUID select I.Cost).FirstOrDefault();
                        objCountLineItemDetail.Created = objToolAssetCountDetail.Created;
                        objCountLineItemDetail.Updated = objToolAssetCountDetail.Updated;
                        objCountLineItemDetail.CreatedBy = objToolAssetCountDetail.CreatedBy;
                        objCountLineItemDetail.LastUpdatedBy = objToolAssetCountDetail.LastUpdatedBy;
                        objCountLineItemDetail.IsDeleted = objToolAssetCountDetail.IsDeleted;
                        objCountLineItemDetail.IsArchived = objToolAssetCountDetail.IsArchived;
                        objCountLineItemDetail.CompanyID = objToolAssetCountDetail.CompanyId;
                        objCountLineItemDetail.RoomID = objToolAssetCountDetail.RoomId;
                        objCountLineItemDetail.CountGUID = objToolAssetCountDetail.ToolAssetCountGUID;
                        objCountLineItemDetail.CountDetailGUID = objToolAssetCountDetail.GUID;
                        objCountLineItemDetail.ReceivedOn = objToolAssetCountDetail.ReceivedOn;
                        objCountLineItemDetail.ReceivedOnWeb = objToolAssetCountDetail.ReceivedOnWeb;
                        objCountLineItemDetail.AddedFrom = objToolAssetCountDetail.AddedFrom;
                        objCountLineItemDetail.EditedFrom = objToolAssetCountDetail.EditedFrom;

                        objCountLineItemDetail.QuantityDifference = objToolAssetCountDetail.QuantityDifference;

                        context.ToolAssetCountLineItemDetails.Add(objCountLineItemDetail);
                        context.SaveChanges();
                    }
                }

                return objToolAssetCountDetailDTO;
            }
        }

        public bool GetCustomerConsignDiff(List<ToolAssetCountLineItemDetailDTO> lstCountLineItemDetail, bool IsSerialTracking, eTurnsEntities context, out double Difference)
        {
            Difference = 0;
            try
            {
                //--------------------------------------------------------------
                //
                var CountLineItemDetailNew = (from A in lstCountLineItemDetail
                                              group A by new { A.ToolGUID, A.ToolBinID, A.SerialNumber } into G
                                              select new
                                              {
                                                  ToolGuid = G.Key.ToolGUID.Value,
                                                  ToolBinID = G.Key.ToolBinID.Value,
                                                  LotSerial = (G.Key.SerialNumber ?? string.Empty).Trim(),
                                                  CountQuantity = (G.Sum(x => x.CountQuantity) == null || G.Sum(x => x.CountQuantity) < 0 ? 0 : G.Sum(x => x.CountQuantity)),
                                              }).ToList();

                //--------------------------------------------------------------
                //
                Guid ToolGUID = lstCountLineItemDetail[0].ToolGUID.Value;
                long ToolBinID = lstCountLineItemDetail[0].ToolBinID.Value;

                var lstILD = (from TL in context.ToolMasters
                              join TLD in context.ToolLocationDetails on new { ToolGuid = TL.GUID } equals new { ToolGuid = TLD.ToolGuid.Value }
                              join LM in context.LocationMasters on new { ToolBinID = TLD.LocationID.Value } equals new { ToolBinID = LM.ID }
                              join TLQD in context.ToolAssetQuantityDetails on new { ToolGuid = TL.GUID, ToolBinID = TLD.ID } equals new { ToolGuid = TLQD.ToolGUID.Value, ToolBinID = TLQD.ToolBinID.Value }
                              where TL.GUID == ToolGUID && LM.ID == TLD.LocationID && TLD.ID == ToolBinID
                                      && TL.IsDeleted == false && TL.IsArchived == false
                                      && TLD.IsDeleted == false && TLD.IsArchieved == false
                                      && LM.IsDeleted == false && LM.IsArchived == false
                                      && TLQD.IsDeleted == false && TLQD.IsArchived == false
                              select new
                              {
                                  ToolGuid = TL.GUID,
                                  ToolBinID = TLQD.ToolBinID.Value,
                                  LotSerial = (TL.SerialNumberTracking ? TLQD.SerialNumber : ""),
                                  Quantity = (TLQD.Quantity == null ? 0 : TLQD.Quantity),
                              }
                 ).GroupBy(x => new { x.ToolGuid, x.ToolBinID, x.LotSerial }).Select(y => new
                 {
                     ToolGuid = y.Key.ToolGuid,
                     ToolBinID = y.Key.ToolBinID,
                     LotSerial = y.Key.LotSerial,
                     Quantity = y.Sum(z => z.Quantity)
                 }).ToList();


                var varNewOld1 = (from A in lstILD
                                  join B1 in CountLineItemDetailNew on new { ToolGuid = A.ToolGuid, ToolBinID = A.ToolBinID, LotSerial = A.LotSerial }
                                                                            equals new { ToolGuid = B1.ToolGuid, ToolBinID = B1.ToolBinID, LotSerial = B1.LotSerial }
                                  into B2
                                  from B in B2.DefaultIfEmpty()
                                  select new
                                  {
                                      ToolGuid = A.ToolGuid,
                                      ToolBinID = A.ToolBinID,
                                      Quantity = A.Quantity,
                                      QuantityNewQty = (B != null ? B.CountQuantity : (IsSerialTracking == true ? 0 : A.Quantity))
                                  }
                 ).ToList();

                var varNewOld2 = (from A in CountLineItemDetailNew
                                  where !lstILD.Any(x => x.ToolGuid == A.ToolGuid
                                                    && x.ToolBinID == A.ToolBinID
                                                    && x.LotSerial == A.LotSerial
                                                    )
                                  select new
                                  {
                                      ToolGuid = A.ToolGuid,
                                      ToolBinID = A.ToolBinID,
                                      Quantity = 0,
                                      QuantityNewQty = A.CountQuantity
                                  }).ToList();

                //--------------------------------------------------------------
                //
                var varDiff = (from A in varNewOld1
                               group A by new { A.ToolGuid, A.ToolBinID } into G
                               select new
                               {
                                   QuantityDifference = G.Sum(x => x.QuantityNewQty) - G.Sum(x => x.Quantity)
                               }
                              ).FirstOrDefault();

                var varDiff2 = (from A in varNewOld2
                                group A by new { A.ToolGuid, A.ToolBinID } into G
                                select new
                                {
                                    QuantityDifference = G.Sum(x => x.QuantityNewQty) - G.Sum(x => x.Quantity)
                                }
                              ).FirstOrDefault();

                //--------------------------------------------------------------
                //

                if (varDiff != null)
                {
                    Difference = varDiff.QuantityDifference.GetValueOrDefault(0);
                }

                if (varDiff2 != null)
                {
                    Difference = Difference + (varDiff2.QuantityDifference.GetValueOrDefault(0));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
