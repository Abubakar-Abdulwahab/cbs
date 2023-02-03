using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortAmountChartSheetMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortAmountChartSheet).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Rank_Id", column => column.NotNull())
                    .Column<decimal>("Rate", column => column.NotNull())
                    .Column<int>("TaxEntitySubSubCategory_Id", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.NotNull())
                    .Column<int>("AddedBy_Id", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(EscortAmountChartSheet).Name);
            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.AddColumn(nameof(EscortAmountChartSheet.State) + "_Id", System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.AddColumn(nameof(EscortAmountChartSheet.LGA) + "_Id", System.Data.DbType.Int32));

            string queryString = string.Format("UPDATE {0} SET [State_Id] = 1, [LGA_Id] = 1, [UpdatedAtUtc] = CURRENT_TIMESTAMP", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }

        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(EscortAmountChartSheet).Name);
            string queryString = string.Format("ALTER TABLE {0} add [CompositeUniqueChartItem] as ((concat([Rank_Id],'-',[TaxEntitySubSubCategory_Id],'-',[State_Id],'-',[LGA_Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            string unqiueQuery = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ChartItem_Unique_Constraint UNIQUE([CompositeUniqueChartItem]); ", tableName);
            SchemaBuilder.ExecuteSql(unqiueQuery);
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.AddColumn(nameof(EscortAmountChartSheet.CommandType)+ "_Id", System.Data.DbType.Int32, column => column.NotNull()));

            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.DropUniqueConstraint("ChartItem_Unique_Constraint"));

            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.DropColumn("CompositeUniqueChartItem"));

            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.DropColumn("TaxEntitySubSubCategory_Id"));

            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.AddColumn("PSSEscortServiceCategory_Id", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(EscortAmountChartSheet).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint EscortAmountChartSheet_Rank_State_LGA_CommandType_PSSEscortServiceCategory_UQ_Constraint UNIQUE([{1}], [{2}], [{3}], [{4}], [{5}]); ", tableName, nameof(EscortAmountChartSheet.Rank) + "_Id",  nameof(EscortAmountChartSheet.State) + "_Id", nameof(EscortAmountChartSheet.LGA) + "_Id", nameof(EscortAmountChartSheet.CommandType) + "_Id", nameof(EscortAmountChartSheet.PSSEscortServiceCategory) + "_Id"));

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(EscortAmountChartSheet).Name, table => table.AddColumn(nameof(EscortAmountChartSheet.PSSEscortDayType) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));

            return 5;
        }

    }
}