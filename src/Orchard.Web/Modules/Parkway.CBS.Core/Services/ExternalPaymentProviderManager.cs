using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;


namespace Parkway.CBS.Core.Services
{
    public class ExternalPaymentProviderManager : BaseManager<ExternalPaymentProvider>, IExternalPaymentProviderManager<ExternalPaymentProvider>
    {
        private readonly IRepository<ExternalPaymentProvider> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ExternalPaymentProviderManager(IRepository<ExternalPaymentProvider> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Get a list of all payment providers
        /// </summary>
        /// <returns> List{PaymentProviderVM}</returns>
        public List<PaymentProviderVM> GetProviders()
        {
            return _transactionManager.GetSession().Query<ExternalPaymentProvider>()
                .Select( x => new PaymentProviderVM { Id = x.Id.ToString(), Name = x.Name }).ToList();
        }

        /// <summary>
        /// Get payment provider with the specified Id
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public IEnumerable<PaymentProviderVM> GetProvider(int providerId)
        {
            return _transactionManager.GetSession().Query<ExternalPaymentProvider>().Where(x => x.Id == providerId && x.IsActive == true).Select(x => new PaymentProviderVM
            {
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToFuture();
        }


        /// <summary>
        /// Get payment provider with the specified client Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>ExternalPaymentProviderVM</returns>
        public ExternalPaymentProviderVM GetProvider(string clientId)
        {
            return _transactionManager.GetSession().Query<ExternalPaymentProvider>().Where(x => x.ClientID == clientId).Select(x => new ExternalPaymentProviderVM
            {
                Id = x.Id,
                Name = x.Name,
                ClientID = x.ClientID,
                ClientSecret = x.ClientSecret,
                ClassImplementation = x.ClassImplementation,
                CreatedAt = x.CreatedAtUtc,
                IsActive = x.IsActive,
                AllowAgentFeeAddition = x.AllowAgentFeeAddition
            }).FirstOrDefault();
        }


        /// <summary>
        /// Get external payment provider with the specified Id
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>ExternalPaymentProviderVM</returns>
        public ExternalPaymentProviderVM GetExternalProvider(int providerId)
        {
            return _transactionManager.GetSession().Query<ExternalPaymentProvider>().Where(x => x.Id == providerId).Select(x => new ExternalPaymentProviderVM
            {
                Id = x.Id,
                Name = x.Name,
                ClientID = x.ClientID,
                ClientSecret = x.ClientSecret,
                ClassImplementation = x.ClassImplementation,
                CreatedAt = x.CreatedAtUtc,
                IsActive = x.IsActive,
                AllowAgentFeeAddition = x.AllowAgentFeeAddition
            }).FirstOrDefault();
        }


        /// <summary>
        /// Get client secret by client Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public string GetClientSecretByClientId(string clientId)
        {
                return _orchardServices.TransactionManager.GetSession().Query<ExternalPaymentProvider>().Where(epp => epp.ClientID == clientId).Select(epp => epp.ClientSecret).Single();
        }

    }
}