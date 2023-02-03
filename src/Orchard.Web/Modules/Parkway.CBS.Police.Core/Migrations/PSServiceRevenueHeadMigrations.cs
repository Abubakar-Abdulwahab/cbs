using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceRevenueHeadMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceRevenueHead).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Service_Id", column => column.NotNull())
                    .Column<int>("RevenueHead_Id", column => column.NotNull())
                    .Column<int>("ApplicationRequestStage", column => column.NotNull())
                    .Column<string>("Description", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRevenueHead).Name, table => table.AddColumn("FlowDefinitionLevel_Id", System.Data.DbType.Int32, column => column.Nullable()));
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRevenueHead).Name);

            string queryString = string.Format("UPDATE {0} SET [FlowDefinitionLevel_Id] = [ApplicationRequestStage]", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN FlowDefinitionLevel_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }

        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRevenueHead).Name);

            string queryString = string.Format("UPDATE {0} SET [FlowDefinitionLevel_Id] = [ApplicationRequestStage]", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN FlowDefinitionLevel_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            SchemaBuilder.AlterTable(typeof(PSServiceRevenueHead).Name, table => table.DropColumn("ApplicationRequestStage"));
            return 3;
        }
    }
}