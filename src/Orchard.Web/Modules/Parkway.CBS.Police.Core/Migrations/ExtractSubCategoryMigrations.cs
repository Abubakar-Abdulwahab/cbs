using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ExtractSubCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExtractSubCategory).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ExtractCategory_Id", column => column.NotNull())
                    .Column<string>("Name", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(ExtractSubCategory).Name);
            SchemaBuilder.AlterTable(typeof(ExtractSubCategory).Name, table => table.AddColumn("FreeForm", System.Data.DbType.Boolean, column => column.WithDefault(false)));

            string queryString = string.Format("UPDATE {0} SET [FreeForm] = 0", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN FreeForm bit NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
    }
}