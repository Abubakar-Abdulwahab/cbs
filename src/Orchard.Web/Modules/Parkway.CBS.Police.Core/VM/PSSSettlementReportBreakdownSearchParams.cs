using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportBreakdownSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int SelectedState { get; set; }

        public int SelectedLGA { get; set; }

        public int SelectedService { get; set; }

        public int SelectedSettlementParty { get; set; }

        public int SelectedCommand { get; set; }

        public string FileNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public bool PageData { get; set; }
    }
}