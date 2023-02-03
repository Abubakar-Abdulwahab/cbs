using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class ExpertSystemSettingsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExpertSystemSettings).Name,
                table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity().PrimaryKey())
                            .Column<string>("CompanyEmail", column => column.NotNull().WithLength(255))
                            .Column<string>("CompanyAddress", column => column.NotNull().WithLength(500))
                            .Column<string>("CompanyName", column => column.NotNull().WithLength(255))
                            .Column<string>("CompanyMobile", column => column.Nullable().WithLength(20))
                            .Column<string>("BusinessNature", column => column.NotNull().WithLength(255))
                            .Column<int>("TenantCBSSettings_Id", column => column.NotNull())
                            .Column<string>("ReferenceDataSourceType", column => column.Nullable().WithLength(50))
                            .Column<string>("ReferenceDataSourceName", column => column.Nullable().WithLength(200))
                            .Column<string>("AdapterClassName", column => column.Nullable().WithLength(1000))
                            .Column<string>("LogoPath", column => column.NotNull().WithLength(1000))
                            .Column<string>("SignaturePath", column => column.NotNull().WithLength(1000))
                            .Column<int>("TSA", column => column.NotNull())
                            .Column<string>("TSABankNumber", column => column.NotNull())
                            .Column<string>("ClientId", column => column.NotNull().Unique().WithLength(255))
                            .Column<string>("ClientSecret", column => column.Unique().NotNull())
                            .Column<string>("BillingSchedulerIdentifier", column => column.Unique().NotNull().WithLength(150))
                            .Column<string>("BankCode", column => column.NotNull())
                            .Column<bool>("IsRoot", column => column.NotNull())
                            .Column<bool>("IsActive", column => column.NotNull())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(ExpertSystemSettings).Name,
                    table => table.CreateIndex("NCI_ClientId", new string[] { "ClientId" }));
            
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("URLPath", System.Data.DbType.String, column => column.Nullable()));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("CallBackURL", System.Data.DbType.String, column => column.Nullable()));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("AccessList", System.Data.DbType.String, column => column.Unlimited()));
            return 4;
        }

        public int UpdateFrom4()
        {
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("CanProcessPaye", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("PaymentProviderName", System.Data.DbType.String, column => column.WithLength(50)));
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("IsAPaymentProvider", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("CanMakePayePayments", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("PermissionJSONList", System.Data.DbType.String, column => column.Unlimited()));
            SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("PaymentProviderId", System.Data.DbType.Int32));
            return 6;
        }

        public int UpdateFrom6()
        {
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.DropColumn("CanMakePayePayments"));
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.DropColumn("CanProcessPaye"));
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.DropColumn("IsAPaymentProvider"));
            //SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.DropColumn("PaymentProviderName"));
            return 7;
        }



        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(ExpertSystemSettings).Name, table => table.AddColumn("ThirdPartyAuthorizedAdmin_Id", System.Data.DbType.Int32));
            return 8;
        }
    }
}