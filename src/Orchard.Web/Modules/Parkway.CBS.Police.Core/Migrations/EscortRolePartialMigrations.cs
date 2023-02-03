using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortRolePartialMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortRolePartial).Name,
                table => table
                            .Column<int>(nameof(EscortRolePartial.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(EscortRolePartial.PartialName), column => column.NotNull())
                            .Column<string>(nameof(EscortRolePartial.ImplementationClass), column => column.NotNull())
                            .Column<int>(nameof(EscortRolePartial.Role) + "_Id", column => column.NotNull())
                            .Column<bool>(nameof(EscortRolePartial.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(EscortRolePartial.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(EscortRolePartial.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}