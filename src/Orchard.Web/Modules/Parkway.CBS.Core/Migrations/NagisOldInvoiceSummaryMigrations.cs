using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NagisOldInvoiceSummaryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NagisOldInvoiceSummary).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("NagisDataBatch_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<int>("RevenueHead_Id", column => column.Nullable())
                            .Column<int>("MDAId", column => column.Nullable())
                            .Column<int>("ExpertSystemId", column => column.Nullable())
                            .Column<int>("StatusId", column => column.Nullable())
                            .Column<int>("TaxEntityCategory_Id", column => column.Nullable())
                            .Column<decimal>("TotalAmount", column => column.NotNull())
                            .Column<decimal>("AmountDue", column => column.NotNull())
                            .Column<int>("GroupId", column => column.NotNull())
                            .Column<string>("NagisInvoiceNumber", column => column.NotNull().WithLength(50))
                            .Column<int>("NumberOfItems", column => column.NotNull())
                            .Column<string>("InvoiceUniqueKey", column => column.Nullable())
                            .Column<string>("InvoiceNumber", column => column.Nullable())
                            .Column<string>("InvoiceDescription", column => column.Nullable().WithLength(255))
                            .Column<string>("CashflowInvoiceIdentifier", column => column.Nullable().WithLength(100))
                            .Column<Int64>("PrimaryContactId", column => column.Nullable())
                            .Column<Int64>("CashflowCustomerId", column => column.Nullable())
                            .Column<DateTime>("DueDate", column => column.Nullable())
                            .Column<string>("InvoiceURL", column => column.Nullable().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}