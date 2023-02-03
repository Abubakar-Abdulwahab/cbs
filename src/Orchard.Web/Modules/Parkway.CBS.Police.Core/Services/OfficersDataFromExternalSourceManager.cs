using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class OfficersDataFromExternalSourceManager : BaseManager<OfficersDataFromExternalSource>, IOfficersDataFromExternalSourceManager<OfficersDataFromExternalSource>
    {
        private readonly IRepository<OfficersDataFromExternalSource> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public OfficersDataFromExternalSourceManager(IRepository<OfficersDataFromExternalSource> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Get the count of items between the startSN
        /// and endSN
        /// </summary>
        /// <param name="startSN"></param>
        /// <param name="endSN"></param>
        /// <returns>int</returns>
        public int GetCountWithinRange(int startSN, int endSN)
        {
            return _transactionManager.GetSession().Query<OfficersDataFromExternalSource>().Count(ext => ((ext.RequestItemSN >= startSN) && (ext.RequestItemSN <= endSN)));
        }


    }
}