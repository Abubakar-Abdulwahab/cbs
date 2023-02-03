using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSPresettlementDeductionConfigurationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSPresettlementDeductionConfiguration).Name,
                table => table
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.Id), column => column.PrimaryKey().Identity())
                    .Column<string >(nameof(PSSPresettlementDeductionConfiguration.Name), column => column.NotNull().Unique())
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.Channel), column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.MDA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.SettlementRule) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.RevenueHead) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.Service) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.PaymentProvider) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSPresettlementDeductionConfiguration.ImplementClass), column => column.NotNull())
                    .Column<int>(nameof(PSSPresettlementDeductionConfiguration.DefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSPresettlementDeductionConfiguration.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSPresettlementDeductionConfiguration.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSPresettlementDeductionConfiguration).Name, table => table.AddColumn(nameof(PSSPresettlementDeductionConfiguration.DeductionShareTypeId), System.Data.DbType.Int32, column => column.NotNull().WithDefault(1)));
            SchemaBuilder.AlterTable(typeof(PSSPresettlementDeductionConfiguration).Name, table => table.AddColumn(nameof(PSSPresettlementDeductionConfiguration.PercentageShare), System.Data.DbType.Decimal, column => column.NotNull().WithDefault(0)));
            SchemaBuilder.AlterTable(typeof(PSSPresettlementDeductionConfiguration).Name, table => table.AddColumn(nameof(PSSPresettlementDeductionConfiguration.FlatShare), System.Data.DbType.Decimal, column => column.NotNull().WithDefault(0)));
            return 2;
        }


    }
}