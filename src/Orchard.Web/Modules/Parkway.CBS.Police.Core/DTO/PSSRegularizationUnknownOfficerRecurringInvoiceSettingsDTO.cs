using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO
    {
        public long Id { get; set; }

        public long GenerateRequestWithoutOfficersUploadBatchStagingId { get; set; }

        public long EscortDetailId { get; set; }

        public long RequestId { get; set; }

        public int WeekDayNumber { get; set; }

        public int OffSet { get; set; }

        public int PaymentBillingType { get; set; }

        public string CronExpression { get; set; }

        public DateTime NextInvoiceGenerationDate { get; set; }
    }
}