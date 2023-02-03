using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class BillingScheduleMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(BillingSchedule).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<DateTime>("LastRunDate", column => column.Nullable())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<int>("BillingModel_Id", column => column.NotNull())
                            .Column<int>("ScheduleStatus", column => column.NotNull())
                            .Column<Int64>("Rounds", column => column.Nullable())
                            .Column<Int64>("TaxPayer_Id", column => column.Nullable())
                            .Column<string>("TaxPayerNumber", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}