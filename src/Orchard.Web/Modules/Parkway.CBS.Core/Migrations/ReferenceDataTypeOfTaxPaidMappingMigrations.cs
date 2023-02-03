using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataTypeOfTaxPaidMappingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataTypeOfTaxPaidMapping).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("ReferenceDataRecord_Id", column => column.Nullable())
                            .Column<int>("SerialNumberId", column => column.NotNull())
                            .Column<int>("ReferenceDataTypeOfTaxPaid", column => column.Nullable())
                            .Column<string>("ReferenceDataBatch_Id", column => column.NotNull().WithLength(1000))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}