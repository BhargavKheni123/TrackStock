using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eTurnsWeb.BAL
{
    public class MasterBAL : IDisposable
    {
        public string getGridState(string ListName)
        {
            //string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]],""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}],""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1,3,2,4]}";
            //UsersUISettingsController obj = new UsersUISettingsController();
            eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
            UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
            objDTO = obj.GetRecord(SessionHelper.UserID, ListName, SiteSettingHelper.UsersUISettingType, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
            string jsonData = "";
            if (objDTO != null && !string.IsNullOrEmpty(objDTO.JSONDATA))
            {
                jsonData = objDTO.JSONDATA;
            }
            else
            {
                var isLoadEnterpriseGridOrdering = eTurnsWeb.Helper.CommonUtility.GetIsLoadEnterpriseGridOrdering();

                if (isLoadEnterpriseGridOrdering)
                {
                    SiteListMasterDAL siteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
                    string siteListMasterJson = siteListMasterDAL.GetSiteListMasterDataByListName(ListName, SiteSettingHelper.UsersUISettingType);

                    if (!string.IsNullOrEmpty(siteListMasterJson))
                    {
                        jsonData = siteListMasterJson;
                        eTurns.DAL.UDFDAL obj1 = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                        var tmpList = ListName;
                        if (tmpList.ToLower() == "toollistnew")
                        {
                            tmpList = "ToolList";
                        }

                        if (tmpList == "PullMaster" || tmpList == "ItemsListForNewReceive" || tmpList == "ToolHistoryList"
                            || tmpList == "AssetToolSchedulerList" || tmpList == "KitToolMasterList" || tmpList == "EnterpriseList"
                            || tmpList == "BomCategoryMasterList" || tmpList == "BomGLAccountMasterList"
                            || tmpList == "BomInventoryClassificationMasterList"
                            || tmpList == "BomManufacturerMasterList" || tmpList == "BomSupplierMasterList"
                            || tmpList == "UnitMasterList" || tmpList == "BomUnitMasterList" || tmpList == "MinMaxTuningTable")
                        {
                            var udfTables = GetUDFTableNamesByListName(tmpList);
                            int totalUDFCount = 0;

                            foreach (var tableName in udfTables)
                            {
                                totalUDFCount += obj1.GetNonDeletedUDFCountByUDFTableName(tableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                            }

                            if (totalUDFCount > 0)
                            {
                                jsonData = UpdateJson(siteListMasterJson, ListName, totalUDFCount);
                            }
                        }
                        else
                        {
                            eTurnsMaster.DAL.CommonMasterDAL objUDF = new eTurnsMaster.DAL.CommonMasterDAL();
                            Dictionary<int, string> GridListName = objUDF.GetUDfTableNameByListName(tmpList);
                            int totalUDFCount = 0;

                            if (GridListName != null && GridListName.Count() > 0)
                            {
                                foreach (KeyValuePair<int, string> ReportResourceFileName in GridListName)
                                {
                                    string[] Values = ReportResourceFileName.Value.Split('$');
                                    if (Values != null && Values.Count() > 0)
                                    {
                                        int udfCount = 0;
                                        //IEnumerable<UDFDTO> UDFDataFromDB = null;
                                        //int TotalRecordCount = 0;
                                        udfCount = obj1.GetNonDeletedUDFCountByUDFTableName(Values[0], SessionHelper.RoomID, SessionHelper.CompanyID);
                                        //UDFDataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, Values[0], SessionHelper.RoomID);
                                        //if (UDFDataFromDB != null && UDFDataFromDB.Count() > 0)
                                        //{
                                        //    udfCount = UDFDataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                                        //}

                                        if (tmpList == "CartItemList" || tmpList == "ReceiveMasterList")
                                        {
                                            Values[1] = "Yes";
                                            Values[2] = "ItemMaster";
                                        }

                                        int ExtraUDFinGrid = 0;

                                        if (Values[1] == "Yes")
                                        {
                                            ExtraUDFinGrid = obj1.GetNonDeletedUDFCountByUDFTableName(Values[2], SessionHelper.RoomID, SessionHelper.CompanyID);
                                            //IEnumerable<UDFDTO> DataFromDB = null;
                                            //int TotalRecordCountExtra = 0;
                                            //DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCountExtra, string.Empty, "ID asc", SessionHelper.CompanyID, Values[2], SessionHelper.RoomID);
                                            //if (DataFromDB != null && DataFromDB.Count() > 0)
                                            //{
                                            //    ExtraUDFinGrid = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                                            //}
                                        }
                                        totalUDFCount += udfCount + ExtraUDFinGrid + Convert.ToInt32(Values[3]);

                                        if (totalUDFCount > 0)
                                        {
                                            jsonData = UpdateJson(siteListMasterJson, ListName, totalUDFCount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return jsonData;
        }

        public GridState GetGridStateObj(string listName)
        {
            string gridState = getGridState(listName);
            GridState gridSateObj = JsonConvert.DeserializeObject<GridState>(gridState);
            return gridSateObj;
        }

        public static List<string> GetUDFTableNamesByListName(string ListName)
        {
            List<string> udfTables = new List<string>();
            switch (ListName.ToLower())
            {
                case "pullmaster":
                    udfTables.Add("PullMaster");
                    udfTables.Add("RequisitionMaster");
                    udfTables.Add("WorkOrder");
                    break;
                case "itemslistfornewreceive":
                    udfTables.Add("ItemMaster");
                    break;
                case "toolhistorylist":
                    udfTables.Add("ToolMaster");
                    break;
                case "assettoolschedulerlist":
                    udfTables.Add("SchedulerMaster");
                    break;
                case "kittoolmasterlist":
                    udfTables.Add("ToolMaster");
                    udfTables.Add("ToolCheckInOutHistory");
                    break;
                case "enterpriselist":
                    udfTables.Add("Enterprise");
                    break;
                case "bomcategorymasterlist":
                    udfTables.Add("CategoryMaster");
                    break;
                case "bomglaccountmasterlist":
                    udfTables.Add("GLAccountMaster");
                    break;
                case "bominventoryclassificationmasterlist":
                    udfTables.Add("InventoryClassificationMaster");
                    break;
                case "bommanufacturermasterlist":
                    udfTables.Add("ManufacturerMaster");
                    break;
                case "bomsuppliermasterlist":
                    udfTables.Add("SupplierMaster");
                    break;
                case "unitmasterlist":
                case "bomunitmasterlist":
                    udfTables.Add("UnitMaster");
                    break;
                case "minmaxtuningtable":
                    udfTables.Add("ItemMaster");
                    break;
            }
            return udfTables;
        }

        /// <summary>
        /// This method is used to update grid json by adding grid's UDF
        /// </summary>
        /// <param name="Json"></param>
        /// <param name="ListName"></param>
        /// <param name="UDFCount"></param>
        /// <returns></returns>
        private string UpdateJson(string Json, string ListName, int UDFCount)
        {
            string updatedJSON = string.Empty;

            if (!string.IsNullOrEmpty(Json))
            {
                if (!string.IsNullOrEmpty(Json))
                {
                    JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();
                    // jsonData = objDTO.JSONDATA;
                    /*////////CODE FOR UPDATE JSON STRING/////////*/
                    // JObject gridStaeJS = new JObject();
                    gridStateJS = JObject.Parse(Json);
                    /*////////CODE FOR UPDATE JSON STRING/////////*/

                    JToken orderCols = gridStateJS["ColReorder"];
                    JArray arrOCols = (JArray)orderCols;
                    JArray arrONewCols = new JArray();

                    if (arrOCols != null)
                    {
                        int orderClength = arrOCols.Count;

                        if (orderClength > 4)
                        {
                            JToken abVisCols = gridStateJS["abVisCols"];
                            JArray visCols = (JArray)abVisCols;
                            JToken aoSearchCols = gridStateJS["aoSearchCols"];
                            JArray arrSCols = (JArray)aoSearchCols;

                            if (arrSCols != null)
                            {
                                JObject UpdateAccProfile = new JObject(
                                        new JProperty("bCaseInsensitive", true),
                                        new JProperty("sSearch", ""),
                                        new JProperty("bRegex", false),
                                        new JProperty("bSmart", true));
                                for (int count = 0; count < UDFCount; count++)
                                {
                                    arrSCols.Add((object)UpdateAccProfile);
                                }
                            }

                            if (visCols != null)
                            {
                                for (int count = 0; count < UDFCount; count++)
                                {
                                    visCols.Add(true);
                                }
                            }

                            JToken widthCols = gridStateJS["ColWidth"];
                            JArray arrWCols = (JArray)widthCols;

                            if (arrWCols != null)
                            {
                                for (int count = 0; count < UDFCount; count++)
                                {
                                    arrWCols.Insert(arrWCols.Count, "100px");
                                }
                            }

                            int maxOrder = arrOCols.Select(c => (int)c).ToList().Max();
                            long currentUDFVAl = maxOrder + 1;

                            for (int count = 0; count < UDFCount; count++)
                            {
                                arrOCols.Insert(arrOCols.Count, currentUDFVAl + count);
                            }

                            gridStateJS["ColReorder"] = arrOCols;
                            updatedJSON = gridStateJS.ToString();

                            /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                            //objDTO = new UsersUISettingsDTO();
                            //objDTO.UserID = SessionHelper.UserID;

                            //objDTO.EnterpriseID = SessionHelper.EnterPriceID;
                            //objDTO.CompanyID = SessionHelper.CompanyID;
                            //objDTO.RoomID = SessionHelper.RoomID;

                            //objDTO.JSONDATA = updatedJSON;
                            //objDTO.ListName = ListName;
                            //obj.SaveUserListViewSettings(objDTO);
                            /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                        }
                    }
                }
            }
            return updatedJSON;
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MasterBAL()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }// class


    public class GridState
    {
        public long iCreate { get; set; }

        /// <summary>
        ///  Page Start index
        /// </summary>
        public int iStart { get; set; }

        /// <summary>
        /// Pae end index
        /// </summary>
        public int iEnd { get; set; }

        /// <summary>
        /// Page length
        /// </summary>
        public int iLength { get; set; }

        public List<GridSortCol> aaSorting { get; set; }

        //public JObject oSearch { get; set; }

        //public JArray aoSearchCols { get; set; }

        //public List<bool> abVisCols { get; set; }

        //public List<string> ColWidth { get; set; }

        public List<int> ColReorder { get; set; }
    }

    public class GridSortCol
    {
        public int Col1Index { get; set; }
        public string Col1Sort { get; set; }
        public int Col2Index { get; set; }
        public string Col2Sort { get; set; }
    }

} // ns