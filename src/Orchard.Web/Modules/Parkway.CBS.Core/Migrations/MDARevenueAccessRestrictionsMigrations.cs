using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class MDARevenueAccessRestrictionsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(MDARevenueAccessRestrictions).Name,
                table => table
                            .Column<Int64>("Id", column => column.Identity().PrimaryKey())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.Nullable())
                            .Column<bool>("IsDeleted", column => column.NotNull())
                            .Column<int>("OperationType", column => column.NotNull())
                            .Column<long>("OperationTypeIdentifierId", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}