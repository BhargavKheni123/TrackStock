using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class OrderUOMMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public OrderUOMMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public OrderUOMMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public List<OrderUOMMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, bool IsForBom, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedBy = "";
            string UpdatedBy = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string UDF1 = "";
            string UDF2 = "";
            string UDF3 = "";
            string UDF4 = "";
            string UDF5 = "";
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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
                    CreatedBy = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedBy = FieldsPara[1].TrimEnd(',');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
            }
            else
            {
                SearchTerm = "";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedBy),
                    new SqlParameter("@LastUpdatedBy", UpdatedBy),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@IsForBom", IsForBom)

                };
                List<OrderUOMMasterDTO> lstcats = context.Database.SqlQuery<OrderUOMMasterDTO>("exec [GetPagedOrderUOMMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@UpdatedFrom,@UpdatedTo,@CreatedBy,@LastUpdatedBy,@IsDeleted,@IsArchived,@CompanyID,@Room,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsForBom", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }

        }

        /// <summary>
        /// Get Particullar Record from the OrderUOMMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderUOMMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? isForBom = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsForBom", isForBom), new SqlParameter("@OrderUOMID", id) };
                return context.Database.SqlQuery<OrderUOMMasterDTO>("exec [GetOrderUOMMasterByID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@IsForBom,@OrderUOMID", params1).FirstOrDefault();
            }
        }

        public OrderUOMMasterDTO GetRecord(string OrderUOM, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsForBom", false), new SqlParameter("@OrderUOM", OrderUOM) };
                return context.Database.SqlQuery<OrderUOMMasterDTO>("exec [GetOrderUOMMasterByOrderUOM] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@IsForBom,@OrderUOM", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase OrderUOMMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(OrderUOMMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderUOMMaster obj = new OrderUOMMaster();
                obj.ID = 0;
                obj.OrderUOM = objDTO.OrderUOM;
                obj.OrderUOMValue = objDTO.OrderUOMValue;
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
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.IsForBOM = objDTO.isForBOM;
                if ((objDTO.isForBOM))
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                context.OrderUOMMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(OrderUOMMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderUOMMaster obj = new OrderUOMMaster();
                obj.ID = objDTO.ID;
                obj.OrderUOM = objDTO.OrderUOM;
                obj.OrderUOMValue = objDTO.OrderUOMValue;
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
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsForBOM = objDTO.isForBOM;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                context.OrderUOMMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if ((objDTO.isForBOM))
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }

                ////Get Cached-Media
                //IEnumerable<CostUOMMasterDTO> ObjCache = CacheHelper<IEnumerable<CostUOMMasterDTO>>.GetCacheItem("Cached_CostUOMMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<CostUOMMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<CostUOMMasterDTO> tempC = new List<CostUOMMasterDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<CostUOMMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<CostUOMMasterDTO>>.AppendToCacheItem("Cached_CostUOMMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //}


                return true;
            }
        }

        public double GetOrderUOMQty(ItemMasterDTO objItemMasterDTO, double? QTY)
        {
            double Total = 0;
            if (objItemMasterDTO.IsAllowOrderCostuom)
            {
                OrderUOMMasterDTO orderUOM = new OrderUOMMasterDTO();
                if (objItemMasterDTO.OrderUOMValue <= 0)
                {
                    orderUOM.OrderUOMValue = 1;
                }
                else
                {
                    orderUOM.OrderUOMValue = objItemMasterDTO.OrderUOMValue;
                }

                if (orderUOM.OrderUOMValue == null || orderUOM.OrderUOMValue <= 0)
                {
                    orderUOM.OrderUOMValue = 1;
                }

                if (QTY != null && QTY >= 0)
                    Total = Convert.ToDouble(QTY * orderUOM.OrderUOMValue);
            }
            else
            {
                Total = (double)QTY;
            }
            return Total;

        }

        public void InsertDefaultOrderUOMByNameAndValue(string OrderUOM, int OrderUOMValue, Int64? CreatedUserID, Int64 RoomID, Int64? CompanyID, bool? IsForBOM = false)
        {
            OrderUOMMasterDTO objDTO = new OrderUOMMasterDTO();
            objDTO.ID = 0;
            objDTO.OrderUOM = OrderUOM;
            objDTO.OrderUOMValue = OrderUOMValue;
            objDTO.GUID = Guid.NewGuid();
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = CreatedUserID;
            objDTO.LastUpdatedBy = CreatedUserID;
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;
            objDTO.CompanyID = CompanyID;
            objDTO.Room = RoomID;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.AddedFrom = "Web";
            objDTO.EditedFrom = "Web";
            objDTO.isForBOM = IsForBOM ?? false;
            Insert(objDTO);
        }

        public Int64 GetOrderUomIdByCostUomId(Int64 CostUOMID, ItemMasterDTO objDTO)
        {
            Int64 OrderUOMID = 0;
            CostUOMMaster CUOMMaster = null;
            OrderUOMMaster OrderUOM = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CUOMMaster = context.CostUOMMasters.Where(t => t.ID == CostUOMID && t.CompanyID == objDTO.CompanyID && t.Room == objDTO.Room).FirstOrDefault();
                if (CUOMMaster != null)
                {
                    OrderUOM = context.OrderUOMMasters.Where(t => t.OrderUOM == CUOMMaster.CostUOM && t.CompanyID == CUOMMaster.CompanyID && t.Room == CUOMMaster.Room).FirstOrDefault();
                    if (OrderUOM == null)
                    {
                        OrderUOMMasterDTO objOrderUOMMasterDTO = new OrderUOMMasterDTO();
                        objOrderUOMMasterDTO.OrderUOM = CUOMMaster.CostUOM;
                        objOrderUOMMasterDTO.OrderUOMValue = CUOMMaster.CostUOMValue;
                        objOrderUOMMasterDTO.GUID = Guid.NewGuid();
                        objOrderUOMMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objOrderUOMMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objOrderUOMMasterDTO.AddedFrom = objDTO.AddedFrom;
                        objOrderUOMMasterDTO.EditedFrom = objDTO.EditedFrom;
                        objOrderUOMMasterDTO.CreatedBy = objDTO.CreatedBy;
                        objOrderUOMMasterDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                        objOrderUOMMasterDTO.CompanyID = objDTO.CompanyID;
                        objOrderUOMMasterDTO.Room = objDTO.Room;

                        OrderUOMID = Insert(objOrderUOMMasterDTO);
                    }
                    else
                    {
                        OrderUOMID = OrderUOM.ID;
                    }
                }
            }

            return OrderUOMID;
        }

    }
}
