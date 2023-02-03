using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ReferenceData;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class RefDataTempMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RefDataTemp).Name,
                table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("TaxIdentificationNumber", column => column.WithLength(500).NotNull())
                            .Column<string>("Recipient", column => column.WithLength(1000).NotNull())
                            .Column<string>("Address", column => column.WithLength(1000).NotNull())
                            .Column<string>("Email", column => column.WithLength(1000).NotNull())
                            .Column<int>("TaxEntityCategoryId", column => column.NotNull())
                            .Column<int>("BillingModelId", column => column.NotNull())
                            .Column<int>("RevenueHeadId", column => column.NotNull())
                            .Column<string>("BatchNumber", column => column.WithLength(500).NotNull())
                            .Column<string>("AdditionalDetails", column => column.Unlimited().Nullable())
                            .Column<string>("ErrorLog", column => column.Unlimited().Nullable())
                            .Column<string>("StatusDetail", column => column.WithLength(250).NotNull().WithDefault(0))
                            .Column<decimal>("Amount", column => column.Nullable())
                            .Column<int>("Status", column => column.NotNull().WithDefault(0))
                            .Column<string>("UniqueIdentifier", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("Id", new string[] { "Id" }))
                .AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("TaxIdentificationNumber", new string[] { "TaxIdentificationNumber" }))
                 .AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("BatchNumber", new string[] { "BatchNumber" }))
                 .AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("RevenueHeadId", new string[] { "RevenueHeadId" }))
                 .AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("Status", new string[] { "Status" }))
                 .AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("BillingModelId", new string[] { "BillingModelId" }))
                 .AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("UniqueIdentifier", new string[] { "UniqueIdentifier" }))
                 .AlterTable(typeof(RefDataTemp).Name,
                    table => table.CreateIndex("PK_JOINER", new string[] { "BatchNumber", "RevenueHeadId", "BillingModelId" }));
            //("key name", "concerned table", "concerned column on the concerned table", "the parent table", "the parent table 'primary key' ")

            return 1;
        }

        //public int UpdateFrom1()
        //{
        //    SchemaBuilder.AlterTable(typeof(RefDataTemp).Name, table => table.AddColumn("UsesTSA", System.Data.DbType.Boolean));
        //    return 2;
        //}
    }
}