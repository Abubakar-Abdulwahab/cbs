using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Globalization;
using Parkway.CBS.Police.Core.CoreServices.Contracts;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class DeploymentAllowancePaymentHandler : IDeploymentAllowancePaymentHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> _walletUserConfigurationPSServiceRequestFlowApproverManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _serviceRequestFlowDefinitionLevelManager;
        private readonly IAccountWalletConfigurationManager<AccountWalletConfiguration> _accountWalletConfigurationManager;
        private readonly IDeploymentAllowancePaymentRequestManager<DeploymentAllowancePaymentRequest> _deploymentAllowancePaymentRequestManager;
        private readonly IDeploymentAllowancePaymentRequestItemManager<DeploymentAllowancePaymentRequestItem> _deploymentAllowancePaymentRequestItemManager;
        private readonly IPSSRequestInvoiceManager<PSSRequestInvoice> _requestInvoiceManager;
        private readonly IAccountNumberValidation _accountNumberValidation;
        private readonly IBankManager<Bank> _bankManager;
        private readonly IPSSEscortDayTypeManager<PSSEscortDayType> _escortDayTypeManager;
        private readonly ICommandTypeManager<Core.Models.CommandType> _commandTypeManager;
        private readonly IPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> _regularizationUnknownPoliceOfficerDeploymentContributionLogManager;
        private readonly IPSSRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog> _unknownPoliceOfficerDeploymentLogManager;
        private readonly ICoreDeploymentAllowancePaymentService _coreDeploymentAllowancePaymentService;
        ILogger Logger { get; set; }

        public DeploymentAllowancePaymentHandler(IHandlerComposition handlerComposition, IOrchardServices orchardServices, IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> walletUserConfigurationPSServiceRequestFlowApproverManager, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> serviceRequestFlowDefinitionLevelManager, IAccountWalletConfigurationManager<AccountWalletConfiguration> accountWalletConfigurationManager, IDeploymentAllowancePaymentRequestItemManager<DeploymentAllowancePaymentRequestItem> deploymentAllowancePaymentRequestItemManager, IDeploymentAllowancePaymentRequestManager<DeploymentAllowancePaymentRequest> deploymentAllowancePaymentRequestManager, IPSSRequestInvoiceManager<PSSRequestInvoice> requestInvoiceManager, IAccountNumberValidation accountNumberValidation, IBankManager<Bank> bankManager, IPSSEscortDayTypeManager<PSSEscortDayType> escortDayTypeManager, ICommandTypeManager<Core.Models.CommandType> commandTypeManager, IPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> regularizationUnknownPoliceOfficerDeploymentContributionLogManager, ICoreDeploymentAllowancePaymentService coreDeploymentAllowancePaymentService, IPSSRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog> unknownPoliceOfficerDeploymentLogManager)
        {
            _handlerComposition = handlerComposition;
            _orchardServices = orchardServices;
            _walletUserConfigurationPSServiceRequestFlowApproverManager = walletUserConfigurationPSServiceRequestFlowApproverManager;
            _serviceRequestFlowDefinitionLevelManager = serviceRequestFlowDefinitionLevelManager;
            _accountWalletConfigurationManager = accountWalletConfigurationManager;
            _deploymentAllowancePaymentRequestManager = deploymentAllowancePaymentRequestManager;
            _deploymentAllowancePaymentRequestItemManager = deploymentAllowancePaymentRequestItemManager;
            _requestInvoiceManager = requestInvoiceManager;
            _accountNumberValidation = accountNumberValidation;
            _bankManager = bankManager;
            _escortDayTypeManager = escortDayTypeManager;
            _commandTypeManager = commandTypeManager;
            _regularizationUnknownPoliceOfficerDeploymentContributionLogManager = regularizationUnknownPoliceOfficerDeploymentContributionLogManager;
            _coreDeploymentAllowancePaymentService = coreDeploymentAllowancePaymentService;
            _unknownPoliceOfficerDeploymentLogManager = unknownPoliceOfficerDeploymentLogManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canInitateAccountWalletPayment"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission canInitateDeploymentAllowancePayment)
        {
            _handlerComposition.IsAuthorized(canInitateDeploymentAllowancePayment);
        }


        /// <summary>
        /// Get the initiate deployment allowance payment vm
        /// </summary>
        /// <returns><see cref="InitiateDeploymentAllowancePaymentVM"/></returns>
        public InitiateDeploymentAllowancePaymentVM GetInitiateDeploymentAllowancePaymentVM()
        {

            return new InitiateDeploymentAllowancePaymentVM
            {
                AccountWalletConfigurations = _walletUserConfigurationPSServiceRequestFlowApproverManager.GetCommandWalletsAssignedToUser(_orchardServices.WorkContext.CurrentUser.Id, SettlementAccountType.DeploymentAllowanceSettlement),
                Banks = _bankManager.GetAllActiveBanks(),
                EscortDayTypes = _escortDayTypeManager.GetPSSEscortDayTypes(),
                CommandTypes = _commandTypeManager.GetCommandTypes()
            };
        }


        /// <summary>
        /// Initiates deployment allowance payment request
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        /// <returns>string</returns>
        public string InitiateDeploymentAllowancePaymentRequest(InitiateDeploymentAllowancePaymentVM userInput, ref List<ErrorModel> errors)
        {
            try
            {
                if(userInput == null || userInput.DeploymentAllowancePaymentRequests == null || userInput.DeploymentAllowancePaymentRequests.Count == 0) 
                {
                    errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = "No payment requests added" });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.InvoiceNumber?.Trim()))
                {
                    errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.InvoiceNumber), ErrorMessage = "Invoice number is required" });
                    throw new DirtyFormDataException();
                }

                if (userInput.SelectedSourceAccountId <= 0)
                {
                    errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.SelectedSourceAccountId), ErrorMessage = "Selected source account is not valid" });
                    throw new DirtyFormDataException();
                }

                PSSRequestInvoiceDTO requestInvoiceDTO = _requestInvoiceManager.GetPSSRequestInvoiceWithInvoiceNumber(userInput.InvoiceNumber.Trim());
                if (requestInvoiceDTO == null)
                {
                    errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.InvoiceNumber), ErrorMessage = "Invoice number is not valid" });
                    throw new DirtyFormDataException();
                }

                if (requestInvoiceDTO.InvoiceStatus != (int)CBS.Core.Models.Enums.InvoiceStatus.Paid)
                {
                    errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.InvoiceNumber), ErrorMessage = $"Expected invoice status - {nameof(CBS.Core.Models.Enums.InvoiceStatus.Paid)} Current invoice status - {(CBS.Core.Models.Enums.InvoiceStatus)requestInvoiceDTO.InvoiceStatus}. Invoice number - {userInput.InvoiceNumber}" });
                    throw new DirtyFormDataException();
                }

                AccountWalletConfigurationDTO walletDetails = _accountWalletConfigurationManager.GetAccountWalletConfigurationDetailWithCommandDetails(userInput.SelectedSourceAccountId);
                if (walletDetails == null) 
                {
                    errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.SelectedSourceAccountId), ErrorMessage = "Selected source account is not valid" });
                    throw new DirtyFormDataException();
                }

                PSServiceRequestFlowDefinitionLevelDTO paymentInitiatorFlowDefinitionLevel = _serviceRequestFlowDefinitionLevelManager.GetPaymentInitiatorFlowDefinitionLevel(walletDetails.FlowDefinitionId);
                if(paymentInitiatorFlowDefinitionLevel == null)
                {
                    throw new Exception("No payment initiator flow definition level configured");
                }

                PSServiceRequestFlowDefinitionLevelDTO firstPaymentApprovalFlowDefinitionLevel = _serviceRequestFlowDefinitionLevelManager.GetFirstPaymentApprovalFlowDefinitionLevel(walletDetails.FlowDefinitionId);

                DeploymentAllowancePaymentRequest deploymentAllowancePaymentRequest = new DeploymentAllowancePaymentRequest
                {
                    AccountWalletConfiguration = new AccountWalletConfiguration { Id = userInput.SelectedSourceAccountId },
                    InitiatedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                    PaymentRequestStatus = (int)PaymentRequestStatus.AWAITINGAPPROVAL,
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = (firstPaymentApprovalFlowDefinitionLevel == null) ? paymentInitiatorFlowDefinitionLevel.Id : firstPaymentApprovalFlowDefinitionLevel.Id },
                    Bank = new Bank { Id = walletDetails.BankId },
                    AccountName = walletDetails.AccountName,
                    AccountNumber = walletDetails.AccountNumber,
                    PSSRequestInvoice = new PSSRequestInvoice { Id = requestInvoiceDTO.Id }
                };

                if (!_deploymentAllowancePaymentRequestManager.Save(deploymentAllowancePaymentRequest))
                {
                    throw new CouldNotSaveRecord("Unable to save Deployment Allowance Payment Request");
                }

                string paymentReference = _deploymentAllowancePaymentRequestManager.GetWalletPaymentReference(deploymentAllowancePaymentRequest.Id);

                DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(DeploymentAllowancePaymentRequestItem).Name);

                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.Bank) + "_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.AccountName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.AccountNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.Amount), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.PaymentReference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.TransactionStatus), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.StartDate), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.EndDate), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.CommandType)+"_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.DayType)+"_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(DeploymentAllowancePaymentRequestItem.UpdatedAtUtc), typeof(DateTime)));

                int serialNumber = 1;

                List<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO> contributionLogDTOs = _regularizationUnknownPoliceOfficerDeploymentContributionLogManager.GetPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogs(requestInvoiceDTO.RequestId, requestInvoiceDTO.InvoiceId, walletDetails.CommandId);

                if(contributionLogDTOs == null || !contributionLogDTOs.Any())
                {
                    throw new Exception($"No regularization police officer deployment contribution log found for request with id {requestInvoiceDTO.RequestId}, command with id {walletDetails.CommandId} and invoice with id {requestInvoiceDTO.InvoiceId}");
                }

                List<DeploymentAllowancePaymentRequestItemDTO> existingItems = _deploymentAllowancePaymentRequestItemManager.GetDeploymentAllowancePaymentRequestItemsForBatchWithRequestInvoiceId(requestInvoiceDTO.Id, walletDetails.CommandId);

                Dictionary<string, decimal> deploymentAllowancePaymentRequestAmountCombos = new Dictionary<string, decimal>();

                foreach (var paymentRequestItem in userInput.DeploymentAllowancePaymentRequests)
                {
                    if (string.IsNullOrEmpty(paymentRequestItem.AccountName?.Trim()))
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Account name is required" });
                        throw new DirtyFormDataException();
                    }

                    if (string.IsNullOrEmpty(paymentRequestItem.AccountNumber?.Trim()) || paymentRequestItem.AccountNumber?.Trim().Length != 10)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Account number is not valid" });
                        throw new DirtyFormDataException();
                    }

                    if (string.IsNullOrEmpty(paymentRequestItem.BankCode?.Trim()))
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Bank Code is required" });
                        throw new DirtyFormDataException();
                    }

                    BankViewModel bank = _bankManager.GetActiveBankByBankCode(paymentRequestItem.BankCode.Trim());

                    if(bank == null)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Selected bank is not valid" });
                        throw new DirtyFormDataException();
                    }

                    DateTime startDate, endDate;

                    try
                    {
                        startDate = DateTime.ParseExact(paymentRequestItem.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error($"Unable to parse start date {paymentRequestItem.StartDate}. Exception message - {exception.Message}");
                        throw;
                    }

                    try
                    {
                        endDate = DateTime.ParseExact(paymentRequestItem.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error($"Unable to parse end date {paymentRequestItem.EndDate}. Exception message - {exception.Message}");
                        throw;
                    }

                    if(startDate > DateTime.Now.Date || endDate > DateTime.Now.Date)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Start date and end date can not be future dates" });
                        throw new DirtyFormDataException();
                    }

                    if(startDate > endDate)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Start date cannot be ahead of end date" });
                        throw new DirtyFormDataException();
                    }

                    if (_commandTypeManager.Count(x => x.Id == paymentRequestItem.CommandTypeId && x.IsActive) == 0)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Selected unit is not valid" });
                        throw new DirtyFormDataException();
                    }

                    if(_escortDayTypeManager.Count(x => x.Id == paymentRequestItem.DayTypeId && x.IsActive) == 0)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Selected day type is not valid" });
                        throw new DirtyFormDataException();
                    }

                    PSSRegularizationUnknownPoliceOfficerDeploymentLogDTO unknownPoliceOfficerDeploymentLogDTO = _unknownPoliceOfficerDeploymentLogManager.GetPSSRegularizationUnknownPoliceOfficerDeploymentLog(paymentRequestItem.CommandTypeId, paymentRequestItem.DayTypeId, walletDetails.CommandId, requestInvoiceDTO.RequestId, requestInvoiceDTO.InvoiceId);

                    if(unknownPoliceOfficerDeploymentLogDTO == null)
                    {
                        throw new Exception($"Could not get PSSRegularizationUnknownPoliceOfficerDeploymentLog with command type id {paymentRequestItem.CommandTypeId}, day type id {paymentRequestItem.DayTypeId}, command id {walletDetails.CommandId}, request id {requestInvoiceDTO.RequestId} and invoice id {requestInvoiceDTO.InvoiceId}");
                    }

                    if(startDate < unknownPoliceOfficerDeploymentLogDTO.StartDate)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Start date specified {startDate:dd/MM/yyyy} cannot be earlier than start date of the request {unknownPoliceOfficerDeploymentLogDTO.StartDate:dd/MM/yyyy}" });
                        throw new DirtyFormDataException();
                    }

                    if(endDate > unknownPoliceOfficerDeploymentLogDTO.EndDate)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"End date specified {endDate:dd/MM/yyyy} cannot be later than end date of the request {unknownPoliceOfficerDeploymentLogDTO.EndDate:dd/MM/yyyy}" });
                        throw new DirtyFormDataException();
                    }

                    var deploymentRateInfo = contributionLogDTOs.Where(x => x.CommandTypeId == paymentRequestItem.CommandTypeId && x.DayTypeId == paymentRequestItem.DayTypeId).Select(x => new { DeploymentRate = x.DeploymentRate, NumberOfOfficers = x.NumberOfOfficers, Percentage = x.DeploymentAllowancePercentage, AvailableAmount = x.DeploymentAllowanceAmount }).SingleOrDefault();

                    if(deploymentRateInfo  == null || deploymentRateInfo.DeploymentRate == 0.00m || deploymentRateInfo.NumberOfOfficers == 0)
                    {
                        throw new Exception($"Could not find deployment rate for personnel with account number {paymentRequestItem.AccountNumber}, command type id {paymentRequestItem.CommandTypeId} and day type id {paymentRequestItem.DayTypeId} in PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog for request with id {requestInvoiceDTO.RequestId} and invoice with id {requestInvoiceDTO.InvoiceId} initiated with source account attached to command with id {walletDetails.CommandId}");
                    }

                    int days = (endDate - startDate).Days + 1;
                    decimal amount = deploymentRateInfo.DeploymentRate * deploymentRateInfo.NumberOfOfficers * days * (deploymentRateInfo.Percentage/100);

                    decimal availableAmount = deploymentRateInfo.AvailableAmount;
                    decimal debitedAmount = existingItems.Where(x => x.CommandTypeId == paymentRequestItem.CommandTypeId && x.DayTypeId == paymentRequestItem.DayTypeId).Sum(x => x.Amount);

                    availableAmount -= debitedAmount;
                    if (amount > availableAmount)
                    {
                        throw new Exception($"Amount allocated for personnels in unit with command type id {paymentRequestItem.CommandTypeId} for day with day type id {paymentRequestItem.DayTypeId} exceeds the amount available. Allocated amount {amount}  Available amount {availableAmount}");
                    }

                    paymentRequestItem.Amount = amount;
                    if (deploymentAllowancePaymentRequestAmountCombos.ContainsKey($"{paymentRequestItem.CommandTypeId}-{paymentRequestItem.DayTypeId}"))
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(InitiateDeploymentAllowancePaymentVM.DeploymentAllowancePaymentRequests), ErrorMessage = $"Duplicate personnels with unit {paymentRequestItem.CommandTypeName} and day type {paymentRequestItem.DayTypeName}" });
                        throw new DirtyFormDataException();
                    }
                    else
                    {
                        deploymentAllowancePaymentRequestAmountCombos.Add($"{paymentRequestItem.CommandTypeId}-{paymentRequestItem.DayTypeId}", amount);
                    }

                    Logger.Information($"Computing amount using duration of {days} days, number of officers {deploymentRateInfo.NumberOfOfficers} and rate of {deploymentRateInfo.DeploymentRate} for personnel with Account Number {paymentRequestItem.AccountNumber}");

                    DataRow row = dataTable.NewRow();
                    row[nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest) + "_Id"] = deploymentAllowancePaymentRequest.Id;
                    row[nameof(DeploymentAllowancePaymentRequestItem.Bank) + "_Id"] = bank.Id;
                    row[nameof(DeploymentAllowancePaymentRequestItem.AccountName)] = paymentRequestItem.AccountName;
                    row[nameof(DeploymentAllowancePaymentRequestItem.AccountNumber)] = paymentRequestItem.AccountNumber;
                    row[nameof(DeploymentAllowancePaymentRequestItem.Amount)] = amount;
                    row[nameof(DeploymentAllowancePaymentRequestItem.TransactionStatus)] = (int)PaymentRequestStatus.AWAITINGAPPROVAL;
                    row[nameof(DeploymentAllowancePaymentRequestItem.PaymentReference)] = $"{paymentReference}-{serialNumber}";
                    row[nameof(DeploymentAllowancePaymentRequestItem.StartDate)] = startDate;
                    row[nameof(DeploymentAllowancePaymentRequestItem.EndDate)] = endDate;
                    row[nameof(DeploymentAllowancePaymentRequestItem.CommandType)+"_Id"] = paymentRequestItem.CommandTypeId;
                    row[nameof(DeploymentAllowancePaymentRequestItem.DayType)+"_Id"] = paymentRequestItem.DayTypeId;
                    row[nameof(DeploymentAllowancePaymentRequestItem.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(DeploymentAllowancePaymentRequestItem.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);

                    ++serialNumber;
                }

                if (!_deploymentAllowancePaymentRequestItemManager.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(DeploymentAllowancePaymentRequestItem).Name))
                {
                    throw new CouldNotSaveRecord("Unable to save Deployment Allowance Payment Request Items");
                }

                if (firstPaymentApprovalFlowDefinitionLevel == null)
                {
                    return _coreDeploymentAllowancePaymentService.ProcessPayment(paymentReference);
                }
                else
                {
                    return $"Deployment Allowance Payment Request from {userInput.SelectedSourceAccountName} wallet with a Total amount of ₦{userInput.DeploymentAllowancePaymentRequests.Sum(x => x.Amount):n2} has been successfully initiated to the following account detail(s).{string.Join(",", userInput.DeploymentAllowancePaymentRequests.Select(x => $"{x.AccountName}: {x.AccountNumber }"))}";

                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _deploymentAllowancePaymentRequestManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Populates InitiateDeploymentAllowancePaymentVM for postback
        /// </summary>
        /// <param name="userInput"></param>
        public void PopulateModelForPostback(InitiateDeploymentAllowancePaymentVM userInput)
        {
            try
            {
                userInput.AccountWalletConfigurations = _walletUserConfigurationPSServiceRequestFlowApproverManager.GetCommandWalletsAssignedToUser(_orchardServices.WorkContext.CurrentUser.Id, SettlementAccountType.DeploymentAllowanceSettlement);
                userInput.Banks = _bankManager.GetAllActiveBanks();
                userInput.EscortDayTypes = _escortDayTypeManager.GetPSSEscortDayTypes();
                userInput.CommandTypes = _commandTypeManager.GetCommandTypes();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Validates account number and returns the account name
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="bankCode"></param>
        /// <returns></returns>
        public APIResponse ValidateAccountNumber(string accountNumber, string bankCode)
        {
            return new APIResponse { ResponseObject = _accountNumberValidation.ValidateAccountNumber(accountNumber, bankCode) };
        }


        /// <summary>
        /// Compute amount for personnel using specified parameters
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="commandTypeId"></param>
        /// <param name="dayTypeId"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="sourceAccountId"></param>
        /// <returns></returns>
        public APIResponse ComputeAmountForPersonnel(string startDate, string endDate, int commandTypeId, int dayTypeId, string invoiceNumber, int sourceAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate?.Trim()))
                {
                    Logger.Error($"Start date {startDate} is not valid");
                    return new APIResponse { Error = true, ResponseObject = $"Start date {startDate} is not valid" };
                }

                if (string.IsNullOrEmpty(endDate?.Trim()))
                {
                    Logger.Error($"End date {endDate} is not valid");
                    return new APIResponse { Error = true, ResponseObject = $"End date {startDate} is not valid" };
                }

                if (commandTypeId <= 0) 
                {
                    Logger.Error("Selected command type is not valid");
                    return new APIResponse { Error = true, ResponseObject = "Selected unit is not valid" };
                }

                if (dayTypeId <= 0) 
                {
                    Logger.Error("Selected day type is not valid");
                    return new APIResponse { Error = true, ResponseObject = "Selected day type is not valid" };
                }

                if (sourceAccountId <= 0) 
                {
                    Logger.Error("Selected source account is not valid");
                    return new APIResponse { Error = true, ResponseObject = "Selected source account is not valid" };
                }

                if (string.IsNullOrEmpty(invoiceNumber?.Trim())) 
                {
                    Logger.Error("Invoice number is required");
                    return new APIResponse { Error = true, ResponseObject = "Invoice number is required" };
                }

                DateTime startDateParsed, endDateParsed;

                try
                {
                    startDateParsed = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception exception)
                {
                    Logger.Error($"Unable to parse start date {startDate}. Exception message - {exception.Message}");
                    return new APIResponse { Error = true, ResponseObject = "Start date is not valid" };
                }

                try
                {
                    endDateParsed = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception exception)
                {
                    Logger.Error($"Unable to parse end date {endDate}. Exception message - {exception.Message}");
                    return new APIResponse { Error = true, ResponseObject = "End date is not valid" };
                }

                PSSRequestInvoiceDTO requestInvoiceDTO = _requestInvoiceManager.GetPSSRequestInvoiceWithInvoiceNumber(invoiceNumber.Trim());
                if (requestInvoiceDTO == null)
                {
                    Logger.Error($"Invoice number {invoiceNumber} is not valid");
                    return new APIResponse { Error = true, ResponseObject = "Invoice number is not valid" };
                }

                var sourceAccountCommand = _accountWalletConfigurationManager.GetCommandAttachedToSourceAccount(sourceAccountId);
                if(sourceAccountCommand == null)
                {
                    Logger.Error($"No command found for account wallet configuration with id {sourceAccountId}");
                    return new APIResponse { Error = true, ResponseObject = "Selected source account is not valid" };
                }

                PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO contributionLogDTO = _regularizationUnknownPoliceOfficerDeploymentContributionLogManager.GetDeploymentContributionLog(requestInvoiceDTO.RequestId, requestInvoiceDTO.InvoiceId, sourceAccountCommand.Id, commandTypeId, dayTypeId);
                if (contributionLogDTO == null)
                {
                    Logger.Error($"No deployment allowance contribution log found for request with id {requestInvoiceDTO.RequestId}, invoice with id {requestInvoiceDTO.InvoiceId}, command with id {sourceAccountCommand.Id}, command type with id {commandTypeId} and day type with id {dayTypeId}");
                    return new APIResponse { Error = true, ResponseObject = "No officers belonging to the selected unit and day type were selected for the request attached to the given invoice number" };
                }

                int days = (endDateParsed - startDateParsed).Days + 1;
                decimal amount = contributionLogDTO.DeploymentRate * contributionLogDTO.NumberOfOfficers * days * (contributionLogDTO.DeploymentAllowancePercentage / 100);

                return new APIResponse { ResponseObject = amount.ToString("N2") };
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}