using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System;
using Parkway.CBS.Core.HelperModels;
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.ThirdParty.Payment.Processor.Models;
using Parkway.ThirdParty.Payment.Processor.Processors.PayDirect;
using Parkway.CBS.Core.Utilities;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using System.Collections.Generic;
using System.Net.Http;
using Parkway.CBS.Core.Notifications.Contracts;
using Parkway.CBS.Core.StateConfig;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Parkway.CBS.CacheProvider;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CorePaymentService : ICorePaymentService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionLogManager<TransactionLog> _transactionLogManager;
        private readonly IInvoiceManager<Invoice> _invoiceManager;
        private IPayDirect _payDirect;
        private readonly IWebPaymentRequestManager<WebPaymentRequest> _webPayRequestManager;
        private readonly IRemoteClient _remoteClient;
        private readonly IPaymentNotifications _paymentNotifications;
        private readonly ICoreReceiptService _coreReceiptService;
        private readonly IPaymentReferenceManager<PaymentReference> _paymentReferenceManager;
        private readonly IEnumerable<Lazy<ISMSProvider>> _smsProvider;
        private readonly Lazy<ICorePAYEPaymentService> _corePAYEPaymentService;
        private readonly IActivityPermissionManager<ActivityPermission> _activityPermissionRepo;
        public ILogger Logger { get; set; }

        public CorePaymentService(IOrchardServices orchardServices, ITransactionLogManager<TransactionLog> transactionLogManager, IInvoiceManager<Invoice> invoiceManager, IPaymentNotifications paymentNotifications, ICoreReceiptService coreReceiptService, IPaymentReferenceManager<PaymentReference> paymentReferenceManager, IEnumerable<Lazy<ISMSProvider>> smsProvider, Lazy<ICorePAYEPaymentService> corePAYEPaymentService, IActivityPermissionManager<ActivityPermission> activityPermissionRepo)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _transactionLogManager = transactionLogManager;
            _invoiceManager = invoiceManager;
            _coreReceiptService = coreReceiptService;
            _paymentNotifications = paymentNotifications;
            _paymentReferenceManager = paymentReferenceManager;
            _smsProvider = smsProvider;
            _corePAYEPaymentService = corePAYEPaymentService;
            _activityPermissionRepo = activityPermissionRepo;
        }


        /// <summary>
        /// Payment Reversal for pay direct
        /// </summary>
        /// <param name="transactionLogVM"></param>
        /// <param name="payDirect"></param>
        /// <param name="originalPaymentReference"></param>
        /// <param name="originalPaymentLogId"></param>
        /// <returns>InvoiceValidationResponseModel</returns>
        public PaymentReversalResponseObj PaymentReversalForPayDirect(TransactionLogVM transactionLogVM, string originalPaymentReference, string originalPaymentLogId)
        {
            try
            {
                InvoiceDetailsHelperModel helperModel = _invoiceManager.GetInvoiceDetails(transactionLogVM.InvoiceNumber);

                if (helperModel == null)
                {
                    Logger.Information(string.Format("PAY Direct REV ::: invoice number not found {0}", transactionLogVM.InvoiceNumber));

                    return new PaymentReversalResponseObj
                    {
                        HasError = true,
                        ErrorMessage = ErrorLang.invoice404().ToString()
                    };
                }

                //check for payment log Id
                //here we are chekcing for the transaction, to more or less see if we have already processed this reversal
                TransactionLogGroup reversalPaymentByLogIdGrp = _transactionLogManager.GetTransactionForPaydirectReversal(transactionLogVM.PaymentLogId);

                if (reversalPaymentByLogIdGrp != null)
                {
                    //here we check if the gotten transaction has the same original payment and ref from the request
                    bool badPaymentRequest = !string.Equals(reversalPaymentByLogIdGrp.OriginalPaymentLogID, originalPaymentLogId, StringComparison.OrdinalIgnoreCase);
                    bool badRefRequest = !string.Equals(reversalPaymentByLogIdGrp.OriginalPaymentReference, originalPaymentReference, StringComparison.OrdinalIgnoreCase);

                    if (badPaymentRequest || badRefRequest)
                    {
                        Logger.Information(string.Format("PAY Direct REV ::: a record with payment log id was found but the revered params donot match either the originalref or orginal log id. PaymentId: {0}, OrigLogId: {1} OrigRef: {2}", reversalPaymentByLogIdGrp.PaymentLogId, originalPaymentLogId, originalPaymentReference));

                        return new PaymentReversalResponseObj
                        {
                            HasError = true,
                            ErrorMessage = "Mismatch between transaction and original values"
                        };
                    }

                    //check that amount tally
                    if (reversalPaymentByLogIdGrp.TotalAmountPaid != transactionLogVM.AmountPaid)
                    {
                        Logger.Error("PAY DIRECT REV ::: The amount paid is a mismatch to the existing payment log record. Supplied amount PaymentLogId" + reversalPaymentByLogIdGrp.PaymentLogId);

                        return new PaymentReversalResponseObj
                        {
                            HasError = true,
                            ErrorMessage = ErrorLang.amountmismatchforexistingpaymentlogid().ToString()
                        };
                    }

                    //check payment ref tally
                    if (reversalPaymentByLogIdGrp.PaymentReference != transactionLogVM.PaymentReference)
                    {
                        Logger.Error("PAY DIRECT NOTIF: payment ref mismacth ref " + reversalPaymentByLogIdGrp.PaymentReference);

                        return new PaymentReversalResponseObj
                        {
                            HasError = true,
                            ErrorMessage = ErrorLang.paymentrefmismatchforexistingpaymentid().ToString()
                        };
                    }

                    //do check here
                    //we are checking that the record for paymentref was for the same invoice and the same tax profile
                    if (reversalPaymentByLogIdGrp.InvoiceId == helperModel.Invoice.Id && reversalPaymentByLogIdGrp.TaxEntityId == helperModel.TaxEntityId)
                    {
                        Logger.Information("Pay Direct ::: Payment log Id has already been processed. Log Id: " + transactionLogVM.PaymentLogId);
                        return new PaymentReversalResponseObj
                        {
                            ErrorMessage = Lang.Lang.paymentnotificationalreadyprocess.ToString()
                        };
                    }
                    else
                    {
                        Logger.Error("PAY DIRECT REV ::: " + string.Format("The invoice associated with the duplicate payment log id {0} provider - {1} does not match the invoice {2}", transactionLogVM.PaymentLogId, PaymentProvider.PayDirect.ToString(), transactionLogVM.InvoiceNumber));
                        return new PaymentReversalResponseObj
                        {
                            HasError = true,
                            ErrorMessage = ErrorLang.paymentrefmismatchforexistingpaymentid().ToString()
                        };
                    }
                }

                //first we need to get the payment log by payment log Id
                //payment log ref is tored in the old payment log Id
                TransactionLogGroup initialCreditTransaction = _transactionLogManager.GetGroupedTransactionLogByPaymentLogIdWithReversal(originalPaymentLogId, PaymentProvider.PayDirect);

                if (initialCreditTransaction == null)
                {
                    Logger.Error(string.Format("PAY Direct Rev ::: not found for payment log Id {0} ", originalPaymentLogId));
                    return new PaymentReversalResponseObj
                    {
                        HasError = true,
                        ErrorMessage = "Transaction log with transaction payment log Id could not be found " + originalPaymentLogId
                    };
                }

                //now we check if the initial credit transaction has been reversed before
                if (initialCreditTransaction.IsReversed)
                {
                    Logger.Error(string.Format("PAY Direct Rev ::: transaction has already been reversed {0}", originalPaymentLogId));
                    return new PaymentReversalResponseObj
                    {
                        HasError = true,
                        ErrorMessage = ErrorLang.transactionhasalreadybeenreversed().ToString()
                    };
                }

                //check if amounts tally
                //so what we are checking for here is that the amount on the old transaction
                //is equal to the amount on the new transaction for reversal
                if (initialCreditTransaction.TotalAmountPaid != Math.Abs(transactionLogVM.AmountPaid))
                {
                    Logger.Error(string.Format("PAY DIRECT REV ::: amount reversed is not equal to the amount paid initially. Payment Id: {0}", initialCreditTransaction.PaymentLogId));
                    return new PaymentReversalResponseObj
                    {
                        HasError = true,
                        ErrorMessage = ErrorLang.reversalamountdoesnotmatch().ToString()
                    };
                }

                //check if payment log id tally
                if (initialCreditTransaction.PaymentReference != originalPaymentReference)
                {
                    Logger.Error(string.Format("PAY DIRECT REV ::: old payment log ref does not match reversal payment log ref.", initialCreditTransaction.PaymentLogId.ToString(), originalPaymentLogId));

                    return new PaymentReversalResponseObj { HasError = true, ErrorMessage = ErrorLang.transactionlogrefdonotmatch().ToString() };
                }

                //we are checking that the amount due on the invoice + the amount reversed is less that the invoice amount
                //this is pretty much how much you owe, if the amount you owe is less than the invoice amount then this invoice is marked as part paid

                //abs is used for the negative value pay direct sends
                decimal amountOwed = Math.Round((Math.Round(helperModel.AmountDue, 2) + Math.Abs(transactionLogVM.AmountPaid)), 2);
                //Question: why do we need this? why do we need to get the sum of all amount paid for a particular invoice, when we could
                //as well just do a substraction from the amount due and the invoice amount.
                //Answer: we need to get the sum of all transactions from the transac log, instead of the said above, because for payments
                //were the invoice was over paid for, we need to take into account that over paid sum.
                //Scenario. Mr A pays for an invoice for 5.00 naira. A makes 4 payments of 1 naira and a final payment of 2 naira
                //so as of the final payment A has paid 7 naira, excess of 2 naira. Now suppose one of the 1 naira payments was from pay direct 
                //doing a invoice amount - amountOwed, would give us a value of 4 naira. Now this indicates that A has a deficit of 1 naira
                //but infact has a surplus of 2 naira if the sum of all payments on the  invoice was taken into consideration.
                //so here if we went invoice amount - amountOwed, the invoice status would be marked as part paid (which is not true)

                //so using A as an example. From the above amountOwed == 1
                //totalAmountPaidForInvoice == 7
                decimal totalAmountPaidForInvoice = Math.Round(_transactionLogManager.GetSumOfAmountPaidForInvoice(transactionLogVM.InvoiceNumber), 2);
                //now the actual totalAmountPaidForInvoice paid == totalAmountPaidForInvoice - reversed amount
                //which is 7-1 == 6
                //this gives the total amount ever paid for the invioce less the amount that is to be reversed.
                totalAmountPaidForInvoice -= Math.Abs(transactionLogVM.AmountPaid);

                //now we want to set the invoice status

                if (totalAmountPaidForInvoice <= 0) { helperModel.Invoice.Status = (int)InvoiceStatus.Unpaid; }
                else if (totalAmountPaidForInvoice < Math.Round(helperModel.Invoice.Amount, 2))
                { helperModel.Invoice.Status = (int)InvoiceStatus.PartPaid; }
                //
                transactionLogVM.IsReversal = true;
                //update the initial transactions to reversed
                if (!_transactionLogManager.UpdateTransactionToReversed(initialCreditTransaction.PaymentReference, PaymentProvider.PayDirect))
                {
                    throw new CouldNotSaveRecord(string.Format("Could not update reversed flag for payment reversal for {0} {1}", initialCreditTransaction.PaymentReference, PaymentProvider.PayDirect));
                }

                var result = UpdatePayment(transactionLogVM, PaymentProvider.PayDirect, false, helperModel);
                return new PaymentReversalResponseObj { };
            }
            catch (Exception exception)
            {
                _transactionLogManager.RollBackAllTransactions();
                Logger.Error(exception, string.Format("PAY DIRECT REV ::: ERROR in pay direct reversal Exception: {0}", exception.Message));
                throw new Exception(ErrorLang.genericexception().ToString());
            }
        }


        /// <summary>
        /// Get transaction group for the given provider and payment reference
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        public TransactionLogGroup GetByPaymentRef(string paymentReference, PaymentProvider provider)
        {
            return _transactionLogManager.GetGroupedTransactionLogByPayment(paymentReference, provider);
        }


        /// <summary>
        /// Payment notification
        /// </summary>
        /// <param name="tranLogVM"></param>
        /// <param name="channel"></param>
        /// <param name="checkPaymentReference"></param>
        /// <returns>InvoiceValidationResponseModel</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="PaymentNoficationAlreadyExistsException"></exception>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        public InvoiceValidationResponseModel UpdatePayment(TransactionLogVM tranLogVM, PaymentProvider provider, bool checkPaymentReference = true, InvoiceDetailsHelperModel helperModel = null)
        {
            try
            {
                TransactionLogGroup modelForPaymentRefCheck = null;

                if (checkPaymentReference)
                {
                    //lets check if this payment request has already been processed previously
                    //if so lets get the invoiceId assigned to it
                    modelForPaymentRefCheck = GetByPaymentRef(tranLogVM.PaymentReference, provider);
                }

                Logger.Information("Getting the invoice details UpdatePayment..");
                if (helperModel == null)
                { helperModel = _invoiceManager.GetInvoiceDetails(tranLogVM.InvoiceNumber); }

                if (helperModel == null) { throw new NoInvoicesMatchingTheParametersFoundException("No invoice found"); }

                //do the check for ascertaining that the invoice belongs to the customer
                if (modelForPaymentRefCheck != null)
                {
                    //we are checking that the record for paymentref was for the same invoice and the same tax profile
                    if (modelForPaymentRefCheck.InvoiceId == helperModel.Invoice.Id && modelForPaymentRefCheck.TaxEntityId == helperModel.TaxEntityId)
                    {
                        throw new PaymentNoficationAlreadyExistsException(string.Format("A payment reference {0} for this channel {1} already exists. Invoice number {2}", tranLogVM.PaymentReference, provider.ToString(), helperModel.Invoice.InvoiceNumber));
                    }
                    else
                    {
                        throw new NoRecordFoundException(string.Format("The invoice associated with the duplicate payment ref {0} provider - {1} invoiceId - {2} does not match the invoice from the invoice number {3} and invoice Id {4}", tranLogVM.PaymentReference, provider.ToString(), modelForPaymentRefCheck.InvoiceId, tranLogVM.InvoiceNumber, helperModel.Invoice.Id));
                    }
                }

                //check if non-reversal notification invoice has already been paid for
                if (!tranLogVM.IsReversal)
                {
                    if (helperModel.Invoice.Status == (int)InvoiceStatus.Paid)
                    {
                        Logger.Information("Invoice Number {0} has already been paid for", helperModel.Invoice.InvoiceNumber);
                        throw new InvoiceAlreadyPaidForException(string.Format("Invoice Number {0} has already been paid for", helperModel.Invoice.InvoiceNumber));
                    }
                }


                //here we set the amountpaid for this transaction
                SetAmountPaid(tranLogVM);

                bool partPaid = false;
                bool doCallBack = true;
                if (!tranLogVM.IsReversal)
                {
                    if (tranLogVM.AmountPaid < helperModel.AmountDue) { partPaid = true; doCallBack = false; }
                }

                if (!tranLogVM.IsReversal)
                {
                    if (partPaid && !AllowPartPayment(helperModel))
                    {
                        throw new PartPaymentNotAllowedException(string.Format("Part payment not not allowed for Invoice Number {0}", helperModel.Invoice.InvoiceNumber));
                    }
                }

                //Add payment notification
                ReceiptVM receipt = AddPaymentNotification(tranLogVM, partPaid, helperModel);

                //Check if the invoice is Direct Assessment invoice
                if (helperModel.Invoice.InvoiceType == (int)InvoiceType.DirectAssessment)
                {
                    int PAYERevenueHeadId = _corePAYEPaymentService.Value.GetPAYERevenueHeadId();
                    _corePAYEPaymentService.Value.ProcessPAYEPayment(receipt.TransactionLogs.Where(t => t.RevenueHeadId == PAYERevenueHeadId).ToList(), helperModel.Invoice.Id, receipt.Id);
                }

                //after payment has been made let see if the invoice has a callback
                //do callback
                if (doCallBack)
                {
                    string callBackURL = !string.IsNullOrEmpty(helperModel.Invoice.CallBackURL) ? helperModel.Invoice.CallBackURL : helperModel.Invoice.RevenueHead.CallBackURL;

                    if (!string.IsNullOrEmpty(callBackURL))
                    {
                        if (tranLogVM.InvoiceNumber.Length < 10) { tranLogVM.InvoiceNumber = helperModel.Invoice.InvoiceNumber; }

                        string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                        Node setting = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                            .Node.Where(k => k.Key == TenantConfigKeys.SiteNameOnFile.ToString()).FirstOrDefault();

                        if (setting != null) { siteName = setting.Value; }

                        SendNotifications(callBackURL, tranLogVM, siteName, helperModel.ExpertSystemClientSecret, helperModel.APIRequestReference);
                    }
                }

                Node node = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                    .Node.Where(x => x.Key == TenantConfigKeys.IsSMSEnabled.ToString()).FirstOrDefault();

                Node sendPaymentNotifNode = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                    .Node.Where(x => x.Key == TenantConfigKeys.SendPaymentNotification.ToString()).FirstOrDefault();

                bool sendPaymentNotification = (sendPaymentNotifNode == null) ? true : bool.Parse(sendPaymentNotifNode.Value);

                if (node != null && !string.IsNullOrEmpty(node.Value))
                {
                    bool isSMSEnabled = false;
                    bool.TryParse(node.Value, out isSMSEnabled);
                    if (isSMSEnabled && !string.IsNullOrEmpty(helperModel.Invoice.TaxPayer.PhoneNumber) && sendPaymentNotification)
                    {
                        //Send sms notification
                        int providerId = 0;
                        bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                        if (!result)
                        {
                            providerId = (int)SMSProvider.Pulse;
                        }
                        foreach (var impl in _smsProvider)
                        {
                            if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                            {
                                string message = $"Dear {helperModel.Invoice.TaxPayer.Recipient}, your payment of {tranLogVM.AmountPaid.ToString("F")} for {helperModel.RevenueHead} was successful. Receipt Number: {receipt.ReceiptNumber}";
                                impl.Value.SendSMS(new List<string> { helperModel.Invoice.TaxPayer.PhoneNumber }, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                                break;
                            }
                        }
                    }
                }

                return new InvoiceValidationResponseModel { ReceiptId = receipt.Id, InvoiceId = helperModel.Invoice.Id, ReceiptNumber = receipt.ReceiptNumber, InvoiceNumber = helperModel.Invoice.InvoiceNumber, Amount = tranLogVM.AmountPaid, MDAName = helperModel.MDAName, RevenueHead = helperModel.RevenueHead, PayerName = helperModel.Invoice.TaxPayer.Recipient };
            }
            catch (PaymentNoficationAlreadyExistsException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " - Error occured in UpdatePayment " + tranLogVM.RequestDump);
                _transactionLogManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Here we compute the amount paid when 
        /// deductibles are performed
        /// </summary>
        /// <param name="tranLogVM"></param>
        private void SetAmountPaid(TransactionLogVM tranLogVM)
        {
            if (!tranLogVM.AllowAgentFeeAddition) { tranLogVM.AgentFee = 0.00m; return; }
            tranLogVM.AmountPaid += tranLogVM.AgentFee;
        }


        /// <summary>
        /// send payment notifications
        /// </summary>
        /// <param name="callBackURL"></param>
        /// <param name="transactionLogVM"></param>
        /// <param name="siteName"></param>
        /// <param name="messageEncryptionKey"></param>
        public void SendNotifications(string callBackURL, TransactionLogVM transactionLogVM, string siteName, string messageEncryptionKey, string requestReference)
        {
            _paymentNotifications.SendPaymentNotification(transactionLogVM, callBackURL, siteName, messageEncryptionKey, requestReference);
        }


        /// <summary>
        /// Search for invoice number
        /// </summary>
        /// <param name="custReference"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GetInvoiceDetails(string invoiceNumber)
        {
            return _invoiceManager.GetInvoiceDetailsForPaymentView(invoiceNumber);
        }


        /// <summary>
        /// Get the model for generating web payment for pay direct
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns>PayDirectWebPaymentModel</returns>
        public PayDirectWebPaymentFormModel GetPayDirectWebFormParameters(PayDirectConfigurations payDirectConfig, string transactionRef, decimal amountDue)
        {
            if (_payDirect == null) { _payDirect = new PayDirect(payDirectConfig) { }; }

            PayDirectWebPaymentFormModel model = _payDirect.GetWebPaymentModel(transactionRef, amountDue);
            model.ActionURL = AppSettingsConfigurations.GetSettingsValue("PayDirectWebPostURL");
            return model;
        }


        /// <summary>
        /// Get transaction ref for pay direct web
        /// </summary>
        /// <param name="payDirectConfig"></param>
        /// <param name="stateName"></param>
        /// <param name="amountDue"></param>
        /// <returns>string</returns>
        public string GetTransactionRefForPayDirectWeb(PayDirectConfigurations payDirectConfig, decimal amountDue, decimal fee, PayDirectWebPaymentRequestModel tokenModel)
        {
            //create a new payment request
            WebPaymentRequest model = new WebPaymentRequest { Amount = amountDue, InvoiceNumber = tokenModel.InvoiceNumber, TransactionReference = Guid.NewGuid().ToString() + DateTime.Now.Ticks.ToString(), CallBackURL = tokenModel.CallBackURL, ClientId = tokenModel.ClientId, RequestIdentifier = tokenModel.HashValue, FeeApplied = fee, RequestDump = JsonConvert.SerializeObject(tokenModel), RequestSource = WebPaymentRequestSource.Eregistry, /*WebPaymentChannel = PaymentChannel.PayDirectWeb*/ };

            if (!_webPayRequestManager.Save(model))
            { throw new CannotSaveRecordException("Cannot save WebPaymentRequest record " + JsonConvert.SerializeObject(model)); }
            string prefix = payDirectConfig.ConfigNodes.Where(k => k.Key == "ShortCode").First().Value;

            //pay direct web has a payment ref max length of 15
            //
            string payRef = GenerateRef(Util.ZeroPadUp(model.Id.ToString(), 10));

            if (payRef.Length > 15)
            {
                _webPayRequestManager.RollBackAllTransactions();
                throw new Exception("Payment ref value has exceeded 15. TODO, find new ref generation");
            }
            model.TransactionReference = payRef;
            return payRef;
        }


        /// <summary>
        /// Returns the fee to be paid 
        /// </summary>
        /// <param name="payDirectConfig"></param>
        /// <param name="amountDue"></param>
        /// <returns>decimal</returns>
        public decimal GetFeeToBeAppliedForPayDirectWeb(PayDirectConfigurations payDirectConfig, decimal amountDue)
        {
            if (_payDirect == null) { _payDirect = new PayDirect(payDirectConfig) { }; }
            return _payDirect.GetFeeToBeApplied(amountDue);
        }

        /// <summary>
        /// Add some random number to mask serial patternery
        /// <para>Refnumber is serial, that is the Id of the record. This method adds some letters to the value and pipes. 
        /// This in no way removes from the uniqueness of the refnumber </para>
        /// </summary>
        /// <param name="refNumber"></param>
        /// <returns>string</returns>
        private string GenerateRef(string refNumber)
        {
            StringBuilder refNumberBuilder = new StringBuilder(refNumber);
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rand = new Random();
            int randIndex0 = rand.Next(0, 8);
            int randIndex1 = rand.Next(8, 19);
            int randIndex2 = rand.Next(19, 26);
            string randValue0 = alphabets.Substring(randIndex0, 1);
            string randValue1 = alphabets.Substring(randIndex1, 1);
            string randValue2 = alphabets.Substring(randIndex2, 1);

            Random refRand = new Random();
            int refIndex0 = refRand.Next(0, 4);
            int refIndex1 = refRand.Next(4, 7);
            int refIndex2 = refRand.Next(7, 10);

            refNumberBuilder.Insert(refIndex0, randValue0);
            refNumberBuilder.Insert(refIndex1, randValue1);
            refNumberBuilder.Insert(refIndex2, randValue2);
            refNumberBuilder.Insert(4, "|");
            refNumberBuilder.Insert(9, "|");
            return refNumberBuilder.ToString();
        }


        /// <summary>
        /// Get transaction log by payment log Id
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <returns>TransactionLog</returns>
        public TransactionLogGroup GetTransactionLogByPaymentLogId(string paymentLogId, PaymentProvider channel)
        {
            return _transactionLogManager.GetGroupedTransactionLogByPaymentLogId(paymentLogId, channel);
        }


        /// <summary>
        /// Get transaction with RetrievalReferenceNumber
        /// </summary>
        /// <param name="retrievalRef"></param>
        /// <returns>TransactionLogGroup</returns>
        public TransactionLogGroup GetTransactionLogByRetrievalReferenceNumber(string invoiceNumber, string retrievalRef, int paymentProviderId, PaymentChannel channel)
        {
            return _transactionLogManager.GetGroupedTransactionLogByRetrievalReferenceNumber(invoiceNumber, retrievalRef, paymentProviderId, channel);
        }


        /// <summary>
        /// Update invoice, adding transaction log and receipt generation for payment
        /// <para>Handle exception. All check to determine whether the invoice has been paid for should have been done before this method</para>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="helperModel"></param>
        /// <exception cref="Exception"></exception>
        private ReceiptVM AddPaymentNotification(TransactionLogVM model, bool partPaid, InvoiceDetailsHelperModel helperModel)
        {
            try
            {
                Logger.Information("Adding invoice payment notification..");
                //get the amount due
                if (!model.IsReversal)
                {
                    if (partPaid)
                    { helperModel.Invoice.Status = (int)InvoiceStatus.PartPaid; } //should check whether we can update an invoice in overdue state
                    else { helperModel.Invoice.Status = (int)InvoiceStatus.Paid; }
                }

                //set the payment date on the invoice
                helperModel.Invoice.PaymentDate = model.PaymentDate;

                Dictionary<long, decimal> amountPairs = helperModel.InvoiceItems.ToDictionary(itm => itm.Id, eitm => eitm.TotalAmount);
                Dictionary<long, decimal> splits = amountPairs.Count == 1 ? new Dictionary<long, decimal> { { amountPairs.First().Key, model.AmountPaid } } : GetSplits(amountPairs, model, helperModel);

                //do tran log models
                List<TransactionLog> transactionLogs = new List<TransactionLog>(helperModel.InvoiceItems.Count);
                var amountPaid = 0.00m;
                decimal feeDeduction = 0.00m;

                Receipt receipt = _coreReceiptService.SaveTransactionReceipt(new Receipt()
                {
                    Invoice = helperModel.Invoice,
                });

                _coreReceiptService.EvictReceiptObject(receipt);

                string receiptNumber = _coreReceiptService.GetReceiptNumberById(receipt.Id);

                if (string.IsNullOrEmpty(receiptNumber)) { throw new Exception { }; }

                int paymentType = model.IsReversal ? (int)PaymentType.Debit : (int)PaymentType.Credit;
                //do fee split
                Dictionary<long, decimal> feeSplits = null;
                if (model.AllowAgentFeeAddition)
                {
                    feeSplits = Util.DoAmountSplit(amountPairs, model.AgentFee);
                }

                foreach (var item in helperModel.InvoiceItems)
                {
                    feeDeduction = 0.00m;

                    if (!splits.TryGetValue(item.Id, out amountPaid))
                    { throw new Exception(string.Format("Could not find/compute split of invoice items for invoice number {0}", helperModel.Invoice.InvoiceNumber)); }

                    if (feeSplits != null)
                    { feeSplits.TryGetValue(item.Id, out feeDeduction); }

                    transactionLogs.Add(new TransactionLog
                    {
                        Receipt = receipt,
                        ReceiptNumber = receiptNumber,
                        Invoice = helperModel.Invoice,
                        InvoiceItem = new InvoiceItems { Id = item.Id },
                        Status = PaymentStatus.Successful,
                        TypeID = paymentType,
                        TaxEntity = new TaxEntity { Id = helperModel.TaxEntityId },
                        TaxEntityCategory = new TaxEntityCategory { Id = helperModel.TaxCategoryId },
                        RevenueHead = new RevenueHead { Id = item.RevenueHeadId },
                        MDA = new MDA { Id = item.MDAId },
                        AmountPaid = model.IsReversal ? (-amountPaid) : amountPaid,
                        InvoiceNumber = helperModel.Invoice.InvoiceNumber,
                        Channel = model.Channel,
                        AdminUser = model.AdminUser,
                        AgencyCode = model.AgencyCode,
                        Bank = model.Bank,
                        BankBranch = model.BankBranch,
                        BankChannel = model.BankChannel,
                        BankCode = model.BankCode,
                        Fee = model.Fee,
                        ItemCode = model.ItemCode,
                        ItemName = model.ItemName,
                        OriginalPaymentLogID = model.OriginalPaymentLogID,
                        OriginalPaymentReference = model.OriginalPaymentReference,
                        PayerAddress = model.PayerAddress,
                        PayerEmail = model.PayerEmail,
                        PayerName = model.PayerName,
                        PayerPhoneNumber = model.PayerPhoneNumber,
                        PaymentDate = model.PaymentDate,
                        PaymentLogId = model.PaymentLogId,
                        PaymentMethod = model.PaymentMethod,
                        PaymentMethodId = model.PaymentMethodId,
                        PaymentProvider = model.PaymentProvider,
                        PaymentReference = model.PaymentReference,
                        ThirdPartyReceiptNumber = model.ThirdPartyReceiptNumber,
                        RequestDump = model.RequestDump,
                        RetrievalReferenceNumber = model.RetrievalReferenceNumber,
                        RevenueHeadCode = model.RevenueHeadCode,
                        ServiceType = model.ServiceType,
                        SlipNumber = model.SlipNumber,
                        TellerName = model.TellerName,
                        TotalAmountPaid = model.TotalAmountPaid,
                        TransactionDate = model.TransactionDate,
                        UpdatedByAdmin = model.UpdatedByAdmin,
                        SettlementFeeDeduction = feeDeduction,
                    });
                }

                if (_transactionLogManager.SaveBundleUnCommitStatelessWithErrors(transactionLogs) != -1)
                { throw new Exception("Error saving transaction log"); }

                return new ReceiptVM { Id = receipt.Id, ReceiptNumber = receiptNumber, TransactionLogs = transactionLogs.Select(t => new TransactionLogVM { TransactionLogId = t.Id, RevenueHeadId = t.RevenueHead.Id, AmountPaid = t.AmountPaid }).ToList() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _transactionLogManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Get invoice items split
        /// </summary>
        /// <param name="amountPairs"></param>
        /// <param name="model"></param>
        /// <param name="helperModel"></param>
        /// <returns>Dictionary{long, decimal}</returns>
        private Dictionary<long, decimal> GetSplits(Dictionary<long, decimal> amountPairs, TransactionLogVM model, InvoiceDetailsHelperModel helperModel)
        {
            return CheckIfAmountHasNoPartPayments(model, helperModel) ? amountPairs : DoSplitComputation(amountPairs, model, helperModel);
        }


        /// <summary>
        /// Here we know that the amount paid is the same as the amount due
        /// as well as it is the same with the invoice amount
        /// there by the invoice has never received any payment before.
        /// What this method does is to 
        /// </summary>
        /// <param name="amountPairs"></param>
        /// <param name="model"></param>
        /// <param name="helperModel"></param>
        /// <returns>Dictionary{long, decimal}</returns>
        private Dictionary<long, decimal> DoSplitComputation(Dictionary<long, decimal> amountPairs, TransactionLogVM model, InvoiceDetailsHelperModel helperModel)
        {
            if (!model.IsReversal)
            {
                return Util.DoAmountSplit(amountPairs, model.AmountPaid);
            }
            else
            {
                return Util.DoAmountSplit(amountPairs, Math.Abs(model.AmountPaid));
            }
        }

        /// <summary>
        /// When a payment is sent we check to see if the payment
        /// has been fully paid for on first payment, that is this invoice has no
        /// part payments and the exact amount is being paid for
        /// </summary>
        /// <param name="model"></param>
        /// <param name="helperModel"></param>
        /// <returns>bool</returns>
        private bool CheckIfAmountHasNoPartPayments(TransactionLogVM tranLogVM, InvoiceDetailsHelperModel helperModel)
        {
            return ((tranLogVM.AmountPaid == helperModel.AmountDue) && (helperModel.AmountDue == helperModel.Invoice.Amount));
        }


        /// <summary>
        /// Get transaction details for pay direct web
        /// </summary>
        /// <param name="model"></param>
        /// <returns>PayDirectWebServerResponse</returns>
        public PayDirectWebServerResponse GetPayDirectWebTransaction(PayDirectConfigurations payDirectConfig, PayDirectWebPaymentResponseModel model)
        {
            Logger.Information(string.Format("Calling PDW for transaction confirmation"));
            if (_payDirect == null) { _payDirect = new PayDirect(payDirectConfig) { }; }
            string productId = _payDirect.GetProductId();
            string trnxConfirmationHash = _payDirect.GetTransactionConfirmationHash(model, productId);
            string url = AppSettingsConfigurations.GetSettingsValue("PayDirectWebGetTransactionURL");
            string responseString = _remoteClient.SendRequest(new RequestModel { Model = new { }, URL = url, Headers = new Dictionary<string, dynamic> { { "hash", trnxConfirmationHash } } }, HttpMethod.Get, new Dictionary<string, string> { { "productid", productId }, { "transactionreference", model.txnref }, { "amount", model.apprAmt }, { "paymentRef", model.payRef }, { "retreivalRef", model.retRef } });

            Logger.Information(string.Format("String response form PDW confirmation {0}", responseString));
            return JsonConvert.DeserializeObject<PayDirectWebServerResponse>(responseString);
        }


        /// <summary>
        /// Get the amount paid less the transaction fee for pay direct web
        /// </summary>
        /// <param name="payDirectConfig"></param>
        /// <param name="model"></param>
        /// <returns>decimal</returns>
        public decimal GetAmountPaidLessTransactionFeeForPayDirectWeb(PayDirectConfigurations payDirectConfig, int amountPaid, decimal feeApplied)
        {
            if (_payDirect == null) { _payDirect = new PayDirect(payDirectConfig) { }; }
            return _payDirect.GetAmountPaidLessTransactionFee(amountPaid, feeApplied);
        }


        /// <summary>
        /// Save Payment Reference details
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <returns>PaymentReferenceVM</returns>
        public PaymentReference SavePaymentReference(PaymentReference paymentReference)
        {
            if (_invoiceManager.IsInvoicePaid(paymentReference.InvoiceNumber))
            {
                Logger.Information("Invoice Number {0} has already been paid for", paymentReference.InvoiceNumber);
                throw new InvoiceAlreadyPaidForException(string.Format("Invoice Number {0} has already been paid for, kindly refresh your page", paymentReference.InvoiceNumber));
            }
            if (!_paymentReferenceManager.Save(paymentReference))
            { throw new CouldNotSaveRecord(string.Format("Cannot save Payment Reference details for invoice number {0}", paymentReference.Invoice.InvoiceNumber)); }
            return paymentReference;
        }


        /// <summary>
        /// Get Payment Reference details using Id
        /// </summary>
        /// <param name="paymentReferenceId"></param>
        /// <returns>PaymentReference</returns>
        public APIResponse GetPaymentReference(Int64 paymentReferenceId)
        {
            var paymentReference = _paymentReferenceManager.Get(r => r.Id == paymentReferenceId);
            if (paymentReference == null) { throw new NoRecordFoundException(string.Format("No Payment Reference record found for Reference Id {0}", paymentReferenceId)); }
            return new APIResponse { Error = false, ResponseObject = paymentReference.ReferenceNumber };
        }


        /// <summary>
        /// Evict PaymentReference object from cache
        /// </summary>
        /// <param name="paymentReference"></param>
        public void EvictPaymentReferenceObject(PaymentReference paymentReference)
        {
            _paymentReferenceManager.Evict(paymentReference);
        }


        /// <summary>
        /// Get Payment Reference details using referenceNumber
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns>PaymentReferenceVM</returns>
        public PaymentReferenceVM GetPaymentReferenceDetail(string referenceNumber)
        {
            var paymentReference = _paymentReferenceManager.Get(r => r.ReferenceNumber == referenceNumber);
            if (paymentReference == null) { throw new NoRecordFoundException(string.Format("No Payment Reference record found for Reference Number {0}", referenceNumber)); }
            return new PaymentReferenceVM { ReferenceNumber = paymentReference.ReferenceNumber, InvoiceNumber = paymentReference.Invoice.InvoiceNumber, InvoiceDescription = paymentReference.Invoice.InvoiceDescription, PayerId = paymentReference.Invoice.TaxPayer.PayerId, Recipient = paymentReference.Invoice.TaxPayer.Recipient, RevenueHead = paymentReference.Invoice.RevenueHead.Name };
        }

        /// <summary>
        /// Send notification for the specified payment reference and provider
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="provider"></param>
        /// <returns>APIResponse</returns>
        public APIResponse SendNotifications(string paymentReference, PaymentProvider provider)
        {
            try
            {
                InvoiceDetails invoiceDetails = _transactionLogManager.GetTransactionLogByPaymentReference(paymentReference, provider);

                string callBackURL = !string.IsNullOrEmpty(invoiceDetails.CallBackURL) ? invoiceDetails.CallBackURL : invoiceDetails.RevenueHeadCallBackURL;

                if (string.IsNullOrEmpty(callBackURL))
                {
                    return new APIResponse { Error = true, ResponseObject = ErrorLang.callbackurl404().ToString() };
                }

                _paymentNotifications.SendPaymentNotification(invoiceDetails.Transaction, callBackURL, invoiceDetails.ExpertSystemKey, invoiceDetails.RequestRef);
                var stateConfig = Util.StateConfig().StateConfig.Where(s => s.Value == _orchardServices.WorkContext.CurrentSite.SiteName).FirstOrDefault();
                var merchantSite = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.MerchantSite.ToString()).FirstOrDefault();
                var invoiceCallback = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.MerchantCallBackURL.ToString()).FirstOrDefault();
                return new APIResponse { Error = false, ResponseObject = merchantSite.Value + invoiceCallback.Value };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Call NetPay to get status of a transaction using referenceNumber
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns>Task<NetPayTransactionVM></returns>
        public async Task<NetPayTransactionVM> VerifyNetPayPayment(string referenceNumber)
        {
            NetPayTransactionVM model = new NetPayTransactionVM();
            try
            {
                var stateConfig = Util.StateConfig().StateConfig.Where(s => s.Value == _orchardServices.WorkContext.CurrentSite.SiteName).FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string merchantKey = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayMerchantKey.ToString()).FirstOrDefault().Value;
                string merchantSecretId = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayMerchantSecretId.ToString()).FirstOrDefault().Value;
                string URL = $"{AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.NetPayURL)}/{referenceNumber}";
                HttpClientHandler handler = new HttpClientHandler();
                handler.Credentials = new NetworkCredential(merchantKey, merchantSecretId);

                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.GetAsync(URL);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CannotVerifyNetPayTransaction($"Unable to verify {referenceNumber} reference number. {response.Content.ReadAsStringAsync().Result}");
                    }
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    model = JsonConvert.DeserializeObject<NetPayTransactionVM>(jsonResponse);
                }

                return model;
            }
            catch (AggregateException exception)
            {
                throw new Exception(exception.InnerExceptions.First().InnerException.Message);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<InvoiceValidationResponseModel> SaveNetpayPayment(PaymentAcknowledgeMentModel model)
        {
            try
            {
                //Get transaction details from Netpay
                NetPayTransactionVM transactionVM = await VerifyNetPayPayment(model.PaymentRequestRef);
                if (string.IsNullOrEmpty(transactionVM.PaymentRef) || !transactionVM.Code.Equals(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.NetPaySuccessCode)))
                {
                    Logger.Error($"Payment was not successful:::Reference Number { model.PaymentRequestRef} Payload: {JsonConvert.SerializeObject(transactionVM)}");
                    throw new Exception($"Payment for Reference Number {model.PaymentRequestRef} was not successful");
                }

                Logger.Information($"NetPay payment details {JsonConvert.SerializeObject(transactionVM)}");

                DateTime paymentDate = paymentDate = DateTimeOffset.ParseExact(transactionVM.PaymentDate, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture).DateTime;
                DateTime transactionDate = DateTimeOffset.ParseExact(transactionVM.TransactionDate, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture).DateTime;

                model.Amount = (transactionVM.Amount / 100);
                InvoiceValidationResponseModel response = UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)PaymentChannel.Web,
                    InvoiceNumber = model.InvoiceNumber,
                    PaymentReference = transactionVM.MerchantRef,
                    AmountPaid = Math.Round((model.Amount), 2),
                    PaymentDate = paymentDate,
                    TransactionDate = transactionDate,
                    RequestDump = JsonConvert.SerializeObject(transactionVM),
                    UpdatedByAdmin = false,
                    RetrievalReferenceNumber = transactionVM.PaymentRef,
                    PaymentMethodId = (int)PaymentMethods.DebitCard,
                    PaymentProvider = (int)PaymentProvider.Bank3D
                }, PaymentProvider.Bank3D);

                model.PaymentStatus = PaymentStatus.Successful;
                return response;
            }
            catch (PaymentNoficationAlreadyExistsException)
            {
                TransactionLogGroup res = GetByPaymentRef(model.PaymentRequestRef, PaymentProvider.Bank3D);
                model.PaymentStatus = PaymentStatus.Successful;
                return new InvoiceValidationResponseModel { ReceiptNumber = res.ReceiptNumber, Amount = res.TotalAmountPaid };
            }
            catch (CannotVerifyNetPayTransaction)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (InvoiceAlreadyPaidForException)
            {
                throw;
            }
            catch (PartPaymentNotAllowedException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the preference value of the highest priority activityType
        /// </summary>
        /// <param name="helperModel"></param>
        /// <returns></returns>
        private bool AllowPartPayment(InvoiceDetailsHelperModel helperModel)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
            Dictionary<int, bool> permissions = ObjectCacheProvider.GetCachedObject<Dictionary<int, bool>>(tenant, $"{nameof(CachePrefix.AllowPartPayment)}-{helperModel.RevenueHeadId}-{helperModel.MDAId}");
            if (permissions == null)
            {
                permissions = _activityPermissionRepo.GetPermissionPreference(CBSPermissionName.Allow_Part_Payment, helperModel.RevenueHeadId, helperModel.MDAId);
                if (permissions != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(CachePrefix.AllowPartPayment)}-{helperModel.RevenueHeadId}-{helperModel.MDAId}", permissions);
                }
            }

            if (permissions == null || !permissions.Any())
                return true;

            if (permissions.TryGetValue((int)ActivityType.RevenueHead, out bool revenueHeadValue))
                return revenueHeadValue;

            if (permissions.TryGetValue((int)ActivityType.MDA, out bool mdaValue))
                return mdaValue;

            return true;
        }

    }
}