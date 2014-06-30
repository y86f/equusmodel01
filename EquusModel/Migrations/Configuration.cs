namespace EquusModel.Migrations
{
    using EquusModel.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EquusModel.Models.ModelContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator()); //it will generate MySql commands instead of SqlServer commands.
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema)); //here s the thing.
        }

        protected override void Seed(EquusModel.Models.ModelContext context)
        {
            ModelContext.Seed(context);
        }
    }
}
