using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBusinessTypeAndSizeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBusinessTypeAndSize).Name,
                table => table
                            .Column<int>(nameof(PAYEBusinessTypeAndSize.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(PAYEBusinessTypeAndSize.PAYEBusinessType) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(PAYEBusinessTypeAndSize.PAYEBusinessSize) + "_Id", column => column.NotNull())
                            .Column<decimal>(nameof(PAYEBusinessTypeAndSize.Amount), column => column.NotNull())
                            .Column<int>(nameof(PAYEBusinessTypeAndSize.AddedBy) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBusinessTypeAndSize.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBusinessTypeAndSize.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}