namespace eTurns.DAL
{
    public partial class ItemMasterDAL : eTurnsBaseDAL
    {
        //public IEnumerable<ItemMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate, string Criteria = "Critical")
        //{
        //    //Get Cached-Media
        //    IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria);

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {
        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        if (SearchTerm.Contains("1000_1000"))
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                   ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0])))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //        else
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                                      ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                                   && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                                   && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                                   && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                                   && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                                   && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                                   && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                                   && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                                   && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                                   && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                                   && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0]) && t.Cost <= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[1])))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                                   && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                                  );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.SerialNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.LotNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.SerialNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.LotNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //}

        //public IEnumerable<ItemMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate, bool IsMoving, string Criteria = "Critical")
        //{
        //    //Get Cached-Media
        //    IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);

        //    // Need to get data for Slow Moving Fast Moving and Stock Out data and Graph 
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    RoomDTO objRoomDTO = objRoomDAL.GetRecord(RoomID, CompanyID, IsArchived, IsDeleted);

        //    if (objRoomDTO != null)
        //    {
        //        double SlowMoving = objRoomDTO.SlowMovingValue;
        //        double FastMoving = objRoomDTO.FastMovingValue;
        //        List<InventoryDashboardDTO> ObjInventoryDATA = new List<InventoryDashboardDTO>();
        //        ObjInventoryDATA = GetTurnsValueOfItems(RoomID, CompanyID);
        //        if (Criteria == "Slow Moving")
        //        {
        //            ObjCache = (from x in ObjCache
        //                        join y in ObjInventoryDATA on
        //                            x.GUID equals y.GUID
        //                        where Convert.ToDouble(y.Turns.GetValueOrDefault(0)) <= SlowMoving
        //                        && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
        //                        select x
        //                            ).ToList();
        //        }
        //        else if (Criteria == "Fast Moving")
        //        {
        //            ObjCache = (from x in ObjCache
        //                        join y in ObjInventoryDATA on
        //                            x.GUID equals y.GUID
        //                        where Convert.ToDouble(y.Turns.GetValueOrDefault(0)) >= FastMoving
        //                        && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
        //                        select x
        //                            ).ToList();
        //        }
        //        else if (Criteria == "Stock Out")
        //        {
        //            List<ItemMasterDTO> ObjStockOutCount = new List<ItemMasterDTO>();
        //            ObjStockOutCount = GetStockOutData(RoomID, CompanyID, FromDate, ToDate);

        //            ObjCache = (from x in ObjCache
        //                        where x.OnHandQuantity.GetValueOrDefault(0) <= 0
        //                        && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
        //                        select x
        //                            ).ToList();

        //            ObjCache = (from u in ObjCache
        //                        join y in ObjStockOutCount on u.ItemNumber equals y.ItemNumber
        //                        orderby y.StockOutCount descending
        //                        select new ItemMasterDTO
        //                        {
        //                            ItemNumber = u.ItemNumber,
        //                            OnHandQuantity = u.OnHandQuantity,
        //                            CriticalQuantity = u.CriticalQuantity,
        //                            MinimumQuantity = u.MinimumQuantity,
        //                            MaximumQuantity = u.MaximumQuantity,
        //                            OnOrderQuantity = u.OnOrderQuantity,
        //                            OnTransferQuantity = u.OnTransferQuantity,
        //                            AverageUsage = u.AverageUsage,
        //                            Created = u.Created,
        //                            GUID = u.GUID,
        //                            Updated = u.Updated,
        //                            CreatedBy = u.CreatedBy,
        //                            LastUpdatedBy = u.LastUpdatedBy,
        //                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                            CompanyID = u.CompanyID,
        //                            Room = u.Room,
        //                            Description = u.Description,
        //                            LongDescription = u.LongDescription,
        //                            DefaultReorderQuantity = u.DefaultReorderQuantity,
        //                            Turns = u.Turns,
        //                            InventoryClassification = u.InventoryClassification,
        //                            Cost = u.Cost.GetValueOrDefault(0),
        //                            StockOutCount = y.StockOutCount
        //                        }).ToList();
        //        }
        //    }

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {
        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        if (SearchTerm.Contains("1000_1000"))
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                   ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0])))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //        else
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                                      ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                                   && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                                   && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                                   && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                                   && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                                   && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                                   && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                                   && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                                   && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                                   && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                                   && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0]) && t.Cost <= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[1])))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                                   && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                                  );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.SerialNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.LotNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.SerialNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.LotNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //}

        //public List<ItemMasterDTO> GetBelowCriticalItems(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate, bool WithFilter)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return (from ish in context.ItemStockOutHistories
        //                join im in context.ItemMasters on ish.ItemGUID equals im.GUID
        //                where ish.RoomId == RoomID && ish.CompanyId == CompanyID && EntityFunctions.TruncateTime(ish.StockOutDate) >= EntityFunctions.TruncateTime(FromDate) && EntityFunctions.TruncateTime(ish.StockOutDate) <= EntityFunctions.TruncateTime(ToDate)
        //                group ish by new { ish.ItemGUID, im.ItemNumber } into groupedsts
        //                select new ItemMasterDTO
        //                {
        //                    ItemNumber = groupedsts.Key.ItemNumber,
        //                    LeadTimeInDays = groupedsts.Count(),
        //                    StockOutCount = groupedsts.Count(),
        //                }).ToList();
        //    }
        //}


        //public InventoryDashboardDTO GetHeaderCountByItemIDForService(Int64 RoomID, Int64 CompanyID, string ItemGUID, string ConnectionString)
        //{

        //    InventoryDashboardDTO ObjCache = null;

        //    string strQuery = @"SELECT Convert(decimal(18,2), ((Sum(ISNULL(MonthlyPulledQty,0)) / (Sum(ISNULL(InventoryValue,0))/" + DateTime.Now.Month.ToString() + ")) * 12/" + DateTime.Now.Month.ToString() + " )) AS Turns , Convert(decimal(18,2), Sum(ISNULL(InventoryValue,0))) AS [InventoryValue] FROM  DashboardItemCalculation WHERE Room = " + RoomID.ToString() + " AND CompanyID = " + CompanyID.ToString() + " AND ItemGUID = '" + ItemGUID + "'";

        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        ObjCache = (from u in context.Database.SqlQuery<InventoryDashboardDTO>(strQuery)
        //                    select new InventoryDashboardDTO
        //                    {
        //                        InventoryValue = u.InventoryValue,
        //                        Turns = u.Turns,
        //                    }).SingleOrDefault();


        //    }
        //    return ObjCache;
        //}
        //public List<ItemMaster> GetItems()
        //{
        //    List<ItemMaster> lstids = new List<ItemMaster>();
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        lstids = context.ItemMasters.Where(t => (t.IsDeleted ?? false) == false).ToList();
        //    }
        //    return lstids;
        //}

        //public ItemMasterDTO GetRecordByItemNumber(string ItemNumber, Int64 RoomID, Int64 CompanyID)
        //{
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO obj = null;
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        obj = (from u in context.Database.SqlQuery<ItemMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', 
        //                            C.UserName AS UpdatedByName, D.RoomName,M.Manufacturer as ManufacturerName,S.SupplierName, 
        //                            C1.Category AS CategoryName,U1.Unit,G1.GLAccount , B1.BinNumber as DefaultLocationName 
        //                            FROM ItemMaster A 
        //                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
        //                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                            LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                            LEFT OUTER join ManufacturerMaster M on M.id = A.ManufacturerID 
        //                            left outer join SupplierMaster S on S.id = A.SupplierID 
        //                            LEFT OUTER join CategoryMaster C1 on C1.id = A.CategoryID
        //                            LEFT OUTER join UnitMaster U1 on U1.id = A.UOMID
        //                            LEFT OUTER join GLAccountMaster G1 on G1.id = A.GLAccountID
        //                            LEFT OUTER join BinMaster B1 on B1.id = A.DefaultLocation
        //                            WHERE A.CompanyID = " + CompanyID.ToString() + " AND A.Room = " + RoomID.ToString() + " AND A.ItemNumber =  '" + ItemNumber + "'")
        //               select new ItemMasterDTO
        //               {
        //                   ID = u.ID,
        //                   ItemNumber = u.ItemNumber,
        //                   ManufacturerID = u.ManufacturerID,
        //                   ManufacturerNumber = u.ManufacturerNumber,
        //                   ManufacturerName = u.ManufacturerName,
        //                   SupplierID = u.SupplierID,
        //                   SupplierPartNo = u.SupplierPartNo,
        //                   SupplierName = u.SupplierName,
        //                   UPC = u.UPC,
        //                   UNSPSC = u.UNSPSC,
        //                   Description = u.Description,
        //                   LongDescription = u.LongDescription,
        //                   CategoryID = u.CategoryID,
        //                   GLAccountID = u.GLAccountID,
        //                   UOMID = u.UOMID,
        //                   PricePerTerm = u.PricePerTerm,
        //                   CostUOMID = u.CostUOMID,
        //                   DefaultReorderQuantity = u.DefaultReorderQuantity,
        //                   DefaultPullQuantity = u.DefaultPullQuantity,
        //                   Cost = u.Cost,
        //                   Markup = u.Markup,
        //                   SellPrice = u.SellPrice,
        //                   ExtendedCost = u.ExtendedCost,
        //                   AverageCost = u.AverageCost,
        //                   LeadTimeInDays = u.LeadTimeInDays,
        //                   Link1 = u.Link1,
        //                   Link2 = u.Link2,
        //                   Trend = u.Trend,
        //                   Taxable = u.Taxable,
        //                   Consignment = u.Consignment,
        //                   StagedQuantity = u.StagedQuantity,
        //                   InTransitquantity = u.InTransitquantity,
        //                   OnOrderQuantity = u.OnOrderQuantity,
        //                   OnReturnQuantity = u.OnReturnQuantity,
        //                   OnTransferQuantity = u.OnTransferQuantity,
        //                   SuggestedOrderQuantity = u.SuggestedOrderQuantity,
        //                   SuggestedTransferQuantity = u.SuggestedTransferQuantity,
        //                   RequisitionedQuantity = u.RequisitionedQuantity,
        //                   PackingQuantity = u.PackingQuantity,
        //                   AverageUsage = u.AverageUsage,
        //                   Turns = u.Turns,
        //                   OnHandQuantity = u.OnHandQuantity,
        //                   CriticalQuantity = u.CriticalQuantity,
        //                   MinimumQuantity = u.MinimumQuantity,
        //                   MaximumQuantity = u.MaximumQuantity,
        //                   WeightPerPiece = u.WeightPerPiece,
        //                   ItemUniqueNumber = u.ItemUniqueNumber,
        //                   //TransferOrPurchase = u.TransferOrPurchase,
        //                   IsPurchase = u.IsPurchase,
        //                   IsTransfer = u.IsTransfer,
        //                   DefaultLocation = u.DefaultLocation,
        //                   DefaultLocationName = u.DefaultLocationName,
        //                   InventoryClassification = u.InventoryClassification,
        //                   SerialNumberTracking = u.SerialNumberTracking,
        //                   LotNumberTracking = u.LotNumberTracking,
        //                   DateCodeTracking = u.DateCodeTracking,
        //                   ItemType = u.ItemType,
        //                   ImagePath = u.ImagePath,
        //                   UDF1 = u.UDF1,
        //                   UDF2 = u.UDF2,
        //                   UDF3 = u.UDF3,
        //                   UDF4 = u.UDF4,
        //                   UDF5 = u.UDF5,
        //                   GUID = u.GUID,
        //                   Created = u.Created,
        //                   Updated = u.Updated,
        //                   CreatedBy = u.CreatedBy,
        //                   LastUpdatedBy = u.LastUpdatedBy,
        //                   IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                   IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                   CompanyID = u.CompanyID,
        //                   Room = u.Room,
        //                   CreatedByName = u.CreatedByName,
        //                   UpdatedByName = u.UpdatedByName,
        //                   RoomName = u.RoomName,
        //                   IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
        //                   ItemTypeName = u.ItemTypeName,
        //                   CategoryName = u.CategoryName,
        //                   Unit = u.Unit,
        //                   GLAccount = u.GLAccount,
        //                   IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
        //                   IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
        //                   IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
        //                   BondedInventory = u.BondedInventory,
        //                   ItemLocations = objLocationDAL.GetAllRecords(RoomID, CompanyID, u.GUID, null, "ID ASC").ToList(),
        //                   IsBOMItem = u.IsBOMItem,
        //                   RefBomId = u.RefBomId,
        //               }).SingleOrDefault();
        //    }
        //    return obj;
        //}
        //public ItemMasterDTO GetHistoryRecord(Int64 id)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return (from u in context.Database.SqlQuery<ItemMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
        //                select new ItemMasterDTO
        //                {
        //                    ID = u.ID,
        //                    ItemNumber = u.ItemNumber,
        //                    ManufacturerID = u.ManufacturerID,
        //                    ManufacturerNumber = u.ManufacturerNumber,
        //                    SupplierID = u.SupplierID,
        //                    SupplierPartNo = u.SupplierPartNo,
        //                    UPC = u.UPC,
        //                    UNSPSC = u.UNSPSC,
        //                    Description = u.Description,
        //                    LongDescription = u.LongDescription,
        //                    CategoryID = u.CategoryID,
        //                    GLAccountID = u.GLAccountID,
        //                    UOMID = u.UOMID,
        //                    PricePerTerm = u.PricePerTerm,
        //                    CostUOMID = u.CostUOMID,
        //                    DefaultReorderQuantity = u.DefaultReorderQuantity,
        //                    DefaultPullQuantity = u.DefaultPullQuantity,
        //                    Cost = u.Cost,
        //                    Markup = u.Markup,
        //                    SellPrice = u.SellPrice,
        //                    ExtendedCost = u.ExtendedCost,
        //                    AverageCost = u.AverageCost,
        //                    LeadTimeInDays = u.LeadTimeInDays,
        //                    Link1 = u.Link1,
        //                    Link2 = u.Link2,
        //                    Trend = u.Trend,
        //                    Taxable = u.Taxable,
        //                    Consignment = u.Consignment,
        //                    StagedQuantity = u.StagedQuantity,
        //                    InTransitquantity = u.InTransitquantity,
        //                    OnOrderQuantity = u.OnOrderQuantity,
        //                    OnReturnQuantity = u.OnReturnQuantity,
        //                    OnTransferQuantity = u.OnTransferQuantity,
        //                    SuggestedOrderQuantity = u.SuggestedOrderQuantity,
        //                    SuggestedTransferQuantity = u.SuggestedTransferQuantity,
        //                    RequisitionedQuantity = u.RequisitionedQuantity,
        //                    PackingQuantity = u.PackingQuantity,
        //                    AverageUsage = u.AverageUsage,
        //                    Turns = u.Turns,
        //                    OnHandQuantity = u.OnHandQuantity,
        //                    CriticalQuantity = u.CriticalQuantity,
        //                    MinimumQuantity = u.MinimumQuantity,
        //                    MaximumQuantity = u.MaximumQuantity,
        //                    WeightPerPiece = u.WeightPerPiece,
        //                    ItemUniqueNumber = u.ItemUniqueNumber,
        //                    //TransferOrPurchase = u.TransferOrPurchase,
        //                    IsPurchase = u.IsPurchase,
        //                    IsTransfer = u.IsTransfer,
        //                    DefaultLocation = u.DefaultLocation,
        //                    InventoryClassification = u.InventoryClassification,
        //                    SerialNumberTracking = u.SerialNumberTracking,
        //                    LotNumberTracking = u.LotNumberTracking,
        //                    DateCodeTracking = u.DateCodeTracking,
        //                    ItemType = u.ItemType,
        //                    ImagePath = u.ImagePath,
        //                    UDF1 = u.UDF1,
        //                    UDF2 = u.UDF2,
        //                    UDF3 = u.UDF3,
        //                    UDF4 = u.UDF4,
        //                    UDF5 = u.UDF5,
        //                    UDF6 = u.UDF6,
        //                    UDF7 = u.UDF7,
        //                    UDF8 = u.UDF8,
        //                    UDF9 = u.UDF9,
        //                    UDF10 = u.UDF10,
        //                    GUID = u.GUID,
        //                    Created = u.Created,
        //                    Updated = u.Updated,
        //                    CreatedBy = u.CreatedBy,
        //                    LastUpdatedBy = u.LastUpdatedBy,
        //                    IsDeleted = u.IsDeleted,
        //                    IsArchived = u.IsArchived,
        //                    CompanyID = u.CompanyID,
        //                    Room = u.Room,
        //                    IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
        //                    BondedInventory = u.BondedInventory,
        //                    IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
        //                    IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
        //                    IsBOMItem = u.IsBOMItem,
        //                    RefBomId = u.RefBomId,
        //                }).SingleOrDefault();
        //    }
        //}
        //public IEnumerable<ItemMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 SupplierID = 0)
        //{
        //    //Get Cached-Media
        //    IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);

        //    if (SupplierID > 0)
        //        ObjCache = ObjCache.Where(x => x.SupplierID.GetValueOrDefault(0) == SupplierID);

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        if (SearchTerm.Contains("100_1000"))
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                   ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDecimal(Fields[1].Split('@')[15].Split('_')[0])))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //        else if (SearchTerm.Contains("10_1000"))
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                   ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                && ((Fields[1].Split('@')[15] == "") || (t.Cost <= Convert.ToDecimal(Fields[1].Split('@')[15].Split('_')[0])))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //        else
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                                      ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                                   && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                                   && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                                   && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                                   && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                                   && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                                   && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                                   && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                                   && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                                   && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                                   && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDecimal(Fields[1].Split('@')[15].Split('_')[0]) && t.Cost <= Convert.ToDecimal(Fields[1].Split('@')[15].Split('_')[1])))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                                   && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                                  );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                    //--------------------------------------------------------------------------------------------
        //                 (t.UNSPSC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                  (t.OnHandQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                   (t.OnOrderQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.SuggestedOrderQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.MinimumQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.MaximumQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.InTransitquantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.RequisitionedQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.CriticalQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.CategoryName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.AverageUsage.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.DefaultReorderQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.DefaultPullQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.StagedQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                 (t.WeightPerPiece.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                  (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                   (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                   (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                   (t.Unit ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                   (t.AverageCost.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                    //--------------------------------------------------------------------------------------------
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.SerialNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.LotNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                //--------------------------------------------------------------------------------------------
        //                (t.UNSPSC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.OnHandQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.OnOrderQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SuggestedOrderQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.MinimumQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.MaximumQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.InTransitquantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.RequisitionedQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CriticalQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CategoryName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.AverageUsage.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.DefaultReorderQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.DefaultPullQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.StagedQuantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.WeightPerPiece.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Unit ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.AverageCost.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                //--------------------------------------------------------------------------------------------

        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.SerialNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                ((t.ItemLocations != null) && (t.ItemLocations.Where(x => (x.LotNumber ?? "").Contains(SearchTerm)).Count() > 0)) ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //}
        //public ItemMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO obj = null;

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        obj = (from im in context.ItemMasters
        //               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //               from im_sm in im_sm_join.DefaultIfEmpty()

        //               join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //               from im_mm in im_mm_join.DefaultIfEmpty()

        //               join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //               from im_rm in im_rm_join.DefaultIfEmpty()

        //               join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //               from im_UMC in im_UMC_join.DefaultIfEmpty()

        //               join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //               from im_UMU in im_UMU_join.DefaultIfEmpty()

        //               join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //               from im_CM in im_CM_join.DefaultIfEmpty()

        //               join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //               from im_UNM in im_UNM_join.DefaultIfEmpty()

        //               join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //               from im_gla in im_gla_join.DefaultIfEmpty()


        //               join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //               from im_bm in im_bm_join.DefaultIfEmpty()

        //               where im.CompanyID == CompanyID && im.Room == RoomID
        //               && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived
        //               && im.ID == id

        //               select new ItemMasterDTO
        //               {
        //                   ID = im.ID,
        //                   ItemNumber = im.ItemNumber,
        //                   ManufacturerID = im.ManufacturerID,
        //                   ManufacturerNumber = im.ManufacturerNumber,
        //                   ManufacturerName = im_mm.Manufacturer,
        //                   SupplierID = im.SupplierID,
        //                   SupplierPartNo = im.SupplierPartNo,
        //                   SupplierName = im_sm.SupplierName,
        //                   UPC = im.UPC,
        //                   UNSPSC = im.UNSPSC,
        //                   Description = im.Description,
        //                   LongDescription = im.LongDescription,
        //                   CategoryID = im.CategoryID,
        //                   GLAccountID = im.GLAccountID,
        //                   UOMID = im.UOMID == null ? 0 : (Int64)im.UOMID,
        //                   PricePerTerm = im.PricePerTerm,
        //                   CostUOMID = im.CostUOMID,
        //                   DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                   DefaultPullQuantity = im.DefaultPullQuantity,
        //                   Cost = im.Cost,
        //                   Markup = im.Markup,
        //                   SellPrice = im.SellPrice,
        //                   ExtendedCost = im.ExtendedCost,
        //                   AverageCost = im.AverageCost,
        //                   LeadTimeInDays = im.LeadTimeInDays,
        //                   Link1 = im.Link1,
        //                   Link2 = im.Link2,
        //                   Trend = im.Trend,
        //                   Taxable = im.Taxable,
        //                   Consignment = im.Consignment,
        //                   StagedQuantity = im.StagedQuantity,
        //                   InTransitquantity = im.InTransitquantity,
        //                   OnOrderQuantity = im.OnOrderQuantity,
        //                   OnReturnQuantity = im.OnReturnQuantity,
        //                   OnTransferQuantity = im.OnTransferQuantity,
        //                   SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                   SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                   RequisitionedQuantity = im.RequisitionedQuantity,
        //                   PackingQuantity = im.PackingQuantity,
        //                   AverageUsage = im.AverageUsage,
        //                   Turns = im.Turns,
        //                   OnHandQuantity = im.OnHandQuantity,
        //                   CriticalQuantity = im.CriticalQuantity,
        //                   MinimumQuantity = im.MinimumQuantity,
        //                   MaximumQuantity = im.MaximumQuantity,
        //                   WeightPerPiece = im.WeightPerPiece,
        //                   ItemUniqueNumber = im.ItemUniqueNumber,
        //                   IsPurchase = (im.IsPurchase == true ? true : false),
        //                   IsTransfer = (im.IsTransfer == true ? true : false),
        //                   DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                   DefaultLocationName = im_bm.BinNumber,
        //                   InventoryClassification = im.InventoryClassification,
        //                   SerialNumberTracking = im.SerialNumberTracking,
        //                   LotNumberTracking = im.LotNumberTracking,
        //                   DateCodeTracking = im.DateCodeTracking,
        //                   ItemType = im.ItemType,
        //                   ImagePath = im.ImagePath,
        //                   UDF1 = im.UDF1,
        //                   UDF2 = im.UDF2,
        //                   UDF3 = im.UDF3,
        //                   UDF4 = im.UDF4,
        //                   UDF5 = im.UDF5,
        //                   GUID = im.GUID,
        //                   Created = im.Created,
        //                   Updated = im.Updated,
        //                   CreatedBy = im.CreatedBy,
        //                   LastUpdatedBy = im.LastUpdatedBy,
        //                   IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                   IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                   CompanyID = im.CompanyID,
        //                   Room = im.Room,
        //                   CreatedByName = im_UMC.UserName,
        //                   UpdatedByName = im_UMU.UserName,
        //                   RoomName = im_rm.RoomName,
        //                   IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                   CategoryName = im_CM.Category,
        //                   Unit = im_UNM.Unit,
        //                   GLAccount = im_gla.GLAccount,
        //                   IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                   IsBuildBreak = im.IsBuildBreak,
        //                   BondedInventory = im.BondedInventory,
        //                   PullQtyScanOverride = im.PullQtyScanOverride,
        //                   TrendingSetting = im.TrendingSetting,
        //                   IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                   IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                   LastCost = im.LastCost,
        //                   ItemImageExternalURL = im.ItemImageExternalURL,
        //                   ImageType = im.ImageType,
        //                   ItemLocations = (from A in context.ItemLocationDetails
        //                                    join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                    from A_B in A_B_join.DefaultIfEmpty()
        //                                    join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                    from A_C in A_C_join.DefaultIfEmpty()
        //                                    join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                    from A_D in A_D_join.DefaultIfEmpty()
        //                                    where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                    select new ItemLocationDetailsDTO
        //                                    {
        //                                        ID = A.ID,
        //                                        BinID = A.BinID,
        //                                        CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                        ConsignedQuantity = A.ConsignedQuantity,
        //                                        MeasurementID = A.MeasurementID,
        //                                        LotNumber = A.LotNumber,
        //                                        SerialNumber = A.SerialNumber,
        //                                        ExpirationDate = A.ExpirationDate,
        //                                        ReceivedDate = A.ReceivedDate,
        //                                        Expiration = A.Expiration,
        //                                        Received = A.Received,
        //                                        Cost = A.Cost,
        //                                        GUID = A.GUID,
        //                                        ItemGUID = A.ItemGUID,
        //                                        Created = A.Created,
        //                                        Updated = A.Updated,
        //                                        CreatedBy = A.CreatedBy,
        //                                        LastUpdatedBy = A.LastUpdatedBy,
        //                                        IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                        IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                        CompanyID = A.CompanyID,
        //                                        Room = A.Room,
        //                                        CreatedByName = A_C.UserName,
        //                                        UpdatedByName = A_D.UserName,
        //                                        BinNumber = A_B.BinNumber,
        //                                        ItemNumber = im.ItemNumber,
        //                                        SerialNumberTracking = im.SerialNumberTracking,
        //                                        LotNumberTracking = im.LotNumberTracking,
        //                                        DateCodeTracking = im.DateCodeTracking,
        //                                        OrderDetailGUID = A.OrderDetailGUID,
        //                                        TransferDetailGUID = A.TransferDetailGUID,
        //                                        KitDetailGUID = A.KitDetailGUID,
        //                                        CriticalQuantity = A_B.CriticalQuantity,
        //                                        MinimumQuantity = A_B.MinimumQuantity,
        //                                        MaximumQuantity = A_B.MaximumQuantity,
        //                                        SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                    }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                   IsBOMItem = im.IsBOMItem,
        //                   RefBomId = im.RefBomId,
        //               }).SingleOrDefault();
        //    }
        //    return obj;
        //}

        //public ItemMasterDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO obj = null;

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        obj = (from im in context.ItemMasters
        //               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //               from im_sm in im_sm_join.DefaultIfEmpty()

        //               join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //               from im_mm in im_mm_join.DefaultIfEmpty()

        //               join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //               from im_rm in im_rm_join.DefaultIfEmpty()

        //               join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //               from im_UMC in im_UMC_join.DefaultIfEmpty()

        //               join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //               from im_UMU in im_UMU_join.DefaultIfEmpty()

        //               join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //               from im_CM in im_CM_join.DefaultIfEmpty()

        //               join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //               from im_UNM in im_UNM_join.DefaultIfEmpty()

        //               join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //               from im_gla in im_gla_join.DefaultIfEmpty()


        //               join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //               from im_bm in im_bm_join.DefaultIfEmpty()

        //               where im.CompanyID == CompanyID && im.Room == RoomID
        //               && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived
        //               && im.GUID == new Guid(GUID)

        //               select new ItemMasterDTO
        //               {
        //                   ID = im.ID,
        //                   ItemNumber = im.ItemNumber,
        //                   ManufacturerID = im.ManufacturerID,
        //                   ManufacturerNumber = im.ManufacturerNumber,
        //                   ManufacturerName = im_mm.Manufacturer,
        //                   SupplierID = im.SupplierID,
        //                   SupplierPartNo = im.SupplierPartNo,
        //                   SupplierName = im_sm.SupplierName,
        //                   UPC = im.UPC,
        //                   UNSPSC = im.UNSPSC,
        //                   Description = im.Description,
        //                   LongDescription = im.LongDescription,
        //                   CategoryID = im.CategoryID,
        //                   GLAccountID = im.GLAccountID,
        //                   UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
        //                   PricePerTerm = im.PricePerTerm,
        //                   CostUOMID = im.CostUOMID,
        //                   DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                   DefaultPullQuantity = im.DefaultPullQuantity,
        //                   Cost = im.Cost,
        //                   Markup = im.Markup,
        //                   SellPrice = im.SellPrice,
        //                   ExtendedCost = im.ExtendedCost,
        //                   AverageCost = im.AverageCost,
        //                   LeadTimeInDays = im.LeadTimeInDays,
        //                   Link1 = im.Link1,
        //                   Link2 = im.Link2,
        //                   Trend = im.Trend,
        //                   Taxable = im.Taxable,
        //                   Consignment = im.Consignment,
        //                   StagedQuantity = im.StagedQuantity,
        //                   InTransitquantity = im.InTransitquantity,
        //                   OnOrderQuantity = im.OnOrderQuantity,
        //                   OnReturnQuantity = im.OnReturnQuantity,
        //                   OnTransferQuantity = im.OnTransferQuantity,
        //                   SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                   SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                   RequisitionedQuantity = im.RequisitionedQuantity,
        //                   PackingQuantity = im.PackingQuantity,
        //                   AverageUsage = im.AverageUsage,
        //                   Turns = im.Turns,
        //                   OnHandQuantity = im.OnHandQuantity,
        //                   CriticalQuantity = im.CriticalQuantity,
        //                   MinimumQuantity = im.MinimumQuantity,
        //                   MaximumQuantity = im.MaximumQuantity,
        //                   WeightPerPiece = im.WeightPerPiece,
        //                   ItemUniqueNumber = im.ItemUniqueNumber,
        //                   IsPurchase = (im.IsPurchase == true ? true : false),
        //                   IsTransfer = (im.IsTransfer == true ? true : false),
        //                   DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                   DefaultLocationName = im_bm.BinNumber,
        //                   InventoryClassification = im.InventoryClassification,
        //                   SerialNumberTracking = im.SerialNumberTracking,
        //                   LotNumberTracking = im.LotNumberTracking,
        //                   DateCodeTracking = im.DateCodeTracking,
        //                   ItemType = im.ItemType,
        //                   ImagePath = im.ImagePath,
        //                   UDF1 = im.UDF1,
        //                   UDF2 = im.UDF2,
        //                   UDF3 = im.UDF3,
        //                   UDF4 = im.UDF4,
        //                   UDF5 = im.UDF5,
        //                   GUID = im.GUID,
        //                   Created = im.Created,
        //                   Updated = im.Updated,
        //                   CreatedBy = im.CreatedBy,
        //                   LastUpdatedBy = im.LastUpdatedBy,
        //                   IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                   IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                   CompanyID = im.CompanyID,
        //                   Room = im.Room,
        //                   CreatedByName = im_UMC.UserName,
        //                   UpdatedByName = im_UMU.UserName,
        //                   RoomName = im_rm.RoomName,
        //                   IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                   CategoryName = im_CM.Category,
        //                   Unit = im_UNM.Unit,
        //                   GLAccount = im_gla.GLAccount,
        //                   IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                   IsBuildBreak = im.IsBuildBreak,
        //                   BondedInventory = im.BondedInventory,
        //                   PullQtyScanOverride = im.PullQtyScanOverride,
        //                   TrendingSetting = im.TrendingSetting,
        //                   IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                   IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                   IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
        //                   LastCost = im.LastCost,
        //                   ReceivedOn = im.ReceivedOn,
        //                   ReceivedOnWeb = im.ReceivedOnWeb,
        //                   AddedFrom = im.AddedFrom,
        //                   EditedFrom = im.EditedFrom,
        //                   ItemImageExternalURL = im.ItemImageExternalURL,
        //                   ImageType = im.ImageType,
        //                   ItemDocExternalURL = im.ItemDocExternalURL,
        //                   QtyToMeetDemand = im.QtyToMeetDemand,

        //                   ItemLocations = (from A in context.ItemLocationDetails
        //                                    join I in context.ItemMasters on A.ItemGUID equals I.GUID into A_I_join
        //                                    from A_I in A_I_join.DefaultIfEmpty()
        //                                    join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                    from A_B in A_B_join.DefaultIfEmpty()
        //                                    join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                    from A_C in A_C_join.DefaultIfEmpty()
        //                                    join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                    from A_D in A_D_join.DefaultIfEmpty()
        //                                    where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                    select new ItemLocationDetailsDTO
        //                                    {
        //                                        ID = A.ID,
        //                                        BinID = A.BinID,
        //                                        CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                        ConsignedQuantity = A.ConsignedQuantity,
        //                                        MeasurementID = A.MeasurementID,
        //                                        LotNumber = A.LotNumber,
        //                                        SerialNumber = A.SerialNumber,
        //                                        ExpirationDate = A.ExpirationDate,
        //                                        ReceivedDate = A.ReceivedDate,
        //                                        Expiration = A.Expiration,
        //                                        Received = A.Received,
        //                                        Cost = A.Cost,
        //                                        GUID = A.GUID,
        //                                        ItemGUID = A.ItemGUID,
        //                                        Created = A.Created,
        //                                        Updated = A.Updated,
        //                                        CreatedBy = A.CreatedBy,
        //                                        LastUpdatedBy = A.LastUpdatedBy,
        //                                        IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                        IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                        CompanyID = A.CompanyID,
        //                                        Room = A.Room,
        //                                        CreatedByName = A_C.UserName,
        //                                        UpdatedByName = A_D.UserName,
        //                                        BinNumber = A_B.BinNumber,
        //                                        ItemNumber = im.ItemNumber,
        //                                        SerialNumberTracking = im.SerialNumberTracking,
        //                                        LotNumberTracking = im.LotNumberTracking,
        //                                        DateCodeTracking = im.DateCodeTracking,
        //                                        OrderDetailGUID = A.OrderDetailGUID,
        //                                        TransferDetailGUID = A.TransferDetailGUID,
        //                                        KitDetailGUID = A.KitDetailGUID,
        //                                        CriticalQuantity = A_B.CriticalQuantity,
        //                                        MinimumQuantity = A_B.MinimumQuantity,
        //                                        MaximumQuantity = A_B.MaximumQuantity,
        //                                        Markup = A_I.Markup,
        //                                        SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                        AddedFrom = A.AddedFrom,
        //                                        EditedFrom = A.EditedFrom,
        //                                        ReceivedOn = A.ReceivedOn,
        //                                        ReceivedOnWeb = A.ReceivedOnWeb
        //                                    }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                   IsBOMItem = im.IsBOMItem,
        //                   RefBomId = im.RefBomId
        //                   //CreatedDate = CommonUtility.ConvertDateByTimeZone(im.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
        //                   //UpdatedDate = CommonUtility.ConvertDateByTimeZone(im.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true)

        //               }).FirstOrDefault();
        //    }
        //    return obj;

        //}

        //public ItemMasterDTO GetRecordByItemNumber(string ItemNumber, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO obj = null;

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        obj = (from im in context.ItemMasters
        //               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //               from im_sm in im_sm_join.DefaultIfEmpty()

        //               join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //               from im_mm in im_mm_join.DefaultIfEmpty()

        //               join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //               from im_rm in im_rm_join.DefaultIfEmpty()

        //               join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //               from im_UMC in im_UMC_join.DefaultIfEmpty()

        //               join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //               from im_UMU in im_UMU_join.DefaultIfEmpty()

        //               join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //               from im_CM in im_CM_join.DefaultIfEmpty()

        //               join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //               from im_UNM in im_UNM_join.DefaultIfEmpty()

        //               join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //               from im_gla in im_gla_join.DefaultIfEmpty()

        //               join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //               from im_bm in im_bm_join.DefaultIfEmpty()

        //               where im.CompanyID == CompanyID && im.Room == RoomID
        //               && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived
        //               && im.ItemNumber == ItemNumber

        //               select new ItemMasterDTO
        //               {
        //                   ID = im.ID,
        //                   ItemNumber = im.ItemNumber,
        //                   ManufacturerID = im.ManufacturerID,
        //                   ManufacturerNumber = im.ManufacturerNumber,
        //                   ManufacturerName = im_mm.Manufacturer,
        //                   SupplierID = im.SupplierID,
        //                   SupplierPartNo = im.SupplierPartNo,
        //                   SupplierName = im_sm.SupplierName,
        //                   UPC = im.UPC,
        //                   UNSPSC = im.UNSPSC,
        //                   Description = im.Description,
        //                   LongDescription = im.LongDescription,
        //                   CategoryID = im.CategoryID,
        //                   GLAccountID = im.GLAccountID,
        //                   UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
        //                   PricePerTerm = im.PricePerTerm,
        //                   CostUOMID = im.CostUOMID,
        //                   DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                   DefaultPullQuantity = im.DefaultPullQuantity,
        //                   Cost = im.Cost,
        //                   Markup = im.Markup,
        //                   SellPrice = im.SellPrice,
        //                   ExtendedCost = im.ExtendedCost,
        //                   AverageCost = im.AverageCost,
        //                   LeadTimeInDays = im.LeadTimeInDays,
        //                   Link1 = im.Link1,
        //                   Link2 = im.Link2,
        //                   Trend = im.Trend,
        //                   Taxable = im.Taxable,
        //                   Consignment = im.Consignment,
        //                   StagedQuantity = im.StagedQuantity,
        //                   InTransitquantity = im.InTransitquantity,
        //                   OnOrderQuantity = im.OnOrderQuantity,
        //                   OnReturnQuantity = im.OnReturnQuantity,
        //                   OnTransferQuantity = im.OnTransferQuantity,
        //                   SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                   SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                   RequisitionedQuantity = im.RequisitionedQuantity,
        //                   PackingQuantity = im.PackingQuantity,
        //                   AverageUsage = im.AverageUsage,
        //                   Turns = im.Turns,
        //                   OnHandQuantity = im.OnHandQuantity,
        //                   CriticalQuantity = im.CriticalQuantity,
        //                   MinimumQuantity = im.MinimumQuantity,
        //                   MaximumQuantity = im.MaximumQuantity,
        //                   WeightPerPiece = im.WeightPerPiece,
        //                   ItemUniqueNumber = im.ItemUniqueNumber,
        //                   IsPurchase = (im.IsPurchase == true ? true : false),
        //                   IsTransfer = (im.IsTransfer == true ? true : false),
        //                   DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                   DefaultLocationName = im_bm.BinNumber,
        //                   InventoryClassification = im.InventoryClassification,
        //                   SerialNumberTracking = im.SerialNumberTracking,
        //                   LotNumberTracking = im.LotNumberTracking,
        //                   DateCodeTracking = im.DateCodeTracking,
        //                   ItemType = im.ItemType,
        //                   ImagePath = im.ImagePath,
        //                   UDF1 = im.UDF1,
        //                   UDF2 = im.UDF2,
        //                   UDF3 = im.UDF3,
        //                   UDF4 = im.UDF4,
        //                   UDF5 = im.UDF5,
        //                   GUID = im.GUID,
        //                   Created = im.Created,
        //                   Updated = im.Updated,
        //                   CreatedBy = im.CreatedBy,
        //                   LastUpdatedBy = im.LastUpdatedBy,
        //                   IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                   IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                   CompanyID = im.CompanyID,
        //                   Room = im.Room,
        //                   CreatedByName = im_UMC.UserName,
        //                   UpdatedByName = im_UMU.UserName,
        //                   RoomName = im_rm.RoomName,
        //                   IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                   CategoryName = im_CM.Category,
        //                   Unit = im_UNM.Unit,
        //                   GLAccount = im_gla.GLAccount,
        //                   IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                   IsBuildBreak = im.IsBuildBreak,
        //                   BondedInventory = im.BondedInventory,
        //                   PullQtyScanOverride = im.PullQtyScanOverride,
        //                   TrendingSetting = im.TrendingSetting,
        //                   IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                   IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                   LastCost = im.LastCost,
        //                   ItemImageExternalURL = im.ItemImageExternalURL,
        //                   ImageType = im.ImageType,
        //                   ItemLocations = (from A in context.ItemLocationDetails
        //                                    join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                    from A_B in A_B_join.DefaultIfEmpty()
        //                                    join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                    from A_C in A_C_join.DefaultIfEmpty()
        //                                    join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                    from A_D in A_D_join.DefaultIfEmpty()
        //                                    where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                    select new ItemLocationDetailsDTO
        //                                    {
        //                                        ID = A.ID,
        //                                        BinID = A.BinID,
        //                                        CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                        ConsignedQuantity = A.ConsignedQuantity,
        //                                        MeasurementID = A.MeasurementID,
        //                                        LotNumber = A.LotNumber,
        //                                        SerialNumber = A.SerialNumber,
        //                                        ExpirationDate = A.ExpirationDate,
        //                                        ReceivedDate = A.ReceivedDate,
        //                                        Expiration = A.Expiration,
        //                                        Received = A.Received,
        //                                        Cost = A.Cost,
        //                                        GUID = A.GUID,
        //                                        ItemGUID = A.ItemGUID,
        //                                        Created = A.Created,
        //                                        Updated = A.Updated,
        //                                        CreatedBy = A.CreatedBy,
        //                                        LastUpdatedBy = A.LastUpdatedBy,
        //                                        IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                        IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                        CompanyID = A.CompanyID,
        //                                        Room = A.Room,
        //                                        CreatedByName = A_C.UserName,
        //                                        UpdatedByName = A_D.UserName,
        //                                        BinNumber = A_B.BinNumber,
        //                                        ItemNumber = im.ItemNumber,
        //                                        SerialNumberTracking = im.SerialNumberTracking,
        //                                        LotNumberTracking = im.LotNumberTracking,
        //                                        DateCodeTracking = im.DateCodeTracking,
        //                                        OrderDetailGUID = A.OrderDetailGUID,
        //                                        TransferDetailGUID = A.TransferDetailGUID,
        //                                        KitDetailGUID = A.KitDetailGUID,
        //                                        CriticalQuantity = A_B.CriticalQuantity,
        //                                        MinimumQuantity = A_B.MinimumQuantity,
        //                                        MaximumQuantity = A_B.MaximumQuantity,
        //                                        SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                    }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                   IsBOMItem = im.IsBOMItem,
        //                   RefBomId = im.RefBomId,

        //               }).FirstOrDefault();
        //    }


        //    return obj;
        //}

        //public ItemMasterDTO GetRecordByItemNumber(string ItemNumber, Int64 RoomID)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return (from im in context.ItemMasters
        //                where (im.IsDeleted ?? false) == false && (im.IsArchived ?? false) == false && im.Room == RoomID && im.ItemNumber == ItemNumber
        //                select new ItemMasterDTO
        //                {
        //                    ID = im.ID,
        //                    ItemNumber = im.ItemNumber,
        //                    ManufacturerID = im.ManufacturerID,
        //                    ManufacturerNumber = im.ManufacturerNumber,
        //                    SupplierID = im.SupplierID,
        //                    SupplierPartNo = im.SupplierPartNo,
        //                    UPC = im.UPC,
        //                    UNSPSC = im.UNSPSC,
        //                    Description = im.Description,
        //                    LongDescription = im.LongDescription,
        //                    CategoryID = im.CategoryID,
        //                    GLAccountID = im.GLAccountID,
        //                    UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
        //                    PricePerTerm = im.PricePerTerm,
        //                    CostUOMID = im.CostUOMID,
        //                    DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                    DefaultPullQuantity = im.DefaultPullQuantity,
        //                    Cost = im.Cost,
        //                    Markup = im.Markup,
        //                    SellPrice = im.SellPrice,
        //                    ExtendedCost = im.ExtendedCost,
        //                    AverageCost = im.AverageCost,
        //                    LeadTimeInDays = im.LeadTimeInDays,
        //                    Link1 = im.Link1,
        //                    Link2 = im.Link2,
        //                    Trend = im.Trend,
        //                    Taxable = im.Taxable,
        //                    Consignment = im.Consignment,
        //                    StagedQuantity = im.StagedQuantity,
        //                    InTransitquantity = im.InTransitquantity,
        //                    OnOrderQuantity = im.OnOrderQuantity,
        //                    OnReturnQuantity = im.OnReturnQuantity,
        //                    OnTransferQuantity = im.OnTransferQuantity,
        //                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                    SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                    RequisitionedQuantity = im.RequisitionedQuantity,
        //                    PackingQuantity = im.PackingQuantity,
        //                    AverageUsage = im.AverageUsage,
        //                    Turns = im.Turns,
        //                    OnHandQuantity = im.OnHandQuantity,
        //                    CriticalQuantity = im.CriticalQuantity,
        //                    MinimumQuantity = im.MinimumQuantity,
        //                    MaximumQuantity = im.MaximumQuantity,
        //                    WeightPerPiece = im.WeightPerPiece,
        //                    ItemUniqueNumber = im.ItemUniqueNumber,
        //                    IsPurchase = im.IsPurchase ?? false,
        //                    IsTransfer = im.IsTransfer ?? false,
        //                    DefaultLocation = im.DefaultLocation ?? 0,
        //                    InventoryClassification = im.InventoryClassification,
        //                    SerialNumberTracking = im.SerialNumberTracking,
        //                    LotNumberTracking = im.LotNumberTracking,
        //                    DateCodeTracking = im.DateCodeTracking,
        //                    ItemType = im.ItemType,
        //                    ImagePath = im.ImagePath,
        //                    UDF1 = im.UDF1,
        //                    UDF2 = im.UDF2,
        //                    UDF3 = im.UDF3,
        //                    UDF4 = im.UDF4,
        //                    UDF5 = im.UDF5,
        //                    GUID = im.GUID,
        //                    Created = im.Created,
        //                    Updated = im.Updated,
        //                    CreatedBy = im.CreatedBy,
        //                    LastUpdatedBy = im.LastUpdatedBy,
        //                    IsDeleted = im.IsDeleted ?? false,
        //                    IsArchived = im.IsArchived ?? false,
        //                    CompanyID = im.CompanyID,
        //                    Room = im.Room,
        //                    IsItemLevelMinMaxQtyRequired = im.IsItemLevelMinMaxQtyRequired ?? false,
        //                    IsBuildBreak = im.IsBuildBreak,
        //                    BondedInventory = im.BondedInventory,
        //                    PullQtyScanOverride = im.PullQtyScanOverride,
        //                    TrendingSetting = im.TrendingSetting,
        //                    IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                    IsEnforceDefaultReorderQuantity = im.IsEnforceDefaultReorderQuantity ?? false,
        //                    IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
        //                    LastCost = im.LastCost,
        //                    ReceivedOn = im.ReceivedOn,
        //                    ReceivedOnWeb = im.ReceivedOnWeb,
        //                    AddedFrom = im.AddedFrom,
        //                    EditedFrom = im.EditedFrom,
        //                    ItemImageExternalURL = im.ItemImageExternalURL,
        //                    ImageType = im.ImageType,
        //                    ItemDocExternalURL = im.ItemDocExternalURL,
        //                    QtyToMeetDemand = im.QtyToMeetDemand,
        //                    IsBOMItem = im.IsBOMItem,
        //                    RefBomId = im.RefBomId
        //                }).FirstOrDefault();
        //    }
        //}

        //public ItemMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        //{
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO obj = null;

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        obj = (from im in context.ItemMasters
        //               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //               from im_sm in im_sm_join.DefaultIfEmpty()

        //               join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //               from im_mm in im_mm_join.DefaultIfEmpty()

        //               join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //               from im_rm in im_rm_join.DefaultIfEmpty()

        //               join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //               from im_UMC in im_UMC_join.DefaultIfEmpty()

        //               join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //               from im_UMU in im_UMU_join.DefaultIfEmpty()

        //               join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //               from im_CM in im_CM_join.DefaultIfEmpty()

        //               join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //               from im_UNM in im_UNM_join.DefaultIfEmpty()

        //               join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //               from im_gla in im_gla_join.DefaultIfEmpty()

        //               join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //               from im_bm in im_bm_join.DefaultIfEmpty()

        //               where im.CompanyID == CompanyID && im.Room == RoomID
        //               && im.ID == id

        //               select new ItemMasterDTO
        //               {
        //                   ID = im.ID,
        //                   ItemNumber = im.ItemNumber,
        //                   ManufacturerID = im.ManufacturerID,
        //                   ManufacturerNumber = im.ManufacturerNumber,
        //                   ManufacturerName = im_mm.Manufacturer,
        //                   SupplierID = im.SupplierID,
        //                   SupplierPartNo = im.SupplierPartNo,
        //                   SupplierName = im_sm.SupplierName,
        //                   UPC = im.UPC,
        //                   UNSPSC = im.UNSPSC,
        //                   Description = im.Description,
        //                   LongDescription = im.LongDescription,
        //                   CategoryID = im.CategoryID,
        //                   GLAccountID = im.GLAccountID,
        //                   UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
        //                   PricePerTerm = im.PricePerTerm,
        //                   CostUOMID = im.CostUOMID,
        //                   DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                   DefaultPullQuantity = im.DefaultPullQuantity,
        //                   Cost = im.Cost,
        //                   Markup = im.Markup,
        //                   SellPrice = im.SellPrice,
        //                   ExtendedCost = im.ExtendedCost,
        //                   AverageCost = im.AverageCost,
        //                   LeadTimeInDays = im.LeadTimeInDays,
        //                   Link1 = im.Link1,
        //                   Link2 = im.Link2,
        //                   Trend = im.Trend,
        //                   Taxable = im.Taxable,
        //                   Consignment = im.Consignment,
        //                   StagedQuantity = im.StagedQuantity,
        //                   InTransitquantity = im.InTransitquantity,
        //                   OnOrderQuantity = im.OnOrderQuantity,
        //                   OnReturnQuantity = im.OnReturnQuantity,
        //                   OnTransferQuantity = im.OnTransferQuantity,
        //                   SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                   SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                   RequisitionedQuantity = im.RequisitionedQuantity,
        //                   PackingQuantity = im.PackingQuantity,
        //                   AverageUsage = im.AverageUsage,
        //                   Turns = im.Turns,
        //                   OnHandQuantity = im.OnHandQuantity,
        //                   CriticalQuantity = im.CriticalQuantity,
        //                   MinimumQuantity = im.MinimumQuantity,
        //                   MaximumQuantity = im.MaximumQuantity,
        //                   WeightPerPiece = im.WeightPerPiece,
        //                   ItemUniqueNumber = im.ItemUniqueNumber,
        //                   IsPurchase = (im.IsPurchase == true ? true : false),
        //                   IsTransfer = (im.IsTransfer == true ? true : false),
        //                   DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                   DefaultLocationName = im_bm.BinNumber,
        //                   InventoryClassification = im.InventoryClassification,
        //                   SerialNumberTracking = im.SerialNumberTracking,
        //                   LotNumberTracking = im.LotNumberTracking,
        //                   DateCodeTracking = im.DateCodeTracking,
        //                   ItemType = im.ItemType,
        //                   ImagePath = im.ImagePath,
        //                   UDF1 = im.UDF1,
        //                   UDF2 = im.UDF2,
        //                   UDF3 = im.UDF3,
        //                   UDF4 = im.UDF4,
        //                   UDF5 = im.UDF5,
        //                   GUID = im.GUID,
        //                   Created = im.Created,
        //                   Updated = im.Updated,
        //                   CreatedBy = im.CreatedBy,
        //                   LastUpdatedBy = im.LastUpdatedBy,
        //                   IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                   IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                   CompanyID = im.CompanyID,
        //                   Room = im.Room,
        //                   CreatedByName = im_UMC.UserName,
        //                   UpdatedByName = im_UMU.UserName,
        //                   RoomName = im_rm.RoomName,
        //                   IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                   CategoryName = im_CM.Category,
        //                   Unit = im_UNM.Unit,
        //                   GLAccount = im_gla.GLAccount,
        //                   IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                   IsBuildBreak = im.IsBuildBreak,
        //                   BondedInventory = im.BondedInventory,
        //                   PullQtyScanOverride = im.PullQtyScanOverride,
        //                   TrendingSetting = im.TrendingSetting,
        //                   IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                   IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                   LastCost = im.LastCost,
        //                   ItemImageExternalURL = im.ItemImageExternalURL,
        //                   ImageType = im.ImageType,
        //                   ItemDocExternalURL = im.ItemDocExternalURL,
        //                   ItemLocations = (from A in context.ItemLocationDetails
        //                                    join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                    from A_B in A_B_join.DefaultIfEmpty()
        //                                    join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                    from A_C in A_C_join.DefaultIfEmpty()
        //                                    join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                    from A_D in A_D_join.DefaultIfEmpty()
        //                                    where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                    select new ItemLocationDetailsDTO
        //                                    {
        //                                        ID = A.ID,
        //                                        BinID = A.BinID,
        //                                        CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                        ConsignedQuantity = A.ConsignedQuantity,
        //                                        MeasurementID = A.MeasurementID,
        //                                        LotNumber = A.LotNumber,
        //                                        SerialNumber = A.SerialNumber,
        //                                        ExpirationDate = A.ExpirationDate,
        //                                        ReceivedDate = A.ReceivedDate,
        //                                        Expiration = A.Expiration,
        //                                        Received = A.Received,
        //                                        Cost = A.Cost,
        //                                        GUID = A.GUID,
        //                                        ItemGUID = A.ItemGUID,
        //                                        Created = A.Created,
        //                                        Updated = A.Updated,
        //                                        CreatedBy = A.CreatedBy,
        //                                        LastUpdatedBy = A.LastUpdatedBy,
        //                                        IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                        IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                        CompanyID = A.CompanyID,
        //                                        Room = A.Room,
        //                                        CreatedByName = A_C.UserName,
        //                                        UpdatedByName = A_D.UserName,
        //                                        BinNumber = A_B.BinNumber,
        //                                        ItemNumber = im.ItemNumber,
        //                                        SerialNumberTracking = im.SerialNumberTracking,
        //                                        LotNumberTracking = im.LotNumberTracking,
        //                                        DateCodeTracking = im.DateCodeTracking,
        //                                        OrderDetailGUID = A.OrderDetailGUID,
        //                                        TransferDetailGUID = A.TransferDetailGUID,
        //                                        KitDetailGUID = A.KitDetailGUID,
        //                                        CriticalQuantity = A_B.CriticalQuantity,
        //                                        MinimumQuantity = A_B.MinimumQuantity,
        //                                        MaximumQuantity = A_B.MaximumQuantity,
        //                                        SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                    }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                   IsBOMItem = im.IsBOMItem,
        //                   RefBomId = im.RefBomId,
        //               }).SingleOrDefault();
        //    }
        //    return obj;
        //}

        //public ItemMasterDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID, Int64 SupplierID)
        //{
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO obj = null;

        //    if (SupplierID > 0)
        //    {

        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            obj = (from im in context.ItemMasters
        //                   join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //                   from im_sm in im_sm_join.DefaultIfEmpty()

        //                   join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //                   from im_mm in im_mm_join.DefaultIfEmpty()

        //                   join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //                   from im_rm in im_rm_join.DefaultIfEmpty()

        //                   join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //                   from im_UMC in im_UMC_join.DefaultIfEmpty()

        //                   join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //                   from im_UMU in im_UMU_join.DefaultIfEmpty()

        //                   join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //                   from im_CM in im_CM_join.DefaultIfEmpty()

        //                   join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //                   from im_UNM in im_UNM_join.DefaultIfEmpty()

        //                   join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //                   from im_gla in im_gla_join.DefaultIfEmpty()

        //                   join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //                   from im_bm in im_bm_join.DefaultIfEmpty()

        //                   where im.CompanyID == CompanyID && im.Room == RoomID
        //                   && im.GUID == new Guid(GUID)
        //                   && im.SupplierID == SupplierID

        //                   select new ItemMasterDTO
        //                   {
        //                       ID = im.ID,
        //                       ItemNumber = im.ItemNumber,
        //                       ManufacturerID = im.ManufacturerID,
        //                       ManufacturerNumber = im.ManufacturerNumber,
        //                       ManufacturerName = im_mm.Manufacturer,
        //                       SupplierID = im.SupplierID,
        //                       SupplierPartNo = im.SupplierPartNo,
        //                       SupplierName = im_sm.SupplierName,
        //                       UPC = im.UPC,
        //                       UNSPSC = im.UNSPSC,
        //                       Description = im.Description,
        //                       LongDescription = im.LongDescription,
        //                       CategoryID = im.CategoryID,
        //                       GLAccountID = im.GLAccountID,
        //                       UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
        //                       PricePerTerm = im.PricePerTerm,
        //                       CostUOMID = im.CostUOMID,
        //                       DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                       DefaultPullQuantity = im.DefaultPullQuantity,
        //                       Cost = im.Cost,
        //                       Markup = im.Markup,
        //                       SellPrice = im.SellPrice,
        //                       ExtendedCost = im.ExtendedCost,
        //                       AverageCost = im.AverageCost,
        //                       LeadTimeInDays = im.LeadTimeInDays,
        //                       Link1 = im.Link1,
        //                       Link2 = im.Link2,
        //                       Trend = im.Trend,
        //                       Taxable = im.Taxable,
        //                       Consignment = im.Consignment,
        //                       StagedQuantity = im.StagedQuantity,
        //                       InTransitquantity = im.InTransitquantity,
        //                       OnOrderQuantity = im.OnOrderQuantity,
        //                       OnReturnQuantity = im.OnReturnQuantity,
        //                       OnTransferQuantity = im.OnTransferQuantity,
        //                       SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                       SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                       RequisitionedQuantity = im.RequisitionedQuantity,
        //                       PackingQuantity = im.PackingQuantity,
        //                       AverageUsage = im.AverageUsage,
        //                       Turns = im.Turns,
        //                       OnHandQuantity = im.OnHandQuantity,
        //                       CriticalQuantity = im.CriticalQuantity,
        //                       MinimumQuantity = im.MinimumQuantity,
        //                       MaximumQuantity = im.MaximumQuantity,
        //                       WeightPerPiece = im.WeightPerPiece,
        //                       ItemUniqueNumber = im.ItemUniqueNumber,
        //                       IsPurchase = (im.IsPurchase == true ? true : false),
        //                       IsTransfer = (im.IsTransfer == true ? true : false),
        //                       DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                       DefaultLocationName = im_bm.BinNumber,
        //                       InventoryClassification = im.InventoryClassification,
        //                       SerialNumberTracking = im.SerialNumberTracking,
        //                       LotNumberTracking = im.LotNumberTracking,
        //                       DateCodeTracking = im.DateCodeTracking,
        //                       ItemType = im.ItemType,
        //                       ImagePath = im.ImagePath,
        //                       UDF1 = im.UDF1,
        //                       UDF2 = im.UDF2,
        //                       UDF3 = im.UDF3,
        //                       UDF4 = im.UDF4,
        //                       UDF5 = im.UDF5,
        //                       ItemUDF1 = im.UDF1,
        //                       ItemUDF2 = im.UDF2,
        //                       ItemUDF3 = im.UDF3,
        //                       ItemUDF4 = im.UDF4,
        //                       ItemUDF5 = im.UDF5,
        //                       GUID = im.GUID,
        //                       Created = im.Created,
        //                       Updated = im.Updated,
        //                       CreatedBy = im.CreatedBy,
        //                       LastUpdatedBy = im.LastUpdatedBy,
        //                       IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                       IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                       CompanyID = im.CompanyID,
        //                       Room = im.Room,
        //                       CreatedByName = im_UMC.UserName,
        //                       UpdatedByName = im_UMU.UserName,
        //                       RoomName = im_rm.RoomName,
        //                       IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                       CategoryName = im_CM.Category,
        //                       Unit = im_UNM.Unit,
        //                       GLAccount = im_gla.GLAccount,
        //                       IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                       IsBuildBreak = im.IsBuildBreak,
        //                       BondedInventory = im.BondedInventory,
        //                       PullQtyScanOverride = im.PullQtyScanOverride,
        //                       TrendingSetting = im.TrendingSetting,
        //                       IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                       IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                       ImageType = im.ImageType,
        //                       ItemImageExternalURL = im.ItemImageExternalURL,
        //                       ItemLocations = (from A in context.ItemLocationDetails
        //                                        join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                        from A_B in A_B_join.DefaultIfEmpty()
        //                                        join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                        from A_C in A_C_join.DefaultIfEmpty()
        //                                        join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                        from A_D in A_D_join.DefaultIfEmpty()
        //                                        where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                        select new ItemLocationDetailsDTO
        //                                        {
        //                                            ID = A.ID,
        //                                            BinID = A.BinID,
        //                                            CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                            ConsignedQuantity = A.ConsignedQuantity,
        //                                            MeasurementID = A.MeasurementID,
        //                                            LotNumber = A.LotNumber,
        //                                            SerialNumber = A.SerialNumber,
        //                                            ExpirationDate = A.ExpirationDate,
        //                                            ReceivedDate = A.ReceivedDate,
        //                                            Expiration = A.Expiration,
        //                                            Received = A.Received,
        //                                            Cost = A.Cost,
        //                                            GUID = A.GUID,
        //                                            ItemGUID = A.ItemGUID,
        //                                            Created = A.Created,
        //                                            Updated = A.Updated,
        //                                            CreatedBy = A.CreatedBy,
        //                                            LastUpdatedBy = A.LastUpdatedBy,
        //                                            IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                            IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                            CompanyID = A.CompanyID,
        //                                            Room = A.Room,
        //                                            CreatedByName = A_C.UserName,
        //                                            UpdatedByName = A_D.UserName,
        //                                            BinNumber = A_B.BinNumber,
        //                                            ItemNumber = im.ItemNumber,
        //                                            SerialNumberTracking = im.SerialNumberTracking,
        //                                            LotNumberTracking = im.LotNumberTracking,
        //                                            DateCodeTracking = im.DateCodeTracking,
        //                                            OrderDetailGUID = A.OrderDetailGUID,
        //                                            TransferDetailGUID = A.TransferDetailGUID,
        //                                            KitDetailGUID = A.KitDetailGUID,
        //                                            CriticalQuantity = A_B.CriticalQuantity,
        //                                            MinimumQuantity = A_B.MinimumQuantity,
        //                                            MaximumQuantity = A_B.MaximumQuantity,
        //                                            SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                        }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                       IsBOMItem = im.IsBOMItem,
        //                       RefBomId = im.RefBomId,

        //                   }).SingleOrDefault();
        //        }

        //    }
        //    else
        //    {
        //        obj = GetRecord(GUID, RoomID, CompanyID);
        //    }
        //    return obj;
        //}

        //public ItemMasterDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID)
        //{
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO obj = null;

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        obj = (from im in context.ItemMasters
        //               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //               from im_sm in im_sm_join.DefaultIfEmpty()

        //               join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //               from im_mm in im_mm_join.DefaultIfEmpty()

        //               join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //               from im_rm in im_rm_join.DefaultIfEmpty()

        //               join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //               from im_UMC in im_UMC_join.DefaultIfEmpty()

        //               join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //               from im_UMU in im_UMU_join.DefaultIfEmpty()

        //               join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //               from im_CM in im_CM_join.DefaultIfEmpty()

        //               join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //               from im_UNM in im_UNM_join.DefaultIfEmpty()

        //               join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //               from im_gla in im_gla_join.DefaultIfEmpty()

        //               join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //               from im_bm in im_bm_join.DefaultIfEmpty()

        //               join CUOM in context.CostUOMMasters on im.CostUOMID equals CUOM.ID into im_CUOM_join
        //               from im_CUOM in im_CUOM_join.DefaultIfEmpty()

        //               where im.CompanyID == CompanyID && im.Room == RoomID
        //               && im.GUID == new Guid(GUID)

        //               select new ItemMasterDTO
        //               {
        //                   ID = im.ID,
        //                   ItemNumber = im.ItemNumber,
        //                   ManufacturerID = im.ManufacturerID,
        //                   ManufacturerNumber = im.ManufacturerNumber,
        //                   ManufacturerName = im_mm.Manufacturer,
        //                   SupplierID = im.SupplierID,
        //                   SupplierPartNo = im.SupplierPartNo,
        //                   SupplierName = im_sm.SupplierName,
        //                   UPC = im.UPC,
        //                   UNSPSC = im.UNSPSC,
        //                   Description = im.Description,
        //                   LongDescription = im.LongDescription,
        //                   CategoryID = im.CategoryID,
        //                   GLAccountID = im.GLAccountID,
        //                   UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
        //                   PricePerTerm = im.PricePerTerm,
        //                   CostUOMID = im.CostUOMID,
        //                   DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                   DefaultPullQuantity = im.DefaultPullQuantity,
        //                   Cost = im.Cost,
        //                   Markup = im.Markup,
        //                   SellPrice = im.SellPrice,
        //                   ExtendedCost = im.ExtendedCost,
        //                   AverageCost = im.AverageCost,
        //                   LeadTimeInDays = im.LeadTimeInDays,
        //                   Link1 = im.Link1,
        //                   Link2 = im.Link2,
        //                   Trend = im.Trend,
        //                   Taxable = im.Taxable,
        //                   Consignment = im.Consignment,
        //                   StagedQuantity = im.StagedQuantity,
        //                   InTransitquantity = im.InTransitquantity,
        //                   OnOrderQuantity = im.OnOrderQuantity,
        //                   OnReturnQuantity = im.OnReturnQuantity,
        //                   OnTransferQuantity = im.OnTransferQuantity,
        //                   SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                   SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                   RequisitionedQuantity = im.RequisitionedQuantity,
        //                   PackingQuantity = im.PackingQuantity,
        //                   AverageUsage = im.AverageUsage,
        //                   Turns = im.Turns,
        //                   OnHandQuantity = im.OnHandQuantity,
        //                   CriticalQuantity = im.CriticalQuantity,
        //                   MinimumQuantity = im.MinimumQuantity,
        //                   MaximumQuantity = im.MaximumQuantity,
        //                   WeightPerPiece = im.WeightPerPiece,
        //                   ItemUniqueNumber = im.ItemUniqueNumber,
        //                   IsPurchase = (im.IsPurchase == true ? true : false),
        //                   IsTransfer = (im.IsTransfer == true ? true : false),
        //                   DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                   DefaultLocationName = im_bm.BinNumber,
        //                   InventoryClassification = im.InventoryClassification,
        //                   SerialNumberTracking = im.SerialNumberTracking,
        //                   LotNumberTracking = im.LotNumberTracking,
        //                   DateCodeTracking = im.DateCodeTracking,
        //                   ItemType = im.ItemType,
        //                   ImagePath = im.ImagePath,
        //                   UDF1 = im.UDF1,
        //                   UDF2 = im.UDF2,
        //                   UDF3 = im.UDF3,
        //                   UDF4 = im.UDF4,
        //                   UDF5 = im.UDF5,
        //                   ItemUDF1 = im.UDF1,
        //                   ItemUDF2 = im.UDF2,
        //                   ItemUDF3 = im.UDF3,
        //                   ItemUDF4 = im.UDF4,
        //                   ItemUDF5 = im.UDF5,
        //                   GUID = im.GUID,
        //                   Created = im.Created,
        //                   Updated = im.Updated,
        //                   CreatedBy = im.CreatedBy,
        //                   LastUpdatedBy = im.LastUpdatedBy,
        //                   IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                   IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                   CompanyID = im.CompanyID,
        //                   Room = im.Room,
        //                   CreatedByName = im_UMC.UserName,
        //                   UpdatedByName = im_UMU.UserName,
        //                   RoomName = im_rm.RoomName,
        //                   IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                   CategoryName = im_CM.Category,
        //                   Unit = im_UNM.Unit,
        //                   GLAccount = im_gla.GLAccount,
        //                   IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                   IsBuildBreak = im.IsBuildBreak,
        //                   BondedInventory = im.BondedInventory,
        //                   PullQtyScanOverride = im.PullQtyScanOverride,
        //                   TrendingSetting = im.TrendingSetting,
        //                   IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                   IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                   IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
        //                   LastCost = im.LastCost,
        //                   AddedFrom = im.AddedFrom,
        //                   EditedFrom = im.EditedFrom,
        //                   ReceivedOn = im.ReceivedOn,
        //                   ReceivedOnWeb = im.ReceivedOnWeb,
        //                   ItemImageExternalURL = im.ItemImageExternalURL,
        //                   ImageType = im.ImageType,
        //                   CostUOMName = im_CUOM.CostUOM,
        //                   ItemLocations = (from A in context.ItemLocationDetails
        //                                    join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                    from A_B in A_B_join.DefaultIfEmpty()
        //                                    join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                    from A_C in A_C_join.DefaultIfEmpty()
        //                                    join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                    from A_D in A_D_join.DefaultIfEmpty()
        //                                    where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                    select new ItemLocationDetailsDTO
        //                                    {
        //                                        ID = A.ID,
        //                                        BinID = A.BinID,
        //                                        CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                        ConsignedQuantity = A.ConsignedQuantity,
        //                                        MeasurementID = A.MeasurementID,
        //                                        LotNumber = A.LotNumber,
        //                                        SerialNumber = A.SerialNumber,
        //                                        ExpirationDate = A.ExpirationDate,
        //                                        ReceivedDate = A.ReceivedDate,
        //                                        Expiration = A.Expiration,
        //                                        Received = A.Received,
        //                                        Cost = A.Cost,
        //                                        GUID = A.GUID,
        //                                        ItemGUID = A.ItemGUID,
        //                                        Created = A.Created,
        //                                        Updated = A.Updated,
        //                                        CreatedBy = A.CreatedBy,
        //                                        LastUpdatedBy = A.LastUpdatedBy,
        //                                        IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                        IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                        CompanyID = A.CompanyID,
        //                                        Room = A.Room,
        //                                        CreatedByName = A_C.UserName,
        //                                        UpdatedByName = A_D.UserName,
        //                                        BinNumber = A_B.BinNumber,
        //                                        ItemNumber = im.ItemNumber,
        //                                        SerialNumberTracking = im.SerialNumberTracking,
        //                                        LotNumberTracking = im.LotNumberTracking,
        //                                        DateCodeTracking = im.DateCodeTracking,
        //                                        OrderDetailGUID = A.OrderDetailGUID,
        //                                        TransferDetailGUID = A.TransferDetailGUID,
        //                                        KitDetailGUID = A.KitDetailGUID,
        //                                        CriticalQuantity = A_B.CriticalQuantity,
        //                                        MinimumQuantity = A_B.MinimumQuantity,
        //                                        MaximumQuantity = A_B.MaximumQuantity,
        //                                        SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                    }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                   IsBOMItem = im.IsBOMItem,
        //                   RefBomId = im.RefBomId,

        //               }).SingleOrDefault();


        //        //var sql = ((System.Data.Objects.ObjectQuery)query).ToTraceString();
        //    }

        //    return obj;
        //}

        //public ItemMasterDTO GetRecordDB(string GUID, Int64 RoomID, Int64 CompanyID)
        //{
        //    ItemMasterDTO obj = null;
        //    Guid ItemGUID = Guid.Empty;
        //    if (Guid.TryParse(GUID, out ItemGUID))
        //    {
        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            return (from im in context.GetItemByID(null, ItemGUID, RoomID, CompanyID)
        //                    select new ItemMasterDTO
        //                    {
        //                        ID = im.ID,
        //                        ItemNumber = im.ItemNumber,
        //                        ManufacturerID = im.ManufacturerID,
        //                        ManufacturerNumber = im.ManufacturerNumber,
        //                        ManufacturerName = im.Manufacturer,
        //                        SupplierID = im.SupplierID,
        //                        SupplierPartNo = im.SupplierPartNo,
        //                        SupplierName = im.SupplierName,
        //                        UPC = im.UPC,
        //                        UNSPSC = im.UNSPSC,
        //                        Description = im.Description,
        //                        LongDescription = im.LongDescription,
        //                        CategoryID = im.CategoryID,
        //                        GLAccountID = im.GLAccountID,
        //                        UOMID = im.UOMID ?? 0,
        //                        PricePerTerm = im.PricePerTerm,
        //                        CostUOMID = im.CostUOMID,
        //                        DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                        DefaultPullQuantity = im.DefaultPullQuantity,
        //                        Cost = im.Cost,
        //                        Markup = im.Markup,
        //                        SellPrice = im.SellPrice,
        //                        ExtendedCost = im.ExtendedCost,
        //                        AverageCost = im.AverageCost,
        //                        LeadTimeInDays = im.LeadTimeInDays,
        //                        Link1 = im.Link1,
        //                        Link2 = im.Link2,
        //                        Trend = im.Trend,
        //                        Taxable = im.Taxable,
        //                        Consignment = im.Consignment,
        //                        StagedQuantity = im.StagedQuantity,
        //                        InTransitquantity = im.InTransitquantity,
        //                        OnOrderQuantity = im.OnOrderQuantity,
        //                        OnReturnQuantity = im.OnReturnQuantity,
        //                        OnTransferQuantity = im.OnTransferQuantity,
        //                        SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                        SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                        RequisitionedQuantity = im.RequisitionedQuantity,
        //                        PackingQuantity = im.PackingQuantity,
        //                        AverageUsage = im.AverageUsage,
        //                        Turns = im.Turns,
        //                        OnHandQuantity = im.OnHandQuantity,
        //                        CriticalQuantity = im.CriticalQuantity,
        //                        MinimumQuantity = im.MinimumQuantity,
        //                        MaximumQuantity = im.MaximumQuantity,
        //                        WeightPerPiece = im.WeightPerPiece,
        //                        ItemUniqueNumber = im.ItemUniqueNumber,
        //                        IsPurchase = im.IsPurchase ?? false,
        //                        IsTransfer = im.IsTransfer ?? false,
        //                        DefaultLocation = im.DefaultLocation ?? 0,
        //                        DefaultLocationName = im.BinNumber,
        //                        InventoryClassification = im.InventoryClassification,
        //                        SerialNumberTracking = im.SerialNumberTracking,
        //                        LotNumberTracking = im.LotNumberTracking,
        //                        DateCodeTracking = im.DateCodeTracking,
        //                        ItemType = im.ItemType,
        //                        ImagePath = im.ImagePath,
        //                        UDF1 = im.UDF1,
        //                        UDF2 = im.UDF2,
        //                        UDF3 = im.UDF3,
        //                        UDF4 = im.UDF4,
        //                        UDF5 = im.UDF5,
        //                        ItemUDF1 = im.UDF1,
        //                        ItemUDF2 = im.UDF2,
        //                        ItemUDF3 = im.UDF3,
        //                        ItemUDF4 = im.UDF4,
        //                        ItemUDF5 = im.UDF5,
        //                        GUID = im.GUID,
        //                        Created = im.Created,
        //                        Updated = im.Updated,
        //                        CreatedBy = im.CreatedBy,
        //                        LastUpdatedBy = im.LastUpdatedBy,
        //                        IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                        IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                        CompanyID = im.CompanyID,
        //                        Room = im.Room,
        //                        CreatedByName = im.CreatedByName,
        //                        UpdatedByName = im.UpdatedByName,
        //                        RoomName = im.RoomName,
        //                        IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                        CategoryName = im.Category,
        //                        Unit = im.Unit,
        //                        GLAccount = im.GLAccount,
        //                        IsItemLevelMinMaxQtyRequired = im.IsItemLevelMinMaxQtyRequired ?? false,
        //                        IsBuildBreak = im.IsBuildBreak,
        //                        BondedInventory = im.BondedInventory,
        //                        PullQtyScanOverride = im.PullQtyScanOverride,
        //                        TrendingSetting = im.TrendingSetting,
        //                        IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                        IsEnforceDefaultReorderQuantity = im.IsEnforceDefaultReorderQuantity ?? false,
        //                        IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
        //                        LastCost = im.LastCost,
        //                        AddedFrom = im.AddedFrom,
        //                        EditedFrom = im.EditedFrom,
        //                        ReceivedOn = im.ReceivedOn,
        //                        ReceivedOnWeb = im.ReceivedOnWeb,
        //                        ItemImageExternalURL = im.ItemImageExternalURL,
        //                        ImageType = im.ImageType,
        //                        CostUOMName = im.CostUOM,
        //                        IsBOMItem = im.IsBOMItem,
        //                        RefBomId = im.RefBomId,
        //                        ItemLocations = (from A in context.GetItemLocationDetailsByItemGUID(im.GUID)
        //                                         select new ItemLocationDetailsDTO
        //                                         {
        //                                             ID = A.ID,
        //                                             BinID = A.BinID,
        //                                             CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                             ConsignedQuantity = A.ConsignedQuantity,
        //                                             MeasurementID = A.MeasurementID,
        //                                             LotNumber = A.LotNumber,
        //                                             SerialNumber = A.SerialNumber,
        //                                             ExpirationDate = A.ExpirationDate,
        //                                             ReceivedDate = A.ReceivedDate,
        //                                             Expiration = A.Expiration,
        //                                             Received = A.Received,
        //                                             Cost = A.Cost,
        //                                             GUID = A.GUID,
        //                                             ItemGUID = A.ItemGUID,
        //                                             Created = A.Created,
        //                                             Updated = A.Updated,
        //                                             CreatedBy = A.CreatedBy,
        //                                             LastUpdatedBy = A.LastUpdatedBy,
        //                                             IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                             IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                             CompanyID = A.CompanyID,
        //                                             Room = A.Room,
        //                                             CreatedByName = A.CreatedByName,
        //                                             UpdatedByName = A.UpdatedByName,
        //                                             BinNumber = A.BinNumber,
        //                                             ItemNumber = A.itemNumber,
        //                                             SerialNumberTracking = im.SerialNumberTracking,
        //                                             LotNumberTracking = im.LotNumberTracking,
        //                                             DateCodeTracking = im.DateCodeTracking,
        //                                             OrderDetailGUID = A.OrderDetailGUID,
        //                                             TransferDetailGUID = A.TransferDetailGUID,
        //                                             KitDetailGUID = A.KitDetailGUID,
        //                                             CriticalQuantity = A.CriticalQuantity,
        //                                             MinimumQuantity = A.MinimumQuantity,
        //                                             MaximumQuantity = A.MaximumQuantity,
        //                                             SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => (t.Quantity ?? 0)).Sum(),
        //                                         }).AsEnumerable<ItemLocationDetailsDTO>(),

        //                    }).FirstOrDefault();
        //        }
        //    }
        //    else
        //    {
        //        return obj;
        //    }
        //}
        //public bool updateZipName(long Id, string updateZipName)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.ID == Id);
        //        if (objItemMaster != null)
        //        {
        //            objItemMaster.ZipfileName = updateZipName;
        //            context.SaveChanges();
        //        }
        //    }
        //    return true;
        //}
        //public List<ItemMasterDTO> GetAllRecords(bool IsBuildBreak, Int32 ItemType, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 SupplierID = 0)
        //{
        //    List<ItemMasterDTO> ObjCache;
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);

        //    BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
        //    IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);
        //    #region "Conditional"
        //    if (SupplierID > 0)
        //    {
        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            ObjCache = (from im in context.ItemMasters
        //                        join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //                        from im_sm in im_sm_join.DefaultIfEmpty()

        //                        join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //                        from im_mm in im_mm_join.DefaultIfEmpty()

        //                        join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //                        from im_rm in im_rm_join.DefaultIfEmpty()

        //                        join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
        //                        from im_cuomm in im_cuomm_join.DefaultIfEmpty()

        //                        join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //                        from im_UMC in im_UMC_join.DefaultIfEmpty()

        //                        join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //                        from im_UMU in im_UMU_join.DefaultIfEmpty()

        //                        join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //                        from im_CM in im_CM_join.DefaultIfEmpty()

        //                        join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //                        from im_UNM in im_UNM_join.DefaultIfEmpty()

        //                        join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //                        from im_gla in im_gla_join.DefaultIfEmpty()

        //                        join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
        //                        from im_icm in im_icm_join.DefaultIfEmpty()

        //                        join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //                        from im_bm in im_bm_join.DefaultIfEmpty()

        //                        where im.CompanyID == CompanyID && im.Room == RoomID && (im.IsBuildBreak ?? false) == true
        //                        && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived
        //                        && im.SupplierID == SupplierID

        //                        select new ItemMasterDTO
        //                        {
        //                            ID = im.ID,
        //                            ItemNumber = im.ItemNumber,
        //                            ManufacturerID = im.ManufacturerID,
        //                            ManufacturerNumber = im.ManufacturerNumber,
        //                            ManufacturerName = im_mm.Manufacturer,
        //                            SupplierID = im.SupplierID,
        //                            SupplierPartNo = im.SupplierPartNo,
        //                            SupplierName = im_sm.SupplierName,
        //                            UPC = im.UPC,
        //                            UNSPSC = im.UNSPSC,
        //                            Description = im.Description,
        //                            LongDescription = im.LongDescription,
        //                            CategoryID = im.CategoryID,
        //                            GLAccountID = im.GLAccountID,
        //                            UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
        //                            PricePerTerm = im.PricePerTerm,
        //                            CostUOMID = im.CostUOMID,
        //                            DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                            DefaultPullQuantity = im.DefaultPullQuantity,
        //                            Cost = im.Cost,
        //                            Markup = im.Markup,
        //                            SellPrice = im.SellPrice,
        //                            ExtendedCost = im.ExtendedCost,
        //                            AverageCost = im.AverageCost,
        //                            LeadTimeInDays = im.LeadTimeInDays,
        //                            Link1 = im.Link1,
        //                            Link2 = im.Link2,
        //                            Trend = im.Trend,
        //                            Taxable = im.Taxable,
        //                            Consignment = im.Consignment,
        //                            StagedQuantity = im.StagedQuantity,
        //                            InTransitquantity = im.InTransitquantity,
        //                            OnOrderQuantity = im.OnOrderQuantity,
        //                            OnReturnQuantity = im.OnReturnQuantity,
        //                            OnTransferQuantity = im.OnTransferQuantity,
        //                            SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                            SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                            RequisitionedQuantity = im.RequisitionedQuantity,
        //                            PackingQuantity = im.PackingQuantity,
        //                            AverageUsage = im.AverageUsage,
        //                            Turns = im.Turns,
        //                            OnHandQuantity = im.OnHandQuantity,
        //                            CriticalQuantity = im.CriticalQuantity,
        //                            MinimumQuantity = im.MinimumQuantity,
        //                            MaximumQuantity = im.MaximumQuantity,
        //                            WeightPerPiece = im.WeightPerPiece,
        //                            ItemUniqueNumber = im.ItemUniqueNumber,
        //                            IsPurchase = (im.IsPurchase == true ? true : false),
        //                            IsTransfer = (im.IsTransfer == true ? true : false),
        //                            DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                            DefaultLocationName = im_bm.BinNumber,
        //                            InventoryClassification = im.InventoryClassification,
        //                            SerialNumberTracking = im.SerialNumberTracking,
        //                            LotNumberTracking = im.LotNumberTracking,
        //                            DateCodeTracking = im.DateCodeTracking,
        //                            ItemType = im.ItemType,
        //                            ImagePath = im.ImagePath,
        //                            UDF1 = im.UDF1,
        //                            UDF2 = im.UDF2,
        //                            UDF3 = im.UDF3,
        //                            UDF4 = im.UDF4,
        //                            UDF5 = im.UDF5,
        //                            ItemUDF1 = im.UDF1,
        //                            ItemUDF2 = im.UDF2,
        //                            ItemUDF3 = im.UDF3,
        //                            ItemUDF4 = im.UDF4,
        //                            ItemUDF5 = im.UDF5,
        //                            GUID = im.GUID,
        //                            Created = im.Created,
        //                            Updated = im.Updated,
        //                            CreatedBy = im.CreatedBy,
        //                            LastUpdatedBy = im.LastUpdatedBy,
        //                            IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                            IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                            CompanyID = im.CompanyID,
        //                            Room = im.Room,
        //                            CreatedByName = im_UMC.UserName,
        //                            UpdatedByName = im_UMU.UserName,
        //                            RoomName = im_rm.RoomName,
        //                            IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                            CategoryName = im_CM.Category,
        //                            Unit = im_UNM.Unit,
        //                            GLAccount = im_gla.GLAccount,
        //                            IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                            IsBuildBreak = im.IsBuildBreak,
        //                            BondedInventory = im.BondedInventory,
        //                            IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                            PullQtyScanOverride = im.PullQtyScanOverride,
        //                            TrendingSetting = im.TrendingSetting,
        //                            IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                            ItemImageExternalURL = im.ItemImageExternalURL,
        //                            LastCost = im.LastCost,
        //                            ItemLocations = (from A in context.ItemLocationDetails
        //                                             join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                             from A_B in A_B_join.DefaultIfEmpty()
        //                                             join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                             from A_C in A_C_join.DefaultIfEmpty()
        //                                             join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                             from A_D in A_D_join.DefaultIfEmpty()
        //                                             where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                             select new ItemLocationDetailsDTO
        //                                             {
        //                                                 ID = A.ID,
        //                                                 BinID = A.BinID,
        //                                                 CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                                 ConsignedQuantity = A.ConsignedQuantity,
        //                                                 MeasurementID = A.MeasurementID,
        //                                                 LotNumber = A.LotNumber,
        //                                                 SerialNumber = A.SerialNumber,
        //                                                 ExpirationDate = A.ExpirationDate,
        //                                                 ReceivedDate = A.ReceivedDate,
        //                                                 Expiration = A.Expiration,
        //                                                 Received = A.Received,
        //                                                 Cost = A.Cost,
        //                                                 GUID = A.GUID,
        //                                                 ItemGUID = A.ItemGUID,
        //                                                 Created = A.Created,
        //                                                 Updated = A.Updated,
        //                                                 CreatedBy = A.CreatedBy,
        //                                                 LastUpdatedBy = A.LastUpdatedBy,
        //                                                 IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                                 IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                                 CompanyID = A.CompanyID,
        //                                                 Room = A.Room,
        //                                                 CreatedByName = A_C.UserName,
        //                                                 UpdatedByName = A_D.UserName,
        //                                                 BinNumber = A_B.BinNumber,
        //                                                 ItemNumber = im.ItemNumber,
        //                                                 SerialNumberTracking = im.SerialNumberTracking,
        //                                                 LotNumberTracking = im.LotNumberTracking,
        //                                                 DateCodeTracking = im.DateCodeTracking,
        //                                                 OrderDetailGUID = A.OrderDetailGUID,
        //                                                 TransferDetailGUID = A.TransferDetailGUID,
        //                                                 KitDetailGUID = A.KitDetailGUID,
        //                                                 CriticalQuantity = A_B.CriticalQuantity,
        //                                                 MinimumQuantity = A_B.MinimumQuantity,
        //                                                 MaximumQuantity = A_B.MaximumQuantity,
        //                                                 SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                             }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                            //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, im.GUID, "Item Master"),
        //                            InventoryClassificationName = im_icm.InventoryClassification,
        //                            IsBOMItem = im.IsBOMItem,
        //                            RefBomId = im.RefBomId,
        //                            CostUOMName = im_cuomm.CostUOM,
        //                            ReceivedOn = im.ReceivedOn,
        //                            ReceivedOnWeb = im.ReceivedOnWeb,
        //                            AddedFrom = im.AddedFrom,
        //                            EditedFrom = im.EditedFrom,
        //                            IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive
        //                        }).AsParallel().ToList();
        //        }
        //    }
        //    else
        //    {

        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {

        //            ObjCache = (from im in context.ItemMasters
        //                        join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //                        from im_sm in im_sm_join.DefaultIfEmpty()

        //                        join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //                        from im_mm in im_mm_join.DefaultIfEmpty()

        //                        join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //                        from im_rm in im_rm_join.DefaultIfEmpty()

        //                        join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
        //                        from im_cuomm in im_cuomm_join.DefaultIfEmpty()

        //                        join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //                        from im_UMC in im_UMC_join.DefaultIfEmpty()

        //                        join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //                        from im_UMU in im_UMU_join.DefaultIfEmpty()

        //                        join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //                        from im_CM in im_CM_join.DefaultIfEmpty()

        //                        join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //                        from im_UNM in im_UNM_join.DefaultIfEmpty()

        //                        join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //                        from im_gla in im_gla_join.DefaultIfEmpty()

        //                        join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
        //                        from im_icm in im_icm_join.DefaultIfEmpty()

        //                        join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //                        from im_bm in im_bm_join.DefaultIfEmpty()

        //                        where im.CompanyID == CompanyID && im.Room == RoomID && (im.IsBuildBreak ?? false) == true
        //                        && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived


        //                        select new ItemMasterDTO
        //                        {
        //                            ID = im.ID,
        //                            ItemNumber = im.ItemNumber,
        //                            ManufacturerID = im.ManufacturerID,
        //                            ManufacturerNumber = im.ManufacturerNumber,
        //                            ManufacturerName = im_mm.Manufacturer,
        //                            SupplierID = im.SupplierID,
        //                            SupplierPartNo = im.SupplierPartNo,
        //                            SupplierName = im_sm.SupplierName,
        //                            UPC = im.UPC,
        //                            UNSPSC = im.UNSPSC,
        //                            Description = im.Description,
        //                            LongDescription = im.LongDescription,
        //                            CategoryID = im.CategoryID,
        //                            GLAccountID = im.GLAccountID,
        //                            UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
        //                            PricePerTerm = im.PricePerTerm,
        //                            CostUOMID = im.CostUOMID,
        //                            DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                            DefaultPullQuantity = im.DefaultPullQuantity,
        //                            Cost = im.Cost,
        //                            Markup = im.Markup,
        //                            SellPrice = im.SellPrice,
        //                            ExtendedCost = im.ExtendedCost,
        //                            AverageCost = im.AverageCost,
        //                            LeadTimeInDays = im.LeadTimeInDays,
        //                            Link1 = im.Link1,
        //                            Link2 = im.Link2,
        //                            Trend = im.Trend,
        //                            Taxable = im.Taxable,
        //                            Consignment = im.Consignment,
        //                            StagedQuantity = im.StagedQuantity,
        //                            InTransitquantity = im.InTransitquantity,
        //                            OnOrderQuantity = im.OnOrderQuantity,
        //                            OnReturnQuantity = im.OnReturnQuantity,
        //                            OnTransferQuantity = im.OnTransferQuantity,
        //                            SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                            SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                            RequisitionedQuantity = im.RequisitionedQuantity,
        //                            PackingQuantity = im.PackingQuantity,
        //                            AverageUsage = im.AverageUsage,
        //                            Turns = im.Turns,
        //                            OnHandQuantity = im.OnHandQuantity,
        //                            CriticalQuantity = im.CriticalQuantity,
        //                            MinimumQuantity = im.MinimumQuantity,
        //                            MaximumQuantity = im.MaximumQuantity,
        //                            WeightPerPiece = im.WeightPerPiece,
        //                            ItemUniqueNumber = im.ItemUniqueNumber,
        //                            IsPurchase = (im.IsPurchase == true ? true : false),
        //                            IsTransfer = (im.IsTransfer == true ? true : false),
        //                            DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                            DefaultLocationName = im_bm.BinNumber,
        //                            InventoryClassification = im.InventoryClassification,
        //                            SerialNumberTracking = im.SerialNumberTracking,
        //                            LotNumberTracking = im.LotNumberTracking,
        //                            DateCodeTracking = im.DateCodeTracking,
        //                            ItemType = im.ItemType,
        //                            ImagePath = im.ImagePath,
        //                            UDF1 = im.UDF1,
        //                            UDF2 = im.UDF2,
        //                            UDF3 = im.UDF3,
        //                            UDF4 = im.UDF4,
        //                            UDF5 = im.UDF5,
        //                            ItemUDF1 = im.UDF1,
        //                            ItemUDF2 = im.UDF2,
        //                            ItemUDF3 = im.UDF3,
        //                            ItemUDF4 = im.UDF4,
        //                            ItemUDF5 = im.UDF5,
        //                            GUID = im.GUID,
        //                            Created = im.Created,
        //                            Updated = im.Updated,
        //                            CreatedBy = im.CreatedBy,
        //                            LastUpdatedBy = im.LastUpdatedBy,
        //                            IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                            IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                            CompanyID = im.CompanyID,
        //                            Room = im.Room,
        //                            CreatedByName = im_UMC.UserName,
        //                            UpdatedByName = im_UMU.UserName,
        //                            RoomName = im_rm.RoomName,
        //                            IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                            CategoryName = im_CM.Category,
        //                            Unit = im_UNM.Unit,
        //                            GLAccount = im_gla.GLAccount,
        //                            IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                            IsBuildBreak = im.IsBuildBreak,
        //                            BondedInventory = im.BondedInventory,
        //                            PullQtyScanOverride = im.PullQtyScanOverride,
        //                            TrendingSetting = im.TrendingSetting,
        //                            IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                            IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                            LastCost = im.LastCost,
        //                            ItemLocations = (from A in context.ItemLocationDetails
        //                                             join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                             from A_B in A_B_join.DefaultIfEmpty()
        //                                             join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                             from A_C in A_C_join.DefaultIfEmpty()
        //                                             join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                             from A_D in A_D_join.DefaultIfEmpty()
        //                                             where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                             select new ItemLocationDetailsDTO
        //                                             {
        //                                                 ID = A.ID,
        //                                                 BinID = A.BinID,
        //                                                 CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                                 ConsignedQuantity = A.ConsignedQuantity,
        //                                                 MeasurementID = A.MeasurementID,
        //                                                 LotNumber = A.LotNumber,
        //                                                 SerialNumber = A.SerialNumber,
        //                                                 ExpirationDate = A.ExpirationDate,
        //                                                 ReceivedDate = A.ReceivedDate,
        //                                                 Expiration = A.Expiration,
        //                                                 Received = A.Received,
        //                                                 Cost = A.Cost,
        //                                                 GUID = A.GUID,
        //                                                 ItemGUID = A.ItemGUID,
        //                                                 Created = A.Created,
        //                                                 Updated = A.Updated,
        //                                                 CreatedBy = A.CreatedBy,
        //                                                 LastUpdatedBy = A.LastUpdatedBy,
        //                                                 IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                                 IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                                 CompanyID = A.CompanyID,
        //                                                 Room = A.Room,
        //                                                 CreatedByName = A_C.UserName,
        //                                                 UpdatedByName = A_D.UserName,
        //                                                 BinNumber = A_B.BinNumber,
        //                                                 ItemNumber = im.ItemNumber,
        //                                                 SerialNumberTracking = im.SerialNumberTracking,
        //                                                 LotNumberTracking = im.LotNumberTracking,
        //                                                 DateCodeTracking = im.DateCodeTracking,
        //                                                 OrderDetailGUID = A.OrderDetailGUID,
        //                                                 TransferDetailGUID = A.TransferDetailGUID,
        //                                                 KitDetailGUID = A.KitDetailGUID,
        //                                                 CriticalQuantity = A_B.CriticalQuantity,
        //                                                 MinimumQuantity = A_B.MinimumQuantity,
        //                                                 MaximumQuantity = A_B.MaximumQuantity,
        //                                                 SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                             }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                            // AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, im.GUID, "Item Master"),
        //                            InventoryClassificationName = im_icm.InventoryClassification,
        //                            IsBOMItem = im.IsBOMItem,
        //                            RefBomId = im.RefBomId,
        //                            CostUOMName = im_cuomm.CostUOM,
        //                            ReceivedOn = im.ReceivedOn,
        //                            ReceivedOnWeb = im.ReceivedOnWeb,
        //                            AddedFrom = im.AddedFrom,
        //                            EditedFrom = im.EditedFrom,
        //                            IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive
        //                        }).AsParallel().ToList();
        //        }
        //    }
        //    #endregion
        //    return ObjCache;
        //}
        //public double GetSuggestedOrderQtyByBinID(Guid ItemGUIDId, long? BinID)
        //{
        //    double TotalSuggestedQty = 0;
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        if (context.CartItems.Any(t => t.ItemGUID == ItemGUIDId && t.BinId == BinID && t.IsDeleted == false && t.IsArchived == false && t.ReplenishType == "Purchase"))
        //        {
        //            TotalSuggestedQty = context.CartItems.Where(t => t.ItemGUID == ItemGUIDId && t.BinId == BinID && t.ReplenishType == "Purchase" && t.IsDeleted == false && t.IsArchived == false).Sum(t => (t.Quantity ?? 0));
        //        }
        //        else
        //        {
        //            TotalSuggestedQty = 0;
        //        }
        //    }
        //    return TotalSuggestedQty;

        //}
        //public IEnumerable<ItemMasterDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUIDs, Int64 SupplierID)
        //{
        //    List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
        //    TotalCount = 0;
        //    ItemMasterDTO objItemDTO = new ItemMasterDTO();

        //    string ItemSuppliers = null;
        //    string Manufacturers = null;
        //    string CreatedByName = null;
        //    string UpdatedByName = null;
        //    string ItemCategory = null;
        //    string Cost = null;
        //    string Cost1 = null;
        //    string ItemType = null;
        //    string CreatedDateFrom = null;
        //    string CreatedDateTo = null;
        //    string UpdatedDateFrom = null;
        //    string UpdatedDateTo = null;
        //    string UDF1 = null;
        //    string UDF2 = null;
        //    string UDF3 = null;
        //    string UDF4 = null;
        //    string UDF5 = null;
        //    string OnHandQuantity = null;

        //    DataSet dsCart = new DataSet();
        //    //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
        //    string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

        //    if (Connectionstring == "")
        //    {
        //        return lstItems;
        //    }
        //    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {
        //        dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, SupplierID, ItemGUIDs, null, null);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
        //        string[] FieldsPara = Fields[1].Split('@');
        //        if (Fields.Length > 2)
        //        {
        //            if (!string.IsNullOrEmpty(Fields[2]))
        //            {
        //                SearchTerm = Fields[2];
        //            }
        //            else
        //            {
        //                SearchTerm = string.Empty;
        //            }
        //        }
        //        else
        //        {
        //            SearchTerm = string.Empty;
        //        }

        //        if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
        //        {
        //            ItemSuppliers = FieldsPara[9].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
        //        {
        //            Manufacturers = FieldsPara[10].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
        //        {
        //            ItemCategory = FieldsPara[11].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
        //        {
        //            ItemType = FieldsPara[22].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
        //        {
        //            UpdatedByName = FieldsPara[1].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
        //        {
        //            CreatedByName = FieldsPara[0].TrimEnd(',');
        //        }

        //        if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
        //        {
        //            if (FieldsPara[15].Contains("100_1000"))
        //            {
        //                Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
        //            }
        //            else if (FieldsPara[15].Contains("10_1000"))
        //            {
        //                Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
        //            }
        //            else
        //            {
        //                Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
        //                Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
        //            }
        //            //  Cost = FieldsPara[15].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
        //        {
        //            CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
        //            CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
        //        {
        //            UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
        //            UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
        //        }

        //        if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[4].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF1 = UDF1 + supitem + "','";
        //            }
        //            UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[5].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF2 = UDF2 + supitem + "','";
        //            }
        //            UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[6].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF3 = UDF3 + supitem + "','";
        //            }
        //            UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[7].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF4 = UDF4 + supitem + "','";
        //            }
        //            UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[8].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF5 = UDF5 + supitem + "','";
        //            }
        //            UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
        //        {
        //            if (FieldsPara[21] == "1")
        //            {
        //                OnHandQuantity = ("Out of Stock");
        //            }
        //            else if (FieldsPara[21] == "2")
        //            {
        //                OnHandQuantity = ("Below Critical");
        //            }
        //            else if (FieldsPara[21] == "3")
        //            {
        //                OnHandQuantity = ("Below Min");
        //            }
        //            else if (FieldsPara[21] == "4")
        //            {
        //                OnHandQuantity = ("Above Max");
        //            }
        //        }

        //        dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, SupplierID, ItemGUIDs, null, null, OnHandQuantity);
        //    }
        //    else
        //    {
        //        dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, SupplierID, ItemGUIDs, null, null, OnHandQuantity);
        //    }
        //    if (dsCart != null && dsCart.Tables.Count > 0)
        //    {
        //        DataTable dtCart = dsCart.Tables[0];
        //        if (dtCart.Rows.Count > 0)
        //        {
        //            TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
        //            lstItems = dtCart.AsEnumerable()
        //            .Select(row => new ItemMasterDTO
        //            {
        //                ID = row.Field<long>("ID"),
        //                ItemNumber = row.Field<string>("ItemNumber"),
        //                ManufacturerID = row.Field<long?>("ManufacturerID"),
        //                ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
        //                ManufacturerName = row.Field<string>("ManufacturerName"),
        //                SupplierID = row.Field<long?>("SupplierID"),
        //                SupplierPartNo = row.Field<string>("SupplierPartNo"),
        //                SupplierName = row.Field<string>("SupplierName"),
        //                UPC = row.Field<string>("UPC"),
        //                UNSPSC = row.Field<string>("UNSPSC"),
        //                Description = row.Field<string>("Description"),
        //                LongDescription = row.Field<string>("LongDescription"),
        //                CategoryID = row.Field<long?>("CategoryID"),
        //                GLAccountID = row.Field<long?>("GLAccountID"),
        //                UOMID = row.Field<long?>("UOMID"),
        //                PricePerTerm = row.Field<double?>("PricePerTerm"),
        //                CostUOMID = row.Field<long?>("CostUOMID"),
        //                DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
        //                DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
        //                Cost = row.Field<double?>("Cost"),
        //                Markup = row.Field<double?>("Markup"),
        //                SellPrice = row.Field<double?>("SellPrice"),
        //                ExtendedCost = row.Field<double?>("ExtendedCost"),
        //                AverageCost = row.Field<double?>("AverageCost"),
        //                LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
        //                Link1 = row.Field<string>("Link1"),
        //                Link2 = row.Field<string>("Link2"),
        //                Trend = row.Field<bool>("Trend"),
        //                IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
        //                Taxable = row.Field<bool>("Taxable"),
        //                Consignment = row.Field<bool>("Consignment"),
        //                StagedQuantity = row.Field<double?>("StagedQuantity"),
        //                InTransitquantity = row.Field<double?>("InTransitquantity"),
        //                OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
        //                OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
        //                OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
        //                SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
        //                SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
        //                RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
        //                PackingQuantity = row.Field<double?>("PackingQuantity"),
        //                AverageUsage = row.Field<double?>("AverageUsage"),
        //                Turns = row.Field<double?>("Turns"),
        //                OnHandQuantity = row.Field<double?>("OnHandQuantity"),
        //                CriticalQuantity = row.Field<double>("CriticalQuantity"),
        //                MinimumQuantity = row.Field<double>("MinimumQuantity"),
        //                MaximumQuantity = row.Field<double>("MaximumQuantity"),
        //                WeightPerPiece = row.Field<double?>("WeightPerPiece"),
        //                ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
        //                IsPurchase = row.Field<bool>("IsPurchase"),
        //                IsTransfer = row.Field<bool>("IsTransfer"),
        //                DefaultLocation = row.Field<long>("DefaultLocation"),
        //                DefaultLocationName = row.Field<string>("DefaultLocationName"),
        //                InventoryClassification = row.Field<int?>("InventoryClassification"),
        //                SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
        //                LotNumberTracking = row.Field<bool>("LotNumberTracking"),
        //                DateCodeTracking = row.Field<bool>("DateCodeTracking"),
        //                ItemType = row.Field<int>("ItemType"),
        //                ImagePath = row.Field<string>("ImagePath"),
        //                UDF1 = row.Field<string>("UDF1"),
        //                UDF2 = row.Field<string>("UDF2"),
        //                UDF3 = row.Field<string>("UDF3"),
        //                UDF4 = row.Field<string>("UDF4"),
        //                UDF5 = row.Field<string>("UDF5"),
        //                GUID = row.Field<Guid>("GUID"),
        //                // Created = row.Field<DateTime?>("Created"),
        //                //  Updated = row.Field<DateTime?>("Updated"),
        //                Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
        //                Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),

        //                CreatedBy = row.Field<long?>("CreatedBy"),
        //                LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
        //                IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
        //                IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
        //                CompanyID = row.Field<long?>("CompanyID"),
        //                Room = row.Field<long?>("Room"),
        //                CreatedByName = row.Field<string>("CreatedByName"),
        //                UpdatedByName = row.Field<string>("UpdatedByName"),
        //                RoomName = row.Field<string>("RoomName"),
        //                IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
        //                CategoryName = row.Field<string>("CategoryName"),
        //                Unit = row.Field<string>("Unit"),
        //                GLAccount = row.Field<string>("GLAccount"),
        //                IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
        //                IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
        //                BondedInventory = row.Field<string>("BondedInventory"),
        //                IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
        //                InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
        //                IsBOMItem = row.Field<bool>("IsBOMItem"),
        //                RefBomId = row.Field<long?>("RefBomId"),
        //                CostUOMName = row.Field<string>("CostUOMName"),
        //                PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
        //                TrendingSetting = row.Field<byte?>("TrendingSetting"),
        //                AddedFrom = row.Field<string>("AddedFrom"),
        //                EditedFrom = row.Field<string>("EditedFrom"),
        //                ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
        //                ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
        //            }).ToList();
        //        }
        //    }
        //    return lstItems;
        //}

        //public IEnumerable<ItemMasterDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUIDs, bool ItemsHaveQuantityOnly, string ItemType, string QuickListType, long SupplierID = 0)
        //{
        //    //Get Cached-Media
        //    IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
        //    if (SupplierID > 0)
        //    {
        //        ObjCache = ObjCache.Where(t => t.SupplierID == SupplierID);
        //    }
        //    //ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));
        //    if (!string.IsNullOrEmpty(ItemGUIDs))
        //    {
        //        string[] arrGUIds = ItemGUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //        IEnumerable<ItemMasterDTO> obj = (from x in ObjCache
        //                                          where !arrGUIds.Contains(x.GUID.ToString())
        //                                          select x).AsEnumerable();
        //        ObjCache = obj;
        //    }
        //    if (ItemsHaveQuantityOnly)
        //    {
        //        IEnumerable<ItemMasterDTO> obj = (from x in ObjCache
        //                                          where x.OnHandQuantity != null && x.OnHandQuantity > 0
        //                                          select x).AsEnumerable();
        //        ObjCache = obj;
        //    }

        //    if (ItemType == "item")
        //        ObjCache = ObjCache.Where(x => x.ItemType != 4);


        //    // QuickList Data Binding for Item PopUP /////////////////////////////////START
        //    QuickListDAL objQLDAL = new QuickListDAL(base.DataBaseName);
        //    List<QuickListMasterDTO> QuickListDATA = objQLDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted, QuickListType).ToList();
        //    List<ItemMasterDTO> TempItemQLData = new List<ItemMasterDTO>();
        //    foreach (QuickListMasterDTO item in QuickListDATA)
        //    {
        //        ItemMasterDTO tempItem = new ItemMasterDTO();
        //        tempItem.QuickListGUID = item.GUID.ToString();
        //        tempItem.QuickListName = item.Name;
        //        tempItem.ItemNumber = item.Name;
        //        tempItem.GUID = item.GUID;
        //        TempItemQLData.Add(tempItem);
        //    }
        //    if (TempItemQLData.Count > 0)
        //        ObjCache = ObjCache.Concat(TempItemQLData.AsEnumerable());
        //    // QuickList Data Binding for Item PopUP /////////////////////////////////END



        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        if (SearchTerm.Contains("100_1000"))
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                   ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0])))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //        else if (SearchTerm.Contains("10_1000"))
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                   ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                && ((Fields[1].Split('@')[15] == "") || (t.Cost <= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0])))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //        else
        //        {
        //            ObjCache = ObjCache.Where(t =>
        //                                      ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //                                   && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //                                   && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //                                   && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //                                   && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //                                   && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //                                   && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //                                   && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //                                   && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //                                   && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //                                   && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //                                   && ((Fields[1].Split('@')[15] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0]) && t.Cost <= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[1])))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
        //                                   && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
        //                                   && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
        //                                  );

        //            TotalCount = ObjCache.Count();
        //            return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //        }
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ItemMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //}
        //public IEnumerable<ItemMasterDTO> GetPagedRecordsForTransferItemModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUIDs, Int64 ReplinishRoomID)
        //{
        //    IEnumerable<ItemMasterDTO> ObjCacheRequesting = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);

        //    List<ItemMasterDTO> ObjCacheReplenish = GetCachedData(ReplinishRoomID, CompanyID, IsArchived, IsDeleted).ToList();

        //    List<ItemMasterDTO> objNewCache = new List<ItemMasterDTO>();
        //    foreach (var item in ObjCacheRequesting)
        //    {
        //        if (ObjCacheReplenish.FindIndex(x => x.ItemNumber == item.ItemNumber) >= 0)
        //        {
        //            objNewCache.Add(item);
        //        }
        //    }

        //    IEnumerable<ItemMasterDTO> ObjCache = objNewCache.AsEnumerable();

        //    if (!string.IsNullOrEmpty(ItemGUIDs))
        //    {
        //        string[] arrGUIDs = ItemGUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //        IEnumerable<ItemMasterDTO> obj = (from x in ObjCache
        //                                          where !arrGUIDs.Contains(x.GUID.ToString())
        //                                          select x).AsEnumerable();
        //        ObjCache = obj;
        //    }


        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        ObjCache = ObjCache.Where(t =>
        //               ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //            && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //            && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //            && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //            && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //            && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //            && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //            && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
        //            && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
        //            && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
        //            );

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else
        //    {
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //}
        //public CostDTO GetExtCostAndAvgCostLocationWise(Guid ItemGUID, Int64 BinID, Int64 RoomID, Int64 CompanyID)
        //{
        //    // Decimal CostDivider = 1; //100;
        //    ItemMasterDTO ObjItemDTO = new ItemMasterDTO();
        //    ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
        //    RoomDTO ObjRoomDTO = new RoomDTO();
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);

        //    ObjItemDTO = objItemDAL.GetItemWithoutJoins(null, ItemGUID);
        //    ObjRoomDTO = objRoomDAL.GetRecord(RoomID, CompanyID, false, false);

        //    CostUOMMasterDAL objCostDAL = new CostUOMMasterDAL(base.DataBaseName);
        //    CostUOMMasterDTO objUOMCostDTO = objCostDAL.GetRecord(ObjItemDTO.CostUOMID.GetValueOrDefault(0), RoomID, CompanyID, false, false);

        //    if (objUOMCostDTO != null && objUOMCostDTO.CostUOMValue > 0)
        //    {
        //        //CostDivider = objUOMCostDTO.CostUOMValue.GetValueOrDefault(1);
        //    }
        //    CostDTO objCostDTO = new CostDTO();
        //    ItemLocationDetailsDAL objItemLocDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    List<ItemLocationDetailsDTO> ObjItemLocDTOs = objItemLocDAL.GetCachedData(ItemGUID, RoomID, CompanyID).Where(x => x.IsDeleted == false && x.IsArchived == false && x.BinID == BinID).ToList();
        //    objCostDTO.ExtCost = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))) * (x.Cost.GetValueOrDefault(0)));

        //    double avgcostqty1 = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))) * (x.Cost.GetValueOrDefault(0)));
        //    double avgcost1 = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))));

        //    //if (objCostDTO.ExtCost > 0)
        //    if (avgcost1 > 0)
        //        objCostDTO.AvgCost = (avgcostqty1 / avgcost1);     //(objCostDTO.ExtCost / ObjItemLocDTOs.Sum(x => (Convert.ToDecimal(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)))));

        //    if (ObjItemLocDTOs != null)
        //    {
        //        //if (ObjRoomDTO.MethodOfValuingInventory == "0") // Nothing Is Selected
        //        //{
        //        //    objCostDTO.Cost = ObjItemDTO.Cost.GetValueOrDefault(0);
        //        //    objCostDTO.SellPrice = ObjItemDTO.SellPrice.GetValueOrDefault(0);
        //        //    objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //        //    objCostDTO.ExtCost = avgcost1 * ObjItemDTO.Cost.GetValueOrDefault(0);

        //        //}
        //        if (ObjRoomDTO.MethodOfValuingInventory == "3" || ObjRoomDTO.MethodOfValuingInventory == "2" || ObjRoomDTO.MethodOfValuingInventory == "1")   // Average cost // LIFO // FIFO
        //        {
        //            double avgcostqty = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))) * (x.Cost.GetValueOrDefault(0)));
        //            double avgcost = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))));

        //            if (avgcost > 0)
        //            {
        //                double Cost = (avgcostqty / avgcost);
        //                double? markup = 0;
        //                double sellprise = 0;
        //                if (ObjItemDTO.Markup > 0)
        //                {
        //                    markup = (Cost * ObjItemDTO.Markup.GetValueOrDefault(0)) / 100;
        //                    sellprise = Cost + markup.GetValueOrDefault(0);
        //                }
        //                else
        //                {
        //                    markup = 0;
        //                    sellprise = Cost;
        //                }
        //                objCostDTO.Cost = Cost;
        //                objCostDTO.SellPrice = sellprise;
        //                objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //                // objCostDTO.ExtCost = avgcost * Cost;
        //            }
        //            else
        //            {
        //                objCostDTO.Cost = ObjItemDTO.Cost.GetValueOrDefault(0);
        //                objCostDTO.SellPrice = ObjItemDTO.SellPrice.GetValueOrDefault(0);
        //                objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //                //objCostDTO.ExtCost = avgcost * (decimal)ObjItemDTO.Cost.GetValueOrDefault(0);
        //            }
        //        }
        //        else if (ObjRoomDTO.MethodOfValuingInventory == "4" || ObjRoomDTO.MethodOfValuingInventory == "0")   // Last cost
        //        {
        //            if (ObjItemLocDTOs.Count > 0)
        //            {
        //                List<ItemLocationDetailsDTO> ObjItemLocDTOs1 = ObjItemLocDTOs.OrderByDescending(x => x.ID).ToList();
        //                if (ObjItemLocDTOs1 != null && ObjItemLocDTOs1.Count > 0)
        //                {
        //                    double? Cost = ObjItemLocDTOs1[0].Cost;
        //                    double? markup = 0;
        //                    double sellprise = 0;
        //                    if (ObjItemDTO.Markup > 0)
        //                    {
        //                        markup = (Cost.GetValueOrDefault(0) * ObjItemDTO.Markup.GetValueOrDefault(0)) / 100;
        //                        sellprise = Convert.ToDouble(Cost.GetValueOrDefault(0)) + Convert.ToDouble(markup);
        //                    }
        //                    else
        //                    {
        //                        markup = 0;
        //                        sellprise = (double)Cost.GetValueOrDefault(0);
        //                    }
        //                    objCostDTO.Cost = Cost.GetValueOrDefault(0);
        //                    objCostDTO.SellPrice = sellprise;
        //                    objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //                    // objCostDTO.ExtCost = avgcost1 * (decimal)Cost.GetValueOrDefault(0);
        //                }
        //            }
        //            else
        //            {
        //                objCostDTO.Cost = ObjItemDTO.Cost.GetValueOrDefault(0);
        //                objCostDTO.SellPrice = ObjItemDTO.SellPrice.GetValueOrDefault(0);
        //                objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //                // objCostDTO.ExtCost = avgcost1 * (decimal)ObjItemDTO.Cost.GetValueOrDefault(0);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        objCostDTO.Cost = ObjItemDTO.Cost.GetValueOrDefault(0);
        //        objCostDTO.SellPrice = ObjItemDTO.SellPrice.GetValueOrDefault(0);
        //        objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //        //objCostDTO.ExtCost = avgcost1 * ObjItemDTO.Cost.GetValueOrDefault(0);
        //    }
        //    return objCostDTO;
        //}
        //public IEnumerable<ItemMasterDTO> GetItemUsage(Int64 RoomID, Int64 CompanyID)
        //{
        //    IEnumerable<ItemMasterDTO> ObjCache;
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ObjCache = GetAllRecords(RoomID, CompanyID);
        //    return ObjCache;
        //}
        //public List<ItemMasterDTO> GetAllItemsForNS(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long SupplierId, bool CanSeeConsignItems, bool CanOrderConsignItems, bool CanUseConsignedQuantity, long LoggedInUserId)
        //{
        //    List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        lstItems = (from im in context.ItemMasters
        //                    join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //                    from im_sm in im_sm_join.DefaultIfEmpty()

        //                    join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //                    from im_mm in im_mm_join.DefaultIfEmpty()

        //                    join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //                    from im_rm in im_rm_join.DefaultIfEmpty()

        //                    join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
        //                    from im_cuomm in im_cuomm_join.DefaultIfEmpty()

        //                    join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //                    from im_UMC in im_UMC_join.DefaultIfEmpty()

        //                    join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //                    from im_UMU in im_UMU_join.DefaultIfEmpty()

        //                    join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //                    from im_CM in im_CM_join.DefaultIfEmpty()

        //                    join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //                    from im_UNM in im_UNM_join.DefaultIfEmpty()

        //                    join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //                    from im_gla in im_gla_join.DefaultIfEmpty()

        //                    join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
        //                    from im_icm in im_icm_join.DefaultIfEmpty()

        //                    join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //                    from im_bm in im_bm_join.DefaultIfEmpty()

        //                    where im.CompanyID == CompanyID && im.Room == RoomID && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived

        //                    select new ItemMasterDTO
        //                    {
        //                        ID = im.ID,
        //                        ItemNumber = im.ItemNumber,
        //                        ManufacturerID = im.ManufacturerID,
        //                        ManufacturerNumber = im.ManufacturerNumber,
        //                        ManufacturerName = im_mm.Manufacturer,
        //                        SupplierID = im.SupplierID,
        //                        SupplierPartNo = im.SupplierPartNo,
        //                        SupplierName = im_sm.SupplierName,
        //                        UPC = im.UPC,
        //                        UNSPSC = im.UNSPSC,
        //                        Description = im.Description,
        //                        LongDescription = im.LongDescription,
        //                        CategoryID = im.CategoryID,
        //                        GLAccountID = im.GLAccountID,
        //                        UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
        //                        PricePerTerm = im.PricePerTerm,
        //                        CostUOMID = im.CostUOMID,
        //                        DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                        DefaultPullQuantity = im.DefaultPullQuantity,
        //                        Cost = im.Cost,
        //                        Markup = im.Markup,
        //                        SellPrice = im.SellPrice,
        //                        ExtendedCost = im.ExtendedCost,
        //                        AverageCost = im.AverageCost,
        //                        LeadTimeInDays = im.LeadTimeInDays,
        //                        Link1 = im.Link1,
        //                        Link2 = im.Link2,
        //                        Trend = im.Trend,
        //                        IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                        Taxable = im.Taxable,
        //                        Consignment = im.Consignment,
        //                        StagedQuantity = im.StagedQuantity,
        //                        InTransitquantity = im.InTransitquantity,
        //                        OnOrderQuantity = im.OnOrderQuantity,
        //                        OnReturnQuantity = im.OnReturnQuantity,
        //                        OnTransferQuantity = im.OnTransferQuantity,
        //                        SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                        SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                        RequisitionedQuantity = im.RequisitionedQuantity,
        //                        PackingQuantity = im.PackingQuantity,
        //                        AverageUsage = im.AverageUsage,
        //                        Turns = im.Turns,
        //                        OnHandQuantity = im.OnHandQuantity,
        //                        CriticalQuantity = im.CriticalQuantity,
        //                        MinimumQuantity = im.MinimumQuantity,
        //                        MaximumQuantity = im.MaximumQuantity,
        //                        WeightPerPiece = im.WeightPerPiece,
        //                        ItemUniqueNumber = im.ItemUniqueNumber,
        //                        IsPurchase = (im.IsPurchase == true ? true : false),
        //                        IsTransfer = (im.IsTransfer == true ? true : false),
        //                        DefaultLocation = im.DefaultLocation ?? 0,
        //                        DefaultLocationName = im_bm.BinNumber,
        //                        InventoryClassification = im.InventoryClassification,
        //                        SerialNumberTracking = im.SerialNumberTracking,
        //                        LotNumberTracking = im.LotNumberTracking,
        //                        DateCodeTracking = im.DateCodeTracking,
        //                        ItemType = im.ItemType,
        //                        ImagePath = im.ImagePath,
        //                        UDF1 = im.UDF1,
        //                        UDF2 = im.UDF2,
        //                        UDF3 = im.UDF3,
        //                        UDF4 = im.UDF4,
        //                        UDF5 = im.UDF5,
        //                        GUID = im.GUID,
        //                        Created = im.Created,
        //                        Updated = im.Updated,
        //                        CreatedBy = im.CreatedBy,
        //                        LastUpdatedBy = im.LastUpdatedBy,
        //                        IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                        IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                        CompanyID = im.CompanyID,
        //                        Room = im.Room,
        //                        CreatedByName = im_UMC.UserName,
        //                        UpdatedByName = im_UMU.UserName,
        //                        RoomName = im_rm.RoomName,
        //                        IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                        CategoryName = im_CM.Category,
        //                        Unit = im_UNM.Unit,
        //                        GLAccount = im_gla.GLAccount,
        //                        IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                        IsBuildBreak = im.IsBuildBreak,
        //                        BondedInventory = im.BondedInventory,
        //                        PullQtyScanOverride = im.PullQtyScanOverride,
        //                        TrendingSetting = im.TrendingSetting,
        //                        IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                        InventoryClassificationName = im_icm.InventoryClassification,
        //                        IsBOMItem = im.IsBOMItem,
        //                        RefBomId = im.RefBomId,
        //                        CostUOMName = im_cuomm.CostUOM,
        //                        ItemImageExternalURL = im.ItemImageExternalURL,
        //                        LastCost = im.LastCost
        //                    }).AsParallel().ToList();


        //    }

        //    return lstItems;
        //}

        //public void UpdateItemMasterCost(Guid ItemGUID, long RoomID, long CompanyID)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
        //        Room objRoomDTO = context.Rooms.FirstOrDefault(t => t.ID == objItemMaster.Room);
        //        if (objItemMaster != null && objRoomDTO != null)
        //        {
        //            if (objItemMaster.Consignment)
        //            {

        //            }
        //            else
        //            {
        //                IQueryable<ItemLocationDetail> Ilq = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemMaster.GUID && t.IsDeleted == false).OrderByDescending(t => t.ReceivedDate).ThenByDescending(t => t.ID);
        //                if (Ilq.Any())
        //                {
        //                    objItemMaster.Cost = Ilq.First().Cost;
        //                    if (objItemMaster.Markup > 0)
        //                    {
        //                        objItemMaster.SellPrice = objItemMaster.Cost + ((objItemMaster.Cost * objItemMaster.Markup) / 100);
        //                    }
        //                    else
        //                    {
        //                        objItemMaster.SellPrice = objItemMaster.Cost;
        //                    }
        //                    context.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //}
        //public CostDTO GetExtCostAndAvgCostOld(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        //{
        //    // Decimal CostDivider = 1;
        //    ItemMasterDTO ObjItemDTO = new ItemMasterDTO();
        //    RoomDTO ObjRoomDTO = new RoomDTO();
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    CostDTO objCostDTO = new CostDTO();
        //    ItemLocationDetailsDAL objItemLocDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ObjItemDTO = GetRecordOnlyItemsFields(ItemGUID.ToString(), RoomID, CompanyID);
        //    ObjRoomDTO = objRoomDAL.GetRecord(RoomID, CompanyID, false, false);
        //    CostUOMMasterDAL objCostDAL = new CostUOMMasterDAL(base.DataBaseName);
        //    CostUOMMasterDTO objUOMCostDTO = objCostDAL.GetRecord(ObjItemDTO.CostUOMID.GetValueOrDefault(0), RoomID, CompanyID, false, false);
        //    if (objUOMCostDTO != null && objUOMCostDTO.CostUOMValue > 0)
        //    {
        //        //CostDivider = objUOMCostDTO.CostUOMValue.GetValueOrDefault(1);
        //    }
        //    List<ItemLocationDetailsDTO> ObjItemLocDTOs = objItemLocDAL.GetCachedDataeVMI(ItemGUID, RoomID, CompanyID).Where(x => x.IsDeleted == false && x.IsArchived == false).ToList();
        //    objCostDTO.ExtCost = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))) * (x.Cost.GetValueOrDefault(0)));
        //    if (objCostDTO != null && ObjItemLocDTOs.Sum(x => (Convert.ToDecimal(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)))) > 0)
        //        objCostDTO.AvgCost = (objCostDTO.ExtCost / ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)))));
        //    double avgcost = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))));
        //    if (ObjItemLocDTOs != null)
        //    {
        //        //if (ObjRoomDTO.MethodOfValuingInventory == "0") // Nothing Is Selected
        //        //{
        //        //    objCostDTO.Cost = ObjItemDTO.Cost;
        //        //    objCostDTO.SellPrice = ObjItemDTO.SellPrice;
        //        //    objCostDTO.Markup = ObjItemDTO.Markup;
        //        //    objCostDTO.ExtCost = (decimal)ObjItemDTO.OnHandQuantity.GetValueOrDefault(0) * ObjItemDTO.Cost.GetValueOrDefault(0);
        //        //}
        //        //else
        //        if (ObjRoomDTO.MethodOfValuingInventory == "3" || ObjRoomDTO.MethodOfValuingInventory == "2" || ObjRoomDTO.MethodOfValuingInventory == "1")   // Average cost
        //        {
        //            double avgcostqty = ObjItemLocDTOs.Sum(x => (Convert.ToDouble(x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))) * (x.Cost.GetValueOrDefault(0)));
        //            if (avgcost > 0)
        //            {
        //                double Cost = (avgcostqty / avgcost);
        //                double? markup = 0;
        //                double sellprise = 0;
        //                if (ObjItemDTO.Markup > 0)
        //                {
        //                    markup = (Cost * Convert.ToDouble(ObjItemDTO.Markup.GetValueOrDefault(0))) / 100;
        //                    sellprise = Cost + markup.GetValueOrDefault(0);
        //                }
        //                else
        //                {
        //                    markup = 0;
        //                    sellprise = Cost;
        //                }
        //                objCostDTO.Cost = Cost;
        //                objCostDTO.SellPrice = sellprise;
        //                objCostDTO.Markup = ObjItemDTO.Markup;
        //                // objCostDTO.ExtCost = avgcost * Cost;
        //            }
        //            else
        //            {
        //                objCostDTO.Cost = ObjItemDTO.Cost.GetValueOrDefault(0);
        //                objCostDTO.SellPrice = ObjItemDTO.SellPrice.GetValueOrDefault(0);
        //                objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //                //objCostDTO.ExtCost = avgcost * (decimal)ObjItemDTO.Cost.GetValueOrDefault(0);
        //            }
        //        }
        //        else if (ObjRoomDTO.MethodOfValuingInventory == "4" || ObjRoomDTO.MethodOfValuingInventory == "0")   // Last cost
        //        {
        //            if (ObjItemLocDTOs.Count > 0)
        //            {
        //                List<ItemLocationDetailsDTO> ObjItemLocDTOs1 = ObjItemLocDTOs.OrderByDescending(x => x.ID).ToList();
        //                if (ObjItemLocDTOs1 != null && ObjItemLocDTOs1.Count > 0)
        //                {
        //                    double? Cost = ObjItemLocDTOs1[0].Cost;
        //                    double? markup = 0;
        //                    double sellprise = 0;
        //                    if (ObjItemDTO.Markup > 0)
        //                    {
        //                        markup = (Cost.GetValueOrDefault(0) * ObjItemDTO.Markup.GetValueOrDefault(0)) / 100;
        //                        sellprise = Cost.GetValueOrDefault(0) + markup.GetValueOrDefault(0);
        //                    }
        //                    else
        //                    {
        //                        markup = 0;
        //                        sellprise = Cost.GetValueOrDefault(0);
        //                    }
        //                    objCostDTO.Cost = Cost.GetValueOrDefault(0);
        //                    objCostDTO.SellPrice = sellprise;
        //                    objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //                    // objCostDTO.ExtCost = avgcost * (decimal)Cost.GetValueOrDefault(0);
        //                }
        //            }
        //            else
        //            {
        //                objCostDTO.Cost = ObjItemDTO.Cost.GetValueOrDefault(0);
        //                objCostDTO.SellPrice = ObjItemDTO.SellPrice.GetValueOrDefault(0);
        //                objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //                //  objCostDTO.ExtCost = avgcost * (decimal)ObjItemDTO.Cost.GetValueOrDefault(0);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        objCostDTO.Cost = ObjItemDTO.Cost.GetValueOrDefault(0);
        //        objCostDTO.SellPrice = ObjItemDTO.SellPrice.GetValueOrDefault(0);
        //        objCostDTO.Markup = ObjItemDTO.Markup.GetValueOrDefault(0);
        //        // objCostDTO.ExtCost = (decimal)ObjItemDTO.OnHandQuantity.GetValueOrDefault(0) * ObjItemDTO.Cost.GetValueOrDefault(0);

        //    }
        //    return objCostDTO;
        //}


        //public IEnumerable<ItemMasterDTO> GetCachedDataForService(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ConnectionString)
        //{
        //    IEnumerable<ItemMasterDTO> ObjCache;
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        ObjCache = (from im in context.ItemMasters
        //                    join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //                    from im_sm in im_sm_join.DefaultIfEmpty()

        //                    join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //                    from im_mm in im_mm_join.DefaultIfEmpty()

        //                    join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //                    from im_rm in im_rm_join.DefaultIfEmpty()

        //                    join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
        //                    from im_cuomm in im_cuomm_join.DefaultIfEmpty()

        //                    join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //                    from im_UMC in im_UMC_join.DefaultIfEmpty()

        //                    join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //                    from im_UMU in im_UMU_join.DefaultIfEmpty()

        //                    join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //                    from im_CM in im_CM_join.DefaultIfEmpty()

        //                    join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //                    from im_UNM in im_UNM_join.DefaultIfEmpty()

        //                    join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //                    from im_gla in im_gla_join.DefaultIfEmpty()

        //                    join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
        //                    from im_icm in im_icm_join.DefaultIfEmpty()

        //                    join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //                    from im_bm in im_bm_join.DefaultIfEmpty()

        //                    where im.CompanyID == CompanyID && im.Room == RoomID

        //                    orderby im.ID descending

        //                    select new ItemMasterDTO
        //                    {
        //                        ID = im.ID,
        //                        ItemNumber = im.ItemNumber,
        //                        ManufacturerID = im.ManufacturerID,
        //                        ManufacturerNumber = im.ManufacturerNumber,
        //                        ManufacturerName = im_mm.Manufacturer,
        //                        SupplierID = im.SupplierID,
        //                        SupplierPartNo = im.SupplierPartNo,
        //                        SupplierName = im_sm.SupplierName,
        //                        UPC = im.UPC,
        //                        UNSPSC = im.UNSPSC,
        //                        Description = im.Description,
        //                        LongDescription = im.LongDescription,
        //                        CategoryID = im.CategoryID,
        //                        GLAccountID = im.GLAccountID,
        //                        UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
        //                        PricePerTerm = im.PricePerTerm,
        //                        CostUOMID = im.CostUOMID,
        //                        DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                        DefaultPullQuantity = im.DefaultPullQuantity,
        //                        Cost = im.Cost,
        //                        Markup = im.Markup,
        //                        SellPrice = im.SellPrice,
        //                        ExtendedCost = im.ExtendedCost,
        //                        AverageCost = im.AverageCost,
        //                        LeadTimeInDays = im.LeadTimeInDays,
        //                        Link1 = im.Link1,
        //                        Link2 = im.Link2,
        //                        Trend = im.Trend,
        //                        IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                        Taxable = im.Taxable,
        //                        Consignment = im.Consignment,
        //                        StagedQuantity = im.StagedQuantity,
        //                        InTransitquantity = im.InTransitquantity,
        //                        OnOrderQuantity = im.OnOrderQuantity,
        //                        OnReturnQuantity = im.OnReturnQuantity,
        //                        OnTransferQuantity = im.OnTransferQuantity,
        //                        SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                        SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                        RequisitionedQuantity = im.RequisitionedQuantity,
        //                        PackingQuantity = im.PackingQuantity,
        //                        AverageUsage = im.AverageUsage,
        //                        Turns = im.Turns,
        //                        OnHandQuantity = im.OnHandQuantity,
        //                        CriticalQuantity = im.CriticalQuantity,
        //                        MinimumQuantity = im.MinimumQuantity,
        //                        MaximumQuantity = im.MaximumQuantity,
        //                        WeightPerPiece = im.WeightPerPiece,
        //                        ItemUniqueNumber = im.ItemUniqueNumber,
        //                        IsPurchase = (im.IsPurchase == true ? true : false),
        //                        IsTransfer = (im.IsTransfer == true ? true : false),
        //                        DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                        DefaultLocationName = im_bm.BinNumber,
        //                        InventoryClassification = im.InventoryClassification,
        //                        SerialNumberTracking = im.SerialNumberTracking,
        //                        LotNumberTracking = im.LotNumberTracking,
        //                        DateCodeTracking = im.DateCodeTracking,
        //                        ItemType = im.ItemType,
        //                        ImagePath = im.ImagePath,
        //                        UDF1 = im.UDF1,
        //                        UDF2 = im.UDF2,
        //                        UDF3 = im.UDF3,
        //                        UDF4 = im.UDF4,
        //                        UDF5 = im.UDF5,
        //                        GUID = im.GUID,
        //                        Created = im.Created,
        //                        Updated = im.Updated,
        //                        CreatedBy = im.CreatedBy,
        //                        LastUpdatedBy = im.LastUpdatedBy,
        //                        IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                        IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                        CompanyID = im.CompanyID,
        //                        Room = im.Room,
        //                        CreatedByName = im_UMC.UserName,
        //                        UpdatedByName = im_UMU.UserName,
        //                        RoomName = im_rm.RoomName,
        //                        IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                        CategoryName = im_CM.Category,
        //                        Unit = im_UNM.Unit,
        //                        GLAccount = im_gla.GLAccount,
        //                        IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                        IsBuildBreak = im.IsBuildBreak,
        //                        BondedInventory = im.BondedInventory,
        //                        IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                        InventoryClassificationName = im_icm.InventoryClassification,
        //                        IsBOMItem = im.IsBOMItem,
        //                        RefBomId = im.RefBomId,
        //                        CostUOMName = im_cuomm.CostUOM,
        //                        PullQtyScanOverride = im.PullQtyScanOverride,
        //                        ItemImageExternalURL = im.ItemImageExternalURL,
        //                        TrendingSetting = im.TrendingSetting
        //                    }).AsParallel().ToList();
        //    }
        //    return ObjCache;
        //}

        //public IEnumerable<ItemMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        //{
        //    IEnumerable<ItemMasterDTO> ObjCache;
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
        //    IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);


        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {

        //        ObjCache = (from im in context.ItemMasters
        //                    join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //                    from im_sm in im_sm_join.DefaultIfEmpty()

        //                    join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //                    from im_mm in im_mm_join.DefaultIfEmpty()

        //                    join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //                    from im_rm in im_rm_join.DefaultIfEmpty()

        //                    join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
        //                    from im_cuomm in im_cuomm_join.DefaultIfEmpty()

        //                    join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //                    from im_UMC in im_UMC_join.DefaultIfEmpty()

        //                    join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //                    from im_UMU in im_UMU_join.DefaultIfEmpty()

        //                    join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //                    from im_CM in im_CM_join.DefaultIfEmpty()

        //                    join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //                    from im_UNM in im_UNM_join.DefaultIfEmpty()

        //                    join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //                    from im_gla in im_gla_join.DefaultIfEmpty()

        //                    join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
        //                    from im_icm in im_icm_join.DefaultIfEmpty()

        //                    join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //                    from im_bm in im_bm_join.DefaultIfEmpty()

        //                    where im.CompanyID == CompanyID && im.Room == RoomID


        //                    select new ItemMasterDTO
        //                    {
        //                        ID = im.ID,
        //                        ItemNumber = im.ItemNumber,
        //                        ManufacturerID = im.ManufacturerID,
        //                        ManufacturerNumber = im.ManufacturerNumber,
        //                        ManufacturerName = im_mm.Manufacturer,
        //                        SupplierID = im.SupplierID,
        //                        SupplierPartNo = im.SupplierPartNo,
        //                        SupplierName = im_sm.SupplierName,
        //                        UPC = im.UPC,
        //                        UNSPSC = im.UNSPSC,
        //                        Description = im.Description,
        //                        LongDescription = im.LongDescription,
        //                        CategoryID = im.CategoryID,
        //                        GLAccountID = im.GLAccountID,
        //                        UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
        //                        PricePerTerm = im.PricePerTerm,
        //                        CostUOMID = im.CostUOMID,
        //                        DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                        DefaultPullQuantity = im.DefaultPullQuantity,
        //                        Cost = im.Cost,
        //                        Markup = im.Markup,
        //                        SellPrice = im.SellPrice,
        //                        ExtendedCost = im.ExtendedCost,
        //                        AverageCost = im.AverageCost,
        //                        LeadTimeInDays = im.LeadTimeInDays,
        //                        Link1 = im.Link1,
        //                        Link2 = im.Link2,
        //                        Trend = im.Trend,
        //                        Taxable = im.Taxable,
        //                        Consignment = im.Consignment,
        //                        StagedQuantity = im.StagedQuantity,
        //                        InTransitquantity = im.InTransitquantity,
        //                        OnOrderQuantity = im.OnOrderQuantity,
        //                        OnReturnQuantity = im.OnReturnQuantity,
        //                        OnTransferQuantity = im.OnTransferQuantity,
        //                        SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                        SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                        RequisitionedQuantity = im.RequisitionedQuantity,
        //                        PackingQuantity = im.PackingQuantity,
        //                        AverageUsage = im.AverageUsage,
        //                        Turns = im.Turns,
        //                        OnHandQuantity = im.OnHandQuantity,
        //                        CriticalQuantity = im.CriticalQuantity,
        //                        MinimumQuantity = im.MinimumQuantity,
        //                        MaximumQuantity = im.MaximumQuantity,
        //                        WeightPerPiece = im.WeightPerPiece,
        //                        ItemUniqueNumber = im.ItemUniqueNumber,
        //                        IsPurchase = (im.IsPurchase == true ? true : false),
        //                        IsTransfer = (im.IsTransfer == true ? true : false),
        //                        DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
        //                        DefaultLocationName = im_bm.BinNumber,
        //                        InventoryClassification = im.InventoryClassification,
        //                        SerialNumberTracking = im.SerialNumberTracking,
        //                        LotNumberTracking = im.LotNumberTracking,
        //                        DateCodeTracking = im.DateCodeTracking,
        //                        ItemType = im.ItemType,
        //                        ImagePath = im.ImagePath,
        //                        UDF1 = im.UDF1,
        //                        UDF2 = im.UDF2,
        //                        UDF3 = im.UDF3,
        //                        UDF4 = im.UDF4,
        //                        UDF5 = im.UDF5,
        //                        ItemUDF1 = im.UDF1,
        //                        ItemUDF2 = im.UDF2,
        //                        ItemUDF3 = im.UDF3,
        //                        ItemUDF4 = im.UDF4,
        //                        ItemUDF5 = im.UDF5,
        //                        GUID = im.GUID,
        //                        Created = im.Created,
        //                        Updated = im.Updated,
        //                        CreatedBy = im.CreatedBy,
        //                        LastUpdatedBy = im.LastUpdatedBy,
        //                        IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                        IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                        CompanyID = im.CompanyID,
        //                        Room = im.Room,
        //                        CreatedByName = im_UMC.UserName,
        //                        UpdatedByName = im_UMU.UserName,
        //                        RoomName = im_rm.RoomName,
        //                        IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                        CategoryName = im_CM.Category,
        //                        Unit = im_UNM.Unit,
        //                        GLAccount = im_gla.GLAccount,
        //                        IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                        IsBuildBreak = im.IsBuildBreak,
        //                        BondedInventory = im.BondedInventory,
        //                        IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                        PullQtyScanOverride = im.PullQtyScanOverride,
        //                        TrendingSetting = im.TrendingSetting,
        //                        IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                        ItemImageExternalURL = im.ItemImageExternalURL,
        //                        LastCost = im.LastCost,
        //                        ItemLocations = (from A in context.ItemLocationDetails
        //                                         join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                         from A_B in A_B_join.DefaultIfEmpty()
        //                                         join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                         from A_C in A_C_join.DefaultIfEmpty()
        //                                         join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                         from A_D in A_D_join.DefaultIfEmpty()
        //                                         where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                         select new ItemLocationDetailsDTO
        //                                         {
        //                                             ID = A.ID,
        //                                             BinID = A.BinID,
        //                                             CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                             ConsignedQuantity = A.ConsignedQuantity,
        //                                             MeasurementID = A.MeasurementID,
        //                                             LotNumber = A.LotNumber,
        //                                             SerialNumber = A.SerialNumber,
        //                                             ExpirationDate = A.ExpirationDate,
        //                                             ReceivedDate = A.ReceivedDate,
        //                                             Expiration = A.Expiration,
        //                                             Received = A.Received,
        //                                             Cost = A.Cost,
        //                                             eVMISensorPort = A.eVMISensorPort,
        //                                             eVMISensorID = A.eVMISensorID,
        //                                             GUID = A.GUID,
        //                                             ItemGUID = A.ItemGUID,
        //                                             Created = A.Created,
        //                                             Updated = A.Updated,
        //                                             CreatedBy = A.CreatedBy,
        //                                             LastUpdatedBy = A.LastUpdatedBy,
        //                                             IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                             IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                             CompanyID = A.CompanyID,
        //                                             Room = A.Room,
        //                                             CreatedByName = A_C.UserName,
        //                                             UpdatedByName = A_D.UserName,
        //                                             BinNumber = A_B.BinNumber,
        //                                             ItemNumber = im.ItemNumber,
        //                                             SerialNumberTracking = im.SerialNumberTracking,
        //                                             LotNumberTracking = im.LotNumberTracking,
        //                                             DateCodeTracking = im.DateCodeTracking,
        //                                             OrderDetailGUID = A.OrderDetailGUID,
        //                                             TransferDetailGUID = A.TransferDetailGUID,
        //                                             KitDetailGUID = A.KitDetailGUID,
        //                                             CriticalQuantity = A_B.CriticalQuantity,
        //                                             MinimumQuantity = A_B.MinimumQuantity,
        //                                             MaximumQuantity = A_B.MaximumQuantity,
        //                                             SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                         }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                        InventoryClassificationName = im_icm.InventoryClassification,
        //                        IsBOMItem = im.IsBOMItem,
        //                        RefBomId = im.RefBomId,
        //                        CostUOMName = im_cuomm.CostUOM,
        //                        IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive
        //                    }).AsParallel().ToList();


        //    }
        //    return ObjCache;
        //}

        //public IEnumerable<ItemMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    IEnumerable<ItemMasterDTO> ObjCache = null;
        //    //ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    // BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
        //    // IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);
        //    #region "Conditional"

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        ObjCache = (from im in context.ItemMasters
        //                    join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
        //                    from im_sm in im_sm_join.DefaultIfEmpty()

        //                    join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
        //                    from im_mm in im_mm_join.DefaultIfEmpty()

        //                    join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
        //                    from im_rm in im_rm_join.DefaultIfEmpty()

        //                    join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
        //                    from im_cuomm in im_cuomm_join.DefaultIfEmpty()

        //                    join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
        //                    from im_UMC in im_UMC_join.DefaultIfEmpty()

        //                    join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
        //                    from im_UMU in im_UMU_join.DefaultIfEmpty()

        //                    join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
        //                    from im_CM in im_CM_join.DefaultIfEmpty()

        //                    join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
        //                    from im_UNM in im_UNM_join.DefaultIfEmpty()

        //                    join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
        //                    from im_gla in im_gla_join.DefaultIfEmpty()

        //                    join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
        //                    from im_icm in im_icm_join.DefaultIfEmpty()

        //                    join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
        //                    from im_bm in im_bm_join.DefaultIfEmpty()



        //                    where im.CompanyID == CompanyID && im.Room == RoomID && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived

        //                    select new ItemMasterDTO
        //                    {
        //                        ID = im.ID,
        //                        ItemNumber = im.ItemNumber,
        //                        ManufacturerID = im.ManufacturerID,
        //                        ManufacturerNumber = im.ManufacturerNumber,
        //                        ManufacturerName = im_mm.Manufacturer,
        //                        SupplierID = im.SupplierID,
        //                        SupplierPartNo = im.SupplierPartNo,
        //                        SupplierName = im_sm.SupplierName,
        //                        UPC = im.UPC,
        //                        UNSPSC = im.UNSPSC,
        //                        Description = im.Description,
        //                        LongDescription = im.LongDescription,
        //                        CategoryID = im.CategoryID,
        //                        GLAccountID = im.GLAccountID,
        //                        UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
        //                        PricePerTerm = im.PricePerTerm,
        //                        CostUOMID = im.CostUOMID,
        //                        DefaultReorderQuantity = im.DefaultReorderQuantity,
        //                        DefaultPullQuantity = im.DefaultPullQuantity,
        //                        Cost = im.Cost,
        //                        Markup = im.Markup,
        //                        SellPrice = im.SellPrice,
        //                        ExtendedCost = im.ExtendedCost,
        //                        AverageCost = im.AverageCost,
        //                        LeadTimeInDays = im.LeadTimeInDays,
        //                        Link1 = im.Link1,
        //                        Link2 = im.Link2,
        //                        Trend = im.Trend,
        //                        Taxable = im.Taxable,
        //                        Consignment = im.Consignment,
        //                        StagedQuantity = im.StagedQuantity,
        //                        InTransitquantity = im.InTransitquantity,
        //                        OnOrderQuantity = im.OnOrderQuantity,
        //                        OnReturnQuantity = im.OnReturnQuantity,
        //                        OnTransferQuantity = im.OnTransferQuantity,
        //                        SuggestedOrderQuantity = im.SuggestedOrderQuantity,
        //                        SuggestedTransferQuantity = im.SuggestedTransferQuantity,
        //                        RequisitionedQuantity = im.RequisitionedQuantity,
        //                        PackingQuantity = im.PackingQuantity,
        //                        AverageUsage = im.AverageUsage,
        //                        Turns = im.Turns,
        //                        OnHandQuantity = im.OnHandQuantity == null ? 0 : im.OnHandQuantity,
        //                        CriticalQuantity = im.CriticalQuantity,
        //                        MinimumQuantity = im.MinimumQuantity,
        //                        MaximumQuantity = im.MaximumQuantity,
        //                        WeightPerPiece = im.WeightPerPiece,
        //                        ItemUniqueNumber = im.ItemUniqueNumber,
        //                        IsPurchase = (im.IsPurchase == true ? true : false),
        //                        IsTransfer = (im.IsTransfer == true ? true : false),
        //                        DefaultLocation = im.DefaultLocation ?? 0,
        //                        DefaultLocationName = im_bm.BinNumber,
        //                        InventoryClassification = im.InventoryClassification,
        //                        SerialNumberTracking = im.SerialNumberTracking,
        //                        LotNumberTracking = im.LotNumberTracking,
        //                        DateCodeTracking = im.DateCodeTracking,
        //                        ItemType = im.ItemType,
        //                        ImagePath = im.ImagePath,
        //                        UDF1 = im.UDF1,
        //                        UDF2 = im.UDF2,
        //                        UDF3 = im.UDF3,
        //                        UDF4 = im.UDF4,
        //                        UDF5 = im.UDF5,
        //                        ItemUDF1 = im.UDF1,
        //                        ItemUDF2 = im.UDF2,
        //                        ItemUDF3 = im.UDF3,
        //                        ItemUDF4 = im.UDF4,
        //                        ItemUDF5 = im.UDF5,
        //                        GUID = im.GUID,
        //                        Created = im.Created,
        //                        Updated = im.Updated,
        //                        CreatedBy = im.CreatedBy,
        //                        LastUpdatedBy = im.LastUpdatedBy,
        //                        IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
        //                        IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
        //                        CompanyID = im.CompanyID,
        //                        Room = im.Room,
        //                        CreatedByName = im_UMC.UserName,
        //                        UpdatedByName = im_UMU.UserName,
        //                        RoomName = im_rm.RoomName,
        //                        IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
        //                        CategoryName = im_CM.Category,
        //                        Unit = im_UNM.Unit,
        //                        GLAccount = im_gla.GLAccount,
        //                        IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
        //                        IsBuildBreak = im.IsBuildBreak,
        //                        BondedInventory = im.BondedInventory,
        //                        PullQtyScanOverride = im.PullQtyScanOverride,
        //                        TrendingSetting = im.TrendingSetting,
        //                        IsAutoInventoryClassification = im.IsAutoInventoryClassification,
        //                        IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
        //                        LastCost = im.LastCost,
        //                        ReceivedOn = im.ReceivedOn,
        //                        ReceivedOnWeb = im.ReceivedOnWeb,
        //                        AddedFrom = im.AddedFrom,
        //                        EditedFrom = im.EditedFrom,
        //                        ItemImageExternalURL = im.ItemImageExternalURL,
        //                        ImageType = im.ImageType,
        //                        ItemDocExternalURL = im.ItemDocExternalURL,
        //                        CostUOMName = im_cuomm.CostUOM,
        //                        ItemLocations = (from A in context.ItemLocationDetails
        //                                         join B in context.BinMasters on A.BinID equals B.ID into A_B_join
        //                                         from A_B in A_B_join.DefaultIfEmpty()
        //                                         join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
        //                                         from A_C in A_C_join.DefaultIfEmpty()
        //                                         join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
        //                                         from A_D in A_D_join.DefaultIfEmpty()
        //                                         where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                         select new ItemLocationDetailsDTO
        //                                         {
        //                                             ID = A.ID,
        //                                             BinID = A.BinID,
        //                                             CustomerOwnedQuantity = A.CustomerOwnedQuantity,
        //                                             ConsignedQuantity = A.ConsignedQuantity,
        //                                             MeasurementID = A.MeasurementID,
        //                                             LotNumber = A.LotNumber,
        //                                             SerialNumber = A.SerialNumber,
        //                                             ExpirationDate = A.ExpirationDate,
        //                                             ReceivedDate = A.ReceivedDate,
        //                                             Expiration = A.Expiration,
        //                                             Received = A.Received,
        //                                             Cost = A.Cost,
        //                                             GUID = A.GUID,
        //                                             ItemGUID = A.ItemGUID,
        //                                             Created = A.Created,
        //                                             Updated = A.Updated,
        //                                             CreatedBy = A.CreatedBy,
        //                                             LastUpdatedBy = A.LastUpdatedBy,
        //                                             IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
        //                                             IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
        //                                             CompanyID = A.CompanyID,
        //                                             Room = A.Room,
        //                                             CreatedByName = A_C.UserName,
        //                                             UpdatedByName = A_D.UserName,
        //                                             BinNumber = A_B.BinNumber,
        //                                             ItemNumber = im.ItemNumber,
        //                                             SerialNumberTracking = im.SerialNumberTracking,
        //                                             LotNumberTracking = im.LotNumberTracking,
        //                                             DateCodeTracking = im.DateCodeTracking,
        //                                             OrderDetailGUID = A.OrderDetailGUID,
        //                                             TransferDetailGUID = A.TransferDetailGUID,
        //                                             KitDetailGUID = A.KitDetailGUID,
        //                                             CriticalQuantity = A_B.CriticalQuantity,
        //                                             MinimumQuantity = A_B.MinimumQuantity,
        //                                             MaximumQuantity = A_B.MaximumQuantity,
        //                                             SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
        //                                         }).AsEnumerable<ItemLocationDetailsDTO>(),
        //                        InventoryClassificationName = im_icm.InventoryClassification,
        //                        IsBOMItem = im.IsBOMItem,
        //                        RefBomId = im.RefBomId,

        //                        IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
        //                        BlanketOrderNumber = (from A in context.ItemSupplierDetails
        //                                              join B in context.SupplierBlanketPODetails on A.BlanketPOID equals B.ID
        //                                              where A.ItemGUID == im.GUID && A.SupplierID == im_sm.ID && (A.IsDefault ?? false)
        //                                              && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
        //                                              select B.BlanketPO
        //                                                  ).FirstOrDefault(),
        //                        //CreatedDate = CommonUtility.ConvertDateByTimeZone(im.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),

        //                        //UpdatedDate = CommonUtility.ConvertDateByTimeZone(im.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
        //                    }).AsParallel().ToList();
        //    }

        //    #endregion
        //    return ObjCache;
        //}
    }
}
