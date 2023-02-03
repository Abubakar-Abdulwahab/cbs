using Orchard.Data.Migration;
using Parkway.CBS.POSSAP.Scheduler.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Migrations
{
    public class PSSExternalDataRankStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSExternalDataRankStaging).Name,
                table => table
                            .Column<Int64>(nameof(PSSExternalDataRankStaging.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(PSSExternalDataRankStaging.Name), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(PSSExternalDataRankStaging.Code), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(PSSExternalDataRankStaging.ExternalDataRankId), column => column.NotNull())
                            .Column<Int64>(nameof(PSSExternalDataRankStaging.CallLogForExternalSystem) + "_Id", column => column.NotNull())
                            .Column<bool>(nameof(PSSExternalDataRankStaging.HasError), column => column.NotNull().WithDefault(false))
                            .Column<string>(nameof(PSSExternalDataRankStaging.ErrorMessage), column => column.Nullable().WithLength(500))
                            .Column<DateTime>(nameof(PSSExternalDataRankStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSSExternalDataRankStaging.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSExternalDataRankStaging).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ExternalDataRankStaging_Name_CallLogForExternalSystem_Id_UQ_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSExternalDataRankStaging.Name), nameof(PSSExternalDataRankStaging.CallLogForExternalSystem) + "_Id"));

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ExternalDataRankStaging_Code_CallLogForExternalSystem_Id_UQ_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSExternalDataRankStaging.Code), nameof(PSSExternalDataRankStaging.CallLogForExternalSystem) + "_Id"));

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ExternalDataRankStaging_Id_CallLogForExternalSystem_Id_UQ_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSExternalDataRankStaging.ExternalDataRankId), nameof(PSSExternalDataRankStaging.CallLogForExternalSystem) + "_Id"));

            return 1;
        }
    }
}