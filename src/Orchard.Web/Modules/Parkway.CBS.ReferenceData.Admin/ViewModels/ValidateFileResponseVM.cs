using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class ValidateFileResponseVM
    {
        public bool ErrorOccurred { get; set; }

        public Int64 BatchId { get; set; }

        public string ErrorMessage { get; set; }

        public string BatchRef { get; set; }

        public string RedirectToAction { get; set; }
    }
}