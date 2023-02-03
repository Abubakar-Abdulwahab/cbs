using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSSReceiptService : ICorePSSReceiptService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<IPDFExportEngine> _exportToPDF = new Lazy<IPDFExportEngine>(() =>
        {
            return new PdfEngine();
        });

        public CorePSSReceiptService(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        public CreateReceiptDocumentVM CreateReceiptDocument(ReceiptDetailsVM receiptVM, bool returnByte = false)
        {
            var template = TemplateUtil.RazorTemplateFor("PaymentReceipt", _orchardServices.WorkContext.CurrentSite.SiteName);
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
            receiptVM.ReceiptLogoURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.ReceiptWatermark.ToDescription());
            receiptVM.ReceiptLogoPath = receiptVM.ReceiptLogoURL.Replace("C:\\", "\\").Replace("\\", "\\\\");
            receiptVM.ShortStrip = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.ReceiptThemeShortStrip.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            receiptVM.LongStrip = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + TenantConfigKeys.ReceiptThemeLongStrip.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");

            //Save the barcode
            string encodingString = $"RN:{receiptVM.ReceiptNumber} Name:{receiptVM.Recipient} Amount:{receiptVM.TotalAmountPaid}";
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

            return new CreateReceiptDocumentVM { SavedPath = fileFullPath, FileName = fileName};
        }
    }
}