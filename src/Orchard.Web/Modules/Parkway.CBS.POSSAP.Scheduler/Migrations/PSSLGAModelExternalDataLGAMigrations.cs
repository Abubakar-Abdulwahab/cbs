using Orchard.Data.Migration;
using Parkway.CBS.POSSAP.Scheduler.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Migrations
{
    public class PSSLGAModelExternalDataLGAMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSLGAModelExternalDataLGA).Name,
                table => table
                            .Column<Int64>(nameof(PSSLGAModelExternalDataLGA.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(PSSLGAModelExternalDataLGA.State) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(PSSLGAModelExternalDataLGA.LGA) + "_Id", column => column.NotNull())
                            .Column<string>(nameof(PSSLGAModelExternalDataLGA.ExternalDataLGACode), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(PSSLGAModelExternalDataLGA.ExternalDataLGAStateCode), column => column.NotNull().WithLength(100))
                            .Column<Int64>(nameof(PSSLGAModelExternalDataLGA.CallLogForExternalSystem) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(PSSLGAModelExternalDataLGA.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSSLGAModelExternalDataLGA.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}