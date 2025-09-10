using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace eTurns.DAL
{
    public class ToolAssetCICOTransactionDAL : eTurnsBaseDAL
    {
        #region [Class constructor]

        public ToolAssetCICOTransactionDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolAssetCICOTransactionDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public List<ToolLocationLotSerialDTO> GetItemLocationsLotSerials(Guid ItemGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, string PullQuantityLimit)
        {
            List<ToolLocationLotSerialDTO> lstItemLocations = new List<ToolLocationLotSerialDTO>();
            //ItemMasterDTO im = new ItemMasterDAL(base.DataBaseName).GetRecord(ItemGUID.ToString(), RoomID, CompanyID);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ToolLocationLotSerialDTO>(@"EXEC GetLotSerialsByTool '" + ItemGUID.ToString() + "'," + BinID + "," + PullQuantity + "," + CompanyID + "," + RoomID + "," + PullQuantityLimit)
                                    select new ToolLocationLotSerialDTO
                                    {
                                        Location = il.Location,
                                        DateCodeTracking = il.DateCodeTracking,
                                        IsCreditPull = false,
                                        ToolName = il.ToolName,
                                        ToolType = il.ToolType,
                                        LotNumberTracking = il.LotNumberTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        WorkOrderGUID = Guid.Empty,
                                        //BinID = il.BinID,
                                        Quantity = il.Quantity,
                                        Cost = il.Cost,

                                        Expiration = il.Expiration,
                                        ExpirationDate = il.ExpirationDate,
                                        GUID = il.GUID,
                                        ID = il.ID,
                                        ToolGUID = il.ToolGUID,
                                        //LotNumber = il.LotNumber,
                                        //OrderDetailGUID = il.OrderDetailGUID,
                                        Received = il.Received,
                                        ReceivedDate = il.ReceivedDate,
                                        Room = il.Room,
                                        SerialNumber = il.SerialNumber,
                                        //TransferDetailGUID = il.TransferDetailGUID,
                                        //IsConsignedLotSerial = (il.Quantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.Quantity ?? 0) > 0 ? (il.Quantity ?? 0) : 0,
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.Quantity ?? 0) > 0 ? (il.Quantity ?? 0) : 0,
                                        //CumulativeTotalQuantity = il.CumulativeTotalQuantity,
                                        //ItemLocationDetailGUID = il.ItemLocationDetailGUID
                                    }).ToList();
                return lstItemLocations;
            }

        }
        public List<ToolAssetPullMasterDTO> GetPullWithDetails(List<ToolAssetPullMasterDTO> lstPullInfo, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                Room objRoom = context.Rooms.FirstOrDefault(p => p.ID == RoomId);
                if (lstPullInfo != null)
                {
                    lstPullInfo.ForEach(t =>
                    {
                        ToolMaster objItem = context.ToolMasters.FirstOrDefault(p => p.GUID == t.ToolGUID);
                        LocationMaster objBin = new LocationMaster();
                        ToolLocationDetail objToolLocationDetail = new ToolLocationDetail();


                        if (t.BinID.HasValue && (t.BinID ?? 0) > 0)
                        {
                            objToolLocationDetail = context.ToolLocationDetails.FirstOrDefault(p => p.ID == t.BinID);
                            objBin = context.LocationMasters.FirstOrDefault(p => p.GUID == objToolLocationDetail.LocationGuid);
                            if (objBin == null)
                            {
                                objBin = new LocationMaster();
                            }
                        }

                        if (objItem != null)
                        {
                            t.SerialNumberTracking = objItem.SerialNumberTracking;
                            //t.LotNumberTracking = objItem.LotNumberTracking;
                            //t.DateCodeTracking = objItem.DateCodeTracking;
                            t.ToolTypeTracking = objItem.ToolTypeTracking;
                            t.Quantity = objItem.AvailableToolQty;
                            t.ToolName = objItem.ToolName;

                            t.Location = objBin.Location;

                            //t.lstItemLocationDetails = GetItemLocationsDetails(t.ItemGUID ?? Guid.Empty, t.Room ?? 0, t.BinID ?? 0, t.PoolQuantity ?? 0, t.InventoryConsuptionMethod).ToList();
                        }

                    });
                }
                if (lstPullInfo == null)
                {
                    lstPullInfo = new List<ToolAssetPullMasterDTO>();
                }
                return lstPullInfo;

            }

        }
        public List<ToolQuantityLotSerialDTO> GetToolLocationsWithLotSerialsForPull(Guid ToolGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, string PullQuantityLimit, string SerialOrLotNumber)
        {
            List<ToolQuantityLotSerialDTO> lstItemLocations = new List<ToolQuantityLotSerialDTO>();
            //ItemMasterDTO im = new ItemMasterDAL(base.DataBaseName).GetRecord(ItemGUID.ToString(), RoomID, CompanyID);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ToolQuantityLotSerialDTO>(@"EXEC GetToolLocationsWithLotSerialsForPull '" + ToolGUID.ToString() + "'," + BinID + "," + PullQuantity + "," + CompanyID + "," + RoomID + "," + PullQuantityLimit + ",'" + SerialOrLotNumber + "'")
                                    select new ToolQuantityLotSerialDTO
                                    {
                                        ToolGUID = il.ToolGUID,
                                        IsCreditPull = false,
                                        Location = il.Location,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        Quantity = il.Quantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true, true),
                                        ExpirationDate = il.ExpirationDate,

                                        LotSerialQuantity = (il.Quantity == null ? 0 : (double)il.Quantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.Quantity == null ? 0 : (double)il.Quantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate

                                    }).ToList();
                return lstItemLocations;
            }
        }
        public List<ToolQuantityLotSerialDTO> GetToolLocationsWithLotSerials(Guid ToolGUID, long RoomID, long CompanyID, Guid LocationGUID)
        {
            List<ToolQuantityLotSerialDTO> lstItemLocations = new List<ToolQuantityLotSerialDTO>();
            //ItemMasterDTO im = new ItemMasterDAL(base.DataBaseName).GetRecord(ItemGUID.ToString(), RoomID, CompanyID);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ToolQuantityLotSerialDTO>(@"EXEC GetToolLocationsWithLotSerials '" + ToolGUID.ToString() + "','" + LocationGUID.ToString() + "'," + CompanyID + "," + RoomID + "")
                                    select new ToolQuantityLotSerialDTO
                                    {
                                        ToolGUID = il.ToolGUID,
                                        IsCreditPull = false,
                                        Location = il.Location,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.ID,
                                        Quantity = il.Quantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true, true),
                                        ExpirationDate = il.ExpirationDate,

                                        LotSerialQuantity = (il.Quantity == null ? 0 : (double)il.Quantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.Quantity == null ? 0 : (double)il.Quantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate

                                    }).ToList();
                return lstItemLocations;
            }
        }
        public List<ToolQuantityLotSerialDTO> GetToolLocationsWithLotSerialsForOut(Guid ToolGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, string PullQuantityLimit, string SerialOrLotNumber)
        {
            List<ToolQuantityLotSerialDTO> lstItemLocations = new List<ToolQuantityLotSerialDTO>();
            //ItemMasterDTO im = new ItemMasterDAL(base.DataBaseName).GetRecord(ItemGUID.ToString(), RoomID, CompanyID);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ToolQuantityLotSerialDTO>(@"EXEC GetToolLocationsWithLotSerialsForPullOut '" + ToolGUID.ToString() + "'," + BinID + "," + PullQuantity + "," + CompanyID + "," + RoomID + "," + PullQuantityLimit + ",'" + SerialOrLotNumber + "'")
                                    select new ToolQuantityLotSerialDTO
                                    {
                                        ToolGUID = il.ToolGUID,
                                        IsCreditPull = false,
                                        Location = il.Location,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        // BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        Quantity = il.Quantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true, true),
                                        ExpirationDate = il.ExpirationDate,

                                        LotSerialQuantity = (il.Quantity == null ? 0 : (double)il.Quantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.Quantity == null ? 0 : (double)il.Quantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate

                                    }).ToList();
                return lstItemLocations;
            }
        }

        #endregion
    }
}
