using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSProposedRegularizationUnknownPoliceOfficerDeploymentLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog).Name,
                table => table
                            .Column<Int64>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.GenerateRequestWithoutOfficersUploadBatchItemsStaging) + "_Id", c => c.NotNull())
                            .Column<DateTime>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.StartDate), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.EndDate), c => c.NotNull())
                            .Column<decimal>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.DeploymentRate), c => c.NotNull())
                            .Column<Int64>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.Request) + "_Id", c => c.NotNull())
                            .Column<bool>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.IsActive), c => c.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.UpdatedAtUtc), c => c.NotNull())
                );
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog).Name, table => table.AddColumn(nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.Invoice)+"_Id", System.Data.DbType.Int64, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog).Name);
            string requestInvoiceTableName = SchemaBuilder.TableDbName(typeof(PSSRequestInvoice).Name);

            string queryString = $"UPDATE t1 SET t1.Invoice_Id = t2.Invoice_Id FROM {tableName} AS t1 INNER JOIN {requestInvoiceTableName} AS t2 ON t1.Request_Id = t2.Request_Id WHERE t1.Request_Id = t2.Request_Id";
            SchemaBuilder.ExecuteSql(queryString);

            queryString = $"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog.Invoice) + "_Id"} bigint NOT NULL";
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
    }
}