using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigCommandRatioComputeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigCommandRatioCompute).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Command) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Request) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatio), column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatioSum), column => column.NotNull().WithDefault(0))
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.RatioAmount), column => column.NotNull().WithDefault(0.00m))
                    .Column<bool>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FallRatioFlag), column => column.NotNull().WithDefault(false))
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePercentage), column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeeParty) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePartyName), column => column.NotNull().WithLength(500))
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePartyAccountNumber), column => column.NotNull().WithLength(20))
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePartyBankCodeForAccountNumber), column => column.NotNull().WithLength(10))
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandWalletDetails) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.LGA) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigCommandRatioCompute).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint CRATIOCOMPUTE_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Command)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.TransactionLog)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeeParty)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}