using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSServiceSettlementConfigurationTransactionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSServiceSettlementConfigurationTransaction).Name,
                table => table
                    .Column<Int64>(nameof(PSSServiceSettlementConfigurationTransaction.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSServiceSettlementConfigurationTransaction.Batch) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfigurationTransaction.Service) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfigurationTransaction.MDA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfigurationTransaction.Channel), column => column.NotNull())
                    .Column<Int64>(nameof(PSSServiceSettlementConfigurationTransaction.Request) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSServiceSettlementConfigurationTransaction.Invoice) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSServiceSettlementConfigurationTransaction.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSServiceSettlementConfigurationTransaction.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSServiceSettlementConfigurationTransaction).Name);

            string queryString = $"ALTER TABLE {tableName} add [{nameof(PSSServiceSettlementConfigurationTransaction.CompositeUniqueValue)}] as (concat(CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfigurationTransaction.Channel)}]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id])));"
                +

             $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSCT_TRANSACTION_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id], [{nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id], [{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            string unqiueQuery = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSCT_COMPOSITEVALUE_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSServiceSettlementConfigurationTransaction.CompositeUniqueValue)}]);";
            SchemaBuilder.ExecuteSql(unqiueQuery);


            return 1;
        }

    }
}