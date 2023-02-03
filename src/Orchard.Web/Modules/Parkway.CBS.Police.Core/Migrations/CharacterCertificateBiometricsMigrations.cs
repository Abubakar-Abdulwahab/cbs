using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CharacterCertificateBiometricsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CharacterCertificateBiometrics).Name,
                table => table
                    .Column<long>(nameof(CharacterCertificateBiometrics.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(CharacterCertificateBiometrics.UserPartRecord) + "_Id", column => column.NotNull())
                     .Column(nameof(CharacterCertificateBiometrics.PassportImage), System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.RightIndex),System.Data.DbType.String , column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.RightMiddle),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.RightPinky),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.RightRing),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.RightThumb),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.LeftIndex),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.LeftMiddle),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.LeftPinky),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.LeftRing),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column(nameof(CharacterCertificateBiometrics.LeftThumb),System.Data.DbType.String, column => column.NotNull().Unlimited())
                    .Column<long>(nameof(CharacterCertificateBiometrics.CharacterCertificateDetails) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(CharacterCertificateBiometrics.Request) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(CharacterCertificateBiometrics.RegisteredAt), column => column.NotNull())
                    .Column<DateTime>(nameof(CharacterCertificateBiometrics.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(CharacterCertificateBiometrics.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}