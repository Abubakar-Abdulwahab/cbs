using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class RevenueHeadPermissionConstraintsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RevenueHeadPermissionConstraints).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("RevenueHeadPermission_Id", column => column.NotNull())
                            .Column<Int64>("MDA_Id", column => column.NotNull())
                            .Column<Int64>("RevenueHead_Id", column => column.Nullable())
                            .Column<Int32>("ExpertSystem_Id", column => column.NotNull())
                            .Column<Int64>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}