using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class CBSUserTaxEntityProfileLocationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CBSUserTaxEntityProfileLocation).Name,
                table => table
                            .Column<int>(nameof(CBSUserTaxEntityProfileLocation.Id), column => column.PrimaryKey().Identity())
                            .Column<Int64>(nameof(CBSUserTaxEntityProfileLocation.CBSUser)+"_Id", column => column.NotNull())
                            .Column<int>(nameof(CBSUserTaxEntityProfileLocation.TaxEntityProfileLocation)+"_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(CBSUserTaxEntityProfileLocation).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint CBSUserTaxEntityProfileLocation_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(CBSUserTaxEntityProfileLocation.CBSUser) + "_Id", nameof(CBSUserTaxEntityProfileLocation.TaxEntityProfileLocation) + "_Id"));

            return 1;
        }
    }
}