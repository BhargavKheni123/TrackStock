using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ImportBOMItemMasterDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        #region [Class Constructor]

        //public ImportDAL(base.DataBaseName)
        //{

        //}

        public ImportBOMItemMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ImportDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        List<ItemGUIDLocation> lstItemGUID = new List<ItemGUIDLocation>();

        public List<T> BulkInsert<T>(string tableName, IList<T> list, long RoomId, long CompanyId, string ColumnName, long UserId, long SessionUserId, long EnterpriseID, List<UDFOptionsMain> lstOption = null, bool isImgZipAvail = false, bool isLink2ZipAvail = false, bool _AllowToolOrdering = false)
        {

            string[] arrcolumns = ColumnName.ToLower().Split(',');
            List<T> lstreturnFinal = new List<T>();

            using (var bulkCopy = new SqlBulkCopy(DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F")), SqlBulkCopyOptions.FireTriggers))
            {
                bulkCopy.BulkCopyTimeout = 0;
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                PropertyDescriptor[] props;

                if (tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "DefaultCartQuantity" && propertyInfo.Name != "RoomName" && propertyInfo.Name != "CreatedByName" &&
                                propertyInfo.Name != "UpdatedByName" && propertyInfo.Name != "Action" && propertyInfo.Name != "HistoryID" &&
                                propertyInfo.Name != "CategoryName" && propertyInfo.Name != "CategoryColor" && propertyInfo.Name != "ItemLocations" &&
                                propertyInfo.Name != "Manufacturer" && propertyInfo.Name != "SupplierName" &&
                                propertyInfo.Name != "ItemTypeName" && propertyInfo.Name != "Unit" && propertyInfo.Name != "GLAccount" &&
                                propertyInfo.Name != "InventryLocation" && propertyInfo.Name != "AppendedBarcodeString" && propertyInfo.Name != "ManufacturerName"
                                && propertyInfo.Name != "QuickListName" && propertyInfo.Name != "QuickListGUID" && propertyInfo.Name != "QuickListItemQTY"
                                && propertyInfo.Name != "CustomerOwnedQuantity" && propertyInfo.Name != "RefBomI"
                                && propertyInfo.Name != "ConsignedQuantity"
                                && propertyInfo.Name != "CountConsignedQuantity"
                                && propertyInfo.Name != "CountCustomerOwnedQuantity" && propertyInfo.Name != "BinID" && propertyInfo.Name != "BinNumber"
                                && propertyInfo.Name != "DefaultLocationName" && propertyInfo.Name != "TrendingSettingName"
                                && propertyInfo.Name != "CountLineItemDescriptionEntry" && propertyInfo.Name != "StockOutCount" && propertyInfo.Name != "MonthValue"
                                && propertyInfo.Name != "CostUOMName" && propertyInfo.Name != "InventoryClassificationName" && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                                && propertyInfo.Name != "DispExtendedCost" && propertyInfo.Name != "DispStagedQuantity" && propertyInfo.Name != "DispInTransitquantity"
                                && propertyInfo.Name != "DispOnOrderQuantity" && propertyInfo.Name != "DispOnTransferQuantity" && propertyInfo.Name != "DispSuggestedOrderQuantity" && propertyInfo.Name != "DispRequisitionedQuantity"
                                && propertyInfo.Name != "DispAverageUsage" && propertyInfo.Name != "DispTurns" && propertyInfo.Name != "DispOnHandQuantity"
                                && propertyInfo.Name != "BlanketOrderNumber" && propertyInfo.Name != "BlanketPOID" && propertyInfo.Name != "UOM"
                                && propertyInfo.Name != "PerItemCost" && propertyInfo.Name != "OutTransferQuantity" && propertyInfo.Name != "CompanyNumber" && propertyInfo.Name != "IsBlankConsignment" && propertyInfo.Name != "ISNullConsignment"
                                )
                        .ToArray();
                }
                else
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                   .Cast<PropertyDescriptor>()
                   .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                   .ToArray();
                }

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }
                List<T> lstFinalAdd = new List<T>();
                List<T> lstEdit = new List<T>();
                List<T> lstFinalDelete = new List<T>();
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    #region BOM Item Master
                    if (tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                    {
                        CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
                        BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(base.DataBaseName);
                        string cultureCode = "en-US";
                        var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomId, CompanyId, -1);

                        if (regionInfo != null)
                        {
                            cultureCode = regionInfo.CultureCode;
                        }
                        List<string> p = (from d in context.ItemMasters where d.Room == null && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select (d.SupplierPartNo ?? string.Empty).Trim().ToLower()).ToList();
                        
                        List<string> Q = (from d in (List<BOMItemMasterMain>)list select d.SupplierPartNo).ToList();
                        List<ItemMaster> lstItemMaster = (from m in context.ItemMasters
                                                          where (Q.Contains(m.SupplierPartNo)) && m.Room == null && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                          select m).ToList();

                        #region Bulk Delete
                        lstFinalDelete = (from m in (List<BOMItemMasterMain>)list
                                          where (p.Contains((m.SupplierPartNo ?? string.Empty).Trim().ToLower())) && m.IsDeleted == true
                                          select m).Cast<T>().ToList();

                        bool DeleteItems = false;
                        foreach (BOMItemMasterMain objDTO in lstFinalDelete.Cast<BOMItemMasterMain>().ToList())
                        {
                            try
                            {
                                DeleteItems = true;
                                ItemMaster obj = (from m in lstItemMaster
                                                  where m.SupplierPartNo == objDTO.SupplierPartNo && m.CompanyID == objDTO.CompanyID
                                                  select m).FirstOrDefault();
                                if (obj == null)
                                {
                                    obj = new ItemMaster();
                                }
                                string response = string.Empty;
                                ModuleDeleteDTO objModuleDeleteDTO = new ModuleDeleteDTO();
                                objModuleDeleteDTO = objCommonDAL.DeleteModulewiseForBOM(obj.GUID.ToString(), ImportMastersDTO.TableName.BOMItemMaster.ToString(), true, UserId, EnterpriseID, CompanyId, RoomId);

                                if (objModuleDeleteDTO != null && objModuleDeleteDTO.SuccessItems != null && objModuleDeleteDTO.SuccessItems.Count > 0)
                                {
                                    objDTO.Status = "Success";
                                    objDTO.Reason = "N/A";
                                    lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                }
                                else
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = objModuleDeleteDTO.CommonMessage;
                                    lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                }
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }

                        }
                        if (DeleteItems)
                        {
                            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                            objItemMasterDAL.CleanDeletedItemRefs(RoomId, CompanyId);
                        }
                        #endregion

                        #region Edit
                        lstEdit = (from m in (List<BOMItemMasterMain>)list
                                   where (p.Contains((m.SupplierPartNo ?? string.Empty).Trim().ToLower())) && m.IsDeleted == false
                                   select m).Cast<T>().ToList();
                        foreach (BOMItemMasterMain objDTO in lstEdit.Cast<BOMItemMasterMain>().ToList())
                        {
                            try
                            {
                                ItemMaster obj = (from m in lstItemMaster
                                                  where ((m.SupplierPartNo ?? string.Empty).Trim().ToLower()) == ((objDTO.SupplierPartNo ?? string.Empty).Trim().ToLower()) && m.Room == null
                                                  select m).FirstOrDefault();

                                if (obj != null)
                                {
                                    //BOMItemMasterDAL objItemDal = new BOMItemMasterDAL(base.DataBaseName);
                                    //BOMItemDTO ObjITEM2 = objItemDal.GetItemByItemID(obj.ID);
                                    objDTO.ID = obj.ID;
                                    objDTO.GUID = obj.GUID;
                                    if (objDTO.InventoryClassification == 0)
                                    {
                                        objDTO.InventoryClassification = null;
                                    }
                                    objDTO.WhatWhereAction = "BomServiceImportEdit";

                                    if (objDTO.ItemNumber.ToLower() != obj.ItemNumber.ToLower())
                                    {
                                        string strOK = objBOMItemMasterDAL.BOMDuplicateCheck(objDTO.ID, objDTO.ItemNumber, CompanyId);
                                        if (strOK == "duplicate")
                                        {
                                            objDTO.Status = "Fail";
                                            objDTO.Reason = "Item Number already exist.";
                                        }
                                        else
                                        {
                                            objDTO.Status = "Success";
                                            objDTO.Reason = "N/A";
                                        }
                                    }

                                    if (string.IsNullOrEmpty(objDTO.Status) || objDTO.Status.ToLower() == "success")
                                    {
                                        ItemManufacturerDetailsDTO DefaultBOMItemManufacturer = new ItemManufacturerDetailsDTO();

                                        List<ItemManufacturerDetailsDTO> lstOfManufacturer = objBOMItemMasterDAL.GetAllRecordsManuBOMForItem(0, obj.CompanyID ?? 0, obj.GUID).ToList();

                                        if (lstOfManufacturer != null && lstOfManufacturer.Count > 0)
                                        {
                                            DefaultBOMItemManufacturer = lstOfManufacturer.Where(x => x.IsDefault == true).FirstOrDefault();

                                            List<long> lstOfManufactureIds = lstOfManufacturer.Where(x => x.IsDefault == false).Select(x => x.ManufacturerID).ToList();

                                            DefaultBOMItemManufacturer.ItemGUID = obj.GUID;
                                            DefaultBOMItemManufacturer.ManufacturerName = objDTO.ManufacturerName;
                                            DefaultBOMItemManufacturer.ManufacturerNumber = objDTO.ManufacturerNumber;
                                            DefaultBOMItemManufacturer.Updated = DateTimeUtility.DateTimeNow;

                                            if (lstOfManufactureIds.Contains(objDTO.ManufacturerID ?? 0))
                                            {
                                                objDTO.Status = "Fail";
                                                objDTO.Reason = "This Manufacturer is already added";
                                            }
                                        }

                                        if (string.IsNullOrEmpty(objDTO.Status) || objDTO.Status.ToLower() == "success")
                                        {
                                            EditBOMItemMaster(objDTO, arrcolumns, DefaultBOMItemManufacturer, obj);
                                        }

                                        objDTO.Status = (!string.IsNullOrEmpty(objDTO.Status) ? objDTO.Status : "Success");
                                        objDTO.Reason = (!string.IsNullOrEmpty(objDTO.Reason) ? objDTO.Reason : "N/A");
                                    }
                                    lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                }
                                else
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = "Supplier PN does not exist.";
                                    lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                }
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                        #endregion

                        #region Insert Bulk Data
                        lstFinalAdd = (from m in (List<BOMItemMasterMain>)list
                                       where (!p.Contains((m.SupplierPartNo ?? string.Empty).Trim().ToLower())) && m.IsDeleted == false //&& m.Category != ""
                                       select m).Cast<T>().ToList();
                        if (lstFinalAdd != null && lstFinalAdd.Count > 0)
                        {
                            try
                            {
                                if (tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                                {
                                    bulkCopy.DestinationTableName = "ItemMaster";
                                }
                                var values = new object[props.Length];

                                foreach (BOMItemMasterMain objDTO in lstFinalAdd.Cast<BOMItemMasterMain>().ToList())
                                {
                                    string strOK = objBOMItemMasterDAL.BOMDuplicateCheck(0, objDTO.ItemNumber, CompanyId);
                                    if (strOK == "duplicate")
                                    {
                                        objDTO.Status = "Fail";
                                        objDTO.Reason = "Item Number already exist.";
                                    }
                                    else
                                    {
                                        objDTO.Status = "Success";
                                    }
                                }
                                List<T> lstValidItems = lstFinalAdd.Cast<BOMItemMasterMain>().Where(x => x.Status.ToLower() == "success").Cast<T>().ToList();

                                foreach (var item in lstValidItems)
                                {
                                    for (var i = 0; i < values.Length; i++)
                                    {
                                        values[i] = props[i].GetValue(item);
                                    }
                                    table.Rows.Add(values);
                                }
                                bulkCopy.WriteToServer(table);

                                lstreturnFinal = GetUpdatedList(lstFinalAdd, tableName, "Success", lstreturnFinal, "N/A");
                                if (tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                                {
                                    foreach (ItemGUIDLocation objItemGuidU in lstItemGUID)
                                    {
                                        if (CompanyId > 0 && UserId > 0 && objItemGuidU.ItemGUID != null)
                                        {
                                            UpdateBOMItemMSLDetail(objItemGuidU.ItemGUID, CompanyId, UserId);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                lstreturnFinal = GetUpdatedList(lstFinalAdd, tableName, "Fail", lstreturnFinal, ex.Message.ToString());

                                return lstreturnFinal;
                                //throw ex;
                            }
                        }
                        
                        #endregion
                    }
                    #endregion
                }
            }

            if (lstOption != null && lstOption.Count > 0)
            {
                using (var bulkCopy = new SqlBulkCopy(DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F")), SqlBulkCopyOptions.FireTriggers))
                {


                    bulkCopy.BatchSize = list.Count;
                    bulkCopy.DestinationTableName = "UDFOptions";

                    var tableudf = new DataTable();
                    var propsudf = TypeDescriptor.GetProperties(typeof(UDFOptionsMain))
                    .Cast<PropertyDescriptor>()
                    .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                    .ToArray();
                    foreach (var propertyInfo in propsudf)
                    {
                        //bulkCopy.ColumnMappings.Add("", "");
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        tableudf.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }


                    #region Insert Bulk Data
                    var valuesudf = new object[propsudf.Length];
                    foreach (var item in lstOption)
                    {
                        for (var i = 0; i < valuesudf.Length; i++)
                        {
                            valuesudf[i] = propsudf[i].GetValue(item);
                        }
                        tableudf.Rows.Add(valuesudf);

                    }
                    bulkCopy.WriteToServer(tableudf);
                    #endregion
                }
            }
            // lstreturn = new List<T>();
            return lstreturnFinal;
        }
        public List<T> GetUpdatedList<T>(IList<T> lstItems, string TableName, string Status, IList<T> lstRetItems, string reason)
        {
            List<T> lstReturnFinal = new List<T>();
            if (TableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
            {
                foreach (BOMItemMasterMain item in lstItems.Cast<BOMItemMasterMain>())
                {
                    item.Status = (string.IsNullOrEmpty(item.Status) ? Status : item.Status);
                    item.Reason =(string.IsNullOrEmpty(item.Reason) ? reason : item.Reason);
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));

                    ItemGUIDLocation objItemGUIDLocation = new ItemGUIDLocation();
                    objItemGUIDLocation.ItemGUID = item.GUID;
                    objItemGUIDLocation.LocationName = string.Empty;
                    lstItemGUID.Add(objItemGUIDLocation);
                }
            }
            lstReturnFinal = lstRetItems.ToList();
            return lstReturnFinal;
        }
        public void UpdateBOMItemMSLDetail(Guid ItemGUID, long CompanyId, long UserId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "USP_UpdateBOMItemMSLDetail", ItemGUID, UserId, CompanyId);
        }
        public bool EditBOMItemMaster(BOMItemMasterMain objDTO, string[] arrColumns, ItemManufacturerDetailsDTO BOMItemManufacturer, ItemMaster EditItem)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = new ItemMaster();
                obj = EditItem;
                if (obj != null)
                {
                    if (arrColumns.Contains("item number"))
                    {
                        obj.ItemNumber = objDTO.ItemNumber;
                    }
                    if (objDTO.ManufacturerID > 0)
                        obj.ManufacturerID = objDTO.ManufacturerID;
                    if (arrColumns.Contains("manufacturernumber"))
                    {
                        obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                    }
                    if (objDTO.SupplierID > 0)
                        obj.SupplierID = objDTO.SupplierID;
                    if (arrColumns.Contains("supplierpartno"))
                    {
                        obj.SupplierPartNo = objDTO.SupplierPartNo;
                    }

                    if (arrColumns.Contains("upc"))
                    {
                        obj.UPC = objDTO.UPC;
                    }

                    if (arrColumns.Contains("unspsc"))
                    {
                        obj.UNSPSC = objDTO.UNSPSC;
                    }

                    if (arrColumns.Contains("description"))
                    {
                        obj.Description = objDTO.Description;
                    }

                    if (arrColumns.Contains("udf1"))
                    {
                        if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
                            obj.CategoryID = objDTO.CategoryID;
                    }

                    if (arrColumns.Contains("glaccount"))
                    {
                        if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
                            obj.GLAccountID = objDTO.GLAccountID;
                    }

                    if (arrColumns.Contains("unit"))
                    {
                        if (objDTO.UOMID > 0)
                            obj.UOMID = objDTO.UOMID;
                    }

                    if (arrColumns.Contains("leadtimeindays"))
                    {
                        obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                    }

                    if (arrColumns.Contains("taxable"))
                    {
                        obj.Taxable = objDTO.Taxable;
                    }

                    if (arrColumns.Contains("consignment"))
                    {
                        obj.Consignment = objDTO.Consignment;
                    }


                    if (arrColumns.Contains("itemuniquenumber"))
                    {
                        obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                    }


                    //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                    if (arrColumns.Contains("ispurchase"))
                    {
                        obj.IsPurchase = objDTO.IsPurchase;
                    }

                    if (arrColumns.Contains("istransfer"))
                    {
                        obj.IsTransfer = objDTO.IsTransfer;
                    }

                    if (arrColumns.Contains("inventrylocation"))
                    {
                        obj.DefaultLocation = objDTO.DefaultLocation;
                    }

                    if (arrColumns.Contains("inventoryclassification"))
                    {
                        obj.InventoryClassification = objDTO.InventoryClassification;
                    }

                    //if (arrColumns.Contains("serialnumbertracking"))
                    //{
                    //    obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                    //}

                    //if (arrColumns.Contains("lotnumbertracking"))
                    //{
                    //    obj.LotNumberTracking = objDTO.LotNumberTracking;
                    //}

                    //if (arrColumns.Contains("datecodetracking"))
                    //{
                    //    obj.DateCodeTracking = objDTO.DateCodeTracking;
                    //}

                    if (arrColumns.Contains("itemtype"))
                    {
                        obj.ItemType = objDTO.ItemType;
                    }


                    if (arrColumns.Contains("udf1"))
                    {
                        obj.UDF1 = objDTO.UDF1;
                    }

                    if (arrColumns.Contains("udf2"))
                    {
                        obj.UDF2 = objDTO.UDF2;
                    }

                    if (arrColumns.Contains("udf3"))
                    {
                        obj.UDF3 = objDTO.UDF3;
                    }

                    if (arrColumns.Contains("udf4"))
                    {
                        obj.UDF4 = objDTO.UDF4;
                    }

                    if (arrColumns.Contains("udf5"))
                    {
                        obj.UDF5 = objDTO.UDF5;
                    }
                    if (arrColumns.Contains("criticalquantity"))
                    {
                        obj.CriticalQuantity = objDTO.CriticalQuantity;
                    }
                    if (arrColumns.Contains("minimumquantity"))
                    {
                        obj.MinimumQuantity = objDTO.MinimumQuantity;
                    }
                    if (arrColumns.Contains("maximumquantity"))
                    {
                        obj.MaximumQuantity = objDTO.MaximumQuantity;
                    }
                    if (arrColumns.Contains("cost"))
                    {
                        obj.LastCost = objDTO.Cost ?? 0;
                        obj.Cost = objDTO.Cost ?? 0;
                    }
                    if (arrColumns.Contains("markup"))
                    {
                        obj.Markup = objDTO.Markup;
                    }
                    if (arrColumns.Contains("sellprice"))
                    {
                        obj.SellPrice = objDTO.SellPrice;
                    }
                    if (arrColumns.Contains("costuom"))
                    {
                        obj.CostUOMID = objDTO.CostUOMID;
                    }
                    if (arrColumns.Contains("defaultreorderquantity"))
                    {
                        obj.DefaultReorderQuantity = objDTO.DefaultReorderQuantity ?? 0;
                    }
                    if (arrColumns.Contains("defaultpullquantity"))
                    {
                        obj.DefaultPullQuantity = objDTO.DefaultPullQuantity ?? 0;
                    }
                    if (arrColumns.Contains("link1"))
                    {
                        obj.Link1 = objDTO.Link1;
                    }
                    if (arrColumns.Contains("link2"))
                    {
                        obj.Link2 = objDTO.Link2;
                    }
                    if (arrColumns.Contains("isitemlevelminmaxqtyrequired"))
                    {
                        obj.IsItemLevelMinMaxQtyRequired = objDTO.IsItemLevelMinMaxQtyRequired;
                    }
                    if (arrColumns.Contains("enforcedefaultpullquantity"))
                    {
                        obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                    }
                    if (arrColumns.Contains("enforcedefaultreorderquantity"))
                    {
                        obj.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                    }
                    if (arrColumns.Contains("itemimageexternalurl"))
                    {
                        obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                    }
                    if (arrColumns.Contains("itemdocexternalurl"))
                    {
                        obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;
                    }
                    if (arrColumns.Contains("itemlink2externalurl"))
                    {
                        obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                    }
                    if (arrColumns.Contains("isactive"))
                    {
                        obj.IsActive = objDTO.IsActive;
                    }
                    if (arrColumns.Contains("longdescription"))
                    {
                        obj.LongDescription = objDTO.LongDescription;
                    }
                    if (arrColumns.Contains("enhanceddescription"))
                    {
                        obj.EnhancedDescription = objDTO.EnhancedDescription;
                    }
                    if (arrColumns.Contains("enrichedproductdata"))
                    {
                        obj.EnrichedProductData = objDTO.EnrichedProductData;
                    }
                    obj.IsActive = objDTO.IsActive;
                    obj.Updated = objDTO.Updated;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "BomServiceImportEdit";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;

                    context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    
                    ItemGUIDLocation objItemGUIDLocation = new ItemGUIDLocation();
                    objItemGUIDLocation.ItemGUID = objDTO.GUID;
                    objItemGUIDLocation.LocationName = string.Empty;
                    lstItemGUID.Add(objItemGUIDLocation);

                    //----WI-8348----//
                    BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(base.DataBaseName);
                    ItemManufacturerDetailsDAL objItemLocationLevelQuanityDAL = new ItemManufacturerDetailsDAL(base.DataBaseName);
                    long ManufacturerID = obj.ManufacturerID ?? 0;

                    if (BOMItemManufacturer.ID == 0 && ManufacturerID > 0) // when we first time add manufacturer at edit item.
                    {
                        BOMItemManufacturer.ManufacturerName = objDTO.ManufacturerName;
                        BOMItemManufacturer.ManufacturerNumber = objDTO.ManufacturerNumber;
                        BOMItemManufacturer.Room = objDTO.Room;
                        BOMItemManufacturer.CompanyID = obj.CompanyID;
                        BOMItemManufacturer.ItemGUID = obj.GUID;
                        BOMItemManufacturer.GUID = Guid.NewGuid();
                        BOMItemManufacturer.EditedFrom = "BomServiceImportEdit";
                        BOMItemManufacturer.ReceivedOn = DateTimeUtility.DateTimeNow;
                        BOMItemManufacturer.AddedFrom = "BomServiceImportEdit";
                        BOMItemManufacturer.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        BOMItemManufacturer.ManufacturerID = ManufacturerID;
                        BOMItemManufacturer.CreatedBy = objDTO.CreatedBy;
                        BOMItemManufacturer.LastUpdatedBy = objDTO.LastUpdatedBy;
                        BOMItemManufacturer.IsDefault = true;
                        objItemLocationLevelQuanityDAL.Insert(BOMItemManufacturer);
                    }
                    else if (BOMItemManufacturer.ID > 0 && (BOMItemManufacturer.ManufacturerID != ManufacturerID))
                    {
                        BOMItemManufacturer.EditedFrom = "BomServiceImportEdit";
                        BOMItemManufacturer.ReceivedOn = DateTimeUtility.DateTimeNow;
                        BOMItemManufacturer.AddedFrom = "BomServiceImportEdit";
                        BOMItemManufacturer.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        BOMItemManufacturer.ManufacturerID = ManufacturerID;
                        objItemLocationLevelQuanityDAL.Edit(BOMItemManufacturer);
                    }
                    //----WI-8348----//

                    objBOMItemMasterDAL.UpdateReferenceBOMItem(obj.ID, objDTO.LastUpdatedBy ?? 0);
                }
            }
            return true;
        }
    }
}