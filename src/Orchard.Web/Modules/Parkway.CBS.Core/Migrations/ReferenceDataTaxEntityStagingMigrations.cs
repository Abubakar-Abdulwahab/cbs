using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataTaxEntityStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataTaxEntityStaging).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("ReferenceDataBatch_Id", column => column.NotNull())
                            .Column<Int64>("ReferenceDataRecord_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<string>("Surname", column => column.Nullable().WithLength(50))
                            .Column<string>("Firstname", column => column.Nullable().WithLength(50))
                            .Column<string>("Middlename", column => column.Nullable().WithLength(50))
                            .Column<string>("TIN", column => column.Nullable().WithLength(50))
                            .Column<string>("HouseNo", column => column.Nullable().WithLength(15))
                            .Column<string>("StreetName", column => column.Nullable().WithLength(250))
                            .Column<string>("DbLGAId", column => column.Nullable().WithLength(5))
                            .Column<string>("City", column => column.Nullable().WithLength(250))
                            .Column<string>("EmailAddress", column => column.Nullable().WithLength(100))
                            .Column<string>("PhoneNumber", column => column.NotNull().WithLength(50))
                            .Column<int>("TaxEntityCategory_Id", column => column.NotNull())
                            .Column<decimal>("PropertyRentAmount", column => column.Nullable())
                            .Column<int>("OperationType_Id", column => column.NotNull())
                            .Column<Boolean>("IsEvidenceProvided", column => column.NotNull())
                            .Column<Boolean>("IsTaxPayerLandlord", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}