using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSBranchOfficersUploadBatchStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSBranchOfficersUploadBatchStaging).Name,
                table => table
                            .Column<Int64>(nameof(PSSBranchOfficersUploadBatchStaging.Id), c => c.Identity().PrimaryKey())
                            .Column<int>(nameof(PSSBranchOfficersUploadBatchStaging.TaxEntityProfileLocation) + "_Id", c => c.NotNull())
                            .Column<int>(nameof(PSSBranchOfficersUploadBatchStaging.AddedBy) + "_Id", c => c.NotNull())
                            .Column<bool>(nameof(PSSBranchOfficersUploadBatchStaging.HasError), c => c.NotNull().WithDefault(false))
                            .Column<int>(nameof(PSSBranchOfficersUploadBatchStaging.Status), c => c.NotNull())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchStaging.FilePath), c => c.NotNull().Unlimited())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchStaging.ErrorMessage), c => c.Unlimited())
                            .Column<DateTime>(nameof(PSSBranchOfficersUploadBatchStaging.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSBranchOfficersUploadBatchStaging.UpdatedAtUtc), c => c.NotNull())
                );
            string tableName = SchemaBuilder.TableDbName(typeof(PSSBranchOfficersUploadBatchStaging).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat('PSSBOfficers', '_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSBranchOfficersUploadBatchStaging).Name, table => table.AddColumn(nameof(PSSBranchOfficersUploadBatchStaging.HasGeneratedInvoice), System.Data.DbType.Boolean, column => column.NotNull().WithDefault(false)));
            return 2;
        }
    }
}