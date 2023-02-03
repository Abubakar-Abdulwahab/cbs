﻿using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NagisOldInvoiceResponseStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NagisOldInvoiceResponseStaging).Name,
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
                            .Column<int>("Status", column => column.NotNull())
                            .Column<int>("RevenueHeadId", column => column.NotNull())
                            .Column<int>("MDAId", column => column.NotNull())
                            .Column<int>("ExpertSystemId", column => column.NotNull())
                            .Column<string>("PDFURL", column => column.Nullable().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}