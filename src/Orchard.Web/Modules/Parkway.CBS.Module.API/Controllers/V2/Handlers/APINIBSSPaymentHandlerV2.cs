using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using Newtonsoft.Json;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Exceptions;
using System.Linq;
using Parkway.CBS.Module.API.Controllers.V2.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.EbillsPay.Models;
using Parkway.EbillsPay;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Module.API.Controllers.Handlers;
using Orchard.Services;
using System.Text;
using Parkway.CBS.CacheProvider;

namespace Parkway.CBS.Module.API.Controllers.V2.Handlers
{
    public class APINIBSSPaymentHandlerV2 : BaseAPIHandler, INIBSSPaymentHandlerV2
    {
        private readonly ICorePaymentService _corePaymentService;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreNIBSSIntegrationService _nibssIntegrationService;

        public APINIBSSPaymentHandlerV2(ICorePaymentService corePaymentService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IOrchardServices orchardServices, ICoreNIBSSIntegrationService nibssIntegrationService) : base(settingsRepository)
        {
            _corePaymentService = corePaymentService;
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _nibssIntegrationService = nibssIntegrationService;
        }


        /// <summary>
        /// Payment notification for NIBSS EBills pay (Encrypted JSON)
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="authorization"></param>
        /// <param name="hash"></param>
        /// <returns>APIResponse</returns>
        public APIResponse NIBSSPaymentNotif(string signature, string authorization, string hash)
        {
            //NotificationRequest
            NotificationResponseJson notifResponse = null;
            string invoiceNumber = string.Empty;
            string encryptedResponseString = string.Empty;
            string IV = string.Empty;
            string secretKey = string.Empty;
            string responseString = string.Empty;

            try
            {
                notifResponse = new NotificationResponseJson { };

                _nibssIntegrationService.GetNibssIntegrationCredential(ref IV, ref secretKey);

                //do auth validations on secretkey, iv
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
                    Logger.Error("NIBSSPaymentNotif Error: Hash header value cannot be null");
                    notifResponse.Message = "Failed";
                    notifResponse.HasError = true;
                    notifResponse.ErrorMessages = new List<string> { "Hash header value cannot be null" };
                    responseString = JsonConvert.SerializeObject(notifResponse);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                //do signature checks
                if (string.IsNullOrEmpty(signature) || Util.Sha256Hash($"{DateTime.Now.Year}{DateTime.Now.Month:d2}{DateTime.Now.Day:d2}{secretKey}") != signature)
                {
                    Logger.Error("NIBSSPaymentNotif Error: Signature header value not valid");
                    notifResponse.Message = "Failed";
                    notifResponse.HasError = true;
                    notifResponse.ErrorMessages = new List<string> { "Signature header value not valid" };
                    responseString = JsonConvert.SerializeObject(notifResponse);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                
                var notifObj = JsonConvert.DeserializeObject<NotificationRequestJson>(Util.Decrypt(hash, secretKey, IV));

                //if notifObj is empty
                if (notifObj == null)
                {
                    Logger.Error("NIBSSPaymentNotif Error: Model is empty");
                    notifResponse.Message = "Failed";
                    notifResponse.HasError = true;
                    notifResponse.ErrorMessages = new List<string> { "Model is empty" };
                    responseString = JsonConvert.SerializeObject(notifResponse);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                //do authorization checks
                if (string.IsNullOrEmpty(authorization) || Convert.ToBase64String(Encoding.UTF8.GetBytes(notifObj.BillerName.ToLower().Replace(" ", ""))) != authorization)
                {
                    //authorization not valid
                    Logger.Error("NIBSSPaymentNotif Error: Authorization header is not Valid");
                    notifResponse.Message = "Failed";
                    notifResponse.HasError = true;
                    notifResponse.ErrorMessages = new List<string> { "Authorization header is not Valid" };
                    responseString = JsonConvert.SerializeObject(notifResponse);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                if (string.IsNullOrEmpty(notifObj.SessionID))
                {
                    notifResponse.Message = "Invalid Session or Record ID.";
                    throw new Exception("SessionID field is empty " + ErrorLang.norecord404().Text);
                }

                //do auth validation
                DoRequestValidation(notifObj);

                //get the invoice number
                invoiceNumber = notifObj.Params?.InvoiceNumber;
                string phoneNumber = notifObj.Params?.PhoneNumber;
                string email = notifObj.Params?.Email;

                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    Logger.Error("NIBSSPaymentNotif Error: Invoice Number not found in the notification object");
                    notifResponse.Message = "Failed";
                    notifResponse.HasError = true;
                    notifResponse.ErrorMessages = new List<string> { "Invalid Invoice Number" };
                    responseString = JsonConvert.SerializeObject(notifResponse);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                if (string.IsNullOrEmpty(notifObj.BankName))
                {
                    Logger.Error("NIBSSPaymentNotif Error: Bank name is empty " + ErrorLang.norecord404().Text);
                    notifResponse.Message = "Failed";
                    notifResponse.HasError = true;
                    notifResponse.ErrorMessages = new List<string> { "Invalid Bank name" };
                    responseString = JsonConvert.SerializeObject(notifResponse);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                if (string.IsNullOrEmpty(notifObj.TransactionDate))
                {
                    Logger.Error("NIBSSPaymentNotif Error: Transaction date is empty " + ErrorLang.norecord404().Text);
                    notifResponse.Message = "Failed";
                    notifResponse.HasError = true;
                    notifResponse.ErrorMessages = new List<string> { "Invalid Transaction date" };
                    responseString = JsonConvert.SerializeObject(notifResponse);
                    Logger.Information("NIBSS response: " + responseString);
                    encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
                    return new APIResponse
                    {
                        ResponseObject = encryptedResponseString,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                PaymentChannel channel = GetChannelTypeForNIBSS(notifObj.ChannelCode);

                DateTime.TryParse(notifObj.TransactionDate, out DateTime transactionDate);
                TimeSpan.TryParse(notifObj.TransactionTime, out TimeSpan transactionTime);
                transactionDate = transactionDate.Date.Add(transactionTime);

                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)channel,
                    InvoiceNumber = invoiceNumber,
                    PaymentReference = notifObj.SessionID,
                    AmountPaid = notifObj.Amount,
                    PaymentDate = transactionDate,
                    TransactionDate = transactionDate,
                    RequestDump = JsonConvert.SerializeObject(notifObj),
                    Bank = notifObj.BankName,
                    BankBranch = notifObj.BranchName,
                    PayerPhoneNumber = phoneNumber,
                    TypeID = (int)PaymentType.Credit,
                    TotalAmountPaid = notifObj.Amount,
                    PaymentProvider = (int)PaymentProvider.NIBSS,
                    BankChannel = notifObj.ChannelCode,
                    PayerEmail = email
                }, PaymentProvider.NIBSS);


                notifResponse.Message = Lang.paymentnotificationsuccessful.Text;
            }
            catch (NoRecordFoundException exception)
            {
                notifResponse.Message = "Failed";
                notifResponse.HasError = true;
                notifResponse.ErrorMessages = new List<string> { "Exception NIBSSPaymentNotif: " + ErrorLang.datamismatch().Text };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                notifResponse.Message = "Failed";
                notifResponse.HasError = true;
                notifResponse.ErrorMessages = new List<string> { "Exception NIBSSPaymentNotif: " + ErrorLang.norecord404().Text };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (PaymentNoficationAlreadyExistsException exception)
            {
                notifResponse.Message = "Failed";
                notifResponse.HasError = true;
                notifResponse.ErrorMessages = new List<string> { "Exception NIBSSPaymentNotif: " + Lang.paymentnotificationalreadyprocess.Text };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                notifResponse.Message = "Failed";
                notifResponse.HasError = true;
                notifResponse.ErrorMessages = new List<string> { "Exception NIBSSPaymentNotif: " + ErrorLang.invoiceFullyPaid(invoiceNumber).Text };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (PartPaymentNotAllowedException exception)
            {
                notifResponse.Message = "Failed";
                notifResponse.HasError = true;
                notifResponse.ErrorMessages = new List<string> { "Exception NIBSSPaymentNotif: " + ErrorLang.nopartpaymentsallow().Text };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (Exception exception)
            {
                notifResponse.Message = "Failed";
                notifResponse.HasError = true;
                notifResponse.ErrorMessages = new List<string> { "Exception NIBSSPaymentNotif: " + ErrorLang.genericexception().Text };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            responseString = JsonConvert.SerializeObject(notifResponse);
            Logger.Information("NIBSS response: " + responseString);
            encryptedResponseString = Util.Encrypt(responseString, secretKey, IV);
            return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = encryptedResponseString };
        }


        /// <summary>
        /// Do request validation for NIBSS
        /// </summary>
        /// <param name="notifObj"></param>
        private void DoRequestValidation(NotificationRequestJson notifObj)
        {
            var stateName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig stateconfig = Util.GetTenantConfigBySiteName(stateName);

            if (stateconfig == null)
            {
                throw new Exception(string.Format("No state config found. From Request {0}. State {1}", notifObj.SessionID, stateName));
            }

            Node billeNameNode = null;
            Node productIdNode = null;

            foreach (var item in stateconfig.Node)
            {
                if (item.Key == EnvKeys.EbillsBillerName.ToString()) { billeNameNode = item; continue; }
                if (item.Key == EnvKeys.EbillsProductID.ToString()) { productIdNode = item; continue; }
            }

            //check for biller name
            if (billeNameNode == null)
            {
                throw new Exception(string.Format("No biller found in config {0}.", notifObj.BillerName));
            }

            if (notifObj.BillerName != billeNameNode.Value)
            {
                throw new Exception(string.Format("billeNameNode mismatch expected : {0}, actual : {1}", billeNameNode.Value, notifObj.BillerName));
            }

            //check for product id
            if (productIdNode == null)
            {
                throw new Exception(string.Format("No product id entry found in {0} config", stateName));
            }

            if (notifObj.ProductID != productIdNode.Value)
            {
                throw new Exception(string.Format("product id mismatch expected : {0}, actual : {1}", productIdNode.Value, notifObj.ProductID));
            }
        }


        /// <summary>
        /// Get payment channel mapping for NIBSS channel code
        /// </summary>
        /// <param name="channelCode"></param>
        /// <returns>PaymentChannel</returns>
        private PaymentChannel GetChannelTypeForNIBSS(string channelCode)
        {
            switch (channelCode)
            {
                case "1":
                    return PaymentChannel.BankBranch;
                case "2":
                    return PaymentChannel.Web;
                case "3":
                    return PaymentChannel.MOB;
                case "4":
                    return PaymentChannel.POS;
                case "5":
                    return PaymentChannel.ATM;
                case "6":
                    return PaymentChannel.VendorMerchantWebPortal;
                case "7":
                    return PaymentChannel.ThirdPartyPaymentPlatform;
                default:
                    return PaymentChannel.OtherChannels;
            }
        }
    }
}