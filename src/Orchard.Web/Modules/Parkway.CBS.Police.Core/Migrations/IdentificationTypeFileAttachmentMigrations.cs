using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class IdentificationTypeFileAttachmentMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(IdentificationTypeFileAttachment).Name,
                table => table
                    .Column<Int64>(nameof(IdentificationTypeFileAttachment.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(IdentificationTypeFileAttachment.IdentificationType)+"_Id", column => column.NotNull())
                    .Column<Int64>(nameof(IdentificationTypeFileAttachment.TaxEntity)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(IdentificationTypeFileAttachment.OriginalFileName), column => column.NotNull())
                    .Column<string>(nameof(IdentificationTypeFileAttachment.FilePath), column => column.NotNull())
                    .Column<string>(nameof(IdentificationTypeFileAttachment.ContentType), column => column.NotNull().WithLength(100))
                    .Column<string>(nameof(IdentificationTypeFileAttachment.Blob), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(IdentificationTypeFileAttachment.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}