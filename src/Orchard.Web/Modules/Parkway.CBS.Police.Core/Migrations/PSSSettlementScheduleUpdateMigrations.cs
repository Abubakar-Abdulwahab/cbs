using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementScheduleUpdateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementScheduleUpdate).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementScheduleUpdate.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementScheduleUpdate.PSSSettlementPreFlightBatch) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementScheduleUpdate.PSSSettlement) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementScheduleUpdate.SettlementRule) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementScheduleUpdate.CurrentSchedule), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementScheduleUpdate.NextSchedule), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementScheduleUpdate.NextStartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementScheduleUpdate.NextEndDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementScheduleUpdate.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementScheduleUpdate.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}