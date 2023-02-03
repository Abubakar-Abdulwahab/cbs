using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSExpenditureHeadMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSExpenditureHead).Name,
                table => table
                    .Column<int>(nameof(PSSExpenditureHead.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSExpenditureHead.Name), column => column.NotNull().WithLength(250).Unique())
                    .Column<string>(nameof(PSSExpenditureHead.Code), column => column.NotNull().WithLength(20).Unique())
                    .Column<bool>(nameof(PSSExpenditureHead.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<int>(nameof(PSSExpenditureHead.LastUpdatedBy) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSExpenditureHead.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSExpenditureHead.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}