using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementBatchMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementBatch).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementBatch.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSSettlementBatch.PSSSettlement) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatch.ScheduleDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatch.SettlementRangeStartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatch.SettlementRangeEndDate), column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementBatch.SettlementAmount), column => column.NotNull().WithDefault(0.00m))
                    .Column<int>(nameof(PSSSettlementBatch.Status), column => column.NotNull().WithDefault((int)Models.Enums.PSSSettlementBatchStatus.ReadyForQueueing))
                    .Column<string>(nameof(PSSSettlementBatch.StatusMessage), column => column.NotNull().WithDefault("Ready For Queueing "))
                    .Column<bool>(nameof(PSSSettlementBatch.HasError), column => column.NotNull().WithDefault(false))
                    .Column<string>(nameof(PSSSettlementBatch.ErrorMessage), column => column.Nullable().Unlimited())
                    .Column<bool>(nameof(PSSSettlementBatch.HasCommandSplits), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSSettlementBatch.SettlementDate), column => column.Nullable())
                    .Column<DateTime>(nameof(PSSSettlementBatch.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatch.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementBatch).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as (rtrim('PSS_SETTLEMENT_REF_')+case when len(rtrim(CONVERT([nvarchar](20),[Id],0)))>(9) then CONVERT([nvarchar](20),[Id],0) else right(replicate((0),(10))+rtrim(CONVERT([nvarchar](10),[Id],0)),(10)) end) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSSSETTLEMENT_UNIQUE_CONSTRAINT UNIQUE([{1}], [{2}], [{3}]); ", tableName, nameof(PSSSettlementBatch.PSSSettlement) + "_Id", nameof(PSSSettlementBatch.SettlementRangeStartDate), nameof(PSSSettlementBatch.SettlementRangeEndDate)));

            return 1;
        }
    }
}