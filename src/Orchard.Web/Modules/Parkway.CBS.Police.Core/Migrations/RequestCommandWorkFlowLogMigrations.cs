using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class RequestCommandWorkFlowLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RequestCommandWorkFlowLog).Name,
                table => table
                    .Column<Int64>(nameof(RequestCommandWorkFlowLog.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(RequestCommandWorkFlowLog.Request) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(RequestCommandWorkFlowLog.Command) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(RequestCommandWorkFlowLog.DefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(RequestCommandWorkFlowLog.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<DateTime>(nameof(RequestCommandWorkFlowLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(RequestCommandWorkFlowLog.UpdatedAtUtc), column => column.NotNull())
                );

            ///
            string tableName = SchemaBuilder.TableDbName(typeof(RequestCommandWorkFlowLog).Name);

            SchemaBuilder.ExecuteSql($"INSERT INTO [dbo].[{tableName}] ({nameof(RequestCommandWorkFlowLog.Request)}_Id, {nameof(RequestCommandWorkFlowLog.Command)}_Id, {nameof(RequestCommandWorkFlowLog.DefinitionLevel)}_Id, {nameof(RequestCommandWorkFlowLog.CreatedAtUtc)}, {nameof(RequestCommandWorkFlowLog.UpdatedAtUtc)}, {nameof(RequestCommandWorkFlowLog.IsActive)}) " +
                $"SELECT RSL.{nameof(RequestStatusLog.Request)}_Id, RQ.{nameof(PSSRequest.Command)}_Id, RSL.{nameof(RequestStatusLog.FlowDefinitionLevel)}_Id, RSL.{nameof(RequestStatusLog.CreatedAtUtc)}, RSL.{nameof(RequestStatusLog.UpdatedAtUtc)}, 0 " +
                $"FROM (SELECT {nameof(RequestStatusLog.Request)}_Id, {nameof(RequestStatusLog.FlowDefinitionLevel)}_Id, {nameof(RequestStatusLog.CreatedAtUtc)}, {nameof(RequestStatusLog.UpdatedAtUtc)} FROM [dbo].[{SchemaBuilder.TableDbName(typeof(RequestStatusLog).Name)}]) RSL " +
                $"LEFT JOIN [dbo].[{SchemaBuilder.TableDbName(typeof(PSSRequest).Name)}] RQ ON RQ.Id = RSL.{nameof(RequestCommandWorkFlowLog.Request)}_Id");

            SchemaBuilder.ExecuteSql($"UPDATE RCWFL SET [{nameof(RequestCommandWorkFlowLog.IsActive)}] = 1 FROM [dbo].[{tableName}] AS RCWFL INNER JOIN [dbo].[{SchemaBuilder.TableDbName(typeof(PSSRequest).Name)}] AS RQ ON RQ.{nameof(PSSRequest.Id)} = RCWFL.{nameof(RequestCommandWorkFlowLog.Request)}_Id AND RQ.{nameof(PSSRequest.FlowDefinitionLevel)}_Id = RCWFL.{nameof(RequestCommandWorkFlowLog.DefinitionLevel)}_Id AND (RQ.{nameof(PSSRequest.Status)} NOT IN ({(int)PSSRequestStatus.Approved}, {(int)PSSRequestStatus.Rejected}))");


            SchemaBuilder.ExecuteSql($"ALTER TABLE[dbo].[{tableName}] ADD constraint RequestCommandWorkFlowLog_Unique_Constraint UNIQUE([{nameof(RequestCommandWorkFlowLog.Request)}_Id], [{nameof(RequestCommandWorkFlowLog.Command)}_Id], [{nameof(RequestCommandWorkFlowLog.DefinitionLevel)}_Id])");
            ///
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(RequestCommandWorkFlowLog).Name, table => table.AddColumn(nameof(RequestCommandWorkFlowLog.RequestPhaseId), System.Data.DbType.Int32, column => column.WithDefault((int)RequestPhase.New)));
            SchemaBuilder.AlterTable(typeof(RequestCommandWorkFlowLog).Name, table => table.AddColumn(nameof(RequestCommandWorkFlowLog.RequestPhaseName), System.Data.DbType.String, column => column.WithDefault(nameof(RequestPhase.New))));

            string tableName = SchemaBuilder.TableDbName(typeof(RequestCommandWorkFlowLog).Name);
            string queryString = string.Format("UPDATE {0} SET [RequestPhaseId] = {1}, [RequestPhaseName] = '{2}'", tableName, (int)RequestPhase.New, nameof(RequestPhase.New));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN RequestPhaseId int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN RequestPhaseName nvarchar(30) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }

    }
}