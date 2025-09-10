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
    public partial class LocationMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public LocationMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public LocationMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Method]

        public Int64 Insert(LocationMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);
            string columnList = "ID,RoomName";
            RoomDTO objROOMDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objDTO.Room.GetValueOrDefault(0).ToString() + "", "");


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LocationMaster obj = new LocationMaster();
                obj.ID = 0;
                obj.Location = objDTO.Location;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                context.LocationMasters.Add(obj);
                context.SaveChanges();
                objDTO.RoomName = objROOMDTO.RoomName;
                objDTO.CreatedByName = (from u in context.UserMasters where u.ID == objDTO.CreatedBy select u.UserName).FirstOrDefault();
                objDTO.UpdatedByName = (from u in context.UserMasters where u.ID == objDTO.LastUpdatedBy select u.UserName).FirstOrDefault();
                objDTO.ID = obj.ID;


                return obj.ID;
            }
        }

        public bool Edit(LocationMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                LocationMaster obj = context.LocationMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.Location = objDTO.Location;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.LastUpdated = objDTO.LastUpdated;
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

                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;

                context.SaveChanges();


                return true;
            }
        }


        public bool UnDeleteRecords(string IDs, Int64 userid)
        {
            if (!string.IsNullOrWhiteSpace(IDs))
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string vIDs = (IDs ?? string.Empty).TrimEnd(',');
                    if (!string.IsNullOrWhiteSpace(vIDs))
                    {
                        var params1 = new SqlParameter[] {
                                                        new SqlParameter("@UserID", userid),
                                                        new SqlParameter("@LocationIDs", vIDs)
                                                        };

                        string strQuery = @"EXEC UnDeleteLocationMasterByIDs @UserID,@LocationIDs";
                        context.Database.ExecuteSqlCommand(strQuery, params1);

                        return true;
                    }
                    return false;
                }
            }
            return false;
        }


        public LocationMasterDTO GetLocationOrInsert(string LocationName, long RoomID, long CompanyID, long UserID, string WhatWhereAction)
        {
            try
            {
                if (LocationName == null || LocationName == "null")
                {
                    LocationName = string.Empty;
                }
                LocationMasterDTO objBinMasterDTO = GetLocationByNamePlain(LocationName, RoomID, CompanyID);
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

                    objBinMasterDTO.ID = Insert(objBinMasterDTO);
                    objBinMasterDTO = GetLocationByNamePlain(LocationName, RoomID, CompanyID);
                }
                return objBinMasterDTO;
            }
            catch
            {
                return null;
            }
        }
        public LocationMasterDTO GetToolLocation(Guid ToolGUID, string LocationName, long RoomID, long CompanyID, long UserID, string WhatWhereAction, bool? IsDefault = false)
        {
            if (LocationName == null || LocationName == "null")
            {
                LocationName = string.Empty;
            }
            ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);

            Guid? UsedToolGUId = ToolGUID;


            long binID = 0;

            LocationMasterDTO objBinMasterDTO = GetLocationByNamePlain(LocationName, RoomID, CompanyID);
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

                objBinMasterDTO.ID = Insert(objBinMasterDTO);
                objBinMasterDTO = GetLocationByNamePlain(LocationName, RoomID, CompanyID);
                binID = objBinMasterDTO.ID;


                ToolLocationDetailsDTO objToolLocationDetailsDTO = new ToolLocationDetailsDTO();
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
                objToolLocationDetailsDTO.IsArchieved = (false);
                objToolLocationDetailsDTO.IsDefault = IsDefault;
                objToolLocationDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objToolLocationDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objToolLocationDetailsDTO.WhatWhereAction = WhatWhereAction;
                objToolLocationDetailsDTO.LocationID = objBinMasterDTO.ID;
                objToolLocationDetail.Insert(objToolLocationDetailsDTO);

            }
            else
            {
                binID = objBinMasterDTO.ID;

                List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetail.GetToolAllLocationsByToolGUID(RoomID, CompanyID, false, false, ToolGUID).ToList();
                ToolLocationDetailsDTO objToolLocationDetailsDTO = null;
                if (lstToolLocationDetailsDTO != null && lstToolLocationDetailsDTO.Count() > 0 && (!string.IsNullOrWhiteSpace(LocationName)))
                {
                    objToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(c => c.ToolLocationName.ToLower().Trim() == LocationName.ToLower().Trim()).FirstOrDefault();
                }
                else
                {
                    objToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(c => c.ToolLocationName == LocationName).FirstOrDefault();
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
                    objToolLocationDetailsDTO.IsArchieved = (false);
                    objToolLocationDetailsDTO.IsDefault = IsDefault;
                    objToolLocationDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolLocationDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objToolLocationDetailsDTO.WhatWhereAction = WhatWhereAction;
                    objToolLocationDetailsDTO.LocationID = objBinMasterDTO.ID;
                    objToolLocationDetail.Insert(objToolLocationDetailsDTO);

                }
            }
            if ((IsDefault ?? false) == true)
            {
                UpdateToolWithDefault(ToolGUID, binID, objBinMasterDTO.GUID);
            }
            return objBinMasterDTO;

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
                    ToolLocationDetail objToolLocationDetailDefault = context.ToolLocationDetails.Where(t => t.IsDeleted == false && t.ToolGuid == ToolGUID && t.LocationGuid == LocationGUID).FirstOrDefault();
                    if (objToolLocationDetailDefault != null)
                    {
                        objToolLocationDetailDefault.IsDefault = true;
                    }
                    context.SaveChanges();
                }
            }
        }

        public LocationMasterDTO GetLocationByNameNormal(string LocationName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LocationName", LocationName ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByNameNormal] @LocationName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public LocationMasterDTO GetLocationByNamePlain(string LocationName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LocationName", LocationName ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByNamePlain] @LocationName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public LocationMasterDTO GetLocationByGuidNormal(Guid LocationGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LocationGUID", LocationGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByGuidNormal] @LocationGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public LocationMasterDTO GetLocationByGuidPlain(Guid LocationGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LocationGUID", LocationGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByGuidPlain] @LocationGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public LocationMasterDTO GetLocationByIDNormal(long LocationID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LocationID", LocationID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByIDNormal] @LocationID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public LocationMasterDTO GetLocationByIDPlain(long LocationID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LocationID", LocationID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByIDPlain] @LocationID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<LocationMasterDTO> GetLocationByRoomNormal(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByRoomNormal] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<LocationMasterDTO> GetLocationByRoomPlain(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByRoomPlain] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<LocationMasterDTO> GetLocationListSearch(long RoomID, long CompanyID, string SearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LocationName", SearchKey ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationListSearch] @LocationName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ToolAssetQuantityDetailDTO> GetToolBinWiseSummary(Guid ToolGUID, Int64 RoomID, Int64 CompanyID, string sortColumnName, bool IsLocationRequireQty = false, string parentSearch = "")
        {
            List<ToolAssetQuantityDetailDTO> lstItemLocations = new List<ToolAssetQuantityDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@SearchTerm", parentSearch) };

                lstItemLocations = (from iqty in context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetToolLocationSummary] @RoomID,@CompanyID,@ToolGUID,@SearchTerm", params1)
                                    select iqty).ToList();
                //lstItemLocations.Sort(sortColumnName);
                return lstItemLocations;


            }
        }
        public List<LocationMasterDTO> GetPagedLocationMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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



            if ((SearchTerm ?? string.Empty).Contains("[###]"))
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    //Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    //Updated = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
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
            else
            {
                SearchTerm = "";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),

                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),

                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CompanyID", CompanyID),

                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),

                };
                List<LocationMasterDTO> lstcats = context.Database.SqlQuery<LocationMasterDTO>("exec [GetPagedLocationMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsDeleted,@IsArchived,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords.GetValueOrDefault(0);
                }

                return lstcats;
            }

        }

        public List<LocationMasterDTO> GetLocationByIDsPlain(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationByIDsPlain] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<NarrowSearchDTO> GetLocationListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetLocationListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }

        public LocationMasterDTO GetLocationBySerialPlain(string SerialNumber, Guid ToolGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@SerialNumber", SerialNumber) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationBySerialPlain] @RoomID,@CompanyID,@ToolGUID,@SerialNumber", params1).FirstOrDefault();
            }
        }
        public List<LocationMasterDTO> GetLocationMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        #endregion


        #region [Check Later on for Optimization]

        public List<LocationMasterDTO> GetAllRecordsByToolLocationLevelQuanity(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string sToolGUID)
        {
            List<LocationMasterDTO> obj = new List<LocationMasterDTO>();
            if (IsArchived == false && IsDeleted == false)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //  obj = (from u in context.Database.SqlQuery<LocationMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM BinMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString() + " AND A.ItemGUID = '" + ItemGUID + "'") //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@ToolGuid", sToolGUID) };

                    obj = (from u in context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationBYToolGUID] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@ToolGuid", params1)
                           select new LocationMasterDTO
                           {
                               ID = u.ID,
                               Location = ((u.Location ?? string.Empty).Equals("[|EmptyStagingBin|]") ? string.Empty : u.Location),
                               Created = u.Created,
                               LastUpdated = u.LastUpdated,

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
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               ToolName = u.ToolName,
                               ToolGUID = u.ToolGUID,
                               Serial = u.Serial
                           }).AsParallel().ToList();
                }
            }
            return obj.ToList();
        }

        public LocationMasterDTO GetLocationUsingGeneralCO(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGUID, Guid ToolCheckoutGUID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@ToolCheckoutGUID", ToolCheckoutGUID) };

                LocationMasterDTO obj = (from u in context.Database.SqlQuery<LocationMasterDTO>("exec [GetLocationUsingGeneralCO] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ToolGUID,@ToolCheckoutGUID", params1)
                                         select new LocationMasterDTO
                                         {
                                             ID = u.ID,
                                             Location = u.Location,
                                             Created = u.Created,
                                             LastUpdated = u.LastUpdated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             CreatedByName = u.CreatedByName,
                                             UpdatedByName = u.UpdatedByName,
                                             Room = u.Room,
                                             RoomName = u.RoomName,
                                             GUID = u.GUID,
                                             CompanyID = u.CompanyID,
                                             IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                             IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }

        }

        #endregion



    }
}