using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSPresettlementDeductionsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSPresettlementDeductions).Name,
                table => table
                    .Column<Int64>(nameof(PSSPresettlementDeductions.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSPresettlementDeductions.Name), column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductions.SettlementBatch) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductions.Service) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductions.MDA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductions.RevenueHead) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductions.Channel), column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductions.PaymentProvider) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSPresettlementDeductions.Request) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSPresettlementDeductions.Invoice) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PSSPresettlementDeductions.Amount), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSPresettlementDeductions.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSPresettlementDeductions.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

    }
}