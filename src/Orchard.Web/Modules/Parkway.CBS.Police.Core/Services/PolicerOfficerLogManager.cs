using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PolicerOfficerLogManager : BaseManager<PolicerOfficerLog>, IPolicerOfficerLogManager<PolicerOfficerLog>
    {
        private readonly IRepository<PolicerOfficerLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PolicerOfficerLogManager(IRepository<PolicerOfficerLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets police officer log details with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PoliceOfficerLogVM GetPoliceOfficerDetails(long id)
        {
            try
            {
                return _transactionManager.GetSession().Query<PolicerOfficerLog>().Where(x => x.Id == id).Select(x => new PoliceOfficerLogVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    RankId = x.Rank.Id,
                    CommandId = x.Command.Id,
                    IdentificationNumber = x.IdentificationNumber,
                    RankCode = x.Rank.ExternalDataCode,
                    RankName = x.Rank.RankName
                }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets police officer log id for officer with specified APNumber
        /// </summary>
        /// <param name="apNumber"></param>
        /// <returns></returns>
        public PoliceOfficerLogVM GetPoliceOfficerDetails(string apNumber)
        {
            return _transactionManager.GetSession().Query<PolicerOfficerLog>()
                .OrderByDescending(x => x.Id)
                .Where(x => x.IdentificationNumber == apNumber)
                .Select(x => new PoliceOfficerLogVM 
                { 
                    Id = x.Id,
                    RankId = x.Rank.Id
                }).FirstOrDefault();
        }
    }
}