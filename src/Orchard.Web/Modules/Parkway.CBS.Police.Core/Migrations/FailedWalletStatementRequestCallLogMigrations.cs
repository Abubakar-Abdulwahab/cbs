using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class FailedWalletStatementRequestCallLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(FailedWalletStatementRequestCallLog).Name,
                table => table
                    .Column<long>(nameof(FailedWalletStatementRequestCallLog.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(FailedWalletStatementRequestCallLog.WalletId), column => column.NotNull())
                    .Column<int>(nameof(FailedWalletStatementRequestCallLog.WalletIdentifierType), column => column.NotNull())
                    .Column<bool>(nameof(FailedWalletStatementRequestCallLog.IsSuccessful), column => column.NotNull().WithDefault(false))
                    .Column<int>(nameof(FailedWalletStatementRequestCallLog.RetryCount), column => column.NotNull())
                    .Column<string>(nameof(FailedWalletStatementRequestCallLog.ErrorMessage), column => column.Nullable().Unlimited())
                    .Column<DateTime>(nameof(FailedWalletStatementRequestCallLog.StartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(FailedWalletStatementRequestCallLog.EndDate), column => column.NotNull())
                    .Column<DateTime>(nameof(FailedWalletStatementRequestCallLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(FailedWalletStatementRequestCallLog.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(FailedWalletStatementRequestCallLog).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint FAILEDWALLETSTATEMENTREQUESTCALLLOG_UNIQUE_CONSTRAINT UNIQUE([{nameof(FailedWalletStatementRequestCallLog.WalletId)}], [{nameof(FailedWalletStatementRequestCallLog.WalletIdentifierType)}], [{nameof(FailedWalletStatementRequestCallLog.StartDate)}], [{nameof(FailedWalletStatementRequestCallLog.EndDate)}]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}