using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ServiceWorkflowDifferentialMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ServiceWorkflowDifferential).Name,
                table => table
                    .Column<int>(nameof(ServiceWorkflowDifferential.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(ServiceWorkflowDifferential.Service) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(ServiceWorkflowDifferential.Name), column => column.NotNull().Unique())
                    .Column<string>(nameof(ServiceWorkflowDifferential.DifferentialModelName), column => column.NotNull())
                    .Column<int>(nameof(ServiceWorkflowDifferential.DifferentialValue), column => column.NotNull())
                    .Column<int>(nameof(ServiceWorkflowDifferential.FlowDefinition) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(ServiceWorkflowDifferential.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<DateTime>(nameof(ServiceWorkflowDifferential.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(ServiceWorkflowDifferential.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(ServiceWorkflowDifferential).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ServiceWorkflowDifferential_Unique_Constraint UNIQUE([{1}], [{2}], [{3}]); ", tableName, nameof(ServiceWorkflowDifferential.Service) + "_Id", nameof(ServiceWorkflowDifferential.DifferentialModelName), nameof(ServiceWorkflowDifferential.DifferentialValue)));

            return 1;
        }

    }
}
