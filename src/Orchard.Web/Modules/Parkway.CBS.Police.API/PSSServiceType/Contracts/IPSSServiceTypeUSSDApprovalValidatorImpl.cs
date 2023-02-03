using Orchard;

namespace Parkway.CBS.Police.Core.PSSServiceType.Contracts
{
    public interface IPSSServiceTypeUSSDApprovalValidatorImpl : IDependency
    {
        Models.Enums.PSSServiceTypeDefinition GetServiceTypeDefinition { get; }

        /// <summary>
        /// Validate that the approver can approve for the request command
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="fileNumber"></param>
        void ValidateApproverCommand(int flowDefinitionLevelId, string phoneNumber, string fileNumber);
    }
}
