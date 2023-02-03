using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBusinessSizeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBusinessSize).Name,
                table => table
                            .Column<int>(nameof(PAYEBusinessSize.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(PAYEBusinessSize.Size), column => column.NotNull().Unique())
                            .Column<bool>(nameof(PAYEBusinessSize.IsActive), column => column.NotNull())
                            .Column<int>(nameof(PAYEBusinessSize.AddedBy) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBusinessSize.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBusinessSize.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}