using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEAPIBatchItemsPagesTrackerMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEAPIBatchItemsPagesTracker).Name,
                table => table
                            .Column<long>(nameof(PAYEAPIBatchItemsPagesTracker.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(PAYEAPIBatchItemsPagesTracker.PageNumber), column => column.NotNull())
                            .Column<long>(nameof(PAYEAPIBatchItemsPagesTracker.PAYEAPIRequest) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEAPIBatchItemsPagesTracker.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEAPIBatchItemsPagesTracker.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}