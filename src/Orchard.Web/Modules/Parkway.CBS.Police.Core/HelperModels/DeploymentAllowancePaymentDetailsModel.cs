using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class DeploymentAllowancePaymentDetailsModel
    {
        public string ReferenceNumber { get; set; }

        public string Hmac { get; set; }

        public List<DeploymentAllowancePaymentItemModel> Items { get; set; }
    }
}