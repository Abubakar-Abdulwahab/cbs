using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportBreakdownVM
    {
        public IEnumerable<PSSSettlementBatchItemsVM> ReportRecords { get; set; }

        public dynamic Pager { get; set; }

        public List<StateModelVM> StateAndLGAs { get; set; }

        public IEnumerable<LGAVM> LGAs { get; set; }

        public IEnumerable<CommandVM> Commands { get; set; }

        public IEnumerable<PSServiceVM> Services { get; set; }

        public IEnumerable<PSSFeePartyVM> FeeParties { get; set; }

        public int SelectedState { get; set; }

        public int SelectedLGA { get; set; }

        public int SelectedService { get; set; }

        public int SelectedSettlementParty { get; set; }

        public int SelectedCommand { get; set; }

        public string FileNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public long TotalRecordCount { get; set; }

        public string LogoURL { get; set; }

        public string TenantName { get; set; }

        public decimal TotalReportAmount { get; set; }
    }
}