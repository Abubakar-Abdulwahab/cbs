using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.PayeeProcessor.Contract
{
    public interface IBasePayeeFileProcessor
    { 
        void ProcessFile(string fileName, string fileFullPath, string tenantIdentifier); 

    }
}
