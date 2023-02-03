using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class AuditTrailMigrations : DataMigrationImpl
    {


            public int Create()
            {
                SchemaBuilder.CreateTable(typeof(AuditTrail).Name,
                    table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())
                                .Column<int>("Type", column => column.NotNull())
                                .Column<string>("Model", column => column.NotNull().Unlimited())
                                .Column<int>("Source_Id", column => column.NotNull())
                                .Column<int>("AddedBy_Id", column => column.NotNull())
                                .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                                .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                    );
                return 1;
            }
        }
    }
