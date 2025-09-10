using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public partial class CommonDAL : eTurnsBaseDAL
    {
        public Dictionary<string, int> GetUDFDDData(string TableName, string UDFName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab,int LoadDataCount, List<long> SupplierIds,string RoomDateFormat, TimeZoneInfo CurrentTimeZone,List<RoomDTO> RoomList, List<CompanyMasterDTO> CompanyList,Int64 RoleID, List<BinMasterDTO> AllBins, List<CompanyMasterDTO> AllCompanies, List<RoomDTO> AllRooms,  bool Session_ConsignedAllowed, IEnumerable<ItemMasterDTO> ItemMasterListForCount,int? Session_QuicklistType,IEnumerable<MinMaxDataTableInfo> SessionMinMaxTableForNarrow, List<ToolDetailDTO> ToolKitDetail,string ToolType,Int64 Session_EnterPriceID, string ItemModelCallFrom = "", string EnterpriseIds = "", bool IsAllowConsignedCredit = true, string ModuleGuid = "", Int64 ParentID = 0, MoveType? moveType = null, bool NotIncludeDeletedUDF = false,string ToolCurrentTab="",long UserID=0,string RequestFor="")
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            UDFName = UDFName.Replace("_dd", "");
            switch (TableName)
            {
                case "TechnicianMaster":
                    TechnicialMasterDAL technicialMasterDAL = new TechnicialMasterDAL(base.DataBaseName);
                    return technicialMasterDAL.GetTechnicianListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "BinMaster":
                    BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
                    List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                    if (AllBins != null && AllBins.Count > 0)
                    {
                        lstBins = AllBins;
                    }
                    else
                    {
                        lstBins = objBinDAL.GetAllBins(RoomID, CompanyID, IsArchived, IsDeleted, string.Empty, null);
                    }
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        var lstsupps = (from ci in lstBins
                                        where ci.UDF1 != null && ci.UDF1 != string.Empty && (!string.IsNullOrEmpty(ci.UDF1))
                                        orderby ci.UDF1
                                        group ci by new { ci.UDF1 } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.UDF1,
                                            supname = grpms.Key.UDF1
                                        });

                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname, e => (int)e.count);
                    }
                    else if (UDFName == "UDF2")
                    {
                        var lstsupps = (from ci in lstBins
                                        where ci.UDF2 != null && ci.UDF2 != string.Empty && (!string.IsNullOrEmpty(ci.UDF2))
                                        orderby ci.UDF2
                                        group ci by new { ci.UDF2 } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.UDF2,
                                            supname = grpms.Key.UDF2
                                        });

                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname, e => (int)e.count);
                    }
                    else if (UDFName == "UDF3")
                    {
                        var lstsupps = (from ci in lstBins
                                        where ci.UDF3 != null && ci.UDF3 != string.Empty && (!string.IsNullOrEmpty(ci.UDF3))
                                        orderby ci.UDF3
                                        group ci by new { ci.UDF3 } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.UDF3,
                                            supname = grpms.Key.UDF3
                                        });

                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname, e => (int)e.count);
                    }
                    else if (UDFName == "UDF4")
                    {
                        var lstsupps = (from ci in lstBins
                                        where ci.UDF4 != null && ci.UDF4 != string.Empty && (!string.IsNullOrEmpty(ci.UDF4))
                                        orderby ci.UDF4
                                        group ci by new { ci.UDF4 } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.UDF4,
                                            supname = grpms.Key.UDF4
                                        });

                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname, e => (int)e.count);
                    }
                    else if (UDFName == "UDF5")
                    {
                        var lstsupps = (from ci in lstBins
                                        where ci.UDF5 != null && ci.UDF5 != string.Empty && (!string.IsNullOrEmpty(ci.UDF5))
                                        orderby ci.UDF5
                                        group ci by new { ci.UDF5 } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.UDF5,
                                            supname = grpms.Key.UDF5
                                        });

                        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname, e => (int)e.count);
                    }
                    break;
                #endregion
                case "CategoryMaster":
                    CategoryMasterDAL objCateDAL = new CategoryMasterDAL(base.DataBaseName);
                    return objCateDAL.GetCategoryListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "CustomerMaster":
                    CustomerMasterDAL objCustomerMasterDAL = new CustomerMasterDAL(base.DataBaseName);
                    return objCustomerMasterDAL.GetCustomerListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                //case "FreightTypeMaster":
                //    FreightTypeMasterDAL objFreiDAL = new FreightTypeMasterDAL(base.DataBaseName);
                //    return objFreiDAL.GetFreightTypeListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "GLAccountMaster":
                    GLAccountMasterDAL objGLAccDAL = new GLAccountMasterDAL(base.DataBaseName);
                    return objGLAccDAL.GetGLAccountListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, false).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "GXPRConsigmentJobMaster":
                    GXPRConsignedJobMasterDAL objGXPRConsiDAL = new GXPRConsignedJobMasterDAL(base.DataBaseName);
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1))).OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                        //var tempData = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF1 ASC").Select(t => t.UDF1).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF1 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        //return objBinDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF1 ASC").Select(t => t.UDF1).Distinct();
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2))).OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                        //var tempData = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF2 ASC").Select(t => t.UDF2).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF2 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))).OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);

                        //var tempData = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF3 ASC").Select(t => t.UDF3).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF3 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4))).OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                        //var tempData = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF4 ASC").Select(t => t.UDF4).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF4 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else
                    {
                        ColUDFData = (from tmp in objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5))).OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                        //var tempData = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF5 ASC").Select(t => t.UDF5).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF5 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                #endregion
                case "JobTypeMaster":
                    JobTypeMasterDAL objJobTypeDAL = new JobTypeMasterDAL(base.DataBaseName);
                    return objJobTypeDAL.GetJobTypeListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "LocationMaster":
                    LocationMasterDAL locationMasterDAL = new LocationMasterDAL(base.DataBaseName);
                    return locationMasterDAL.GetLocationListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "ManufacturerMaster":
                    ManufacturerMasterDAL objMFDAL = new ManufacturerMasterDAL(base.DataBaseName);
                    return objMFDAL.GetMFListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, false).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "MeasurementTerm":
                    MeasurementTermDAL objMeasureDAL = new MeasurementTermDAL(base.DataBaseName);
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in objMeasureDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1))).OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);

                        //var tempData = objMeasureDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF1 ASC").Select(t => t.UDF1).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF1 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        //return objBinDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF1 ASC").Select(t => t.UDF1).Distinct();
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in objMeasureDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2))).OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                        //var tempData = objMeasureDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF2 ASC").Select(t => t.UDF2).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF2 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in objMeasureDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))).OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                               ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);


                        //var tempData = objMeasureDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF3 ASC").Select(t => t.UDF3).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF3 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in objMeasureDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4))).OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                               ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);

                        //var tempData = objMeasureDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF4 ASC").Select(t => t.UDF4).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF4 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else
                    {
                        ColUDFData = (from tmp in objMeasureDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5))).OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                        //var tempData = objMeasureDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF5 ASC").Select(t => t.UDF5).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF5 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                #endregion
                case "ShipViaMaster":
                    ShipViaDAL objShipViaDAL = new ShipViaDAL(base.DataBaseName);
                    return objShipViaDAL.GetShipViaListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "SupplierMaster":
                    SupplierMasterDAL objSupDAL = new SupplierMasterDAL(base.DataBaseName);
                    return objSupDAL.GetSupplierListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, false).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "ToolCategoryMaster":
                    ToolCategoryMasterDAL objToolCateDAL = new ToolCategoryMasterDAL(base.DataBaseName);
                    return objToolCateDAL.GetToolCategoryListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "ToolMaster":
                case "KitToolMaster":
                case "ToolMasterNew":
                    ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);
                    string Type = "1";
                    if (TableName == "KitToolMaster")
                    {
                        Type = "2";
                    }
                    if (ToolType != null && (!string.IsNullOrWhiteSpace(ToolType)))
                    {
                        Type = ToolType;
                    }
                    #region UDF
                    List<ToolDetailDTO> objQLItems = null;
                    if (ToolKitDetail != null && ToolKitDetail.Count > 0)
                    {
                        objQLItems = ToolKitDetail;
                    }
                    string ToolIDs = "";
                    if (objQLItems != null && objQLItems.Count > 0)
                    {
                        foreach (var item in objQLItems)
                        {
                            if (!string.IsNullOrEmpty(ToolIDs))
                                ToolIDs += ",";

                            ToolIDs += item.ToolItemGUID.ToString();
                        }
                    }

                    return objToolDAL.GetToolListNarrowSearch(UDFName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, Type, NotIncludeDeletedUDF, RequisitionCurrentTab, LoadDataCount).ToDictionary(e => e.NSColumnText, e => e.NSCount);


                //IEnumerable<RequisitionMasterNarrowSearchDTO> objToolMasterDTO = objToolDAL.GetAllNarrowSearchRecordsForUDF(UDFName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, Type, NotIncludeDeletedUDF);
                //if (objToolMasterDTO != null && objToolMasterDTO.Count() > 0)
                //{
                //    ColUDFData = (from tmp in objToolMasterDTO
                //                  group tmp by new { tmp.NarrowSearchText, tmp.TotalCount }
                //                      into grp
                //                  select new
                //                  {
                //                      NarrowSearchText = grp.Key.NarrowSearchText,
                //                      TotalCount = grp.Key.TotalCount ?? 0
                //                  }
                //          ).AsParallel().ToDictionary(e => e.NarrowSearchText, e => e.TotalCount);
                //}
                //return ColUDFData;
                #endregion
                case "ToolCheckOUT":
                case "KitToolCheckOUT":
                    ToolMasterDAL objToolChkoutDAL = new ToolMasterDAL(base.DataBaseName);
                    #region UDF
                    Type = "1";
                    if (TableName == "KitToolCheckOUT")
                    {
                        Type = "2";
                    }
                    objQLItems = null;
                    if (ToolType != null && (!string.IsNullOrWhiteSpace(ToolType)))
                    {
                        Type = Convert.ToString(ToolType);
                    }
                    if (ToolKitDetail != null && ToolKitDetail.Count > 0)
                    {
                        objQLItems = ToolKitDetail;
                    }
                    ToolIDs = "";
                    if (objQLItems != null && objQLItems.Count > 0)
                    {
                        foreach (var item in objQLItems)
                        {
                            if (!string.IsNullOrEmpty(ToolIDs))
                                ToolIDs += ",";

                            ToolIDs += item.ToolItemGUID.ToString();
                        }
                    }
                    objToolDAL = new ToolMasterDAL(base.DataBaseName);

                    return objToolDAL.GetToolListNarrowSearchCheckOutUDF(UDFName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, Type, ToolCurrentTab, LoadDataCount).ToDictionary(e => e.NSColumnText, e => e.NSCount);


                //objToolMasterDTO = objToolDAL.GetAllNarrowSearchCheckOutUDFRecords(UDFName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, Type, ToolCurrentTab);
                //if (objToolMasterDTO != null && objToolMasterDTO.Count() > 0)
                //{
                //    ColUDFData = (from tmp in objToolMasterDTO
                //                  group tmp by new { tmp.NarrowSearchText, tmp.TotalCount }
                //                      into grp
                //                  select new
                //                  {
                //                      NarrowSearchText = grp.Key.NarrowSearchText,
                //                      TotalCount = grp.Key.TotalCount ?? 0
                //                  }
                //          ).AsParallel().ToDictionary(e => e.NarrowSearchText, e => e.TotalCount);
                //}
                //return ColUDFData;

                #endregion
                case "ToolTechnicianUDF":
                case "KitToolTechnicianUDF":
                    #region UDF
                    Type = "1";
                    if (TableName == "KitToolTechnicianUDF")
                    {
                        Type = "2";
                    }
                    objQLItems = null;
                    if (ToolType != null && (!string.IsNullOrWhiteSpace(ToolType)))
                    {
                        Type = Convert.ToString(ToolType);
                    }
                    if (ToolKitDetail != null && ToolKitDetail.Count > 0)
                    {
                        objQLItems = ToolKitDetail;
                    }
                    ToolIDs = "";
                    if (objQLItems != null && objQLItems.Count > 0)
                    {
                        foreach (var item in objQLItems)
                        {
                            if (!string.IsNullOrEmpty(ToolIDs))
                                ToolIDs += ",";

                            ToolIDs += item.ToolItemGUID.ToString();
                        }
                    }
                    objToolDAL = new ToolMasterDAL(base.DataBaseName);
                    return objToolDAL.GetToolListNarrowSearchTechnicianUDF(UDFName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, Type, ToolCurrentTab, LoadDataCount).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                #endregion
                case "EnterpriseMaster":
                    break;
                case "CompanyMaster":
                    CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(base.DataBaseName);
                    List<CompanyMasterDTO> lstCompanies = new List<CompanyMasterDTO>();
                    if (AllCompanies != null && AllCompanies.Count > 0)
                    {
                        lstCompanies = AllCompanies;
                    }
                    else
                    {
                        lstCompanies = objCompanyDAL.GetAllCompaniesFromETurnsMaster(IsArchived, IsDeleted, CompanyList, RoleID).ToList();
                    }
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in lstCompanies.Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1))).OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in lstCompanies.Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2))).OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);

                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in lstCompanies.Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))).OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);


                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in lstCompanies.Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4))).OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);

                        return ColUDFData;
                    }
                    else
                    {
                        ColUDFData = (from tmp in lstCompanies.Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5))).OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);

                        return ColUDFData;
                    }
                #endregion
                //  break;
                case "RoleMaster":
                    RoleMasterDAL objRoleDAL = new RoleMasterDAL(base.DataBaseName);
                    break;
                case "Room":
                    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                    List<RoomDTO> lstRooms = new List<RoomDTO>();
                    if (AllRooms != null && (!IsDeleted))
                    {
                        lstRooms = AllRooms;
                    }
                    else
                    {
                        lstRooms = objRoomDAL.GetAllRoomsFromETurnsMaster(CompanyID, IsDeleted, IsArchived, RoomList, string.Empty, RoleID, Session_EnterPriceID).ToList();
                    }
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in lstRooms.Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1))).OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in lstRooms.Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2))).OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);

                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in lstRooms.Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))).OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);


                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in lstRooms.Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4))).OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);

                        return ColUDFData;
                    }
                    else
                    {
                        ColUDFData = (from tmp in lstRooms.Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5))).OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);

                        return ColUDFData;
                    }
                #endregion

                case "QuickListMaster":
                    QuickListDAL objDAL = new QuickListDAL(base.DataBaseName);
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in objDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1))).OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);

                        //var tempData = objDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF1 ASC").Select(t => t.UDF1).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF1 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        //return objBinDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF1 ASC").Select(t => t.UDF1).Distinct();
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in objDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2))).OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                        //var tempData = objDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF2 ASC").Select(t => t.UDF2).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF2 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in objDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))).OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);

                        //var tempData = objDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF3 ASC").Select(t => t.UDF3).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF3 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in objDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4))).OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                        //var tempData = objDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF4 ASC").Select(t => t.UDF4).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF4 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                    else
                    {
                        ColUDFData = (from tmp in objDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5))).OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                        //var tempData = objDAL.GetAllRecords(RoomID, CompanyID).OrderBy("UDF5 ASC").Select(t => t.UDF5).Distinct();
                        //foreach (var item in tempData)
                        //{
                        //    if (item != null && item != "")
                        //    {
                        //        int tempCount = objDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UDF5 == item.ToString()).Count();
                        //        ColUDFData.Add(item.ToString(), tempCount);
                        //    }
                        //}
                        return ColUDFData;
                    }
                #endregion
                case "ItemMaster":
                    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                    IEnumerable<ItemMasterDTO> lstAllItems;
                    if (ItemModelCallFrom.ToLower().Trim() == "newpull")
                    {
                        PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                        RoomDAL roomDAL = new RoomDAL(base.DataBaseName);
                        RoomDTO objRoomDTO = roomDAL.GetRoomByIDPlain(RoomID);
                        if (objRoomDTO != null && objRoomDTO.AllowPullBeyondAvailableQty == true)
                        {
                            return objPullMasterDAL.GetNegativePullNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, RequestFor).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                        }
                        else
                        {
                            return objPullMasterDAL.GetNewPullNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, RequestFor).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                        }
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "credit")
                    {
                        PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                        return objPullMasterDAL.GetNewCreditNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, "Pull", LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "creditms")
                    {
                        PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                        return objPullMasterDAL.GetNewCreditNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, "Ms Pull", LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (ItemModelCallFrom.ToLower().Trim() == "ql")
                    {
                        int? QuicklistType = Session_QuicklistType;

                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "ql", QuicklistType).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


                        //if (HttpContext.Current.Session["ItemMasterList"] != null)
                        //{
                        //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["ItemMasterList"];
                        //}
                        //else
                        //{
                        //    lstAllItems = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).ToList();
                        //    if (HttpContext.Current.Session["QuicklistType"] != null && Convert.ToInt32(HttpContext.Current.Session["QuicklistType"]) == 3)
                        //    {
                        //        lstAllItems = lstAllItems.Where(x => x.SerialNumberTracking == false && x.LotNumberTracking == false && x.DateCodeTracking == false).ToList();
                        //    }
                        //    HttpContext.Current.Session["ItemMasterList"] = lstAllItems;
                        //}
                    }
                    else if (ItemModelCallFrom.ToLower().Trim() == "rq")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "requisition").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (ItemModelCallFrom.ToLower().Trim() == "ord")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "order").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (ItemModelCallFrom.ToLower().Trim() == "retord")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "returnorder").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "receive")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "receive").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (ItemModelCallFrom.ToLower().Trim() == "trf")
                    {
                        return objItemDAL.GetItemsForTransferPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, ParentID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "icnt")
                    {
                        InventoryCountDAL objCountDAL = new InventoryCountDAL(base.DataBaseName);
                        InventoryCountDTO objInventoryCountDTO = objCountDAL.GetInventoryCountById(ParentID, RoomID, CompanyID);
                        if (objInventoryCountDTO != null)
                        {
                            return objItemDAL.GetItemCountPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, false, objInventoryCountDTO.GUID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                        }
                        else
                        {
                            lstAllItems = new List<ItemMasterDTO>();
                        }
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "as")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "as").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (ItemModelCallFrom.ToLower().Trim() == "movemtr")
                    {
                        return objItemDAL.GetItemMoveMTRPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, Convert.ToInt32(moveType)).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "newcart")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "newcart").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "materialstaging")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "materialstaging").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "ps")
                    {
                        return objItemDAL.GetProjectSpendItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, ParentID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else if (ItemModelCallFrom.ToLower().Trim() == "quote")
                    {
                        return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "quote").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    }
                    else
                    {
                        return objItemDAL.GetItemsListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                        //if (HttpContext.Current.Session["ItemMasterList"] != null)
                        //{
                        //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["ItemMasterList"];
                        //}
                        //else
                        //{
                        //    lstAllItems = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).ToList();
                        //    HttpContext.Current.Session["ItemMasterList"] = lstAllItems;
                        //}
                    }
                    #region UDF
                    if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "icnt")
                    {
                        if (UDFName == "UDF1")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF1 != null && (!string.IsNullOrEmpty(t.ItemUDF1)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("itemUDF1 ASC")
                                              group tmp by new { tmp.ItemUDF1 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF1,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF1, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF1 != null && (!string.IsNullOrEmpty(t.ItemUDF1)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("itemUDF1 ASC")
                                              group tmp by new { tmp.ItemUDF1 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF1,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF1, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF2")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF2 != null && (!string.IsNullOrEmpty(t.ItemUDF2)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("itemUDF2 ASC")
                                              group tmp by new { tmp.ItemUDF2 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF2,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF2, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF2 != null && (!string.IsNullOrEmpty(t.ItemUDF2)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("itemUDF2 ASC")
                                              group tmp by new { tmp.ItemUDF2 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF2,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF2, e => e.Count);
                            }
                            return ColUDFData;
                        }
                        else if (UDFName == "UDF3")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF3 != null && (!string.IsNullOrEmpty(t.ItemUDF3)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("itemUDF3 ASC")
                                              group tmp by new { tmp.ItemUDF3 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF3,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF3, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF3 != null && (!string.IsNullOrEmpty(t.ItemUDF3)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("itemUDF3 ASC")
                                              group tmp by new { tmp.ItemUDF3 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF3,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF3, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF4")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF4 != null && (!string.IsNullOrEmpty(t.ItemUDF4)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("itemUDF4 ASC")
                                              group tmp by new { tmp.ItemUDF4 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF4,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF4, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF4 != null && (!string.IsNullOrEmpty(t.ItemUDF4)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("itemUDF4 ASC")
                                              group tmp by new { tmp.ItemUDF4 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF4,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF4, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF5 != null && (!string.IsNullOrEmpty(t.ItemUDF5))
                                              && (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("itemUDF5 ASC")
                                              group tmp by new { tmp.ItemUDF5 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF5,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF5, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemUDF5 != null && (!string.IsNullOrEmpty(t.ItemUDF5))
                                              && (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("itemUDF5 ASC")
                                              group tmp by new { tmp.ItemUDF5 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.ItemUDF5,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.ItemUDF5, e => e.Count);
                            }

                            return ColUDFData;
                        }
                    }
                    else
                    {
                        if (UDFName == "UDF1")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF1 ASC")
                                              group tmp by new { tmp.UDF1 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF1,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF1 ASC")
                                              group tmp by new { tmp.UDF1 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF1,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF2")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF2 ASC")
                                              group tmp by new { tmp.UDF2 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF2,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF2 ASC")
                                              group tmp by new { tmp.UDF2 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF2,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF3")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))
                                              && (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF3 ASC")
                                              group tmp by new { tmp.UDF3 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF3,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))
                                              && (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF3 ASC")
                                              group tmp by new { tmp.UDF3 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF3,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF4")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4))
                                              && (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF4 ASC")
                                              group tmp by new { tmp.UDF4 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF4,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF4 ASC")
                                              group tmp by new { tmp.UDF4 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF4,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF5")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF5 ASC")
                                              group tmp by new { tmp.UDF5 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF5,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF5 ASC")
                                              group tmp by new { tmp.UDF5 }
                                              into grp
                                              select new
                                              {
                                                  grp.Key.UDF5,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF6")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF6 != null && (!string.IsNullOrEmpty(t.UDF6)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF6 ASC")
                                              group tmp by new { tmp.UDF6 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF6,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF6, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF6 != null && (!string.IsNullOrEmpty(t.UDF6)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF6 ASC")
                                              group tmp by new { tmp.UDF6 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF6,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF6, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF7")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF7 != null && (!string.IsNullOrEmpty(t.UDF7)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF7 ASC")
                                              group tmp by new { tmp.UDF7 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF7,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF7, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF7 != null && (!string.IsNullOrEmpty(t.UDF7)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF7 ASC")
                                              group tmp by new { tmp.UDF7 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF7,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF7, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF8")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF8 != null && (!string.IsNullOrEmpty(t.UDF8)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF8 ASC")
                                              group tmp by new { tmp.UDF8 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF8,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF8, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF8 != null && (!string.IsNullOrEmpty(t.UDF8)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF8 ASC")
                                              group tmp by new { tmp.UDF8 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF8,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF8, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF9")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF9 != null && (!string.IsNullOrEmpty(t.UDF9)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF9 ASC")
                                              group tmp by new { tmp.UDF9 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF9,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF9, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF9 != null && (!string.IsNullOrEmpty(t.UDF9)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF9 ASC")
                                              group tmp by new { tmp.UDF9 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF9,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF9, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF10 != null && (!string.IsNullOrEmpty(t.UDF10)) &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF10 ASC")
                                              group tmp by new { tmp.UDF10 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF10,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF10, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllItems.Where(t => t.UDF10 != null && (!string.IsNullOrEmpty(t.UDF10)) &&
                                              (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF10 ASC")
                                              group tmp by new { tmp.UDF10 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF10,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF10, e => e.Count);
                            }

                            return ColUDFData;
                        }
                    }
                #endregion
                /////////////// Added By Hetal For UDF in Count Add Popup///////////////////////////////////
                case "ItemBinMaster":
                    ItemMasterDAL objItemBinDAL = new ItemMasterDAL(base.DataBaseName);
                    return objItemBinDAL.GetItemsBinListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                case "ItemCountList":

                    if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "icnt")
                    {
                        InventoryCountDAL objCountDAL = new InventoryCountDAL(base.DataBaseName);
                        InventoryCountDTO objInventoryCountDTO = objCountDAL.GetInventoryCountById(ParentID, RoomID, CompanyID);
                        if (objInventoryCountDTO != null)
                        {
                            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                            return objItemMasterDAL.GetItemCountPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, false, objInventoryCountDTO.GUID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                        }
                        else
                        {
                            lstAllItems = new List<ItemMasterDTO>();
                        }
                        return ColUDFData;
                    }
                    else
                    {
                        IEnumerable<ItemMasterDTO> lstAllCountItems;
                        if (ItemMasterListForCount != null)
                        {
                            lstAllCountItems = ItemMasterListForCount;
                        }
                        else
                        {
                            lstAllCountItems = new ItemMasterDAL(base.DataBaseName).GetAllItemsPlain(RoomID, CompanyID).ToList();
                        }
                        #region UDF
                        if (UDFName == "UDF1")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1)) &&
                                          (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF1 ASC")
                                              group tmp by new { tmp.UDF1 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF1,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1)) &&
                                          (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF1 ASC")
                                              group tmp by new { tmp.UDF1 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF1,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF2")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2)) &&
                                          (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF2 ASC")
                                              group tmp by new { tmp.UDF2 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF2,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2)) &&
                                          (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF2 ASC")
                                              group tmp by new { tmp.UDF2 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF2,
                                                  Count = grp.Count()
                                              }
                            ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF3")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3)) &&
                                          (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF3 ASC")
                                              group tmp by new { tmp.UDF3 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF3,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3)) &&
                                          (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF3 ASC")
                                              group tmp by new { tmp.UDF3 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF3,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else if (UDFName == "UDF4")
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4)) &&
                                          (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF4 ASC")
                                              group tmp by new { tmp.UDF4 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF4,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4)) &&
                                          (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF4 ASC")
                                              group tmp by new { tmp.UDF4 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF4,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        else
                        {
                            if (SupplierIds != null && SupplierIds.Any())
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5)) &&
                                          (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).OrderBy("UDF5 ASC")
                                              group tmp by new { tmp.UDF5 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF5,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                            }
                            else
                            {
                                ColUDFData = (from tmp in lstAllCountItems.Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5)) &&
                                          (t.SupplierID.GetValueOrDefault(0) > 0)).OrderBy("UDF5 ASC")
                                              group tmp by new { tmp.UDF5 }
                                                  into grp
                                              select new
                                              {
                                                  grp.Key.UDF5,
                                                  Count = grp.Count()
                                              }
                                ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                            }

                            return ColUDFData;
                        }
                        #endregion
                    }

                case "OrderMaster":
                    //OrderMasterDAL objOrderDAL = new OrderMasterDAL(base.DataBaseName);
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        //ColUDFData = (from tmp in objOrderDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF1 != null).OrderBy("UDF1 ASC")
                        //              group tmp by new { tmp.UDF1 }
                        //                  into grp
                        //                  select new
                        //                  {
                        //                      grp.Key.UDF1,
                        //                      Count = grp.Count()
                        //                  }
                        //    ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        //ColUDFData = (from tmp in objOrderDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF2 != null).OrderBy("UDF2 ASC")
                        //              group tmp by new { tmp.UDF2 }
                        //                  into grp
                        //                  select new
                        //                  {
                        //                      grp.Key.UDF2,
                        //                      Count = grp.Count()
                        //                  }
                        //).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        //ColUDFData = (from tmp in objOrderDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF3 != null).OrderBy("UDF3 ASC")
                        //              group tmp by new { tmp.UDF3 }
                        //                  into grp
                        //                  select new
                        //                  {
                        //                      grp.Key.UDF3,
                        //                      Count = grp.Count()
                        //                  }
                        //    ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        //ColUDFData = (from tmp in objOrderDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF4 != null).OrderBy("UDF4 ASC")
                        //              group tmp by new { tmp.UDF4 }
                        //                  into grp
                        //                  select new
                        //                  {
                        //                      grp.Key.UDF4,
                        //                      Count = grp.Count()
                        //                  }
                        //    ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                        return ColUDFData;
                    }
                    else
                    {
                        //ColUDFData = (from tmp in objOrderDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF5 != null).OrderBy("UDF5 ASC")
                        //              group tmp by new { tmp.UDF5 }
                        //                  into grp
                        //                  select new
                        //                  {
                        //                      grp.Key.UDF5,
                        //                      Count = grp.Count()
                        //                  }
                        //    ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);

                        return ColUDFData;
                    }
                #endregion
                case "ReceiveList":

                    return GetNarrowDDData_ReceiveMaster(UDFName, CompanyID, RoomID, IsArchived, IsDeleted, false, SupplierIds);

                //ItemMasterDAL objItemRecieveDAL = new ItemMasterDAL(base.DataBaseName);
                //if (HttpContext.Current.Session["ItemMasterList"] != null)
                //{
                //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["ItemMasterList"];
                //}
                //else
                //{
                //    lstAllItems = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).ToList();
                //}
                //ReceiveOrderDetailsDAL objReceiveDetailDAL = new ReceiveOrderDetailsDAL(base.DataBaseName);
                //int outCount = 0;
                //IEnumerable<ReceivableItemDTO> obj = objReceiveDetailDAL.GetALLReceiveListByPaging(0, int.MaxValue, out outCount, "", "OrderStatus", RoomID, CompanyID, false, false, RequisitionCurrentTab, SupplierIds);
                //IEnumerable<Guid> itmGuids = obj.Select(x => x.ItemGUID);
                //lstAllItems = lstAllItems.Where(x => itmGuids.Contains(x.GUID));

                #region UDF
                //if (UDFName == "UDF1")
                //{
                //    if (SupplierIds == null || !SupplierIds.Any())
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID
                //                      group ims by new { itm.UDF1 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF1))
                //                      select new
                //                      {
                //                          UDF1 = grp.Key.UDF1,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF1 + "[###]" + e.UDF1, e => e.Count);
                //    }
                //    else
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID
                //                      where (SupplierIds.Contains(ims.ItemSupplierID.GetValueOrDefault(0)))
                //                      group ims by new { itm.UDF1 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF1))
                //                      select new
                //                      {
                //                          UDF1 = grp.Key.UDF1,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF1 + "[###]" + e.UDF1, e => e.Count);
                //    }

                //    return ColUDFData;
                //}
                //else if (UDFName == "UDF2")
                //{
                //    if (SupplierIds == null || !SupplierIds.Any())
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID                                          
                //                      group ims by new { itm.UDF2 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF2))
                //                      select new
                //                      {
                //                          UDF2 = grp.Key.UDF2,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF2 + "[###]" + e.UDF2, e => e.Count);
                //    }
                //    else
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID
                //                      where (SupplierIds.Contains(ims.ItemSupplierID.GetValueOrDefault(0)))
                //                      group ims by new { itm.UDF2 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF2))
                //                      select new
                //                      {
                //                          UDF2 = grp.Key.UDF2,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF2 + "[###]" + e.UDF2, e => e.Count);
                //    }

                //    return ColUDFData;
                //}
                //else if (UDFName == "UDF3")
                //{
                //    if (SupplierIds == null || !SupplierIds.Any())
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID                                          
                //                      group ims by new { itm.UDF3 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF3))
                //                      select new
                //                      {
                //                          UDF3 = grp.Key.UDF3,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF3 + "[###]" + e.UDF3, e => e.Count);
                //    }
                //    else
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID
                //                      where (SupplierIds.Contains(ims.ItemSupplierID.GetValueOrDefault(0)))
                //                      group ims by new { itm.UDF3 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF3))
                //                      select new
                //                      {
                //                          UDF3 = grp.Key.UDF3,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF3 + "[###]" + e.UDF3, e => e.Count);
                //    }

                //    return ColUDFData;
                //}
                //else if (UDFName == "UDF4")
                //{
                //    if (SupplierIds == null || !SupplierIds.Any())
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID                                          
                //                      group ims by new { itm.UDF4 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF4))
                //                      select new
                //                      {
                //                          UDF4 = grp.Key.UDF4,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF4 + "[###]" + e.UDF4, e => e.Count);
                //    }
                //    else
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID
                //                      where (SupplierIds.Contains(ims.ItemSupplierID.GetValueOrDefault(0)))
                //                      group ims by new { itm.UDF4 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF4))
                //                      select new
                //                      {
                //                          UDF4 = grp.Key.UDF4,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF4 + "[###]" + e.UDF4, e => e.Count);
                //    }

                //    return ColUDFData;
                //}
                //else
                //{
                //    if (SupplierIds == null || !SupplierIds.Any())
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID
                //                      group ims by new { itm.UDF5 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF5))
                //                      select new
                //                      {
                //                          UDF5 = grp.Key.UDF5,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF5 + "[###]" + e.UDF5, e => e.Count);
                //    }
                //    else
                //    {
                //        ColUDFData = (from ims in obj
                //                      join itm in lstAllItems on ims.ItemGUID equals itm.GUID
                //                      where (SupplierIds.Contains(ims.ItemSupplierID.GetValueOrDefault(0)))
                //                      group ims by new { itm.UDF5 } into grp
                //                      where (!string.IsNullOrEmpty(grp.Key.UDF5))
                //                      select new
                //                      {
                //                          UDF5 = grp.Key.UDF5,
                //                          Count = grp.Count()
                //                      }).AsParallel().ToDictionary(e => e.UDF5 + "[###]" + e.UDF5, e => e.Count);
                //    }


                //    return ColUDFData;
                //}
                #endregion

                case "ReceiveToolAssetList":
                    
                    ReceiveToolAssetOrderDetailsDAL objReceiveToolAssetDetailDAL = new ReceiveToolAssetOrderDetailsDAL(base.DataBaseName);
                    IEnumerable<ToolMasterDTO> lstAllTools;
                    lstAllTools = objReceiveToolAssetDetailDAL.GetAllToolRecords(RoomID, CompanyID, false, false).ToList();
                    //lstAllTools = objReceiveToolAssetDetailDAL.GetAllToolRecords(RoomID, CompanyID, false, false).ToList();
                    int outReceiveTool = 0;
                    IEnumerable<ReceivableToolDTO> objReceiveToolList = objReceiveToolAssetDetailDAL.GetALLReceiveListByPaging(0, int.MaxValue, out outReceiveTool, "", "OrderStatus", RoomID, CompanyID, false, false, RequisitionCurrentTab, RoomDateFormat, CurrentTimeZone);
                    IEnumerable<Guid> toolGuids = objReceiveToolList.Select(x => x.ToolGUID);
                    lstAllTools = lstAllTools.Where(x => toolGuids.Contains(x.GUID));

                    #region UDF
                    if (UDFName == "UDF1")
                    {

                        ColUDFData = (from ims in objReceiveToolList
                                      join itm in lstAllTools on ims.ToolGUID equals itm.GUID
                                      where 1 == 1
                                      group ims by new { itm.UDF1 } into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF1))
                                      select new
                                      {
                                          UDF1 = grp.Key.UDF1,
                                          Count = grp.Count()
                                      }).AsParallel().ToDictionary(e => e.UDF1 + "[###]" + e.UDF1, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from ims in objReceiveToolList
                                      join itm in lstAllTools on ims.ToolGUID equals itm.GUID
                                      where 1 == 1
                                      group ims by new { itm.UDF2 } into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF2))
                                      select new
                                      {
                                          UDF2 = grp.Key.UDF2,
                                          Count = grp.Count()
                                      }).AsParallel().ToDictionary(e => e.UDF2 + "[###]" + e.UDF2, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from ims in objReceiveToolList
                                      join itm in lstAllTools on ims.ToolGUID equals itm.GUID
                                      where 1 == 1
                                      group ims by new { itm.UDF3 } into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF3))
                                      select new
                                      {
                                          UDF3 = grp.Key.UDF3,
                                          Count = grp.Count()
                                      }).AsParallel().ToDictionary(e => e.UDF3 + "[###]" + e.UDF3, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from ims in objReceiveToolList
                                      join itm in lstAllTools on ims.ToolGUID equals itm.GUID
                                      where 1 == 1
                                      group ims by new { itm.UDF4 } into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF4))
                                      select new
                                      {
                                          UDF4 = grp.Key.UDF4,
                                          Count = grp.Count()
                                      }).AsParallel().ToDictionary(e => e.UDF4 + "[###]" + e.UDF4, e => e.Count);
                        return ColUDFData;
                    }
                    else
                    {
                        ColUDFData = (from ims in objReceiveToolList
                                      join itm in lstAllTools on ims.ToolGUID equals itm.GUID
                                      where 1 == 1
                                      group ims by new { itm.UDF5 } into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF5))
                                      select new
                                      {
                                          UDF5 = grp.Key.UDF5,
                                          Count = grp.Count()
                                      }).AsParallel().ToDictionary(e => e.UDF5 + "[###]" + e.UDF5, e => e.Count);

                        return ColUDFData;
                    }
                #endregion

                case "PullMaster":
                    bool ConsignedAllowed = true;
                    
                    ConsignedAllowed =Convert.ToBoolean(Session_ConsignedAllowed);
                    
                    return GetNarrowDDData_PullMaster(UDFName, CompanyID, RoomID, IsArchived, IsDeleted, SupplierIds, ConsignedAllowed,LoadDataCount,UserID);
                case "UnitMaster":
                    UnitMasterDAL unitMasterDAL = new UnitMasterDAL(base.DataBaseName);
                    return unitMasterDAL.GetUnitListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, false).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "KitMaster":
                    KitMasterDAL kitMasterDAL = new KitMasterDAL(base.DataBaseName);
                    return kitMasterDAL.GetKitListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "VenderMaster":
                    VenderMasterDAL venderMasterDAL = new VenderMasterDAL(base.DataBaseName);
                    return venderMasterDAL.GetVendorListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "RequisitionMaster":
                    RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(base.DataBaseName);
                    return objRequisitionMasterDAL.GetRequistionMasterNarrowSearchRecords(UDFName, RequisitionCurrentTab, RoomID, CompanyID, SupplierIds, IsArchived, IsDeleted).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "CartItem":
                case "CartItemList":
                //bool CartConsignedAllowed = true;
                //if (HttpContext.Current.Session["ConsignedAllowed"] != null)
                //{
                //    CartConsignedAllowed = Convert.ToBoolean(HttpContext.Current.Session["ConsignedAllowed"]);
                //}
                //return GetNarrowDDData_CartItem(UDFName, CompanyID, RoomID, IsArchived, IsDeleted, SupplierIds, CartConsignedAllowed, LoadDataCount);

                case "MaterialStaging":
                    MaterialStagingDAL objMaterialStagingDAL = new MaterialStagingDAL(base.DataBaseName);
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in objMaterialStagingDAL.GetMaterialStagingUDF(RoomID, CompanyID, IsArchived, IsDeleted, "UDF1").OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF1))
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in objMaterialStagingDAL.GetMaterialStagingUDF(RoomID, CompanyID, IsArchived, IsDeleted, "UDF2").OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF2))
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in objMaterialStagingDAL.GetMaterialStagingUDF(RoomID, CompanyID, IsArchived, IsDeleted, "UDF3").OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF3))
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in objMaterialStagingDAL.GetMaterialStagingUDF(RoomID, CompanyID, IsArchived, IsDeleted, "UDF4").OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF4))
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF5")
                    {
                        ColUDFData = (from tmp in objMaterialStagingDAL.GetMaterialStagingUDF(RoomID, CompanyID, IsArchived, IsDeleted, "UDF5").OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF5))
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);

                        return ColUDFData;
                    }
                    else
                    {
                        return ColUDFData;
                    }
                #endregion
                case "AssetMaster":
                    AssetMasterDAL objAsetDAL = new AssetMasterDAL(base.DataBaseName);
                    return objAsetDAL.GetAssetListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "WorkOrder":
                    WorkOrderDAL objWorkOrderDAL = new WorkOrderDAL(base.DataBaseName);
                    return objWorkOrderDAL.GetWorkOrderListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, SupplierIds, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);
                case "toolschedulemapping":
                    AssetMasterDAL objGetSchedulerMappingMasterDAL = new AssetMasterDAL(base.DataBaseName);
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in objGetSchedulerMappingMasterDAL.GetSchedulerMappingRecord(CompanyID, RoomID, IsArchived, IsDeleted).Where(t => t.UDF1 != null).OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF1))
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in objGetSchedulerMappingMasterDAL.GetSchedulerMappingRecord(CompanyID, RoomID, IsArchived, IsDeleted).Where(t => t.UDF2 != null).OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF2))
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in objGetSchedulerMappingMasterDAL.GetSchedulerMappingRecord(CompanyID, RoomID, IsArchived, IsDeleted).Where(t => t.UDF3 != null).OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF3))
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in objGetSchedulerMappingMasterDAL.GetSchedulerMappingRecord(CompanyID, RoomID, IsArchived, IsDeleted).Where(t => t.UDF4 != null).OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF4))
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF5")
                    {
                        ColUDFData = (from tmp in objGetSchedulerMappingMasterDAL.GetSchedulerMappingRecord(CompanyID, RoomID, IsArchived, IsDeleted).Where(t => t.UDF5 != null).OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF5))
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);

                        return ColUDFData;
                    }
                    else
                    {
                        return ColUDFData;
                    }
                #endregion
                case "InventoryCountList":
                    InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(base.DataBaseName);
                    #region UDF
                    if (UDFName == "UDF1")
                    {
                        ColUDFData = (from tmp in objInventoryCountDAL.GetAllCounts(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF1 != null).OrderBy("UDF1 ASC")
                                      group tmp by new { tmp.UDF1 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF1))
                                      select new
                                      {
                                          grp.Key.UDF1,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF2")
                    {
                        ColUDFData = (from tmp in objInventoryCountDAL.GetAllCounts(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF2 != null).OrderBy("UDF2 ASC")
                                      group tmp by new { tmp.UDF2 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF2))
                                      select new
                                      {
                                          grp.Key.UDF2,
                                          Count = grp.Count()
                                      }
                        ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF3")
                    {
                        ColUDFData = (from tmp in objInventoryCountDAL.GetAllCounts(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF3 != null).OrderBy("UDF3 ASC")
                                      group tmp by new { tmp.UDF3 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF3))
                                      select new
                                      {
                                          grp.Key.UDF3,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF4")
                    {
                        ColUDFData = (from tmp in objInventoryCountDAL.GetAllCounts(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF4 != null).OrderBy("UDF4 ASC")
                                      group tmp by new { tmp.UDF4 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF4))
                                      select new
                                      {
                                          grp.Key.UDF4,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                        return ColUDFData;
                    }
                    else if (UDFName == "UDF5")
                    {
                        ColUDFData = (from tmp in objInventoryCountDAL.GetAllCounts(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UDF5 != null).OrderBy("UDF5 ASC")
                                      group tmp by new { tmp.UDF5 }
                                          into grp
                                      where (!string.IsNullOrEmpty(grp.Key.UDF5))
                                      select new
                                      {
                                          grp.Key.UDF5,
                                          Count = grp.Count()
                                      }
                            ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);

                        return ColUDFData;
                    }
                    else
                    {
                        return ColUDFData;
                    }
                #endregion
                case "AssetCategoryMaster":

                    AssetCategoryMasterDAL objAssetCateDAL = new AssetCategoryMasterDAL(base.DataBaseName);
                    return objAssetCateDAL.GetAssetCategoryListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "BOMItemMaster":
                    BOMItemMasterDAL objBOMItemMDAL = new BOMItemMasterDAL(base.DataBaseName);
                    #region GET DATA
                    IEnumerable<RequisitionMasterNarrowSearchDTO> objBOMItemMasterDTO = objBOMItemMDAL.GetAllNarrowSearchRecordsForUDF(UDFName, CompanyID, IsArchived, IsDeleted);
                    if (objBOMItemMasterDTO != null && objBOMItemMasterDTO.Count() > 0)
                    {
                        ColUDFData = (from tmp in objBOMItemMasterDTO
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

                case "InventoryClassificationMaster":

                    InventoryClassificationMasterDAL objinvDAL = new InventoryClassificationMasterDAL(base.DataBaseName);
                    return objinvDAL.GetInventoryClassificationListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName, false).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "CostUOMMaster":
                    CostUOMMasterDAL objCostDAL = new CostUOMMasterDAL(base.DataBaseName);
                    return objCostDAL.GetCostUOMListListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, UDFName).ToDictionary(e => e.NSColumnText, e => e.NSCount);

                case "MinMaxTunnigGrid":
                    IEnumerable<MinMaxDataTableInfo> lstAllMinMaxItems = null;
                    if (SessionMinMaxTableForNarrow != null && SessionMinMaxTableForNarrow.ToList().Count > 0)
                    {
                        lstAllMinMaxItems = SessionMinMaxTableForNarrow;
                        #region UDF
                        if (UDFName == "UDF1")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF1 != null && (!string.IsNullOrEmpty(t.UDF1))).OrderBy("UDF1 ASC")
                                          group tmp by new { tmp.UDF1 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF1,
                                              Count = grp.Count()
                                          }
                                ).AsParallel().ToDictionary(e => e.UDF1, e => e.Count);
                            return ColUDFData;
                        }
                        else if (UDFName == "UDF2")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF2 != null && (!string.IsNullOrEmpty(t.UDF2))).OrderBy("UDF2 ASC")
                                          group tmp by new { tmp.UDF2 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF2,
                                              Count = grp.Count()
                                          }
                                 ).AsParallel().ToDictionary(e => e.UDF2, e => e.Count);
                            return ColUDFData;
                        }
                        else if (UDFName == "UDF3")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF3 != null && (!string.IsNullOrEmpty(t.UDF3))).OrderBy("UDF3 ASC")
                                          group tmp by new { tmp.UDF3 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF3,
                                              Count = grp.Count()
                                          }
                                ).AsParallel().ToDictionary(e => e.UDF3, e => e.Count);
                            return ColUDFData;
                        }
                        else if (UDFName == "UDF4")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF4 != null && (!string.IsNullOrEmpty(t.UDF4))).OrderBy("UDF4 ASC")
                                          group tmp by new { tmp.UDF4 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF4,
                                              Count = grp.Count()
                                          }
                               ).AsParallel().ToDictionary(e => e.UDF4, e => e.Count);
                            return ColUDFData;
                        }
                        else if (UDFName == "UDF5")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF5 != null && (!string.IsNullOrEmpty(t.UDF5))).OrderBy("UDF5 ASC")
                                          group tmp by new { tmp.UDF5 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF5,
                                              Count = grp.Count()
                                          }
                               ).AsParallel().ToDictionary(e => e.UDF5, e => e.Count);
                            return ColUDFData;
                        }

                        else if (UDFName == "UDF6")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF6 != null && (!string.IsNullOrEmpty(t.UDF6))).OrderBy("UDF6 ASC")
                                          group tmp by new { tmp.UDF6 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF6,
                                              Count = grp.Count()
                                          }
                               ).AsParallel().ToDictionary(e => e.UDF6, e => e.Count);
                            return ColUDFData;
                        }

                        else if (UDFName == "UDF7")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF7 != null && (!string.IsNullOrEmpty(t.UDF7))).OrderBy("UDF7 ASC")
                                          group tmp by new { tmp.UDF7 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF7,
                                              Count = grp.Count()
                                          }
                               ).AsParallel().ToDictionary(e => e.UDF7, e => e.Count);
                            return ColUDFData;
                        }

                        else if (UDFName == "UDF8")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF8 != null && (!string.IsNullOrEmpty(t.UDF8))).OrderBy("UDF8 ASC")
                                          group tmp by new { tmp.UDF8 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF8,
                                              Count = grp.Count()
                                          }
                               ).AsParallel().ToDictionary(e => e.UDF8, e => e.Count);
                            return ColUDFData;
                        }

                        else if (UDFName == "UDF9")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF9 != null && (!string.IsNullOrEmpty(t.UDF9))).OrderBy("UDF9 ASC")
                                          group tmp by new { tmp.UDF9 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF9,
                                              Count = grp.Count()
                                          }
                               ).AsParallel().ToDictionary(e => e.UDF9, e => e.Count);
                            return ColUDFData;
                        }

                        else if (UDFName == "UDF10")
                        {
                            ColUDFData = (from tmp in lstAllMinMaxItems.Where(t => t.UDF10 != null && (!string.IsNullOrEmpty(t.UDF10))).OrderBy("UDF10 ASC")
                                          group tmp by new { tmp.UDF10 }
                                              into grp
                                          select new
                                          {
                                              grp.Key.UDF10,
                                              Count = grp.Count()
                                          }
                               ).AsParallel().ToDictionary(e => e.UDF10, e => e.Count);
                            return ColUDFData;
                        }
                        else
                        {
                            return ColUDFData;
                        }

                        #endregion
                    }
                    else
                    {
                        return ColUDFData;
                    }
                case "QuoteMaster":
                    QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(base.DataBaseName);
                    return quoteMasterDAL.GetQuoteMasterNarrowSearchData(UDFName, RequisitionCurrentTab, RoomID, CompanyID, IsArchived, IsDeleted).ToDictionary(e => e.NSColumnText, e => e.NSCount);
            }
            return null;
        }


        public bool CheckUDFIsRequired(string TableName, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, out string Reason, long CompanyID, long RoomID,long EnterpriseID,long UserID, string prefix = "")
        {
            bool isRequired = false;
            Reason = string.Empty;

            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(base.DataBaseName);
            IEnumerable<UDFDTO> DataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain(TableName, RoomID, CompanyID);
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            
            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            var ResMessageFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", currentCulture, EnterpriseID, CompanyID);
            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                foreach (var i in DataFromDB)
                {
                    string MsgRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResMessage", currentCulture);
                    if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(UDF1))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(UDF2))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(UDF3))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(UDF4))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(UDF5))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                }

                if (!string.IsNullOrEmpty(Reason))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }

        public bool CheckUDFIsRequiredLight(List<UDFDTO> DataFromDB, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, out string Reason, long CompanyID, long RoomID,long EnterpriseID,long UserID, string prefix = "")
        {
            bool isRequired = false;
            Reason = string.Empty;

            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            var ResMessageFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMessage", currentCulture, EnterpriseID, CompanyID);
            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                string MsgRequired = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRequired", ResMessageFilePath, EnterpriseID, CompanyID, RoomID, "ResMessage", currentCulture);
                foreach (var i in DataFromDB)
                {
                    if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(UDF1))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(UDF2))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(UDF3))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(UDF4))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                    if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(UDF5))
                    {
                        Reason = string.Format(MsgRequired, Reason + " " + prefix + "" + i.UDFColumnName);
                    }
                }

                if (!string.IsNullOrEmpty(Reason))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }

    }
}
