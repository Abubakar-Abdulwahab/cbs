using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CreateCertificateDocumentVM
    {
        public string SavedPath { get; set; }

        public string FileName { get; set; }

        public byte[] DocByte { get; internal set; }
    }
}