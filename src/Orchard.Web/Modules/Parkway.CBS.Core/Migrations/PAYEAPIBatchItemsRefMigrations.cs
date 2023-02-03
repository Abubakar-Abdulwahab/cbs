using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEAPIBatchItemsRefMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEAPIBatchItemsRef).Name,
                table => table
                            .Column<long>(nameof(PAYEAPIBatchItemsRef.Id), column => column.PrimaryKey().Identity())
                            .Column<long>(nameof(PAYEAPIBatchItemsRef.PAYEBatchItemsStaging) + "_Id", column => column.NotNull())
                            .Column<long>(nameof(PAYEAPIBatchItemsRef.PAYEAPIRequest) + "_Id", column => column.NotNull())
                            .Column<long>(nameof(PAYEAPIBatchItemsRef.PAYEAPIBatchItemsPagesTracker) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(PAYEAPIBatchItemsRef.ItemNumber), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEAPIBatchItemsRef.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEAPIBatchItemsRef.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PAYEAPIBatchItemsRef).Name, table => table.AddColumn("Mac", System.Data.DbType.String, column => column.NotNull()));
            return 2;
        }
    }
}