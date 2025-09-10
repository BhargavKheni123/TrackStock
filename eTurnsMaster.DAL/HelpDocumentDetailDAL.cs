using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public class HelpDocumentDetailDAL : eTurnsMasterBaseDAL
    {
        #region [Class Constructor]
        public HelpDocumentDetailDAL()
        {
        }
        #endregion

        public HelpDocumentDetailDTO GetHelpDocumentDetailByMasterIDModName(Int64 MasterID, string ModuleDocName, bool IsDoc, bool IsVideo)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@MasterID", MasterID), 
                                                   new SqlParameter("@ModuleDocName", ModuleDocName),
                                                   new SqlParameter("@IsDoc", IsDoc),
                                                   new SqlParameter("@IsVideo", IsVideo)}; // 
                return context.Database.SqlQuery<HelpDocumentDetailDTO>("exec [GetHelpDocumentDetailByMasterIDModName] @MasterID,@ModuleDocName,@IsDoc,@IsVideo", params1).FirstOrDefault();
            }
        }

        public bool InsertHelpDocumentDetail(HelpDocumentDetailDTO objHelpDocDtlDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HelpDocMasterID", objHelpDocDtlDTO.HelpDocMasterID),
                                                   new SqlParameter("@ModuleDocName", objHelpDocDtlDTO.ModuleDocName ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleDocPath", objHelpDocDtlDTO.ModuleDocPath ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleVideoName", objHelpDocDtlDTO.ModuleVideoName ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleVideoPath", objHelpDocDtlDTO.ModuleVideoPath ?? (object)DBNull.Value),
                                                   new SqlParameter("@CreatedBy", objHelpDocDtlDTO.CreatedBy ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsDeleted", objHelpDocDtlDTO.IsDeleted ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsArchived", objHelpDocDtlDTO.IsArchived ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsDoc", objHelpDocDtlDTO.IsDoc ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsVideo", objHelpDocDtlDTO.IsVideo ?? (object)DBNull.Value)
                };
                context.Database.ExecuteSqlCommand("exec [InsertHelpDocumentDetail] @HelpDocMasterID,@ModuleDocName,@ModuleDocPath,@ModuleVideoName,@ModuleVideoPath,@CreatedBy,@IsDeleted,@IsArchived,@IsDoc,@IsVideo", params1);
                return true;
            }
        }
        public bool UpdateHelpDocumentDetail(HelpDocumentDetailDTO objHelpDocDtlDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objHelpDocDtlDTO.ID),
                                                   new SqlParameter("@ModuleDocName", objHelpDocDtlDTO.ModuleDocName ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleDocPath", objHelpDocDtlDTO.ModuleDocPath ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleVideoName", objHelpDocDtlDTO.ModuleVideoName ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleVideoPath", objHelpDocDtlDTO.ModuleVideoPath ?? (object)DBNull.Value),
                                                   new SqlParameter("@LastUpdatedBy", objHelpDocDtlDTO.LastUpdatedBy ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsDoc", objHelpDocDtlDTO.IsDoc ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsVideo", objHelpDocDtlDTO.IsVideo ?? (object)DBNull.Value)

                };
                context.Database.ExecuteSqlCommand("exec [UpdateHelpDocumentDetail] @ID,@ModuleDocName,@ModuleDocPath,@ModuleVideoName,@ModuleVideoPath,@LastUpdatedBy,@IsDoc,@IsVideo", params1);
                return true;
            }
        }

        public List<HelpDocumentDetailDTO> GetHelpDocumentDetailByMasterID(Int64 MasterID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@MasterID", MasterID) };  
                return context.Database.SqlQuery<HelpDocumentDetailDTO>("exec [GetHelpDocumentDetailByMasterID] @MasterID", params1).ToList();
            }
        }
        public HelpDocumentDetailDTO GetHelpDocumentDetailByID(Int64 DetailID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@DetailID", DetailID) };
                return context.Database.SqlQuery<HelpDocumentDetailDTO>("exec [GetHelpDocumentDetailByID] @DetailID", params1).FirstOrDefault();
            }
        }
        public void DeleteHelpdocument(Int64 HelpDocumentDetailId, Int64 UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HelpDocumentDetailId", HelpDocumentDetailId)
                                                    ,new SqlParameter("@UserId", UserId)};
                context.Database.ExecuteSqlCommand("exec [DeleteHelpDocumentDetailByID] @HelpDocumentDetailId,@UserId", params1);
                
            }
        }

        public List<HelpDocumentDetailDTO> GetHelpDocumentDetailByModuleName(string ModuleName, int DocType)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleName", ModuleName), new SqlParameter("@DocType", DocType) };
                return context.Database.SqlQuery<HelpDocumentDetailDTO>("exec [GetHelpDocumentDetailByModuleName] @ModuleName,@DocType", params1).ToList();
            }
        }

        public List<HelpDocumentDetailDTO> GetHelpDocumentDetailByReportID(Int64 ReportID, string DBName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ReportID", ReportID), new SqlParameter("@DBName", DBName) };
                return context.Database.SqlQuery<HelpDocumentDetailDTO>("exec [GetHelpDocumentDetailByReportID] @ReportID,@DBName", params1).ToList();
            }
        }

        public List<HelpDocumentDetailDTO> GetHelpDocumentDetailByDocType(string Name, int DocType)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Name", Name), new SqlParameter("@DocType", DocType) };
                return context.Database.SqlQuery<HelpDocumentDetailDTO>("exec [GetHelpDocumentDetailByDocType] @Name,@DocType", params1).ToList();
            }
        }

        public List<HelpDocumentDetailDTO> GetHelpDocumentDetailAllDocType ()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<HelpDocumentDetailDTO>("exec [GetHelpDocumentDetailAllDocType]").ToList();
            }
        }

    }
}
