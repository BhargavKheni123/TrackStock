using Dynamite.Data.Extensions;
using Dynamite.Extensions;
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
    public partial class ToolMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ToolMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]


        public Int64 Insert(ToolMasterDTO objDTO, bool? IsAllowNewToolOrder = false)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMaster obj = new ToolMaster();
                obj.ID = 0;
                obj.ToolName = objDTO.ToolName;
                obj.Serial = objDTO.Serial;
                obj.Description = objDTO.Description;
                obj.Cost = objDTO.Cost;
                obj.Quantity = objDTO.Quantity;
                obj.IscheckedOut = objDTO.IsCheckedOut;
                obj.IsGroupOfItems = objDTO.IsGroupOfItems;
                obj.ToolCategoryID = objDTO.ToolCategoryID;
                obj.LocationID = objDTO.LocationID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.GUID = string.IsNullOrWhiteSpace(objDTO.GUID.ToString()) ? Guid.NewGuid() : objDTO.GUID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.NoOfPastMntsToConsider = objDTO.NoOfPastMntsToConsider;
                obj.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;
                obj.MaintenanceType = objDTO.MaintenanceType;
                obj.TechnicianGUID = objDTO.TechnicianGuID ?? Guid.Empty;
                obj.ImageType = objDTO.ImageType;
                obj.ImagePath = objDTO.ImagePath;
                obj.ToolImageExternalURL = objDTO.ToolImageExternalURL;
                obj.Type = objDTO.Type;
                obj.IsBuildBreak = objDTO.IsBuildBreak;
                obj.AvailableToolQty = objDTO.AvailableToolQty;
                if (string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking))
                {
                    objDTO.ToolTypeTracking = "1";
                }
                obj.ToolTypeTracking = objDTO.ToolTypeTracking;
                obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                obj.LotNumberTracking = objDTO.LotNumberTracking;
                obj.DateCodeTracking = objDTO.DateCodeTracking;
                obj.WhatWhereAction = string.IsNullOrEmpty(objDTO.WhatWhereAction) ? string.Empty : objDTO.WhatWhereAction;
                context.ToolMasters.Add(obj);
                context.SaveChanges();


                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;


                if ((obj.LocationID ?? 0) > 0)
                {
                    if ((IsAllowNewToolOrder ?? true) == true)
                    {
                        Guid? UsedToolGUId = objDTO.GUID;
                        if (UsedToolGUId == Guid.Empty)
                        {
                            UsedToolGUId = objDTO.GUID;
                        }

                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(base.DataBaseName);
                        LocationMasterDTO objLocationMasterDTO = objLocationCntrl.GetLocationByIDPlain(obj.LocationID ?? 0, obj.Room ?? 0, obj.CompanyID ?? 0);
                        ToolLocationDetailsDTO objToolLocationDetailsDTO = new ToolLocationDetailsDTO();
                        if (objLocationMasterDTO != null)
                        {
                            ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                            objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(UsedToolGUId ?? Guid.Empty, objLocationMasterDTO.Location, obj.Room ?? 0, obj.CompanyID ?? 0, obj.CreatedBy ?? 0, "ToolMasterDAL>>ToolInsert", true);
                        }
                    }
                }
                return obj.ID;
            }
        }


        public bool Edit(ToolMasterDTO objDTO, bool? IsAllowNewToolOrder = false)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMaster obj = context.ToolMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.ID = objDTO.ID;
                obj.ToolName = objDTO.ToolName;
                if ((obj.SerialNumberTracking) == false)
                {
                    obj.Serial = objDTO.Serial;
                    obj.Quantity = objDTO.Quantity;
                }
                obj.Description = objDTO.Description;
                obj.Cost = objDTO.Cost;

                obj.CheckedOutMQTY = objDTO.CheckedOutMQTY;
                obj.CheckedOutQTY = objDTO.CheckedOutQTY;
                obj.IscheckedOut = objDTO.IsCheckedOut;
                obj.IsGroupOfItems = objDTO.IsGroupOfItems;
                obj.ToolCategoryID = objDTO.ToolCategoryID;
                obj.LocationID = objDTO.LocationID;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.NoOfPastMntsToConsider = objDTO.NoOfPastMntsToConsider;
                obj.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;
                obj.MaintenanceType = objDTO.MaintenanceType;
                obj.TechnicianGUID = objDTO.TechnicianGuID ?? Guid.Empty;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = "Web";
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                obj.ImageType = objDTO.ImageType;
                if (!string.IsNullOrEmpty(objDTO.ImagePath))
                {
                    obj.ImagePath = objDTO.ImagePath;
                }

                obj.ToolImageExternalURL = objDTO.ToolImageExternalURL;
                if (objDTO.Type == null)
                {
                    objDTO.Type = 1;
                }
                obj.Type = objDTO.Type;

                obj.IsBuildBreak = objDTO.IsBuildBreak;
                obj.AvailableToolQty = objDTO.AvailableToolQty;
                if (string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking))
                {
                    objDTO.ToolTypeTracking = "1";
                }
                obj.ToolTypeTracking = objDTO.ToolTypeTracking;
                obj.WhatWhereAction = string.IsNullOrEmpty(objDTO.WhatWhereAction) ? string.Empty : objDTO.WhatWhereAction;

                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if ((obj.LocationID ?? 0) > 0)
                {
                    if ((IsAllowNewToolOrder ?? true) == true)
                    {

                        Guid? UsedToolGUId = Guid.Empty;
                        if (UsedToolGUId == Guid.Empty)
                        {
                            UsedToolGUId = objDTO.GUID;
                        }

                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(base.DataBaseName);
                        LocationMasterDTO objLocationMasterDTO = objLocationCntrl.GetLocationByIDPlain(obj.LocationID ?? 0, obj.Room ?? 0, obj.CompanyID ?? 0);
                        ToolLocationDetailsDTO objToolLocationDetailsDTO = new ToolLocationDetailsDTO();
                        if (objLocationMasterDTO != null)
                        {
                            ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                            objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(UsedToolGUId ?? objDTO.GUID, objLocationMasterDTO.Location, obj.Room ?? 0, obj.CompanyID ?? 0, obj.CreatedBy ?? 0, "ToolMasterDAL>>ToolInsert", true);
                        }
                    }
                }

                return true;
            }
        }
        public bool EditFromOrder(ToolMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ToolMaster obj = context.ToolMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.ID = objDTO.ID;
                obj.ToolName = objDTO.ToolName;

                obj.Serial = objDTO.Serial;
                obj.Quantity = objDTO.Quantity;

                obj.Description = objDTO.Description;
                obj.Cost = objDTO.Cost;

                obj.CheckedOutMQTY = objDTO.CheckedOutMQTY;
                obj.CheckedOutQTY = objDTO.CheckedOutQTY;
                obj.IscheckedOut = objDTO.IsCheckedOut;
                obj.IsGroupOfItems = objDTO.IsGroupOfItems;
                obj.ToolCategoryID = objDTO.ToolCategoryID;
                obj.LocationID = objDTO.LocationID;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.NoOfPastMntsToConsider = objDTO.NoOfPastMntsToConsider;
                obj.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;
                obj.MaintenanceType = objDTO.MaintenanceType;
                obj.TechnicianGUID = objDTO.TechnicianGuID ?? Guid.Empty;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = "Web";
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                obj.ImageType = objDTO.ImageType;
                if (!string.IsNullOrEmpty(objDTO.ImagePath))
                {
                    obj.ImagePath = objDTO.ImagePath;
                }

                obj.ToolImageExternalURL = objDTO.ToolImageExternalURL;
                if (objDTO.Type == null)
                {
                    objDTO.Type = 1;
                }
                obj.Type = objDTO.Type;

                obj.IsBuildBreak = objDTO.IsBuildBreak;
                obj.AvailableToolQty = objDTO.AvailableToolQty;
                if (string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking))
                {
                    objDTO.ToolTypeTracking = "1";
                }
                obj.ToolTypeTracking = objDTO.ToolTypeTracking;
                obj.WhatWhereAction = string.IsNullOrEmpty(objDTO.WhatWhereAction) ? string.Empty : objDTO.WhatWhereAction;

                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if ((obj.LocationID ?? 0) > 0)
                {

                    Guid? UsedToolGUId = Guid.Empty;
                    if (UsedToolGUId == Guid.Empty)
                    {
                        UsedToolGUId = objDTO.GUID;
                    }

                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(base.DataBaseName);
                    LocationMasterDTO objLocationMasterDTO = objLocationCntrl.GetLocationByIDPlain(obj.LocationID ?? 0, obj.Room ?? 0, obj.CompanyID ?? 0);
                    ToolLocationDetailsDTO objToolLocationDetailsDTO = new ToolLocationDetailsDTO();
                    if (objLocationMasterDTO != null)
                    {
                        ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(UsedToolGUId ?? objDTO.GUID, objLocationMasterDTO.Location, obj.Room ?? 0, obj.CompanyID ?? 0, obj.CreatedBy ?? 0, "ToolMasterDAL>>ToolInsert", true);
                    }
                }

                return true;
            }
        }

        public bool updateImagePath(long Id, string fileName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMaster objToolMaster = context.ToolMasters.FirstOrDefault(t => t.ID == Id);
                if (objToolMaster != null)
                {
                    objToolMaster.ImagePath = fileName;
                    objToolMaster.WhatWhereAction = "Update ImagePath";
                    context.SaveChanges();
                }
            }
            return true;
        }

        public ToolMasterDTO GetHistoryRecord(Int64 HistoryID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<ToolMasterDTO>(
                            @"SELECT A.ID,A.ToolName, A.Serial, A.Description, A.Cost, A.IsDeleted
                                , A.IsArchived,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.IsCheckedOut,A.IsGroupOfItems
                                , A.Created, A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Quantity,A.GUID                                
                                , A.ToolCategoryID,A.LocationID,A.Room,A.CompanyId,B.UserName AS 'CreatedByName'
                                ,C.UserName AS 'UpdatedByName',D.RoomName, E.ToolCategory ,L.Location  ,A.AddedFrom,A.EditedFrom,A.Receivedon,A.ReceivedOnWeb,
                                A.ImageType,A.ImagePath,A.ToolImageExternalURL,isnull(A.[Type],1) as [Type]
                                ,isnull(A.[IsBuildBreak],0) as IsBuildBreak
                                ,isnull(A.AvailableToolQty,0) as AvailableToolQty
                                 ,isnull(A.ToolTypeTracking,'') as ToolTypeTracking
                                    ,isnull(A.SerialNumberTracking,0) as SerialNumberTracking
                                ,isnull(A.LotNumberTracking,0) as LotNumberTracking
                                ,isnull(A.DateCodeTracking,0) as DateCodeTracking
                                FROM ToolMaster_History A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID  
                                LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID                                 
                                WHERE A.HistoryID = " + HistoryID.ToString())
                        select new ToolMasterDTO
                        {
                            ID = u.ID,
                            ToolName = u.ToolName,
                            Serial = u.Serial,
                            Description = u.Description,
                            Cost = u.Cost,
                            Quantity = u.Quantity,
                            IsCheckedOut = u.IsCheckedOut,
                            IsGroupOfItems = u.IsGroupOfItems,
                            ToolCategoryID = u.ToolCategoryID,
                            ToolCategory = u.ToolCategory,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            RoomName = u.RoomName,
                            Location = u.Location,
                            LocationID = u.LocationID,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ToolUDF1 = u.UDF1,
                            ToolUDF2 = u.UDF2,
                            ToolUDF3 = u.UDF3,
                            ToolUDF4 = u.UDF4,
                            ToolUDF5 = u.UDF5,
                            AddedFrom = u.AddedFrom,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ImageType = u.ImageType,
                            ImagePath = u.ImagePath,
                            ToolImageExternalURL = u.ToolImageExternalURL,
                            Type = u.Type,
                            IsBuildBreak = u.IsBuildBreak,
                            AvailableToolQty = u.AvailableToolQty,
                            ToolTypeTracking = u.ToolTypeTracking,
                            SerialNumberTracking = u.SerialNumberTracking,
                            LotNumberTracking = u.LotNumberTracking,
                            DateCodeTracking = u.DateCodeTracking
                        }).SingleOrDefault();
            }
        }

        public ToolMasterDTO GetToolByGUIDNormal(Guid ToolGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", ToolGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByGUIDNormal] @GUID", params1).FirstOrDefault();
            }
        }

        public ToolMasterDTO GetToolByGUIDPlain(Guid ToolGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", ToolGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByGUIDPlain] @GUID", params1).FirstOrDefault();
            }
        }
        public ToolMasterDTO GetToolByIDNormal(long ToolID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ToolID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByIDNormal] @ID", params1).FirstOrDefault();
            }
        }
        public ToolMasterDTO GetToolByIDPlain(long ToolID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ToolID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByIDPlain] @ID", params1).FirstOrDefault();
            }
        }

        public ToolMasterDTO GetToolByIDFull(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByIDFull] @ID", params1).FirstOrDefault();
            }
        }

        public List<ToolMasterDTO> GetToolByGUIDsNormal(string ToolGUIDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUIDs", ToolGUIDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByGUIDsNormal] @ToolGUIDs", params1).ToList();
            }
        }

        public List<ToolMasterDTO> GetToolByIDsNormal(string ToolIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolIDs", ToolIDs), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByIDsNormal] @ToolIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ToolMasterDTO> GetToolByIDsFull(string ToolIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolIDs", ToolIDs), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByIDsFull] @ToolIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public ToolMasterDTO GetToolByGUIDFull(Guid ToolGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", ToolGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByGUIDFull] @GUID", params1).FirstOrDefault();
            }
        }

        public List<ToolMasterDTO> GetToolByRoomNormal(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByRoomNormal] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<ToolMasterDTO> GetToolByRoomPlain(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByRoomPlain] @RoomID,@CompanyID", params1).ToList();
            }
        }


        public ToolMasterDTO GetToolBySerialNormal(string ToolSerial, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolSerial", ToolSerial), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolBySerialNormal] @ToolSerial,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public ToolMasterDTO GetToolBySerialPlain(string ToolSerial, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolSerial", ToolSerial), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolBySerialPlain] @ToolSerial,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<ToolMasterDTO> GetToolBySerialsAndNamesPlain(string ToolName, string SerialName, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolName", ToolName ?? string.Empty), new SqlParameter("@SerialName", SerialName ?? string.Empty), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ToolMasterDTO>("exec GetToolByToolAndSerialNameList @ToolName,@SerialName,@RoomId,@CompanyID", params1).ToList<ToolMasterDTO>();
            }
        }

        public ToolMasterDTO GetToolBySerialAndNamePlain(string ToolName, string SerialName, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolName", ToolName ?? string.Empty), new SqlParameter("@SerialName", SerialName ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ToolMasterDTO>("exec GetToolBySerialAndNamePlain @ToolName,@SerialName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
            //  return GetToolBySerialsAndNamesPlain(ToolName, SerialName, RoomID, CompanyID).FirstOrDefault();
        }

        public List<ToolMasterDTO> GetToolBySerialSearchAndName(string Serial, string ToolName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Serial", Serial ?? (object)DBNull.Value), new SqlParameter("@ToolName", ToolName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolBySerialSearchAndName] @Serial,@ToolName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ToolMasterDTO> GetToolByNameSearch(string SearchValue, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SearchValue", SearchValue ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByNameSearch] @SearchValue,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ToolMasterDTO> GetToolSimpleDataWithFilter(string ToolName, string Serial, Int64? ToolID, Guid? ToolGUID, Int64 RoomID, Int64 CompanyID)
        {
            List<ToolMasterDTO> lstTools = new List<ToolMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolName", ToolName ?? (object)DBNull.Value),
                                                   new SqlParameter("@Serial", Serial ?? (object)DBNull.Value),
                                                    new SqlParameter("@ToolID", ToolID ?? (object)DBNull.Value),
                                                    new SqlParameter("@ToolGUID", ToolGUID ?? (object)DBNull.Value),
                                                    new SqlParameter("@RoomID", RoomID ),
                                                    new SqlParameter("@CompanyID", CompanyID) };
                lstTools = context.Database.SqlQuery<ToolMasterDTO>("exec GetToolSimpleDataWithFilter @ToolName,@Serial,@ToolID,@ToolGUID,@RoomID,@CompanyID", params1).ToList();
            }
            return lstTools;
        }

        public string ToolSerialDuplicateCheck(string SerailNumber, long ToolID, long RoomID, long CompanyID)
        {
            string ReturnResult = "ok";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<ToolMaster> qry = (from tm in context.ToolMasters
                                              where tm.Room == RoomID && tm.CompanyID == CompanyID && (tm.IsDeleted ?? false) == false && (tm.IsArchived ?? false) == false && (tm.Serial ?? string.Empty) == (SerailNumber ?? string.Empty) && tm.ID != ToolID
                                              select tm);

                if (qry.Any())
                {
                    ReturnResult = "duplicate";
                }
            }
            return ReturnResult;

        }

        public int GetToolMaintainanceDueCountForDashboard(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@RoomId", RoomId)
                                                };
                return context.Database.SqlQuery<int>("exec [GetToolMaintainanceDueCountForDashboard] @CompanyId,@RoomId", params1).FirstOrDefault();
            }
        }

        public List<ToolMasterDTO> GetPagedToolMaintainanceDue(long RoomID, long CompanyID, int NoOfRecords = int.MaxValue)
        {
            List<ToolMasterDTO> lstTools = new List<ToolMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@NoOfRecords", NoOfRecords ),
                                                    new SqlParameter("@RoomID", RoomID ),
                                                    new SqlParameter("@CompanyID", CompanyID) };
                lstTools = context.Database.SqlQuery<ToolMasterDTO>("exec GetPagedToolMaintainanceDue @NoOfRecords,@RoomID,@CompanyID", params1).ToList();
            }
            return lstTools;

        }

        public List<ToolMasterDTO> ToolAdjustmentCountExport(long RoomID, long CompanyID)
        {
            List<ToolMasterDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                List<ToolMasterDTO> lstresult = context.Database.SqlQuery<ToolMasterDTO>("exec ToolAdjustmentCountExport @RoomID,@CompanyID", params1).ToList();
                obj = (from im in lstresult
                       select new ToolMasterDTO
                       {
                           ToolName = im.ToolName,
                           Location = im.Location,
                           Quantity = im.Quantity,
                           Serial = im.Serial
                       }).ToList();
            }
            return obj;
        }

        public List<ToolMasterDTO> GetPagedTools(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ToolsIDs, string ToolGUIDs, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, string ExcludeToolGuids = "", string Type = "1")
        {
            List<ToolMasterDTO> lstTools = new List<ToolMasterDTO>();
            TotalCount = 0;
            ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
            DataSet dsTools = new DataSet();




            string ToolLocs = null;
            string ToolCats = null;
            string ToolCost = null;
            string ToolCreaters = null;
            string ToolUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ToolCheckOutUDF1 = null;
            string ToolCheckOutUDF2 = null;
            string ToolCheckOutUDF3 = null;
            string ToolCheckOutUDF4 = null;
            string ToolCheckOutUDF5 = null;
            string ToolTechUDF1 = null;
            string ToolTechUDF2 = null;
            string ToolTechUDF3 = null;
            string ToolTechUDF4 = null;
            string ToolTechUDF5 = null;
            string ToolMaintence = null;
            string TechnicianList = null;
            using (SqlConnection EturnsConnection = new SqlConnection(base.DataBaseConnectionString))
            {
                if (String.IsNullOrEmpty(SearchTerm))
                {
                    dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
                }
                else if (SearchTerm.Contains("[###]"))
                {
                    string[] stringSeparators = new string[] { "[###]" };
                    string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                    string[] FieldsPara = Fields[1].Split('@');
                    if (Fields.Length > 2)
                    {
                        if (!string.IsNullOrEmpty(Fields[2]))
                        {
                            SearchTerm = Fields[2];
                        }
                        else
                        {
                            SearchTerm = string.Empty;
                        }
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {
                        ToolCreaters = FieldsPara[0].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                    {
                        ToolUpdators = FieldsPara[1].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                    {
                        CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                    {
                        UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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
                    if (!string.IsNullOrWhiteSpace(FieldsPara[71]))
                    {
                        string[] arrReplenishTypes = FieldsPara[71].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolCheckOutUDF1 = ToolCheckOutUDF1 + supitem + "','";
                        }
                        ToolCheckOutUDF1 = ToolCheckOutUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolCheckOutUDF1 = HttpUtility.UrlDecode(ToolCheckOutUDF1);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[72]))
                    {
                        string[] arrReplenishTypes = FieldsPara[72].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolCheckOutUDF2 = ToolCheckOutUDF2 + supitem + "','";
                        }
                        ToolCheckOutUDF2 = ToolCheckOutUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolCheckOutUDF2 = HttpUtility.UrlDecode(ToolCheckOutUDF2);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[73]))
                    {
                        string[] arrReplenishTypes = FieldsPara[73].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolCheckOutUDF3 = ToolCheckOutUDF3 + supitem + "','";
                        }
                        ToolCheckOutUDF3 = ToolCheckOutUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolCheckOutUDF3 = HttpUtility.UrlDecode(ToolCheckOutUDF3);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[74]))
                    {
                        string[] arrReplenishTypes = FieldsPara[74].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolCheckOutUDF4 = ToolCheckOutUDF4 + supitem + "','";
                        }
                        ToolCheckOutUDF4 = ToolCheckOutUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolCheckOutUDF4 = HttpUtility.UrlDecode(ToolCheckOutUDF4);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[75]))
                    {
                        string[] arrReplenishTypes = FieldsPara[75].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolCheckOutUDF5 = ToolCheckOutUDF5 + supitem + "','";
                        }
                        ToolCheckOutUDF5 = ToolCheckOutUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolCheckOutUDF5 = HttpUtility.UrlDecode(ToolCheckOutUDF5);
                    }


                    if (!string.IsNullOrWhiteSpace(FieldsPara[114]))
                    {
                        string[] arrReplenishTypes = FieldsPara[114].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolTechUDF1 = ToolTechUDF1 + supitem + "','";
                        }
                        ToolTechUDF1 = ToolTechUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolTechUDF1 = HttpUtility.UrlDecode(ToolTechUDF1);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[115]))
                    {
                        string[] arrReplenishTypes = FieldsPara[115].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolTechUDF2 = ToolTechUDF2 + supitem + "','";
                        }
                        ToolTechUDF2 = ToolTechUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolTechUDF2 = HttpUtility.UrlDecode(ToolTechUDF2);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[116]))
                    {
                        string[] arrReplenishTypes = FieldsPara[116].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolTechUDF3 = ToolTechUDF3 + supitem + "','";
                        }
                        ToolTechUDF3 = ToolTechUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolTechUDF3 = HttpUtility.UrlDecode(ToolTechUDF3);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[117]))
                    {
                        string[] arrReplenishTypes = FieldsPara[117].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolTechUDF4 = ToolTechUDF4 + supitem + "','";
                        }
                        ToolTechUDF4 = ToolTechUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolTechUDF4 = HttpUtility.UrlDecode(ToolTechUDF4);
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[118]))
                    {
                        string[] arrReplenishTypes = FieldsPara[118].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolTechUDF5 = ToolTechUDF5 + supitem + "','";
                        }
                        ToolTechUDF5 = ToolTechUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        ToolTechUDF5 = HttpUtility.UrlDecode(ToolTechUDF5);
                    }




                    if (!string.IsNullOrWhiteSpace(FieldsPara[27]))
                    {
                        ToolLocs = FieldsPara[27].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[29]))
                    {
                        ToolCats = FieldsPara[29].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                    {
                        ToolCost = FieldsPara[31].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[101]))
                    {
                        string[] arrReplenishTypes = FieldsPara[101].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            ToolMaintence = ToolMaintence + supitem + ",";
                        }
                        ToolMaintence = ToolMaintence.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[77]))
                    {
                        string[] arrReplenishTypes = FieldsPara[77].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            TechnicianList = TechnicianList + supitem + "','";
                        }
                        TechnicianList = TechnicianList.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    }

                    dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
                }
                else
                {
                    dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
                }
            }
            if (dsTools != null && dsTools.Tables.Count > 0)
            {
                DataTable dtTools = dsTools.Tables[0];
                if (dtTools.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtTools.Rows[0]["TotalRecords"]);
                    lstTools = dtTools.AsEnumerable().Select(row => new ToolMasterDTO()
                    {
                        AddedFrom = row.Field<string>("AddedFrom"),
                        CheckedOutMQTY = row.Field<double?>("CheckedOutMQTY"),
                        CheckedOutQTY = row.Field<double?>("CheckedOutQTY"),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Cost = row.Field<double?>("Cost"),
                        Created = row.Field<DateTime>("Created"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        Description = row.Field<string>("Description"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        GUID = row.Field<Guid>("GUID"),
                        ID = row.Field<long>("ID"),
                        IsArchived = row.Field<bool?>("IsArchived"),
                        IsAutoMaintain = row.Field<bool>("IsAutoMaintain"),
                        IsCheckedOut = row.Field<bool?>("IscheckedOut"),
                        IsDeleted = row.Field<bool?>("IsDeleted"),
                        IsGroupOfItems = row.Field<int?>("IsGroupOfItems"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        LocationID = row.Field<long?>("LocationID"),
                        MaintenanceType = row.Field<int>("MaintenanceType"),
                        NoOfPastMntsToConsider = row.Field<int?>("NoOfPastMntsToConsider"),
                        MaintenanceDueNoticeDays = row.Field<int?>("MaintenanceDueNoticeDays"),
                        Quantity = row.Field<double>("Quantity"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        Room = row.Field<long?>("Room"),
                        Serial = row.Field<string>("Serial"),
                        ToolCategoryID = row.Field<long?>("ToolCategoryID"),
                        ToolName = row.Field<string>("ToolName"),
                        ToolUDF1 = row.Field<string>("UDF1"),
                        ToolUDF2 = row.Field<string>("UDF2"),
                        ToolUDF3 = row.Field<string>("UDF3"),
                        ToolUDF4 = row.Field<string>("UDF4"),
                        ToolUDF5 = row.Field<string>("UDF5"),
                        Updated = row.Field<DateTime?>("Updated"),
                        Location = row.Field<string>("Location"),
                        ToolCategory = row.Field<string>("ToolCategory"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        Technician = row.Field<string>("Technician"),
                        ImageType = row.Field<string>("ImageType"),
                        CheckedOutQTYTotal = row.Field<int?>("CheckedOutQTYTotal"),
                        ImagePath = row.Field<string>("ImageType") == "ExternalImage" ? string.Empty : row.Field<string>("ImagePath"),
                        ToolImageExternalURL = row.Field<string>("ImageType") == "ExternalImage" ? row.Field<string>("ToolImageExternalURL") : string.Empty,
                        Type = row.Field<Int64?>("Type"),
                        IsBuildBreak = row.Field<bool>("IsBuildBreak"),
                        AvailableToolQty = row.Field<double?>("AvailableToolQty"),
                        ToolTypeTracking = row.Field<string>("ToolTypeTracking"),
                        DefaultLocation = row.Field<Int64?>("DefaultLocation"),
                        BinID = row.Field<Int64?>("BinID"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        IsBeforeCheckOutAndCheckIn = row.Field<bool>("IsBeforeCheckOutAndCheckIn")
                    }).ToList();

                }
            }
            return lstTools;
        }


        public List<ToolMasterDTO> GetPagedToolHistory(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ToolsIDs, string ToolGUIDs, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, string ExcludeToolGuids = "")
        {
            List<ToolMasterDTO> lstTools = new List<ToolMasterDTO>();
            TotalCount = 0;
            ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
            DataSet dsTools = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                Connectionstring = base.DataBaseConnectionString;
            }
            if (Connectionstring == "")
            {
                return lstTools;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ToolLocs = null;
            string ToolCats = null;
            string ToolCost = null;
            string ToolCreaters = null;
            string ToolUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ToolCheckOutUDF1 = null;
            string ToolCheckOutUDF2 = null;
            string ToolCheckOutUDF3 = null;
            string ToolCheckOutUDF4 = null;
            string ToolCheckOutUDF5 = null;

            string ToolTechUDF1 = null;
            string ToolTechUDF2 = null;
            string ToolTechUDF3 = null;
            string ToolTechUDF4 = null;
            string ToolTechUDF5 = null;
            string ToolMaintence = null;
            string TechnicianList = null;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools_History", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ToolCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ToolUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string[] arrReplenishTypes = FieldsPara[9].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF1 = ToolCheckOutUDF1 + supitem + "','";
                    }
                    ToolCheckOutUDF1 = ToolCheckOutUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF1 = HttpUtility.UrlDecode(ToolCheckOutUDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string[] arrReplenishTypes = FieldsPara[10].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF2 = ToolCheckOutUDF2 + supitem + "','";
                    }
                    ToolCheckOutUDF2 = ToolCheckOutUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF2 = HttpUtility.UrlDecode(ToolCheckOutUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    string[] arrReplenishTypes = FieldsPara[11].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF3 = ToolCheckOutUDF3 + supitem + "','";
                    }
                    ToolCheckOutUDF3 = ToolCheckOutUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF3 = HttpUtility.UrlDecode(ToolCheckOutUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[12]))
                {
                    string[] arrReplenishTypes = FieldsPara[12].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF4 = ToolCheckOutUDF4 + supitem + "','";
                    }
                    ToolCheckOutUDF4 = ToolCheckOutUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF4 = HttpUtility.UrlDecode(ToolCheckOutUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[13]))
                {
                    string[] arrReplenishTypes = FieldsPara[13].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF5 = ToolCheckOutUDF5 + supitem + "','";
                    }
                    ToolCheckOutUDF5 = ToolCheckOutUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF5 = HttpUtility.UrlDecode(ToolCheckOutUDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[17]))
                {
                    ToolLocs = FieldsPara[17].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[14]))
                {
                    ToolCats = FieldsPara[14].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    ToolCost = FieldsPara[15].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[16]))
                {
                    string[] arrReplenishTypes = FieldsPara[16].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolMaintence = ToolMaintence + supitem + ",";
                    }
                    ToolMaintence = ToolMaintence.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[18]))
                {
                    string[] arrReplenishTypes = FieldsPara[18].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechnicianList = TechnicianList + supitem + "','";
                    }
                    TechnicianList = TechnicianList.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[19]))
                {
                    string[] arrReplenishTypes = FieldsPara[19].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF1 = ToolTechUDF1 + supitem + "','";
                    }
                    ToolTechUDF1 = ToolTechUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF1 = HttpUtility.UrlDecode(ToolTechUDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[20]))
                {
                    string[] arrReplenishTypes = FieldsPara[20].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF2 = ToolTechUDF2 + supitem + "','";
                    }
                    ToolTechUDF2 = ToolTechUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF2 = HttpUtility.UrlDecode(ToolTechUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    string[] arrReplenishTypes = FieldsPara[21].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF3 = ToolTechUDF3 + supitem + "','";
                    }
                    ToolTechUDF3 = ToolTechUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF3 = HttpUtility.UrlDecode(ToolTechUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    string[] arrReplenishTypes = FieldsPara[22].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF4 = ToolTechUDF4 + supitem + "','";
                    }
                    ToolTechUDF4 = ToolTechUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF4 = HttpUtility.UrlDecode(ToolTechUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    string[] arrReplenishTypes = FieldsPara[23].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF5 = ToolTechUDF5 + supitem + "','";
                    }
                    ToolTechUDF5 = ToolTechUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF5 = HttpUtility.UrlDecode(ToolTechUDF5);
                }

                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools_History", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            else
            {
                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools_History", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            if (dsTools != null && dsTools.Tables.Count > 0)
            {
                DataTable dtTools = dsTools.Tables[0];
                if (dtTools.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtTools.Rows[0]["TotalRecords"]);
                    lstTools = dtTools.AsEnumerable().Select(row => new ToolMasterDTO()
                    {
                        AddedFrom = row.Field<string>("AddedFrom"),
                        CheckedOutMQTY = row.Field<double?>("CheckedOutMQTY"),
                        CheckedOutQTY = row.Field<double?>("CheckedOutQTY"),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Cost = row.Field<double?>("Cost"),
                        Created = row.Field<DateTime>("Created"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        Description = row.Field<string>("Description"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        GUID = row.Field<Guid>("GUID"),
                        ID = row.Field<long>("ID"),
                        IsArchived = row.Field<bool?>("IsArchived"),
                        IsAutoMaintain = row.Field<bool>("IsAutoMaintain"),
                        IsCheckedOut = row.Field<bool?>("IscheckedOut"),
                        IsDeleted = row.Field<bool?>("IsDeleted"),
                        IsGroupOfItems = row.Field<int?>("IsGroupOfItems"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        LocationID = row.Field<long?>("LocationID"),
                        MaintenanceType = row.Field<int>("MaintenanceType"),
                        NoOfPastMntsToConsider = row.Field<int?>("NoOfPastMntsToConsider"),
                        MaintenanceDueNoticeDays = row.Field<int?>("MaintenanceDueNoticeDays"),
                        Quantity = row.Field<double>("Quantity"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        Room = row.Field<long?>("Room"),
                        Serial = row.Field<string>("Serial"),
                        ToolCategoryID = row.Field<long?>("ToolCategoryID"),
                        ToolName = row.Field<string>("ToolName"),
                        ToolUDF1 = row.Field<string>("UDF1"),
                        ToolUDF2 = row.Field<string>("UDF2"),
                        ToolUDF3 = row.Field<string>("UDF3"),
                        ToolUDF4 = row.Field<string>("UDF4"),
                        ToolUDF5 = row.Field<string>("UDF5"),
                        Updated = row.Field<DateTime?>("Updated"),
                        Location = row.Field<string>("Location"),
                        ToolCategory = row.Field<string>("ToolCategory"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        Technician = row.Field<string>("Technician"),
                        ImageType = row.Field<string>("ImageType"),
                        IsBeforeCheckOutAndCheckIn = row.Field<bool>("IsBeforeCheckOutAndCheckIn"),
                        CheckedOutQTYTotal = row.Field<int?>("CheckedOutQTYTotal"),
                        ImagePath = row.Field<string>("ImageType") == "ExternalImage" ? string.Empty : row.Field<string>("ImagePath"),
                        ToolImageExternalURL = row.Field<string>("ImageType") == "ExternalImage" ? row.Field<string>("ToolImageExternalURL") : string.Empty,
                        Type = row.Field<Int64?>("Type"),
                        IsBuildBreak = row.Field<bool>("IsBuildBreak"),
                        IsAtleaseOneCheckOutCompleted = row.Field<bool>("IsAtleaseOneCheckOutCompleted"),
                        AvailableToolQty = row.Field<double?>("AvailableToolQty"),
                        ToolTypeTracking = row.Field<string>("ToolTypeTracking"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                    }).ToList();

                }
            }
            return lstTools;
        }

        public List<ToolMasterDTO> GetAllToolDataForService(long RoomID, long CompanyID, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<ToolMasterDTO> objList = new List<ToolMasterDTO>();
            int TotalRecordCount = 0;
            objList = GetPagedTools(0, int.MaxValue, out TotalRecordCount, string.Empty, string.Empty, RoomID, CompanyID, false, false, null, null, RoomDateFormat, CurrentTimeZone);

            return objList;
        }
        public void UpdateToolCost(Guid ToolGUID, Int64 RoomID, Int64 CompanyID, string FromWhere = "")
        {
            ToolMasterDAL objItem = new ToolMasterDAL(base.DataBaseName);
            ToolMasterDTO ItemDTO = objItem.GetToolByGUIDPlain(ToolGUID);
            ToolAssetQuantityDetailDAL objToolAssetQuantityDetail = new ToolAssetQuantityDetailDAL(base.DataBaseName);
            ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
            List<ToolAssetQuantityDetailDTO> lstToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDAL(base.DataBaseName).GetToolAssetQuantityByAssetToolGuid(RoomID, CompanyID, false, false, ToolGUID).OrderByDescending(t => t.ReceivedDate).ThenByDescending(t => t.ID).ToList();
            if (lstToolAssetQuantityDetailDTO != null && lstToolAssetQuantityDetailDTO.Any() && lstToolAssetQuantityDetailDTO.Count() > 0)
            {
                ItemDTO.Cost = lstToolAssetQuantityDetailDTO.FirstOrDefault().Cost;
            }
            if (ItemDTO != null)
            {
                ItemDTO.Cost = ItemDTO.Cost;
                ItemDTO.WhatWhereAction = "Update Cost";
                objItem.Edit(ItemDTO);
            }
        }


        public List<NarrowSearchDTO> GetToolListNarrowSearch(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type, bool NotIncludeDeletedUDF, string CurrentTab, int LoadDataCount = 0)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName),
                            new SqlParameter("@RoomId", RoomID),
                            new SqlParameter("@CompanyId", CompanyId),
                            new SqlParameter("@Isdeleted", IsDeleted),
                            new SqlParameter("@IsArchived", IsArchived),
                            new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid),
                            new SqlParameter("@Type", Type),
                            new SqlParameter("@NotIncludeDeletedUDF", NotIncludeDeletedUDF),
                            new SqlParameter("@CurrentTab", CurrentTab ?? (object)DBNull.Value),
                            new SqlParameter("@LoadDataCount", LoadDataCount)
                };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetToolListNarrowSearch] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type,@NotIncludeDeletedUDF,@CurrentTab,@LoadDataCount", params1).ToList();
            }
        }

        public List<NarrowSearchDTO> GetToolListNarrowSearchCheckOutUDF(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type, string ToolCurrentTab, int LoadDataCount)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type), new SqlParameter("@ToolCurrentTab", ToolCurrentTab ?? (object)DBNull.Value), new SqlParameter("@LoadDataCount", LoadDataCount) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetToolListNarrowSearchCheckOutUDF] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type,@ToolCurrentTab,@LoadDataCount", params1).ToList();
            }
        }

        public List<NarrowSearchDTO> GetToolListNarrowSearchTechnicianUDF(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type, string ToolCurrentTab, int LoadDataCount)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type), new SqlParameter("@ToolCurrentTab", ToolCurrentTab ?? (object)DBNull.Value), new SqlParameter("@LoadDataCount", LoadDataCount) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetToolListNarrowSearchTechnicianUDF] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type,@ToolCurrentTab,@LoadDataCount", params1).ToList();
            }
        }

        public List<CommonDTO> GetAllToolWrittenOffCategories(long companyId,long roomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ToolWrittenOffCategories.Where(t => t.IsDeleted == false && t.CompanyID == companyId && t.Room == roomId).Select(u => new CommonDTO()
                {
                    ID = u.ID,
                    Text = u.WrittenOffCategory,
                }).AsParallel().ToList();
            }
        }

        public bool UpdateToolQuantityOnToolWrittenOff(Guid ToolGUID, double Quantity, long UserId, long CompanyId, long RoomId, out string reasonToFailErrorCode, bool IsFromKit)
        {
            reasonToFailErrorCode = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var tool = context.ToolMasters.Where(x => x.GUID == ToolGUID && x.Room == RoomId && x.CompanyID == CompanyId).FirstOrDefault();
                if (tool != null)
                {
                    if (!IsFromKit)
                    {
                        var qtyAvailableForWrittenOff = (tool.Quantity.GetValueOrDefault(0) - (tool.CheckedOutQTY.GetValueOrDefault(0) + tool.CheckedOutMQTY.GetValueOrDefault(0)));
                        if (Quantity > qtyAvailableForWrittenOff)
                        {
                            reasonToFailErrorCode = "1"; // "Not enough quantity to written off";
                            return false;
                        }
                        //else if ((tool.SerialNumberTracking || tool.IsGroupOfItems.GetValueOrDefault(0) == 1) && (Quantity > (qtyAvailableForWrittenOff - 1)))
                        //{
                        //    reasonToFailErrorCode = "2"; //"Available quantity can not be less than 1.";
                        //    return false;
                        //}
                    }
                }
                var toolAssetQuantityDetails = context.ToolAssetQuantityDetails.Where(x => x.ToolGUID == ToolGUID && x.RoomID == RoomId && x.CompanyID == CompanyId && (x.Quantity ?? 0) > 0).OrderBy(x => x.ID);

                if (toolAssetQuantityDetails != null && toolAssetQuantityDetails.Any())
                {
                    double pendingQtyToWrittenOff = Quantity;

                    foreach (var toolAssetQuantityDetail in toolAssetQuantityDetails)
                    {
                        if (pendingQtyToWrittenOff <= 0)
                            break;

                        var currentQty = pendingQtyToWrittenOff > toolAssetQuantityDetail.Quantity.GetValueOrDefault(0) ? toolAssetQuantityDetail.Quantity.GetValueOrDefault(0) : pendingQtyToWrittenOff;

                        toolAssetQuantityDetail.Quantity = (pendingQtyToWrittenOff > toolAssetQuantityDetail.Quantity) ? 0 : (toolAssetQuantityDetail.Quantity - pendingQtyToWrittenOff);
                        toolAssetQuantityDetail.WhatWhereAction = "Tool was written off from Web.";
                        toolAssetQuantityDetail.EditedOnAction = "Tool was written off from Web.";
                        toolAssetQuantityDetail.ToolAssetOrderDetailGUID = null;
                        toolAssetQuantityDetail.Updated = DateTimeUtility.DateTimeNow;
                        toolAssetQuantityDetail.UpdatedBy = UserId;
                        toolAssetQuantityDetail.EditedFrom = "Web";
                        pendingQtyToWrittenOff -= currentQty;
                    }
                }
                if (toolAssetQuantityDetails != null && toolAssetQuantityDetails.Any())
                {
                    context.SaveChanges();
                }

                ToolMaster obj = context.ToolMasters.Where(x => x.GUID == ToolGUID && x.Room == RoomId && x.CompanyID == CompanyId).FirstOrDefault();
                obj.Quantity = (obj.Quantity.GetValueOrDefault(0) - Quantity);
                //obj.AvailableToolQty = (obj.AvailableToolQty - Quantity);
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = UserId;
                obj.EditedFrom = "Web";
                obj.WhatWhereAction = "The tool was written off from the web";
                context.SaveChanges();
                return true;
            }
        }

        public List<string> UpdateToolQuantityOnToolWrittenOffForNewTool(ToolWrittenOffDTO ToolWrittenOff, long UserId, long CompanyId, long RoomId, out List<string> serialsUpdatedSuccessfully, out string reasonToFailErrorCode, bool IsFromKit)
        {
            List<string> failSerialList = new List<string>();
            serialsUpdatedSuccessfully = new List<string>();
            reasonToFailErrorCode = string.Empty;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var serials = ToolWrittenOff.Serial.Split(',');

                if (!IsFromKit)
                {
                    ToolMaster toolMaster = context.ToolMasters.Where(x => x.GUID == ToolWrittenOff.ToolGUID && x.Room == RoomId && x.CompanyID == CompanyId).FirstOrDefault();
                    var qtyAvailableForWrittenOff = (toolMaster.Quantity.GetValueOrDefault(0) - (toolMaster.CheckedOutQTY.GetValueOrDefault(0) + toolMaster.CheckedOutMQTY.GetValueOrDefault(0)));

                    if (serials.Count() > qtyAvailableForWrittenOff)
                    {
                        reasonToFailErrorCode = "1"; //"Not enough quantity to written off";
                        failSerialList.AddRange(ToolWrittenOff.Serial.Split(','));
                        return failSerialList;
                    }
                    //else if ((toolMaster.SerialNumberTracking || toolMaster.IsGroupOfItems.GetValueOrDefault(0) == 1)
                    //        && (serials.Count() > (qtyAvailableForWrittenOff - 1)))
                    //{
                    //    reasonToFailErrorCode = "2"; //"Available quantity can not be less than 1.";
                    //    failSerialList.AddRange(ToolWrittenOff.Serial.Split(','));
                    //    return failSerialList;
                    //}
                }

                foreach (var serial in serials)
                {
                    var toolAssetQuantityDetail = context.ToolAssetQuantityDetails.Where(x => x.ToolGUID == ToolWrittenOff.ToolGUID && x.RoomID == RoomId && x.CompanyID == CompanyId && x.SerialNumber == serial).OrderByDescending(x => x.ID).FirstOrDefault();
                    if (toolAssetQuantityDetail != null && toolAssetQuantityDetail.Quantity > 0)
                    {
                        toolAssetQuantityDetail.Quantity = 0;
                        toolAssetQuantityDetail.WhatWhereAction = "Tool was written off from Web.";
                        toolAssetQuantityDetail.EditedOnAction = "Tool was written off from Web.";
                        toolAssetQuantityDetail.ToolAssetOrderDetailGUID = null;
                        toolAssetQuantityDetail.Updated = DateTimeUtility.DateTimeNow;
                        toolAssetQuantityDetail.UpdatedBy = UserId;
                        toolAssetQuantityDetail.EditedFrom = "Web";
                        context.SaveChanges();

                        ToolMaster tool = context.ToolMasters.Where(x => x.GUID == ToolWrittenOff.ToolGUID && x.Room == RoomId && x.CompanyID == CompanyId).FirstOrDefault();
                        tool.Quantity = (tool.Quantity.GetValueOrDefault(0) - 1);
                        tool.Updated = DateTimeUtility.DateTimeNow;
                        tool.LastUpdatedBy = UserId;
                        tool.EditedFrom = "Web";
                        tool.WhatWhereAction = "The tool was written off from the web, Serial: " + serial;
                        context.SaveChanges();

                        serialsUpdatedSuccessfully.Add(serial);
                    }
                    else
                    {
                        failSerialList.Add(serial);
                    }
                }

                return failSerialList;
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForWrittenOff(Guid ToolGUID, long RoomID, long CompanyID, string SerialOrLotNumber, double PullQuantity)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var tool = context.ToolMasters.Where(e => e.GUID.Equals(ToolGUID)).FirstOrDefault();

                if (tool != null && tool.GUID != Guid.Empty)
                {
                    if (tool.SerialNumberTracking)
                    {
                        ToolAssetQuantityDetailDAL toolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);
                        var writtenOffSerialQtyDetails = toolAssetQuantityDetailDAL.GetToolSerialForWrittenOff(ToolGUID, RoomID, CompanyID);

                        if (!string.IsNullOrEmpty(SerialOrLotNumber) && writtenOffSerialQtyDetails.Any())
                        {
                            writtenOffSerialQtyDetails = writtenOffSerialQtyDetails.Where(t => t.Value == SerialOrLotNumber.Trim()).ToList();
                        }

                        if (writtenOffSerialQtyDetails.Any())
                        {
                            foreach (var serial in writtenOffSerialQtyDetails)
                            {
                                var toolSerialDetails = new ItemLocationLotSerialDTO
                                {
                                    ItemGUID = tool.GUID,
                                    ID = tool.ID,
                                    SerialNumberTracking = tool.SerialNumberTracking,
                                    Received = FnCommon.ConvertDateByTimeZone(tool.ReceivedOn, true, true),
                                    ReceivedDate = tool.ReceivedOn,
                                    SerialNumber = serial.Value,
                                    LotSerialQuantity = (tool.Quantity.GetValueOrDefault(0) - (tool.CheckedOutQTY.GetValueOrDefault(0) + tool.CheckedOutMQTY.GetValueOrDefault(0))),
                                    LotOrSerailNumber = serial.Value,
                                    PullQuantity = 1,
                                };
                                lstItemLocations.Add(toolSerialDetails);
                            }
                        }
                    }
                    else
                    {
                        var toolDetails = new ItemLocationLotSerialDTO
                        {
                            ItemGUID = tool.GUID,
                            ID = tool.ID,
                            SerialNumberTracking = false,
                            Received = FnCommon.ConvertDateByTimeZone(tool.ReceivedOn, true, true),
                            ReceivedDate = tool.ReceivedOn,
                            SerialNumber = string.Empty,
                            LotSerialQuantity = (tool.Quantity.GetValueOrDefault(0) - (tool.CheckedOutQTY.GetValueOrDefault(0) + tool.CheckedOutMQTY.GetValueOrDefault(0))),
                            LotOrSerailNumber = string.Empty,
                            PullQuantity = PullQuantity < (tool.Quantity.GetValueOrDefault(0) - (tool.CheckedOutQTY.GetValueOrDefault(0) + tool.CheckedOutMQTY.GetValueOrDefault(0)))
                                                        ? PullQuantity
                                                        : (tool.Quantity.GetValueOrDefault(0) - (tool.CheckedOutQTY.GetValueOrDefault(0) + tool.CheckedOutMQTY.GetValueOrDefault(0)))
                        };
                        lstItemLocations.Add(toolDetails);
                    }
                }
            }
            return lstItemLocations;
        }

        public List<ToolMasterDTO> GetToolCheckoutStatusExportData(string ToolIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolIDs", ToolIDs), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [ToolCheckoutStatus_Data] @ToolIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<ToolMasterDTO> GetToolMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolMasterChangeLog] @ID", params1).ToList();
            }
        }

        public List<ToolMasterDTO> GetToolByToolCategoryID(long ToolCategoryID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolCategoryID", ToolCategoryID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolByToolCategoryID] @ToolCategoryID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        #endregion

        #region [Check later for optimization]

        public ToolMasterDTO GetToolByName(string ToolName, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ToolMasters.Where(t => t.Room == RoomID &&
                                                      t.CompanyID == CompanyID &&
                                                      (t.IsDeleted ?? false) == false &&
                                                      t.ToolName.Trim().ToUpper() == ToolName.Trim().ToUpper()).Select(u => new ToolMasterDTO()
                                                      {
                                                          ToolName = u.ToolName,
                                                          Created = u.Created ?? DateTime.MinValue,
                                                          CreatedBy = u.CreatedBy,
                                                          ID = u.ID,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          Room = u.Room,
                                                          Updated = u.Updated,
                                                          CompanyID = u.CompanyID,
                                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                          GUID = u.GUID,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          IsGroupOfItems = u.IsGroupOfItems,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          Type = u.Type,
                                                          IsBuildBreak = u.IsBuildBreak,
                                                          AvailableToolQty = u.AvailableToolQty,
                                                          Quantity = u.Quantity.HasValue ? u.Quantity.Value : 0,
                                                          ToolTypeTracking = u.ToolTypeTracking ?? "1",
                                                          SerialNumberTracking = u.SerialNumberTracking,
                                                          LotNumberTracking = u.LotNumberTracking,
                                                          DateCodeTracking = u.DateCodeTracking
                                                      }).FirstOrDefault();
            }

        }

        #endregion

        //public void ReassignToolCheckOutTechnician(string ToolCheckInCheckOutGuids, Guid TechnicianGuid, long UserId)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var params1 = new SqlParameter[]
        //                                        {
        //                                            new SqlParameter("@ToolCheckInCheckOutGuids", ToolCheckInCheckOutGuids),
        //                                            new SqlParameter("@TechnicianGuid", TechnicianGuid),
        //                                            new SqlParameter("@UpdatedBy", UserId)
        //                                        };
        //        context.Database.ExecuteSqlCommand("EXEC [ReassignToolCheckOutTechnician] @ToolCheckInCheckOutGuids,@TechnicianGuid,@UpdatedBy ", params1);
        //    }
        //}

        public void ReassignToolCheckOutTechnician(string ToolCheckInCheckOutGuids, Guid TechnicianGuid, long UserId, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                                                {
                                                    new SqlParameter("@ToolCheckInCheckOutGuids", ToolCheckInCheckOutGuids),
                                                    new SqlParameter("@TechnicianGuid", TechnicianGuid),
                                                    new SqlParameter("@UpdatedBy", UserId),
                                                    new SqlParameter("@UDF1", UDF1),
                                                    new SqlParameter("@UDF2", UDF2),
                                                    new SqlParameter("@UDF3", UDF3),
                                                    new SqlParameter("@UDF4", UDF4),
                                                    new SqlParameter("@UDF5", UDF5)                                                    
                                                };

                context.Database.ExecuteSqlCommand("EXEC [ReassignToolCheckOutTechnician] @ToolCheckInCheckOutGuids,@TechnicianGuid,@UpdatedBy,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5 ", params1);
            }
        }
        //public void ReassignToolCheckOutTechnician(DataTable ReassignCheckOutTechnician, Guid TechnicianGuid, long UserId)
        //{
        //    string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
        //    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
        //    DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "ReassignToolCheckOutTechnician", TechnicianGuid, UserId, ReassignCheckOutTechnician);
        //}
        
        public bool RemoveToolImage(Guid ToolGUID, string EditedFrom, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[RemoveToolImage] '" + Convert.ToString(ToolGUID) + "','" + EditedFrom + "'," + UserID + "";
                    context.Database.ExecuteSqlCommand(strQuery);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }
        }

    }
}
