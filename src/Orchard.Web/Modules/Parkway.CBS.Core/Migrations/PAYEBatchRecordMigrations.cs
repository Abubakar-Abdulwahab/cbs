using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchRecordMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBatchRecord).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<int>("Billing_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<string>("FilePath", column => column.Nullable().Unlimited())
                            .Column<string>("FileName", column => column.Nullable().WithLength(400))
                            .Column<string>("AdapterValue", column => column.NotNull().WithLength(100))
                            .Column<Int64>("CBSUser_Id", column => column.NotNull())
                            .Column<int>("AssessmentType", column => column.NotNull())
                            .Column<Int64>("OriginIdentifier", column => column.Nullable())
                            .Column<string>("TaxPayerCode", column => column.Nullable())
                            .Column<string>("PaymentTypeCode", column => column.NotNull())
                            .Column<string>("DuplicateComposite", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PAYEBatchRecord).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat([PaymentTypeCode], '_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            string queryStringComposite = string.Format("CREATE UNIQUE INDEX CHECK_FOR_DUPLICATE_COMPOSITE_IF_NOT_NULL ON [dbo].[{0}](DuplicateComposite) WHERE DuplicateComposite IS NOT NULL ", tableName);
            SchemaBuilder.ExecuteSql(queryStringComposite);

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PAYEBatchRecord).Name, table => table.AddColumn(nameof(PAYEBatchRecord.RevenueHeadSurCharge), System.Data.DbType.Decimal, c => c.WithDefault(0.00m).NotNull()));
            string tableName = SchemaBuilder.TableDbName(typeof(PAYEBatchRecord).Name);
            string queryString = string.Format("UPDATE {0} SET [{1}] = {2}", tableName, nameof(PAYEBatchRecord.RevenueHeadSurCharge), 0.00m);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} decimal(19, 5) NOT NULL", tableName, nameof(PAYEBatchRecord.RevenueHeadSurCharge));
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PAYEBatchRecord).Name, table => table.AddColumn(nameof(PAYEBatchRecord.PaymentCompleted), System.Data.DbType.Boolean, c => c.WithDefault(false).NotNull()));
            SchemaBuilder.AlterTable(typeof(PAYEBatchRecord).Name, table => table.AddColumn(nameof(PAYEBatchRecord.IsActive), System.Data.DbType.Boolean, c => c.WithDefault(true).NotNull()));
            return 3;
        }

    }
}