using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSFailedProcessingFeeConfirmationsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSFailedProcessingFeeConfirmations).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Message", column => column.NotNull().Unlimited())
                    .Column<string>("InvoiceNumber", column => column.Nullable())
                    .Column<Int64>("RequestId", column => column.Nullable())
                    .Column<bool>("NeedsAction", column => column.WithDefault(false))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}