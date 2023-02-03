using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataBatchMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataBatch).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("AdminUser_Id", column => column.NotNull())
                            .Column<int>("ProccessStage", column => column.NotNull())
                            .Column<int>("NumberOfRecords", column => column.Nullable())
                            .Column<int>("LGA_Id", column => column.NotNull())
                            .Column<int>("StateModel_Id", column => column.NotNull())
                            .Column<string>("FilePath", column => column.Nullable().WithLength(1000))
                            .Column<string>("FileName", column => column.Nullable().WithLength(150))
                            .Column<string>("AdapterClassName", column => column.Nullable().WithLength(500))
                            .Column<bool>("ErrorOccurred", column => column.NotNull().WithDefault(false))
                            .Column<string>("ErrorMessage", column => column.Nullable().Unlimited())
                            .Column<int>("PercentageProgress", column => column.NotNull().WithDefault(0))
                            .Column<int>("Page", column => column.Nullable().WithDefault(0))
                            .Column<int>("NumberOfRecordSentToCashFlow", column => column.Nullable())
                            .Column<int>("TotalPage", column => column.NotNull().WithDefault(0))
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<Int64>("GeneralBatchReference_Id", column => column.NotNull())
                            .Column<string>("BatchInvoiceFileName", column => column.Nullable().WithLength(150))
                            .Column<string>("BatchInvoiceCallBackURL", column => column.Nullable().WithLength(250))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(ReferenceDataBatch).Name);

            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as (concat('REF_DATA_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id])) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}