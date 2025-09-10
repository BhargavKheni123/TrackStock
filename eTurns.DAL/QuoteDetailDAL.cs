using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data.SqlClient;
using System.Web;
using eTurns.DTO.Resources;
using System.Data;

namespace eTurns.DAL
{
    public class QuoteDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public QuoteDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        #endregion
        #region [Class Methods]
        //for approved Quotemaster data only. method need to use in cart page
        public List<QuoteDetailDTO> GetPagedQuoteDetailByQuoteGUID(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid QuoteMasterGUID, List<long> SupplierIds, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {

            string Locations = null;
            string Suppliers = null;
            string Manufacturers = null;
            string Category = null;
            string QuoteCreaters = null;
            string QuoteUpdators = null;
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
            
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('~');
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
                    Locations = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    Suppliers = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    Category = FieldsPara[2].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    Manufacturers = FieldsPara[3].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    QuoteCreaters = FieldsPara[4].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    QuoteUpdators = FieldsPara[5].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('~')[6].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('~')[6].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('~')[7].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('~')[7].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrQItemsUDF = FieldsPara[8].Split(',');
                    foreach (string supitem in arrQItemsUDF)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string[] arrQItemsUDF = FieldsPara[9].Split(',');
                    foreach (string supitem in arrQItemsUDF)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string[] arrQItemsUDF = FieldsPara[10].Split(',');
                    foreach (string supitem in arrQItemsUDF)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    string[] arrQItemsUDF = FieldsPara[11].Split(',');
                    foreach (string supitem in arrQItemsUDF)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[12]))
                {
                    string[] arrQItemsUDF = FieldsPara[12].Split(',');
                    foreach (string supitem in arrQItemsUDF)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
            }

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            List<QuoteDetailDTO> lstQuoteLineItems = new List<QuoteDetailDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@StartRowIndex", StartRowIndex),
                                new SqlParameter("@MaxRows", MaxRows),
                new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                new SqlParameter("@Locations", Locations ?? (object)DBNull.Value),
                new SqlParameter("@Suppliers", Suppliers ?? (object)DBNull.Value),
                new SqlParameter("@Manufacturers", Manufacturers ?? (object)DBNull.Value),
                new SqlParameter("@Category", Category ?? (object)DBNull.Value),
                new SqlParameter("@QuoteCreaters", QuoteCreaters ?? (object)DBNull.Value),
                new SqlParameter("@QuoteUpdators", QuoteUpdators ?? (object)DBNull.Value),
                new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? (object)DBNull.Value),
                new SqlParameter("@CreatedDateTo", CreatedDateTo ?? (object)DBNull.Value),
                new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? (object)DBNull.Value),
                new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? (object)DBNull.Value),
                new SqlParameter("@UDF1", UDF1 ?? (object)DBNull.Value),
                new SqlParameter("@UDF2", UDF2 ?? (object)DBNull.Value),
                new SqlParameter("@UDF3", UDF3 ?? (object)DBNull.Value),
                new SqlParameter("@UDF4", UDF4 ?? (object)DBNull.Value),
                new SqlParameter("@UDF5", UDF5 ?? (object)DBNull.Value),
                new SqlParameter("@IsDeleted", IsDeleted),
                new SqlParameter("@IsArchived", IsArchived),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@CompanyID", CompanyID),
                new SqlParameter("@ItemUDF1", ItemUDF1 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF2", ItemUDF2 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF3", ItemUDF3 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF4", ItemUDF4 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF5", ItemUDF5 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF6", ItemUDF6 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF7", ItemUDF7 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF8", ItemUDF8 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF9", ItemUDF9 ?? (object)DBNull.Value),
                new SqlParameter("@ItemUDF10", ItemUDF10 ?? (object)DBNull.Value),
                new SqlParameter("@QuoteMasterGUID", QuoteMasterGUID),
                new SqlParameter("@SupplierIds", strSupplierIds ?? (object)DBNull.Value)
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstQuoteLineItems = context.Database.SqlQuery<QuoteDetailDTO>("exec [GetPagedQuoteDetailByQuoteGUID] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@Locations,@Suppliers,@Manufacturers,@Category,@QuoteCreaters,@QuoteUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsDeleted,@IsArchived,@RoomID,@CompanyID,@ItemUDF1,@ItemUDF2,@ItemUDF3,@ItemUDF4,@ItemUDF5,@ItemUDF6,@ItemUDF7,@ItemUDF8,@ItemUDF9,@ItemUDF10,@QuoteMasterGUID,@SupplierIds", params1).ToList();
                if (lstQuoteLineItems != null && lstQuoteLineItems.Count > 0)
                {
                    TotalCount = lstQuoteLineItems[0].TotalRecords;
                }
                else
                {
                    TotalCount = 0;
                }
            }
            return lstQuoteLineItems;
        }

        //for any Quotemaster data only. use this method while edit quotemaster and bound grid
        public List<QuoteDetailDTO> GetQuoteDetailPagedDataUsingQuoteGUID(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string QuoteMasterGUID)
        {
            string spName = "GetQuoteDetailPagedDataUsingQuoteGUID";

            List<QuoteDetailDTO> lstQuote = new List<QuoteDetailDTO>();
            TotalCount = 0;
            DataSet dsQuote = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstQuote;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            string QuoteStatusIn = string.Empty;

            if (MaxRows < 1)
            {
                MaxRows = 10;
            }

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());


            dsQuote = SqlHelper.ExecuteDataset(EturnsConnection, spName, CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, QuoteMasterGUID);



            if (dsQuote != null && dsQuote.Tables.Count > 0)
            {
                lstQuote = DataTableHelper.ToList<QuoteDetailDTO>(dsQuote.Tables[0]);
                TotalCount = 0;

                if (lstQuote != null && lstQuote.Count() > 0)
                {
                    TotalCount = lstQuote.ElementAt(0).TotalRecords;
                }
            }

            return lstQuote;
        }
        public IEnumerable<CommonDTO> GetQuoteDetailForNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> SupplierIds, int LoadDataCount, Guid QuoteMasterGUID)
        {
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds ?? (object)DBNull.Value),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@QuoteMasterGUID", QuoteMasterGUID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CommonDTO>("exec [GetQuoteDetailCartNarrowSearchData] @RoomID,@CompanyID, @IsDeleted,@NarrowSearchKey,@SupplierIds,@LoadDataCount,@QuoteMasterGUID", params1).ToList();
            }
        }
        public QuoteDetailDTO GetQuoteDetailbyGUIDPlain(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                return context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteDetailbyGUIDPlain] @Guid", params1).FirstOrDefault();
            }
        }
        public QuoteDetailDTO GetQuoteDetailbyGUIDNormal(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                return context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteDetailbyGUIDNormal] @Guid", params1).FirstOrDefault();
            }
        }
        public List<QuoteDetailDTO> GetQuoteDetailbyItemGUIDPlain(Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid) };
                return context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteDetailbyItemGUIDPlain] @ItemGuid", params1).ToList();
            }
        }
        public List<QuoteDetailDTO> GetQuoteDetailbyItemGUIDNormal(Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid) };
                return context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteDetailbyItemGUIDNormal] @ItemGuid", params1).ToList();
            }
        }
        public List<QuoteDetailDTO> GetQuoteDetailbyItemAndBinGUIDPlain(Guid ItemGuid, Int64 BinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@BinID", BinID) };
                return context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteDetailbyItemAndBinGUIDPlain] @ItemGuid,@BinID", params1).ToList();
            }
        }
        public List<QuoteDetailDTO> GetQuoteDetailbyItemAndBinGUIDNormal(Guid ItemGuid, Int64 BinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@BinID", BinID) };
                return context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteDetailbyItemAndBinGUIDNormal] @ItemGuid,@BinID", params1).ToList();
            }
        }

        public QuoteDetailDTO Insert(QuoteDetailDTO objDTO, long SessionUserId, long EnterpriseId)
        {
            QuoteDetailDTO DetailDTO = InsertQuoteDetail(objDTO);

            ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, DetailDTO.ItemGUID);
            objItemDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
            objItemDTO.WhatWhereAction = "Quote";
            itmDAL.Edit(objItemDTO, SessionUserId,EnterpriseId);

            return DetailDTO;
        }

        public QuoteDetailDTO InsertQuoteDetail(QuoteDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                      {
                        new SqlParameter("@QuoteGUID", objDTO.QuoteGUID),
                        new SqlParameter("@ItemGUID", objDTO.ItemGUID ),
                        new SqlParameter("@BinID", objDTO.BinID.GetValueOrDefault(0) > 0  ? objDTO.BinID.GetValueOrDefault(0): (object)DBNull.Value),
                        new SqlParameter("@RequestedQuantity", objDTO.RequestedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantity", objDTO.ApprovedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@OrderedQuantity", objDTO.OrderedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@RequiredDate", objDTO.RequiredDate.GetValueOrDefault(DateTime.Now)),
                        new SqlParameter("@CreatedBy",  objDTO.CreatedBy),
                        new SqlParameter("@Room", objDTO.Room),
                        new SqlParameter("@CompanyID", objDTO.CompanyID),
                        new SqlParameter("@InTransitQuantity", objDTO.InTransitQuantity.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantity.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ASNNumber", objDTO.ASNNumber ?? (object)DBNull.Value),
                        new SqlParameter("@IsEDISent",objDTO.IsEDISent.GetValueOrDefault(false)),
                        new SqlParameter("@ReceivedOnWeb", objDTO.ReceivedOnWeb),
                        new SqlParameter("@ReceivedON", objDTO.ReceivedOn),
                        new SqlParameter("@AddedFrom", (!string.IsNullOrWhiteSpace(objDTO.AddedFrom)) ? objDTO.AddedFrom : "Web" ),
                        new SqlParameter("@EditedFrom", (!string.IsNullOrWhiteSpace(objDTO.EditedFrom)) ? objDTO.EditedFrom : "Web" ),
                        new SqlParameter("@LineNumber", objDTO.LineNumber ?? (object)DBNull.Value),
                        new SqlParameter("@ControlNumber", objDTO.ControlNumber ?? (object)DBNull.Value),
                        new SqlParameter("@Comment", objDTO.Comment ?? (object)DBNull.Value),
                        new SqlParameter("@UDF1", objDTO.UDF1 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF2", objDTO.UDF2 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF3", objDTO.UDF3 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF4", objDTO.UDF4 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF5", objDTO.UDF5 ?? (object)DBNull.Value),
                        new SqlParameter("@DetailGUID", objDTO.GUID != Guid.Empty ?  objDTO.GUID : (object)DBNull.Value),
                        new SqlParameter("@QuoteLineItemExtendedCost", objDTO.QuoteLineItemExtendedCost.GetValueOrDefault(0)),
                        new SqlParameter("@QuoteLineItemExtendedPrice", objDTO.QuoteLineItemExtendedPrice.GetValueOrDefault(0)),
                        new SqlParameter("@IsCloseItem",objDTO.IsCloseItem.GetValueOrDefault(false)),
                        new SqlParameter("@RequestedQuantityUOM", objDTO.RequestedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantityUOM", objDTO.ApprovedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@OrderedQuantityUOM",objDTO.OrderedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@InTransitQuantityUOM", objDTO.InTransitQuantityUOM.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantityUOM.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ItemCost", objDTO.ItemCost.GetValueOrDefault(0)),
                        new SqlParameter("@ItemCostUOM", objDTO.ItemCostUOM.GetValueOrDefault(0) > 0 ? objDTO.ItemCostUOM.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@SupplierId", objDTO.SupplierID.GetValueOrDefault(0) > 0 ? objDTO.SupplierID.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@SupplierPartNo", objDTO.SupplierPartNo ?? (object)DBNull.Value),
                        new SqlParameter("@ItemSellPrice", objDTO.ItemSellPrice.GetValueOrDefault(0)),
                        new SqlParameter("@ItemMarkup", objDTO.ItemMarkup.GetValueOrDefault(0)),
                        new SqlParameter("@ItemCostUOMValue", (objDTO.ItemCostUOMValue == null || objDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0 ? 1 : objDTO.ItemCostUOMValue.GetValueOrDefault(1))),
                        new SqlParameter("@POItemLineNumber", objDTO.POItemLineNumber ?? (object)DBNull.Value)
                      };


                string strCommand = " EXEC [InsertQuoteDetailData] @QuoteGUID,@ItemGUID,@BinID,@RequestedQuantity,@ApprovedQuantity,@OrderedQuantity,@RequiredDate,@CreatedBy,@Room,@CompanyID,@InTransitQuantity,@ASNNumber,@IsEDISent,@ReceivedOnWeb,@ReceivedON,@AddedFrom,@EditedFrom,@LineNumber,@ControlNumber,@Comment,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@DetailGUID,@QuoteLineItemExtendedCost,@QuoteLineItemExtendedPrice,@IsCloseItem,@RequestedQuantityUOM,@ApprovedQuantityUOM,@OrderedQuantityUOM,@InTransitQuantityUOM,@ItemCost,@ItemCostUOM,@SupplierId,@SupplierPartNo,@ItemSellPrice,@ItemMarkup,@ItemCostUOMValue,@POItemLineNumber";
                context.Database.CommandTimeout = 600;
                return context.Database.SqlQuery<QuoteDetailDTO>(strCommand, params1).FirstOrDefault();

            }
        }

        public bool UpdateQuoteDetail(QuoteDetailDTO objDTO, long SessionUserId,long EnterpriseId)
        {
            var isUpdated = UpdateQuoteDetailData(objDTO);

            if (isUpdated)
            {
                ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
                ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);
                objItemDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                objItemDTO.WhatWhereAction = "Quote";
                objItemDTO.IsOnlyFromItemUI = false;
                itmDAL.Edit(objItemDTO, SessionUserId,EnterpriseId);

                return true;
            }
            return false;
        }

        private bool UpdateQuoteDetailData(QuoteDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                  {
                        new SqlParameter("@DetailID",  objDTO.ID),
                        new SqlParameter("@DetailGUID", (objDTO.GUID != Guid.Empty) ? objDTO.GUID : (object)DBNull.Value),
                        new SqlParameter("@QuoteGUID", objDTO.QuoteGUID ),
                        new SqlParameter("@ItemGUID", objDTO.ItemGUID ),
                        new SqlParameter("@Bin", objDTO.BinID  ?? (object)DBNull.Value),
                        new SqlParameter("@RequestedQuantity", objDTO.RequestedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantity", objDTO.ApprovedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@RequiredDate", objDTO.RequiredDate.GetValueOrDefault(DateTime.Now)),
                        new SqlParameter("@OrderedQuantity", objDTO.OrderedQuantity  ?? (object)DBNull.Value),
                        new SqlParameter("@LastUpdatedBy",  objDTO.LastUpdatedBy),
                        new SqlParameter("@Room", objDTO.Room),
                        new SqlParameter("@CompanyID", objDTO.CompanyID),
                        new SqlParameter("@IsEDISENT",objDTO.IsEDISent.GetValueOrDefault(false)),
                        new SqlParameter("@InTransitQuantity", objDTO.InTransitQuantity.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantity.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ASNNumber", objDTO.ASNNumber ?? (object)DBNull.Value),
                        new SqlParameter("@ReceivedOn", objDTO.ReceivedOn),
                        new SqlParameter("@EditedFrom", (!string.IsNullOrWhiteSpace(objDTO.EditedFrom)) ? objDTO.EditedFrom : "Web" ),
                        new SqlParameter("@Comment", objDTO.Comment ?? (object)DBNull.Value),
                        new SqlParameter("@LastEDIDate", objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) : (object)DBNull.Value),
                        new SqlParameter("@IsCloseItem", objDTO.IsCloseItem.GetValueOrDefault(false)),
                        new SqlParameter("@UDF1", objDTO.UDF1 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF2", objDTO.UDF2 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF3", objDTO.UDF3 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF4", objDTO.UDF4 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF5", objDTO.UDF5 ?? (object)DBNull.Value),
                        new SqlParameter("@QuoteLineItemExtendedCost", objDTO.QuoteLineItemExtendedCost.GetValueOrDefault(0)),
                        new SqlParameter("@QuoteLineItemExtendedPrice", objDTO.QuoteLineItemExtendedPrice.GetValueOrDefault(0)),
                        new SqlParameter("@RequestedQuantityUOM", objDTO.RequestedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantityUOM", objDTO.ApprovedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@OrderedQuantityUOM", objDTO.OrderedQuantityUOM  ?? (object)DBNull.Value),
                        new SqlParameter("@InTransitQuantityUOM", objDTO.InTransitQuantityUOM.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantityUOM.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ItemCost", objDTO.ItemCost.GetValueOrDefault(0)),
                        new SqlParameter("@SupplierId", objDTO.SupplierID.GetValueOrDefault(0) > 0 ? objDTO.SupplierID.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@SupplierPartNo", objDTO.SupplierPartNo ?? (object)DBNull.Value),
                        new SqlParameter("@ItemSellPrice", objDTO.ItemSellPrice.GetValueOrDefault(0)),
                        new SqlParameter("@ItemMarkup", objDTO.ItemMarkup.GetValueOrDefault(0)),
                        new SqlParameter("@ItemCostUOMValue",(objDTO.ItemCostUOMValue == null || objDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0 ? 1 : objDTO.ItemCostUOMValue.GetValueOrDefault(1))),
                        new SqlParameter("@IsOrdered", objDTO.IsOrdered),
                        new SqlParameter("@POItemLineNumber", objDTO.POItemLineNumber ?? (object)DBNull.Value),

                };

                string strCommand = " EXEC [UpdateQuoteDetailData] @DetailID,@DetailGUID,@QuoteGUID,@ItemGUID,@Bin,@RequestedQuantity,@ApprovedQuantity,@RequiredDate,@OrderedQuantity,@LastUpdatedBy,@Room,@CompanyID,@IsEDISENT,@InTransitQuantity,@ASNNumber,@ReceivedOn,@EditedFrom,@Comment,@LastEDIDate,@IsCloseItem,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@QuoteLineItemExtendedCost,@QuoteLineItemExtendedPrice,@RequestedQuantityUOM,@ApprovedQuantityUOM,@OrderedQuantityUOM,@InTransitQuantityUOM,@ItemCost,@SupplierId,@SupplierPartNo,@ItemSellPrice,@ItemMarkup,@ItemCostUOMValue,@IsOrdered,@POItemLineNumber";

                context.Database.CommandTimeout= 7200;
                context.Database.ExecuteSqlCommand(strCommand, params1);
                return true;
            }
        }

        public List<QuoteDetailDTO> GetDeletedOrUnDeletedQuoteDetailByQuoteGUIDPlain(Guid QuoteGUID, long RoomID, long CompanyID, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetDeletedOrUnDeletedQuoteDetailByQuoteGUIDPlain] @IsDeleted,@QuoteGUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@QuoteGUID", QuoteGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<QuoteDetailDTO>(stryQry, params1).ToList();

            }
        }
        public List<QuoteDetailDTO> GetQuoteDetailByQuoteGUIDFull(Guid QuoteGUID, long RoomID, long CompanyID, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetQuoteDetailByQuoteGUIDFull] @QuoteGUID,@RoomID,@CompanyID,@IsDeleted";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@QuoteGUID", QuoteGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@IsDeleted",IsDeleted )
                };
                return context.Database.SqlQuery<QuoteDetailDTO>(stryQry, params1).ToList();

            }
        }

        public List<QuoteDetailDTO> GetQuoteDetailByQuoteGUIDFullWithSupFltr(Guid QuoteGUID, long RoomID, long CompanyID, bool IsDeleted, List<long> SupplierIds)
        {
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetQuoteDetailByQuoteGUIDFullWithSupFltr] @QuoteGUID,@RoomID,@CompanyID,@IsDeleted,@SupplierIds";
                
                var params1 = new SqlParameter[] {
                    new SqlParameter("@QuoteGUID", QuoteGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@IsDeleted",IsDeleted ),
                    new SqlParameter("@SupplierIds",strSupplierIds ?? string.Empty)
                };
                return context.Database.SqlQuery<QuoteDetailDTO>(stryQry, params1).ToList();

            }
        }

        public DataTable GetQuoteDetailTableFromList(List<QuoteDetailDTO> lstQuoteDetails)
        {
            DataTable ReturnDT = new DataTable("OrderDetailWithCost");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]
                {
                    new DataColumn() { AllowDBNull=true,ColumnName="QuoteGuid",DataType=typeof(Guid)},
                    new DataColumn() { AllowDBNull=true,ColumnName="QuoteDetailGuid",DataType=typeof(Guid)},
                    new DataColumn() { AllowDBNull=true,ColumnName="ItemGuid",DataType=typeof(Guid)},
                    new DataColumn() { AllowDBNull=true,ColumnName="QuoteDetailItemCost",DataType=typeof(double)}
                };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstQuoteDetails != null && lstQuoteDetails.Count > 0)
                {
                    foreach (var item in lstQuoteDetails)
                    {
                        DataRow row = ReturnDT.NewRow();
                        row["QuoteGUID"] = item.QuoteGUID;
                        row["QuoteDetailGuid"] = item.GUID;
                        row["ItemGuid"] = item.ItemGUID;
                        //row["OrderDetailItemCost"] = item.ItemCost.HasValue ? item.ItemCost.Value : DBNull.Value;
                        row["QuoteDetailItemCost"] = item.ItemCost ?? 0;

                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }
        public IEnumerable<QuoteLineItemDetailDTO> GetQuoteDetailExport(Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<QuoteLineItemDetailDTO>("exec [GetQuoteDetailExport] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<QuoteDetailDTO> GetQuoteDetailByRoomPlain(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetQuoteDetailByRoomPlain] @RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<QuoteDetailDTO>(stryQry, params1).ToList();

            }
        }

        public QuoteDetailDTO GetQuoteDetailbyIDPlain(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<QuoteDetailDTO>("exec [GetQuoteDetailbyIDPlain] @Id", params1).FirstOrDefault();
            }
        }

        public bool DeleteQuoteDetail(string IDs, long UserId, long RoomId, long CompanyID,long EnterpriseId)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@IDs", IDs),
                    new SqlParameter("@UserID", UserId),
                    new SqlParameter("@RoomID", RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                     new SqlParameter("@EditedFrom", "Web")
                };

                    string strCommand = "EXEC DeleteQuoteDetail @IDs,@UserID,@RoomID,@CompanyID,@EditedFrom";

                    int intReturn = context.Database.SqlQuery<int>(strCommand, params1).FirstOrDefault();
                    ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);

                    foreach (var item in strArrIDs)
                    {
                        long Id = 0;
                        long.TryParse(item, out Id);
                        var quoteDetail = GetQuoteDetailbyIDPlain(long.Parse(item));

                        if (quoteDetail != null && quoteDetail.ID > 0)
                        {
                            ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, quoteDetail.ItemGUID);
                            objItemDTO.LastUpdatedBy = UserId;
                            objItemDTO.IsOnlyFromItemUI = false;
                            itmDAL.Edit(objItemDTO, UserId, EnterpriseId);
                        }
                    }

                    if (intReturn > 0)
                        return true;
                }
            }

            return false;
        }

        public bool UpdateQuoteDetailCommentAndUDF(QuoteDetailDTO objDTO, long UserID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new List<SqlParameter>() {
                new SqlParameter("@ID",objDTO.ID),
                new SqlParameter("@GUID",objDTO.GUID),
                new SqlParameter("@Comment",objDTO.Comment.ToDBNull()),
                new SqlParameter("@UDF1",objDTO.UDF1.ToDBNull()),
                new SqlParameter("@UDF2",objDTO.UDF2.ToDBNull()),
                new SqlParameter("@UDF3",objDTO.UDF3.ToDBNull()),
                new SqlParameter("@UDF4",objDTO.UDF4.ToDBNull()),
                new SqlParameter("@UDF5",objDTO.UDF5.ToDBNull()),
                new SqlParameter("@LastUpdatedBy",UserID),
                new SqlParameter("@LastUpdated", DateTimeUtility.DateTimeNow),
                new SqlParameter("@EditedFrom","Web"),
                new SqlParameter("@RoomId",RoomId),
                new SqlParameter("@CompanyID",CompanyId),
                };

                string strCommand = " EXEC [UpdateQuoteDetailCommentAndUDF] @ID, @GUID ,@Comment, @UDF1,@UDF2 , @UDF3 , @UDF4 , @UDF5 , @LastUpdatedBy ,@LastUpdated ,@EditedFrom,@RoomId,@CompanyID ";
                context.Database.SqlQuery<OrderDetailsDTO>(strCommand, params1.ToArray()).FirstOrDefault();

                return true;
            }
        }

        public List<Guid> GetItemGuidsByQuoteGuid(Guid QuoteGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@QuoteGUID", QuoteGUID) };
                return context.Database.SqlQuery<Guid>("exec [GetItemGuidsByQuoteGuid] @QuoteGUID", params1).ToList();
            }
        }

        public bool CloseQuoteDetailItem(string IDs, long UserId, long RoomId, long CompanyID, long EnterpriseId)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@IDs", IDs),
                    new SqlParameter("@UserID", UserId),
                    new SqlParameter("@RoomID", RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@EditedFrom", "Web"),
                    new SqlParameter("@IsCloseItem", "1")
                };

                    string strCommand = "EXEC CloseQuoteDetailItem @IDs,@UserID,@RoomID,@CompanyID,@EditedFrom,@IsCloseItem";

                    int intReturn = context.Database.SqlQuery<int>(strCommand, params1).FirstOrDefault();
                    string[] arrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrIDs != null && arrIDs.Length > 0)
                    {
                        ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
                        
                        foreach (var item in arrIDs)
                        {
                            long quoteDetailId = 0;
                            long.TryParse(item,out quoteDetailId);

                            var detailDTO = GetQuoteDetailbyIDPlain(quoteDetailId);
                            
                            if (detailDTO != null && detailDTO.ID > 0)
                            {
                                try
                                {
                                    ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, detailDTO.ItemGUID);
                                    objItemDTO.LastUpdatedBy = UserId;
                                    objItemDTO.IsOnlyFromItemUI = false;
                                    itmDAL.Edit(objItemDTO, UserId,EnterpriseId);
                                    UpdateQuoteStatusOnLineItemClose(detailDTO.QuoteGUID, RoomId, CompanyID, UserId, false);
                                }
                                catch (Exception)
                                {

                                }
                            }                          
                        }
                    }

                    if (intReturn > 0)
                        return true;

                }
            }
            return false;
        }

        public bool UpdateQuoteStatusOnLineItemClose(Guid QuoteGuid, long RoomID, long CompanyID, long UserID, bool IsOnlyFromUI)
        {
            var quoteMasterDAL = new QuoteMasterDAL(base.DataBaseName);
            var quote = quoteMasterDAL.GetQuoteByGuidPlain(QuoteGuid);

            if (quote.QuoteStatus > (int)QuoteStatus.Approved && quote.QuoteStatus < (int)QuoteStatus.Closed)
            {
                var lstQuoteDetailRecords = GetQuoteDetailByQuoteGUIDFull(QuoteGuid, RoomID, CompanyID,false);
                
                if (lstQuoteDetailRecords != null && lstQuoteDetailRecords.Any() && lstQuoteDetailRecords.Count > 0)
                {
                    var canBeClosed = (lstQuoteDetailRecords.Where(e=> !e.IsCloseItem.GetValueOrDefault(false) && !e.IsOrdered).Count() < 1);
                    
                    if(canBeClosed)
                    {
                        if (quote.QuoteStatus != (int)QuoteStatus.Closed)
                        {
                            quote.QuoteStatus = (int)QuoteStatus.Closed;
                            quote.LastUpdated = DateTimeUtility.DateTimeNow;
                            quote.LastUpdatedBy = UserID;

                            if (IsOnlyFromUI)
                            {
                                quote.EditedFrom = "Web";
                                quote.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            
                            quoteMasterDAL.UpdateQuoteMaster(quote);
                        }

                    }
                }
            }

            return true;
        }

        public bool UpdateReqDateandCommentToQuoteLineItems(QuoteDetailDTO objDTO, long UserID, bool isCommentUpdate, bool isReqDateUpdate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                QuoteDetail objODNew = null;

                if (objDTO.ID > 0)
                {
                    objODNew = context.QuoteDetails.FirstOrDefault(x => x.ID == objDTO.ID);
                }
                else
                {
                    objODNew = context.QuoteDetails.FirstOrDefault(x => x.GUID == objDTO.GUID);
                }
                
                if (objODNew != null)
                {

                    if (isReqDateUpdate)
                        objODNew.RequiredDate = objDTO.RequiredDate;
                    if (isCommentUpdate)
                        objODNew.Comment = objDTO.Comment;
                    objODNew.LastUpdatedBy = UserID;
                    objODNew.LastUpdated = DateTimeUtility.DateTimeNow;
                    objODNew.EditedFrom = "Web";
                    context.SaveChanges();
                }
                return true;
            }
        }

        #endregion
    }
}
