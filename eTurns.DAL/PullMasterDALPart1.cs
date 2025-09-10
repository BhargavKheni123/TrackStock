using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;

namespace eTurns.DAL
{
    public partial class PullMasterDALPart1 : eTurnsBaseDAL
    {
        public IEnumerable<PullMasterViewDTO> GetCachedDataForModel(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string PullCredit)
        {
            IEnumerable<PullMasterViewDTO> ObjCache;

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
                ObjCache = (from u in context.ExecuteStoreQuery<PullMasterViewDTO>(@"SELECT A.*,
                            B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,P.ProjectSpendName,
                            I.ItemNumber,I.DefaultPullQuantity,I.OnHandQuantity,I.CategoryID,I.ManufacturerID,I.SupplierID
                            ,I.ItemType,BM.BinNumber,
                            C1.Category AS CategoryName,U1.Unit,I.Markup,I.SellPrice,I.Description  
                           ,I.PackingQuantity,I.ManufacturerNumber, MM.Manufacturer
                            ,I.SupplierPartNo,SM.SupplierName
                            ,I.LongDescription ,GM.GLAccount ,I.Taxable
                            ,I.InTransitquantity,I.OnOrderQuantity
                            ,I.OnTransferQuantity,I.OnHandQuantity,I.CriticalQuantity
                            ,I.MinimumQuantity,I.MaximumQuantity
                            ,I.AverageUsage,I.Turns
                            ,I.IsItemLevelMinMaxQtyRequired
                            ,I.ExtendedCost,I.AverageCost
                            ,I.Consignment
                            ,I.Cost As ItemCost
                            FROM PullMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            LEFT OUTER  JOIN ProjectMaster P on P.GUID = A.ProjectSpendGUID 
                            LEFT OUTER  JOIN ItemMaster I on I.GUID = A.ItemGUID
                            LEFT OUTER join CategoryMaster C1 on C1.id = I.CategoryID
                            LEFT OUTER join UnitMaster U1 on U1.id = I.UOMID 
                            LEFT OUTER  JOIN BinMaster BM on BM.ID = A.BinID 
                            LEFT OUTER  JOIN ManufacturerMaster MM on MM.ID = I.ManufacturerID
                            LEFT OUTER  JOIN SupplierMaster SM on SM.ID = I.SupplierID
                            LEFT OUTER  JOIN GLAccountMaster GM on GM.ID = I.GLAccountID
                            WHERE  PoolQuantity > (isnull(CreditConsignedQuantity,0) + isnull(CreditCustomerOwnedQuantity,0)) And A.CompanyID = " + CompanyID.ToString() + " AND A.Room = " + RoomID.ToString() + " AND lower(A.PullCredit) As PullCredit = " + PullCredit.Trim().ToLower() + "   AND  " + sSQL)
                            select new PullMasterViewDTO
                            {
                                ID = u.ID,
                                ProjectSpendGUID = u.ProjectSpendGUID,
                                ProjectName = u.ProjectName,
                                ProjectSpendName = u.ProjectSpendName,
                                BinNumber = u.BinNumber,
                                DefaultPullQuantity = u.DefaultPullQuantity.GetValueOrDefault(0),//Math.Round(u.DefaultPullQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                OnHandQuantity = u.OnHandQuantity,
                                ItemGUID = u.ItemGUID,
                                ItemNumber = u.ItemNumber,
                                CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),// Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                CreditConsignedQuantity = u.CreditConsignedQuantity.GetValueOrDefault(0),//Math.Round(u.CreditConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CreditCustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),//Math.Round(u.PoolQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                PullCost = u.PullCost.GetValueOrDefault(0),//Math.Round(u.PullCost.GetValueOrDefault(0), objRegionInfo.CurrencyDecimalDigits),
                                SerialNumber = u.SerialNumber,
                                LotNumber = u.LotNumber,
                                DateCode = u.DateCode,
                                BinID = u.BinID,
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
                                SupplierID = u.SupplierID,
                                ManufacturerID = u.ManufacturerID,
                                CategoryID = u.CategoryID,
                                CategoryName = u.CategoryName,
                                Markup = u.Markup,
                                ItemCost = u.ItemCost,
                                SellPrice = u.SellPrice,
                                Description = u.Description,
                                Unit = u.Unit,
                                PullCredit = u.PullCredit,
                                ActionType = u.ActionType,
                                RequisitionDetailGUID = u.RequisitionDetailGUID,
                                //WorkOrderDetailID = u.WorkOrderDetailID,
                                WorkOrderDetailGUID = u.WorkOrderDetailGUID,
                                Billing = u.Billing.GetValueOrDefault(false),
                                CountLineItemGuid = u.CountLineItemGuid,
                                PackingQuantity = u.PackingQuantity,
                                Manufacturer = u.Manufacturer,
                                ManufacturerNumber = u.ManufacturerNumber,
                                SupplierName = u.SupplierName,
                                SupplierPartNo = u.SupplierPartNo,
                                AverageUsage = u.AverageUsage,
                                CriticalQuantity = u.CriticalQuantity,
                                GLAccount = u.GLAccount,
                                InTransitquantity = u.InTransitquantity,
                                IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                LongDescription = u.LongDescription,
                                MaximumQuantity = u.MaximumQuantity,
                                MinimumQuantity = u.MinimumQuantity,
                                Turns = u.Turns,
                                Taxable = u.Taxable,
                                OnOrderQuantity = u.OnOrderQuantity,
                                OnTransferQuantity = u.OnTransferQuantity,
                                Consignment = u.Consignment,
                                ItemOnhandQty = u.ItemOnhandQty,
                                IsAddedFromPDA = u.IsAddedFromPDA,
                                IsProcessedAfterSync = u.IsProcessedAfterSync,
                                ItemStageQty = u.ItemStageQty,
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                ExtendedCost = u.ExtendedCost,
                                AverageCost = u.AverageCost,
                                PullPrice = u.PullPrice.GetValueOrDefault(0)
                            }).AsParallel().ToList();
            }
            #endregion

            return ObjCache;
        }

        /// <summary>
        /// Get Particullar Record from the PullMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public PullMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<PullMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM PullMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new PullMasterDTO
                        {
                            ID = u.ID,

                            ProjectSpendGUID = u.ProjectSpendGUID,
                            UOI = u.UOI,
                            PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),//Math.Round(u.PoolQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                            PULLCost = u.PULLCost.GetValueOrDefault(0),//Math.Round(u.PULLCost.GetValueOrDefault(0), objRegionInfo.CurrencyDecimalDigits),
                            SerialNumber = u.SerialNumber,
                            LotNumber = u.LotNumber,
                            DateCode = u.DateCode,
                            BinID = u.BinID,
                            Turns = u.Turns,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ControlNumber = u.ControlNumber,
                            PULLPrice = u.PULLPrice.GetValueOrDefault(0)
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Method for fast loading of items...
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public PullMasterViewDTO FillWithExtraDetail(PullMasterViewDTO objDTO)
        {
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            RoomDTO ObjRoomCache = null;
            if (objDTO.Room.GetValueOrDefault(0) > 0)
            {
                ObjRoomCache = objRoomDAL.GetRecord(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false);
                objDTO.RoomName = ObjRoomCache.RoomName;
            }
            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            UserMasterDTO ObjUserCreatedCache = objUserDAL.GetRecord(objDTO.CreatedBy.GetValueOrDefault(0));
            UserMasterDTO ObjUserUpdatedCache = objUserDAL.GetRecord(objDTO.LastUpdatedBy.GetValueOrDefault(0));

            if (ObjUserCreatedCache != null)
                objDTO.CreatedByName = ObjUserCreatedCache.UserName;

            if (ObjUserUpdatedCache != null)
                objDTO.UpdatedByName = ObjUserUpdatedCache.UserName;

            BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO objBinDTO = null;
            if (objDTO.BinID.GetValueOrDefault(0) > 0)
            {
                objBinDTO = objBinDAL.GetBinByID(objDTO.BinID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                //objBinDTO = objBinDAL.GetItemLocation(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false,Guid.Empty, objDTO.BinID.GetValueOrDefault(0), null,null).FirstOrDefault();
            }
            if (objBinDTO != null)
                objDTO.BinNumber = objBinDTO.BinNumber;

            ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
            ProjectMasterDTO objProjectDTO = null;
            if (objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objProjectDTO = objProjectDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false);
            }
            if (objProjectDTO != null)
                objDTO.ProjectSpendName = objProjectDTO.ProjectSpendName;


            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItemDTO = null;
            if (objDTO.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
            }
            if (objItemDTO != null)
            {
                objDTO.ItemNumber = objItemDTO.ItemNumber;
                objDTO.ItemType = objItemDTO.ItemType;
                objDTO.Markup = objItemDTO.Markup.GetValueOrDefault(0);
                objDTO.SellPrice = objItemDTO.SellPrice.GetValueOrDefault(0);
                objDTO.CategoryName = objItemDTO.CategoryName;
                objDTO.Unit = objItemDTO.Unit;
                objDTO.Description = objItemDTO.Description;
                objDTO.DefaultPullQuantity = objItemDTO.DefaultPullQuantity.GetValueOrDefault(0);
            }
            return objDTO;
        }

        public bool BinWiseQuantityCheckNew(ItemLocationQTYDTO lstLocDTO, ItemLocationQTYDAL objLocQTY, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, out string ItemLocationMSG, BinMasterDTO objBINDTO, PullMasterViewDTO objDTO, BinMasterDAL objBINDAL, ItemMasterDTO ItemDTO, bool IsConsingedPull)
        {
            #region "Bin Wise Quantity Check"
            // if selected location has not enough quantity then send message  and don't save data                

            //string LocationName = objBINDTO.BinNumber;
            string LocationName = string.Empty; ;
            if (lstLocDTO != null)
            {
                LocationName = lstLocDTO.BinNumber;
            }


            if (objDTO.ID == 0)
                objDTO.TempPullQTY = objDTO.PoolQuantity.GetValueOrDefault(0);

            if (objDTO.TempPullQTY == 0)
            {
                ItemLocationMSG = "Enter Proper value instead of (0)";
                return true;
            }
            if (IsCreditPullNothing != 1) // No credit
            {
                if (lstLocDTO != null)
                {
                    Double AvailableQuantity = 0;
                    if (IsConsingedPull)
                    {
                        AvailableQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                        ItemLocationMSG = "Not Enough Customer and Consigned Quantity for Location ## " + LocationName + "(Avl.QTY=" + lstLocDTO.Quantity.ToString() + ")";
                    }
                    else
                    {
                        AvailableQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                        ItemLocationMSG = "Not Enough Customer Quantity for Location ## " + LocationName + "(Avl.QTY=" + lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + ")";
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
                    ItemLocationMSG = "Not Enough Quantity for Location ## " + LocationName + "(Avl.QTY=0)";
                    return true;
                }
            }
            #endregion
            ItemLocationMSG = "";
            return false;
        }

        public bool ProjectWiseQuantityCheckNew(ProjectMasterDAL objPrjMsgDAL, ProjectSpendItemsDTO objPrjSpenItmDTO, ProjectMasterDTO objPrjMstDTO, ProjectSpendItemsDAL objPrjSpenItmDAL, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, out string ItemLocationMSG, BinMasterDTO objBINDTO, PullMasterViewDTO objDTO, BinMasterDAL objBINDAL, ItemMasterDTO ItemDTO, bool IsProjectSpendAllowed, out bool IsPSLimitExceed, List<ItemLocationDetailsDTO> ObjItemLocation, bool IsConsingedPull)
        {
            double pickPrice = 0;
            #region "Project Wise Quantity Check"
            if (IsProjectSpendAllowed == false)
            {
                if (objPrjMstDTO != null)
                {
                    if (ItemDTO.SerialNumberTracking)
                    {
                        foreach (var iLocDetail in ObjItemLocation)
                        {
                            if (IsConsingedPull)
                            {
                                pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) * (double)(iLocDetail.Cost ?? 0)) + (iLocDetail.ConsignedQuantity.GetValueOrDefault(0) * (double)(iLocDetail.Cost ?? 0));
                            }
                            else
                            {
                                pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) * (double)(iLocDetail.Cost ?? 0));
                            }
                        }
                    }
                    else
                    {
                        double localPickQTY = 0;
                        foreach (var iLocDetail in ObjItemLocation)
                        {
                            if (IsConsingedPull)
                            {
                                if (objDTO.TempPullQTY.GetValueOrDefault(0) != localPickQTY)
                                {
                                    if ((iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) >= (objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY))
                                    {
                                        pickPrice += ((objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY) * (double)(iLocDetail.Cost ?? 0));
                                        localPickQTY += objDTO.TempPullQTY.GetValueOrDefault(0);
                                    }
                                    else if ((iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) < (objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY))
                                    {
                                        localPickQTY += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0));
                                        pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + iLocDetail.ConsignedQuantity.GetValueOrDefault(0)) * (double)(iLocDetail.Cost ?? 0);
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
                                        pickPrice += (objDTO.TempPullQTY.GetValueOrDefault(0) * (double)(iLocDetail.Cost ?? 0));
                                    }
                                    else if (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) < (objDTO.TempPullQTY.GetValueOrDefault(0) - localPickQTY))
                                    {
                                        localPickQTY += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - localPickQTY);
                                        pickPrice += (iLocDetail.CustomerOwnedQuantity.GetValueOrDefault(0) * (double)(iLocDetail.Cost ?? 0));
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
                        ItemLocationMSG = "Project spend Dollar Amount limit exceed";
                        IsPSLimitExceed = true;
                        return true;
                    }
                }
                if (objPrjSpenItmDTO != null)
                {
                    if (objPrjSpenItmDTO.QuantityLimit.GetValueOrDefault(0) > 0 && (objDTO.TempPullQTY.GetValueOrDefault(0) + objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0)) > objPrjSpenItmDTO.QuantityLimit.GetValueOrDefault(0))
                    {
                        ItemLocationMSG = "Project spend Item's Quantity limit exceed";
                        IsPSLimitExceed = true;
                        return true;
                    }
                    if (objPrjSpenItmDTO.DollarLimitAmount.GetValueOrDefault(0) > 0 && ((decimal)pickPrice + objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0)) > objPrjSpenItmDTO.DollarLimitAmount.GetValueOrDefault(0))
                    {
                        ItemLocationMSG = "Project spend Item's Dollar limit exceed";
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

        public List<RPT_PullMasterDTO> GetPullMasterData(Int64[] CompanyIDs, Int64[] RoomIDs, DateTime? StartDate, DateTime? EndDate, Int64? UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                for (int i = 0; i < CompanyIDs.Length; i++)
                {
                    if (compIDs.Length > 0)
                        compIDs += ",";

                    compIDs += CompanyIDs[i];
                }

                for (int i = 0; i < RoomIDs.Length; i++)
                {
                    if (roomIDs.Length > 0)
                        roomIDs += ",";

                    roomIDs += RoomIDs[i];
                }

                string strQuery = "Exec RPT_GetPullHeaderData ";
                if (StartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                    strQuery += "'" + StartDate.GetValueOrDefault(DateTime.MinValue).ToShortDateString() + "'";
                else
                    strQuery += "NULL";

                if (EndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                    strQuery += ",'" + EndDate.GetValueOrDefault(DateTime.MinValue).ToShortDateString() + "'";
                else
                    strQuery += ", NULL";

                if (string.IsNullOrEmpty(roomIDs))
                    strQuery += ",'" + roomIDs + "'";
                else
                    strQuery += ", NULL";

                if (string.IsNullOrEmpty(compIDs))
                    strQuery += ",'" + compIDs + "'";
                else
                    strQuery += ", NULL";

                strQuery += ",NULL , 0, 0, NULL, NULL";

                if (UserID.GetValueOrDefault(0) > 0)
                    strQuery += "," + UserID;
                else
                    strQuery += ", NULL";

                //IEnumerable<PullMasterViewDTO> obj = (from u in context.ExecuteStoreQuery<PullMasterViewDTO>(@"SELECT A.*,");
                //IEnumerable<PullMasterViewDTO> ienPullView = (from u in context.PullMasters
                //                                              join i in context.ItemMasters on u.ItemGUID equals i.GUID into PI_join
                //                                              from u_i in PI_join.DefaultIfEmpty()
                //                                              where CompanyIDs.Contains(u.CompanyID ?? 0) && RoomIDs.Contains(u.Room ?? 0)
                IEnumerable<RPT_PullMasterDTO> ienPullView = context.ExecuteStoreQuery<RPT_PullMasterDTO>(strQuery);

                return ienPullView.ToList();

            }


        }

        private string getSortColumnName(string sortColumnName)
        {
            if (!string.IsNullOrWhiteSpace(sortColumnName))
            {
                return sortColumnName.Replace("ID", "PullId").Replace("ItemCost", "Cost").Replace("Manufacturer", "manufacturername").Replace("UDF1", "Pull_UDF1").Replace("UDF2", "Pull_UDF2").Replace("UDF3", "Pull_UDF3").Replace("UDF4", "Pull_UDF4").Replace("UDF5", "Pull_UDF5").Replace("Created", "pull_Created").Replace("Updated", "pull_Updated").Replace("pull_CreatedByName", "createdby").Replace("pull_UpdatedByName", "updated").Replace("manufacturernameNumber", "manufacturernumber").Replace("ReceivedOnPullWeb", "ReceivedOnPull").Replace("ReceivedOnWeb", "ReceivedOnWebPull").Replace("EditedFrom", "EditedFromPull").Replace("AddedFrom", "AddedFromPull");
            }
            else
            {
                return "PullID DESC";
            }
        }

        public PullMasterViewDTO GetRecordWithoutCache(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            #region "Both False"

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PullMasterViewDTO obj = (from u in context.ExecuteStoreQuery<PullMasterViewDTO>(@"SELECT A.*,ISNULL(A.PullCredit,'Pull') As tempPullCredit,ISNULL(A.ActionType,'Pull') as tempActionType,
                           B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,P.ProjectSpendName,
                           I.ItemNumber,I.DefaultPullQuantity,I.OnHandQuantity,I.CategoryID,I.ManufacturerID,
                           I.SupplierID,I.ItemType,BM.BinNumber,
                           C1.Category AS CategoryName,U1.Unit,I.Markup,I.SellPrice,I.Description  
                           ,I.PackingQuantity,I.ManufacturerNumber, MM.Manufacturer
                           ,I.SupplierPartNo,SM.SupplierName
                           ,I.LongDescription ,GM.GLAccount ,I.Taxable
                           ,I.InTransitquantity,I.OnOrderQuantity
                           ,I.OnTransferQuantity,I.OnHandQuantity,I.CriticalQuantity
                           ,I.MinimumQuantity,I.MaximumQuantity
                           ,I.AverageUsage,I.Turns
                           ,I.IsItemLevelMinMaxQtyRequired
                           ,I.Consignment
                           ,I.UDF1 AS ItemUDF1
                           ,I.UDF2 AS ItemUDF2
                           ,I.UDF3 AS ItemUDF3
                           ,I.UDF4 AS ItemUDF4
                           ,I.UDF5 AS ItemUDF5
                           ,I.Cost AS ItemCost
                           ,REQM.RequisitionNumber
                            FROM PullMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            LEFT OUTER  JOIN ProjectMaster P on P.GUID = A.ProjectSpendGUID 
                            LEFT OUTER  JOIN ItemMaster I on I.GUID = A.ItemGUID
                            LEFT OUTER join CategoryMaster C1 on C1.id = I.CategoryID
                            LEFT OUTER join UnitMaster U1 on U1.id = I.UOMID 
                            LEFT OUTER  JOIN BinMaster BM on BM.ID = A.BinID 
                            LEFT OUTER  JOIN ManufacturerMaster MM on MM.ID = I.ManufacturerID
                            LEFT OUTER  JOIN SupplierMaster SM on SM.ID = I.SupplierID
                            LEFT OUTER  JOIN GLAccountMaster GM on GM.ID = I.GLAccountID
                            LEFT OUTER  JOIN RequisitionDetails REQD on A.RequisitionDetailGUID = REQD.GUID
                            LEFT OUTER  JOIN RequisitionMaster REQM on REQD.RequisitionGUID = REQM.GUID
                            WHERE A.CompanyID = " + CompanyID.ToString() + " AND A.GUID = '" + GUID.ToString() + "'")
                                         select new PullMasterViewDTO
                                         {
                                             ID = u.ID,
                                             ProjectSpendGUID = u.ProjectSpendGUID,
                                             ProjectName = u.ProjectName,
                                             ProjectSpendName = u.ProjectSpendName,
                                             BinNumber = u.BinNumber,
                                             //DefaultPullQuantity = Math.Round(u.DefaultPullQuantity.GetValueOrDefault(0), singleTonConfigDTO.QuantityDecimalPoints.GetValueOrDefault(0)),
                                             DefaultPullQuantity = u.DefaultPullQuantity.GetValueOrDefault(0),// Math.Round(u.DefaultPullQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                             OnHandQuantity = u.OnHandQuantity.GetValueOrDefault(0),//Math.Round(u.OnHandQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                             ItemGUID = u.ItemGUID,
                                             ItemNumber = u.ItemNumber,
                                             CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),// Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                             ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),// Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                             CreditConsignedQuantity = u.CreditConsignedQuantity.GetValueOrDefault(0),//Math.Round(u.CreditConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                             CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CreditCustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                             PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),// Math.Round(u.PoolQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                             PullCost = u.PullCost.GetValueOrDefault(0),//Math.Round(u.PullCost.GetValueOrDefault(0), objRegionInfo.CurrencyDecimalDigits),
                                             //PullCost = u.PullCost,
                                             SerialNumber = u.SerialNumber,
                                             LotNumber = u.LotNumber,
                                             DateCode = u.DateCode,
                                             BinID = u.BinID,
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5,
                                             ItemUDF1 = u.ItemUDF1,
                                             ItemUDF2 = u.ItemUDF2,
                                             ItemUDF3 = u.ItemUDF3,
                                             ItemUDF4 = u.ItemUDF4,
                                             ItemUDF5 = u.ItemUDF5,
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
                                             SupplierID = u.SupplierID,
                                             ManufacturerID = u.ManufacturerID,
                                             CategoryID = u.CategoryID,
                                             CategoryName = u.CategoryName,
                                             Markup = u.Markup,
                                             SellPrice = u.SellPrice,
                                             ItemCost = u.ItemCost,
                                             Description = u.Description,
                                             Unit = u.Unit,
                                             PullCredit = u.tempPullCredit,
                                             ActionType = u.tempActionType,
                                             //PullCredit = (u.PullCredit == null ? u.PullCredit : "Pull"),
                                             //ActionType = (u.ActionType == null ? u.ActionType : string.Empty),
                                             ItemType = u.ItemType,
                                             RequisitionDetailGUID = u.RequisitionDetailGUID,
                                             //WorkOrderDetailID = u.WorkOrderDetailID,
                                             WorkOrderDetailGUID = u.WorkOrderDetailGUID,
                                             Billing = u.Billing.GetValueOrDefault(false),
                                             CountLineItemGuid = u.CountLineItemGuid,
                                             PackingQuantity = u.PackingQuantity,
                                             Manufacturer = u.Manufacturer,
                                             ManufacturerNumber = u.ManufacturerNumber,
                                             SupplierName = u.SupplierName,
                                             SupplierPartNo = u.SupplierPartNo,
                                             AverageUsage = u.AverageUsage,
                                             CriticalQuantity = u.CriticalQuantity,
                                             GLAccount = u.GLAccount,
                                             InTransitquantity = u.InTransitquantity,
                                             IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                             LongDescription = u.LongDescription,
                                             MaximumQuantity = u.MaximumQuantity,
                                             MinimumQuantity = u.MinimumQuantity,
                                             Turns = u.Turns,
                                             Taxable = u.Taxable,
                                             OnOrderQuantity = u.OnOrderQuantity,
                                             OnTransferQuantity = u.OnTransferQuantity,
                                             Consignment = u.Consignment,
                                             ItemOnhandQty = u.ItemOnhandQty,
                                             IsAddedFromPDA = u.IsAddedFromPDA,
                                             IsProcessedAfterSync = u.IsProcessedAfterSync,
                                             ItemStageQty = u.ItemStageQty,
                                             ItemStageLocationQty = u.ItemStageLocationQty,
                                             ItemLocationOnHandQty = u.ItemLocationOnHandQty,
                                             ReceivedOn = u.ReceivedOn,
                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                             AddedFrom = u.AddedFrom,
                                             EditedFrom = u.EditedFrom,
                                             PullOrderNumber = u.PullOrderNumber,
                                             ControlNumber = u.ControlNumber,
                                             PullPrice = u.PullPrice.GetValueOrDefault(0)
                                         }).FirstOrDefault();
                return obj;
            }

            #endregion
        }

        private void PullItemFromStagingBin(ItemInfoToPull ItemPullData, bool IsFIFO)
        {

        }

        public IEnumerable<PullMasterViewDTO> GetAllMSRecords(Int64 RoomID, Int64 CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var objPullMasterViewDTO = (from u in context.ExecuteStoreQuery<PullMasterViewDTO>(@"SELECT A.*,ISNULL(A.PullCredit,'Pull') As tempPullCredit,ISNULL(A.ActionType,'Pull') as tempActionType,
                            B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,P.ProjectSpendName,
                            I.ItemNumber,I.DefaultPullQuantity,I.OnHandQuantity,I.CategoryID,I.ManufacturerID,
                            I.SupplierID,I.ItemType,BM.BinNumber,
                            C1.Category AS CategoryName,U1.Unit,I.Markup,I.SellPrice,I.Description  
                            ,I.PackingQuantity,I.ManufacturerNumber, MM.Manufacturer
                            ,I.SupplierPartNo,SM.SupplierName
                            ,I.LongDescription ,GM.GLAccount ,I.Taxable
                            ,I.InTransitquantity,I.OnOrderQuantity
                            ,I.OnTransferQuantity,I.OnHandQuantity,I.CriticalQuantity
                            ,I.MinimumQuantity,I.MaximumQuantity
                            ,I.AverageUsage,I.Turns
                            ,I.ExtendedCost,I.AverageCost
                            ,I.IsItemLevelMinMaxQtyRequired
                            ,I.Consignment
                            ,I.UDF1 AS ItemUDF1
                            ,I.UDF2 AS ItemUDF2
                            ,I.UDF3 AS ItemUDF3
                            ,I.UDF4 AS ItemUDF4
                            ,I.UDF5 AS ItemUDF5
                            ,I.Cost AS ItemCost
                            ,REQM.RequisitionNumber
                            ,WOO.WOName
                            ,REQD.RequisitionGUID
                            FROM PullMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            LEFT OUTER  JOIN ProjectMaster P on P.GUID = A.ProjectSpendGUID 
                            LEFT OUTER  JOIN ItemMaster I on I.GUID = A.ItemGUID
                            LEFT OUTER join CategoryMaster C1 on C1.id = I.CategoryID
                            LEFT OUTER join UnitMaster U1 on U1.id = I.UOMID 
                            LEFT OUTER  JOIN BinMaster BM on BM.ID = A.BinID 
                            LEFT OUTER  JOIN ManufacturerMaster MM on MM.ID = I.ManufacturerID
                            LEFT OUTER  JOIN SupplierMaster SM on SM.ID = I.SupplierID
                            LEFT OUTER  JOIN GLAccountMaster GM on GM.ID = I.GLAccountID
                            LEFT OUTER  JOIN RequisitionDetails REQD on A.RequisitionDetailGUID = REQD.GUID
                            LEFT OUTER  JOIN RequisitionMaster REQM on REQD.RequisitionGUID = REQM.GUID
                            LEFT OUTER  JOIN WorkOrder WOO on A.WorkOrderDetailGUID = WOO.GUID
                            WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.Room = " + RoomID + " AND A.CompanyID = " + CompanyId + " and (A.PullCredit in ('cr_evmi','credit','P_eVMI','pull','ms pull','ms credit','credit:evmi') OR A.ActionType in ('cr_evmi','credit','P_eVMI','pull','ms pull','ms credit','credit:evmi')) Order by ID DESC")
                                            select new PullMasterViewDTO
                                            {
                                                ID = u.ID,
                                                ProjectSpendGUID = u.ProjectSpendGUID,
                                                ProjectName = u.ProjectName,
                                                ProjectSpendName = u.ProjectSpendName,
                                                BinNumber = u.BinNumber,
                                                //DefaultPullQuantity = Math.Round(u.DefaultPullQuantity.GetValueOrDefault(0), singleTonConfigDTO.QuantityDecimalPoints.GetValueOrDefault(0)),
                                                DefaultPullQuantity = u.DefaultPullQuantity.GetValueOrDefault(0),// Math.Round(u.DefaultPullQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                                OnHandQuantity = u.OnHandQuantity.GetValueOrDefault(0),// Math.Round(u.OnHandQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                                ItemGUID = u.ItemGUID,
                                                ItemNumber = u.ItemNumber,
                                                CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                                ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),// Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                                CreditConsignedQuantity = u.CreditConsignedQuantity.GetValueOrDefault(0),// Math.Round(u.CreditConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                                CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CreditCustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                                PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),//Math.Round(u.PoolQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
                                                PullCost = u.PullCost.GetValueOrDefault(0),//Math.Round(u.PullCost.GetValueOrDefault(0), objRegionInfo.CurrencyDecimalDigits),
                                                //PullCost = u.PullCost,
                                                SerialNumber = u.SerialNumber,
                                                LotNumber = u.LotNumber,
                                                DateCode = u.DateCode,
                                                BinID = u.BinID,
                                                UDF1 = u.UDF1,
                                                UDF2 = u.UDF2,
                                                UDF3 = u.UDF3,
                                                UDF4 = u.UDF4,
                                                UDF5 = u.UDF5,
                                                ItemUDF1 = u.ItemUDF1,
                                                ItemUDF2 = u.ItemUDF2,
                                                ItemUDF3 = u.ItemUDF3,
                                                ItemUDF4 = u.ItemUDF4,
                                                ItemUDF5 = u.ItemUDF5,
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
                                                SupplierID = u.SupplierID,
                                                ManufacturerID = u.ManufacturerID,
                                                CategoryID = u.CategoryID,
                                                CategoryName = u.CategoryName,
                                                Markup = u.Markup,
                                                SellPrice = u.SellPrice,
                                                ItemCost = u.ItemCost,
                                                Description = u.Description,
                                                Unit = u.Unit,
                                                PullCredit = u.tempPullCredit,
                                                ActionType = u.tempActionType,
                                                //PullCredit = (u.PullCredit == null ? u.PullCredit : "Pull"),
                                                //ActionType = (u.ActionType == null ? u.ActionType : string.Empty),
                                                ItemType = u.ItemType,
                                                RequisitionDetailGUID = u.RequisitionDetailGUID,
                                                //WorkOrderDetailID = u.WorkOrderDetailID,
                                                WorkOrderDetailGUID = u.WorkOrderDetailGUID,
                                                Billing = u.Billing.GetValueOrDefault(false),
                                                CountLineItemGuid = u.CountLineItemGuid,
                                                PackingQuantity = u.PackingQuantity,
                                                Manufacturer = u.Manufacturer,
                                                ManufacturerNumber = u.ManufacturerNumber,
                                                SupplierName = u.SupplierName,
                                                SupplierPartNo = u.SupplierPartNo,
                                                AverageUsage = u.AverageUsage,
                                                CriticalQuantity = u.CriticalQuantity,
                                                GLAccount = u.GLAccount,
                                                InTransitquantity = u.InTransitquantity,
                                                IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                                LongDescription = u.LongDescription,
                                                MaximumQuantity = u.MaximumQuantity,
                                                MinimumQuantity = u.MinimumQuantity,
                                                Turns = u.Turns,
                                                Taxable = u.Taxable,
                                                OnOrderQuantity = u.OnOrderQuantity,
                                                OnTransferQuantity = u.OnTransferQuantity,
                                                Consignment = u.Consignment,
                                                ItemOnhandQty = u.ItemOnhandQty,
                                                IsAddedFromPDA = u.IsAddedFromPDA,
                                                IsProcessedAfterSync = u.IsProcessedAfterSync,
                                                ItemStageQty = u.ItemStageQty,
                                                ItemStageLocationQty = u.ItemStageLocationQty,
                                                ItemLocationOnHandQty = u.ItemLocationOnHandQty,
                                                ReceivedOn = u.ReceivedOn,
                                                ReceivedOnWeb = u.ReceivedOnWeb,
                                                AddedFrom = u.AddedFrom,
                                                EditedFrom = u.EditedFrom,
                                                ExtendedCost = u.ExtendedCost,
                                                AverageCost = u.AverageCost,
                                                ControlNumber = u.ControlNumber,
                                                WOName = u.WOName,
                                                RequisitionGUID = u.RequisitionGUID,
                                                RequisitionNumber = u.RequisitionNumber,
                                                PullPrice = u.PullPrice.GetValueOrDefault(0)
                                            }).AsParallel().ToList();

                return objPullMasterViewDTO;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        //public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID, out string MSG)
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            string MSG = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool isAllowDelete = false;
                string strQuery = "";
                string PULLGUID = "";
                PullMasterViewDTO objDTO = null;
                try
                {
                    string[] strPullIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strLocDetailQryFroCredit = "";
                    #region "Reverse process after Successful DELETE"
                    foreach (var item in strPullIDs)
                    {
                        objDTO = new PullMasterViewDTO();
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            objDTO = GetRecord(Int64.Parse(item), RoomID, CompanyID, false, false);
                            PULLGUID = objDTO.GUID.ToString();
                            isAllowDelete = PullCreditAfterDelete(objDTO, 1, RoomID, CompanyID, out MSG);

                            //below code is to credit back the requisitioned quantity which has pulled during pull from requisition
                            if (isAllowDelete && objDTO.RequisitionDetailGUID != null)
                            {
                                RequisitionDetailsDAL objReqDtlDal = new RequisitionDetailsDAL(base.DataBaseName);
                                objReqDtlDal.UpdateItemOnRequisitionQty(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0));

                                // Below code is to credit back the approved and pulled qty in Requisition module
                                RequisitionDetailsDTO objRequDetDTO = objReqDtlDal.GetRequisitionDetailsByGUIDPlain(objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty));
                                if (objRequDetDTO != null)
                                {
                                    objRequDetDTO.QuantityPulled = objRequDetDTO.QuantityPulled.GetValueOrDefault(0) - objDTO.PoolQuantity.GetValueOrDefault(0);
                                    objReqDtlDal.Edit(objRequDetDTO);
                                }
                            }

                        }
                    }
                    #endregion

                    if (isAllowDelete)
                    {
                        foreach (var item in strPullIDs)
                        {
                            if (!string.IsNullOrEmpty(item.Trim()))
                            {
                                strQuery += "UPDATE PullMaster SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE ID =" + item + "; ";
                                strQuery += "UPDATE PullDetails SET Updated = getutcdate(), LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE PULLGUID IN (SELECT GUID FROM PULLMaster WHERE ID=" + item + ");";
                                strQuery += @"Update ItemLocationDetails SET InitialQuantity = 0,InitialQuantityWeb=0,InitialQuantityPDA =0 
                                              WHERE Guid IN (SELECT ItemLocationDetailGuid FROM PullDetails 
                                                             WHERE PullGuid in (SELECT Guid FROM PullMaster WHERE ID = " + item + ") AND CHARIndex('cr',PullCredit)>0 ); ";
                                //strQuery += "UPDATE PullDetails SET Updated = getutcdate(), LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE PULLGUID ='" + PULLGUID + "';";
                            }
                        }
                        context.ExecuteStoreCommand(strQuery);

                        foreach (var item in strPullIDs)
                        {
                            if (!string.IsNullOrEmpty(item.Trim()))
                            {
                                objDTO = GetRecord(Int64.Parse(item), RoomID, CompanyID, true, false);

                                if (objDTO != null && objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                                    objWOLDAL.UpdateWOItemAndTotalCost(objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));

                                }
                                if (objDTO != null)
                                {
                                    DashboardDAL objdashBoard = new DashboardDAL(base.DataBaseName);
                                    objdashBoard.UpdateTurnsByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID ?? Guid.Empty, objDTO.LastUpdatedBy.GetValueOrDefault(0));
                                    objdashBoard.UpdateAvgUsageByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID ?? Guid.Empty, objDTO.LastUpdatedBy.GetValueOrDefault(0));
                                }

                            }
                        }
                        //Get Cached-Media
                        IEnumerable<PullMasterViewDTO> ObjCache = CacheHelper<IEnumerable<PullMasterViewDTO>>.GetCacheItem("Cached_PullMaster_" + CompanyID.ToString());
                        if (ObjCache != null)
                        {
                            List<PullMasterViewDTO> objTemp = ObjCache.ToList();
                            objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                            ObjCache = objTemp.AsEnumerable();
                            CacheHelper<IEnumerable<PullMasterViewDTO>>.AppendToCacheItem("Cached_PullMaster_" + CompanyID.ToString(), ObjCache);
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
                catch (Exception)
                {
                    MSG = "error";
                    return false;
                }
            }
        }

        public List<PullMasterViewDTO> GetPullByGuids(Int64 RoomID, Int64 CompanyID, string Guids)
        {
            //List<Guid> lstGuids = new List<Guid>();
            //if (!string.IsNullOrWhiteSpace(Guids))
            //{
            //    string[] arr = Guids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //    if (arr != null && arr.Count() > 0)
            //    {
            //        foreach (var item in arr)
            //        {
            //            Guid temp;
            //            string item1 = item.TrimStart('\'').TrimEnd('\'');
            //            if (Guid.TryParse(item1, out temp))
            //            {
            //                lstGuids.Add(temp);
            //            }
            //        }
            //    }
            //}

            List<PullMasterViewDTO> ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //IQueryable<PullMasterViewDTO> qry = (from u in context.PullMasters
                //                                     join im in context.ItemMasters on u.ItemGUID equals im.GUID

                //                                     join projm in context.ProjectMasters on u.ProjectSpendGUID equals projm.GUID into u_projm_join
                //                                     from u_projm in u_projm_join.DefaultIfEmpty()

                //                                     join rm in context.Rooms on u.Room equals rm.ID into u_rm_join
                //                                     from u_rm in u_rm_join.DefaultIfEmpty()

                //                                     join um in context.UserMasters on u.CreatedBy equals um.ID into u_UMC_join
                //                                     from u_UMC in u_UMC_join.DefaultIfEmpty()

                //                                     join umU in context.UserMasters on u.LastUpdatedBy equals umU.ID into u_UMU_join
                //                                     from u_UMU in u_UMU_join.DefaultIfEmpty()

                //                                     join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
                //                                     from im_CM in im_CM_join.DefaultIfEmpty()

                //                                     join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
                //                                     from im_UNM in im_UNM_join.DefaultIfEmpty()

                //                                     join bm in context.BinMasters on u.BinID equals bm.ID into u_bm_join
                //                                     from u_bm in u_bm_join.DefaultIfEmpty()

                //                                     join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
                //                                     from im_mm in im_mm_join.DefaultIfEmpty()

                //                                     join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                //                                     from im_sm in im_sm_join.DefaultIfEmpty()

                //                                     join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
                //                                     from im_gla in im_gla_join.DefaultIfEmpty()

                //                                     join rd in context.RequisitionDetails on u.RequisitionDetailGUID equals rd.GUID into u_rd_join
                //                                     from u_rd in u_rd_join.DefaultIfEmpty()

                //                                     join reqm in context.RequisitionMasters on u_rd.RequisitionGUID equals reqm.GUID into rd_reqm_join
                //                                     from rd_reqm in rd_reqm_join.DefaultIfEmpty()
                //                                     where lstGuids.Contains(u.GUID)
                var params1 = new SqlParameter[] { new SqlParameter("@PullGUID", Guids ?? (object)DBNull.Value) };
                List<PullMasterViewDTO> qry = (from u in context.ExecuteStoreQuery<PullMasterViewDTO>("exec [GetPullByGuIDs] @PullGUID ", params1)
                                               select new PullMasterViewDTO()
                                               {
                                                   ID = u.ID,
                                                   ProjectSpendGUID = u.ProjectSpendGUID,
                                                   ProjectName = u.ProjectSpendName,
                                                   ProjectSpendName = u.ProjectSpendName,
                                                   BinNumber = u.BinNumber,
                                                   DefaultPullQuantity = u.DefaultPullQuantity,
                                                   OnHandQuantity = u.OnHandQuantity,
                                                   ItemGUID = u.ItemGUID,
                                                   ItemNumber = u.ItemNumber,
                                                   CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                   ConsignedQuantity = u.ConsignedQuantity,
                                                   CreditConsignedQuantity = u.CreditConsignedQuantity,
                                                   CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity,
                                                   PoolQuantity = u.PoolQuantity,
                                                   PullCost = u.PullCost,
                                                   //SerialNumber = im.SerialNumber,
                                                   //LotNumber = u.LotNumber,
                                                   //DateCode = u.DateCode,
                                                   BinID = u.BinID,
                                                   UDF1 = u.UDF1,
                                                   UDF2 = u.UDF2,
                                                   UDF3 = u.UDF3,
                                                   UDF4 = u.UDF4,
                                                   UDF5 = u.UDF5,
                                                   ItemUDF1 = u.ItemUDF1,
                                                   ItemUDF2 = u.ItemUDF2,
                                                   ItemUDF3 = u.ItemUDF3,
                                                   ItemUDF4 = u.ItemUDF4,
                                                   ItemUDF5 = u.ItemUDF5,
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
                                                   SupplierID = u.SupplierID,
                                                   ManufacturerID = u.ManufacturerID,
                                                   CategoryID = u.CategoryID,
                                                   CategoryName = u.CategoryName,
                                                   Markup = u.Markup,
                                                   SellPrice = u.SellPrice,
                                                   ItemCost = u.ItemCost,
                                                   Description = u.Description,
                                                   Unit = u.Unit,
                                                   PullCredit = u.PullCredit,
                                                   ActionType = u.ActionType,
                                                   ItemType = u.ItemType,
                                                   RequisitionDetailGUID = u.RequisitionDetailGUID,
                                                   //WorkOrderDetailID = u.WorkOrderDetailID,
                                                   WorkOrderDetailGUID = u.WorkOrderDetailGUID,
                                                   Billing = u.Billing,
                                                   CountLineItemGuid = u.CountLineItemGuid,
                                                   PackingQuantity = u.PackingQuantity,
                                                   Manufacturer = u.Manufacturer,
                                                   ManufacturerNumber = u.ManufacturerNumber,
                                                   SupplierName = u.SupplierName,
                                                   SupplierPartNo = u.SupplierPartNo,
                                                   AverageUsage = u.AverageUsage,
                                                   CriticalQuantity = u.CriticalQuantity,
                                                   GLAccount = u.GLAccount,
                                                   InTransitquantity = u.InTransitquantity,
                                                   IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                                   LongDescription = u.LongDescription,
                                                   MaximumQuantity = u.MaximumQuantity,
                                                   MinimumQuantity = u.MinimumQuantity,
                                                   Turns = u.Turns,
                                                   Taxable = u.Taxable,
                                                   OnOrderQuantity = u.OnOrderQuantity,
                                                   OnTransferQuantity = u.OnTransferQuantity,
                                                   Consignment = u.Consignment,
                                                   ItemOnhandQty = u.ItemOnhandQty,
                                                   IsAddedFromPDA = u.IsAddedFromPDA,
                                                   IsProcessedAfterSync = u.IsProcessedAfterSync,
                                                   ItemStageQty = u.ItemStageQty,
                                                   ItemStageLocationQty = u.ItemStageLocationQty,
                                                   ItemLocationOnHandQty = u.ItemLocationOnHandQty,
                                                   ReceivedOn = u.ReceivedOn,
                                                   ReceivedOnWeb = u.ReceivedOnWeb,
                                                   AddedFrom = u.AddedFrom,
                                                   EditedFrom = u.EditedFrom,
                                                   PullOrderNumber = u.PullOrderNumber,
                                                   ControlNumber = u.ControlNumber,
                                                   PullPrice = u.PullPrice
                                               }).ToList();
                //var sql = ((System.Data.Objects.ObjectQuery)qry).ToTraceString();
                ObjCache = qry.ToList();
            }
            return ObjCache.ToList();

        }

        #region Commented

        //public IEnumerable<PullMasterViewDTO> GetCachedDataForSerivce(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ConnectionString)
        //{
        //    IEnumerable<PullMasterViewDTO> ObjCache;
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        ObjCache = (from u in context.ExecuteStoreQuery<PullMasterViewDTO>(@"SELECT A.*,
        //                            B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,P.ProjectSpendName,
        //                            I.ItemNumber,I.DefaultPullQuantity,I.OnHandQuantity,I.CategoryID,I.ManufacturerID,
        //                            I.SupplierID,I.ItemType,BM.BinNumber,
        //                            C1.Category AS CategoryName,U1.Unit,I.Markup,I.SellPrice,I.Description,  
        //                            I.Cost As ItemCost
        //                            FROM PullMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
        //                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                            LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                            LEFT OUTER  JOIN ProjectMaster P on P.GUID = A.ProjectSpendGUID 
        //                            LEFT OUTER  JOIN ItemMaster I on I.GUID = A.ItemGUID
        //                            LEFT OUTER join CategoryMaster C1 on C1.id = I.CategoryID
        //                            LEFT OUTER join UnitMaster U1 on U1.id = I.UOMID 
        //                            LEFT OUTER  JOIN BinMaster BM on BM.ID = A.BinID 
        //                            WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
        //                    select new PullMasterViewDTO
        //                    {
        //                        ID = u.ID,
        //                        ProjectSpendGUID = u.ProjectSpendGUID,
        //                        ProjectName = u.ProjectName,
        //                        ProjectSpendName = u.ProjectSpendName,
        //                        BinNumber = u.BinNumber,
        //                        DefaultPullQuantity = u.DefaultPullQuantity.GetValueOrDefault(0),//Math.Round(u.DefaultPullQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
        //                        OnHandQuantity = u.OnHandQuantity.GetValueOrDefault(0),// Math.Round(u.OnHandQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
        //                        ItemGUID = u.ItemGUID,
        //                        ItemNumber = u.ItemNumber,
        //                        CustomerOwnedQuantity = u.CustomerOwnedQuantity.GetValueOrDefault(0),//Math.Round(u.CustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
        //                        ConsignedQuantity = u.ConsignedQuantity.GetValueOrDefault(0),//Math.Round(u.ConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
        //                        CreditConsignedQuantity = u.CreditConsignedQuantity.GetValueOrDefault(0),// Math.Round(u.CreditConsignedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
        //                        CreditCustomerOwnedQuantity = u.CreditCustomerOwnedQuantity.GetValueOrDefault(0),// Math.Round(u.CreditCustomerOwnedQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
        //                        PoolQuantity = u.PoolQuantity.GetValueOrDefault(0),//Math.Round(u.PoolQuantity.GetValueOrDefault(0), objRegionInfo.NumberDecimalDigits),
        //                        PullCost = u.PullCost.GetValueOrDefault(0),// Math.Round(u.PullCost.GetValueOrDefault(0), objRegionInfo.CurrencyDecimalDigits),
        //                        //PullCost = u.PullCost,
        //                        SerialNumber = u.SerialNumber,
        //                        LotNumber = u.LotNumber,
        //                        DateCode = u.DateCode,
        //                        BinID = u.BinID,
        //                        UDF1 = u.UDF1,
        //                        UDF2 = u.UDF2,
        //                        UDF3 = u.UDF3,
        //                        UDF4 = u.UDF4,
        //                        UDF5 = u.UDF5,
        //                        GUID = u.GUID,
        //                        Created = u.Created,
        //                        Updated = u.Updated,
        //                        CreatedBy = u.CreatedBy,
        //                        LastUpdatedBy = u.LastUpdatedBy,
        //                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                        CompanyID = u.CompanyID,
        //                        Room = u.Room,
        //                        CreatedByName = u.CreatedByName,
        //                        UpdatedByName = u.UpdatedByName,
        //                        RoomName = u.RoomName,
        //                        SupplierID = u.SupplierID,
        //                        ManufacturerID = u.ManufacturerID,
        //                        CategoryID = u.CategoryID,
        //                        CategoryName = u.CategoryName,
        //                        Markup = u.Markup,
        //                        SellPrice = u.SellPrice,
        //                        ItemCost = u.ItemCost,
        //                        Description = u.Description,
        //                        Unit = u.Unit,
        //                        PullCredit = u.PullCredit,
        //                        ActionType = u.ActionType,
        //                        ItemType = u.ItemType,
        //                        RequisitionDetailGUID = u.RequisitionDetailGUID,
        //                        //WorkOrderDetailID = u.WorkOrderDetailID,
        //                        WorkOrderDetailGUID = u.WorkOrderDetailGUID,
        //                        CountLineItemGuid = u.CountLineItemGuid,
        //                        ReceivedOn = u.ReceivedOn,
        //                        ReceivedOnWeb = u.ReceivedOnWeb,
        //                        AddedFrom = u.AddedFrom,
        //                        EditedFrom = u.EditedFrom,
        //                        ControlNumber = u.ControlNumber,
        //                        PullPrice = u.PullPrice.GetValueOrDefault(0)
        //                    }).AsParallel().ToList();
        //    }
        //    return ObjCache.Where(t => t.Room == RoomID);
        //}

        //public IEnumerable<PullMasterViewDTO> GetAllRecordsForService(Int64 RoomID, Int64 CompanyId, string ConnectionString)
        //{
        //    return GetCachedDataForSerivce(RoomID, CompanyId, false, false, ConnectionString).OrderBy("ID DESC");
        //}

        //public PullMasterViewDTO GetRecordForService(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived, string ConnectionString)
        //{
        //    return GetCachedDataForSerivce(RoomID, CompanyID, IsArchived, IsDeleted, ConnectionString).SingleOrDefault(t => t.GUID == GUID);
        //}



        //public bool UpdatePullDataForService(PullMasterViewDTO objDTO, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, out string ItemLocationMSG, bool IsProjectSpendAllowed, out bool IsPSLimitExceed)
        //{
        //    #region "Global Variables"
        //    ItemLocationMSG = "";
        //    IsPSLimitExceed = false;
        //    Int64 TempOldBinID = 0;
        //    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
        //    ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
        //    ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
        //    ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
        //    ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
        //    PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
        //    PullMasterViewDTO obj = new PullMasterViewDTO();
        //    ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
        //    BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
        //    ItemLocationQTYDTO lstLocDTO1 = new ItemLocationQTYDTO();
        //    BinMasterDTO objBINDTO = new BinMasterDTO();
        //    List<ItemLocationDetailsDTO> ObjItemLocation = null;
        //    PullMasterViewDTO ReturnDto = null;
        //    RoomDTO objRoomDTO = new RoomDTO();
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);

        //    bool IsStagginLocation = false;
        //    //bool IsProjectSpendMandatoryPleaseSelect = false;

        //    #endregion
        //    ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

        //    objRoomDTO = objRoomDAL.GetRecord(RoomID, CompanyID, false, false);

        //    //if (ItemDTO != null && ItemDTO.ItemType != 4)
        //    //{
        //    //    if (objRoomDTO != null)
        //    //    {
        //    //        if (objRoomDTO.IsProjectSpendMandatory)
        //    //        {
        //    //            if (objDTO.ProjectSpendGUID == null)
        //    //            {
        //    //                IsProjectSpendMandatoryPleaseSelect = true;
        //    //                ItemLocationMSG = "Project Spend is Mandatory, Please select it.";
        //    //                return false;
        //    //            }
        //    //        }
        //    //    }
        //    //}


        //    BinMasterDAL objLocDAL = new BinMasterDAL(base.DataBaseName);
        //    BinMasterDTO objLocDTO = objLocDAL.GetRecord(objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID, false, false);
        //    //BinMasterDTO objLocDTO = objLocDAL.GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, objDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
        //    if (objLocDTO != null && objLocDTO.ID > 0)
        //    {
        //        if (objLocDTO.IsStagingLocation)
        //        {
        //            IsStagginLocation = true;
        //        }
        //    }

        //    if (ItemDTO != null && ItemDTO.ItemType == 4)
        //    {
        //        #region "Pull Insert Update"
        //        ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //        ItemLocationDetailsDTO ObjTempItemLocation = new ItemLocationDetailsDTO();
        //        ObjTempItemLocation.CustomerOwnedQuantity = objDTO.TempPullQTY;
        //        ObjTempItemLocation.ItemGUID = objDTO.ItemGUID;
        //        ObjTempItemLocation.Room = objDTO.Room;
        //        ObjTempItemLocation.CompanyID = objDTO.CompanyID;
        //        AddtoPullDetail(ObjTempItemLocation, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), ItemDTO.Cost.GetValueOrDefault(0), obj.LastUpdatedBy, objDTO.TempPullQTY.GetValueOrDefault(0), 0, ItemDTO.SellPrice.GetValueOrDefault(0));
        //        #endregion

        //        #region "Project Spend Quantity Update"
        //        //obj.WhatWhereAction = "Project Spend";
        //        if (objPullDal.Edit(obj))
        //        {
        //            if (objDTO.ProjectSpendGUID != null)
        //            {
        //                UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //            }
        //        }
        //        #endregion
        //        // return true;
        //    }
        //    else
        //    {
        //        #region "LIFO FIFO"

        //        Boolean IsFIFO = false;
        //        if (objRoomDTO != null && objRoomDTO.ID > 0)
        //        {
        //            if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "fifo")
        //                IsFIFO = true;
        //            if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "lifo")
        //                IsFIFO = false;
        //        }
        //        else
        //        {
        //            IsFIFO = true;
        //        }

        //        #endregion

        //        #region "For Item Pull"
        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            //if staging location then check qty on staging module
        //            if (IsStagginLocation)
        //            {
        //                #region "Stagging Bin Wise Quantity Check"
        //                double retval = 0;
        //                var qry = (from msd in context.MaterialStagingDetails
        //                           join bm in context.BinMasters on msd.StagingBinID equals bm.ID
        //                           where msd.ItemGUID == ItemDTO.GUID && msd.IsArchived == false && msd.IsDeleted == false && bm.ID == objDTO.BinID
        //                           select msd);
        //                if (qry.Any())
        //                {
        //                    retval = qry.Sum(t => t.Quantity) ?? 0;
        //                }
        //                if (retval < objDTO.TempPullQTY)
        //                {
        //                    ItemLocationMSG = "Not Enough Quantity for Location ## " + objLocDTO.BinNumber + "(Avl.QTY=" + retval.ToString() + ")";
        //                    return true;
        //                }
        //                #endregion

        //            }
        //            else
        //            {
        //                #region "Bin Wise Quantity Check"

        //                lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).SingleOrDefault();
        //                lstLocDTO1 = objLocationDAL.GetItemQtyByLocation(objDTO.BinID ?? 0, objDTO.ItemGUID ?? Guid.Empty, RoomID, CompanyID, objDTO.CreatedBy ?? 0);
        //                if (lstLocDTO == null && lstLocDTO1 != null && lstLocDTO1.Quantity > 0)
        //                {
        //                    objLocQTY.Insert(lstLocDTO1);
        //                    lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).SingleOrDefault();
        //                }
        //                objBINDTO = null;// objBINDAL.GetRecord((Int64)objDTO.BinID, RoomID, CompanyID, false, false);

        //                if (BinWiseQuantityCheck(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO))
        //                {
        //                    return true;
        //                }
        //                #endregion
        //            }


        //            if (!IsStagginLocation)
        //            {

        //                //Pick up the locations.....
        //                if (ItemDTO.SerialNumberTracking)
        //                {
        //                    if (IsCreditPullNothing == 2) // pull
        //                    {
        //                        ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).Take((int)objDTO.TempPullQTY).ToList();
        //                    }

        //                }
        //                else
        //                {
        //                    ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).ToList();
        //                }



        //                #region "Project Wise Quantity Check"
        //                if (objDTO.ProjectSpendGUID != null && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
        //                {
        //                    objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID).SingleOrDefault();
        //                    objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, false, false);

        //                    if (ProjectWiseQuantityCheck(objPrjMsgDAL, objPrjSpenItmDTO, objPrjMstDTO, objPrjSpenItmDAL, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation))
        //                    {
        //                        return true;
        //                    }
        //                }
        //                #endregion

        //                #region "Pull Insert Update"
        //                if (ItemDTO.Consignment)
        //                {
        //                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, ItemDTO.SupplierID ?? 0, ItemDTO.GUID, null, false);
        //                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
        //                    {
        //                        objDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
        //                    }
        //                    else
        //                    {
        //                        DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
        //                        objDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
        //                    }
        //                }

        //                ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //                #endregion

        //                #region "Item Location & Quantity  Wise Deduction"

        //                #region "ItemLocation Deduction"
        //                if (IsCreditPullNothing != 3)
        //                {
        //                    if (ItemDTO.SerialNumberTracking)
        //                    {
        //                        #region "Serial logic"


        //                        double LocalSearilaCount = 0;
        //                        double TakenCreditCount = 0;
        //                        foreach (var itemoil in ObjItemLocation)
        //                        {
        //                            double loopCurrentTakenCustomer = 0;
        //                            double loopCurrentTakenConsignment = 0;

        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (ItemDTO.Consignment)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID).Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID).Where(t => (t.ConsignedQuantity.GetValueOrDefault(0) != t.CreditConsignedQuantity.GetValueOrDefault(0)) || (t.CustomerOwnedQuantity.GetValueOrDefault(0) != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0))).OrderByDescending(t => t.ID).Take(1).SingleOrDefault();

        //                                    if (result != null)
        //                                    {
        //                                        //double unSatalledDifferent = 0;
        //                                        if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            //unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
        //                                            if (TakenCreditCount != objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + 1;
        //                                                TakenCreditCount += 1;
        //                                            }
        //                                            itemoil.CustomerOwnedQuantity = 1;
        //                                            result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + 1;
        //                                        }
        //                                        else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            if (TakenCreditCount != objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
        //                                                TakenCreditCount += 1;
        //                                            }
        //                                            itemoil.ConsignedQuantity = 1;
        //                                            result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
        //                                        }
        //                                        result.WhatWhereAction = "Consignment";
        //                                        objPullDal.Edit(result);
        //                                    }

        //                                    //get last pull record to know the quantity....
        //                                    itemoil.ConsignedQuantity = 1;//itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY;
        //                                }
        //                                if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //loopCurrentTaken = 1;

        //                                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                                    {
        //                                        loopCurrentTakenConsignment = 1;
        //                                        itemoil.ConsignedQuantity = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        loopCurrentTakenCustomer = 1;
        //                                        itemoil.CustomerOwnedQuantity = 0;
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                            else //customerowendQuantity
        //                            {
        //                                #region "customerowned Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    if (LocalSearilaCount != objDTO.TempPullQTY)
        //                                    {
        //                                        LocalSearilaCount += 1;
        //                                        itemoil.CustomerOwnedQuantity = 1;//itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY;
        //                                    }
        //                                    else
        //                                    {
        //                                        objLocationDAL.Edit(itemoil);
        //                                        break;
        //                                    }
        //                                }
        //                                else if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //loopCurrentTaken = 1;
        //                                    loopCurrentTakenCustomer = 1;
        //                                    itemoil.CustomerOwnedQuantity = 0;
        //                                }
        //                                #endregion
        //                            }
        //                            objLocationDAL.Edit(itemoil);
        //                            //AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.Value, ItemDTO.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), itemoil.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemoil.SellPrice);

        //                        }
        //                        obj.CreditCustomerOwnedQuantity = LocalSearilaCount;
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region "LOt and other type logic"

        //                        Double takenQunatity = 0;
        //                        foreach (var itemoil in ObjItemLocation)
        //                        {
        //                            Double loopCurrentTakenCustomer = 0;
        //                            Double loopCurrentTakenConsignment = 0;
        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (takenQunatity == objDTO.TempPullQTY)
        //                            {
        //                                break;
        //                            }

        //                            if (ItemDTO.Consignment)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID)
        //                                                               .Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID)
        //                                                               .Where(t => (t.ConsignedQuantity.GetValueOrDefault(0)
        //                                                                    != t.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                                                    || (t.CustomerOwnedQuantity.GetValueOrDefault(0)
        //                                                                        != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0)))
        //                                                                .OrderByDescending(t => t.ID).Take(1).SingleOrDefault();
        //                                    if (result != null)
        //                                    {
        //                                        double unSatalledDifferent = 0;
        //                                        if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + qtyavailable;
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }

        //                                        }
        //                                        else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.ConsignedQuantity.GetValueOrDefault(0) - result.CreditConsignedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + qtyavailable;
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }
        //                                        }
        //                                        result.WhatWhereAction = "Consignment";
        //                                        objPullDal.Edit(result);
        //                                    }
        //                                }
        //                                if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //Both's sum we have available.
        //                                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        itemoil.ConsignedQuantity = (Double)(itemoil.ConsignedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        goto Save;
        //                                    }
        //                                    else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
        //                                        // needs to write logic for break down deduction from consigned or customer quantity location wise ...
        //                                        if (itemoil.ConsignedQuantity >= ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity))
        //                                        {
        //                                            //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                            loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                            itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                            takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
        //                                            goto Save;
        //                                        }
        //                                        else
        //                                        {
        //                                            //loopCurrentTaken = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            itemoil.ConsignedQuantity = 0;
        //                                        }
        //                                        //PENDING -- loop by varialbe from mupliple locations...
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                            else
        //                            {
        //                                #region "Customreowned Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit - customer owened - lot number
        //                                {

        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID)
        //                                        .Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID)
        //                                        .Where(t => (t.ConsignedQuantity.GetValueOrDefault(0)
        //                                                != t.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        || (t.CustomerOwnedQuantity.GetValueOrDefault(0)
        //                                                != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0)))
        //                                        .OrderByDescending(t => t.ID).Take(1).SingleOrDefault();

        //                                    if (result != null)
        //                                    {
        //                                        double unSatalledDifferent = 0;
        //                                        if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + qtyavailable;
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }

        //                                        }
        //                                        else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.ConsignedQuantity.GetValueOrDefault(0) - result.CreditConsignedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + qtyavailable;
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }
        //                                        }
        //                                        result.WhatWhereAction = "Customreowned";
        //                                        objPullDal.Edit(result);
        //                                    }

        //                                }
        //                                else if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //Both's sum we have available.
        //                                    if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        itemoil.CustomerOwnedQuantity = (Double)(itemoil.CustomerOwnedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                        Save:
        //                            objLocationDAL.Edit(itemoil);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID, itemoil.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemoil.SellPrice);

        //                        }
        //                        #endregion
        //                    }
        //                }
        //                #endregion

        //                #region "ItemLocation Quantity Deduction"
        //                if (IsCreditPullNothing == 1) // credit
        //                {
        //                    if (!ItemDTO.Consignment)
        //                    {
        //                        ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
        //                        lstLocDTO.CustomerOwnedQuantity = (Double)lstLocDTO.CustomerOwnedQuantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                    }
        //                    else
        //                    {
        //                        /// PENDING
        //                        //get last pull record to know the quantity....Credit Unsetteled records



        //                        obj.CreditCustomerOwnedQuantity = 0;
        //                        obj.CreditConsignedQuantity = 0;


        //                        ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
        //                        lstLocDTO.ConsignedQuantity = (Double)lstLocDTO.ConsignedQuantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                    }
        //                }
        //                else if (IsCreditPullNothing == 2) // pull
        //                {
        //                    ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - objDTO.TempPullQTY;
        //                    if (ItemDTO.Consignment)
        //                    {
        //                        //Both's sum we have available.
        //                        if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                        {
        //                            obj.ConsignedQuantity = (Double)(objDTO.TempPullQTY ?? 0);
        //                            lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - objDTO.TempPullQTY;
        //                            lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY ?? 0);
        //                        }
        //                        else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= (Double)(objDTO.TempPullQTY ?? 0))
        //                        {
        //                            obj.CustomerOwnedQuantity = (Double)(objDTO.TempPullQTY ?? 0);
        //                            lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - (Double)(objDTO.TempPullQTY ?? 0);
        //                            lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY ?? 0);
        //                        }
        //                        else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < (Double)(objDTO.TempPullQTY ?? 0))
        //                        {
        //                            Double cstqty = (Double)(objDTO.TempPullQTY ?? 0) - (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); // -((Double)(objDTO.TempPullQTY??0) - cstqty);
        //                            Double consqty = cstqty;

        //                            lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
        //                            obj.CustomerOwnedQuantity = (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                            obj.ConsignedQuantity = consqty;
        //                            lstLocDTO.CustomerOwnedQuantity = 0;
        //                            lstLocDTO.Quantity = lstLocDTO.Quantity - (obj.CustomerOwnedQuantity.GetValueOrDefault(0) + obj.ConsignedQuantity.GetValueOrDefault(0));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objDTO.TempPullQTY;
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY ?? 0);

        //                        obj.CustomerOwnedQuantity = (Double)(objDTO.TempPullQTY ?? 0);
        //                    }
        //                }


        //                #endregion

        //                #region "Saving Location and QTY data"
        //                // update requisition qty

        //                if (objDTO.RequisitionDetailGUID != null && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //                {
        //                    //if (ItemDTO.RequisitionedQuantity != null && ItemDTO.RequisitionedQuantity.GetValueOrDefault(0) > 0)
        //                    //  ItemDTO.RequisitionedQuantity = ItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - objDTO.TempPullQTY.GetValueOrDefault(0);
        //                    new RequisitionDetailsDAL(base.DataBaseName).UpdateItemOnRequisitionQty(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0));

        //                }
        //                ItemDTO.WhatWhereAction = "Pull";
        //                objItemDAL.Edit(ItemDTO);


        //                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
        //                lstUpdate.Add(lstLocDTO);
        //                objLocQTY.Save(lstUpdate);
        //                #endregion

        //                #endregion

        //                #region "Project Spend Quantity Update"
        //                //obj.WhatWhereAction = "Project Spend";
        //                if (objPullDal.Edit(obj))
        //                {
        //                    if (objDTO.ProjectSpendGUID != null && objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //                    {
        //                        UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //                    }
        //                }
        //                #endregion

        //                #region "Update Turns and Average Usgae"
        //                UpdateTurnsAverageUsage(obj);
        //                #endregion
        //            }
        //            else
        //            {
        //                List<MaterialStagingPullDetailDTO> ObjItemLocationMS = null;
        //                MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
        //                MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
        //                List<MaterialStagingDetailDTO> lstLocDTOMS = new List<MaterialStagingDetailDTO>();

        //                List<MaterialStagingPullDetailDTO> lstPullDetailsDTO = objMaterialStagingPullDetailDAL.GetMsPullDetailsByItemGUIDANDBinID(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID).ToList();


        //                //Pick up the locations.....
        //                if (ItemDTO.SerialNumberTracking)
        //                {
        //                    ObjItemLocationMS = lstPullDetailsDTO.Take((int)objDTO.TempPullQTY).ToList();

        //                }
        //                else
        //                {
        //                    ObjItemLocationMS = lstPullDetailsDTO;
        //                }

        //                #region "Project Wise Quantity Check"
        //                if (objDTO.ProjectSpendGUID != null)
        //                {
        //                    objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID).SingleOrDefault();
        //                    objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, false, false);

        //                    if (ProjectWiseQuantityCheck(objPrjMsgDAL, objPrjSpenItmDTO, objPrjMstDTO, objPrjSpenItmDAL, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation))
        //                    {
        //                        return true;
        //                    }
        //                }
        //                #endregion



        //                #region "Pull Insert Update"
        //                if (ItemDTO.Consignment)
        //                {
        //                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, ItemDTO.SupplierID ?? 0, ItemDTO.GUID, null, false);
        //                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
        //                    {
        //                        objDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
        //                    }
        //                    else
        //                    {
        //                        DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
        //                        objDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
        //                    }
        //                }

        //                ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //                #endregion


        //                #region "Item Location & Quantity  Wise Deduction"

        //                #region "ItemLocation Deduction"
        //                if (IsCreditPullNothing != 3)
        //                {
        //                    if (ItemDTO.SerialNumberTracking)
        //                    {
        //                        #region "Serial logic"


        //                        double LocalSearilaCount = 0;
        //                        //double TakenCreditCount = 0;
        //                        foreach (var itemoil in ObjItemLocationMS)
        //                        {
        //                            double loopCurrentTakenCustomer = 0;
        //                            double loopCurrentTakenConsignment = 0;

        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (ItemDTO.Consignment)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                //loopCurrentTaken = 1;
        //                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                                {
        //                                    loopCurrentTakenConsignment = 1;
        //                                    itemoil.ConsignedQuantity = 0;
        //                                }
        //                                else
        //                                {
        //                                    loopCurrentTakenCustomer = 1;
        //                                    itemoil.CustomerOwnedQuantity = 0;
        //                                }

        //                                #endregion
        //                            }
        //                            else //customerowendQuantity
        //                            {
        //                                //loopCurrentTaken = 1;
        //                                loopCurrentTakenCustomer = 1;
        //                                itemoil.CustomerOwnedQuantity = 0;
        //                            }
        //                            objMaterialStagingPullDetailDAL.Edit(itemoil);
        //                            //AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.Value, ItemDTO.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), itemoil.ItemCost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemoil.ItemCost);

        //                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
        //                            objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
        //                            objMaterialStagingDetailDAL.Edit(objmsddto);
        //                        }
        //                        obj.CreditCustomerOwnedQuantity = LocalSearilaCount;



        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region "LOt and other type logic"

        //                        Double takenQunatity = 0;
        //                        foreach (var itemoil in ObjItemLocationMS)
        //                        {
        //                            Double loopCurrentTakenCustomer = 0;
        //                            Double loopCurrentTakenConsignment = 0;
        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (takenQunatity == objDTO.TempPullQTY)
        //                            {
        //                                break;
        //                            }

        //                            if (ItemDTO.Consignment)
        //                            {
        //                                #region "Consignment Credit and Pull"

        //                                //Both's sum we have available.
        //                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    itemoil.ConsignedQuantity = (Double)(itemoil.ConsignedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    goto Save;
        //                                }
        //                                else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
        //                                    goto Save;
        //                                }
        //                                else
        //                                {
        //                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
        //                                    // needs to write logic for break down deduction from consigned or customer quantity location wise ...
        //                                    if (itemoil.ConsignedQuantity >= ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        //loopCurrentTaken = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        itemoil.ConsignedQuantity = 0;
        //                                    }
        //                                    //PENDING -- loop by varialbe from mupliple locations...
        //                                }

        //                                #endregion
        //                            }
        //                            else
        //                            {
        //                                #region "Customreowned Credit and Pull"

        //                                //Both's sum we have available.
        //                                if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    itemoil.CustomerOwnedQuantity = (Double)(itemoil.CustomerOwnedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    goto Save;
        //                                }
        //                                else
        //                                {
        //                                    //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
        //                                }

        //                                #endregion
        //                            }
        //                        Save:
        //                            objMaterialStagingPullDetailDAL.Edit(itemoil);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), Convert.ToDouble(itemoil.ItemCost.GetValueOrDefault(0)), obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, Convert.ToDouble(itemoil.ItemCost.GetValueOrDefault(0)));

        //                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
        //                            if (objmsddto != null)
        //                            {
        //                                objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
        //                                objMaterialStagingDetailDAL.Edit(objmsddto);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                #endregion
        //                ItemDTO.WhatWhereAction = "Pull";
        //                objItemDAL.Edit(ItemDTO);


        //                #endregion

        //                //Update started quantity...
        //                objMaterialStagingPullDetailDAL.UpdateStagedQuantity(ItemDTO.GUID, RoomID, CompanyID);


        //                //Updated PS
        //                #region "Project Spend Quantity Update"
        //                //obj.WhatWhereAction = "Project Spend";
        //                if (objPullDal.Edit(obj))
        //                {
        //                    if (objDTO.ProjectSpendGUID != null && objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //                    {
        //                        UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //                    }
        //                }
        //                #endregion

        //            }
        //            //return true;
        //        }
        //        #endregion
        //    }

        //    if (objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //    {
        //        WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
        //        objWOLDAL.UpdateWOItemAndTotalCost(objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
        //    }
        //    UpdateCumulativeOnHand(ReturnDto);
        //    return true;
        //}

        //public bool UpdatePullDataForServiceNew(PullMasterViewDTO objDTO, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, out string ItemLocationMSG, bool IsProjectSpendAllowed, out bool IsPSLimitExceed, bool IsConsingedPull)
        //{
        //    #region "Global Variables"
        //    ItemLocationMSG = "";
        //    IsPSLimitExceed = false;
        //    Int64 TempOldBinID = 0;
        //    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
        //    ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
        //    ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
        //    ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
        //    ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
        //    PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
        //    PullMasterViewDTO obj = new PullMasterViewDTO();
        //    ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
        //    BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
        //    ItemLocationQTYDTO lstLocDTO1 = new ItemLocationQTYDTO();
        //    BinMasterDTO objBINDTO = new BinMasterDTO();
        //    List<ItemLocationDetailsDTO> ObjItemLocation = null;
        //    PullMasterViewDTO ReturnDto = null;
        //    RoomDTO objRoomDTO = new RoomDTO();
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);

        //    bool IsStagginLocation = false;
        //    //bool IsProjectSpendMandatoryPleaseSelect = false;

        //    #endregion
        //    ItemMasterDTO ItemDTO = objItemDAL.GetRecord(objDTO.ItemGUID.ToString(), RoomID, CompanyID);

        //    objRoomDTO = objRoomDAL.GetRecord(RoomID, CompanyID, false, false);

        //    BinMasterDAL objLocDAL = new BinMasterDAL(base.DataBaseName);
        //    BinMasterDTO objLocDTO = objLocDAL.GetRecord(objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID, false, false);
        //    if (objLocDTO != null && objLocDTO.ID > 0)
        //    {
        //        if (objLocDTO.IsStagingLocation)
        //        {
        //            IsStagginLocation = true;
        //        }
        //    }

        //    if (ItemDTO != null && ItemDTO.ItemType == 4)
        //    {
        //        #region "Pull Insert Update"
        //        ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //        ItemLocationDetailsDTO ObjTempItemLocation = new ItemLocationDetailsDTO();
        //        if (IsConsingedPull)
        //            ObjTempItemLocation.ConsignedQuantity = objDTO.TempPullQTY;
        //        else
        //            ObjTempItemLocation.CustomerOwnedQuantity = objDTO.TempPullQTY;

        //        ObjTempItemLocation.ItemGUID = objDTO.ItemGUID;
        //        ObjTempItemLocation.Room = objDTO.Room;
        //        ObjTempItemLocation.CompanyID = objDTO.CompanyID;
        //        AddtoPullDetail(ObjTempItemLocation, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), ItemDTO.Cost.GetValueOrDefault(0), obj.LastUpdatedBy, objDTO.TempPullQTY.GetValueOrDefault(0), 0);
        //        #endregion

        //        #region "Project Spend Quantity Update"
        //        //obj.WhatWhereAction = "Project Spend";
        //        if (objPullDal.Edit(obj))
        //        {
        //            if (objDTO.ProjectSpendGUID != null)
        //            {
        //                UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //            }
        //        }
        //        #endregion
        //        // return true;
        //    }
        //    else
        //    {
        //        #region "LIFO FIFO"

        //        Boolean IsFIFO = false;
        //        if (objRoomDTO != null && objRoomDTO.ID > 0)
        //        {
        //            if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "fifo")
        //                IsFIFO = true;
        //            if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "lifo")
        //                IsFIFO = false;
        //        }
        //        else
        //        {
        //            IsFIFO = true;
        //        }

        //        #endregion

        //        #region "For Item Pull"
        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            //if staging location then check qty on staging module
        //            if (IsStagginLocation)
        //            {
        //                #region "Stagging Bin Wise Quantity Check"
        //                double retval = 0;
        //                var qry = (from msd in context.MaterialStagingDetails
        //                           join bm in context.BinMasters on msd.StagingBinID equals bm.ID
        //                           where msd.ItemGUID == ItemDTO.GUID && msd.IsArchived == false && msd.IsDeleted == false && bm.ID == objDTO.BinID
        //                           select msd);
        //                if (qry.Any())
        //                {
        //                    retval = qry.Sum(t => t.Quantity) ?? 0;
        //                }
        //                if (retval < objDTO.TempPullQTY)
        //                {
        //                    ItemLocationMSG = "Not Enough Quantity for Location ## " + objLocDTO.BinNumber + "(Avl.QTY=" + retval.ToString() + ")";
        //                    return true;
        //                }
        //                #endregion

        //            }
        //            else
        //            {
        //                #region "Bin Wise Quantity Check"

        //                lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).SingleOrDefault();
        //                lstLocDTO1 = objLocationDAL.GetItemQtyByLocation(objDTO.BinID ?? 0, objDTO.ItemGUID ?? Guid.Empty, RoomID, CompanyID, objDTO.CreatedBy ?? 0);
        //                if (lstLocDTO == null && lstLocDTO1 != null && lstLocDTO1.Quantity > 0)
        //                {
        //                    objLocQTY.Insert(lstLocDTO1);
        //                    lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).SingleOrDefault();
        //                }
        //                objBINDTO = null;// objBINDAL.GetRecord((Int64)objDTO.BinID, RoomID, CompanyID, false, false);

        //                if (BinWiseQuantityCheckNew(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsConsingedPull))
        //                {
        //                    return true;
        //                }
        //                #endregion
        //            }


        //            if (!IsStagginLocation)
        //            {

        //                //Pick up the locations.....
        //                if (ItemDTO.SerialNumberTracking)
        //                {
        //                    if (IsCreditPullNothing == 2) // pull
        //                    {
        //                        ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).Take((int)objDTO.TempPullQTY).ToList();
        //                    }

        //                }
        //                else
        //                {
        //                    ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).ToList();
        //                }



        //                #region "Project Wise Quantity Check"
        //                if (objDTO.ProjectSpendGUID != null && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
        //                {
        //                    objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID).SingleOrDefault();
        //                    objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, false, false);

        //                    if (ProjectWiseQuantityCheckNew(objPrjMsgDAL, objPrjSpenItmDTO, objPrjMstDTO, objPrjSpenItmDAL, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, IsConsingedPull))
        //                    {
        //                        return true;
        //                    }
        //                }
        //                #endregion

        //                #region "Pull Insert Update"
        //                ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //                #endregion

        //                #region "Item Location & Quantity  Wise Deduction"

        //                #region "ItemLocation Deduction"
        //                if (IsCreditPullNothing != 3)
        //                {
        //                    if (ItemDTO.SerialNumberTracking)
        //                    {
        //                        #region "Serial logic"


        //                        double LocalSearilaCount = 0;
        //                        double TakenCreditCount = 0;
        //                        foreach (var itemoil in ObjItemLocation)
        //                        {
        //                            double loopCurrentTakenCustomer = 0;
        //                            double loopCurrentTakenConsignment = 0;

        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (IsConsingedPull)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID).Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID).Where(t => (t.ConsignedQuantity.GetValueOrDefault(0) != t.CreditConsignedQuantity.GetValueOrDefault(0)) || (t.CustomerOwnedQuantity.GetValueOrDefault(0) != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0))).OrderByDescending(t => t.ID).Take(1).SingleOrDefault();

        //                                    if (result != null)
        //                                    {
        //                                        //double unSatalledDifferent = 0;
        //                                        if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            //unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
        //                                            if (TakenCreditCount != objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + 1;
        //                                                TakenCreditCount += 1;
        //                                            }
        //                                            itemoil.CustomerOwnedQuantity = 1;
        //                                            result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + 1;
        //                                        }
        //                                        else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            if (TakenCreditCount != objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
        //                                                TakenCreditCount += 1;
        //                                            }
        //                                            itemoil.ConsignedQuantity = 1;
        //                                            result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
        //                                        }
        //                                        result.WhatWhereAction = "Consignment";
        //                                        objPullDal.Edit(result);
        //                                    }

        //                                    //get last pull record to know the quantity....
        //                                    itemoil.ConsignedQuantity = 1;//itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY;
        //                                }
        //                                if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //loopCurrentTaken = 1;

        //                                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                                    {
        //                                        loopCurrentTakenConsignment = 1;
        //                                        itemoil.ConsignedQuantity = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        loopCurrentTakenCustomer = 1;
        //                                        itemoil.CustomerOwnedQuantity = 0;
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                            else //customerowendQuantity
        //                            {
        //                                #region "customerowned Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    if (LocalSearilaCount != objDTO.TempPullQTY)
        //                                    {
        //                                        LocalSearilaCount += 1;
        //                                        itemoil.CustomerOwnedQuantity = 1;//itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY;
        //                                    }
        //                                    else
        //                                    {
        //                                        objLocationDAL.Edit(itemoil);
        //                                        break;
        //                                    }
        //                                }
        //                                else if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //loopCurrentTaken = 1;
        //                                    loopCurrentTakenCustomer = 1;
        //                                    itemoil.CustomerOwnedQuantity = 0;
        //                                }
        //                                #endregion
        //                            }
        //                            objLocationDAL.Edit(itemoil);
        //                            //AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.Value, ItemDTO.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), itemoil.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);

        //                        }
        //                        obj.CreditCustomerOwnedQuantity = LocalSearilaCount;
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region "LOt and other type logic"

        //                        Double takenQunatity = 0;
        //                        foreach (var itemoil in ObjItemLocation)
        //                        {
        //                            Double loopCurrentTakenCustomer = 0;
        //                            Double loopCurrentTakenConsignment = 0;
        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (takenQunatity == objDTO.TempPullQTY)
        //                            {
        //                                break;
        //                            }

        //                            if (IsConsingedPull)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID)
        //                                                               .Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID)
        //                                                               .Where(t => (t.ConsignedQuantity.GetValueOrDefault(0)
        //                                                                    != t.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                                                    || (t.CustomerOwnedQuantity.GetValueOrDefault(0)
        //                                                                        != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0)))
        //                                                                .OrderByDescending(t => t.ID).Take(1).SingleOrDefault();
        //                                    if (result != null)
        //                                    {
        //                                        double unSatalledDifferent = 0;
        //                                        if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + qtyavailable;
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }

        //                                        }
        //                                        else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.ConsignedQuantity.GetValueOrDefault(0) - result.CreditConsignedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + qtyavailable;
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }
        //                                        }
        //                                        result.WhatWhereAction = "Consignment";
        //                                        objPullDal.Edit(result);
        //                                    }
        //                                }
        //                                if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //Both's sum we have available.
        //                                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        itemoil.ConsignedQuantity = (Double)(itemoil.ConsignedQuantity??0) - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        takenQunatity += ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        goto Save;
        //                                    }
        //                                    else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        takenQunatity += (Double)(objDTO.TempPullQTY??0) - takenQunatity;
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
        //                                        // needs to write logic for break down deduction from consigned or customer quantity location wise ...
        //                                        if (itemoil.ConsignedQuantity >= ((Double)(objDTO.TempPullQTY??0) - takenQunatity))
        //                                        {
        //                                            //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                            loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                            itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                            takenQunatity += (Double)(objDTO.TempPullQTY??0) - takenQunatity;
        //                                            goto Save;
        //                                        }
        //                                        else
        //                                        {
        //                                            //loopCurrentTaken = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            itemoil.ConsignedQuantity = 0;
        //                                        }
        //                                        //PENDING -- loop by varialbe from mupliple locations...
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                            else
        //                            {
        //                                #region "Customreowned Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit - customer owened - lot number
        //                                {

        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID)
        //                                        .Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID)
        //                                        .Where(t => (t.ConsignedQuantity.GetValueOrDefault(0)
        //                                                != t.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        || (t.CustomerOwnedQuantity.GetValueOrDefault(0)
        //                                                != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0)))
        //                                        .OrderByDescending(t => t.ID).Take(1).SingleOrDefault();

        //                                    if (result != null)
        //                                    {
        //                                        double unSatalledDifferent = 0;
        //                                        if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + qtyavailable;
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }

        //                                        }
        //                                        else if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.ConsignedQuantity.GetValueOrDefault(0) - result.CreditConsignedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + qtyavailable;
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }
        //                                        }
        //                                        result.WhatWhereAction = "Customreowned";
        //                                        objPullDal.Edit(result);
        //                                    }

        //                                }
        //                                else if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //Both's sum we have available.
        //                                    if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        takenQunatity += ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                        Save:
        //                            objLocationDAL.Edit(itemoil);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID, itemoil.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);

        //                        }
        //                        #endregion
        //                    }
        //                }
        //                #endregion

        //                #region "ItemLocation Quantity Deduction"
        //                if (IsCreditPullNothing == 1) // credit
        //                {
        //                    if (!IsConsingedPull)
        //                    {
        //                        ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
        //                        lstLocDTO.CustomerOwnedQuantity = (Double)lstLocDTO.CustomerOwnedQuantity + (Double)(objDTO.TempPullQTY??0);
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity + (Double)(objDTO.TempPullQTY??0);
        //                    }
        //                    else
        //                    {
        //                        obj.CreditCustomerOwnedQuantity = 0;
        //                        obj.CreditConsignedQuantity = 0;


        //                        ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
        //                        lstLocDTO.ConsignedQuantity = (Double)lstLocDTO.ConsignedQuantity + (Double)(objDTO.TempPullQTY??0);
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity + (Double)(objDTO.TempPullQTY??0);
        //                    }
        //                }
        //                else if (IsCreditPullNothing == 2) // pull
        //                {
        //                    ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - objDTO.TempPullQTY;
        //                    if (IsConsingedPull)
        //                    {
        //                        //Both's sum we have available.
        //                        if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                        {
        //                            obj.ConsignedQuantity = (Double)(objDTO.TempPullQTY??0);
        //                            lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - objDTO.TempPullQTY;
        //                            lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY??0);
        //                        }
        //                        else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= (Double)(objDTO.TempPullQTY??0))
        //                        {
        //                            obj.CustomerOwnedQuantity = (Double)(objDTO.TempPullQTY??0);
        //                            lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - (Double)(objDTO.TempPullQTY??0);
        //                            lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY??0);
        //                        }
        //                        else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < (Double)(objDTO.TempPullQTY??0))
        //                        {
        //                            Double cstqty = (Double)(objDTO.TempPullQTY??0) - (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); // -((Double)(objDTO.TempPullQTY??0) - cstqty);
        //                            Double consqty = cstqty;

        //                            lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
        //                            obj.CustomerOwnedQuantity = (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                            obj.ConsignedQuantity = consqty;
        //                            lstLocDTO.CustomerOwnedQuantity = 0;
        //                            lstLocDTO.Quantity = lstLocDTO.Quantity - (obj.CustomerOwnedQuantity.GetValueOrDefault(0) + obj.ConsignedQuantity.GetValueOrDefault(0));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objDTO.TempPullQTY;
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY??0);

        //                        obj.CustomerOwnedQuantity = (Double)(objDTO.TempPullQTY??0);
        //                    }
        //                }


        //                #endregion

        //                #region "Saving Location and QTY data"
        //                // update requisition qty

        //                if (objDTO.RequisitionDetailGUID != null && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //                {
        //                    //if (ItemDTO.RequisitionedQuantity != null && ItemDTO.RequisitionedQuantity.GetValueOrDefault(0) > 0)
        //                    //  ItemDTO.RequisitionedQuantity = ItemDTO.RequisitionedQuantity.GetValueOrDefault(0) - objDTO.TempPullQTY.GetValueOrDefault(0);
        //                    new RequisitionDetailsDAL(base.DataBaseName).UpdateItemOnRequisitionQty(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0));

        //                }
        //                ItemDTO.WhatWhereAction = "Pull";
        //                objItemDAL.Edit(ItemDTO);


        //                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
        //                lstUpdate.Add(lstLocDTO);
        //                objLocQTY.Save(lstUpdate);
        //                #endregion

        //                #endregion

        //                #region "Project Spend Quantity Update"
        //                //obj.WhatWhereAction = "Project Spend";
        //                if (objPullDal.Edit(obj))
        //                {
        //                    if (objDTO.ProjectSpendGUID != null && objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //                    {
        //                        UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //                    }
        //                }
        //                #endregion

        //                #region "Update Turns and Average Usgae"
        //                UpdateTurnsAverageUsage(obj);
        //                #endregion
        //            }
        //            else
        //            {
        //                List<MaterialStagingPullDetailDTO> ObjItemLocationMS = null;
        //                MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
        //                MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
        //                List<MaterialStagingDetailDTO> lstLocDTOMS = new List<MaterialStagingDetailDTO>();

        //                List<MaterialStagingPullDetailDTO> lstPullDetailsDTO = objMaterialStagingPullDetailDAL.GetMsPullDetailsByItemGUIDANDBinID(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID).ToList();


        //                //Pick up the locations.....
        //                if (ItemDTO.SerialNumberTracking)
        //                {
        //                    ObjItemLocationMS = lstPullDetailsDTO.Take((int)objDTO.TempPullQTY).ToList();

        //                }
        //                else
        //                {
        //                    ObjItemLocationMS = lstPullDetailsDTO;
        //                }

        //                #region "Project Wise Quantity Check"
        //                if (objDTO.ProjectSpendGUID != null)
        //                {
        //                    objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID).SingleOrDefault();
        //                    objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, false, false);

        //                    if (ProjectWiseQuantityCheckNew(objPrjMsgDAL, objPrjSpenItmDTO, objPrjMstDTO, objPrjSpenItmDAL, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, IsConsingedPull))
        //                    {
        //                        return true;
        //                    }
        //                }
        //                #endregion



        //                #region "Pull Insert Update"
        //                if (IsConsingedPull)
        //                {
        //                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, ItemDTO.SupplierID ?? 0, ItemDTO.GUID, null, false);
        //                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
        //                    {
        //                        objDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
        //                    }
        //                    else
        //                    {
        //                        DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
        //                        objDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
        //                    }
        //                }

        //                ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //                #endregion


        //                #region "Item Location & Quantity  Wise Deduction"

        //                #region "ItemLocation Deduction"
        //                if (IsCreditPullNothing != 3)
        //                {
        //                    if (ItemDTO.SerialNumberTracking)
        //                    {
        //                        #region "Serial logic"


        //                        double LocalSearilaCount = 0;
        //                        //double TakenCreditCount = 0;
        //                        foreach (var itemoil in ObjItemLocationMS)
        //                        {
        //                            double loopCurrentTakenCustomer = 0;
        //                            double loopCurrentTakenConsignment = 0;

        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (IsConsingedPull)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                //loopCurrentTaken = 1;
        //                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                                {
        //                                    loopCurrentTakenConsignment = 1;
        //                                    itemoil.ConsignedQuantity = 0;
        //                                }
        //                                else
        //                                {
        //                                    loopCurrentTakenCustomer = 1;
        //                                    itemoil.CustomerOwnedQuantity = 0;
        //                                }

        //                                #endregion
        //                            }
        //                            else //customerowendQuantity
        //                            {
        //                                //loopCurrentTaken = 1;
        //                                loopCurrentTakenCustomer = 1;
        //                                itemoil.CustomerOwnedQuantity = 0;
        //                            }
        //                            objMaterialStagingPullDetailDAL.Edit(itemoil);
        //                            //AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.Value, ItemDTO.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), itemoil.ItemCost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);

        //                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
        //                            objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
        //                            objMaterialStagingDetailDAL.Edit(objmsddto);
        //                        }
        //                        obj.CreditCustomerOwnedQuantity = LocalSearilaCount;



        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region "LOt and other type logic"

        //                        Double takenQunatity = 0;
        //                        foreach (var itemoil in ObjItemLocationMS)
        //                        {
        //                            Double loopCurrentTakenCustomer = 0;
        //                            Double loopCurrentTakenConsignment = 0;
        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (takenQunatity == objDTO.TempPullQTY)
        //                            {
        //                                break;
        //                            }

        //                            if (IsConsingedPull)
        //                            {
        //                                #region "Consignment Credit and Pull"

        //                                //Both's sum we have available.
        //                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    itemoil.ConsignedQuantity = (Double)(itemoil.ConsignedQuantity??0) - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    takenQunatity += ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    goto Save;
        //                                }
        //                                else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    takenQunatity += (Double)(objDTO.TempPullQTY??0) - takenQunatity;
        //                                    goto Save;
        //                                }
        //                                else
        //                                {
        //                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
        //                                    // needs to write logic for break down deduction from consigned or customer quantity location wise ...
        //                                    if (itemoil.ConsignedQuantity >= ((Double)(objDTO.TempPullQTY??0) - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        takenQunatity += (Double)(objDTO.TempPullQTY??0) - takenQunatity;
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        //loopCurrentTaken = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        itemoil.ConsignedQuantity = 0;
        //                                    }
        //                                    //PENDING -- loop by varialbe from mupliple locations...
        //                                }

        //                                #endregion
        //                            }
        //                            else
        //                            {
        //                                #region "Customreowned Credit and Pull"

        //                                //Both's sum we have available.
        //                                if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity - ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    takenQunatity += ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    goto Save;
        //                                }
        //                                else
        //                                {
        //                                    //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
        //                                }

        //                                #endregion
        //                            }
        //                        Save:
        //                            objMaterialStagingPullDetailDAL.Edit(itemoil);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), Convert.ToDouble(itemoil.ItemCost.GetValueOrDefault(0)), obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);

        //                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
        //                            if (objmsddto != null)
        //                            {
        //                                objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
        //                                objMaterialStagingDetailDAL.Edit(objmsddto);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                #endregion
        //                ItemDTO.WhatWhereAction = "Pull";
        //                objItemDAL.Edit(ItemDTO);


        //                #endregion

        //                //Update started quantity...
        //                objMaterialStagingPullDetailDAL.UpdateStagedQuantity(ItemDTO.GUID, RoomID, CompanyID);


        //                //Updated PS
        //                #region "Project Spend Quantity Update"
        //                //obj.WhatWhereAction = "Project Spend";
        //                if (objPullDal.Edit(obj))
        //                {
        //                    if (objDTO.ProjectSpendGUID != null && objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //                    {
        //                        UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //                    }
        //                }
        //                #endregion

        //            }
        //            //return true;
        //        }
        //        #endregion
        //    }

        //    if (objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //    {
        //        WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
        //        objWOLDAL.UpdateWOItemAndTotalCost(objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
        //    }
        //    UpdateCumulativeOnHand(ReturnDto);
        //    return true;
        //}

        //public bool UpdatePullDataForServiceNew(PullMasterViewDTO objDTO, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, out string ItemLocationMSG, bool IsProjectSpendAllowed, out bool IsPSLimitExceed, bool IsConsingedPull, string CountType = null)
        //{
        //    #region "Global Variables"
        //    ItemLocationMSG = "";
        //    IsPSLimitExceed = false;
        //    Int64 TempOldBinID = 0;
        //    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
        //    ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(base.DataBaseName);
        //    ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
        //    ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
        //    ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
        //    PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
        //    PullMasterViewDTO obj = new PullMasterViewDTO();
        //    ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
        //    BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
        //    ItemLocationQTYDTO lstLocDTO1 = new ItemLocationQTYDTO();
        //    BinMasterDTO objBINDTO = new BinMasterDTO();
        //    List<ItemLocationDetailsDTO> ObjItemLocation = null;
        //    PullMasterViewDTO ReturnDto = null;
        //    RoomDTO objRoomDTO = new RoomDTO();
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);

        //    bool IsStagginLocation = false;
        //    //bool IsProjectSpendMandatoryPleaseSelect = false;

        //    #endregion
        //    ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

        //    objRoomDTO = objRoomDAL.GetRecord(RoomID, CompanyID, false, false);

        //    BinMasterDAL objLocDAL = new BinMasterDAL(base.DataBaseName);
        //    BinMasterDTO objLocDTO = objLocDAL.GetRecord(objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID, false, false);
        //    //BinMasterDTO objLocDTO = objLocDAL.GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, objDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
        //    if (objLocDTO != null && objLocDTO.ID > 0)
        //    {
        //        if (objLocDTO.IsStagingLocation)
        //        {
        //            IsStagginLocation = true;
        //        }
        //    }

        //    if (!IsStagginLocation && objDTO.BinID.GetValueOrDefault(0) > 0)
        //    {
        //        lstLocDTO = objLocQTY.GetRecordByBinItem(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID);
        //    }
        //    if (ItemDTO != null && ItemDTO.ItemType == 4)
        //    {
        //        #region "Pull Insert Update"
        //        ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //        ItemLocationDetailsDTO ObjTempItemLocation = new ItemLocationDetailsDTO();
        //        if (IsConsingedPull)
        //            ObjTempItemLocation.ConsignedQuantity = objDTO.TempPullQTY;
        //        else
        //            ObjTempItemLocation.CustomerOwnedQuantity = objDTO.TempPullQTY;

        //        ObjTempItemLocation.ItemGUID = objDTO.ItemGUID;
        //        ObjTempItemLocation.Room = objDTO.Room;
        //        ObjTempItemLocation.CompanyID = objDTO.CompanyID;
        //        AddtoPullDetail(ObjTempItemLocation, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), ItemDTO.Cost.GetValueOrDefault(0), obj.LastUpdatedBy, objDTO.TempPullQTY.GetValueOrDefault(0), 0, ItemDTO.SellPrice.GetValueOrDefault(0));
        //        #endregion

        //        #region "Project Spend Quantity Update"
        //        //obj.WhatWhereAction = "Project Spend";
        //        if (objPullDal.Edit(obj))
        //        {
        //            if (objDTO.ProjectSpendGUID != null)
        //            {
        //                UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //            }
        //        }
        //        #endregion
        //        // return true;
        //    }
        //    else
        //    {
        //        #region "LIFO FIFO"

        //        Boolean IsFIFO = false;
        //        Boolean Iscust = true;

        //        if (objDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
        //            Iscust = false;
        //        else
        //            Iscust = true;

        //        if (objRoomDTO != null && objRoomDTO.ID > 0)
        //        {
        //            if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "fifo")
        //                IsFIFO = true;
        //            if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "lifo")
        //                IsFIFO = false;
        //        }
        //        else
        //        {
        //            IsFIFO = true;
        //        }

        //        #endregion

        //        #region "For Item Pull"
        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            //if staging location then check qty on staging module
        //            //if (IsStagginLocation)
        //            //{
        //            //    #region "Stagging Bin Wise Quantity Check"
        //            //    double retval = 0;
        //            //    var qry = (from msd in context.MaterialStagingDetails
        //            //               join bm in context.BinMasters on msd.StagingBinID equals bm.ID
        //            //               where msd.ItemGUID == ItemDTO.GUID && msd.IsArchived == false && msd.IsDeleted == false && bm.ID == objDTO.BinID
        //            //               select msd);
        //            //    if (qry.Any())
        //            //    {
        //            //        retval = qry.Sum(t => t.Quantity) ?? 0;
        //            //    }
        //            //    if (retval < objDTO.TempPullQTY)
        //            //    {
        //            //        ItemLocationMSG = "Not Enough Quantity for Location ## " + objLocDTO.BinNumber + "(Avl.QTY=" + retval.ToString() + ")";
        //            //        return true;
        //            //    }
        //            //    #endregion

        //            //}
        //            //else
        //            //{
        //            //    #region "Bin Wise Quantity Check"

        //            //    lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).SingleOrDefault();
        //            //    lstLocDTO1 = objLocationDAL.GetItemQtyByLocation(objDTO.BinID ?? 0, objDTO.ItemGUID ?? Guid.Empty, RoomID, CompanyID, objDTO.CreatedBy ?? 0);
        //            //    if (lstLocDTO == null && lstLocDTO1 != null && lstLocDTO1.Quantity > 0)
        //            //    {
        //            //        objLocQTY.Insert(lstLocDTO1);
        //            //        lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objDTO.BinID && x.ItemGUID == objDTO.ItemGUID.Value).SingleOrDefault();
        //            //    }
        //            //    objBINDTO = null;// objBINDAL.GetRecord((Int64)objDTO.BinID, RoomID, CompanyID, false, false);

        //            //    if (BinWiseQuantityCheckNew(lstLocDTO, objLocQTY, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsConsingedPull))
        //            //    {
        //            //        return true;
        //            //    }
        //            //    #endregion
        //            //}


        //            if (!IsStagginLocation)
        //            {

        //                //Pick up the locations.....
        //                if (ItemDTO.SerialNumberTracking)
        //                {
        //                    if (IsCreditPullNothing == 2) // pull
        //                    {
        //                        //ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).Take((int)objDTO.TempPullQTY).ToList();
        //                        if (string.IsNullOrWhiteSpace(objDTO.SerialNumber))
        //                        {
        //                            if (Iscust)
        //                                ObjItemLocation = objLocationDAL.GetCustForLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).Take((int)objDTO.TempPullQTY).ToList();
        //                            else
        //                                ObjItemLocation = objLocationDAL.GetConsForLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).Take((int)objDTO.TempPullQTY).ToList();
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Iscust)
        //                        ObjItemLocation = objLocationDAL.GetCustForLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).ToList();
        //                    else
        //                        ObjItemLocation = objLocationDAL.GetConsForLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).ToList();
        //                    //ObjItemLocation = objLocationDAL.GetCustomerFirstThenConsigedByLIFOFIFO(IsFIFO, (Int64)objDTO.BinID, RoomID, CompanyID, objDTO.ItemGUID.Value, null).ToList();
        //                }

        //                #region "Project Wise Quantity Check"
        //                //if (objDTO.ProjectSpendGUID != null && objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
        //                //{
        //                //    objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID).SingleOrDefault();
        //                //    objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, false, false);

        //                //    if (ProjectWiseQuantityCheckNew(objPrjMsgDAL, objPrjSpenItmDTO, objPrjMstDTO, objPrjSpenItmDAL, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, IsConsingedPull))
        //                //    {
        //                //        return true;
        //                //    }
        //                //}
        //                #endregion

        //                #region "Pull Insert Update"
        //                if (ItemDTO.Consignment && (!string.IsNullOrEmpty(CountType)) && CountType == "M")
        //                {
        //                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, ItemDTO.SupplierID ?? 0, ItemDTO.GUID, null, false);
        //                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
        //                    {
        //                        objDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
        //                    }
        //                    else
        //                    {
        //                        DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
        //                        objDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
        //                    }

        //                }
        //                ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //                #endregion

        //                #region "Item Location & Quantity  Wise Deduction"

        //                #region "ItemLocation Deduction"
        //                if (IsCreditPullNothing != 3)
        //                {
        //                    if (ItemDTO.SerialNumberTracking)
        //                    {
        //                        #region "Serial logic"


        //                        double LocalSearilaCount = 0;
        //                        double TakenCreditCount = 0;
        //                        foreach (var itemoil in ObjItemLocation)
        //                        {
        //                            double loopCurrentTakenCustomer = 0;
        //                            double loopCurrentTakenConsignment = 0;

        //                            if (!Iscust)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID).Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID).Where(t => (t.ConsignedQuantity.GetValueOrDefault(0) != t.CreditConsignedQuantity.GetValueOrDefault(0)) || (t.CustomerOwnedQuantity.GetValueOrDefault(0) != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0))).OrderByDescending(t => t.ID).Take(1).SingleOrDefault();
        //                                    if (result != null)
        //                                    {
        //                                        if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            if (TakenCreditCount != objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
        //                                                TakenCreditCount += 1;
        //                                            }
        //                                            itemoil.ConsignedQuantity = 1;
        //                                            result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + 1;
        //                                        }
        //                                        this.Edit(result);
        //                                    }
        //                                    itemoil.ConsignedQuantity = 1;//itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY;
        //                                }
        //                                if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //loopCurrentTaken = 1;
        //                                    if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                                    {
        //                                        loopCurrentTakenConsignment = 1;
        //                                        itemoil.ConsignedQuantity = 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        loopCurrentTakenCustomer = 1;
        //                                        itemoil.CustomerOwnedQuantity = 0;
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                            else //customerowendQuantity
        //                            {
        //                                #region "customerowned Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    if (LocalSearilaCount != objDTO.TempPullQTY)
        //                                    {
        //                                        LocalSearilaCount += 1;
        //                                        itemoil.CustomerOwnedQuantity = 1;//itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY;
        //                                    }
        //                                    else
        //                                    {
        //                                        objLocationDAL.Edit(itemoil);
        //                                        break;
        //                                    }
        //                                }
        //                                else if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //loopCurrentTaken = 1;
        //                                    loopCurrentTakenCustomer = 1;
        //                                    itemoil.CustomerOwnedQuantity = 0;
        //                                }
        //                                #endregion
        //                            }
        //                            double? itemCost = CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemDTO.SellPrice, itemoil.Cost, ItemDTO.CostUOMID, RoomID, CompanyID);
        //                            double? itemPrice = CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, loopCurrentTakenConsignment > 0, ItemDTO.Cost, itemoil.Cost, ItemDTO.CostUOMID, RoomID, CompanyID);
        //                            objLocationDAL.Edit(itemoil);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), itemoil.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemoil.SellPrice);
        //                        }
        //                        obj.CreditCustomerOwnedQuantity = LocalSearilaCount;

        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region "LOt and other type logic"

        //                        Double takenQunatity = 0;
        //                        foreach (var itemoil in ObjItemLocation)
        //                        {
        //                            Double loopCurrentTakenCustomer = 0;
        //                            Double loopCurrentTakenConsignment = 0;
        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (takenQunatity == objDTO.TempPullQTY)
        //                            {
        //                                break;
        //                            }
        //                            if (!Iscust)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit
        //                                {
        //                                    #region "Consignment Credit"
        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID)
        //                                                               .Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID)
        //                                                               .Where(t => (t.ConsignedQuantity.GetValueOrDefault(0)
        //                                                                    != t.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                                                    || (t.CustomerOwnedQuantity.GetValueOrDefault(0)
        //                                                                        != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0)))
        //                                                                .OrderByDescending(t => t.ID).Take(1).SingleOrDefault();

        //                                    if (result != null)
        //                                    {
        //                                        double unSatalledDifferent = 0;
        //                                        if (result.ConsignedQuantity.GetValueOrDefault(0) != result.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.ConsignedQuantity.GetValueOrDefault(0) - result.CreditConsignedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditConsignedQuantity = obj.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity + qtyavailable;
        //                                                result.CreditConsignedQuantity = result.CreditConsignedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }
        //                                        }
        //                                        this.Edit(result);
        //                                    }

        //                                    #endregion
        //                                }
        //                                if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    #region "Consignment Pull"
        //                                    //Both's sum we have available.
        //                                    if (itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        itemoil.ConsignedQuantity = (Double)(itemoil.ConsignedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        itemoil.ConsignedQuantity = 0; // 
        //                                        if (itemoil.ConsignedQuantity >= ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity))
        //                                        {
        //                                            loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                            itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                            takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
        //                                            goto Save;
        //                                        }
        //                                        else
        //                                        {
        //                                            loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                            itemoil.ConsignedQuantity = 0;
        //                                        }
        //                                    }
        //                                    #endregion
        //                                }
        //                                #endregion
        //                            }
        //                            else
        //                            {
        //                                #region "Customreowned Credit and Pull"
        //                                if (IsCreditPullNothing == 1) // credit - customer owened - lot number
        //                                {

        //                                    PullMasterViewDTO result = objPullDal.GetAllRecords(RoomID, CompanyID)
        //                                        .Where(t => t.ItemGUID == ItemDTO.GUID && t.ID != obj.ID)
        //                                        .Where(t => (t.ConsignedQuantity.GetValueOrDefault(0)
        //                                                != t.CreditConsignedQuantity.GetValueOrDefault(0))
        //                                        || (t.CustomerOwnedQuantity.GetValueOrDefault(0)
        //                                                != t.CreditCustomerOwnedQuantity.GetValueOrDefault(0)))
        //                                        .OrderByDescending(t => t.ID).Take(1).SingleOrDefault();

        //                                    if (result != null)
        //                                    {
        //                                        double unSatalledDifferent = 0;
        //                                        if (result.CustomerOwnedQuantity.GetValueOrDefault(0) != result.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                                        {
        //                                            // un-satled Customer Owned                   
        //                                            unSatalledDifferent = result.CustomerOwnedQuantity.GetValueOrDefault(0) - result.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
        //                                            if (unSatalledDifferent >= objDTO.TempPullQTY.GetValueOrDefault(0))
        //                                            {
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                takenQunatity += objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + objDTO.TempPullQTY.GetValueOrDefault(0);
        //                                            }
        //                                            else
        //                                            {
        //                                                double qtyavailable = 0;
        //                                                qtyavailable = objDTO.TempPullQTY.GetValueOrDefault(0) - itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                                obj.CreditCustomerOwnedQuantity = obj.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                                takenQunatity += qtyavailable;
        //                                                itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity + qtyavailable;
        //                                                result.CreditCustomerOwnedQuantity = result.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + qtyavailable;
        //                                            }

        //                                        }

        //                                        this.Edit(result);
        //                                    }

        //                                }
        //                                else if (IsCreditPullNothing == 2) // pull
        //                                {
        //                                    //Both's sum we have available.
        //                                    if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        itemoil.CustomerOwnedQuantity = (Double)(itemoil.CustomerOwnedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                        itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                        Save:
        //                            objLocationDAL.Edit(itemoil);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID, itemoil.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemoil.SellPrice);
        //                        }

        //                        #endregion
        //                    }
        //                }
        //                #endregion

        //                #region "ItemLocation Quantity Deduction"
        //                if (IsCreditPullNothing == 1) // credit
        //                {
        //                    if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
        //                    {
        //                        ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
        //                        lstLocDTO.CustomerOwnedQuantity = (Double)lstLocDTO.CustomerOwnedQuantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                    }
        //                    else
        //                    {
        //                        obj.CreditCustomerOwnedQuantity = 0;
        //                        obj.CreditConsignedQuantity = 0;
        //                        ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity + objDTO.TempPullQTY;
        //                        lstLocDTO.ConsignedQuantity = (Double)lstLocDTO.ConsignedQuantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity + (Double)(objDTO.TempPullQTY ?? 0);
        //                    }

        //                }
        //                else if (IsCreditPullNothing == 2) // pull
        //                {
        //                    ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - objDTO.TempPullQTY;
        //                    if (!Iscust)
        //                    {
        //                        //Both's sum we have available.
        //                        if (lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
        //                        {
        //                            obj.ConsignedQuantity = (Double)(objDTO.TempPullQTY ?? 0);
        //                            lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - objDTO.TempPullQTY;
        //                            lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY ?? 0);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objDTO.TempPullQTY;
        //                        lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)(objDTO.TempPullQTY ?? 0);
        //                        obj.CustomerOwnedQuantity = (Double)(objDTO.TempPullQTY ?? 0);
        //                    }
        //                }


        //                #endregion

        //                #region "Saving Location and QTY data"


        //                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
        //                lstUpdate.Add(lstLocDTO);
        //                objLocQTY.Save(lstUpdate);
        //                #endregion

        //                #endregion

        //                #region "Project Spend Quantity Update"
        //                //obj.WhatWhereAction = "Project Spend";
        //                if (objPullDal.Edit(obj))
        //                {
        //                    if (objDTO.ProjectSpendGUID != null && objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //                    {
        //                        UpdateProjectSpendWithCost(ItemDTO, obj, obj.ProjectSpendGUID.Value, RoomID, CompanyID);
        //                    }
        //                }
        //                #endregion

        //                #region "Update Turns and Average Usgae"
        //                UpdateTurnsAverageUsage(obj);
        //                #endregion
        //            }
        //            else
        //            {
        //                List<MaterialStagingPullDetailDTO> ObjItemLocationMS = null;
        //                MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
        //                MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
        //                List<MaterialStagingDetailDTO> lstLocDTOMS = new List<MaterialStagingDetailDTO>();

        //                List<MaterialStagingPullDetailDTO> lstPullDetailsDTO = objMaterialStagingPullDetailDAL.GetMsPullDetailsByItemGUIDANDBinID(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID).ToList();


        //                //Pick up the locations.....
        //                if (ItemDTO.SerialNumberTracking)
        //                {
        //                    ObjItemLocationMS = lstPullDetailsDTO.Take((int)objDTO.TempPullQTY).ToList();

        //                }
        //                else
        //                {
        //                    ObjItemLocationMS = lstPullDetailsDTO;
        //                }

        //                #region "Project Wise Quantity Check"
        //                if (objDTO.ProjectSpendGUID != null)
        //                {
        //                    objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(objDTO.ProjectSpendGUID.Value, (Int64)RoomID, (Int64)CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID).SingleOrDefault();
        //                    objPrjMstDTO = objPrjMsgDAL.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), (Int64)RoomID, (Int64)CompanyID, false, false);

        //                    if (ProjectWiseQuantityCheckNew(objPrjMsgDAL, objPrjSpenItmDTO, objPrjMstDTO, objPrjSpenItmDAL, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, objBINDTO, objDTO, objBINDAL, ItemDTO, IsProjectSpendAllowed, out IsPSLimitExceed, ObjItemLocation, IsConsingedPull))
        //                    {
        //                        return true;
        //                    }
        //                }
        //                #endregion



        //                #region "Pull Insert Update"
        //                if (IsConsingedPull)
        //                {
        //                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, ItemDTO.SupplierID ?? 0, ItemDTO.GUID, null, false);
        //                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
        //                    {
        //                        objDTO.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
        //                    }
        //                    else
        //                    {
        //                        DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
        //                        objDTO.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
        //                    }
        //                }

        //                ReturnDto = PullInsertUpdate(objDTO, obj, TempOldBinID, objPullDal);
        //                #endregion


        //                #region "Item Location & Quantity  Wise Deduction"

        //                #region "ItemLocation Deduction"
        //                if (IsCreditPullNothing != 3)
        //                {
        //                    if (ItemDTO.SerialNumberTracking)
        //                    {
        //                        #region "Serial logic"


        //                        double LocalSearilaCount = 0;
        //                        //double TakenCreditCount = 0;
        //                        foreach (var itemoil in ObjItemLocationMS)
        //                        {
        //                            double loopCurrentTakenCustomer = 0;
        //                            double loopCurrentTakenConsignment = 0;

        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (IsConsingedPull)
        //                            {
        //                                #region "Consignment Credit and Pull"
        //                                //loopCurrentTaken = 1;
        //                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                                {
        //                                    loopCurrentTakenConsignment = 1;
        //                                    itemoil.ConsignedQuantity = 0;
        //                                }
        //                                else
        //                                {
        //                                    loopCurrentTakenCustomer = 1;
        //                                    itemoil.CustomerOwnedQuantity = 0;
        //                                }

        //                                #endregion
        //                            }
        //                            else //customerowendQuantity
        //                            {
        //                                //loopCurrentTaken = 1;
        //                                loopCurrentTakenCustomer = 1;
        //                                itemoil.CustomerOwnedQuantity = 0;
        //                            }
        //                            objMaterialStagingPullDetailDAL.Edit(itemoil);
        //                            //AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.Value, ItemDTO.Cost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), itemoil.ItemCost, obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemoil.ItemCost);

        //                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
        //                            objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
        //                            objMaterialStagingDetailDAL.Edit(objmsddto);
        //                        }
        //                        obj.CreditCustomerOwnedQuantity = LocalSearilaCount;



        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region "LOt and other type logic"

        //                        Double takenQunatity = 0;
        //                        foreach (var itemoil in ObjItemLocationMS)
        //                        {
        //                            Double loopCurrentTakenCustomer = 0;
        //                            Double loopCurrentTakenConsignment = 0;
        //                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                            if (takenQunatity == objDTO.TempPullQTY)
        //                            {
        //                                break;
        //                            }

        //                            if (IsConsingedPull)
        //                            {
        //                                #region "Consignment Credit and Pull"

        //                                //Both's sum we have available.
        //                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    itemoil.ConsignedQuantity = (Double)(itemoil.ConsignedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    goto Save;
        //                                }
        //                                else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
        //                                    goto Save;
        //                                }
        //                                else
        //                                {
        //                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
        //                                    // needs to write logic for break down deduction from consigned or customer quantity location wise ...
        //                                    if (itemoil.ConsignedQuantity >= ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity))
        //                                    {
        //                                        //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                        loopCurrentTakenConsignment = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                        takenQunatity += (Double)(objDTO.TempPullQTY ?? 0) - takenQunatity;
        //                                        goto Save;
        //                                    }
        //                                    else
        //                                    {
        //                                        //loopCurrentTaken = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                        itemoil.ConsignedQuantity = 0;
        //                                    }
        //                                    //PENDING -- loop by varialbe from mupliple locations...
        //                                }

        //                                #endregion
        //                            }
        //                            else
        //                            {
        //                                #region "Customreowned Credit and Pull"

        //                                //Both's sum we have available.
        //                                if (itemoil.CustomerOwnedQuantity >= (objDTO.TempPullQTY - takenQunatity))
        //                                {
        //                                    //loopCurrentTaken = ((Double)(objDTO.TempPullQTY??0) - takenQunatity);
        //                                    loopCurrentTakenCustomer = ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    itemoil.CustomerOwnedQuantity = (Double)(itemoil.CustomerOwnedQuantity ?? 0) - ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    takenQunatity += ((Double)(objDTO.TempPullQTY ?? 0) - takenQunatity);
        //                                    goto Save;
        //                                }
        //                                else
        //                                {
        //                                    //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                                    itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - loopCurrentTakenCustomer;
        //                                }

        //                                #endregion
        //                            }
        //                        Save:
        //                            objMaterialStagingPullDetailDAL.Edit(itemoil);
        //                            obj = AddtoPullDetail(itemoil, obj.GUID, obj.ProjectSpendGUID != null ? obj.ProjectSpendGUID.Value : obj.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), Convert.ToDouble(itemoil.ItemCost.GetValueOrDefault(0)), obj.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, Convert.ToDouble(itemoil.ItemCost.GetValueOrDefault(0)));

        //                            MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(itemoil.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
        //                            if (objmsddto != null)
        //                            {
        //                                objmsddto.Quantity = objmsddto.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
        //                                objMaterialStagingDetailDAL.Edit(objmsddto);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                #endregion
        //                ItemDTO.WhatWhereAction = "Pull";
        //                objItemDAL.Edit(ItemDTO);


        //                #endregion

        //                //Update started quantity...
        //                //objMaterialStagingPullDetailDAL.UpdateStagedQuantity(ItemDTO.GUID, RoomID, CompanyID);
        //            }
        //            //return true;
        //        }
        //        #endregion
        //    }

        //    //if (objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //    //{
        //    //    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
        //    //    objWOLDAL.UpdateWOItemAndTotalCost(objDTO.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
        //    //}
        //    UpdateCumulativeOnHand(ReturnDto);
        //    return true;
        //}

        //public Int64 InsertForService(PullMasterViewDTO objDTO, string ConnectionString)
        //{
        //    objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
        //    objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

        //    objDTO.Updated = DateTimeUtility.DateTimeNow;
        //    objDTO.Created = DateTimeUtility.DateTimeNow;

        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        PullMaster obj = new PullMaster();
        //        obj.ID = 0;

        //        obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
        //        obj.UOI = objDTO.UOI;
        //        obj.PoolQuantity = objDTO.PoolQuantity;
        //        obj.PULLCost = objDTO.PullCost;
        //        obj.PullPrice = objDTO.PullPrice;
        //        obj.ConsignedQuantity = objDTO.ConsignedQuantity;
        //        obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
        //        obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
        //        obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;
        //        obj.ActionType = objDTO.ActionType;
        //        obj.PullCredit = objDTO.PullCredit;
        //        //obj.SerialNumber = objDTO.SerialNumber;
        //        //obj.LotNumber = objDTO.LotNumber;
        //        //obj.DateCode = objDTO.DateCode;
        //        obj.BinID = objDTO.BinID;
        //        obj.UDF1 = objDTO.UDF1;
        //        obj.UDF2 = objDTO.UDF2;
        //        obj.UDF3 = objDTO.UDF3;
        //        obj.UDF4 = objDTO.UDF4;
        //        obj.UDF5 = objDTO.UDF5;
        //        obj.GUID = objDTO.GUID;//Guid.NewGuid();
        //        obj.ItemGUID = objDTO.ItemGUID;
        //        obj.Created = DateTimeUtility.DateTimeNow;
        //        obj.Updated = objDTO.Updated;
        //        obj.CreatedBy = objDTO.CreatedBy;
        //        obj.LastUpdatedBy = objDTO.LastUpdatedBy;
        //        obj.IsDeleted = (bool)objDTO.IsDeleted;
        //        obj.IsArchived = (bool)objDTO.IsArchived;
        //        obj.CompanyID = objDTO.CompanyID;
        //        obj.Room = objDTO.Room;
        //        //obj.WorkOrderDetailID = objDTO.WorkOrderDetailID;
        //        obj.WorkOrderDetailGUID = objDTO.WorkOrderDetailGUID;
        //        obj.CountLineItemGuid = objDTO.CountLineItemGuid;
        //        obj.RequisitionDetailGUID = objDTO.RequisitionDetailGUID;

        //        obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
        //        obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
        //        obj.AddedFrom = objDTO.AddedFrom = "Web";
        //        obj.EditedFrom = objDTO.EditedFrom = "Web";

        //        context.PullMasters.AddObject(obj);
        //        context.SaveChanges();
        //        objDTO.ID = obj.ID;
        //        objDTO.GUID = obj.GUID;

        //        //Fill to cache
        //        //objDTO = FillWithExtraDetailForService(objDTO, ConnectionString);

        //        if (objDTO.ID > 0)
        //        {
        //            //Get Cached-Media
        //            IEnumerable<PullMasterViewDTO> ObjCache = CacheHelper<IEnumerable<PullMasterViewDTO>>.GetCacheItem("Cached_PullMaster_" + objDTO.CompanyID.ToString());
        //            if (ObjCache != null)
        //            {
        //                List<PullMasterViewDTO> tempC = new List<PullMasterViewDTO>();
        //                tempC.Add(objDTO);

        //                IEnumerable<PullMasterViewDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<PullMasterViewDTO>>.AppendToCacheItem("Cached_PullMaster_" + objDTO.CompanyID.ToString(), NewCache);
        //            }
        //        }

        //        return obj.ID;
        //    }

        //}

        //public PullMasterViewDTO FillWithExtraDetailForService(PullMasterViewDTO objDTO, string ConnectionString)
        //{
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    RoomDTO ObjRoomCache = null;
        //    if (objDTO.Room.GetValueOrDefault(0) > 0)
        //    {
        //        ObjRoomCache = objRoomDAL.GetRecordForSerice(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false, ConnectionString);
        //        objDTO.RoomName = ObjRoomCache.RoomName;
        //    }
        //    UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
        //    UserMasterDTO ObjUserCreatedCache = objUserDAL.GetRecordForService(objDTO.CreatedBy.GetValueOrDefault(0), ConnectionString);
        //    UserMasterDTO ObjUserUpdatedCache = objUserDAL.GetRecordForService(objDTO.LastUpdatedBy.GetValueOrDefault(0), ConnectionString);

        //    if (ObjUserCreatedCache != null)
        //        objDTO.CreatedByName = ObjUserCreatedCache.UserName;

        //    if (ObjUserUpdatedCache != null)
        //        objDTO.UpdatedByName = ObjUserUpdatedCache.UserName;

        //    BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
        //    BinMasterDTO objBinDTO = null;
        //    if (objDTO.BinID.GetValueOrDefault(0) > 0)
        //    {
        //        objBinDTO = objBinDAL.GetRecordForService(objDTO.BinID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false, ConnectionString);
        //    }
        //    if (objBinDTO != null)
        //        objDTO.BinNumber = objBinDTO.BinNumber;

        //    ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
        //    ProjectMasterDTO objProjectDTO = null;
        //    if (objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //    {
        //        objProjectDTO = objProjectDAL.GetRecordForService(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false, ConnectionString);
        //    }
        //    if (objProjectDTO != null)
        //        objDTO.ProjectSpendName = objProjectDTO.ProjectSpendName;


        //    ItemMasterDAL objItemDAL = new ItemMasterDAL(ConnectionString);
        //    ItemMasterDTO objItemDTO = null;
        //    if (objDTO.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //    {
        //        objItemDTO = objItemDAL.GetRecordForService(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false, ConnectionString);
        //    }
        //    if (objItemDTO != null)
        //    {
        //        objDTO.ItemNumber = objItemDTO.ItemNumber;
        //        objDTO.ItemType = objItemDTO.ItemType;
        //        objDTO.Markup = objItemDTO.Markup.GetValueOrDefault(0);
        //        objDTO.SellPrice = objItemDTO.SellPrice.GetValueOrDefault(0);
        //        objDTO.CategoryName = objItemDTO.CategoryName;
        //        objDTO.Unit = objItemDTO.Unit;
        //        objDTO.Description = objItemDTO.Description;
        //        objDTO.DefaultPullQuantity = objItemDTO.DefaultPullQuantity.GetValueOrDefault(0);
        //    }
        //    return objDTO;
        //}
        //public bool EditForService(PullMasterViewDTO objDTO, string ConnectionString)
        //{
        //    objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
        //    objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

        //    objDTO.Updated = DateTimeUtility.DateTimeNow;

        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        PullMaster obj = new PullMaster();
        //        obj = context.PullMasters.FirstOrDefault(t => t.ID == objDTO.ID);
        //        if (obj == null)
        //            obj = new PullMaster();
        //        obj.ID = objDTO.ID;
        //        obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
        //        obj.UOI = objDTO.UOI;
        //        obj.ActionType = objDTO.ActionType;
        //        obj.PullCredit = objDTO.PullCredit;
        //        obj.PULLCost = objDTO.PullCost;
        //        obj.PullPrice = objDTO.PullPrice;
        //        obj.PoolQuantity = objDTO.PoolQuantity;
        //        obj.ConsignedQuantity = objDTO.ConsignedQuantity;
        //        obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
        //        obj.CreditConsignedQuantity = objDTO.CreditConsignedQuantity;
        //        obj.CreditCustomerOwnedQuantity = objDTO.CreditCustomerOwnedQuantity;

        //        //obj.SerialNumber = objDTO.SerialNumber;
        //        //obj.LotNumber = objDTO.LotNumber;
        //        //obj.DateCode = objDTO.DateCode;
        //        obj.BinID = objDTO.BinID;

        //        obj.UDF1 = objDTO.UDF1;
        //        obj.UDF2 = objDTO.UDF2;
        //        obj.UDF3 = objDTO.UDF3;
        //        obj.UDF4 = objDTO.UDF4;
        //        obj.UDF5 = objDTO.UDF5;
        //        obj.GUID = objDTO.GUID;
        //        obj.ItemGUID = objDTO.ItemGUID;
        //        obj.Created = objDTO.Created;
        //        obj.Updated = objDTO.Updated;
        //        obj.CreatedBy = objDTO.CreatedBy;
        //        obj.LastUpdatedBy = objDTO.LastUpdatedBy;
        //        obj.IsDeleted = (bool)objDTO.IsDeleted;
        //        obj.IsArchived = (bool)objDTO.IsArchived;
        //        obj.CompanyID = objDTO.CompanyID;
        //        obj.Room = objDTO.Room;
        //        obj.RequisitionDetailGUID = objDTO.RequisitionDetailGUID;
        //        //obj.WorkOrderDetailID = objDTO.WorkOrderDetailID;
        //        obj.WorkOrderDetailGUID = objDTO.WorkOrderDetailGUID;
        //        obj.CountLineItemGuid = objDTO.CountLineItemGuid;



        //        context.PullMasters.Attach(obj);
        //        context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
        //        context.SaveChanges();

        //        //Get Cached-Media
        //        IEnumerable<PullMasterViewDTO> ObjCache = CacheHelper<IEnumerable<PullMasterViewDTO>>.GetCacheItem("Cached_PullMaster_" + objDTO.CompanyID.ToString());
        //        if (ObjCache != null)
        //        {
        //            List<PullMasterViewDTO> objTemp = ObjCache.ToList();
        //            objTemp.RemoveAll(i => i.ID == objDTO.ID);
        //            ObjCache = objTemp.AsEnumerable();

        //            List<PullMasterViewDTO> tempC = new List<PullMasterViewDTO>();
        //            tempC.Add(objDTO);
        //            IEnumerable<PullMasterViewDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //            CacheHelper<IEnumerable<PullMasterViewDTO>>.AppendToCacheItem("Cached_PullMaster_" + objDTO.CompanyID.ToString(), NewCache);
        //        }


        //        return true;
        //    }
        //}

        //public void UpdateProjectSpendWithCostForService(ItemMasterDTO ItemDTO, PullMasterViewDTO objPullDTO, Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyID, string ConnectionString)
        //{
        //    ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
        //    ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
        //    objProjectSpendDTO = objProjectDAL.GetRecordForService(ProjectSpendGUID, RoomID, CompanyID, false, false, ConnectionString);
        //    if (objProjectSpendDTO != null)
        //    {
        //        //objProjectSpendDTO.DollarUsedAmount = objProjectSpendDTO.DollarUsedAmount.GetValueOrDefault(0) + ((decimal)objPullDTO.PoolQuantity.GetValueOrDefault(0) * (decimal)objPullDTO.PullCost.GetValueOrDefault(0));
        //        objProjectSpendDTO.DollarUsedAmount = objProjectSpendDTO.DollarUsedAmount.GetValueOrDefault(0) + (decimal)objPullDTO.PullCost.GetValueOrDefault(0);
        //        objProjectDAL.Edit(objProjectSpendDTO);

        //        ProjectSpendItemsDTO objPrjSpenItmDTO = new ProjectSpendItemsDTO();
        //        ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(base.DataBaseName);
        //        objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecordsForService(ProjectSpendGUID, RoomID, CompanyID, ConnectionString).Where(x => x.ItemGUID == ItemDTO.GUID).SingleOrDefault(); //&& x.Created.Value <= ItemDTO.Created.Value

        //        if (objPrjSpenItmDTO != null)
        //        {
        //            //Update Quanitty used limit
        //            objPrjSpenItmDTO.DollarUsedAmount = objPrjSpenItmDTO.DollarUsedAmount.GetValueOrDefault(0) + (decimal)objPullDTO.PullCost.GetValueOrDefault(0);
        //            objPrjSpenItmDTO.QuantityUsed = (double)objPrjSpenItmDTO.QuantityUsed.GetValueOrDefault(0) + (double)objPullDTO.PoolQuantity.GetValueOrDefault(0);
        //            objPrjSpenItmDAL.EditForService(objPrjSpenItmDTO, ConnectionString);
        //        }

        //    }

        //}

        //protected PullMasterViewDTO AddtoPullDetailForService(ItemLocationDetailsDTO itemlocationdetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, string ConnectionString)
        //{
        //    PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
        //    PullDetailsDTO objDTO = new PullDetailsDTO();

        //    objDTO.PULLGUID = PullGUID;
        //    objDTO.ItemGUID = itemlocationdetail.ItemGUID;
        //    if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //    {
        //        objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
        //    }
        //    if (itemlocationdetail.CustomerOwnedQuantity != null)
        //        objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

        //    if (itemlocationdetail.ConsignedQuantity != null)
        //        objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

        //    objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

        //    if (itemlocationdetail.SerialNumber != null)
        //        objDTO.SerialNumber = itemlocationdetail.SerialNumber;

        //    if (itemlocationdetail.LotNumber != null)
        //        objDTO.LotNumber = itemlocationdetail.LotNumber;

        //    if (itemlocationdetail.Expiration != null)
        //        objDTO.Expiration = itemlocationdetail.Expiration;

        //    objDTO.Received = itemlocationdetail.Received;
        //    objDTO.BinID = itemlocationdetail.BinID;
        //    objDTO.Created = DateTimeUtility.DateTimeNow;
        //    objDTO.Updated = DateTimeUtility.DateTimeNow;
        //    objDTO.CreatedBy = UserID;
        //    objDTO.LastUpdatedBy = UserID;
        //    objDTO.Room = itemlocationdetail.Room;
        //    objDTO.CompanyID = itemlocationdetail.CompanyID;
        //    objDTO.ItemLocationDetailGUID = itemlocationdetail.GUID;
        //    objDTO.ItemCost = ItemCost;
        //    objDAL.InsertForService(objDTO, ConnectionString);

        //    //Edit pull master
        //    PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
        //    PullMasterViewDTO objPullDTO = objPullMAster.GetRecordForService(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false, ConnectionString);
        //    if (objPullDTO != null)
        //    {
        //        objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * (double)ItemCost.GetValueOrDefault(0));
        //        objPullMAster.EditForService(objPullDTO, ConnectionString);
        //    }

        //    return objPullDTO;
        //}

        //protected PullMasterViewDTO AddtoPullDetailForService(MaterialStagingPullDetailDTO itemlocationdetail, Guid? PullGUID, Guid? ProjectSpendGUID, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment, string ConnectionString)
        //{
        //    PullDetailsDAL objDAL = new PullDetailsDAL(base.DataBaseName);
        //    PullDetailsDTO objDTO = new PullDetailsDTO();

        //    objDTO.PULLGUID = PullGUID;
        //    objDTO.ItemGUID = itemlocationdetail.ItemGUID;
        //    if (ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //    {
        //        objDTO.ProjectSpendGUID = ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
        //    }
        //    if (itemlocationdetail.CustomerOwnedQuantity != null)
        //        objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

        //    if (itemlocationdetail.ConsignedQuantity != null)
        //        objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

        //    objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

        //    if (itemlocationdetail.SerialNumber != null)
        //        objDTO.SerialNumber = itemlocationdetail.SerialNumber;

        //    if (itemlocationdetail.LotNumber != null)
        //        objDTO.LotNumber = itemlocationdetail.LotNumber;

        //    if (itemlocationdetail.Expiration != null)
        //        objDTO.Expiration = itemlocationdetail.Expiration;

        //    objDTO.Received = itemlocationdetail.Received;
        //    objDTO.BinID = itemlocationdetail.StagingBinId;
        //    objDTO.Created = DateTimeUtility.DateTimeNow;
        //    objDTO.Updated = DateTimeUtility.DateTimeNow;
        //    objDTO.CreatedBy = UserID;
        //    objDTO.LastUpdatedBy = UserID;
        //    objDTO.Room = itemlocationdetail.Room;
        //    objDTO.CompanyID = itemlocationdetail.CompanyID;
        //    //objDTO.ItemLocationDetailGUID = itemlocationdetail.GUID;
        //    objDTO.MaterialStagingPullDetailGUID = itemlocationdetail.GUID;
        //    objDTO.ItemCost = ItemCost;
        //    objDAL.InsertForService(objDTO, ConnectionString);

        //    //Edit pull master
        //    PullMasterDAL objPullMAster = new PullMasterDAL(base.DataBaseName);
        //    PullMasterViewDTO objPullDTO = objPullMAster.GetRecordForService(objDTO.PULLGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false, ConnectionString);
        //    if (objPullDTO != null)
        //    {
        //        objPullDTO.PullCost = (objPullDTO.PullCost.GetValueOrDefault(0)) + (objDTO.PoolQuantity.GetValueOrDefault(0) * (double)ItemCost.GetValueOrDefault(0));
        //        objPullMAster.EditForService(objPullDTO, ConnectionString);
        //    }

        //    return objPullDTO;
        //}
        //public void PullInsertUpdateForService(PullMasterViewDTO objDTO, PullMasterViewDTO obj, Int64 TempOldBinID, PullMasterDAL objPullDal, string ConnectionString)
        //{
        //    #region "Pull Insert Update"
        //    if (objDTO.ID > 0)
        //    {
        //        #region "Pull Update"
        //        //// commented the above code and added below code to Insert new Pull every time ....
        //        objDTO.ID = 0;
        //        objDTO.PoolQuantity = objDTO.TempPullQTY;

        //        obj.ID = objDTO.ID; obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : objDTO.ProjectSpendGUID;
        //        obj.PoolQuantity = objDTO.PoolQuantity;
        //        obj.BinID = objDTO.BinID; obj.Updated = objDTO.Updated; obj.LastUpdatedBy = objDTO.LastUpdatedBy;
        //        obj.GUID = Guid.NewGuid(); obj.ItemGUID = objDTO.ItemGUID; obj.Created = DateTimeUtility.DateTimeNow;
        //        obj.Updated = DateTimeUtility.DateTimeNow; obj.CreatedBy = objDTO.CreatedBy; obj.LastUpdatedBy = objDTO.LastUpdatedBy; obj.IsDeleted = false; obj.IsArchived = false; obj.CompanyID = objDTO.CompanyID; obj.Room = objDTO.Room;
        //        obj.PullCredit = objDTO.PullCredit;
        //        obj.ActionType = objDTO.PullCredit;
        //        obj.UDF1 = objDTO.UDF1; obj.UDF2 = objDTO.UDF2; obj.UDF3 = objDTO.UDF3; obj.UDF4 = objDTO.UDF4; obj.UDF5 = objDTO.UDF5;
        //        obj.RequisitionDetailGUID = objDTO.RequisitionDetailGUID;
        //        //obj.WorkOrderDetailID = objDTO.WorkOrderDetailID;
        //        obj.WorkOrderDetailGUID = obj.WorkOrderDetailGUID;
        //        obj.CountLineItemGuid = obj.CountLineItemGuid;
        //        //TempOldBinID = (Int64)objDTO.BinID;

        //        obj.ID = objPullDal.InsertForService(obj, ConnectionString);

        //        #endregion
        //    }
        //    else
        //    {
        //        #region "Pull Insert"


        //        obj.ID = objDTO.ID; obj.ProjectSpendGUID = objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? null : obj.ProjectSpendGUID;
        //        obj.PoolQuantity = objDTO.PoolQuantity;
        //        obj.BinID = objDTO.BinID; obj.Updated = objDTO.Updated; obj.LastUpdatedBy = objDTO.LastUpdatedBy;
        //        obj.GUID = Guid.NewGuid(); obj.ItemGUID = objDTO.ItemGUID; obj.Created = DateTimeUtility.DateTimeNow;
        //        obj.Updated = DateTimeUtility.DateTimeNow; obj.CreatedBy = objDTO.CreatedBy; obj.LastUpdatedBy = objDTO.LastUpdatedBy; obj.IsDeleted = false; obj.IsArchived = false; obj.CompanyID = objDTO.CompanyID; obj.Room = objDTO.Room;
        //        obj.PullCredit = objDTO.PullCredit;
        //        obj.ActionType = objDTO.PullCredit;
        //        obj.UDF1 = objDTO.UDF1; obj.UDF2 = objDTO.UDF2; obj.UDF3 = objDTO.UDF3; obj.UDF4 = objDTO.UDF4; obj.UDF5 = objDTO.UDF5;
        //        obj.RequisitionDetailGUID = objDTO.RequisitionDetailGUID;
        //        //obj.WorkOrderDetailID = objDTO.WorkOrderDetailID;
        //        obj.WorkOrderDetailGUID = objDTO.WorkOrderDetailGUID;
        //        obj.CountLineItemGuid = objDTO.CountLineItemGuid;
        //        //TempOldBinID = (Int64)objDTO.BinID;

        //        obj.ID = objPullDal.InsertForService(obj, ConnectionString);

        //        #endregion
        //    }


        //    //if (objDTO.RequisitionDetailGUID != null)
        //    //{
        //    //    RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(base.DataBaseName);
        //    //    RequisitionDetailsDTO objReqDTO = objReqDDAL.GetRecord(objDTO.RequisitionDetailGUID.Value, (Int64)objDTO.Room, (Int64)objDTO.CompanyID);
        //    //    RequisitionDetailsDTO ObjOldReqDTO = objReqDDAL.GetRecord(objDTO.RequisitionDetailGUID.Value, (Int64)objDTO.Room, (Int64)objDTO.CompanyID);

        //    //    objReqDTO.QuantityPulled = objReqDTO.QuantityPulled.GetValueOrDefault(0) + objDTO.PoolQuantity;
        //    //    objReqDDAL.Edit(objReqDTO);
        //    //}

        //    #endregion
        //}

        //public void UpdateTurnsAverageUsageForService(PullMasterViewDTO objDTO, string ConnectionString)
        //{
        //    #region "Update Turns and Average Usage"
        //    try
        //    {
        //        DashboardDAL objDashbordDal = new DashboardDAL(base.DataBaseName);
        //        DashboardAnalysisInfo objDashbordTurns = objDashbordDal.UpdateTurnsByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy ?? 0, null, null);
        //        DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy ?? 0, null, null);
        //    }
        //    catch (Exception)
        //    {
        //    }


        //    //ItemMasterDAL objItem = new ItemMasterDAL(ConnectionString);
        //    //ItemMasterDTO ItemDTO = objItem.GetRecord(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty).ToString(), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
        //    //InventoryDashboardDTO InvDashDTO = objItem.GetHeaderCountByItemIDForService(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ItemGUID.GetValueOrDefault(Guid.Empty).ToString(), ConnectionString);
        //    //RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    //RoomDTO objRoomDTO = objRoomDAL.GetRecordForSerice(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false, false, ConnectionString);
        //    //if (ItemDTO != null)
        //    //{
        //    //    ItemDTO.Turns = Convert.ToDouble(InvDashDTO.Turns.GetValueOrDefault(0));
        //    //    if (objRoomDTO != null && objRoomDTO.IsAverageUsageBasedOnPull)
        //    //    {
        //    //        double ItemInventoryValue = ItemDTO.OnHandQuantity.GetValueOrDefault(0) * Convert.ToDouble(ItemDTO.Cost.GetValueOrDefault(1));
        //    //        double TotalQtyPUlled = objDTO.PoolQuantity.GetValueOrDefault(0) * objDTO.PullCost.GetValueOrDefault(1);
        //    //        double AverageUsage = TotalQtyPUlled / (ItemInventoryValue / objDTO.PoolQuantity.GetValueOrDefault(1));
        //    //        ItemDTO.AverageUsage = AverageUsage;
        //    //    }
        //    //    objItem.EditForService(ItemDTO, ConnectionString);
        //    //}
        //    #endregion
        //}

        #endregion
    }
}
