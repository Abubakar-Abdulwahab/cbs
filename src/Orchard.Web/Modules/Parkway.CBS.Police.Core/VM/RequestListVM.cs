using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestListVM
    {
        public IEnumerable<PSSRequestVM> Requests { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public Int64 DataSize { get; set; }

        public long TaxEntityId { get; set; }

        public string startDateString { get; set; }

        public string endDateString { get; set; }

        public int requestStatus { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public List<StateModelVM> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public int SelectedState { get; set; }

        public int SelectedStateLGA { get; set; }

        public string BranchName { get; set; }

        public IEnumerable<TaxEntityProfileLocationVM> Branches { get; set; }

        public int SelectedBranch { get; set; }
    }
}