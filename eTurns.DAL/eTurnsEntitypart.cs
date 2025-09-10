using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace eTurns.DAL
{
    public partial class eTurnsEntities : DbContext
    {
        //protected eTurnsEntities(DbCompiledModel model) : base(model)
        //{
        //}

        //public eTurnsEntities(string nameOrConnectionString) : base(nameOrConnectionString)
        //{
        //}

        //public eTurnsEntities(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model)
        //{
        //}

        //public eTurnsEntities(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        //{
        //}

        //public eTurnsEntities(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
        //{
        //}

        //public eTurnsEntities(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
        //{
        //}
    }
}
