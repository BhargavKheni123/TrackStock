using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class UDFOptionDAL : eTurnsBaseDAL
    {
        public UDFOptionsDTO UDFOptionByID(long OptionID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OptionID", OptionID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UDFOptionsDTO>("exec [UDFOptionByID] @OptionID", params1).FirstOrDefault();
            }
        }

        public List<UDFOptionsDTO> UDFOptionsByUDFID(long UdfId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UdfId", UdfId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UDFOptionsDTO>("exec [UDFOptionsByUDFID] @UdfId", params1).ToList();
            }
        }
        public UDFOptionsDTO GetUDFOptionByUDFIDAndOptionName(long UdfId, string UDFOption)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UdfId", UdfId), new SqlParameter("@UDFOption", UDFOption ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UDFOptionsDTO>("exec [GetUDFOptionByUDFIDAndOptionName] @UdfId,@UDFOption", params1).FirstOrDefault();
            }
        }
        public List<UDFOptionsDTO> UDFOptionsByUDFIDNameSearch(long UdfId, string OptionName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UdfId", UdfId), new SqlParameter("@OptionName", OptionName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UDFOptionsDTO>("exec [UDFOptionsByUDFIDNameSearch] @UdfId,@OptionName", params1).ToList();
            }
        }

        public bool IsUdfOptionExistsInUDF(long UDfID, string optionValue)
        {

            UDFOptionsDTO objUDFOptionsDTO = GetUDFOptionByUDFIDAndOptionName(UDfID, optionValue);
            if (objUDFOptionsDTO != null && objUDFOptionsDTO.ID > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete(Int64 id, Int64 userid, bool OthereTurns = true)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (OthereTurns)
                {
                    string sSQL = "";
                    sSQL += "UPDATE UDFOptions SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + id.ToString() + ";";
                    context.ExecuteStoreCommand(sSQL);
                    return true;
                }
                else
                {
                    string sSQL = "";
                    sSQL += "UPDATE " + CommonDAL.GeteTurnsDatabase() + ".dbo.UDFOptions SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + id.ToString() + ";";
                    context.ExecuteStoreCommand(sSQL);
                    return true;
                }
            }
        }

        public IEnumerable<UDFDTO> GetAllRecords(Int64 CompanyID, string UDFTableName, Int64 RoomID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;
            //   return GetCachedData(CompanyID).Where(t => (t.UDFTableName ?? string.Empty).Trim().ToLower() == (UDFTableName ?? string.Empty).Trim().ToLower() && (t.Room ?? 0) == RoomID).OrderBy("UDFColumnName ASC");
            return GetDataCompanyTableNameWise(CompanyID, UDFTableName, RoomID).OrderBy("UDFColumnName ASC");

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName' FROM UDF A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.UDFTableName = '" + UDFTableName + "' AND A.CompanyID = " + CompanyID)
            //            select new UDFDTO
            //            {
            //                ID = u.ID,
            //                CompanyID = u.CompanyID,
            //                UDFTableName = u.UDFTableName,
            //                UDFColumnName = u.UDFColumnName,
            //                UDFControlType = u.UDFControlType,
            //                UDFDefaultValue = u.UDFDefaultValue,
            //                UDFOptionsCSV = u.UDFOptionsCSV,
            //                UDFIsRequired = u.UDFIsRequired,
            //                UDFIsSearchable = u.UDFIsSearchable,
            //                Created = u.Created,
            //                Updated = u.Updated,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                IsDeleted = u.IsDeleted
            //            }).ToList();
            //}
        }
    }
}
