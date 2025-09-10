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
    public partial class WorkOrderDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public WorkOrderDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        
        #endregion

        #region [Class Methods]
        public Guid Insert(WorkOrderDTO objDTO)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = new WorkOrder();
                obj.ID = 0;
                obj.WOName = objDTO.WOName;
                obj.TechnicianID = objDTO.TechnicianID;
                obj.Technician = objDTO.Technician;
                obj.CustomerID = objDTO.CustomerID;
                obj.Customer = objDTO.Customer;
                obj.AssetGUID = objDTO.AssetGUID;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.Odometer_OperationHours = objDTO.Odometer_OperationHours.GetValueOrDefault(0);
                obj.UsedItems = objDTO.UsedItems.GetValueOrDefault(0);
                obj.UsedItemsCost = objDTO.UsedItemsCost.GetValueOrDefault(0);
                obj.UsedItemsSellPrice = objDTO.UsedItemsSellPrice;
                obj.WOStatus = objDTO.WOStatus;
                obj.WOType = objDTO.WOType;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                if (objDTO.GUID == Guid.Empty)
                {
                    obj.GUID = Guid.NewGuid();
                }
                else
                {
                    obj.GUID = objDTO.GUID;
                }
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Description = objDTO.Description;
                obj.IsSignatureCapture = objDTO.IsSignatureCapture;
                obj.IsSignatureRequired = objDTO.IsSignatureRequired;
                obj.SignatureName = objDTO.SignatureName;
                obj.CustomerGUID = objDTO.CustomerGUID;
                obj.ReleaseNumber = objDTO.ReleaseNumber;

                if (string.IsNullOrWhiteSpace(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Work Order";

                obj.WhatWhereAction = objDTO.WhatWhereAction;

                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.AddedFrom = objDTO.AddedFrom;
                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom;
                obj.SupplierId = objDTO.SupplierId;
                obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
                obj.UsedItems = objDTO.UsedItems.GetValueOrDefault(0);
                obj.UsedItemsCost = objDTO.UsedItemsCost.GetValueOrDefault(0);
                obj.UsedItemsSellPrice = objDTO.UsedItemsSellPrice;
                obj.CreatedFrom = (objDTO.CreatedFrom.GetValueOrDefault(0) > 0) ? objDTO.CreatedFrom : (int)WorkOrderCreatedFrom.Web;
                context.WorkOrders.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                if (objDTO.ID > 0)
                {
                    new AutoSequenceDAL(base.DataBaseName).UpdateNextWorkOrderNumber(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.SupplierId.GetValueOrDefault(0), objDTO.WOName);
                }
                return obj.GUID;
            }
        }

        public Guid InsertFromPullHistoryImport(WorkOrderDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = new WorkOrder();
                obj.ID = 0;
                obj.WOName = objDTO.WOName;
                obj.TechnicianID = objDTO.TechnicianID;
                obj.Technician = objDTO.Technician;
                obj.CustomerID = objDTO.CustomerID;
                obj.Customer = objDTO.Customer;
                obj.AssetGUID = objDTO.AssetGUID;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.Odometer_OperationHours = objDTO.Odometer_OperationHours.GetValueOrDefault(0);
                obj.UsedItems = objDTO.UsedItems.GetValueOrDefault(0);
                obj.UsedItemsCost = objDTO.UsedItemsCost.GetValueOrDefault(0);
                obj.UsedItemsSellPrice = objDTO.UsedItemsSellPrice;
                obj.WOStatus = objDTO.WOStatus;
                obj.WOType = objDTO.WOType;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                if (objDTO.GUID == Guid.Empty)
                {
                    obj.GUID = Guid.NewGuid();
                }
                else
                {
                    obj.GUID = objDTO.GUID;
                }
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Description = objDTO.Description;
                obj.IsSignatureCapture = objDTO.IsSignatureCapture;
                obj.IsSignatureRequired = objDTO.IsSignatureRequired;
                obj.SignatureName = objDTO.SignatureName;
                obj.CustomerGUID = objDTO.CustomerGUID;
                obj.ReleaseNumber = objDTO.ReleaseNumber;

                if (string.IsNullOrWhiteSpace(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Work Order";

                obj.WhatWhereAction = objDTO.WhatWhereAction;
                obj.ReceivedOn = objDTO.ReceivedOn = objDTO.Created ?? DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = objDTO.Created ?? DateTimeUtility.DateTimeNow;
                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.AddedFrom = objDTO.AddedFrom;
                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom;
                obj.SupplierId = objDTO.SupplierId;
                obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;
                obj.UsedItems = objDTO.UsedItems.GetValueOrDefault(0);
                obj.UsedItemsCost = objDTO.UsedItemsCost.GetValueOrDefault(0);
                obj.UsedItemsSellPrice = objDTO.UsedItemsSellPrice;
                obj.CreatedFrom = (objDTO.CreatedFrom.GetValueOrDefault(0) > 0) ? objDTO.CreatedFrom : (int)WorkOrderCreatedFrom.Web;
                context.WorkOrders.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                if (objDTO.ID > 0)
                {
                    new AutoSequenceDAL(base.DataBaseName).UpdateNextWorkOrderNumber(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.SupplierId.GetValueOrDefault(0), objDTO.WOName);
                }
                return obj.GUID;
            }
        }

        public bool Edit(WorkOrderDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = context.WorkOrders.FirstOrDefault(t => t.ID == objDTO.ID);
                if (obj != null)
                {
                    obj.WOName = objDTO.WOName;
                    obj.TechnicianID = objDTO.TechnicianID;
                    obj.Technician = objDTO.Technician;
                    obj.CustomerID = objDTO.CustomerID;
                    obj.Customer = objDTO.Customer;
                    obj.AssetGUID = objDTO.AssetGUID;
                    obj.Odometer_OperationHours = objDTO.Odometer_OperationHours.GetValueOrDefault(0);
                    obj.UsedItems = objDTO.UsedItems.GetValueOrDefault(0);
                    obj.UsedItemsCost = objDTO.UsedItemsCost.GetValueOrDefault(0);
                    obj.UsedItemsSellPrice = objDTO.UsedItemsSellPrice;
                    obj.WOStatus = objDTO.WOStatus;
                    obj.WOType = objDTO.WOType;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.Description = objDTO.Description;
                    obj.IsSignatureCapture = objDTO.IsSignatureCapture;
                    obj.IsSignatureRequired = objDTO.IsSignatureRequired;
                    obj.SignatureName = objDTO.SignatureName;
                    if (string.IsNullOrWhiteSpace(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Work Order";
                    objDTO.WhatWhereAction = objDTO.WhatWhereAction;
                    obj.CustomerGUID = objDTO.CustomerGUID;
                    obj.WhatWhereAction = objDTO.WhatWhereAction;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.ReleaseNumber = objDTO.ReleaseNumber;
                    obj.SupplierId = objDTO.SupplierId;
                    obj.SupplierAccountGuid = objDTO.SupplierAccountGuid;

                    if (objDTO.ToolGUID != null)
                    {
                        obj.ToolGUID = objDTO.ToolGUID;
                    }

                    context.SaveChanges();
                    objDTO.ID = obj.ID;
                }
                return true;
            }
        }

        public List<WorkOrderDTO> GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool UserConsignmentAllowed, string WOTypes, DataTable WOStatus, string RoomDateFormat,TimeZoneInfo CurrentTimeZone)
        {
            List<WorkOrderDTO> lstWorkOrders = new List<WorkOrderDTO>();
            TotalCount = 0;
            WorkOrderDTO objCartItemDTO = new WorkOrderDTO();
            DataSet dsWorkOrders = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstWorkOrders;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string Teches = null;
            string Custs = null;
            string Assets = null;
            string Tools = null;
            string WOCreaters = null;
            string WOUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string WoSupplier = null;
            string woStatusNS = null;
            string spName = IsArchived ? "GetPagedWorkOrders_Archive" : "GetPagedWorkOrders";

            if (string.IsNullOrEmpty(SearchTerm))
            {
                dsWorkOrders = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, Teches, Custs, Assets, Tools, WOCreaters, WOUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, WOTypes, WOStatus, WoSupplier, woStatusNS);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    WOCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    WOUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[33]))
                {                    
                    string[] arrCusts = FieldsPara[33].Split(',');
                    foreach (string custs in arrCusts)
                    {
                        Custs = Custs + HttpUtility.UrlDecode(custs) + "','";
                    }
                    Custs = Custs.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    Custs = HttpUtility.UrlDecode(Custs);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[34]))
                {
                    Teches = Convert.ToString(FieldsPara[34]).TrimEnd();
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    string[] arrReplenishTypes = FieldsPara[32].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        Assets = Assets + supitem + "','";
                    }
                    Assets = Assets.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[35]))
                {
                    string[] arrReplenishTypes = FieldsPara[35].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        Tools = Tools + supitem + "','";
                    }
                    Tools = Tools.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[90]))
                {
                    WoSupplier = Convert.ToString(FieldsPara[90]).TrimEnd();
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[107]))
                {
                    WOTypes = string.Empty;
                    string[] arrWOTypes = FieldsPara[107].Split(',');
                    foreach (string woitem in arrWOTypes)
                    {
                        WOTypes = WOTypes + woitem + "','";
                    }
                    WOTypes = WOTypes.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[127]))
                {
                    woStatusNS = string.Empty;
                    string[] arrWOStatus = FieldsPara[127].Split(',');
                    foreach (string woitem in arrWOStatus)
                    {
                        woStatusNS = woStatusNS + woitem + ",";
                    }
                    woStatusNS = woStatusNS.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }

                dsWorkOrders = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, Teches, Custs, Assets, Tools, WOCreaters, WOUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, WOTypes, WOStatus, WoSupplier, woStatusNS);
            }
            else
            {
                dsWorkOrders = SqlHelper.ExecuteDataset(EturnsConnection, spName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, Teches, Custs, Assets, Tools, WOCreaters, WOUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, WOTypes, WOStatus, WoSupplier, woStatusNS);
            }

            if (dsWorkOrders != null && dsWorkOrders.Tables.Count > 0)
            {
                DataTable dtWorkOrders = dsWorkOrders.Tables[0];
                if (dtWorkOrders.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtWorkOrders.Rows[0]["TotalRecords"]);
                    lstWorkOrders = (from row in dtWorkOrders.AsEnumerable()
                                     select new WorkOrderDTO
                                     {
                                         ID = row.Field<long>("ID"),
                                         GUID = row.Field<Guid>("GUID"),
                                         WOName = row.Field<string>("WOName"),
                                         SubJob = row.Field<string>("SubJob"),
                                         TechnicianID = row.Field<long?>("TechnicianID"),
                                         Technician = row.Field<string>("Technician"),
                                         CustomerID = row.Field<long?>("CustomerID"),
                                         Customer = row.Field<string>("CustomerName"),
                                         AssetGUID = row.Field<Guid?>("AssetGUID"),
                                         AssetName = row.Field<string>("AssetName"),
                                         ToolGUID = row.Field<Guid?>("ToolGUID"),
                                         ToolName = row.Field<string>("ToolName"),
                                         Odometer_OperationHours = row.Field<double?>("Odometer_OperationHours"),
                                         UsedItems = row.Field<int?>("UsedItems"),
                                         UsedItemsCost = row.Field<double?>("UsedItemsCost"),
                                         //JobTypeID = row.Field<long?>("JobTypeID"),
                                         GXPRConsigmentJobID = row.Field<long?>("GXPRConsigmentJobID"),
                                         WOType = row.Field<string>("WOType"),
                                         WOStatus = row.Field<string>("StatusValue"),
                                         Created = row.Field<DateTime?>("Created"),
                                         Updated = row.Field<DateTime?>("Updated"),
                                         CreatedBy = row.Field<long?>("CreatedBy"),
                                         LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                                         Room = row.Field<long?>("Room"),
                                         RoomName = row.Field<string>("RoomName"),
                                         IsDeleted = row.Field<bool?>("IsDeleted"),
                                         IsArchived = row.Field<bool?>("IsArchived"),
                                         CompanyID = row.Field<long?>("CompanyID"),
                                         UDF1 = row.Field<string>("UDF1"),
                                         UDF2 = row.Field<string>("UDF2"),
                                         UDF3 = row.Field<string>("UDF3"),
                                         UDF4 = row.Field<string>("UDF4"),
                                         UDF5 = row.Field<string>("UDF5"),
                                         CreatedByName = row.Field<string>("CreatedByName"),
                                         UpdatedByName = row.Field<string>("UpdatedByName"),
                                         Description = row.Field<string>("Description"),
                                         RequisitionNumber = row.Field<string>("RequisitionNumber"),
                                         IsSignatureCapture = row.Field<bool>("IsSignatureCapture"),
                                         IsSignatureRequired = row.Field<bool>("IsSignatureRequired"),
                                         SignatureName = row.Field<string>("SignatureName"),
                                         CustomerGUID = row.Field<Guid?>("CustomerGUID"),
                                         ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                                         ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                                         AddedFrom = row.Field<string>("AddedFrom"),
                                         EditedFrom = row.Field<string>("EditedFrom"),
                                         ReleaseNumber = row.Field<string>("ReleaseNumber"),
                                         SupplierName = row.Field<string>("SupplierName"),
                                         SupplierAccountNumberName = row.Field<string>("SupplierAccountNumberName"),
                                         CreatedFrom = row.Field<int?>("CreatedFrom"),
                                         UsedItemsSellPrice = row.Field<double>("UsedItemsSellPrice"),
                                         PriseSelectionOption = row.Field<int>("PriseSelectionOption"),
                                         ProjectSpendName = row.Field<string>("ProjectSpendName")
                                     }).ToList();
                }
            }
            return lstWorkOrders;
        }

        public List<WorkOrderDTO> GetAllRecordsOnlyImages()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                    return context.Database.SqlQuery<WorkOrderDTO>("exec [GetAllWorkordersHavingImages]").ToList();
            }
        }
        
        public List<WorkOrderImageDetail> GetWorkorderImagesByWOGuidPlain(Guid WorkOrderGuid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WorkorderGuid", WorkOrderGuid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderImageDetail>("exec [GetWorkorderImagesByWOGuidPlain] @WorkorderGuid", params1).ToList();
            }
        }
        
        public WorkOrderDTO GetWorkOrderByGUIDPlain(Guid WOGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderByGUIDPlain] @WOGUID", params1).FirstOrDefault();
            }
        }
        public WorkOrderDTO GetWorkOrderByIDPlain(long WOID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOID", WOID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderByIDPlain] @WOID", params1).FirstOrDefault();
            }
        }
        
        public WorkOrderDTO GetLastWorkOrderByRoomPlain(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetLastWorkOrderByRoomPlain] @RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<NarrowSearchDTO> GetWorkOrderListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, string NarrowSearchKey)
        {
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@IsArchived", IsArchived),
                                               new SqlParameter("@IsDeleted", IsDeleted),
                                               new SqlParameter("@SupplierIds", strSupplierIds),
                                               new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetWorkOrderListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@SupplierIds,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<WorkOrderDTO> GetWorkOrdersByNamePlain(string WoName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WoName", WoName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrdersByNamePlain] @WoName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        
        public WorkOrderDTO GetWorkOrdersByNamePlainSingle(string WoName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WoName", WoName ?? (object)DBNull.Value), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID) 
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var workorder =  context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderByNamePlain] @WoName,@RoomID,@CompanyID", params1).FirstOrDefault();
                
                if (workorder != null && workorder.ID > 0)
                {
                    workorder.Odometer_OperationHours = workorder.Odometer_OperationHours ?? 0;
                    workorder.UsedItems = workorder.UsedItems ?? 0;
                    workorder.UsedItemsCost = workorder.UsedItemsCost ?? 0;
                }
                return workorder;
            }
        }
        public WorkOrderDTO GetWorkOrdersByNamePlainByStatus(string WoName, long RoomID, long CompanyID, string WOStatus)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WoName", WoName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@WOStatus", WOStatus ?? (object)DBNull.Value)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderByNameAndStatusPlain] @WoName,@RoomID,@CompanyID,@WOStatus", params1).FirstOrDefault();
            }
        }

        public List<WorkOrderDTO> GetWorkOrdersByRoomWOTypeAndStatusPlain(long RoomID,long CompanyId,string  WOType)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyId), 
                                               new SqlParameter("@WOType", WOType) 
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrdersByRoomWOTypeAndStatusPlain] @RoomID,@CompanyID,@WOType", params1).ToList();
            }            
        }
        
        public List<WorkOrderDTO> GetWorkOrdersByIdsJoins(string WOIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOIDs", WOIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrdersByIdsJoins] @WOIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        
        public WorkOrderDTO GetWorkOrdersByGUIDFullJoins(Guid WOGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrdersByGUIDFullJoins] @WOGUID", params1).FirstOrDefault();
            }
        }
        
        public WorkOrderDTO GetWorkOrderHistoryByHistoryIdFull(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", Id) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderHistoryByHistoryIdFull] @ID", params1).FirstOrDefault();
            }
        }
        public WorkOrderDTO GetHistoryRecordForMaintenance(long WorkOrderID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", WorkOrderID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderHistoryByIdFull] @ID", params1).FirstOrDefault();
            }
        }
        
        public string DeleteRecords(string IDs, long UserId, long CompanyID, long RoomID, long SessionUserId, string RecordsDeletedMsg, string RecordsUsedInOtherModuleMsg, long EnterpriseId)
        {
            int ctrdelete = 0;
            int ctrExist = 0;
            string msg = string.Empty;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);

                        if (!objPullDetailsDAL.CheckPullExist(System.Guid.Parse(item)))
                        {
                            var params1 = new SqlParameter[] { new SqlParameter("@Guid", Guid.Parse(item)), 
                                                               new SqlParameter("@UserId", UserId) 
                                                             };
                            context.Database.ExecuteSqlCommand("exec [DeleteWorkorderByGuid] @Guid,@UserId", params1);

                            ctrdelete += 1;
                            PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                            var pullIds = objPullDAL.GetPullIdsByWorkOrderDetailGuid(Guid.Parse(item), RoomID, CompanyID);

                            if (pullIds != null && pullIds.Any())
                            {
                                objPullDAL.DeletePullsByPullIds(string.Join(",", pullIds.Select(i => i.ToString()).ToArray()), UserId, CompanyID, RoomID, SessionUserId,EnterpriseId);
                            }
                        }
                        else
                        {
                            ctrExist += 1;
                        }
                    }
                }

                if (ctrdelete > 0)
                {
                    msg = string.Format(RecordsDeletedMsg, ctrdelete); //
                }

                if (ctrExist > 0)
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = string.Format(RecordsUsedInOtherModuleMsg, ctrExist); 
                    }
                    else
                    {
                        msg = msg + " " + string.Format(RecordsUsedInOtherModuleMsg, ctrExist); //MsgRecordsUsedInOtherModule
                    }
                }

                return msg;
            }
        }

        public List<WorkOrderDTO> GetAllWorkOrderReport(string StartDate, string EndDate, string QuantityType = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@StartDate", StartDate ?? (object)DBNull.Value), 
                                                   new SqlParameter("@EndDate", EndDate ?? (object)DBNull.Value), 
                                                   new SqlParameter("@QuantityType", QuantityType ?? (object)DBNull.Value) 
                                                };
                return context.Database.SqlQuery<WorkOrderDTO>("exec GetWorkOrderListForReport @StartDate,@EndDate,@QuantityType", params1).ToList();
            }
        }
        
        public Guid GetWorkOrderGuid(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var workorder = GetWorkOrderByIDPlain(Id);
                return workorder.GUID;
            }
        }
        
        public List<WorkOrderDTO> WorkOrderExport(long RoomID, long CompanyID, string Ids)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@RoomId",RoomID),
                                                    new SqlParameter("@CompanyId", CompanyID),
                                                    new SqlParameter("@WOGuids", Ids ?? (object)DBNull.Value)
                                                 };

                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkordersByGuIDsForExport] @RoomId,@CompanyId,@WOGuids ", params1).ToList();
            }

        }
        public string GenerateAndGetReleaseNumber(string WorkOrderName, Int64 WorkOrderId, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderName", WorkOrderName ?? string.Empty), new SqlParameter("@WorkOrderId", WorkOrderId), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                string ReleaseNumber = (from u in context.Database.SqlQuery<string>("EXEC csp_GenerateAndGetWorkOrderReleaseNumber @WorkOrderName, @WorkOrderId, @RoomID, @CompanyID", params1)
                                        select u).FirstOrDefault();
                return ReleaseNumber;
            }
        }

        public string GenerateAndGetWorkOrderReleaseNumberByGuid(string WorkOrderName, Guid WorkorderGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { 
                                                    new SqlParameter("@WorkOrderName", WorkOrderName ?? string.Empty),
                                                    new SqlParameter("@WorkOrderGuid", WorkorderGuid), 
                                                    new SqlParameter("@RoomID", RoomID), 
                                                    new SqlParameter("@CompanyID", CompanyID) 
                                                 };
                string ReleaseNumber = (from u in context.Database.SqlQuery<string>("EXEC GenerateAndGetWorkOrderReleaseNumber @WorkOrderName, @WorkOrderGuid, @RoomID, @CompanyID", params1)
                                        select u).FirstOrDefault();
                return ReleaseNumber;
            }
        }
        

        public bool AssignAssetToWorkorder(Guid workorderGuid, Guid assetGUID, long userId, long companyId, long roomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = context.WorkOrders.FirstOrDefault(t => t.GUID == workorderGuid);
                if (obj != null)
                {
                    obj.AssetGUID = assetGUID;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = userId;
                    obj.EditedFrom = "Web";
                    context.SaveChanges();
                }
                return true;
            }
        }
        public bool AssignAssetToWorkorderFromPullHistoryImport(Guid workorderGuid, Guid assetGUID, long userId, long companyId, long roomId,DateTime Updated)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = context.WorkOrders.FirstOrDefault(t => t.GUID == workorderGuid);
                if (obj != null)
                {
                    obj.AssetGUID = assetGUID;
                    obj.Updated = Updated;
                    obj.LastUpdatedBy = userId;
                    obj.EditedFrom = "Web";
                    context.SaveChanges();
                }
                return true;
            }
        }
        public bool UpdateWorkorderItemUsedAmdCost(Guid workorderGuid, int usedItems, double usedItemCost, long userId, long companyId, long roomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = context.WorkOrders.FirstOrDefault(t => t.GUID == workorderGuid && t.Room == roomId && t.CompanyID == companyId);

                if (obj != null)
                {
                    obj.UsedItems = usedItems;
                    obj.UsedItemsCost = usedItemCost;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = userId;
                    obj.EditedFrom = "Web";
                    context.SaveChanges();
                }

                return true;
            }
        }
        public bool UpdateWorkorderItemSellPrice(Guid workorderGuid, double UsedItemsSellPrice, long userId, long companyId, long roomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = context.WorkOrders.FirstOrDefault(t => t.GUID == workorderGuid && t.Room == roomId && t.CompanyID == companyId);

                if (obj != null)
                {
                    obj.UsedItemsSellPrice = UsedItemsSellPrice;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.LastUpdatedBy = userId;
                    obj.EditedFrom = "Web";
                    context.SaveChanges();
                }

                return true;
            }
        }
        public List<WorkOrderDTO> GetWorkOrderMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderMasterChangeLog] @GUID", params1).ToList();
            }
        }

        #endregion

        public void CloseWorkorderByIds(string IDs, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@Ids", IDs)
                                                };

                context.Database.ExecuteSqlCommand("exec [CloseWorkorderByIds] @UserID,@Ids", params1);
            }
        }

        public bool UpdateWOSignatureById(long Id, string FileName, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Id", Id),
                                                    new SqlParameter("@FileName", FileName),
                                                    new SqlParameter("@UserId", UserId)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateWOSignatureById] @Id,@FileName,@UserId", params1);
                return true;
            }
        }

        public bool RemoveWOSignatureById(long Id, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Id", Id),
                                                    new SqlParameter("@UserId", UserId)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [RemoveWOSignatureById] @Id,@UserId", params1);
                return true;
            }
        }

        public string GetWorkOrdersByReportRange(string _range, string _rangeFieldID, string _rangeData, string RoomIDs, string CompanyIDs, bool _isSelectAllRangeData = false)
        {

            if (string.IsNullOrWhiteSpace(_range) || string.IsNullOrWhiteSpace(_rangeFieldID))
                return string.Empty;

            string _dataGuids = string.Empty;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@range",_range)
                                    , new SqlParameter("@rangeData",_rangeData)
                                    , new SqlParameter("@rangeFieldID",_rangeFieldID)
                                    , new SqlParameter("@roomid",RoomIDs)
                                    , new SqlParameter("@companyid",CompanyIDs)
                                    , new SqlParameter("@IsSelectAllRangeData",_isSelectAllRangeData)};

                string qry = "exec [Schl_GetWorkOrderGuidByReportRange] @range, @rangeData,@rangeFieldID, @roomid, @companyid, @IsSelectAllRangeData";
                List<Guid> lstGuids = context.Database.SqlQuery<Guid>(qry, params1).ToList();
                if (lstGuids != null && lstGuids.Any())
                {
                    _dataGuids = string.Join(",", lstGuids.Select(t => t).ToArray());
                }

                return _dataGuids;
            }
        }

        public WorkOrderDTO GetWorkOrderByRequisitionDetailGuidPlain(Guid RequisitionDetailGuid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReqDetailGUID", RequisitionDetailGuid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrderByReqDetailGuidPlain] @ReqDetailGUID", params1).FirstOrDefault();
            }
        }

        public WorkOrderDTO GetWorkOrdersByGUIDFullJoinsArchieve(Guid WOGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderDTO>("exec [GetWorkOrdersByGUIDFullJoins_Archieve] @WOGUID", params1).FirstOrDefault();
            }
        }

    }
}