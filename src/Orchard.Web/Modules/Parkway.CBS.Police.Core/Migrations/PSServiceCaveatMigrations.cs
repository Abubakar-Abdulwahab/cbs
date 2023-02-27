using NHibernate.Linq;
using Orchard;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceCaveatMigrations : DataMigrationImpl
    {
        private readonly IOrchardServices _orchardServices;

        public PSServiceCaveatMigrations(IOrchardServices orchardServices) { _orchardServices = orchardServices; }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceCaveat).Name,
                table => table
                            .Column<int>(nameof(PSServiceCaveat.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(PSServiceCaveat.CaveatHeader), column => column.NotNull().WithLength(40))
                            .Column<string>(nameof(PSServiceCaveat.CaveatContent), column => column.NotNull().WithLength(1500))
                            .Column<int>(nameof(PSServiceCaveat.Service)+"_Id", column => column.NotNull().Unique())
                            .Column<bool>(nameof(PSServiceCaveat.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(PSServiceCaveat.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSServiceCaveat.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceCaveat).Name);
            var services = _orchardServices.TransactionManager.GetSession().Query<PSService>().Where(x => x.IsActive).Select(x => new DTO.PSServiceVM { ServiceId = x.Id, ServiceType = x.ServiceType });
            foreach (var service in services)
            {
                if (service.ServiceType == (int)Models.Enums.PSSServiceTypeDefinition.CharacterCertificate)
                {
                    string queryString = $"INSERT INTO {tableName}({nameof(PSServiceCaveat.CaveatHeader)}, {nameof(PSServiceCaveat.CaveatContent)}, {nameof(PSServiceCaveat.Service)}_Id, {nameof(PSServiceCaveat.IsActive)}, {nameof(PSServiceCaveat.CreatedAtUtc)}, {nameof(PSServiceCaveat.UpdatedAtUtc)}) VALUES(:caveatHeader, :caveatContent, :serviceId, :isActive, :createdAt, :updatedAt)";
                    var sqlQuery = _orchardServices.TransactionManager.GetSession().CreateSQLQuery(queryString);
                    sqlQuery.SetParameter("caveatHeader", "Police Character Certificate Caveat");
                    sqlQuery.SetParameter("caveatContent", "<p class=caveat-content>The information, finger-prints and this record are voluntarily submitted by the undersigned as a statement of fact and the qualifications for the position/licence. The Employer/Licensing Officer/Permit Officer may submit the said information and the finger-prints to any person, firm, corporation, body bureau department, Police officers and Police Record Bureau, whatever or whomever for the purpose of any investigation whatsoever which the Employer/Licensing Officer/Permit Officer may desire to make with reference thereto.</p><p class=caveat-content>The Applicant does remise, release and forever discharge the Employer/Licensing Office/Permit Office, its successors and assigns of and from all manner of actions, suits, either in law or in equity, which against the Employer/Licensing Officer/Permit Officer the undersigned ever had, now has or which the Applicant, his heirs, executor and administrators hereafter can, shall or may have for upon or by reason of any matters causes of things whatsoever in connection with the foregoing.</p>");
                    sqlQuery.SetParameter("serviceId", service.ServiceId);
                    sqlQuery.SetParameter("isActive", true);
                    sqlQuery.SetParameter("createdAt", DateTime.Now.ToLocalTime());
                    sqlQuery.SetParameter("updatedAt", DateTime.Now.ToLocalTime());
                    sqlQuery.ExecuteUpdate();
                }
            }
            return 2;
        }
    }
}