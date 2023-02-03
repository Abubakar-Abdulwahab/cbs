using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ScheduleLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ScheduleLog).Name,
                table => table
                            .Column<string>("BillingSnap", column => column.NotNull())
                            .Column<int>("BillingModel_Id", column => column.NotNull())
                            .Column<Int64>("BillingSchedule_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<string>("InvoiceNumber", column => column.NotNull())
                            .Column<decimal>("Amount", column => column.NotNull())
                            .Column<string>("TaxPayerNumber", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}