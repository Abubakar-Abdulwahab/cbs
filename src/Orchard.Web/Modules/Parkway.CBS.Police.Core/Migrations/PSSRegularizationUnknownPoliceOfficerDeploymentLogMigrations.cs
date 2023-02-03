using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRegularizationUnknownPoliceOfficerDeploymentLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRegularizationUnknownPoliceOfficerDeploymentLog).Name,
                table => table
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.GenerateRequestWithoutOfficersUploadBatchItemsStaging) + "_Id", c => c.NotNull())
                            .Column<DateTime>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.StartDate), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.EndDate), c => c.NotNull())
                            .Column<decimal>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.DeploymentRate), c => c.NotNull())
                            .Column<int>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.State) + "_Id", c => c.NotNull())
                            .Column<int>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.LGA) + "_Id", c => c.NotNull())
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.Invoice) + "_Id", c => c.NotNull())
                            .Column<Int64>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.Request) + "_Id", c => c.NotNull())
                            .Column<bool>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.IsActive), c => c.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSRegularizationUnknownPoliceOfficerDeploymentLog.UpdatedAtUtc), c => c.NotNull())
                );
            return 1;
        }
    }
}