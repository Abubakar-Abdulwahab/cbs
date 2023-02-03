using Orchard;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using NHibernate;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IMDAManager<MDA> : IDependency, IBaseManager<MDA>
    {

        /// <summary>
        /// Get the list of MDAs that this user has access to
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<MDAVM> GetAccessList(int userId, AccessType accessType, bool applyAccessRestrictions = false);

        /// <summary>
        /// Get all active mdas as MDAVMs
        /// </summary>
        /// <returns></returns>
        List<MDAVM> GetActiveMDAVMs();

        /// <summary>
        /// Update HasPaymentProviderValidationConstraints for MDA with specified Id.
        /// </summary>
        void UpdateMDAPaymentProviderValidationConstraintsStatus();

        /// <summary>
        /// Get list of MDAs
        /// </summary>
        /// <returns>IEnumerable{MDAVM}</returns>
        IEnumerable<MDAVM> GetListOfMDAs();


        /// <summary>
        /// Check if MDA exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns>bool</returns>
        bool CheckIfMDAExists(int mdaId);

    }
}