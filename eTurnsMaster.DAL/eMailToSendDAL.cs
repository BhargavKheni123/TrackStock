using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public class eMailToSendDAL : eTurnsMasterBaseDAL
    {
        public List<eMailToSendListDTO> GetSendEmailPagedRecords(string eTurnsEmailsDBName, Int32 StartRowIndex, Int32 MaxRows, out Int64 TotalCount, string SearchTerm, string sortColumnName, Int64 LoggedInUserID, string strActionFilter, string RoomDateFormat)
        {

            string CreatedDateFrom = null, CreatedDateTo = null, HistoryDateFrom = null, HistoryDateTo = null;
            string strEnterpriseIDs = null, strCompanyIDs = null, strRoomIDs = null;

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

                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = (DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult)).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = (DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399)).ToString("dd-MM-yyyy HH:mm:ss");
                    //CreatedDateFrom = (DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                    //CreatedDateTo = (DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    HistoryDateFrom = (DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult)).ToString("dd-MM-yyyy HH:mm:ss");
                    HistoryDateTo = (DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399)).ToString("dd-MM-yyyy HH:mm:ss");
                }

                string filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[40]))
                {
                    string[] value = FieldsPara[40].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                strRoomIDs = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[42]))
                {
                    string[] value = FieldsPara[42].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                strCompanyIDs = filter;
                filter = string.Empty;
                if (!string.IsNullOrEmpty(FieldsPara[79]))
                {
                    string[] value = FieldsPara[79].Split(',');

                    if (value != null && value.Length > 0)
                    {
                        foreach (var item in value)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += ",";
                                }
                                filter += item;
                            }
                        }
                    }
                }
                strEnterpriseIDs = filter;

            }

            List<eMailToSendListDTO> lstSendEmail = new List<eMailToSendListDTO>();
            var params1 = new SqlParameter[] {
                  new SqlParameter("@StartRowIndex", StartRowIndex)
                , new SqlParameter("@MaxRows", MaxRows)
                , new SqlParameter("@SearchTerm", SearchTerm ??(object)DBNull.Value)
                , new SqlParameter("@sortColumnName", sortColumnName ??(object)DBNull.Value)
                , new SqlParameter("@CreatedDateFrom", CreatedDateFrom ??(object)DBNull.Value)
                , new SqlParameter("@CreatedDateTo", CreatedDateTo ??(object)DBNull.Value)
                , new SqlParameter("@HistoryDateFrom", HistoryDateFrom ??(object)DBNull.Value)
                , new SqlParameter("@HistoryDateTo", HistoryDateTo ??(object)DBNull.Value)
                , new SqlParameter("@EnterpriseIDs", strEnterpriseIDs  ??(object)DBNull.Value)
                , new SqlParameter("@RoomIDs", strRoomIDs  ??(object)DBNull.Value)
                , new SqlParameter("@CompanyIDs", strCompanyIDs  ??(object)DBNull.Value)
                , new SqlParameter("@LoggedinUserID", LoggedInUserID)
                , new SqlParameter("@ActionFilter", strActionFilter  ??(object)DBNull.Value)


            };
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))

            {
                lstSendEmail = context.Database.SqlQuery<eMailToSendListDTO>("EXEC " + eTurnsEmailsDBName + " .dbo.[GetSendEmailPagedRecords] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedDateFrom,@CreatedDateTo,@HistoryDateFrom,@HistoryDateTo,@EnterpriseIDs,@CompanyIDs,@RoomIDs,@LoggedinUserID,@ActionFilter", params1).ToList();
            }
            if (lstSendEmail != null && lstSendEmail.Count > 0)
            {
                TotalCount = lstSendEmail.First().TotalRecords;
            }
            else
            {
                TotalCount = 0;
            }
            return lstSendEmail;
        }

        public eMailToSendListDTO GeteMailToSendHistoryData(string eTurnsEmailsDBName, long HistoryID, string strActionFilter)
        {
            var params1 = new SqlParameter[] {
                  new SqlParameter("@HistoryID", HistoryID)
                , new SqlParameter("@ActionFilter", strActionFilter  ??(object)DBNull.Value)

            };
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                eMailToSendListDTO objDTO = context.Database.SqlQuery<eMailToSendListDTO>("EXEC " + eTurnsEmailsDBName + " .dbo.[GeteMailToSendHistoryByHistoryID] @HistoryID,@ActionFilter", params1).FirstOrDefault();
                return objDTO;
            }
        }
    }
}
