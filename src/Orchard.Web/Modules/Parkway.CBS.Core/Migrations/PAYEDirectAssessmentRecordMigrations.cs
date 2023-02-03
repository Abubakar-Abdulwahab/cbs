using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEDirectAssessmentRecordMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEDirectAssessmentRecord).Name,
                table => table
                            .Column<Int64>(nameof(PAYEDirectAssessmentRecord.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(PAYEDirectAssessmentRecord.AssessedBy) + "_Id", column => column.Nullable())
                            .Column<int>(nameof(PAYEDirectAssessmentRecord.AssessedByExternalExpertSystem) + "_Id", column => column.Nullable())
                            .Column<Int64>(nameof(PAYEDirectAssessmentRecord.Invoice) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(PAYEDirectAssessmentRecord.Month), column => column.NotNull())
                            .Column<int>(nameof(PAYEDirectAssessmentRecord.Year), column => column.NotNull())
                            .Column<int>(nameof(PAYEDirectAssessmentRecord.PAYEBusinessSize) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(PAYEDirectAssessmentRecord.PAYEBusinessType) + "_Id", column => column.NotNull())
                            .Column<int>("AssessmentTypeId", column => column.NotNull())
                            .Column<decimal>(nameof(PAYEDirectAssessmentRecord.IncomeTaxPerMonth), column => column.NotNull())
                            .Column<string>(nameof(PAYEDirectAssessmentRecord.Comment), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEDirectAssessmentRecord.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEDirectAssessmentRecord.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PAYEDirectAssessmentRecord).Name);
            string queryString = string.Format("sp_rename '{0}.AssessmentTypeId', 'AssessmentType_Id', 'COLUMN';", tableName);
            SchemaBuilder.ExecuteSql(queryString);
           
            return 2;
        }

    }
}