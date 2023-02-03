using CBSPay.Core.APIModels;
using CBSPay.Core.Entities;
using CBSPay.Core.Helpers;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Models;
using CBSPay.Core.Services;
using CBSPay.Core.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CBSPay.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaxPayerService _taxPayerService;
        private readonly IPaymentService _paymentService;
        private readonly IConstantAPIModelService _constantAPIModelService;

        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }

        public HomeController()
        {
            _taxPayerService = new TaxPayerService();
            _paymentService = new PaymentService();
            _constantAPIModelService = new ConstantAPIModelService();

        }
        #region data to be seeded to the db when the site is set up
        /// <summary>
        /// Populates the Tax Payer Table with data from the API
        /// </summary>
        /// <returns></returns>
        public ActionResult SeedTaxPayerTypes()
        {
            _constantAPIModelService.SaveTaxPayersTypeList();

            return View();
        }

        /// <summary>
        /// Populates the Economic Activities Table with data from the API (for all tax payer types)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SeedEconomicActivities(int taxPayerTypeId)
        {
            _constantAPIModelService.SaveEconomicActivities(taxPayerTypeId);

            return View();
        }

        /// <summary>
        /// Populates the Revenue Stream Table from the API
        /// </summary>
        /// <returns></returns>
        public ActionResult SeedRevenueStream()
        {
            _constantAPIModelService.SaveRevenueStreamList();

            return View();
        }

        /// <summary>
        /// Populates the Revenue Sub Stream Table with data from the API
        /// </summary>
        /// <returns></returns>
        public ActionResult SeedRevenueSubStream()
        {
            _constantAPIModelService.SaveRevenueSubStreamList();

            return View();
        }

        #endregion

        #region GET activities
        /// <summary>
        /// EIRS Home Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region TaxPayers Segment
        public ActionResult TaxPayers()
        {
            return View();
        }

        public ActionResult TaxPayerIndividuals()
        {
            return View();
        }

        public ActionResult TaxPayerCompanies()
        {
            return View();
        }

        public ActionResult TaxPayerGovernments()
        {
            return View();
        }

        public ActionResult TaxPayerSpecial()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// Bill Ref Payment Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Pay()
        {
            return View("EIRSBillRef");
        }

        public ActionResult InnerBIllRefDetails()
        {
            Logger.Info("got to the InnerBIllRefDetails Action method!");
            return View();
        }

        /// <summary>
        /// Pay On Account page
        /// </summary>
        /// <returns></returns>
        public ActionResult PAYOnAccount()
        {
            List<SelectListItem> revStreamItems = new List<SelectListItem>();
            var revenueStream = _constantAPIModelService.FetchRevenueStreamList();
            if (revenueStream.Count() > 0)
            {
                foreach (var item in revenueStream)
                {
                    revStreamItems.Add(new SelectListItem { Text = item.RevenueStreamName, Value = item.RevenueStreamName });
                }
            }

            List<SelectListItem> revSubStreamItems = new List<SelectListItem>();
            var revenueSubStream = _constantAPIModelService.FetchRevenueSubStreamList();

            if (revenueSubStream.Count() > 0)
            {
                foreach (var item in revenueSubStream)
                {
                    revSubStreamItems.Add(new SelectListItem { Text = item.RevenueSubStreamName, Value = item.RevenueSubStreamName });
                }
            }

            List<SelectListItem> taxPayerTypes = new List<SelectListItem>();

            ViewBag.RevenueStreamItems = revStreamItems;
            ViewBag.RevenueSubStreamItems = revSubStreamItems;

            return View("EIRSPayOnAccount");
        }
        //public List<SelectListItem> DropdownList<T>(IEnumerable<T> data)
        //{
        //    List<SelectListItem> DropdownModel = new List<SelectListItem>();
        //    var DropdownData = data;
        //    if (DropdownData.Count() > 0)
        //    {
        //        foreach (var item in DropdownData)
        //        {
        //            DropdownModel.Add(new SelectListItem { Text = item.ToString(), Value = item.ToString() });
        //        }
        //    }
        //    return DropdownModel;
        //}
        /// <summary>
        /// NO RIN Capture Page
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterForRIN()
        {
            List<SelectListItem> revStreamItems = new List<SelectListItem>();
            var revenueStream = _constantAPIModelService.FetchRevenueStreamList();
            if (revenueStream.Count() > 0)
            {
                foreach (var item in revenueStream)
                {
                    revStreamItems.Add(new SelectListItem { Text = item.RevenueStreamName, Value = item.RevenueStreamName });
                }

                //revStreamItems.First().Selected = true;
            }

            List<SelectListItem> revSubStreamItems = new List<SelectListItem>();
            var revenueSubStream = _constantAPIModelService.FetchRevenueSubStreamList();

            if (revenueSubStream.Count() > 0)
            {
                foreach (var item in revenueSubStream)
                {
                    revSubStreamItems.Add(new SelectListItem { Text = item.RevenueSubStreamName, Value = item.RevenueSubStreamName });
                }
                //revSubStreamItems.First().Selected = true;
            }

            List<SelectListItem> taxPayerTypes = new List<SelectListItem>();

            var taxPayerTypeList = _constantAPIModelService.FetchTaxPayersTypeList();

            if (taxPayerTypeList.Count() > 0)
            {
                foreach (var item in taxPayerTypeList)
                {
                    taxPayerTypes.Add(new SelectListItem { Text = item.TaxPayerTypeName, Value = item.TaxPayerTypeName });
                }
                //taxPayerTypes.First().Selected = true;
            }


            ViewBag.TaxPayerTypes = taxPayerTypes;
            ViewBag.RevenueStreamItems = revStreamItems;
            ViewBag.RevenueSubStreamItems = revSubStreamItems;

            return View("NoRINCapture");
        }       

        #region Capture/Add New TaxPayers
        public ActionResult IndividualCapture()
        {
            return View();
        }

        public ActionResult CorporateCapture()
        {
            return View();
        }

        public ActionResult SpecialCapture()
        {
            return View();
        }

        public ActionResult GovernmentCapture()
        {
            return View();
        }
        public ActionResult NewGovernmentAdd()
        {
            return View();
        }

        public ActionResult NewCorporateAdd()
        {
            return View();
        }

        public ActionResult NewSpecialAdd()
        {
            return View();
        }

        public ActionResult NewIndividualAdd()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }
        public ActionResult Partnership()
        {
            return View();
        }
        public ActionResult TaxAsset()
        {
            return View();
        }
        public ActionResult TaxPayer()
        {
            return View();
        }
        public ActionResult Business()
        {
            return View();
        }
        public ActionResult PayTaxes()
        {
            return View();
        }
        public ActionResult TaxType()
        {
            return View();
        }
        //public ActionResult EIRSBillRef()
        //{
        //    return View();
        //}
        #endregion

        #region Assets
        public ActionResult CaptureBusinessSearch()
        {
            return View();
        }

        public ActionResult CaptureBuildingSearch()
        {
            return View();
        }

        public ActionResult CaptureLandSearch()
        {
            return View();
        }

        public ActionResult CaptureVehicleSearch()
        {
            return View();
        }

        public ActionResult IndividualBusinessAdd()
        {
            return View();
        }

        public ActionResult IndividualBuildingAdd()
        {
            return View();
        }

        public ActionResult IndividualLandAdd()
        {
            return View();
        }

        public ActionResult IndividualVehicleAdd()
        {
            return View();
        }
        //there needs to be all 4 also for company/business, government, and special
        #endregion

        public ActionResult IndividualTaxPayerDetails()
        {
            return View();
        }
        #region Revenue data
        public ActionResult AssessmentBill()
        {
            return View();
        }
        public ActionResult AssessmentRule()
        {
            return View();
        }
        public ActionResult ServiceBill()
        {
            return View();
        }
        public ActionResult MDAServiceBill()
        {
            return View();
        }
        
        #endregion

        /// <summary>
        /// Fetch  Economic Activities for the particular tax payer type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchEconomicActivities(string taxPayerTypeName)
        {
            POASearchResponse refModel;
            try
            {
                Logger.Info($"Retrieving Economic Activities list for TaxPayer - {taxPayerTypeName}");

                var result = _constantAPIModelService.FetchEconomicActivitiesList(taxPayerTypeName);

                if (result.Count() > 0)
                {
                    refModel = new POASearchResponse
                    {
                        EconomicActivities = result,
                        Status = "Success"
                    };
                    return Json(refModel, JsonRequestBehavior.AllowGet);
                }
                refModel = new POASearchResponse { Status = "Failed" };
                return Json(refModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                refModel = new POASearchResponse { Status = "Failed" };
                Logger.Error(ex.StackTrace, ex);
                return Json(refModel, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region POST activities

        [HttpPost]
        public ActionResult RegisterForRIN(EIRSPaymentRequestInfo model)
        {
            try
            {

                Logger.InfoFormat($"About to Save EIRS Payment Request");
                //if valid, first save the object and create a payment identifier for it    

                var savedInfo = _paymentService.SaveNoRINPOAPaymentRequestInfo(model);
                if (string.IsNullOrWhiteSpace(savedInfo))
                {
                    throw new Exception("An error occurred while saving payment information. Please try again.");
                }
                model.PaymentIdentifier = savedInfo;

                model.PaymentIdentifier = savedInfo;
                var totalAmountPaid = model.TotalAmountToPay;
                var prefix = Configuration.GetInterswitchQuicktellerPrefix;

                var paymentCode = Configuration.CBSPayQuicktellerPaymentCode;
                var redirectUrl = Configuration.GetQuicktellerCBSPayRedirectURL;


                var transfermodel = new TempPaymentRequest
                {
                    Amount = totalAmountPaid * 100, //this is the value I expect to be paid. In lower denomination. (N10 = 1000) that's why i multiply by 100, cos it's in kobo
                    CustomerName = model.TaxPayerName,
                    TransactionReference = model.PaymentIdentifier, // this is a unique reference for this payment or customer. Something to ID the payment
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,
                    QuickTellerRedirectUrl = redirectUrl,
                    PaymentCode = paymentCode, //the merchant’s payment code on the Quickteller platform
                    MerchantRequestReference = prefix + model.PaymentIdentifier  //This is the Merchant’s own Transaction ID generated uniquely for each transaction
                };

                Session["RequestReference"] = transfermodel.MerchantRequestReference;
                Session["AmountPaid"] = transfermodel.Amount;

                // if it is a web payment
                if (model.SettlementMethod == 1)
                {
                    Logger.Debug($"It is a web payment for customer ID - {transfermodel.TransactionReference}");
                    return View("WebPaymentChannelOption", transfermodel);
                }
                //change this to appropraite endpoints when discussed with project manager - Esther
                return RedirectToAction("ProceedToNetPay");


            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process payment EIRSPayment Request");
                Logger.Error(ex.StackTrace, ex);

                ViewBag.ResponseCode = "505";
                ViewBag.ErrorMessage = ex.Message;

                return View("ErrorPage");
            }
        }

        [HttpPost]
        [Route("home/paywithreference")]
        public ActionResult MakeOnlineEIRSPayment(EIRSPaymentRequestInfo model, IEnumerable<RefRule> RefRules)
        {
            try
            {
                if (model.RefRules.Count < 1)
                {
                    throw new Exception("An Error occurred processing request. No reference items attached");
                }

                if (model.SettlementMethod < 1)
                {
                    throw new Exception("Settlement method cannot be null. Please select the settlement method");
                }
                //get the proposed settlement method

                Logger.InfoFormat($"About to Save EIRS Payment Request");
                //if valid, first save the object and create a payment identifier for it    

                var savedInfo = _paymentService.SaveEIRSBillRefPaymentRequestInfo(model);

                if (string.IsNullOrWhiteSpace(savedInfo))
                {
                    throw new Exception("An error occurred while saving payment information. Please try again.");
                }

                model.PaymentIdentifier = savedInfo;
                //var totalAmountPaid = model.RefRules.Sum(item => Convert.ToDecimal(item.RuleAmountToPay));
                var totalAmountPaid = model.TotalAmountToPay;
                var prefix = Configuration.GetInterswitchQuicktellerPrefix;
                //prefix = prefix ?? throw new Exception("An Error occurred processing request. prefix not configured");
                // prefix = prefix;

                //var paymentCode = Configuration.CBSPayQuicktellerPaymentCode ?? throw new Exception("An Error occurred processing request. Payment Code not configured");
                var paymentCode = Configuration.CBSPayQuicktellerPaymentCode;
                //var redirectUrl = Configuration.GetQuicktellerCBSPayRedirectURL ?? throw new Exception("An Error occurred processing request. RedirectUrl not configured");
                var redirectUrl = Configuration.GetQuicktellerCBSPayRedirectURL;


                var transfermodel = new TempPaymentRequest
                {
                    Amount = totalAmountPaid * 100, //this is the value I expect to be paid. In lower denomination. (N10 = 1000) that's why i multiply by 100, cos it's in kobo
                    CustomerName = model.TaxPayerName,
                    TransactionReference = model.PaymentIdentifier, // this is a unique reference for this payment or customer. Something to ID the payment
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,
                    QuickTellerRedirectUrl = redirectUrl,
                    PaymentCode = paymentCode, //the merchant’s payment code on the Quickteller platform
                    MerchantRequestReference = prefix + model.PaymentIdentifier  //This is the Merchant’s own Transaction ID generated uniquely for each transaction
                };

                Session["RequestReference"] = transfermodel.MerchantRequestReference;
                Session["AmountPaid"] = transfermodel.Amount;
                // if it is a web payment
                if (model.SettlementMethod == 1)
                {
                    Logger.Debug($"It is a web payment for customer ID - {transfermodel.TransactionReference}");
                    Logger.Debug("Redirecting to WebPaymentChannelOption page");
                    return View("WebPaymentChannelOption", transfermodel);
                }
                return RedirectToAction("ProceedToNetPay");
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process payment EIRSPayment Request");
                Logger.Error(ex.StackTrace, ex);

                ViewBag.ResponseCode = "505";
                ViewBag.ErrorMessage = ex.Message;

                return View("ErrorPage");
            }
        }

        [HttpPost]
        [Route("home/getreferenceitems")]
        public ActionResult GetReferenceItems(string ReferenceNumber, string phoneNumber)
        {
            var refModel = new InnerBillDetailsViewModel();
            try
            {
                Logger.InfoFormat($"About to get reference Items");

                //check if it is Assessment or Service Bill
                var rNo = ReferenceNumber.Trim();
                var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();
                
                switch (firstTwoChars)
                {
                    case "AB":
                        var assessment = _taxPayerService.ProcessAssessmentDetails(rNo);
                        Logger.Debug("Assessment returned to the controller; check if it is null");
                        Logger.Debug("Assessment returned to the controller; check if it is null");
                        if (assessment != null && assessment.AssessmentRuleItems.Count() > 0)
                        {
                            Logger.Debug("Assessment returned to the controller is not null");
                            //var TaxPayerInfo = _taxPayerService.RetrieveTaxPayerInfo(assessment.TaxPayerRIN, phoneNumber);
                            refModel = new InnerBillDetailsViewModel
                            {
                                TemplateType = "Assessment",
                                Date = Convert.ToDateTime(assessment.AssessmentDate).Date.Date,
                                TaxPayerType = assessment.TaxPayerTypeName, //may remove TaxPayerType, Notes and TaxPayerTypeID
                                TaxPayerName = assessment.TaxPayerName,
                                SettlementStatus = assessment.SettlementStatusName,
                                Notes = assessment.AssessmentNotes,
                                RefNumber = rNo,
                                TotalAmount = assessment.AssessmentAmount,//The total amount of money that should be paid
                                PhoneNumber = phoneNumber,
                                TaxPayerID = assessment.TaxPayerID,
                                TaxPayerTypeID = assessment.TaxPayerTypeID,
                                TaxPayerRIN = assessment.TaxPayerRIN,
                                Address = "",//TaxPayerInfo.TaxPayerAddress,
                                ReferenceId = assessment.AssessmentID,
                                //Status = "Success",
                                RuleItems = assessment.AssessmentRuleItems.Select(x => new RefItem
                                {
                                    ItemRef = x.AssessmentItemReferenceNo,
                                    RefRuleID = x.AARID,
                                    Computation = x.ComputationName,
                                    PendingAmount = x.PendingAmount,
                                    AmountPaid = x.SettlementAmount,
                                    TotalAmount = x.TaxAmount,
                                    ItemName = x.AssessmentItemName,
                                    ItemID = x.AssessmentItemID.ToString()
                                }).ToList(),
                                RefRules = assessment.AssessmentRules.Select(
                                    x => new RefRule
                                    {
                                        RuleAmount = x.AssessmentRuleAmount == null ? 0 : x.AssessmentRuleAmount.Value,
                                        RefRuleID = x.AARID,
                                        RuleID = x.AssessmentRuleID,
                                        RuleName = x.AssessmentRuleName,
                                        SettledAmount = x.SettledAmount == null ? 0 : x.SettledAmount.Value,
                                        OutstandingAmount = Math.Round((x.AssessmentRuleAmount == null ? 0 : x.AssessmentRuleAmount.Value) - (x.SettledAmount == null ? 0 : x.SettledAmount.Value), MidpointRounding.ToEven),
                                        TaxYear = x.TaxYear,
                                        TBPKID = assessment.AssessmentRuleItems.Where(y => y.AARID == x.AARID).FirstOrDefault().AAIID
                                    }).ToList(),
                            };
                            var tAmountPaid = refModel.RefRules.Sum(x => x.SettledAmount);
                            var tOutstandingAmount = refModel.RefRules.Sum(x => x.OutstandingAmount);
                            refModel.TotalOutstandingAmount = Math.Round(tOutstandingAmount, MidpointRounding.ToEven);
                            refModel.TotalAmountPaid = Math.Round(tAmountPaid, MidpointRounding.ToEven);

                            foreach (var item in refModel.RuleItems)
                            {
                                var itemToChange = refModel.RefRules.FirstOrDefault(d => d.RefRuleID == item.RefRuleID);
                                if (itemToChange != null)
                                    itemToChange.RuleItemID = item.ItemID;
                                itemToChange.RuleItemName = item.ItemName;
                                itemToChange.RuleItemRef = item.ItemRef;
                                itemToChange.RuleComputation = item.Computation;
                            }
                            Logger.Info("got the refModel");
                            return View("InnerBillRefDetails", refModel);
                        }
                        //include an error message
                        Logger.Error("Assessment details is null, hence not successfully processed");
                        ViewBag.Message = "An error occurred, could not retrieve assessment details";
                        return View("EIRSBillRef");
                    case "SB":
                        var serviceBill = _taxPayerService.ProcessServiceBill(rNo);
                        Logger.Debug("Service bill returned to the controller; check if it is null");
                        if (serviceBill != null && serviceBill.ServiceBillItems.Count() > 0)
                        {
                            Logger.Debug("Service bill returned to the controller is not null");
                            //var TaxPayerInfo = _taxPayerService.RetrieveTaxPayerInfo(serviceBill.TaxpayerRIN, phoneNumber);
                            refModel = new InnerBillDetailsViewModel
                            {
                                TemplateType = "Service Bill",
                                Date = Convert.ToDateTime(serviceBill.ServiceBillDate).Date,
                                TaxPayerType = "",//TaxPayerInfo.TaxPayerTypeName, //may remove TaxPayerType,address, Notes and TaxPayerTypeID
                                TaxPayerName = serviceBill.TaxPayerName,
                                SettlementStatus = serviceBill.SettlementStatusName,
                                Notes = "Online payment by "+serviceBill.TaxPayerName,
                                RefNumber = rNo,
                                TotalAmount = serviceBill.ServiceBillAmount,//The total amount of money that should be paid
                                PhoneNumber = phoneNumber,
                                TaxPayerID = serviceBill.TaxPayerID,
                                TaxPayerTypeID = 0,//TaxPayerInfo.TaxPayerTypeID,
                                TaxPayerRIN = serviceBill.TaxpayerRIN,
                                Address = "",//TaxPayerInfo.TaxPayerAddress,
                                ReferenceId = serviceBill.ServiceBillID,
                                //Status = "Success",
                                RuleItems = serviceBill.ServiceBillItems.Select(x => new RefItem
                                {
                                    ItemRef = x.MDAServiceItemReferenceNo,
                                    Computation = x.ComputationName,
                                    PendingAmount = Convert.ToDecimal(x.PendingAmount),
                                    AmountPaid = Convert.ToDecimal(x.SettlementAmount),
                                    TotalAmount = Convert.ToDecimal(x.ServiceAmount),
                                    ItemName = x.MDAServiceItemName,
                                    ItemID = x.MDAServiceItemID.ToString()
                                }).ToList(),
                                RefRules = serviceBill.MDAServiceRules.Select(
                                        x => new RefRule
                                        {
                                            RuleAmount = Convert.ToDecimal(x.ServiceAmount),
                                            RefRuleID = x.SBSID,
                                            RuleID = x.MDAServiceID,
                                            RuleName = x.MDAServiceName,
                                            SettledAmount = Convert.ToDecimal(x.SettledAmount),
                                            OutstandingAmount = Math.Round(Convert.ToDecimal((x.ServiceAmount)) - Convert.ToDecimal((x.SettledAmount)), MidpointRounding.ToEven),
                                            TaxYear = x.TaxYear,
                                            TBPKID = serviceBill.ServiceBillItems.Where(y => y.MDAServiceID == x.MDAServiceID).FirstOrDefault().SBSIID
                                        }).ToList()

                            };
                            var tAmountPaid = refModel.RefRules.Sum(x => x.SettledAmount);
                            var tOutstandingAmount = refModel.RefRules.Sum(x => x.OutstandingAmount);
                            refModel.TotalOutstandingAmount = Math.Round(tOutstandingAmount, MidpointRounding.ToEven);
                            refModel.TotalAmountPaid = Math.Round(tAmountPaid, MidpointRounding.ToEven);
                            refModel.PhoneNumber = phoneNumber;
                            return View("InnerBillRefDetails", refModel);
                        }
                        //include an error message
                        Logger.Error("service bill details is null, hence not successfully processed");
                        ViewBag.Message = "An error occurred, could not retrieve service bill details";
                        return View("EIRSBillRef");
                    default:
                        //include an error message
                        ViewBag.Message = "An error occurred, invalid reference number";
                        Logger.Error($"invalid reference number - {ReferenceNumber}");
                        return View("EIRSBillRef");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                Logger.Error("An error occurred, please try again");
                //include an error message
                ViewBag.Message = "An error occurred, please try again";
                return View("EIRSBillRef");
            }
        }

        [HttpPost]
        [Route("home/VerifyTaxPayer")]
        public JsonResult VerifyTaxPayer(ValidateRin model)
        {
            POASearchResponse refModel;
            try
            {
                Logger.Info($"Validating web request for online eirs pay on account with {model.ClientId}, {model.ClientSecret}");
                if (WebRequestValidationService.IsValidRequest(model.ClientId, model.ClientSecret))
                {
                    Logger.InfoFormat($"About to retrieve TaxPayerData");

                    //check if it is Assessment or Service Bill
                    if (string.IsNullOrWhiteSpace(model.TaxPayerPOASearchOption) || string.IsNullOrWhiteSpace(model.POASearchOptionValue))
                    {
                        refModel = new POASearchResponse { Status = "Failed" };
                        return Json(refModel, JsonRequestBehavior.DenyGet);
                    }
                    var POAName = model.TaxPayerPOASearchOption.Trim();
                    var searchValue = model.POASearchOptionValue.Trim();

                    IEnumerable<POATaxPayerResponse> taxPayerDetails = new List<POATaxPayerResponse>();
                    switch (POAName)
                    {
                        case "BusinessName":
                            Logger.Debug($"it is a business name - {model.POASearchOptionValue}");
                            taxPayerDetails = _taxPayerService.RetrieveTaxPayerInfoByBusinessName(searchValue);
                            break;
                        case "RIN":
                            Logger.Debug($"it is a RIN - {model.POASearchOptionValue}");
                            taxPayerDetails = _taxPayerService.RetrieveTaxPayerInfoByRIN(searchValue);
                            break;
                        case "MobileNumber":
                            Logger.Debug($"it is a mobile number - {model.POASearchOptionValue}");
                            taxPayerDetails = _taxPayerService.RetrieveTaxPayerInfoByMobileNumber(searchValue);
                            break;
                    }

                    if (taxPayerDetails.Count() > 0)
                    {

                        refModel = new POASearchResponse
                        {
                            TaxPayerResponses = taxPayerDetails,
                            Status = "Success"
                        };
                        return Json(refModel, JsonRequestBehavior.DenyGet);
                    }
                    refModel = new POASearchResponse { Status = "Failed" };
                    return Json(refModel, JsonRequestBehavior.DenyGet);

                }
                else
                {
                    //throw new Exception("An Error occurred processing request. UnRecognized Client Credentials");
                    refModel = new POASearchResponse { Status = "Failed" };
                    return Json(refModel, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                refModel = new POASearchResponse { Status = "Failed" };
                Logger.Error(ex.StackTrace, ex);
                return Json(refModel, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [Route("home/EIRSPayOnAccount")]
        public ActionResult MakeOnlineEIRSPaymentOnAccount(EIRSPaymentRequestInfo model)
        {
            try
            {
                Logger.InfoFormat($"About to Save EIRS Payment Request");
                //if valid, first save the object and create a payment identifier for it    

                var savedInfo = _paymentService.SaveEIRSPOAPaymentRequestInfo(model);
                if (string.IsNullOrWhiteSpace(savedInfo))
                {
                    throw new Exception("An error occurred while saving payment information. Please try again.");
                }

                model.PaymentIdentifier = savedInfo;
                var totalAmountPaid = model.TotalAmountToPay;
                var prefix = Configuration.GetInterswitchQuicktellerPrefix;

                var paymentCode = Configuration.CBSPayQuicktellerPaymentCode;
                var redirectUrl = Configuration.GetQuicktellerCBSPayRedirectURL;


                var transfermodel = new TempPaymentRequest
                {
                    Amount = totalAmountPaid * 100, //this is the value I expect to be paid. In lower denomination. (N10 = 1000) that's why i multiply by 100, cos it's in kobo
                    CustomerName = model.TaxPayerName,
                    TransactionReference = model.PaymentIdentifier, // this is a unique reference for this payment or customer. Something to ID the payment
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,
                    QuickTellerRedirectUrl = redirectUrl,
                    PaymentCode = paymentCode, //the merchant’s payment code on the Quickteller platform
                    MerchantRequestReference = prefix + model.PaymentIdentifier  //This is the Merchant’s own Transaction ID generated uniquely for each transaction
                };

                Session["RequestReference"] = transfermodel.MerchantRequestReference;
                Session["AmountPaid"] = transfermodel.Amount;
                // if it is a web payment
                //if (model.SettlementMethod == 1)
                //{
                //    Logger.Debug($"It is a web payment for customer ID - {transfermodel.TransactionReference}");
                //    return View("WebPaymentChannelOption", transfermodel);
                //}
                return View("WebPaymentChannelOption", transfermodel);
                //change this to appropraite endpoints when discussed with project manager - Esther
                //return RedirectToAction("ProceedToNetPay");


            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process payment EIRSPayment Request");
                Logger.Error(ex.StackTrace, ex);

                ViewBag.ResponseCode = "505";
                ViewBag.ErrorMessage = ex.Message;

                return View("ErrorPage");
            }
        }

        #endregion

        #region not in use so much,if at all  lol
        public ActionResult ProceedToNetPay()
        {
            
            Logger.Info($"About to proceed to Netpay");

            string MerchantSecretKey = Configuration.MerchantSecret;
            string MerchantUniqueId = Configuration.MerchantId;

            TempPaymentRequest temppaymentRequest = TempData["TempPaymentRequest"] as TempPaymentRequest;
            NetPayRequestModel modelToConvert = new NetPayRequestModel()
            {
                MerchantUniqueId = MerchantUniqueId,
                ReturnUrl = Configuration.NetPayReturnUrl,
                Amount = temppaymentRequest.Amount,
                Description = temppaymentRequest.Description,
                TransactionReference = temppaymentRequest.TransactionReference,
                Currency = "NGN",
                HMAC = string.Empty
            };

            var requestDictionary = modelToConvert.ToDictionary().OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);

            string concatenatedValues = string.Join("", requestDictionary.Select(kvp => string.Format("{0}", kvp.Value)));

            string HMAC = Utils.ComputeHMAC(concatenatedValues, MerchantSecretKey);

            Logger.Info($"HMAC generated successfully {HMAC}");

            var viewModel = new NetPayPaymentRequestModel()
            {
                ReturnUrl = Configuration.NetPayReturnUrl,
                HMAC = HMAC,
                Amount = modelToConvert.Amount,
                Currency = "NGN",
                CustomerName = temppaymentRequest.CustomerName,
                Description = modelToConvert.Description,
                MerchantUniqueId = MerchantUniqueId,
                TransactionReference = modelToConvert.TransactionReference,
                FormUrl = Configuration.NetPayPaymentUrl
            };

            return View("ProceedToNetPay", viewModel);
        }

        private APIResponse BadModelRequest()
        {
            var requestDictionary = ModelState.OrderBy(x => x.Key).ToList().Select(c => new
            {
                Key = c.Key.Substring(6),
                Value = string.Join(",", c.Value.Errors.Select(x => x.ErrorMessage).ToList())
            });

            string concatenatedValues = string.Join(";", requestDictionary.Select(kvp => string.Format("{0}", kvp.Value)));

            Logger.Error($"Request Model is Invalid. See Error Details {concatenatedValues}");

            return new APIResponse { ErrorMessage = concatenatedValues, StatusCode = HttpStatusCode.BadRequest, Success = false, Result = null };
        }

        #endregion
    }
}
