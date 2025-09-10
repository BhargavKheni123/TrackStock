using System;

namespace eTurns.DTO
{
    public class EnterPriseSQLScriptsDTO
    {
        public long SQLScriptID { get; set; }
        public string ScriptName { get; set; }
        public string ScriptText { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public bool IsDeleted { get; set; }
        public int ExecuitionSequence { get; set; }
        public bool IsMasterScript { get; set; }
    }

    public class EnterPriseSQLScriptsDtlDTO
    {
        public long EnterpriseSqlScriptsDtlID { get; set; }
        public long SQLScriptID { get; set; }
        public long EnterpriseID { get; set; }
        public bool IsExecuitedSuccessfully { get; set; }
        public bool IsLatestExecution { get; set; }
        public string ReturnResult { get; set; }
        public string EnterPriseName { get; set; }
        public string EnterPriseDBName { get; set; }
    }

}
