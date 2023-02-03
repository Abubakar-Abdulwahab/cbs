using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSBranchSubUsersUploadBatchItemsStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSBranchSubUsersUploadBatchItemsStaging).Name,
                table => table
                            .Column<Int64>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging) + "_Id", c => c.NotNull())
                            .Column<int>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchState) + "_Id", c => c.Nullable())
                            .Column<int>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGA) + "_Id", c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateCode), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGACode), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateValue), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGAValue), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName), c => c.Nullable())
                            .Column<int>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.TaxEntityProfileLocation)+"_Id", c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserName), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber), c => c.Nullable())
                            .Column<int>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.User)+"_Id", c => c.Nullable())
                            .Column<bool>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError), c => c.NotNull().WithDefault(false))
                            .Column<string>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage), c => c.Nullable().Unlimited())
                            .Column<DateTime>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.CreatedAtUtc), c => c.Nullable())
                            .Column<DateTime>(nameof(PSSBranchSubUsersUploadBatchItemsStaging.UpdatedAtUtc), c => c.Nullable())
                );
            return 1;
        }
    }
}