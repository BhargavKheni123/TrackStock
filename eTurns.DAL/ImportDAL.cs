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
    public partial class ImportDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        #region [Class Constructor]

        //public ImportDAL(base.DataBaseName)
        //{

        //}

        public ImportDAL(string DbName)
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

                if (tableName == ImportMastersDTO.TableName.ItemMaster.ToString() || tableName == ImportMastersDTO.TableName.EditItemMaster.ToString() || tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
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
                else if (tableName == ImportMastersDTO.TableName.ToolMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "CheckOutStatus" && propertyInfo.Name != "CheckOutDate" && propertyInfo.Name != "CheckInDate"
                         && propertyInfo.Name != "Technician"
                         && propertyInfo.Name != "TechnicianGuid" && propertyInfo.Name != "CheckInQuantity" &&

        propertyInfo.Name != "CheckOutQuantity" &&

        propertyInfo.Name != "ToolCategory" && propertyInfo.Name != "RoomName" && propertyInfo.Name != "Location" &&
                                 propertyInfo.Name != "CreatedByName" && propertyInfo.Name != "UpdatedByName" &&

                                 propertyInfo.Name != "CheckOutUDF1" && propertyInfo.Name != "CheckOutUDF2" &&
                                 propertyInfo.Name != "CheckOutUDF3" && propertyInfo.Name != "CheckOutUDF4" &&
                                 propertyInfo.Name != "CheckOutUDF5" &&
                                propertyInfo.Name != "Action" && propertyInfo.Name != "HistoryID" && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason")
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.AssetMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "RoomName" && propertyInfo.Name != "CreatedByName" && propertyInfo.Name != "UpdatedByName" && propertyInfo.Name != "AssetCategory" && propertyInfo.Name != "PurchaseDateString" &&

                                propertyInfo.Name != "ToolCategory" && propertyInfo.Name != "Action" && propertyInfo.Name != "HistoryID" && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                                 )
                        .ToArray();
                }

                else if (tableName == ImportMastersDTO.TableName.AssetToolSchedulerMapping.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                       .Cast<PropertyDescriptor>()
                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                       && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                       && propertyInfo.Name.ToLower() != "assetname" && propertyInfo.Name.ToLower() != "scheduleforname"
                       && propertyInfo.Name.ToLower() != "serial" && propertyInfo.Name.ToLower() != "schedulername"
                       && propertyInfo.Name.ToLower() != "toolname").ToArray();
                }

                else if (tableName == ImportMastersDTO.TableName.QuickListItems.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "QuickListname" && propertyInfo.Name != "Type" && propertyInfo.Name != "QLType" &&
                                propertyInfo.Name != "BinNumber" && propertyInfo.Name != "ItemNumber" && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason" && propertyInfo.Name != "Comments"
                                 )
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.InventoryClassificationMaster.ToString() || tableName == ImportMastersDTO.TableName.CategoryMaster.ToString() || tableName == ImportMastersDTO.TableName.CustomerMaster.ToString() || tableName == ImportMastersDTO.TableName.BinMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "Status" && propertyInfo.Name != "displayExpiration" && propertyInfo.Name != "Reason"
                                 )
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.TechnicianMaster.ToString() || tableName == ImportMastersDTO.TableName.ShipViaMaster.ToString() || tableName == ImportMastersDTO.TableName.GLAccountMaster.ToString() || tableName == ImportMastersDTO.TableName.FreightTypeMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"

                                 )
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.ManufacturerMaster.ToString() || tableName == ImportMastersDTO.TableName.MeasurementTermMaster.ToString() || tableName == ImportMastersDTO.TableName.UnitMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"

                                 )
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.LocationMaster.ToString() || tableName == ImportMastersDTO.TableName.ToolCategoryMaster.ToString() || tableName == ImportMastersDTO.TableName.CostUOMMaster.ToString() || tableName == ImportMastersDTO.TableName.InventoryLocation.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "KitPartNumber" && propertyInfo.Name != "ItemNumber"
                          && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"

                                 )
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.kitdetail.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                                          .Cast<PropertyDescriptor>()
                                          .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                                          && propertyInfo.Name != "KitPartNumber" && propertyInfo.Name != "ItemNumber"
                                            && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                                            && propertyInfo.Name != "SupplierName" && propertyInfo.Name != "SupplierPartNo"
                                            && propertyInfo.Name != "DefaultLocation" && propertyInfo.Name != "DefaultLocationName"
                                            && propertyInfo.Name != "CostUOMName" && propertyInfo.Name != "CostUOMID"
                                            && propertyInfo.Name != "CostUOMValue" && propertyInfo.Name != "UOMID"
                                            && propertyInfo.Name != "UOM" && propertyInfo.Name != "DefaultReorderQuantity"
                                            && propertyInfo.Name != "DefaultPullQuantity" && propertyInfo.Name != "ItemType"
                                            && propertyInfo.Name != "ItemTypeName" && propertyInfo.Name != "IsItemLevelMinMaxQtyRequired"
                                            && propertyInfo.Name != "IsBuildBreak" && propertyInfo.Name != "OnHandQuantity"
                                            && propertyInfo.Name != "CriticalQuantity" && propertyInfo.Name != "MinimumQuantity"
                                            && propertyInfo.Name != "MaximumQuantity" && propertyInfo.Name != "ReOrderType"
                                            && propertyInfo.Name != "KitCategory" && propertyInfo.Name != "AvailableKitQuantity"
                                            && propertyInfo.Name != "Description" && propertyInfo.Name != "KitDemand"
                                            && propertyInfo.Name != "SerialNumberTracking" && propertyInfo.Name != "LotNumberTracking"
                                            && propertyInfo.Name != "DateCodeTracking" && propertyInfo.Name != "IsActive" && propertyInfo.Name != "BinNumber"
                                                   )
                                          .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.SupplierMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                        && propertyInfo.Name != "OrderNumberTypeBlank" && propertyInfo.Name != "OrderNumberTypeFixed"
                        && propertyInfo.Name != "OrderNumberTypeBlanketOrderNumber" && propertyInfo.Name != "OrderNumberTypeIncrementingOrderNumber"
                        && propertyInfo.Name != "OrderNumberTypeIncrementingbyDay" && propertyInfo.Name != "OrderNumberTypeDateIncrementing"
                        && propertyInfo.Name != "OrderNumberTypeDate" && propertyInfo.Name != "AccountNumber"
                        && propertyInfo.Name != "BlanketPONumber" && propertyInfo.Name != "StartDate"
                        && propertyInfo.Name != "EndDate" && propertyInfo.Name != "MaxLimit"
                        && propertyInfo.Name != "IsNotExceed" && propertyInfo.Name != "MaxLimitQty"
                        && propertyInfo.Name != "IsNotExceedQty"
                                 )
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.ItemManufacturerDetails.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                       .Cast<PropertyDescriptor>()
                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                       && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                       && propertyInfo.Name != "ItemNumber"
                                )
                       .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.ItemSupplierDetails.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                       .Cast<PropertyDescriptor>()
                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                       && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                       && propertyInfo.Name != "ItemNumber" && propertyInfo.Name != "BlanketPOName"
                                )
                       .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.BarcodeMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                      .Cast<PropertyDescriptor>()
                      .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                      && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                      && propertyInfo.Name != "ItemNumber" && propertyInfo.Name != "BinNumber"
                      && propertyInfo.Name != "ModuleName"
                               )
                      .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.ProjectMaster.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                      .Cast<PropertyDescriptor>()
                      .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                      && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                    && propertyInfo.Name != "ItemNumber" && propertyInfo.Name != "ItemDollarLimitAmount"
                    && propertyInfo.Name != "ItemQuantityLimitAmount" && propertyInfo.Name != "IsLineItemDeleted"

                               )
                      .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.WorkOrder.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                        && propertyInfo.Name != "RoomName" && propertyInfo.Name != "ID" && propertyInfo.Name != "CreatedByName" && propertyInfo.Name != "UpdatedByName" && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                        && propertyInfo.Name != "SupplierName" && propertyInfo.Name != "SupplierAccount"
                                 )
                        .ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.AssetToolScheduler.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                       .Cast<PropertyDescriptor>()
                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                       && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                       && propertyInfo.Name.ToLower() != "schedulertypename" && propertyInfo.Name.ToLower() != "scheduleforname"
                       && propertyInfo.Name.ToLower() != "itemnumber" && propertyInfo.Name.ToLower() != "itemguid"
                       && propertyInfo.Name.ToLower() != "quantity"
                       && propertyInfo.Name.ToLower() != "timebasedunitname").ToArray();
                }
                else if (tableName == ImportMastersDTO.TableName.PastMaintenanceDue.ToString())
                {
                    props = TypeDescriptor.GetProperties(typeof(T))
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                         && propertyInfo.Name != "Reason" && propertyInfo.Name != "ItemNumber"
                         && propertyInfo.Name != "AssetName"
                         && propertyInfo.Name != "ToolName" && propertyInfo.Name != "Serial"
                         && propertyInfo.Name != "SchedulerName" && propertyInfo.Name != "displayMaitenanceDate"
                         && propertyInfo.Name != "ActionType" && propertyInfo.Name != "WorkOrder"
                         && propertyInfo.Name != "ItemCost" && propertyInfo.Name != "Quantity"
                         && propertyInfo.Name != "AddedFrom" && propertyInfo.Name != "EditedFrom"
                         && propertyInfo.Name != "ReceivedOn" && propertyInfo.Name != "ReceivedOnWeb"

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

                if (tableName == ImportMastersDTO.TableName.ItemMaster.ToString() || tableName == ImportMastersDTO.TableName.EditItemMaster.ToString())
                {
                    bulkCopy.ColumnMappings.Add("ItemIsActiveDate", "ItemIsActiveDate");
                    table.Columns.Add("ItemIsActiveDate", typeof(System.DateTime));
                }
                List<T> lstFinalAdd = new List<T>();
                List<T> lstEdit = new List<T>();
                List<T> lstFinalDelete = new List<T>();
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    #region Bin Master
                    //if (tableName == ImportMastersDTO.TableName.BinMaster.ToString())
                    //{
                    //    List<string> p = (from d in context.BinMasters where d.Room == RoomId && d.CompanyID == CompanyId select d.BinNumber).ToList();
                    //    lstFinalAdd = (from m in (List<BinMasterMain>)list
                    //                   where (!p.Contains(m.BinNumber)) //&& m.BinNumber != ""
                    //                   select m).Cast<T>().ToList();

                    //    lstEdit = (from m in (List<BinMasterMain>)list
                    //               where (p.Contains(m.BinNumber)) //&& m.BinNumber != ""
                    //               select m).Cast<T>().ToList();

                    //    //lstreturnFinal = (from m in (List<BinMasterMain>)list
                    //    //                  where m.BinNumber == ""
                    //    //                  select m).Cast<T>().ToList();

                    //    List<string> Q = (from d in (List<BinMasterMain>)list select d.BinNumber).ToList();
                    //    List<BinMaster> lstBinmaster = (from m in context.BinMasters
                    //                                    where (Q.Contains(m.BinNumber)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsDeleted == false && m.IsArchived == false
                    //                                    select m).ToList();


                    //    foreach (BinMasterMain objDTO in lstEdit.Cast<BinMasterMain>().ToList())
                    //    {
                    //        try
                    //        {
                    //            BinMaster obj = (from m in lstBinmaster
                    //                             where m.BinNumber == objDTO.BinNumber && m.Room == RoomId && m.CompanyID == CompanyId
                    //                             select m).SingleOrDefault();
                    //            if (arrcolumns.Contains("binnumber"))
                    //            {
                    //                obj.BinNumber = objDTO.BinNumber;
                    //            }
                    //            obj.IsStagingLocation = objDTO.IsStagingLocation;
                    //            obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    //            // obj.Room = objDTO.Room;
                    //            obj.LastUpdated = objDTO.LastUpdated;
                    //            //obj.Created = objDTO.Created;
                    //            // obj.CreatedBy = objDTO.CreatedBy;
                    //            //obj.GUID = objDTO.GUID;
                    //            //obj.IsDeleted = (bool)objDTO.IsDeleted;
                    //            // obj.IsArchived = (bool)objDTO.IsArchived;
                    //            //obj.CompanyID = objDTO.CompanyID;
                    //            //obj.Room = objDTO.Room;
                    //            if (arrcolumns.Contains("udf1"))
                    //            {
                    //                obj.UDF1 = objDTO.UDF1;
                    //            }

                    //            if (arrcolumns.Contains("udf2"))
                    //            {
                    //                obj.UDF2 = objDTO.UDF2;
                    //            }

                    //            if (arrcolumns.Contains("udf3"))
                    //            {
                    //                obj.UDF3 = objDTO.UDF3;
                    //            }

                    //            if (arrcolumns.Contains("udf4"))
                    //            {
                    //                obj.UDF4 = objDTO.UDF4;
                    //            }

                    //            if (arrcolumns.Contains("udf5"))
                    //            {
                    //                obj.UDF5 = objDTO.UDF5;
                    //            }
                    //            objDTO.Status = "Success";
                    //            objDTO.Reason = "N/A";
                    //            lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            objDTO.Status = "Fail";
                    //            objDTO.Reason = ex.Message.ToString();
                    //            lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                    //        }
                    //    }

                    //}
                    #endregion
                    #region Category Master
                    if (tableName == ImportMastersDTO.TableName.CategoryMaster.ToString())
                    {
                        List<string> p = (from d in context.CategoryMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.Category).ToList();
                        lstFinalAdd = (from m in (List<CategoryMasterMain>)list
                                       where (!p.Contains(m.Category)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<CategoryMasterMain>)list
                                   where (p.Contains(m.Category)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        lstreturnFinal = (from m in (List<CategoryMasterMain>)list
                                          where m.Category == ""
                                          select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<CategoryMasterMain>)list select d.Category).ToList();
                        List<CategoryMaster> lstCategorymaster = (from m in context.CategoryMasters
                                                                  where (Q.Contains(m.Category)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                  select m).ToList();

                        foreach (CategoryMasterMain objDTO in lstEdit.Cast<CategoryMasterMain>().ToList())
                        {
                            try
                            {

                                CategoryMaster obj = (from m in lstCategorymaster
                                                      where m.Category == objDTO.Category && m.Room == RoomId && m.CompanyID == CompanyId
                                                      select m).SingleOrDefault();

                                obj.Category = objDTO.Category;
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = objDTO.Created;
                                //obj.CreatedBy = objDTO.CreatedBy;
                                obj.GUID = objDTO.GUID;
                                obj.IsDeleted = (bool)objDTO.IsDeleted;
                                obj.IsArchived = (bool)objDTO.IsArchived;
                                obj.CompanyID = objDTO.CompanyID;
                                obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                obj.EditedFrom = "Web";

                                obj.CategoryColor = objDTO.CategoryColor;
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Customer Master
                    else if (tableName == ImportMastersDTO.TableName.CustomerMaster.ToString())
                    {
                        List<string> p = (from d in context.CustomerMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.Customer).ToList();
                        lstFinalAdd = (from m in (List<CustomerMasterMain>)list
                                       where (!p.Contains(m.Customer)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<CustomerMasterMain>)list
                                   where (p.Contains(m.Customer)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<CategoryMasterMain>)list
                        //                  where m.Category == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<CustomerMasterMain>)list select d.Customer).ToList();
                        List<CustomerMaster> lstCustomerMaster = (from m in context.CustomerMasters
                                                                  where (Q.Contains(m.Customer)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                  select m).ToList();

                        foreach (CustomerMasterMain objDTO in lstEdit.Cast<CustomerMasterMain>().ToList())
                        {
                            try
                            {
                                CustomerMaster obj = (from m in lstCustomerMaster
                                                      where m.Customer == objDTO.Customer && m.Room == RoomId && m.CompanyID == CompanyId
                                                      select m).SingleOrDefault();

                                if (arrcolumns.Contains("customer"))
                                {
                                    obj.Customer = objDTO.Customer;
                                }
                                if (arrcolumns.Contains("account"))
                                {
                                    obj.Account = objDTO.Account;
                                }
                                if (arrcolumns.Contains("contact"))
                                {
                                    obj.Contact = objDTO.Contact;
                                }
                                if (arrcolumns.Contains("address"))
                                {
                                    obj.Address = objDTO.Address;
                                }
                                if (arrcolumns.Contains("city"))
                                {
                                    obj.City = objDTO.City;
                                }
                                if (arrcolumns.Contains("state"))
                                {
                                    obj.State = objDTO.State;
                                }
                                if (arrcolumns.Contains("zipcode"))
                                {
                                    obj.ZipCode = objDTO.ZipCode;
                                }
                                if (arrcolumns.Contains("country"))
                                {
                                    obj.Country = objDTO.Country;
                                }
                                if (arrcolumns.Contains("phone"))
                                {
                                    obj.Phone = objDTO.Phone;
                                }
                                if (arrcolumns.Contains("email"))
                                {
                                    obj.Email = objDTO.Email;
                                }

                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                // obj.Created = objDTO.Created;
                                //  obj.CreatedBy = objDTO.CreatedBy;
                                // obj.GUID = objDTO.GUID;
                                // obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                // obj.CompanyID = objDTO.CompanyID;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }

                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;

                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Freight Type Master
                    if (tableName == ImportMastersDTO.TableName.FreightTypeMaster.ToString())
                    {
                        List<string> p = (from d in context.FreightTypeMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.FreightType).ToList();
                        lstFinalAdd = (from m in (List<FreightTypeMasterMain>)list
                                       where (!p.Contains(m.FreightType)) //&& m.BinNumber != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<FreightTypeMasterMain>)list
                                   where (p.Contains(m.FreightType)) //&& m.BinNumber != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<BinMasterMain>)list
                        //                  where m.BinNumber == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<FreightTypeMasterMain>)list select d.FreightType).ToList();
                        List<FreightTypeMaster> lstFreightType = (from m in context.FreightTypeMasters
                                                                  where (Q.Contains(m.FreightType)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                  select m).ToList();


                        foreach (FreightTypeMasterMain objDTO in lstEdit.Cast<FreightTypeMasterMain>().ToList())
                        {
                            try
                            {
                                FreightTypeMaster obj = (from m in lstFreightType
                                                         where m.FreightType == objDTO.FreightType && m.Room == RoomId && m.CompanyID == CompanyId
                                                         select m).SingleOrDefault();

                                if (arrcolumns.Contains("freighttype"))
                                {
                                    obj.FreightType = objDTO.FreightType;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                // obj.Room = objDTO.Room;
                                obj.LastUpdated = objDTO.LastUpdated;
                                //obj.Created = objDTO.Created;
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                // obj.CompanyID = objDTO.CompanyID;
                                // obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }

                    }
                    #endregion
                    #region G/L Account Master
                    else if (tableName == ImportMastersDTO.TableName.GLAccountMaster.ToString())
                    {
                        List<string> p = (from d in context.GLAccountMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.GLAccount).ToList();
                        lstFinalAdd = (from m in (List<GLAccountMasterMain>)list
                                       where (!p.Contains(m.GLAccount)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<GLAccountMasterMain>)list
                                   where (p.Contains(m.GLAccount)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<CategoryMasterMain>)list
                        //                  where m.Category == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<GLAccountMasterMain>)list select d.GLAccount).ToList();
                        List<GLAccountMaster> lstGLAccountMaster = (from m in context.GLAccountMasters
                                                                    where (Q.Contains(m.GLAccount)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                    select m).ToList();

                        foreach (GLAccountMasterMain objDTO in lstEdit.Cast<GLAccountMasterMain>().ToList())
                        {
                            try
                            {
                                GLAccountMaster obj = (from m in lstGLAccountMaster
                                                       where m.GLAccount == objDTO.GLAccount && m.Room == RoomId && m.CompanyID == CompanyId
                                                       select m).SingleOrDefault();


                                if (arrcolumns.Contains("glaccount"))
                                {
                                    obj.GLAccount = objDTO.GLAccount;
                                }
                                if (arrcolumns.Contains("description"))
                                {
                                    obj.Description = objDTO.Description;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                // obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = objDTO.Created;
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region GXPR Consigned Job Master
                    else if (tableName == ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString())
                    {
                        List<string> p = (from d in context.GXPRConsigmentJobMasters where d.Room == RoomId && d.CompanyID == CompanyId select d.GXPRConsigmentJob).ToList();
                        lstFinalAdd = (from m in (List<GXPRConsignedMasterMain>)list
                                       where (!p.Contains(m.GXPRConsigmentJob)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<GXPRConsignedMasterMain>)list
                                   where (p.Contains(m.GXPRConsigmentJob)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<CategoryMasterMain>)list
                        //                  where m.Category == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<GXPRConsignedMasterMain>)list select d.GXPRConsigmentJob).ToList();
                        List<GXPRConsigmentJobMaster> lstGXPRConsigmentJobMasters = (from m in context.GXPRConsigmentJobMasters
                                                                                     where (Q.Contains(m.GXPRConsigmentJob)) && m.Room == RoomId && m.CompanyID == CompanyId
                                                                                     select m).ToList();

                        foreach (GXPRConsignedMasterMain objDTO in lstEdit.Cast<GXPRConsignedMasterMain>().ToList())
                        {
                            GXPRConsigmentJobMaster obj = (from m in lstGXPRConsigmentJobMasters
                                                           where m.GXPRConsigmentJob == objDTO.GXPRConsigmentJob && m.Room == RoomId && m.CompanyID == CompanyId
                                                           select m).SingleOrDefault();

                            obj.GXPRConsigmentJob = objDTO.GXPRConsigmentJob;
                            obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                            obj.Room = objDTO.Room;
                            obj.Updated = objDTO.Updated;
                            obj.Created = objDTO.Created;
                            obj.CreatedBy = objDTO.CreatedBy;
                            obj.GUID = objDTO.GUID;
                            obj.IsDeleted = (bool)objDTO.IsDeleted;
                            obj.IsArchived = (bool)objDTO.IsArchived;
                            obj.CompanyID = objDTO.CompanyID;
                            obj.Room = objDTO.Room;
                            obj.UDF1 = objDTO.UDF1;
                            obj.UDF2 = objDTO.UDF2;
                            obj.UDF3 = objDTO.UDF3;
                            obj.UDF4 = objDTO.UDF4;
                            obj.UDF5 = objDTO.UDF5;

                        }
                    }
                    #endregion
                    #region Job Type Master
                    else if (tableName == ImportMastersDTO.TableName.JobTypeMaster.ToString())
                    {
                        List<string> p = (from d in context.JobTypeMasters where d.Room == RoomId && d.CompanyID == CompanyId select d.JobType).ToList();
                        lstFinalAdd = (from m in (List<JobTypeMasterMain>)list
                                       where (!p.Contains(m.JobType)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<JobTypeMasterMain>)list
                                   where (p.Contains(m.JobType)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<CategoryMasterMain>)list
                        //                  where m.Category == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<JobTypeMasterMain>)list select d.JobType).ToList();
                        List<JobTypeMaster> lstJobTypeMasters = (from m in context.JobTypeMasters
                                                                 where (Q.Contains(m.JobType)) && m.Room == RoomId && m.CompanyID == CompanyId
                                                                 select m).ToList();

                        foreach (JobTypeMasterMain objDTO in lstEdit.Cast<JobTypeMasterMain>().ToList())
                        {
                            JobTypeMaster obj = (from m in lstJobTypeMasters
                                                 where m.JobType == objDTO.JobType
                                                 select m).SingleOrDefault();

                            obj.JobType = objDTO.JobType;
                            obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                            obj.Room = objDTO.Room;
                            obj.LastUpdated = objDTO.LastUpdated;
                            obj.Created = Convert.ToDateTime(objDTO.Created);
                            obj.CreatedBy = objDTO.CreatedBy;
                            obj.GUID = objDTO.GUID;
                            obj.IsDeleted = (bool)objDTO.IsDeleted;
                            obj.IsArchived = (bool)objDTO.IsArchived;
                            obj.CompanyID = objDTO.CompanyID;
                            obj.Room = objDTO.Room;
                            obj.UDF1 = objDTO.UDF1;
                            obj.UDF2 = objDTO.UDF2;
                            obj.UDF3 = objDTO.UDF3;
                            obj.UDF4 = objDTO.UDF4;
                            obj.UDF5 = objDTO.UDF5;

                        }
                    }
                    #endregion
                    #region Ship Via Master
                    else if (tableName == ImportMastersDTO.TableName.ShipViaMaster.ToString())
                    {
                        List<string> p = (from d in context.ShipViaMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.ShipVia).ToList();
                        lstFinalAdd = (from m in (List<ShipViaMasterMain>)list
                                       where (!p.Contains(m.ShipVia)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<ShipViaMasterMain>)list
                                   where (p.Contains(m.ShipVia)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<ShipViaMasterMain>)list select d.ShipVia).ToList();
                        List<ShipViaMaster> lstShipViaMasters = (from m in context.ShipViaMasters
                                                                 where (Q.Contains(m.ShipVia)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                 select m).ToList();

                        foreach (ShipViaMasterMain objDTO in lstEdit.Cast<ShipViaMasterMain>().ToList())
                        {
                            try
                            {
                                ShipViaMaster obj = (from m in lstShipViaMasters
                                                     where m.ShipVia == objDTO.ShipVia
                                                     select m).SingleOrDefault();


                                if (arrcolumns.Contains("shipvia"))
                                {
                                    obj.ShipVia = objDTO.ShipVia;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = Convert.ToDateTime(objDTO.Created);
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                obj.EditedFrom = "Web";
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Technician Master
                    else if (tableName == ImportMastersDTO.TableName.TechnicianMaster.ToString())
                    {
                        var technicianMasterDAL = new TechnicialMasterDAL(base.DataBaseName);
                        List<string> p = technicianMasterDAL.GetTechnicianCodesByRoom(RoomId, CompanyId);

                        lstFinalAdd = (from m in (List<TechnicianMasterMain>)list
                                       where (!p.Contains(m.TechnicianCode.ToLower())) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<TechnicianMasterMain>)list
                                   where (p.Contains(m.TechnicianCode.ToLower())) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<TechnicianMasterMain>)list select d.TechnicianCode.ToLower()).ToList();
                        List<TechnicianMaster> lstTechnicianMaster = (from m in context.TechnicianMasters
                                                                      where (Q.Contains(m.TechnicianCode.ToLower())) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                      select m).ToList();

                        foreach (TechnicianMasterMain objDTO in lstEdit.Cast<TechnicianMasterMain>().ToList())
                        {
                            try
                            {
                                TechnicianMaster obj = (from m in lstTechnicianMaster
                                                        where m.TechnicianCode.ToLower() == objDTO.TechnicianCode.ToLower() && m.Room == RoomId && m.CompanyID == CompanyId
                                                        select m).SingleOrDefault();


                                if (arrcolumns.Contains("technician"))
                                {
                                    obj.Technician = objDTO.Technician;
                                }
                                if (arrcolumns.Contains("techniciancode"))
                                {
                                    obj.TechnicianCode = objDTO.TechnicianCode.Replace(" ", "");
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = Convert.ToDateTime(objDTO.Created);
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Manufacturer Master
                    else if (tableName == ImportMastersDTO.TableName.ManufacturerMaster.ToString())
                    {
                        List<string> p = (from d in context.ManufacturerMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.Manufacturer).ToList();
                        lstFinalAdd = (from m in (List<ManufacturerMasterMain>)list
                                       where (!p.Contains(m.Manufacturer)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<ManufacturerMasterMain>)list
                                   where (p.Contains(m.Manufacturer)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<ManufacturerMasterMain>)list select d.Manufacturer).ToList();
                        List<ManufacturerMaster> lstManufacturerMaster = (from m in context.ManufacturerMasters
                                                                          where (Q.Contains(m.Manufacturer)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                          select m).ToList();

                        foreach (ManufacturerMasterMain objDTO in lstEdit.Cast<ManufacturerMasterMain>().ToList())
                        {
                            try
                            {
                                ManufacturerMaster obj = (from m in lstManufacturerMaster
                                                          where m.Manufacturer == objDTO.Manufacturer && m.Room == RoomId && m.CompanyID == CompanyId
                                                          select m).SingleOrDefault();


                                if (arrcolumns.Contains("manufacturer"))
                                {
                                    obj.Manufacturer = objDTO.Manufacturer;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = Convert.ToDateTime(objDTO.Created);
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region MeasurementTerm Master
                    else if (tableName == ImportMastersDTO.TableName.MeasurementTermMaster.ToString())
                    {
                        List<string> p = (from d in context.MeasurementTermMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.MeasurementTerm).ToList();
                        lstFinalAdd = (from m in (List<MeasurementTermMasterMain>)list
                                       where (!p.Contains(m.MeasurementTerm)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<MeasurementTermMasterMain>)list
                                   where (p.Contains(m.MeasurementTerm)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<MeasurementTermMasterMain>)list select d.MeasurementTerm).ToList();
                        List<MeasurementTermMaster> lstMeasurementTermMaster = (from m in context.MeasurementTermMasters
                                                                                where (Q.Contains(m.MeasurementTerm)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                                select m).ToList();

                        foreach (MeasurementTermMasterMain objDTO in lstEdit.Cast<MeasurementTermMasterMain>().ToList())
                        {
                            try
                            {
                                MeasurementTermMaster obj = (from m in lstMeasurementTermMaster
                                                             where m.MeasurementTerm == objDTO.MeasurementTerm && m.Room == RoomId && m.CompanyID == CompanyId
                                                             select m).SingleOrDefault();


                                if (arrcolumns.Contains("measurementterm"))
                                {
                                    obj.MeasurementTerm = objDTO.MeasurementTerm;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = Convert.ToDateTime(objDTO.Created);
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Units Master
                    else if (tableName == ImportMastersDTO.TableName.UnitMaster.ToString())
                    {
                        List<string> p = (from d in context.UnitMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.Unit).ToList();
                        lstFinalAdd = (from m in (List<UnitMasterMain>)list
                                       where (!p.Contains(m.Unit)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<UnitMasterMain>)list
                                   where (p.Contains(m.Unit)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<UnitMasterMain>)list select d.Unit).ToList();
                        List<UnitMaster> lstUnitMaster = (from m in context.UnitMasters
                                                          where (Q.Contains(m.Unit)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                          select m).ToList();

                        foreach (UnitMasterMain objDTO in lstEdit.Cast<UnitMasterMain>().ToList())
                        {
                            try
                            {
                                UnitMaster obj = (from m in lstUnitMaster
                                                  where m.Unit == objDTO.Unit && m.Room == RoomId && m.CompanyID == CompanyId
                                                  select m).SingleOrDefault();


                                if (arrcolumns.Contains("unit"))
                                {
                                    obj.Unit = objDTO.Unit;
                                }
                                if (arrcolumns.Contains("description"))
                                {
                                    obj.Description = objDTO.Description;
                                }
                                if (arrcolumns.Contains("odometer"))
                                {
                                    obj.Odometer = objDTO.Odometer;
                                }
                                if (arrcolumns.Contains("odometerupdate"))
                                {
                                    obj.OdometerUpdate = objDTO.OdometerUpdate;
                                }
                                if (arrcolumns.Contains("ophours"))
                                {
                                    obj.OpHours = objDTO.OpHours;
                                }
                                if (arrcolumns.Contains("ophoursupdate"))
                                {
                                    obj.OpHoursUpdate = objDTO.OpHoursUpdate;
                                }
                                if (arrcolumns.Contains("serialno"))
                                {
                                    obj.SerialNo = objDTO.SerialNo;
                                }
                                if (arrcolumns.Contains("year"))
                                {
                                    obj.Year = objDTO.Year;
                                }
                                if (arrcolumns.Contains("make"))
                                {
                                    obj.Make = objDTO.Make;
                                }
                                if (arrcolumns.Contains("model"))
                                {
                                    obj.Model = objDTO.Model;
                                }
                                if (arrcolumns.Contains("plate"))
                                {
                                    obj.Plate = objDTO.Plate;
                                }
                                if (arrcolumns.Contains("enginemodel"))
                                {
                                    obj.EngineModel = objDTO.EngineModel;
                                }
                                if (arrcolumns.Contains("engineserialno"))
                                {
                                    obj.EngineSerialNo = objDTO.EngineSerialNo;
                                }
                                if (arrcolumns.Contains("markupparts"))
                                {
                                    obj.MarkupParts = objDTO.MarkupParts;
                                }
                                if (arrcolumns.Contains("markuplabour"))
                                {
                                    obj.MarkupLabour = objDTO.MarkupLabour;
                                }

                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = Convert.ToDateTime(objDTO.Created);
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;

                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Supplier Master
                    else if (tableName == ImportMastersDTO.TableName.SupplierMaster.ToString())
                    {
                        List<string> p = (from d in context.SupplierMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.SupplierName).ToList();
                        lstFinalAdd = (from m in (List<SupplierMasterMain>)list
                                       where (!p.Contains(m.SupplierName)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<SupplierMasterMain>)list
                                   where (p.Contains(m.SupplierName)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<SupplierMasterMain>)list select d.SupplierName).ToList();
                        List<SupplierMaster> lstSupplierMaster = (from m in context.SupplierMasters
                                                                  where (Q.Contains(m.SupplierName)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                  select m).ToList();

                        foreach (SupplierMasterMain objDTO in lstEdit.Cast<SupplierMasterMain>().ToList())
                        {
                            try
                            {
                                SupplierMaster obj = (from m in lstSupplierMaster
                                                      where m.SupplierName == objDTO.SupplierName && m.Room == RoomId && m.CompanyID == CompanyId
                                                      select m).SingleOrDefault();


                                if (arrcolumns.Contains("suppliername"))
                                {
                                    obj.SupplierName = objDTO.SupplierName;
                                }
                                if (arrcolumns.Contains("description"))
                                {
                                    obj.Description = objDTO.Description;
                                }
                                if (arrcolumns.Contains("address"))
                                {
                                    obj.Address = objDTO.Address;
                                }
                                if (arrcolumns.Contains("city"))
                                {
                                    obj.City = objDTO.City;
                                }
                                if (arrcolumns.Contains("state"))
                                {
                                    obj.State = objDTO.State;
                                }
                                if (arrcolumns.Contains("zipcode"))
                                {
                                    obj.ZipCode = objDTO.ZipCode;
                                }
                                if (arrcolumns.Contains("country"))
                                {
                                    obj.Country = objDTO.Country;
                                }
                                if (arrcolumns.Contains("contact"))
                                {
                                    obj.Contact = objDTO.Contact;
                                }
                                if (arrcolumns.Contains("phone"))
                                {
                                    obj.Phone = objDTO.Phone;
                                }
                                if (arrcolumns.Contains("fax"))
                                {
                                    obj.Fax = objDTO.Fax;
                                }

                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.LastUpdated = objDTO.LastUpdated;
                                //obj.Created = Convert.ToDateTime(objDTO.Created);
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Item Master
                    else if (tableName == ImportMastersDTO.TableName.ItemMaster.ToString())
                    {
                        //var allNonDeletedItemsInRoom = context.ItemMasters.Where(d=> d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false).ToList();
                        //List<string> p = (from d in context.ItemMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select (d.ItemNumber ?? string.Empty).Trim().ToLower()).ToList();
                        //List<string> p = (from d in allNonDeletedItemsInRoom select (d.ItemNumber ?? string.Empty).Trim().ToLower()).ToList();
                        List<string> p = (from d in context.ItemMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select (d.ItemNumber ?? string.Empty).Trim().ToLower()).ToList();

                        lstFinalAdd = (from m in (List<BOMItemMasterMain>)list
                                       where (!p.Contains((m.ItemNumber ?? string.Empty).Trim().ToLower())) && m.IsDeleted == false //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        //var tmplstFinalAdd = (from TIM in (List<BOMItemMasterMain>)list
                        //                      join itm in allNonDeletedItemsInRoom on (TIM.ItemNumber ?? string.Empty).Trim().ToLower() equals (itm.ItemNumber ?? string.Empty).Trim().ToLower() 
                        //                      where TIM.ItemType != itm.ItemType && TIM.IsDeleted == false 
                        //                      select TIM).Cast<T>().ToList();

                        //if (tmplstFinalAdd != null && tmplstFinalAdd.Any() && tmplstFinalAdd.Count() > 0)
                        //{
                        //    lstFinalAdd.AddRange(tmplstFinalAdd);
                        //}

                        lstFinalDelete = (from m in (List<BOMItemMasterMain>)list
                                          where (p.Contains((m.ItemNumber ?? string.Empty).Trim().ToLower())) && m.IsDeleted == true
                                          select m).Cast<T>().ToList();

                        CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
                        string columnList = "ID,RoomName,MethodOfValuingInventory";
                        RoomDTO oRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");
                        string cultureCode = "en-US";
                        var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomId, CompanyId, -1);
                        
                        if (regionInfo != null)
                        {
                            cultureCode = regionInfo.CultureCode;
                        }

                        foreach (BOMItemMasterMain objDTO in lstFinalAdd.Cast<BOMItemMasterMain>().ToList())
                        {
                            if ((!arrcolumns.Contains("ispurchase")) && (!arrcolumns.Contains("istransfer")))
                            {
                                objDTO.IsPurchase = true;
                            }

                            //objDTO.IsActive = true;
                            if (string.IsNullOrEmpty(objDTO.ItemUniqueNumber) || string.IsNullOrWhiteSpace(objDTO.ItemUniqueNumber))
                            {
                                //objDTO.ItemUniqueNumber = objCommonDAL.UniqueItemId();
                                objDTO.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                            }

                            if (oRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                && !objDTO.Consignment
                                && (objDTO.OnHandQuantity == null || objDTO.OnHandQuantity <= 0))
                            {
                                //objDTO.Cost = 0;
                                //objDTO.Markup = 0;
                                //objDTO.SellPrice = 0;
                            }
                            if (objDTO.ItemType == 4)
                            {
                                //objDTO.Cost = 0;
                                objDTO.OnHandQuantity = 0;
                                //objDTO.SellPrice = 0;
                            }

                            if (((!objDTO.IsTransfer) && (!objDTO.IsPurchase)))
                            {
                                objDTO.IsTransfer = false;
                                objDTO.IsPurchase = true;
                            }
                            /*----------CODE FOR SET IMAGEPATH AND LINK2 NULL IF IMAGE AND LINK2 ZIPFILE NOT UPLOAD--------------*/
                            if (!isImgZipAvail)
                            {
                                objDTO.ImagePath = string.Empty;
                                objDTO.ImageType = "ExternalImage";
                            }
                            if (!isLink2ZipAvail)
                            {
                                objDTO.Link2 = string.Empty;
                                objDTO.ItemLink2ImageType = "InternalLink";
                            }
                            /*----------CODE FOR SET IMAGEPATH AND LINK2 NULL IF IMAGE AND LINK2 ZIPFILE NOT UPLOAD--------------*/
                        }
                        lstEdit = (from m in (List<BOMItemMasterMain>)list
                                   where (p.Contains((m.ItemNumber ?? string.Empty).Trim().ToLower())) && m.IsDeleted == false //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        //lstEdit = (from TIM in (List<BOMItemMasterMain>)list
                        //           join itm in allNonDeletedItemsInRoom on new { TIM.ItemNumber, TIM.ItemType } equals new { itm.ItemNumber, itm.ItemType }
                        //           where TIM.ItemType == itm.ItemType && TIM.IsDeleted == false && (TIM.ItemNumber ?? string.Empty).Trim().ToLower() == (itm.ItemNumber ?? string.Empty).Trim().ToLower()
                        //           select TIM).Cast<T>().ToList();

                        List<string> Q = (from d in (List<BOMItemMasterMain>)list select (d.ItemNumber ?? string.Empty).Trim().ToLower()).ToList();
                        List<ItemMaster> lstItemMaster = (from m in context.ItemMasters
                                                          where (Q.Contains((m.ItemNumber ?? string.Empty).Trim().ToLower())) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                          select m).ToList();

                        var itemMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, EnterpriseID, CompanyId);
                        string msgCantChangeConsignedToCustomer = ResourceRead.GetResourceValueByKeyAndFullFilePath("CantChangeConsignedToCustomer", itemMasterResourceFilePath, EnterpriseID, CompanyId, RoomId, "ResItemMaster", cultureCode);

                        foreach (BOMItemMasterMain objDTO in lstEdit.Cast<BOMItemMasterMain>().ToList())
                        {
                            try
                            {
                                bool IsValidItem = true;

                                //ItemMaster obj = (from m in allNonDeletedItemsInRoom
                                //                  where (m.ItemNumber ?? string.Empty).Trim().ToLower() == (objDTO.ItemNumber ?? string.Empty).Trim().ToLower() && 
                                //                        m.ItemType == objDTO.ItemType && m.Room == objDTO.Room
                                //                  select m).FirstOrDefault();

                                ItemMaster obj = (from m in lstItemMaster
                                                  where (m.ItemNumber ?? string.Empty).Trim().ToLower() == (objDTO.ItemNumber ?? string.Empty).Trim().ToLower() && m.Room == objDTO.Room
                                                  select m).FirstOrDefault();

                                if (obj.IsActive == true && objDTO.IsActive == false)
                                {
                                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                                    bool Res = true;
                                    string Result = objItemMasterDAL.CheckItemExistsForInActive(obj.GUID, RoomId, CompanyId, ref Res,EnterpriseID,cultureCode);
                                    if (!Res)
                                    {
                                        objDTO.Status = "Fail";
                                        objDTO.Reason = Result;
                                        lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                        IsValidItem = false;
                                    }
                                }
                                obj.IsActive = objDTO.IsActive;
                                obj.IsOrderable = objDTO.IsOrderable;
                                obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                                ItemMasterDAL objItemDal = new ItemMasterDAL(base.DataBaseName);
                                ItemMasterDTO ObjITEM2 = objItemDal.GetItemWithoutJoins(obj.ID, null);
                                objDTO.ID = ObjITEM2.ID;
                                objDTO.GUID = ObjITEM2.GUID;
                                objDTO.ReceivedOnWeb = ObjITEM2.Created;
                                // obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                                //obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;

                                if (objDTO.InventoryClassification == 0)
                                {
                                    objDTO.InventoryClassification = null;
                                }
                                objDTO.ItemUniqueNumber = obj.ItemUniqueNumber;
                                objDTO.WhatWhereAction = "Import";
                                if ((!objDTO.IsTransfer) && (!objDTO.IsPurchase))
                                {
                                    objDTO.IsPurchase = true;
                                    objDTO.IsTransfer = false;
                                }
                                if (oRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !objDTO.Consignment)
                                {
                                    //objDTO.Cost = ObjITEM2.Cost;
                                }
                                else if (oRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !objDTO.Consignment
                                    && objDTO.Cost != ObjITEM2.Cost && arrcolumns.Contains("cost"))
                                {
                                    //new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                                }

                                BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(base.DataBaseName);
                                List<BinMasterDTO> lstBinReplanish = objItemLocationLevelQuanityDAL.GetItemLocations(objDTO.GUID, (long)objDTO.Room, (long)objDTO.CompanyID);
                                lstBinReplanish = lstBinReplanish.Where(t => t.IsStagingLocation == false && t.ConsignedQuantity != null && t.ConsignedQuantity > 0).ToList();

                                if (lstBinReplanish != null && lstBinReplanish.Count > 0 && obj.Consignment == true && objDTO.Consignment == false)
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason += " " + msgCantChangeConsignedToCustomer; 
                                    lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                }
                                else
                                {
                                    if (IsValidItem)
                                    {
                                        EditItemMaster(objDTO, arrcolumns, SessionUserId, isImgZipAvail, isLink2ZipAvail);
                                        if (objDTO.IsAllowOrderCostuom && objDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                            objItemLocationLevelQuanityDAL.UpdateItemBinOrderUOM(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false);
                                        objDTO.Status = "Success";
                                        objDTO.Reason = "N/A";
                                        lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                        }

                        bool DeleteItems = false;
                        foreach (BOMItemMasterMain objDTO in lstFinalDelete.Cast<BOMItemMasterMain>().ToList())
                        {
                            try
                            {
                                DeleteItems = true;
                                //ItemMaster obj = (from m in allNonDeletedItemsInRoom
                                //                  where (m.ItemNumber ?? string.Empty).Trim().ToLower() == (objDTO.ItemNumber ?? string.Empty).Trim().ToLower()
                                //                        && m.ItemType == objDTO.ItemType && m.Room == objDTO.Room
                                //                  select m).FirstOrDefault();
                                ItemMaster obj = (from m in lstItemMaster
                                                  where m.ItemNumber == objDTO.ItemNumber && m.Room == objDTO.Room
                                                  select m).FirstOrDefault();
                                if (obj == null)
                                {
                                    obj = new ItemMaster();
                                }
                                string response = string.Empty;
                                ModuleDeleteDTO objModuleDeleteDTO = new ModuleDeleteDTO();
                                objModuleDeleteDTO = objCommonDAL.DeleteModulewise(obj.GUID.ToString(), ImportMastersDTO.TableName.ItemMaster.ToString(), true, UserId, true,EnterpriseID,CompanyId,RoomId);
                                response = objModuleDeleteDTO.CommonMessage;

                                if (objModuleDeleteDTO != null && objModuleDeleteDTO.SuccessItems != null && objModuleDeleteDTO.SuccessItems.Count > 0)
                                {
                                    if ((from d in context.BinMasters where d.Room == RoomId && d.BinNumber == objDTO.InventryLocation && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false && d.ItemGUID == obj.GUID select d.ParentBinId).Any())
                                    {
                                        Int64 BinID = (from d in context.BinMasters where d.Room == RoomId && d.BinNumber == objDTO.InventryLocation && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false && d.ItemGUID == obj.GUID select d.ParentBinId).FirstOrDefault() ?? 0;

                                        if (!((from d in context.BinMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false && d.ItemGUID != obj.GUID && d.ParentBinId == BinID select d.ParentBinId).Any()))
                                        {
                                            Int64 BinCount = (from d in context.BinMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false && d.ItemGUID != obj.GUID && d.ParentBinId == BinID select d.ParentBinId).Count();
                                            if (BinCount == 0)
                                            {
                                                ModuleDeleteDTO objModuleDTO = new ModuleDeleteDTO();
                                                objModuleDTO = objCommonDAL.DeleteModulewise(BinID.ToString(), ImportMastersDTO.TableName.BinMaster.ToString(), false, UserId, true,EnterpriseID,CompanyId,RoomId);
                                            }
                                        }
                                    }

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
                    }
                    #endregion
                    #region Edit Item Master
                    else if (tableName == ImportMastersDTO.TableName.EditItemMaster.ToString())
                    {
                        List<string> p = (from d in context.ItemMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select (d.SupplierPartNo ?? string.Empty).Trim().ToLower()).ToList();

                        CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
                        string columnList = "ID,RoomName,MethodOfValuingInventory";
                        RoomDTO oRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");
                        string cultureCode = "en-US";
                        var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomId, CompanyId, -1);

                        if (regionInfo != null)
                        {
                            cultureCode = regionInfo.CultureCode;
                        }

                        lstEdit = (from m in (List<BOMItemMasterMain>)list
                                   where (p.Contains((m.SupplierPartNo ?? string.Empty).Trim().ToLower())) && m.IsDeleted == false //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<BOMItemMasterMain>)list select (d.SupplierPartNo ?? string.Empty).Trim().ToLower()).ToList();
                        List<ItemMaster> lstItemMaster = (from m in context.ItemMasters
                                                          where (Q.Contains((m.SupplierPartNo ?? string.Empty).Trim().ToLower())) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                          select m).ToList();

                        var itemMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", cultureCode, EnterpriseID, CompanyId);
                        string msgCantChangeConsignedToCustomer = ResourceRead.GetResourceValueByKeyAndFullFilePath("CantChangeConsignedToCustomer", itemMasterResourceFilePath, EnterpriseID, CompanyId, RoomId, "ResItemMaster", cultureCode);

                        foreach (BOMItemMasterMain objDTO in lstEdit.Cast<BOMItemMasterMain>().ToList())
                        {
                            try
                            {
                                bool IsValidItem = true;
                                List<ItemMaster> lstItemMasterBySN = (from m in lstItemMaster
                                                                  where (m.SupplierPartNo ?? string.Empty).Trim().ToLower() == (objDTO.SupplierPartNo ?? string.Empty).Trim().ToLower() && m.Room == objDTO.Room
                                                                  select m).ToList();

                                foreach (ItemMaster obj in lstItemMasterBySN)
                                {
                                    //ItemMaster obj = (from m in lstItemMaster
                                    //                  where (m.SupplierPartNo ?? string.Empty).Trim().ToLower() == (objDTO.SupplierPartNo ?? string.Empty).Trim().ToLower() && m.Room == objDTO.Room
                                    //                  select m).FirstOrDefault();

                                    if (obj.IsActive == true && objDTO.IsActive == false)
                                    {
                                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                                        bool Res = true;
                                        string Result = objItemMasterDAL.CheckItemExistsForInActive(obj.GUID, RoomId, CompanyId, ref Res, EnterpriseID, cultureCode);
                                        if (!Res)
                                        {
                                            objDTO.Status = "Fail";
                                            objDTO.Reason = Result;
                                            lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                            IsValidItem = false;
                                        }
                                    }
                                    obj.IsActive = objDTO.IsActive;
                                    obj.IsOrderable = objDTO.IsOrderable;
                                    obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                                    ItemMasterDAL objItemDal = new ItemMasterDAL(base.DataBaseName);
                                    ItemMasterDTO ObjITEM2 = objItemDal.GetItemWithoutJoins(obj.ID, null);
                                    objDTO.ID = ObjITEM2.ID;
                                    objDTO.GUID = ObjITEM2.GUID;
                                    objDTO.ReceivedOnWeb = ObjITEM2.Created;
                                    objDTO.ItemNumber = obj.ItemNumber;
                                    objDTO.DefaultLocation = (long)obj.DefaultLocation;
                                    if (objDTO.InventoryClassification == 0)
                                    {
                                        objDTO.InventoryClassification = null;
                                    }
                                    objDTO.ItemUniqueNumber = obj.ItemUniqueNumber;
                                    objDTO.WhatWhereAction = "Import";
                                    if ((!objDTO.IsTransfer) && (!objDTO.IsPurchase))
                                    {
                                        objDTO.IsPurchase = true;
                                        objDTO.IsTransfer = false;
                                    }
                                    if (oRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !objDTO.Consignment)
                                    {
                                        //objDTO.Cost = ObjITEM2.Cost;
                                    }
                                    else if (oRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !objDTO.Consignment
                                        && objDTO.Cost != ObjITEM2.Cost && arrcolumns.Contains("cost"))
                                    {
                                        //new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                                    }

                                    BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(base.DataBaseName);
                                    List<BinMasterDTO> lstBinReplanish = objItemLocationLevelQuanityDAL.GetItemLocations(objDTO.GUID, (long)objDTO.Room, (long)objDTO.CompanyID);
                                    lstBinReplanish = lstBinReplanish.Where(t => t.IsStagingLocation == false && t.ConsignedQuantity != null && t.ConsignedQuantity > 0).ToList();

                                    if (lstBinReplanish != null && lstBinReplanish.Count > 0 && obj.Consignment == true && objDTO.Consignment == false)
                                    {
                                        objDTO.Status = "Fail";
                                        objDTO.Reason += " " + msgCantChangeConsignedToCustomer;
                                        lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                    }
                                    else
                                    {
                                        if (IsValidItem)
                                        {
                                            EditItemMaster(objDTO, arrcolumns, SessionUserId, isImgZipAvail, isLink2ZipAvail);
                                            if (objDTO.IsAllowOrderCostuom && objDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                                objItemLocationLevelQuanityDAL.UpdateItemBinOrderUOM(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), false);
                                            objDTO.Status = "Success";
                                            objDTO.Reason = "N/A";
                                            lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                                        }
                                    }
                                }

                                
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                        }
                    }
                    #endregion
                    #region Location Master
                    if (tableName == ImportMastersDTO.TableName.LocationMaster.ToString())
                    {
                        List<string> p = (from d in context.LocationMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.Location).ToList();
                        lstFinalAdd = (from m in (List<LocationMasterMain>)list
                                       where (!p.Contains(m.Location)) //&& m.Location != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<LocationMasterMain>)list
                                   where (p.Contains(m.Location)) //&& m.Location != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<LocationMasterMain>)list
                        //                  where m.Location == ""
                        //                  select m).Cast<T>().ToList();




                        List<string> Q = (from d in (List<LocationMasterMain>)list select d.Location).ToList();
                        List<LocationMaster> lstLocationmaster = (from m in context.LocationMasters
                                                                  where (Q.Contains(m.Location)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                  select m).ToList();


                        foreach (LocationMasterMain objDTO in lstEdit.Cast<LocationMasterMain>().ToList())
                        {
                            try
                            {
                                LocationMaster obj = (from m in lstLocationmaster
                                                      where m.Location == objDTO.Location && m.Room == RoomId && m.CompanyID == CompanyId
                                                      select m).SingleOrDefault();


                                if (arrcolumns.Contains("location"))
                                {
                                    obj.Location = objDTO.Location;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                // obj.Room = objDTO.Room;
                                obj.LastUpdated = objDTO.LastUpdated;
                                //obj.Created = objDTO.Created;
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }

                    }
                    #endregion
                    #region ToolCategory Master
                    if (tableName == ImportMastersDTO.TableName.ToolCategoryMaster.ToString())
                    {
                        List<string> p = (from d in context.ToolCategoryMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.ToolCategory).ToList();
                        lstFinalAdd = (from m in (List<ToolCategoryMasterMain>)list
                                       where (!p.Contains(m.ToolCategory)) //&& m.ToolCategory != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<ToolCategoryMasterMain>)list
                                   where (p.Contains(m.ToolCategory)) //&& m.ToolCategory != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<ToolCategoryMasterMain>)list
                        //                  where m.ToolCategory == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<ToolCategoryMasterMain>)list select d.ToolCategory).ToList();
                        List<ToolCategoryMaster> lstToolCategorymaster = (from m in context.ToolCategoryMasters
                                                                          where (Q.Contains(m.ToolCategory)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                          select m).ToList();


                        foreach (ToolCategoryMasterMain objDTO in lstEdit.Cast<ToolCategoryMasterMain>().ToList())
                        {
                            try
                            {
                                ToolCategoryMaster obj = (from m in lstToolCategorymaster
                                                          where m.ToolCategory == objDTO.ToolCategory && m.Room == RoomId && m.CompanyID == CompanyId
                                                          select m).SingleOrDefault();


                                if (arrcolumns.Contains("toolcategory"))
                                {
                                    obj.ToolCategory = objDTO.ToolCategory;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = objDTO.Created;
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }

                    }
                    #endregion
                    #region CostUOM Master
                    else if (tableName == ImportMastersDTO.TableName.CostUOMMaster.ToString())
                    {
                        List<string> p = (from d in context.CostUOMMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.CostUOM).ToList();
                        lstFinalAdd = (from m in (List<CostUOMMasterMain>)list
                                       where (!p.Contains(m.CostUOM)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<CostUOMMasterMain>)list
                                   where (p.Contains(m.CostUOM)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<CategoryMasterMain>)list
                        //                  where m.Category == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<CostUOMMasterMain>)list select d.CostUOM).ToList();
                        List<CostUOMMaster> lstCostUOMMaster = (from m in context.CostUOMMasters
                                                                where (Q.Contains(m.CostUOM)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsDeleted == false && m.IsArchived == false
                                                                select m).ToList();

                        foreach (CostUOMMasterMain objDTO in lstEdit.Cast<CostUOMMasterMain>().ToList())
                        {
                            try
                            {
                                CostUOMMaster obj = (from m in lstCostUOMMaster
                                                     where m.CostUOM == objDTO.CostUOM && m.Room == RoomId && m.CompanyID == CompanyId
                                                     select m).SingleOrDefault();

                                if (arrcolumns.Contains("costuom"))
                                {
                                    obj.CostUOM = objDTO.CostUOM;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = objDTO.Created;
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                if (arrcolumns.Contains("costuomvalue"))
                                {
                                    obj.CostUOMValue = objDTO.CostUOMValue;
                                }
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                obj.EditedFrom = "Web";
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region InventoryClassification Master
                    else if (tableName == ImportMastersDTO.TableName.InventoryClassificationMaster.ToString())
                    {
                        List<string> p = (from d in context.InventoryClassificationMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.InventoryClassification).ToList();
                        lstFinalAdd = (from m in (List<InventoryClassificationMasterMain>)list
                                       where (!p.Contains(m.InventoryClassification)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<InventoryClassificationMasterMain>)list
                                   where (p.Contains(m.InventoryClassification)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();

                        //lstreturnFinal = (from m in (List<CategoryMasterMain>)list
                        //                  where m.Category == ""
                        //                  select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<InventoryClassificationMasterMain>)list select d.InventoryClassification).ToList();
                        List<InventoryClassificationMaster> lstCostUOMMaster = (from m in context.InventoryClassificationMasters
                                                                                where (Q.Contains(m.InventoryClassification)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                                                select m).ToList();

                        foreach (InventoryClassificationMasterMain objDTO in lstEdit.Cast<InventoryClassificationMasterMain>().ToList())
                        {
                            try
                            {
                                InventoryClassificationMaster obj = (from m in lstCostUOMMaster
                                                                     where m.InventoryClassification == objDTO.InventoryClassification && m.Room == RoomId && m.CompanyID == CompanyId
                                                                     select m).SingleOrDefault();


                                if (arrcolumns.Contains("inventoryclassification"))
                                {
                                    obj.InventoryClassification = objDTO.InventoryClassification;
                                }
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //obj.Room = objDTO.Room;
                                obj.Updated = objDTO.Updated;
                                //obj.Created = objDTO.Created;
                                //obj.CreatedBy = objDTO.CreatedBy;
                                //obj.GUID = objDTO.GUID;
                                //obj.IsDeleted = (bool)objDTO.IsDeleted;
                                //obj.IsArchived = (bool)objDTO.IsArchived;
                                //obj.CompanyID = objDTO.CompanyID;
                                //obj.Room = objDTO.Room;

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                if (arrcolumns.Contains("baseofinventory"))
                                {
                                    obj.BaseOfInventory = objDTO.BaseOfInventory;
                                }
                                if (arrcolumns.Contains("rangestart"))
                                {
                                    obj.RangeStart = objDTO.RangeStart;
                                }
                                if (arrcolumns.Contains("rangeend"))
                                {
                                    obj.RangeEnd = objDTO.RangeEnd;
                                }
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                obj.EditedFrom = "Web";
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Tool Master
                    else if (tableName == ImportMastersDTO.TableName.ToolMaster.ToString())
                    {
                        List<ToolMaster> p = (from d in context.ToolMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d).ToList();


                        lstFinalAdd = ((List<ToolMasterMain>)list).Where(a => !p.Select(b => new { b.ToolName, b.Serial }).Contains(new { a.ToolName, a.Serial })).Cast<T>().ToList();
                        lstEdit = ((List<ToolMasterMain>)list).Where(a => p.Select(b => new { b.ToolName, b.Serial }).Contains(new { a.ToolName, a.Serial })).Cast<T>().ToList();


                        List<string> Q = (from d in (List<ToolMasterMain>)list select d.ToolName).ToList();
                        List<ToolMaster> lstCostUOMMaster = (from m in context.ToolMasters
                                                             where (Q.Contains(m.ToolName)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                             select m).ToList();


                        foreach (ToolMasterMain objDTO in lstFinalAdd.Cast<ToolMasterMain>().ToList())
                        {
                            if (objDTO.SerialNumberTracking)
                            {
                                objDTO.IsGroupOfItems = 1;
                            }
                            else
                            {
                                objDTO.IsGroupOfItems = (objDTO.IsGroupOfItems == 0 && objDTO.Quantity > 1) ? 1 : objDTO.IsGroupOfItems;
                            }

                            if (!isImgZipAvail)
                            {
                                objDTO.ImagePath = string.Empty;
                                objDTO.ImageType = "ExternalImage";

                            }
                        }


                        #region  Edit Tool Master


                        foreach (ToolMasterMain objDTO in lstEdit.Cast<ToolMasterMain>().ToList())
                        {
                            try
                            {
                                ToolMaster obj = (from m in lstCostUOMMaster
                                                  where m.ToolName == objDTO.ToolName && m.Room == RoomId && m.CompanyID == CompanyId && (m.Serial ?? string.Empty) == (objDTO.Serial ?? string.Empty)
                                                  && m.IsDeleted == false && m.IsArchived == false
                                                  select m).SingleOrDefault();


                                if (arrcolumns.Contains("toolname"))
                                {
                                    obj.ToolName = objDTO.ToolName;
                                }


                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                if (arrcolumns.Contains("serial"))
                                {
                                    obj.Serial = objDTO.Serial;
                                }
                                if (arrcolumns.Contains("description"))
                                {
                                    obj.Description = objDTO.Description;
                                }
                                if (arrcolumns.Contains("toolcategory"))
                                {
                                    obj.ToolCategoryID = objDTO.ToolCategoryID;
                                }

                                if (arrcolumns.Contains("cost"))
                                {
                                    obj.Cost = objDTO.Cost;
                                }


                                if (arrcolumns.Contains("quantity"))
                                {
                                    if (obj.SerialNumberTracking == false)
                                    {
                                        if (obj.IsGroupOfItems == 0)
                                        {
                                            obj.Quantity = 1;
                                        }
                                        else
                                        {
                                            if ((Convert.ToInt32(obj.CheckedOutMQTY) + Convert.ToInt32(obj.CheckedOutQTY)) < objDTO.Quantity)
                                                obj.Quantity = objDTO.Quantity;
                                            else
                                                obj.Quantity = (Convert.ToInt32(obj.CheckedOutMQTY) + Convert.ToInt32(obj.CheckedOutQTY));
                                        }
                                    }
                                }

                                if (arrcolumns.Contains("isgroupofitems"))
                                {
                                    // obj.IsGroupOfItems = objDTO.IsGroupOfItems;
                                }

                                if (arrcolumns.Contains("toolimageexternalurl"))
                                {
                                    obj.ToolImageExternalURL = objDTO.ToolImageExternalURL;
                                }

                                obj.ImageType = objDTO.ImageType;

                                if (arrcolumns.Contains("imagepath"))
                                {

                                    if (string.IsNullOrEmpty(objDTO.ImagePath))
                                    {
                                        obj.ImagePath = string.Empty;
                                        obj.ImageType = "ExternalImage";
                                    }
                                    else
                                    {
                                        obj.ImagePath = objDTO.ImagePath;
                                        obj.ImageType = objDTO.ImageType;
                                    }

                                }
                                bool isQtyChanged = false;
                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Updated = objDTO.Updated;
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow; ;

                                if (_AllowToolOrdering == true)
                                {

                                    ToolLocationDetailsDTO objToolLocationDetailsDTO = null;
                                    ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                                    LocationMasterDTO objLocationMasterDTO = null;
                                    if (arrcolumns.Contains("location"))
                                    {
                                        if (obj.LocationID != objDTO.LocationID)
                                        {
                                            obj.LocationID = objDTO.LocationID;
                                            isQtyChanged = true;


                                            if (objDTO.LocationID.GetValueOrDefault(0) <= 0)
                                            {
                                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(obj.GUID, string.Empty, obj.Room ?? 0, obj.CompanyID ?? 0, UserId, "ImportController>>Saveimport", true);
                                            }
                                            else
                                            {
                                                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(base.DataBaseName);
                                                objLocationMasterDTO = objLocationCntrl.GetLocationByIDPlain(obj.LocationID.GetValueOrDefault(0), obj.Room ?? 0, obj.CompanyID ?? 0);
                                                //objLocationMasterDTO = lstLocation.Where(i => i.ID == obj.LocationID).FirstOrDefault();
                                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(obj.GUID, objLocationMasterDTO.Location, obj.Room ?? 0, obj.CompanyID ?? 0, UserId, "ImportController>>Saveimport", true);

                                            }

                                        }

                                    }

                                    if (!obj.SerialNumberTracking && isQtyChanged)
                                    {
                                        ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);


                                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);
                                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                        objToolAssetQuantityDetailDTO.ToolGUID = obj.GUID;

                                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                                        objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO.ID;
                                        objToolAssetQuantityDetailDTO.Quantity = obj.Quantity ?? 0;
                                        objToolAssetQuantityDetailDTO.RoomID = obj.Room ?? 0;
                                        objToolAssetQuantityDetailDTO.CompanyID = obj.CompanyID ?? 0;
                                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.WhatWhereAction = "ImportController>>SaveImport";
                                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = obj.Quantity ?? 0;
                                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Edited From Import (using Tool Master Import).";
                                        objToolAssetQuantityDetailDTO.CreatedBy = objDTO.LastUpdatedBy ?? 0;
                                        objToolAssetQuantityDetailDTO.UpdatedBy = objDTO.LastUpdatedBy ?? 0;
                                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                                        objToolAssetQuantityDetailDTO.IsArchived = false;

                                        objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, null, false, "AdjCredit", ReferalAction: "Tool Edit");
                                    }
                                }
                                else
                                {
                                    if (arrcolumns.Contains("location"))
                                    {
                                        obj.LocationID = objDTO.LocationID;
                                    }
                                }

                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";

                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region AssetsMaintainance
                    else if (tableName == ImportMastersDTO.TableName.AssetMaster.ToString())
                    {
                        List<string> p = (from d in context.AssetMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d.AssetName).ToList();
                        lstFinalAdd = (from m in (List<AssetMasterMain>)list
                                       where (!p.Contains(m.AssetName)) //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<AssetMasterMain>)list
                                   where (p.Contains(m.AssetName)) //&& m.Category != ""
                                   select m).Cast<T>().ToList();



                        List<string> Q = (from d in (List<AssetMasterMain>)list select d.AssetName).ToList();
                        List<AssetMaster> lstCostUOMMaster = (from m in context.AssetMasters
                                                              where (Q.Contains(m.AssetName)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                              select m).ToList();
                        foreach (AssetMasterMain objDTO in lstFinalAdd.Cast<AssetMasterMain>().ToList())
                        {
                            if (!isImgZipAvail)
                            {
                                objDTO.ImagePath = string.Empty;
                                objDTO.ImageType = "ExternalImage";

                            }
                        }

                        foreach (AssetMasterMain objDTO in lstEdit.Cast<AssetMasterMain>().ToList())
                        {
                            try
                            {
                                AssetMaster obj = (from m in lstCostUOMMaster
                                                   where m.AssetName == objDTO.AssetName && m.Room == RoomId && m.CompanyID == CompanyId
                                                   select m).FirstOrDefault();


                                if (arrcolumns.Contains("assetname"))
                                {
                                    obj.AssetName = objDTO.AssetName;
                                }

                                if (arrcolumns.Contains("assetcategory"))
                                {
                                    obj.AssetCategoryId = objDTO.AssetCategoryId;
                                }

                                if (arrcolumns.Contains("serial"))
                                {
                                    obj.Serial = objDTO.Serial;
                                }
                                if (arrcolumns.Contains("description"))
                                {
                                    obj.Description = objDTO.Description;
                                }
                                if (arrcolumns.Contains("toolcategory"))
                                {
                                    obj.ToolCategoryID = objDTO.ToolCategoryID;
                                }
                                if (arrcolumns.Contains("make"))
                                {
                                    obj.Make = objDTO.Make;
                                }
                                if (arrcolumns.Contains("model"))
                                {
                                    obj.Model = objDTO.Model;
                                }
                                if (arrcolumns.Contains("purchaseprice"))
                                {
                                    obj.PurchasePrice = objDTO.PurchasePrice;
                                }
                                if (arrcolumns.Contains("depreciatedvalue"))
                                {
                                    obj.DepreciatedValue = objDTO.DepreciatedValue;
                                }
                                if (arrcolumns.Contains("purchasedate"))
                                {
                                    obj.PurchaseDate = objDTO.PurchaseDate;
                                }

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }
                                if (arrcolumns.Contains("udf6"))
                                {
                                    obj.UDF6 = objDTO.UDF6;
                                }
                                if (arrcolumns.Contains("udf7"))
                                {
                                    obj.UDF7 = objDTO.UDF7;
                                }
                                if (arrcolumns.Contains("udf8"))
                                {
                                    obj.UDF8 = objDTO.UDF8;
                                }
                                if (arrcolumns.Contains("udf9"))
                                {
                                    obj.UDF9 = objDTO.UDF9;
                                }
                                if (arrcolumns.Contains("udf10"))
                                {
                                    obj.UDF10 = objDTO.UDF10;
                                }

                                obj.ImageType = objDTO.ImageType;

                                if (arrcolumns.Contains("imagepath"))
                                {
                                    if (string.IsNullOrEmpty(objDTO.ImagePath))
                                    {
                                        obj.ImagePath = string.Empty;
                                        obj.ImageType = "ExternalImage";
                                    }
                                    else
                                    {
                                        obj.ImagePath = objDTO.ImagePath;
                                        obj.ImageType = objDTO.ImageType;
                                    }

                                }

                                if (arrcolumns.Contains("assetimageexternalurl"))
                                {
                                    obj.AssetImageExternalURL = objDTO.AssetImageExternalURL;
                                }

                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Updated = objDTO.Updated;
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region QuickListItems
                    else if (tableName == ImportMastersDTO.TableName.QuickListItems.ToString())
                    {
                        List<ItemQuickListGUIDDTO> plist = (from d in context.QuickListItems
                                                            where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false
                                                            select new ItemQuickListGUIDDTO
                                                            { ItemGUID = d.ItemGUID ?? Guid.NewGuid(), QuickListGUID = d.QuickListGUID ?? Guid.NewGuid(), BinID = d.BinID ?? 0 }).ToList();

                        lstFinalAdd = (from m in (List<QuickListItemsMain>)list
                                       join p in plist on new { m.QuickListGUID, m.ItemGUID, m.BinID } equals new { p.QuickListGUID, p.ItemGUID, p.BinID } into m_p_join
                                       from m_p in m_p_join.DefaultIfEmpty()
                                       where m_p == null
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<QuickListItemsMain>)list
                                   join p in plist on new { m.QuickListGUID, m.ItemGUID, m.BinID } equals new { p.QuickListGUID, p.ItemGUID, p.BinID } into m_p_join
                                   from m_p in m_p_join.DefaultIfEmpty()
                                   where m_p != null
                                   select m).Cast<T>().ToList();

                        foreach (QuickListItemsMain objDTO in lstEdit.Cast<QuickListItemsMain>().ToList())
                        {
                            try
                            {
                                QuickListItem obj = (from m in context.QuickListItems
                                                     where m.QuickListGUID == objDTO.QuickListGUID && m.ItemGUID == objDTO.ItemGUID && m.BinID == objDTO.BinID && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                     select m).FirstOrDefault();
                                ////QuickListItem obj = (from m in lstCostUOMMaster
                                ////                     where m.QuickListGUID == objDTO.QuickListGUID && m.ItemGUID == objDTO.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                ////                     select m).FirstOrDefault();


                                if (arrcolumns.Contains("name"))
                                {
                                    obj.QuickListGUID = objDTO.QuickListGUID;
                                }
                                if (arrcolumns.Contains("binnumber"))
                                {
                                    obj.BinID = objDTO.BinID;
                                }



                                if (arrcolumns.Contains("itemnumber"))
                                {
                                    obj.ItemGUID = objDTO.ItemGUID;
                                }
                                if (arrcolumns.Contains("quantity"))
                                {
                                    obj.Quantity = objDTO.Quantity;
                                }
                                if (arrcolumns.Contains("consignedquantity"))
                                {
                                    obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                                }
                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }

                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.LastUpdated = objDTO.LastUpdated;
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                obj.EditedFrom = "Web";
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Kits
                    else if (tableName == ImportMastersDTO.TableName.kitdetail.ToString())
                    {

                        //foreach (KitDetailmain kit in (List<KitDetailmain>)list)
                        //{
                        //    if (context.KitDetails.Where(d => d.Room == RoomId && d.CompanyID == CompanyId && d.ItemGUID == kit.ItemGUID && d.KitGUID == kit.KitGUID).Any())
                        //    {
                        //        lstEdit.Add(T <kit>);
                        //    }
                        //}


                        List<ItemKitListGUIDDTO> plist = (from d in context.KitDetails where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select new ItemKitListGUIDDTO { ItemGUID = d.ItemGUID ?? Guid.Empty, KitListGUID = d.KitGUID ?? Guid.Empty }).ToList();

                        lstFinalAdd = (from m in (List<KitDetailmain>)list
                                       join p in plist on new { ktguid = m.KitGUID ?? Guid.Empty, itmgid = m.ItemGUID ?? Guid.Empty } equals new { ktguid = p.KitListGUID, itmgid = p.ItemGUID } into m_p_join
                                       from m_p in m_p_join.DefaultIfEmpty()
                                       where m_p == null
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<KitDetailmain>)list
                                   join p in plist on new { ktguid = m.KitGUID ?? Guid.Empty, itmgid = m.ItemGUID ?? Guid.Empty } equals new { ktguid = p.KitListGUID, itmgid = p.ItemGUID } into m_p_join
                                   from m_p in m_p_join.DefaultIfEmpty()
                                   where m_p != null
                                   select m).Cast<T>().ToList();
                        foreach (KitDetailmain objDTO in lstEdit.Cast<KitDetailmain>().ToList())
                        {
                            try
                            {
                                KitDetail obj = (from m in context.KitDetails
                                                 where m.KitGUID == objDTO.KitGUID && m.ItemGUID == objDTO.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                 select m).FirstOrDefault();

                                if (arrcolumns.Contains("quantityperkit"))
                                {
                                    obj.QuantityPerKit = objDTO.QuantityPerKit;
                                }


                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.LastUpdated = objDTO.LastUpdated;
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                obj.EditedFrom = "Web";

                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";

                                ItemMaster objItemMaster = (from m in context.ItemMasters
                                                            where m.GUID == objDTO.KitGUID && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                            select m).FirstOrDefault();
                                if (objItemMaster != null)
                                {
                                    //objItemMaster.IsBuildBreak = objDTO.IsBuildBreak;

                                    if (!objItemMaster.IsActive && objDTO.IsActive)
                                    {
                                        objItemMaster.ItemIsActiveDate = DateTimeUtility.DateTimeNow;
                                    }
                                    else if (objItemMaster.IsActive && objDTO.IsActive)
                                    {
                                        objItemMaster.ItemIsActiveDate = objItemMaster.ItemIsActiveDate;
                                    }
                                    else if (!objDTO.IsActive)
                                    {
                                        objItemMaster.ItemIsActiveDate = null;
                                    }

                                    objItemMaster.IsDeleted = objDTO.IsDeleted;
                                    objItemMaster.IsActive = objDTO.IsActive;
                                }
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }


                    }
                    #endregion
                    #region BOM Item Master
                    else if (tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                    {
                        CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
                        string cultureCode = "en-US";
                        var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomId, CompanyId, -1);

                        if (regionInfo != null)
                        {
                            cultureCode = regionInfo.CultureCode;
                        }
                        List<string> p = (from d in context.ItemMasters where d.Room == null && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select (d.ItemNumber ?? string.Empty).Trim().ToLower()).ToList();
                        lstFinalAdd = (from m in (List<BOMItemMasterMain>)list
                                       where (!p.Contains((m.ItemNumber ?? string.Empty).Trim().ToLower())) && m.IsDeleted == false //&& m.Category != ""
                                       select m).Cast<T>().ToList();

                        lstEdit = (from m in (List<BOMItemMasterMain>)list
                                   where (p.Contains((m.ItemNumber ?? string.Empty).Trim().ToLower())) && m.IsDeleted == false
                                   select m).Cast<T>().ToList();

                        lstFinalDelete = (from m in (List<BOMItemMasterMain>)list
                                          where (p.Contains((m.ItemNumber ?? string.Empty).Trim().ToLower())) && m.IsDeleted == true
                                          select m).Cast<T>().ToList();

                        List<string> Q = (from d in (List<BOMItemMasterMain>)list select d.ItemNumber).ToList();
                        List<ItemMaster> lstItemMaster = (from m in context.ItemMasters
                                                          where (Q.Contains(m.ItemNumber)) && m.Room == null && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                          select m).ToList();

                        foreach (BOMItemMasterMain objDTO in lstEdit.Cast<BOMItemMasterMain>().ToList())
                        {
                            try
                            {

                                ItemMaster obj = (from m in lstItemMaster
                                                  where m.ItemNumber == objDTO.ItemNumber && m.Room == null
                                                  select m).FirstOrDefault();


                                BOMItemMasterDAL objItemDal = new BOMItemMasterDAL(base.DataBaseName);
                                BOMItemDTO ObjITEM2 = objItemDal.GetItemByItemID(obj.ID);
                                objDTO.ID = ObjITEM2.ID;
                                objDTO.GUID = ObjITEM2.GUID;

                                if (objDTO.InventoryClassification == 0)
                                {
                                    objDTO.InventoryClassification = null;
                                }
                                objDTO.WhatWhereAction = "Import";
                                EditBOMItemMaster(objDTO, arrcolumns);
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }

                        bool DeleteItems = false;
                        foreach (BOMItemMasterMain objDTO in lstFinalDelete.Cast<BOMItemMasterMain>().ToList())
                        {
                            try
                            {
                                DeleteItems = true;
                                ItemMaster obj = (from m in lstItemMaster
                                                  where m.ItemNumber == objDTO.ItemNumber && m.CompanyID == objDTO.CompanyID
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
                    }
                    #endregion
                    #region Inventory Location Master
                    else if (tableName == ImportMastersDTO.TableName.BinMaster.ToString())
                    {
                        foreach (InventoryLocationMain objDTO in list.Cast<InventoryLocationMain>().ToList())
                        {
                            try
                            {
                                BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                                BinMaster obj = new BinMaster();
                                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                                long BinId = 0;
                                obj = context.BinMasters.FirstOrDefault(t => t.ParentBinId == null && t.BinNumber == objDTO.BinNumber && t.IsDeleted == false && t.IsArchived == false && t.Room == objDTO.Room && t.CompanyID == objDTO.CompanyID);
                                if (obj == null)
                                {
                                    objBinMasterDTO.BinNumber = objDTO.BinNumber;
                                    objBinMasterDTO.Room = objDTO.Room;
                                    objBinMasterDTO.CompanyID = objDTO.CompanyID;
                                    objBinMasterDTO.IsArchived = false;
                                    objBinMasterDTO.IsDeleted = false;
                                    objBinMasterDTO.IsDefault = false;
                                    objBinMasterDTO.Created = objDTO.Created;
                                    objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                                    objBinMasterDTO.LastUpdated = objDTO.Updated;
                                    objBinMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objBinMasterDTO.AddedFrom = "Web";
                                    objBinMasterDTO.EditedFrom = "Web";
                                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);
                                    BinId = objBinMasterDTO.ID;


                                    objBinMasterDTO = new BinMasterDTO();
                                    objBinMasterDTO.BinNumber = objDTO.BinNumber;
                                    objBinMasterDTO.ItemGUID = objDTO.ItemGUID;
                                    objBinMasterDTO.CriticalQuantity = 0;
                                    objBinMasterDTO.MinimumQuantity = 0;
                                    objBinMasterDTO.MaximumQuantity = 0;
                                    objBinMasterDTO.Room = objDTO.Room;
                                    objBinMasterDTO.CompanyID = objDTO.CompanyID;
                                    objBinMasterDTO.IsArchived = false;
                                    objBinMasterDTO.IsDeleted = false;
                                    objBinMasterDTO.IsDefault = false;
                                    objBinMasterDTO.ParentBinId = BinId;
                                    objBinMasterDTO.Created = objDTO.Created;
                                    objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                                    objBinMasterDTO.LastUpdated = objDTO.Updated;
                                    objBinMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objBinMasterDTO.AddedFrom = "Web";
                                    objBinMasterDTO.EditedFrom = "Web";
                                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);
                                    BinId = objBinMasterDTO.ID;
                                    //obj = context.BinMasters.FirstOrDefault(t => t.ParentBinId != null && t.ItemGUID == objDTO.ItemGUID && t.BinNumber == objDTO.BinNumber && t.IsDeleted == false && t.IsArchived == false);
                                }
                                else
                                {
                                    BinMaster obj1 = context.BinMasters.FirstOrDefault(t => t.ParentBinId == obj.ID && t.ItemGUID == objDTO.ItemGUID && t.IsDeleted == false && t.IsArchived == false && t.Room == objDTO.Room && t.CompanyID == objDTO.CompanyID);
                                    if (obj1 != null)
                                    {
                                        BinId = obj1.ID;
                                    }
                                    else
                                    {
                                        objBinMasterDTO = new BinMasterDTO();
                                        objBinMasterDTO.BinNumber = objDTO.BinNumber;
                                        objBinMasterDTO.ItemGUID = objDTO.ItemGUID;
                                        objBinMasterDTO.CriticalQuantity = 0;
                                        objBinMasterDTO.MinimumQuantity = 0;
                                        objBinMasterDTO.MaximumQuantity = 0;
                                        objBinMasterDTO.Room = objDTO.Room;
                                        objBinMasterDTO.CompanyID = objDTO.CompanyID;
                                        objBinMasterDTO.IsArchived = false;
                                        objBinMasterDTO.IsDeleted = false;
                                        objBinMasterDTO.IsDefault = false;
                                        objBinMasterDTO.ParentBinId = obj.ID;
                                        objBinMasterDTO.Created = objDTO.Created;
                                        objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                                        objBinMasterDTO.LastUpdated = objDTO.Updated;
                                        objBinMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                        objBinMasterDTO.AddedFrom = "Web";
                                        objBinMasterDTO.EditedFrom = "Web";
                                        objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);
                                        BinId = objBinMasterDTO.ID;
                                    }
                                }

                                ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                                //if (objItemLocationDetailsDAL.CheckIsQuantityDifferences(objDTO.ItemGUID ?? Guid.Empty, BinId, objDTO.consignedquantity ?? 0, objDTO.customerownedquantity ?? 0))
                                //{

                                //}

                                //if (objDTO.IsAdjustment)
                                //{
                                //    ApplyCountForItemLocation(RoomId, CompanyId, objDTO.CreatedBy ?? 0, BinId, objDTO.consignedquantity ?? 0, objDTO.customerownedquantity ?? 0, objDTO.ItemGUID ?? Guid.Empty, objDTO.SerialNumber, objDTO.LotNumber, objDTO.Expiration);
                                //}
                                //else
                                //{
                                if ((objDTO.customerownedquantity.GetValueOrDefault(0) + objDTO.consignedquantity.GetValueOrDefault(0)) > 0)
                                {
                                    ItemLocationDetailsDTO objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                    objItemLocationDetailsDTO.Room = RoomId;
                                    objItemLocationDetailsDTO.CompanyID = CompanyId;
                                    objItemLocationDetailsDTO.ItemGUID = objDTO.ItemGUID;
                                    objItemLocationDetailsDTO.BinID = BinId;
                                    objItemLocationDetailsDTO.BinNumber = objDTO.BinNumber;
                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = objDTO.customerownedquantity;
                                    objItemLocationDetailsDTO.ConsignedQuantity = objDTO.consignedquantity;
                                    objItemLocationDetailsDTO.SerialNumber = objDTO.SerialNumber;
                                    objItemLocationDetailsDTO.LotNumber = objDTO.LotNumber;
                                    objItemLocationDetailsDTO.Expiration = objDTO.Expiration;
                                    objItemLocationDetailsDTO.Received = objDTO.Received;
                                    objItemLocationDetailsDTO.IsDeleted = false;
                                    objItemLocationDetailsDTO.IsArchived = false;
                                    objItemLocationDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                                    objItemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objItemLocationDetailsDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objItemLocationDetailsDTO.CreatedBy = objDTO.CreatedBy;
                                    objItemLocationDetailsDTO.GUID = Guid.NewGuid();
                                    objItemLocationDetailsDTO.InsertedFrom = "Import";
                                    objItemLocationDetailsDTO.Cost = objDTO.Cost;
                                    objItemLocationDetailsDAL.ItemLocationDetailsImportSave(objItemLocationDetailsDTO, SessionUserId,EnterpriseID);
                                }
                                //}

                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                        }
                        CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.InvalidateCache();
                    }
                    #endregion
                    #region Inventory Location Master
                    else if (tableName == ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString())
                    {
                        foreach (InventoryLocationQuantityMain objDTO in list.Cast<InventoryLocationQuantityMain>().ToList())
                        {
                            try
                            {


                                BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                                BinMaster obj = new BinMaster();
                                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                                objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);
                                long BinId = 0;
                                if (objDTO.IsStagingLocation.GetValueOrDefault(false) == true)
                                {
                                    obj = context.BinMasters.FirstOrDefault(t => t.IsStagingLocation == true && t.BinNumber == objDTO.BinNumber && t.Room == objDTO.Room && t.CompanyID == objDTO.CompanyID && t.IsDeleted == false && t.IsArchived == false && t.ParentBinId == null);
                                }
                                else
                                {
                                    obj = context.BinMasters.FirstOrDefault(t => t.IsStagingLocation == false && t.BinNumber == objDTO.BinNumber && t.Room == objDTO.Room && t.CompanyID == objDTO.CompanyID && t.IsDeleted == false && t.IsArchived == false && t.ParentBinId == null);
                                }

                                if (obj == null)
                                {

                                    objBinMasterDTO.BinNumber = objDTO.BinNumber;
                                    objBinMasterDTO.Room = objDTO.Room;
                                    objBinMasterDTO.CompanyID = objDTO.CompanyID;
                                    objBinMasterDTO.IsArchived = false;
                                    objBinMasterDTO.IsDeleted = objDTO.IsDeleted;
                                    objBinMasterDTO.IsDefault = false;
                                    objBinMasterDTO.Created = objDTO.Created;
                                    objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                                    objBinMasterDTO.LastUpdated = objDTO.Updated;
                                    objBinMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objBinMasterDTO.AddedFrom = "Web";
                                    objBinMasterDTO.EditedFrom = "Web";
                                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO.IsStagingLocation = objDTO.IsStagingLocation.GetValueOrDefault(false);
                                    objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);

                                    BinId = objBinMasterDTO.ID;


                                    objBinMasterDTO = new BinMasterDTO();
                                    objBinMasterDTO.BinNumber = objDTO.BinNumber;
                                    objBinMasterDTO.ItemGUID = objDTO.ItemGUID;
                                    if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired == false)
                                    {
                                        objBinMasterDTO.CriticalQuantity = objDTO.CriticalQuantity;
                                        objBinMasterDTO.MinimumQuantity = objDTO.MinimumQuantity;
                                        objBinMasterDTO.MaximumQuantity = objDTO.MaximumQuantity;
                                    }
                                    else
                                    {
                                        objBinMasterDTO.CriticalQuantity = 0;
                                        objBinMasterDTO.MinimumQuantity = 0;
                                        objBinMasterDTO.MaximumQuantity = 0;
                                    }

                                    objBinMasterDTO.IsEnforceDefaultPullQuantity = objDTO.IsEnforceDefaultPullQuantity;
                                    objBinMasterDTO.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                                    objBinMasterDTO.DefaultPullQuantity = objDTO.DefaultPullQuantity;
                                    objBinMasterDTO.DefaultReorderQuantity = objDTO.DefaultReorderQuantity;

                                    objBinMasterDTO.Room = objDTO.Room;
                                    objBinMasterDTO.CompanyID = objDTO.CompanyID;
                                    objBinMasterDTO.IsArchived = false;
                                    objBinMasterDTO.IsDeleted = false;
                                    objBinMasterDTO.IsDefault = objDTO.IsDefault;
                                    objBinMasterDTO.ParentBinId = BinId;
                                    objBinMasterDTO.Created = objDTO.Created;
                                    objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                                    objBinMasterDTO.LastUpdated = objDTO.Updated;
                                    objBinMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                    objBinMasterDTO.AddedFrom = "Web";
                                    objBinMasterDTO.EditedFrom = "Web";
                                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objBinMasterDTO.IsStagingLocation = objDTO.IsStagingLocation.GetValueOrDefault(false);
                                    if (objBinMasterDTO.IsStagingLocation)
                                    {
                                        objBinMasterDTO.IsDefault = false;
                                        objDTO.IsDefault = false;
                                    }
                                    objBinMasterDTO.BinUDF1 = objDTO.BinUDF1;
                                    objBinMasterDTO.BinUDF2 = objDTO.BinUDF2;
                                    objBinMasterDTO.BinUDF3 = objDTO.BinUDF3;
                                    objBinMasterDTO.BinUDF4 = objDTO.BinUDF4;
                                    objBinMasterDTO.BinUDF5 = objDTO.BinUDF5;
                                    objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);
                                    BinId = objBinMasterDTO.ID;
                                    //obj = context.BinMasters.FirstOrDefault(t => t.ParentBinId != null && t.ItemGUID == objDTO.ItemGUID && t.BinNumber == objDTO.BinNumber && t.IsDeleted == false && t.IsArchived == false);

                                }
                                else
                                {
                                    BinMaster obj1 = context.BinMasters.FirstOrDefault(t => t.ParentBinId == obj.ID && t.ItemGUID == objDTO.ItemGUID && t.IsDeleted == false && t.IsArchived == false && t.Room == objDTO.Room && t.CompanyID == objDTO.CompanyID);
                                    if (obj1 != null)
                                    {
                                        bool OldIsDeleted = obj1.IsDeleted;

                                        obj1.LastUpdated = DateTimeUtility.DateTimeNow;
                                        obj1.LastUpdatedBy = UserId;

                                        if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired == false)
                                        {
                                            obj1.CriticalQuantity = objDTO.CriticalQuantity;
                                            obj1.MinimumQuantity = objDTO.MinimumQuantity;
                                            obj1.MaximumQuantity = objDTO.MaximumQuantity;
                                            obj1.IsDefault = objDTO.IsDefault;

                                        }

                                        obj1.IsEnforceDefaultPullQuantity = objDTO.IsEnforceDefaultPullQuantity;
                                        obj1.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                                        obj1.DefaultPullQuantity = objDTO.DefaultPullQuantity;
                                        obj1.DefaultReorderQuantity = objDTO.DefaultReorderQuantity;
                                        
                                        obj1.BinUDF1 = objDTO.BinUDF1;
                                        obj1.BinUDF2 = objDTO.BinUDF2;
                                        obj1.BinUDF3 = objDTO.BinUDF3;
                                        obj1.BinUDF4 = objDTO.BinUDF4;
                                        obj1.BinUDF5 = objDTO.BinUDF5;

                                        obj1.IsDeleted = objDTO.IsDeleted ?? false;
                                        BinId = obj1.ID;

                                        if (OldIsDeleted == false && objDTO.IsDeleted == true)
                                        {
                                            List<ItemMasterDTO> lstItemMaster = objBinMasterDAL.CSP_DeleteBinDataById(obj1.ItemGUID.ToString() + "," + obj1.ID.ToString(), (long)obj1.CompanyID, (long)obj1.Room, UserId, "BulkImport >> Delete Bin", SessionUserId);
                                            if (lstItemMaster != null && lstItemMaster.Count > 0 && lstItemMaster[0].OnHandQuantity != null)
                                            {
                                                ItemMasterDAL objItemMaster = new ItemMasterDAL(base.DataBaseName);
                                                objItemMaster.UpdateOnHandQuantity((Guid)obj1.ItemGUID, (double)lstItemMaster[0].OnHandQuantity);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        objBinMasterDTO = new BinMasterDTO();
                                        objBinMasterDTO.BinNumber = objDTO.BinNumber;
                                        objBinMasterDTO.ItemGUID = objDTO.ItemGUID;
                                        if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired == false)
                                        {
                                            objBinMasterDTO.CriticalQuantity = objDTO.CriticalQuantity;
                                            objBinMasterDTO.MinimumQuantity = objDTO.MinimumQuantity;
                                            objBinMasterDTO.MaximumQuantity = objDTO.MaximumQuantity;
                                        }
                                        else
                                        {
                                            objBinMasterDTO.CriticalQuantity = 0;
                                            objBinMasterDTO.MinimumQuantity = 0;
                                            objBinMasterDTO.MaximumQuantity = 0;
                                        }

                                        objBinMasterDTO.IsEnforceDefaultPullQuantity = objDTO.IsEnforceDefaultPullQuantity;
                                        objBinMasterDTO.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                                        objBinMasterDTO.DefaultPullQuantity = objDTO.DefaultPullQuantity;
                                        objBinMasterDTO.DefaultReorderQuantity = objDTO.DefaultReorderQuantity;
                                        
                                        objBinMasterDTO.BinUDF1 = objDTO.BinUDF1;
                                        objBinMasterDTO.BinUDF2 = objDTO.BinUDF2;
                                        objBinMasterDTO.BinUDF3 = objDTO.BinUDF3;
                                        objBinMasterDTO.BinUDF4 = objDTO.BinUDF4;
                                        objBinMasterDTO.BinUDF5 = objDTO.BinUDF5;

                                        objBinMasterDTO.Room = objDTO.Room;
                                        objBinMasterDTO.CompanyID = objDTO.CompanyID;
                                        objBinMasterDTO.IsArchived = false;
                                        objBinMasterDTO.IsDeleted = objDTO.IsDeleted;
                                        objBinMasterDTO.IsDefault = objDTO.IsDefault;
                                        objBinMasterDTO.ParentBinId = obj.ID;
                                        objBinMasterDTO.Created = objDTO.Created;
                                        objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                                        objBinMasterDTO.LastUpdated = objDTO.Updated;
                                        objBinMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                        objBinMasterDTO.AddedFrom = "Web";
                                        objBinMasterDTO.EditedFrom = "Web";
                                        objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTO.IsStagingLocation = objDTO.IsStagingLocation.GetValueOrDefault(false);
                                        if (objBinMasterDTO.IsStagingLocation)
                                        {
                                            objBinMasterDTO.IsDefault = false;
                                            objDTO.IsDefault = false;
                                        }
                                        objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);

                                        BinId = objBinMasterDTO.ID;
                                    }

                                }
                                //if (obj == null)
                                //{

                                //    objBinMasterDTO.BinNumber = objDTO.BinNumber;
                                //    objBinMasterDTO.ItemGUID = objDTO.ItemGUID;
                                //    if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired == false)
                                //    {
                                //        objBinMasterDTO.CriticalQuantity = objDTO.CriticalQuantity;
                                //        objBinMasterDTO.MinimumQuantity = objDTO.MinimumQuantity;
                                //        objBinMasterDTO.MaximumQuantity = objDTO.MaximumQuantity;
                                //    }
                                //    else
                                //    {
                                //        objBinMasterDTO.CriticalQuantity = 0;
                                //        objBinMasterDTO.MinimumQuantity = 0;
                                //        objBinMasterDTO.MaximumQuantity = 0;
                                //    }
                                //    objBinMasterDTO.Room = objDTO.Room;
                                //    objBinMasterDTO.CompanyID = objDTO.CompanyID;
                                //    objBinMasterDTO.IsArchived = false;
                                //    objBinMasterDTO.IsDeleted = false;
                                //    objBinMasterDTO.IsDefault = false;
                                //    objBinMasterDTO.Created = objDTO.Created;
                                //    objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                                //    objBinMasterDTO.LastUpdated = objDTO.Updated;
                                //    objBinMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                //    BinId = objBinMasterDAL.Insert(objBinMasterDTO);
                                //}
                                //else
                                //{
                                //    if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired == false)
                                //    {
                                //        obj.CriticalQuantity = objDTO.CriticalQuantity;
                                //        obj.MinimumQuantity = objDTO.MinimumQuantity;
                                //        obj.MaximumQuantity = objDTO.MaximumQuantity;
                                //        obj.IsDefault = objDTO.IsDefault;
                                //        obj.LastUpdated = DateTimeUtility.DateTimeNow;
                                //        obj.LastUpdatedBy = UserId;
                                //    }
                                //    BinId = obj.ID;
                                //}
                                if (objDTO.IsDefault == true && BinId > 0)
                                {
                                    UpdateDefaultLocation(BinId, objDTO.ItemGUID ?? Guid.NewGuid(), UserId);
                                }
                                if ((objDTO.SensorPort != null && objDTO.SensorPort != string.Empty) || (objDTO.SensorId != null))
                                {
                                    ItemLocationeVMISetupDTO objItemLocationeVMISetupDTO = new ItemLocationeVMISetupDTO();
                                    ItemLocationeVMISetupDAL objItemLocationeVMISetupDAL = new ItemLocationeVMISetupDAL(base.DataBaseName);
                                    ItemLocationeVMISetup objItemLocationeVMISetup = new ItemLocationeVMISetup();
                                    objItemLocationeVMISetup = context.ItemLocationeVMISetups.FirstOrDefault(t => t.BinID == BinId && t.ItemGUID == objDTO.ItemGUID && t.IsDeleted == false && t.IsArchived == false);
                                    if (objItemLocationeVMISetup != null)
                                    {
                                        objItemLocationeVMISetup.eVMISensorPort = objDTO.SensorPort;
                                        objItemLocationeVMISetup.eVMISensorID = objDTO.SensorId;
                                        objItemLocationeVMISetup.Updated = objDTO.Updated;
                                        objItemLocationeVMISetup.LastUpdatedBy = objDTO.LastUpdatedBy;
                                        objItemLocationeVMISetup.IsDeleted = objDTO.IsDeleted;
                                        context.SaveChanges();
                                    }
                                    else if (objDTO.IsDeleted == false)
                                    {
                                        //objItemLocationeVMISetupDTO
                                        objItemLocationeVMISetupDTO.BinID = BinId;
                                        objItemLocationeVMISetupDTO.ItemGUID = objDTO.ItemGUID;
                                        objItemLocationeVMISetupDTO.eVMISensorPort = objDTO.SensorPort;
                                        objItemLocationeVMISetupDTO.eVMISensorID = objDTO.SensorId;
                                        objItemLocationeVMISetupDTO.Room = objDTO.Room;
                                        objItemLocationeVMISetupDTO.CompanyID = objDTO.CompanyID;
                                        objItemLocationeVMISetupDTO.IsArchived = false;
                                        //objItemLocationeVMISetupDTO.IsDeleted = false;
                                        objItemLocationeVMISetupDTO.Created = objDTO.Created;
                                        objItemLocationeVMISetupDTO.CreatedBy = objDTO.CreatedBy;
                                        objItemLocationeVMISetupDTO.Updated = objDTO.Updated;
                                        objItemLocationeVMISetupDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                                        objItemLocationeVMISetupDTO.AddedFrom = "Web";
                                        objItemLocationeVMISetupDTO.EditedFrom = "Web";
                                        objItemLocationeVMISetupDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetupDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objItemLocationeVMISetupDTO.IsDeleted = objDTO.IsDeleted;
                                        objItemLocationeVMISetupDAL.Insert(objItemLocationeVMISetupDTO);
                                    }
                                }
                                if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired == false)
                                {
                                    /*
                                    CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                                    //objCartItemDAL.AutoCartUpdateByCode(objDTO.ItemGUID ?? Guid.NewGuid(), UserId, "Web", "ImportDAL >> BulkInsert >> TableName.ItemLocationeVMISetup");
                                    objCartItemDAL.AutoCartUpdateByCode(objDTO.ItemGUID ?? Guid.NewGuid(), UserId, "Web", "BulkImport >> ItemLocation");
                                    */
                                }
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                        }
                        CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<ItemLocationeVMISetupDTO>>.InvalidateCache();
                    }
                    #endregion
                    #region ItemManufacturer
                    else if (tableName == ImportMastersDTO.TableName.ItemManufacturerDetails.ToString())
                    {
                        List<ItemManufacturerDetail> p = (from d in context.ItemManufacturerDetails where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d).ToList();

                        lstFinalAdd = ((List<ItemManufacturer>)list).Where(a => !p.Select(b => new { b.ItemGUID, b.ManufacturerName }).Contains(new { a.ItemGUID, a.ManufacturerName })).Cast<T>().ToList();
                        lstEdit = ((List<ItemManufacturer>)list).Where(a => p.Select(b => new { b.ItemGUID, b.ManufacturerName }).Contains(new { a.ItemGUID, a.ManufacturerName })).Cast<T>().ToList();

                        foreach (ItemManufacturer objItemManufacturer in lstEdit.Cast<ItemManufacturer>().ToList().GroupBy(l => l.ItemGUID).Select(g => g.First()))
                        {
                            List<ItemManufacturerDetail> objlist = (from m in p
                                                                    where m.ItemGUID == objItemManufacturer.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                    && m.IsDeleted == false && m.IsArchived == false
                                                                    select m).ToList();

                            if (objlist != null)
                            {
                                int DefaultCount = objlist.Where(o => o.IsDefault == true).Count();
                                int DefaultListCount = (lstEdit.Cast<ItemManufacturer>()).Where(l => l.IsDefault == true && l.ItemGUID == objItemManufacturer.ItemGUID).Count();
                                if (DefaultCount > 0 && DefaultListCount > 0)
                                {
                                    ItemManufacturer itemamnu = (lstEdit.Cast<ItemManufacturer>()).Where(l => l.IsDefault == true && l.ItemGUID == objItemManufacturer.ItemGUID).FirstOrDefault();
                                    if (itemamnu != null)
                                    {
                                        if (DefaultListCount > 1)
                                        {

                                            (lstEdit.Cast<ItemManufacturer>()).Where(l => l.IsDefault == true && l.ItemGUID == itemamnu.ItemGUID && l.ManufacturerName != itemamnu.ManufacturerName).ToList().ForEach(
                                                l => l.IsDefault = false
                                            );


                                        }
                                        string ExistingManufacturerName = objlist.Where(o => o.IsDefault == true).FirstOrDefault().ManufacturerName;
                                        string CurrentManufacturerName = itemamnu.ManufacturerName;
                                        if (ExistingManufacturerName != null && CurrentManufacturerName != null && ExistingManufacturerName != CurrentManufacturerName)
                                        {
                                            ItemManufacturerDetail obj = (from m in p
                                                                          where m.ItemGUID == itemamnu.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId && (m.ManufacturerName ?? string.Empty) == (ExistingManufacturerName)
                                                                          && m.IsDeleted == false && m.IsArchived == false
                                                                          select m).FirstOrDefault();
                                            obj.IsDefault = false;
                                        }
                                    }
                                }

                            }
                        }
                        foreach (ItemManufacturer objItemManufacturer in lstFinalAdd.Cast<ItemManufacturer>().ToList().GroupBy(l => l.ItemGUID).Select(g => g.First()))
                        {
                            ItemManufacturer itemamnu = (lstFinalAdd.Cast<ItemManufacturer>()).Where(l => l.IsDefault == true && l.ItemGUID == objItemManufacturer.ItemGUID).FirstOrDefault();
                            (lstFinalAdd.Cast<ItemManufacturer>()).Where(l => l.IsDefault == true && l.ItemGUID == itemamnu.ItemGUID && l.ManufacturerName != itemamnu.ManufacturerName).ToList().ForEach(
                                           l => l.IsDefault = false
                                       );
                            List<ItemManufacturerDetail> objlist = (from m in p
                                                                    where m.ItemGUID == objItemManufacturer.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                    && m.IsDeleted == false && m.IsArchived == false
                                                                    select m).ToList();

                            if (objlist != null && itemamnu != null)
                            {
                                int DefaultCount = objlist.Where(o => o.IsDefault == true).Count();
                                if (DefaultCount > 0)
                                {
                                    ItemManufacturerDetail obj = (from m in p
                                                                  where m.ItemGUID == itemamnu.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                  && m.IsDeleted == false && m.IsArchived == false
                                                                  select m).FirstOrDefault();
                                    obj.IsDefault = false;
                                }
                            }


                        }
                        foreach (ItemManufacturer objDTO in lstEdit.Cast<ItemManufacturer>().ToList())
                        {
                            try
                            {
                                ItemManufacturerDetail obj = (from m in p
                                                              where m.ItemGUID == objDTO.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId && (m.ManufacturerName ?? string.Empty) == (objDTO.ManufacturerName ?? string.Empty)
                                                              && m.IsDeleted == false && m.IsArchived == false
                                                              select m).SingleOrDefault();


                                if (arrcolumns.Contains("manufacturernumber"))
                                {
                                    obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                                }
                                if (arrcolumns.Contains("isdefault"))
                                {
                                    obj.IsDefault = objDTO.IsDefault;
                                }


                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Updated = objDTO.Updated;




                                //else
                                {
                                    objDTO.Status = "Success";
                                    objDTO.Reason = "N/A";
                                }
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region ItemSupplier
                    else if (tableName == ImportMastersDTO.TableName.ItemSupplierDetails.ToString())
                    {
                        /* SET DEFAULT FALSE FOR OTHER RECORD IN AVAILABLE LIST GROUP BY ITEMGUID */
                        int itemCount = 0;
                        foreach (var item in list.Cast<ItemSupplier>().ToList().GroupBy(l => l.ItemGUID))
                        {
                            var currentDefaultItemSuppliers = item.Cast<ItemSupplier>().Where(x => x.IsDefault == true).ToList();
                            foreach (ItemSupplier objItem in currentDefaultItemSuppliers)
                            {
                                if (itemCount < currentDefaultItemSuppliers.Count() - 1)
                                {
                                    objItem.IsDefault = false;
                                    itemCount++;
                                }
                            }
                        }
                        /* SET DEFAULT FALSE FOR OTHER RECORD IN AVAILABLE LIST GROUP BY ITEMGUID */

                        List<ItemSupplierDetail> p = (from d in context.ItemSupplierDetails where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d).ToList();

                        lstFinalAdd = ((List<ItemSupplier>)list).Where(a => !p.Select(b => new { b.ItemGUID, b.SupplierName }).Contains(new { a.ItemGUID, a.SupplierName })).Cast<T>().ToList();
                        lstEdit = ((List<ItemSupplier>)list).Where(a => p.Select(b => new { b.ItemGUID, b.SupplierName }).Contains(new { a.ItemGUID, a.SupplierName })).Cast<T>().ToList();

                        foreach (ItemSupplier objItemSupplier in lstEdit.Cast<ItemSupplier>().ToList().GroupBy(l => l.ItemGUID).Select(g => g.First()))
                        {
                            List<ItemSupplierDetail> objlist = (from m in p
                                                                where m.ItemGUID == objItemSupplier.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                && m.IsDeleted == false && m.IsArchived == false
                                                                select m).ToList();

                            if (objlist != null)
                            {
                                int DefaultCount = objlist.Where(o => o.IsDefault == true).Count();
                                int DefaultListCount = (lstEdit.Cast<ItemSupplier>()).Where(l => l.IsDefault == true && l.ItemGUID == objItemSupplier.ItemGUID).Count();
                                if (DefaultCount > 0 && DefaultListCount > 0)
                                {
                                    ItemSupplier itemamnu = (lstEdit.Cast<ItemSupplier>()).Where(l => l.IsDefault == true && l.ItemGUID == objItemSupplier.ItemGUID).FirstOrDefault();
                                    if (DefaultListCount > 1)
                                    {

                                        (lstEdit.Cast<ItemSupplier>()).Where(l => l.IsDefault == true && l.ItemGUID == itemamnu.ItemGUID && l.SupplierName != itemamnu.SupplierName).ToList().ForEach(
                                            l => l.IsDefault = false
                                        );


                                    }
                                    string ExistingSupplierName = objlist.Where(o => o.IsDefault == true).FirstOrDefault().SupplierName;
                                    string CurrentSupplierName = itemamnu.SupplierName;
                                    if (ExistingSupplierName != null && CurrentSupplierName != null && ExistingSupplierName != CurrentSupplierName)
                                    {
                                        ItemSupplierDetail obj = (from m in p
                                                                  where m.ItemGUID == itemamnu.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId && (m.SupplierName ?? string.Empty) == (ExistingSupplierName)
                                                                  && m.IsDeleted == false && m.IsArchived == false
                                                                  select m).FirstOrDefault();
                                        obj.IsDefault = false;
                                    }
                                }

                            }
                        }
                        foreach (ItemSupplier objItemSupplier in lstFinalAdd.Cast<ItemSupplier>().ToList().GroupBy(l => l.ItemGUID).Select(g => g.First()))
                        {
                            ItemSupplier itemamnu = (lstFinalAdd.Cast<ItemSupplier>()).Where(l => l.IsDefault == true && l.ItemGUID == objItemSupplier.ItemGUID).FirstOrDefault();

                            if (itemamnu != null)
                            {
                                (lstFinalAdd.Cast<ItemSupplier>()).Where(l => l.IsDefault == true && l.ItemGUID == itemamnu.ItemGUID && l.SupplierName != itemamnu.SupplierName).ToList().ForEach(
                                           l => l.IsDefault = false
                                       );
                                List<ItemSupplierDetail> objlist = (from m in p
                                                                    where m.ItemGUID == objItemSupplier.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                    && m.IsDeleted == false && m.IsArchived == false
                                                                    select m).ToList();

                                if (objlist != null)
                                {
                                    int DefaultCount = objlist.Where(o => o.IsDefault == true).Count();
                                    if (DefaultCount > 0 && itemamnu != null)
                                    {
                                        List<ItemSupplierDetail> obj = (from m in p
                                                                        where m.ItemGUID == itemamnu.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                        && m.IsDeleted == false && m.IsArchived == false
                                                                        orderby m.ID descending
                                                                        select m).ToList();
                                        foreach (ItemSupplierDetail objDfault in obj)
                                        {
                                            objDfault.IsDefault = false;
                                        }
                                    }
                                }
                            }
                        }
                        foreach (ItemSupplier objDTO in lstEdit.Cast<ItemSupplier>().ToList())
                        {
                            try
                            {
                                ItemSupplierDetail obj = (from m in p
                                                          where m.ItemGUID == objDTO.ItemGUID && m.Room == RoomId && m.CompanyID == CompanyId && (m.SupplierName ?? string.Empty) == (objDTO.SupplierName ?? string.Empty)
                                                              && m.IsDeleted == false && m.IsArchived == false
                                                          select m).SingleOrDefault();


                                if (arrcolumns.Contains("suppliernumber"))
                                {
                                    obj.SupplierNumber = objDTO.SupplierNumber;
                                }
                                if (arrcolumns.Contains("isdefault"))
                                {
                                    if (objDTO.IsDefault != null && objDTO.IsDefault.Value == true && objDTO.SupplierID == obj.SupplierID) // WI-3487
                                        obj.IsDefault = objDTO.IsDefault;
                                }

                                /*===comment code due to blanketpoid not update*/
                                //if (arrcolumns.Contains("blanketpoid"))
                                //{
                                //    obj.BlanketPOID = objDTO.BlanketPOID;
                                //}
                                /*===comment code due to blanketpoid not update*/

                                obj.BlanketPOID = objDTO.BlanketPOID; // for update BlanketPOID

                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Updated = objDTO.Updated;




                                //else
                                {
                                    objDTO.Status = "Success";
                                    objDTO.Reason = "N/A";
                                }
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region BarcodeMaster
                    else if (tableName == ImportMastersDTO.TableName.BarcodeMaster.ToString())
                    {
                        List<BarcodeMaster> p = (from d in context.BarcodeMasters where d.RoomID == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false && d.BarcodeAdded.ToLower() == "manual" select d).ToList();

                        lstFinalAdd = ((List<ImportBarcodeMaster>)list).Where(a => !p.Select(b => new { b.RefGuid, b.ModuleGuid, b.BinGuid, b.BarcodeString }).Contains(new { a.RefGuid, a.ModuleGuid, a.BinGuid, a.BarcodeString })).Cast<T>().ToList();
                        // lstEdit = ((List<ImportBarcodeMaster>)list).Where(a => p.Select(b => new { b.RefGuid, b.ModuleGuid, b.BarcodeString }).Contains(new { a.RefGuid, a.ModuleGuid, a.BarcodeString })).Cast<T>().ToList();


                        foreach (ImportBarcodeMaster objDTO in lstEdit.Cast<ImportBarcodeMaster>().ToList())
                        {
                            try
                            {
                                BarcodeMaster obj = (from m in p
                                                     where m.RefGuid == objDTO.RefGuid && m.RoomID == RoomId && m.CompanyID == CompanyId && m.ModuleGuid == objDTO.ModuleGuid
                                                     && m.BinGuid == objDTO.BinGuid && m.BarcodeAdded.ToLower() == "manual"
                                                              && m.IsDeleted == false && m.IsArchived == false
                                                     select m).SingleOrDefault();


                                if (arrcolumns.Contains("barcodestring"))
                                {
                                    obj.BarcodeString = objDTO.BarcodeString;
                                }



                                obj.UpdatedBy = objDTO.UpdatedBy;
                                obj.UpdatedOn = objDTO.UpdatedOn;

                                obj.EditedFrom = objDTO.EditedFrom;
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                //obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;


                                //else
                                {
                                    objDTO.Status = "Success";
                                    objDTO.Reason = "N/A";
                                }
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Project Master
                    else if (tableName == ImportMastersDTO.TableName.ProjectMaster.ToString())
                    {
                        List<ProjectMaster> plist = (from d in context.ProjectMasters where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d).ToList();

                        //lstFinalAdd = (from m in (List<ProjectMasterMain>)list
                        //               join p in plist on new { m.ProjectSpendName } equals new { p.ProjectSpendName } into m_p_join
                        //               from m_p in m_p_join.DefaultIfEmpty()
                        //               where m_p == null
                        //               select m).Cast<T>().ToList();

                        //lstEdit = (from m in (List<ProjectMasterMain>)list
                        //           join p in plist on new { m.ProjectSpendName } equals new { p.ProjectSpendName } into m_p_join
                        //           from m_p in m_p_join.DefaultIfEmpty()
                        //           where m_p != null
                        //           select m).Cast<T>().ToList();
                        // List<ItemSupplierDetail> p = (from d in context.ItemSupplierDetails where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false select d).ToList();

                        lstFinalAdd = ((List<ProjectMasterMain>)list).Where(a => !plist.Select(b => new { b.ProjectSpendName }).Contains(new { a.ProjectSpendName })).Cast<T>().ToList();
                        lstEdit = ((List<ProjectMasterMain>)list).Where(a => plist.Select(b => new { b.ProjectSpendName }).Contains(new { a.ProjectSpendName })).Cast<T>().ToList();



                        foreach (ProjectMasterMain objDTO in lstEdit.Cast<ProjectMasterMain>().ToList())
                        {
                            try
                            {
                                ProjectMaster obj = (from m in context.ProjectMasters
                                                     where m.ProjectSpendName.ToLower().Trim() == objDTO.ProjectSpendName.ToLower().Trim() && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                     select m).FirstOrDefault();


                                if (arrcolumns.Contains("projectspendname"))
                                {
                                    obj.ProjectSpendName = objDTO.ProjectSpendName;
                                }
                                if (arrcolumns.Contains("dollarlimitamount"))
                                {
                                    obj.DollarLimitAmount = objDTO.DollarLimitAmount ?? 0;
                                }
                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }

                                if (arrcolumns.Contains("trackallusageagainstthis"))
                                {
                                    obj.TrackAllUsageAgainstThis = objDTO.TrackAllUsageAgainstThis;
                                }
                                if (arrcolumns.Contains("isclosed"))
                                {
                                    obj.IsClosed = objDTO.IsClosed;
                                }
                                if (arrcolumns.Contains("isdeleted"))
                                {
                                    obj.IsDeleted = objDTO.IsDeleted;
                                    obj.IsArchived = objDTO.IsArchived;
                                }

                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Updated = objDTO.Updated;
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                obj.EditedFrom = "Web";
                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion
                    #region Work Order
                    else if (tableName == ImportMastersDTO.TableName.WorkOrder.ToString())
                    {
                        List<WorkOrderMain> AddWO = new List<WorkOrderMain>();
                        WorkOrderDAL objWorkOrderDAL = new WorkOrderDAL(base.DataBaseName);
                        CommonDAL objC = new CommonDAL(base.DataBaseName);
                        RoomDTO roomDTO = null;
                        var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomId, CompanyId, -1);
                        string cultureCode = "en-US";

                        if (regionInfo != null)
                        {
                            cultureCode = regionInfo.CultureCode;
                        }
                        
                        var workorderResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResWorkOrder", cultureCode, EnterpriseID, CompanyId);
                        string msgDuplicateWorkorderAlreadyExist = ResourceRead.GetResourceValueByKeyAndFullFilePath("DuplicateWorkorderAlreadyExist", workorderResourceFilePath, EnterpriseID, CompanyId, RoomId, "ResWorkOrder", cultureCode);

                        //-------------------------ADD-------------------------
                        //
                        List<WorkOrder> plist = (from d in context.WorkOrders where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false && !string.IsNullOrEmpty(d.WOName) select d).ToList();
                        foreach (WorkOrderMain objWorkOrderMain in (List<WorkOrderMain>)list)
                        {
                            WorkOrder objWorkOrder = (from W in plist
                                                      where W.WOName.Trim().ToUpper() == objWorkOrderMain.WOName.Trim().ToUpper()
                                                      && (string.IsNullOrEmpty(W.ReleaseNumber) ? "" : W.ReleaseNumber.Trim().ToUpper()) == (string.IsNullOrEmpty(objWorkOrderMain.ReleaseNumber) ? "" : objWorkOrderMain.ReleaseNumber.Trim().ToUpper())
                                                      select W).FirstOrDefault();
                            if (objWorkOrder != null && objWorkOrder.ID > 0)
                            {
                                //-------------------------Check Work Order Duplication-------------------------
                                //
                                string strOK = "";
                                string columnList = "ID,RoomName,IsAllowWorkOrdersDuplicate";
                                roomDTO = new eTurns.DAL.CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objWorkOrderMain.Room.GetValueOrDefault(0).ToString() + "", "");
                                //roomDTO = new eTurns.DAL.RoomDAL(base.DataBaseName).GetRoomByIDPlain((long)objWorkOrderMain.Room);

                                if (roomDTO.IsAllowWorkOrdersDuplicate != true)
                                {
                                    strOK = objC.DuplicateCheck(objWorkOrderMain.WOName, "edit", objWorkOrder.ID, "WorkOrder", "WOName", (long)objWorkOrderMain.Room, (long)objWorkOrderMain.CompanyID);
                                }

                                //------------------------------------------------------------------------------
                                //
                                if (strOK.ToUpper() == "DUPLICATE")
                                {
                                    objWorkOrderMain.Status = "Fail"; 
                                    objWorkOrderMain.Reason = msgDuplicateWorkorderAlreadyExist;
                                    lstreturnFinal.Add((T)Convert.ChangeType(objWorkOrderMain, typeof(T)));
                                }
                                else
                                {
                                    objWorkOrder.WOName = objWorkOrderMain.WOName;
                                    objWorkOrder.ReleaseNumber = objWorkOrderMain.ReleaseNumber;
                                    objWorkOrder.Description = objWorkOrderMain.Description;
                                    objWorkOrder.TechnicianID = objWorkOrderMain.TechnicianID;
                                    objWorkOrder.Technician = objWorkOrderMain.Technician;
                                    objWorkOrder.CustomerID = objWorkOrderMain.CustomerID;
                                    objWorkOrder.Customer = objWorkOrderMain.Customer;
                                    objWorkOrder.WOStatus = objWorkOrderMain.WOStatus;
                                    objWorkOrder.WOType = objWorkOrderMain.WOType;
                                    objWorkOrder.UDF1 = objWorkOrderMain.UDF1;
                                    objWorkOrder.UDF2 = objWorkOrderMain.UDF2;
                                    objWorkOrder.UDF3 = objWorkOrderMain.UDF3;
                                    objWorkOrder.UDF4 = objWorkOrderMain.UDF4;
                                    objWorkOrder.UDF5 = objWorkOrderMain.UDF5;
                                    objWorkOrder.Updated = DateTime.UtcNow;
                                    objWorkOrder.LastUpdatedBy = objWorkOrderMain.LastUpdatedBy;
                                    objWorkOrder.EditedFrom = objWorkOrder.EditedFrom;
                                    objWorkOrder.WhatWhereAction = objWorkOrder.WhatWhereAction;
                                    objWorkOrder.CustomerGUID = objWorkOrderMain.CustomerGUID;
                                    objWorkOrder.SupplierId = objWorkOrderMain.SupplierId;
                                    objWorkOrder.Asset = objWorkOrderMain.Asset;
                                    objWorkOrder.Odometer_OperationHours = objWorkOrderMain.Odometer_OperationHours;
                                    objWorkOrder.SupplierAccountGuid = objWorkOrderMain.SupplierAccountGuid;
                                    AssignAssetToWorkorder(objWorkOrder, RoomId, CompanyId, UserId);

                                    //objWorkOrder.AssetGUID = objWorkOrderMain.AssetGUID;
                                    //objWorkOrder.Asset = objWorkOrderMain.Asset;
                                    //objWorkOrder.ToolGUID = objWorkOrderMain.ToolGUID;
                                    //objWorkOrder.Tool = objWorkOrderMain.Tool;                                
                                    //objWorkOrder.Odometer_OperationHours = objWorkOrderMain.Odometer_OperationHours;                                
                                    //objWorkOrder.SignatureName = objWorkOrderMain.SignatureName;
                                    //objWorkOrder.IsSignatureCapture = objWorkOrderMain.IsSignatureCapture;
                                    //objWorkOrder.IsSignatureRequired = objWorkOrderMain.IsSignatureRequired;
                                }
                            }
                            else
                            {
                                //-------------------------Check Work Order Duplication-------------------------
                                //
                                string strOK = "";
                                // roomDTO = new eTurns.DAL.RoomDAL(base.DataBaseName).GetRoomByIDPlain((long)objWorkOrderMain.Room);
                                string columnList = "ID,RoomName,IsAllowWorkOrdersDuplicate";
                                roomDTO = new eTurns.DAL.CommonDAL(base.DataBaseName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objWorkOrderMain.Room.GetValueOrDefault(0).ToString() + "", "");


                                if (roomDTO.IsAllowWorkOrdersDuplicate != true)
                                {
                                    strOK = objC.DuplicateCheck(objWorkOrderMain.WOName, "add", 0, "WorkOrder", "WOName", (long)objWorkOrderMain.Room, (long)objWorkOrderMain.CompanyID);
                                }

                                //------------------------------------------------------------------------------
                                //
                                if (strOK.ToUpper() == "DUPLICATE")
                                {
                                    objWorkOrderMain.Status = "Fail";
                                    objWorkOrderMain.Reason = msgDuplicateWorkorderAlreadyExist;
                                    lstreturnFinal.Add((T)Convert.ChangeType(objWorkOrderMain, typeof(T)));
                                }
                                else
                                {
                                    objWorkOrderMain.ReleaseNumber = objWorkOrderDAL.GenerateAndGetReleaseNumber(objWorkOrderMain.WOName, 0, (long)objWorkOrderMain.Room, (long)objWorkOrderMain.CompanyID);
                                    objWorkOrderMain.Created = DateTime.UtcNow;
                                    objWorkOrderMain.CreatedBy = objWorkOrderMain.CreatedBy;

                                    AssignAssetToWorkorder(objWorkOrderMain, RoomId, CompanyId, UserId);

                                    AddWO.Add(objWorkOrderMain);
                                }
                            }
                        }

                        if (AddWO != null && AddWO.Count > 0)
                        {
                            lstFinalAdd = AddWO.Cast<T>().ToList();
                        }
                    }

                    #endregion
                    else if (tableName == ImportMastersDTO.TableName.PastMaintenanceDue.ToString())
                    {
                        lstFinalAdd = ((List<PastMaintenanceDueImport>)list).Cast<T>().ToList();

                    }

                    #region Asset Tool SchedulerMapping
                    else if (tableName == ImportMastersDTO.TableName.AssetToolSchedulerMapping.ToString())
                    {
                        lstFinalAdd = ((List<AssetToolSchedulerMapping>)list).Cast<T>().ToList();

                    }
                    #endregion

                    #region AssetToolScheduler
                    else if (tableName == ImportMastersDTO.TableName.AssetToolScheduler.ToString())
                    {
                        List<AssetToolScheduler> p = (from d in context.ToolsSchedulers
                                                      where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false
                                                      select new AssetToolScheduler
                                                      {
                                                          ScheduleFor = d.ScheduleFor,
                                                          SchedulerName = d.SchedulerName,
                                                          GUID = d.GUID
                                                      }).ToList();

                        List<AssetToolScheduler> identicalList = ((List<AssetToolScheduler>)list).GroupBy(x => new { x.SchedulerName, x.ScheduleFor, x.SchedulerType }).Select(s => s.First()).Cast<AssetToolScheduler>().ToList();

                        lstFinalAdd = identicalList.Where(a => !p.Select(b => new { b.SchedulerName, b.ScheduleFor }).Contains(new { a.SchedulerName, a.ScheduleFor })).Cast<T>().ToList();

                        lstEdit = identicalList.Where(a => p.Select(b => new { b.SchedulerName, b.ScheduleFor }).Contains(new { a.SchedulerName, a.ScheduleFor })).Cast<T>().ToList();




                        foreach (AssetToolScheduler objDTO in lstEdit.Cast<AssetToolScheduler>().ToList())
                        {
                            try
                            {
                                ToolsScheduler obj = (from m in context.ToolsSchedulers
                                                      where m.SchedulerName == objDTO.SchedulerName
                                                      && m.ScheduleFor == objDTO.ScheduleFor
                                                      && m.Room == RoomId && m.CompanyID == CompanyId && m.IsDeleted == false && m.IsArchived == false
                                                      select m).SingleOrDefault();


                                if (arrcolumns.Contains("schedulername"))
                                {
                                    obj.SchedulerName = objDTO.SchedulerName;
                                }

                                if (arrcolumns.Contains("description"))
                                {
                                    obj.Description = objDTO.Description;
                                }


                                if (arrcolumns.Contains("schedulertype"))
                                {
                                    obj.SchedulerType = objDTO.SchedulerType;
                                }

                                if (arrcolumns.Contains("timebasedunit"))
                                {
                                    obj.TimeBaseUnit = objDTO.TimeBaseUnit.GetValueOrDefault(0);
                                }

                                if (arrcolumns.Contains("timebasedfrequency"))
                                {
                                    obj.TimeBasedFrequency = objDTO.TimeBasedFrequency.GetValueOrDefault(0);
                                }

                                if (arrcolumns.Contains("checkouts"))
                                {
                                    obj.CheckOuts = objDTO.CheckOuts;
                                }

                                if (arrcolumns.Contains("operationalhours"))
                                {
                                    obj.OperationalHours = objDTO.OperationalHours;
                                }

                                if (arrcolumns.Contains("mileage"))
                                {
                                    obj.Mileage = objDTO.Mileage;
                                }

                                if (arrcolumns.Contains("udf1"))
                                {
                                    obj.UDF1 = objDTO.UDF1;
                                }
                                if (arrcolumns.Contains("udf2"))
                                {
                                    obj.UDF2 = objDTO.UDF2;
                                }
                                if (arrcolumns.Contains("udf3"))
                                {
                                    obj.UDF3 = objDTO.UDF3;
                                }
                                if (arrcolumns.Contains("udf4"))
                                {
                                    obj.UDF4 = objDTO.UDF4;
                                }
                                if (arrcolumns.Contains("udf5"))
                                {
                                    obj.UDF5 = objDTO.UDF5;
                                }


                                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                                obj.Updated = DateTimeUtility.DateTimeNow;

                                objDTO.Status = "Success";
                                objDTO.Reason = "N/A";

                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));
                            }
                            catch (Exception ex)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(objDTO, typeof(T)));

                            }
                        }
                    }
                    #endregion

                    if (tableName != ImportMastersDTO.TableName.ItemMaster.ToString() && tableName != ImportMastersDTO.TableName.EditItemMaster.ToString())
                    {
                        context.SaveChanges();
                    }
                }

                #region Insert Bulk Data

                try
                {
                    if (tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                    {
                        bulkCopy.DestinationTableName = "ItemMaster";
                    }
                    else if (tableName == ImportMastersDTO.TableName.LocationMaster.ToString())
                    {
                        bulkCopy.DestinationTableName = "LocationMaster";
                    }
                    else if(tableName == ImportMastersDTO.TableName.AssetToolScheduler.ToString())
                    {
                        bulkCopy.DestinationTableName = "ToolsScheduler";
                    }
                    else if(tableName == ImportMastersDTO.TableName.AssetToolSchedulerMapping.ToString())
                    {
                        bulkCopy.DestinationTableName = "ToolsSchedulerMapping";
                    }
                    else if(tableName == ImportMastersDTO.TableName.PastMaintenanceDue.ToString())
                    {
                        bulkCopy.DestinationTableName = "ToolsMaintenance";
                    }

                    var values = new object[props.Length];

                    foreach (var item in lstFinalAdd)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }
                        table.Rows.Add(values);
                    }
                    if (tableName == ImportMastersDTO.TableName.ItemMaster.ToString() || tableName == ImportMastersDTO.TableName.EditItemMaster.ToString())
                    {
                        table.AsEnumerable().Where(s => Convert.ToBoolean(Convert.ToString(s["IsActive"])) == true).ToList().ForEach(D => D.SetField("ItemIsActiveDate", DateTimeUtility.DateTimeNow));
                    }
                    bulkCopy.WriteToServer(table);

                    lstreturnFinal = GetUpdatedList(lstFinalAdd, tableName, "Success", lstreturnFinal, "N/A");
                    if (tableName == ImportMastersDTO.TableName.ItemMaster.ToString())
                    {
                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                        ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
                        ItemMasterDTO objItem = null;
                        DataTable ReturnDT = new DataTable("ItemLocationParam");
                        DataColumn[] arrColumns = new DataColumn[] {
                                                    new DataColumn() { AllowDBNull=true,ColumnName="ItemGUID",DataType=typeof(Guid)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="ItemNumber",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="BinID",DataType=typeof(Int64)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="Expiration",DataType=typeof(DateTime)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="LotNumber",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="ConsignedQuantity",DataType=typeof(float)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="CustomerOwnedQuantity",DataType=typeof(float)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)},
                                                    new DataColumn() { AllowDBNull=true,ColumnName="ProjectSpend",DataType=typeof(String)}
                        };
                        ReturnDT.Columns.AddRange(arrColumns);
                        DateTime datetimetoConsider = DateTime.UtcNow;  //new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
                        foreach (var item in lstFinalAdd.Cast<BOMItemMasterMain>().Where(e => e.ItemType != 4))
                        {
                            objItem = objItemMasterDAL.GetItemByGuidPlain(item.GUID, RoomId, CompanyId);

                            if (objItem != null && objItem.ID > 0 && !objItem.LotNumberTracking && !objItem.SerialNumberTracking && !objItem.DateCodeTracking && (item.OnHandQuantity ?? 0) > 0)
                            {
                                DataRow row = ReturnDT.NewRow();
                                row["ItemGUID"] = objItem.GUID;
                                row["ItemNumber"] = objItem.ItemNumber;
                                row["BinID"] = (objItem.DefaultLocation ?? 0) > 0 ? (object)objItem.DefaultLocation : DBNull.Value;
                                row["BinNumber"] = DBNull.Value;
                                row["Expiration"] = DBNull.Value;
                                row["Received"] = datetimetoConsider;
                                row["LotNumber"] = DBNull.Value;
                                row["SerialNumber"] = DBNull.Value;
                                row["ConsignedQuantity"] = (item.Consignment ? item.OnHandQuantity : 0);
                                row["CustomerOwnedQuantity"] = (item.Consignment ? 0 : item.OnHandQuantity);
                                row["ReceiptCost"] = (item.Cost ?? 0) > 0 ? (object)item.Cost : DBNull.Value;
                                row["UDF1"] = string.Empty;
                                row["UDF2"] = string.Empty;
                                row["UDF3"] = string.Empty;
                                row["UDF4"] = string.Empty;
                                row["UDF5"] = string.Empty;
                                row["ProjectSpend"] = string.Empty;
                                ReturnDT.Rows.Add(row);




                                //BinMasterDTO objBin = new BinMasterDTO();
                                //if (!string.IsNullOrWhiteSpace(item.InventryLocation))
                                //{
                                //    objBin = objItemLocationDetailsDAL.GetItemBin(item.GUID, item.InventryLocation, RoomId, CompanyId, UserId, false);
                                //    if (objBin == null)
                                //    {
                                //        objBin = new BinMasterDTO();
                                //    }
                                //}


                                //ItemLocationDetailsDTO objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                //objItemLocationDetailsDTO.Room = RoomId;
                                //objItemLocationDetailsDTO.CompanyID = CompanyId;
                                //objItemLocationDetailsDTO.ItemGUID = item.GUID;
                                //objItemLocationDetailsDTO.BinID = objItem.DefaultLocation;
                                //objItemLocationDetailsDTO.BinNumber = objItem.InventryLocation;
                                //objItemLocationDetailsDTO.CustomerOwnedQuantity = item.Consignment ? 0 : item.OnHandQuantity;
                                //objItemLocationDetailsDTO.ConsignedQuantity = item.Consignment ? item.OnHandQuantity : 0;
                                //objItemLocationDetailsDTO.SerialNumber = null;
                                //objItemLocationDetailsDTO.LotNumber = null;
                                //objItemLocationDetailsDTO.Expiration = null;
                                //objItemLocationDetailsDTO.Received = DateTime.UtcNow.ToString("MM/dd/yyyy");
                                //objItemLocationDetailsDTO.IsDeleted = false;
                                //objItemLocationDetailsDTO.IsArchived = false;
                                //objItemLocationDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                                //objItemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                //objItemLocationDetailsDTO.LastUpdatedBy = UserId;
                                //objItemLocationDetailsDTO.CreatedBy = UserId;
                                //objItemLocationDetailsDTO.GUID = Guid.NewGuid();
                                //objItemLocationDetailsDTO.InsertedFrom = "import";
                                //objItemLocationDetailsDTO.Cost = item.Cost;
                                //objItemLocationDetailsDAL.ItemLocationDetailsImportSave(objItemLocationDetailsDTO);
                            }
                        }
                        if (ReturnDT != null && ReturnDT.Rows.Count > 0)
                        {
                            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                            SqlConnection ChildDbConnection = new SqlConnection(Connectionstring);
                            DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "ItemMasterImportIL", RoomId, CompanyId, UserId, ReturnDT);
                        }
                        if (lstItemGUID != null && lstItemGUID.Count > 0 && CompanyId > 0 && RoomId > 0 && UserId > 0)
                        {
                            string ItemGUID = "";
                            string ItemBlanketPOId = "";
                            string ItemMinMaxQty = "";
                            foreach (ItemGUIDLocation objItemGuidU in lstItemGUID)
                            {
                                if (objItemGuidU.ItemGUID != null)
                                {
                                    //UpdateItemMSLDetail(objItemGuidU.ItemGUID, CompanyId, RoomId, UserId, objItemGuidU.LocationName, objItemGuidU.CriticalQuantity, objItemGuidU.MinimumQuantity, objItemGuidU.MaximumQuantity, objItemGuidU.DefaultBlanketPOID);
                                    ItemGUID = ItemGUID + (ItemGUID == "" ? "" : ",") + objItemGuidU.ItemGUID.ToString();
                                    ItemBlanketPOId = ItemBlanketPOId + (ItemBlanketPOId == "" ? "" : ",") + objItemGuidU.ItemGUID.ToString() + "#" + objItemGuidU.DefaultBlanketPOID.ToString();
                                    ItemMinMaxQty = ItemMinMaxQty + (ItemMinMaxQty == "" ? "" : ",") + objItemGuidU.ItemGUID.ToString() + "#" + objItemGuidU.CriticalQuantity.ToString() + "#" + objItemGuidU.MinimumQuantity.ToString() + "#" + objItemGuidU.MaximumQuantity.ToString();
                                }
                            }

                            if (!string.IsNullOrEmpty(ItemGUID) && !string.IsNullOrWhiteSpace(ItemGUID))
                            {
                                SetDefaultManufacturerBulk(ItemGUID, UserId, RoomId, CompanyId);
                            }

                            if (!string.IsNullOrEmpty(ItemBlanketPOId) && !string.IsNullOrWhiteSpace(ItemBlanketPOId))
                            {
                                SetDefaultSupplierBulk(ItemBlanketPOId, UserId, RoomId, CompanyId);
                            }

                            if (!string.IsNullOrEmpty(ItemMinMaxQty) && !string.IsNullOrWhiteSpace(ItemMinMaxQty))
                            {
                                SetDefaultLocationBulk(ItemMinMaxQty, UserId, RoomId, CompanyId);
                                foreach (ItemGUIDLocation objItemGuidU in lstItemGUID)
                                {
                                    if (objItemGuidU.ItemGUID != null)
                                    {
                                        BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
                                        objBinDAL.UpdateItemBinOrderUOM(objItemGuidU.ItemGUID, RoomId, CompanyId, true);
                                    }
                                }

                            }
                        }
                    }
                    else if (tableName == ImportMastersDTO.TableName.EditItemMaster.ToString())
                    {
                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                        
                        if (lstItemGUID != null && lstItemGUID.Count > 0 && CompanyId > 0 && RoomId > 0 && UserId > 0)
                        {
                            string ItemGUID = "";
                            string ItemBlanketPOId = "";
                            string ItemMinMaxQty = "";
                            foreach (ItemGUIDLocation objItemGuidU in lstItemGUID)
                            {
                                if (objItemGuidU.ItemGUID != null)
                                {
                                    ItemGUID = ItemGUID + (ItemGUID == "" ? "" : ",") + objItemGuidU.ItemGUID.ToString();
                                    ItemBlanketPOId = ItemBlanketPOId + (ItemBlanketPOId == "" ? "" : ",") + objItemGuidU.ItemGUID.ToString() + "#" + objItemGuidU.DefaultBlanketPOID.ToString();
                                    ItemMinMaxQty = ItemMinMaxQty + (ItemMinMaxQty == "" ? "" : ",") + objItemGuidU.ItemGUID.ToString() + "#" + objItemGuidU.CriticalQuantity.ToString() + "#" + objItemGuidU.MinimumQuantity.ToString() + "#" + objItemGuidU.MaximumQuantity.ToString();
                                }
                            }

                            if (!string.IsNullOrEmpty(ItemGUID) && !string.IsNullOrWhiteSpace(ItemGUID))
                            {
                                SetDefaultManufacturerBulk(ItemGUID, UserId, RoomId, CompanyId);
                            }
                        }
                    }
                    else if (tableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                    {
                        foreach (ItemGUIDLocation objItemGuidU in lstItemGUID)
                        {
                            if (CompanyId > 0 && UserId > 0 && objItemGuidU.ItemGUID != null)
                            {
                                UpdateBOMItemMSLDetail(objItemGuidU.ItemGUID, CompanyId, UserId);
                            }
                        }
                    }
                    else if (tableName == ImportMastersDTO.TableName.ToolMaster.ToString())
                    {

                        if (_AllowToolOrdering)
                        {
                            List<ToolMasterDTO> objtool = new ToolMasterDAL(base.DataBaseName).GetToolByRoomPlain(RoomId, CompanyId).ToList();

                            foreach (var item in lstFinalAdd.Cast<ToolMasterMain>())
                            {

                                ToolMasterDAL objToolDAL = new ToolMasterDAL(base.DataBaseName);
                                ToolMasterDTO objToolMasterDTO = objtool.Where(t => t.ToolName == item.ToolName && t.Serial == item.Serial && t.Room == item.Room && t.IsDeleted == false).FirstOrDefault();
                                if (objToolMasterDTO != null)
                                {

                                    ToolLocationDetailsDTO objToolLocationDetailsDTO = null;
                                    ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                                    LocationMasterDTO objLocationMasterDTO = null;
                                    if (objToolMasterDTO.LocationID.GetValueOrDefault(0) <= 0)
                                    {
                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objToolMasterDTO.GUID, string.Empty, objToolMasterDTO.Room ?? 0, objToolMasterDTO.CompanyID ?? 0, UserId, "ImportController>>CheckOutQty(Saveimport)");
                                    }
                                    else
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(base.DataBaseName);
                                        objLocationMasterDTO = objLocationCntrl.GetLocationByIDPlain(objToolMasterDTO.LocationID.GetValueOrDefault(0), objToolMasterDTO.Room ?? 0, objToolMasterDTO.CompanyID ?? 0);
                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objToolMasterDTO.GUID, objLocationMasterDTO.Location, objToolMasterDTO.Room ?? 0, objToolMasterDTO.CompanyID ?? 0, UserId, "ImportController>>CheckOutQty(Saveimport)");

                                    }

                                    if (objToolMasterDTO.Quantity > 0 && !objToolMasterDTO.SerialNumberTracking)
                                    {

                                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                        objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                                        objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : ((objLocationMasterDTO != null) ? objLocationMasterDTO.ID : 0);
                                        objToolAssetQuantityDetailDTO.Quantity = objToolMasterDTO.Quantity;
                                        objToolAssetQuantityDetailDTO.RoomID = RoomId;
                                        objToolAssetQuantityDetailDTO.CompanyID = CompanyId;
                                        objToolAssetQuantityDetailDTO.Created = objToolMasterDTO.Created;
                                        objToolAssetQuantityDetailDTO.Updated = objToolMasterDTO.Updated ?? DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = objToolMasterDTO.ReceivedOnWeb;
                                        objToolAssetQuantityDetailDTO.ReceivedOn = objToolMasterDTO.ReceivedOn;
                                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.WhatWhereAction = "ImportController>>ToolSave";
                                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolMasterDTO.Quantity;
                                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Created From Web Page Import(using Tool Master). insert Entry of Tool.";
                                        objToolAssetQuantityDetailDTO.CreatedBy = UserId;
                                        objToolAssetQuantityDetailDTO.UpdatedBy = UserId;
                                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                                        objToolAssetQuantityDetailDTO.IsArchived = false;

                                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);
                                        objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, "AdjCredit", ReferalAction: "Initial Tool Create");
                                    }
                                }
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


                #endregion
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

        /// <summary>
        /// This method is used to set/link asset with workorder (WI-4657)
        /// </summary>
        /// <param name="workorder"></param>
        /// <param name="roomId"></param>
        /// <param name="companyId"></param>
        private void AssignAssetToWorkorder<T>(T objWo, long roomId, long companyId, long userId)
        {
            string assetName = "";

            if (objWo is WorkOrder)
            {
                assetName = (objWo as WorkOrder).Asset;
            }
            else
            {
                assetName = (objWo as WorkOrderMain).Asset;
            }

            if (!string.IsNullOrEmpty(assetName))
            {
                assetName = assetName.Trim();
            }

            if (objWo != null && !string.IsNullOrEmpty(assetName))
            {
                AssetMasterDAL assetMaster = new AssetMasterDAL(base.DataBaseName);
                AssetMasterDTO asset = new AssetMasterDTO();

                asset = assetMaster.GetAssetByName(assetName, roomId, companyId);

                if (asset != null && asset.GUID != null && asset.GUID != Guid.Empty)
                {
                    if (objWo is WorkOrder)
                    {
                        (objWo as WorkOrder).AssetGUID = asset.GUID;
                    }
                    else
                    {
                        (objWo as WorkOrderMain).AssetGUID = asset.GUID;
                    }
                }
                else
                {
                    asset = new AssetMasterDTO();
                    asset.ID = 0;
                    asset.CreatedBy = userId;
                    asset.LastUpdatedBy = userId;
                    asset.Room = roomId;
                    asset.CompanyID = companyId;
                    asset.AddedFrom = "Web";
                    asset.EditedFrom = "Web";
                    asset.ReceivedOn = DateTimeUtility.DateTimeNow;
                    asset.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    asset.AssetName = assetName;
                    asset.GUID = Guid.NewGuid();
                    asset.MaintenanceType = 0;
                    var assetId = assetMaster.Insert(asset);

                    if (assetId > 0)
                    {
                        if (objWo is WorkOrder)
                        {
                            (objWo as WorkOrder).AssetGUID = asset.GUID;
                        }
                        else
                        {
                            (objWo as WorkOrderMain).AssetGUID = asset.GUID;
                        }
                    }
                }
            }
        }
        public void UpdateItemMSLDetail(Guid ItemGUID, long CompanyId, long RoomId, long UserId, string ItemLocationName, double CriticalQuantity, double MinimumQuantity, double MaximumQuantity, long BlanketPOID)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "USP_UpdateItemMSLDetail_Import", ItemGUID, UserId, RoomId, CompanyId, ItemLocationName, CriticalQuantity, MinimumQuantity, MaximumQuantity, BlanketPOID);
        }
        public void SetDefaultManufacturerBulk(string ItemGUID, long UserId, long RoomId, long CompanyId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "csp_SetDefaultManufacturer", ItemGUID, UserId, RoomId, CompanyId);
        }
        public void SetDefaultSupplierBulk(string ItemBlanketPOId, long UserId, long RoomId, long CompanyId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "csp_SetDefaultSupplier", ItemBlanketPOId, UserId, RoomId, CompanyId);
        }

        public void SetDefaultLocationBulk(string ItemMinMaxQty, long UserId, long RoomId, long CompanyId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "csp_SetDefaultLocation", ItemMinMaxQty, UserId, RoomId, CompanyId);
        }

        public void UpdateBOMItemMSLDetail(Guid ItemGUID, long CompanyId, long UserId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "USP_UpdateBOMItemMSLDetail", ItemGUID, UserId, CompanyId);
        }

        public List<UDFOptionsCheckDTO> GetAllUDFList(Int64 RoomID, string TableName)
        {
            List<UDFOptionsCheckDTO> lst = new List<UDFOptionsCheckDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@TableName", TableName) };
                lst = context.Database.SqlQuery<UDFOptionsCheckDTO>("exec [GetAllUDFList] @RoomID,@TableName", params1).ToList();

                //lst = (from u in context.Database.SqlQuery<UDFOptionsCheckDTO>(@"Select ISNULL(A.ID,0) AS UDFID, UO.UDFOption , A.UDFColumnName " +
                //"from UDF A LEFT OUTER JOIN UDFOptions UO ON UO.UDFID = A.ID " +
                //"where A.room=" + RoomID.ToString() + " and A.UDFTableName='" + TableName + "' and A.IsDeleted=0  " +
                //"AND (UO.IsDeleted = 0 OR UO.IsDeleted IS NULL) ")
                //       select new UDFOptionsCheckDTO
                //       {
                //           UDFOption = u.UDFOption,
                //           UDFID = u.UDFID,
                //           UDFColumnName = u.UDFColumnName
                //       }).ToList();
            }
            return lst;
        }
        public void UpdateDefaultLocation(long BinId, Guid ItemGUID, long UserId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "USP_UpdateDefaultLocation", BinId, ItemGUID, UserId);
        }
        public List<PullImportWithSameQty> PullImportWithSameQty(DataTable PullMasterTable, Int64 RoomID, Int64 CompanyID, Int64 UserID, string EditedFrom, string WhatwhereAction)
        {
            try
            {
                //List<PullImportWithSameQty> lstPullImportWithSameQty = new List<PullImportWithSameQty>();
                //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                //{

                //    var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@UserID", UserID), new SqlParameter("@EditedFrom", EditedFrom), new SqlParameter("@WhatwhereAction", WhatwhereAction), new SqlParameter("@PullMasterTable", PullMasterTable) };
                //    return context.Database.SqlQuery<PullImportWithSameQty>("exec [PullImportWithSameQty] @RoomID,@CompanyID,@UserID,@EditedFrom,@WhatwhereAction,@PullMasterTable", params1).ToList();



                //}


                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<PullImportWithSameQty> lstPullImportWithSameQty = new List<PullImportWithSameQty>();
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "PullImportWithSameQty", RoomID, CompanyID, UserID, EditedFrom, WhatwhereAction, PullMasterTable);
                if (dsBins.Tables.Count > 0)
                {
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                PullImportWithSameQty objPull = new PullImportWithSameQty();
                                if (dt.Columns.Contains("Reason"))
                                {
                                    objPull.Reason = Convert.ToString(dr["Reason"]);
                                }
                                if (dt.Columns.Contains("Status"))
                                {
                                    objPull.Status = Convert.ToString(dr["Status"]);
                                }
                                if (dt.Columns.Contains("ID"))
                                {
                                    objPull.ID = Convert.ToInt32(dr["ID"]);
                                }
                                if (dt.Columns.Contains("ItemNumber"))
                                {
                                    objPull.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                                }
                                if (dt.Columns.Contains("PullQuantity"))
                                {
                                    objPull.PullQuantity = Convert.ToDouble(dr["PullQuantity"]);
                                }
                                if (dt.Columns.Contains("BinNumber"))
                                {
                                    objPull.BinNumber = Convert.ToString(dr["BinNumber"]);
                                }
                                if (dt.Columns.Contains("UDF1"))
                                {
                                    objPull.UDF1 = Convert.ToString(dr["UDF1"]);
                                }
                                if (dt.Columns.Contains("UDF2"))
                                {
                                    objPull.UDF2 = Convert.ToString(dr["UDF2"]);
                                }
                                if (dt.Columns.Contains("UDF3"))
                                {
                                    objPull.UDF3 = Convert.ToString(dr["UDF3"]);
                                }
                                if (dt.Columns.Contains("UDF4"))
                                {
                                    objPull.UDF4 = Convert.ToString(dr["UDF4"]);
                                }
                                if (dt.Columns.Contains("UDF5"))
                                {
                                    objPull.UDF5 = Convert.ToString(dr["UDF5"]);
                                }
                                if (dt.Columns.Contains("ProjectSpendName"))
                                {
                                    objPull.ProjectSpendName = Convert.ToString(dr["ProjectSpendName"]);
                                }
                                if (dt.Columns.Contains("PullOrderNumber"))
                                {
                                    objPull.PullOrderNumber = Convert.ToString(dr["PullOrderNumber"]);
                                }
                                if (dt.Columns.Contains("WorkOrder"))
                                {
                                    objPull.WorkOrder = Convert.ToString(dr["WorkOrder"]);
                                }
                                if (dt.Columns.Contains("Asset"))
                                {
                                    objPull.Asset = Convert.ToString(dr["Asset"]);
                                }
                                if (dt.Columns.Contains("ActionType"))
                                {
                                    objPull.ActionType = Convert.ToString(dr["ActionType"]);
                                }
                                if (dt.Columns.Contains("Created"))
                                {
                                    objPull.Created = Convert.ToString(dr["Created"]);
                                }
                                if (dt.Columns.Contains("ItemCost"))
                                {
                                    objPull.ItemCost = Convert.ToString(dr["ItemCost"]);
                                }
                                if (dt.Columns.Contains("CostUOMValue"))
                                {
                                    objPull.CostUOMValue = Convert.ToString(dr["CostUOMValue"]);
                                }
                                lstPullImportWithSameQty.Add(objPull);
                            }
                        }
                    }
                }
                return lstPullImportWithSameQty;


            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// This method is used to import the ToolcheckOutCheckInHistory data
        /// </summary>
        /// <param name="ToolCheckOutCheckInTable"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <param name="AddedFrom"></param>        
        /// <returns></returns>
        public List<ToolCheckInCheckOut> ImportToolCheckInCheckOutHistory(DataTable ToolCheckOutCheckInTable, long RoomID, long CompanyID, long UserID, string AddedFrom, bool allowToolOrdering)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<ToolCheckInCheckOut> lstToolCheckInCheckOut = new List<ToolCheckInCheckOut>();
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                string spName = allowToolOrdering ? "ImportToolCheckInCheckOutHitory_New" : "ImportToolCheckInCheckOutHitory";
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, spName, RoomID, CompanyID, UserID, AddedFrom, ToolCheckOutCheckInTable);

                if (dsBins.Tables.Count > 0)
                {
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[dsBins.Tables.Count - 1];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                ToolCheckInCheckOut toolCheckOutCheckIn = new ToolCheckInCheckOut();

                                if (dt.Columns.Contains("Id"))
                                {
                                    toolCheckOutCheckIn.Id = Convert.ToInt32(dr["Id"]);
                                }
                                if (allowToolOrdering)
                                {
                                    if (dt.Columns.Contains("ToolName"))
                                    {
                                        toolCheckOutCheckIn.ToolName = Convert.ToString(dr["ToolName"]);
                                    }
                                }
                                if (dt.Columns.Contains("Serial"))
                                {
                                    toolCheckOutCheckIn.Serial = Convert.ToString(dr["Serial"]);
                                }

                                if (dt.Columns.Contains("Location"))
                                {
                                    toolCheckOutCheckIn.Location = Convert.ToString(dr["Location"]);
                                }

                                if (dt.Columns.Contains("TechnicianCode"))
                                {
                                    toolCheckOutCheckIn.TechnicianCode = Convert.ToString(dr["TechnicianCode"]);
                                }
                                if (dt.Columns.Contains("Quantity"))
                                {
                                    toolCheckOutCheckIn.Quantity = Convert.ToDouble(dr["Quantity"]);
                                }
                                if (dt.Columns.Contains("Operation"))
                                {
                                    toolCheckOutCheckIn.Operation = Convert.ToString(dr["Operation"]);
                                }
                                if (dt.Columns.Contains("CheckOutUDF1"))
                                {
                                    toolCheckOutCheckIn.CheckOutUDF1 = Convert.ToString(dr["CheckOutUDF1"]);
                                }
                                if (dt.Columns.Contains("CheckOutUDF2"))
                                {
                                    toolCheckOutCheckIn.CheckOutUDF2 = Convert.ToString(dr["CheckOutUDF2"]);
                                }
                                if (dt.Columns.Contains("CheckOutUDF3"))
                                {
                                    toolCheckOutCheckIn.CheckOutUDF3 = Convert.ToString(dr["CheckOutUDF3"]);
                                }
                                if (dt.Columns.Contains("CheckOutUDF4"))
                                {
                                    toolCheckOutCheckIn.CheckOutUDF4 = Convert.ToString(dr["CheckOutUDF4"]);
                                }
                                if (dt.Columns.Contains("CheckOutUDF5"))
                                {
                                    toolCheckOutCheckIn.CheckOutUDF5 = Convert.ToString(dr["CheckOutUDF5"]);
                                }
                                if (dt.Columns.Contains("Reason"))
                                {
                                    toolCheckOutCheckIn.Reason = Convert.ToString(dr["Reason"]);
                                }
                                if (dt.Columns.Contains("Status"))
                                {
                                    toolCheckOutCheckIn.Status = string.IsNullOrEmpty(Convert.ToString(dr["Status"]))
                                        ? (string.IsNullOrEmpty(Convert.ToString(dr["Reason"])) ? "Success" : "Fail")
                                        : Convert.ToString(dr["Status"]);
                                }

                                lstToolCheckInCheckOut.Add(toolCheckOutCheckIn);
                            }
                        }
                    }
                }
                return lstToolCheckInCheckOut;
            }
            catch
            {
                return null;
            }
        }

        public List<ToolImageImport> ImportToolCertificationImages(DataTable ToolCertificationImagesTable, long RoomID, long CompanyID, long UserID, string AddedFrom)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<ToolImageImport> toolCertificationImages = new List<ToolImageImport>();
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "ImportToolCertificationImages", RoomID, CompanyID, UserID, AddedFrom, ToolCertificationImagesTable);

                if (dsBins.Tables.Count > 0)
                {
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[dsBins.Tables.Count - 1];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                ToolImageImport toolImage = new ToolImageImport();

                                if (dt.Columns.Contains("Id"))
                                {
                                    toolImage.Id = Convert.ToInt32(dr["Id"]);
                                }
                                if (dt.Columns.Contains("ToolName"))
                                {
                                    toolImage.ToolName = Convert.ToString(dr["ToolName"]);
                                }
                                if (dt.Columns.Contains("Serial"))
                                {
                                    toolImage.Serial = Convert.ToString(dr["Serial"]);
                                }
                                if (dt.Columns.Contains("ImageName"))
                                {
                                    toolImage.ImageName = Convert.ToString(dr["ImageName"]);
                                }
                                if (dt.Columns.Contains("Reason"))
                                {
                                    toolImage.Reason = Convert.ToString(dr["Reason"]);
                                }
                                if (dt.Columns.Contains("Status"))
                                {
                                    toolImage.Status = string.IsNullOrEmpty(Convert.ToString(dr["Status"]))
                                        ? (string.IsNullOrEmpty(Convert.ToString(dr["Reason"])) ? "Success" : "Fail")
                                        : Convert.ToString(dr["Status"]);
                                }

                                toolCertificationImages.Add(toolImage);
                            }
                        }
                    }
                }
                return toolCertificationImages;
            }
            catch
            {
                return null;
            }
        }

        public bool EditItemMaster(BOMItemMasterMain objDTO, string[] arrColumns, long SessionUserId, bool isImgZipAvail = false, bool isLink2ZipAvail = false)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;


            ItemMasterDAL objItemMaster = new ItemMasterDAL(base.DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                //ItemMaster obj = (from m in context.ItemMasters
                //                  where (m.ItemNumber ?? string.Empty).Trim().ToLower() == (objDTO.ItemNumber ?? string.Empty).Trim().ToLower() && m.Room == objDTO.Room 
                //                  && m.ItemType == objDTO.ItemType && m.CompanyID == objDTO.CompanyID && m.IsArchived == false && m.IsDeleted == false
                //                  select m).FirstOrDefault();
                ItemMaster obj = (from m in context.ItemMasters
                                  where (m.ItemNumber ?? string.Empty).Trim().ToLower() == (objDTO.ItemNumber ?? string.Empty).Trim().ToLower() && m.Room == objDTO.Room && m.CompanyID == objDTO.CompanyID && m.IsArchived == false && m.IsDeleted == false
                                  select m).FirstOrDefault();
                if (obj != null)
                {

                    if (objDTO.ManufacturerID > 0)
                        obj.ManufacturerID = objDTO.ManufacturerID;
                    if (arrColumns.Contains("manufacturernumber"))
                    {
                        obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                    }
                    if (!obj.IsActive && objDTO.IsActive)
                    {
                        obj.ItemIsActiveDate = DateTimeUtility.DateTimeNow;
                    }
                    else if (obj.IsActive && objDTO.IsActive)
                    {
                        obj.ItemIsActiveDate = obj.ItemIsActiveDate;
                    }

                    else if (!objDTO.IsActive)
                    {
                        obj.ItemIsActiveDate = null;
                    }
                    if (arrColumns.Contains("isactive"))
                    {
                        obj.IsActive = objDTO.IsActive;
                        obj.IsOrderable = objDTO.IsOrderable;
                    }
                    
                    if (objDTO.SupplierID > 0)
                        obj.SupplierID = objDTO.SupplierID;
                    if (arrColumns.Contains("supplierpartno"))
                    {
                        obj.SupplierPartNo = objDTO.SupplierPartNo;
                    }
                    if (arrColumns.Contains("isallowordercostuom"))
                    {
                        obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                    }
                    obj.ImageType = objDTO.ImageType;
                    obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;

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

                    if (arrColumns.Contains("longdescription"))
                    {
                        obj.LongDescription = objDTO.LongDescription;
                    }

                    if (arrColumns.Contains("categoryname"))
                    {
                        if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
                            obj.CategoryID = objDTO.CategoryID;
                    }

                    if (arrColumns.Contains("glaccount"))
                    {
                        if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
                            obj.GLAccountID = objDTO.GLAccountID;
                    }

                    if (arrColumns.Contains("unit") || arrColumns.Contains("uom"))
                    {
                        if (objDTO.UOMID > 0)
                            obj.UOMID = objDTO.UOMID;
                    }

                    //if (arrColumns.Contains("priceperterm"))
                    //{
                    //    obj.PricePerTerm = objDTO.PricePerTerm;
                    //}

                    if (arrColumns.Contains("costuom"))
                    {
                        obj.CostUOMID = objDTO.CostUOMID;
                    }

                    if (arrColumns.Contains("defaultreorderquantity"))
                    {
                        obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                    }

                    if (arrColumns.Contains("defaultpullquantity"))
                    {
                        obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                    }

                    if (arrColumns.Contains("cost"))
                    {
                        obj.Cost = objDTO.Cost;
                    }

                    if (arrColumns.Contains("markup"))
                    {
                        obj.Markup = objDTO.Markup;
                    }

                    if (arrColumns.Contains("sellprice"))
                    {
                        obj.SellPrice = objDTO.SellPrice;
                    }

                    //obj.ExtendedCost = objDTO.ExtendedCost;
                    if (arrColumns.Contains("leadtimeindays"))
                    {
                        obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                    }

                    if (arrColumns.Contains("link1"))
                    {
                        obj.Link1 = objDTO.Link1;
                    }

                    if (arrColumns.Contains("link2"))
                    {
                        if (string.IsNullOrEmpty(objDTO.Link2))
                        {
                            obj.Link2 = string.Empty;
                            obj.ItemLink2ImageType = "InternalLink";
                        }
                        else
                        {
                            obj.Link2 = objDTO.Link2;
                            obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                        }
                        //obj.Link2 = objDTO.Link2;
                    }

                    if (arrColumns.Contains("itemimageexternalurl"))
                    {
                        obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                    }

                    if (arrColumns.Contains("itemdocexternalurl"))
                    {
                        obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;
                    }

                    if (arrColumns.Contains("trend"))
                    {
                        obj.Trend = objDTO.Trend;
                    }

                    if (arrColumns.Contains("taxable"))
                    {
                        obj.Taxable = objDTO.Taxable;
                    }

                    if (arrColumns.Contains("consignment"))
                    {
                        obj.Consignment = objDTO.Consignment;
                    }
                    if (arrColumns.Contains("itemlink2externalurl"))
                    {
                        obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                    }


                    //if (arrColumns.Contains("stagedquantity"))
                    //{
                    //    obj.StagedQuantity = objDTO.StagedQuantity;
                    //}

                    //if (arrColumns.Contains("intransitquantity"))
                    //{
                    //    obj.InTransitquantity = objDTO.InTransitquantity;
                    //}

                    //if (arrColumns.Contains("onorderquantity"))
                    //{
                    //    obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                    //}

                    //if (arrColumns.Contains("ontransferquantity"))
                    //{
                    //    obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                    //}

                    //if (arrColumns.Contains("suggestedorderquantity"))
                    //{
                    //    obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                    //}

                    //if (arrColumns.Contains("requisitionedquantity"))
                    //{
                    //    obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                    //}

                    //if (arrColumns.Contains("packingquantity"))
                    //{
                    //    obj.PackingQuantity = objDTO.PackingQuantity;
                    //}

                    //if (arrColumns.Contains("averageusage"))
                    //{
                    //    obj.AverageUsage = objDTO.AverageUsage;
                    //}

                    //if (arrColumns.Contains("turns"))
                    //{
                    //    obj.Turns = objDTO.Turns;
                    //}

                    //if (arrColumns.Contains("onhandquantity"))
                    //{
                    //    obj.OnHandQuantity = objDTO.OnHandQuantity;
                    //}
                    if (obj.IsItemLevelMinMaxQtyRequired == true)
                    {
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
                    }
                    else
                    {
                        obj.CriticalQuantity = 0;
                        obj.MinimumQuantity = 0;
                        obj.MaximumQuantity = 0;
                    }
                    if (arrColumns.Contains("weightperpiece"))
                    {
                        obj.WeightPerPiece = objDTO.WeightPerPiece;
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
                    if (arrColumns.Contains("ispackslipmandatoryatreceive"))
                    {
                        obj.IsPackslipMandatoryAtReceive = objDTO.IsPackslipMandatoryAtReceive;
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
                    if (arrColumns.Contains("trendingsetting"))
                    {
                        obj.TrendingSetting = objDTO.TrendingSetting;
                    }
                    if (arrColumns.Contains("enforcedefaultpullquantity"))
                    {
                        obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                    }
                    if (arrColumns.Contains("enforcedefaultreorderquantity"))
                    {
                        obj.IsEnforceDefaultReorderQuantity = objDTO.IsEnforceDefaultReorderQuantity;
                    }
                    if (arrColumns.Contains("isautoinventoryclassification"))
                    {
                        obj.IsAutoInventoryClassification = objDTO.IsAutoInventoryClassification;
                    }
                    //if (arrColumns.Contains("isbuildbreak"))
                    //{
                    //    obj.IsBuildBreak = objDTO.IsBuildBreak;
                    //}
                    //if (arrColumns.Contains("itemtype"))
                    //{
                    //    obj.ItemType = objDTO.ItemType;
                    //}
                    //wi-3351
                    if (arrColumns.Contains("imagepath"))
                    {
                        if (string.IsNullOrEmpty(objDTO.ImagePath))
                        {
                            obj.ImagePath = string.Empty;
                            obj.ImageType = "ExternalImage";
                        }
                        else
                        {
                            obj.ImagePath = objDTO.ImagePath;
                            obj.ImageType = objDTO.ImageType;
                        }
                        // obj.ImagePath = objDTO.ImagePath;
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
                    if (arrColumns.Contains("udf6"))
                    {
                        obj.UDF6 = objDTO.UDF6;
                    }

                    if (arrColumns.Contains("udf7"))
                    {
                        obj.UDF7 = objDTO.UDF7;
                    }

                    if (arrColumns.Contains("udf8"))
                    {
                        obj.UDF8 = objDTO.UDF8;
                    }

                    if (arrColumns.Contains("udf9"))
                    {
                        obj.UDF9 = objDTO.UDF9;
                    }

                    if (arrColumns.Contains("udf10"))
                    {
                        obj.UDF10 = objDTO.UDF10;
                    }

                    //obj.GUID = objDTO.GUID;
                    //obj.Created = objDTO.Created;
                    obj.Updated = objDTO.Updated;
                    //obj.CreatedBy = objDTO.CreatedBy;

                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;

                    //obj.IsDeleted = false;
                    //obj.IsArchived = false;
                    //obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                    //obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                    //obj.CompanyID = objDTO.CompanyID;
                    // obj.Room = objDTO.Room;
                    if (arrColumns.Contains("islotserialexpirycost"))
                    {
                        obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                    }
                    if (arrColumns.Contains("itemimageexternalurl"))
                    {
                        if (!string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
                            obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                    }

                    // obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                    //obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                    //obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                    // Get ext cost based on Item Location details START
                    //eTurns.DAL.ItemMasterDAL.CostDTO ObjCostDTO = objItemMaster.GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                    //obj.ExtendedCost = ObjCostDTO.ExtCost;
                    //obj.AverageCost = ObjCostDTO.AvgCost;
                    //objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                    //objDTO.AverageCost = ObjCostDTO.AvgCost;
                    // Get ext cost based on Item Location details END
                    obj.BondedInventory = objDTO.BondedInventory;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Import";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.AddedFrom = objDTO.AddedFrom;
                    obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                    //obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb.HasValue ? Convert.ToDateTime(objDTO.ReceivedOnWeb) : DateTimeUtility.DateTimeNow);

                    var tmpId = obj.ID;
                    if (arrColumns.Contains("elabelkey"))
                    {
                        obj.eLabelKey = objDTO.eLabelKey;
                    }
                    if (arrColumns.Contains("enrichedproductdata"))
                    {
                        obj.EnrichedProductData = objDTO.EnrichedProductData;
                    }
                    if (arrColumns.Contains("enhanceddescription"))
                    {
                        obj.EnhancedDescription = objDTO.EnhancedDescription;
                    }
                    if (arrColumns.Contains("poitemlinenumber"))
                    {
                        obj.POItemLineNumber = objDTO.POItemLineNumber;
                    }

                    context.SaveChanges();

                    if (tmpId > 0)
                    {
                        CostDTO ObjCostDTO = objItemMaster.GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.WhatWhereAction);
                        ItemMaster objUpdate = context.ItemMasters.Where(e => e.ID == tmpId).FirstOrDefault();
                        objUpdate.ExtendedCost = ObjCostDTO.ExtCost;
                        objUpdate.AverageCost = ObjCostDTO.AvgCost;
                        objUpdate.PerItemCost = ObjCostDTO.PerItemCost;

                        context.SaveChanges();
                    }

                    objDTO = objItemMaster.FillWithExtraDetailImport(objDTO);

                    ItemGUIDLocation objItemGUIDLocation = new ItemGUIDLocation();
                    objItemGUIDLocation.ItemGUID = objDTO.GUID;
                    objItemGUIDLocation.LocationName = objDTO.InventryLocation;
                    objItemGUIDLocation.EntryType = "Edit";
                    objItemGUIDLocation.CriticalQuantity = objDTO.CriticalQuantity;
                    objItemGUIDLocation.MinimumQuantity = objDTO.MinimumQuantity;
                    objItemGUIDLocation.MaximumQuantity = objDTO.MaximumQuantity;
                    objItemGUIDLocation.DefaultBlanketPOID = objDTO.BlanketPOID ?? 0;


                    lstItemGUID.Add(objItemGUIDLocation);

                    //if (obj.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false) == false)
                    //{
                    //    BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
                    //    List<ItemLocationDetailsDTO> lstItemLocationDetails = new List<ItemLocationDetailsDTO>();
                    //    lstItemLocationDetails = objBinDAL.GetAllItemLocationsByItemGuid(objDTO.GUID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0));

                    //    objItemMaster.SendMailWhenItemStockOutByBin(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0), objDTO.DispOnHandQuantity.GetValueOrDefault(0), objDTO.ItemNumber, objDTO.GUID, true, lstItemLocationDetails);
                    //    objItemMaster.RemoveItemStockOutMailLogByBin(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0), lstItemLocationDetails);
                    //}
                    //else
                    //{
                    //    if (objDTO.DispOnHandQuantity.GetValueOrDefault(0) <= 0)
                    //    {
                    //        try
                    //        {
                    //            objItemMaster.SendMailWhenItemStockOut(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0), objDTO.DispOnHandQuantity.GetValueOrDefault(0), objDTO.ItemNumber, objDTO.GUID);
                    //        }
                    //        catch (Exception)
                    //        {
                    //            //Log the email exception

                    //        }
                    //    }
                    //    else
                    //    {
                    //        objItemMaster.RemoveItemStockOutMailLog(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0));
                    //    }
                    //}
                }
            }

            //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "ImportDAL >> EditItemMaster");
            new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "BulkImport >> Modified Item", SessionUserId);
            objDTO.SuggestedOrderQuantity = objItemMaster.GetSuggestedOrderQty(objDTO.GUID);

            //IEnumerable<ItemMasterDTO> ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + objDTO.CompanyID.ToString());
            //if (ObjCache != null)
            //{
            //    CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();

            //}
            new DashboardDAL(base.DataBaseName).SetItemsAutoClassification(objDTO.GUID, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.LastUpdatedBy ?? 0, 1);
            return true;
        }
        public bool EditBOMItemMaster(BOMItemMasterMain objDTO, string[] arrColumns)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;



            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ItemMaster obj = new ItemMaster();
                obj = (from m in context.ItemMasters
                       where m.ItemNumber == objDTO.ItemNumber && m.Room == null && m.CompanyID == objDTO.CompanyID && m.IsArchived == false && m.IsDeleted == false
                       select m).FirstOrDefault();
                if (obj != null)
                {
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

                    if (arrColumns.Contains("categoryname"))
                    {
                        if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
                            obj.CategoryID = objDTO.CategoryID;
                    }

                    if (arrColumns.Contains("glaccount"))
                    {
                        if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
                            obj.GLAccountID = objDTO.GLAccountID;
                    }

                    if (arrColumns.Contains("uom"))
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
                    if (arrColumns.Contains("uom"))
                    {
                        obj.UOMID = objDTO.UOMID;
                    }
                    obj.IsActive = objDTO.IsActive;
                    obj.Updated = objDTO.Updated;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Import";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;


                    context.SaveChanges();

                    ItemGUIDLocation objItemGUIDLocation = new ItemGUIDLocation();
                    objItemGUIDLocation.ItemGUID = objDTO.GUID;
                    objItemGUIDLocation.LocationName = string.Empty;
                    lstItemGUID.Add(objItemGUIDLocation);

                    BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(base.DataBaseName);
                    UpdateBOMItemMSLDetail(obj.GUID, obj.CompanyID ?? 0, objDTO.LastUpdatedBy ?? 0);
                    objBOMItemMasterDAL.UpdateReferenceBOMItem(obj.ID, objDTO.LastUpdatedBy ?? 0);
                }






            }


            return true;
        }
        public List<T> GetUpdatedList<T>(IList<T> lstItems, string TableName, string Status, IList<T> lstRetItems, string reason)
        {
            List<T> lstReturnFinal = new List<T>();

            if (TableName == ImportMastersDTO.TableName.CategoryMaster.ToString())
            {
                foreach (CategoryMasterMain item in lstItems.Cast<CategoryMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.InventoryClassificationMaster.ToString())
            {
                foreach (InventoryClassificationMasterMain item in lstItems.Cast<InventoryClassificationMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.UnitMaster.ToString())
            {
                foreach (UnitMasterMain item in lstItems.Cast<UnitMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.CostUOMMaster.ToString())
            {
                foreach (CostUOMMasterMain item in lstItems.Cast<CostUOMMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.CustomerMaster.ToString())
            {
                foreach (CustomerMasterMain item in lstItems.Cast<CustomerMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.FreightTypeMaster.ToString())
            {
                foreach (FreightTypeMasterMain item in lstItems.Cast<FreightTypeMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.GLAccountMaster.ToString())
            {
                foreach (GLAccountMasterMain item in lstItems.Cast<GLAccountMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.BinMaster.ToString())
            {
                foreach (BinMasterMain item in lstItems.Cast<BinMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.ManufacturerMaster.ToString())
            {
                foreach (ManufacturerMasterMain item in lstItems.Cast<ManufacturerMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.MeasurementTermMaster.ToString())
            {
                foreach (MeasurementTermMasterMain item in lstItems.Cast<MeasurementTermMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.ShipViaMaster.ToString())
            {
                foreach (ShipViaMasterMain item in lstItems.Cast<ShipViaMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.SupplierMaster.ToString())
            {
                foreach (SupplierMasterMain item in lstItems.Cast<SupplierMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.TechnicianMaster.ToString())
            {
                foreach (TechnicianMasterMain item in lstItems.Cast<TechnicianMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.ToolCategoryMaster.ToString())
            {
                foreach (ToolCategoryMasterMain item in lstItems.Cast<ToolCategoryMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.LocationMaster.ToString())
            {
                foreach (LocationMasterMain item in lstItems.Cast<LocationMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.ItemMaster.ToString())
            {
                foreach (BOMItemMasterMain item in lstItems.Cast<BOMItemMasterMain>())
                {

                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));


                    ItemGUIDLocation objItemGUIDLocation = new ItemGUIDLocation();
                    objItemGUIDLocation.ItemGUID = item.GUID;
                    objItemGUIDLocation.LocationName = item.InventryLocation;
                    objItemGUIDLocation.EntryType = "Insert";
                    objItemGUIDLocation.CriticalQuantity = item.CriticalQuantity;
                    objItemGUIDLocation.MinimumQuantity = item.MinimumQuantity;
                    objItemGUIDLocation.MaximumQuantity = item.MaximumQuantity;
                    objItemGUIDLocation.DefaultBlanketPOID = item.BlanketPOID ?? 0;
                    lstItemGUID.Add(objItemGUIDLocation);
                }
            }
            else if (TableName == ImportMastersDTO.TableName.EditItemMaster.ToString())
            {
                foreach (BOMItemMasterMain item in lstItems.Cast<BOMItemMasterMain>())
                {

                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));


                    ItemGUIDLocation objItemGUIDLocation = new ItemGUIDLocation();
                    objItemGUIDLocation.ItemGUID = item.GUID;
                    objItemGUIDLocation.LocationName = item.InventryLocation;
                    objItemGUIDLocation.EntryType = "Insert";
                    objItemGUIDLocation.CriticalQuantity = item.CriticalQuantity;
                    objItemGUIDLocation.MinimumQuantity = item.MinimumQuantity;
                    objItemGUIDLocation.MaximumQuantity = item.MaximumQuantity;
                    objItemGUIDLocation.DefaultBlanketPOID = item.BlanketPOID ?? 0;
                    lstItemGUID.Add(objItemGUIDLocation);
                }
            }
            else if (TableName == ImportMastersDTO.TableName.BOMItemMaster.ToString())
            {
                foreach (BOMItemMasterMain item in lstItems.Cast<BOMItemMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));

                    ItemGUIDLocation objItemGUIDLocation = new ItemGUIDLocation();
                    objItemGUIDLocation.ItemGUID = item.GUID;
                    objItemGUIDLocation.LocationName = string.Empty;
                    lstItemGUID.Add(objItemGUIDLocation);
                }
            }
            else if (TableName == ImportMastersDTO.TableName.AssetMaster.ToString())
            {
                foreach (AssetMasterMain item in lstItems.Cast<AssetMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.ToolMaster.ToString())
            {
                foreach (ToolMasterMain item in lstItems.Cast<ToolMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.QuickListItems.ToString())
            {
                foreach (QuickListItemsMain item in lstItems.Cast<QuickListItemsMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.InventoryLocation.ToString())
            {
                foreach (InventoryLocationMain item in lstItems.Cast<InventoryLocationMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.kitdetail.ToString())
            {
                foreach (KitDetailmain item in lstItems.Cast<KitDetailmain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }

            else if (TableName == ImportMastersDTO.TableName.ProjectMaster.ToString())
            {
                foreach (ProjectMasterMain item in lstItems.Cast<ProjectMasterMain>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }

            else if (TableName == ImportMastersDTO.TableName.BarcodeMaster.ToString())
            {
                foreach (ImportBarcodeMaster item in lstItems.Cast<ImportBarcodeMaster>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.PastMaintenanceDue.ToString())
            {
                foreach (PastMaintenanceDueImport item in lstItems.Cast<PastMaintenanceDueImport>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else if (TableName == ImportMastersDTO.TableName.AssetToolScheduler.ToString())
            {
                foreach (AssetToolScheduler item in lstItems.Cast<AssetToolScheduler>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }

            else if (TableName == ImportMastersDTO.TableName.AssetToolSchedulerMapping.ToString())
            {
                foreach (AssetToolSchedulerMapping item in lstItems.Cast<AssetToolSchedulerMapping>())
                {
                    item.Status = Status;
                    item.Reason = reason;
                    lstRetItems.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }

            lstReturnFinal = lstRetItems.ToList();
            return lstReturnFinal;
        }
        //public void SaveItemLocationLevelQuantity(ItemMasterDTO objItemLeveQuantityItemMasterDTO, long RoomId, long CompanyId, long UserId)
        //{
        //    BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(base.DataBaseName);
        //    BinMasterDTO objItemLocationLevelQuanityDTO = new BinMasterDTO();
        //    BinMaster obj = new BinMaster();
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        obj = (from u in context.BinMasters
        //               where u.ItemGUID == objItemLeveQuantityItemMasterDTO.GUID && u.IsArchived == false && u.IsDeleted == false && u.ID == objItemLeveQuantityItemMasterDTO.DefaultLocation
        //               select u).FirstOrDefault();
        //        if (obj == null)
        //        {
        //            objItemLocationLevelQuanityDTO.ID = objItemLeveQuantityItemMasterDTO.DefaultLocation ?? 0;
        //            objItemLocationLevelQuanityDTO.ItemGUID = objItemLeveQuantityItemMasterDTO.GUID;
        //            objItemLocationLevelQuanityDTO.Room = RoomId;
        //            objItemLocationLevelQuanityDTO.CompanyID = CompanyId;
        //            objItemLocationLevelQuanityDTO.CriticalQuantity = 0;
        //            objItemLocationLevelQuanityDTO.MinimumQuantity = 0;
        //            objItemLocationLevelQuanityDTO.MaximumQuantity = 0;
        //            objItemLocationLevelQuanityDTO.Created = DateTimeUtility.DateTimeNow;
        //            objItemLocationLevelQuanityDTO.CreatedBy = UserId;
        //            objItemLocationLevelQuanityDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //            objItemLocationLevelQuanityDTO.LastUpdatedBy = UserId;
        //            // objItemLocationLevelQuanityDTO.IsDefault = true;
        //            objItemLocationLevelQuanityDTO.IsDeleted = false;
        //            objItemLocationLevelQuanityDTO.IsArchived = false;
        //            objItemLocationLevelQuanityDAL.Insert(objItemLocationLevelQuanityDTO);
        //        }

        //    }



        //}

        public List<T> BulkInsertWithChiled<T>(string tableName, IList<T> list, long RoomId, long CompanyId, string ColumnName, long UserId, bool IsAllowToInsertSA, bool IsAllowToEditSA,
                                               long EnterpriseId,List<UDFOptionsMain> lstOption = null, bool isImgZipAvail = false)
        {
            string[] arrcolumns = ColumnName.ToLower().Split(',');
            List<T> lstreturnFinal = new List<T>();
            CommonDAL objCDAL = new CommonDAL(base.DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (tableName == ImportMastersDTO.TableName.SupplierMaster.ToString())
                {
                    List<string> suppImportList = (from d in (List<SupplierMasterMain>)list select d.SupplierName).Distinct().ToList();
                    var suppDBList = (from d in context.SupplierMasters
                                      where d.Room == RoomId && d.CompanyID == CompanyId && d.IsArchived == false && d.IsDeleted == false
                                      select new
                                      {
                                          SupplierName = d.SupplierName,
                                          ID = d.ID
                                      }).ToList();

                    List<SupplierMasterMain> lstFinalAdd = (from m in (List<SupplierMasterMain>)list
                                                            where (!suppDBList.Select(x => x.SupplierName).Contains(m.SupplierName)) //&& m.Category != ""
                                                            select m).ToList();

                    List<SupplierMasterMain> lstEdit = (from m in (List<SupplierMasterMain>)list
                                                        join s in suppDBList on m.SupplierName equals s.SupplierName
                                                        select m).ToList();
                    if (lstEdit != null)
                    {
                        foreach (SupplierMasterMain objSupplierMasterMain in lstEdit)
                            objSupplierMasterMain.ID = suppDBList.Where(x => x.SupplierName == objSupplierMasterMain.SupplierName).FirstOrDefault().ID;
                    }

                    List<SupplierMaster> lstSupplierMaster = new List<SupplierMaster>();
                    List<SupplierAccountDetail> lstSupplierAccountDetails = new List<SupplierAccountDetail>();
                    List<SupplierBlanketPODetail> lstSupplierBlanketPODetails = new List<SupplierBlanketPODetail>();

                    if (lstEdit != null && lstEdit.Count() > 0)
                    {

                        lstSupplierMaster = (from m in context.SupplierMasters
                                             where (suppImportList.Contains(m.SupplierName)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                             select m).ToList();

                        lstSupplierAccountDetails = (from m in context.SupplierAccountDetails
                                                     join sm in context.SupplierMasters on m.SupplierID equals sm.ID
                                                     where (suppImportList.Contains(sm.SupplierName)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                     select m).ToList();

                        lstSupplierBlanketPODetails = (from m in context.SupplierBlanketPODetails
                                                       join sm in context.SupplierMasters on m.SupplierID equals sm.ID
                                                       where (suppImportList.Contains(sm.SupplierName)) && m.Room == RoomId && m.CompanyID == CompanyId && m.IsArchived == false && m.IsDeleted == false
                                                       select m).ToList();
                    }

                    string cultureCode = "en-US";
                    var regionalSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomId, CompanyId, -1);

                    if (regionInfo != null)
                    {
                        cultureCode = regionInfo.CultureCode;
                    }

                    var supplierAccountDetailsResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResSupplierAccountDetails", cultureCode, EnterpriseId, CompanyId); 
                    var supplierBlanketPODetailResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResSupplierBlanketPODetails", cultureCode, EnterpriseId, CompanyId); 
                    string msgSupplierAccountNameCantBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("SupplierAccountNameCantBlank", supplierAccountDetailsResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResSupplierAccountDetails", cultureCode);
                    string msgSupplierAccountNumberCantBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("SupplierAccountNumberCantBlank", supplierAccountDetailsResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResSupplierAccountDetails", cultureCode); 
                    string msgNotAllowedToInsertSupplierAccountDetail = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotAllowedToInsertSupplierAccountDetail", supplierAccountDetailsResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResSupplierAccountDetails", cultureCode); 
                    string msgNotAllowedToUpdateSupplierAccountDetail = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotAllowedToUpdateSupplierAccountDetail", supplierAccountDetailsResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResSupplierAccountDetails", cultureCode); 
                    string msgBlanketPOIsAlreadyAdded = ResourceRead.GetResourceValueByKeyAndFullFilePath("BlanketPOIsAlreadyAdded", supplierBlanketPODetailResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResSupplierBlanketPODetails", cultureCode);
                    
                    foreach (string SupplierName in suppImportList)
                    {
                        #region //Import for New Supplier

                        List<SupplierMasterMain> oSupp = lstFinalAdd.Where(x => x.SupplierName == SupplierName).ToList();
                        if (oSupp.Count() > 0)
                        {
                            try
                            {
                                string strOK1 = objCDAL.DuplicateCheck(oSupp[0].SupplierColor, "add", 0, "SupplierMaster", "SupplierColor", RoomId, CompanyId);
                                if (strOK1 == "duplicate")
                                {
                                    foreach (SupplierMasterMain item in oSupp)
                                    {
                                        item.Status = "Fail";
                                        item.Reason = string.Format(DTO.Resources.ResMessage.DuplicateMessage, ResSupplierMaster.SupplierColor, oSupp[0].SupplierColor);
                                        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                    }
                                    continue;
                                }

                                //if (oSupp.Where(x => x.AccountIsDefault == true).Count() <= 0)
                                //{
                                //    foreach (SupplierMasterMain item in oSupp)
                                //    {
                                //        item.Status = "Fail";
                                //        item.Reason = "Please Select atleast one default Account Detail";
                                //        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                //    }
                                //    continue;
                                //}
                                //else
                                //    if (oSupp.Where(x => x.AccountIsDefault == true).Count() > 1)
                                //{
                                //    foreach (SupplierMasterMain item in oSupp)
                                //    {
                                //        item.Status = "Fail";
                                //        item.Reason = "Supplier can not have more then one default account";
                                //        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                //    }
                                //    continue;
                                //}



                                SupplierMasterDTO oSupplierMasterDTO = new SupplierMasterDTO();
                                oSupplierMasterDTO.CompanyID = CompanyId;
                                oSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                oSupplierMasterDTO.CreatedBy = UserId;
                                oSupplierMasterDTO.IsArchived = false;
                                oSupplierMasterDTO.IsDeleted = false;
                                oSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                oSupplierMasterDTO.LastUpdatedBy = UserId;
                                oSupplierMasterDTO.Room = RoomId;
                                oSupplierMasterDTO.SupplierName = oSupp[0].SupplierName;
                                oSupplierMasterDTO.SupplierColor = oSupp[0].SupplierColor;
                                oSupplierMasterDTO.Description = oSupp[0].Description;
                                oSupplierMasterDTO.BranchNumber = oSupp[0].BranchNumber;
                                oSupplierMasterDTO.MaximumOrderSize = oSupp[0].MaximumOrderSize;
                                oSupplierMasterDTO.Address = oSupp[0].Address;
                                oSupplierMasterDTO.City = oSupp[0].City;
                                oSupplierMasterDTO.State = oSupp[0].State;
                                oSupplierMasterDTO.ZipCode = oSupp[0].ZipCode;
                                oSupplierMasterDTO.Country = oSupp[0].Country;
                                oSupplierMasterDTO.Contact = oSupp[0].Contact;
                                oSupplierMasterDTO.Phone = oSupp[0].Phone;
                                oSupplierMasterDTO.Fax = oSupp[0].Fax;
                                oSupplierMasterDTO.Email = oSupp[0].Email;
                                oSupplierMasterDTO.IsSendtoVendor = oSupp[0].IsSendtoVendor;
                                oSupplierMasterDTO.IsVendorReturnAsn = oSupp[0].IsVendorReturnAsn;
                                oSupplierMasterDTO.IsSupplierReceivesKitComponents = oSupp[0].IsSupplierReceivesKitComponents;
                                oSupplierMasterDTO.POAutoSequence = oSupp[0].POAutoSequence;
                                oSupplierMasterDTO.UDF1 = oSupp[0].UDF1;
                                oSupplierMasterDTO.UDF2 = oSupp[0].UDF2;
                                oSupplierMasterDTO.UDF3 = oSupp[0].UDF3;
                                oSupplierMasterDTO.UDF4 = oSupp[0].UDF4;
                                oSupplierMasterDTO.UDF5 = oSupp[0].UDF5;
                                oSupplierMasterDTO.UDF6 = oSupp[0].UDF6;
                                oSupplierMasterDTO.UDF7 = oSupp[0].UDF7;
                                oSupplierMasterDTO.UDF8 = oSupp[0].UDF8;
                                oSupplierMasterDTO.UDF9 = oSupp[0].UDF9;
                                oSupplierMasterDTO.UDF10 = oSupp[0].UDF10;
                                oSupplierMasterDTO.AddedFrom = "Web";
                                oSupplierMasterDTO.EditedFrom = "Web";
                                oSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                oSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                oSupplierMasterDTO.PullPurchaseNumberType = oSupp[0].PullPurchaseNumberType;
                                oSupplierMasterDTO.LastPullPurchaseNumberUsed = oSupp[0].LastPullPurchaseNumberUsed;

                                oSupplierMasterDTO.ImageExternalURL = oSupp[0].ImageExternalURL;
                                //if (isImgZipAvail)
                                //{
                                //    oSupplierMasterDTO.ImageType = oSupp[0].ImageType;
                                //    oSupplierMasterDTO.SupplierImage = oSupp[0].SupplierImage;
                                //}
                                //else
                                //{
                                //    oSupplierMasterDTO.ImageType = "ExternalImage";
                                //    oSupplierMasterDTO.SupplierImage = string.Empty;
                                //}

                                if (oSupp[0].IsSendtoVendor == true)
                                {
                                    if (oSupp.Where(x => (!string.IsNullOrWhiteSpace(x.AccountName)) || x.AccountName != "").Count() == 0)
                                    {
                                        foreach (SupplierMasterMain item in oSupp)
                                        {
                                            item.Status = "Fail";
                                            item.Reason = msgSupplierAccountNameCantBlank;
                                            lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                        }
                                        continue;

                                    }
                                    if (oSupp.Where(x => (!string.IsNullOrWhiteSpace(x.AccountNumber)) || x.AccountNumber != "").Count() == 0)
                                    {
                                        foreach (SupplierMasterMain item in oSupp)
                                        {
                                            item.Status = "Fail";
                                            item.Reason = msgSupplierAccountNumberCantBlank; 
                                            lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                        }
                                        continue;

                                    }

                                }

                                oSupplierMasterDTO.ID = new SupplierMasterDAL(base.DataBaseName).Insert(oSupplierMasterDTO);

                                // Start -- insert into Room Schedule for PullSchedule type is Immediate 
                                if (oSupplierMasterDTO.ID > 0)
                                {
                                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                                    SchedulerDTO objSchedulerDTO = objSupplierMasterDAL.GetRoomSchedule(oSupplierMasterDTO.ID, oSupplierMasterDTO.Room.GetValueOrDefault(0), 7);
                                    if (objSchedulerDTO == null)
                                    {
                                        // insert into Room Schedule for PullSchedule type is Immediate
                                        SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                                        objPullSchedulerDTO.SupplierId = oSupplierMasterDTO.ID;
                                        objPullSchedulerDTO.CompanyId = oSupplierMasterDTO.CompanyID.GetValueOrDefault(0);
                                        objPullSchedulerDTO.RoomId = oSupplierMasterDTO.Room.GetValueOrDefault(0);
                                        objPullSchedulerDTO.LoadSheduleFor = 7;
                                        objPullSchedulerDTO.ScheduleMode = 5;
                                        objPullSchedulerDTO.IsScheduleActive = true;
                                        objPullSchedulerDTO.MonthlyDayOfMonth = 2;
                                        objSupplierMasterDAL.SaveSupplierSchedule(objPullSchedulerDTO);
                                    }
                                }
                                /// End- logic for insert into Room Schedule for PullSchedule type is Immediate


                                foreach (SupplierMasterMain item in oSupp)
                                {
                                    try
                                    {
                                        //Duplicate check for Blanket PO values (WI-1013)

                                        if (!string.IsNullOrWhiteSpace(item.BlanketPONumber))
                                        {
                                            long BlanketPOID = 0;
                                            BlanketPOID = new SupplierBlanketPODetailsDAL(base.DataBaseName).SupplierBlanketPODetailsDuplicateCheck(0, item.BlanketPONumber, oSupplierMasterDTO.ID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                                            if (BlanketPOID > 0)
                                            {
                                                item.Status = "Fail";
                                                item.Reason = msgBlanketPOIsAlreadyAdded; 
                                                lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                                continue;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(item.BlanketPONumber))
                                        {
                                            SupplierBlanketPODetailsDTO oSupplierBlanketPODetailsDTO = new SupplierBlanketPODetailsDTO();
                                            oSupplierBlanketPODetailsDTO.SupplierID = oSupplierMasterDTO.ID;
                                            oSupplierBlanketPODetailsDTO.BlanketPO = item.BlanketPONumber;
                                            oSupplierBlanketPODetailsDTO.StartDate = item.StartDate;
                                            oSupplierBlanketPODetailsDTO.Enddate = item.EndDate;
                                            oSupplierBlanketPODetailsDTO.GUID = Guid.NewGuid();
                                            oSupplierBlanketPODetailsDTO.Created = DateTime.Now;
                                            oSupplierBlanketPODetailsDTO.CreatedBy = UserId;
                                            oSupplierBlanketPODetailsDTO.Updated = DateTime.Now;
                                            oSupplierBlanketPODetailsDTO.LastUpdatedBy = UserId;
                                            oSupplierBlanketPODetailsDTO.CompanyID = CompanyId;
                                            oSupplierBlanketPODetailsDTO.Room = RoomId;
                                            oSupplierBlanketPODetailsDTO.IsArchived = false;
                                            oSupplierBlanketPODetailsDTO.IsDeleted = item.IsBlanketDeleted;
                                            oSupplierBlanketPODetailsDTO.MaxLimit = item.MaxLimit;
                                            oSupplierBlanketPODetailsDTO.IsNotExceed = item.IsNotExceed;
                                            oSupplierBlanketPODetailsDTO.AddedFrom = "Web";
                                            oSupplierBlanketPODetailsDTO.EditedFrom = "Web";
                                            oSupplierBlanketPODetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            oSupplierBlanketPODetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            oSupplierBlanketPODetailsDTO.MaxLimitQty = item.MaxLimitQty;
                                            oSupplierBlanketPODetailsDTO.IsNotExceedQty = item.IsNotExceedQty;

                                            new SupplierBlanketPODetailsDAL(base.DataBaseName).Insert(oSupplierBlanketPODetailsDTO);
                                        }

                                        if (!string.IsNullOrWhiteSpace(item.AccountNumber))
                                        {
                                            if (IsAllowToInsertSA)
                                            {
                                                SupplierAccountDetailsDTO oSupplierAccountDetailsDTO = new SupplierAccountDetailsDTO();
                                                oSupplierAccountDetailsDTO.SupplierID = oSupplierMasterDTO.ID;
                                                oSupplierAccountDetailsDTO.AccountNo = item.AccountNumber;
                                                oSupplierAccountDetailsDTO.AccountName = item.AccountName;
                                                oSupplierAccountDetailsDTO.Address = item.AccountAddress;
                                                oSupplierAccountDetailsDTO.City = item.AccountCity;
                                                oSupplierAccountDetailsDTO.State = item.AccountState;
                                                oSupplierAccountDetailsDTO.ZipCode = item.AccountZip;
                                                oSupplierAccountDetailsDTO.IsDefault = item.AccountIsDefault;
                                                oSupplierAccountDetailsDTO.Created = DateTime.Now;
                                                oSupplierAccountDetailsDTO.CreatedBy = UserId;
                                                oSupplierAccountDetailsDTO.Updated = DateTime.Now;
                                                oSupplierAccountDetailsDTO.LastUpdatedBy = UserId;
                                                oSupplierAccountDetailsDTO.Room = RoomId;
                                                oSupplierAccountDetailsDTO.CompanyID = CompanyId;
                                                oSupplierAccountDetailsDTO.IsArchived = false;
                                                oSupplierAccountDetailsDTO.IsDeleted = false;
                                                oSupplierAccountDetailsDTO.GUID = Guid.NewGuid();
                                                oSupplierAccountDetailsDTO.ShipToID = item.AccountShipToID;
                                                oSupplierAccountDetailsDTO.Country = item.AccountCountry;
                                                new SupplierAccountDetailsDAL(base.DataBaseName).Insert(oSupplierAccountDetailsDTO);
                                            }
                                            else
                                            {
                                                item.Status = "Fail";
                                                item.Reason = msgNotAllowedToInsertSupplierAccountDetail;
                                                lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                                continue;
                                            }
                                        }

                                        item.Status = "Success";
                                        item.Reason = "N/A";
                                        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                    }
                                    catch (Exception ex)
                                    {
                                        item.Status = "Fail";
                                        item.Reason = ex.Message.ToString();
                                        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                oSupp[0].Status = "Fail";
                                oSupp[0].Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(oSupp[0], typeof(T)));
                            }
                        }

                        #endregion

                        #region //Import Existing Supplier

                        List<SupplierMasterMain> oSuppEdit = lstEdit.Where(x => x.SupplierName == SupplierName).ToList();

                        if (oSuppEdit.Count() > 0)
                        {
                            try
                            {
                                string strOK1 = objCDAL.DuplicateCheck(oSuppEdit[0].SupplierColor, "edit", oSuppEdit[0].ID, "SupplierMaster", "SupplierColor", RoomId, CompanyId);
                                if (strOK1 == "duplicate")
                                {
                                    foreach (SupplierMasterMain item in oSuppEdit)
                                    {
                                        item.Status = "Fail";
                                        item.Reason = string.Format(DTO.Resources.ResMessage.DuplicateMessage, ResSupplierMaster.SupplierColor, oSuppEdit[0].SupplierColor);
                                        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                    }
                                    continue;
                                }

                                //---------------Get Existing Accounts From DB and Mearge With Imported Account---------------
                                //
                                List<long> lstSuppAccId = new List<long>();
                                SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                                SupplierAccountDetailsDAL objSupplierADDAL = new SupplierAccountDetailsDAL(base.DataBaseName);
                                List<SupplierAccountDetailsDTO> lstSupplierAccountDetailsDTO = objSupplierADDAL.GetSupplierAccountDetails(oSuppEdit[0].ID, RoomId, CompanyId);
                                List<SupplierAccountDetailsDTO> lstSupplierAccountDetailsDTO2 = new List<SupplierAccountDetailsDTO>();
                                SupplierAccountDetailsDTO objSupplierAccountDetailsDTO;
                                foreach (SupplierMasterMain item in oSuppEdit)
                                {
                                    if (lstSupplierAccountDetailsDTO != null)
                                        objSupplierAccountDetailsDTO = lstSupplierAccountDetailsDTO.Where(x => x.AccountNo.Trim().ToUpper() == item.AccountNumber.Trim().ToUpper()).FirstOrDefault();
                                    else
                                        objSupplierAccountDetailsDTO = null;

                                    if (lstSupplierAccountDetailsDTO != null && lstSupplierAccountDetailsDTO.Count > 0)
                                    {
                                        foreach (SupplierAccountDetailsDTO itemAcc in lstSupplierAccountDetailsDTO)
                                        {
                                            if (itemAcc.IsDefault == true)
                                                lstSuppAccId.Add(itemAcc.ID);
                                        }
                                    }


                                    if (objSupplierAccountDetailsDTO != null)
                                    {
                                        if (objSupplierAccountDetailsDTO.IsDefault == true && item.AccountIsDefault == false)
                                            lstSuppAccId.Add(objSupplierAccountDetailsDTO.ID);

                                        objSupplierAccountDetailsDTO.IsDefault = item.AccountIsDefault;
                                    }
                                    else
                                    {
                                        lstSupplierAccountDetailsDTO.Add(new SupplierAccountDetailsDTO
                                        {
                                            ID = item.ID,
                                            SupplierID = item.ID,
                                            AccountNo = item.AccountNumber,
                                            AccountName = item.AccountName != null ? (item.AccountName) : string.Empty,
                                            Address = item.AccountAddress,
                                            City = item.AccountCity,
                                            State = item.AccountState,
                                            ZipCode = item.AccountZip,
                                            Country = item.AccountCountry,
                                            ShipToID = item.AccountShipToID,
                                            IsDefault = item.AccountIsDefault
                                        });
                                    }
                                }

                                //---------------Check If At Least One Account Has IsDefault---------------
                                //
                                //if (lstSupplierAccountDetailsDTO.Where(x => x.IsDefault == true).Count() <= 0)
                                //{
                                //    foreach (SupplierMasterMain item in oSuppEdit)
                                //    {
                                //        item.Status = "Fail";
                                //        item.Reason = "Please Select atleast one default Account Detail";
                                //        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                //    }
                                //    continue;
                                //}
                                //else if (oSuppEdit.Where(x => x.AccountIsDefault == true).Count() > 1)
                                //{
                                //    foreach (SupplierMasterMain item in oSuppEdit)
                                //    {
                                //        item.Status = "Fail";
                                //        item.Reason = "Supplier can not have more then one default account";
                                //        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                //    }
                                //    continue;
                                //}

                                //---------------Set Default Account To False For Existing Accounts---------------
                                //
                                //if (lstSuppAccId != null && lstSuppAccId.Count > 0)
                                //{
                                //    objSupplierMasterDAL.SetDefaultAccFalse(lstSuppAccId, RoomId, CompanyId, (oSuppEdit[0].LastUpdatedBy == null ? 0 : (long)oSuppEdit[0].LastUpdatedBy));
                                //}

                                SupplierMaster obj = (from m in lstSupplierMaster
                                                      where m.SupplierName == oSuppEdit[0].SupplierName && m.Room == RoomId && m.CompanyID == CompanyId
                                                      select m).SingleOrDefault();

                                if (arrcolumns.Contains("suppliername"))
                                    obj.SupplierName = oSuppEdit[0].SupplierName;
                                if (arrcolumns.Contains("suppliercolor"))
                                    obj.SupplierColor = oSuppEdit[0].SupplierColor;
                                if (arrcolumns.Contains("description"))
                                    obj.Description = oSuppEdit[0].Description;
                                if (arrcolumns.Contains("branchnumber"))
                                    obj.BranchNumber = oSuppEdit[0].BranchNumber;
                                if (arrcolumns.Contains("maximumordersize"))
                                    obj.MaximumOrderSize = oSuppEdit[0].MaximumOrderSize;
                                if (arrcolumns.Contains("address"))
                                    obj.Address = oSuppEdit[0].Address;
                                if (arrcolumns.Contains("city"))
                                    obj.City = oSuppEdit[0].City;
                                if (arrcolumns.Contains("state"))
                                    obj.State = oSuppEdit[0].State;
                                if (arrcolumns.Contains("zipcode"))
                                    obj.ZipCode = oSuppEdit[0].ZipCode;
                                if (arrcolumns.Contains("country"))
                                    obj.Country = oSuppEdit[0].Country;
                                if (arrcolumns.Contains("contact"))
                                    obj.Contact = oSuppEdit[0].Contact;
                                if (arrcolumns.Contains("phone"))
                                    obj.Phone = oSuppEdit[0].Phone;
                                if (arrcolumns.Contains("fax"))
                                    obj.Fax = oSuppEdit[0].Fax;
                                if (arrcolumns.Contains("email"))
                                    obj.Email = oSuppEdit[0].Email;

                                if (arrcolumns.Contains("issendtovendor"))
                                    obj.IsSendtoVendor = oSuppEdit[0].IsSendtoVendor;
                                if (arrcolumns.Contains("isvendorreturnasn"))
                                    obj.IsVendorReturnAsn = oSuppEdit[0].IsVendorReturnAsn;
                                if (arrcolumns.Contains("issupplierreceiveskitcomponents"))
                                    obj.IsSupplierReceivesKitComponents = oSuppEdit[0].IsSupplierReceivesKitComponents;

                                if (arrcolumns.Contains("ordernumbertypeblank") || arrcolumns.Contains("ordernumbertypefixed")
                                    || arrcolumns.Contains("ordernumbertypeblanketordernumber")
                                    || arrcolumns.Contains("ordernumbertypeincrementingordernumber") || arrcolumns.Contains("ordernumbertypeincrementingbyday")
                                    || arrcolumns.Contains("ordernumbertypedateincrementing") || arrcolumns.Contains("ordernumbertypedate"))
                                    obj.POAutoSequence = oSuppEdit[0].POAutoSequence;

                                obj.LastUpdatedBy = oSuppEdit[0].LastUpdatedBy;
                                obj.LastUpdated = oSuppEdit[0].LastUpdated;

                                if (arrcolumns.Contains("udf1"))
                                    obj.UDF1 = oSuppEdit[0].UDF1;
                                if (arrcolumns.Contains("udf2"))
                                    obj.UDF2 = oSuppEdit[0].UDF2;
                                if (arrcolumns.Contains("udf3"))
                                    obj.UDF3 = oSuppEdit[0].UDF3;
                                if (arrcolumns.Contains("udf4"))
                                    obj.UDF4 = oSuppEdit[0].UDF4;
                                if (arrcolumns.Contains("udf5"))
                                    obj.UDF5 = oSuppEdit[0].UDF5;
                                if (arrcolumns.Contains("udf6"))
                                    obj.UDF6 = oSuppEdit[0].UDF6;
                                if (arrcolumns.Contains("udf7"))
                                    obj.UDF7 = oSuppEdit[0].UDF7;
                                if (arrcolumns.Contains("udf8"))
                                    obj.UDF8 = oSuppEdit[0].UDF8;
                                if (arrcolumns.Contains("udf9"))
                                    obj.UDF9 = oSuppEdit[0].UDF9;
                                if (arrcolumns.Contains("udf10"))
                                    obj.UDF10 = oSuppEdit[0].UDF10;


                                obj.ImageType = oSuppEdit[0].ImageType;
                                if (arrcolumns.Contains("supplierimage"))
                                {
                                    if (string.IsNullOrEmpty(oSuppEdit[0].SupplierImage))
                                    {
                                        obj.SupplierImage = null;
                                        obj.ImageType = "ExternalImage";
                                    }
                                    else
                                    {
                                        obj.SupplierImage = oSuppEdit[0].SupplierImage;
                                    }
                                }

                                if (arrcolumns.Contains("imageexternalurl"))
                                    obj.ImageExternalURL = oSuppEdit[0].ImageExternalURL;

                                obj.EditedFrom = "Web";
                                obj.AddedFrom = "Web";
                                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                                //obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                                if (arrcolumns.Contains("pullpurchasenumberfixed") || arrcolumns.Contains("pullpurchasenumberblanketorder") || arrcolumns.Contains("pullpurchasenumberdateincrementing") ||
                                    arrcolumns.Contains("pullpurchasenumberdate"))
                                    obj.PullPurchaseNumberType = oSuppEdit[0].PullPurchaseNumberType;

                                if (arrcolumns.Contains("lastpullpurchasenumberused"))
                                    obj.LastPullPurchaseNumberUsed = oSuppEdit[0].LastPullPurchaseNumberUsed;

                                long supplierID = obj.ID;

                                foreach (SupplierMasterMain item in oSuppEdit)
                                {
                                    try
                                    {


                                        SupplierBlanketPODetail objSupplierBlanketPODetail = (from m in lstSupplierBlanketPODetails
                                                                                              where m.BlanketPO == item.BlanketPONumber && m.SupplierID == supplierID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                                              select m).SingleOrDefault();
                                        if (objSupplierBlanketPODetail != null)
                                        {
                                            if (arrcolumns.Contains("blanketponumber"))
                                            {
                                                objSupplierBlanketPODetail.BlanketPO = item.BlanketPONumber;
                                            }
                                            if (arrcolumns.Contains("isblanketdeleted") && item.IsBlanketDeleted == true)
                                            {
                                                objSupplierBlanketPODetail.IsDeleted = item.IsBlanketDeleted;
                                            }
                                            if (arrcolumns.Contains("startdate"))
                                            {
                                                objSupplierBlanketPODetail.StartDate = item.StartDate;
                                            }
                                            if (arrcolumns.Contains("enddate"))
                                            {
                                                objSupplierBlanketPODetail.Enddate = item.EndDate;
                                            }
                                            if (arrcolumns.Contains("maxlimit"))
                                            {
                                                objSupplierBlanketPODetail.MaxLimit = item.MaxLimit;
                                            }
                                            if (arrcolumns.Contains("donotexceed"))
                                            {
                                                objSupplierBlanketPODetail.IsNotExceed = item.IsNotExceed;
                                            }

                                            if (arrcolumns.Contains("maxlimitqty"))
                                            {
                                                objSupplierBlanketPODetail.MaxLimitQty = item.MaxLimitQty;
                                            }
                                            if (arrcolumns.Contains("donotexceedqty"))
                                            {
                                                objSupplierBlanketPODetail.IsNotExceedQty = item.IsNotExceedQty;
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrWhiteSpace(item.BlanketPONumber))
                                            {
                                                SupplierBlanketPODetailsDTO oSupplierBlanketPODetailsDTO = new SupplierBlanketPODetailsDTO();
                                                oSupplierBlanketPODetailsDTO.SupplierID = supplierID;
                                                oSupplierBlanketPODetailsDTO.BlanketPO = item.BlanketPONumber;
                                                oSupplierBlanketPODetailsDTO.StartDate = item.StartDate;
                                                oSupplierBlanketPODetailsDTO.Enddate = item.EndDate;
                                                oSupplierBlanketPODetailsDTO.GUID = Guid.NewGuid();
                                                oSupplierBlanketPODetailsDTO.Created = DateTime.Now;
                                                oSupplierBlanketPODetailsDTO.CreatedBy = UserId;
                                                oSupplierBlanketPODetailsDTO.Updated = DateTime.Now;
                                                oSupplierBlanketPODetailsDTO.LastUpdatedBy = UserId;
                                                oSupplierBlanketPODetailsDTO.CompanyID = CompanyId;
                                                oSupplierBlanketPODetailsDTO.Room = RoomId;
                                                oSupplierBlanketPODetailsDTO.IsArchived = false;
                                                oSupplierBlanketPODetailsDTO.IsDeleted = item.IsBlanketDeleted;
                                                oSupplierBlanketPODetailsDTO.MaxLimit = item.MaxLimit;
                                                oSupplierBlanketPODetailsDTO.IsNotExceed = item.IsNotExceed;
                                                oSupplierBlanketPODetailsDTO.AddedFrom = "Web";
                                                oSupplierBlanketPODetailsDTO.EditedFrom = "Web";
                                                oSupplierBlanketPODetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                                oSupplierBlanketPODetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                oSupplierBlanketPODetailsDTO.MaxLimitQty = item.MaxLimitQty;
                                                oSupplierBlanketPODetailsDTO.IsNotExceedQty = item.IsNotExceedQty;
                                                new SupplierBlanketPODetailsDAL(base.DataBaseName).Insert(oSupplierBlanketPODetailsDTO);
                                            }
                                        }

                                        SupplierAccountDetail objSuppAccount = (from m in lstSupplierAccountDetails
                                                                                where m.AccountNo == item.AccountNumber && m.SupplierID == supplierID && m.Room == RoomId && m.CompanyID == CompanyId
                                                                                select m).FirstOrDefault();
                                        if (objSuppAccount != null)
                                        {
                                            if (IsAllowToEditSA)
                                            {
                                                if (arrcolumns.Contains("accountnumber"))
                                                {
                                                    objSuppAccount.AccountNo = item.AccountNumber;
                                                }

                                                if (arrcolumns.Contains("accountname"))
                                                {
                                                    objSuppAccount.AccountName = item.AccountName;
                                                }

                                                if (arrcolumns.Contains("accountaddress"))
                                                {
                                                    objSuppAccount.Address = item.AccountAddress;
                                                }

                                                if (arrcolumns.Contains("accountcity"))
                                                {
                                                    objSuppAccount.City = item.AccountCity;
                                                }

                                                if (arrcolumns.Contains("accountstate"))
                                                {
                                                    objSuppAccount.State = item.AccountState;
                                                }

                                                if (arrcolumns.Contains("accountzip"))
                                                {
                                                    objSuppAccount.ZipCode = item.AccountZip;
                                                }

                                                if (arrcolumns.Contains("accountcountry"))
                                                {
                                                    objSuppAccount.Country = item.AccountCountry;
                                                }

                                                if (arrcolumns.Contains("accountshiptoid"))
                                                {
                                                    objSuppAccount.ShipToID = item.AccountShipToID;
                                                }

                                                if (arrcolumns.Contains("accountisdefault"))
                                                {
                                                    //objSuppAccount.IsDefault = item.AccountIsDefault;

                                                    SupplierAccountDetail objSuppAccount1 = (from m in lstSupplierAccountDetails
                                                                                             where m.SupplierID == supplierID && m.Room == RoomId && m.CompanyID == CompanyId && m.IsDefault == true
                                                                                             select m).FirstOrDefault();

                                                    objSuppAccount.IsDefault = (objSuppAccount1 != null && objSuppAccount1.ID > 0 && objSuppAccount1.IsDefault == true ? false : item.AccountIsDefault);

                                                    if (objSuppAccount1 != null && objSuppAccount1.AccountNo.Trim().ToLower() == item.AccountNumber.Trim().ToLower())
                                                    {
                                                        objSuppAccount.IsDefault = item.AccountIsDefault;
                                                    }
                                                    else if (objSuppAccount1 != null && item.AccountIsDefault)
                                                    {
                                                        objSuppAccount.IsDefault = item.AccountIsDefault;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                item.Status = "Fail";
                                                item.Reason = msgNotAllowedToUpdateSupplierAccountDetail;
                                                lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrWhiteSpace(item.AccountNumber))
                                            {

                                                if (IsAllowToInsertSA)
                                                {
                                                    SupplierAccountDetail objSuppAccount1 = (from m in lstSupplierAccountDetails
                                                                                             where m.SupplierID == supplierID && m.Room == RoomId && m.CompanyID == CompanyId && m.IsDefault == true
                                                                                             select m).FirstOrDefault();

                                                    SupplierAccountDetailsDTO oSupplierAccountDetailsDTO = new SupplierAccountDetailsDTO();
                                                    oSupplierAccountDetailsDTO.SupplierID = supplierID;
                                                    oSupplierAccountDetailsDTO.AccountNo = item.AccountNumber;
                                                    oSupplierAccountDetailsDTO.AccountName = item.AccountName;
                                                    oSupplierAccountDetailsDTO.Address = item.AccountAddress;
                                                    oSupplierAccountDetailsDTO.City = item.AccountCity;
                                                    oSupplierAccountDetailsDTO.State = item.AccountState;
                                                    oSupplierAccountDetailsDTO.ZipCode = item.AccountZip;
                                                    oSupplierAccountDetailsDTO.IsDefault = (objSuppAccount1 != null && objSuppAccount1.ID > 0 && objSuppAccount1.IsDefault == true ? false : item.AccountIsDefault);
                                                    oSupplierAccountDetailsDTO.Created = DateTime.Now;
                                                    oSupplierAccountDetailsDTO.CreatedBy = UserId;
                                                    oSupplierAccountDetailsDTO.Updated = DateTime.Now;
                                                    oSupplierAccountDetailsDTO.LastUpdatedBy = UserId;
                                                    oSupplierAccountDetailsDTO.Room = RoomId;
                                                    oSupplierAccountDetailsDTO.CompanyID = CompanyId;
                                                    oSupplierAccountDetailsDTO.IsArchived = false;
                                                    oSupplierAccountDetailsDTO.IsDeleted = false;
                                                    oSupplierAccountDetailsDTO.GUID = Guid.NewGuid();
                                                    oSupplierAccountDetailsDTO.ShipToID = item.AccountShipToID;
                                                    oSupplierAccountDetailsDTO.Country = item.AccountCountry;
                                                    new SupplierAccountDetailsDAL(base.DataBaseName).Insert(oSupplierAccountDetailsDTO);
                                                }
                                                else
                                                {
                                                    item.Status = "Fail";
                                                    item.Reason = msgNotAllowedToInsertSupplierAccountDetail;
                                                    lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                                    continue;
                                                }
                                            }
                                        }

                                        item.Status = "Success";
                                        item.Reason = "N/A";
                                        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                    }
                                    catch (Exception ex)
                                    {
                                        item.Status = "Fail";
                                        item.Reason = ex.Message.ToString();
                                        lstreturnFinal.Add((T)Convert.ChangeType(item, typeof(T)));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                oSuppEdit[0].Status = "Fail";
                                oSuppEdit[0].Reason = ex.Message.ToString();
                                lstreturnFinal.Add((T)Convert.ChangeType(oSuppEdit[0], typeof(T)));
                            }
                        }

                        #endregion
                    }

                    context.SaveChanges();
                }
            }

            return lstreturnFinal;
        }

        private string CreateCreditDataForCount(List<ItemLocationDetailsDTO> objData, string RoomDateFormat, long SessionUserId)
        {
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(base.DataBaseName);
            List<CreditHistory> lstCreditGuids = new List<CreditHistory>();

            if (obj.ItemLocationDetailsSaveForCreditPullnew(objData, "controller", RoomDateFormat, out lstCreditGuids, SessionUserId,0, "adjcredit"))
            {
                return "OK";
            }
            else
                return "Fail";
        }

        public long GetBinForImportItem(string BinNumber, string ItemNumber, long RoomId, long CompanyId, long UserID)
        {
            BinMasterDTO objDTO = null;
            long MasterBinId = 0;
            long ItemBinId = 0;
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);

            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //-----------------------CHECK IF MASTER BIN EXSIST-----------------------
                    //
                    BinMaster objMasterBinMaster = (from BM in context.BinMasters
                                                    where BM.BinNumber.Trim().ToUpper() == BinNumber.Trim().ToUpper()
                                                          && BM.Room == RoomId && BM.CompanyID == CompanyId
                                                          && (BM.IsDeleted == false)
                                                          && (BM.IsArchived == null || BM.IsArchived == false)
                                                          && BM.ParentBinId == null
                                                          && BM.ItemGUID == null
                                                    select BM).FirstOrDefault();

                    if (objMasterBinMaster == null)
                    {
                        //BinMasterDAL objDAL = new BinMasterDAL(base.DataBaseName);
                        objDTO = new BinMasterDTO();
                        objDTO.BinNumber = BinNumber.Length > 128 ? BinNumber.Substring(0, 128) : BinNumber;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = UserID;
                        objDTO.Room = RoomId;
                        objDTO.CompanyID = CompanyId;
                        objDTO.CreatedBy = UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ParentBinId = null;
                        objDTO.ItemGUID = null;
                        objDTO.IsDefault = false;
                        MasterBinId = objBinMasterDAL.Insert(objDTO);
                    }
                    else
                    {
                        MasterBinId = objMasterBinMaster.ID;
                    }

                    //-------------------------CHECK IF ITEM EXISTS WITH ITEM NUMBER-------------------------
                    //
                    ItemMaster objItemMaster = (from IM in context.ItemMasters
                                                where IM.ItemNumber.Trim().ToUpper() == ItemNumber.Trim().ToUpper()
                                                          && IM.Room == RoomId && IM.CompanyID == CompanyId
                                                          && (IM.IsDeleted == null || IM.IsDeleted == false)
                                                          && (IM.IsArchived == null || IM.IsArchived == false)
                                                select IM).FirstOrDefault();

                    BinMaster objItemBinMaster = null;
                    if (objItemMaster != null)
                    {
                        objItemBinMaster = (from BM in context.BinMasters
                                            where BM.BinNumber.Trim().ToUpper() == BinNumber.Trim().ToUpper()
                                                  && BM.Room == RoomId && BM.CompanyID == CompanyId
                                                  && (BM.IsDeleted == false)
                                                  && (BM.IsArchived == null || BM.IsArchived == false)
                                                  && BM.ParentBinId == MasterBinId
                                                && BM.ItemGUID == objItemMaster.GUID
                                            select BM).FirstOrDefault();
                    }

                    if (objItemBinMaster == null)
                    {
                        objDTO = new BinMasterDTO();
                        objDTO.BinNumber = BinNumber.Length > 128 ? BinNumber.Substring(0, 128) : BinNumber;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = UserID;
                        objDTO.Room = RoomId;
                        objDTO.CompanyID = CompanyId;
                        objDTO.CreatedBy = UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ParentBinId = MasterBinId;
                        objDTO.ItemGUID = (objItemMaster != null ? (Guid?)objItemMaster.GUID : null);
                        objDTO.IsDefault = false;
                        objDTO = objBinMasterDAL.InsertBin(objDTO);
                        ItemBinId = objDTO.ID;
                    }
                    else
                    {
                        ItemBinId = objItemBinMaster.ID;
                    }

                    return ItemBinId;
                }
            }
            catch
            {
                return 0;
            }
        }

        public long GetBinIdForImportItem(string BinNumber, string ItemNumber, long RoomId, long CompanyId, long UserID)
        {
            long MasterBinId = 0;
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);

            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //-----------------------CHECK IF MASTER BIN EXSIST-----------------------
                    //
                    BinMaster objMasterBinMaster = (from BM in context.BinMasters
                                                    where BM.BinNumber.Trim().ToUpper() == BinNumber.Trim().ToUpper()
                                                          && BM.Room == RoomId && BM.CompanyID == CompanyId
                                                          && (BM.IsDeleted == false)
                                                          && (BM.IsArchived == null || BM.IsArchived == false)
                                                          && BM.ParentBinId == null
                                                          && BM.ItemGUID == null
                                                    select BM).FirstOrDefault();

                    if (objMasterBinMaster != null)
                    {
                        MasterBinId = objMasterBinMaster.ID;
                    }

                    return MasterBinId;
                }
            }
            catch
            {
                return 0;
            }
        }

        public long GetBinForItemByGUID(string BinNumber, Guid ItemGUID, long RoomId, long CompanyId, long UserID)
        {
            BinMasterDTO objDTO = null;
            long MasterBinId = 0;
            long ItemBinId = 0;
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);

            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //-----------------------CHECK IF MASTER BIN EXSIST-----------------------
                    //
                    BinMaster objMasterBinMaster = (from BM in context.BinMasters
                                                    where BM.BinNumber.Trim().ToUpper() == BinNumber.Trim().ToUpper()
                                                          && BM.Room == RoomId && BM.CompanyID == CompanyId
                                                          && (BM.IsDeleted == false)
                                                          && (BM.IsArchived == null || BM.IsArchived == false)
                                                          && BM.ParentBinId == null
                                                          && BM.ItemGUID == null
                                                    select BM).FirstOrDefault();

                    if (objMasterBinMaster == null)
                    {
                        //BinMasterDAL objDAL = new BinMasterDAL(base.DataBaseName);
                        objDTO = new BinMasterDTO();
                        objDTO.BinNumber = BinNumber.Length > 128 ? BinNumber.Substring(0, 128) : BinNumber;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = UserID;
                        objDTO.Room = RoomId;
                        objDTO.CompanyID = CompanyId;
                        objDTO.CreatedBy = UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ParentBinId = null;
                        objDTO.ItemGUID = null;
                        objDTO.IsDefault = false;
                        MasterBinId = objBinMasterDAL.Insert(objDTO);
                    }
                    else
                    {
                        MasterBinId = objMasterBinMaster.ID;
                    }

                    //-------------------------CHECK IF ITEM EXISTS WITH ITEM NUMBER-------------------------
                    //
                    //ItemMaster objItemMaster = (from IM in context.ItemMasters
                    //                            where IM.ItemNumber.Trim().ToUpper() == ItemNumber.Trim().ToUpper()
                    //                                      && IM.Room == RoomId && IM.CompanyID == CompanyId
                    //                                      && (IM.IsDeleted == null || IM.IsDeleted == false)
                    //                                      && (IM.IsArchived == null || IM.IsArchived == false)
                    //                            select IM).FirstOrDefault();

                    BinMaster objItemBinMaster = null;
                    objItemBinMaster = (from BM in context.BinMasters
                                        where BM.BinNumber.Trim().ToUpper() == BinNumber.Trim().ToUpper()
                                              && BM.Room == RoomId && BM.CompanyID == CompanyId
                                              && (BM.IsDeleted == false)
                                              && (BM.IsArchived == null || BM.IsArchived == false)
                                              && BM.ParentBinId == MasterBinId
                                            && BM.ItemGUID == ItemGUID
                                        select BM).FirstOrDefault();

                    if (objItemBinMaster == null)
                    {
                        objDTO = new BinMasterDTO();
                        objDTO.BinNumber = BinNumber.Length > 128 ? BinNumber.Substring(0, 128) : BinNumber;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = UserID;
                        objDTO.Room = RoomId;
                        objDTO.CompanyID = CompanyId;
                        objDTO.CreatedBy = UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ParentBinId = MasterBinId;
                        objDTO.ItemGUID = ItemGUID;
                        objDTO.IsDefault = false;
                        objDTO = objBinMasterDAL.InsertBin(objDTO);
                        ItemBinId = objDTO.ID;
                    }
                    else
                    {
                        ItemBinId = objItemBinMaster.ID;
                    }

                    return ItemBinId;
                }
            }
            catch
            {
                return 0;
            }
        }

        public void UpdateFTPFileExportLog(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, string ImportDBName, string FileName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@FileName", FileName)
                                                 };

                context.Database.SqlQuery<Int64>(@"EXEC [" + ImportDBName + "].DBO.CSP_UpdateFTPFileExportLog @EnterpriseID,@CompanyID,@RoomID,@FileName", params1);
            }
        }

        /// <summary>
        /// This method is used to import the ToolcheckOutCheckInHistory data
        /// </summary>
        /// <param name="MoveMaterialTable"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <param name="AddedFrom"></param>        
        /// <returns></returns>
        public List<MoveMaterial> ImportMoveMaterial(DataTable MoveMaterialTable, long RoomID, long CompanyID, long UserID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<MoveMaterial> lstMoveMaterial = new List<MoveMaterial>();
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "ImportMoveMaterial", RoomID, CompanyID, UserID, MoveMaterialTable);

                if (dsBins.Tables.Count > 0)
                {
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[dsBins.Tables.Count - 1];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                MoveMaterial moveMaterial = new MoveMaterial();

                                if (dt.Columns.Contains("Id"))
                                {
                                    moveMaterial.Id = Convert.ToInt32(dr["Id"]);
                                }
                                if (dt.Columns.Contains("ItemNumber"))
                                {
                                    moveMaterial.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                                }
                                if (dt.Columns.Contains("SourceBin"))
                                {
                                    moveMaterial.SourceBin = Convert.ToString(dr["SourceBin"]);
                                }
                                if (dt.Columns.Contains("DestinationBin"))
                                {
                                    moveMaterial.DestinationBin = Convert.ToString(dr["DestinationBin"]);
                                }
                                if (dt.Columns.Contains("Quantity"))
                                {
                                    moveMaterial.Quantity = Convert.ToDouble(dr["Quantity"]);
                                }
                                if (dt.Columns.Contains("MoveType"))
                                {
                                    moveMaterial.MoveType = Convert.ToString(dr["MoveType"]);
                                }
                                if (dt.Columns.Contains("DestinationStagingHeader"))
                                {
                                    moveMaterial.DestinationStagingHeader = Convert.ToString(dr["DestinationStagingHeader"]);
                                }
                                if (dt.Columns.Contains("Reason"))
                                {
                                    moveMaterial.Reason = Convert.ToString(dr["Reason"]);
                                }
                                if (dt.Columns.Contains("Status"))
                                {
                                    moveMaterial.Status = string.IsNullOrEmpty(Convert.ToString(dr["Status"]))
                                        ? (string.IsNullOrEmpty(Convert.ToString(dr["Reason"])) ? "Success" : "Fail")
                                        : Convert.ToString(dr["Status"]);
                                }

                                lstMoveMaterial.Add(moveMaterial);
                            }
                        }
                    }
                }
                return lstMoveMaterial;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// This method is used to import the ToolcheckOutCheckInHistory data
        /// </summary>
        /// <param name="EnterpriseQLTable"></param>
        /// <param name="UserID"></param>
        /// <param name="AddedFrom"></param>        
        /// <returns></returns>
        public List<EnterpriseQLImport> ImportEnterpriseQL(DataTable EnterpriseQLTable, long EnterpriseId, long UserID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<EnterpriseQLImport> lstEnterpriseQL = new List<EnterpriseQLImport>();
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "ImportEnterpriseQuickList", UserID, EnterpriseId, EnterpriseQLTable);

                if (dsBins.Tables.Count > 0)
                {
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[dsBins.Tables.Count - 1];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                EnterpriseQLImport enterpriseQL = new EnterpriseQLImport();

                                if (dt.Columns.Contains("Id"))
                                {
                                    enterpriseQL.Id = Convert.ToInt32(dr["Id"]);
                                }
                                if (dt.Columns.Contains("Name"))
                                {
                                    enterpriseQL.Name = Convert.ToString(dr["Name"]);
                                }
                                if (dt.Columns.Contains("QLDetailNumber"))
                                {
                                    enterpriseQL.QLDetailNumber = Convert.ToString(dr["QLDetailNumber"]);
                                }
                                if (dt.Columns.Contains("Quantity"))
                                {
                                    enterpriseQL.Quantity = Convert.ToDouble(dr["Quantity"]);
                                }
                                if (dt.Columns.Contains("Reason"))
                                {
                                    enterpriseQL.Reason = Convert.ToString(dr["Reason"]);
                                }
                                if (dt.Columns.Contains("Status"))
                                {
                                    enterpriseQL.Status = string.IsNullOrEmpty(Convert.ToString(dr["Status"]))
                                        ? (string.IsNullOrEmpty(Convert.ToString(dr["Reason"])) ? "Success" : "Fail")
                                        : Convert.ToString(dr["Status"]);
                                }

                                lstEnterpriseQL.Add(enterpriseQL);
                            }
                        }
                    }
                }
                return lstEnterpriseQL;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void InsertUpdateFileHistory(OfflineImportFileHistoryDTO offlineImportFileHistory)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", offlineImportFileHistory.ID),
                                                   new SqlParameter("@CompanyID", offlineImportFileHistory.CompanyID),
                                                   new SqlParameter("@Room", offlineImportFileHistory.Room),
                                                   new SqlParameter("@ModuleId", offlineImportFileHistory.ModuleId),
                                                   new SqlParameter("@FileName", offlineImportFileHistory.FileName),
                                                   new SqlParameter("@FileUniqueName", offlineImportFileHistory.FileUniqueName),
                                                   new SqlParameter("@FilePath", offlineImportFileHistory.FilePath),
                                                   new SqlParameter("@IsProcessed", offlineImportFileHistory.IsProcessed),
                                                   new SqlParameter("@ProcessStart", offlineImportFileHistory.ProcessStart.HasValue ? offlineImportFileHistory.ProcessStart.Value : (object)DBNull.Value),
                                                   new SqlParameter("@ProcessEnd", offlineImportFileHistory.ProcessEnd.HasValue ? offlineImportFileHistory.ProcessEnd.Value : (object)DBNull.Value),
                                                   new SqlParameter("@UserID", offlineImportFileHistory.CreatedBy),
                                                 };

                context.Database.ExecuteSqlCommand(@"EXEC DBO.InsertUpdateOfflineFileData @ID,@CompanyID,@Room,@ModuleId,@FileName,@FileUniqueName,@FilePath,@IsProcessed,@ProcessStart,@ProcessEnd,@UserID", params1);
            }
        }

        public List<RequisitionImport> ImportRequisition(DataTable ToolCheckOutCheckInTable, long RoomID, long CompanyID, long UserID, string AddedFrom)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<RequisitionImport> lstRequisitionData = new List<RequisitionImport>();
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "ImportRequisition", RoomID, CompanyID, UserID, AddedFrom, ToolCheckOutCheckInTable);

                if (dsBins.Tables.Count > 0)
                {
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[dsBins.Tables.Count - 1];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                RequisitionImport requisition = new RequisitionImport();

                                if (dt.Columns.Contains("Id"))
                                {
                                    requisition.Id = Convert.ToInt32(dr["Id"]);
                                }
                                if (dt.Columns.Contains("RequisitionNumber"))
                                {
                                    requisition.RequisitionNumber = Convert.ToString(dr["RequisitionNumber"]);
                                }
                                if (dt.Columns.Contains("Workorder"))
                                {
                                    requisition.Workorder = Convert.ToString(dr["Workorder"]);
                                }
                                if (dt.Columns.Contains("RequiredDate"))
                                {
                                    requisition.RequiredDate = Convert.ToString(dr["RequiredDate"]);
                                }
                                if (dt.Columns.Contains("RequisitionStatus"))
                                {
                                    requisition.RequisitionStatus = Convert.ToString(dr["RequisitionStatus"]);
                                }
                                if (dt.Columns.Contains("CustomerName"))
                                {
                                    requisition.CustomerName = Convert.ToString(dr["CustomerName"]);
                                }
                                //if (dt.Columns.Contains("ReleaseNumber"))
                                //{
                                //    requisition.ReleaseNumber = Convert.ToString(dr["ReleaseNumber"]);
                                //}
                                if (dt.Columns.Contains("ProjectSpend"))
                                {
                                    requisition.ProjectSpend = Convert.ToString(dr["ProjectSpend"]);
                                }
                                if (dt.Columns.Contains("Description"))
                                {
                                    requisition.Description = Convert.ToString(dr["Description"]);
                                }
                                if (dt.Columns.Contains("StagingName"))
                                {
                                    requisition.StagingName = Convert.ToString(dr["StagingName"]);
                                }
                                if (dt.Columns.Contains("Supplier"))
                                {
                                    requisition.Supplier = Convert.ToString(dr["Supplier"]);
                                }
                                if (dt.Columns.Contains("SupplierAccount"))
                                {
                                    requisition.SupplierAccount = Convert.ToString(dr["SupplierAccount"]);
                                }
                                if (dt.Columns.Contains("BillingAccount"))
                                {
                                    requisition.BillingAccount = Convert.ToString(dr["BillingAccount"]);
                                }
                                if (dt.Columns.Contains("Technician"))
                                {
                                    requisition.Technician = Convert.ToString(dr["Technician"]);
                                }
                                if (dt.Columns.Contains("RequisitionUDF1"))
                                {
                                    requisition.RequisitionUDF1 = Convert.ToString(dr["RequisitionUDF1"]);
                                }
                                if (dt.Columns.Contains("RequisitionUDF2"))
                                {
                                    requisition.RequisitionUDF2 = Convert.ToString(dr["RequisitionUDF2"]);
                                }
                                if (dt.Columns.Contains("RequisitionUDF3"))
                                {
                                    requisition.RequisitionUDF3 = Convert.ToString(dr["RequisitionUDF3"]);
                                }
                                if (dt.Columns.Contains("RequisitionUDF4"))
                                {
                                    requisition.RequisitionUDF4 = Convert.ToString(dr["RequisitionUDF4"]);
                                }
                                if (dt.Columns.Contains("RequisitionUDF5"))
                                {
                                    requisition.RequisitionUDF5 = Convert.ToString(dr["RequisitionUDF5"]);
                                }
                                if (dt.Columns.Contains("ItemNumber"))
                                {
                                    requisition.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                                }
                                if (dt.Columns.Contains("Tool"))
                                {
                                    requisition.Tool = Convert.ToString(dr["Tool"]);
                                }
                                if (dt.Columns.Contains("ToolSerial"))
                                {
                                    requisition.ToolSerial = Convert.ToString(dr["ToolSerial"]);
                                }
                                if (dt.Columns.Contains("Bin"))
                                {
                                    requisition.Bin = Convert.ToString(dr["Bin"]);
                                }
                                if (dt.Columns.Contains("QuantityRequisitioned"))
                                {
                                    double qtyRequisitioned = 0;
                                    double.TryParse(Convert.ToString(dr["QuantityRequisitioned"]),out qtyRequisitioned);
                                    requisition.QuantityRequisitioned = qtyRequisitioned;
                                }
                                if (dt.Columns.Contains("QuantityApproved"))
                                {
                                    double qtyApproved = 0;
                                    if (double.TryParse(Convert.ToString(dr["QuantityApproved"]), out qtyApproved))
                                    {
                                        requisition.QuantityApproved = qtyApproved;
                                    }                                        
                                }
                                if (dt.Columns.Contains("QuantityPulled"))
                                {
                                    double qtyPulled = 0;

                                    if (double.TryParse(Convert.ToString(dr["QuantityPulled"]), out qtyPulled))
                                    {
                                        requisition.QuantityPulled = qtyPulled;
                                    }
                                }
                                if (dt.Columns.Contains("LineItemRequiredDate"))
                                {
                                    requisition.LineItemRequiredDate = Convert.ToString(dr["LineItemRequiredDate"]);
                                }
                                if (dt.Columns.Contains("LineItemProjectSpend"))
                                {
                                    requisition.LineItemProjectSpend = Convert.ToString(dr["LineItemProjectSpend"]);
                                }
                                if (dt.Columns.Contains("LineItemSupplierAccount"))
                                {
                                    requisition.LineItemSupplierAccount = Convert.ToString(dr["LineItemSupplierAccount"]);
                                }
                                if (dt.Columns.Contains("PullOrderNumber"))
                                {
                                    requisition.PullOrderNumber = Convert.ToString(dr["PullOrderNumber"]);
                                }
                                if (dt.Columns.Contains("LineItemTechnician"))
                                {
                                    requisition.LineItemTechnician = Convert.ToString(dr["LineItemTechnician"]);
                                }
                                if (dt.Columns.Contains("PullUDF1"))
                                {
                                    requisition.PullUDF1 = Convert.ToString(dr["PullUDF1"]);
                                }
                                if (dt.Columns.Contains("PullUDF2"))
                                {
                                    requisition.PullUDF2 = Convert.ToString(dr["PullUDF2"]);
                                }
                                if (dt.Columns.Contains("PullUDF3"))
                                {
                                    requisition.PullUDF3 = Convert.ToString(dr["PullUDF3"]);
                                }
                                if (dt.Columns.Contains("PullUDF4"))
                                {
                                    requisition.PullUDF4 = Convert.ToString(dr["PullUDF4"]);
                                }
                                if (dt.Columns.Contains("PullUDF5"))
                                {
                                    requisition.PullUDF5 = Convert.ToString(dr["PullUDF5"]);
                                }
                                if (dt.Columns.Contains("ToolCheckoutUDF1"))
                                {
                                    requisition.ToolCheckoutUDF1 = Convert.ToString(dr["ToolCheckoutUDF1"]);
                                }
                                if (dt.Columns.Contains("ToolCheckoutUDF2"))
                                {
                                    requisition.ToolCheckoutUDF2 = Convert.ToString(dr["ToolCheckoutUDF2"]);
                                }
                                if (dt.Columns.Contains("ToolCheckoutUDF3"))
                                {
                                    requisition.ToolCheckoutUDF3 = Convert.ToString(dr["ToolCheckoutUDF3"]);
                                }
                                if (dt.Columns.Contains("ToolCheckoutUDF4"))
                                {
                                    requisition.ToolCheckoutUDF4 = Convert.ToString(dr["ToolCheckoutUDF4"]);
                                }
                                if (dt.Columns.Contains("ToolCheckoutUDF5"))
                                {
                                    requisition.ToolCheckoutUDF5 = Convert.ToString(dr["ToolCheckoutUDF5"]);
                                }
                                if (dt.Columns.Contains("RequisitionId"))
                                {
                                    long RequisitionId = 0;

                                    if (long.TryParse(Convert.ToString(dr["RequisitionId"]), out RequisitionId))
                                    {
                                        requisition.RequisitionId = RequisitionId;
                                    }
                                }
                                if (dt.Columns.Contains("RequisitionGuid"))
                                {
                                    Guid RequisitionGuid = Guid.Empty;

                                    if (Guid.TryParse(Convert.ToString(dr["RequisitionGuid"]), out RequisitionGuid))
                                    {
                                        requisition.RequisitionGuid = RequisitionGuid;
                                    }
                                }
                                if (dt.Columns.Contains("Reason"))
                                {
                                    requisition.Reason = Convert.ToString(dr["Reason"]);
                                }

                                if (dt.Columns.Contains("Status"))
                                {
                                    requisition.Status = string.IsNullOrEmpty(Convert.ToString(dr["Status"]))
                                        ? (string.IsNullOrEmpty(Convert.ToString(dr["Reason"])) ? "Success" : "Fail")
                                        : Convert.ToString(dr["Status"]);
                                }

                                lstRequisitionData.Add(requisition);
                            }
                        }
                    }
                }
                return lstRequisitionData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<SupplierCatalogImport> ImportSupplierCatalog(DataTable SupplierCatalogTable)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                List<SupplierCatalogImport> lstSupplierCatalogData = new List<SupplierCatalogImport>();
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "ImportSupplierCatalog", SupplierCatalogTable);

                if (dsBins.Tables.Count > 0)
                {
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[dsBins.Tables.Count - 1];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                SupplierCatalogImport supplierCatalog = new SupplierCatalogImport();

                                if (dt.Columns.Contains("Id"))
                                {
                                    supplierCatalog.Id = Convert.ToInt32(dr["Id"]);
                                }
                                if (dt.Columns.Contains("ItemNumber"))
                                {
                                    supplierCatalog.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                                }
                                if (dt.Columns.Contains("Description"))
                                {
                                    supplierCatalog.Description = Convert.ToString(dr["Description"]);
                                }
                                if (dt.Columns.Contains("SellPrice"))
                                {
                                    double qtyRequisitioned = 0;
                                    double.TryParse(Convert.ToString(dr["SellPrice"]), out qtyRequisitioned);
                                    supplierCatalog.SellPrice = qtyRequisitioned;
                                }
                                //if (dt.Columns.Contains("PackingQuantity"))
                                //{
                                //    double qtyRequisitioned = 0;
                                //    double.TryParse(Convert.ToString(dr["PackingQuantity"]), out qtyRequisitioned);
                                //    supplierCatalog.PackingQuantity = qtyRequisitioned;
                                //}
                                
                                if (dt.Columns.Contains("ManufacturerPartNumber"))
                                {
                                    supplierCatalog.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerPartNumber"]);
                                }
                                if (dt.Columns.Contains("ImagePath"))
                                {
                                    supplierCatalog.ImagePath = Convert.ToString(dr["ImagePath"]);
                                }
                                
                                if (dt.Columns.Contains("UPC"))
                                {
                                    supplierCatalog.UPC = Convert.ToString(dr["UPC"]);
                                }
                                if (dt.Columns.Contains("SupplierPartNumber"))
                                {
                                    supplierCatalog.SupplierPartNumber = Convert.ToString(dr["SupplierPartNumber"]);
                                }
                                if (dt.Columns.Contains("SupplierName"))
                                {
                                    supplierCatalog.SupplierName = Convert.ToString(dr["SupplierName"]);
                                }
                                if (dt.Columns.Contains("ManufacturerName"))
                                {
                                    supplierCatalog.ManufacturerName = Convert.ToString(dr["ManufacturerName"]);
                                }
                                //if (dt.Columns.Contains("ConcatedColumnText"))
                                //{
                                //    supplierCatalog.ConcatedColumnText = Convert.ToString(dr["ConcatedColumnText"]);
                                //}
                                if (dt.Columns.Contains("UOM"))
                                {
                                    supplierCatalog.UOM = Convert.ToString(dr["UOM"]);
                                }
                                if (dt.Columns.Contains("CostUOM"))
                                {
                                    supplierCatalog.CostUOM = Convert.ToString(dr["CostUOM"]);
                                }
                                if (dt.Columns.Contains("Cost"))
                                {
                                    double qtyRequisitioned = 0;
                                    double.TryParse(Convert.ToString(dr["Cost"]), out qtyRequisitioned);
                                    supplierCatalog.Cost = qtyRequisitioned;
                                }
                                if (dt.Columns.Contains("Reason"))
                                {
                                    supplierCatalog.Reason = Convert.ToString(dr["Reason"]);
                                }

                                if (dt.Columns.Contains("Status"))
                                {
                                    supplierCatalog.Status = string.IsNullOrEmpty(Convert.ToString(dr["Status"]))
                                        ? (string.IsNullOrEmpty(Convert.ToString(dr["Reason"])) ? "Success" : "Fail")
                                        : Convert.ToString(dr["Status"]);
                                }
                                if (dt.Columns.Contains("UNSPSC"))
                                {
                                    supplierCatalog.UNSPSC = Convert.ToString(dr["UNSPSC"]);
                                }
                                if (dt.Columns.Contains("LongDescription"))
                                {
                                    supplierCatalog.LongDescription = Convert.ToString(dr["LongDescription"]);
                                }
                                if (dt.Columns.Contains("Category"))
                                {
                                    supplierCatalog.Category = Convert.ToString(dr["Category"]);
                                }
                                lstSupplierCatalogData.Add(supplierCatalog);
                            }
                        }
                    }
                }
                return lstSupplierCatalogData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public OfflineImportFileHistoryDTO GetOfflineFileUserEmail(long CompanyID, long RoomID, int ModuleId, string FileName)
        {
            OfflineImportFileHistoryDTO offlineData = new OfflineImportFileHistoryDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@ModuleId", ModuleId),
                                                   new SqlParameter("@FileName", FileName)
                                                 };

                offlineData = context.Database.SqlQuery<OfflineImportFileHistoryDTO>(@"EXEC GetOfflineFileUserEmail @CompanyID,@RoomID,@ModuleId,@FileName", params1).FirstOrDefault();
            }
            return offlineData;
        }
        
    }

    public class ItemQuickListGUIDDTO
    {
        public Guid ItemGUID { get; set; }
        public Guid QuickListGUID { get; set; }
        public long BinID { get; set; }
    }
    public class ItemKitListGUIDDTO
    {
        public Guid ItemGUID { get; set; }
        public Guid KitListGUID { get; set; }
    }
    public class ItemGUIDLocation
    {
        public Guid ItemGUID { get; set; }
        public string LocationName { get; set; }
        public System.Double MaximumQuantity { get; set; }
        public System.Double MinimumQuantity { get; set; }
        public System.Double CriticalQuantity { get; set; }
        public string EntryType { get; set; }
        public long DefaultBlanketPOID { get; set; }
    }

}