using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;
using System.Globalization;


namespace eTurns.DAL
{
    public class QuoteMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public QuoteMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        #endregion

        #region [Public Methods]

        public QuoteMasterDTO GetQuoteByIdPlain(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByIdPlain] @Id", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO GetQuoteByIdNormal(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByIdNormal] @Id", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO GetQuoteByIdFull(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByIdFull] @Id", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO GetQuoteByGuidPlain(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByGuidPlain] @Guid", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO GetQuoteByGuidNormal(Guid Guid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Guid", Guid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByGuidNormal] @Guid", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO GetQuoteByGuidFull(Guid Guid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Guid", Guid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByGuidFull] @Guid", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO GetQuoteByQuoteNumberPlain(string QuoteNumber, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@QuoteNumber", QuoteNumber ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByQuoteNumberPlain] @QuoteNumber,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO GetQuoteByQuoteNumberNormal(string QuoteNumber, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@QuoteNumber", QuoteNumber ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuoteByQuoteNumberNormal] @QuoteNumber,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public QuoteMasterDTO InsertQuoteMaster(QuoteMasterDTO quoteMasterDTO)
        {
           
            var  QuoteSuppliers = quoteMasterDTO.QuoteSupplierIdsCSV.Split(',');
            QuoteSuppliers = QuoteSuppliers.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            
            var params1 = new SqlParameter[] { new SqlParameter("@QuoteNumber", quoteMasterDTO.QuoteNumber ?? (object)DBNull.Value),
                new SqlParameter("@ReleaseNumber", quoteMasterDTO.ReleaseNumber ?? (object)DBNull.Value),
                new SqlParameter("@CreatedBy", quoteMasterDTO.CreatedBy),
                new SqlParameter("@Room", quoteMasterDTO.Room),
                new SqlParameter("@CompanyID", quoteMasterDTO.CompanyID),
                new SqlParameter("@QuoteStatus", quoteMasterDTO.QuoteStatus),
                new SqlParameter("@RequiredDate", quoteMasterDTO.RequiredDate.ToString("yyyy-MM-dd")),
                new SqlParameter("@QuoteDate", quoteMasterDTO.QuoteDate ?? (object)DBNull.Value),
                new SqlParameter("@Comment", quoteMasterDTO.Comment ?? (object)DBNull.Value),
                new SqlParameter("@UDF1", quoteMasterDTO.UDF1 ?? (object)DBNull.Value), 
                new SqlParameter("@UDF2", quoteMasterDTO.UDF2 ?? (object)DBNull.Value),
                new SqlParameter("@UDF3", quoteMasterDTO.UDF3 ?? (object)DBNull.Value),
                new SqlParameter("@UDF4", quoteMasterDTO.UDF4 ?? (object)DBNull.Value),
                new SqlParameter("@UDF5", quoteMasterDTO.UDF5 ?? (object)DBNull.Value),
                new SqlParameter("@WhatWhereAction", quoteMasterDTO.WhatWhereAction ?? (object)DBNull.Value),
                new SqlParameter("@QuoteNumber_ForSorting", quoteMasterDTO.QuoteNumber_ForSorting ?? (object)DBNull.Value),
                new SqlParameter("@ReceivedOn", quoteMasterDTO.ReceivedOn),
                new SqlParameter("@ReceivedOnWeb", quoteMasterDTO.ReceivedOnWeb),
                new SqlParameter("@AddedFrom", quoteMasterDTO.AddedFrom ?? (object)DBNull.Value),
                new SqlParameter("@EditedFrom", quoteMasterDTO.EditedFrom ?? (object)DBNull.Value),
                new SqlParameter("@IsEDIQuote", quoteMasterDTO.IsEDIQuote ?? (object)DBNull.Value),
                new SqlParameter("@QuoteGuid", quoteMasterDTO.GUID),
                new SqlParameter("@RequesterID", quoteMasterDTO.RequesterID ?? (object)DBNull.Value),
                new SqlParameter("@ApproverID", quoteMasterDTO.ApproverID ?? (object)DBNull.Value),
                new SqlParameter("@QuoteSupplierIdsCSV", quoteMasterDTO.QuoteSupplierIdsCSV ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                quoteMasterDTO = context.Database.SqlQuery<QuoteMasterDTO>("exec [InsertQuoteMaster] @QuoteNumber,@ReleaseNumber,@CreatedBy,@Room,@CompanyID,@QuoteStatus,@RequiredDate,@QuoteDate,@Comment,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@WhatWhereAction,@QuoteNumber_ForSorting,@ReceivedOn,@ReceivedOnWeb,@AddedFrom,@EditedFrom,@IsEDIQuote,@QuoteGuid,@RequesterID,@ApproverID,@QuoteSupplierIdsCSV", params1).FirstOrDefault();
            }

            if (quoteMasterDTO != null && quoteMasterDTO.ID > 0)
            {
                var autoSequenceDAL = new AutoSequenceDAL(base.DataBaseName);
                foreach (var quotesupp in QuoteSuppliers)
                {
                    autoSequenceDAL.UpdateNextQuoteNumber(quoteMasterDTO.Room, quoteMasterDTO.CompanyID, quoteMasterDTO.QuoteNumber, Convert.ToInt64(quotesupp), quoteMasterDTO.CreatedBy, quoteMasterDTO.ReleaseNumber);
                }
                
            }

            return quoteMasterDTO;
        }
        public QuoteMasterDTO UpdateQuoteMaster(QuoteMasterDTO objQuoteMasterDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", objQuoteMasterDTO.ID),
                new SqlParameter("@GUID", objQuoteMasterDTO.GUID),
                new SqlParameter("@LastUpdatedBy", objQuoteMasterDTO.LastUpdatedBy),
                new SqlParameter("@Room", objQuoteMasterDTO.Room),
                new SqlParameter("@CompanyID", objQuoteMasterDTO.CompanyID),
                new SqlParameter("@QuoteStatus", objQuoteMasterDTO.QuoteStatus),
                new SqlParameter("@RequiredDate", objQuoteMasterDTO.RequiredDate.ToString("yyyy-MM-dd")), 
                new SqlParameter("@QuoteDate", objQuoteMasterDTO.QuoteDate ?? (object)DBNull.Value),
                new SqlParameter("@Comment", objQuoteMasterDTO.Comment ?? (object)DBNull.Value),
                new SqlParameter("@UDF1", objQuoteMasterDTO.UDF1 ?? (object)DBNull.Value), 
                new SqlParameter("@UDF2", objQuoteMasterDTO.UDF2 ?? (object)DBNull.Value),
                new SqlParameter("@UDF3", objQuoteMasterDTO.UDF3 ?? (object)DBNull.Value),
                new SqlParameter("@UDF4", objQuoteMasterDTO.UDF4 ?? (object)DBNull.Value),
                new SqlParameter("@UDF5", objQuoteMasterDTO.UDF5 ?? (object)DBNull.Value),
                new SqlParameter("@QuoteCost", objQuoteMasterDTO.QuoteCost ?? (object)DBNull.Value),
                new SqlParameter("@NoOfLineItems", objQuoteMasterDTO.NoOfLineItems ?? (object)DBNull.Value),
                new SqlParameter("@ChangeQuoteRevisionNo", objQuoteMasterDTO.ChangeQuoteRevisionNo ?? (object)DBNull.Value),
                new SqlParameter("@RejectionReason", objQuoteMasterDTO.RejectionReason ?? (object)DBNull.Value),
                new SqlParameter("@ReleaseNumber", objQuoteMasterDTO.ReleaseNumber ?? (object)DBNull.Value),
                new SqlParameter("@QuoteNumber", objQuoteMasterDTO.QuoteNumber ?? (object)DBNull.Value),
                new SqlParameter("@WhatWhereAction", objQuoteMasterDTO.WhatWhereAction ?? (object)DBNull.Value),
                new SqlParameter("@ReceivedOn", objQuoteMasterDTO.ReceivedOn), 
                new SqlParameter("@EditedFrom", objQuoteMasterDTO.EditedFrom ?? (object)DBNull.Value),
                new SqlParameter("@IsEDIQuote", objQuoteMasterDTO.IsEDIQuote ?? (object)DBNull.Value),
                new SqlParameter("@QuotePrice", objQuoteMasterDTO.QuotePrice ?? (object)DBNull.Value),
                new SqlParameter("@RequesterID", objQuoteMasterDTO.RequesterID ?? (object)DBNull.Value),
                new SqlParameter("@ApproverID", objQuoteMasterDTO.ApproverID ?? (object)DBNull.Value),
                new SqlParameter("@QuoteSupplierIdsCSV", objQuoteMasterDTO.QuoteSupplierIdsCSV ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [UpdateQuoteMaster] @ID,@GUID,@LastUpdatedBy,@Room,@CompanyID,@QuoteStatus,@RequiredDate,@QuoteDate,@Comment,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@QuoteCost,@NoOfLineItems,@ChangeQuoteRevisionNo,@RejectionReason,@ReleaseNumber,@QuoteNumber,@WhatWhereAction,@ReceivedOn,@EditedFrom,@IsEDIQuote,@QuotePrice,@RequesterID,@ApproverID,@QuoteSupplierIdsCSV", params1).FirstOrDefault();
            }
        }
        public List<QuoteMasterDTO> GetQuotesByRoomPlain(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuotesByRoomPlain] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<QuoteMasterDTO> GetQuotesByRoomNormal(long? RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetQuotesByRoomNormal] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public bool CheckDuplicateQuoteNumberbyRoomID(Int64 RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int32 duplicatecount = context.Database.SqlQuery<Int32>("exec [CheckDuplicateQuoteNumberbyRoomID] @RoomID", params1).FirstOrDefault();
                if (duplicatecount > 0)
                { return true; }
                return false;
            }
        }
        public List<QuoteMasterDTO> GetApprovedQuotesList(long RoomID, long CompanyID, List<long> SupplierIds)
        {
            string strSupplierIds = string.Empty;
            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID)
                            , new SqlParameter("@CompanyID", CompanyID)
                            , new SqlParameter("@SupplierIds", strSupplierIds)
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetApprovedQuotesList] @RoomID,@CompanyID,@SupplierIds", params1).ToList();
            }
        }
        public IEnumerable<NarrowSearchDTO> GetQuoteMasterNarrowSearchData(string TextFieldName, string CurrentTab, long RoomID, long CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string quoteStatus = string.Empty;

                if (!string.IsNullOrEmpty(CurrentTab.ToLower()) && !string.IsNullOrWhiteSpace(CurrentTab.ToLower()))
                {
                    switch (CurrentTab.ToLower())
                    {
                        case "unsubmitted":
                            quoteStatus = ((int)QuoteStatus.UnSubmitted).ToString();
                            break;
                        case "submitted":
                            quoteStatus = ((int)QuoteStatus.Submitted).ToString();
                            break;
                        case "changequote":
                            quoteStatus = (((int)QuoteStatus.Transmitted).ToString() + "," + ((int)QuoteStatus.TransmittedPastDue).ToString() + "," + ((int)QuoteStatus.TransmittedInCompletePastDue).ToString());
                            break;
                        default:
                            quoteStatus = "";
                            break;
                    }
                }

                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@RoomId", RoomID),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@IsArchived", IsArchived),
                                                    new SqlParameter("@Isdeleted", IsDeleted),
                                                    new SqlParameter("@NarrowSearchKey", TextFieldName),
                                                    new SqlParameter("@StatusQry", quoteStatus),                                                    
                                                    //new SqlParameter("@LoadDataCount", 0) 
                                                  };
                var result = context.Database.SqlQuery<NarrowSearchDTO>("exec [GetQuoteMasterNarrowSearchData] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@StatusQry", params1).ToList();

                if (TextFieldName == "QuoteStatus" && result != null && result.Any())
                {
                    result.ForEach(x => x.NSColumnText = ResQuoteMaster.GetQuoteStatusText(Enum.Parse(typeof(OrderStatus), x.NSColumnText, true).ToString()));
                }

                return result;
            }
        }

        public List<QuoteMasterDTO> GetQuoteMasterPagedData(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName,
                                        long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string spName = "GetQuoteMasterPagedData";//IsArchived ? "GetPagedRequisitions_Archive" : "GetPagedRequisitions";

            List<QuoteMasterDTO> lstQuote = new List<QuoteMasterDTO>();
            TotalCount = 0;
            DataSet dsQuote = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstQuote;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string Creaters = null;
            string Updators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string QuoteStatusIn = string.Empty;
            string quoteSupplier = null;

            if (MaxRows < 1)
            {
                MaxRows = 10;
            }

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            if (string.IsNullOrEmpty(SearchTerm))
            {
                dsQuote = SqlHelper.ExecuteDataset(EturnsConnection, spName, CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, Creaters, Updators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, QuoteStatusIn, quoteSupplier, 0);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (!string.IsNullOrEmpty(FieldsPara[27]))
                {
                    QuoteStatusIn = FieldsPara[27];
                    if (!string.IsNullOrEmpty(QuoteStatusIn) && !string.IsNullOrWhiteSpace(QuoteStatusIn))
                    {
                        QuoteStatusIn = GetQuoteStatusValueFromText(QuoteStatusIn);
                    }
                }
                if (!string.IsNullOrEmpty(FieldsPara[25]))
                {
                    QuoteStatusIn = FieldsPara[25];
                    if (!string.IsNullOrEmpty(QuoteStatusIn) && !string.IsNullOrWhiteSpace(QuoteStatusIn))
                    {
                        QuoteStatusIn = GetQuoteStatusValueFromText(QuoteStatusIn);
                    }
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    Creaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    Updators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");// Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss"); //Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss"); //Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");// Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrEmpty(FieldsPara[125]))
                {
                    QuoteStatusIn = FieldsPara[125].TrimEnd(',');
                }

                if (!string.IsNullOrEmpty(FieldsPara[126]) && !string.IsNullOrWhiteSpace(FieldsPara[126]))
                {
                    quoteSupplier = FieldsPara[126].TrimEnd(',');
                }

                dsQuote = SqlHelper.ExecuteDataset(EturnsConnection, spName, CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, Creaters, Updators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, QuoteStatusIn, quoteSupplier, 0);
            }
            else
            {
                if (SearchTerm.Contains("[####]"))
                {
                    QuoteStatusIn = SearchTerm.Split(new string[] { "[####]" }, StringSplitOptions.None)[1];
                    SearchTerm = SearchTerm.Split(new string[] { "[####]" }, StringSplitOptions.None)[0];
                    QuoteStatusIn = QuoteStatusIn.ToLower().Replace("all", "");
                    //QuoteStatusIn = QuoteStatusIn.ToLower().Replace("all", "").Replace("changequote", (((int)QuoteStatus.Transmitted).ToString() + "," + ((int)QuoteStatus.TransmittedPastDue).ToString() + "," + ((int)QuoteStatus.TransmittedInCompletePastDue).ToString()));
                }

                if (!string.IsNullOrEmpty(QuoteStatusIn) && !string.IsNullOrWhiteSpace(QuoteStatusIn))
                {
                    QuoteStatusIn = GetQuoteStatusValueFromText(QuoteStatusIn);
                }

                dsQuote = SqlHelper.ExecuteDataset(EturnsConnection, spName, CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, Creaters, Updators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, QuoteStatusIn, quoteSupplier, 0);
            }

            if (dsQuote != null && dsQuote.Tables.Count > 0)
            {
                lstQuote = DataTableHelper.ToList<QuoteMasterDTO>(dsQuote.Tables[0]);
                TotalCount = 0;

                if (lstQuote != null && lstQuote.Count() > 0)
                {
                    TotalCount = lstQuote.ElementAt(0).TotalRecords;
                }
            }

            return lstQuote;
        }

        private string GetQuoteStatusValueFromText(string QuoteStatusText)
        {
            if (!string.IsNullOrEmpty(QuoteStatusText) && !string.IsNullOrWhiteSpace(QuoteStatusText))
            {
                switch (QuoteStatusText.ToLower())
                {
                    case "unsubmitted":
                        return ((int)QuoteStatus.UnSubmitted).ToString();
                    case "submitted":
                        return ((int)QuoteStatus.Submitted).ToString();
                    case "approved":
                        return ((int)QuoteStatus.Approved).ToString();
                    case "transmitted":
                        return ((int)QuoteStatus.Transmitted).ToString();
                    case "transmittedincomplete":
                        return ((int)QuoteStatus.TransmittedIncomplete).ToString();
                    case "transmittedpastdue":
                        return ((int)QuoteStatus.TransmittedPastDue).ToString();
                    case "transmittedincompletepastdue":
                        return ((int)QuoteStatus.TransmittedInCompletePastDue).ToString();
                    case "closed":
                        return ((int)QuoteStatus.Closed).ToString();
                    case "changequote":
                        return (((int)QuoteStatus.Transmitted).ToString() + "," + ((int)QuoteStatus.TransmittedPastDue).ToString() + "," + ((int)QuoteStatus.TransmittedInCompletePastDue).ToString());
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }
        public int GetCountOfQuoteByQuoteNumber(long RoomId, long CompanyId, string QuoteNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@QuoteNumber", QuoteNumber) };
                return context.Database.SqlQuery<int>("exec [GetCountOfQuoteByQuoteNumber] @RoomID,@CompanyID,@QuoteNumber", params1).FirstOrDefault();
            }
        }

        public IEnumerable<QuoteMasterDTO> GetPagedQuoteMasterChangeLog(long QuoteId, int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName)
        {
            TotalCount = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Id", QuoteId),
                                                    new SqlParameter("@StartRowIndex", StartRowIndex),
                                                    new SqlParameter("@MaxRows", MaxRows),
                                                    new SqlParameter("@SearchTerm", SearchTerm ?? string.Empty),
                                                    new SqlParameter("@sortColumnName", sortColumnName ?? string.Empty)
                                                  };
                var quoteHistory = context.Database.SqlQuery<QuoteMasterDTO>("exec [GetPagedQuoteMasterChangeLog] @Id,@StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName", params1).ToList();

                if (quoteHistory != null && quoteHistory.Any() && quoteHistory.Count() > 0)
                {
                    TotalCount = quoteHistory.ElementAt(0).TotalRecords;
                }

                return quoteHistory;
            }
        }

        #endregion

        public void UpdateQuoteStatus(Guid? QuoteGUID, int QuoteStatus, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@QuoteGUID", QuoteGUID ?? (object)DBNull.Value),
                new SqlParameter("@QuoteStatus", QuoteStatus),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@CompanyID", CompanyID)
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [UpdateQuoteStatus] @QuoteGUID,@QuoteStatus,@RoomID,@CompanyID", params1);
            }
        }

        public bool IsQuoteNumberDuplicateById(string QuoteNumber, long QuoteID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@QuoteID", QuoteID),
                                new SqlParameter("@QuoteNumber", QuoteNumber ?? (object)DBNull.Value),
                                new SqlParameter("@RoomID", RoomID),
                                new SqlParameter("@CompanyID", CompanyID)
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int32 QuoteCount = context.Database.SqlQuery<Int32>("exec [IsQuoteNumberDuplicateById] @QuoteID,@QuoteNumber,@RoomID,@CompanyID", params1).FirstOrDefault();
                return (QuoteCount > 0);
            }
        }

        public int GetNextQuoteReleaseNumber(string QuoteNumber, Guid? QuoteGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@QuoteNumber", QuoteNumber ?? (object)DBNull.Value),
                                                new SqlParameter("@QuoteGUID", QuoteGUID ?? (object)DBNull.Value),
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("EXEC [dbo].[csp_GenerateAndGetQuoteReleaseNumber] @QuoteNumber,@QuoteGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public QuoteMasterDTO GetLatestQuoteByRoomIdPlain(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId)};

                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetLatestQuoteByRoomIdPlain] @RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public int GetMaximumReleaseNoByQuoteNumber(long RoomId, long CompanyId, string QuoteNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@QuoteNumber", QuoteNumber)
                                                };
                var maximumReleaseNo = context.Database.SqlQuery<int?>("exec [GetMaximumReleaseNoByQuoteNumber] @RoomID,@CompanyID,@QuoteNumber", params1).FirstOrDefault();
                return maximumReleaseNo ?? 0;
            }
        }
        public void Qut_UpdateItemCostBasedonQuoteDetailCost(long UserID, string EditedFrom, long RoomID, long CompanyID, DataTable DT)
        {
            try
            {
                SqlConnection ChildDbConnection = new SqlConnection(base.DataBaseConnectionString);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "Qut_UpdateItemCostBasedonQuoteDetailCost", UserID, EditedFrom, RoomID, CompanyID, DT);
            }
            catch
            {

            }
        }

        public long TransmitQuote(QuoteMasterDTO Quote)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@QuoteGUID", Quote.GUID),
                                                   new SqlParameter("@RoomID", Quote.Room),
                                                   new SqlParameter("@CompanyID", Quote.CompanyID),
                                                   new SqlParameter("@EditedFrom", Quote.EditedFrom),
                                                   new SqlParameter("@WhatWhereAction", Quote.WhatWhereAction)
                                                 };
                return context.Database.SqlQuery<long>("EXEC [TransmitQuote] @QuoteGUID,@RoomID,@CompanyID,@EditedFrom,@WhatWhereAction", params1).FirstOrDefault();
            }
        }

        public void UpdateQuoteComment(string comment, long QuoteId, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var quote = context.QuoteMasters.FirstOrDefault(x => x.ID == QuoteId);
                quote.Comment = comment;
                //obj.ShippingTrackNumber = ShipmentTrackingNumber;
                //obj.PackSlipNumber = PackslipNumber;
                quote.LastUpdatedBy = UserID;
                quote.LastUpdated = DateTimeUtility.DateTimeNow;
                quote.ReceivedOn = DateTimeUtility.DateTimeNow;
                quote.EditedFrom = "Web";
                context.SaveChanges();
            }
        }

        public IEnumerable<QuoteMasterDTO> GetUnapprovedQuotesByIdsPlain(string Ids)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Ids", Ids) };
                return context.Database.SqlQuery<QuoteMasterDTO>("exec [GetUnapprovedQuotesByIdsPlain] @Ids", params1).ToList();
            }
        }



        public bool DeleteQuoteByIds(string IDs, long UserId, long RoomID, long CompanyID,long EnterpriseId)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@IDs", strIDs),
                                                       new SqlParameter("@UserID", UserId),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                       new SqlParameter("@ReceivedOn", DateTimeUtility.DateTimeNow),
                                                       new SqlParameter("@EditedFrom", "Web"),
                                                     };
                    int intReturn = context.Database.SqlQuery<int>("exec [DeleteQuoteByIds] @IDs,@UserID,@RoomID,@CompanyID,@ReceivedOn,@EditedFrom", params1).FirstOrDefault(); ;
                    ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
                    var quotes = GetUnapprovedQuotesByIdsPlain(strIDs);

                    var quoteDetailDAL = new QuoteDetailDAL(base.DataBaseName);

                    foreach (var item in quotes)
                    {
                        var itemGuids = quoteDetailDAL.GetItemGuidsByQuoteGuid(item.GUID);

                        foreach (var itemGuid in itemGuids)
                        {
                            ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, itemGuid);
                            objItemDTO.LastUpdatedBy = item.LastUpdatedBy;
                            objItemDTO.IsOnlyFromItemUI = false;
                            itmDAL.Edit(objItemDTO, UserId,EnterpriseId);
                        }
                    }

                    if (intReturn > 0)
                        return true;
                }
            }
            return false;

        }

        #region Quote to Order
        public List<OrderMasterDTO> CreateOrdersByQuote(Guid QuoteGuid, List<OrderMasterDTO> lstOrders, long RoomId, long CompanyId, long UserId, string EpDatabaseName, short SubmissionMethod, long EnterpriseId, out Dictionary<string, string> rejectedOrderLineItems, long SessionUserId, string callingFrom = "")
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
                string columnList = "ID,RoomName,PreventMaxOrderQty,AllowABIntegration";
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
                    objOrderMasterDTO.CustomerID = null;
                    CustomerMasterDTO objCustomerMasterDTO = objCommonDAL.GetOrInsertCustomerGUIDByName(objOrderMasterDTO.CustomerName, UserId, RoomId, CompanyId);
                    if (objCustomerMasterDTO != null)
                    {
                        objOrderMasterDTO.CustomerGUID = objCustomerMasterDTO.GUID;
                    }
                    objOrderMasterDTO.ShippingVendor = objCommonDAL.GetOrInsertVendorIDByName(objOrderMasterDTO.ShippingVendorName, UserId, RoomId, CompanyId);
                    objOrderMasterDTO.GUID = Guid.NewGuid();
                    objOrderMasterDTO.WhatWhereAction = "QuoteToOrder";
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
                        List<QuoteDetailDTO> lstQuoteItems = objCartItemDAL.GetQuoteItemsByGuids(QuoteGuid, lstids, RoomId, CompanyId, true);
                        string[] AllGuid = objOrderMasterDTO.OrderLineItemsIds.Split(',').ToArray();
                        string[] udf1;
                        string[] udf2;
                        string[] udf3;
                        string[] udf4;
                        string[] udf5;
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

                        long temp_CreatedBy = UserId;
                        long temp_LastUpdatedBy = UserId;

                        foreach (QuoteDetailDTO quoteitem in lstQuoteItems)
                        {
                            if ((callingFrom ?? string.Empty).ToLower() == "service")
                            {
                                if (quoteitem.CreatedBy.GetValueOrDefault(0) > 0)
                                {
                                    temp_CreatedBy = quoteitem.CreatedBy.GetValueOrDefault(0);
                                }

                                if (quoteitem.LastUpdatedBy > 0)
                                {
                                    temp_LastUpdatedBy = quoteitem.LastUpdatedBy;
                                }
                            }

                            int Index = Array.FindIndex(AllGuid, row => row.Contains(quoteitem.GUID.ToString()));

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

                            objOrderDetailsDTO.ItemGUID = quoteitem.ItemGUID;
                            objOrderDetailsDTO.Bin = quoteitem.BinID;
                            objOrderDetailsDTO.RequestedQuantity = quoteitem.RequestedQuantity.GetValueOrDefault(0);
                            if (SubmissionMethod == 2 || ActualOrderStatus > 2)
                            {
                                objOrderDetailsDTO.ApprovedQuantity = quoteitem.RequestedQuantity.GetValueOrDefault(0);
                            }
                            objOrderDetailsDTO.RequiredDate = quoteitem.LeadTimeInDays.GetValueOrDefault(0) > 0 ? CurrentDateTime.AddDays(quoteitem.LeadTimeInDays.GetValueOrDefault(0)) : objReturnAfterSave.RequiredDate;
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
                            objOrderDetailsDTO.POItemLineNumber = quoteitem.POItemLineNumber;
                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain(quoteitem.ItemGUID, RoomId, CompanyId);

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

                            if (objRoomDTO != null && objRoomDTO.AllowABIntegration)
                            {
                                if (objItemMasterDTO != null && !string.IsNullOrWhiteSpace(objItemMasterDTO.SupplierPartNo))
                                {
                                    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(EpDatabaseName);
                                    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objItemMasterDTO.SupplierPartNo, objItemMasterDTO.GUID, CompanyId, RoomId);
                                    if (ABItemMappingID > 0 && !objItemMasterDTO.IsOrderable)
                                    {
                                        if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                        {
                                            rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = string.Format("Item: {0} can not be add to order, item is not orderable.", objItemMasterDTO.ItemNumber);
                                        }
                                        rejectedDueToPreventMaxValidation++;
                                        unSuccessfulOrders.Add(objReturnAfterSave.GUID);
                                        rejectedOrderLineItemsGuids.Add(Convert.ToString(quoteitem.GUID));
                                        continue;
                                    }
                                }
                            }

                            if (objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder)
                            {
                                if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.Value)
                                {
                                    if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0) + objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0)) > objItemMasterDTO.MaximumQuantity.Value)
                                    {
                                        if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                        {
                                            rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = "Item Maximum Quantity reached.";
                                        }
                                        rejectedDueToPreventMaxValidation++;
                                        unSuccessfulOrders.Add(objReturnAfterSave.GUID);
                                        rejectedOrderLineItemsGuids.Add(Convert.ToString(quoteitem.GUID));
                                        continue;
                                    }
                                }
                                else
                                {
                                    List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemMasterDTO.GUID, RoomId, CompanyId).OrderBy(x => x.BinNumber).ToList();
                                    var maxQtyAtBinLevel = lstItemBins.Where(e => e.BinNumber.Equals(quoteitem.BinName)).FirstOrDefault();
                                    var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : quoteitem.BinID.GetValueOrDefault(0);
                                    var onOrderQtyAtBin = objOrderDetailsDAL.GetOrderdQtyOfItemBinWise(RoomId, CompanyId, objItemMasterDTO.GUID, tmpBinId);

                                    if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && (onOrderQtyAtBin + objOrderDetailsDTO.RequestedQuantity.GetValueOrDefault(0)) > maxQtyAtBinLevel.MaximumQuantity.Value)
                                    {
                                        if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                        {
                                            rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = "Bin Maximum Quantity reached.";
                                        }
                                        rejectedDueToPreventMaxValidation++;
                                        unSuccessfulOrders.Add(objReturnAfterSave.GUID);
                                        rejectedOrderLineItemsGuids.Add(Convert.ToString(quoteitem.GUID));
                                        continue;
                                    }
                                }
                            }

                            OrderDetailsDTO returnorderDetails = new OrderDetailsDTO();
                            returnorderDetails = objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId,EnterpriseId);
                            itemGuidsToUpdate.Add(quoteitem.ItemGUID);

                            // update Item onQuotedQuantity
                            if (objItemMasterDTO != null)
                            {
                                objItemMasterDTO.OnQuotedQuantity = (objItemMasterDTO.OnQuotedQuantity.GetValueOrDefault(0) - quoteitem.ApprovedQuantity.GetValueOrDefault(0));
                                objItemMasterDAL.EditDateAndOnQuotedQuantity(objItemMasterDTO.GUID, RoomId, CompanyId, objItemMasterDTO.OnQuotedQuantity.GetValueOrDefault(0));
                            }
                            // update QD IsOrdered = true
                            UpdateIsOrderedByQDGuid(QuoteGuid, quoteitem.GUID, true, RoomId, CompanyId);
                            // MaintainTransaction History for Quote To Order
                            if(returnorderDetails != null)
                            {
                                objCartItemDAL.InsertCartQuoteTransitionDetail(null, quoteitem.GUID, returnorderDetails.GUID, null, (int)TransactionConversionType.QuotetoOrder, UserId);
                            }

                            //WI-8417 JKP
                            if (ActualOrderStatus >= (int)OrderStatus.Approved)
                            {
                                try
                                {
                                    objOrderDetailsDAL.UpdateOrderUsedTotalValueBPO(quoteitem.ItemGUID, returnorderDetails.ApprovedQuantity.GetValueOrDefault(0), returnorderDetails.OrderLineItemExtendedCost.GetValueOrDefault(0), "order");
                                }
                                catch (Exception ex) { }
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
                        //if (!string.IsNullOrEmpty(objOrderMasterDTO.OrderLineItemsIds))
                        //{
                        //    objCartItemDAL.DeleteRecords(objOrderMasterDTO.OrderLineItemsIds, UserId, CompanyId, EnterpriseId, SessionUserId);
                        //}
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

        public void UpdateIsOrderedByQDGuid(Guid QuoteGUID, Guid QuoteDetailGUID, bool IsOrdered, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@QuoteGUID", QuoteGUID),
                new SqlParameter("@QuoteDetailGUID", QuoteDetailGUID),
                new SqlParameter("@IsOrdered", IsOrdered),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@CompanyID", CompanyID)
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [UpdateIsOrderedQuoteItemByGuid] @QuoteGUID,@QuoteDetailGUID,@IsOrdered,@RoomID,@CompanyID", params1);
            }
        }
        #endregion
    }
}
