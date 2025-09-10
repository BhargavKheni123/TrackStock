
namespace eTurns.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    public partial class eTurnsMasterEntities : DbContext
    {
        eTurnsMasterEntitiesDB objBase;
        public eTurnsMasterEntities(string connectionString) : base(connectionString)
        {
            objBase = new DAL.eTurnsMasterEntitiesDB(connectionString);
        }

        //public void AddToeMailToSends(eMailToSend obj)
        //{
        //    this.eMailToSends.Add(obj);
        //}
    }
    public class eTurnsMasterEntitiesDB : ObjectContext
    {

        public eTurnsMasterEntitiesDB(string connectionString) : base(connectionString)
        {
            this.ContextOptions.LazyLoadingEnabled = true;

        }
    }
}
