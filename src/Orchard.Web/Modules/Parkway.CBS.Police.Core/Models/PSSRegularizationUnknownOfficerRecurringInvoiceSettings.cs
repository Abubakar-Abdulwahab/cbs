using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRegularizationUnknownOfficerRecurringInvoiceSettings : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual GenerateRequestWithoutOfficersUploadBatchStaging GenerateRequestWithoutOfficersUploadBatchStaging { get; set; }

        public virtual PSSEscortDetails EscortDetails { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual int WeekDayNumber { get; set; }

        public virtual int OffSet { get; set; }

        /// <summary>
        /// <see cref="Enums.PSBillingType"/>
        /// </summary>
        public virtual int PaymentBillingType { get; set; }

        public virtual string CronExpression { get; set; }

        public virtual DateTime NextInvoiceGenerationDate { get; set; }
    }
}