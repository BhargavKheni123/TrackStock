using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public partial class CommonDAL : eTurnsBaseDAL
    {
        public Dictionary<string, int> GetNarrowDDData(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab, string RoomDateFormat, int LoadDataCount, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone, List<RoomDTO> RoomList, List<CompanyMasterDTO> CompanyList, Int64 RoleID, List<EnterpriseDTO> EnterPriseList, List<BinMasterDTO> AllBins, List<CompanyMasterDTO> AllCompanies, bool? Session_ConsignedAllowed, List<BOMItemDTO> lstBomIterms, int? Session_QuicklistType, List<ReportMasterDTO> ReportMasterList, List<ToolWrittenOffDTO> RoomAllWrittenOffTool, List<ToolAssetOrderDetailsDTO> ToolAssetOrderDetail, List<ToolDetailDTO> ToolKitDetail, string ToolORDType, string ToolType, Int64 Session_EnterPriceID, List<OrderDetailsDTO> lstDetailDTO = null, string MainFilter = "", string QuickListType = "1", long[] RoomsAccess = null, bool IsIncludeClosedOrder = true, string ItemModelCallFrom = "", string EnterpriseIds = "", bool IsAllowConsignedCredit = true, string ModuleGuid = "", Int64 ParentID = 0, MoveType? moveType = null, bool IsSLCount = false, long UserID = 0, string requestFor = "")
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            switch (TableName)
            {

                case "BOMList":
                    return GetNarrowDDData_BOMList(TextFieldName, RoomDateFormat, CurrentTimeZone, RoomID, CompanyID, lstBomIterms);
                case "SupplierCatalog":
                    return GetNarrowDDData_SupplierCatalog(TextFieldName, RoomID, CompanyID, ItemModelCallFrom);
                case "TechnicianMaster":
                    return GetNarrowDDData_TechnicianMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "BinMaster":
                    return GetNarrowDDData_BinMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, AllBins);
                case "CategoryMaster":
                    return GetNarrowDDData_CategoryMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "BomCategoryMaster":
                    return GetNarrowDDData_BomCategoryMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "CustomerMaster":
                    return GetNarrowDDData_CustomerMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                //case "FreightTypeMaster":
                //    return GetNarrowDDData_FreightTypeMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "GLAccountMaster":
                    return GetNarrowDDData_GLAccountMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "BomGLAccountMaster":
                    return GetNarrowDDData_BomGLAccountMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "GXPRConsigmentJobMaster":
                    return GetNarrowDDData_GXPRConsigmentJobMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "JobTypeMaster":
                    return GetNarrowDDData_JobTypeMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "LocationMaster":
                    return GetNarrowDDData_LocationMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "ManufacturerMaster":
                    return GetNarrowDDData_ManufacturerMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "WrittenOffCategory":
                    return GetNarrowDDData_WrittenOffCategoryMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "BomManufacturerMaster":
                    return GetNarrowDDData_BomManufacturerMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "MeasurementTerm":
                    return GetNarrowDDData_MeasurementTerm(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "ShipViaMaster":
                    return GetNarrowDDData_ShipViaMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);

                case "SupplierMaster":
                    return GetNarrowDDData_SupplierMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "BomSupplierMaster":
                    return GetNarrowDDData_BomSupplierMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);

                case "ToolCategoryMaster":
                    return GetNarrowDDData_ToolCategoryMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "ToolMaster":
                case "KitToolMaster":
                    return GetNarrowDDData_ToolAndKitToolMaster(TableName, TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, RequisitionCurrentTab, ToolKitDetail, ToolType, LoadDataCount);
                case "ToolMasterNew":
                case "KitToolMasterNew":
                    return GetNarrowDDData_ToolAndKitToolMasterNew(TableName, TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, ToolKitDetail, ToolType, LoadDataCount, RequisitionCurrentTab);

                case "ToolAssetOrder":
                    return GetNarrowDDData_ToolAssetOrder(TableName, TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, ToolAssetOrderDetail, ToolORDType);
                case "ToolHistory":
                    return GetNarrowDDData_ToolHistory(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, LoadDataCount);
                case "ToolHistoryNew":
                    return GetNarrowDDData_ToolHistoryNew(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, LoadDataCount);


                case "OrderMaster":
                    return GetNarrowDDData_OrderMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "ReceiveMaster":
                    return GetNarrowDDData_ReceiveMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, IsIncludeClosedOrder, SupplierIds);
                case "ReceiveToolAssetMaster":
                    return GetNarrowDDData_ReceiveToolAssetMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, IsIncludeClosedOrder);

                case "ToolScheduleMapping":

                    return GetNarrowDDData_ToolScheduleMapping(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);

                case "ToolsMaintenanceDetails":
                    return GetNarrowDDData_ToolsMaintenanceDetails(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);


                case "EnterpriseMaster":

                    break;

                case "CompanyMaster":
                    return GetNarrowDDData_CompanyMaster(TextFieldName, IsArchived, IsDeleted, CompanyList, RoleID, AllCompanies);
                case "InventoryCountList":
                    return GetNarrowDDData_InventoryCountList(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);

                case "RoleMaster":
                    return GetNarrowDDData_RoleMaster(TextFieldName, CompanyID, RoomID);
                case "UserMaster":
                    return GetNarrowDDData_UserMaster(TextFieldName, CompanyID, RoomID);
                case "QuickListMaster":
                    return GetNarrowDDData_QuickListMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "ItemMaster":
                    return GetNarrowDDData_ItemMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, ItemModelCallFrom, RoomDateFormat, SupplierIds,LoadDataCount, Session_ConsignedAllowed, Session_QuicklistType, lstDetailDTO, QuickListType, ParentID, IsAllowConsignedCredit, IsSLCount, moveType,requestFor);
                case "ItemBinMaster":
                    return GetNarrowDDData_ItemMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, "ItemBinMaster", RoomDateFormat, SupplierIds, LoadDataCount, Session_ConsignedAllowed, Session_QuicklistType, lstDetailDTO, QuickListType, ParentID, IsAllowConsignedCredit, IsSLCount, moveType);
                case "BOMItemMaster":
                    BOMItemMasterDAL objBOMItemDAL = new BOMItemMasterDAL(base.DataBaseName);
                    #region GET DATA
                    //BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(base.DataBaseName);
                    IEnumerable<RequisitionMasterNarrowSearchDTO> objItemMasterBinDTO = objBOMItemDAL.GetAllNarrowSearchRecords(TextFieldName, SupplierIds, CompanyID, IsArchived, IsDeleted);
                    if (objItemMasterBinDTO != null && objItemMasterBinDTO.Count() > 0)
                    {
                        ColUDFData = (from tmp in objItemMasterBinDTO
                                      group tmp by new { tmp.NarrowSearchText, tmp.TotalCount }
                                          into grp
                                      select new
                                      {
                                          NarrowSearchText = grp.Key.NarrowSearchText,
                                          TotalCount = grp.Key.TotalCount ?? 0
                                      }
                              ).AsParallel().ToDictionary(e => e.NarrowSearchText, e => e.TotalCount);
                    }
                    return ColUDFData;

                #endregion

                case "PullMaster":
                    bool ConsignedAllowed = true;
                    if (Session_ConsignedAllowed != null)
                    {
                        ConsignedAllowed = (Session_ConsignedAllowed ?? true);
                    }

                    return GetNarrowDDData_PullMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, SupplierIds, ConsignedAllowed, LoadDataCount,UserID);
                case "Room":
                    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                    List<RoomDTO> lstRooms = new List<RoomDTO>();
                    List<EnterpriseDTO> oEnterPriseList = EnterPriseList;
                    string UserEnterPrises = string.Empty;

                    if (oEnterPriseList == null)
                        oEnterPriseList = new List<EnterpriseDTO>();

                    long RoleId = -1;
                    long.TryParse(Convert.ToString(RoleID), out RoleId);

                    if (RoleId != -1)
                    {
                        UserEnterPrises = string.Join(",", oEnterPriseList.Select(t => t.ID).ToArray());
                    }

                    if (!string.IsNullOrWhiteSpace(UserEnterPrises))
                    {
                        if (!string.IsNullOrWhiteSpace(EnterpriseIds))
                        {
                            var selected = (from c in EnterpriseIds.Split(',')
                                            join nc in UserEnterPrises.Split(',') on c equals nc
                                            select c);

                            EnterpriseIds = string.Join(",", selected);
                            if (string.IsNullOrWhiteSpace(EnterpriseIds))
                                EnterpriseIds = "0";
                        }
                        else
                        {
                            EnterpriseIds = UserEnterPrises;
                        }
                    }

                    lstRooms = objRoomDAL.GetAllRoomsFromETurnsMaster(CompanyID, IsDeleted, IsArchived, RoomList, EnterpriseIds, RoleID, Session_EnterPriceID).ToList();

                    #region GET DATA
                    if (TextFieldName == "LastUpdatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            var lstsupps = (from ci in lstRooms
                                            where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                            orderby ci.UpdatedByName
                                            group ci by new { ci.LastUpdatedBy, ci.UpdatedByName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.LastUpdatedBy,
                                                supname = grpms.Key.UpdatedByName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);


                        }
                    }
                    else if (TextFieldName == "CreatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in lstRooms
                                            where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                            orderby ci.UpdatedByName
                                            group ci by new { ci.CreatedBy, ci.CreatedByName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.CreatedByName
                                            });
                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else if (TextFieldName == "EnterpriseName")
                    {
                        var lstsupps = (from ci in lstRooms
                                        where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                        orderby ci.EnterpriseName
                                        group ci by new { ci.EnterpriseId, ci.EnterpriseName } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.EnterpriseId,
                                            supname = grpms.Key.EnterpriseName
                                        });
                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                    }
                    else if (TextFieldName == "CompanyName")
                    {
                        var lstsupps = (from ci in lstRooms
                                        where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                        orderby ci.CompanyName
                                        group ci by new { ci.CompanyID, ci.CompanyName } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.CompanyID,
                                            supname = grpms.Key.CompanyName
                                        });
                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                    }
                    else if (TextFieldName == "InvoiceBranch")
                    {
                        var lstsupps = (from ci in lstRooms
                                        where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && (ci.CostCenter ?? string.Empty) != string.Empty
                                        orderby ci.CostCenter
                                        group ci by new { ci.CostCenter } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.CostCenter,
                                            supname = grpms.Key.CostCenter
                                        });
                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                    }
                    else if (TextFieldName == "BillingRoomType")
                    {
                        var lstsupps = (from ci in lstRooms
                                        where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && (ci.BillingRoomType ?? 0) > 0
                                        orderby ci.CostCenter
                                        group ci by new { ci.BillingRoomType } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.BillingRoomType.ToString(),
                                            supname = grpms.Key.BillingRoomType.ToString()
                                        });
                        //return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        var lstsuppsD = lstsupps.OrderBy(t => t.supname).ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        return lstsuppsD;
                    }
                    return ColUDFData;
                #endregion
                case "UnitMaster":
                    UnitMasterDAL unitMasterDAL = new UnitMasterDAL(base.DataBaseName);
                    return unitMasterDAL.GetUnitListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, false).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "BomUnitMaster":
                    UnitMasterDAL bomUnitMasterDAL = new UnitMasterDAL(base.DataBaseName);
                    return bomUnitMasterDAL.GetUnitListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, true).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "KitMaster":
                    KitMasterDAL kitMasterDAL = new KitMasterDAL(base.DataBaseName);
                    return kitMasterDAL.GetKitListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "ProjectList":
                    ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL(base.DataBaseName);
                    return objProjectMasterDAL.GetProjectListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "RequisitionMaster":
                    RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(base.DataBaseName);
                    return objRequisitionMasterDAL.GetRequistionMasterNarrowSearchRecords(TextFieldName, RequisitionCurrentTab, RoomID, CompanyID, SupplierIds, IsArchived, IsDeleted).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "ItemMasterBinList":
                    ItemMasterBinDAL objItemMasterBinDAL = new ItemMasterBinDAL(base.DataBaseName);
                    IEnumerable<RequisitionMasterNarrowSearchDTO> objBOMItemMasterBinDTO = objItemMasterBinDAL.GetAllNarrowSearchRecords(TextFieldName, RoomID, CompanyID, IsArchived, IsDeleted);
                    if (objBOMItemMasterBinDTO != null && objBOMItemMasterBinDTO.Count() > 0)
                    {
                        ColUDFData = (from tmp in objBOMItemMasterBinDTO
                                      group tmp by new { tmp.NarrowSearchText, tmp.TotalCount }
                                          into grp
                                      select new
                                      {
                                          NarrowSearchText = grp.Key.NarrowSearchText,
                                          TotalCount = grp.Key.TotalCount ?? 0
                                      }
                              ).AsParallel().ToDictionary(e => e.NarrowSearchText, e => e.TotalCount);
                    }
                    return ColUDFData;

                case "WorkOrder":

                    WorkOrderDAL objWorkOrderDAL = new WorkOrderDAL(base.DataBaseName);
                    return objWorkOrderDAL.GetWorkOrderListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, SupplierIds, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                case "VenderMaster":
                    VenderMasterDAL venderMasterDAL = new VenderMasterDAL(base.DataBaseName);
                    return venderMasterDAL.GetVendorListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "AssetMaster":
                    AssetMasterDAL objAsetDAL = new AssetMasterDAL(base.DataBaseName);
                    return objAsetDAL.GetAssetListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue, e => e.NSCount);
                case "MaterialStaging":
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        if (TextFieldName == "BinName")
                        {
                            var lstStagingBins = (from ms in context.MaterialStagings
                                                  //join bm in context.BinMasters on ms.BinID equals bm.ID
                                                  where !string.IsNullOrEmpty(ms.StagingLocationName) && ms.StagingLocationName != "[|EmptyStagingBin|]" && ms.Room == RoomID && ms.CompanyID == CompanyID && ms.IsArchived == IsArchived && ms.IsDeleted == IsDeleted
                                                  orderby ms.StagingLocationName
                                                  group ms by new { ms.StagingLocationName } into grpms
                                                  select new
                                                  {
                                                      count = grpms.Count(),
                                                      bid = grpms.Key.StagingLocationName,
                                                      BinName = grpms.Key.StagingLocationName
                                                  });


                            return lstStagingBins.OrderBy(t => t.BinName).AsParallel().ToDictionary(e => e.BinName + "[###]" + e.bid.ToString(), e => (int)e.count);
                        }
                        if (TextFieldName == "CreatedBy")
                        {
                            var lstCreaters = (from ms in context.MaterialStagings
                                               join usr in context.UserMasters on ms.CreatedBy equals usr.ID
                                               where ms.CreatedBy != null && ms.CreatedBy > 0 && ms.Room == RoomID && ms.CompanyID == CompanyID && ms.IsArchived == IsArchived && ms.IsDeleted == IsDeleted
                                               orderby usr.UserName
                                               group ms by new { ms.CreatedBy, usr.UserName } into grpms
                                               select new
                                               {
                                                   count = grpms.Count(),
                                                   uid = grpms.Key.CreatedBy,
                                                   UserName = grpms.Key.UserName
                                               });


                            return lstCreaters.OrderBy(t => t.UserName).AsParallel().ToDictionary(e => e.UserName + "[###]" + e.uid.ToString(), e => (int)e.count);
                        }
                        if (TextFieldName == "LastUpdatedBy")
                        {
                            var lstUpdators = (from ms in context.MaterialStagings
                                               join usr in context.UserMasters on ms.LastUpdatedBy equals usr.ID
                                               where ms.LastUpdatedBy != null && ms.LastUpdatedBy > 0 && ms.Room == RoomID && ms.CompanyID == CompanyID && ms.IsArchived == IsArchived && ms.IsDeleted == IsDeleted
                                               orderby usr.UserName
                                               group ms by new { ms.LastUpdatedBy, usr.UserName } into grpms
                                               select new
                                               {
                                                   count = grpms.Count(),
                                                   uid = grpms.Key.LastUpdatedBy,
                                                   UserName = grpms.Key.UserName
                                               });

                            return lstUpdators.OrderBy(t => t.UserName).AsParallel().ToDictionary(e => e.UserName + "[###]" + e.uid.ToString(), e => (int)e.count);

                        }
                    }
                    return ColUDFData;
                case "CartItem":

                case "CartItemList":
                //bool CartConsignedAllowed = true;
                //if (HttpContext.Current.Session["ConsignedAllowed"] != null)
                //{
                //    CartConsignedAllowed = Convert.ToBoolean(HttpContext.Current.Session["ConsignedAllowed"]);
                //}
                //return GetNarrowDDData_CartItem(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, SupplierIds, CartConsignedAllowed, LoadDataCount, ReplenishType);

                case "CostUOMMaster":
                    CostUOMMasterDAL objCostDAL = new CostUOMMasterDAL(base.DataBaseName);
                    return objCostDAL.GetCostUOMListListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "BomCostUOMMaster":
                    CostUOMMasterDAL objBOMCostDAL = new CostUOMMasterDAL(base.DataBaseName);
                    return objBOMCostDAL.GetBOMCostUOMListListNarrowSearch(CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "InventoryClassificationMaster":
                    InventoryClassificationMasterDAL objinvDAL = new InventoryClassificationMasterDAL(base.DataBaseName);
                    return objinvDAL.GetInventoryClassificationListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, false).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "BomInventoryClassificationMaster":

                    InventoryClassificationMasterDAL objBinvDAL = new InventoryClassificationMasterDAL(base.DataBaseName);
                    return objBinvDAL.GetInventoryClassificationListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, true).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                case "AssetToolSchedulerList":
                    #region GET DATA
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        if (TextFieldName == "CreatedBy")
                        {
                            var lstCreaters = (from ms in context.ToolsSchedulers
                                               join usr in context.UserMasters on ms.CreatedBy equals usr.ID
                                               where ms.CreatedBy != null && ms.CreatedBy > 0 && ms.Room == RoomID && ms.CompanyID == CompanyID && ms.IsArchived == IsArchived && ms.IsDeleted == IsDeleted
                                               orderby usr.UserName
                                               group ms by new { ms.CreatedBy, usr.UserName } into grpms
                                               select new
                                               {
                                                   count = grpms.Count(),
                                                   uid = grpms.Key.CreatedBy,
                                                   UserName = grpms.Key.UserName
                                               });


                            return lstCreaters.OrderBy(t => t.UserName).AsParallel().ToDictionary(e => e.UserName + "[###]" + e.uid.ToString(), e => (int)e.count);
                        }
                        if (TextFieldName == "LastUpdatedBy")
                        {
                            var lstUpdators = (from ms in context.ToolsSchedulers
                                               join usr in context.UserMasters on ms.LastUpdatedBy equals usr.ID
                                               where ms.LastUpdatedBy != null && ms.LastUpdatedBy > 0 && ms.Room == RoomID && ms.CompanyID == CompanyID && ms.IsArchived == IsArchived && ms.IsDeleted == IsDeleted
                                               orderby usr.UserName
                                               group ms by new { ms.LastUpdatedBy, usr.UserName } into grpms
                                               select new
                                               {
                                                   count = grpms.Count(),
                                                   uid = grpms.Key.LastUpdatedBy,
                                                   UserName = grpms.Key.UserName
                                               });

                            return lstUpdators.OrderBy(t => t.UserName).AsParallel().ToDictionary(e => e.UserName + "[###]" + e.uid.ToString(), e => (int)e.count);

                        }
                    }
                    return ColUDFData;
                #endregion
                case "AssetCategoryMaster":
                    return GetNarrowDDData_AssetCategoryMaster(TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted);
                case "EnterpriseQLMaster":
                    return GetNarrowDDData_EnterpriseQuickListMaster(TextFieldName, IsDeleted);
                case "NotificationMasterList":
                    NotificationDAL objNotificationDAL = new NotificationDAL(base.DataBaseName);
                    #region GET DATA
                    if (TextFieldName == "LastUpdatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in context.Notifications
                                            join um in context.UserMasters on ci.UpdatedBy equals um.ID
                                            where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && ci.UpdatedBy > 0
                                            orderby um.UserName
                                            group ci by new { ci.UpdatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.UpdatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else if (TextFieldName == "NotificationType")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            if (RoleID >= 0)
                            {
                                List<long> lstsreport = (from ci in context.ReportMasters
                                                         where ci.ReportName == "EnterpriseRoom" || ci.ReportName == "EnterpriseUser"
                                                         select ci.ID).ToList<long>();

                                var lstsupps = (from ci in context.Notifications
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
                                                && lstsreport.Contains(ci.ReportID ?? 0) == false
                                                group ci by new { ci.ScheduleFor } into grpms
                                                select new
                                                {
                                                    count = grpms.Count(),
                                                    sid = grpms.Key.ScheduleFor,
                                                    supname = grpms.Key.ScheduleFor == 5 ? "Reports" : "Alerts"
                                                });

                                return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                            }
                            else
                            {
                                var lstsupps = (from ci in context.Notifications
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
                                                group ci by new { ci.ScheduleFor } into grpms
                                                select new
                                                {
                                                    count = grpms.Count(),
                                                    sid = grpms.Key.ScheduleFor,
                                                    supname = grpms.Key.ScheduleFor == 5 ? "Reports" : "Alerts"
                                                });

                                return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                            }
                        }
                    }
                    else if (TextFieldName == "EmailTemplate")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            if (RoleID >= 0)
                            {
                                List<long> lstsreport = (from ci in context.ReportMasters
                                                         where ci.ReportName == "EnterpriseRoom" || ci.ReportName == "EnterpriseUser"
                                                         select ci.ID).ToList<long>();

                                var lstsupps = (from ci in context.Notifications
                                                join um in context.EmailTemplates on ci.EmailTemplateID equals um.ID
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && (ci.EmailTemplateID ?? 0) > 0
                                                && lstsreport.Contains(ci.ReportID ?? 0) == false
                                                orderby um.TemplateName
                                                group ci by new { ci.EmailTemplateID, um.TemplateName, um.ResourceKeyName } into grpms
                                                select new NarrowSearchSceduleDTO
                                                {
                                                    NSCount = grpms.Count(),
                                                    NSColumnValue = grpms.Key.EmailTemplateID ?? 0,
                                                    NSColumnText = grpms.Key.TemplateName,
                                                    ResourceKeyName = grpms.Key.ResourceKeyName
                                                }).ToList();
                                lstsupps.ForEach(x =>
                                {
                                    if(!string.IsNullOrWhiteSpace(x.ResourceKeyName))
                                    {
                                        x.NSColumnText = ResourceHelper.GetAlertNameByResource(x.ResourceKeyName);
                                    }
                                }
                                );
                                return lstsupps.OrderBy(t => t.NSColumnText).AsParallel().ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => (int)e.NSCount);
                            }
                            else
                            {
                                var lstsupps = (from ci in context.Notifications
                                                join um in context.EmailTemplates on ci.EmailTemplateID equals um.ID
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && (ci.EmailTemplateID ?? 0) > 0
                                                orderby um.TemplateName
                                                group ci by new { ci.EmailTemplateID, um.TemplateName,um.ResourceKeyName } into grpms
                                                select new NarrowSearchSceduleDTO
                                                {
                                                    NSCount = grpms.Count(),
                                                    NSColumnValue = grpms.Key.EmailTemplateID ?? 0,
                                                    NSColumnText = grpms.Key.TemplateName,
                                                    ResourceKeyName = grpms.Key.ResourceKeyName
                                                }).ToList();
                                lstsupps.ForEach(x =>
                                {
                                    if (!string.IsNullOrWhiteSpace(x.ResourceKeyName))
                                    {
                                        x.NSColumnText = ResourceHelper.GetAlertNameByResource(x.ResourceKeyName);
                                    }
                                });

                                return lstsupps.OrderBy(t => t.NSColumnText).AsParallel().ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => (int)e.NSCount);
                            }
                        }
                    }
                    else if (TextFieldName == "Schedule")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            if (RoleID >= 0)
                            {

                                List<long> lstsreport = (from ci in context.ReportMasters
                                                         where ci.ReportName == "EnterpriseRoom" || ci.ReportName == "EnterpriseUser"
                                                         select ci.ID).ToList<long>();


                                var lstsupps = (from ci in context.Notifications
                                                join um in context.RoomSchedules on ci.RoomScheduleID equals um.ScheduleID
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
                                                && lstsreport.Contains(ci.ReportID ?? 0) == false
                                                orderby um.ScheduleName
                                                group ci by new { ci.RoomScheduleID, um.ScheduleName, ci.ReportID } into grpms
                                                select new
                                                {
                                                    count = grpms.Count(),
                                                    sid = grpms.Key.RoomScheduleID,
                                                    supname = grpms.Key.ScheduleName,
                                                    reportID = grpms.Key.ReportID
                                                });


                                return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                            }
                            else
                            {
                                var lstsupps = (from ci in context.Notifications
                                                join um in context.RoomSchedules on ci.RoomScheduleID equals um.ScheduleID
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
                                                orderby um.ScheduleName
                                                group ci by new { ci.RoomScheduleID, um.ScheduleName } into grpms
                                                select new
                                                {
                                                    count = grpms.Count(),
                                                    sid = grpms.Key.RoomScheduleID,
                                                    supname = grpms.Key.ScheduleName
                                                });

                                return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                            }
                        }
                    }
                    else if (TextFieldName == "Report")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            if (RoleID >= 0)
                            {
                                var lstsupps = (from ci in context.Notifications
                                                join um in context.ReportMasters on ci.ReportID equals um.ID
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && (ci.ReportID ?? 0) > 0
                                                && um.ReportName != "EnterpriseRoom" && um.ReportName != "EnterpriseUser"
                                                orderby um.ReportName
                                                group ci by new { ci.ReportID, um.ReportName, um.ResourceKey } into grpms
                                                select new NarrowSearchSceduleDTO
                                                {
                                                    NSCount = grpms.Count(),
                                                    NSColumnValue = grpms.Key.ReportID ?? 0,
                                                    NSColumnText = grpms.Key.ReportName,
                                                    ResourceKeyName = grpms.Key.ResourceKey
                                                }).ToList();
                                lstsupps.ForEach(x =>
                                {
                                    if (!string.IsNullOrWhiteSpace(x.ResourceKeyName))
                                    {
                                        x.NSColumnText = ResourceHelper.GetReportNameByResource(x.ResourceKeyName);
                                    }
                                });

                                return lstsupps.OrderBy(t => t.NSColumnText).AsParallel().ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => (int)e.NSCount);
                            }
                            else
                            {
                                var lstsupps = (from ci in context.Notifications
                                                join um in context.ReportMasters on ci.ReportID equals um.ID
                                                where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && (ci.ReportID ?? 0) > 0
                                                orderby um.ReportName
                                                group ci by new { ci.ReportID, um.ReportName, um.ResourceKey } into grpms
                                                select new NarrowSearchSceduleDTO
                                                {
                                                    NSCount = grpms.Count(),
                                                    NSColumnValue = grpms.Key.ReportID ?? 0,
                                                    NSColumnText = grpms.Key.ReportName,
                                                    ResourceKeyName = grpms.Key.ResourceKey
                                                }).ToList();
                                lstsupps.ForEach(x =>
                                {
                                    if (!string.IsNullOrWhiteSpace(x.ResourceKeyName))
                                    {
                                        x.NSColumnText = ResourceHelper.GetReportNameByResource(x.ResourceKeyName);
                                    }
                                });

                                return lstsupps.OrderBy(t => t.NSColumnText).AsParallel().ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => (int)e.NSCount);
                            }
                        }
                    }
                    else
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in context.Notifications
                                            join um in context.UserMasters on ci.CreatedBy equals um.ID
                                            where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && ci.CreatedBy > 0
                                            orderby um.UserName
                                            group ci by new { ci.CreatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.UserName
                                            });
                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                #endregion
                case "FTPMasterList":
                    #region GET DATA
                    if (TextFieldName == "LastUpdatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in context.FTPMasters
                                            join um in context.UserMasters on ci.UpdatedBy equals um.ID
                                            where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && ci.UpdatedBy > 0
                                            orderby um.UserName
                                            group ci by new { ci.UpdatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.UpdatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from ci in context.FTPMasters
                                            join um in context.UserMasters on ci.CreatedBy equals um.ID
                                            where ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID && ci.CreatedBy > 0
                                            orderby um.UserName
                                            group ci by new { ci.CreatedBy, um.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.UserName
                                            });
                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                #endregion
                case "PermissionTemplateList":
                    if (TextFieldName == "LastUpdatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from pt in context.PermissionTemplateMasters
                                            join umc in context.UserMasters on pt.UpdatedBy equals umc.ID
                                            where pt.IsDeleted == IsDeleted && pt.UpdatedBy > 0
                                            orderby umc.UserName
                                            group pt by new { pt.UpdatedBy, umc.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.UpdatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else if (TextFieldName == "CreatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from pt in context.PermissionTemplateMasters
                                            join umc in context.UserMasters on pt.CreatedBy equals umc.ID
                                            where pt.IsDeleted == IsDeleted && pt.CreatedBy > 0
                                            orderby umc.UserName
                                            group pt by new { pt.CreatedBy, umc.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    return ColUDFData;
                case "MoveMaterial":
                    #region GET DATA
                    if (TextFieldName == "CreatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from mm in context.MoveMaterialMasters
                                            join umc in context.UserMasters on mm.CreatedBy equals umc.ID
                                            where mm.IsDeleted == IsDeleted && mm.CreatedBy > 0
                                            && mm.CompanyID == CompanyID && mm.RoomID == RoomID
                                            orderby umc.UserName
                                            group mm by new { mm.CreatedBy, umc.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from mm in context.MoveMaterialMasters
                                            join umc in context.UserMasters on mm.UpdatedBy equals umc.ID
                                            where mm.IsDeleted == IsDeleted && mm.UpdatedBy > 0
                                            && mm.CompanyID == CompanyID && mm.RoomID == RoomID
                                            orderby umc.UserName
                                            group mm by new { mm.UpdatedBy, umc.UserName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.UpdatedBy,
                                                supname = grpms.Key.UserName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                #endregion
                case "BarcodeMaster":
                    BarcodeMasterDAL objBarcodeDAL = new BarcodeMasterDAL(base.DataBaseName);
                    #region GET DATA
                    if (TextFieldName == "CreatedBy")
                    {
                        ColUDFData = (from tmp in objBarcodeDAL.GetAllRecordsUsingModuleId(RoomID, CompanyID, IsDeleted, IsArchived, ModuleGuid).Where(t => t.CreatedByName != null)
                                      orderby tmp.CreatedByName
                                      group tmp by new { tmp.CreatedByName, tmp.CreatedBy }
                                          into grp
                                      select new
                                      {
                                          CreatedByName = grp.Key.CreatedByName + "[###]" + grp.Key.CreatedBy,
                                          Count = grp.Count()
                                      }
                          ).AsParallel().ToDictionary(e => e.CreatedByName, e => e.Count);
                        return ColUDFData;
                    }
                    else if (TextFieldName == "ModuleType")
                    {
                        ColUDFData = (from tmp in objBarcodeDAL.GetAllRecordsUsingModuleId(RoomID, CompanyID, IsDeleted, IsArchived, ModuleGuid).Where(t => t.ModuleName != null)
                                      orderby tmp.CreatedByName
                                      group tmp by new { tmp.ModuleName, tmp.ModuleGUID }
                                          into grp
                                      select new
                                      {
                                          ModuleName = grp.Key.ModuleName + "[###]" + grp.Key.ModuleGUID,
                                          Count = grp.Count()
                                      }
                         ).AsParallel().ToDictionary(e => e.ModuleName, e => e.Count);
                        return ColUDFData;
                    }
                    else if (TextFieldName == "Supplier")
                    {

                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            ColUDFData = (from tmp in objBarcodeDAL.GetAllRecordsSupplier(RoomID, CompanyID, IsDeleted, IsArchived, ModuleGuid)
                                          orderby tmp.SupplierName
                                          group tmp by new { tmp.SupplierName, tmp.ID, tmp.Count }
                                              into grp
                                          select new
                                          {
                                              ModuleName = grp.Key.SupplierName + "[###]" + grp.Key.ID,
                                              Count = grp.Count()
                                          }
                          ).AsParallel().ToDictionary(e => e.ModuleName, e => e.Count);
                            return ColUDFData;
                        }

                    }
                    else if (TextFieldName == "Category")
                    {

                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {
                            ColUDFData = (from tmp in objBarcodeDAL.GetAllRecordsCategory(RoomID, CompanyID, IsDeleted, IsArchived, ModuleGuid)
                                          orderby tmp.Category
                                          group tmp by new { tmp.Category, tmp.ID, tmp.Count }
                                              into grp
                                          select new
                                          {
                                              ModuleName = grp.Key.Category + "[###]" + grp.Key.ID,
                                              Count = grp.Count()
                                          }
                          ).AsParallel().ToDictionary(e => e.ModuleName, e => e.Count);
                            return ColUDFData;

                        }
                    }
                    else if (TextFieldName == "Items")
                    {
                        if (ModuleGuid == null || ModuleGuid == Convert.ToString(Guid.Empty) || (string.IsNullOrEmpty(ModuleGuid)))
                        {
                            ColUDFData = (from tmp in objBarcodeDAL.GetAllRecordsUsingModuleId(RoomID, CompanyID, IsDeleted, IsArchived, ModuleGuid).Where(t => t.items != null)
                                          orderby tmp.CreatedByName
                                          group tmp by new { tmp.items, tmp.RefGUID }
                                              into grp
                                          select new
                                          {
                                              items = grp.Key.items + "[###]" + grp.Key.RefGUID,
                                              Count = grp.Count()
                                          }
                             ).AsParallel().ToDictionary(e => e.items, e => e.Count);
                            return ColUDFData;
                        }
                        else
                        {
                            ColUDFData = (from tmp in objBarcodeDAL.GetAllRecordsUsingModuleId(RoomID, CompanyID, IsDeleted, IsArchived, ModuleGuid).Where(t => t.items != null)
                                          orderby tmp.CreatedByName
                                          group tmp by new { tmp.items, tmp.RefGUID }
                                              into grp
                                          select new
                                          {
                                              items = grp.Key.items + "[###]" + grp.Key.RefGUID,
                                              Count = grp.Count()
                                          }
                             ).AsParallel().ToDictionary(e => e.items, e => e.Count);
                            return ColUDFData;
                        }
                    }
                    else
                    {
                        ColUDFData = (from tmp in objBarcodeDAL.GetAllRecordsUsingModuleId(RoomID, CompanyID, IsDeleted, IsArchived, ModuleGuid).Where(t => t.UpdatedByName != null)
                                      orderby tmp.UpdatedByName
                                      group tmp by new { tmp.UpdatedByName, tmp.UpdatedBy }
                                          into grp
                                      select new
                                      {
                                          UpdatedByName = grp.Key.UpdatedByName + "[###]" + grp.Key.UpdatedBy,
                                          Count = grp.Count()
                                      }
                           ).AsParallel().ToDictionary(e => e.UpdatedByName, e => e.Count);


                        return ColUDFData;
                    }
                #endregion
                case "PullPoMasterList":
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        if (TextFieldName == "CreatedBy")
                        {
                            var lstCreaters = (from ms in context.PullPOMasters
                                               join usr in context.UserMasters on ms.CreatedBy equals usr.ID
                                               where ms.CreatedBy != null && ms.CreatedBy > 0 && ms.RoomId == RoomID && ms.CompanyID == CompanyID && ms.Isdeleted == IsDeleted
                                               orderby usr.UserName
                                               group ms by new { ms.CreatedBy, usr.UserName } into grpms
                                               select new
                                               {
                                                   count = grpms.Count(),
                                                   uid = grpms.Key.CreatedBy,
                                                   UserName = grpms.Key.UserName
                                               });


                            return lstCreaters.OrderBy(t => t.UserName).AsParallel().ToDictionary(e => e.UserName + "[###]" + e.uid.ToString(), e => (int)e.count);
                        }
                        if (TextFieldName == "LastUpdatedBy")
                        {
                            var lstUpdators = (from ms in context.PullPOMasters
                                               join usr in context.UserMasters on ms.UpdatedBy equals usr.ID
                                               where ms.UpdatedBy != null && ms.UpdatedBy > 0 && ms.RoomId == RoomID && ms.CompanyID == CompanyID && ms.Isdeleted == IsDeleted
                                               orderby usr.UserName
                                               group ms by new { ms.UpdatedBy, usr.UserName } into grpms
                                               select new
                                               {
                                                   count = grpms.Count(),
                                                   uid = grpms.Key.UpdatedBy,
                                                   UserName = grpms.Key.UserName
                                               });

                            return lstUpdators.OrderBy(t => t.UserName).AsParallel().ToDictionary(e => e.UserName + "[###]" + e.uid.ToString(), e => (int)e.count);

                        }
                    }
                    return ColUDFData;
                case "ReportMasterList":

                    List<ReportMasterDTO> lstReportMaster = ReportMasterList;


                    if (TextFieldName == "LastUpdatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from pt in lstReportMaster.Where(t => t.UpdatedByName != null && t.UpdatedByName != string.Empty)
                                            where (pt.IsDeleted ?? false) == IsDeleted && pt.UpdatedBy > 0
                                            orderby pt.UpdatedByName
                                            group pt by new { pt.UpdatedBy, pt.UpdatedByName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.UpdatedBy,
                                                supname = grpms.Key.UpdatedByName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    else if (TextFieldName == "CreatedBy")
                    {
                        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                        {

                            var lstsupps = (from pt in lstReportMaster.Where(t => t.CreatedByName != null && t.CreatedByName != string.Empty)
                                            where (pt.IsDeleted ?? false) == IsDeleted && pt.CreatedBy > 0
                                            orderby pt.CreatedByName
                                            group pt by new { pt.CreatedBy, pt.CreatedByName } into grpms
                                            select new
                                            {
                                                count = grpms.Count(),
                                                sid = grpms.Key.CreatedBy,
                                                supname = grpms.Key.CreatedByName
                                            });

                            return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                        }
                    }
                    return ColUDFData;
                case "WrittenOffToolList":
                    ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(base.DataBaseName);
                    return toolWrittenOffDAL.GetWrittenOffToolListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "QuoteMaster":
                    QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(base.DataBaseName);
                    return quoteMasterDAL.GetQuoteMasterNarrowSearchData(TextFieldName, RequisitionCurrentTab, RoomID, CompanyID, IsArchived, IsDeleted).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            return null;
        }

    }
}
