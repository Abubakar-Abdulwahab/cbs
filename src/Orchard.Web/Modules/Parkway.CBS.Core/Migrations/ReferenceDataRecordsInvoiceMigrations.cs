using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataRecordsInvoiceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataRecordsInvoice).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("ReferenceDataBatch_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<int>("TaxEntityCategory_Id", column => column.NotNull())
                            .Column<Int64>("InvoiceUniqueKey", column => column.NotNull())
                            .Column<string>("InvoiceModel", column => column.Nullable().Unlimited())
                            .Column<string>("InvoiceNumber", column => column.Nullable())
                            .Column<decimal>("InvoiceAmount", column => column.Nullable())
                            .Column<string>("CashflowInvoiceIdentifier", column => column.Nullable().WithLength(100))
                            .Column<Int64>("PrimaryContactId", column => column.Nullable())
                            .Column<Int64>("CashflowCustomerId", column => column.Nullable())
                            .Column<DateTime>("DueDate", column => column.Nullable())
                            .Column<string>("InvoiceDescription", column => column.Nullable().WithLength(500))
                            .Column<string>("InvoiceURL", column => column.Nullable().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}