using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;



namespace eTurns.DAL
{
    public partial class WorkOrderDAL : eTurnsBaseDAL
    {
        public IEnumerable<WorkOrderDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            //IEnumerable<WorkOrderDTO> ObjCache = CacheHelper<IEnumerable<WorkOrderDTO>>.GetCacheItem("Cached_WorkOrder_" + CompanyID.ToString());
            IEnumerable<WorkOrderDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {

                ObjCache = CacheHelper<IEnumerable<WorkOrderDTO>>.GetCacheItem("Cached_WorkOrder_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                    //IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<WorkOrderDTO> obj = (from u in context.ExecuteStoreQuery<WorkOrderDTO>(@"
                    SELECT 
                    A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                    A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                    A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                    A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType, 
                    B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName 
                    ,T.ToolName as Tool,AST.AssetName as Asset, A.Description 
                    ,A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,A.CustomerGUID
                    ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.ReleaseNumber,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                    FROM WorkOrder A 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                    LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID
                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                         select new WorkOrderDTO
                                                         {
                                                             ID = u.ID,
                                                             GUID = u.GUID,
                                                             WOName = u.WOName,
                                                             SubJob = u.SubJob,
                                                             TechnicianID = u.TechnicianID,
                                                             Technician = u.Technician,
                                                             CustomerID = u.CustomerID,
                                                             Customer = u.Customer,
                                                             CustomerGUID = u.CustomerGUID,
                                                             AssetGUID = u.AssetGUID,
                                                             AssetName = u.AssetName,
                                                             ToolGUID = u.ToolGUID,
                                                             ToolName = u.ToolName,
                                                             Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                                                             UsedItems = u.UsedItems.GetValueOrDefault(0),
                                                             UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                                                             JobTypeID = u.JobTypeID,
                                                             GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                                                             WOType = u.WOType == null ? "" : u.WOType,
                                                             WOStatus = u.WOStatus,
                                                             Created = u.Created,
                                                             Updated = u.Updated,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             Room = u.Room,
                                                             IsDeleted = u.IsDeleted,
                                                             IsArchived = u.IsArchived,
                                                             CompanyID = u.CompanyID,
                                                             UDF1 = u.UDF1,
                                                             UDF2 = u.UDF2,
                                                             UDF3 = u.UDF3,
                                                             UDF4 = u.UDF4,
                                                             UDF5 = u.UDF5,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             Description = u.Description,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Work Orders"),
                                                             IsSignatureCapture = u.IsSignatureCapture,
                                                             IsSignatureRequired = u.IsSignatureRequired,
                                                             SignatureName = u.SignatureName,
                                                             ReleaseNumber = u.ReleaseNumber,
                                                             SupplierId = u.SupplierId,
                                                             SupplierAccountGuid = u.SupplierAccountGuid,
                                                             CreatedFrom = u.CreatedFrom,
                                                             UsedItemsSellPrice = u.UsedItemsSellPrice
                                                         }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<WorkOrderDTO>>.AddCacheItem("Cached_WorkOrder_" + CompanyID.ToString(), obj);
                    }
                }
            }
            else
            {
                //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                //IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<WorkOrderDTO>(
                                @" SELECT 
                                A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                                A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                                A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                                A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType, 
                                B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName 
                                ,T.ToolName as Tool,AST.AssetName as Asset, A.Description
                                ,A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,A.CustomerGUID
                                ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                                FROM WorkOrder A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                                LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID
                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new WorkOrderDTO
                                {
                                    ID = u.ID,
                                    GUID = u.GUID,
                                    WOName = u.WOName,
                                    SubJob = u.SubJob,
                                    TechnicianID = u.TechnicianID,
                                    Technician = u.Technician,
                                    CustomerID = u.CustomerID,
                                    CustomerGUID = u.CustomerGUID,
                                    Customer = u.Customer,
                                    AssetGUID = u.AssetGUID,
                                    AssetName = u.AssetName,
                                    ToolGUID = u.ToolGUID,
                                    ToolName = u.ToolName,
                                    Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                                    UsedItems = u.UsedItems.GetValueOrDefault(0),
                                    UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                                    JobTypeID = u.JobTypeID,
                                    GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                                    WOType = u.WOType,
                                    WOStatus = u.WOStatus,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    Description = u.Description,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Work Orders"),
                                    IsSignatureCapture = u.IsSignatureCapture,
                                    IsSignatureRequired = u.IsSignatureRequired,
                                    SignatureName = u.SignatureName,
                                    SupplierId = u.SupplierId,
                                    SupplierAccountGuid = u.SupplierAccountGuid,
                                    CreatedFrom = u.CreatedFrom,
                                    UsedItemsSellPrice = u.UsedItemsSellPrice
                                }).AsParallel().ToList();
                }
            }
            string[] WoTypes = new string[] { "workorder", "reqn" };
            return ObjCache.Where(t => t.Room == RoomID).Where(t => WoTypes.Contains(t.WOType.ToLower()));
        }
        public IEnumerable<WorkOrderDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string Status)
        {
            //Get Cached-Media
            //IEnumerable<WorkOrderDTO> ObjCache = CacheHelper<IEnumerable<WorkOrderDTO>>.GetCacheItem("Cached_WorkOrder_" + CompanyID.ToString());
            IEnumerable<WorkOrderDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {

                ObjCache = CacheHelper<IEnumerable<WorkOrderDTO>>.GetCacheItem("Cached_WorkOrder_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                    //IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<WorkOrderDTO> obj = (from u in context.ExecuteStoreQuery<WorkOrderDTO>(@"
                    SELECT 
                    A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                    A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                    A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                    A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType, 
                    B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName 
                    ,T.ToolName as Tool,AST.AssetName as Asset, A.Description
                    ,A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,A.CustomerGUID
                    ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.ReleaseNumber,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                    FROM WorkOrder A 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                    LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID
                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                         select new WorkOrderDTO
                                                         {
                                                             ID = u.ID,
                                                             GUID = u.GUID,
                                                             WOName = u.WOName,
                                                             SubJob = u.SubJob,
                                                             TechnicianID = u.TechnicianID,
                                                             Technician = u.Technician,
                                                             CustomerID = u.CustomerID,
                                                             Customer = u.Customer,
                                                             CustomerGUID = u.CustomerGUID,
                                                             AssetGUID = u.AssetGUID,
                                                             AssetName = u.AssetName,
                                                             ToolGUID = u.ToolGUID,
                                                             ToolName = u.ToolName,
                                                             Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                                                             UsedItems = u.UsedItems.GetValueOrDefault(0),
                                                             UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                                                             JobTypeID = u.JobTypeID,
                                                             GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                                                             WOType = u.WOType == null ? "" : u.WOType,
                                                             WOStatus = u.WOStatus,
                                                             Created = u.Created,
                                                             Updated = u.Updated,
                                                             CreatedBy = u.CreatedBy,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             Room = u.Room,
                                                             IsDeleted = u.IsDeleted,
                                                             IsArchived = u.IsArchived,
                                                             CompanyID = u.CompanyID,
                                                             UDF1 = u.UDF1,
                                                             UDF2 = u.UDF2,
                                                             UDF3 = u.UDF3,
                                                             UDF4 = u.UDF4,
                                                             UDF5 = u.UDF5,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             Description = u.Description,
                                                             ReceivedOn = u.ReceivedOn,
                                                             ReceivedOnWeb = u.ReceivedOnWeb,
                                                             AddedFrom = u.AddedFrom,
                                                             EditedFrom = u.EditedFrom,
                                                             //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Work Orders")
                                                             IsSignatureCapture = u.IsSignatureCapture,
                                                             IsSignatureRequired = u.IsSignatureRequired,
                                                             SignatureName = u.SignatureName,
                                                             ReleaseNumber = u.ReleaseNumber,
                                                             SupplierId = u.SupplierId,
                                                             SupplierAccountGuid = u.SupplierAccountGuid,
                                                             CreatedFrom = u.CreatedFrom,
                                                             UsedItemsSellPrice = u.UsedItemsSellPrice
                                                         }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<WorkOrderDTO>>.AddCacheItem("Cached_WorkOrder_" + CompanyID.ToString(), obj);
                    }
                }
            }
            else
            {
                //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                //IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<WorkOrderDTO>(
                                @" SELECT 
                                A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                                A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                                A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                                A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType, 
                                B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName 
                                ,T.ToolName as Tool,AST.AssetName as Asset, A.Description
                                ,A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,A.CustomerGUID
                                ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                                FROM WorkOrder A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                                LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID
                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new WorkOrderDTO
                                {
                                    ID = u.ID,
                                    GUID = u.GUID,
                                    WOName = u.WOName,
                                    SubJob = u.SubJob,
                                    TechnicianID = u.TechnicianID,
                                    Technician = u.Technician,
                                    CustomerID = u.CustomerID,
                                    Customer = u.Customer,
                                    CustomerGUID = u.CustomerGUID,
                                    AssetGUID = u.AssetGUID,
                                    AssetName = u.AssetName,
                                    ToolGUID = u.ToolGUID,
                                    ToolName = u.ToolName,
                                    Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                                    UsedItems = u.UsedItems.GetValueOrDefault(0),
                                    UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                                    JobTypeID = u.JobTypeID,
                                    GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                                    WOType = u.WOType,
                                    WOStatus = u.WOStatus,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    Description = u.Description,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Work Orders")
                                    IsSignatureCapture = u.IsSignatureCapture,
                                    IsSignatureRequired = u.IsSignatureRequired,
                                    SignatureName = u.SignatureName,
                                    SupplierId = u.SupplierId,
                                    SupplierAccountGuid = u.SupplierAccountGuid,
                                    CreatedFrom = u.CreatedFrom,
                                    UsedItemsSellPrice = u.UsedItemsSellPrice
                                }).AsParallel().ToList();
                }
            }
            return ObjCache.Where(t => t.Room == RoomID);
        }
        public IEnumerable<WorkOrderDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsDeleted, bool IsArchived)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }
        public IEnumerable<WorkOrderDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string WOType)
        {
            //Get Cached-Media
            IEnumerable<WorkOrderDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<WorkOrderDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && ((Fields[1].Split('@')[32] == "") || (Fields[1].Split('@')[32].Split(',').ToList().Contains(t.AssetGUID.ToString())))
                    && ((Fields[1].Split('@')[35] == "") || (Fields[1].Split('@')[35].Split(',').ToList().Contains(t.ToolGUID.ToString())))
                    && ((Fields[1].Split('@')[33] == "") || (Fields[1].Split('@')[33].Split(',').ToList().Contains(t.CustomerID.ToString())))
                    && ((Fields[1].Split('@')[34] == "") || (Fields[1].Split('@')[34].Split(',').ToList().Contains(t.TechnicianID.ToString())))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<WorkOrderDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.WOName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.WOName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public WorkOrderDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).SingleOrDefault(t => t.ID == id);
        }
        public WorkOrderDTO GetRecord(string WorkOrderGUID, Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).SingleOrDefault(t => t.GUID.ToString() == WorkOrderGUID);
        }
        public WorkOrderDTO GetRecord(string WorkOrderGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<WorkOrderDTO>(
                                        @" SELECT 
                                A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                                A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                                A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                                A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType, 
                                B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName 
                                ,T.ToolName as Tool,AST.AssetName as Asset, A.Description
                                ,A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,
                                
                            (SELECT ',' + proj.ProjectSpendName FROM RequisitionMaster as req
                                LEFT OUTER JOIN ProjectMaster as proj on proj.GUID = req.ProjectSpendGUID WHERE   A.GUID = req.WorkorderGUID FOR XML PATH(''))  as ProjectSpendName

                           ,A.CustomerGUID
                                ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.ReleaseNumber,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                                FROM WorkOrder A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                                LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID

                          

                                LEFT OUTER JOIN Room D on A.Room = D.ID  
                                WHERE A.GUID = '" + WorkOrderGUID + "'")
                        select new WorkOrderDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            WOName = u.WOName,
                            SubJob = u.SubJob,
                            TechnicianID = u.TechnicianID,
                            Technician = u.Technician,
                            CustomerID = u.CustomerID,
                            Customer = u.Customer,
                            CustomerGUID = u.CustomerGUID,
                            AssetGUID = u.AssetGUID,
                            AssetName = u.AssetName,
                            ToolGUID = u.ToolGUID,
                            ToolName = u.ToolName,
                            Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                            UsedItems = u.UsedItems.GetValueOrDefault(0),
                            UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                            JobTypeID = u.JobTypeID,
                            GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                            WOType = u.WOType,
                            WOStatus = u.WOStatus,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            Description = u.Description,
                            IsSignatureCapture = u.IsSignatureCapture,
                            IsSignatureRequired = u.IsSignatureRequired,
                            SignatureName = u.SignatureName,
                            ProjectSpendName = u.ProjectSpendName,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReleaseNumber = u.ReleaseNumber,
                            SupplierId = u.SupplierId,
                            SupplierAccountGuid = u.SupplierAccountGuid,
                            CreatedFrom = u.CreatedFrom,
                            UsedItemsSellPrice = u.UsedItemsSellPrice
                        }).SingleOrDefault();
            }
        }
        public WorkOrderDTO GetRecord(Int64 RoomID, string WorkOrderName, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<WorkOrderDTO>(
                                    @" SELECT 
                                A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                                A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                                A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                                A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType, 
                                B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName 
                                ,T.ToolName as Tool,AST.AssetName as Asset, A.Description
                                ,A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,
                               A.CustomerGUID
                                ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom, A.ReleaseNumber,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                                FROM WorkOrder A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                                LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID

                              

                                LEFT OUTER JOIN Room D on A.Room = D.ID  
                                WHERE LTRIM(RTRIM(A.WOName)) = LTRIM(RTRIM('" + WorkOrderName + "')) and A.Room= " + RoomID + " and A.CompanyID= " + CompanyID + " and isnull(A.IsDeleted,0)=0")
                        select new WorkOrderDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            WOName = u.WOName,
                            SubJob = u.SubJob,
                            TechnicianID = u.TechnicianID,
                            Technician = u.Technician,
                            CustomerID = u.CustomerID,
                            Customer = u.Customer,
                            CustomerGUID = u.CustomerGUID,
                            AssetGUID = u.AssetGUID,
                            AssetName = u.AssetName,
                            ToolGUID = u.ToolGUID,
                            ToolName = u.ToolName,
                            Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                            UsedItems = u.UsedItems.GetValueOrDefault(0),
                            UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                            JobTypeID = u.JobTypeID,
                            GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                            WOType = u.WOType,
                            WOStatus = u.WOStatus,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            Description = u.Description,
                            IsSignatureCapture = u.IsSignatureCapture,
                            IsSignatureRequired = u.IsSignatureRequired,
                            SignatureName = u.SignatureName,
                            // ProjectSpendName = u.ProjectSpendName,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReleaseNumber = u.ReleaseNumber,
                            SupplierId = u.SupplierId,
                            SupplierAccountGuid = u.SupplierAccountGuid,
                            CreatedFrom = u.CreatedFrom,
                            UsedItemsSellPrice = u.UsedItemsSellPrice
                        }).FirstOrDefault();
            }
        }
        public WorkOrderDTO GetRecord(string WorkOrderGUID, Int64 RoomID, Int64 CompanyID, string DBConnection)
        {
            using (var context = new eTurnsEntities(DBConnection))
            {
                return (from u in context.ExecuteStoreQuery<WorkOrderDTO>(
                                    @" SELECT 
                                A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                                A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                                A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                                A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType, 
                                B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName 
                                ,T.ToolName as Tool,AST.AssetName as Asset, A.Description
                                ,A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,A.CustomerGUID
                                ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom, A.ReleaseNumber,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                                FROM WorkOrder A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                                LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID
                                LEFT OUTER JOIN Room D on A.Room = D.ID  
                                WHERE A.GUID = '" + WorkOrderGUID + "'")
                        select new WorkOrderDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            WOName = u.WOName,
                            SubJob = u.SubJob,
                            TechnicianID = u.TechnicianID,
                            Technician = u.Technician,
                            CustomerID = u.CustomerID,
                            Customer = u.Customer,
                            CustomerGUID = u.CustomerGUID,
                            AssetGUID = u.AssetGUID,
                            AssetName = u.AssetName,
                            ToolGUID = u.ToolGUID,
                            ToolName = u.ToolName,
                            Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                            UsedItems = u.UsedItems.GetValueOrDefault(0),
                            UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                            JobTypeID = u.JobTypeID,
                            GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                            WOType = u.WOType,
                            WOStatus = u.WOStatus,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            Description = u.Description,
                            IsSignatureCapture = u.IsSignatureCapture,
                            IsSignatureRequired = u.IsSignatureRequired,
                            SignatureName = u.SignatureName,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReleaseNumber = u.ReleaseNumber,
                            SupplierId = u.SupplierId,
                            SupplierAccountGuid = u.SupplierAccountGuid,
                            CreatedFrom = u.CreatedFrom,
                            UsedItemsSellPrice = u.UsedItemsSellPrice
                        }).SingleOrDefault();
            }
        }
        
        public IEnumerable<WorkOrderDTO> GetCachedNarrowDataWorkOrder(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                //                var empnamesEnum = (from u in context.ExecuteStoreQuery<WorkOrderDTO>(@"Select proj.WOName,proj.GUID,COUNT(pm.ID) from PullMaster as pm
                //                                left join WorkOrder as proj on pm.WorkOrderDetailGUID=proj.GUID
                //                                Where pm.Room=" + RoomID.ToString() + " and pm.WorkOrderDetailGUID is not null and pm.CompanyID = " + CompanyID.ToString() +
                //                                    "group by proj.WOName,proj.GUID,proj.IsArchived,proj.IsDeleted")
                //                                    select new WorkOrderDTO
                //                                    {
                //                                        WOName = u.WOName,
                //                                        IsArchived = u.IsArchived,
                //                                        IsDeleted = u.IsDeleted,
                //                                        GUID= u.GUID

                //                                    }).AsQueryable().ToList();
                var empnamesEnum = (from pm in context.PullMasters
                                    join WO in context.WorkOrders on pm.WorkOrderDetailGUID equals WO.GUID
                                    where pm.Room == RoomID && pm.WorkOrderDetailGUID != null && pm.CompanyID == CompanyID
                                    select new
                                    {
                                        WOName = WO.WOName,
                                        IsArchived = WO.IsArchived,
                                        IsDeleted = WO.IsDeleted,
                                        GUID = WO.GUID
                                    }
                    ).AsQueryable().ToList().Select(x => new WorkOrderDTO
                    {
                        WOName = x.WOName,
                        IsArchived = x.IsArchived,
                        IsDeleted = x.IsDeleted,
                        GUID = x.GUID
                    });
                return empnamesEnum.ToList<WorkOrderDTO>();
            }
        }
        
        public double? GetWorkOrderTotalCost(long RoomID, long CompanyID, Guid WoGUID, int PriseSelectionOption)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var Result = (from PM in context.PullMasters
                              join IM in context.ItemMasters on PM.ItemGUID equals IM.GUID
                              join cuom in context.CostUOMMasters on IM.CostUOMID equals cuom.ID into cuom_join
                              from CUOM in cuom_join.DefaultIfEmpty()
                              where PM.WorkOrderDetailGUID == WoGUID
                                    && PM.CompanyID == CompanyID
                                    && PM.Room == RoomID
                                    && (PM.IsDeleted == null || PM.IsDeleted == false)
                                    && (IM.IsDeleted == null || IM.IsDeleted == false)
                              select new
                              {
                                  WorkOrderDetailGUID = PM.WorkOrderDetailGUID,
                                  Cost = (IM.Cost == null ? 0 : IM.Cost) * PM.PoolQuantity,
                                  SellPrice = (IM.SellPrice == null ? 0 : IM.SellPrice) * PM.PoolQuantity,
                                  CostUOMValue = (CUOM == null || CUOM.CostUOMValue == null || CUOM.CostUOMValue == 0 ? 1 : CUOM.CostUOMValue)
                              }).GroupBy(x => x.WorkOrderDetailGUID).Select(G => new
                              {
                                  GUID = G.Key,
                                  TotalCost = G.Sum(x => (x.Cost / x.CostUOMValue)),
                                  TotalSellPrice = G.Sum(x => (x.SellPrice / x.CostUOMValue))
                              }).FirstOrDefault();

                if (Result != null)
                {
                    if (PriseSelectionOption == 1)
                    {
                        return Result.TotalSellPrice;
                    }
                    else
                    {
                        return Result.TotalCost;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }


        public List<WorkOrderDTO> GetAllWorkOrders(long RoomId, long CompanyId, string[] WOTypes)
        {
            List<WorkOrderDTO> lsyWos = new List<WorkOrderDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lsyWos = (from wo in context.WorkOrders
                          join cm in context.CustomerMasters on wo.CustomerGUID equals cm.GUID into wo_cm_join
                          from wo_cm in wo_cm_join.DefaultIfEmpty()
                          join tm in context.TechnicianMasters on wo.TechnicianID equals tm.ID into wo_tm_join
                          from wo_tm in wo_tm_join.DefaultIfEmpty()
                          join am in context.AssetMasters on wo.AssetGUID equals am.GUID into wo_am_join
                          from wo_am in wo_am_join.DefaultIfEmpty()
                          join tom in context.ToolMasters on wo.ToolGUID equals tom.GUID into wo_tom_join
                          from wo_tom in wo_tom_join.DefaultIfEmpty()
                          join wocrtr in context.UserMasters on wo.CreatedBy equals wocrtr.ID into wo_wocrtr_join
                          from wo_wocrtr in wo_wocrtr_join.DefaultIfEmpty()
                          join wouptr in context.UserMasters on wo.LastUpdatedBy equals wouptr.ID into wo_wouptr_join
                          from wo_wouptr in wo_wouptr_join.DefaultIfEmpty()
                          join woSupp in context.SupplierMasters on wo.SupplierId equals woSupp.ID into wo_woSupp_join
                          from wo_woSupp in wo_woSupp_join.DefaultIfEmpty()
                          where WOTypes.Contains(wo.WOType) && wo.Room == RoomId && wo.CompanyID == CompanyId && (wo.IsArchived ?? false) == false && (wo.IsDeleted ?? false) == false
                          select new WorkOrderDTO
                          {
                              ID = wo.ID,
                              GUID = wo.GUID,
                              WOName = wo.WOName,
                              SubJob = string.Empty,
                              TechnicianID = wo.TechnicianID,
                              Technician = wo.Technician,
                              CustomerID = wo.CustomerID,
                              Customer = wo_cm.Customer,
                              AssetGUID = wo.AssetGUID,
                              AssetName = wo_am.AssetName,
                              ToolGUID = wo.ToolGUID,
                              ToolName = wo_tom.ToolName,
                              Odometer_OperationHours = wo.Odometer_OperationHours,
                              UsedItems = wo.UsedItems,
                              UsedItemsCost = wo.UsedItemsCost,
                              //JobTypeID = wo.JobTypeID,
                              //GXPRConsigmentJobID = wo.GXPRConsigmentJobID,
                              WOType = wo.WOType == null ? "" : wo.WOType,
                              WOStatus = wo.WOStatus,
                              Created = wo.Created,
                              Updated = wo.Updated,
                              CreatedBy = wo.CreatedBy,
                              LastUpdatedBy = wo.LastUpdatedBy,
                              Room = wo.Room,
                              IsDeleted = wo.IsDeleted,
                              IsArchived = wo.IsArchived,
                              CompanyID = wo.CompanyID,
                              UDF1 = wo.UDF1,
                              UDF2 = wo.UDF2,
                              UDF3 = wo.UDF3,
                              UDF4 = wo.UDF4,
                              UDF5 = wo.UDF5,
                              CreatedByName = wo_wocrtr.UserName,
                              UpdatedByName = wo_wouptr.UserName,
                              Description = wo.Description,
                              IsSignatureCapture = wo.IsSignatureCapture,
                              IsSignatureRequired = wo.IsSignatureRequired,
                              SignatureName = wo.SignatureName,
                              CustomerGUID = wo.CustomerGUID,
                              ReceivedOn = wo.ReceivedOn,
                              ReceivedOnWeb = wo.ReceivedOnWeb,
                              AddedFrom = wo.AddedFrom,
                              EditedFrom = wo.EditedFrom,
                              ReleaseNumber = wo.ReleaseNumber,
                              SupplierId = wo.SupplierId,
                              SupplierAccountGuid = wo.SupplierAccountGuid,
                              SupplierName = wo_woSupp.SupplierName,
                              CreatedFrom = wo.CreatedFrom,
                              UsedItemsSellPrice = wo.UsedItemsSellPrice
                          }).ToList();
            }
            return lsyWos;
        }

        public IEnumerable<WorkOrderDTO> GetAllRecordsOnlyImages()
        {
            IEnumerable<WorkOrderDTO> ObjCache = null;
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<WorkOrderDTO> obj = (from u in context.ExecuteStoreQuery<WorkOrderDTO>(@"SELECT distinct A.Id,A.WOName,A.Room,A.GUID,A.CompanyID
                        FROM WorkOrder A left join
						WorkOrderImageDetail WI on WI.WorkOrderGuid = A.GUID
                        where  Isnull(WI.WOImageName,'') != ''

                        ")
                                                 select new WorkOrderDTO
                                                 {
                                                     ID = u.ID,
                                                     Room = u.Room,
                                                     GUID = u.GUID,
                                                     CompanyID = u.CompanyID

                                                 }).AsParallel().ToList();
                ObjCache = obj;
                return obj;
            }




        }

        public IEnumerable<WorkOrderImageDetail> GetImagesUsingWOGuid(Guid WorkOrderGuid)
        {
            IEnumerable<WorkOrderImageDetail> ObjCache = null;
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<WorkOrderImageDetail> obj = (from u in context.ExecuteStoreQuery<WorkOrderImageDetail>(@"SELECT Wi.*
                    From
						WorkOrderImageDetail WI 
                        where  WI.WorkOrderGuid ='" + WorkOrderGuid.ToString() + @"'
                        ")
                                                         select new WorkOrderImageDetail
                                                         {
                                                             ID = u.ID,
                                                             WOImageName = u.WOImageName,
                                                             WorkOrderGuid = WorkOrderGuid
                                                         }).AsParallel().ToList();
                ObjCache = obj;
                return obj;
            }

        }

        public List<WorkOrderDTO> GetWorkOrdersByRoomBasic(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByRoomBasic] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<WorkOrderDTO> GetWorkOrdersByRoomPlain(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByRoomPlain] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<WorkOrderDTO> GetWorkOrdersByType(long RoomID, long Company, string[] WOStatus)
        {
            List<WorkOrderDTO> lstWO = new List<WorkOrderDTO>();
            lstWO = GetWorkOrdersByRoomPlain(RoomID, Company);
            if (lstWO != null && lstWO.Count > 0)
            {
                lstWO = lstWO.Where(wo => (wo.WOName ?? string.Empty) != string.Empty && WOStatus.Contains(wo.WOType)).ToList();
                return lstWO;
            }

            return lstWO;
        }

        public WorkOrderDTO GetWorkOrdersByGUIDBasic(Guid WOGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByGUIDBasic] @WOGUID", params1).FirstOrDefault();
            }
        }

        public WorkOrderDTO GetWorkOrdersByIDBasic(long WOID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOID", WOID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByIDBasic] @WOID", params1).FirstOrDefault();
            }
        }

        public List<WorkOrderDTO> GetWorkOrdersByNameSearchPlain(string WoName, long? RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WoName", WoName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByNameSearchPlain] @WoName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public WorkOrderDTO GetWorkOrdersByNamePlainSingle(string WoName, long RoomID, long CompanyID)
        {
            return GetWorkOrdersByNamePlain(WoName, RoomID, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public WorkOrderDTO GetWorkOrdersByNamePlainByStatus(string WoName, long RoomID, long CompanyID, string WOStatus)
        {
            return GetWorkOrdersByNamePlain(WoName, RoomID, CompanyID).Where(t => (t.WOStatus ?? string.Empty).ToLower() == (WOStatus ?? string.Empty).ToLower()).OrderByDescending(t => t.ID).FirstOrDefault();
        }

        public List<WorkOrderDTO> GetWorkOrdersByGUIdsJoins(string WOIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOIDs", WOIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByGUIdsJoins] @WOIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<WorkOrderDTO> GetWorkOrdersByGUIdsPlain(string WOIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOIDs", WOIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByGUIdsPlain] @WOIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<WorkOrderDTO> GetWorkOrdersByIdsPlain(string WOIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOIDs", WOIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByIdsPlain] @WOIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        
        public WorkOrderDTO GetWorkOrdersByIDFullJoins(long WOID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOID", WOID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<WorkOrderDTO>("exec [GetWorkOrdersByIDFullJoins] @WOID", params1).FirstOrDefault();
            }
        }

        public WorkOrderDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<WorkOrderDTO>(@"
                    SELECT 
                    A.HistoryID,A.Action,A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                    A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                    A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                    A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType,
                    B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName
                    ,T.ToolName as Tool,AST.AssetName as Asset, A.Description,A.CustomerGUID
                    ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.ReleaseNumber,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                    FROM WorkOrder_History A
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                    LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID
                WHERE A.HistoryID=" + id.ToString())
                        select new WorkOrderDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            WOName = u.WOName,
                            HistoryID = u.HistoryID,
                            SubJob = u.SubJob,
                            TechnicianID = u.TechnicianID,
                            Technician = u.Technician,
                            CustomerID = u.CustomerID,
                            CustomerGUID = u.CustomerGUID,
                            Customer = u.Customer,
                            AssetGUID = u.AssetGUID,
                            AssetName = u.AssetName,
                            ToolGUID = u.ToolGUID,
                            ToolName = u.ToolName,
                            Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                            UsedItems = u.UsedItems.GetValueOrDefault(0),
                            UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                            JobTypeID = u.JobTypeID,
                            GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                            WOType = u.WOType,
                            WOStatus = u.WOStatus,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Description = u.Description,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReleaseNumber = u.ReleaseNumber,
                            SupplierId = u.SupplierId,
                            SupplierAccountGuid = u.SupplierAccountGuid,
                            CreatedFrom = u.CreatedFrom,
                            UsedItemsSellPrice = u.UsedItemsSellPrice
                        }).SingleOrDefault();
            }
        }

        public WorkOrderDTO GetHistoryRecordForMaintenance(Int64 WorkOrderID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<WorkOrderDTO>(@"
                SELECT 
                    A.HistoryID,A.Action,A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,
                    A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,
                    A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID,
                    A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.UsedItemsSellPrice,A.WOType,
                    B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName
                    ,T.ToolName as Tool,AST.AssetName as Asset, A.Description,A.CustomerGUID
                    ,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.ReleaseNumber,A.SupplierId,A.SupplierAccountGuid,A.CreatedFrom
                    FROM WorkOrder_History A
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID
                    LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID 
                WHERE A.ID=" + WorkOrderID.ToString())
                        select new WorkOrderDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            WOName = u.WOName,
                            HistoryID = u.HistoryID,
                            SubJob = u.SubJob,
                            TechnicianID = u.TechnicianID,
                            Technician = u.Technician,
                            CustomerID = u.CustomerID,
                            Customer = u.Customer,
                            CustomerGUID = u.CustomerGUID,
                            AssetGUID = u.AssetGUID,
                            AssetName = u.AssetName,
                            ToolGUID = u.ToolGUID,
                            ToolName = u.ToolName,
                            Odometer_OperationHours = u.Odometer_OperationHours.GetValueOrDefault(0),
                            UsedItems = u.UsedItems.GetValueOrDefault(0),
                            UsedItemsCost = u.UsedItemsCost.GetValueOrDefault(0),
                            JobTypeID = u.JobTypeID,
                            GXPRConsigmentJobID = u.GXPRConsigmentJobID,
                            WOType = u.WOType,
                            WOStatus = u.WOStatus,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Description = u.Description,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReleaseNumber = u.ReleaseNumber,
                            SupplierId = u.SupplierId,
                            SupplierAccountGuid = u.SupplierAccountGuid,
                            CreatedFrom = u.CreatedFrom,
                            UsedItemsSellPrice = u.UsedItemsSellPrice
                        }).LastOrDefault();
            }
        }

        public string DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID, long SessionUserId)
        {
            int ctrdelete = 0;
            int ctrExist = 0;
            string msg = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var item in IDs.Split(','))
                {
                    string strQuery = "";
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {

                        PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);

                        if (!objPullDetailsDAL.CheckPullExist(System.Guid.Parse(item)))
                        {

                            strQuery += "UPDATE WorkOrder SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                            context.ExecuteStoreCommand(strQuery);
                            ctrdelete += 1;
                            PullMasterDAL objPullDAL = new PullMasterDAL(base.DataBaseName);
                            var pullIds = objPullDAL.GetPullIdsByWorkOrderDetailGuid(Guid.Parse(item), RoomID, CompanyID);

                            if (pullIds != null && pullIds.Any())
                            {
                                objPullDAL.DeletePullsByPullIds(string.Join(",", pullIds.Select(i => i.ToString()).ToArray()), userid, CompanyID, RoomID, SessionUserId);
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
                    msg = ctrdelete + " record(s) deleted successfully.";
                }
                if (ctrExist > 0)
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = ctrExist + " record(s) used in another module.";
                    }
                    else
                    {
                        msg = msg + " " + ctrExist + " record(s) used in another module.";
                    }
                }
                return msg;
            }
        }

        public WorkOrderDTO GetWoByGUId(Guid WOID, long RoomId, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrderDTO objWorkOrderDTO = new WorkOrderDTO();
                objWorkOrderDTO = (from wo in context.WorkOrders
                                   join tm in context.ToolMasters on wo.ToolGUID equals tm.GUID into wo_tm_join
                                   from wo_tm in wo_tm_join.DefaultIfEmpty()
                                   join am in context.AssetMasters on wo.AssetGUID equals am.GUID into wo_am_join
                                   from wo_am in wo_am_join.DefaultIfEmpty()
                                   join req in context.RequisitionMasters on wo.GUID equals req.WorkorderGUID into wo_req_join
                                   from wo_req in wo_req_join.DefaultIfEmpty()

                                   join proj in context.ProjectMasters on wo_req.ProjectSpendGUID equals proj.GUID into req_proj_join
                                   from req_proj in req_proj_join.DefaultIfEmpty()

                                   join cc in context.UserMasters on wo.CreatedBy equals cc.ID into ci_cc_join
                                   from ci_cc in ci_cc_join.DefaultIfEmpty()
                                   join cu in context.UserMasters on wo.LastUpdatedBy equals cu.ID into ci_cu_join
                                   from ci_cu in ci_cu_join.DefaultIfEmpty()
                                   join rm in context.Rooms on wo.Room equals rm.ID into ci_rm_join
                                   from ci_rm in ci_rm_join.DefaultIfEmpty()
                                   where wo.GUID == WOID && wo.Room == RoomId && wo.CompanyID == CompanyID
                                   //&& (wo.IsDeleted ?? false) == false && (wo.IsArchived ?? false) == false
                                   select new WorkOrderDTO
                                   {
                                       ID = wo.ID,
                                       GUID = wo.GUID,
                                       WOName = wo.WOName,
                                       //SubJob = wo.SubJob,
                                       TechnicianID = wo.TechnicianID,
                                       Technician = wo.Technician,
                                       CustomerID = wo.CustomerID,
                                       Customer = wo.Customer,
                                       AssetGUID = wo.AssetGUID,
                                       AssetName = wo_am.AssetName,
                                       ToolGUID = wo.ToolGUID,
                                       ToolName = wo_tm.ToolName,
                                       Odometer_OperationHours = wo.Odometer_OperationHours ?? 0,
                                       UsedItems = wo.UsedItems ?? 0,
                                       UsedItemsCost = wo.UsedItemsCost ?? 0,
                                       //JobTypeID = wo.JobTypeID,
                                       //GXPRConsigmentJobID = wo.GXPRConsigmentJobID,
                                       WOType = wo.WOType == null ? "" : wo.WOType,
                                       WOStatus = wo.WOStatus,
                                       Created = wo.Created,
                                       Updated = wo.Updated,
                                       CreatedBy = wo.CreatedBy,
                                       LastUpdatedBy = wo.LastUpdatedBy,
                                       Room = wo.Room,
                                       IsDeleted = wo.IsDeleted,
                                       IsArchived = wo.IsArchived,
                                       CompanyID = wo.CompanyID,
                                       UDF1 = wo.UDF1,
                                       UDF2 = wo.UDF2,
                                       UDF3 = wo.UDF3,
                                       UDF4 = wo.UDF4,
                                       UDF5 = wo.UDF5,
                                       CreatedByName = ci_cc.UserName,
                                       UpdatedByName = ci_cu.UserName,
                                       RoomName = ci_rm.RoomName,
                                       Description = wo.Description,
                                       RequisitionNumber = wo_req.RequisitionNumber,
                                       IsSignatureCapture = wo.IsSignatureCapture,
                                       IsSignatureRequired = wo.IsSignatureRequired,
                                       SignatureName = wo.SignatureName,
                                       ProjectSpendName = req_proj.ProjectSpendName,
                                       CustomerGUID = wo.CustomerGUID,
                                       ReceivedOn = wo.ReceivedOn,
                                       ReceivedOnWeb = wo.ReceivedOnWeb,
                                       AddedFrom = wo.AddedFrom,
                                       EditedFrom = wo.EditedFrom,
                                       ReleaseNumber = wo.ReleaseNumber,
                                       SupplierId = wo.SupplierId,
                                       SupplierAccountGuid = wo.SupplierAccountGuid,
                                       CreatedFrom = wo.CreatedFrom,
                                       UsedItemsSellPrice = wo.UsedItemsSellPrice
                                   }).FirstOrDefault();
                return objWorkOrderDTO;
            }


        }

        public WorkOrderDTO GetWoByWorkOrderName(string WorkOrderName, long RoomId, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrderDTO objWorkOrderDTO = new WorkOrderDTO();
                objWorkOrderDTO = (from wo in context.WorkOrders
                                   join tm in context.ToolMasters on wo.ToolGUID equals tm.GUID into wo_tm_join
                                   from wo_tm in wo_tm_join.DefaultIfEmpty()
                                   join am in context.AssetMasters on wo.AssetGUID equals am.GUID into wo_am_join
                                   from wo_am in wo_am_join.DefaultIfEmpty()
                                   join req in context.RequisitionMasters on wo.GUID equals req.WorkorderGUID into wo_req_join
                                   from wo_req in wo_req_join.DefaultIfEmpty()

                                   join proj in context.ProjectMasters on wo_req.ProjectSpendGUID equals proj.GUID into req_proj_join
                                   from req_proj in req_proj_join.DefaultIfEmpty()

                                   join cc in context.UserMasters on wo.CreatedBy equals cc.ID into ci_cc_join
                                   from ci_cc in ci_cc_join.DefaultIfEmpty()
                                   join cu in context.UserMasters on wo.LastUpdatedBy equals cu.ID into ci_cu_join
                                   from ci_cu in ci_cu_join.DefaultIfEmpty()
                                   join rm in context.Rooms on wo.Room equals rm.ID into ci_rm_join
                                   from ci_rm in ci_rm_join.DefaultIfEmpty()
                                   where wo.WOName.Trim().ToUpper() == WorkOrderName.Trim().ToUpper() && wo.Room == RoomId && wo.CompanyID == CompanyID
                                   //&& (wo.IsDeleted ?? false) == false && (wo.IsArchived ?? false) == false
                                   select new WorkOrderDTO
                                   {
                                       ID = wo.ID,
                                       GUID = wo.GUID,
                                       WOName = wo.WOName,
                                       //SubJob = wo.SubJob,
                                       TechnicianID = wo.TechnicianID,
                                       Technician = wo.Technician,
                                       CustomerID = wo.CustomerID,
                                       Customer = wo.Customer,
                                       AssetGUID = wo.AssetGUID,
                                       AssetName = wo_am.AssetName,
                                       ToolGUID = wo.ToolGUID,
                                       ToolName = wo_tm.ToolName,
                                       Odometer_OperationHours = wo.Odometer_OperationHours ?? 0,
                                       UsedItems = wo.UsedItems ?? 0,
                                       UsedItemsCost = wo.UsedItemsCost ?? 0,
                                       //JobTypeID = wo.JobTypeID,
                                       //GXPRConsigmentJobID = wo.GXPRConsigmentJobID,
                                       WOType = wo.WOType == null ? "" : wo.WOType,
                                       WOStatus = wo.WOStatus,
                                       Created = wo.Created,
                                       Updated = wo.Updated,
                                       CreatedBy = wo.CreatedBy,
                                       LastUpdatedBy = wo.LastUpdatedBy,
                                       Room = wo.Room,
                                       IsDeleted = wo.IsDeleted,
                                       IsArchived = wo.IsArchived,
                                       CompanyID = wo.CompanyID,
                                       UDF1 = wo.UDF1,
                                       UDF2 = wo.UDF2,
                                       UDF3 = wo.UDF3,
                                       UDF4 = wo.UDF4,
                                       UDF5 = wo.UDF5,
                                       CreatedByName = ci_cc.UserName,
                                       UpdatedByName = ci_cu.UserName,
                                       RoomName = ci_rm.RoomName,
                                       Description = wo.Description,
                                       RequisitionNumber = wo_req.RequisitionNumber,
                                       IsSignatureCapture = wo.IsSignatureCapture,
                                       IsSignatureRequired = wo.IsSignatureRequired,
                                       SignatureName = wo.SignatureName,
                                       ProjectSpendName = req_proj.ProjectSpendName,
                                       CustomerGUID = wo.CustomerGUID,
                                       ReceivedOn = wo.ReceivedOn,
                                       ReceivedOnWeb = wo.ReceivedOnWeb,
                                       AddedFrom = wo.AddedFrom,
                                       EditedFrom = wo.EditedFrom,
                                       ReleaseNumber = wo.ReleaseNumber,
                                       SupplierId = wo.SupplierId,
                                       SupplierAccountGuid = wo.SupplierAccountGuid,
                                       CreatedFrom = wo.CreatedFrom,
                                       UsedItemsSellPrice = wo.UsedItemsSellPrice
                                   }).FirstOrDefault();
                return objWorkOrderDTO;
            }


        }

        public IEnumerable<WorkOrderDTO> GetRequistionByWO(Guid WOGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var empnamesEnum = (from WO in context.WorkOrders
                                    join Req in context.RequisitionMasters on WO.GUID equals Req.WorkorderGUID
                                    where WO.Room == RoomID && WO.GUID == WOGUID && WO.CompanyID == CompanyID
                                    select new
                                    {
                                        WOName = WO.WOName,
                                        RequisitionNumber = Req.RequisitionNumber,
                                        IsArchived = WO.IsArchived,
                                        IsDeleted = WO.IsDeleted,
                                        GUID = WO.GUID,

                                    }
                    ).AsQueryable().ToList().Select(x => new WorkOrderDTO
                    {
                        WOName = x.WOName,
                        RequisitionNumber = x.RequisitionNumber,
                        IsArchived = x.IsArchived,
                        IsDeleted = x.IsDeleted,
                        GUID = x.GUID
                    });
                return empnamesEnum.ToList<WorkOrderDTO>();
            }
        }

        public IEnumerable<WorkOrderDTO> GetAllWorkOrderReport(bool IsArchived, bool IsDeleted, string StartDate, string EndDate, string QuantityType = "")
        {

            IEnumerable<WorkOrderDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //string Query = "SELECT A.ID,A.GUID,A.WOName,A.TechnicianID,A.Technician,A.CustomerID,A.Customer,A.AssetGUID,A.ToolGUID,A.WOStatus,A.Odometer_OperationHours,A.Created,"+
                //    "A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Room,A.IsDeleted,A.IsArchived,A.CompanyID, A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UsedItems,A.UsedItemsCost,A.WOType,"+
                //    "B.UserName AS 'CreatedByName',C.UserName AS UpdatedByName, D.RoomName,T.ToolName as Tool,AST.AssetName as Asset, A.Description "+
                //    ",A.SignatureName,A.IsSignatureCapture,A.IsSignatureRequired,A.CustomerGUID"+
                //    ",A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom "+
                //    "FROM WorkOrder A "+
                //    "LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID "+
                //    "LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID "+
                //    "LEFT OUTER JOIN toolmaster T on A.ToolGUID = T.GUID "+
                //    "LEFT OUTER JOIN AssetMaster AST on A.AssetGUID = AST.GUID "+
                //    "LEFT OUTER JOIN Room D on A.Room = D.ID "+
                //    "WHERE A.IsDeleted!=1 AND A.IsArchived != 1";

                //string Query = @"SELECT Distinct Wo.room,WO.GUID,WO.WOName,WO.WOStatus,Wo.SupplierId,Wo.SupplierAccountGuid FROM WorkOrder WO Left outer join  PULLMASTER PM ON PM.WorkOrderDetailGUID = WO.[Guid]
                //                WHERE 1=1 ";
                //if (StartDate != null)
                //{
                //    Query += " And  PM.Created >= Convert(DateTime,'" + StartDate + "')";
                //}
                //if (EndDate != null)
                //{
                //    Query += " And  PM.Created <= Convert(DateTime,'" + EndDate + "')";
                //}

                var params1 = new SqlParameter[] { new SqlParameter("@StartDate", StartDate ?? (object)DBNull.Value), new SqlParameter("@EndDate", EndDate ?? (object)DBNull.Value), new SqlParameter("@QuantityType", QuantityType ?? (object)DBNull.Value) };

                string Query = @"exec GetWorkOrderListForReport @StartDate,@EndDate,@QuantityType";

                obj = (from u in context.ExecuteStoreQuery<WorkOrderDTO>(Query, params1)
                       select new WorkOrderDTO
                       {
                           GUID = u.GUID,
                           WOName = u.WOName,
                           WOStatus = u.WOStatus,
                           Room = u.Room,
                           SupplierId = u.SupplierId,
                           SupplierAccountGuid = u.SupplierAccountGuid
                       }).AsParallel().ToList();

            }

            return obj;
        }

        public Guid GetWorkOrderGuid(Int64 Id, long RoomId, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrder obj = context.WorkOrders.FirstOrDefault(t => t.ID == Id && t.Room == RoomId && t.CompanyID == CompanyID);
                return obj.GUID;
            }


        }
    }
}


