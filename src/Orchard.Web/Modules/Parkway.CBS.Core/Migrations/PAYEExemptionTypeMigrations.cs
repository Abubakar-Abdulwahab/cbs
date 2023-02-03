using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEExemptionTypeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEExemptionType).Name,
                table => table
                            .Column<int>(nameof(PAYEExemptionType.Id), column => column.PrimaryKey().Identity())
                            .Column<bool>(nameof(PAYEExemptionType.IsActive), column => column.NotNull())
                            .Column<string>(nameof(PAYEExemptionType.Name), column => column.Unique().WithLength(200))
                            .Column<DateTime>(nameof(PAYEExemptionType.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEExemptionType.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}