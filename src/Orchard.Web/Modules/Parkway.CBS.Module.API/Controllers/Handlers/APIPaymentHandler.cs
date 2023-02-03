using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using System.Globalization;
using Newtonsoft.Json;
using Parkway.ThirdParty.Payment.Processor.Processors.PayDirect;
using Parkway.ThirdParty.Payment.Processor.Models;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Exceptions;
using System.Linq;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Models.Enums;
using Parkway.EbillsPay.Models;
using Parkway.EbillsPay;
using Parkway.CBS.Core.StateConfig;
using System.Net;
using Parkway.CBS.Core.PaymentProviderHandlers.Contracts;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIPaymentHandler : BaseAPIHandler, IAPIPaymentHandler
    {
        private readonly ICorePaymentService _corePaymentService;
        private readonly ICoreDirectAssessmentBatchRecord _coreDirectAssessmentService;
        private readonly IOrchardServices _orchardServices;
        private IPayDirect _payDirect;

        private readonly ICoreTaxPayerService _coreTaxPayerService;
        private readonly ICoreInvoiceService _coreInvoiceService;
        private readonly ICoreRevenueHeadService _coreRevenueHeadService;
        private readonly INIBSSEBillsPay _iNIBSSEBillsPay;
        private readonly Lazy<ICoreExternalPaymentProviderService> _corePaymentProvider;
        private readonly Lazy<IPaymentProviderHandler> _paymentProviderHandler;
        private readonly Lazy<IReadyCashPaymentProviderHandler> _readycashPaymentProviderHandler;

        public APIPaymentHandler(ICorePaymentService corePaymentService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IOrchardServices orchardServices, ICoreDirectAssessmentBatchRecord coreDirectAssessmentService, ICoreInvoiceService coreInvoiceService, ICoreTaxPayerService coreTaxPayerService, ICoreRevenueHeadService coreRevenueHeadService, Lazy<ICoreExternalPaymentProviderService> corePaymentProvider, Lazy<IPaymentProviderHandler> paymentProviderHandler, Lazy<IReadyCashPaymentProviderHandler> readycashPaymentProviderHandler) : base(settingsRepository)
        {
            _corePaymentService = corePaymentService;
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _coreDirectAssessmentService = coreDirectAssessmentService;

            _coreInvoiceService = coreInvoiceService;
            _iNIBSSEBillsPay = new NIBSSEBillsPay();
            _coreTaxPayerService = coreTaxPayerService;
            _coreRevenueHeadService = coreRevenueHeadService;

            _corePaymentProvider = corePaymentProvider;
            _paymentProviderHandler = paymentProviderHandler;
            _readycashPaymentProviderHandler = readycashPaymentProviderHandler;
        }


        /// <summary>
        /// Validate the payment noification model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        private void ValidatePaymentNotificationModel(PaymentNotification model, ref List<ErrorModel> errors)
        {
            //need to validate model
            if (model == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Model is empty", FieldName = "PaymentNotification" });
                throw new DirtyFormDataException("Model is empty");
            }

            HasStringValue(model.PaymentRef, "PaymentRef", ref errors);
            HasStringValue(model.PaymentDate, "PaymentDate", ref errors);
            HasStringValue(model.TransactionDate, "TransactionDate", ref errors);
            if (model.AmountPaid < 0.00m)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.invoiceamountistoosmall().ToString(), FieldName = "AmountPaid" });
                throw new DirtyFormDataException { };
            }
        }


        /// <summary>
        /// Process paye payment notification
        /// </summary>
        /// <param name="paymentController"></param>
        /// <param name="model"></param>
        /// <param name="p"></param>
        /// <returns>APIResponse</returns>
        public APIResponse PayePaymentNotification(PaymentNotification model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            bool IsDuplicatePaymentReference = false;

            Logger.Information("create PaymentNotification for " + JsonConvert.SerializeObject(model));
            try
            {
                ValidatePaymentNotificationModel(model, ref errors);
                DateTime paymentDate = ValidateDate(model.PaymentDate, "PaymentDate", "dd/MM/yyyy HH:mm:ss", ref errors);
                DateTime tranxDate = ValidateDate(model.TransactionDate, "TransactionDate", "dd/MM/yyyy HH:mm:ss", ref errors);
                DateTime payePeriod = new DateTime { };
                try
                {
                    payePeriod = new DateTime(model.Year, model.Month, 1);
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("Error getting paye period month : {0}, year : {1}", model.Month, model.Year));
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.payeperiodnotvalid(model.Month.ToString(), model.Year.ToString()).ToString(), FieldName = "Month/Year" });
                    throw new DirtyFormDataException { };
                }

                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //check if this is registered as a payment provider
                if (expertSystem.PaymentProviderId <= 0)
                {
                    string msg = ErrorLang.apiactionforbidden("Paye Payment Notification").ToString();
                    Logger.Error(msg + "Expert system " + expertSystem.CompanyName);
                    throw new APIPermissionException(msg);
                }

                //check if this is expert system can make paye payments
                if (expertSystem.ListOfPermissions().Where(per => per.Permission == EnumExpertSystemPermissions.CanMakePayePayments).FirstOrDefault() == null)
                {
                    string msg = ErrorLang.apiactionforbidden("Paye Payment Notification").ToString();
                    Logger.Error(msg + "Expert system " + expertSystem.CompanyName);
                    throw new APIPermissionException(msg);
                }

                string valueString = model.PaymentRef + model.AmountPaid.ToString("F") + model.AgencyCode + model.BankCode + model.Month + model.Year;

                if (!CheckHash(valueString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + valueString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                //check for the assessment type
                //if the assessment type is none, we default to FileUploadForIPPIS
                PayeAssessmentType type = (PayeAssessmentType)model.PayeAssessmentType;
                if (type == PayeAssessmentType.None) { type = PayeAssessmentType.FileUploadForIPPIS; }

                PaymentProvider provider = (PaymentProvider)expertSystem.PaymentProviderId;

                //get the tax entity that has this agency code
                var taxEntity = _coreTaxPayerService.GetTaxEntity(t => t.TaxPayerCode == model.AgencyCode && t.IsDisabled == false);
                if (taxEntity == null)
                { taxEntity = _coreTaxPayerService.GetTaxEntity(t => ((t.UnknownProfile) && (t.IsDisabled == false))); }

                Logger.Information(string.Format("Getting the batch record that maps to the Agency code {0}, type {1}, and period {2}", model.AgencyCode, type.ToString(), payePeriod.ToString()));

                DirectAssessmentBatchRecord record = _coreDirectAssessmentService.GetPayeAssessmentByMonthAndYear(model.AgencyCode, type, payePeriod);

                if (record == null)
                {
                    Logger.Information(string.Format("No record found. Doing GenerateInvoiceForRecordsThatDontExistsForIPPIS for agency code {0}, type {1}, time {2}", model.AgencyCode, type.ToString(), payePeriod.ToString()));

                    return GenerateInvoiceForRecordsThatDontExistsForIPPIS(model, taxEntity, expertSystem, ref errors, paymentDate, tranxDate, provider, payePeriod, record);
                }


                bool saveUnreconciledPayePayment = false;
                //This is com
                ////check that the batch record belongs to the tax profile
                ////so here we are checking that the tax profile for this record maps to the tax entity for this agency code
                ////or the unknow entity
                if (taxEntity.Id != record.TaxEntity.Id)
                {
                    saveUnreconciledPayePayment = true;
                    Logger.Information(string.Format("The tax profile attached to the batch do not match. Batch Id {0}, Tax entity Id {1}", record.Id, taxEntity.Id));
                }

                //get the invoice that has this record Id
                Logger.Information(string.Format("We are getting the invoice helper model that maps to the DirectAssessment type {0} and batch record {1}", InvoiceType.DirectAssessment.ToString(), record.Id));
                InvoiceDetailsHelperModel helperModel = _coreInvoiceService.GetInvoiceHelperDetailsByInvoiceType(InvoiceType.DirectAssessment, record.Id);

                if (helperModel == null)
                {
                    throw new NoRecordFoundException(string.Format("No record found for Direct assessment invoice. Direct assessment number {0}", record.Id));
                }

                //now we need to check that the tax profile on the helper model maps to the same taxEntity profile
                if (taxEntity.Id != helperModel.TaxEntityId)
                {
                    saveUnreconciledPayePayment = true;
                    Logger.Information(string.Format("The tax profile attached to the batch do not match, the tax profile the invoice is attached to. Batch Id {0}, Tax entity Id {1}, invoice number {2}, invoice Id {3}", record.Id, taxEntity.Id, helperModel.Invoice.InvoiceNumber, helperModel.Invoice.Id));
                }

                try
                {
                    PaymentChannel channel = GetChannelTypeForPayDirect(model.Channel);

                    InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                    {
                        Channel = (int)channel,
                        InvoiceNumber = helperModel.Invoice.InvoiceNumber,
                        PaymentReference = model.PaymentRef,
                        AmountPaid = model.AmountPaid,
                        PaymentDate = paymentDate,
                        TransactionDate = tranxDate,
                        RequestDump = JsonConvert.SerializeObject(model),
                        Bank = model.BankName,
                        BankBranch = model.BankBranch,
                        BankCode = model.BankCode,
                        AgencyCode = model.AgencyCode,
                        PaymentMethod = model.PaymentMethod,
                        PaymentProvider = (int)provider,
                        TypeID = (int)PaymentType.Credit,
                    }, provider, helperModel: helperModel);


                    if (saveUnreconciledPayePayment)
                    {
                        _coreDirectAssessmentService.SaveUnreconciledPayePayments(new UnreconciledPayePayments
                        {
                            Receipt = new Receipt { Id = response.ReceiptId },
                            Month = payePeriod.Month,
                            Year = payePeriod.Year,
                            PaymentReference = model.PaymentRef,
                            ExpertSystem = expertSystem,
                            UnReconciledTaxEntity = new TaxEntity { Id = taxEntity.Id },
                            DirectAssessmentBatchRecord = new DirectAssessmentBatchRecord { Id = record.Id }
                        });
                    }
                }
                catch (PaymentNoficationAlreadyExistsException exception)
                {
                    Logger.Information(exception, ErrorLang.paymentrefalreadyprocessed(model.PaymentRef).ToString() + exception.Message);
                    IsDuplicatePaymentReference = true;
                }

                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseObject = new
                    {
                        model.PaymentRef,
                        helperModel.Invoice.InvoiceNumber,
                        IsDuplicatePaymentReference
                    }
                };
            }
            catch (NoRecordFoundException)
            {
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.norecord404().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPREC404;
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404("Tenant").ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404("Tenant").ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPVE;
            }
            catch (AmountTooSmallException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "PayePaymentNotification", ErrorMessage = ErrorLang.invoiceamountistoosmall().ToString() });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (InvoiceAlreadyPaidForException exception) 
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoiceFullyPaid(model.InvoiceNumber).ToString() });
                errorCode = ErrorCode.PP_INVOICE_ALREADY_PAID;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch(PartPaymentNotAllowedException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.nopartpaymentsallow().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (APIPermissionException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "Authorization", ErrorMessage = exception.Message });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Payment", ErrorMessage = ErrorLang.genericexception().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }


        /// <summary>
        /// This method generates the invoice for assessments that payment notifications
        /// have been sent, but we donot have a record of these payments
        /// </summary>
        /// <param name="model"></param>
        /// <param name="taxEntity"></param>
        /// <param name="expertSystem"></param>
        /// <param name="errors"></param>
        /// <returns>APIResponse</returns>
        private APIResponse GenerateInvoiceForRecordsThatDontExistsForIPPIS(PaymentNotification model, TaxEntity taxEntity, ExpertSystemSettings expertSystem, ref List<ErrorModel> errors, DateTime paymentDate, DateTime tranxDate, PaymentProvider provider, DateTime payePeriod, DirectAssessmentBatchRecord record)
        {
            try
            {
                Logger.Information(string.Format("Ref 404 for {0}. Checking the payment ref ", model.PaymentRef));
                TransactionLogGroup paymentRefCheck = _corePaymentService.GetByPaymentRef(model.PaymentRef, provider);

                if (paymentRefCheck != null)
                {
                    //an invoice is tied to this payment ref
                    throw new NoRecordFoundException(string.Format("There is an invoice associated with the payment ref {0} provider - {1} invoiceId - {2}", model.PaymentRef, provider.ToString(), paymentRefCheck.InvoiceId));
                }

                //get revenue head assigned for allocation of unreconciled collection
                int revid = _coreRevenueHeadService.GetRevenueHeadIdForUnreconciledCollections(_orchardServices.WorkContext.CurrentSite.SiteName);
                //lets create and invoice for this
                var createInvoiceModel = new CreateInvoiceModel
                {
                    RevenueHeadId = revid,
                    TaxEntityInvoice = new TaxEntityInvoice
                    {
                        TaxEntity = new TaxEntity
                        {
                            Address = taxEntity.Address,
                            Email = taxEntity.Email,
                            PhoneNumber = taxEntity.PhoneNumber,
                            Recipient = taxEntity.Recipient,
                            TaxPayerIdentificationNumber = taxEntity.TaxPayerIdentificationNumber,
                            Id = taxEntity.Id,
                            PayerId = taxEntity.PayerId
                        },
                        Amount = model.AmountPaid,
                        CategoryId = taxEntity.TaxEntityCategory.Id,
                        InvoiceDescription = Util.GetInvoiceDescriptionForIPPIS(model.AgencyCode, payePeriod.Month, payePeriod.Year, true)
                    },
                };

                createInvoiceModel.ApplySurcharge = false;
                Int64 invoiceId = _coreInvoiceService.TryCreateInvoice(createInvoiceModel, ref errors, expertSystem).InvoiceId;
                _settingsRepository.Flush();

                InvoiceDetailsHelperModel helperModel = _coreInvoiceService.GetInvoiceHelperDetailsByInvoiceType(invoiceId);

                PaymentChannel channel = GetChannelTypeForPayDirect(model.Channel);

                //invoice has been generated, lets make payment
                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)channel,
                    InvoiceNumber = helperModel.Invoice.InvoiceNumber,
                    PaymentReference = model.PaymentRef,
                    AmountPaid = model.AmountPaid,
                    PaymentDate = paymentDate,
                    TransactionDate = tranxDate,
                    RequestDump = JsonConvert.SerializeObject(model),
                    Bank = model.BankName,
                    BankBranch = model.BankBranch,
                    BankCode = model.BankCode,
                    AgencyCode = model.AgencyCode,
                    PaymentMethod = model.PaymentMethod,
                    PaymentProvider = (int)provider,
                    TypeID = (int)PaymentType.Credit,
                    RevenueHeadCode = revid.ToString(),
                }, provider, false, helperModel);
                //lets save the records to the UnreconciledPayePayments table

                _coreDirectAssessmentService.SaveUnreconciledPayePayments(new UnreconciledPayePayments
                {
                    Receipt = new Receipt { Id = response.ReceiptId },
                    Month = payePeriod.Month,
                    Year = payePeriod.Year,
                    PaymentReference = model.PaymentRef,
                    ExpertSystem = expertSystem,
                    UnReconciledTaxEntity = new TaxEntity { Id = taxEntity.Id },
                });

                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseObject = new { model.PaymentRef, helperModel.Invoice.InvoiceNumber }
                };
            }
            catch (NoRecordFoundException)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.datamismatch().ToString(), FieldName = "Invoice" });
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, ErrorLang.norecord404().ToString() + exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.invoice404().ToString(), FieldName = "Invoice" });
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoiceFullyPaid().ToString() });
            }
            catch(PartPaymentNotAllowedException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.nopartpaymentsallow().ToString() });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Payment", ErrorMessage = ErrorLang.genericexception().ToString() });
            }

            return new APIResponse
            {
                ErrorCode = ErrorCode.PPIE.ToString(),
                Error = true,
                ResponseObject = errors,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }



        #region readycash

        /// <summary>
        /// Payment notification for readycash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse PaymentNotification(ReadyCashPaymentNotification model)
        {
            string message = Lang.remitapaymentnotificationok.ToString();
            try
            {
                //hmac_sha1(custid + ref +amt + date + channel)
                string value = model.InvoiceNumber + model.PaymentReference + model.AmountPaid + model.TransactionDate + model.Channel;
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.ReadycashExchangeKeyValue.ToString()).FirstOrDefault();

                var localHash = Util.HexHMACHash256(value, node.Value);

                if (localHash != model.Mac)
                {
                    Logger.Error(string.Format("Could not compute the model MAC : {0} | value : {1}", model.Mac, value));
                    return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.macauthfailed().ToString() } };
                }

                if (Math.Round(model.AmountPaid, 2) < 0.01m) { return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.invoiceamountistoosmall(model.AmountPaid).ToString() } }; }


                string bankCode = null;
                var banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
                var bankItem = banks.Where(b => b.Code == "100").FirstOrDefault();
                bankCode = bankItem.Code;
                //get channel
                PaymentChannel channel = GetReadyCashChannel(model.Channel);
                PaymentMethods methodType = GetReadyCashMethod(channel);

                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)channel,
                    InvoiceNumber = model.InvoiceNumber,
                    PaymentReference = model.TranId,
                    AmountPaid = Math.Round(model.AmountPaid, 2),
                    PaymentDate = DateTime.ParseExact(model.TransactionDate.Trim(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    TransactionDate = DateTime.ParseExact(model.TransactionDate.Trim(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    RequestDump = model.RequestDump,
                    Bank = bankItem.Name.ToUpper(),
                    BankCode = bankCode,
                    PaymentMethod = methodType.ToString(),
                    PaymentMethodId = (int)methodType,
                    PaymentProvider = (int)PaymentProvider.Readycash
                }, PaymentProvider.Readycash);

                return new APIResponse { ResponseObject = new { ResponseCode = Lang.bankcollectresponseokcode.ToString(), ResponseDescription = Lang.paymentnotificationsuccessful.ToString(), response.PayerName }, StatusCode = HttpStatusCode.OK, };
            }
            catch (PaymentNoficationAlreadyExistsException exception)
            {
                Logger.Information(string.Format("A payment with the given payment ref already exists exception message {0} | ref {1}", exception.Message, model.PaymentReference));
                message = Lang.paymentnotificationalreadyprocess.ToString();
                return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { ResponseCode = Lang.bankcollectresponseokcode.ToString(), ResponseDescription = message } };
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                message = ErrorLang.invoice404(model.InvoiceNumber).ToString();
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                message = ErrorLang.invoice404(model.InvoiceNumber).ToString();
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { ResponseCode = ErrorLang.readycasherrorcodeforalreadypaidforinvoice.ToString(), ResponseDescription = ErrorLang.invoiceFullyPaid(model.InvoiceNumber).ToString() } };
            }
            catch (PartPaymentNotAllowedException exception)
            {
                Logger.Error(exception, exception.Message);
                message = ErrorLang.nopartpaymentsallow().ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                message = ErrorLang.genericexception().ToString();
            }

            return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = message } };
        }

        private PaymentMethods GetReadyCashMethod(PaymentChannel channel)
        {
            switch (channel)
            {
                case PaymentChannel.POS:
                    return PaymentMethods.DebitCard;
                case PaymentChannel.Web:
                    return PaymentMethods.DebitCard;
                case PaymentChannel.MOB:
                    return PaymentMethods.BankTransfer;
                default:
                    return PaymentMethods.Cash;
            }
        }


        /// <summary>
        /// Get the corresponding channel for readycash
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>PaymentChannel</returns>
        private PaymentChannel GetReadyCashChannel(string channel)
        {
            switch (channel)
            {
                case "web":
                    return PaymentChannel.Web;
                case "mobile":
                    return PaymentChannel.MOB;
                case "bank":
                    return PaymentChannel.BankBranch;
                case "pos":
                    return PaymentChannel.POS;
                default:
                    return PaymentChannel.MOB;
            }
        }

        #endregion

        #region Bank Collect

        /// <summary>
        /// Payment notification for bank collect
        /// </summary>
        /// <param name="paymentController"></param>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        public APIResponse PaymentNotification(PaymentNotification model)
        {
            string message = Lang.remitapaymentnotificationok.ToString();
            try
            {
                //do HMAC validation
                string value = model.InvoiceNumber + model.PaymentRef;
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.InfoGridExchangeValue.ToString()).FirstOrDefault();

                var localHash = Util.HMACHash(value, node.Value);

                if (localHash != model.Mac)
                {
                    Logger.Error(string.Format("Could not compute the expected MAC : {0} | computed mac : {1} | value : {2}", model.Mac, localHash, value));
                    return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.macauthfailed().ToString() } };
                }

                if (Math.Round(model.AmountPaid, 2) < 0.01m) { return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.invoiceamountistoosmall(model.AmountPaid).ToString() } }; }

                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)PaymentChannel.BankBranch,
                    InvoiceNumber = model.InvoiceNumber,
                    PaymentReference = model.PaymentRef,
                    AmountPaid = Math.Round(model.AmountPaid, 2),
                    PaymentDate = DateTime.ParseExact(model.PaymentDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    TransactionDate = DateTime.ParseExact(model.TransactionDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    RequestDump = JsonConvert.SerializeObject(model),
                    Bank = model.BankName,
                    BankBranch = model.BankBranch,
                    BankCode = model.BankCode,
                    PaymentMethod = model.PaymentMethod,
                    PaymentProvider = (int)PaymentProvider.Bank3D
                }, PaymentProvider.Bank3D);
            }
            catch (PaymentNoficationAlreadyExistsException exception)
            {
                Logger.Information(string.Format("A payment with the given payment ref already exists exception message {0} | ref {1}", exception.Message, model.PaymentRef));
                message = Lang.paymentnotificationalreadyprocess.ToString();
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.invoice404(model.InvoiceNumber).ToString() } };
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.invoice404(model.InvoiceNumber).ToString() } };
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = Lang.invoicealreadypaid(model.InvoiceNumber).ToString() } };
            }
            catch (PartPaymentNotAllowedException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.nopartpaymentsallow().ToString() } };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.genericexception().ToString() } };
            }

            return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { ResponseCode = Lang.bankcollectresponseokcode.ToString(), ResponseDescription = message } };
        }

        #endregion


        /// <summary>
        /// Get the transaction with the given refs
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <param name="channel"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="exheaderParams"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetReadycashTransactionRequery(PaymentNotification model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if (model == null || string.IsNullOrEmpty(model.PaymentRef) || string.IsNullOrEmpty(model.Channel))
                {
                    errors.Add(new ErrorModel { FieldName = "Payment", ErrorMessage = ErrorLang.valuerequired("Payment").ToString() });
                    throw new Exception("Model is empty");
                }

                Logger.Information(string.Format("Payment requery request for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));
                ExternalPaymentProviderVM paymentProvider = _corePaymentProvider.Value.GetPaymentProvider(headerParams.CLIENTID);
                model.Signature = headerParams.SIGNATURE;
                return _readycashPaymentProviderHandler.Value.RequeryTransaction(model, paymentProvider);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                Logger.Error("Payment provider is inactive CLIENTID " + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.usernotauthorized().ToString() });
                errorCode = ResponseCodeLang.user_not_authorized;
            }
            catch (PaymentProvider404)
            {
                Logger.Error("Payment provider 404 CLIENTID " + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Provider", ErrorMessage = ErrorLang.norecord404().ToString() });
                errorCode = ResponseCodeLang.payment_provider_404;
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true,
                ErrorCode = errorCode
            };
        }


        public APIResponse PaymentNotification(PaymentNotification model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if(model == null)
                {
                    errors.Add(new ErrorModel { FieldName = "Payment", ErrorMessage = ErrorLang.valuerequired("Invoice").ToString() });
                    throw new Exception("Model is empty");
                }
                Logger.Information(string.Format("Payment notification request for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));
                ExternalPaymentProviderVM paymentProvider = _corePaymentProvider.Value.GetPaymentProvider(headerParams.CLIENTID);
                model.Signature = headerParams.SIGNATURE;
                return _paymentProviderHandler.Value.DoSynchronization(model, paymentProvider);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                Logger.Error("Payment provider is inactive CLIENTID " + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.usernotauthorized().ToString() });
                errorCode = ResponseCodeLang.user_not_authorized;
            }
            catch (PaymentProvider404)
            {
                Logger.Error("Payment provider 404 CLIENTID " + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Provider", ErrorMessage = ErrorLang.norecord404().ToString() });
                errorCode = ResponseCodeLang.payment_provider_404;
            }
            catch (NoRecordFoundException)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.datamismatch().ToString(), FieldName = "Payment" });
                errorCode = ResponseCodeLang.payment_data_mismatch;
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true,
                ErrorCode = errorCode
            };
        }

        #region PayDirect

        public PayDirectAPIResponseObj ProcessPaymentRequestForPayDirect()
        {
            return new PayDirectAPIResponseObj
            { ResponseObject = new CustomerInformationResponse { MerchantReference = "request.MerchantReference", Customers = new List<Customer> { { new Customer { Status = 1, CustReference = "request.CustReference", ThirdPartyCode = "request.ThirdPartyCode", StatusMessage = ErrorLang.merchantrefmismatch("request.MerchantReference").ToString() } } } }, StatusCode = System.Net.HttpStatusCode.OK, ReturnType = "CustomerInformationResponse" };

        }


        /// <summary>
        /// Process request for pay direct
        /// </summary>
        /// <param name="requestStreamString"></param>
        /// <returns>APIResponse</returns>
        public PayDirectAPIResponseObj ProcessPaymentRequestForPayDirect(string requestStreamString, PayDirectConfigurations config, bool flatScheme = false)
        {
            //instantiating the pay direct lib
            _payDirect = new PayDirect(config) { };

            Functions function = Functions.None;
            //we first want to determine what the request is for
            //whether it is a customer information (validation) or payment notification (synchronization)
            try
            {
                function = _payDirect.GetRequestFunction(requestStreamString);
                return PayDirectProcessResponse(function, requestStreamString, flatScheme);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting pay direct request function. {0}", requestStreamString));
                return new PayDirectAPIResponseObj { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.paydirectrequesttype404().ToString() };
            }
        }


        private PayDirectAPIResponseObj PayDirectProcessResponse(Functions function, string requestStreamString, bool flatScheme = false)
        {
            switch (function)
            {
                case Functions.PayDirectValidation:
                    return ProcessValidationRequest(requestStreamString, flatScheme);
                case Functions.PayDirectNotification:
                    return PaymentNotificationResponse(requestStreamString, flatScheme);
                default:
                    throw new NotImplementedException("No valid pay direct function.");
            }
        }


        /// <summary>
        /// process pay direct customer information request
        /// </summary>
        /// <param name="requestStreamString"></param>
        /// <returns>APIResponse</returns>
        protected PayDirectAPIResponseObj ProcessValidationRequest(string requestStreamString, bool flatScheme = false)
        {
            CustomerInformationRequest request = null;
            try
            {
                try
                {
                    request = _payDirect.DeserializeXMLRequest<CustomerInformationRequest>(requestStreamString);
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("Error deserializing request stream"));
                    return new PayDirectAPIResponseObj { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.errordeserializingpaydirectrequest().ToString() };
                }

                if (request == null)
                {
                    Logger.Error("Paydirect request stream string deserialized is null " + requestStreamString);
                    return new PayDirectAPIResponseObj
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        ResponseObject = new CustomerInformationResponse { MerchantReference = request.MerchantReference, Customers = new List<Customer> { { new Customer { StatusMessage = ErrorLang.errordeserializingpaydirectrequest().ToString(), Status = 1, Amount = 0.0m, CustReference = request.CustReference, FirstName = string.Empty, } } } },
                        ReturnType = "CustomerInformationResponse"
                    };
                }

                try
                {
                    if (!_payDirect.ValidateMerchantReference(request.MerchantReference, flatScheme))
                    {
                        Logger.Error(string.Format("Could not confirm merchant ref {0}", request.MerchantReference));
                        return new PayDirectAPIResponseObj
                        {
                            ResponseObject = new CustomerInformationResponse
                            {
                                MerchantReference = request.MerchantReference,
                                Customers = new List<Customer> { { new Customer { Status = 1, CustReference = request.CustReference, ThirdPartyCode = request.ThirdPartyCode, StatusMessage = ErrorLang.merchantrefmismatch(request.MerchantReference).ToString(), Amount = 0.0m, FirstName = string.Empty, } } }
                            },
                            StatusCode = System.Net.HttpStatusCode.OK,
                            ReturnType = "CustomerInformationResponse"
                        };
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("IE Error getting merchant ref {0}", request.MerchantReference));
                    return new PayDirectAPIResponseObj { ResponseObject = new CustomerInformationResponse { MerchantReference = request.MerchantReference, Customers = new List<Customer> { { new Customer { Status = 1, CustReference = request.CustReference, ThirdPartyCode = request.ThirdPartyCode, StatusMessage = ErrorLang.couldnotgetmerchantref(request.MerchantReference).ToString(), Amount = 0.0m, FirstName = string.Empty, } } } }, StatusCode = HttpStatusCode.OK, ReturnType = "CustomerInformationResponse" };
                }

                //now validation checks are over, lets check if we have this invoice number
                if (string.IsNullOrEmpty(request.CustReference))
                {
                    Logger.Information(string.Format("PAY DIRECT ::: Cust ref is empty.No invoice found for pay direct invoice number : {0} ", request.CustReference));
                    return new PayDirectAPIResponseObj { ResponseObject = new CustomerInformationResponse { MerchantReference = _payDirect.GetMerchantRef(), Customers = new List<Customer> { { new Customer { Status = 1, ThirdPartyCode = request.ThirdPartyCode, StatusMessage = ErrorLang.invoice404(request.CustReference).ToString(), CustReference = request.CustReference, Amount = 0.0m, FirstName = string.Empty, } } } }, StatusCode = System.Net.HttpStatusCode.OK, ReturnType = "CustomerInformationResponse" };
                }
                InvoiceGeneratedResponseExtn invoiceDetails = _corePaymentService.GetInvoiceDetails(request.CustReference);

                if (invoiceDetails == null)
                {
                    Logger.Information(string.Format("PAY DIRECT ::: InvoiceDetails null value: No invoice found for pay direct invoice number : {0} ", request.CustReference));
                    return new PayDirectAPIResponseObj
                    { StatusCode = HttpStatusCode.OK, ResponseObject = new CustomerInformationResponse { Customers = new List<Customer> { { new Customer { CustReference = request.CustReference, Status = 1, StatusMessage = ErrorLang.invoice404(request.CustReference).ToString(), ThirdPartyCode = request.ThirdPartyCode, Amount = 0.0m, FirstName = string.Empty } } }, MerchantReference = request.MerchantReference }, ReturnType = "CustomerInformationResponse" };
                }

                //do check if validation check is required
                if (invoiceDetails.HasPaymentProviderValidationConstraint)
                {
                    if (DoValidationConstraintCheck(invoiceDetails.MDAId, invoiceDetails.RevenueHeadID, PaymentProvider.PayDirect))
                    {
                        Logger.Information(string.Format("PAY DIRECT ::: is restricted from MDA {0} and Revenue head {1} ", invoiceDetails.MDAId, invoiceDetails.RevenueHeadID));
                        return new PayDirectAPIResponseObj
                        { StatusCode = HttpStatusCode.OK, ResponseObject = new CustomerInformationResponse { Customers = new List<Customer> { { new Customer { CustReference = request.CustReference, Status = 1, StatusMessage = ErrorLang.usernotauthorized().ToString(), ThirdPartyCode = request.ThirdPartyCode, Amount = 0.0m, FirstName = string.Empty } } }, MerchantReference = request.MerchantReference }, ReturnType = "CustomerInformationResponse" };
                    }
                }

                PayDirectAPIResponseObj settlementTypeCheck = CheckSettlementType(request.CustReference, request.MerchantReference, request.ThirdPartyCode, flatScheme, invoiceDetails.MDASettlementType, invoiceDetails.RevenueHeadSettlementType);
                if (settlementTypeCheck != null) return settlementTypeCheck;
                ///
                string productItemCode = string.IsNullOrEmpty(invoiceDetails.RevenueHeadSettlementCode) ? invoiceDetails.MDASettlementCode : invoiceDetails.RevenueHeadSettlementCode;

                if (string.IsNullOrEmpty(productItemCode))
                {
                    productItemCode = string.IsNullOrEmpty(productItemCode) ? _payDirect.GetProductId() : request.PaymentItemCode;
                }

                return new PayDirectAPIResponseObj
                { StatusCode = HttpStatusCode.OK, ResponseObject = new CustomerInformationResponse { Customers = new List<Customer> { { new Customer { CustReference = invoiceDetails.InvoiceNumber, FirstName = invoiceDetails.Recipient, Amount = invoiceDetails.AmountDue, PaymentItems = new List<Item> { { new Item { Price = invoiceDetails.AmountDue, ProductName = invoiceDetails.RevenueHeadName, Quantity = 1, Subtotal = invoiceDetails.AmountDue, Total = invoiceDetails.AmountDue, ProductCode = productItemCode, } } } } } }, MerchantReference = request.MerchantReference }, ReturnType = "CustomerInformationResponse" };

            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("PAY DIRECT ::: InvoiceDetails Exception invoice number : {0} ", request.CustReference));
                return new PayDirectAPIResponseObj
                { StatusCode = HttpStatusCode.OK, ResponseObject = new CustomerInformationResponse { Customers = new List<Customer> { { new Customer { CustReference = request.CustReference, Status = 1, StatusMessage = ErrorLang.genericexception().ToString(), ThirdPartyCode = request.ThirdPartyCode, Amount = 0.0m, FirstName = string.Empty } } }, MerchantReference = request.MerchantReference }, ReturnType = "CustomerInformationResponse" };
            }
        }


        /// <summary>
        /// do validation constraint check
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="revenueHeadID"></param>
        /// <param name="payDirect"></param>
        /// <returns>bool</returns>
        private bool DoValidationConstraintCheck(int mdaId, int revenueHeadID, PaymentProvider payDirect)
        {
            return _coreInvoiceService.CheckForValidationConstraint(mdaId, revenueHeadID, (int)payDirect);
        }


        /// <summary>
        /// check that the settlement type match the store settlement type
        /// <para>This method will return null if the settlement type tally</para>
        /// </summary>
        /// <param name="custRef"></param>
        /// <param name="merchRef"></param>
        /// <param name="thirdPartyCode"></param>
        /// <param name="flatScheme"></param>
        /// <param name="mDASettlementType"></param>
        /// <param name="revenueHeadSettlementType"></param>
        /// <returns>PayDirectAPIResponseObj</returns>
        private PayDirectAPIResponseObj CheckSettlementType(string custRef, string merchRef, string thirdPartyCode, bool flatScheme, int mDASettlementType, int revenueHeadSettlementType)
        {
            string msg = string.Empty;
            try
            {
                int settlementType = revenueHeadSettlementType == ((int)SettlementType.None) ? mDASettlementType : revenueHeadSettlementType;
                if (settlementType == ((int)SettlementType.None))
                {
                    msg = string.Format("PAY DIRECT ::: settlement scheme not specified {0} {1}", (SettlementType)settlementType, custRef);
                    throw new UserNotAuthorizedForThisActionException(msg);
                }

                if (flatScheme)
                {
                    if (settlementType != (int)SettlementType.Flat)
                    {
                        msg = string.Format("PAY DIRECT ::: scheme is flat but settlement type is {0} {1} ", (SettlementType)settlementType, custRef);
                        throw new UserNotAuthorizedForThisActionException(msg);
                    }
                }
                else
                {
                    if (settlementType != (int)SettlementType.Percentage)
                    {
                        msg = string.Format("PAY DIRECT ::: scheme is percentage but settlement type is {0} ", (SettlementType)settlementType);
                        throw new UserNotAuthorizedForThisActionException();
                    }
                }
                return null;
            }
            catch(UserNotAuthorizedForThisActionException exception)
            { Logger.Error("Customer ref : " + custRef + " " + exception.Message); }
            catch (Exception exception)
            { Logger.Error(exception, "Customer ref : " + custRef + " " + exception.Message); }

            return new PayDirectAPIResponseObj
            { StatusCode = HttpStatusCode.OK, ResponseObject = new CustomerInformationResponse { Customers = new List<Customer> { { new Customer { CustReference = custRef, Status = 1, StatusMessage = ErrorLang.usernotauthorized().ToString(), ThirdPartyCode = thirdPartyCode, Amount = 0.0m, FirstName = string.Empty } } }, MerchantReference = merchRef }, ReturnType = "CustomerInformationResponse" };
        }



        private PayDirectAPIResponseObj PaymentNotificationResponse(string requestStreamString, bool flatScheme)
        {
            //deserialize request
            PaymentNotificationRequest request = null;
            try
            {
                Logger.Debug(string.Format("Deserializing the request stream string for payment notification {0}", requestStreamString));
                //deserialize request
                request = _payDirect.DeserializeXMLRequest<ThirdParty.Payment.Processor.Models.PaymentNotificationRequest>(requestStreamString);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error deserializing request stream for payment notification"));
                return new PayDirectAPIResponseObj
                { StatusCode = HttpStatusCode.BadRequest, ResponseObject = ErrorLang.errordeserializingpaydirectrequest().ToString() };
            }

            if (request == null)
            {
                Logger.Error("Paydirect request stream string deserialized is null for payment notification " + requestStreamString);
                return new PayDirectAPIResponseObj
                { StatusCode = HttpStatusCode.BadRequest, ResponseObject = ErrorLang.errordeserializingpaydirectrequest().ToString() };
            }

            //validate service URL

            List<PaymentResponse> payDirectPaymentsResponse = new List<PaymentResponse> { };

            foreach (var item in request.Payments)
            {
                try
                {
                    if (string.IsNullOrEmpty(item.PaymentReference))
                    {
                        Logger.Error("PAY DIRECT NOTIF :: PAYMENT ref IS EMPTY");
                        payDirectPaymentsResponse.Add(new PaymentResponse
                        { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.valuerequired("PaymentReference").ToString() });
                        continue;
                    }

                    if (string.IsNullOrEmpty(item.PaymentLogId))
                    {
                        Logger.Error("PAY DIRECT NOTIF :: PAYMENTLOGID IS EMPTY");
                        payDirectPaymentsResponse.Add(new PaymentResponse
                        { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.valuerequired("PaymentLogId").ToString() });
                        continue;
                    }

                    //first we need to check if this is a reversal or not
                    if (item.IsReversal)
                    {
                        payDirectPaymentsResponse.Add(ProcessPayDirectReversal(item, flatScheme));
                    }
                    else
                    {
                        payDirectPaymentsResponse.Add(AddPayDirectPaymentNotification(item));
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, exception.Message);
                    return new PayDirectAPIResponseObj { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.genericexception().ToString() } };
                }
            }

            return new PayDirectAPIResponseObj
            {
                StatusCode = HttpStatusCode.OK,
                ResponseObject = new PaymentNotificationResponse { Payments = payDirectPaymentsResponse },
                ReturnType = "PaymentNotificationResponse"
            };
        }


        /// <summary>
        /// Process payment notification for paydirect
        /// <para>Do string validation for paymentlogid and payment ref before calling this method</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>PaymentResponse</returns>
        protected PaymentResponse AddPayDirectPaymentNotification(Payment item)
        {
            try
            {
                if (Math.Round(item.Amount, 2) < 0.01m)
                {
                    Logger.Error("PAY DIRECT NOTIF :: PAYMENT item amount is too small");
                    return new PaymentResponse
                    { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.invoiceamountistoosmall(item.Amount).ToString() };
                }

                InvoiceDetailsHelperModel helperModel = _coreInvoiceService.GetInvoiceHelperDetails(item.CustReference);
                if (helperModel == null)
                {
                    throw new NoInvoicesMatchingTheParametersFoundException(string.Format("Invoice not found {0}", item.CustReference));
                }

                //check for paymentlogid
                TransactionLogGroup byPaymentLogId = _corePaymentService.GetTransactionLogByPaymentLogId(item.PaymentLogId, PaymentProvider.PayDirect);

                if (byPaymentLogId != null)
                {
                    //check for amount tally
                    if (byPaymentLogId.TotalAmountPaid != item.Amount)
                    {
                        Logger.Error("PAY DIRECT NOTIF:  amount mismatch log id " + byPaymentLogId.PaymentLogId);
                        return new PaymentResponse
                        { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.amountmismatchforexistingpaymentlogid().ToString() };
                    }

                    //check payment ref tally
                    if (byPaymentLogId.PaymentReference != item.PaymentReference)
                    {
                        Logger.Error("PAY DIRECT NOTIF: payment ref mismaych ref " + byPaymentLogId.PaymentReference);
                        return new PaymentResponse { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.paymentrefmismatchforexistingpaymentid().ToString() };
                    }

                    //check receipt number
                    if (byPaymentLogId.ThirdPartyReceiptNumber != item.ReceiptNo)
                    {
                        Logger.Error("PAY DIRECT NOTIF: " + ErrorLang.receiptmismatchforexistingpaymentid(item.ReceiptNo, byPaymentLogId.ThirdPartyReceiptNumber).ToString());
                        return new PaymentResponse { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.receiptmismatchforexistingpaymentid(item.ReceiptNo, byPaymentLogId.ThirdPartyReceiptNumber).ToString() };
                    }
                    //do check here
                    //we are checking that the record for paymentref was for the same invoice and the same tax profile
                    if (byPaymentLogId.InvoiceId == helperModel.Invoice.Id && byPaymentLogId.TaxEntityId == helperModel.TaxEntityId)
                    {
                        Logger.Information("Pay Direct ::: Payment log Id has already been processed. Log Id: " + item.PaymentLogId);
                        return new PaymentResponse
                        { Status = 0, PaymentLogId = item.PaymentLogId, StatusMessage = Lang.paymentnotificationalreadyprocess.ToString() };
                    }
                    else
                    {
                        throw new NoInvoicesMatchingTheParametersFoundException(string.Format("The invoice associated with the duplicate payment log id {0} provider - {1} invoiceId - {2} does not match the invoice from the invoice number {3} and invoice Id {4}", byPaymentLogId.PaymentLogId, PaymentProvider.PayDirect.ToString(), byPaymentLogId.InvoiceId, item.CustReference, helperModel.Invoice.Id));
                    }
                }

                //get transaction log by payment ref
                TransactionLogGroup byPaymentRef = _corePaymentService.GetByPaymentRef(item.PaymentReference, PaymentProvider.PayDirect);

                if (byPaymentRef != null)
                {
                    //check for amount tally
                    if (byPaymentRef.TotalAmountPaid != item.Amount)
                    {
                        Logger.Error("PAY DIRECT NOTIF Pay ref : " + ErrorLang.amountmismatchforexistingpaymentref(item.Amount.ToString(), byPaymentRef.TotalAmountPaid.ToString()).ToString());
                        return new PaymentResponse { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.amountmismatchforexistingpaymentref(item.Amount.ToString(), byPaymentRef.TotalAmountPaid.ToString()).ToString() };
                    }

                    //check payment log id tally
                    if (byPaymentRef.PaymentLogId != item.PaymentLogId)
                    {
                        Logger.Error("PAY DIRECT NOTIF Pay ref : " + ErrorLang.paymentlogidmismatchforexistingpaymentref(item.PaymentLogId, byPaymentRef.PaymentLogId).ToString());
                        return new PaymentResponse { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.paymentlogidmismatchforexistingpaymentref(item.PaymentLogId, byPaymentRef.PaymentLogId).ToString() };
                    }

                    //check receipt number
                    if (byPaymentLogId.ThirdPartyReceiptNumber != item.ReceiptNo)
                    {
                        Logger.Error("PAY DIRECT NOTIF Pay ref : " + ErrorLang.receiptmismatchforexistingpaymentid(item.ReceiptNo, byPaymentLogId.ThirdPartyReceiptNumber).ToString());
                        return new PaymentResponse { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.receiptmismatchforexistingpaymentid(item.ReceiptNo, byPaymentLogId.ThirdPartyReceiptNumber).ToString() };
                    }

                    //we are checking that the record for paymentref was for the same invoice and the same tax profile
                    if (byPaymentLogId.InvoiceId == helperModel.Invoice.Id && byPaymentLogId.TaxEntityId == helperModel.TaxEntityId)
                    {
                        Logger.Information("Pay Direct ::: Payment log Id has already been processed. Log Id: " + item.PaymentLogId);
                        return new PaymentResponse
                        { Status = 0, PaymentLogId = item.PaymentLogId, StatusMessage = Lang.paymentnotificationalreadyprocess.ToString() };
                    }
                    else
                    {
                        throw new NoInvoicesMatchingTheParametersFoundException(string.Format("The invoice associated with the duplicate payment ref id {0} provider - {1} invoiceId - {2} does not match the invoice from the invoice number {3} and invoice Id {4}", byPaymentLogId.PaymentReference, PaymentProvider.PayDirect.ToString(), byPaymentLogId.InvoiceId, item.CustReference, helperModel.Invoice.Id));
                    }
                }

                string bankCode = null;
                var banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
                var bankItem = banks.Where(b => b.PayDirectBankCode == item.BankCode).FirstOrDefault();
                if (bankItem != null) { bankCode = bankItem.Code; }
                //get channel type
                PaymentChannel channelType = GetChannelTypeForPayDirect(item.ChannelName);

                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)channelType,
                    InvoiceNumber = helperModel.Invoice.InvoiceNumber,
                    PaymentReference = item.PaymentReference,
                    AmountPaid = item.Amount,
                    PaymentDate = DateTime.ParseExact(item.PaymentDate.Trim(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    TransactionDate = DateTime.ParseExact(item.SettlementDate.Trim(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    RequestDump = JsonConvert.SerializeObject(item),
                    Bank = item.BankName,
                    BankBranch = item.BranchName,
                    BankCode = bankCode,
                    BankChannel = item.ChannelName,
                    PayerName = item.DepositorName,
                    ThirdPartyReceiptNumber = item.ReceiptNo,
                    SlipNumber = item.DepositSlipNumber,
                    TellerName = item.Teller,
                    PayerPhoneNumber = item.CustomerPhoneNumber,
                    PayerAddress = item.CustomerAddress,
                    TypeID = (int)PaymentType.Credit,
                    PaymentMethod = item.PaymentMethod,
                    PaymentLogId = item.PaymentLogId,
                    PaymentProvider = (int)PaymentProvider.PayDirect,
                    TotalAmountPaid = item.Amount,
                }, PaymentProvider.PayDirect, false, helperModel);

                return new PaymentResponse
                { Status = 0, PaymentLogId = item.PaymentLogId, StatusMessage = Lang.remitapaymentnotificationok.ToString() };
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new PaymentResponse
                { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.invoice404(item.CustReference).ToString() };
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                return new PaymentResponse
                { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.invoiceFullyPaid().ToString() };
            }
            catch (PartPaymentNotAllowedException exception)
            {
                Logger.Error(exception, exception.Message);
                return new PaymentResponse
                { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.nopartpaymentsallow().ToString() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new PaymentResponse
                { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.genericexception().ToString() };
            }
        }


        /// <summary>
        /// Mapp channel name to payment channel value
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns>PaymentChannel</returns>
        private PaymentChannel GetChannelTypeForPayDirect(string channelName)
        {
            switch (channelName)
            {
                case "Bank Branc":
                    return PaymentChannel.BankBranch;
                case "ATM":
                    return PaymentChannel.ATM;
                case "POS":
                    return PaymentChannel.POS;
                case "Web":
                    return PaymentChannel.Web;
                case "MOB":
                    return PaymentChannel.MOB;
                case "Kiosk":
                    return PaymentChannel.Kiosk;
                case "Voice":
                    return PaymentChannel.Voice;
                default:
                    return PaymentChannel.OtherChannels;
            }
        }


        /// <summary>
        /// Process pay direct reversal
        /// </summary>
        /// <param name="item"></param>
        /// <returns>PaymentResponse</returns>
        protected PaymentResponse ProcessPayDirectReversal(Payment item, bool flatScheme)
        {
            try
            {
                //validate institution Id
                try
                {
                    if (!_payDirect.IsInstitutionIdValid(item.InstitutionId, flatScheme))
                    {
                        Logger.Error("PAY DIRECT Payment notif ::: No institution Id found " + item.InstitutionId);
                        return new PaymentResponse
                        { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.institutionId404(item.InstitutionId).ToString() };
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("PAY DIRECT ::: Exception Error checking for valid service URL  Payment notif"));
                    return new PaymentResponse
                    { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = ErrorLang.institutionId404(item.InstitutionId).ToString() };
                }

                if (string.IsNullOrEmpty(item.OriginalPaymentLogId))
                {
                    Logger.Error(string.Format("PAY DIRECT rev ::: OriginalPaymentLogId is empty"));
                    return new PaymentResponse
                    { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.valuerequired("OriginalPaymentLogId").ToString() };
                }

                if (string.IsNullOrEmpty(item.OriginalPaymentReference))
                {
                    Logger.Error(string.Format("PAY DIRECT rev ::: OriginalPaymentReference is empty"));
                    return new PaymentResponse
                    { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.valuerequired("OriginalPaymentReference").ToString() };
                }

                if (item.Amount >= 0.00m)
                {
                    Logger.Error(string.Format("PAY DIRECT rev ::: Amount is >= 0.00m"));
                    return new PaymentResponse
                    { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.transactionreversalisnotnegative(item.Amount.ToString()).ToString() };
                }

                var channelType = GetChannelTypeForPayDirect(item.ChannelName);
                string bankCode = null;
                var banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
                var bankItem = banks.Where(b => b.PayDirectBankCode == item.BankCode).FirstOrDefault();
                if (bankItem != null) { bankCode = bankItem.Code; }

                PaymentReversalResponseObj response = _corePaymentService.PaymentReversalForPayDirect(new TransactionLogVM
                {
                    Channel = (int)channelType,
                    InvoiceNumber = item.CustReference,
                    PaymentReference = item.PaymentReference,
                    AmountPaid = item.Amount,
                    PaymentDate = DateTime.ParseExact(item.PaymentDate.Trim(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    TransactionDate = DateTime.ParseExact(item.SettlementDate.Trim(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    RequestDump = JsonConvert.SerializeObject(item),
                    Bank = item.BankName,
                    BankBranch = item.BranchName,
                    BankCode = bankCode,
                    BankChannel = item.ChannelName,
                    PayerName = item.DepositorName,
                    ThirdPartyReceiptNumber = item.ReceiptNo,
                    SlipNumber = item.DepositSlipNumber,
                    TellerName = item.Teller,
                    PayerPhoneNumber = item.CustomerPhoneNumber,
                    PayerAddress = item.CustomerAddress,
                    TypeID = (int)PaymentType.Debit,
                    PaymentMethod = item.PaymentMethod,
                    PaymentLogId = item.PaymentLogId,
                    OriginalPaymentLogID = item.OriginalPaymentLogId,
                    OriginalPaymentReference = item.OriginalPaymentReference,
                    PaymentProvider = (int)PaymentProvider.PayDirect,
                }, item.OriginalPaymentReference, item.OriginalPaymentLogId);

                if (response.HasError)
                { return new PaymentResponse { Status = 1, PaymentLogId = item.PaymentLogId, StatusMessage = response.ErrorMessage }; }

                return new PaymentResponse
                { Status = 0, StatusMessage = response.Message, PaymentLogId = item.PaymentLogId };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new PaymentResponse
                { PaymentLogId = item.PaymentLogId, Status = 1, StatusMessage = ErrorLang.genericexception().ToString() };
            }
        }

        #endregion


        #region NIBSS EBills Pay

        /// <summary>
        /// Payment notification for NIBSS EBills pay
        /// </summary>
        /// <param name="requestStreamString"></param>
        /// <returns>APIResponse</returns>
        public APIResponse NIBSSPaymentNotif(string requestStreamString)
        {
            //NotificationRequest
            NotificationRequest notifObj = null;
            NotificationResponse notifResponse = null;
            Param invoiceObj = null;

            try
            {
                //if string is empty
                if (string.IsNullOrEmpty(requestStreamString))
                {
                    return new APIResponse
                    {
                        ResponseObject = new NotificationResponse
                        { ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.ErrorCode), ResponseMessage = ErrorLang.badrequest().ToString(), Param = new List<Param> { } },
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                //deserialize the request stream string
                notifObj = _iNIBSSEBillsPay.DeserializeXMLRequest<NotificationRequest>(requestStreamString);

                if (notifObj == null)
                {
                    Logger.Error("Could not deserialize the request stream " + requestStreamString);
                    return new APIResponse
                    {
                        ResponseObject = new NotificationResponse
                        { ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.ErrorCode), ResponseMessage = ErrorLang.badrequest().ToString(), Param = new List<Param> { } },
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                notifResponse = new NotificationResponse { SessionID = notifObj.SessionID, BillerID = notifObj.BillerID, ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.ErrorCode) };

                if (string.IsNullOrEmpty(notifObj.SessionID) || string.IsNullOrEmpty(notifObj.BillerID))
                {
                    notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SessionID404);//Invalid Session or Record ID
                    notifResponse.ResponseMessage = "Invalid Session or Record ID.";
                    notifResponse.Param = new List<Param> { };
                    throw new Exception("SessionID field is empty " + ErrorLang.norecord404().ToString());
                }

                //do auth validation
                DoRequestValidation(notifObj, notifResponse);

                //get the invoice number
                invoiceObj = notifObj.Param.Where(x => x.Key == "InvoiceNumber").SingleOrDefault();
                string phoneNumber = notifObj.Param.Where(x => x.Key == "PhoneNumber").FirstOrDefault()?.Value;
                string email = notifObj.Param.Where(x => x.Key == "Email").FirstOrDefault()?.Value;

                if (invoiceObj == null || string.IsNullOrEmpty(invoiceObj.Value))
                {
                    notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.FormatError);//Format error
                    notifResponse.ResponseMessage = ErrorLang.norecord404().ToString();
                    notifResponse.Param = new List<Param> { };
                    throw new Exception("invoice object not found in the validation object");
                }

                var banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
                var bankItem = banks.Where(b => b.Code == notifObj.SourceBankCode?.Trim()).FirstOrDefault();

                if (bankItem == null)
                {
                    notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BankCode404);//Unknown Bank Code
                    notifResponse.ResponseMessage = "Unknown Bank Code.";
                    notifResponse.Param = new List<Param> { };
                    throw new Exception("Unknown Bank Code. " + ErrorLang.bankCode404(notifObj.SourceBankCode).ToString());
                }

                PaymentChannel channel = GetChannelTypeForNIBSS(notifObj.ChannelCode);

                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)channel,
                    InvoiceNumber = invoiceObj.Value,
                    PaymentReference = notifObj.SessionID,
                    AmountPaid = notifObj.TotalAmount,
                    PaymentDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(notifObj.TransactionInitiatedDate)),
                    TransactionDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(notifObj.TransactionApprovalDate)),
                    RequestDump = JsonConvert.SerializeObject(notifObj),
                    Bank = bankItem.Name,
                    BankCode = bankItem.Code,
                    PayerName = notifObj.CustomerName,
                    PayerPhoneNumber = phoneNumber,
                    TypeID = (int)PaymentType.Credit,
                    Fee = notifObj.Fee,
                    TotalAmountPaid = notifObj.TotalAmount,
                    PaymentProvider = (int)PaymentProvider.NIBSS,
                    BankChannel = notifObj.ChannelCode,
                }, PaymentProvider.NIBSS);


                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.OK);
                notifResponse.ResponseMessage = Lang.paymentnotificationsuccessful.ToString();
                notifResponse.Param = new List<Param>
                {
                    { new Param { Key = "InvoiceNumber", Value = response.InvoiceNumber } },
                    { new Param { Key = "Amount", Value = notifObj.TotalAmount.ToString() } },
                };
            }
            catch (NoRecordFoundException exception)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.ErrorCode);
                notifResponse.ResponseMessage = ErrorLang.datamismatch().ToString();
                notifResponse.Param = new List<Param>
                {
                    { new Param { Key = "InvoiceNumber", Value = invoiceObj.Value } },
                };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.Record404);
                notifResponse.ResponseMessage = ErrorLang.norecord404().ToString();
                notifResponse.Param = new List<Param>
                {
                    { new Param { Key = "InvoiceNumber", Value = invoiceObj.Value } },
                };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (PaymentNoficationAlreadyExistsException exception)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.OK);
                notifResponse.ResponseMessage = Lang.paymentnotificationalreadyprocess.ToString();
                notifResponse.Param = new List<Param>
                {
                    { new Param { Key = "InvoiceNumber", Value = invoiceObj.Value } },
                    { new Param { Key = "Amount", Value = notifObj.Amount.ToString() } },
                };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.ErrorCode);
                notifResponse.ResponseMessage = ErrorLang.invoiceFullyPaid(invoiceObj.Value).ToString();
                notifResponse.Param = new List<Param>
                {
                    { new Param { Key = "InvoiceNumber", Value = invoiceObj.Value } },
                };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (PartPaymentNotAllowedException exception)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.ErrorCode);
                notifResponse.ResponseMessage = ErrorLang.nopartpaymentsallow().ToString();
                notifResponse.Param = new List<Param>
                {
                    { new Param { Key = "InvoiceNumber", Value = invoiceObj.Value } },
                };
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception NIBSSPaymentNotif" + exception.Message);
            }
            return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = notifResponse };
        }

        /// <summary>
        /// Do request validation for NIBSS
        /// </summary>
        /// <param name="notifObj"></param>
        /// <param name="notifResponse"></param>
        private void DoRequestValidation(NotificationRequest notifObj, NotificationResponse notifResponse)
        {
            var stateName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig stateconfig = Util.GetTenantConfigBySiteName(stateName);

            if (stateconfig == null)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.InternalError);//Format error
                notifResponse.ResponseMessage = ErrorLang.genericexception().ToString();
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("No state config found. From Request {0}. State {1}", notifObj.BillerID, stateName));
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
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                notifResponse.ResponseMessage = ErrorLang.billinginfo404().ToString();
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("No biller Id entry found in config {0}. State {1}", notifObj.BillerID, stateName));
            }

            if (notifObj.BillerID != billerIdNode.Value)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                notifResponse.ResponseMessage = ErrorLang.billinginfo404().ToString();
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("BillerID mismatch expected : {0}, actual : {1}", billerIdNode.Value, notifObj.BillerID));
            }

            //check for biller name
            if (billeNameNode == null)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                notifResponse.ResponseMessage = ErrorLang.billinginfo404().ToString();
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("No biller found in config {0}.", notifObj.BillerName));
            }

            if (notifObj.BillerName != billeNameNode.Value)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                notifResponse.ResponseMessage = ErrorLang.billinginfo404().ToString();
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("billeNameNode mismatch expected : {0}, actual : {1}", billeNameNode.Value, notifObj.BillerName));
            }

            //check for product id
            if (productIdNode == null)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                notifResponse.ResponseMessage = ErrorLang.billinginfo404().ToString();
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("No product id entry found in {0} config", stateName));
            }

            if (notifObj.ProductID != productIdNode.Value)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                //notifResponse.ResponseMessage = ErrorLang.productinfo404().ToString();
                notifResponse.ResponseMessage = "No product information was found.";
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("product id mismatch expected : {0}, actual : {1}", productIdNode.Value, notifObj.ProductID));
            }

            //check product name
            if (productNameNode == null)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                //notifResponse.ResponseMessage = ErrorLang.productinfo404().ToString();
                notifResponse.ResponseMessage = "No product information was found.";
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("No product name entry found in config. State {0}", stateName));
            }

            if (notifObj.ProductName != productNameNode.Value)
            {
                notifResponse.ResponseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                //notifResponse.ResponseMessage = ErrorLang.productinfo404().ToString();
                notifResponse.ResponseMessage = "No product information was found.";
                notifResponse.Param = new List<Param> { };
                throw new Exception(string.Format("product name mismatch expected : {0}, actual : {1}", productNameNode.Value, notifObj.ProductName));
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

        /// <summary>
        /// Payment notification for NetPay card collection
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse NetPayPaymentNotification(PaymentController callback, NetPayTransactionVM model)
        {
            string message = Lang.netpaypaymentnotificationok.ToString();
            try
            {
                //do HMAC validation
                string value = model.MerchantRef + model.CurrencyCode + model.Amount;
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.NetPayMerchantSecretId.ToString()).FirstOrDefault();

                if (model.Code != AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.NetPaySuccessCode))
                {
                    return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { ResponseCode = Lang.netpayresponseokcode.ToString(), ResponseDescription = "Logged in file" } };
                }

                var localHash = Util.HexHMACHash256(value, node.Value);
                if (localHash != model.HMac)
                {
                    Logger.Error(string.Format("NetPay notification:::Could not compute the expected MAC : {0} | computed mac : {1} | value : {2}", model.HMac, localHash, value));
                    return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.netpayerrorcode().ToString(), ResponseDescription = ErrorLang.macauthfailed().ToString() } };
                }

                string invoiceNumber = string.Empty;
                //Check if it is old reference, new reference length is currently not longer than 10
                if(!model.MerchantRef.Contains("-"))
                {
                    int invoiceNumberLength = 10;
                    //Get the last ten digits of the merchantref from NetPay since Invoice number is 10 digits
                    invoiceNumber = model.MerchantRef.Substring((model.MerchantRef.Length - invoiceNumberLength), invoiceNumberLength);
                    InvoiceDetailsHelperModel invoiceDetails = _coreInvoiceService.GetInvoiceHelperDetails(invoiceNumber);
                    if (invoiceDetails == null)
                    {
                        Logger.Error($"Invoice Number {invoiceNumber} not found");
                        throw new NoRecordFoundException($"Invoice Number {invoiceNumber} not found");
                    }
                }
                else
                {
                    //Get the Invoice details attached to this Payment Reference
                    PaymentReferenceVM paymentReferenceDetail = _corePaymentService.GetPaymentReferenceDetail(model.MerchantRef);
                    if (paymentReferenceDetail == null)
                    {
                        Logger.Error($"Payment Reference {model.MerchantRef} not found");
                        throw new NoRecordFoundException($"Payment Reference {model.MerchantRef} not found");
                    }
                    invoiceNumber = paymentReferenceDetail.InvoiceNumber;
                }

                if (Math.Round((model.Amount / 100), 2) < 0.01m) { return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.netpayerrorcode().ToString(), ResponseDescription = ErrorLang.invoiceamountistoosmall(model.Amount).ToString() } }; }

                DateTime paymentDate = paymentDate = DateTimeOffset.ParseExact(model.PaymentDate, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture).DateTime;
                DateTime transactionDate = DateTimeOffset.ParseExact(model.TransactionDate, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture).DateTime;

                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)PaymentChannel.Web,
                    InvoiceNumber = invoiceNumber,
                    PaymentReference = model.MerchantRef,
                    AmountPaid = Math.Round((model.Amount / 100), 2),
                    PaymentDate = paymentDate,
                    TransactionDate = transactionDate,
                    RequestDump = JsonConvert.SerializeObject(model),
                    UpdatedByAdmin = false,
                    RetrievalReferenceNumber = model.PaymentRef,
                    PaymentMethodId = (int)PaymentMethods.DebitCard,
                    PaymentProvider = (int)PaymentProvider.Bank3D
                }, PaymentProvider.Bank3D);
            }
            catch (PaymentNoficationAlreadyExistsException exception)
            {
                Logger.Information(string.Format("A payment with the given payment ref already exists exception message {0} | ref {1}", exception.Message, model.PaymentRef));
                message = Lang.paymentnotificationalreadyprocess.ToString();
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.netpayerrorcode().ToString(), ResponseDescription = exception.Message } };
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorCode.PP_INVOICE_ALREADY_PAID, ResponseDescription = ErrorLang.invoiceFullyPaid().ToString() } };
            }
            catch (PartPaymentNotAllowedException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.netpayerrorcode().ToString(), ResponseDescription = ErrorLang.nopartpaymentsallow().ToString() } };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { StatusCode = HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorLang.netpayerrorcode().ToString(), ResponseDescription = ErrorLang.genericexception().ToString() } };
            }

            return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { ResponseCode = Lang.netpayresponseokcode.ToString(), ResponseDescription = message } };
        }


        #endregion


    }
}