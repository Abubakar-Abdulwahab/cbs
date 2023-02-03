using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigZonalCommandRatioComputeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommand) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Request) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatio), column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatioSum), column => column.NotNull().WithDefault(0))
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.RatioAmount), column => column.NotNull().WithDefault(0.00m))
                    .Column<bool>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FallRatioFlag), column => column.NotNull().WithDefault(false))
                    .Column<decimal>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePercentage), column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeeParty) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePartyName), column => column.NotNull().WithLength(500))
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePartyAccountNumber), column => column.NotNull().WithLength(20))
                    .Column<string>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePartyBankCodeForAccountNumber), column => column.NotNull().WithLength(10))
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.CommandWalletDetails) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.LGA) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint ZONALTCMDRATIOCOMPUTE_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.TransactionLog)}_Id], [{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeeParty)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}