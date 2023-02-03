using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class UnreconciledPayePaymentsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(UnreconciledPayePayments).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("ExpertSystem_Id", column => column.NotNull())
                    .Column<string>("PaymentReference", column => column.NotNull())
                    .Column<Int64>("UnReconciledTaxEntity_Id", column => column.NotNull())
                    .Column<Int64>("DirectAssessmentBatchRecord_Id", column => column.Nullable())
                    .Column<Int64>("Receipt_Id", column => column.NotNull())
                    .Column<int>("Month", column => column.NotNull())
                    .Column<int>("Year", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}