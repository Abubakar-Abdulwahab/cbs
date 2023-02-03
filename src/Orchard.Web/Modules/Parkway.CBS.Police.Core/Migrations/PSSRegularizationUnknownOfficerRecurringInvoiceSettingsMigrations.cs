using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRegularizationUnknownOfficerRecurringInvoiceSettingsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings).Name,
                table => table
                            .Column<Int64>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.GenerateRequestWithoutOfficersUploadBatchStaging) + "_Id", c => c.NotNull())
                            .Column<Int64>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.EscortDetails) + "_Id", c => c.NotNull())
                            .Column<Int64>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.Request) + "_Id", c => c.NotNull())
                            .Column<int>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.WeekDayNumber), c => c.NotNull())
                            .Column<int>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.OffSet), c => c.NotNull())
                            .Column<int>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.PaymentBillingType), c => c.NotNull())
                            .Column<string>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.CronExpression), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.NextInvoiceGenerationDate), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSRegularizationUnknownOfficerRecurringInvoiceSettings.UpdatedAtUtc), c => c.NotNull())
                );
            return 1;
        }
    }
}