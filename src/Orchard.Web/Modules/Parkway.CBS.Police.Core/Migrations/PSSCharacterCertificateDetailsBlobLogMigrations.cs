using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSCharacterCertificateDetailsBlobLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSCharacterCertificateDetailsBlobLog).Name,
                table => table
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlobLog.Id), column => column.PrimaryKey().Identity())
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlobLog.Request) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetailsBlob) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetailsLog) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetails) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographOriginalFileName), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageOriginalFileName), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.SignatureOriginalFileName), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographFilePath), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageFilePath), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.SignatureFilePath), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographContentType), column => column.NotNull().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageContentType), column => column.Nullable().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.SignatureContentType), column => column.Nullable().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographBlob), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageBlob), column => column.Nullable().Unlimited())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsBlobLog.SignatureBlob), column => column.Nullable().Unlimited())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsBlobLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsBlobLog.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            try
            {
                //var queryString = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetailsBlobLog)} " +
                //    $"({nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetails)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetailsBlob)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetailsLog)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.Request)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographOriginalFileName)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageOriginalFileName)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureOriginalFileName)}, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographFilePath)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageFilePath)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureFilePath)}, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographContentType)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageContentType)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureContentType)}, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographBlob)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageBlob)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureBlob)}, {nameof(PSSCharacterCertificateDetailsBlobLog.CreatedAtUtc)}, {nameof(PSSCharacterCertificateDetailsBlobLog.UpdatedAtUtc)}) " +
                //                $"SELECT blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PSSCharacterCertificateDetails)}_Id, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.Id)}, deetsLog.{nameof(PSSCharacterCertificateDetailsLog.Id)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.Request)}_Id, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographOriginalFileName)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageOriginalFileName)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureOriginalFileName)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographFilePath)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageFilePath)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureFilePath)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographContentType)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageContentType)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureContentType)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographBlob)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageBlob)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureBlob)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.CreatedAtUtc)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.UpdatedAtUtc)} FROM Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetailsBlob)} blobDeets INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetailsLog)} deetsLog" +
                //                $" ON blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PSSCharacterCertificateDetails)}_Id = deetsLog.{nameof(PSSCharacterCertificateDetailsLog.PSSCharacterCertificateDetails)}_Id";

                //SchemaBuilder.ExecuteSql(queryString);

                return 2;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}