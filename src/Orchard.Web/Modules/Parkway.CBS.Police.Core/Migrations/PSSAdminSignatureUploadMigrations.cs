using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSAdminSignatureUploadMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSAdminSignatureUpload).Name,
                table => table
                    .Column<int>(nameof(PSSAdminSignatureUpload.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSAdminSignatureUpload.AddedBy)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSAdminSignatureUpload.SignatureBlob), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSAdminSignatureUpload.SignatureFileName), column => column.NotNull())
                    .Column<string>(nameof(PSSAdminSignatureUpload.SignatureFilePath), column => column.NotNull())
                    .Column<string>(nameof(PSSAdminSignatureUpload.SignatureContentType), column => column.NotNull().WithLength(100))
                    .Column<bool>(nameof(PSSAdminSignatureUpload.IsActive), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSAdminSignatureUpload.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSAdminSignatureUpload.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}