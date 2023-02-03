using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSEscortDetailsManager : BaseManager<PSSEscortDetails>, IPSSEscortDetailsManager<PSSEscortDetails>
    {
        private readonly IRepository<PSSEscortDetails> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSEscortDetailsManager(IRepository<PSSEscortDetails> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get escort details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortDetailsDTO</returns>
        public EscortDetailsDTO GetEscortDetails(long requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortDetails>()
                  .Where(x => x.Request.Id == requestId)
                  .Select(esd => new EscortDetailsDTO
                  {
                      StartDate = esd.StartDate,
                      EndDate = esd.EndDate,
                      NumberOfOfficers = esd.NumberOfOfficers,
                      Address = esd.Address,
                      CustomerName = esd.Request.TaxEntity.Recipient,
                      FileRefNumber = esd.Request.FileRefNumber,
                      LGAId = esd.LGA.Id,
                      LGAName = esd.LGA.Name,
                      StateId = esd.State.Id,
                      StateName = esd.State.Name,
                      OfficersHaveBeenAssigned = esd.OfficersHaveBeenAssigned,
                      SubSubTaxCategoryId = (esd.TaxEntitySubSubCategory != null) ? esd.TaxEntitySubSubCategory.Id : 0,
                      Id = esd.Id,
                      CommandTypeId = esd.CommandType.Id,
                      OriginLGAName = (esd.OriginLGA != null) ? esd.OriginLGA.Name : "",
                      OriginStateId = (esd.OriginState != null) ? esd.OriginState.Id : 0,
                      OriginStateName = (esd.OriginState != null) ? esd.OriginState.Name : "",
                      OriginAddress = esd.OriginAddress
                  }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting escort details for request Id" + requestId));
                throw;
            }
        }


        /// <summary>
        /// Gets command type for escort request with specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public int GetCommandType(long requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.Id == requestId).Select(x => x.CommandType.Id).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, $"Error getting command type id for request with id {requestId}");
                throw;
            }
        }


        /// <summary>
        /// Get escort details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortRequestDetailsVM</returns>
        public EscortRequestDetailsVM GetEscortDetailsVM(long requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortDetails>()
                  .Where(x => x.Request.Id == requestId)
                  .Select(escort => new EscortRequestDetailsVM
                  {
                      TaxEntity = new TaxEntityViewModel
                      {
                          Recipient = escort.Request.TaxEntity.Recipient,
                          PhoneNumber = escort.Request.TaxEntity.PhoneNumber,
                          RCNumber = escort.Request.TaxEntity.RCNumber,
                          Address = escort.Request.TaxEntity.Address,
                          Email = escort.Request.TaxEntity.Email,
                          TaxPayerIdentificationNumber = escort.Request.TaxEntity.TaxPayerIdentificationNumber,
                          SelectedStateName = escort.Request.TaxEntity.StateLGA.State.Name,
                          SelectedState = escort.Request.TaxEntity.StateLGA.State.Id,
                          SelectedLGAName = escort.Request.TaxEntity.StateLGA.Name,
                          SelectedStateLGA = escort.Request.TaxEntity.StateLGA.Id,
                          CategoryId = escort.Request.TaxEntity.TaxEntityCategory.Id,
                      },
                      CategorySettings = escort.Request.TaxEntity.TaxEntityCategory.GetSettings(),
                      EscortInfo = new EscortRequestVM
                      {
                          SelectedCommandType = escort.CommandType.Id,
                          SelectedCommandTypeName = escort.CommandType.Name,
                          Address = escort.Address,
                          CommandAddress = escort.Request.Command.Address,
                          CommandName = escort.Request.Command.Name,
                          SelectedCommand = escort.Request.Command.Id,
                          NumberOfOfficers = escort.NumberOfOfficers,
                          StateName = escort.LGA.State.Name,
                          LGAName = escort.LGA.Name,
                          CommandStateName = escort.Request.Command.State.Name,
                          CommandLgaName = escort.Request.Command.LGA.Name,
                          SelectedState = escort.Request.Command.State.Id,
                          SelectedStateLGA = escort.Request.Command.LGA.Id,
                          StartDate = escort.StartDate.ToString("dd/MM/yyyy"),
                          EndDate = escort.EndDate.ToString("dd/MM/yyyy"),
                          DurationNumber = escort.DurationNumber,
                          DurationType = escort.DurationType,
                          OfficersHasBeenAssigned = escort.OfficersHaveBeenAssigned,
                          TaxEntitySubSubCategoryName = (escort.TaxEntitySubSubCategory != null) ? escort.TaxEntitySubSubCategory.Name : escort.TaxEntitySubCategory.Name,
                          ServiceCategoryName = escort.ServiceCategory.Name,
                          ServiceCategoryTypeName = escort.CategoryType != null? escort.CategoryType.Name : "",
                          ShowExtraFieldsForServiceCategoryType = escort.CategoryType != null ? escort.CategoryType.ShowExtraFields : false,
                          OriginStateName = escort.OriginState != null ? escort.OriginState.Name : "",
                          OriginLGAName = escort.OriginLGA != null ? escort.OriginLGA.Name : "",
                          AddressOfOriginLocation = escort.OriginAddress
                      },
                      RequestId = escort.Request.Id,
                      ServiceTypeId = escort.Request.Service.ServiceType,
                      ServiceName = escort.Request.Service.Name,
                      FileRefNumber = escort.Request.FileRefNumber,
                      Status = escort.Request.Status,
                      FlowDefinitionId = escort.Request.FlowDefinitionLevel.Definition.Id,
                      ApprovalButtonName = escort.Request.FlowDefinitionLevel.ApprovalButtonName,
                      ApprovalPartialName = escort.Request.FlowDefinitionLevel.PartialName,
                      CommandTypeId = escort.CommandType.Id,
                      CbsUser = new CBSUserVM { Name = escort.Request.CBSUser.Name, PhoneNumber = escort.Request.CBSUser.PhoneNumber, Email = escort.Request.CBSUser.Email },
                      LocationName = escort.Request.TaxEntityProfileLocation.Name
                  }).ToList().SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting escort details for request Id" + requestId));
                throw;
            }
        }

        /// <summary>
        /// Get escort details
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>EscortDetailsDTO</returns>
        public EscortDetailsDTO GetEscortDetailsVM(string fileNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortDetails>()
                  .Where(x => x.Request.FileRefNumber == fileNumber)
                  .Select(escort => new EscortDetailsDTO
                  {
                      NumberOfOfficers = escort.NumberOfOfficers,
                      StartDate = escort.StartDate
                  }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting escort details for file number" + fileNumber));
                throw;
            }
        }


        /// <summary>
        /// Set the assigned officers value to true
        /// </summary>
        /// <param name="escortDetailsId"></param>
        public void SetAssignedOfficersValueToTrue(long escortDetailsId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSEscortDetails).Name;
                string assignedOfficerFlag = nameof(PSSEscortDetails.OfficersHaveBeenAssigned);
                string updatedAtName = nameof(PSSEscortDetails.UpdatedAtUtc);
                string escortIdName = nameof(PSSEscortDetails.Id);

                var queryText = $"UPDATE esd SET esd.{assignedOfficerFlag} = :boolval, esd.{updatedAtName} = :updateDate FROM {tableName} esd WHERE {escortIdName} = :escortDetailsId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("boolval", true);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("escortDetailsId", escortDetailsId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Get escort request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>EscortDetailsVM</returns>
        public IEnumerable<EscortDetailsVM> GetEscortRequestViewDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity.Id == taxEntityId).Select(escort => new EscortDetailsVM
                {
                    TaxEntity = new TaxEntityViewModel
                    {
                        Recipient = escort.Request.TaxEntity.Recipient,
                        PhoneNumber = escort.Request.TaxEntity.PhoneNumber,
                        RCNumber = escort.Request.TaxEntity.RCNumber,
                        Address = escort.Request.TaxEntity.Address,
                        Email = escort.Request.TaxEntity.Email,
                        TaxPayerIdentificationNumber = escort.Request.TaxEntity.TaxPayerIdentificationNumber,
                        SelectedStateName = escort.Request.TaxEntity.StateLGA.State.Name,
                        SelectedLGAName = escort.Request.TaxEntity.StateLGA.Name
                    },

                    EscortInfo = new EscortRequestVM
                    {
                        CommandAddress = escort.Request.Command.Address,
                        CommandName = escort.Request.Command.Name,
                        NumberOfOfficers = escort.NumberOfOfficers,
                        StateName = escort.LGA.State.Name,
                        LGAName = escort.LGA.Name,
                        CommandStateName = escort.Request.Command.State.Name,
                        CommandLgaName = escort.Request.Command.LGA.Name,
                        StartDate = escort.StartDate.ToString("dd/MM/yyyy"),
                        EndDate = escort.EndDate.ToString("dd/MM/yyyy"),
                        DurationNumber = escort.DurationNumber,
                        DurationType = escort.DurationType,
                        Address = escort.Address,
                        ServiceName = escort.Request.Service.Name,
                        FileRefNumber = escort.Request.FileRefNumber,
                        ApprovalNumber = escort.Request.ApprovalNumber,
                        TaxEntitySubSubCategoryName = escort.TaxEntitySubSubCategory.Name,
                        ServiceCategoryName = escort.ServiceCategory.Name,
                        ServiceCategoryTypeName = escort.CategoryType != null ? escort.CategoryType.Name : "",
                        ShowExtraFieldsForServiceCategoryType = escort.CategoryType != null ? escort.CategoryType.ShowExtraFields : false,
                        OriginStateName = escort.OriginState != null ? escort.OriginState.Name : "",
                        OriginLGAName = escort.OriginLGA != null ? escort.OriginLGA.Name : "",
                        AddressOfOriginLocation = escort.OriginAddress
                    },
                    RequestStatus = escort.Request.Status
                }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting escort details for request with file number" + fileRefNumber));
                throw;
            }
        }


        /// <summary>
        /// Get escort document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IEnumerable<EscortDetailsVM> GetEscortDocumentInfo(long requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.Id == requestId).Select(escort => new EscortDetailsVM
                {
                    TaxEntity = new TaxEntityViewModel
                    {
                        Recipient = escort.Request.TaxEntity.Recipient,
                    },

                    EscortInfo = new EscortRequestVM
                    {
                        CommandName = escort.Request.Command.Name,
                        NumberOfOfficers = escort.NumberOfOfficers,
                        StateName = escort.LGA.State.Name,
                        LGAName = escort.LGA.Name,
                        CommandStateName = escort.Request.Command.State.Name,
                        CommandLgaName = escort.Request.Command.LGA.Name,
                        ServiceName = escort.Request.Service.Name,
                        ApprovalNumber = escort.Request.ApprovalNumber
                    },
                    RequestDate = escort.Request.CreatedAtUtc,
                    ApprovalDate = escort.Request.UpdatedAtUtc.Value,
                    CbsUser = new CBSUserVM { Name = escort.Request.CBSUser.Name }
                }).ToFuture();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting escort details for request with request Id" + requestId));
                throw;
            }
        }
    }
}