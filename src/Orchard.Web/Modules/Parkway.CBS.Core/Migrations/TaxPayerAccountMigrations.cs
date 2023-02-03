using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxPayerAccountMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxEntityAccount).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<decimal>("Credits", column => column.NotNull())
                            .Column<decimal>("Debits", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        //public int UpdateFrom1()
        //{
        //    SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("Recipient", System.Data.DbType.String, column => column.WithLength(500)));
        //    return 2;
        //}


        //public int UpdateFrom2()
        //{
        //    SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("RCNumber", System.Data.DbType.String, column => column.WithLength(50)));
        //    return 3;
        //}


        //public int UpdateFrom2()
        //{
        //    SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.DropColumn("RecipientValue1"));
        //    SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.DropColumn("RecipientValue2"));
        //    SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.DropColumn("Occupation"));
        //    return 3;
        //}
    }
}