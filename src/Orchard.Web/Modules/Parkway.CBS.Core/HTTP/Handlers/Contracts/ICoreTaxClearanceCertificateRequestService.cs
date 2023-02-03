using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Linq.Expressions;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreTaxClearanceCertificateRequestService : IDependency
    {
        /// <summary>
        /// Get count of enitities
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>int</returns>
        int CheckCount(Expression<Func<TaxClearanceCertificateRequest, bool>> lambda);

        /// <summary>
        /// Save the TCC request
        /// </summary>
        /// <param name="clearanceCertifcateRequest"></param>
        /// <returns></returns>
        void SaveTCCRequest(TaxClearanceCertificateRequest clearanceCertifcateRequest);

        /// <summary>
        /// Retrieves Tax Clearance Certificate
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        CreateReceiptDocumentVM CreateCertificateDocument(string tccNumber, bool returnByte = false);

        /// <summary>
        /// Checks if invoice with specified invoice id has been used by an existing tcc request
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        bool CheckIfInvoiceHasBeenUsed(long invoiceId);
    }
}
