using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.NAGISImplementation.Models
{
    public class NAGISTemplateHeaderValidation
    {
        public bool HeaderPresent { get; internal set; }

        public string ErrorMessage { get; internal set; }

        public int IndexOnFile { get; internal set; }

    }
}
