using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSubUserVM
    {
        public string DateFilter { get; set; }

        public int FilteredBranch { get; set; }

        public string FilteredAddress { get; set; }

        public string FilteredName { get; set; }

        public RegisterCBSUserModel SubUserInfo { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public bool ShowCreateSubUserModal { get; set; }

        public IEnumerable<TaxEntityProfileLocationVM> Branches { get; set; }

        public IEnumerable<CBSUserTaxEntityProfileLocationVM> SubUsers { get; set; }

        public int SelectedBranch { get; set; }

        public int TotalRecordCount { get; set; }

        public int DataSize { get; set; }

        public string Token { get; set; }

        public FlashObj FlashObj { get; set; }
    }
}