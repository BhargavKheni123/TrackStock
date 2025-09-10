using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class ReceiveToolAssetOrderDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ReceiveToolAssetOrderDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public ReceiveToolAssetOrderDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class methods]        
        public IEnumerable<ToolMasterDTO> GetAllToolRecords(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolMasterDTO> objTool;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived) };
                objTool = context.Database.SqlQuery<ToolMasterDTO>("exec GetToolList @RoomId,@CompanyId,@IsDeleted,@IsArchieved", params1).ToList();
            }
            return objTool;
        }
        public IEnumerable<ReceivableToolDTO> GetALLReceiveList(Int64 RoomID, Int64 CompanyID, Guid? ItemGuid, Guid? OrderGuid, Guid? BinID, Guid? OrderDetailGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC RCV_GetPendingReceiveTools ";
                strCommand += CompanyID;
                strCommand += ", " + RoomID;
                if (ItemGuid.HasValue)
                    strCommand += ",'" + ItemGuid.Value + "'";
                else
                    strCommand += ",null";

                if (OrderGuid.HasValue)
                    strCommand += ",'" + OrderGuid.Value + "'";
                else
                    strCommand += ",null";

                if (BinID.HasValue)
                    strCommand += "," + BinID.Value;
                else
                    strCommand += ",null";

                if (OrderDetailGuid.HasValue)
                    strCommand += ",'" + OrderDetailGuid.Value + "'";
                else
                    strCommand += ",null";



                IEnumerable<ReceivableToolDTO> obj = (from u in context.Database.SqlQuery<ReceivableToolDTO>(strCommand)
                                                      select new ReceivableToolDTO
                                                      {
                                                          ToolAssetOrderDetailGUID = u.ToolAssetOrderDetailGUID,

                                                          OrderDetailRequiredDate = u.OrderDetailRequiredDate,
                                                          ToolAssetOrderGUID = u.ToolAssetOrderGUID,
                                                          ToolAssetOrderNumber = u.ToolAssetOrderNumber,
                                                          OrderReleaseNumber = u.OrderReleaseNumber,
                                                          OrderStatus = u.OrderStatus,


                                                          ReceiveBinID = u.ReceiveBinID,
                                                          ReceiveBinName = u.ReceiveBinName,
                                                          RequestedQuantity = u.RequestedQuantity,
                                                          ApprovedQuantity = u.ApprovedQuantity,
                                                          ToolGUID = u.ToolGUID,
                                                          Serial = u.Serial,
                                                          ToolName = u.ToolName,
                                                          ToolDescription = u.ToolDescription,
                                                          OrderRequiredDate = u.OrderRequiredDate,
                                                          ToolType = u.ToolType,
                                                          TotalRecords = u.TotalRecords,
                                                          OrderCreated = u.OrderCreated,
                                                          OrderCreatedByID = u.OrderCreatedByID,
                                                          OrderCreatedByName = u.OrderCreatedByName,
                                                          OrderLastUpdated = u.OrderLastUpdated,
                                                          OrderLastUpdatedByID = u.OrderLastUpdatedByID,
                                                          OrderUpdatedByName = u.OrderUpdatedByName,

                                                          ToolCost = u.ToolCost,

                                                          ToolID = u.ToolID,
                                                          ToolAssetOrderDetailID = u.ToolAssetOrderDetailID,
                                                          ToolAssetOrderID = u.ToolAssetOrderID,
                                                          ReceivedQuantity = u.ReceivedQuantity,

                                                          ToolUDF1 = u.ToolUDF1,
                                                          ToolUDF2 = u.ToolUDF2,
                                                          ToolUDF3 = u.ToolUDF3,
                                                          ToolUDF4 = u.ToolUDF4,
                                                          ToolUDF5 = u.ToolUDF5,

                                                          PackingQuantity = u.PackingQuantity,

                                                          InTransitQuantity = u.InTransitQuantity,
                                                          ODPackSlipNumbers = u.ODPackSlipNumbers,

                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,

                                                          IsCloseTool = u.IsCloseTool,
                                                          SerialNumberTracking = u.SerialNumberTracking

                                                      }).AsParallel().ToList();
                return obj;
            }

        }
        private string GetCommaSaperatedSearchPara(string val)
        {
            string[] ss = val.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string inPara = string.Empty;
            if (ss != null && ss.Length > 0)
            {
                foreach (var item in ss)
                {
                    if (!string.IsNullOrEmpty(inPara) && inPara.Length > 0)
                    {
                        inPara += ",";
                    }

                    inPara += "'" + item + "'";
                }

            }
            return inPara;
        }
        public IEnumerable<ReceivableToolDTO> GetALLReceiveListByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string OrderStatusIn, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            if (sortColumnName.Contains("OrderStatusText"))
            {
                sortColumnName = "OrderStatus";
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuer = "";
                string StatusQuery = OrderStatusIn;
                string NarrowSearchQry = "";
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
                {
                    string[] stringSeparators = new string[] { "[###]" };
                    string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string[] FieldsPara = Fields[1].Split('@');

                    DateTime FromdDate = DateTime.Now;
                    DateTime ToDate = DateTime.Now;


                    if (FieldsPara.Length > 1 && FieldsPara.Length > 1 && !string.IsNullOrWhiteSpace(FieldsPara[1]) && GetCommaSaperatedSearchPara(FieldsPara[1]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += " ToolAssetOrderNumber IN (" + GetCommaSaperatedSearchPara(FieldsPara[1]) + ")";
                    }

                    if (FieldsPara.Length > 2 && !string.IsNullOrWhiteSpace(FieldsPara[2]) && GetCommaSaperatedSearchPara(FieldsPara[2]).Count() > 0)
                    {

                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += " ToolAssetOrderCreatedByID IN (" + GetCommaSaperatedSearchPara(FieldsPara[2]) + ")";
                    }

                    if (FieldsPara.Length > 3 && !string.IsNullOrWhiteSpace(FieldsPara[3]) && GetCommaSaperatedSearchPara(FieldsPara[3]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += " ToolAssetOrderLastUpdatedByID IN (" + GetCommaSaperatedSearchPara(FieldsPara[3]) + ")";
                    }

                    if (FieldsPara.Length > 4 && FieldsPara.Length < 14 && !string.IsNullOrWhiteSpace(FieldsPara[4]) && GetCommaSaperatedSearchPara(FieldsPara[4]).Count() > 0)
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                        ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += @" Convert(Datetime,ToolAssetOrderCreated) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";

                    }

                    if (FieldsPara.Length > 5 && FieldsPara.Length < 14 && !string.IsNullOrWhiteSpace(FieldsPara[5]) && GetCommaSaperatedSearchPara(FieldsPara[5]).Count() > 0)
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                        ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += @" Convert(Datetime,ToolAssetOrderLastUpdated) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";

                    }
                    if (FieldsPara.Length > 6 && !string.IsNullOrWhiteSpace(FieldsPara[6]) && GetCommaSaperatedSearchPara(FieldsPara[6]).Count() > 0)
                    {
                        if (FieldsPara[6].Contains("1")) //This Week
                        {
                            FromdDate = DateTime.UtcNow;
                            ToDate = DateTime.UtcNow.AddDays(-7);
                            if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";

                            NarrowSearchQry += @" Convert(Datetime,ToolAssetReceivedDate) Between Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }

                        if (FieldsPara[6].Contains("2")) // Previous Week
                        {
                            FromdDate = DateTime.UtcNow.AddDays(-7);
                            ToDate = DateTime.UtcNow.AddDays(-14);
                            if (FieldsPara[6].Contains("1"))
                                NarrowSearchQry += " OR ";
                            else if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";
                            NarrowSearchQry += @" Convert(Datetime,ToolAssetReceivedDate) Between Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }
                        if (FieldsPara[6].Contains("3")) //  2-3 Week
                        {
                            FromdDate = DateTime.UtcNow.AddDays(-14);
                            ToDate = DateTime.UtcNow.AddDays(-21);
                            if (FieldsPara[6].Contains("1") || FieldsPara[6].Contains("2"))
                                NarrowSearchQry += " OR ";
                            else if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";
                            NarrowSearchQry += @" Convert(Datetime,ToolAssetReceivedDate) Between Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }
                        if (FieldsPara[6].Contains("4")) // > 3 weeks
                        {
                            FromdDate = DateTime.UtcNow.AddDays(-21);
                            if (FieldsPara[6].Contains("1") || FieldsPara[6].Contains("2") || FieldsPara[6].Contains("3"))
                                NarrowSearchQry += " OR ";
                            else if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";

                            NarrowSearchQry += @" Convert(Datetime,ToolAssetReceivedDate) <= Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }

                    }
                    if (FieldsPara.Length > 7 && !string.IsNullOrWhiteSpace(FieldsPara[7]) && GetCommaSaperatedSearchPara(FieldsPara[7]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ToolUDF1 IN (" + GetCommaSaperatedSearchPara(FieldsPara[7]) + ")";
                    }

                    if (FieldsPara.Length > 7 && !string.IsNullOrWhiteSpace(FieldsPara[8]) && GetCommaSaperatedSearchPara(FieldsPara[8]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ToolUDF2 IN (" + GetCommaSaperatedSearchPara(FieldsPara[8]) + ")";
                    }

                    if (FieldsPara.Length > 8 && !string.IsNullOrWhiteSpace(FieldsPara[9]) && GetCommaSaperatedSearchPara(FieldsPara[9]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ToolUDF3 IN (" + GetCommaSaperatedSearchPara(FieldsPara[9]) + ")";
                    }

                    if (FieldsPara.Length > 8 && !string.IsNullOrWhiteSpace(FieldsPara[10]) && GetCommaSaperatedSearchPara(FieldsPara[10]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ToolUDF4 IN (" + GetCommaSaperatedSearchPara(FieldsPara[10]) + ")";
                    }

                    if (FieldsPara.Length > 8 && !string.IsNullOrWhiteSpace(FieldsPara[11]) && GetCommaSaperatedSearchPara(FieldsPara[11]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ToolUDF5 IN (" + GetCommaSaperatedSearchPara(FieldsPara[11]) + ")";
                    }

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
                }

                strQuer = @"EXEC [RCV_GetReceivableToolsByPaging] " + CompanyID + "," + RoomID + "," + IsDeleted + "," + IsArchived + "," + StartRowIndex + "," + MaxRows + ",'" + sortColumnName + "','" + SearchTerm + "','" + NarrowSearchQry + "','" + StatusQuery + "' ";

                List<ReceivableToolDTO> obj = new List<ReceivableToolDTO>();
                DataSet ds = new DataSet();
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                ds = SqlHelper.ExecuteDataset(EturnsConnection, "RCV_GetReceivableToolsByPaging", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, NarrowSearchQry, OrderStatusIn);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dtCart = ds.Tables[0];
                    if (dtCart.Rows.Count > 0)
                    {
                        obj = dtCart.AsEnumerable()
                        .Select(row => new ReceivableToolDTO
                        {
                            ToolAssetOrderDetailGUID = row.Field<Guid>("ToolAssetOrderDetailGUID"),
                            OrderDetailRequiredDate = row.Field<DateTime>("OrderDetailRequiredDate"),
                            ToolAssetOrderGUID = row.Field<Guid>("ToolAssetOrderGUID"),
                            ToolAssetOrderNumber = row.Field<string>("ToolAssetOrderNumber"),
                            OrderReleaseNumber = row.Field<string>("OrderReleaseNumber"),
                            OrderStatus = row.Field<int>("OrderStatus"),
                            OrderStatusText = ResToolAssetOrder.GetOrderStatusText(((eTurns.DTO.ToolAssetOrderStatus)row.Field<int>("OrderStatus")).ToString()),

                            ReceiveBinID = row.Field<Int64?>("ReceiveBinID"),
                            ReceiveBinName = row.Field<string>("ReceiveBinName"),
                            RequestedQuantity = row.Field<double>("RequestedQuantity"),
                            ApprovedQuantity = row.Field<double>("ApprovedQuantity"),
                            ToolGUID = row.Field<Guid>("ToolGUID"),
                            ToolName = row.Field<string>("ToolName"),
                            Serial = row.Field<string>("Serial"),
                            OrderRequiredDate = row.Field<DateTime>("OrderRequiredDate"),
                            ToolType = row.Field<int>("ToolType"),

                            TotalRecords = row.Field<int>("TotalRecords"),
                            OrderCreated = row.Field<DateTime>("OrderCreated"),
                            OrderCreatedByID = row.Field<Int64>("OrderCreatedByID"),
                            OrderCreatedByName = row.Field<string>("OrderCreatedByName"),
                            OrderLastUpdated = row.Field<DateTime>("OrderLastUpdated"),
                            OrderLastUpdatedByID = row.Field<Int64>("OrderLastUpdatedByID"),
                            OrderUpdatedByName = row.Field<string>("OrderUpdatedByName"),

                            SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                            ToolCost = row.Field<double>("ToolCost"),
                            ToolDefaultLocation = row.Field<Int64>("ToolDefaultLocation"),
                            ToolID = row.Field<Int64>("ToolID"),
                            ToolAssetOrderDetailID = row.Field<Int64>("ToolAssetOrderDetailID"),
                            ToolAssetOrderID = row.Field<Int64>("ToolAssetOrderID"),
                            ToolDefaultLocationName = row.Field<string>("ToolDefaultLocationName"),
                            ReceivedQuantity = row.Field<double>("ReceivedQuantity"),

                            ToolUDF1 = row.Field<string>("ToolUDF1"),
                            ToolUDF2 = row.Field<string>("ToolUDF2"),
                            ToolUDF3 = row.Field<string>("ToolUDF3"),
                            ToolUDF4 = row.Field<string>("ToolUDF4"),
                            ToolUDF5 = row.Field<string>("ToolUDF5"),


                            AddedFrom = row.Field<string>("AddedFrom"),
                            EditedFrom = row.Field<string>("EditedFrom"),
                            ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                            ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                            IsCloseTool = row.Field<bool?>("IsCloseTool"),
                            ToolQuantity = row.Field<double?>("ToolQuantity"),

                        }).ToList();
                    }
                }
                TotalCount = 0;
                if (obj != null && obj.Count() > 0)
                {
                    TotalCount = obj.ElementAt(0).TotalRecords;
                }

                return obj;
            }




        }

        public IEnumerable<ToolAssetOrderDetailsDTO> CreateDirectReceiveToolAssetOrder(string OrderNumber, Guid ToolGuid, double ReqQty, Int64 LocationID, DateTime ReceiveDate, double ReceiveQty, Int64 RoomID, Int64 CompanyID, Int64 UserID, string OrderNumberForSorting, string EditedFrom, DateTime ReceivedOn, string addedFrom, DateTime ReceivedOnWeb, string PackSlipNumber)
        {
            string strCommand = "EXEC RCV_CreateDirectReceiveToolAssetOrder @ordernumber,@toolguid,@reqqty,@locationid,@receivedate,@receiveqty,@userid,@roomid,@companyid,@ordershort,@AddedFrom,@ReceivedOnWeb,@EditedFrom,@ReceivedOn,@PackSlipNumber";


            SqlParameter OrderNumberParam = new SqlParameter() { ParameterName = "ordernumber", Value = OrderNumber, SqlDbType = SqlDbType.NVarChar };
            SqlParameter ToolGuidParam = new SqlParameter() { ParameterName = "toolguid", Value = ToolGuid, SqlDbType = SqlDbType.UniqueIdentifier };
            SqlParameter ReqQtyParam = new SqlParameter() { ParameterName = "reqqty", Value = ReqQty, SqlDbType = SqlDbType.Float };
            SqlParameter LocationIDParam = new SqlParameter() { ParameterName = "locationid", Value = LocationID, SqlDbType = SqlDbType.BigInt };
            SqlParameter ReceiveDateParam = new SqlParameter() { ParameterName = "receivedate", Value = ReceiveDate.ToString("yyyy-MM-dd"), SqlDbType = SqlDbType.NVarChar };
            SqlParameter ReceiveQtyParam = new SqlParameter() { ParameterName = "receiveqty", Value = ReceiveQty, SqlDbType = SqlDbType.Float };
            SqlParameter UserIDParam = new SqlParameter() { ParameterName = "userid", Value = UserID, SqlDbType = SqlDbType.BigInt };
            SqlParameter RoomIDParam = new SqlParameter() { ParameterName = "roomid", Value = RoomID, SqlDbType = SqlDbType.BigInt };
            SqlParameter CompanyIDParam = new SqlParameter() { ParameterName = "companyid", Value = CompanyID, SqlDbType = SqlDbType.BigInt };
            SqlParameter OrderShortParam = new SqlParameter() { ParameterName = "ordershort", Value = CommonDAL.GetSortingString(OrderNumber), SqlDbType = SqlDbType.NVarChar };
            SqlParameter AddedFromParam = new SqlParameter() { ParameterName = "AddedFrom", Value = addedFrom, SqlDbType = SqlDbType.NVarChar };
            SqlParameter ReceivedOnWebParam = new SqlParameter() { ParameterName = "ReceivedOnWeb", Value = ReceivedOnWeb, SqlDbType = SqlDbType.DateTime };
            SqlParameter EditedFromParam = new SqlParameter() { ParameterName = "EditedFrom", Value = EditedFrom, SqlDbType = SqlDbType.NVarChar };
            SqlParameter ReceivedOnParam = new SqlParameter() { ParameterName = "ReceivedOn", Value = ReceivedOn, SqlDbType = SqlDbType.DateTime };
            SqlParameter PackSlipNumberParam;
            if (!string.IsNullOrEmpty(PackSlipNumber))
            {
                PackSlipNumberParam = new SqlParameter() { ParameterName = "PackSlipNumber", Value = PackSlipNumber, SqlDbType = SqlDbType.NVarChar };
            }
            else
            {
                PackSlipNumberParam = new SqlParameter() { ParameterName = "PackSlipNumber", Value = (object)DBNull.Value, SqlDbType = SqlDbType.NVarChar };
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<Guid> OrderGuid = (from u in context.Database.SqlQuery<Guid>(strCommand, OrderNumberParam, ToolGuidParam, ReqQtyParam, LocationIDParam, ReceiveDateParam, ReceiveQtyParam, UserIDParam, RoomIDParam, CompanyIDParam, OrderShortParam, AddedFromParam, ReceivedOnWebParam, EditedFromParam, ReceivedOnParam, PackSlipNumberParam)
                                               select u);

                Guid OrdGuid = OrderGuid.FirstOrDefault();


                ToolAssetOrderMasterDAL objOrderDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);
                ToolAssetOrderMasterDTO objOrderDTO = objOrderDAL.GetRecord(OrdGuid, RoomID, CompanyID);

                new AutoSequenceDAL(base.DataBaseName).UpdateNextToolAssetOrderNumber(RoomID, CompanyID, objOrderDTO.ToolAssetOrderNumber);

                ToolAssetOrderDetailsDAL objOrderDetailDAL = new ToolAssetOrderDetailsDAL(base.DataBaseName);
                List<ToolAssetOrderDetailsDTO> obj = objOrderDetailDAL.GetOrderedRecord(OrdGuid, RoomID, CompanyID);

                return obj;
            }

        }

        #endregion
    }
}


