using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
namespace Parkway.CBS.Police.Core.Migrations
{
    public class RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog).Name,
                table => table
                    .Column<long>(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.Reference), column => column.NotNull())
                    .Column<string>(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.PaymentReference), column => column.NotNull().WithLength(100))
                    .Column<int>(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.TransactionStatus), column => column.NotNull())
                    .Column<DateTime>(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.UpdatedAtUtc), column => column.NotNull()));
            return 1;
        }
    }
}