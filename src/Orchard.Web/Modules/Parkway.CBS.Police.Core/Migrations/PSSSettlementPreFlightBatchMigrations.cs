using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementPreFlightBatchMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementPreFlightBatch).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementPreFlightBatch.Id), column => column.PrimaryKey().Identity())
                    .Column<DateTime>(nameof(PSSSettlementPreFlightBatch.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementPreFlightBatch.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}