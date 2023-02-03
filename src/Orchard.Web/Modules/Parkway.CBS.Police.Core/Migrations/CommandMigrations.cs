using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CommandMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Command).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Name", column => column.NotNull().Unique())
                            .Column<string>("Code", column => column.NotNull().Unique())
                            .Column<int>("CommandCategory_Id", column => column.NotNull())
                            .Column<int>("State_Id", column => column.NotNull())
                            .Column<int>("LGA_Id", column => column.NotNull())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<bool>("IsActive", column => column.NotNull().WithDefault(true))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(Command).Name, table => table.AddColumn("Address", System.Data.DbType.String, column => column.WithLength(500)));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(Command).Name, table => table.AddColumn("CommandType_Id", System.Data.DbType.Int32, column => column.Nullable().WithDefault(1)));
            return 3;
        }

        public int UpdateFrom3()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(Command).Name);

            string queryString = string.Format("UPDATE {0} SET [CommandType_Id] = 1", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN CommandType_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(Command).Name, table => table.AddColumn(nameof(Command.ParentCode), System.Data.DbType.String, column => column.Nullable().WithLength(50)));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(Command).Name, table => table.AddColumn(nameof(Command.ZonalCommand) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 6;
        }

    }
}