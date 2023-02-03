using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementFeePartyRequestTransactionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementFeePartyRequestTransaction).Name,
                table => table
                   .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransaction.Id), column => column.PrimaryKey().Identity())
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransaction.Batch) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransaction.FeeParty) + "_Id", column => column.NotNull())
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction) + "_Id", column => column.NotNull())
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransaction.TransactionLog) + "_Id", column => column.NotNull())
                        .Column<decimal>(nameof(PSSSettlementFeePartyRequestTransaction.DeductioPercentage), column => column.NotNull().WithDefault(0))
                        .Column<decimal>(nameof(PSSSettlementFeePartyRequestTransaction.TransactionAmount), column => column.NotNull().WithDefault(0))
                        .Column<decimal>(nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle), column => column.NotNull().WithDefault(0))
                        .Column<bool>(nameof(PSSSettlementFeePartyRequestTransaction.IsMaxPercentage), column => column.NotNull().WithDefault(false))
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransaction.Request) + "_Id", column => column.NotNull())
                        .Column<bool>(nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit), column => column.NotNull().WithDefault(false))
                        .Column<string>(nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue), column => column.Nullable())
                        .Column<DateTime>(nameof(PSSSettlementFeePartyRequestTransaction.CreatedAtUtc), column => column.NotNull())
                        .Column<DateTime>(nameof(PSSSettlementFeePartyRequestTransaction.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementFeePartyRequestTransaction).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint FEEPARTYCONRQTXN_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id], [{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id], [{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}