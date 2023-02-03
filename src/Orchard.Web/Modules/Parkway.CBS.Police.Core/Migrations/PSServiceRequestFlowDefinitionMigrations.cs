using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceRequestFlowDefinitionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceRequestFlowDefinition).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("DefinitionName", column => column.NotNull().Unique())
                    .Column<string>("DefinitionDescription", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.WithDefault(true).NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRequestFlowDefinition).Name);
            string queryString = string.Format("INSERT INTO {0} ([DefinitionName], [DefinitionDescription], [IsActive], [CreatedAtUtc], [UpdatedAtUtc]) VALUES ('Extract Request Flow','Extract Request Flow', 1, '{1}', '{1}')", tableName, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            SchemaBuilder.ExecuteSql(queryString);
            queryString = string.Format("INSERT INTO {0} ([DefinitionName], [DefinitionDescription], [IsActive], [CreatedAtUtc], [UpdatedAtUtc]) VALUES ('Generic Request Flow','Generic Request Flow', 1, '{1}', '{1}')", tableName, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            SchemaBuilder.ExecuteSql(queryString);            
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinition).Name, table => table.AddColumn(nameof(PSServiceRequestFlowDefinition.DefinitionType), System.Data.DbType.Int32));

            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRequestFlowDefinition).Name);
            string queryString = string.Format("UPDATE {0} SET [{1}] = {2}", tableName, nameof(PSServiceRequestFlowDefinition.DefinitionType), $"{(int)DefinitionType.Request}");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, nameof(PSServiceRequestFlowDefinition.DefinitionType));
            SchemaBuilder.ExecuteSql(queryString);
            return 3;
        }

    }
}