using eTurns.DTO;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTurnsWeb.BAL
{
    public class UserNarrowSearchSettingsBAL : IDisposable
    {
        private bool disposedValue;

              public UserNarrowSearchSettingsDTO GetUserNarrowSearchSettings(string listName)
        {
            UserNarrowSearchSettingsDTO obj = null;

            using (UserNarrowSearchSettingsDAL settingsDAL = new UserNarrowSearchSettingsDAL())
            {
                obj = settingsDAL.GetUserNarrowSearchSettings(SessionHelper.UserID, listName, SessionHelper.EnterPriceID
                    , SessionHelper.CompanyID,SessionHelper.RoomID);
            }

            return obj;
        }

        //public string GetSavedSearchStr(NarrowSearchSaveListEnum listName)
        //{
        //    var dto = GetUserNarrowSearchSettings(listName.ToString());
        //    string search = "";
        //    string globalSearchVal = "";
        //    bool isAnySearchValue = false;
        //    if (dto!= null && string.IsNullOrWhiteSpace(dto.SettingsJson) == false)
        //    {
        //       string json = dto.SettingsJson;
        //       var searchFieldList = JsonConvert.DeserializeObject<List<NarrowSearchValues>>(json);

        //        // prepare dictionary of search field , value
        //        Dictionary<string, string> SearchFieldValue = new Dictionary<string, string>();

        //        foreach (var searchField in searchFieldList)
        //        {
        //            if(string.IsNullOrWhiteSpace(searchField.field))
        //            {
        //                continue;
        //            }

        //            string val = "";
        //            if (string.IsNullOrWhiteSpace(searchField.val) == false)
        //            {
        //                val = searchField.val;
        //            }
        //            else if (searchField.arrVal != null)
        //            {
        //                val = string.Join(",", searchField.arrVal.ToArray());
        //            }

        //            if (!string.IsNullOrWhiteSpace(val))
        //            {
        //                isAnySearchValue = true;
        //            }

        //            if (SearchFieldValue.ContainsKey(searchField.field) == false)
        //            {
        //                SearchFieldValue.Add(searchField.field, val);
        //            }
        //            else if (searchField.field == "DateCreatedFrom" || searchField.field == "DateUpdatedFrom")
        //            {
        //                string fval = SearchFieldValue[searchField.field];
        //                if (string.IsNullOrWhiteSpace(fval) == false)
        //                {
        //                    fval = fval + "," + val;
        //                    SearchFieldValue[searchField.field] = fval;
        //                }
        //                else
        //                {
        //                    SearchFieldValue[searchField.field] = "";
        //                }
        //            }
        //        }

        //        globalSearchVal = GetSearchFieldValue(SearchFieldValue, "GlobalSearch");

        //        if (!isAnySearchValue && !string.IsNullOrWhiteSpace(globalSearchVal))
        //        {
        //            isAnySearchValue = true;
        //        }

        //        if (!isAnySearchValue)
        //        {
        //            // no search value found
        //            return search;
        //        }

        //        switch (listName)
        //        {
        //            case NarrowSearchSaveListEnum.ItemMaster:
        //                search = GetItemMasterSearchStr(SearchFieldValue, globalSearchVal);
        //                break;
        //            case NarrowSearchSaveListEnum.ItemBinMaster:
        //                search = GetItemBinMasterSearchStr(SearchFieldValue, globalSearchVal);
        //                break;
        //        }

        //    }

        //    return search;
        //}


        //private string GetItemMasterSearchStr(Dictionary<string, string>  SearchFieldValue,string globalSearchVal)
        //{
        //   string search = string.Format(SearchStrings.ItemMaster,
        //                        GetSearchFieldValue(SearchFieldValue, "CreatedBy"), // 0 
        //                        GetSearchFieldValue(SearchFieldValue, "UpdatedBy"), // 1
        //                        GetSearchFieldValue(SearchFieldValue, "DateCreatedFrom"),
        //                        GetSearchFieldValue(SearchFieldValue, "DateUpdatedFrom"),
        //                        GetSearchFieldValue(SearchFieldValue, "UDF1"), // 5
        //                        GetSearchFieldValue(SearchFieldValue, "UDF2"),
        //                        GetSearchFieldValue(SearchFieldValue, "UDF3"),
        //                        GetSearchFieldValue(SearchFieldValue, "UDF4"),
        //                        GetSearchFieldValue(SearchFieldValue, "UDF5"),
        //                        GetSearchFieldValue(SearchFieldValue, "SupplierID"), // 9
        //                        GetSearchFieldValue(SearchFieldValue, "ManufacturerID"),//10
        //                        GetSearchFieldValue(SearchFieldValue, "CategoryID"),//11
        //                        "", // 12
        //                        "",
        //                        "",
        //                        GetSearchFieldValue(SearchFieldValue, "Cost"), // 15
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", // 20
        //                        "",
        //                        GetSearchFieldValue(SearchFieldValue, "ItemType"),
        //                        "",
        //                        "",
        //                        "", // 25
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", // 30
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", //35
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", // 40
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", // 45
        //                        "",
        //                        "",
        //                        GetSearchFieldValue(SearchFieldValue, "AverageCost"), // 48
        //                        GetSearchFieldValue(SearchFieldValue, "Turns"),
        //                        "", // 50
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        GetSearchFieldValue(SearchFieldValue, "ItemLocations"), // 55
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "",  // 60                           
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", // 65
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "",  // 70
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", // 75
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "",  // 80
        //                        "",
        //                        "",
        //                        "",
        //                        "",
        //                        "", // 85
        //                        "",
        //                        GetSearchFieldValue(SearchFieldValue, "InventoryClassification"), // 87
        //                        GetSearchFieldValue(SearchFieldValue, "ItemTrackingType"),
        //                        GetSearchFieldValue(SearchFieldValue, "StockStatus"), // 89
        //                        "",  // 90
        //                        "",
        //                        GetSearchFieldValue(SearchFieldValue, "UDF6"), // 92
        //                        GetSearchFieldValue(SearchFieldValue, "UDF7"),
        //                        GetSearchFieldValue(SearchFieldValue, "UDF8"),
        //                        GetSearchFieldValue(SearchFieldValue, "UDF9"), // 95
        //                        GetSearchFieldValue(SearchFieldValue, "UDF10"),
        //                        GetSearchFieldValue(SearchFieldValue, "IsActive"),
        //                        "",
        //                        "",
        //                        "" // 100
        //                        );

        //    search += globalSearchVal;

        //    return search;
        //}

        //private string GetItemBinMasterSearchStr(Dictionary<string, string> SearchFieldValue, string globalSearchVal)
        //{
        //    string search = string.Format(SearchStrings.ItemBinMaster,
        //                         GetSearchFieldValue(SearchFieldValue, "CreatedBy"), // 0 
        //                         GetSearchFieldValue(SearchFieldValue, "UpdatedBy"), // 1
        //                         GetSearchFieldValue(SearchFieldValue, "DateCreatedFrom"),
        //                         GetSearchFieldValue(SearchFieldValue, "DateUpdatedFrom"),
        //                         GetSearchFieldValue(SearchFieldValue, "UDF1"), // 5
        //                         GetSearchFieldValue(SearchFieldValue, "UDF2"),
        //                         GetSearchFieldValue(SearchFieldValue, "UDF3"),
        //                         GetSearchFieldValue(SearchFieldValue, "UDF4"),
        //                         GetSearchFieldValue(SearchFieldValue, "UDF5"),
        //                         GetSearchFieldValue(SearchFieldValue, "SupplierID"), // 9
        //                         GetSearchFieldValue(SearchFieldValue, "ManufacturerID"),//10
        //                         GetSearchFieldValue(SearchFieldValue, "CategoryID"),//11
        //                         "", // 12
        //                         "",
        //                         "",
        //                         GetSearchFieldValue(SearchFieldValue, "Cost"), // 15
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", // 20
        //                         "",
        //                         GetSearchFieldValue(SearchFieldValue, "ItemType"),
        //                         "",
        //                         "",
        //                         "", // 25
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", // 30
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", //35
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", // 40
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", // 45
        //                         "",
        //                         "",
        //                         GetSearchFieldValue(SearchFieldValue, "AverageCost"), // 48
        //                         GetSearchFieldValue(SearchFieldValue, "Turns"),
        //                         "", // 50
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         GetSearchFieldValue(SearchFieldValue, "ItemLocations"), // 55
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "",  // 60                           
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", // 65
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "",  // 70
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", // 75
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "",  // 80
        //                         "",
        //                         "",
        //                         "",
        //                         "",
        //                         "", // 85
        //                         "",
        //                         GetSearchFieldValue(SearchFieldValue, "InventoryClassification"), // 87
        //                         GetSearchFieldValue(SearchFieldValue, "ItemTrackingType"),
        //                         GetSearchFieldValue(SearchFieldValue, "StockStatus"), // 89
        //                         "",  // 90
        //                         "",
        //                         GetSearchFieldValue(SearchFieldValue, "UDF6"), // 92
        //                         GetSearchFieldValue(SearchFieldValue, "UDF7"),
        //                         GetSearchFieldValue(SearchFieldValue, "UDF8"),
        //                         GetSearchFieldValue(SearchFieldValue, "UDF9"), // 95
        //                         GetSearchFieldValue(SearchFieldValue, "UDF10"),
        //                         GetSearchFieldValue(SearchFieldValue, "IsActive"),
        //                         "",
        //                         "",
        //                         "" // 100
        //                         );

        //    search += globalSearchVal;

        //    return search;
        //}


        //private static string GetSearchFieldValue(Dictionary<string, string> keyValues, string key)
        //{
        //    string value = "";

        //    if(keyValues.ContainsKey(key))
        //    {
        //        value = keyValues[key];
        //    }

        //    return value;           
        //}


        public Tuple<bool, long> SaveUserNarrowSearchSettings(UserNarrowSearchSettingsDTO obj)
        {
            using (UserNarrowSearchSettingsDAL settingsDAL = new UserNarrowSearchSettingsDAL())
            {
                obj.UserID = SessionHelper.UserID;
                obj.EnterpriseID = SessionHelper.EnterPriceID;
                obj.CompanyID = SessionHelper.CompanyID;
                obj.RoomID = SessionHelper.RoomID;
                obj.CreatedBy = SessionHelper.UserID;

                return settingsDAL.SaveUserNarrowSearchSettings(obj);
            }

            
        }


        protected virtual void Dispose(bool disposing)
        { 
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UserNarrowSearchSettingsBAL()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    //public static class SearchStrings
    //{
    //    public static readonly string ItemMaster = @"CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom," +
    //        "UDF1,UDF2,UDF3,UDF4,UDF5,SupplierID,ManufacturerID,CategoryID,SupplierID,OrderStatus," +
    //        "OrderRequiredDate,Cost,spendPerSpendLimit,TotalSpendLimit,TotalSpendRemaining,TotalItemSpendLimit," +
    //        "BinName,SS,ItemType,SupplierName,ReplenishType,RequisitionStatus,OrderNumber,LocationID,WorkOrderID," +
    //        "ToolCategoryID,ToolMaintanenceID,Cost,AssetID,CustomerID,TechnicianID,ToolID,UserType,CountType," +
    //        "CountStatus,UserType,UserRoom,UserRole,UserCompany,RoleRoom,RoleCompany,SchedulerItem,SchedulerType," +
    //        "WorkOrderName,AverageCost,Turns,ActionType,EmailTemplate,Reports,Schedules,NotificationTypes," +
    //        "ItemLocations,EnterpriseName,EnterpriseName,ProjectSpendID,WorkOrderID,RequistionID,Billing,IsEDISent" +
    //        ",AverageUsage,Turns,ItemUDF1,ItemUDF2,ItemUDF3,ItemUDF4,ItemUDF5,RoomStatusValue,ToolCheckOutUDF1," +
    //        "ToolCheckOutUDF2,ToolCheckOutUDF3,ToolCheckOutUDF4,ToolCheckOutUDF5,Maintense,ToolTechnician,OrderNumber," +
    //        "UserEnterprise,MoveType,ModuleType,Items,Supplier,Category,PullPOStatusValue,CompanyStatusValue" +
    //        ",InventoryClassification,ItemTrackingType,StockStatus,SupplierID,SupplierID,UDF6,UDF7,UDF8,UDF9,UDF10," +
    //        "IsActive,RoleEnterprise,ItemNumber,BinNumber,MaintenanceValue,BillingRoomType,InvoiceBranch," +
    //        "RequisitionStatusNS,Consignment,AssetCategoryID,WOType,ReportType,ItemUDF6,ItemUDF7,ItemUDF8," +
    //        "ItemUDF9,ItemUDF10,ToolTechUDF1,ToolTechUDF2,ToolTechUDF3,ToolTechUDF4,ToolTechUDF5,SupplierAccountNumber," +
    //        "[###]{0}@{1}@{2}@{3}@{4}@{5}@{6}@{7}@{8}@{9}@{10}" +
    //        "@{11}@{12}@{13}@{14}@{15}@{16}@{17}@{18}@{19}@{20}" +
    //        "@{21}@{22}@{23}@{24}@{25}@{26}@{27}@{28}@{29}@{30}" +
    //        "@{31}@{32}@{33}@{34}@{35}@{36}@{37}@{38}@{39}@{40}" +
    //        "@{41}@{42}@{43}@{44}@{45}@{46}@{47}@{48}@{49}@{50}" +
    //        "@{51}@{52}@{53}@{54}@{55}@{56}@{57}@{58}@{59}@{60}" +
    //        "@{61}@{62}@{63}@{64}@{65}@{66}@{67}@{68}@{69}@{70}" +
    //        "@{71}@{72}@{73}@{74}@{75}@{76}@{77}@{78}@{79}@{80}" +
    //        "@{81}@{82}@{83}@{84}@{85}@{86}@{87}@{88}@{89}@{90}" +
    //        "@{91}@{92}@{93}@{94}@{95}@{96}@{97}@{98}@{99}@{100}" +
    //        "@@@@@@@@@@@@@@@@@@@" +
    //        "[###]";
    //    public static readonly string ItemBinMaster = ItemMaster;

    //    /// <summary>
    //    /// Is search string has any search value ?
    //    /// </summary>
    //    /// <param name="searchStr"></param>
    //    /// <returns></returns>
    //    public static bool IsEmptySearchString(string searchStr)
    //    {
    //        bool isEmpty = true;
    //        if (string.IsNullOrWhiteSpace(searchStr))
    //        {
    //            return isEmpty;
    //        }
            

    //        string[] split = searchStr.Split( new string[] { "[###]" }, StringSplitOptions.None);
    //        if (split.Length >= 1)
    //        {
    //            if (!string.IsNullOrWhiteSpace(split[2]))
    //            {
    //                isEmpty = false;
    //            }

    //            if (isEmpty)
    //            {
    //                string[] split11 = split[1].Split(new string[] { "@" }, StringSplitOptions.None);
    //                foreach (string s in split11)
    //                {
    //                    if (!string.IsNullOrWhiteSpace(s))
    //                    {
    //                        isEmpty = false;
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        return isEmpty;
    //    }
    //}

}