using CBSPay.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBSPay.Core.Models;
using log4net;
using System.Net;
using CBSPay.Core.Helpers;
using CBSPay.Core.APIModels;
using CBSPay.Core.Interfaces;
using Newtonsoft.Json;
using CBSPay.Core.Entities;
using NHibernate;
using System.Globalization;
using NHibernate.Criterion;

namespace CBSPay.Core.Services
{
    public class TaxPayerService : ITaxPayerService
    {
        private readonly IBaseRepository<AssessmentDetailsResult> _assessmentRepo;
        private readonly IBaseRepository<ServiceBillResult> _serviceBillRepo;
        private readonly IBaseRepository<AssessmentRuleItem> _assessmentRuleItemRepo;
        private readonly IBaseRepository<ServiceBillItem> _serviceBillItemRepo;
        private readonly IBaseRepository<AssessmentRule> _assessmentRuleRepo;
        private readonly IBaseRepository<MDAService> _mdaServiceRepo;
        private readonly IBaseRepository<TaxPayerDetails> _taxPayerRepo;
        private readonly IBaseRepository<TaxPayerType> _taxPayerTypeRepo;
        private readonly IRestService _restService;
        public TaxPayerService()
        {
            _assessmentRepo = new Repository<AssessmentDetailsResult>();
            _serviceBillRepo = new Repository<ServiceBillResult>();
            _assessmentRuleItemRepo = new Repository<AssessmentRuleItem>();
            _serviceBillItemRepo = new Repository<ServiceBillItem>();
            _assessmentRuleRepo = new Repository<AssessmentRule>();
            _mdaServiceRepo = new Repository<MDAService>();
            _taxPayerRepo = new Repository<TaxPayerDetails>();
            _taxPayerTypeRepo = new Repository<TaxPayerType>();
            _restService = new RestService();
        }
        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }
        public APIResponse DoBasicValidation(string refNumber)
        {

            var result = new APIResponse();
            if (refNumber == null)
            {
                Logger.Debug("Validating ref number");
                result = new APIResponse { Success = false, ErrorMessage = "The Reference Number cannot be null", Result = null, StatusCode = HttpStatusCode.BadRequest };
                //log the error
                Logger.Error("The Reference Number is null");
                return result;
            }
            result = new APIResponse { Success = true, ErrorMessage = "", Result = null };
            return result;
        }

        /// <summary>
        /// retieves tax payer information from EIRS API based on the parameters and returns a TaxPayerPaymentInfo object
        /// </summary>
        /// <param name="refNumber"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>TaxPayerPaymentInfo</returns>
        public TaxPayerPaymentInfo RetrieveTaxPayerInfo(string refNumber)
        {
            try
            {
                Logger.Info($"About to retrieve tax payer details for ref {refNumber}");
                var rNo = refNumber.Trim();
                var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();
                switch (firstTwoChars)
                {
                    case "AB":
                        
                        //EIRSAPIResponse res = _restService.GetAssessmentDetailsByRefNumber(refNumber);
                        //var assessmentDetails = res.Result;
                        //string output = JsonConvert.SerializeObject(assessmentDetails);
                        //AssessmentDetailsResult deserializedAssessment = JsonConvert.DeserializeObject<AssessmentDetailsResult>(output);
                        var assessment = ProcessAssessmentDetails(refNumber);
                        if (assessment != null)
                        {
                            Logger.Info("Creating a taxpayerpaymentinfo class to return to the bank");
                            var taxPayerInfo = new TaxPayerPaymentInfo
                            {
                                ReferenceAmount = assessment.AssessmentAmount,
                                ReferenceDate = assessment.AssessmentDate.Value,
                                ReferenceNumber = assessment.AssessmentRefNo,
                                TaxPayerName = assessment.TaxPayerName,
                                TaxPayerTypeName = assessment.TaxPayerTypeName,
                                ProductCode = assessment.AssessmentID.ToString(),
                                ProductName = assessment.AssessmentNotes,
                                Quantity = assessment.AssessmentRuleItems.Count(),
                                Tax = 0,
                                //confirm this
                                TotalAmountPaid = assessment.SetlementAmount
                            };
                            return taxPayerInfo;
                        }
                        return null;
                    case "SB":
                        //EIRSAPIResponse resp = _restService.GetServiceBillDetailsByRefNumber(refNumber);
                        //var serviceBill = resp.Result;
                        //string billOutput = JsonConvert.SerializeObject(serviceBill);
                        //ServiceBillResult deserializedBill = JsonConvert.DeserializeObject<ServiceBillResult>(billOutput);

                        var serviceBill = ProcessServiceBill(refNumber);

                        if (serviceBill != null)
                        {
                           
                            Logger.Info("Creating a taxpayerpaymentinfo class to return to the bank");
                            var taxPayerInfo = new TaxPayerPaymentInfo
                            {
                                ReferenceAmount = serviceBill.ServiceBillAmount,
                                ReferenceDate = serviceBill.ServiceBillDate.Value,
                                ReferenceNumber = serviceBill.ServiceBillRefNo,
                                TaxPayerName = serviceBill.TaxPayerName,
                                //PhoneNumber = phoneNumber,
                                ProductCode = serviceBill.ServiceBillID.ToString(),
                                ProductName = serviceBill.ServiceBillRefNo,
                                Quantity = serviceBill.ServiceBillItems.Count(),
                                //Tax = 0,
                                //confirm this
                                TotalAmountPaid = serviceBill.SetlementAmount
                            };
                            return taxPayerInfo;
                        }
                        return null;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while retrieving Tax Payer Information");
                Logger.Error(ex.Message, ex);
                return null;
            }


        }

        /// <summary>
        /// Validates PayDirect Call 
        /// </summary>
        /// <param name="XMLString"></param>
        /// <param name="methodName"></param>
        /// <returns>string</returns>
        public string DoPayDirectReferenceBasicValidation(string XMLString, string methodName)
        {
            try
            {
                Logger.Debug("Validating Paydirect Call");
                //does the call contain the service url
                if (!XMLString.Contains("ServiceUrl"))
                {
                    Logger.Error("ServiceUrl was not included in the call");
                    return "";
                }
                bool urlVal;
                string serviceUrl;
                //check if the service url is the one configured in the config
                switch (methodName)
                {
                    //if customer validation
                    case "CUSTOMER VALIDATION":
                        var customerDetails = Utils.DeserializeXML<CustomerInformationRequest>(XMLString);
                        if (customerDetails == null)
                        {
                            Logger.Error("Could not deserialize the xml string to a CustomerInformationRequest object");
                            return "";
                        }
                        //retrieve the taxpayer information
                        serviceUrl = Configuration.GetPayDirectReferenceServiceUrl;
                        urlVal = Utils.ConfigValueIsNull(serviceUrl);
                        if (urlVal)
                        {
                            Logger.Error("Service Url for PayDirectReferenceRequest has not been added/cannot be gotten to/from the config");
                            return "";
                        }
                        if (string.Equals(serviceUrl, customerDetails.ServiceUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.Debug("The Service Url for PayDirectReferenceRequest Configured in the config is the same as the one used in the call");
                            return "Call is valid";
                        }
                        Logger.Error("Could not validate Customer Validation PayDirect Call");
                        return "";

                    //if payment notification
                    case "PAYMENT NOTIFICATION":
                        var paymentNotification = Utils.DeserializeXML<PaymentNotificationRequest>(XMLString);
                        if (paymentNotification == null)
                        {
                            Logger.Error("Could not deserialize the xml string to a PaymentNotificationRequest object");
                            return "";
                        }
                        serviceUrl = Configuration.GetPayDirectReferenceServiceUrl;
                        urlVal = Utils.ConfigValueIsNull(serviceUrl);
                        if (urlVal)
                        {
                            Logger.Error("Service Url for PayDirectReferenceRequest has not been added/cannot be gotten to/from the config");
                            return "";
                        }
                        if (string.Equals(serviceUrl, paymentNotification.ServiceUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.Debug("The Service Url for PayDirectReferenceRequest Configured in the config is the same as the one used in the call");
                            return "Call is valid";
                        }
                        Logger.Error("Could not validate Payment Notification PayDirect Call");
                        return "";

                    default:
                        return "";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not validate PayDirect Call");
                Logger.Error(ex.StackTrace, ex);
                return "";
            }
        }

        /// <summary>
        /// Retrieves TaxPayerInfo for PayDirect Customer Validation Call
        /// </summary>
        /// <param name="merchantReference"></param>
        /// <param name="custReference"></param>
        /// <returns>PayDirectResponse</returns>
        public APIResponse RetrievePayDirectTaxPayerInfo(string merchantReference, string custReference, string thirdPartyCode)
        {
            //variables
            APIResponse response;
            CustomerInformationResponse res;
            try
            {
                Logger.Debug("About to Retrieve TaxPayerInfo for PayDirect Customer Validation Call ");

                if (custReference == null)
                {
                    res = new CustomerInformationResponse
                    {
                        Customers = new List<Customer>
                        {
                            new Customer
                            {
                                Status = 1,
                                StatusMessage = "",
                                Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = thirdPartyCode,
                                        CustReference = custReference
                            }
                        },
                        MerchantReference = merchantReference
                    };
                    return response = new APIResponse { Success = true, Result = res };
                }
                //var refNo = Utils.GetRefNoFromCustReference(custReference);
                //var phoneNumber = Utils.GetPhoneNumberFromCustReference(custReference);
                var taxPayerInfo = RetrieveTaxPayerInfo(custReference);
                if (taxPayerInfo == null)
                {
                    Logger.Error("Retrieve TaxPayerInfo for PayDirect Customer Validation Call Failed ");
                    res = new CustomerInformationResponse
                    {
                        Customers = new List<Customer>
                        {
                            new Customer
                            {
                                Status = 1,
                                StatusMessage = "Could not retrieve customer information from EIRS API, please try again",
                                Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = thirdPartyCode,
                                        CustReference = custReference
                            }
                        },
                        MerchantReference = merchantReference
                    };
                    return response = new APIResponse { Success = false, Result = res };
                }
                var payDirectResponse = new CustomerInformationResponse
                {
                    MerchantReference = merchantReference,
                    Customers = new List<Customer>
                {
                    new Customer
                    {
                        Amount =  taxPayerInfo.ReferenceAmount,
                        CustReference = custReference,
                        FirstName = taxPayerInfo.TaxPayerName,
                        Status = 0,
                        PaymentItems = new List<Item>
                        {
                            new Item
                            {
                                Price = taxPayerInfo.ReferenceAmount,
                                ProductCode = taxPayerInfo.ProductCode,
                                Tax = taxPayerInfo.Tax,
                                ProductName = taxPayerInfo.ProductName,
                                Quantity = taxPayerInfo.Quantity,
                                Subtotal = taxPayerInfo.ReferenceAmount - taxPayerInfo.Tax,
                                Total = taxPayerInfo.ReferenceAmount
                            }
                        }
                    },
                }
                };
                Logger.Debug("Return the retrieved object");
                //var responseString = Utils.SerializeToXML<CustomerInformationResponse>(payDirectResponse);
                return response = new APIResponse { Success = true, Result = payDirectResponse };
            }
            catch (Exception ex)
            {
                Logger.Error("Could not Retrieve TaxPayerInfo for PayDirect Customer Validation");
                Logger.Error(ex.StackTrace, ex);
                res = new CustomerInformationResponse
                {
                    Customers = new List<Customer>
                        {
                            new Customer
                            {
                                Status = 1,
                                StatusMessage = $"An error occured, could not retrieve customer information, please try again ",
                                Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = thirdPartyCode,
                                        CustReference = custReference
                            }
                        },
                    MerchantReference = merchantReference
                };
                //response = Utils.SerializeToXML<CustomerInformationResponse>(res);
                return response = new APIResponse { Success = false, Result = res };
            }

        }

        public ServiceBillResult ProcessServiceBill(string refNumber)
        {
            try
            {
                ServiceBillResult serviceBill;

                EIRSAPIResponse resp = _restService.GetServiceBillDetailsByRefNumber(refNumber);
                var serviceBillDetail = resp.Result;
                string billOutput = JsonConvert.SerializeObject(serviceBillDetail);
                ServiceBillResult serviceBillDetails = JsonConvert.DeserializeObject<ServiceBillResult>(billOutput, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });

                if (serviceBillDetails == null)
                {
                    Logger.Error("An error occurred while trying to process service bill details");
                    return null;
                }

                //get the main content; first check if that record exists in our db
                serviceBill = _serviceBillRepo.Fetch(x => x.ServiceBillRefNo == serviceBillDetails.ServiceBillRefNo).FirstOrDefault();
                if (serviceBill == null)
                {
                    Logger.Debug("Service bill doesn't exist in the db, so save");
                    serviceBill = serviceBillDetails;
                    serviceBill.DateCreated = DateTime.Today;
                    serviceBill.DateModified = DateTime.Today;
                    serviceBill.IsDeleted = false;

                    Logger.Debug("Saving the ServiceBill Details");
                    //save the returned info
                    _serviceBillRepo.Insert(serviceBill);
                    Logger.Debug("Successfully saved the ServiceBill Details");

                }

                else
                {
                    Logger.Debug("Service bill already exists in the database, so update record");
                    serviceBill.ServiceBillID = serviceBillDetails.ServiceBillID;
                    serviceBill.ServiceBillAmount = serviceBillDetails.ServiceBillAmount;
                    serviceBill.SetlementAmount = serviceBillDetails.SetlementAmount;
                    serviceBill.SettlementDate = serviceBillDetails.SettlementDate;
                    serviceBill.SettlementDueDate = serviceBillDetails.SettlementDueDate;
                    serviceBill.SettlementStatusName = serviceBillDetails.SettlementStatusName;
                    serviceBill.SettlementStatusID = serviceBillDetails.SettlementStatusID;
                    serviceBill.TaxPayerID = serviceBillDetails.TaxPayerID;
                    serviceBill.TaxPayerName = serviceBillDetails.TaxPayerName;
                    serviceBill.SettlementDate = serviceBillDetails.SettlementDate;
                    serviceBill.DateModified = DateTime.Now;
                    Logger.Debug("updating the assessment details");
                    _serviceBillRepo.Update(serviceBill);
                    Logger.Debug("successfully updated assessment details");
                }

                #region service bill rule items
                Logger.Debug("fetch and save/update the service bill rule items");
                var res = _restService.GetServiceBillItems(serviceBill.ServiceBillID);
                var serviceBillItems = res.Result;
                string output = JsonConvert.SerializeObject(serviceBillItems);
                IEnumerable<ServiceBillItem> deserializedServiceBillItems = JsonConvert.DeserializeObject<IEnumerable<ServiceBillItem>>(output, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });

                if (deserializedServiceBillItems.Count() < 0)
                {
                    Logger.Error("An error occurred while trying to process service bill details");
                    return null;
                }

                var serviceBillRuleItems = _serviceBillItemRepo.Fetch(x => x.ServiceBill.ServiceBillID == serviceBill.ServiceBillID);
                if (serviceBillRuleItems.Count() > 0)
                {
                    Logger.Debug("Service Bill Items already exists in the db, so update");

                    foreach (var item in deserializedServiceBillItems)
                    {
                        serviceBillRuleItems.Where(x => x.MDAServiceItemReferenceNo == item.MDAServiceItemReferenceNo).FirstOrDefault().CopyFrom(item);
                    }

                    _serviceBillItemRepo.UpdateRange(serviceBillRuleItems);
                    serviceBill.ServiceBillItems = serviceBillRuleItems;
                    Logger.Debug("successfully updated assessment rule items");

                }
                else
                {
                    Logger.Debug("Service Bill Items dos not exist in the db, so save");
                    foreach (var item in deserializedServiceBillItems)
                    {
                        item.ServiceBill = serviceBill;
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                    _serviceBillItemRepo.SaveBundle(deserializedServiceBillItems);
                    serviceBill.ServiceBillItems = deserializedServiceBillItems;
                    Logger.Debug("successfully saved service bill items");
                }
                #endregion

                #region mdaa service bill rules
                Logger.Debug("fetch and save/update the service bill rule items");
                var serviceBillRulesResult = _restService.GetServiceBillRules(serviceBill.ServiceBillID);
                var serviceBillRule = serviceBillRulesResult.Result;
                string ruleOutput = JsonConvert.SerializeObject(serviceBillRule);
                IEnumerable<MDAService> deserializedServiceBillRules = JsonConvert.DeserializeObject<IEnumerable<MDAService>>(ruleOutput, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });

                if (deserializedServiceBillRules.Count() < 0)
                {
                    Logger.Error("An error occurred while trying to process service bill rules");
                    return null;
                }

                var serviceBillRules = _mdaServiceRepo.Fetch(x => x.ServiceBill.ServiceBillID == serviceBill.ServiceBillID);
                if (serviceBillRules.Count() > 0)
                {
                    Logger.Debug("Service Bill Rules already exists in the db, so update");

                    foreach (var item in deserializedServiceBillRules)
                    {
                        serviceBillRules.Where(x => x.MDAServiceID == item.MDAServiceID).FirstOrDefault().CopyFrom(item);
                    }

                    _mdaServiceRepo.UpdateRange(serviceBillRules);
                    serviceBill.MDAServiceRules = serviceBillRules;
                    Logger.Debug("successfully updated mda service rules");

                }
                else
                {
                    Logger.Debug("Service Bill Rules dos not exist in the db, so save");
                    foreach (var item in deserializedServiceBillRules)
                    {
                        item.ServiceBill = serviceBill;
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                    _mdaServiceRepo.SaveBundle(deserializedServiceBillRules);
                    serviceBill.MDAServiceRules = deserializedServiceBillRules;
                    Logger.Debug("successfully saved service bill rules");
                }
                #endregion 
                return serviceBill;

            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while trying to process service bill details");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        //private TaxPayerPaymentInfo ProcessAssessmentDetails(AssessmentDetailsResult assessmentDetails)
        public AssessmentDetailsResult ProcessAssessmentDetails(string refNumber)
        {
            try
            {
                AssessmentDetailsResult assessment;
                Logger.Debug($"About to process assessment details for assessment ref - {refNumber}");

                Logger.Debug("Fetch assessment details from the API");
                EIRSAPIResponse response = _restService.GetAssessmentDetailsByRefNumber(refNumber);
                var assessmentDetail = response.Result;
                string output = JsonConvert.SerializeObject(assessmentDetail);
                AssessmentDetailsResult assessmentDetails = JsonConvert.DeserializeObject<AssessmentDetailsResult>(output, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });
                Logger.Debug("Assessment details successfully retrieved and deserialized");
                // IEnumerable<AssessmentRuleItem> assessmentRuleItems;
                //use the assessment id to get the assessment items; save the items and the parent object too

                //get the main content; first check if that record exists in our db
                Logger.Debug("first check if that record exists in our db");
                assessment = _assessmentRepo.Fetch(x => x.AssessmentRefNo == assessmentDetails.AssessmentRefNo).FirstOrDefault();
                if (assessment == null)
                {
                    Logger.Debug("it doesn't exist in the database, so save record");
                    //it doesn't exist in the database, so save record
                    assessment = assessmentDetails;
                    assessment.DateCreated = DateTime.Now;
                    assessment.DateModified = DateTime.Now;
                    assessment.IsDeleted = false;
                    Logger.Info("Saving the Assessment Details");
                    //save the returned info
                    _assessmentRepo.Insert(assessment);
                    Logger.Debug("successfully saved assessment details");
                }

                else
                {
                    Logger.Debug("it already exists in the database, so update record");
                    assessment.AssessmentID = assessmentDetails.AssessmentID;
                    assessment.AssessmentAmount = assessmentDetails.AssessmentAmount;
                    assessment.SetlementAmount = assessmentDetails.SetlementAmount;
                    assessment.SettlementDate = assessmentDetails.SettlementDate;
                    assessment.SettlementDueDate = assessmentDetails.SettlementDueDate;
                    assessment.SettlementStatusName = assessmentDetails.SettlementStatusName;
                    assessment.SettlementStatusID = assessmentDetails.SettlementStatusID;
                    assessment.TaxPayerID = assessmentDetails.TaxPayerID;
                    assessment.TaxPayerName = assessmentDetails.TaxPayerName;
                    assessment.TaxPayerTypeID = assessmentDetails.TaxPayerTypeID;
                    assessment.TaxPayerTypeName = assessmentDetails.TaxPayerTypeName;
                    assessment.TaxPayerRIN = assessmentDetails.TaxPayerRIN;
                    assessment.AssessmentDate = assessmentDetails.AssessmentDate;
                    assessment.DateModified = DateTime.Now;
                    Logger.Debug("updating the assessment details");
                    _assessmentRepo.Update(assessment);
                    Logger.Debug("successfully updated assessment details");
                }

                #region AssessmentRuleItems
                Logger.Debug($"fetch and save/update the assessment rule items using assessment ID - {assessmentDetails.AssessmentID}");
                var res = _restService.GetAssessmentRuleItems(assessmentDetails.AssessmentID);
                var assessmentItems = res.Result;
                string ItemsOutput = JsonConvert.SerializeObject(assessmentItems);
                IEnumerable<AssessmentRuleItem> deserializedAssessment = JsonConvert.DeserializeObject<IEnumerable<AssessmentRuleItem>>(ItemsOutput, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });

                Logger.Debug("Successfully retrieved the records from API and deserialized it");
                var assessmentRuleItems = _assessmentRuleItemRepo.Fetch(x => x.AssessmentDetails.AssessmentID == assessmentDetails.AssessmentID);
                if (assessmentRuleItems.Count() > 0)
                {
                    Logger.Debug($" {assessmentRuleItems.Count()} retrieved. Assessment Rule Items already exists in the db, so update");

                    var count = 0;
                    foreach (var item in deserializedAssessment)
                    {
                        Logger.Debug($"Updating item with AAIID- {item.AAIID}, item number {count++}");
                        assessmentRuleItems.Where(x => x.AAIID == item.AAIID).FirstOrDefault().CopyFrom(item);
                    }


                    _assessmentRuleItemRepo.UpdateRange(assessmentRuleItems);
                    assessment.AssessmentRuleItems = assessmentRuleItems;
                    Logger.Debug("successfully updated assessment rule items");
                }
                else
                {
                    Logger.Debug("Assessment Rule Items does not exist in the db, so save");
                    foreach (var item in deserializedAssessment)
                    {
                        item.AssessmentDetails = assessment;
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                    _assessmentRuleItemRepo.SaveBundle(deserializedAssessment);
                    assessment.AssessmentRuleItems = deserializedAssessment;
                    Logger.Debug("successfully saved assessment rule items");

                }
                #endregion

                #region AssessmentRules
                Logger.Debug($"fetch and save/update the assessment rule using assessment ID - {assessmentDetails.AssessmentID}");
                var assmntRulesres = _restService.GetAssessmentRules(assessmentDetails.AssessmentID);
                var assessmentRule = assmntRulesres.Result;
                string rulesOutput = JsonConvert.SerializeObject(assessmentRule);
                IEnumerable<AssessmentRule> deserializedAssessmentRules = JsonConvert.DeserializeObject<IEnumerable<AssessmentRule>>(rulesOutput, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate});

                Logger.Debug("assessment rules succesfully retreieved and deserialized");
                var assessmentRules = _assessmentRuleRepo.Fetch(x => x.AssessmentDetails.AssessmentID == assessmentDetails.AssessmentID).ToList();

                if (assessmentRules.Any())
                {
                    Logger.Debug($" {assessmentRules.Count()} rerieved. Assessment Rules already exists in the db, so update");

                    //update each item
                    var count = 0;
                    foreach (var item in deserializedAssessmentRules)
                    {
                        Logger.Debug($"Updating item with AAIID- {item.AARID}, item number {count++}");
                        assessmentRules.Where(x => x.AssessmentRuleID == item.AssessmentRuleID).FirstOrDefault().CopyFrom(item);
                    }

                    _assessmentRuleRepo.UpdateRange(assessmentRules);
                    assessment.AssessmentRules = assessmentRules;
                    Logger.Debug("successfully updated assessment rules");
                }
                else
                {
                    Logger.Debug("Assessment Rules do not exist in the db, so save");
                    foreach (var item in deserializedAssessmentRules)
                    {
                        item.AssessmentDetails = assessment;
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                    _assessmentRuleRepo.SaveBundle(deserializedAssessmentRules);
                    assessment.AssessmentRules = deserializedAssessmentRules;
                    Logger.Debug("successfully saved assessment rules");

                }
                #endregion


                return assessment;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while trying to process assessment details");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        public APIResponse RetrievePayDirectPOATaxPayerInfo(string merchantReference, string custReference, string thirdPartyCode)
        {
            //variables
            APIResponse response;
            CustomerInformationResponse res;
            try
            {
                Logger.Debug("About to Retrieve TaxPayerInfo for PayDirect Customer Validation Call ");
                if (string.IsNullOrWhiteSpace(custReference))
                {
                    res = new CustomerInformationResponse
                    {
                        MerchantReference = merchantReference,
                        Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = custReference,
                                        Status = 0,
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = thirdPartyCode
                                    }
                                }
                    };
                    return response = new APIResponse { Success = true, Result = res };
                }
                var RIN = Utils.GetRINFromCustReference(custReference);
                var phoneNumber = Utils.GetPhoneNumberFromCustReference(custReference);
                var taxPayerInfo = RetrieveTaxPayerInfo(RIN,phoneNumber);
                if (taxPayerInfo == null)
                {
                    Logger.Error("Retrieve TaxPayerInfo for PayDirect Customer Validation Call Failed ");
                    res = new CustomerInformationResponse
                    {
                        MerchantReference = merchantReference,
                        Customers = new List<Customer>
                                {
                                    new Customer
                                    {
                                        CustReference = custReference,
                                        Status = 1,
                                        StatusMessage = "Could not retrieve customer information from EIRS API, please try again",
                                        Amount = 0,
                                        FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = thirdPartyCode
                                    }
                                }
                    };
                    return response = new APIResponse { Success = false, Result = res };
                }
                var payDirectResponse = new CustomerInformationResponse
                {
                    MerchantReference = merchantReference,
                    Customers = new List<Customer>
                {
                    new Customer
                    {
                        CustReference = custReference,
                        FirstName = taxPayerInfo.TaxPayerName,
                        Status = 0,
                        Phone = taxPayerInfo.TaxPayerMobileNumber,
                        Email = "",
                        ThirdPartyCode = thirdPartyCode
                    },
                }
                };
                Logger.Debug("Return the retrieved object");
                return response = new APIResponse { Success = true, Result = payDirectResponse };
            }
            catch (Exception ex)
            {
                Logger.Error("Could not Retrieve TaxPayerInfo for PayDirect Customer Validation");
                Logger.Error(ex.StackTrace, ex);
                res = new CustomerInformationResponse
                {
                    Customers = new List<Customer>
                        {
                            new Customer
                            {
                                CustReference = custReference,
                                Status = 1,
                                StatusMessage = $"An error -  {ex.Message} - occured, could not retrieve customer information, please try again ",
                                Amount = 0,
                                FirstName = "",
                                        Email = "",
                                        Phone = "",
                                        ThirdPartyCode = thirdPartyCode
                            }
                        },
                    MerchantReference = merchantReference
                };
                //response = Utils.SerializeToXML<CustomerInformationResponse>(res);
                return response = new APIResponse { Success = false, Result = res };
            }
        }

        /// <summary>
        /// Retrieve TaxPayerInfo via API using the RIN and Mobile Number as search parameters
        /// </summary>
        /// <param name="RIN"></param>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public POATaxPayerResponse RetrieveTaxPayerInfo(string RIN, string mobileNumber)
        {
            try
            {
                var rin = RIN.Trim();
                Logger.Debug($"About to make API calls to retriev tax payer info using RIN {RIN} and Phone Number {mobileNumber}");
                EIRSAPIResponse res = _restService.GetTaxPayerByRINAndMobile(rin, mobileNumber);
                var taxPayer = res.Result;
                var output = JsonConvert.SerializeObject(taxPayer);
                IEnumerable<POATaxPayerResponse> deserializedTaxPayer = JsonConvert.DeserializeObject<IEnumerable<POATaxPayerResponse>>(output);

                if (deserializedTaxPayer.Count() > 0)
                {
                    Logger.Debug("Successfully retrieved taxpayer's data");
                    
                    var taxPayerInfo = ProcessPOATaxPayerDetails(deserializedTaxPayer.FirstOrDefault());
                    if (taxPayerInfo != null)
                    {
                        return taxPayerInfo;
                    }
                    return null;
                }
                return null;

            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while retrieving Tax Payer Information");
                Logger.Error(ex.Message, ex);
                return null;
            }

        }

        public POATaxPayerResponse ProcessPOATaxPayerDetails(POATaxPayerResponse deserializedTaxPayer)//private
        {
            try
            {
                Logger.Debug($"Processing Pay On Account Tax payer Details for RIN - {deserializedTaxPayer.TaxPayerRIN} and phone number - {deserializedTaxPayer.TaxPayerMobileNumber}");
                TaxPayerDetails taxPayerDetails;
                //check if the record exists in the db
                //if no, save; if yes, update

                Logger.Debug("check if the record exists in the db");
                taxPayerDetails = _taxPayerRepo.Fetch(x => x.TaxPayerRIN == deserializedTaxPayer.TaxPayerRIN).FirstOrDefault();
                if (taxPayerDetails == null)
                {
                    Logger.Debug("it doesn't exist in the database, so save record");
                    //it doesn't exist in the database, so save record
                    taxPayerDetails = new TaxPayerDetails
                    {
                        TaxPayerRIN = deserializedTaxPayer.TaxPayerRIN,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        TaxPayerAddress = deserializedTaxPayer.TaxPayerAddress,
                        TaxPayerID = deserializedTaxPayer.TaxPayerID,
                        TaxPayerMobileNumber = deserializedTaxPayer.TaxPayerMobileNumber,
                        TaxPayerName = deserializedTaxPayer.TaxPayerName,
                        TaxPayerTypeID = deserializedTaxPayer.TaxPayerTypeID,
                       
                        //TaxPayerTypeName = deserializedTaxPayer.TaxPayerTypeName,
                        IsDeleted = false
                    };

                    Logger.Info("Saving the Tax Payer Details");
                    //save the returned info
                    _taxPayerRepo.Insert(taxPayerDetails);
                    Logger.Debug("successfully saved Tax Payer details");
                    return deserializedTaxPayer;
                }

                else
                {
                    Logger.Debug("it already exists in the database, so update record");
                    taxPayerDetails.DateModified = DateTime.Now;
                    taxPayerDetails.TaxPayerAddress = deserializedTaxPayer.TaxPayerAddress;
                    taxPayerDetails.TaxPayerID = deserializedTaxPayer.TaxPayerID;
                    taxPayerDetails.TaxPayerMobileNumber = deserializedTaxPayer.TaxPayerMobileNumber;
                    taxPayerDetails.TaxPayerName = deserializedTaxPayer.TaxPayerName;
                    taxPayerDetails.TaxPayerTypeID = deserializedTaxPayer.TaxPayerTypeID;

                    Logger.Debug("updating the Tax Payer details");
                    _taxPayerRepo.Update(taxPayerDetails);
                    Logger.Debug("successfully updated Tax Payer details");
                    return deserializedTaxPayer;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred, couldd not process tax payer details - {ex.Message}");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        public string DoPayDirectPOABasicValidation(string XMLString, string methodName)
        {
            try
            {
                Logger.Debug("Validating Paydirect Call");
                //does the call contain the service url
                if (!XMLString.Contains("ServiceUrl"))
                {
                    Logger.Error("ServiceUrl was not included in the call");
                    return "";
                }
                bool urlVal;
                string serviceUrl;
                //check if the service url is the one configured in the config
                switch (methodName)
                {
                    //if customer validation
                    case "CUSTOMER VALIDATION":
                        var customerDetails = Utils.DeserializeXML<CustomerInformationRequest>(XMLString);
                        if (customerDetails == null)
                        {
                            Logger.Error("Could not deserialize the xml string to a CustomerInformationRequest object");
                            return "";
                        }
                        //retrieve the taxpayer information
                        serviceUrl = Configuration.GetPayDirectPOAServiceUrl;
                        urlVal = Utils.ConfigValueIsNull(serviceUrl);
                        if (urlVal)
                        {
                            Logger.Error("Service Url for PayDirectPOAServiceUrl has not been added/cannot be gotten to/from the config");
                            return "";
                        }
                        if (string.Equals(serviceUrl, customerDetails.ServiceUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.Debug("The Service Url for PayDirectPOAServiceUrl Configured in the config is the same as the one used in the call");
                            return "Call is valid";
                        }
                        Logger.Error("Could not validate Customer Validation PayDirect Call");
                        return "";

                    //if payment notification
                    case "PAYMENT NOTIFICATION":
                        var paymentNotification = Utils.DeserializeXML<PaymentNotificationRequest>(XMLString);
                        if (paymentNotification == null)
                        {
                            Logger.Error("Could not deserialize the xml string to a PaymentNotificationRequest object");
                            return "";
                        }
                        serviceUrl = Configuration.GetPayDirectPOAServiceUrl;
                        urlVal = Utils.ConfigValueIsNull(serviceUrl);
                        if (urlVal)
                        {
                            Logger.Error("Service Url for PayDirectPOAServiceUrl has not been added/cannot be gotten to/from the config");
                            return "";
                        }
                        if (string.Equals(serviceUrl, paymentNotification.ServiceUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.Debug("The Service Url for PayDirectPOAServiceUrl Configured in the config is the same as the one used in the call");
                            return "Call is valid";
                        }
                        Logger.Error("Could not validate Payment Notification PayDirect Call");
                        return "";

                    default:
                        return "";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not validate PayDirect Call");
                Logger.Error(ex.StackTrace, ex);
                return "";
            }
        }

        public AssessmentDetailsResult GetAssessmentDetails(string rNo, EIRSTaskConfigValues configValues)
        {
            try
            { 
                Logger.Debug("Trying to fetch assessment detials");
                EIRSAPIResponse res = _restService.GetAssessmentDetailsByRefNumber(rNo, configValues);
                var assessmentDetails = res.Result;
                string output = JsonConvert.SerializeObject(assessmentDetails);
                AssessmentDetailsResult deserializedAssessment = JsonConvert.DeserializeObject<AssessmentDetailsResult>(output);
                if (assessmentDetails != null)
                {
                    EIRSAPIResponse ruleItemsResponse = _restService.GetAssessmentRuleItems(deserializedAssessment.AssessmentID, configValues);
                    var assessmentRuleItems = ruleItemsResponse.Result;
                    string result = JsonConvert.SerializeObject(assessmentRuleItems);
                    IEnumerable<AssessmentRuleItem> deserializedAssessmentItems = JsonConvert.DeserializeObject<IEnumerable<AssessmentRuleItem>>(result);
                    deserializedAssessment.AssessmentRuleItems = deserializedAssessmentItems;
                Logger.Debug("Successfully fetched assessment detials");
                return deserializedAssessment;
                }
            Logger.Debug("Unable to fetch assessment detials");
            return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred - {ex.Message}, could not retrieve assessment details ");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        public ServiceBillResult GetServiceBillDetails(string rNo, EIRSTaskConfigValues configValues)
        {
            try
            {
                Logger.Debug("Trying to fetch service bill detials");
                EIRSAPIResponse res = _restService.GetServiceBillDetailsByRefNumber(rNo, configValues);
                var serviceBillDetails = res.Result;
                string output = JsonConvert.SerializeObject(serviceBillDetails);
                ServiceBillResult deserializedServiceBill = JsonConvert.DeserializeObject<ServiceBillResult>(output);
                if (serviceBillDetails != null)
                {
                    EIRSAPIResponse ruleItemsResponse = _restService.GetServiceBillItems(deserializedServiceBill.ServiceBillID, configValues);
                    var serviceBillItems = ruleItemsResponse.Result;
                    string result = JsonConvert.SerializeObject(serviceBillItems);
                    IEnumerable<ServiceBillItem> deserializedServiceBillItems = JsonConvert.DeserializeObject<IEnumerable<ServiceBillItem>>(result);
                    deserializedServiceBill.ServiceBillItems = deserializedServiceBillItems;
                    Logger.Debug("Successfully fetched service bill detials");
                    return deserializedServiceBill;
                }
                Logger.Debug("Unable to fetch service bill detials");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred - {ex.Message}, could not retrieve service bill details ");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Fetches TaxPayer Data from the db using the RIn and Mobile Number
        /// </summary>
        /// <param name="RIN"></param>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public POATaxPayerResponse FetchPOATaxPayerInfo(string RIN, string mobileNumber)
        {
            try
            {
                Logger.Debug("About to fetch tax payer details from the taxpayer db");
                var taxPayer = _taxPayerRepo.Find(x => x.TaxPayerRIN == RIN && x.TaxPayerMobileNumber == mobileNumber);
                if (taxPayer != null)
                {
                    var taxPayerResponse = new POATaxPayerResponse
                    {
                        TaxPayerMobileNumber = taxPayer.TaxPayerMobileNumber,
                        TaxPayerRIN = taxPayer.TaxPayerRIN,
                        TaxPayerID = taxPayer.TaxPayerID,
                        TaxPayerName = taxPayer.TaxPayerName,
                        TaxPayerTypeID = taxPayer.TaxPayerTypeID,
                    };
                    Logger.Debug("successfully fetched tax payer details from the taxpayer db");
                    return taxPayerResponse;
                }
                Logger.Error("Unable to fatch tax payer details from the taxpayer db");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to fatch tax payer details from the taxpayer db");
                Logger.Error(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives Tax Payer data from the API using the RIN
        /// </summary>
        /// <param name="RIN"></param>
        /// <returns></returns>
        public IEnumerable<POATaxPayerResponse> RetrieveTaxPayerInfoByRIN(string RIN)
        {
            try
            {
                var rin = RIN.Trim();

                EIRSAPIResponse res = _restService.GetTaxPayerByRIN(rin);
                var taxPayer = res.Result;
                var output = JsonConvert.SerializeObject(taxPayer);
                IEnumerable<POATaxPayerResponse> deserializedTaxPayers = JsonConvert.DeserializeObject<IEnumerable<POATaxPayerResponse>>(output);

                var returnValue = new List<POATaxPayerResponse>();

                if (deserializedTaxPayers.Count() > 0)
                {
                    Logger.Debug($"Successfully retrieved {deserializedTaxPayers.Count()} tax payer details using the search by RIN");
                    foreach (var deserializedTaxPayer in deserializedTaxPayers)
                    {
                        var taxPayerType = _taxPayerTypeRepo.Find(x => x.TaxPayerTypeID == deserializedTaxPayer.TaxPayerTypeID);
                        if (taxPayerType != null)
                        {
                            var taxPayerTypeName = taxPayerType.TaxPayerTypeName;
                            deserializedTaxPayer.TaxPayerTypeName = taxPayerTypeName;
                        }
                        else
                        {
                            deserializedTaxPayer.TaxPayerTypeName = deserializedTaxPayer.TaxPayerTypeID.ToString();
                        }
                        var taxPayerInfo = ProcessPOATaxPayerDetails(deserializedTaxPayer);
                        if (taxPayerInfo != null)
                        {
                            returnValue.Add(taxPayerInfo);
                        }
                    }
                    return returnValue;
                }
                Logger.Error("An error occured-could not successfully Tax Payer Details");
                return null;

            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while retrieving Tax Payer Information");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives Tax Payer data from the API using the Mobile Number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public IEnumerable<POATaxPayerResponse> RetrieveTaxPayerInfoByMobileNumber(string mobileNumber)
        {
            try
            {
                var mobile = mobileNumber.Trim();

                EIRSAPIResponse res = _restService.GetTaxPayerByMobileNumber(mobile);
                var taxPayer = res.Result;
                var output = JsonConvert.SerializeObject(taxPayer);
                IEnumerable<POATaxPayerResponse> deserializedTaxPayers = JsonConvert.DeserializeObject<IEnumerable<POATaxPayerResponse>>(output);

                var returnValue = new List<POATaxPayerResponse>();

                if (deserializedTaxPayers.Count() > 0)
                {
                    Logger.Debug($"Successfully retrieved {deserializedTaxPayers.Count()} tax payer details using the search by mobile number ");
                    foreach (var deserializedTaxPayer in deserializedTaxPayers)
                    {
                        var taxPayerType = _taxPayerTypeRepo.Find(x => x.TaxPayerTypeID == deserializedTaxPayer.TaxPayerTypeID);
                        if (taxPayerType != null)
                        {
                            var taxPayerTypeName = taxPayerType.TaxPayerTypeName;
                            deserializedTaxPayer.TaxPayerTypeName = taxPayerTypeName;
                        }
                        else
                        {
                            deserializedTaxPayer.TaxPayerTypeName = deserializedTaxPayer.TaxPayerTypeID.ToString();
                        }
                        var taxPayerInfo = ProcessPOATaxPayerDetails(deserializedTaxPayer);
                        if (taxPayerInfo != null)
                        {
                            returnValue.Add(taxPayerInfo);
                        }
                    }
                    return returnValue;
                }
                Logger.Error("An error occured-could not successfully Tax Payer Details");
                return null;

            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while retrieving Tax Payer Information");
                Logger.Error(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives Tax Payer data from the API using the Business Name
        /// </summary>
        /// <param name="businessName"></param>
        /// <returns></returns>
        public IEnumerable<POATaxPayerResponse> RetrieveTaxPayerInfoByBusinessName(string businessName)
        {
            try
            {
                var business = businessName.Trim();
                var returnValue = new List<POATaxPayerResponse>();

                EIRSAPIResponse res = _restService.GetTaxPayerByBusinessName(business);
                var taxPayer = res.Result;
                var output = JsonConvert.SerializeObject(taxPayer);
                IEnumerable<POATaxPayerResponse> deserializedTaxPayers = JsonConvert.DeserializeObject<IEnumerable<POATaxPayerResponse>>(output);

                if (deserializedTaxPayers.Count() > 0)
                {
                    Logger.Debug($"Successfully retrieved {deserializedTaxPayers.Count()} tax payer details using the search by Business name ");
                    foreach (var deserializedTaxPayer in deserializedTaxPayers)
                    {
                        var taxPayerType = _taxPayerTypeRepo.Find(x => x.TaxPayerTypeID == deserializedTaxPayer.TaxPayerTypeID);
                        if (taxPayerType != null)
                        {
                            var taxPayerTypeName = taxPayerType.TaxPayerTypeName;
                            deserializedTaxPayer.TaxPayerTypeName = taxPayerTypeName;
                        }
                        else
                        {
                            deserializedTaxPayer.TaxPayerTypeName = deserializedTaxPayer.TaxPayerTypeID.ToString();
                        }
                        var taxPayerInfo = ProcessPOATaxPayerDetails(deserializedTaxPayer);
                        if (taxPayerInfo != null)
                        {
                            returnValue.Add(taxPayerInfo);
                        }
                    }
                    return returnValue;
                }
                Logger.Error("Could not successsfully retrieve Tax Payer details");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while retrieving Tax Payer Information");
                Logger.Error(ex.Message, ex);
                return null;
            }
        }
        public List<TaxPayerDetails> GetTaxPayerDetails(string filterOption)
        {
            try
            {
                var returnmodels = new List<TaxPayerDetails>();
                Logger.Info($"About to build query to get Tax Payer Details");
                var query = BuildQueryTaxpayer(filterOption);
                var record = query.List<TaxPayerDetails>().OrderBy(c => c.TaxPayerName).Where(d => d.Id > 0 && d.IsDeleted == false);
                var str = JsonConvert.SerializeObject(record);
                return JsonConvert.DeserializeObject<List<TaxPayerDetails>>(str);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return new List<TaxPayerDetails>();
            }
        }
        private ICriteria BuildQueryTaxpayer(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(TaxPayerDetails));
            CultureInfo culInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<TaxPayerFilterParams>(filterOption);
            if (filter != null)
            {
                if (filter.TaxPayerTypeID != null)
                {
                    criteria.Add(Restrictions.Eq("TaxPayerTypeID", filter.TaxPayerTypeID));
                }
                if (!string.IsNullOrEmpty(filter.TaxPayerRIN))
                {
                    criteria.Add(Restrictions.Eq("TaxPayerRIN", filter.TaxPayerRIN));
                }
                if (!string.IsNullOrEmpty(filter.TaxPayerMobileNumber))
                {
                    criteria.Add(Restrictions.Eq("TaxPayerRIN", filter.TaxPayerMobileNumber));
                }
            }
            return criteria;
        }
        public List<AssessmentDetailsResult> GetAssessmentDetailsResult(string filterOption)
        {
            try
            {
                //var returnmodels = new List<AssessmentDetailsResult>();
                Logger.Info($"About to build query to get Assessment Details from the db");
                var query = BuildQueryAssessmentDetails(filterOption);
                var record = query.List<AssessmentDetailsResult>().OrderBy(a => a.AssessmentRefNo).Where(b => b.Id > 0 && b.IsDeleted == false);
                var str = JsonConvert.SerializeObject(record);
                return JsonConvert.DeserializeObject<List<AssessmentDetailsResult>>(str);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return new List<AssessmentDetailsResult>();
            }
        }
        private ICriteria BuildQueryAssessmentDetails(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(AssessmentDetailsResult));
            CultureInfo culInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<TaxPayerAssessmentFilterParams>(filterOption);
            if(filter != null)
            {
                if(filter.fromRange != null && filter.endRange != null)
                {
                    criteria.Add(Restrictions.Between("AssessmentDate", filter.fromRange.Value, filter.endRange.Value));
                }
                if(filter.AssessmentRefNo != null)
                {
                    criteria.Add(Restrictions.Eq("AssessmentRefNo", filter.AssessmentRefNo));
                }
                if(filter.TaxPayerRIN != null)
                {
                    criteria.Add(Restrictions.Eq("TaxPayerRIN", filter.TaxPayerRIN));
                }
                if(filter.minAmount != null && filter.maxAmount != null)
                {
                    criteria.Add(Restrictions.Between("AssessmentAmount",filter.minAmount,filter.maxAmount));
                }
                if(filter.SettlementStatusID != null)
                {
                    criteria.Add(Restrictions.Eq("SettlementStatusID", filter.SettlementStatusID));
                }
                if(filter.Active != null)
                {
                    criteria.Add(Restrictions.Eq("Active", filter.Active));
                }
                if(filter.settlementFromRange != null && filter.settlementEndRange != null)
                {
                    criteria.Add(Restrictions.Between("SettlementDueDate",filter.settlementFromRange, filter.settlementEndRange));
                }
                if(filter.due != null)
                {
                    criteria.Add(Restrictions.Gt("SettlementDueDate", DateTime.Now.Date));
                }
            }
            return criteria;
        }
        public List<AssessmentRule> GetAssessmentRule(string filterOption)
        {
            try
            {
                Logger.Info($"About to build query to get Assessment rules from the db");
                var query = BuildQueryAssessmentRule(filterOption);
                var record = query.List<AssessmentRule>().OrderBy(a => a.AssessmentRuleName).Where(b => b.Id > 0 && b.IsDeleted == false);
                var str = JsonConvert.SerializeObject(record);
                return JsonConvert.DeserializeObject<List<AssessmentRule>>(str);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return new List<AssessmentRule>();
            }
        }
        private ICriteria BuildQueryAssessmentRule(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(AssessmentRule));
            CultureInfo culInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<TaxPayerAssessmentRuleFilterParams>(filterOption);
            if(filter != null)
            {
                if(filter.AssetTypeId != null)
                {
                    criteria.Add(Restrictions.Eq("AssetTypeId", filter.AssetTypeId));
                }
                if(filter.TaxYear != null)
                {
                    criteria.Add(Restrictions.Eq("TaxYear", filter.TaxYear));
                }
                if(filter.minAssessmentRuleAmount != null && filter.maxAssessmentRuleAmount != null)
                {
                    criteria.Add(Restrictions.Between("AssessmentRuleAmount", filter.minAssessmentRuleAmount, filter.maxAssessmentRuleAmount));
                }
                if (filter.minSettledAmount != null && filter.maxSettledAmount != null)
                {
                    criteria.Add(Restrictions.Between("SettledAmount", filter.minSettledAmount, filter.maxSettledAmount));
                }
            }
            return criteria;
        }
        public List<Asset> GetAsset(string filterOption)
        {
            try
            {
                Logger.Info($"About to build query to get Assessment rules from the db");
                var query = BuildQueryAsset(filterOption);
                var record = query.List<AssessmentRule>().OrderBy(a => a.AssetRIN).Where(b => b.Id > 0 && b.IsDeleted == false);
                var str = JsonConvert.SerializeObject(record);
                var records = JsonConvert.DeserializeObject<List<AssessmentRule>>(str);
                var Assets = new List<Asset>();
                var Asset = new Asset();
                var AssetIds = records.Select(x => x.AssetID).Distinct();
                foreach (var asset in AssetIds)
                {
                    Asset = new Asset
                    {
                        AssetRIN = records.Where(a => a.AssetID == asset).FirstOrDefault().AssetRIN,
                        AssetType = records.Where(a => a.AssetID == asset).FirstOrDefault().AssetTypeName,
                        Profile = records.Where(a => a.AssetID == asset).FirstOrDefault().ProfileDescription,
                        TotalAmountBilled = records.Where(a => a.AssetID == asset).Sum(b => b.AssessmentRuleAmount),
                        TotalAmountSettled = records.Where(a => a.AssetID == asset).Sum(b => b.SettledAmount)
                    };
                    Assets.Add(Asset);
                }
                return Assets;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return new List<Asset>();
            }
        }
        private ICriteria BuildQueryAsset(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(AssessmentRule));
            CultureInfo culInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<AssetFilterParams>(filterOption);
            if(filter != null)
            {

            }
            return criteria;
        }
        public List<Profile> GetProfile(string filterOption)
        {
            try
            {
                Logger.Info($"About to get profile data from the db");
                var query = BuildQueryProfile(filterOption);
                var record = query.List<AssessmentRule>().OrderBy(a => a.ProfileDescription).Where(b => b.Id > 0 && b.IsDeleted == false);
                var ProfileList = new List<Profile>();
                var Profile = new Profile();
                var ProfileIds = record.Select(x => x.ProfileID).Distinct();
                foreach(var Id in ProfileIds)
                {
                    Profile = new Profile
                    {
                        ProfileName = record.Where(a => a.ProfileID == Id).FirstOrDefault().ProfileDescription,
                        ProfileAmountBilled = record.Where(a => a.ProfileID == Id).Sum(b => b.AssessmentRuleAmount),
                        ProfileAmountSettled = record.Where(a => a.ProfileID == Id).Sum(b => b.SettledAmount)
                    };
                    ProfileList.Add(Profile);
                }
                return ProfileList;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return new List<Profile>();
            }
        }

        private ICriteria BuildQueryProfile(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(AssessmentRule));
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<ProfileFilterParams>(filterOption);
            if(filter != null)
            {

            }
            return criteria;
        }
        public IEnumerable<AssessmentRuleItem> GetAssessmentItem(string filterOption)
        {
            try
            {
                Logger.Info($"about to get data from the AssessmentRuleItem Table");
                var query = BuildQueryAssessmentItem(filterOption);
                IEnumerable<AssessmentRuleItem> records = query.List<AssessmentRuleItem>().OrderBy(a => a.AssessmentItemName).Where(b => b.Id > 0 && b.IsDeleted == false);
                return records;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured inside the GetAssessmentItem(string filterOption) method");
                Logger.Error(ex.Message, ex);
                return new List<AssessmentRuleItem>();
            }
        }
        private ICriteria BuildQueryAssessmentItem(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(AssessmentRuleItem));
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<AssessmentItemFilterParams>(filterOption);
            if (filter != null)
            {

            }
            return criteria;
        }
        public List<ServiceBillResult> GetServiceBill(string filterOption)
        {
            try
            {
                var query = BuildQueryServiceBill(filterOption);
                var filteredData = query.List<ServiceBillResult>().OrderBy(a => a.ServiceBillRefNo).Where(b => b.Id > 0 && b.IsDeleted == false);
                var dataStr = JsonConvert.SerializeObject(filteredData);
                var returnedData  = JsonConvert.DeserializeObject<List<ServiceBillResult>>(dataStr);
                return returnedData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return new List<ServiceBillResult>();
            }
        }
        private ICriteria BuildQueryServiceBill(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(ServiceBillResult));
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<ServiceBillfilterParams>(filterOption);
            if (filter != null)
            {

            }
            return criteria;
        }
        public List<MDAService> GetMDAService(string filterOption)
        {
            try
            {
                var query = BuildQueryMDAService(filterOption);
                var records = query.List<MDAService>().OrderBy(a => a.MDAServiceName).Where(b => b.Id > 0 && b.IsDeleted == false);
                return JsonConvert.DeserializeObject<List<MDAService>>(JsonConvert.SerializeObject(records));
            }
            catch (Exception ex)
            {

                Logger.Error(ex.Message,ex);
                return new List<MDAService>();
            }
        }

        private ICriteria BuildQueryMDAService(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(MDAService));
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<MDAfilter>(filterOption);
            if (filter != null)
            {

            }
            return criteria;
        }

        public List<ServiceBillItem> GetServiceBillItem(string filterOption)
        {
            try
            {
                var query = BuildQueryServiceBillItem(filterOption);
                var records = query.List<ServiceBillItem>().OrderBy(a => a.MDAServiceItemReferenceNo).Where(b => b.Id > 0 && b.IsDeleted == false);
                return JsonConvert.DeserializeObject<List<ServiceBillItem>>(JsonConvert.SerializeObject(records));
            }
            catch (Exception ex)
            {

                Logger.Error(ex.Message, ex);
                return new List<ServiceBillItem>();
            }
        }

        private ICriteria BuildQueryServiceBillItem(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(ServiceBillItem));
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<ServiceItemFilter>(filterOption);
            if (filter != null)
            {

            }
            return criteria;
        }

        public string ReportFilter(int? page, string fromRange, string endRange, string referenceNumber, string paymentChannel, string paymentDate, string TaxPayerRIN, int pageSize, out int pageIndex)
        {
            DateTime startDate = new DateTime(2000, 01, 01);
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(fromRange))
            {
                try
                {
                    startDate = DateTime.Parse(fromRange, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    startDate = new DateTime(2000, 01, 01);
                }

            }
            if (!string.IsNullOrEmpty(endRange))
            {
                try
                {
                    endDate = DateTime.Parse(endRange, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    endDate = DateTime.Now;
                }

            }
            endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //if (startDate > endDate)
            //{
            //    ViewBag.Alert = "The end date cannot be earlier than the start date!!";
            //    return "";
            //}
            var filterData = new FilterParams
            {
                PaymentChannel = paymentChannel?.Trim(),
                ReferenceNumber = referenceNumber?.Trim(),
                StartDate = startDate,
                EndDate = endDate,
                PaymentDate = DateTime.Now,
                RIN = TaxPayerRIN?.Trim()
            };

            return JsonConvert.SerializeObject(filterData);
        }
        
    }
}
