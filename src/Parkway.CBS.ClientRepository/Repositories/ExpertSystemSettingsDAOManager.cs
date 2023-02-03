using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class ExpertSystemSettingsDAOManager : Repository<ExpertSystemSettings>, IExpertSystemSettingsDAOManager
    {
        public ExpertSystemSettingsDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Get root expert system
        /// <para>Returns the future instance</para>
        /// </summary>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        public IEnumerable<Core.HelperModels.ExpertSystemVM> GetRootExpertSystem()
        {
            return _uow.Session.Query<ExpertSystemSettings>()
          .Where(x => x.IsRoot)
          .Select(x => new Core.HelperModels.ExpertSystemVM
          {
              Id = x.Id,
              StateId = x.TenantCBSSettings.StateId,
          }).ToFuture();
        }
    }
}
