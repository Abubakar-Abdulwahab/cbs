using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class VerificationCodeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(VerificationCode).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("CBSUser_Id", column => column.NotNull())
                    .Column<int>("ResendCount", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(VerificationCode).Name, table => table.AddColumn(nameof(VerificationCode.VerificationType), System.Data.DbType.Int32, column => column.WithDefault(0)));

            string tableName = SchemaBuilder.TableDbName(typeof(VerificationCode).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = {2}", tableName, nameof(VerificationCode.VerificationType), (int)Models.Enums.VerificationType.AccountVerification);
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }
    }

}