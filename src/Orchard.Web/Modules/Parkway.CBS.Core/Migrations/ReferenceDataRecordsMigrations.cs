using System;
using System.Linq;
using System.Reflection;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataRecordsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            base.SchemaBuilder.CreateTable(typeof(ReferenceDataRecords).Name, table => table
              .Column<Int64>("Id", c => c.Identity().PrimaryKey())
              .Column<Int64>("ReferenceDataBatch_Id", column => column.NotNull())
              .Column<int>("SerialNumberId", column => column.Nullable())
              .Column<Boolean>("IsEvidenceProvided", column => column.NotNull())
              .Column<decimal>("PropertyRentAmount", column => column.NotNull())
              .Column<Boolean>("IsTaxPayerLandlord", column => column.NotNull())
              .Column<DateTime>("CreatedAtUtc")
              .Column<DateTime>("UpdatedAtUtc")
          );

            return 1;
        }

        public int UpdateFrom1()
        {
            var properties = typeof(ReferenceDataRecords).GetProperties().Where(x => (x.Name != "Id" && x.Name != "ReferenceDataBatch_Id" && x.Name != "SerialNumberId" && x.Name != "PropertyRentAmount" && x.Name != "IsEvidenceProvided" && x.Name != "IsTaxPayerNotLandlord" && x.Name != "CreatedAtUtc" && x.Name != "UpdatedAtUtc"));
            foreach (PropertyInfo item in properties)
            {
                if (item.PropertyType.IsEnum)
                {
                    base.SchemaBuilder.AlterTable(typeof(ReferenceDataRecords).Name, tbl => tbl.AddColumn<string>(item.Name));
                }
                else if (typeof(String).IsAssignableFrom(item.PropertyType))
                {
                    base.SchemaBuilder.AlterTable(typeof(ReferenceDataRecords).Name, tbl => tbl.AddColumn<string>(item.Name));
                }
            }

            return 2;
        }
    }
}