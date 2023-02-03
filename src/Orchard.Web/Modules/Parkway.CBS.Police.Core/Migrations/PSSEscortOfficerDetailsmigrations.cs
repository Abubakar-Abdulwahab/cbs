using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSEscortOfficerDetailsmigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSEscortOfficerDetails).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("EscortDetail_Id", column => column.NotNull())
                    .Column<Int64>("Request_Id", column => column.NotNull())
                    .Column<Int64>("PoliceRanking_Id", column => column.NotNull())
                    .Column<int>("Quantity", column => column.NotNull())
                    .Column<decimal>("RankAmountRate", column => column.NotNull())
                    .Column<decimal>("TotalAmount", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortOfficerDetails).Name, table => table.DropColumn("EscortDetail_Id"));
            SchemaBuilder.AlterTable(typeof(PSSEscortOfficerDetails).Name, table => table.DropColumn("TotalAmount"));
            string tableName = SchemaBuilder.TableDbName(typeof(PSSEscortOfficerDetails).Name);
            string queryString = string.Format("ALTER TABLE {0} add [TotalAmount] as (([RankAmountRate]*[Quantity])) ", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSSEscortOfficerDetails).Name, table => table.AddColumn(nameof(PSSEscortOfficerDetails.Name), System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSEscortOfficerDetails).Name, table => table.AddColumn(nameof(PSSEscortOfficerDetails.Command)+"_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSEscortOfficerDetails).Name, table => table.AddColumn(nameof(PSSEscortOfficerDetails.IPPISNumber), System.Data.DbType.String, column => column.Nullable()));
            return 3;
        }

    }
}