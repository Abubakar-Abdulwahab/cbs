using System;
using System.Net;
using System.Web;
using System.Linq;
using Orchard.Logging;
using Newtonsoft.Json;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Core;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class RDCAPICreateInvoiceHandler : BaseAPIHandler, IRDCAPICreateInvoiceHandler
    {
        private readonly ICoreTaxPayerService _coreTaxPayerService;
        private readonly ICoreRevenueHeadService _coreRevenueHeadService;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _catRepo;
        private readonly ICoreInvoiceService _coreInvoiceService;


        public RDCAPICreateInvoiceHandler(ICoreTaxPayerService coreTaxPayerService, ICoreRevenueHeadService coreRevenueHeadService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ITaxEntityCategoryManager<TaxEntityCategory> catRepo, ICoreInvoiceService coreInvoiceService) : base(settingsRepository)
        {
            Logger = NullLogger.Instance;
            _coreTaxPayerService = coreTaxPayerService;
            _coreRevenueHeadService = coreRevenueHeadService;
            _catRepo = catRepo;
            _coreInvoiceService = coreInvoiceService;
        }


        /// <summary>
        /// Generate invoice for readycash 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="exheaderParams"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GenerateInvoice(RDCBillerCreateInvoiceModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            TaxEntityInvoice taxEntityInvoice = null;
            CreateInvoiceModel createInvoiceModel = new CreateInvoiceModel { };

            ErrorCode errorCode = new ErrorCode();

            try
            {
                if (model == null)
                {
                    errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.valuerequired("Invoice").ToString() });
                    throw new Exception("Model is empty");
                }
                Logger.Information(string.Format("validate invoice for model {0}", JsonConvert.SerializeObject(new { model, headerParams })));
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //
                string hashString = model.RevenueHeadId.ToString() + model.Amount.ToString("F") + model.TaxPayerCode + headerParams.CLIENTID;

                if (!CheckHash(hashString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                //
                if (!string.IsNullOrEmpty(model.RevenueHeadCode))
                {
                    int revenueHeadId = 0;
                    if (!Int32.TryParse(model.RevenueHeadCode.Trim(), out revenueHeadId))
                    { throw new CannotFindRevenueHeadException { }; }
                    model.RevenueHeadId = revenueHeadId;
                }

                RevenueHeadVM revVM = _coreRevenueHeadService.GetRevenueHeadVM(model.RevenueHeadId);
                //this changed from Niger implementation
                //the initial idea behind this request was to generate invoices for patients in IBB hospital
                //where the patient Id is stored inisde the TaxPayerCode
                //this endpoint has been repurposed for something else, which requires we TaxPayerCode hold the payer name
                //and the phone number is now used as the unqiue identifier for the tax profile
                //TaxPayerWithDetails entity = _coreTaxPayerService.GetTaxEntityDetails(model.TaxPayerCode);
                TaxPayerWithDetails entity = _coreTaxPayerService.GetTaxEntityDetails(model.PhoneNumber);

                if(entity == null)
                {
                    taxEntityInvoice = new TaxEntityInvoice
                    {
                        Amount = model.Amount,
                        TaxEntity = new TaxEntity
                        {
                            PhoneNumber = model.PhoneNumber,
                            Recipient = model.TaxPayerCode,
                            Address = revVM.Address,
                            TaxPayerCode = model.PhoneNumber,
                            StateLGA = new LGA { Id = expertSystem.TenantCBSSettings.DefaultLGA.Id }
                        },
                        CategoryId = _catRepo.GetFirstCategoryId()
                    };
                }
                else
                {
                    taxEntityInvoice = new TaxEntityInvoice { Amount = model.Amount, TaxEntity = new TaxEntity { Id = entity.Id }, CategoryId = entity.CategoryId };
                }

                createInvoiceModel = new CreateInvoiceModel
                {
                    TaxEntityInvoice = taxEntityInvoice,
                    ApplySurcharge = false,
                    Quantity = 1,
                    RevenueHeadId = model.RevenueHeadId,
                };
                InvoiceGeneratedResponseExtn response = _coreInvoiceService.TryCreateInvoice(createInvoiceModel, ref errors, expertSystem);

                return new APIResponse
                {
                    ResponseObject = new InvoiceValidationModel
                    {
                        ResponseCode = ResponseCodeLang.ok,
                        Amount = Math.Round(response.AmountDue, 2) + 0.00m,
                        PayerId = response.PayerId,
                        Email = response.Email,
                        InvoiceNumber = response.InvoiceNumber,
                        PhoneNumber = response.PhoneNumber,
                        Recipient = response.Recipient,
                        ResponseDescription = string.Format("{0} {1}", response.InvoiceTitle, response.InvoiceDesc),
                        SettlementCode = string.IsNullOrEmpty(response.RevenueHeadSettlementCode) ? response.MDASettlementCode : response.RevenueHeadSettlementCode,
                        SettlementType = response.RevenueHeadSettlementType == 0 ? response.MDASettlementType : response.RevenueHeadSettlementType,
                        IssuerName = response.MDAName
                    },
                    StatusCode = HttpStatusCode.OK,
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
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.invoiceamountistoosmall(model.Amount).ToString() });
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


    }
}