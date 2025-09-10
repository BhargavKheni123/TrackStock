using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public partial class ToolMasterDAL : eTurnsBaseDAL
    {

        #region [Class Methods]


        public List<ToolMasterDTO> GetPagedToolsNew(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ToolsIDs, string ToolGUIDs, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, string ExcludeToolGuids = "", string Type = "1", string CalledPage = "")
        {
            List<ToolMasterDTO> lstTools = new List<ToolMasterDTO>();
            TotalCount = 0;
            ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
            DataSet dsTools = new DataSet();

            SqlConnection EturnsConnection = new SqlConnection(base.DataBaseConnectionString);
            string ToolLocs = null;
            string ToolCats = null;
            string ToolCost = null;
            string ToolCreaters = null;
            string ToolUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ToolCheckOutUDF1 = null;
            string ToolCheckOutUDF2 = null;
            string ToolCheckOutUDF3 = null;
            string ToolCheckOutUDF4 = null;
            string ToolCheckOutUDF5 = null;
            string ToolMaintence = null;
            string TechnicianList = null;
            string ToolTechUDF1 = null;
            string ToolTechUDF2 = null;
            string ToolTechUDF3 = null;
            string ToolTechUDF4 = null;
            string ToolTechUDF5 = null;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsNew", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type, CalledPage, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ToolCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ToolUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[71]))
                {
                    string[] arrReplenishTypes = FieldsPara[71].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF1 = ToolCheckOutUDF1 + supitem + "','";
                    }
                    ToolCheckOutUDF1 = ToolCheckOutUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF1 = HttpUtility.UrlDecode(ToolCheckOutUDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[72]))
                {
                    string[] arrReplenishTypes = FieldsPara[72].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF2 = ToolCheckOutUDF2 + supitem + "','";
                    }
                    ToolCheckOutUDF2 = ToolCheckOutUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF2 = HttpUtility.UrlDecode(ToolCheckOutUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[73]))
                {
                    string[] arrReplenishTypes = FieldsPara[73].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF3 = ToolCheckOutUDF3 + supitem + "','";
                    }
                    ToolCheckOutUDF3 = ToolCheckOutUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF3 = HttpUtility.UrlDecode(ToolCheckOutUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[74]))
                {
                    string[] arrReplenishTypes = FieldsPara[74].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF4 = ToolCheckOutUDF4 + supitem + "','";
                    }
                    ToolCheckOutUDF4 = ToolCheckOutUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF4 = HttpUtility.UrlDecode(ToolCheckOutUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[75]))
                {
                    string[] arrReplenishTypes = FieldsPara[75].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF5 = ToolCheckOutUDF5 + supitem + "','";
                    }
                    ToolCheckOutUDF5 = ToolCheckOutUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF5 = HttpUtility.UrlDecode(ToolCheckOutUDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[27]))
                {
                    ToolLocs = FieldsPara[27].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[29]))
                {
                    ToolCats = FieldsPara[29].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    ToolCost = FieldsPara[31].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[101]))
                {
                    string[] arrReplenishTypes = FieldsPara[101].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolMaintence = ToolMaintence + supitem + ",";
                    }
                    ToolMaintence = ToolMaintence.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[77]))
                {
                    string[] arrReplenishTypes = FieldsPara[77].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechnicianList = TechnicianList + supitem + "','";
                    }
                    TechnicianList = TechnicianList.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[114]))
                {
                    string[] arrReplenishTypes = FieldsPara[114].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF1 = ToolTechUDF1 + supitem + "','";
                    }
                    ToolTechUDF1 = ToolTechUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF1 = HttpUtility.UrlDecode(ToolTechUDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[115]))
                {
                    string[] arrReplenishTypes = FieldsPara[115].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF2 = ToolTechUDF2 + supitem + "','";
                    }
                    ToolTechUDF2 = ToolTechUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF2 = HttpUtility.UrlDecode(ToolTechUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[116]))
                {
                    string[] arrReplenishTypes = FieldsPara[116].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF3 = ToolTechUDF3 + supitem + "','";
                    }
                    ToolTechUDF3 = ToolTechUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF3 = HttpUtility.UrlDecode(ToolTechUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[117]))
                {
                    string[] arrReplenishTypes = FieldsPara[117].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF4 = ToolTechUDF4 + supitem + "','";
                    }
                    ToolTechUDF4 = ToolTechUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF4 = HttpUtility.UrlDecode(ToolTechUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[118]))
                {
                    string[] arrReplenishTypes = FieldsPara[118].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF5 = ToolTechUDF5 + supitem + "','";
                    }
                    ToolTechUDF5 = ToolTechUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF5 = HttpUtility.UrlDecode(ToolTechUDF5);
                }


                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsNew", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type, CalledPage, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            else
            {
                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsNew", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type, CalledPage, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            if (dsTools != null && dsTools.Tables.Count > 0)
            {
                DataTable dtTools = dsTools.Tables[0];
                if (dtTools.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtTools.Rows[0]["TotalRecords"]);
                    lstTools = dtTools.AsEnumerable().Select(row => new ToolMasterDTO()
                    {
                        AddedFrom = row.Field<string>("AddedFrom"),
                        CheckedOutMQTY = row.Field<double?>("CheckedOutMQTY"),
                        CheckedOutQTY = row.Field<double?>("CheckedOutQTY"),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Cost = row.Field<double?>("Cost"),
                        Created = row.Field<DateTime>("Created"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        Description = row.Field<string>("Description"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        GUID = row.Field<Guid>("GUID"),
                        ID = row.Field<long>("ID"),
                        IsArchived = row.Field<bool?>("IsArchived"),
                        IsAutoMaintain = row.Field<bool>("IsAutoMaintain"),
                        IsCheckedOut = row.Field<bool?>("IscheckedOut"),
                        IsDeleted = row.Field<bool?>("IsDeleted"),
                        IsGroupOfItems = row.Field<int?>("IsGroupOfItems"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        LocationID = row.Field<long?>("LocationID"),
                        MaintenanceType = row.Field<int>("MaintenanceType"),
                        NoOfPastMntsToConsider = row.Field<int?>("NoOfPastMntsToConsider"),
                        MaintenanceDueNoticeDays = row.Field<int?>("MaintenanceDueNoticeDays"),
                        Quantity = row.Field<double>("Quantity"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        Room = row.Field<long?>("Room"),
                        Serial = row.Field<string>("Serial"),
                        ToolCategoryID = row.Field<long?>("ToolCategoryID"),
                        ToolName = row.Field<string>("ToolName"),
                        ToolUDF1 = row.Field<string>("UDF1"),
                        ToolUDF2 = row.Field<string>("UDF2"),
                        ToolUDF3 = row.Field<string>("UDF3"),
                        ToolUDF4 = row.Field<string>("UDF4"),
                        ToolUDF5 = row.Field<string>("UDF5"),
                        Updated = row.Field<DateTime?>("Updated"),
                        Location = row.Field<string>("Location"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        LocationQty = row.Field<double>("LocationQty"),
                        ToolCategory = row.Field<string>("ToolCategory"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        Technician = row.Field<string>("Technician"),
                        ImageType = row.Field<string>("ImageType"),
                        CheckedOutQTYTotal = row.Field<int?>("CheckedOutQTYTotal"),
                        ImagePath = row.Field<string>("ImageType") == "ExternalImage" ? string.Empty : row.Field<string>("ImagePath"),
                        ToolImageExternalURL = row.Field<string>("ImageType") == "ExternalImage" ? row.Field<string>("ToolImageExternalURL") : string.Empty,
                        Type = row.Field<Int64?>("Type"),
                        IsBuildBreak = row.Field<bool>("IsBuildBreak"),
                        AvailableToolQty = row.Field<double?>("AvailableToolQty"),
                        ToolTypeTracking = row.Field<string>("ToolTypeTracking"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        DefaultLocation = row.Field<Int64?>("DefaultLocation"),
                        IsSerialAvailable = row.Field<string>("IsSerialAvailable"),
                        IsCheckOutSerialAvailable = row.Field<string>("IsCheckOutSerialAvailable"),
                        IsBeforeCheckOutAndCheckIn = row.Field<bool>("IsBeforeCheckOutAndCheckIn"),
                    }).ToList();

                }
            }
            return lstTools;
        }


        public List<ToolMasterDTO> GetPagedToolHistoryNew(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ToolsIDs, string ToolGUIDs, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, string ExcludeToolGuids = "")
        {
            List<ToolMasterDTO> lstTools = new List<ToolMasterDTO>();
            TotalCount = 0;
            ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
            DataSet dsTools = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                Connectionstring = base.DataBaseConnectionString;
            }
            if (Connectionstring == "")
            {
                return lstTools;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ToolLocs = null;
            string ToolCats = null;
            string ToolCost = null;
            string ToolCreaters = null;
            string ToolUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ToolCheckOutUDF1 = null;
            string ToolCheckOutUDF2 = null;
            string ToolCheckOutUDF3 = null;
            string ToolCheckOutUDF4 = null;
            string ToolCheckOutUDF5 = null;
            string ToolTechUDF1 = null;
            string ToolTechUDF2 = null;
            string ToolTechUDF3 = null;
            string ToolTechUDF4 = null;
            string ToolTechUDF5 = null;
            string ToolMaintence = null;
            string TechnicianList = null;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsNew_History", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ToolCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ToolUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string[] arrReplenishTypes = FieldsPara[9].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF1 = ToolCheckOutUDF1 + supitem + "','";
                    }
                    ToolCheckOutUDF1 = ToolCheckOutUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF1 = HttpUtility.UrlDecode(ToolCheckOutUDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string[] arrReplenishTypes = FieldsPara[10].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF2 = ToolCheckOutUDF2 + supitem + "','";
                    }
                    ToolCheckOutUDF2 = ToolCheckOutUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF2 = HttpUtility.UrlDecode(ToolCheckOutUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    string[] arrReplenishTypes = FieldsPara[11].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF3 = ToolCheckOutUDF3 + supitem + "','";
                    }
                    ToolCheckOutUDF3 = ToolCheckOutUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF3 = HttpUtility.UrlDecode(ToolCheckOutUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[12]))
                {
                    string[] arrReplenishTypes = FieldsPara[12].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF4 = ToolCheckOutUDF4 + supitem + "','";
                    }
                    ToolCheckOutUDF4 = ToolCheckOutUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF4 = HttpUtility.UrlDecode(ToolCheckOutUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[13]))
                {
                    string[] arrReplenishTypes = FieldsPara[13].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolCheckOutUDF5 = ToolCheckOutUDF5 + supitem + "','";
                    }
                    ToolCheckOutUDF5 = ToolCheckOutUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolCheckOutUDF5 = HttpUtility.UrlDecode(ToolCheckOutUDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[17]))
                {
                    ToolLocs = FieldsPara[17].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[14]))
                {
                    ToolCats = FieldsPara[14].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    ToolCost = FieldsPara[15].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[16]))
                {
                    string[] arrReplenishTypes = FieldsPara[16].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolMaintence = ToolMaintence + supitem + ",";
                    }
                    ToolMaintence = ToolMaintence.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[18]))
                {
                    string[] arrReplenishTypes = FieldsPara[18].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        TechnicianList = TechnicianList + supitem + "','";
                    }
                    TechnicianList = TechnicianList.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[19]))
                {
                    string[] arrReplenishTypes = FieldsPara[19].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF1 = ToolTechUDF1 + supitem + "','";
                    }
                    ToolTechUDF1 = ToolTechUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF1 = HttpUtility.UrlDecode(ToolTechUDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[20]))
                {
                    string[] arrReplenishTypes = FieldsPara[20].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF2 = ToolTechUDF2 + supitem + "','";
                    }
                    ToolTechUDF2 = ToolTechUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF2 = HttpUtility.UrlDecode(ToolTechUDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    string[] arrReplenishTypes = FieldsPara[21].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF3 = ToolTechUDF3 + supitem + "','";
                    }
                    ToolTechUDF3 = ToolTechUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF3 = HttpUtility.UrlDecode(ToolTechUDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    string[] arrReplenishTypes = FieldsPara[22].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF4 = ToolTechUDF4 + supitem + "','";
                    }
                    ToolTechUDF4 = ToolTechUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF4 = HttpUtility.UrlDecode(ToolTechUDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    string[] arrReplenishTypes = FieldsPara[23].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ToolTechUDF5 = ToolTechUDF5 + supitem + "','";
                    }
                    ToolTechUDF5 = ToolTechUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    ToolTechUDF5 = HttpUtility.UrlDecode(ToolTechUDF5);
                }


                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsNew_History", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            else
            {
                dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedToolsNew_History", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, ToolTechUDF1, ToolTechUDF2, ToolTechUDF3, ToolTechUDF4, ToolTechUDF5);
            }
            if (dsTools != null && dsTools.Tables.Count > 0)
            {
                DataTable dtTools = dsTools.Tables[0];
                if (dtTools.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtTools.Rows[0]["TotalRecords"]);
                    lstTools = dtTools.AsEnumerable().Select(row => new ToolMasterDTO()
                    {
                        AddedFrom = row.Field<string>("AddedFrom"),
                        CheckedOutMQTY = row.Field<double?>("CheckedOutMQTY"),
                        CheckedOutQTY = row.Field<double?>("CheckedOutQTY"),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Cost = row.Field<double?>("Cost"),
                        Created = row.Field<DateTime>("Created"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        Description = row.Field<string>("Description"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        GUID = row.Field<Guid>("GUID"),
                        ID = row.Field<long>("ID"),
                        IsArchived = row.Field<bool?>("IsArchived"),
                        IsAutoMaintain = row.Field<bool>("IsAutoMaintain"),
                        IsCheckedOut = row.Field<bool?>("IscheckedOut"),
                        IsDeleted = row.Field<bool?>("IsDeleted"),
                        IsGroupOfItems = row.Field<int?>("IsGroupOfItems"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        LocationID = row.Field<long?>("LocationID"),
                        MaintenanceType = row.Field<int>("MaintenanceType"),
                        NoOfPastMntsToConsider = row.Field<int?>("NoOfPastMntsToConsider"),
                        MaintenanceDueNoticeDays = row.Field<int?>("MaintenanceDueNoticeDays"),
                        Quantity = row.Field<double>("Quantity"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        Room = row.Field<long?>("Room"),
                        Serial = row.Field<string>("Serial"),
                        ToolCategoryID = row.Field<long?>("ToolCategoryID"),
                        ToolName = row.Field<string>("ToolName"),
                        ToolUDF1 = row.Field<string>("UDF1"),
                        ToolUDF2 = row.Field<string>("UDF2"),
                        ToolUDF3 = row.Field<string>("UDF3"),
                        ToolUDF4 = row.Field<string>("UDF4"),
                        ToolUDF5 = row.Field<string>("UDF5"),
                        Updated = row.Field<DateTime?>("Updated"),
                        Location = row.Field<string>("Location"),
                        ToolCategory = row.Field<string>("ToolCategory"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        Technician = row.Field<string>("Technician"),
                        ImageType = row.Field<string>("ImageType"),
                        IsBeforeCheckOutAndCheckIn = row.Field<bool>("IsBeforeCheckOutAndCheckIn"),
                        CheckedOutQTYTotal = row.Field<int?>("CheckedOutQTYTotal"),
                        ImagePath = row.Field<string>("ImageType") == "ExternalImage" ? string.Empty : row.Field<string>("ImagePath"),
                        ToolImageExternalURL = row.Field<string>("ImageType") == "ExternalImage" ? row.Field<string>("ToolImageExternalURL") : string.Empty,
                        Type = row.Field<Int64?>("Type"),
                        IsBuildBreak = row.Field<bool>("IsBuildBreak"),
                        IsAtleaseOneCheckOutCompleted = row.Field<bool>("IsAtleaseOneCheckOutCompleted"),
                        AvailableToolQty = row.Field<double?>("AvailableToolQty"),
                        ToolTypeTracking = row.Field<string>("ToolTypeTracking"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                    }).ToList();

                }
            }
            return lstTools;
        }


        public string ToolNameDuplicateCheck(string ToolName, long ToolID, long RoomID, long CompanyID)
        {
            string ReturnResult = "ok";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@ToolName", ToolName), new SqlParameter("@ToolID", ToolID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                string qry = context.Database.SqlQuery<string>("exec [GETToolDuplicate] @ToolName,@ToolID,@RoomID,@CompanyID", params1).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(qry))
                {
                    ReturnResult = "duplicate";
                }
            }
            return ReturnResult;

        }

        public ToolMasterDTO GetHistoryRecordNew(Int64 HistoryID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID) };
                return context.Database.SqlQuery<ToolMasterDTO>("exec [GetToolHisToryRecordByIDNew] @HistoryID", params1).FirstOrDefault();
            }
        }

        public string ToolNameSerialDuplicateCheck(string ToolNameSerial, long ToolID, long RoomID, long CompanyID)
        {
            string ReturnResult = "ok";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@ToolNameSerial", ToolNameSerial), new SqlParameter("@ToolID", ToolID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                string qry = context.Database.SqlQuery<string>("exec [GETToolSerailDuplicate] @ToolNameSerial,@ToolID,@RoomID,@CompanyID", params1).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(qry))
                {
                    ReturnResult = "duplicate";
                }
            }
            return ReturnResult;

        }

        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecordsForHistoryNew(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolBinNarrowSearch_HistoryNew] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived", params1).ToList();
            }
        }
        public double? GetToolPriceByRoomModuleSettings(long CompanyId, long RoomId, long ModuleId, Guid ToolGuid, bool ConsiderCostUOM)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //----------------------------------------------------------------------------------------
                //
                int? PriseSelectionOption = 0;
                PriseSelectionOption = (from RMS in context.RoomModuleSettings
                                        join M in context.ModuleMasters on RMS.ModuleId equals M.ID
                                        where RMS.CompanyId == CompanyId
                                               && RMS.RoomId == RoomId
                                               && RMS.ModuleId == ModuleId
                                        select RMS.PriseSelectionOption).FirstOrDefault();

                if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
                    PriseSelectionOption = 1;


                var Result = (from IM in context.ToolMasters

                              where IM.GUID == ToolGuid
                                    && IM.CompanyID == CompanyId
                                    && IM.Room == RoomId
                                    && (IM.IsDeleted == null || IM.IsDeleted == false)
                                    && (IM.IsArchived == null || IM.IsArchived == false)
                              select new
                              {
                                  Cost = (IM.Cost == null ? 0 : IM.Cost),
                              }).FirstOrDefault();


                return Result.Cost;

            }
        }

        #endregion
    }
}
