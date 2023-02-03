using System;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSCollectionSearchParams
    {

        public string PaymentRef { get; set; }

        public int RevenueHeadId { get; set; }

        public string InvoiceNumber { get; set; }

        public string ReceiptNumber { get; set; }

        public CollectionPaymentDirection PaymentDirection { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int AdminUserId { get; set; }

        public bool DontPageData { get; set; }

        public string FileNumber { get; set; }

        public int CommandId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SelectedRevenueHead { get; set; }

        public string SelectedCommand { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

    }
}