using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceRequestFlowDefinitionLevelMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceRequestFlowDefinitionLevel).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Definition_Id", column => column.NotNull())
                    .Column<int>("Position", column => column.NotNull())
                    .Column<string>("PositionName", column => column.NotNull())
                    .Column<string>("PositionDescription", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRequestFlowDefinitionLevel).Name);

            string queryString = string.Format("INSERT INTO {0} ([Definition_Id], [Position], [PositionName], [PositionDescription], [CreatedAtUtc], [UpdatedAtUtc]) VALUES (1,1,'Application and Request Fee', 'Application and Request Fee', '{1}', '{1}')", tableName, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            SchemaBuilder.ExecuteSql(queryString);
            queryString = string.Format("INSERT INTO {0} ([Definition_Id], [Position], [PositionName], [PositionDescription], [CreatedAtUtc], [UpdatedAtUtc]) VALUES (2,1,'Application/Processing Fee', 'Application/Processing Fee', '{1}', '{1}')", tableName, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            SchemaBuilder.ExecuteSql(queryString);
            queryString = string.Format("INSERT INTO {0} ([Definition_Id], [Position], [PositionName], [PositionDescription], [CreatedAtUtc], [UpdatedAtUtc]) VALUES (2,2,'Request Fee', 'Request Fee', '{1}', '{1}')", tableName, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.AddColumn("WorkFlowActionValue", System.Data.DbType.Int32, column => column.Nullable()));
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.AddColumn("AssignedApprover_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.AddColumn("ContactEmail", System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.AddColumn("ContactPhoneNumber", System.Data.DbType.String, column => column.Nullable()));
            return 4;
        }


        public int UpdateFrom4()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRequestFlowDefinitionLevel).Name);
            string queryString = string.Format("UPDATE {0} SET [AssignedApprover_Id] = 2, [ContactEmail] = {1}, [ContactPhoneNumber] = {2} WHERE AssignedApprover_Id IS NULL", tableName, "'stephenfregene@gmail.com'", "07030695875");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN AssignedApprover_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN ContactEmail nvarchar(255) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN ContactPhoneNumber nvarchar(255) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 5;
        }


        public int UpdateFrom5()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRequestFlowDefinitionLevel).Name);
            string queryString = string.Format("UPDATE {0} SET [ContactPhoneNumber] = {1} WHERE  ContactPhoneNumber = '7030695875'", tableName, "'07030695875'");
            SchemaBuilder.ExecuteSql(queryString);
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.AddColumn(nameof(PSServiceRequestFlowDefinitionLevel.ApprovalButtonName), System.Data.DbType.String, column => column.NotNull().WithDefault("Submit")));
            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.DropColumn("AssignedApprover_Id"));
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.DropColumn("ContactEmail"));
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.DropColumn("ContactPhoneNumber"));
            return 8;
        }


        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowDefinitionLevel).Name, table => table.AddColumn(nameof(PSServiceRequestFlowDefinitionLevel.PartialName), System.Data.DbType.String, column => column.Nullable()));
            return 9;
        }


        public int UpdateFrom9()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRequestFlowDefinitionLevel).Name);
            string queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_PSService AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id WHERE T2.ServiceType = {1}", "ApprovalPartial\\\\Escort", (int)Models.Enums.PSSServiceTypeDefinition.Escort);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_ServiceWorkflowDifferential AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id INNER JOIN Parkway_CBS_Police_Core_PSService AS T3 ON T2.Service_Id = T3.Id WHERE T3.ServiceType = {1}", "ApprovalPartial\\\\Escort", (int)Models.Enums.PSSServiceTypeDefinition.Escort);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_PSService AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id WHERE T2.ServiceType = {1}", "ApprovalPartial\\\\Extract", (int)Models.Enums.PSSServiceTypeDefinition.Extract);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_ServiceWorkflowDifferential AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id INNER JOIN Parkway_CBS_Police_Core_PSService AS T3 ON T2.Service_Id = T3.Id WHERE T3.ServiceType = {1}", "ApprovalPartial\\\\Extract", (int)Models.Enums.PSSServiceTypeDefinition.Extract);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_PSService AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id WHERE T2.ServiceType = {1}", "ApprovalPartial\\\\CharacterCertificate", (int)Models.Enums.PSSServiceTypeDefinition.CharacterCertificate);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_ServiceWorkflowDifferential AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id INNER JOIN Parkway_CBS_Police_Core_PSService AS T3 ON T2.Service_Id = T3.Id WHERE T3.ServiceType = {1}", "ApprovalPartial\\\\CharacterCertificate", (int)Models.Enums.PSSServiceTypeDefinition.CharacterCertificate);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_PSService AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id WHERE T2.ServiceType = {1}", "ApprovalPartial\\\\Generic", (int)Models.Enums.PSSServiceTypeDefinition.GenericPoliceServices);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE T1 SET T1.PartialName = '{0}' FROM Parkway_CBS_Police_Core_PSServiceRequestFlowDefinitionLevel AS T1 INNER JOIN Parkway_CBS_Police_Core_ServiceWorkflowDifferential AS T2 ON T1.Definition_Id = T2.FlowDefinition_Id INNER JOIN Parkway_CBS_Police_Core_PSService AS T3 ON T2.Service_Id = T3.Id WHERE T3.ServiceType = {1}", "ApprovalPartial\\\\Generic", (int)Models.Enums.PSSServiceTypeDefinition.GenericPoliceServices);
            SchemaBuilder.ExecuteSql(queryString);

            return 10;
        }
    }
}