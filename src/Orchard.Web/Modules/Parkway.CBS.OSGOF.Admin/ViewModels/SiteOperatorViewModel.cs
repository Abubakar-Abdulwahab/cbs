using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{
    public class SiteOperatorViewModel
    {
        public string Name { get; set; }

        public string TIN { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string ShortName { get; set; }

        public string Username { get; set; }

        public string RCNumber { get; set; }

        public string PayerId { get; set; }

        public IEnumerable<TaxEntityCategoryViewModel> Categories { get; set; }
        public string CategoryId { get; set; }

    }

    [Obsolete]
    public class TaxEntityCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}