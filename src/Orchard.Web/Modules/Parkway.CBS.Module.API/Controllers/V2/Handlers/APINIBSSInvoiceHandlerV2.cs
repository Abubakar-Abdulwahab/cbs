using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Models.Enums;
using Orchard;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.API.Controllers.V2.Handlers.Contracts;
using Parkway.EbillsPay.Models;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Module.API.Controllers.Handlers;
using System.Text;
using Newtonsoft.Json;

namespace Parkway.CBS.Module.API.Controllers.V2.Handlers
{
    public class APINIBSSInvoiceHandlerV2 : BaseAPIHandler, IAPINIBSSInvoiceHandlerV2
    {
        private readonly ICoreInvoiceService _coreInvoiceService;
        public readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreNIBSSIntegrationService _nibssIntegrationService;

        public APINIBSSInvoiceHandlerV2(ICoreInvoiceService coreInvoiceService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IRevenueHeadManager<RevenueHead> revenueHeadRepository, IOrchardServices orchardServices, ICoreNIBSSIntegrationService nibssIntegrationService) : base(settingsRepository)
        {
            _coreInvoiceService = coreInvoiceService;
            _settingsRepository = settingsRepository;
            _revenueHeadRepository = revenueHeadRepository;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _nibssIntegrationService = nibssIntegrationService;
        }


        /// <summary>
        /// Do invoice validate for NIBSS Ebills Pay (Encrypted JSON)
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="authorization"></param>
        /// <param name="hash"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ValidateInvoiceNIBSS(string signature, string authorization, string hash)
        {
            Params paramsObj = null;
            decimal paymentAmount;
            string IV = string.Empty;
            string secretKey = string.Empty;
            ValidationResponseJsonBaseModel responseObj = null;
            string encryptedResponseString = string.Empty;
            string responseString = string.Empty;

            try
            {
                _nibssIntegrationService.GetNibssIntegrationCredential(ref IV, ref secretKey);

                //do auth validations on secretkey and iv
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new Exception("Nibss Ebills Secret Key not found");
                }

                if (string.IsNullOrEmpty(IV))
                {
                    throw new Exception("Nibss Ebills IV not found");
                }

                //do header null checks
                if (string.IsNullOrEmpty(hash))
                {
                    Logger.Error("ValidateInvoiceNIBSS Error: Hash header value cannot be null");
                    responseObj = new ValidationResponseJsonBaseModel { Message = $"Failed", HasError = true, ErrorMessages = new List<string> { "Hash header value cannot be null" } };
                    responseString = JsonConvert.SerializeObject(responseObj);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                //do auth validations on signature
                if (string.IsNullOrEmpty(signature) || Util.Sha256Hash($"{DateTime.Now.Year}{DateTime.Now.Month:d2}{DateTime.Now.Day:d2}{secretKey}") != signature)
                {
                    Logger.Error("ValidateInvoiceNIBSS Error: Signature header value not valid");
                    responseObj = new ValidationResponseJsonBaseModel { Message = $"Failed", HasError = true, ErrorMessages = new List<string> { "Signature header value not valid" } };
                    responseString = JsonConvert.SerializeObject(responseObj);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                var validationObj = JsonConvert.DeserializeObject<ValidationRequestJson>(Util.Decrypt(hash, secretKey, IV));

                //if model is empty
                if (validationObj == null)
                {
                    Logger.Error("ValidateInvoiceNIBSS Error: Model is empty");
                    responseObj = new ValidationResponseJsonBaseModel { Message = $"Failed, {ErrorCode.PPVE}", HasError = true, ErrorMessages = new List<string> { "Model is empty" } };
                    responseString = JsonConvert.SerializeObject(responseObj);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                if (string.IsNullOrEmpty(authorization) || Convert.ToBase64String(Encoding.UTF8.GetBytes(validationObj.BillerName.ToLower().Replace(" ", ""))) != authorization)
                {
                    //authorization not valid
                    Logger.Error("ValidateInvoiceNIBSS Error: Authorization header is not Valid");
                    responseObj = new ValidationResponseJsonBaseModel { Message = $"Failed", HasError = true, ErrorMessages = new List<string> { "Authorization header value not valid" } };
                    responseString = JsonConvert.SerializeObject(responseObj);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                //do auth validation
                DoEbillsPayAuthValidation(validationObj);

                if (validationObj.Params == null || string.IsNullOrEmpty(validationObj.Params.InvoiceNumber))
                { throw new Exception("invoice object not found in the validation object"); }

                //
                InvoiceGeneratedResponseExtn response = _coreInvoiceService.GetInvoiceDetailsForPaymentView(validationObj.Params.InvoiceNumber.Trim());

                if (response == null)
                { throw new Exception(ErrorLang.invoice404(validationObj.Params.InvoiceNumber).Text); }

                //check for restrictions
                CheckForPaymentProviderRestrictions(response, (int)PaymentProvider.NIBSS);

                paramsObj = new Params
                {
                    InvoiceNumber = response.InvoiceNumber,
                    AmountToPay = response.AmountDue.ToString("F"),
                    CustomerName = response.Recipient,
                    Email = response.Email,
                    PhoneNumber = response.PhoneNumber
                };
                paymentAmount = response.AmountDue;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                responseObj = new ValidationResponseJsonBaseModel { Message = "Failed", HasError = true, ErrorMessages = new List<string> { $"{exception.Message}" } };
                responseString = JsonConvert.SerializeObject(responseObj);
                Logger.Information("NIBSS response: " + responseString);
                encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseObject = encryptedResponseString
                };
            }

            responseObj = new ValidationResponseJson { Message = $"Successful", Amount = paymentAmount, Params = paramsObj, HasError = false };
            responseString = JsonConvert.SerializeObject(responseObj);
            Logger.Information("NIBSS response: " + responseString);
            encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
            return new APIResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseObject = encryptedResponseString
            };
        }

        /// <summary>
        /// Check for payment provider restrictions
        /// </summary>
        /// <param name="invoiceDetails"></param>
        /// <param name="paymentProviderId"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        private void CheckForPaymentProviderRestrictions(InvoiceGeneratedResponseExtn invoiceDetails, int paymentProviderId)
        {
            if (_coreInvoiceService.CheckForRestrictions(invoiceDetails, paymentProviderId))
            {
                throw new UserNotAuthorizedForThisActionException(string.Format("Payment provider {2} ::: is restricted from MDA {0} and Revenue head {1} ", invoiceDetails.MDAId, invoiceDetails.RevenueHeadID, paymentProviderId));
            }
        }


        /// <summary>
        /// Do validation for NIBSS Ebills pay
        /// </summary>
        /// <param name="validationObj"></param>
        private void DoEbillsPayAuthValidation(ValidationRequestJson validationObj)
        {
            var stateName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig stateconfig = Util.GetTenantConfigBySiteName(stateName);

            if (stateconfig == null)
            {
                throw new Exception(ErrorLang.genericexception().ToString());
            }

            Node billerIdNode = null;
            Node billeNameNode = null;
            Node productIdNode = null;
            Node productNameNode = null;

            foreach (var item in stateconfig.Node)
            {
                if (item.Key == EnvKeys.EbillsBillerID.ToString()) { billerIdNode = item; continue; }
                if (item.Key == EnvKeys.EbillsBillerName.ToString()) { billeNameNode = item; continue; }
                if (item.Key == EnvKeys.EbillsProductID.ToString()) { productIdNode = item; continue; }
                if (item.Key == EnvKeys.EbillsProductName.ToString()) { productNameNode = item; continue; }
            }

            //check biller Id
            if (billerIdNode == null)
            {
                throw new Exception(string.Format("No biller Id entry found in config"));
            }

            if (validationObj.BillerID != billerIdNode.Value)
            {
                throw new Exception(string.Format("BillerID mismatch expected : {0}, actual : {1}", billerIdNode.Value, validationObj.BillerID));
            }

            //check for biller name
            if (billeNameNode == null)
            {
                throw new Exception(string.Format("No biller Id entry found in config"));
            }

            if (validationObj.BillerName != billeNameNode.Value)
            {
                throw new Exception(string.Format("billeNameNode mismatch expected : {0}, actual : {1}", billeNameNode.Value, validationObj.BillerName));
            }

            //check for product id
            if (productIdNode == null)
            {
                throw new Exception(string.Format("No product id entry found in config"));
            }

            if (validationObj.ProductID != productIdNode.Value)
            {
                throw new Exception(string.Format("product id mismatch expected : {0}, actual : {1}", productIdNode.Value, validationObj.ProductID));
            }

            //check product name
            if (productNameNode == null)
            {
                throw new Exception(string.Format("No product name entry found in config"));
            }

            if (validationObj.ProductName != productNameNode.Value)
            {
                throw new Exception(string.Format("product name mismatch expected : {0}, actual : {1}", productNameNode.Value, validationObj.ProductName));
            }
        }
    }
}