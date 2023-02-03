using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortSquadAllocationGroupMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortSquadAllocationGroup).Name,
                table => table
                    .Column<Int64>(nameof(EscortSquadAllocationGroup.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(EscortSquadAllocationGroup.Comment), column => column.NotNull())
                    .Column<int>(nameof(EscortSquadAllocationGroup.RequestLevel) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortSquadAllocationGroup.Service) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(EscortSquadAllocationGroup.Request) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortSquadAllocationGroup.AdminUser) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(EscortSquadAllocationGroup.Fulfilled), column => column.NotNull())
                    .Column<string>(nameof(EscortSquadAllocationGroup.StatusDescription), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortSquadAllocationGroup.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortSquadAllocationGroup.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}