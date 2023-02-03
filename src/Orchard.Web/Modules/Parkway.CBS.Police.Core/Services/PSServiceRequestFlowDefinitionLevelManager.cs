using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceRequestFlowDefinitionLevelManager : BaseManager<PSServiceRequestFlowDefinitionLevel>, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>
    {
        private readonly IRepository<PSServiceRequestFlowDefinitionLevel> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PSServiceRequestFlowDefinitionLevelManager(IRepository<PSServiceRequestFlowDefinitionLevel> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// This method gets the user id tied to a particular phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        public int GetAssignedApproverId(string phoneNumber, int flowDefinitionLevelId)
        {
            try
            {
                return  _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>()
                        .Where(s => s.PSSAdminUser.PhoneNumber == phoneNumber && s.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = flowDefinitionLevelId }).Single().AssignedApprover.Id;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, $"Error getting assigned approver id with the phone number {phoneNumber}");
                throw new NoRecordFoundException($"User with phone number {phoneNumber} not authorized to perform this action");
            }
        }

        /// <summary>
        /// Checks if definition level exists and is an approval
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public bool CheckIfDefinitionLevelExistAndIsApproval(int flowDefinitionLevelId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                        .Count(s => s.Id == flowDefinitionLevelId && s.WorkFlowActionValue == (int)RequestDirection.Approval) > 0;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, $"Error Checking If Definition Level Exist And Is Approval {flowDefinitionLevelId}");
                throw new NoRecordFoundException($"Error Checking If Definition Level Exist And Is Approval {flowDefinitionLevelId}");
            }
        }

        /// <summary>
        /// Checks if the flow level definition exist
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public bool CheckIfDefinitionLevelExist(int flowDefinitionLevelId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                        .Count(s => s.Id == flowDefinitionLevelId) > 0;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, $"Error Checking If Definition Level Exist {flowDefinitionLevelId}");
                throw new NoRecordFoundException($"Error Checking If Definition Level Exist {flowDefinitionLevelId}");
            }
        }


        /// <summary>
        /// This method gets the next level Id in the PSServiceRequestFlowDefinitionLevel
        /// where definition Id is given and position
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        public PSServiceRequestFlowDefinitionLevelDTO GetNextLevelDefinitionId(int definitionId, int position)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                        .Where(s => ((s.Definition == new PSServiceRequestFlowDefinition { Id = definitionId }) && (s.Position > position))).OrderBy(or => or.Position)
                        .Select(s => new PSServiceRequestFlowDefinitionLevelDTO
                        {
                            Id = s.Id,
                            RequestDirectionValue = (RequestDirection)s.WorkFlowActionValue,
                            PositionDescription = s.PositionDescription,
                            Position = s.Position,
                            ApprovalButtonName = s.ApprovalButtonName,
                            DefinitionId = s.Definition.Id
                        }).First();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting PSServiceRequestFlowDefinitionLevel with Id definition " + definitionId + " and position " + position, exception.Message));
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Get the active work flow definition for this definition level
        /// <para>That is the definition that this definition level is tied to</para>
        /// </summary>
        /// <param name="definitionLevelId"></param>
        /// <returns>int</returns>
        public int GetWorkFlowDefinitionId(int definitionLevelId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                        .Where(s => ((s == new PSServiceRequestFlowDefinitionLevel { Id = definitionLevelId }) && (s.Definition.IsActive))).Select(x => x.Definition.Id).Single();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting PSServiceRequestFlowDefinitionLevel with Id definition " + definitionLevelId + " work flow definition", exception.Message));
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Check if this definition level will be the one to enter the reference number. 
        /// This returns the next approval button name
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns>bool</returns>
        public string CheckIfCanShowRefNumberForm(int definitionId, int position)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                       .Where(s => s.Definition.Id == definitionId && s.Position > position).OrderBy(or => or.Position).First().ApprovalButtonName;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting next PSServiceRequestFlowDefinitionLevel for definition id " + definitionId + " and position " + position, exception.Message));
                throw new NoRecordFoundException();
            }

        }

        /// <summary>
        /// Check if the approver with the specified definition id and position is the last approver in the flow definition level
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CheckIfThisIsLastApprover(int definitionId, int position)
        {
            try
            {
                return _repository.Count(x => (x.Definition.Id == definitionId) && (x.Position > position) && (x.WorkFlowActionValue == (int)RequestDirection.Approval)) == 0; 
            }catch(System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting PSServiceRequestFlowDefinitionLevel for definition id " + definitionId + " and position " + position, exception.Message));
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Check if the approver with the specified definition id and position is the last approver in the flow definition level
        /// for payment request
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CheckIfThisIsLastPaymentApprover(int definitionId, int position)
        {
            try
            {
                return _repository.Count(x => (x.Definition.Id == definitionId) && (x.Position > position) && (x.WorkFlowActionValue == (int)RequestDirection.PaymentApproval)) == 0;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting PSServiceRequestFlowDefinitionLevel for definition id " + definitionId + " and position " + position, exception.Message));
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Gets flow definition levels that have a workflow action value set to Approval for the flow definition with the specified id
        /// </summary>
        /// <param name="flowDefinitionId"></param>
        /// <returns></returns>
        public IEnumerable<PSServiceRequestFlowDefinitionLevelDTO> GetApprovalDefinitionLevelsForDefinitionWithId(int flowDefinitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                    .Where(x => (x.WorkFlowActionValue == (int)RequestDirection.Approval) && (x.Definition.Id == flowDefinitionId))
                    .Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.Id, PositionName = x.PositionName, DefinitionId = x.Definition.Id, DefinitionName = x.Definition.DefinitionName });
            }catch(System.Exception exception)
            {
                Logger.Error(exception, $"Error getting approval flow definition levels for definition id {flowDefinitionId}. Exception message - {exception.Message}");
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Gets the first approval level of the flow definition with the specified id
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public PSServiceRequestFlowDefinitionLevelDTO GetFirstLevelApprovalDefinition(int definitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                    .Where(x => (x.Definition.Id == definitionId) && (x.WorkFlowActionValue == (int)RequestDirection.Approval))
                    .Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.Id, Position = x.Position })
                    .OrderBy(x => x.Position)
                    .First();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting first approval PSServiceRequestFlowDefinitionLevel for definition id {0}. Exception message - {1}", definitionId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Gets the first approval level of the flow definition with the specified id for payment
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">throws if sequence does not contain elements</exception>
        public PSServiceRequestFlowDefinitionLevelDTO GetPaymentFirstLevelApprovalDefinition(int definitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                    .Where(x => (x.Definition.Id == definitionId) && (x.WorkFlowActionValue == (int)RequestDirection.PaymentApproval))
                    .Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.Id, Position = x.Position, DefinitionId = x.Definition.Id })
                    .OrderBy(x => x.Position)
                    .First();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting first payment approval PSServiceRequestFlowDefinitionLevel for definition id {0}. Exception message - {1}", definitionId, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Gets the first approval level of the flow definition with the specified id for payment
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        public PSServiceRequestFlowDefinitionLevelDTO GetFirstPaymentApprovalFlowDefinitionLevel(int definitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                    .Where(x => (x.Definition.Id == definitionId) && (x.WorkFlowActionValue == (int)RequestDirection.PaymentApproval))
                    .Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.Id, Position = x.Position, DefinitionId = x.Definition.Id })
                    .OrderBy(x => x.Position)
                    .FirstOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting first payment approval PSServiceRequestFlowDefinitionLevel for definition id {0}. Exception message - {1}", definitionId, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Gets the payment initiator level of the flow definition with the specified id for payment
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public PSServiceRequestFlowDefinitionLevelDTO GetPaymentInitiatorFlowDefinitionLevel(int definitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                    .Where(x => (x.Definition.Id == definitionId) && (x.WorkFlowActionValue == (int)RequestDirection.InitiatePaymentRequest))
                    .Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.Id, Position = x.Position, DefinitionId = x.Definition.Id })
                    .SingleOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting first payment approval PSServiceRequestFlowDefinitionLevel for definition id {0}. Exception message - {1}", definitionId, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Get the report viewer flow definition level id
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public int GetPaymentReportViewerDefinitionLevelId(int definitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                    .Where(x => (x.Definition.Id == definitionId) && (x.WorkFlowActionValue == (int)RequestDirection.PaymentReportViewer))
                    .Select(x => x.Id)
                    .First();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Concat("Error getting first payment approval PSServiceRequestFlowDefinitionLevel for definition id {0}. Exception message - {1}", definitionId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Gets approval definition levels for flow definition with specified id at a position greater than the specified position
        /// </summary>
        /// <param name="flowDefinition"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerable<PSServiceRequestFlowDefinitionLevelDTO> GetApprovalDefinitionLevelsAfterPositionForDefinitionWithId(int flowDefinition, int position)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>()
                    .Where(x => (x.Definition.Id == flowDefinition) && (x.Position > position) && (x.WorkFlowActionValue == (int)RequestDirection.Approval))
                    .OrderBy(x => x.Position)
                    .Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.Id, PositionName = x.PositionName, DefinitionId = x.Definition.Id, DefinitionName = x.Definition.DefinitionName })
                    .Take(2)
                    .ToFuture();
            }catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}