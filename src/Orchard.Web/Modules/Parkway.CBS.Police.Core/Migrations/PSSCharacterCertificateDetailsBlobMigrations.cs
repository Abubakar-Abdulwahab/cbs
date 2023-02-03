using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSCharacterCertificateDetailsBlobMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSCharacterCertificateDetailsBlob).Name,
                table => table
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlob.Id), column => column.PrimaryKey().Identity())
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlob.Request) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlob.PSSCharacterCertificateDetails) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographOriginalFileName), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageOriginalFileName), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.SignatureOriginalFileName), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographFilePath), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageFilePath), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.SignatureFilePath), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographContentType), column => column.NotNull().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageContentType), column => column.Nullable().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.SignatureContentType), column => column.Nullable().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographBlob), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageBlob), column => column.Nullable().Unlimited())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlob.SignatureBlob), column => column.Nullable().Unlimited())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsBlob.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsBlob.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetailsBlob).Name);
            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSSCHARCERTDETSBLOB_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSCharacterCertificateDetailsBlob.PSSCharacterCertificateDetails)}_Id]);";
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }
    }
}