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
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services
{
    public class ServiceWorkflowDifferentialManager : BaseManager<ServiceWorkflowDifferential>, IServiceWorkflowDifferentialManager<ServiceWorkflowDifferential>
    {
        private readonly IRepository<ServiceWorkflowDifferential> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public ServiceWorkflowDifferentialManager(IRepository<ServiceWorkflowDifferential> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        
        /// <summary>
        /// Get the first level defintiion Id
        /// <para>this method get the level Id attached to this service, it is used to 
        /// determine what direction and revenue heads to use for invoice generation</para>
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differentialVM">ServiceWorkFlowDifferentialDataParam</param>
        public int GetFirstLevelDefinitionId(int serviceId, ServiceWorkFlowDifferentialDataParam differentialVM)
        {
            try
            {
                return _transactionManager.GetSession().Query<ServiceWorkflowDifferential>()
                      .Where(s => 
                      ((s.Service == new PSService { Id = serviceId }) 
                      && (s.DifferentialValue == differentialVM.DifferentialValue) 
                      && (s.DifferentialModelName == differentialVM.DifferentialModelName)
                      && (s.IsActive)))
                      .Select(s => s.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, Position = ld.Position }).OrderBy(ld => ld.Position).ElementAt(0)).ToList().ElementAt(0).Id;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting service Id definition level " + serviceId, exception.Message));
                throw new NoRecordFoundException();
            }
        }


        /// <summary>
        /// Get the work flow definition for based off the differential data param
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        public PSServiceRequestFlowDefinitionLevelDTO GetFirstLevelDefinitionObj(int serviceId, ServiceWorkFlowDifferentialDataParam differential)
        {
            try
            {
                return _transactionManager.GetSession().Query<ServiceWorkflowDifferential>()
                      .Where(s =>
                     ((s.Service == new PSService { Id = serviceId })
                      && (s.DifferentialValue == differential.DifferentialValue)
                      && (s.DifferentialModelName == differential.DifferentialModelName)
                      && (s.IsActive)))
                      .Select(s => s.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, PositionDescription = ld.PositionDescription, DefinitionId = ld.Definition.Id })
                      .OrderBy(ld => ld.Position).ElementAt(0)).ToList().ElementAt(0);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting service Id definition level " + serviceId, exception.Message));
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Get the last work flow definition with specified workflow action value based off the differential data param
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        public PSServiceRequestFlowDefinitionLevelDTO GetLastFlowDefinitionLevelObjWithWorkflowActionValue(int serviceId, ServiceWorkFlowDifferentialDataParam differential, Models.Enums.RequestDirection actionValue)
        {
            try
            {
                return _transactionManager.GetSession().Query<ServiceWorkflowDifferential>()
                      .Where(s => (s.Service.Id == serviceId) && (s.DifferentialValue == differential.DifferentialValue) && (s.DifferentialModelName == differential.DifferentialModelName) && s.IsActive)
                      .Select(x => x.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, PositionDescription = ld.PositionDescription, DefinitionId = ld.Definition.Id, RequestDirectionValue = (Models.Enums.RequestDirection)ld.WorkFlowActionValue }))
                      .ToList()
                      .ElementAt(0)
                      .Where(defLevel => defLevel.RequestDirectionValue == Models.Enums.RequestDirection.GenerateInvoice)
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
        /// Gets flow definition for service type with specified id
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        public IEnumerable<PSServiceRequestFlowDefinitionDTO> GetFlowDefinitionForServiceType(int serviceTypeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<ServiceWorkflowDifferential>()
                    .Where(x => x.IsActive && x.Service.Id == serviceTypeId)
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