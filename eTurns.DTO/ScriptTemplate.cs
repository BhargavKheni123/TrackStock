using System.Collections.Generic;
using System.Web;


namespace eTurns.DTO
{
    public class ScriptTemplate
    {
        public System.Int64 ID { get; set; }
        public System.String Message { get; set; }
        public System.String EnterPriceDB { get; set; }
        public System.String Script { get; set; }
        public List<object> ResultList { get; set; }
        public List<object> ColumnList { get; set; }
        public bool IsUpdateDeleteQuery { get; set; }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public string SelectedDB { get; set; }
        public HttpPostedFileBase SQLFile { get; set; }

    }
}
