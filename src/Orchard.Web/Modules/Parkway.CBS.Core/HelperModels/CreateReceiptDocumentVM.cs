using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateReceiptDocumentVM
    {
        public string SavedPath { get; set; }

        public string FileName { get; set; }

        public byte[] DocByte { get; internal set; }
    }
}