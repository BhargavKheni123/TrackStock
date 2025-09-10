using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace eTurns.DAL
{
    public static class DateTimeUtility
    {
        public static DateTime DateTimeNow { get { return DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local); } }

        public static DateTime? ConvertLocalDateTimeToUTCDateTime(DateTime? targetDate)
        {
            if (targetDate.HasValue && targetDate.Value != DateTime.MinValue)
            {
                //TimeZoneInfo.ConvertTimeToUtc(targetDate.Value,)

                TimeZoneInfo easternZone = TimeZoneInfo.Utc;
                return TimeZoneInfo.ConvertTime(targetDate.Value, easternZone, TimeZoneInfo.Utc);
            }
            return null;
        }

        public static string ConvertDateByTimeZone(DateTime? DateToConvert, TimeZoneInfo DestinationTimeZone, string DateTimeFormate, CultureInfo RoomCulture, bool ConvertAndFormat)
        {
            DateTime? retdate = null;
            if (DateToConvert.HasValue && DateToConvert.Value != DateTime.MinValue)
            {
                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);
                if (ConvertAndFormat)
                {
                    retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, DestinationTimeZone);
                }
                return retdate.Value.ToString(DateTimeFormate);
            }
            return string.Empty;
        }

        public static string ConvertDateByTimeZone(DateTime? DateToConvert, string TimeZoneName, string DateTimeFormate, string CultureCode, bool ConvertAndFormat)
        {
            DateTime? retdate = null;
            CultureInfo roomculture = CultureInfo.CreateSpecificCulture(CultureCode);
            TimeZoneInfo roomTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            if (DateToConvert.HasValue && DateToConvert.Value != DateTime.MinValue)
            {
                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);
                if (ConvertAndFormat)
                {
                    retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, roomTimeZone);
                }
                else
                {
                    retdate = DateToConvert;
                }
                return retdate.Value.ToString(DateTimeFormate);
            }
            return string.Empty;
        }

        public static DateTime ConvertDateFromRoomTimeZoneToUTC(DateTime DateToConvert)
        {
            DateToConvert = DateTime.SpecifyKind(DateToConvert, DateTimeKind.Unspecified);
            return DateToConvert;
        }

        public static DateTime? ConvertDateByTimeZonedt(DateTime? DateToConvert, TimeZoneInfo DestinationTimeZone, string DateTimeFormate, CultureInfo RoomCulture)
        {

            DateTime? retdate = null;
            if (DateToConvert.HasValue && DateToConvert.Value != DateTime.MinValue)
            {
                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);
                retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, DestinationTimeZone);
                return retdate;
            }
            return null;
        }

        public static DateTime GetCurrentDatetimeByTimeZone(string TimeZoneID)
        {
            TimeZoneInfo DestTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            DateTime dataTimeByZoneId = System.TimeZoneInfo.ConvertTime(DateTime.UtcNow, System.TimeZoneInfo.Utc, DestTimeZone);
            return dataTimeByZoneId;
        }
        public static DateTime ConvertDateToUTC(string TimeZoneID, DateTime DateToConvert)
        {
            TimeZoneInfo SourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            return TimeZoneInfo.ConvertTimeToUtc(DateToConvert, SourceTimeZone);
        }
        public static DateTime ConvertDateFromUTC(string TimeZoneID, DateTime UTCDAte)
        {
            TimeZoneInfo DestiNationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(UTCDAte, DateTimeKind.Utc), DestiNationTimeZone);

        }

        public static DateTime GetNewReceivedDate(DateTime currentDateAsPerRoom, string strDate, string _roomDateFormat, CultureInfo _roomCulture, TimeZoneInfo _currentTimeZone, string _TimeFormat)
        {
            DateTime newReceiveTempDate = DateTime.UtcNow;
            DateTime dtRcvTemp;

            try
            {
                if (!string.IsNullOrWhiteSpace(strDate))
                {
                    DateTime.TryParseExact(strDate, _roomDateFormat, _roomCulture, System.Globalization.DateTimeStyles.None, out dtRcvTemp);
                    if (currentDateAsPerRoom.Date != dtRcvTemp.Date)
                    {
                        DateTime newTempDate = currentDateAsPerRoom;
                        string timeOfDay = currentDateAsPerRoom.ToString(_TimeFormat);
                        DateTime.TryParseExact(strDate + " " + timeOfDay, _roomDateFormat + " " + _TimeFormat, _roomCulture, System.Globalization.DateTimeStyles.None, out newTempDate);
                        newReceiveTempDate = TimeZoneInfo.ConvertTimeToUtc(newTempDate, _currentTimeZone);
                    }
                }
            }
            catch
            {


            }


            return newReceiveTempDate;
        }

    }


    public static class CommonUtilityHelper
    {
        public static bool CheckUDFIsRequired(IEnumerable<UDFDTO> DataFromDB, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, out string Reason,long EnterpriseID,long CompanyID,long RoomID,string currentCulture, string forCheckout = null, string UDF6 = "", string UDF7 = "", string UDF8 = "", string UDF9 = "", string UDF10 = "")
        {
            bool isRequired = false;
            Reason = string.Empty;
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            var ResMessageFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", currentCulture, EnterpriseID, CompanyID);
            string MsgRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResMessage", currentCulture);
            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                foreach (var i in DataFromDB)
                {
                    if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(UDF1))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(UDF2))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(UDF3))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(UDF4))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(UDF5))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));

                    if (i.UDFColumnName == "UDF6" && string.IsNullOrEmpty(UDF6))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF7" && string.IsNullOrEmpty(UDF7))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF8" && string.IsNullOrEmpty(UDF8))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF9" && string.IsNullOrEmpty(UDF9))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF10" && string.IsNullOrEmpty(UDF10))
                        Reason = string.Format(MsgRequired, Reason + " " + (forCheckout ?? string.Empty) + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                }

                if (!string.IsNullOrEmpty(Reason))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }
        public static bool CheckUDFIsRequired_Asset(IEnumerable<UDFDTO> DataFromDB, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string UDF6, string UDF7, string UDF8, string UDF9, string UDF10, out string Reason,long EnterpriseID, long CompanyID, long RoomID, string currentCulture)
        {
            bool isRequired = false;
            Reason = string.Empty;
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            var ResMessageFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", currentCulture, EnterpriseID, CompanyID);
            string MsgRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResMessage", currentCulture);
            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                foreach (var i in DataFromDB)
                {
                    if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(UDF1))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName)); 
                    if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(UDF2))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(UDF3))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(UDF4))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(UDF5))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));

                    if (i.UDFColumnName == "UDF6" && string.IsNullOrEmpty(UDF6))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF7" && string.IsNullOrEmpty(UDF7))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF8" && string.IsNullOrEmpty(UDF8))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF9" && string.IsNullOrEmpty(UDF9))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                    if (i.UDFColumnName == "UDF10" && string.IsNullOrEmpty(UDF10))
                        Reason = string.Format(MsgRequired, Reason + " " + GetColumnNameFromResource(i.UDFColumnName, i.UDFTableName));
                }

                if (!string.IsNullOrEmpty(Reason))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }


        private static string GetColumnNameFromResource(string UDFColumnName, string UDFTableName, bool isUDFName = true, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false)
        {
            return eTurns.DTO.Resources.ResourceHelper.ColumnNameFromResource(UDFColumnName, UDFTableName, isUDFName, OtherFromeTurns: OtherFromeTurns, ForEnterPriseSetup: ForEnterPriseSetup);
        }


        public static bool ValidateStrongPassword(string strPassword, out string message,string strCulture,long EnterpriseID,long CompanyID,long RoomID)
        {
            bool isStrong = true;
            message = string.Empty;

            string patdi = @"\d+"; //match digits
            string patupp = @"[A-Z]+"; //match upper cases
            string patlow = @"[a-z]+"; //match lower cases
            string patsym = @"[`~!@$#%^&\\-\\+*/_=,;.':|]+";
            Match id = Regex.Match(strPassword, patdi);
            Match upp = Regex.Match(strPassword, patupp);
            Match low = Regex.Match(strPassword, patlow);
            Match sym = Regex.Match(strPassword, patsym);
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            var ResMessageFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResUserMaster", strCulture, EnterpriseID, CompanyID);
            string MsgCharacterLength = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCharacterLength", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResUserMaster", strCulture);
            string MsgAtleastOneNumberRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgAtleastOneNumberRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResUserMaster", strCulture);
            string MsgAtleastOneCapitalRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgAtleastOneCapitalRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResUserMaster", strCulture);
            string MsgAtleasetOneLetterRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgAtleasetOneLetterRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResUserMaster", strCulture);
            string MsgAtleastOneSpecialLetterRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgAtleastOneSpecialLetterRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResUserMaster", strCulture);
            if ((strPassword ?? string.Empty).Length < 8)
            {
                message += MsgCharacterLength;
            }
            if (!id.Success)
            {
                message += Environment.NewLine + MsgAtleastOneNumberRequired;
            }
            if (!upp.Success)
            {
                message += Environment.NewLine + MsgAtleastOneCapitalRequired;
            }
            if (!low.Success)
            {
                message += Environment.NewLine + MsgAtleasetOneLetterRequired;
            }
            if (!sym.Success)
            {
                message += Environment.NewLine + MsgAtleastOneSpecialLetterRequired;
            }

            if (string.IsNullOrWhiteSpace(message))
                isStrong = true;
            else
                isStrong = false;

            return isStrong;

        }


        public static List<T> ConvertDataTable<T>(DataTable dt) where T : class, new()
        {
            List<T> lstItems = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                    lstItems.Add(ConvertDataRowToGenericType<T>(row));
            else
                lstItems = null;
            return lstItems;
        }

        private static T ConvertDataRowToGenericType<T>(DataRow row) where T : class, new()
        {
            Type entityType = typeof(T);
            T objEntity = new T();
            foreach (DataColumn column in row.Table.Columns)
            {
                object value = row[column.ColumnName];
                if (value == DBNull.Value) value = null;
                PropertyInfo property = entityType.GetProperty(column.ColumnName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                try
                {
                    if (property != null && property.CanWrite)
                        property.SetValue(objEntity, value, null);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objEntity;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
