using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{
    public class TaxEntityProfileVM
    {
        public string PayerId { get; set; }

        public string Name { get; set; }

        public Int64 Id { get; set; }

        public string TIN { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string TaxPayerCode { get; set; }
    }

    public class OperatorProfileVM
    {
        public string PayerId { get; set; }

        public IEnumerable<TaxEntityProfileVM> Operators { get; set; }
    }

}