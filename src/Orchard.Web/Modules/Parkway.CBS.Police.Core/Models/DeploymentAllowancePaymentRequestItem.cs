using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class DeploymentAllowancePaymentRequestItem : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual DeploymentAllowancePaymentRequest DeploymentAllowancePaymentRequest { get; set; }

        public virtual string AccountName { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual Bank Bank { get; set; }

        public virtual decimal Amount { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentRequestStatus"/>
        /// </summary>
        public virtual int TransactionStatus { get; set; }

        public virtual CommandType CommandType { get; set; }

        public virtual PSSEscortDayType DayType { get; set; }

        public virtual string PaymentReference { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }
    }
}