using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CommandHierarchyMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CommandHierarchy).Name,
                table => table
                            .Column<int>(nameof(CommandHierarchy.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(CommandHierarchy.GroupName), column => column.NotNull().Unique())
                            .Column<string>(nameof(CommandHierarchy.GroupId), column => column.NotNull().Unique())
                            .Column<string>(nameof(CommandHierarchy.AddedBy)+"_Id", column => column.NotNull())
                            .Column<string>(nameof(CommandHierarchy.LastUpdatedBy)+ "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(CommandHierarchy.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(CommandHierarchy.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}