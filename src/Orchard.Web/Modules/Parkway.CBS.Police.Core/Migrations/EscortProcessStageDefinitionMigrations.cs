using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortProcessStageDefinitionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortProcessStageDefinition).Name,
                table => table
                            .Column<int>(nameof(EscortProcessStageDefinition.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(EscortProcessStageDefinition.Name), column => column.NotNull().Unique().WithLength(100))
                            .Column<string>(nameof(EscortProcessStageDefinition.StageDescription), column => column.NotNull().Unique().WithLength(250))
                            .Column<int>(nameof(EscortProcessStageDefinition.ParentDefinition) + "_Id", column => column.Nullable())
                            .Column<int>(nameof(EscortProcessStageDefinition.AddedBy) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(EscortProcessStageDefinition.LastUpdatedBy) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(EscortProcessStageDefinition.LevelGroupIdentifier), column => column.NotNull())
                            .Column<bool>(nameof(EscortProcessStageDefinition.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<int>(nameof(EscortProcessStageDefinition.CommandType) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(EscortProcessStageDefinition.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(EscortProcessStageDefinition.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}