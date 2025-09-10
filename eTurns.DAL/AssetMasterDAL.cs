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
    public class AssetMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]        
        public AssetMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public AssetMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion

        #region [Class Methods]
        public List<AssetMasterDTO> GetAllAssetsByRoom(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<AssetMasterDTO>("exec [GetAllAssetsByRoom] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1).ToList();
            }
        }
        public List<AssetMasterDTO> GetAssetsByName(string AssetName, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@AssetName", AssetName ?? string.Empty), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<AssetMasterDTO>("exec GetAssetByName @AssetName,@RoomId,@CompanyID", params1).ToList();
            }
        }
        public List<NarrowSearchDTO> GetAssetListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetAssetListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public IEnumerable<AssetMasterDTO> GetAllRecordsOnlyImages()
        {
            IEnumerable<AssetMasterDTO> ObjCache = null;
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<AssetMasterDTO> obj = (from u in context.Database.SqlQuery<AssetMasterDTO>("EXEC dbo.GetAllAssetMasterOnlyImages ")
                                                   select new AssetMasterDTO
                                                   {
                                                       ID = u.ID,
                                                       Room = u.Room,
                                                       GUID = u.GUID,
                                                       CompanyID = u.CompanyID,
                                                       RoomName = u.RoomName,
                                                       ImagePath = u.ImagePath,
                                                       ImageType = u.ImageType,
                                                       AssetImageExternalURL = u.AssetImageExternalURL,
                                                   }).AsParallel().ToList();
                ObjCache = obj;
                return obj;
            }




        }
        public List<AssetMasterDTO> GetAssetMasterByGUID(Guid GUID, Boolean IsDeleted, Boolean IsArchived)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<AssetMasterDTO>("exec [GetAssetMasterByGUID] @GUID,@IsDeleted,@IsArchived", params1).ToList();
            }
        }
        public List<AssetMasterDTO> GetPagedAssetMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64? Room, Int64? CompanyID, Boolean? IsArchived, Boolean? IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string UDF1 = "";
            string UDF2 = "";
            string UDF3 = "";
            string UDF4 = "";
            string UDF5 = "";
            string UDF6 = "";
            string UDF7 = "";
            string UDF8 = "";
            string UDF9 = "";
            string UDF10 = "";
            string ToolCategoryID = "";
            string AssetCategoryID = "";
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {


                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[29]))
                {
                    ToolCategoryID = FieldsPara[29].TrimEnd(',');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[92]))
                {
                    string[] arrReplenishTypes = FieldsPara[92].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + "','";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[93]))
                {
                    string[] arrReplenishTypes = FieldsPara[93].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + "','";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[94]))
                {
                    string[] arrReplenishTypes = FieldsPara[94].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + "','";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[95]))
                {
                    string[] arrReplenishTypes = FieldsPara[95].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + "','";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[96]))
                {
                    string[] arrReplenishTypes = FieldsPara[96].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + "','";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }

                //if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                //{
                //    Itemname = FieldsPara[11].TrimEnd(',');
                //}

                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[6].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //CreatedDateTo = Convert.ToDateTime(FieldsPara[6].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[7].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // UpdatedDateTo = Convert.ToDateTime(FieldsPara[7].Split(',')[1]).Date.ToString("dd-MM-yyyy");

                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[106]))
                {
                    AssetCategoryID = FieldsPara[106].TrimEnd(',');
                }
            }
            else
            {
                SearchTerm = "";
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm),
                    new SqlParameter("@sortColumnName", sortColumnName),

                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),

                    new SqlParameter("@Room", Room),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@UDF6", UDF6),
                    new SqlParameter("@UDF7", UDF7),
                    new SqlParameter("@UDF8", UDF8),
                    new SqlParameter("@UDF9", UDF9),
                    new SqlParameter("@UDF10", UDF10),
                    new SqlParameter("@AssetCategory", AssetCategoryID)


                };
                List<AssetMasterDTO> lstcats = context.Database.SqlQuery<AssetMasterDTO>("exec [GetPagedAssetMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@UDF6,@UDF7,@UDF8,@UDF9,@UDF10,@AssetCategory", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }

        }
        public AssetMasterDTO GetAssetById(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<AssetMasterDTO>("exec [GetAssetMasterByID] @ID,@IsDeleted,@IsArchived,@RoomId,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<AssetMasterDTO> GetRecordByAssetName(string AssetName, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@AssetName", AssetName ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<AssetMasterDTO>("exec GetAssetByAssetNameList @AssetName,@RoomID,@CompanyID", params1).ToList<AssetMasterDTO>();
            }
        }
        public AssetMasterDTO GetHistoryRecord(Int64 HistoryID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID), new SqlParameter("@dbName", DataBaseName) };
                return (from u in context.Database.SqlQuery<AssetMasterDTO>("exec GetAssetMasterChangeLogByHistoryID @HistoryID,@dbName", params1)
                        select new AssetMasterDTO
                        {
                            ID = u.ID,
                            AssetName = u.AssetName,
                            Description = u.Description,
                            Make = u.Make,
                            Model = u.Model,
                            Serial = u.Serial,
                            ToolCategoryID = u.ToolCategoryID,
                            ToolCategory = u.ToolCategory,
                            PurchaseDate = u.PurchaseDate,
                            PurchasePrice = u.PurchasePrice,
                            DepreciatedValue = u.DepreciatedValue,
                            SuggestedMaintenanceDate = u.SuggestedMaintenanceDate,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
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
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                            MaintenanceType = u.MaintenanceType,
                            ImagePath = u.ImagePath,
                            ImageType = u.ImageType,
                            AssetImageExternalURL = u.AssetImageExternalURL,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays
                        }).SingleOrDefault();
            }
        }
        public AssetMasterDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            AssetMasterDTO ObjDTO = GetAssetMasterByGUID(GUID, IsArchived, IsDeleted).FirstOrDefault();
            if (ObjDTO != null && ObjDTO.ID > 0)
                return ObjDTO;
            else
                return new AssetMasterDTO();
        }
        public Int64 Insert(AssetMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                AssetMaster obj = new AssetMaster();
                obj.ID = 0;
                obj.AssetName = objDTO.AssetName;
                obj.Description = objDTO.Description;
                obj.Make = objDTO.Make;
                obj.Model = objDTO.Model;
                obj.Serial = objDTO.Serial;
                obj.ToolCategoryID = objDTO.ToolCategoryID;
                obj.PurchaseDate = objDTO.PurchaseDate;
                obj.PurchasePrice = objDTO.PurchasePrice;
                obj.DepreciatedValue = objDTO.DepreciatedValue;
                obj.SuggestedMaintenanceDate = objDTO.SuggestedMaintenanceDate;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;
                obj.AssetCategoryId = objDTO.AssetCategoryID;
                obj.NoOfPastMntsToConsider = objDTO.NoOfPastMntsToConsider;
                obj.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;
                obj.MaintenanceType = objDTO.MaintenanceType;

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;
                else
                    obj.AddedFrom = "Web";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom;
                else
                    obj.EditedFrom = "Web";

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;

                obj.ImageType = objDTO.ImageType;
                obj.ImagePath = objDTO.ImagePath;
                obj.AssetImageExternalURL = objDTO.AssetImageExternalURL;

                context.AssetMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("NextAssetNo", objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.AssetName);
                }

                return obj.ID;
            }

        }

        public Int64 InsertFromPullHistoryImport(AssetMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                AssetMaster obj = new AssetMaster();
                obj.ID = 0;
                obj.AssetName = objDTO.AssetName;
                obj.Description = objDTO.Description;
                obj.Make = objDTO.Make;
                obj.Model = objDTO.Model;
                obj.Serial = objDTO.Serial;
                obj.ToolCategoryID = objDTO.ToolCategoryID;
                obj.PurchaseDate = objDTO.PurchaseDate;
                obj.PurchasePrice = objDTO.PurchasePrice;
                obj.DepreciatedValue = objDTO.DepreciatedValue;
                obj.SuggestedMaintenanceDate = objDTO.SuggestedMaintenanceDate;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;
                obj.AssetCategoryId = objDTO.AssetCategoryID;
                obj.NoOfPastMntsToConsider = objDTO.NoOfPastMntsToConsider;
                obj.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;
                obj.MaintenanceType = objDTO.MaintenanceType;

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom;
                else
                    obj.AddedFrom = "Web";

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom;
                else
                    obj.EditedFrom = "Web";
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;

                obj.ImageType = objDTO.ImageType;
                obj.ImagePath = objDTO.ImagePath;
                obj.AssetImageExternalURL = objDTO.AssetImageExternalURL;

                context.AssetMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("NextAssetNo", objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.AssetName);
                }

                return obj.ID;
            }

        }
        public bool Edit(AssetMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                // AssetMaster obj = new AssetMaster();
                AssetMaster obj = context.AssetMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.ID = objDTO.ID;
                obj.AssetName = objDTO.AssetName;
                obj.Description = objDTO.Description;
                obj.Make = objDTO.Make;
                obj.Model = objDTO.Model;
                obj.Serial = objDTO.Serial;
                obj.ToolCategoryID = objDTO.ToolCategoryID;
                obj.PurchaseDate = objDTO.PurchaseDate;
                obj.PurchasePrice = objDTO.PurchasePrice;
                obj.DepreciatedValue = objDTO.DepreciatedValue;
                obj.SuggestedMaintenanceDate = objDTO.SuggestedMaintenanceDate;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = objDTO.Updated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.GUID = objDTO.GUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;
                obj.AssetCategoryId = objDTO.AssetCategoryID;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.NoOfPastMntsToConsider = objDTO.NoOfPastMntsToConsider;
                obj.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;
                obj.MaintenanceType = objDTO.MaintenanceType;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                }

                obj.ImageType = objDTO.ImageType;
                if (!string.IsNullOrEmpty(objDTO.ImagePath))
                {
                    obj.ImagePath = objDTO.ImagePath;
                }

                obj.AssetImageExternalURL = objDTO.AssetImageExternalURL;

                // context.AssetMasters.Attach(obj);                
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                context.Database.ExecuteSqlCommand("EXEC [DeleteAssetMaster] @IDs,@UserID,@CompanyID", params1);
                return true;
            }
        }
        public string DeleteMappingRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            int cnt = 0;
            string strmsg = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                cnt = context.Database.SqlQuery<int>("EXEC [DeleteToolsSchedulerMapping] @IDs,@UserID,@CompanyID", params1).FirstOrDefault();
                strmsg = Convert.ToString(cnt); 

                return strmsg;
            }
        }
        public string UnDeleteMappingRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            int cnt = 0;
            string strmsg = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID) };

                cnt = context.Database.SqlQuery<int>("EXEC [UnDeleteToolsSchedulerMapping] @IDs,@UserID,@CompanyID", params1).FirstOrDefault();
                strmsg = Convert.ToString(cnt);

                return strmsg;
            }
        }
        public bool ExecuteJob(DateTime Rundate)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[Job_ScheduleMaintenance_Testing] '" + Rundate.ToShortDateString() + "'";
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
        public List<ToolsSchedulerMappingDTO> GetPagedRecordsToolScheduleMapping(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, Int64 RoomId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            DataSet dsMapping = new DataSet();
            List<ToolsSchedulerMappingDTO> lstToolsSchedulerMappingDTO = new List<ToolsSchedulerMappingDTO>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            TotalCount = 0;
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            string SchedulerFor = null;
            string SchedulerType = null;
            string Itemname = null;
            string SchedulerName = null;
            string ScheduleCreaters = null;
            string ToolAssets = null;
            string Tools = null;
            string Assets = null;
            string ScheduleUpdators = null;
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
                dsMapping = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedShedulermapping", StartRowIndex, MaxRows, SearchTerm, sortColumnName, SchedulerFor, SchedulerType, Itemname, Tools, Assets, SchedulerName, ScheduleCreaters, ScheduleUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if(FieldsPara.Count() < 20)
                {
                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {
                        SchedulerFor = FieldsPara[0].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                    {
                        SchedulerType = FieldsPara[2].TrimEnd(',');
                    }
                    //if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                    //{
                    //    Itemname = FieldsPara[11].TrimEnd(',');
                    //}
                    if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                    {
                        SchedulerName = FieldsPara[3].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                    {
                        ScheduleUpdators = FieldsPara[4].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                    {
                        ScheduleCreaters = FieldsPara[5].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                    {
                        ToolAssets = FieldsPara[8].TrimEnd(',');
                        foreach (var item in ToolAssets.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[1] == "tool")
                            {
                                Tools = Tools + item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[0] + "','";
                            }
                            else
                            {
                                Assets = Assets + item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[0] + "','";
                            }
                        }
                        Tools = (Tools ?? string.Empty).TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        Assets = (Assets ?? string.Empty).TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                    {
                        //CreatedDateFrom = Convert.ToDateTime(FieldsPara[6].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        //CreatedDateTo = Convert.ToDateTime(FieldsPara[6].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                        CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                    {
                        //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[7].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        // UpdatedDateTo = Convert.ToDateTime(FieldsPara[7].Split(',')[1]).Date.ToString("dd-MM-yyyy");

                        UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[7].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[7].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                   
                    if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                    {
                        string[] arrReplenishTypes = FieldsPara[9].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF1 = UDF1 + supitem + "','";
                        }
                        UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF1 = HttpUtility.UrlDecode(UDF1);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                    {
                        string[] arrReplenishTypes = FieldsPara[10].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF2 = UDF2 + supitem + "','";
                        }
                        UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF2 = HttpUtility.UrlDecode(UDF2);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                    {
                        string[] arrReplenishTypes = FieldsPara[11].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF3 = UDF3 + supitem + "','";
                        }
                        UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF3 = HttpUtility.UrlDecode(UDF3);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[12]))
                    {
                        string[] arrReplenishTypes = FieldsPara[12].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF4 = UDF4 + supitem + "','";
                        }
                        UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF4 = HttpUtility.UrlDecode(UDF4);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[13]))
                    {
                        string[] arrReplenishTypes = FieldsPara[13].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF5 = UDF5 + supitem + "','";
                        }
                        UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF5 = HttpUtility.UrlDecode(UDF5);
                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(FieldsPara[120]))
                    {
                        SchedulerFor = FieldsPara[120].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[122]))
                    {
                        SchedulerType = FieldsPara[122].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[123]))
                    {
                        SchedulerName = FieldsPara[123].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                    {
                        ScheduleUpdators = FieldsPara[1].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {
                        ScheduleCreaters = FieldsPara[0].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[124]))
                    {
                        ToolAssets = FieldsPara[124].TrimEnd(',');
                        foreach (var item in ToolAssets.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[1] == "tool")
                            {
                                Tools = Tools + item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[0] + "','";
                            }
                            else
                            {
                                Assets = Assets + item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[0] + "','";
                            }
                        }
                        Tools = (Tools ?? string.Empty).TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        Assets = (Assets ?? string.Empty).TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
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

                }

                dsMapping = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedShedulermapping", StartRowIndex, MaxRows, SearchTerm, sortColumnName, SchedulerFor, SchedulerType, Itemname, Tools, Assets, SchedulerName, ScheduleCreaters, ScheduleUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId);
            }
            else
            {
                dsMapping = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedShedulermapping", StartRowIndex, MaxRows, SearchTerm, sortColumnName, SchedulerFor, SchedulerType, Itemname, Tools, Assets, SchedulerName, ScheduleCreaters, ScheduleUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId);
            }
            if (dsMapping != null && dsMapping.Tables.Count > 0)
            {
                DataTable dtRooms = dsMapping.Tables[0];

                if (dtRooms.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtRooms.Rows[0]["TotalRecords"]);
                }
                lstToolsSchedulerMappingDTO = (from im in dtRooms.AsEnumerable()
                                               select new ToolsSchedulerMappingDTO
                                               {
                                                   ID = im.Field<long>("ID"),
                                                   SchedulerForName = im.Field<string>("SchedulerForName"),
                                                   SchedulerTypeName = im.Field<string>("SchedulerTypeName"),
                                                   AssetName = im.Field<string>("AssetName"),
                                                   ToolName = im.Field<string>("ToolName"),
                                                   SchedulerName = im.Field<string>("SchedulerName"),
                                                   UDF1 = im.Field<string>("UDF1"),
                                                   UDF2 = im.Field<string>("UDF2"),
                                                   UDF3 = im.Field<string>("UDF3"),
                                                   UDF4 = im.Field<string>("UDF4"),
                                                   UDF5 = im.Field<string>("UDF5"),
                                                   GUID = im.Field<Guid>("GUID"),
                                                   Created = im.Field<DateTime?>("Created"),
                                                   Updated = im.Field<DateTime?>("Updated"),
                                                   CreatedBy = im.Field<long?>("CreatedBy"),
                                                   LastUpdatedBy = im.Field<long?>("LastUpdatedBy"),
                                                   IsDeleted = im.Field<bool>("IsDeleted"),
                                                   IsArchived = im.Field<bool>("IsArchived"),
                                                   CompanyID = im.Field<long>("CompanyID"),
                                                   CreatedByName = im.Field<string>("CreatedByName"),
                                                   UpdatedByName = im.Field<string>("UpdatedByName"),
                                                   TrackingMeasurement = im.Field<int?>("TrackingMeasurement"),
                                                   Serial = im.Field<string>("Serial")
                                               }).ToList();
            }


            return lstToolsSchedulerMappingDTO;


        }
        public ToolsSchedulerMappingDTO InsertToolmapping(ToolsSchedulerMappingDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsSchedulerMapping obj = new ToolsSchedulerMapping();
                obj.ID = 0;
                obj.SchedulerFor = objDTO.SchedulerFor;
                //obj.SchedulerType = objDTO.SchedulerType;
                obj.ToolSchedulerGuid = objDTO.ToolSchedulerGuid;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.AssetGUID = objDTO.AssetGUID;
                obj.MaintenanceName = objDTO.MaintenanceName;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.TrackingMeasurement = objDTO.TrackingMeasurement;
                context.ToolsSchedulerMappings.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return GetSchedulerMappingRecord(obj.GUID, obj.CompanyID ?? 0, obj.Room ?? 0, false, false);
            }

        }
        public ToolsSchedulerMappingDTO GetSchedulerMappingRecord(Guid MappingGUID, long CompanyId, long RoomId, bool IsArchive, bool Isdeleted)
        { 
            ToolsSchedulerMappingDTO objDTO = new ToolsSchedulerMappingDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@MappingGUID", MappingGUID) 
                                                   ,new SqlParameter("@CompanyID", CompanyId)
                                                   ,new SqlParameter("@RoomID", RoomId)
                                                   ,new SqlParameter("@IsDeleted", Isdeleted)
                                                   ,new SqlParameter("@IsArchived", IsArchive) };

                objDTO = (from u in context.Database.SqlQuery<ToolsSchedulerMappingDTO>(@"EXEC dbo.GetSchedulerMappingRecord  @MappingGUID,@CompanyID,@RoomID,@IsDeleted,@IsArchived", params1)
                          select new ToolsSchedulerMappingDTO
                          {
                              ID = u.ID,
                              SchedulerFor = u.SchedulerFor,
                              //SchedulerType = u.SchedulerType,
                              ToolSchedulerGuid = u.ToolSchedulerGuid,
                              ToolGUID = u.ToolGUID,
                              AssetGUID = u.AssetGUID,
                              Created = u.Created,
                              CreatedBy = u.CreatedBy,
                              Updated = u.Updated,
                              LastUpdatedBy = u.LastUpdatedBy,
                              Room = u.Room,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              GUID = u.GUID,
                              CompanyID = u.CompanyID,
                              UDF1 = u.UDF1,
                              UDF2 = u.UDF2,
                              UDF3 = u.UDF3,
                              UDF4 = u.UDF4,
                              UDF5 = u.UDF5,
                              CreatedByName = u.CreatedByName,
                              UpdatedByName = u.UpdatedByName,
                              RoomName = u.RoomName,
                              MaintenanceName = u.MaintenanceName,
                              TrackingMeasurement = u.TrackingMeasurement
                          }
                    ).SingleOrDefault();
            }
            return objDTO;
        }
        public List<ToolsSchedulerMappingDTO> GetSchedulerMappingRecord(long CompanyId, long RoomId, bool IsArchive, bool Isdeleted)
        {
            List<ToolsSchedulerMappingDTO> objDTO = new List<ToolsSchedulerMappingDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objDTO = (from u in context.ToolsSchedulerMappings
                          join um in context.UserMasters on u.CreatedBy equals um.ID into im_UMC_join
                          from im_UMC in im_UMC_join.DefaultIfEmpty()

                          join umU in context.UserMasters on u.LastUpdatedBy equals umU.ID into im_UMU_join
                          from im_UMU in im_UMU_join.DefaultIfEmpty()
                          join rm in context.Rooms on u.Room equals rm.ID into im_RM_join
                          from im_RM in im_RM_join.DefaultIfEmpty()
                          where u.CompanyID == CompanyId && u.Room == RoomId && u.IsArchived == IsArchive && u.IsDeleted == Isdeleted
                          select new ToolsSchedulerMappingDTO
                          {
                              ID = u.ID,
                              SchedulerFor = u.SchedulerFor,
                              //SchedulerType = u.SchedulerType,
                              ToolSchedulerGuid = u.ToolSchedulerGuid,
                              ToolGUID = u.ToolGUID,
                              AssetGUID = u.AssetGUID,
                              Created = u.Created,
                              CreatedBy = u.CreatedBy,
                              Updated = u.Updated,
                              LastUpdatedBy = u.LastUpdatedBy,
                              Room = u.Room ?? 0,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              GUID = u.GUID,
                              CompanyID = u.CompanyID ?? 0,
                              UDF1 = u.UDF1,
                              UDF2 = u.UDF2,
                              UDF3 = u.UDF3,
                              UDF4 = u.UDF4,
                              UDF5 = u.UDF5,
                              CreatedByName = im_UMC.UserName,
                              UpdatedByName = im_UMU.UserName,
                              RoomName = im_RM.RoomName,
                              MaintenanceName = u.MaintenanceName,
                              TrackingMeasurement = u.TrackingMeasurement
                          }).ToList();
            }
            return objDTO;
        }
        public List<ToolsSchedulerMappingDTO> GetSchedulerMappingRecordforTool_SchedularGUID(Guid ToolGUID, long CompanyId, long RoomId, bool IsArchive, bool Isdeleted)
        {

            List<ToolsSchedulerMappingDTO> objDTO = new List<ToolsSchedulerMappingDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objDTO = (from u in context.ToolsSchedulerMappings
                          join ts in context.ToolsSchedulers on u.ToolSchedulerGuid equals ts.GUID
                          join um in context.UserMasters on u.CreatedBy equals um.ID into im_UMC_join
                          from im_UMC in im_UMC_join.DefaultIfEmpty()

                          join umU in context.UserMasters on u.LastUpdatedBy equals umU.ID into im_UMU_join
                          from im_UMU in im_UMU_join.DefaultIfEmpty()
                          join rm in context.Rooms on u.Room equals rm.ID into im_RM_join
                          from im_RM in im_RM_join.DefaultIfEmpty()
                          where u.CompanyID == CompanyId && u.Room == RoomId && u.IsArchived == IsArchive && u.IsDeleted == Isdeleted
                                && u.ToolGUID == ToolGUID && ts.SchedulerType == (int)MaintenanceScheduleType.CheckOuts
                          select new ToolsSchedulerMappingDTO
                          {
                              ID = u.ID,
                              SchedulerFor = u.SchedulerFor,
                              //SchedulerType = u.SchedulerType,
                              ToolSchedulerGuid = u.ToolSchedulerGuid,
                              ToolGUID = u.ToolGUID,
                              AssetGUID = u.AssetGUID,
                              Created = u.Created,
                              CreatedBy = u.CreatedBy,
                              Updated = u.Updated,
                              LastUpdatedBy = u.LastUpdatedBy,
                              Room = u.Room ?? 0,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              GUID = u.GUID,
                              CompanyID = u.CompanyID ?? 0,
                              UDF1 = u.UDF1,
                              UDF2 = u.UDF2,
                              UDF3 = u.UDF3,
                              UDF4 = u.UDF4,
                              UDF5 = u.UDF5,
                              CreatedByName = im_UMC.UserName,
                              UpdatedByName = im_UMU.UserName,
                              RoomName = im_RM.RoomName,
                              MaintenanceName = u.MaintenanceName,
                              TrackingMeasurement = u.TrackingMeasurement
                          }).ToList();
            }
            return objDTO;
        }
        public bool CheckScheduleMapping(ToolsSchedulerMappingDTO objDTO)
        {
            bool ret = false;
            ToolsSchedulerMappingDTO objToolsSchedulerMappingDTO = new ToolsSchedulerMappingDTO();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDTO != null && objDTO.AssetGUID != null)
                {
                    objToolsSchedulerMappingDTO = (from u in context.ToolsSchedulerMappings
                                                   where u.CompanyID == objDTO.CompanyID && u.Room == objDTO.Room && u.IsArchived == objDTO.IsArchived && u.IsDeleted == objDTO.IsDeleted
                                                   && u.AssetGUID == objDTO.AssetGUID && u.ToolSchedulerGuid == objDTO.ToolSchedulerGuid &&
                                                   u.SchedulerFor == objDTO.SchedulerFor
                                                   select new ToolsSchedulerMappingDTO
                                                   {
                                                       ID = u.ID,
                                                       SchedulerFor = u.SchedulerFor,
                                                       //SchedulerType = u.SchedulerType,
                                                       ToolSchedulerGuid = u.ToolSchedulerGuid,
                                                       ToolGUID = u.ToolGUID,
                                                       AssetGUID = u.AssetGUID,
                                                       Created = u.Created,
                                                       CreatedBy = u.CreatedBy,
                                                       Updated = u.Updated,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       Room = u.Room ?? 0,
                                                       IsArchived = u.IsArchived,
                                                       IsDeleted = u.IsDeleted,
                                                       GUID = u.GUID,
                                                       CompanyID = u.CompanyID ?? 0,
                                                       UDF1 = u.UDF1,
                                                       UDF2 = u.UDF2,
                                                       UDF3 = u.UDF3,
                                                       UDF4 = u.UDF4,
                                                       UDF5 = u.UDF5,
                                                       MaintenanceName = u.MaintenanceName,
                                                       TrackingMeasurement = u.TrackingMeasurement
                                                   }
                        ).FirstOrDefault();
                }
                else if (objDTO != null && objDTO.ToolGUID != null)
                {
                    objToolsSchedulerMappingDTO = (from u in context.ToolsSchedulerMappings
                                                   where u.CompanyID == objDTO.CompanyID && u.Room == objDTO.Room && u.IsArchived == objDTO.IsArchived && u.IsDeleted == objDTO.IsDeleted
                                                   && u.ToolGUID == objDTO.ToolGUID && u.ToolSchedulerGuid == objDTO.ToolSchedulerGuid &&
                                                   u.SchedulerFor == objDTO.SchedulerFor
                                                   select new ToolsSchedulerMappingDTO
                                                   {
                                                       ID = u.ID,
                                                       SchedulerFor = u.SchedulerFor,
                                                       //SchedulerType = u.SchedulerType,
                                                       ToolSchedulerGuid = u.ToolSchedulerGuid,
                                                       ToolGUID = u.ToolGUID,
                                                       AssetGUID = u.AssetGUID,
                                                       Created = u.Created,
                                                       CreatedBy = u.CreatedBy,
                                                       Updated = u.Updated,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       Room = u.Room ?? 0,
                                                       IsArchived = u.IsArchived,
                                                       IsDeleted = u.IsDeleted,
                                                       GUID = u.GUID,
                                                       CompanyID = u.CompanyID ?? 0,
                                                       UDF1 = u.UDF1,
                                                       UDF2 = u.UDF2,
                                                       UDF3 = u.UDF3,
                                                       UDF4 = u.UDF4,
                                                       UDF5 = u.UDF5,
                                                       MaintenanceName = u.MaintenanceName,
                                                       TrackingMeasurement = u.TrackingMeasurement
                                                   }
                            ).FirstOrDefault();
                }
            }
            if (objToolsSchedulerMappingDTO != null && objToolsSchedulerMappingDTO.ID > 0)
            {
                ret = true;
            }
            return ret;
        }
        public string GetTrackingMeasurement(Guid SchedulerGUID)
        {
            string strreturn = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var q = context.ToolsSchedulers.FirstOrDefault(t => t.GUID == SchedulerGUID);
                if (q != null)
                {
                    strreturn = q.SchedulerType == 1 ? ResToolsScheduler.TimeBased : (q.SchedulerType == 2 ? ResToolsScheduler.OperationalHours : q.SchedulerType == 3 ? ResToolsScheduler.Mileage : string.Empty);
                }
            }
            return strreturn;
        }
        public List<ToolsMaintenanceDTO> GetPagedRecordsToolMaintenance(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, Int64 RoomId, bool IsArchived, bool IsDeleted, string RoomDateFormat, string TabName, Guid? ToolGUID, Guid? AssetGUID, string GUIDS, TimeZoneInfo CurrentTimeZone)
        {
            DataSet dsMapping = new DataSet();
            List<ToolsMaintenanceDTO> lstToolsMaintenanceDTO = new List<ToolsMaintenanceDTO>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            TotalCount = 0;
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            sortColumnName = sortColumnName.Replace("WOName", "WorkOrder").Replace("RequisitionName", "RequisitionNumber");
            string SchedulerFor = null;
            string SchedulerType = null;
            string Tools = ToolGUID.HasValue && ToolGUID != Guid.Empty ? ToolGUID.ToString() : string.Empty;
            string Assets = AssetGUID.HasValue && AssetGUID != Guid.Empty ? AssetGUID.ToString() : string.Empty;
            string SchedulerName = null;
            string ScheduleCreaters = null;
            string ScheduleUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ToolAssets = null;
            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsMapping = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsMaintenance", StartRowIndex, MaxRows, SearchTerm, sortColumnName, SchedulerFor, SchedulerType, Tools, Assets, SchedulerName, ScheduleCreaters, ScheduleUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId, TabName, GUIDS);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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
                    SchedulerFor = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    SchedulerType = FieldsPara[2].TrimEnd(',');
                }
                //if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                //{
                //    Itemname = FieldsPara[11].TrimEnd(',');
                //}
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    SchedulerName = FieldsPara[3].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    ScheduleUpdators = FieldsPara[4].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    ScheduleCreaters = FieldsPara[5].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    // CreatedDateFrom = Convert.ToDateTime(FieldsPara[6].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // CreatedDateTo = Convert.ToDateTime(FieldsPara[6].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    // UpdatedDateFrom = Convert.ToDateTime(FieldsPara[7].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // UpdatedDateTo = Convert.ToDateTime(FieldsPara[7].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[7].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[7].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    ToolAssets = FieldsPara[8].TrimEnd(',');
                    foreach (var item in ToolAssets.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[1] == "tool")
                        {
                            Tools = Tools + item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[0] + "','";
                        }
                        else
                        {
                            Assets = Assets + item.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries)[0] + "','";
                        }
                    }
                    Tools = (Tools ?? string.Empty).TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    Assets = (Assets ?? string.Empty).TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }



                dsMapping = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsMaintenance", StartRowIndex, MaxRows, SearchTerm, sortColumnName, SchedulerFor, SchedulerType, Tools, Assets, SchedulerName, ScheduleCreaters, ScheduleUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId, TabName, GUIDS);
            }
            else
            {
                dsMapping = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsMaintenance", StartRowIndex, MaxRows, SearchTerm, sortColumnName, SchedulerFor, SchedulerType, Tools, Assets, SchedulerName, ScheduleCreaters, ScheduleUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId, TabName, GUIDS);
            }
            if (dsMapping != null && dsMapping.Tables.Count > 0)
            {
                DataTable dtRooms = dsMapping.Tables[0];

                if (dtRooms.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtRooms.Rows[0]["TotalRecords"]);
                }
                lstToolsMaintenanceDTO = (from im in dtRooms.AsEnumerable()
                                          select new ToolsMaintenanceDTO
                                          {
                                              ID = im.Field<long>("ID"),
                                              MaintenanceName = im.Field<string>("MaintenanceName"),
                                              MaintenanceDate = im.Field<DateTime?>("MaintenanceDate"),
                                              SchedulerName = im.Field<string>("SchedulerName"),
                                              ScheduleDate = im.Field<DateTime?>("ScheduleDate"),
                                              SchedulerForName = im.Field<string>("SchedulerForName"),
                                              SchedulerTypeName = im.Field<string>("SchedulerTypeName"),
                                              Itemname = im.Field<string>("Itemname"),
                                              TrackngMeasurement = im.Field<int>("TrackngMeasurement"),
                                              TrackingMeasurementValue = im.Field<string>("TrackingMeasurementValue"),
                                              LastMaintenanceDate = im.Field<DateTime?>("LastMaintenanceDate"),
                                              LastMeasurementValue = im.Field<string>("LastMeasurementValue"),
                                              RequisitionName = im.Field<string>("RequisitionNumber"),
                                              WOName = im.Field<string>("WorkOrder"),
                                              UDF1 = im.Field<string>("UDF1"),
                                              UDF2 = im.Field<string>("UDF2"),
                                              UDF3 = im.Field<string>("UDF3"),
                                              UDF4 = im.Field<string>("UDF4"),
                                              UDF5 = im.Field<string>("UDF5"),
                                              GUID = im.Field<Guid>("GUID"),
                                              Created = im.Field<DateTime>("Created"),
                                              Updated = im.Field<DateTime>("Updated"),
                                              CreatedBy = im.Field<long?>("CreatedBy"),
                                              LastUpdatedBy = im.Field<long?>("LastUpdatedBy"),
                                              IsDeleted = im.Field<bool>("IsDeleted"),
                                              IsArchived = im.Field<bool>("IsArchived"),
                                              CompanyID = im.Field<long>("CompanyID"),
                                              CreatedByName = im.Field<string>("CreatedByName"),
                                              UpdatedByName = im.Field<string>("UpdatedByName"),
                                              Status = im.Field<string>("Status"),
                                              ToolName = im.Field<string>("ToolName"),
                                              AssetName = im.Field<string>("AssetName"),
                                              WorkorderGUID = im.Field<Guid?>("WorkorderGUID"),
                                              RequisitionGUID = im.Field<Guid?>("RequisitionGUID"),
                                              TrackingMeasurementMapping = im.Field<int?>("TrackingMeasurementMapping"),
                                              Serial = im.Field<string>("Serial"),
                                              TotalCost = im.Field<Double?>("TotalCost"),
                                              AssetGUID = im.Field<Guid?>("AssetGUID"),
                                              ToolGUID = im.Field<Guid?>("ToolGUID")
                                          }).ToList();
            }
            return lstToolsMaintenanceDTO;
        }
        public ToolsMaintenanceDTO GetMaintenanceByGUID(Guid MntnsGUID)
        {
            ToolsMaintenanceDTO objToolsMaintenanceDTO = new ToolsMaintenanceDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objToolsMaintenanceDTO = (from mm in context.ToolsMaintenances
                                          where mm.GUID == MntnsGUID
                                          select new ToolsMaintenanceDTO()
                                          {
                                              AssetGUID = mm.AssetGUID,
                                              CompanyID = mm.CompanyID,
                                              AssetToolGUID = null,
                                              Created = mm.Created,
                                              CreatedBy = mm.CreatedBy,
                                              CreatedByName = string.Empty,
                                              GUID = mm.GUID,
                                              ID = mm.ID,
                                              IsArchived = mm.IsArchived,
                                              IsDeleted = mm.IsDeleted,
                                              LastMaintenanceDate = mm.LastMaintenanceDate,
                                              LastMeasurementValue = mm.LastMeasurementValue,
                                              MaintenanceDate = mm.MaintenanceDate,
                                              LastUpdatedBy = mm.LastUpdatedBy,
                                              MaintenanceName = mm.MaintenanceName,
                                              MaintenanceType = mm.MaintenanceType,
                                              RequisitionGUID = mm.RequisitionGUID,
                                              Room = mm.Room,
                                              ScheduleDate = mm.ScheduleDate,
                                              ScheduleFor = mm.ScheduleFor,
                                              SchedulerGUID = mm.SchedulerGUID,
                                              Status = mm.Status,
                                              SchedulerType = mm.SchedulerType,
                                              ToolGUID = mm.ToolGUID,
                                              TrackngMeasurement = mm.TrackngMeasurement,
                                              TrackingMeasurementValue = mm.TrackingMeasurementValue,
                                              ToolSchedulerGuid = mm.SchedulerGUID,
                                              UDF1 = mm.UDF1,
                                              UDF2 = mm.UDF2,
                                              UDF3 = mm.UDF3,
                                              UDF4 = mm.UDF4,
                                              UDF5 = mm.UDF5,
                                              Updated = mm.Updated,
                                              WorkorderGUID = mm.WorkorderGUID
                                          }).FirstOrDefault();
            }
            return objToolsMaintenanceDTO;
        }
        public ToolsSchedulerDTO GetToolSchedulebyGUID(Guid ScheduleID)
        {
            ToolsSchedulerDTO objToolsMaintenanceDTO = new ToolsSchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objToolsMaintenanceDTO = (from u in context.ToolsSchedulers
                                          where u.GUID == ScheduleID
                                          select new ToolsSchedulerDTO()
                                          {
                                              ID = u.ID,
                                              SchedulerName = u.SchedulerName,
                                              SchedulerType = u.SchedulerType,
                                              OperationalHours = u.OperationalHours,
                                              Mileage = u.Mileage,
                                              Created = u.Created,
                                              CreatedBy = u.CreatedBy,
                                              Updated = u.Updated,
                                              LastUpdatedBy = u.LastUpdatedBy,
                                              Room = u.Room,
                                              IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                              IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                              GUID = u.GUID,
                                              CompanyID = u.CompanyID,
                                              UDF1 = u.UDF1,
                                              UDF2 = u.UDF2,
                                              UDF3 = u.UDF3,
                                              UDF4 = u.UDF4,
                                              UDF5 = u.UDF5,
                                              MainScheduleType = u.SchedulerType,
                                              ScheduleFor = u.ScheduleFor,
                                              lstItems = (from tsd in context.ToolsSchedulerDetails
                                                          join im in context.ItemMasters on new { imguid = tsd.ItemGUID ?? Guid.Empty } equals new { imguid = im.GUID }
                                                          where tsd.ScheduleGUID == u.GUID
                                                          select new ToolsSchedulerDetailsDTO()
                                                          {

                                                              CompanyID = tsd.CompanyID,
                                                              Created = tsd.Created,
                                                              CreatedBy = tsd.CreatedBy,
                                                              GUID = tsd.GUID,
                                                              ID = tsd.ID,
                                                              IsArchived = tsd.IsArchived,
                                                              IsDeleted = tsd.IsDeleted,
                                                              ItemCost = im.Cost,
                                                              ItemDescription = im.Description,
                                                              ItemGUID = im.GUID,
                                                              ItemNumber = im.ItemNumber,
                                                              LastUpdated = tsd.LastUpdated,
                                                              LastUpdatedBy = tsd.LastUpdatedBy,
                                                              Quantity = tsd.Quantity,
                                                              Room = tsd.Room,
                                                              ScheduleGUID = tsd.ScheduleGUID,
                                                              SerialNumberTracking = im.SerialNumberTracking
                                                          })
                                          }).FirstOrDefault();

                return objToolsMaintenanceDTO;
            }
        }
        public ToolsMaintenanceDTO UpdateMaintenanceParams(ToolsMaintenanceDTO objToolsMaintenanceDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsMaintenance objToolsMaintenance = context.ToolsMaintenances.FirstOrDefault(t => t.GUID == objToolsMaintenanceDTO.GUID);
                if (objToolsMaintenance != null)
                {
                    objToolsMaintenance.LastUpdatedBy = objToolsMaintenanceDTO.LastUpdatedBy;
                    objToolsMaintenance.MaintenanceDate = objToolsMaintenanceDTO.MaintenanceDate;
                    objToolsMaintenance.RequisitionGUID = objToolsMaintenanceDTO.RequisitionGUID;

                    objToolsMaintenance.Status = objToolsMaintenanceDTO.Status;
                    objToolsMaintenance.TrackingMeasurementValue = objToolsMaintenanceDTO.TrackingMeasurementValue;
                    objToolsMaintenance.Updated = objToolsMaintenanceDTO.Updated;
                    objToolsMaintenance.WorkorderGUID = objToolsMaintenanceDTO.WorkorderGUID;
                    context.SaveChanges();
                }

            }
            return GetMaintenanceByGUID(objToolsMaintenanceDTO.GUID);
        }
        public bool updateImagePath(long Id, string fileName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                AssetMaster objAssetMaster = context.AssetMasters.FirstOrDefault(t => t.ID == Id);
                if (objAssetMaster != null)
                {
                    objAssetMaster.ImagePath = fileName;
                    context.SaveChanges();
                }
            }
            return true;
        }
        public AssetMasterDTO GetAssetByName(string AssetName, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.AssetMasters.Where(t => t.Room == RoomID && t.CompanyID == CompanyID && (t.IsDeleted ?? false) == false && t.AssetName.Trim().ToUpper() == AssetName.Trim().ToUpper()).Select(u => new AssetMasterDTO()
                {
                    AssetName = u.AssetName,
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

                    AddedFrom = u.AddedFrom,
                    EditedFrom = u.EditedFrom,
                    ReceivedOn = u.ReceivedOn,
                    ReceivedOnWeb = u.ReceivedOnWeb
                }).FirstOrDefault();
            }
        }

        public int GetAssetMaintenceCountForDashboard(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@RoomId", RoomId)
                                                };
                return context.Database.SqlQuery<int>("exec [GetAssetMaintenceCountForDashboard] @CompanyId,@RoomId", params1).FirstOrDefault();
            }
        }

        public List<ToolsMaintenanceDTO> GetAssetMaintenceForDashboardChart(long RoomId, long CompanyId, int NoOfRecords)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@RoomId", RoomId),
                                                    new SqlParameter("@MaxRows", NoOfRecords),

                                                };
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [GetAssetMaintenceForDashboardChart] @CompanyId,@RoomId,@MaxRows", params1).ToList();
            }
        }

        public List<ToolsMaintenanceDTO> GetAssetMaintenceForDashboard(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string SortColumnName, long RoomID, long CompanyID)
        {
            List<ToolsMaintenanceDTO> assets = new List<ToolsMaintenanceDTO>();
            TotalCount = 0;
            DataSet dsAsset = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return assets;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsAsset = SqlHelper.ExecuteDataset(EturnsConnection, "GetAssetMaintenceForDashboard", StartRowIndex, MaxRows, SearchTerm, SortColumnName, CompanyID, RoomID);

            if (dsAsset != null && dsAsset.Tables.Count > 0)
            {
                assets = DataTableHelper.ToList<ToolsMaintenanceDTO>(dsAsset.Tables[0]);

                if (assets != null && assets.Any())
                {
                    TotalCount = assets.ElementAt(0).TotalRecords ?? 0;
                }
            }

            return assets;
        }
        public List<AssetMasterDTO> GetAssetMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<AssetMasterDTO>("exec [GetAssetMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }

        public bool RemoveAssetImage(Guid AssetGUID, string EditedFrom, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[RemoveAssetImage] '" + Convert.ToString(AssetGUID) + "','" + EditedFrom + "'," + UserID + "";
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

        public List<ToolsMaintenanceDTO> GetToolsMaintenanceByIDsNormal(string ToolsMaintenanceIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolsMaintenanceIDs", ToolsMaintenanceIDs), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [GetToolsMaintenanceByIDsNormal] @ToolsMaintenanceIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        #endregion

    }
}


