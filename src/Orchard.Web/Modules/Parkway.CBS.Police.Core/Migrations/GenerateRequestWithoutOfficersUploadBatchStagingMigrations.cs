using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class GenerateRequestWithoutOfficersUploadBatchStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(GenerateRequestWithoutOfficersUploadBatchStaging).Name,
                table => table
                            .Column<Int64>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.Id), c => c.Identity().PrimaryKey())
                            .Column<int>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.TaxEntityProfileLocation) + "_Id", c => c.NotNull())
                            .Column<int>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.AddedBy) + "_Id", c => c.NotNull())
                            .Column<bool>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.HasError), c => c.NotNull().WithDefault(false))
                            .Column<int>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.Status), c => c.NotNull())
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.ErrorMessage), c => c.Nullable().Unlimited())
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.FilePath), c => c.NotNull().Unlimited())
                            .Column<bool>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.HasGeneratedInvoice), c => c.NotNull().WithDefault(false))
                            .Column<DateTime>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.UpdatedAtUtc), c => c.NotNull())
                );
            string tableName = SchemaBuilder.TableDbName(typeof(GenerateRequestWithoutOfficersUploadBatchStaging).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat('PSSGRW', '_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}