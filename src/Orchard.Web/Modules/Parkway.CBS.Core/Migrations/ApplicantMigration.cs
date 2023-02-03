using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ApplicantMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Applicant).Name,
                table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                     .Column<string>("Title")
                     .Column<string>("FirstName")
                     .Column<string>("MiddleName")
                     .Column<string>("LastName")
                     .Column<string>("Email")
                     .Column<string>("Phone")
                     .Column<string>("Phone2")
                     .Column<string>("Phone3")
                     .Column<string>("Sex")
                     .Column<string>("StateOfOrigin")
                     .Column<string>("LGA")
                     .Column<DateTime>("DateOfBirth", c => c.Nullable())
                     .Column<DateTime>("DateOfRegistration", c => c.Nullable())
                     .Column<string>("DateOfIncorporation", c => { c.Nullable(); })
                     .Column<string>("Occupation")
                     .Column<string>("TIN")
                     .Column<string>("CompanyName")
                     .Column<string>("RCNumber")
                     .Column<int>("TaxEntityCategory_Id")
                     .Column<string>("IdentificationNumber")
                     .Column<string>("IdentificationFilePath")
                     .Column<string>("IdentificationType")
                     .Column<string>("Nationality")
                     .Column<string>("MotherMaidenName")
                     .Column<string>("MotherName")
                     .Column<string>("MaritalStatus")
                     .Column<DateTime>("CreatedAtUtc", c => c.Nullable())
                     .Column<DateTime>("UpdatedAtUtc", c => c.Nullable())
                );
            return 1;
        }

    }
}