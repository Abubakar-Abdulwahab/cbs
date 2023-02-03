using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Migrations;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceRolesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceRoles).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Name", column => column.NotNull().Unique())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<bool>("IsActive", column => column.NotNull().WithDefault(true))
                            .Column<int>("AccessType", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}