using Orchard;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IRoleUserManager<RoleUser> : IDependency, IBaseManager<RoleUser>
    {
        /// <summary>
        /// Here we check if the user has been constrained to any role
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invoiceAssessmentReport"></param>
        /// <returns>bool</returns>
        bool UserHasAcessTypeRole(int id, AccessType invoiceAssessmentReport);
    }
}
