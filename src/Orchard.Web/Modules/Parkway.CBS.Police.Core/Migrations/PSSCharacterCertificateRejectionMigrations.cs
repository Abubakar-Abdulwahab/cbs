using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSCharacterCertificateRejectionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSCharacterCertificateRejection).Name,
                table => table
                    .Column<Int64>(nameof(PSSCharacterCertificateRejection.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSCharacterCertificateRejection.PSSCharacterCertificateDetails) + "_Id", column => column.NotNull().Unique())
                    .Column<string>(nameof(PSSCharacterCertificateRejection.NameOfCentralRegistrar), column => column.Nullable().WithLength(200))
                    .Column<string>(nameof(PSSCharacterCertificateRejection.CPCCRRankCode), column => column.Nullable().WithLength(20))
                    .Column<string>(nameof(PSSCharacterCertificateRejection.PassportPhotoBlob), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSCharacterCertificateRejection.PassportPhotoContentType), column => column.NotNull().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateRejection.RefNumber), column => column.NotNull().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificateRejection.CustomerName), column => column.NotNull().WithLength(200))
                    .Column<string>(nameof(PSSCharacterCertificateRejection.PassportNumber), column => column.NotNull().WithLength(50))
                    .Column<string>(nameof(PSSCharacterCertificateRejection.PlaceOfIssuance), column => column.NotNull().WithLength(200))
                    .Column<DateTime>(nameof(PSSCharacterCertificateRejection.DateOfIssuance), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateRejection.DateOfRejection), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateRejection.ReasonForInquiry), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateRejection.DestinationCountry), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificateRejection.CharacterCertificateRejectionTemplate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateRejection.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificateRejection.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateRejection).Name);

            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(200) NULL", tableName, nameof(PSSCharacterCertificateRejection.PlaceOfIssuance));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(50) NULL", tableName, nameof(PSSCharacterCertificateRejection.PassportNumber));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} datetime NULL", tableName, nameof(PSSCharacterCertificateRejection.DateOfIssuance));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(255) NULL", tableName, nameof(PSSCharacterCertificateRejection.DestinationCountry));
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }


        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateRejection).Name);
            string sourceTableName = "Parkway_CBS_Core_" + typeof(CBS.Core.Models.Country).Name;

            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificateRejection).Name, table => table.AddColumn(nameof(PSSCharacterCertificateRejection.CountryOfPassport), System.Data.DbType.String, column => column.Nullable()));


            string queryString = string.Format($"UPDATE T1 SET T1.{nameof(PSSCharacterCertificateRejection.CountryOfPassport)} = T2.{nameof(CBS.Core.Models.Country.Name)}  FROM {tableName} AS T1 INNER JOIN {sourceTableName} AS T2 ON T2.{nameof(CBS.Core.Models.Country.Name)} = 'nigeria' WHERE T1.{nameof(PSSCharacterCertificateRejection.PassportNumber)} IS NOT NULL;");

            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }

    }
}