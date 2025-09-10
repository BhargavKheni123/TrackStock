using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class ItemMasterDAL : eTurnsBaseDAL
    {

        public int GetItemStatusCount(long RoomID, long CompanyID, DateTime FromDate, DateTime ToDate, string Criteria)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@CompanyId", CompanyID) ,
                                                new SqlParameter("@RoomId", RoomID),
                                                new SqlParameter("@FromDate", FromDate) ,
                                                new SqlParameter("@ToDate", ToDate) ,
                                                new SqlParameter("@Criteria", Criteria) ,
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int?>("exec GetItemStatusCount @CompanyId,@RoomId,@FromDate,@ToDate,@Criteria ", params1).FirstOrDefault().GetValueOrDefault(0);
            }
        }

        public long GetStockOutsForDashboard(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@RoomID", RoomId),
                                                    new SqlParameter("@CompanyID", CompanyId)
                                                };
                return context.Database.SqlQuery<long>("exec [GetStockOutsForDashboard] @RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<ItemMonthWiseStockOutDTO> GetMonthWiseStockOutList(Int64 RoomID, Int64 CompanyID)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            List<ItemMonthWiseStockOutDTO> lstItemMonthWiseStockOutDTO = new List<ItemMonthWiseStockOutDTO>();
            DataSet ds = new DataSet();
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "USP_GetMonthWiseInventoryStockOut", CompanyID, RoomID);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                lstItemMonthWiseStockOutDTO = (from so in dt.AsEnumerable()
                                               select new ItemMonthWiseStockOutDTO
                                               {
                                                   Id = so.Field<int?>("Id"),
                                                   Month = so.Field<int?>("calcmonth"),
                                                   Year = so.Field<int?>("calcYear"),
                                                   Turns = so.Field<double?>("Turns"),
                                                   InventoryValue = so.Field<double?>("InventoryValue"),
                                                   Stockouts = so.Field<int?>("Stockouts")
                                               }).ToList();
            }
            return lstItemMonthWiseStockOutDTO;
        }

    }
}
