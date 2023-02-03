using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSFeePartyAdapterConfigurationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSFeePartyAdapterConfiguration).Name,
                table => table
                    .Column<int>(nameof(PSSFeePartyAdapterConfiguration.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSFeePartyAdapterConfiguration.Name), column => column.NotNull().Unique())
                    .Column<bool>(nameof(PSSFeePartyAdapterConfiguration.IsActive), column => column.NotNull().WithDefault(false))
                    .Column<string>(nameof(PSSFeePartyAdapterConfiguration.ImplementingClass), column => column.Nullable())
                    .Column<DateTime>(nameof(PSSFeePartyAdapterConfiguration.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSFeePartyAdapterConfiguration.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}