using CBSPay.Core.Helpers;
using CBSPay.Core.Models;
using CBSPay.Core.ViewModels;
using log4net;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using CBSPay.Core.Interfaces;
using CBSPay.Core.APIModels;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using System.IO;

namespace CBSPay.Core.Services
{
    /// <summary>
    /// Performs all REST-related calls
    /// </summary>
    public class RestService : IRestService
    {
        //private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }
        private ILog Logger
        {
            get { return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); }
        }
        public RestService()
        {

        }

        public EIRSAPILoginResponse GetEIRSAPIToken()
        {
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string loginUrl = Configuration.GetEIRSAPILoginUrl;
            var loginVal = Utils.ConfigValueIsNull(loginUrl);
            if (loginVal)
            {
                Logger.Error("could not find endpoint url for accessing login token");
                return null;
            }

            try
            {
                var contentType = Configuration.GetEIRSAPILoginContentType.ToString();
                var acceptValue = Configuration.GetEIRSAPILoginAcceptValue.ToString();

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(loginUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddHeader("Content-Type", contentType);
                request.AddHeader("Accept", acceptValue);

                var obj = new EIRSAPILoginRequest
                {
                    grant_type = Configuration.GetEIRSAPILoginGrantType,
                    Password = Configuration.GetEIRSAPILoginPassword,
                    UserName = Configuration.GetEIRSAPILoginUsername
                };

                Logger.Info("Make call to get Login Token Details");
                //request.AddParameter(contentType, obj, contentType, ParameterType.RequestBody);
                request.AddParameter(contentType, $"Username={obj.UserName}&Password={obj.Password}&grant_type={obj.grant_type}", ParameterType.RequestBody);
                IRestResponse<EIRSAPILoginResponse> response = client.Execute<EIRSAPILoginResponse>(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK )
                {
                    Logger.Error("An error occured, could not retrieve Token Details");
                    var error = $"An error with status code - {response.StatusCode} occurred when trying to retrieve Token details;" +
                        $"the error message is {response.ErrorMessage}";
                    Logger.Error(error);
                    return null;
                }
                var res = response.Data;
                Logger.Info("Successfully retrieved Token details");
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Token Details");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        public EIRSAPIResponse GetAssessmentDetailsByRefNumber(string referenceNumber)
        {
            Logger.Debug($"About to fetch assessment details from the API for ref {referenceNumber}");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string assessmentDetailsUrl = Configuration.GetAssessmentDetailUrl;
            var assessmentVal = Utils.ConfigValueIsNull(assessmentDetailsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for accessing assessment Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(assessmentDetailsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("refno", referenceNumber, ParameterType.UrlSegment);
                //request.AddParameter("mobilenumber", phoneNumber, ParameterType.UrlSegment);

                Logger.Info("Make call to get Assessment Details ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success == false)
                {
                    Logger.Error("An error occured, while retrieving assessment Details");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved Assessment details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Assessment Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public EIRSAPIResponse GetServiceBillDetailsByRefNumber(string referenceNumber)
        {
            Logger.Debug($"About to fetch service bill details from EIRS API for ref {referenceNumber}");

            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string serviceBillDetailsUrl = Configuration.GetServiceBillDetailsUrl;
            var assessmentVal = Utils.ConfigValueIsNull(serviceBillDetailsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for accessing service bill Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(serviceBillDetailsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("refno", referenceNumber, ParameterType.UrlSegment);
                //request.AddParameter("mobilenumber", phoneNumber, ParameterType.UrlSegment);

                Logger.Info("Make call to get Service Bill Details ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);

                //if (response.IsSuccessful == false || response.Data.Success != true)
                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success == false)
                {
                    Logger.Error("An error occured, while retrieving Service Bill Details");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved Service Bill details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Service Bill Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }
        public string GetConfigPath(string file)
        {
            string[] execPath = System.Reflection.Assembly.GetEntryAssembly().Location.Split('\\');
            string folderPath = string.Empty;
            for (int i = 0; i < (execPath.Length - 1); i++)
            {
                folderPath += execPath[i] + "\\";
            }
            var path = folderPath + file;//"config.json"
            return path;
        }
        public void UpdatePaymentHistoryRecords(PaymentHistory record, EIRSTaskConfigValues configValues)
        {
            Logger.Debug($"About to update payment record from App API");
            var AppBaseUrl = configValues.AppBaseUrl;
            var UpdatePaymentUrl = configValues.UpdatePaymentUrl;
            //Dictionary<string, string> config;
            //var path = GetConfigPath("config.json");
            //Logger.Info(path);
            //using (StreamReader r = new StreamReader(path))
            //{
            //    config = JsonConvert.DeserializeObject<Dictionary<string, string>>(r.ReadToEnd());
            //    AppBaseUrl = config["AppBaseUrl"];
            //    UpdatePaymentUrl = config["UpdatePaymentUrl"];
            //}
            var AppValue = Utils.ConfigValueIsNull(AppBaseUrl);
            if (AppValue)
            {
                Logger.Error("could not find endpoint base url for the App");
            }
            var Value = Utils.ConfigValueIsNull(UpdatePaymentUrl);
            if (Value)
            {
                Logger.Error("could not find endpoint url for updating payment history");
            }
            try
            {
                var client = new RestClient(AppBaseUrl);
                var request = new RestRequest(UpdatePaymentUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                var clientId = configValues.ClientId;//config["ClientId"];
                var clientsceret = configValues.ClientSecret;//config["ClientSecret"];
                request.AddHeader("ClientId", clientId);
                request.AddHeader("ClientSecret", clientsceret);
                request.AddJsonBody(record);
                Logger.Info("Make call to update payment");
                IRestResponse<APIResponse> response = client.Execute<APIResponse>(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success == false)
                {
                    Logger.Error("An error occured, while updating payments");
                    var error = $"See error details - {response.Data.ErrorMessage}";
                    Logger.Error(error);
                }
                Logger.Info("Successfully updated payments");
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to update payments");
                Logger.Error(ex.Message, ex);
            }
        }

        public List<PaymentHistory> GetUnsyncedPaymentRecords(EIRSTaskConfigValues configValues)
        {
            Logger.Debug($"About to fetch unsynced payment record from App API");
            var AppBaseUrl = configValues.AppBaseUrl;
            var UnsyncedPaymentUrl = configValues.UnsyncedPaymentUrl;
            //Dictionary<string, string> config;
            //var path = GetConfigPath("config.json");
            //Logger.Info(path);
            //using (StreamReader r = new StreamReader(path))
            //{
            //    config = JsonConvert.DeserializeObject<Dictionary<string, string>>(r.ReadToEnd());
            //    AppBaseUrl = config["AppBaseUrl"];
            //    UnsyncedPaymentUrl = config["UnsyncedPaymentUrl"];
            //}
            var AppValue = Utils.ConfigValueIsNull(AppBaseUrl);
            if (AppValue)
            {
                Logger.Error("could not find endpoint base url for the App");
                return null;
            }
            var Value = Utils.ConfigValueIsNull(UnsyncedPaymentUrl);
            if (Value)
            {
                Logger.Error("could not find endpoint url for processing unsynced payment");
                return null;
            }
            try
            {
                var client = new RestClient(AppBaseUrl);
                var request = new RestRequest(UnsyncedPaymentUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };
                var clientId = configValues.ClientId; //config["ClientId"];
                var clientsceret = configValues.ClientSecret;//config["ClientSecret"];
                request.AddHeader("ClientId", clientId);
                request.AddHeader("ClientSecret", clientsceret);
                Logger.Info("Make call to get unsynced payments");
                var response = client.Execute<List<PaymentHistory>>(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logger.Error("An error occured, while retrieving unsynced payments");
                    return null;
                }
                Logger.Info("Successfully retrieved unsynced payments");
                List<PaymentHistory> resp = JsonConvert.DeserializeObject<List<PaymentHistory>>(response.Content);
                return resp;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to retrieve unsynced payments");
                Logger.Error(ex.Message, ex);
                return null;
            }
        }
        public APIResponse MakeNETPAYPayment(EIRSPaymentRequestInfo model)
        {
            string MerchantId = Configuration.MerchantId;
            string MerchantSecret = Configuration.MerchantSecret;

            try
            {
                Logger.Info("About to make payment through netpay...");

                string NetPayUrl = Configuration.NetPayBaseUrl;
                string paymentUrl = Configuration.NetPayPaymentUrl;

                var netpayclient = new RestClient(NetPayUrl);
                var request = new RestRequest(paymentUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                var postObject = Utils.GetNetPayModel(model.PaymentIdentifier, model.Description, model.TaxPayerName, model.TotalAmount, Configuration.NetPayReturnUrl);
                request.AddHeader("MerchantId", MerchantId);
                request.AddHeader("MerchantSecret", MerchantSecret);
                request.AddBody(postObject); ;

                Logger.Info("Calling NetPay Payment Endpoint");

                var response = netpayclient.Execute(request);

                Logger.Info($"Netpayment done.... {response.Content}");

                return new APIResponse { StatusCode = response.StatusCode, ErrorMessage = response.ErrorMessage, Result = response.StatusDescription };

            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured, while calling netpay. See details {ex}");

                throw;
            }

        }


        public bool MakePaymentViaNetPay(EIRSPaymentRequestInfo model)
        {
            string MerchantId = Configuration.MerchantId;
            string MerchantSecret = Configuration.MerchantSecret;

            try
            {
                Logger.Info("About to make payment through netpay...");

                string NetPayUrl = Configuration.NetPayBaseUrl;
                string paymentUrl = Configuration.NetPayPaymentUrl;

                var netpayclient = new RestClient(NetPayUrl);
                var request = new RestRequest(paymentUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                var postObject = Utils.GetNetPayModel(model.PaymentIdentifier, model.Description, model.TaxPayerName, model.TotalAmount, Configuration.NetPayReturnUrl);
                request.AddHeader("MerchantId", MerchantId);
                request.AddHeader("MerchantSecret", MerchantSecret);
                request.AddBody(postObject); ;

                Logger.Info("Calling NetPay Payment Endpoint");

                var response = netpayclient.Execute(request);

                Logger.Info($"Netpayment done.... {response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured, while calling netpay. See details {ex}");

                throw;
            }

        }

        public bool NotifyEIRSOfSettlementPayment(EIRSSettlementInfo settlementRequest, EIRSTaskConfigValues configValues)
        {
            string EIRSBaseUrl = configValues.EIRSBaseUrl;//Configuration.EIRSBaseUrl;//"https://api.eirsautomation.xyz";
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return false;
            }
            string addSettlementUrl = configValues.EIRSAddSettlementUrl;//Configuration.EIRSAddSettlementUrl;
            var settlementUrl = Utils.ConfigValueIsNull(addSettlementUrl);
            if (settlementUrl)
            {
                Logger.Error("could not find endpoint url for notifying EIRS of settlement info");
                return false;
            }
            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken(configValues);
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return false;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(addSettlementUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddJsonBody(settlementRequest);

                Logger.Info("Make call to add settlement ");
                var response = client.Execute<EIRSAPIResponse>(request);

                //if (response.Data.Success != true)
                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured while adding settlement details");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error + " The full response content: " + response.Content);
                    return false;
                }

                Logger.Info($"Response for EIRS is {response.Content.ToString()} for transaction Ref {settlementRequest.TransactionRefNo} with reference Number {settlementRequest.ReferenceNumber}");

                Logger.Info($"Successfully added settlement details for transaction refno - {settlementRequest.TransactionRefNo} ");
                Logger.Debug(response.Data.Message + " The full response content: " + response.Content);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not add settlement Details");
                Logger.Error(ex.StackTrace, ex);
                return false;
            }
        }
        public bool NotifyEIRSOfSettlementPayment(EIRSSettlementDetails settlementRequest, EIRSTaskConfigValues configValues, ILog log)
        {
            string EIRSBaseUrl = configValues.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                log.Error("could not find endpoint url for EIRS base url");
                return false;
            }
            string addSettlementUrl = configValues.EIRSAddSettlementUrl;
            var settlementUrl = Utils.ConfigValueIsNull(addSettlementUrl);
            if (settlementUrl)
            {
                log.Error("could not find endpoint url for notifying EIRS of settlement info");
                return false;
            }
            try
            {
                log.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken(configValues);
                if (tokenResult == null)
                {
                    log.Error("An error occured, could not get token for api call");
                    return false;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(addSettlementUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddJsonBody(settlementRequest);

                log.Info("Make call to add settlement ");
                var response = client.Execute<EIRSAPIResponse>(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    log.Error("An error occured while adding settlement details");
                    var error = $"See error details - {response.Data.Message}";
                    log.Error(error + " The full response content: " + response.Content);
                    return false;
                }

                log.Info($"Response for EIRS is {response.Content.ToString()}");

                log.Info($"Successfully added settlement details");
                log.Debug(response.Data.Message + " The full response content: " + response.Content);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("An error occured, could not add settlement Details");
                log.Error(ex.StackTrace, ex);
                return false;
            }
        }
        //public bool NotifyEIRSOfSettlementPayment(EIRSSettlementInfo settlementRequest, EIRSTaskConfigValues configValues)
        //{
        //    string EIRSBaseUrl = Configuration.EIRSBaseUrl;//configValues.EIRSBaseUrl;
        //    var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
        //    if (EIRSVal)
        //    {
        //        Logger.Error("could not find endpoint url for EIRS base url");
        //        return false;
        //    }
        //    string addSettlementUrl = Configuration.EIRSAddSettlementUrl;//configValues.EIRSAddSettlementUrl;
        //    var settlementUrl = Utils.ConfigValueIsNull(addSettlementUrl);
        //    if (settlementUrl)
        //    {
        //        Logger.Error("could not find endpoint url for notifying EIRS of settlement info");
        //        return false;
        //    }
        //    try
        //    {
        //        Logger.Debug("Get Token for this call");
        //        var tokenResult = GetEIRSAPIToken();//GetEIRSAPIToken(configValues);
        //        if (tokenResult == null)
        //        {
        //            Logger.Error("An error occured, could not get token for api call");
        //            return false;
        //        }
        //        var token = tokenResult.access_token;
        //        var tokenType = tokenResult.token_type;
        //        var header = $"{tokenType} {token}";

        //        var client = new RestClient(EIRSBaseUrl);
        //        var request = new RestRequest(addSettlementUrl, Method.POST)
        //        {
        //            RequestFormat = DataFormat.Json
        //        };

        //        request.AddHeader("Authorization", header);
        //        request.AddJsonBody(settlementRequest);

        //        Logger.Info("Make call to add settlement ");
        //        var response = client.Execute<EIRSAPIResponse>(request);

        //        //if (response.Data.Success != true)
        //        if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
        //        {
        //            Logger.Error("An error occured while adding settlement details");
        //            var error = $"See error details - {response.Data.Message}";
        //            Logger.Error(error);
        //            return false;
        //        }

        //        Logger.Info($"Successfully added settlement details for transaction refno - {settlementRequest.TransactionRefNo} ");
        //        Logger.Debug(response.Data.Message);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("An error occured, could not add settlement Details");
        //        Logger.Error(ex.StackTrace, ex);
        //        return false;
        //    }
        //}

        protected EIRSAPILoginResponse GetEIRSAPIToken(EIRSTaskConfigValues configValues)
        {
            string EIRSBaseUrl = configValues.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string loginUrl = configValues.LoginUrl;
            var loginVal = Utils.ConfigValueIsNull(loginUrl);
            if (loginVal)
            {
                Logger.Error("could not find endpoint url for accessing login token");
                return null;
            }

            try
            {
                var contentType = configValues.EIRSAPILoginContentType.ToString();
                var acceptValue = configValues.EIRSAPILoginAccept.ToString();

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(loginUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddHeader("Content-Type", contentType);
                request.AddHeader("Accept", acceptValue);

                var obj = new EIRSAPILoginRequest
                {
                    grant_type = configValues.EIRSAPILoginGrantType,
                    Password = configValues.EIRSAPILoginPassword,
                    UserName = configValues.EIRSAPILoginUsername
                };

                Logger.Info("Make call to get Login Token Details");
                //request.AddParameter(contentType, obj, contentType, ParameterType.RequestBody);
                request.AddParameter(contentType, $"Username={obj.UserName}&Password={obj.Password}&grant_type={obj.grant_type}", ParameterType.RequestBody);
                var response = client.Execute<EIRSAPILoginResponse>(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logger.Error("An error occured, could not retrieve Token Details");
                    var error = $"An error with status code - {response.StatusCode} occurred when trying to retrieve Token details;" +
                        $"the error message is {response.ErrorMessage}";
                    Logger.Error(error);
                    return null;
                }
                var res = response.Data;
                Logger.Info("Successfully retrieved Token details");
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Token Details");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        public EIRSAPIResponse GetAssessmentRuleItems(long assessmentID)
        {
            Logger.Debug($"About to fetch assessment rule items using assessmentID {assessmentID} ");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string assessmentRuleItemsUrl = Configuration.GetAssessmentRuleItemsUrl;
            var assessmentVal = Utils.ConfigValueIsNull(assessmentRuleItemsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for getting assessment rule items Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(assessmentRuleItemsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("id", assessmentID, ParameterType.UrlSegment);

                Logger.Info("Make call to get Assessment Rule Items ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving assessment rule items");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved Assessment Rule Items");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Assessment Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public EIRSAPIResponse GetServiceBillItems(long serviceBillID)
        {
            Logger.Debug($"About to fetch service bill items using service bill ID-{serviceBillID} ");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string serviceBillItemsUrl = Configuration.GetServiceBillItemsUrl;
            var assessmentVal = Utils.ConfigValueIsNull(serviceBillItemsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for getting service bill items Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(serviceBillItemsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("id", serviceBillID, ParameterType.UrlSegment);

                Logger.Info("Make call to get Service Bill Items ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success == false)
                {
                    Logger.Error("An error occured, while retrieving service bill items");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved service bill Items");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Service Bill Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public EIRSAPIResponse GetTaxPayerByRINAndMobile(string rin, string mobileNumber)
        {
            Logger.Debug($"About to fetch tax payer details using RIN {rin}  and mobile number {mobileNumber}");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string searchTaxPayerUrl = Configuration.POASearchTaxPayerUrl;
            var searchTaxPayerVal = Utils.ConfigValueIsNull(searchTaxPayerUrl);
            if (searchTaxPayerVal)
            {
                Logger.Error("could not find endpoint url for getting POA tax payer Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(searchTaxPayerUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("TaxPayerRIN", rin, ParameterType.QueryString);
                request.AddParameter("MobileNumber", mobileNumber, ParameterType.QueryString);

                Logger.Info("Make call to get POA tax payer  details ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving assessment rule items");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved POA tax payer details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve POA tax payer details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public bool NotifyEIRSOfPOASettlement(PayOnAccountSettlement payOnSettlement, EIRSTaskConfigValues configValues)
        {
            string EIRSBaseUrl = configValues.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return false;
            }
            string addSettlementUrl = configValues.EIRSInsertPayOnAccountUrl;
            var settlementUrl = Utils.ConfigValueIsNull(addSettlementUrl);
            if (settlementUrl)
            {
                Logger.Error("could not find endpoint url for notifying EIRS of Pay on Account settlement info");
                return false;
            }
            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken(configValues);
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return false;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(addSettlementUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddJsonBody(payOnSettlement);

                Logger.Info("Make call to add settlement ");
                var response = client.Execute<EIRSAPIResponse>(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured while adding settlement details");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return false;
                }

                Logger.Info("Successfully added Pay On Account settlement details");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not add Pay On Account settlement Details");
                Logger.Error(ex.StackTrace, ex);
                return false;
            }
        }

        public EIRSAPIResponse GetAssessmentDetailsByRefNumber(string referenceNumber, EIRSTaskConfigValues configValues)
        {
            Logger.Debug($"About to fetch assessment details by ref number -  {referenceNumber} for EIRS Task Settlement");
            string EIRSBaseUrl = configValues.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string assessmentDetailsUrl = configValues.GetAssessmentDetailUrl;
            var assessmentVal = Utils.ConfigValueIsNull(assessmentDetailsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for accessing assessment Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken(configValues);
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(assessmentDetailsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("refno", referenceNumber, ParameterType.UrlSegment);
                //request.AddParameter("mobilenumber", phoneNumber, ParameterType.UrlSegment);

                Logger.Info("Make call to get Assessment Details ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success == false)
                {
                    Logger.Error("An error occured, while retrieving assessment Details");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved Assessment details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Assessment Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public EIRSAPIResponse GetServiceBillDetailsByRefNumber(string referenceNumber, EIRSTaskConfigValues configValues)
        {
            Logger.Debug($"About to service bill details by ref number {referenceNumber} for EIRS Task Settlement");
            string EIRSBaseUrl = configValues.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string serviceBillDetailsUrl = configValues.GetServiceBillDetailsUrl;
            var assessmentVal = Utils.ConfigValueIsNull(serviceBillDetailsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for accessing service bill Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken(configValues);
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(serviceBillDetailsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("refno", referenceNumber, ParameterType.UrlSegment);

                Logger.Info("Make call to get Service Bill Details ");
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving Service Bill Details");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved Service Bill details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Service Bill Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get List of Assessment Rule Items using the assessment ID
        /// </summary>
        /// <param name="assessmentID"></param>
        /// <param name="configValues"></param>
        /// <returns></returns>
        public EIRSAPIResponse GetAssessmentRuleItems(long assessmentID, EIRSTaskConfigValues configValues)
        {
            Logger.Debug($"About toget assessment rule items using assessment ID -  {assessmentID} for EIRS Task Settlem");
            string EIRSBaseUrl = configValues.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string assessmentRuleItemsUrl = configValues.GetAssessmentRuleItemsUrl;
            var assessmentVal = Utils.ConfigValueIsNull(assessmentRuleItemsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for getting assessment rule items Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken(configValues);
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(assessmentRuleItemsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("id", assessmentID, ParameterType.UrlSegment);

                Logger.Info("Make call to get Assessment Rule Items ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving assessment rule items");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved Assessment Rule Items");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Assessment Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public EIRSAPIResponse GetServiceBillItems(long serviceBillID, EIRSTaskConfigValues configValues)
        {
            Logger.Debug($"About to fetch service bill items using service bill ID {serviceBillID} for EIRS Task Settlement");
            string EIRSBaseUrl = configValues.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string serviceBillItemsUrl = configValues.GetServiceBillItemsUrl;
            var assessmentVal = Utils.ConfigValueIsNull(serviceBillItemsUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for getting service bill items Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken(configValues);
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(serviceBillItemsUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("id", serviceBillID, ParameterType.UrlSegment);

                Logger.Info("Make call to get Service Bill Items ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success == false)
                {
                    Logger.Error("An error occured, while retrieving service bill items");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved service bill Items");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Service Bill Details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get TaxPayer Data from the API using the business name
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        public EIRSAPIResponse GetTaxPayerByBusinessName(string business)
        {
            Logger.Debug($"About to fetch tax payer details by business name using  {business} ");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string searchTaxPayerUrl = Configuration.SearchByBusinessNameUrl;
            var searchTaxPayerVal = Utils.ConfigValueIsNull(searchTaxPayerUrl);
            if (searchTaxPayerVal)
            {
                Logger.Error("could not find endpoint url for getting tax payer Details using the business name");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(searchTaxPayerUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("BusinessName", business, ParameterType.QueryString);

                Logger.Info("Make call to get POA tax payer  details using the business name");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving taxpayer details by business name");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved POA tax payer details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve POA tax payer details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get TaxPayer Data from the API using the mobile number
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public EIRSAPIResponse GetTaxPayerByMobileNumber(string mobile)
        {
            Logger.Debug($"About to fetch tax payer details using and mobile number {mobile}");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string searchTaxPayerUrl = Configuration.SearchByPhoneUrl;
            var searchTaxPayerVal = Utils.ConfigValueIsNull(searchTaxPayerUrl);
            if (searchTaxPayerVal)
            {
                Logger.Error("could not find endpoint url for getting POA tax payer Details using Mobile Number");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(searchTaxPayerUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("MobileNumber", mobile, ParameterType.QueryString);

                Logger.Info("Make call to get POA tax payer details using the mobile number");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving tax payer details");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved POA tax payer details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve POA tax payer details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get TaxPayer Data from the API using the RIN
        /// </summary>
        /// <param name="rin"></param>
        /// <returns></returns>
        public EIRSAPIResponse GetTaxPayerByRIN(string rin)
        {
            Logger.Debug($"About to fetch tax payer details using RIN {rin}");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string searchTaxPayerUrl = Configuration.SearchByRINUrl;
            var searchTaxPayerVal = Utils.ConfigValueIsNull(searchTaxPayerUrl);
            if (searchTaxPayerVal)
            {
                Logger.Error("could not find endpoint url for getting POA tax payer Details using the RIN");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(searchTaxPayerUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("TaxPayerRIN", rin, ParameterType.QueryString);

                Logger.Info("Make call to get POA tax payer  details ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving tax payer details using the RIN");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved POA tax payer details");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve POA tax payer details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }
        

        /// <summary>
        /// Fetches the list of Revenue sub stream from EIRS api
        /// </summary>
        /// <returns></returns>
        public EIRSAPIResponse GetRevenueSubStreamList()
        {
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string revenueSubStreamUrl = Configuration.RevenueSubStreamListUrl;
            var revenueSubStreamVal = Utils.ConfigValueIsNull(revenueSubStreamUrl);
            if (revenueSubStreamVal)
            {
                Logger.Error("could not find endpoint url for getting POA tax payer Details using Mobile Number");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(revenueSubStreamUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);

                Logger.Info("Make call to get revenue substream list");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving revenue sub stream list");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved revenue sub stream list");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve POA tax payer details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the list of Revenue stream from EIRS api
        /// </summary>
        /// <returns></returns>
        public EIRSAPIResponse GetRevenueStreamList()
        {
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string revenueStreamUrl = Configuration.RevenueStreamListUrl;
            var revenueStreamVal = Utils.ConfigValueIsNull(revenueStreamUrl);
            if (revenueStreamVal)
            {
                Logger.Error("could not find endpoint url for getting revenue stream list");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(revenueStreamUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);

                Logger.Info("Make call to get revenue stream list");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving revenue  stream list");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved revenue  stream list");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve POA tax payer details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the list of Tax Payers Type from EIRS api
        /// </summary>
        /// <returns></returns>
        public EIRSAPIResponse GetTaxPayerTypeList()
        {
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string taxPayerTypeUrl = Configuration.TaxPayerTypeListUrl;
            var taxPayerTypeVal = Utils.ConfigValueIsNull(taxPayerTypeUrl);
            if (taxPayerTypeVal)
            {
                Logger.Error("could not find endpoint url for getting tax payer type ");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(taxPayerTypeUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);

                Logger.Info("Make call to get tax payer type list");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving tax payer type list");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved tax payers type list");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve tax payers type details");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches economic activities from EIRS api based on the TaxPayerTypeID
        /// </summary>
        /// <returns></returns>
        public EIRSAPIResponse GetEconomicActivitiesList(int taxPayerTypeID)
        {
            Logger.Debug($"About to fetch economic activities list using taxpayertypeID - {taxPayerTypeID}");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string taxPayerTypeUrl = Configuration.EconomicActivitiesListUrl;
            var taxPayerTypeVal = Utils.ConfigValueIsNull(taxPayerTypeUrl);
            if (taxPayerTypeVal)
            {
                Logger.Error("could not find endpoint url for getting economic activities for this tax payer  ");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(taxPayerTypeUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("TaxPayerTypeID", taxPayerTypeID, ParameterType.QueryString);

                Logger.Info("Make call to get economic activities for this tax payer type");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving economic activities for this tax payer type");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved economic activities for this tax payer type");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve economic activities for this tax payer type");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get list of assessement rules using the assessment Id
        /// </summary>
        /// <param name="assessmentID"></param>
        /// <returns></returns>
        public EIRSAPIResponse GetAssessmentRules(long assessmentID)
        {
            Logger.Debug($"About to fetchassessment rules using assessmentId {assessmentID}");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string assessmentRulesUrl = Configuration.GetAssessmentRulesUrl;
            var assessmentVal = Utils.ConfigValueIsNull(assessmentRulesUrl);
            if (assessmentVal)
            {
                Logger.Error("could not find endpoint url for getting assessment rules Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(assessmentRulesUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("id", assessmentID, ParameterType.UrlSegment);

                Logger.Info("Make call to get Assessment Rules ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success != true)
                {
                    Logger.Error("An error occured, while retrieving assessment rules");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved Assessment Rules");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve Assessment Rules");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get list of service bill rules using the serviceBill ID 
        /// </summary>
        /// <param name="serviceBillID"></param>
        /// <returns></returns>
        public EIRSAPIResponse GetServiceBillRules(long serviceBillID)
        {
            Logger.Debug($"About to fetch service bill rules for service bill ID {serviceBillID}");
            string EIRSBaseUrl = Configuration.EIRSBaseUrl;
            var EIRSVal = Utils.ConfigValueIsNull(EIRSBaseUrl);
            if (EIRSVal)
            {
                Logger.Error("could not find endpoint url for EIRS base url");
                return null;
            }
            string serviceBillRulesUrl = Configuration.GetServiceBillRulesUrl;
            var serviceBillRuleVal = Utils.ConfigValueIsNull(serviceBillRulesUrl);
            if (serviceBillRuleVal)
            {
                Logger.Error("could not find endpoint url for getting service bill rules Details");
                return null;
            }

            try
            {
                Logger.Debug("Get Token for this call");
                var tokenResult = GetEIRSAPIToken();
                if (tokenResult == null)
                {
                    Logger.Error("An error occured, could not get token for api call");
                    return null;
                }
                var token = tokenResult.access_token;
                var tokenType = tokenResult.token_type;
                var header = $"{tokenType} {token}";

                var client = new RestClient(EIRSBaseUrl);
                var request = new RestRequest(serviceBillRulesUrl, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Authorization", header);
                request.AddParameter("id", serviceBillID, ParameterType.UrlSegment);

                Logger.Info("Make call to get Service Bill Rules ");
                //var response = client.Execute(request);
                IRestResponse<EIRSAPIResponse> response = client.Execute<EIRSAPIResponse>(request);


                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data.Success == false)
                {
                    Logger.Error("An error occured, while retrieving service bill rules");
                    var error = $"See error details - {response.Data.Message}";
                    Logger.Error(error);
                    return new EIRSAPIResponse { Message = response.Data.Message, Result = response.Data.Result, Success = response.Data.Success };
                }

                Logger.Info("Successfully retrieved MDA service bill rules");
                return response.Data;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve MDA Service Bill Rules");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// get quickteller payment transaction details
        /// </summary>
        /// <param name="transRef"></param>
        /// <returns><see cref="QuicktellerAPIResponse"/></returns>
        public List<KeyValuePair<string,object>> GetQuickTellerPaymentTransactionDetails(string transRef, string requestReference)//QuicktellerAPIResponse
        {
            Logger.Debug($"About to fetch quickteller payment transaction details using transRef - {transRef}");
            string QTBaseUrl = Configuration.GetQuicktellerBaseRESTURL;
            var QTVal = Utils.ConfigValueIsNull(QTBaseUrl);
            if (QTVal)
            {
                Logger.Error("could not find endpoint url quickteller base url");
                return null;
            }

            string QTRequestUrl = Configuration.GetQuicktellerRESTRequestURL;
            var QTRequestVal = Utils.ConfigValueIsNull(QTRequestUrl);
            if (QTRequestVal)
            {
                Logger.Error("could not find endpoint url for getting quickteller payment transaction details for this trans ref");
                return null;
            }

            try
            {
                
                var client = new RestClient(QTBaseUrl);
                var request = new RestRequest(QTRequestUrl, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                var clientId = Configuration.GetQuicktellerClientId ?? throw new Exception("could not retrieve QT client Id");
                //var clientId = "https://pinscher.eirsautomation.xyz/rsps/cbs-parkway/cbspay/";
                var secretKey = Configuration.GetQuicktellerSecretKey ?? throw new Exception("could not retrieve QT secret key");

                var hash = Utils.GenerateSHA512String(requestReference + secretKey);

                request.AddHeader("clientid", clientId);
                request.AddHeader("Hash", hash);
                request.AddParameter("requestReference", requestReference, ParameterType.UrlSegment);

                Logger.Info("Make call to get quickteller payment transaction details");
                //var response = client.Execute(request);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                IRestResponse<List<KeyValuePair<string, object>>> response = client.Execute<List<KeyValuePair<string, object>>>(request);
                //IRestResponse<QuicktellerAPIResponse> response = client.Execute<QuicktellerAPIResponse>(request);
                //var response = client.Execute(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logger.Error("An error occured, while retrieving quickteller payment transaction details");
                    var error = $"See error details - {response.ErrorMessage}";
                    Logger.Error(error);
                    return null;
                }

                Logger.Info("Successfully retrieved quickteller payment transaction details for this transaction ref");
                //List<KeyValuePair<string,object>> resp = (List<KeyValuePair<string, object>>)JsonConvert.DeserializeObject(response.Content);
                //var resp = JsonConvert.DeserializeObject<QuicktellerAPIResponse>(response.Content);
                //var resp = (QuicktellerAPIResponse)response;
                return response.Data;
                //return resp.d
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured, could not retrieve quickteller payment transaction details for this trans ref");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        
    }
}
