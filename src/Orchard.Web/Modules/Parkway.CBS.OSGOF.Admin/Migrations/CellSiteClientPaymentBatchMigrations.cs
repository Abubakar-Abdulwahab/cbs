using System;
using Orchard.Data.Migration;
using Parkway.CBS.OSGOF.Admin.Models;

namespace Parkway.CBS.OSGOF.Admin.Migrations
{
    public class CellSiteClientPaymentBatchMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CellSiteClientPaymentBatch).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<string>("FilePath", column => column.Nullable().Unlimited())
                            .Column<string>("Template", column => column.Nullable().WithLength(100))
                            .Column<bool>("ErrorOccurred", column => column.WithDefault(false).NotNull())
                            .Column<string>("ErrorMessage", column => column.Nullable())
                            //.Column<decimal>("Amount", column => column.Nullable())
                            .Column<decimal>("PercentageProgress", column => column.NotNull())
                            .Column<int>("TotalNoOfRowsProcessed", column => column.Nullable())
                            .Column<string>("Type", column => column.NotNull())
                            .Column<string>("Origin", column => column.NotNull())
                            .Column<Int64>("CBSUser_Id", column => column.NotNull())
                            .Column<string>("FileName", column => column.Nullable().WithLength(250))
                            .Column<bool>("InvoiceConfirmed", column => column.NotNull().WithDefault(false))
                            .Column<bool>("PaymentStatus", column => column.NotNull().WithDefault(false))
                            .Column<string>("ReceiptNumber", column => column.Nullable())
                            .Column<Int64>("Invoice_Id", column => column.Nullable())
                            .Column<bool>("Processed", column => column.NotNull().WithDefault(false))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(CellSiteClientPaymentBatch).Name,
                    table => table.CreateIndex("FileName", new string[] { "FileName" }));

            string tableName = SchemaBuilder.TableDbName(typeof(CellSites).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat('OSGOF_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED NOT NULL", tableName);
            return 1;
        }
    }
}