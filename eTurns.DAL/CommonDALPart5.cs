using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace eTurns.DAL
{
    public partial class CommonDAL : eTurnsBaseDAL
    {
        private Dictionary<string, int> GetNarrowDDData_CompanyMaster(string TextFieldName, bool IsArchived, bool IsDeleted, List<CompanyMasterDTO> CompanyList, Int64 RoleID, List<CompanyMasterDTO> AllCompanies)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(base.DataBaseName);
            #region GET DATA
            List<CompanyMasterDTO> lstCompanies = new List<CompanyMasterDTO>();
            if (AllCompanies != null && AllCompanies.Count > 0)
            {
                lstCompanies = AllCompanies;
            }
            else
            {
                lstCompanies = objCompanyDAL.GetAllCompaniesFromETurnsMaster(IsArchived, IsDeleted, CompanyList, RoleID).ToList();
            }
            if (TextFieldName == "LastUpdatedBy")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var lstsupps = (from ci in lstCompanies
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

                    var lstsupps = (from ci in lstCompanies
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
                var lstsupps = (from ci in lstCompanies
                                where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted
                                orderby ci.EnterPriseName
                                group ci by new { ci.EnterPriseId, ci.EnterPriseName } into grpms
                                select new
                                {
                                    count = grpms.Count(),
                                    sid = grpms.Key.EnterPriseId,
                                    supname = grpms.Key.EnterPriseName
                                });
                return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
            }
            return ColUDFData;
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_InventoryCountList(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            #region GET DATA
            if (TextFieldName == "CountType")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var lstsupps = (from ci in context.InventoryCounts
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
                                    orderby ci.CountType
                                    group ci by new { ci.CountType } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.CountType,
                                        supname = (grpms.Key.CountType == "M" ? "Manual" : (grpms.Key.CountType == "A" ? "Adjustment" : "Cycle"))
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            if (TextFieldName == "CountStatus")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from ci in context.InventoryCounts
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
                                    orderby ci.CountStatus
                                    group ci by new { ci.CountStatus } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.CountStatus,
                                        supname = (grpms.Key.CountStatus == "O") ? "Open" : (grpms.Key.CountStatus == "C") ? "Closed" : "Applied"
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            if (TextFieldName == "LastUpdatedBy")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from ci in context.InventoryCounts
                                    join um in context.UserMasters on ci.LastUpdatedBy equals um.ID
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
                                    orderby um.UserName
                                    group ci by new { ci.LastUpdatedBy, um.UserName } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.LastUpdatedBy,
                                        supname = grpms.Key.UserName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "CreatedBy")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstsupps = (from ci in context.InventoryCounts
                                    join um in context.UserMasters on ci.CreatedBy equals um.ID
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.RoomId == RoomID && ci.CompanyId == CompanyID
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
            return ColUDFData;
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_RoleMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            RoleMasterDAL objRoleDAL = new RoleMasterDAL(base.DataBaseName);
            #region GET DATA
            if (TextFieldName == "CreatedBy")
            {
                //ColUDFData = (from tmp in objRoleDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.CreatedByName != null)
                ColUDFData = (from tmp in objRoleDAL.GetAllRecords(RoomID, CompanyID, true, null)
                              orderby tmp.CreatedByName
                              group tmp by new { tmp.CreatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.CreatedByName,
                                  Count = grp.Count()
                              }
              ).AsParallel().ToDictionary(e => e.CreatedByName, e => e.Count);

                //var tempData = objRoleDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.CreatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objRoleDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.CreatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            else
            {
                //ColUDFData = (from tmp in objRoleDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UpdatedByName != null)
                ColUDFData = (from tmp in objRoleDAL.GetAllRecords(RoomID, CompanyID, null, true)
                              orderby tmp.UpdatedByName
                              group tmp by new { tmp.UpdatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.UpdatedByName,
                                  Count = grp.Count()
                              }
                   ).AsParallel().ToDictionary(e => e.UpdatedByName, e => e.Count);

                //var tempData = objRoleDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.UpdatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objRoleDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UpdatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_UserMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID)
        {
            return new Dictionary<string, int>();
            // Removed unused code
        }

        private Dictionary<string, int> GetNarrowDDData_QuickListMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            QuickListDAL objDAL = new QuickListDAL(base.DataBaseName);
            #region GET DATA
            if (TextFieldName == "CreatedBy")
            {
                ColUDFData = (from tmp in objDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.CreatedByName != null)
                              orderby tmp.CreatedByName
                              group tmp by new { tmp.CreatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.CreatedByName,
                                  Count = grp.Count()
                              }
              ).AsParallel().ToDictionary(e => e.CreatedByName, e => e.Count);

                //var tempData = objDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.CreatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.CreatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            else
            {
                ColUDFData = (from tmp in objDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UpdatedByName != null)
                              orderby tmp.UpdatedByName
                              group tmp by new { tmp.UpdatedByName }
                                  into grp
                              select new
                              {
                                  grp.Key.UpdatedByName,
                                  Count = grp.Count()
                              }
                    ).AsParallel().ToDictionary(e => e.UpdatedByName, e => e.Count);

                //var tempData = objDAL.GetAllRecords(RoomID, CompanyID).Select(t => t.UpdatedByName).Distinct();
                //foreach (var item in tempData)
                //{
                //    if (item != null)
                //    {
                //        int tempCount = objDAL.GetAllRecords(RoomID, CompanyID).Where(t => t.UpdatedByName == item.ToString()).Count();
                //        ColUDFData.Add(item.ToString(), tempCount);
                //    }
                //}
                return ColUDFData;
            }
            #endregion
        }


        private Dictionary<string, int> GetNarrowDDData_OrderMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            //OrderMasterDAL objOrderDAL = new OrderMasterDAL(base.DataBaseName);
            #region GET DATA
            if (TextFieldName == "CreatedBy")
            {
                //  ColUDFData = (from tmp in objOrderDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.CreatedByName != null)
                //                orderby tmp.CreatedByName
                //                group tmp by new { tmp.CreatedByName }
                //                    into grp
                //                    select new
                //                    {
                //                        grp.Key.CreatedByName,
                //                        Count = grp.Count()
                //                    }
                //).AsParallel().ToDictionary(e => e.CreatedByName, e => e.Count);

                return ColUDFData;
            }
            else if (TextFieldName == "SupplierName")
            {
                //OrderMasterDAL ObjItemDAL = new OrderMasterDAL(base.DataBaseName);
                //SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                //var ItemData = ObjItemDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.IsArchived == IsArchived && t.IsDeleted == IsDeleted); ;
                //var tempDataSupplier = (from x in ItemData
                //                        orderby x.SupplierName
                //                        where x.Supplier != null
                //                        select new { x.Supplier }).Distinct();
                //foreach (var item in tempDataSupplier)
                //{
                //    if (item != null)
                //    {
                //        var Data = objSupplierDAL.GetRecord((Int64)(item.Supplier), RoomID, CompanyID, IsArchived, IsDeleted);
                //        if (Data != null)
                //        {
                //            string SupplierName = Data.SupplierName;
                //            int tempCount = ObjItemDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(x => x.Supplier == item.Supplier && x.IsDeleted == IsDeleted && x.IsArchived == IsArchived).Count();
                //            if (!ColUDFData.ContainsKey(SupplierName + "[###]" + item.Supplier.ToString()))
                //                ColUDFData.Add(SupplierName + "[###]" + item.Supplier.ToString(), tempCount);
                //        }
                //    }
                //}
                return ColUDFData;
            }
            else if (TextFieldName == "OrderStatus")
            {
                //OrderMasterDAL ObjItemDAL = new OrderMasterDAL(base.DataBaseName);
                //var ItemData = ObjItemDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.IsArchived == IsArchived && t.IsDeleted == IsDeleted);
                //var tempDataOrdStatus = (from x in ItemData
                //                         select new { x.OrderStatus }).Distinct();

                //foreach (var item in tempDataOrdStatus)
                //{
                //    if (item != null)
                //    {
                //        string OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)item.OrderStatus).ToString()); ;
                //        int tempCount = ItemData.Where(x => x.OrderStatus == item.OrderStatus).Count();
                //        if (!ColUDFData.ContainsKey(OrderStatusText + "[###]" + item.OrderStatus.ToString()))
                //            ColUDFData.Add(OrderStatusText + "[###]" + item.OrderStatus.ToString(), tempCount);

                //    }
                //}
                //return ColUDFData.OrderBy(e => e.Key).ToDictionary(e => e.Key, e => e.Value);
                return ColUDFData;
            }
            else
            {
                //ColUDFData = (from tmp in objOrderDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.UpdatedByName != null)
                //              orderby tmp.UpdatedByName
                //              group tmp by new { tmp.UpdatedByName }
                //                  into grp
                //                  select new
                //                  {
                //                      grp.Key.UpdatedByName,
                //                      Count = grp.Count()
                //                  }
                //    ).AsParallel().ToDictionary(e => e.UpdatedByName, e => e.Count);
                return ColUDFData;
            }
            #endregion
        }

        private Dictionary<string, int> GetNarrowDDData_ReceiveMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, bool IsIncludeClosedOrder, List<long> SupplierIds)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            if (TextFieldName == "POReceiveaDate")
            {
                ColUDFData.Add("This Week[###]1", 0);
                ColUDFData.Add("Previous Week[###]2", 0);
                ColUDFData.Add("2-3 Week[###]3", 0);
                ColUDFData.Add("> 3 Week[###]4", 0);
                return ColUDFData;
            }
            #region GET DATA BY SP
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string orderStatusText = "4,5,6,7";
                if (IsIncludeClosedOrder)
                    orderStatusText = "4,5,6,7,8";

                var supplierIdsStr = string.Empty;
                if (SupplierIds != null && SupplierIds.Any())
                {
                    supplierIdsStr = string.Join(",", SupplierIds);
                }

                string strQry = "EXEC RCV_GetNarrowSearch @NarrowFieldName,@RoomID,@CompanyID,@OrderType,@OrderStatus,@IsDeleted,@IsArchived,@IsIncludeClosedOrder,@SupplierIDs ";
                var params1 = new SqlParameter[] {  new SqlParameter("@NarrowFieldName", TextFieldName),
                                                                new SqlParameter("@RoomID", RoomID),
                                                                new SqlParameter("@CompanyID", CompanyID),
                                                                new SqlParameter("@OrderType", 1),
                                                                new SqlParameter("@OrderStatus", orderStatusText),
                                                                new SqlParameter("@IsDeleted", false),
                                                                new SqlParameter("@IsArchived", false),
                                                                new SqlParameter("@IsIncludeClosedOrder", IsIncludeClosedOrder),
                                                                new SqlParameter("@SupplierIDs", supplierIdsStr)
                            };

                IEnumerable<CommonDTO> obj = context.Database.SqlQuery<CommonDTO>(strQry, params1).ToList();
                if (TextFieldName == "OrderNumber")
                {
                    ColUDFData = obj.ToDictionary(e => e.Text, e => e.Count);
                }
                else if (TextFieldName == "UDF1"
                        || TextFieldName == "UDF2"
                        || TextFieldName == "UDF3"
                        || TextFieldName == "UDF4"
                        || TextFieldName == "UDF5"
                        || TextFieldName == "UDF6"
                        || TextFieldName == "UDF7"
                        || TextFieldName == "UDF8"
                        || TextFieldName == "UDF9"
                        || TextFieldName == "UDF10"
                    )
                {
                    ColUDFData = obj.ToDictionary(e => e.Text + "[###]" + e.Value.ToString(), e => e.Count);
                }
                else
                {
                    ColUDFData = obj.ToDictionary(e => e.Text + "[###]" + e.ID.ToString(), e => e.Count);
                }
                return ColUDFData;
            }
            #endregion

        }

        private Dictionary<string, int> GetNarrowDDData_ReceiveToolAssetMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, bool IsIncludeClosedOrder)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            #region GET DATA BY SP
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<int> lstOrderStatus = new List<int>();
                lstOrderStatus.Add(4);
                lstOrderStatus.Add(5);
                lstOrderStatus.Add(6);
                lstOrderStatus.Add(7);
                if (IsIncludeClosedOrder)
                    lstOrderStatus.Add(8);
                int[] ordstatus = lstOrderStatus.ToArray();

                if (TextFieldName == "POReceiveaDate")
                {
                    ColUDFData.Add("This Week[###]1", 0);
                    ColUDFData.Add("Previous Week[###]2", 0);
                    ColUDFData.Add("2-3 Week[###]3", 0);
                    ColUDFData.Add("> 3 Week[###]4", 0);
                }
                else
                {
                    string orderStatusText = string.Join(",", ordstatus);
                    string strQry = "EXEC TARCV_GetNarrowSearch @NarrowFieldName,@RoomID,@CompanyID,@OrderType,@OrderStatus,@IsDeleted,@IsArchived,@IsIncludeClosedOrder ";
                    var params1 = new SqlParameter[] {  new SqlParameter("@NarrowFieldName", TextFieldName),
                                                                new SqlParameter("@RoomID", RoomID),
                                                                new SqlParameter("@CompanyID", CompanyID),
                                                                new SqlParameter("@OrderType", 1),
                                                                new SqlParameter("@OrderStatus", orderStatusText),
                                                                new SqlParameter("@IsDeleted", false),
                                                                new SqlParameter("@IsArchived", false),
                                                                new SqlParameter("@IsIncludeClosedOrder", IsIncludeClosedOrder)
                            };

                    IEnumerable<CommonDTO> obj = context.Database.SqlQuery<CommonDTO>(strQry, params1).ToList();
                    if (TextFieldName == "OrderNumber")
                    {
                        ColUDFData = obj.ToDictionary(e => e.Text, e => e.Count);
                    }
                    else
                    {
                        ColUDFData = obj.ToDictionary(e => e.Text + "[###]" + e.ID.ToString(), e => e.Count);
                    }
                }
                return ColUDFData;
            }
            #endregion

        }

        private Dictionary<string, int> GetNarrowDDData_ItemMaster(string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string ItemModelCallFrom, string RoomDateFormat, List<long> SupplierIds, int LoadDataCount, bool? Session_ConsignedAllowed, int? Session_QuicklistType, List<OrderDetailsDTO> lstDetailDTO = null, string QuickListType = "1", long ParentID = 0, bool IsAllowConsignedCredit = true, bool IsSLCount = false, MoveType? moveType = null, string requestFor = "")
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            IEnumerable<ItemMasterDTO> lstAllItems = new List<ItemMasterDTO>();

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            //int LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
            //int LoadDataCount = LoadDataCount < 0 ? (SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0);

            if (TextFieldName.ToLower() == "itemtype" && ItemModelCallFrom != requestFor)
            {
                if (!(!string.IsNullOrEmpty(ItemModelCallFrom) && !string.IsNullOrWhiteSpace(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "movemtr"))
                {
                    ItemModelCallFrom = requestFor;
                }
            }
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
            if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "newpull" || ((ItemModelCallFrom.Trim().ToLower() == "credit" || ItemModelCallFrom.Trim().ToLower() == "creditms") && TextFieldName == "ItemType" && objRoomDTO.IsIgnoreCreditRule == true))
            {
                PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                if (objRoomDTO != null && objRoomDTO.AllowPullBeyondAvailableQty == true)
                {
                    return objPullMasterDAL.GetNegativePullNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, requestFor).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                }
                else
                {
                    return objPullMasterDAL.GetNewPullNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, requestFor).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                }
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "credit")
            {
                PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                return objPullMasterDAL.GetNewCreditNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, "Pull", LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "creditms")
            {
                PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                return objPullMasterDAL.GetNewCreditNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, "Ms Pull", LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "rq")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "requisition").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


                //int TotalRecordCountRQ = 0;
                //ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
                //lstAllItems = obj.GetPagedItemsForModel(0, Int32.MaxValue, out TotalRecordCountRQ, string.Empty, "ID ASC", RoomID, CompanyID, false, false, SupplierIds, true, true, true, 0, "requisition", RoomDateFormat, true, null, null).ToList();
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "ord")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "order").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "quote")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "quote").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "retord")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "returnorder").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //int TotalRecordCountretord = 0;
                //if (HttpContext.Current.Session["AddItemToReturnOrder_" + RoomID + "_" + CompanyID] != null)
                //    lstAllItems = (List<ItemMasterDTO>)HttpContext.Current.Session["AddItemToReturnOrder_" + RoomID + "_" + CompanyID];
                //else
                //{
                //    ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
                //    lstAllItems = obj.GetPagedItemsForModel(0, Int32.MaxValue, out TotalRecordCountretord, string.Empty, "ID ASC", RoomID, CompanyID, false, false, SupplierIds, true, true, true, 0, "returnorder", RoomDateFormat, true, null, null).ToList();
                //    HttpContext.Current.Session["AddItemToReturnOrder_" + RoomID + "_" + CompanyID] = lstAllItems;
                //}
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "receive")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "receive").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //int TotalRecordCountreceive = 0;
                //ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
                //lstAllItems = obj.GetPagedRecordsForModel(0, Int32.MaxValue, out TotalRecordCountreceive, string.Empty, "ID asc", RoomID, CompanyID, false, false, "", false, SupplierIds);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "ql")
            {
                int? QuicklistType = Session_QuicklistType;

                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "ql", QuicklistType).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //int TotalRecordCountQL = 0;
                //ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
                //lstAllItems = obj.GetPagedRecordsForModel(0, Int32.MaxValue, out TotalRecordCountQL, string.Empty, "ID Asc", RoomID, CompanyID, false, false, string.Empty, string.Empty, SupplierIds).ToList();
                //if (HttpContext.Current.Session["QuicklistType"] != null && Convert.ToInt32(HttpContext.Current.Session["QuicklistType"]) == 3)
                //{
                //    lstAllItems = lstAllItems.Where(x => x.SerialNumberTracking == false && x.LotNumberTracking == false && x.DateCodeTracking == false).ToList();
                //}
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "returnord")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "returnorder").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //int TotalRecordCountreturnord = 0;
                //ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
                //lstAllItems = obj.GetPagedItemsForModel(0, Int32.MaxValue, out TotalRecordCountreturnord, string.Empty, "ID ASC", RoomID, CompanyID, false, false, SupplierIds, true, true, true, 0, "returnorder", RoomDateFormat, true, null, null).ToList();
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "trf")
            {
                return objItemDAL.GetItemsForTransferPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, ParentID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //if (HttpContext.Current.Session["TransferItemMasterList"] != null)
                //{
                //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["TransferItemMasterList"];
                //}
                //else
                //{
                //    lstAllItems = (IEnumerable<ItemMasterDTO>)new List<ItemMasterDTO>();
                //}
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "icnt")
            {
                //if (HttpContext.Current.Session["ItemMasterListForCount"] != null)
                //{
                //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["ItemMasterListForCount"];
                //}
                //else
                //{
                InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(base.DataBaseName);
                InventoryCountDTO objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountById(ParentID, RoomID, CompanyID);
                //int TotalRecordCounticnt = 0;
                if (objInventoryCountDTO != null)
                {
                    return objItemDAL.GetItemCountPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, IsSLCount, objInventoryCountDTO.GUID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                    //lstAllItems = objInventoryCountDAL.GetPagedItemLocationsForCount(0, int.MaxValue, out TotalRecordCounticnt, "", "ItemNumber Asc", RoomID, CompanyID, false, false, 0, IsSLCount, "", tmpsupplierIds, objInventoryCountDTO.GUID.ToString()).ToList();
                }
                else
                {
                    lstAllItems = new List<ItemMasterDTO>();
                }

                //  HttpContext.Current.Session["ItemMasterListForCount"] = lstAllItems;
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "as")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "as").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                //lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["AssetItemNarrowSearch"];
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "movemtr")
            {
                return objItemDAL.GetItemMoveMTRPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, Convert.ToInt32(moveType)).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //if (HttpContext.Current.Session["ItemMasterList"] != null)
                //{
                //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["ItemMasterList"];
                //}
                //else
                //{
                //    lstAllItems = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).Where(x => x.IsActive == true && x.ItemType != 4 && x.ItemType != 2).ToList();
                //    if (moveType != null)
                //    {
                //        if (moveType == MoveType.InvToInv || moveType == MoveType.InvToStag)
                //        {
                //            lstAllItems = lstAllItems.Where(x => x.OnHandQuantity.GetValueOrDefault(0) > 0);
                //        }
                //        else if (moveType == MoveType.StagToInv || moveType == MoveType.StagToStag)
                //        {
                //            lstAllItems = lstAllItems.Where(x => x.StagedQuantity.GetValueOrDefault(0) > 0);
                //        }
                //    }
                //    HttpContext.Current.Session["ItemMasterList"] = lstAllItems;
                //}
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "newcart")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "newcart").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


                //if (HttpContext.Current.Session["ItemMasterList"] != null)
                //{
                //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["ItemMasterList"];
                //}
                //else
                //{
                // lstAllItems = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).Where(x => x.IsActive == true).ToList();
                //    HttpContext.Current.Session["ItemMasterList"] = lstAllItems;
                //}
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "mntnance")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "mntnance").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "materialstaging")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "materialstaging").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "ps")
            {
                return objItemDAL.GetProjectSpendItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, ParentID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "itembinmaster")
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                return objItemMasterDAL.GetItemsBinListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "kit")
            {
                return objItemDAL.GetItemPopupNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount, "kit").ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
            }
            else
            {
                //if (HttpContext.Current.Session["ItemMasterList"] != null)
                //{
                //    lstAllItems = (IEnumerable<ItemMasterDTO>)HttpContext.Current.Session["ItemMasterList"];
                //}
                //else
                //{
                //lstAllItems = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).ToList();                    
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                return objItemMasterDAL.GetItemsListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

                //PullMasterDAL objGLAccDAL = new PullMasterDAL(base.DataBaseName);
                //return objGLAccDAL.GetPullMasterListNarrowSearch(RoomID, CompanyID, IsArchived, IsDeleted, TextFieldName, SupplierIds, IsAllowConsignedCredit, LoadDataCount).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);


                // HttpContext.Current.Session["ItemMasterList"] = lstAllItems;
                // }
            }

            Guid[] itemGuidFromOrder = null;
            if (string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() != "ord")
            {
                if (lstDetailDTO != null)
                {
                    itemGuidFromOrder = lstDetailDTO.Select(x => x.ItemGUID.GetValueOrDefault(Guid.Empty)).ToArray();
                    lstAllItems = lstAllItems.Where(x => !itemGuidFromOrder.Contains(x.GUID));
                }
            }
            else
            {
                itemGuidFromOrder = new Guid[0];
            }

            #region GET DATA
            if (TextFieldName == "CreatedBy")
            {
                if (SupplierIds != null && SupplierIds.Any())
                {
                    var lstsupps = (from tmp in lstAllItems.Where(t => t.CreatedBy.GetValueOrDefault(0) != 0 && t.IsArchived == IsArchived &&
                                t.IsDeleted == IsDeleted && (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0))))
                                    orderby tmp.CreatedByName
                                    group tmp by new { tmp.CreatedBy, tmp.CreatedByName } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.CreatedBy,
                                        supname = grp.Key.CreatedByName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
                else
                {
                    var lstsupps = (from tmp in lstAllItems.Where(t => t.CreatedBy.GetValueOrDefault(0) != 0 && t.IsArchived == IsArchived &&
                                t.IsDeleted == IsDeleted && (t.SupplierID.GetValueOrDefault(0) >= 0))
                                    orderby tmp.CreatedByName
                                    group tmp by new { tmp.CreatedBy, tmp.CreatedByName } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.CreatedBy,
                                        supname = grp.Key.CreatedByName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }

            }
            else if (TextFieldName == "SupplierName")
            {
                if (SupplierIds != null && SupplierIds.Any())
                {
                    var lstsupps = (from ci in lstAllItems
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted &&
                                    (SupplierIds.Contains(ci.SupplierID.GetValueOrDefault(0)))
                                    orderby ci.SupplierName
                                    group ci by new { ci.SupplierName, ci.SupplierID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.SupplierID,
                                        supname = grpms.Key.SupplierName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
                else
                {
                    var lstsupps = (from ci in lstAllItems
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && (ci.SupplierID.GetValueOrDefault(0) > 0)
                                    orderby ci.SupplierName
                                    group ci by new { ci.SupplierName, ci.SupplierID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.SupplierID,
                                        supname = grpms.Key.SupplierName
                                    });
                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if ((TextFieldName == "StageLocationHeader" || TextFieldName == "StageLocation") && !string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "icnt")
            {
                if (SupplierIds != null && SupplierIds.Any())
                {
                    var lstsupps = (from tmp in lstAllItems.Where(t => t.ParentBinId.GetValueOrDefault(0) != 0 && t.IsArchived == IsArchived &&
                                    t.IsDeleted == IsDeleted && (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0))))
                                    orderby tmp.ParentBinName
                                    group tmp by new { tmp.ParentBinId, tmp.ParentBinName } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.ParentBinId,
                                        supname = grp.Key.ParentBinName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
                else
                {
                    var lstsupps = (from tmp in lstAllItems.Where(t => t.ParentBinId.GetValueOrDefault(0) != 0 && t.IsArchived == IsArchived &&
                                    t.IsDeleted == IsDeleted && (t.SupplierID.GetValueOrDefault(0) >= 0))
                                    orderby tmp.ParentBinName
                                    group tmp by new { tmp.ParentBinId, tmp.ParentBinName } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.ParentBinId,
                                        supname = grp.Key.ParentBinName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }

            }
            else if (TextFieldName == "StageLocationHeader")
            {
                MaterialStagingDetailDAL objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                ColUDFData = objMSDtlDAL.GetPullStagingNarrowSerach(RoomID, CompanyID, SupplierIds, Session_ConsignedAllowed);
                return ColUDFData;
            }
            else if (TextFieldName == "StageLocation")
            {
                MaterialStagingDetailDAL objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                ColUDFData = objMSDtlDAL.GetPullStagingLocationsNarrowSerach(RoomID, CompanyID, SupplierIds, Session_ConsignedAllowed);

                return ColUDFData;
            }
            else if (TextFieldName == "Manufacturer")
            {
                if (SupplierIds != null && SupplierIds.Any())
                {
                    var lstsupps = (from ci in lstAllItems
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.ManufacturerID.GetValueOrDefault(0) > 0 &&
                                    (SupplierIds.Contains(ci.SupplierID.GetValueOrDefault(0)))
                                    orderby ci.ManufacturerName
                                    group ci by new { ci.ManufacturerName, ci.ManufacturerID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.ManufacturerID,
                                        supname = grpms.Key.ManufacturerName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
                else
                {
                    var lstsupps = (from ci in lstAllItems
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.ManufacturerID.GetValueOrDefault(0) > 0 &&
                                    (ci.SupplierID.GetValueOrDefault(0) > 0)
                                    orderby ci.ManufacturerName
                                    group ci by new { ci.ManufacturerName, ci.ManufacturerID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.ManufacturerID,
                                        supname = grpms.Key.ManufacturerName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "ItemLocation")
            {
                string supplierIdStr = "";

                if (SupplierIds != null && SupplierIds.Any())
                {
                    supplierIdStr = string.Join(",", SupplierIds);
                }

                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@SupplierIds", supplierIdStr),
                                            };

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ColUDFData = context.Database.SqlQuery<BinMasterDTO>("exec GetItemLocationNarrowSearchForItem @RoomID,@CompanyID,@SupplierIds ", params1).ToList().ToDictionary(e => e.BinNumber + "[###]" + e.ID.ToString(), e => (int)e.Count);
                }

                return ColUDFData;
            }
            else if (TextFieldName == "Category")
            {
                if (SupplierIds != null && SupplierIds.Any())
                {
                    var lstsupps = (from ci in lstAllItems
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.CategoryID.GetValueOrDefault(0) > 0
                                    && ((ci.ItemType != 2 ? SupplierIds.Contains(ci.SupplierID.GetValueOrDefault(0)) : ci.SupplierID.GetValueOrDefault(0) > 0) || (ci.ItemType == 2))
                                    orderby ci.CategoryName
                                    group ci by new { ci.CategoryName, ci.CategoryID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.CategoryID ?? 0,
                                        supname = grpms.Key.CategoryName ?? ""
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
                else
                {
                    var lstsupps = (from ci in lstAllItems
                                    where ci.IsArchived == IsArchived && ci.IsDeleted == IsDeleted && ci.CategoryID.GetValueOrDefault(0) > 0
                                    && ((ci.SupplierID.GetValueOrDefault(0) > 0) || (ci.ItemType == 2))
                                    orderby ci.CategoryName
                                    group ci by new { ci.CategoryName, ci.CategoryID } into grpms
                                    select new
                                    {
                                        count = grpms.Count(),
                                        sid = grpms.Key.CategoryID ?? 0,
                                        supname = grpms.Key.CategoryName ?? ""
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else if (TextFieldName == "ItemType")
            {
                if (SupplierIds != null && SupplierIds.Any())
                {
                    ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemType > 0 && t.IsArchived.GetValueOrDefault(false) == IsArchived &&
                              t.IsDeleted.GetValueOrDefault(false) == IsDeleted && (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))
                              && ((itemGuidFromOrder == null) || (itemGuidFromOrder != null && !itemGuidFromOrder.Contains(t.GUID))))
                                  orderby tmp.ItemType
                                  group tmp by new { tmp.ItemType }
                                  into grp
                                  select new
                                  {
                                      grp.Key.ItemType,
                                      Count = grp.Count()
                                  }
                    ).AsParallel().ToDictionary(e => e.ItemType.ToString(), e => e.Count);
                }
                else
                {
                    ColUDFData = (from tmp in lstAllItems.Where(t => t.ItemType > 0 && t.IsArchived.GetValueOrDefault(false) == IsArchived &&
                              t.IsDeleted.GetValueOrDefault(false) == IsDeleted && (t.SupplierID.GetValueOrDefault(0) >= 0)
                              && ((itemGuidFromOrder == null) || (itemGuidFromOrder != null && !itemGuidFromOrder.Contains(t.GUID))))
                                  orderby tmp.ItemType
                                  group tmp by new { tmp.ItemType }
                                  into grp
                                  select new
                                  {
                                      grp.Key.ItemType,
                                      Count = grp.Count()
                                  }
                    ).AsParallel().ToDictionary(e => e.ItemType.ToString(), e => e.Count);
                }

                if (ItemModelCallFrom.Trim().ToLower() != "retord" && (string.IsNullOrEmpty(ItemModelCallFrom) || ItemModelCallFrom.Trim().ToLower() != "ql"))
                {
                    if (ColUDFData.Where(c => c.Key == "2").Count() == 0)
                    {
                        QuickListDAL qlDAL = new QuickListDAL(base.DataBaseName);
                        int QLCount = qlDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted, QuickListType).Where(x => x.NoOfItems > 0).Count();

                        ColUDFData.Add("2", QLCount);
                    }
                }
                return ColUDFData;
            }
            else if (TextFieldName == "StockStatus")
            {
                ColUDFData.Add("" + "###" + "0", -1);

                if (SupplierIds != null && SupplierIds.Any())
                {
                    Int32 OutOfStockCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) == 0 && t.ItemType != 4 &&
                                            t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                            (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.OutOfStock + "###" + "1", OutOfStockCount);

                    Int32 BelowCriticalCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) < (t.CriticalQuantity == null ? 0
                                                : t.CriticalQuantity) && t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                                (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.BelowCritical + "###" + "2", BelowCriticalCount);

                    Int32 BelowMinimumCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) < (t.MinimumQuantity == null ? 0
                                              : t.MinimumQuantity) && (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) > (t.CriticalQuantity == null ? 0
                                              : t.CriticalQuantity) && t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                              (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.BelowMinimum + "###" + "3", BelowMinimumCount);

                    Int32 AboveMaximumCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) > (t.MaximumQuantity == null ? 0
                                                : t.MaximumQuantity) && t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                                (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0)))).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.AboveMaximum + "###" + "4", AboveMaximumCount);
                }
                else
                {
                    Int32 OutOfStockCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) == 0 && t.ItemType != 4 &&
                                            t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                            (t.SupplierID.GetValueOrDefault(0) > 0)).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.OutOfStock + "###" + "1", OutOfStockCount);

                    Int32 BelowCriticalCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) < (t.CriticalQuantity == null ? 0
                                                : t.CriticalQuantity) && t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                                (t.SupplierID.GetValueOrDefault(0) > 0)).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.BelowCritical + "###" + "2", BelowCriticalCount);

                    Int32 BelowMinimumCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) < (t.MinimumQuantity == null ? 0
                                            : t.MinimumQuantity) && (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) > (t.CriticalQuantity == null ? 0
                                            : t.CriticalQuantity) && t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                            (t.SupplierID.GetValueOrDefault(0) > 0)).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.BelowMinimum + "###" + "3", BelowMinimumCount);

                    Int32 AboveMaximumCount = lstAllItems.Where(t => (t.OnHandQuantity == null ? 0 : t.OnHandQuantity) > (t.MaximumQuantity == null ? 0
                                            : t.MaximumQuantity) && t.IsArchived == IsArchived && t.IsDeleted == IsDeleted &&
                                            (t.SupplierID.GetValueOrDefault(0) > 0)).Count();
                    ColUDFData.Add(eTurns.DTO.ResItemMaster.AboveMaximum + "###" + "4", AboveMaximumCount);
                }

                return ColUDFData;
            }
            else if (TextFieldName == "ItemTrackingType")
            {
                Int32 NoTrackingCount = lstAllItems.Where(l => l.SerialNumberTracking == false && l.LotNumberTracking == false && l.DateCodeTracking == false).Count();
                ColUDFData.Add("No Tracking" + "###" + "1", NoTrackingCount);
                Int32 SerialTrackingCount = lstAllItems.Where(l => l.SerialNumberTracking == true).Count();
                ColUDFData.Add(eTurns.DTO.ResItemMaster.SerialNumberTracking + "###" + "2", SerialTrackingCount);
                Int32 LotTrackingCount = lstAllItems.Where(l => l.LotNumberTracking == true).Count();
                ColUDFData.Add(eTurns.DTO.ResItemMaster.LotNumberTracking + "###" + "3", LotTrackingCount);
                Int32 DateTrackingCount = lstAllItems.Where(l => l.DateCodeTracking == true).Count();
                ColUDFData.Add(eTurns.DTO.ResItemMaster.DateCodeTracking + "###" + "4", DateTrackingCount);
                return ColUDFData;
            }
            else if (TextFieldName == "LastUpdatedBy")
            {
                if (SupplierIds != null && SupplierIds.Any())
                {
                    var lstsupps = (from tmp in lstAllItems.Where(t => t.LastUpdatedBy.GetValueOrDefault(0) != 0 && t.IsArchived == IsArchived &&
                                    t.IsDeleted == IsDeleted && (SupplierIds.Contains(t.SupplierID.GetValueOrDefault(0))))
                                    orderby tmp.UpdatedByName
                                    group tmp by new { tmp.LastUpdatedBy, tmp.UpdatedByName } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.LastUpdatedBy,
                                        supname = grp.Key.UpdatedByName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
                else
                {
                    var lstsupps = (from tmp in lstAllItems.Where(t => t.LastUpdatedBy.GetValueOrDefault(0) != 0 && t.IsArchived == IsArchived &&
                                    t.IsDeleted == IsDeleted && (t.SupplierID.GetValueOrDefault(0) >= 0))
                                    orderby tmp.UpdatedByName
                                    group tmp by new { tmp.LastUpdatedBy, tmp.UpdatedByName } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.LastUpdatedBy,
                                        supname = grp.Key.UpdatedByName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }

            }
            else if (TextFieldName == "InventoryClassification")
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var lstsupps = (from tmp in lstAllItems.Where(t => t.InventoryClassification > 0)
                                    join ICM in context.InventoryClassificationMasters on tmp.InventoryClassification equals ICM.ID
                                    orderby tmp.InventoryClassification
                                    group tmp by new { InventoryClassificationId = tmp.InventoryClassification, InventoryClassificationName = ICM.InventoryClassification } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.InventoryClassificationId,
                                        supname = grp.Key.InventoryClassificationName
                                    });

                    return lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count);
                }
            }
            else
            {
                return null;
            }
            #endregion
        }
    }
}
