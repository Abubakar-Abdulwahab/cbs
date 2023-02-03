using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSCharacterCertificateDetailsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSCharacterCertificateDetails).Name,
                table => table
                    .Column<Int64>(nameof(PSSCharacterCertificateDetails.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSCharacterCertificateDetails.Request)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSCharacterCertificateDetails.Reason)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetails.ReasonValue), column => column.NotNull())
                    .Column<int>(nameof(PSSCharacterCertificateDetails.Tribe)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetails.TribeValue), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetails.PlaceOfBirth), column => column.NotNull())
                    .Column<int>("YearOfBirth", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetails.DateOfBirth), column => column.NotNull())
                    .Column<int>(nameof(PSSCharacterCertificateDetails.DestinationCountry)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetails.DestinationCountryValue), column => column.NotNull())
                    .Column<bool>(nameof(PSSCharacterCertificateDetails.PreviouslyConvicted), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateDetails.PreviousConvictionHistory), column => column.Nullable())
                    .Column<string>("PassportPhotographOriginalFileName", column => column.NotNull())
                    .Column<string>("InternationalPassportDataPageOriginalFileName", column => column.NotNull())
                    .Column<string>("SignatureOriginalFileName", column => column.NotNull())
                    .Column<string>("PassportPhotographFilePath", column => column.NotNull())
                    .Column<string>("InternationalPassportDataPageFilePath", column => column.NotNull())
                    .Column<string>("SignatureFilePath", column => column.NotNull())
                    .Column<string>("PassportPhotographContentType", column => column.NotNull().WithLength(100))
                    .Column<string>("InternationalPassportDataPageContentType", column => column.NotNull().WithLength(100))
                    .Column<string>("SignatureContentType", column => column.NotNull().WithLength(100))
                    .Column<string>("PassportPhotographBlob", column => column.NotNull().Unlimited())
                    .Column<string>("InternationalPassportDataPageBlob", column => column.NotNull().Unlimited())
                    .Column<string>("SignatureBlob", column => column.NotNull().Unlimited())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetails.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateDetails.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.PassportNumber), System.Data.DbType.String, column => column.Nullable().WithLength(50)));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.PlaceOfIssuance), System.Data.DbType.String, column => column.Nullable().WithLength(200)));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.DateOfIssuance), System.Data.DbType.DateTime, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.RefNumber), System.Data.DbType.String, column => column.Nullable().WithLength(100)));

            return 2;
        }


        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);

            string queryString = string.Format("UPDATE {0} SET PassportNumber = 'A00000000', PlaceOfIssuance = 'Lagos', DateOfIssuance = GETDATE();", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN PassportNumber nvarchar(10) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN PlaceOfIssuance nvarchar(50) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN DateOfIssuance datetime NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.IsBiometricEnrolled), System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.IsApplicantInvitedForCapture), System.Data.DbType.Boolean, column => column.WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CaptureInvitationDate), System.Data.DbType.DateTime, column => column.Nullable()));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.RequestType)+"_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 6;
        }

        public int UpdateFrom6()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);

            string queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSCharacterCertificateDetails.RequestType) + "_Id"} = 2");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.RequestType) + "_Id"} int NOT NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.PassportNumber)} nvarchar(10) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.PlaceOfIssuance)} nvarchar(50) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.DateOfIssuance)} datetime NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN InternationalPassportDataPageBlob nvarchar(MAX) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN InternationalPassportDataPageFilePath nvarchar(255) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN InternationalPassportDataPageOriginalFileName nvarchar(255) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN InternationalPassportDataPageContentType nvarchar(100) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            return 7;
        }


        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("YearOfBirth"));

            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.StateOfOrigin) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.StateOfOriginValue), System.Data.DbType.String, column => column.Nullable().WithLength(100)));

            return 9;
        }


        public int UpdateFrom9()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);

            string queryString = string.Format($"UPDATE T1 SET T1.{nameof(PSSCharacterCertificateDetails.StateOfOrigin)}_Id = T2.Id, T1.{nameof(PSSCharacterCertificateDetails.StateOfOriginValue)} = T2.Name  FROM Parkway_CBS_Police_Core_PSSCharacterCertificateDetails AS T1 INNER JOIN Parkway_CBS_Core_StateModel AS T2 ON T2.Id = 1 WHERE T1.Id > 0;");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.StateOfOrigin)}_Id int NOT NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.StateOfOriginValue)} nvarchar(100) NOT NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.Tribe)}_Id int NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.TribeValue)} nvarchar(255) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            return 10;
        }

        public int UpdateFrom10()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.BiometricCaptureDueDate), System.Data.DbType.DateTime, column => column.Nullable()));
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);

            string queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN SignatureBlob nvarchar(MAX) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN SignatureFilePath nvarchar(255) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN SignatureOriginalFileName nvarchar(255) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN SignatureContentType nvarchar(100) NULL");
            SchemaBuilder.ExecuteSql(queryString);
            return 11;
        }


        public int UpdateFrom11()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CPCCRServiceNumber), System.Data.DbType.String, column => column.WithLength(50).Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CPCCRName), System.Data.DbType.String, column => column.WithLength(100).Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CPCCRAddedBy) +"_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 12;
        }

        public int UpdateFrom12()
        {

            string destinationTableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetailsBlob).Name);
            string orginTableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);

            string queryString = string.Format("INSERT INTO {0} (PassportPhotographBlob, InternationalPassportDataPageBlob, SignatureBlob, PSSCharacterCertificateDetails_Id, Request_Id, PassportPhotographOriginalFileName, InternationalPassportDataPageOriginalFileName, SignatureOriginalFileName, PassportPhotographFilePath, InternationalPassportDataPageFilePath, SignatureFilePath, PassportPhotographContentType, InternationalPassportDataPageContentType, SignatureContentType, UpdatedAtUtc, CreatedAtUtc) SELECT PassportPhotographBlob, InternationalPassportDataPageBlob, SignatureBlob, Id, Request_Id, PassportPhotographOriginalFileName, InternationalPassportDataPageOriginalFileName, SignatureOriginalFileName, PassportPhotographFilePath, InternationalPassportDataPageFilePath, SignatureFilePath, PassportPhotographContentType, InternationalPassportDataPageContentType, SignatureContentType, UpdatedAtUtc, CreatedAtUtc FROM {1};", destinationTableName, orginTableName);
            SchemaBuilder.ExecuteSql(queryString);

            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("PassportPhotographBlob"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("InternationalPassportDataPageBlob"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("SignatureBlob"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("PassportPhotographOriginalFileName"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("InternationalPassportDataPageOriginalFileName"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("SignatureOriginalFileName"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("PassportPhotographFilePath"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("InternationalPassportDataPageFilePath"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("SignatureFilePath"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("PassportPhotographContentType"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("InternationalPassportDataPageContentType"));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.DropColumn("SignatureContentType"));

            return 13;
        }


        public int UpdateFrom13()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);

            string queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.DestinationCountry)}_Id int NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.DestinationCountryValue)} nvarchar(255) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            return 14;
        }


        public int UpdateFrom14()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CPCCRRankCode), System.Data.DbType.String, column => column.Nullable().WithLength(20)));
            return 15;
        }


        public int UpdateFrom15()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);
            string sourceTableName = SchemaBuilder.TableDbName(typeof(PoliceRanking).Name);

            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CPCCRRankName), System.Data.DbType.String, column => column.Nullable()));


            string queryString = string.Format($"UPDATE T1 SET T1.{nameof(PSSCharacterCertificateDetails.CPCCRRankName)} = T2.{nameof(PoliceRanking.RankName)} FROM {tableName} AS T1 INNER JOIN {sourceTableName} AS T2 ON T1.{nameof(PSSCharacterCertificateDetails.CPCCRRankCode)} = T2.{nameof(PoliceRanking.ExternalDataCode)};");
            SchemaBuilder.ExecuteSql(queryString);
            return 16;
        }


        public int UpdateFrom16()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateDetails).Name);
            string sourceTableName = "Parkway_CBS_Core_" + typeof(CBS.Core.Models.Country).Name;

            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CountryOfOrigin)+"_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CountryOfOriginValue), System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CountryOfPassport)+"_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateDetails).Name, table => table.AddColumn(nameof(PSSCharacterCertificateDetails.CountryOfPassportValue), System.Data.DbType.String, column => column.Nullable()));


            string queryString = string.Format($"UPDATE T1 SET T1.{nameof(PSSCharacterCertificateDetails.CountryOfOrigin)}_Id = T2.{nameof(CBS.Core.Models.Country.Id)}, T1.{nameof(PSSCharacterCertificateDetails.CountryOfOriginValue)} = T2.{nameof(CBS.Core.Models.Country.Name)}  FROM {tableName} AS T1 INNER JOIN {sourceTableName} AS T2 ON T2.{nameof(CBS.Core.Models.Country.Name)} = 'Nigeria';");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"UPDATE T1 SET T1.{nameof(PSSCharacterCertificateDetails.CountryOfPassport)}_Id = T2.{nameof(CBS.Core.Models.Country.Id)}, T1.{nameof(PSSCharacterCertificateDetails.CountryOfPassportValue)} = T2.{nameof(CBS.Core.Models.Country.Name)}  FROM {tableName} AS T1 INNER JOIN {sourceTableName} AS T2 ON T2.{nameof(CBS.Core.Models.Country.Name)} = 'Nigeria' WHERE T1.{nameof(PSSCharacterCertificateDetails.PassportNumber)} IS NOT NULL;");

            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.CountryOfOrigin)}_Id int NOT NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.CountryOfOriginValue)} nvarchar(255) NOT NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.StateOfOrigin)}_Id int NULL");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSCharacterCertificateDetails.StateOfOriginValue)} nvarchar(100) NULL");
            SchemaBuilder.ExecuteSql(queryString);

            return 17;
        }

    }
}