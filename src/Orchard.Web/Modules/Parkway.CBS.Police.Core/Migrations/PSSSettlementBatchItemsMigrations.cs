using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementBatchItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementBatchItems).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementBatchItems.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSSettlementBatchItems.Settlement) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementBatchItems.Invoice) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementBatchItems.Batch) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.FeeParty) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.SettlementFeeParty) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementBatchItems.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.MDA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.RevenueHead) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementBatchItems.TransactionAmount), column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementBatchItems.FeePercentage), column => column.NotNull())
                    .Column<decimal>(nameof(PSSSettlementBatchItems.AmountSettled), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchItems.TransactionDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchItems.PaymentDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchItems.SynchronizationDate), column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.GeneratedByCommand) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(PSSSettlementBatchItems.StateCommand) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(PSSSettlementBatchItems.ZonalCommand) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(PSSSettlementBatchItems.Service) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementBatchItems.Request) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.State) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(PSSSettlementBatchItems.LGA) + "_Id", column => column.Nullable())
                    .Column<string>(nameof(PSSSettlementBatchItems.FeePartyName), column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.PaymentProvider) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementBatchItems.PaymentChannel), column => column.NotNull())
                    .Column<string>(nameof(PSSSettlementBatchItems.AdditionalSplitValue), column => column.Nullable())
                    .Column<Int64>(nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate)+"_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchItems.SettlementDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchItems.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementBatchItems.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSSettlementBatchItems).Name, table => table.AddColumn(nameof(PSSSettlementBatchItems.SettlementCommand) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementBatchItems).Name);

            string queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSSettlementBatchItems.SettlementCommand)+"_Id"} = {nameof(PSSSettlementBatchItems.GeneratedByCommand) + "_Id"} WHERE {nameof(PSSSettlementBatchItems.AdditionalSplitValue)} = 'Command';");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSSettlementBatchItems.SettlementCommand) + "_Id"} = {nameof(PSSSettlementBatchItems.StateCommand) + "_Id"} WHERE {nameof(PSSSettlementBatchItems.AdditionalSplitValue)} = 'State';");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSSettlementBatchItems.SettlementCommand) + "_Id"} = {nameof(PSSSettlementBatchItems.ZonalCommand) + "_Id"} WHERE {nameof(PSSSettlementBatchItems.AdditionalSplitValue)} = 'Zonal';");
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }

    }
}