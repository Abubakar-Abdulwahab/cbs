using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CharacterCertificateReasonForInquiryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CharacterCertificateReasonForInquiry).Name,
                table => table
                            .Column<int>(nameof(CharacterCertificateReasonForInquiry.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(CharacterCertificateReasonForInquiry.Name), column => column.NotNull().Unique())
                            .Column<bool>(nameof(CharacterCertificateReasonForInquiry.IsDeleted), column => column.NotNull().WithDefault(false))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(CharacterCertificateReasonForInquiry).Name, table => table.AddColumn(nameof(CharacterCertificateReasonForInquiry.FreeForm), System.Data.DbType.Boolean, col => col.NotNull().WithDefault(false)));
            return 2;
        }
    }
}