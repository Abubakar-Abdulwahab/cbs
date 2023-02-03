using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CommandCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CommandCategory).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Name", column => column.NotNull().Unique())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(CommandCategory).Name, table => table.AddColumn("CategoryLevel", System.Data.DbType.Int32));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(CommandCategory).Name, table => table.AddColumn(nameof(CommandCategory.LeftIndex), System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(CommandCategory).Name, table => table.AddColumn(nameof(CommandCategory.RightIndex), System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(CommandCategory).Name, table => table.AddColumn(nameof(CommandCategory.ParentCommandCategory) +"_Id", System.Data.DbType.Int32));

            // string tableName = SchemaBuilder.TableDbName(typeof(EscortAmountChartSheet).Name);
            //string queryString = string.Format("ALTER TABLE {0} add [CompositeUniqueChartItem] as ((concat([Rank_Id],'-',[TaxEntitySubSubCategory_Id],'-',[State_Id],'-',[LGA_Id]))) PERSISTED", tableName);
            //SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }
    }
}