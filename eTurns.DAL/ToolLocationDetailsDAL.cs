using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace eTurns.DAL
{
    public class ToolLocationDetailsDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public ToolLocationDetailsDAL(base.DataBaseName)
        //{

        //}

        public ToolLocationDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolLocationDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public IEnumerable<ToolLocationDetailsDTO> GetToolLocationsByToolGUID(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ToolGuid", ToolGuid) };
                IEnumerable<ToolLocationDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolLocationDetailsDTO>("exec [GetToolLocationsByToolGUID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ToolGuid", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                                           select new ToolLocationDetailsDTO
                                                           {
                                                               ID = u.ID,
                                                               ToolGuid = u.ToolGuid,
                                                               LocationGUID = u.LocationGUID,
                                                               IsArchieved = u.IsArchieved,
                                                               Createdon = u.Createdon,
                                                               LastUpdatedOn = u.LastUpdatedOn,
                                                               CreatedBy = u.CreatedBy,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               WhatWhereAction = u.WhatWhereAction,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               RoomID = u.RoomID,
                                                               CompanyID = u.CompanyID,
                                                               ToolName = u.ToolName,
                                                               GUID = u.GUID,
                                                               ToolLocationName = u.ToolLocationName,
                                                               LocationID = u.LocationID,
                                                               Cost = u.Cost,
                                                               IsDefault = u.IsDefault,

                                                           }).AsParallel().ToList();
                return obj;
            }



        }

        public IEnumerable<ToolLocationDetailsDTO> GetToolLocationsWithBlankByToolGUID(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ToolGuid", ToolGuid) };
                IEnumerable<ToolLocationDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolLocationDetailsDTO>("exec [GetToolLocationsWithBlankByToolGUID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ToolGuid", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                                           select new ToolLocationDetailsDTO
                                                           {
                                                               ID = u.ID,
                                                               ToolGuid = u.ToolGuid,
                                                               LocationGUID = u.LocationGUID,
                                                               IsArchieved = u.IsArchieved,
                                                               Createdon = u.Createdon,
                                                               LastUpdatedOn = u.LastUpdatedOn,
                                                               CreatedBy = u.CreatedBy,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               WhatWhereAction = u.WhatWhereAction,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               RoomID = u.RoomID,
                                                               CompanyID = u.CompanyID,
                                                               ToolName = u.ToolName,
                                                               GUID = u.GUID,
                                                               ToolLocationName = u.ToolLocationName,
                                                               LocationID = u.LocationID,
                                                               Cost = u.Cost,
                                                               IsDefault = u.IsDefault,

                                                           }).AsParallel().ToList();
                return obj;
            }



        }

        public List<ToolLocationDetailsDTO> GetToolLocationByToolGUID(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ToolGuid", ToolGuid) };
                List<ToolLocationDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolLocationDetailsDTO>("exec [GetToolLocationsByToolGUID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ToolGuid", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                                    select new ToolLocationDetailsDTO
                                                    {
                                                        ID = u.ID,
                                                        ToolGuid = u.ToolGuid,
                                                        LocationGUID = u.LocationGUID,
                                                        IsArchieved = u.IsArchieved,
                                                        Createdon = u.Createdon,
                                                        LastUpdatedOn = u.LastUpdatedOn,
                                                        CreatedBy = u.CreatedBy,
                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                        ReceivedOn = u.ReceivedOn,
                                                        ReceivedOnWeb = u.ReceivedOnWeb,
                                                        WhatWhereAction = u.WhatWhereAction,
                                                        AddedFrom = u.AddedFrom,
                                                        EditedFrom = u.EditedFrom,
                                                        RoomID = u.RoomID,
                                                        CompanyID = u.CompanyID,
                                                        ToolName = u.ToolName,
                                                        GUID = u.GUID,
                                                        ToolLocationName = u.ToolLocationName,
                                                        LocationID = u.LocationID,
                                                        Cost = u.Cost,
                                                        IsDefault = u.IsDefault

                                                    }).AsParallel().ToList();
                return obj;
            }



        }
        public IEnumerable<ToolLocationDetailsDTO> GetToolAllLocationsByToolGUID(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ToolGuid", ToolGuid) };
                IEnumerable<ToolLocationDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolLocationDetailsDTO>("exec [GetToolAllLocationsByToolGUID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ToolGuid", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                                           select new ToolLocationDetailsDTO
                                                           {
                                                               ID = u.ID,
                                                               ToolGuid = u.ToolGuid,
                                                               LocationGUID = u.LocationGUID,
                                                               IsArchieved = u.IsArchieved,
                                                               Createdon = u.Createdon,
                                                               LastUpdatedOn = u.LastUpdatedOn,
                                                               CreatedBy = u.CreatedBy,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               WhatWhereAction = u.WhatWhereAction,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               RoomID = u.RoomID,
                                                               CompanyID = u.CompanyID,
                                                               ToolName = u.ToolName,
                                                               GUID = u.GUID,
                                                               ToolLocationName = u.ToolLocationName != null ? u.ToolLocationName : "",
                                                               LocationID = u.LocationID,
                                                               Cost = u.Cost,
                                                               IsDefault = u.IsDefault

                                                           }).AsParallel().ToList();
                return obj;
            }



        }
        public ToolLocationDetailsDTO Insert(ToolLocationDetailsDTO objDTO)
        {

            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ToolLocationDetail obj = new ToolLocationDetail();
                    obj.ID = 0;

                    obj.ToolGuid = objDTO.ToolGuid;
                    obj.IsDeleted = objDTO.IsDeleted;
                    obj.LocationGuid = objDTO.LocationGUID;
                    obj.IsArchieved = objDTO.IsArchieved;
                    obj.Createdon = objDTO.Createdon;
                    obj.LastUpdatedOn = objDTO.LastUpdatedOn;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                    obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                    obj.WhatWhereAction = objDTO.WhatWhereAction;
                    obj.AddedFrom = objDTO.AddedFrom;
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.RoomID = objDTO.RoomID;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.LocationID = objDTO.LocationID;
                    obj.Cost = objDTO.Cost;
                    obj.IsDefault = objDTO.IsDefault;
                    obj.LocationID = objDTO.LocationID;
                    if (objDTO.GUID != null && objDTO.GUID != Guid.Empty)
                    {
                        obj.GUID = objDTO.GUID ?? Guid.NewGuid();
                    }
                    else
                    {
                        obj.GUID = Guid.NewGuid();
                    }
                    context.ToolLocationDetails.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.ID;

                    return objDTO;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteByToolLocationGuid(Guid ToolLocationGuid, Int64? userid, string WhatWhereAction, string EditedFrom, Guid ToolGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolLocationDetail obj = context.ToolLocationDetails.FirstOrDefault(t => t.LocationGuid == ToolLocationGuid && t.ToolGuid == ToolGUID);
                obj.IsDeleted = true;
                obj.IsArchieved = false;
                obj.LastUpdatedOn = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = EditedFrom ?? "Web";
                obj.WhatWhereAction = WhatWhereAction ?? "Web-ToolLocation.Delete";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.ToolLocationDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                return true;
            }
        }
        public bool DeleteByToolGUID(Int64 userid, Guid ToolGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolLocationDetail obj = context.ToolLocationDetails.FirstOrDefault(t => t.ToolGuid == ToolGUID);
                obj.IsDeleted = true;
                obj.IsArchieved = false;
                obj.LastUpdatedOn = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = "Web-ToolLocation.Delete";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.ToolLocationDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                return true;
            }
        }
        public ToolLocationDetailsDTO GetToolLocation(Guid ToolGUID, string LocationName, long RoomID, long CompanyID, long UserID, string WhatWhereAction, bool? IsDefault = false)
        {
            //if (!string.IsNullOrWhiteSpace(LocationName))
            {
                if (LocationName == null || LocationName == "null")
                {
                    LocationName = string.Empty;
                }
                ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);

                Guid? UsedToolGUId = ToolGUID;
                //if(UsedToolGUId != null && UsedToolGUId.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                //{
                //    ToolGUID = UsedToolGUId.GetValueOrDefault(Guid.Empty);
                //}
                long binID = 0;
                LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(base.DataBaseName);
                LocationMasterDTO objBinMasterDTO = objLocationMasterDAL.GetLocationByNamePlain(LocationName ?? string.Empty, RoomID, CompanyID);
                ToolLocationDetailsDTO objToolLocationDetailsDTO = new ToolLocationDetailsDTO();
                ToolLocationDetailsDAL objToolLocationDetail = new ToolLocationDetailsDAL(base.DataBaseName);
                if (objBinMasterDTO == null)
                {
                    objBinMasterDTO = new LocationMasterDTO();
                    objBinMasterDTO.ID = 0;

                    objBinMasterDTO.Location = LocationName;
                    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.AddedFrom = "Web";
                    objBinMasterDTO.EditedFrom = "Web";
                    objBinMasterDTO.CreatedBy = UserID;
                    objBinMasterDTO.LastUpdatedBy = UserID;
                    objBinMasterDTO.Room = RoomID;
                    objBinMasterDTO.GUID = Guid.NewGuid();
                    objBinMasterDTO.CompanyID = CompanyID;
                    objBinMasterDTO.IsDeleted = (false);
                    objBinMasterDTO.IsArchived = (false);
                    objBinMasterDTO.UDF1 = string.Empty;
                    objBinMasterDTO.UDF2 = string.Empty;
                    objBinMasterDTO.UDF3 = string.Empty;
                    objBinMasterDTO.UDF4 = string.Empty;
                    objBinMasterDTO.UDF5 = string.Empty;

                    objBinMasterDTO.ID = objLocationMasterDAL.Insert(objBinMasterDTO);
                    objBinMasterDTO = objLocationMasterDAL.GetLocationByNamePlain(LocationName, RoomID, CompanyID);
                    binID = objBinMasterDTO.ID;



                    objToolLocationDetailsDTO.ID = 0;
                    objToolLocationDetailsDTO.ToolGuid = ToolGUID;
                    objToolLocationDetailsDTO.LocationGUID = objBinMasterDTO.GUID;
                    objToolLocationDetailsDTO.Createdon = DateTimeUtility.DateTimeNow;
                    objToolLocationDetailsDTO.LastUpdatedOn = DateTimeUtility.DateTimeNow;

                    objToolLocationDetailsDTO.CreatedBy = UserID;
                    objToolLocationDetailsDTO.LastUpdatedBy = UserID;
                    objToolLocationDetailsDTO.RoomID = RoomID;
                    objToolLocationDetailsDTO.CompanyID = CompanyID;
                    objToolLocationDetailsDTO.IsDeleted = (false);
                    objToolLocationDetailsDTO.AddedFrom = "Web";
                    objToolLocationDetailsDTO.EditedFrom = "Web";
                    objToolLocationDetailsDTO.GUID = Guid.NewGuid();
                    objToolLocationDetailsDTO.IsArchieved = (false);
                    objToolLocationDetailsDTO.IsDefault = IsDefault;
                    objToolLocationDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolLocationDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objToolLocationDetailsDTO.WhatWhereAction = WhatWhereAction;
                    objToolLocationDetailsDTO.LocationID = objBinMasterDTO.ID;
                    objToolLocationDetailsDTO = objToolLocationDetail.Insert(objToolLocationDetailsDTO);

                }
                else
                {
                    binID = objBinMasterDTO.ID;

                    List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetail.GetToolAllLocationsByToolGUID(RoomID, CompanyID, false, false, ToolGUID).ToList();
                    objToolLocationDetailsDTO = null;
                    if (lstToolLocationDetailsDTO != null && lstToolLocationDetailsDTO.Count() > 0 && (!string.IsNullOrWhiteSpace(LocationName)))
                    {
                        objToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(c => c.ToolLocationName != null && c.ToolLocationName.ToLower().Trim() == LocationName.ToLower().Trim()).FirstOrDefault();
                    }
                    else
                    {
                        objToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(c => c.ToolLocationName != null && c.ToolLocationName == LocationName).FirstOrDefault();
                    }
                    if (objToolLocationDetailsDTO == null)
                    {
                        objToolLocationDetailsDTO = new ToolLocationDetailsDTO();

                        objToolLocationDetailsDTO.ID = 0;
                        objToolLocationDetailsDTO.ToolGuid = ToolGUID;
                        objToolLocationDetailsDTO.LocationGUID = objBinMasterDTO.GUID;
                        objToolLocationDetailsDTO.Createdon = DateTimeUtility.DateTimeNow;
                        objToolLocationDetailsDTO.LastUpdatedOn = DateTimeUtility.DateTimeNow;
                        objToolLocationDetailsDTO.CreatedBy = UserID;
                        objToolLocationDetailsDTO.LastUpdatedBy = UserID;
                        objToolLocationDetailsDTO.RoomID = RoomID;
                        objToolLocationDetailsDTO.CompanyID = CompanyID;
                        objToolLocationDetailsDTO.IsDeleted = (false);
                        objToolLocationDetailsDTO.AddedFrom = "Web";
                        objToolLocationDetailsDTO.EditedFrom = "Web";
                        objToolLocationDetailsDTO.GUID = Guid.NewGuid();
                        objToolLocationDetailsDTO.IsArchieved = (false);
                        objToolLocationDetailsDTO.IsDefault = IsDefault;
                        objToolLocationDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolLocationDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objToolLocationDetailsDTO.WhatWhereAction = WhatWhereAction;
                        objToolLocationDetailsDTO.LocationID = objBinMasterDTO.ID;
                        objToolLocationDetailsDTO = objToolLocationDetail.Insert(objToolLocationDetailsDTO);

                    }
                }
                if ((IsDefault ?? false) == true)
                {
                    UpdateToolWithDefault(ToolGUID, binID, objBinMasterDTO.GUID);
                }
                return objToolLocationDetailsDTO;
            }
            //else
            //{
            //    return null;
            //}
        }
        public void UpdateToolWithDefault(Guid ToolGUID, Int64 binID, Guid LocationGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMaster objTool = context.ToolMasters.Where(t => t.GUID == ToolGUID).FirstOrDefault();
                if (objTool != null)
                {
                    objTool.LocationID = binID;
                    ToolLocationDetail objToolLocationDetail = context.ToolLocationDetails.Where(t => t.IsDeleted == false && t.IsDefault == true && t.ToolGuid == ToolGUID && t.LocationGuid != LocationGUID).FirstOrDefault();
                    if (objToolLocationDetail != null)
                    {
                        objToolLocationDetail.IsDefault = false;
                    }
                    context.SaveChanges();
                }
            }
        }
        public ToolLocationDetailsDTO GetToolDefaultLocation(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGuid, Int64 UserID, string AddedFrom, string WhatWhereAction)
        {
            ToolLocationDetailsDTO objToolAssetQuantityDetailDTO;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived), new SqlParameter("@ToolGuid", ToolGuid), new SqlParameter("@UserID", UserID), new SqlParameter("@AddedFrom", AddedFrom), new SqlParameter("@WhatWhereAction", WhatWhereAction) };
                objToolAssetQuantityDetailDTO = context.Database.SqlQuery<ToolLocationDetailsDTO>("exec GetToolDefaultLocation @RoomId,@CompanyId,@IsDeleted,@IsArchieved,@ToolGuid,@UserID,@AddedFrom,@WhatWhereAction", params1).FirstOrDefault();
            }
            return objToolAssetQuantityDetailDTO;
        }
        public IEnumerable<ToolLocationDetailsDTO> GetAllRecordsBinWise(Int64 LocationID, Int64 RoomID, Int64 CompanyId, Guid ToolGUID, Guid? ToolAssetOrderDetailGUID, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ToolLocationDetailsDTO> result = null;
            if (ToolAssetOrderDetailGUID == null || ToolAssetOrderDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                result = GetCachedDataBinWise(LocationID, ToolGUID, RoomID, CompanyId).Where(t => ((t.IsArchieved ?? false) == false && (t.IsDeleted ?? false) == false && (t.Quantity.GetValueOrDefault(0) != 0))).OrderBy(ColumnName);
            else
                result = GetCachedDataBinWise(LocationID, ToolGUID, RoomID, CompanyId).Where(t => ((t.IsArchieved ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);

            return result;
        }
        public IEnumerable<ToolLocationDetailsDTO> GetCachedDataBinWise(Int64 LocationID, Guid ToolGUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(ToolGUID, RoomID, CompanyID).Where(t => t.LocationID == LocationID);
        }
        public IEnumerable<ToolLocationDetailsDTO> GetCachedData(Guid ToolGUID, Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ToolGUID", ToolGUID) };
                IEnumerable<ToolLocationDetailsDTO> obj = (from u in context.Database.SqlQuery<ToolLocationDetailsDTO>("exec [GetToolLocationWithQty] @CompanyID,@RoomID,@ToolGUID", paramA)
                                                           select new ToolLocationDetailsDTO
                                                           {
                                                               ID = u.ID,
                                                               ToolGuid = u.ToolGuid,
                                                               LocationGUID = u.LocationGUID,
                                                               IsArchieved = u.IsArchieved,
                                                               Createdon = u.Createdon,
                                                               LastUpdatedOn = u.LastUpdatedOn,
                                                               CreatedBy = u.CreatedBy,
                                                               LastUpdatedBy = u.LastUpdatedBy,
                                                               ReceivedOn = u.ReceivedOn,
                                                               ReceivedOnWeb = u.ReceivedOnWeb,
                                                               WhatWhereAction = u.WhatWhereAction,
                                                               AddedFrom = u.AddedFrom,
                                                               EditedFrom = u.EditedFrom,
                                                               RoomID = u.RoomID,
                                                               CompanyID = u.CompanyID,
                                                               ToolName = u.ToolName,
                                                               GUID = u.GUID,
                                                               ToolLocationName = u.ToolLocationName,
                                                               LocationID = u.LocationID,
                                                               Cost = u.Cost,

                                                               InitialQuantityWeb = u.InitialQuantityWeb,
                                                               InitialQuantityPDA = u.InitialQuantityPDA,
                                                               CreatedByName = u.CreatedByName,
                                                               UpdatedByName = u.UpdatedByName,
                                                               RoomName = u.RoomName,
                                                               Quantity = u.Quantity,
                                                               SerialNumberTracking = u.SerialNumberTracking,
                                                               IsDefault = u.IsDefault
                                                           }).AsParallel().ToList();
                return obj;


            }
        }
        public IEnumerable<ToolAssetQuantityDetailDTO> GetPagedRecords_NoCache(Int64 LocationID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGUID, Guid? ToolAssetOrderDetailGUID, string parentLOCSearch = "")
        {
            //Get Cached-Media
            IEnumerable<ToolAssetQuantityDetailDTO> ObjCache = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@LocationID", LocationID), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@SearchTerm", parentLOCSearch) };
                ObjCache = (from u in context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetToolLocationSerial] @LocationID,@ToolGUID,@RoomID,@CompanyID,@SearchTerm", paramA)
                            select new ToolAssetQuantityDetailDTO
                            {
                                ID = u.ID,
                                GUID = u.GUID,
                                ToolGUID = u.ToolGUID,
                                ToolBinID = u.ToolBinID,
                                IsArchived = u.IsArchived,
                                IsDeleted = u.IsDeleted,
                                Created = u.Created,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                UpdatedBy = u.UpdatedBy,
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                WhatWhereAction = u.WhatWhereAction,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                RoomID = u.RoomID,
                                CompanyID = u.CompanyID,
                                ToolName = u.ToolName,




                                Cost = u.Cost,

                                InitialQuantityWeb = u.InitialQuantityWeb,
                                InitialQuantityPDA = u.InitialQuantityPDA,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                RoomName = u.RoomName,
                                Quantity = u.Quantity,

                                SerialNumber = u.SerialNumber,
                                Description = u.Description,
                                AvailableQuantity = u.AvailableQuantity
                            }).AsParallel().ToList();
            }




            IEnumerable<ToolAssetQuantityDetailDTO> ObjGlobalCache = null;

            if (ToolAssetOrderDetailGUID == null)
            {
                ObjGlobalCache = ObjCache;
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.ToolGUID == ToolGUID));
            }
            else
            {
                ObjGlobalCache = ObjCache;
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));
            }

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                     );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public ToolLocationDetailsDTO GetToolLocationDetailsByID(long ID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ID", ID) };
                ToolLocationDetailsDTO obj = context.Database.SqlQuery<ToolLocationDetailsDTO>("exec [GetToolLocationDetailsByID] @ID,@RoomID,@CompanyID", params1).ToList().FirstOrDefault();
                return obj;
            }

        }

    }

}
