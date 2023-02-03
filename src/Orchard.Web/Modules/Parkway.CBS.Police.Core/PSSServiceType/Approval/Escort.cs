using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalWorkFlow.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval
{
    public class Escort : IPSSServiceTypeApprovalImpl, IPSSServiceTypeCustomApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Escort;

        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortDetailsManager;
        private readonly Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> _escortSettingsManager;
        private readonly Lazy<IEscortRolePartialManager<EscortRolePartial>> _escortRoleManager;
        

        private readonly Lazy<IEscortApprovalWorkFlow> _escortWorkFlow;

        private readonly Lazy<IPoliceRankingManager<PoliceRanking>> _policeRankingManager;
        private readonly Lazy<IPSSEscortOfficerDetailsManager<PSSEscortOfficerDetails>> _escortOfficerDetailsManager;
        private readonly Lazy<ICoreStateAndLGA> _coreStateLGAService;
        public ILogger Logger { get; set; }
        private readonly Lazy<IPoliceOfficerManager<PoliceOfficer>> _policeOfficerRepo;
        private readonly Lazy<IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog>> _policeDeploymentLogRepo;
        private readonly Lazy<IProposedEscortOfficerManager<ProposedEscortOfficer>> _proposedEscortOfficerRepo;
        private readonly Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> _escortRateRepo;
        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly IApprovalComposition _approvalCompositionHandler;
        private readonly IPolicerOfficerLogManager<PolicerOfficerLog> _policeOfficerLogManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IEscortFormationOfficerManager<EscortFormationOfficer> _escortFormationOfficerManager;
        private readonly Lazy<IEscortProcessStageDefinitionManager<EscortProcessStageDefinition>> _escortProcessStageDefinitionManager;

        public Escort(Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortDetailsManager, IApprovalComposition approvalCompositionHandler, Lazy<IPoliceRankingManager<PoliceRanking>> policeRankingManager, Lazy<IPSSEscortOfficerDetailsManager<PSSEscortOfficerDetails>> escortOfficerDetailsManager, Lazy<ICoreStateAndLGA> coreStateLGAService, Lazy<IPoliceOfficerManager<PoliceOfficer>> policeOfficerRepo, Lazy<IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog>> policeDeploymentLogRepo, Lazy<IProposedEscortOfficerManager<ProposedEscortOfficer>> proposedEscortOfficerRepo, Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> escortRateRepo, Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> escortSettingsManager, Lazy<ICoreCommand> coreCommand, IPolicerOfficerLogManager<PolicerOfficerLog> policeOfficerLogManager, Lazy<IEscortRolePartialManager<EscortRolePartial>> escortRoleManager, IOrchardServices orchardServices, Lazy<IEscortApprovalWorkFlow> escortWorkFlow, IEscortFormationOfficerManager<EscortFormationOfficer> escortFormationOfficerManager, Lazy<IEscortProcessStageDefinitionManager<EscortProcessStageDefinition>> escortProcessStageDefinitionManager)
        {
            _escortDetailsManager = escortDetailsManager;
            _approvalCompositionHandler = approvalCompositionHandler;
            Logger = NullLogger.Instance;
            _policeRankingManager = policeRankingManager;
            _escortOfficerDetailsManager = escortOfficerDetailsManager;
            _coreStateLGAService = coreStateLGAService;
            _policeOfficerRepo = policeOfficerRepo;
            _policeDeploymentLogRepo = policeDeploymentLogRepo;
            _proposedEscortOfficerRepo = proposedEscortOfficerRepo;
            _escortRateRepo = escortRateRepo;
            _escortSettingsManager = escortSettingsManager;
            _coreCommand = coreCommand;
            _policeOfficerLogManager = policeOfficerLogManager;
            _escortRoleManager = escortRoleManager;
            _orchardServices = orchardServices;
            _escortWorkFlow = escortWorkFlow;
            _escortFormationOfficerManager = escortFormationOfficerManager;
            _escortProcessStageDefinitionManager = escortProcessStageDefinitionManager;
        }


        private EscortRequestDetailsVM GetEscortDetails(long requestId)
        {
            EscortRequestDetailsVM escort = _escortDetailsManager.Value.GetEscortDetailsVM(requestId);
            escort.EscortInfo.ProposedOfficers = _escortFormationOfficerManager.GetEscortOfficers(requestId);
            escort.ListOfCommands = (escort.EscortInfo.SelectedStateLGA != 0) ? _coreCommand.Value.GetCommandsByLGAId(escort.EscortInfo.SelectedStateLGA) : null;
            escort.ViewName = "PSSEscortDetails";
            return escort;
        }


        /// <summary>
        /// Get the Escort view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        public PSSRequestDetailsVM GetServiceRequestViewDetails(long requestId)
        {
            EscortRequestDetailsVM escort = GetEscortDetails(requestId);
            PSSRequestDetailsVM returnObj = escort;
            returnObj.ServiceVM = escort;
            return returnObj;
        }


        /// <summary>
        /// Get partials for this admin user
        /// </summary>
        /// <returns></returns>
        private List<EscortPartialVM> GetPartialsForAdminUser(int adminUserId)
        {
            return _escortRoleManager.Value.GetPartials(adminUserId).ToList();
        }

        /// <summary>
        /// Get the Escort view details using request id for approvals
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userPartId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetailsForApproval(long requestId, int userPartId)
        {
            EscortRequestDetailsVM escort = GetEscortDetails(requestId);
            escort.RequestStages = _escortProcessStageDefinitionManager.Value.GetEscortProcessStageDefinitions(escort.CommandTypeId);
            //for this admin user, lets get the partials for that are mapped to it's role
            List<EscortPartialVM> partials = GetPartialsForAdminUser(userPartId);
            foreach (var item in partials)
            {
                item.CommandTypeId = escort.EscortInfo.SelectedCommandType;
                item.UserId = userPartId;
            }
            BuildRevenueHeadFormFields(partials, requestId);

            //we need to know if the escort details has been assigned
            if (!escort.EscortInfo.OfficersHasBeenAssigned)
            {
                //check from settings if this officer is enbaled to assign officers
                if (_escortSettingsManager.Value.CanAdminAssignOfficers(userPartId, escort.FlowDefinitionId))
                {
                    escort.PoliceRanks = _policeRankingManager.Value.GetPoliceRanks();
                    escort.StateLGAs = _coreStateLGAService.Value.GetStates();
                }
            }

            List<EscortViewRubricDTO> rubricPermissions = _escortWorkFlow.Value.DoProcessLevelValidation(userPartId, escort.EscortInfo.SelectedCommandType, requestId);
            List<EscortApprovalViewPermissions> permissions = _escortWorkFlow.Value.GetPermissions(rubricPermissions);

            escort.DisplayDetailsForApproval = permissions[0] == EscortApprovalViewPermissions.CanEdit;
            escort.Partials = (partials != null && partials.Any()) ? partials : new List<EscortPartialVM> { };
            escort.EscortInfo.ProposedOfficers = _escortFormationOfficerManager.GetEscortOfficers(requestId).ToList();
            PSSRequestDetailsVM returnObj = escort;
            returnObj.ServiceVM = escort;

            return returnObj;
        }


        /// <summary>
        /// Get the form fields for this revenue head
        /// <para>this includes the data needed for the form control</para>
        /// </summary>
        /// <param name="forms"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        public void BuildRevenueHeadFormFields(IEnumerable<EscortPartialVM> partials, long requestId)
        {
            var groupByClassAssemblyAndClass = partials.GroupBy(mp => mp.ImplementationClass);

            foreach (var group in groupByClassAssemblyAndClass)
            {
                GetPartialImplmentations(group.ToList(), requestId);
            }
        }


        /// <summary>
        /// Get partial implmenetation based on the set class name
        /// </summary>
        private void GetPartialImplmentations(List<EscortPartialVM> partials, long requestId)
        {
            IEscortViewComposition partialCompImpl = ((IEscortViewComposition)Activator.CreateInstance(partials[0].ImplementationClass.Split(',')[0], partials[0].ImplementationClass.Split(',')[1]).Unwrap());
            partialCompImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);

            foreach (var item in partials)
            {
                item.RequestId = requestId;
                //item.UserId = 
                item.PartialModel =  partialCompImpl.SetPartialData(item);
            }
        }

        

        /// <summary>
        /// First we validate, if validation is correct, then we process to process approval
        /// If validation fails we return with a list of errors in the errors model object
        /// </summary>
        /// <param name="requestDetails"></param>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>RequestApprovalResponse</returns>
        public RequestApprovalResponse ValidatedAndProcessRequestApproval(GenericRequestDetails requestDetails, ref List<ErrorModel> errors, dynamic userInput)
        {
            try
            {
                //do validation
                EscortRequestDetailsVM objUserInput = (EscortRequestDetailsVM)userInput;
                objUserInput.ServiceTypeId = requestDetails.ServiceTypeId;
                //get the escort details
                EscortDetailsDTO escort = _escortDetailsManager.Value.GetEscortDetails(requestDetails.RequestId);

                //before we do the partial validation we need to know what level and role this user is on
                List<EscortViewRubricDTO> rubricPermissions = _escortWorkFlow.Value.DoProcessLevelValidation(objUserInput.ApproverId, escort.CommandTypeId, objUserInput.RequestId);

                //////List<EscortViewRubricDTO> rubricPermissions = _escortWorkFlow.Value.DoProcessLevelValidation(2, 1, 10226);
                List<EscortApprovalViewPermissions> permissions = _escortWorkFlow.Value.GetPermissions(rubricPermissions);
                if(permissions == null || permissions.Count < 1)
                {
                    Logger.Error(string.Format("This user {0} does not have permission to perform this task", objUserInput.ApproverId));
                    throw new UserNotAuthorizedForThisActionException();
                }
                //we check the permission to act on this stage
                if(permissions[0] != EscortApprovalViewPermissions.CanEdit)
                {
                    Logger.Error(string.Format("This user {0} does not have permission to perform this task", objUserInput.ApproverId));
                    throw new UserNotAuthorizedForThisActionException();
                }
                //do validation here
                List<EscortPartialVM> partials = GetPartialsForAdminUser(objUserInput.ApproverId);
                //after we have gotten the partials we want to run the validation on each partial
                objUserInput.Permissions = permissions;
                RunValidationAndSaveIfErrorOnEachPartial(partials, objUserInput, escort, ref errors);
                if(errors.Count > 0) { throw new DirtyFormDataException(); }

                IEscortViewComposition partialCompImpl = ((IEscortViewComposition)Activator.CreateInstance(partials[0].ImplementationClass.Split(',')[0], partials[0].ImplementationClass.Split(',')[1]).Unwrap());
                partialCompImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);

                RequestApprovalResponse result = partialCompImpl.OnSubmit(objUserInput.ApproverId, objUserInput.RequestId, escort.CommandTypeId);
                if (result.ResponseFromPartial) { return result; }

                //we do validation and save them in the proposed officer table for escort

                //if (!escort.OfficersHaveBeenAssigned)
                //{
                //    if (_escortSettingsManager.Value.CanAdminAssignOfficers(objUserInput.ApproverId, requestDetails.FlowDefinitionId))
                //    {
                //        var proposedOfficiers = ValidatedRequestApproval(escort, ref errors, objUserInput);
                //        //save the escort officers details
                //        SaveProposedEscortOfficersDetails(escort, proposedOfficiers);
                //        //we need to set officers assigned now that we have assigned them
                //        SetOfficersAssignedToEscortDetails(escort.Id);
                //    }
                //}
                //we need to know what to 

                //so when we are done with validation and saving the data we need to move the request to a d
                return _approvalCompositionHandler.ProcessRequestApproval(requestDetails, objUserInput.ApproverId);
            }
            catch (DirtyFormDataException exception)
            {
                if(errors.Count < 1)
                    errors.Add(new ErrorModel { ErrorMessage = exception.Message });
                _approvalCompositionHandler.RollBackAllTransactions();
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _approvalCompositionHandler.RollBackAllTransactions();
                throw;
            }

        }


        /// <summary>
        /// Here we are running th validations on each partial
        /// </summary>
        /// <param name="partials"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        private void RunValidationAndSaveIfErrorOnEachPartial(List<EscortPartialVM> partials, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors)
        {
            if (partials.Count > 1) { throw new UserNotAuthorizedForThisActionException("Missing configuration."); }

            var groupByClassAssemblyAndClass = partials.GroupBy(mp => mp.ImplementationClass);

            foreach (var group in groupByClassAssemblyAndClass)
            {
                GetPartialImplmentationsForValidations(group.ToList(), objUserInput, escortDetails, ref errors);
            }
        }


        /// <summary>
        /// Get partial implmenetation based on the set class name
        /// </summary>
        private void GetPartialImplmentationsForValidations(List<EscortPartialVM> partials, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors)
        {
            IEscortViewComposition partialCompImpl = ((IEscortViewComposition)Activator.CreateInstance(partials[0].ImplementationClass.Split(',')[0], partials[0].ImplementationClass.Split(',')[1]).Unwrap());
            partialCompImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);

            if(partials.Count> 1) { throw new UserNotAuthorizedForThisActionException("Missing configuration."); }
            //first we validate all the records
            foreach (var item in partials)
            {
                item.RequestId = objUserInput.RequestId;
                item.CommandTypeId = escortDetails.CommandTypeId;
                item.UserId = objUserInput.ApproverId;
                if(!partialCompImpl.DoValidation(item, objUserInput, ref errors))
                {
                    partialCompImpl.SaveRecords(item, objUserInput, escortDetails, ref errors);
                }
                item.PartialModel = partialCompImpl.SetPartialData(item);
            }
        }


        /// <summary>
        /// Set the escor details officers assigned value to true
        /// </summary>
        /// <param name="id"></param>
        private void SetOfficersAssignedToEscortDetails(long escortDetailsId)
        {
            _escortDetailsManager.Value.SetAssignedOfficersValueToTrue(escortDetailsId);
        }


        private void SaveProposedEscortOfficersDetails(EscortDetailsDTO escort, ICollection<ProposedEscortOffficerVM> officersSelection)
        {
            List<ProposedEscortOfficer> proposedOfficers = new List<ProposedEscortOfficer> { };

            foreach (var item in officersSelection)
            {
                proposedOfficers.Add(new ProposedEscortOfficer
                {
                    EscortDetails = new PSSEscortDetails { Id = escort.Id },
                    OfficerLog = new PolicerOfficerLog { Id = item.PoliceOfficerLogId },
                    EscortRankRate = _escortRateRepo.Value.GetRateSheetId(item.OfficerRankId, escort.PSSEscortServiceCategoryId, escort.StateId, escort.LGAId),
                });
            }

            if (!_proposedEscortOfficerRepo.Value.SaveBundleUnCommit(proposedOfficers))
            {
                throw new CouldNotSaveRecord("Could not save proposed officers bundle");
            }
        }


        private ICollection<ProposedEscortOffficerVM> ValidatedRequestApproval(EscortDetailsDTO escort, ref List<ErrorModel> errors, EscortRequestDetailsVM userInput)
        {
            //for escort validation
            //check officers count
            //if (escort.NumberOfOfficers != userInput.OfficersSelection.Count)
            //{
            //    errors.Add(new ErrorModel { ErrorMessage = PoliceErrorLang.escort_officers_assigned_mismatch(escort.NumberOfOfficers, userInput.OfficersSelection.Count).ToString(), FieldName = nameof(userInput.NumberOfOfficers) });
            //    throw new DirtyFormDataException { };
            //}
            int ind = 0;
            List<ProposedEscortOffficerVM> officers = new List<ProposedEscortOffficerVM>(userInput.OfficersSelection.Count) { };

            //let's validate that the officers selected exists
            foreach (var item in userInput.OfficersSelection)
            {
                //check that the officer exisits
                PoliceOfficerLogVM policeOfficerLogDetails = _policeOfficerLogManager.GetPoliceOfficerDetails(item.PoliceOfficerLogId);

                if (policeOfficerLogDetails == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.record404().ToString(), FieldName = $"PoliceRankSelection[{ind}]" });
                    throw new DirtyFormDataException();
                }
                //check if there's an active deployment
                if (_policeDeploymentLogRepo.Value.Count(pdl => pdl.PoliceOfficerLog == new PolicerOfficerLog { Id = item.PoliceOfficerLogId }  && pdl.IsActive) != 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = PoliceErrorLang.police_officer_is_in_active_deployment.ToString(), FieldName = $"PoliceRankSelection[{ind}]" });
                    throw new DirtyFormDataException();
                }
                ind++;

                officers.Add(new ProposedEscortOffficerVM
                {
                    OfficerCommandId = policeOfficerLogDetails.CommandId,
                    PoliceOfficerLogId = policeOfficerLogDetails.Id,
                    OfficerRankId = policeOfficerLogDetails.RankId,
                });

            }

            return officers;
        }


        public CanApproveEscortVM CanApprove(long requestId, int userPartId)
        {
            //first we need to know who is approving what
            EscortRequestDetailsVM escort = GetEscortDetails(requestId);
            //for this admin user, lets get the partials for that are mapped to it's role
            List<EscortPartialVM> partials = GetPartialsForAdminUser(userPartId);
            ///
            IEscortViewComposition partialCompImpl = ((IEscortViewComposition)Activator.CreateInstance(partials[0].ImplementationClass.Split(',')[0], partials[0].ImplementationClass.Split(',')[1]).Unwrap());
            partialCompImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);
            return partialCompImpl.CanApprove(partials, requestId, escort, userPartId);
        }


        public EscortApprovalMessage EscortApproval(long requestId, int userPartId)
        {
            //first we need to know who is approving what
            EscortRequestDetailsVM escort = GetEscortDetails(requestId);
            //for this admin user, lets get the partials for that are mapped to it's role
            List<EscortPartialVM> partials = GetPartialsForAdminUser(userPartId);
            ///
            IEscortViewComposition partialCompImpl = ((IEscortViewComposition)Activator.CreateInstance(partials[0].ImplementationClass.Split(',')[0], partials[0].ImplementationClass.Split(',')[1]).Unwrap());
            partialCompImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);
            return partialCompImpl.Approval(partials, requestId, escort, userPartId);
        }

    }
}