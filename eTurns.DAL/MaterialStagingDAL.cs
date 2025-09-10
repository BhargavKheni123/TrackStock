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
    public class MaterialStagingDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public MaterialStagingDAL(base.DataBaseName)
        //{

        //}

        public MaterialStagingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public MaterialStagingDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public IEnumerable<MaterialStagingDTO> GetAllMaterialStagings(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.MaterialStagings
                        join uc in context.UserMasters on u.CreatedBy equals uc.ID into u_uc_join
                        from u_uc in u_uc_join.DefaultIfEmpty()
                        join uu in context.UserMasters on u.LastUpdatedBy equals uu.ID into u_uu_join
                        from u_uu in u_uu_join.DefaultIfEmpty()
                        join rm in context.Rooms on u.Room equals rm.ID
                        where u.Room == RoomID && u.CompanyID == CompanyID
                        select new MaterialStagingDTO
                        {
                            ID = u.ID,
                            StagingName = u.StagingName,
                            Description = u.Description,
                            BinID = u.BinID,
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u_uc.UserName,
                            UpdatedByName = u_uu.UserName,
                            RoomName = rm.RoomName,
                            AppendedBarcodeString = string.Empty,
                            StagingLocationName = u.StagingLocationName,
                            StagingStatus = u.StagingStatus,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).AsParallel().ToList();

            }

        }

        public IEnumerable<MaterialStagingDTO> GetMaterialStaging(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string StagingName, int? StagingStatus)
        {
              
            List<MaterialStagingDTO> lstMaterialStaging;
             
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var param1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),  
                                                  new SqlParameter("@CompanyID", CompanyId),
                                                  new SqlParameter("@IsDeleted", IsDeleted),
                                                  new SqlParameter("@IsArchived", IsArchived),
                                                  new SqlParameter("@StagingName", StagingName ?? (object)DBNull.Value),
                                                  new SqlParameter("@StagingStatus", StagingStatus ?? (object)DBNull.Value) };

                lstMaterialStaging = (from u in context.Database.SqlQuery<MaterialStagingDTO>("exec [GetMaterialStagingByCompanyRoomID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@StagingName,@StagingStatus", param1)
                                        select new MaterialStagingDTO
                                        {
                                            ID = u.ID,
                                            StagingName = u.StagingName,
                                            Description = u.Description,
                                            BinID = u.BinID,
                                            GUID = u.GUID,
                                            UDF1 = u.UDF1,
                                            UDF2 = u.UDF2,
                                            UDF3 = u.UDF3,
                                            UDF4 = u.UDF4,
                                            UDF5 = u.UDF5,
                                            CompanyID = u.CompanyID,
                                            Room = u.Room,
                                            IsDeleted = u.IsDeleted,
                                            IsArchived = u.IsArchived,
                                            Created = u.Created,
                                            Updated = u.Updated,
                                            CreatedBy = u.CreatedBy,
                                            LastUpdatedBy = u.LastUpdatedBy,
                                            CreatedByName = u.CreatedByName,
                                            UpdatedByName = u.UpdatedByName,
                                            RoomName = u.RoomName,
                                            AppendedBarcodeString = string.Empty,
                                            StagingLocationName = u.StagingLocationName,
                                            StagingStatus = u.StagingStatus,
                                            AddedFrom = u.AddedFrom,
                                            EditedFrom = u.EditedFrom,
                                            ReceivedOn = u.ReceivedOn,
                                            ReceivedOnWeb = u.ReceivedOnWeb
                                        }).AsParallel().ToList();
            }

            return lstMaterialStaging;
        }

        public IEnumerable<MaterialStagingDTO> GetMaterialStagingUDF(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string UDFName)
        {
            
            List<MaterialStagingDTO> lstMaterialStaging;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var param1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                  new SqlParameter("@CompanyID", CompanyId),
                                                  new SqlParameter("@IsDeleted", IsDeleted),
                                                  new SqlParameter("@IsArchived", IsArchived),
                                                  new SqlParameter("@UDFName", UDFName) };

                lstMaterialStaging = (from u in context.Database.SqlQuery<MaterialStagingDTO>("exec [GetMaterialStagingByUDF] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@UDFName", param1)
                                      select new MaterialStagingDTO
                                      {
                                          ID = u.ID,
                                          StagingName = u.StagingName,
                                          Description = u.Description,
                                          BinID = u.BinID,
                                          GUID = u.GUID,
                                          UDF1 = u.UDF1,
                                          UDF2 = u.UDF2,
                                          UDF3 = u.UDF3,
                                          UDF4 = u.UDF4,
                                          UDF5 = u.UDF5,
                                          CompanyID = u.CompanyID,
                                          Room = u.Room,
                                          IsDeleted = u.IsDeleted,
                                          IsArchived = u.IsArchived,
                                          Created = u.Created,
                                          Updated = u.Updated,
                                          CreatedBy = u.CreatedBy,
                                          LastUpdatedBy = u.LastUpdatedBy,
                                          CreatedByName = u.CreatedByName,
                                          UpdatedByName = u.UpdatedByName,
                                          RoomName = u.RoomName,
                                          AppendedBarcodeString = string.Empty,
                                          StagingLocationName = u.StagingLocationName,
                                          StagingStatus = u.StagingStatus,
                                          AddedFrom = u.AddedFrom,
                                          EditedFrom = u.EditedFrom,
                                          ReceivedOn = u.ReceivedOn,
                                          ReceivedOnWeb = u.ReceivedOnWeb
                                      }).AsParallel().ToList();
            }

            return lstMaterialStaging;
        }

        public MaterialStagingDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            MaterialStagingDTO objMaterialStagingDTO = new MaterialStagingDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objMaterialStagingDTO = (from u in context.MaterialStagings
                                         join creater in context.UserMasters on u.CreatedBy equals creater.ID into u_creater_join
                                         from u_creater in u_creater_join.DefaultIfEmpty()
                                         join updatr in context.UserMasters on u.LastUpdatedBy equals updatr.ID into u_updatr_join
                                         from u_updatr in u_updatr_join.DefaultIfEmpty()
                                         join rm in context.Rooms on u.Room equals rm.ID
                                         where u.ID == id && u.Room == RoomID && u.CompanyID == CompanyID
                                         select new MaterialStagingDTO
                                         {
                                             ID = u.ID,
                                             StagingName = u.StagingName,
                                             Description = u.Description,
                                             BinID = u.BinID,
                                             GUID = u.GUID,
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5,
                                             CompanyID = u.CompanyID,
                                             Room = u.Room,
                                             IsDeleted = u.IsDeleted,
                                             IsArchived = u.IsArchived,
                                             Created = u.Created,
                                             Updated = u.Updated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             CreatedByName = u_creater.UserName,
                                             UpdatedByName = u_updatr.UserName,
                                             RoomName = rm.RoomName,
                                             StagingLocationName = u.StagingLocationName,
                                             StagingStatus = u.StagingStatus,
                                             AddedFrom = u.AddedFrom,
                                             EditedFrom = u.EditedFrom,
                                             ReceivedOn = u.ReceivedOn,
                                             ReceivedOnWeb = u.ReceivedOnWeb

                                         }).FirstOrDefault();
            }

            if (objMaterialStagingDTO != null && objMaterialStagingDTO.BinID > 0 && objMaterialStagingDTO.BinID.HasValue)
            {
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetBinByID(objMaterialStagingDTO.BinID.Value, (objMaterialStagingDTO.Room ?? 0), (objMaterialStagingDTO.CompanyID ?? 0));
                //BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetItemLocation( (objMaterialStagingDTO.Room ?? 0), (objMaterialStagingDTO.CompanyID ?? 0), false, false,Guid.Empty, objMaterialStagingDTO.BinID.Value,null,null).FirstOrDefault();
                if (objBinMasterDTO != null)
                {
                    objMaterialStagingDTO.StagingLocationName = objBinMasterDTO.BinNumber;
                }

            }
            return objMaterialStagingDTO;
        }

        public MaterialStagingDTO GetRecord(Guid Guid, Int64 RoomID, Int64 CompanyID)
        {
            MaterialStagingDTO objMaterialStagingDTO = new MaterialStagingDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objMaterialStagingDTO = (from u in context.MaterialStagings
                                         join creater in context.UserMasters on u.CreatedBy equals creater.ID into u_creater_join
                                         from u_creater in u_creater_join.DefaultIfEmpty()
                                         join updatr in context.UserMasters on u.LastUpdatedBy equals updatr.ID into u_updatr_join
                                         from u_updatr in u_updatr_join.DefaultIfEmpty()
                                         join rm in context.Rooms on u.Room equals rm.ID
                                         where u.GUID == Guid && u.Room == RoomID && u.CompanyID == CompanyID
                                         select new MaterialStagingDTO
                                         {
                                             ID = u.ID,
                                             StagingName = u.StagingName,
                                             Description = u.Description,
                                             BinID = u.BinID,
                                             GUID = u.GUID,
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5,
                                             CompanyID = u.CompanyID,
                                             Room = u.Room,
                                             IsDeleted = u.IsDeleted,
                                             IsArchived = u.IsArchived,
                                             Created = u.Created,
                                             Updated = u.Updated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             CreatedByName = u_creater.UserName,
                                             UpdatedByName = u_updatr.UserName,
                                             RoomName = rm.RoomName,
                                             StagingLocationName = u.StagingLocationName,
                                             StagingStatus = u.StagingStatus,
                                             AddedFrom = u.AddedFrom,
                                             EditedFrom = u.EditedFrom,
                                             ReceivedOn = u.ReceivedOn,
                                             ReceivedOnWeb = u.ReceivedOnWeb
                                         }).FirstOrDefault();
            }

            if (objMaterialStagingDTO != null && objMaterialStagingDTO.BinID > 0 && objMaterialStagingDTO.BinID.HasValue)
            {
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetBinByID(objMaterialStagingDTO.BinID.Value, (objMaterialStagingDTO.Room ?? 0), (objMaterialStagingDTO.CompanyID ?? 0));
                //BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetItemLocation( (objMaterialStagingDTO.Room ?? 0), (objMaterialStagingDTO.CompanyID ?? 0), false, false,Guid.Empty, objMaterialStagingDTO.BinID.Value,null,null).FirstOrDefault();
                if (objBinMasterDTO != null)
                {
                    objMaterialStagingDTO.StagingLocationName = objBinMasterDTO.BinNumber;
                }

            }
            return objMaterialStagingDTO;
        }

        public MaterialStagingDTO GetRecordByName(string StagingName, Int64 RoomID, Int64 CompanyID)
        {
            MaterialStagingDTO objMaterialStagingDTO = new MaterialStagingDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objMaterialStagingDTO = (from u in context.MaterialStagings
                                         join creater in context.UserMasters on u.CreatedBy equals creater.ID into u_creater_join
                                         from u_creater in u_creater_join.DefaultIfEmpty()
                                         join updatr in context.UserMasters on u.LastUpdatedBy equals updatr.ID into u_updatr_join
                                         from u_updatr in u_updatr_join.DefaultIfEmpty()
                                         join rm in context.Rooms on u.Room equals rm.ID
                                         where u.StagingName == StagingName && u.Room == RoomID && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                         select new MaterialStagingDTO
                                         {
                                             ID = u.ID,
                                             StagingName = u.StagingName,
                                             Description = u.Description,
                                             BinID = u.BinID,
                                             GUID = u.GUID,
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5,
                                             CompanyID = u.CompanyID,
                                             Room = u.Room,
                                             IsDeleted = u.IsDeleted,
                                             IsArchived = u.IsArchived,
                                             Created = u.Created,
                                             Updated = u.Updated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             CreatedByName = u_creater.UserName,
                                             UpdatedByName = u_updatr.UserName,
                                             RoomName = rm.RoomName,
                                             StagingLocationName = u.StagingLocationName,
                                             StagingStatus = u.StagingStatus,
                                             AddedFrom = u.AddedFrom,
                                             EditedFrom = u.EditedFrom,
                                             ReceivedOn = u.ReceivedOn,
                                             ReceivedOnWeb = u.ReceivedOnWeb
                                         }).FirstOrDefault();
            }

            if (objMaterialStagingDTO != null && objMaterialStagingDTO.BinID > 0 && objMaterialStagingDTO.BinID.HasValue)
            {
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetBinByID(objMaterialStagingDTO.BinID.Value, (objMaterialStagingDTO.Room ?? 0), (objMaterialStagingDTO.CompanyID ?? 0));
                //BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetItemLocation((objMaterialStagingDTO.Room ?? 0), (objMaterialStagingDTO.CompanyID ?? 0), false, false,Guid.Empty, objMaterialStagingDTO.BinID.Value, null,null).FirstOrDefault();
                if (objBinMasterDTO != null)
                {
                    objMaterialStagingDTO.StagingLocationName = objBinMasterDTO.BinNumber;
                }

            }
            return objMaterialStagingDTO;
        }

        public MaterialStagingDTO GetHistoryRecord(Int64 ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramsMS1 = new SqlParameter[] { new SqlParameter("@HistoryID", ID), new SqlParameter("@dbName", DataBaseName) };
                return context.Database.SqlQuery<MaterialStagingDTO>("exec [GetMaterialStagingChangeLogByHistoryID] @HistoryID,@dbName", paramsMS1).SingleOrDefault();
            }
        }

        public Int64 Insert(MaterialStagingDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStaging obj = new MaterialStaging();
                obj.ID = 0;
                obj.StagingName = objDTO.StagingName;
                obj.Description = objDTO.Description;
                obj.BinID = objDTO.BinID;
                obj.BinGUID = objDTO.BinGUID;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.StagingLocationName = objDTO.StagingLocationName;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.StagingStatus = objDTO.StagingStatus;
                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "MaterialStaging";

                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                obj.ReceivedOn = (objDTO.ReceivedOn);//objDTO.ReceivedOn == null ? DateTimeUtility.DateTimeNow :
                obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb);//objDTO.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : 

                obj.WhatWhereAction = objDTO.WhatWhereAction;

                context.MaterialStagings.Add(obj);
                context.SaveChanges();


                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                new AutoSequenceDAL(base.DataBaseName).UpdateNextStagingNumber(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.StagingName);
                //new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("NextStagingNo", objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.StagingName);
                return obj.ID;
            }

        }

        public bool Edit(MaterialStagingDTO objDTO, long SessionUserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStaging obj = context.MaterialStagings.FirstOrDefault(t => t.ID == objDTO.ID);

                if (obj != null)
                {
                    var tmpStagingStatus = obj.StagingStatus;
                    objDTO.Created = obj.Created;
                    objDTO.CreatedBy = obj.CreatedBy;
                    objDTO.IsArchived = obj.IsArchived;
                    objDTO.IsDeleted = obj.IsDeleted;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = obj.GUID;
                    obj.ID = objDTO.ID;
                    obj.StagingName = objDTO.StagingName;
                    obj.Description = objDTO.Description;
                    obj.BinID = objDTO.BinID;
                    obj.BinGUID = objDTO.BinGUID;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.Room = objDTO.Room;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.StagingLocationName = objDTO.StagingLocationName;
                    obj.StagingStatus = objDTO.StagingStatus;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "MaterialStaging";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;

                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    objDTO.GUID = obj.GUID;
                    objDTO.CreatedBy = obj.CreatedBy;
                    objDTO.LastUpdatedBy = obj.LastUpdatedBy;
                    objDTO.Room = obj.Room;
                    objDTO.CompanyID = obj.CompanyID;
                    //obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                    //obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : objDTO.ReceivedOnWeb);
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        obj.ReceivedOn = objDTO.ReceivedOn;
                        obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                    }
                    context.SaveChanges();
                    if (objDTO.StagingStatus == 0 && tmpStagingStatus != 0)
                    {
                        CloseMaterialStaging(objDTO, SessionUserId);
                    }
                }
                return true;
            }
        }

        public void CloseMaterialStaging(MaterialStagingDTO objDTO, long SessionUserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<MaterialStagingDetail> lstStagingItems = context.MaterialStagingDetails.Where(t => t.MaterialStagingGUID == objDTO.GUID && (t.IsDeleted ?? false) == false);
                if (lstStagingItems != null && lstStagingItems.Any())
                {
                    foreach (var StagingDetailitem in lstStagingItems)
                    {
                        ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == StagingDetailitem.ItemGUID);
                        IQueryable<MaterialStagingPullDetail> lstStagingPullDetails = context.MaterialStagingPullDetails.Where(t => t.MaterialStagingdtlGUID == StagingDetailitem.GUID && ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0)) > 0);
                        if (lstStagingPullDetails != null && lstStagingPullDetails.Any())
                        {
                            foreach (var StagingPullDetailitem in lstStagingPullDetails)
                            {
                                if (StagingPullDetailitem.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                                    continue;

                                //ItemLocationDetail objItemLocationDetail = new ItemLocationDetail();
                                ItemLocationDetail objItemLocationDetail = context.ItemLocationDetails.FirstOrDefault(x => x.GUID == StagingPullDetailitem.ItemLocationDetailGUID);
                                if (objItemLocationDetail != null)
                                {
                                    ItemLocationDetail objNewItemLocationDetail = new ItemLocationDetail();
                                    objNewItemLocationDetail.BinID = objItemMaster.DefaultLocation;
                                    objNewItemLocationDetail.CompanyID = StagingPullDetailitem.CompanyID;
                                    objNewItemLocationDetail.ConsignedQuantity = StagingPullDetailitem.ConsignedQuantity;
                                    objNewItemLocationDetail.Cost = StagingPullDetailitem.ItemCost;
                                    objNewItemLocationDetail.Created = DateTime.UtcNow;
                                    objNewItemLocationDetail.CreatedBy = objDTO.CreatedBy;
                                    objNewItemLocationDetail.CustomerOwnedQuantity = StagingPullDetailitem.CustomerOwnedQuantity;
                                    objNewItemLocationDetail.Expiration = objItemLocationDetail.Expiration;
                                    objNewItemLocationDetail.ExpirationDate = objItemLocationDetail.ExpirationDate;
                                    objNewItemLocationDetail.GUID = Guid.NewGuid();
                                    objNewItemLocationDetail.ID = 0;
                                    objNewItemLocationDetail.InitialQuantity = (StagingPullDetailitem.CustomerOwnedQuantity ?? 0) + (StagingPullDetailitem.ConsignedQuantity ?? 0);
                                    objNewItemLocationDetail.InitialQuantityPDA = 0;
                                    objNewItemLocationDetail.InitialQuantityWeb = (StagingPullDetailitem.CustomerOwnedQuantity ?? 0) + (StagingPullDetailitem.ConsignedQuantity ?? 0);
                                    objNewItemLocationDetail.IsArchived = false;
                                    objNewItemLocationDetail.IsConsignedSerialLot = false;
                                    objNewItemLocationDetail.IsDeleted = false;
                                    objNewItemLocationDetail.IsPDAEdit = false;
                                    objNewItemLocationDetail.IsWebEdit = false;
                                    objNewItemLocationDetail.ItemGUID = StagingPullDetailitem.ItemGUID;
                                    objNewItemLocationDetail.KitDetailGUID = null;
                                    objNewItemLocationDetail.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objNewItemLocationDetail.LotNumber = StagingPullDetailitem.LotNumber;
                                    objNewItemLocationDetail.OrderDetailGUID = null;
                                    objNewItemLocationDetail.PULLGUID = null;
                                    objNewItemLocationDetail.Received = StagingPullDetailitem.Received;
                                    objNewItemLocationDetail.RefPDASelfGUID = null;
                                    objNewItemLocationDetail.RefWebSelfGUID = null;
                                    objNewItemLocationDetail.Room = StagingPullDetailitem.Room;
                                    objNewItemLocationDetail.SerialNumber = StagingPullDetailitem.SerialNumber;
                                    objNewItemLocationDetail.TransferDetailGUID = null;
                                    objNewItemLocationDetail.Updated = DateTimeUtility.DateTimeNow;
                                    objNewItemLocationDetail.AddedFrom = "Web";
                                    objNewItemLocationDetail.EditedFrom = "Web";
                                    objNewItemLocationDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objNewItemLocationDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objNewItemLocationDetail.InsertedFrom = "Staging-Close";
                                    context.ItemLocationDetails.Add(objNewItemLocationDetail);
                                }

                                StagingPullDetailitem.CustomerOwnedQuantity = 0;
                                StagingPullDetailitem.ConsignedQuantity = 0;
                                StagingPullDetailitem.EditedFrom = "Web Staging-Close";
                                StagingPullDetailitem.Updated = DateTimeUtility.DateTimeNow;
                                StagingPullDetailitem.ReceivedOn = DateTimeUtility.DateTimeNow;
                                StagingPullDetailitem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            }
                        }

                        StagingDetailitem.EditedFrom = "Staging-Close";
                        StagingDetailitem.Quantity = 0;

                    }
                    context.SaveChanges();
                    List<Guid> itemGUIDs = lstStagingItems.Where(t => t.ItemGUID != null).Select(t => t.ItemGUID.Value).Distinct().ToList();
                    DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                    CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                    foreach (var itm in itemGUIDs)
                    {
                        ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == itm);
                        double ohQty = context.ItemLocationDetails.Where(t => t.ItemGUID == itm).Sum(t => ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0)));
                        objItemMaster.OnHandQuantity = ohQty;
                        objItemMaster.WhatWhereAction = "StageClose";
                        ohQty = context.ItemLocationDetails.Where(t => t.ItemGUID == itm).Sum(t => ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0)) * ((double)(t.Cost ?? 0)));
                        objItemMaster.ExtendedCost = ohQty;
                        ohQty = context.MaterialStagingDetails.Where(t => t.ItemGUID == itm && (t.IsDeleted ?? false) == false).Sum(t => (t.Quantity ?? 0));
                        objItemMaster.StagedQuantity = ohQty;
                        objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, itm, objDTO.CreatedBy ?? 0, null, null);
                        objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, itm, objDTO.CreatedBy ?? 0, SessionUserId, null, null);
                        //objCartItemDAL.AutoCartUpdateByCode(itm, objDTO.LastUpdatedBy ?? 0, "web", "MaterialStatginDAL >> CloseMaterialStaging");
                        objCartItemDAL.AutoCartUpdateByCode(itm, objDTO.LastUpdatedBy ?? 0, "web", "Inventory >> CloseStaging", SessionUserId);

                        var ilqtys = (from ilq in context.ItemLocationDetails
                                      where ilq.ItemGUID == itm
                                      group ilq by new { ilq.ItemGUID, ilq.BinID, ilq.Room, ilq.CompanyID } into groupedilq
                                      select new
                                      {
                                          ItemGUID = groupedilq.Key.ItemGUID,
                                          BinID = groupedilq.Key.BinID,
                                          Room = groupedilq.Key.Room,
                                          CompanyID = groupedilq.Key.CompanyID,
                                          CustomerOwnedQuantity = groupedilq.Sum(t => (t.CustomerOwnedQuantity ?? 0)),
                                          ConsignedQuantity = groupedilq.Sum(t => (t.ConsignedQuantity ?? 0)),
                                      });
                        if (ilqtys.Any())
                        {
                            foreach (var ilqty in ilqtys)
                            {
                                ItemLocationQTY objItemLocationQty = context.ItemLocationQTies.FirstOrDefault(t => t.ItemGUID == ilqty.ItemGUID && t.BinID == ilqty.BinID);
                                if (objItemLocationQty != null)
                                {
                                    objItemLocationQty.CustomerOwnedQuantity = ilqty.CustomerOwnedQuantity;
                                    objItemLocationQty.ConsignedQuantity = ilqty.ConsignedQuantity;
                                    objItemLocationQty.Quantity = (objItemLocationQty.CustomerOwnedQuantity ?? 0) + (objItemLocationQty.ConsignedQuantity ?? 0);
                                    objItemLocationQty.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objItemLocationQty.LastUpdated = DateTimeUtility.DateTimeNow;
                                    objItemLocationQty.EditedFrom = "Web StageClose";
                                    objItemLocationQty.ReceivedOn = DateTimeUtility.DateTimeNow;

                                }
                                else
                                {
                                    objItemLocationQty = new ItemLocationQTY();
                                    objItemLocationQty.BinID = ilqty.BinID ?? 0;
                                    objItemLocationQty.CompanyID = ilqty.CompanyID;
                                    objItemLocationQty.ConsignedQuantity = ilqty.ConsignedQuantity;
                                    objItemLocationQty.Created = DateTimeUtility.DateTimeNow;
                                    objItemLocationQty.CreatedBy = objDTO.CreatedBy;
                                    objItemLocationQty.CustomerOwnedQuantity = ilqty.CustomerOwnedQuantity;
                                    objItemLocationQty.GUID = Guid.NewGuid();
                                    objItemLocationQty.ID = 0;
                                    objItemLocationQty.ItemGUID = ilqty.ItemGUID;
                                    objItemLocationQty.LastUpdated = DateTimeUtility.DateTimeNow;
                                    objItemLocationQty.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objItemLocationQty.LotNumber = null;
                                    objItemLocationQty.Quantity = (objItemLocationQty.CustomerOwnedQuantity ?? 0) + (objItemLocationQty.ConsignedQuantity ?? 0);
                                    objItemLocationQty.Room = ilqty.Room;
                                    objItemLocationQty.EditedFrom = "Web StageClose";
                                    objItemLocationQty.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objItemLocationQty.AddedFrom = "Web";
                                    objItemLocationQty.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    context.ItemLocationQTies.Add(objItemLocationQty);
                                }
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MaterialStaging SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                //if (context.Database.ExecuteSqlCommand(strQuery) > 0)
                //{
                //    foreach (var item in IDs.Split(','))
                //    {
                //        if (!string.IsNullOrEmpty(item.Trim()))
                //        {
                //            MaterialStagingDetailDAL objDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                //            List<MaterialStagingDetailDTO> lst = objDAL.GetAllRecords(Guid.Parse(item), RoomID, CompanyID).ToList();
                //            //string.Join(",", new List<string>(lst.Select(t => t.ID.ToString())).ToArray());
                //            if (lst != null && lst.Count > 0)
                //                objDAL.DeleteRecords(string.Join(",", new List<string>(lst.Select(t => t.GUID.ToString())).ToArray()), userid, CompanyID, RoomID);
                //        }
                //    }
                //}
                return true;
            }
        }

        public long SaveMaterialStaging(MaterialStagingDTO objMaterialStagingDTO, long SessionUserId)
        {
            //BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
            //BinMasterDTO objBinMasterDTO = null;
            long retId = 0;
            //if (!string.IsNullOrWhiteSpace(objMaterialStagingDTO.StagingLocationName))
            //{
            //    objBinMasterDTO = objBinMasterDAL.GetAllRecords(objMaterialStagingDTO.Room.Value, objMaterialStagingDTO.CompanyID.Value, false, false).Where(t => t.IsStagingLocation == true && t.BinNumber.ToLower() == objMaterialStagingDTO.StagingLocationName.Trim().ToLower()).FirstOrDefault();
            //    if (objBinMasterDTO == null)
            //    {
            //        objBinMasterDTO = new BinMasterDTO();
            //        objBinMasterDTO.BinNumber = objMaterialStagingDTO.StagingLocationName.Trim();
            //        objBinMasterDTO.CompanyID = objMaterialStagingDTO.CompanyID;
            //        objBinMasterDTO.Created = objMaterialStagingDTO.Created;
            //        objBinMasterDTO.CreatedBy = objMaterialStagingDTO.CreatedBy;
            //        objBinMasterDTO.CreatedByName = objMaterialStagingDTO.CreatedByName;
            //        objBinMasterDTO.GUID = Guid.NewGuid();
            //        objBinMasterDTO.IsArchived = false;
            //        objBinMasterDTO.IsDeleted = false;
            //        objBinMasterDTO.Room = objMaterialStagingDTO.Room;
            //        objBinMasterDTO.RoomName = objMaterialStagingDTO.RoomName;
            //        objBinMasterDTO.IsStagingLocation = true;
            //        objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);
            //    }
            //}

            //if (objBinMasterDTO != null)
            //{
            //    objMaterialStagingDTO.BinID = objBinMasterDTO.ID;
            //    objMaterialStagingDTO.BinGUID = objBinMasterDTO.GUID;
            //}
            if (objMaterialStagingDTO.ID < 1)
            {
                objMaterialStagingDTO.AddedFrom = "Web";
                objMaterialStagingDTO.EditedFrom = "Web";
                objMaterialStagingDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objMaterialStagingDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                retId = Insert(objMaterialStagingDTO);
            }
            else
            {
                objMaterialStagingDTO.EditedFrom = "Web";
                objMaterialStagingDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                Edit(objMaterialStagingDTO, SessionUserId);
                retId = objMaterialStagingDTO.ID;
            }
            return retId;
        }

        public long UpdateMaterialStagingLineItem(Guid MsGUID, Guid ItmGUID, long msbinID, string StagingBinName, long RoomId, long ComapnyID, long CretedBy, string UserName, string RoomName)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            MaterialStagingDAL objMaterialStagingDAL = new MaterialStagingDAL(base.DataBaseName);
            MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            MaterialStagingDTO objMaterialStagingDTO = new MaterialStagingDTO();
            BinMasterDTO StagingBin = new BinMasterDTO();

            objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItmGUID);
            objMaterialStagingDTO = objMaterialStagingDAL.GetRecord(MsGUID, RoomId, ComapnyID);
            StagingBin = objBinMasterDAL.GetItemBinPlain(ItmGUID, StagingBinName, RoomId, ComapnyID, CretedBy, true);
            //StagingBin = objBinMasterDAL.GetAllRecords(RoomId, ComapnyID, false, false).Where(t => t.IsStagingLocation == true && t.IsArchived == false && t.IsDeleted == false && t.BinNumber.ToLower() == StagingBinName.Trim().ToLower()).FirstOrDefault();
            //if (StagingBin == null)
            //{
            //    StagingBin = new BinMasterDTO();
            //    StagingBin.Action = "I";
            //    StagingBin.BinNumber = StagingBinName.Trim();
            //    StagingBin.CompanyID = ComapnyID;
            //    StagingBin.Created = DateTimeUtility.DateTimeNow;
            //    StagingBin.CreatedBy = CretedBy;
            //    StagingBin.CreatedByName = UserName;
            //    StagingBin.GUID = Guid.NewGuid();
            //    StagingBin.IsArchived = false;
            //    StagingBin.IsDeleted = false;
            //    StagingBin.IsStagingLocation = true;
            //    StagingBin.LastUpdated = DateTimeUtility.DateTimeNow;
            //    StagingBin.LastUpdatedBy = CretedBy;
            //    StagingBin.Room = RoomId;
            //    StagingBin.RoomName = RoomName;
            //    StagingBin.UpdatedByName = UserName;
            //    StagingBin = objBinMasterDAL.InsertBin(StagingBin);

            //}

            if (objItemMasterDTO != null && objMaterialStagingDTO != null && StagingBin != null)
            {
                //IEnumerable<MaterialStagingDetailDTO> lstItems = objMaterialStagingDetailDAL.GetAllRecords(RoomId, ComapnyID).Where(t => t.MaterialStagingGUID == MsGUID && t.IsDeleted == false && t.IsArchived == false && t.ItemGUID == ItmGUID && t.StagingBinID == msbinID);
                IEnumerable<MaterialStagingDetailDTO> lstItems = objMaterialStagingDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(MsGUID), Convert.ToString(ItmGUID), msbinID, RoomId, ComapnyID, false, false);
                if (lstItems.Any())
                {
                    foreach (var item in lstItems)
                    {
                        item.StagingBinID = StagingBin.ID;
                        item.StagingBinGUID = StagingBin.GUID;
                        objMaterialStagingDetailDAL.Edit(item);
                        updateMaterialsStagingPullDetails(item.GUID, StagingBin.ID, StagingBin.GUID);

                    }

                }
            }

            return 0;
        }

        public void updateMaterialsStagingPullDetails(Guid MSdtlGUID, long StagingBinID, Guid StagingBinGUID)
        {
            using (var Context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<MaterialStagingPullDetail> lstmspulldetails = Context.MaterialStagingPullDetails.Where(t => t.MaterialStagingdtlGUID == MSdtlGUID);
                foreach (var item in lstmspulldetails)
                {
                    item.StagingBinId = StagingBinID;
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.EditedFrom = "Web";
                }
                Context.SaveChanges();
            }

        }

        public bool MaterialStagingDuplicatecheck(Guid Guid, string StagingName, long RoomId, long CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", Guid),
                                                   new SqlParameter("@StagingName", StagingName),
                                                   new SqlParameter("@RoomId", RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId)
                                                 };
                return context.Database.SqlQuery<bool>("exec [MaterialStagingDuplicateCheck] @GUID,@StagingName,@RoomId,@CompanyId", params1).FirstOrDefault();
            }
        }

        public List<MaterialStagingDTO> GetPagedMaterialStagingsFromDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {

            List<MaterialStagingDTO> lstStagings = new List<MaterialStagingDTO>();
            TotalCount = 0;
            MaterialStagingDTO objMaterialStagingDTO = new MaterialStagingDTO();
            DataSet dsStaging = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstStagings;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string StagingBins = null;
            string StagingCreaters = null;
            string StagingUpdators = null;
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
                dsStaging = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedMaterialStagings", StartRowIndex, MaxRows, SearchTerm, sortColumnName, StagingBins, StagingCreaters, StagingUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //WI-1461 related changes 
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
                    StagingCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    StagingUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
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


                if (!string.IsNullOrWhiteSpace(FieldsPara[20]))
                {
                    StagingBins = FieldsPara[20].TrimEnd(',');
                }
                dsStaging = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedMaterialStagings", StartRowIndex, MaxRows, SearchTerm, sortColumnName, StagingBins, StagingCreaters, StagingUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else
            {
                dsStaging = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedMaterialStagings", StartRowIndex, MaxRows, SearchTerm, sortColumnName, StagingBins, StagingCreaters, StagingUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }

            if (dsStaging != null && dsStaging.Tables.Count > 0)
            {
                DataTable dtStaging = dsStaging.Tables[0];
                if (dtStaging.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtStaging.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dtStaging.Rows)
                    {
                        long templong = 0;
                        Guid tempguid = Guid.Empty;
                        bool tempbool = false;
                        objMaterialStagingDTO = new MaterialStagingDTO();
                        if (dtStaging.Columns.Contains("BinGUID"))
                        {
                            tempguid = Guid.Empty;
                            Guid.TryParse(Convert.ToString(dr["BinGUID"]), out tempguid);
                            objMaterialStagingDTO.BinGUID = tempguid;
                        }
                        if (dtStaging.Columns.Contains("BinID"))
                        {
                            templong = 0;
                            long.TryParse(Convert.ToString(dr["BinID"]), out templong);
                            objMaterialStagingDTO.BinID = templong;
                        }
                        if (dtStaging.Columns.Contains("StagingLocationName"))
                        {
                            objMaterialStagingDTO.StagingLocationName = Convert.ToString(dr["StagingLocationName"]);
                        }
                        objMaterialStagingDTO.CompanyID = CompanyID;
                        if (dtStaging.Columns.Contains("Created"))
                        {
                            objMaterialStagingDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtStaging.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objMaterialStagingDTO.CreatedBy = templong;
                        }
                        if (dtStaging.Columns.Contains("CreatedByName"))
                        {
                            objMaterialStagingDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                        }
                        if (dtStaging.Columns.Contains("Description"))
                        {
                            objMaterialStagingDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtStaging.Columns.Contains("GUID"))
                        {
                            tempguid = Guid.Empty;
                            Guid.TryParse(Convert.ToString(dr["GUID"]), out tempguid);
                            objMaterialStagingDTO.GUID = tempguid;
                        }
                        if (dtStaging.Columns.Contains("ID"))
                        {
                            templong = 0;
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objMaterialStagingDTO.ID = templong;
                        }
                        if (dtStaging.Columns.Contains("IsArchived"))
                        {
                            tempbool = false;
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempbool);
                            objMaterialStagingDTO.IsArchived = tempbool;
                        }
                        if (dtStaging.Columns.Contains("IsDeleted"))
                        {
                            tempbool = false;
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempbool);
                            objMaterialStagingDTO.IsDeleted = tempbool;
                        }
                        if (dtStaging.Columns.Contains("LastUpdatedBy"))
                        {
                            templong = 0;
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            objMaterialStagingDTO.LastUpdatedBy = templong;
                        }
                        objMaterialStagingDTO.Room = RoomID;
                        objMaterialStagingDTO.RoomName = string.Empty;
                        if (dtStaging.Columns.Contains("StagingName"))
                        {
                            objMaterialStagingDTO.StagingName = Convert.ToString(dr["StagingName"]);
                        }
                        if (dtStaging.Columns.Contains("UDF1"))
                        {
                            objMaterialStagingDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtStaging.Columns.Contains("UDF2"))
                        {
                            objMaterialStagingDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtStaging.Columns.Contains("UDF3"))
                        {
                            objMaterialStagingDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtStaging.Columns.Contains("UDF4"))
                        {
                            objMaterialStagingDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtStaging.Columns.Contains("UDF5"))
                        {
                            objMaterialStagingDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtStaging.Columns.Contains("Updated"))
                        {
                            objMaterialStagingDTO.Updated = Convert.ToDateTime(dr["Updated"]);
                        }
                        if (dtStaging.Columns.Contains("UpdatedByName"))
                        {
                            objMaterialStagingDTO.UpdatedByName = Convert.ToString(dr["UpdatedByName"]);
                        }
                        if (dtStaging.Columns.Contains("AddedFrom"))
                        {
                            objMaterialStagingDTO.AddedFrom = Convert.ToString(dr["AddedFrom"]);
                        }
                        if (dtStaging.Columns.Contains("EditedFrom"))
                        {
                            objMaterialStagingDTO.EditedFrom = Convert.ToString(dr["EditedFrom"]);
                        }
                        if (dtStaging.Columns.Contains("ReceivedOn"))
                        {
                            objMaterialStagingDTO.ReceivedOn = Convert.ToDateTime(dr["ReceivedOn"]);
                        }
                        if (dtStaging.Columns.Contains("ReceivedOnWeb"))
                        {
                            objMaterialStagingDTO.ReceivedOnWeb = Convert.ToDateTime(dr["ReceivedOnWeb"]);
                        }
                        lstStagings.Add(objMaterialStagingDTO);
                    }
                }
            }
            return lstStagings;
        }
        public List<MaterialStagingDTO> GetMaterialStagingChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<MaterialStagingDTO>("exec [GetMaterialStagingChangeLog] @ID,@dbName", params1).ToList();
            }
        }

        public MaterialStagingDTO GetMaterialStagingByIdPlain(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<MaterialStagingDTO>("exec [GetMaterialStagingByIdPlain] @Id", params1).FirstOrDefault();
            }
        }
        #endregion

    }

}


