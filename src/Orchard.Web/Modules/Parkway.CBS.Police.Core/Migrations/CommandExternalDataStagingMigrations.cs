using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CommandExternalDataStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CommandExternalDataStaging).Name,
                table => table
                            .Column<int>(nameof(CommandExternalDataStaging.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(CommandExternalDataStaging.Name), column => column.NotNull())
                            .Column<string>(nameof(CommandExternalDataStaging.Code), column => column.NotNull())
                            .Column<int>(nameof(CommandExternalDataStaging.CommandCategoryId), column => column.NotNull())
                            .Column<string>(nameof(CommandExternalDataStaging.StateCode), column => column.NotNull())
                            .Column<string>(nameof(CommandExternalDataStaging.LGACode), column => column.NotNull())
                            .Column<string>(nameof(CommandExternalDataStaging.Address), column => column.NotNull())
                            .Column<int>(nameof(CommandExternalDataStaging.CommandTypeId), column => column.NotNull())
                            .Column<int>(nameof(CommandExternalDataStaging.CallLogForExternalSystemId), column => column.NotNull())
                            .Column<bool>(nameof(CommandExternalDataStaging.HasError), column => column.NotNull().WithDefault(false))
                            .Column<string>(nameof(CommandExternalDataStaging.ErrorMessage), column => column.Nullable().WithLength(500))
                            .Column<string>(nameof(CommandExternalDataStaging.ParentCode), column => column.Nullable().WithLength(50))
                            .Column<int>(nameof(CommandExternalDataStaging.AddedBy), column => column.NotNull())
                            .Column<int>(nameof(CommandExternalDataStaging.LastUpdatedBy), column => column.NotNull())
                            .Column<DateTime>(nameof(CommandExternalDataStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(CommandExternalDataStaging.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(CommandExternalDataStaging).Name, table => table.AddColumn(nameof(CommandExternalDataStaging.ZonalCode), System.Data.DbType.String, column => column.WithLength(20)));
            return 2;
        }


    }
}