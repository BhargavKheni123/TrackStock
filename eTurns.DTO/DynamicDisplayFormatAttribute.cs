using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace eTurns.DTO
{
    public class DynamicDisplayFormatAttribute : DisplayFormatAttribute
    {
        public DynamicDisplayFormatAttribute()
        {
            string defaultFormate = "{0:MM/dd/yyyy}";
            if (HttpContext.Current.Session["RoomDateFormat"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(HttpContext.Current.Session["RoomDateFormat"])))
            {
                defaultFormate = "{0:" + Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]) + "}";
            }
            DataFormatString = defaultFormate;
        }
    }
}