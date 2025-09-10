using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static eTurns.DTO.ImportDTO;

namespace eTurns.DAL
{
    public partial class BOMItemMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]



        //public BOMItemMasterDAL(base.DataBaseName)
        //{

        //}

        public BOMItemMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public BOMItemMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public IEnumerable<BOMItemDTO> GetSuppliers()
        {
            IEnumerable<BOMItemDTO> lstSuppliers = null;
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSuppliers = (from scm in dbcntx.SupplierCatalogs
                                group scm by new { scm.SupplierName } into groupSups
                                select new BOMItemDTO
                                {
                                    SupplierName = groupSups.Key.SupplierName
                                });
            }
            return lstSuppliers;
        }

        public List<BOMItemDTO> GetPagedRecordsByDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string OrderSupplier, string RoomIds)
        {
            //StartRowIndex = StartRowIndex + 1;
            List<BOMItemDTO> lstSuppliers = new List<BOMItemDTO>();
            DataSet dsSupCatalogItems = new DataSet();
            //string Connectionstring = ConfigurationManager.ConnectionStrings["eTurnsConnection"].ConnectionString;

            TotalCount = 0;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstSuppliers;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string Suppliers = string.Empty, Manufacturers = string.Empty, rooms = string.Empty;
            if (!string.IsNullOrWhiteSpace(OrderSupplier))
            {
                Suppliers = OrderSupplier;
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetBomItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, Suppliers, Manufacturers, rooms, RoomID, RoomIds);
                if (dsSupCatalogItems != null)
                {
                    DataTable dtCataItems = dsSupCatalogItems.Tables[0];
                    if (dtCataItems != null && dtCataItems.Rows.Count > 0)
                    {
                        TotalCount = Convert.ToInt32(dtCataItems.Rows[0]["TotalRecords"]);
                    }
                    foreach (DataRow dr in dtCataItems.Rows)
                    {
                        BOMItemDTO objBOMItemDTO = new BOMItemDTO();
                        if (dtCataItems.Columns.Contains("ID"))
                        {
                            long tempid = 0;
                            long.TryParse(Convert.ToString(dr["ID"]), out tempid);
                            objBOMItemDTO.ID = tempid;
                        }
                        if (dtCataItems.Columns.Contains("ItemNumber"))
                        {
                            objBOMItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("Description"))
                        {
                            objBOMItemDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtCataItems.Columns.Contains("SellPrice"))
                        {
                            double tempPrice = 0;
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                            objBOMItemDTO.SellPrice = tempPrice;
                        }
                        if (dtCataItems.Columns.Contains("PackingQuantity"))
                        {
                            double temppq = 0;
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out temppq);
                            objBOMItemDTO.PackingQantity = temppq;
                        }
                        if (dtCataItems.Columns.Contains("UPC"))
                        {
                            objBOMItemDTO.UPC = Convert.ToString(dr["UPC"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierPartNo"))
                        {
                            objBOMItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNo"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierName"))
                        {
                            objBOMItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCataItems.Columns.Contains("Manufacturer"))
                        {
                            objBOMItemDTO.ManufacturerName = Convert.ToString(dr["Manufacturer"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerNumber"))
                        {
                            objBOMItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("ImagePath"))
                        {
                            objBOMItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                        }
                        if (dtCataItems.Columns.Contains("ImageType"))
                        {
                            objBOMItemDTO.ImageType = Convert.ToString(dr["ImageType"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemLink2ImageType"))
                        {
                            objBOMItemDTO.ItemLink2ImageType = Convert.ToString(dr["ItemLink2ImageType"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemLink2ExternalURL"))
                        {
                            objBOMItemDTO.ItemLink2ExternalURL = Convert.ToString(dr["ItemLink2ExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemImageExternalURL"))
                        {
                            objBOMItemDTO.ItemImageExternalURL = Convert.ToString(dr["ItemImageExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("IsActive"))
                        {
                            objBOMItemDTO.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemDocExternalURL"))
                        {
                            objBOMItemDTO.ItemDocExternalURL = Convert.ToString(dr["ItemDocExternalURL"]);
                        }

                        if (dtCataItems.Columns.Contains("UDF1"))
                        {
                            objBOMItemDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF2"))
                        {
                            objBOMItemDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF3"))
                        {
                            objBOMItemDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF4"))
                        {
                            objBOMItemDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF5"))
                        {
                            objBOMItemDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        lstSuppliers.Add(objBOMItemDTO);
                    }
                }
                return lstSuppliers;
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                rooms = Fields[1].Split('@')[2];
                Suppliers = Fields[1].Split('@')[3];
                Manufacturers = Fields[1].Split('@')[4];

                dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetBomItems", StartRowIndex, MaxRows, string.Empty, sortColumnName, Suppliers, Manufacturers, rooms, RoomID, RoomIds);
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
                        BOMItemDTO objBOMItemDTO = new BOMItemDTO();
                        if (dtCataItems.Columns.Contains("ID"))
                        {
                            long tempid = 0;
                            long.TryParse(Convert.ToString(dr["ID"]), out tempid);
                            objBOMItemDTO.ID = tempid;
                        }
                        if (dtCataItems.Columns.Contains("ItemNumber"))
                        {
                            objBOMItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("Description"))
                        {
                            objBOMItemDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtCataItems.Columns.Contains("SellPrice"))
                        {
                            double tempPrice = 0;
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                            objBOMItemDTO.SellPrice = tempPrice;
                        }
                        if (dtCataItems.Columns.Contains("PackingQuantity"))
                        {
                            double temppq = 0;
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out temppq);
                            objBOMItemDTO.PackingQantity = temppq;
                        }
                        if (dtCataItems.Columns.Contains("UPC"))
                        {
                            objBOMItemDTO.UPC = Convert.ToString(dr["UPC"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierPartNo"))
                        {
                            objBOMItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNo"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierName"))
                        {
                            objBOMItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCataItems.Columns.Contains("Manufacturer"))
                        {
                            objBOMItemDTO.ManufacturerName = Convert.ToString(dr["Manufacturer"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerNumber"))
                        {
                            objBOMItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("ImagePath"))
                        {
                            objBOMItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                        }
                        if (dtCataItems.Columns.Contains("ImageType"))
                        {
                            objBOMItemDTO.ImageType = Convert.ToString(dr["ImageType"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemLink2ImageType"))
                        {
                            objBOMItemDTO.ItemLink2ImageType = Convert.ToString(dr["ItemLink2ImageType"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemLink2ExternalURL"))
                        {
                            objBOMItemDTO.ItemLink2ExternalURL = Convert.ToString(dr["ItemLink2ExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemImageExternalURL"))
                        {
                            objBOMItemDTO.ItemImageExternalURL = Convert.ToString(dr["ItemImageExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("IsActive"))
                        {
                            objBOMItemDTO.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemDocExternalURL"))
                        {
                            objBOMItemDTO.ItemDocExternalURL = Convert.ToString(dr["ItemDocExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF1"))
                        {
                            objBOMItemDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF2"))
                        {
                            objBOMItemDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF3"))
                        {
                            objBOMItemDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF4"))
                        {
                            objBOMItemDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF5"))
                        {
                            objBOMItemDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        lstSuppliers.Add(objBOMItemDTO);
                    }
                }
                return lstSuppliers;
            }
            else
            {
                dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetBomItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, Suppliers, Manufacturers, rooms, RoomID, RoomIds);
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
                        BOMItemDTO objBOMItemDTO = new BOMItemDTO();
                        if (dtCataItems.Columns.Contains("ID"))
                        {
                            long tempid = 0;
                            long.TryParse(Convert.ToString(dr["ID"]), out tempid);
                            objBOMItemDTO.ID = tempid;
                        }
                        if (dtCataItems.Columns.Contains("ItemNumber"))
                        {
                            objBOMItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("Description"))
                        {
                            objBOMItemDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtCataItems.Columns.Contains("SellPrice"))
                        {
                            double tempPrice = 0;
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                            objBOMItemDTO.SellPrice = tempPrice;
                        }
                        if (dtCataItems.Columns.Contains("PackingQuantity"))
                        {
                            double temppq = 0;
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out temppq);
                            objBOMItemDTO.PackingQantity = temppq;
                        }
                        if (dtCataItems.Columns.Contains("UPC"))
                        {
                            objBOMItemDTO.UPC = Convert.ToString(dr["UPC"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierPartNo"))
                        {
                            objBOMItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNo"]);
                        }
                        if (dtCataItems.Columns.Contains("SupplierName"))
                        {
                            objBOMItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCataItems.Columns.Contains("Manufacturer"))
                        {
                            objBOMItemDTO.ManufacturerName = Convert.ToString(dr["Manufacturer"]);
                        }
                        if (dtCataItems.Columns.Contains("ManufacturerNumber"))
                        {
                            objBOMItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerNumber"]);
                        }
                        if (dtCataItems.Columns.Contains("ImagePath"))
                        {
                            objBOMItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                        }
                        if (dtCataItems.Columns.Contains("ImageType"))
                        {
                            objBOMItemDTO.ImageType = Convert.ToString(dr["ImageType"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemLink2ImageType"))
                        {
                            objBOMItemDTO.ItemLink2ImageType = Convert.ToString(dr["ItemLink2ImageType"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemLink2ExternalURL"))
                        {
                            objBOMItemDTO.ItemLink2ExternalURL = Convert.ToString(dr["ItemLink2ExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemImageExternalURL"))
                        {
                            objBOMItemDTO.ItemImageExternalURL = Convert.ToString(dr["ItemImageExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("IsActive"))
                        {
                            objBOMItemDTO.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        }
                        if (dtCataItems.Columns.Contains("ItemDocExternalURL"))
                        {
                            objBOMItemDTO.ItemDocExternalURL = Convert.ToString(dr["ItemDocExternalURL"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF1"))
                        {
                            objBOMItemDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF2"))
                        {
                            objBOMItemDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF3"))
                        {
                            objBOMItemDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF4"))
                        {
                            objBOMItemDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCataItems.Columns.Contains("UDF5"))
                        {
                            objBOMItemDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        lstSuppliers.Add(objBOMItemDTO);
                    }
                }
                return lstSuppliers;
            }

        }

        public BOMItemDTO GetItemByItemName(string ItemNumber, Int64 CompanyID, Int64 RoomId)
        {
            BOMItemDTO objItemMasterDTO = new BOMItemDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] { new SqlParameter("@ItemNumber", ItemNumber)
										            ,new SqlParameter("@RoomID", RoomId)
                                                    ,new SqlParameter("@CompanyID", CompanyID) };

                objItemMasterDTO = (from u in context.Database.SqlQuery<BOMItemDTO>("exec [GetItemByItemName] @ItemNumber,@RoomID,@CompanyID", sqlParams)
                                    select new BOMItemDTO
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
                                        ItemLink2ImageType = u.ItemLink2ImageType,
                                        ImageType = u.ImageType,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        ItemImageExternalURL = u.ItemImageExternalURL,
                                        IsActive = u.IsActive,
                                        ItemDocExternalURL = u.ItemDocExternalURL
                                    }).FirstOrDefault();



            }
            return objItemMasterDTO;
        }

        public BOMItemDTO GetItemByItemID(long ID)
        {
            BOMItemDTO objItemMasterDTO = new BOMItemDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] { new SqlParameter("@ID", ID) };
                objItemMasterDTO = (from u in context.Database.SqlQuery<BOMItemDTO>("Exec [GetItemByItemID] @ID", sqlParams)
                                    select new BOMItemDTO
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
                                        ItemLink2ImageType = u.ItemLink2ImageType,
                                        ImageType = u.ImageType,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        ItemImageExternalURL = u.ItemImageExternalURL,
                                        IsActive = u.IsActive,
                                        ItemDocExternalURL = u.ItemDocExternalURL
                                    }).FirstOrDefault();



            }
            return objItemMasterDTO;
        }

        public void AddItemsToRoomWithoutAutoSot(string strIds, long TargetRoomId, BOMItemMasterMain objEditItem, long UserId)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string StoredCommand = "EXEC CopyItemToRoomWithoutautosot '" + strIds + "'," + TargetRoomId + "," + UserId + ",'" + objEditItem.ManufacturerName + "','" + objEditItem.InventoryClassificationName + "'," + objEditItem.DefaultReorderQuantity.GetValueOrDefault(0) + "," + objEditItem.DefaultPullQuantity.GetValueOrDefault(0) + "," + objEditItem.LeadTimeInDays.GetValueOrDefault(0) + ",'" + objEditItem.Link1 + "'";
                    StoredCommand += ",'" + objEditItem.Link2 + "'," + objEditItem.Taxable + "," + objEditItem.Consignment + "," + objEditItem.ISNullConsignment + "," + objEditItem.OnHandQuantity.GetValueOrDefault(0) + "," + objEditItem.CriticalQuantity + "," + objEditItem.MinimumQuantity + "," + objEditItem.MaximumQuantity + "," + objEditItem.WeightPerPiece.GetValueOrDefault(0) + ",'" + objEditItem.ItemUniqueNumber + "'," + objEditItem.IsTransfer + "," + objEditItem.IsPurchase + "";
                    StoredCommand += ",'" + objEditItem.InventryLocation + "'," + objEditItem.SerialNumberTracking + "," + objEditItem.LotNumberTracking + "," + objEditItem.DateCodeTracking + ",'" + objEditItem.ItemTypeName + "','" + objEditItem.ImagePath + "','" + objEditItem.UDF1 + "','" + objEditItem.UDF2 + "','" + objEditItem.UDF3 + "','" + objEditItem.UDF4 + "','" + objEditItem.UDF5 + "','" + objEditItem.IsLotSerialExpiryCost + "'";
                    StoredCommand += "," + objEditItem.IsItemLevelMinMaxQtyRequired.GetValueOrDefault() + "," + objEditItem.IsEnforceDefaultReorderQuantity.GetValueOrDefault() + ",'" + objEditItem.ItemImageExternalURL + "','" + objEditItem.ItemLink2ExternalURL + "','" + objEditItem.ItemDocExternalURL + "'," + objEditItem.IsBuildBreak.GetValueOrDefault() + "," + objEditItem.IsAutoInventoryClassification + "," + objEditItem.IsPackslipMandatoryAtReceive + "," + objEditItem.IsDeleted.GetValueOrDefault() + "";
                    StoredCommand += "," + objEditItem.IsActive + "," + objEditItem.IsActive + ",'" + objEditItem.UDF6 + "','" + objEditItem.UDF7 + "','" + objEditItem.UDF8 + "','" + objEditItem.UDF9 + "','" + objEditItem.UDF10 + "'," + objEditItem.IsAllowOrderCostuom + ",'" + objEditItem.eLabelKey + "'," + objEditItem.POItemLineNumber.GetValueOrDefault(0) + "," + objEditItem.PullQtyScanOverride + ",'" + objEditItem.TrendingSettingName + "','"+ objEditItem.BlanketOrderNumber +"';";
                    context.Database.CommandTimeout = 3600;
                    context.Database.ExecuteSqlCommand(StoredCommand);
                    objEditItem.Status = "Success";
                }
            }catch(Exception ex)
            {
                objEditItem.Status = "fail";
                objEditItem.Reason = ex.StackTrace.ToString();
            }
        }

        public void AddItemsToRoom(string strIds, long TargetRoomId, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string StoredCommand = "EXEC CopyItemToRoom '" + strIds + "'," + TargetRoomId + "," + UserId;
                context.Database.CommandTimeout = 3600;
                context.Database.ExecuteSqlCommand(StoredCommand);
            }
        }

        public List<BOMItemDTO> GetAllBOMs(Int64 RoomID, Int64 CompanyId, string RoomIds)
        {

            List<BOMItemDTO> lstSuppliers = new List<BOMItemDTO>();
            DataSet dsSupCatalogItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstSuppliers;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string Suppliers = string.Empty, Manufacturers = string.Empty, rooms = string.Empty;

            dsSupCatalogItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetALLBomItems", 0, 0, string.Empty, "BOMItemID asc", string.Empty, string.Empty, RoomID, RoomIds);
            if (dsSupCatalogItems != null)
            {
                DataTable dtCataItems = dsSupCatalogItems.Tables[0];

                foreach (DataRow dr in dtCataItems.Rows)
                {
                    BOMItemDTO objBOMItemDTO = new BOMItemDTO();
                    if (dtCataItems.Columns.Contains("ID"))
                    {
                        long tempid = 0;
                        long.TryParse(Convert.ToString(dr["ID"]), out tempid);
                        objBOMItemDTO.ID = tempid;
                    }
                    if (dtCataItems.Columns.Contains("Room"))
                    {
                        long tempid = 0;
                        long.TryParse(Convert.ToString(dr["Room"]), out tempid);
                        objBOMItemDTO.RoomId = tempid;
                    }
                    if (dtCataItems.Columns.Contains("RoomName"))
                    {
                        objBOMItemDTO.RoomName = Convert.ToString(dr["RoomName"]);
                    }
                    if (dtCataItems.Columns.Contains("SupplierID"))
                    {
                        long tempid = 0;
                        long.TryParse(Convert.ToString(dr["SupplierID"]), out tempid);
                        objBOMItemDTO.SupplierID = tempid;
                    }
                    if (dtCataItems.Columns.Contains("ManufacturerID"))
                    {
                        long tempid = 0;
                        long.TryParse(Convert.ToString(dr["ManufacturerID"]), out tempid);
                        objBOMItemDTO.ManufacturerID = tempid;
                    }
                    if (dtCataItems.Columns.Contains("ItemNumber"))
                    {
                        objBOMItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                    }
                    if (dtCataItems.Columns.Contains("Description"))
                    {
                        objBOMItemDTO.Description = Convert.ToString(dr["Description"]);
                    }
                    if (dtCataItems.Columns.Contains("SellPrice"))
                    {
                        double tempPrice = 0;
                        double.TryParse(Convert.ToString(dr["SellPrice"]), out tempPrice);
                        objBOMItemDTO.SellPrice = tempPrice;
                    }
                    if (dtCataItems.Columns.Contains("PackingQuantity"))
                    {
                        double temppq = 0;
                        double.TryParse(Convert.ToString(dr["PackingQuantity"]), out temppq);
                        objBOMItemDTO.PackingQantity = temppq;
                    }
                    if (dtCataItems.Columns.Contains("UPC"))
                    {
                        objBOMItemDTO.UPC = Convert.ToString(dr["UPC"]);
                    }
                    if (dtCataItems.Columns.Contains("SupplierPartNo"))
                    {
                        objBOMItemDTO.SupplierPartNumber = Convert.ToString(dr["SupplierPartNo"]);
                    }
                    if (dtCataItems.Columns.Contains("SupplierName"))
                    {
                        objBOMItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                    }
                    if (dtCataItems.Columns.Contains("Manufacturer"))
                    {
                        objBOMItemDTO.ManufacturerName = Convert.ToString(dr["Manufacturer"]);
                    }
                    if (dtCataItems.Columns.Contains("ManufacturerNumber"))
                    {
                        objBOMItemDTO.ManufacturerPartNumber = Convert.ToString(dr["ManufacturerNumber"]);
                    }
                    if (dtCataItems.Columns.Contains("ImagePath"))
                    {
                        objBOMItemDTO.ImagePath = Convert.ToString(dr["ImagePath"]);
                    }
                    if (dtCataItems.Columns.Contains("ItemLink2ImageType"))
                    {
                        objBOMItemDTO.ItemLink2ImageType = Convert.ToString(dr["ItemLink2ImageType"]);
                    }
                    if (dtCataItems.Columns.Contains("ImageType"))
                    {
                        objBOMItemDTO.ImageType = Convert.ToString(dr["ImageType"]);
                    }
                    if (dtCataItems.Columns.Contains("ItemLink2ExternalURL"))
                    {
                        objBOMItemDTO.ItemLink2ExternalURL = Convert.ToString(dr["ItemLink2ExternalURL"]);
                    }
                    if (dtCataItems.Columns.Contains("ItemImageExternalURL"))
                    {
                        objBOMItemDTO.ItemImageExternalURL = Convert.ToString(dr["ItemImageExternalURL"]);
                    }
                    if (dtCataItems.Columns.Contains("IsActive"))
                    {
                        objBOMItemDTO.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    }
                    if (dtCataItems.Columns.Contains("ItemDocExternalURL"))
                    {
                        objBOMItemDTO.ItemDocExternalURL = Convert.ToString(dr["ItemDocExternalURL"]);
                    }
                    if (dtCataItems.Columns.Contains("UDF1"))
                    {
                        objBOMItemDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                    }
                    if (dtCataItems.Columns.Contains("UDF2"))
                    {
                        objBOMItemDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                    }
                    if (dtCataItems.Columns.Contains("UDF3"))
                    {
                        objBOMItemDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                    }
                    if (dtCataItems.Columns.Contains("UDF4"))
                    {
                        objBOMItemDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                    }
                    if (dtCataItems.Columns.Contains("UDF5"))
                    {
                        objBOMItemDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                    }
                    lstSuppliers.Add(objBOMItemDTO);
                }
            }
            return lstSuppliers;

        }


        public string BOMDuplicateCheck(long ID, string Itemnumber, long CompanyID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.ItemMasters
                           where em.ItemNumber == Itemnumber && em.IsArchived == false && em.IsDeleted == false && em.ID != ID && em.CompanyID == CompanyID && em.IsBOMItem == true
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;
        }

        /// <summary>
        /// Get Paged Records from the ItemMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<BOMItemDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetAllBOMItems(CompanyId).OrderBy("ID DESC");
        }
        
        public BOMItemDTO BOMItemByGuid(Guid GUID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //return GetAllBOMItems(CompanyID, Guid.Parse(GUID));
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@GUID", GUID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted) };
                return context.Database.SqlQuery<BOMItemDTO>("exec [BOMItemByGuid] @CompanyID,@GUID,@IsArchived,@IsDeleted", params1).FirstOrDefault();
            }
        }
        public List<BOMItemDTO> CompanyBOMItems(Int64 CompanyID, bool IsArchived, bool IsDeleted, string BOMIds = null)
        {
            //return GetAllBOMItems(CompanyID, Guid.Parse(GUID));
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@BOMIds", BOMIds) };
                List<SqlParameter> sqlParas = new List<SqlParameter>();
                sqlParas.Add(new SqlParameter("@CompanyID", CompanyID));
                sqlParas.Add(new SqlParameter("@IsArchived", IsArchived));
                sqlParas.Add(new SqlParameter("@IsDeleted", IsDeleted));

                if (!string.IsNullOrEmpty(BOMIds))
                    sqlParas.Add(new SqlParameter("@BOMIds", BOMIds));
                else
                    sqlParas.Add(new SqlParameter("@BOMIds", DBNull.Value));

                return context.Database.SqlQuery<BOMItemDTO>("exec [CompanyBOMItems] @CompanyID,@IsArchived,@IsDeleted,@BOMIds", sqlParas.ToArray()).ToList();
            }
        }
        

        /// <summary>
        /// Insert Record in the DataBase ItemMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(BOMItemDTO objDTO)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = new ItemMaster();
                obj.ID = 0;
                obj.ItemNumber = objDTO.ItemNumber;


                if (objDTO.ManufacturerID > 0)
                    obj.ManufacturerID = objDTO.ManufacturerID;

                obj.ManufacturerNumber = objDTO.ManufacturerNumber;

                if (objDTO.ItemType != 4)
                {
                    if (objDTO.SupplierID > 0)
                        obj.SupplierID = objDTO.SupplierID;
                    obj.SupplierPartNo = objDTO.SupplierPartNo;
                }
                obj.UPC = objDTO.UPC;
                obj.UNSPSC = objDTO.UNSPSC;
                obj.Description = objDTO.Description;
                obj.LongDescription = objDTO.LongDescription;
                if (objDTO.CategoryID > 0)
                    obj.CategoryID = objDTO.CategoryID;

                obj.GLAccountID = objDTO.GLAccountID;

                if (objDTO.UOMID > 0)
                    obj.UOMID = objDTO.UOMID;

                obj.PricePerTerm = objDTO.PricePerTerm;
                obj.CostUOMID = objDTO.CostUOMID;
                obj.CategoryID = objDTO.CategoryID;
                obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                obj.Cost = objDTO.Cost;
                obj.Markup = objDTO.Markup;
                obj.SellPrice = objDTO.SellPrice;
                //obj.ExtendedCost = objDTO.ExtendedCost;
                obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                obj.Link1 = objDTO.Link1;
                obj.Link2 = objDTO.Link2;
                obj.Trend = objDTO.Trend;
                obj.Taxable = objDTO.Taxable;
                obj.Consignment = objDTO.Consignment;
                obj.StagedQuantity = objDTO.StagedQuantity;
                obj.InTransitquantity = objDTO.InTransitquantity;
                obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                obj.OnReturnQuantity = objDTO.OnReturnQuantity;
                obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                obj.PackingQuantity = objDTO.PackingQuantity;
                obj.AverageUsage = objDTO.AverageUsage;
                obj.Turns = objDTO.Turns;
                obj.OnHandQuantity = objDTO.OnHandQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.WeightPerPiece = objDTO.WeightPerPiece;
                //obj.ItemUniqueNumber = objCommonDAL.UniqueItemId();
                obj.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                obj.IsTransfer = objDTO.IsTransfer;
                obj.IsPurchase = objDTO.IsPurchase;


                obj.DefaultLocation = 0;

                obj.InventoryClassification = objDTO.InventoryClassification;
                obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                obj.LotNumberTracking = objDTO.LotNumberTracking;
                obj.DateCodeTracking = objDTO.DateCodeTracking;
                obj.ItemType = objDTO.ItemType;
                obj.ImagePath = objDTO.ImagePath;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                obj.BondedInventory = objDTO.BondedInventory;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.AddedFrom = objDTO.AddedFrom;
                objDTO.WhatWhereAction = "BOMItem";
                obj.WhatWhereAction = objDTO.WhatWhereAction;
                obj.IsBOMItem = true;
                obj.ReceivedOn = DateTime.UtcNow;
                obj.ReceivedOnWeb = DateTime.UtcNow;
                obj.ImageType = objDTO.ImageType;
                obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;
                obj.IsActive = objDTO.IsActive;
                obj.EnhancedDescription = objDTO.EnhancedDescription;
                obj.EnrichedProductData = objDTO.EnrichedProductData;
                context.ItemMasters.Add(obj);


                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                //string strUpdateOnHand = "EXEC [dbo].[AutoCartEntryonInventoryUpDown] '" + obj.GUID.ToString() + "', " + obj.CreatedBy;
                //context.Database.ExecuteSqlCommand(strUpdateOnHand);

                //Fill to cache
                objDTO = FillWithExtraDetail(objDTO);
                //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.CreatedBy ?? 0);
                //objDTO.SuggestedOrderQuantity = GetSuggestedOrderQty(obj.GUID);
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    //IEnumerable<BOMItemDTO> ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.GetCacheItem("Cached_BOMItemMaster_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<BOMItemDTO> tempC = new List<BOMItemDTO>();
                    //    tempC.Add(objDTO);

                    //    ObjCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<BOMItemDTO>>.AppendToCacheItem("Cached_BOMItemMaster_" + objDTO.CompanyID.ToString(), ObjCache);
                    //}
                }

                return obj.ID;
            }

        }

        /// <summary>
        /// Method for fast loading of items...
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public BOMItemDTO FillWithExtraDetail(BOMItemDTO objDTO)
        {
            ManufacturerMasterDAL objItemManDAL = new ManufacturerMasterDAL(base.DataBaseName);
            ManufacturerMasterDTO ObjItemManCache = null;

            if (objDTO.ManufacturerID > 0)
            {
                ObjItemManCache = objItemManDAL.GetManufacturerByIDNormal(objDTO.ManufacturerID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), true);
            }

            SupplierMasterDAL objItemSupDAL = new SupplierMasterDAL(base.DataBaseName);
            SupplierMasterDTO ObjItemSuppTemp = null;

            if (objDTO.SupplierID > 0)
            {
                ObjItemSuppTemp = objItemSupDAL.GetSupplierByIDPlain(objDTO.SupplierID.GetValueOrDefault(0));
            }

            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);

            UserMasterDTO ObjUserCreatedCache = objUserDAL.GetUserByIdPlain(objDTO.CreatedBy.GetValueOrDefault(0));
            UserMasterDTO ObjUserUpdatedCache = objUserDAL.GetUserByIdPlain(objDTO.LastUpdatedBy.GetValueOrDefault(0));

            UnitMasterDAL objUnitDAL = new UnitMasterDAL(base.DataBaseName);
            UnitMasterDTO objUnitTemp = null;
            if (objDTO.UOMID > 0)
            {
                objUnitTemp = objUnitDAL.GetUnitByIDPlain(objDTO.UOMID);
            }

            CategoryMasterDAL objCategoryDAL = new CategoryMasterDAL(base.DataBaseName);
            CategoryMasterDTO objCategoryCache = null;
            if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
            {
                objCategoryCache = objCategoryDAL.GetCategoryByCatID(objDTO.CategoryID.GetValueOrDefault(0));
            }
            GLAccountMasterDAL objGLAccountDAL = new GLAccountMasterDAL(base.DataBaseName);
            GLAccountMasterDTO objGLAccountCaChe = null;
            if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
            {
                objGLAccountCaChe = objGLAccountDAL.GetGLAccountByID(objDTO.GLAccountID ?? 0);
            }
            InventoryClassificationMasterDAL objInventoryClassificationMasterDAL = new InventoryClassificationMasterDAL(base.DataBaseName);
            InventoryClassificationMasterDTO objInventoryClassificationMasterDTOTemp = null;
            if (objDTO.InventoryClassification.GetValueOrDefault(0) > 0)
            {
                objInventoryClassificationMasterDTOTemp = objInventoryClassificationMasterDAL.GetInventoryClassificationByIDPlain((Int64)objDTO.InventoryClassification.GetValueOrDefault(0));
            }
            if (ObjItemManCache != null)
                objDTO.ManufacturerName = ObjItemManCache.Manufacturer;

            if (ObjItemSuppTemp != null)
                objDTO.SupplierName = ObjItemSuppTemp.SupplierName;

            if (ObjUserCreatedCache != null)
                objDTO.CreatedByName = ObjUserCreatedCache.UserName;

            if (ObjUserUpdatedCache != null)
                objDTO.UpdatedByName = ObjUserUpdatedCache.UserName;

            if (objUnitTemp != null && objUnitTemp.ID > 0)
                objDTO.Unit = objUnitTemp.Unit;

            if (objCategoryCache != null)
                objDTO.CategoryName = objCategoryCache.Category;

            if (objGLAccountCaChe != null)
                objDTO.GLAccount = objGLAccountCaChe.GLAccount;

            if (objInventoryClassificationMasterDTOTemp != null)
                objDTO.InventoryClassificationName = objInventoryClassificationMasterDTOTemp.InventoryClassification;

            return objDTO;
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(BOMItemDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;



            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ItemMaster obj = context.ItemMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.ID = objDTO.ID;
                obj.ItemNumber = objDTO.ItemNumber;

                if (objDTO.ManufacturerID > 0)
                    obj.ManufacturerID = objDTO.ManufacturerID;

                obj.ManufacturerNumber = objDTO.ManufacturerNumber;

                if (objDTO.SupplierID > 0)
                    obj.SupplierID = objDTO.SupplierID;

                obj.SupplierPartNo = objDTO.SupplierPartNo;
                obj.UPC = objDTO.UPC;
                obj.UNSPSC = objDTO.UNSPSC;
                obj.Description = objDTO.Description;
                obj.LongDescription = objDTO.LongDescription;

                if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
                    obj.CategoryID = objDTO.CategoryID;

                if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
                    obj.GLAccountID = objDTO.GLAccountID;

                if (objDTO.UOMID > 0)
                    obj.UOMID = objDTO.UOMID;

                obj.PricePerTerm = objDTO.PricePerTerm;
                obj.CostUOMID = objDTO.CostUOMID;
                obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                obj.Cost = objDTO.Cost;
                obj.Markup = objDTO.Markup;
                obj.SellPrice = objDTO.SellPrice;
                //obj.ExtendedCost = objDTO.ExtendedCost;
                obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                obj.Link1 = objDTO.Link1;
                if (!string.IsNullOrEmpty(objDTO.Link2))
                {
                    obj.Link2 = objDTO.Link2;
                }
                obj.Trend = objDTO.Trend;
                obj.Taxable = objDTO.Taxable;
                obj.Consignment = objDTO.Consignment;
                obj.StagedQuantity = objDTO.StagedQuantity;
                obj.InTransitquantity = objDTO.InTransitquantity;
                obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                obj.OnReturnQuantity = objDTO.OnReturnQuantity;
                obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                obj.PackingQuantity = objDTO.PackingQuantity;
                obj.AverageUsage = objDTO.AverageUsage;
                obj.Turns = objDTO.Turns;
                obj.OnHandQuantity = objDTO.OnHandQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.WeightPerPiece = objDTO.WeightPerPiece;
                //obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                obj.IsPurchase = objDTO.IsPurchase;
                obj.IsTransfer = objDTO.IsTransfer;
                obj.DefaultLocation = objDTO.DefaultLocation;
                obj.InventoryClassification = objDTO.InventoryClassification;
                obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                obj.LotNumberTracking = objDTO.LotNumberTracking;
                obj.DateCodeTracking = objDTO.DateCodeTracking;
                if (!string.IsNullOrWhiteSpace(objDTO.ImageType))
                {
                    obj.ImageType = objDTO.ImageType;
                }
                if (!string.IsNullOrEmpty(objDTO.ImagePath))
                {
                    obj.ImagePath = objDTO.ImagePath;
                }
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = objDTO.GUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                //obj.IsDeleted = false;
                //obj.IsArchived = false;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                // Get ext cost based on Item Location details START
                //CostDTO ObjCostDTO = GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                //obj.ExtendedCost = ObjCostDTO.ExtCost;
                //obj.AverageCost = ObjCostDTO.AvgCost;
                //objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                //objDTO.AverageCost = ObjCostDTO.AvgCost;
                // Get ext cost based on Item Location details END
                obj.BondedInventory = objDTO.BondedInventory;
                obj.ReceivedOn = DateTime.UtcNow;
                obj.ReceivedOnWeb = DateTime.UtcNow;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.AddedFrom = objDTO.AddedFrom;
                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "BOMItem";

                obj.WhatWhereAction = objDTO.WhatWhereAction;
                obj.IsBOMItem = true;

                if (!string.IsNullOrWhiteSpace(objDTO.ItemLink2ImageType))
                {
                    obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                }
                obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                if (objDTO.ItemType != 0)
                {
                    obj.ItemType = objDTO.ItemType;
                }
                obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;
                obj.IsActive = objDTO.IsActive;
                obj.EnhancedDescription = objDTO.EnhancedDescription;
                obj.EnrichedProductData = objDTO.EnrichedProductData;
                obj.LongDescription = objDTO.LongDescription;
                //context.ItemMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                objDTO = FillWithExtraDetail(objDTO);

                //if (objDTO.OnHandQuantity.GetValueOrDefault(0) <= 0)
                //{
                //    try
                //    {
                //        SendMailWhenItemStockOut(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0), objDTO.OnHandQuantity.GetValueOrDefault(0), objDTO.ItemNumber, objDTO.GUID);
                //    }
                //    catch (Exception ex)
                //    {
                //        //Log the email exception

                //    }
                //}
                //else
                //{
                //    RemoveItemStockOutMailLog(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0));
                //}
            }

            //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.CreatedBy ?? 0);
            //objDTO.SuggestedOrderQuantity = GetSuggestedOrderQty(objDTO.GUID);

            //IEnumerable<BOMItemDTO> ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.GetCacheItem("Cached_BOMItemMaster_" + objDTO.CompanyID.ToString());
            //if (ObjCache != null)
            //{
            //    List<BOMItemDTO> objTemp = ObjCache.ToList();
            //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
            //    ObjCache = objTemp.AsEnumerable();

            //    List<BOMItemDTO> tempC = new List<BOMItemDTO>();
            //    tempC.Add(objDTO);
            //    IEnumerable<BOMItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable()).AsEnumerable();
            //    CacheHelper<IEnumerable<BOMItemDTO>>.AddCacheItem("Cached_BOMItemMaster_" + objDTO.CompanyID.ToString(), NewCache);
            //}
            return true;
        }

        /// <summary>
        /// Get Paged Records from the ItemMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>

        public bool UpdateSupplierDetails(Int64 CompanyID, Int64 RoomID, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strUpdateOnHand = "EXEC [dbo].[UpdateItemSupplierRecordsForIsDefualt] " + CompanyID + ", " + RoomID + ", " + UserID;
                    strUpdateOnHand += "   EXEC [dbo].[UpdateItemManufacturerRecordsForIsDefualt] " + CompanyID + ", " + RoomID + ", " + UserID;
                    context.Database.ExecuteSqlCommand(strUpdateOnHand);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }
        }

        public IEnumerable<ItemManufacturerDetailsDTO> GetBOMManufacturer(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemManufacturerDetailsDTO>("exec [GetBOMManufacturer] @CompanyID", params1).ToList();
            }
        }
        public IEnumerable<ItemManufacturerDetailsDTO> GetBOMSupplier(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemManufacturerDetailsDTO>("exec [GetBOMSupplierCompanywise] @CompanyID", params1).ToList();
            }
        }
        public IEnumerable<ItemManufacturerDetailsDTO> GetBOMManufacturerUsingItemGuid(Int64 RoomID, Int64 CompanyID, Guid ItemGuid)
        {
            //Get Cached-Media

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGuid", ItemGuid) };
                return context.Database.SqlQuery<ItemManufacturerDetailsDTO>("exec [GetBOMManufacturerItemWise] @CompanyID,@ItemGuid", params1).ToList();



            }

        }
        public IEnumerable<ItemSupplierDetailsDTO> GetBOMSupplierUsingItemGuid(Int64 RoomID, Int64 CompanyID, Guid ItemGuid)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGuid", ItemGuid) };
                return context.Database.SqlQuery<ItemSupplierDetailsDTO>("exec [GetBOMSupplierItemwise] @CompanyID,@ItemGuid", params1).ToList();
            }
        }
        
        public IEnumerable<ItemManufacturerDetailsDTO> GetAllRecordsManuBOMForItem(Int64 RoomID, Int64 CompanyId, Guid ItemGuid)
        {
            return GetBOMManufacturerUsingItemGuid(RoomID, CompanyId, ItemGuid).OrderBy("ID DESC");
        }
        public IEnumerable<ItemSupplierDetailsDTO> GetAllRecordsSupplierBOMForItem(Int64 RoomID, Int64 CompanyId, Guid ItemGuid)
        {
            return GetBOMSupplierUsingItemGuid(RoomID, CompanyId, ItemGuid).OrderBy("ID DESC");
        }

        public List<BOMItemDTO> GetAllBOMItems(long CompanyId)
        {
            List<BOMItemDTO> lstBOMItems = new List<BOMItemDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {


                lstBOMItems = (from im in context.ItemMasters
                               join isd in context.ItemSupplierDetails on new { supid = im.SupplierID ?? 0, iid = im.GUID } equals new { supid = isd.SupplierID, iid = isd.ItemGUID ?? Guid.Empty } into im_isd_join
                               from im_isd in im_isd_join.DefaultIfEmpty()

                               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                               from im_sm in im_sm_join.DefaultIfEmpty()

                               join imd in context.ItemManufacturerDetails on new { Manufactureid = im.ManufacturerID ?? 0, iid = im.GUID } equals new { Manufactureid = imd.ManufacturerID, iid = imd.ItemGUID ?? Guid.Empty } into im_imd_join
                               from im_imd in im_imd_join.DefaultIfEmpty()

                               join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
                               from im_mm in im_mm_join.DefaultIfEmpty()

                               join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
                               from im_UMC in im_UMC_join.DefaultIfEmpty()

                               join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
                               from im_UMU in im_UMU_join.DefaultIfEmpty()

                               join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
                               from im_CM in im_CM_join.DefaultIfEmpty()

                               join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
                               from im_UNM in im_UNM_join.DefaultIfEmpty()

                               join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
                               from im_gla in im_gla_join.DefaultIfEmpty()

                               join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                               from im_icm in im_icm_join.DefaultIfEmpty()

                               join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
                               from im_bm in im_bm_join.DefaultIfEmpty()

                               join cuom in context.CostUOMMasters on im.CostUOMID equals cuom.ID into im_Cuom_join
                               from im_Cuom in im_Cuom_join.DefaultIfEmpty()

                               where im.CompanyID == CompanyId && im.Room == null && im.IsDeleted == false && im.IsArchived == false
                               select new BOMItemDTO
                               {
                                   ID = im.ID,
                                   ItemNumber = im.ItemNumber,
                                   ManufacturerID = im.ManufacturerID,
                                   ManufacturerNumber = im.ManufacturerNumber,
                                   ManufacturerName = im_mm.Manufacturer,
                                   SupplierID = im.SupplierID,
                                   SupplierPartNo = im.SupplierPartNo,
                                   SupplierName = im_sm.SupplierName,
                                   UPC = im.UPC,
                                   UNSPSC = im.UNSPSC,
                                   Description = im.Description,
                                   CategoryID = im.CategoryID,
                                   GLAccountID = im.GLAccountID,
                                   UOMID = im.UOMID ?? 0,
                                   LeadTimeInDays = im.LeadTimeInDays,
                                   Taxable = im.Taxable,
                                   Consignment = im.Consignment,
                                   ItemUniqueNumber = im.ItemUniqueNumber,
                                   IsPurchase = im.IsPurchase ?? false,
                                   IsTransfer = im.IsTransfer ?? false,
                                   DefaultLocationName = im_bm.BinNumber,
                                   InventoryClassification = im.InventoryClassification,
                                   SerialNumberTracking = im.SerialNumberTracking,
                                   LotNumberTracking = im.LotNumberTracking,
                                   DateCodeTracking = im.DateCodeTracking,
                                   ItemType = im.ItemType,
                                   UDF1 = im.UDF1,
                                   UDF2 = im.UDF2,
                                   UDF3 = im.UDF3,
                                   UDF4 = im.UDF4,
                                   UDF5 = im.UDF5,
                                   GUID = im.GUID,
                                   Created = im.Created,
                                   Updated = im.Updated,
                                   CreatedBy = im.CreatedBy,
                                   LastUpdatedBy = im.LastUpdatedBy,
                                   IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                                   IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                                   CompanyID = im.CompanyID,
                                   Room = im.Room,
                                   CreatedByName = im_UMC.UserName,
                                   UpdatedByName = im_UMU.UserName,

                                   IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
                                   ItemTypeName = (im.ItemType == 1 ? "Item" : ""),
                                   CategoryName = im_CM.Category,
                                   Unit = im_UNM.Unit,
                                   GLAccount = im_gla.GLAccount,
                                   InventoryClassificationName = im_icm.InventoryClassification,
                                   AddedFrom = im.AddedFrom,
                                   EditedFrom = im.EditedFrom,
                                   CriticalQuantity = im.CriticalQuantity,
                                   MinimumQuantity = im.MinimumQuantity,
                                   MaximumQuantity = im.MaximumQuantity,
                                   Cost = im.Cost,
                                   Markup = im.Markup,
                                   SellPrice = im.SellPrice,
                                   CostUOMID = im.CostUOMID,
                                   CostUOMName = im_Cuom.CostUOM,
                                   DefaultReorderQuantity = im.DefaultReorderQuantity,
                                   DefaultPullQuantity = im.DefaultPullQuantity,
                                   Link1 = im.Link1,
                                   Link2 = im.Link2,
                                   IsItemLevelMinMaxQtyRequired = im.IsItemLevelMinMaxQtyRequired,
                                   PullQtyScanOverride = im.PullQtyScanOverride,
                                   IsEnforceDefaultReorderQuantity = im.IsEnforceDefaultReorderQuantity,
                                   ItemImageExternalURL = im.ItemImageExternalURL,
                                   ItemLink2ExternalURL = im.ItemLink2ExternalURL,
                                   IsActive = im.IsActive,
                                   WeightPerPiece = im.WeightPerPiece,
                                   ItemDocExternalURL = im.ItemDocExternalURL,

                               }).ToList();
                return lstBOMItems;
            }

        }
        public List<bomUpdateResp> UpdateReferenceBOMItem(long Id, long UserId)
        {
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            //SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            //SqlHelper.ExecuteNonQuery(EturnsConnection, "USP_UpdateRefeenceBOMItem", Id, UserId);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@BOMItemId", Id), new SqlParameter("@UserId", UserId) };
                return context.Database.SqlQuery<bomUpdateResp>("exec [USP_UpdateRefeenceBOMItem] @BOMItemId,@UserId", params1).ToList();

            }


        }
        public void UpdateBOMMasterReference(long Id, string TableName, long UserId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "USP_UpdateBOMMaster", Id, TableName);
        }
        public List<BOMItemDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 RoomId, string CallWhere, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            DataSet dsRooms = new DataSet();
            List<BOMItemDTO> lstItemMasterDTO = new List<BOMItemDTO>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            TotalCount = 0;
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            string Manufactures = null;
            string Suppliers = null;
            string Category = null;
            string ItemTypes = null;
            string BomCreaters = null;
            string BomUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            if (sortColumnName != null && (!string.IsNullOrEmpty(sortColumnName)))
            {
                sortColumnName = sortColumnName.Replace("ManufacturerName", "ManufacturerNumber");
                sortColumnName = sortColumnName.Replace("CategoryName", "Category");
            }
            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsRooms = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedBOMItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, Manufactures, Suppliers, Category, ItemTypes, BomCreaters, BomUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId, CallWhere);
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
                if (CallWhere == "page")
                {
                    if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                    {
                        Suppliers = FieldsPara[9].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                    {
                        Manufactures = FieldsPara[10].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                    {
                        Category = FieldsPara[11].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                    {
                        ItemTypes = FieldsPara[22].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                    {
                        BomUpdators = FieldsPara[1].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {
                        BomCreaters = FieldsPara[0].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                    {
                        //  CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        //                        CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                        CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                    {
                        // UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        // UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                        UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                    {
                        Suppliers = FieldsPara[3].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                    {
                        Manufactures = FieldsPara[4].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                    {
                        Category = FieldsPara[5].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                    {
                        ItemTypes = FieldsPara[6].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                    {
                        BomUpdators = FieldsPara[7].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                    {
                        BomCreaters = FieldsPara[8].TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                    {
                        //CreatedDateFrom = Convert.ToDateTime(FieldsPara[9].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        // CreatedDateTo = Convert.ToDateTime(FieldsPara[9].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                        CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[9].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[9].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                    {
                        //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[10].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        //UpdatedDateTo = Convert.ToDateTime(FieldsPara[10].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                        UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[10].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[10].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                }

                if(CallWhere != "popup" && CallWhere != "Kit")
                {
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
                }
                
                dsRooms = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedBOMItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, Manufactures, Suppliers, Category, ItemTypes, BomCreaters, BomUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId, CallWhere);
            }
            else
            {
                dsRooms = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedBOMItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, Manufactures, Suppliers, Category, ItemTypes, BomCreaters, BomUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyID, RoomId, CallWhere);
            }
            if (dsRooms != null && dsRooms.Tables.Count > 0)
            {
                DataTable dtRooms = dsRooms.Tables[0];

                if (dtRooms.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtRooms.Rows[0]["TotalRecords"]);
                }
                lstItemMasterDTO = (from im in dtRooms.AsEnumerable()
                                    select new BOMItemDTO
                                    {
                                        ID = im.Field<long>("ID"),
                                        ItemNumber = im.Field<string>("ItemNumber"),
                                        ManufacturerID = im.Field<long?>("ManufacturerID"),
                                        ManufacturerNumber = im.Field<string>("ManufacturerNumber"),
                                        ManufacturerName = im.Field<string>("Manufacturer"),
                                        SupplierID = im.Field<long?>("SupplierID"),
                                        SupplierPartNo = im.Field<string>("SupplierPartNo"),
                                        SupplierName = im.Field<string>("SupplierName"),
                                        UPC = im.Field<string>("UPC"),
                                        UNSPSC = im.Field<string>("UNSPSC"),
                                        Description = im.Field<string>("Description"),
                                        CategoryID = im.Field<long?>("CategoryID"),
                                        GLAccountID = im.Field<long?>("GLAccountID"),
                                        UOMID = (im.Field<long?>("UOMID")) ?? 0,
                                        LeadTimeInDays = im.Field<int?>("LeadTimeInDays"),
                                        Taxable = im.Field<bool>("Taxable"),
                                        Consignment = im.Field<bool>("Consignment"),
                                        ItemUniqueNumber = im.Field<string>("ItemUniqueNumber"),
                                        IsPurchase = im.Field<bool>("IsPurchase"),
                                        IsTransfer = im.Field<bool>("IsTransfer"),
                                        InventoryClassification = im.Field<int?>("InventoryClassificationId"),
                                        SerialNumberTracking = im.Field<bool>("SerialNumberTracking"),
                                        LotNumberTracking = im.Field<bool>("LotNumberTracking"),
                                        DateCodeTracking = im.Field<bool>("DateCodeTracking"),
                                        ItemType = im.Field<int>("ItemType"),
                                        UDF1 = im.Field<string>("UDF1"),
                                        UDF2 = im.Field<string>("UDF2"),
                                        UDF3 = im.Field<string>("UDF3"),
                                        UDF4 = im.Field<string>("UDF4"),
                                        UDF5 = im.Field<string>("UDF5"),
                                        GUID = im.Field<Guid>("GUID"),
                                        Created = im.Field<DateTime?>("Created"),
                                        Updated = im.Field<DateTime?>("Updated"),
                                        CreatedBy = im.Field<long?>("CreatedBy"),
                                        LastUpdatedBy = im.Field<long?>("LastUpdatedBy"),
                                        IsDeleted = im.Field<bool?>("IsDeleted"),
                                        IsArchived = im.Field<bool?>("IsArchived"),
                                        CompanyID = im.Field<long?>("CompanyID"),

                                        CreatedByName = im.Field<string>("CreatedByName"),
                                        UpdatedByName = im.Field<string>("UpdatedByName"),

                                        IsLotSerialExpiryCost = im.Field<string>("IsLotSerialExpiryCost"),
                                        ItemTypeName = (im.Field<int>("ItemType") == 1 ? "Item" : im.Field<int>("ItemType") == 3 ? "Kit": ""),
                                        CategoryName = im.Field<string>("Category"),
                                        Unit = im.Field<string>("Unit"),
                                        GLAccount = im.Field<string>("GLAccount"),
                                        InventoryClassificationName = im.Field<string>("InventoryClassificationName"),
                                        ItemLink2ImageType = im.Field<string>("ItemLink2ImageType"),
                                        ImageType = im.Field<string>("ImageType"),
                                        ImagePath = im.Field<string>("ImagePath"),
                                        ItemLink2ExternalURL = im.Field<string>("ItemLink2ExternalURL"),
                                        ItemImageExternalURL = im.Field<string>("ItemImageExternalURL"),
                                        ItemDocExternalURL = im.Field<string>("ItemDocExternalURL"),
                                        IsActive = im.Field<bool>("IsActive"),
                                        SellPrice = im.Field<double?>("SellPrice"),
                                        LongDescription = im.Field<string>("LongDescription"),
                                        EnhancedDescription = im.Field<string>("EnhancedDescription"),
                                        EnrichedProductData = im.Field<string>("EnrichedProductData"),
                                    }).ToList();
            }

            //TotalCount = lstItemMasterDTO.Count();
            return lstItemMasterDTO;


        }
        public string DeleteRecordsItem(string TableName, string IDs, Int64 CompanyId, long UserId)
        {
            try
            {
                if (!(CompanyId > 0))
                {
                    return "";
                }

                string strError = "";

                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {

                            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

                            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                            strError = Convert.ToString(SqlHelper.ExecuteScalar(EturnsConnection, "USP_DeleteBOMItem", item, UserId));

                        }
                    }
                }

                return strError;
            }
            catch (EntitySqlException ex)
            {
                return ex.Message;
            }
        }
        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecords(string TextFieldName, List<long> SupplierIds, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@SupplierIds", strSupplierIds) };
                return context.Database.SqlQuery<RequisitionMasterNarrowSearchDTO>("exec [GetBomItemNarrowSearch] @TextFieldName,@CompanyId,@Isdeleted,@IsArchived,@SupplierIds", params1).ToList();
            }
        }

        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecordsForUDF(string TextFieldName, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@Isdeleted", IsDeleted),
                                                    new SqlParameter("@IsArchived", IsArchived) };

                return context.Database.SqlQuery<RequisitionMasterNarrowSearchDTO>("exec [GetBOMItemNarrowSearchUDF] @TextFieldName,@CompanyId,@Isdeleted,@IsArchived", params1).ToList();
            }
        }

        public IEnumerable<BOMItemDTO> GetBOMPagedRecordsNew_ChnageLog(Guid ItemGuid, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds)
        {
            List<BOMItemDTO> lstItems = new List<BOMItemDTO>();
            TotalCount = 0;
            BOMItemDTO objItemDTO = new BOMItemDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetBOMPagedInventoryItems_ChangeLog", ItemGuid, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                    //  Cost = FieldsPara[15].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        AvgUsageTo = (Fields[1].Split('@')[48].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[49].Contains("10_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        turnsTo = (Fields[1].Split('@')[49].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetBOMPagedInventoryItems_ChangeLog", ItemGuid, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetBOMPagedInventoryItems_ChangeLog", ItemGuid, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }
            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new BOMItemDTO
                    {
                        HistoryID = row.Field<long>("HistoryID"),
                        ID = row.Field<long>("ID"),
                        Action = row.Field<string>("Action"),
                        WhatWhereAction = row.Field<string>("WhatWhereAction"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long>("UOMID"),
                        PricePerTerm = (row.Field<double?>("PricePerTerm").HasValue ? row.Field<double?>("PricePerTerm") : 0),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        //Cost = row.Field<double?>("Cost"),
                        //Markup = row.Field<double?>("Markup"),
                        //SellPrice = row.Field<double?>("SellPrice"),
                        //ExtendedCost = row.Field<double?>("ExtendedCost"),
                        //AverageCost = row.Field<double?>("AverageCost"),
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                        ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                        Cost = (row.Field<double?>("Cost").HasValue ? row.Field<double?>("Cost") : 0),
                        Markup = (row.Field<double?>("Markup").HasValue ? row.Field<double?>("Markup") : 0),
                        SellPrice = (row.Field<double?>("SellPrice").HasValue ? row.Field<double?>("SellPrice") : 0),
                        ExtendedCost = (row.Field<double?>("ExtendedCost").HasValue ? row.Field<double?>("ExtendedCost") : 0),
                        AverageCost = (row.Field<double?>("AverageCost").HasValue ? row.Field<double?>("AverageCost") : 0),
                        PerItemCost = (row.Field<double?>("PerItemCost").HasValue ? row.Field<double?>("PerItemCost") : 0),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Link2 = row.Field<string>("Link2"),
                        Trend = (row.Field<bool?>("Trend").HasValue ? row.Field<bool>("Trend") : false),// row.Field<bool>("Trend"),
                        Taxable = (row.Field<bool?>("Taxable").HasValue ? row.Field<bool>("Taxable") : false), //row.Field<bool>("Taxable"),
                        Consignment = (row.Field<bool?>("Consignment").HasValue ? row.Field<bool>("Consignment") : false),//   row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        // InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),

                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = (row.Field<bool?>("IsPurchase").HasValue ? row.Field<bool>("IsPurchase") : false), //row.Field<bool>("IsPurchase"),
                        IsTransfer = (row.Field<bool?>("IsTransfer").HasValue ? row.Field<bool>("IsTransfer") : false), //row.Field<bool>("IsTransfer"),
                        DefaultLocation = row.Field<long>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = (row.Field<bool?>("SerialNumberTracking").HasValue ? row.Field<bool>("SerialNumberTracking") : false),
                        LotNumberTracking = (row.Field<bool?>("LotNumberTracking").HasValue ? row.Field<bool>("LotNumberTracking") : false),
                        DateCodeTracking = (row.Field<bool?>("DateCodeTracking").HasValue ? row.Field<bool>("DateCodeTracking") : false),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),

                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = (row.Field<bool?>("IsBuildBreak").HasValue ? row.Field<bool>("IsBuildBreak") : false),
                        // row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = (row.Field<bool?>("IsBOMItem").HasValue ? row.Field<bool>("IsBOMItem") : false),
                        //row.Field<bool>("IsBOMItem"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = (row.Field<bool?>("PullQtyScanOverride").HasValue ? row.Field<bool>("PullQtyScanOverride") : false),
                        // row.Field<bool>("PullQtyScanOverride"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        //ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        //ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        IsActive = row.Field<bool>("IsActive"),
                        HistoryOn = row.Field<DateTime?>("HistoryDate").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("HistoryDate")) : row.Field<DateTime?>("HistoryDate"),
                    }).ToList();
                }
            }
            return lstItems;
        }


        #region "UnUsed Code"

        public void SaveItemStockOutMailLog(ItemStockOutMailLogDTO objItemStockOutMailLogDTo)
        {
            ItemStockOutMailLog objItemStockOutMailLog = new ItemStockOutMailLog();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objItemStockOutMailLog = (from u in context.ItemStockOutMailLogs
                                          where u.ItemId == objItemStockOutMailLogDTo.ItemId && u.RoomId == objItemStockOutMailLogDTo.RoomId && u.CompanyId == objItemStockOutMailLogDTo.CompanyId
                                          select u
                                           ).FirstOrDefault();
                if (objItemStockOutMailLog == null)
                {
                    objItemStockOutMailLog = new ItemStockOutMailLog();
                    objItemStockOutMailLog.Id = 0;
                    objItemStockOutMailLog.ItemId = objItemStockOutMailLogDTo.ItemId;
                    objItemStockOutMailLog.RoomId = objItemStockOutMailLogDTo.RoomId;
                    objItemStockOutMailLog.CompanyId = objItemStockOutMailLogDTo.CompanyId;
                    objItemStockOutMailLog.OnHandQuantity = objItemStockOutMailLogDTo.OnHandQuantity ?? 0;
                    objItemStockOutMailLog.ItemNumber = objItemStockOutMailLogDTo.ItemNumber;
                    objItemStockOutMailLog.ItemGUID = objItemStockOutMailLogDTo.ItemGUID;
                    context.ItemStockOutMailLogs.Add(objItemStockOutMailLog);
                    context.SaveChanges();
                }
            }

        }
        public Guid? GetGuidByItemNumber(string ItemNumber, Int64 RoomID, Int64 CompanyID)
        {
            Guid? RetGUID = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = new ItemMaster();
                obj = (from u in context.ItemMasters
                       where u.ItemNumber == ItemNumber && u.Room == RoomID && u.CompanyID == CompanyID && u.IsDeleted == false && u.IsArchived == false
                       select u).FirstOrDefault();
                if (obj != null)
                {
                    RetGUID = obj.GUID;
                }
            }
            return RetGUID;
        }

        public void RemoveItemStockOutMailLog(Int64 ItemID, Int64 CompanyId, Int64 RoomID)
        {
            ItemStockOutMailLog objItemStockOutMailLog = new ItemStockOutMailLog();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objItemStockOutMailLog = (from u in context.ItemStockOutMailLogs
                                          where u.ItemId == ItemID && u.RoomId == RoomID && u.CompanyId == CompanyId
                                          select u
                                           ).FirstOrDefault();
                if (objItemStockOutMailLog != null)
                {
                    context.ItemStockOutMailLogs.Remove(objItemStockOutMailLog);
                    context.SaveChanges();
                }
            }

        }
        public bool EditMultiple(BOMItemDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = new ItemMaster();
                obj.ID = objDTO.ID;
                obj.ItemNumber = objDTO.ItemNumber;
                obj.ManufacturerID = objDTO.ManufacturerID;
                obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                obj.SupplierID = objDTO.SupplierID;
                obj.SupplierPartNo = objDTO.SupplierPartNo;
                obj.UPC = objDTO.UPC;
                obj.UNSPSC = objDTO.UNSPSC;
                obj.Description = objDTO.Description;
                obj.LongDescription = objDTO.LongDescription;
                obj.CategoryID = objDTO.CategoryID;
                obj.GLAccountID = objDTO.GLAccountID;
                obj.UOMID = objDTO.UOMID;
                obj.PricePerTerm = objDTO.PricePerTerm;
                obj.CostUOMID = objDTO.CostUOMID;
                obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                obj.Cost = objDTO.Cost;
                obj.Markup = objDTO.Markup;
                obj.SellPrice = objDTO.SellPrice;
                //obj.ExtendedCost = objDTO.ExtendedCost;
                obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                obj.Link1 = objDTO.Link1;
                if (!string.IsNullOrEmpty(objDTO.Link2))
                {
                    obj.Link2 = objDTO.Link2;
                }
                obj.Trend = objDTO.Trend;
                obj.Taxable = objDTO.Taxable;
                obj.Consignment = objDTO.Consignment;
                obj.StagedQuantity = objDTO.StagedQuantity;
                obj.InTransitquantity = objDTO.InTransitquantity;
                obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                obj.OnReturnQuantity = objDTO.OnReturnQuantity;
                obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                obj.PackingQuantity = objDTO.PackingQuantity;
                obj.AverageUsage = objDTO.AverageUsage;
                obj.Turns = objDTO.Turns;
                obj.OnHandQuantity = objDTO.OnHandQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity;
                obj.MinimumQuantity = objDTO.MinimumQuantity;
                obj.MaximumQuantity = objDTO.MaximumQuantity;
                obj.WeightPerPiece = objDTO.WeightPerPiece;
                //obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                obj.IsPurchase = objDTO.IsPurchase;
                obj.IsTransfer = objDTO.IsTransfer;
                obj.DefaultLocation = objDTO.DefaultLocation;
                obj.InventoryClassification = objDTO.InventoryClassification;
                obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                obj.LotNumberTracking = objDTO.LotNumberTracking;
                obj.DateCodeTracking = objDTO.DateCodeTracking;
                obj.ItemType = objDTO.ItemType;
                //obj.ImagePath = objDTO.ImagePath;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = objDTO.GUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                //obj.IsDeleted = false;
                //obj.IsArchived = false;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                // Get ext cost based on Item Location details START
                //CostDTO ObjCostDTO = GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                //obj.ExtendedCost = ObjCostDTO.ExtCost;
                //obj.AverageCost = ObjCostDTO.AvgCost;
                //objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                //objDTO.AverageCost = ObjCostDTO.AvgCost;
                // Get ext cost based on Item Location details END
                obj.BondedInventory = objDTO.BondedInventory;
                if (!string.IsNullOrWhiteSpace(objDTO.ImageType))
                {
                    obj.ImageType = objDTO.ImageType;
                }
                if (!string.IsNullOrWhiteSpace(objDTO.ItemLink2ImageType))
                {
                    obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                }
                obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;
                obj.IsActive = objDTO.IsActive;
                if (!string.IsNullOrEmpty(objDTO.ImagePath))
                {
                    obj.ImagePath = objDTO.ImagePath;
                }
                context.ItemMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                //Fill to cache
                objDTO = FillWithExtraDetail(objDTO);

                //Get Cached-Media
                //IEnumerable<BOMItemDTO> ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.GetCacheItem("Cached_ItemMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<BOMItemDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<BOMItemDTO> tempC = new List<BOMItemDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<BOMItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable()).AsEnumerable();
                //    CacheHelper<IEnumerable<BOMItemDTO>>.AddCacheItem("Cached_ItemMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //}


                return true;
            }
        }

        #endregion

        public List<AutoSotForImport_DTO> GetPendingAutoSotForImport()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<AutoSotForImport_DTO>("EXEC [dbo].[GetPendingAutoSotForImport]").ToList();
            }
        }
        public int UpdateStatusForAutoSotForImport(Guid NewItemGUID, bool IsStarted, bool IsCompleted)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@NewItemGUID", NewItemGUID),
                                                new SqlParameter("@IsStarted", IsStarted),
                                                new SqlParameter("@IsCompleted", IsCompleted)
                                              };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("EXEC [dbo].[UpdateStatusForAutoSotForImport] @NewItemGUID, @IsStarted, @IsCompleted", params1)
                    .FirstOrDefault();
            }
        }
        public void InsertFailDataForAutoSotForImport(Guid NewItemGUID, string Error)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@NewItemGUID", NewItemGUID),
                                               new SqlParameter("@Error", Error),};

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [dbo].[InsertFailDataForAutoSotForImport] @NewItemGUID, @Error", params1);
            }
        }
    }
}


