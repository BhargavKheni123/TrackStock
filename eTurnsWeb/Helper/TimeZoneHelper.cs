using System;
using System.Globalization;

namespace eTurnsWeb.Helper
{
    public static class DatetimeHelper
    {
        /// <summary>
        /// Gets the TimeZoneInfo.AdjustmentRule in effect for the given year.
        /// </summary>
        /// <param name="timeZoneInfo"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static TimeZoneInfo.AdjustmentRule GetAdjustmentRuleForYear(this TimeZoneInfo timeZoneInfo, int year)
        {
            TimeZoneInfo.AdjustmentRule[] adjustments = timeZoneInfo.GetAdjustmentRules();
            // Iterate adjustment rules for time zone 
            foreach (TimeZoneInfo.AdjustmentRule adjustment in adjustments)
            {
                // Determine if this adjustment rule covers year desired 
                if (adjustment.DateStart.Year <= year && adjustment.DateEnd.Year >= year)
                {
                    return adjustment;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the Daylight Savings Time start date for the given year.
        /// </summary>
        /// <param name="adjustmentRule"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetDaylightTransitionStartForYear(this TimeZoneInfo.AdjustmentRule adjustmentRule, int year)
        {
            return adjustmentRule.DaylightTransitionStart.GetDateForYear(year);
        }

        /// <summary>
        /// Gets the Daylight Savings Time end date for the given year.
        /// </summary>
        /// <param name="adjustmentRule"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetDaylightTransitionEndForYear(this TimeZoneInfo.AdjustmentRule adjustmentRule, int year)
        {
            return adjustmentRule.DaylightTransitionEnd.GetDateForYear(year);
        }

        /// <summary>
        /// Gets the date of the transition for the given year.
        /// </summary>
        /// <param name="transitionTime"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetDateForYear(this TimeZoneInfo.TransitionTime transitionTime, int year)
        {
            if (transitionTime.IsFixedDateRule)
            {
                return GetFixedDateRuleDate(transitionTime, year);
            }
            else
            {
                return GetFloatingDateRuleDate(transitionTime, year);
            }
        }

        private static DateTime GetFixedDateRuleDate(TimeZoneInfo.TransitionTime transitionTime, int year)
        {
            return new DateTime(year,
                               transitionTime.Month,
                               transitionTime.Day,
                               transitionTime.TimeOfDay.Hour,
                               transitionTime.TimeOfDay.Minute,
                               transitionTime.TimeOfDay.Second,
                               DateTimeKind.Unspecified);
        }

        private static DateTime GetFloatingDateRuleDate(TimeZoneInfo.TransitionTime transitionTime, int year)
        {
            // For non-fixed date rules, get local calendar
            Calendar localCalendar = CultureInfo.CurrentCulture.Calendar;

            // Get first day of week for transition
            // For example, the 3rd week starts no earlier than the 15th of the month
            int startOfWeek = transitionTime.Week * 7 - 6;

            // What day of the week does the month start on?
            int firstDayOfWeek = (int)localCalendar.GetDayOfWeek(new DateTime(year, transitionTime.Month, 1));

            // Determine how much start date has to be adjusted
            int transitionDay;
            int changeDayOfWeek = (int)transitionTime.DayOfWeek;
            if (firstDayOfWeek <= changeDayOfWeek)
                transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
            else
                transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);

            // Adjust for months with no fifth week
            if (transitionDay > localCalendar.GetDaysInMonth(year, transitionTime.Month))
                transitionDay -= 7;

            return new DateTime(year,
                       transitionTime.Month,
                       transitionDay,
                       transitionTime.TimeOfDay.Hour,
                       transitionTime.TimeOfDay.Minute,
                       transitionTime.TimeOfDay.Second,
                       DateTimeKind.Unspecified);
        }
    }
}