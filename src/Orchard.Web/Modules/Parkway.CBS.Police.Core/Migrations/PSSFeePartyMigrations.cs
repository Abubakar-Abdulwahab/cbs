using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSFeePartyMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSFeeParty).Name,
                table => table
                    .Column<int>(nameof(PSSFeeParty.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSFeeParty.AccountNumber), column => column.Nullable())
                    .Column<int>(nameof(PSSFeeParty.Bank) + "_Id", column => column.Nullable())                
                    .Column<string>(nameof(PSSFeeParty.Name), column => column.NotNull().Unique())
                    .Column<int>(nameof(PSSFeeParty.LastUpdatedBy) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSFeeParty.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<DateTime>(nameof(PSSFeeParty.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSFeeParty.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSFeeParty).Name);
            SchemaBuilder.ExecuteSql($"CREATE UNIQUE NONCLUSTERED INDEX PSSFeeParty_Unique_AccountNumber_NotNullConstraint ON {tableName} ({nameof(PSSFeeParty.AccountNumber)}) WHERE {nameof(PSSFeeParty.AccountNumber)} IS NOT NULL");

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSFeeParty).Name, table => table.AddColumn($"{nameof(PSSFeeParty.SettlementAccountType)}", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSFeeParty).Name);

            string queryString = string.Format("UPDATE {0} SET [SettlementAccountType] = {1}", tableName, (int)Models.Enums.SettlementAccountType.CommandSettlement);

            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, $"{nameof(PSSFeeParty.SettlementAccountType)}");
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
    }
}