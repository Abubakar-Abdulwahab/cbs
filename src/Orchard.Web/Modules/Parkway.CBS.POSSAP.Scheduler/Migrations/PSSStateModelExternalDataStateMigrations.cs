using Orchard.Data.Migration;
using Parkway.CBS.POSSAP.Scheduler.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Migrations
{
    public class PSSStateModelExternalDataStateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSStateModelExternalDataState).Name,
                table => table
                            .Column<Int64>(nameof(PSSStateModelExternalDataState.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(PSSStateModelExternalDataState.State) + "_Id", column => column.NotNull())
                            .Column<string>(nameof(PSSStateModelExternalDataState.ExternalDataStateCode), column => column.NotNull().WithLength(100))
                            .Column<Int64>(nameof(PSSStateModelExternalDataState.CallLogForExternalSystem) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(PSSStateModelExternalDataState.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSSStateModelExternalDataState.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}