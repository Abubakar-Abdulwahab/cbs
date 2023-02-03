using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class GeneralBatchReferenceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(GeneralBatchReference).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity()));

            string tableName = SchemaBuilder.TableDbName(typeof(GeneralBatchReference).Name);

            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat('GENREF',case when len(CONVERT([varchar](10),[Id]))<(3) then '_000' else '_' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            SchemaBuilder.AlterTable(typeof(GeneralBatchReference).Name, table => table.AddColumn("AdapterClassName", System.Data.DbType.String, column => column.WithLength(500)));
            SchemaBuilder.AlterTable(typeof(GeneralBatchReference).Name, table => table.AddColumn("CreatedAtUtc", System.Data.DbType.DateTime, column => column.NotNull()));
            SchemaBuilder.AlterTable(typeof(GeneralBatchReference).Name, table => table.AddColumn("UpdatedAtUtc", System.Data.DbType.DateTime, column => column.NotNull()));


            return 1;
        }
    }
}