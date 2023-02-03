using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{
    public class CellSiteReturnModelVM
    {
        public bool CellSiteMatchFound { get; set; }

        public string OSGOFId { get; set; }

        public string LGA { get; set; }

        public string State { get; set; }

        public string CellSite { get; set; }

        public string CellSiteCode { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        public string Reference { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public string AmountValue { get; set; }

        public string Address { get; set; }

        public string Ref { get; set; }

        public string Coords { get; set; }
    }
}