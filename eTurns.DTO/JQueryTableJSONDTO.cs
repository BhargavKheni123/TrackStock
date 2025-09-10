using System.Collections.Generic;

namespace eTurns.DTO
{
    //public class JQueryTableJSONDTO
    // {
    //    public string iCreate { get; set; }
    //    public string iStart { get; set; }
    //    public string iEnd { get; set; }
    //    public string iLength { get; set; }
    //  //  public string[] aaSorting { get; set; }

    //    public bool[] abVisCols { get; set; }
    //    public string[] ColWidth { get; set; }
    //    public int[] ColReorder { get; set; }
    // }
    public class OSearch
    {
        public bool bCaseInsensitive { get; set; }
        public string sSearch { get; set; }
        public bool bRegex { get; set; }
        public bool bSmart { get; set; }
    }

    public class AoSearchCol
    {
        public bool bCaseInsensitive { get; set; }
        public string sSearch { get; set; }
        public bool bRegex { get; set; }
        public bool bSmart { get; set; }
    }

    public class JQueryTableJSONDTO
    {
        public long iCreate { get; set; }
        public int iStart { get; set; }
        public int iEnd { get; set; }
        public int iLength { get; set; }
        public List<List<object>> aaSorting { get; set; }
        public OSearch oSearch { get; set; }
        public List<AoSearchCol> aoSearchCols { get; set; }
        public List<bool> abVisCols { get; set; }
        public List<string> ColWidth { get; set; }
        public List<int> ColReorder { get; set; }
    }
}
