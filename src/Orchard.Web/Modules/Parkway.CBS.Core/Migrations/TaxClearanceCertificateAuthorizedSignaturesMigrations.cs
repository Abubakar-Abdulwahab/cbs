using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxClearanceCertificateAuthorizedSignaturesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxClearanceCertificateAuthorizedSignatures).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("TCCAuthorizedSignatoryId", column => column.NotNull())
                            .Column<string>("OriginalFileName", column => column.NotNull())
                            .Column<string>("FilePath", column => column.NotNull())
                            .Column<string>("ContentType", column => column.NotNull().WithLength(100))
                            .Column<string>("TCCAuthorizedSignatureBlob", column => column.NotNull().Unlimited())
                            .Column<bool>("IsActive", column => column.NotNull())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("Owner_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

    }
}