using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class WalletStatementCallLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(WalletStatementCallLog).Name,
                table => table
                    .Column<long>(nameof(WalletStatementCallLog.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(WalletStatementCallLog.WalletId), column => column.NotNull())
                    .Column<int>(nameof(WalletStatementCallLog.WalletIdentifierType), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementCallLog.StartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementCallLog.EndDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementCallLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementCallLog.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(WalletStatementCallLog).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint WALLETSTATEMENTCALLLOG_UNIQUE_CONSTRAINT UNIQUE([{nameof(WalletStatementCallLog.WalletId)}], [{nameof(WalletStatementCallLog.WalletIdentifierType)}], [{nameof(WalletStatementCallLog.StartDate)}], [{nameof(WalletStatementCallLog.EndDate)}]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}