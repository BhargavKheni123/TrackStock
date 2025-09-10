namespace eTurnsMaster.DAL
{
    public class MasterEnums
    {
        public enum UserType
        {
            /// <summary>
            /// 1
            /// </summary>
            SuperAdmin = 1,
            /// <summary>
            /// 2
            /// </summary>
            EnterpriseAdmin = 2,
            /// <summary>
            /// 3
            /// </summary>
            CompanyAdmin = 3,
            /// <summary>
            /// 4
            /// </summary>
            EnterpriseSystemUser = 4,
            /// <summary>
            /// 5
            /// </summary>
            eTurnsSystemUser = 5
        }

        public enum DBOperation
        {
            INSERT = 1,
            UPDATE = 2,
            DELETE = 3,
            SelectALL = 4,
            SelectById = 5
        }
    }
}
