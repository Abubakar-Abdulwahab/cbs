using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class CollectionReportVM
    {
        public string InvoiceNumber { get; set; }

        public string FileNumber { get; set; }

        public string From { get; set; }

        public string End { get; set; }

        public List<PSSRequestTypeVM> ServiceRequestTypes { get; set; }

        public List<PSSTransactionLogVM> Reports { get; set; }

        public long TotalNumberOfIndividualRequestsReceived { get; set; }

        public dynamic Pager { get; set; }

        public int TotalRequestRecord { get; set; }

        public CollectionPaymentDirection PaymentDirection { get; set; }

        public decimal TotalRequestAmount { get; set; }

        /// <summary>
        /// Logo URL
        /// </summary>
        public string LogoURL { get; set; }

        /// <summary>
        /// Tenant name
        /// </summary>
        public string TenantName { get; set; }

        public string PaymentRef { get; set; }

        public List<RevenueHeadDropDownListViewModel> RevenueHeads { get; set; }

        public string SelectedRevenueHead { get; set; }

        public string ReceiptNumber { get; set; }

        public List<CommandVM> Commands { get; set; }

        public string SelectedCommand { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }
    }
}