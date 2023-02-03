using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload
{

    public class OSGOFHeaderValidationModel
    {
        public bool HeaderPresent { get; internal set; }

        public string ErrorMessage { get; internal set; }

        public int IndexOnFile { get; internal set; }
    }


    public class OSGOFCellSiteHeaderValidateObject
    {
        public bool Error { get; set; }

        public string ErrorMessage { get; set; }        
    }


    public class CellSiteFileProcessResponse
    {
        public OSGOFCellSiteHeaderValidateObject HeaderValidationObject { get; set; }

        public List<FileUploadCellSites> FileUploadCellSites { get; set; }
    }
}
