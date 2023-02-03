using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class BillingModelMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(BillingModel).Name,
                table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<string>("Assessment", column => column.WithType(System.Data.DbType.String).Unlimited())
                            .Column<decimal>("Amount", column => column.WithType(System.Data.DbType.Decimal))
                            .Column<int>("BillingType", column => column.Nullable())
                            .Column<string>("Discounts", column => column.Nullable().WithType(System.Data.DbType.String).Unlimited())
                            .Column<string>("DueDate", column => column.Nullable().WithType(System.Data.DbType.String).Unlimited())
                            .Column<string>("Penalties", column => column.Nullable().WithType(System.Data.DbType.String).Unlimited())
                            .Column<string>("DemandNotice", column => column.Nullable().WithType(System.Data.DbType.String).Unlimited())
                            .Column<string>("BillingFrequency", column => column.Nullable().WithType(System.Data.DbType.String).Unlimited())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(BillingModel).Name, table => table.AddColumn("Duration", System.Data.DbType.String));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(BillingModel).Name, table => table.AddColumn("NextBillingDate", System.Data.DbType.DateTime));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(BillingModel).Name, table => table.AddColumn("StillRunning", System.Data.DbType.Boolean));
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(BillingModel).Name, table => table.AddColumn("DirectAssessmentModel", System.Data.DbType.String, action => action.WithLength(1000)));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(BillingModel).Name, table => table.AddColumn("FileUploadModel", System.Data.DbType.String, action => action.WithLength(1000)));
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(BillingModel).Name, table => table.AddColumn("Surcharge", System.Data.DbType.Decimal));
            return 7;
        }


        public int UpdateFrom7()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(BillingModel).Name);

            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN DueDate nvarchar(1000) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 8;
        }
    }
}