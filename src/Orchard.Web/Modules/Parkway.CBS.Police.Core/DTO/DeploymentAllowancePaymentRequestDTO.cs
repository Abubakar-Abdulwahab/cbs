using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.DTO
{
    public class DeploymentAllowancePaymentRequestDTO
    {
        public long Id { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public BankVM Bank { get; set; }

        public string PaymentReference { get; set; }

        public PSServiceRequestFlowDefinitionLevelDTO PSServiceRequestFlowDefinitionLevel { get; set; }
    }
}