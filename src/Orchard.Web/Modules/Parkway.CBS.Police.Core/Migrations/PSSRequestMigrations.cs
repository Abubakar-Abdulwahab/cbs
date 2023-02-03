using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRequest).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Service_Id", column => column.NotNull())
                    .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                    .Column<int>("Command_Id", column => column.NotNull())
                    .Column<int>("Status", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryString = string.Format("ALTER TABLE {0} add [FileRefNumber] as (rtrim('NPFPSS_')+case when len(rtrim(CONVERT([nvarchar](20),[Id],0)))>(9) then CONVERT([nvarchar](20),[Id],0) else right(replicate((0),(10))+rtrim(CONVERT([nvarchar](10),[Id],0)),(10)) end) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn("ExtractNumber", System.Data.DbType.String, column => column.Nullable()));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn("ServicePrefix", System.Data.DbType.String, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryString = string.Format("UPDATE {0} SET [ServicePrefix] = 'APPR'", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN ServicePrefix nvarchar(5) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.DropColumn("ExtractNumber"));

            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn("ApprovalNumber", System.Data.DbType.String, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryString = string.Format("CREATE UNIQUE INDEX NULL_APPROVAL_NUMBER ON [dbo].[{0}](ApprovalNumber) WHERE ApprovalNumber IS NOT NULL ", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.DropColumn("FileRefNumber"));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryString = string.Format("UPDATE {0} SET [ServicePrefix] = 'APPR'", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            string queryStringFile = string.Format("ALTER TABLE {0} ADD [FileRefNumber] as (rtrim('NPFPSS_'+[ServicePrefix])+case when len(rtrim(CONVERT([nvarchar](20),[Id],(0))))>(9) then CONVERT([nvarchar](20),[Id],(0)) else right(replicate((0),(10))+rtrim(CONVERT([nvarchar](10),[Id],(0))),(10)) end) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryStringFile);

            return 5;
        }


        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn("Reason", System.Data.DbType.String, column => column.Nullable().WithLength(500)));
            return 6;
        }


        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn("FlowDefinitionLevel_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 7;
        }


        public int UpdateFrom7()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryString = string.Format("UPDATE {0} SET [FlowDefinitionLevel_Id] = 2", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE {0} SET [FlowDefinitionLevel_Id] = 1 WHERE ServicePrefix = 'EXT'", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN FlowDefinitionLevel_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 8;
        }

        public int UpdateFrom8()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryString = string.Format("UPDATE {0} SET [FlowDefinitionLevel_Id] = 2", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE {0} SET [FlowDefinitionLevel_Id] = 1 WHERE ServicePrefix = 'EXT'", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN FlowDefinitionLevel_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 9;
        }


        public int UpdateFrom9()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn("ExpectedHash", System.Data.DbType.String, column => column.Nullable()));
            return 10;
        }

        public int UpdateFrom10()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.DropColumn("FileRefNumber"));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryStringFile = string.Format("ALTER TABLE {0} ADD [FileRefNumber] as (rtrim([ServicePrefix])+case when len(rtrim(CONVERT([nvarchar](20),[Id],(0))))>(9) then CONVERT([nvarchar](20),[Id],(0)) else right(replicate((0),(10))+rtrim(CONVERT([nvarchar](10),[Id],(0))),(10)) end) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryStringFile);

            return 11;
        }

        public int UpdateFrom11()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn(nameof(PSSRequest.ContactPersonName), System.Data.DbType.String, col => col.Nullable().WithLength(500)));
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn(nameof(PSSRequest.ContactPersonEmail), System.Data.DbType.String, col => col.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn(nameof(PSSRequest.ContactPersonPhoneNumber), System.Data.DbType.String, col => col.Nullable().WithLength(50)));
            return 12;
        }


        public int UpdateFrom12()
        {
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn(nameof(PSSRequest.TaxEntityProfileLocation)+"_Id", System.Data.DbType.Int32, col => col.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSRequest).Name, table => table.AddColumn(nameof(PSSRequest.CBSUser)+"_Id", System.Data.DbType.Int64, col => col.Nullable()));
            return 13;
        }


        public int UpdateFrom13()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequest).Name);

            string queryString = string.Format("UPDATE T0 SET T0.TaxEntityProfileLocation_Id = T1.Id, T0.CBSUser_Id = T2.Id, T0.UpdatedAtUtc = GETDATE() FROM [dbo].[{0}] AS T0 INNER JOIN [dbo].[Parkway_CBS_Core_TaxEntityProfileLocation] AS T1 ON T0.TaxEntity_Id = T1.TaxEntity_Id INNER JOIN [dbo].[Parkway_CBS_Core_CBSUser] AS T2 ON T0.TaxEntity_Id = T2.TaxEntity_Id WHERE T1.IsDefault = 'true' AND T2.IsAdministrator = 'true';", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN TaxEntityProfileLocation_Id int NOT NULL;", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN CBSUser_Id bigint NOT NULL;", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 14;
        }
    }
}