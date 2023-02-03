using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Orchard.Data.Migration.Schema;

namespace Parkway.CBS.Core.Migrations
{
    public class DirectAssessmentBatchRecordMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(DirectAssessmentBatchRecord).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<string>("FilePath", column => column.Nullable().Unlimited())
                            .Column<string>("FileName", column => column.Nullable().WithLength(400))
                            .Column<string>("AdapterValue", column => column.Nullable().WithLength(100))
                            .Column<string>("RulesApplied", column => column.Nullable().WithLength(1000))
                            .Column<string>("PaymentTypeCode", column => column.Nullable())
                            .Column<bool>("ErrorOccurred", column => column.Nullable())
                            .Column<string>("ErrorMessage", column => column.Nullable())
                            .Column<decimal>("Amount", column => column.Nullable())
                            .Column<int>("Billing_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<bool>("InvoiceConfirmed", column => column.WithDefault(false))
                            .Column<decimal>("PercentageProgress", column => column.Nullable())
                            .Column<int>("TotalNoOfRowsProcessed", column => column.Nullable())
                            .Column<string>("Type", column => column.NotNull())
                            .Column<string>("Origin", column => column.Nullable())
                            .Column<Int64>("CBSUser_Id", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(DirectAssessmentBatchRecord).Name);

            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat([PaymentTypeCode], '_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("PaymentStatus", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("ReceiptNumber", System.Data.DbType.String));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("Month", System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("Year", System.Data.DbType.Int32));
            return 3;
        }


        public int UpdateFrom3()
        {
            return 4;
        }


        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.DropColumn("Origin"));
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("AssessmentType", System.Data.DbType.Int32));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("OriginIdentifier", System.Data.DbType.Int64));
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("TaxPayerCode", System.Data.DbType.String, col => col.WithLength(100)));
            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("DuplicateComposite", System.Data.DbType.String, col => col.WithLength(100)));
            string tableName = SchemaBuilder.TableDbName(typeof(DirectAssessmentBatchRecord).Name);

            string queryString = string.Format("CREATE UNIQUE INDEX CHECK_FOR_DUPLICATE_COMPOSITE_IF_NOT_NULL ON [dbo].[{0}](DuplicateComposite) WHERE DuplicateComposite IS NOT NULL ", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("Invoice_Id", System.Data.DbType.Int64, col => col.Nullable()));
            return 9;
        }

        public int UpdateFrom9()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentBatchRecord).Name, table => table.AddColumn("InvoiceItem_Id", System.Data.DbType.Int64, col => col.Nullable()));
            return 10;
        }

    }
}