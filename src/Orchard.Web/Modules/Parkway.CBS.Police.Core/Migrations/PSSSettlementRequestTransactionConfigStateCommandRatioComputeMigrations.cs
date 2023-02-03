using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigStateCommandRatioComputeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommand) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Request) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatio), column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatioSum), column => column.NotNull().WithDefault(0))
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.RatioAmount), column => column.NotNull().WithDefault(0.00m))
                    .Column<bool>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FallRatioFlag), column => column.NotNull().WithDefault(false))
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePercentage), column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeeParty) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePartyName), column => column.NotNull().WithLength(500))
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePartyAccountNumber), column => column.NotNull().WithLength(20))
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePartyBankCodeForAccountNumber), column => column.NotNull().WithLength(10))
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.CommandWalletDetails) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.LGA) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint STCRRATIOCOMPUTE_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.TransactionLog)}_Id], [{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeeParty)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}