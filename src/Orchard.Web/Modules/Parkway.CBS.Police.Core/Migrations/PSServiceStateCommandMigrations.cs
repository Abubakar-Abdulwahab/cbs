using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceStateCommandMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceStateCommand).Name,
                table => table
                    .Column<int>(nameof(PSServiceStateCommand.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSServiceStateCommand.ServiceState) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSServiceStateCommand.Command) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSServiceStateCommand.IsActive), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSServiceStateCommand.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceStateCommand.UpdatedAtUtc), column => column.NotNull())
                );

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE [dbo].[{0}] ADD constraint PSServiceStateCommand_Unique_Constraint UNIQUE([{1}], [{2}]);", SchemaBuilder.TableDbName(typeof(PSServiceStateCommand).Name), nameof(PSServiceStateCommand.ServiceState) + "_Id", nameof(PSServiceStateCommand.Command) + "_Id"));

            return 1;
        }

    }
}