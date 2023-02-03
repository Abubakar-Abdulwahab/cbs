using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class WalletStatementScheduleLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(WalletStatementScheduleLog).Name,
                table => table
                            .Column<int>(nameof(WalletStatementScheduleLog.Id), column => column.Identity().PrimaryKey())
                            .Column<int>(nameof(WalletStatementScheduleLog.WalletStatementSchedule)+"_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementScheduleLog.PeriodStartDate), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementScheduleLog.PeriodEndDate), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementScheduleLog.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(WalletStatementScheduleLog.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}