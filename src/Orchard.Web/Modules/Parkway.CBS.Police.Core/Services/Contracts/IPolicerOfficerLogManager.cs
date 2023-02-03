using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPolicerOfficerLogManager<PolicerOfficerLog> : IDependency, IBaseManager<PolicerOfficerLog>
    {
        /// <summary>
        /// Gets police officer log details with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PoliceOfficerLogVM GetPoliceOfficerDetails(long id);

        /// <summary>
        /// Gets police officer log id for officer with specified APNumber
        /// </summary>
        /// <param name="apNumber"></param>
        /// <returns></returns>
        PoliceOfficerLogVM GetPoliceOfficerDetails(string apNumber);
    }
}
