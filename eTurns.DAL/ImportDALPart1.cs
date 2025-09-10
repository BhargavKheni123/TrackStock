using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class ImportDAL : eTurnsBaseDAL
    {
        public List<UDFOptionsCheckDTO> GetUDFList(Int64 RoomID, string TableName, string ControlType)
        {
            List<UDFOptionsCheckDTO> lst = new List<UDFOptionsCheckDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //lst = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.UDFControlType <> '" + ControlType + "' AND  A.UDFTableName= '" + TableName + "' AND A.CompanyID = " + CompanyID.ToString())
                lst = (from u in context.ExecuteStoreQuery<UDFOptionsCheckDTO>(@"Select ISNULL(A.ID,0) AS UDFID, UO.UDFOption , A.UDFColumnName " +
                           //" --, A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName " +
                "from UDF A LEFT OUTER JOIN UDFOptions UO ON UO.UDFID = A.ID " +
                "where A.room=" + RoomID.ToString() + " and A.UDFTableName='" + TableName + "' and A.IsDeleted=0 and A.UDFControlType<>'" + ControlType + "' " +
                "AND (UO.IsDeleted = 0 OR UO.IsDeleted IS NULL) ")


                       select new UDFOptionsCheckDTO
                       {
                           UDFOption = u.UDFOption,
                           UDFID = u.UDFID,
                           UDFColumnName = u.UDFColumnName
                       }).ToList();
            }
            return lst;
        }
    }
}
