using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementBatchItems : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSSettlement Settlement { get; set; }

        public virtual PSSSettlementBatch Batch { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual PSSSettlementFeeParty SettlementFeeParty { get; set; }

        public virtual PSSFeeParty FeeParty { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual decimal TransactionAmount { get; set; }

        public virtual decimal AmountSettled { get; set; }

        public virtual decimal FeePercentage { get; set; }

        public virtual DateTime TransactionDate { get; set; }

        public virtual DateTime PaymentDate { get; set; }

        public virtual DateTime SynchronizationDate { get; set; }

        //nullable
        public virtual Command GeneratedByCommand { get; set; }

        //nullable
        public virtual Command StateCommand { get; set; }

        //nullable
        public virtual Command ZonalCommand { get; set; }

        public virtual PSService Service { get; set; }

        public virtual PSSRequest Request { get; set; }

        //nullable
        public virtual StateModel State { get; set; }

        //nullable
        public virtual LGA LGA { get; set; }

        public virtual string FeePartyName { get; set; }

        public virtual ExternalPaymentProvider PaymentProvider { get; set; }

        public virtual int PaymentChannel { get; set; }

        public virtual DateTime SettlementDate { get; set; }

        public virtual string AdditionalSplitValue { get; set; }

        public virtual PSSSettlementFeePartyBatchAggregate SettlementFeePartyBatchAggregate { get; set; }

        public virtual Command SettlementCommand { get; set; }
    }
}