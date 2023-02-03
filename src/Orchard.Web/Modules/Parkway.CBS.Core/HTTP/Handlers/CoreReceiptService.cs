using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.IO;
using System.Linq;
using ZXing;


namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreReceiptService : CoreBaseService, ICoreReceiptService
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        private readonly IReceiptManager<Receipt> _receiptManager;
        private readonly ITransactionLogManager<TransactionLog> _transLogRepo;

        private readonly Lazy<IPDFExportEngine> _exportToPDF = new Lazy<IPDFExportEngine>(() =>
        {
            return new PdfEngine();
        });

        public CoreReceiptService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IReceiptManager<Receipt> receiptManager, ITransactionLogManager<TransactionLog> transLogRepo) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _receiptManager = receiptManager;
            _transLogRepo = transLogRepo;
        }


        /// <summary>
        /// Generate receipt
        /// </summary>
        /// <param name="receipt">Receipt</param>
        /// <returns>Receipt</returns>
        /// <exception cref="CannotCreateReceiptException"></exception>
        public Receipt SaveTransactionReceipt(Receipt receipt)
        {
            if (!_receiptManager.Save(receipt))
            { throw new CannotCreateReceiptException(string.Format("Cannot create receipt for invoice number {0}", receipt.Invoice.InvoiceNumber)); }
            return receipt;
        }


        /// <summary>
        /// Evict receipt object from cache
        /// </summary>
        /// <param name="receipt"></param>
        public void EvictReceiptObject(Receipt receipt)
        {
            _receiptManager.Evict(receipt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>TransactionLog</returns>
        public ReceiptViewModel GetReceiptVMByReceiptNumber(string receiptNumber)
        {
            var result = _receiptManager.GetReceiptDetails(receiptNumber);
            if(result == null || !result.TransactionLogInvoiceDetails.Any()) { throw new NoRecordFoundException("No record found for receipt null " + receiptNumber); }
            return result;
        }


        public CreateReceiptDocumentVM CreateReceiptDocument(ReceiptViewModel receiptVM, bool returnByte = false)
        {
            var template = TemplateUtil.RazorTemplateFor("PaymentReceipt");
            string fileName = receiptVM.ReceiptNumber + ".pdf";

            //Check if the file already exist in the folder
            string vSavingPath = template.SavingPath + "/" + _orchardServices.WorkContext.CurrentSite.SiteName + "/";
            string savingPath = System.Web.HttpContext.Current.Server.MapPath(vSavingPath);
            string fileFullPath = System.Web.HttpContext.Current.Server.MapPath(vSavingPath + fileName);

            if (!returnByte)
            {
                if (File.Exists(fileFullPath))
                {
                    return new CreateReceiptDocumentVM { SavedPath = fileFullPath, FileName = fileName };
                }
            }
            

            receiptVM.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName.ToUpper();
            receiptVM.ReceiptLogoURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.TenantThemeLogo.ToDescription());
            receiptVM.ReceiptLogoPath = receiptVM.ReceiptLogoURL.Replace("C:\\", "\\").Replace("\\", "\\\\");
            receiptVM.ShortStrip = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.ReceiptThemeShortStrip.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            receiptVM.LongStrip = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.ReceiptThemeLongStrip.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            receiptVM.TenantNameSuffix = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(k => k.Key == TenantConfigKeys.StateDisplayName.ToString()).FirstOrDefault()?.Value;
            //Save the barcode
            string encodingString = $"RN:{receiptVM.ReceiptNumber} Name:{receiptVM.TaxPayerName} Amount:{receiptVM.TotalAmountPaid}";
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE
            };
            string barcodeSavingPath = System.Web.HttpContext.Current.Server.MapPath(vSavingPath + receiptVM.ReceiptNumber + "-Barcode.png");
            var savedFileDir = System.Web.HttpContext.Current.Server.MapPath(vSavingPath);
            Directory.CreateDirectory(savedFileDir);

            barcodeWriter.Write(encodingString).Save(barcodeSavingPath);
            receiptVM.BarCodeSavingPath = barcodeSavingPath;

            //Check if the file already exist in the folder
            byte[] receiptByte = null;
            if (returnByte)
            {
                receiptByte = _exportToPDF.Value.DownloadAsPdfNRecoLib(null, template.File, receiptVM, template.BasePath);
            }
            else
            {
                _exportToPDF.Value.SaveAsPdfNRecoLib(null, template.File, receiptVM, fileName, vSavingPath, template.BasePath);
            }
            File.Delete(barcodeSavingPath);

            return new CreateReceiptDocumentVM { SavedPath = fileFullPath, FileName = fileName, DocByte = receiptByte };
        }


        /// <summary>
        /// Get receipt number by receipt Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string | Receipt number</returns>
        public string GetReceiptNumberById(long id)
        {
            var result = _receiptManager.Get(r => r.Id == id);
            if(result == null) { throw new NoRecordFoundException(string.Format("No receipt record found for reciept Id {0}", id)); }
            return result.ReceiptNumber;
        }


    }
}