using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DAL;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Cryptography;
using System.Configuration;
using System.IO;

namespace eTurns.DAL
{
    public partial class CommonDAL : eTurnsBaseDAL
    {
        public List<CommonDTO> GetTabStatusCount_Old(string TableName, string StatusName, Int64 CompanyID, Int64 RoomID, List<long> SupplierIds, string MainFilter = "false")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "SELECT " + StatusName + " as 'Text', COUNT(" + StatusName + ") 'Count'  from " + TableName + "  WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND IsDeleted = 0  AND IsNotification = 1 group by " + StatusName;

                if (TableName == "OrderMaster")
                {
                    if (!string.IsNullOrEmpty(MainFilter) && MainFilter.ToLower() == "true")
                    {
                        if (SupplierIds != null && SupplierIds.Any())
                        {
                            strQuery = "SELECT Convert(varchar(20)," + StatusName + ") as 'Text', COUNT(" + StatusName + ") 'Count'  from " + TableName + "  WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0   AND GUID IN (SELEct Orderguid from OrderDetails  WHERE ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0  AND ISNULL(ASNNumber,'')= '' ) AND OrderStatus  IN (1,4) and Supplier IN (" + string.Join(",", SupplierIds) + ")  group by " + StatusName;
                        }
                        else
                        {
                            strQuery = "SELECT Convert(varchar(20)," + StatusName + ") as 'Text', COUNT(" + StatusName + ") 'Count'  from " + TableName + "  WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0   AND GUID IN (SELEct Orderguid from OrderDetails  WHERE ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0  AND ISNULL(ASNNumber,'')= '' ) AND OrderStatus  IN (1,4)  group by " + StatusName;
                        }
                    }
                    else
                    {
                        if (SupplierIds != null && SupplierIds.Any())
                        {
                            strQuery = "SELECT Convert(varchar(20)," + StatusName + ") as 'Text', COUNT(" + StatusName + ") 'Count'  from " + TableName + "  WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0   AND GUID IN (SELEct Orderguid from OrderDetails  WHERE ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0  ) and Supplier IN (" + string.Join(",", SupplierIds) + ") group by " + StatusName;
                        }
                        else
                        {
                            strQuery = "SELECT Convert(varchar(20)," + StatusName + ") as 'Text', COUNT(" + StatusName + ") 'Count'  from " + TableName + "  WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0   AND GUID IN (SELEct Orderguid from OrderDetails  WHERE ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0  ) group by " + StatusName;
                        }
                    }
                }
                else if (TableName == "TransferMaster")
                {
                    if (!string.IsNullOrEmpty(MainFilter) && MainFilter.ToLower() == "true")
                        strQuery = "SELECT Convert(varchar(20)," + StatusName + ") as 'Text', COUNT(" + StatusName + ") 'Count'  from " + TableName + "  WHERE CompanyId = " + CompanyID + " AND RoomID = " + RoomID + " AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0   AND GUID IN (SELEct Transferguid from TransferDetail  WHERE ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0  ) AND TransferStatus  IN (1,4)  group by " + StatusName;
                    else
                        strQuery = "SELECT Convert(varchar(20)," + StatusName + ") as 'Text', COUNT(" + StatusName + ") 'Count'  from " + TableName + "  WHERE CompanyId = " + CompanyID + " AND RoomID = " + RoomID + " AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0   AND GUID IN (SELEct Transferguid from TransferDetail  WHERE ISNULL(IsDeleted,0) = 0 AND ISNULL(IsArchived,0) = 0  ) group by " + StatusName;
                }

                else if (TableName == "RequisitionMaster")
                {
                    strQuery = @"SELECT RequisitionStatus as 'Text', COUNT(Distinct RM.GUID) 'Count'  
						FROM   dbo.RequisitionMaster RM INNER JOIN RequisitionDetails RD ON RM.GUID = RD.RequisitionGUID
						WHERE RM.RequisitionStatus in ('Unsubmitted','Submitted')
						AND ISNULL(RD.IsDeleted,0) =0 AND isnull(RM.IsDeleted,0) =0  
						AND ISNULL(RD.IsArchived,0) =0 AND isnull(RM.IsArchived,0) =0  
						AND RM.Room = " + RoomID + @" AND RM.CompanyID = " + CompanyID + @"
						AND RD.Room = " + RoomID + @" AND RD.CompanyID = " + CompanyID + @"
						group by RequisitionStatus
						Union ALL
						SELECT RM.RequisitionStatus as 'Text', Count( distinct RM.GUID)
						FROM   dbo.RequisitionMaster RM INNER JOIN RequisitionDetails RD ON RM.GUID = RD.RequisitionGUID
						WHERE RM.RequisitionStatus = 'Approved'
						And ISNULL(Rd.QuantityApproved,0) > ISNULL(Rd.QuantityPulled,0)
						AND ISNULL(RD.IsDeleted,0) =0 AND isnull(RM.IsDeleted,0) =0  
						AND ISNULL(RD.IsArchived,0) =0 AND isnull(RM.IsArchived,0) =0  
						AND RM.Room = " + RoomID + @" AND RM.CompanyID = " + CompanyID + @"
						AND RD.Room = " + RoomID + @" AND RD.CompanyID = " + CompanyID + @"
						group by RequisitionStatus";
                }

                return (from u in context.ExecuteStoreQuery<CommonDTO>(strQuery)
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text.ToString(),
                            Count = u.Count
                        }).ToList();
            }
        }

        public EnterpriseDTO GetEnterpriseByID(long enterpriseID)
        {
            string MasterDBConnectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString;
            string MasterDBConnectionstring = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection MasterDBConnection = new SqlConnection(MasterDBConnectionstring);
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            string SqlQry = "Select * from EnterpriseMaster Where ID = " + enterpriseID;
            DataSet ds = SqlHelper.ExecuteDataset(MasterDBConnection, CommandType.Text, SqlQry);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                objEnterpriseDTO = (from drs in ds.Tables[0].AsEnumerable()
                                    select new EnterpriseDTO
                                    {
                                        ID = drs.Field<long>("ID"),
                                        //EnterpriseDBConnectionString = drs.Field<string>("EnterpriseDBConnectionString"),
                                        Name = drs.Field<string>("Name"),
                                        EnterpriseDBName = drs.Field<string>("EnterpriseDBName"),
                                        EnterpriseLogo = drs.Field<string>("EnterpriseLogo"),
                                        CreatedBy = drs.Field<Int64>("CreatedBy"),
                                        LastUpdatedBy = drs.Field<Int64>("LastUpdatedBy"),
                                    }).FirstOrDefault();
            }
            return objEnterpriseDTO;
        }

        public IEnumerable<CommonDTO> GetDDData_Old(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // SELECT UserName,ID FROM "DBA"."UserMaster" where CompanyID = 1 and IsDeleted = 0
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT Convert(bigint,ID) as ID,");
                sb.Append(TextFieldName);
                sb.Append(" as Text from ");
                sb.Append(TableName);
                sb.Append(" WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND IsDeleted = 0  order by " + TextFieldName + "");

                int RecordCount = context.ExecuteStoreQuery<CommonDTO>(sb.ToString()).Count();

                return (from u in context.ExecuteStoreQuery<CommonDTO>(sb.ToString())
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Count = RecordCount
                        }).ToList();
            }
        }

        public IEnumerable<CommonDTO> GetDDDataWithValue_Old(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, string ValueField = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // SELECT UserName,ID FROM "DBA"."UserMaster" where CompanyID = 1 and IsDeleted = 0
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT Convert(bigint,ID) as ID,");
                sb.Append(TextFieldName + " as Text ");
                if (!string.IsNullOrWhiteSpace(ValueField))
                {
                    sb.Append(",Convert(varchar, " + ValueField + ") as [Value]");
                }
                sb.Append(" from  " + TableName);
                sb.Append(" WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND IsDeleted = 0  order by " + TextFieldName + "");

                int RecordCount = context.ExecuteStoreQuery<CommonDTO>(sb.ToString()).Count();

                return (from u in context.ExecuteStoreQuery<CommonDTO>(sb.ToString())
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Value = u.Value,
                            Count = RecordCount
                        }).ToList();
            }
        }

        public IEnumerable<CommonDTO> GetDDData_Old(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, bool isBom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // SELECT UserName,ID FROM "DBA"."UserMaster" where CompanyID = 1 and IsDeleted = 0
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT Convert(bigint,ID) as ID,");
                sb.Append(TextFieldName);
                sb.Append(" as Text from ");
                sb.Append(TableName);
                sb.Append(" WHERE CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND IsDeleted = 0  AND IsforBom = 0  order by " + TextFieldName + "");

                int RecordCount = context.ExecuteStoreQuery<CommonDTO>(sb.ToString()).Count();

                return (from u in context.ExecuteStoreQuery<CommonDTO>(sb.ToString())
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Count = RecordCount
                        }).ToList();
            }
        }

        public IEnumerable<CommonDTO> GetDDData_Old(string TableName, string TextFieldName, string WhereCondition, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // SELECT UserName,ID FROM "DBA"."UserMaster" where CompanyID = 1 and IsDeleted = 0
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ID,");
                sb.Append(TextFieldName);
                sb.Append(" as Text from ");
                sb.Append(TableName);
                sb.Append(" WHERE " + WhereCondition + " CompanyId = " + CompanyID + " AND Room = " + RoomID + " AND IsDeleted = 0  order by " + TextFieldName + "");

                int RecordCount = context.ExecuteStoreQuery<CommonDTO>(sb.ToString()).Count();

                return (from u in context.ExecuteStoreQuery<CommonDTO>(sb.ToString())
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Count = RecordCount
                        }).ToList();
            }
        }
    }    
}
