using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxEntityProfileLocationManager : BaseManager<TaxEntityProfileLocation>, ITaxEntityProfileLocationManager<TaxEntityProfileLocation>
    {
        private readonly IRepository<TaxEntityProfileLocation> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public TaxEntityProfileLocationManager(IRepository<TaxEntityProfileLocation> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets locations of tax entity with specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public IEnumerable<TaxEntityProfileLocationVM> GetTaxEntityLocations(long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxEntityProfileLocation>()
                    .Where(x => x.TaxEntity == new TaxEntity { Id = taxEntityId })
                    .Select(x => new TaxEntityProfileLocationVM { Id = x.Id, Name = x.Name, Address = x.Address })
                    .ToList();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets tax entity profile location with the specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public TaxEntityProfileLocationVM GetTaxEntityLocationWithId(long taxEntityId, int locationId)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxEntityProfileLocation>()
                    .Where(x => x.TaxEntity == new TaxEntity { Id = taxEntityId } && x.Id == locationId)
                    .Select(x => new TaxEntityProfileLocationVM { Id = x.Id, Name = x.Name, Address = x.Address, State = x.State.Id, LGA = x.LGA.Id })
                    .SingleOrDefault();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets tax entity profile location with the specified id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public TaxEntityProfileLocationVM GetTaxEntityLocationWithId(int locationId)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxEntityProfileLocation>()
                    .Where(x => x.Id == locationId)
                    .Select(x => new TaxEntityProfileLocationVM { Id = x.Id, Name = x.Name, TaxEntity = new TaxEntityViewModel { PhoneNumber = x.TaxEntity.PhoneNumber, Email = x.TaxEntity.Email, Id = x.TaxEntity.Id, CategoryName = x.TaxEntity.TaxEntityCategory.Name, CategoryId = x.TaxEntity.TaxEntityCategory.Id }, Code = x.Code, Address = x.Address })
                    .SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets default tax entity profile location for tax entity with specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public TaxEntityProfileLocationVM GetDefaultTaxEntityLocation(long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxEntityProfileLocation>()
                    .Where(x => x.TaxEntity.Id == taxEntityId && x.IsDefault)
                    .Select(x => new TaxEntityProfileLocationVM { Id = x.Id, Name = x.Name, Code = x.Code, TaxEntityId = x.TaxEntity.Id, TaxEntity = new TaxEntityViewModel { PhoneNumber = x.TaxEntity.PhoneNumber, Email = x.TaxEntity.Email, PayerId = x.TaxEntity.PayerId, CategoryName = x.TaxEntity.TaxEntityCategory.Name }, Address = x.Address, State = x.State.Id, LGA = x.LGA.Id })
                    .SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets id of default tax entity location
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        public int GetDefaultTaxEntityLocationId(string payerId)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxEntityProfileLocation>()
                    .Where(x => x.TaxEntity.PayerId == payerId && x.IsDefault)
                    .Select(x => x.Id)
                    .SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}