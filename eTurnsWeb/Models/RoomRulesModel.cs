using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eTurnsWeb.Models
{
    public class RoomRulesModel
    {
        public long ID { get; set; }
        public SelectList ModuleList { get; set; }
    }
}