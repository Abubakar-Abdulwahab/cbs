using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSServiceSettlementConfigurationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSServiceSettlementConfiguration).Name,
                table => table
                    .Column<int>(nameof(PSSServiceSettlementConfiguration.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSServiceSettlementConfiguration.Channel), column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfiguration.MDA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfiguration.Settlement) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfiguration.RevenueHead) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfiguration.Service) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSServiceSettlementConfiguration.PaymentProvider) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSServiceSettlementConfiguration.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<DateTime>(nameof(PSSServiceSettlementConfiguration.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSServiceSettlementConfiguration.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSServiceSettlementConfiguration).Name);
            string queryString = $"ALTER TABLE {tableName} add [{nameof(PSSServiceSettlementConfiguration.CompositeUniqueValue)}] as ((case when [{nameof(PSSServiceSettlementConfiguration.IsActive)}] = (1) then concat(CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfiguration.Service)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfiguration.MDA)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfiguration.RevenueHead)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfiguration.PaymentProvider)}_Id]),'-',CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfiguration.Channel)}])) else CONVERT([nvarchar](30),[{nameof(PSSServiceSettlementConfiguration.Id)}]) end));";

            SchemaBuilder.ExecuteSql(queryString);

            string unqiueQuery = $"ALTER TABLE[dbo].[{tableName}] ADD constraint COMPOSITEVALUE_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSServiceSettlementConfiguration.CompositeUniqueValue)}]);";
            SchemaBuilder.ExecuteSql(unqiueQuery);

            return 1;
        }
        
    }
}