using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class CBSUserTaxEntityProfileLocationManager : BaseManager<CBSUserTaxEntityProfileLocation>, ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation>
    {
        private readonly IRepository<CBSUserTaxEntityProfileLocation> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public CBSUserTaxEntityProfileLocationManager(IRepository<CBSUserTaxEntityProfileLocation> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _user = user;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets tax entity profile location id for cbs user with the specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        public int GetCBSUserLocationId(long cbsUserId)
        {
            try
            {
                return _transactionManager.GetSession()
                    .Query<CBSUserTaxEntityProfileLocation>()
                    .Where(x => x.CBSUser == new CBSUser { Id = cbsUserId })
                    .Select(x => x.TaxEntityProfileLocation.Id)
                    .SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets location for cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        public TaxEntityProfileLocationVM GetCBSUserLocationWithId(long cbsUserId)
        {
            try
            {
                return _transactionManager.GetSession()
                        .Query<CBSUserTaxEntityProfileLocation>()
                        .Where(x => x.CBSUser.Id == cbsUserId)
                        .Select(x => new TaxEntityProfileLocationVM { Name = x.TaxEntityProfileLocation.Name, State = x.TaxEntityProfileLocation.State.Id, StateName = x.TaxEntityProfileLocation.State.Name, LGAName = x.TaxEntityProfileLocation.LGA.Name, LGA = x.TaxEntityProfileLocation.LGA.Id, Address = x.TaxEntityProfileLocation.Address })
                        .SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets cbs user in location with specified id
        /// </summary>
        /// <param name="taxEntityProfileLocationId"></param>
        /// <returns></returns>
        public CBSUserVM GetSubUserInLocation(int taxEntityProfileLocationId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CBSUserTaxEntityProfileLocation>().Where(x => x.TaxEntityProfileLocation.Id == taxEntityProfileLocationId)
                    .Select(x => new CBSUserVM { Id = x.CBSUser.Id, TaxEntity = new TaxEntityViewModel { Id = x.CBSUser.TaxEntity.Id, CategoryId = x.CBSUser.TaxEntity.TaxEntityCategory.Id } }).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}