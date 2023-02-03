using CBSPay.Core.APIModels;
using CBSPay.Core.Helpers;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Services;
using CBSPay.Core.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace CBSPay.API.Controllers
{
    public class PaymentNotificationController : Controller
    {
        private readonly ITaxPayerService _taxPayerService;
        private readonly IPaymentService _paymentService;

        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }

        public PaymentNotificationController()
        {
            _taxPayerService = new TaxPayerService();
            _paymentService = new PaymentService();
        }

        [HttpPost]
        [Route("")]
        public ActionResult Index()
        {
             
            return View();
        }
        [HttpPost]
        [Route("api/notifypayment")]
        public ActionResult NotifyPayment(NetPayPaymentNotificationModel model)
        {
            PaymentStatusModel response;
            if (model.ResponseCode == "0000")
            {
                try
                {
                    Logger.InfoFormat($"About to update payment Request for transactionReference {model.MerchantTransactionReference}");

                    response = _paymentService.ProcessWebPaymentNotification(model.MerchantTransactionReference, model.TransactionAmount, "");
                    //response = _paymentService.TestProcessNetPayPaymentNotification(model.MerchantTransactionReference, model.TransactionAmount);
                    return View(response);
                }
                catch (Exception ex)
                {
                    Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process payment EIRSPayment Request");
                    Logger.Error(ex.StackTrace, ex);
                    return View("ErrorPaymentStatus", new PaymentStatusModel { Success = false, Message = ex.Message});
                }

            }
            else {
                return View("ErrorPaymentStatus");
            }
             
        }

        //public ActionResult QTPaymentNotificationOld(PayWithQuicktellerResponse responseModel)
        //{
        //    PaymentStatusModel response = null;
        //    if (responseModel.resp_code == "00")
        //    {
        //        var baseUrl = Configuration.GetQuicktellerClientId;
        //        ViewBag.HomeUrl = baseUrl;
        //        var details = new List<KeyValuePair<string, object>>();
        //        decimal amountPaid = 0;
        //        decimal amtPaid = 0;
        //        try
        //        {
        //            Logger.Debug($"Successfully made payment via QuickTeller; transaction reference - {responseModel.tx_ref}, response code - {responseModel.resp_code}, response description - {responseModel.resp_desc}");
        //            Logger.Info($"About to update payment Request for transactionReference {responseModel.tx_ref}");

        //            //get details of transaction via api
        //            //var transRef = responseModel.tx_ref ?? throw new Exception("An Error occurred processing, txref is empty");
        //            var transRef = responseModel.tx_ref;

        //            var requestReference = Session["RequestReference"] as string;
        //            var amount = Session["AmountPaid"];
        //            var AmountKobo = Convert.ToDecimal(amount);
        //            var AmountNaira = Math.Round(AmountKobo / 100, MidpointRounding.ToEven);

        //            Logger.Debug("Call the call back API URL to get full details of transaction");
        //             details = _paymentService.QuickTellerPaymentTransactionDetails(transRef, requestReference);

                    
        //            if (details != null)
        //            {
        //                Logger.Debug("Transaction Details returned successfully");
        //                amtPaid = 0;//details.Amount;
        //                 amountPaid = Math.Round(amtPaid / 100, MidpointRounding.ToEven);

        //                var paymentRef = requestReference.Substring(4);

        //                Logger.Debug("Confirm that the amount paid is the same as the amount sent");

        //                    if (AmountNaira == amountPaid)
        //                    {
        //                    Logger.Debug("The amounts are the same");
        //                        response = _paymentService.ProcessWebPaymentNotification(paymentRef, amountPaid);
        //                    ViewBag.AmountPaid = amountPaid;
        //                        return View(response);
        //                    }

        //                    //if not the same, confirm from PM what to do
        //            }
        //            else
        //            {
        //                Logger.Error(" could not retrieve transaction details from the api, try again");
        //                Logger.Debug("Call the call back API URL again to get full details of transaction");

        //                details = _paymentService.QuickTellerPaymentTransactionDetails(transRef, requestReference);
        //                if (details != null)
        //                {
        //                    Logger.Debug("Transaction Details returned successfully");
        //                    amtPaid = 0;//details.Amount;
        //                    amountPaid = Math.Round(amtPaid / 100, MidpointRounding.ToEven); //dividing by 100 because it is returned in Kobo and has to be converted to naira

        //                    Logger.Debug("Confirm that the amount paid is the same as the amount sent");

        //                    if (AmountNaira == amountPaid)
        //                    {
        //                        Logger.Debug("The amounts are the same");
        //                        response = _paymentService.ProcessWebPaymentNotification(requestReference, amountPaid);
        //                        ViewBag.AmountPaid = amountPaid;
        //                        return View(response);
        //                    }
        //                    //if not the same, confirm from PM what to do
        //                }
        //                Logger.Error("Could not still confirm details from the Callback API URL");
        //            }
        //            //confirm that you r doing the right thing here
        //            return View();
                    
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process payment Payment Request");
        //            Logger.Error(ex.StackTrace, ex);
        //            return View("ErrorPaymentStatus", new PaymentStatusModel { Success = false, Message = ex.Message });
        //        }

        //    }
        //    else
        //    {
        //        Logger.Error($"Could not process payment for transcation ref - {responseModel.tx_ref}, due to error {responseModel.resp_code} - {responseModel.resp_desc}");
        //        return View("ErrorPaymentStatus", new PaymentStatusModel { Success = false, Message = responseModel.resp_desc });
        //    }
        //}

        /// <summary>
        /// Redirect Url After Successful Payment on QuickTellers page
        /// </summary>
        /// <param name="responseModel"></param>
        /// <returns></returns>
        public ActionResult QTPaymentNotification(PayWithQuicktellerResponse responseModel)
        {
            PaymentStatusModel response = null;
            if (responseModel.resp_code == "00")
            {
                var baseUrl = Configuration.GetQuicktellerClientId;
                ViewBag.HomeUrl = baseUrl;
                try
                {
                    var responseModelStr = JsonConvert.SerializeObject(responseModel);
                    Logger.Debug($"Successfully made payment via QuickTeller; transaction reference - {responseModel.tx_ref}, response code - {responseModel.resp_code}, response description - {responseModel.resp_desc}");
                    Logger.Info($"About to update payment Request for transactionReference {responseModel.tx_ref}");

                    var transRef = responseModel.tx_ref;
                    var requestReference = Session["RequestReference"] as string;
                    ViewBag.requestReference = requestReference;
                    var amountKobo = Convert.ToDecimal(Session["AmountPaid"]);
                    var amountNaira = Math.Round(amountKobo / 100, MidpointRounding.ToEven);
                    Logger.Debug("Call the call back API URL to get full details of transaction");
                    var paymentRef = requestReference.Substring(4);
                    Session["ReceiptPaymentRef"] = paymentRef;
                    response = _paymentService.ProcessWebPaymentNotification(paymentRef, amountNaira, responseModelStr);
                    ViewBag.AmountPaid = amountNaira;
                    return View(response);
                }
                catch (Exception ex)
                {
                    Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process payment Payment Request");
                    Logger.Error(ex.StackTrace, ex);
                    return View("ErrorPaymentStatus", new PaymentStatusModel { Success = false, Message = ex.Message });
                }
            }
            else
            {
                Logger.Error($"Could not process payment for transaction ref - {responseModel.tx_ref}, due to error {responseModel.resp_code} - {responseModel.resp_desc}");
                return View("ErrorPaymentStatus", new PaymentStatusModel { Success = false, Message = responseModel.resp_desc });
            }
        }

        public byte[] GenerateReceipt(string transactionRefNo)
        {
            Logger.Info("Now within the GenerateReceipt Action method!");
            try
            {
                var record = _paymentService.GetSettlementReportDetails(transactionRefNo);
                string[] array = new string[]
                {
                };
                byte[] data;
                //if(!System.IO.File.Exists(Server.MapPath("~/Files/Receipt/pdf/Receipt" + transactionRefNo + ".pdf")))
                //{
                //    string templatePath = Server.MapPath("~/Files/Receipt/Template/Receipt.cshtml");
                //    string ResourcePath = Server.MapPath("");
                //    data = new ReportService().GeneratePdf(array, templatePath, record, ResourcePath);
                //    //System.IO.File.WriteAllBytes(Server.MapPath("~/Files/Receipt/pdf/Receipt"+transactionRefNo+".pdf"), data);
                //    //var Writer = new BinaryWriter(System.IO.File.OpenWrite(Server.MapPath("~/Files/Receipt/pdf/Receipt" + transactionRefNo + ".pdf")));
                //    //Writer.Write(data);
                //    //Writer.Flush();
                //    //Writer.Close();
                //}
                string templatePath = Server.MapPath("~/Files/Receipt/Template/Receipt.cshtml");
                string ResourcePath = Server.MapPath("");
                data = new ReportService().GeneratePdf(array, templatePath, record, ResourcePath);
                Logger.Info("GenerateReceipt Action method completed!");
                return data;//"/Files/Receipt/pdf/Receipt" + transactionRefNo + ".pdf";
            }
            catch (Exception exception)
            {
                Logger.Error(exception.StackTrace, exception);
                throw;
            }
        }

        public ActionResult Receipt(string pref = "")
        {
            Logger.Info("Now within the receipt page.");
            try
            {
                var PayRef = Session["ReceiptPaymentRef"] as string;
                Logger.Info($"the PaymentRef (from the session) is {PayRef}");
                var PaymentRef = pref == "" ? PayRef : pref;
                Logger.Info($"the PaymentRef is {PaymentRef}");
                EIRSSettlementInfo record = _paymentService.GetSettlementReportDetails(PaymentRef);
                Logger.Info($"the payment details for the payment with PaymentRef: {PayRef} is {record}");
                return View(record);
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to show the receipt!");
                Logger.ErrorFormat(ex.Message, ex);
                ViewBag.Error = ex.Message;
                return View(new EIRSSettlementInfo());
            }
        }
        // Get action method that tries to show a PDF file in the browser (inline)
        public ActionResult ShowPdfInBrowser(string pref = "")
        {
            Logger.Info("now within the ShowPdfInBrowser Action method");
            try
            {
                var PayRef = Session["ReceiptPaymentRef"] as string;
                var PaymentRef = pref == "" ? PayRef : pref;
                byte[] pdfContent = GenerateReceipt(PaymentRef);
                if (pdfContent == null)
                {
                    return null;
                }
                var contentDispositionHeader = new System.Net.Mime.ContentDisposition
                {
                    Inline = true,
                    FileName = "receipt.pdf"
                };
                Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
                Logger.Info("ShowPdfInBrowser Action method completed");
                return File(pdfContent, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to download the receipt!");
                Logger.ErrorFormat(ex.Message, ex);
                ViewBag.Error = ex.Message;
                return View("Receipt", new { pref });
            }
            
        }
    }
}