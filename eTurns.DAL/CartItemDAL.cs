using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace eTurns.DAL
{
    public partial class CartItemDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public CartItemDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public CartItemDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        #region [Class Methods]
        public List<CartItemDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool ActiveOnly, List<long> SupplierIds, bool AllowUserToOrderConsignedItems)
        {
            eTurnsRegionInfo objeTurnsRegionInfo = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyId, 0);
            if (objeTurnsRegionInfo == null)
            {
                objeTurnsRegionInfo = new eTurnsRegionInfo();
            }
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            var params1 = new SqlParameter[] {
                    new SqlParameter("@arrids", (object)DBNull.Value),
                    new SqlParameter("@RoomId", RoomID),
                    new SqlParameter("@CompanyId", CompanyId),
                    new SqlParameter("@SupplierIds", strSupplierIds)
                };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstCartItems = (from ci in context.Database.SqlQuery<CartItemDTO>("exec [GetCartItemsByGuids] @arrids,@RoomId,@CompanyId,@SupplierIds", params1)
                                select new CartItemDTO
                                {
                                    ID = ci.ID,
                                    ItemNumber = ci.ItemNumber,
                                    ItemGUID = ci.ItemGUID,
                                    Quantity = Math.Round(ci.Quantity ?? 0, objeTurnsRegionInfo.NumberDecimalDigits),
                                    Status = ci.Status,
                                    ReplenishType = ci.ReplenishType,
                                    IsKitComponent = ci.IsKitComponent,
                                    UDF1 = ci.UDF1,
                                    UDF2 = ci.UDF2,
                                    UDF3 = ci.UDF3,
                                    UDF4 = ci.UDF4,
                                    UDF5 = ci.UDF5,
                                    GUID = ci.GUID,
                                    Created = ci.Created,
                                    Updated = ci.Updated,
                                    CreatedBy = ci.CreatedBy,
                                    LastUpdatedBy = ci.LastUpdatedBy,
                                    IsDeleted = ci.IsDeleted,
                                    IsArchived = ci.IsArchived,
                                    CompanyID = ci.CompanyID,
                                    Room = ci.Room,
                                    CreatedByName = ci.CreatedByName,
                                    UpdatedByName = ci.UpdatedByName,
                                    RoomName = ci.RoomName,
                                    IsAutoMatedEntry = ci.IsAutoMatedEntry,
                                    SupplierId = ci.SupplierId,
                                    SupplierName = ci.SupplierName,
                                    BlanketPOID = ci.BlanketPOID,
                                    BlanketPONumber = ci.BlanketPONumber,
                                    BinId = ci.BinId,
                                    BinName = ci.BinName,
                                    ReceivedOn = ci.ReceivedOn,
                                    ReceivedOnWeb = ci.ReceivedOnWeb,
                                    AddedFrom = ci.AddedFrom,
                                    EditedFrom = ci.EditedFrom,
                                    ItemUDF1 = ci.ItemUDF1,
                                    ItemUDF2 = ci.ItemUDF2,
                                    ItemUDF3 = ci.ItemUDF3,
                                    ItemUDF4 = ci.ItemUDF4,
                                    ItemUDF5 = ci.ItemUDF5,
                                    OnHandQuantity = ci.OnHandQuantity,
                                    CriticalQuantity = ci.CriticalQuantity,
                                    MinimumQuantity = ci.MinimumQuantity,
                                    MaximumQuantity = ci.MaximumQuantity,
                                    CategoryID = ci.CategoryID,
                                    CategoryName = ci.CategoryName,
                                    ItemType = ci.ItemType,
                                    SerialNumberTracking = ci.SerialNumberTracking,
                                    LotNumberTracking = ci.LotNumberTracking,
                                    DateCodeTracking = ci.DateCodeTracking
                                }).ToList();
            }
            return lstCartItems;
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 EnterpriseID, long SessionUserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@GUIDs", IDs)  };

                context.Database.ExecuteSqlCommand("EXEC [DeleteCartByGUIDs] @UserID,@GUIDs", params1);

                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        CartItem objCartItemDTO = new CartItem();
                        Guid ciguid = Guid.Empty;
                        if (Guid.TryParse(item.ToString(), out ciguid))
                        {
                            objCartItemDTO = context.CartItems.FirstOrDefault(t => t.GUID == ciguid);
                            if (objCartItemDTO != null)
                            {
                                UpdateSuggestedQtyOfItem(objCartItemDTO.ItemGUID ?? Guid.Empty, objCartItemDTO.Room ?? 0, objCartItemDTO.CompanyID ?? 0, objCartItemDTO.LastUpdatedBy ?? 0, SessionUserId, EnterpriseID);
                            }
                        }
                    }
                }
                return true;
            }
        }
        public CartItemDTO SaveCartItem(CartItemDTO objDTO, long SessionUserId, long EnterpriseId)
        {
            CartItem objCartItem = new CartItem();

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                if (objDTO.ID > 0)
                {
                    objCartItem = context.CartItems.FirstOrDefault(t => t.ID == objDTO.ID);
                    objDTO.CompanyID = objCartItem.CompanyID;
                    objDTO.Created = objCartItem.Created;
                    objDTO.CreatedBy = objCartItem.CreatedBy;
                    objDTO.CreatedByName = string.Empty;
                    objDTO.GUID = objCartItem.GUID;
                    objDTO.ID = objCartItem.ID;
                    objDTO.IsArchived = objCartItem.IsArchived;
                    objDTO.IsDeleted = objCartItem.IsDeleted;
                    objDTO.IsKitComponent = objCartItem.IsKitComponent;
                    objDTO.ItemGUID = objCartItem.ItemGUID;
                    objDTO.Room = objCartItem.Room;

                    objCartItem.Updated = objDTO.Updated;
                    objCartItem.LastUpdatedBy = objDTO.LastUpdatedBy;
                    objCartItem.Quantity = objDTO.Quantity;
                    objCartItem.ReplenishType = objDTO.ReplenishType;
                    objCartItem.Status = "U";
                    objCartItem.UDF1 = objDTO.UDF1;
                    objCartItem.UDF2 = objDTO.UDF2;
                    objCartItem.UDF3 = objDTO.UDF3;
                    objCartItem.UDF4 = objDTO.UDF4;
                    objCartItem.UDF5 = objDTO.UDF5;
                    objCartItem.BinId = objDTO.BinId;
                    objCartItem.BinGUID = objDTO.BinGUID;
                    objCartItem.IsAutoMatedEntry = objDTO.IsAutoMatedEntry;
                    objCartItem.AddedFrom = "Web";
                    objCartItem.EditedFrom = "Web";
                    objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCartItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                            objCartItem.EditedFrom = objDTO.EditedFrom;
                        else
                            objCartItem.EditedFrom = "Web";
                        objCartItem.ReceivedOn = objDTO.ReceivedOn;
                    }

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Cart";

                    objCartItem.WhatWhereAction = objDTO.WhatWhereAction;

                    context.SaveChanges();
                    //IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<CartItemDTO> objTemp = ObjCache.ToList();
                    //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    //    ObjCache = objTemp.AsEnumerable();

                    //    List<CartItemDTO> tempC = new List<CartItemDTO>();
                    //    tempC.Add(objDTO);
                    //    IEnumerable<CartItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<CartItemDTO>>.AppendToCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString(), NewCache);
                    //}

                }
                else
                {
                    objDTO.IsArchived = false;
                    objDTO.IsDeleted = false;

                    objCartItem.Created = objDTO.Created;
                    objCartItem.CreatedBy = objDTO.CreatedBy;
                    objCartItem.Updated = objDTO.Updated;
                    objCartItem.LastUpdatedBy = objDTO.LastUpdatedBy;

                    objCartItem.CompanyID = objDTO.CompanyID;
                    objCartItem.GUID = Guid.NewGuid(); //objDTO.GUID;
                    objCartItem.ItemGUID = objDTO.ItemGUID;
                    objCartItem.Quantity = objDTO.Quantity;
                    objCartItem.ReplenishType = objDTO.ReplenishType;
                    objCartItem.Room = objDTO.Room;
                    objCartItem.Status = objDTO.Status;
                    objCartItem.UDF1 = objDTO.UDF1;
                    objCartItem.UDF2 = objDTO.UDF2;
                    objCartItem.UDF3 = objDTO.UDF3;
                    objCartItem.UDF4 = objDTO.UDF4;
                    objCartItem.UDF5 = objDTO.UDF5;
                    objCartItem.IsDeleted = objDTO.IsDeleted;
                    objCartItem.IsArchived = objDTO.IsArchived;
                    objCartItem.IsKitComponent = objCartItem.IsKitComponent;
                    objCartItem.BinId = objDTO.BinId;
                    objCartItem.BinGUID = objDTO.BinGUID;
                    objCartItem.IsAutoMatedEntry = objDTO.IsAutoMatedEntry;
                    objCartItem.AddedFrom = "Web";
                    objCartItem.EditedFrom = "Web";
                    objCartItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCartItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Cart";

                    if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                        objCartItem.AddedFrom = objDTO.AddedFrom;
                    else
                        objCartItem.AddedFrom = "Web";

                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        objCartItem.EditedFrom = objDTO.EditedFrom;
                    else
                        objCartItem.EditedFrom = "Web";



                    objCartItem.WhatWhereAction = objDTO.WhatWhereAction;

                    context.CartItems.Add(objCartItem);
                    context.SaveChanges();
                    objDTO.ID = objCartItem.ID;
                    objDTO.GUID = objCartItem.GUID;
                    //if (objDTO.ID > 0)
                    //{
                    //    //Get Cached-Media
                    //    IEnumerable<CartItemDTO> ObjCache = CacheHelper<IEnumerable<CartItemDTO>>.GetCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString());
                    //    if (ObjCache != null)
                    //    {
                    //        List<CartItemDTO> tempC = new List<CartItemDTO>();
                    //        tempC.Add(objDTO);

                    //        IEnumerable<CartItemDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //        CacheHelper<IEnumerable<CartItemDTO>>.AppendToCacheItem("Cached_CartItem_" + objDTO.CompanyID.ToString(), NewCache);
                    //    }
                    //}

                }

            }

            if (objDTO.IsAutoMatedEntry == false)
            {
                UpdateSuggestedQtyOfItem(objDTO.ItemGUID ?? Guid.NewGuid(), objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.LastUpdatedBy ?? 0, SessionUserId, EnterpriseId);
            }
            return objDTO;
        }
        public List<CartItemDTO> SaveCartItems(List<CartItemDTO> lstCartItems, long EnterpriseId, long SessionUserId)
        {
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(base.DataBaseName);
            List<CartItemDTO> lstReturncart = new List<CartItemDTO>();
            foreach (CartItemDTO objCartItemDTO in lstCartItems)
            {
                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                objItemMasterDTO = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objCartItemDTO.ItemGUID);
                if (!string.IsNullOrWhiteSpace(objCartItemDTO.BinName))
                {
                    BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(objCartItemDTO.ItemGUID ?? Guid.Empty, objCartItemDTO.BinName, objCartItemDTO.Room ?? 0, objCartItemDTO.CompanyID ?? 0, objCartItemDTO.CreatedBy ?? 0, false);
                    //BinMasterDTO objBinMasterDTO = new BinMasterDAL(base.DataBaseName).GetRecord(objCartItemDTO.BinName, objCartItemDTO.ItemGUID ?? Guid.Empty, objCartItemDTO.Room ?? 0, objCartItemDTO.CompanyID ?? 0, false, false);
                    //if (objBinMasterDTO != null)
                    //{
                    objCartItemDTO.BinId = objBinMasterDTO.ID;
                    objCartItemDTO.BinGUID = objBinMasterDTO.GUID;
                    //}
                    //else
                    //{
                    //    objBinMasterDTO = new BinMasterDTO();
                    //    objBinMasterDTO.BinNumber = objCartItemDTO.BinName;
                    //    objBinMasterDTO.ItemGUID = objCartItemDTO.ItemGUID;
                    //    objBinMasterDTO.CompanyID = objCartItemDTO.CompanyID;
                    //    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    //    objBinMasterDTO.CreatedBy = objCartItemDTO.CreatedBy;
                    //    objBinMasterDTO.GUID = Guid.NewGuid();
                    //    objBinMasterDTO.IsArchived = false;
                    //    objBinMasterDTO.IsDeleted = false;
                    //    objBinMasterDTO.Room = objCartItemDTO.Room;
                    //    objBinMasterDTO.IsStagingLocation = false;
                    //    objBinMasterDTO = new BinMasterDAL(base.DataBaseName).InsertBin(objBinMasterDTO);
                    //    objCartItemDTO.BinId = objBinMasterDTO.ID;
                    //    objCartItemDTO.BinGUID = objBinMasterDTO.GUID;
                    //}
                }
                else
                {
                    objCartItemDTO.BinId = null;
                    objCartItemDTO.BinGUID = null;
                }
                if (objItemMasterDTO.IsEnforceDefaultReorderQuantity ?? false)
                {
                    double newOrderQty = 0;
                    int devideval = 0;
                    double tempQty = objCartItemDTO.Quantity ?? 0;
                    double drq = objItemMasterDTO.DefaultReorderQuantity ?? 0;
                    if (tempQty > 0 && drq > 0)
                    {
                        if ((tempQty % drq) != 0)
                        {
                            devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                            newOrderQty = drq * devideval;
                            objCartItemDTO.EnforsedCartQuanity = true;
                        }
                        else
                        {
                            devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq)));
                            newOrderQty = drq * devideval;
                        }
                    }
                    objCartItemDTO.Quantity = newOrderQty;
                }
                if (objCartItemDTO.ID > 0)
                {
                    if (objCartItemDTO.IsOnlyFromItemUI)
                    {
                        objCartItemDTO.EditedFrom = "Web";
                        objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                }
                else
                {
                    objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCartItemDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objCartItemDTO.AddedFrom = "Web";
                    objCartItemDTO.EditedFrom = "Web";
                }

                CartItemDTO objCartItemDTO1 = this.SaveCartItem(objCartItemDTO, SessionUserId, EnterpriseId);
                lstReturncart.Add(objCartItemDTO1);
                // AutoCartUpdateByCode(objCartItemDTO.ItemGUID.Value, objCartItemDTO.LastUpdatedBy ?? 0, "web", "CartItemDAL>SaveCartItems");
                AutoCartUpdateByCode(objCartItemDTO.ItemGUID.Value, objCartItemDTO.LastUpdatedBy ?? 0, "web", "Replenish >> SaveCart", SessionUserId);
            }
            return lstReturncart;

        }

        public List<CartItemDTO> GetPagedRecordsCartListFromDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUID, List<long> SupplierIds, bool UserConsignmentAllowed, string ReplenishType, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            TotalCount = 0;
            CartItemDTO objCartItemDTO = new CartItemDTO();
            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                Connectionstring = base.DataBaseConnectionString;
            }

            if (Connectionstring == "")
            {
                return lstCartItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ReplenishTypes = null;
            string ItemSuppliers = null;
            string CartCreaters = null;
            string CartUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ItemUDF1 = null;
            string ItemUDF2 = null;
            string ItemUDF3 = null;
            string ItemUDF4 = null;
            string ItemUDF5 = null;
            string ItemUDF6 = null;
            string ItemUDF7 = null;
            string ItemUDF8 = null;
            string ItemUDF9 = null;
            string ItemUDF10 = null;
            string ItemLocations = null;
            string OnHandQuantity = null;
            string CategoryID = null;
            sortColumnName = sortColumnName.Replace("UpdatedByName", "Updated").Replace("CreatedByName", "Created");
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }
            if (String.IsNullOrEmpty(SearchTerm))
            {
                if (string.IsNullOrWhiteSpace(ReplenishType))
                {

                    dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedCartItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ReplenishTypes, ItemSuppliers, CartCreaters, CartUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, ItemGUID, strSupplierIds, UserConsignmentAllowed, ItemUDF1, ItemUDF2, ItemUDF3, ItemUDF4, ItemUDF5, ItemLocations, null, CategoryID, ItemUDF6, ItemUDF7, ItemUDF8, ItemUDF9, ItemUDF10);
                }
                else
                {
                    dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedCartItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ReplenishType, ItemSuppliers, CartCreaters, CartUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, ItemGUID, strSupplierIds, UserConsignmentAllowed, ItemUDF1, ItemUDF2, ItemUDF3, ItemUDF4, ItemUDF5, ItemLocations, null, CategoryID, ItemUDF6, ItemUDF7, ItemUDF8, ItemUDF9, ItemUDF10);
                }

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
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CartCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    CartUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    CategoryID = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[65]))
                {
                    string[] arrReplenishTypes = FieldsPara[65].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF1 = ItemUDF1 + supitem + "','";
                    }
                    ItemUDF1 = ItemUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[66]))
                {
                    string[] arrReplenishTypes = FieldsPara[66].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF2 = ItemUDF2 + supitem + "','";
                    }
                    ItemUDF2 = ItemUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[67]))
                {
                    string[] arrReplenishTypes = FieldsPara[67].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF3 = ItemUDF3 + supitem + "','";
                    }
                    ItemUDF3 = ItemUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[68]))
                {
                    string[] arrReplenishTypes = FieldsPara[68].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF4 = ItemUDF4 + supitem + "','";
                    }
                    ItemUDF4 = ItemUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[69]))
                {
                    string[] arrReplenishTypes = FieldsPara[69].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF5 = ItemUDF5 + supitem + "','";
                    }
                    ItemUDF5 = ItemUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[109]))
                {
                    string[] arrReplenishTypes = FieldsPara[109].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF6 = ItemUDF6 + supitem + "','";
                    }
                    ItemUDF6 = ItemUDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[110]))
                {
                    string[] arrReplenishTypes = FieldsPara[110].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF7 = ItemUDF7 + supitem + "','";
                    }
                    ItemUDF7 = ItemUDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[111]))
                {
                    string[] arrReplenishTypes = FieldsPara[111].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF8 = ItemUDF8 + supitem + "','";
                    }
                    ItemUDF8 = ItemUDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[112]))
                {
                    string[] arrReplenishTypes = FieldsPara[112].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF9 = ItemUDF9 + supitem + "','";
                    }
                    ItemUDF9 = ItemUDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[113]))
                {
                    string[] arrReplenishTypes = FieldsPara[113].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemUDF10 = ItemUDF10 + supitem + "','";
                    }
                    ItemUDF10 = ItemUDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[55]))
                {
                    ItemLocations = FieldsPara[55].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[24]))
                {
                    string[] arrReplenishTypes = FieldsPara[24].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ReplenishTypes = ReplenishTypes + supitem + "','";
                    }
                    ReplenishTypes = ReplenishTypes.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[89]))
                {
                    //if (FieldsPara[89] == "1")
                    //{
                    //    OnHandQuantity = ("Out of Stock");
                    //}
                    //else if (FieldsPara[89] == "2")
                    //{
                    //    OnHandQuantity = ("Below Critical");
                    //}
                    //else if (FieldsPara[89] == "3")
                    //{
                    //    OnHandQuantity = ("Below Min");
                    //}
                    //else if (FieldsPara[89] == "4")
                    //{
                    //    OnHandQuantity = ("Above Max");
                    //}

                    OnHandQuantity = FieldsPara[89].TrimEnd(',');
                }
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedCartItems", StartRowIndex, MaxRows, SearchTerm.Trim(), sortColumnName, ReplenishTypes, ItemSuppliers, CartCreaters, CartUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, ItemGUID, strSupplierIds, UserConsignmentAllowed, ItemUDF1, ItemUDF2, ItemUDF3, ItemUDF4, ItemUDF5, ItemLocations, OnHandQuantity, CategoryID, ItemUDF6, ItemUDF7, ItemUDF8, ItemUDF9, ItemUDF10);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedCartItems", StartRowIndex, MaxRows, SearchTerm.Trim(), sortColumnName, ReplenishTypes, ItemSuppliers, CartCreaters, CartUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, ItemGUID, strSupplierIds, UserConsignmentAllowed, ItemUDF1, ItemUDF2, ItemUDF3, ItemUDF4, ItemUDF5, ItemLocations, OnHandQuantity, CategoryID, ItemUDF6, ItemUDF7, ItemUDF8, ItemUDF9, ItemUDF10);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);

                    foreach (DataRow dr in dtCart.Rows)
                    {
                        long templong = 0;
                        //Guid? tempguid = null;
                        bool tempbool = false;
                        double tempdouble = 0;

                        objCartItemDTO = new CartItemDTO();
                        objCartItemDTO.CompanyID = CompanyID;
                        objCartItemDTO.Room = RoomID;
                        if (dtCart.Columns.Contains("Created"))
                        {
                            objCartItemDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtCart.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objCartItemDTO.CreatedBy = templong;
                        }
                        if (dtCart.Columns.Contains("Creater"))
                        {
                            objCartItemDTO.CreatedByName = Convert.ToString(dr["Creater"]);
                        }
                        if (dtCart.Columns.Contains("GUID"))
                        {
                            objCartItemDTO.GUID = (Guid)dr["GUID"];
                        }
                        if (dtCart.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objCartItemDTO.ID = templong;
                        }
                        if (dtCart.Columns.Contains("IsArchived"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempbool);
                            objCartItemDTO.IsArchived = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsDeleted"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempbool);
                            objCartItemDTO.IsDeleted = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsPurchase"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsPurchase"]), out tempbool);
                            objCartItemDTO.IsPurchase = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsAutomatedEntry"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsAutomatedEntry"]), out tempbool);
                            objCartItemDTO.IsAutoMatedEntry = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsTransfer"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsTransfer"]), out tempbool);
                            objCartItemDTO.IsTransfer = tempbool;
                        }
                        if (dtCart.Columns.Contains("ItemGUID"))
                        {
                            objCartItemDTO.ItemGUID = (Guid)dr["ItemGUID"];
                        }
                        //if (dtCart.Columns.Contains("ItemID"))
                        //{
                        //    Guid.TryParse(Convert.ToString(dr["ItemID"]), out tempguid);
                        //    objCartItemDTO.ItemGUID = templong;
                        //}
                        if (dtCart.Columns.Contains("ItemNumber"))
                        {
                            objCartItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCart.Columns.Contains("SerialNumberTracking"))
                        {
                            objCartItemDTO.SerialNumberTracking = Convert.ToBoolean(dr["SerialNumberTracking"]);
                        }
                        if (dtCart.Columns.Contains("LastUpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            objCartItemDTO.LastUpdatedBy = templong;
                        }
                        if (dtCart.Columns.Contains("Quantity"))
                        {
                            double.TryParse(Convert.ToString(dr["Quantity"]), out tempdouble);
                            objCartItemDTO.Quantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("ReplenishType"))
                        {
                            objCartItemDTO.ReplenishType = Convert.ToString(dr["ReplenishType"]);
                        }

                        if (dtCart.Columns.Contains("supplierID"))
                        {
                            long.TryParse(Convert.ToString(dr["supplierID"]), out templong);
                            objCartItemDTO.SupplierId = templong;
                        }
                        if (dtCart.Columns.Contains("SupplierName"))
                        {
                            objCartItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCart.Columns.Contains("UDF1"))
                        {
                            objCartItemDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCart.Columns.Contains("UDF2"))
                        {
                            objCartItemDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCart.Columns.Contains("UDF3"))
                        {
                            objCartItemDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCart.Columns.Contains("UDF4"))
                        {
                            objCartItemDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCart.Columns.Contains("UDF5"))
                        {
                            objCartItemDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtCart.Columns.Contains("Updated"))
                        {
                            objCartItemDTO.Updated = Convert.ToDateTime(dr["Updated"]);
                        }
                        if (dtCart.Columns.Contains("Updator"))
                        {
                            objCartItemDTO.UpdatedByName = Convert.ToString(dr["Updator"]);
                        }
                        if (dtCart.Columns.Contains("BinId"))
                        {
                            long.TryParse(Convert.ToString(dr["BinId"]), out templong);
                            objCartItemDTO.BinId = templong;
                        }
                        if (dtCart.Columns.Contains("BinName"))
                        {
                            objCartItemDTO.BinName = Convert.ToString(dr["BinName"]);
                        }
                        if (dtCart.Columns.Contains("CategoryName"))
                        {
                            objCartItemDTO.CategoryName = Convert.ToString(dr["CategoryName"]);
                        }
                        if (dtCart.Columns.Contains("CategoryID"))
                        {
                            long.TryParse(Convert.ToString(dr["CategoryID"]), out templong);
                            objCartItemDTO.CategoryID = templong;
                        }
                        if (dtCart.Columns.Contains("UOMID"))
                        {
                            long.TryParse(Convert.ToString(dr["UOMID"]), out templong);
                            objCartItemDTO.UOMID = templong;
                        }
                        if (dtCart.Columns.Contains("UnitName"))
                        {
                            objCartItemDTO.UnitName = Convert.ToString(dr["UnitName"]);
                        }
                        if (dtCart.Columns.Contains("PackingQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["PackingQuantity"]), out tempdouble);
                            objCartItemDTO.PackingQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("DefaultReorderQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["DefaultReorderQuantity"]), out tempdouble);
                            objCartItemDTO.DefaultReorderQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("OnOrderQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["OnOrderQuantity"]), out tempdouble);
                            objCartItemDTO.OnOrderQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("OnTransferQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["OnTransferQuantity"]), out tempdouble);
                            objCartItemDTO.OnTransferQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("RequisitionedQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["RequisitionedQuantity"]), out tempdouble);
                            objCartItemDTO.RequisitionedQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("CriticalQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["CriticalQuantity"]), out tempdouble);
                            objCartItemDTO.CriticalQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("MaximumQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["MaximumQuantity"]), out tempdouble);
                            objCartItemDTO.MaximumQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("MinimumQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["MinimumQuantity"]), out tempdouble);
                            objCartItemDTO.MinimumQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("OnHandQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["OnHandQuantity"]), out tempdouble);
                            objCartItemDTO.OnHandQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("SupplierName"))
                        {
                            objCartItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]); ;
                        }
                        if (dtCart.Columns.Contains("SupplierPartNo"))
                        {
                            objCartItemDTO.SupplierPartNo = Convert.ToString(dr["SupplierPartNo"]); ;
                        }
                        if (dtCart.Columns.Contains("ManufacturerName"))
                        {
                            objCartItemDTO.ManufacturerName = Convert.ToString(dr["ManufacturerName"]); ;
                        }
                        if (dtCart.Columns.Contains("ManufacturerNumber"))
                        {
                            objCartItemDTO.ManufacturerNumber = Convert.ToString(dr["ManufacturerNumber"]); ;
                        }
                        if (dtCart.Columns.Contains("ManufacturerID"))
                        {
                            long.TryParse(Convert.ToString(dr["ManufacturerID"]), out templong);
                            objCartItemDTO.ManufacturerID = templong;
                        }
                        if (dtCart.Columns.Contains("IsItemLevelMinMaxQtyRequired"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsItemLevelMinMaxQtyRequired"]), out tempbool);
                            objCartItemDTO.IsItemLevelMinMaxQtyRequired = tempbool;
                        }
                        if (dtCart.Columns.Contains("ItemLocationCriticalQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemLocationCriticalQuantity"]), out tempdouble);
                            objCartItemDTO.ItemLocationCriticalQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("ItemLocationMinimumQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemLocationMinimumQuantity"]), out tempdouble);
                            objCartItemDTO.ItemLocationMinimumQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("ItemLocationMaximumQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemLocationMaximumQuantity"]), out tempdouble);
                            objCartItemDTO.ItemLocationMaximumQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("Description"))
                        {
                            objCartItemDTO.Description = Convert.ToString(dr["Description"]); ;
                        }
                        if (dtCart.Columns.Contains("AddedFrom"))
                        {
                            objCartItemDTO.AddedFrom = Convert.ToString(dr["AddedFrom"]);
                        }
                        if (dtCart.Columns.Contains("EditedFrom"))
                        {
                            objCartItemDTO.EditedFrom = Convert.ToString(dr["EditedFrom"]);
                        }
                        if (dtCart.Columns.Contains("ReceivedOn"))
                        {
                            objCartItemDTO.ReceivedOn = Convert.ToDateTime(dr["ReceivedOn"]);
                        }
                        if (dtCart.Columns.Contains("ReceivedOnWeb"))
                        {
                            objCartItemDTO.ReceivedOnWeb = Convert.ToDateTime(dr["ReceivedOnWeb"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf1"))
                        {
                            objCartItemDTO.ItemUDF1 = Convert.ToString(dr["ItemUDf1"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf2"))
                        {
                            objCartItemDTO.ItemUDF2 = Convert.ToString(dr["ItemUDf2"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf3"))
                        {
                            objCartItemDTO.ItemUDF3 = Convert.ToString(dr["ItemUDf3"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf4"))
                        {
                            objCartItemDTO.ItemUDF4 = Convert.ToString(dr["ItemUDf4"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf5"))
                        {
                            objCartItemDTO.ItemUDF5 = Convert.ToString(dr["ItemUDf5"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDF6"))
                        {
                            objCartItemDTO.ItemUDF6 = Convert.ToString(dr["ItemUDF6"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf7"))
                        {
                            objCartItemDTO.ItemUDF7 = Convert.ToString(dr["ItemUDf7"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf8"))
                        {
                            objCartItemDTO.ItemUDF8 = Convert.ToString(dr["ItemUDf8"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf9"))
                        {
                            objCartItemDTO.ItemUDF9 = Convert.ToString(dr["ItemUDf9"]);
                        }
                        if (dtCart.Columns.Contains("ItemUDf10"))
                        {
                            objCartItemDTO.ItemUDF10 = Convert.ToString(dr["ItemUDf10"]);
                        }
                        if (dtCart.Columns.Contains("ItemLocationCustomerOwnedQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemLocationCustomerOwnedQuantity"]), out tempdouble);
                            objCartItemDTO.ItemLocationCustomerOwnedQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("ItemLocationConsignedQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemLocationConsignedQuantity"]), out tempdouble);
                            objCartItemDTO.ItemLocationConsignedQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("ItemLocationOH"))
                        {
                            double.TryParse(Convert.ToString(dr["ItemLocationOH"]), out tempdouble);
                            objCartItemDTO.ItemLocationOH = tempdouble;
                        }
                        if (dtCart.Columns.Contains("Cost"))
                        {
                            double.TryParse(Convert.ToString(dr["Cost"]), out tempdouble);
                            objCartItemDTO.Cost = tempdouble;
                        }
                        if (dtCart.Columns.Contains("SellPrice"))
                        {
                            double.TryParse(Convert.ToString(dr["SellPrice"]), out tempdouble);
                            objCartItemDTO.SellPrice = tempdouble;
                        }
                        if (dtCart.Columns.Contains("OrderUOM"))
                        {
                            objCartItemDTO.OrderUOM = Convert.ToString(dr["OrderUOM"]);
                        }
                        if (dtCart.Columns.Contains("OrderUOMValue"))
                        {
                            long.TryParse(Convert.ToString(dr["OrderUOMValue"]), out templong);
                            objCartItemDTO.OrderUOMValue = templong;
                        }
                        if (dtCart.Columns.Contains("IsAllowOrderCostuom"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsAllowOrderCostuom"]), out tempbool);
                            objCartItemDTO.IsAllowOrderCostuom = tempbool;
                        }
                        if (dtCart.Columns.Contains("OrderUOMQuantity"))
                        {
                            double.TryParse(Convert.ToString(dr["OrderUOMQuantity"]), out tempdouble);
                            objCartItemDTO.OrderUOMQuantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("CostUOM"))
                        {
                            objCartItemDTO.CostUOM = Convert.ToString(dr["CostUOM"]);
                        }
                        if (dtCart.Columns.Contains("IsItemOrderable"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsItemOrderable"]), out tempbool);
                            objCartItemDTO.IsItemOrderable = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsItemActive"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsItemActive"]), out tempbool);
                            objCartItemDTO.IsItemActive = tempbool;
                        }
                        if (dtCart.Columns.Contains("Markup"))
                        {
                            double.TryParse(Convert.ToString(dr["Markup"]), out tempdouble);
                            objCartItemDTO.Markup = tempdouble;
                        }
                        if (dtCart.Columns.Contains("LongDescription"))
                        {
                            objCartItemDTO.LongDescription = Convert.ToString(dr["LongDescription"]);
                        }


                        lstCartItems.Add(objCartItemDTO);
                    }
                }
                else
                    TotalCount = 0;
            }
            return lstCartItems;
        }

        public List<CartChartDTO> GetPurchaseCartForDashboardChart(out IEnumerable<CartChartDTO> SupplierList, out IEnumerable<CartChartDTO> CategoryList,
                                long StartRowIndex, long MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID,
                                bool IsArchived, bool IsDeleted, string ItemGUID, List<long> SupplierIds, bool UserConsignmentAllowed, string ReplenishType,
                                string SelectedSupplierIds, string SelectedCategoryIds)
        {
            List<CartChartDTO> lstCartItems = new List<CartChartDTO>();
            TotalCount = 0;
            CartItemDTO objCartItemDTO = new CartItemDTO();
            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                Connectionstring = base.DataBaseConnectionString;
            }

            if (Connectionstring == "")
            {
                SupplierList = null;
                CategoryList = null;
                return lstCartItems;
            }

            if (MaxRows < 1)
            {
                MaxRows = 10;
            }

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ReplenishTypes = null;
            sortColumnName = sortColumnName.Replace("UpdatedByName", "Updated").Replace("CreatedByName", "Created");
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            string strSelectedSupplierIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedSupplierIds) && !string.IsNullOrWhiteSpace(SelectedSupplierIds))
            {
                strSelectedSupplierIds = SelectedSupplierIds;
            }

            string strSelectedCategoryIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedCategoryIds) && !string.IsNullOrWhiteSpace(SelectedCategoryIds))
            {
                strSelectedCategoryIds = SelectedCategoryIds;
            }

            if (string.IsNullOrEmpty(SearchTerm))
            {
                if (string.IsNullOrWhiteSpace(ReplenishType))
                {
                    dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPurchaseCartForDashboardChart", StartRowIndex, MaxRows, sortColumnName, ReplenishTypes, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, strSelectedSupplierIds, strSelectedCategoryIds);
                }
                else
                {
                    dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPurchaseCartForDashboardChart", StartRowIndex, MaxRows, sortColumnName, ReplenishType, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, strSelectedSupplierIds, strSelectedCategoryIds);
                }
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                SupplierList = DataTableHelper.ToList<CartChartDTO>(dsCart.Tables[0]);
                CategoryList = DataTableHelper.ToList<CartChartDTO>(dsCart.Tables[1]);

                if (dsCart.Tables.Count > 2)
                {
                    lstCartItems = DataTableHelper.ToList<CartChartDTO>(dsCart.Tables[2]);

                    if (lstCartItems != null && lstCartItems.Count() > 0)
                    {
                        TotalCount = lstCartItems.ElementAt(0).TotalRecords;
                    }
                }
            }
            else
            {
                SupplierList = null;
                CategoryList = null;
            }

            return lstCartItems;
        }

        public List<CartItemDTO> GetPagedGroupedRecordsCartListFromDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            TotalCount = 0;
            CartItemDTO objCartItemDTO = new CartItemDTO();
            DataSet dsCart = new DataSet();
            //string Connectionstring = ConfigurationManager.ConnectionStrings["eTurnsConnection"].ConnectionString;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstCartItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ReplenishTypes = null;
            string ItemSuppliers = null;
            string CartCreaters = null;
            string CartUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;


            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedGroupCartItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ReplenishTypes, ItemSuppliers, CartCreaters, CartUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //&& ((Fields[1].Split('@')[24] == "") || (Fields[1].Split('@')[24].Split(',').ToList().Contains(t.ReplenishType)))
                //    && ((Fields[1].Split('@')[23] == "") || (Fields[1].Split('@')[23].Split(',').ToList().Contains(t.SupplierId.ToString())))
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                SearchTerm = string.Empty;
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CartCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    CartUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = Convert.ToString(TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone));
                    CreatedDateTo = Convert.ToString(TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone));
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //  UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //  UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = Convert.ToString(TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone));
                    UpdatedDateTo = Convert.ToString(TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone));
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    UDF1 = Convert.ToString(FieldsPara[4]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    UDF2 = Convert.ToString(FieldsPara[5]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    UDF3 = Convert.ToString(FieldsPara[6]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    UDF4 = Convert.ToString(FieldsPara[6]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    UDF5 = Convert.ToString(FieldsPara[7]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[24]))
                {
                    string[] arrReplenishTypes = FieldsPara[24].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ReplenishTypes = ReplenishTypes + supitem + "','";
                    }
                    ReplenishTypes = ReplenishTypes.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

                }
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedGroupCartItems", StartRowIndex, MaxRows, SearchTerm.Trim(), sortColumnName, ReplenishTypes, ItemSuppliers, CartCreaters, CartUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedGroupCartItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ReplenishTypes, ItemSuppliers, CartCreaters, CartUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dtCart.Rows)
                    {
                        long templong = 0;
                        bool tempbool = false;
                        double tempdouble = 0;

                        objCartItemDTO = new CartItemDTO();
                        objCartItemDTO.CompanyID = CompanyID;
                        objCartItemDTO.Room = RoomID;
                        if (dtCart.Columns.Contains("Created"))
                        {
                            objCartItemDTO.Created = Convert.ToDateTime(dr["Created"]);
                        }
                        if (dtCart.Columns.Contains("CreatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                            objCartItemDTO.CreatedBy = templong;
                        }
                        if (dtCart.Columns.Contains("Creater"))
                        {
                            objCartItemDTO.CreatedByName = Convert.ToString(dr["Creater"]);
                        }
                        if (dtCart.Columns.Contains("GUID"))
                        {
                            objCartItemDTO.GUID = (Guid)dr["GUID"];
                        }
                        if (dtCart.Columns.Contains("ID"))
                        {
                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                            objCartItemDTO.ID = templong;
                        }
                        if (dtCart.Columns.Contains("IsArchived"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempbool);
                            objCartItemDTO.IsArchived = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsDeleted"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempbool);
                            objCartItemDTO.IsDeleted = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsPurchase"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsPurchase"]), out tempbool);
                            objCartItemDTO.IsPurchase = tempbool;
                        }
                        if (dtCart.Columns.Contains("IsTransfer"))
                        {
                            bool.TryParse(Convert.ToString(dr["IsTransfer"]), out tempbool);
                            objCartItemDTO.IsTransfer = tempbool;
                        }
                        if (dtCart.Columns.Contains("ItemGUID"))
                        {
                            objCartItemDTO.ItemGUID = (Guid)dr["ItemGUID"];
                        }
                        //if (dtCart.Columns.Contains("ItemID"))
                        //{
                        //    long.TryParse(Convert.ToString(dr["ItemID"]), out templong);
                        //    objCartItemDTO.ItemID = templong;
                        //}
                        if (dtCart.Columns.Contains("ItemNumber"))
                        {
                            objCartItemDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtCart.Columns.Contains("LastUpdatedBy"))
                        {
                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                            objCartItemDTO.LastUpdatedBy = templong;
                        }
                        if (dtCart.Columns.Contains("Quantity"))
                        {
                            double.TryParse(Convert.ToString(dr["Quantity"]), out tempdouble);
                            objCartItemDTO.Quantity = tempdouble;
                        }
                        if (dtCart.Columns.Contains("ReplenishType"))
                        {
                            objCartItemDTO.ReplenishType = Convert.ToString(dr["ReplenishType"]);
                        }

                        if (dtCart.Columns.Contains("supplierID"))
                        {
                            long.TryParse(Convert.ToString(dr["supplierID"]), out templong);
                            objCartItemDTO.SupplierId = templong;
                        }
                        if (dtCart.Columns.Contains("SupplierName"))
                        {
                            objCartItemDTO.SupplierName = Convert.ToString(dr["SupplierName"]);
                        }
                        if (dtCart.Columns.Contains("UDF1"))
                        {
                            objCartItemDTO.UDF1 = Convert.ToString(dr["UDF1"]);
                        }
                        if (dtCart.Columns.Contains("UDF2"))
                        {
                            objCartItemDTO.UDF2 = Convert.ToString(dr["UDF2"]);
                        }
                        if (dtCart.Columns.Contains("UDF3"))
                        {
                            objCartItemDTO.UDF3 = Convert.ToString(dr["UDF3"]);
                        }
                        if (dtCart.Columns.Contains("UDF4"))
                        {
                            objCartItemDTO.UDF4 = Convert.ToString(dr["UDF4"]);
                        }
                        if (dtCart.Columns.Contains("UDF5"))
                        {
                            objCartItemDTO.UDF5 = Convert.ToString(dr["UDF5"]);
                        }
                        if (dtCart.Columns.Contains("Updated"))
                        {
                            objCartItemDTO.Updated = Convert.ToDateTime(dr["Updated"]);
                        }
                        if (dtCart.Columns.Contains("Updator"))
                        {
                            objCartItemDTO.UpdatedByName = Convert.ToString(dr["Updator"]);
                        }
                        lstCartItems.Add(objCartItemDTO);
                    }
                }
            }
            return lstCartItems;
        }

        public List<CartItemDTO> GetCartItemsByGuids(List<Guid> arrids, long RoomId, long CompanyId, bool WithSelection, List<long> SupplierIds)
        {
            try
            {
                string DbConnectionString = "";
                List<CartItemDTO> lstCart = null;
                eTurnsEntities context = null;

                if (!string.IsNullOrWhiteSpace(DbConnectionString))
                {
                    context = new eTurnsEntities(DbConnectionString);
                }
                else
                {
                    context = new eTurnsEntities(base.DataBaseEntityConnectionString);
                }
                string CSVGUIDS = string.Empty;

                if (arrids == null || arrids.Count < 1)
                {

                }
                else
                {
                    CSVGUIDS = string.Join(",", arrids);
                }

                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var params1 = new SqlParameter[] {
                new SqlParameter("@arrids", CSVGUIDS ?? (object)DBNull.Value),
                new SqlParameter("@RoomId", RoomId),
                new SqlParameter("@CompanyId", CompanyId),
                new SqlParameter("@SupplierIds",  strSupplierIds)
            };

                using (context)
                {
                    lstCart = (from ci in context.Database.SqlQuery<CartItemDTO>("exec [GetCartItemsByGuids] @arrids,@RoomId,@CompanyId,@SupplierIds", params1)
                               select new CartItemDTO
                               {
                                   ID = ci.ID,
                                   ItemNumber = ci.ItemNumber,
                                   ItemGUID = ci.ItemGUID,
                                   Quantity = ci.Quantity,
                                   Status = ci.Status,
                                   ReplenishType = ci.ReplenishType,
                                   IsKitComponent = ci.IsKitComponent,
                                   UDF1 = ci.UDF1,
                                   UDF2 = ci.UDF2,
                                   UDF3 = ci.UDF3,
                                   UDF4 = ci.UDF4,
                                   UDF5 = ci.UDF5,
                                   GUID = ci.GUID,
                                   Created = ci.Created,
                                   Updated = ci.Updated,
                                   CreatedBy = ci.CreatedBy,
                                   LastUpdatedBy = ci.LastUpdatedBy,
                                   IsDeleted = ci.IsDeleted,
                                   IsArchived = ci.IsArchived,
                                   CompanyID = ci.CompanyID,
                                   Room = ci.Room,
                                   CreatedByName = ci.CreatedByName,
                                   UpdatedByName = ci.UpdatedByName,
                                   RoomName = ci.RoomName,
                                   IsAutoMatedEntry = ci.IsAutoMatedEntry,
                                   SupplierId = ci.SupplierId,
                                   SupplierName = ci.SupplierName,
                                   BlanketPOID = ci.BlanketPOID,
                                   BlanketPONumber = ci.BlanketPONumber,
                                   BinId = ci.BinId,
                                   BinName = ci.BinName,
                                   MaxOrderSize = ci.MaxOrderSize,
                                   LeadTimeInDays = ci.LeadTimeInDays,
                                   CategoryID = ci.CategoryID,
                                   CategoryName = ci.CategoryName,
                                   SerialNumberTracking = ci.SerialNumberTracking,
                                   LotNumberTracking = ci.LotNumberTracking,
                                   DateCodeTracking = ci.DateCodeTracking,
                                   ItemType = ci.ItemType
                               }).ToList();

                    return lstCart;
                }
            }
            catch
            {

                throw;
            }
        }
        public bool UpdateSuggestedQtyOfItem(Guid ItemGUIDId, long RoomId, long CompanyId, long UserId, long SessionUserId, long EnterpriseId)
        {
            double TotalSuggestedQty = GetSuggestedQtyByReplenishType(ItemGUIDId, RoomId, CompanyId, "Purchase");
            double TotalSuggestedTransferQty = GetSuggestedQtyByReplenishType(ItemGUIDId, RoomId, CompanyId, "Transfer");

            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, ItemGUIDId);
            if (objItem != null)
            {
                objItem.LastUpdatedBy = UserId;
                objItem.Updated = DateTimeUtility.DateTimeNow;
                objItem.SuggestedOrderQuantity = TotalSuggestedQty;
                objItem.SuggestedTransferQuantity = TotalSuggestedTransferQty;
                objItem.WhatWhereAction = "Cart";
                objItemMasterDAL.Edit(objItem, SessionUserId, EnterpriseId);
            }
            return true;

        }
        public List<CartItemDTO> AutoCartUpdateByCode(Guid ItemGUID, long UserId, string CalledFrom, string calledFromFunctionName, long SessionUserId)
        {
            List<CartItemDTO> lstCarts = new List<CartItemDTO>();
            List<CartItemDTO> lstCartsChanges = new List<CartItemDTO>();
            List<CartItemDTO> lstCartsBC = new List<CartItemDTO>();
            List<CartItemDTO> lstCartsBM = new List<CartItemDTO>();
            long EnterpriseID = 0;
            long CompanyID = 0;
            long RoomID = 0;
            string RoomName = "";
            //string WhatWhereAction = GetCallStackData(calledFromFunctionName, Environment.StackTrace);
            string WhatWhereAction = calledFromFunctionName;

            //if (HttpContext.Current != null && HttpContext.Current.Session != null)
            //{
            //    long tempUSERID = 0;
            //    long.TryParse(Convert.ToString(HttpContext.Current.Session["UserID"]), out tempUSERID);
            //    if (tempUSERID > 0)
            //    {
            //        UserId = tempUSERID;
            //    }
            //}
            if (SessionUserId > 0 && UserId != SessionUserId)
            {
                UserId = SessionUserId;
            }
            var params1 = new SqlParameter[] { new SqlParameter("@itemguid", ItemGUID), new SqlParameter("@userid", UserId), new SqlParameter("@calledfrom", CalledFrom ?? (object)DBNull.Value), new SqlParameter("@calledfromFunctionName", WhatWhereAction ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstCarts = context.Database.SqlQuery<CartItemDTO>("exec [createautosot] @itemguid,@userid,@calledfrom,@calledfromFunctionName", params1).ToList();
            }


            lstCartsChanges = lstCarts.Where(t => (t.isstchanged ?? false) == true || (t.issochanged ?? false) == true).ToList();
            if (lstCartsChanges != null && lstCartsChanges.Count > 0)
            {
                EnterpriseID = lstCartsChanges.First().EnterpriseId;
                CompanyID = lstCartsChanges.First().CompanyID ?? 0;
                RoomID = lstCartsChanges.First().Room ?? 0;
                RoomName = lstCartsChanges.First().RoomName ?? "";
                if (HttpContext.Current != null && lstCarts != null && lstCarts.Count > 0)
                {
                    HttpRuntime.Cache["LastCalledEach"] = DateTime.UtcNow;
                    if (RedCircleStatic.SignalGroups == null)
                    {
                        RedCircleStatic.SignalGroups = new List<ECR>();
                    }
                    if (!RedCircleStatic.SignalGroups.Any(t => t.EID == EnterpriseID && t.CID == CompanyID && t.RID == RoomID))
                    {
                        RedCircleStatic.SignalGroups.Add(new ECR() { EID = EnterpriseID, CID = CompanyID, RID = RoomID, IsProcessed = false });
                    }
                }
                lstCartsBC = lstCartsChanges.Where(t => t.IsItemBelowCritcal == true || t.IsItemLocationBelowCritcal == true).ToList();
                lstCartsBM = lstCartsChanges.Where(t => t.IsItemBelowMinimum == true || t.IsItemLocationBelowMinimum == true).ToList();
                if (lstCartsBC != null && lstCartsBC.Count > 0)
                {
                    SendMailForSuggestedOrder(string.Empty, 0, 0, 0, RoomID, CompanyID, "Critical", RoomName, UserId, "SuggestedOrdersCritical", EnterpriseID, 0, string.Empty);
                }
                else if (lstCartsBM != null && lstCartsBM.Count > 0)
                {
                    SendMailForSuggestedOrder(string.Empty, 0, 0, 0, RoomID, CompanyID, "Minimum", RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseID, 0, string.Empty);
                }

            }
            return lstCarts;
        }
        public double GetSuggestedOrderQtyForBin(Guid ItemGUIDId, long BinID)
        {
            double TotalSuggestedQty = GetSuggestedQtyByReplenishType(ItemGUIDId, 0, 0, "Purchase", BinID);
            return TotalSuggestedQty;
        }
        public List<CartItemDTO> GetCartListForDailyEmail(long RoomId, long CompanyId)
        {
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@RoomID", RoomId), new SqlParameter("@ReplenishType", "Purchase") };

                lstCartItems = context.Database.SqlQuery<CartItemDTO>("exec [GetCartListForDailyEmail] @CompanyID,@RoomID,@ReplenishType", params1).ToList();
            }
            return lstCartItems;
        }

        public IList<OrderMasterDTO> GetOrdersByCartIds(string Ids, long RoomId, long CompanyId, long UserId, string DatabaseName, List<long> SupplierIds, string OrderLineItemUDF1, string OrderLineItemUDF2, string OrderLineItemUDF3, string OrderLineItemUDF4, string OrderLineItemUDF5, long EnterpriseId, string OrderItemQuantity)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            CartItemDAL objCartItemDAL = new CartItemDAL(DatabaseName);
            List<Guid> arrcartguids = new List<Guid>();
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            bool WithSelection;
            IList<OrderMasterDTO> lstOrders = new List<OrderMasterDTO>();
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(base.DataBaseName);
            SupplierMasterDTO objSupplierDTO = new SupplierMasterDTO();

            try
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    WithSelection = true;
                }
                else
                {
                    WithSelection = false;
                }
                lstCartItems = objCartItemDAL.GetCartItemsByGuids(arrcartguids, RoomId, CompanyId, WithSelection, SupplierIds);
                List<CartItemDTO> lstcartitemsgrped = (from ci in lstCartItems
                                                       where ci.ReplenishType == "Purchase"
                                                       group ci by new { ci.SupplierId, ci.SupplierName, ci.BlanketPOID, ci.MaxOrderSize, ci.BlanketPONumber } into groupedci
                                                       //group ci by new { ci.SupplierId, ci.SupplierName, ci.MaxOrderSize } into groupedci
                                                       select new CartItemDTO
                                                       {
                                                           SupplierId = groupedci.Key.SupplierId,
                                                           BlanketPOID = groupedci.Key.BlanketPOID,
                                                           MaxOrderSize = groupedci.Key.MaxOrderSize,
                                                           SupplierName = groupedci.Key.SupplierName,
                                                           BlanketPONumber = groupedci.Key.BlanketPONumber
                                                       }).ToList();
                string OrderLineItemIds = string.Empty;
                string OrderLineItemUDF1New = string.Empty;
                string OrderLineItemUDF2New = string.Empty;
                string OrderLineItemUDF3New = string.Empty;
                string OrderLineItemUDF4New = string.Empty;
                string OrderLineItemUDF5New = string.Empty;
                string CartItemQuantityNew = string.Empty;
                List<CartItemDTO> lstGroupedCartItems = new List<CartItemDTO>();
                int LineItemCount = 0;
                AutoOrderNumberGenerate objAutoNumber = null;

                foreach (var item in lstcartitemsgrped)
                {
                    OrderLineItemUDF1New = string.Empty;
                    OrderLineItemUDF2New = string.Empty;
                    OrderLineItemUDF3New = string.Empty;
                    OrderLineItemUDF4New = string.Empty;
                    OrderLineItemUDF5New = string.Empty;
                    CartItemQuantityNew = string.Empty;
                    lstGroupedCartItems = lstCartItems.Where(t => t.SupplierId == item.SupplierId && t.BlanketPOID == item.BlanketPOID && t.ReplenishType == "Purchase").ToList();
                    //lstGroupedCartItems = lstCartItems.Where(t => t.SupplierId == item.SupplierId && t.ReplenishType == "Purchase").ToList();
                    LineItemCount = lstGroupedCartItems.Count;

                    if (item.MaxOrderSize != null && item.MaxOrderSize > 0 && LineItemCount > item.MaxOrderSize)
                    {
                        decimal divfactor = (LineItemCount / (item.MaxOrderSize ?? 1));
                        decimal modfactor = (LineItemCount % (item.MaxOrderSize ?? 1));
                        int divfactorint = (int)divfactor;
                        if (modfactor > 0)
                        {
                            divfactorint = divfactorint + 1;
                        }
                        bool isSameSuppier = false;
                        for (int i = 1; i <= divfactorint; i++)
                        {
                            string orderNumber = string.Empty;
                            string orderNumberSorting = string.Empty;

                            objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierId);
                            bool isPOAutoSequenceBlanketOrder = false;
                            if (objSupplierDTO.POAutoSequence.HasValue && objSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 2)
                            {
                                isPOAutoSequenceBlanketOrder = true;
                            }
                            if ((item.BlanketPOID ?? 0) > 0 && !string.IsNullOrWhiteSpace(item.BlanketPONumber) && isPOAutoSequenceBlanketOrder)
                            {
                                orderNumber = item.BlanketPONumber;
                                objAutoNumber = new AutoOrderNumberGenerate() { OrderNumber = orderNumber };
                            }
                            else
                            {
                                objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextOrderNumber(RoomId, CompanyId, item.SupplierId, EnterpriseId, objAutoNumber);
                            }
                            //if ((item.BlanketPOID ?? 0) > 0 && !string.IsNullOrWhiteSpace(item.BlanketPONumber))
                            //{
                            //    orderNumber = item.BlanketPONumber;
                            //    objAutoNumber = new AutoOrderNumberGenerate() { OrderNumber = orderNumber };
                            //}
                            //else
                            //{
                            //objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextOrderNumber(RoomId, CompanyId, item.SupplierId, EnterpriseId, objAutoNumber, isSameSuppier);
                            //if (objAutoNumber.IsBlanketPO)
                            //    orderNumber = objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTimeUtility.DateTimeNow) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTimeUtility.DateTimeNow) >= DateTimeUtility.DateTimeNow).FirstOrDefault().BlanketPO;
                            //else
                            orderNumber = objAutoNumber.OrderNumber;

                            orderNumberSorting = objAutoNumber.OrderNumberForSorting;
                            isSameSuppier = true;
                            //}
                            if (orderNumber == null || string.IsNullOrWhiteSpace(orderNumber))
                            {
                                orderNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
                            }
                            int ReleaseNo = 1;
                            if (!string.IsNullOrWhiteSpace(orderNumber))
                            {
                                OrderMasterDAL objOrderDAL = new OrderMasterDAL(DatabaseName);
                                ReleaseNo = objOrderDAL.GetNextReleaseNumber(orderNumber, null, RoomId, CompanyId);

                                //IEnumerable<OrderMasterDTO> objOrderList = objOrderDAL.GetAllRecords(RoomId, CompanyId, false, false, OrderType.Order);
                                ////ReleaseNo = objOrderList.Where(x => x.OrderNumber == orderNumber).Count() + 1;
                                //if (objOrderList != null && objOrderList.Count() > 0)
                                //    ReleaseNo = objOrderList.Max(x => int.Parse(x.ReleaseNumber)) + 1;
                            }

                            if (string.IsNullOrEmpty(orderNumberSorting))
                                orderNumberSorting = orderNumber;

                            int DReqOrderDays = 0;
                            //objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierId);
                            if (objSupplierDTO != null)
                            {
                                DReqOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);
                                if (objSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 1)
                                {
                                    if (!string.IsNullOrWhiteSpace(objSupplierDTO.POAutoNrReleaseNumber) && objSupplierDTO.NextOrderNo.Trim() == orderNumber.Trim())
                                    {
                                        ReleaseNo = Convert.ToInt32(objSupplierDTO.POAutoNrReleaseNumber) + 1;
                                    }

                                }
                            }


                            OrderMasterDTO objDTO = new OrderMasterDTO()
                            {
                                OrderType = (int)OrderType.Order,
                                RequiredDate = CurrentDateTime.AddDays(DReqOrderDays),
                                OrderNumber = orderNumber,
                                OrderStatus = (int)OrderStatus.UnSubmitted,
                                ReleaseNumber = ReleaseNo.ToString(),
                                LastUpdated = DateTime.UtcNow,
                                Created = DateTime.UtcNow,
                                Supplier = item.SupplierId,
                                SupplierName = item.SupplierName,
                                CreatedBy = UserId,
                                LastUpdatedBy = UserId,
                                CompanyID = CompanyId,
                                Room = RoomId,
                                OrderDate = CurrentDateTime,
                                AutoOrderNumber = objAutoNumber,
                                IsBlanketOrder = objAutoNumber.IsBlanketPO,
                                OrderLineItemsIds = OrderLineItemIds,
                                OrderNumber_ForSorting = orderNumberSorting,
                                RequiredDateString = string.Empty,
                                OrderLineItemUDF1 = OrderLineItemUDF1,
                                OrderLineItemUDF2 = OrderLineItemUDF2,
                                OrderLineItemUDF3 = OrderLineItemUDF3,
                                OrderLineItemUDF4 = OrderLineItemUDF4,
                                OrderLineItemUDF5 = OrderLineItemUDF5,
                                CartQuantityString = OrderItemQuantity
                            };
                            if (i == divfactorint)
                            {
                                objDTO.NoOfLineItems = LineItemCount - ((i - 1) * (int)item.MaxOrderSize);
                                OrderLineItemIds = string.Join(",", lstGroupedCartItems.Skip((i - 1) * ((int)item.MaxOrderSize)).Take((objDTO.NoOfLineItems ?? 0)).Select(t => t.GUID));
                                objDTO.OrderLineItemsIds = OrderLineItemIds;

                                if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                                {
                                    string[] AllGuid = Ids.Split(',').ToArray();
                                    string[] udf1 = OrderLineItemUDF1.Split(',').ToArray();
                                    string[] udf2 = OrderLineItemUDF2.Split(',').ToArray();
                                    string[] udf3 = OrderLineItemUDF3.Split(',').ToArray();
                                    string[] udf4 = OrderLineItemUDF4.Split(',').ToArray();
                                    string[] udf5 = OrderLineItemUDF5.Split(',').ToArray();
                                    string[] cartitemqty = OrderItemQuantity.Split(',').ToArray();
                                    foreach (string guid in OrderLineItemIds.Split(','))
                                    {
                                        //AllGuid.Where(x => x.v)
                                        int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                        if (udf1.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF1New))
                                            {
                                                OrderLineItemUDF1New = udf1[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF1New = OrderLineItemUDF1New + "," + udf1[Index];
                                            }
                                        }
                                        if (udf2.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF2New))
                                            {
                                                OrderLineItemUDF2New = udf2[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF2New = OrderLineItemUDF2New + "," + udf2[Index];
                                            }
                                        }
                                        if (udf3.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF3New))
                                            {
                                                OrderLineItemUDF3New = udf3[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF3New = OrderLineItemUDF3New + "," + udf3[Index];
                                            }
                                        }
                                        if (udf4.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF4New))
                                            {
                                                OrderLineItemUDF4New = udf4[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF4New = OrderLineItemUDF4New + "," + udf4[Index];
                                            }
                                        }
                                        if (udf5.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF5New))
                                            {
                                                OrderLineItemUDF5New = udf5[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF5New = OrderLineItemUDF5New + "," + udf5[Index];
                                            }
                                        }
                                        if (cartitemqty.Length > Index)
                                        {
                                            CartItemQuantityNew = cartitemqty[Index];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                objDTO.NoOfLineItems = (int)item.MaxOrderSize;
                                OrderLineItemIds = string.Join(",", lstGroupedCartItems.Skip((i - 1) * ((int)item.MaxOrderSize)).Take(((int)item.MaxOrderSize)).Select(t => t.GUID));
                                objDTO.OrderLineItemsIds = OrderLineItemIds;

                                if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                                {
                                    string[] AllGuid = Ids.Split(',').ToArray();
                                    string[] udf1 = OrderLineItemUDF1.Split(',').ToArray();
                                    string[] udf2 = OrderLineItemUDF2.Split(',').ToArray();
                                    string[] udf3 = OrderLineItemUDF3.Split(',').ToArray();
                                    string[] udf4 = OrderLineItemUDF4.Split(',').ToArray();
                                    string[] udf5 = OrderLineItemUDF5.Split(',').ToArray();
                                    string[] cartitemqty = OrderItemQuantity.Split(',').ToArray();
                                    foreach (string guid in OrderLineItemIds.Split(','))
                                    {
                                        //AllGuid.Where(x => x.v)
                                        int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                        if (udf1.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF1New))
                                            {
                                                OrderLineItemUDF1New = udf1[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF1New = OrderLineItemUDF1New + "," + udf1[Index];
                                            }
                                        }
                                        if (udf2.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF2New))
                                            {
                                                OrderLineItemUDF2New = udf2[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF2New = OrderLineItemUDF2New + "," + udf2[Index];
                                            }
                                        }
                                        if (udf3.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF3New))
                                            {
                                                OrderLineItemUDF3New = udf3[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF3New = OrderLineItemUDF3New + "," + udf3[Index];
                                            }
                                        }
                                        if (udf4.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF4New))
                                            {
                                                OrderLineItemUDF4New = udf4[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF4New = OrderLineItemUDF4New + "," + udf4[Index];
                                            }
                                        }
                                        if (udf5.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF5New))
                                            {
                                                OrderLineItemUDF5New = udf5[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF5New = OrderLineItemUDF5New + "," + udf5[Index];
                                            }
                                        }
                                        if (cartitemqty.Length > Index)
                                        {
                                            CartItemQuantityNew = cartitemqty[Index];
                                        }
                                    }
                                }

                            }
                            if (objDTO.IsBlanketOrder)
                            {
                                IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                                                                                              where x != null
                                                                                              && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.Now).ToShortDateString()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                              && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.Now).ToShortDateString()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                              select x);
                                if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                                {
                                    objDTO.BlanketOrderNumberID = objSuppBlnkPOList.FirstOrDefault().ID;
                                }

                            }
                            objDTO.OrderLineItemUDF1 = OrderLineItemUDF1New;
                            objDTO.OrderLineItemUDF2 = OrderLineItemUDF2New;
                            objDTO.OrderLineItemUDF3 = OrderLineItemUDF3New;
                            objDTO.OrderLineItemUDF4 = OrderLineItemUDF4New;
                            objDTO.OrderLineItemUDF5 = OrderLineItemUDF5New;
                            objDTO.CartQuantityString = CartItemQuantityNew;
                            objDTO.IsOrderSelected = true;
                            lstOrders.Add(objDTO);
                        }

                    }
                    else
                    {
                        string orderNumber = string.Empty;
                        string orderNumberSorting = string.Empty;
                        objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierId);
                        bool isPOAutoSequenceBlanketOrder = false;
                        if (objSupplierDTO.POAutoSequence.HasValue && objSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 2)
                        {
                            isPOAutoSequenceBlanketOrder = true;
                        }
                        if ((item.BlanketPOID ?? 0) > 0 && !string.IsNullOrWhiteSpace(item.BlanketPONumber) && isPOAutoSequenceBlanketOrder)
                        {
                            orderNumber = item.BlanketPONumber;
                            objAutoNumber = new AutoOrderNumberGenerate() { OrderNumber = orderNumber };
                        }
                        else
                        {
                            objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextOrderNumber(RoomId, CompanyId, item.SupplierId, EnterpriseId, objAutoNumber);
                        }
                        //else
                        //{

                        //if (objAutoNumber.IsBlanketPO && objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTimeUtility.DateTimeNow) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTimeUtility.DateTimeNow) >= DateTimeUtility.DateTimeNow).Any())
                        //    orderNumber = objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTimeUtility.DateTimeNow) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTimeUtility.DateTimeNow) >= DateTimeUtility.DateTimeNow).FirstOrDefault().BlanketPO;
                        //else
                        orderNumber = objAutoNumber.OrderNumber;

                        orderNumberSorting = objAutoNumber.OrderNumberForSorting;

                        //}
                        if (orderNumber == null || string.IsNullOrWhiteSpace(orderNumber))
                        {
                            orderNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
                        }
                        if (string.IsNullOrEmpty(orderNumberSorting))
                            orderNumberSorting = orderNumber;

                        int ReleaseNo = 1;

                        if (!string.IsNullOrWhiteSpace(orderNumber))
                        {
                            OrderMasterDAL objOrderDAL = new OrderMasterDAL(DatabaseName);
                            ReleaseNo = objOrderDAL.GetNextReleaseNumber(orderNumber, null, RoomId, CompanyId);
                        }
                        OrderLineItemIds = OrderLineItemIds = string.Join(",", lstGroupedCartItems.Select(t => t.GUID));
                        if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                        {
                            string[] AllGuid = Ids.Split(',').ToArray();
                            string[] udf1 = OrderLineItemUDF1.Split(',').ToArray();
                            string[] udf2 = OrderLineItemUDF2.Split(',').ToArray();
                            string[] udf3 = OrderLineItemUDF3.Split(',').ToArray();
                            string[] udf4 = OrderLineItemUDF4.Split(',').ToArray();
                            string[] udf5 = OrderLineItemUDF5.Split(',').ToArray();
                            string[] cartitemqty = OrderItemQuantity.Split(',').ToArray();
                            foreach (string guid in OrderLineItemIds.Split(','))
                            {
                                //AllGuid.Where(x => x.v)
                                int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                if (udf1.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF1New))
                                    {
                                        OrderLineItemUDF1New = udf1[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF1New = OrderLineItemUDF1New + "," + udf1[Index];
                                    }
                                }
                                if (udf2.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF2New))
                                    {
                                        OrderLineItemUDF2New = udf2[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF2New = OrderLineItemUDF2New + "," + udf2[Index];
                                    }
                                }
                                if (udf3.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF3New))
                                    {
                                        OrderLineItemUDF3New = udf3[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF3New = OrderLineItemUDF3New + "," + udf3[Index];
                                    }
                                }
                                if (udf4.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF4New))
                                    {
                                        OrderLineItemUDF4New = udf4[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF4New = OrderLineItemUDF4New + "," + udf4[Index];
                                    }
                                }
                                if (udf5.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF5New))
                                    {
                                        OrderLineItemUDF5New = udf5[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF5New = OrderLineItemUDF5New + "," + udf5[Index];
                                    }
                                }
                                if (cartitemqty.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(CartItemQuantityNew))
                                    {
                                        CartItemQuantityNew = cartitemqty[Index];
                                    }
                                    else
                                    {
                                        CartItemQuantityNew = CartItemQuantityNew + "," + cartitemqty[Index];
                                    }
                                }
                            }
                        }
                        //OrderLineItemUDF1  = string.Join(",", lstGroupedCartItems.Select(t => t.UDF1));
                        //OrderLineItemUDF2 = string.Join(",", lstGroupedCartItems.Select(t => t.UDF2));
                        //OrderLineItemUDF3 = string.Join(",", lstGroupedCartItems.Select(t => t.UDF3));
                        //OrderLineItemUDF4 = string.Join(",", lstGroupedCartItems.Select(t => t.UDF4));
                        //OrderLineItemUDF5 = string.Join(",", lstGroupedCartItems.Select(t => t.UDF5));
                        int DefaultRequiredOrderDays = 0;
                        //objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierId);
                        if (objSupplierDTO != null)
                        {
                            DefaultRequiredOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);

                            if (objSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 1)
                            {
                                if (!string.IsNullOrWhiteSpace(objSupplierDTO.POAutoNrReleaseNumber) && objSupplierDTO.NextOrderNo.Trim() == orderNumber.Trim())
                                {
                                    ReleaseNo = Convert.ToInt32(objSupplierDTO.POAutoNrReleaseNumber) + 1;
                                }

                            }
                        }


                        OrderMasterDTO objDTO = new OrderMasterDTO()
                        {
                            RequiredDate = CurrentDateTime.AddDays(DefaultRequiredOrderDays),
                            OrderNumber = orderNumber,
                            OrderStatus = (int)OrderStatus.UnSubmitted,
                            ReleaseNumber = ReleaseNo.ToString(),
                            LastUpdated = DateTime.UtcNow,
                            Created = DateTime.UtcNow,
                            Supplier = item.SupplierId,
                            SupplierName = item.SupplierName,
                            CreatedBy = UserId,
                            LastUpdatedBy = UserId,
                            CompanyID = CompanyId,
                            Room = RoomId,
                            OrderDate = CurrentDateTime,
                            AutoOrderNumber = objAutoNumber,
                            IsBlanketOrder = objAutoNumber.IsBlanketPO,
                            NoOfLineItems = LineItemCount,
                            OrderLineItemsIds = OrderLineItemIds,
                            OrderNumber_ForSorting = orderNumberSorting,
                            RequiredDateString = string.Empty,
                            OrderLineItemUDF1 = OrderLineItemUDF1New,
                            OrderLineItemUDF2 = OrderLineItemUDF2New,
                            OrderLineItemUDF3 = OrderLineItemUDF3New,
                            OrderLineItemUDF4 = OrderLineItemUDF4New,
                            OrderLineItemUDF5 = OrderLineItemUDF5New,
                            CartQuantityString = CartItemQuantityNew
                        };

                        if (objDTO.IsBlanketOrder)
                        {

                            IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                                                                                          where x != null
                                                                                          && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.Now).ToShortDateString()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                          && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.Now).ToShortDateString()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                          select x);
                            if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                            {
                                objDTO.BlanketOrderNumberID = objSuppBlnkPOList.FirstOrDefault().ID;
                            }

                        }
                        objDTO.IsOrderSelected = true;
                        lstOrders.Add(objDTO);
                    }
                }

                return lstOrders;
            }
            catch
            {
                throw;
            }

        }
        public IList<TransferMasterDTO> GetTransfersByCartIdsNew(string Ids, long RoomId, long CompanyId, long UserId, string DatabaseName, short SubmissionMethod, long EnterpriseID, string TransferItemQuantity)
        {
            IList<TransferMasterDTO> lstTransfers = new List<TransferMasterDTO>();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
                CartItemDAL objCartItemDAL = new CartItemDAL(DatabaseName);
                List<Guid> arrcartguids = new List<Guid>();
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
                //bool WithSelection;
                TransferMasterDAL objTransferMasterDAL = new TransferMasterDAL(base.DataBaseName);
                //RoomDTO objRoom = objRoomDAL.GetRoomByIDPlain(RoomId);
                string columnList = "ID,RoomName,ReplineshmentRoom";
                RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");

                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    //  WithSelection = true;
                }

                AutoOrderNumberGenerate objAutoNumber = null;
                AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(base.DataBaseName);
                objAutoNumber = objAutoSeqDAL.GetNextTransferNumber(RoomId, CompanyId, EnterpriseID);
                string autoNumber = objAutoNumber.OrderNumber;

                TransferMasterDTO objDTO = new TransferMasterDTO()
                {
                    RequireDate = CurrentDateTime.AddDays(1),
                    TransferNumber = autoNumber,
                    TransferStatus = SubmissionMethod == 2 ? (int)TransferStatus.Approved : (int)TransferStatus.UnSubmitted,
                    RequestType = (int)RequestType.In,
                    RequestingRoomID = RoomId,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    CreatedBy = UserId,
                    RoomID = RoomId,
                    CompanyID = CompanyId,
                    ReplenishingRoomID = objRoom.ReplineshmentRoom ?? 0,
                    GUID = Guid.NewGuid(),
                    AddedFrom = "Web",
                    EditedFrom = "Web",
                    ReceivedOn = DateTimeUtility.DateTimeNow,
                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    NoOfItems = arrcartguids.Count,
                    TransferLineItemsIds = Ids,
                    TransferQuantityString = TransferItemQuantity
                };
                lstTransfers.Add(objDTO);
                return lstTransfers;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IList<TransferMasterDTO> GetTransfersByCartIdsUsingReplinish(string Ids, long RoomId, long CompanyId, long UserId, string DatabaseName, List<long> SupplierIds, short SubmissionMethod, long EnterpriseId, long ReplinishRoomID, DateTime RequiredDate, string TransferNumber, int TransferStatus, string StagingName, string Comment, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, long SessionUserId, string TransferQuantityString)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            CartItemDAL objCartItemDAL = new CartItemDAL(DatabaseName);
            List<Guid> arrcartguids = new List<Guid>();
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            IEnumerable<ItemMasterDTO> ReplinishItems;
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            bool WithSelection;
            TransferMasterDAL objTransferMasterDAL = new TransferMasterDAL(base.DataBaseName);
            //RoomDTO objRoom = objRoomDAL.GetRoomByIDPlain(RoomId);
            IList<TransferMasterDTO> lstTransfers = new List<TransferMasterDTO>();

            if (ReplinishRoomID > 0)
            {
                CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,CompanyID";
                RoomDTO objReplinishRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + ReplinishRoomID.ToString() + "", "");
                if (objReplinishRoom != null && objReplinishRoom.ID > 0)
                    ReplinishItems = objItemDAL.GetAllItemsWithoutJoins(objReplinishRoom.ID, objReplinishRoom.CompanyID.GetValueOrDefault(0), false, false, null);
                else
                    ReplinishItems = null;

                if (ReplinishItems == null || ReplinishItems.Count() <= 0)
                    return lstTransfers;
                else
                {
                    ReplinishItems = ReplinishItems.Where(e => e.IsActive == true && e.ItemType != 4).ToList();
                }

            }
            else
            {
                return lstTransfers;
            }
            List<string> ReplinishingItemNumbers = ReplinishItems.Select(x => x.ItemNumber).ToList();
            try
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    WithSelection = true;
                }
                else
                {
                    WithSelection = false;
                }

                lstCartItems = objCartItemDAL.GetCartItemsByGuids(arrcartguids, RoomId, CompanyId, WithSelection, SupplierIds);
                List<CartItemDTO> lstcartitemsgrped = (from ci in lstCartItems
                                                       join cm in ReplinishItems on new { ci.ItemNumber, ci.SerialNumberTracking, ci.DateCodeTracking, ci.LotNumberTracking, ci.ItemType } equals new { cm.ItemNumber, cm.SerialNumberTracking, cm.DateCodeTracking, cm.LotNumberTracking, cm.ItemType }
                                                       where ci.ReplenishType == "Transfer"
                                                       //&& ReplinishingItemNumbers.Contains(ci.ItemNumber)
                                                       select ci).ToList();
                string OrderLineItemIds = string.Empty;
                List<CartItemDTO> lstGroupedCartItems = new List<CartItemDTO>();
                int totalLineCount = lstcartitemsgrped.Count;
                if (totalLineCount > 0)
                {
                    AutoOrderNumberGenerate objAutoNumber = null;
                    AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(base.DataBaseName);
                    //string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextAutoNumberByModule("NextTransferNo", SessionHelper.RoomID, SessionHelper.CompanyID);
                    objAutoNumber = objAutoSeqDAL.GetNextTransferNumber(RoomId, CompanyId, EnterpriseId);

                    string autoNumber = objAutoNumber.OrderNumber;
                    //string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextAutoNumberByModule("NextTransferNo", RoomId, CompanyId);
                    TransferMasterDTO objDTO = new TransferMasterDTO()
                    {
                        RequireDate = RequiredDate,//CurrentDateTime.AddDays(1),
                        TransferNumber = TransferNumber,// autoNumber,
                        TransferStatus = (int)eTurns.DTO.TransferStatus.UnSubmitted,// TransferStatus,//SubmissionMethod == 2 ? (int)TransferStatus.Approved : (int)TransferStatus.UnSubmitted,
                        RequestType = (int)RequestType.In,
                        RequestingRoomID = RoomId,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        CreatedBy = UserId,
                        LastUpdatedBy = UserId,
                        RoomID = RoomId,
                        CompanyID = CompanyId,
                        ReplenishingRoomID = ReplinishRoomID,
                        GUID = Guid.NewGuid(),
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                        TransferLineItemsIds = Ids,
                        NoOfItems = totalLineCount,
                        Comment = Comment,
                        UDF1 = UDF1,
                        UDF2 = UDF2,
                        UDF3 = UDF3,
                        UDF4 = UDF4,
                        UDF5 = UDF5
                    };
                    objDTO = objTransferMasterDAL.InsertTransfer(objDTO);

                    if (objDTO.ID > 0)
                    {
                        TransferDetailDAL objTransferDetailDAL = new TransferDetailDAL(base.DataBaseName);
                        string cartguids = string.Empty;
                        string[] AllGuid = Ids.Split(',').ToArray();
                        string[] transferQuantity = TransferQuantityString.Split(',').ToArray();
                        foreach (var cartitem in lstcartitemsgrped)
                        {
                            int Index = Array.FindIndex(AllGuid, row => row.Contains(cartitem.GUID.ToString()));
                            double? cartItemQty = cartitem.Quantity;
                            if (transferQuantity != null && transferQuantity.Length > 0)
                            {
                                try
                                {
                                    cartItemQty = Convert.ToDouble(transferQuantity[Index]);
                                }
                                catch (Exception)
                                {
                                    cartItemQty = cartitem.Quantity;
                                }
                            }
                            TransferDetailDTO objTransferDetailDTO = new TransferDetailDTO();
                            objTransferDetailDTO.Bin = cartitem.BinId;
                            objTransferDetailDTO.CompanyID = CompanyId;
                            objTransferDetailDTO.Created = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.CreatedBy = UserId;
                            objTransferDetailDTO.FulFillQuantity = cartItemQty;
                            objTransferDetailDTO.GUID = Guid.NewGuid();
                            objTransferDetailDTO.ID = cartitem.ID;
                            objTransferDetailDTO.IntransitQuantity = 0;
                            objTransferDetailDTO.IsArchived = false;
                            objTransferDetailDTO.IsDeleted = false;
                            objTransferDetailDTO.ItemGUID = cartitem.ItemGUID ?? Guid.Empty;
                            objTransferDetailDTO.ItemNumber = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, cartitem.ItemGUID ?? Guid.Empty).ItemNumber;
                            objTransferDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.LastUpdatedBy = UserId;
                            objTransferDetailDTO.ReceivedQuantity = 0;
                            objTransferDetailDTO.RequestedQuantity = cartItemQty ?? 0;
                            //objTransferDetailDTO.RequiredDate = CurrentDateTime.AddDays(7);
                            objTransferDetailDTO.RequiredDate = cartitem.LeadTimeInDays.GetValueOrDefault(0) > 0 ? CurrentDateTime.AddDays(cartitem.LeadTimeInDays.GetValueOrDefault(0)) : CurrentDateTime.AddDays(1);
                            objTransferDetailDTO.Room = RoomId;
                            objTransferDetailDTO.ShippedQuantity = 0;
                            objTransferDetailDTO.TransferGUID = objDTO.GUID;
                            objTransferDetailDTO.AddedFrom = "Web";
                            objTransferDetailDTO.EditedFrom = "Web";
                            objTransferDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.ID = objTransferDetailDAL.Insert(objTransferDetailDTO, SessionUserId, false, EnterpriseId);
                            if (objDTO.TransferDetailList == null)
                            {
                                objDTO.TransferDetailList = new List<TransferDetailDTO>();
                            }

                            objDTO.TransferDetailList.Add(objTransferDetailDTO);
                            cartguids += cartitem.GUID.ToString() + ",";

                            // MaintainTransaction History for Cart To Tranfer
                            if (objTransferDetailDTO != null)
                            {
                                objCartItemDAL.InsertCartQuoteTransitionDetail(cartitem.GUID, null, null, objTransferDetailDTO.GUID, (int)TransactionConversionType.CarttoTransfer, UserId);
                            }
                        }
                        DeleteRecords(cartguids, UserId, CompanyId, EnterpriseId, SessionUserId);
                    }
                    if (TransferStatus == (int)eTurns.DTO.TransferStatus.Transmitted || TransferStatus == (int)eTurns.DTO.TransferStatus.Submitted)
                    {
                        TransferMasterDAL objTransfer = new TransferMasterDAL(base.DataBaseName);
                        objDTO.TransferStatus = TransferStatus;
                        objTransfer.Edit(objDTO);
                    }
                    objDTO.NoOfItems = totalLineCount;
                    lstTransfers.Add(objDTO);
                }

                return lstTransfers;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IList<TransferMasterDTO> GetTransfersByCartIds(string Ids, long RoomId, long CompanyId, long UserId, string DatabaseName, List<long> SupplierIds, short SubmissionMethod, long EnterpriseId, long SessionUserId)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            CartItemDAL objCartItemDAL = new CartItemDAL(DatabaseName);
            List<Guid> arrcartguids = new List<Guid>();
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            IEnumerable<ItemMasterDTO> ReplinishItems;
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            bool WithSelection;
            TransferMasterDAL objTransferMasterDAL = new TransferMasterDAL(base.DataBaseName);
            string columnList = "ID,RoomName,ReplineshmentRoom";
            RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");
            IList<TransferMasterDTO> lstTransfers = new List<TransferMasterDTO>();
            if (objRoom.ReplineshmentRoom.GetValueOrDefault(0) > 0)
            {
                string _columnList = "ID,RoomName,ReplineshmentRoom,CompanyID";
                RoomDTO objReplinishRoom = objCommonDAL.GetSingleRecord<RoomDTO>(_columnList, "Room", "ID = " + objRoom.ReplineshmentRoom.GetValueOrDefault(0).ToString() + "", "");
                //RoomDTO objReplinishRoom = objRoomDAL.GetRoomByIDPlain(objRoom.ReplineshmentRoom.GetValueOrDefault(0));
                ReplinishItems = objItemDAL.GetAllItemsWithoutJoins(objReplinishRoom.ID, objReplinishRoom.CompanyID.GetValueOrDefault(0), false, false, null);

                if (ReplinishItems == null || ReplinishItems.Count() <= 0)
                    return lstTransfers;
                else
                {
                    ReplinishItems = ReplinishItems.Where(e => e.IsActive == true && e.ItemType != 4).ToList();
                }
            }
            else
            {
                return lstTransfers;
            }
            List<string> ReplinishingItemNumbers = ReplinishItems.Select(x => x.ItemNumber).ToList();
            try
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    WithSelection = true;
                }
                else
                {
                    WithSelection = false;
                }
                lstCartItems = objCartItemDAL.GetCartItemsByGuids(arrcartguids, RoomId, CompanyId, WithSelection, SupplierIds);
                List<CartItemDTO> lstcartitemsgrped = (from ci in lstCartItems
                                                       join cm in ReplinishItems on new { ci.ItemNumber, ci.SerialNumberTracking, ci.DateCodeTracking, ci.LotNumberTracking, ci.ItemType } equals new { cm.ItemNumber, cm.SerialNumberTracking, cm.DateCodeTracking, cm.LotNumberTracking, cm.ItemType }
                                                       where ci.ReplenishType == "Transfer"
                                                       //&& ReplinishingItemNumbers.Contains(ci.ItemNumber)
                                                       select ci).ToList();
                string OrderLineItemIds = string.Empty;
                List<CartItemDTO> lstGroupedCartItems = new List<CartItemDTO>();
                int totalLineCount = lstcartitemsgrped.Count;
                if (totalLineCount > 0)
                {
                    AutoOrderNumberGenerate objAutoNumber = null;
                    AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(base.DataBaseName);
                    //string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextAutoNumberByModule("NextTransferNo", SessionHelper.RoomID, SessionHelper.CompanyID);
                    objAutoNumber = objAutoSeqDAL.GetNextTransferNumber(RoomId, CompanyId, EnterpriseId);

                    string autoNumber = objAutoNumber.OrderNumber;

                    //string autoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextAutoNumberByModule("NextTransferNo", RoomId, CompanyId);
                    TransferMasterDTO objDTO = new TransferMasterDTO()
                    {
                        RequireDate = CurrentDateTime.AddDays(1),
                        TransferNumber = autoNumber,
                        TransferStatus = (int)TransferStatus.UnSubmitted,
                        RequestType = (int)RequestType.In,
                        RequestingRoomID = RoomId,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        CreatedBy = UserId,
                        LastUpdatedBy = UserId,
                        RoomID = RoomId,
                        CompanyID = CompanyId,
                        ReplenishingRoomID = objRoom.ReplineshmentRoom ?? 0,
                        GUID = Guid.NewGuid(),
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                        TransferLineItemsIds = Ids
                    };
                    objDTO = objTransferMasterDAL.InsertTransfer(objDTO);

                    if (objDTO.ID > 0)
                    {
                        TransferDetailDAL objTransferDetailDAL = new TransferDetailDAL(base.DataBaseName);
                        string cartguids = string.Empty;
                        foreach (var cartitem in lstcartitemsgrped)
                        {
                            TransferDetailDTO objTransferDetailDTO = new TransferDetailDTO();
                            objTransferDetailDTO.Bin = cartitem.BinId;
                            objTransferDetailDTO.CompanyID = CompanyId;
                            objTransferDetailDTO.Created = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.CreatedBy = UserId;
                            objTransferDetailDTO.FulFillQuantity = cartitem.Quantity;
                            objTransferDetailDTO.GUID = Guid.NewGuid();
                            objTransferDetailDTO.ID = cartitem.ID;
                            objTransferDetailDTO.IntransitQuantity = 0;
                            objTransferDetailDTO.IsArchived = false;
                            objTransferDetailDTO.IsDeleted = false;
                            objTransferDetailDTO.ItemGUID = cartitem.ItemGUID ?? Guid.Empty;

                            objTransferDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.LastUpdatedBy = UserId;
                            objTransferDetailDTO.ReceivedQuantity = 0;
                            objTransferDetailDTO.RequestedQuantity = cartitem.Quantity ?? 0;
                            //objTransferDetailDTO.RequiredDate = CurrentDateTime.AddDays(7);
                            objTransferDetailDTO.RequiredDate = cartitem.LeadTimeInDays.GetValueOrDefault(0) > 0 ? CurrentDateTime.AddDays(cartitem.LeadTimeInDays.GetValueOrDefault(0)) : CurrentDateTime.AddDays(1);
                            objTransferDetailDTO.Room = RoomId;
                            objTransferDetailDTO.ShippedQuantity = 0;
                            objTransferDetailDTO.TransferGUID = objDTO.GUID;
                            objTransferDetailDTO.AddedFrom = "Web";
                            objTransferDetailDTO.EditedFrom = "Web";
                            objTransferDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objTransferDetailDTO.ID = objTransferDetailDAL.Insert(objTransferDetailDTO, SessionUserId, false, EnterpriseId);
                            if (objDTO.TransferDetailList == null)
                            {
                                objDTO.TransferDetailList = new List<TransferDetailDTO>();
                            }

                            objDTO.TransferDetailList.Add(objTransferDetailDTO);
                            cartguids += cartitem.GUID.ToString() + ",";
                        }

                        if (SubmissionMethod == 2)
                        {
                            objDTO.TransferStatus = (int)TransferStatus.Transmitted;
                            objTransferMasterDAL.Edit(objDTO);
                        }

                        DeleteRecords(cartguids, UserId, CompanyId, EnterpriseId, SessionUserId);
                    }
                    lstTransfers.Add(objDTO);
                }

                return lstTransfers;
            }
            catch
            {
                throw;
            }

        }
        public void SuggestedOrderRoom(long RoomId, long CompanyId, long UserId, long EnterpriseID, long SessionUserId, bool isAfterSync = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.FirstOrDefault(t => t.ID == RoomId && t.IsDeleted == false);
                if (objRoom != null)
                {
                    if (objRoom.SuggestedOrder)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                               new SqlParameter("@CompanyID", CompanyId) };

                            List<Guid> oItemGuid = (from u in context.Database.SqlQuery<Guid>("exec [GetSuggestedItemGUIDByRoomCompanyID] @RoomID,@CompanyID", params1)
                                                    select u
                                               ).AsParallel().ToList();

                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                //AutoCartUpdateByCode(item, UserId, "web", "CartItemDAL>SuggestedOrderRoom");
                                AutoCartUpdateByCode(item, UserId, "web", "Replenish >> Order", SessionUserId);

                            }
                        }
                    }
                    else
                    {
                        //IQueryable<CartItem> lstcarts = context.CartItems.Where(t => t.Room == RoomId && (t.IsDeleted ?? false) == false && t.IsAutoMatedEntry == true && t.CompanyID == CompanyId && t.ReplenishType == "Purchase");
                        //if (lstcarts.Any())
                        //{
                        //    foreach (CartItem cartitem in lstcarts)
                        //    {
                        //        cartitem.IsDeleted = true;
                        //    }
                        //    context.SaveChanges();
                        //}

                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                           new SqlParameter("@CompanyID", CompanyId)  };

                        context.Database.ExecuteSqlCommand("exec [DeleteCartByRoomCompanyID] @RoomID,@CompanyID", params1);

                    }

                    if (objRoom.SuggestedTransfer)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                               new SqlParameter("@CompanyID", CompanyId) };

                            List<Guid> oItemGuid = (from u in context.Database.SqlQuery<Guid>("exec [GetSuggestedItemGUIDByRoomCompanyID] @RoomID,@CompanyID", params1)
                                                    select u
                                               ).AsParallel().ToList();

                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                //AutoCartUpdateByCode(item, UserId, "web", "CartItemDAL>SaveCartItems");
                                AutoCartUpdateByCode(item, UserId, "web", "Replenish >> SaveCart", SessionUserId);
                            }
                        }
                    }
                    else
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                           new SqlParameter("@CompanyID", CompanyId)  };

                        context.Database.ExecuteSqlCommand("exec [DeleteCartForSuggestedTransQty] @RoomID,@CompanyID", params1);

                        //IQueryable<CartItem> lstcarts = context.CartItems.Where(t => t.Room == RoomId && (t.IsDeleted ?? false) == false && t.IsAutoMatedEntry == true && t.CompanyID == CompanyId && t.ReplenishType == "Transfer");                        

                        //IQueryable<CartItem> lstcarts = context.CartItems.Where(t => t.Room == RoomId && (t.IsDeleted ?? false) == false && t.IsAutoMatedEntry == true && t.CompanyID == CompanyId && t.ReplenishType == "Transfer");
                        //if (lstcarts.Any())
                        //{
                        //    foreach (CartItem cartitem in lstcarts)
                        //    {
                        //        cartitem.IsDeleted = true;
                        //    }
                        //    context.SaveChanges();
                        //}
                    }

                    if (isAfterSync && objRoom.SuggestedReturn)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                               new SqlParameter("@CompanyID", CompanyId) };

                            List<Guid> oItemGuid = (from u in context.Database.SqlQuery<Guid>("exec [GetSuggestedItemGUIDByRoomCompanyID] @RoomID,@CompanyID", params1)
                                                    select u
                                               ).AsParallel().ToList();

                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                AutoCartForSuggestedReturnUpdateByCode(item, UserId, "web", "Replenish >> SaveCart", SessionUserId);
                            }
                        }
                    }
                    else
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                           new SqlParameter("@CompanyID", CompanyId),
                                                           new SqlParameter("@userid", UserId),
                                                           new SqlParameter("@editedfrom", "web"),
                                                           new SqlParameter("@calledfromfunctionname", "Web all cart from Room"),
                                                           new SqlParameter("@ReplenishType", "SuggestedReturn")
                                                         };
                        context.Database.CommandTimeout = 3600;
                        context.Database.ExecuteSqlCommand("EXEC [DeleteSuggestedReturnCartFromRoomSave] @RoomID,@CompanyID,@userid,@editedfrom,@calledfromfunctionname,@ReplenishType", params1);

                        //string DeleteSTqry = "UPDATE CartItem SET IsDeleted = 1, editedfrom = 'web', lastupdatedby = 2, updated = GETUTCDATE(), whatwhereaction = 'Web all cart from Room', receivedon = GETUTCDATE() WHERE Room = " + RoomId + " AND CompanyID = " + CompanyId + " AND isnull(IsDeleted, 0) = 0 AND ReplenishType = 'SuggestedReturn' AND IsAutoMatedEntry = 1; UPDATE ItemMaster  SET SuggestedReturnQuantity = 0 WHERE isnull(Isdeleted, 0) = 0 AND Room = " + RoomId + " AND CompanyID = " + CompanyId + " and isnull(SuggestedReturnQuantity,0) > 0";
                        //context.Database.ExecuteSqlCommand(DeleteSTqry);
                    }
                }
            }
        }
        private bool SendMailForSuggestedOrder(string ItemNumber, double OnhandQty, double CritQTY, double MinQty, Int64 RoomID, Int64 CompanyID, string strType, string RoomName, long UserId, string TemplateName, long EnterpriseId, double SO, string BinName)
        {
            EnterpriseDAL objEnterpriseDAL = new EnterpriseDAL(base.DataBaseName);
            EnterpriseDTO objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(EnterpriseId);
            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            AlertMail objAlertMail = new AlertMail();
            EmailTemplateDAL objEmailTemplateDAL = null;
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            CompanyMasterDAL objCompDAL = null;
            CompanyMasterDTO objCompany = null;
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            eTurnsUtility objUtils = null;
            NotificationDAL objNotificationDAL = new NotificationDAL(base.DataBaseName);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            RegionSettingDAL objRegionDAL = new RegionSettingDAL(base.DataBaseName);
            try
            {
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
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserId);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                MailTemplate tmplat = new MailTemplate();
                Enum.TryParse<MailTemplate>(TemplateName, out tmplat);
                objNotificationDAL = new NotificationDAL(base.DataBaseName);
                objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
                long tempid = objEmailTemplateDAL.GetTemplateId(TemplateName);

                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate(tempid, RoomID, CompanyID, currentCulture);
                UserMasterDTO ObjUserCreatedCache = objUserDAL.GetUserByIdPlain(UserId);

                lstNotifications.ForEach(t =>
                {
                    t.objeTurnsRegionInfo = objRegionDAL.GetRegionSettingsById(t.RoomID, t.CompanyID, t.UpdatedBy);

                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {

                    lstNotificationsImidiate.ForEach(nt =>
                    {
                        string QtyFormat = "N0";
                        string CostFormat = "N";
                        string dateFormate = "MM/dd/yyyy";

                        //string FromAddress = System.Configuration.ConfigurationManager.AppSettings["FromAddress"].ToString();
                        string strCCAddress = "";
                        //string strBCCAddress = "";
                        //string strNotificationAddress = "";
                        string strToAddress = nt.EmailAddress;
                        //if (string.IsNullOrEmpty(strToAddress))
                        //    return false;

                        if (nt.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                            QtyFormat += nt.objeTurnsRegionInfo.NumberDecimalDigits;

                        if (nt.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                            CostFormat += nt.objeTurnsRegionInfo.CurrencyDecimalDigits;

                        if (!string.IsNullOrEmpty(nt.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(nt.objeTurnsRegionInfo.ShortTimePattern))
                            dateFormate = nt.objeTurnsRegionInfo.ShortDatePattern + " " + nt.objeTurnsRegionInfo.ShortTimePattern;


                        string StrSubject = string.Empty;
                        MessageBody = new StringBuilder();

                        objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();

                        if (nt.EmailTemplateDetail.lstEmailTemplateDtls != null && nt.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                        {
                            objEmailTemplateDetailDTO = nt.EmailTemplateDetail.lstEmailTemplateDtls.First();

                        }
                        if (objEmailTemplateDetailDTO != null)
                        {
                            MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                            StrSubject = objEmailTemplateDetailDTO.MailSubject;
                        }

                        string ResourceFilePath = GetResourceFileFullPath("ResItemMaster", currentCulture, EnterpriseId, CompanyID);
                        string htmlTabl = string.Empty;

                        htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 5%; text-align: left;""> 
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", currentCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;""> 
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("DefaultLocation", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", currentCulture) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("OnHandQuantity", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", currentCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("MinimumQuantity", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", currentCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("CriticalQuantity", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", currentCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("SuggestedOrderQuantity", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", currentCulture) + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";

                        string trs = "";

                        //lstItems1 = lstItems.Where(x => x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.OnHandQuantity < x.MinimumQuantity && x.OnHandQuantity >= x.CriticalQuantity).ToList();

                        List<CartItemDTO> lstItems = GetCartListForDailyEmail(RoomID, CompanyID);
                        List<CartItemDTO> lstItems1 = new List<CartItemDTO>();
                        if (TemplateName.ToLower().Contains("minimum"))
                            lstItems1 = lstItems;
                        else if (strType.ToLower().Contains("critical"))
                            lstItems1 = lstItems.Where(x => x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.OnHandQuantity < x.CriticalQuantity).ToList();

                        if (!string.IsNullOrWhiteSpace(nt.SupplierIds))
                        {
                            List<long> arrsupids = new List<long>();
                            string[] suppids = nt.SupplierIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in suppids)
                            {
                                long temp = 0;
                                if (long.TryParse(item, out temp))
                                {
                                    arrsupids.Add(temp);
                                }

                            }
                            if (arrsupids.Count > 0)
                            {
                                lstItems1 = lstItems1.Where(t => arrsupids.Contains(t.SupplierId)).ToList();
                            }
                        }
                        string DataGuids = Guid.Empty.ToString();
                        if (lstItems1 != null && lstItems1.Count > 0)
                        {
                            DataGuids = string.Join(",", lstItems1.Select(t => t.GUID).ToArray());
                        }
                        //foreach (var item in lstItems1)
                        //{
                        //    trs += "<tr><td>" + (string.IsNullOrEmpty(item.ItemNumber) ? "&nbsp;" : item.ItemNumber) + "</td><td>" + (string.IsNullOrEmpty(item.BinName) ? "&nbsp;" : item.BinName) + @"</td><td>" + item.OnHandQuantity.ToString(QtyFormat) + @"</td><td>" + Item.MinimumQuantity.ToString(QtyFormat) + @"</td><td>" + t.CriticalQuantity.ToString(QtyFormat) + @"</td><td>" + (t.Quantity ?? 0).ToString(QtyFormat) + @"</td></tr>";
                        //}
                        if (lstItems1.Count > 0)
                        {
                            lstItems1.ForEach(t =>
                            {
                                trs += @"<tr>
                                            <td>
                                                " + (string.IsNullOrEmpty(t.ItemNumber) ? "&nbsp;" : t.ItemNumber) + @"
                                            </td>
                                            <td>
                                                " + (string.IsNullOrEmpty(t.BinName) ? "&nbsp;" : t.BinName) + @"
                                            </td>
                                            <td>
                                                " + t.OnHandQuantity.ToString(QtyFormat) + @"
                                            </td>
                                            <td>
                                                " + t.MinimumQuantity.ToString(QtyFormat) + @"
                                            </td>
                                            <td>
                                                " + t.CriticalQuantity.ToString(QtyFormat) + @"
                                            </td>
                                            <td>
                                                " + (t.Quantity ?? 0).ToString(QtyFormat) + @"
                                            </td>
                                        </tr>";

                            });

                            htmlTabl = htmlTabl.Replace("##TRS##", trs);


                            objCompDAL = new CompanyMasterDAL(base.DataBaseName);
                            objCompany = objCompDAL.GetCompanyByID(CompanyID);
                            //---------------------------------------------
                            MessageBody.Replace("@@TYPE@@", strType);
                            MessageBody.Replace("@@TABLE@@", htmlTabl);
                            MessageBody.Replace("@@COMPANYNAME@@", objCompany.Name);
                            MessageBody.Replace("@@ROOMNAME@@", RoomName);
                            MessageBody.Replace("@@USERNAME@@", ObjUserCreatedCache.UserName);
                            string strPath = string.Empty;
                            string strReplacepath = "";
                            if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                            {
                                strPath = objEnterpriseDTO.EnterPriseDomainURL;
                            }
                            else if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                            {
                                strPath = System.Web.HttpContext.Current.Request.Url.ToString();
                                strReplacepath = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                                strPath = strPath.Replace(strReplacepath, "/");
                            }
                            else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DomainName"]))
                            {
                                strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];

                            }

                            MessageBody = MessageBody.Replace("@@ETURNSLOGO@@", GeteTurnsImage(strPath, "Content/OpenAccess/logoInReport.png", EnterpriseId));
                            MessageBody = MessageBody.Replace("@@Year@@", DateTime.Now.Year.ToString());
                            MessageBody = MessageBody.Replace("/CKUpload/", strPath + "CKUpload/");
                            // /CKUpload/Gexpro GPS.png


                            objUtils = new eTurnsUtility();
                            objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", DataGuids);
                            lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(nt, objEnterpriseDTO, Params);
                            objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, nt, objEnterpriseDTO);
                        }
                        //SqlHelper.ExecuteNonQuery(MasterDbConection, "MailSendEntry", MessageBody.ToString(), 0, DateTime.Now, false, 0, EnterpriseId, CompanyID, RoomID, UserId, strToAddress, string.Empty, string.Empty, StrSubject, string.Empty);

                    });
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                MessageBody = null;
                objEmailTemplateDetailDTO = null;
                objCompDAL = null;
                objCompany = null;
                objUtils = null;
            }
        }

        private string GetResourceFileFullPath(string fileName, string Culture, Int64 EntID, Int64 CompanyID)
        {
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            string path = ResourceBaseFilePath;
            path += "\\" + EntID + "\\" + CompanyID + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }

        public string GeteTurnsImage(string path, string imagePath, long EntID)
        {
            //string str = string.Empty;

            //str = @"<a href='" + path + @"' title=""E Turns Powered""> <img alt=""E Turns Powered"" src='" + (path + imagePath) + @"' style=""border: 0px currentColor; border-image: none;"" /></a>";
            //return str;
            string urlPart = string.Empty;
            string replacePart = string.Empty;
            if (HttpContext.Current != null)
            {
                urlPart = HttpContext.Current.Request.Url.ToString();
                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
            }
            else
            {
                replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
            }
            if (EntID > 0)
            {
                string EnterpriseLogo = string.Empty;

                EnterpriseDAL objEnterpriseDAL = new EnterpriseDAL(base.DataBaseName);
                EnterpriseDTO objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(EntID);
                EnterpriseLogo = objEnterpriseDTO.EnterpriseLogo;

                if (!string.IsNullOrWhiteSpace(EnterpriseLogo))
                {
                    return GetImage(replacePart, "/Uploads/EnterpriseLogos/" + EntID.ToString() + "/" + EnterpriseLogo);
                }
                else
                {
                    return GetImage(replacePart, "/Content/OpenAccess/logo.jpg");
                }
            }
            else
            {
                return GetImage(replacePart, "/Content/OpenAccess/logo.jpg");
            }
        }
        public string GetImage(string path, string imagePath)
        {
            string str = string.Empty;
            str = @"<a href='" + path + @"' title=""E Turns Powered""> <img alt=""E Turns Powered"" width=""135"" height=""75"" src='" + (path + imagePath) + @"' style=""border: 0px currentColor; border-image: none;width:135px;height:75px;"" /></a>";
            return str;
        }
        private string GetMailBodySupplier(OrderMasterDTO obj, long RoomID, long CompanyID, eTurnsRegionInfo objeTurnsRegionInfo)
        {
            string mailBody = "";
            string suppliername = "";
            string OrdNumber = ResOrder.OrderNumber;
            string ReqDateCap = ResOrder.RequiredDate;
            string OrdStatus = ResOrder.OrderStatus;
            string OrdReqQty = ResOrder.RequestedQuantity;
            SupplierMasterDTO objSupplierMasterDTP = null;
            if (obj.OrderType == (int)OrderType.RuturnOrder)
            {
                OrdNumber = ResOrder.ReturnOrderNumber;
                ReqDateCap = ResOrder.ReturnDate; //ResOrder.ReturnDate
                OrdStatus = ResOrder.ReturnOrderStatus;
                OrdReqQty = ResOrder.ReturnQuantity;
            }

            if (obj.Supplier != null && obj.Supplier > 0)
                objSupplierMasterDTP = new SupplierMasterDAL(base.DataBaseName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));

            suppliername = objSupplierMasterDTP.SupplierName;

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                        <tr>
                            <td style=""width: 48%"">
                                <table style=""margin-left: 0px; width: 99%;"">
                                <tr>
                                    <td>
                                        <label style=""font-weight: bold;"">
                                            " + OrdNumber + @": </label>
                                        <label style=""font-weight: bold;"">
                                            " + obj.OrderNumber + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResOrder.Comment + @": </label>
                                        <label>
                                            " + obj.Comment + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResOrder.ShippingMethod + @": </label>
                                        <label>
                                            " + obj.ShipViaName + @"</label>
                                    </td>
                                </tr>
                            </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + obj.RequiredDate.ToString(objeTurnsRegionInfo.ShortDatePattern) + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResOrder.Supplier + @": </label>
                                    <label>
                                        " +
                                          suppliername
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(OrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResOrder.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.SupplierPartNo + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
            ";
            string trs = "";
            if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
            {
                OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(base.DataBaseName);
                obj.OrderListItem = objOrdDetailDAL.GetOrderDetailByOrderGUIDFull(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
            }


            if (objSupplierMasterDTP.IsSupplierReceivesKitComponents && obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {
                List<OrderDetailsDTO> objOrderItemList = new List<OrderDetailsDTO>();
                foreach (var item in obj.OrderListItem)
                {
                    objOrderItemList.Add(item);
                    if (item.ItemType == 3)
                    {
                        IEnumerable<KitDetailDTO> objKitDeailList = new KitDetailDAL(base.DataBaseName).GetAllRecordsByKitGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false, true);

                        foreach (var KitCompitem in objKitDeailList)
                        {
                            OrderDetailsDTO objOrderDetailDTO = new OrderDetailsDTO()
                            {
                                ItemNumber = KitCompitem.ItemNumber + "&nbsp;" + " <I>(comp. of Kit: " + item.ItemNumber + ")</I>", //string.Format(ResOrder.ComponentOfKit, item.ItemNumber)
                                BinName = item.BinName,
                                ApprovedQuantity = KitCompitem.QuantityPerKit.GetValueOrDefault(0) * item.ApprovedQuantity.GetValueOrDefault(),
                                RequiredDate = item.RequiredDate,
                            };
                            objOrderItemList.Add(objOrderDetailDTO);
                        }
                    }
                }
                obj.OrderListItem.Clear();
                obj.OrderListItem = objOrderItemList;
            }
            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {

                foreach (var item in obj.OrderListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.Bin != null && item.Bin > 0)
                        //binname = new BinMasterController().GetRecord(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID, false, false).BinNumber;
                        binname = new BinMasterDAL(base.DataBaseName).GetBinByID(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID).BinNumber;
                    //binname = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.Bin)),null,null).FirstOrDefault().BinNumber;

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.OrderStatus == (int)OrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(objeTurnsRegionInfo.ShortDatePattern);


                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + item.ItemDescription + @"
                        </td>
                        <td>
                            " + ReqQty + @"
                        </td>
                        <td>
                            " + ReqDate + @"
                        </td>
                        <td>
                            " + item.SupplierPartNo + @"
                        </td>
                    </tr>";

                }
            }
            else
            {
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           There is no item for this order
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }
        public void SendMailToSupplier(SupplierMasterDTO ToSuppliers, OrderMasterDTO objOrder, string RoomName, string CompanyName, string UserName, long EnterPriceID, eTurnsRegionInfo objeTurnsRegionInfo, long CompanyId)
        {
            AlertMail objAlertMail = new AlertMail();
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(base.DataBaseName).GetEnterprise(EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            try
            {
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
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }

                string ResourceFileOrder = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterPriceID, CompanyId);
                if ((objOrder.OrderType ?? 0) == (int)OrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(base.DataBaseName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.OrderToSupplier, ToSuppliers.Room ?? 0, ToSuppliers.CompanyID ?? 0, ResourceHelper.CurrentCult.Name);

                    lstNotifications = lstNotifications.Where(t => (t.SupplierIds ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(Convert.ToString(ToSuppliers.ID))).ToList();

                    lstNotifications.ForEach(t =>
                    {
                        if (t.SchedulerParams.ScheduleMode == 5)
                        {
                            lstNotificationsImidiate.Add(t);
                        }
                    });

                    if (lstNotificationsImidiate.Count > 0)
                    {
                        lstNotificationsImidiate.ForEach(t =>
                        {
                            string StrSubject = string.Empty;
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                            }
                            string strToAddress = t.EmailAddress;
                            string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                StringBuilder MessageBody = new StringBuilder();
                                objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
                                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                                {
                                    objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                                }
                                if (objEmailTemplateDetailDTO != null)
                                {
                                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                                }
                                else
                                {
                                    return;
                                }
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(ToSuppliers.Room ?? 0, ToSuppliers.CompanyID ?? 0, -1);
                                        string DateTimeFormat = "MM/dd/yyyy";
                                        DateTime TZDateTimeNow = DateTime.UtcNow;
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }
                                    }
                                }
                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@Supplier@@", objOrder.SupplierName);
                                string stratatTABLEatatTag = GetMailBodySupplier(objOrder, ToSuppliers.Room ?? 0, ToSuppliers.CompanyID ?? 0, objeTurnsRegionInfo);

                                string replacePart = string.Empty;
                                if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                                {
                                    replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                                }
                                else if (HttpContext.Current.Request == null)
                                {
                                    replacePart = ConfigurationManager.AppSettings["DomainName"];
                                }
                                else
                                {
                                    string urlPart = HttpContext.Current.Request.Url.ToString();
                                    replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                                }

                                if (ToSuppliers.IsEmailPOInBody)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else
                                {
                                    string SeeAttachedFilesForOrderDetail = ResourceRead.GetResourceValueByKeyAndFullFilePath("SeeAttachedFilesForOrderDetail", ResourceFileOrder, EnterPriceID, CompanyId, objOrder.Room.Value, "ResOrder", currentCulture);
                                    string strReplText = SeeAttachedFilesForOrderDetail;
                                    if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                                        strReplText = SeeAttachedFilesForOrderDetail;

                                    MessageBody.Replace("@@TABLE@@", strReplText);
                                }

                                objeMailAttchList = new List<eMailAttachmentDTO>();


                                MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                MessageBody.Replace("@@USERNAME@@", UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", CompanyName);
                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                                //objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier");
                            }
                        });
                    }

                }
                if ((objOrder.OrderType ?? 0) == (int)OrderType.RuturnOrder)
                {
                    objNotificationDAL = new NotificationDAL(base.DataBaseName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ReturnOrderApproval, ToSuppliers.Room ?? 0, ToSuppliers.CompanyID ?? 0, ResourceHelper.CurrentCult.Name);

                    lstNotifications.ForEach(t =>
                    {
                        if (t.SchedulerParams.ScheduleMode == 5)
                        {
                            lstNotificationsImidiate.Add(t);
                        }
                    });

                    if (lstNotificationsImidiate.Count > 0)
                    {
                        lstNotificationsImidiate.ForEach(t =>
                        {
                            string StrSubject = string.Empty;
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                            }
                            string strToAddress = t.EmailAddress;
                            string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                            if (!string.IsNullOrEmpty(strToAddress))
                            {

                                StringBuilder MessageBody = new StringBuilder();
                                objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
                                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                                {
                                    objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                                }
                                if (objEmailTemplateDetailDTO != null)
                                {
                                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                                }
                                else
                                {
                                    return;
                                }
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(ToSuppliers.Room ?? 0, ToSuppliers.CompanyID ?? 0, -1);
                                        string DateTimeFormat = "MM/dd/yyyy";
                                        DateTime TZDateTimeNow = DateTime.UtcNow;
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }
                                    }
                                }
                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@Supplier@@", objOrder.SupplierName);
                                string stratatTABLEatatTag = GetMailBodySupplier(objOrder, ToSuppliers.Room ?? 0, ToSuppliers.CompanyID ?? 0, objeTurnsRegionInfo);

                                string replacePart = string.Empty;
                                if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                                {
                                    replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                                }
                                else if (HttpContext.Current.Request == null)
                                {
                                    replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                                }
                                else
                                {
                                    string urlPart = HttpContext.Current.Request.Url.ToString();
                                    replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                                }

                                if (ToSuppliers.IsEmailPOInBody)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else
                                {
                                    string SeeAttachedFilesForOrderDetail = ResourceRead.GetResourceValueByKeyAndFullFilePath("SeeAttachedFilesForOrderDetail", ResourceFileOrder, EnterPriceID, CompanyId, objOrder.Room.Value, "ResOrder", currentCulture);
                                    string strReplText = SeeAttachedFilesForOrderDetail; //ResOrder.SeeAttachedFilesForOrderDetail;
                                    if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                                        strReplText = SeeAttachedFilesForOrderDetail; //ResOrder.SeeAttachedFilesForReturnOrderDetail;

                                    MessageBody.Replace("@@TABLE@@", strReplText);
                                }

                                objeMailAttchList = new List<eMailAttachmentDTO>();


                                MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                MessageBody.Replace("@@USERNAME@@", UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", CompanyName);
                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                                //objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier");
                            }
                        });
                    }
                }


            }
            finally
            {
                //objUtils = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //splitEmails = null;
                objeMailAttchList = null;
                //objeMailAttch = null;
                //arrAttchament = null;
            }
        }
        public double getOnOrderQty(Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double onOrderQuantity = 0;

                if (context.OrderDetails.Join(context.OrderMasters, od => od.OrderGUID, om => om.GUID, (od, om) => new { od, om }).Where(t => t.om.IsDeleted == false && (t.od.IsDeleted ?? false) == false && (t.od.IsCloseItem ?? false) == false && new int[] { 1, 2, 3, 4, 5, 6, 7 }.Contains(t.om.OrderStatus) && t.od.ItemGUID == ItemGUID && t.om.OrderType == 1).Any())
                {
                    onOrderQuantity = context.OrderDetails.Join(context.OrderMasters, od => od.OrderGUID, om => om.GUID, (od, om) => new { od, om }).Where(t => t.om.IsDeleted == false && (t.od.IsDeleted ?? false) == false && (t.od.IsCloseItem ?? false) == false && new int[] { 1, 2, 3, 4, 5, 6, 7 }.Contains(t.om.OrderStatus) && t.od.ItemGUID == ItemGUID && t.om.OrderType == 1).Sum(t => ((t.od.ApprovedQuantity ?? (t.od.RequestedQuantity ?? 0))) - (t.od.ReceivedQuantity ?? 0));
                }
                return onOrderQuantity;
            }
        }
        public List<BinMasterDTO> GetItemLocations(Guid ItemGuid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double sdas = context.CartItems.Where(t => (t.ItemGUID ?? Guid.Empty) == ItemGuid && (t.BinId ?? 0) == 3 && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false).Any() ? 23 : 0;
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                lstBins = (from bm in context.BinMasters
                           join im in context.ItemMasters on bm.ItemGUID equals im.GUID
                           join rm in context.Rooms on bm.Room equals rm.ID
                           join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                           where bm.ItemGUID == ItemGuid && (bm.IsArchived ?? false) == false && bm.IsDeleted == false
                           select new BinMasterDTO
                           {
                               CriticalQuantity = bm.CriticalQuantity,
                               MaximumQuantity = bm.MaximumQuantity,
                               MinimumQuantity = bm.MinimumQuantity,
                               BinNumber = bm.BinNumber,
                               CompanyName = cm.Name,
                               SuggestedOrderQuantity = context.CartItems.Where(t => (t.ItemGUID ?? Guid.Empty) == ItemGuid && (t.BinId ?? 0) == bm.ID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false).Any() ? context.CartItems.Where(t => (t.ItemGUID ?? Guid.Empty) == ItemGuid && (t.BinId ?? 0) == bm.ID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false).Sum(t => t.Quantity) : 0,
                               CompanyID = bm.CompanyID,
                               Created = bm.Created,
                               CreatedBy = bm.CreatedBy,
                               ConsignedQuantity = 0,
                               CustomerOwnedQuantity = 0,
                               GUID = bm.GUID,
                               ID = bm.ID,
                               IsDefault = bm.IsDefault,
                               ItemGUID = bm.ItemGUID,
                               ItemNumber = im.ItemNumber,
                               LastUpdated = bm.LastUpdated,
                               LastUpdatedBy = bm.LastUpdatedBy,
                               Room = bm.Room,
                               RoomName = rm.RoomName,
                               OnHandQuantity = context.ItemLocationDetails.Where(t => (t.ItemGUID ?? Guid.Empty) == ItemGuid && (t.BinID ?? 0) == bm.ID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false).Any() ? context.ItemLocationDetails.Where(t => (t.ItemGUID ?? Guid.Empty) == ItemGuid && (t.BinID ?? 0) == bm.ID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false).Sum(t => ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0))) : 0,
                           }).ToList();

                return lstBins;
            }
        }
        public List<OrderMasterDTO> CreateOrdersByCart(List<OrderMasterDTO> lstOrders, long RoomId, long CompanyId, long UserId, string EpDatabaseName, short SubmissionMethod, long EnterpriseId, out Dictionary<string, string> rejectedOrderLineItems, long SessionUserId, string callingFrom = "")
        {

            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            SupplierMasterDAL objSupplierDAl = new SupplierMasterDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            List<OrderMasterDTO> lstSuccessOrders = new List<OrderMasterDTO>();
            SupplierMasterDTO objSupplier = new SupplierMasterDTO();
            int ActualOrderStatus = 1;
            rejectedOrderLineItems = new Dictionary<string, string>();

            if (lstOrders != null && lstOrders.Count > 0 && lstOrders.Any(x => x.IsValid))
            {
                CommonDAL objCommonDAL = new CommonDAL(EpDatabaseName);
                OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(EpDatabaseName);
                CartItemDAL objCartItemDAL = new CartItemDAL(EpDatabaseName);
                OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(EpDatabaseName);
                RoomDAL objRoomDAL = new RoomDAL(EpDatabaseName);
                //RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomId);
                string columnList = "ID,RoomName,PreventMaxOrderQty";
                RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");
                Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
                Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
                int rejectedDueToPreventMaxValidation = 0;
                BinMasterDAL objBinDAL = new BinMasterDAL(EpDatabaseName);
                List<Guid> unSuccessfulOrders = new List<Guid>();
                List<string> rejectedOrderLineItemsGuids = new List<string>();
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                OrderMasterDAL objOrderDAL = new OrderMasterDAL(EpDatabaseName);

                foreach (OrderMasterDTO objOrderMasterDTO in lstOrders.Where(x => x.IsValid))
                {
                    OrderMasterDTO objReturnAfterSave = new OrderMasterDTO();

                    objSupplier = objSupplierDAl.GetSupplierByIDPlain(objOrderMasterDTO.Supplier.GetValueOrDefault(0));

                    if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderNumber))
                    {
                        objOrderMasterDTO.ReleaseNumber = Convert.ToString(objOrderDAL.GetNextReleaseNumber(objOrderMasterDTO.OrderNumber, objOrderMasterDTO.GUID, RoomId, CompanyId));
                        //IEnumerable<OrderMasterDTO> objOrderList = objOrderDAL.GetAllRecords(RoomId, CompanyId, false, false, OrderType.Order);
                        //objOrderMasterDTO.ReleaseNumber = (objOrderList.Where(x => x.OrderNumber == objOrderMasterDTO.OrderNumber).Count() + 1).ToString();
                    }
                    objOrderMasterDTO.OrderType = (int)OrderType.Order;

                    ActualOrderStatus = objOrderMasterDTO.OrderStatus;
                    objOrderMasterDTO.OrderStatus = (int)OrderStatus.UnSubmitted;
                    objOrderMasterDTO.LastUpdatedBy = UserId;
                    objOrderMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objOrderMasterDTO.CreatedBy = UserId;
                    objOrderMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objOrderMasterDTO.Room = RoomId;
                    objOrderMasterDTO.CompanyID = CompanyId;
                    objOrderMasterDTO.OrderDate = CurrentDateTime;
                    objOrderMasterDTO.StagingID = objCommonDAL.GetOrInsertMaterialStagingIDByName(objOrderMasterDTO.StagingName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.MaterialStagingGUID = objCommonDAL.GetOrInsertMaterialStagingGUIDByName(objOrderMasterDTO.StagingName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.ShipVia = objCommonDAL.GetOrInsertShipVaiIDByName(objOrderMasterDTO.ShipViaName, UserId, RoomId, CompanyId);
                    //objOrderMasterDTO.CustomerID = objCommonDAL.GetOrInsertCustomerIDByName(objOrderMasterDTO.CustomerName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.CustomerID = null;
                    CustomerMasterDTO objCustomerMasterDTO = objCommonDAL.GetOrInsertCustomerGUIDByName(objOrderMasterDTO.CustomerName, UserId, RoomId, CompanyId);
                    if (objCustomerMasterDTO != null)
                    {
                        objOrderMasterDTO.CustomerGUID = objCustomerMasterDTO.GUID;
                    }
                    objOrderMasterDTO.ShippingVendor = objCommonDAL.GetOrInsertVendorIDByName(objOrderMasterDTO.ShippingVendorName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.GUID = Guid.NewGuid();
                    objOrderMasterDTO.WhatWhereAction = "cart";
                    objOrderMasterDTO.AddedFrom = "Web";
                    objOrderMasterDTO.EditedFrom = "Web";
                    objOrderMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objOrderMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Submitted)
                    {
                        objOrderMasterDTO.RequesterID = UserId;
                    }
                    else if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved)
                    {
                        objOrderMasterDTO.ApproverID = UserId;
                    }

                    objReturnAfterSave = objOrderMasterDAL.InsertOrder(objOrderMasterDTO, SessionUserId);
                    objReturnAfterSave.OrderPrice = objOrderMasterDTO.OrderPrice;
                    objReturnAfterSave.OrderCost = objOrderMasterDTO.OrderCost;

                    List<Guid> lstids = new List<Guid>();
                    List<Guid> itemGuidsToUpdate = new List<Guid>();

                    if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                    {
                        foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                        {
                            Guid tempid = Guid.Empty;
                            if (Guid.TryParse(item, out tempid))
                            {
                                lstids.Add(tempid);
                            }
                        }
                    }
                    if (lstids.Count > 0)
                    {
                        var tmpsupplierIds = new List<long>();
                        List<CartItemDTO> lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, RoomId, CompanyId, true, tmpsupplierIds);
                        string[] AllGuid = objOrderMasterDTO.OrderLineItemsIds.Split(',').ToArray();
                        string[] udf1;
                        string[] udf2;
                        string[] udf3;
                        string[] udf4;
                        string[] udf5;
                        string[] cartItemQuantity;
                        if (objOrderMasterDTO.OrderLineItemUDF1 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF1)))
                        {
                            udf1 = objOrderMasterDTO.OrderLineItemUDF1.Split(',').ToArray();
                        }
                        else
                        {
                            udf1 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF2 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF2)))
                        {
                            udf2 = objOrderMasterDTO.OrderLineItemUDF2.Split(',').ToArray();
                        }
                        else
                        {
                            udf2 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF3 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF3)))
                        {
                            udf3 = objOrderMasterDTO.OrderLineItemUDF3.Split(',').ToArray();
                        }
                        else
                        {
                            udf3 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF4 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF4)))
                        {
                            udf4 = objOrderMasterDTO.OrderLineItemUDF4.Split(',').ToArray();
                        }
                        else
                        {
                            udf4 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF5 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF5)))
                        {
                            udf5 = objOrderMasterDTO.OrderLineItemUDF5.Split(',').ToArray();
                        }
                        else
                        {
                            udf5 = null;
                        }
                        if (objOrderMasterDTO.CartQuantityString != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.CartQuantityString)))
                        {
                            cartItemQuantity = objOrderMasterDTO.CartQuantityString.Split(',').ToArray();
                        }
                        else
                        {
                            cartItemQuantity = null;
                        }

                        long temp_CreatedBy = UserId;
                        long temp_LastUpdatedBy = UserId;

                        foreach (CartItemDTO cartitem in lstCartItems)
                        {

                            if ((callingFrom ?? string.Empty).ToLower() == "service")
                            {
                                if (cartitem.CreatedBy.GetValueOrDefault(0) > 0)
                                {
                                    temp_CreatedBy = cartitem.CreatedBy.GetValueOrDefault(0);
                                }

                                if (cartitem.LastUpdatedBy.GetValueOrDefault(0) > 0)
                                {
                                    temp_LastUpdatedBy = cartitem.LastUpdatedBy.GetValueOrDefault(0);
                                }
                            }

                            int Index = Array.FindIndex(AllGuid, row => row.Contains(cartitem.GUID.ToString()));

                            string UDF1 = string.Empty;
                            string UDF2 = string.Empty;
                            string UDF3 = string.Empty;
                            string UDF4 = string.Empty;
                            string UDF5 = string.Empty;
                            if (udf1 != null && udf1.Length > 0)
                            {
                                UDF1 = udf1[Index];
                            }
                            if (udf2 != null && udf2.Length > 0)
                            {
                                UDF2 = udf2[Index];
                            }
                            if (udf3 != null && udf3.Length > 0)
                            {
                                UDF3 = udf3[Index];
                            }
                            if (udf4 != null && udf4.Length > 0)
                            {
                                UDF4 = udf4[Index];
                            }
                            if (udf5 != null && udf5.Length > 0)
                            {
                                UDF5 = udf5[Index];
                            }

                            OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();

                            objOrderDetailsDTO.OrderGUID = objReturnAfterSave.GUID;

                            objOrderDetailsDTO.ItemGUID = cartitem.ItemGUID;
                            objOrderDetailsDTO.Bin = cartitem.BinId;
                            double? QuanityToSet = cartitem.Quantity;
                            if (cartItemQuantity != null && cartItemQuantity.Length > 0)
                            {
                                try
                                {
                                    QuanityToSet = Convert.ToDouble(cartItemQuantity[Index]);
                                }
                                catch (Exception) { }
                            }
                            objOrderDetailsDTO.RequestedQuantity = QuanityToSet;
                            if (SubmissionMethod == 2 || ActualOrderStatus > 2)
                            {
                                objOrderDetailsDTO.ApprovedQuantity = QuanityToSet;
                            }
                            //objOrderDetailsDTO.RequiredDate = objReturnAfterSave.RequiredDate;
                            objOrderDetailsDTO.RequiredDate = cartitem.LeadTimeInDays.GetValueOrDefault(0) > 0 ? CurrentDateTime.AddDays(cartitem.LeadTimeInDays.GetValueOrDefault(0)) : objReturnAfterSave.RequiredDate;
                            objOrderDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.CreatedBy = temp_CreatedBy;
                            objOrderDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.LastUpdatedBy = temp_LastUpdatedBy;
                            objOrderDetailsDTO.Room = RoomId;
                            objOrderDetailsDTO.CompanyID = CompanyId;
                            objOrderDetailsDTO.AddedFrom = "Web";
                            objOrderDetailsDTO.EditedFrom = "Web";
                            objOrderDetailsDTO.UDF1 = UDF1;
                            objOrderDetailsDTO.UDF2 = UDF2;
                            objOrderDetailsDTO.UDF3 = UDF3;
                            objOrderDetailsDTO.UDF4 = UDF4;
                            objOrderDetailsDTO.UDF5 = UDF5;
                            objOrderDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;


                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), RoomId, CompanyId);
                            if (objItemMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false))
                            {
                                double newOrderQty = 0;
                                int devideval = 0;
                                double tempQty = QuanityToSet ?? 0;
                                double drq = objItemMasterDTO.DefaultReorderQuantity ?? 0;
                                if (tempQty > 0 && drq > 0)
                                {
                                    if ((tempQty % drq) != 0)
                                    {
                                        devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                                        newOrderQty = drq * devideval;
                                    }
                                    else
                                    {
                                        //  devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq)));
                                        newOrderQty = tempQty;
                                    }
                                }
                                objOrderDetailsDTO.RequestedQuantity = newOrderQty;
                                if (SubmissionMethod == 2 || ActualOrderStatus > 2)
                                {
                                    objOrderDetailsDTO.ApprovedQuantity = newOrderQty;
                                }
                            }
                            if (cartitem.BinName != null && cartitem.BinName != string.Empty)
                            {
                                var lstOfOrderLineItemBin = objBinDAL.GetAllBinMastersByBinList(cartitem.BinName, RoomId, CompanyId);
                                double qtytoset = lstOfOrderLineItemBin.Where(x => x.ItemGUID == objItemMasterDTO.GUID).FirstOrDefault().DefaultReorderQuantity.GetValueOrDefault(0);
                                bool isEqtytoset = lstOfOrderLineItemBin.Where(x => x.ItemGUID == objItemMasterDTO.GUID).FirstOrDefault().IsEnforceDefaultReorderQuantity.GetValueOrDefault(false);
                                if (qtytoset > 0&& isEqtytoset)
                                {
                                    double newOrderQty = 0;
                                    int devideval = 0;
                                    double tempQty = QuanityToSet ?? 0;
                                    double drq = qtytoset;
                                    if (tempQty > 0 && drq > 0)
                                    {
                                        if ((tempQty % drq) != 0)
                                        {
                                            devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                                            newOrderQty = drq * devideval;
                                        }
                                        else
                                        {
                                            //  devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq)));
                                            newOrderQty = tempQty;
                                        }
                                    }
                                    objOrderDetailsDTO.RequestedQuantity = newOrderQty;
                                    if (SubmissionMethod == 2 || ActualOrderStatus > 2)
                                    {
                                        objOrderDetailsDTO.ApprovedQuantity = newOrderQty;
                                    }
                                }
                            }
                            if (objItemMasterDTO != null)
                            {
                                var supplierForOrderLineItem = objItemMasterDAL.GetItemSupplierForOrder(objItemMasterDTO.GUID, objOrderMasterDTO.Supplier.GetValueOrDefault(0));

                                if (supplierForOrderLineItem != null && supplierForOrderLineItem.SupplierID.GetValueOrDefault(0) > 0)
                                {
                                    objOrderDetailsDTO.SupplierID = supplierForOrderLineItem.SupplierID;
                                    objOrderDetailsDTO.SupplierPartNo = supplierForOrderLineItem.SupplierPartNo;
                                }

                                CostUOMMasterDTO costUOM = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                                if (costUOM == null)
                                    costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                                if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                                {
                                    costUOM.CostUOMValue = 1;
                                }

                                #region WI-6215 and Other Relevant order cost related jira
                                objOrderDetailsDTO.ItemSellPrice = objItemMasterDTO.SellPrice.GetValueOrDefault(0);
                                objOrderDetailsDTO.ItemCostUOMValue = costUOM.CostUOMValue.GetValueOrDefault(0);
                                objOrderDetailsDTO.ItemMarkup = objItemMasterDTO.Markup.GetValueOrDefault(0);
                                #endregion

                                objOrderDetailsDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objOrderMasterDTO.OrderStatus <= 2 ? (objOrderDetailsDTO.RequestedQuantity * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                                                             : (objOrderDetailsDTO.ApprovedQuantity.GetValueOrDefault(0) * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));

                                objOrderDetailsDTO.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objOrderMasterDTO.OrderStatus <= 2 ? (objOrderDetailsDTO.RequestedQuantity * (objItemMasterDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                                                             : (objOrderDetailsDTO.ApprovedQuantity.GetValueOrDefault(0) * (objItemMasterDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));

                                OrderUOMMasterDTO OrderUOM = new OrderUOMMasterDAL(base.DataBaseName).GetRecord(objItemMasterDTO.OrderUOMID.GetValueOrDefault(0), objItemMasterDTO.Room.GetValueOrDefault(0), objItemMasterDTO.CompanyID.GetValueOrDefault(0), false, false);
                                if (OrderUOM == null)
                                    OrderUOM = new OrderUOMMasterDTO() { OrderUOMValue = 1 };

                                if (OrderUOM.OrderUOMValue == null || OrderUOM.OrderUOMValue <= 0)
                                {
                                    OrderUOM.OrderUOMValue = 1;
                                }

                                if (objOrderDetailsDTO.RequestedQuantity != null && objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0) > 0 && objItemMasterDTO.IsAllowOrderCostuom)
                                {
                                    objOrderDetailsDTO.RequestedQuantityUOM = objOrderDetailsDTO.RequestedQuantity / OrderUOM.OrderUOMValue;
                                }
                                else
                                {
                                    objOrderDetailsDTO.RequestedQuantityUOM = objOrderDetailsDTO.RequestedQuantity;
                                }

                                if (objOrderDetailsDTO.ApprovedQuantity != null && objOrderDetailsDTO.ApprovedQuantity.GetValueOrDefault(0) > 0 && objItemMasterDTO.IsAllowOrderCostuom)
                                {
                                    objOrderDetailsDTO.ApprovedQuantityUOM = objOrderDetailsDTO.ApprovedQuantity / OrderUOM.OrderUOMValue;
                                }
                                else
                                {
                                    objOrderDetailsDTO.ApprovedQuantityUOM = objOrderDetailsDTO.ApprovedQuantity;
                                }

                                objOrderDetailsDTO.ItemCost = objItemMasterDTO.Cost.GetValueOrDefault(0);
                                objOrderDetailsDTO.ItemCostUOM = objItemMasterDTO.CostUOMID.GetValueOrDefault(0);
                            }
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
                                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomId, CompanyId, UserId);
                                currentCulture = objeTurnsRegionInfo.CultureName;
                            }
                            string ResourceFileCartItem = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCartItem", currentCulture, EnterpriseId, CompanyId);
                            string MsgItemMaximumQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemMaximumQuantity", ResourceFileCartItem, EnterpriseId, CompanyId, RoomId, "ResCartItem", currentCulture);
                            string MsgBinMaximumQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBinMaximumQuantity", ResourceFileCartItem, EnterpriseId, CompanyId, RoomId, "ResCartItem", currentCulture);
                            if (objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder)
                            {
                                if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.Value)
                                {
                                    if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0) + objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0)) > objItemMasterDTO.MaximumQuantity.Value)
                                    {
                                        if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                        {
                                            rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = MsgItemMaximumQuantity;
                                        }
                                        rejectedDueToPreventMaxValidation++;
                                        unSuccessfulOrders.Add(objReturnAfterSave.GUID);
                                        rejectedOrderLineItemsGuids.Add(Convert.ToString(cartitem.GUID));
                                        continue;
                                    }
                                }
                                else
                                {
                                    List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemMasterDTO.GUID, RoomId, CompanyId).OrderBy(x => x.BinNumber).ToList();
                                    var maxQtyAtBinLevel = lstItemBins.Where(e => e.BinNumber.Equals(cartitem.BinName)).FirstOrDefault();
                                    var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : cartitem.BinId.GetValueOrDefault(0);
                                    var onOrderQtyAtBin = objOrderDetailsDAL.GetOrderdQtyOfItemBinWise(RoomId, CompanyId, objItemMasterDTO.GUID, tmpBinId);

                                    if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && (onOrderQtyAtBin + objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0)) > maxQtyAtBinLevel.MaximumQuantity.Value)
                                    {
                                        if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                        {
                                            rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = MsgBinMaximumQuantity;
                                        }
                                        rejectedDueToPreventMaxValidation++;
                                        unSuccessfulOrders.Add(objReturnAfterSave.GUID);
                                        rejectedOrderLineItemsGuids.Add(Convert.ToString(cartitem.GUID));
                                        continue;
                                    }
                                }
                            }

                            OrderDetailsDTO returnOrderDetailsDTO = new OrderDetailsDTO();
                            returnOrderDetailsDTO = objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId, EnterpriseId);
                            itemGuidsToUpdate.Add(cartitem.ItemGUID ?? Guid.Empty);
                            //WI-8417 JKP
                            if (ActualOrderStatus >= (int)OrderStatus.Approved)
                            {
                                try
                                {
                                    objOrderDetailsDAL.UpdateOrderUsedTotalValueBPO(cartitem.ItemGUID.Value, objOrderDetailsDTO.ApprovedQuantity.GetValueOrDefault(0), objOrderDetailsDTO.OrderLineItemExtendedCost.GetValueOrDefault(0),"order");
                                }
                                catch (Exception ex) { }
                            }
                            // MaintainTransaction History for Cart To Order
                            if (returnOrderDetailsDTO != null)
                            {
                                objCartItemDAL.InsertCartQuoteTransitionDetail(cartitem.GUID, null, returnOrderDetailsDTO.GUID, null, (int)TransactionConversionType.CarttoOrder, UserId);
                            }

                            //ItemMasterDAL itemmasterobj = new ItemMasterDAL(base.DataBaseName);
                            //itemmasterobj.EditDate(cartitem.ItemGUID ?? Guid.Empty, "EditOrderedDate");
                        }
                        if (objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder && rejectedOrderLineItemsGuids.Any())
                        {
                            List<string> Items = objOrderMasterDTO.OrderLineItemsIds.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                            foreach (var guid in rejectedOrderLineItemsGuids)
                            {
                                Items.Remove(guid);
                            }
                            objOrderMasterDTO.OrderLineItemsIds = string.Join(",", Items.ToArray());
                        }
                        if (!string.IsNullOrEmpty(objOrderMasterDTO.OrderLineItemsIds))
                        {
                            objCartItemDAL.DeleteRecords(objOrderMasterDTO.OrderLineItemsIds, UserId, CompanyId, EnterpriseId, SessionUserId);
                        }

                    }

                    //--------------------------------------------------------------------
                    //
                    objOrderMasterDTO.OrderStatus = ActualOrderStatus;
                    if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved)
                    {
                        if (!objSupplier.IsSendtoVendor)
                        {
                            objOrderMasterDTO.OrderStatus = (int)OrderStatus.Transmitted;
                        }
                    }
                    if (SubmissionMethod == 2)
                    {
                        if (!objSupplier.IsSendtoVendor)
                        {
                            objOrderMasterDTO.OrderStatus = (int)OrderStatus.Transmitted;
                        }
                        else
                        {
                            objOrderMasterDTO.OrderStatus = (int)OrderStatus.Approved;
                        }
                    }

                    if (!unSuccessfulOrders.Contains(objReturnAfterSave.GUID))
                    {
                        objOrderMasterDAL.UpdateOrderStatus(objReturnAfterSave.GUID, null, objOrderMasterDTO.OrderStatus);

                        OrderMasterDTO objOMDto = objOrderMasterDAL.GetOrderByGuidPlain(objReturnAfterSave.GUID);
                        if (objOMDto != null)
                        {
                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Submitted)
                            {
                                objReturnAfterSave.RequesterID = UserId;
                                objOMDto.RequesterID = UserId;
                            }
                            else if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved
                                    || objOrderMasterDTO.OrderStatus == (int)OrderStatus.Transmitted)
                            {
                                objReturnAfterSave.RequesterID = UserId;
                                objReturnAfterSave.ApproverID = UserId;
                                objOMDto.RequesterID = UserId;
                                objOMDto.ApproverID = UserId;
                            }
                            objOrderMasterDAL.Edit(objOMDto);
                        }

                        objReturnAfterSave.OrderStatus = objOrderMasterDTO.OrderStatus;
                        lstSuccessOrders.Add(objReturnAfterSave);
                    }

                    if (itemGuidsToUpdate != null && itemGuidsToUpdate.Any() && itemGuidsToUpdate.Count > 0)
                    {
                        foreach (var guid in itemGuidsToUpdate)
                        {
                            var onOrderInTransitQuantity = objItemMasterDAL.getOnOrderInTransitQty(guid);
                            objItemMasterDAL.EditDateAndOnOrderInTransitQuantity(guid, "EditOrderedDate", onOrderInTransitQuantity);
                        }
                    }
                }
            }
            return lstSuccessOrders;
        }
        public Int32 GetCartOrderCount(long CompanyId, long RoomId)
        {
            Int32 ret = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ret = context.CartItems.Where(u => u.IsDeleted == false && u.IsArchived == false && u.ReplenishType == "Purchase" && u.CompanyID == CompanyId && u.Room == RoomId).Count();
            }
            return ret;
        }
        public Int32 GetCartTransferCount(long CompanyId, long RoomId)
        {
            Int32 ret = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ret = context.CartItems.Where(u => u.IsDeleted == false && u.IsArchived == false && u.ReplenishType == "Transfer" && u.CompanyID == CompanyId && u.Room == RoomId).Count();
            }
            return ret;
        }

        public List<CartChartDTO> GetCartItemsForDashboard(out IEnumerable<CartChartDTO> SupplierList, out IEnumerable<CartChartDTO> CategoryList,
            Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID,
            string type, List<long> UserSupplierIds, string SelectedSupplierIds, string SelectedCategoryIds)
        {
            DataSet dsDashitems = new DataSet();
            List<CartChartDTO> lstItems = new List<CartChartDTO>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            TotalCount = 0;

            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                SupplierList = null;
                CategoryList = null;
                return lstItems;
            }

            if (MaxRows < 1)
            {
                MaxRows = 10;
            }

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            string strUserSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strUserSupplierIds = string.Join(",", UserSupplierIds);
            }

            string strSelectedSupplierIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedSupplierIds) && !string.IsNullOrWhiteSpace(SelectedSupplierIds))
            {
                strSelectedSupplierIds = SelectedSupplierIds;
            }

            string strSelectedCategoryIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedCategoryIds) && !string.IsNullOrWhiteSpace(SelectedCategoryIds))
            {
                strSelectedCategoryIds = SelectedCategoryIds;
            }

            dsDashitems = SqlHelper.ExecuteDataset(EturnsConnection, "GetCartItemsForDashboard", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, false, false, type, strUserSupplierIds, strSelectedSupplierIds, strSelectedCategoryIds);

            if (dsDashitems != null && dsDashitems.Tables.Count > 0)
            {
                SupplierList = DataTableHelper.ToList<CartChartDTO>(dsDashitems.Tables[0]);
                CategoryList = DataTableHelper.ToList<CartChartDTO>(dsDashitems.Tables[1]);

                if (dsDashitems.Tables.Count > 2)
                {
                    lstItems = DataTableHelper.ToList<CartChartDTO>(dsDashitems.Tables[2]);

                    if (lstItems != null && lstItems.Count() > 0)
                    {
                        TotalCount = lstItems.ElementAt(0).TotalRecords;
                    }
                }
            }
            else
            {
                SupplierList = null;
                CategoryList = null;
            }
            return lstItems;

        }
        public CartItemDTO GetCartByGUID(Guid CartGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CartGUID", CartGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartByGUID] @CartGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public CartItemDTO GetCartByGUIDPlain(Guid CartGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CartGUID", CartGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartByGUIDPlain] @CartGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public CartItemDTO GetCartByID(long ID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartByID] @ID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public CartItemDTO GetCartByIDPlain(long ID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartByIDPlain] @ID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<CartItemDTO> GetCartsByItemGUID(Guid ItemGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartsByItemGUID] @ItemGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CartItemDTO> GetCartsByItemID(long ItemID, long RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemID", ItemID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartsByItemID] @ItemID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CartItemDTO> GetCartsByItemGUIDPlain(Guid ItemGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartsByItemGUIDPlain] @ItemGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }


        #region Return for Cart Item 5330

        public IList<OrderMasterDTO> GetReturnOrdersByCartIds(string Ids, long RoomId, long CompanyId, long UserId, string DatabaseName, List<long> SupplierIds, string OrderLineItemUDF1, string OrderLineItemUDF2, string OrderLineItemUDF3, string OrderLineItemUDF4, string OrderLineItemUDF5, long EnterpriseID, string OrderItemQuantity)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            CartItemDAL objCartItemDAL = new CartItemDAL(DatabaseName);
            List<Guid> arrcartguids = new List<Guid>();
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            bool WithSelection;
            IList<OrderMasterDTO> lstOrders = new List<OrderMasterDTO>();
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(base.DataBaseName);
            SupplierMasterDTO objSupplierDTO = new SupplierMasterDTO();

            try
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    WithSelection = true;
                }
                else
                {
                    WithSelection = false;
                }
                lstCartItems = objCartItemDAL.GetCartItemsByGuids(arrcartguids, RoomId, CompanyId, WithSelection, SupplierIds);
                List<CartItemDTO> lstcartitemsgrped = (from ci in lstCartItems
                                                       where ci.ReplenishType == "SuggestedReturn"
                                                       //group ci by new { ci.SupplierId, ci.SupplierName, ci.BlanketPOID, ci.MaxOrderSize, ci.BlanketPONumber } into groupedci
                                                       group ci by new { ci.SupplierId, ci.SupplierName, ci.MaxOrderSize } into groupedci
                                                       select new CartItemDTO
                                                       {
                                                           SupplierId = groupedci.Key.SupplierId,
                                                           //BlanketPOID = groupedci.Key.BlanketPOID,
                                                           MaxOrderSize = groupedci.Key.MaxOrderSize,
                                                           SupplierName = groupedci.Key.SupplierName,
                                                           //BlanketPONumber = groupedci.Key.BlanketPONumber
                                                       }).ToList();
                string OrderLineItemIds = string.Empty;
                string OrderLineItemUDF1New = string.Empty;
                string OrderLineItemUDF2New = string.Empty;
                string OrderLineItemUDF3New = string.Empty;
                string OrderLineItemUDF4New = string.Empty;
                string OrderLineItemUDF5New = string.Empty;
                string CartItemQuantityNew = string.Empty;

                List<CartItemDTO> lstGroupedCartItems = new List<CartItemDTO>();
                int LineItemCount = 0;
                AutoOrderNumberGenerate objAutoNumber = null;

                foreach (var item in lstcartitemsgrped)
                {
                    OrderLineItemUDF1New = string.Empty;
                    OrderLineItemUDF2New = string.Empty;
                    OrderLineItemUDF3New = string.Empty;
                    OrderLineItemUDF4New = string.Empty;
                    OrderLineItemUDF5New = string.Empty;
                    CartItemQuantityNew = string.Empty;
                    //lstGroupedCartItems = lstCartItems.Where(t => t.SupplierId == item.SupplierId && t.BlanketPOID == item.BlanketPOID && t.ReplenishType == "SuggestedReturn").ToList();
                    lstGroupedCartItems = lstCartItems.Where(t => t.SupplierId == item.SupplierId && t.ReplenishType == "SuggestedReturn").ToList();
                    LineItemCount = lstGroupedCartItems.Count;

                    if (item.MaxOrderSize != null && item.MaxOrderSize > 0 && LineItemCount > item.MaxOrderSize)
                    {
                        decimal divfactor = (LineItemCount / (item.MaxOrderSize ?? 1));
                        decimal modfactor = (LineItemCount % (item.MaxOrderSize ?? 1));
                        int divfactorint = (int)divfactor;
                        if (modfactor > 0)
                        {
                            divfactorint = divfactorint + 1;
                        }
                        bool isSameSuppier = false;
                        for (int i = 1; i <= divfactorint; i++)
                        {
                            string orderNumber = string.Empty;
                            string orderNumberSorting = string.Empty;

                            //if ((item.BlanketPOID ?? 0) > 0 && !string.IsNullOrWhiteSpace(item.BlanketPONumber))
                            //{
                            //    orderNumber = item.BlanketPONumber;
                            //    objAutoNumber = new AutoOrderNumberGenerate() { OrderNumber = orderNumber };
                            //}
                            //else
                            //{
                            objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextOrderNumber(RoomId, CompanyId, item.SupplierId, EnterpriseID, objAutoNumber, isSameSuppier);
                            //if (objAutoNumber.IsBlanketPO)
                            //    orderNumber = objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTimeUtility.DateTimeNow) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTimeUtility.DateTimeNow) >= DateTimeUtility.DateTimeNow).FirstOrDefault().BlanketPO;
                            //else
                            orderNumber = objAutoNumber.OrderNumber;

                            orderNumberSorting = objAutoNumber.OrderNumberForSorting;
                            isSameSuppier = true;
                            //}
                            if (orderNumber == null || string.IsNullOrWhiteSpace(orderNumber))
                            {
                                orderNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
                            }
                            int ReleaseNo = 1;
                            if (!string.IsNullOrWhiteSpace(orderNumber))
                            {
                                OrderMasterDAL objOrderDAL = new OrderMasterDAL(DatabaseName);
                                var maximumReleaseNumberByOrderNo = objOrderDAL.GetMaximumReleaseNoByOrderNumber(RoomId, CompanyId, orderNumber, OrderType.RuturnOrder);
                                if (maximumReleaseNumberByOrderNo > 0)
                                    ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                            }

                            if (string.IsNullOrEmpty(orderNumberSorting))
                                orderNumberSorting = orderNumber;

                            int DReqOrderDays = 0;
                            objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierId);
                            if (objSupplierDTO != null)
                            {
                                DReqOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);
                            }


                            OrderMasterDTO objDTO = new OrderMasterDTO()
                            {
                                OrderType = (int)OrderType.RuturnOrder,
                                RequiredDate = CurrentDateTime.AddDays(DReqOrderDays),
                                OrderNumber = orderNumber,
                                OrderStatus = (int)OrderStatus.UnSubmitted,
                                ReleaseNumber = ReleaseNo.ToString(),
                                LastUpdated = DateTime.UtcNow,
                                Created = DateTime.UtcNow,
                                Supplier = item.SupplierId,
                                SupplierName = item.SupplierName,
                                CreatedBy = UserId,
                                LastUpdatedBy = UserId,
                                CompanyID = CompanyId,
                                Room = RoomId,
                                OrderDate = CurrentDateTime,
                                AutoOrderNumber = objAutoNumber,
                                IsBlanketOrder = objAutoNumber.IsBlanketPO,
                                OrderLineItemsIds = OrderLineItemIds,
                                OrderNumber_ForSorting = orderNumberSorting,
                                RequiredDateString = string.Empty,
                                OrderLineItemUDF1 = OrderLineItemUDF1,
                                OrderLineItemUDF2 = OrderLineItemUDF2,
                                OrderLineItemUDF3 = OrderLineItemUDF3,
                                OrderLineItemUDF4 = OrderLineItemUDF4,
                                OrderLineItemUDF5 = OrderLineItemUDF5,
                                CartQuantityString = OrderItemQuantity
                            };
                            if (i == divfactorint)
                            {
                                objDTO.NoOfLineItems = LineItemCount - ((i - 1) * (int)item.MaxOrderSize);
                                OrderLineItemIds = string.Join(",", lstGroupedCartItems.Skip((i - 1) * ((int)item.MaxOrderSize)).Take((objDTO.NoOfLineItems ?? 0)).Select(t => t.GUID));
                                objDTO.OrderLineItemsIds = OrderLineItemIds;

                                if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                                {
                                    string[] AllGuid = Ids.Split(',').ToArray();
                                    string[] udf1 = OrderLineItemUDF1.Split(',').ToArray();
                                    string[] udf2 = OrderLineItemUDF2.Split(',').ToArray();
                                    string[] udf3 = OrderLineItemUDF3.Split(',').ToArray();
                                    string[] udf4 = OrderLineItemUDF4.Split(',').ToArray();
                                    string[] udf5 = OrderLineItemUDF5.Split(',').ToArray();
                                    string[] cartitemqty = OrderItemQuantity.Split(',').ToArray();
                                    foreach (string guid in OrderLineItemIds.Split(','))
                                    {
                                        //AllGuid.Where(x => x.v)
                                        int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                        if (udf1.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF1New))
                                            {
                                                OrderLineItemUDF1New = udf1[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF1New = OrderLineItemUDF1New + "," + udf1[Index];
                                            }
                                        }
                                        if (udf2.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF2New))
                                            {
                                                OrderLineItemUDF2New = udf2[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF2New = OrderLineItemUDF2New + "," + udf2[Index];
                                            }
                                        }
                                        if (udf3.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF3New))
                                            {
                                                OrderLineItemUDF3New = udf3[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF3New = OrderLineItemUDF3New + "," + udf3[Index];
                                            }
                                        }
                                        if (udf4.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF4New))
                                            {
                                                OrderLineItemUDF4New = udf4[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF4New = OrderLineItemUDF4New + "," + udf4[Index];
                                            }
                                        }
                                        if (udf5.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF5New))
                                            {
                                                OrderLineItemUDF5New = udf5[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF5New = OrderLineItemUDF5New + "," + udf5[Index];
                                            }
                                        }
                                        if (cartitemqty.Length > Index)
                                        {
                                            CartItemQuantityNew = cartitemqty[Index];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                objDTO.NoOfLineItems = (int)item.MaxOrderSize;
                                OrderLineItemIds = string.Join(",", lstGroupedCartItems.Skip((i - 1) * ((int)item.MaxOrderSize)).Take(((int)item.MaxOrderSize)).Select(t => t.GUID));
                                objDTO.OrderLineItemsIds = OrderLineItemIds;

                                if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                                {
                                    string[] AllGuid = Ids.Split(',').ToArray();
                                    string[] udf1 = OrderLineItemUDF1.Split(',').ToArray();
                                    string[] udf2 = OrderLineItemUDF2.Split(',').ToArray();
                                    string[] udf3 = OrderLineItemUDF3.Split(',').ToArray();
                                    string[] udf4 = OrderLineItemUDF4.Split(',').ToArray();
                                    string[] udf5 = OrderLineItemUDF5.Split(',').ToArray();
                                    string[] cartitemqty = OrderItemQuantity.Split(',').ToArray();
                                    foreach (string guid in OrderLineItemIds.Split(','))
                                    {
                                        //AllGuid.Where(x => x.v)
                                        int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                        if (udf1.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF1New))
                                            {
                                                OrderLineItemUDF1New = udf1[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF1New = OrderLineItemUDF1New + "," + udf1[Index];
                                            }
                                        }
                                        if (udf2.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF2New))
                                            {
                                                OrderLineItemUDF2New = udf2[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF2New = OrderLineItemUDF2New + "," + udf2[Index];
                                            }
                                        }
                                        if (udf3.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF3New))
                                            {
                                                OrderLineItemUDF3New = udf3[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF3New = OrderLineItemUDF3New + "," + udf3[Index];
                                            }
                                        }
                                        if (udf4.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF4New))
                                            {
                                                OrderLineItemUDF4New = udf4[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF4New = OrderLineItemUDF4New + "," + udf4[Index];
                                            }
                                        }
                                        if (udf5.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(OrderLineItemUDF5New))
                                            {
                                                OrderLineItemUDF5New = udf5[Index];
                                            }
                                            else
                                            {
                                                OrderLineItemUDF5New = OrderLineItemUDF5New + "," + udf5[Index];
                                            }
                                        }
                                        if (cartitemqty.Length > Index)
                                        {
                                            CartItemQuantityNew = cartitemqty[Index];
                                        }
                                    }
                                }

                            }
                            if (objDTO.IsBlanketOrder)
                            {
                                IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                                                                                              where x != null
                                                                                              && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.Now).ToShortDateString()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                              && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.Now).ToShortDateString()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                              select x);
                                if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                                {
                                    objDTO.BlanketOrderNumberID = objSuppBlnkPOList.FirstOrDefault().ID;
                                }

                            }
                            objDTO.OrderLineItemUDF1 = OrderLineItemUDF1New;
                            objDTO.OrderLineItemUDF2 = OrderLineItemUDF2New;
                            objDTO.OrderLineItemUDF3 = OrderLineItemUDF3New;
                            objDTO.OrderLineItemUDF4 = OrderLineItemUDF4New;
                            objDTO.OrderLineItemUDF5 = OrderLineItemUDF5New;
                            objDTO.CartQuantityString = CartItemQuantityNew;
                            objDTO.IsOrderSelected = true;
                            lstOrders.Add(objDTO);
                        }

                    }
                    else
                    {
                        string orderNumber = string.Empty;
                        string orderNumberSorting = string.Empty;
                        //if ((item.BlanketPOID ?? 0) > 0 && !string.IsNullOrWhiteSpace(item.BlanketPONumber))
                        //{
                        //    orderNumber = item.BlanketPONumber;
                        //    objAutoNumber = new AutoOrderNumberGenerate() { OrderNumber = orderNumber };
                        //}
                        //else
                        //{
                        objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextOrderNumber(RoomId, CompanyId, item.SupplierId, EnterpriseID, objAutoNumber);
                        //if (objAutoNumber.IsBlanketPO && objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTimeUtility.DateTimeNow) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTimeUtility.DateTimeNow) >= DateTimeUtility.DateTimeNow).Any())
                        //    orderNumber = objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTimeUtility.DateTimeNow) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTimeUtility.DateTimeNow) >= DateTimeUtility.DateTimeNow).FirstOrDefault().BlanketPO;
                        //else
                        orderNumber = objAutoNumber.OrderNumber;
                        orderNumberSorting = objAutoNumber.OrderNumberForSorting;
                        //}
                        if (orderNumber == null || string.IsNullOrWhiteSpace(orderNumber))
                        {
                            orderNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
                        }
                        if (string.IsNullOrEmpty(orderNumberSorting))
                            orderNumberSorting = orderNumber;

                        int ReleaseNo = 1;

                        if (!string.IsNullOrWhiteSpace(orderNumber))
                        {
                            OrderMasterDAL objOrderDAL = new OrderMasterDAL(DatabaseName);
                            var maximumReleaseNumberByOrderNo = objOrderDAL.GetMaximumReleaseNoByOrderNumber(RoomId, CompanyId, orderNumber, OrderType.RuturnOrder);
                            if (maximumReleaseNumberByOrderNo > 0)
                                ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                        }
                        OrderLineItemIds = OrderLineItemIds = string.Join(",", lstGroupedCartItems.Select(t => t.GUID));
                        if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                        {
                            string[] AllGuid = Ids.Split(',').ToArray();
                            string[] udf1 = OrderLineItemUDF1.Split(',').ToArray();
                            string[] udf2 = OrderLineItemUDF2.Split(',').ToArray();
                            string[] udf3 = OrderLineItemUDF3.Split(',').ToArray();
                            string[] udf4 = OrderLineItemUDF4.Split(',').ToArray();
                            string[] udf5 = OrderLineItemUDF5.Split(',').ToArray();
                            string[] cartitemqty = OrderItemQuantity.Split(',').ToArray();
                            foreach (string guid in OrderLineItemIds.Split(','))
                            {
                                //AllGuid.Where(x => x.v)
                                int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                if (udf1.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF1New))
                                    {
                                        OrderLineItemUDF1New = udf1[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF1New = OrderLineItemUDF1New + "," + udf1[Index];
                                    }
                                }
                                if (udf2.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF2New))
                                    {
                                        OrderLineItemUDF2New = udf2[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF2New = OrderLineItemUDF2New + "," + udf2[Index];
                                    }
                                }
                                if (udf3.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF3New))
                                    {
                                        OrderLineItemUDF3New = udf3[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF3New = OrderLineItemUDF3New + "," + udf3[Index];
                                    }
                                }
                                if (udf4.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF4New))
                                    {
                                        OrderLineItemUDF4New = udf4[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF4New = OrderLineItemUDF4New + "," + udf4[Index];
                                    }
                                }
                                if (udf5.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(OrderLineItemUDF5New))
                                    {
                                        OrderLineItemUDF5New = udf5[Index];
                                    }
                                    else
                                    {
                                        OrderLineItemUDF5New = OrderLineItemUDF5New + "," + udf5[Index];
                                    }
                                }
                                if (cartitemqty.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(CartItemQuantityNew))
                                    {
                                        CartItemQuantityNew = cartitemqty[Index];
                                    }
                                    else
                                    {
                                        CartItemQuantityNew = CartItemQuantityNew + "," + cartitemqty[Index];
                                    }
                                }
                            }
                        }
                        int DefaultRequiredOrderDays = 0;
                        objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierId);
                        if (objSupplierDTO != null)
                        {
                            DefaultRequiredOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);
                        }

                        OrderMasterDTO objDTO = new OrderMasterDTO()
                        {
                            RequiredDate = CurrentDateTime.AddDays(DefaultRequiredOrderDays),
                            OrderNumber = orderNumber,
                            OrderStatus = (int)OrderStatus.UnSubmitted,
                            OrderType = (int)OrderType.RuturnOrder,
                            ReleaseNumber = ReleaseNo.ToString(),
                            LastUpdated = DateTime.UtcNow,
                            Created = DateTime.UtcNow,
                            Supplier = item.SupplierId,
                            SupplierName = item.SupplierName,
                            CreatedBy = UserId,
                            LastUpdatedBy = UserId,
                            CompanyID = CompanyId,
                            Room = RoomId,
                            OrderDate = CurrentDateTime,
                            AutoOrderNumber = objAutoNumber,
                            IsBlanketOrder = objAutoNumber.IsBlanketPO,
                            NoOfLineItems = LineItemCount,
                            OrderLineItemsIds = OrderLineItemIds,
                            OrderNumber_ForSorting = orderNumberSorting,
                            RequiredDateString = string.Empty,
                            OrderLineItemUDF1 = OrderLineItemUDF1New,
                            OrderLineItemUDF2 = OrderLineItemUDF2New,
                            OrderLineItemUDF3 = OrderLineItemUDF3New,
                            OrderLineItemUDF4 = OrderLineItemUDF4New,
                            OrderLineItemUDF5 = OrderLineItemUDF5New,
                            CartQuantityString = CartItemQuantityNew

                        };

                        if (objDTO.IsBlanketOrder)
                        {

                            IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                                                                                          where x != null
                                                                                          && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.Now).ToShortDateString()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                          && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.Now).ToShortDateString()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                          select x);
                            if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                            {
                                objDTO.BlanketOrderNumberID = objSuppBlnkPOList.FirstOrDefault().ID;
                            }

                        }
                        objDTO.IsOrderSelected = true;
                        lstOrders.Add(objDTO);
                    }
                }

                return lstOrders;
            }
            catch
            {
                throw;
            }

        }

        public List<OrderMasterDTO> CreateRetrunOrdersByCart(List<OrderMasterDTO> lstOrders, long RoomId, long CompanyId, long UserId, string EpDatabaseName, short SubmissionMethod, long EnterpriseId, out Dictionary<string, string> rejectedOrderLineItems, long SessionUserId, string callingFrom = "")
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            SupplierMasterDAL objSupplierDAl = new SupplierMasterDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            List<OrderMasterDTO> lstSuccessOrders = new List<OrderMasterDTO>();
            SupplierMasterDTO objSupplier = new SupplierMasterDTO();
            int ActualOrderStatus = 1;
            rejectedOrderLineItems = new Dictionary<string, string>();

            if (lstOrders != null && lstOrders.Count > 0)
            {
                CommonDAL objCommonDAL = new CommonDAL(EpDatabaseName);
                OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(EpDatabaseName);
                CartItemDAL objCartItemDAL = new CartItemDAL(EpDatabaseName);
                OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(EpDatabaseName);
                RoomDAL objRoomDAL = new RoomDAL(EpDatabaseName);
                // RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomId);
                string columnList = "ID,RoomName,PreventMaxOrderQty";
                RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");
                Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
                Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
                int rejectedDueToPreventMaxValidation = 0;
                BinMasterDAL objBinDAL = new BinMasterDAL(EpDatabaseName);
                List<Guid> unSuccessfulOrders = new List<Guid>();
                List<string> rejectedOrderLineItemsGuids = new List<string>();

                foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                {
                    OrderMasterDTO objReturnAfterSave = new OrderMasterDTO();

                    objSupplier = objSupplierDAl.GetSupplierByIDPlain(objOrderMasterDTO.Supplier.GetValueOrDefault(0));

                    if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderNumber))
                    {
                        OrderMasterDAL objOrderDAL = new OrderMasterDAL(EpDatabaseName);
                        objOrderMasterDTO.ReleaseNumber = Convert.ToString(objOrderDAL.GetNextReleaseNumber(objOrderMasterDTO.OrderNumber, objOrderMasterDTO.GUID, RoomId, CompanyId));
                    }
                    objOrderMasterDTO.OrderType = (int)OrderType.RuturnOrder;

                    ActualOrderStatus = objOrderMasterDTO.OrderStatus;
                    objOrderMasterDTO.OrderStatus = (int)OrderStatus.UnSubmitted;
                    objOrderMasterDTO.LastUpdatedBy = UserId;
                    objOrderMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objOrderMasterDTO.CreatedBy = UserId;
                    objOrderMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objOrderMasterDTO.Room = RoomId;
                    objOrderMasterDTO.CompanyID = CompanyId;
                    objOrderMasterDTO.OrderDate = CurrentDateTime;
                    objOrderMasterDTO.StagingID = objCommonDAL.GetOrInsertMaterialStagingIDByName(objOrderMasterDTO.StagingName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.MaterialStagingGUID = objCommonDAL.GetOrInsertMaterialStagingGUIDByName(objOrderMasterDTO.StagingName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.ShipVia = objCommonDAL.GetOrInsertShipVaiIDByName(objOrderMasterDTO.ShipViaName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.CustomerID = null;
                    CustomerMasterDTO objCustomerMasterDTO = objCommonDAL.GetOrInsertCustomerGUIDByName(objOrderMasterDTO.CustomerName, UserId, RoomId, CompanyId);
                    if (objCustomerMasterDTO != null)
                    {
                        objOrderMasterDTO.CustomerGUID = objCustomerMasterDTO.GUID;
                    }
                    objOrderMasterDTO.ShippingVendor = objCommonDAL.GetOrInsertVendorIDByName(objOrderMasterDTO.ShippingVendorName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.GUID = Guid.NewGuid();
                    objOrderMasterDTO.WhatWhereAction = "cart";
                    objOrderMasterDTO.AddedFrom = "Web";
                    objOrderMasterDTO.EditedFrom = "Web";
                    objOrderMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objOrderMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    objReturnAfterSave = objOrderMasterDAL.InsertOrder(objOrderMasterDTO, SessionUserId);

                    List<Guid> lstids = new List<Guid>();
                    if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                    {
                        foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                        {
                            Guid tempid = Guid.Empty;
                            if (Guid.TryParse(item, out tempid))
                            {
                                lstids.Add(tempid);
                            }
                        }
                    }
                    if (lstids.Count > 0)
                    {
                        List<CartItemDTO> lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, RoomId, CompanyId, true, null);
                        string[] AllGuid = objOrderMasterDTO.OrderLineItemsIds.Split(',').ToArray();
                        string[] udf1;
                        string[] udf2;
                        string[] udf3;
                        string[] udf4;
                        string[] udf5;
                        string[] cartItemQuantity;
                        if (objOrderMasterDTO.OrderLineItemUDF1 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF1)))
                        {
                            udf1 = objOrderMasterDTO.OrderLineItemUDF1.Split(',').ToArray();
                        }
                        else
                        {
                            udf1 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF2 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF2)))
                        {
                            udf2 = objOrderMasterDTO.OrderLineItemUDF2.Split(',').ToArray();
                        }
                        else
                        {
                            udf2 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF3 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF3)))
                        {
                            udf3 = objOrderMasterDTO.OrderLineItemUDF3.Split(',').ToArray();
                        }
                        else
                        {
                            udf3 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF4 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF4)))
                        {
                            udf4 = objOrderMasterDTO.OrderLineItemUDF4.Split(',').ToArray();
                        }
                        else
                        {
                            udf4 = null;
                        }
                        if (objOrderMasterDTO.OrderLineItemUDF5 != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemUDF5)))
                        {
                            udf5 = objOrderMasterDTO.OrderLineItemUDF5.Split(',').ToArray();
                        }
                        else
                        {
                            udf5 = null;
                        }
                        if (objOrderMasterDTO.CartQuantityString != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.CartQuantityString)))
                        {
                            cartItemQuantity = objOrderMasterDTO.CartQuantityString.Split(',').ToArray();
                        }
                        else
                        {
                            cartItemQuantity = null;
                        }

                        long temp_CreatedBy = UserId;
                        long temp_LastUpdatedBy = UserId;

                        foreach (CartItemDTO cartitem in lstCartItems)
                        {

                            if ((callingFrom ?? string.Empty).ToLower() == "service")
                            {
                                if (cartitem.CreatedBy.GetValueOrDefault(0) > 0)
                                {
                                    temp_CreatedBy = cartitem.CreatedBy.GetValueOrDefault(0);
                                }

                                if (cartitem.LastUpdatedBy.GetValueOrDefault(0) > 0)
                                {
                                    temp_LastUpdatedBy = cartitem.LastUpdatedBy.GetValueOrDefault(0);
                                }
                            }

                            int Index = Array.FindIndex(AllGuid, row => row.Contains(cartitem.GUID.ToString()));

                            string UDF1 = string.Empty;
                            string UDF2 = string.Empty;
                            string UDF3 = string.Empty;
                            string UDF4 = string.Empty;
                            string UDF5 = string.Empty;
                            if (udf1 != null && udf1.Length > 0)
                            {
                                UDF1 = udf1[Index];
                            }
                            if (udf2 != null && udf2.Length > 0)
                            {
                                UDF2 = udf2[Index];
                            }
                            if (udf3 != null && udf3.Length > 0)
                            {
                                UDF3 = udf3[Index];
                            }
                            if (udf4 != null && udf4.Length > 0)
                            {
                                UDF4 = udf4[Index];
                            }
                            if (udf5 != null && udf5.Length > 0)
                            {
                                UDF5 = udf5[Index];
                            }

                            OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();

                            objOrderDetailsDTO.OrderGUID = objReturnAfterSave.GUID;

                            objOrderDetailsDTO.ItemGUID = cartitem.ItemGUID;
                            objOrderDetailsDTO.Bin = cartitem.BinId;
                            double? quantityToset = cartitem.Quantity;
                            if (cartItemQuantity != null && cartItemQuantity.Length > 0)
                            {
                                try
                                {
                                    quantityToset = Convert.ToDouble(cartItemQuantity[Index]);
                                }
                                catch (Exception) { }
                            }

                            objOrderDetailsDTO.RequestedQuantity = quantityToset;
                            if (SubmissionMethod == 2 || ActualOrderStatus > 2)
                            {
                                objOrderDetailsDTO.ApprovedQuantity = quantityToset;
                            }
                            //objOrderDetailsDTO.RequiredDate = objReturnAfterSave.RequiredDate;
                            objOrderDetailsDTO.RequiredDate = cartitem.LeadTimeInDays.GetValueOrDefault(0) > 0 ? CurrentDateTime.AddDays(cartitem.LeadTimeInDays.GetValueOrDefault(0)) : objReturnAfterSave.RequiredDate;
                            objOrderDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.CreatedBy = temp_CreatedBy;
                            objOrderDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.LastUpdatedBy = temp_LastUpdatedBy;
                            objOrderDetailsDTO.Room = RoomId;
                            objOrderDetailsDTO.CompanyID = CompanyId;
                            objOrderDetailsDTO.AddedFrom = "Web";
                            objOrderDetailsDTO.EditedFrom = "Web";
                            objOrderDetailsDTO.UDF1 = UDF1;
                            objOrderDetailsDTO.UDF2 = UDF2;
                            objOrderDetailsDTO.UDF3 = UDF3;
                            objOrderDetailsDTO.UDF4 = UDF4;
                            objOrderDetailsDTO.UDF5 = UDF5;
                            objOrderDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), RoomId, CompanyId);

                            if (objItemMasterDTO != null)
                            {
                                CostUOMMasterDTO costUOM = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                                if (costUOM == null)
                                    costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                                if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                                {
                                    costUOM.CostUOMValue = 1;
                                }

                                objOrderDetailsDTO.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objOrderMasterDTO.OrderStatus <= 2 ? (objOrderDetailsDTO.RequestedQuantity * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                                                             : (objOrderDetailsDTO.ApprovedQuantity.GetValueOrDefault(0) * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));

                                objOrderDetailsDTO.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objOrderMasterDTO.OrderStatus <= 2 ? (objOrderDetailsDTO.RequestedQuantity * (objItemMasterDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                                                             : (objOrderDetailsDTO.ApprovedQuantity.GetValueOrDefault(0) * (objItemMasterDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));

                                OrderUOMMasterDTO OrderUOM = new OrderUOMMasterDAL(base.DataBaseName).GetRecord(objItemMasterDTO.OrderUOMID.GetValueOrDefault(0), objItemMasterDTO.Room.GetValueOrDefault(0), objItemMasterDTO.CompanyID.GetValueOrDefault(0), false, false);
                                if (OrderUOM == null)
                                    OrderUOM = new OrderUOMMasterDTO() { OrderUOMValue = 1 };

                                if (OrderUOM.OrderUOMValue == null || OrderUOM.OrderUOMValue <= 0)
                                {
                                    OrderUOM.OrderUOMValue = 1;
                                }

                                if (objOrderDetailsDTO.RequestedQuantity != null && objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0) > 0 && objItemMasterDTO.IsAllowOrderCostuom)
                                {
                                    objOrderDetailsDTO.RequestedQuantityUOM = objOrderDetailsDTO.RequestedQuantity / OrderUOM.OrderUOMValue;
                                }
                                else
                                {
                                    objOrderDetailsDTO.RequestedQuantityUOM = objOrderDetailsDTO.RequestedQuantity;
                                }

                                if (objOrderDetailsDTO.ApprovedQuantity != null && objOrderDetailsDTO.ApprovedQuantity.GetValueOrDefault(0) > 0 && objItemMasterDTO.IsAllowOrderCostuom)
                                {
                                    objOrderDetailsDTO.ApprovedQuantityUOM = objOrderDetailsDTO.ApprovedQuantity / OrderUOM.OrderUOMValue;
                                }
                                else
                                {
                                    objOrderDetailsDTO.ApprovedQuantityUOM = objOrderDetailsDTO.ApprovedQuantity;
                                }

                            }
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
                                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomId, CompanyId, UserId);
                                currentCulture = objeTurnsRegionInfo.CultureName;
                            }
                            string ResourceFileCartItem = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCartItem", currentCulture, EnterpriseId, CompanyId);
                            string MsgItemMaximumQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemMaximumQuantity", ResourceFileCartItem, EnterpriseId, CompanyId, RoomId, "ResCartItem", currentCulture);
                            string MsgBinMaximumQuantity = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBinMaximumQuantity", ResourceFileCartItem, EnterpriseId, CompanyId, RoomId, "ResCartItem", currentCulture);

                            if (objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder)
                            {
                                if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.Value)
                                {
                                    if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0) + objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0)) > objItemMasterDTO.MaximumQuantity.Value)
                                    {
                                        if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                        {
                                            rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = MsgItemMaximumQuantity;
                                        }
                                        rejectedDueToPreventMaxValidation++;
                                        unSuccessfulOrders.Add(objReturnAfterSave.GUID);
                                        rejectedOrderLineItemsGuids.Add(Convert.ToString(cartitem.GUID));
                                        continue;
                                    }
                                }
                                else
                                {
                                    List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemMasterDTO.GUID, RoomId, CompanyId).OrderBy(x => x.BinNumber).ToList();
                                    var maxQtyAtBinLevel = lstItemBins.Where(e => e.BinNumber.Equals(cartitem.BinName)).FirstOrDefault();
                                    var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : cartitem.BinId.GetValueOrDefault(0);
                                    var onOrderQtyAtBin = objOrderDetailsDAL.GetOrderdQtyOfItemBinWise(RoomId, CompanyId, objItemMasterDTO.GUID, tmpBinId);

                                    if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && (onOrderQtyAtBin + objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0)) > maxQtyAtBinLevel.MaximumQuantity.Value)
                                    {
                                        if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                        {
                                            rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = MsgBinMaximumQuantity;
                                        }
                                        rejectedDueToPreventMaxValidation++;
                                        unSuccessfulOrders.Add(objReturnAfterSave.GUID);
                                        rejectedOrderLineItemsGuids.Add(Convert.ToString(cartitem.GUID));
                                        continue;
                                    }
                                }
                            }

                            OrderDetailsDTO returnOrderDetailsDTO = new OrderDetailsDTO();
                            returnOrderDetailsDTO = objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId, EnterpriseId);
                            ItemMasterDAL itemmasterobj = new ItemMasterDAL(base.DataBaseName);
                            itemmasterobj.EditDate(cartitem.ItemGUID ?? Guid.Empty, "EditOrderedDate");

                            // MaintainTransaction History for Cart To ReturnOrder
                            if (returnOrderDetailsDTO != null)
                            {
                                objCartItemDAL.InsertCartQuoteTransitionDetail(cartitem.GUID, null, returnOrderDetailsDTO.GUID, null, (int)TransactionConversionType.CarttoReturn, UserId);
                            }

                        }
                        if (objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder && rejectedOrderLineItemsGuids.Any())
                        {
                            List<string> Items = objOrderMasterDTO.OrderLineItemsIds.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                            foreach (var guid in rejectedOrderLineItemsGuids)
                            {
                                Items.Remove(guid);
                            }
                            objOrderMasterDTO.OrderLineItemsIds = string.Join(",", Items.ToArray());
                        }
                        if (!string.IsNullOrEmpty(objOrderMasterDTO.OrderLineItemsIds))
                        {
                            objCartItemDAL.DeleteRecords(objOrderMasterDTO.OrderLineItemsIds, UserId, CompanyId, EnterpriseId, SessionUserId);
                        }

                    }

                    //--------------------------------------------------------------------
                    //
                    objOrderMasterDTO.OrderStatus = ActualOrderStatus;
                    if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved)
                    {
                        if (!objSupplier.IsSendtoVendor)
                        {
                            objOrderMasterDTO.OrderStatus = (int)OrderStatus.Transmitted;
                        }
                    }
                    if (SubmissionMethod == 2)
                    {
                        if (!objSupplier.IsSendtoVendor)
                        {
                            objOrderMasterDTO.OrderStatus = (int)OrderStatus.Transmitted;
                        }
                        else
                        {
                            objOrderMasterDTO.OrderStatus = (int)OrderStatus.Approved;
                        }
                    }

                    if (!unSuccessfulOrders.Contains(objReturnAfterSave.GUID))
                    {
                        objOrderMasterDAL.UpdateOrderStatus(objReturnAfterSave.GUID, null, objOrderMasterDTO.OrderStatus);

                        OrderMasterDTO objOMDto = objOrderMasterDAL.GetOrderByGuidPlain(objReturnAfterSave.GUID);
                        if (objOMDto != null)
                        {
                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Submitted)
                            {
                                objOMDto.RequesterID = UserId;
                            }
                            else if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved
                                    || objOrderMasterDTO.OrderStatus == (int)OrderStatus.Transmitted)
                            {
                                objOMDto.RequesterID = UserId;
                                objOMDto.ApproverID = UserId;
                            }
                            objOrderMasterDAL.Edit(objOMDto);
                        }
                        objReturnAfterSave.OrderStatus = objOrderMasterDTO.OrderStatus;
                        lstSuccessOrders.Add(objReturnAfterSave);
                    }
                }
            }
            return lstSuccessOrders;
        }

        public void SuggestedReturnOrderRoom(long RoomId, long CompanyId, long UserId, long EnterpriseID, long SessionUserId, bool isAfterSync = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.FirstOrDefault(t => t.ID == RoomId && t.IsDeleted == false);
                if (objRoom != null)
                {
                    #region Suggested Return

                    if (objRoom.SuggestedReturn)
                    {
                        List<Guid> AllRoomItems;
                        if (isAfterSync)
                        {
                            var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyId", CompanyId) };
                            List<Guid> oItemGuid = context.Database.SqlQuery<Guid>("exec [GetSuggestedReturnOrderItemGuids] @RoomId,@CompanyId", params1).ToList();
                            AllRoomItems = oItemGuid;
                        }
                        else
                        {
                            AllRoomItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
                        }

                        if (AllRoomItems != null && AllRoomItems.Any())
                        {
                            foreach (var item in AllRoomItems)
                            {
                                AutoCartForSuggestedReturnUpdateByCode(item, UserId, "web", "Replenish >> Suggested Return", SessionUserId);
                            }
                        }
                    }
                    else
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                           new SqlParameter("@CompanyID", CompanyId),
                                                           new SqlParameter("@userid", UserId),
                                                           new SqlParameter("@editedfrom", "web"),
                                                           new SqlParameter("@calledfromfunctionname", "Web all cart from Room"),
                                                           new SqlParameter("@ReplenishType", "SuggestedReturn")
                                                         };
                        context.Database.ExecuteSqlCommand("EXEC [DeleteSuggestedReturnCartFromRoomSave] @RoomID,@CompanyID,@userid,@editedfrom,@calledfromfunctionname,@ReplenishType", params1);

                        //string DeleteSOferqry = "UPDATE CartItem SET IsDeleted = 1, editedfrom = 'web', lastupdatedby = 2, updated = GETUTCDATE(), whatwhereaction = 'Web all cart from Room', receivedon = GETUTCDATE() WHERE Room = " + RoomId + " AND CompanyID = " + CompanyId + " AND isnull(IsDeleted, 0) = 0 AND ReplenishType = 'SuggestedReturn' AND IsAutoMatedEntry = 1; UPDATE ItemMaster  SET SuggestedReturnQuantity = 0 WHERE isnull(Isdeleted, 0) = 0 AND Room = " + RoomId + " AND CompanyID = " + CompanyId + " and isnull(SuggestedReturnQuantity,0) > 0";
                        //context.Database.ExecuteSqlCommand(DeleteSOferqry);
                    }

                    #endregion
                }
            }
        }

        public List<CartItemDTO> AutoCartForSuggestedReturnUpdateByCode(Guid ItemGUID, long UserId, string CalledFrom, string calledFromFunctionName, long SessionUserId)
        {
            List<CartItemDTO> lstCarts = new List<CartItemDTO>();
            List<CartItemDTO> lstCartsChanges = new List<CartItemDTO>();
            List<CartItemDTO> lstCartsBC = new List<CartItemDTO>();
            List<CartItemDTO> lstCartsBM = new List<CartItemDTO>();
            long EnterpriseID = 0;
            long CompanyID = 0;
            long RoomID = 0;
            string RoomName = "";
            //string WhatWhereAction = GetCallStackData(calledFromFunctionName, Environment.StackTrace);
            string WhatWhereAction = calledFromFunctionName;
            // bool sendMail = true;
            //if (HttpContext.Current != null && HttpContext.Current.Session != null)
            //{
            //    long tempUSERID = 0;
            //    long.TryParse(Convert.ToString(HttpContext.Current.Session["UserID"]), out tempUSERID);
            //    if (tempUSERID > 0)
            //    {
            //        UserId = tempUSERID;
            //    }
            //}
            if (SessionUserId > 0 && UserId != SessionUserId)
            {
                UserId = SessionUserId;
            }
            //sendMail = Convert.ToBoolean(SendcartMail);


            var params1 = new SqlParameter[] { new SqlParameter("@itemguid", ItemGUID), new SqlParameter("@userid", UserId), new SqlParameter("@calledfrom", CalledFrom ?? (object)DBNull.Value), new SqlParameter("@calledfromFunctionName", WhatWhereAction ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstCarts = context.Database.SqlQuery<CartItemDTO>("exec [CreateautoSuggestedReturn] @itemguid,@userid,@calledfrom,@calledfromFunctionName", params1).ToList();
            }


            lstCartsChanges = lstCarts.Where(t => (t.isstchanged ?? false) == true || (t.issochanged ?? false) == true).ToList();
            if (lstCartsChanges != null && lstCartsChanges.Count > 0)
            {
                EnterpriseID = lstCartsChanges.First().EnterpriseId;
                CompanyID = lstCartsChanges.First().CompanyID ?? 0;
                RoomID = lstCartsChanges.First().Room ?? 0;
                if (HttpContext.Current != null && lstCarts != null && lstCarts.Count > 0)
                {
                    HttpRuntime.Cache["LastCalledEach"] = DateTime.UtcNow;
                    if (RedCircleStatic.SignalGroups == null)
                    {
                        RedCircleStatic.SignalGroups = new List<ECR>();
                    }
                    if (!RedCircleStatic.SignalGroups.Any(t => t.EID == EnterpriseID && t.CID == CompanyID && t.RID == RoomID))
                    {
                        RedCircleStatic.SignalGroups.Add(new ECR() { EID = EnterpriseID, CID = CompanyID, RID = RoomID, IsProcessed = false });
                    }
                }
                lstCartsBC = lstCartsChanges.Where(t => t.IsItemBelowCritcal == true || t.IsItemLocationBelowCritcal == true).ToList();
                lstCartsBM = lstCartsChanges.Where(t => t.IsItemBelowMinimum == true || t.IsItemLocationBelowMinimum == true).ToList();
                if (lstCartsBC != null && lstCartsBC.Count > 0)
                {
                    SendMailForSuggestedOrder(string.Empty, 0, 0, 0, RoomID, CompanyID, "Critical", RoomName, UserId, "SuggestedOrdersCritical", EnterpriseID, 0, string.Empty);
                }
                else if (lstCartsBM != null && lstCartsBM.Count > 0)
                {
                    SendMailForSuggestedOrder(string.Empty, 0, 0, 0, RoomID, CompanyID, "Minimum", RoomName, UserId, "SuggestedOrdersMinimum", EnterpriseID, 0, string.Empty);
                }

            }
            return lstCarts;
        }

        public List<NarrowSearchDTO> GetCartItemsByGuidsForNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount, string ReplenishType)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@ReplenishType", ReplenishType ?? (object)DBNull.Value)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetCartItemsByGuidsForNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@ReplenishType", params1).ToList();
            }


            //eTurnsRegionInfo objeTurnsRegionInfo = null;
            //RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyId, 0);
            //if (objeTurnsRegionInfo == null)
            //{
            //    objeTurnsRegionInfo = new eTurnsRegionInfo();
            //}
            //List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            //string strSupplierIds = string.Empty;

            //if (SupplierIds != null && SupplierIds.Any())
            //{
            //    strSupplierIds = string.Join(",", SupplierIds);
            //}

            //var params1 = new SqlParameter[] {
            //        new SqlParameter("@RoomId", RoomID),
            //        new SqlParameter("@CompanyId", CompanyId),
            //        new SqlParameter("@ReplenishType", ReplenishType),
            //        new SqlParameter("@SupplierIds", strSupplierIds)
            //    };

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    lstCartItems = (from ci in context.Database.SqlQuery<CartItemDTO>("exec [GetCartItemsByGuidsForNarrowSearch] @RoomId,@CompanyId,@ReplenishType,@SupplierIds", params1)
            //                    select new CartItemDTO
            //                    {
            //                        ID = ci.ID,
            //                        ItemNumber = ci.ItemNumber,
            //                        ItemGUID = ci.ItemGUID,
            //                        Quantity = Math.Round(ci.Quantity ?? 0, objeTurnsRegionInfo.NumberDecimalDigits),
            //                        Status = ci.Status,
            //                        ReplenishType = ci.ReplenishType,
            //                        IsKitComponent = ci.IsKitComponent,
            //                        UDF1 = ci.UDF1,
            //                        UDF2 = ci.UDF2,
            //                        UDF3 = ci.UDF3,
            //                        UDF4 = ci.UDF4,
            //                        UDF5 = ci.UDF5,
            //                        GUID = ci.GUID,
            //                        Created = ci.Created,
            //                        Updated = ci.Updated,
            //                        CreatedBy = ci.CreatedBy,
            //                        LastUpdatedBy = ci.LastUpdatedBy,
            //                        IsDeleted = ci.IsDeleted,
            //                        IsArchived = ci.IsArchived,
            //                        CompanyID = ci.CompanyID,
            //                        Room = ci.Room,
            //                        CreatedByName = ci.CreatedByName,
            //                        UpdatedByName = ci.UpdatedByName,
            //                        RoomName = ci.RoomName,
            //                        IsAutoMatedEntry = ci.IsAutoMatedEntry,
            //                        SupplierId = ci.SupplierId,
            //                        SupplierName = ci.SupplierName,
            //                        BlanketPOID = ci.BlanketPOID,
            //                        BlanketPONumber = ci.BlanketPONumber,
            //                        BinId = ci.BinId,
            //                        BinName = ci.BinName,
            //                        ReceivedOn = ci.ReceivedOn,
            //                        ReceivedOnWeb = ci.ReceivedOnWeb,
            //                        AddedFrom = ci.AddedFrom,
            //                        EditedFrom = ci.EditedFrom,
            //                        ItemUDF1 = ci.ItemUDF1,
            //                        ItemUDF2 = ci.ItemUDF2,
            //                        ItemUDF3 = ci.ItemUDF3,
            //                        ItemUDF4 = ci.ItemUDF4,
            //                        ItemUDF5 = ci.ItemUDF5,
            //                        OnHandQuantity = ci.OnHandQuantity,
            //                        CriticalQuantity = ci.CriticalQuantity,
            //                        MinimumQuantity = ci.MinimumQuantity,
            //                        MaximumQuantity = ci.MaximumQuantity,
            //                        CategoryID = ci.CategoryID,
            //                        CategoryName = ci.CategoryName
            //                    }).ToList();
            //}
            //return lstCartItems;
        }

        #endregion
        public List<CartItemDTO> GetCartItemsMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CartItemDTO>("exec [GetCartItemsMasterChangeLog] @GUID", params1).ToList();
            }
        }
        #endregion

        #region Create Quote From Cart

        public IList<QuoteMasterDTO> GetQuotesByCartIds(string Ids, long RoomId, long CompanyId, long UserId, string DatabaseName, string QuoteLineItemUDF1, string QuoteLineItemUDF2, string QuoteLineItemUDF3, string QuoteLineItemUDF4, string QuoteLineItemUDF5, long EnterpriseID, string OrderItemQuantity, string QuoteSuppliers)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            CartItemDAL objCartItemDAL = new CartItemDAL(DatabaseName);
            List<Guid> arrcartguids = new List<Guid>();
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            bool WithSelection;
            IList<QuoteMasterDTO> lstQuotes = new List<QuoteMasterDTO>();
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(base.DataBaseName);
            SupplierMasterDTO objSupplierDTO = new SupplierMasterDTO();

            try
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    WithSelection = true;
                }
                else
                {
                    WithSelection = false;
                }
                int LineItemCount = 0;
                AutoQuoteNumberGenerate objAutoNumber = null;
                LineItemCount = arrcartguids.Count;
                long SupplierID = 0;
                try
                {
                    List<string> SupplierIDs = new List<string>();
                    if (!string.IsNullOrWhiteSpace(QuoteSuppliers))
                    {
                        foreach (string item in QuoteSuppliers.Split(','))
                        {
                            if (item != "" && (!SupplierIDs.Contains(item)))
                            {
                                SupplierIDs.Add(item);
                            }
                        }
                    }
                    if (SupplierIDs.Count == 1)
                    {
                        SupplierID = Convert.ToInt64(SupplierIDs[0]);
                    }
                    else
                    {
                        SupplierID = 0;
                    }

                }
                catch (Exception ex)
                {
                    SupplierID = 0;
                }
                string quoteNumber = string.Empty;
                string quoteNumberSorting = string.Empty;
                objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextQuoteNumber(RoomId, CompanyId, EnterpriseID, SupplierID, objAutoNumber);
                quoteNumber = objAutoNumber.QuoteNumber;
                quoteNumberSorting = objAutoNumber.QuoteNumberForSorting;

                if (quoteNumber == null || string.IsNullOrWhiteSpace(quoteNumber))
                {
                    quoteNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
                }

                if (string.IsNullOrEmpty(quoteNumberSorting))
                    quoteNumberSorting = objAutoNumber.QuoteNumberForSorting;

                int ReleaseNo = 1;

                if (!string.IsNullOrWhiteSpace(quoteNumber))
                {
                    QuoteMasterDAL objQuoteDAL = new QuoteMasterDAL(DatabaseName);
                    ReleaseNo = objQuoteDAL.GetNextQuoteReleaseNumber(quoteNumber, null, RoomId, CompanyId);
                }

                objSupplierDTO = objSupDAL.GetSupplierByIDPlain(SupplierID);
                int DReqOrderDays = 0;
                if (objSupplierDTO != null)
                {
                    DReqOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);
                    if (objSupplierDTO.QuoteAutoSequence.GetValueOrDefault(0) == 1)
                    {
                        if (!string.IsNullOrWhiteSpace(objSupplierDTO.QuoteAutoNrReleaseNumber) && objSupplierDTO.NextQuoteNo.Trim() == quoteNumber.Trim())
                        {
                            ReleaseNo = Convert.ToInt32(objSupplierDTO.QuoteAutoNrReleaseNumber) + 1;
                        }
                    }
                }

                QuoteMasterDTO objDTO = new QuoteMasterDTO()
                {
                    //RequiredDate = CurrentDateTime.AddDays(DefaultRequiredOrderDays),
                    QuoteNumber = quoteNumber,
                    QuoteStatus = (int)QuoteStatus.UnSubmitted,
                    ReleaseNumber = ReleaseNo.ToString(),
                    LastUpdated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CreatedBy = UserId,
                    LastUpdatedBy = UserId,
                    CompanyID = CompanyId,
                    Room = RoomId,
                    QuoteDate = CurrentDateTime,
                    // AutoQuoteNumber = objAutoNumber,
                    //IsBlanketOrder = objAutoNumber.IsBlanketPO,
                    NoOfLineItems = LineItemCount,
                    QuoteLineItemsIds = Ids,
                    QuoteNumber_ForSorting = quoteNumberSorting,
                    RequiredDateString = string.Empty,
                    QuoteLineItemUDF1 = QuoteLineItemUDF1,
                    QuoteLineItemUDF2 = QuoteLineItemUDF2,
                    QuoteLineItemUDF3 = QuoteLineItemUDF3,
                    QuoteLineItemUDF4 = QuoteLineItemUDF4,
                    QuoteLineItemUDF5 = QuoteLineItemUDF5,
                    QuoteQuantityString = OrderItemQuantity,
                    SelectedSuppliers = QuoteSuppliers

                };
                objDTO.IsQuoteSelected = true;
                lstQuotes.Add(objDTO);
                return lstQuotes;
            }
            catch
            {
                throw;
            }
        }

        public List<QuoteMasterDTO> CreateQuotesByCart(List<QuoteMasterDTO> lstQuotes, long RoomId, long CompanyId, long UserId, string EpDatabaseName, short SubmissionMethod, long EnterpriseId, out Dictionary<string, string> rejectedQuoteLineItems, long SessionUserId, string callingFrom = "")
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            SupplierMasterDAL objSupplierDAl = new SupplierMasterDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            List<QuoteMasterDTO> lstSuccessQuotes = new List<QuoteMasterDTO>();
            SupplierMasterDTO objSupplier = new SupplierMasterDTO();
            int ActualQuoteStatus = 1;
            rejectedQuoteLineItems = new Dictionary<string, string>();

            if (lstQuotes != null && lstQuotes.Count > 0)
            {
                CommonDAL objCommonDAL = new CommonDAL(EpDatabaseName);
                QuoteMasterDAL objQuoteMasterDAL = new QuoteMasterDAL(EpDatabaseName);
                CartItemDAL objCartItemDAL = new CartItemDAL(EpDatabaseName);
                QuoteDetailDAL objQuoteDetailDAL = new QuoteDetailDAL(EpDatabaseName);

                Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
                Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
                BinMasterDAL objBinDAL = new BinMasterDAL(EpDatabaseName);
                List<Guid> unSuccessfulOrders = new List<Guid>();
                List<string> rejectedOrderLineItemsGuids = new List<string>();
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                QuoteMasterDAL objQuoteDAL = new QuoteMasterDAL(EpDatabaseName);

                foreach (QuoteMasterDTO objQuoteMasterDTO in lstQuotes)
                {
                    QuoteMasterDTO objReturnAfterSave = new QuoteMasterDTO();

                    objSupplier = objSupplierDAl.GetSupplierByIDPlain(objQuoteMasterDTO.Supplier.GetValueOrDefault(0));

                    if (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteNumber))
                    {
                        objQuoteMasterDTO.ReleaseNumber = Convert.ToString(objQuoteDAL.GetNextQuoteReleaseNumber(objQuoteMasterDTO.QuoteNumber, objQuoteMasterDTO.GUID, RoomId, CompanyId));
                    }
                    ActualQuoteStatus = objQuoteMasterDTO.QuoteStatus;
                    objQuoteMasterDTO.QuoteStatus = (int)QuoteStatus.UnSubmitted;
                    objQuoteMasterDTO.LastUpdatedBy = UserId;
                    objQuoteMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objQuoteMasterDTO.CreatedBy = UserId;
                    objQuoteMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objQuoteMasterDTO.Room = RoomId;
                    objQuoteMasterDTO.CompanyID = CompanyId;
                    objQuoteMasterDTO.QuoteDate = CurrentDateTime;
                    objQuoteMasterDTO.GUID = Guid.NewGuid();
                    objQuoteMasterDTO.WhatWhereAction = "cart";
                    objQuoteMasterDTO.AddedFrom = "Web";
                    objQuoteMasterDTO.EditedFrom = "Web";
                    objQuoteMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objQuoteMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                    {
                        objQuoteMasterDTO.RequesterID = UserId;
                    }
                    else if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Approved)
                    {
                        objQuoteMasterDTO.ApproverID = UserId;
                    }

                    objReturnAfterSave = objQuoteMasterDAL.InsertQuoteMaster(objQuoteMasterDTO);
                    objReturnAfterSave.QuotePrice = objQuoteMasterDTO.QuotePrice;
                    objReturnAfterSave.QuoteCost = objQuoteMasterDTO.QuoteCost;
                    objReturnAfterSave.QuoteSupplierIdsCSV = objQuoteMasterDTO.QuoteSupplierIdsCSV;
                    List<Guid> lstids = new List<Guid>();
                    List<Guid> itemGuidsToUpdate = new List<Guid>();

                    if (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteLineItemsIds))
                    {
                        foreach (var item in objQuoteMasterDTO.QuoteLineItemsIds.Split(','))
                        {
                            Guid tempid = Guid.Empty;
                            if (Guid.TryParse(item, out tempid))
                            {
                                lstids.Add(tempid);
                            }
                        }
                    }
                    if (lstids.Count > 0)
                    {
                        var tmpsupplierIds = new List<long>();
                        List<CartItemDTO> lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, RoomId, CompanyId, true, tmpsupplierIds);
                        string[] AllGuid = objQuoteMasterDTO.QuoteLineItemsIds.Split(',').ToArray();
                        string[] udf1;
                        string[] udf2;
                        string[] udf3;
                        string[] udf4;
                        string[] udf5;
                        string[] cartItemQuantity;
                        if (objQuoteMasterDTO.QuoteLineItemUDF1 != null && (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteLineItemUDF1)))
                        {
                            udf1 = objQuoteMasterDTO.QuoteLineItemUDF1.Split(',').ToArray();
                        }
                        else
                        {
                            udf1 = null;
                        }
                        if (objQuoteMasterDTO.QuoteLineItemUDF2 != null && (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteLineItemUDF2)))
                        {
                            udf2 = objQuoteMasterDTO.QuoteLineItemUDF2.Split(',').ToArray();
                        }
                        else
                        {
                            udf2 = null;
                        }
                        if (objQuoteMasterDTO.QuoteLineItemUDF3 != null && (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteLineItemUDF3)))
                        {
                            udf3 = objQuoteMasterDTO.QuoteLineItemUDF3.Split(',').ToArray();
                        }
                        else
                        {
                            udf3 = null;
                        }
                        if (objQuoteMasterDTO.QuoteLineItemUDF4 != null && (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteLineItemUDF4)))
                        {
                            udf4 = objQuoteMasterDTO.QuoteLineItemUDF4.Split(',').ToArray();
                        }
                        else
                        {
                            udf4 = null;
                        }
                        if (objQuoteMasterDTO.QuoteLineItemUDF5 != null && (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteLineItemUDF5)))
                        {
                            udf5 = objQuoteMasterDTO.QuoteLineItemUDF5.Split(',').ToArray();
                        }
                        else
                        {
                            udf5 = null;
                        }
                        if (objQuoteMasterDTO.QuoteQuantityString != null && (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteQuantityString)))
                        {
                            cartItemQuantity = objQuoteMasterDTO.QuoteQuantityString.Split(',').ToArray();
                        }
                        else
                        {
                            cartItemQuantity = null;
                        }

                        long temp_CreatedBy = UserId;
                        long temp_LastUpdatedBy = UserId;

                        foreach (CartItemDTO cartitem in lstCartItems)
                        {
                            if ((callingFrom ?? string.Empty).ToLower() == "service")
                            {
                                if (cartitem.CreatedBy.GetValueOrDefault(0) > 0)
                                {
                                    temp_CreatedBy = cartitem.CreatedBy.GetValueOrDefault(0);
                                }

                                if (cartitem.LastUpdatedBy.GetValueOrDefault(0) > 0)
                                {
                                    temp_LastUpdatedBy = cartitem.LastUpdatedBy.GetValueOrDefault(0);
                                }
                            }

                            int Index = Array.FindIndex(AllGuid, row => row.Contains(cartitem.GUID.ToString()));

                            string UDF1 = string.Empty;
                            string UDF2 = string.Empty;
                            string UDF3 = string.Empty;
                            string UDF4 = string.Empty;
                            string UDF5 = string.Empty;
                            if (udf1 != null && udf1.Length > 0)
                            {
                                UDF1 = udf1[Index];
                            }
                            if (udf2 != null && udf2.Length > 0)
                            {
                                UDF2 = udf2[Index];
                            }
                            if (udf3 != null && udf3.Length > 0)
                            {
                                UDF3 = udf3[Index];
                            }
                            if (udf4 != null && udf4.Length > 0)
                            {
                                UDF4 = udf4[Index];
                            }
                            if (udf5 != null && udf5.Length > 0)
                            {
                                UDF5 = udf5[Index];
                            }

                            QuoteDetailDTO objQuoteDetailDTO = new QuoteDetailDTO();

                            objQuoteDetailDTO.QuoteGUID = objReturnAfterSave.GUID;

                            objQuoteDetailDTO.ItemGUID = cartitem.ItemGUID.GetValueOrDefault(Guid.Empty);
                            objQuoteDetailDTO.BinID = cartitem.BinId;
                            double? quantityToset = cartitem.Quantity;
                            if (cartItemQuantity != null && cartItemQuantity.Length > 0)
                            {
                                try
                                {
                                    quantityToset = Convert.ToDouble(cartItemQuantity[Index]);
                                }
                                catch (Exception) { }
                            }
                            objQuoteDetailDTO.RequestedQuantity = quantityToset;
                            if (SubmissionMethod == 2 || ActualQuoteStatus > 2)
                            {
                                objQuoteDetailDTO.ApprovedQuantity = quantityToset;
                            }
                            objQuoteDetailDTO.RequiredDate = cartitem.LeadTimeInDays.GetValueOrDefault(0) > 0 ? CurrentDateTime.AddDays(cartitem.LeadTimeInDays.GetValueOrDefault(0)) : objReturnAfterSave.RequiredDate;
                            objQuoteDetailDTO.Created = DateTimeUtility.DateTimeNow;
                            objQuoteDetailDTO.CreatedBy = temp_CreatedBy;
                            objQuoteDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objQuoteDetailDTO.LastUpdatedBy = temp_LastUpdatedBy;
                            objQuoteDetailDTO.Room = RoomId;
                            objQuoteDetailDTO.CompanyID = CompanyId;
                            objQuoteDetailDTO.AddedFrom = "Web";
                            objQuoteDetailDTO.EditedFrom = "Web";
                            objQuoteDetailDTO.UDF1 = UDF1;
                            objQuoteDetailDTO.UDF2 = UDF2;
                            objQuoteDetailDTO.UDF3 = UDF3;
                            objQuoteDetailDTO.UDF4 = UDF4;
                            objQuoteDetailDTO.UDF5 = UDF5;
                            objQuoteDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objQuoteDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), RoomId, CompanyId);

                            if (objItemMasterDTO != null)
                            {
                                CostUOMMasterDTO costUOM = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                                if (costUOM == null)
                                    costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                                if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                                {
                                    costUOM.CostUOMValue = 1;
                                }

                                #region WI-6215 and Other Relevant order cost related jira
                                objQuoteDetailDTO.ItemSellPrice = objItemMasterDTO.SellPrice.GetValueOrDefault(0);
                                objQuoteDetailDTO.ItemCostUOMValue = costUOM.CostUOMValue.GetValueOrDefault(0);
                                objQuoteDetailDTO.ItemMarkup = objItemMasterDTO.Markup.GetValueOrDefault(0);
                                #endregion

                                objQuoteDetailDTO.QuoteLineItemExtendedCost = double.Parse(Convert.ToString((objQuoteMasterDTO.QuoteStatus <= 2 ? (objQuoteDetailDTO.RequestedQuantity * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                                                             : (objQuoteDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));

                                objQuoteDetailDTO.QuoteLineItemExtendedPrice = double.Parse(Convert.ToString((objQuoteMasterDTO.QuoteStatus <= 2 ? (objQuoteDetailDTO.RequestedQuantity * (objItemMasterDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                                                             : (objQuoteDetailDTO.ApprovedQuantity.GetValueOrDefault(0) * (objItemMasterDTO.SellPrice.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1))))));

                                OrderUOMMasterDTO OrderUOM = new OrderUOMMasterDAL(base.DataBaseName).GetRecord(objItemMasterDTO.OrderUOMID.GetValueOrDefault(0), objItemMasterDTO.Room.GetValueOrDefault(0), objItemMasterDTO.CompanyID.GetValueOrDefault(0), false, false);
                                if (OrderUOM == null)
                                    OrderUOM = new OrderUOMMasterDTO() { OrderUOMValue = 1 };

                                if (OrderUOM.OrderUOMValue == null || OrderUOM.OrderUOMValue <= 0)
                                {
                                    OrderUOM.OrderUOMValue = 1;
                                }

                                if (objQuoteDetailDTO.RequestedQuantity != null && objQuoteDetailDTO.RequestedQuantity.GetValueOrDefault(0) > 0 && objItemMasterDTO.IsAllowOrderCostuom)
                                {
                                    objQuoteDetailDTO.RequestedQuantityUOM = objQuoteDetailDTO.RequestedQuantity / OrderUOM.OrderUOMValue;
                                }
                                else
                                {
                                    objQuoteDetailDTO.RequestedQuantityUOM = objQuoteDetailDTO.RequestedQuantity;
                                }

                                if (objQuoteDetailDTO.ApprovedQuantity != null && objQuoteDetailDTO.ApprovedQuantity.GetValueOrDefault(0) > 0 && objItemMasterDTO.IsAllowOrderCostuom)
                                {
                                    objQuoteDetailDTO.ApprovedQuantityUOM = objQuoteDetailDTO.ApprovedQuantity / OrderUOM.OrderUOMValue;
                                }
                                else
                                {
                                    objQuoteDetailDTO.ApprovedQuantityUOM = objQuoteDetailDTO.ApprovedQuantity;
                                }

                                objQuoteDetailDTO.ItemCost = objItemMasterDTO.Cost.GetValueOrDefault(0);
                                objQuoteDetailDTO.ItemCostUOM = objItemMasterDTO.CostUOMID.GetValueOrDefault(0);
                                objQuoteDetailDTO.SupplierID = objItemMasterDTO.SupplierID;
                                objQuoteDetailDTO.SupplierPartNo = objItemMasterDTO.SupplierPartNo;
                            }

                            QuoteDetailDTO returnQuoteDetailDTO = new QuoteDetailDTO();
                            returnQuoteDetailDTO = objQuoteDetailDAL.Insert(objQuoteDetailDTO, UserId, EnterpriseId);

                            // MaintainTransaction History for Cart To Quote
                            if (returnQuoteDetailDTO != null)
                            {
                                objCartItemDAL.InsertCartQuoteTransitionDetail(cartitem.GUID, returnQuoteDetailDTO.GUID, null, null, (int)TransactionConversionType.CarttoQuote, UserId);
                            }

                            itemGuidsToUpdate.Add(cartitem.ItemGUID ?? Guid.Empty);
                        }
                        if (!string.IsNullOrEmpty(objQuoteMasterDTO.QuoteLineItemsIds))
                        {
                            objCartItemDAL.DeleteRecords(objQuoteMasterDTO.QuoteLineItemsIds, UserId, CompanyId, EnterpriseId, SessionUserId);
                        }
                    }

                    List<SupplierMasterDTO> lstSupplier = objSupplierDAl.GetNonDeletedSupplierByIDsNormal(objQuoteMasterDTO.QuoteSupplierIdsCSV, RoomId, CompanyId);
                    bool anyIsSendtoVendor = false;
                    if (lstSupplier.Any(x => x.IsSendtoVendor == true))
                    {
                        anyIsSendtoVendor = true;
                    }

                    //--------------------------------------------------------------------
                    //
                    objQuoteMasterDTO.QuoteStatus = ActualQuoteStatus;
                    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                    string columnList = "ID,RoomName,DoSendQuotetoVendor";
                    RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");

                    if (SubmissionMethod == 2)
                    {
                        if (!objSupplier.IsSendtoVendor)
                        {
                            objQuoteMasterDTO.QuoteStatus = (int)OrderStatus.Transmitted;
                        }
                        else
                        {
                            objQuoteMasterDTO.QuoteStatus = (int)OrderStatus.Approved;
                        }

                    }
                    else
                    {
                        if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Approved && !anyIsSendtoVendor)
                        {
                            if (objRoom != null && !objRoom.DoSendQuotetoVendor)
                            {
                                objQuoteMasterDTO.QuoteStatus = (int)QuoteStatus.Transmitted;
                            }
                        }
                        if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Transmitted && (anyIsSendtoVendor || objRoom.DoSendQuotetoVendor))
                        {
                            objQuoteMasterDTO.QuoteStatus = (int)QuoteStatus.Approved;
                        }
                    }

                    if (!unSuccessfulOrders.Contains(objReturnAfterSave.GUID))
                    {
                        objQuoteMasterDAL.UpdateQuoteStatus(objReturnAfterSave.GUID, objQuoteMasterDTO.QuoteStatus, RoomId, CompanyId);

                        QuoteMasterDTO objQMDto = objQuoteMasterDAL.GetQuoteByGuidPlain(objReturnAfterSave.GUID);
                        if (objQMDto != null)
                        {
                            if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                            {
                                objReturnAfterSave.RequesterID = UserId;
                                objQMDto.RequesterID = UserId;
                            }
                            else if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Approved
                                    || objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Transmitted)
                            {
                                objReturnAfterSave.RequesterID = UserId;
                                objReturnAfterSave.ApproverID = UserId;
                                objQMDto.RequesterID = UserId;
                                objQMDto.ApproverID = UserId;
                            }
                            objQuoteMasterDAL.UpdateQuoteMaster(objQMDto);
                            //if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Transmitted)
                            //{
                            //    objQuoteMasterDTO.EditedFrom = "Web";
                            //    objQuoteMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            //    QuoteMasterDTO objDTO = new QuoteMasterDTO() { GUID = objQuoteMasterDTO.GUID, Room = RoomId, CompanyID = CompanyId, EditedFrom = "Web", WhatWhereAction = "CartToQuote.ToTransmitFromApproved" };
                            //    objQuoteMasterDAL.TransmitQuote(objDTO);
                            //}
                        }

                        objReturnAfterSave.QuoteStatus = objQuoteMasterDTO.QuoteStatus;
                        lstSuccessQuotes.Add(objReturnAfterSave);
                    }

                    if (itemGuidsToUpdate != null && itemGuidsToUpdate.Any() && itemGuidsToUpdate.Count > 0)
                    {
                        foreach (var guid in itemGuidsToUpdate)
                        {
                            ItemQuoteInfoDTO objItemQuoteInfoDTO = objItemMasterDAL.GetItemQuotedQuantity(guid, RoomId, CompanyId);
                            if (objItemQuoteInfoDTO != null)
                            {
                                objItemMasterDAL.EditDateAndOnQuotedQuantity(guid, RoomId, CompanyId, objItemQuoteInfoDTO.OnQuotedQuantity);
                            }
                        }
                    }
                }
            }
            return lstSuccessQuotes;
        }

        #endregion

        #region Crate Order from Quote

        public IList<OrderMasterDTO> GetOrdersBySelectedQuote(Guid QuoteGuid, string Ids, long RoomId, long CompanyId, long UserId, string DatabaseName, string QuoteLineItemUDF1, string QuoteLineItemUDF2, string QuoteLineItemUDF3, string QuoteLineItemUDF4, string QuoteLineItemUDF5, List<long> supplierIDs, long EnterpriseID)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
            CartItemDAL objCartItemDAL = new CartItemDAL(DatabaseName);
            List<Guid> arrcartguids = new List<Guid>();
            List<QuoteDetailDTO> lstQuoteItems = new List<QuoteDetailDTO>();
            bool WithSelection;
            IList<OrderMasterDTO> lstOrders = new List<OrderMasterDTO>();
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(base.DataBaseName);
            SupplierMasterDTO objSupplierDTO = new SupplierMasterDTO();

            bool DoGroupSupplierQuoteToOrder = false;
            CommonDAL objCommonDAL = new CommonDAL(DatabaseName);
            string columnList = "ID,RoomName,DoGroupSupplierQuoteToOrder";
            RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomId.ToString() + "", "");
            if (objRoom != null && objRoom.ID > 0)
            {
                DoGroupSupplierQuoteToOrder = objRoom.DoGroupSupplierQuoteToOrder;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    WithSelection = true;
                }
                else
                {
                    WithSelection = false;
                }
                lstQuoteItems = objCartItemDAL.GetQuoteItemsByGuids(QuoteGuid, arrcartguids, RoomId, CompanyId, WithSelection);
                List<QuoteDetailDTO> lstquoteitemsgrped = new List<QuoteDetailDTO>();
                if (DoGroupSupplierQuoteToOrder)
                {
                    lstquoteitemsgrped = (from ci in lstQuoteItems
                                          group ci by new { ci.SupplierID, ci.SupplierName, ci.MaxOrderSize } into groupedci
                                          select new QuoteDetailDTO
                                          {
                                              SupplierID = groupedci.Key.SupplierID,
                                              MaxOrderSize = groupedci.Key.MaxOrderSize,
                                              SupplierName = groupedci.Key.SupplierName
                                          }).ToList();
                }
                else
                {
                    lstquoteitemsgrped = (from ci in lstQuoteItems
                                          group ci by new { ci.SupplierID, ci.SupplierName, ci.MaxOrderSize } into groupedci
                                          select new QuoteDetailDTO
                                          {
                                              SupplierID = groupedci.Key.SupplierID,
                                              MaxOrderSize = groupedci.Key.MaxOrderSize,
                                              SupplierName = groupedci.Key.SupplierName
                                          }).ToList();
                }
                string QuoteLineItemIds = string.Empty;
                string QuoteLineItemUDF1New = string.Empty;
                string QuoteLineItemUDF2New = string.Empty;
                string QuoteLineItemUDF3New = string.Empty;
                string QuoteLineItemUDF4New = string.Empty;
                string QuoteLineItemUDF5New = string.Empty;
                List<QuoteDetailDTO> lstGroupedQuoteItems = new List<QuoteDetailDTO>();
                int LineItemCount = 0;
                AutoOrderNumberGenerate objAutoNumber = null;

                string strSupplierIds = string.Empty;
                if (supplierIDs != null && supplierIDs.Any())
                {
                    strSupplierIds = string.Join(",", supplierIDs);
                }
                List<SupplierMasterDTO> lstSuppliers = new List<SupplierMasterDTO>();
                lstSuppliers = objSupDAL.GetNonDeletedSupplierByIDsNormal(strSupplierIds, RoomId, CompanyId);

                foreach (var item in lstquoteitemsgrped)
                {
                    if (DoGroupSupplierQuoteToOrder)
                    {
                        lstGroupedQuoteItems = lstQuoteItems.Where(t => t.SupplierID == item.SupplierID).ToList();
                        LineItemCount = lstGroupedQuoteItems.Count;
                    }
                    else
                    {
                        lstGroupedQuoteItems = lstQuoteItems.Where(t => t.SupplierID == item.SupplierID).ToList();
                        LineItemCount = lstGroupedQuoteItems.Count;
                    }

                    if (supplierIDs != null
                        && supplierIDs.Count > 0
                        && !supplierIDs.Contains(item.SupplierID.GetValueOrDefault(0)))
                    {
                        if (lstSuppliers != null && lstSuppliers.Count > 0)
                        {
                            item.SupplierID = lstSuppliers[0].ID;
                        }
                    }

                    objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierID.GetValueOrDefault(0));

                    if (objSupplierDTO != null)
                    {
                        item.MaxOrderSize = objSupplierDTO.MaximumOrderSize.GetValueOrDefault(0);
                        item.SupplierName = objSupplierDTO.SupplierName;
                    }

                    QuoteLineItemUDF1New = string.Empty;
                    QuoteLineItemUDF2New = string.Empty;
                    QuoteLineItemUDF3New = string.Empty;
                    QuoteLineItemUDF4New = string.Empty;
                    QuoteLineItemUDF5New = string.Empty;

                    if (item.MaxOrderSize != null && item.MaxOrderSize > 0 && LineItemCount > item.MaxOrderSize)
                    {
                        decimal divfactor = (LineItemCount / (item.MaxOrderSize ?? 1));
                        decimal modfactor = (LineItemCount % (item.MaxOrderSize ?? 1));
                        int divfactorint = (int)divfactor;
                        if (modfactor > 0)
                        {
                            divfactorint = divfactorint + 1;
                        }
                        bool isSameSuppier = false;
                        for (int i = 1; i <= divfactorint; i++)
                        {
                            string orderNumber = string.Empty;
                            string orderNumberSorting = string.Empty;
                            objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextOrderNumber(RoomId, CompanyId, item.SupplierID.GetValueOrDefault(0), EnterpriseID, objAutoNumber, isSameSuppier);
                            orderNumber = objAutoNumber.OrderNumber;

                            orderNumberSorting = objAutoNumber.OrderNumberForSorting;
                            isSameSuppier = true;

                            //if (orderNumber == null || string.IsNullOrWhiteSpace(orderNumber))
                            //{
                            //    orderNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
                            //}
                            int ReleaseNo = 1;
                            if (!string.IsNullOrWhiteSpace(orderNumber))
                            {
                                OrderMasterDAL objOrderDAL = new OrderMasterDAL(DatabaseName);
                                ReleaseNo = objOrderDAL.GetNextReleaseNumber(orderNumber, null, RoomId, CompanyId);
                            }

                            if (string.IsNullOrEmpty(orderNumberSorting))
                                orderNumberSorting = orderNumber;

                            int DReqOrderDays = 0;

                            if (objSupplierDTO != null)
                            {
                                DReqOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);
                                if (objSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 1)
                                {
                                    if (!string.IsNullOrWhiteSpace(objSupplierDTO.POAutoNrReleaseNumber) && objSupplierDTO.NextOrderNo.Trim() == orderNumber.Trim())
                                    {
                                        ReleaseNo = Convert.ToInt32(objSupplierDTO.POAutoNrReleaseNumber) + 1;
                                    }
                                }
                            }

                            OrderMasterDTO objDTO = new OrderMasterDTO()
                            {
                                OrderType = (int)OrderType.Order,
                                RequiredDate = CurrentDateTime.AddDays(DReqOrderDays),
                                OrderNumber = orderNumber,
                                OrderStatus = (int)OrderStatus.UnSubmitted,
                                ReleaseNumber = ReleaseNo.ToString(),
                                LastUpdated = DateTime.UtcNow,
                                Created = DateTime.UtcNow,
                                Supplier = item.SupplierID.GetValueOrDefault(0),
                                SupplierName = item.SupplierName,
                                CreatedBy = UserId,
                                LastUpdatedBy = UserId,
                                CompanyID = CompanyId,
                                Room = RoomId,
                                OrderDate = CurrentDateTime,
                                AutoOrderNumber = objAutoNumber,
                                IsBlanketOrder = objAutoNumber.IsBlanketPO,
                                OrderLineItemsIds = QuoteLineItemIds,
                                OrderNumber_ForSorting = orderNumberSorting,
                                RequiredDateString = string.Empty,
                                OrderLineItemUDF1 = QuoteLineItemUDF1,
                                OrderLineItemUDF2 = QuoteLineItemUDF2,
                                OrderLineItemUDF3 = QuoteLineItemUDF3,
                                OrderLineItemUDF4 = QuoteLineItemUDF4,
                                OrderLineItemUDF5 = QuoteLineItemUDF5,
                            };
                            if (i == divfactorint)
                            {
                                objDTO.NoOfLineItems = LineItemCount - ((i - 1) * (int)item.MaxOrderSize);
                                QuoteLineItemIds = string.Join(",", lstGroupedQuoteItems.Skip((i - 1) * ((int)item.MaxOrderSize)).Take((objDTO.NoOfLineItems ?? 0)).Select(t => t.GUID));
                                objDTO.OrderLineItemsIds = QuoteLineItemIds;

                                if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                                {
                                    string[] AllGuid = Ids.Split(',').ToArray();
                                    string[] udf1 = (QuoteLineItemUDF1 != null ? QuoteLineItemUDF1.Split(',').ToArray() : null);
                                    string[] udf2 = (QuoteLineItemUDF2 != null ? QuoteLineItemUDF2.Split(',').ToArray() : null);
                                    string[] udf3 = (QuoteLineItemUDF3 != null ? QuoteLineItemUDF3.Split(',').ToArray() : null);
                                    string[] udf4 = (QuoteLineItemUDF4 != null ? QuoteLineItemUDF4.Split(',').ToArray() : null);
                                    string[] udf5 = (QuoteLineItemUDF5 != null ? QuoteLineItemUDF5.Split(',').ToArray() : null);
                                    foreach (string guid in QuoteLineItemIds.Split(','))
                                    {
                                        int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                        if (udf1 != null && udf1.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF1New))
                                            {
                                                QuoteLineItemUDF1New = udf1[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF1New = QuoteLineItemUDF1New + "," + udf1[Index];
                                            }
                                        }
                                        if (udf2 != null && udf2.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF2New))
                                            {
                                                QuoteLineItemUDF2New = udf2[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF2New = QuoteLineItemUDF2New + "," + udf2[Index];
                                            }
                                        }
                                        if (udf3 != null && udf3.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF3New))
                                            {
                                                QuoteLineItemUDF3New = udf3[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF3New = QuoteLineItemUDF3New + "," + udf3[Index];
                                            }
                                        }
                                        if (udf4 != null && udf4.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF4New))
                                            {
                                                QuoteLineItemUDF4New = udf4[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF4New = QuoteLineItemUDF4New + "," + udf4[Index];
                                            }
                                        }
                                        if (udf5 != null && udf5.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF5New))
                                            {
                                                QuoteLineItemUDF5New = udf5[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF5New = QuoteLineItemUDF5New + "," + udf5[Index];
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                objDTO.NoOfLineItems = (int)item.MaxOrderSize;
                                QuoteLineItemIds = string.Join(",", lstGroupedQuoteItems.Skip((i - 1) * ((int)item.MaxOrderSize)).Take(((int)item.MaxOrderSize)).Select(t => t.GUID));
                                objDTO.OrderLineItemsIds = QuoteLineItemIds;

                                if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                                {
                                    string[] AllGuid = Ids.Split(',').ToArray();
                                    string[] udf1 = (QuoteLineItemUDF1 != null ? QuoteLineItemUDF1.Split(',').ToArray() : null);
                                    string[] udf2 = (QuoteLineItemUDF2 != null ? QuoteLineItemUDF2.Split(',').ToArray() : null);
                                    string[] udf3 = (QuoteLineItemUDF3 != null ? QuoteLineItemUDF3.Split(',').ToArray() : null);
                                    string[] udf4 = (QuoteLineItemUDF4 != null ? QuoteLineItemUDF4.Split(',').ToArray() : null);
                                    string[] udf5 = (QuoteLineItemUDF5 != null ? QuoteLineItemUDF5.Split(',').ToArray() : null);

                                    foreach (string guid in QuoteLineItemIds.Split(','))
                                    {
                                        int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                        if (udf1 != null && udf1.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF1New))
                                            {
                                                QuoteLineItemUDF1New = udf1[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF1New = QuoteLineItemUDF1New + "," + udf1[Index];
                                            }
                                        }
                                        if (udf2 != null && udf2.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF2New))
                                            {
                                                QuoteLineItemUDF2New = udf2[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF2New = QuoteLineItemUDF2New + "," + udf2[Index];
                                            }
                                        }
                                        if (udf3 != null && udf3.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF3New))
                                            {
                                                QuoteLineItemUDF3New = udf3[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF3New = QuoteLineItemUDF3New + "," + udf3[Index];
                                            }
                                        }
                                        if (udf4 != null && udf4.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF4New))
                                            {
                                                QuoteLineItemUDF4New = udf4[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF4New = QuoteLineItemUDF4New + "," + udf4[Index];
                                            }
                                        }
                                        if (udf5 != null && udf5.Length > Index)
                                        {
                                            if (string.IsNullOrWhiteSpace(QuoteLineItemUDF5New))
                                            {
                                                QuoteLineItemUDF5New = udf5[Index];
                                            }
                                            else
                                            {
                                                QuoteLineItemUDF5New = QuoteLineItemUDF5New + "," + udf5[Index];
                                            }
                                        }
                                    }
                                }
                            }
                            if (objDTO.IsBlanketOrder)
                            {
                                IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                                                                                              where x != null
                                                                                              && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.Now).ToShortDateString()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                              && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.Now).ToShortDateString()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                              select x);
                                if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                                {
                                    objDTO.BlanketOrderNumberID = objSuppBlnkPOList.FirstOrDefault().ID;
                                }

                            }
                            objDTO.OrderLineItemUDF1 = QuoteLineItemUDF1New;
                            objDTO.OrderLineItemUDF2 = QuoteLineItemUDF2New;
                            objDTO.OrderLineItemUDF3 = QuoteLineItemUDF3New;
                            objDTO.OrderLineItemUDF4 = QuoteLineItemUDF4New;
                            objDTO.OrderLineItemUDF5 = QuoteLineItemUDF5New;
                            objDTO.IsOrderSelected = true;
                            lstOrders.Add(objDTO);
                        }
                    }
                    else
                    {
                        string orderNumber = string.Empty;
                        string orderNumberSorting = string.Empty;
                        objAutoNumber = new AutoSequenceDAL(DatabaseName).GetNextOrderNumber(RoomId, CompanyId, item.SupplierID.GetValueOrDefault(0), EnterpriseID, objAutoNumber);
                        orderNumber = objAutoNumber.OrderNumber;
                        orderNumberSorting = objAutoNumber.OrderNumberForSorting;

                        //if (orderNumber == null || string.IsNullOrWhiteSpace(orderNumber))
                        //{
                        //    orderNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
                        //}
                        if (string.IsNullOrEmpty(orderNumberSorting))
                            orderNumberSorting = orderNumber;

                        int ReleaseNo = 1;

                        if (!string.IsNullOrWhiteSpace(orderNumber))
                        {
                            OrderMasterDAL objOrderDAL = new OrderMasterDAL(DatabaseName);
                            ReleaseNo = objOrderDAL.GetNextReleaseNumber(orderNumber, null, RoomId, CompanyId);
                        }
                        QuoteLineItemIds = QuoteLineItemIds = string.Join(",", lstGroupedQuoteItems.Select(t => t.GUID));
                        if (Ids != null && (!string.IsNullOrWhiteSpace(Ids)))
                        {
                            string[] AllGuid = Ids.Split(',').ToArray();
                            string[] udf1 = (QuoteLineItemUDF1 != null ? QuoteLineItemUDF1.Split(',').ToArray() : null);
                            string[] udf2 = (QuoteLineItemUDF2 != null ? QuoteLineItemUDF2.Split(',').ToArray() : null);
                            string[] udf3 = (QuoteLineItemUDF3 != null ? QuoteLineItemUDF3.Split(',').ToArray() : null);
                            string[] udf4 = (QuoteLineItemUDF4 != null ? QuoteLineItemUDF4.Split(',').ToArray() : null);
                            string[] udf5 = (QuoteLineItemUDF5 != null ? QuoteLineItemUDF5.Split(',').ToArray() : null);
                            foreach (string guid in QuoteLineItemIds.Split(','))
                            {
                                int Index = Array.FindIndex(AllGuid, row => row.Contains(guid));
                                if (udf1 != null && udf1.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(QuoteLineItemUDF1New))
                                    {
                                        QuoteLineItemUDF1New = udf1[Index];
                                    }
                                    else
                                    {
                                        QuoteLineItemUDF1New = QuoteLineItemUDF1New + "," + udf1[Index];
                                    }
                                }
                                if (udf2 != null && udf2.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(QuoteLineItemUDF2New))
                                    {
                                        QuoteLineItemUDF2New = udf2[Index];
                                    }
                                    else
                                    {
                                        QuoteLineItemUDF2New = QuoteLineItemUDF2New + "," + udf2[Index];
                                    }
                                }
                                if (udf3 != null && udf3.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(QuoteLineItemUDF3New))
                                    {
                                        QuoteLineItemUDF3New = udf3[Index];
                                    }
                                    else
                                    {
                                        QuoteLineItemUDF3New = QuoteLineItemUDF3New + "," + udf3[Index];
                                    }
                                }
                                if (udf4 != null && udf4.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(QuoteLineItemUDF4New))
                                    {
                                        QuoteLineItemUDF4New = udf4[Index];
                                    }
                                    else
                                    {
                                        QuoteLineItemUDF4New = QuoteLineItemUDF4New + "," + udf4[Index];
                                    }
                                }
                                if (udf5 != null && udf5.Length > Index)
                                {
                                    if (string.IsNullOrWhiteSpace(QuoteLineItemUDF5New))
                                    {
                                        QuoteLineItemUDF5New = udf5[Index];
                                    }
                                    else
                                    {
                                        QuoteLineItemUDF5New = QuoteLineItemUDF5New + "," + udf5[Index];
                                    }
                                }
                            }
                        }
                        int DefaultRequiredOrderDays = 0;
                        objSupplierDTO = objSupDAL.GetSupplierByIDPlain(item.SupplierID.GetValueOrDefault(0));
                        if (objSupplierDTO != null)
                        {
                            DefaultRequiredOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);

                            if (objSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 1)
                            {
                                if (!string.IsNullOrWhiteSpace(objSupplierDTO.POAutoNrReleaseNumber) && objSupplierDTO.NextOrderNo.Trim() == orderNumber.Trim())
                                {
                                    ReleaseNo = Convert.ToInt32(objSupplierDTO.POAutoNrReleaseNumber) + 1;
                                }
                            }
                        }

                        OrderMasterDTO objDTO = new OrderMasterDTO()
                        {
                            RequiredDate = CurrentDateTime.AddDays(DefaultRequiredOrderDays),
                            OrderNumber = orderNumber,
                            OrderStatus = (int)OrderStatus.UnSubmitted,
                            ReleaseNumber = ReleaseNo.ToString(),
                            LastUpdated = DateTime.UtcNow,
                            Created = DateTime.UtcNow,
                            Supplier = item.SupplierID.GetValueOrDefault(0),
                            SupplierName = item.SupplierName,
                            CreatedBy = UserId,
                            LastUpdatedBy = UserId,
                            CompanyID = CompanyId,
                            Room = RoomId,
                            OrderDate = CurrentDateTime,
                            AutoOrderNumber = objAutoNumber,
                            IsBlanketOrder = objAutoNumber.IsBlanketPO,
                            NoOfLineItems = LineItemCount,
                            OrderLineItemsIds = QuoteLineItemIds,
                            OrderNumber_ForSorting = orderNumberSorting,
                            RequiredDateString = string.Empty,
                            OrderLineItemUDF1 = QuoteLineItemUDF1New,
                            OrderLineItemUDF2 = QuoteLineItemUDF2New,
                            OrderLineItemUDF3 = QuoteLineItemUDF3New,
                            OrderLineItemUDF4 = QuoteLineItemUDF4New,
                            OrderLineItemUDF5 = QuoteLineItemUDF5New,
                        };

                        if (objDTO.IsBlanketOrder)
                        {

                            IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                                                                                          where x != null
                                                                                          && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.Now).ToShortDateString()) <= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                          && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.Now).ToShortDateString()) >= Convert.ToDateTime(DateTime.Now.ToShortDateString())
                                                                                          select x);
                            if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                            {
                                objDTO.BlanketOrderNumberID = objSuppBlnkPOList.FirstOrDefault().ID;
                            }

                        }
                        objDTO.IsOrderSelected = true;
                        lstOrders.Add(objDTO);
                    }
                }

                return lstOrders;
            }
            catch
            {
                throw;
            }
        }

        public List<QuoteDetailDTO> GetQuoteItemsByGuids(Guid QuoteGuid, List<Guid> arrids, long RoomId, long CompanyId, bool WithSelection)
        {
            try
            {
                string DbConnectionString = "";
                List<QuoteDetailDTO> lstQuoteDetails = null;
                eTurnsEntities context = null;

                if (!string.IsNullOrWhiteSpace(DbConnectionString))
                {
                    context = new eTurnsEntities(DbConnectionString);
                }
                else
                {
                    context = new eTurnsEntities(base.DataBaseEntityConnectionString);
                }
                string CSVGUIDS = string.Empty;

                if (arrids == null || arrids.Count < 1)
                {

                }
                else
                {
                    CSVGUIDS = string.Join(",", arrids);
                }

                var params1 = new SqlParameter[] {
                    new SqlParameter("@QuoteGuid", QuoteGuid),
                    new SqlParameter("@arrids", CSVGUIDS ?? (object)DBNull.Value),
                    new SqlParameter("@RoomId", RoomId),
                    new SqlParameter("@CompanyId", CompanyId)
                };

                using (context)
                {
                    lstQuoteDetails = (from QD in context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteItemsByGuids] @QuoteGuid,@arrids,@RoomId,@CompanyId", params1)
                                       select new QuoteDetailDTO
                                       {
                                           ID = QD.ID,
                                           ItemNumber = QD.ItemNumber,
                                           ItemGUID = QD.ItemGUID,
                                           ApprovedQuantity = QD.ApprovedQuantity,
                                           UDF1 = QD.UDF1,
                                           UDF2 = QD.UDF2,
                                           UDF3 = QD.UDF3,
                                           UDF4 = QD.UDF4,
                                           UDF5 = QD.UDF5,
                                           GUID = QD.GUID,
                                           Created = QD.Created,
                                           LastUpdated = QD.LastUpdated,
                                           CreatedBy = QD.CreatedBy,
                                           LastUpdatedBy = QD.LastUpdatedBy,
                                           IsDeleted = QD.IsDeleted,
                                           CompanyID = QD.CompanyID,
                                           Room = QD.Room,
                                           CreatedByName = QD.CreatedByName,
                                           UpdatedByName = QD.UpdatedByName,
                                           ItemRoomName = QD.ItemRoomName,
                                           SupplierID = QD.SupplierID,
                                           SupplierName = QD.SupplierName,
                                           BlanketPOID = QD.BlanketPOID,
                                           BlanketPONumber = QD.BlanketPONumber,
                                           BinID = QD.BinID,
                                           BinName = QD.BinName,
                                           MaxOrderSize = QD.MaxOrderSize,
                                           LeadTimeInDays = QD.LeadTimeInDays,
                                           CategoryID = QD.CategoryID,
                                           Category = QD.Category,
                                           SerialNumberTracking = QD.SerialNumberTracking,
                                           LotNumberTracking = QD.LotNumberTracking,
                                           DateCodeTracking = QD.DateCodeTracking,
                                           ItemType = QD.ItemType,
                                           IsOrdered = QD.IsOrdered,
                                           RequestedQuantity = QD.RequestedQuantity,
                                           POItemLineNumber = QD.POItemLineNumber,
                                       }).ToList();

                    return lstQuoteDetails;
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Maintain TransactionHistory, Cart To Order,ReturnOrder,Transfer,Quote and Quote ToOrder
        public void InsertCartQuoteTransitionDetail(Guid? CartGUID,
                            Guid? QuoteDetailGUID,
                            Guid? OrderDetailGUID,
                            Guid? TransferDetailGUID,
                            int? ConversionType,
                            long? CreatedBy)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@CartGUID", CartGUID ?? (object)DBNull.Value),
                    new SqlParameter("@QuoteDetailGUID", QuoteDetailGUID ?? (object)DBNull.Value),
                    new SqlParameter("@OrderDetailGUID", OrderDetailGUID ?? (object)DBNull.Value),
                    new SqlParameter("@TransferDetailGUID", TransferDetailGUID ?? (object)DBNull.Value),
                    new SqlParameter("@ConversionType", ConversionType ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", CreatedBy ?? (object)DBNull.Value)
                };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [InsertCartQuoteTransitionDetail] @CartGUID,@QuoteDetailGUID,@OrderDetailGUID,@TransferDetailGUID,@ConversionType,@CreatedBy", params1);
            }
        }
        #endregion

        #region Cart save From Window Service

        public List<CartItemDTO> CartSaveForSolumn(Guid ItemGUID, long UserId, string CalledFrom, string calledFromFunctionName, long SessionUserId)
        {
            List<CartItemDTO> lstCarts = new List<CartItemDTO>();
            string WhatWhereAction = calledFromFunctionName;
            if (SessionUserId > 0 && UserId != SessionUserId)
            {
                UserId = SessionUserId;
            }
            var params1 = new SqlParameter[] { new SqlParameter("@itemguid", ItemGUID), new SqlParameter("@userid", UserId), new SqlParameter("@calledfrom", CalledFrom ?? (object)DBNull.Value), new SqlParameter("@calledfromFunctionName", WhatWhereAction ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstCarts = context.Database.SqlQuery<CartItemDTO>("exec [CreateManualCart] @itemguid,@userid,@calledfrom,@calledfromFunctionName", params1).ToList();
            }
            return lstCarts;
        }
        #endregion

        public double GetSuggestedQtyByReplenishType(Guid ItemGuid, long RoomID, long CompanyID, string ReplenishType, long BinID = 0)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@ReplenishType", ReplenishType),
                                                new SqlParameter("@BinID", BinID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<double>("exec GetSuggestedQtyByReplenishType @ItemGuid,@RoomId,@CompanyId,@ReplenishType,@BinID", paramInnerCase).FirstOrDefault();
            }
        }

        public AutoQuoteNumberGenerate getQuoteNumber(long EnterpriseID, long CompanyId, long RoomId, long SupplierID)
        {
            var objAutoNumber = new AutoSequenceDAL(base.DataBaseName).GetNextQuoteNumber(RoomId, CompanyId, EnterpriseID, SupplierID);
            return objAutoNumber;
        }

        public Int32 AddAutosotForImport(Guid ItemGuid, long UserID, string EnterpriseDBName,bool ISQtyToMeedDemandUpdateRequired, long SessionUserID)
        {
            try
            {
                var objparams = new SqlParameter[] {
                                                new SqlParameter("@NewItemGUID", ItemGuid),
                                                new SqlParameter("@UserID", UserID),
                                                new SqlParameter("@EnterpriseDBName", EnterpriseDBName),
                                                new SqlParameter("@ISQtyToMeedDemandUpdateRequired", ISQtyToMeedDemandUpdateRequired),
                                                new SqlParameter("@SessionUserID", SessionUserID),
                                            };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string qry = "exec [AddAutosotForImport] @NewItemGUID,@UserID,@EnterpriseDBName,@ISQtyToMeedDemandUpdateRequired,@SessionUserID";
                    return context.Database.SqlQuery<Int32>(qry, objparams).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
           
        }
    }
}


