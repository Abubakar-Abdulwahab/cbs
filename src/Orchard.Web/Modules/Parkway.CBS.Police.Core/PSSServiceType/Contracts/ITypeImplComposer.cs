using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.PSSServiceType.Contracts
{
    public interface ITypeImplComposer : IDependency
    {

        /// <summary>
        /// Get details for the revenue head associated with this service Id at the give process stage
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="stage"></param>
        /// <returns>IEnumerable{PSServiceRevenueHeadVM}</returns>
        IEnumerable<PSServiceRevenueHeadVM> GetRevenueHeadDetails(int serviceId, int stage);


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="expertSystem"></param>
        /// <param name="entityVM"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGenerationResponse GenerateInvoice(CreateInvoiceUserInputModel inputModel, ExpertSystemVM expertSystem, TaxEntityViewModel entityVM);


        /// <summary>
        /// Get file ref number for this request object 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string GetRequestFileRefNumber(PSSRequest request);


        /// <summary>
        /// Get root expert system
        /// </summary>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        IEnumerable<ExpertSystemVM> GetExpertSystem();


        /// <summary>
        /// Send email notification
        /// </summary>
        /// <param name="emailDetails"></param>
        void SendEmailNotification(dynamic emailDetails);


        /// <summary>
        /// Roll back transactions
        /// </summary>
        void RollBackAllTransactions();

     
        /// <summary>
        /// Get the initialization level definition id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>int</returns>
        int GetInitFlow(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams);


        /// <summary>
        /// Get the initialization level definition
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        PSServiceRequestFlowDefinitionLevelDTO GetInitFlowLevel(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams);


        /// <summary>
        /// Get the last level definition with specified workflow action value
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        PSServiceRequestFlowDefinitionLevelDTO GetLastFlowLevelWithWorkflowActionValue(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams, RequestDirection actionValue);


        /// <summary>
        /// Get the request token for this 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="expectedHash"></param>
        /// <returns></returns>
        string GetURLRequestTokenString(long requestId, string expectedHash);


        /// <summary>
        /// Do a hash for this serviceId and it's initial levelId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="levelId"></param>
        /// <returns>string</returns>
        string GetExpectedHash(int serviceId, int levelId);


        /// <summary>
        /// Save request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="requestVM"></param>
        PSSRequest SaveRequest(PSSRequestStageModel processStage, RequestDumpVM requestVM, TaxEntityViewModel taxPayerProfileVM, int definitionLevelId, PSSRequestStatus status);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="requestVM"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <param name="requestStatus"></param>
        /// <param name="formDetails"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        InvoiceGenerationResponse SaveRequestDetails(PSSRequestStageModel processStage, RequestDumpVM requestVM, TaxEntityViewModel taxPayerProfileVM, PSSRequestStatus requestStatus, IEnumerable<UserFormDetails> formDetails);


        /// <summary>
        /// Save the service and request
        /// <para>Here the relationship between the service and request is defined</para>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="serviceRevenueHeads"></param>
        /// <param name="serviceId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="definitionLevelId"></param>
        void SaveServiceRequest(PSSRequest request, IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads, int serviceId, Int64 invoiceId, int definitionLevelId, PSSRequestStatus status);


        /// <summary>
        /// Add the request and invoice joiner
        /// </summary>
        /// <param name="request"></param>
        /// <param name="invoiceId"></param>
        void AddRequestAndInvoice(PSSRequest request, long invoiceId);


        /// <summary>
        /// Send sms notification after invoice generation
        /// </summary>
        /// <param name="model"></param>
        void SendInvoiceSMSNotification(SMSDetailVM model);


        /// <summary>
        /// Add request status log
        /// </summary>
        /// <param name="requestStatus"></param>
        void AddRequestStatusLog(RequestStatusLog requestStatus);


        /// <summary>
        /// Updates request command workflow log
        /// </summary>
        /// <param name="requestDeets"></param>
        void UpdateRequestCommandWorkFlowLog(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets);


        /// <summary>
        /// Creates request command workflow log
        /// </summary>
        /// <param name="log"></param>
        void AddRequestCommandWorkFlowLog(RequestCommandWorkFlowLog log);


        /// <summary>
        /// Compute deployment allowance
        /// </summary>
        /// <param name="invoiceContributedAmount"></param>
        /// <param name="deductionPercentage"></param>
        /// <param name="paymentPercentage"></param>
        /// <returns></returns>
        decimal ComputeAllowanceFee(decimal invoiceContributedAmount, decimal deductionPercentage, decimal paymentPercentage);


        /// <summary>
        /// update the definition level for a rewuest wit the request Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newDefinitionLevelId"></param>
        /// <param name="status"></param>
        /// <exception cref="Exception">Throws exception if update fails</exception>
        void UpdateRequestDefinitionFlowLevel(long requestId, int newDefinitionLevelId, PSSRequestStatus status);


        /// <summary>
        /// Get the flow definition this definition level ID
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>int</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        int GetWorkFlowDefinition(int definitionLevelId);


        /// <summary>
        /// Save request command details
        /// </summary>
        /// <param name="request"></param>
        void SaveCommandDetails(List<RequestCommand> reqCommands);


        /// <summary>
        /// Set approval number for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="approvalNumber"></param>
        void SetApprovalNumber(long requestId, string approvalNumber);


        /// <summary>
        /// Send SMS notification to the next person to act on an approval request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="phoneNumbers"></param>
        void SendApproverSMSNotification(dynamic model, List<string> phoneNumbers);

        /// <summary>
        /// Send sms notification to notify the requester of the service about the approval
        /// </summary>
        /// <param name="smsDetails"></param>
        void SendPSSRequestApprovalSMSNotification(dynamic smsDetails);

        /// <summary>
        /// Send sms notification to notify the requester of the service about the rejection
        /// </summary>
        /// <param name="smsDetails"></param>
        void SendPSSRequestRejectionSMSNotification(dynamic smsDetails);

        /// <summary>
        /// Send any generic SMS notification
        /// </summary>
        /// <param name="smsDetails"></param>
        void SendGenericSMSNotification(List<string> phoneNumbers, string message);

        /// <summary>
        /// Get request details using user input file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>PSSRequestVM</returns>
        PSSRequestVM ConfirmFileNumber(string fileNumber);

        /// <summary>
        /// Gets police officer with specified service number
        /// </summary>
        /// <param name="serviceNumber"></param>
        /// <returns>PoliceOfficerVM</returns>
        PoliceOfficerVM GetPoliceOfficer(string serviceNumber);

        /// <summary>
        /// Sets all request command workflow logs for request with the specified id to inactive
        /// </summary>
        /// <param name="requestId"></param>
        void SetPreviousRequestCommandWorkflowLogsToInactive(long requestId);
    }
}
