using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPssInvoiceHandler : IDependency
    {
        /// <summary>
        /// get invoice URL
        /// </summary>
        /// <param name="bIN"></param>
        /// <returns></returns>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        string GetInvoiceURL(string bin);

        /// <summary>
        ///Get generated invoice response using specified BIN 
        /// </summary>
        /// <param name="bIN">Invoice Number</param>
        /// <returns></returns>
        InvoiceGeneratedResponseExtn SearchForInvoiceForPaymentView(string bIN);
    }
}
