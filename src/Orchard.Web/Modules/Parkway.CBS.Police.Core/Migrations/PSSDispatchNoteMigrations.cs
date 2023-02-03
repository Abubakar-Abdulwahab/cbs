using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSDispatchNoteMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSDispatchNote).Name,
                table => table
                    .Column<Int64>(nameof(PSSDispatchNote.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSDispatchNote.ApplicantName), column => column.NotNull())
                    .Column<string>(nameof(PSSDispatchNote.ApprovalNumber), column => column.NotNull())
                    .Column<string>(nameof(PSSDispatchNote.FileRefNumber), column => column.NotNull())
                    .Column<string>(nameof(PSSDispatchNote.OriginStateName), column => column.Nullable())
                    .Column<string>(nameof(PSSDispatchNote.OriginLGAName), column => column.Nullable())
                    .Column<string>(nameof(PSSDispatchNote.OriginAddress), column => column.Nullable())
                    .Column<string>(nameof(PSSDispatchNote.ServiceDeliveryStateName), column => column.NotNull())
                    .Column<string>(nameof(PSSDispatchNote.ServiceDeliveryLGAName), column => column.NotNull())
                    .Column<string>(nameof(PSSDispatchNote.ServiceDeliveryAddress), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSDispatchNote.StartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSDispatchNote.EndDate), column => column.NotNull())
                    .Column<string>(nameof(PSSDispatchNote.ServicingCommands), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSDispatchNote.PoliceOfficers), column => column.NotNull().Unlimited())
                    .Column<string>(nameof(PSSDispatchNote.DispatchNoteTemplate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSDispatchNote.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSDispatchNote.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}