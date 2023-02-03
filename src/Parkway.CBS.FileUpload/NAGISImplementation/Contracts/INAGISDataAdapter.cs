using Parkway.CBS.FileUpload.NAGISImplementation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.NAGISImplementation.Contracts
{
    public interface INAGISDataAdapter
    {
        NAGISDataResponse GetNAGISDataResponseModels(string filePath);
    }
}
