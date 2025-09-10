using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace eTurns.DAL
{
    public class ReportMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        //public ReportMasterDAL(base.DataBaseName)
        //{

        //}
        public ReportMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public ReportMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion


        #region [Class Methods]

        public List<RPT_PullMasterDTO> GetConsumePullRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate,
                                  Int64? UserID, string UserSupplierIds , bool IsRoomIdFilter = true, string QuantityType = "", int FilterDateOn = 0,
                                  bool _isRunWithReportConnection = false, string _usageType = "Consolidate")
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    compIDs = String.Join(",", CompanyIDs);
                    //for (int i = 0; i < CompanyIDs.Length; i++)
                    //{
                    //    if (compIDs.Length > 0)
                    //        compIDs += ",";

                    //    compIDs += CompanyIDs[i];
                    //}
                }

                if (RoomIDs != null)
                {
                    roomIDs = String.Join(",", RoomIDs);
                    //for (int i = 0; i < RoomIDs.Length; i++)
                    //{
                    //    if (roomIDs.Length > 0)
                    //        roomIDs += ",";

                    //    roomIDs += RoomIDs[i];
                    //}
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if ((SpName ?? string.Empty).ToLower() == "rpt_gettotalpulled")
                {
                    if (!string.IsNullOrEmpty(UserSupplierIds))
                    {
                        SqlParameter QT = new SqlParameter("@SupplierIDs", SqlDbType.NVarChar, -1);
                        QT.Value = UserSupplierIds;
                        parameters.Add(QT);
                    }
                    else
                    {
                        SqlParameter QT = new SqlParameter("@SupplierIDs", SqlDbType.NVarChar, -1);
                        QT.Value = DBNull.Value;
                        parameters.Add(QT);
                    }
                }

                if (!string.IsNullOrEmpty(QuantityType))
                {
                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = QuantityType;
                    parameters.Add(QT);
                }
                else
                {
                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = DBNull.Value;
                    parameters.Add(QT);
                }


                if (!((SpName ?? string.Empty).ToLower().Contains("rpt_cumulative_pullsummary")))
                {
                    SqlParameter FD = new SqlParameter("@FilterDateOn", SqlDbType.Int);
                    FD.Value = FilterDateOn;
                    parameters.Add(FD);
                }

                string strQuery1 = string.Empty;
                var ArrParams = parameters.ToArray();

                if ((SpName ?? string.Empty).ToLower() == "rpt_gettotalpulled")
                {
                    strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@SupplierIDs,@QuantityType";
                }
                else
                {
                    strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@QuantityType";
                }

                if (!((SpName ?? string.Empty).ToLower().Contains("rpt_cumulative_pullsummary")))
                {
                    strQuery1 += ",@FilterDateOn";
                }

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_PullMasterDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_PullMasterDTO> GetConsumePullSummaryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate,
                                  Int64? UserID, bool IsRoomIdFilter = true, string QuantityType = "", int FilterDateOn = 0,
                                  bool _isRunWithReportConnection = false, string _usageType = "Consolidate", string[] arrItemIsActive = null
                                  ,bool IsAllowedZeroPullUsage = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {
                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);

                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }
                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(QuantityType))
                {

                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = QuantityType;
                    parameters.Add(QT);
                }
                else
                {
                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = DBNull.Value;
                    parameters.Add(QT);
                }
                if (!((SpName ?? string.Empty).ToLower().Contains("rpt_cumulative_pullsummary")))
                {
                    SqlParameter FD = new SqlParameter("@FilterDateOn", SqlDbType.Int);
                    FD.Value = FilterDateOn;
                    parameters.Add(FD);
                }
                if (!string.IsNullOrEmpty(itemIsActive))
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = itemIsActive;
                    parameters.Add(IAC);
                }
                else
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = DBNull.Value;
                    parameters.Add(IAC);
                }

                if ((SpName ?? string.Empty).ToLower().Contains("rpt_getpullsummarybyquarter"))
                {
                    SqlParameter ZU = new SqlParameter("@IsAllowedZeroPullUsage", SqlDbType.Bit);
                    ZU.Value = IsAllowedZeroPullUsage;
                    parameters.Add(ZU);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@QuantityType,@FilterDateOn,@ItemIsActive";

                if ((SpName ?? string.Empty).ToLower().Contains("rpt_getpullsummarybyquarter"))
                {
                    strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@QuantityType,@FilterDateOn,@ItemIsActive,@IsAllowedZeroPullUsage";
                }

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_PullMasterDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_ItemAuditTrailDTO> GetItemAuditTrailRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }



                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_ItemAuditTrailDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_ItemAuditTrailDTO> GetAuditTrailRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }



                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_ItemAuditTrailDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_InventoryDailyHistory> GetATTSummaryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }



                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_InventoryDailyHistory>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RRT_InstockByBinDTO> GetInStockByBinRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, Int64? UserID, string QOHFilters, string OnlyExirationItems, string[] arrItemIsActive, string QuantityType ="", bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }


                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(QOHFilters))
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = QOHFilters;
                    parameters.Add(QOH);
                }
                else
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = DBNull.Value;
                    parameters.Add(QOH);
                }

                if (!string.IsNullOrEmpty(OnlyExirationItems))
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = OnlyExirationItems;
                    parameters.Add(OEX);
                }
                else
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = DBNull.Value;
                    parameters.Add(OEX);
                }


                if (!string.IsNullOrEmpty(itemIsActive))
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = itemIsActive;
                    parameters.Add(IAC);
                }
                else
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = DBNull.Value;
                    parameters.Add(IAC);
                }
                
                if (SpName.ToLower() == "rpt_instockbybinreport")
                {
                    if (!string.IsNullOrEmpty(QuantityType) && QuantityType != ",")
                    {
                        SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                        QT.Value = QuantityType;
                        parameters.Add(QT);
                    }
                    else
                    {
                        SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                        QT.Value = DBNull.Value;
                        parameters.Add(QT);
                    }
                    var ArrParams = parameters.ToArray();
                    string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID,@QOHFilter,@OnlyExirationItems,@ItemIsActive,@QuantityType";
                    context.Database.CommandTimeout = 3600;
                    return context.Database.SqlQuery<RRT_InstockByBinDTO>(strQuery1, ArrParams).ToList();
                }
                else
                {
                    var ArrParams = parameters.ToArray();
                    string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID,@QOHFilter,@OnlyExirationItems,@ItemIsActive";
                    context.Database.CommandTimeout = 3600;
                    return context.Database.SqlQuery<RRT_InstockByBinDTO>(strQuery1, ArrParams).ToList();
                }
            }
        }

        public List<RRT_InStockForSerialLotDateCodeDTO> GetInStockForSerialLotDateCodeRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, Int64? UserID, string QOHFilters, string OnlyExirationItems, string[] arrItemIsActive, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false,string ItemTypeFilter = null)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }


                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(QOHFilters))
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = QOHFilters;
                    parameters.Add(QOH);
                }
                else
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = DBNull.Value;
                    parameters.Add(QOH);
                }

                if (!string.IsNullOrEmpty(OnlyExirationItems))
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = OnlyExirationItems;
                    parameters.Add(OEX);
                }
                else
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = DBNull.Value;
                    parameters.Add(OEX);
                }


                if (!string.IsNullOrEmpty(itemIsActive))
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = itemIsActive;
                    parameters.Add(IAC);
                }
                else
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = DBNull.Value;
                    parameters.Add(IAC);
                }

                if (!string.IsNullOrEmpty(ItemTypeFilter))
                {
                    SqlParameter QItemTypeFilter = new SqlParameter("@ItemTypeFilter", SqlDbType.VarChar, 1000);
                    QItemTypeFilter.Value = ItemTypeFilter;
                    parameters.Add(QItemTypeFilter);
                }
                else
                {
                    SqlParameter QItemTypeFilter = new SqlParameter("@ItemTypeFilter", SqlDbType.VarChar, 1000);
                    QItemTypeFilter.Value = DBNull.Value;
                    parameters.Add(QItemTypeFilter);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID,@QOHFilter,@OnlyExirationItems,@ItemIsActive,@ItemTypeFilter";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RRT_InStockForSerialLotDateCodeDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RRT_InstockByBinDTO> GetInStockByActivityRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, Int64? UserID, string QOHFilters, string OnlyExirationItems, string[] arrItemIsActive, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }


                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(QOHFilters))
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = QOHFilters;
                    parameters.Add(QOH);
                }
                else
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = DBNull.Value;
                    parameters.Add(QOH);
                }

                if (!string.IsNullOrEmpty(OnlyExirationItems))
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = OnlyExirationItems;
                    parameters.Add(OEX);
                }
                else
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = DBNull.Value;
                    parameters.Add(OEX);
                }


                if (!string.IsNullOrEmpty(itemIsActive))
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = itemIsActive;
                    parameters.Add(IAC);
                }
                else
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = DBNull.Value;
                    parameters.Add(IAC);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID,@QOHFilter,@OnlyExirationItems,@ItemIsActive";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RRT_InstockByBinDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RRT_InstockByBinDTO> GetInStockWithQOHRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, Int64? UserID, string QOHFilters, string[] arrItemIsActive, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }


                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(QOHFilters))
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = QOHFilters;
                    parameters.Add(QOH);
                }
                else
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = DBNull.Value;
                    parameters.Add(QOH);
                }




                if (!string.IsNullOrEmpty(itemIsActive))
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = itemIsActive;
                    parameters.Add(IAC);
                }
                else
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = DBNull.Value;
                    parameters.Add(IAC);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID,@QOHFilter,@ItemIsActive";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RRT_InstockByBinDTO>(strQuery1, ArrParams).ToList();
            }
        }

        //
        public List<RPT_ReceiveDTO> GetRangeReceiveRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }



                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_ReceiveDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_ReceivableItemDTO> GetReceiveRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, Int64? UserID, string[] arrOrderStatus = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }


                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrOrderStatus != null && arrOrderStatus.Length > 0)
                {
                    string strOrdstatus = "";
                    for (int i = 0; i < arrOrderStatus.Length; i++)
                    {
                        if (strOrdstatus.Length > 0)
                            strOrdstatus += ",";

                        strOrdstatus += arrOrderStatus[i];
                    }

                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = strOrdstatus;
                    parameters.Add(OS);


                }
                else
                {
                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = DBNull.Value;
                    parameters.Add(OS);

                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID,@OrderStatus";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_ReceivableItemDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_ReceiveDTO> GetReturnItemCandidatesRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false, string Days = null)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrWhiteSpace(Days))
                {
                    SqlParameter DaysValue = new SqlParameter("@Days", SqlDbType.VarChar, 1000);
                    DaysValue.Value = Days;
                    parameters.Add(DaysValue);
                }
                else
                {
                    SqlParameter DaysValue = new SqlParameter("@Days", SqlDbType.VarChar, 1000);
                    DaysValue.Value = DBNull.Value;
                    parameters.Add(DaysValue);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@Days";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_ReceiveDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_Requistions> GetConsumeRequisitionRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrReqStatus = null, string[] arrWOStatus = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrReqStatus != null && arrReqStatus.Length > 0)
                {
                    string strReqstatus =  string.Join(",", arrReqStatus);//"";
                    SqlParameter RS = new SqlParameter("@RequisitionStatus", SqlDbType.VarChar, 1000);
                    RS.Value = strReqstatus;
                    parameters.Add(RS);
                }
                else
                {
                    SqlParameter RS = new SqlParameter("@RequisitionStatus", SqlDbType.VarChar, 1000);
                    RS.Value = DBNull.Value;
                    parameters.Add(RS);

                }

                if (arrWOStatus != null && arrWOStatus.Length > 0)
                {
                    string strWOstatus = string.Join(",", arrWOStatus);//"";
                    SqlParameter WOS = new SqlParameter("@WOStatus", SqlDbType.VarChar, 1000);
                    WOS.Value = strWOstatus;
                    parameters.Add(WOS);
                }
                else
                {
                    SqlParameter WOS = new SqlParameter("@WOStatus", SqlDbType.VarChar, 1000);
                    WOS.Value = DBNull.Value;
                    parameters.Add(WOS);

                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@RequisitionStatus,@WOStatus";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_Requistions>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_RequistionWithItemDTO> GetRangeConsumeRequisitionRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrReqStatus = null, string[] arrWOStatus = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }

                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrReqStatus != null && arrReqStatus.Length > 0)
                {
                    string strReqstatus = string.Join(",", arrReqStatus);//"";
                    SqlParameter RS = new SqlParameter("@RequisitionStatus", SqlDbType.VarChar, 1000);
                    RS.Value = strReqstatus;
                    parameters.Add(RS);
                }
                else
                {
                    SqlParameter RS = new SqlParameter("@RequisitionStatus", SqlDbType.VarChar, 1000);
                    RS.Value = DBNull.Value;
                    parameters.Add(RS);

                }

                if (arrWOStatus != null && arrWOStatus.Length > 0)
                {
                    string strWOstatus = string.Join(",", arrWOStatus);//"";
                    SqlParameter WOS = new SqlParameter("@WOStatus", SqlDbType.VarChar, 1000);
                    WOS.Value = strWOstatus;
                    parameters.Add(WOS);
                }
                else
                {
                    SqlParameter WOS = new SqlParameter("@WOStatus", SqlDbType.VarChar, 1000);
                    WOS.Value = DBNull.Value;
                    parameters.Add(WOS);

                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@RequisitionStatus,@WOStatus";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_RequistionWithItemDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_RequistionWithItemDTO> GetReqItemSummaryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();
                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);
                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);
                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }

                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";
                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_RequistionWithItemDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_PullMasterDTO> GetWOPullSummaryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate,
                          Int64? UserID, bool IsRoomIdFilter = true, string QuantityType = "", bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {
                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();
                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);
                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);
                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }

                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(QuantityType))
                {
                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = QuantityType;
                    parameters.Add(QT);
                }
                else
                {
                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = DBNull.Value;
                    parameters.Add(QT);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@QuantityType";
                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_PullMasterDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_WorkOrder> GetWorkorderListRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {
                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();
                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);
                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";
                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_WorkOrder>(strQuery1, ArrParams).ToList();
            }
        }
        public List<RPT_WorkOrder> GetWorkorderRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrstrStatusType, string QuantityType = "", string WOType = "", bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "WOName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                    //for (int i = 0; i < CompanyIDs.Length; i++)
                    //{
                    //    if (compIDs.Length > 0)
                    //        compIDs += ",";

                    //    compIDs += CompanyIDs[i];
                    //}
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                    //for (int i = 0; i < RoomIDs.Length; i++)
                    //{
                    //    if (roomIDs.Length > 0)
                    //        roomIDs += ",";

                    //    roomIDs += RoomIDs[i];
                    //}
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }
                if (!string.IsNullOrEmpty(QuantityType))
                {
                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = QuantityType;
                    parameters.Add(QT);
                }
                else
                {
                    SqlParameter QT = new SqlParameter("@QuantityType", SqlDbType.VarChar, 1000);
                    QT.Value = DBNull.Value;
                    parameters.Add(QT);
                }
                if (arrstrStatusType != null && arrstrStatusType.Length > 0)
                {
                    //string strWOrdstatus = "";
                    string strWOrdstatus = string.Join(",", arrstrStatusType);//"";
                    //for (int i = 0; i < arrstrStatusType.Length; i++)
                    //{
                    //    if (strWOrdstatus.Length > 0)
                    //        strWOrdstatus += ",";

                    //    strWOrdstatus += arrstrStatusType[i];
                    //}
                    SqlParameter WOS = new SqlParameter("@WOStatus", SqlDbType.VarChar, 1000);
                    WOS.Value = strWOrdstatus;
                    parameters.Add(WOS);
                }
                else
                {
                    SqlParameter WOS = new SqlParameter("@WOStatus", SqlDbType.VarChar, 1000);
                    WOS.Value = DBNull.Value;
                    parameters.Add(WOS);
                }
                if (!string.IsNullOrEmpty(WOType))
                {
                    SqlParameter QT = new SqlParameter("@WOType", SqlDbType.VarChar, 1000);
                    QT.Value = WOType;
                    parameters.Add(QT);
                }
                else
                {
                    SqlParameter QT = new SqlParameter("@WOType", SqlDbType.VarChar, 1000);
                    QT.Value = DBNull.Value;
                    parameters.Add(QT);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@QuantityType,@WOStatus,@WOType";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_WorkOrder>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_OrderMasterDTO> GetOrderRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrOrdStatus = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    compIDs = String.Join(",", CompanyIDs);
                }

                if (RoomIDs != null)
                {
                    roomIDs = String.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrOrdStatus != null && arrOrdStatus.Length > 0)
                {

                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = string.Join(",", arrOrdStatus);
                    parameters.Add(OS);
                }
                else
                {
                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = DBNull.Value;
                    parameters.Add(OS);

                }



                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@OrderStatus";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_OrderMasterDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_QuoteMasterDTO> GetQuoteRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrQutStatus = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrQutStatus != null && arrQutStatus.Length > 0)
                {

                    SqlParameter OS = new SqlParameter("@QuoteStatus", SqlDbType.VarChar, 1000);
                    OS.Value = string.Join(",", arrQutStatus);
                    parameters.Add(OS);
                }
                else
                {
                    SqlParameter OS = new SqlParameter("@QuoteStatus", SqlDbType.VarChar, 1000);
                    OS.Value = DBNull.Value;
                    parameters.Add(OS);

                }



                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@QuoteStatus";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_QuoteMasterDTO>(strQuery1, ArrParams).ToList();
            }
        }
        public List<RPT_MoveMaterialDTO> GetMoveBinTransactionsRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string strMoveType = null, string[] arrItemIsActive = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);

                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }               

                if (arrItemIsActive  != null && arrItemIsActive.Length > 0)
                {
                    SqlParameter OS = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    OS.Value = string.Join(",", arrItemIsActive);
                    parameters.Add(OS);
                }
                else
                {
                    SqlParameter OS = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    OS.Value = DBNull.Value;
                    parameters.Add(OS);
                }
                if (!string.IsNullOrWhiteSpace(strMoveType))
                {
                    SqlParameter QF = new SqlParameter("@MoveType", strMoveType);
                    parameters.Add(QF);
                }
                else
                {
                    SqlParameter QF = new SqlParameter("@MoveType", DBNull.Value);
                    parameters.Add(QF);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@ItemIsActive,@MoveType";
                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_MoveMaterialDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_OrderWithLineItems> GetReplenishOrderRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrOrdStatus = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "OrderNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrOrdStatus != null && arrOrdStatus.Length > 0)
                {
                    string strOrdstatus = "";
                    for (int i = 0; i < arrOrdStatus.Length; i++)
                    {
                        if (strOrdstatus.Length > 0)
                            strOrdstatus += ",";

                        strOrdstatus += arrOrdStatus[i];
                    }
                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = strOrdstatus;
                    parameters.Add(OS);
                }
                else
                {
                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = DBNull.Value;
                    parameters.Add(OS);

                }



                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@OrderStatus";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_OrderWithLineItems>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_OrderSummaryWithLineItems> GetReplenishOrderSummaryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrOrdStatus = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "OrderNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrOrdStatus != null && arrOrdStatus.Length > 0)
                {
                    string strOrdstatus = "";
                    for (int i = 0; i < arrOrdStatus.Length; i++)
                    {
                        if (strOrdstatus.Length > 0)
                            strOrdstatus += ",";

                        strOrdstatus += arrOrdStatus[i];
                    }
                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = strOrdstatus;
                    parameters.Add(OS);
                }
                else
                {
                    SqlParameter OS = new SqlParameter("@OrderStatus", SqlDbType.VarChar, 1000);
                    OS.Value = DBNull.Value;
                    parameters.Add(OS);

                }
                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@OrderStatus";
                               
                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_OrderSummaryWithLineItems>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_OrderItemSummary> OrderItemSummaryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, int FilterDateOn = 0, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false,bool ExcludeZeroOrdQty= false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }
            using (var context = new eTurnsEntities(_strConnectionString))
            {
                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);

                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }
                SqlParameter FD = new SqlParameter("@FilterDateOn", SqlDbType.Int);
                FD.Value = FilterDateOn;
                parameters.Add(FD);            

                if (ExcludeZeroOrdQty != null)
                {
                    SqlParameter PExcludeZeroOrdQty = new SqlParameter("@ExcludeZeroOrdQty", SqlDbType.Bit);
                    PExcludeZeroOrdQty.Value = ExcludeZeroOrdQty;
                    parameters.Add(PExcludeZeroOrdQty);
                }
                else
                {
                    SqlParameter PExcludeZeroOrdQty = new SqlParameter("@ExcludeZeroOrdQty", SqlDbType.Bit);
                    PExcludeZeroOrdQty.Value = DBNull.Value;
                    parameters.Add(PExcludeZeroOrdQty);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@FilterDateOn,@ExcludeZeroOrdQty";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_OrderItemSummary>(strQuery1, ArrParams).ToList();
            }
        }
        //GetUnFulFilledOrderRangeData
        public List<UnFulFilledOrderMasterDTO> GetUnFulFilledOrderRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "OrderNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<UnFulFilledOrderMasterDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_SuggestedOrderDTO> GetCartSuggestedOrderRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false, string SelectedCartType = null)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(SelectedCartType))
                {
                    SqlParameter CartType = new SqlParameter("@CartType", SqlDbType.VarChar, 1000);
                    CartType.Value = SelectedCartType;
                    parameters.Add(CartType);
                }
                else
                {
                    SqlParameter CartType = new SqlParameter("@CartType", SqlDbType.VarChar, 1000);
                    CartType.Value = DBNull.Value;
                    parameters.Add(CartType);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@CartType";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_SuggestedOrderDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_Transfer> GetTransferRangeData(string SpName, string rangeFieldName, long[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string[] arrTrnsfrStatus, string[] arrTrnsfrType, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;

            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();
                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);
                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);
                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }

                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (arrTrnsfrStatus != null && arrTrnsfrStatus.Length > 0)
                {
                    SqlParameter TS = new SqlParameter("@TransferStatus", SqlDbType.VarChar, 1000);
                    TS.Value = string.Join(",", arrTrnsfrStatus);
                    parameters.Add(TS);
                }
                else
                {
                    SqlParameter TS = new SqlParameter("@TransferStatus", SqlDbType.VarChar, 1000);
                    TS.Value = DBNull.Value;
                    parameters.Add(TS);
                }

                if (arrTrnsfrType != null && arrTrnsfrType.Length > 0)
                {
                    SqlParameter TT = new SqlParameter("@TransferType", SqlDbType.VarChar, 1000);
                    TT.Value = string.Join(",", arrTrnsfrType);
                    parameters.Add(TT);
                }
                else
                {
                    SqlParameter TT = new SqlParameter("@TransferType", SqlDbType.VarChar, 1000);
                    TT.Value = DBNull.Value;
                    parameters.Add(TT);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@TransferStatus,@TransferType";
                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_Transfer>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_TransferWithLineItemDTO> GetTransferdItemsRangeData(string SpName, string rangeFieldName, long[] CompanyIDs, long[] RoomIDs, string StartDate, string EndDate, long? UserID, string[] arrTrnsfrStatus, string[] arrTrnsfrType, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null && CompanyIDs.Any() && CompanyIDs.Count() > 0)
                {
                    compIDs = string.Join(",", CompanyIDs);
                }

                if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
                {
                    roomIDs = string.Join(",", RoomIDs);
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }



                if (arrTrnsfrStatus != null && arrTrnsfrStatus.Length > 0)
                {

                    SqlParameter TS = new SqlParameter("@TransferStatus", SqlDbType.VarChar, 1000);
                    TS.Value = string.Join(",", arrTrnsfrStatus);
                    parameters.Add(TS);
                }
                else
                {
                    SqlParameter TS = new SqlParameter("@TransferStatus", SqlDbType.VarChar, 1000);
                    TS.Value = DBNull.Value;
                    parameters.Add(TS);

                }


                if (arrTrnsfrType != null && arrTrnsfrType.Length > 0)
                {

                    SqlParameter TT = new SqlParameter("@TransferType", SqlDbType.VarChar, 1000);
                    TT.Value = string.Join(",", arrTrnsfrType);
                    parameters.Add(TT);
                }
                else
                {
                    SqlParameter TT = new SqlParameter("@TransferType", SqlDbType.VarChar, 1000);
                    TT.Value = DBNull.Value;
                    parameters.Add(TT);

                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@TransferStatus,@TransferType";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_TransferWithLineItemDTO>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_ToolsCheckOut> GetCheckOutToolRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool FromListPage = false, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false,bool AllCheckedOutTools = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ToolName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue                    
                    && AllCheckedOutTools == false)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue
                    && AllCheckedOutTools == false)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                SqlParameter FL = new SqlParameter("@FromListPage", SqlDbType.Bit);
                FL.Value = FromListPage;
                parameters.Add(FL);

                if (AllCheckedOutTools)
                {
                    SqlParameter AllCheckedOut = new SqlParameter("@AllCheckedOutTools", SqlDbType.Bit);
                    AllCheckedOut.Value = AllCheckedOutTools;
                    parameters.Add(AllCheckedOut);
                }
                else
                {
                    SqlParameter AllCheckedOut = new SqlParameter("@AllCheckedOutTools", SqlDbType.Bit);
                    AllCheckedOut.Value = DBNull.Value;
                    parameters.Add(AllCheckedOut);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@FromListPage,@AllCheckedOutTools";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_ToolsCheckOut>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_Tools> GetToolsRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, string strQtyFilter, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ToolName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }


                if (!string.IsNullOrWhiteSpace(strQtyFilter))
                {
                    //SqlParameter QF = new SqlParameter("@OnlyAvailable", SqlDbType.Bit);
                    //QF.Value = strQtyFilter;
                    SqlParameter QF = new SqlParameter("@OnlyAvailable", strQtyFilter);
                    parameters.Add(QF);
                }
                else
                {
                    //SqlParameter QF = new SqlParameter("@OnlyAvailable", SqlDbType.Bit);
                    //QF.Value = DBNull.Value;
                    SqlParameter QF = new SqlParameter("@OnlyAvailable", DBNull.Value);
                    parameters.Add(QF);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@OnlyAvailable";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_Tools>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_Tools> GetToolInOutHistoryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ToolName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_Tools>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_InventoryDailyHistory> GetInventoryDailyHistoryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, Int64? EnterpriseID = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                //@EnterpriseID
                if (EnterpriseID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter EID = new SqlParameter("@EnterpriseID", SqlDbType.VarChar, 1000);
                    EID.Value = EnterpriseID;
                    parameters.Add(EID);
                }
                else
                {
                    SqlParameter EID = new SqlParameter("@EnterpriseID", SqlDbType.VarChar, 1000);
                    EID.Value = DBNull.Value;
                    parameters.Add(EID);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@EnterpriseID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_InventoryDailyHistory>(strQuery1, ArrParams).ToList();
            }
        }


        public List<RPT_InventoryDailyHistory> GetInventoryDailyHistoryDataWithDateRangeRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, Int64? EnterpriseID = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                //@EnterpriseID
                if (EnterpriseID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter EID = new SqlParameter("@EnterpriseID", SqlDbType.VarChar, 1000);
                    EID.Value = EnterpriseID;
                    parameters.Add(EID);
                }
                else
                {
                    SqlParameter EID = new SqlParameter("@EnterpriseID", SqlDbType.VarChar, 1000);
                    EID.Value = DBNull.Value;
                    parameters.Add(EID);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@EnterpriseID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_InventoryDailyHistory>(strQuery1, ArrParams).ToList();
            }
        }



        public List<RPT_InventoryDailyHistory> GetInventoryReconciliationRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, Int64? EnterpriseID = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                //@EnterpriseID
                if (EnterpriseID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter EID = new SqlParameter("@EnterpriseID", SqlDbType.VarChar, 1000);
                    EID.Value = EnterpriseID;
                    parameters.Add(EID);
                }
                else
                {
                    SqlParameter EID = new SqlParameter("@EnterpriseID", SqlDbType.VarChar, 1000);
                    EID.Value = DBNull.Value;
                    parameters.Add(EID);
                }


                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@EnterpriseID";



                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_InventoryDailyHistory>(strQuery1, ArrParams).ToList();
            }
        }

        public List<ToolsMaintenanceDTO> GetAssetMaintenanceRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "AssetName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<ToolsMaintenanceDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_KitSerialHeader> GetKitSerialRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "KitPartNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_KitSerialHeader>(strQuery1, ArrParams).ToList();
            }
        }
        public List<RPT_KitHeader> KitSummaryRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "KitPartNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }                

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_KitHeader>(strQuery1, ArrParams).ToList();
            }
        }
        public List<RPT_KitHeader> KitDetailRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "KitPartNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@UserID";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_KitHeader>(strQuery1, ArrParams).ToList();
            }
        }

        #endregion

        public List<ReportBuilderDTO> GetReportList()
        {
            List<ReportBuilderDTO> lstReportBuilderDTO = new List<ReportBuilderDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstReportBuilderDTO = (from u in context.ReportMasters
                                       where !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                       orderby u.ReportName
                                       select new ReportBuilderDTO
                                       {
                                           ID = u.ID,
                                           ReportName = u.ReportName,
                                           ReportFileName = u.ReportFileName,
                                           ReportType = u.ReportType,
                                           SubReportFileName = u.SubReportFileName,
                                           SortColumns = u.SortColumns,
                                           IsPrivate = u.IsPrivate,
                                           PrivateUserID = u.PrivateUserID,
                                           MasterReportResFile = u.MasterReportResFile,
                                           SubReportResFile = u.SubReportResFile,
                                           IsIncludeDateRange = u.IsIncludeDateRange,
                                           IsIncludeTotal = u.IsIncludeTotal,
                                           IsIncludeSubTotal = u.IsIncludeSubTotal,
                                           IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                                           IsIncludeGroup = u.IsIncludeGroup,
                                           GroupName = u.GroupName,
                                           Days = u.Days,
                                           FromDate = u.FromDate,
                                           ToDate = u.ToDate,
                                           ParentID = u.ParentID,
                                           CompanyID = u.CompanyID,
                                           RoomID = u.RoomID,
                                           ToEmailAddress = u.ToEmailAddress,
                                           ModuleName = u.ModuleName,
                                           IsBaseReport = u.IsBaseReport,
                                           ISEnterpriseReport = u.ISEnterpriseReport,
                                           IsArchived = u.IsArchived,
                                           IsDeleted = u.IsDeleted,
                                           SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                                           IsIncludeTax1 = u.IsIncludeTax1,
                                           IsIncludeTax2 = u.IsIncludeTax2,
                                           IsNotEditable = u.IsNotEditable,
                                           HideHeader = u.HideHeader,
                                           ShowSignature = u.ShowSignature,
                                           ReportAppIntent = u.ReportAppIntent,
                                          // IsSupplierRequired = u.IsSupplierRequired
                                       }
                                      ).ToList();
            }
            return lstReportBuilderDTO;
        }

        public List<ReportBuilderDTO> GetReportList(long UserID)
        {
            List<ReportBuilderDTO> lstReportBuilderDTO = new List<ReportBuilderDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var q1 = (from u in context.ReportMasters
                          where !(u.IsDeleted ?? false) && !(u.IsArchived ?? false) && u.IsPrivate == false && !(u.ISEnterpriseReport ?? false)
                          orderby u.ReportName
                          select new ReportBuilderDTO
                          {
                              ID = u.ID,
                              ReportName = u.ReportName,
                              ReportFileName = u.ReportFileName,
                              ReportType = u.ReportType,
                              SubReportFileName = u.SubReportFileName,
                              SortColumns = u.SortColumns,
                              IsPrivate = u.IsPrivate,
                              PrivateUserID = u.PrivateUserID,
                              MasterReportResFile = u.MasterReportResFile,
                              SubReportResFile = u.SubReportResFile,
                              IsIncludeDateRange = u.IsIncludeDateRange,
                              IsIncludeTotal = u.IsIncludeTotal,
                              IsIncludeSubTotal = u.IsIncludeSubTotal,
                              IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                              IsIncludeGroup = u.IsIncludeGroup,
                              GroupName = u.GroupName,
                              Days = u.Days,
                              FromDate = u.FromDate,
                              ToDate = u.ToDate,
                              ParentID = u.ParentID,
                              CompanyID = u.CompanyID,
                              RoomID = u.RoomID,
                              ToEmailAddress = u.ToEmailAddress,
                              ModuleName = u.ModuleName,
                              IsBaseReport = u.IsBaseReport,
                              ISEnterpriseReport = u.ISEnterpriseReport,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                              IsIncludeTax1 = u.IsIncludeTax1,
                              IsIncludeTax2 = u.IsIncludeTax2,
                              IsNotEditable = u.IsNotEditable,
                              HideHeader = u.HideHeader,
                              ShowSignature = u.ShowSignature,
                              ReportAppIntent = u.ReportAppIntent,
                              ResourceKey = u.ResourceKey,
                              //IsSupplierRequired = u.IsSupplierRequired
            });

                var q2 = (from u in context.ReportMasters
                          where !(u.IsDeleted ?? false) && !(u.IsArchived ?? false) && u.IsPrivate == true && u.PrivateUserID == UserID && !(u.ISEnterpriseReport ?? false)
                          orderby u.ReportName
                          select new ReportBuilderDTO
                          {
                              ID = u.ID,
                              ReportName = u.ReportName,
                              ReportFileName = u.ReportFileName,
                              ReportType = u.ReportType,
                              SubReportFileName = u.SubReportFileName,
                              SortColumns = u.SortColumns,
                              IsPrivate = u.IsPrivate,
                              PrivateUserID = u.PrivateUserID,
                              MasterReportResFile = u.MasterReportResFile,
                              SubReportResFile = u.SubReportResFile,
                              IsIncludeDateRange = u.IsIncludeDateRange,
                              IsIncludeTotal = u.IsIncludeTotal,
                              IsIncludeSubTotal = u.IsIncludeSubTotal,
                              IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                              IsIncludeGroup = u.IsIncludeGroup,
                              GroupName = u.GroupName,
                              Days = u.Days,
                              FromDate = u.FromDate,
                              ToDate = u.ToDate,
                              ParentID = u.ParentID,
                              CompanyID = u.CompanyID,
                              RoomID = u.RoomID,
                              ToEmailAddress = u.ToEmailAddress,
                              ModuleName = u.ModuleName,
                              IsBaseReport = u.IsBaseReport,
                              ISEnterpriseReport = u.ISEnterpriseReport,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                              IsIncludeTax1 = u.IsIncludeTax1,
                              IsIncludeTax2 = u.IsIncludeTax2,
                              IsNotEditable = u.IsNotEditable,
                              HideHeader = u.HideHeader,
                              ShowSignature = u.ShowSignature,
                              ReportAppIntent = u.ReportAppIntent,
                              ResourceKey = u.ResourceKey,
                              //IsSupplierRequired = u.IsSupplierRequired
                          });
                var q3 = (from u in context.ReportMasters
                          where !(u.IsDeleted ?? false) && !(u.IsArchived ?? false) && u.ISEnterpriseReport == true
                          orderby u.ReportName
                          select new ReportBuilderDTO
                          {
                              ID = u.ID,
                              ReportName = u.ReportName,
                              ReportFileName = u.ReportFileName,
                              ReportType = u.ReportType,
                              SubReportFileName = u.SubReportFileName,
                              SortColumns = u.SortColumns,
                              IsPrivate = u.IsPrivate,
                              PrivateUserID = u.PrivateUserID,
                              MasterReportResFile = u.MasterReportResFile,
                              SubReportResFile = u.SubReportResFile,
                              IsIncludeDateRange = u.IsIncludeDateRange,
                              IsIncludeTotal = u.IsIncludeTotal,
                              IsIncludeSubTotal = u.IsIncludeSubTotal,
                              IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                              IsIncludeGroup = u.IsIncludeGroup,
                              GroupName = u.GroupName,
                              Days = u.Days,
                              FromDate = u.FromDate,
                              ToDate = u.ToDate,
                              ParentID = u.ParentID,
                              CompanyID = u.CompanyID,
                              RoomID = u.RoomID,
                              ToEmailAddress = u.ToEmailAddress,
                              ModuleName = u.ModuleName,
                              IsBaseReport = u.IsBaseReport,
                              ISEnterpriseReport = u.ISEnterpriseReport,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                              IsIncludeTax1 = u.IsIncludeTax1,
                              IsIncludeTax2 = u.IsIncludeTax2,
                              IsNotEditable = u.IsNotEditable,
                              HideHeader = u.HideHeader,
                              ShowSignature = u.ShowSignature,
                              ReportAppIntent = u.ReportAppIntent,
                              ResourceKey = u.ResourceKey,
                              //IsSupplierRequired = u.IsSupplierRequired
                          });
                var res = q1.Union(q2).ToList();
                res = res.Union(q3).ToList();
                lstReportBuilderDTO = res.AsEnumerable().Distinct().ToList();
            }
            return lstReportBuilderDTO;
        }

        public List<ReportBuilderDTO> GetReportList(Int64 CompanyID, Int64? RoomID, long UserID)
        {
            List<ReportBuilderDTO> lstReportBuilderDTO = new List<ReportBuilderDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstReportBuilderDTO = (from u in context.ReportMasters
                                       where u.CompanyID == CompanyID && u.RoomID == RoomID
                                       && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                       && (u.ParentID ?? 0) == 0 && !(u.IsNotEditable ?? false)
                                       // && (u.PrivateUserID == UserID || u.PrivateUserID == null)
                                       orderby u.ReportName ascending
                                       select new ReportBuilderDTO
                                       {
                                           ID = u.ID,
                                           ReportName = u.ReportName,
                                           ReportFileName = u.ReportFileName,
                                           ReportType = u.ReportType,
                                           SubReportFileName = u.SubReportFileName,
                                           SortColumns = u.SortColumns,
                                           IsPrivate = u.IsPrivate,
                                           PrivateUserID = u.PrivateUserID,
                                           MasterReportResFile = u.MasterReportResFile,
                                           SubReportResFile = u.SubReportResFile,
                                           IsIncludeDateRange = u.IsIncludeDateRange,
                                           IsIncludeTotal = u.IsIncludeTotal,
                                           IsIncludeSubTotal = u.IsIncludeSubTotal,
                                           IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                                           IsIncludeGroup = u.IsIncludeGroup,
                                           GroupName = u.GroupName,
                                           Days = u.Days,
                                           FromDate = u.FromDate,
                                           ToDate = u.ToDate,
                                           ParentID = u.ParentID,
                                           CompanyID = u.CompanyID,
                                           RoomID = u.RoomID,
                                           ISEnterpriseReport = u.ISEnterpriseReport,
                                           IsArchived = u.IsArchived,
                                           IsDeleted = u.IsDeleted,
                                           SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                                           IsIncludeTax1 = u.IsIncludeTax1,
                                           IsIncludeTax2 = u.IsIncludeTax2,
                                           IsNotEditable = u.IsNotEditable,
                                           ModuleName = u.ModuleName,
                                           HideHeader = u.HideHeader,
                                           ShowSignature = u.ShowSignature,
                                           ReportAppIntent = u.ReportAppIntent,
                                           ResourceKey = u.ResourceKey,
                                           CombineReportID = u.CombineReportID
                                           //IsSupplierRequired = u.IsSupplierRequired
                                       }
                                      ).ToList();
            }
            return lstReportBuilderDTO;
        }
        public ReportBuilderDTO GetReportDetail(Int64 ID)
        {
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // read with nolock
                using (var txn = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                    }
                ))
                {
                    objReportBuilderDTO = (from u in context.ReportMasters
                                           where u.ID == ID
                                           select new ReportBuilderDTO
                                           {
                                               ID = u.ID,
                                               ReportName = u.ReportName,
                                               ReportFileName = u.ReportFileName,
                                               SubReportFileName = u.SubReportFileName,
                                               ReportType = u.ReportType,
                                               IsPrivate = u.IsPrivate,
                                               PrivateUserID = u.PrivateUserID,
                                               IsBaseReport = u.IsBaseReport,
                                               MasterReportResFile = u.MasterReportResFile,
                                               SubReportResFile = u.SubReportResFile,
                                               IsIncludeDateRange = u.IsIncludeDateRange,
                                               IsIncludeTotal = u.IsIncludeTotal,
                                               IsIncludeSubTotal = u.IsIncludeSubTotal,
                                               IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                                               IsIncludeGroup = u.IsIncludeGroup,
                                               GroupName = u.GroupName,
                                               Days = u.Days,
                                               FromDate = u.FromDate,
                                               ToDate = u.ToDate,
                                               ParentID = u.ParentID,
                                               ToEmailAddress = u.ToEmailAddress,
                                               ModuleName = u.ModuleName,
                                               ISEnterpriseReport = u.ISEnterpriseReport,
                                               IsArchived = u.IsArchived,
                                               IsDeleted = u.IsDeleted,
                                               SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                                               IsIncludeTax1 = u.IsIncludeTax1,
                                               IsIncludeTax2 = u.IsIncludeTax2,
                                               IsNotEditable = u.IsNotEditable,
                                               HideHeader = u.HideHeader,
                                               ShowSignature = u.ShowSignature,
                                               ReportAppIntent = u.ReportAppIntent,
                                               ResourceKey = u.ResourceKey,
                                               CombineReportID = u.CombineReportID
                                               //IsSupplierRequired = u.IsSupplierRequired
                                           }).FirstOrDefault();

                    txn.Complete();
                }
            }
            return objReportBuilderDTO;
        }
        public ReportBuilderDTO GetReportDetail(Int64 ID, string DBConnectionstring)
        {
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                objReportBuilderDTO = (from u in context.ReportMasters
                                       where u.ID == ID
                                       select new ReportBuilderDTO
                                       {
                                           ID = u.ID,
                                           ReportName = u.ReportName,
                                           ReportFileName = u.ReportFileName,
                                           SubReportFileName = u.SubReportFileName,
                                           ReportType = u.ReportType,
                                           IsPrivate = u.IsPrivate,
                                           PrivateUserID = u.PrivateUserID,
                                           IsBaseReport = u.IsBaseReport,
                                           MasterReportResFile = u.MasterReportResFile,
                                           SubReportResFile = u.SubReportResFile,
                                           IsIncludeDateRange = u.IsIncludeDateRange,
                                           IsIncludeTotal = u.IsIncludeTotal,
                                           IsIncludeSubTotal = u.IsIncludeSubTotal,
                                           IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                                           IsIncludeGroup = u.IsIncludeGroup,
                                           GroupName = u.GroupName,
                                           Days = u.Days,
                                           FromDate = u.FromDate,
                                           ToDate = u.ToDate,
                                           ParentID = u.ParentID,
                                           ToEmailAddress = u.ToEmailAddress,
                                           ModuleName = u.ModuleName,
                                           ISEnterpriseReport = u.ISEnterpriseReport,
                                           IsArchived = u.IsArchived,
                                           IsDeleted = u.IsDeleted,
                                           SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                                           IsIncludeTax1 = u.IsIncludeTax1,
                                           IsIncludeTax2 = u.IsIncludeTax2,
                                           IsNotEditable = u.IsNotEditable,
                                           HideHeader = u.HideHeader,
                                           ShowSignature = u.ShowSignature,
                                           ReportAppIntent = u.ReportAppIntent
                                       }).FirstOrDefault();

            }
            return objReportBuilderDTO;
        }

        public bool DeleteReport(long ReportID, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string ReportStringId = Convert.ToString(ReportID);
                var objNotification = (from u in context.Notifications
                                       where (u.ReportID == ReportID || u.AttachmentReportIDs == ReportStringId)
                                       select u);
                if (objNotification != null && objNotification.Count() > 0)
                {
                    foreach (var item in objNotification)
                    {
                        item.IsDeleted = true;

                        var objRoomSchedule = (from Rs in context.RoomSchedules
                                               where Rs.ScheduleID == item.RoomScheduleID
                                               select Rs);
                        if (objRoomSchedule != null && objRoomSchedule.Count() > 0)
                        {
                            foreach (var itemNew in objRoomSchedule)
                            {
                                itemNew.IsDeleted = true;
                                itemNew.LastUpdatedBy = UserId;
                                itemNew.Updated = DateTime.UtcNow;
                            }
                        }

                    }
                }




                var objGrp = (from u in context.ReportGroupMasters
                              where u.ReportID == ReportID
                              select u);
                if (objGrp != null && objGrp.Count() > 0)
                {
                    foreach (var item in objGrp)
                    {
                        item.IsDeleted = true;  
                    }
                }

                ReportMaster obj = (from u in context.ReportMasters
                                    where u.ID == ReportID
                                    select u
                                    ).FirstOrDefault();
                if (obj != null)
                {
                    obj.IsDeleted = true;
                    obj.UpdatedBy = UserId;
                    obj.UpdatedON = DateTime.UtcNow;
                }

                context.SaveChanges();
            }
            return true;
        }

        public bool EditReportMaster(ReportBuilderDTO objDTO, string ModuleName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool isDeleteNotification = false;
                ReportMaster obj = new ReportMaster();
                obj = (from u in context.ReportMasters
                       where u.ID == objDTO.ID
                       select u
                    ).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.ISEnterpriseReport.GetValueOrDefault(false) == true && objDTO.ISEnterpriseReport.GetValueOrDefault(false) == false)
                    {
                        isDeleteNotification = true;
                    }

                    if (objDTO.SetAsDefaultPrintReport ?? false)
                    {
                        IEnumerable<ReportMaster> objReports = from m in context.ReportMasters
                                                               where m.ModuleName == objDTO.ModuleName
                                                               && m.ReportType == objDTO.ReportType
                                                               && m.SetAsDefaultPrintReport == true
                                                               && m.IsBaseReport == false
                                                               && m.ID != objDTO.ID
                                                               //&& (m.CompanyID == objDTO.CompanyID || m.CompanyID == 0)
                                                               && (m.ISEnterpriseReport ?? false) == (objDTO.ISEnterpriseReport ?? false)

                                                               select m;

                        if (objReports != null && objReports.Count() > 0)
                        {
                            foreach (var item in objReports)
                            {
                                item.SetAsDefaultPrintReport = false;
                                item.UpdatedON = DateTime.UtcNow;
                                item.UpdatedBy = objDTO.LastUpdatedBy;
                            }
                            context.SaveChanges();
                        }

                    }
                    obj.SortColumns = objDTO.SortColumns;
                    if (!string.IsNullOrEmpty(objDTO.ReportName))
                    {
                        obj.ReportName = objDTO.ReportName;
                    }
                    obj.IsPrivate = objDTO.IsPrivate;
                    obj.ISEnterpriseReport = objDTO.ISEnterpriseReport;
                    if (objDTO.ISEnterpriseReport == false)
                    {
                        obj.CompanyID = objDTO.CompanyID;
                        obj.RoomID = objDTO.RoomID;
                    }
                    obj.PrivateUserID = objDTO.PrivateUserID;
                    obj.ReportFileName = objDTO.ReportFileName;
                    obj.IsIncludeTotal = objDTO.IsIncludeTotal;
                    obj.IsIncludeSubTotal = objDTO.IsIncludeSubTotal;
                    obj.IsIncludeGrandTotal = objDTO.IsIncludeGrandTotal;
                    obj.IsIncludeTax1 = objDTO.IsIncludeTax1;
                    obj.IsIncludeTax2 = objDTO.IsIncludeTax2;
                    obj.IsNotEditable = objDTO.IsNotEditable;
                    obj.ToEmailAddress = objDTO.ToEmailAddress;
                    obj.SetAsDefaultPrintReport = objDTO.SetAsDefaultPrintReport;
                    obj.ShowSignature = objDTO.ShowSignature;
                    obj.HideHeader = objDTO.HideHeader;
                    obj.UpdatedON = DateTime.UtcNow;
                    obj.UpdatedBy = objDTO.LastUpdatedBy;
                    
                    if (!string.IsNullOrEmpty(objDTO.ReportAppIntent) && !string.IsNullOrWhiteSpace(objDTO.ReportAppIntent))
                    {
                        obj.ReportAppIntent = objDTO.ReportAppIntent;
                    }
                    if(!string.IsNullOrWhiteSpace(obj.SubReportFileName) 
                        && !string.IsNullOrWhiteSpace(objDTO.SubReportFileName))
                    {
                        if(!obj.SubReportFileName.Equals(objDTO.SubReportFileName))
                        {
                            obj.SubReportFileName = objDTO.SubReportFileName;
                        }
                    }
                    context.SaveChanges();


                    if (isDeleteNotification == true)
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", objDTO.CompanyID), new SqlParameter("@RoomID", objDTO.RoomID ?? (object)DBNull.Value), new SqlParameter("@ReportID", obj.ID) };
                        context.Database.ExecuteSqlCommand("exec DeleteNotification_IsEntReportFalse @CompanyID,@RoomID,@ReportID", params1);

                        #region For Default Print option as par module WI-4440

                        UpdateDefaultReportAfterReportDeleted(obj.ID, null, null);

                        if (objDTO.SetAsDefaultPrintReport ?? false)
                        {
                            UpdateDefaultReportBasedonCustomiseReport(obj.ID, ModuleName, objDTO.CompanyID, objDTO.RoomID.GetValueOrDefault(0));
                        }

                        #endregion
                    }
                }


            }
            return true;
        }
        public long Insert(ReportBuilderDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDTO.SetAsDefaultPrintReport ?? false)
                {
                    IEnumerable<ReportMaster> objReports = from m in context.ReportMasters
                                                           where m.ModuleName == objDTO.ModuleName
                                                            && m.ReportType == objDTO.ReportType
                                                           && m.IsBaseReport == false
                                                           && (m.CompanyID == objDTO.CompanyID || m.CompanyID == 0)
                                                           && (m.ISEnterpriseReport ?? false) == (objDTO.ISEnterpriseReport ?? false)
                                                           select m;

                    if (objReports != null && objReports.Count() > 0)
                    {
                        foreach (var item in objReports)
                        {
                            item.SetAsDefaultPrintReport = false;
                            item.UpdatedBy = objDTO.LastUpdatedBy;
                            item.UpdatedON = DateTime.UtcNow;
                        }
                        context.SaveChanges();
                    }
                }
                ReportMaster obj = new ReportMaster();
                obj.ID = 0;

                obj.CompanyID = objDTO.CompanyID;
                obj.RoomID = objDTO.RoomID;
                obj.IsPrivate = objDTO.IsPrivate;
                obj.IsIncludeTotal = objDTO.IsIncludeTotal;
                obj.IsIncludeSubTotal = objDTO.IsIncludeSubTotal;
                obj.IsIncludeGrandTotal = objDTO.IsIncludeGrandTotal;
                obj.IsIncludeTax1 = objDTO.IsIncludeTax1;
                obj.IsIncludeTax2 = objDTO.IsIncludeTax2;
                obj.IsNotEditable = objDTO.IsNotEditable;
                obj.PrivateUserID = objDTO.PrivateUserID;
                obj.ReportFileName = objDTO.ReportFileName;
                obj.ReportName = objDTO.ReportName;
                obj.ReportType = objDTO.ReportType;
                obj.SortColumns = objDTO.SortColumns;
                obj.SubReportFileName = objDTO.SubReportFileName;
                obj.IsBaseReport = objDTO.IsBaseReport;
                obj.MasterReportResFile = objDTO.MasterReportResFile;
                obj.SubReportResFile = objDTO.SubReportResFile;
                obj.ParentID = objDTO.ParentID;
                obj.ToEmailAddress = objDTO.ToEmailAddress;
                obj.ModuleName = objDTO.ModuleName;
                obj.CreatedOn = DateTimeUtility.DateTimeNow;
                obj.UpdatedON = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.UpdatedBy = objDTO.LastUpdatedBy;
                obj.ISEnterpriseReport = objDTO.ISEnterpriseReport;
                obj.SetAsDefaultPrintReport = objDTO.SetAsDefaultPrintReport;
                obj.ShowSignature = objDTO.ShowSignature;
                obj.HideHeader = objDTO.HideHeader;
                
                if (!string.IsNullOrEmpty(objDTO.ReportAppIntent) && !string.IsNullOrWhiteSpace(objDTO.ReportAppIntent))
                {
                    obj.ReportAppIntent = objDTO.ReportAppIntent;
                }
                else
                {
                    obj.ReportAppIntent = "ReadOnly";
                }
                context.ReportMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                if (objDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    InsertNewCreatedReportInReportAlertConfig(objDTO);
                }

                return obj.ID;
            }
        }


        public void InsertNewCreatedReportInReportAlertConfig(ReportBuilderDTO objDTO)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", objDTO.CompanyID), new SqlParameter("@RoomID", objDTO.RoomID ?? (object)DBNull.Value), new SqlParameter("@ReportID", objDTO.ID), new SqlParameter("@ParentID", objDTO.ParentID) };
                    context.Database.ExecuteSqlCommand("exec SaveNewReportInReportAlertConfig @CompanyID,@RoomID,@ReportID,@ParentID", params1);
                }
            }
            catch
            {
            }
        }

        public bool CheckReportExist(string ReportName, long CompanyId, long ReportID = 0)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (ReportID == 0)
                {
                    return context.ReportMasters.Where(r => (r.CompanyID == 0 || r.CompanyID == CompanyId) && !(r.IsDeleted ?? false) && !(r.IsArchived ?? false)).Any(r => r.ReportName == ReportName);
                    //&& !(r.IsDeleted ?? false) Jira wi-2823
                }
                else
                {
                    return context.ReportMasters.Where(r => (r.CompanyID == 0 || r.CompanyID == CompanyId) && !(r.IsDeleted ?? false) && !(r.IsArchived ?? false) && r.ID != ReportID).Any(r => r.ReportName == ReportName);
                    //&& !(r.IsDeleted ?? false) refer wi-2823
                }
            }
        }
        public Int64 GetReportID(string ReportName)
        {
            Int64 reportid = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                reportid = (from u in context.ReportMasters
                            where u.ReportName.ToLower() == ReportName.ToLower() && u.IsBaseReport == true && u.ParentID == 0 //&& u.ReportType == 2
                            && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                            select u.ID).FirstOrDefault();
            }
            return reportid;
        }
        public ReportMaster GetReportIDNew(string ReportName)
        {
            ReportMaster reportid = new DAL.ReportMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                reportid = (from u in context.ReportMasters
                            where u.ReportName.ToLower() == ReportName.ToLower() && u.IsBaseReport == true && u.ParentID == 0 //&& u.ReportType == 2
                            && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                            select u).FirstOrDefault();
            }
            return reportid;
        }
        public ReportMaster GetDefaultReportIDNew(string ReportName, Int64 RoomId, Int64 CompanyID)
        {
            //Int64 reportid = 0;
            ReportMaster reportid = new DAL.ReportMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                reportid = (from u in context.ReportMasters
                            where u.ModuleName.ToLower() == ReportName.ToLower()
                            && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                             && u.CompanyID == CompanyID
                            && u.SetAsDefaultPrintReport == true
                            && (u.ISEnterpriseReport ?? false) == false
                            select u).FirstOrDefault();

                if (reportid == null || reportid.ID <= 0)
                {
                    reportid = (from u in context.ReportMasters
                                where u.ModuleName.ToLower() == ReportName.ToLower()
                                && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                && (u.ISEnterpriseReport ?? false) == true
                                && u.SetAsDefaultPrintReport == true
                                select u).FirstOrDefault();
                }

            }
            return reportid;
        }
        public Int64 GetDefaultReportID(string ReportName, Int64 RoomId, Int64 CompanyID)
        {
            Int64 reportid = 0;
            //ReportMaster reportid = new DAL.ReportMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                reportid = (from u in context.ReportMasters
                            where u.ModuleName.ToLower() == ReportName.ToLower()
                            && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                             && u.CompanyID == CompanyID
                            && u.SetAsDefaultPrintReport == true
                            && (u.ISEnterpriseReport ?? false) == false
                            select u.ID).FirstOrDefault();

                if (reportid <= 0)
                {
                    reportid = (from u in context.ReportMasters
                                where u.ModuleName.ToLower() == ReportName.ToLower()
                                && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                && (u.ISEnterpriseReport ?? false) == true
                                && u.SetAsDefaultPrintReport == true
                                select u.ID).FirstOrDefault();
                }

            }
            return reportid;
        }

        public string GetReportSortFields(string ReportName)
        {
            string reportSortColumns = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                reportSortColumns = (from u in context.ReportMasters
                                     where u.ReportName.ToLower() == ReportName.ToLower() && u.IsBaseReport == true && u.ParentID == 0 //&& u.ReportType == 2
                                     && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                     select u.SortColumns).FirstOrDefault();
            }
            return reportSortColumns;
        }

        public List<ReportBuilderDTO> GetChildReport(long ParentID, long CompanyID, long PrivateUserID)
        {
            List<ReportBuilderDTO> lstReportBuilderDTO = new List<ReportBuilderDTO>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsReportlist = new DataSet();

            dsReportlist = SqlHelper.ExecuteDataset(EturnsConnection, "RPT_GetChildReports", ParentID, CompanyID, PrivateUserID);
            if (dsReportlist != null && dsReportlist.Tables[0].Rows.Count > 0)
            {
                DataTable dt = new DataTable();
                dt = dsReportlist.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    ReportBuilderDTO objDTO = new ReportBuilderDTO();
                    objDTO.ID = Convert.ToInt64(dr["id"]);
                    objDTO.ReportName = Convert.ToString(dr["reportname"]);
                    objDTO.ParentID = Convert.ToInt64(dr["parentid"]);
                    lstReportBuilderDTO.Add(objDTO);
                }
            }
            return lstReportBuilderDTO;
        }
        public List<ReportGroupMasterDTO> GetreportGroupFieldList(Int64 ReportId)
        {
            List<ReportGroupMasterDTO> lstReportGroupMasterDTO = new List<ReportGroupMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstReportGroupMasterDTO = (from u in context.ReportGroupMasters
                                           where u.ReportID == ReportId
                                           && !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                           select new ReportGroupMasterDTO
                                           {
                                               Id = u.Id,
                                               ReportID = u.ReportID,
                                               FieldName = u.FieldName,
                                               GroupOrder = u.GroupOrder,
                                               IsArchived = u.IsArchived,
                                               IsDeleted = u.IsDeleted,
                                               FieldColumnID = u.FieldColumnID

                                           }
                                      ).ToList();
            }
            return lstReportGroupMasterDTO;
        }
        public List<ReportScheduleDTO> getSchedule(long RoomId, long CompanyID, string DBConnectionstring)
        {
            List<ReportScheduleDTO> lstRoomSchedule = new List<ReportScheduleDTO>();
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                lstRoomSchedule = (from u in context.RoomSchedules
                                   where u.CompanyId == CompanyID && u.RoomId == RoomId && (u.ReportID ?? 0) > 0 && u.ScheduleFor == 5
                                   select new ReportScheduleDTO
                                   {
                                       RoomId = u.RoomId ?? 0,
                                       CompanyId = u.CompanyId ?? 0,
                                       ScheduleID = u.ScheduleID,
                                       ReportId = u.ReportID ?? 0,
                                       IsScheduleActive = u.IsScheduleActive,
                                       NextRundate = u.NextRunDate,
                                       ReportDataSelectionType = u.ReportDataSelectionType,
                                       ReportDataSince = u.ReportDataSince
                                   }).ToList();
            }


            return lstRoomSchedule;
        }
        public List<ReportScheduleDTO> getSchedule(long RoomId, long CompanyID, string DBConnectionstring, DateTime fromDate, DateTime toDate)
        {
            List<ReportScheduleDTO> lstRoomSchedule = new List<ReportScheduleDTO>();
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                lstRoomSchedule = (from u in context.RoomSchedules
                                   where u.CompanyId == CompanyID && u.RoomId == RoomId && (u.ReportID ?? 0) > 0 && u.ScheduleFor == 5 && (u.NextRunDate >= fromDate && u.NextRunDate <= toDate)
                                   select new ReportScheduleDTO
                                   {
                                       RoomId = u.RoomId ?? 0,
                                       CompanyId = u.CompanyId ?? 0,
                                       ScheduleID = u.ScheduleID,
                                       ReportId = u.ReportID ?? 0,
                                       IsScheduleActive = u.IsScheduleActive,
                                       NextRundate = u.NextRunDate,
                                       ReportDataSelectionType = u.ReportDataSelectionType,
                                       ReportDataSince = u.ReportDataSince
                                   }).ToList();
            }


            return lstRoomSchedule;
        }
        public ReportMailLogDTO GetReportMaillog(long ScheduleID, string DBConnectionstring)
        {
            ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                objReportMailLogDTO = (from u in context.ReportMailLogs
                                       where u.ScheduleID == ScheduleID
                                       select new ReportMailLogDTO
                                       {
                                           Id = u.Id,
                                           RoomID = u.RoomID,
                                           CompanyID = u.CompanyID,
                                           ScheduleID = u.ScheduleID,
                                           ReportID = u.ReportID,
                                           SendEmailAddress = u.SendEmailAddress,
                                           SendDate = u.SendDate,
                                           AttachmentCount = u.AttachmentCount,
                                       }).OrderByDescending(x => x.SendDate).FirstOrDefault();
            }


            return objReportMailLogDTO;
        }
        public ReportMailLogDTO GetReportMaillog(long NotificationID)
        {
            ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objReportMailLogDTO = (from u in context.ReportMailLogs
                                       where u.NotificationID == NotificationID
                                       select new ReportMailLogDTO
                                       {
                                           Id = u.Id,
                                           RoomID = u.RoomID,
                                           CompanyID = u.CompanyID,
                                           ScheduleID = u.ScheduleID,
                                           ReportID = u.ReportID,
                                           SendEmailAddress = u.SendEmailAddress,
                                           SendDate = u.SendDate,
                                           AttachmentCount = u.AttachmentCount,
                                           NotificationID = u.NotificationID
                                       }).OrderByDescending(x => x.SendDate).FirstOrDefault();
            }


            return objReportMailLogDTO;
        }
        public ReportMailLogDTO GetReportMaillog(long NotificationID, bool ExecuitAsPastSchedule, DateTime? NextRunDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@NotificationID", NotificationID), new SqlParameter("@ExecuitAsPastSchedule", ExecuitAsPastSchedule), new SqlParameter("@nextRunDate", NextRunDate ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReportMailLogDTO>("exec [GetReportMailLogByNotiID] @NotificationID,@ExecuitAsPastSchedule,@nextRunDate", params1).FirstOrDefault();
            }
        }

        public bool InsertMailLog(ReportMailLogDTO objReportMailLogDTO, string DbConnectionString)
        {

            using (var context = new eTurnsEntities(DbConnectionString))
            {
                ReportMailLog objReportMailLog = new ReportMailLog();
                objReportMailLog.Id = 0;
                objReportMailLog.CompanyID = objReportMailLogDTO.CompanyID;
                objReportMailLog.RoomID = objReportMailLogDTO.RoomID;
                objReportMailLog.ReportID = objReportMailLogDTO.ReportID;
                objReportMailLog.ScheduleID = objReportMailLogDTO.ScheduleID;
                objReportMailLog.AttachmentCount = objReportMailLogDTO.AttachmentCount;
                objReportMailLog.SendEmailAddress = objReportMailLogDTO.SendEmailAddress;
                objReportMailLog.SendDate = objReportMailLogDTO.SendDate;
                objReportMailLog.NotificationID = objReportMailLogDTO.NotificationID;
                objReportMailLog.Created = DateTime.UtcNow;
                context.ReportMailLogs.Add(objReportMailLog);
                context.SaveChanges();
            }


            return true;
        }
        public bool InsertMailLog(ReportMailLogDTO objReportMailLogDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReportMailLog objReportMailLog = new ReportMailLog();
                objReportMailLog.Id = 0;
                objReportMailLog.CompanyID = objReportMailLogDTO.CompanyID;
                objReportMailLog.RoomID = objReportMailLogDTO.RoomID;
                objReportMailLog.ReportID = objReportMailLogDTO.ReportID;
                objReportMailLog.ScheduleID = objReportMailLogDTO.ScheduleID;
                objReportMailLog.AttachmentCount = objReportMailLogDTO.AttachmentCount;
                objReportMailLog.SendEmailAddress = objReportMailLogDTO.SendEmailAddress;
                objReportMailLog.SendDate = objReportMailLogDTO.SendDate;
                objReportMailLog.NotificationID = objReportMailLogDTO.NotificationID;
                objReportMailLog.Created = DateTime.UtcNow;
                context.ReportMailLogs.Add(objReportMailLog);
                context.SaveChanges();
            }


            return true;
        }
        public bool UpdateNextRunDate(long ScheduleID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

            }
            return true;
        }
        public bool UpdateNextRunDate(long ScheduleID, string DBConnectionstring)
        {

            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                string qry = "EXEC RPT_GetNextReportRunTime_nd " + ScheduleID;
                context.Database.ExecuteSqlCommand(qry);
            }
            return true;
        }

        public List<RPT_DicrepencyItem> GetDecripencyReportData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, bool @IsFromAlert)
        {
            string startdate = null, enddate = null, roomids = null, companyids = null, dataguids = null, ids = null, sortfields = null, userid = null;
            bool isdeleted = false, isarchived = false;
            if (arrCompanyid != null && arrCompanyid.Count() > 0)
            {
                companyids = string.Join(",", arrCompanyid);
            }
            if (arrRoomid != null && arrRoomid.Count() > 0)
            {
                roomids = string.Join(",", arrRoomid);
            }
            if (!string.IsNullOrEmpty(startDate) && Convert.ToDateTime(startDate) != DateTime.MinValue)
            {
                startdate = startDate;
            }
            if (!string.IsNullOrEmpty(endDate) && Convert.ToDateTime(endDate) != DateTime.MinValue)
            {
                enddate = endDate;
            }
            List<RPT_DicrepencyItem> lstItems = new List<RPT_DicrepencyItem>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@StartDate", startdate ?? (object)DBNull.Value), new SqlParameter("@EndDate", enddate ?? (object)DBNull.Value), new SqlParameter("@RoomIDs", roomids ?? (object)DBNull.Value), new SqlParameter("@CompanyIDs", companyids ?? (object)DBNull.Value), new SqlParameter("@DataGuids", dataguids ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", isdeleted), new SqlParameter("@IsArchived", isarchived), new SqlParameter("@IDs", ids ?? (object)DBNull.Value), new SqlParameter("@SortFields", sortfields ?? (object)DBNull.Value), new SqlParameter("@UserID", userid ?? (object)DBNull.Value), new SqlParameter("@IsFromAlert", IsFromAlert) };

                lstItems = context.Database.SqlQuery<RPT_DicrepencyItem>("exec [RPT_DiscrepancyAfterSync] @StartDate,@EndDate,@RoomIDs,@CompanyIDs,@DataGuids,@IsDeleted,@IsArchived,@IDs,@SortFields,@UserID,@IsFromAlert", params1).ToList();

                //List<RPT_DiscrepancyAfterSync_Result> rptdata = context.RPT_DiscrepancyAfterSync(startdate, enddate, roomids, companyids, dataguids, isdeleted, isarchived, ids, sortfields, userid, false).ToList();
                //lstItems = (from rpt in rptdata
                //            select new RPT_DicrepencyItem
                //            {
                //                BinNumber = rpt.binnumber,
                //                BlanketPO = rpt.blanketpo,
                //                created = rpt.created,
                //                CriticalQuantity = rpt.criticalquantity,
                //                ExtendedCost = rpt.extendedcost,
                //                IsAddedFromPDA = rpt.isaddedfrompda,
                //                ItemBinOnHand = rpt.itembinonhand,
                //                itemCost = rpt.itemcost,
                //                ItemNumber = rpt.itemnumber,
                //                LotNumber = rpt.lotnumber,
                //                ManufacturerName = rpt.manufacturername,
                //                ManufacturerNumber = rpt.manufacturernumber,
                //                MaximumQuantity = rpt.maximumquantity,
                //                MinimumQuantity = rpt.minimumquantity,
                //                OnHandQuantity = rpt.onhandquantity,
                //                OnOrderQuantity = rpt.onorderquantity,
                //                PackSlipNumber = rpt.packslipnumber,
                //                PoolQuantity = rpt.poolquantity,
                //                PullCredit = rpt.pullcredit,
                //                RoomInfo = rpt.roominfo,
                //                SerialNumber = rpt.serialnumber,
                //                SupplierName = rpt.suppliername,
                //                SupplierPartNo = rpt.supplierpartno,
                //                UserName = rpt.username,
                //                ItemGUID = rpt.itemguid ?? Guid.Empty,
                //                ItemOnhandQty = rpt.itemonhandqty
                //            }).ToList();
            }
            return lstItems;

        }

        public ReportBuilderDTO GetParentReportMasterByReportID(long ReportID)
        {
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsReportlist = new DataSet();

            dsReportlist = SqlHelper.ExecuteDataset(EturnsConnection, "GetParentReportMasterByReportID", ReportID);
            if (dsReportlist != null && dsReportlist.Tables[0].Rows.Count > 0)
            {
                DataTable dt = new DataTable();
                dt = dsReportlist.Tables[0];

                objDTO.ID = Convert.ToInt64(dt.Rows[0]["id"]);
                if (dt.Rows[0]["reportname"] != DBNull.Value)
                    objDTO.ReportName = Convert.ToString(dt.Rows[0]["reportname"]);
                if (dt.Rows[0]["reportfilename"] != DBNull.Value)
                    objDTO.ReportFileName = Convert.ToString(dt.Rows[0]["reportfilename"]);
                if (dt.Rows[0]["parentid"] != DBNull.Value)
                    objDTO.ParentID = Convert.ToInt64(dt.Rows[0]["parentid"]);
            }
            return objDTO;
        }
        public ReportBuilderDTO GetParentReportDetailByReportID(long ReportID)
        {
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsReportlist = new DataSet();

            dsReportlist = SqlHelper.ExecuteDataset(EturnsConnection, "GetParentReportDetailByReportID", ReportID);
            if (dsReportlist != null && dsReportlist.Tables[0].Rows.Count > 0)
            {
                DataTable dt = new DataTable();
                dt = dsReportlist.Tables[0];

                objDTO.ID = Convert.ToInt64(dt.Rows[0]["id"]);
                if (dt.Rows[0]["reportname"] != DBNull.Value)
                    objDTO.ReportName = Convert.ToString(dt.Rows[0]["reportname"]);
                if (dt.Rows[0]["reportfilename"] != DBNull.Value)
                    objDTO.ReportFileName = Convert.ToString(dt.Rows[0]["reportfilename"]);
                if (dt.Rows[0]["parentid"] != DBNull.Value)
                    objDTO.ParentID = Convert.ToInt64(dt.Rows[0]["parentid"]);
                if (dt.Rows[0]["CombineReportID"] != DBNull.Value)
                    objDTO.CombineReportID = Convert.ToString(dt.Rows[0]["CombineReportID"]);
                if (dt.Rows[0]["MasterReportResFile"] != DBNull.Value)
                    objDTO.MasterReportResFile = Convert.ToString(dt.Rows[0]["MasterReportResFile"]);
            }
            return objDTO;
        }
        public ReportBuilderDTO GetDefaultReportByModuleAndCompany(long CompanyId, string ModuleName)
        {
            ReportBuilderDTO objReportBuilderDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int64 ReportId = GetDefaultReportID(ModuleName, 0, CompanyId);

                objReportBuilderDTO = (from u in context.ReportMasters
                                       where u.ID == ReportId
                                       //where !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                       //      && u.ModuleName.Trim().ToUpper() == ModuleName.Trim().ToUpper()
                                       //      && u.ReportType == 2
                                       //      && u.SetAsDefaultPrintReport == true
                                       //      && (u.ISEnterpriseReport == true || u.CompanyID == CompanyId)
                                       select new ReportBuilderDTO
                                       {
                                           ID = u.ID,
                                           ReportName = u.ReportName,
                                           ReportFileName = u.ReportFileName,
                                           ReportType = u.ReportType,
                                           SubReportFileName = u.SubReportFileName,
                                           SortColumns = u.SortColumns,
                                           IsPrivate = u.IsPrivate,
                                           PrivateUserID = u.PrivateUserID,
                                           MasterReportResFile = u.MasterReportResFile,
                                           SubReportResFile = u.SubReportResFile,
                                           IsIncludeDateRange = u.IsIncludeDateRange,
                                           IsIncludeTotal = u.IsIncludeTotal,
                                           IsIncludeSubTotal = u.IsIncludeSubTotal,
                                           IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                                           IsIncludeGroup = u.IsIncludeGroup,
                                           GroupName = u.GroupName,
                                           Days = u.Days,
                                           FromDate = u.FromDate,
                                           ToDate = u.ToDate,
                                           ParentID = u.ParentID,
                                           CompanyID = u.CompanyID,
                                           RoomID = u.RoomID,
                                           ToEmailAddress = u.ToEmailAddress,
                                           ModuleName = u.ModuleName,
                                           IsBaseReport = u.IsBaseReport,
                                           ISEnterpriseReport = u.ISEnterpriseReport,
                                           IsArchived = u.IsArchived,
                                           IsDeleted = u.IsDeleted,
                                           SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                                           IsIncludeTax1 = u.IsIncludeTax1,
                                           IsIncludeTax2 = u.IsIncludeTax2,
                                           IsNotEditable = u.IsNotEditable,
                                           HideHeader = u.HideHeader,
                                           ShowSignature = u.ShowSignature
                                       }
                                      ).FirstOrDefault();
            }
            return objReportBuilderDTO;
        }

        public ReportBuilderDTO GetReportByReportNameAndCompany(string ReportName, bool? CheckIsBaseReport = null)
        {
            ReportBuilderDTO objReportBuilderDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objReportBuilderDTO = (from u in context.ReportMasters
                                       where !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                                             && u.ReportName.Trim().ToUpper() == ReportName.Trim().ToUpper()
                                             && u.ReportType == 2
                                             && (CheckIsBaseReport == null || u.IsBaseReport == CheckIsBaseReport)
                                       select new ReportBuilderDTO
                                       {
                                           ID = u.ID,
                                           ReportName = u.ReportName,
                                           ReportFileName = u.ReportFileName,
                                           ReportType = u.ReportType,
                                           SubReportFileName = u.SubReportFileName,
                                           SortColumns = u.SortColumns,
                                           IsPrivate = u.IsPrivate,
                                           PrivateUserID = u.PrivateUserID,
                                           MasterReportResFile = u.MasterReportResFile,
                                           SubReportResFile = u.SubReportResFile,
                                           IsIncludeDateRange = u.IsIncludeDateRange,
                                           IsIncludeTotal = u.IsIncludeTotal,
                                           IsIncludeSubTotal = u.IsIncludeSubTotal,
                                           IsIncludeGrandTotal = u.IsIncludeGrandTotal,
                                           IsIncludeGroup = u.IsIncludeGroup,
                                           GroupName = u.GroupName,
                                           Days = u.Days,
                                           FromDate = u.FromDate,
                                           ToDate = u.ToDate,
                                           ParentID = u.ParentID,
                                           CompanyID = u.CompanyID,
                                           RoomID = u.RoomID,
                                           ToEmailAddress = u.ToEmailAddress,
                                           ModuleName = u.ModuleName,
                                           IsBaseReport = u.IsBaseReport,
                                           ISEnterpriseReport = u.ISEnterpriseReport,
                                           IsArchived = u.IsArchived,
                                           IsDeleted = u.IsDeleted,
                                           SetAsDefaultPrintReport = u.SetAsDefaultPrintReport,
                                           IsIncludeTax1 = u.IsIncludeTax1,
                                           IsIncludeTax2 = u.IsIncludeTax2,
                                           IsNotEditable = u.IsNotEditable,
                                           HideHeader = u.HideHeader,
                                           ShowSignature = u.ShowSignature
                                       }
                                      ).FirstOrDefault();
            }
            return objReportBuilderDTO;
        }

        public List<ReportMasterDTO> GetPagedReports(Int32 StartRowIndex, Int32 MaxRows, out Int64 TotalCount, string SearchTerm, string sortColumnName, Int64 LoggedInUserID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {

            string CreatedDateFrom = null, CreatedDateTo = null, UpdatedDateFrom = null, UpdatedDateTo = null, ReportCreaters = null, ReportUpdators = null;

            string ReportType = string.Empty;

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
                    ReportCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ReportUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    // UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    // UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrEmpty(FieldsPara[108]))
                {
                    ReportType = FieldsPara[108].TrimEnd(',');
                }
            }




            List<ReportMasterDTO> lstReports = new List<ReportMasterDTO>();
            var params1 = new SqlParameter[] {
                  new SqlParameter("@StartRowIndex", StartRowIndex)
                , new SqlParameter("@MaxRows", MaxRows)
                , new SqlParameter("@SearchTerm", SearchTerm ??(object)DBNull.Value)
                , new SqlParameter("@sortColumnName", sortColumnName ??(object)DBNull.Value)
                , new SqlParameter("@ReportCreaters", ReportCreaters??(object)DBNull.Value )
                , new SqlParameter("@ReportUpdators",  ReportUpdators ??(object)DBNull.Value)
                , new SqlParameter("@CreatedDateFrom", CreatedDateFrom ??(object)DBNull.Value)
                , new SqlParameter("@CreatedDateTo", CreatedDateTo ??(object)DBNull.Value)
                , new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ??(object)DBNull.Value)
                , new SqlParameter("@UpdatedDateTo", UpdatedDateTo ??(object)DBNull.Value)
                , new SqlParameter("@IsDeleted", IsDeleted)
                , new SqlParameter("@IsArchived", IsArchived)
                , new SqlParameter("@RoomID", RoomID)
                , new SqlParameter("@CompanyID", CompanyID)
                , new SqlParameter("@LoggedinUserID", LoggedInUserID)
                , new SqlParameter("@ReportType", ReportType  ??(object)DBNull.Value)


            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstReports = context.Database.SqlQuery<ReportMasterDTO>("EXEC [GetPagedReports] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@ReportCreaters,@ReportUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@IsDeleted,@IsArchived,@RoomID,@CompanyID,@LoggedinUserID,@ReportType", params1).ToList();
            }
            if (lstReports != null && lstReports.Count > 0)
            {
                TotalCount = lstReports.First().TotalRecords;
            }
            else
            {
                TotalCount = 0;
            }
            return lstReports;
        }


        public List<ReportMasterDTO> GetAllBaseReport(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            List<ReportMasterDTO> lstReports = new List<ReportMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstReports = (from u in context.ReportMasters
                              where !(u.IsDeleted ?? false) && !(u.IsArchived ?? false)
                              && (u.IsBaseReport == true)
                              orderby u.ReportName ascending
                              select new ReportMasterDTO
                              {
                                  ID = u.ID,
                                  ReportName = u.ReportName,
                                  ReportFileName = u.ReportFileName,
                                  ReportType = u.ReportType,
                                  SubReportFileName = u.SubReportFileName,
                                  ResourceKey = u.ResourceKey
                              }
                                      ).ToList();
            }
            return lstReports;
        }

        public List<TransactionEventMasterDTO> GetTransactionEventDetailByReportID(long ReportID)
        {
            List<TransactionEventMasterDTO> lstEventMaster = new List<TransactionEventMasterDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@ReportID", ReportID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstEventMaster = context.Database.SqlQuery<TransactionEventMasterDTO>("EXEC [GetTransactionEventDetailByReportID] @ReportID", params1).ToList();
            }

            return lstEventMaster;
        }

        public List<TransactionEventMasterDTO> GetTransactionEventDetailByItemType(long ReportID, long EmailTemplateID, string ItemType)
        {
            List<TransactionEventMasterDTO> lstEventMaster = new List<TransactionEventMasterDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@ReportID", ReportID), new SqlParameter("@EmailTemplateID", EmailTemplateID), new SqlParameter("@ItemType", ItemType) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstEventMaster = context.Database.SqlQuery<TransactionEventMasterDTO>("EXEC [GetTransactionEventDetailByReportID] @ReportID,@EmailTemplateID,@ItemType", params1).ToList();
            }

            return lstEventMaster;
        }

        public ReportMasterDTO GetReportSigleRecord(long ID, long EmailTemplateID, string ItemType)
        {
            ReportMasterDTO objReportMasterDTO = new ReportMasterDTO();

            var params1 = new SqlParameter[] { new SqlParameter("@ReportID", ID), new SqlParameter("@EmailTemplateID", EmailTemplateID), new SqlParameter("@ItemType", ItemType) };


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objReportMasterDTO = context.Database.SqlQuery<ReportMasterDTO>("EXEC [GetReportRecordByID] @ReportID, @EmailTemplateID, @ItemType", params1).ToList().FirstOrDefault();

            }
            return objReportMasterDTO;
        }

        public string GetChildReportIDs(string ParentReportIDs, long CompanyID)
        {
            string ChildReportIDs = string.Empty;

            ReportMasterDTO objReportMasterDTO = new ReportMasterDTO();

            var params1 = new SqlParameter[] { new SqlParameter("@ParentReportIDs", ParentReportIDs), new SqlParameter("@CompanyID", CompanyID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<long> childList = context.Database.SqlQuery<long>("EXEC [GetAllChildReportFromParentList] @ParentReportIDs, @CompanyID", params1).ToList();
                if (childList != null && childList.Count > 0)
                {
                    ChildReportIDs = string.Join(",", childList.ToArray());
                }
            }

            return ChildReportIDs;
        }

        public ReportMasterDTO GetParentReportSigleRecord(long ID, long EmailTemplateID, string ItemType)
        {
            ReportMasterDTO objReportMasterDTO = new ReportMasterDTO();

            var params1 = new SqlParameter[] { new SqlParameter("@ReportID", ID), new SqlParameter("@EmailTemplateID", EmailTemplateID), new SqlParameter("@ItemType", ItemType) };


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objReportMasterDTO = context.Database.SqlQuery<ReportMasterDTO>("EXEC [GetParentReportRecordByID] @ReportID, @EmailTemplateID, @ItemType", params1).ToList().FirstOrDefault();

            }
            return objReportMasterDTO;
        }

        public Int64 InsertReportAlertConfig(ReportMasterDTO objDTO, long CompanyID, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReportAlertConfig obj = new ReportAlertConfig();
                
                if (objDTO.ItemType == "Report")
                {
                    obj.EmailTemplateID = 0;
                    obj.ReportID = objDTO.ID;
                }
                else if (objDTO.ItemType == "EmailTemplate")
                {
                    obj.ReportID = 0;
                    obj.EmailTemplateID = objDTO.EmailTemplateID;
                }
                else
                {
                    obj.EmailTemplateID = 0;
                    obj.ReportID = 0;
                }

                obj.AllowScheduleIMM = objDTO.AllowScheduleIMM;
                obj.AllowScheduleHourly = objDTO.AllowScheduleHourly;
                obj.AllowScheduleWeekly = objDTO.AllowScheduleWeekly;
                obj.AllowScheduleMonthly = objDTO.AllowScheduleMonthly;
                obj.AllowedIMMActions = objDTO.AllowedIMMActions;
                obj.AllowDataSelectSinceLastReportFilter = objDTO.AllowDataSelectSinceLastReportFilter;
                obj.AllowDataSelectFirstOfMonth = objDTO.AllowDataSelectFirstOfMonth;
                obj.AllowDataSinceFilter = objDTO.AllowDataSinceFilter;
                obj.AllowSupplierFilter = objDTO.AllowSupplierFilter;
                obj.AllowPDFAttachment = objDTO.AllowPDFAttachment;
                obj.AllowExcelAttachment = objDTO.AllowExcelAttachment;
                obj.AllowAttachmentSelection = objDTO.AllowAttachmentSelection;
                obj.AllowedAttahmentReports = objDTO.AllowedAttahmentReports;
                obj.AllowScheduleDaily = objDTO.AllowScheduleDaily;
                obj.AllowRangeDataSelect = objDTO.AllowRangeDataSelect;
                obj.IsSupplierRequired = objDTO.IsSupplierRequired;
                obj.IsDateRangeRequired =objDTO.IsDateRangeRequired;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.CreatedDate = objDTO.CreatedOn;
                obj.UpdatedDate = objDTO.UpdatedON;

                context.ReportAlertConfigs.Add(obj);
                context.SaveChanges();
                objDTO.AlertConfigID = obj.ID;

                if (objDTO.ItemType == "Report")
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ReportID", objDTO.ID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@UserID", UserID) };
                    context.Database.ExecuteSqlCommand("EXEC [SaveChileReportAlertConfig] @ReportID, @CompanyID, @UserID", params1);
                }

                return obj.ID;
            }
        }

        public bool EditReportAlertConfig(ReportMasterDTO objDTO, long CompanyID, long UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {


                ReportAlertConfig obj = new ReportAlertConfig();
                if (objDTO.ItemType == "Report")
                {
                    obj = context.ReportAlertConfigs.Where(x => x.ID == objDTO.AlertConfigID && x.ReportID == objDTO.ID).FirstOrDefault();
                    obj.EmailTemplateID = 0;
                    obj.ReportID = objDTO.ID;
                }
                else if (objDTO.ItemType == "EmailTemplate")
                {
                    obj = context.ReportAlertConfigs.Where(x => x.ID == objDTO.AlertConfigID && x.EmailTemplateID == objDTO.EmailTemplateID).FirstOrDefault();
                    obj.ReportID = 0;
                    obj.EmailTemplateID = objDTO.EmailTemplateID;
                }
                else
                {
                    obj.EmailTemplateID = 0;
                    obj.ReportID = 0;
                }

                obj.AllowScheduleIMM = objDTO.AllowScheduleIMM;
                obj.AllowScheduleHourly = objDTO.AllowScheduleHourly;
                obj.AllowScheduleWeekly = objDTO.AllowScheduleWeekly;
                obj.AllowScheduleMonthly = objDTO.AllowScheduleMonthly;
                obj.AllowedIMMActions = objDTO.AllowedIMMActions;
                obj.AllowDataSelectSinceLastReportFilter = objDTO.AllowDataSelectSinceLastReportFilter;
                obj.AllowDataSelectFirstOfMonth = objDTO.AllowDataSelectFirstOfMonth;
                obj.AllowDataSinceFilter = objDTO.AllowDataSinceFilter;
                obj.AllowSupplierFilter = objDTO.AllowSupplierFilter;
                obj.AllowPDFAttachment = objDTO.AllowPDFAttachment;
                obj.AllowExcelAttachment = objDTO.AllowExcelAttachment;
                obj.AllowAttachmentSelection = objDTO.AllowAttachmentSelection;
                obj.AllowedAttahmentReports = objDTO.AllowedAttahmentReports;
                obj.AllowScheduleDaily = objDTO.AllowScheduleDaily;
                obj.AllowRangeDataSelect = objDTO.AllowRangeDataSelect;
                obj.IsSupplierRequired = objDTO.IsSupplierRequired;
                obj.IsDateRangeRequired = objDTO.IsDateRangeRequired;

                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if (objDTO.ItemType == "Report")
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ReportID", objDTO.ID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@UserID", UserID) };
                    context.Database.ExecuteSqlCommand("EXEC [SaveChileReportAlertConfig] @ReportID, @CompanyID, @UserID", params1);
                }

                return true;
            }

        }

        public List<TransactionEventMasterDTO> GetTransactionEventByCode(string eventCode)
        {
            List<TransactionEventMasterDTO> lstEventMaster = new List<TransactionEventMasterDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@EventCode", eventCode) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstEventMaster = context.Database.SqlQuery<TransactionEventMasterDTO>("EXEC [GetTransactionEventDetailByCode] @EventCode", params1).ToList();
            }

            return lstEventMaster;
        }


        //Audit Trail

        public void Call_AT_AT_Calc_Qty(string ItemGuids)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                {
                    new SqlParameter("@ItemGuid", ItemGuids  ?? (object)DBNull.Value)
                };
                string strQuery = @"EXEC [AT_AT_Calc_Qty] @ItemGuid";
                context.Database.CommandTimeout = 7200;
                context.Database.ExecuteSqlCommand(strQuery, params1);
            }
        }


        //AuditTrail Transaction

        public void Call_AT_AuditTrail_CalculateQty(string ItemGuids)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                {
                    new SqlParameter("@ItemGuid", ItemGuids  ?? (object)DBNull.Value)
                };
                string strQuery = @"EXEC [AT_AuditTrail_CalculateQty] @ItemGuid";
                context.Database.CommandTimeout = 7200;
                context.Database.ExecuteSqlCommand(strQuery, params1);
            }
        }

        public List<ReportBuilderDTO> GetChildReportListByReportFileName(string reportFileName)
        {
            List<ReportBuilderDTO> lstReportData = new List<ReportBuilderDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@reportFileName", reportFileName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstReportData = context.Database.SqlQuery<ReportBuilderDTO>("EXEC [GetChildReportListByReportFileName] @reportFileName", params1).ToList();
            }
            return lstReportData;
        }

        #region [InStock Data For Service]
        public List<RRT_InstockByBinDTO> GetDailyInStockDataForService(long EnterpriseID, long CompanyID, long RoomID, bool isLocal, string strInStockDate, long? SupplierId, string ItemNumber = null)
        {
            List<RRT_InstockByBinDTO> lstItemData = new List<RRT_InstockByBinDTO>();
            SupplierId = (SupplierId == null || SupplierId < 1) ? 0 : SupplierId;
            ItemNumber = (ItemNumber == null || ItemNumber == "") ? "" : ItemNumber;
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@IsLocal", isLocal), new SqlParameter("@InStockDate", strInStockDate), new SqlParameter("@SupplierId", SupplierId), new SqlParameter("@ItemNumber", ItemNumber) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemData = context.Database.SqlQuery<RRT_InstockByBinDTO>("EXEC [SVC_GetInStockDataForService] @EnterpriseID,@CompanyID,@RoomID,@IsLocal,@InStockDate,@SupplierId,@ItemNumber", params1).ToList();
            }
            return lstItemData;
        }
        #endregion

        #region For Default Print option as par module WI-4440

        public List<ModuleWiseReportListForDefaultPrintDTO> GetModuleWiseReportListForDefaultPrint(Int64 CompanyID)
        {
            List<ModuleWiseReportListForDefaultPrintDTO> lstReportMaster = new List<ModuleWiseReportListForDefaultPrintDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                lstReportMaster = context.Database.SqlQuery<ModuleWiseReportListForDefaultPrintDTO>("EXEC [GetModuleWiseReportListForDefaultPrint] @CompanyID", params1).ToList();
            }

            return lstReportMaster;
        }

        public List<ModuleWiseReportForDefaultPrintDTO> GetReportForDefaultPrintByModuleID(Int64? ModuleID, Int64 RoomID, Int64 CompanyID, string ModuleName)
        {
            List<ModuleWiseReportForDefaultPrintDTO> lstDefaultPrintReportMaster = new List<ModuleWiseReportForDefaultPrintDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleID", ModuleID > 0 ? ModuleID : (object)DBNull.Value), new SqlParameter("@Room", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ModuleName", string.IsNullOrEmpty(ModuleName) ? (object)DBNull.Value : ModuleName) };
                lstDefaultPrintReportMaster = context.Database.SqlQuery<ModuleWiseReportForDefaultPrintDTO>("EXEC [GetReportForDefaultPrintByModuleID]  @ModuleID,@Room	,@CompanyID,@ModuleName", params1).ToList();
            }

            return lstDefaultPrintReportMaster;
        }

        public long InsertOrUpdateModuleWiseDefaultPrint(List<ModuleWiseReportForDefaultPrintDTO> lstModuleWiseDefaultPrint, Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            long Success = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (lstModuleWiseDefaultPrint != null && lstModuleWiseDefaultPrint.Count > 0)
                {
                    foreach (ModuleWiseReportForDefaultPrintDTO objReportData in lstModuleWiseDefaultPrint)
                    {
                        if (objReportData.ModuleID > 0)
                        {

                            ModuleWiseReportForDefaultPrint objData = context.ModuleWiseReportForDefaultPrints.Where(t => t.ModuleID == objReportData.ModuleID && t.Room == RoomID && t.CompanyID == CompanyID && t.IsDeleted == false).FirstOrDefault();
                            if (objData != null && objData.ModuleID == objReportData.ModuleID)
                            {
                                if (objReportData.DefaultPrintReportID <= 0)
                                {
                                    objData.IsDeleted = true;
                                }
                                else
                                {
                                    objData.DefaultPrintReportID = objReportData.DefaultPrintReportID;
                                    objData.IsDeleted = objReportData.IsDeleted.HasValue ? (bool)objReportData.IsDeleted : false;
                                }
                                objData.IsArchived = objReportData.IsArchived.HasValue ? (bool)objReportData.IsArchived : false;
                                objData.ModuleID = objReportData.ModuleID;
                                objData.Updated = DateTimeUtility.DateTimeNow;
                                objData.LastUpdatedBy = UserID;
                                objData.CompanyID = CompanyID;
                                objData.Room = RoomID;

                                objData.EditedFrom = !string.IsNullOrEmpty(objReportData.EditedFrom) ? objReportData.EditedFrom : "web";

                                context.SaveChanges();

                                Success = objData.ModuleID;
                            }
                            else
                            {
                                if (objReportData.DefaultPrintReportID > 0)
                                {
                                    ModuleWiseReportForDefaultPrint obj = new ModuleWiseReportForDefaultPrint();

                                    obj.ID = 0;
                                    obj.ModuleID = objReportData.ModuleID;
                                    obj.DefaultPrintReportID = objReportData.DefaultPrintReportID;
                                    obj.Created = DateTimeUtility.DateTimeNow;
                                    obj.Updated = DateTimeUtility.DateTimeNow;
                                    obj.CreatedBy = UserID;
                                    obj.LastUpdatedBy = UserID;
                                    obj.CompanyID = CompanyID;
                                    obj.Room = RoomID;
                                    obj.IsDeleted = objReportData.IsDeleted.HasValue ? (bool)objReportData.IsDeleted : false;
                                    obj.IsArchived = objReportData.IsArchived.HasValue ? (bool)objReportData.IsArchived : false;
                                    obj.AddedFrom = !string.IsNullOrEmpty(objReportData.AddedFrom) ? objReportData.AddedFrom : "web";
                                    obj.EditedFrom = !string.IsNullOrEmpty(objReportData.EditedFrom) ? objReportData.EditedFrom : "web";

                                    context.ModuleWiseReportForDefaultPrints.Add(obj);
                                    context.SaveChanges();
                                    obj.ID = Success = obj.ID;
                                }
                            }

                            if (objReportData.SetAsDefaultPrintReportForAllRoom == true)
                            {
                                InsertDefaultReportForAllRoom(objReportData.DefaultPrintReportID, "", objReportData.ModuleID, 0, 0);
                            }
                        }
                    }
                }
            }
            return Success;
        }

        public Int64 GetDefaultReportIDBasedonModuleName(string ModuleName, Int64 RoomId, Int64 CompanyID)
        {
            Int64 reportid = 0;
            List<ModuleWiseReportForDefaultPrintDTO> lstReports = GetReportForDefaultPrintByModuleID(0, RoomId, CompanyID, ModuleName);

            if (lstReports != null && lstReports.Count > 0)
            {
                reportid = lstReports[0].DefaultPrintReportID;
            }
            return reportid;
        }

        public bool InsertModuleWiseDefaulrReport(long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyIDValue", CompanyID), new SqlParameter("@RoomIDValue", RoomID) };
                context.Database.ExecuteSqlCommand("EXEC [InsertModuleWiseDefaulrReport] @CompanyIDValue, @RoomIDValue", params1);
            }

            return true;
        }

        public bool UpdateDefaultReportAfterReportDeleted(Int64 DeletedReportID, Int64? CompanyID = null, Int64? RoomID = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@DeletedReportID", DeletedReportID),
                                                   new SqlParameter("@CompanyID", (CompanyID > 0 ? CompanyID.GetValueOrDefault(0) : (object)DBNull.Value)),
                                                   new SqlParameter("@RoomID", (RoomID > 0 ? RoomID.GetValueOrDefault(0) : (object)DBNull.Value))};
                context.Database.ExecuteSqlCommand("EXEC [UpdateDefaultReportAfterReportDeleted] @DeletedReportID,@CompanyID,@RoomID", params1);
            }
            return true;
        }

        public bool UpdateDefaultReportBasedonCustomiseReport(Int64 NewDefaultPrintReportID, string ModuleName, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@NewDefaultPrintReportID", NewDefaultPrintReportID) ,
                                                   new SqlParameter("@ModuleName", ModuleName),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID)};
                context.Database.ExecuteSqlCommand("EXEC [UpdateDefaultReportBasedonCustomiseReport] @NewDefaultPrintReportID,@ModuleName,@CompanyID,@RoomID", params1);
            }
            return true;
        }

        public ReportMaster GetReportDetailsByDefaultReportID(Int64 RoomID, Int64 CompanyID, string ModuleName)
        {
            ReportMaster objReportMaster = new ReportMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Room", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ModuleName", string.IsNullOrEmpty(ModuleName) ? (object)DBNull.Value : ModuleName) };
                objReportMaster = context.Database.SqlQuery<ReportMaster>("EXEC [GetReportDetailsByDefaultReportID] @Room,@CompanyID,@ModuleName", params1).FirstOrDefault();
            }

            return objReportMaster;
        }

        #endregion

        #region WI-4947 - Please add a Last Run Date to the schedule reports

        public ReportMailLogDTO GetLastRunDateforSchedule(long NotificationID, long CompanyID, long RoomID)
        {
            ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@NotificationID", NotificationID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objReportMailLogDTO = context.Database.SqlQuery<ReportMailLogDTO>("EXEC [GetLastRunDateforSchedule] @CompanyID, @RoomID, @NotificationID", params1).ToList().FirstOrDefault();

            }
            return objReportMailLogDTO;
        }

        public bool InsertViewReportHistory(ViewReportHistory objViewReportHistory)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ViewReportHistory objViewReport = new ViewReportHistory();
                objViewReport.ID = 0;
                objViewReport.ReportID = objViewReportHistory.ReportID;
                objViewReport.RoomId = objViewReportHistory.RoomId;
                objViewReport.CompanyId = objViewReportHistory.CompanyId;
                objViewReport.ViewReportDate = DateTime.UtcNow;
                objViewReport.UserId = objViewReportHistory.UserId;
                objViewReport.RequestType = objViewReportHistory.RequestType;
                objViewReport.ReportParameters = objViewReportHistory.ReportParameters;
                context.ViewReportHistories.Add(objViewReport);
                context.SaveChanges();
            }
            return true;
        }

        #endregion

        public List<ReportMasterDTO> GetChildReport(Int64 ParentID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ParentID", ParentID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted) };
                return (from u in context.Database.SqlQuery<ReportMasterDTO>("exec [RPT_GetReportByParentID] @ParentID,@IsArchived,@IsDeleted", params1)
                        select new ReportMasterDTO
                        {
                            ID = u.ID,
                            ReportName = u.ReportName,
                            ReportType = u.ReportType,
                            CompanyID = u.CompanyID,
                            RoomID = u.RoomID,
                            SortColumns = u.SortColumns,
                            IsBaseReport = u.IsBaseReport,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            SetAsDefaultPrintReport = u.SetAsDefaultPrintReport
                        }).ToList();
            }
        }

        #region for find pullsummaryreport child WI-5107 Pull Summary report - consolidate each item into only one line item summary

        public List<ReportMasterDTO> FindPullSummaryChildReport()
        {
            List<ReportMasterDTO> lstReportMasters = new List<ReportMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string eTurnsMasterDBName = DbConnectionHelper.GetETurnsMasterDBName();
                lstReportMasters = context.Database.SqlQuery<ReportMasterDTO>("EXEC [" + eTurnsMasterDBName + "].[dbo].[FindPullSummaryChildReport]").ToList();
            }
            return lstReportMasters;
        }

        #endregion


        public string GetScheduleNamebyReportID(long ReportID, long RoomID, long CompanyID)
        {
            string ChildReportIDs = string.Empty;

            ReportMasterDTO objReportMasterDTO = new ReportMasterDTO();

            var params1 = new SqlParameter[] { new SqlParameter("@ReportID", ReportID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<string> ScheduleName = context.Database.SqlQuery<string>("EXEC [GetScheduleNamebyReportID] @ReportID,@RoomID,@CompanyID", params1).ToList();
                if (ScheduleName != null && ScheduleName.Count > 0)
                {
                    ChildReportIDs = string.Join(",", ScheduleName.ToArray());
                }
            }

            return ChildReportIDs;
        }

        #region WI-5426 -Tool Instock Report

        public List<RPT_Tools> getToolsforInstockRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string Includestockedouttools, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ToolName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);

                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(roomIDs))
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = roomIDs;
                    parameters.Add(rm);
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }

                if (!string.IsNullOrEmpty(Includestockedouttools))
                {
                    SqlParameter OEX = new SqlParameter("@Includestockedouttools", SqlDbType.VarChar, 1000);
                    OEX.Value = Includestockedouttools;
                    parameters.Add(OEX);
                }
                else
                {
                    SqlParameter OEX = new SqlParameter("@Includestockedouttools", SqlDbType.VarChar, 1000);
                    OEX.Value = DBNull.Value;
                    parameters.Add(OEX);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@RoomIDs,@CompanyIDs,@Includestockedouttools";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_Tools>(strQuery1, ArrParams).ToList();
            }
        }

        #endregion

        public bool InsertDefaultReportForAllRoom(Int64 NewDefaultPrintReportID, string ModuleName, Int64 ModuleID, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@NewDefaultPrintReportID", NewDefaultPrintReportID) ,
                                                   new SqlParameter("@ModuleName", ModuleName),
                                                   new SqlParameter("@ModuleID", ModuleID),
                                                   new SqlParameter("@MainCompanyID", CompanyID),
                                                   new SqlParameter("@MainRoomId", RoomID)};
                context.Database.ExecuteSqlCommand("EXEC [InsertDefaultReportForAllRoom] @NewDefaultPrintReportID,@ModuleName,@ModuleID,@MainCompanyID,@MainRoomId", params1);
            }
            return true;
        }

        public List<ReportMasterDTO> GetChildReportsFromParentID(Int64 ParentReportID)
        {
            List<ReportMasterDTO> lstReportMaster = new List<ReportMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ParentReportID", ParentReportID) };
                lstReportMaster = context.Database.SqlQuery<ReportMasterDTO>("EXEC [GetChildReportsFromParentID] @ParentReportID", params1).ToList();
            }

            return lstReportMaster;
        }

        public string GetOrderItemGuidByReportRange(string _range, string _rangeFieldID, string _rangeData, string RoomID, string CompanyID, bool _isSelectAllRangeDataPull = false)
        {
            if (string.IsNullOrWhiteSpace(_range) || string.IsNullOrWhiteSpace(_rangeFieldID))
                return string.Empty;

            string _dataGuids = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Range",_range)
                                    , new SqlParameter("@RangeData",_rangeData)
                                    , new SqlParameter("@RoomID",RoomID)
                                    , new SqlParameter("@CompanyID",CompanyID)
                                    , new SqlParameter("@IsSelectAllRangeDataPull",_isSelectAllRangeDataPull)};

                string qry = "exec [Schl_GetOrderItemGuidByReportRange] @Range, @RangeData,@RoomID, @CompanyID, @IsSelectAllRangeDataPull";
                List<RPT_OrderItemSummary> lst = context.Database.SqlQuery<RPT_OrderItemSummary>(qry, params1).ToList();

                if (lst != null && lst.Any())
                {
                    if (_range == "ItemNumber")
                    {
                        _dataGuids = string.Join(",", lst.Select(t => t.ItemGuid).Distinct().ToArray());
                    }
                    else if (_range == "SupplierName")
                    {
                        _dataGuids = string.Join(",", lst.Select(t => t.SupplierID).Distinct().ToArray());
                    }
                    else if (_range == "ManufacturerName")
                    {
                        _dataGuids = string.Join(",", lst.Select(t => t.ManufacturerID).Distinct().ToArray());
                    }
                    else if(_range == "CategoryName")
                    {
                        _dataGuids = string.Join(",", lst.Select(t => t.CategoryID).Distinct().ToArray());
                    }
                }

                return _dataGuids;
            }
        }

        public List<string> GetReportIgnoreColumnListByReportName(string ReportName)
        {
            List<string> lstReportMaster = new List<string>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string eTurnsMasterDBName = DbConnectionHelper.GetETurnsMasterDBName();                
                var params1 = new SqlParameter[] { new SqlParameter("@ReportName", ReportName) };
                lstReportMaster = context.Database.SqlQuery<string>("EXEC [" + eTurnsMasterDBName + "].[dbo].[GetReportIgnoreColumnListByReportName] @ReportName", params1).ToList();
            }

            return lstReportMaster;
        }

        public List<RPT_WorkOrder> GetWorkorderListRangeDataForSchedule(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, Int64? UserID, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "WOName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_ForSchedule @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID";

                context.Database.CommandTimeout= 3600;
                return context.Database.SqlQuery<RPT_WorkOrder>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_CountMasterDTO> GetCountMasterForCustAndConsignedRangeData(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate,string CountAppliedFilter, Int64? UserID, bool IsRoomIdFilter, bool _isRunWithReportConnection, bool IsConsignedReport, bool isCustomerOwnedReport)
        {

            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "CountName";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;

                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {

                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }


                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }

                if (!string.IsNullOrEmpty(CountAppliedFilter))
                {
                    SqlParameter cm = new SqlParameter("@AppliedFilter", SqlDbType.VarChar, 2048);
                    cm.Value = CountAppliedFilter;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@AppliedFilter", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }

                SqlParameter cmIsConsignedReport = new SqlParameter("@IsConsignedReport", SqlDbType.VarChar, 2048);
                cmIsConsignedReport.Value = IsConsignedReport;
                parameters.Add(cmIsConsignedReport);

                SqlParameter cmisCustomerOwnedReport = new SqlParameter("@isCustomerOwnedReport", SqlDbType.VarChar, 2048);
                cmisCustomerOwnedReport.Value = isCustomerOwnedReport;
                parameters.Add(cmisCustomerOwnedReport);
               

                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_RangeData @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@AppliedFilter,@IsConsignedReport,@isCustomerOwnedReport";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_CountMasterDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RRT_InstockByBinDTO> GetInStockByBinRangeDataForSchedule(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate, string SupplierIDs, string QOHFilters, string OnlyExirationItems, Int64? UserID,string[] arrItemIsActive=null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }
                if (!string.IsNullOrEmpty(SupplierIDs))
                {
                    SqlParameter IS = new SqlParameter("@SupplierIDs", SqlDbType.VarChar, 1000);
                    IS.Value = SupplierIDs;
                    parameters.Add(IS);
                }
                else
                {
                    SqlParameter IS = new SqlParameter("@SupplierIDs", SqlDbType.VarChar, 1000);
                    IS.Value = DBNull.Value;
                    parameters.Add(IS);
                }

                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }
                if (!string.IsNullOrEmpty(QOHFilters))
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = QOHFilters;
                    parameters.Add(QOH);
                }
                else
                {
                    SqlParameter QOH = new SqlParameter("@QOHFilter", SqlDbType.VarChar, 1000);
                    QOH.Value = DBNull.Value;
                    parameters.Add(QOH);
                }

                if (!string.IsNullOrEmpty(OnlyExirationItems))
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = OnlyExirationItems;
                    parameters.Add(OEX);
                }
                else
                {
                    SqlParameter OEX = new SqlParameter("@OnlyExirationItems", SqlDbType.VarChar, 1000);
                    OEX.Value = DBNull.Value;
                    parameters.Add(OEX);
                }


                if (!string.IsNullOrEmpty(itemIsActive))
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = itemIsActive;
                    parameters.Add(IAC);
                }
                else
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = DBNull.Value;
                    parameters.Add(IAC);
                }
                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_ForSchedule @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@SupplierIDs,@QOHFilter,@OnlyExirationItems,@ItemIsActive";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RRT_InstockByBinDTO>(strQuery1, ArrParams).ToList();
            }
        }

        public List<RPT_MoveMaterialDTO> GetMoveBinTransactionsRangeDataForSchedule(string SpName, string rangeFieldName, Int64[] CompanyIDs, Int64[] RoomIDs, string StartDate, string EndDate,  Int64? UserID,string MoveType, string[] arrItemIsActive = null, bool IsRoomIdFilter = true, bool _isRunWithReportConnection = false)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (_isRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            using (var context = new eTurnsEntities(_strConnectionString))
            {

                if (string.IsNullOrWhiteSpace(rangeFieldName))
                {
                    rangeFieldName = "ItemNumber";
                }

                var parameters = new List<SqlParameter>();

                SqlParameter RF = new SqlParameter("@RangeFieldName", SqlDbType.VarChar, 2048);
                RF.Value = rangeFieldName;
                parameters.Add(RF);


                string compIDs = string.Empty;
                string roomIDs = string.Empty;
                string itemIsActive = string.Empty;
                if (CompanyIDs != null)
                {
                    for (int i = 0; i < CompanyIDs.Length; i++)
                    {
                        if (compIDs.Length > 0)
                            compIDs += ",";

                        compIDs += CompanyIDs[i];
                    }
                }

                if (RoomIDs != null)
                {
                    for (int i = 0; i < RoomIDs.Length; i++)
                    {
                        if (roomIDs.Length > 0)
                            roomIDs += ",";

                        roomIDs += RoomIDs[i];
                    }
                }
                if (arrItemIsActive != null)
                {
                    for (int i = 0; i < arrItemIsActive.Length; i++)
                    {
                        if (itemIsActive.Length > 0)
                            itemIsActive += ",";

                        itemIsActive += arrItemIsActive[i];
                    }
                }

                if (!string.IsNullOrEmpty(StartDate) && Convert.ToDateTime(StartDate) > DateTime.MinValue)
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(SD);

                }
                else
                {
                    SqlParameter SD = new SqlParameter("@StartDate", SqlDbType.VarChar, 1000);
                    SD.Value = DBNull.Value;
                    parameters.Add(SD);
                }

                if (!string.IsNullOrEmpty(EndDate) && Convert.ToDateTime(EndDate) > DateTime.MinValue)
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add(ED);
                }
                else
                {
                    SqlParameter ED = new SqlParameter("@EndDate", SqlDbType.VarChar, 1000);
                    ED.Value = DBNull.Value;
                    parameters.Add(ED);
                }

                if (IsRoomIdFilter)
                {
                    if (!string.IsNullOrEmpty(roomIDs))
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = roomIDs;
                        parameters.Add(rm);
                    }
                    else
                    {
                        SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                        rm.Value = DBNull.Value;
                        parameters.Add(rm);
                    }
                }
                else
                {
                    SqlParameter rm = new SqlParameter("@RoomIDs", SqlDbType.VarChar, 2048);
                    rm.Value = DBNull.Value;
                    parameters.Add(rm);
                }

                if (!string.IsNullOrEmpty(compIDs))
                {

                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 2048);
                    cm.Value = compIDs;
                    parameters.Add(cm);
                }
                else
                {
                    SqlParameter cm = new SqlParameter("@CompanyIDs", SqlDbType.VarChar, 1000);
                    cm.Value = DBNull.Value;
                    parameters.Add(cm);
                }
                if (UserID.GetValueOrDefault(0) > 0)
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = UserID;
                    parameters.Add(UID);
                }
                else
                {
                    SqlParameter UID = new SqlParameter("@UserID", SqlDbType.VarChar, 1000);
                    UID.Value = DBNull.Value;
                    parameters.Add(UID);
                }
                if (!string.IsNullOrEmpty(MoveType))
                {
                    SqlParameter PMoveType = new SqlParameter("@MoveType", SqlDbType.VarChar, 1000);
                    PMoveType.Value = MoveType;
                    parameters.Add(PMoveType);
                }
                else
                {
                    SqlParameter PMoveType = new SqlParameter("@MoveType", SqlDbType.VarChar, 1000);
                    PMoveType.Value = DBNull.Value;
                    parameters.Add(PMoveType);
                }
                if (!string.IsNullOrEmpty(itemIsActive))
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = itemIsActive;
                    parameters.Add(IAC);
                }
                else
                {
                    SqlParameter IAC = new SqlParameter("@ItemIsActive", SqlDbType.VarChar, 1000);
                    IAC.Value = DBNull.Value;
                    parameters.Add(IAC);
                }
                var ArrParams = parameters.ToArray();
                string strQuery1 = "Exec  " + SpName + "_ForSchedule @RangeFieldName,@StartDate,@EndDate,@RoomIDs,@CompanyIDs,@UserID,@ItemIsActive,@MoveType";

                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<RPT_MoveMaterialDTO>(strQuery1, ArrParams).ToList();
            }
        }
    }

}
