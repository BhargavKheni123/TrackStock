using System.Collections.Generic;
using System.Data;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public class HelpDocumentDAL : eTurnsMasterBaseDAL
    {

        #region [Class Constructor]

        public HelpDocumentDAL()
        {

        }


        #endregion


        public IEnumerable<HelpDocumentMasterDTO> GetAllRecord()
        {
            List<HelpDocumentMasterDTO> helpdocs = new List<HelpDocumentMasterDTO>();

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                helpdocs = (from im in context.HelpDocumentMasters
                            select new HelpDocumentMasterDTO
                            {
                                ID = im.ID,
                                ModuleName = im.ModuleName,
                                ModuleDocName = im.ModuleDocName,
                                ModuleDocPath = im.ModuleDocPath
                            }).OrderBy(i => i.ModuleName).ToList();
            }
            return helpdocs;
        }

        public List<HelpDocumentMasterDTO> GetPagedHelpDocumentRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, string HelpDocType)
        {
            List<HelpDocumentMasterDTO> helpdocs = new List<HelpDocumentMasterDTO>();
            TotalCount = 0;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@HelpDocType", HelpDocType ?? (object)DBNull.Value)
                };
                
                helpdocs = context.Database.SqlQuery<HelpDocumentMasterDTO>("exec [GetHelpDocumentByPaging] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@HelpDocType", params1).ToList();
                TotalCount = 0;
                if (helpdocs != null && helpdocs.Count > 0)
                {
                    TotalCount = helpdocs.First().TotalRecords ?? 0;
                }
            }
            return helpdocs;
        }

        public bool UpdateHelpDocument(HelpDocumentMasterDTO objHelpDocDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objHelpDocDTO.ID),
                                                   new SqlParameter("@ModuleDocName", objHelpDocDTO.ModuleDocName ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleDocPath", objHelpDocDTO.ModuleDocPath ?? (object)DBNull.Value),
                                                   new SqlParameter("@ModuleVideoPath", objHelpDocDTO.ModuleVideoPath ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsDoc", objHelpDocDTO.IsDoc),
                                                   new SqlParameter("@IsVideo", objHelpDocDTO.IsVideo),
                                                   new SqlParameter("@UpdatedBy", objHelpDocDTO.LastUpdatedBy),
                                                   new SqlParameter("@ModuleVideoName", objHelpDocDTO.ModuleVideoName ?? (object)DBNull.Value)
                };
                context.Database.ExecuteSqlCommand("EXEC [UpdateHelpDocument] @ID,@ModuleDocName,@ModuleDocPath,@ModuleVideoPath,@IsDoc,@IsVideo,@UpdatedBy,@ModuleVideoName", params1);
                return true;
            }
        }

        public HelpDocumentMasterDTO GetHelpDocumentMasterByID(Int64 ID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
                return context.Database.SqlQuery<HelpDocumentMasterDTO>("exec [GetHelpDocumentMasterByID] @ID", params1).FirstOrDefault();
            }
        }

        public HelpDocumentMasterDTO GetHelpDocumentMasterByName(string Name)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Name", Name) };
                return context.Database.SqlQuery<HelpDocumentMasterDTO>("exec [GetHelpDocumentMasterByName] @Name", params1).FirstOrDefault();
            }
        }

        public HelpDocumentMasterDTO GetHelpDocumentByReportID(Int64 ReportID, string DBName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ReportID", ReportID), new SqlParameter("@DBName", DBName) };
                return context.Database.SqlQuery<HelpDocumentMasterDTO>("exec [GetHelpDocumentByReportID] @ReportID,@DBName", params1).FirstOrDefault();
            }
        }

        public HelpDocumentMasterDTO GetHelpDocumentMasterByDocType(string Name, int DocType)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Name", Name), new SqlParameter("@DocType", DocType) };
                return context.Database.SqlQuery<HelpDocumentMasterDTO>("exec [GetHelpDocumentMasterByDocType] @Name,@DocType", params1).FirstOrDefault();
            }
        }

    }
}
