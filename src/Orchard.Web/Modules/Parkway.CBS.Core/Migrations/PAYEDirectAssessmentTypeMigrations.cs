using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEDirectAssessmentTypeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEDirectAssessmentType).Name,
                table => table
                            .Column<int>(nameof(PAYEDirectAssessmentType.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(PAYEDirectAssessmentType.Name), column => column.NotNull().Unique())
                            .Column<bool>(nameof(PAYEDirectAssessmentType.IsActive), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEDirectAssessmentType.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEDirectAssessmentType.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}