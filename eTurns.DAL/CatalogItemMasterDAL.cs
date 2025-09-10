using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class CatalogItemMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public CatalogItemMasterDAL(SessionHelper.EnterPriseDBName)
        //{

        //}

        public CatalogItemMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CatalogItemMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public List<SupplierCatalogItemDTO> GetPagedRecordsByDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string OrderSupplier,long OrderSupplierID)
        {
            //StartRowIndex = StartRowIndex + 1;
            List<SupplierCatalogItemDTO> lstSuppliers = new List<SupplierCatalogItemDTO>();
            DataSet dsSupCatalogItems = new DataSet();
            //string Connectionstring = ConfigurationManager.ConnectionStrings["eTurnsConnection"].ConnectionString;

            TotalCount = 0;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstSuppliers;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string Suppliers = string.Empty, Manufacturers = string.Empty;
            if (!string.IsNullOrWhiteSpace(OrderSupplier))
            {
                Suppliers = OrderSupplier;
               
            }
            else if(OrderSupplierID > 0)
            {
                SupplierMasterDAL objSuppDAL = new SupplierMasterDAL(base.DataBaseName);
                SupplierMasterDTO objDTO = objSuppDAL.GetSupplierByIDPlain(Convert.ToInt64(OrderSupplierID));
                Suppliers = objDTO.SupplierName;
                Suppliers = Suppliers.Replace("'", "''");
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetCatalogItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, Suppliers, Manufacturers, RoomID);
                if (dsSupCatalogItems != null)
                {
                    DataTable dtCataItems = dsSupCatalogItems.Tables[0];
                    if (dtCataItems != null && dtCataItems.Rows.Count > 0)
                    {
                        TotalCount = Convert.ToInt32(dtCataItems.Rows[0]["TotalRecords"]);
                    }
                    foreach (DataRow dr in dtCataItems.Rows)
                    {
                        SupplierCatalogItemDTO objSupplierCatalogItemDTO = new SupplierCatalogItemDTO();
                        if (dtCataItems.Columns.Contains("SupplierCatalogItemID"))
                        {
                            long tempid = 0;
                            long.TryParse(Convert.ToString(dr["SupplierCatalogItemID"]), out tempid);
                            objSupplierCatalogItemDTO.SupplierCatalogItemID = tempid;
                        }
                        if (dtCataItems.Columns.Contains("ItemNumber"))
                        {
                            objSupplierCatalogItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("Description"))
                        {
                            objSupplierCatalogItemDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtCataItems.Columns.Contains("SellPrice"))
                        {
                            double tempPrice = 0;
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                            objSupplierCatalogItemDTO.SellPrice = tempPrice;
                        }
                        if (dtCataItems.Columns.Contains("PackingQuantity"))
                        {
                            double temppq = 0;
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out temppq);
                            objSupplierCatalogItemDTO.PackingQantity = temppq;
                        }
                        if (dtCataItems.Columns.Contains("UPC"))
                        {
                            objSupplierCatalogItemDTO.UPC = Convert.ToString(dr["UPC"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierPartNumber"))
                        {
                            objSupplierCatalogItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierName"))
                        {
                            objSupplierCatalogItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerName"))
                        {
                            objSupplierCatalogItemDTO.ManufacturerName = Convert.ToString(dr["ManufacturerName"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerPartNumber"))
                        {
                            objSupplierCatalogItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerPartNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("ImagePath"))
                        {
                            objSupplierCatalogItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                        }
                        if (dtCataItems.Columns.Contains("UOM"))
                        {
                            objSupplierCatalogItemDTO.UOM = Convert.ToString(dr["UOM"]);
                        }
                        if (dtCataItems.Columns.Contains("CostUOM"))
                        {
                            objSupplierCatalogItemDTO.CostUOM = Convert.ToString(dr["CostUOM"]);
                        }
                        if (dtCataItems.Columns.Contains("Cost"))
                        {
                            double d = 0;
                            double.TryParse(Convert.ToString(dr["Cost"]), out d);
                            objSupplierCatalogItemDTO.Cost = d;
                        }
                        if (dtCataItems.Columns.Contains("UNSPSC"))
                        {
                            objSupplierCatalogItemDTO.UNSPSC = Convert.ToString(dr["UNSPSC"]);
                        }
                        if (dtCataItems.Columns.Contains("Category"))
                        {
                            objSupplierCatalogItemDTO.Category = Convert.ToString(dr["Category"]);
                        }
                        if (dtCataItems.Columns.Contains("LongDescription"))
                        {
                            objSupplierCatalogItemDTO.LongDescription = Convert.ToString(dr["LongDescription"]);
                        }
                        lstSuppliers.Add(objSupplierCatalogItemDTO);
                    }
                }
                return lstSuppliers;
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //string[] arrSuppliers = Fields[1].Split('@')[0].Split(',');
                //string[] arrManufacturers = Fields[1].Split('@')[1].Split(',');

                string[] arrSuppliers = Fields[1].Split('@')[0].Split(',');
                string[] arrManufacturers = { string.Empty }; //= Fields[1].Split('@')[1].Split(',');

                if (Fields[1].Split('@').Count() >= 2)
                {
                    arrManufacturers = Fields[1].Split('@')[1].Split(',');
                }

                foreach (string supitem in arrSuppliers)
                {
                    Suppliers = Suppliers + supitem + "','";
                }
                foreach (string manitem in arrManufacturers)
                {
                    Manufacturers = Manufacturers + manitem + "','";
                }
                Suppliers = Suppliers.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                Manufacturers = Manufacturers.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

                dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetCatalogItems", StartRowIndex, MaxRows, string.Empty, sortColumnName, Suppliers, Manufacturers, RoomID);
                TotalCount = 0;
                if (dsSupCatalogItems != null)
                {
                    DataTable dtCataItems = dsSupCatalogItems.Tables[0];
                    if (dtCataItems != null && dtCataItems.Rows.Count > 0)
                    {
                        TotalCount = Convert.ToInt32(dtCataItems.Rows[0]["TotalRecords"]);
                    }
                    foreach (DataRow dr in dtCataItems.Rows)
                    {
                        SupplierCatalogItemDTO objSupplierCatalogItemDTO = new SupplierCatalogItemDTO();
                        if (dtCataItems.Columns.Contains("SupplierCatalogItemID"))
                        {
                            long tempid = 0;
                            long.TryParse(Convert.ToString(dr["SupplierCatalogItemID"]), out tempid);
                            objSupplierCatalogItemDTO.SupplierCatalogItemID = tempid;
                        }
                        if (dtCataItems.Columns.Contains("ItemNumber"))
                        {
                            objSupplierCatalogItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("Description"))
                        {
                            objSupplierCatalogItemDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtCataItems.Columns.Contains("SellPrice"))
                        {
                            double tempPrice = 0;
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                            objSupplierCatalogItemDTO.SellPrice = tempPrice;
                        }
                        if (dtCataItems.Columns.Contains("PackingQuantity"))
                        {
                            double temppq = 0;
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out temppq);
                            objSupplierCatalogItemDTO.PackingQantity = temppq;
                        }
                        if (dtCataItems.Columns.Contains("UPC"))
                        {
                            objSupplierCatalogItemDTO.UPC = Convert.ToString(dr["UPC"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierPartNumber"))
                        {
                            objSupplierCatalogItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierName"))
                        {
                            objSupplierCatalogItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerName"))
                        {
                            objSupplierCatalogItemDTO.ManufacturerName = Convert.ToString(dr["ManufacturerName"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerPartNumber"))
                        {
                            objSupplierCatalogItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerPartNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("ImagePath"))
                        {
                            objSupplierCatalogItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                        }
                        if (dtCataItems.Columns.Contains("UOM"))
                        {
                            objSupplierCatalogItemDTO.UOM = Convert.ToString(dr["UOM"]);
                        }
                        if (dtCataItems.Columns.Contains("CostUOM"))
                        {
                            objSupplierCatalogItemDTO.CostUOM = Convert.ToString(dr["CostUOM"]);
                        }
                        if (dtCataItems.Columns.Contains("Cost"))
                        {
                            double d = 0;
                            double.TryParse(Convert.ToString(dr["Cost"]), out d);
                            objSupplierCatalogItemDTO.Cost = d;
                        }
                        if (dtCataItems.Columns.Contains("UNSPSC"))
                        {
                            objSupplierCatalogItemDTO.UNSPSC = Convert.ToString(dr["UNSPSC"]);
                        }
                        if (dtCataItems.Columns.Contains("Category"))
                        {
                            objSupplierCatalogItemDTO.Category = Convert.ToString(dr["Category"]);
                        }
                        if (dtCataItems.Columns.Contains("LongDescription"))
                        {
                            objSupplierCatalogItemDTO.LongDescription = Convert.ToString(dr["LongDescription"]);
                        }
                        lstSuppliers.Add(objSupplierCatalogItemDTO);
                    }
                }
                return lstSuppliers;
            }
            else
            {
                dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetCatalogItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, Suppliers, Manufacturers, RoomID);
                TotalCount = 0;
                if (dsSupCatalogItems != null)
                {
                    DataTable dtCataItems = dsSupCatalogItems.Tables[0];
                    if (dtCataItems != null && dtCataItems.Rows.Count > 0)
                    {
                        TotalCount = Convert.ToInt32(dtCataItems.Rows[0]["TotalRecords"]);
                    }
                    foreach (DataRow dr in dtCataItems.Rows)
                    {
                        SupplierCatalogItemDTO objSupplierCatalogItemDTO = new SupplierCatalogItemDTO();
                        if (dtCataItems.Columns.Contains("SupplierCatalogItemID"))
                        {
                            long tempid = 0;
                            long.TryParse(Convert.ToString(dr["SupplierCatalogItemID"]), out tempid);
                            objSupplierCatalogItemDTO.SupplierCatalogItemID = tempid;
                        }
                        if (dtCataItems.Columns.Contains("ItemNumber"))
                        {
                            objSupplierCatalogItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("Description"))
                        {
                            objSupplierCatalogItemDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtCataItems.Columns.Contains("SellPrice"))
                        {
                            double tempPrice = 0;
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                            objSupplierCatalogItemDTO.SellPrice = tempPrice;
                        }
                        if (dtCataItems.Columns.Contains("PackingQuantity"))
                        {
                            double temppq = 0;
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out temppq);
                            objSupplierCatalogItemDTO.PackingQantity = temppq;
                        }
                        if (dtCataItems.Columns.Contains("UPC"))
                        {
                            objSupplierCatalogItemDTO.UPC = Convert.ToString(dr["UPC"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierPartNumber"))
                        {
                            objSupplierCatalogItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierName"))
                        {
                            objSupplierCatalogItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerName"))
                        {
                            objSupplierCatalogItemDTO.ManufacturerName = Convert.ToString(dr["ManufacturerName"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerPartNumber"))
                        {
                            objSupplierCatalogItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerPartNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("ImagePath"))
                        {
                            objSupplierCatalogItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                        }
                        if (dtCataItems.Columns.Contains("UOM"))
                        {
                            objSupplierCatalogItemDTO.UOM = Convert.ToString(dr["UOM"]);
                        }
                        if (dtCataItems.Columns.Contains("CostUOM"))
                        {
                            objSupplierCatalogItemDTO.CostUOM = Convert.ToString(dr["CostUOM"]);
                        }
                        if (dtCataItems.Columns.Contains("Cost"))
                        {
                            double d = 0;
                            double.TryParse(Convert.ToString(dr["Cost"]), out d);
                            objSupplierCatalogItemDTO.Cost = d;
                        }
                        if (dtCataItems.Columns.Contains("UNSPSC"))
                        {
                            objSupplierCatalogItemDTO.UNSPSC = Convert.ToString(dr["UNSPSC"]);
                        }
                        if (dtCataItems.Columns.Contains("Category"))
                        {
                            objSupplierCatalogItemDTO.Category = Convert.ToString(dr["Category"]);
                        }
                        if (dtCataItems.Columns.Contains("LongDescription"))
                        {
                            objSupplierCatalogItemDTO.LongDescription = Convert.ToString(dr["LongDescription"]);
                        }
                        lstSuppliers.Add(objSupplierCatalogItemDTO);
                    }
                }
                return lstSuppliers;
            }

        }

        public IEnumerable<SupplierCatalogItemDTO> GetSuppliers()
        {
            IEnumerable<SupplierCatalogItemDTO> lstSuppliers = null;
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSuppliers = (from scm in dbcntx.SupplierCatalogs
                                group scm by new { scm.SupplierName } into groupSups
                                select new SupplierCatalogItemDTO
                                {
                                    SupplierName = groupSups.Key.SupplierName
                                });
            }
            return lstSuppliers;
        }

        public ItemMasterDTO GetItemByItemName(string ItemNumber, Int64 CompanyID, Int64 RoomId)
        {
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] { new SqlParameter("@ItemNumber", ItemNumber)
										            ,new SqlParameter("@RoomID", RoomId)
                                                    ,new SqlParameter("@CompanyID", CompanyID) };

                objItemMasterDTO = (from u in context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemByItemName] @ItemNumber,@RoomID,@CompanyID", sqlParams)
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber,
                                        ManufacturerName = u.ManufacturerName,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo,
                                        SupplierName = u.SupplierName,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description,
                                        LongDescription = u.LongDescription,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,
                                        PricePerTerm = u.PricePerTerm,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity,
                                        DefaultPullQuantity = u.DefaultPullQuantity,
                                        Cost = u.Cost,
                                        Markup = u.Markup,
                                        SellPrice = u.SellPrice,
                                        ExtendedCost = u.ExtendedCost,
                                        LeadTimeInDays = u.LeadTimeInDays,
                                        Trend = u.Trend,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity,
                                        OnOrderQuantity = u.OnOrderQuantity,
                                        OnReturnQuantity = u.OnReturnQuantity,
                                        OnTransferQuantity = u.OnTransferQuantity,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        AverageUsage = u.AverageUsage,
                                        Turns = u.Turns,
                                        OnHandQuantity = u.OnHandQuantity,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        ItemUniqueNumber = u.ItemUniqueNumber,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        InventoryClassification = u.InventoryClassification,
                                        SerialNumberTracking = u.SerialNumberTracking,
                                        LotNumberTracking = u.LotNumberTracking,
                                        DateCodeTracking = u.DateCodeTracking,
                                        ItemType = u.ItemType,
                                        ImagePath = u.ImagePath,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        GUID = u.GUID,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                        CategoryName = u.CategoryName,
                                        Unit = u.Unit,
                                        GLAccount = u.GLAccount,
                                        ItemTypeName = u.ItemTypeName,
                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,

                                    }).FirstOrDefault();



            }
            return objItemMasterDTO;
        }

        public List<SupplierCatalogItemDTO> SupplierCatalogsCSV(string SuppliersName,string Manufacturers,long RoomID)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SuppliersName = SuppliersName.Replace("'", "''");
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            List<SupplierCatalogItemDTO> supplierCatalogItems = new List<SupplierCatalogItemDTO>();
            try
            {
                var dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetCatalogItemsForCSV", SuppliersName, RoomID);
                DataTable dtSupCatalogItems = dsSupCatalogItems.Tables[0];
                foreach (DataRow dr in dtSupCatalogItems.Rows)
                {
                    SupplierCatalogItemDTO objSupplierCatalogItemDTO = new SupplierCatalogItemDTO();
                    if (dtSupCatalogItems.Columns.Contains("SupplierCatalogItemID"))
                    {
                        long tempid = 0;
                        long.TryParse(Convert.ToString(dr["SupplierCatalogItemID"]), out tempid);
                        objSupplierCatalogItemDTO.SupplierCatalogItemID = tempid;
                    }
                    if (dtSupCatalogItems.Columns.Contains("ItemNumber"))
                    {
                        objSupplierCatalogItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("Description"))
                    {
                        objSupplierCatalogItemDTO.Description = Convert.ToString(dr["Description"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("SellPrice"))
                    {
                        double tempPrice = 0;
                        double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                        objSupplierCatalogItemDTO.SellPrice = tempPrice;
                    }
                    if (dtSupCatalogItems.Columns.Contains("ManufacturerPartNumber"))
                    {
                        objSupplierCatalogItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerPartNumber"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("ImagePath"))
                    {
                        objSupplierCatalogItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("UPC"))
                    {
                        objSupplierCatalogItemDTO.UPC = Convert.ToString(dr["UPC"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("SupplierPartNumber"))
                    {
                        objSupplierCatalogItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNumber"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("SupplierName"))
                    {
                        objSupplierCatalogItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("ManufacturerName"))
                    {
                        objSupplierCatalogItemDTO.ManufacturerName = Convert.ToString(dr["ManufacturerName"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("UOM"))
                    {
                        objSupplierCatalogItemDTO.UOM = Convert.ToString(dr["UOM"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("CostUOM"))
                    {
                        objSupplierCatalogItemDTO.CostUOM = Convert.ToString(dr["CostUOM"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("Cost"))
                    {
                        double tempCost = 0;
                        double.TryParse(Convert.ToString(dr["Cost"]), out tempCost);
                        objSupplierCatalogItemDTO.Cost = tempCost;
                    }

                    if (dtSupCatalogItems.Columns.Contains("UNSPSC"))
                    {
                        objSupplierCatalogItemDTO.UNSPSC = Convert.ToString(dr["UNSPSC"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("Category"))
                    {
                        objSupplierCatalogItemDTO.Category = Convert.ToString(dr["Category"]);
                    }
                    if (dtSupCatalogItems.Columns.Contains("LongDescription"))
                    {
                        objSupplierCatalogItemDTO.LongDescription = Convert.ToString(dr["LongDescription"]);
                    }
                    supplierCatalogItems.Add(objSupplierCatalogItemDTO);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return supplierCatalogItems;

        }


    }
}


