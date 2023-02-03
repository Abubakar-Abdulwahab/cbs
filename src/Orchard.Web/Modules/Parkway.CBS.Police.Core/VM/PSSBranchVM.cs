using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSBranchVM
    {
        public string DateFilter { get; set; }

        public int FilteredState { get; set; }

        public int FilteredLGA { get; set; }

        public string FilteredAddress { get; set; }

        public string FilteredName { get; set; }

        public TaxEntityProfileLocationVM BranchInfo { get; set; }

        public List<StateModelVM> StateLGAs { get; set; }

        public List<LGAVM> ListLGAs { get; set; }

        public List<LGAVM> FilterListLGAs { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public bool ShowCreateBranchModal { get; set; }

        public IEnumerable<TaxEntityProfileLocationVM> Branches { get; set; }

        public int TotalRecordCount { get; set; }

        public int DataSize { get; set; }

        public string Token { get; set; }
    }
}