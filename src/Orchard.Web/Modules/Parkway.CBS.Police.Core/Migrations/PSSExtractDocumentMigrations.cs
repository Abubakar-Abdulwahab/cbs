using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSExtractDocumentMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSExtractDocument).Name,
                table => table
                    .Column<Int64>(nameof(PSSExtractDocument.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSExtractDocument.ExtractDetails)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSExtractDocument.CommandName), column => column.NotNull())
                    .Column<string>(nameof(PSSExtractDocument.CommandStateName), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSExtractDocument.ApprovalDate), column => column.NotNull())
                    .Column<string>(nameof(PSSExtractDocument.ApprovalNumber), column => column.NotNull())
                    .Column<string>(nameof(PSSExtractDocument.DiarySerialNumber), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSExtractDocument.IncidenDateAndTime), column => column.NotNull())
                    .Column<string>(nameof(PSSExtractDocument.CrossRef), column => column.Nullable().WithLength(10))
                    .Column<string>(nameof(PSSExtractDocument.ExtractCategories), column => column.NotNull())
                    .Column<string>(nameof(PSSExtractDocument.Content), column => column.NotNull().WithLength(1000))
                    .Column<string>(nameof(PSSExtractDocument.DPOName), column => column.Nullable().WithLength(50))
                    .Column<string>(nameof(PSSExtractDocument.DPOSignatureContentType), column => column.Nullable().WithLength(100))
                    .Column<string>(nameof(PSSExtractDocument.DPOSignatureBlob), column => column.Nullable().Unlimited())
                    .Column<string>(nameof(PSSExtractDocument.ExtractDocumentTemplate), column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSExtractDocument).Name, table => table.AddColumn(nameof(PSSExtractDocument.DPORankCode), System.Data.DbType.String, column => column.Nullable().WithLength(20)));
            return 2;
        }


        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSExtractDocument).Name);

            string queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSExtractDocument.DPORankCode)} = 'S/INSPR';");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(20) NOT NULL", tableName, nameof(PSSExtractDocument.DPORankCode));
            SchemaBuilder.ExecuteSql(queryString);
            return 3;
        }
    }
}