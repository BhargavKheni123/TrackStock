
namespace eTurns.DAL
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    public partial class eTurnsEntities : DbContext
    {
        eTurnsEntitiesDB objBase;
        public eTurnsEntities(string connectionString) : base(connectionString)
        {
            objBase = new DAL.eTurnsEntitiesDB(connectionString);
            this.Database.CommandTimeout = 3600;
        }

        protected eTurnsEntities(DbCompiledModel model) : base(model)
        {
        }
        public eTurnsEntities(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model)
        {
        }

        public eTurnsEntities(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public eTurnsEntities(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
        {
        }

        public eTurnsEntities(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
        {
        }
    }

    public class eTurnsEntitiesDB : ObjectContext
    {

        public eTurnsEntitiesDB(string connectionString) : base(connectionString)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            this.CommandTimeout = 7200;
        }
    }
}
