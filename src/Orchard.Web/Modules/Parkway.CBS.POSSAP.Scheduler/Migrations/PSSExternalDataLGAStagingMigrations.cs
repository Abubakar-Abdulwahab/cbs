using Orchard.Data.Migration;
using Parkway.CBS.POSSAP.Scheduler.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Migrations
{
    public class PSSExternalDataLGAStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSExternalDataLGAStaging).Name,
                table => table
                            .Column<Int64>(nameof(PSSExternalDataLGAStaging.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(PSSExternalDataLGAStaging.Name), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(PSSExternalDataLGAStaging.Code), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(PSSExternalDataLGAStaging.StateCode), column => column.NotNull().WithLength(100))
                            .Column<Int64>(nameof(PSSExternalDataLGAStaging.CallLogForExternalSystem) + "_Id", column => column.NotNull())
                            .Column<bool>(nameof(PSSExternalDataLGAStaging.HasError), column => column.NotNull().WithDefault(false))
                            .Column<string>(nameof(PSSExternalDataLGAStaging.ErrorMessage), column => column.Nullable().WithLength(500))
                            .Column<DateTime>(nameof(PSSExternalDataLGAStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSSExternalDataLGAStaging.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSExternalDataLGAStaging).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSSExternalDataLGAStaging_Name_CallLogForExternalSystem_Id_StateCode_UQ_Constraint UNIQUE([{1}], [{2}], [{3}]); ", tableName, nameof(PSSExternalDataLGAStaging.Name), nameof(PSSExternalDataLGAStaging.CallLogForExternalSystem) + "_Id", nameof(PSSExternalDataLGAStaging.StateCode)));

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSSExternalDataLGAStaging_Code_CallLogForExternalSystem_Id_UQ_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSExternalDataLGAStaging.Code), nameof(PSSExternalDataLGAStaging.CallLogForExternalSystem) + "_Id"));

            return 1;
        }
    }
}