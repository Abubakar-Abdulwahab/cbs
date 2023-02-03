using Orchard;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreDeploymentAllowancePaymentService : IDependency
    {
        /// <summary>
        /// Process deployment allowance payment request
        /// </summary>
        /// <param name="paymentReference">Deployment Allowance Payment Request Payment Reference</param>
        /// <returns></returns>
        string ProcessPayment(string paymentReference);
    }
}
