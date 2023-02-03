using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Parkway.CBS.Police.Core.Services
{
    public class PSSCharacterCertificateOptionsManager : BaseManager<PSServiceOptions>, IPSSCharacterCertificateOptionsManager<PSServiceOptions>
    {
        private readonly IRepository<PSServiceOptions> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSCharacterCertificateOptionsManager(IRepository<PSServiceOptions> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Gets active character certificate options
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{PSSCharacterCertificateOptionsPageVM}</returns>
        public IEnumerable<PSServiceOptionsVM> GetActiveCharacterCertificateOtpions(int serviceId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceOptions>()
               .Where(x => ((x.IsActive) && (x.Service == new PSService { Id = serviceId })))
               .Select(x => new PSServiceOptionsVM
               {
                   Name = x.Name,
                   Id = x.Id
               }).ToFuture<PSServiceOptionsVM>();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get service option with the service Id and option Id that is active
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="optionId"></param>
        /// <returns>PSSServiceOptionsVM</returns>
        public PSServiceOptionsVM GetServiceOption(int serviceId, int optionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceOptions>()
               .Where(x => ((x.IsActive) && (x.Service == new PSService { Id = serviceId }) && (x.Id == optionId)))
               .Select(x => new PSServiceOptionsVM
               {
                   ServiceOptionId = x.ServiceOption.Id,
                   OptionType = x.ServiceOptionType,
                   Name = x.ServiceOption.Name
               }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}