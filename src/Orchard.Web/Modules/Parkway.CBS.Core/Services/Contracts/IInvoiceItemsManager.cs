using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IInvoiceItemsManager<InvoiceItems> : IDependency, IBaseManager<InvoiceItems>
    {
        /// <summary>
        /// this method is used to save a collection of invoice items in the InvoiceItemFormsAndPosition collection
        /// but without commiting. Committing the transaction is let for the request session that has been started
        /// by the framework with the request is initiated
        /// <para>returns true is insertion is successful or false if not</para>
        /// </summary>
        /// <param name="invoiceItemAndPostion"></param>
        /// <returns>bool</returns>
        bool SaveBundleUnCommit(List<InvoiceItemFormsAndPosition> invoiceItemAndPostion);
    }
}
