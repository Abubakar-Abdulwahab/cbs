using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class StateModelMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(StateModel).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Name", column => column.NotNull().Unique().WithLength(100))
                            .Column<string>("ShortName", column => column.NotNull().Unique().WithLength(5))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(StateModel).Name, table => table.AddColumn("IsActive", System.Data.DbType.Boolean, x => x.WithDefault(true)));

            string tableName = SchemaBuilder.TableDbName(typeof(StateModel).Name);
            string queryString = string.Format("UPDATE {0} SET [{1}] = 1", tableName, nameof(StateModel.IsActive));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bit NOT NULL", tableName, nameof(StateModel.IsActive));
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }
    }
}