using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlement).Name,
                table => table
                    .Column<int>(nameof(PSSSettlement.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSSettlement.Name), column => column.NotNull().WithLength(200).Unique())
                    .Column<int>(nameof(PSSSettlement.SettlementRule) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlement.Service) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlement.IsActive), column => column.NotNull().WithDefault(false))
                    .Column<bool>(nameof(PSSSettlement.HasCommandSplits), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSSettlement.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlement.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}