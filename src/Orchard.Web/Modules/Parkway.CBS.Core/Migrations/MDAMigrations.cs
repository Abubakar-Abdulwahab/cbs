using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class MDAMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(MDA).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Code", column => column.Unique().WithLength(255).NotNull())
                            .Column<string>("SMEKey", column => column.Nullable())
                            .Column<string>("Name", column => column.Unique().WithLength(255).NotNull())
                            .Column<string>("Slug", column => column.Unique().WithLength(600).NotNull())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("BankDetails_Id", column => column.Nullable())
                            .Column<int>("MDASettings_Id", column => column.Nullable())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull()) //LastUpdatedBy
                            .Column<bool>("IsActive", column => column.WithDefault(true).NotNull())
                            .Column<bool>("IsVisible", column => column.WithDefault(false).NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(MDA).Name,
                    table => table.CreateIndex("Name", new string[] { "Name" }))
                 .AlterTable(typeof(MDA).Name,
                    table => table.CreateIndex("Code", new string[] { "Code" }))
                 .AlterTable(typeof(MDA).Name,
                    table => table.CreateIndex("Slug", new string[] { "Slug" }))
                 .AlterTable(typeof(MDA).Name,
                    table => table.CreateIndex("IsActive", new string[] { "IsActive" }));

            //("key name", "concerned table", "concerned column on the concerned table", "the parent table", "the parent table 'primary key' ")
            
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.AddColumn("UsesTSA", System.Data.DbType.Boolean));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.AddColumn("ExpertSystemSettings_Id", System.Data.DbType.Int32));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.AddColumn("HasPaymentProviderValidationConstraint", System.Data.DbType.Boolean, col => col.WithDefault(false)));
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.AddColumn("SettlementCode", System.Data.DbType.String, col => col.WithDefault("001")));
            SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.AddColumn("SettlementType", System.Data.DbType.Int32, col => col.WithDefault(((int)SettlementType.Percentage))));
            return 5;
        }

        //public int UpdateFrom3()
        //{
        //    SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.AddColumn("RequestIdentifier", System.Data.DbType.String));
        //    SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.CreateIndex("RequestIdentifier", new string[] { "RequestIdentifier" }));
        //    return 4;
        //}

        //public int UpdateFrom1()
        //{
        //    try
        //    {
        //////////////////////var tabName = typeof(MDA).Name;
        //////////////////////var sd = SchemaBuilder;
        //////////////////////var th = SchemaBuilder.AlterTable(typeof(MDA).Name, table => table.DropColumn("sd"));
        //////////////////////th.CreateForeignKey("MDA_Setiing_Id_FK", typeof(MDA).Name, new string[] { "MDASettings_Id" }, typeof(MDASettings).Name, new string[] { "Id" })
        //////////////////////    .CreateForeignKey("BankDetails_Id_FK", typeof(MDA).Name, new string[] { "BankDetails_Id" }, typeof(BankDetails).Name, new string[] { "Id" })
        //////////////////////    .CreateForeignKey("AddedBy_Id_FK", typeof(MDA).Name, new string[] { "AddedBy_Id" }, "Orchard.Users", typeof(UserPartRecord).Name, new string[] { "Id" })
        //////////////////////    .CreateForeignKey("LastUpdatedBy_Id_FK", typeof(MDA).Name, new string[] { "LastUpdatedBy_Id" }, "Orchard.Users", typeof(UserPartRecord).Name, new string[] { "Id" });



        //        //SchemaBuilder.CreateForeignKey("MDA_Setiing_Id_FK", typeof(MDA).Name, new string[] { "MDASettings_Id" }, typeof(MDASettings).Name, new string[] { "Id" });
        //        SchemaBuilder.CreateForeignKey("BankDetails_Id_FK", typeof(MDA).Name, new string[] { "BankDetails_Id" }, typeof(BankDetails).Name, new string[] { "Id" });
        //        SchemaBuilder.CreateForeignKey("AddedBy_Id_FK", typeof(MDA).Name, new string[] { "AddedBy_Id" }, "Orchard.Users", typeof(UserPartRecord).Name, new string[] { "Id" });
        //        SchemaBuilder.CreateForeignKey("LastUpdatedBy_Id_FK", typeof(MDA).Name, new string[] { "LastUpdatedBy_Id" }, "Orchard.Users", typeof(UserPartRecord).Name, new string[] { "Id" });
        //    }
        //    catch (Exception)
        //    {
        //        return 1;
        //    }
        //    return 2;
        //}
    }
}