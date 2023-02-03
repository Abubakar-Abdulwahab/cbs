using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ZXing;
using Newtonsoft.Json;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreTaxClearanceCertificateRequestService : ICoreTaxClearanceCertificateRequestService
    {
        private readonly ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest> _taxClearanceCertifcateManager;
        private readonly ITaxClearanceCertificateAuthorizedSignaturesManager<TaxClearanceCertificateAuthorizedSignatures> _authorizedSignaturesManager;
        private readonly ITaxClearanceCertificateManager<TaxClearanceCertificate> _tccManager;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<IPDFExportEngine> _exportToPDF = new Lazy<IPDFExportEngine>(() =>
        {
            return new PdfEngine();
        });

        public CoreTaxClearanceCertificateRequestService(ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest> taxClearanceCertifcateManager, ITaxClearanceCertificateAuthorizedSignaturesManager<TaxClearanceCertificateAuthorizedSignatures> authorizedSignaturesManager, IOrchardServices orchardServices, ITaxClearanceCertificateManager<TaxClearanceCertificate> tccManager)
        {
            Logger = NullLogger.Instance;
            _taxClearanceCertifcateManager = taxClearanceCertifcateManager;
            _authorizedSignaturesManager = authorizedSignaturesManager;
            _orchardServices = orchardServices;
            _tccManager = tccManager;
        }

        /// <summary>
        /// Get count of enitities
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>int</returns>
        public int CheckCount(Expression<Func<TaxClearanceCertificateRequest, bool>> lambda)
        {
            try
            {
                return _taxClearanceCertifcateManager.Count(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Save the TCC request
        /// </summary>
        /// <param name="clearanceCertifcateRequest"></param>
        /// <returns></returns>
        public void SaveTCCRequest(TaxClearanceCertificateRequest clearanceCertifcateRequest)
        {
            try
            {
                if (!_taxClearanceCertifcateManager.Save(clearanceCertifcateRequest))
                {
                    throw new Exception("Unable to save TCC request details");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Retrieves Tax Clearance Certificate
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        public CreateReceiptDocumentVM CreateCertificateDocument(string tccNumber, bool returnByte = false)
        {
            TCCertificateDetailsVM certificate = _tccManager.GetCertificateDetails(tccNumber);
            if (certificate == null) { throw new NoRecordFoundException("404 for Tax Clearance Certificate with TCC Number " + tccNumber); }
            DateTime tccYear = new DateTime(certificate.ApplicationYear, 12, 31);
            List<TCCYearlyTaxSummary> taxSummary = certificate.TotalIncomeAndTaxAmountPaidWithYearCollection.ToList();
            TCCertificateVM tccDetailsVM = new TCCertificateVM {
                ApplicantName = certificate.ApplicantName,
                Address = certificate.ResidentialAddress,
                YearAppliedFor = certificate.ApplicationYear,
                YearOne = tccYear.AddYears(-2).Year,
                YearTwo = tccYear.AddYears(-1).Year,
                ExpireDate = tccYear.ToString("dd MMMM yyyy"),
                LastDateModified = certificate.LastDateModified,
                YearOneTotalIncome = taxSummary.ElementAt(2).TotalIncome,
                YearTwoTotalIncome = taxSummary.ElementAt(1).TotalIncome,
                YearAppliedForTotalIncome = taxSummary.ElementAt(0).TotalIncome,
                YearOneTaxPaid = taxSummary.ElementAt(2).TotalTaxPaid,
                YearTwoTaxPaid = taxSummary.ElementAt(1).TotalTaxPaid,
                YearAppliedForTaxPaid = taxSummary.ElementAt(0).TotalTaxPaid
            };
            //var template = TemplateUtil.RazorTemplateFor("TCCertificate");
            
            var template = JsonConvert.DeserializeObject<TCCTemplateVM>(certificate.TaxClearanceCertificateTemplate).Template;
            string fileName = tccDetailsVM.TCCNumber + ".pdf";
            //Check if the file already exist in the folder
            tccDetailsVM.LogoURL = HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.TenantThemeLogo.ToDescription());
            tccDetailsVM.LogoURLPath = tccDetailsVM.LogoURL.Replace("C:\\", "\\").Replace("\\", "\\\\");
            tccDetailsVM.TCCCertificateBGPath = HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.TCCCertificateBG.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            string vSavingPath = template.SavingPath + "/" + _orchardServices.WorkContext.CurrentSite.SiteName + "/";
            string savingPath = HttpContext.Current.Server.MapPath(vSavingPath);
            string fileFullPath = HttpContext.Current.Server.MapPath(vSavingPath + fileName);

            if (!returnByte)
            {
                if (File.Exists(fileFullPath))
                {
                    return new CreateReceiptDocumentVM { SavedPath = fileFullPath, FileName = fileName };
                }
            }

            //Save the barcode
            string encodingString = $"Name: {tccDetailsVM.ApplicantName} Certificate No: {tccDetailsVM.TCCNumber} Expiry Date: {tccDetailsVM.ExpireDate}";
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
            };
            string barcodeSavingPath = System.Web.HttpContext.Current.Server.MapPath(vSavingPath + tccDetailsVM.TCCNumber + "-Barcode.png");
            var savedFileDir = System.Web.HttpContext.Current.Server.MapPath(vSavingPath);
            Directory.CreateDirectory(savedFileDir);
            barcodeWriter.Options.Margin = 0;
            barcodeWriter.Write(encodingString).Save(barcodeSavingPath);
            tccDetailsVM.BarCodeSavingPath = barcodeSavingPath;
            tccDetailsVM.RevenueOfficerSignature = $"data:{certificate.RevenueOfficerSignatureContentType};base64,{certificate.RevenueOfficerSignatureBlob}";
            tccDetailsVM.DirectorOfRevenueSignature = $"data:{certificate.DirectorOfRevenueSignatureContentType};base64,{certificate.DirectorOfRevenueSignatureBlob}";

            //Check if the file already exist in the folder
            byte[] receiptByte = null;
            if (returnByte)
            {
                receiptByte = _exportToPDF.Value.DownloadAsPdfNRecoLib(null, template.File, tccDetailsVM, template.BasePath);
            }
            else
            {
                _exportToPDF.Value.SaveAsPdfNRecoLib(null, template.File, tccDetailsVM, fileName, vSavingPath, template.BasePath);
            }
            File.Delete(barcodeSavingPath);

            return new CreateReceiptDocumentVM { SavedPath = fileFullPath, FileName = fileName, DocByte = receiptByte };
        }

        /// <summary>
        /// Checks if invoice with specified invoice id has been used by an existing tcc request
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public bool CheckIfInvoiceHasBeenUsed(long invoiceId)
        {
            try
            {
                return _taxClearanceCertifcateManager.Count(x => x.DevelopmentLevyInvoice.Id == invoiceId) > 0;
            }
            catch (Exception) { throw; }
        }
    }
}