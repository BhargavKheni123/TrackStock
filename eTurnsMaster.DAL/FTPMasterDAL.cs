using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data.SqlClient;


namespace eTurnsMaster.DAL
{
    public class FTPMasterDAL : eTurnsMasterBaseDAL
    {
        public IEnumerable<FTPMasterDTO> GetAllImportFTPs()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FTPMasterDTO>("exec [GetAllImportFTPs] ").ToList();
            }
        }

        public void InsertFTPFileExportLog(string ImportDBName, long FTPMasterID, long EnterpriseID, long CompanyID, long RoomID, string FileName)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var ArrParams = new SqlParameter[]
                {
                        new SqlParameter("@FTPMasterID", FTPMasterID),
                        new SqlParameter("@EnterpriseID", EnterpriseID),
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@RoomID", RoomID),
                        new SqlParameter("@FileName", FileName),
                        //new SqlParameter("@IsProcessed", IsProcessed),
                        //new SqlParameter("@ProcessedTime", ProcessedTime  ?? (object)DBNull.Value)
                };

                context.Database.ExecuteSqlCommand(@"EXEC [" + ImportDBName + "].DBO.InsertFTPFileExportLog @FTPMasterID, @EnterpriseID, @CompanyID, @RoomID, @FileName", ArrParams);
            }

        }
    }

}
