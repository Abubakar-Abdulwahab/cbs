using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> : IDependency, IBaseManager<PSServiceRequestFlowDefinitionLevel>
    {

        /// <summary>
        /// Checks if definition level exists and is an approval
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        /// <exception cref="NoRecordFoundException"></exception>
        bool CheckIfDefinitionLevelExistAndIsApproval(int flowDefinitionLevelId);

        /// Checks if the flow level definition exist
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        /// <exception cref="NoRecordFoundException"></exception>
        bool CheckIfDefinitionLevelExist(int flowDefinitionLevelId);

        /// <summary>
        /// Check if the approver with the specified definition id and position is the last approver in the flow definition level
        /// for payment request
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        bool CheckIfThisIsLastPaymentApprover(int definitionId, int position);

        /// <summary>
        /// This method gets the next level in the PSServiceRequestFlowDefinitionLevel
        /// where definition Id is given and position
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        PSServiceRequestFlowDefinitionLevelDTO GetNextLevelDefinitionId(int definitionId, int position);

        /// <summary>
        /// This method gets the user id tied to a particular phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        int GetAssignedApproverId(string phoneNumber, int flowDefinitionLevelId);


        /// <summary>
        /// Get the work flow definition for this definition level
        /// <para>That is the definition that this definition level is tied to</para>
        /// </summary>
        /// <param name="definitionLevelId"></param>
        /// <returns>int</returns>
        int GetWorkFlowDefinitionId(int definitionLevelId);


        /// <summary>
        /// Check if this definition level will be the one to enter the reference number. 
        /// This returns the next approval button name
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns>string</returns>
        string CheckIfCanShowRefNumberForm(int definitionId, int position);

        /// <summary>
        /// Check if the approver with the specified definition id and position is the last approver in the flow definition level
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        bool CheckIfThisIsLastApprover(int definitionId, int position);

        /// <summary>
        /// Gets flow definition levels that have a workflow action value set to Approval for the flow definition with the specified id
        /// </summary>
        /// <param name="flowDefinitionId"></param>
        /// <returns></returns>
        IEnumerable<PSServiceRequestFlowDefinitionLevelDTO> GetApprovalDefinitionLevelsForDefinitionWithId(int flowDefinitionId);

        /// <summary>
        /// Get the report viewer flow definition level
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        int GetPaymentReportViewerDefinitionLevelId(int definitionId);

        /// <summary>
        /// Gets the first approval level of the flow definition with the specified id
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        PSServiceRequestFlowDefinitionLevelDTO GetFirstLevelApprovalDefinition(int definitionId);

        /// <summary>
        /// Gets the first approval level of the flow definition with the specified id for payment
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        PSServiceRequestFlowDefinitionLevelDTO GetPaymentFirstLevelApprovalDefinition(int definitionId);

        /// <summary>
        /// Gets the first approval level of the flow definition with the specified id for payment
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        PSServiceRequestFlowDefinitionLevelDTO GetFirstPaymentApprovalFlowDefinitionLevel(int definitionId);

        /// <summary>
        /// Gets the payment initiator level of the flow definition with the specified id for payment
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        PSServiceRequestFlowDefinitionLevelDTO GetPaymentInitiatorFlowDefinitionLevel(int definitionId);

        /// <summary>
        /// Gets approval definition levels for flow definition with specified id at a position greater than the specified position
        /// </summary>
        /// <param name="flowDefinition"></param>
        /// <param name="position"></param>
        /// <returns>IEnumerable<PSServiceRequestFlowDefinitionLevelDTO></returns>
        IEnumerable<PSServiceRequestFlowDefinitionLevelDTO> GetApprovalDefinitionLevelsAfterPositionForDefinitionWithId(int flowDefinition, int position);
    }
}
