using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Newtonsoft.Json;


namespace eTurnsMaster.DAL
{
    public class BillingRoomTypeModulesMapDAL : eTurnsMasterBaseDAL
    {
        public List<BillingRoomTypeModulesMapDTO> GetBillingRoomTypeModulesMap(long billingRoomTypeID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@BillingRoomTypeID", billingRoomTypeID)
                };

                List<BillingRoomTypeModulesMapDTO> list = context.Database.SqlQuery<BillingRoomTypeModulesMapDTO>
                    ("exec [GetBillingRoomTypeModulesMap] @BillingRoomTypeID", para).ToList();
                return list;
            }
        }
        public bool SaveBillingRoomModuleMap(List<BillingRoomTypeModulesMapDTO> list, long createdBy)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string RoomModuleMapJSON = JsonConvert.SerializeObject(list, Formatting.Indented);
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@RoomModuleMapJSON", RoomModuleMapJSON),
                    new SqlParameter("@CreatedBy",createdBy)
                };
                context.Database.CommandTimeout = 3600;
                int status = context.Database.SqlQuery<int>
                    ("exec [SaveBillingRoomModuleMap] @RoomModuleMapJSON , @CreatedBy", para).FirstOrDefault();

                return status >= 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billingRoomTypeId"></param>
        /// <param name="createdBy"></param>
        /// <param name="jobType">'RoomChange' or 'ConfigChange'</param>
        public void InsertBillingTypeMapUpdateJob(int? billingRoomTypeId, long createdBy, string jobType
            , long? roomID, long? enterpriseID, long? companyID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@BillingRoomTypeId", billingRoomTypeId.ToDBNull()),
                    new SqlParameter("@CreatedBy", createdBy),
                    new SqlParameter("@JobType", jobType),
                    new SqlParameter("@RoomID", roomID.ToDBNull()),
                    new SqlParameter("@EnterpriseID", enterpriseID.ToDBNull()),
                    new SqlParameter("@CompanyID", companyID.ToDBNull())
                };
                context.Database.CommandTimeout = 3600;
                int status = context.Database.ExecuteSqlCommand("exec [InsertBillingTypeMapUpdateJob] @BillingRoomTypeId,@CreatedBy, @JobType , @RoomID,@EnterpriseID,@CompanyID ", para);
            }
        }

        public List<BillingRoomTypeModulesMapDTO> GetBillingRoomModules(long roomID, long compId, long entId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", entId),
                    new SqlParameter("@CompanyID", compId),
                    new SqlParameter("@RoomID", roomID)
                };

                List<BillingRoomTypeModulesMapDTO> list = context.Database.SqlQuery<BillingRoomTypeModulesMapDTO>
                    ("exec GetBillingRoomModules @EnterpriseID, @CompanyID, @RoomID", para).ToList();
                return list;
            }
        }

        public List<BillingRoomTypeModuleMasterDTO> GetBillingRoomModulesFromeTurnsBilling(long roomID, long compId, long entId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", entId),
                    new SqlParameter("@CompanyID", compId),
                    new SqlParameter("@RoomID", roomID)
                };

                var list = context.Database.SqlQuery<BillingRoomTypeModuleMasterDTO>("exec GetBillingRoomModulesFromeTurnsBilling @EnterpriseID, @CompanyID, @RoomID", para).ToList();
                return list;
            }
        }

        public int SaveBillingRoomTypeMaster(string newBillingRoomTypeName, long enterpriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@newBillingRoomTypeName", newBillingRoomTypeName),
                    new SqlParameter("@enterpriseID", enterpriseID)
                };
                context.Database.CommandTimeout = 3600;
                int BillingRoomTypeID = context.Database.SqlQuery<int>
                    ("exec [SaveBillingRoomTypeMaster] @newBillingRoomTypeName , @enterpriseID", para).FirstOrDefault();

                return BillingRoomTypeID;
            }
        }
    }
}
