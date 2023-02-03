using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceServiceRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceServiceRequest).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("RevenueHead_Id", column => column.NotNull())
                    .Column<Int64>("Invoice_Id", column => column.Nullable())
                    .Column<Int64>("Request_Id", column => column.NotNull())
                    .Column<int>("Service_Id", column => column.NotNull())
                    .Column<int>("Status", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PoliceServiceRequest).Name, table => table.AddColumn("IsProcessingFee", System.Data.DbType.Boolean));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PoliceServiceRequest).Name, table => table.AddUniqueConstraint("ServRev", new string[] { "RevenueHead_Id", "Request_Id" }));
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PoliceServiceRequest).Name, table => table.AddColumn("FlowDefinitionLevel_Id", System.Data.DbType.Int32, column => column.Nullable()));
            string tableName = SchemaBuilder.TableDbName(typeof(PoliceServiceRequest).Name);

            string queryString = string.Format("UPDATE {0} SET [FlowDefinitionLevel_Id] = 1", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN FlowDefinitionLevel_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 4;
        }


        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PoliceServiceRequest).Name, table => table.DropColumn("IsProcessingFee"));
            return 5;
        }


        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(PoliceServiceRequest).Name, table => table.DropUniqueConstraint("ServRev"));
            SchemaBuilder.AlterTable(typeof(PoliceServiceRequest).Name, table => table.AddUniqueConstraint("ServRev", new string[] { "RevenueHead_Id", "Request_Id", "FlowDefinitionLevel_Id" }));
            return 6;
        }

    }
}