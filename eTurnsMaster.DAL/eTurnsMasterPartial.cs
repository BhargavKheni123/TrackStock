
namespace eTurnsMaster.DAL
{
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    public partial class eTurns_MasterEntities : DbContext
    {
        eTurnsMasterEntitiesDB objBase;
        public eTurns_MasterEntities(string connectionString) : base(connectionString)
        {
            objBase = new DAL.eTurnsMasterEntitiesDB(connectionString);
            this.Database.CommandTimeout = 3600;
        }

    }
    public class eTurnsMasterEntitiesDB : ObjectContext
    {

        public eTurnsMasterEntitiesDB(string connectionString) : base(connectionString)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            this.CommandTimeout = 7200;
        }
    }
}
