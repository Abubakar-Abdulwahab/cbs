using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CommandWalletDetailsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CommandWalletDetails).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("AccountNumber", column => column.NotNull().Unique().WithLength(10))
                            .Column<int>("Command_Id", column => column.NotNull().Unique())
                            .Column<string>("BankCode", column => column.NotNull())
                            .Column<bool>("IsActive", column => column.WithDefault(true))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())

                );
            string tableName = SchemaBuilder.TableDbName(typeof(CommandWalletDetails).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint CommandWalletDetails_Unique_Constraint UNIQUE([{1}], [{2}], [{3}]); ", tableName, "Command_Id", "BankCode", "AccountNumber"));

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(CommandWalletDetails).Name, table => table.AddColumn($"{nameof(CommandWalletDetails.Bank)}_Id", System.Data.DbType.Int32, column => column.Nullable()));

            return 2;
        }

        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(CommandWalletDetails).Name);

            string queryString = string.Format("UPDATE T1 SET T1.Bank_Id = T2.Id FROM {0} T1 INNER JOIN {1} AS T2 ON T1.BankCode = T2.Code", tableName, $"Parkway_CBS_Core_{nameof(CommandWalletDetails.Bank)}");

            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, $"{nameof(CommandWalletDetails.Bank)}_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(CommandWalletDetails).Name, table => table.AddColumn($"{nameof(CommandWalletDetails.SettlementAccountType)}", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(CommandWalletDetails).Name);

            string queryString = string.Format("UPDATE {0} SET [SettlementAccountType] = {1}", tableName, (int)Models.Enums.SettlementAccountType.CommandSettlement);

            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, $"{nameof(CommandWalletDetails.SettlementAccountType)}");
            SchemaBuilder.ExecuteSql(queryString);

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint CommandWalletDetails_Unique_CommandId_SettlementAccounType UNIQUE([{1}], [{2}]); ", tableName, $"{nameof(CommandWalletDetails.Command)}_Id", $"{nameof(CommandWalletDetails.SettlementAccountType)}"));

            return 4;
        }

    }
}