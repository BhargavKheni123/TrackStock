using System;
using System.Globalization;

namespace eTurns.DTO
{
    public class FnCommon
    {
        public static string ConvertDateByTimeZone(DateTime? DateToConvert, bool ConvertAndFormat)
        {
            if (DateToConvert.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
            {
                DateTime? retdate = null;
                TimeZoneInfo DestinationTimeZone = TimeZoneInfo.Local;
                string DateTimeFormate = "MM/dd/yyyy hh:mm:ss tt";
                CultureInfo RoomCulture = new CultureInfo("en-US");

                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                {
                    if (System.Web.HttpContext.Current.Session["CurrentTimeZone"] != null && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentTimeZone"]))
                        && !string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentTimeZone"])))
                    {
                        DestinationTimeZone = TimeZoneInfo.FromSerializedString(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentTimeZone"]));
                    }                    

                    if (System.Web.HttpContext.Current.Session["DateTimeFormat"] != null && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session["DateTimeFormat"]))
                        && !string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["DateTimeFormat"])))
                    {
                        DateTimeFormate = (string)System.Web.HttpContext.Current.Session["DateTimeFormat"];
                    }

                    if (System.Web.HttpContext.Current.Session["RoomCulture"] != null && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session["RoomCulture"]))
                        && !string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["RoomCulture"])))
                    {
                        RoomCulture = (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"];
                    }                    
                }

                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);

                if (ConvertAndFormat)
                {
                    retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, DestinationTimeZone);
                }
                else
                {
                    retdate = DateToConvert;
                }

                return retdate.Value.ToString(DateTimeFormate, RoomCulture);
            }

            return string.Empty;
        }
        public static string GetToolTypeUsingEnum(string ToolTypeID)
        {
            if (!string.IsNullOrWhiteSpace(ToolTypeID))
            {
                string ToolType = string.Empty;
                char[] trimchar = new char[] { '\'' };
                ToolTypeID = (ToolTypeID ?? string.Empty).TrimStart(trimchar);
                string[] TypeId = ToolTypeID.Split(',');

                for (int cnt = 0; cnt < TypeId.Length; cnt++)
                {
                    if (!string.IsNullOrWhiteSpace(TypeId[cnt]))
                    {
                        if (string.IsNullOrWhiteSpace(ToolType))
                        {
                            if (Convert.ToInt64(TypeId[cnt]) == Convert.ToInt64(ToolTypeTracking.General))
                                ToolType = "General";
                            else if (Convert.ToInt64(TypeId[cnt]) == Convert.ToInt64(ToolTypeTracking.SerialType))
                                ToolType = "SerialType";
                        }
                        else
                        {
                            if (Convert.ToInt64(TypeId[cnt]) == Convert.ToInt64(ToolTypeTracking.General))
                                ToolType += ",General";
                            else if (Convert.ToInt64(TypeId[cnt]) == Convert.ToInt64(ToolTypeTracking.SerialType))
                                ToolType += ",SerialType";
                        }
                    }


                }

                return ToolType;
            }
            else
            {
                return "General";
            }
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
                else
                {
                    retdate = DateToConvert;
                }
                return retdate.Value.ToString(DateTimeFormate);
            }
            return string.Empty;
        }

        public static string ConvertDateByTimeZone(DateTime? DateToConvert, bool ConvertAndFormat, bool OnlyDate)
        {
            if (DateToConvert.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
            {
                DateTime? retdate = null;
                TimeZoneInfo DestinationTimeZone = TimeZoneInfo.Local;
                string DateTimeFormate = "MM/dd/yyyy hh:mm:ss tt";
                CultureInfo RoomCulture = new CultureInfo("en-US");

                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                {
                    if (System.Web.HttpContext.Current.Session["CurrentTimeZone"] != null && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentTimeZone"]))
                        && !string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentTimeZone"])))
                    {
                        DestinationTimeZone = TimeZoneInfo.FromSerializedString(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentTimeZone"]));
                    }

                    if (System.Web.HttpContext.Current.Session["DateTimeFormat"] != null
                        && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session["DateTimeFormat"]))
                        && !string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["DateTimeFormat"])) && !OnlyDate)
                    {
                        DateTimeFormate = (string)System.Web.HttpContext.Current.Session["DateTimeFormat"];
                    }                    
                    else if (System.Web.HttpContext.Current.Session["RoomDateFormat"] != null
                        && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session["RoomDateFormat"]))
                        && !string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["RoomDateFormat"]))
                        && OnlyDate)
                    {
                        DateTimeFormate = (string)System.Web.HttpContext.Current.Session["RoomDateFormat"];
                    }                    

                    if (System.Web.HttpContext.Current.Session["RoomCulture"] != null && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session["RoomCulture"]))
                        && !string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["RoomCulture"])))
                    {
                        RoomCulture = (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"];
                    }
                    
                }

                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);

                if (ConvertAndFormat)
                {
                    retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, DestinationTimeZone);
                }
                else
                {
                    retdate = DateToConvert;
                }

                return retdate.Value.ToString(DateTimeFormate, RoomCulture);
            }

            return string.Empty;
        }

        public static DateTime? ConvertDateByTimeZoneReturnDate(DateTime? DateToConvert, bool ConvertAndFormat, bool OnlyDate)
        {
            if (DateToConvert.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
            {
                DateTime? retdate = null;
                TimeZoneInfo DestinationTimeZone = TimeZoneInfo.Local;
                string DateTimeFormate = "MM/dd/yyyy hh:mm:ss tt";
                CultureInfo RoomCulture = new CultureInfo("en-US");

                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                {
                    if (System.Web.HttpContext.Current.Session["CurrentTimeZone"] != null)
                        DestinationTimeZone = TimeZoneInfo.FromSerializedString(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentTimeZone"]));


                    if (System.Web.HttpContext.Current.Session["DateTimeFormat"] != null && !OnlyDate)
                        DateTimeFormate = (string)System.Web.HttpContext.Current.Session["DateTimeFormat"];
                    else if (System.Web.HttpContext.Current.Session["RoomDateFormat"] != null && OnlyDate)
                        DateTimeFormate = (string)System.Web.HttpContext.Current.Session["RoomDateFormat"];

                    if (System.Web.HttpContext.Current.Session["RoomCulture"] != null)
                        RoomCulture = (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"];
                }

                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);

                if (ConvertAndFormat)
                {
                    retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, DestinationTimeZone);
                }
                else
                {
                    retdate = DateToConvert;
                }

                return retdate.Value;
            }

            return null;
        }
        /// <summary>
        /// GetOrderStatusChar
        /// </summary>
        /// <param name="ordStatus"></param>
        /// <returns></returns>
        public static char GetOrderStatusChar(OrderStatus ordStatus)
        {
            char OS = 'U';
            switch (ordStatus)
            {
                case OrderStatus.UnSubmitted:
                    OS = 'U';
                    break;
                case OrderStatus.Submitted:
                    OS = 'S';
                    break;
                case OrderStatus.Approved:
                    OS = 'A';
                    break;
                case OrderStatus.Transmitted:
                    OS = 'T';
                    break;
                case OrderStatus.TransmittedIncomplete:
                    OS = 'I';
                    break;
                case OrderStatus.TransmittedPastDue:
                    OS = 'P';
                    break;
                case OrderStatus.TransmittedInCompletePastDue:
                    OS = 'F';
                    break;
                case OrderStatus.Closed:
                    OS = 'C';
                    break;
                default:
                    OS = 'U';
                    break;
            }

            return OS;
        }

        /// <summary>
        /// GetOrderStatusChar
        /// </summary>
        /// <param name="ordStatus"></param>
        /// <returns></returns>
        public static char GetOrderStatusChar(int ordStatus)
        {
            char OS = 'U';
            switch ((OrderStatus)ordStatus)
            {
                case OrderStatus.UnSubmitted:
                    OS = 'U';
                    break;
                case OrderStatus.Submitted:
                    OS = 'S';
                    break;
                case OrderStatus.Approved:
                    OS = 'A';
                    break;
                case OrderStatus.Transmitted:
                    OS = 'T';
                    break;
                case OrderStatus.TransmittedIncomplete:
                    OS = 'I';
                    break;
                case OrderStatus.TransmittedPastDue:
                    OS = 'P';
                    break;
                case OrderStatus.TransmittedInCompletePastDue:
                    OS = 'P';
                    break;
                case OrderStatus.Closed:
                    OS = 'C';
                    break;
                default:
                    OS = 'U';
                    break;
            }

            return OS;
        }
        /// <summary>
        /// GetOrderStatusChar
        /// </summary>
        /// <param name="ordStatus"></param>
        /// <returns></returns>
        public static char GetToolAssetOrderStatusChar(int ordStatus)
        {
            char OS = 'U';
            switch ((ToolAssetOrderStatus)ordStatus)
            {
                case ToolAssetOrderStatus.UnSubmitted:
                    OS = 'U';
                    break;
                case ToolAssetOrderStatus.Submitted:
                    OS = 'S';
                    break;
                case ToolAssetOrderStatus.Approved:
                    OS = 'A';
                    break;
                case ToolAssetOrderStatus.Transmitted:
                    OS = 'T';
                    break;
                case ToolAssetOrderStatus.TransmittedIncomplete:
                    OS = 'I';
                    break;
                case ToolAssetOrderStatus.TransmittedPastDue:
                    OS = 'P';
                    break;
                case ToolAssetOrderStatus.TransmittedInCompletePastDue:
                    OS = 'P';
                    break;
                case ToolAssetOrderStatus.Closed:
                    OS = 'C';
                    break;
                default:
                    OS = 'U';
                    break;
            }

            return OS;
        }
        /// <summary>
        /// GetOrderStatusChar
        /// </summary>
        /// <param name="ordStatus"></param>
        /// <returns></returns>
        public static char GetToolAssetOrderStatusChar(string ToolAssetordStatus)
        {
            char OS = 'U';
            switch (ToolAssetordStatus.ToLower())
            {
                case "unsubmitted":
                    OS = 'U';
                    break;
                case "submitted":
                    OS = 'S';
                    break;
                case "approved":
                    OS = 'A';
                    break;
                case "transmitted":
                    OS = 'T';
                    break;
                case "transmittedincomplete":
                    OS = 'I';
                    break;
                case "incomplete":
                    OS = 'I';
                    break;
                case "transmittedpastdue":
                    OS = 'P';
                    break;
                case "pastdue":
                    OS = 'P';
                    break;
                case "transmittedincompletepastdue":
                    OS = 'F';
                    break;
                case "incompletepastdue":
                    OS = 'F';
                    break;
                case "closed":
                    OS = 'C';
                    break;
                default:
                    OS = 'U';
                    break;
            }

            return OS;
        }
        /// <summary>
        /// GetOrderStatusChar
        /// </summary>
        /// <param name="ordStatus"></param>
        /// <returns></returns>
        public static char GetOrderStatusChar(string ordStatus)
        {
            char OS = 'U';
            switch (ordStatus.ToLower())
            {
                case "unsubmitted":
                    OS = 'U';
                    break;
                case "submitted":
                    OS = 'S';
                    break;
                case "approved":
                    OS = 'A';
                    break;
                case "transmitted":
                    OS = 'T';
                    break;
                case "transmittedincomplete":
                    OS = 'I';
                    break;
                case "incomplete":
                    OS = 'I';
                    break;
                case "transmittedpastdue":
                    OS = 'P';
                    break;
                case "pastdue":
                    OS = 'P';
                    break;
                case "transmittedincompletepastdue":
                    OS = 'F';
                    break;
                case "incompletepastdue":
                    OS = 'F';
                    break;
                case "closed":
                    OS = 'C';
                    break;
                default:
                    OS = 'U';
                    break;
            }

            return OS;
        }

        public static char GetQuoteStatusChar(int ordStatus)
        {
            char OS = 'U';
            switch ((OrderStatus)ordStatus)
            {
                case OrderStatus.UnSubmitted:
                    OS = 'U';
                    break;
                case OrderStatus.Submitted:
                    OS = 'S';
                    break;
                case OrderStatus.Approved:
                    OS = 'A';
                    break;
                case OrderStatus.Transmitted:
                    OS = 'T';
                    break;
                case OrderStatus.TransmittedIncomplete:
                    OS = 'I';
                    break;
                case OrderStatus.TransmittedPastDue:
                    OS = 'P';
                    break;
                case OrderStatus.TransmittedInCompletePastDue:
                    OS = 'P';
                    break;
                case OrderStatus.Closed:
                    OS = 'C';
                    break;
                default:
                    OS = 'U';
                    break;
            }

            return OS;
        }
        public static char GetQuoteStatusChar(string ordStatus)
        {
            char OS = 'U';
            switch (ordStatus.ToLower())
            {
                case "unsubmitted":
                    OS = 'U';
                    break;
                case "submitted":
                    OS = 'S';
                    break;
                case "approved":
                    OS = 'A';
                    break;
                case "transmitted":
                    OS = 'T';
                    break;
                case "transmittedincomplete":
                    OS = 'I';
                    break;
                case "incomplete":
                    OS = 'I';
                    break;
                case "transmittedpastdue":
                    OS = 'P';
                    break;
                case "pastdue":
                    OS = 'P';
                    break;
                case "transmittedincompletepastdue":
                    OS = 'F';
                    break;
                case "incompletepastdue":
                    OS = 'F';
                    break;
                case "closed":
                    OS = 'C';
                    break;
                default:
                    OS = 'U';
                    break;
            }

            return OS;
        }

    }
}
