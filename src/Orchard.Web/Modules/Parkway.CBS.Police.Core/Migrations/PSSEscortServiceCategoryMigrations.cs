using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSEscortServiceCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSEscortServiceCategory).Name,
                table => table
                    .Column<int>(nameof(PSSEscortServiceCategory.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSEscortServiceCategory.Parent) + "_Id", column => column.Nullable())
                    .Column<string>(nameof(PSSEscortServiceCategory.Name), column => column.NotNull())
                    .Column<int>(nameof(PSSEscortServiceCategory.MinimumRequiredOfficers), column => column.NotNull())
                    .Column<bool>(nameof(PSSEscortServiceCategory.ShowExtraFields), column => column.NotNull())
                    .Column<bool>(nameof(PSSEscortServiceCategory.IsActive), column => column.NotNull())
                    .Column<bool>(nameof(PSSEscortServiceCategory.IsDeleted), column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}