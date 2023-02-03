using Orchard;
using Orchard.Localization;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using System.Globalization;
using Newtonsoft.Json;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class PaymentHandler : BaseHandler, IPaymentHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICorePaymentService _corePaymentService;
        private readonly ICoreInvoiceService _coreInvoiceService;
        private readonly ICoreReceiptService _coreReceiptService;
        //private readonly ICollectionReportFilter _collectionReportFilter;


        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        public Localizer T { get; set; }

        public PaymentHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository,
            ICorePaymentService corePaymentService, ICoreInvoiceService coreInvoiceService, ICoreReceiptService coreReceiptService)
            : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _settingsRepository = settingsRepository;
            _corePaymentService = corePaymentService;
            _coreInvoiceService = coreInvoiceService;
            _coreReceiptService = coreReceiptService;
        }



        public AddPaymentVM GetAddPaymentVM(string invoiceNumber)
        {
            IsAuthorized<PaymentHandler>(Permissions.AddPayments);
            var result = _coreInvoiceService.GetInvoiceDetailsForPaymentView(invoiceNumber);
            if (result == null)
            { throw new NoInvoicesMatchingTheParametersFoundException("no invoice found " + invoiceNumber); }

            if (result.InvoiceStatus == InvoiceStatus.Paid) { throw new InvoiceAlreadyPaidForException("invoice has already been paid for " + invoiceNumber); }
            return new AddPaymentVM { Email = result.Email, InvoiceNumber = result.InvoiceNumber, PhoneNumber = result.PhoneNumber, Recipient = result.Recipient, TIN = result.TIN, AmountDue = result.AmountDue, DueDate = result.DueDate };
        }


        /// <summary>
        /// update invoice payment
        /// </summary>
        /// <param name="model">AddPaymentVM</param>
        public void UpdateInvoicePayment(PaymentController callback, AddPaymentVM model)
        {
            //check live mode
            var val = Core.Utilities.AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.IsLive);
            if (string.IsNullOrEmpty(val))
            {
                try
                {
                    var boolVal = Convert.ToBoolean(val);
                    if (boolVal) { throw new UserNotAuthorizedForThisActionException(); }
                }
                catch (Exception)
                { throw; }
            }

            IsAuthorized<PaymentHandler>(Permissions.AddPayments).IsModelValid<PaymentHandler, PaymentController>(callback);
            DateTime paymentDate = DateTime.Now;
            try
            { paymentDate = DateTime.ParseExact(model.PaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
            catch (Exception)
            {
                AddValidationErrorsToCallback<PaymentHandler, PaymentController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Date format incorrect. Expected format dd/MM/YYYY", FieldName = "PaymentDate" } } });
                throw new DirtyFormDataException();
            }

            if(paymentDate > DateTime.Now)
            {
                AddValidationErrorsToCallback<PaymentHandler, PaymentController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Date is in the future. Add a valid date.", FieldName = "PaymentDate" } } });
                throw new DirtyFormDataException();
            }

            if (string.IsNullOrEmpty(model.AmountPaid))
            {
                AddValidationErrorsToCallback<PaymentHandler, PaymentController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Amount paid is required", FieldName = "AmountPaid" } } });
                throw new DirtyFormDataException();
            }

            decimal amountPaid = 00m;
            bool parsed = decimal.TryParse(model.AmountPaid, out amountPaid);
            if (!parsed || amountPaid < 0.01m)
            {
                AddValidationErrorsToCallback<PaymentHandler, PaymentController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Enter a valid amount", FieldName = "AmountPaid" } } });
                throw new DirtyFormDataException();
            }

            //if (model.PaymentProvider == PaymentProvider.None)
            //{
            //    AddValidationErrorsToCallback<PaymentHandler, PaymentController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Please select a valid payment method", FieldName = "PaymentChannel" } } });
            //    throw new DirtyFormDataException();
            //}
            //gaurd against posting the same payment twice
            string adminMakePaymentRef = string.Format("CBS|{0}|{1}|{2}|{3}|{4}|{5}", model.InvoiceNumber, "011", Math.Round(amountPaid, 2).ToString("F"), DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss"), _orchardServices.WorkContext.CurrentUser.Id.ToString(), model.Reference);//invoice number, bank code, amount, date, admin Id

            InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
            {
                Channel = (int)PaymentChannel.BankBranch,
                InvoiceNumber = model.InvoiceNumber,
                PaymentReference = adminMakePaymentRef,
                AmountPaid = Math.Round(amountPaid, 2),
                PaymentDate = paymentDate,
                TransactionDate = DateTime.Now,
                RequestDump = JsonConvert.SerializeObject(model),
                UpdatedByAdmin = true,
                AdminUser = GetUser(_orchardServices.WorkContext.CurrentUser.Id),
                Bank = "FIRST BANK OF NIGERIA PLC",
                BankBranch = "Yaba, Lagos",
                BankCode = "011",
                BankChannel = "Bank Branch",
                SlipNumber = model.Reference,
                PaymentMethodId = 1,
                PaymentMethod = "Pay Direct",
                PaymentProvider = (int)PaymentProvider.Bank3D
            }, PaymentProvider.Bank3D);
        }


        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            IsAuthorized<PaymentHandler>(permission);
        }


        /// <summary>
        /// Get byte doc for receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        public CreateReceiptDocumentVM CreateReceiptByteFile(string receiptNumber)
        {
            var vm = _coreReceiptService.GetReceiptVMByReceiptNumber(receiptNumber);
            return _coreReceiptService.CreateReceiptDocument(vm, true);
        }
    }
}