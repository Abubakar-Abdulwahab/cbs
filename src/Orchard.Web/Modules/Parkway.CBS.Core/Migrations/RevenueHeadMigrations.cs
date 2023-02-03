using Orchard.Data.Migration;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Users;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Migrations
{
    public class RevenueHeadMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RevenueHead).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Code", column => column.NotNull())
                            .Column<string>("Name", column => column.NotNull())
                            .Column<string>("Slug", column => column.NotNull())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.Nullable())
                            .Column<string>("CashFlowProductCode", column => column.Nullable().WithLength(500))
                            .Column<string>("RefDataURL", column => column.Nullable().Unlimited())
                            .Column<Int64>("CashFlowProductId", column => column.Nullable())
                            .Column<int>("Billing_Id", column => column.Nullable())
                            .Column<int>("BillingModel_Id", column => column.Nullable())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<bool>("IsActive", column => column.NotNull().WithDefault(true))
                            .Column<bool>("IsVisible", column => column.WithDefault(false).NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(RevenueHead).Name,
                    table => table.CreateIndex("Name", new string[] { "Name" }))
                 .AlterTable(typeof(RevenueHead).Name,
                    table => table.CreateIndex("Code", new string[] { "Code" }))
                 .AlterTable(typeof(RevenueHead).Name,
                    table => table.CreateIndex("Slug", new string[] { "Slug" }))
                 .AlterTable(typeof(RevenueHead).Name,
                    table => table.CreateIndex("IsActive", new string[] { "IsActive" }));

            //("key name", "concerned table", "concerned column on the concerned table", "the parent table", "the parent table 'primary key' ")
            //SchemaBuilder.CreateForeignKey("MDA_Id_FK", typeof(RevenueHead).Name, new string[] { "MDA_Id" }, typeof(MDA).Name, new string[] { "Id" });

            //SchemaBuilder.CreateForeignKey("RevenueHead_Id_FK", typeof(RevenueHead).Name, new string[] { "RevenueHead_Id" }, typeof(RevenueHead).Name, new string[] { "Id" });

            //SchemaBuilder.CreateForeignKey("AddedBy_Id_FK", typeof(RevenueHead).Name, new string[] { "AddedBy_Id" }, "Orchard.Users", typeof(UserPartRecord).Name, new string[] { "Id" });

            //SchemaBuilder.CreateForeignKey("LastUpdatedBy_Id_FK", typeof(RevenueHead).Name, new string[] { "LastUpdatedBy_Id" }, "Orchard.Users", typeof(UserPartRecord).Name, new string[] { "Id" });

            //SchemaBuilder.CreateForeignKey("BillingModel_Id_FK", typeof(RevenueHead).Name, new string[] { "BillingModel_Id" }, typeof(BillingModel).Name, new string[] { "Id" });
            //SchemaBuilder.CreateForeignKey("Order_Customer", "OrderRecord", new[] { "CustomerId" }, "CustomerRecord", new[] { "Id" });
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("IsPayeAssessment", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.DropColumn("Billing_Id"));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("InvoiceGenerationRedirectURL", System.Data.DbType.String, column => column.WithLength(500)));
            return 4;
        }


        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("RefDataDescription_Id", System.Data.DbType.Int32));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("IsGroup", System.Data.DbType.Boolean));
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("CallBackURL", System.Data.DbType.String, column => column.Nullable()));
            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("SettlementType", System.Data.DbType.Int32, col => col.WithDefault(((int)SettlementType.None))));
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("SettlementCode", System.Data.DbType.String));
            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(RevenueHead).Name, table => table.AddColumn("ServiceId", System.Data.DbType.String));
            return 9;
        }
        
    }
}