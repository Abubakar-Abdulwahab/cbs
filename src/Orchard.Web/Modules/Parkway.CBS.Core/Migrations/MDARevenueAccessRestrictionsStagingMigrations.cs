using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class MDARevenueAccessRestrictionsStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(MDARevenueAccessRestrictionsStaging).Name,
                table => table
                            .Column<Int64>("Id", column => column.Identity().PrimaryKey())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.Nullable())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<bool>("IsRemoval", column => column.NotNull())
                            .Column<Int64>("MDARevenueHeadEntryStaging_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

    }
}