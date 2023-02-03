using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class RegularizationDeploymentAllowanceSettlementEngineDetailMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RegularizationDeploymentAllowanceSettlementEngineDetail).Name,
                table => table
                    .Column<long>(nameof(RegularizationDeploymentAllowanceSettlementEngineDetail.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(RegularizationDeploymentAllowanceSettlementEngineDetail.SettlementEngineRequestJSON), column => column.NotNull().WithLength(4000))
                    .Column<string>(nameof(RegularizationDeploymentAllowanceSettlementEngineDetail.SettlementEngineResponseJSON), column => column.Nullable().WithLength(4000))
                    .Column<string>(nameof(RegularizationDeploymentAllowanceSettlementEngineDetail.PaymentReference), column => column.NotNull().WithLength(100))
                    .Column<DateTime>(nameof(RegularizationDeploymentAllowanceSettlementEngineDetail.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(RegularizationDeploymentAllowanceSettlementEngineDetail.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}