using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ProposedEscortOfficerSelectionTrackingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ProposedEscortOfficerSelectionTracking).Name,
                table => table
                    .Column<Int64>(nameof(ProposedEscortOfficerSelectionTracking.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(ProposedEscortOfficerSelectionTracking.EscortDetails)+"_Id", column => column.NotNull())
                    .Column<Int64>(nameof(ProposedEscortOfficerSelectionTracking.OfficerLog)+"_Id", column => column.NotNull())
                    .Column<decimal>(nameof(ProposedEscortOfficerSelectionTracking.EscortRankRate), column => column.NotNull())
                    .Column<bool>(nameof(ProposedEscortOfficerSelectionTracking.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<string>(nameof(ProposedEscortOfficerSelectionTracking.Reference), column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}