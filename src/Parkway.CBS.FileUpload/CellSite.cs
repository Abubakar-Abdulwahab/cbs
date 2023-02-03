using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload
{
    public class CellSite
    {
        public string State { get; set; }

        public string LGA { get; set; }

        public string CellSiteId { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        public string Ref { get; set; }
    }

    public class FileUploadCellSites
    {
        public string CellSiteId { get; set; }

        public string Ref { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }
    }
}
