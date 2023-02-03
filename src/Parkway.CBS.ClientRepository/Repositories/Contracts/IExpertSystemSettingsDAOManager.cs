using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IExpertSystemSettingsDAOManager : IRepository<ExpertSystemSettings>
    {
        /// <summary>
        /// Get root expert system
        /// <para>Returns the future instance</para>
        /// </summary>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        IEnumerable<Core.HelperModels.ExpertSystemVM> GetRootExpertSystem();
    }
}
