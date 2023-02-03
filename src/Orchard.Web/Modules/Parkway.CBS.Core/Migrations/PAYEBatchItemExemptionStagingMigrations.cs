using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchItemExemptionStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBatchItemExemptionStaging).Name,
                table => table
                            .Column<long>(nameof(PAYEBatchItemExemptionStaging.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(PAYEBatchItemExemptionStaging.PAYEExemptionType) + "_Id", column => column.Nullable())
                            .Column<string>(nameof(PAYEBatchItemExemptionStaging.PAYEExemptionTypeName), column => column.Nullable())
                            .Column<long>(nameof(PAYEBatchItemExemptionStaging.PAYEBatchRecordStaging) + "_Id", column => column.NotNull())
                            .Column<long>(nameof(PAYEBatchItemExemptionStaging.PAYEBatchItemsStaging) + "_Id", column => column.Nullable())
                            .Column<decimal>(nameof(PAYEBatchItemExemptionStaging.Amount), column => column.NotNull())
                            .Column<string>(nameof(PAYEBatchItemExemptionStaging.AmountStringValue), column => column.Nullable())
                            .Column<int>(nameof(PAYEBatchItemExemptionStaging.SerialNumber), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBatchItemExemptionStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBatchItemExemptionStaging.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}