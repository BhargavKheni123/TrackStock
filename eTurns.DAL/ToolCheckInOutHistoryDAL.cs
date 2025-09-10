using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class ToolCheckInOutHistoryDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]
        public ToolCheckInOutHistoryDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolCheckInOutHistoryDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public List<ToolCheckInOutHistoryDTO> GetPagedTCIOH(int StartRowIndex, int MaxRows, string SearchTerm, string sortColumnName, string TechGUIDs, string TCIOHCreaters, string TCIOHUpdators, string CreatedDateFrom, string CreatedDateTo, string UpdatedDateFrom, string UpdatedDateTo, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, bool IsDeleted, bool IsArchived, long RoomID, long CompanyID, Guid? ToolGUID, Guid? RequisitionDetailGUID, Guid? WorkOrderGUID, Guid? ToolCheckoutGUID, string ListType, string TECUDF1, string TECUDF2, string TECUDF3, string TECUDF4, string TECUDF5)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@StartRowIndex", StartRowIndex), new SqlParameter("@MaxRows", MaxRows), new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value), new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value), new SqlParameter("@TechGUIDs", TechGUIDs ?? (object)DBNull.Value), new SqlParameter("@TCIOHCreaters", TCIOHCreaters ?? (object)DBNull.Value), new SqlParameter("@TCIOHUpdators", TCIOHUpdators ?? (object)DBNull.Value), new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? (object)DBNull.Value), new SqlParameter("@CreatedDateTo", CreatedDateTo ?? (object)DBNull.Value), new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? (object)DBNull.Value), new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? (object)DBNull.Value), new SqlParameter("@UDF1", UDF1 ?? (object)DBNull.Value), new SqlParameter("@UDF2", UDF2 ?? (object)DBNull.Value), new SqlParameter("@UDF3", UDF3 ?? (object)DBNull.Value), new SqlParameter("@UDF4", UDF4 ?? (object)DBNull.Value), new SqlParameter("@UDF5", UDF5 ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ToolGUID", ToolGUID ?? (object)DBNull.Value), new SqlParameter("@RequisitionDetailGUID", RequisitionDetailGUID ?? (object)DBNull.Value), new SqlParameter("@WorkOrderGUID", WorkOrderGUID ?? (object)DBNull.Value), new SqlParameter("@ToolCheckoutGUID", ToolCheckoutGUID ?? (object)DBNull.Value), new SqlParameter("@ListType", ListType ?? (object)DBNull.Value), new SqlParameter("@TECUDF1", TECUDF1 ?? (object)DBNull.Value), new SqlParameter("@TECUDF2", TECUDF2 ?? (object)DBNull.Value), new SqlParameter("@TECUDF3", TECUDF3 ?? (object)DBNull.Value), new SqlParameter("@TECUDF4", TECUDF4 ?? (object)DBNull.Value), new SqlParameter("@TECUDF5", TECUDF5 ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetPagedTCIOH] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@TechGUIDs,@TCIOHCreaters,@TCIOHUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsDeleted,@IsArchived,@RoomID,@CompanyID,@ToolGUID,@RequisitionDetailGUID,@WorkOrderGUID,@ToolCheckoutGUID,@ListType,@TECUDF1,@TECUDF2,@TECUDF3,@TECUDF4,@TECUDF5", params1).ToList();
            }
        }
        public ToolCheckInOutHistoryDTO GetTCIOHByGUIDFull(Guid TCIOHGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCIOHGUID", TCIOHGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHByGUIDFull] @TCIOHGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public ToolCheckInOutHistoryDTO GetTCIOHByGUIDPlain(Guid? TCIOHGUID, long? RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TCIOHGUID", TCIOHGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHByGUIDPlain] @TCIOHGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByToolGUIDWithToolInfo(Guid ToolGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByToolGUIDWithToolInfo] @ToolGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByToolGUIDAndTechnicianGuidPlain(Guid ToolGUID, long RoomID, long CompanyID, Guid TechnicianGuid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@TechnicianGuid", TechnicianGuid),
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByToolGUIDAndTechnicianGuidPlain] @ToolGUID,@RoomID,@CompanyID,@TechnicianGuid", params1).ToList();
            }
        }

        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByWOGUIDWithToolInfo(Guid WOGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByWOGUIDWithToolInfo] @WOGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<ToolCheckInOutHistoryDTO> GetPagedToolCheckouts(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGUID, Guid? RequisitionDetailGUID, Guid? WorkOrderGUID, Guid? ToolCheckoutGUID, string ListType)
        {
            List<ToolCheckInOutHistoryDTO> lstCheckouts = new List<ToolCheckInOutHistoryDTO>();
            string TCIOHCreaters = string.Empty;
            string TCIOHUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            string UDF1 = string.Empty;
            string UDF2 = string.Empty;
            string UDF3 = string.Empty;
            string UDF4 = string.Empty;
            string UDF5 = string.Empty;
            string TechUDF1 = string.Empty;
            string TechUDF2 = string.Empty;
            string TechUDF3 = string.Empty;
            string TechUDF4 = string.Empty;
            string TechUDF5 = string.Empty;
            string TechGUIDs = string.Empty;
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //ToolCheckOutUDF1 = Fields[1].Split('@')[0];
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    string[] arrReplenishTypes = FieldsPara[0].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    string[] arrReplenishTypes = FieldsPara[1].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    string[] arrReplenishTypes = FieldsPara[2].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    string[] arrReplenishTypes = FieldsPara[3].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechGUIDs = TechGUIDs + supitem + "','";
                    }
                    TechGUIDs = TechGUIDs.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechGUIDs = HttpUtility.UrlDecode(TechGUIDs);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF1 = TechUDF1 + supitem + "','";
                    }
                    TechUDF1 = TechUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF1 = HttpUtility.UrlDecode(TechUDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF2 = TechUDF2 + supitem + "','";
                    }
                    TechUDF2 = TechUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF2 = HttpUtility.UrlDecode(TechUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF3 = TechUDF3 + supitem + "','";
                    }
                    TechUDF3 = TechUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF3 = HttpUtility.UrlDecode(TechUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string[] arrReplenishTypes = FieldsPara[9].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF4 = TechUDF4 + supitem + "','";
                    }
                    TechUDF4 = TechUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF4 = HttpUtility.UrlDecode(TechUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string[] arrReplenishTypes = FieldsPara[10].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF5 = TechUDF5 + supitem + "','";
                    }
                    TechUDF5 = TechUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF5 = HttpUtility.UrlDecode(TechUDF5);
                }
            }
            TotalCount = 0;
            lstCheckouts = GetPagedTCIOH(StartRowIndex, MaxRows, SearchTerm, sortColumnName, TechGUIDs, TCIOHCreaters, TCIOHUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, ToolGUID, RequisitionDetailGUID, WorkOrderGUID, ToolCheckoutGUID, ListType, TechUDF1, TechUDF2, TechUDF3, TechUDF4, TechUDF5);
            if (lstCheckouts != null && lstCheckouts.Count > 0)
            {
                TotalCount = lstCheckouts.First().TotalRecords;
            }
            return lstCheckouts;
        }
        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByWOGUIDFull(Guid WOGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByWOGUIDFull] @WOGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByToolGUIDFull(Guid ToolGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByToolGUIDFull] @ToolGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public int GetCheckoutCount(Guid? ToolGUID, Guid? WOGUID, Guid? ReqdtlGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID ?? (object)DBNull.Value), new SqlParameter("@WOGUID", WOGUID ?? (object)DBNull.Value), new SqlParameter("@ReqdtlGUID", ReqdtlGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("exec [GetCheckoutCount] @ToolGUID,@WOGUID,@ReqdtlGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByTechGUIDWithToolInfo(Guid TeckGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@TeckGUID", TeckGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByTechGUIDWithToolInfo] @TeckGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByTechToolGUIDWithToolInfo(Guid ToolGUID, Guid TeckGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@TeckGUID", TeckGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByTechToolGUIDWithToolInfo] @ToolGUID,@TeckGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ToolCheckInOutHistoryDTO> GetTCIOHsByToolSerialWithToolInfo(Guid ToolGUID, string ToolSerial, long RoomID, long CompanyID, Guid TechnicianGuid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), 
                                               new SqlParameter("@ToolSerial", ToolSerial ?? (object)DBNull.Value), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@TechnicianGuid", TechnicianGuid)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetTCIOHsByToolSerialWithToolInfo] @ToolGUID,@ToolSerial,@RoomID,@CompanyID,@TechnicianGuid", params1).ToList();
            }
        }

        public Guid GetToolCheckinOutGUID(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<Guid>("exec [GetToolCheckinOutGUID] @ID", params1).FirstOrDefault();
            }
        }
        public Int64 Insert(ToolCheckInOutHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolCheckInOutHistory obj = new ToolCheckInOutHistory();
                obj.ID = 0;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.CheckOutStatus = objDTO.CheckOutStatus;
                obj.CheckedOutQTY = objDTO.CheckedOutQTY;
                obj.CheckedOutMQTY = objDTO.CheckedOutMQTY;
                obj.CheckOutDate = objDTO.CheckOutDate;
                obj.CheckInDate = objDTO.CheckInDate;
                obj.CheckedOutMQTYCurrent = objDTO.CheckedOutMQTYCurrent.GetValueOrDefault(0);
                obj.CheckedOutQTYCurrent = objDTO.CheckedOutQTYCurrent.GetValueOrDefault(0);
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.TechnicianGuid = objDTO.TechnicianGuid;
                obj.RequisitionDetailGuid = objDTO.RequisitionDetailGuid;
                obj.Historydate = DateTime.UtcNow;
                obj.WorkOrderGuid = objDTO.WorkOrderGuid;
                obj.ToolDetailGUID = objDTO.ToolDetailGUID;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.ToolBinID = objDTO.ToolBinID;

                context.ToolCheckInOutHistories.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return obj.ID;
            }

        }
        public bool Edit(ToolCheckInOutHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolCheckInOutHistory obj = new ToolCheckInOutHistory();
                obj.ID = objDTO.ID;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.CheckOutStatus = objDTO.CheckOutStatus;
                obj.CheckedOutQTY = objDTO.CheckedOutQTY;
                obj.CheckedOutMQTY = objDTO.CheckedOutMQTY;
                obj.CheckedOutMQTYCurrent = objDTO.CheckedOutMQTYCurrent;
                obj.CheckedOutQTYCurrent = objDTO.CheckedOutQTYCurrent;
                obj.CheckOutDate = objDTO.CheckOutDate;
                obj.CheckInDate = objDTO.CheckInDate;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = objDTO.Updated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = objDTO.GUID;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.TechnicianGuid = objDTO.TechnicianGuid;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.RequisitionDetailGuid = objDTO.RequisitionDetailGuid;
                obj.WorkOrderGuid = objDTO.WorkOrderGuid;

                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                }
                obj.ToolDetailGUID = objDTO.ToolDetailGUID;

                context.ToolCheckInOutHistories.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID, out string MSG)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                MSG = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        ToolCheckInOutHistoryDTO tempDTO = GetTCIOHByGUIDPlain(Guid.Parse(item), RoomID, CompanyID);
                        if (tempDTO != null)
                        {
                            if (tempDTO.CheckOutStatus.ToLower() == "check in")
                            {
                                strQuery += "UPDATE ToolCheckInOutHistory SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                            }
                            else if (tempDTO.CheckedOutMQTY.GetValueOrDefault(0) > 0)
                            {
                                if (tempDTO.CheckedOutMQTY.GetValueOrDefault(0) == tempDTO.CheckedOutMQTYCurrent.GetValueOrDefault(0))
                                    strQuery += "UPDATE ToolCheckInOutHistory SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                                else
                                    MSG += tempDTO.ID.ToString() + ",";
                            }
                            else if (tempDTO.CheckedOutQTY.GetValueOrDefault(0) > 0)
                            {
                                if (tempDTO.CheckedOutQTY.GetValueOrDefault(0) == tempDTO.CheckedOutQTYCurrent.GetValueOrDefault(0))
                                    strQuery += "UPDATE ToolCheckInOutHistory SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                                else
                                    MSG += tempDTO.ID.ToString() + ",";
                            }
                        }
                    }
                }
                if (strQuery != "")
                {
                    context.Database.ExecuteSqlCommand(strQuery);
                }
                return true;
            }
        }
        public List<ToolCheckInOutHistoryDTO> GetRecordByToolDetailGUID(Guid? ToolDetailGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolDetailGUID", ToolDetailGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetToolCheckInOutHistoryByToolDetailGUID] @ToolDetailGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public IEnumerable<ToolCheckInOutHistoryDTO> GetToolCheckInOutHistoryNew(Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
                return context.Database.SqlQuery<ToolCheckInOutHistoryDTO>("exec [GetToolCheckInOutHistory] @CompanyID, @RoomID", params1).ToList();

            }

        }
        public IEnumerable<ToolCheckInOutHistoryDTO> GetPagedRecordsNew(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGUID, Guid? RequisitionDetailGUID, Guid? WorkOrderGUID, Guid? ToolCheckoutGUID, string parentSearch = "")
        {
            //Get Cached-Media
            IEnumerable<ToolCheckInOutHistoryDTO> ObjCache;
            //if (ToolCheckoutGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)

            string UDF1 = string.Empty;
            string UDF2 = string.Empty;
            string UDF3 = string.Empty;
            string UDF4 = string.Empty;
            string UDF5 = string.Empty;
            string TechUDF1 = string.Empty;
            string TechUDF2 = string.Empty;
            string TechUDF3 = string.Empty;
            string TechUDF4 = string.Empty;
            string TechUDF5 = string.Empty;
            string TechGUIDs = string.Empty;
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //ToolCheckOutUDF1 = Fields[1].Split('@')[0];
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    string[] arrReplenishTypes = FieldsPara[0].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    string[] arrReplenishTypes = FieldsPara[1].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    string[] arrReplenishTypes = FieldsPara[2].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    string[] arrReplenishTypes = FieldsPara[3].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechGUIDs = TechGUIDs + supitem + "','";
                    }
                    TechGUIDs = TechGUIDs.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechGUIDs = HttpUtility.UrlDecode(TechGUIDs);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF1 = TechUDF1 + supitem + "','";
                    }
                    TechUDF1 = TechUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF1 = HttpUtility.UrlDecode(TechUDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF2 = TechUDF2 + supitem + "','";
                    }
                    TechUDF2 = TechUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF2 = HttpUtility.UrlDecode(TechUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF3 = TechUDF3 + supitem + "','";
                    }
                    TechUDF3 = TechUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF3 = HttpUtility.UrlDecode(TechUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string[] arrReplenishTypes = FieldsPara[9].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF4 = TechUDF4 + supitem + "','";
                    }
                    TechUDF4 = TechUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF4 = HttpUtility.UrlDecode(TechUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string[] arrReplenishTypes = FieldsPara[10].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF5 = TechUDF5 + supitem + "','";
                    }
                    TechUDF5 = TechUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF5 = HttpUtility.UrlDecode(TechUDF5);
                }
            }

            ObjCache = GetCheckOutDataUsingGUID(RoomID, CompanyID, ToolGUID, ToolCheckoutGUID.GetValueOrDefault(Guid.Empty), WorkOrderGUID.GetValueOrDefault(Guid.Empty), RequisitionDetailGUID.GetValueOrDefault(Guid.Empty), false, TechGUIDs, UDF1, UDF2, UDF3, UDF4, UDF5, TechUDF1, TechUDF2, TechUDF3, TechUDF4, TechUDF5, parentSearch);
            //else if (WorkOrderGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //    ObjCache = GetCachedDataNew(RoomID, CompanyID).Where(x => x.ToolGUID == ToolGUID).Where(x => x.WorkOrderGuid.GetValueOrDefault(Guid.Empty) == WorkOrderGUID.GetValueOrDefault(Guid.Empty));
            //else if (RequisitionDetailGUID != null && RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //    ObjCache = GetCachedDataNew(RoomID, CompanyID).Where(x => x.ToolGUID == ToolGUID).Where(x => x.RequisitionDetailGuid.GetValueOrDefault(Guid.Empty) == RequisitionDetailGUID.GetValueOrDefault(Guid.Empty));
            //else
            //    ObjCache = GetCachedDataNew(RoomID, CompanyID).Where(x => x.ToolGUID == ToolGUID);

            //if (ObjCache != null && ObjCache.Any())
            //{
            //    ObjCache = ObjCache.Where(x => (x.CheckedOutQTY.GetValueOrDefault(0) != 0 && (x.CheckedOutQTY.GetValueOrDefault(0) - x.CheckedOutQTYCurrent.GetValueOrDefault(0)) > 0)
            //                                     || (x.CheckedOutMQTY.GetValueOrDefault(0) != 0 && (x.CheckedOutMQTY.GetValueOrDefault(0) - x.CheckedOutMQTYCurrent.GetValueOrDefault(0)) > 0)
            //                                     );

            //}



            IEnumerable<ToolCheckInOutHistoryDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

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
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);


                ObjCache = ObjCache.Where(t =>
                  ((Fields[1].Split('@')[0] == "") || (UDF1.Contains(t.UDF1)))
                 && ((Fields[1].Split('@')[1] == "") || (UDF2.Contains(t.UDF2)))
                 && ((Fields[1].Split('@')[2] == "") || (UDF3.Contains(t.UDF3)))
                 && ((Fields[1].Split('@')[3] == "") || (UDF4.Contains(t.UDF4)))
                 && ((Fields[1].Split('@')[4] == "") || (UDF5.Contains(t.UDF5)))

                 && ((Fields[1].Split('@')[5] == "") || (TechGUIDs.Contains((t.TechnicianGuid ?? Guid.Empty).ToString().ToUpperInvariant())))
                 );
                /*/////////ONLY REQUIRED FILTER FOR SUBGRID WI-3525/////////*/

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                TotalCount = ObjCache.Where
                  (
                      t => t.ID.ToString().Contains(SearchTerm) ||
                      (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                      (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                      (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                      (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                      (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                  ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public IEnumerable<ToolCheckInOutHistoryDTO> GetCheckOutDataUsingGUID(Int64 RoomID, Int64 CompanyID, Guid? ToolGUID, Guid? ToolCheckoutGUID, Guid? WorkOrderGuid, Guid? RequisitionDetailGUID, bool? IsHistory, string TechGUIDs, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string TECUDF1, string TECUDF2, string TECUDF3, string TECUDF4, string TECUDF5, string parentSearch = "")
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                              { new SqlParameter("@CompanyID", CompanyID )
                                    , new SqlParameter("@RoomID", RoomID )
                              ,new SqlParameter("@ToolGUID", ToolGUID ??Guid.Empty)
                                    , new SqlParameter("@ToolCheckoutGUID", ToolCheckoutGUID ??Guid.Empty )
                             , new SqlParameter("@WorkOrderGuid", WorkOrderGuid ??Guid.Empty )
                                    , new SqlParameter("@RequisitionDetailGUID", RequisitionDetailGUID ??Guid.Empty )
                              , new SqlParameter("@IsHistory", IsHistory ??false )
                              , new SqlParameter("@SearchTerm", parentSearch ?? string.Empty )
                              , new SqlParameter("@TechGUIDs", TechGUIDs ?? (object)DBNull.Value)
                                , new SqlParameter("@UDF1", UDF1 ?? (object)DBNull.Value)
                                , new SqlParameter("@UDF2", UDF2 ?? (object)DBNull.Value)
                                , new SqlParameter("@UDF3", UDF3 ?? (object)DBNull.Value)
                                , new SqlParameter("@UDF4", UDF4 ?? (object)DBNull.Value)
                                , new SqlParameter("@UDF5", UDF5 ?? (object)DBNull.Value)
                                , new SqlParameter("@TECHUDF1", TECUDF1 ?? (object)DBNull.Value)
                                , new SqlParameter("@TECHUDF2", TECUDF2 ?? (object)DBNull.Value)
                                , new SqlParameter("@TECHUDF3", TECUDF3 ?? (object)DBNull.Value)
                                , new SqlParameter("@TECHUDF4", TECUDF4 ?? (object)DBNull.Value)
                                , new SqlParameter("@TECHUDF5", TECUDF5 ?? (object)DBNull.Value)};
                IEnumerable<ToolCheckInOutHistoryDTO> obj = (from u in context.Database.SqlQuery<ToolCheckInOutHistoryDTO>(@"EXEC GetCheckOutDataUsingGUID @CompanyId,@RoomId,@ToolGUID,@ToolCheckoutGUID,@WorkOrderGuid,@RequisitionDetailGUID,@IsHistory,@SearchTerm,@TechGUIDs,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@TECHUDF1,@TECHUDF2,@TECHUDF3,@TECHUDF4,@TECHUDF5", params1)
                                                             select new ToolCheckInOutHistoryDTO
                                                             {
                                                                 ID = u.ID,
                                                                 GUID = u.GUID,
                                                                 ToolGUID = u.ToolGUID,
                                                                 CheckOutStatus = u.CheckOutStatus,
                                                                 CheckedOutQTY = u.CheckedOutQTY,
                                                                 CheckedOutMQTY = u.CheckedOutMQTY,
                                                                 CheckedOutMQTYCurrent = u.CheckedOutMQTYCurrent,
                                                                 CheckedOutQTYCurrent = u.CheckedOutQTYCurrent,
                                                                 CheckOutDate = u.CheckOutDate,
                                                                 CheckInDate = u.CheckInDate,
                                                                 Created = u.Created,
                                                                 CreatedBy = u.CreatedBy,
                                                                 Updated = u.Updated,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 Room = u.Room,
                                                                 IsArchived = u.IsArchived,
                                                                 IsDeleted = u.IsDeleted,
                                                                 CompanyID = u.CompanyID,
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                                 IsGroupOfItems = u.IsGroupOfItems,
                                                                 Technician = u.Technician,
                                                                 TechnicianGuid = u.TechnicianGuid,
                                                                 TechnicianCode = u.TechnicianCode,
                                                                 RequisitionDetailGuid = u.RequisitionDetailGuid,
                                                                 WorkOrderGuid = u.WorkOrderGuid,
                                                                 ToolDetailGUID = u.ToolDetailGUID,
                                                                 SerialNumber = u.SerialNumber,
                                                                 SerialNumberTracking = u.SerialNumberTracking,
                                                                 ToolBinID = u.ToolBinID,
                                                                 Location = u.Location
                                                             }).AsParallel().ToList();
                return obj;
            }


        }
        public IEnumerable<ToolCheckInOutHistoryDTO> GetPagedRecords_HistoryNew(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGUID, Guid? RequisitionDetailGUID, Guid? WorkOrderGUID, Guid? ToolCheckoutGUID)
        {
            //Get Cached-Media
            IEnumerable<ToolCheckInOutHistoryDTO> ObjCache;

            string UDF1 = string.Empty;
            string UDF2 = string.Empty;
            string UDF3 = string.Empty;
            string UDF4 = string.Empty;
            string UDF5 = string.Empty;
            string TechUDF1 = string.Empty;
            string TechUDF2 = string.Empty;
            string TechUDF3 = string.Empty;
            string TechUDF4 = string.Empty;
            string TechUDF5 = string.Empty;
            string TechGUIDs = string.Empty;
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //ToolCheckOutUDF1 = Fields[1].Split('@')[0];
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    string[] arrReplenishTypes = FieldsPara[0].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    string[] arrReplenishTypes = FieldsPara[1].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    string[] arrReplenishTypes = FieldsPara[2].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    string[] arrReplenishTypes = FieldsPara[3].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechGUIDs = TechGUIDs + supitem + "','";
                    }
                    TechGUIDs = TechGUIDs.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechGUIDs = HttpUtility.UrlDecode(TechGUIDs);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF1 = TechUDF1 + supitem + "','";
                    }
                    TechUDF1 = TechUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF1 = HttpUtility.UrlDecode(TechUDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF2 = TechUDF2 + supitem + "','";
                    }
                    TechUDF2 = TechUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF2 = HttpUtility.UrlDecode(TechUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF3 = TechUDF3 + supitem + "','";
                    }
                    TechUDF3 = TechUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF3 = HttpUtility.UrlDecode(TechUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string[] arrReplenishTypes = FieldsPara[9].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF4 = TechUDF4 + supitem + "','";
                    }
                    TechUDF4 = TechUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF4 = HttpUtility.UrlDecode(TechUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string[] arrReplenishTypes = FieldsPara[10].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechUDF5 = TechUDF5 + supitem + "','";
                    }
                    TechUDF5 = TechUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    TechUDF5 = HttpUtility.UrlDecode(TechUDF5);
                }
            }

            //    ObjCache = GetCachedDataNew(RoomID, CompanyID).Where(x => x.ToolGUID == ToolGUID && x.GUID == ToolCheckoutGUID.GetValueOrDefault(Guid.Empty));
            ObjCache = GetCheckOutDataUsingGUID(RoomID, CompanyID, ToolGUID, ToolCheckoutGUID.GetValueOrDefault(Guid.Empty), WorkOrderGUID.GetValueOrDefault(Guid.Empty), RequisitionDetailGUID.GetValueOrDefault(Guid.Empty), true, TechGUIDs, UDF1, UDF2, UDF3, UDF4, UDF5, TechUDF1, TechUDF2, TechUDF3, TechUDF4, TechUDF5);
            IEnumerable<ToolCheckInOutHistoryDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

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

            //ObjCache = ObjCache.Where(x => (x.CheckedOutQTY.GetValueOrDefault(0) != 0 && (x.CheckedOutQTY.GetValueOrDefault(0) == x.CheckedOutQTYCurrent.GetValueOrDefault(0)))
            //                                    || (x.CheckedOutMQTY.GetValueOrDefault(0) != 0 && (x.CheckedOutMQTY.GetValueOrDefault(0) == x.CheckedOutMQTYCurrent.GetValueOrDefault(0))));

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ToolCheckInOutHistoryDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);


                ObjCache = ObjCache.Where(t =>
                  ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.UDF1)))
                 && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UDF2)))
                 && ((Fields[1].Split('@')[2] == "") || (Fields[1].Split('@')[2].Split(',').ToList().Contains(t.UDF3)))
                 && ((Fields[1].Split('@')[3] == "") || (Fields[1].Split('@')[3].Split(',').ToList().Contains(t.UDF4)))
                 && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF5)))

                 && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains((t.TechnicianGuid ?? Guid.Empty).ToString().ToUpperInvariant())))
                 );
                /*/////////ONLY REQUIRED FILTER FOR SUBGRID WI-3525/////////*/

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ToolCheckInOutHistoryDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        #endregion
    }
}


