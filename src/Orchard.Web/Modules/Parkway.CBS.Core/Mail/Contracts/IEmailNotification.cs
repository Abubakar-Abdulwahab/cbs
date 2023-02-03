using Orchard;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Mail.Contracts
{
    public interface IEmailNotification : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxEntityObj"></param>
        /// <param name="invoiceObj"></param>
        /// <returns></returns>
        bool InvoiceGeneration(TaxEntity taxEntityObj, Invoice invoiceObj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxEntityObj"></param>
        /// <returns></returns>
        bool AccountRegistration(TaxEntity taxEntityObj);

    }
}
