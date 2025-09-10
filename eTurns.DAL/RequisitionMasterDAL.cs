using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public partial class RequisitionMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public RequisitionMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public RequisitionMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion

        #region [Class Methods]

        public List<RequisitionMasterDTO> GetRequisitionByDateAndStatusDashboard(long RoomID, long CompanyID, DateTime? FromDate, DateTime? ToDate, string ReqStatus)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@FromDate", FromDate ?? (object)DBNull.Value), new SqlParameter("@ToDate", ToDate ?? (object)DBNull.Value), new SqlParameter("@ReqStatus", ReqStatus ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByDateAndStatusDashboard] @RoomID,@CompanyID,@FromDate,@ToDate,@ReqStatus", params1).ToList();
            }
        }
        public RequisitionMasterDTO GetRequisitionByGUIDFull(Guid ReqGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqGUID", ReqGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByGUIDFull] @ReqGUID", params1).FirstOrDefault();
            }
        }

        public RequisitionMasterDTO GetArchivedRequisitionByGUIDFull(Guid ReqGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqGUID", ReqGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetArchivedRequisitionByGUIDFull] @ReqGUID", params1).FirstOrDefault();
            }
        }

        public RequisitionMasterDTO GetRequisitionByGUIDNormal(Guid ReqGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqGUID", ReqGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByGUIDNormal] @ReqGUID", params1).FirstOrDefault();
            }
        }
        public RequisitionMasterDTO GetRequisitionByGUIDPlain(Guid ReqGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqGUID", ReqGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByGUIDPlain] @ReqGUID", params1).FirstOrDefault();
            }
        }
        public RequisitionMasterDTO GetRequisitionByIDFull(long ReqID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqID", ReqID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByIDFull] @ReqID", params1).FirstOrDefault();
            }
        }
        public RequisitionMasterDTO GetRequisitionByIDNormal(long ReqID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqID", ReqID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByIDNormal] @ReqID", params1).FirstOrDefault();
            }
        }
        public RequisitionMasterDTO GetRequisitionByIDPlain(long ReqID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqID", ReqID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByIDPlain] @ReqID", params1).FirstOrDefault();
            }
        }

        public RequisitionMasterDTO GetArchivedRequisitionByIDPlain(long ReqID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqID", ReqID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetArchivedRequisitionByIDPlain] @ReqID", params1).FirstOrDefault();
            }
        }

        public List<RequisitionMasterDTO> GetRequisitionsByNameFull(string ReqName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqName", ReqName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionsByNameFull] @ReqName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<RequisitionMasterDTO> GetRequisitionsByNameNormal(string ReqName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqName", ReqName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionsByNameNormal] @ReqName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<RequisitionMasterDTO> GetRequisitionsByNamePlain(string ReqName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqName", ReqName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionsByNamePlain] @ReqName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<RequisitionMasterDTO> GetRequisitionsByNameSearch(string ReqName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqName", ReqName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionsByNameSearch] @ReqName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public RequisitionMasterDTO GetRequisitionByNameFull(string ReqName, long RoomID, long CompanyID)
        {
            return GetRequisitionsByNameFull(ReqName, RoomID, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public RequisitionMasterDTO GetRequisitionByNameNormal(string ReqName, long RoomID, long CompanyID)
        {
            return GetRequisitionsByNameNormal(ReqName, RoomID, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public RequisitionMasterDTO GetRequisitionByNamePlain(string ReqName, long RoomID, long CompanyID)
        {
            return GetRequisitionsByNamePlain(ReqName, RoomID, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public List<RequisitionMasterDTO> GetPendingRequisitions(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetPendingRequisitions] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<RequisitionMasterDTO> GetRequisitionsByRoomIDFull(long RoomID, long CompanyID, string ReqStatus)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ReqStatus", ReqStatus ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionsByRoomIDFull] @RoomID,@CompanyID,@ReqStatus", params1).ToList();
            }
        }
        public List<RequisitionMasterDTO> GetRequisitionsByWOFull(Guid WOGUID, long RoomID, long CompanyID, string ReqStatus)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ReqStatus", ReqStatus ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionsByWOFull] @WOGUID,@RoomID,@CompanyID,@ReqStatus", params1).ToList();
            }
        }
        public List<RequisitionMasterDTO> GetRequisitionsByWOPlain(Guid WOGUID, long RoomID, long CompanyID, string ReqStatus)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ReqStatus", ReqStatus ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionsByWOPlain] @WOGUID,@RoomID,@CompanyID,@ReqStatus", params1).ToList();
            }
        }
        public RequisitionMasterDTO GetLastRequisitionByRoomIDPlain(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetLastRequisitionByRoomIDPlain] @RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public IEnumerable<NarrowSearchDTO> GetRequistionMasterNarrowSearchRecords(string TextFieldName, string RequisitionCurrentTab, Int64 RoomID, Int64 CompanyId, List<long> SupplierIds, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RequisitionCurrentTab", RequisitionCurrentTab), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@SupplierIds", strSupplierIds) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetRequisitionNarrowSearch] @TextFieldName,@RequisitionCurrentTab,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@SupplierIds", params1).ToList();

            }
        }

        public RequisitionMasterDTO GetRequisitionHistoryByHistoryID(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionHistoryByHistoryID] @Id ", params1).FirstOrDefault();
            }
        }

        public RequisitionMasterDTO GetHistoryRecordMaintenance(long RequisitionID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", RequisitionID) };
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionHistoryByID] @Id ", params1).FirstOrDefault();
            }
        }

        public RequisitionMasterDTO Insert(RequisitionMasterDTO objDTO)
        {
            AutoSequenceDAL objAutoSeqDAL = null;

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            SetRequisitionReleaseNumber(objDTO);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionMaster obj = new RequisitionMaster();
                obj.ID = 0;
                obj.RequisitionNumber = objDTO.RequisitionNumber;
                obj.Description = objDTO.Description;
                obj.WorkorderGUID = objDTO.WorkorderGUID;
                obj.RequiredDate = objDTO.RequiredDate;
                obj.NumberofItemsrequisitioned = objDTO.NumberofItemsrequisitioned;
                obj.TotalCost = objDTO.TotalCost;
                obj.TotalSellPrice = objDTO.TotalSellPrice;
                obj.CustomerID = objDTO.CustomerID;
                obj.ProjectSpendGUID = objDTO.ProjectSpendGUID;
                obj.RequisitionStatus = objDTO.RequisitionStatus;
                obj.RequisitionType = objDTO.RequisitionType;
                obj.BillingAccountID = objDTO.BillingAccountID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                if (objDTO.GUID != null && objDTO.GUID != Guid.Empty)
                {
                    obj.GUID = objDTO.GUID;
                }
                else
                {
                    obj.GUID = Guid.NewGuid();
                }
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.CustomerGUID = objDTO.CustomerGUID;

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Requisition";

                obj.WhatWhereAction = objDTO.WhatWhereAction;
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                {
                    objDTO.AddedFrom = "Web";
                }
                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                {
                    objDTO.EditedFrom = "Web";
                }
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.SupplierId = objDTO.SupplierId;
                obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
                obj.ReleaseNumber = objDTO.ReleaseNumber;
                obj.StagingID = objDTO.StagingID;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.TechnicianID = objDTO.TechnicianID;
                if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                {
                    obj.RequesterID = objDTO.RequesterID;
                }
                if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                {
                    obj.ApproverID = objDTO.ApproverID;
                }
                context.RequisitionMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                if (objDTO.ID > 0)
                {
                    objAutoSeqDAL = new AutoSequenceDAL(base.DataBaseName);
                    objAutoSeqDAL.UpdateNextRequisitionNumber(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), 0, objDTO.RequisitionNumber);
                }

                return GetRequisitionByIDFull(objDTO.ID);
            }

        }
        public bool Edit(RequisitionMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            SetRequisitionReleaseNumber(objDTO);

            RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(base.DataBaseName);
            IEnumerable<RequisitionDetailsDTO> lstDetail = objDetailDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));

            if (lstDetail != null)
            {
                objDTO.NumberofItemsrequisitioned = lstDetail.Count();
                double? RequisitionTotalCost = null;
                double? RequisitionTotalSellPrice = null;
                foreach (var item in lstDetail)
                {
                    if (item.ItemGUID.HasValue)
                    {
                        double? Itemcost = new ItemMasterDAL(base.DataBaseName).CalculateAndGetItemCost(item.ItemGUID.Value, item.ItemCost, item.Room, item.CompanyID);
                        RequisitionTotalCost = RequisitionTotalCost.GetValueOrDefault(0) + (Itemcost.GetValueOrDefault(0) * (double)(item.QuantityApproved.GetValueOrDefault(0) > 0 ? item.QuantityApproved.GetValueOrDefault(0) : item.QuantityRequisitioned.GetValueOrDefault(0)));

                        double? ItemSellPrice = new ItemMasterDAL(base.DataBaseName).CalculateAndGetItemSellPrice(item.ItemGUID.Value, item.ItemSellPrice, item.Room, item.CompanyID);
                        RequisitionTotalSellPrice = RequisitionTotalSellPrice.GetValueOrDefault(0) + (ItemSellPrice.GetValueOrDefault(0) * (double)(item.QuantityApproved.GetValueOrDefault(0) > 0 ? item.QuantityApproved.GetValueOrDefault(0) : item.QuantityRequisitioned.GetValueOrDefault(0)));

                    }
                }
                //objDTO.TotalCost = lstDetail.Select(x => (double)(x.ItemCost.GetValueOrDefault(0)) * (double)(x.QuantityApproved.GetValueOrDefault(0) > 0 ? x.QuantityApproved.GetValueOrDefault(0) : x.QuantityRequisitioned.GetValueOrDefault(0))).Sum();
                objDTO.TotalCost = RequisitionTotalCost;
                objDTO.TotalSellPrice = RequisitionTotalSellPrice ?? 0;
            }
            else
            {
                objDTO.NumberofItemsrequisitioned = 0;
                objDTO.TotalCost = 0;
                objDTO.TotalSellPrice = 0;
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionMaster obj = new RequisitionMaster();
                obj.ID = objDTO.ID;
                obj.RequisitionNumber = objDTO.RequisitionNumber;
                obj.Description = objDTO.Description;
                obj.WorkorderGUID = objDTO.WorkorderGUID;
                obj.RequiredDate = objDTO.RequiredDate;
                obj.NumberofItemsrequisitioned = objDTO.NumberofItemsrequisitioned;
                obj.TotalCost = objDTO.TotalCost;
                obj.TotalSellPrice = objDTO.TotalSellPrice;
                obj.CustomerID = objDTO.CustomerID;
                obj.ProjectSpendGUID = objDTO.ProjectSpendGUID;
                obj.RequisitionStatus = objDTO.RequisitionStatus;
                obj.RequisitionType = objDTO.RequisitionType;
                obj.BillingAccountID = objDTO.BillingAccountID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = objDTO.GUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.CustomerGUID = objDTO.CustomerGUID;
                obj.SupplierId = objDTO.SupplierId;
                obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Requisition";

                obj.WhatWhereAction = objDTO.WhatWhereAction;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                {
                    objDTO.AddedFrom = "Web";
                }
                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                {
                    objDTO.EditedFrom = "Web";
                }
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReleaseNumber = objDTO.ReleaseNumber;
                obj.StagingID = objDTO.StagingID;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.TechnicianID = objDTO.TechnicianID;
                if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                {
                    obj.RequesterID = objDTO.RequesterID;
                }
                if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                {
                    obj.ApproverID = objDTO.ApproverID;
                }
                obj.RequisitionApprover = objDTO.RequisitionApprover;
                obj.RequisitionDataLog = objDTO.RequisitionDataLog;
                context.RequisitionMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID, long SessionUserId, long EnterpriseId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
                string Guids = string.Empty;

                if (!string.IsNullOrEmpty(IDs) && !string.IsNullOrWhiteSpace(IDs))
                {
                    var arr = IDs.Split(',');

                    if (arr != null && arr.Any())
                    {
                        Guids = string.Join(",", arr);
                    }
                }

                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", userid),
                                                    new SqlParameter("@Guids", Guids)
                                                };

                context.Database.ExecuteSqlCommand("exec [DeleteRequisitionByGuids] @UserID,@Guids", params1);

                if (!string.IsNullOrEmpty(IDs) && !string.IsNullOrWhiteSpace(IDs))
                {
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            RequisitionDetailsDAL objDAL = new RequisitionDetailsDAL(base.DataBaseName);
                            List<RequisitionDetailsDTO> lst = objDAL.GetReqLinesByReqGUIDPlain(Guid.Parse(item), 0, RoomID, CompanyID).ToList();
                            //string.Join(",", new List<string>(lst.Select(t => t.ID.ToString())).ToArray());
                            if (lst != null && lst.Count > 0)
                            {
                                objDAL.DeleteRecordsFromMaster(string.Join(",", new List<string>(lst.Select(t => t.GUID.ToString())).ToArray()), userid, RoomID, CompanyID, SessionUserId, EnterpriseId);
                                foreach (var reqdtlitem in lst)
                                {
                                    objDAL.UpdateItemOnRequisitionQty(reqdtlitem.ItemGUID.GetValueOrDefault(Guid.Empty), reqdtlitem.Room, reqdtlitem.CompanyID, reqdtlitem.LastUpdatedBy);
                                }
                            }

                        }
                    }
                }

                return true;
            }
        }

        public List<RequisitionMasterDTO> GetPagedRequisitionsForDashboard(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool UserConsignmentAllowed, string ReqStatus, string ReqType, long LoggedInUserId, DataTable ReqStatuses, DataTable ReqTypes, bool WithLineItems)
        {
            List<RequisitionMasterDTO> lstRequisitions = new List<RequisitionMasterDTO>();
            TotalCount = 0;
            RequisitionMasterDTO objRequisitionMasterDTO = new RequisitionMasterDTO();
            DataSet dsRequisitions = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstRequisitions;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            //sortColumnName = sortColumnName.Replace("WorkorderName", "WOName");
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            dsRequisitions = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedRequisitionsForDashboard", StartRowIndex, MaxRows, SearchTerm, sortColumnName, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, ReqStatus, ReqType, WithLineItems, ReqStatuses, ReqTypes);

            if (dsRequisitions != null && dsRequisitions.Tables.Count > 0)
            {
                lstRequisitions = DataTableHelper.ToList<RequisitionMasterDTO>(dsRequisitions.Tables[0]);

                if (lstRequisitions != null && lstRequisitions.Any())
                {
                    TotalCount = lstRequisitions.ElementAt(0).TotalRecords;
                }
            }
            return lstRequisitions;
        }

        public bool CheckDuplicateToolRequisition(string RequisitionGUID, Int64 RoomID, Int64 CompanyID)
        {
            bool IsDuplicateExist = false;
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetDuplicateToolRequisition", RequisitionGUID, RoomID, CompanyID);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Total"] != null && Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]) > 1)
                {
                    IsDuplicateExist = true;
                }
            }
            return IsDuplicateExist;
        }

        public List<RequisitionMasterDTO> GetRequisitionsForConsumeChart(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool UserConsignmentAllowed, string ReqStatus, string ReqType, long LoggedInUserId, DataTable ReqStatuses, DataTable ReqTypes, bool WithLineItems)
        {
            List<RequisitionMasterDTO> lstRequisitions = new List<RequisitionMasterDTO>();
            TotalCount = 0;
            DataSet dsRequisitions = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstRequisitions;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            sortColumnName = sortColumnName.Replace("WorkorderName", "WOName");

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            dsRequisitions = SqlHelper.ExecuteDataset(EturnsConnection, "GetRequisitionsForConsumeChart", StartRowIndex, MaxRows, SearchTerm, sortColumnName, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, ReqStatus, ReqType, WithLineItems, ReqStatuses, ReqTypes);

            if (dsRequisitions != null && dsRequisitions.Tables.Count > 0)
            {
                lstRequisitions = DataTableHelper.ToList<RequisitionMasterDTO>(dsRequisitions.Tables[0]);

                if (lstRequisitions != null && lstRequisitions.Any())
                {
                    TotalCount = lstRequisitions.ElementAt(0).TotalRecords;
                }
            }

            return lstRequisitions;
        }

        public List<RequisitionMasterDTO> GetPagedRecordsRequisitionList(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID,
                                          bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool UserConsignmentAllowed, string ReqStatus, string ReqType, long LoggedInUserId,
                                          DataTable ReqStatuses, DataTable ReqTypes, bool WithLineItems, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string spName = IsArchived ? "GetPagedRequisitions_Archive" : "GetPagedRequisitions";

            List<RequisitionMasterDTO> lstRequisitions = new List<RequisitionMasterDTO>();
            TotalCount = 0;
            RequisitionMasterDTO objRequisitionMasterDTO = new RequisitionMasterDTO();
            DataSet dsWorkOrders = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstRequisitions;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string Requiredatepara = null;
            string ReqCusts = null;
            string ReqWOs = null;
            string Tools = null;
            string ReqCreaters = null;
            string ReqUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            sortColumnName = sortColumnName.Replace("WorkorderName", "WOName");
            string ReqSupplier = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsWorkOrders = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, Requiredatepara, ReqCusts, ReqWOs, ReqCreaters, ReqUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, ReqStatus, ReqType, WithLineItems, ReqStatuses, ReqTypes, string.Empty, ReqSupplier);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (!string.IsNullOrEmpty(FieldsPara[27]))
                {
                    ReqStatus = FieldsPara[27];
                }
                if (!string.IsNullOrEmpty(FieldsPara[25]))
                {
                    ReqStatus = FieldsPara[25];
                }

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
                    ReqCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ReqUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");// Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss"); //Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss"); //Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");// Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[35]))
                {
                    string[] arrReplenishTypes = FieldsPara[35].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        Tools = Tools + supitem + "','";
                    }
                    Tools = Tools.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ReqWOs = ReqWOs + supitem + "','";
                    }
                    ReqWOs = ReqWOs.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[12]))
                {

                    string[] arrReplenishTypes = FieldsPara[12].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ReqCusts = ReqCusts + supitem + "','";
                    }
                    ReqCusts = ReqCusts.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[91]))
                {
                    ReqSupplier = Convert.ToString(FieldsPara[91]).TrimEnd();
                }
                string RequiredDate = Fields[1].Split('@')[14];

                if (!string.IsNullOrEmpty(FieldsPara[104]))
                {
                    ReqStatus = FieldsPara[104].TrimEnd(',');
                }

                dsWorkOrders = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, Requiredatepara, ReqCusts, ReqWOs, ReqCreaters, ReqUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, ReqStatus, ReqType, WithLineItems, ReqStatuses, ReqTypes, RequiredDate, ReqSupplier);
            }
            else
            {
                if (SearchTerm.Contains("[####]"))
                {

                    ReqStatus = SearchTerm.Split(new string[] { "[####]" }, StringSplitOptions.None)[1];
                    SearchTerm = SearchTerm.Split(new string[] { "[####]" }, StringSplitOptions.None)[0];
                    ReqStatus = ReqStatus.ToLower().Replace("all", "");
                }
                dsWorkOrders = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, Requiredatepara, ReqCusts, ReqWOs, ReqCreaters, ReqUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, ReqStatus, ReqType, WithLineItems, ReqStatuses, ReqTypes, string.Empty, ReqSupplier);
            }

            if (dsWorkOrders != null && dsWorkOrders.Tables.Count > 0)
            {
                DataTable dtWorkOrders = dsWorkOrders.Tables[0];
                if (dtWorkOrders.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtWorkOrders.Rows[0]["TotalRecords"]);
                    lstRequisitions = (from row in dtWorkOrders.AsEnumerable()
                                       select new RequisitionMasterDTO
                                       {
                                           ID = row.Field<long>("ID"),
                                           RequisitionNumber = row.Field<string>("RequisitionNumber"),
                                           Description = row.Field<string>("Description"),
                                           WorkorderGUID = row.Field<Guid?>("WorkorderGUID"),
                                           WorkorderName = row.Field<string>("WOName"),
                                           RequiredDate = row.Field<DateTime?>("RequiredDate"),
                                           RequiredDateStr = row.Field<string>("RequiredDateStr"),
                                           NumberofItemsrequisitioned = row.Field<int?>("NumberofItemsrequisitioned"),
                                           TotalCost = row.Field<double?>("TotalCost"),
                                           TotalSellPrice = row.Field<double>("TotalSellPrice"),
                                           CustomerID = row.Field<long?>("CustomerID"),
                                           Customer = row.Field<string>("Customer"),
                                           ProjectSpendGUID = row.Field<Guid?>("ProjectSpendGUID"),
                                           RequisitionStatus = row.Field<string>("RequisitionStatus"),
                                           RequisitionType = row.Field<string>("RequisitionType"),
                                           BillingAccountID = row.Field<long?>("BillingAccountID"),
                                           UDF1 = row.Field<string>("UDF1"),
                                           UDF2 = row.Field<string>("UDF2"),
                                           UDF3 = row.Field<string>("UDF3"),
                                           UDF4 = row.Field<string>("UDF4"),
                                           UDF5 = row.Field<string>("UDF5"),
                                           GUID = row.Field<Guid>("GUID"),
                                           Created = row.Field<DateTime?>("Created"),
                                           Updated = row.Field<DateTime?>("Updated"),
                                           CreatedBy = row.Field<long?>("CreatedBy"),
                                           LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                                           IsDeleted = row.Field<bool?>("IsDeleted"),
                                           IsArchived = row.Field<bool?>("IsArchived"),
                                           CompanyID = row.Field<long?>("CompanyID"),
                                           Room = row.Field<long?>("Room"),
                                           CreatedByName = row.Field<string>("CreatedByName"),
                                           UpdatedByName = row.Field<string>("UpdatedByName"),
                                           InCompletePullCount = row.Field<int?>("InCompletePullCount"),
                                           //RequisitionNumberForSorting = CommonDAL.GetSortingString(row.Field<string>("RequisitionNumber")),
                                           ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                                           ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                                           AddedFrom = row.Field<string>("AddedFrom"),
                                           EditedFrom = row.Field<string>("EditedFrom"),
                                           RoomName = row.Field<string>("RoomName"),
                                           SupplierName = row.Field<string>("SupplierName"),
                                           SupplierAccountNumberName = row.Field<string>("SupplierAccountNumberName"),
                                           ReleaseNumber = row.Field<string>("ReleaseNumber"),
                                           StagingID = row.Field<long?>("StagingID"),
                                           StagingName = row.Field<string>("StagingName"),
                                           MaterialStagingGUID = row.Field<Guid?>("MaterialStagingGUID")
                                       }).ToList();
                }
            }
            return lstRequisitions;
        }
        public void CloseRequisition(Guid RequisitionGuid, long RoomId, long CompanyId)
        {
            IQueryable<RequisitionDetail> lstReqItems = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstReqItems = (from ri in context.RequisitionDetails
                               where ri.RequisitionGUID == RequisitionGuid && (ri.IsDeleted ?? false) == false
                               select ri);
                if (lstReqItems.Any())
                {
                    foreach (var item in lstReqItems)
                    {
                        var q1 = (from ri in context.RequisitionDetails
                                  join rm in context.RequisitionMasters on ri.RequisitionGUID equals rm.GUID
                                  where ri.Room == RoomId && ri.CompanyID == CompanyId && (ri.IsDeleted ?? false) == false && (rm.IsDeleted ?? false) == false && rm.RequisitionStatus != "Closed" && ri.ItemGUID == item.ItemGUID
                                  select ri);
                        if (q1.Any())
                        {
                            double reqqty = q1.Sum(t => (t.QuantityRequisitioned ?? 0) - (t.QuantityPulled ?? 0));
                            ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == item.ItemGUID);
                            if (objItemMaster != null)
                            {
                                objItemMaster.RequisitionedQuantity = reqqty;

                            }
                        }
                        else
                        {
                            ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == item.ItemGUID);
                            if (objItemMaster != null)
                            {
                                objItemMaster.RequisitionedQuantity = 0;

                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
        }
        public void CloseRequisitionIfPullCompleted(Guid RequisitionGUID, long RoomID, long CompanyID, string EditedFrom, long SessionUserId = 0, bool iswebservice = false)
        {
            RequisitionMasterDAL objReqDAL = new RequisitionMasterDAL(base.DataBaseName);
            RequisitionMasterDTO objReqDTO = objReqDAL.GetRequisitionByGUIDPlain(RequisitionGUID);

            if (objReqDTO != null && objReqDTO.ID > 0)
            {
                RequisitionDetailsDAL objReqDtlDAL = new RequisitionDetailsDAL(base.DataBaseName);
                //IEnumerable<RequisitionDetailsDTO> objReqDtlDTO = objReqDtlDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.RequisitionGUID == objReqDTO.GUID).ToList();
                RequisitionMasterDTO objReqDTO1 = objReqDtlDAL.GetRequisitionQty(RoomID, CompanyID, objReqDTO.GUID);
                if (objReqDTO1 != null)
                {
                    objReqDTO.RequisitionedQuantity = objReqDTO1.RequisitionedQuantity;
                    objReqDTO.ApprovedQuantity = objReqDTO1.ApprovedQuantity;
                    objReqDTO.PulledQuantity = objReqDTO1.PulledQuantity;
                    if (!string.IsNullOrWhiteSpace(EditedFrom))
                    {
                        objReqDTO.EditedFrom = EditedFrom;
                    }
                }
                if (objReqDTO.ApprovedQuantity == objReqDTO.PulledQuantity)
                {
                    if (objReqDTO != null)
                    {
                        objReqDTO.RequisitionStatus = "Closed";
                        if (iswebservice)
                            objReqDTO.LastUpdatedBy = SessionUserId;
                        Edit(objReqDTO);
                    }
                }
            }
        }
        public string RequisitionNumberDuplicateCheck(long ID, string RequistionNumber, long CompanyID, long RoomID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.RequisitionMasters
                           where em.RequisitionNumber == RequistionNumber && em.IsArchived == false && em.IsDeleted == false && em.ID != ID && em.CompanyID == CompanyID
                           && em.Room == RoomID
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;
        }
        public Double GetTotal(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long ModuleId)
        {
            Double? Total = 0.0;
            int? PriseSelectionOption = 0;

            //----------------------------------------------------------------------------
            //
            eTurns.DAL.RoomDAL onjRoomDAL = new eTurns.DAL.RoomDAL(base.DataBaseName);
            RoomModuleSettingsDTO objRoomModuleSettingsDTO = onjRoomDAL.GetRoomModuleSettings(CompanyID, RoomID, ModuleId);
            if (objRoomModuleSettingsDTO != null)
            {
                PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;
            }

            if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
            {
                PriseSelectionOption = 1;
            }

            //----------------------------------------------------------------------------
            //
            eTurns.DAL.RequisitionDetailsDAL objRequisitionDetailsDAL = new eTurns.DAL.RequisitionDetailsDAL(base.DataBaseName);
            Total = objRequisitionDetailsDAL.GetRequisitionTotalCost(RoomID, CompanyID, GUID, (int)PriseSelectionOption, true);
            if (Total == null)
                Total = 0.0;

            //----------------------------------------------------------------------------
            //
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionMaster objRequisitionMaster = (from p in context.RequisitionMasters
                                                          where p.GUID == GUID && p.Room == RoomID && p.CompanyID == CompanyID
                                                          select p).FirstOrDefault();
                if (objRequisitionMaster != null && objRequisitionMaster.ID > 0)
                {
                    objRequisitionMaster.TotalCost = Total;
                    context.SaveChanges();
                }

            }

            //string Deleted = "0";
            //if(IsDeleted)
            //{
            //    Deleted = "1";
            //}
            //
            //string Qry = @"SELECT Isnull(SUM((CASE WHEN ISNULL(RED.QuantityApproved,0)>0 THEN ISNULL(RED.QuantityApproved,0) ELSE ISNULL(QuantityRequisitioned,0) END) * (Convert(float,ISNULL(IM.SellPrice,0)))/isnull(NULLIF(cm.costuomvalue,0),1)),0) as Total
            //                                                                            FROM RequisitionDetails RED Inner Join ItemMaster IM ON RED.ItemGuid = IM.[Guid]
            //                left outer join CostUOMMaster cm on im.CostUOMID = cm.id
            //                WHERE RED.RequisitionGuid  = '" + GUID + "' and IM.ROOM=" + RoomID + " AND IM.CompanyID=" + CompanyID + " and RED.Isdeleted= " + Deleted;
            //
            //
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    Total = (from u in context.Database.SqlQuery<Double>(Qry)
            //                select u
            //                ).FirstOrDefault();
            //    (from p in context.RequisitionMasters
            //     where p.GUID == GUID && p.Room == RoomID && p.CompanyID == CompanyID
            //     select p).ToList()
            //                            .ForEach(x => x.TotalCost = Total);
            //    context.SaveChanges();
            //
            //}

            return (double)Total;

        }
        public Double GetTotalSellPrice(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long ModuleId)
        {
            Double? TotalSellPrice = 0.0;
            eTurns.DAL.RequisitionDetailsDAL objRequisitionDetailsDAL = new eTurns.DAL.RequisitionDetailsDAL(base.DataBaseName);

            TotalSellPrice = objRequisitionDetailsDAL.GetRequisitionTotalSellPrice(RoomID, CompanyID, GUID);
            if (TotalSellPrice == null)
                TotalSellPrice = 0.0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RequisitionMaster objRequisitionMaster = (from p in context.RequisitionMasters
                                                          where p.GUID == GUID && p.Room == RoomID && p.CompanyID == CompanyID
                                                          select p).FirstOrDefault();

                if (objRequisitionMaster != null && objRequisitionMaster.ID > 0)
                {
                    objRequisitionMaster.TotalSellPrice = TotalSellPrice ?? 0;
                    context.SaveChanges();
                }

            }
            return (double)TotalSellPrice;
        }
        public double GetRequisitionReleaseNumber(long id, string RequistionNumber, long CompanyID, long RoomID)
        {
            double releaseNumber = 1;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var releaseNumberStr = context.RequisitionMasters.Where(u => !string.IsNullOrEmpty(u.ReleaseNumber) && u.Room == RoomID && u.CompanyID == CompanyID
                                                   && u.RequisitionNumber == RequistionNumber && (id < 1 || (id >= 1 && u.ID != id)) && u.IsDeleted == false && u.IsArchived == false).ToList().OrderByDescending(u => Convert.ToInt32(u.ReleaseNumber)).FirstOrDefault();

                if (releaseNumberStr != null && !string.IsNullOrEmpty(releaseNumberStr.ReleaseNumber))
                {
                    releaseNumber = Convert.ToInt32(releaseNumberStr.ReleaseNumber) + 1;
                }
            }
            return releaseNumber;
        }

        public double GetRequisitionReleaseNumberByReqGuid(Guid ReqGUID, string RequistionNumber, long CompanyID, long RoomID)
        {
            double releaseNumber = 1;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var releaseNumberStr = context.RequisitionMasters.Where(u => !string.IsNullOrEmpty(u.ReleaseNumber) && u.Room == RoomID && u.CompanyID == CompanyID
                                                   && u.RequisitionNumber == RequistionNumber && (ReqGUID == Guid.Empty || (ReqGUID != Guid.Empty && u.GUID != ReqGUID)) && u.IsDeleted == false && u.IsArchived == false).ToList().OrderByDescending(u => Convert.ToInt32(u.ReleaseNumber)).FirstOrDefault();

                if (releaseNumberStr != null && !string.IsNullOrEmpty(releaseNumberStr.ReleaseNumber))
                {
                    releaseNumber = Convert.ToInt32(releaseNumberStr.ReleaseNumber) + 1;
                }
            }
            return releaseNumber;
        }

        public void SetRequisitionReleaseNumber(RequisitionMasterDTO objDTO)
        {
            var releaseNumber = GetRequisitionReleaseNumber(objDTO.ID, objDTO.RequisitionNumber, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0));
            objDTO.ReleaseNumber = Convert.ToString(releaseNumber);
        }
        public List<RequisitionMasterDTO> GetRequisitionMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionMasterChangeLog] @GUID", params1).ToList();
            }
        }
        #endregion
        #region "2023-05-25 Linq to SP"
        public RequisitionMasterDTO GetRequisitionByReqDetailGUIDPlain(Guid ReqGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqDetailGUID", ReqGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionMasterDTO>("exec [GetRequisitionByReqDetailGUIDPlain] @ReqDetailGUID", params1).FirstOrDefault();
            }
        }
        #endregion
        public bool UpdateRequisitionStatusByEmailLink(long ReqisitionID, long? LastUpdatedBy, long? ApproverID, string RequisitionStatus, DateTime Updated, string RequisitionApprover, string RequisitionDataLog)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@ReqisitionID", ReqisitionID),
                                                new SqlParameter("@LastUpdatedBy", LastUpdatedBy),
                                                new SqlParameter("@ApproverID", ApproverID),
                                                new SqlParameter("@RequisitionStatus", RequisitionStatus),
                                                new SqlParameter("@Updated", Updated),
                                                new SqlParameter("@RequisitionApprover", RequisitionApprover),
                                                new SqlParameter("@RequisitionDataLog", RequisitionDataLog)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<bool>("exec UpdateRequisitionStatusByEmailLink @ReqisitionID,@LastUpdatedBy,@ApproverID,@RequisitionStatus,@Updated,@RequisitionApprover,@RequisitionDataLog", paramInnerCase).FirstOrDefault();
            }
        }

        public Int32 ClosedRequistionByIDs(string stringIDS, long? LastUpdatedBy)
        {
            try
            {
                var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@IdsString", stringIDS),
                                                new SqlParameter("@LastUpdatedBy", LastUpdatedBy)
                                            };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    return context.Database.SqlQuery<Int32>("exec ClosedRequistionByIDs @IdsString,@LastUpdatedBy", paramInnerCase).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}


