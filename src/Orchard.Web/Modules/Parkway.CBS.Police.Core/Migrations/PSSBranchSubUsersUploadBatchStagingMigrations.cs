using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSBranchSubUsersUploadBatchStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSBranchSubUsersUploadBatchStaging).Name,
                table => table
                            .Column<Int64>(nameof(PSSBranchSubUsersUploadBatchStaging.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSBranchSubUsersUploadBatchStaging.CBSUser) + "_Id", c => c.NotNull())
                            .Column<Int64>(nameof(PSSBranchSubUsersUploadBatchStaging.TaxEntity) + "_Id", c => c.NotNull())
                            .Column<int>(nameof(PSSBranchSubUsersUploadBatchStaging.AddedBy) + "_Id", c => c.NotNull())
                            .Column<bool>(nameof(PSSBranchSubUsersUploadBatchStaging.HasError), c => c.NotNull().WithDefault(false))
                            .Column<int>(nameof(PSSBranchSubUsersUploadBatchStaging.Status), c => c.NotNull())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchStaging.ErrorMessage), c => c.Nullable().Unlimited())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchStaging.FilePath), c => c.NotNull().Unlimited())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchStaging.FileName), c => c.NotNull().WithLength(400))
                            .Column<DateTime>(nameof(PSSBranchSubUsersUploadBatchStaging.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSBranchSubUsersUploadBatchStaging.UpdatedAtUtc), c => c.NotNull())
                );
            string tableName = SchemaBuilder.TableDbName(typeof(PSSBranchSubUsersUploadBatchStaging).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat('PSSBSU', '_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}