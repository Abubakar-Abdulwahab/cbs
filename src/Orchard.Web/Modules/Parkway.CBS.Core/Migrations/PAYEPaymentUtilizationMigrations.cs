using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEPaymentUtilizationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEPaymentUtilization).Name,
                table => table
                    .Column<Int64>(nameof(PAYEPaymentUtilization.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PAYEPaymentUtilization.PAYEBatchRecord) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PAYEPaymentUtilization.PAYEReceipt) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PAYEPaymentUtilization.UtilizedAmount), column => column.NotNull())
                    .Column<DateTime>(nameof(PAYEPaymentUtilization.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PAYEPaymentUtilization.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}