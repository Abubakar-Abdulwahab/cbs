using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class LGAMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(LGA).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Name", column => column.NotNull().WithLength(100))
                            .Column<string>("CodeName", column => column.NotNull().WithLength(100))
                            .Column<int>("State_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }

        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(LGA).Name);
            string queryString = string.Format("CREATE NONCLUSTERED INDEX LGA_CODENAME_INDEX ON dbo.{0}(CodeName)", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(LGA).Name, table => table.AddColumn("IsActive", System.Data.DbType.Boolean, x => x.WithDefault(true)));

            string tableName = SchemaBuilder.TableDbName(typeof(LGA).Name);
            string queryString = string.Format("UPDATE {0} SET [{1}] = 1", tableName, nameof(LGA.IsActive));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bit NOT NULL", tableName, nameof(LGA.IsActive));
            SchemaBuilder.ExecuteSql(queryString);
            return 3;
        }

    }
}