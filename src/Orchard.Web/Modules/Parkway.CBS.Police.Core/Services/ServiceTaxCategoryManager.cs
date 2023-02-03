using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using NHibernate.Criterion;
using NHibernate.Transform;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Police.Core.Services
{
    public class ServiceTaxCategoryManager : BaseManager<ServiceTaxCategory>, IServiceTaxCategoryManager<ServiceTaxCategory>
    {
        private readonly IRepository<ServiceTaxCategory> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public ServiceTaxCategoryManager(IRepository<ServiceTaxCategory> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get the list of PSS services
        /// <para>ToFuture query</para>
        /// </summary>
        /// <returns>IEnumerable<PSSRequestTypeVM></returns>
        public IEnumerable<PSSRequestTypeVM> GetAllServices()
        {
            return _transactionManager.GetSession().Query<PSService>()
               .Select(sr => new PSSRequestTypeVM
               {
                   Id = sr.Id,
                   Name = sr.Name
               }).ToFuture<PSSRequestTypeVM>();
        }


        /// <summary>
        /// Get the list of active PSS services
        /// <para>Future query</para>
        /// </summary>
        /// <returns>IEnumerable<PSSRequestTypeVM></returns>
        public IEnumerable<PSSRequestTypeVM> GetAllActiveServices(int categoryId)
        {
            return _transactionManager.GetSession()
               .CreateCriteria<ServiceTaxCategory>(nameof(ServiceTaxCategory))
                .AddOrder(Order.Desc(nameof(ServiceTaxCategory.Service) + "." + nameof(ServiceTaxCategory.Service.Name)))
                   .CreateAlias(nameof(ServiceTaxCategory.Service), nameof(ServiceTaxCategory.Service))
                   .CreateAlias(nameof(ServiceTaxCategory.TaxCategory), nameof(ServiceTaxCategory.TaxCategory))
               .Add(Restrictions.Where<ServiceTaxCategory>(c => ((c.IsActive) && (c.TaxCategory ==  new CBS.Core.Models.TaxEntityCategory { Id = categoryId }))))
               .SetProjection(Projections.ProjectionList()
               .Add(Projections.Property(nameof(ServiceTaxCategory.Service) + "." + nameof(ServiceTaxCategory.Service.Name)), nameof(PSSRequestTypeVM.Name))
               .Add(Projections.Property(nameof(ServiceTaxCategory.Service) + "." + nameof(ServiceTaxCategory.Service.Id)), nameof(PSSRequestTypeVM.Id))
               .Add(Projections.Property(nameof(ServiceTaxCategory.Service) + "." + nameof(ServiceTaxCategory.Service.ServiceType)), nameof(PSSRequestTypeVM.ServiceType)))
               .SetResultTransformer(Transformers.AliasToBean<PSSRequestTypeVM>())
               .Future<PSSRequestTypeVM>();
        }


        /// <summary>
        /// Get the first level defintiion Id
        /// <para>this method get the level Id attached to this service, it is used to 
        /// determine what direction and revenue heads to use for invoice generation</para>
        /// </summary>
        /// <param name="serviceId"></param>
        public int GetFirstLevelDefinitionId(int serviceId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSService>()
                      .Where(s => s == new PSService { Id = serviceId })
                      .Select(s => s.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, Position = ld.Position }).OrderBy(ld => ld.Position).ElementAt(0)).ToList().ElementAt(0).Id;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting service Id definition level " + serviceId, exception.Message));
                throw new NoRecordFoundException();
            }
        }


        /// <summary>
        /// Get the first level defintiion
        /// <para>this method get the level attached to this service, it is used to 
        /// determine what direction and revenue heads to use for invoice generation. Return only the Id and the Postion description</para>
        /// </summary>
        /// <param name="serviceId"></param>
        public PSServiceRequestFlowDefinitionLevelDTO GetFirstLevelDefinition(int serviceId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSService>()
                      .Where(s => s == new PSService { Id = serviceId })
                      .Select(s => s.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, PositionDescription = ld.PositionDescription }).OrderBy(ld => ld.Position).ElementAt(0)).ToList().ElementAt(0);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting service Id definition level " + serviceId, exception.Message));
                throw new NoRecordFoundException();
            }
        }


        /// <summary>
        /// Get service type
        /// </summary>
        /// <param name="id">ID of the service</param>
        /// <param name="categoryId">categoryId of the tax payer</param>
        /// <returns>PSServiceVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public PSServiceVM GetServiceType(int id, int categoryId)
        {
            var result = _transactionManager.GetSession().Query<ServiceTaxCategory>().Where(s => ((s.Service == new PSService { Id = id }) && (s.TaxCategory == new CBS.Core.Models.TaxEntityCategory { Id = categoryId })))
               .Select(sr => new PSServiceVM
               {
                   ServiceType = sr.Service.ServiceType,
                   IsActive = sr.IsActive,
                   ServicePrefix = sr.Service.ServicePrefix,
                   ServiceName = sr.Service.Name,
                   ServiceNote = sr.Service.ServiceNotes,
                   HasDifferentialWorkFlow = sr.Service.HasDifferentialWorkFlow
               }).SingleOrDefault();

            if (result == null) { throw new NoRecordFoundException("No service record foound for Id " + id); }
            return result;
        }


        /// <summary>
        /// Get flow definition Id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>int</returns>
        public int GetFlowDefinitionId(int serviceId)
        {
            return _transactionManager.GetSession().Query<PSService>()
                .Where(s => ((s.Id == serviceId) && (s.IsActive)))
                .Select(s => s.FlowDefinition.Id).FirstOrDefault();
        }


    }
}