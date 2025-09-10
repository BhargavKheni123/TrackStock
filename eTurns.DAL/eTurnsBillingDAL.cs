using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using eTurns.DTO;
using Newtonsoft.Json;

namespace eTurns.DAL
{
    public class eTurnsBillingDAL : eTurnsBaseDAL
    {
        public eTurnsBillingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<BillingRoomTypeModuleMasterDTO> GetBillingRoomTypeModuleMaster(int BillingRoomTypeID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@BillingRoomTypeID", BillingRoomTypeID)
                };

                List<BillingRoomTypeModuleMasterDTO> list = context.Database.SqlQuery<BillingRoomTypeModuleMasterDTO>("exec [GetBillingRoomTypeModuleMaster] @BillingRoomTypeID", para).ToList();
                return list;
            }
        }

        public eTurnsBillingViewModel GetBillingRoomTypeCostMaster(int BillingRoomTypeID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@BillingRoomTypeID", BillingRoomTypeID)
                };

                return context.Database.SqlQuery<eTurnsBillingViewModel>("exec [GetBillingRoomTypeCostMaster] @BillingRoomTypeID", para).FirstOrDefault();
            }
        }

        public bool SaveBillingRoomModuleAndCost(int BillingRoomTypeId,double BaseCost,double OneTimeLicenceFee,byte Grouping, List<BillingRoomTypeModuleMasterDTO> list, long createdBy)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string RoomModuleMapJSON = JsonConvert.SerializeObject(list, Formatting.Indented);
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@BillingRoomTypeId", BillingRoomTypeId),
                    new SqlParameter("@BaseCost", BaseCost),
                    new SqlParameter("@OneTimeLicenceFee", OneTimeLicenceFee),
                    new SqlParameter("@Grouping", Grouping),
                    new SqlParameter("@RoomModuleMapJSON", RoomModuleMapJSON),
                    new SqlParameter("@CreatedBy",createdBy)                    
                };

                context.Database.CommandTimeout = 3600;
                int status = context.Database.SqlQuery<int>("exec [SaveBillingRoomModuleAndCost] @BillingRoomTypeId,@BaseCost,@OneTimeLicenceFee,@Grouping,@RoomModuleMapJSON,@CreatedBy", para).FirstOrDefault();

                return status >= 0;
            }
        }

    }
}
