using CBSPay.Core.APIModels;
using CBSPay.Core.Helpers;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Services;
using CBSPay.Core.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CBSPay.API.Controllers
{
    //[XMLControllerConfig]
    public class PayDirectIntegrationController : ApiController
    {
        private readonly ITaxPayerService _taxPayerService;
        private readonly IPaymentService _paymentService;
        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }
        public PayDirectIntegrationController()
        {
            _taxPayerService = new TaxPayerService();
            _paymentService = new PaymentService();
        }
        /// <summary>
        /// Endpoint for all paydirect's customer validation and payment notification
        /// </summary>
        /// <param name="payDirectRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PayDirectReferenceRequest")]
        public IHttpActionResult PayDirectRequest()
        {
            //variables
            IHttpActionResult response;
            string methodName = "";
            string txt;
            CustomerInformationResponse pdTaxpayerInfo;
            PaymentNotificationResponse pdPaymentInfo;

            //validate the calling IP
            var clientIP = GetClientIpAddress();
            if (clientIP == null)
            {
                Logger.Error($"Could not get PayDirectIP from the HttpContext");
                response = Content(HttpStatusCode.Forbidden, "");
                return response;
            }
            //var requestUrI = Request.RequestUri.ToString();
            var IsValidCall = Utils.IsValidPayDirectCallingIP(clientIP);
            if (!IsValidCall)
            {
                Logger.Error($"Invalid Configured PayDirectIP {clientIP} or PayDirect Ip has not beeen added to the web config ");
                response = Content(HttpStatusCode.Forbidden, "");
                return response;
            }
            try
            {
                //get the request via content stream     
                Logger.Debug("Getting PayDirect Request via content stream");
                using (var stream = Request.Content.ReadAsStreamAsync().Result)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        txt = sr.ReadToEnd();
                    }
                }

                //determine wha the call is for
                Logger.Debug("Determine what call it is ");
                if (txt.Contains("CustomerInformationRequest"))
                {
                    Logger.Debug("it is a CustomerInformationRequest call");
                    methodName = "CUSTOMER VALIDATION";
                }
                if (txt.Contains("PaymentNotificationRequest"))
                {
                    Logger.Debug("it is a PaymentNotificationRequest call");
                    methodName = "PAYMENT NOTIFICATION";
                }
               

                //do necessary validation 
                //(check if service url is valid and the same with what is in the web config)
                Logger.Debug("Validating the service url");
                var result = _taxPayerService.DoPayDirectReferenceBasicValidation(txt, methodName);
                if (string.IsNullOrWhiteSpace(result))
                {
                    var customerDetails = Utils.DeserializeXML<CustomerInformationRequest>(txt);
                    if (customerDetails != null)
                    {
                        Logger.Error("Service Url not stated or not configured in the web config or cannot deserialize the xml string to a concrete object");
                        pdTaxpayerInfo = new CustomerInformationResponse
                        {
                            MerchantReference = customerDetails.MerchantReference,
                            Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = customerDetails.CustReference,
                                        Status = 1,
                                        StatusMessage = "An error occurred, check that the right service url is configured and the XML is well structured",
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = customerDetails.ThirdPartyCode
                                    }
                                }
                        };
                        response = Content(HttpStatusCode.OK, pdTaxpayerInfo);
                        return response;
                    }
                }

                //if call is valid, determine whether it is for customer validation or payment notification
                switch (methodName)
                {
                    //if customer validation
                    case "CUSTOMER VALIDATION":
                        Logger.Debug("Call for CUSTOMER VALIDATION");
                        var customerDetails = Utils.DeserializeXML<CustomerInformationRequest>(txt);
                        if (customerDetails == null)
                        {
                            Logger.Error($"An error occurred, could not deserialize XML string to CustomerInformationRequest object");
                            pdTaxpayerInfo = new CustomerInformationResponse
                            {
                                MerchantReference = customerDetails.MerchantReference,
                                Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = customerDetails.CustReference,
                                        Status = 1,
                                        StatusMessage = "An error occured, please try again",
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = customerDetails.ThirdPartyCode
                                    }
                                }
                            };
                            response = Content(HttpStatusCode.OK, pdTaxpayerInfo);
                            return response;
                        }
                        //retrieve the taxpayer information
                        var payDirectTaxPayerInfo = _taxPayerService.RetrievePayDirectTaxPayerInfo(customerDetails.MerchantReference, customerDetails.CustReference, customerDetails.ThirdPartyCode);
                        pdTaxpayerInfo = payDirectTaxPayerInfo.Result;
                        if (payDirectTaxPayerInfo.Success == false)
                        {
                            pdTaxpayerInfo = new CustomerInformationResponse
                            {
                                MerchantReference = customerDetails.MerchantReference,
                                Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = customerDetails.CustReference,
                                        Status = 1,
                                        StatusMessage = "An error occured, please try again",
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = customerDetails.ThirdPartyCode
                                    }
                                }
                            };
                            response = Content(HttpStatusCode.OK, pdTaxpayerInfo);

                            return response;
                        }

                        return Ok(pdTaxpayerInfo);                        

                    //if payment notification
                    case "PAYMENT NOTIFICATION":
                        Logger.Debug("Call for PAYMENT NOTIFICATION");
                        var paymentNotification = Utils.DeserializeXML<PaymentNotificationRequest>(txt);
                        if (paymentNotification == null)
                        {
                            Logger.Error($"An error occurred, could not deserialize XML string to PaymentNotificationRequest object");
                            pdPaymentInfo = new PaymentNotificationResponse
                            {
                                Payments = new List<PaymentResponse>
                                {
                                     new PaymentResponse
                                     {
                                         Status = 1,
                                         PaymentLogId = paymentNotification.Payments.FirstOrDefault().PaymentLogId,
                                         //StatusMessage = "An error occurred, check that the right service url is configured and the XML is well structured"
                                     }
                                }
                            };
                            response = Content(HttpStatusCode.OK, pdPaymentInfo);
                            return response;
                        }
                        var payDirectResponse = _paymentService.SynchronizePayDirectPayments(paymentNotification.Payments);
                        pdPaymentInfo = payDirectResponse.Result;
                        if (payDirectResponse.Success == false)
                        {
                            pdPaymentInfo = new PaymentNotificationResponse
                            {
                                Payments = new List<PaymentResponse>
                                {
                                     new PaymentResponse
                                     {
                                         Status = 1,
                                         PaymentLogId = paymentNotification.Payments.FirstOrDefault().PaymentLogId,
                                         //StatusMessage = "An error occurred, please try again"
                                     }
                                }
                            };
                            response = Content(HttpStatusCode.OK, pdPaymentInfo);
                            return response;
                        }
                        response = Content(HttpStatusCode.OK, pdPaymentInfo);
                        return response;

                    default:
                        Logger.Error("Cannot determine whether it is a Customer Validation Call or Payment Notification Call ");
                        response = Content(HttpStatusCode.BadRequest, "");
                        return response;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process pay direct call");
                Logger.Error(ex.StackTrace, ex);
                response = Content(HttpStatusCode.InternalServerError, "");
                return response;
            }

        }
        /// <summary>
        /// Endpoint for Paay On Account paydirect's customer validation and payment notification
        /// </summary>
        /// <param name="payDirectRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PayDirectPOARequest")]
        public IHttpActionResult PayDirectPOARequest()
        {
            //variables
            IHttpActionResult response;
            string methodName = "";
            string txt;
            CustomerInformationResponse pdTaxpayerInfo;
            PaymentNotificationResponse pdPaymentInfo;

            //validate the calling IP
            var clientIP = GetClientIpAddress();
            if (clientIP == null)
            {
                Logger.Error($"Could not get PayDirectIP from the HttpContext");
                response = Content(HttpStatusCode.Forbidden, "");
                return response;
            }
            var IsValidCall = Utils.IsValidPayDirectCallingIP(clientIP);
            if (!IsValidCall)
            {
                Logger.Error($"Invalid Configured PayDirectIP or PayDirect Ip has not beeen added to the web config ");
                response = Content(HttpStatusCode.Forbidden, "");
                return response;
            }

            try
            {

                //get the request via content stream     
                Logger.Debug("Getting PayDirect Request via content stream");
                using (var stream = Request.Content.ReadAsStreamAsync().Result)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        txt = sr.ReadToEnd();
                    }
                }

                //determine wha the call is for
                Logger.Debug("Determine what call it is ");
                if (txt.Contains("CustomerInformationRequest"))
                {
                    methodName = "CUSTOMER VALIDATION";
                }
                else if (txt.Contains("PaymentNotificationRequest"))
                {
                    methodName = "PAYMENT NOTIFICATION";
                }
                else
                {
                    Logger.Error("Call type cannot be deduced, whether it isCustomerInformationRequest or  PaymentNotificationRequest");
                    response = Content(HttpStatusCode.BadRequest, "An error occurred, Call type cannot be deduced; check that the XML is well structured");
                    return response;
                }

                //do necessary validation 
                //(check if service url is valid and the same with what is in the web config)
                Logger.Debug("Validating the service url");
                var result = _taxPayerService.DoPayDirectPOABasicValidation(txt, methodName);
                if (string.IsNullOrWhiteSpace(result))
                {
                    var customerDetails = Utils.DeserializeXML<CustomerInformationRequest>(txt);
                    if (customerDetails != null)
                    {
                        Logger.Error("Service Url not stated or not configured in the web config or cannot deserialize the xml string to a concrete object");
                        pdTaxpayerInfo = new CustomerInformationResponse
                        {
                            MerchantReference = customerDetails.MerchantReference,
                            Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = customerDetails.CustReference,
                                        Status = 1,
                                        StatusMessage = "An error occurred, check that the right service url is configured and the XML is well structured",
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = customerDetails.ThirdPartyCode
                                    }
                                }
                        };
                        response = Content(HttpStatusCode.OK, pdTaxpayerInfo);
                        return response;
                    }
                }

                //if call is valid, determine whether it is for customer validation or payment notification
                switch (methodName)
                {
                    //if customer validation
                    case "CUSTOMER VALIDATION":
                        Logger.Debug("Call for CUSTOMER VALIDATION");
                        var customerDetails = Utils.DeserializeXML<CustomerInformationRequest>(txt);
                        if (customerDetails == null)
                        {
                            Logger.Error($"An error occurred, could not deserialize XML string to CustomerInformationRequest object");
                            pdTaxpayerInfo = new CustomerInformationResponse
                            {
                                MerchantReference = customerDetails.MerchantReference,
                                Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = customerDetails.CustReference,
                                        Status = 1,
                                        StatusMessage = "An error occured, please try again",
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = customerDetails.ThirdPartyCode
                                    }
                                }
                            };
                            response = Content(HttpStatusCode.OK, pdTaxpayerInfo);
                            return response;
                        }
                        if (string.IsNullOrWhiteSpace(customerDetails.CustReference))
                        {
                            pdTaxpayerInfo = new CustomerInformationResponse
                            {
                                MerchantReference = customerDetails.MerchantReference,
                                Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = customerDetails.CustReference,
                                        Status = 0,
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = customerDetails.ThirdPartyCode
                                    }
                                }
                            };
                            response = Content(HttpStatusCode.OK, pdTaxpayerInfo);
                            return response;
                        }
                        //retrieve the taxpayer information
                        var payDirectTaxPayerInfo = _taxPayerService.RetrievePayDirectPOATaxPayerInfo(customerDetails.MerchantReference, customerDetails.CustReference, customerDetails.ThirdPartyCode);
                        pdTaxpayerInfo = payDirectTaxPayerInfo.Result;
                        if (payDirectTaxPayerInfo.Success == false)
                        {

                            response = Content(HttpStatusCode.OK, pdTaxpayerInfo);
                            return response;
                        }
                        response = Content(HttpStatusCode.OK, pdTaxpayerInfo);
                        return response;

                    //if payment notification
                    case "PAYMENT NOTIFICATION":
                        Logger.Debug("Call for PAYMENT NOTIFICATION");
                        var paymentNotification = Utils.DeserializeXML<PaymentNotificationRequest>(txt);
                        if (paymentNotification == null)
                        {
                            Logger.Error($"An error occurred, could not deserialize XML string to PaymentNotificationRequest object");
                            pdPaymentInfo = new PaymentNotificationResponse
                            {
                                Payments = new List<PaymentResponse>
                                {
                                     new PaymentResponse
                                     {
                                         Status = 1,
                                         PaymentLogId = paymentNotification.Payments.FirstOrDefault().PaymentLogId,
                                         //StatusMessage = "An error occurred, check that the right service url is configured and the XML is well structured"
                                     }
                                }
                            };
                            response = Content(HttpStatusCode.OK, pdPaymentInfo);
                            return response;
                        }
                        APIResponse payDirectResponse = _paymentService.SynchronizePayDirectPOAPayments(paymentNotification.Payments);
                        pdPaymentInfo = payDirectResponse.Result;
                        if (payDirectResponse.Success == false)
                        {
                            //pdPaymentInfo = new PaymentNotificationResponse
                            //{
                            //    Payments = new List<PaymentResponse>
                            //    {
                            //         new PaymentResponse
                            //         {
                            //             Status = 1,
                            //             PaymentLogId = paymentNotification.Payments.FirstOrDefault().PaymentLogId,
                            //             //StatusMessage = "An error occurred, please try agin"
                            //         }
                            //    }
                            //};
                            response = Content(HttpStatusCode.OK, pdPaymentInfo);
                            return response;
                        }

                        response = Content(HttpStatusCode.OK, pdPaymentInfo);
                        return response;

                    default:
                        Logger.Error("Cannot determine whether it is a Customer Validation Call or Payment Notification Call ");
                        response = Content(HttpStatusCode.BadRequest, "");
                        return response;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process pay direct call");
                Logger.Error(ex.StackTrace, ex);
                response = Content(HttpStatusCode.InternalServerError, "");
                return response;
            }

        }
        private string GetClientIpAddress()
        {
            var request = Request;
            string HttpContext = "MS_HttpContext";
            string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            //var hostname = HttpContext.Current.Request.UserHostAddress;
            //IPAddress ipAddress = IPAddress.Parse(hostname);
            //IPHostEntry ipHostEntry = Dns.GetHostEntry(ipAddress);

            return null;
        }
    }
    /// <summary>
    /// serialize to XML
    /// </summary>
    internal class XMLControllerConfigAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            var config = GlobalConfiguration.Configuration;
            //var xmlFormatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            //xmlFormatter.UseXmlSerializer = true;
            //controllerSettings.Formatters.Clear();
            //controllerSettings.Formatters.Add(xmlFormatter);

            //config.Formatters.Clear();
            //config.Formatters.Add(new CustomNamespaceXmlFormatter { UseXmlSerializer = true });

        }
    }
}
