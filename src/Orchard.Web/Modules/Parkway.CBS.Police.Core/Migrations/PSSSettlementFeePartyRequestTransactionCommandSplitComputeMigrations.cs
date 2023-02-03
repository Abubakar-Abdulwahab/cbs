using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementFeePartyRequestTransactionCommandSplitComputeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute).Name,
                table => table
                   .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Id), column => column.PrimaryKey().Identity())
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction) + "_Id", column => column.NotNull())
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty) + "_Id", column => column.NotNull())
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction) + "_Id", column => column.NotNull())
                        .Column<bool>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FallFlag), column => column.NotNull().WithDefault(false))
                        .Column<decimal>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit), column => column.NotNull())
                        .Column<decimal>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitItemCount), column => column.NotNull().WithDefault(0))
                        .Column<decimal>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage), column => column.NotNull().WithDefault(0.00m))
                        .Column<decimal>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount), column => column.NotNull().WithDefault(0.00m))
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State) + "_Id", column => column.NotNull())
                        .Column<int>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA) + "_Id", column => column.NotNull())
                        .Column<Int64>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.RequestCommand) + "_Id", column => column.NotNull())
                        .Column<string>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue), column => column.Nullable())
                        .Column<bool>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit), column => column.NotNull().WithDefault(false))
                        .Column<DateTime>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.CreatedAtUtc), column => column.NotNull())
                        .Column<DateTime>(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.UpdatedAtUtc), column => column.NotNull())
                ).AlterTable(typeof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute).Name,
                    table => table.CreateIndex("FEEPARTYRQTRNXSPLITCOMPUTE_GROUPBY_INDEX", new string[] { nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch) + "_Id", nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction) + "_Id" })); ;

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint FEEPARTYRQTRNXSPLITCOMPUTE_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id], [{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id], [{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id], [{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id], [{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.RequestCommand)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute).Name, table => table.AddColumn(nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute).Name);

            string queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand) + "_Id"} = {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command) + "_Id"} WHERE {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)} = 'Command';");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand) + "_Id"} = {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand) + "_Id"} WHERE {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)} = 'State';");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand) + "_Id"} = {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand) + "_Id"} WHERE {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)} = 'Zonal';");
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute).Name, table => table.AddColumn($"{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementAccountType)}", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute).Name);

            string queryString = string.Format($"UPDATE {tableName} SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementAccountType)} = {(int)Models.Enums.SettlementAccountType.CommandSettlement} WHERE {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = 1;");

            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }


    }
}