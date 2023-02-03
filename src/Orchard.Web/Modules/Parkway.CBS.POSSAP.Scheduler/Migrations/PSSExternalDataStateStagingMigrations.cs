using Orchard.Data.Migration;
using Parkway.CBS.POSSAP.Scheduler.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Migrations
{
    public class PSSExternalDataStateStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSExternalDataStateStaging).Name,
                table => table
                            .Column<Int64>(nameof(PSSExternalDataStateStaging.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(PSSExternalDataStateStaging.Name), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(PSSExternalDataStateStaging.Code), column => column.NotNull().WithLength(100))
                            .Column<Int64>(nameof(PSSExternalDataStateStaging.CallLogForExternalSystem) + "_Id", column => column.NotNull())
                            .Column<bool>(nameof(PSSExternalDataStateStaging.HasErorr), column => column.NotNull().WithDefault(false))
                            .Column<string>(nameof(PSSExternalDataStateStaging.ErrorMessage), column => column.Nullable().WithLength(500))
                            .Column<DateTime>(nameof(PSSExternalDataStateStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSSExternalDataStateStaging.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSExternalDataStateStaging).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ExternalDataStateStaging_Name_CallLogForExternalSystem_Id_UQ_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSExternalDataStateStaging.Name), nameof(PSSExternalDataStateStaging.CallLogForExternalSystem) + "_Id"));

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ExternalDataStateStaging_Code_CallLogForExternalSystem_Id_UQ_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSExternalDataStateStaging.Code), nameof(PSSExternalDataStateStaging.CallLogForExternalSystem) + "_Id"));

            return 1;
        }
    }
}