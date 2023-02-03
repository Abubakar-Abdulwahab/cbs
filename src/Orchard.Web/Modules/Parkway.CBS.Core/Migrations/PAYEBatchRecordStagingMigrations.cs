using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchRecordStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBatchRecordStaging).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<int>("Billing_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<string>("FilePath", column => column.Nullable().Unlimited())
                            .Column<string>("FileName", column => column.Nullable().WithLength(400))
                            .Column<string>("AdapterValue", column => column.Nullable().WithLength(100))
                            .Column<bool>("ErrorOccurred", column => column.Nullable())
                            .Column<string>("ErrorMessage", column => column.Nullable())
                            .Column<decimal>("PercentageProgress", column => column.Nullable())
                            .Column<int>("TotalNoOfRowsProcessed", column => column.Nullable())
                            .Column<Int64>("CBSUser_Id", column => column.Nullable())
                            .Column<bool>("InvoiceConfirmed", column => column.WithDefault(false))
                            .Column<bool>("Treated", column => column.WithDefault(false))
                            .Column<string>("ReceiptNumber", column => column.Nullable())
                            .Column<string>("Month", column => column.Nullable().WithLength(11))
                            .Column<string>("Year", column => column.Nullable().WithLength(11))
                            .Column<int>("AssessmentType", column => column.NotNull())
                            .Column<Int64>("InvoiceItem_Id", column => column.Nullable())
                            .Column<Int64>("OriginIdentifier", column => column.Nullable())
                            .Column<string>("TaxPayerCode", column => column.Nullable())
                            .Column<string>("PaymentTypeCode", column => column.Nullable())
                            .Column<string>("DuplicateComposite", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PAYEBatchRecordStaging).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat([PaymentTypeCode], '_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            string queryStringComposite = string.Format("CREATE UNIQUE INDEX CHECK_FOR_DUPLICATE_COMPOSITE_IF_NOT_NULL ON [dbo].[{0}](DuplicateComposite) WHERE DuplicateComposite IS NOT NULL ", tableName);
            SchemaBuilder.ExecuteSql(queryStringComposite);
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PAYEBatchRecordStaging).Name, table => table.AddColumn("CurrentStage", System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(PAYEBatchRecordStaging).Name, table => table.AddColumn("NextStage", System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(PAYEBatchRecordStaging).Name, table => table.AddColumn("IsProcessingCompleted", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 2;
        }
    }
}