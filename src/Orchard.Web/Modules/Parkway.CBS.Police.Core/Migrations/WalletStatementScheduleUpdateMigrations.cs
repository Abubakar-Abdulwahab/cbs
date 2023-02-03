using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class WalletStatementScheduleUpdateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(WalletStatementScheduleUpdate).Name,
                table => table
                    .Column<int>(nameof(WalletStatementScheduleUpdate.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(WalletStatementScheduleUpdate.WalletStatementSchedule)+"_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementScheduleUpdate.CurrentSchedule), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementScheduleUpdate.NextScheduleDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementScheduleUpdate.NextStartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementScheduleUpdate.NextEndDate), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementScheduleUpdate.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(WalletStatementScheduleUpdate.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}