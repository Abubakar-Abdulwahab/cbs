using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceRankingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceRanking).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("RankName", column => column.NotNull().Unique())
                    .Column<int>("RankLevel", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PoliceRanking).Name, table => table.AddColumn("RankAmountRate", System.Data.DbType.Decimal, column => column.WithDefault(0)));
            return 2;
        }


        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PoliceRanking).Name);
            string indexDropQueryString = string.Format("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'DF__Parkway_C__RankA__34E8D562') DROP INDEX [dbo].[{0}].DF__Parkway_C__RankA__34E8D562", tableName);
            SchemaBuilder.ExecuteSql(indexDropQueryString);
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PoliceRanking).Name, table => table.AddColumn("ExternalDataRankId", System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PoliceRanking).Name, table => table.AddColumn("ExternalDataCode", System.Data.DbType.String, column => column.Nullable()));
            return 4;
        }


        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PoliceRanking).Name, table => table.DropColumn("RankLevel"));

            string tableName = SchemaBuilder.TableDbName(typeof(PoliceRanking).Name);

            string queryStringFile = string.Format("ALTER TABLE {0} ADD [RankLevel] as ([Id]) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryStringFile);

            return 5;
        }

    }
}