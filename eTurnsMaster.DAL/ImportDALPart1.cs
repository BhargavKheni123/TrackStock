using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurnsMaster.DAL
{
    public partial class ImportDAL : eTurnsMasterBaseDAL
    {
        public bool CheckPullGUIDExistInImportDB(long RoomID, long CompanyID, Guid GUID, string ImportDBName)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                ImportDTO.Import_PULL_DTO obj = context.ExecuteStoreQuery<ImportDTO.Import_PULL_DTO>(@"SELECT A.* FROM [" + ImportDBName + "].DBO.Import_PULL A WHERE A.RoomID = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + " AND A.PullGUID = '" + GUID.ToString() + "'").ToList().FirstOrDefault();
                if (obj != null && obj.PullGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    return true;
                }
                return false;
            }
        }

        public List<ImportDTO.ImportStatus_DTO> GetTaskByImportBulkID(string ImportDBName, string ImportType, string ImportBulkID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {


                    List<ImportDTO.ImportStatus_DTO> lstImportStatus = (from A in context.ExecuteStoreQuery<ImportDTO.ImportStatus_DTO>(@"SELECT ID, ImportType, ImportBulkID, TotalRecords, ImportCreatedDate, IsAllItemInserted, IsImportStarted, IsImportCompleted, ImportComplitionDate, SuccessRecords	FROM [" + ImportDBName + "].DBO.ImportStatus	WHERE ImportBulkID = '" + ImportBulkID + "'")
                                                                        select new ImportDTO.ImportStatus_DTO()
                                                                        {
                                                                            ID = A.ID,
                                                                            ImportType = A.ImportType,
                                                                            ImportBulkID = A.ImportBulkID,
                                                                            TotalRecords = A.TotalRecords,
                                                                            ImportCreatedDate = A.ImportCreatedDate,
                                                                            IsAllItemInserted = A.IsAllItemInserted,
                                                                            IsImportStarted = A.IsImportStarted,
                                                                            IsImportCompleted = A.IsImportCompleted,
                                                                            ImportComplitionDate = A.ImportComplitionDate,
                                                                            SuccessRecords = A.SuccessRecords
                                                                        }
                                                                       ).ToList();

                    return lstImportStatus;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
