using eTurnsWeb.Controllers;
using eTurnsWeb.Helper;
using System;
using System.Web.Mvc;


namespace eTurnsWeb
{
    public class ExportController : eTurnsControllerBase
    {
        //
        // GET: /Default1/ExportModuleInfo
        [HttpPost]
        public JsonResult ExportModuleInfo(string ExportModuleName, string Ids, string Type, string SortNameString, bool? IsDeleted, string TableName = "", string CallFromPage = "", string BinIds = "")
        {
            string path = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(SortNameString) || SortNameString == "undefine" || SortNameString == "0" || SortNameString.Trim().Contains("null") || string.IsNullOrEmpty(SortNameString.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                    SortNameString = "ID asc";
                else
                    SortNameString = SortNameString + ", ID asc";

                if (Type == "Excel")
                {
                    eTurnsExcellHelper objExcel = new eTurnsExcellHelper();
                    path = objExcel.ExcelMain(Server.MapPath("~/Downloads/"), ExportModuleName, Ids, SortNameString, BinIds);
                }
                else if (Type == "CSV")
                {
                    eTurnsCSVHelper objCSV = new eTurnsCSVHelper();
                    path = objCSV.CSVlMain(Server.MapPath("~/Downloads/"), ExportModuleName, Ids, SortNameString, IsDeleted, TableName, CallFromPage, BinIds);
                    if (!path.ToLower().Contains(".csv"))
                    {
                        return Json(new
                        {
                            rStatus = "Fail",
                            rMessage = path
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (Type == "ItemLocationCSV")
                {
                    eTurnsCSVHelper objCSV = new eTurnsCSVHelper();
                    path = objCSV.CSVlMain(Server.MapPath("~/Downloads/"), "ItemLocationCSV", Ids, SortNameString, IsDeleted, string.Empty, string.Empty, BinIds);
                }
                else if (Type == "ItemLocationQtyCSV")
                {
                    eTurnsCSVHelper objCSV = new eTurnsCSVHelper();
                    path = objCSV.CSVlMain(Server.MapPath("~/Downloads/"), "ItemLocationQtyCSV", Ids, SortNameString, IsDeleted, string.Empty, string.Empty, BinIds);
                }
                else if (Type == "KitsCSV")
                {
                    eTurnsCSVHelper objCSV = new eTurnsCSVHelper();
                    path = objCSV.CSVlMain(Server.MapPath("~/Downloads/"), "KitsCSV", Ids, SortNameString, IsDeleted, string.Empty, string.Empty, BinIds);
                }


                // CreatingCsvFiles();



            }
            catch (Exception ex)
            { }
            return Json(path);
        }

        [HttpPost]
        public JsonResult ExportEnterpriseUserList(string EntId)
        {
            string path = string.Empty;
            try
            {
                eTurnsExcellHelper objExcel = new eTurnsExcellHelper();
                path = objExcel.EnterpriseUserList(Server.MapPath("~/Downloads/"), EntId);
            }
            catch (Exception)
            { }
            return Json(path);
        }
    }
}

