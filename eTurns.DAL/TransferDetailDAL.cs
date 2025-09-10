using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class TransferDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public TransferDetailDAL(base.DataBaseName)
        //{

        //}

        public TransferDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public TransferDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public List<TransferDetailDTO> GetTransferDetailByTrfGuidPlain(Guid TransferGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferGuid", TransferGuid)
                                                };
                return context.Database.SqlQuery<TransferDetailDTO>("exec [GetTransferDetailByTrfGuidPlain] @TransferGuid ", params1).OrderByDescending(e=> e.ID).ToList();
            }
        }

        public List<TransferDetailDTO> GetTransferDetailByTrfGuidAndSupplierNormal(Guid TransferGuid, List<long> SupplierIds)
        {
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferGUID", TransferGuid),
                                                   new SqlParameter("@SupplierIds", strSupplierIds),
                                                };
                return context.Database.SqlQuery<TransferDetailDTO>("exec [GetTransferDetailByTrfGuidAndSupplierNormal] @TransferGUID,@SupplierIds ", params1).OrderByDescending(e => e.ID).ToList();
            }
        }

        public List<TransferDetailDTO> GetTransferDetailDataByItemGUIDNormal(long RoomID, long CompanyId, List<long> SupplierIds, Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString)) 
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var params1 = new SqlParameter[] { new SqlParameter("@CompnayID", CompanyId),  
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@SupplierIds", strSupplierIds),
                                                   new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", DBNull.Value) };

                return context.Database.SqlQuery<TransferDetailDTO>("EXEC [GetTransferDetailDataByItemGUIDNormal] @CompnayID,@RoomID,@SupplierIds,@ItemGUID,@BinID", params1).ToList();
            }
        }

        public List<TransferDetailDTO> GetTransferDetailDataByItemGUIDAndBinNormal(long RoomID, long CompanyId, List<long> SupplierIds, Guid ItemGUID, long BinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var params1 = new SqlParameter[] { new SqlParameter("@CompnayID", CompanyId),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@SupplierIds", strSupplierIds),
                                                   new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID ) };

                return context.Database.SqlQuery<TransferDetailDTO>("EXEC [GetTransferDetailDataByItemGUIDNormal] @CompnayID,@RoomID,@SupplierIds,@ItemGUID,@BinID", params1).ToList();
            }
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<TransferDetailDTO> GetTransferedRecord(Guid TransferGUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SuppierIds)
        {
            return DB_GetCachedData(CompanyID, RoomID, IsDeleted, IsArchived, null, null, TransferGUID, SuppierIds).ToList();
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<TransferDetailDTO> GetTransferDetailHistoryByTrfGuidNormal(Guid TransferGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TransferGuid", TransferGUID) };

                return context.Database.SqlQuery<TransferDetailDTO>("EXEC [GetTransferDetailHistoryByTrfGuidNormal] @TransferGuid", params1).ToList();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase TransferDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(TransferDetailDTO objDTO, long SessionUserId, bool IsCallFromSVC,long EnterpriseId)
        {
            return DB_Insert(objDTO, SessionUserId, IsCallFromSVC,EnterpriseId);
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(TransferDetailDTO objDTO, long SessionUserId, long EnterpriseId)
        {
            return DB_Edit(objDTO, SessionUserId, EnterpriseId);
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID, long SessionUserId, long EnterpriseId)
        {
            return DB_DeleteTransferDetailRecords(IDs, userid, SessionUserId,EnterpriseId);
        }


        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferDetailDTO> DB_GetCachedData(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? GuID, Guid? TransferGuid, List<long> SupplierIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var params1 = new SqlParameter[] { new SqlParameter("@CompnayID", CompanyID ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value),
                                               new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value),
                                               new SqlParameter("@IsArchived", IsArchived ?? (object)DBNull.Value),
                                               new SqlParameter("@TransferGUID", TransferGuid ?? (object)DBNull.Value),
                                               new SqlParameter("@ID", ID ?? (object)DBNull.Value),
                                               new SqlParameter("@GUID", GuID ?? (object)DBNull.Value),
                                               new SqlParameter("@SupplierIds", strSupplierIds),
                                            };

                IEnumerable<TransferDetailDTO> obj = (from u in context.Database.SqlQuery<TransferDetailDTO>("EXEC Trnsfrdtl_GetTransferDetailData @CompnayID,@RoomID,@IsDeleted,@IsArchived,@TransferGUID,@ID,@GUID,@SupplierIds", params1)
                                                      select new TransferDetailDTO
                                                      {
                                                          ID = u.ID,
                                                          TransferGUID = u.TransferGUID,
                                                          ItemGUID = u.ItemGUID,
                                                          Bin = u.Bin,
                                                          RequestedQuantity = u.RequestedQuantity,
                                                          RequiredDate = u.RequiredDate,
                                                          ReceivedQuantity = u.ReceivedQuantity,
                                                          FulFillQuantity = u.FulFillQuantity,
                                                          ApprovedQuantity = u.ApprovedQuantity,
                                                          Created = u.Created,
                                                          LastUpdated = u.LastUpdated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,
                                                          GUID = u.GUID,
                                                          IntransitQuantity = u.IntransitQuantity,
                                                          ShippedQuantity = u.ShippedQuantity,
                                                          Action = string.Empty,
                                                          HistoryID = 0,
                                                          IsHistory = false,
                                                          AverageCost = u.AverageCost,
                                                          BinName = u.BinName,
                                                          Cost = u.Cost,
                                                          CreatedByName = u.CreatedByName,
                                                          Description = u.Description,
                                                          ItemID = u.ItemID,
                                                          ItemNumber = u.ItemNumber,
                                                          ManufacturerName = u.ManufacturerName,
                                                          ManufacturerNumber = u.ManufacturerNumber,
                                                          OnHandQuantity = u.OnHandQuantity,
                                                          OnOrderQuantity = u.OnOrderQuantity,
                                                          OnTransferQuantity = u.OnTransferQuantity,
                                                          RoomName = u.RoomName,
                                                          SupplierName = u.SupplierName,
                                                          SupplierPartNo = u.SupplierPartNo,
                                                          UpdatedByName = u.UpdatedByName,
                                                          DefaultLocation = u.DefaultLocation,
                                                          SerialNumberTracking = u.SerialNumberTracking,
                                                          LotNumberTracking = u.LotNumberTracking,
                                                          DateCodeTracking = u.DateCodeTracking,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          TotalRecords = 0,
                                                          IsLotSelectionRequire = u.IsLotSelectionRequire,
                                                          ReturnedQuantity = u.ReturnedQuantity,
                                                          StagedQuantity = u.StagedQuantity,
                                                          DestinationBin = u.DestinationBin,
                                                          ItemUDF1 = u.ItemUDF1,
                                                          ItemUDF2 = u.ItemUDF2,
                                                          ItemUDF3 = u.ItemUDF3,
                                                          ItemUDF4 = u.ItemUDF4,
                                                          ItemUDF5 = u.ItemUDF5,
                                                          ItemUDF6 = u.ItemUDF6,
                                                          ItemUDF7 = u.ItemUDF7,
                                                          ItemUDF8 = u.ItemUDF8,
                                                          ItemUDF9 = u.ItemUDF9,
                                                          ItemUDF10 = u.ItemUDF10,
                                                      }).AsParallel().ToList();
                return obj;

            }

        }

        /// <summary>
        /// Get Particullar Record from the TransferDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferDetailDTO DB_GetRecord(Int64? RoomID, Int64? CompanyID, Int64? ID, Guid? GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (GUID == null && ID == null)
                    return null;


                string strCommand = "EXEC Trnsfrdtl_GetTransferDetailData ";

                if (CompanyID.HasValue)
                    strCommand += CompanyID.Value.ToString();
                else
                    strCommand += "null";
                if (RoomID.HasValue)
                    strCommand += ", " + RoomID.Value.ToString();
                else
                    strCommand += ", " + "null";

                strCommand += ", " + "null";
                strCommand += ", " + "null";
                strCommand += ", " + "null";

                if (ID.HasValue)
                    strCommand += ", " + ID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (GUID.HasValue)
                    strCommand += ", '" + GUID.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";


                TransferDetailDTO obj = (from u in context.Database.SqlQuery<TransferDetailDTO>(strCommand)
                                         select new TransferDetailDTO
                                         {
                                             ID = u.ID,
                                             TransferGUID = u.TransferGUID,
                                             ItemGUID = u.ItemGUID,
                                             Bin = u.Bin,
                                             RequestedQuantity = u.RequestedQuantity,
                                             RequiredDate = u.RequiredDate,
                                             ReceivedQuantity = u.ReceivedQuantity,
                                             FulFillQuantity = u.FulFillQuantity,
                                             ApprovedQuantity = u.ApprovedQuantity,
                                             Created = u.Created,
                                             LastUpdated = u.LastUpdated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             Room = u.Room,
                                             IsDeleted = u.IsDeleted,
                                             IsArchived = u.IsArchived,
                                             CompanyID = u.CompanyID,
                                             GUID = u.GUID,
                                             IntransitQuantity = u.IntransitQuantity,
                                             ShippedQuantity = u.ShippedQuantity,
                                             Action = string.Empty,
                                             HistoryID = 0,
                                             IsHistory = false,
                                             AverageCost = u.AverageCost,
                                             BinName = u.BinName,
                                             Cost = u.Cost,
                                             CreatedByName = u.CreatedByName,
                                             Description = u.Description,
                                             ItemID = u.ItemID,
                                             ItemNumber = u.ItemNumber,
                                             ManufacturerName = u.ManufacturerName,
                                             ManufacturerNumber = u.ManufacturerNumber,
                                             OnHandQuantity = u.OnHandQuantity,
                                             OnOrderQuantity = u.OnOrderQuantity,
                                             OnTransferQuantity = u.OnTransferQuantity,
                                             RoomName = u.RoomName,
                                             SupplierName = u.SupplierName,
                                             SupplierPartNo = u.SupplierPartNo,
                                             UpdatedByName = u.UpdatedByName,
                                             DefaultLocation = u.DefaultLocation,
                                             SerialNumberTracking = u.SerialNumberTracking,
                                             ReceivedOn = u.ReceivedOn,
                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                             AddedFrom = u.AddedFrom,
                                             EditedFrom = u.EditedFrom,
                                             TotalRecords = 0,
                                             ControlNumber = u.ControlNumber,
                                             LotNumberTracking = u.LotNumberTracking,
                                             DateCodeTracking = u.DateCodeTracking,
                                             ReturnedQuantity = u.ReturnedQuantity
                                         }).AsParallel().FirstOrDefault();
                return obj;

            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DB_DeleteTransferDetailRecords(string IDs, Int64 userid, long SessionUserId, long EnterpriseId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string strIDs = "";
                string[] strGUIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strGUIDs)
                {
                    strIDs += item + ",";
                }

                strIDs = strIDs.TrimEnd(',');

                strQuery += "Exec Trnsfrdtl_DeleteTransferDetail " + userid.ToString() + ", '" + strIDs + "'";
                if (userid > 0 && !string.IsNullOrWhiteSpace(strQuery))
                {
                    context.Database.ExecuteSqlCommand(strQuery);
                    foreach (string item in strGUIDs)
                    {
                        TransferDetailDTO objTDetailDTO = DB_GetRecord(null, null, null, Guid.Parse(item));
                        ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                        ItemMasterDTO objITemDTO = objItemDAL.GetItemWithoutJoins(null, objTDetailDTO.ItemGUID);
                        objItemDAL.Edit(objITemDTO, SessionUserId,EnterpriseId);
                    }

                    return true;
                }


            }
            return false;
        }

        /// <summary>
        /// Insert Record in the DataBase TransferDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 DB_Insert(TransferDetailDTO objDTO, long SessionUserId, bool IsCallFromSVC, long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> lstSQLPara = new List<SqlParameter>();
                lstSQLPara.Add(new SqlParameter("@TransferGUID", objDTO.TransferGUID));
                lstSQLPara.Add(new SqlParameter("@ItemGUID", objDTO.ItemGUID));
                
                if (objDTO.Bin.HasValue)
                {
                    lstSQLPara.Add(new SqlParameter("@Bin", objDTO.Bin.GetValueOrDefault(0)));
                }                    
                else
                {
                    lstSQLPara.Add(new SqlParameter("@Bin", DBNull.Value));
                }

                lstSQLPara.Add(new SqlParameter("@RequestedQuantity", objDTO.RequestedQuantity));

                if (objDTO.RequiredDate.HasValue)
                {
                    lstSQLPara.Add(new SqlParameter("@RequiredDate", objDTO.RequiredDate));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@RequiredDate", DBNull.Value));
                }

                lstSQLPara.Add(new SqlParameter("@UserID", objDTO.CreatedBy));
                lstSQLPara.Add(new SqlParameter("@RoomID", objDTO.Room));
                lstSQLPara.Add(new SqlParameter("@CompanyID", objDTO.CompanyID));

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                {
                    lstSQLPara.Add(new SqlParameter("@AddedFrom", objDTO.AddedFrom));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@AddedFrom", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                {
                    lstSQLPara.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@EditedFrom", DBNull.Value));
                }

                lstSQLPara.Add(new SqlParameter("@ReceivedOn", objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow)));
                lstSQLPara.Add(new SqlParameter("@ReceivedOnWeb", objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow)));

                if (objDTO.DestinationBinId.HasValue)
                {
                    lstSQLPara.Add(new SqlParameter("@DestinationBinId", objDTO.DestinationBinId.GetValueOrDefault(0)));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@DestinationBinId", DBNull.Value));
                }

                if (IsCallFromSVC)
                {
                    lstSQLPara.Add(new SqlParameter("@GUID", objDTO.GUID));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@GUID", DBNull.Value));
                }

                TransferDetailDTO obj = (from u in context.Database.SqlQuery<TransferDetailDTO>("EXEC [Trnsfrdtl_InsertTransferDetail] @TransferGUID,@ItemGUID,@Bin,@RequestedQuantity,@RequiredDate,@UserID,@RoomID,@CompanyID,@AddedFrom,@EditedFrom,@ReceivedOn,@ReceivedOnWeb,@DestinationBinId,@GUID", lstSQLPara.ToArray())
                                            select new TransferDetailDTO
                                            {
                                                ID = u.ID,
                                                GUID = u.GUID,
                                            }).AsParallel().FirstOrDefault();

                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                ItemMasterDTO objITemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);
                objItemDAL.Edit(objITemDTO, SessionUserId, EnterpriseId);

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                return obj.ID;
                
            }
        }

        /// <summary>
        /// Insert Record in the DataBase TransferDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public bool DB_Edit(TransferDetailDTO objDTO, long SessionUserId, long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommnad = "Exec Trnsfrdtl_UpdateTransferDetail ";

                if (objDTO.Bin.HasValue)
                    strCommnad += "" + objDTO.Bin.GetValueOrDefault(0);
                else
                    strCommnad += "null";

                strCommnad += "," + objDTO.RequestedQuantity;

                if (objDTO.RequiredDate.HasValue)
                    strCommnad += ",'" + objDTO.RequiredDate + "'";
                else
                    strCommnad += ",null";

                if (objDTO.ReceivedQuantity.HasValue)
                    strCommnad += "," + objDTO.ReceivedQuantity;
                else
                    strCommnad += ",null";

                if (objDTO.FulFillQuantity.HasValue)
                    strCommnad += "," + objDTO.FulFillQuantity;
                else
                    strCommnad += ",null";

                strCommnad += "," + objDTO.LastUpdatedBy;
                strCommnad += "," + objDTO.Room;


                if (objDTO.IntransitQuantity.HasValue)
                    strCommnad += "," + objDTO.IntransitQuantity;
                else
                    strCommnad += ",null";


                if (objDTO.ShippedQuantity.HasValue)
                    strCommnad += "," + objDTO.ShippedQuantity;
                else
                    strCommnad += ",null";

                if (objDTO.ApprovedQuantity.HasValue)
                    strCommnad += "," + objDTO.ApprovedQuantity;
                else
                    strCommnad += ",null";

                strCommnad += ",'" + objDTO.GUID + "'";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    strCommnad += ",'" + objDTO.EditedFrom + "'";
                else
                    strCommnad += ",null";
                if (objDTO.ReceivedOn.HasValue)
                    strCommnad += ",'" + objDTO.ReceivedOn.Value + "'";
                else
                    strCommnad += ",null";

                if (!string.IsNullOrEmpty(strCommnad))
                {
                    int cnt = context.Database.ExecuteSqlCommand(strCommnad);

                    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                    ItemMasterDTO objITemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);
                    objItemDAL.Edit(objITemDTO, SessionUserId,EnterpriseId);

                    return true;
                }

                return false;
            }


        }

        /// <summary>
        /// This method is used to get the transferdetails that needs to be render in lot/serial selection popup
        /// </summary>
        /// <param name="lstTransferDetails"></param>
        /// <param name="RoomId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="UserId"></param>
        /// <param name="isForReceiveTransfer"></param>
        /// <returns></returns>
        public List<TransferDetailDTO> GeTransferWithDetails(List<TransferDetailDTO> lstTransferDetails, long RoomId, long CompanyId, long UserId, bool isForReceiveTransfer)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.FirstOrDefault(p => p.ID == RoomId);

                if (lstTransferDetails != null)
                {
                    lstTransferDetails.ForEach(t =>
                    {
                        ItemMaster objItem = context.ItemMasters.FirstOrDefault(p => p.GUID == t.ItemGUID);
                        BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                        BinMaster objBin = new BinMaster();

                        if (isForReceiveTransfer)
                        {
                            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(base.DataBaseName);
                            objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(t.ItemGUID, t.BinName, RoomId, CompanyId, UserId, t.IsStaging);
                            var transferInOutItemDetailDAL = new TransferInOutItemDetailDAL(base.DataBaseName);
                            transferInOutItemDetailDAL.UpdateBinIdForTransInOutItemDetail(t.GUID, t.ItemGUID, objBinMasterDTO.ID);
                        }
                        else
                        {
                            if (t.TransferRequestType.HasValue && t.TransferRequestType.Value == (int)RequestType.Out && !string.IsNullOrEmpty(t.BinName))
                            {
                                var binId = new BinMasterDAL(base.DataBaseName).GetOrInsertBinIDByName(objItem.GUID, t.BinName, UserId, RoomId, CompanyId, false);
                                objBin.ID = binId.GetValueOrDefault(0);
                                objBin.BinNumber = t.BinName;
                            }
                            else
                            {
                                if (t.Bin.HasValue && (t.Bin ?? 0) > 0)
                                {
                                    objBin = context.BinMasters.FirstOrDefault(p => p.ID == t.Bin);
                                    if (objBin == null)
                                    {
                                        objBin = new BinMaster();
                                    }
                                }
                            }
                        }

                        if (objItem != null)
                        {
                            t.SerialNumberTracking = objItem.SerialNumberTracking;
                            t.LotNumberTracking = objItem.LotNumberTracking;
                            t.DateCodeTracking = objItem.DateCodeTracking;
                            t.ItemDetail = t.ItemDetail == null ? new ItemMasterDTO() : t.ItemDetail;
                            t.ItemDetail.GUID = objItem.GUID;
                            t.ItemDetail.ItemType = objItem.ItemType;
                            t.ItemDetail.Consignment = objItem.Consignment;
                            t.ItemNumber = objItem.ItemNumber;
                            t.ItemDetail.BinNumber = isForReceiveTransfer ? objBinMasterDTO.BinNumber : objBin.BinNumber;
                            t.ItemDetail.BinID = isForReceiveTransfer ? objBinMasterDTO.ID : objBin.ID;
                            t.ItemDetail.OutTransferQuantity = t.ApprovedQuantity;
                        }

                    });
                }

                if (lstTransferDetails == null)
                {
                    lstTransferDetails = new List<TransferDetailDTO>();
                }

                return lstTransferDetails;
            }
        }

        /// <summary>
        ///  This method is used to get the item locations with lot serials for transfer(lot/serial selection provision for transfer)
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="BinID"></param>
        /// <param name="TransferQuantity"></param>
        /// <param name="TransferQuantityLimit"></param>
        /// <param name="SerialOrLotNumber"></param>
        /// <param name="IsStagginLocation"></param>
        /// <returns></returns>
        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForTransfer(Guid ItemGUID, long RoomID, long CompanyID, long BinID, double TransferQuantity, string TransferQuantityLimit, string SerialOrLotNumber, string IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>(@"EXEC GetItemLocationsWithLotSerialsForPull '" + ItemGUID.ToString() + "'," + BinID + "," + TransferQuantity + "," + CompanyID + "," + RoomID + "," + TransferQuantityLimit + ",'" + SerialOrLotNumber + "'," + IsStagginLocation)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        //Received = !il.ReceivedDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        //Expiration = !il.ExpirationDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate,
                                        ItemNumber = il.ItemNumber
                                    }).ToList();
                return lstItemLocations;
            }
        }

        /// <summary>
        ///  This method is used to get the item locations with lot serials for receive transfer(lot/serial selection provision for transfer)
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="BinID"></param>
        /// <param name="TransferQuantity"></param>
        /// <param name="TransferQuantityLimit"></param>
        /// <param name="SerialOrLotNumber"></param>
        /// <param name="IsStagginLocation"></param>
        /// <returns></returns>
        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForReceiveTransfer(Guid TransferDetailGUID, Guid ItemGUID, long RoomID, long CompanyID, long BinID, double TransferQuantity, string TransferQuantityLimit, string SerialOrLotNumber, string IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>(@"EXEC GetItemLocationsWithLotSerialsForReceiveTransfer '" + TransferDetailGUID.ToString() + "','" + ItemGUID.ToString() + "'," + BinID + "," + TransferQuantity + "," + CompanyID + "," + RoomID + "," + TransferQuantityLimit + ",'" + SerialOrLotNumber + "'," + IsStagginLocation)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        //Received = !il.ReceivedDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        //Expiration = !il.ExpirationDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate,
                                        ItemNumber = il.ItemNumber
                                    }).ToList();
                return lstItemLocations;
            }
        }

        public int GetTransferDetailRecordCount(Guid TransferGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@TransferGUID", TransferGUID)
                                                };
                return context.Database.SqlQuery<int>("exec [GetTransferDetailRecordCount] @TransferGUID ", params1).FirstOrDefault();
            }
        }

        public List<TransferDetailDTO> GetTransferDetailNormal(Guid TransferGUID, long RoomId, long CompanyId, List<long> SupplierIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@CompnayID", CompanyId ),
                                                   new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@TransferGUID", TransferGUID),
                                                   new SqlParameter("@SupplierIds", strSupplierIds),
                                                    
                                                };
                return context.Database.SqlQuery<TransferDetailDTO>("exec [GetTransferDetailNormal] @CompnayID,@RoomID,@TransferGUID,@SupplierIds ", params1).ToList();
            }
        }

        public TransferDetailDTO GetTransferDetailsByGuidPlain(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@GUID", GUID)                  
                                                };
                return context.Database.SqlQuery<TransferDetailDTO>("exec [GetTransferDetailsByGuidPlain] @GUID ", params1).FirstOrDefault();
            }
        }

        public List<TransferDetailDTO> GetTransferDetailsByTrfGuidAndItemGuidPlain(Guid TransferGuid, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferGUID", TransferGuid),
                                                   new SqlParameter("@ItemGuid", ItemGuid)
                                                };
                return context.Database.SqlQuery<TransferDetailDTO>("exec [GetTransferDetailsByTrfGuidAndItemGuidPlain] @TransferGUID,@ItemGuid ", params1).ToList();
            }
        }
        public List<TransferDetailDTO> GetTransferDetailsByTrfGuidAndItemGuidNormal(Guid TransferGuid, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@TransferGUID", TransferGuid),
                                                   new SqlParameter("@ItemGuid", ItemGuid)
                                                };
                return context.Database.SqlQuery<TransferDetailDTO>("exec [GetTransferDetailsByTrfGuidAndItemGuidNormal] @TransferGUID,@ItemGuid ", params1).ToList();
            }
        }
        public TransferDetailDTO GetTransferDetailsByGuidNormal(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                   new SqlParameter("@GUID", GUID)
                                                };
                return context.Database.SqlQuery<TransferDetailDTO>("exec [GetTransferDetailsByGuidNormal] @GUID ", params1).FirstOrDefault();
            }
        }
    }
}


