using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxPayerEnumerationMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxPayerEnumeration).Name,
                table => table
                            .Column<long>(nameof(TaxPayerEnumeration.Id), column => column.PrimaryKey().Identity())
                            .Column<long>(nameof(TaxPayerEnumeration.Employer) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(TaxPayerEnumeration.UploadType), column => column.NotNull())
                            .Column<string>(nameof(TaxPayerEnumeration.UploadTypeCode), column => column.NotNull())
                            .Column<bool>(nameof(TaxPayerEnumeration.UploadedByAdmin), column => column.NotNull())
                            .Column<long>(nameof(TaxPayerEnumeration.Admin) + "_Id", column => column.Nullable())
                            .Column<bool>(nameof(TaxPayerEnumeration.UploadedByUser), column => column.NotNull())
                            .Column<long>(nameof(TaxPayerEnumeration.User) + "_Id", column => column.Nullable())
                            .Column<bool>(nameof(TaxPayerEnumeration.IsActive), column => column.Nullable())
                            .Column<int>(nameof(TaxPayerEnumeration.ProcessingStage), column => column.NotNull())
                            .Column<DateTime>(nameof(TaxPayerEnumeration.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(TaxPayerEnumeration.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(TaxPayerEnumeration).Name);
            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat([UploadTypeCode], '_SCHEDULE_',case when len(CONVERT([nvarchar](250),[Id]))<(3) then '000' else '' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(TaxPayerEnumeration).Name, table => table.AddColumn(nameof(TaxPayerEnumeration.FilePath), System.Data.DbType.String, column => column.Nullable().Unlimited()));
            SchemaBuilder.AlterTable(typeof(TaxPayerEnumeration).Name, table => table.AddColumn(nameof(TaxPayerEnumeration.FileName), System.Data.DbType.String, column => column.Nullable().WithLength(400)));
            return 2;
        }
    }
}