using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PaymentProviderValidationConstraintMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PaymentProviderValidationConstraint).Name,
                table => table
                            .Column<Int64>("Id", column => column.Identity().PrimaryKey())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.Nullable())
                            .Column<int>("PaymentProvider", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(PaymentProviderValidationConstraint).Name,
                    table => table.CreateIndex("FindQuery", new string[] { "MDA_Id", "PaymentProvider" }));

            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.CreateIndex("PaymentProviderIndex", new string[] { "PaymentProvider" }));

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.DropIndex("FindQuery"));
            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.CreateIndex("FindQuery", new string[] { "MDA_Id", "RevenueHead_Id", "PaymentProvider" }));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.AddColumn("PaymentProvider_Id", System.Data.DbType.Int32));
            return 3;
        }


        public int UpdateFrom3()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PaymentProviderValidationConstraint).Name);

            string queryString = string.Format("UPDATE {0} SET [PaymentProvider_Id] = [PaymentProvider]", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN PaymentProvider_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.DropIndex("FindQuery"));
            string indexDropQueryString = string.Format("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'PaymentProviderIndex') DROP INDEX [dbo].[{0}].PaymentProviderIndex", tableName);
            SchemaBuilder.ExecuteSql(indexDropQueryString);
            //SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.DropIndex("PaymentProviderIndex"));
            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.CreateIndex("FindQuery", new string[] { "MDA_Id", "RevenueHead_Id", "PaymentProvider_Id" }));
            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.DropColumn("PaymentProvider"));

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PaymentProviderValidationConstraint).Name, table => table.AddColumn("IsDeleted", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 5;
        }

    }
}