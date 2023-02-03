using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class SettlementTaskLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SettlementBatch).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ProcessStage", column => column.NotNull().WithDefault(0))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );            

            //string tableName = SchemaBuilder.TableDbName(typeof(SettlementTaskLog).Name);
            ////string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as Format(CreatedAtUtc, 'dd/MM/yyyy') PERSISTED", tableName);
            //string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as [Id] PERSISTED", tableName);
            //SchemaBuilder.ExecuteSql(queryString);

            //string unqiueQuery = string.Format("ALTER TABLE [dbo].[{0}] ADD constraint BatchRef_Unique_Constraint UNIQUE([BatchRef]); ", tableName);
            //SchemaBuilder.ExecuteSql(unqiueQuery);

            //SchemaBuilder.AlterTable(typeof(SettlementTaskLog).Name, table => table.AddColumn("ProcessStage", System.Data.DbType.Int32, cmd => cmd.NotNull().WithDefault(0)));

            return 1;
        }
    }
}