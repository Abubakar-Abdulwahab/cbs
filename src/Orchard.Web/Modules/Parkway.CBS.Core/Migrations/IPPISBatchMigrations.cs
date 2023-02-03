using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class IPPISBatchMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(IPPISBatch).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("ProccessStage", column => column.NotNull())
                            .Column<int>("NumberOfRecords", column => column.Nullable())
                            .Column<string>("FilePath", column => column.NotNull().WithLength(1000))
                            .Column<bool>("ErrorOccurred", column => column.NotNull().WithDefault(false))
                            .Column<string>("ErrorMessage", column => column.Nullable().Unlimited())
                            .Column<int>("Month", column => column.NotNull())
                            .Column<int>("Year", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(IPPISBatch).Name, table => table.AddColumn("HasSummaryFileMoved", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(IPPISBatch).Name, table => table.AddColumn("IsSummaryFileReady", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(IPPISBatch).Name, table => table.AddColumn("ErrorProcessingSummaryFile", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(IPPISBatch).Name, table => table.AddColumn("ErrorMessageProcessingSummaryFile", System.Data.DbType.String, column => column.Unlimited()));
            return 2;
        }

    }
}