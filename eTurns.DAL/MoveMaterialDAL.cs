using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class MoveMaterialDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public MoveMaterialDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public MoveMaterialDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        CommonDAL objCommonDAL = null;
        ItemMasterDAL objItemDAL = null;
        ItemLocationDetailsDAL objItmLocDtlsDAL = null;
        ItemLocationQTYDAL objItemLocQtyDAL = null;
        MaterialStagingDAL objMSDAL = null;
        MaterialStagingDetailDAL objMSDtlDAL = null;
        BinMasterDAL objBinDAL = null;
        MaterialStagingPullDetailDAL objMSPullDtlDAL = null;

        /// <summary>
        /// MoveInventory
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool MoveInventory(MoveMaterialDTO objDTO, long SessionUserId,string RoomDateFormat, long EnterpriseId)
        {
            MaterialStagingDTO objMSDTO = null;
            BinMasterDTO objBINDTO = null;
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(base.DataBaseName);

            #region Insert Location or Material Staging

            objCommonDAL = new CommonDAL(base.DataBaseName);
            if (objDTO.MoveType == (int)MoveType.InvToInv || objDTO.MoveType == (int)MoveType.StagToInv)
            {
                if (!string.IsNullOrEmpty(objDTO.DestinationLocation))
                {
                    objItmLocDtlsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                    BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(objDTO.ItemGUID, objDTO.DestinationLocation, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy, false);
                    objDTO.DestBinID = objBinMasterDTO.ID;
                    //objDTO.DestBinID = objCommonDAL.GetOrInsertBinIDByName(objDTO.ItemGUID, objDTO.DestinationLocation, objDTO.CreatedBy, objDTO.RoomID, objDTO.CompanyID).GetValueOrDefault(0);
                }
            }
            else if (objDTO.MoveType == (int)MoveType.StagToStag || objDTO.MoveType == (int)MoveType.InvToStag)
            {
                if (objDTO.DestStagingHeaderGuid.GetValueOrDefault(Guid.Empty) == Guid.Empty && !string.IsNullOrEmpty(objDTO.DestinationStagingHeader))
                {
                    Int64 StagHeaderID = objCommonDAL.GetOrInsertMaterialStagingIDByName(objDTO.DestinationStagingHeader, objDTO.CreatedBy, objDTO.RoomID, objDTO.CompanyID).GetValueOrDefault(0);
                    objMSDAL = new MaterialStagingDAL(base.DataBaseName);
                    objMSDTO = objMSDAL.GetRecord(StagHeaderID, objDTO.RoomID, objDTO.CompanyID);
                    objDTO.DestBinID = objCommonDAL.GetOrInsertBinIDByName(objDTO.ItemGUID, objDTO.DestinationLocation, objDTO.CreatedBy, objDTO.RoomID, objDTO.CompanyID, true).GetValueOrDefault(0);
                    BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(objDTO.ItemGUID, objDTO.DestinationLocation, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy, true);
                    objDTO.DestBinID = objBinMasterDTO.ID;
                    objDTO.DestStagingHeaderGuid = objMSDTO.GUID;
                    //objMSDTO.BinID = objDTO.DestBinID;
                    objBinDAL = new BinMasterDAL(base.DataBaseName);
                    //objBINDTO = objBinDAL.GetBinByID(objDTO.DestBinID, objDTO.RoomID, objDTO.CompanyID);
                    //objBINDTO = objBinDAL.GetItemLocation( objDTO.RoomID, objDTO.CompanyID, false, false,Guid.Empty, objDTO.DestBinID,null,null).FirstOrDefault();
                    // objMSDTO.BinGUID = objBINDTO.GUID;
                    objMSDTO.StagingLocationName = "[|EmptyStagingBin|]";
                    objMSDTO.IsOnlyFromItemUI = true;
                    objMSDTO.EditedFrom = objDTO.EditedFrom;
                    objMSDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objMSDAL.Edit(objMSDTO, SessionUserId);
                }
                else if (objDTO.DestStagingHeaderGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty && !string.IsNullOrEmpty(objDTO.DestinationStagingHeader))
                {
                    if (objDTO.DestBinID <= 0 && !string.IsNullOrEmpty(objDTO.DestinationLocation))
                    {
                        BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(objDTO.ItemGUID, objDTO.DestinationLocation, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy, true);
                        objDTO.DestBinID = objBinMasterDTO.ID;
                        //objDTO.DestBinID = objCommonDAL.GetOrInsertBinIDByName(objDTO.ItemGUID, objDTO.DestinationLocation, objDTO.CreatedBy, objDTO.RoomID, objDTO.CompanyID, true).GetValueOrDefault(0);
                    }
                }
            }

            #endregion

            if (objDTO.MoveType == (int)MoveType.InvToInv)
            {
                MoveInvetoryToInventoryLocation(objDTO, SessionUserId, RoomDateFormat,EnterpriseId);
            }
            else if (objDTO.MoveType == (int)MoveType.InvToStag)
            {
                MoveInvetoryToStagingLocation(objDTO, SessionUserId, RoomDateFormat,EnterpriseId);
                
                QBItemQOHProcess(objDTO.ItemGUID, objDTO.CompanyID, objDTO.RoomID, SessionUserId, "Move Invetory To Staging");
            }
            else if (objDTO.MoveType == (int)MoveType.StagToInv)
            {
                MoveStagingToInvetoryLocation(objDTO, SessionUserId, RoomDateFormat,EnterpriseId);
                
                QBItemQOHProcess(objDTO.ItemGUID, objDTO.CompanyID, objDTO.RoomID, SessionUserId, "Move Staging To Invetory");
            }
            else if (objDTO.MoveType == (int)MoveType.StagToStag)
            {
                MoveStagingToStagingLocation(objDTO, SessionUserId, RoomDateFormat,EnterpriseId);
            }

            return true;
        }

        private void QBItemQOHProcess(Guid ItemGUID, Int64 CompanyID, Int64 RoomID, long SessionUserId, string WhatWhereAction)
        {
            QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(base.DataBaseName);
            EnterpriseDAL objEntDAL = new EnterpriseDAL(base.DataBaseName);
            EnterpriseDTO objEntDTO = objEntDAL.GetEnterpriseByDbName(base.DataBaseName);
            //objQBItemDAL.InsertQuickBookItem(ItemGUID, objEntDTO.ID, CompanyID, RoomID, "Update", false, SessionUserId, "Web", null, WhatWhereAction);
        }

        public DateTime GetExpireDateFromString(string ExpirationDate,string RoomDateFormat)
        {
            string MainExpirationDate = ExpirationDate;
            ExpirationDate = ExpirationDate.Replace("-", "/");

            string strDateFormate = "MM/dd/yy";

            string[] arrDates = ExpirationDate.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (arrDates != null && arrDates.Length == 3)
            {
                for (int i = 0; i < arrDates.Length; i++)
                {
                    if (i == 0 && arrDates[i].Length == 1)
                    {
                        arrDates[i] = "0" + arrDates[i];
                    }
                    else if (i == 1 && arrDates[i].Length == 1)
                    {
                        arrDates[i] = "0" + arrDates[i];
                    }
                    else if (i == 2 && arrDates[i].Length == 4)
                    {
                        strDateFormate = "MM/dd/yyyy";
                    }
                }

                ExpirationDate = arrDates[0] + "/" + arrDates[1] + "/" + arrDates[2];
                DateTime? expDate = null;
                try
                {
                    expDate = DateTime.ParseExact(ExpirationDate, strDateFormate, new System.Globalization.CultureInfo("en-US").DateTimeFormat);
                }
                catch
                {
                    expDate = null;
                }
                
                if (expDate == null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(RoomDateFormat) && !string.IsNullOrWhiteSpace(RoomDateFormat))
                        {
                            if (RoomDateFormat.Contains("-"))
                            {
                                MainExpirationDate = MainExpirationDate.Replace("/", "-");
                            }
                            else if (RoomDateFormat.Contains("/"))
                            {
                                MainExpirationDate = MainExpirationDate.Replace("-", "/");
                            }
                            
                            expDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(MainExpirationDate, RoomDateFormat, ResourceHelper.CurrentCult));
                            //return expDate.Value.Date;
                        }
                        //else
                        //{
                        //    //expDate =  Convert.ToDateTime(MainExpirationDate);
                        //    return expDate.Value;
                        //}
                    }
                    catch
                    {
                        expDate = null;
                    }

                    if (expDate == null)
                    {
                        try 
                        {
                            expDate =  Convert.ToDateTime(MainExpirationDate);
                        }
                        catch 
                        {
                            expDate = null;
                        }
                    }
                    
                    return expDate.Value;
                    
                }
                else
                    return expDate.Value;
            }
            else
            {
                if (!string.IsNullOrEmpty(RoomDateFormat) && !string.IsNullOrWhiteSpace(RoomDateFormat))
                {
                    if (RoomDateFormat.Contains("-"))
                    {
                        MainExpirationDate = MainExpirationDate.Replace("/", "-");
                    }
                    else if (RoomDateFormat.Contains("/"))
                    {
                        MainExpirationDate = MainExpirationDate.Replace("-", "/");
                    }
                }
                
                DateTime expDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(MainExpirationDate, RoomDateFormat, ResourceHelper.CurrentCult));
                return expDate.Date;
            }            
        }

        #region ****** Methods for Inventory To Inventory Location ******

        private void MoveInvetoryToInventoryLocation(MoveMaterialDTO objDTO, long SessionUserId,string RoomDateFormat,long EnterpriseId)
        {
            List<ItemLocationDetailsDTO> lstSourceInvLocs = GetSourceLocationsFromGeneralInventory(objDTO);
            List<ItemLocationDetailsDTO> lstDestInvLocs = GetDestinationLocationsForGeneralInventory(objDTO, lstSourceInvLocs);
            UpdateSourceAndDestinationLocationQty(objDTO, lstSourceInvLocs, lstDestInvLocs);
            List<MoveMaterialDetailDTO> lstMoveMaterialDtlDTO = GetMoveMaterialDeteailsDTO(objDTO, lstDestInvLocs, RoomDateFormat);

            #region Save and Insert Source And Destination Location
            objItmLocDtlsDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            int CountSource = lstSourceInvLocs.Count;
            int CountDesting = lstDestInvLocs.Count;
            int CountMoveMtrDtl = lstMoveMaterialDtlDTO.Count;

            if (CountSource == CountDesting && CountDesting == CountMoveMtrDtl)
            {
                for (int i = 0; i < CountSource; i++)
                {
                    lstSourceInvLocs[i].Updated = DateTimeUtility.DateTimeNow;
                    lstSourceInvLocs[i].LastUpdatedBy = objDTO.UpdatedBy;
                    lstSourceInvLocs[i].MoveQuantity = lstSourceInvLocs[i].MoveQuantity;
                    lstSourceInvLocs[i].IsOnlyFromUI = true;
                    lstSourceInvLocs[i].EditedFrom = "Web";
                    lstSourceInvLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    objItmLocDtlsDAL.Edit(lstSourceInvLocs[i]);
                    lstDestInvLocs[i].AddedFrom = "Web";
                    lstDestInvLocs[i].EditedFrom = "Web";
                    lstDestInvLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    lstDestInvLocs[i].ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItmLocDtlsDAL.Insert(lstDestInvLocs[i]);

                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Source = lstSourceInvLocs[i].GUID;
                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Destination = lstDestInvLocs[i].GUID;
                }

                objItemLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                ItemLocationQTYDTO objItemLocQtyDTOS = objItemLocQtyDAL.GetRecordByBinItem(objDTO.ItemGUID, objDTO.SourceBinID, objDTO.RoomID, objDTO.CompanyID);
                ItemLocationQTYDTO objItemLocQtyDTOD = objItemLocQtyDAL.GetRecordByBinItem(objDTO.ItemGUID, objDTO.DestBinID, objDTO.RoomID, objDTO.CompanyID);
                List<ItemLocationQTYDTO> listItemLocQty = new List<ItemLocationQTYDTO>();
                listItemLocQty.Add(objItemLocQtyDTOS);
                if (objItemLocQtyDTOD == null)
                {
                    objItemLocQtyDTOD = new ItemLocationQTYDTO()
                    {
                        BinID = objDTO.DestBinID,
                        ItemGUID = objDTO.ItemGUID,
                        Room = objDTO.RoomID,
                        CompanyID = objDTO.CompanyID,
                        CreatedBy = objDTO.CreatedBy,
                        LastUpdatedBy = objDTO.UpdatedBy,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    };
                }
                listItemLocQty.Add(objItemLocQtyDTOD);
                objItemLocQtyDAL.Save(listItemLocQty, SessionUserId,EnterpriseId);

                objDTO.AddedFrom = "Web";
                objDTO.EditedFrom = "Web";
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                InsertMoveMaterial(objDTO);
                foreach (var item in lstMoveMaterialDtlDTO)
                {
                    item.MoveMaterialGuid = objDTO.GUID;
                    item.AddedFrom = "Web";
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    InsertMoveMaterialDetail(item);
                }
            }
            #endregion

        }

        private List<ItemLocationDetailsDTO> GetSourceLocationsFromGeneralInventory(MoveMaterialDTO objDTO)
        {
            List<ItemLocationDetailsDTO> lstSourceInvLocs = new List<ItemLocationDetailsDTO>();

            #region Take Records up to sum of Move qty

            #region "LIFO FIFO"
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);
            RoomDTO objRoomDTO = new RoomDTO();

            string columnList = "ID,RoomName,InventoryConsuptionMethod";
            objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objDTO.RoomID.ToString() + "", "");


            //objRoomDTO = objRoomDAL.GetRoomByIDPlain(objDTO.RoomID);
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

            List<ItemLocationDetailsDTO> objItemLocDtlsList = null;
            objItmLocDtlsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            objItemLocDtlsList = objItmLocDtlsDAL.GetCustomerFirstThenConsigedByLIFOFIFOForLotSr(IsFIFO, objDTO.SourceBinID, objDTO.RoomID, objDTO.CompanyID, objDTO.ItemGUID, null, objDTO.LotNumber, objDTO.SerialNumber).ToList();

            foreach (var item in objItemLocDtlsList)
            {
                if ((item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0)) > 0)
                {
                    ItemLocationDetailsDTO objItemLocDetailDTO = new ItemLocationDetailsDTO();
                    double SumSourceQty = lstSourceInvLocs.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    if (SumSourceQty < objDTO.MoveQuanity)
                    {
                        item.OrderDetailGUID = null;
                        item.TransferDetailGUID = null;
                        item.KitDetailGUID = null;
                        lstSourceInvLocs.Add(item);
                    }
                }
            }
            #endregion

            #region Commented Check and modify if sum of source location > move qty

            //double sumSourceInvLocs = lstSourceInvLocs.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));

            //if (sumSourceInvLocs > objDTO.MoveQuanity)
            //{
            //    double sumQty = 0;
            //    foreach (var item in lstSourceInvLocs)
            //    {
            //        sumQty += item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
            //        if (sumQty > objDTO.MoveQuanity)
            //        {
            //            double diff = sumQty - objDTO.MoveQuanity;
            //            if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > diff)
            //            {
            //                item.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0) - diff;
            //            }
            //            else if (item.ConsignedQuantity.GetValueOrDefault(0) > diff)
            //            {
            //                item.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0) - diff;
            //            }
            //        }

            //    }
            //}

            #endregion

            return lstSourceInvLocs;
        }

        private void UpdateSourceAndDestinationLocationQty(MoveMaterialDTO objDTO, List<ItemLocationDetailsDTO> SourceLocation, List<ItemLocationDetailsDTO> DestinatonLocation)
        {
            double sumSourceInvLocs = SourceLocation.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
            //double sumQty = 0;
            double quantityToMove = objDTO.MoveQuanity;

            for (int i = 0; i < SourceLocation.Count; i++)
            {
                if (quantityToMove <= 0)
                {
                    break;
                }

                double sumQty = SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) + SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                if (sumQty > quantityToMove)
                {
                    //double diff = sumQty - objDTO.MoveQuanity;
                    if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                    {
                        if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) <= quantityToMove)
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity;
                            SourceLocation[i].CustomerOwnedQuantity = 0;
                            quantityToMove-= DestinatonLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0);
                            //SourceLocation[i].LotNumber = string.Empty;
                            //SourceLocation[i].SerialNumber = string.Empty;
                            //SourceLocation[i].Expiration = string.Empty;

                        }
                        else if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) > quantityToMove)
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = quantityToMove;//SourceLocation[i].CustomerOwnedQuantity - diff;
                            SourceLocation[i].CustomerOwnedQuantity = (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) - quantityToMove);//diff;
                            quantityToMove -= DestinatonLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0);

                        }
                    }

                    if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) > 0)
                    {
                        if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) <= quantityToMove)
                        {
                            DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                            SourceLocation[i].ConsignedQuantity = 0;
                            quantityToMove -= DestinatonLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                            //SourceLocation[i].LotNumber = string.Empty;
                            //SourceLocation[i].SerialNumber = string.Empty;
                            //SourceLocation[i].Expiration = string.Empty;

                        }
                        else if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) > quantityToMove)
                        {
                            DestinatonLocation[i].ConsignedQuantity = quantityToMove;//SourceLocation[i].ConsignedQuantity - diff;
                            SourceLocation[i].ConsignedQuantity = (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) - quantityToMove);//diff;
                            quantityToMove -= DestinatonLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                        }
                    }
                }
                else
                {
                    DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity;
                    DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity;
                    SourceLocation[i].CustomerOwnedQuantity = 0;
                    SourceLocation[i].ConsignedQuantity = 0;
                    quantityToMove-=(DestinatonLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) + DestinatonLocation[i].ConsignedQuantity.GetValueOrDefault(0));
                    //SourceLocation[i].LotNumber = string.Empty;
                    //SourceLocation[i].SerialNumber = string.Empty;
                    //SourceLocation[i].Expiration = string.Empty;
                }
                SourceLocation[i].MoveQuantity = (DestinatonLocation[i].CustomerOwnedQuantity ?? 0) + (DestinatonLocation[i].ConsignedQuantity ?? 0);
            }

        }

        private List<ItemLocationDetailsDTO> GetDestinationLocationsForGeneralInventory(MoveMaterialDTO objDTO, List<ItemLocationDetailsDTO> SourceInvLocList)
        {
            List<ItemLocationDetailsDTO> lstDestInvLocs = new List<ItemLocationDetailsDTO>();
            foreach (var item in SourceInvLocList)
            {
                ItemLocationDetailsDTO obj = new ItemLocationDetailsDTO();

                obj.BinID = objDTO.DestBinID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.UpdatedBy;
                obj.Room = objDTO.RoomID;
                obj.CompanyID = objDTO.CompanyID;

                obj.ItemGUID = item.ItemGUID;
                obj.ConsignedQuantity = 0;//item.ConsignedQuantity;
                obj.CustomerOwnedQuantity = 0;// item.CustomerOwnedQuantity;
                obj.Expiration = item.Expiration;
                obj.ExpirationDate = item.ExpirationDate;
                obj.SerialNumber = item.SerialNumber;
                obj.LotNumber = item.LotNumber;
                obj.Cost = item.Cost;
                obj.Received = item.Received;
                obj.ReceivedDate = item.ReceivedDate;

                obj.Markup = item.Markup; obj.SellPrice = item.SellPrice;
                obj.DateCodeTracking = item.DateCodeTracking; obj.SerialNumberTracking = item.SerialNumberTracking; obj.LotNumberTracking = item.LotNumberTracking;
                obj.CriticalQuantity = item.CriticalQuantity; obj.SuggestedOrderQuantity = item.SuggestedOrderQuantity; obj.MaximumQuantity = item.MaximumQuantity; obj.MeasurementID = item.MeasurementID; obj.MinimumQuantity = item.MinimumQuantity;
                //obj.TransferDetailGUID = item.TransferDetailGUID; obj.OrderDetailGUID = item.OrderDetailGUID; obj.ProjectSpentGUID = item.ProjectSpentGUID; obj.KitDetailGUID = item.KitDetailGUID;
                obj.ItemNumber = item.ItemNumber; obj.ItemType = item.ItemType; obj.BinNumber = string.Empty;
                obj.Created = DateTimeUtility.DateTimeNow; obj.Updated = DateTimeUtility.DateTimeNow;
                obj.IsArchived = false; obj.IsDeleted = false; obj.IsCreditPull = false;

                obj.ID = 0; obj.GUID = Guid.Empty; obj.HistoryID = 0;
                obj.UDF1 = item.UDF1; obj.UDF2 = item.UDF2; obj.UDF3 = item.UDF3; obj.UDF4 = item.UDF4; obj.UDF5 = item.UDF5; obj.UDF6 = item.UDF6; obj.UDF7 = item.UDF7; obj.UDF8 = item.UDF8; obj.UDF9 = item.UDF9; obj.UDF10 = item.UDF10;
                obj.Action = string.Empty; obj.UpdatedByName = string.Empty; obj.CreatedByName = string.Empty; obj.RoomName = "";
                obj.mode = "Move Material"; obj.eVMISensorID = string.Empty; obj.eVMISensorPort = null;
                obj.IsConsignedSerialLot = item.IsConsignedSerialLot;
                lstDestInvLocs.Add(obj);

            }

            return lstDestInvLocs;
        }

        private List<MoveMaterialDetailDTO> GetMoveMaterialDeteailsDTO(MoveMaterialDTO objDTO, List<ItemLocationDetailsDTO> lstInvLocs,string RoomDateFormat)
        {
            List<MoveMaterialDetailDTO> lstMoveMaterialDtlDTO = new List<MoveMaterialDetailDTO>();

            MoveMaterialDetailDTO objMoveMtrlDtlDTO = null;
            foreach (var item in lstInvLocs)
            {
                DateTime? expDate = null;
                if (item.ExpirationDate.HasValue)
                {
                    expDate = item.ExpirationDate.Value;
                }
                else if (!string.IsNullOrEmpty(item.Expiration))
                {
                    expDate = GetExpireDateFromString(item.Expiration, RoomDateFormat);
                }

                objMoveMtrlDtlDTO = new MoveMaterialDetailDTO()
                {
                    CompanyID = objDTO.CompanyID,
                    ConsignQty = item.ConsignedQuantity.GetValueOrDefault(0),
                    CustomerQty = item.CustomerOwnedQuantity.GetValueOrDefault(0),
                    CreatedBy = objDTO.CreatedBy,
                    CreatedOn = DateTime.Now,
                    ExpireDate = expDate,
                    GUID = Guid.Empty,
                    ID = 0,
                    ItemLocationDetailsGUID_Source = item.GUID,
                    LotNumber = item.LotNumber,
                    RoomID = objDTO.RoomID,
                    SerialNumber = item.SerialNumber,
                    TotalQuanity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0),
                    UpdatedBy = objDTO.UpdatedBy,
                    UpdatedOn = DateTime.Now,
                };

                lstMoveMaterialDtlDTO.Add(objMoveMtrlDtlDTO);
            }


            return lstMoveMaterialDtlDTO;
        }



        #endregion

        #region ****** Methods for Inventory To Staging Location ******

        private void MoveInvetoryToStagingLocation(MoveMaterialDTO objDTO, long SessionUserId,string RoomDateFormat,long EnterpriseId)
        {
            List<ItemLocationDetailsDTO> lstSourceInvLocs = GetSourceLocationsFromGeneralInventory(objDTO);
            List<MaterialStagingPullDetailDTO> lstDestInvLocs = GetDestinationStagingPullDetail(objDTO, lstSourceInvLocs);
            MaterialStagingDetailDTO objMSDetail = GetDestinationStagingDetail(objDTO);
            UpdateSourceAndDestinationLocationQty(objDTO, lstSourceInvLocs, lstDestInvLocs);
            List<MoveMaterialDetailDTO> lstMoveMaterialDtlDTO = GetMoveMaterialDeteailsDTO(objDTO, lstDestInvLocs, RoomDateFormat);

            #region Save and Insert Source And Destination Location

            objItmLocDtlsDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            objMSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);

            int CountSource = lstSourceInvLocs.Count;
            int CountDesting = lstDestInvLocs.Count;
            int CountMoveMtrDtl = lstMoveMaterialDtlDTO.Count;

            if (CountSource == CountDesting && CountDesting == CountMoveMtrDtl)
            {
                if (objMSDetail != null)
                {
                    objMSDetail.AddedFrom = "Web";
                    objMSDetail.EditedFrom = "Web";
                    if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                        objMSDetail.AddedFrom = objDTO.AddedFrom;
                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        objMSDetail.EditedFrom = objDTO.EditedFrom;

                    objMSDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMSDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                }
                objMSDtlDAL.Insert(objMSDetail);
                for (int i = 0; i < CountSource; i++)
                {
                    lstSourceInvLocs[i].Updated = DateTimeUtility.DateTimeNow;
                    lstSourceInvLocs[i].LastUpdatedBy = objDTO.UpdatedBy;
                    lstSourceInvLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    lstSourceInvLocs[i].EditedFrom = "Web";

                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        lstSourceInvLocs[i].EditedFrom = objDTO.EditedFrom;

                    lstSourceInvLocs[i].IsOnlyFromUI = true;
                    objItmLocDtlsDAL.Edit(lstSourceInvLocs[i]);

                    lstDestInvLocs[i].MaterialStagingdtlGUID = objMSDetail.GUID;
                    lstDestInvLocs[i].AddedFrom = "Web-Move";
                    lstDestInvLocs[i].EditedFrom = "Web-Move";

                    if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                        lstDestInvLocs[i].AddedFrom = objDTO.AddedFrom;
                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        lstDestInvLocs[i].EditedFrom = objDTO.EditedFrom;


                    lstDestInvLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    lstDestInvLocs[i].ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objMSPullDtlDAL.Insert(lstDestInvLocs[i]);

                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Source = lstSourceInvLocs[i].GUID;
                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Destination = lstDestInvLocs[i].GUID;
                }

                objItemLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                ItemLocationQTYDTO objItemLocQtyDTOS = objItemLocQtyDAL.GetRecordByBinItem(objDTO.ItemGUID, objDTO.SourceBinID, objDTO.RoomID, objDTO.CompanyID);
                List<ItemLocationQTYDTO> listItemLocQty = new List<ItemLocationQTYDTO>();
                if (objItemLocQtyDTOS != null)
                {
                    objItemLocQtyDTOS.AddedFrom = "Web";
                    objItemLocQtyDTOS.EditedFrom = "Web";

                    if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                        objItemLocQtyDTOS.AddedFrom = objDTO.AddedFrom;
                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        objItemLocQtyDTOS.EditedFrom = objDTO.EditedFrom;


                    objItemLocQtyDTOS.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItemLocQtyDTOS.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                listItemLocQty.Add(objItemLocQtyDTOS);
                objItemLocQtyDAL.Save(listItemLocQty, SessionUserId, EnterpriseId);

                List<MaterialStagingPullDetailDTO> msPullDetailList = objMSPullDtlDAL.GetMsPullDetailsByItemGUID(objDTO.ItemGUID, objDTO.RoomID, objDTO.CompanyID);
                objItemDAL = new ItemMasterDAL(base.DataBaseName);
                ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);
                if (objItemDTO != null)
                {
                    objItemDTO.StagedQuantity = msPullDetailList.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    objItemDTO.WhatWhereAction = "Update Stage Qty";
                    objItemDAL.Edit(objItemDTO, SessionUserId,EnterpriseId);
                }
                if (objDTO != null)
                {

                    if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                        objDTO.AddedFrom = objDTO.AddedFrom;
                    else
                        objDTO.AddedFrom = "Web";

                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        objDTO.EditedFrom = objDTO.EditedFrom;
                    else
                        objDTO.EditedFrom = "Web";



                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                InsertMoveMaterial(objDTO);
                foreach (var item in lstMoveMaterialDtlDTO)
                {
                    item.MoveMaterialGuid = objDTO.GUID;
                    item.AddedFrom = "Web";
                    item.EditedFrom = "Web";

                    if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                        item.AddedFrom = objDTO.AddedFrom;
                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        item.EditedFrom = objDTO.EditedFrom;

                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    InsertMoveMaterialDetail(item);
                }
            }

            #endregion

        }

        private List<MaterialStagingPullDetailDTO> GetDestinationStagingPullDetail(MoveMaterialDTO objDTO, List<ItemLocationDetailsDTO> SourceInvLocList)
        {
            List<MaterialStagingPullDetailDTO> lstDestInvLocs = new List<MaterialStagingPullDetailDTO>();
            foreach (var item in SourceInvLocList)
            {
                MaterialStagingPullDetailDTO obj = new MaterialStagingPullDetailDTO();
                obj.ActualAvailableQuantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                obj.BinID = item.BinID;
                obj.CompanyID = objDTO.CompanyID;
                obj.ConsignedQuantity = 0;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.DateCodeTracking = item.DateCodeTracking;
                obj.Expiration = item.Expiration;
                obj.ExpirationDate = item.ExpirationDate;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.ItemCost = item.Cost;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemLocationDetailGUID = item.GUID;
                obj.ItemNumber = item.ItemNumber;
                obj.LastUpdatedBy = objDTO.UpdatedBy;
                obj.LotNumber = item.LotNumber;
                obj.LotNumberTracking = item.LotNumberTracking;
                obj.MaterialStagingdtlGUID = null;
                obj.MaterialStagingGUID = objDTO.DestStagingHeaderGuid;
                obj.Received = item.Received;// DateTime.Now.ToString("MM/dd/yyyy");
                obj.ReceivedDate = item.ReceivedDate;
                obj.Room = objDTO.RoomID;
                obj.SerialNumber = item.SerialNumber;
                obj.SerialNumberTracking = item.SerialNumberTracking;
                obj.StagingBinId = objDTO.DestBinID;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.PoolQuantity = 0;
                obj.OrderDetailGUID = null;
                obj.RoomName = string.Empty;
                obj.PullCredit = string.Empty;
                obj.UpdatedByName = string.Empty;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.BinNumber = string.Empty;
                obj.GUID = Guid.NewGuid();
                obj.ID = 0;
                obj.CreatedByName = string.Empty;
                obj.CustomerOwnedQuantity = 0;

                lstDestInvLocs.Add(obj);

            }

            return lstDestInvLocs;
        }

        private MaterialStagingDetailDTO GetDestinationStagingDetail(MoveMaterialDTO objDTO)
        {
            MaterialStagingDetailDTO obj = new MaterialStagingDetailDTO();
            obj.BinID = objDTO.SourceBinID;
            obj.StagingBinID = objDTO.DestBinID;
            obj.MaterialStagingGUID = objDTO.DestStagingHeaderGuid;
            obj.CreatedBy = objDTO.CreatedBy;
            obj.LastUpdatedBy = objDTO.UpdatedBy;
            obj.RoomId = objDTO.RoomID;
            obj.CompanyID = objDTO.CompanyID;
            obj.ItemGUID = objDTO.ItemGUID;
            obj.Quantity = objDTO.MoveQuanity;
            obj.ItemNumber = objDTO.ItemNumber;
            obj.Created = DateTimeUtility.DateTimeNow; obj.Updated = DateTimeUtility.DateTimeNow;
            obj.IsArchived = false; obj.IsDeleted = false;
            obj.ID = 0; obj.GUID = Guid.NewGuid();
            obj.Action = string.Empty; obj.UpdatedByName = string.Empty; obj.CreatedByName = string.Empty; obj.RoomName = "";
            return obj;

        }

        private void UpdateSourceAndDestinationLocationQty(MoveMaterialDTO objDTO, List<ItemLocationDetailsDTO> SourceLocation, List<MaterialStagingPullDetailDTO> DestinatonLocation)
        {
            double sumSourceInvLocs = SourceLocation.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
            double sumQty = 0;
            for (int i = 0; i < SourceLocation.Count; i++)
            {
                sumQty += SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) + SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                if (sumQty > objDTO.MoveQuanity)
                {
                    double diff =  objDTO.MoveQuanity;
                    if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                    {
                        if(diff <= SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0))
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = diff;
                            SourceLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity - diff;
                            diff = 0;
                        }
                        else if(diff > SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0))
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0);
                            SourceLocation[i].CustomerOwnedQuantity = 0;
                            diff = diff - DestinatonLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0);
                        }

                    }

                    if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) > 0 && diff > 0)
                    {
                        if (diff <= SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0))
                        {
                            DestinatonLocation[i].ConsignedQuantity =  diff;
                            SourceLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) - diff;
                        }
                    }
                }
                else
                {
                    DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity;
                    DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity;
                    SourceLocation[i].CustomerOwnedQuantity = 0;
                    SourceLocation[i].ConsignedQuantity = 0;
                    //SourceLocation[i].LotNumber = string.Empty;
                    //SourceLocation[i].SerialNumber = string.Empty;
                    //SourceLocation[i].Expiration = string.Empty;
                }

            }

        }

        private List<MoveMaterialDetailDTO> GetMoveMaterialDeteailsDTO(MoveMaterialDTO objDTO, List<MaterialStagingPullDetailDTO> lstInvLocs,string RoomDateFormat)
        {
            List<MoveMaterialDetailDTO> lstMoveMaterialDtlDTO = new List<MoveMaterialDetailDTO>();

            MoveMaterialDetailDTO objMoveMtrlDtlDTO = null;
            foreach (var item in lstInvLocs)
            {
                DateTime? expDate = null;
                if (item.ExpirationDate.HasValue)
                { 
                    expDate = item.ExpirationDate.Value;
                }
                else if (!string.IsNullOrEmpty(item.Expiration))
                {
                    expDate = GetExpireDateFromString(item.Expiration, RoomDateFormat);
                }

                objMoveMtrlDtlDTO = new MoveMaterialDetailDTO()
                {
                    CompanyID = objDTO.CompanyID,
                    ConsignQty = item.ConsignedQuantity.GetValueOrDefault(0),
                    CustomerQty = item.CustomerOwnedQuantity.GetValueOrDefault(0),
                    CreatedBy = objDTO.CreatedBy,
                    CreatedOn = DateTime.Now,
                    ExpireDate = expDate,
                    GUID = Guid.Empty,
                    ID = 0,
                    ItemLocationDetailsGUID_Source = item.GUID,
                    LotNumber = item.LotNumber,
                    RoomID = objDTO.RoomID,
                    SerialNumber = item.SerialNumber,
                    TotalQuanity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0),
                    UpdatedBy = objDTO.UpdatedBy,
                    UpdatedOn = DateTime.Now,
                };

                lstMoveMaterialDtlDTO.Add(objMoveMtrlDtlDTO);
            }


            return lstMoveMaterialDtlDTO;
        }

        #endregion

        #region ****** Methods for Staging To Inventory Location ******

        private void MoveStagingToInvetoryLocation(MoveMaterialDTO objDTO, long SessionUserId,string RoomDateFormat,long EnterpriseId)
        {
            List<MaterialStagingPullDetailDTO> lstSourceStageLocs = GetSourceLocationsFromStaging(objDTO);
            List<ItemLocationDetailsDTO> lstDestinationLocs = GetDestinationInvLocsFromMSPullDtl(objDTO, lstSourceStageLocs);

            UpdateSourceAndDestinationLocationQty(objDTO, lstSourceStageLocs, lstDestinationLocs);
            List<MoveMaterialDetailDTO> lstMoveMaterialDtlDTO = GetMoveMaterialDeteailsDTO(objDTO, lstDestinationLocs, RoomDateFormat);

            #region Save and Insert Source And Destination Location

            objItmLocDtlsDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            objMSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);

            int CountSource = lstSourceStageLocs.Count;
            int CountDesting = lstDestinationLocs.Count;
            int CountMoveMtrDtl = lstMoveMaterialDtlDTO.Count;

            if (CountSource == CountDesting && CountDesting == CountMoveMtrDtl)
            {

                for (int i = 0; i < CountSource; i++)
                {
                    lstSourceStageLocs[i].Updated = DateTimeUtility.DateTimeNow;
                    lstSourceStageLocs[i].LastUpdatedBy = objDTO.UpdatedBy;
                    lstSourceStageLocs[i].IsOnlyFromItemUI = true;
                    lstSourceStageLocs[i].EditedFrom = "Web-Move";
                    lstSourceStageLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMSPullDtlDAL.Edit(lstSourceStageLocs[i]);
                    lstDestinationLocs[i].AddedFrom = "Web";
                    lstDestinationLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    lstDestinationLocs[i].ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    lstDestinationLocs[i].EditedFrom = "Web";
                    objItmLocDtlsDAL.Insert(lstDestinationLocs[i]);

                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Source = lstSourceStageLocs[i].GUID;
                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Destination = lstDestinationLocs[i].GUID;
                }


                objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                //List<MaterialStagingDetailDTO> sourceMSDetailList = objMSDtlDAL.GetAllRecords(objDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty), objDTO.RoomID, objDTO.CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID && x.StagingBinID == objDTO.SourceBinID).ToList();
                List<MaterialStagingDetailDTO> sourceMSDetailList = objMSDtlDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(objDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty)), Convert.ToString(objDTO.ItemGUID), objDTO.SourceBinID, objDTO.RoomID, objDTO.CompanyID, null, null).ToList();
                foreach (var item in sourceMSDetailList)
                {
                    objMSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                    List<MaterialStagingPullDetailDTO> msPullDtlList = objMSPullDtlDAL.GetMsPullDetailsByMsDetailsId(item.GUID);
                    item.Quantity = msPullDtlList.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    item.LastUpdatedBy = objDTO.UpdatedBy;
                    item.Updated = DateTimeUtility.DateTimeNow;
                    item.IsOnlyFromItemUI = true;
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMSDtlDAL.Edit(item);
                }

                objItemLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                ItemLocationQTYDTO objItemLocQtyDTO = objItemLocQtyDAL.GetRecordByBinItem(objDTO.ItemGUID, objDTO.DestBinID, objDTO.RoomID, objDTO.CompanyID);
                List<ItemLocationQTYDTO> listItemLocQty = new List<ItemLocationQTYDTO>();

                if (objItemLocQtyDTO == null)
                {
                    objItemLocQtyDTO = new ItemLocationQTYDTO()
                    {
                        BinID = objDTO.DestBinID,
                        ItemGUID = objDTO.ItemGUID,
                        Room = objDTO.RoomID,
                        CompanyID = objDTO.CompanyID,
                        CreatedBy = objDTO.CreatedBy,
                        LastUpdatedBy = objDTO.UpdatedBy,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        EditedFrom = "Web",
                        AddedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow
                    };
                }
                listItemLocQty.Add(objItemLocQtyDTO);
                objItemLocQtyDAL.Save(listItemLocQty, SessionUserId,EnterpriseId);

                List<MaterialStagingPullDetailDTO> msPullDetailList = objMSPullDtlDAL.GetMsPullDetailsByItemGUID(objDTO.ItemGUID, objDTO.RoomID, objDTO.CompanyID);
                objItemDAL = new ItemMasterDAL(base.DataBaseName);
                ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);
                if (objItemDTO != null)
                {
                    objItemDTO.StagedQuantity = msPullDetailList.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    objItemDTO.WhatWhereAction = "Update Stage Qty";
                    objItemDAL.Edit(objItemDTO, SessionUserId,EnterpriseId);
                }
                objDTO.AddedFrom = "Web";
                objDTO.EditedFrom = "Web";
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                InsertMoveMaterial(objDTO);
                foreach (var item in lstMoveMaterialDtlDTO)
                {
                    item.MoveMaterialGuid = objDTO.GUID;
                    item.AddedFrom = "Web";
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    InsertMoveMaterialDetail(item);
                }

            }
            #endregion
        }

        private List<MaterialStagingPullDetailDTO> GetSourceLocationsFromStaging(MoveMaterialDTO objDTO)
        {
            List<MaterialStagingPullDetailDTO> lstSourceInvLocs = new List<MaterialStagingPullDetailDTO>();

            objMSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            List<MaterialStagingPullDetailDTO> MSPullDtlList = objMSPullDtlDAL.GetMsPullDetailsByItemGUIDANDBinIDForLotSr(objDTO.ItemGUID, objDTO.SourceBinID, objDTO.RoomID, objDTO.CompanyID, objDTO.LotNumber, objDTO.SerialNumber).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == objDTO.SourceStagingHeaderGuid).ToList();

            foreach (var item in MSPullDtlList)
            {
                if ((item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0)) > 0)
                {
                    ItemLocationDetailsDTO objItemLocDetailDTO = new ItemLocationDetailsDTO();
                    double SumSourceQty = lstSourceInvLocs.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    if (SumSourceQty < objDTO.MoveQuanity)
                    {
                        lstSourceInvLocs.Add(item);
                    }
                }
            }


            return lstSourceInvLocs;
        }

        private List<ItemLocationDetailsDTO> GetDestinationInvLocsFromMSPullDtl(MoveMaterialDTO objDTO, List<MaterialStagingPullDetailDTO> SourceInvLocList)
        {
            List<ItemLocationDetailsDTO> lstDestInvLocs = new List<ItemLocationDetailsDTO>();
            foreach (var item in SourceInvLocList)
            {
                ItemLocationDetailsDTO obj = new ItemLocationDetailsDTO();

                obj.BinID = objDTO.DestBinID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.UpdatedBy;
                obj.Room = objDTO.RoomID;
                obj.CompanyID = objDTO.CompanyID;

                obj.ItemGUID = item.ItemGUID;
                obj.ConsignedQuantity = 0;//item.ConsignedQuantity;
                obj.CustomerOwnedQuantity = 0;// item.CustomerOwnedQuantity;
                obj.Expiration = item.Expiration;
                obj.ExpirationDate = item.ExpirationDate;
                obj.SerialNumber = item.SerialNumber;
                obj.LotNumber = item.LotNumber;
                obj.Cost = item.ItemCost;
                obj.Received = item.Received;
                obj.ReceivedDate = item.ReceivedDate;

                obj.Markup = null; obj.SellPrice = null;
                obj.DateCodeTracking = item.DateCodeTracking; obj.SerialNumberTracking = item.SerialNumberTracking; obj.LotNumberTracking = item.LotNumberTracking;
                obj.CriticalQuantity = null; obj.SuggestedOrderQuantity = null; obj.MaximumQuantity = null; obj.MeasurementID = null; obj.MinimumQuantity = null;
                obj.TransferDetailGUID = null;
                //obj.OrderDetailGUID = item.OrderDetailGUID;
                obj.ProjectSpentGUID = null; obj.KitDetailGUID = null;
                obj.ItemNumber = item.ItemNumber; obj.ItemType = 0; obj.BinNumber = string.Empty;
                obj.Created = DateTimeUtility.DateTimeNow; obj.Updated = DateTimeUtility.DateTimeNow;
                obj.IsArchived = false; obj.IsDeleted = false; obj.IsCreditPull = false;

                obj.ID = 0; obj.GUID = Guid.NewGuid(); obj.HistoryID = 0;
                obj.UDF1 = string.Empty; obj.UDF2 = string.Empty; obj.UDF3 = string.Empty; obj.UDF4 = string.Empty; obj.UDF5 = string.Empty; obj.UDF6 = string.Empty; obj.UDF7 = string.Empty; obj.UDF8 = string.Empty; obj.UDF9 = string.Empty; obj.UDF10 = string.Empty;
                obj.Action = string.Empty; obj.UpdatedByName = string.Empty; obj.CreatedByName = string.Empty; obj.RoomName = "";
                obj.mode = "Move Material"; obj.eVMISensorID = string.Empty; obj.eVMISensorPort = null;


                lstDestInvLocs.Add(obj);

            }

            return lstDestInvLocs;
        }

        private void UpdateSourceAndDestinationLocationQty(MoveMaterialDTO objDTO, List<MaterialStagingPullDetailDTO> SourceLocation, List<ItemLocationDetailsDTO> DestinatonLocation)
        {
            double sumSourceInvLocs = SourceLocation.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
            double sumQty = 0;
            for (int i = 0; i < SourceLocation.Count; i++)
            {
                sumQty += SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) + SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                if (sumQty > objDTO.MoveQuanity)
                {
                    double diff = sumQty - objDTO.MoveQuanity;
                    if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                    {
                        if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) <= diff)
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity;
                            SourceLocation[i].CustomerOwnedQuantity = 0;
                            //SourceLocation[i].LotNumber = string.Empty;
                            //SourceLocation[i].SerialNumber = string.Empty;
                            //SourceLocation[i].Expiration = string.Empty;
                        }
                        else if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) > diff)
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity - diff;
                            SourceLocation[i].CustomerOwnedQuantity = diff;
                        }
                    }

                    if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) > 0)
                    {
                        if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) <= diff)
                        {
                            DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                            SourceLocation[i].ConsignedQuantity = 0;
                            //SourceLocation[i].LotNumber = string.Empty;
                            //SourceLocation[i].SerialNumber = string.Empty;
                            //SourceLocation[i].Expiration = string.Empty;
                        }
                        else if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) > diff)
                        {
                            DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity - diff;
                            SourceLocation[i].ConsignedQuantity = diff;
                        }
                    }
                }
                else
                {
                    DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity;
                    DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity;
                    SourceLocation[i].CustomerOwnedQuantity = 0;
                    SourceLocation[i].ConsignedQuantity = 0;
                    //SourceLocation[i].LotNumber = string.Empty;
                    //SourceLocation[i].SerialNumber = string.Empty;
                    //SourceLocation[i].Expiration = string.Empty;
                }

            }

        }

        #endregion

        #region ****** Methods for Staging To StagingLocation ******

        private void MoveStagingToStagingLocation(MoveMaterialDTO objDTO, long SessionUserId,string RoomDateFormat,long EnterpriseId)
        {
            List<MaterialStagingPullDetailDTO> lstSourceStageLocs = GetSourceLocationsFromStaging(objDTO);
            List<MaterialStagingPullDetailDTO> lstDestStageLocs = GetDestinationStageLocsFromMSPullDtl(objDTO, lstSourceStageLocs);
            UpdateSourceAndDestinationLocationQty(objDTO, lstSourceStageLocs, lstDestStageLocs);
            List<MoveMaterialDetailDTO> lstMoveMaterialDtlDTO = GetMoveMaterialDeteailsDTO(objDTO, lstDestStageLocs, RoomDateFormat);
            MaterialStagingDetailDTO objMSDetail = GetDestinationStagingDetail(objDTO);

            #region Save and Insert Source And Destination Location
            objMSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);

            int CountSource = lstSourceStageLocs.Count;
            int CountDesting = lstDestStageLocs.Count;
            int CountMoveMtrDtl = lstMoveMaterialDtlDTO.Count;

            if (CountSource == CountDesting && CountDesting == CountMoveMtrDtl)
            {
                if (objMSDetail != null)
                {
                    objMSDetail.AddedFrom = "Web";
                    objMSDetail.EditedFrom = "Web";
                    objMSDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMSDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                }
                objMSDtlDAL.Insert(objMSDetail);
                for (int i = 0; i < CountSource; i++)
                {
                    lstSourceStageLocs[i].Updated = DateTimeUtility.DateTimeNow;
                    lstSourceStageLocs[i].LastUpdatedBy = objDTO.UpdatedBy;
                    lstSourceStageLocs[i].EditedFrom = "Web-Move";
                    lstSourceStageLocs[i].IsOnlyFromItemUI = true;
                    lstSourceStageLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMSPullDtlDAL.Edit(lstSourceStageLocs[i]);

                    lstDestStageLocs[i].MaterialStagingdtlGUID = objMSDetail.GUID;
                    lstDestStageLocs[i].AddedFrom = "Web-Move";
                    lstDestStageLocs[i].EditedFrom = "Web-Move";
                    lstDestStageLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    lstDestStageLocs[i].ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objMSPullDtlDAL.Insert(lstDestStageLocs[i]);

                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Source = lstSourceStageLocs[i].GUID;
                    lstMoveMaterialDtlDTO[i].ItemLocationDetailsGUID_Destination = lstDestStageLocs[i].GUID;
                }

                objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                objMSPullDtlDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                //List<MaterialStagingDetailDTO> sourceMSDetailList = objMSDtlDAL.GetAllRecords(objDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty), objDTO.RoomID, objDTO.CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID && x.StagingBinID == objDTO.SourceBinID).ToList();
                List<MaterialStagingDetailDTO> sourceMSDetailList = objMSDtlDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(objDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty)), Convert.ToString(objDTO.ItemGUID), objDTO.SourceBinID, objDTO.RoomID, objDTO.CompanyID, null, null).ToList();

                foreach (var item in sourceMSDetailList)
                {
                    List<MaterialStagingPullDetailDTO> msPullDtlList = objMSPullDtlDAL.GetMsPullDetailsByMsDetailsId(item.GUID);
                    item.Quantity = msPullDtlList.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    item.LastUpdatedBy = objDTO.UpdatedBy;
                    item.Updated = DateTimeUtility.DateTimeNow;
                    item.IsOnlyFromItemUI = true;
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMSDtlDAL.Edit(item);
                }

                //List<MaterialStagingDetailDTO> DestMSDetailList = objMSDtlDAL.GetAllRecords(objDTO.DestStagingHeaderGuid.GetValueOrDefault(Guid.Empty), objDTO.RoomID, objDTO.CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID && x.StagingBinID == objDTO.DestBinID).ToList();
                List<MaterialStagingDetailDTO> DestMSDetailList = objMSDtlDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(objDTO.DestStagingHeaderGuid.GetValueOrDefault(Guid.Empty)), Convert.ToString(objDTO.ItemGUID), objDTO.DestBinID, objDTO.RoomID, objDTO.CompanyID, null, null).ToList();

                foreach (var item in DestMSDetailList)
                {
                    List<MaterialStagingPullDetailDTO> msPullDtlList = objMSPullDtlDAL.GetMsPullDetailsByMsDetailsId(item.GUID);
                    item.Quantity = msPullDtlList.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    item.LastUpdatedBy = objDTO.UpdatedBy;
                    item.Updated = DateTimeUtility.DateTimeNow;
                    item.IsOnlyFromItemUI = true;
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMSDtlDAL.Edit(item);
                }

                List<MaterialStagingPullDetailDTO> msPullDetailList = objMSPullDtlDAL.GetMsPullDetailsByItemGUID(objDTO.ItemGUID, objDTO.RoomID, objDTO.CompanyID);
                objItemDAL = new ItemMasterDAL(base.DataBaseName);
                ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);
                if (objItemDTO != null)
                {
                    objItemDTO.StagedQuantity = msPullDetailList.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    objItemDTO.WhatWhereAction = "Update Stage Qty";
                    objItemDAL.Edit(objItemDTO, SessionUserId,EnterpriseId);
                }
                objDTO.AddedFrom = "Web";
                objDTO.EditedFrom = "Web";
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                InsertMoveMaterial(objDTO);
                foreach (var item in lstMoveMaterialDtlDTO)
                {
                    item.MoveMaterialGuid = objDTO.GUID;
                    item.AddedFrom = "Web";
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    InsertMoveMaterialDetail(item);
                }

            }
            #endregion


        }

        private void UpdateSourceAndDestinationLocationQty(MoveMaterialDTO objDTO, List<MaterialStagingPullDetailDTO> SourceLocation, List<MaterialStagingPullDetailDTO> DestinatonLocation)
        {
            double sumSourceInvLocs = SourceLocation.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
            double sumQty = 0;
            for (int i = 0; i < SourceLocation.Count; i++)
            {
                sumQty += SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) + SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                if (sumQty > objDTO.MoveQuanity)
                {
                    double diff = sumQty - objDTO.MoveQuanity;
                    if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                    {
                        if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) <= diff)
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity;
                            SourceLocation[i].CustomerOwnedQuantity = 0;
                            //SourceLocation[i].LotNumber = string.Empty;
                            //SourceLocation[i].SerialNumber = string.Empty;
                            //SourceLocation[i].Expiration = string.Empty;
                        }
                        else if (SourceLocation[i].CustomerOwnedQuantity.GetValueOrDefault(0) > diff)
                        {
                            DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity - diff;
                            SourceLocation[i].CustomerOwnedQuantity = diff;
                        }
                    }

                    if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) > 0)
                    {
                        if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) <= diff)
                        {
                            DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0);
                            SourceLocation[i].ConsignedQuantity = 0;
                            //SourceLocation[i].LotNumber = string.Empty;
                            //SourceLocation[i].SerialNumber = string.Empty;
                            //SourceLocation[i].Expiration = string.Empty;
                        }
                        else if (SourceLocation[i].ConsignedQuantity.GetValueOrDefault(0) > diff)
                        {
                            DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity - diff;
                            SourceLocation[i].ConsignedQuantity = diff;
                        }
                    }
                }
                else
                {
                    DestinatonLocation[i].CustomerOwnedQuantity = SourceLocation[i].CustomerOwnedQuantity;
                    DestinatonLocation[i].ConsignedQuantity = SourceLocation[i].ConsignedQuantity;
                    SourceLocation[i].CustomerOwnedQuantity = 0;
                    SourceLocation[i].ConsignedQuantity = 0;
                    //SourceLocation[i].LotNumber = string.Empty;
                    //SourceLocation[i].SerialNumber = string.Empty;
                    //SourceLocation[i].Expiration = string.Empty;
                }

            }

        }

        private List<MaterialStagingPullDetailDTO> GetDestinationStageLocsFromMSPullDtl(MoveMaterialDTO objDTO, List<MaterialStagingPullDetailDTO> SourceInvLocList)
        {
            List<MaterialStagingPullDetailDTO> lstDestInvLocs = new List<MaterialStagingPullDetailDTO>();
            foreach (var item in SourceInvLocList)
            {
                MaterialStagingPullDetailDTO obj = new MaterialStagingPullDetailDTO();
                obj.ActualAvailableQuantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                obj.CompanyID = objDTO.CompanyID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.LastUpdatedBy = objDTO.UpdatedBy;
                obj.MaterialStagingdtlGUID = null;
                obj.MaterialStagingGUID = objDTO.DestStagingHeaderGuid;
                obj.StagingBinId = objDTO.DestBinID;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.BinNumber = string.Empty;
                obj.GUID = Guid.NewGuid();
                obj.Created = DateTimeUtility.DateTimeNow;

                obj.BinID = item.BinID;
                obj.ConsignedQuantity = 0;
                obj.CustomerOwnedQuantity = 0;
                obj.DateCodeTracking = item.DateCodeTracking;
                obj.Expiration = item.Expiration;
                obj.ExpirationDate = item.ExpirationDate;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.ItemCost = item.ItemCost;
                obj.ItemLocationDetailGUID = item.GUID;
                obj.ItemNumber = item.ItemNumber;
                obj.LotNumber = item.LotNumber;
                obj.LotNumberTracking = item.LotNumberTracking;
                obj.Received = item.Received;// DateTime.Now.ToString("MM/dd/yyyy");
                obj.ReceivedDate = item.ReceivedDate;
                obj.Room = objDTO.RoomID;
                obj.SerialNumber = item.SerialNumber;
                obj.SerialNumberTracking = item.SerialNumberTracking;
                obj.PoolQuantity = 0;
                obj.OrderDetailGUID = item.OrderDetailGUID;
                obj.RoomName = string.Empty;
                obj.PullCredit = string.Empty;
                obj.UpdatedByName = string.Empty;

                obj.ID = 0;
                obj.CreatedByName = string.Empty;

                lstDestInvLocs.Add(obj);

            }

            return lstDestInvLocs;
        }

        #endregion

        /// <summary>
        /// InsertMoveMaterial
        /// </summary>
        /// <param name="objMove"></param>
        /// <returns></returns>
        public Int64 InsertMoveMaterial(MoveMaterialDTO objMove)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MoveMaterialMaster obj = new MoveMaterialMaster()
                {

                    DestBinID = objMove.DestBinID,
                    DestStagingHeaderGuid = objMove.DestStagingHeaderGuid,
                    ItemGUID = objMove.ItemGUID,
                    MoveQuanity = objMove.MoveQuanity,
                    MoveType = objMove.MoveType,

                    SourceBinID = objMove.SourceBinID,
                    SourceStagingHeaderGuid = objMove.SourceStagingHeaderGuid,

                    RoomID = objMove.RoomID,
                    CompanyID = objMove.CompanyID,
                    UpdatedBy = objMove.UpdatedBy,
                    CreatedBy = objMove.CreatedBy,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    GUID = Guid.NewGuid(),
                    IsArchived = false,
                    IsDeleted = false,
                    ID = 0,
                    AddedFrom = (objMove.AddedFrom == null ? "Web" : objMove.AddedFrom),
                    EditedFrom = (objMove.EditedFrom == null ? "Web" : objMove.EditedFrom),
                    ReceivedOn = objMove.ReceivedOn,
                    ReceivedOnWeb = objMove.ReceivedOnWeb

                };
                context.MoveMaterialMasters.Add(obj);
                context.SaveChanges();
                objMove.ID = obj.ID;
                objMove.GUID = obj.GUID;

            }

            return objMove.ID;
        }

        /// <summary>
        /// InsertMoveMaterialDetail
        /// </summary>
        /// <param name="objMoveDtl"></param>
        /// <returns></returns>
        public Int64 InsertMoveMaterialDetail(MoveMaterialDetailDTO objMoveDtl)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MoveMaterialDetail obj = new MoveMaterialDetail()
                {
                    ConsignQty = objMoveDtl.ConsignQty,
                    CustomerQty = objMoveDtl.CustomerQty,
                    TotalQuanity = objMoveDtl.TotalQuanity,
                    ItemLocationDetailsGUID_Destination = objMoveDtl.ItemLocationDetailsGUID_Destination,
                    ItemLocationDetailsGUID_Source = objMoveDtl.ItemLocationDetailsGUID_Source,
                    MoveMaterialGuid = objMoveDtl.MoveMaterialGuid,
                    SerialNumber = objMoveDtl.SerialNumber,
                    LotNumber = objMoveDtl.LotNumber,
                    ExpireDate = objMoveDtl.ExpireDate,
                    RoomID = objMoveDtl.RoomID,
                    CompanyID = objMoveDtl.CompanyID,
                    UpdatedBy = objMoveDtl.UpdatedBy,
                    CreatedBy = objMoveDtl.CreatedBy,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    GUID = Guid.NewGuid(),
                    IsArchived = false,
                    IsDeleted = false,
                    ID = 0,
                    AddedFrom = (objMoveDtl.AddedFrom == null ? "Web" : objMoveDtl.AddedFrom),
                    EditedFrom = (objMoveDtl.EditedFrom == null ? "Web" : objMoveDtl.EditedFrom),
                    ReceivedOn = objMoveDtl.ReceivedOn,
                    ReceivedOnWeb = objMoveDtl.ReceivedOnWeb
                };
                context.MoveMaterialDetails.Add(obj);
                context.SaveChanges();
                objMoveDtl.ID = obj.ID;
            }

            return objMoveDtl.ID;
        }

        public IEnumerable<MoveMaterialDTO> GetPagedRecordsNew(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID,List<long> SupplierIds, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<MoveMaterialDTO> lstItems = new List<MoveMaterialDTO>();
            TotalCount = 0;
            //ItemMasterDTO objItemDTO = new ItemMasterDTO();

            string MoveType = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                sortColumnName = sortColumnName.Replace("CreatedOnDate", "CreatedOn").Replace("ReceivedOnDateWeb", "ReceivedOnWeb");
            }
            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedMoveMaterialItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, MoveType, IsDeleted, IsArchived, RoomID, CompanyID, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, strSupplierIds);
            }
            else if (SearchTerm.Contains("[###]"))
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[80]))
                {
                    MoveType = FieldsPara[80].TrimEnd(',');
                }

                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedMoveMaterialItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, MoveType, IsDeleted, IsArchived, RoomID, CompanyID, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo,strSupplierIds);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedMoveMaterialItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, MoveType, IsDeleted, IsArchived, RoomID, CompanyID, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo,strSupplierIds);
            }
            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new MoveMaterialDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ItemGUID = row.Field<Guid>("ItemGUID"),
                        SourceBinID = row.Field<long>("SourceBinID"),
                        SourceStagingHeaderGuid = row.Field<Guid?>("SourceStagingHeaderGuid"),
                        MoveQuanity = row.Field<double>("MoveQuanity"),
                        MoveType = row.Field<int>("MoveType"),
                        DestBinID = row.Field<long>("DestBinID"),
                        DestStagingHeaderGuid = row.Field<Guid?>("DestStagingHeaderGuid"),
                        SourceLocation = row.Field<string>("SourceLocation") != null ? row.Field<string>("SourceLocation").Replace("[|EmptyStagingBin|]", "") : string.Empty,
                        SourceStagingHeader = row.Field<string>("SourceStagingHeader"),
                        DestinationLocation = row.Field<string>("DestinationLocation") != null ? row.Field<string>("DestinationLocation").Replace("[|EmptyStagingBin|]", "") : string.Empty,
                        DestinationStagingHeader = row.Field<string>("DestinationStagingHeader"),
                        GUID = row.Field<Guid>("GUID"),
                        IsDeleted = (row.Field<bool>("IsDeleted")),
                        IsArchived = (row.Field<bool>("IsArchived")),
                        CompanyID = row.Field<long>("CompanyID"),
                        RoomID = row.Field<long>("RoomID"),
                        CreatedOn = row.Field<DateTime>("CreatedOn"),
                        UpdatedOn = row.Field<DateTime>("UpdatedOn"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                    }).ToList();
                }
            }
            return lstItems;
        }

    }
}
