using Orchard;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;
using System.Web;
using Parkway.CBS.Payee;
using Parkway.CBS.Module.Web.ViewModels;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.Contracts
{
    public interface IModuleCollectionHandler : IDependency, ICommonBaseHandler
    {
        /// <summary>
        /// get invoice URL
        /// </summary>
        /// <param name="bIN"></param>
        /// <returns></returns>
        string GetInvoiceURL(string bIN);


        InvoiceGeneratedResponseExtn SearchForInvoiceForPaymentView(string bIN);


        /// <summary>
        /// Get the view model for invoice confirmation page
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="processStage"></param>
        /// <returns>ViewToShowVM</returns>
        ViewToShowVM GetDirectAssessmentBillVM(TaxEntity entity, GenerateInvoiceStepsModel processStage, bool isLoggedIn = false);


        /// <summary>
        /// Check if the category requires the user to login
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>bool</returns>
        /// <exception cref="NoCategoryFoundException"></exception>
        bool RequiresLogin(int categoryId);


        /// <summary>
        /// Get view data for self assessment page
        /// </summary>
        /// <returns>GenerateInvoiceVM</returns>
        GenerateInvoiceVM GetSelftAssessmentView();


        /// <summary>
        /// Create a claim token for this request
        /// </summary>
        /// <param name="email"></param>
        /// <param name="requestToken"></param>
        /// <returns>string</returns>
        string CreateClaims(IUser ouser, string requestToken);


        /// <summary>
        /// get view model for register view
        /// </summary>
        /// <returns>RegisterCBSUserObj</returns>
        RegisterCBSUserObj GetRegsiterView();


        /// <summary>
        /// Try register CBS user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        void TryRegisterCBSUser(BaseCollectionController callback, RegisterCBSUserObj model);

        /// <summary>
        /// Get view model for invoice confirmation
        /// </summary>
        /// <returns>dynamic</returns>
        dynamic ConfirmingInvoiceVM(GenerateInvoiceStepsModel processStage, UserDetailsModel user);


        /// <summary>
        /// validate confirm invoice model
        /// </summary>
        /// <param name="collectionController"></param>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <returns>ValidateInvoiceConfirmModel</returns>
        void ValidateConfirmInvoiceModel(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, ref List<ErrorModel> errors);


        /// <summary>
        /// Add the bunch of errors into the controller model, throw a DirtyFormDataException
        /// </summary>
        /// <param name="collectionController"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        void AddErrorsToModel(BaseCollectionController callback, List<ErrorModel> errors);

        /// <summary>
        /// if the validation for invoice confirmation has errors
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns>ValidateInvoiceConfirmModel</returns>
        ValidateInvoiceConfirmModel GetViewModelForConfirmInvoiceModelPostBack(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, UserDetailsModel user, List<ErrorModel> errors);


        /// <summary>
        /// get the model for invoice generation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>CreateInvoiceModel</returns>
        CreateInvoiceModel GetCreateInvoiceModel(GenerateInvoiceStepsModel processStage, TaxEntity user);

        /// <summary>
        /// when the invoice has been confirmed this model would contain the necessary information you would need to 
        /// generate the CreateInvoiceModel
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        InvoiceConfirmedModel GetInvoiceConfirmedModel(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model);


        /// <summary>
        /// Get invoice details for tax payer
        /// </summary>
        /// <param name="createInvoiceModel"></param>
        /// <param name="errors"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGeneratedResponseExtn GetInvoiceDetails(CreateInvoiceModel createInvoiceModel, ref List<ErrorModel> errors);


        /// <summary>
        /// When a user is signed in, we would like to redirect the user to a page where they can progress with thir invoice generation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>ProceedWithInvoiceGenerationVM</returns>
        ProceedWithInvoiceGenerationVM GetModelWhenUserIsSignedIn(UserDetailsModel user, int revenueHeadId, int categoryId);


        /// <summary>
        /// For anons, we would like to get the model for invoice generation
        /// <para>For categories that users are selected from, we should get the select tax entity profile and continue
        /// with invoice generation else we redirect to profile page
        /// </para>
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <param name="profileIdentifier"></param>
        /// <returns>ProceedWithInvoiceGenerationVM</returns>
        ProceedWithInvoiceGenerationVM GetModelForAnonymous(int revenueHeadId, int categoryId, string profileIdentifier);


        /// <summary>
        /// Get the route to redirect the user to for invoice confirmation
        /// </summary>
        /// <param name="billingType"></param>
        /// <returns>dynamic</returns>
        dynamic GetConfirmingInvoiceRoute(BillingType billingType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PaymentsVM GetCollectionReport(long entityId, PaymentsVM model, string datefilter, int skip);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        APIResponse GetPagedPaymentList(long operatorId, int page, string datefilter);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        ReceiptViewModel GetReceiptVM(string receiptNumber);


        /// <summary>
        /// create PDF file for receipt download
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        CreateReceiptDocumentVM CreateReceiptFile(string receiptNumber);


        /// <summary>
        /// Save Netpay payment details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<InvoiceValidationResponseModel> SavePayment(PaymentAcknowledgeMentModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns>PaymentReferenceVM</returns>
        PaymentReferenceVM GetPaymentReferenceDetail(string referenceNumber);


        /// <summary>
        /// Validate form fields
        /// </summary>
        void ValidateForms(IEnumerable<FormControlViewModel> userControlInputs, IEnumerable<FormControlViewModel> expectedControlInputs, ref List<ErrorModel> errors);


        /// <summary>
        /// Get expected forms for this revenue head and categoryId
        /// </summary>
        /// <param name="processStage"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        IEnumerable<FormControlViewModel> GetDBForms(GenerateInvoiceStepsModel processStage);


        /// <summary>
        /// Get the state and LGAs
        /// </summary>
        /// <returns>List{StateModel}</returns>
        List<StateModel> GetStatesAndLGAs();

    }
}
