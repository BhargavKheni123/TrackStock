using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DAL
{
    public partial class RequisitionMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<RequisitionMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<RequisitionMasterDTO> ObjCache;
            #region "Both False"
            //Get Cached-Media
            //ObjCache = CacheHelper<IEnumerable<RequisitionMasterDTO>>.GetCacheItem("Cached_RequisitionMaster_" + CompanyID.ToString());
            //if (ObjCache == null)
            //{
            //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            //IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(@"
                                                            SELECT A.*, B.UserName AS 'CreatedByName', 
                                                            C.UserName AS UpdatedByName, D.RoomName
                                                            ,ISNULL(MSM.StagingName,'') AS StagingName
                                                            FROM RequisitionMaster A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                            WHERE A.CompanyID = " + CompanyID.ToString())
                            select new RequisitionMasterDTO
                            {
                                ID = u.ID,
                                RequisitionNumber = u.RequisitionNumber,
                                Description = u.Description,
                                WorkorderGUID = u.WorkorderGUID,
                                WorkorderName = u.WorkorderGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? "" : new WorkOrderDAL(base.DataBaseName).GetWorkOrderByGUIDPlain(u.WorkorderGUID.GetValueOrDefault(Guid.Empty)).WOName,
                                RequiredDate = u.RequiredDate,
                                NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                TotalCost = u.TotalCost,
                                TotalSellPrice = u.TotalSellPrice,
                                CustomerID = u.CustomerID,
                                CustomerGUID = u.CustomerGUID,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                RequisitionStatus = u.RequisitionStatus,
                                RequisitionType = u.RequisitionType,
                                BillingAccountID = u.BillingAccountID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                GUID = u.GUID,
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
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                AppendedBarcodeString = string.Empty,// objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Requisitions"),
                                //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                SupplierId = u.SupplierId,
                                SupplierAccountGuid = u.SupplierAccountGuid,
                                ReleaseNumber = u.ReleaseNumber,
                                StagingID = u.StagingID,
                                StagingName = u.StagingName,
                                MaterialStagingGUID = u.MaterialStagingGUID
                            }).AsParallel().ToList();
                //ObjCache = CacheHelper<IEnumerable<RequisitionMasterDTO>>.AddCacheItem("Cached_RequisitionMaster_" + CompanyID.ToString(), obj);
            }
            //}
            #endregion
            return ObjCache.Where(t => t.Room == RoomID);
        }
        public bool Edit(RequisitionMasterDTO objDTO, String dbConnString)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(dbConnString))
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

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.SupplierId = objDTO.SupplierId;
                obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
                obj.StagingID = objDTO.StagingID;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                context.RequisitionMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<RequisitionMasterDTO> ObjCache = CacheHelper<IEnumerable<RequisitionMasterDTO>>.GetCacheItem("Cached_RequisitionMaster_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<RequisitionMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<RequisitionMasterDTO> tempC = new List<RequisitionMasterDTO>();
                    //objDTO.RequisitionNumberForSorting = CommonDAL.GetSortingString(objDTO.RequisitionNumber);
                    tempC.Add(objDTO);
                    IEnumerable<RequisitionMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<RequisitionMasterDTO>>.AppendToCacheItem("Cached_RequisitionMaster_" + objDTO.CompanyID.ToString(), NewCache);
                }

                RequisitionDetailsDAL objReqDtlDAL = new RequisitionDetailsDAL(base.DataBaseName);
                IEnumerable<RequisitionDetailsDTO> objReqDetails = objReqDtlDAL.GetCachedData(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), dbConnString).Where(x => x.RequisitionGUID.GetValueOrDefault(Guid.Empty) == objDTO.GUID);

                foreach (var item in objReqDetails)
                {
                    objReqDtlDAL.UpdateItemOnRequisitionQty(item.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0));
                }

                if (objDTO.RequisitionStatus == "Closed" && objDTO.WorkorderGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    // need to close the work order as well if it is associated with this requisitino
                    WorkOrderDAL objWODAL = new WorkOrderDAL(base.DataBaseName);
                    WorkOrderDTO objWODTO = objWODAL.GetWorkOrderByGUIDPlain(objDTO.WorkorderGUID ?? Guid.Empty);
                    if (objWODTO != null)
                    {
                        objWODTO.WOStatus = "Close";
                        objWODTO.Updated = DateTimeUtility.DateTimeNow;
                        objWODTO.WhatWhereAction = "Requisition";
                        objWODTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objWODTO.EditedFrom = "Web";
                        objWODAL.Edit(objWODTO);
                    }
                }

                return true;
            }
        }
        public List<RequisitionMasterDTO> GetAllRequisitionRecords(long[] CompanyIds, long[] RoomIds, string[] Statuses, string[] ReqTypes, bool applydatefilter, string StartDate, string EndDate, string RoomDateFormat)
        {
            List<RequisitionMasterDTO> obj = null;
            DateTime sDate = new DateTime();
            DateTime eDate = new DateTime();

            if (applydatefilter)
            {
                sDate = Convert.ToDateTime(StartDate);
                eDate = Convert.ToDateTime(EndDate);
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.RequisitionMasters
                       where u.IsDeleted == false && u.IsArchived == false && CompanyIds.Contains(u.CompanyID ?? 0) && RoomIds.Contains(u.Room ?? 0)
                       && Statuses.Contains(u.RequisitionStatus) && ReqTypes.Contains(u.RequisitionType)
                       && (applydatefilter ? (u.Created >= sDate && u.Created <= eDate) : true)
                       select new RequisitionMasterDTO
                       {
                           GUID = u.GUID,
                           RequisitionNumber = u.RequisitionNumber,
                           RequisitionStatus = u.RequisitionStatus,
                           RequisitionType = u.RequisitionType,
                           Created = u.Created,
                           Room = u.Room
                       }).AsParallel().ToList();
            }
            return obj;

        }
        public RequisitionMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, string DBConnection)
        {
            //return DB_GetRecord(CompanyID, RoomID, id);

            RequisitionMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            if (id > 0)
            {
                using (var context = new eTurnsEntities(DBConnection))
                {
                    objOrder = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', 
                                                            CM.Customer, C.UserName AS UpdatedByName, D.RoomName,ISNULL(MSM.StagingName,'') AS StagingName	
                                                            FROM RequisitionMaster A 
                                                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                            LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
							                                LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID WHERE A.ID = " + id)
                                select new RequisitionMasterDTO
                                {
                                    ID = u.ID,
                                    RequisitionNumber = u.RequisitionNumber,
                                    Description = u.Description,
                                    WorkorderGUID = u.WorkorderGUID,
                                    WorkorderName = u.WorkorderGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? "" : new WorkOrderDAL(base.DataBaseName).GetWorkOrderByGUIDPlain(u.WorkorderGUID.GetValueOrDefault(Guid.Empty)).WOName,
                                    RequiredDate = u.RequiredDate,
                                    NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                    TotalCost = u.TotalCost,
                                    TotalSellPrice = u.TotalSellPrice,
                                    CustomerID = u.CustomerID,
                                    CustomerGUID = u.CustomerGUID,
                                    Customer = u.Customer,
                                    ProjectSpendGUID = u.ProjectSpendGUID,
                                    RequisitionStatus = u.RequisitionStatus,
                                    RequisitionType = u.RequisitionType,
                                    BillingAccountID = u.BillingAccountID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    GUID = u.GUID,
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
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    SupplierId = u.SupplierId,
                                    SupplierAccountGuid = u.SupplierAccountGuid,
                                    ReleaseNumber = u.ReleaseNumber,
                                    StagingID = u.StagingID,
                                    StagingName = u.StagingName,
                                    MaterialStagingGUID = u.MaterialStagingGUID
                                }).FirstOrDefault();

                }
            }
            return objOrder;

        }
        public IEnumerable<RequisitionMasterDTO> GetRequistionMasterUsingRoomId(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                IEnumerable<RequisitionMasterDTO> obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetRequistionMasterUsingRoomId] @RoomID,@CompanyID,@IsDeleted,@IsArchived ", params1).ToList();
                //obj.ToList().ForEach(m =>
                //{
                //    m.RequisitionNumberForSorting = CommonDAL.GetSortingString(m.RequisitionNumber);
                //});
                return obj;
            }
        }
        //Workorder controller >> SaveWO
        public RequisitionMasterDTO GetRequistionMasterUsingWoGuid(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid? WorkOrderGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@WoGuid", WorkOrderGuid) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetRequistionMasterUsingWoGuid] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@WoGuid ", params1).FirstOrDefault();


                //obj.RequisitionNumberForSorting = CommonDAL.GetSortingString(obj.RequisitionNumber);

                return obj;
            }
        }
        //RequisitionDAL>>GetPendingRequisitions
        public List<RequisitionMasterDTO> GetAlertMailRequisition(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                List<RequisitionMasterDTO> obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetAlertMailRequisition] @RoomID,@CompanyID  ", params1).ToList();

                //obj.ToList().ForEach(m =>
                //{
                //    m.RequisitionNumberForSorting = CommonDAL.GetSortingString(m.RequisitionNumber);
                //});
                return obj;
            }
        }
        //ChartController>>CreateChart
        public List<RequisitionMasterDTO> GetRequisitionDateFilter(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate, string Criteria)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@FromDate", FromDate), new SqlParameter("@ToDate", ToDate), new SqlParameter("@Criteria", Criteria) };
                List<RequisitionMasterDTO> obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetRequisitionDateFilter] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@FromDate,@ToDate,@Criteria  ", params1).ToList();

                //obj.ToList().ForEach(m =>
                //{
                //    m.RequisitionNumberForSorting = CommonDAL.GetSortingString(m.RequisitionNumber);
                //});
                return obj;
            }
        }
        //ChartController 
        public IEnumerable<RequisitionMasterDTO> GetRequisitionUsingNRecords(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, int NofRecords)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@NofRecords", NofRecords) };
                IEnumerable<RequisitionMasterDTO> obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetDatausingRoomIdNofRecords] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@NofRecords ", params1).ToList();
                //obj.ToList().ForEach(m =>
                //{
                //    m.RequisitionNumberForSorting = CommonDAL.GetSortingString(m.RequisitionNumber);
                //});
                return obj;
            }
        }
        //requistionmasterdal>>GetCachedData
        public IEnumerable<RequisitionMasterDTO> GetRequisitionMasterUsingConnectionstring(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string DbConnecstionString)
        {
            using (var context = new eTurnsEntities(DbConnecstionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                IEnumerable<RequisitionMasterDTO> obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetDatausingRoomId] @RoomID,@CompanyID,@IsDeleted,@IsArchived ", params1).ToList();
                //obj.ToList().ForEach(m =>
                //{
                //    m.RequisitionNumberForSorting = CommonDAL.GetSortingString(m.RequisitionNumber);
                //});
                return obj;
            }
        }
        //AutosequenceDAL>>GetNextRequisitionNumber
        public RequisitionMasterDTO GetRequisitionMasterLastRecord(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetDatausingRoomIdLastRecord] @RoomID,@CompanyID,@IsDeleted,@IsArchived ", params1).FirstOrDefault();
                //obj.RequisitionNumberForSorting = CommonDAL.GetSortingString(obj.RequisitionNumber);
                return obj;
            }
        }

        public RequisitionMasterDTO GetRequisitionById(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@id", id) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetRequisitionById] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@id ", params1).FirstOrDefault();
                //obj.RequisitionNumberForSorting = CommonDAL.GetSortingString(obj.RequisitionNumber);
                return obj;
            }
        }
        public RequisitionMasterDTO GetRequisitionByGuid(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@GUID", GUID) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetRequisitionByGuid] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@GUID ", params1).FirstOrDefault();
                //obj.RequisitionNumberForSorting = CommonDAL.GetSortingString(obj.RequisitionNumber);
                return obj;
            }
        }
        public RequisitionMasterDTO GetRecordIdOrGuid(Int64? Id, Guid? ReqGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id ?? (object)DBNull.Value), new SqlParameter("@ReqGUID", ReqGUID ?? (object)DBNull.Value) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetRecordIdOrGuid] @idIdReqGUID,@IsDeleted,@IsArchived,@GUID ", params1).FirstOrDefault();
                //obj.RequisitionNumberForSorting = CommonDAL.GetSortingString(obj.RequisitionNumber);
                return obj;
            }
        }
        public IEnumerable<RequisitionMasterDTO> GetPendingRequisitionsNew(Int64 RoomID, Int64 CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
                IEnumerable<RequisitionMasterDTO> obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetPendingRequisitionsNew] @RoomID,@CompanyID ", params1).ToList();
                //obj.ToList().ForEach(m =>
                //{
                //    m.RequisitionNumberForSorting = CommonDAL.GetSortingString(m.RequisitionNumber);
                //});
                return obj;
            }
        }
        public RequisitionMasterDTO GetHistoryRecordNew(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetHistoryRequisitionsById] @ID ", params1).FirstOrDefault();

                return obj;

            }
        }
        public RequisitionMasterDTO GetHistoryRecordMaintenanceNew(Int64 RequisitionID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", RequisitionID) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetHistoryRecordMaintenanceNew] @ID ", params1).FirstOrDefault();
                return obj;
            }
        }
        public void CloseRequisitionNew(Guid RequisitionGuid, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RequisitionGuid", RequisitionGuid), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyId", CompanyId) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [CloseRequisition] @RequisitionGuid,@RoomId ,@CompanyId ", params1).FirstOrDefault();

            }
        }
        public void CloseRequisitionIfPullCompletedNew(Guid RequisitionGUID, long RoomID, long CompanyID, string EditedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RequisitionGuid", RequisitionGUID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@EditedFrom", EditedFrom) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [CloseRequisitionIfPullComplete] @RequisitionGuid,@RoomId ,@CompanyId,@EditedFrom ", params1).FirstOrDefault();

            }
        }
        public RequisitionMasterDTO GetRecordByIdNew(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@id", id), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                RequisitionMasterDTO obj = context.ExecuteStoreQuery<RequisitionMasterDTO>("exec [GetRecordByIdNew] @id,@RoomID,@CompanyID ", params1).FirstOrDefault();

                return obj;

            }
        }
        public double GetTotalByRequistionGuid(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long ModuleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@ModuleId", ModuleId) };
                double obj = context.ExecuteStoreQuery<double>("exec [GetTotalNew] @GUID,@RoomId,@CompanyId,@IsArchived,@IsDeleted,@ModuleId ", params1).FirstOrDefault();

                return obj;

            }
        }
        public IEnumerable<RequisitionMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string MainFilter = "")
        {
            //Get Cached-Media
            IEnumerable<RequisitionMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);

            //foreach (var item in ObjCache)
            //{
            //    item.RequisitionNumber_ForSorting = CommonDAL.GetSortingString(item.RequisitionNumber);
            //}

            if (!string.IsNullOrEmpty(MainFilter) && MainFilter.ToString().Trim().ToLower() == "true")
            {
                ObjCache = ObjCache.Where(x => x.NumberofItemsrequisitioned.GetValueOrDefault(0) > 0 && x.RequisitionStatus.ToLower().Contains("closed") == false);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<RequisitionMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                DateTime FromRequiredDate = DateTime.Now;
                DateTime ToRequiredDate = DateTime.Now;

                if (Fields[1].Split('@')[14] != "")
                {
                    if (Fields[1].Split('@')[14] == "1")//  > 8 weeks 
                    {
                        FromRequiredDate = DateTime.Now.AddDays(56);
                        ToRequiredDate = FromRequiredDate.AddDays(999);
                    }
                    else if (Fields[1].Split('@')[14] == "2")// 4-8 weeks
                    {
                        FromRequiredDate = DateTime.Now.AddDays(28);
                        ToRequiredDate = FromRequiredDate.AddDays(56);
                    }
                    else if (Fields[1].Split('@')[14] == "3")// < 4 weeks
                    {
                        FromRequiredDate = DateTime.Now.AddDays(-28);
                        ToRequiredDate = FromRequiredDate.AddDays(7);
                    }
                    else if (Fields[1].Split('@')[14] == "4")// < 2 weeks
                    {
                        FromRequiredDate = DateTime.Now.AddDays(-14);
                        ToRequiredDate = FromRequiredDate.AddDays(7);
                    }
                    else if (Fields[1].Split('@')[14] == "5")// < 1 weeks
                    {
                        FromRequiredDate = DateTime.Now.AddDays(-7);
                        ToRequiredDate = FromRequiredDate.AddDays(7);
                    }
                    else if (Fields[1].Split('@')[14] == "6")// This weeks
                    {
                        ToRequiredDate = FromRequiredDate.AddDays(7);
                    }
                    else if (Fields[1].Split('@')[14] == "8")// past due
                    {
                        FromRequiredDate = DateTime.Now.AddDays(-999);
                    }
                    else if (Fields[1].Split('@')[14] == "7")// today
                    {
                        FromRequiredDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0);
                        ToRequiredDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, 999);
                    }
                }



                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && ((Fields[1].Split('@')[25] == "") || (Fields[1].Split('@')[25].Split(',').ToList().Contains(t.RequisitionStatus)))
                    && ((Fields[1].Split('@')[12] == "") || (Fields[1].Split('@')[12].Split(',').ToList().Contains(t.CustomerID.ToString())))
                    && ((Fields[1].Split('@')[47] == "") || (Fields[1].Split('@')[47].Split(',').ToList().Contains(t.WorkorderGUID.ToString())))
                    && ((Fields[1].Split('@')[14] == "") ||
                         (t.RequiredDate >= Convert.ToDateTime(FromRequiredDate) &&
                         t.RequiredDate <= Convert.ToDateTime(ToRequiredDate))
                          )
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                string[] stringSeparators = new string[] { "[####]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                if (Fields != null && Fields.Length > 1 && !string.IsNullOrEmpty(Convert.ToString(Fields[1])) && Fields[1].ToString() != "All")
                {
                    ObjCache = ObjCache.Where(t => t.RequisitionStatus == Fields[1].ToString());
                }
                if (Fields[0].ToString() != "")
                {
                    TotalCount = ObjCache.Where(t =>
                            t.ID.ToString().Contains(Fields[0].ToString()) ||
                            t.RequisitionNumber.ToString().ToLower().Contains(Fields[0].ToString().ToLower()) ||
                            (t.Description ?? "").ToString().ToLower().Contains(Fields[0].ToString().ToLower()) ||
                            (t.RequisitionType ?? "").ToString().ToLower().Contains(Fields[0].ToString().ToLower()) ||
                        (t.AppendedBarcodeString ?? "").ToString().ToLower().Contains(Fields[0].ToString().ToLower())
                        ).Count();
                    return ObjCache = ObjCache.Where(t =>
                            t.ID.ToString().Contains(Fields[0].ToString()) ||
                            t.RequisitionNumber.ToString().ToLower().Contains(Fields[0].ToString().ToLower()) ||
                            (t.Description ?? "").ToString().ToLower().Contains(Fields[0].ToString().ToLower()) ||
                            (t.RequisitionType ?? "").ToString().ToLower().Contains(Fields[0].ToString().ToLower()) ||
                            (t.AppendedBarcodeString ?? "").ToString().ToLower().Contains(Fields[0].ToString().ToLower())
                         ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
                }
                else
                {
                    TotalCount = ObjCache.Count();
                    return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
                }
            }
        }

        public IEnumerable<RequisitionMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }
        public IEnumerable<RequisitionMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media

            IEnumerable<RequisitionMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                #region "Both False"
                //Get Cached-Media
                ObjCache = null;// CacheHelper<IEnumerable<RequisitionMasterDTO>>.GetCacheItem("Cached_RequisitionMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    try
                    {
                        //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                        //IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            IEnumerable<RequisitionMasterDTO> obj = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(@"
                                                            SELECT A.*, B.UserName AS 'CreatedByName', WO.WOName as 'WorkorderName',
                                                            CM.Customer, C.UserName AS UpdatedByName, D.RoomName,
                                                            SM.SupplierName,ISNULL(MSM.StagingName,'') AS StagingName
                                                            FROM RequisitionMaster A 
                                                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                            LEFT OUTER JOIN WorkOrder as WO on A.WorkOrderGUID = WO.GUID
							                                left outer join CustomerMaster CM on A.CustomerGUID = CM.GUID
                                                            left outer join SupplierMaster SM on A.SupplierId = SM.ID
                                                            LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                                            WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString() + " and A.Room=" + RoomID.ToString())
                                                                     select new RequisitionMasterDTO
                                                                     {
                                                                         ID = u.ID,
                                                                         RequisitionNumber = u.RequisitionNumber,
                                                                         Description = u.Description,
                                                                         WorkorderGUID = u.WorkorderGUID,
                                                                         WorkorderName = u.WorkorderName,
                                                                         RequiredDate = u.RequiredDate,
                                                                         NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                                                         TotalCost = u.TotalCost,
                                                                         TotalSellPrice = u.TotalSellPrice,
                                                                         CustomerID = u.CustomerID,
                                                                         CustomerGUID = u.CustomerGUID,
                                                                         Customer = u.Customer,
                                                                         ProjectSpendGUID = u.ProjectSpendGUID,
                                                                         RequisitionStatus = u.RequisitionStatus,
                                                                         RequisitionType = u.RequisitionType,
                                                                         BillingAccountID = u.BillingAccountID,
                                                                         UDF1 = u.UDF1,
                                                                         UDF2 = u.UDF2,
                                                                         UDF3 = u.UDF3,
                                                                         UDF4 = u.UDF4,
                                                                         UDF5 = u.UDF5,
                                                                         GUID = u.GUID,
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
                                                                         ReceivedOn = u.ReceivedOn,
                                                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                                                         AddedFrom = u.AddedFrom,
                                                                         EditedFrom = u.EditedFrom,
                                                                         AppendedBarcodeString = string.Empty,//objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Requisitions"),
                                                                         //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                                                         SupplierId = u.SupplierId,
                                                                         SupplierAccountGuid = u.SupplierAccountGuid,
                                                                         SupplierName = u.SupplierName,
                                                                         ReleaseNumber = u.ReleaseNumber,
                                                                         StagingID = u.StagingID,
                                                                         StagingName = u.StagingName,
                                                                         MaterialStagingGUID = u.MaterialStagingGUID
                                                                     }).AsParallel().ToList();
                            ObjCache = CacheHelper<IEnumerable<RequisitionMasterDTO>>.AddCacheItem("Cached_RequisitionMaster_" + CompanyID.ToString(), obj);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                #endregion
            }
            else
            {
                //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                //IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                #region "Conditional"
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(@"
                                SELECT A.*, 
                                B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,CM.Customer, D.RoomName ,
                                SM.SupplierName,ISNULL(MSM.StagingName,'') AS StagingName
                                FROM RequisitionMaster A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID
                                LEFT OUTER JOIN SupplierMaster SM on A.SupplierId = SM.ID
                                LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                WHERE A.CompanyID = " + CompanyID.ToString() + " and A.Room=" + RoomID.ToString() + " AND " + sSQL)
                                select new RequisitionMasterDTO
                                {
                                    ID = u.ID,
                                    RequisitionNumber = u.RequisitionNumber,
                                    Description = u.Description,
                                    WorkorderName = u.WorkorderGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty ? new WorkOrderDAL(base.DataBaseName).GetWorkOrderByGUIDPlain(u.WorkorderGUID ?? Guid.Empty).WOName : "",
                                    RequiredDate = u.RequiredDate,
                                    NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                    TotalCost = u.TotalCost,
                                    TotalSellPrice = u.TotalSellPrice,
                                    CustomerID = u.CustomerID,
                                    CustomerGUID = u.CustomerGUID,
                                    Customer = u.Customer,
                                    ProjectSpendGUID = u.ProjectSpendGUID,
                                    RequisitionStatus = u.RequisitionStatus,
                                    RequisitionType = u.RequisitionType,
                                    BillingAccountID = u.BillingAccountID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    GUID = u.GUID,
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
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    AppendedBarcodeString = string.Empty,// objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Requisitions"),
                                    //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                    SupplierId = u.SupplierId,
                                    SupplierAccountGuid = u.SupplierAccountGuid,
                                    SupplierName = u.SupplierName,
                                    StagingID = u.StagingID,
                                    StagingName = u.StagingName,
                                    MaterialStagingGUID = u.MaterialStagingGUID
                                }).AsParallel().ToList();
                }
                #endregion
            }
            return ObjCache;
        }

        public IEnumerable<RequisitionMasterDTO> GetPendingRequisitionsOld(Int64 RoomID, Int64 CompanyId)
        {
            List<RequisitionMasterDTO> ObjReqData = GetCachedData(RoomID, CompanyId, false, false).ToList();
            RequisitionDetailsDAL objReqDtlDAL = new RequisitionDetailsDAL(base.DataBaseName);
            IEnumerable<RequisitionDetailsDTO> objReqDtlDTO = objReqDtlDAL.GetCachedData(RoomID, CompanyId);

            List<RequisitionMasterDTO> ObjFinalReqData = (from reqM in ObjReqData
                                                          join reqD in objReqDtlDTO
                                                          on reqM.GUID equals reqD.RequisitionGUID
                                                          where reqD.QuantityApproved.GetValueOrDefault(0) >= reqD.QuantityPulled.GetValueOrDefault(0)
                                                          && reqD.RequiredDate.GetValueOrDefault(DateTime.Now) > DateTime.Now
                                                          select reqM
                                                          ).ToList();

            return ObjFinalReqData;
        }

        public RequisitionMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //return DB_GetRecord(CompanyID, RoomID, id);

            RequisitionMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            string Qry = @"SELECT A.*,CM.Customer,B.UserName AS 'CreatedByName',C.UserName AS 'UpdatedByName',D.RoomName,WO.WoName as 'WorkorderName'
                                                                                        ,ISNULL(MSM.StagingName,'') AS StagingName
                                                                                        FROM RequisitionMaster A 
                                                                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                        LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID
                                                                                        LEFT OUTER JOIN WorkOrder WO on A.WorkorderGUID = WO.GUID
                                                                                        LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                                                                        LEFT OUTER JOIN ProjectMaster PM on A.ProjectSpendGUID = PM.GUID WHERE A.[ID] = '" + id + "' and A.ROOM=" + RoomID + " AND A.CompanyID=" + CompanyID + "";


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objOrder = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(Qry)
                            select new RequisitionMasterDTO
                            {
                                ID = u.ID,
                                RequisitionNumber = u.RequisitionNumber,
                                Description = u.Description,
                                WorkorderGUID = u.WorkorderGUID,
                                WorkorderName = u.WorkorderName,
                                RequiredDate = u.RequiredDate,
                                NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                TotalCost = u.TotalCost,
                                TotalSellPrice = u.TotalSellPrice,
                                CustomerID = u.CustomerID,
                                CustomerGUID = u.CustomerGUID,
                                Customer = u.Customer,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                RequisitionStatus = u.RequisitionStatus,
                                RequisitionType = u.RequisitionType,
                                BillingAccountID = u.BillingAccountID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                GUID = u.GUID,
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
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                AppendedBarcodeString = string.Empty,
                                SupplierId = u.SupplierId,
                                SupplierAccountGuid = u.SupplierAccountGuid,
                                ReleaseNumber = u.ReleaseNumber,
                                StagingID = u.StagingID,
                                StagingName = u.StagingName,
                                MaterialStagingGUID = u.MaterialStagingGUID
                            }).FirstOrDefault();

            }

            return objOrder;

        }
        public RequisitionMasterDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //return DB_GetRecord(CompanyID, RoomID, id);

            RequisitionMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            string Qry = @"SELECT A.*,CM.Customer,B.UserName AS 'CreatedByName',C.UserName AS 'UpdatedByName',D.RoomName,WO.WoName as 'WorkorderName',WO.TechnicianID
                                                                                        ,ISNULL(MSM.StagingName,'') AS StagingName	
                                                                                        FROM RequisitionMaster A 
                                                                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                        LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID
                                                                                        LEFT OUTER JOIN WorkOrder WO on A.WorkorderGUID = WO.GUID
                                                                                        LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                                                                        LEFT OUTER JOIN ProjectMaster PM on A.ProjectSpendGUID = PM.GUID WHERE A.[GUID] = '" + GUID + "' and A.ROOM=" + RoomID + " AND A.CompanyID=" + CompanyID + "";


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objOrder = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(Qry)
                            select new RequisitionMasterDTO
                            {
                                ID = u.ID,
                                RequisitionNumber = u.RequisitionNumber,
                                Description = u.Description,
                                WorkorderGUID = u.WorkorderGUID,
                                WorkorderName = u.WorkorderName,
                                RequiredDate = u.RequiredDate,
                                NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                TotalCost = u.TotalCost,
                                TotalSellPrice = u.TotalSellPrice,
                                CustomerID = u.CustomerID,
                                CustomerGUID = u.CustomerGUID,
                                Customer = u.Customer,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                RequisitionStatus = u.RequisitionStatus,
                                RequisitionType = u.RequisitionType,
                                BillingAccountID = u.BillingAccountID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                GUID = u.GUID,
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
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                AppendedBarcodeString = string.Empty,
                                SupplierId = u.SupplierId,
                                SupplierAccountGuid = u.SupplierAccountGuid,
                                TechnicianID = u.TechnicianID,
                                ReleaseNumber = u.ReleaseNumber,
                                StagingID = u.StagingID,
                                StagingName = u.StagingName,
                                MaterialStagingGUID = u.MaterialStagingGUID
                            }).FirstOrDefault();

            }

            return objOrder;

        }
        public RequisitionMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            //return DB_GetRecord(CompanyID, RoomID, id);

            RequisitionMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            string Qry = @"SELECT A.*,CM.Customer,B.UserName AS 'CreatedByName',C.UserName AS 'UpdatedByName',D.RoomName,WO.WoName as 'WorkorderName'
                                                                                        ,ISNULL(MSM.StagingName,'') AS StagingName	
                                                                                        FROM RequisitionMaster A 
                                                                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                        LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID
                                                                                        LEFT OUTER JOIN WorkOrder WO on A.WorkorderGUID = WO.GUID
                                                                                        LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                                                                        LEFT OUTER JOIN ProjectMaster PM on A.ProjectSpendGUID = PM.GUID WHERE A.[ID] = '" + id.ToString() + "' AND A.Room=" + RoomID + " AND A.CompanyID=" + CompanyID + "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objOrder = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(Qry)
                            select new RequisitionMasterDTO
                            {
                                ID = u.ID,
                                RequisitionNumber = u.RequisitionNumber,
                                Description = u.Description,
                                WorkorderGUID = u.WorkorderGUID,
                                WorkorderName = u.WorkorderName,
                                RequiredDate = u.RequiredDate,
                                NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                TotalCost = u.TotalCost,
                                TotalSellPrice = u.TotalSellPrice,
                                CustomerID = u.CustomerID,
                                CustomerGUID = u.CustomerGUID,
                                Customer = u.Customer,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                RequisitionStatus = u.RequisitionStatus,
                                RequisitionType = u.RequisitionType,
                                BillingAccountID = u.BillingAccountID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                GUID = u.GUID,
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
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                AppendedBarcodeString = string.Empty,
                                SupplierId = u.SupplierId,
                                SupplierAccountGuid = u.SupplierAccountGuid,
                                ReleaseNumber = u.ReleaseNumber,
                                StagingID = u.StagingID,
                                StagingName = u.StagingName,
                                MaterialStagingGUID = u.MaterialStagingGUID
                            }).FirstOrDefault();

            }

            return objOrder;

        }
        public RequisitionMasterDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            //return DB_GetRecord(CompanyID, RoomID, id);

            RequisitionMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            string Qry = @"SELECT A.*,CM.Customer,B.UserName AS 'CreatedByName',C.UserName AS 'UpdatedByName',D.RoomName,WO.WoName as 'WorkorderName'
                                                                                        ,ISNULL(MSM.StagingName,'') AS StagingName
                                                                                        FROM RequisitionMaster A 
                                                                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                        LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID
                                                                                        LEFT OUTER JOIN WorkOrder WO on A.WorkorderGUID = WO.GUID
                                                                                        LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                                                                        LEFT OUTER JOIN ProjectMaster PM on A.ProjectSpendGUID = PM.GUID WHERE A.[GUID] = '" + GUID.ToString() + "' AND A.Room=" + RoomID + " AND A.CompanyID=" + CompanyID + "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objOrder = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(Qry)
                            select new RequisitionMasterDTO
                            {
                                ID = u.ID,
                                RequisitionNumber = u.RequisitionNumber,
                                Description = u.Description,
                                WorkorderGUID = u.WorkorderGUID,
                                WorkorderName = u.WorkorderName,
                                RequiredDate = u.RequiredDate,
                                NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                TotalCost = u.TotalCost,
                                TotalSellPrice = u.TotalSellPrice,
                                CustomerID = u.CustomerID,
                                CustomerGUID = u.CustomerGUID,
                                Customer = u.Customer,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                RequisitionStatus = u.RequisitionStatus,
                                RequisitionType = u.RequisitionType,
                                BillingAccountID = u.BillingAccountID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                GUID = u.GUID,
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
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                AppendedBarcodeString = string.Empty,
                                SupplierId = u.SupplierId,
                                SupplierAccountGuid = u.SupplierAccountGuid,
                                ReleaseNumber = u.ReleaseNumber,
                                StagingID = u.StagingID,
                                StagingName = u.StagingName,
                                MaterialStagingGUID = u.MaterialStagingGUID
                            }).FirstOrDefault();

            }

            return objOrder;

        }
        public RequisitionMasterDTO GetRecord(Int64? id, Guid? ReqGUID)
        {
            //return DB_GetRecord(CompanyID, RoomID, id);

            RequisitionMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            string Qry = @"SELECT A.*,CM.Customer,B.UserName AS 'CreatedByName',C.UserName AS 'UpdatedByName',D.RoomName,WO.WoName as 'WorkorderName'
                                                                                        ,ISNULL(MSM.StagingName,'') AS StagingName
                                                                                        FROM RequisitionMaster A 
                                                                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                        LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID
                                                                                        LEFT OUTER JOIN WorkOrder WO on A.WorkorderGUID = WO.GUID
                                                                                        LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                                                                        LEFT OUTER JOIN ProjectMaster PM on A.ProjectSpendGUID = PM.GUID WHERE A.ID = " + (id ?? 0).ToString() + " OR A.[GUID] = '" + (ReqGUID ?? Guid.Empty).ToString() + "'";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objOrder = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(Qry)
                            select new RequisitionMasterDTO
                            {
                                ID = u.ID,
                                RequisitionNumber = u.RequisitionNumber,
                                Description = u.Description,
                                WorkorderGUID = u.WorkorderGUID,
                                WorkorderName = u.WorkorderName,
                                RequiredDate = u.RequiredDate,
                                NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                TotalCost = u.TotalCost,
                                TotalSellPrice = u.TotalSellPrice,
                                CustomerID = u.CustomerID,
                                CustomerGUID = u.CustomerGUID,
                                Customer = u.Customer,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                RequisitionStatus = u.RequisitionStatus,
                                RequisitionType = u.RequisitionType,
                                BillingAccountID = u.BillingAccountID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                GUID = u.GUID,
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
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                //RequisitionNumberForSorting = CommonDAL.GetSortingString(u.RequisitionNumber),
                                AppendedBarcodeString = string.Empty,
                                SupplierId = u.SupplierId,
                                SupplierAccountGuid = u.SupplierAccountGuid,
                                ReleaseNumber = u.ReleaseNumber,
                                StagingID = u.StagingID,
                                StagingName = u.StagingName,
                                MaterialStagingGUID = u.MaterialStagingGUID
                            }).FirstOrDefault();

            }

            return objOrder;

        }
        public RequisitionMasterDTO GetRecordByName(string ReqName, long RoomID, long CompanyID)
        {
            //return DB_GetRecord(CompanyID, RoomID, id);

            RequisitionMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            string Qry = @"SELECT A.*,CM.Customer,B.UserName AS 'CreatedByName',C.UserName AS 'UpdatedByName',D.RoomName,WO.WoName as 'WorkorderName'
                                                                                        ,ISNULL(MSM.StagingName,'') AS StagingName
                                                                                        FROM RequisitionMaster A 
                                                                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                        LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID = CM.GUID
                                                                                        LEFT OUTER JOIN WorkOrder WO on A.WorkorderGUID = WO.GUID
                                                                                        LEFT OUTER JOIN ProjectMaster PM on A.ProjectSpendGUID = PM.GUID 
                                                                                        LEFT OUTER JOIN MaterialStaging MSM on A.MaterialStagingGUID = MSM.[GUID]
                                                                                        WHERE isnull(A.IsDeleted,0)=0 and isnull(A.IsArchived,0)=0 and A.RequisitionNumber = '" + ReqName + "' AND A.Room=" + RoomID + " AND A.CompanyID=" + CompanyID + "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objOrder = (from u in context.ExecuteStoreQuery<RequisitionMasterDTO>(Qry)
                            select new RequisitionMasterDTO
                            {
                                ID = u.ID,
                                RequisitionNumber = u.RequisitionNumber,
                                Description = u.Description,
                                WorkorderGUID = u.WorkorderGUID,
                                WorkorderName = u.WorkorderName,
                                RequiredDate = u.RequiredDate,
                                NumberofItemsrequisitioned = u.NumberofItemsrequisitioned,
                                TotalCost = u.TotalCost,
                                TotalSellPrice = u.TotalSellPrice,
                                CustomerID = u.CustomerID,
                                CustomerGUID = u.CustomerGUID,
                                Customer = u.Customer,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                RequisitionStatus = u.RequisitionStatus,
                                RequisitionType = u.RequisitionType,
                                BillingAccountID = u.BillingAccountID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                GUID = u.GUID,
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
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                SupplierId = u.SupplierId,
                                SupplierAccountGuid = u.SupplierAccountGuid,
                                ReleaseNumber = u.ReleaseNumber,
                                StagingID = u.StagingID,
                                StagingName = u.StagingName,
                                MaterialStagingGUID = u.MaterialStagingGUID
                            }).FirstOrDefault();

            }

            return objOrder;

        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID, long SessionUserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);

                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE RequisitionMaster SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                
                if (context.ExecuteStoreCommand(strQuery) > 0)
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
                                objDAL.DeleteRecordsFromMaster(string.Join(",", new List<string>(lst.Select(t => t.GUID.ToString())).ToArray()), userid, RoomID, CompanyID, SessionUserId);
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
    }
}
