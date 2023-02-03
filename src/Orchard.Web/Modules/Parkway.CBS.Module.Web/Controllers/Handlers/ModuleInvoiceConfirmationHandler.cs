using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public class ModuleInvoiceConfirmationHandler : IModuleInvoiceConfirmationHandler
    {
        private readonly ICoreCollectionService _coreCollectionService;
        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly Lazy<IEnumerable<IBillingTypeInvoiceConfirmation>> _billingTypeInvoiceConfirmationImpls;
        private readonly ICoreInvoiceService _coreInvoiceService;


        public ModuleInvoiceConfirmationHandler(ICoreInvoiceService coreInvoiceService, ICoreCollectionService coreCollectionService, IHandlerHelper handlerHelper, IAdminSettingManager<ExpertSystemSettings> settingsRepository, Lazy<IEnumerable<IBillingTypeInvoiceConfirmation>> billingTypeInvoiceConfirmationImpls)
        {
            _coreCollectionService = coreCollectionService;
            _coreInvoiceService = coreInvoiceService;
            _handlerHelper = handlerHelper;
            _settingsRepository = settingsRepository;
            _billingTypeInvoiceConfirmationImpls = billingTypeInvoiceConfirmationImpls;
        }



        /// <summary>
        /// get the model for invoice generation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>CreateInvoiceModel</returns>
        public CreateInvoiceModel GetCreateInvoiceModel(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
            //lets do a validation on the revenue head
            //lets validate that this revenue head is bound to an external redirect for invoice generation'
            RevenueHeadDetails result = _coreCollectionService.GetRevenueHeadDetails(processStage.RevenueHeadId);
            if (!string.IsNullOrEmpty(result.InvoiceGenerationRedirectURL))
            { return new CreateInvoiceModel { ExternalRedirect = new ExternalRedirect { Redirecting = true, URL = result.InvoiceGenerationRedirectURL } }; }

            return new CreateInvoiceModel
            {
                UserIsAuthorized = true, //lets us know that the user generating the invoice is authorized to do so
                RevenueHeadId = processStage.RevenueHeadId,
                TaxEntityInvoice = new TaxEntityInvoice
                {
                    TaxEntity = new TaxEntity
                    {
                        Address = entity.Address,
                        Email = entity.Email,
                        PhoneNumber = entity.PhoneNumber,
                        Recipient = entity.Recipient,
                        TaxPayerIdentificationNumber = entity.TaxPayerIdentificationNumber,
                        Id = entity.Id,
                        PayerId = entity.PayerId
                    },
                    Amount = processStage.InvoiceConfirmedModel.Amount,
                    CategoryId = processStage.CategoryId
                },
                ExternalRefNumber = processStage.InvoiceConfirmedModel.ExternalRef,
                Forms = processStage.UserFormDetails,
                MDAName = processStage.ProceedWithInvoiceGenerationVM.MDAName,
                RevenueHeadName = processStage.ProceedWithInvoiceGenerationVM.RevenueHeadName,
                Surcharge = processStage.ProceedWithInvoiceGenerationVM.Surcharge,
            };
        }


        /// <summary>
        /// Try generate invoice
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="entity"></param>
        /// <returns>string</returns>
        public string TryGenerateInvoice(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
            try
            {
                CreateInvoiceUserInputModel createInvoiceModel = new CreateInvoiceUserInputModel
                {
                    GroupId = 0,
                    InvoiceTitle = string.Format("{0} : {1}", processStage.ProceedWithInvoiceGenerationVM.MDANameAndCode, processStage.ProceedWithInvoiceGenerationVM.RevenueHeadNameAndCode),
                    InvoiceDescription = string.Format("{0} : {1}", processStage.ProceedWithInvoiceGenerationVM.MDANameAndCode, processStage.ProceedWithInvoiceGenerationVM.RevenueHeadNameAndCode),
                    CallBackURL = processStage.ProceedWithInvoiceGenerationVM.CallBackURL,
                    TaxEntityCategoryId = processStage.CategoryId,
                    RevenueHeadModels = new List<RevenueHeadUserInputModel>
                {
                    {
                        new RevenueHeadUserInputModel
                        {
                            Amount = processStage.InvoiceConfirmedModel.Amount,
                            Quantity = 1,
                            RevenueHeadId = processStage.RevenueHeadId,
                            FormValues = processStage.UserFormDetails?.Select(f => new FormControlViewModel
                            {
                                ControlIdentifier = f.ControlIdentifier,
                                FormValue = f.FormValue,
                                RevenueHeadId = processStage.RevenueHeadId
                            }).ToList(),
                            ApplySurcharge = true,
                            Surcharge = processStage.ProceedWithInvoiceGenerationVM.Surcharge
                        }
                    }
                },
                    DontValidateFormControls = true,
                    ExternalRefNumber = processStage.ExternalRef,
                };

                List<ErrorModel> errors = new List<ErrorModel> { };
                ExpertSystemVM expertSys = _settingsRepository.GetRootExpertSystem().First();

                InvoiceGenerationResponse response = _coreInvoiceService.TryGenerateInvoice(createInvoiceModel, ref errors, expertSys, new TaxEntityViewModel { Id = entity.Id });

                foreach (var item in _billingTypeInvoiceConfirmationImpls.Value)
                {
                    if (item.BillingType == processStage.BillingType)
                    {
                        processStage.InvoiceConfirmedModel.RevenueHeadSurcharge = processStage.ProceedWithInvoiceGenerationVM.Surcharge;
                        item.InvoiceHasBeenConfirmed(processStage.InvoiceConfirmedModel, response.InvoiceId);
                    }
                }
                return response.InvoiceNumber;
            }
            catch (Exception)
            {
                _settingsRepository.RollBackAllTransactions();
                throw;
            }
        }

    }
}