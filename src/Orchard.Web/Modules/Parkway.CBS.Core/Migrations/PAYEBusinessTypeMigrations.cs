using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBusinessTypeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBusinessType).Name,
                table => table
                            .Column<int>(nameof(PAYEBusinessType.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(PAYEBusinessType.Name), column => column.NotNull().Unique())
                            .Column<bool>(nameof(PAYEBusinessType.IsActive), column => column.NotNull())
                            .Column<int>(nameof(PAYEBusinessType.AddedBy) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBusinessType.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBusinessType.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}