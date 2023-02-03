using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSCharacterCertificateRequestTypeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSCharacterCertificateRequestType).Name,
                table => table
                            .Column<int>(nameof(PSSCharacterCertificateRequestType.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(PSSCharacterCertificateRequestType.Name), column => column.NotNull().Unique())
                            .Column<bool>(nameof(PSSCharacterCertificateRequestType.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(PSSCharacterCertificateRequestType.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSSCharacterCertificateRequestType.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSCharacterCertificateRequestType).Name);

            string queryString = string.Format("INSERT INTO {0}(" + nameof(PSSCharacterCertificateRequestType.Name) + "," + nameof(PSSCharacterCertificateRequestType.IsActive) + "," + nameof(PSSCharacterCertificateRequestType.CreatedAtUtc) + "," + nameof(PSSCharacterCertificateRequestType.UpdatedAtUtc) + ") VALUES('Domestic', '1', GETDATE(), GETDATE()), ('International', '1', GETDATE(), GETDATE())", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }
    }
}