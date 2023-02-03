using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementPercentageRecalculationFeePartyBatchAggregateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate).Name,
                table => table
                   .Column<Int64>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Id), column => column.PrimaryKey().Identity())
                        .Column<string>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeePartyName), column => column.NotNull())
                        .Column<decimal>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.TotalSettlementAmount), column => column.NotNull())
                        .Column<decimal>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AggregateTotalSettlementAmount), column => column.NotNull())
                        .Column<decimal>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Percentage), column => column.NotNull().WithDefault(0.00m))
                        .Column<decimal>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.CommandPercentage), column => column.NotNull().WithDefault(0.00m))
                        .Column<bool>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FallFlag), column => column.NotNull().WithDefault(false))
                        .Column<string>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankCode), column => column.NotNull())
                        .Column<string>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankName), column => column.NotNull())
                        .Column<string>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankAccountNumber), column => column.NotNull())
                        .Column<Int64>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.SettlementFeeParty) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeeParty) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Command) + "_Id", column => column.NotNull())
                        .Column<string>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AdditionalSplitValue), column => column.NotNull())
                        .Column<DateTime>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.CreatedAtUtc), column => column.NotNull())
                        .Column<DateTime>(nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PERCENTAGE_RECALCULATION_FEEPARTYBATCHAGG_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.SettlementFeeParty)}_Id], [{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch)}_Id], [{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankCode)}], [{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Command)}_Id], [{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankAccountNumber)}]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}