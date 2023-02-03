using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class GenerateRequestWithoutOfficersUploadBatchItemsStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(GenerateRequestWithoutOfficersUploadBatchItemsStaging).Name,
                table => table
                            .Column<Int64>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging) + "_Id", c => c.NotNull())
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.BranchCode), c => c.Nullable())
                            .Column<int>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficers), c => c.Nullable())
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficersValue), c => c.Nullable())
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode), c => c.Nullable())
                            .Column<int>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType), c => c.Nullable())
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandTypeValue), c => c.Nullable())
                            .Column<int>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command) + "_Id", c => c.Nullable())
                            .Column<int>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType), c => c.Nullable())
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayTypeValue), c => c.Nullable())
                            .Column<bool>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError), c => c.NotNull().WithDefault(false))
                            .Column<string>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage), c => c.Unlimited())
                            .Column<DateTime>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.UpdatedAtUtc), c => c.NotNull())
                );
            return 1;
        }
    }
}