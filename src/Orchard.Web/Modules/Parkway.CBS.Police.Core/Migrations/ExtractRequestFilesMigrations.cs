using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ExtractRequestFilesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExtractRequestFiles).Name,
                table => table
                            .Column<Int64>(nameof(ExtractRequestFiles.Id), column => column.PrimaryKey().Identity())
                            .Column<Int64>(nameof(ExtractRequestFiles.ExtractDetails)+"_Id", column => column.NotNull())
                            .Column<string>(nameof(ExtractRequestFiles.FileName), column => column.NotNull())
                            .Column<string>(nameof(ExtractRequestFiles.FilePath), column => column.NotNull())
                            .Column<string>(nameof(ExtractRequestFiles.ContentType), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(ExtractRequestFiles.Blob), column => column.NotNull().Unlimited())
                            .Column<DateTime>(nameof(ExtractRequestFiles.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(ExtractRequestFiles.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}