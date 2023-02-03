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
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceManager : BaseManager<PSService>, IPSServiceManager<PSService>
    {
        private readonly IRepository<PSService> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSServiceManager(IRepository<PSService> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get id of service with specified service type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public PSServiceVM GetServiceWithServiceType(Models.Enums.PSSServiceTypeDefinition serviceType)
        {
            return _transactionManager.GetSession().Query<PSService>().Where(x => x.ServiceType == (int)serviceType).Select(x => new PSServiceVM { ServiceId = x.Id, ServicePrefix = x.ServicePrefix, HasDifferentialWorkFlow = x.HasDifferentialWorkFlow }).SingleOrDefault();
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
                      .Select(s => s.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, PositionDescription = ld.PositionDescription, DefinitionId = ld.Definition.Id }).OrderBy(ld => ld.Position).ElementAt(0)).ToList().ElementAt(0);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting service Id definition level " + serviceId, exception.Message));
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Get the last level defintiion with the specified workflow action value
        /// <para>this method get the level attached to this service, it is used to 
        /// determine what direction and revenue heads to use for invoice generation. Return only the Id and the Postion description</para>
        /// </summary>
        /// <param name="serviceId"></param>
        public PSServiceRequestFlowDefinitionLevelDTO GetLastLevelDefinitionWithWorkflowActionValue(int serviceId, Models.Enums.RequestDirection actionValue)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSService>()
                      .Where(s => s == new PSService { Id = serviceId })
                      .Select(x => x.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, PositionDescription = ld.PositionDescription, DefinitionId = ld.Definition.Id, RequestDirectionValue = (Models.Enums.RequestDirection)ld.WorkFlowActionValue }))
                      .ToList()
                      .ElementAt(0)
                      .Where(defLevel => defLevel.RequestDirectionValue == actionValue)
                      .OrderByDescending(defLevel => defLevel.Position)
                      .FirstOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting service Id definition level " + serviceId, exception.Message));
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Checks if the <paramref name="serviceId"/> exist
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public bool CheckIfServiceIdExist(int serviceId)
        {
            return _transactionManager.GetSession().Query<PSService>().Count(x => x.Id == serviceId) > 0;
        }


        /// <summary>
        /// Gets services with no differential workflow
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PSServiceVM> GetServicesWithNoDifferentialWorkflow()
        {
            try
            {
                return _transactionManager.GetSession().Query<PSService>()
                    .Where(x => !x.HasDifferentialWorkFlow && x.IsActive)
                    .Select(x => new PSServiceVM { ServiceId = x.Id, ServiceName = x.Name });
            }catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Gets services 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PSServiceVM> GetServices()
        {
            try
            {
                return _transactionManager.GetSession().Query<PSService>()
                    .Where(x => x.IsActive)
                    .Select(x => new PSServiceVM { ServiceId = x.Id, ServiceName = x.Name });
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException();
            }
        }


        /// <summary>
        /// Gets flow definition for service type with specified id
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        public IEnumerable<PSServiceRequestFlowDefinitionDTO> GetFlowDefinitionForServiceType(int serviceTypeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSService>()
                    .Where(x => x.IsActive && x.Id == serviceTypeId)
                    .Select(x => new PSServiceRequestFlowDefinitionDTO { Id = x.FlowDefinition.Id, Name = x.FlowDefinition.DefinitionName, ServiceId = serviceTypeId });
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException();
            }
        }

    }
}