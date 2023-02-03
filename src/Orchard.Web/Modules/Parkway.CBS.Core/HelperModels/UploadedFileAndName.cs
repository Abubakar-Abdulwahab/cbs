using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class UploadedFileAndName
    {
        public HttpPostedFileBase Upload { get; set; }

        public string UploadName { get; set; }

        public List<string> AcceptedMimes { get; set; }

        public List<string> AcceptedExtensions { get; set; }

        public int MaxSize { get; set; }
    }
}