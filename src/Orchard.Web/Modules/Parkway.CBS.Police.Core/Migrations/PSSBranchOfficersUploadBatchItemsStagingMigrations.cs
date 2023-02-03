using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSBranchOfficersUploadBatchItemsStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSBranchOfficersUploadBatchItemsStaging).Name,
                table => table
                            .Column<Int64>(nameof(PSSBranchOfficersUploadBatchItemsStaging.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging) + "_Id", c => c.NotNull())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.BranchCode), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.BranchCodeValue), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerName), c => c.Nullable())
                            .Column<int>(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommand) + "_Id", c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandValue), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandCode), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.RankCode), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.RankName), c => c.Nullable())
                            .Column<int>(nameof(PSSBranchOfficersUploadBatchItemsStaging.Rank) + "_Id", c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.BankCode), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.BankName), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.PhoneNumber), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.IPPISNumber), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.Gender), c => c.Nullable())
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.AccountNumber), c => c.Nullable())
                            .Column<bool>(nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError), c => c.NotNull().WithDefault(false))
                            .Column<string>(nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage), c => c.Unlimited())
                            .Column<DateTime>(nameof(PSSBranchOfficersUploadBatchItemsStaging.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(PSSBranchOfficersUploadBatchItemsStaging.UpdatedAtUtc), c => c.NotNull())
                );
            return 1;
        }
    }
}