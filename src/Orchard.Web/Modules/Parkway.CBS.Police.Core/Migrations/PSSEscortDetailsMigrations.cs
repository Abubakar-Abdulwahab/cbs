using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSEscortDetailsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSEscortDetails).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("Request_Id", column => column.NotNull())
                    .Column<string>("RequestReason", column => column.NotNull())
                    .Column<int>("NumberOfOfficers", column => column.NotNull())
                    .Column<int>("PaymentPeriodType", column => column.NotNull())
                    .Column<int>("State_Id", column => column.NotNull())
                    .Column<int>("LGA_Id", column => column.NotNull())
                    .Column<string>("Address", column => column.NotNull().WithLength(500))
                    .Column<int>("DurationNumber", column => column.Nullable())
                    .Column<int>("DurationType", column => column.Nullable())
                    .Column<DateTime>("StartDate", column => column.NotNull())
                    .Column<DateTime>("EndDate", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn("TaxEntitySubCategory_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn("TaxEntitySubSubCategory_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn("Reason_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn("OfficersHaveBeenAssigned", System.Data.DbType.Boolean, column => column.Nullable().WithDefault(false)));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSEscortDetails).Name);

            string queryString = string.Format("UPDATE {0} SET [OfficersHaveBeenAssigned] = 0", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 4;
        }


        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn(nameof(PSSEscortDetails.Settings)+"_Id", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSEscortDetails).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 1", tableName, nameof(PSSEscortDetails.Settings) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, nameof(PSSEscortDetails.Settings) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 5;
        }


        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn(nameof(PSSEscortDetails.ServiceCategory) + "_Id", System.Data.DbType.Int64, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn(nameof(PSSEscortDetails.CategoryType) + "_Id", System.Data.DbType.Int64, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn(nameof(PSSEscortDetails.OriginState) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn(nameof(PSSEscortDetails.OriginLGA) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn(nameof(PSSEscortDetails.OriginAddress), System.Data.DbType.String, column => column.Nullable().WithLength(500)));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSEscortDetails).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 16", tableName, nameof(PSSEscortDetails.ServiceCategory) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bigint NOT NULL", tableName, nameof(PSSEscortDetails.ServiceCategory) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 6;
        }


        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.DropColumn("RequestReason"));
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.DropColumn("Reason_Id"));
            return 7;
        }


        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortDetails).Name, table => table.AddColumn(nameof(PSSEscortDetails.CommandType) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSEscortDetails).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 1", tableName, nameof(PSSEscortDetails.CommandType) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, nameof(PSSEscortDetails.CommandType) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 8;
        }
    }
}