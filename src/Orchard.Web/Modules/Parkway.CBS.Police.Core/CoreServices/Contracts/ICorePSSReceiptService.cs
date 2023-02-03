using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSSReceiptService : IDependency
    {
        CreateReceiptDocumentVM CreateReceiptDocument(ReceiptDetailsVM receiptVM, bool returnByte = false);
    }
}
