using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Core.Migrations
{
    public class VerificationCodeItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(VerificationCodeItems).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("VerificationCode_Id", column => column.NotNull())
                    .Column<string>("CodeHash", column => column.NotNull().WithLength(250))
                    .Column<int>("State", column => column.NotNull().WithDefault(0))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}