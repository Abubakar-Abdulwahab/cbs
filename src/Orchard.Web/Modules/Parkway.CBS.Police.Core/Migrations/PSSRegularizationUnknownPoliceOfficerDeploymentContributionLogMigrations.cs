using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog).Name,
                table => table
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.GenerateRequestWithoutOfficersUploadBatchItemsStaging) + "_Id", c => c.NotNull())
                            .Column<int>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.NumberOfDays), c => c.NotNull())
                            .Column<decimal>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.DeploymentRate), c => c.NotNull())
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.Invoice) + "_Id", c => c.NotNull())
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.Request) + "_Id", c => c.NotNull())
                            .Column<decimal>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.DeploymentAllowanceAmount), c => c.NotNull())
                            .Column<decimal>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.DeploymentAllowancePercentage), c => c.NotNull())
                            .Column<bool>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.IsActive), c => c.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog.UpdatedAtUtc), c => c.NotNull())
                );
            return 1;
        }
    }
}