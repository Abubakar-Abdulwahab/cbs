using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSDeploymentAllowanceSettlementEngineDetailsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSDeploymentAllowanceSettlementEngineDetails).Name,
                table => table
                    .Column<Int64>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.PoliceofficerDeploymentAllowance) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.RetryCount), column => column.NotNull().WithDefault(0))
                    .Column<bool>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.Error), column => column.NotNull())
                    .Column<decimal>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.Amount), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.TimeFired), column => column.Nullable())
                    .Column<string>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.SettlementEngineResponseJSON), column => column.Nullable().Unlimited())
                    .Column<string>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.SettlementEngineRequestJSON), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.RequestReference), column => column.NotNull())
                    .Column<string>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.ErrorMessage), column => column.Nullable().Unlimited())
                    .Column<DateTime>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSDeploymentAllowanceSettlementEngineDetails.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

    }
}