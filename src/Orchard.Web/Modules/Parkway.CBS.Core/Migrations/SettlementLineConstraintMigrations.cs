using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class SettlementLineConstraintMigrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SettlementLineConstraint).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("RevenueHead_Id", column => column.Nullable().WithDefault(0))
                            .Column<int>("MDA_Id", column => column.Nullable().WithDefault(0))
                            .Column<int>("PaymentProvider_Id", column => column.Nullable().WithDefault(0))
                            .Column<int>("PaymentChannel", column => column.Nullable().WithDefault(0))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }

    }
}