using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchItemExemptionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBatchItemExemption).Name,
                table => table
                            .Column<long>(nameof(PAYEBatchItemExemption.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(PAYEBatchItemExemption.PAYEExemptionType) + "_Id", column => column.NotNull())
                            .Column<long>(nameof(PAYEBatchItemExemption.PAYEBatchItems) + "_Id", column => column.NotNull())
                            .Column<long>(nameof(PAYEBatchItemExemption.PAYEBatchRecord) + "_Id", column => column.NotNull())
                            .Column<decimal>(nameof(PAYEBatchItemExemption.Amount), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBatchItemExemption.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBatchItemExemption.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}