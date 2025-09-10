using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static eTurns.DTO.ImportDTO;

namespace eTurnsMaster.DAL
{
    public partial class ImportDAL : eTurnsMasterBaseDAL
    {
        public bool Insert_ImportStatus(string ImportDBName, string ImportType, string ImportBulkID, Int32 TotalRecords, DateTime ImportCreatedDate, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportType", ImportType  ?? (object)DBNull.Value),
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                        new SqlParameter("@TotalRecords", TotalRecords),
                        new SqlParameter("@ImportCreatedDate", ImportCreatedDate)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_ImportStatus @ImportType, @ImportBulkID, @TotalRecords, @ImportCreatedDate", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSuccess = true;
                    }

                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Update_ImportStatus(string ImportDBName, string ImportType, string ImportBulkID, Int32? TotalRecords,
                                        bool? IsAllItemInserted, bool? IsImportStarted, bool? IsImportCompleted, DateTime? ImportComplitionDate, int? SuccessRecords,
                                        out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportType", ImportType ?? (object)DBNull.Value),
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                        new SqlParameter("@TotalRecords", (object)TotalRecords??DBNull.Value),
                        new SqlParameter("@IsAllItemInserted", (object)IsAllItemInserted??DBNull.Value),
                        new SqlParameter("@IsImportStarted", (object)IsImportStarted??DBNull.Value),
                        new SqlParameter("@IsImportCompleted", (object)IsImportCompleted??DBNull.Value),
                        new SqlParameter("@ImportComplitionDate", (object)ImportComplitionDate??DBNull.Value),
                        new SqlParameter("@SuccessRecords", (object)SuccessRecords??DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_UpdareStatus @ImportType, @ImportBulkID, @TotalRecords, @IsAllItemInserted, @IsImportStarted, @IsImportCompleted, @ImportComplitionDate, @SuccessRecords", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSuccess = true;
                    }

                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Insert_Pull_Import(string ImportDBName,
                                         string ImportBulkID, Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, Int64 UserID, Int64 ItemID, Int64 ProjectMasterID, Int64 WorkOrderID,
                                         string EnterpriseName, string CompanyName, string RoomName, string UserName, string ItemNumber, double PullQuantity, string PullBin, string ProjectSpend,
                                         string WorkOrder, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, bool IsCredit, DateTime ImportDate, string ImportedFrom, string EditedFrom, out string Message, string PullOrderNumber = "", Guid? WOGUID = null, Guid? PullGUID = null,
                                         Guid? SupplierAccountGUID = null, double? SellPrice = null,
                                         List<eTurns.DTO.ImportDTO.Import_PULLDetails_DTO> lstPullDetails = null)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                        new SqlParameter("@EnterpriseID", EnterpriseID),
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@RoomID", RoomID),
                        new SqlParameter("@UserID", UserID),
                        new SqlParameter("@ItemID", ItemID),
                        new SqlParameter("@ProjectMasterID", ProjectMasterID),
                        new SqlParameter("@WorkOrderID", WorkOrderID),
                        new SqlParameter("@EnterpriseName", EnterpriseName  ?? (object)DBNull.Value),
                        new SqlParameter("@CompanyName", CompanyName  ?? (object)DBNull.Value),
                        new SqlParameter("@RoomName", RoomName  ?? (object)DBNull.Value),
                        new SqlParameter("@UserName", UserName  ?? (object)DBNull.Value),
                        new SqlParameter("@ItemNumber", ItemNumber  ?? (object)DBNull.Value),
                        new SqlParameter("@PullQuantity", PullQuantity),
                        new SqlParameter("@PullBin", PullBin  ?? (object)DBNull.Value),
                        new SqlParameter("@ProjectSpend", ProjectSpend ?? (object)DBNull.Value),
                        new SqlParameter("@WorkOrder", WorkOrder ?? (object)DBNull.Value),
                        new SqlParameter("@UDF1", UDF1 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF2", UDF2 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF3", UDF3 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF4", UDF4 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF5", UDF5 ?? (object)DBNull.Value),
                        new SqlParameter("@IsCredit", (IsCredit == true ? "1" : "0")),
                        new SqlParameter("@ImportDate", ImportDate),
                        new SqlParameter("@ImportedFrom", ImportedFrom  ?? (object)DBNull.Value),
                        new SqlParameter("@EditedFrom", EditedFrom  ?? (object)DBNull.Value),
                        new SqlParameter("@PullOrderNumber", PullOrderNumber ?? (object)DBNull.Value),
                        new SqlParameter("@WOGUID", WOGUID ?? (object)DBNull.Value),
                        new SqlParameter("@PullGUID", PullGUID ?? (object)DBNull.Value),
                        new SqlParameter("@SupplierAccountGUID", SupplierAccountGUID ?? (object)DBNull.Value),
                        new SqlParameter("@SellPrice", SellPrice ?? (object)DBNull.Value)
                    };

                    decimal InsertedImportPullID = context.Database.SqlQuery<decimal>(@"EXEC [" + ImportDBName + "].DBO.CSP_Insert_Pull_Import @ImportBulkID, @EnterpriseID, @CompanyID, @RoomID, @UserID, @ItemID, @ProjectMasterID, @WorkOrderID, " +
                                                                                                                       "@EnterpriseName, @CompanyName, @RoomName, @UserName, @ItemNumber, @PullQuantity, @PullBin, @ProjectSpend, " +
                                                                                                                       "@WorkOrder, @UDF1, @UDF2, @UDF3, @UDF4, @UDF5, @IsCredit, @ImportDate, @ImportedFrom, @EditedFrom,@PullOrderNumber,@WOGUID,@PullGUID,@SupplierAccountGUID,@SellPrice", ArrParams).FirstOrDefault();


                    if (InsertedImportPullID > 0)
                    {
                        if (lstPullDetails != null && lstPullDetails.Count > 0)
                        {
                            #region for add Serial number into Insert Pull service

                            foreach (ImportDTO.Import_PULLDetails_DTO objPullDetails in lstPullDetails)
                            {
                                var ArrDetailsParams = new SqlParameter[]
                                {
                                    new SqlParameter("@ImportPullID", InsertedImportPullID),
                                    new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                    new SqlParameter("@EnterpriseID", EnterpriseID),
                                    new SqlParameter("@CompanyID", CompanyID),
                                    new SqlParameter("@RoomID", RoomID),
                                    new SqlParameter("@UserID", UserID),
                                    new SqlParameter("@IsCredit", (IsCredit == true ? "1" : "0")),
                                    new SqlParameter("@EditedFrom", objPullDetails.EditedFrom  ?? (object)DBNull.Value),
                                    new SqlParameter("@PullGUID", PullGUID ?? (object)DBNull.Value),
                                    new SqlParameter("@PullDetailGUID",Guid.NewGuid()),
                                    new SqlParameter("@SerialNumber",objPullDetails.SerialNumber ?? (object)DBNull.Value),
                                    new SqlParameter("@LotNumber",objPullDetails.LotNumber ?? (object)DBNull.Value),
                                    new SqlParameter("@DateCode",objPullDetails.DateCode ?? (object)DBNull.Value),
                                    new SqlParameter("@ExpirationDate",objPullDetails.ExpirationDate ?? (object)DBNull.Value),
                                    new SqlParameter("@Quantity",objPullDetails.Quantity)
                                };

                                Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Insert_PullDetails_Import @ImportPullID, @ImportBulkID, @EnterpriseID, @CompanyID, @RoomID, @UserID, @IsCredit, @EditedFrom, @PullGUID, @PullDetailGUID, @SerialNumber, @LotNumber, @DateCode, @ExpirationDate, @Quantity", ArrDetailsParams).FirstOrDefault();
                            }

                            #endregion
                        }
                        else
                            IsSuccess = true;
                    }
                    else
                    {
                        IsSuccess = false;
                    }

                    if (lstPullDetails != null && lstPullDetails.Count > 0)
                    {
                        if (Message == "SUCCESS")
                        {
                            IsSuccess = true;
                        }
                        else
                        {
                            IsSuccess = false;
                        }
                    }

                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public List<ImportDTO.ImportStatus_DTO> GetTaskToExecute(string ImportDBName, string ImportType)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportType", ImportType  ?? (object)DBNull.Value)
                    };

                    List<ImportDTO.ImportStatus_DTO> lstImportStatus = (from A in context.Database.SqlQuery<ImportDTO.ImportStatus_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetTaskToExecute @ImportType", ArrParams)
                                                                        select new ImportDTO.ImportStatus_DTO()
                                                                        {
                                                                            ID = A.ID,
                                                                            ImportType = A.ImportType,
                                                                            ImportBulkID = A.ImportBulkID,
                                                                            TotalRecords = A.TotalRecords,
                                                                            ImportCreatedDate = A.ImportCreatedDate,
                                                                            IsAllItemInserted = A.IsAllItemInserted,
                                                                            IsImportStarted = A.IsImportStarted,
                                                                            IsImportCompleted = A.IsImportCompleted,
                                                                            ImportComplitionDate = A.ImportComplitionDate,
                                                                            SuccessRecords = A.SuccessRecords
                                                                        }
                                                                       ).ToList();

                    return lstImportStatus;
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }

        public List<ImportDTO.ImportStatus_DTO> CSP_GetTaskToExecute_test(string ImportDBName, string ImportType)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportType", ImportType  ?? (object)DBNull.Value)
                    };

                    List<ImportDTO.ImportStatus_DTO> lstImportStatus = (from A in context.Database.SqlQuery<ImportDTO.ImportStatus_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetTaskToExecute_test @ImportType", ArrParams)
                                                                        select new ImportDTO.ImportStatus_DTO()
                                                                        {
                                                                            ID = A.ID,
                                                                            ImportType = A.ImportType,
                                                                            ImportBulkID = A.ImportBulkID,
                                                                            TotalRecords = A.TotalRecords,
                                                                            ImportCreatedDate = A.ImportCreatedDate,
                                                                            IsAllItemInserted = A.IsAllItemInserted,
                                                                            IsImportStarted = A.IsImportStarted,
                                                                            IsImportCompleted = A.IsImportCompleted,
                                                                            ImportComplitionDate = A.ImportComplitionDate,
                                                                            SuccessRecords = A.SuccessRecords
                                                                        }
                                                                       ).ToList();

                    return lstImportStatus;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool CheckPullGUIDExistInImportDB(long RoomID, long CompanyID, Guid GUID, string ImportDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID) ,
                                               new SqlParameter("@CompanyId", CompanyID) ,
                                               new SqlParameter("@GUID", GUID) ,
                                               new SqlParameter("@ImportDB", ImportDBName)
                                            };

            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                ImportDTO.Import_PULL_DTO obj = context.Database.SqlQuery<ImportDTO.Import_PULL_DTO>("exec [CheckPullGUIDExistInImportDB] @RoomId,@CompanyId,@GUID,@ImportDB", params1).ToList().FirstOrDefault();

                if (obj != null && obj.PullGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    return true;
                }
                return false;
            }
        }

        public List<ImportDTO.Import_PULL_DTO> GetImportPullItems(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                     {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                     };

                    List<ImportDTO.Import_PULL_DTO> lstImportPullItems = (from A in context.Database.SqlQuery<ImportDTO.Import_PULL_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportPullItems @ImportBulkID", ArrParams)
                                                                          select new ImportDTO.Import_PULL_DTO()
                                                                          {
                                                                              ID = A.ID,
                                                                              ImportBulkID = A.ImportBulkID,
                                                                              EnterpriseID = A.EnterpriseID,
                                                                              CompanyID = A.CompanyID,
                                                                              RoomID = A.RoomID,
                                                                              UserID = A.UserID,
                                                                              ItemID = A.ItemID,
                                                                              ProjectMasterID = A.ProjectMasterID,
                                                                              WorkOrderID = A.WorkOrderID,
                                                                              EnterpriseName = A.EnterpriseName,
                                                                              CompanyName = A.CompanyName,
                                                                              RoomName = A.RoomName,
                                                                              UserName = A.UserName,
                                                                              ItemNumber = A.ItemNumber,
                                                                              PullQuantity = A.PullQuantity,
                                                                              PullBin = A.PullBin,
                                                                              ProjectSpend = A.ProjectSpend,
                                                                              WorkOrder = A.WorkOrder,
                                                                              UDF1 = A.UDF1,
                                                                              UDF2 = A.UDF2,
                                                                              UDF3 = A.UDF3,
                                                                              UDF4 = A.UDF4,
                                                                              UDF5 = A.UDF5,
                                                                              IsCredit = A.IsCredit,
                                                                              ImportDate = A.ImportDate,
                                                                              ImportedFrom = A.ImportedFrom,
                                                                              IsProcessed = A.IsProcessed,
                                                                              ProcessDate = A.ProcessDate,
                                                                              IsSuccess = A.IsSuccess,
                                                                              ErrorMessage = A.ErrorMessage,
                                                                              StatusMessage = A.StatusMessage,
                                                                              EditedFrom = A.EditedFrom ?? "Web",
                                                                              PullOrderNumber = A.PullOrderNumber,
                                                                              WOGUID = A.WOGUID,
                                                                              PullGUID = A.PullGUID,
                                                                              SupplierAccountGUID = A.SupplierAccountGUID,
                                                                              SellPrice = A.SellPrice
                                                                          }
                                                                         ).ToList();

                    foreach (ImportDTO.Import_PULL_DTO objPull in lstImportPullItems)
                    {

                        var ArrDetailsParams = new SqlParameter[]
                        {
                                new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                new SqlParameter("@ImportPullID", objPull.ID)
                        };

                        List<ImportDTO.Import_PULLDetails_DTO> lstImportPullDetails = (from A in context.Database.SqlQuery<ImportDTO.Import_PULLDetails_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportPullDetails @ImportBulkID , @ImportPullID", ArrDetailsParams)
                                                                                       select new ImportDTO.Import_PULLDetails_DTO()
                                                                                       {
                                                                                           ID = A.ID,
                                                                                           ImportBulkID = A.ImportBulkID,
                                                                                           EnterpriseID = A.EnterpriseID,
                                                                                           CompanyID = A.CompanyID,
                                                                                           RoomID = A.RoomID,
                                                                                           UserID = A.UserID,
                                                                                           IsCredit = A.IsCredit,
                                                                                           ErrorMessage = A.ErrorMessage,
                                                                                           StatusMessage = A.StatusMessage,
                                                                                           EditedFrom = A.EditedFrom ?? "Web Svc",
                                                                                           PullGUID = A.PullGUID,
                                                                                           PullDetailGUID = A.PullDetailGUID,
                                                                                           SerialNumber = A.SerialNumber,
                                                                                           LotNumber = A.LotNumber,
                                                                                           ExpirationDate = A.ExpirationDate,
                                                                                           DateCode = A.DateCode,
                                                                                           Quantity = A.Quantity
                                                                                       }
                                                                             ).ToList();


                        objPull.PullDetails = new List<ImportDTO.Import_PULLDetails_DTO>();
                        objPull.PullDetails.AddRange(lstImportPullDetails);
                    }

                    return lstImportPullItems;
                }
                catch
                {
                    return null;
                }
            }
        }


        public List<ImportDTO.Import_PULL_DTO> GetSuccessPullByImportBulkID(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    List<ImportDTO.Import_PULL_DTO> lstSuccessPullItems = context.Database.SqlQuery<ImportDTO.Import_PULL_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetSuccessPullByImportBulkID @ImportBulkID", ArrParams).ToList();

                    return lstSuccessPullItems;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool Updare_Pull_Import(string ImportDBName, Int64 PullImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage, string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                     {
                        new SqlParameter("@PullImportID", PullImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                     };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Updare_Pull_Import @PullImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public List<ImportDTO.Import_PULL_DTO> GetProcessedPulls(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ArrImportBulkID", ArrImportBulkID  ?? (object)DBNull.Value)
                    };

                    List<ImportDTO.Import_PULL_DTO> lstImportPull = (from A in context.Database.SqlQuery<ImportDTO.Import_PULL_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedPulls @ArrImportBulkID", ArrParams)
                                                                     select new ImportDTO.Import_PULL_DTO()
                                                                     {
                                                                         ID = A.ID,
                                                                         ImportBulkID = A.ImportBulkID,
                                                                         EnterpriseName = A.EnterpriseName,
                                                                         CompanyName = A.CompanyName,
                                                                         RoomName = A.RoomName,
                                                                         UserName = A.UserName,
                                                                         ItemNumber = A.ItemNumber,
                                                                         PullQuantity = A.PullQuantity,
                                                                         PullBin = A.PullBin,
                                                                         ProjectSpend = A.ProjectSpend,
                                                                         WorkOrder = A.WorkOrder,
                                                                         WOGUID = A.WOGUID,
                                                                         PullGUID = A.PullGUID,
                                                                         UDF1 = A.UDF1,
                                                                         UDF2 = A.UDF2,
                                                                         UDF3 = A.UDF3,
                                                                         UDF4 = A.UDF4,
                                                                         UDF5 = A.UDF5,
                                                                         IsCredit = A.IsCredit,
                                                                         ImportDate = A.ImportDate,
                                                                         strImportDate = A.strImportDate,
                                                                         ImportedFrom = A.ImportedFrom,
                                                                         IsProcessed = A.IsProcessed,
                                                                         strProcessDate = A.strProcessDate,
                                                                         ProcessDate = A.ProcessDate,
                                                                         IsSuccess = A.IsSuccess,
                                                                         ErrorMessage = A.ErrorMessage,
                                                                         StatusMessage = A.StatusMessage
                                                                     }
                                                                       ).ToList();

                    return lstImportPull;
                }
                catch
                {
                    return null;
                }
            }
        }


        public List<ImportDTO.Import_WorkOrderHeader_DTO> GetProcessedWorkOrders(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    List<ImportDTO.Import_WorkOrderHeader_DTO> lstImportWorkOrder = null;
                    var ArrParams = new SqlParameter[]
                 {
                        new SqlParameter("@ArrImportBulkID", ArrImportBulkID  ?? (object)DBNull.Value)
                 };

                    lstImportWorkOrder = (from A in context.Database.SqlQuery<ImportDTO.Import_WorkOrderHeader_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedWorkOrders @ArrImportBulkID", ArrParams)
                                          select new ImportDTO.Import_WorkOrderHeader_DTO()
                                          {
                                              ID = A.ID,
                                              ImportBulkID = A.ImportBulkID,
                                              EnterpriseID = A.EnterpriseID,
                                              CompanyID = A.CompanyID,
                                              RoomID = A.RoomID,
                                              UserID = A.UserID,
                                              EnterpriseName = A.EnterpriseName,
                                              CompanyName = A.CompanyName,
                                              RoomName = A.RoomName,
                                              UserName = A.UserName,
                                              Asset = A.Asset,
                                              Customer = A.Customer,
                                              Description = A.Description,
                                              ImportGUID = A.ImportGUID,
                                              Odometer_OperationHours = A.Odometer_OperationHours,
                                              SignatureContent = A.SignatureContent,
                                              SupplierName = A.SupplierName,
                                              SupplierAccountNumber = A.SupplierAccountNumber,
                                              TechnicianCode = A.TechnicianCode,
                                              TechnicianName = A.TechnicianName,
                                              Tool = A.Tool,
                                              WOGUID = A.WOGUID,
                                              WorkOrderStatus = A.WorkOrderStatus,
                                              WorkOrderType = A.WorkOrderType,
                                              WorkorderName = A.WorkorderName,
                                              UDF1 = A.UDF1,
                                              UDF2 = A.UDF2,
                                              UDF3 = A.UDF3,
                                              UDF4 = A.UDF4,
                                              UDF5 = A.UDF5,
                                              ImportDate = A.ImportDate,
                                              ImportedFrom = A.ImportedFrom,
                                              IsProcessed = A.IsProcessed,
                                              ProcessDate = A.ProcessDate,
                                              IsSuccess = A.IsSuccess,
                                              strImportDate = A.strImportDate,
                                              strProcessDate = A.strProcessDate,
                                              ErrorMessage = A.ErrorMessage,
                                              StatusMessage = A.StatusMessage
                                          }).ToList();

                    return lstImportWorkOrder;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_WorkOrderImageDetail_DTO> GetProcessedWorkOrderImageDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    List<ImportDTO.Import_WorkOrderImageDetail_DTO> lstImportWorkOrderImageDetail = null;
                    var ArrParams = new SqlParameter[]
                 {
                        new SqlParameter("@ArrImportBulkID", ArrImportBulkID  ?? (object)DBNull.Value)
                 };

                    lstImportWorkOrderImageDetail = (from A in context.Database.SqlQuery<ImportDTO.Import_WorkOrderImageDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedWorkOrderImageDetails @ArrImportBulkID", ArrParams)
                                                     select new ImportDTO.Import_WorkOrderImageDetail_DTO()
                                                     {
                                                         ID = A.ID,
                                                         ImportBulkID = A.ImportBulkID,
                                                         WOImgGUID = A.WOImgGUID,
                                                         WOImageName = A.WOImageName,
                                                         WOImportHeaderID = A.WOImportHeaderID,
                                                         ImportDate = A.ImportDate,
                                                         ImportedFrom = A.ImportedFrom,
                                                         IsProcessed = A.IsProcessed,
                                                         ProcessDate = A.ProcessDate,
                                                         IsSuccess = A.IsSuccess,
                                                         strImportDate = A.strImportDate,
                                                         strProcessDate = A.strProcessDate,
                                                         ErrorMessage = A.ErrorMessage,
                                                         StatusMessage = A.StatusMessage
                                                     }).ToList();

                    return lstImportWorkOrderImageDetail;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_OrderMaster_DTO> GetProcessedOrders(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.Import_OrderMaster_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedOrders @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<ImportDTO.Import_OrderDetail_DTO> GetProcessedOrderDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.Import_OrderDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedOrderDetails @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_PreReceivOrderDetail_DTO> GetProcessedPreReceivOrderDetail(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.Import_PreReceivOrderDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedPreReceivOrderDetail @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<ImportDTO.Import_WorkOrderHeader_DTO> GetImportWorkOrderHeader(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    List<ImportDTO.Import_WorkOrderHeader_DTO> lstImportWorkOrder = null;
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    lstImportWorkOrder = (from A in context.Database.SqlQuery<ImportDTO.Import_WorkOrderHeader_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportWorkOrderHeader @ImportBulkID", ArrParams)
                                          select new ImportDTO.Import_WorkOrderHeader_DTO()
                                          {
                                              ID = A.ID,
                                              ImportBulkID = A.ImportBulkID,
                                              EnterpriseID = A.EnterpriseID,
                                              CompanyID = A.CompanyID,
                                              RoomID = A.RoomID,
                                              UserID = A.UserID,
                                              EnterpriseName = A.EnterpriseName,
                                              CompanyName = A.CompanyName,
                                              RoomName = A.RoomName,
                                              UserName = A.UserName,
                                              Asset = A.Asset,
                                              Customer = A.Customer,
                                              Description = A.Description,
                                              ImportGUID = A.ImportGUID,
                                              Odometer_OperationHours = A.Odometer_OperationHours,
                                              SignatureContent = A.SignatureContent,
                                              SupplierName = A.SupplierName,
                                              SupplierAccountNumber = A.SupplierAccountNumber,
                                              TechnicianCode = A.TechnicianCode,
                                              TechnicianName = A.TechnicianName,
                                              Tool = A.Tool,
                                              WOGUID = A.WOGUID,
                                              WorkOrderStatus = A.WorkOrderStatus,
                                              WorkOrderType = A.WorkOrderType,
                                              WorkorderName = A.WorkorderName,
                                              UDF1 = A.UDF1,
                                              UDF2 = A.UDF2,
                                              UDF3 = A.UDF3,
                                              UDF4 = A.UDF4,
                                              UDF5 = A.UDF5,
                                              ImportDate = A.ImportDate,
                                              ImportedFrom = A.ImportedFrom,
                                              IsProcessed = A.IsProcessed,
                                              ProcessDate = A.ProcessDate,
                                              IsSuccess = A.IsSuccess,
                                              ErrorMessage = A.ErrorMessage,
                                              StatusMessage = A.StatusMessage,
                                              EditedFrom = A.EditedFrom ?? "Web",
                                              CreatedFrom = A.CreatedFrom
                                          }).ToList();

                    return lstImportWorkOrder;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_WorkOrderImageDetail_DTO> GetImportWOImageDetail(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    List<ImportDTO.Import_WorkOrderImageDetail_DTO> lstImportWorkOrderImageDetail = null;
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    lstImportWorkOrderImageDetail = (from A in context.Database.SqlQuery<ImportDTO.Import_WorkOrderImageDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportWorkOrderImageDetail @ImportBulkID", ArrParams)
                                                     select new ImportDTO.Import_WorkOrderImageDetail_DTO()
                                                     {
                                                         ID = A.ID,
                                                         ImportBulkID = A.ImportBulkID,
                                                         WOImportHeaderID = A.WOImportHeaderID,
                                                         WOImageName = A.WOImageName,
                                                         ImportGUID = A.ImportGUID,
                                                         IsDeleted = A.IsDeleted,
                                                         WOImageContent = A.WOImageContent,
                                                         WOImgGUID = A.WOImgGUID,
                                                         ImportDate = A.ImportDate,
                                                         ImportedFrom = A.ImportedFrom,
                                                         IsProcessed = A.IsProcessed,
                                                         ProcessDate = A.ProcessDate,
                                                         IsSuccess = A.IsSuccess,
                                                         ErrorMessage = A.ErrorMessage,
                                                         StatusMessage = A.StatusMessage,
                                                         EditedFrom = A.EditedFrom ?? "Web"

                                                     }).ToList();

                    return lstImportWorkOrderImageDetail;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_OrderImageDetail_DTO> GetImportOImageDetail(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    List<ImportDTO.Import_OrderImageDetail_DTO> lstImporOrderImageDetail = null;
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    lstImporOrderImageDetail = (from A in context.Database.SqlQuery<ImportDTO.Import_OrderImageDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportOrderImageDetail @ImportBulkID", ArrParams)
                                                select new ImportDTO.Import_OrderImageDetail_DTO()
                                                {
                                                    ID = A.ID,
                                                    ImportBulkID = A.ImportBulkID,
                                                    OImportHeaderID = A.OImportHeaderID,
                                                    OImageName = A.OImageName,
                                                    ImportGUID = A.ImportGUID,
                                                    IsDeleted = A.IsDeleted,
                                                    OImageContent = A.OImageContent,
                                                    OImgGUID = A.OImgGUID,
                                                    ImportDate = A.ImportDate,
                                                    ImportedFrom = A.ImportedFrom,
                                                    IsProcessed = A.IsProcessed,
                                                    ProcessDate = A.ProcessDate,
                                                    IsSuccess = A.IsSuccess,
                                                    ErrorMessage = A.ErrorMessage,
                                                    StatusMessage = A.StatusMessage,
                                                    EditedFrom = A.EditedFrom ?? "Web"

                                                }).ToList();

                    return lstImporOrderImageDetail;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_ReceiveFileDetail_DTO> GetImportReceieveFileDetail(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    List<ImportDTO.Import_ReceiveFileDetail_DTO> lstImporReceiveFileDetail = null;
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    lstImporReceiveFileDetail = (from A in context.Database.SqlQuery<ImportDTO.Import_ReceiveFileDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportReceiveDetail @ImportBulkID", ArrParams)
                                                 select new ImportDTO.Import_ReceiveFileDetail_DTO()
                                                 {
                                                     ID = A.ID,
                                                     ImportBulkID = A.ImportBulkID,
                                                     ImportHeaderID = A.ImportHeaderID,
                                                     OrderDetailsGUID = A.OrderDetailsGUID,
                                                     FileName = A.FileName,
                                                     ImportGUID = A.ImportGUID,
                                                     IsDeleted = A.IsDeleted,
                                                     FileContent = A.FileContent,
                                                     GUID = A.GUID,
                                                     ImportDate = A.ImportDate,
                                                     ImportedFrom = A.ImportedFrom,
                                                     IsProcessed = A.IsProcessed,
                                                     ProcessDate = A.ProcessDate,
                                                     IsSuccess = A.IsSuccess,
                                                     ErrorMessage = A.ErrorMessage,
                                                     StatusMessage = A.StatusMessage,
                                                     EditedFrom = A.EditedFrom ?? "Web"

                                                 }).ToList();

                    return lstImporReceiveFileDetail;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public bool Insert_WorkOrderHeader_Import(string ImportDBName, string ImportBulkID, ImportDTO.Import_WorkOrderHeader_DTO objImportWOHDTO, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = true;
                Message = "";

                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                        new SqlParameter("@EnterpriseID", objImportWOHDTO.EnterpriseID  ?? (object)DBNull.Value),
                        new SqlParameter("@CompanyID", objImportWOHDTO.CompanyID  ?? (object)DBNull.Value),
                        new SqlParameter("@RoomID", objImportWOHDTO.RoomID  ?? (object)DBNull.Value),
                        new SqlParameter("@UserID", objImportWOHDTO.UserID  ?? (object)DBNull.Value),
                        new SqlParameter("@EnterpriseName", objImportWOHDTO.EnterpriseName  ?? (object)DBNull.Value),
                        new SqlParameter("@CompanyName", objImportWOHDTO.CompanyName  ?? (object)DBNull.Value),
                        new SqlParameter("@RoomName", objImportWOHDTO.RoomName  ?? (object)DBNull.Value),
                        new SqlParameter("@UserName", objImportWOHDTO.UserName  ?? (object)DBNull.Value),
                        new SqlParameter("@Description", objImportWOHDTO.Description ?? (object)DBNull.Value),
                        new SqlParameter("@Asset", objImportWOHDTO.Asset ?? (object)DBNull.Value),
                        new SqlParameter("@Customer", objImportWOHDTO.Customer ?? (object)DBNull.Value),
                        new SqlParameter("@SupplierName", objImportWOHDTO.SupplierName ?? (object)DBNull.Value),
                        new SqlParameter("@TechnicianCode", objImportWOHDTO.TechnicianCode ?? (object)DBNull.Value),
                        new SqlParameter("@TechnicianName", objImportWOHDTO.TechnicianName ?? (object)DBNull.Value),
                        new SqlParameter("@Tool", objImportWOHDTO.Tool ?? (object)DBNull.Value),
                        new SqlParameter("@Odometer_OperationHours", objImportWOHDTO.Odometer_OperationHours ?? (object)DBNull.Value),
                        new SqlParameter("@WOGUID", objImportWOHDTO.WOGUID ?? (object)DBNull.Value),
                        new SqlParameter("@WorkOrderType", objImportWOHDTO.WorkOrderType ?? (object)DBNull.Value),
                        new SqlParameter("@WorkOrderStatus", objImportWOHDTO.WorkOrderStatus ?? (object)DBNull.Value),
                        new SqlParameter("@WorkorderName", objImportWOHDTO.WorkorderName  ?? (object)DBNull.Value),
                        new SqlParameter("@UDF1", objImportWOHDTO.UDF1 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF2", objImportWOHDTO.UDF2 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF3", objImportWOHDTO.UDF3 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF4", objImportWOHDTO.UDF4 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF5", objImportWOHDTO.UDF5 ?? (object)DBNull.Value),
                        new SqlParameter("@ImportDate", objImportWOHDTO.ImportDate),
                        new SqlParameter("@ImportedFrom", objImportWOHDTO.ImportedFrom  ?? (object)DBNull.Value),
                        new SqlParameter("@EditedFrom", objImportWOHDTO.EditedFrom  ?? (object)DBNull.Value),
                        new SqlParameter("@CreatedFrom", objImportWOHDTO.CreatedFrom  ?? (object)DBNull.Value),
                        new SqlParameter("@SupplierAccountNumber", objImportWOHDTO.SupplierAccountNumber ?? (object)DBNull.Value)
                    };


                    if (objImportWOHDTO.SignatureContent != null && objImportWOHDTO.SignatureContent.Length > 0)
                    {
                        SqlParameter sp = new SqlParameter("@SignatureContent", SqlDbType.VarBinary, int.MaxValue);
                        sp.Value = objImportWOHDTO.SignatureContent;
                        parameters.Add(sp);
                    }
                    else
                    {
                        SqlParameter sp = new SqlParameter("@SignatureContent", SqlDbType.VarBinary, -1);
                        sp.Value = DBNull.Value;
                        parameters.Add(sp);
                    }

                    var ArrParams = parameters.ToArray();

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Insert_WorkOrderHeader_Import @ImportBulkID,@EnterpriseID,@CompanyID,@RoomID,@UserID,@EnterpriseName,@CompanyName,@RoomName,@UserName,@Description,@Asset,@Customer,@SupplierName, @TechnicianCode, @TechnicianName, @Tool, @Odometer_OperationHours, @WOGUID, @WorkOrderStatus, @WorkOrderType,@WorkorderName, @UDF1, @UDF2, @UDF3, @UDF4, @UDF5,@ImportDate, @ImportedFrom, @EditedFrom,@CreatedFrom,@SignatureContent,@SupplierAccountNumber ", ArrParams).FirstOrDefault();
                    long newid = 0;
                    if (long.TryParse(Message, out newid))
                    {
                        if (newid > 0)
                        {
                            #region WO Image Detail
                            if (objImportWOHDTO.WOImageDetail != null && objImportWOHDTO.WOImageDetail.Count > 0)
                            {
                                foreach (var item in objImportWOHDTO.WOImageDetail)
                                {
                                    var childparameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                        new SqlParameter("@WOImportHeaderID", newid),
                                        new SqlParameter("@WOImgGUID", item.WOImgGUID  ?? (object)DBNull.Value),
                                        new SqlParameter("@WOImageName", item.WOImageName  ?? (object)DBNull.Value),
                                        new SqlParameter("@IsDeleted", item.IsDeleted  ?? (object)DBNull.Value),
                                        new SqlParameter("@ImportDate", objImportWOHDTO.ImportDate  ?? (object)DBNull.Value),
                                        new SqlParameter("@ImportedFrom", objImportWOHDTO.ImportedFrom  ?? (object)DBNull.Value),
                                        new SqlParameter("@EditedFrom", objImportWOHDTO.EditedFrom ?? (object)DBNull.Value)

                                    };


                                    if (item.WOImageContent != null && item.WOImageContent.Length > 0)
                                    {
                                        SqlParameter sp = new SqlParameter("@WOImageContent", SqlDbType.VarBinary, int.MaxValue);
                                        sp.Value = item.WOImageContent;
                                        childparameters.Add(sp);
                                    }
                                    else
                                    {
                                        SqlParameter sp = new SqlParameter("@WOImageContent", SqlDbType.VarBinary, -1);
                                        sp.Value = DBNull.Value;
                                        childparameters.Add(sp);
                                    }

                                    var ArrParamsch = childparameters.ToArray();

                                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.[CSP_Insert_WorkOrderImageDetail_Import] @ImportBulkID,@WOImportHeaderID,@WOImgGUID,@WOImageName,@IsDeleted,@ImportDate, @ImportedFrom, @EditedFrom,@WOImageContent ", ArrParamsch).FirstOrDefault();
                                    long newchid = 0;
                                    if (long.TryParse(Message, out newchid))
                                    {
                                        if (newchid > 0)
                                        {
                                            Message += "success";
                                            IsSuccess = true;
                                        }
                                        else
                                        {
                                            IsSuccess = false;
                                        }

                                    }
                                }
                            }
                            #endregion

                            #region WO Pull Items
                            if (objImportWOHDTO.WOPullItems != null && objImportWOHDTO.WOPullItems.Count > 0)
                            {
                                foreach (var objImportPullDTO in objImportWOHDTO.WOPullItems)
                                {
                                    string message1 = string.Empty;
                                    bool isInsert = Insert_Pull_Import(ImportDBName, ImportBulkID,
                                                                           (long)objImportWOHDTO.EnterpriseID,
                                                                           (long)objImportWOHDTO.CompanyID,
                                                                           (long)objImportWOHDTO.RoomID,
                                                                           (long)objImportWOHDTO.UserID,
                                                                           (long)objImportPullDTO.ItemID,
                                                                           (long)objImportPullDTO.ProjectMasterID,
                                                                           (long)objImportPullDTO.WorkOrderID,
                                                                           objImportWOHDTO.EnterpriseName,
                                                                           objImportWOHDTO.CompanyName,
                                                                           objImportWOHDTO.RoomName,
                                                                           objImportWOHDTO.UserName,
                                                                           objImportPullDTO.ItemNumber,
                                                                           objImportPullDTO.PullQuantity,
                                                                           objImportPullDTO.PullBin,
                                                                           objImportPullDTO.ProjectSpend,
                                                                           objImportWOHDTO.WorkorderName,
                                                                           objImportPullDTO.UDF1,
                                                                           objImportPullDTO.UDF2,
                                                                           objImportPullDTO.UDF3,
                                                                           objImportPullDTO.UDF4,
                                                                           objImportPullDTO.UDF5,
                                                                           (bool)objImportPullDTO.IsCredit,
                                                                           (DateTime)objImportWOHDTO.ImportDate,
                                                                           objImportPullDTO.ImportedFrom,
                                                                           objImportPullDTO.EditedFrom, out message1, objImportPullDTO.PullOrderNumber, objImportWOHDTO.WOGUID, PullGUID: objImportPullDTO.PullGUID,
                                                                           SupplierAccountGUID: objImportPullDTO.SupplierAccountGUID,
                                                                           SellPrice: objImportPullDTO.SellPrice,
                                                                           lstPullDetails: objImportPullDTO.PullDetails);

                                    if (isInsert)
                                    {
                                        Message += message1;
                                        IsSuccess = true;
                                    }
                                    else
                                    {
                                        IsSuccess = false;
                                    }


                                }
                            }
                            #endregion
                        }
                        else
                        {
                            IsSuccess = false;
                        }
                    }

                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Insert_OrderMaster_Import(string ImportDBName, string ImportBulkID, ImportDTO.Import_OrderMaster_DTO ImportOrderMaster,
            DataTable OrderDetailsTbl, DataTable PreReceivOrderDetailTbl, string cultureCode, out string Message, DataTable OrderDetailTrackingTbl = null)
        {
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = true;
                Message = "";

                try
                {
                    string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(ImportDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                    DataSet dsOrder = new DataSet();

                    dsOrder = SqlHelper.ExecuteDataset(EturnsConnection, "CSP_Insert_OrderMaster_Import",
                        (ImportBulkID ?? (object)DBNull.Value), (ImportOrderMaster.EnterpriseID ?? (object)DBNull.Value)
                    , (ImportOrderMaster.CompanyID ?? (object)DBNull.Value), (ImportOrderMaster.RoomID ?? (object)DBNull.Value), (ImportOrderMaster.UserID ?? (object)DBNull.Value), (ImportOrderMaster.EnterpriseName ?? (object)DBNull.Value), (ImportOrderMaster.CompanyName ?? (object)DBNull.Value)
                    , (ImportOrderMaster.RoomName ?? (object)DBNull.Value), (ImportOrderMaster.UserName ?? (object)DBNull.Value), (ImportOrderMaster.OrderGUID ?? (object)DBNull.Value), (ImportOrderMaster.OrderNumber ?? (object)DBNull.Value), (ImportOrderMaster.ReleaseNumber ?? (object)DBNull.Value)
                    , (ImportOrderMaster.RequiredDate ?? (object)DBNull.Value), (ImportOrderMaster.OrderStatus ?? (object)DBNull.Value), (ImportOrderMaster.Comment ?? (object)DBNull.Value)
                    , (ImportOrderMaster.PackSlipNumber ?? (object)DBNull.Value), (ImportOrderMaster.ShippingTrackNumber ?? (object)DBNull.Value), (ImportOrderMaster.Customer ?? (object)DBNull.Value)
                    , (ImportOrderMaster.ShipViaName ?? (object)DBNull.Value), (ImportOrderMaster.ShippingVendorName ?? (object)DBNull.Value), (ImportOrderMaster.SupplierName ?? (object)DBNull.Value)
                    , (ImportOrderMaster.SupplierAccountName ?? (object)DBNull.Value), (ImportOrderMaster.SupplierAccountNumber ?? (object)DBNull.Value), (ImportOrderMaster.IsEDIOrder ?? (object)DBNull.Value), (ImportOrderMaster.IsOrderExist ?? (object)DBNull.Value)
                    , (ImportOrderMaster.VersionNumber ?? (object)DBNull.Value), (ImportOrderMaster.EditedFrom ?? (object)DBNull.Value), (ImportOrderMaster.UDF1 ?? (object)DBNull.Value)
                    , (ImportOrderMaster.UDF2 ?? (object)DBNull.Value), (ImportOrderMaster.UDF3 ?? (object)DBNull.Value), (ImportOrderMaster.UDF4 ?? (object)DBNull.Value)
                    , (ImportOrderMaster.UDF5 ?? (object)DBNull.Value), ImportOrderMaster.ImportDate, (ImportOrderMaster.ImportedFrom ?? (object)DBNull.Value), (ImportOrderMaster.CreatedFrom ?? (object)DBNull.Value), (ImportOrderMaster.SalesOrder ?? (object)DBNull.Value)
                    );

                    if (dsOrder != null && dsOrder.Tables.Count > 0)
                    {
                        DataTable dt = dsOrder.Tables[0];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var raw = dt.Rows[0];
                            long newid = 0;

                            if (dt.Columns.Contains("StatusMessage"))
                            {
                                long.TryParse(Convert.ToString(raw["StatusMessage"]), out newid);
                            }

                            if (newid > 0)
                            {
                                Message = "success";
                                IsSuccess = true;

                                if (OrderDetailsTbl != null && OrderDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblType = new SqlParameter("@OrderDetailsDataTbl", OrderDetailsTbl);
                                    SqlParameterTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblType.TypeName = "OrderDetailsDataTbl";

                                    var childparameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportOrderMaster.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportOrderMaster.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportOrderMaster.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportOrderMaster.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseName", (ImportOrderMaster.EnterpriseName ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyName", (ImportOrderMaster.CompanyName ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomName", (ImportOrderMaster.RoomName ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserName", (ImportOrderMaster.UserName ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportOrderMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportOrderMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparameters.Add(SqlParameterTblType);
                                    var ArrParamsch = childparameters.ToArray();
                                    var dsOrderDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_OrderDetails_Import", ArrParamsch);
                                    if (dsOrderDetail != null && dsOrderDetail.Tables.Count > 0)
                                    {
                                        DataTable dtOrderDetail = dsOrderDetail.Tables[0];

                                        if (dtOrderDetail != null && dtOrderDetail.Rows.Count > 0)
                                        {
                                            var tmpraw = dtOrderDetail.Rows[0];
                                            long newOrderDetailId = 0;

                                            if (dtOrderDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmpraw["StatusMessage"]), out newOrderDetailId);
                                            }

                                            if (newOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                                if (ImportOrderMaster.OrderReceiveDetails != null && ImportOrderMaster.OrderReceiveDetails.Count() > 0)
                                                {
                                                    foreach (var item in ImportOrderMaster.OrderReceiveDetails)
                                                    {
                                                        if (item.ReceiveOrderImageDetails != null && item.ReceiveOrderImageDetails.Count > 0)
                                                        {
                                                            foreach (var itemimage in item.ReceiveOrderImageDetails)
                                                            {

                                                                var receiveparams = new List<SqlParameter>
                                                            {
                                                            new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                                            new SqlParameter("@ImportHeaderID", newOrderDetailId),
                                                            new SqlParameter("@FileGUID", itemimage.GUID  ?? (object)DBNull.Value),
                                                            new SqlParameter("@FileName", itemimage.FileName  ?? (object)DBNull.Value),
                                                            new SqlParameter("@IsDeleted", itemimage.IsDeleted  ?? (object)DBNull.Value),
                                                            new SqlParameter("@ImportDate", item.ImportDate  ?? (object)DBNull.Value),
                                                            new SqlParameter("@ImportedFrom", item.ImportedFrom  ?? (object)DBNull.Value),
                                                            new SqlParameter("@EditedFrom", item.EditedFrom ?? (object)DBNull.Value),
                                                            new SqlParameter("@OrderDetailsGUID",item.OrderDetailGUID ?? (object)DBNull.Value)
                                                            };


                                                                if (itemimage.FileContent != null && itemimage.FileContent.Length > 0)
                                                                {
                                                                    SqlParameter sp = new SqlParameter("@FileContent", SqlDbType.VarBinary, int.MaxValue);
                                                                    sp.Value = itemimage.FileContent;
                                                                    receiveparams.Add(sp);
                                                                }
                                                                else
                                                                {
                                                                    SqlParameter sp = new SqlParameter("@FileContent", SqlDbType.VarBinary, -1);
                                                                    sp.Value = DBNull.Value;
                                                                    receiveparams.Add(sp);
                                                                }
                                                                var RecArrParamsch = receiveparams.ToArray();

                                                                Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.[CSP_Insert_ReceieveFileDetail_Import] @ImportBulkID,@ImportHeaderID,@FileGUID,@FileName,@IsDeleted,@ImportDate, @ImportedFrom, @EditedFrom,@FileContent,@OrderDetailsGUID ", RecArrParamsch).FirstOrDefault();
                                                                long newchid = 0;
                                                                if (long.TryParse(Message, out newchid))
                                                                {
                                                                    if (newchid > 0)
                                                                    {
                                                                        Message += "success";
                                                                        IsSuccess = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        IsSuccess = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrderDetails", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderDetail", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrderDetails", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderDetail", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }

                                if (OrderDetailTrackingTbl != null && OrderDetailTrackingTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblType = new SqlParameter("@OrderDetailTrackingDataTbl", OrderDetailTrackingTbl);
                                    SqlParameterTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblType.TypeName = "OrderDetailTrackingDataTbl";

                                    var childparameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportOrderMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportOrderMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparameters.Add(SqlParameterTblType);
                                    var ArrParamsch = childparameters.ToArray();
                                    var dsOrderDetailTracking = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_OrderDetailTracking_Import", ArrParamsch);
                                    if (dsOrderDetailTracking != null && dsOrderDetailTracking.Tables.Count > 0)
                                    {
                                        DataTable dtOrderDetailTracking = dsOrderDetailTracking.Tables[0];

                                        if (dtOrderDetailTracking != null && dtOrderDetailTracking.Rows.Count > 0)
                                        {
                                            var tmpraw = dtOrderDetailTracking.Rows[0];
                                            long newOrderDetailTrackingId = 0;

                                            if (dtOrderDetailTracking.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmpraw["StatusMessage"]), out newOrderDetailTrackingId);
                                            }

                                            if (newOrderDetailTrackingId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrderDetailTracking", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderDetailTracking", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrderDetailTracking", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderDetailTracking", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }
                               
                                if (ImportOrderMaster.OrderImageDetails != null && ImportOrderMaster.OrderImageDetails.Any())
                                {
                                    #region Order Image Detail

                                    foreach (var item in ImportOrderMaster.OrderImageDetails)
                                    {
                                        var childparameters = new List<SqlParameter>
                                            {
                                            new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                            new SqlParameter("@OImportHeaderID", newid),
                                            new SqlParameter("@OImgGUID", item.OImgGUID  ?? (object)DBNull.Value),
                                            new SqlParameter("@OImageName", item.OImageName  ?? (object)DBNull.Value),
                                            new SqlParameter("@IsDeleted", item.IsDeleted  ?? (object)DBNull.Value),
                                            new SqlParameter("@ImportDate", item.ImportDate  ?? (object)DBNull.Value),
                                            new SqlParameter("@ImportedFrom", item.ImportedFrom  ?? (object)DBNull.Value),
                                            new SqlParameter("@EditedFrom", item.EditedFrom ?? (object)DBNull.Value)
                                            };


                                        if (item.OImageContent != null && item.OImageContent.Length > 0)
                                        {
                                            SqlParameter sp = new SqlParameter("@OImageContent", SqlDbType.VarBinary, int.MaxValue);
                                            sp.Value = item.OImageContent;
                                            childparameters.Add(sp);
                                        }
                                        else
                                        {
                                            SqlParameter sp = new SqlParameter("@OImageContent", SqlDbType.VarBinary, -1);
                                            sp.Value = DBNull.Value;
                                            childparameters.Add(sp);
                                        }

                                        var ArrParamsch = childparameters.ToArray();

                                        Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.[CSP_Insert_OrderImageDetail_Import] @ImportBulkID,@OImportHeaderID,@OImgGUID,@OImageName,@IsDeleted,@ImportDate, @ImportedFrom, @EditedFrom,@OImageContent ", ArrParamsch).FirstOrDefault();
                                        long newchid = 0;
                                        if (long.TryParse(Message, out newchid))
                                        {
                                            if (newchid > 0)
                                            {
                                                Message += "success";
                                                IsSuccess = true;
                                            }
                                            else
                                            {
                                                IsSuccess = false;
                                            }
                                        }
                                    }

                                    #endregion
                                }

                                if (PreReceivOrderDetailTbl != null && PreReceivOrderDetailTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblReceiveType = new SqlParameter("@PreReceivOrderDetailTbl", PreReceivOrderDetailTbl);
                                    SqlParameterTblReceiveType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblReceiveType.TypeName = "PreReceivOrderDetailTbl";

                                    var childparametersReceive = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportOrderMaster.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportOrderMaster.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportOrderMaster.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportOrderMaster.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportOrderMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportOrderMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparametersReceive.Add(SqlParameterTblReceiveType);
                                    var ArrParamschReceive = childparametersReceive.ToArray();
                                    var dsPreReceivOrderDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_PreReceivOrderDetail_Import", ArrParamschReceive);

                                    if (dsPreReceivOrderDetail != null && dsPreReceivOrderDetail.Tables.Count > 0)
                                    {
                                        DataTable dtPreReceivOrderDetail = dsPreReceivOrderDetail.Tables[0];

                                        if (dtPreReceivOrderDetail != null && dtPreReceivOrderDetail.Rows.Count > 0)
                                        {
                                            var tmprawreceive = dtPreReceivOrderDetail.Rows[0];
                                            long newPreReceivOrderDetailId = 0;

                                            if (dtPreReceivOrderDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmprawreceive["StatusMessage"]), out newPreReceivOrderDetailId);
                                            }

                                            if (newPreReceivOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderReceiveDetail", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderReceiveDetail", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }


                            }
                            else
                            {
                                Message = "fail";
                                IsSuccess = false;
                            }
                        }
                        else
                        {
                            Message = "fail";
                            IsSuccess = false;
                        }
                    }
                    else
                    {
                        Message = "fail";
                        IsSuccess = false;
                    }
                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Insert_QuoteMaster_Import(string ImportDBName, string ImportBulkID, ImportDTO.Import_QuoteMaster_DTO ImportQuoteMaster,
           DataTable OrderDetailsTbl, DataTable PreReceivOrderDetailTbl, string cultureCode, out string Message)
        {
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = true;
                Message = "";

                try
                {
                    string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(ImportDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                    DataSet dsOrder = new DataSet();

                    dsOrder = SqlHelper.ExecuteDataset(EturnsConnection, "CSP_Insert_OrderMaster_Import",
                        (ImportBulkID ?? (object)DBNull.Value), (ImportQuoteMaster.EnterpriseID ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.CompanyID ?? (object)DBNull.Value), (ImportQuoteMaster.RoomID ?? (object)DBNull.Value), (ImportQuoteMaster.UserID ?? (object)DBNull.Value), (ImportQuoteMaster.EnterpriseName ?? (object)DBNull.Value), (ImportQuoteMaster.CompanyName ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.RoomName ?? (object)DBNull.Value), (ImportQuoteMaster.UserName ?? (object)DBNull.Value), (ImportQuoteMaster.QuoteGuid ?? (object)DBNull.Value), (ImportQuoteMaster.QuoteNumber ?? (object)DBNull.Value), (ImportQuoteMaster.ReleaseNumber ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.RequiredDate ?? (object)DBNull.Value), (ImportQuoteMaster.QuoteStatus ?? (object)DBNull.Value), (ImportQuoteMaster.Comment ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.PackSlipNumber ?? (object)DBNull.Value), (ImportQuoteMaster.ShippingTrackNumber ?? (object)DBNull.Value), (ImportQuoteMaster.Customer ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.ShipViaName ?? (object)DBNull.Value), (ImportQuoteMaster.ShippingVendorName ?? (object)DBNull.Value), (ImportQuoteMaster.QuoteSupplierIdsCSV ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.SupplierAccountNumber ?? (object)DBNull.Value), (ImportQuoteMaster.IsEDIQuote ?? (object)DBNull.Value), (ImportQuoteMaster.IsQuoteExist ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.VersionNumber ?? (object)DBNull.Value), (ImportQuoteMaster.EditedFrom ?? (object)DBNull.Value), (ImportQuoteMaster.UDF1 ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.UDF2 ?? (object)DBNull.Value), (ImportQuoteMaster.UDF3 ?? (object)DBNull.Value), (ImportQuoteMaster.UDF4 ?? (object)DBNull.Value)
                    , (ImportQuoteMaster.UDF5 ?? (object)DBNull.Value), ImportQuoteMaster.ImportDate, (ImportQuoteMaster.ImportedFrom ?? (object)DBNull.Value), (ImportQuoteMaster.CreatedFrom ?? (object)DBNull.Value), (ImportQuoteMaster.SalesOrder ?? (object)DBNull.Value)
                    );

                    if (dsOrder != null && dsOrder.Tables.Count > 0)
                    {
                        DataTable dt = dsOrder.Tables[0];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var raw = dt.Rows[0];
                            long newid = 0;

                            if (dt.Columns.Contains("StatusMessage"))
                            {
                                long.TryParse(Convert.ToString(raw["StatusMessage"]), out newid);
                            }

                            if (newid > 0)
                            {
                                Message = "success";
                                IsSuccess = true;

                                if (OrderDetailsTbl != null && OrderDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblType = new SqlParameter("@OrderDetailsDataTbl", OrderDetailsTbl);
                                    SqlParameterTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblType.TypeName = "OrderDetailsDataTbl";

                                    var childparameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportQuoteMaster.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportQuoteMaster.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportQuoteMaster.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportQuoteMaster.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseName", (ImportQuoteMaster.EnterpriseName ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyName", (ImportQuoteMaster.CompanyName ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomName", (ImportQuoteMaster.RoomName ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserName", (ImportQuoteMaster.UserName ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportQuoteMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportQuoteMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparameters.Add(SqlParameterTblType);
                                    var ArrParamsch = childparameters.ToArray();
                                    var dsOrderDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_OrderDetails_Import", ArrParamsch);
                                    if (dsOrderDetail != null && dsOrderDetail.Tables.Count > 0)
                                    {
                                        DataTable dtOrderDetail = dsOrderDetail.Tables[0];

                                        if (dtOrderDetail != null && dtOrderDetail.Rows.Count > 0)
                                        {
                                            var tmpraw = dtOrderDetail.Rows[0];
                                            long newOrderDetailId = 0;

                                            if (dtOrderDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmpraw["StatusMessage"]), out newOrderDetailId);
                                            }

                                            if (newOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                                //if (ImportQuoteMaster.OrderReceiveDetails != null && ImportQuoteMaster.OrderReceiveDetails.Count() > 0)
                                                //{
                                                //    foreach (var item in ImportQuoteMaster.QReceiveDetails)
                                                //    {
                                                //        if (item.ReceiveOrderImageDetails != null && item.ReceiveOrderImageDetails.Count > 0)
                                                //        {
                                                //            foreach (var itemimage in item.ReceiveOrderImageDetails)
                                                //            {

                                                //                var receiveparams = new List<SqlParameter>
                                                //            {
                                                //            new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                                //            new SqlParameter("@ImportHeaderID", newOrderDetailId),
                                                //            new SqlParameter("@FileGUID", itemimage.GUID  ?? (object)DBNull.Value),
                                                //            new SqlParameter("@FileName", itemimage.FileName  ?? (object)DBNull.Value),
                                                //            new SqlParameter("@IsDeleted", itemimage.IsDeleted  ?? (object)DBNull.Value),
                                                //            new SqlParameter("@ImportDate", item.ImportDate  ?? (object)DBNull.Value),
                                                //            new SqlParameter("@ImportedFrom", item.ImportedFrom  ?? (object)DBNull.Value),
                                                //            new SqlParameter("@EditedFrom", item.EditedFrom ?? (object)DBNull.Value),
                                                //            new SqlParameter("@OrderDetailsGUID",item.OrderDetailGUID ?? (object)DBNull.Value)
                                                //            };


                                                //                if (itemimage.FileContent != null && itemimage.FileContent.Length > 0)
                                                //                {
                                                //                    SqlParameter sp = new SqlParameter("@FileContent", SqlDbType.VarBinary, int.MaxValue);
                                                //                    sp.Value = itemimage.FileContent;
                                                //                    receiveparams.Add(sp);
                                                //                }
                                                //                else
                                                //                {
                                                //                    SqlParameter sp = new SqlParameter("@FileContent", SqlDbType.VarBinary, -1);
                                                //                    sp.Value = DBNull.Value;
                                                //                    receiveparams.Add(sp);
                                                //                }
                                                //                var RecArrParamsch = receiveparams.ToArray();

                                                //                Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.[CSP_Insert_ReceieveFileDetail_Import] @ImportBulkID,@ImportHeaderID,@FileGUID,@FileName,@IsDeleted,@ImportDate, @ImportedFrom, @EditedFrom,@FileContent,@OrderDetailsGUID ", RecArrParamsch).FirstOrDefault();
                                                //                long newchid = 0;
                                                //                if (long.TryParse(Message, out newchid))
                                                //                {
                                                //                    if (newchid > 0)
                                                //                    {
                                                //                        Message += "success";
                                                //                        IsSuccess = true;
                                                //                    }
                                                //                    else
                                                //                    {
                                                //                        IsSuccess = false;
                                                //                    }
                                                //                }
                                                //            }
                                                //        }
                                                //    }
                                                //}

                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrderDetails", cultureCode, ImportQuoteMaster.EnterpriseID.GetValueOrDefault(0), ImportQuoteMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderDetail", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrderDetails", cultureCode, ImportQuoteMaster.EnterpriseID.GetValueOrDefault(0), ImportQuoteMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderDetail", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }
                                //if (ImportQuoteMaster.OrderImageDetails != null && ImportQuoteMaster.OrderImageDetails.Any())
                                //{
                                //    #region Order Image Detail

                                //    foreach (var item in ImportOrderMaster.OrderImageDetails)
                                //    {
                                //        var childparameters = new List<SqlParameter>
                                //            {
                                //            new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                //            new SqlParameter("@OImportHeaderID", newid),
                                //            new SqlParameter("@OImgGUID", item.OImgGUID  ?? (object)DBNull.Value),
                                //            new SqlParameter("@OImageName", item.OImageName  ?? (object)DBNull.Value),
                                //            new SqlParameter("@IsDeleted", item.IsDeleted  ?? (object)DBNull.Value),
                                //            new SqlParameter("@ImportDate", item.ImportDate  ?? (object)DBNull.Value),
                                //            new SqlParameter("@ImportedFrom", item.ImportedFrom  ?? (object)DBNull.Value),
                                //            new SqlParameter("@EditedFrom", item.EditedFrom ?? (object)DBNull.Value)
                                //            };


                                //        if (item.OImageContent != null && item.OImageContent.Length > 0)
                                //        {
                                //            SqlParameter sp = new SqlParameter("@OImageContent", SqlDbType.VarBinary, int.MaxValue);
                                //            sp.Value = item.OImageContent;
                                //            childparameters.Add(sp);
                                //        }
                                //        else
                                //        {
                                //            SqlParameter sp = new SqlParameter("@OImageContent", SqlDbType.VarBinary, -1);
                                //            sp.Value = DBNull.Value;
                                //            childparameters.Add(sp);
                                //        }

                                //        var ArrParamsch = childparameters.ToArray();

                                //        Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.[CSP_Insert_OrderImageDetail_Import] @ImportBulkID,@OImportHeaderID,@OImgGUID,@OImageName,@IsDeleted,@ImportDate, @ImportedFrom, @EditedFrom,@OImageContent ", ArrParamsch).FirstOrDefault();
                                //        long newchid = 0;
                                //        if (long.TryParse(Message, out newchid))
                                //        {
                                //            if (newchid > 0)
                                //            {
                                //                Message += "success";
                                //                IsSuccess = true;
                                //            }
                                //            else
                                //            {
                                //                IsSuccess = false;
                                //            }
                                //        }
                                //    }

                                //    #endregion
                                //}

                                //if (PreReceivOrderDetailTbl != null && PreReceivOrderDetailTbl.Rows.Count > 0)
                                //{
                                //    var SqlParameterTblReceiveType = new SqlParameter("@PreReceivOrderDetailTbl", PreReceivOrderDetailTbl);
                                //    SqlParameterTblReceiveType.SqlDbType = SqlDbType.Structured;
                                //    SqlParameterTblReceiveType.TypeName = "PreReceivOrderDetailTbl";

                                //    var childparametersReceive = new List<SqlParameter>
                                //    {
                                //        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                //        new SqlParameter("@EnterpriseID", (ImportOrderMaster.EnterpriseID ?? (object)DBNull.Value)),
                                //        new SqlParameter("@CompanyID",  (ImportOrderMaster.CompanyID ?? (object)DBNull.Value)),
                                //        new SqlParameter("@RoomID", (ImportOrderMaster.RoomID ?? (object)DBNull.Value)),
                                //        new SqlParameter("@UserID", (ImportOrderMaster.UserID ?? (object)DBNull.Value)),
                                //        new SqlParameter("@ImportDate", ImportOrderMaster.ImportDate),
                                //        new SqlParameter("@ImportedFrom", (ImportOrderMaster.ImportedFrom ?? (object)DBNull.Value))
                                //    };
                                //    childparametersReceive.Add(SqlParameterTblReceiveType);
                                //    var ArrParamschReceive = childparametersReceive.ToArray();
                                //    var dsPreReceivOrderDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_PreReceivOrderDetail_Import", ArrParamschReceive);

                                //    if (dsPreReceivOrderDetail != null && dsPreReceivOrderDetail.Tables.Count > 0)
                                //    {
                                //        DataTable dtPreReceivOrderDetail = dsPreReceivOrderDetail.Tables[0];

                                //        if (dtPreReceivOrderDetail != null && dtPreReceivOrderDetail.Rows.Count > 0)
                                //        {
                                //            var tmprawreceive = dtPreReceivOrderDetail.Rows[0];
                                //            long newPreReceivOrderDetailId = 0;

                                //            if (dtPreReceivOrderDetail.Columns.Contains("StatusMessage"))
                                //            {
                                //                long.TryParse(Convert.ToString(tmprawreceive["StatusMessage"]), out newPreReceivOrderDetailId);
                                //            }

                                //            if (newPreReceivOrderDetailId > 0)
                                //            {
                                //                Message = "success";
                                //                IsSuccess = true;
                                //            }
                                //        }
                                //        else
                                //        {
                                //            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                //            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderReceiveDetail", ResourceFilePath);
                                //            IsSuccess = false;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResReceiveOrderDetails", cultureCode, ImportOrderMaster.EnterpriseID.GetValueOrDefault(0), ImportOrderMaster.CompanyID.GetValueOrDefault(0));
                                //        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailInsertUpdateOrderReceiveDetail", ResourceFilePath);
                                //        IsSuccess = false;
                                //    }
                                //}


                            }
                            else
                            {
                                Message = "fail";
                                IsSuccess = false;
                            }
                        }
                        else
                        {
                            Message = "fail";
                            IsSuccess = false;
                        }
                    }
                    else
                    {
                        Message = "fail";
                        IsSuccess = false;
                    }
                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// This method is used to get the count of unprocessed order by order number(to find the release number of order)
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="RoomId"></param>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public int GetCountOfUnporcessedOrderByOrderNumber(string ImportDBName, long EnterpriseId, long CompanyId, long RoomId, string OrderNumber)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@EnterpriseId", EnterpriseId),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@OrderNumber", OrderNumber),
                                                   new SqlParameter("@SupplierName", DBNull.Value),
                                                 };
                return context.Database.SqlQuery<int>(@"EXEC [" + ImportDBName + "].DBO.[GetCountOfUnporcessedOrderByOrderNumber] @EnterpriseId,@RoomID,@CompanyID,@OrderNumber,@SupplierName", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// This method is used to get the count of unprocessed workorder by workorder name(to find the release number of workorder)
        /// </summary>
        /// <param name="ImportDBName"></param>
        /// <param name="EnterpriseId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="RoomId"></param>
        /// <param name="WorkorderName"></param>
        /// <returns></returns>
        public int GetCountOfUnporcessedWorkorderByWOName(string ImportDBName, long EnterpriseId, long CompanyId, long RoomId, string WorkorderName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@EnterpriseId", EnterpriseId),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@WorkorderName", WorkorderName)
                                                 };
                return context.Database.SqlQuery<int>(@"EXEC [" + ImportDBName + "].DBO.[GetCountOfUnporcessedWorkorderByWOName] @EnterpriseId,@RoomID,@CompanyID,@WorkorderName", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// This method is used to get the count of unprocessed requisition by requisition number(to find the release number of requisition)
        /// </summary>
        /// <param name="ImportDBName"></param>
        /// <param name="EnterpriseId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="RoomId"></param>
        /// <param name="RequisitionNumber"></param>
        /// <returns></returns>
        public int GetCountOfUnporcessedRequisitionByReqNo(string ImportDBName, long EnterpriseId, long CompanyId, long RoomId, string RequisitionNumber)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@EnterpriseId", EnterpriseId),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@RequisitionNumber", RequisitionNumber)
                                                 };
                return context.Database.SqlQuery<int>(@"EXEC [" + ImportDBName + "].DBO.[GetCountOfUnporcessedRequisitionByReqNo] @EnterpriseId,@RoomID,@CompanyID,@RequisitionNumber", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// This method is used to get the count of unprocessed order by order number and supplier(to find the release number of order)
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="RoomId"></param>
        /// <param name="OrderNumber"></param>
        /// <param name="SupplierName"></param>
        /// <returns></returns>
        public int GetCountOfUnporcessedOrderByOrderNumberAndSupplier(string ImportDBName, long EnterpriseId, long CompanyId, long RoomId, string OrderNumber, string SupplierName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@EnterpriseId", EnterpriseId),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@OrderNumber", OrderNumber),
                                                   new SqlParameter("@SupplierName", SupplierName),
                                                 };
                return context.Database.SqlQuery<int>(@"EXEC [" + ImportDBName + "].DBO.[GetCountOfUnporcessedOrderByOrderNumber] @EnterpriseId,@RoomID,@CompanyID,@OrderNumber,@SupplierName", params1).FirstOrDefault();
            }
        }

        public bool Insert_ItemMaster_Import(string ImportDBName, string ImportBulkID, ImportDTO.EDIItemImportDTO ImportItemMaster,
            DataTable ItemSupplierDetailsTbl, DataTable ItemManufacturerDetailsTbl, DataTable InventoryLocationDetailsTbl,
            DataTable KitDetailsTbl, string cultureCode, out string Message)
        {
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = true;
                Message = "";

                try
                {
                    string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(ImportDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                    DataSet dsOrder = new DataSet();

                    dsOrder = SqlHelper.ExecuteDataset(EturnsConnection, "CSP_Insert_ItemMaster_Import", (ImportBulkID ?? (object)DBNull.Value), (ImportItemMaster.EnterpriseID ?? (object)DBNull.Value)
                    , (ImportItemMaster.CompanyID ?? (object)DBNull.Value), (ImportItemMaster.RoomID ?? (object)DBNull.Value), (ImportItemMaster.UserID ?? (object)DBNull.Value), (ImportItemMaster.EnterpriseName ?? (object)DBNull.Value), (ImportItemMaster.CompanyName ?? (object)DBNull.Value)
                    , (ImportItemMaster.RoomName ?? (object)DBNull.Value), (ImportItemMaster.UserName ?? (object)DBNull.Value), (ImportItemMaster.Guid ?? (object)DBNull.Value), (ImportItemMaster.ItemNumber ?? (object)DBNull.Value), (ImportItemMaster.DefaultReorderQuantity ?? (object)DBNull.Value)
                    , (ImportItemMaster.DefaultPullQuantity ?? (object)DBNull.Value), (ImportItemMaster.UOM ?? (object)DBNull.Value), (ImportItemMaster.CostUOM ?? (object)DBNull.Value)
                    , (ImportItemMaster.InventoryClassification ?? (object)DBNull.Value), (ImportItemMaster.Category ?? (object)DBNull.Value), (ImportItemMaster.Cost ?? (object)DBNull.Value)
                    , (ImportItemMaster.Markup ?? (object)DBNull.Value), (ImportItemMaster.SellPrice ?? (object)DBNull.Value), (ImportItemMaster.CriticalQuantity ?? (object)DBNull.Value)
                    , (ImportItemMaster.MinimumQuantity ?? (object)DBNull.Value), (ImportItemMaster.MaximumQuantity ?? (object)DBNull.Value), (ImportItemMaster.IsTransfer ?? false)
                    , (ImportItemMaster.IsPurchase ?? false), (ImportItemMaster.Consignment ?? false), (ImportItemMaster.UPC ?? (object)DBNull.Value)
                    , (ImportItemMaster.UNSPSC ?? (object)DBNull.Value), (ImportItemMaster.Description ?? (object)DBNull.Value), (ImportItemMaster.LongDescription ?? (object)DBNull.Value), (ImportItemMaster.IsActive ?? false)
                    , (ImportItemMaster.EditedFrom ?? (object)DBNull.Value), (ImportItemMaster.ImageContent ?? (object)DBNull.Value), (ImportItemMaster.ImageName ?? (object)DBNull.Value), (ImportItemMaster.ImageType ?? (object)DBNull.Value)
                    , (ImportItemMaster.ItemImageExternalURL ?? (object)DBNull.Value), (ImportItemMaster.ItemDocExternalURL ?? (object)DBNull.Value), (ImportItemMaster.GLAccount ?? (object)DBNull.Value), (ImportItemMaster.TrendingSetting ?? (object)DBNull.Value)
                    , (ImportItemMaster.Taxable ?? false), (ImportItemMaster.IsAutoInventoryClassification ?? false), (ImportItemMaster.IsEnforceDefaultReorderQuantity ?? false)
                    , (ImportItemMaster.PullQtyScanOverride ?? false), (ImportItemMaster.IsPackslipMandatoryAtReceive ?? false), (ImportItemMaster.ItemLink2ImageContent)
                    , (ImportItemMaster.ItemLink2ImageName ?? (object)DBNull.Value), (ImportItemMaster.ItemLink2ImageType ?? (object)DBNull.Value)
                    , (ImportItemMaster.ItemLink2ImageExternalURL ?? (object)DBNull.Value), (ImportItemMaster.WeightPerPiece ?? (object)DBNull.Value)
                    , (ImportItemMaster.UDF1 ?? (object)DBNull.Value), (ImportItemMaster.UDF2 ?? (object)DBNull.Value), (ImportItemMaster.UDF3 ?? (object)DBNull.Value), (ImportItemMaster.UDF4 ?? (object)DBNull.Value)
                    , (ImportItemMaster.UDF5 ?? (object)DBNull.Value), (ImportItemMaster.UDF6 ?? (object)DBNull.Value), (ImportItemMaster.UDF7 ?? (object)DBNull.Value), (ImportItemMaster.UDF8 ?? (object)DBNull.Value), (ImportItemMaster.UDF9 ?? (object)DBNull.Value), (ImportItemMaster.UDF10 ?? (object)DBNull.Value)
                    , (ImportItemMaster.LeadTimeInDays ?? (object)DBNull.Value), ImportItemMaster.ImportDate, (ImportItemMaster.ImportedFrom ?? (object)DBNull.Value), (ImportItemMaster.EnhancedDescription ?? (object)DBNull.Value), (ImportItemMaster.POItemLineNumber ?? (object)DBNull.Value)
                    );

                    if (dsOrder != null && dsOrder.Tables.Count > 0)
                    {
                        DataTable dt = dsOrder.Tables[0];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var raw = dt.Rows[0];
                            long newid = 0;

                            if (dt.Columns.Contains("StatusMessage"))
                            {
                                long.TryParse(Convert.ToString(raw["StatusMessage"]), out newid);
                            }

                            if (newid > 0)
                            {
                                Message = "success";
                                IsSuccess = true;

                                if (ItemSupplierDetailsTbl != null && ItemSupplierDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblType = new SqlParameter("@ItemSupplierDetailsTbl", ItemSupplierDetailsTbl);
                                    SqlParameterTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblType.TypeName = "ItemSupplierDetailsTbl";

                                    var childparameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportItemMaster.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportItemMaster.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportItemMaster.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportItemMaster.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ItemGuid", (ImportItemMaster.Guid ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportItemMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportItemMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparameters.Add(SqlParameterTblType);
                                    var ArrParamsch = childparameters.ToArray();

                                    var dsOrderDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_ItemSupplierDetails_Import", ArrParamsch);

                                    if (dsOrderDetail != null && dsOrderDetail.Tables.Count > 0)
                                    {
                                        DataTable dtOrderDetail = dsOrderDetail.Tables[0];

                                        if (dtOrderDetail != null && dtOrderDetail.Rows.Count > 0)
                                        {
                                            var tmpraw = dtOrderDetail.Rows[0];
                                            long newOrderDetailId = 0;

                                            if (dtOrderDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmpraw["StatusMessage"]), out newOrderDetailId);
                                            }

                                            if (newOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailSupplierDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailSupplierDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }

                                if (ItemManufacturerDetailsTbl != null && ItemManufacturerDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblReceiveType = new SqlParameter("@ItemManufacturerDetailsTbl", ItemManufacturerDetailsTbl);
                                    SqlParameterTblReceiveType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblReceiveType.TypeName = "ItemManufacturerDetailsTbl";

                                    var childparametersReceive = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportItemMaster.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportItemMaster.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportItemMaster.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportItemMaster.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ItemGuid", (ImportItemMaster.Guid ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportItemMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportItemMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparametersReceive.Add(SqlParameterTblReceiveType);
                                    var ArrParamschReceive = childparametersReceive.ToArray();
                                    var dsPreReceivOrderDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_ItemManufacturerDetails_Import", ArrParamschReceive);

                                    if (dsPreReceivOrderDetail != null && dsPreReceivOrderDetail.Tables.Count > 0)
                                    {
                                        DataTable dtPreReceivOrderDetail = dsPreReceivOrderDetail.Tables[0];

                                        if (dtPreReceivOrderDetail != null && dtPreReceivOrderDetail.Rows.Count > 0)
                                        {
                                            var tmprawreceive = dtPreReceivOrderDetail.Rows[0];
                                            long newPreReceivOrderDetailId = 0;

                                            if (dtPreReceivOrderDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmprawreceive["StatusMessage"]), out newPreReceivOrderDetailId);
                                            }

                                            if (newPreReceivOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailManufacturerDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailManufacturerDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }

                                if (InventoryLocationDetailsTbl != null && InventoryLocationDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblbin = new SqlParameter("@InventoryLocationDetailsTbl", InventoryLocationDetailsTbl);
                                    SqlParameterTblbin.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblbin.TypeName = "InventoryLocationDetailsTbl";

                                    var childparametersbin = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportItemMaster.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportItemMaster.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportItemMaster.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportItemMaster.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ItemGuid", (ImportItemMaster.Guid ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportItemMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportItemMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparametersbin.Add(SqlParameterTblbin);
                                    var ArrParamschBin = childparametersbin.ToArray();
                                    var dsbin = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_InventoryLocationDetails_Import", ArrParamschBin);

                                    if (dsbin != null && dsbin.Tables.Count > 0)
                                    {
                                        DataTable dtBin = dsbin.Tables[0];

                                        if (dtBin != null && dtBin.Rows.Count > 0)
                                        {
                                            var tmprawreceive = dtBin.Rows[0];
                                            long newPreReceivOrderDetailId = 0;

                                            if (dtBin.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmprawreceive["StatusMessage"]), out newPreReceivOrderDetailId);
                                            }

                                            if (newPreReceivOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailLocationDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailLocationDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }

                                if (KitDetailsTbl != null && KitDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblKit = new SqlParameter("@KitDetailsTbl", KitDetailsTbl);
                                    SqlParameterTblKit.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblKit.TypeName = "KitDetailsTbl";

                                    var childparametersKit = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportItemMaster.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportItemMaster.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportItemMaster.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportItemMaster.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ItemGuid", (ImportItemMaster.Guid ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportItemMaster.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportItemMaster.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparametersKit.Add(SqlParameterTblKit);
                                    var ArrParamschKit = childparametersKit.ToArray();
                                    var dsKit = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_KitDetails_Import", ArrParamschKit);

                                    if (dsKit != null && dsKit.Tables.Count > 0)
                                    {
                                        DataTable dtKit = dsKit.Tables[0];

                                        if (dtKit != null && dtKit.Rows.Count > 0)
                                        {
                                            var tmprawreceive = dtKit.Rows[0];
                                            long newPreReceivOrderDetailId = 0;

                                            if (dtKit.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmprawreceive["StatusMessage"]), out newPreReceivOrderDetailId);
                                            }

                                            if (newPreReceivOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailKitDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, ImportItemMaster.EnterpriseID.GetValueOrDefault(0), ImportItemMaster.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailKitDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }
                            }
                            else
                            {
                                Message = "fail";
                                IsSuccess = false;
                            }
                        }
                        else
                        {
                            Message = "fail";
                            IsSuccess = false;
                        }
                    }
                    else
                    {
                        Message = "fail";
                        IsSuccess = false;
                    }
                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Updare_WorkOrder_Import(string ImportDBName, Int64 WOImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage, string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@WOImportID", WOImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Updare_WorkOrder_Import @WOImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Updare_WorkOrderImageDetail_Import(string ImportDBName, Int64 WOImportID, Int64 WOImgDtlImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage, string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@WOImportID", WOImportID),
                        new SqlParameter("@WOImgDtlImportID", WOImgDtlImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Updare_WorkOrderImageDetail_Import  @WOImportID, @WOImgDtlImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Update_ReceieveImageDetail_Import(string ImportDBName, Int64? OImportID, Int64 OImgDtlImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage, string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", OImportID),
                        new SqlParameter("@ImgDtlImportID", OImgDtlImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_ReceieveFileDetail_Import  @ImportID, @ImgDtlImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Update_OrderImageDetail_Import(string ImportDBName, Int64 OImportID, Int64 OImgDtlImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage, string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@OImportID", OImportID),
                        new SqlParameter("@OImgDtlImportID", OImgDtlImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_OrderImageDetail_Import  @OImportID, @OImgDtlImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool Update_RequisitionFileDetail_Import(string ImportDBName, Int64 OImportID, Int64 OImgDtlImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage, string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", OImportID),
                        new SqlParameter("@ImgDtlImportID", OImgDtlImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_RequisitionFilesDetail_Import  @ImportID, @ImgDtlImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }
        public string GetFTPMasterEmail(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, string FileName)
        {
            string strEmail = "";
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@FileName", FileName)
                                                 };

                strEmail = context.Database.SqlQuery<string>(@"EXEC GetFTPMasterEmail @EnterpriseID,@CompanyID,@RoomID,@FileName", params1).FirstOrDefault();
            }
            return strEmail;
        }

        public List<ImportDTO.Import_OrderMaster_DTO> GetImportOrderMaster(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.Import_OrderMaster_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportOrderMaster @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_OrderDetail_DTO> GetImportOrderDetails(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.Import_OrderDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportOrderDetails @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<ImportDTO.Import_OrderDetail_Tracking_DTO> GetImportOrderDetailTracking(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.Import_OrderDetail_Tracking_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportOrderDetailTracking @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.Import_PreReceivOrderDetail_DTO> GetImportPreReceivOrderDetail(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.Import_PreReceivOrderDetail_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportPreReceivOrderDetail @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        #region InsertNewReceiveOrder

        public bool Insert_RequisitionMaster_Import(string ImportDBName, string ImportBulkID, ImportDTO.RequisitionImportDTO ImportRequisition,
            DataTable RequisitionDetailsTbl, DataTable RequisitionPullDetailsTbl, DataTable RequisitionToolDetailsTbl, string cultureCode, out string Message)
        {
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = true;
                Message = "";

                try
                {
                    string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(ImportDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                    DataSet dsRequisition = new DataSet();

                    dsRequisition = SqlHelper.ExecuteDataset(EturnsConnection, "CSP_Insert_RequisitionMaster_Import", (ImportBulkID ?? (object)DBNull.Value), (ImportRequisition.EnterpriseID ?? (object)DBNull.Value)
                    , (ImportRequisition.CompanyID ?? (object)DBNull.Value), (ImportRequisition.RoomID ?? (object)DBNull.Value), (ImportRequisition.UserID ?? (object)DBNull.Value), (ImportRequisition.EnterpriseName ?? (object)DBNull.Value), (ImportRequisition.CompanyName ?? (object)DBNull.Value)
                    , (ImportRequisition.RoomName ?? (object)DBNull.Value), (ImportRequisition.UserName ?? (object)DBNull.Value), (ImportRequisition.ReqGUID ?? (object)DBNull.Value), (ImportRequisition.RequisitionNumber ?? (object)DBNull.Value), (ImportRequisition.WorkorderName ?? (object)DBNull.Value)
                    , (ImportRequisition.RequiredDate ?? (object)DBNull.Value), (ImportRequisition.RequisitionStatus ?? (object)DBNull.Value), (ImportRequisition.Customer ?? (object)DBNull.Value)
                    , (ImportRequisition.ProjectSpendName ?? (object)DBNull.Value), (ImportRequisition.RequisitionType ?? (object)DBNull.Value), (ImportRequisition.Description ?? (object)DBNull.Value)
                    , (ImportRequisition.BillingAccount ?? (object)DBNull.Value), (ImportRequisition.SupplierName ?? (object)DBNull.Value), (ImportRequisition.EditedFrom ?? (object)DBNull.Value), (ImportRequisition.UDF1 ?? (object)DBNull.Value)
                    , (ImportRequisition.UDF2 ?? (object)DBNull.Value), (ImportRequisition.UDF3 ?? (object)DBNull.Value), (ImportRequisition.UDF4 ?? (object)DBNull.Value)
                    , (ImportRequisition.UDF5 ?? (object)DBNull.Value), ImportRequisition.ImportDate, (ImportRequisition.ImportedFrom ?? (object)DBNull.Value), (ImportRequisition.CreatedFrom ?? (object)DBNull.Value)
                    );

                    if (dsRequisition != null && dsRequisition.Tables.Count > 0)
                    {
                        DataTable dt = dsRequisition.Tables[0];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var raw = dt.Rows[0];
                            long newid = 0;

                            if (dt.Columns.Contains("StatusMessage"))
                            {
                                long.TryParse(Convert.ToString(raw["StatusMessage"]), out newid);
                            }

                            if (newid > 0)
                            {
                                Message = "success";
                                IsSuccess = true;

                                if (RequisitionDetailsTbl != null && RequisitionDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterTblType = new SqlParameter("@RequisitionDetailsTbl", RequisitionDetailsTbl);
                                    SqlParameterTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterTblType.TypeName = "RequisitionDetailsTbl";

                                    var childparameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportRequisition.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportRequisition.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportRequisition.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportRequisition.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportRequisition.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportRequisition.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparameters.Add(SqlParameterTblType);
                                    var ArrParamsch = childparameters.ToArray();

                                    var dsRequisitionDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_RequisitionDetails_Import", ArrParamsch);

                                    if (dsRequisitionDetail != null && dsRequisitionDetail.Tables.Count > 0)
                                    {
                                        DataTable dtRequisitionDetail = dsRequisitionDetail.Tables[0];

                                        if (dtRequisitionDetail != null && dtRequisitionDetail.Rows.Count > 0)
                                        {
                                            var tmpraw = dtRequisitionDetail.Rows[0];
                                            long newOrderDetailId = 0;

                                            if (dtRequisitionDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmpraw["StatusMessage"]), out newOrderDetailId);
                                            }

                                            if (newOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisition.EnterpriseID.GetValueOrDefault(0), ImportRequisition.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisition.EnterpriseID.GetValueOrDefault(0), ImportRequisition.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }

                                if (RequisitionToolDetailsTbl != null && RequisitionToolDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParameterReqToolDtlTblType = new SqlParameter("@RequisitionToolDetailsTbl", RequisitionToolDetailsTbl);
                                    SqlParameterReqToolDtlTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParameterReqToolDtlTblType.TypeName = "RequisitionToolDetailsTbl";

                                    var childparametersReceive = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportRequisition.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportRequisition.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportRequisition.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportRequisition.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportRequisition.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportRequisition.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparametersReceive.Add(SqlParameterReqToolDtlTblType);
                                    var ArrParamschReceive = childparametersReceive.ToArray();
                                    var dsRequisitionToolDetails = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_RequisitionToolDetails_Import", ArrParamschReceive);

                                    if (dsRequisitionToolDetails != null && dsRequisitionToolDetails.Tables.Count > 0)
                                    {
                                        DataTable dtRequisitionToolDetails = dsRequisitionToolDetails.Tables[0];

                                        if (dtRequisitionToolDetails != null && dtRequisitionToolDetails.Rows.Count > 0)
                                        {
                                            var tmprawreceive = dtRequisitionToolDetails.Rows[0];
                                            long newPreReceivOrderDetailId = 0;

                                            if (dtRequisitionToolDetails.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmprawreceive["StatusMessage"]), out newPreReceivOrderDetailId);
                                            }

                                            if (newPreReceivOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisition.EnterpriseID.GetValueOrDefault(0), ImportRequisition.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionToolDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisition.EnterpriseID.GetValueOrDefault(0), ImportRequisition.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionToolDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }

                                if (RequisitionPullDetailsTbl != null && RequisitionPullDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParamPullDetailsTblType = new SqlParameter("@RequisitionPullDetailsTbl", RequisitionPullDetailsTbl);
                                    SqlParamPullDetailsTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParamPullDetailsTblType.TypeName = "RequisitionPullDetailsTbl";

                                    var childparametersPullDetail = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportRequisition.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportRequisition.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportRequisition.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportRequisition.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportRequisition.ImportDate),
                                        new SqlParameter("@ImportedFrom", (ImportRequisition.ImportedFrom ?? (object)DBNull.Value))
                                    };
                                    childparametersPullDetail.Add(SqlParamPullDetailsTblType);
                                    var pullDetailsParam = childparametersPullDetail.ToArray();

                                    var dsReqPullDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_RequisitionPullDetails_Import", pullDetailsParam);

                                    if (dsReqPullDetail != null && dsReqPullDetail.Tables.Count > 0)
                                    {
                                        DataTable dtPullDetail = dsReqPullDetail.Tables[0];

                                        if (dtPullDetail != null && dtPullDetail.Rows.Count > 0)
                                        {
                                            var tmpraw = dtPullDetail.Rows[0];
                                            long newOrderDetailId = 0;

                                            if (dtPullDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmpraw["StatusMessage"]), out newOrderDetailId);
                                            }

                                            if (newOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisition.EnterpriseID.GetValueOrDefault(0), ImportRequisition.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionPullDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisition.EnterpriseID.GetValueOrDefault(0), ImportRequisition.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionPullDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }
                                //Save Requisition attachment 
                                if (ImportRequisition != null && ImportRequisition.RequisitionFiles.Count > 0)
                                {
                                    #region Requisition Image Detail
                                    foreach (var item in ImportRequisition.RequisitionFiles)
                                    {
                                        var childparameters = new List<SqlParameter>
                                            {
                                            new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                                            new SqlParameter("@ImportHeaderID", newid),
                                            new SqlParameter("@ImgGUID", item.OImgGUID  ?? (object)DBNull.Value),
                                            new SqlParameter("@ImageName", item.OImageName  ?? (object)DBNull.Value),
                                            new SqlParameter("@IsDeleted", item.IsDeleted  ?? (object)DBNull.Value),
                                            new SqlParameter("@ImportDate", item.ImportDate  ?? (object)DBNull.Value),
                                            new SqlParameter("@ImportedFrom", item.ImportedFrom  ?? (object)DBNull.Value),
                                            new SqlParameter("@EditedFrom", item.EditedFrom ?? (object)DBNull.Value)
                                            };


                                        if (item.OImageContent != null && item.OImageContent.Length > 0)
                                        {
                                            SqlParameter sp = new SqlParameter("@ImageContent", SqlDbType.VarBinary, int.MaxValue);
                                            sp.Value = item.OImageContent;
                                            childparameters.Add(sp);
                                        }
                                        else
                                        {
                                            SqlParameter sp = new SqlParameter("@ImageContent", SqlDbType.VarBinary, -1);
                                            sp.Value = DBNull.Value;
                                            childparameters.Add(sp);
                                        }

                                        var ArrParamsch = childparameters.ToArray();

                                        Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.[CSP_Insert_RequisitionFilesDetail_Import] @ImportBulkID,@ImportHeaderID,@ImgGUID,@ImageName,@IsDeleted,@ImportDate, @ImportedFrom, @EditedFrom,@ImageContent ", ArrParamsch).FirstOrDefault();
                                        long newchid = 0;
                                        if (long.TryParse(Message, out newchid))
                                        {
                                            if (newchid > 0)
                                            {
                                                Message += "success";
                                                IsSuccess = true;
                                            }
                                            else
                                            {
                                                IsSuccess = false;
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                Message = "fail";
                                IsSuccess = false;
                            }
                        }
                        else
                        {
                            Message = "fail";
                            IsSuccess = false;
                        }
                    }
                    else
                    {
                        Message = "fail";
                        IsSuccess = false;
                    }
                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }


        public bool Insert_RequisitionandPullDetails_Import(string ImportDBName, string ImportBulkID,
                            ImportDTO.RequisitionDetailsImportDTO ImportRequisitionDetails, DataTable RequisitionPullDetailsTbl, string cultureCode, out string Message)
        {
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSuccess = true;
                Message = "";

                try
                {
                    string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(ImportDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                    DataSet dsRequisitionDetails = new DataSet();

                    dsRequisitionDetails = SqlHelper.ExecuteDataset(EturnsConnection, "CSP_Insert_PullRequisitionDetails_Import",
                      (ImportBulkID ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.EnterpriseID ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.CompanyID ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.RoomID ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.UserID ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.RequisitionNumber ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.RequisitionGuid),
                      (ImportRequisitionDetails.RequisitionDetailGuid ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.ItemGUID ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.ItemNumber ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.BinNumber ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.QuantityRequisitioned ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.QuantityApproved ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.QuantityToPull ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.ProjectSpend ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.PullGUID ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.SupplierAccountNumber ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.EditedFrom ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.PullUDF1 ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.PullUDF2 ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.PullUDF3 ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.PullUDF4 ?? (object)DBNull.Value),
                      (ImportRequisitionDetails.PullUDF5 ?? (object)DBNull.Value),
                       ImportRequisitionDetails.ImportDate,
                      (ImportRequisitionDetails.ImportedFrom ?? (object)DBNull.Value)
                    );

                    if (dsRequisitionDetails != null && dsRequisitionDetails.Tables.Count > 0)
                    {
                        DataTable dt = dsRequisitionDetails.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var raw = dt.Rows[0];
                            long newid = 0;

                            if (dt.Columns.Contains("StatusMessage"))
                            {
                                long.TryParse(Convert.ToString(raw["StatusMessage"]), out newid);
                            }

                            if (newid > 0)
                            {
                                Message = "success";
                                IsSuccess = true;
                                if (RequisitionPullDetailsTbl != null && RequisitionPullDetailsTbl.Rows.Count > 0)
                                {
                                    var SqlParamPullDetailsTblType = new SqlParameter("@RequisitionPullDetailsTbl", RequisitionPullDetailsTbl);
                                    SqlParamPullDetailsTblType.SqlDbType = SqlDbType.Structured;
                                    SqlParamPullDetailsTblType.TypeName = "RequisitionPullDetailsTbl";

                                    var childparametersPullDetail = new List<SqlParameter>
                                    {
                                        new SqlParameter("@ImportBulkID", (ImportBulkID ?? (object)DBNull.Value)),
                                        new SqlParameter("@EnterpriseID", (ImportRequisitionDetails.EnterpriseID ?? (object)DBNull.Value)),
                                        new SqlParameter("@CompanyID",  (ImportRequisitionDetails.CompanyID ?? (object)DBNull.Value)),
                                        new SqlParameter("@RoomID", (ImportRequisitionDetails.RoomID ?? (object)DBNull.Value)),
                                        new SqlParameter("@UserID", (ImportRequisitionDetails.UserID ?? (object)DBNull.Value)),
                                        new SqlParameter("@ImportDate", ImportRequisitionDetails.ImportDate),
                                        new SqlParameter("@RequisitionDetailsID", newid),
                                        new SqlParameter("@ImportedFrom", (ImportRequisitionDetails.ImportedFrom ?? (object)DBNull.Value))

                                    };
                                    childparametersPullDetail.Add(SqlParamPullDetailsTblType);
                                    var pullDetailsParam = childparametersPullDetail.ToArray();

                                    var dsReqPullDetail = SqlHelper.ExecuteDataset(Connectionstring, CommandType.StoredProcedure, "CSP_Insert_RequisitionPullDetails_Import", pullDetailsParam);

                                    if (dsReqPullDetail != null && dsReqPullDetail.Tables.Count > 0)
                                    {
                                        DataTable dtPullDetail = dsReqPullDetail.Tables[0];

                                        if (dtPullDetail != null && dtPullDetail.Rows.Count > 0)
                                        {
                                            var tmpraw = dtPullDetail.Rows[0];
                                            long newOrderDetailId = 0;

                                            if (dtPullDetail.Columns.Contains("StatusMessage"))
                                            {
                                                long.TryParse(Convert.ToString(tmpraw["StatusMessage"]), out newOrderDetailId);
                                            }

                                            if (newOrderDetailId > 0)
                                            {
                                                Message = "success";
                                                IsSuccess = true;
                                            }
                                        }
                                        else
                                        {
                                            string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisitionDetails.EnterpriseID.GetValueOrDefault(0), ImportRequisitionDetails.CompanyID.GetValueOrDefault(0));
                                            Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionPullDetails", ResourceFilePath);
                                            IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        string ResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionDetails", cultureCode, ImportRequisitionDetails.EnterpriseID.GetValueOrDefault(0), ImportRequisitionDetails.CompanyID.GetValueOrDefault(0));
                                        Message = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailRequisitionPullDetails", ResourceFilePath);
                                        IsSuccess = false;
                                    }
                                }
                            }
                            else
                            {
                                Message = "fail";
                                IsSuccess = false;
                            }
                        }
                        else
                        {
                            Message = "fail";
                            IsSuccess = false;
                        }
                    }
                    else
                    {
                        Message = "fail";
                        IsSuccess = false;
                    }
                    return IsSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public List<NewReceiveOrderImportDTO> GetImportReceiveOrderMaster(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<NewReceiveOrderImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportOrderMaster @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<NewReceiveOrderItemImportDTO> GetImportReceiveOrderItem(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<NewReceiveOrderItemImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportOrderDetails @ImportBulkID", ArrParams).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<ReceivedOrderTransferDetailImportDTO> GetImportReceivedOrderTransferDetail(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ReceivedOrderTransferDetailImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportPreReceivOrderDetail @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        #region InsertRequisition

        public List<ImportDTO.RequisitionImportDTO> GetImportRequisitionMaster(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.RequisitionImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportRequisitionMaster @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.RequisitionDetailsImportDTO> GetImportRequisitionDetails(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.RequisitionDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportRequisitionDetails @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.RequisitionDetailsForToolImportDTO> GetImportRequisitionToolDetails(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.RequisitionDetailsForToolImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportRequisitionToolDetails @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<ImportDTO.RequisitionPULLDetailsImportDTO> GetImportRequisitionPullDetails(string ImportDBName, string ImportBulkID, long? RequisitionDetailsID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value),
                        new SqlParameter("@RequisitionDetailsID", RequisitionDetailsID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.RequisitionPULLDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportRequisitionPullDetails @ImportBulkID,@RequisitionDetailsID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool UpdateRequisitionImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_RequisitionMaster_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool UpdateRequisitionDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_RequisitionDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }
        public bool UpdateRequisitionPullDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_RequisitionPullDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }
        public bool UpdateRequisitionToolDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_RequisitionToolDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public List<ImportDTO.RequisitionImportDTO> GetProcessedRequisitions(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.RequisitionImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedRequisitions @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.RequisitionDetailsImportDTO> GetProcessedRequisitionDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.RequisitionDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedRequisitionDetails @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<ImportDTO.RequisitionPULLDetailsImportDTO> GetProcessedRequisitionPullDetails(string ImportDBName, long RequisitionDetailsID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@RequisitionDetailsID", RequisitionDetailsID) };
                    return context.Database.SqlQuery<ImportDTO.RequisitionPULLDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedRequisitionPullDetails @RequisitionDetailsID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<ImportDTO.RequisitionDetailsForToolImportDTO> GetProcessedRequisitionToolDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.RequisitionDetailsForToolImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedRequisitionToolDetails @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion
        public bool UpdateOrderImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_OrderMaster_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool UpdateOrderDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_OrderDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool UpdatePreReceivOrderDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_PreReceivOrderDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();
                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public List<ImportDTO.Import_PULL_DTO> GetSuccessRequisitionPullByImportBulkID(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    List<ImportDTO.Import_PULL_DTO> lstSuccessPullItems = context.Database.SqlQuery<ImportDTO.Import_PULL_DTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetSuccessRequisitionPullByImportBulkID @ImportBulkID", ArrParams).ToList();

                    return lstSuccessPullItems;
                }
                catch
                {
                    return null;
                }
            }
        }

        #region Edit Item

        public List<ImportDTO.EDIItemImportDTO> GetImportItemMaster(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.EDIItemImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportItemMaster @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.InventoryLocationDetailsImportDTO> GetImportItemInventoryLocationDetails(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.InventoryLocationDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportItemInventoryLocationDetails @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.ItemManufacturerDetailsImportDTO> GetImportItemManufacturerDetails(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.ItemManufacturerDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportItemManufacturerDetails @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.ItemSupplierDetailsImportDTO> GetImportItemSupplierDetails(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.ItemSupplierDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportItemSupplierDetails @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.KitDetailsImportDTO> GetImportKitDetails(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    return context.Database.SqlQuery<ImportDTO.KitDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetImportKitDetails @ImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }


        public bool UpdateEditItemImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";

                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_ItemMaster_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();

                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool UpdateItemSupplierDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";

                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_ItemSupplierDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();

                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool UpdateItemInventoryLocationDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";

                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_ItemInventoryLocationDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();

                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool UpdateItemManufacturerDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";

                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_ItemManufacturerDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();

                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public bool UpdateKitDetailsImport(string ImportDBName, long ImportID, bool IsProcessed, DateTime ProcessDate, bool IsSuccess, string ErrorMessage,
                    string StatusMessage, out string Message)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSPSuccess = false;
                Message = "";

                try
                {
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportID", ImportID),
                        new SqlParameter("@IsProcessed", IsProcessed),
                        new SqlParameter("@ProcessDate", ProcessDate),
                        new SqlParameter("@IsSuccess", IsSuccess),
                        new SqlParameter("@ErrorMessage", ErrorMessage  ?? (object)DBNull.Value),
                        new SqlParameter("@StatusMessage", StatusMessage  ?? (object)DBNull.Value)
                    };

                    Message = context.Database.SqlQuery<string>(@"EXEC [" + ImportDBName + "].DBO.CSP_Update_KitDetails_Import @ImportID, @IsProcessed, @ProcessDate, @IsSuccess, @ErrorMessage, @StatusMessage", ArrParams).FirstOrDefault();

                    if (Message.ToUpper() == "SUCCESS")
                    {
                        IsSPSuccess = true;
                    }

                    return IsSPSuccess;
                }
                catch (Exception Ex)
                {
                    Message = Ex.Message;
                    return false;
                }
            }
        }

        public List<ImportDTO.EDIItemImportDTO> GetProcessedEditItems(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.EDIItemImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedItemMaster @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.ItemSupplierDetailsImportDTO> GetProcessedItemSupplierDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.ItemSupplierDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedItemSupplierDetails @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.InventoryLocationDetailsImportDTO> GetProcessedInventoryLocationDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.InventoryLocationDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedItemInventoryLocationDetails @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.ItemManufacturerDetailsImportDTO> GetProcessedItemManufacturerDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.ItemManufacturerDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedItemManufacturerDetails @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<ImportDTO.KitDetailsImportDTO> GetProcessedKitDetails(string ImportDBName, string ArrImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var ArrParams = new SqlParameter[] { new SqlParameter("@ArrImportBulkID", ArrImportBulkID ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<ImportDTO.KitDetailsImportDTO>(@"EXEC [" + ImportDBName + "].DBO.CSP_GetProcessedKitDetails @ArrImportBulkID", ArrParams).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        public List<ImportDTO.Import_RequisitionFile_DTO> GetImportRequisitionDetail(string ImportDBName, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    List<ImportDTO.Import_RequisitionFile_DTO> lstImporOrderImageDetail = null;
                    var ArrParams = new SqlParameter[]
                    {
                        new SqlParameter("@ImportBulkID", ImportBulkID  ?? (object)DBNull.Value)
                    };

                    lstImporOrderImageDetail = (from A in context.Database.SqlQuery<ImportDTO.Import_RequisitionFile_DTO>(@"EXEC [" + ImportDBName + "].DBO.[CSP_GetRequisitionFilesDetail] @ImportBulkID", ArrParams)
                                                select new ImportDTO.Import_RequisitionFile_DTO()
                                                {
                                                    ID = A.ID,
                                                    ImportBulkID = A.ImportBulkID,
                                                    ImportHeaderID = A.ImportHeaderID,
                                                    ImageName = A.ImageName,
                                                    ImportGUID = A.ImportGUID,
                                                    IsDeleted = A.IsDeleted,
                                                    ImageContent = A.ImageContent,
                                                    ImgGUID = A.ImgGUID,
                                                    ImportDate = A.ImportDate,
                                                    ImportedFrom = A.ImportedFrom,
                                                    IsProcessed = A.IsProcessed,
                                                    ProcessDate = A.ProcessDate,
                                                    IsSuccess = A.IsSuccess,
                                                    ErrorMessage = A.ErrorMessage,
                                                    StatusMessage = A.StatusMessage,
                                                    EditedFrom = A.EditedFrom ?? "Web"

                                                }).ToList();

                    return lstImporOrderImageDetail;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
