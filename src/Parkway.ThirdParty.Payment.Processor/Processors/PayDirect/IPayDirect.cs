using Parkway.ThirdParty.Payment.Processor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.ThirdParty.Payment.Processor.Processors.PayDirect
{
    public interface IPayDirect
    {
        /// <summary>
        /// Given an xml stream text, this method would return what function the request is meant for.
        /// Throw an exception if no function type is found
        /// </summary>
        /// <param name="xmlStreamText"></param>
        /// <returns>Functions</returns>
        /// <exception cref="NotImplementedException"></exception>
        Functions GetRequestFunction(string xmlStreamText);


        /// <summary>
        /// Check if service URL is valid
        /// </summary>
        /// <param name="requestStreamString"></param>
        /// <returns>bool</returns>
        bool IsServiceURLValid(string requestStreamString);


        T DeserializeXMLRequest<T>(string requestStreamString) where T : BaseRequest;


        /// <summary>
        /// Get merchant ref
        /// <para>The config key of MerchantReference is checked for</para>
        /// </summary>
        /// <returns>string | null if config is null</returns>
        string GetMerchantRef();


        /// <summary>
        /// Check that merchant reference is valid
        /// <para>The config key of MerchantReference is checked for</para>
        /// </summary>
        /// <param name="merchantReference"></param>
        /// <param name="flatScheme"></param>
        /// <returns>bool</returns>
        bool ValidateMerchantReference(string merchantReference, bool flatScheme = false);


        /// <summary>
        /// Check if institution Id is valid
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns>bool</returns>
        bool IsInstitutionIdValid(string institutionId, bool flatScheme = false);


        /// <summary>
        /// Get pay direct web payment model
        /// </summary>
        /// <param name="amount">paymenet amount</param>
        /// <param name="transactionRef">Transaction ref</param>
        /// <returns>PayDirectWebPaymentModel</returns>
        PayDirectWebPaymentFormModel GetWebPaymentModel(string transactionRef, decimal amount);


        /// <summary>
        /// Gets the fee configured for this pay direct web setup
        /// </summary>
        /// <param name="amountDue"></param>
        /// <returns>decimal</returns>
        decimal GetFeeToBeApplied(decimal amountDue);


        /// <summary>
        /// get the product Id of the merchant collection
        /// </summary>
        /// <returns></returns>
        string GetProductId();


        /// <summary>
        /// Get hash for transaction confirmation
        /// </summary>
        /// <param name="model">PayDirectWebPaymentResponseModel</param>
        /// <param name="productId">string</param>
        /// <returns>string</returns>
        string GetTransactionConfirmationHash(PayDirectWebPaymentResponseModel model, string productId);


        /// <summary>
        /// returns amount paid less the fee applied for a given transaction
        /// </summary>
        /// <param name="amountPaid"></param>
        /// <param name="fee">Fee applied</param>
        /// <returns>decimal</returns>
        decimal GetAmountPaidLessTransactionFee(int amountPaid, decimal fee);
    }
}
