using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PaymentReferenceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PaymentReference).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("Invoice_Id", column => column.NotNull())
                    .Column<int>("PaymentProvider", column => column.NotNull())
                    .Column<string>("InvoiceNumber", column => column.Nullable())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PaymentReference).Name);

            string queryString = string.Format("ALTER TABLE {0} add [ReferenceNumber] as ((concat(substring('EGPBLUSIHAJCFKDMVWQRZNTXYO',case when abs(checksum([Id]))%(26)=(25) then abs(checksum([Id]))%(26)-(2) else abs(checksum([Id]))%(26)+(1) end,(2)),'-',case when len(CONVERT([nvarchar](50),[Id]))<(4) then '0000' else '' end,[Id],[InvoiceNumber]))) PERSISTED NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }

        public int UpdateFrom1()
        {
            return 2;
        }

    }
}