using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRequestExtractDetailsCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRequestExtractDetailsCategory).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSRequestExtractDetailsCategory.Request) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSRequestExtractDetailsCategory.ExtractDetails) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSRequestExtractDetailsCategory.ExtractCategory) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSRequestExtractDetailsCategory.ExtractSubCategory) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSRequestExtractDetailsCategory.RequestReason), column => column.NotNull().WithLength(1000))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequestExtractDetailsCategory).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSSRequestExtractDetailsCategory_Unique_Constraint UNIQUE([{1}], [{2}], [{3}], [{4}]); ", tableName, nameof(PSSRequestExtractDetailsCategory.Request) + "_Id", nameof(PSSRequestExtractDetailsCategory.ExtractDetails) + "_Id", nameof(PSSRequestExtractDetailsCategory.ExtractCategory) + "_Id", nameof(PSSRequestExtractDetailsCategory.ExtractSubCategory) + "_Id"));

            return 1;
        }
    }
}