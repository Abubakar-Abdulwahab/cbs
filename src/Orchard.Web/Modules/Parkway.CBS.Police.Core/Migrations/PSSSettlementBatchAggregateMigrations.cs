using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementBatchAggregateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementBatchAggregate).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementBatchAggregate.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSSettlementBatchAggregate.SettlementBatch) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchAggregate.RetryCount), column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchAggregate.TransactionCount), column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementBatchAggregate.Error), column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementBatchAggregate.Amount), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchAggregate.TimeFired), column => column.NotNull())
                    .Column<string>(nameof(PSSSettlementBatchAggregate.SettlementEngineResponseJSON), column => column.Nullable().Unlimited())
                    .Column<string>(nameof(PSSSettlementBatchAggregate.SettlementEngineRequestJSON), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSSettlementBatchAggregate.RequestReference), column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchAggregate.ErrorType), column => column.NotNull())
                    .Column<string>(nameof(PSSSettlementBatchAggregate.ErrorMessage), column => column.Nullable().Unlimited())
                    .Column<DateTime>(nameof(PSSSettlementBatchAggregate.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchAggregate.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

    }
}