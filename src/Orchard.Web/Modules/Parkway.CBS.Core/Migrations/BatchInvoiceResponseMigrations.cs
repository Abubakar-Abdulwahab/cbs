using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class BatchInvoiceResponseMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(BatchInvoiceResponse).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("BatchIdentifier", column => column.NotNull())
                            .Column<Int64>("InvoiceUniqueKey", column => column.NotNull())
                            .Column<string>("InvoiceNumber", column => column.Nullable().Unique())
                            .Column<decimal>("InvoiceAmount", column => column.Nullable())
                            .Column<string>("CashflowInvoiceIdentifier", column => column.Nullable().WithLength(100))
                            .Column<Int64>("PrimaryContactId", column => column.Nullable())
                            .Column<Int64>("CashflowCustomerId", column => column.Nullable())
                            .Column<DateTime>("DueDate", column => column.Nullable())
                            .Column<string>("InvoiceDescription", column => column.Nullable().WithLength(500))
                            .Column<string>("IntegrationPreviewUrl", column => column.Nullable().Unlimited())
                            .Column<int>("Status", column => column.Nullable().Unlimited())
                            .Column<string>("PDFURL", column => column.Nullable().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}