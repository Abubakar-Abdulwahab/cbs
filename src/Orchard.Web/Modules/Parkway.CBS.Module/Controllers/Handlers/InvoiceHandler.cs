using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Orchard.Security;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Orchard.Modules.Services;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Module.ViewModels;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.Controllers.Handlers.AdminBillingImpl.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Utilities;
using System.Dynamic;
using Newtonsoft.Json;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class InvoiceHandler : BaseHandler, IInvoiceHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreInvoiceService _coreInvoiceService;
        private readonly ICoreFormService _coreFormService;
        public Localizer T { get; set; }
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly ITaxEntityManager<TaxEntity> _taxPayerRepository;
        private readonly IMDAManager<MDA> _mdaRepository;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        private readonly IEnumerable<IAdminBillingImpl> _billingImpls;

        public InvoiceHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ICoreInvoiceService coreInvoiceService, ITaxEntityManager<TaxEntity> taxPayerRepository, IMDAManager<MDA> mdaRepository, IRevenueHeadManager<RevenueHead> revenueHeadRepository, IEnumerable<IAdminBillingImpl> billingImpls, ICoreFormService coreFormService) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _settingsRepository = settingsRepository;
            _coreInvoiceService = coreInvoiceService;
            _taxPayerRepository = taxPayerRepository;
            _mdaRepository = mdaRepository;
            _revenueHeadRepository = revenueHeadRepository;
            _billingImpls = billingImpls;
            _coreFormService = coreFormService;
        }

        /// <summary>
        /// Get the invoice URL of this invoice with this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>string</returns>
        public string GetInvoiceURL(string invoiceNumber)
        {
            return _coreInvoiceService.GetInvoiceURL(invoiceNumber);
        }


        /// <summary>
        /// Search for tax payer by either TIN or Phone number
        /// <para>Does a count and returns true if count is 1 or false otherwise</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>List{TaxEntity}</returns>
        public List<TaxPayerWithDetails> SearchForTaxPayer(SearchForTaxEntityVM model)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);

            List<TaxPayerWithDetails> taxPayers = new List<TaxPayerWithDetails> { };
            if (!string.IsNullOrEmpty(model.TIN))
            {
                taxPayers = _taxPayerRepository.GetListOfTaxPayersWithDetails(model.TIN, "SearchByTIN");
                if (taxPayers.Count < 0)
                {
                    taxPayers = _taxPayerRepository.GetListOfTaxPayersWithDetails(model.PhoneNumber, "SearchByPhoneNumber");
                }
            }
            else
            {
                taxPayers = _taxPayerRepository.GetListOfTaxPayersWithDetails(model.PhoneNumber, "SearchByPhoneNumber");
            }
            return taxPayers;
        }


        /// <summary>
        /// Get tax payer for confirmation
        /// </summary>
        /// <param name="id"></param>
        /// <returns>GenerateInvoiceConfirmTaxPayer</returns>
        public GenerateInvoiceConfirmTaxPayer GetTaxPayerAndRevenueHeads(string sid)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);

            Int64 id = 0;
            if (!Int64.TryParse(sid, out id)) { return null; }
            try
            {
                var taxPayer = _taxPayerRepository.GetTaxPayerWithDetails(id);
                if (taxPayer == null) { return new GenerateInvoiceConfirmTaxPayer { TaxPayerWithDetails = null, RevenueHeads = null }; }
                return new GenerateInvoiceConfirmTaxPayer
                {
                    RevenueHeads = _revenueHeadRepository.GetBillableCollectionForAdminGenerateInvoice(),
                    TaxPayerWithDetails = taxPayer,
                    TaxPayerId = id
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting tax payer Id " + sid);
                throw;
            }
        }


        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <returns>TaxPayerWithDetails</returns>
        public TaxPayerWithDetails GetTaxPayer(string taxPayerId)
        {
            Int64 id = 0;
            if (!Int64.TryParse(taxPayerId, out id)) { throw new Exception("No tax payer found"); }

            var taxPayer = _taxPayerRepository.GetTaxPayerWithDetails(id);
            if (taxPayer == null) { throw new Exception("No tax payer found") { }; }
            return taxPayer;
        }


        /// <summary>
        /// Get view for tax payer search for invoice generation
        /// </summary>
        /// <returns>SearchForTaxEntityVM</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public SearchForTaxEntityVM GetViewForSearchForTaxEntity()
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);
            return new SearchForTaxEntityVM { };
        }


        /// <summary>
        /// Get revenue heads details
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadDetails</returns>
        public RevenueHeadDetails GetRevenueHeadDetails(string srevenueHeadId)
        {
            int revenueHeadId = 0;
            if (!Int32.TryParse(srevenueHeadId, out revenueHeadId)) { throw new Exception("No Revenue head found"); }
            return _revenueHeadRepository.GetRevenueHeadDetails(revenueHeadId);

        }


        /// <summary>
        /// Get the view for input
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns></returns>
        public ViewToShowVM GetViewForCreateBill(TaxPayerWithDetails taxPayer, RevenueHeadDetails revenueHeadDetails)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);
            try
            {
                foreach (var impl in _billingImpls)
                {
                    BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;

                    if (impl.BillingType == billingType)
                    {
                        return impl.ViewForDataInput(revenueHeadDetails, taxPayer);
                    }
                }
                throw new NoBillingTypeSpecifiedException(string.Format("No billing type found in GetViewForCreateBill txp : {0} rh : {1} ", taxPayer.Id, revenueHeadDetails.RevenueHead.Id));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in GetViewForCreateBill txp : {0} rh : {1} ", taxPayer.Id, revenueHeadDetails.RevenueHead.Id));
                throw;
            }
        }



        /// <summary>
        /// Get the view for input
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns></returns>
        public ViewToShowVM GetViewForCreateBill(string taxPayerId, string srevenueHeadId)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);
            //get the tax payer
            Int64 id = 0;
            if (!Int64.TryParse(taxPayerId, out id)) { throw new Exception("No tax payer found"); }
            int revenueHeadId = 0;
            if (!Int32.TryParse(srevenueHeadId, out revenueHeadId)) { throw new Exception("No Revenue head found"); }

            try
            {
                var taxPayer = _taxPayerRepository.GetTaxPayerWithDetails(id);
                if (taxPayer == null) { throw new Exception("No tax payer found") { }; }
                //get the revenue head
                var revenueHeadDetails = _revenueHeadRepository.GetRevenueHeadDetails(revenueHeadId);

                foreach (var impl in _billingImpls)
                {
                    BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;

                    if (impl.BillingType == billingType)
                    {
                        return impl.ViewForDataInput(revenueHeadDetails, taxPayer);
                    }
                }
                throw new NoBillingTypeSpecifiedException(string.Format("No billing type found in GetViewForCreateBill txp : {0} rh : {1} ", taxPayerId, srevenueHeadId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in GetViewForCreateBill txp : {0} rh : {1} ", taxPayerId, srevenueHeadId));
                throw;
            }
        }


        /// <summary>
        /// Validate invoice confirmation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public RevenueHeadDetails ValidateInvoiceDataInput(AdminGenerateInvoiceVM model, string srevenueHeadId, int categoryId, ref List<ErrorModel> errors)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);

            int revenueHeadId = 0;
            if (!Int32.TryParse(srevenueHeadId, out revenueHeadId)) { throw new Exception("No Revenue head found"); }
            try
            {
                //get the revenue head
                var revenueHeadDetails = _revenueHeadRepository.GetRevenueHeadDetails(revenueHeadId);
                BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;

                foreach (var impl in _billingImpls)
                {
                    if (impl.BillingType == billingType)
                    {
                        impl.ValidateInvoiceDataInput(model, ref errors);
                        //if (model.Forms != null && model.Forms.Any()) { errors.AddRange(_coreFormService.ValidateFormValues(model.Forms)); }
                        if (model.Forms != null && model.Forms.Any()) { errors.AddRange(_coreFormService.AddAndValidateFormValueFromUserToCorrespondingDBFormControl(model.Forms, _coreFormService.GetRevenueHeadFormFields(revenueHeadId, categoryId))); }
                        return revenueHeadDetails;
                    }
                }
                throw new NoBillingTypeSpecifiedException(string.Format("No billing type found in ValidateInvoiceDataInput rh : {0} ", srevenueHeadId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in ValidateInvoiceDataInput rh : {0} ", srevenueHeadId));
                throw;
            }
        }


        /// <summary>
        /// If there was an error in the invoice data input page, we need to get the view model back
        /// </summary>
        /// <param name="model"></param>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <returns>ViewToShowVM</returns>
        public ViewToShowVM GetCallBackViewModelForInvoiceDataInput(AdminGenerateInvoiceVM model, string taxPayerId, RevenueHeadDetails revenueHeadDetails)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);

            //get the tax payer
            Int64 id = 0;
            if (!Int64.TryParse(taxPayerId, out id)) { throw new Exception("No tax payer found"); }

            try
            {
                var taxPayer = _taxPayerRepository.GetTaxPayerWithDetails(id);
                if (taxPayer == null) { throw new Exception("No tax payer found") { }; }
                //get the revenue head
                foreach (var impl in _billingImpls)
                {
                    BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;

                    if (impl.BillingType == billingType)
                    {
                        return impl.GetModelForDataInputCallBack(model, revenueHeadDetails, taxPayer);
                    }
                }
                throw new NoBillingTypeSpecifiedException(string.Format("No billing type found in GetViewForCreateBill txp : {0} ", taxPayerId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in GetViewForCreateBill txp : {0}", taxPayerId));
                throw;
            }
        }


        /// <summary>
        /// Add errors to model state
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        public void AddErrorsToModelState(InvoiceController callback, List<ErrorModel> errors)
        {
            AddValidationErrorsToCallback<InvoiceHandler, InvoiceController>(callback, errors);
        }


        /// <summary>
        /// Get model for confirming invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="taxPayerId"></param>
        /// <returns>AdminConfirmingInvoiceVM</returns>
        public AdminConfirmingInvoiceVM GetConfirmingInvoiceVM(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);
            try
            {
                BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;
                foreach (var impl in _billingImpls)
                {
                    if (impl.BillingType == billingType)
                    {
                        AdminConfirmingInvoiceVM vm = impl.GetModelForInvoiceConfirmation(model, revenueHeadDetails);
                        vm.BillingType = billingType;
                        vm.MDAName = revenueHeadDetails.Mda.NameAndCode();
                        vm.RevenueHeadName = revenueHeadDetails.RevenueHead.Name;
                        vm.TaxPayerWithDetails = taxPayer;
                        vm.TaxPayerId = taxPayer.Id;
                        vm.RevenueHeadId = revenueHeadDetails.RevenueHead.Id;
                        vm.Reference = model.Reference;
                        vm.Forms = model.Forms;
                        return vm;
                    }
                }
                throw new NoBillingTypeSpecifiedException(string.Format("No billing type found in GetViewForCreateBill txp : {0} ", taxPayer.Id));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in GetViewForCreateBill txp : {0}", taxPayer.Id));
                throw;
            }
        }


        /// <summary>
        /// Get tamper proof tokens
        /// <para>return a dynamic object with the values SubToken and Token</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTamperProofTokens(string jsonModel)
        {
            DateTime date = DateTime.Now;
            string dateToken = Util.LetsEncrypt(date.ToString(), AppSettingsConfigurations.EncryptionSecret);

            string modelToken = Util.LetsEncrypt(jsonModel, date.ToString() + AppSettingsConfigurations.EncryptionSecret);

            dynamic tamperProofs = new ExpandoObject();
            tamperProofs.SubToken = dateToken;
            tamperProofs.Token = modelToken;
            return tamperProofs;
        }


        /// <summary>
        /// Get AdminConfirmingInvoiceVM model
        /// </summary>
        /// <param name="token"></param>
        /// <param name="subToken"></param>
        /// <returns>AdminConfirmingInvoiceVM</returns>
        public AdminConfirmingInvoiceVM GetAdminConfirmingInvoiceModel(string token, string subToken)
        {
            try
            {
                string dateToken = Util.LetsDecrypt(subToken, AppSettingsConfigurations.EncryptionSecret);

                string modelJson = Util.LetsDecrypt(token, dateToken + AppSettingsConfigurations.EncryptionSecret);

                return JsonConvert.DeserializeObject<AdminConfirmingInvoiceVM>(modelJson);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Could not deserialize token values {0} {1}", token, subToken));
                return null;
            }
        }


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns></returns>
        public CreateInvoiceModel GetCreateInvoiceModel(AdminConfirmingInvoiceVM model)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);

            try
            {
                TaxPayerWithDetails taxPayer = model.TaxPayerWithDetails;
                if (taxPayer == null) { throw new Exception("No tax payer found") { }; }
                foreach (var impl in _billingImpls)
                {
                    if (impl.BillingType == model.BillingType)
                    {
                        var vm = impl.GetCreateInvoiceModel(model, taxPayer);
                        vm.GeneratedFromAdmin = true;
                        vm.RevenueHeadId = model.RevenueHeadId;
                        vm.TaxEntityInvoice.CategoryId = taxPayer.CategoryId;
                        vm.TaxEntityInvoice.TaxEntity = new TaxEntity { Address = taxPayer.Address, Email = taxPayer.Email, PhoneNumber = taxPayer.PhoneNumber, Recipient = taxPayer.Name, TaxPayerIdentificationNumber = taxPayer.TIN, Id = taxPayer.Id, };
                        vm.ExternalRefNumber = model.Reference;
                        vm.Forms = model.Forms?.Select(x => new UserFormDetails { ControlIdentifier = x.ControlIdentifier, FormValue = x.FormValue, RevenueHeadId = model.RevenueHeadId }).ToList();
                        //lets us know that the user generating the invoice is authorized to do so
                        vm.UserIsAuthorized = true;
                        vm.AdminUser = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id };
                        return vm;
                    }
                }
                throw new NoBillingTypeSpecifiedException(string.Format("No billing type found in GetViewForCreateBill txp : {0} ", model.TaxPayerId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in GetViewForCreateBill txp : {0}", model.TaxPayerId));
                throw;
            }
        }

        /// <summary>
        /// Get the list of billable revenue heads 
        /// </summary>
        /// <param name="mdas"></param>
        /// <returns>IEnumerable{RevenueHead}</returns>
        protected virtual List<RevenueHeadLite> GetBillables(IEnumerable<MDA> mdas)
        {
            List<RevenueHeadLite> billables = new List<RevenueHeadLite>();

            foreach (var mda in mdas)
            {
                Logger.Information(string.Format("Searching for active and visible revenue heads for mda {0}", mda.Name));
                //get all billable revenue heads under this mda
                var billableRevenueHeads = mda.RevenueHeads.Where(r => (r.BillingModel != null) && (r.IsActive) && (r.IsVisible)).Select(r => r);
                foreach (var revenueHead in billableRevenueHeads)
                {
                    if (IsVisible(revenueHead))
                    {
                        billables.Add(new RevenueHeadLite { Code = revenueHead.Code, Id = revenueHead.Id, Name = revenueHead.Name, MDAName = mda.Name, MDACode = mda.Code });
                    }
                }
            }
            return billables;
        }


        /// <summary>
        /// Check if the revenue head is visible
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        protected virtual bool IsVisible(RevenueHead revenueHead)
        {
            if (revenueHead.Revenuehead == null) { return true; }
            if (revenueHead.Revenuehead.IsActive && revenueHead.Revenuehead.IsVisible) { IsVisible(revenueHead.Revenuehead); }
            return false;
        }


        /// <summary>
        /// Get a list of active and visble mdas ordered by name
        /// </summary>
        /// <returns>IEnumerable<MDA></returns>
        public IEnumerable<MDA> GetMDAs()
        {
            return _mdaRepository.GetCollection(m => (m.IsVisible == true) && (m.IsActive == true)).OrderBy(k => k.Name);
        }


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="createInvoiceModel"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GenerateInvoice(CreateInvoiceModel createInvoiceModel)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);
            List<ErrorModel> errors = new List<ErrorModel> { };
            createInvoiceModel.ApplySurcharge = true;
            return _coreInvoiceService.TryCreateInvoice(createInvoiceModel, ref errors);
        }


        public InvoiceGeneratedResponseExtn GetInvoiceDetails(string invoiceNumber)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);
            return _coreInvoiceService.GetInvoiceDetailsForPaymentView(invoiceNumber);
        }


        /// <summary>
        /// Get view model for form controls for this taxpayer and revenue head
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>AdminConfirmingInvoiceVM</returns>
        public AdminGenerateInvoiceVM GetDetailsGenerateInvoiceDetailsWithFormFields(string taxPayerId, string revenueHeadId)
        {
            IsAuthorized<InvoiceHandler>(Permissions.GenerateInvoice);
            TaxPayerWithDetails taxPayer = GetTaxPayer(taxPayerId);
            RevenueHeadDetails revenueHeadDetails = GetRevenueHeadDetails(revenueHeadId);

            var model = new AdminGenerateInvoiceVM
            {
                MDAName = revenueHeadDetails.Mda.NameAndCode(),
                RevenueHeadName = revenueHeadDetails.RevenueHead.Name,
                TaxPayerWithDetails = taxPayer,
                TaxPayerId = taxPayer.Id,
                RevenueHeadId = revenueHeadDetails.RevenueHead.Id,
                BillingType = (BillingType)revenueHeadDetails.Billing.BillingType,
                Forms = _coreFormService.GetRevenueHeadFormFields(revenueHeadDetails.RevenueHead.Id, taxPayer.CategoryId).ToList()
            };

            foreach (var impl in _billingImpls)
            {
                BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;

                if (impl.BillingType == billingType)
                {
                    model.PartialToShow = impl.PartialToShow;
                }
            }

            return model;
        }


        /// <summary>
        /// Get payment refs used for this invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        public InvoiceDetails GetInvoicePaymentRefs(string invoiceNumber)
        {
            IsAuthorized<InvoiceHandler>(Permissions.SearchPaymentReference);
            return _coreInvoiceService.GetPaymentReferencesForInvoice(invoiceNumber);
        }

    }
}