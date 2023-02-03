using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class WalletStatementMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(WalletStatement).Name,
                table => table
                            .Column<long>(nameof(WalletStatement.Id), column => column.Identity().PrimaryKey())
                            .Column<int>(nameof(WalletStatement.WalletId), column => column.NotNull())
                            .Column<int>(nameof(WalletStatement.WalletIdentifierType), column => column.NotNull())
                            .Column<int>(nameof(WalletStatement.TransactionTypeId), column => column.NotNull())
                            .Column<string>(nameof(WalletStatement.Narration), column => column.Nullable())
                            .Column<string>(nameof(WalletStatement.TransactionReference), column => column.NotNull().Unique())
                            .Column<decimal>(nameof(WalletStatement.Amount), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatement.TransactionDate), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatement.ValueDate), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatement.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatement.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }

    }
}