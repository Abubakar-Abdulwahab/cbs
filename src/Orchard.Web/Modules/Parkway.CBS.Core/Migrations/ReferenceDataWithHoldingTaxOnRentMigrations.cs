using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataWithHoldingTaxOnRentMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataWithHoldingTaxOnRent).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("ReferenceDataBatch_Id", column => column.NotNull())
                            .Column<Int64>("ReferenceDataTaxEntityStaging_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<decimal>("PropertyRentAmount", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }

    }
}