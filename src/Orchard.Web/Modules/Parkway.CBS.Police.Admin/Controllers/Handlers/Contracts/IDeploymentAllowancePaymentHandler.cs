using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IDeploymentAllowancePaymentHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canInitateDeploymentAllowancePayment"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission canInitateDeploymentAllowancePayment);


        /// <summary>
        /// Get the initiate deployment allowance payment vm
        /// </summary>
        /// <returns><see cref="InitiateDeploymentAllowancePaymentVM"/></returns>
        InitiateDeploymentAllowancePaymentVM GetInitiateDeploymentAllowancePaymentVM();


        /// <summary>
        /// Initiates deployment allowance payment request
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        /// <returns>string</returns>
        string InitiateDeploymentAllowancePaymentRequest(InitiateDeploymentAllowancePaymentVM userInput, ref List<ErrorModel> errors);


        /// <summary>
        /// Populates InitiateDeploymentAllowancePaymentVM for postback
        /// </summary>
        /// <param name="userInput"></param>
        void PopulateModelForPostback(InitiateDeploymentAllowancePaymentVM userInput);


        /// <summary>
        /// Validates account number and returns the account name
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="bankCode"></param>
        /// <returns></returns>
        APIResponse ValidateAccountNumber(string accountNumber, string bankCode);


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
        APIResponse ComputeAmountForPersonnel(string startDate, string endDate, int commandTypeId, int dayTypeId, string invoiceNumber, int sourceAccountId);
    }
}
