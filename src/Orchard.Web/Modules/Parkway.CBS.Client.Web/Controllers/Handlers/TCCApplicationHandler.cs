using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class TCCApplicationHandler : ITCCApplicationHandler
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;

        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly IInvoiceManager<Invoice> _invoiceManager;
        private readonly ICoreTaxClearanceCertificateRequestService _taxClearanceCertificateService;
        private readonly ITaxClearanceCertificateRequestFilesManager<TaxClearanceCertificateRequestFiles> _tccRequestFilesManager;

        public TCCApplicationHandler(IOrchardServices orchardServices, ITaxEntityManager<TaxEntity> taxEntityManager, IInvoiceManager<Invoice> invoiceManager, ICoreTaxClearanceCertificateRequestService taxClearanceCertificateService, ITaxClearanceCertificateRequestFilesManager<TaxClearanceCertificateRequestFiles> tccRequestFilesManager)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _taxEntityManager = taxEntityManager;
            _invoiceManager = invoiceManager;
            _taxClearanceCertificateService = taxClearanceCertificateService;
            _tccRequestFilesManager = tccRequestFilesManager;
        }


        /// <summary>
        /// Validate the existence of a particular stateTIN (payerid)
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns>TaxPayerWithDetails</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public TaxPayerWithDetails ValidateStateTIN(string payerId)
        {
            try
            {
                TaxPayerWithDetails taxProfileDetails = _taxEntityManager.GetTaxPayerWithDetails(payerId);
                if (taxProfileDetails == null) { throw new NoRecordFoundException("No record found for State TIN " + payerId); }
                return taxProfileDetails;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
        }


        /// <summary>
        /// Validate the development levy invoice using the invoice number and development revenue head id 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>long</returns>
        public long ValidateDevelopmentLevyInvoice(string invoiceNumber, int developmentRevenueHeadId)
        {
            return _invoiceManager.CheckDevelopmentLevyInvoice(invoiceNumber, developmentRevenueHeadId);
        }

        /// <summary>
        /// Validate if the development levy invoice has not been used
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>bool</returns>
        public bool CheckDevelopmentLevyInvoiceUsage(string invoiceNumber)
        {
            return _taxClearanceCertificateService.CheckCount(x => x.DevelopmentLevyInvoice.InvoiceNumber == invoiceNumber) > 0;
        }

        /// <summary>
        /// Save new tcc application request
        /// </summary>
        /// <param name="tccApplicationRequest"></param>
        /// <param name="accountStatement"></param>
        /// <param name="exemptionCertificate"></param>
        /// <param name="schoolCertificate"></param>
        /// <param name="stateConfig"></param>
        public void SaveTCCRequest(TCCApplicationRequestVM tccApplicationRequest, HttpPostedFileBase accountStatement, HttpPostedFileBase exemptionCertificate, HttpPostedFileBase schoolCertificate, StateConfig stateConfig)
        {
            try
            {
                //Save file
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                Node setting = stateConfig.Node.Where(k => k.Key == TenantConfigKeys.TCCFilePath.ToString())?.FirstOrDefault();
                if (setting == null) { throw new Exception("Unable to fetch TCC file path from StateConfig in TCCApplicationHandler"); }
                DirectoryInfo baseProcessing = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + setting.Value + "/" + siteName);
                string baseFileName = tccApplicationRequest.StateTIN + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss");

                TaxClearanceCertificateRequest tccRequest = new TaxClearanceCertificateRequest
                {
                    ApplicantName = tccApplicationRequest.ApplicantName.Trim(),
                    PhoneNumber = tccApplicationRequest.PhoneNumber.Trim(),
                    Occupation = tccApplicationRequest.Occupation?.Trim(),
                    TIN = tccApplicationRequest.StateTIN.Trim(),
                    ResidentialAddress = tccApplicationRequest.ResidentialAddress.Trim(),
                    OfficeAddress = tccApplicationRequest.OfficeAddress.Trim(),
                    IsRentedApartment = tccApplicationRequest.IsRentedApartment,
                    DevelopmentLevyInvoice = new Invoice { Id = tccApplicationRequest.DevelopmentLevyInvoiceId },
                    LandlordName = tccApplicationRequest.LandlordName?.Trim(),
                    LandlordAddress = tccApplicationRequest.LandlordAddress?.Trim(),
                    RequestReason = tccApplicationRequest.RequestReason.Trim(),
                    ExemptionCategory = tccApplicationRequest.ExemptionTypeId,
                    HusbandName = tccApplicationRequest.HusbandName?.Trim(),
                    HusbandAddress = tccApplicationRequest.HusbandAddress?.Trim(),
                    InstitutionName = tccApplicationRequest.InstitutionName?.Trim(),
                    IdentificationNumber = tccApplicationRequest.IdCardNumber?.Trim(),
                    TaxEntity = new TaxEntity { Id = tccApplicationRequest.TaxEntityId },
                    Status = (int)TCCRequestStatus.PendingApproval,
                    ApplicationYear = tccApplicationRequest.ApplicationYear,
                    ApprovalStatusLevelId = (int)TCCApprovalLevel.FirstLevelApprover
                };

                _taxClearanceCertificateService.SaveTCCRequest(tccRequest);

                List<TaxClearanceCertificateRequestFiles> fileList = new List<TaxClearanceCertificateRequestFiles>();               

                if (tccApplicationRequest.ExemptionTypeId == (int)TCCExemptionType.WhollyExempted)
                {
                    TaxClearanceCertificateRequestFiles exemptionCertificateFile = new TaxClearanceCertificateRequestFiles();
                    exemptionCertificateFile.TCCFileUploadTypeId = (int)TCCFileUploadType.ExemptionCertificate;
                    exemptionCertificateFile.OriginalFileName = exemptionCertificate.FileName;
                    exemptionCertificateFile.ContentType = exemptionCertificate.ContentType;
                    exemptionCertificateFile.FilePath = Path.Combine(baseProcessing.FullName, $"{baseFileName}_{TCCFileUploadType.ExemptionCertificate.ToString()}{Path.GetExtension(exemptionCertificate.FileName)}");
                    exemptionCertificateFile.TaxClearanceCertificateRequest = new TaxClearanceCertificateRequest { Id = tccRequest.Id };
                    fileList.Add(exemptionCertificateFile);
                    exemptionCertificate.SaveAs(exemptionCertificateFile.FilePath);
                }
                else if (tccApplicationRequest.ExemptionTypeId == (int)TCCExemptionType.Student)
                {
                    TaxClearanceCertificateRequestFiles schoolCertificateFile = new TaxClearanceCertificateRequestFiles();
                    schoolCertificateFile.TCCFileUploadTypeId = (int)TCCFileUploadType.SchoolLeavingCertificate;
                    schoolCertificateFile.OriginalFileName = schoolCertificate.FileName;
                    schoolCertificateFile.ContentType = schoolCertificate.ContentType;
                    schoolCertificateFile.FilePath = Path.Combine(baseProcessing.FullName, $"{baseFileName}_{TCCFileUploadType.SchoolLeavingCertificate.ToString()}{Path.GetExtension(schoolCertificate.FileName)}");
                    schoolCertificateFile.TaxClearanceCertificateRequest = new TaxClearanceCertificateRequest { Id = tccRequest.Id };
                    fileList.Add(schoolCertificateFile);
                    schoolCertificate.SaveAs(schoolCertificateFile.FilePath);
                }

                if (!_tccRequestFilesManager.SaveBundleUnCommit(fileList))
                {
                    throw new Exception("Unable to save TCC request details");
                }
            }
            catch (Exception)
            {
                _tccRequestFilesManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Validate if user upload all the necessary documents during tcc request application
        /// </summary>
        /// <param name="accountStatement"></param>
        /// <param name="exemptionCertificate"></param>
        /// <param name="schoolCertificate"></param>
        /// <param name="exemptionTypeId"></param>
        /// <param name="validationErrors"></param>
        public void DoFileUploadValidation(HttpPostedFileBase accountStatement, HttpPostedFileBase exemptionCertificate, HttpPostedFileBase schoolCertificate, int exemptionTypeId, List<ErrorModel> validationErrors)
        {
            List<HttpPostedFileBase> files = new List<HttpPostedFileBase> { };
            //if (accountStatement == null || accountStatement.ContentLength <= 0)
            //{
            //    validationErrors.Add(new ErrorModel { FieldName = "accountStatement", ErrorMessage = "Statement of account file is required in the supported format (.pdf,.jpg,.jpeg,.png)" });
            //}
            //else { files.Add(accountStatement); }

            if (exemptionTypeId == (int)TCCExemptionType.WhollyExempted)
            {
                if (exemptionCertificate == null || exemptionCertificate.ContentLength <= 0)
                {
                    validationErrors.Add(new ErrorModel { FieldName = "accountStatement", ErrorMessage = "Exemption certificate file is required in the supported format (.pdf,.jpg,.jpeg,.png)" });
                }
                else { files.Add(exemptionCertificate); }
            }
            else if (exemptionTypeId == (int)TCCExemptionType.Student)
            {
                if (schoolCertificate == null || schoolCertificate.ContentLength <= 0)
                {
                    validationErrors.Add(new ErrorModel { FieldName = "accountStatement", ErrorMessage = "School leaving certifcate file is required in the supported format (.pdf,.jpg,.jpeg,.png)" });
                }
                else { files.Add(schoolCertificate); }
            }
            CheckFileType(files, validationErrors);
        }


        /// <summary>
        /// Check file type for uploads
        /// </summary>
        /// <param name="uploadedFiles"></param>
        /// <param name="validationErrors"></param>
        public void CheckFileType(List<HttpPostedFileBase> uploadedFiles, List<ErrorModel> validationErrors)
        {
            if (validationErrors == null) { validationErrors = new List<ErrorModel>(); }
            List<string> mimes = new List<string>() { "jpg", "png", "pdf", "jpeg" };
            List<string> extensions = new List<string>() { ".jpg", ".png", ".pdf", ".jpeg" };

            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                if (uploadedFiles[i].ContentType == null) { continue; }
                if (!mimes.Any(type => uploadedFiles[i].ContentType.Contains(type)) && !extensions.Any(extension => uploadedFiles[i].FileName.EndsWith(extension)))
                {
                    validationErrors.Add(new ErrorModel { FieldName = uploadedFiles[i].FileName, ErrorMessage = ErrorLang.filetypenotallowed(string.Join(",", extensions.ToArray())).ToString() });
                }
            }
        }


        /// <summary>
        /// Check if development levy invoice with specified invoice number has been used.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="developmentLevyRevenueHeadId"></param>
        /// <returns></returns>
        public APIResponse CheckIfDevelopmentLevyInvoiceHasBeenUsed(string invoiceNumber, int developmentLevyRevenueHeadId)
        {
            try
            {
                var invoice = _invoiceManager.CheckDevelopmentLevyInvoice(invoiceNumber);
                if (invoice == null) { return new APIResponse { Error = true, ResponseObject = "Invoice does not exist." }; }
                if (invoice.RevenueHeadId != developmentLevyRevenueHeadId) { return new APIResponse { Error = true, ResponseObject = "Invoice number does not belong to a development levy invoice." }; }
                if (invoice.Status != (int)InvoiceStatus.Paid) { return new APIResponse { Error = true, ResponseObject = "Invoice has not been paid for." }; }
                if(!_taxClearanceCertificateService.CheckIfInvoiceHasBeenUsed(invoice.Id))
                {
                    return new APIResponse { ResponseObject = true };
                }
                else { return new APIResponse { Error = true, ResponseObject = "Invoice has already been used." }; }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Checks if specified payer id is different from that of the logged in user with the specified tax entity id.
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse CheckIfPayerIdValid(string payerId, long taxEntityId)
        {
            try
            {
                TaxPayerWithDetails taxPayer = _taxEntityManager.GetTaxPayerWithDetails(payerId);
                if (taxPayer != null)
                {
                    if(taxPayer.Id != taxEntityId)
                    {
                        return new APIResponse { ResponseObject = taxPayer };
                    }
                    else
                    {
                        return new APIResponse { Error = true, ResponseObject = "Payer Id must be different from that of the logged in user." };
                    }
                }
                else { return new APIResponse { Error = true, ResponseObject = "Payer Id does not exist" }; }
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Do validation for form fields
        /// <para>validation errors fields will contain error models if any error occurs</para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="validationErrors"></param>
        public void DoValidationForFormFields(TCCApplicationRequestVM model, List<ErrorModel> validationErrors)
        {

            if (!Enum.GetValues(typeof(TCCExemptionType)).Cast<int>().ToList().Contains(model.ExemptionTypeId))
            {
                validationErrors.Add(new ErrorModel { ErrorMessage = "Invalid Exemption Type specified.", FieldName = "ExemptionTypeId" });
            }

            if (!Util.DoPhoneNumberValidation(model.PhoneNumber))
            {
                validationErrors.Add(new ErrorModel { ErrorMessage = "Enter a valid phonenumber.", FieldName = "ExemptionTypeId" });
            }

            if (model.IsRentedApartment)
            {
                if(string.IsNullOrEmpty(model.LandlordAddress))
                {
                    validationErrors.Add(new ErrorModel { ErrorMessage = ErrorLang.fieldrequired("Landlord Address").ToString(), FieldName = "LandlordAddress" });
                }
                if (string.IsNullOrEmpty(model.LandlordName))
                {
                    validationErrors.Add(new ErrorModel { ErrorMessage = ErrorLang.fieldrequired("Landlord Name").ToString(), FieldName = "LandlordName" });
                }
            }

            if (model.ExemptionTypeId == (int)TCCExemptionType.HouseWife)
            {
                if (string.IsNullOrEmpty(model.HusbandAddress))
                {
                    validationErrors.Add(new ErrorModel { ErrorMessage = ErrorLang.fieldrequired("Husband Address").ToString(), FieldName = "HusbandAddress" });
                }

                if (string.IsNullOrEmpty(model.HusbandName))
                {
                    validationErrors.Add(new ErrorModel { ErrorMessage = ErrorLang.fieldrequired("Husband Name").ToString(), FieldName = "HusbandName" });
                }
            }
            else if(model.ExemptionTypeId == (int)TCCExemptionType.Student)
            {
                if (string.IsNullOrEmpty(model.InstitutionName))
                {
                    validationErrors.Add(new ErrorModel { ErrorMessage = ErrorLang.fieldrequired("Institution Name").ToString(), FieldName = "InstitutionName" });
                }

                if (string.IsNullOrEmpty(model.IdCardNumber))
                {
                    validationErrors.Add(new ErrorModel { ErrorMessage = ErrorLang.fieldrequired("Id Card Number").ToString(), FieldName = "IdCardNumber" });
                }
            }

        }


    }
}