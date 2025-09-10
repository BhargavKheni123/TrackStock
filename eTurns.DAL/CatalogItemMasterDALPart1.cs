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
using eTurns.DAL;
using System.Configuration;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class CatalogItemMasterDAL : eTurnsBaseDAL
    {
        public List<SupplierCatalogItemDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<SupplierCatalogItemDTO> lstSuppliers = GetAllItems();
            if (String.IsNullOrWhiteSpace(SearchTerm))
            {
                //Get Cached-Media
                TotalCount = lstSuppliers.Count();
                return lstSuppliers.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                lstSuppliers = lstSuppliers.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.SupplierName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.ManufacturerName))));
                TotalCount = lstSuppliers.Count();
                return lstSuppliers.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }
            else
            {
                lstSuppliers = lstSuppliers.Where(
                    t => t.SupplierCatalogItemID.ToString().Contains(SearchTerm) ||
                    t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.Description.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    Convert.ToString(t.SellPrice).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    Convert.ToString(t.PackingQantity).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.UPC.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.SupplierName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.SupplierPartNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.ManufacturerPartNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.ManufacturerName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    );
                TotalCount = lstSuppliers.Count();
                return lstSuppliers.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();

            }

        }

        public IEnumerable<SupplierCatalogItemDTO> GetAllItems()
        {
            bool IsLatest = false;
            IEnumerable<SupplierCatalogItemDTO> ObjCache = null;

            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IsLatest = dbcntx.SupplierCatalogTrackers.First().IsLatest;

                if (IsLatest)
                {
                    ObjCache = CacheHelper<IEnumerable<SupplierCatalogItemDTO>>.GetCacheItem("SupplierCatalog");
                    if (ObjCache == null)
                    {

                        IEnumerable<SupplierCatalogItemDTO> obj = (from scm in dbcntx.SupplierCatalogs
                                                                   select new SupplierCatalogItemDTO
                                                                   {
                                                                       SupplierCatalogItemID = scm.SupplierCatalogItemID,
                                                                       ItemNumber = scm.ItemNumber,
                                                                       Description = scm.Description,
                                                                       UPC = scm.UPC,
                                                                       SellPrice = scm.SellPrice,
                                                                       Cost = scm.Cost,
                                                                       UOM = scm.UOM,
                                                                       CostUOM = scm.CostUOM,
                                                                       ImagePath = scm.ImagePath,
                                                                       SupplierName = scm.SupplierName,
                                                                       SupplierPartNumber = scm.SupplierPartNumber,
                                                                       ManufacturerName = scm.ManufacturerName,
                                                                       ManufacturerPartNumber = scm.ManufacturerPartNumber,
                                                                       PackingQantity = scm.PackingQuantity
                                                                   }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<SupplierCatalogItemDTO>>.AddCacheItem("SupplierCatalog", obj);
                        dbcntx.SupplierCatalogTrackers.First().IsLatest = true;
                        dbcntx.SaveChanges();
                    }
                }
                else
                {

                    IEnumerable<SupplierCatalogItemDTO> obj = (from scm in dbcntx.SupplierCatalogs
                                                               select new SupplierCatalogItemDTO
                                                               {
                                                                   SupplierCatalogItemID = scm.SupplierCatalogItemID,
                                                                   ItemNumber = scm.ItemNumber,
                                                                   Description = scm.Description,
                                                                   UPC = scm.UPC,
                                                                   SellPrice = scm.SellPrice,
                                                                   Cost = scm.Cost,
                                                                   UOM = scm.UOM,
                                                                   CostUOM = scm.CostUOM,
                                                                   ImagePath = scm.ImagePath,
                                                                   SupplierName = scm.SupplierName,
                                                                   SupplierPartNumber = scm.SupplierPartNumber,
                                                                   ManufacturerName = scm.ManufacturerName,
                                                                   ManufacturerPartNumber = scm.ManufacturerPartNumber,
                                                                   PackingQantity = scm.PackingQuantity
                                                               }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<SupplierCatalogItemDTO>>.AddCacheItem("SupplierCatalog", obj);
                    dbcntx.SupplierCatalogTrackers.First().IsLatest = true;
                    dbcntx.SaveChanges();
                }
            }
            return ObjCache;
        }


    }
}
