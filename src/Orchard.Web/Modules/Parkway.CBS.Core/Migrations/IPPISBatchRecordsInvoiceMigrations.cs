using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class IPPISBatchRecordsInvoiceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(IPPISBatchRecordsInvoice).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("IPPISBatch_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<int>("TaxEntityCategory_Id", column => column.NotNull())
                            .Column<Int64>("IPPISTaxPayerSummary_Id", column => column.NotNull())
                            .Column<string>("InvoiceModel", column => column.NotNull().Unlimited())
                            .Column<string>("InvoiceNumber", column => column.Nullable())
                            .Column<decimal>("InvoiceAmount", column => column.NotNull())
                            .Column<string>("CashflowInvoiceIdentifier", column => column.Nullable().WithLength(100))
                            .Column<Int64>("PrimaryContactId", column => column.Nullable())
                            .Column<Int64>("CashflowCustomerId", column => column.Nullable())
                            .Column<bool>("ErrorOccurred", column => column.NotNull().WithDefault(false))
                            .Column<string>("ErrorCode", column => column.Nullable().WithLength(50))
                            .Column<string>("ErrorMessage", column => column.Nullable().Unlimited())
                            .Column<DateTime>("DueDate", column => column.NotNull())
                            .Column<string>("InvoiceDescription", column => column.WithLength(500))
                            .Column<string>("InvoiceURL", column => column.Nullable().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(IPPISBatchRecordsInvoice).Name);

            string queryString = string.Format("CREATE UNIQUE INDEX NULL_INVOICE_NUMBER ON [dbo].[{0}](InvoiceNumber) WHERE InvoiceNumber IS NOT NULL ", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}