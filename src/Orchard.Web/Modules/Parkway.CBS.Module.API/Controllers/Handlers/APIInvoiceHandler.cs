using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Newtonsoft.Json;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Models.Enums;
using Orchard;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Payee;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.EbillsPay;
using Parkway.EbillsPay.Models;
using Parkway.CBS.Core.StateConfig;
using System.Text;
using Parkway.CBS.Services.Implementations.Contracts;
using Parkway.CBS.Services.Implementations;
using System.Collections.Specialized;
using System.Configuration;
using Parkway.CBS.Core.PaymentProviderHandlers.Contracts;
using System.Net;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIInvoiceHandler : BaseAPIHandler, IAPIInvoiceHandler
    {
        private readonly ICoreInvoiceService _coreInvoiceService;
        public readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        private readonly ICBSUserManager<CBSUser> _cbsUserRepository;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreDirectAssessmentBatchRecord _coreDirectAssessmentBatchRecord;
        private readonly ICoreDirectAssessmentPayeeRecord _coreDirectAssessmentPayeeRecord;
        private readonly INIBSSEBillsPay _iNIBSSEBillsPay;
        private readonly ICoreReferenceDataBatchService _coreReferenceDataBatch;
        private readonly Lazy<ICoreExternalPaymentProviderService> _corePaymentProvider;
        private readonly Lazy<IPaymentProviderHandler> _paymentProviderHandler;

        public APIInvoiceHandler(ICoreInvoiceService coreInvoiceService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IRevenueHeadManager<RevenueHead> revenueHeadRepository, ICBSUserManager<CBSUser> cbsUserRepository, IOrchardServices orchardServices, ICoreDirectAssessmentBatchRecord coreDirectAssessmentBatchRecord, ICoreDirectAssessmentPayeeRecord coreDirectAssessmentPayeRecord, ICoreReferenceDataBatchService coreReferenceDataBatch, Lazy<ICoreExternalPaymentProviderService> corePaymentProvider, Lazy<IPaymentProviderHandler> paymentProviderHandler) : base(settingsRepository)
        {
            _coreInvoiceService = coreInvoiceService;
            _settingsRepository = settingsRepository;
            _revenueHeadRepository = revenueHeadRepository;
            _cbsUserRepository = cbsUserRepository;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _coreDirectAssessmentBatchRecord = coreDirectAssessmentBatchRecord;
            _coreDirectAssessmentPayeeRecord = coreDirectAssessmentPayeRecord;
            _iNIBSSEBillsPay = new NIBSSEBillsPay();
            _coreReferenceDataBatch = coreReferenceDataBatch;
            _corePaymentProvider = corePaymentProvider;
            _paymentProviderHandler = paymentProviderHandler;
        }


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GenerateInvoice(InvoiceController callback, CreateInvoiceUserInputModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();

            if (model == null)
            {
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Model is empty", FieldName = "Invoice" } } }, Error = true, ErrorCode = ErrorCode.PPVE.ToString() };
            }

            Logger.Information("create invoice for " + JsonConvert.SerializeObject(model));

            try
            {
                CheckModelState(callback, ref errors);
                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);

                //validate address and recipient if
                if (string.IsNullOrEmpty(model.TaxEntity.PayerId))
                {
                    if (string.IsNullOrEmpty(model.TaxEntity.Address))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Address field is required", FieldName = "Address" });
                        throw new DirtyFormDataException();
                    }
                    if (string.IsNullOrEmpty(model.TaxEntity.Recipient))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Recipient field is required", FieldName = "Recipient" });
                        throw new DirtyFormDataException();
                    }

                    if (!string.IsNullOrEmpty(model.TaxEntity.PhoneNumber))
                    {
                        string sPhoneNumber = model.TaxEntity.PhoneNumber;
                        sPhoneNumber = sPhoneNumber.Replace(" ", string.Empty);
                        sPhoneNumber = sPhoneNumber.Replace("-", string.Empty);
                        sPhoneNumber = sPhoneNumber.Replace("+", string.Empty);

                        if (model.TaxEntity.PhoneNumber.Length < 11)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "A valid PhoneNumber field is required", FieldName = "PhoneNumber" });
                            throw new DirtyFormDataException();
                        }

                        long phoneNumber = 0;
                        if (!long.TryParse(sPhoneNumber, out phoneNumber))
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Add a valid mobile phone number.", FieldName = "PhoneNumber" });
                            throw new DirtyFormDataException();
                        }
                        model.TaxEntity.PhoneNumber = phoneNumber.ToString();
                    }
                }

                var listOfRevenueHeads = new List<int> { };
                StringBuilder partValue = new StringBuilder();

                foreach (var item in model.RevenueHeadModels)
                {
                    if (item.RevenueHeadId <= 0)
                    {
                        errors.Add(new ErrorModel { FieldName = "RevenueHeadModels.RevenueHeadId", ErrorMessage = "RevenueHeadId field is required" });
                        throw new DirtyFormDataException("no valid Revenue head id " + item.RevenueHeadId);
                    }
                    listOfRevenueHeads.Add(item.RevenueHeadId);
                    partValue.AppendFormat("{0}{1}", item.RevenueHeadId, item.Amount.ToString("F"));
                }

                //validate AccessList
                CanGenerateForRevenueHead(expertSystem, listOfRevenueHeads, ref errors);
                //check hash
                string hashString = model.GroupId + partValue.ToString() + model.TaxEntity.Recipient + model.CallBackURL + headerParams.CLIENTID;

                if (!CheckHash(hashString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                InvoiceGenerationResponse response = _coreInvoiceService.TryGenerateInvoice(model, ref errors, new ExpertSystemVM { Id = expertSystem.Id, StateId = expertSystem.TenantCBSSettings.StateId }, new TaxEntityViewModel { Address = model.TaxEntity.Address, Email = model.TaxEntity.Email, PayerId = model.TaxEntity.PayerId, CategoryId = model.TaxEntityCategoryId, PhoneNumber = model.TaxEntity.PhoneNumber, Recipient = model.TaxEntity.Recipient, TaxPayerIdentificationNumber = model.TaxEntity.TaxPayerIdentificationNumber, DefaultLGAId = expertSystem.TenantCBSSettings.DefaultLGA.Id }, null);

                return new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ResponseObject = new
                    {
                        response.Recipient,
                        response.PayerId,
                        response.Email,
                        response.PhoneNumber,
                        response.TIN,
                        response.ExternalRefNumber,
                        response.PaymentURL,
                        response.InvoiceNumber,
                        response.InvoicePreviewUrl,
                        response.AmountDue,
                        response.IsDuplicateRequestReference,
                        model.RequestReference,

                    }
                };
            }
            #region catch clauses
            catch (BillingHasEndedException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.billinghasended().ToString(), FieldName = "Billing" });
                errorCode = ErrorCode.PPBILLINGENDED;
            }
            catch (TINNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.tinnumbernotfound().ToString(), FieldName = "TaxIdentificationNumber" });
                errorCode = ErrorCode.PPTIN404;
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404("Tenant").ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404("Tenant").ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
                errorCode = ErrorCode.PPUSER203;
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPVE;
            }
            catch (NoBillingTypeSpecifiedException exception)
            {
                Logger.Error(exception, ErrorLang.billingtype404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "BillingType", ErrorMessage = ErrorLang.billingtype404().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPBILLINGTYPENOTFOUND;
            }
            catch (BillingIsNotAllowedException exception)
            {
                Logger.Error(exception, ErrorLang.billingisnotallowed().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Billing", ErrorMessage = ErrorLang.billingisnotallowed().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPBILLINGNOTALLOWED;
            }
            catch (NoBillingInformationFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Billing", ErrorMessage = ErrorLang.billinginfo404().ToString() });
                errorCode = ErrorCode.PPBILLING404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, ErrorLang.revenuehead404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenuehead404().ToString() });
                errorCode = ErrorCode.PPRH404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (MDARecordNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.mdacouldnotbefound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.mdacouldnotbefound().ToString() });
                errorCode = ErrorCode.PPM404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.cannotconnettoinvoicingservice().ToString() });
                errorCode = ErrorCode.PPC1;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = exception.Message });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoice404().ToString() });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (AmountTooSmallException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoiceamountistoosmall().ToString() });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (NoCategoryFoundException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "CategoryId", ErrorMessage = ErrorLang.categorynotfound().ToString() });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.genericexception().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }



        /// <summary>
        /// Check if this expert system can generate an invoice for these revenue heads
        /// </summary>
        /// <param name="expertSystem"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        private void CanGenerateForRevenueHead(ExpertSystemSettings expertSystem, List<int> revenueHeadIds, ref List<ErrorModel> errors)
        {
            if (expertSystem.ListOfRevenueHeadAccessList() == null || expertSystem.ListOfRevenueHeadAccessList().Count <= 0) { return; }
            var accessDictionary = expertSystem.ListOfRevenueHeadAccessList().ToDictionary(k => k);
            foreach (var item in revenueHeadIds)
            {
                try { accessDictionary.Add(item, item); }
                catch (ArgumentException) { continue; }
                errors.Add(new ErrorModel { FieldName = "", ErrorMessage = string.Format("You are not allowed to generate invoices for this revenue head {0}.", item) });
                throw new DirtyFormDataException(string.Format("You are not allowed to generate invoices for this revenue head {0}.", item));
            }
        }



        public APIResponse CreateInvoice(CreateInvoiceModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();

            if (model == null)
            {
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Model is empty", FieldName = "Invoice" } } }, Error = true, ErrorCode = ErrorCode.PPVE.ToString() };
            }

            Logger.Information(string.Format("create invoice for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));

            try
            {
                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                if (model.RevenueHeadId <= 0)
                {
                    errorCode = ErrorCode.PPVE;
                    errors.Add(new ErrorModel { FieldName = "RevenueHeadId", ErrorMessage = "RevenueHeadId field is required" });
                    throw new DirtyFormDataException("no valid Revenue head id " + model.RevenueHeadId);
                }

                if (model.TaxEntityInvoice == null || model.TaxEntityInvoice.TaxEntity == null)
                {
                    errorCode = ErrorCode.PPVE;
                    errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = "Model is empty" });
                    throw new DirtyFormDataException("TaxEntityInvoice |  TaxEntity is empty ");
                }

                string hashString = model.RevenueHeadId.ToString() + model.TaxEntityInvoice.Amount.ToString("F") + model.CallBackURL + headerParams.CLIENTID;

                if (!CheckHash(hashString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                //validate address and recipient if
                if (string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.PayerId))
                {
                    if (model.TaxEntityInvoice.CategoryId <= 0)
                    {
                        errorCode = ErrorCode.PPVE;
                        errors.Add(new ErrorModel { FieldName = "CategoryId", ErrorMessage = "CategoryId is empty" });
                        throw new DirtyFormDataException("TaxEntityInvoice |  CategoryId is empty ");
                    }
                    if (string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.Address))
                    {
                        errorCode = ErrorCode.PPVE;
                        errors.Add(new ErrorModel { ErrorMessage = "Address field is required", FieldName = "Address" });
                        throw new DirtyFormDataException();
                    }
                    if (string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.Recipient))
                    {
                        errorCode = ErrorCode.PPVE;
                        errors.Add(new ErrorModel { ErrorMessage = "Recipient field is required", FieldName = "Recipient" });
                        throw new DirtyFormDataException();
                    }

                    model.TaxEntityInvoice.TaxEntity.StateLGA = new LGA { Id = expertSystem.TenantCBSSettings.DefaultLGA.Id };
                }
                //validate AccessList
                bool canGenerateForRevenueHead = CanGenerateForRevenueHead(expertSystem, model.RevenueHeadId);

                if (!canGenerateForRevenueHead)
                {
                    errorCode = ErrorCode.PPVE;
                    errors.Add(new ErrorModel { FieldName = "RevenueHeadId", ErrorMessage = "You are not allowed to generate invoices for this revenue head." });
                    throw new DirtyFormDataException("You are not allowed to generate invoices for this revenue head. " + model.RevenueHeadId);
                }

                model.ApplySurcharge = false;
                InvoiceGeneratedResponseExtn response = _coreInvoiceService.TryCreateInvoice(model, ref errors, expertSystem, model.RequestReference);
                //check if this tenant has a pay direct web config
                var stateName = _orchardServices.WorkContext.CurrentSite.SiteName;
                ////check if this state has a web paydirect integration
                //if (CheckForWebPageDirectIntegration(stateName))
                //{
                //    //if has web pay direct integration, send payment URL
                //    PayDirectConfigurations payDirectConfig = PaymentProcessorUtil.GetConfigurations<PayDirectConfigurations>(Util.GetAppRemotePath(), stateName);
                //    response.PaymentURL = payDirectConfig.ConfigNodes.Where(c => c.Key == "ThirdPartyPaymentURL").First().Value;
                //}

                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.BaseURL.ToString()).FirstOrDefault();

                InvoiceGeneratedResponseLite apiResponse = new InvoiceGeneratedResponseLite { Recipient = response.Recipient, AmountDue = Math.Round(response.AmountDue, 2) + 0.00M, CustomerId = response.CustomerId, CustomerPrimaryContactId = response.CustomerPrimaryContactId, Description = response.Description, Email = response.Email, ExternalRefNumber = response.ExternalRefNumber, InvoiceNumber = response.InvoiceNumber, InvoicePreviewUrl = response.InvoicePreviewUrl, MDAName = response.MDAName, PayerId = response.PayerId, PhoneNumber = response.PhoneNumber, PaymentURL = node.Value + "/c/make-payment/" + response.InvoiceNumber, RevenueHeadName = response.RevenueHeadName, TIN = response.TIN, RequestReference = model.RequestReference, IsDuplicateRequestReference = response.IsDuplicateRequestReference };

                Logger.Information(string.Format("InvoiceResponse {0}", JsonConvert.SerializeObject(new { apiResponse.InvoiceNumber, apiResponse.PayerId, apiResponse.RequestReference })));

                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = apiResponse };
            }
            #region catch clauses
            catch (BillingHasEndedException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.billinghasended().ToString(), FieldName = "Billing" });
                errorCode = ErrorCode.PPBILLINGENDED;
            }
            catch (TINNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.tinnumbernotfound().ToString(), FieldName = "TaxIdentificationNumber" });
                errorCode = ErrorCode.PPTIN404;
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404("Tenant").ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404("Tenant").ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
                errorCode = ErrorCode.PPUSER203;
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPVE;
            }
            catch (NoBillingTypeSpecifiedException exception)
            {
                Logger.Error(exception, ErrorLang.billingtype404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "BillingType", ErrorMessage = ErrorLang.billingtype404().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPBILLINGTYPENOTFOUND;
            }
            catch (BillingIsNotAllowedException exception)
            {
                Logger.Error(exception, ErrorLang.billingisnotallowed().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Billing", ErrorMessage = ErrorLang.billingisnotallowed().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPBILLINGNOTALLOWED;
            }
            catch (NoBillingInformationFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Billing", ErrorMessage = ErrorLang.billinginfo404().ToString() });
                errorCode = ErrorCode.PPBILLING404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, ErrorLang.revenuehead404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenuehead404().ToString() });
                errorCode = ErrorCode.PPRH404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (MDARecordNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.mdacouldnotbefound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.mdacouldnotbefound().ToString() });
                errorCode = ErrorCode.PPM404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.cannotconnettoinvoicingservice().ToString() });
                errorCode = ErrorCode.PPC1;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = exception.Message });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoice404().ToString() });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (AmountTooSmallException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoiceamountistoosmall(model.TaxEntityInvoice.Amount).ToString() });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (NoCategoryFoundException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "CategoryId", ErrorMessage = ErrorLang.categorynotfound().ToString() });
                errorCode = ErrorCode.PPVE;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.genericexception().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="expertSystem"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns></returns>
        private bool CanGenerateForRevenueHead(ExpertSystemSettings expertSystem, int revenueHeadId)
        {
            if (expertSystem.ListOfRevenueHeadAccessList() == null || expertSystem.ListOfRevenueHeadAccessList().Count <= 0) { return true; }
            return expertSystem.ListOfRevenueHeadAccessList().Where(a => a == revenueHeadId).Count() > 0 ? true : false;
        }


        /// <summary>
        /// check if this state allows pay direct integration for web
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>bool</returns>
        private bool CheckForWebPageDirectIntegration(string stateName)
        {
            var stateConfig = Util.GetTenantConfigBySiteName(stateName);
            if (stateConfig == null) { throw new CannotFindTenantIdentifierException(string.Format("Could not find config for {0}", stateName)); }
            var node = stateConfig.Node.Where(n => n.Key == "HasPayDirectWebIntegration").FirstOrDefault();
            if (node == null) { return false; }
            if (string.Equals(node.Value, "true", StringComparison.OrdinalIgnoreCase)) { return true; }
            return false;
        }


        /// <summary>
        /// Process paye assessment
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        public APIResponse ProcessPayeeInvoice(InvoiceController callback, ProcessPayeModel model, HttpPostedFile file, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            Logger.Information("create invoice for " + JsonConvert.SerializeObject(model));

            try
            {
                CheckModelState(callback, ref errors);
                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string hashString = model.RequestReference + model.ProfileId + headerParams.CLIENTID;
                if (!CheckHash(hashString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                InvoiceGeneratedResponseExtn response = _coreInvoiceService.CheckRequestReference(model.RequestReference, expertSystem);

                if (response != null) { return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = response }; }

                if (file == null)
                {
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = "File content is empty", FieldName = "assesssmentfile" } } }, Error = true };
                }

                errors = _coreDirectAssessmentBatchRecord.ValidateFile(new HttpPostedFileWrapper(file));
                if (errors.Count > 0)
                {
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = errors };
                }
                //get the user profile
                UserDetailsModel userProfile = GetUserProfile(model.ProfileId);
                if (userProfile == null) { throw new CBSUserNotFoundException(); }
                //get revenue head
                RevenueHeadDetails revenueHeadDetails = GetPayeAssessmentRevenueHead();
                if (revenueHeadDetails == null) { throw new CannotFindRevenueHeadException(); }

                DirectAssessmentBatchRecord savedBatchRecord = CreateBatchRecordModel(revenueHeadDetails, userProfile, model.Amount);

                //save the file
                try
                {
                    SavePayeeScheduleFile(file, savedBatchRecord);

                    AssessmentInterface adapter = _coreDirectAssessmentBatchRecord.GetDirectAssessmentAdapter(savedBatchRecord.AdapterValue);
                    //get paye computation implementation
                    IPayeeAdapter payeComputationImpl = _coreDirectAssessmentBatchRecord.GetAdapterImplementation(adapter);
                    //based on the type we determine what to do with the file, whether to do a computation of not
                    if (model.Amount > 0)
                    {
                        //if the amount for the schedule has already been computated

                    }
                    //lets get the adapter that contains how paye computations are to be done
                    GetPayeResponse payeResponse = payeComputationImpl.GetPayeeModels(savedBatchRecord.FilePath, Util.GetAppRemotePath(), adapter.StateName);
                    //get breakdown
                    APIResponse canProceedResponse = CanProceedWithScheduleProcessing(payeResponse);
                    if (canProceedResponse != null) { return canProceedResponse; }

                    var breakDown = payeComputationImpl.GetRequestBreakDown(payeResponse.Payes);
                    //now if the breakdown is less than zero, we cannot proceed with invoice generation
                    if (breakDown.TotalAmount <= 0)
                    {
                        return new APIResponse
                        {
                            Error = true,
                            StatusCode = System.Net.HttpStatusCode.BadRequest,
                            ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.scheduleamountistoosmall().ToString(), FieldName = "Paye" } } }
                        };
                    }
                    //lets save the paye records
                    SavePayees(payeResponse.Payes, savedBatchRecord.Id, userProfile.Entity);

                    if (model.Amount > 0) { savedBatchRecord.Amount = model.Amount; }
                    else { savedBatchRecord.Amount = breakDown.TotalAmount; }

                    savedBatchRecord.TotalNoOfRowsProcessed = payeResponse.Payes.Count;
                    savedBatchRecord.PercentageProgress = 100m;
                }
                catch (Exception)
                {
                    _cbsUserRepository.RollBackAllTransactions();
                    throw;
                }
                //based on the type lets decide on what to do with the file
                PayeAssessmentType type = savedBatchRecord.Type;
                //once we create the batch record, we go on to create the invoice
                response = _coreInvoiceService.TryCreateInvoice(new CreateInvoiceModelForPayeAssessment { RevenueHead = revenueHeadDetails.RevenueHead, Billing = revenueHeadDetails.Billing, MDA = revenueHeadDetails.Mda, DirectAssessmentBatchRecord = savedBatchRecord, TaxEntity = userProfile.Entity, TaxEntityCategory = userProfile.Category, UserProfile = userProfile.CBSUser, Amount = savedBatchRecord.Amount }, model.RequestReference, expertSystem);

                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = new InvoiceGeneratedResponse { AmountDue = response.AmountDue, InvoiceNumber = response.InvoiceNumber, InvoicePreviewUrl = response.InvoicePreviewUrl } };
            }
            #region catch clauses
            catch (CBSUserNotFoundException)
            {
                Logger.Error("CBS user not found");
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "ProfileId", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, ErrorLang.revenuehead404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenuehead404().ToString() });
                errorCode = ErrorCode.PPRH404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (MDARecordNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.mdacouldnotbefound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.mdacouldnotbefound().ToString() });
                errorCode = ErrorCode.PPM404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (NoBillingInformationFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Billing", ErrorMessage = ErrorLang.billinginfo404().ToString() });
                errorCode = ErrorCode.PPBILLING404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (BillingHasEndedException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.billinghasended().ToString(), FieldName = "Billing" });
                errorCode = ErrorCode.PPBILLINGENDED;
            }
            catch (TINNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.tinnumbernotfound().ToString(), FieldName = "TaxIdentificationNumber" });
                errorCode = ErrorCode.PPTIN404;
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404("Tenant").ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404("Tenant").ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
                errorCode = ErrorCode.PPUSER203;
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPVE;
            }
            catch (NoBillingTypeSpecifiedException exception)
            {
                Logger.Error(exception, ErrorLang.billingtype404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "BillingType", ErrorMessage = ErrorLang.billingtype404().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPBILLINGTYPENOTFOUND;
            }
            catch (BillingIsNotAllowedException exception)
            {
                Logger.Error(exception, ErrorLang.billingisnotallowed().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Billing", ErrorMessage = ErrorLang.billingisnotallowed().ToString() });
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPBILLINGNOTALLOWED;
            }
            catch (CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, exception.Message + exception.StackTrace);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.cannotconnettoinvoicingservice().ToString() });
                errorCode = ErrorCode.PPC1;
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.genericexception().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            #endregion

            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }


        /// <summary>
        /// Checks if the header object value has errors and the payee are greater than 0
        /// <para>This method would return null if this process can proceed, else would return with response object</para>
        /// </summary>
        /// <param name="payeResponse"></param>
        /// <returns>APIResponse</returns>
        private APIResponse CanProceedWithScheduleProcessing(GetPayeResponse payeResponse)
        {
            if (payeResponse.HeaderValidateObject.Error)
            {
                return new APIResponse
                {
                    Error = true,
                    ErrorCode = ErrorCode.PPVE.ToString(),
                    ResponseObject = new List<ErrorModel>
                            {
                                {
                                    new ErrorModel
                                    {
                                        ErrorMessage = payeResponse.HeaderValidateObject.ErrorMessage
                                    }
                                }
                            }
                };
            }

            if (payeResponse.Payes.Count <= 0)
            {
                return new APIResponse
                {
                    Error = true,
                    ErrorCode = ErrorCode.PPVE.ToString(),
                    ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.payees404().ToString(), FieldName = "Paye" } } }
                };
            }

            return null;
        }


        /// <summary>
        /// Save payee schedule
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savedBatchRecord"></param>
        /// <exception cref="Exception"></exception>
        private void SavePayeeScheduleFile(HttpPostedFile file, DirectAssessmentBatchRecord savedBatchRecord)
        {
            _coreDirectAssessmentBatchRecord.SaveFile(new HttpPostedFileWrapper(file), savedBatchRecord.FilePath);
        }


        /// <summary>
        /// Save payees
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="recordId"></param>
        /// <param name="entity"></param>
        /// <exception cref="Exception">throw exception is save is not successful</exception>
        private void SavePayees(List<PayeeAssessmentLineRecordModel> payees, Int64 recordId, TaxEntity entity)
        {
            _coreDirectAssessmentPayeeRecord.SaveRecords(payees, recordId, entity);
        }


        /// <summary>
        /// Get revenue head
        /// </summary>
        /// <returns>RevenueHead</returns>
        private RevenueHeadDetails GetPayeAssessmentRevenueHead()
        {
            return _revenueHeadRepository.GetRevenueHeadDetailsForPaye();
        }


        /// <summary>
        /// Get CBS user profile
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns>CBSUser</returns>
        /// <exception cref="Exception"></exception>
        private UserDetailsModel GetUserProfile(long profileId)
        {
            return _cbsUserRepository.GetCBSUserAndTaxEntity(profileId);
        }


        private DirectAssessmentBatchRecord CreateBatchRecordModel(RevenueHeadDetails revenueHeadDetails, UserDetailsModel userProfile, decimal amount)
        {
            Logger.Information("Creating batch record for processed file upload");
            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

            PayeAssessmentType type = PayeAssessmentType.None;
            if (amount > 0) { type = PayeAssessmentType.ProcessedFileUpload; }
            else { type = PayeAssessmentType.FileUploadFromAPI; }
            return _coreDirectAssessmentBatchRecord.SaveDirectAssessmentRecord(revenueHeadDetails, userProfile, amount, siteName, type);
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
        /// Get the invoice details for validation for bank3D
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="flatScheme"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ValidateInvoice(Core.HelperModels.ValidationRequest model, bool flatScheme = false)
        {
            Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));
            string errmsg = null;

            try
            {
                if (!string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    //do HMAC validation
                    string value = model.InvoiceNumber;

                    StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                    Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.InfoGridExchangeValue.ToString()).FirstOrDefault();

                    string localHash = Util.HMACHash(value, node.Value);
                    //var localHash = Util.HMACHash(value, ConfigurationManager.AppSettings["Bank3dExchangeValue"]);

                    if (localHash != model.mac)
                    {
                        Logger.Error(string.Format("Could not compute the expected MAC : {0} | computed mac : {1} | value : {2}", model.mac, localHash, value));
                        return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new InvoiceValidationResponseModelForBankCollect { ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), ResponseDescription = ErrorLang.macauthfailed().ToString() } };
                    }

                    InvoiceGeneratedResponseExtn invoiceDetails = _coreInvoiceService.GetInvoiceDetailsForPaymentView(model.InvoiceNumber);
                    if (invoiceDetails != null)
                    {
                        if (invoiceDetails.AmountDue > 00.00m)
                        {
                            CheckForPaymentProviderRestrictions(invoiceDetails, (int)PaymentProvider.Bank3D);

                            CheckSettlementType(model.InvoiceNumber, flatScheme, invoiceDetails.MDASettlementType, invoiceDetails.RevenueHeadSettlementType);

                            return new APIResponse
                            {
                                ResponseObject = new InvoiceValidationResponseModelForBankCollect
                                {
                                    ResponseCode = Lang.bankcollectresponseokcode.ToString(),
                                    Amount = invoiceDetails.AmountDue,
                                    Email = invoiceDetails.Email,
                                    InvoiceNumber = model.InvoiceNumber,
                                    PhoneNumber = invoiceDetails.PhoneNumber,
                                    Recipient = invoiceDetails.Recipient,
                                    ResponseDescription = string.Format("Invoice for {0} {1}", invoiceDetails.MDAName, invoiceDetails.RevenueHeadName)
                                },
                                StatusCode = System.Net.HttpStatusCode.OK
                            };
                        }
                        return new APIResponse
                        {
                            ResponseObject = new InvoiceValidationResponseModelForBankCollect { ResponseDescription = Lang.invoicealreadypaid(model.InvoiceNumber).ToString(), ResponseCode = ErrorLang.bankcollecterrorcode().ToString(), },
                            StatusCode = System.Net.HttpStatusCode.OK
                        };
                    }
                }
            }
            catch (UserNotAuthorizedForThisActionException exception) { errmsg = ErrorLang.usernotauthorized().ToString(); Logger.Error(exception, exception.Message); }
            catch (Exception exception) { errmsg = ErrorLang.genericexception().ToString(); Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ResponseObject = new InvoiceValidationResponseModelForBankCollect
                {
                    ResponseDescription = errmsg ?? ErrorLang.invoice404(model.InvoiceNumber).ToString(),
                    ResponseCode =  ErrorLang.bankcollecterrorcode().ToString(),
                }
            };
        }

        



        /// <summary>
        /// Validate invoice 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        public APIResponse InvoiceValidation(Core.HelperModels.ValidationRequest model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if (model == null)
                {
                    errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.valuerequired("Invoice").ToString() });
                    throw new Exception("Model is empty");
                }
                Logger.Information(string.Format("validate invoice for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));
                //get external third party payment provider
                ExternalPaymentProviderVM paymentProvider = _corePaymentProvider.Value.GetPaymentProvider(headerParams.CLIENTID);
                model.Signature = headerParams.SIGNATURE;
                return _paymentProviderHandler.Value.DoValidation(model, paymentProvider);
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
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "Generic", ErrorMessage = ErrorLang.genericexception().ToString() });
            }

            return new APIResponse
            {
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true,
                ErrorCode = errorCode
            };
        }


        /// <summary>
        /// Validate invoice for readycash
        /// </summary>
        /// <param name="invoiceController"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse ValidateInvoice(ReadycashInvoiceValidationModel model)
        {
            string errorMessage = null;
            try
            {
                Logger.Information(string.Format("{0}", JsonConvert.SerializeObject(model)));
                if (string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    errorMessage = ErrorLang.invoice404().ToString();
                    throw new Exception("No invoice found");
                }

                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                if (siteConfig == null) { throw new TenantNotFoundException(); }
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.ReadycashExchangeKeyValue.ToString()).FirstOrDefault();
                if (node == null) { throw new KeyNotFoundException("Tenant exchange key 404"); }

                var localHash = Util.HMACHash(model.InvoiceNumber + model.Channel, node.Value);

                if (localHash != model.Mac)
                {
                    errorMessage = "Could not compute signature hash " + model.InvoiceNumber;
                    Logger.Error(errorMessage);
                    throw new UserNotAuthorizedForThisActionException(ErrorLang.couldnotcomputehash().ToString());
                }

                InvoiceGeneratedResponseExtn response = _coreInvoiceService.GetInvoiceDetailsForPaymentView(model.InvoiceNumber);

                if (response == null)
                {
                    errorMessage = string.Format("{0} not found", model.InvoiceNumber);
                    throw new NoRecordFoundException(errorMessage);
                }

                if (response.AmountDue > 0m)
                {
                    CheckForPaymentProviderRestrictions(response, (int)PaymentProvider.Readycash);

                    return new APIResponse
                    {
                        ResponseObject = new ReadycashInvoiceValidationResponseModel
                        {
                            ResponseCode = Lang.bankcollectresponseokcode.ToString(),
                            Amount = Math.Round(response.AmountDue) + 0.00m,
                            InvoiceNumber = model.InvoiceNumber,
                            Recipient = response.Recipient,
                            ResponseDescription = string.Format("Invoice for {0} | {1}", response.InvoiceTitle, response.InvoiceDesc)
                        },
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    return new APIResponse
                    {
                        ResponseObject = new ReadycashInvoiceValidationResponseModel
                        {
                            ResponseCode = ErrorLang.bankcollecterrorcode().ToString(),
                            ResponseDescription = Lang.invoicealreadypaidfor.ToString()
                        },
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                return new APIResponse
                {
                    ResponseObject = new ReadycashInvoiceValidationResponseModel
                    {
                        ResponseCode = ErrorLang.bankcollecterrorcode().ToString(),
                        ResponseDescription = ErrorLang.usernotauthorized().ToString()
                    },
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                ResponseObject = new ReadycashInvoiceValidationResponseModel
                {
                    ResponseCode = ErrorLang.bankcollecterrorcode().ToString(),
                    ResponseDescription = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage
                },
                StatusCode = System.Net.HttpStatusCode.OK
            };

        }


        /// <summary>
        /// Do invoice validate for NIBSS Ebills Pay
        /// </summary>
        /// <param name="invoiceController"></param>
        /// <param name="requestStreamString"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ValidateInvoiceNIBSS(string requestStreamString)
        {
            string billerID = string.Empty;
            List<Param> listOfParams = null;
            string responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.ErrorCode);
            PaymentDetail paymentDetail = new PaymentDetail { };

            try
            {
                //if string is empty
                if (string.IsNullOrEmpty(requestStreamString))
                { throw new Exception("Request string is empty"); }

                //deserialize the string request value
                EbillsPay.Models.ValidationRequest validationObj = null;

                validationObj = _iNIBSSEBillsPay.DeserializeXMLRequest<EbillsPay.Models.ValidationRequest>(requestStreamString);

                if (validationObj == null)
                { throw new Exception("Mal formed request string. Could not deserialize EbillsPay.Models.ValidationRequest is null for request value " + requestStreamString); }
                billerID = validationObj.BillerID;

                //do auth validation
                DoEbillsPayAuthValidation(validationObj, ref responseCode);

                //set billerID
                //get the invoice number
                var invoiceObj = validationObj.Param.Where(x => x.Key == "InvoiceNumber").SingleOrDefault();

                if (invoiceObj == null || string.IsNullOrEmpty(invoiceObj.Value))
                { throw new Exception("invoice object not found in the validation object"); }

                //
                //do validation for biller id and 

                //
                InvoiceGeneratedResponseExtn response = _coreInvoiceService.GetInvoiceDetailsForPaymentView(invoiceObj.Value.Trim());

                if (response == null)
                { throw new Exception(ErrorLang.invoice404(invoiceObj.Value).ToString()); }

                //check for restrictions
                CheckForPaymentProviderRestrictions(response, (int)PaymentProvider.NIBSS);

                listOfParams = new List<Param>
                {
                    { new Param { Key = "InvoiceNumber", Value = response.InvoiceNumber } },
                    //{ new Param { Key = "Amount", Value = response.AmountDue.ToString("F") } },
                    { new Param { Key = "total", Value = response.AmountDue.ToString("F") } },
                    { new Param { Key = "PhoneNumber", Value = response.PhoneNumber } },
                    { new Param { Key = "Email", Value = response.Email } },
                    { new Param { Key = "Name", Value = response.Recipient } },
                    { new Param { Key = "Status", Value = "Valid" } }
                };
                paymentDetail.Amount = response.AmountDue.ToString("F");
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.OK);
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseObject = new ValidationResponse { BillerID = billerID, NextStep = 0.ToString(), ResponseCode = responseCode, Param = listOfParams ?? new List<Param> { }, PaymentDetail = paymentDetail }
            };
        }

        /// <summary>
        /// Do validation for NIBSS Ebills pay
        /// </summary>
        /// <param name="validationObj"></param>
        /// <param name="responseCode"></param>
        private void DoEbillsPayAuthValidation(EbillsPay.Models.ValidationRequest validationObj, ref string responseCode)
        {
            var stateName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig stateconfig = Util.GetTenantConfigBySiteName(stateName);

            if (stateconfig == null)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.InternalError);
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
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                throw new Exception(string.Format("No biller Id entry found in config"));
            }

            if (validationObj.BillerID != billerIdNode.Value)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                throw new Exception(string.Format("BillerID mismatch expected : {0}, actual : {1}", billerIdNode.Value, validationObj.BillerID));
            }

            //check for biller name
            if (billeNameNode == null)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                throw new Exception(string.Format("No biller Id entry found in config"));
            }

            if (validationObj.BillerName != billeNameNode.Value)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.BillerMismatch);
                throw new Exception(string.Format("billeNameNode mismatch expected : {0}, actual : {1}", billeNameNode.Value, validationObj.BillerName));
            }

            //check for product id
            if (productIdNode == null)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                throw new Exception(string.Format("No product id entry found in config"));
            }

            if (validationObj.ProductID != productIdNode.Value)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                throw new Exception(string.Format("product id mismatch expected : {0}, actual : {1}", productIdNode.Value, validationObj.ProductID));
            }

            //check product name
            if (productNameNode == null)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                throw new Exception(string.Format("No product name entry found in config"));
            }

            if (validationObj.ProductName != productNameNode.Value)
            {
                responseCode = NIBSSEbillsPayUtils.GetEnvValue(EnvValues.SecurityViolation);
                throw new Exception(string.Format("product name mismatch expected : {0}, actual : {1}", productNameNode.Value, validationObj.ProductName));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse BatchInvoiceResponse(InvoiceController callback, CashflowBatchCustomerAndInvoicesResponse model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();

            if (model == null)
            {
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Model is empty", FieldName = "Invoice" } } }, Error = true, ErrorCode = ErrorCode.PPVE.ToString() };
            }

            long batchId = 0;
            bool parsed = Int64.TryParse(model.BatchIdentifier.Trim(), out batchId);
            if (!parsed)
            {
                throw new Exception("Unable to convert the General Reference ID");
            }


            //Validate the mac
            ReferenceDataBatch referenceData = _coreReferenceDataBatch.GetBatchDetails(batchId);
            if (referenceData == null)
            {
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Invalid Batch Identifier", FieldName = "Invoice" } } }, Error = true, ErrorCode = ErrorCode.PPREC404.ToString() };
            }

            string value = model.BatchIdentifier + referenceData.BatchInvoiceCallBackURL + referenceData.RevenueHead.Mda.SMEKey;
            NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("cashFlowSettings");
            var localHash = Util.HMACHash256(value, section["ThirdPartyRequestSecret"]);

            if (localHash != model.Mac)
            {
                Logger.Error(string.Format("Could not compute the expected MAC : {0} | computed mac : {1} | value : {2}", model.Mac, localHash, value));
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = new { ResponseCode = ErrorCode.PPS1.ToString(), ResponseDescription = ErrorLang.macauthfailed().ToString() } };
            }

            try
            {
                //Passed to the hangfire
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                IBatchInvoiceResponseInterface batchInvoiceResponse = new BatchInvoiceResponseInterface();
                batchInvoiceResponse.ProcessInvoices(siteName.Replace(" ", ""), model.BatchIdentifier, model.FileName);

                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK };
            }
            #region catch clauses
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Batch Invoice Response", ErrorMessage = ErrorLang.genericexception().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        public APIResponse GetInvoiceStatus(Core.HelperModels.ValidationRequest model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if (model == null)
                {
                    errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.valuerequired("Invoice").ToString() });
                    throw new Exception("Model is empty");
                }
                Logger.Information(string.Format("Query invoice for model {0}", JsonConvert.SerializeObject(new { model.InvoiceNumber, headerParams })));

                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);

                string hashString = model.InvoiceNumber.ToString() + headerParams.CLIENTID;

                if (!CheckHash(hashString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                CollectionSearchParams searchParams = new CollectionSearchParams
                {
                    InvoiceNumber = model.InvoiceNumber,
                    AdminUserId = expertSystem.ThirdPartyAuthorizedAdmin.Id
                };

                InvoiceStatusDetailsVM invoiceDeets = _coreInvoiceService.GetInvoiceStatus(searchParams);
                return new APIResponse
                {
                    ResponseObject = invoiceDeets,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (TenantNotFoundException ex)
            {
                Logger.Error($"{ex.Message}" + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "ClientId", ErrorMessage = ErrorLang.tenant404().ToString() });
                errorCode = ResponseCodeLang.tenant_404;
            }
            catch (UserNotAuthorizedForThisActionException ex)
            {
                Logger.Error($"{ex.Message}" + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Permission", ErrorMessage = ErrorLang.usernotauthorized().ToString() });
                errorCode = ResponseCodeLang.user_not_authorized;
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error($"{ex.Message}" + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Invoice Number", ErrorMessage = ErrorLang.invoice404().ToString() });
                errorCode = ResponseCodeLang.invoice_404;
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

        /// <summary>
        /// Invalidate an invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        public APIResponse InvalidateInvoice(Core.HelperModels.ValidationRequest model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;

            try
            {
                if (model == null)
                {
                    errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.valuerequired("Invoice").ToString() });
                    throw new Exception("Model is empty");
                }
                Logger.Information(string.Format("Invalidate invoice model {0}", JsonConvert.SerializeObject(new { model.InvoiceNumber, headerParams })));

                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);

                string hashString = model.InvoiceNumber.ToString() + headerParams.CLIENTID;

                if (!CheckHash(hashString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                CollectionSearchParams searchParams = new CollectionSearchParams
                {
                    InvoiceNumber = model.InvoiceNumber,
                    AdminUserId = expertSystem.ThirdPartyAuthorizedAdmin.Id
                };

                InvalidateInvoiceVM invoiceDeets = _coreInvoiceService.InvalidateInvoice(searchParams);
                return new APIResponse
                {
                    ResponseObject = invoiceDeets,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (TenantNotFoundException ex)
            {
                Logger.Error($"{ex.Message}" + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "ClientId", ErrorMessage = ErrorLang.tenant404().ToString() });
                errorCode = ResponseCodeLang.tenant_404;
            }
            catch (InvoiceHasPaymentException ex)
            {
                Logger.Error($"{ex.Message}" + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoicehaspayment(model.InvoiceNumber).ToString() });
                errorCode = ResponseCodeLang.invoice_already_paid_for;
            }
            catch (UserNotAuthorizedForThisActionException ex)
            {
                Logger.Error($"{ex.Message}" + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Permission", ErrorMessage = ErrorLang.usernotauthorized().ToString() });
                errorCode = ResponseCodeLang.user_not_authorized;
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error($"{ex.Message}" + (string)headerParams.CLIENTID);
                errors.Add(new ErrorModel { FieldName = "Invoice Number", ErrorMessage = ErrorLang.invoice404().ToString() });
                errorCode = ResponseCodeLang.invoice_404;
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

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">InvoiceController</param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> DoModelCheck(InvoiceController callback)
        {
            return CheckModelStateWithoutException(callback);
        }

    }
}