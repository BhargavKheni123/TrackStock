using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public partial class CommonDAL : eTurnsBaseDAL
    {
        private Dictionary<string, int> GetNarrowDDData_BOMList(string TextFieldName, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, long RoomID, long CompanyID, List<BOMItemDTO> lstBomIterms)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(base.DataBaseName);
            #region GET DATA
            //string RoomIds = string.Empty;
            //List<ItemMasterDTO> lstBOMItems = new List<ItemMasterDTO>();
            //if (RoomsAccess == null)
            //{
            //    RoomsAccess = new long[] { };
            //}
            //else
            //{
            //    RoomIds = string.Join(",", RoomsAccess);
            //}
            //if (!string.IsNullOrWhiteSpace(RoomIds))
            //{
            //    lstBOMItems = new BOMItemMasterDAL(base.DataBaseName).GetAllBOMItems(CompanyID);
            //}
            List<BOMItemDTO> lstBOMItems = new List<BOMItemDTO>();
            if (lstBomIterms != null && lstBomIterms.Count > 0)
            {
                lstBOMItems = lstBomIterms;
            }
            else
            {
                int TotalRecordCountRM = 0;
                lstBOMItems = objBOMItemMasterDAL.GetPagedRecords(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, CompanyID, false, false, RoomID, "popup", RoomDateFormat, CurrentTimeZone);

            }

            if (TextFieldName == "SupplierName")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var lstsupps = (from im in lstBOMItems
                                    where im.SupplierID > 0
                                    orderby im.SupplierName
                                    group im by new { im.SupplierName, im.SupplierID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.SupplierID,
                                        supname = grpms.Key.SupplierName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "ManufacturerName")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from im in lstBOMItems
                                    where im.ManufacturerID > 0
                                    orderby im.ManufacturerName
                                    group im by new { im.ManufacturerName, im.ManufacturerID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.ManufacturerID,
                                        supname = grpms.Key.ManufacturerName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "ItemType")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from im in lstBOMItems
                                    where im.ItemType > 0
                                    orderby im.ItemTypeName
                                    group im by new { im.ItemTypeName, im.ItemType } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.ItemType,
                                        supname = grpms.Key.ItemTypeName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "CategoryName")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from im in lstBOMItems
                                    where im.CategoryID > 0
                                    orderby im.CategoryName
                                    group im by new { im.CategoryName, im.CategoryID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.CategoryID,
                                        supname = grpms.Key.CategoryName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "CreatedBy")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from im in lstBOMItems
                                    where im.CreatedBy > 0
                                    orderby im.CreatedByName
                                    group im by new { im.CreatedByName, im.CreatedBy } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.CreatedBy,
                                        supname = grpms.Key.CreatedByName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "UpdatedBy")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from im in lstBOMItems
                                    where im.LastUpdatedBy > 0
                                    orderby im.UpdatedByName
                                    group im by new { im.UpdatedByName, im.LastUpdatedBy } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.LastUpdatedBy,
                                        supname = grpms.Key.UpdatedByName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            //else if (TextFieldName == "RoomName")
            //{
            //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //    {

            //        var lstsupps = (from im in lstBOMItems
            //                        where im.RoomId > 0
            //                        orderby im.RoomName
            //                        group im by new { im.RoomName, im.RoomId } into grpms
            //                        select new
            //                        {
            //                            count = grpms.Count(),
            //                            sid = grpms.Key.RoomId,
            //                            supname = grpms.Key.RoomName
            //                        });
            //        return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
            //    }
            //}
            return ColUDFData;
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_SupplierCatalog(string TextFieldName, long RoomID, long CompanyID, string ItemModelCallFrom)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            CatalogItemMasterDAL objCatlogItemMasterDAL = new CatalogItemMasterDAL(base.DataBaseName);
            #region GET DATA
            List<SupplierCatalogItemDTO> lstCatLogItem = new List<SupplierCatalogItemDTO>();
            int TotalRecordCountRM = 0;
            lstCatLogItem = objCatlogItemMasterDAL.GetPagedRecordsByDB(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, RoomID, CompanyID, false, false, ItemModelCallFrom,0);

            if (TextFieldName == "SupplierName")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var lstsupps = (from im in lstCatLogItem
                                    orderby im.SupplierName
                                    group im by new { im.SupplierName } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.SupplierName,
                                        supname = grpms.Key.SupplierName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "ManufacturerName")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var lstsupps = (from im in lstCatLogItem
                                    orderby im.ManufacturerName
                                    group im by new { im.ManufacturerName } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.ManufacturerName,
                                        supname = grpms.Key.ManufacturerName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            return ColUDFData;
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_TechnicianMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            TechnicialMasterDAL technicialMasterDAL = new TechnicialMasterDAL(base.DataBaseName);
            return technicialMasterDAL.GetTechnicianListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        private Dictionary<string, int> GetNarrowDDData_BinMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, List<BinMasterDTO> AllBins)
        {
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
            #region GET DATA
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (TextFieldName == "CreatedBy")
                {
                    var lstsupps = (from ci in lstBins
                                    where ci.CreatedBy != null
                                    orderby ci.CreatedByName
                                    group ci by new { ci.CreatedByName, ci.CreatedBy } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.CreatedBy,
                                        supname = grpms.Key.CreatedByName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);

                }
                else
                {
                    var lstsupps = (from ci in lstBins
                                    where ci.LastUpdatedBy != null
                                    orderby ci.CreatedByName
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
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_CategoryMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            CategoryMasterDAL objCateDAL = new CategoryMasterDAL(base.DataBaseName);
            return objCateDAL.GetCategoryListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        private Dictionary<string, int> GetNarrowDDData_BomCategoryMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            CategoryMasterDAL objCateDAL = new CategoryMasterDAL(base.DataBaseName);
            return objCateDAL.GetBOMCategoryListNarrowSearch(CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }


        private Dictionary<string, int> GetNarrowDDData_CustomerMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            CustomerMasterDAL objCustomerMasterDAL = new CustomerMasterDAL(base.DataBaseName);
            return objCustomerMasterDAL.GetCustomerListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        //private Dictionary<string, int> GetNarrowDDData_FreightTypeMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        //{
        //    FreightTypeMasterDAL objFreiDAL = new FreightTypeMasterDAL(base.DataBaseName);
        //    return objFreiDAL.GetFreightTypeListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText, e => e.NSCount);

        //}

        private Dictionary<string, int> GetNarrowDDData_GLAccountMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            GLAccountMasterDAL objGLAccDAL = new GLAccountMasterDAL(base.DataBaseName);
            return objGLAccDAL.GetGLAccountListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, false).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        private Dictionary<string, int> GetNarrowDDData_BomGLAccountMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            GLAccountMasterDAL objGLAccDAL = new GLAccountMasterDAL(base.DataBaseName);
            return objGLAccDAL.GetGLAccountListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, true).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        private Dictionary<string, int> GetNarrowDDData_GXPRConsigmentJobMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            GXPRConsignedJobMasterDAL objGXPRConsiDAL = new GXPRConsignedJobMasterDAL(base.DataBaseName);
            #region GET DATA
            if (TextFieldName == "CreatedBy")
            {

                ColUDFData = (from tmp in objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.CreatedByName != null)
                              orderby tmp.CreatedByName
                              group tmp by new { tmp.CreatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.CreatedByName,
                                  Count = grp.Count()
                              }
                  ).AsParallel().ToDictionary(e => e.CreatedByName, e => e.Count);


                //var tempData = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.CreatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.CreatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            else
            {
                ColUDFData = (from tmp in objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UpdatedByName != null)
                              orderby tmp.UpdatedByName
                              group tmp by new { tmp.UpdatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.UpdatedByName,
                                  Count = grp.Count()
                              }
                ).AsParallel().ToDictionary(e => e.UpdatedByName, e => e.Count);

                //var tempData = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.UpdatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objGXPRConsiDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UpdatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            #endregion

        }

        private Dictionary<string, int> GetNarrowDDData_JobTypeMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            JobTypeMasterDAL objJobTypeDAL = new JobTypeMasterDAL(base.DataBaseName);
            return objJobTypeDAL.GetJobTypeListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }


        private Dictionary<string, int> GetNarrowDDData_LocationMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            LocationMasterDAL locationMasterDAL = new LocationMasterDAL(base.DataBaseName);
            return locationMasterDAL.GetLocationListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        private Dictionary<string, int> GetNarrowDDData_ManufacturerMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            ManufacturerMasterDAL objMFDAL = new ManufacturerMasterDAL(base.DataBaseName);
            return objMFDAL.GetMFListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, false).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


        }

        private Dictionary<string, int> GetNarrowDDData_WrittenOffCategoryMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            WrittenOffCategoryDAL objDAL = new WrittenOffCategoryDAL(base.DataBaseName);
            return objDAL.GetWrittenOffCategoryListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, false).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


        }

        private Dictionary<string, int> GetNarrowDDData_BomManufacturerMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            ManufacturerMasterDAL objMFDAL = new ManufacturerMasterDAL(base.DataBaseName);
            return objMFDAL.GetMFListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, true).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


        }

        private Dictionary<string, int> GetNarrowDDData_MeasurementTerm(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            MeasurementTermDAL objMeasureDAL = new MeasurementTermDAL(base.DataBaseName);
            #region GET DATA
            if (TextFieldName == "CreatedBy")
            {
                ColUDFData = (from tmp in objMeasureDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.CreatedByName != null)
                              orderby tmp.CreatedByName
                              group tmp by new { tmp.CreatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.CreatedByName,
                                  Count = grp.Count()
                              }
              ).AsParallel().ToDictionary(e => e.CreatedByName, e => e.Count);

                //var tempData = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.CreatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.CreatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            else
            {
                ColUDFData = (from tmp in objMeasureDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UpdatedByName != null)
                              orderby tmp.UpdatedByName
                              group tmp by new { tmp.UpdatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.UpdatedByName,
                                  Count = grp.Count()
                              }
              ).AsParallel().ToDictionary(e => e.UpdatedByName, e => e.Count);


                //var tempData = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.UpdatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objMeasureDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UpdatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_ShipViaMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            ShipViaDAL objShipViaDAL = new ShipViaDAL(base.DataBaseName);
            return objShipViaDAL.GetShipViaListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

        }


        private Dictionary<string, int> GetNarrowDDData_SupplierMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(base.DataBaseName);
            return objSupDAL.GetSupplierListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, false).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

        }

        private Dictionary<string, int> GetNarrowDDData_BomSupplierMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(base.DataBaseName);
            return objSupDAL.GetSupplierListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, true).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


        }

        private Dictionary<string, int> GetNarrowDDData_ToolCategoryMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            ToolCategoryMasterDAL objCateDAL = new ToolCategoryMasterDAL(base.DataBaseName);
            return objCateDAL.GetToolCategoryListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

        }

        private Dictionary<string, int> GetNarrowDDData_AssetCategoryMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            AssetCategoryMasterDAL objAssetCateDAL = new AssetCategoryMasterDAL(base.DataBaseName);
            return objAssetCateDAL.GetAssetCategoryListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

        }

        private Dictionary<string, int> GetNarrowDDData_EnterpriseQuickListMaster(string TextFieldName, bool IsDeleted)
        {
            EnterpriseQuickListDAL enterpriseQLDAL = new EnterpriseQuickListDAL(base.DataBaseName);
            return enterpriseQLDAL.GetEnterpriseQLNarrowSearch(IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        private Dictionary<string, int> GetNarrowDDData_ToolAndKitToolMaster(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string CurrentTab, List<ToolDetailDTO> ToolKitDetail,string ToolType,int LoadDataCount)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);
            string Type = "1";
            if (TableName == "KitToolMaster")
            {
                Type = "2";
            }
            if (ToolType != null && (!string.IsNullOrWhiteSpace(ToolType)))
            {
                Type = Convert.ToString(ToolType);
            }
            #region GET DATA
            if (TextFieldName != "ToolMaintenance")
            {
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

                return objToolDAL.GetToolListNarrowSearch(TextFieldName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, Type, true, CurrentTab, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //if (objToolMasterDTO != null && objToolMasterDTO.Count() > 0)
                //{
                //    ColUDFData = (from tmp in objToolMasterDTO
                //                  group tmp by new { tmp.NarrowSearchText, tmp.TotalCount }
                //                              into grp
                //                  select new
                //                  {
                //                      NarrowSearchText = grp.Key.NarrowSearchText,
                //                      TotalCount = grp.Key.TotalCount ?? 0
                //                  }
                //          ).AsParallel().ToDictionary(e => e.NarrowSearchText, e => e.TotalCount);
                //}
                //return ColUDFData;
            }

            else
            {

                List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
                lstToolMasterDTO = objToolDAL.GetToolByRoomPlain(RoomID, CompanyID).ToList();
                if (lstToolMasterDTO != null && lstToolMasterDTO.Count() > 0)
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => Type.Contains(Convert.ToString(c.Type))).ToList();
                }
                if (lstToolMasterDTO != null && lstToolMasterDTO.Count() > 0)
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => Type.Contains(Convert.ToString(c.Type))).ToList();
                }
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
                if (ToolIDs != null && (!string.IsNullOrWhiteSpace(ToolIDs)) && ToolIDs != "")
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => (!ToolIDs.Contains(Convert.ToString(c.GUID)))).ToList();
                }
                Int32 OutOfStockCount = lstToolMasterDTO.Where(t => (t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Checkedout + "###" + "1", OutOfStockCount);
                Int32 BelowCriticalCount = lstToolMasterDTO.Where(t => (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.CheckedouttoMaintenance + "###" + "2", BelowCriticalCount);
                Int32 BelowMinimumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) == ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Groupfullycheckedout + "###" + "3", BelowMinimumCount);
                Int32 AboveMaximumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) < ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Nonecheckedout + "###" + "4", AboveMaximumCount);
                return ColUDFData;
            }

            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_ToolAndKitToolMasterNew(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, List<ToolDetailDTO> ToolKitDetail, string ToolType, int LoadDataCount, string CurrentTab)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            ToolMasterDAL objToolNewDAL = new ToolMasterDAL(base.DataBaseName);
            string TypeNew = "1";
            if (TableName == "KitToolMaster")
            {
                TypeNew = "2";
            }
            if (ToolType != null && (!string.IsNullOrWhiteSpace(ToolType)))
            {
                TypeNew = Convert.ToString(ToolType);
            }
            #region GET DATA
            if (TextFieldName != "ToolMaintenance")
            {
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

                return objToolNewDAL.GetToolListNarrowSearch(TextFieldName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, TypeNew, true, CurrentTab, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

            }

            else
            {

                List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
                lstToolMasterDTO = objToolNewDAL.GetToolByRoomPlain(RoomID, CompanyID).ToList();
                if (lstToolMasterDTO != null && lstToolMasterDTO.Count() > 0)
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => TypeNew.Contains(Convert.ToString(c.Type))).ToList();
                }
                if (lstToolMasterDTO != null && lstToolMasterDTO.Count() > 0)
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => TypeNew.Contains(Convert.ToString(c.Type))).ToList();
                }
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
                if (ToolIDs != null && (!string.IsNullOrWhiteSpace(ToolIDs)) && ToolIDs != "")
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => (!ToolIDs.Contains(Convert.ToString(c.GUID)))).ToList();
                }
                Int32 OutOfStockCount = lstToolMasterDTO.Where(t => (t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Checkedout + "###" + "1", OutOfStockCount);
                Int32 BelowCriticalCount = lstToolMasterDTO.Where(t => (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.CheckedouttoMaintenance + "###" + "2", BelowCriticalCount);
                Int32 BelowMinimumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) == ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Groupfullycheckedout + "###" + "3", BelowMinimumCount);
                Int32 AboveMaximumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) < ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Nonecheckedout + "###" + "4", AboveMaximumCount);
                return ColUDFData;
            }

            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_ToolAssetOrder(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, List<ToolAssetOrderDetailsDTO> ToolAssetOrderDetail, string ToolORDType)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(base.DataBaseName);
            string ToolType = "1,2";

            if (ToolORDType != null && (!string.IsNullOrWhiteSpace(ToolORDType)))
            {
                ToolType = ToolORDType;
            }
            #region GET DATA
            if (TextFieldName != "ToolMaintenance")
            {
                List<ToolAssetOrderDetailsDTO> objQLItems = null;
                if (ToolAssetOrderDetail != null && ToolAssetOrderDetail.Count > 0)
                {
                    objQLItems = ToolAssetOrderDetail;
                }
                string ToolIDs = "";
                if (objQLItems != null && objQLItems.Count > 0)
                {
                    foreach (var item in objQLItems)
                    {
                        if (!string.IsNullOrEmpty(ToolIDs))
                            ToolIDs += ",";

                        ToolIDs += item.ToolGUID.ToString();
                    }
                }

                return objToolMasterDAL.GetToolListNarrowSearch(TextFieldName, RoomID, CompanyID, IsArchived, IsDeleted, ToolIDs, ToolType, true, string.Empty).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


            }

            else
            {

                List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
                lstToolMasterDTO = objToolMasterDAL.GetToolByRoomPlain(RoomID, CompanyID).ToList();
                if (lstToolMasterDTO != null && lstToolMasterDTO.Count() > 0)
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => ToolType.Contains(Convert.ToString(c.Type))).ToList();
                }
                if (lstToolMasterDTO != null && lstToolMasterDTO.Count() > 0)
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => ToolType.Contains(Convert.ToString(c.Type))).ToList();
                }
                List<ToolAssetOrderDetailsDTO> objQLItems = null;
                if (ToolAssetOrderDetail != null && ToolAssetOrderDetail.Count > 0)
                {
                    objQLItems = ToolAssetOrderDetail;
                }
                string ToolIDs = "";
                if (objQLItems != null && objQLItems.Count > 0)
                {
                    foreach (var item in objQLItems)
                    {
                        if (!string.IsNullOrEmpty(ToolIDs))
                            ToolIDs += ",";

                        ToolIDs += item.ToolGUID.ToString();
                    }
                }
                if (ToolIDs != null && (!string.IsNullOrWhiteSpace(ToolIDs)) && ToolIDs != "")
                {
                    lstToolMasterDTO = lstToolMasterDTO.Where(c => (!ToolIDs.Contains(Convert.ToString(c.GUID)))).ToList();
                }
                Int32 OutOfStockCount = lstToolMasterDTO.Where(t => (t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Checkedout + "###" + "1", OutOfStockCount);
                Int32 BelowCriticalCount = lstToolMasterDTO.Where(t => (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.CheckedouttoMaintenance + "###" + "2", BelowCriticalCount);
                Int32 BelowMinimumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) == ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Groupfullycheckedout + "###" + "3", BelowMinimumCount);
                Int32 AboveMaximumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) < ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Nonecheckedout + "###" + "4", AboveMaximumCount);
                return ColUDFData;
            }

            #endregion
        }


        private Dictionary<string, int> GetNarrowDDData_ToolHistory(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, int LoadDataCount)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            ToolMasterDAL objToolHistoryDAL = new ToolMasterDAL(base.DataBaseName);

            #region GET DATA
            if (TextFieldName != "ToolMaintenance")
            {

                return objToolHistoryDAL.GetToolListNarrowSearch(TextFieldName, RoomID, CompanyID, IsArchived, IsDeleted, string.Empty, string.Empty, false, string.Empty, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


                //IEnumerable<RequisitionMasterNarrowSearchDTO> objToolMasterDTO = objToolHistoryDAL.GetToolListNarrowSearch(TextFieldName, RoomID, CompanyID, IsArchived, IsDeleted,string.Empty,string.Empty,false);
                //if (objToolMasterDTO != null && objToolMasterDTO.Count() > 0)
                //{
                //    ColUDFData = (from tmp in objToolMasterDTO
                //                  group tmp by new { tmp.NarrowSearchText, tmp.TotalCount }
                //                              into grp
                //                  select new
                //                  {
                //                      NarrowSearchText = grp.Key.NarrowSearchText,
                //                      TotalCount = grp.Key.TotalCount ?? 0
                //                  }
                //          ).AsParallel().ToDictionary(e => e.NarrowSearchText, e => e.TotalCount);
                //}
                //return ColUDFData;
            }

            else
            {
                List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
                lstToolMasterDTO = objToolHistoryDAL.GetToolByRoomPlain(RoomID, CompanyID).ToList();

                Int32 OutOfStockCount = lstToolMasterDTO.Where(t => (t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Checkedout + "###" + "1", OutOfStockCount);
                Int32 BelowCriticalCount = lstToolMasterDTO.Where(t => (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.CheckedouttoMaintenance + "###" + "2", BelowCriticalCount);
                Int32 BelowMinimumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) == ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Groupfullycheckedout + "###" + "3", BelowMinimumCount);
                Int32 AboveMaximumCount = lstToolMasterDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) < ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Nonecheckedout + "###" + "4", AboveMaximumCount);
                return ColUDFData;
            }

            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_ToolHistoryNew(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, int LoadDataCount)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            ToolMasterDAL objToolHistoryNewDAL = new ToolMasterDAL(base.DataBaseName);

            #region GET DATA
            if (TextFieldName != "ToolMaintenance")
            {

                IEnumerable<RequisitionMasterNarrowSearchDTO> objToolMasterNewDTO = objToolHistoryNewDAL.GetAllNarrowSearchRecordsForHistoryNew(TextFieldName, RoomID, CompanyID, IsArchived, IsDeleted);
                if (objToolMasterNewDTO != null && objToolMasterNewDTO.Count() > 0)
                {
                    ColUDFData = (from tmp in objToolMasterNewDTO
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
            }

            else
            {
                List<ToolMasterDTO> lstToolMasterNewDTO = new List<ToolMasterDTO>();
                lstToolMasterNewDTO = objToolHistoryNewDAL.GetToolByRoomPlain(RoomID, CompanyID).ToList();

                Int32 OutOfStockCount = lstToolMasterNewDTO.Where(t => (t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Checkedout + "###" + "1", OutOfStockCount);
                Int32 BelowCriticalCount = lstToolMasterNewDTO.Where(t => (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY) > 0).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.CheckedouttoMaintenance + "###" + "2", BelowCriticalCount);
                Int32 BelowMinimumCount = lstToolMasterNewDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) == ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Groupfullycheckedout + "###" + "3", BelowMinimumCount);
                Int32 AboveMaximumCount = lstToolMasterNewDTO.Where(t => ((t.CheckedOutQTY == null ? 0 : t.CheckedOutQTY) + (t.CheckedOutMQTY == null ? 0 : t.CheckedOutMQTY)) < ((t.Quantity))).Count();
                ColUDFData.Add(eTurns.DTO.ResToolMaster.Nonecheckedout + "###" + "4", AboveMaximumCount);
                return ColUDFData;
            }

            #endregion
        }


        private Dictionary<string, int> GetNarrowDDData_ToolScheduleMapping(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            #region
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {


                if (TextFieldName == "ScheduleFor")
                {
                    ColUDFData = (from ts in context.ToolsSchedulerMappings
                                  where
                                    ((System.Boolean?)ts.IsArchived ?? false) == IsArchived &&
                                    ((System.Boolean?)ts.IsDeleted ?? false) == IsDeleted &&
                                    (ts.Room ?? 0) == RoomID && (ts.CompanyID ?? 0) == CompanyID

                                  group new { ts } by new
                                  {
                                      ts.SchedulerFor
                                  } into g
                                  select new
                                  {
                                      id = g.Key.SchedulerFor,

                                      count = g.Count(p => p.ts.SchedulerFor != null)
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => (e.id == 1 ? "Asset" : "Tools") + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "ToolAssetName")
                {
                    ColUDFData = (from tsm in context.ToolsSchedulerMappings
                                  join tm in context.ToolMasters on tsm.ToolGUID equals tm.GUID into tsm_tm_join
                                  from tsm_tm in tsm_tm_join.DefaultIfEmpty()
                                  join am in context.AssetMasters on tsm.AssetGUID equals am.GUID into tsm_am_join
                                  from tsm_am in tsm_am_join.DefaultIfEmpty()
                                  where (tsm.IsDeleted ?? false) == IsDeleted && tsm.Room == RoomID && tsm.CompanyID == CompanyID
                                  group tsm by new { tsm.ToolGUID, tsm.AssetGUID, tsm_tm.ToolName, tsm_am.AssetName, toolSerial = tsm_tm.Serial } into gropuedtsm
                                  select new
                                  {
                                      name = gropuedtsm.Key.AssetName ?? (gropuedtsm.Key.ToolName + " (" + gropuedtsm.Key.toolSerial + ")"),
                                      id = gropuedtsm.Key.ToolGUID ?? gropuedtsm.Key.AssetGUID,
                                      type = gropuedtsm.Key.ToolGUID.HasValue ? "tool" : "asset",
                                      count = gropuedtsm.Count()
                                  }).AsParallel().Where(x => x.count > 0 && !string.IsNullOrWhiteSpace(x.name) && x.name.Trim() != "()" && x.id.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToDictionary(e => e.name + "[###]" + e.id.ToString() + "[||]" + e.type, e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "ScheduleType")
                {
                    //ColUDFData = (from ts in context.ToolsSchedulerMappings
                    //              where
                    //                ((System.Boolean?)ts.IsArchived ?? false) == false &&
                    //                ((System.Boolean?)ts.IsDeleted ?? false) == false &&
                    //                (ts.Room ?? 0) == RoomID && (ts.CompanyID ?? 0) == CompanyID

                    //              group new { ts } by new
                    //              {
                    //                  ts.SchedulerType
                    //              } into g
                    //              select new
                    //              {
                    //                  id = g.Key.SchedulerType,

                    //                  count = g.Count(p => p.ts.SchedulerType != null)
                    //              }).AsParallel().Where(x => x.count > 0).ToDictionary(e => (e.id == 1 ? "Scheduled" : "UnScheduled") + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }

                else if (TextFieldName == "SchedulerName")
                {
                    ColUDFData = (from tsm in context.ToolsSchedulerMappings
                                  join ts in context.ToolsSchedulers on tsm.ToolSchedulerGuid equals ts.GUID
                                  where
                                  ((System.Boolean?)tsm.IsArchived ?? false) == IsArchived &&
                                  ((System.Boolean?)tsm.IsDeleted ?? false) == IsDeleted &&
                                    (tsm.Room ?? 0) == RoomID && (tsm.CompanyID ?? 0) == CompanyID
                                  group new { ts, tsm } by new
                                  {
                                      ts.ID,
                                      ts.SchedulerName
                                  } into g
                                  select new
                                  {
                                      id = g.Key.ID,
                                      name = g.Key.SchedulerName,
                                      count = g.Count(p => p.tsm.ToolSchedulerGuid != null)
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => e.name + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "UpdatedBy")
                {
                    ColUDFData = (from ODD in context.ToolsSchedulerMappings
                                  join UM in context.UserMasters on ((System.Int64?)ODD.LastUpdatedBy ?? 0) equals UM.ID
                                  where
                                    ((System.Boolean?)ODD.IsArchived ?? false) == IsArchived &&
                                    ((System.Boolean?)ODD.IsDeleted ?? false) == IsDeleted &&
                                    ((System.Int64?)ODD.Room ?? 0) == RoomID && ((System.Int64?)ODD.CompanyID ?? 0) == CompanyID
                                  group new { ODD, UM } by new
                                  {
                                      UM.UserName,
                                      UM.ID
                                  } into g
                                  select new
                                  {
                                      id = g.Key.ID,
                                      name = g.Key.UserName,
                                      count = g.Count()
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => e.name + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "CreatedBy")
                {
                    ColUDFData = (from ODD in context.ToolsSchedulerMappings
                                  join UM in context.UserMasters on ((System.Int64?)ODD.CreatedBy ?? 0) equals UM.ID
                                  where
                                    ((System.Boolean?)ODD.IsArchived ?? false) == IsArchived &&
                                    ((System.Boolean?)ODD.IsDeleted ?? false) == IsDeleted &&
                                    ((System.Int64?)ODD.Room ?? 0) == RoomID && ((System.Int64?)ODD.CompanyID ?? 0) == CompanyID
                                  group new { ODD, UM } by new
                                  {
                                      UM.UserName,
                                      UM.ID
                                  } into g
                                  select new
                                  {
                                      id = g.Key.ID,
                                      name = g.Key.UserName,
                                      count = g.Count()
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => e.name + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;

                }
                else
                {

                    return ColUDFData;
                }


            }
            #endregion
        }


        private Dictionary<string, int> GetNarrowDDData_ToolsMaintenanceDetails(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            #region
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string tabName = string.Empty;
                string[] arrsplt = TextFieldName.Split('_');
                if (arrsplt.Count() > 1)
                {
                    TextFieldName = arrsplt[0];
                    tabName = arrsplt[1];
                }
                string[] mntsDue = new string[] { "Open", "Start" };
                string[] mntshist = new string[] { "Close" };
                if (TextFieldName == "ScheduleFor")
                {
                    ColUDFData = (from ts in context.ToolsMaintenances
                                  where (ts.IsArchived ?? false) == false && (ts.IsDeleted ?? false) == false && (ts.Room ?? 0) == RoomID && (ts.CompanyID ?? 0) == CompanyID && ((tabName == "history") ? mntshist.Contains(ts.Status) : true) && ((tabName == "due") ? mntsDue.Contains(ts.Status) : true)
                                  group new { ts } by new { ts.ScheduleFor } into g
                                  select new
                                  {
                                      id = g.Key.ScheduleFor,
                                      count = g.Count(p => p.ts.ScheduleFor != null)
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => (e.id == 1 ? "Asset" : "Tools") + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "ToolAssetName")
                {
                    ColUDFData = (from tsm in context.ToolsMaintenances
                                  join tm in context.ToolMasters on tsm.ToolGUID equals tm.GUID into tsm_tm_join
                                  from tsm_tm in tsm_tm_join.DefaultIfEmpty()
                                  join am in context.AssetMasters on tsm.AssetGUID equals am.GUID into tsm_am_join
                                  from tsm_am in tsm_am_join.DefaultIfEmpty()
                                  where (tsm.IsDeleted ?? false) == false && tsm.Room == RoomID && tsm.CompanyID == CompanyID && ((tabName == "history") ? mntshist.Contains(tsm.Status) : true) && ((tabName == "due") ? mntsDue.Contains(tsm.Status) : true)
                                  group tsm by new { tsm.ToolGUID, tsm.AssetGUID, tsm_tm.ToolName, tsm_am.AssetName } into gropuedtsm
                                  select new
                                  {
                                      name = gropuedtsm.Key.ToolName ?? gropuedtsm.Key.AssetName,
                                      id = gropuedtsm.Key.ToolGUID ?? gropuedtsm.Key.AssetGUID,
                                      type = gropuedtsm.Key.ToolGUID.HasValue ? "tool" : "asset",
                                      count = gropuedtsm.Count()
                                  }).AsParallel().Where(x => x.count > 0 && !string.IsNullOrWhiteSpace(x.name) && x.name.Trim() != "()" && x.id.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToDictionary(e => e.name + "[###]" + e.id.ToString() + "[||]" + e.type, e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "ScheduleType")
                {
                    ColUDFData = (from ts in context.ToolsMaintenances
                                  where (ts.IsArchived ?? false) == false && (ts.IsDeleted ?? false) == false && (ts.Room ?? 0) == RoomID && (ts.CompanyID ?? 0) == CompanyID && ((tabName == "history") ? mntshist.Contains(ts.Status) : true) && ((tabName == "due") ? mntsDue.Contains(ts.Status) : true)
                                  group new { ts } by new { ts.SchedulerType } into g
                                  select new
                                  {
                                      id = g.Key.SchedulerType,
                                      count = g.Count(p => p.ts.SchedulerType != null)
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => (e.id == 1 ? "Scheduled" : "UnScheduled") + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "SchedulerName")
                {
                    ColUDFData = (from tsm in context.ToolsMaintenances
                                  join ts in context.ToolsSchedulers on tsm.SchedulerGUID equals ts.GUID
                                  where
                                  (tsm.IsArchived ?? false) == false && (tsm.IsDeleted ?? false) == false && (tsm.Room ?? 0) == RoomID && (tsm.CompanyID ?? 0) == CompanyID && ((tabName == "history") ? mntshist.Contains(tsm.Status) : true) && ((tabName == "due") ? mntsDue.Contains(tsm.Status) : true)
                                  group new { ts, tsm } by new { ts.ID, ts.SchedulerName } into g
                                  select new
                                  {
                                      id = g.Key.ID,
                                      name = g.Key.SchedulerName,
                                      count = g.Count(p => p.tsm.SchedulerGUID != null)
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => e.name + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "UpdatedBy")
                {
                    ColUDFData = (from ts in context.ToolsMaintenances
                                  join UM in context.UserMasters on (ts.LastUpdatedBy ?? 0) equals UM.ID
                                  where
                                    (ts.IsArchived ?? false) == false &&
                                    (ts.IsDeleted ?? false) == false &&
                                    (ts.Room ?? 0) == RoomID && (ts.CompanyID ?? 0) == CompanyID && ((tabName == "history") ? mntshist.Contains(ts.Status) : true) && ((tabName == "due") ? mntsDue.Contains(ts.Status) : true)
                                  group new { ts, UM } by new
                                  {
                                      UM.UserName,
                                      UM.ID
                                  } into g
                                  select new
                                  {
                                      id = g.Key.ID,
                                      name = g.Key.UserName,
                                      count = g.Count()
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => e.name + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;
                }
                else if (TextFieldName == "CreatedBy")
                {
                    ColUDFData = (from ts in context.ToolsMaintenances
                                  join UM in context.UserMasters on (ts.CreatedBy ?? 0) equals UM.ID
                                  where (ts.IsArchived ?? false) == false && (ts.IsDeleted ?? false) == false && (ts.Room ?? 0) == RoomID && (ts.CompanyID ?? 0) == CompanyID && ((tabName == "history") ? mntshist.Contains(ts.Status) : true) && ((tabName == "due") ? mntsDue.Contains(ts.Status) : true)
                                  group new { ts, UM } by new { UM.UserName, UM.ID } into g
                                  select new
                                  {
                                      id = g.Key.ID,
                                      name = g.Key.UserName,
                                      count = g.Count()
                                  }).AsParallel().Where(x => x.count > 0).ToDictionary(e => e.name + "[###]" + e.id.ToString(), e => e.count);
                    return ColUDFData;

                }
                else
                {

                    return ColUDFData;
                }


            }
            #endregion
        }
        private Dictionary<string, int> GetNarrowDDData_PullMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, List<long> UserSupplierIds, bool UserConsignmentAllowed,int LoadDataCount,long UserID)
        {
            PullMasterDAL objGLAccDAL = new PullMasterDAL(base.DataBaseName);
            return objGLAccDAL.GetPullMasterListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, UserSupplierIds, UserConsignmentAllowed, LoadDataCount, UserID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

    }
}
