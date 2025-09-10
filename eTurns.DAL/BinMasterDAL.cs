using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace eTurns.DAL
{
    public partial class BinMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]
        public BinMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public BinMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<BinMasterDTO> GetAllRecordsByItemLocationLevelQuanity(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUID)
        {
            Guid vitemGUID = Guid.Empty;
            Guid.TryParse(ItemGUID, out vitemGUID);
            List<BinMasterDTO> lstItemBins = GetInventoryAndStagingBinsByItem(RoomID, CompanyID, vitemGUID);
            return (from u in lstItemBins
                    select new BinMasterDTO
                    {
                        ID = u.ID,
                        BinNumber = ((u.BinNumber ?? string.Empty).Equals("[|EmptyStagingBin|]") ? string.Empty : u.BinNumber),
                        Created = u.Created,
                        LastUpdated = u.LastUpdated,
                        IsStagingLocation = u.IsStagingLocation,
                        IsStagingHeader = u.IsStagingHeader,
                        MaterialStagingGUID = u.MaterialStagingGUID,
                        CreatedByName = u.CreatedByName,
                        UpdatedByName = u.UpdatedByName,
                        RoomName = u.RoomName,
                        CreatedBy = u.CreatedBy,
                        LastUpdatedBy = u.LastUpdatedBy,
                        Room = u.Room,
                        CompanyID = u.CompanyID,
                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                        GUID = u.GUID,
                        UDF1 = u.UDF1,
                        UDF2 = u.UDF2,
                        UDF3 = u.UDF3,
                        UDF4 = u.UDF4,
                        UDF5 = u.UDF5,
                        AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                        EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                        ReceivedOn = (u.ReceivedOn == null ? DateTimeUtility.DateTimeNow : u.ReceivedOn),
                        ReceivedOnWeb = (u.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : u.ReceivedOnWeb),
                        MinimumQuantity = u.MinimumQuantity,
                        MaximumQuantity = u.MaximumQuantity,
                        CriticalQuantity = u.CriticalQuantity,
                        ItemGUID = u.ItemGUID,
                        IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                        IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                        DefaultPullQuantity = u.DefaultPullQuantity,
                        DefaultReorderQuantity = u.DefaultReorderQuantity
                    }).ToList();
        }

        public List<BinMasterDTO> GetAllRecordsByItemLocationLevelQuanity(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUID, bool? IsStagingLocation, string BinNameStart, bool? IsBinNumberNull, bool? EmptyStagingBin)
        {
            Guid vitemGUID = Guid.Empty;
            Guid.TryParse(ItemGUID, out vitemGUID);
            List<BinMasterDTO> lstItemBins = new List<BinMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@RoomID", RoomID),
                                                    new SqlParameter("@CompanyID", CompanyID),
                                                    new SqlParameter("@ItemGUID", vitemGUID),
                                                    new SqlParameter("@IsStagingLocation", IsStagingLocation ?? (object)DBNull.Value),
                                                    new SqlParameter("@BinNameStart", BinNameStart),
                                                    new SqlParameter("@IsBinNumberNull", IsBinNumberNull ?? (object)DBNull.Value),
                                                    new SqlParameter("@EmptyStagingBin", EmptyStagingBin ?? (object)DBNull.Value) };

                lstItemBins = (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetInventoryAndStagingBinsByItemGUID] @RoomID,@CompanyID,@ItemGUID,@IsStagingLocation,@BinNameStart,@IsBinNumberNull,@EmptyStagingBin", params1)
                               select new BinMasterDTO
                               {
                                   ID = u.ID,
                                   BinNumber = ((u.BinNumber ?? string.Empty).Equals("[|EmptyStagingBin|]") ? string.Empty : u.BinNumber),
                                   Created = u.Created,
                                   LastUpdated = u.LastUpdated,
                                   IsStagingLocation = u.IsStagingLocation,
                                   IsStagingHeader = u.IsStagingHeader,
                                   MaterialStagingGUID = u.MaterialStagingGUID,
                                   CreatedByName = u.CreatedByName,
                                   UpdatedByName = u.UpdatedByName,
                                   RoomName = u.RoomName,
                                   CreatedBy = u.CreatedBy,
                                   LastUpdatedBy = u.LastUpdatedBy,
                                   Room = u.Room,
                                   CompanyID = u.CompanyID,
                                   IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                   IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                   GUID = u.GUID,
                                   UDF1 = u.UDF1,
                                   UDF2 = u.UDF2,
                                   UDF3 = u.UDF3,
                                   UDF4 = u.UDF4,
                                   UDF5 = u.UDF5,
                                   AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                   EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                   ReceivedOn = (u.ReceivedOn == null ? DateTimeUtility.DateTimeNow : u.ReceivedOn),
                                   ReceivedOnWeb = (u.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : u.ReceivedOnWeb),
                                   MinimumQuantity = u.MinimumQuantity,
                                   MaximumQuantity = u.MaximumQuantity,
                                   CriticalQuantity = u.CriticalQuantity,
                                   ItemGUID = u.ItemGUID,
                                   IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                   IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                   DefaultPullQuantity = u.DefaultPullQuantity,
                                   DefaultReorderQuantity = u.DefaultReorderQuantity
                               }).ToList();

            }
            return lstItemBins;

        }

        public List<BinMasterDTO> GetStagingBinMasterByRoomID(Int64 RoomID, Int64 CompanyID)
        {
            List<BinMasterDTO> lstMasterStagingBins = GetStagingBinMasterByRoom(RoomID, CompanyID);
            lstMasterStagingBins.ForEach(t =>
            {
                t.BinNumber = t.BinNumber ?? string.Empty;
                t.AddedFrom = (t.AddedFrom == null ? "Web" : t.AddedFrom);
                t.EditedFrom = (t.EditedFrom == null ? "Web" : t.EditedFrom);
                t.ReceivedOn = (t.ReceivedOn == null ? DateTimeUtility.DateTimeNow : t.ReceivedOn);
                t.ReceivedOnWeb = (t.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : t.ReceivedOnWeb);
            });
            return lstMasterStagingBins;
        }

        public BinMasterDTO GetDefaultStagingBinForRequisition(Int64 RoomID, Int64 CompanyID)
        {
            return GetStagingBinMasterByRoomID(RoomID, CompanyID).OrderBy(t => t.BinNumber).FirstOrDefault();
        }

        public BinMasterDTO GetBinByID(Int64 id, Int64 RoomID, Int64 CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("EXEC GetBinByID @ID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public BinMasterDTO GetBinByIDPlain(long id, long RoomID, long CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("EXEC GetBinByIDPlain @ID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public BinMasterDTO GetBinByGUID(Guid BinGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinGUID", BinGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinByGUID] @BinGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<BinMasterDTO> GetItemBinQuantityDict(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            ItemLocationDetailsDAL objdal = new ItemLocationDetailsDAL(base.DataBaseName);
            IEnumerable<ItemLocationDetailsDTO> objDetailDTO = objdal.GetLocationContainsQuantityOfItem(ItemGUID, RoomID, CompanyId, null).OrderBy("BinNumber ASC");// "BinNumber ASC").Where(t => t.ConsignedQuantity > 0 || t.CustomerOwnedQuantity > 0);


            return (from x in objDetailDTO
                    group x by new { x.BinID, x.BinNumber, x.ItemGUID }
                        into grp
                    select new BinMasterDTO
                    {
                        ID = grp.Key.BinID ?? 0,
                        BinNumber = grp.Key.BinNumber + " (" + grp.Sum(y => y.ConsignedQuantity.GetValueOrDefault(0) + y.CustomerOwnedQuantity.GetValueOrDefault(0)) + ")",
                        //grp.Key.ItemID
                    }).ToList();
        }

        public List<BinMasterDTO> GetAllRecordsPULL(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            ItemLocationDetailsDAL objdal = new ItemLocationDetailsDAL(base.DataBaseName);
            IEnumerable<ItemLocationDetailsDTO> objDetailDTO = objdal.GetAllRecords(RoomID, CompanyId, ItemGUID, null, "BinNumber ASC").Where(t => t.ConsignedQuantity > 0 || t.CustomerOwnedQuantity > 0);


            var LocationData = (from x in objDetailDTO
                                group x by new { x.BinID, x.BinNumber, x.ItemGUID }
                                    into grp
                                select new BinMasterDTO
                                {
                                    ID = Convert.ToInt64(grp.Key.BinID),
                                    BinNumber = grp.Key.BinNumber + " (" + grp.Sum(y => y.ConsignedQuantity.GetValueOrDefault(0) + y.CustomerOwnedQuantity.GetValueOrDefault(0)) + ")",
                                    //grp.Key.ItemID
                                }).ToList();

            MaterialStagingDetailDAL objMSDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            //IEnumerable<MaterialStagingDetailDTO> objMSDTO = objMSDAL.GetCachedData(RoomID, CompanyId, false, false).Where(x => x.ItemGUID == ItemGUID && x.Quantity > 0);
            IEnumerable<MaterialStagingDetailDTO> objMSDTO = objMSDAL.GetMSDetailByRoomCompanyItemGUID(RoomID, CompanyId, Convert.ToString(ItemGUID), false, false, true);

            var MSLocationData = (from x in objMSDTO
                                  group x by new { x.StagingBinID, x.StagingBinName, x.ItemGUID }
                                      into grp
                                  select new BinMasterDTO
                                  {
                                      ID = Convert.ToInt64(grp.Key.StagingBinID),
                                      BinNumber = grp.Key.StagingBinName + "[Staging] (" + grp.Sum(y => y.Quantity) + ")", //string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity))
                                  }).ToList();

            List<BinMasterDTO> FinalLData = new List<BinMasterDTO>();
            FinalLData.AddRange(LocationData);
            FinalLData.AddRange(MSLocationData);

            return FinalLData;
        }

        public List<BinMasterDTO> GetBinsForNewPull(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            BinMasterDAL objBinDAL = null;
            IEnumerable<BinMasterDTO> lstBinList = null;
            ItemLocationQTYDAL objLocationQtyDAL = null;
            ItemLocationQTYDTO objLocatQtyDTO = null;
            List<BinMasterDTO> retunList = new List<BinMasterDTO>();
            BinMasterDTO objBinDTO = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            eTurnsRegionInfo objeTurnsRegionInfo = null;
            IEnumerable<MaterialStagingDetailDTO> lstMSDetailDTO = null;
            MaterialStagingDetailDAL objMSDAL = null;

            try
            {
                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                string qtyFormat = "N";

                if (objeTurnsRegionInfo != null && objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    qtyFormat = qtyFormat + "" + objeTurnsRegionInfo.NumberDecimalDigits;

                objBinDAL = new BinMasterDAL(base.DataBaseName);
                lstBinList = objBinDAL.GetInventoryAndStagingBinsByItem(RoomID, CompanyID, ItemGuid);

                foreach (var item in lstBinList)
                {
                    if (item.IsStagingLocation)
                    {
                        objMSDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                        //lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID).Where(x => x.Quantity > 0);
                        lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID, null, string.Empty, true);
                        if (lstMSDetailDTO != null && lstMSDetailDTO.Count() > 0 && lstMSDetailDTO.Sum(x => x.Quantity) > 0)
                        {
                            objBinDTO = new BinMasterDTO()
                            {
                                ID = item.ID,
                                BinNumber = item.BinNumber + " [Staging](" + lstMSDetailDTO.Sum(x => x.Quantity) + ")",  //string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity))
                            };
                            retunList.Add(objBinDTO);
                        }
                    }
                    else
                    {
                        objLocationQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                        objLocatQtyDTO = objLocationQtyDAL.GetRecordByBinItem(ItemGuid, item.ID, RoomID, CompanyID);
                        if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                        {
                            objBinDTO = new BinMasterDTO()
                            {
                                ID = item.ID,
                                BinNumber = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString(qtyFormat) + ")",
                            };
                            retunList.Add(objBinDTO);
                        }
                    }
                }
                return retunList;
            }
            finally
            {
                objBinDAL = null;
                lstBinList = null;
                retunList = null;
                objLocationQtyDAL = null;
                objLocatQtyDTO = null;
                objBinDTO = null;
                objeTurnsRegionInfo = null;
                objRegionSettingDAL = null;
                lstMSDetailDTO = null;
                objMSDAL = null;
            }

        }
        public List<BinMasterDTO> GetAllRecordsPULLStagin(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {

            MaterialStagingDetailDAL objMSDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            //IEnumerable<MaterialStagingDetailDTO> objMSDTO = objMSDAL.GetCachedData(RoomID, CompanyId, false, false).Where(x => x.ItemGUID == ItemGUID && x.Quantity > 0);
            IEnumerable<MaterialStagingDetailDTO> objMSDTO = objMSDAL.GetMSDetailByRoomCompanyItemGUID(RoomID, CompanyId, Convert.ToString(ItemGUID), false, false, true);

            var MSLocationData = (from x in objMSDTO
                                  group x by new { x.StagingBinID, x.StagingBinName, x.ItemGUID }
                                      into grp
                                  select new BinMasterDTO
                                  {
                                      ID = Convert.ToInt64(grp.Key.StagingBinID),
                                      BinNumber = grp.Key.StagingBinName + " [Staging] (" + grp.Sum(y => y.Quantity) + ")", //string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity))
                                      //BinNumber = "[Staging] " + grp.Key.StagingBinName + "(" + grp.Sum(y => y.Quantity) + ")",
                                  }).ToList();

            List<BinMasterDTO> FinalLData = new List<BinMasterDTO>();
            FinalLData.AddRange(MSLocationData);

            return FinalLData;
        }
        public Int64 Insert(BinMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BinMaster obj = new BinMaster();
                obj.ID = 0;
                obj.BinNumber = objDTO.BinNumber;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.IsStagingLocation = objDTO.IsStagingLocation;
                obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb.HasValue ? Convert.ToDateTime(objDTO.ReceivedOnWeb) : DateTimeUtility.DateTimeNow);
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                obj.IsEnforceDefaultPullQuantity = objDTO.IsEnforceDefaultPullQuantity;
                obj.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                obj.DefaultPullQuantity = objDTO.DefaultPullQuantity;
                obj.DefaultReorderQuantity = objDTO.DefaultReorderQuantity;

                if (objDTO.IsDefault == null || objDTO.IsDefault == false)
                    obj.IsDefault = false;
                else
                    obj.IsDefault = true;

                context.BinMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<BinMasterDTO> tempC = new List<BinMasterDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<BinMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return obj.ID;
            }
        }
        public BinMasterDTO InsertBin(BinMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BinMaster obj = new BinMaster();
                obj.ID = 0;
                obj.BinNumber = objDTO.BinNumber;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.IsStagingLocation = objDTO.IsStagingLocation;
                obj.IsStagingHeader = objDTO.IsStagingHeader;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.IsDefault = objDTO.IsDefault ?? false;
                obj.ParentBinId = objDTO.ParentBinId;

                obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb.HasValue ? Convert.ToDateTime(objDTO.ReceivedOnWeb) : DateTimeUtility.DateTimeNow);
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;


                obj.IsEnforceDefaultPullQuantity = objDTO.IsEnforceDefaultPullQuantity;
                obj.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                obj.DefaultPullQuantity = objDTO.DefaultPullQuantity;
                obj.DefaultReorderQuantity = objDTO.DefaultReorderQuantity;

                obj.BinUDF1 = objDTO.BinUDF1;
                obj.BinUDF2 = objDTO.BinUDF2;
                obj.BinUDF3 = objDTO.BinUDF3;
                obj.BinUDF4 = objDTO.BinUDF4;
                obj.BinUDF5 = objDTO.BinUDF5;

                context.BinMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                //if (objDTO.ID > 0)
                //{
                //    new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("NextBinNo", objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.BinNumber);

                //    //Get Cached-Media
                //    IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<BinMasterDTO> tempC = new List<BinMasterDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<BinMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return objDTO;
            }
        }

        public BinMasterDTO InsertBinFromPullHistoryImport(BinMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BinMaster obj = new BinMaster();
                obj.ID = 0;
                obj.BinNumber = objDTO.BinNumber;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.IsStagingLocation = objDTO.IsStagingLocation;
                obj.IsStagingHeader = objDTO.IsStagingHeader;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.IsDefault = objDTO.IsDefault ?? false;
                obj.ParentBinId = objDTO.ParentBinId;

                obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb.HasValue ? Convert.ToDateTime(objDTO.ReceivedOnWeb) : DateTimeUtility.DateTimeNow);
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.IsEnforceDefaultPullQuantity = objDTO.IsEnforceDefaultPullQuantity;
                obj.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                obj.DefaultPullQuantity = objDTO.DefaultPullQuantity;
                obj.DefaultReorderQuantity = objDTO.DefaultReorderQuantity;
                context.BinMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                return objDTO;
            }
        }
        public bool Edit(BinMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTime.UtcNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BinMaster obj = new BinMaster();
                obj = context.BinMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                if (obj != null)
                {
                    obj.ID = objDTO.ID;
                    obj.BinNumber = objDTO.BinNumber;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.IsStagingLocation = objDTO.IsStagingLocation;
                    obj.LastUpdated = DateTimeUtility.DateTimeNow;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.ItemGUID = objDTO.ItemGUID;
                    obj.MinimumQuantity = objDTO.MinimumQuantity;
                    obj.MaximumQuantity = objDTO.MaximumQuantity;
                    obj.CriticalQuantity = objDTO.CriticalQuantity;
                    obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;

                    if (objDTO.IsOnlyFromItemUI) //Only Updated When Bin Updated From UI
                    {
                        obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                        if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                            obj.EditedFrom = objDTO.EditedFrom;
                        else
                            obj.EditedFrom = "Web";
                    }

                    obj.IsDefault = Convert.ToBoolean(objDTO.IsDefault);

                    obj.IsEnforceDefaultPullQuantity = objDTO.IsEnforceDefaultPullQuantity;
                    obj.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                    obj.DefaultPullQuantity = objDTO.DefaultPullQuantity;
                    obj.DefaultReorderQuantity = objDTO.DefaultReorderQuantity;

                    obj.BinUDF1 = objDTO.BinUDF1;
                    obj.BinUDF2 = objDTO.BinUDF2;
                    obj.BinUDF3 = objDTO.BinUDF3;
                    obj.BinUDF4 = objDTO.BinUDF4;
                    obj.BinUDF5 = objDTO.BinUDF5;

                    context.SaveChanges();
                }
                IQueryable<BinMaster> lstBins = context.BinMasters.Where(t => t.ParentBinId == obj.ID);
                if (lstBins.Any())
                {
                    foreach (var item in lstBins)
                    {
                        item.BinNumber = objDTO.BinNumber;
                    }
                    context.SaveChanges();
                }
                //Get Cached-Media
                //IEnumerable<BinMasterDTO> ObjCache = CacheHelper<IEnumerable<BinMasterDTO>>.GetCacheItem("Cached_BinMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<BinMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<BinMasterDTO> tempC = new List<BinMasterDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<BinMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<BinMasterDTO>>.AppendToCacheItem("Cached_BinMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //}
                return true;
            }
        }
        public string UpdateData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE BinMaster SET " + columnName + " = '" + value + "', EditedFrom='Web-BinMaster.UpdateData', LastUpdated =getutcdate() WHERE ID=" + id;
                context.Database.ExecuteSqlCommand(strQuery);
            }
            return value;
        }
        public IEnumerable<BinMasterDTO> GetItemLocationQty(Int64 RoomID, Int64 CompanyID, Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@ItemGuid", ItemGUID) };

                return (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetItemLocationQtyByRoomIDItemGUID]  @RoomID,@CompanyID,@ItemGuid", params1)
                        select new BinMasterDTO
                        {
                            ID = u.ID,
                            BinNumber = u.BinNumber,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            ItemGUID = u.ItemGUID,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            GUID = u.GUID,
                            SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
                            IsStagingLocation = u.IsStagingLocation,
                            IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                            IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                            DefaultPullQuantity = u.DefaultPullQuantity,
                            DefaultReorderQuantity = u.DefaultReorderQuantity
                        }).AsParallel().ToList();

            }

        }
        public IEnumerable<BinMasterDTO> GetAllRecordsItemWise(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, bool? IsStagingLocation, bool? IsBinNumberNull, string BinNameStartWith)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@ItemGuid", ItemGUID),
                                                   new SqlParameter("@IsStagingLocation", IsStagingLocation?? (object)DBNull.Value),
                                                   new SqlParameter("@IsBinNumberNotNull", IsBinNumberNull?? (object)DBNull.Value),
                                                   new SqlParameter("@BinNameStartWith", BinNameStartWith) };

                return (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterByRoomIDItemGUID] @RoomID,@CompanyID,@ItemGuid,@IsStagingLocation,@IsBinNumberNotNull,@BinNameStartWith", params1)
                        select new BinMasterDTO
                        {
                            ID = u.ID,
                            BinNumber = u.BinNumber,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            ItemGUID = u.ItemGUID,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            GUID = u.GUID,
                            SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
                            IsStagingLocation = u.IsStagingLocation,
                            IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                            IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                            DefaultPullQuantity = u.DefaultPullQuantity,
                            DefaultReorderQuantity = u.DefaultReorderQuantity
                        }).AsParallel().ToList();

            }
            //return GetItemLocationQty(RoomID, CompanyId, ItemGUID);
        }
        public IEnumerable<BinMasterDTO> GetAllRecordsItemBinWise(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, Int64 Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@ItemGuid", ItemGUID),
                                                   new SqlParameter("@ID", Id),};

                return (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetItemLocationQtyByRoomBinIDItemGUID] @RoomID,@CompanyID,@ItemGuid,@ID", params1)
                        select new BinMasterDTO
                        {
                            ID = u.ID,
                            BinNumber = u.BinNumber,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            ItemGUID = u.ItemGUID,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            GUID = u.GUID,
                            SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
                            IsStagingLocation = u.IsStagingLocation,
                            IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                            IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                            DefaultPullQuantity = u.DefaultPullQuantity,
                            DefaultReorderQuantity = u.DefaultReorderQuantity
                        }).AsParallel().ToList();

            }
            //return GetItemLocationQty(RoomID, CompanyId, ItemGUID).Where(t => t.ItemGUID == ItemGUID && t.ID == Id);
        }
        private List<BinMasterDTO> GetAllStagingBinsForRequisitionByMSGUId(Guid MSGuid, Guid ItemGuid, long CompanyId, long RoomId)
        {
            List<BinMasterDTO> obj = new List<BinMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<long> binFromRequisitionDetails = (from ms in context.MaterialStagings
                                                        join req in context.RequisitionMasters on ms.GUID equals req.MaterialStagingGUID
                                                        join reqd in context.RequisitionDetails on req.GUID equals reqd.RequisitionGUID
                                                        join ci in context.BinMasters on reqd.BinID equals ci.ID
                                                        join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                                        from ci_cc in ci_cc_join.DefaultIfEmpty()
                                                        join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                                        from ci_cu in ci_cu_join.DefaultIfEmpty()
                                                        where ms.GUID == MSGuid && req.MaterialStagingGUID == MSGuid && reqd.ItemGUID == ItemGuid && ci.ItemGUID == ItemGuid && ms.Room == RoomId && ci.IsStagingLocation == true && ms.CompanyID == CompanyId && ms.IsDeleted == false && ms.IsArchived == false && ci.IsDeleted == false && ci.IsArchived == false
                                                        select ci.ID
                                      ).ToList();

                List<long> binFromMaterialStagingDetails = (from ms in context.MaterialStagings
                                                            join msd in context.MaterialStagingDetails on ms.GUID equals msd.MaterialStagingGUID
                                                            join ci in context.BinMasters on msd.StagingBinID equals ci.ID
                                                            join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                                            from ci_cc in ci_cc_join.DefaultIfEmpty()
                                                            join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                                            from ci_cu in ci_cu_join.DefaultIfEmpty()
                                                            where ms.GUID == MSGuid && msd.MaterialStagingGUID == MSGuid && ci.ItemGUID == ItemGuid && ms.Room == RoomId && ci.IsStagingLocation == true && ms.CompanyID == CompanyId && ms.IsDeleted == false && ms.IsArchived == false && ci.IsDeleted == false && ci.IsArchived == false
                                                            //&& ci.ParentBinId == null
                                                            select ci.ID
                                      ).ToList();


                obj = (from ci in context.BinMasters.Where(t => (binFromRequisitionDetails.Concat(binFromMaterialStagingDetails).Contains(t.ID)))
                           //&& ci.ParentBinId == null
                       select new BinMasterDTO
                       {
                           BinNumber = ((ci.BinNumber ?? string.Empty).Equals("[|EmptyStagingBin|]") ? string.Empty : ci.BinNumber),
                           GUID = ci.GUID,
                           ID = ci.ID,
                           IsStagingLocation = ci.IsStagingLocation,
                           ItemGUID = ci.ItemGUID,
                           MaterialStagingGUID = ci.MaterialStagingGUID
                       }).AsExpandable().ToList();
            }
            return obj;
        }
        public List<BinMasterDTO> GetPagedBinRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            TotalCount = 0;
            BinMasterDTO objBinMaster = new BinMasterDTO();
            DataSet dsBins = new DataSet();
            //string Connectionstring = ConfigurationManager.ConnectionStrings["eTurnsConnection"].ConnectionString;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstBins;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            string BinCreaters = null;
            string BinUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;


            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedBins", StartRowIndex, MaxRows, SearchTerm, sortColumnName, BinCreaters, BinUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //&& ((Fields[1].Split('@')[24] == "") || (Fields[1].Split('@')[24].Split(',').ToList().Contains(t.ReplenishType)))
                //    && ((Fields[1].Split('@')[23] == "") || (Fields[1].Split('@')[23].Split(',').ToList().Contains(t.SupplierId.ToString())))
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
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
                    BinCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    BinUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //  CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    // UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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

                dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedBins", StartRowIndex, MaxRows, SearchTerm, sortColumnName, BinCreaters, BinUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else
            {
                dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedBins", StartRowIndex, MaxRows, SearchTerm, sortColumnName, BinCreaters, BinUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            if (dsBins != null && dsBins.Tables.Count > 0)
            {
                DataTable dtBins = dsBins.Tables[0];
                if (dtBins.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtBins.Rows[0]["TotalRecords"]);
                    long templong = 0;
                    //Guid? tempguid = null;
                    //bool tempbool = false;
                    //double tempdouble = 0;
                    foreach (DataRow dr in dtBins.Rows)
                    {
                        objBinMaster = new BinMasterDTO();
                        if (dtBins.Columns.Contains("BinNumber"))
                        {
                            objBinMaster.BinNumber = Convert.ToString(dr["BinNumber"]);
                        }
                        if (dtBins.Columns.Contains("CompanyID"))
                        {
                            long.TryParse(Convert.ToString(dr["CompanyID"]), out templong);
                            objBinMaster.CompanyID = templong;
                        }
                        if (dtBins.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objBinMaster.ID = templong;
                        }
                        if (dtBins.Columns.Contains("UDF1"))
                        {
                            objBinMaster.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtBins.Columns.Contains("UDF2"))
                        {
                            objBinMaster.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtBins.Columns.Contains("UDF3"))
                        {
                            objBinMaster.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtBins.Columns.Contains("UDF4"))
                        {
                            objBinMaster.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtBins.Columns.Contains("UDF5"))
                        {
                            objBinMaster.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtBins.Columns.Contains("CreatedByName"))
                        {
                            objBinMaster.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                        }
                        if (dtBins.Columns.Contains("UpdatedByName"))
                        {
                            objBinMaster.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }
                        if (dtBins.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objBinMaster.CreatedBy = templong;
                        }
                        if (dtBins.Columns.Contains("LastUpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            objBinMaster.LastUpdatedBy = templong;
                        }
                        if (dtBins.Columns.Contains("Created"))
                        {
                            objBinMaster.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtBins.Columns.Contains("LastUpdated"))
                        {
                            objBinMaster.LastUpdated = Convert.ToDateTime(dr["LastUpdated"]);
                        }
                        if (dtBins.Columns.Contains("AddedFrom"))
                        {
                            objBinMaster.AddedFrom = Convert.ToString(dr["AddedFrom"]);
                        }
                        if (dtBins.Columns.Contains("EditedFrom"))
                        {
                            objBinMaster.EditedFrom = Convert.ToString(dr["EditedFrom"]);
                        }
                        if (dtBins.Columns.Contains("ReceivedOnWeb"))
                        {
                            objBinMaster.ReceivedOnWeb = Convert.ToDateTime(dr["ReceivedOnWeb"]);
                        }
                        if (dtBins.Columns.Contains("ReceivedOn"))
                        {
                            objBinMaster.ReceivedOn = Convert.ToDateTime(dr["ReceivedOn"]);
                        }
                        lstBins.Add(objBinMaster);
                    }
                }
            }
            return lstBins;
        }
        public List<BinMasterDTO> GetAllBins(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string BinNumber, bool? Staginglocation)
        {
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                List<SqlParameter> lstPara = new List<SqlParameter>() {
                new SqlParameter("@RoomID",RoomID),
                new SqlParameter("@CompanyId",CompanyId),
                new SqlParameter("@IsStagingLocation",Staginglocation.ToDBNull()),
                new SqlParameter("@BinNumber",BinNumber.ToDBNull()),
                new SqlParameter("@IsArchived",IsArchived),
                new SqlParameter("@IsDeleted",IsDeleted)
                };

                lstBins = (from ci in context.Database.SqlQuery<BinMasterDTO>("EXEC uspGetAllBins @RoomID,@CompanyId,@IsStagingLocation,@BinNumber,@IsArchived,@IsDeleted"
                    , lstPara.ToArray())
                           select new BinMasterDTO
                           {
                               BinNumber = ci.BinNumber,
                               CompanyID = ci.CompanyID,
                               Created = ci.Created,
                               CreatedBy = ci.CreatedBy,
                               CriticalQuantity = ci.CriticalQuantity,
                               GUID = ci.GUID,
                               ID = ci.ID,
                               IsArchived = ci.IsArchived,
                               IsDefault = ci.IsDefault,
                               IsDeleted = ci.IsDeleted,
                               IsStagingHeader = ci.IsStagingHeader,
                               IsStagingLocation = ci.IsStagingLocation,
                               ItemGUID = ci.ItemGUID,
                               LastUpdated = ci.LastUpdated,
                               LastUpdatedBy = ci.LastUpdatedBy,
                               MaterialStagingGUID = ci.MaterialStagingGUID,
                               MaximumQuantity = ci.MaximumQuantity,
                               MinimumQuantity = ci.MinimumQuantity,
                               ParentBinId = ci.ParentBinId,
                               Room = ci.Room,
                               SuggestedOrderQuantity = ci.SuggestedOrderQuantity,
                               UDF1 = ci.UDF1,
                               UDF10 = ci.UDF10,
                               UDF2 = ci.UDF2,
                               UDF3 = ci.UDF3,
                               UDF4 = ci.UDF4,
                               UDF5 = ci.UDF5,
                               CreatedByName = ci.CreatedByName,
                               UpdatedByName = ci.UpdatedByName,
                               AddedFrom = ci.AddedFrom,
                               EditedFrom = ci.EditedFrom,
                               ReceivedOn = (ci.ReceivedOn == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOn),
                               ReceivedOnWeb = (ci.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOnWeb),
                               IsEnforceDefaultPullQuantity = ci.IsEnforceDefaultPullQuantity,
                               IsEnforceDefaultReorderQuantity = ci.IsEnforceDefaultReorderQuantity,
                               DefaultPullQuantity = ci.DefaultPullQuantity,
                               DefaultReorderQuantity = ci.DefaultReorderQuantity
                           }).ToList();


                //Expression<Func<BinMasterDTO, bool>> Filterexp = q => true;
                //if (Staginglocation.HasValue)
                //{
                //    Filterexp = Filterexp.And(t => t.IsStagingLocation == Staginglocation);
                //}


                //if (!string.IsNullOrWhiteSpace(BinNumber))
                //{
                //    Filterexp = Filterexp.And(t => (t.BinNumber ?? string.Empty).ToLower().Contains(BinNumber));
                //}
                //if (IsArchived && IsDeleted)
                //{
                //    Filterexp = Filterexp.And(t => (t.IsArchived ?? false) == true && t.IsDeleted == true);
                //}
                //else if (!IsArchived && !IsDeleted)
                //{
                //    Filterexp = Filterexp.And(t => (t.IsArchived ?? false) == false && t.IsDeleted == false);
                //}
                //else if (IsDeleted)
                //{
                //    Filterexp = Filterexp.And(t => t.IsDeleted == true);
                //}
                //else if (IsArchived)
                //{
                //    Filterexp = Filterexp.And(t => (t.IsArchived ?? false) == true);
                //}
                //lstBins = (from ci in context.BinMasters
                //           join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                //           from ci_cc in ci_cc_join.DefaultIfEmpty()
                //           join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                //           from ci_cu in ci_cu_join.DefaultIfEmpty()
                //           where ci.Room == RoomID && ci.CompanyID == CompanyId && ci.IsDeleted == false && ci.ParentBinId == null
                //           select new BinMasterDTO
                //           {
                //               BinNumber = ci.BinNumber,
                //               CompanyID = ci.CompanyID,
                //               Created = ci.Created,
                //               CreatedBy = ci.CreatedBy,
                //               CriticalQuantity = ci.CriticalQuantity,
                //               GUID = ci.GUID,
                //               ID = ci.ID,
                //               IsArchived = ci.IsArchived,
                //               IsDefault = ci.IsDefault,
                //               IsDeleted = ci.IsDeleted,
                //               IsStagingHeader = ci.IsStagingHeader,
                //               IsStagingLocation = ci.IsStagingLocation,
                //               ItemGUID = ci.ItemGUID,
                //               LastUpdated = ci.LastUpdated,
                //               LastUpdatedBy = ci.LastUpdatedBy,
                //               MaterialStagingGUID = ci.MaterialStagingGUID,
                //               MaximumQuantity = ci.MaximumQuantity,
                //               MinimumQuantity = ci.MinimumQuantity,
                //               ParentBinId = ci.ParentBinId,
                //               Room = ci.Room,
                //               SuggestedOrderQuantity = ci.SuggestedOrderQuantity,
                //               UDF1 = ci.UDF1,
                //               UDF10 = ci.UDF10,
                //               UDF2 = ci.UDF2,
                //               UDF3 = ci.UDF3,
                //               UDF4 = ci.UDF4,
                //               UDF5 = ci.UDF5,
                //               CreatedByName = ci_cc.UserName,
                //               UpdatedByName = ci_cu.UserName,
                //               AddedFrom = (ci.AddedFrom == null ? "Web" : ci.AddedFrom),
                //               EditedFrom = (ci.EditedFrom == null ? "Web" : ci.EditedFrom),
                //               ReceivedOn = (ci.ReceivedOn == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOn),
                //               ReceivedOnWeb = (ci.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOnWeb),
                //               IsEnforceDefaultPullQuantity = ci.IsEnforceDefaultPullQuantity,
                //               IsEnforceDefaultReorderQuantity = ci.IsEnforceDefaultReorderQuantity,
                //               DefaultPullQuantity = ci.DefaultPullQuantity,
                //               DefaultReorderQuantity = ci.DefaultReorderQuantity
                //           }).AsExpandable().Where(Filterexp).ToList();
            }
            return lstBins;
        }

        public List<BinMasterDTO> GetAllBins(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string BinNumber, bool? Staginglocation, string ItemGUID, bool? IsBinNumberNull, bool? EmptyStagingBin)
        {
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> lstPara = new List<SqlParameter>() {
                                                                new SqlParameter("@RoomID",RoomID),
                                                                new SqlParameter("@CompanyId",CompanyId),
                                                                new SqlParameter("@IsStagingLocation",Staginglocation.ToDBNull()),
                                                                new SqlParameter("@BinNumber",BinNumber.ToDBNull()),
                                                                new SqlParameter("@IsArchived",IsArchived),
                                                                new SqlParameter("@IsDeleted",IsDeleted),
                                                                new SqlParameter("@ItemGUID", ItemGUID),
                                                                new SqlParameter("@IsBinNumberNull", IsBinNumberNull ?? (object)DBNull.Value),
                                                                new SqlParameter("@EmptyStagingBin", EmptyStagingBin ?? (object)DBNull.Value)  };

                lstBins = (from ci in context.Database.SqlQuery<BinMasterDTO>("EXEC [uspGetAllBinsByBinItemGUID] @RoomID,@CompanyId,@IsStagingLocation,@BinNumber,@IsArchived,@IsDeleted,@ItemGUID,@IsBinNumberNull,@EmptyStagingBin", lstPara.ToArray())
                           select new BinMasterDTO
                           {
                               BinNumber = ci.BinNumber,
                               CompanyID = ci.CompanyID,
                               Created = ci.Created,
                               CreatedBy = ci.CreatedBy,
                               CriticalQuantity = ci.CriticalQuantity,
                               GUID = ci.GUID,
                               ID = ci.ID,
                               IsArchived = ci.IsArchived,
                               IsDefault = ci.IsDefault,
                               IsDeleted = ci.IsDeleted,
                               IsStagingHeader = ci.IsStagingHeader,
                               IsStagingLocation = ci.IsStagingLocation,
                               ItemGUID = ci.ItemGUID,
                               LastUpdated = ci.LastUpdated,
                               LastUpdatedBy = ci.LastUpdatedBy,
                               MaterialStagingGUID = ci.MaterialStagingGUID,
                               MaximumQuantity = ci.MaximumQuantity,
                               MinimumQuantity = ci.MinimumQuantity,
                               ParentBinId = ci.ParentBinId,
                               Room = ci.Room,
                               SuggestedOrderQuantity = ci.SuggestedOrderQuantity,
                               UDF1 = ci.UDF1,
                               UDF10 = ci.UDF10,
                               UDF2 = ci.UDF2,
                               UDF3 = ci.UDF3,
                               UDF4 = ci.UDF4,
                               UDF5 = ci.UDF5,
                               CreatedByName = ci.CreatedByName,
                               UpdatedByName = ci.UpdatedByName,
                               AddedFrom = ci.AddedFrom,
                               EditedFrom = ci.EditedFrom,
                               ReceivedOn = (ci.ReceivedOn == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOn),
                               ReceivedOnWeb = (ci.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOnWeb),
                               IsEnforceDefaultPullQuantity = ci.IsEnforceDefaultPullQuantity,
                               IsEnforceDefaultReorderQuantity = ci.IsEnforceDefaultReorderQuantity,
                               DefaultPullQuantity = ci.DefaultPullQuantity,
                               DefaultReorderQuantity = ci.DefaultReorderQuantity
                           }).ToList();
            }
            return lstBins;
        }



        public List<CountLineItemDetailDTOForAutoComplete> GetItemLocationDetailForAutoComplete(Guid ItemGuid, string ItemSourceBin, Int64 CompanyId, Int64 RoomID)
        {
            List<CountLineItemDetailDTOForAutoComplete> lstDTOForAutoComplete = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // lstDTOForAutoComplete = (from I in context.csp_GetItemLocationDetailForAutoComplete(ItemGuid, ItemSourceBin, CompanyId, RoomID)
                //                         select new CountLineItemDetailDTOForAutoComplete()
                //                         {
                //                             LotSerialNumber = I.LotSerialNumber,
                //                             LotSerialNumberWithoutDate = I.LotSerialNumberWithoutDate,
                //                             ExpirationDate = I.ExpirationDate,
                //                             AvailableQuantity = I.AvailableQuantity
                //                         }).ToList();
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@ItemSourceBin", ItemSourceBin), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomID", RoomID) };
                lstDTOForAutoComplete = (from u in context.Database.SqlQuery<CountLineItemDetailDTOForAutoComplete>("exec [csp_GetItemLocationDetailForAutoComplete] @ItemGuid,@ItemSourceBin,@CompanyId,@RoomID", params1) select u).ToList();


                if (lstDTOForAutoComplete == null)
                    lstDTOForAutoComplete = new List<CountLineItemDetailDTOForAutoComplete>();

            }

            return lstDTOForAutoComplete;
        }
        public List<CountLineItemDetailDTO> GetAllItemLocationsByItemId(Guid ItemGuid, Int64 CompanyId, Int64 RoomID, bool IsStage = false)
        {
            List<CountLineItemDetailDTO> lstDTOForAutoComplete = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGuid), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomId", RoomID), new SqlParameter("@IsStage", IsStage) };
                lstDTOForAutoComplete = (from u in context.Database.SqlQuery<CountLineItemDetailDTO>(@"EXEC csp_GetAllItemLocationsByItemId @ItemGUID,@CompanyId,@RoomId,@IsStage", params1)
                                         group u by new { u.BinID, u.BinNumber, u.LotSerialNumber } into grp
                                         select new CountLineItemDetailDTO
                                         {
                                             ID = 0,
                                             LotSerialNumber = grp.Key.LotSerialNumber,
                                             ExpirationDate = grp.Max(u => u.ExpirationDate),
                                             ConsignedQuantity = grp.Sum(x => (x.ConsignedQuantity < 0 ? 0 : x.ConsignedQuantity)),
                                             CustomerOwnedQuantity = grp.Sum(x => (x.CustomerOwnedQuantity < 0 ? 0 : x.CustomerOwnedQuantity)),
                                             BinNumber = grp.Key.BinNumber,
                                             BinID = grp.Key.BinID
                                         }).ToList();

                //lstDTOForAutoComplete = (from I in context.ItemLocationDetails
                //                         join B in context.BinMasters on I.BinID equals B.ID
                //                         where I.ItemGUID == ItemGuid
                //                               && I.CompanyID == CompanyId
                //                               && I.Room == RoomID
                //                               && (I.IsDeleted == null || I.IsDeleted == false)
                //                               && (I.IsArchived == null || I.IsArchived == false)
                //                               && (B.IsDeleted == null || B.IsDeleted == false)
                //                               && (B.IsArchived == null || B.IsArchived == false)
                //                         select new CountLineItemDetailDTO()
                //                         {
                //                             ID = 0,
                //                             LotSerialNumber = (I.LotNumber != null && I.LotNumber.Trim() != "" ? I.LotNumber : I.SerialNumber),
                //                             ExpirationDate = I.ExpirationDate,
                //                             ConsignedQuantity = I.ConsignedQuantity,
                //                             CustomerOwnedQuantity = I.CustomerOwnedQuantity,
                //                             BinNumber = B.BinNumber,
                //                             BinID = B.ID
                //                         }).ToList();

                if (lstDTOForAutoComplete == null)
                    lstDTOForAutoComplete = new List<CountLineItemDetailDTO>();

            }

            return lstDTOForAutoComplete;
        }

        public List<CountLineItemDetailDTO> GetAllDateCodeItemLocationsByItemId(Guid ItemGuid, Int64 CompanyId, Int64 RoomID, bool IsStage = false)
        {
            List<CountLineItemDetailDTO> lstDTOForAutoComplete = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGuid), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomId", RoomID), new SqlParameter("@IsStage", IsStage) };
                lstDTOForAutoComplete = (from u in context.Database.SqlQuery<CountLineItemDetailDTO>(@"EXEC csp_GetAllItemLocationsByItemId @ItemGUID,@CompanyId,@RoomId,@IsStage", params1)
                                         group u by new { u.BinID, u.BinNumber, u.ExpirationDate } into grp
                                         select new CountLineItemDetailDTO
                                         {
                                             ID = 0,
                                             LotSerialNumber = "",
                                             ExpirationDate = grp.Max(u => u.ExpirationDate),
                                             ConsignedQuantity = grp.Sum(x => (x.ConsignedQuantity < 0 ? 0 : x.ConsignedQuantity)),
                                             CustomerOwnedQuantity = grp.Sum(x => (x.CustomerOwnedQuantity < 0 ? 0 : x.CustomerOwnedQuantity)),
                                             BinNumber = grp.Key.BinNumber,
                                             BinID = grp.Key.BinID
                                         }).ToList();

                if (lstDTOForAutoComplete == null)
                    lstDTOForAutoComplete = new List<CountLineItemDetailDTO>();

            }

            return lstDTOForAutoComplete;
        }
        public BinMasterDTO GetBinByName(string BinName, Int64 RoomID, Int64 CompanyId, bool StagingLocation)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinName", BinName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@StagingLocation", StagingLocation) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinByBinNumber] @BinName,@RoomID,@CompanyID,@StagingLocation", params1).FirstOrDefault();
            }
        }

        public BinMasterDTO GetBinByBinNumberPlain(string BinName, Int64 RoomID, Int64 CompanyId, bool StagingLocation)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinName", BinName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@StagingLocation", StagingLocation) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinByBinNumberPlain] @BinName,@RoomID,@CompanyID,@StagingLocation", params1).FirstOrDefault();
            }
        }

        public List<BinMasterDTO> GetListBinByName(string BinName, Int64 RoomID, Int64 CompanyId, bool StagingLocation)
        {
            List<BinMasterDTO> objBinMasterList = new List<BinMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objBinMasterList = (from ci in context.BinMasters
                                    join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                    from ci_cc in ci_cc_join.DefaultIfEmpty()
                                    join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                    from ci_cu in ci_cu_join.DefaultIfEmpty()
                                    where ci.BinNumber == BinName && ci.Room == RoomID && ci.CompanyID == CompanyId && ci.ParentBinId == null && ci.IsStagingLocation == StagingLocation
                                    select new BinMasterDTO
                                    {
                                        BinNumber = ci.BinNumber,
                                        CompanyID = ci.CompanyID,
                                        Created = ci.Created,
                                        CreatedBy = ci.CreatedBy,
                                        CriticalQuantity = ci.CriticalQuantity,
                                        GUID = ci.GUID,
                                        ID = ci.ID,
                                        IsArchived = ci.IsArchived,
                                        IsDefault = ci.IsDefault,
                                        IsDeleted = ci.IsDeleted,
                                        IsStagingHeader = ci.IsStagingHeader,
                                        IsStagingLocation = ci.IsStagingLocation,
                                        ItemGUID = ci.ItemGUID,
                                        LastUpdated = ci.LastUpdated,
                                        LastUpdatedBy = ci.LastUpdatedBy,
                                        MaterialStagingGUID = ci.MaterialStagingGUID,
                                        MaximumQuantity = ci.MaximumQuantity,
                                        MinimumQuantity = ci.MinimumQuantity,
                                        ParentBinId = ci.ParentBinId,
                                        Room = ci.Room,
                                        SuggestedOrderQuantity = ci.SuggestedOrderQuantity,
                                        UDF1 = ci.UDF1,
                                        UDF10 = ci.UDF10,
                                        UDF2 = ci.UDF2,
                                        UDF3 = ci.UDF3,
                                        UDF4 = ci.UDF4,
                                        UDF5 = ci.UDF5,
                                        CreatedByName = ci_cc.UserName,
                                        UpdatedByName = ci_cu.UserName,
                                        AddedFrom = (ci.AddedFrom == null ? "Web" : ci.AddedFrom),
                                        EditedFrom = (ci.EditedFrom == null ? "Web" : ci.EditedFrom),
                                        ReceivedOn = (ci.ReceivedOn == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOn),
                                        ReceivedOnWeb = (ci.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOnWeb),
                                        IsEnforceDefaultPullQuantity = ci.IsEnforceDefaultPullQuantity,
                                        IsEnforceDefaultReorderQuantity = ci.IsEnforceDefaultReorderQuantity,
                                        DefaultPullQuantity = ci.DefaultPullQuantity,
                                        DefaultReorderQuantity = ci.DefaultReorderQuantity
                                    }).ToList();
            }
            return objBinMasterList;
        }
        public BinMasterDTO GetItemBinByParentBin(Guid ItemGuid, long ParentBinID, Int64 RoomID, Int64 CompanyId, bool StagingLocation)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@ParentBinID", ParentBinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@StagingLocation", StagingLocation) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetItemBinByParentBin] @ItemGuid,@ParentBinID,@RoomID,@CompanyID,@StagingLocation", params1).FirstOrDefault();
            }           
        }
        
        public BinMasterDTO GetItemBinByParentBinPlain(Guid ItemGuid, long ParentBinID, Int64 RoomID, Int64 CompanyId, bool StagingLocation)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@ParentBinID", ParentBinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@StagingLocation", StagingLocation) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetItemBinByParentBinPlain] @ItemGuid,@ParentBinID,@RoomID,@CompanyID,@StagingLocation", params1).FirstOrDefault();
            }           
        }
        public BinMasterDTO GetInventoryLocation(long ParentID, Guid ItemGUID, Int64 RoomID, Int64 CompanyId)
        {
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objBinMasterDTO = (from ci in context.BinMasters
                                   join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                                   from ci_cc in ci_cc_join.DefaultIfEmpty()
                                   join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                                   from ci_cu in ci_cu_join.DefaultIfEmpty()
                                   where ci.ParentBinId == ParentID && ci.ItemGUID == ItemGUID && ci.Room == RoomID && ci.CompanyID == CompanyId && ci.IsDeleted == false
                                   select new BinMasterDTO
                                   {
                                       BinNumber = ci.BinNumber,
                                       CompanyID = ci.CompanyID,
                                       Created = ci.Created,
                                       CreatedBy = ci.CreatedBy,
                                       CriticalQuantity = ci.CriticalQuantity,
                                       GUID = ci.GUID,
                                       ID = ci.ID,
                                       IsArchived = ci.IsArchived,
                                       IsDefault = ci.IsDefault,
                                       IsDeleted = ci.IsDeleted,
                                       IsStagingHeader = ci.IsStagingHeader,
                                       IsStagingLocation = ci.IsStagingLocation,
                                       ItemGUID = ci.ItemGUID,
                                       LastUpdated = ci.LastUpdated,
                                       LastUpdatedBy = ci.LastUpdatedBy,
                                       MaterialStagingGUID = ci.MaterialStagingGUID,
                                       MaximumQuantity = ci.MaximumQuantity,
                                       MinimumQuantity = ci.MinimumQuantity,
                                       ParentBinId = ci.ParentBinId,
                                       Room = ci.Room,
                                       SuggestedOrderQuantity = ci.SuggestedOrderQuantity,
                                       UDF1 = ci.UDF1,
                                       UDF10 = ci.UDF10,
                                       UDF2 = ci.UDF2,
                                       UDF3 = ci.UDF3,
                                       UDF4 = ci.UDF4,
                                       UDF5 = ci.UDF5,
                                       CreatedByName = ci_cc.UserName,
                                       UpdatedByName = ci_cu.UserName,
                                       AddedFrom = (ci.AddedFrom == null ? "Web" : ci.AddedFrom),
                                       EditedFrom = (ci.EditedFrom == null ? "Web" : ci.EditedFrom),
                                       ReceivedOn = (ci.ReceivedOn == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOn),
                                       ReceivedOnWeb = (ci.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOnWeb),
                                       IsEnforceDefaultPullQuantity = ci.IsEnforceDefaultPullQuantity,
                                       IsEnforceDefaultReorderQuantity = ci.IsEnforceDefaultReorderQuantity,
                                       DefaultPullQuantity = ci.DefaultPullQuantity,
                                       DefaultReorderQuantity = ci.DefaultReorderQuantity
                                   }).FirstOrDefault();
            }
            return objBinMasterDTO;
        }

        public List<BinMasterDTO> AssignUpdateItemLocations(List<BinMasterDTO> lstLocations, Guid ItemGuid, long RoomId, long CompanyID, long UserID, bool StagingLocation, out double? UpdatedOnHandQuantity, out List<long> insertedBinIds, long SessionUserId)
        {
            List<BinMasterDTO> lstReturn = new List<BinMasterDTO>();
            UpdatedOnHandQuantity = null;
            insertedBinIds = new List<long>();

            if (lstLocations != null && lstLocations.Count > 0)
            {
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGuid);
                    if (objItem != null)
                    {
                        foreach (var item in lstLocations)
                        {
                            BinMasterDTO objBinMasterDTOToInsert = objBinMasterDAL.GetBinByName(item.BinNumber.Trim(), RoomId, CompanyID, StagingLocation);
                            if (objBinMasterDTOToInsert == null)
                            {
                                objBinMasterDTOToInsert = new BinMasterDTO();
                                objBinMasterDTOToInsert.BinNumber = item.BinNumber.Trim();
                                objBinMasterDTOToInsert.ParentBinId = null;
                                objBinMasterDTOToInsert.CreatedBy = UserID;
                                objBinMasterDTOToInsert.LastUpdatedBy = UserID;
                                objBinMasterDTOToInsert.Created = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.LastUpdated = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.Room = RoomId;
                                objBinMasterDTOToInsert.CompanyID = CompanyID;
                                objBinMasterDTOToInsert.AddedFrom = "Web";
                                objBinMasterDTOToInsert.EditedFrom = "Web";
                                objBinMasterDTOToInsert.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.IsOnlyFromItemUI = true;

                                objBinMasterDTOToInsert.IsEnforceDefaultPullQuantity = item.IsEnforceDefaultPullQuantity;
                                objBinMasterDTOToInsert.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
                                objBinMasterDTOToInsert.DefaultPullQuantity = item.DefaultPullQuantity;
                                objBinMasterDTOToInsert.DefaultReorderQuantity = item.DefaultReorderQuantity;
                                objBinMasterDTOToInsert.BinUDF1 = item.BinUDF1;
                                objBinMasterDTOToInsert.BinUDF2 = item.BinUDF2;
                                objBinMasterDTOToInsert.BinUDF3 = item.BinUDF3;
                                objBinMasterDTOToInsert.BinUDF4 = item.BinUDF4;
                                objBinMasterDTOToInsert.BinUDF5 = item.BinUDF5;

                                objBinMasterDTOToInsert = objBinMasterDAL.InsertBin(objBinMasterDTOToInsert);
                            }
                            BinMasterDTO objInventoryLocation = GetInventoryLocation(objBinMasterDTOToInsert.ID, ItemGuid, RoomId, CompanyID);
                            if (objInventoryLocation == null)
                            {
                                objInventoryLocation = new BinMasterDTO();
                                objInventoryLocation.BinNumber = objBinMasterDTOToInsert.BinNumber;
                                objInventoryLocation.ParentBinId = objBinMasterDTOToInsert.ID;
                                objInventoryLocation.CreatedBy = UserID;
                                objInventoryLocation.LastUpdatedBy = UserID;
                                objInventoryLocation.Created = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.MinimumQuantity = item.MinimumQuantity;
                                objInventoryLocation.MaximumQuantity = item.MaximumQuantity;
                                objInventoryLocation.CriticalQuantity = item.CriticalQuantity;
                                objInventoryLocation.eVMISensorID = item.eVMISensorID;
                                objInventoryLocation.eVMISensorPort = item.eVMISensorPort;
                                objInventoryLocation.IsDefault = item.IsDefault;
                                objInventoryLocation.ItemGUID = ItemGuid;
                                objInventoryLocation.Room = RoomId;
                                objInventoryLocation.CompanyID = CompanyID;
                                objInventoryLocation.AddedFrom = "Web";
                                objInventoryLocation.EditedFrom = "Web";
                                objInventoryLocation.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.IsOnlyFromItemUI = true;

                                objInventoryLocation.IsEnforceDefaultPullQuantity = item.IsEnforceDefaultPullQuantity;
                                objInventoryLocation.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
                                objInventoryLocation.DefaultPullQuantity = item.DefaultPullQuantity;
                                objInventoryLocation.DefaultReorderQuantity = item.DefaultReorderQuantity;
                                objInventoryLocation.BinUDF1 = item.BinUDF1;
                                objInventoryLocation.BinUDF2 = item.BinUDF2;
                                objInventoryLocation.BinUDF3 = item.BinUDF3;
                                objInventoryLocation.BinUDF4 = item.BinUDF4;
                                objInventoryLocation.BinUDF5 = item.BinUDF5;

                                objInventoryLocation = objBinMasterDAL.InsertBin(objInventoryLocation);
                                insertedBinIds.Add(objInventoryLocation.ID);
                            }
                            else
                            {
                                objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.LastUpdatedBy = UserID;
                                objInventoryLocation.MinimumQuantity = item.MinimumQuantity;
                                objInventoryLocation.MaximumQuantity = item.MaximumQuantity;
                                objInventoryLocation.CriticalQuantity = item.CriticalQuantity;
                                objInventoryLocation.eVMISensorID = item.eVMISensorID;
                                objInventoryLocation.eVMISensorPort = item.eVMISensorPort;
                                objInventoryLocation.IsDefault = item.IsDefault;
                                objInventoryLocation.EditedFrom = "Web";
                                objInventoryLocation.IsOnlyFromItemUI = true;

                                objInventoryLocation.IsEnforceDefaultPullQuantity = item.IsEnforceDefaultPullQuantity;
                                objInventoryLocation.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
                                objInventoryLocation.DefaultPullQuantity = item.DefaultPullQuantity;
                                objInventoryLocation.DefaultReorderQuantity = item.DefaultReorderQuantity;



                                objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;

                                objInventoryLocation.BinUDF1 = item.BinUDF1;
                                objInventoryLocation.BinUDF2 = item.BinUDF2;
                                objInventoryLocation.BinUDF3 = item.BinUDF3;
                                objInventoryLocation.BinUDF4 = item.BinUDF4;
                                objInventoryLocation.BinUDF5 = item.BinUDF5;

                                objBinMasterDAL.Edit(objInventoryLocation);
                            }

                            if (objInventoryLocation.IsDefault ?? false)
                            {
                                objItem.DefaultLocation = objInventoryLocation.ID;
                            }
                            objInventoryLocation.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            objInventoryLocation.ConsignedQuantity = item.ConsignedQuantity;
                            lstReturn.Add(objInventoryLocation);
                        }
                        IQueryable<BinMaster> lstallbins = context.BinMasters.Where(t => t.ItemGUID == ItemGuid && t.IsDeleted == false && t.IsStagingLocation == false);
                        String DeletedBinIds = "";
                        foreach (var item in lstallbins)
                        {
                            if (!lstReturn.Any(t => t.ID == item.ID))
                            {
                                bool isBinInUse = IsBinInUse_New(RoomId, CompanyID, item.ID, ItemGuid);
                                if (!isBinInUse)
                                {
                                    item.IsDeleted = true;
                                    item.EditedFrom = "Web-AssignUpdateItemLocations";
                                    item.LastUpdated = DateTimeUtility.DateTimeNow;
                                    item.LastUpdatedBy = UserID;
                                    DeletedBinIds = DeletedBinIds + (DeletedBinIds == "" ? "" : "#") + item.ItemGUID.ToString() + "," + item.ID.ToString();
                                }
                            }
                        }
                        context.SaveChanges();

                        if (!String.IsNullOrEmpty(DeletedBinIds) && !String.IsNullOrWhiteSpace(DeletedBinIds))
                        {
                            List<ItemMasterDTO> lstItemMaster = CSP_DeleteBinDataById(DeletedBinIds, CompanyID, RoomId, UserID, "Bin >> Delete Bin", SessionUserId);
                            if (lstItemMaster != null && lstItemMaster.Count > 0)
                            {
                                ItemMasterDTO objItemMaster = lstItemMaster.Where(x => x.GUID == ItemGuid).FirstOrDefault();
                                if (objItemMaster != null)
                                {
                                    UpdatedOnHandQuantity = objItemMaster.OnHandQuantity;
                                }
                            }
                        }

                        if (!StagingLocation)
                        {
                            IQueryable<BinMaster> lstItemBins = context.BinMasters.Where(t => t.ItemGUID == ItemGuid && t.IsDeleted == false && t.IsStagingLocation == false);
                            if (lstItemBins.Any())
                            {
                                foreach (var item in lstItemBins)
                                {
                                    ItemLocationeVMISetup objItemLocationeVMISetup = context.ItemLocationeVMISetups.FirstOrDefault(t => t.ItemGUID == ItemGuid && t.BinID == item.ID && (t.IsDeleted ?? false) == false);
                                    if (objItemLocationeVMISetup != null)
                                    {
                                        BinMasterDTO binm = lstLocations.FirstOrDefault(t => t.BinNumber.Trim() == item.BinNumber.Trim());
                                        if (binm != null)
                                        {
                                            objItemLocationeVMISetup.eVMISensorID = binm.eVMISensorID;
                                            objItemLocationeVMISetup.eVMISensorPort = binm.eVMISensorPort;
                                            objItemLocationeVMISetup.EditedFrom = "Web";
                                            objItemLocationeVMISetup.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        }
                                    }
                                    else
                                    {
                                        objItemLocationeVMISetup = new ItemLocationeVMISetup();
                                        objItemLocationeVMISetup.BinID = item.ID;
                                        objItemLocationeVMISetup.CompanyID = item.CompanyID;
                                        objItemLocationeVMISetup.eVMISensorID = lstLocations.Any(t => t.BinNumber.Trim().ToLower() == item.BinNumber.Trim().ToLower()) ? lstLocations.FirstOrDefault(t => t.BinNumber.Trim().ToLower() == item.BinNumber.Trim().ToLower()).eVMISensorID : null;
                                        objItemLocationeVMISetup.eVMISensorPort = lstLocations.Any(t => t.BinNumber.Trim().ToLower() == item.BinNumber.Trim().ToLower()) ? lstLocations.FirstOrDefault(t => t.BinNumber.Trim().ToLower() == item.BinNumber.Trim().ToLower()).eVMISensorPort : null;
                                        objItemLocationeVMISetup.IsArchived = false;
                                        objItemLocationeVMISetup.IsDeleted = false;
                                        objItemLocationeVMISetup.ItemGUID = item.ItemGUID;
                                        objItemLocationeVMISetup.LastUpdatedBy = item.CreatedBy;
                                        objItemLocationeVMISetup.Room = item.Room;
                                        objItemLocationeVMISetup.Updated = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetup.Created = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetup.CreatedBy = item.CreatedBy;
                                        objItemLocationeVMISetup.AddedFrom = "Web";
                                        objItemLocationeVMISetup.EditedFrom = "Web";
                                        objItemLocationeVMISetup.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetup.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        context.ItemLocationeVMISetups.Add(objItemLocationeVMISetup);
                                    }
                                }
                            }
                            context.SaveChanges();
                        }
                    }
                }
            }

            return lstReturn;
        }


        public List<BinMasterDTO> AssignUpdateItemLocationsOfEditItemImportSVC(List<ImportDTO.InventoryLocationDetailsImportDTO> lstLocations
                , Guid ItemGuid, long RoomId, long CompanyID, long UserID, bool StagingLocation, out double? UpdatedOnHandQuantity,
                out List<long> insertedBinIds, long SessionUserId)
        {
            List<BinMasterDTO> lstReturn = new List<BinMasterDTO>();
            UpdatedOnHandQuantity = null;
            insertedBinIds = new List<long>();

            if (lstLocations != null && lstLocations.Count > 0)
            {
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGuid);

                    if (objItem != null)
                    {
                        foreach (var item in lstLocations)
                        {
                            BinMasterDTO objBinMasterDTOToInsert = objBinMasterDAL.GetBinByName(item.BinNumber.Trim(), RoomId, CompanyID, StagingLocation);

                            if (objBinMasterDTOToInsert == null)
                            {
                                objBinMasterDTOToInsert = new BinMasterDTO();
                                objBinMasterDTOToInsert.BinNumber = item.BinNumber.Trim();
                                objBinMasterDTOToInsert.ParentBinId = null;
                                objBinMasterDTOToInsert.CreatedBy = UserID;
                                objBinMasterDTOToInsert.LastUpdatedBy = UserID;
                                objBinMasterDTOToInsert.Created = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.LastUpdated = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.Room = RoomId;
                                objBinMasterDTOToInsert.CompanyID = CompanyID;
                                objBinMasterDTOToInsert.AddedFrom = "Web";
                                objBinMasterDTOToInsert.EditedFrom = "Web";
                                objBinMasterDTOToInsert.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objBinMasterDTOToInsert.IsOnlyFromItemUI = true;
                                objBinMasterDTOToInsert.IsEnforceDefaultPullQuantity = item.IsEnforceDefaultPullQuantity;
                                objBinMasterDTOToInsert.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
                                objBinMasterDTOToInsert.DefaultPullQuantity = item.DefaultPullQuantity;
                                objBinMasterDTOToInsert.DefaultReorderQuantity = item.DefaultReorderQuantity;

                                objBinMasterDTOToInsert = objBinMasterDAL.InsertBin(objBinMasterDTOToInsert);
                            }

                            BinMasterDTO objInventoryLocation = GetInventoryLocation(objBinMasterDTOToInsert.ID, ItemGuid, RoomId, CompanyID);

                            if (objInventoryLocation == null)
                            {
                                objInventoryLocation = new BinMasterDTO();
                                objInventoryLocation.BinNumber = objBinMasterDTOToInsert.BinNumber;
                                objInventoryLocation.ParentBinId = objBinMasterDTOToInsert.ID;
                                objInventoryLocation.CreatedBy = UserID;
                                objInventoryLocation.LastUpdatedBy = UserID;
                                objInventoryLocation.Created = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;

                                if (objItem != null && !objItem.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                                {
                                    objInventoryLocation.MinimumQuantity = item.MinimumQuantity;
                                    objInventoryLocation.MaximumQuantity = item.MaximumQuantity;
                                    objInventoryLocation.CriticalQuantity = item.CriticalQuantity;
                                }

                                objInventoryLocation.eVMISensorID = item.eVMISensorID;
                                objInventoryLocation.eVMISensorPort = "";//item.eVMISensorPort;
                                objInventoryLocation.IsDefault = item.IsDefault;
                                objInventoryLocation.ItemGUID = ItemGuid;
                                objInventoryLocation.Room = RoomId;
                                objInventoryLocation.CompanyID = CompanyID;
                                objInventoryLocation.AddedFrom = "Web";
                                objInventoryLocation.EditedFrom = "Web";
                                objInventoryLocation.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.IsOnlyFromItemUI = true;
                                objInventoryLocation.IsEnforceDefaultPullQuantity = item.IsEnforceDefaultPullQuantity;
                                objInventoryLocation.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
                                objInventoryLocation.DefaultPullQuantity = item.DefaultPullQuantity;
                                objInventoryLocation.DefaultReorderQuantity = item.DefaultReorderQuantity;

                                objInventoryLocation = objBinMasterDAL.InsertBin(objInventoryLocation);
                                insertedBinIds.Add(objInventoryLocation.ID);
                            }
                            else
                            {
                                objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                                objInventoryLocation.LastUpdatedBy = UserID;

                                if (objItem != null && !objItem.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                                {
                                    objInventoryLocation.MinimumQuantity = item.MinimumQuantity;
                                    objInventoryLocation.MaximumQuantity = item.MaximumQuantity;
                                    objInventoryLocation.CriticalQuantity = item.CriticalQuantity;
                                }

                                objInventoryLocation.eVMISensorID = item.eVMISensorID;
                                objInventoryLocation.eVMISensorPort = "";//item.eVMISensorPort;
                                objInventoryLocation.IsDefault = item.IsDefault.GetValueOrDefault(false);
                                objInventoryLocation.EditedFrom = "SVC";
                                objInventoryLocation.IsOnlyFromItemUI = false;
                                objInventoryLocation.IsEnforceDefaultPullQuantity = item.IsEnforceDefaultPullQuantity;
                                objInventoryLocation.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
                                objInventoryLocation.DefaultPullQuantity = item.DefaultPullQuantity;
                                objInventoryLocation.DefaultReorderQuantity = item.DefaultReorderQuantity;
                                objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;

                                objBinMasterDAL.Edit(objInventoryLocation);
                            }

                            if (objInventoryLocation.IsDefault ?? false)
                            {
                                objItem.DefaultLocation = objInventoryLocation.ID;
                            }

                            objInventoryLocation.CustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            objInventoryLocation.ConsignedQuantity = item.ConsignedQuantity;
                            lstReturn.Add(objInventoryLocation);
                        }

                        IQueryable<BinMaster> lstallbins = context.BinMasters.Where(t => t.ItemGUID == ItemGuid && t.IsDeleted == false && t.IsStagingLocation == false);
                        string DeletedBinIds = "";

                        foreach (var item in lstallbins)
                        {
                            if (!lstReturn.Any(t => t.ID == item.ID))
                            {
                                bool isBinInUse = IsBinInUse_New(RoomId, CompanyID, item.ID, ItemGuid);

                                if (!isBinInUse)
                                {
                                    item.IsDeleted = true;
                                    item.EditedFrom = "Web-AssignUpdateItemLocations";
                                    item.LastUpdated = DateTimeUtility.DateTimeNow;
                                    item.LastUpdatedBy = UserID;
                                    DeletedBinIds = DeletedBinIds + (DeletedBinIds == "" ? "" : "#") + item.ItemGUID.ToString() + "," + item.ID.ToString();
                                }
                            }
                        }

                        context.SaveChanges();

                        if (!string.IsNullOrEmpty(DeletedBinIds) && !string.IsNullOrWhiteSpace(DeletedBinIds))
                        {
                            List<ItemMasterDTO> lstItemMaster = CSP_DeleteBinDataById(DeletedBinIds, CompanyID, RoomId, UserID, "Bin >> Delete Bin", SessionUserId);

                            if (lstItemMaster != null && lstItemMaster.Count > 0)
                            {
                                ItemMasterDTO objItemMaster = lstItemMaster.Where(x => x.GUID == ItemGuid).FirstOrDefault();

                                if (objItemMaster != null)
                                {
                                    UpdatedOnHandQuantity = objItemMaster.OnHandQuantity;
                                }
                            }
                        }

                        if (!StagingLocation)
                        {
                            IQueryable<BinMaster> lstItemBins = context.BinMasters.Where(t => t.ItemGUID == ItemGuid && t.IsDeleted == false && t.IsStagingLocation == false);

                            if (lstItemBins.Any())
                            {
                                foreach (var item in lstItemBins)
                                {
                                    ItemLocationeVMISetup objItemLocationeVMISetup = context.ItemLocationeVMISetups.FirstOrDefault(t => t.ItemGUID == ItemGuid && t.BinID == item.ID && (t.IsDeleted ?? false) == false);

                                    if (objItemLocationeVMISetup != null)
                                    {
                                        var binm = lstLocations.FirstOrDefault(t => t.BinNumber.Trim() == item.BinNumber.Trim());
                                        if (binm != null)
                                        {
                                            objItemLocationeVMISetup.eVMISensorID = binm.eVMISensorID;
                                            objItemLocationeVMISetup.eVMISensorPort = "";//binm.eVMISensorPort;
                                            objItemLocationeVMISetup.EditedFrom = "Web";
                                            objItemLocationeVMISetup.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        }
                                    }
                                    else
                                    {
                                        objItemLocationeVMISetup = new ItemLocationeVMISetup();
                                        objItemLocationeVMISetup.BinID = item.ID;
                                        objItemLocationeVMISetup.CompanyID = item.CompanyID;
                                        objItemLocationeVMISetup.eVMISensorID = lstLocations.Any(t => t.BinNumber.Trim().ToLower() == item.BinNumber.Trim().ToLower()) ? lstLocations.FirstOrDefault(t => t.BinNumber.Trim().ToLower() == item.BinNumber.Trim().ToLower()).eVMISensorID : null;
                                        objItemLocationeVMISetup.eVMISensorPort = null;
                                        objItemLocationeVMISetup.IsArchived = false;
                                        objItemLocationeVMISetup.IsDeleted = false;
                                        objItemLocationeVMISetup.ItemGUID = item.ItemGUID;
                                        objItemLocationeVMISetup.LastUpdatedBy = item.CreatedBy;
                                        objItemLocationeVMISetup.Room = item.Room;
                                        objItemLocationeVMISetup.Updated = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetup.Created = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetup.CreatedBy = item.CreatedBy;
                                        objItemLocationeVMISetup.AddedFrom = "SVC";
                                        objItemLocationeVMISetup.EditedFrom = "SVC";
                                        objItemLocationeVMISetup.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetup.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        context.ItemLocationeVMISetups.Add(objItemLocationeVMISetup);
                                    }
                                }
                            }
                            context.SaveChanges();
                        }
                    }
                }
            }

            return lstReturn;
        }

        public List<ItemMasterDTO> CSP_DeleteBinDataById(string ItemBinIDs, long CompanyID, long RoomID, long UserId, string WhatWhereAction, long SessionUserId)
        {
            try
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                Guid ItemGuid = Guid.Empty;
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                if (Connectionstring == "")
                {
                    return null;
                }

                List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();
                ItemMasterDTO objItemMaster;
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "CSP_DeleteBinDataById", ItemBinIDs, CompanyID, RoomID, UserId, WhatWhereAction);
                if (dsItems != null && dsItems.Tables.Count > 0)
                {
                    if (dsItems.Tables[0] != null && dsItems.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsItems.Tables[0].Rows)
                        {
                            objItemMaster = new ItemMasterDTO();

                            if (dsItems.Tables[0].Columns.Contains("ItemGUID"))
                            {
                                ItemGuid = Guid.Empty;
                                if (Guid.TryParse(Convert.ToString(dr["ItemGUID"]), out ItemGuid))
                                {
                                    objItemMaster.GUID = ItemGuid;
                                    objCartItemDAL.AutoCartUpdateByCode(ItemGuid, UserId, "web", WhatWhereAction, SessionUserId);
                                }
                            }

                            if (dsItems.Tables[0].Columns.Contains("ItemOnhandQty"))
                            {
                                double tmpdbl = 0;
                                if (double.TryParse(Convert.ToString(dr["ItemOnhandQty"]), out tmpdbl))
                                {
                                    objItemMaster.OnHandQuantity = tmpdbl;
                                }
                            }

                            lstItemMaster.Add(objItemMaster);
                        }
                    }
                }

                return lstItemMaster;
            }
            catch
            {
                return null;
            }
        }
        public string BinDuplicateCheck(long ID, string BinNumber, long RoomID, long CompanyID, bool IsStagingLocation)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var qry = (from em in context.BinMasters
                           where (em.BinNumber ?? string.Empty).ToLower() == BinNumber.ToLower() && (em.IsArchived ?? false) == false && em.IsDeleted == false && em.ID != ID && em.CompanyID == CompanyID && em.Room == RoomID && em.ParentBinId == null && (em.BinNumber ?? string.Empty).Replace(" ", string.Empty) != string.Empty && em.IsStagingLocation == IsStagingLocation
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
        public List<BinMasterDTO> GetItemLocations(Guid ItemGUID, Int64 RoomID, Int64 CompanyId)
        {
            List<BinMasterDTO> lstItemBins = new List<BinMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var lstItemLocationDetails = (from ILD in context.ItemLocationDetails
                                              where ILD.ItemGUID == ItemGUID
                                              group ILD by new { ILD.ItemGUID, ILD.BinID } into ILD_G
                                              select new
                                              {
                                                  ItemGUID = ILD_G.Key.ItemGUID,
                                                  BinID = ILD_G.Key.BinID,
                                                  CustomerOwnedQuantity = ILD_G.Sum(x => (x.CustomerOwnedQuantity == null ? 0 : x.CustomerOwnedQuantity)),
                                                  ConsignedQuantity = ILD_G.Sum(x => (x.ConsignedQuantity == null ? 0 : x.ConsignedQuantity))
                                              });

                lstItemBins = (from ci in context.BinMasters
                               join bmevmi in context.ItemLocationeVMISetups on new { iguid = ci.ItemGUID, bid = ci.ID, isdeleted = false } equals new { iguid = bmevmi.ItemGUID, bid = bmevmi.BinID ?? 0, isdeleted = bmevmi.IsDeleted ?? false } into ci_bmevmi_join
                               from ci_bmevmi in ci_bmevmi_join.DefaultIfEmpty()
                               join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                               from ci_cc in ci_cc_join.DefaultIfEmpty()
                               join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                               from ci_cu in ci_cu_join.DefaultIfEmpty()
                                   //join ilq in context.ItemLocationQTies on new { ci.ItemGUID, binid = ci.ID } equals new { ilq.ItemGUID, binid = ilq.BinID } into ci_ilq_join
                                   //from ci_ilq in ci_ilq_join.DefaultIfEmpty()
                               join ilq in lstItemLocationDetails on new { ci.ItemGUID, binid = ci.ID } equals new { ilq.ItemGUID, binid = (long)(ilq.BinID == null ? 0 : ilq.BinID) } into ci_ilq_join
                               from ci_ilq in ci_ilq_join.DefaultIfEmpty()
                               where ci.ItemGUID == ItemGUID && ci.Room == RoomID && ci.CompanyID == CompanyId && ci.IsDeleted == false && ci.IsStagingLocation == false
                               select new BinMasterDTO
                               {
                                   BinNumber = ci.BinNumber != null ? ci.BinNumber.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                   CompanyID = ci.CompanyID,
                                   Created = ci.Created,
                                   CreatedBy = ci.CreatedBy,
                                   CriticalQuantity = ci.CriticalQuantity,
                                   GUID = ci.GUID,
                                   ID = ci.ID,
                                   IsArchived = ci.IsArchived,
                                   IsDefault = ci.IsDefault,
                                   IsDeleted = ci.IsDeleted,
                                   IsStagingHeader = ci.IsStagingHeader,
                                   IsStagingLocation = ci.IsStagingLocation,
                                   ItemGUID = ci.ItemGUID,
                                   LastUpdated = ci.LastUpdated,
                                   LastUpdatedBy = ci.LastUpdatedBy,
                                   MaterialStagingGUID = ci.MaterialStagingGUID,
                                   MaximumQuantity = ci.MaximumQuantity,
                                   MinimumQuantity = ci.MinimumQuantity,
                                   ParentBinId = ci.ParentBinId,
                                   Room = ci.Room,
                                   SuggestedOrderQuantity = ci.SuggestedOrderQuantity,
                                   UDF1 = ci.UDF1,
                                   UDF10 = ci.UDF10,
                                   UDF2 = ci.UDF2,
                                   UDF3 = ci.UDF3,
                                   UDF4 = ci.UDF4,
                                   UDF5 = ci.UDF5,
                                   CreatedByName = ci_cc.UserName,
                                   UpdatedByName = ci_cu.UserName,
                                   eVMISensorID = (float?)ci_bmevmi.eVMISensorID,
                                   eVMISensorPort = ci_bmevmi.eVMISensorPort,
                                   CustomerOwnedQuantity = ci_ilq.CustomerOwnedQuantity,
                                   ConsignedQuantity = ci_ilq.ConsignedQuantity,
                                   AddedFrom = (ci.AddedFrom == null ? "Web" : ci.AddedFrom),
                                   EditedFrom = (ci.EditedFrom == null ? "Web" : ci.EditedFrom),
                                   ReceivedOn = (ci.ReceivedOn == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOn),
                                   ReceivedOnWeb = (ci.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : ci.ReceivedOnWeb),
                                   IsEnforceDefaultPullQuantity = ci.IsEnforceDefaultPullQuantity,
                                   IsEnforceDefaultReorderQuantity = ci.IsEnforceDefaultReorderQuantity,
                                   DefaultPullQuantity = ci.DefaultPullQuantity,
                                   DefaultReorderQuantity = ci.DefaultReorderQuantity,
                                   BinUDF1 = ci.BinUDF1,
                                   BinUDF2 = ci.BinUDF2,
                                   BinUDF3 = ci.BinUDF3,
                                   BinUDF4 = ci.BinUDF4,
                                   BinUDF5 = ci.BinUDF5,
                               }).ToList();
            }
            return lstItemBins;
        }

        public List<BinMasterDTO> GetItemLocationsForKitBreak(Guid ItemGUID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@RoomID", RoomId),
                                                    new SqlParameter("@CompanyID", CompanyId),
                                                    new SqlParameter("@ItemGUID", ItemGUID),
                                                 };

                return context.Database.SqlQuery<BinMasterDTO>("exec [GetItemLocationsForKitBreak] @RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }
        }

        public BinMasterDTO GetItemLocationsWithBinName(Guid ItemGUID, Int64 RoomID, Int64 CompanyId, string BinNumber)
        {
            BinMasterDTO lstItemBins = new BinMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var lstItemLocationDetails = (from ILD in context.ItemLocationDetails
                                              where ILD.ItemGUID == ItemGUID
                                              group ILD by new { ILD.ItemGUID, ILD.BinID } into ILD_G
                                              select new
                                              {
                                                  ItemGUID = ILD_G.Key.ItemGUID,
                                                  BinID = ILD_G.Key.BinID,

                                              });

                lstItemBins = (from ci in context.BinMasters
                               join bmevmi in context.ItemLocationeVMISetups on new { iguid = ci.ItemGUID, bid = ci.ID, isdeleted = false } equals new { iguid = bmevmi.ItemGUID, bid = bmevmi.BinID ?? 0, isdeleted = bmevmi.IsDeleted ?? false } into ci_bmevmi_join
                               from ci_bmevmi in ci_bmevmi_join.DefaultIfEmpty()
                               join cc in context.UserMasters on ci.CreatedBy equals cc.ID into ci_cc_join
                               from ci_cc in ci_cc_join.DefaultIfEmpty()
                               join cu in context.UserMasters on ci.LastUpdatedBy equals cu.ID into ci_cu_join
                               from ci_cu in ci_cu_join.DefaultIfEmpty()
                                   //join ilq in context.ItemLocationQTies on new { ci.ItemGUID, binid = ci.ID } equals new { ilq.ItemGUID, binid = ilq.BinID } into ci_ilq_join
                                   //from ci_ilq in ci_ilq_join.DefaultIfEmpty()
                               join ilq in lstItemLocationDetails on new { ci.ItemGUID, binid = ci.ID } equals new { ilq.ItemGUID, binid = (long)(ilq.BinID == null ? 0 : ilq.BinID) } into ci_ilq_join
                               from ci_ilq in ci_ilq_join.DefaultIfEmpty()
                               where ci.ItemGUID == ItemGUID && ci.Room == RoomID && ci.CompanyID == CompanyId && ci.IsDeleted == false && ci.BinNumber == BinNumber
                               select new BinMasterDTO
                               {
                                   BinNumber = ci.BinNumber,

                                   GUID = ci.GUID,
                                   ID = ci.ID,


                               }).FirstOrDefault();
            }
            return lstItemBins;
        }
        public BinMasterDTO InsertRoomDefaultBin(string BinNumber, long RoomId, long CopmpanyID, long UserID)
        {
            BinMasterDTO objBinMaster = GetBinByName(BinNumber, RoomId, CopmpanyID, false);
            if (objBinMaster == null)
            {
                objBinMaster = new BinMasterDTO();
                objBinMaster.BinNumber = BinNumber;
                objBinMaster.IsDeleted = false;
                objBinMaster.IsStagingLocation = false;
                objBinMaster.Created = DateTimeUtility.DateTimeNow;
                objBinMaster.LastUpdated = DateTimeUtility.DateTimeNow;
                objBinMaster.CreatedBy = UserID;
                objBinMaster.LastUpdatedBy = UserID;
                objBinMaster.AddedFrom = "Web";
                objBinMaster.EditedFrom = "Web";
                objBinMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                objBinMaster.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                objBinMaster = InsertBin(objBinMaster);
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    Room objRoom = context.Rooms.FirstOrDefault(t => t.ID == RoomId);
                    if (objRoom != null)
                    {
                        objRoom.DefaultBinID = objBinMaster.ID;
                        objRoom.DefaultBinName = objBinMaster.BinNumber;
                        context.SaveChanges();
                    }
                }
            }
            return objBinMaster;

        }
        public long GetOrInsertBinForPDA(string BinName, Guid? MaterialStagingGUID, Guid? ItemGUID, bool? IsDefault, double? SuggestedOrderQuantity, double? CriticalQuantity, double? MinimumQuantity, double? MaximunmQuantity, bool isStaggingHeader, bool isStagging, Int64 UserID, Int64 RoomID, Int64 CompanyId, long? ParentBinId, bool? IsEnforceDefaultPullQuantity = null, bool? IsEnforceDefaultReorderQuantity = null, double? DefaultPullQuantity = null, double? DefaultReorderQuantity = null)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                BinMaster objBinMaster = null;
                if (ItemGUID == null || ItemGUID == Guid.Empty)
                {
                    objBinMaster = context.BinMasters.FirstOrDefault(t => t.BinNumber == BinName && t.ItemGUID == null && t.CompanyID == CompanyId && t.Room == RoomID && t.IsStagingLocation == isStagging && t.ParentBinId == null);
                    if (objBinMaster == null)
                    {
                        objBinMaster = new BinMaster();
                        objBinMaster.BinNumber = BinName;
                        objBinMaster.IsStagingLocation = isStagging;
                        objBinMaster.Created = DateTimeUtility.DateTimeNow;
                        objBinMaster.LastUpdated = DateTimeUtility.DateTimeNow;
                        objBinMaster.CreatedBy = UserID;
                        objBinMaster.LastUpdatedBy = UserID;
                        objBinMaster.IsDeleted = false;
                        objBinMaster.IsArchived = false;
                        objBinMaster.ItemGUID = null;
                        objBinMaster.ParentBinId = null;
                        objBinMaster.GUID = Guid.NewGuid();
                        objBinMaster.Room = RoomID;
                        objBinMaster.CompanyID = CompanyId;
                        objBinMaster.IsDefault = IsDefault;
                        objBinMaster.AddedFrom = "PDA";
                        objBinMaster.EditedFrom = "PDA";
                        objBinMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objBinMaster.ReceivedOnWeb = DateTimeUtility.DateTimeNow;



                        context.BinMasters.Add(objBinMaster);
                        context.SaveChanges();
                    }
                }
                else
                {
                    objBinMaster = context.BinMasters.FirstOrDefault(t => t.BinNumber == BinName && t.ItemGUID == ItemGUID && t.CompanyID == CompanyId && t.Room == RoomID && t.IsStagingLocation == isStagging);
                    if (objBinMaster != null)
                    {
                        return objBinMaster.ID;
                    }
                    else
                    {
                        BinMaster objBinMasterparent = context.BinMasters.FirstOrDefault(t => t.BinNumber == BinName && t.CompanyID == CompanyId && t.Room == RoomID && t.IsStagingLocation == isStagging && t.ParentBinId == null);
                        if (objBinMasterparent == null)
                        {
                            objBinMasterparent = new BinMaster();
                            objBinMasterparent.BinNumber = BinName;
                            objBinMasterparent.IsStagingLocation = isStagging;
                            objBinMasterparent.Created = DateTimeUtility.DateTimeNow;
                            objBinMasterparent.LastUpdated = DateTimeUtility.DateTimeNow;
                            objBinMasterparent.CreatedBy = UserID;
                            objBinMasterparent.LastUpdatedBy = UserID;
                            objBinMasterparent.IsDeleted = false;
                            objBinMasterparent.IsArchived = false;
                            objBinMasterparent.ItemGUID = null;
                            objBinMasterparent.ParentBinId = null;
                            objBinMasterparent.GUID = Guid.NewGuid();
                            objBinMasterparent.Room = RoomID;
                            objBinMasterparent.CompanyID = CompanyId;
                            objBinMasterparent.IsDefault = IsDefault;
                            objBinMasterparent.AddedFrom = "PDA";
                            objBinMasterparent.EditedFrom = "PDA";
                            objBinMasterparent.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objBinMasterparent.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            context.BinMasters.Add(objBinMasterparent);
                            context.SaveChanges();
                        }

                        objBinMaster = new BinMaster();
                        objBinMaster.BinNumber = BinName;
                        objBinMaster.IsStagingLocation = isStagging;
                        objBinMaster.Created = DateTimeUtility.DateTimeNow;
                        objBinMaster.LastUpdated = DateTimeUtility.DateTimeNow;
                        objBinMaster.CreatedBy = UserID;
                        objBinMaster.LastUpdatedBy = UserID;
                        objBinMaster.IsDeleted = false;
                        objBinMaster.IsArchived = false;
                        objBinMaster.ItemGUID = ItemGUID;
                        objBinMaster.CriticalQuantity = CriticalQuantity;
                        objBinMaster.MinimumQuantity = MinimumQuantity;
                        objBinMaster.MaximumQuantity = MaximunmQuantity;
                        objBinMaster.GUID = Guid.NewGuid();
                        objBinMaster.ParentBinId = objBinMasterparent.ID;
                        objBinMaster.Room = RoomID;
                        objBinMaster.CompanyID = CompanyId;
                        objBinMaster.IsDefault = IsDefault;
                        objBinMaster.GUID = Guid.NewGuid();
                        objBinMaster.AddedFrom = "PDA";
                        objBinMaster.EditedFrom = "PDA";
                        objBinMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objBinMaster.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        objBinMaster.IsEnforceDefaultPullQuantity = IsEnforceDefaultPullQuantity;
                        objBinMaster.IsEnforceDefaultReorderQuantity = IsEnforceDefaultReorderQuantity;
                        objBinMaster.DefaultPullQuantity = DefaultPullQuantity;
                        objBinMaster.DefaultReorderQuantity = DefaultReorderQuantity;

                        context.BinMasters.Add(objBinMaster);
                        context.SaveChanges();

                    }
                }
                return objBinMaster.ID;
            }

        }
        public bool IsBinInUse_New(long RoomID, long CompanyID, long BinID, Guid ItemGUID)
        {
            bool retval = false;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            if (Connectionstring == "")
            {
                return false;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "CheckItemBinReference_New", RoomID, CompanyID, BinID, ItemGUID);
            if (dsBins != null && dsBins.Tables.Count > 0)
            {
                DataTable dt = dsBins.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        BinMasterDTO objBinMaster = new BinMasterDTO();
                        if (dt.Columns.Contains("CustomerOwnedQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["CustomerOwnedQuantity"]), out tmpdbl);
                            objBinMaster.CustomerOwnedQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("ConsignedQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["ConsignedQuantity"]), out tmpdbl);
                            objBinMaster.ConsignedQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("RequestedQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["RequestedQuantity"]), out tmpdbl);
                            objBinMaster.CriticalQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("QuantityRequisitioned"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["QuantityRequisitioned"]), out tmpdbl);
                            objBinMaster.MinimumQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("RequestedQuantityTransfer"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["RequestedQuantityTransfer"]), out tmpdbl);
                            objBinMaster.MaximumQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("SuggestedOrderQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["SuggestedOrderQuantity"]), out tmpdbl);
                            objBinMaster.SuggestedOrderQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("CountQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["CountQuantity"]), out tmpdbl);
                            objBinMaster.CountQuantity = tmpdbl;
                        }
                        if (dt.Columns.Contains("KitMoveInOutQuantity"))
                        {
                            double tmpdbl = 0;
                            double.TryParse(Convert.ToString(dr["KitMoveInOutQuantity"]), out tmpdbl);
                            objBinMaster.KitMoveInOutQuantity = tmpdbl;
                        }
                        lstBins.Add(objBinMaster);
                    }
                }
            }
            foreach (var item in lstBins)
            {
                if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 || item.ConsignedQuantity.GetValueOrDefault(0) > 0 || item.MinimumQuantity.GetValueOrDefault(0) > 0 || 
                    item.MaximumQuantity.GetValueOrDefault(0) > 0 || item.CriticalQuantity.GetValueOrDefault(0) > 0 || item.SuggestedOrderQuantity.GetValueOrDefault(0) > 0 || 
                    item.CountQuantity.GetValueOrDefault(0) > 0 || item.KitMoveInOutQuantity.GetValueOrDefault(0) > 0 )
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }
        public List<BinMasterDTO> CheckBinInUse_New(long RoomID, long CompanyID, long BinID, Guid ItemGUID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                if (Connectionstring == "")
                {
                    return null;
                }
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "CheckItemBinReference_New", RoomID, CompanyID, BinID, ItemGUID);
                if (dsBins != null && dsBins.Tables.Count > 0)
                {
                    DataTable dt = dsBins.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            BinMasterDTO objBinMaster = new BinMasterDTO();
                            if (dt.Columns.Contains("CustomerOwnedQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["CustomerOwnedQuantity"]), out tmpdbl);
                                objBinMaster.CustomerOwnedQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("ConsignedQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["ConsignedQuantity"]), out tmpdbl);
                                objBinMaster.ConsignedQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("RequestedQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["RequestedQuantity"]), out tmpdbl);
                                objBinMaster.CriticalQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("QuantityRequisitioned"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["QuantityRequisitioned"]), out tmpdbl);
                                objBinMaster.MinimumQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("RequestedQuantityTransfer"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["RequestedQuantityTransfer"]), out tmpdbl);
                                objBinMaster.MaximumQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("SuggestedOrderQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["SuggestedOrderQuantity"]), out tmpdbl);
                                objBinMaster.SuggestedOrderQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("CountQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["CountQuantity"]), out tmpdbl);
                                objBinMaster.CountQuantity = tmpdbl;
                            }
                            if (dt.Columns.Contains("KitMoveInOutQuantity"))
                            {
                                double tmpdbl = 0;
                                double.TryParse(Convert.ToString(dr["KitMoveInOutQuantity"]), out tmpdbl);
                                objBinMaster.KitMoveInOutQuantity = tmpdbl;
                            }
                            lstBins.Add(objBinMaster);
                        }
                    }
                }
                //foreach (var item in lstBins)
                //{
                //    if (((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0) + (item.MinimumQuantity ?? 0) + (item.MaximumQuantity ?? 0) + (item.CriticalQuantity ?? 0) + (item.SuggestedOrderQuantity ?? 0)) != 0)
                //    {
                //        retval = true;
                //        break;
                //    }
                //}
                return lstBins;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private Int64? GetOrInsertBinIDByNameItemGuid(Guid ItemGUID, string Name, Int64 userID, Int64 roomID, Int64 companyID, bool IsStagingLoc = false, string EditedFrom = "", bool IsFromService = false)
        {
            Int64? ID = null;
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name))
            {
                BinMasterDTO objBinMasterDTO = GetItemBinPlain(ItemGUID, Name, roomID, companyID, userID, IsStagingLoc, EditedFrom, IsFromService);
                ID = objBinMasterDTO.ID;
            }
            return ID;
        }
        public Int64? GetOrInsertBinIDByName(Guid ItemGuid, string Name, Int64 userID, Int64 roomID, Int64 companyID, bool IsStagingLoc = false)
        {
            Int64? BinID = GetOrInsertBinIDByNameItemGuid(ItemGuid, Name, userID, roomID, companyID, IsStagingLoc);

            if (!IsStagingLoc)
            {
                //BinMasterDTO itmLocLvQty = GetCachedDataLocationQty(ItemGuid, BinID.GetValueOrDefault(0), roomID, companyID);
                BinMasterDTO itmLocLvQty = GetBinByIDPlain(BinID.GetValueOrDefault(0), roomID, companyID);
                if (itmLocLvQty == null)
                {
                    itmLocLvQty = new BinMasterDTO()
                    {
                        ID = BinID.GetValueOrDefault(0),
                        BinNumber = Name,
                        ItemGUID = ItemGuid,
                        CreatedBy = userID,
                        CompanyID = companyID,
                        Room = roomID,
                        LastUpdatedBy = userID,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        GUID = Guid.NewGuid(),
                        IsStagingHeader = false,
                        IsStagingLocation = false,
                        CriticalQuantity = 0,
                        MinimumQuantity = 0,
                        MaximumQuantity = 0,
                        SuggestedOrderQuantity = 0,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow
                    };

                    Insert(itmLocLvQty);
                }
            }
            return BinID;
        }
        public Int64? GetOrInsertBinIDByBinName(Guid ItemGuid, string Name, Int64 userID, Int64 roomID, Int64 companyID, bool IsStagingLoc = false, string EditedFrom = "", bool IsFromService = false)
        {
            return GetOrInsertBinIDByNameItemGuid(ItemGuid, Name, userID, roomID, companyID, IsStagingLoc, EditedFrom, IsFromService);
        }
        public List<ItemsBins> GetItemsBinForLabels(string itemGuids)
        {
            List<ItemsBins> lstItemBin;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                /*lstItemBin = (from u in context.Database.SqlQuery<ItemsBins>(@"SELECT BM.ID BinID,BM.GUID BinGuid,
		                                                                              IM.ID AS ItemID,IM.[Guid] As ItemGuid,
	                                                                                  IM.ItemNumber, BM.BinNumber,IM.OnHandQuantity,
		                                                                              --ILQ.Quantity BinQuantity,
                                                                                      SUM(ISNULL(ILQ.CustomerOwnedQuantity,0) + ISNULL(ILQ.ConsignedQuantity,0)) AS BinQuantity,
	                                                                                  (CASE WHEN ISNULL(IM.IsItemLevelMinMaxQtyRequired,0)=0 THEN IsNull(BM.CriticalQuantity,0) ELSE NULL END) BinCriticalQuantity,
                                                                                      (CASE WHEN ISNULL(IM.IsItemLevelMinMaxQtyRequired,0)=0 THEN IsNull(BM.MinimumQuantity,0) ELSE NULL END) BinMinimumQuantity,
                                                                                      (CASE WHEN ISNULL(IM.IsItemLevelMinMaxQtyRequired,0)=0 THEN IsNull(BM.MaximumQuantity ,0) ELSE NULL END) BinMaximumQuantity
                                                                                FROM BinMaster BM Inner Join ItemMaster IM On BM.ItemGUID = IM.[Guid]
				                                                                              --Left outer join ItemLocationQTY ILQ ON IM.[GUID] = ILQ.ItemGUID AND BM.ID = ILQ.BinID
                                                                                                Left Outer join ItemLocationDetails ILQ ON IM.[GUID] = ILQ.ItemGUID AND BM.ID = ILQ.BinID
                                                                                                Left Outer join ItemLocationeVMISetup ILVM ON IM.[GUID] = ILVM.ItemGUID AND BM.ID = ILVM.BinID
                                                                                WHERE BM.ITEMGUID IN (SELECT SplitValue from dbo.Split('" + itemGuids + @"',','))
                                                                                   AND BM.BinNumber != '[|EmptyStagingBin|]' AND BM.IsStagingLocation=0  AND BM.ParentBinId >0  
                                                                                    AND IsNull(BM.IsDeleted,0) =0 AND IsNull(BM.IsArchived,0)=0
                                                                                    GROUP BY BM.ID,BM.GUID,IM.ID,IM.[Guid],IM.ItemNumber, BM.BinNumber,IM.OnHandQuantity,
                                                                                            IM.IsItemLevelMinMaxQtyRequired,IsNull(BM.CriticalQuantity,0),
                                                                                            ISNULL(BM.MinimumQuantity,0), IsNull(BM.MaximumQuantity ,0)")*/

                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUIDs", itemGuids) };
                lstItemBin = (from u in context.Database.SqlQuery<ItemsBins>("EXEC [GetItemsBinForLabels] @ItemGUIDs", params1)
                              select new ItemsBins
                              {
                                  BinID = u.BinID,
                                  BinCriticalQuantity = u.BinCriticalQuantity,
                                  BinGuid = u.BinGuid,
                                  BinMaximumQuantity = u.BinMaximumQuantity,
                                  BinMinimumQuantity = u.BinMinimumQuantity,
                                  BinNumber = u.BinNumber,
                                  BinQuantity = u.BinQuantity,
                                  ItemGuid = u.ItemGuid,
                                  ItemID = u.ItemID,
                                  ItemNumber = u.ItemNumber,
                                  OnHandQuantity = u.OnHandQuantity,
                              }).AsParallel().ToList();

                return lstItemBin;

            }
        }
        public List<ItemsBins> GetItemsBinForLabelsUsingSort(string itemGuids, string Sorting = "", string strNSItemLocation = "")
        {
            List<ItemsBins> lstItemBin;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUIDs", itemGuids),
                                                   new SqlParameter("@Sorting", Sorting),
                                                   new SqlParameter("@NSItemLocation", strNSItemLocation) };

                lstItemBin = (from u in context.Database.SqlQuery<ItemsBins>("exec [GetItemsBinForLabelsUsingSort] @ItemGUIDs,@Sorting,@NSItemLocation", params1)
                              select new ItemsBins
                              {
                                  BinID = u.BinID,
                                  BinCriticalQuantity = u.BinCriticalQuantity,
                                  BinGuid = u.BinGuid,
                                  BinMaximumQuantity = u.BinMaximumQuantity,
                                  BinMinimumQuantity = u.BinMinimumQuantity,
                                  BinNumber = u.BinNumber,
                                  BinQuantity = u.BinQuantity,
                                  ItemGuid = u.ItemGuid,
                                  ItemID = u.ItemID,
                                  ItemNumber = u.ItemNumber,
                                  OnHandQuantity = u.OnHandQuantity,
                              }).AsParallel().ToList();

                return lstItemBin;

            }
        }
        public BinMasterDTO GetItemBinByNameOrDefault(Guid ItemGUID, string BinNumber)
        {
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (!string.IsNullOrWhiteSpace(BinNumber))
                {
                    objBinMasterDTO = (from u in context.BinMasters
                                       where u.ItemGUID == ItemGUID && u.IsDeleted == false && u.BinNumber == BinNumber
                                       select new BinMasterDTO()
                                       {
                                           ID = u.ID,
                                           BinNumber = u.BinNumber,
                                           Created = u.Created,
                                           LastUpdated = u.LastUpdated,
                                           IsStagingLocation = u.IsStagingLocation,
                                           IsStagingHeader = u.IsStagingHeader,
                                           MaterialStagingGUID = u.MaterialStagingGUID,
                                           CreatedBy = u.CreatedBy,
                                           LastUpdatedBy = u.LastUpdatedBy,
                                           Room = u.Room,
                                           CompanyID = u.CompanyID,
                                           IsDeleted = u.IsDeleted,
                                           IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                           GUID = u.GUID,
                                           UDF1 = u.UDF1,
                                           UDF2 = u.UDF2,
                                           UDF3 = u.UDF3,
                                           UDF4 = u.UDF4,
                                           UDF5 = u.UDF5,
                                           ItemGUID = u.ItemGUID,
                                           MinimumQuantity = u.MinimumQuantity,
                                           MaximumQuantity = u.MaximumQuantity,
                                           CriticalQuantity = u.CriticalQuantity,
                                           SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                           IsDefault = u.IsDefault,
                                           AddedFrom = u.AddedFrom,
                                           EditedFrom = u.EditedFrom,
                                           ReceivedOn = u.ReceivedOn,
                                           ReceivedOnWeb = u.ReceivedOnWeb,
                                           IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                           IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                           DefaultPullQuantity = u.DefaultPullQuantity,
                                           DefaultReorderQuantity = u.DefaultReorderQuantity
                                       }).FirstOrDefault();
                }
                else
                {
                    objBinMasterDTO = (from u in context.BinMasters
                                       where u.ItemGUID == ItemGUID && u.IsDeleted == false && u.IsDefault == true
                                       select new BinMasterDTO()
                                       {
                                           ID = u.ID,
                                           BinNumber = u.BinNumber,
                                           Created = u.Created,
                                           LastUpdated = u.LastUpdated,
                                           IsStagingLocation = u.IsStagingLocation,
                                           IsStagingHeader = u.IsStagingHeader,
                                           MaterialStagingGUID = u.MaterialStagingGUID,
                                           CreatedBy = u.CreatedBy,
                                           LastUpdatedBy = u.LastUpdatedBy,
                                           Room = u.Room,
                                           CompanyID = u.CompanyID,
                                           IsDeleted = u.IsDeleted,
                                           IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                           GUID = u.GUID,
                                           UDF1 = u.UDF1,
                                           UDF2 = u.UDF2,
                                           UDF3 = u.UDF3,
                                           UDF4 = u.UDF4,
                                           UDF5 = u.UDF5,
                                           ItemGUID = u.ItemGUID,
                                           MinimumQuantity = u.MinimumQuantity,
                                           MaximumQuantity = u.MaximumQuantity,
                                           CriticalQuantity = u.CriticalQuantity,
                                           SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                           IsDefault = u.IsDefault,
                                           AddedFrom = u.AddedFrom,
                                           EditedFrom = u.EditedFrom,
                                           ReceivedOn = u.ReceivedOn,
                                           ReceivedOnWeb = u.ReceivedOnWeb,
                                           IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                                           IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                           DefaultPullQuantity = u.DefaultPullQuantity,
                                           DefaultReorderQuantity = u.DefaultReorderQuantity
                                       }).FirstOrDefault();
                }
            }
            return objBinMasterDTO;
        }
        public bool CSP_InsertCreditPullForNegativeQtyBins(string ItemBinIDs, long CompanyID, long RoomID, long UserId, string WhatWhereAction, string AddedFrom)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("EXEC CSP_InsertCreditPullForNegativeQtyBins '" + ItemBinIDs + "', " + CompanyID.ToString() + ", " + RoomID.ToString() + ", " + UserId.ToString() + ", '" + WhatWhereAction + "', '" + AddedFrom + "'");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CSP_DeletePull_ForAllNegativeItem(long RoomID, long CompanyID, long UserID, string WhatWhereAction, string EditedFrom)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("EXEC CSP_DeletePull_ForAllNegativeItem " + RoomID.ToString() + ", " + CompanyID.ToString() + ", " + UserID.ToString() + ", '" + EditedFrom + "', '" + WhatWhereAction + "'");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void UndeleteparentLocation(long BinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (context.BinMasters.Any(t => t.ID == BinID))
                {
                    context.BinMasters.First(t => t.ID == BinID).IsDeleted = false;
                    context.SaveChanges();
                }
            }

        }
        public List<BinMasterDTO> GetAllBinMastersByBinList(string strBinList, long RoomID, long CompanyID, bool IsStagingLoc = false)
        {
            List<BinMasterDTO> binList = new List<BinMasterDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsStagingLoc", IsStagingLoc), new SqlParameter("@strBinList", strBinList) };
                binList = (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetAllBinMastersByBinList] @strBinList,@RoomID,@CompanyID,@IsStagingLoc", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                           select new BinMasterDTO
                           {
                               ID = u.ID,
                               BinNumber = u.BinNumber,
                               Created = u.Created,
                               LastUpdated = u.LastUpdated,
                               IsStagingLocation = u.IsStagingLocation,
                               IsStagingHeader = u.IsStagingHeader,
                               MaterialStagingGUID = u.MaterialStagingGUID,
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               CompanyID = u.CompanyID,
                               IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                               IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                               GUID = u.GUID,
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               ItemGUID = u.ItemGUID,
                               MinimumQuantity = u.MinimumQuantity,
                               MaximumQuantity = u.MaximumQuantity,
                               CriticalQuantity = u.CriticalQuantity,
                               ConsignedQuantity = u.ConsignedQuantity,
                               SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                               IsDefault = u.IsDefault,
                               CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                               ItemNumber = u.ItemNumber,
                               eVMISensorID = u.eVMISensorID,
                               eVMISensorPort = u.eVMISensorPort,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                               IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                               DefaultPullQuantity = u.DefaultPullQuantity,
                               DefaultReorderQuantity = u.DefaultReorderQuantity
                           }).AsParallel().ToList();
                return binList;
            }

        }

        public List<BinMasterDTO> GetAllBinMastersByBinName(string strBinList, long RoomID, long CompanyID, bool IsStagingLoc = false)
        {
            List<BinMasterDTO> binList = new List<BinMasterDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsStagingLoc", IsStagingLoc), new SqlParameter("@strBinList", strBinList) };
                binList = (from u in context.Database.SqlQuery<BinMasterDTO>("exec [GetAllBinMastersByBinList] @strBinList,@RoomID,@CompanyID,@IsStagingLoc", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                           select new BinMasterDTO
                           {
                               ID = u.ID,
                               BinNumber = u.BinNumber,
                               Created = u.Created,
                               LastUpdated = u.LastUpdated,
                               IsStagingLocation = u.IsStagingLocation,
                               IsStagingHeader = u.IsStagingHeader,
                               MaterialStagingGUID = u.MaterialStagingGUID,
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               CompanyID = u.CompanyID,
                               IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                               IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                               GUID = u.GUID,
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               ItemGUID = u.ItemGUID,
                               MinimumQuantity = u.MinimumQuantity,
                               MaximumQuantity = u.MaximumQuantity,
                               CriticalQuantity = u.CriticalQuantity,
                               ConsignedQuantity = u.ConsignedQuantity,
                               SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                               IsDefault = u.IsDefault,
                               CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                               ItemNumber = u.ItemNumber,
                               eVMISensorID = u.eVMISensorID,
                               eVMISensorPort = u.eVMISensorPort,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                               IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                               DefaultPullQuantity = u.DefaultPullQuantity,
                               DefaultReorderQuantity = u.DefaultReorderQuantity
                           }).AsParallel().ToList();
                return binList;
            }

        }

        public List<BinMasterDTO> GetBinMasterFromParentID(string ParentBinIDs, string ItemGUIDs)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUIDs", ItemGUIDs),
                                                   new SqlParameter("@ParentBinIDs", ParentBinIDs) };

                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterFromParentID] @ItemGUIDs,@ParentBinIDs", params1).ToList<BinMasterDTO>();
            }
        }
        public List<ItemBinChangeHistory> GetItemBinChangeHistoryByUserID(DateTime Created, long UserID, long RoomID, long CompanyID, string ItemGUIDs)
        {
            List<ItemBinChangeHistory> lstBinHistory = new List<ItemBinChangeHistory>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Created", Created), new SqlParameter("@UserID", UserID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUIDs) };

                lstBinHistory = (from ibch in context.Database.SqlQuery<ItemBinChangeHistory>("exec [GetItemBinChangeHistoryByUserID] @Created,@UserID,@RoomID,@CompanyID,@ItemGUID", params1)
                                 select ibch).ToList();
            }

            return lstBinHistory;
        }
        public IEnumerable<BinMasterDTO> GetBinByLocationNameItemGuid(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string LocationName, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@BinNumber", LocationName), new SqlParameter("@ItemGuid", ItemGuid) };
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinByLocationNameItemGuid] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@BinNumber,@ItemGuid", params1).ToList();
            }
        }
        public List<DTOForAutoComplete> GetItemLocationsForNewPullGrid(Guid ItemGuid, long RoomID, long CompanyID, eTurnsRegionInfo objeTurnsRegionInfo, Guid? materialStagingGuid = null, string requisitionStatus = "Unsubmitted")
        {
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            BinMasterDAL objBinDAL = null;
            IEnumerable<BinMasterDTO> lstBinList = null;
            ItemLocationQTYDAL objLocationQtyDAL = null;
            ItemLocationQTYDTO objLocatQtyDTO = null;
            List<BinMasterDTO> retunList = new List<BinMasterDTO>();
            IEnumerable<MaterialStagingDetailDTO> lstMSDetailDTO = null;
            MaterialStagingDetailDAL objMSDAL = null;

            string qtyFormat = "N";
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();

            if (objeTurnsRegionInfo != null)
                qtyFormat = "N" + objeTurnsRegionInfo.NumberDecimalDigits;

            // wi -1221 related changes Allow a user to select a location in the requisition, even if the location has no quantity
            objBinDAL = new BinMasterDAL(base.DataBaseName);

            if (materialStagingGuid.HasValue && materialStagingGuid.Value != Guid.Empty)
            {
                lstBinList = GetAllStagingBinsForRequisitionByMSGUId(materialStagingGuid.Value, ItemGuid, CompanyID, RoomID);
            }
            else
            {
                //lstBinList = objBinDAL.GetAllRecords(RoomID, CompanyID, false, false).Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid);
                lstBinList = objBinDAL.GetInventoryAndStagingBinsByItem(RoomID, CompanyID, ItemGuid);
            }

            foreach (var item in lstBinList)
            {
                if (item.IsStagingLocation)
                {
                    objMSDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    //lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID).Where(x => x.Quantity > 0 && x.StagingBinID == item.ID);
                    if (materialStagingGuid.HasValue && materialStagingGuid.Value != Guid.Empty)
                    {
                        lstMSDetailDTO = objMSDAL.GetStagingLocationByItemAndStagingHeader(materialStagingGuid.Value, ItemGuid, item.ID, RoomID, CompanyID);
                    }
                    else
                    {
                        //lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID).Where(x => x.StagingBinID == item.ID);
                        lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID, item.ID, string.Empty, false);
                    }

                    //if (lstMSDetailDTO != null && lstMSDetailDTO.Count() > 0 && lstMSDetailDTO.Sum(x => x.Quantity) > 0)
                    if (lstMSDetailDTO != null && lstMSDetailDTO.Count() > 0)
                    {
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Key = item.BinNumber,
                            Value = item.BinNumber == "[|EmptyStagingBin|]" ? "[Staging](" + lstMSDetailDTO.Sum(x => x.Quantity) + ")" : item.BinNumber + " [Staging](" + lstMSDetailDTO.Sum(x => x.Quantity) + ")", //string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity))
                            //Value = item.BinNumber + " [Staging](" + lstMSDetailDTO.Sum(x => x.Quantity) + ")",
                            ID = item.ID,
                            GUID = item.GUID,
                        };
                        returnKeyValList.Add(obj);
                    }
                    else if (materialStagingGuid.HasValue && materialStagingGuid.Value != Guid.Empty)
                    {
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Key = item.BinNumber,
                            Value = item.BinNumber == "[|EmptyStagingBin|]" ? "[Staging](0)" : item.BinNumber + " [Staging](0)", //string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity))
                            //Value = item.BinNumber + " [Staging](" + lstMSDetailDTO.Sum(x => x.Quantity) + ")",
                            ID = item.ID,
                            GUID = item.GUID,
                        };
                        returnKeyValList.Add(obj);
                    }
                }
                else
                {
                    objLocationQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                    objLocatQtyDTO = objLocationQtyDAL.GetRecordByBinItem(ItemGuid, item.ID, RoomID, CompanyID);
                    if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                    {
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Key = item.BinNumber,
                            Value = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString(qtyFormat) + ")",
                            ID = item.ID,
                            GUID = item.GUID,
                        };
                        returnKeyValList.Add(obj);
                    }
                    else
                    {
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Key = item.BinNumber,
                            Value = item.BinNumber + " (0)",
                            ID = item.ID,
                            GUID = item.GUID,
                        };
                        returnKeyValList.Add(obj);
                    }
                }
            }


            //if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
            //{
            //    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
            //}
            return returnKeyValList;
            //return Json(returnKeyValList, JsonRequestBehavior.AllowGet);


        }
        public IEnumerable<BinMasterDTO> GetBinMasterByRoomID(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterByRoomID] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1).ToList();
            }
        }

        public bool UpdateItemBinOrderUOM(Guid ItemGUID, long RoomId, long CompanyId, bool IsSetOrderUOM)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@IsSetOrderUOM", IsSetOrderUOM) };
                context.Database.ExecuteSqlCommand("exec UpdateItemBinDefaultOrderUOM @ItemGUID,@RoomId,@CompanyId,@IsSetOrderUOM", params1);
            }
            return true;
        }

        public BinMasterDTO GetBinMasterByItemGUID(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived, string BinNumber, Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@Name", BinNumber), new SqlParameter("@ItemGuid", ItemGUID) };
                return (from u in context.Database.SqlQuery<BinMasterDTO>("EXEC GetBinMasterByItemGUID @RoomID,@CompanyID,@IsDeleted,@IsArchived,@Name,@ItemGuid", params1)
                        select new BinMasterDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            BinNumber = u.BinNumber,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            UDF6 = u.UDF6,
                            UDF7 = u.UDF7,
                            UDF8 = u.UDF8,
                            UDF9 = u.UDF9,
                            UDF10 = u.UDF10,
                            IsStagingLocation = u.IsStagingLocation,
                            IsStagingHeader = u.IsStagingHeader,
                            MaterialStagingGUID = u.MaterialStagingGUID,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            ItemGUID = u.ItemGUID,
                            SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                            IsDefault = u.IsDefault,
                            ParentBinId = u.ParentBinId,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            ReceivedOn = u.ReceivedOn,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            IsEnforceDefaultPullQuantity = u.IsEnforceDefaultPullQuantity,
                            IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultPullQuantity,
                            DefaultPullQuantity = u.DefaultPullQuantity,
                            DefaultReorderQuantity = u.DefaultReorderQuantity,
                            eVMISensorID = u.eVMISensorID,
                            eVMISensorPort = u.eVMISensorPort,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            ItemNumber = u.ItemNumber
                        }).FirstOrDefault();

            }
        }

        public bool UpdateDefaultBin(string ItemGUIDs, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUIDs", ItemGUIDs), new SqlParameter("@UserID", UserID) };
                context.Database.ExecuteSqlCommand("EXEC UpdateDefaultLocation @ItemGUIDs,@UserID", params1);
            }
            return true;
        }
        public List<BinMasterDTO> GetBinMasterByIdNormal(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", Convert.ToInt32(IDs)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterByIdNormal] @ID", params1).ToList();
            }
        }

        public IEnumerable<BinMasterDTO> GetItemLocatoinForeVMI(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetItemLocatoineVMI] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1).ToList();
            }
        }
        public IEnumerable<BinMasterDTO> GetItemLocatoinForeVMIByCOMPort(Int64 RoomID, Int64 CompanyID, long ComPortMappingID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ComPortMappingID", ComPortMappingID) };
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetItemLocatoinForeVMIByCOMPort] @RoomID,@CompanyID,@ComPortMappingID", params1).ToList();
            }
        }

        public void UnDeleteLocation(Int64 BinID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@BinID", BinID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID)};
                context.Database.ExecuteSqlCommand("EXEC UnDeleteLocation @BinID,@RoomID,@CompanyID", params1);
            }
        }
        #endregion

        public string DeleteBinMasterRecords(string Ids, string ModuleName, bool IsGUID, long UserID, string BinQtyMessage, string DeletedSuccessMessage)
        {
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_ModulewiseDelete", Ids, ModuleName, IsGUID, UserID, "");
            DataTable dt = new DataTable();
            string msg = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                    }
                }
                if (Successcnt > 0)
                {
                    msg = string.Format(DeletedSuccessMessage, Successcnt);
                }
                if (Failcnt > 0)
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = Failcnt + " " + BinQtyMessage;
                    }
                    else
                    {
                        msg = msg + " " + Failcnt + " " + BinQtyMessage;
                    }
                }


            }

            return msg;
        }

    }
}
