using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRequestInvoiceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRequestInvoice).Name,
                table => table
                    .Column<Int64>(nameof(PSSRequestInvoice.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSRequestInvoice.Request)+"_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSRequestInvoice.Invoice) +"_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSRequest.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSRequest.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequestInvoice).Name);
            string tableName1 = SchemaBuilder.TableDbName(typeof(PoliceServiceRequest).Name);

            string queryString = $"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName1}') " +
                "BEGIN " +
                $"INSERT INTO {tableName}({nameof(PSSRequestInvoice.Invoice) + "_Id"}, {nameof(PSSRequestInvoice.Request) + "_Id"}, CreatedAtUtc, UpdatedAtUtc) SELECT Invoice_Id, Request_Id, MIN(CreatedAtUtc), MIN(UpdatedAtUtc) FROM {tableName1} GROUP BY Request_Id, Invoice_Id " +
                "END";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
        
    }
}