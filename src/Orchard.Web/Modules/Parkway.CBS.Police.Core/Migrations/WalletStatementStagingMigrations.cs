using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class WalletStatementStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(WalletStatementStaging).Name,
                table => table
                            .Column<long>(nameof(WalletStatementStaging.Id), column => column.Identity().PrimaryKey())
                            .Column<int>(nameof(WalletStatementStaging.WalletId), column => column.NotNull())
                            .Column<int>(nameof(WalletStatementStaging.WalletIdentifierType), column => column.NotNull())
                            .Column<int>(nameof(WalletStatementStaging.TransactionTypeId), column => column.NotNull())
                            .Column<string>(nameof(WalletStatementStaging.Narration), column => column.Nullable())
                            .Column<string>(nameof(WalletStatementStaging.TransactionReference), column => column.NotNull().Unique())
                            .Column<decimal>(nameof(WalletStatementStaging.Amount), column => column.NotNull())
                            .Column<string>(nameof(WalletStatementStaging.Reference), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementStaging.TransactionDate), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementStaging.ValueDate), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementStaging.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(WalletStatementStaging).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint WALLETSTATEMENTSTAGING_UNIQUE_CONSTRAINT UNIQUE([{nameof(WalletStatementStaging.WalletId)}], [{nameof(WalletStatementStaging.WalletIdentifierType)}], [{nameof(WalletStatementStaging.TransactionReference)}], [{nameof(WalletStatementStaging.Reference)}]);";

            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }
    }
}