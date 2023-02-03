using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSCharacterCertificateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSCharacterCertificate).Name,
                table => table
                    .Column<Int64>(nameof(PSSCharacterCertificate.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSCharacterCertificate.PSSCharacterCertificateDetails) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificate.NameOfCentralRegistrar), column => column.Nullable().WithLength(200))
                    .Column<string>(nameof(PSSCharacterCertificate.ApprovalNumber), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificate.PassportPhotoBlob), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSCharacterCertificate.PassportPhotoContentType), column => column.NotNull().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificate.RefNumber), column => column.NotNull().WithLength(100))
                    .Column<DateTime>(nameof(PSSCharacterCertificate.DateOfApproval), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificate.CustomerName), column => column.NotNull().WithLength(200))
                    .Column<string>(nameof(PSSCharacterCertificate.PassportNumber), column => column.NotNull().WithLength(50))
                    .Column<string>(nameof(PSSCharacterCertificate.PlaceOfIssuance), column => column.NotNull().WithLength(200))
                    .Column<DateTime>(nameof(PSSCharacterCertificate.DateOfIssuance), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificate.ReasonForInquiry), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificate.DestinationCountry), column => column.NotNull())
                    .Column<string>(nameof(PSSCharacterCertificate.CentralRegistrarSignatureContentType), column => column.Nullable().WithLength(100))
                    .Column<string>(nameof(PSSCharacterCertificate.CentralRegistrarSignatureBlob), column => column.Nullable().Unlimited())
                    .Column<string>(nameof(PSSCharacterCertificate.CharacterCertificateTemplate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificate.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSCharacterCertificate.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificate).Name);

            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(50) NULL", tableName, nameof(PSSCharacterCertificate.PlaceOfIssuance));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(10) NULL", tableName, nameof(PSSCharacterCertificate.PassportNumber));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} datetime NULL", tableName, nameof(PSSCharacterCertificate.DateOfIssuance));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(255) NULL", tableName, nameof(PSSCharacterCertificate.DestinationCountry));
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificate).Name, table => table.AddColumn(nameof(PSSCharacterCertificate.CPCCRRankCode), System.Data.DbType.String, column => column.Nullable().WithLength(20)));
            return 3;
        }


        public int UpdateFrom3()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificate).Name);

            string queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSCharacterCertificate.CPCCRRankCode)} = 'S/INSPR';");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(20) NOT NULL", tableName, nameof(PSSCharacterCertificate.CPCCRRankCode));
            SchemaBuilder.ExecuteSql(queryString);
            return 4;
        }


        public int UpdateFrom4()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificate).Name);
            string sourceTableName = SchemaBuilder.TableDbName(typeof(PoliceRanking).Name);

            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificate).Name, table => table.AddColumn(nameof(PSSCharacterCertificate.CPCCRRankName), System.Data.DbType.String, column => column.Nullable()));

            string queryString = string.Format($"UPDATE T1 SET T1.{nameof(PSSCharacterCertificate.CPCCRRankName)} = T2.{nameof(PoliceRanking.RankName)} FROM {tableName} AS T1 INNER JOIN {sourceTableName} AS T2 ON T1.{nameof(PSSCharacterCertificate.CPCCRRankCode)} = T2.{nameof(PoliceRanking.ExternalDataCode)};");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(255) NOT NULL", tableName, nameof(PSSCharacterCertificate.CPCCRRankName));
            SchemaBuilder.ExecuteSql(queryString);
            return 5;
        }


        public int UpdateFrom5()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificate).Name);
            string sourceTableName = "Parkway_CBS_Core_" + typeof(CBS.Core.Models.Country).Name;

            SchemaBuilder.AlterTable(typeof(PSSCharacterCertificate).Name, table => table.AddColumn(nameof(PSSCharacterCertificate.CountryOfPassport), System.Data.DbType.String, column => column.Nullable()));


            string queryString = string.Format($"UPDATE T1 SET T1.{nameof(PSSCharacterCertificate.CountryOfPassport)} = T2.{nameof(CBS.Core.Models.Country.Name)}  FROM {tableName} AS T1 INNER JOIN {sourceTableName} AS T2 ON T2.{nameof(CBS.Core.Models.Country.Name)} = 'nigeria' WHERE T1.{nameof(PSSCharacterCertificate.PassportNumber)} IS NOT NULL;");

            SchemaBuilder.ExecuteSql(queryString);

            return 6;
        }
    }
}