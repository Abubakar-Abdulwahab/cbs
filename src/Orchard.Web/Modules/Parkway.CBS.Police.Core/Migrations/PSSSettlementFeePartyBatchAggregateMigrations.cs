using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementFeePartyBatchAggregateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementFeePartyBatchAggregate).Name,
                table => table
                   .Column<Int64>(nameof(PSSSettlementFeePartyBatchAggregate.Id), column => column.PrimaryKey().Identity())                      
                        .Column<string>(nameof(PSSSettlementFeePartyBatchAggregate.FeePartyName), column => column.NotNull())                         
                        .Column<decimal>(nameof(PSSSettlementFeePartyBatchAggregate.TotalSettlementAmount), column => column.NotNull())
                        .Column<decimal>(nameof(PSSSettlementFeePartyBatchAggregate.Percentage), column => column.NotNull().WithDefault(0.00m))
                        .Column<string>(nameof(PSSSettlementFeePartyBatchAggregate.BankCode), column => column.NotNull())
                        .Column<string>(nameof(PSSSettlementFeePartyBatchAggregate.BankName), column => column.NotNull())
                        .Column<string>(nameof(PSSSettlementFeePartyBatchAggregate.BankAccountNumber), column => column.NotNull())
                        .Column<Int64>(nameof(PSSSettlementFeePartyBatchAggregate.Batch) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyBatchAggregate.FeeParty) + "_Id", column => column.NotNull())
                        .Column<string>(nameof(PSSSettlementFeePartyBatchAggregate.AdditionalSplitValue), column => column.Nullable())
                        .Column<int>(nameof(PSSSettlementFeePartyBatchAggregate.Command) + "_Id", column => column.Nullable())
                        .Column<DateTime>(nameof(PSSSettlementFeePartyBatchAggregate.CreatedAtUtc), column => column.NotNull())
                        .Column<DateTime>(nameof(PSSSettlementFeePartyBatchAggregate.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementFeePartyBatchAggregate).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint FEEPARTYBATCHAGG_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id], [{nameof(PSSSettlementFeePartyBatchAggregate.Batch)}_Id], [{nameof(PSSSettlementFeePartyBatchAggregate.BankCode)}], [{nameof(PSSSettlementFeePartyBatchAggregate.BankAccountNumber)}]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}