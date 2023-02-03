using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class WalletStatementScheduleMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(WalletStatementSchedule).Name,
                table => table
                    .Column<int>(nameof(WalletStatementSchedule.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(WalletStatementSchedule.CronExpression), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementSchedule.NextScheduleDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementSchedule.PeriodStartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementSchedule.PeriodEndDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementSchedule.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementSchedule.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}