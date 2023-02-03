using Orchard;
using Parkway.CBS.Core.HelperModels;
using OrchardPermission = Orchard.Security.Permissions.Permission;


namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts
{
    public interface IPAYEReceiptUtilizationHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="CanViewReceiptUtilizations"></param>
        void CheckForPermission(OrchardPermission CanViewReceiptUtilizations);


        /// <summary>
        /// Get paginated PAYE receipts
        /// </summary>
        PAYEReceiptReportVM GetPAYEReceipts(PAYEReceiptSearchParams receiptSearchParams);


        /// <summary>
        /// Gets receipt utilizations report for a specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        PAYEReceiptUtilizationReportObj GetUtilizationsReport(string receiptNumber);

    }
}
