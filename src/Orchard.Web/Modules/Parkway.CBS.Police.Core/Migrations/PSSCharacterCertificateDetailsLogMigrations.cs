using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSCharacterCertificateDetailsLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSCharacterCertificateDetailsLog).Name,
                table => table
                    .Column<Int64>(nameof(PSSCharacterCertificateDetailsLog.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSCharacterCertificateDetailsLog.Request)+"_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSCharacterCertificateDetailsLog.PSSCharacterCertificateDetails)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.Reason)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.ReasonValue), column => column.NotNull())
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.Tribe)+"_Id", column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.TribeValue), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.PlaceOfBirth), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsLog.DateOfBirth), column => column.NotNull())
                    .Column<bool>(nameof(PSSCharacterCertificateDetailsLog.PreviouslyConvicted), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.PreviousConvictionHistory), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.PassportNumber), column => column.Nullable().WithLength(50))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.PlaceOfIssuance), column => column.Nullable().WithLength(200))
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsLog.DateOfIssuance), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.RefNumber), column => column.Nullable().WithLength(100))
                    .Column<bool>(nameof(PSSCharacterCertificateDetailsLog.IsBiometricEnrolled), column => column.NotNull().WithDefault(false))
                    .Column<bool>(nameof(PSSCharacterCertificateDetailsLog.IsApplicantInvitedForCapture), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsLog.CaptureInvitationDate), column => column.Nullable())
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.RequestType) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.StateOfOrigin) + "_Id", column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.StateOfOriginValue), column => column.Nullable().WithLength(100))
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsLog.BiometricCaptureDueDate), column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.CPCCRServiceNumber), column => column.Nullable().WithLength(50))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.CPCCRName), column => column.Nullable().WithLength(100))
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.CPCCRAddedBy) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.DestinationCountry) + "_Id", column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.DestinationCountryValue), column => column.Nullable().WithLength(255))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.CPCCRRankName), column => column.Nullable().WithLength(50))
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.CPCCRRankCode), column => column.Nullable().WithLength(20))
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.CountryOfOrigin) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.CountryOfOriginValue), column => column.NotNull())
                    .Column<int>(nameof(PSSCharacterCertificateDetailsLog.CountryOfPassport) + "_Id", column => column.Nullable())
                    .Column<string>(nameof(PSSCharacterCertificateDetailsLog.CountryOfPassportValue), column => column.Nullable())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetailsLog.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            try
            {
                var queryString = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetailsLog)} " +
                    $"({nameof(PSSCharacterCertificateDetailsLog.PSSCharacterCertificateDetails)}_Id, {nameof(PSSCharacterCertificateDetailsLog.Request)}_Id, {nameof(PSSCharacterCertificateDetailsLog.Reason)}_Id, {nameof(PSSCharacterCertificateDetailsLog.ReasonValue)}, {nameof(PSSCharacterCertificateDetailsLog.Tribe)}_Id, {nameof(PSSCharacterCertificateDetailsLog.TribeValue)}, {nameof(PSSCharacterCertificateDetailsLog.PlaceOfBirth)}, {nameof(PSSCharacterCertificateDetailsLog.DateOfBirth)}, {nameof(PSSCharacterCertificateDetailsLog.PreviouslyConvicted)}, {nameof(PSSCharacterCertificateDetailsLog.PreviousConvictionHistory)}, {nameof(PSSCharacterCertificateDetailsLog.PassportNumber)}, {nameof(PSSCharacterCertificateDetailsLog.PlaceOfIssuance)}, {nameof(PSSCharacterCertificateDetailsLog.DateOfIssuance)}, {nameof(PSSCharacterCertificateDetailsLog.RefNumber)}, {nameof(PSSCharacterCertificateDetailsLog.IsBiometricEnrolled)}, {nameof(PSSCharacterCertificateDetailsLog.IsApplicantInvitedForCapture)}, {nameof(PSSCharacterCertificateDetailsLog.CaptureInvitationDate)}, {nameof(PSSCharacterCertificateDetailsLog.RequestType)}_Id, {nameof(PSSCharacterCertificateDetailsLog.StateOfOrigin)}_Id, {nameof(PSSCharacterCertificateDetailsLog.StateOfOriginValue)}, {nameof(PSSCharacterCertificateDetailsLog.BiometricCaptureDueDate)}, {nameof(PSSCharacterCertificateDetailsLog.CPCCRServiceNumber)}, {nameof(PSSCharacterCertificateDetailsLog.CPCCRName)}, {nameof(PSSCharacterCertificateDetailsLog.CPCCRAddedBy)}_Id, {nameof(PSSCharacterCertificateDetailsLog.DestinationCountry)}_Id, {nameof(PSSCharacterCertificateDetailsLog.DestinationCountryValue)}, {nameof(PSSCharacterCertificateDetailsLog.CPCCRRankName)}, {nameof(PSSCharacterCertificateDetailsLog.CPCCRRankCode)}, {nameof(PSSCharacterCertificateDetailsLog.CountryOfOrigin)}_Id, {nameof(PSSCharacterCertificateDetailsLog.CountryOfOriginValue)}, {nameof(PSSCharacterCertificateDetailsLog.CountryOfPassport)}_Id, {nameof(PSSCharacterCertificateDetailsLog.CountryOfPassportValue)}, {nameof(PSSCharacterCertificateDetailsLog.CreatedAtUtc)}, {nameof(PSSCharacterCertificateDetailsLog.UpdatedAtUtc)}) " +
                                $"SELECT {nameof(PSSCharacterCertificateDetails.Id)}, {nameof(PSSCharacterCertificateDetails.Request)}_Id, {nameof(PSSCharacterCertificateDetails.Reason)}_Id, {nameof(PSSCharacterCertificateDetails.ReasonValue)}, {nameof(PSSCharacterCertificateDetails.Tribe)}_Id, {nameof(PSSCharacterCertificateDetails.TribeValue)}, {nameof(PSSCharacterCertificateDetails.PlaceOfBirth)}, {nameof(PSSCharacterCertificateDetails.DateOfBirth)}, {nameof(PSSCharacterCertificateDetails.PreviouslyConvicted)}, {nameof(PSSCharacterCertificateDetails.PreviousConvictionHistory)}, {nameof(PSSCharacterCertificateDetails.PassportNumber)}, {nameof(PSSCharacterCertificateDetails.PlaceOfIssuance)}, {nameof(PSSCharacterCertificateDetails.DateOfIssuance)}, {nameof(PSSCharacterCertificateDetails.RefNumber)}, {nameof(PSSCharacterCertificateDetails.IsBiometricEnrolled)}, {nameof(PSSCharacterCertificateDetails.IsApplicantInvitedForCapture)}, {nameof(PSSCharacterCertificateDetails.CaptureInvitationDate)}, {nameof(PSSCharacterCertificateDetails.RequestType)}_Id, {nameof(PSSCharacterCertificateDetails.StateOfOrigin)}_Id, {nameof(PSSCharacterCertificateDetails.StateOfOriginValue)}, {nameof(PSSCharacterCertificateDetails.BiometricCaptureDueDate)}, {nameof(PSSCharacterCertificateDetails.CPCCRServiceNumber)}, {nameof(PSSCharacterCertificateDetails.CPCCRName)}, {nameof(PSSCharacterCertificateDetails.CPCCRAddedBy)}_Id, {nameof(PSSCharacterCertificateDetails.DestinationCountry)}_Id, {nameof(PSSCharacterCertificateDetails.DestinationCountryValue)}, {nameof(PSSCharacterCertificateDetails.CPCCRRankName)}, {nameof(PSSCharacterCertificateDetails.CPCCRRankCode)}, {nameof(PSSCharacterCertificateDetails.CountryOfOrigin)}_Id, {nameof(PSSCharacterCertificateDetails.CountryOfOriginValue)}, {nameof(PSSCharacterCertificateDetails.CountryOfPassport)}_Id, {nameof(PSSCharacterCertificateDetails.CountryOfPassportValue)}, {nameof(PSSCharacterCertificateDetails.CreatedAtUtc)}, {nameof(PSSCharacterCertificateDetails.UpdatedAtUtc)} FROM Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetails)} ";

                SchemaBuilder.ExecuteSql(queryString);

                queryString = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetailsBlobLog)} " +
                    $"({nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetails)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetailsBlob)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.PSSCharacterCertificateDetailsLog)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.Request)}_Id, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographOriginalFileName)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageOriginalFileName)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureOriginalFileName)}, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographFilePath)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageFilePath)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureFilePath)}, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographContentType)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageContentType)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureContentType)}, {nameof(PSSCharacterCertificateDetailsBlobLog.PassportPhotographBlob)}, {nameof(PSSCharacterCertificateDetailsBlobLog.InternationalPassportDataPageBlob)}, {nameof(PSSCharacterCertificateDetailsBlobLog.SignatureBlob)}, {nameof(PSSCharacterCertificateDetailsBlobLog.CreatedAtUtc)}, {nameof(PSSCharacterCertificateDetailsBlobLog.UpdatedAtUtc)}) " +
                                $"SELECT blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PSSCharacterCertificateDetails)}_Id, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.Id)}, deetsLog.{nameof(PSSCharacterCertificateDetailsLog.Id)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.Request)}_Id, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographOriginalFileName)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageOriginalFileName)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureOriginalFileName)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographFilePath)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageFilePath)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureFilePath)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographContentType)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageContentType)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureContentType)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PassportPhotographBlob)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.InternationalPassportDataPageBlob)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.SignatureBlob)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.CreatedAtUtc)}, blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.UpdatedAtUtc)} FROM Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetailsBlob)} blobDeets INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSCharacterCertificateDetailsLog)} deetsLog" +
                                $" ON blobDeets.{nameof(PSSCharacterCertificateDetailsBlob.PSSCharacterCertificateDetails)}_Id = deetsLog.{nameof(PSSCharacterCertificateDetailsLog.PSSCharacterCertificateDetails)}_Id";

                SchemaBuilder.ExecuteSql(queryString);

                return 2;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}