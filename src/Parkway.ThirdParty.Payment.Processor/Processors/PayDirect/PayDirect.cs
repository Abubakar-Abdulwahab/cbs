using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Specialized;
using Parkway.ThirdParty.Payment.Processor.Models;
using System.Security.Cryptography;

namespace Parkway.ThirdParty.Payment.Processor.Processors.PayDirect
{
    public class PayDirect : BaseProcessor, IPayDirect
    {
        private readonly PayDirectConfigurations _payDirectConfigurations;

        public PayDirect(PayDirectConfigurations payDirectConfigurations)
        {
            _payDirectConfigurations = payDirectConfigurations;
        }


        /// <summary>
        /// Given an xml stream text, this method would return what function the request is meant for.
        /// Throw an exception if no function type is found
        /// </summary>
        /// <param name="xmlStreamText"></param>
        /// <returns>Functions</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Functions GetRequestFunction(string xmlStreamText)
        {
            if (xmlStreamText.Contains("CustomerInformationRequest"))
            {
                return Functions.PayDirectValidation;
            }
            if (xmlStreamText.Contains("PaymentNotificationRequest"))
            {
                return Functions.PayDirectNotification;
            }
            throw new NotImplementedException();
        }
        

        /// <summary>
        /// Check if service URL is valid
        /// <para>The service URL is gotten from the config. Node value is ServiceURL</para>
        /// </summary>
        /// <param name="serviceURL"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        public bool IsServiceURLValid(string serviceURL)
        {
            if (string.IsNullOrEmpty(serviceURL)) { return false; }
            //get service URL config from configuration obj
            Config serviceURLConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "ServiceURL").FirstOrDefault();

            if (serviceURLConfig == null)
            { throw new Exception("No service URL in configuration file"); }
            
            if(string.Equals(serviceURL, serviceURLConfig.Value.Trim(), StringComparison.OrdinalIgnoreCase)) { return true; }
            return false;
        }


        /// <summary>
        /// Deserializes the request stream string value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestStreamString"></param>
        /// <returns>T : BaseRequest</returns>
        /// <exception cref="Exception">Throw an exception if deserialization goes wrong</exception>
        public T DeserializeXMLRequest<T>(string requestStreamString) where T : BaseRequest
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(requestStreamString)))
                {
                    var _object = deserializer.Deserialize(stream);

                    return (T)_object;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get merchant ref
        /// <para>The config key of MerchantReference is checked for</para>
        /// </summary>
        /// <returns>string | null if config is null</returns>
        public string GetMerchantRef()
        {
            Config merchantRefConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "MerchantReference").FirstOrDefault();
            if(merchantRefConfig == null) { return null; }
            return merchantRefConfig.Value;
        }


        /// <summary>
        /// Check that merchant reference is valid
        /// <para>The config key of MerchantReference is checked for</para>
        /// </summary>
        /// <param name="merchantReference"></param>
        /// <returns>bool</returns>
        public bool ValidateMerchantReference(string merchantReference, bool flatScheme = false)
        {
            Config merchantRefConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "MerchantReference").FirstOrDefault();

            if (flatScheme)
            {
                merchantRefConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "FlatMerchantReference").FirstOrDefault();
            }
            else
            {
                merchantRefConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "MerchantReference").FirstOrDefault();
            }
            if (merchantRefConfig == null) { throw new Exception("No MerchantReference in configuration file"); }

            if (string.Equals(merchantReference, merchantRefConfig.Value.Trim(), StringComparison.OrdinalIgnoreCase)) { return true; }
            return false;
        }


        /// <summary>
        /// Check that merchant reference is valid
        /// <para>The config key of MerchantReference is checked for</para>
        /// </summary>
        /// <param name="merchantReference"></param>
        /// <returns>bool</returns>
        public bool ValidateFlatSchemeMerchantReference(string merchantReference)
        {
            Config merchantRefConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "FlatMerchantReference").FirstOrDefault();
            if (merchantRefConfig == null) { throw new Exception("No FlatMerchantReference in configuration file"); }

            if (string.Equals(merchantReference, merchantRefConfig.Value.Trim(), StringComparison.OrdinalIgnoreCase)) { return true; }
            return false;
        }


        /// <summary>
        /// Check if institution Id is valid
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns>bool</returns>
        public bool IsInstitutionIdValid(string institutionId, bool flatScheme = false)
        {
            if (string.IsNullOrEmpty(institutionId)) { return false; }
            Config institutionIdConfig = null;
            if (flatScheme)
            {
                institutionIdConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "FlatInstitutionId").FirstOrDefault();
            }
            else
            {
                institutionIdConfig = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "InstitutionId").FirstOrDefault();
            }
            //get service URL config from configuration obj

            if (institutionIdConfig == null)
            { throw new Exception("No institution Id in configuration file"); }

            if (string.Equals(institutionId, institutionIdConfig.Value.Trim(), StringComparison.OrdinalIgnoreCase)) { return true; }
            return false;
        }


        /// <summary>
        /// Get pay direct web payment form model
        /// </summary>
        /// <returns>PayDirectWebPaymentModel</returns>
        public PayDirectWebPaymentFormModel GetWebPaymentModel(string transactionRef, decimal amount)
        {
            PayDirectWebPaymentFormModel formModel = new PayDirectWebPaymentFormModel { };
            formModel.ProductId = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "ProductId").First().Value;
            formModel.PayItemId = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "PaymentItemId").First().Value;
            formModel.Currency = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "Currency").First().Value;
            formModel.SiteRedirectURL = _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "WebPayRedirectURL").First().Value;
            formModel.Amount = CalculateAmountPlusFeeInKobo(amount);
            formModel.TxnRef = transactionRef;
            formModel.Hash = ComputeHashFormPayDirectWebPayment(formModel);
            return formModel;
        }


        /// <summary>
        /// Gets the fee configured for this pay direct web setup
        /// </summary>
        /// <param name="amountDue"></param>
        /// <returns>decimal</returns>
        public decimal GetFeeToBeApplied(decimal amountDue)
        { return GetTransactionFee(amountDue); }


        /// <summary>
        /// Compute hash for pay direct web 
        /// </summary>
        /// <param name="formModel"></param>
        /// <returns>string</returns>
        private string ComputeHashFormPayDirectWebPayment(PayDirectWebPaymentFormModel formModel)
        {             
            string value = string.Format("{0}{1}{2}{3}{4}{5}", formModel.TxnRef, formModel.ProductId, formModel.PayItemId, formModel.Amount, formModel.SiteRedirectURL, _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "MacKey").First().Value);
            byte[] preHash = Encoding.Default.GetBytes(value);
            byte[] hash = null;
            using (SHA512 sha = SHA512.Create())
            { hash = sha.ComputeHash(preHash); }
            return BitConverter.ToString(hash).Replace("-", String.Empty).ToUpper();
        }


        /// <summary>
        /// Get the total amount payable by the customer
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>decimal</returns>
        protected string CalculateAmountPlusFeeInKobo(decimal amount)
        {
            decimal transactionFee = GetTransactionFee(amount);
            decimal totalAmount = (amount + transactionFee);
            decimal amountInKobo = Math.Round(totalAmount, 2) * 100;
            return Math.Round(amountInKobo, 0).ToString();
        }


        /// <summary>
        /// Get the transaction fee
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>decimal</returns>
        protected decimal GetTransactionFee(decimal amount)
        {
            decimal webTransactionFee = Convert.ToDecimal(_payDirectConfigurations.ConfigNodes.Where(c => c.Key == "WebPayFee").First().Value);
            decimal transactionFeeValue = webTransactionFee/100;
            decimal transactionFeeCap = Convert.ToDecimal(_payDirectConfigurations.ConfigNodes.Where(c => c.Key == "WebPayFeeCap").First().Value);

            decimal transactionFee = Math.Round((amount * transactionFeeValue), 2);
            return transactionFee > transactionFeeCap ? transactionFeeCap : transactionFee;
        }

        /// <summary>
        /// get the product Id of the merchant collection
        /// </summary>
        /// <returns>string</returns>
        public string GetProductId()
        {
            return _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "ProductId").First().Value;
        }


        /// <summary>
        /// Get hash for transaction confirmation
        /// </summary>
        /// <param name="model">PayDirectWebPaymentResponseModel</param>
        /// <param name="productId">string</param>
        /// <returns>string</returns>
        public string GetTransactionConfirmationHash(PayDirectWebPaymentResponseModel model, string productId)
        {
            string value = string.Format("{0}{1}{2}", productId, model.txnref, _payDirectConfigurations.ConfigNodes.Where(c => c.Key == "MacKey").First().Value);
            byte[] preHash = Encoding.Default.GetBytes(value);
            byte[] hash = null;
            using (SHA512 sha = SHA512.Create())
            { hash = sha.ComputeHash(preHash); }
            return BitConverter.ToString(hash).Replace("-", String.Empty).ToUpper();
        }


        /// <summary>
        /// returns amount paid less the fee applied for a given transaction
        /// </summary>
        /// <param name="amountPaid"></param>
        /// <param name="feeApplied">Fee applied</param>
        /// <returns>decimal</returns>
        public decimal GetAmountPaidLessTransactionFee(int amountPaid, decimal feeApplied)
        {
            decimal amount = ((decimal)amountPaid / 100);
            amount = Math.Round(amount, 2);
            feeApplied = Math.Round(feeApplied, 2);
            return Math.Round((amount - feeApplied), 2);
        }
    }
}
