using System;
using System.Linq;
using Parkway.CBS.Core.Models;
using Orchard.Data.Migration;
using System.Reflection;

namespace Parkway.CBS.Core.Migrations
{
    public class TINRegistrationFormMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TINRegistrationForm).Name, table => table
              .Column<int>("Id", c => c.Identity().PrimaryKey())
              .Column<DateTime>("CreatedAtUtc", c => c.Nullable())
              .Column<DateTime>("UpdatedAtUtc", c => c.Nullable())
          );

            return 1;
        }

        public int UpdateFrom1()
        {
            var properties = typeof(TINRegistrationForm).GetProperties().Where(x => (x.Name != "Id" && x.Name != "CreatedAtUtc" && x.Name != "UpdatedAtUtc")
            );
            foreach (PropertyInfo item in properties)
            {
                if (item.PropertyType.IsValueType)
                {
                    SchemaBuilder.AlterTable(typeof(TINRegistrationForm).Name, tbl => tbl.AddColumn(item.Name, GetEnumType(item.PropertyType)));
                }
                else if (item.PropertyType.IsEnum)
                {
                    SchemaBuilder.AlterTable(typeof(TINRegistrationForm).Name, tbl => tbl.AddColumn<string>(item.Name));
                }
                else if (typeof(string).IsAssignableFrom(item.PropertyType))
                {
                    SchemaBuilder.AlterTable(typeof(TINRegistrationForm).Name, tbl => tbl.AddColumn<string>(item.Name));
                }
                else if (typeof(DateTime).IsAssignableFrom(item.PropertyType))
                {
                    SchemaBuilder.AlterTable(typeof(TINRegistrationForm).Name, tbl => tbl.AddColumn<DateTime>(item.Name));
                }
                else
                {
                    if (typeof(Address).IsAssignableFrom(item.PropertyType))
                    {
                        SchemaBuilder.AlterTable(typeof(TINRegistrationForm).Name, tbl => tbl.AddColumn(item.Name + "_Id", GetEnumType(item.PropertyType)));
                    }
                    if (typeof(Applicant).IsAssignableFrom(item.PropertyType))
                    {
                        SchemaBuilder.AlterTable(typeof(TINRegistrationForm).Name, tbl => tbl.AddColumn(item.Name + "_Id", GetEnumType(item.PropertyType)));
                    }
                }

            }

            return 2;
        }

        //public int UpdateFrom2()
        //{
        //    SchemaBuilder.AlterTable("TINRegistrationForm", table => table.AddColumn<string>("TINApplicantReference"));
        //    return 3;
        //}

        private System.Data.DbType GetEnumType(Type propertyType)
        {
            System.Data.DbType result;
            if (typeof(string).IsAssignableFrom(propertyType))
            {
                result = System.Data.DbType.String;
            }
            else if (typeof(Int64).IsAssignableFrom(propertyType))
            {
                result = System.Data.DbType.Int64;
            }
            else if (typeof(DateTime).IsAssignableFrom(propertyType))
            {
                result = System.Data.DbType.DateTime;
            }
            else if (typeof(Boolean).IsAssignableFrom(propertyType))
            {
                result = System.Data.DbType.Boolean;
            }
            else
            {
                result = System.Data.DbType.Int32;
            }

            return result;

        }



        public int UpdateFrom2()
        {

            SchemaBuilder.AlterTable("TinRegistrationForm", table => table.DropColumn("TaxType"));
            SchemaBuilder.AlterTable("TinRegistrationForm", table => table.AddColumn<string>("TaxType"));

            SchemaBuilder.AlterTable("TinRegistrationForm", table => table.DropColumn("RepType"));
            SchemaBuilder.AlterTable("TinRegistrationForm", table => table.AddColumn<string>("RepType"));

            SchemaBuilder.AlterTable("TinRegistrationForm", table => table.DropColumn("SourceOfIncome"));
            SchemaBuilder.AlterTable("TinRegistrationForm", table => table.AddColumn<string>("SourceOfIncome"));
            return 3;
        }

        //public int UpdateFrom4()
        //{
        //    SchemaBuilder.AlterTable("TINRegistrationForm", table => table.AddColumn<string>("TIN"));
        //    return 5;
        //}
    }
}