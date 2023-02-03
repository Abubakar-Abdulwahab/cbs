using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CBSUserTaxEntityProfileLocationVM
    {
        public int UserPartRecordId { get; set; }

        public string Name { get; set; }

        public bool Verified { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string BranchName { get; set; }

        public TaxEntityProfileLocationVM Location { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsActive { get; set; }
    }
}